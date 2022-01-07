using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EmailHostingTemplates{
		private const string MASS_EMAIL_LOG_DIR="MassEmail";
		#region Get Methods

		///<summary>Returns an instance of the account api for the given Clinic Num.</summary>
		public static IAccountApi GetAccountApi(long clinicNum) {
			string guid=ClinicPrefs.GetPrefValue(PrefName.MassEmailGuid,clinicNum);
			string secret=ClinicPrefs.GetPrefValue(PrefName.MassEmailSecret,clinicNum);
			if(ODBuild.IsDebug()) {
				return AccountApiMock.Get(clinicNum,guid,secret);
			}
			string emailHostingEndpoint=PrefC.GetString(PrefName.EmailHostingEndpoint);
			return new AccountApi(guid,secret,emailHostingEndpoint);
		}

		///<summary>Syncs for ALL clinics. Call overlaod if only one is deisred.</summary>
		public static void SyncWithHq() {
			List<EmailHostingTemplate> listHostingTemplatesAll=Refresh();
			List<long> listClinicNums=new List<long> { 0 };
			if(PrefC.HasClinicsEnabled) {
				listClinicNums.AddRange(Clinics.GetDeepCopy().Select(x => x.ClinicNum));
			}
			foreach(long clinicNum in listClinicNums) {
				try {
					SyncWithHq(clinicNum,listHostingTemplatesAll.FindAll(x => x.ClinicNum==clinicNum));
				}
				catch(Exception ex) {
					Logger.WriteLine(Lans.g("MassEmail","Unable to sync MassEmail templates for clinicNum:")+" "+clinicNum+".\n"+MiscUtils.GetExceptionText(ex)
						,ODFileUtils.CombinePaths(MASS_EMAIL_LOG_DIR,clinicNum.ToString()));
				}
			}
		}

		///<summary>Syncs the EmailHostingTemplates in the database with those found at the EmailHosting api. Per Clinic.</summary>
		private static void SyncWithHq(long clinicNum,List<EmailHostingTemplate> listDatabaseTemplates) {
			if(!Clinics.IsMassEmailEnabled(clinicNum)) {
				return;
			}
			IAccountApi api=GetAccountApi(clinicNum);
			//todo, check if credentials were available, otherwise skip.
			string logSubDir=ODFileUtils.CombinePaths("EmailHostingTemplates",clinicNum.ToString());
			#region Update Database Templates to match API (exists in database, not in API)
			listDatabaseTemplates.RemoveAll(x => x.ClinicNum!=clinicNum);
			#region Remove Birthday templates that do not have an Appointment Reminder Rule
			List<ApptReminderRule> listBirthdayRules=ApptReminderRules.GetForTypes(ApptReminderType.Birthday).Where(x => x.ClinicNum==clinicNum).ToList();
			List<EmailHostingTemplate> listBirthdayTemplates=listDatabaseTemplates.FindAll(x => x.TemplateType==PromotionType.Birthday);
			List<long> listNoRules=listBirthdayTemplates
				.FindAll(x => !ListTools.In(x.EmailHostingTemplateNum,listBirthdayRules.Select(y => y.EmailHostingTemplateNum)))
				.Select(z => z.EmailHostingTemplateNum)
				.ToList();
			listDatabaseTemplates.RemoveAll(x => ListTools.In(x.EmailHostingTemplateNum,listNoRules));//Remove orphaned templates
			foreach(long templateNum in listNoRules) {
				Delete(templateNum);
			}
			#endregion
			string GetHtmlBody(EmailHostingTemplate template) {
				return template.EmailTemplateType==EmailType.Html 
					? MarkupEdit.TranslateToXhtml(template.BodyHTML,true,false,true) 
					: template.BodyHTML;
			}
			#endregion
			#region Get API templates
			GetAllTemplatesByAccountRequest request=new GetAllTemplatesByAccountRequest();
			GetAllTemplatesByAccountResponse accountResponse=api.GetAllTemplatesByAccount(request);
			List<long> listTemplatesRemoving=new List<long>();
			listTemplatesRemoving.AddRange(listNoRules);//if any templates deleted above we'll also want to remove their API info
			foreach(long key in accountResponse.DictionaryTemplates.Keys) {
				if(!listDatabaseTemplates.Any(x => x.TemplateId==key)) {
					//no database template exists for this api template. Remove it. 
					listTemplatesRemoving.Add(key);
				}
			}
			#endregion
			#region Re-create templates that have previously existed in the API but are now missing for some reason
			foreach(EmailHostingTemplate dbTemplate in listDatabaseTemplates) {
				bool existsApi=accountResponse.DictionaryTemplates.TryGetValue(dbTemplate.TemplateId,out Template apiTemplate);
				string htmlBody=GetHtmlBody(dbTemplate);
				if(existsApi && 
					(apiTemplate.TemplateSubject!=dbTemplate.Subject 
					|| apiTemplate.TemplateBodyPlainText!=dbTemplate.BodyPlainText
					|| apiTemplate.TemplateBodyHtml!=htmlBody)) 
				{
					//The template exists at the api but is different then the database. Always assume the database is correct.
					//This may happen because we had a bug where templates were syncing to the EmailHosting API as wiki html.
					UpdateTemplateResponse response=api.UpdateTemplate(new UpdateTemplateRequest {
						TemplateNum=dbTemplate.TemplateId,
						Template=new Template {
							TemplateName=dbTemplate.TemplateName,
							TemplateSubject=dbTemplate.Subject,
							TemplateBodyPlainText=dbTemplate.BodyPlainText,
							TemplateBodyHtml=htmlBody,
						},
					});
				}
				else if(!existsApi) {
					//template exists in database, but no api template exists. It must have been deleted from the api somehow on accident, or never made it in there.
					CreateTemplateResponse response=api.CreateTemplate(new CreateTemplateRequest{
						Template=new Template{
							TemplateName=dbTemplate.TemplateName,
							TemplateBodyHtml=htmlBody,
							TemplateBodyPlainText=dbTemplate.BodyPlainText,
							TemplateSubject=dbTemplate.Subject,
						},
					});
					if(response.TemplateNum==0) {
						Logger.WriteError(Lans.g("EmailHostingTemplates","Upload failed for EmailHostingTemplateNum:")+" "+dbTemplate.TemplateName,logSubDir);
						continue;
					}
					dbTemplate.TemplateId=response.TemplateNum;
					Update(dbTemplate);
				}
			}
			#endregion
			#region Remove templates that exist in API but not in the Database
			foreach(long id in listTemplatesRemoving) {
				try {
					api.DeleteTemplate(new DeleteTemplateRequest {
						TemplateNum=id,
					});
				}
				catch (Exception ex) {
					ex.DoNothing();
				}
			}
			#endregion
			#region Sync Email Signatures
			string plainTextSignature=GetSignature(clinicNum,isHtml:false);
			string htmlSignature=GetSignature(clinicNum,isHtml:true);
			UpdateSignatureRequest updateSignatureRequest=new UpdateSignatureRequest(){ SignatureHtml=htmlSignature, SignaturePlainText=plainTextSignature };
			UpdateSignatureResponse updateSignatureResponse=api.UpdateSignature(updateSignatureRequest);
			//Update the cache in case we just uploaded default/generic signatures.
			if(clinicNum==0) {
				Prefs.UpdateString(PrefName.EmailHostingSignaturePlainText,plainTextSignature);
				Prefs.UpdateString(PrefName.EmailHostingSignatureHtml,htmlSignature);
			}
			else {
				ClinicPrefs.Upsert(PrefName.EmailHostingSignaturePlainText,clinicNum,plainTextSignature);
				ClinicPrefs.Upsert(PrefName.EmailHostingSignatureHtml,clinicNum,htmlSignature);
			}
			#endregion
		}

		///<summary>Returns the configured EmailHosting email signature for the given clinic, or creates one from clinic/practice settings if signature
		///is not yet configured.</summary>
		public static string GetSignature(long clinicNum,bool isHtml) {
			string signature=null;
			//Gets the practice preference if not found for this clinic.
			if(isHtml) {
				signature=ClinicPrefs.GetPrefValue(PrefName.EmailHostingSignatureHtml,clinicNum);
			}
			else {
				signature=ClinicPrefs.GetPrefValue(PrefName.EmailHostingSignaturePlainText,clinicNum);
			}
			if(signature.IsNullOrEmpty()) {
				signature=GetGenericSignature(clinicNum,isHtml);
			}
			return signature;
		}

		///<summary>Returns a generic email signature based on clinic/practice settings.</summary>
		private static string GetGenericSignature(long clinicNum,bool isHtml) {
			Clinic clinic=Clinics.GetClinic(clinicNum);
			if(clinic is null) {
				clinic=Clinics.GetPracticeAsClinicZero();//Get the default/HQ clinic
			}
			string description=GetSignatureField(clinic.Description);
			string city=GetSignatureField(clinic.City,",");
			string state=GetSignatureField(clinic.State,"");
			string zip=GetSignatureField(clinic.Zip,"");
			string cityStateZip=GetSignatureField($"{city} {state} {zip}");
			string phoneWithFormat=TelephoneNumbers.ReFormat(clinic.Phone);
			string phone="";
			if(!string.IsNullOrWhiteSpace(phoneWithFormat)) {
				phone="PH: "+GetSignatureField(phoneWithFormat,"");
			}
			if(isHtml) {
				if(!string.IsNullOrWhiteSpace(description)) {
					description=$"<b>{description}</b>";
				}
				if(!string.IsNullOrWhiteSpace(phoneWithFormat)) {
					phone="PH: <i>"+GetSignatureField(phoneWithFormat,"")+"</i>";
				}
			}
			string signature=description+GetSignatureField(clinic.Address)+GetSignatureField(clinic.Address2)+cityStateZip+phone;
			if(isHtml) {
				signature=signature.Replace("\r\n","<br/>");
			}
			return signature;
		}

		///<summary>Returns blank string if field is blank. Otherwise, returns field with separator.</summary> 
		private static string GetSignatureField(string field,string separator="\r\n") {
			if(string.IsNullOrWhiteSpace(field)) {
				return "";
			}
			return field+separator;
		}

		///<summary></summary>
		public static List<EmailHostingTemplate> Refresh(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EmailHostingTemplate>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM emailhostingtemplate";
			return Crud.EmailHostingTemplateCrud.SelectMany(command);
		}

		///<summary>Gets one EmailHostingTemplate from the db.</summary>
		public static EmailHostingTemplate GetOne(long emailHostingTemplateNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EmailHostingTemplate>(MethodBase.GetCurrentMethod(),emailHostingTemplateNum);
			}
			return Crud.EmailHostingTemplateCrud.SelectOne(emailHostingTemplateNum);
		}

		public static List<EmailHostingTemplate> GetMany(List<long> listEmailHostingTemplateNums) {
			if(listEmailHostingTemplateNums.IsNullOrEmpty()) {
				return new List<EmailHostingTemplate>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EmailHostingTemplate>>(MethodBase.GetCurrentMethod(),listEmailHostingTemplateNums);
			}
			string command=@$"SELECT * FROM emailhostingtemplate WHERE EmailHostingTemplateNum IN (
				{string.Join(",",listEmailHostingTemplateNums.Select(x => POut.Long(x)).Distinct())})";
			return Crud.EmailHostingTemplateCrud.SelectMany(command);
		}

		/// <summary>Makes and returns a list of example mass email templates.</summary>
		public static List<EmailHostingTemplate> GetExamples() {
			List<EmailHostingTemplate> listEmailHostingTemplates=new List<EmailHostingTemplate>();
			//COVID-19: Office Re-Opening (option 1) Template
			EmailHostingTemplate emailHostingTemplate=new EmailHostingTemplate();
			emailHostingTemplate.TemplateName="COVID-19: Office Re-Opening (option 1)";
			emailHostingTemplate.Subject="[{[{ OfficeName }]}] Reopening Update";
			emailHostingTemplate.BodyHTML=@"Good morning, 

We want to extend a thank you to all of our patients affected by the closure of our office for your patience during these unprecedented times. We are looking forward to seeing you again. 

Our office will be re-opening soon. If you have not rescheduled your previous appointment, please contact us to do so.

We will be taking many precautions as we re-open, following local and state guidelines. This includes social distancing measures and increased sanitizing of the office.

Thank you, 
[{[{ OfficeName }]}]
[{[{ OfficePhone }]}]";
			emailHostingTemplate.BodyPlainText=@"Good morning, 

We want to extend a thank you to all of our patients affected by the closure of our office for your patience during these unprecedented times. We are looking forward to seeing you again. 

Our office will be re-opening soon. If you have not rescheduled your previous appointment, please contact us to do so.

We will be taking many precautions as we re-open, following local and state guidelines. This includes social distancing measures and increased sanitizing of the office.

Thank you, 
[{[{ OfficeName }]}]
[{[{ OfficePhone }]}]";
			emailHostingTemplate.EmailTemplateType=EmailType.Html;
			emailHostingTemplate.TemplateType=PromotionType.Manual;
			listEmailHostingTemplates.Add(emailHostingTemplate);
			//COVID-19: Office Re-Opening (option 2) Template
			emailHostingTemplate=new EmailHostingTemplate();
			emailHostingTemplate.TemplateName="COVID-19: Office Re-Opening (option 2)";
			emailHostingTemplate.Subject="[{[{ OfficeName }]}] Re-Opening Update";
			emailHostingTemplate.BodyHTML=@"Good morning,

Thank you for your patience during these unprecedented times, 
We are happy to announce that [{[{ OfficeName }]}] will be re-opening for non-urgent procedures soon.

Our hours of operation are Monday through Friday, 8:00 AM to 5:00 PM. 

If you have not yet rescheduled your appointment, please contact us. 

We will be taking many precautions as we re-open, following local and state guidelines. This includes social distancing measures and increased sanitizing of the office.

Thank you,
[{[{ OfficeName }]}]
[{[{ OfficePhone }]}]";
			emailHostingTemplate.BodyPlainText=@"Good morning,

Thank you for your patience during these unprecedented times, 
We are happy to announce that [{[{ OfficeName }]}] will be re-opening for non-urgent procedures soon. 

Our hours of operation are Monday through Friday, 8:00 AM to 5:00 PM.

If you have not yet rescheduled your appointment, please contact us. 

We will be taking many precautions as we re-open, following local and state guidelines. This includes social distancing measures and increased sanitizing of the office.

Thank you,
[{[{ OfficeName }]}]
[{[{ OfficePhone }]}]";
			emailHostingTemplate.EmailTemplateType=EmailType.Html;
			emailHostingTemplate.TemplateType=PromotionType.Manual;
			listEmailHostingTemplates.Add(emailHostingTemplate);
			//Generic Promotion: Percentage off Treatment Template
			emailHostingTemplate=new EmailHostingTemplate();
			emailHostingTemplate.TemplateName="Generic Promotion: Percentage off Treatment";
			emailHostingTemplate.Subject="Special Treatment Offer";
			emailHostingTemplate.BodyHTML=@"Want a whiter smile? [{[{ OfficeName }]}] is now offering 15% off whitening treatments, now through the end of the month.
Give us a call to schedule an appointment today!";
			emailHostingTemplate.BodyPlainText=@"Want a whiter smile? [{[{ OfficeName }]}] is now offering 15% off whitening treatments, now through the end of the month.
Give us a call to schedule an appointment today!";
			emailHostingTemplate.EmailTemplateType=EmailType.Html;
			emailHostingTemplate.TemplateType=PromotionType.Manual;
			listEmailHostingTemplates.Add(emailHostingTemplate);
			//Patient Portal Template
			emailHostingTemplate=new EmailHostingTemplate();	
			emailHostingTemplate.TemplateName="Patient Portal";
			emailHostingTemplate.Subject="Patient Portal";
			emailHostingTemplate.BodyHTML=@"Did you know that you can view recommended treatment, make payments, and communicate with your provider, all through our secure Patient Portal? 
Contact us at [{[{ OfficePhone }]}] today for access to your Patient Portal account. 
Thanks!";
			emailHostingTemplate.BodyPlainText=@"Did you know that you can view recommended treatment, make payments, and communicate with your provider, all through our secure Patient Portal? 
Contact us at [{[{ OfficePhone }]}] today for access to your Patient Portal account. 
Thanks!";
			emailHostingTemplate.EmailTemplateType=EmailType.Html;
			emailHostingTemplate.TemplateType=PromotionType.Manual;
			listEmailHostingTemplates.Add(emailHostingTemplate);
			//End of Year Insurance Template
			emailHostingTemplate=new EmailHostingTemplate();	
			emailHostingTemplate.TemplateName="End of Year Insurance";
			emailHostingTemplate.Subject="End of Year Insurance";
			emailHostingTemplate.BodyHTML=@"Your insurance benefits will renew soon.  You have insurance remaining amounts that can be applied towards important dental treatment.  
Our records show that you have treatment that still needs to be completed.
Please call our office at your earliest convenience to schedule an appointment.";
			emailHostingTemplate.BodyPlainText=@"Your insurance benefits will renew soon.  You have insurance remaining amounts that can be applied towards important dental treatment.  Our records show that you have treatment that still needs to be completed.
Please call our office at your earliest convenience to schedule an appointment.";
			emailHostingTemplate.EmailTemplateType=EmailType.Html;
			emailHostingTemplate.TemplateType=PromotionType.Manual;
			listEmailHostingTemplates.Add(emailHostingTemplate);
			//Birthday Template
			emailHostingTemplate=new EmailHostingTemplate();		
			emailHostingTemplate.TemplateName="Birthday";
			emailHostingTemplate.Subject="Happy Birthday!";
			emailHostingTemplate.BodyHTML=@"<b>[[font:verdana|Happy Birthday!]]</b>

Wishing you a happy and healthy Birthday! 
Hope your day is full of smiles and memorable moments. 

From your friends at
[{[{ OfficeName }]}]";
			emailHostingTemplate.BodyPlainText=@"Wishing you a happy and healthy Birthday! 
Hope your day is full of smiles and memorable moments. 

From your friends at
[{[{ OfficeName }]}]";
			emailHostingTemplate.EmailTemplateType=EmailType.Html;
			emailHostingTemplate.TemplateType=PromotionType.Manual;
			listEmailHostingTemplates.Add(emailHostingTemplate);
			//COVID-19: Office Re-Opening (option 3) Template 
			emailHostingTemplate=new EmailHostingTemplate();			
			emailHostingTemplate.TemplateName="COVID-19: Office Re-Opening (option 3)";
			emailHostingTemplate.Subject="[{[{ OfficeName }]}] Re-Opening Update";
			emailHostingTemplate.BodyHTML=@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
  <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
  <title>We Are Open!</title>
  <style type=""text/css"">
    @import url(http://fonts.googleapis.com/css?family=Lato:400);

    /* Take care of image borders and formatting */

    img {
      max-width: 600px;
      outline: none;
      text-decoration: none;
      -ms-interpolation-mode: bicubic;
    }

    a {
      text-decoration: none;
      border: 0;
      outline: none;
      color: #21BEB4;
    }

    a img {
      border: none;
    }

    /* General styling */

    td, h1, h2, h3  {
      font-family: Helvetica, Arial, sans-serif;
    }
	
    td  {
      font-weight: 400;
    }

    body {
      -webkit-font-smoothing:antialiased;
      -webkit-text-size-adjust:none;
      width: 100%;
      height: 100%;
      color: #37302d;
      background: #ffffff;
    }

    table {
      background:
    }

    h1, h2, h3 {
      padding: 0;
      margin: 0;
      color: #ffffff;
      font-weight: bold;
    }
	
	h1.black, h2.black, h3.black {
		color: inherit;
	}
	
	h1 {
		font-size: 27px;
	}

    h3 {
      color: #21c5ba;
      font-size: 24px;
    }
  </style>

  <style type=""text/css"" media=""screen"">
    @media screen {
       /* Thanks Outlook 2013! http://goo.gl/XLxpyl*/
      td, h1, h2, h3 {
        font-family: 'Lato', 'Helvetica Neue', 'Arial', 'sans-serif' !important;
      }
    }
  </style>

  <style type=""text/css"" media=""only screen and (max-width: 480px)"">
    /* Mobile styles */
    @media only screen and (max-width: 480px) {
      table[class=""w320""] {
        width: 320px !important;
      }

      table[class=""w300""] {
        width: 300px !important;
      }

      table[class=""w290""] {
        width: 290px !important;
      }

      td[class=""w320""] {
        width: 320px !important;
      }

      td[class=""mobile-center""] {
        text-align: center !important;
      }

      td[class=""mobile-padding""] {
        padding-left: 20px !important;
        padding-right: 20px !important;
        padding-bottom: 20px !important;
      }
    }
  </style>
</head>
<body class=""body"" style=""padding:0; margin:0; display:block; background:#ffffff; -webkit-text-size-adjust:none"" bgcolor=""#ffffff"">
<table align=""center"" cellpadding=""0"" cellspacing=""0"" width=""100%"" height=""100%"" >
  <tr>
    <td align=""center"" valign=""top"" bgcolor=""#ffffff""  width=""100%"">

    <table cellspacing=""0"" cellpadding=""0"" width=""100%"">
      <tr>
        <td width=""100%"">
          <center>
            <table cellspacing=""0"" cellpadding=""0"" width=""500"" class=""w320"">
              <tr>
                <td valign=""top"" style=""padding:10px 0; text-align:center;"" class=""mobile-center"">
                  <img width=""160"" src=""https://www.opendental.com/images/logos/templateLogoSign.png"">
                </td>
              </tr>
            </table>
          </center>
        </td>
      </tr>
      <tr>
        <td valign=""top"">
          <!--[if gte mso 9]>
          <v:rect xmlns:v=""urn:schemas-microsoft-com:vml"" fill=""true"" stroke=""false"" style=""mso-width-percent:1000;height:303px;"">
            <v:fill type=""tile"" src=""https://www.opendental.com/images/logos/templateopensign.png"" color=""#64594b"" />
            <v:textbox inset=""0,0,0,0"">
          <![endif]-->
          <div>
            <center>
							<table cellspacing=""0"" cellpadding=""0"" width=""530"" height=""375"" class=""w320"">
                <tr>
                  <td>
										<img style=""width: 100%;"" src=""https://www.opendental.com/images/logos/templateopensign.png"">
									</td>
                </tr>
              </table>
            </center>
          </div>
          <!--[if gte mso 9]>
            </v:textbox>
          </v:rect>
          <![endif]-->
        </td>
      </tr>
      <tr>
        <td valign=""top"">
          <center>
            <table cellspacing=""0"" cellpadding=""0"" width=""500"" class=""w320"">
              <tr>
                <td>

                  <table cellspacing=""0"" cellpadding=""0"" width=""100%"">
                    <tr>
                      <td class=""mobile-padding"" style=""text-align:left;"">
                        <h1 class=""black"">We will be seeing patients again soon!</h1><br>
						Good morning,<br><br>
 
						We want to extend a thank you to all of our patients affected by the closure of our office for your patience during these unprecedented times. We are looking forward to seeing you again.<br><br>
						 
						Our office will be re-opening soon. If you have not rescheduled your previous appointment, please contact us to do so.<br><br>

						We will be taking many precautions as we re-open, following local and state guidelines. This includes social distancing measures and increased sanitizing of the office. Please check our <a href=""https://www.opendental.com"">website</a> for updates.<br><br>

						Thank you, and we look forward to seeing you soon,<br><br><br>


						Dr. Jane Smith<br>
						[{[{ OfficeName }]}]<br>
						[{[{ OfficePhone }]}]<br>
                      </td>
                    </tr>
					<tr>
                      <td height=""50"">
					  &nbsp;
					  </td>
					</tr>
                  </table>
                </td>
              </tr>
              <tr>
              </tr>
            </table>
          </center>
        </td>
      </tr>
      <tr>
      </tr>
    </table>
    </td>
  </tr>
</table>
</body>
</html>";
			emailHostingTemplate.BodyPlainText=@"Good morning,

We want to extend a thank you to all of our patients affected by the closure of our office for your patience during these unprecedented times. We are looking forward to seeing you again.

Our office will be re-opening soon. If you have not rescheduled your previous appointment, please contact us to do so.

We will be taking many precautions as we re-open, following local and state guidelines. This includes social distancing measures and increased sanitizing of the office. Please check our website for updates.

Thank you, and we look forward to seeing you soon,


Dr. Jane Smith
[{[{ OfficeName }]}]
[{[{ OfficePhone }]}]";
			emailHostingTemplate.EmailTemplateType=EmailType.RawHtml;
			emailHostingTemplate.TemplateType=PromotionType.Manual;
			listEmailHostingTemplates.Add(emailHostingTemplate);
			return listEmailHostingTemplates;
		}
		#endregion Get Methods

		#region Modification Methods
		///<summary></summary>
		public static long Insert(EmailHostingTemplate emailHostingTemplate){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				emailHostingTemplate.EmailHostingTemplateNum=Meth.GetLong(MethodBase.GetCurrentMethod(),emailHostingTemplate);
				return emailHostingTemplate.EmailHostingTemplateNum;
			}
			return Crud.EmailHostingTemplateCrud.Insert(emailHostingTemplate);
		}

		public static EmailHostingTemplate CreateDefaultTemplate(long clinicNum,PromotionType templateType) 
		{
			EmailHostingTemplate template=new EmailHostingTemplate();
			template.ClinicNum=clinicNum;
			template.Subject="Happy Birthday";
			template.BodyPlainText="Wishing you a happy and healthy Birthday! Hope your day is full of smiles and memorable moments. " +
				"From your friends at [{[{ OfficeName }]}]";
			template.BodyHTML="";
			template.EmailTemplateType=EmailType.Regular;
			template.TemplateName="Automated Birthday Message";
			template.TemplateType=templateType;
			return template;
		}

		///<summary></summary>
		public static void Update(EmailHostingTemplate emailHostingTemplate){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),emailHostingTemplate);
				return;
			}
			Crud.EmailHostingTemplateCrud.Update(emailHostingTemplate);
		}

		///<summary></summary>
		public static void Delete(long emailHostingTemplateNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),emailHostingTemplateNum);
				return;
			}
			Crud.EmailHostingTemplateCrud.Delete(emailHostingTemplateNum);
		}
		#endregion Modification Methods

		#region Misc Methods
		public static bool IsBlank(EmailHostingTemplate template) {
			return string.IsNullOrEmpty(template.Subject) && string.IsNullOrEmpty(template.BodyHTML) && string.IsNullOrEmpty(template.BodyPlainText);
		}

		///<summary>Returns a list of the replacements in the given string. Will return the inner key without the outside brackets.</summary>
		public static List<string> GetListReplacements(string subjectOrBody) {
			if(string.IsNullOrWhiteSpace(subjectOrBody)) {
				return new List<string>();
			}
			List<string> retVal=new List<string>();
			foreach(Match match in Regex.Matches(subjectOrBody,@"\[{\[{\s?([A-Za-z0-9]*)\s?}\]}\]")) {
				retVal.Add(match.Groups[1].Value.Trim());
			}
			return retVal;
		}

		#endregion Misc Methods

		#region Cache Pattern
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class EmailHostingTemplateCache : CacheListAbs<EmailHostingTemplate> {
			protected override List<EmailHostingTemplate> GetCacheFromDb() {
				string command="SELECT * FROM emailhostingtemplate";
				return Crud.EmailHostingTemplateCrud.SelectMany(command);
			}
			protected override List<EmailHostingTemplate> TableToList(DataTable table) {
				return Crud.EmailHostingTemplateCrud.TableToList(table);
			}
			protected override EmailHostingTemplate Copy(EmailHostingTemplate emailHostingTemplate) {
				return emailHostingTemplate.Copy();
			}
			protected override DataTable ListToTable(List<EmailHostingTemplate> listEmailHostingTemplates) {
				return Crud.EmailHostingTemplateCrud.ListToTable(listEmailHostingTemplates,"EmailHostingTemplate");
			}
			protected override void FillCacheIfNeeded() {
				EmailHostingTemplates.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static EmailHostingTemplateCache _emailHostingTemplateCache=new EmailHostingTemplateCache();

		public static List<EmailHostingTemplate> GetDeepCopy(bool isShort=false) {
			return _emailHostingTemplateCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _emailHostingTemplateCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<EmailHostingTemplate> match,bool isShort=false) {
			return _emailHostingTemplateCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<EmailHostingTemplate> match,bool isShort=false) {
			return _emailHostingTemplateCache.GetFindIndex(match,isShort);
		}

		public static EmailHostingTemplate GetFirst(bool isShort=false) {
			return _emailHostingTemplateCache.GetFirst(isShort);
		}

		public static EmailHostingTemplate GetFirst(Func<EmailHostingTemplate,bool> match,bool isShort=false) {
			return _emailHostingTemplateCache.GetFirst(match,isShort);
		}

		public static EmailHostingTemplate GetFirstOrDefault(Func<EmailHostingTemplate,bool> match,bool isShort=false) {
			return _emailHostingTemplateCache.GetFirstOrDefault(match,isShort);
		}

		public static EmailHostingTemplate GetLast(bool isShort=false) {
			return _emailHostingTemplateCache.GetLast(isShort);
		}

		public static EmailHostingTemplate GetLastOrDefault(Func<EmailHostingTemplate,bool> match,bool isShort=false) {
			return _emailHostingTemplateCache.GetLastOrDefault(match,isShort);
		}

		public static List<EmailHostingTemplate> GetWhere(Predicate<EmailHostingTemplate> match,bool isShort=false) {
			return _emailHostingTemplateCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_emailHostingTemplateCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_emailHostingTemplateCache.FillCacheFromTable(table);
				return table;
			}
			return _emailHostingTemplateCache.GetTableFromCache(doRefreshCache);
		}
		*/
		#endregion
	}
}