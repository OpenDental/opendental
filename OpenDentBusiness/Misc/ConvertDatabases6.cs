using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace OpenDentBusiness {
	public partial class ConvertDatabases {

		#region Helper Functions/Variables
		///<summary>These two lists are for To19_2_15 and are meant to be static lists of table names as of that version.</summary>
		private static List<string> _listTableNames=new List<string> {
			"account","accountingautopay","adjustment","alertcategory","alertcategorylink","alertitem","alertread","alertsub","allergy","allergydef",
			"anestheticdata","anestheticrecord","anesthmedsgiven","anesthmedsintake","anesthmedsinventory","anesthmedsinventoryadj","anesthmedsuppliers",
			"anesthscore","anesthvsdata","appointmentrule","appointmenttype","apptfield","apptfielddef","apptreminderrule","apptremindersent","apptview",
			"apptviewitem","asapcomm","autocode","autocodecond","autocodeitem","automation","automationcondition","autonote","autonotecontrol","benefit",
			"bugsubmission","canadiannetwork","carrier","cdcrec","cdspermission","centralconnection","chartview","chatuser","claim","claimattach",
			"claimcondcodelog","claimform","claimformitem","claimpayment","claimsnapshot","claimtracking","claimvalcodelog","clearinghouse","clinic",
			"clinicerx","clinicpref","clockevent","codesystem","commoptout","computer","computerpref","confirmationrequest","connectiongroup",
			"conngroupattach","contact","county","covcat","covspan","cpt","creditcard","custrefentry","custreference","cvx","dashboardar","dashboardcell",
			"dashboardlayout","databasemaintenance","dbmlog","definition","deflink","deletedobject","deposit","dictcustom","discountplan","disease",
			"diseasedef","displayfield","displayreport","dispsupply","documentmisc","drugmanufacturer","drugunit","dunning","ebill","eclipboardsheetdef",
			"eduresource","ehramendment","ehraptobs","ehrcareplan","ehrlab","ehrlabclinicalinfo","ehrlabimage","ehrlabnote","ehrlabresult",
			"ehrlabresultscopyto","ehrlabspecimen","ehrlabspecimencondition","ehrlabspecimenrejectreason","ehrmeasure","ehrmeasureevent","ehrnotperformed",
			"ehrpatient","ehrprovkey","ehrquarterlykey","ehrsummaryccd","ehrtrigger","electid","emailaddress","emailattach","emailautograph",
			"emailmessage","emailmessageuid","emailtemplate","employee","employer","encounter","entrylog","eobattach","equipment","erxlog",
			"eservicebilling","eservicecodelink","eservicesignal","etrans","etrans835attach","evaluation","evaluationcriterion","evaluationcriteriondef",
			"evaluationdef","famaging","familyhealth","faq","fee","feesched","fhircontactpoint","fhirsubscription","fielddeflink","files","formpat",
			"gradingscale","gradingscaleitem","grouppermission","guardian","hcpcs","hl7def","hl7deffield","hl7defmessage","hl7defsegment","hl7msg",
			"hl7procattach","icd10","icd9","insfilingcode","insfilingcodesubtype","insplan","installmentplan","instructor","insverify","insverifyhist",
			"intervention","job","jobcontrol","joblink","joblog","jobnote","jobnotification","jobpermission","jobproject","jobquote","jobreview",
			"jobsprint","jobsprintlink","journalentry","labcase","laboratory","labpanel","labresult","labturnaround","language","languageforeign","letter",
			"lettermerge","lettermergefield","loginattempt","loinc","maparea","medicalorder","medication","medicationpat","medlab","medlabfacattach",
			"medlabfacility","medlabresult","medlabspecimen","mobileappdevice","mount","mountdef","mountitem","mountitemdef","oidexternal","oidinternal",
			"operatory","orionproc","orthochart","orthocharttab","orthocharttablink","patfield","patfielddef","patientlink","patientnote",
			"patientportalinvite","patientrace","patplan","patrestriction","payconnectresponseweb","payment","payortype","payperiod","payplan",
			"payplancharge","paysplit","perioexam","pharmacy","pharmclinic","phone","phonecomp","phoneconf","phoneempdefault","phoneempsubgroup",
			"phonegraph","phonemetric","phonenumber","plannedappt","popup","preference","printer","procapptcolor","procbutton","procbuttonitem",
			"procbuttonquick","proccodenote","procedurecode","procgroupitem","procmultivisit","proctp","program","programproperty","provider",
			"providerclinic","providercliniclink","providererx","providerident","question","questiondef","quickpastecat","quickpastenote","reactivation",
			"recalltrigger","recalltype","reconcile","recurringcharge","refattach","referral","registrationkey","reminderrule","repeatcharge",
			"replicationserver","reqneeded","reqstudent","requiredfield","requiredfieldcondition","reseller","resellerservice","rxalert","rxdef","rxnorm",
			"rxpat","scheduledprocess","schoolclass","schoolcourse","screen","screengroup","screenpat","sheet","sheetdef","sheetfielddef","sigbutdef",
			"sigelementdef","sigmessage","signalod","site","sitelink","smsbilling","smsblockphone","smsfrommobile","smsphone","smstomobile","snomed","sop",
			"stateabbr","statement","stmtlink","substitutionlink","supplier","supply","supplyneeded","supplyorder","supplyorderitem","task","taskancestor",
			"taskhist","tasklist","tasknote","tasksubscription","tasktaken","taskunread","terminalactive","timeadjust","timecardrule","toolbutitem",
			"toothgridcell","toothgridcol","toothgriddef","transaction","treatplanattach","triagemetric","tsitranslog","ucum","updatehistory","userclinic",
			"usergroup","usergroupattach","userod","userodapptview","userodpref","userquery","userweb","vaccinedef","vaccineobs","vaccinepat","vitalsign",
			"voicemail","webchatmessage","webchatpref","webchatsession","webchatsurvey","webforms_log","webforms_preference","webforms_sheet",
			"webforms_sheetdef","webforms_sheetfield","webforms_sheetfielddef","webschedrecall","wikilistheaderwidth","wikilisthist","wikipage",
			"wikipagehist","xchargetransaction","xwebresponse","zipcode"
		};
		///<summary>These two lists are for To19_2_15 and are meant to be static lists of table names as of that version.</summary>
		private static List<string> _listLargeTableNames=new List<string> {
			"appointment","claimproc","commlog","document","etransmessagetext","histappointment","inseditlog","inssub","patient","periomeasure",
			"procedurelog","procnote","recall","schedule","scheduleop","securitylog","securityloghash","sheetfield","toothinitial","treatplan"
		};

		///<summary>Attempts to detach all claim procs with IsTransfer set to true from their corresponding claim payment.
		///Only detaches if the sum of all transfer claim procs (that are going to be detached) equate to $0.
		///This has the potential to orphan claim payments (claim payment with no claim procs attached).
		///These orphaned claim payments will need to be handled manually by the user (DBM ClaimPaymentsNotPartialWithNoClaimProcs).</summary>
		private static void DetachTransferClaimProcsFromClaimPayments() {
			//Get all ClaimNums attached to claimprocs flagged as IsTransfer that are also attached to a claimpayment.
			//The information regarding these claimprocs is being selected instead of updated so that we can perform calculations and handle rounding in C#
			//Several tricky rounding issues made the 'one query to rule them all' hard to read (and we just didn't trust MySQL to do our bidding).
			//Also, using a subselect was slow for MySQL 5.5 which is the officially supported version for Open Dental (v5.6 was fast).
			string command=@"
				SELECT DISTINCT claimproc.ClaimNum
				FROM claimproc
				WHERE claimproc.ClaimNum > 0
				AND claimproc.ClaimPaymentNum > 0
				AND claimproc.IsTransfer=1 ";
			List<long> listClaimNums=Db.GetListLong(command);
			if(listClaimNums.Count > 0) {
				List<long> listClaimProcNumsToUpdate=new List<long>();
				command=$@"
				SELECT claimproc.ClaimProcNum,claimproc.ClaimPaymentNum,claimproc.ClaimNum,claimproc.InsPayAmt,claimproc.IsTransfer
				FROM claimproc
				WHERE claimproc.ClaimNum IN({string.Join(",",listClaimNums.Select(x => POut.Long(x)))})
				AND claimproc.ClaimPaymentNum > 0";
				DataTable table=Db.GetTable(command);
				//Group the claimprocs by ClaimPaymentNum because we do not want to detach offsetting claimprocs that are attached to different claim payments.
				Dictionary<long,List<DataRow>> dictClaimPayNumClaimProcs=table.Select()
					.GroupBy(x => PIn.Long(x["ClaimPaymentNum"].ToString()))
					.ToDictionary(x => x.Key,x => x.ToList());
				//Loop through every claim payment to find and detach any offsetting claimproc transfers that are associated to the same claim.
				foreach(KeyValuePair<long,List<DataRow>> claimProcsForPayment in dictClaimPayNumClaimProcs) {
					//Group the claimprocs for this claim payment by claim because we do not want to detach offsetting claimprocs for different claims.
					Dictionary<long,List<DataRow>> dictClaimNumClaimProcs=claimProcsForPayment.Value
						.GroupBy(x => PIn.Long(x["ClaimNum"].ToString()))
						.ToDictionary(x => x.Key,x => x.ToList());
					//Loop through every claim and find claimproc transfers that can be detached (equate to 0).
					foreach(KeyValuePair<long,List<DataRow>> claimProcsForClaim in dictClaimNumClaimProcs) {
						double sumAllClaimProcsForClaim=claimProcsForClaim.Value.Sum(x => PIn.Double(x["InsPayAmt"].ToString()));
						double sumClaimProcsNoTransfers=claimProcsForClaim.Value
							.Where(x => PIn.Bool(x["IsTransfer"].ToString())==false)
							.Sum(x => PIn.Double(x["InsPayAmt"].ToString()));
						double sumClaimProcsTransfers=claimProcsForClaim.Value
							.Where(x => PIn.Bool(x["IsTransfer"].ToString())==true)
							.Sum(x => PIn.Double(x["InsPayAmt"].ToString()));
						//make sure that the transfer procs equate to 0, that removing them does not change the claimpayment amount
						if(AreNumsEqual(sumAllClaimProcsForClaim,sumClaimProcsNoTransfers) && IsNumZero(sumClaimProcsTransfers)) {
							List<long> listTransferClaimProcsForClaim=claimProcsForClaim.Value
								.Where(x => PIn.Bool(x["IsTransfer"].ToString())==true)
								.Select(y => PIn.Long(y["ClaimProcNum"].ToString()))
								.ToList();
							listClaimProcNumsToUpdate.AddRange(listTransferClaimProcsForClaim);
						}
					}
				}
				if(listClaimProcNumsToUpdate.Count>0) {
					command=$@"UPDATE claimproc SET claimproc.ClaimPaymentNum=0 
										WHERE claimproc.ClaimProcNum IN ({string.Join(",",listClaimProcNumsToUpdate.Select(x => POut.Long(x)))})";
					Db.NonQ(command);
				}
			}
		}

		private static Tuple<string,string,string,string> GetCovidOption1Template() {
			string name="COVID-19: Office Re-Opening (option 1)";
			string subject="[{[{ OfficeName }]}] Reopening Update";
			string bodyHtml=@"Good morning, 

We want to extend a thank you to all of our patients affected by the closure of our office for your patience during these unprecedented times. We are looking forward to seeing you again. 

Our office will be re-opening soon. If you have not rescheduled your previous appointment, please contact us to do so.

We will be taking many precautions as we re-open, following local and state guidelines. This includes social distancing measures and increased sanitizing of the office.

Thank you, 
[{[{ OfficeName }]}]
[{[{ OfficePhone }]}]";
			string bodyText=@"Good morning, 

We want to extend a thank you to all of our patients affected by the closure of our office for your patience during these unprecedented times. We are looking forward to seeing you again. 

Our office will be re-opening soon. If you have not rescheduled your previous appointment, please contact us to do so.

We will be taking many precautions as we re-open, following local and state guidelines. This includes social distancing measures and increased sanitizing of the office.

Thank you, 
[{[{ OfficeName }]}]
[{[{ OfficePhone }]}]";
			return new Tuple<string, string, string, string>(POut.String(name),POut.String(subject),POut.String(bodyText),POut.String(bodyHtml));
		}

		private static Tuple<string,string,string,string> GetCovidOption2Template() {
			string name="COVID-19: Office Re-Opening (option 2)";
			string subject="[{[{ OfficeName }]}] Re-Opening Update";
			string bodyHtml=@"Good morning,

Thank you for your patience during these unprecedented times, 
We are happy to announce that [{[{ OfficeName }]}] will be re-opening for non-urgent procedures soon.

Our hours of operation are Monday through Friday, 8:00 AM to 5:00 PM. 

If you have not yet rescheduled your appointment, please contact us. 

We will be taking many precautions as we re-open, following local and state guidelines. This includes social distancing measures and increased sanitizing of the office.

Thank you,
[{[{ OfficeName }]}]
[{[{ OfficePhone }]}]";
			string bodyText=@"Good morning,

Thank you for your patience during these unprecedented times, 
We are happy to announce that [{[{ OfficeName }]}] will be re-opening for non-urgent procedures soon. 

Our hours of operation are Monday through Friday, 8:00 AM to 5:00 PM.

If you have not yet rescheduled your appointment, please contact us. 

We will be taking many precautions as we re-open, following local and state guidelines. This includes social distancing measures and increased sanitizing of the office.

Thank you,
[{[{ OfficeName }]}]
[{[{ OfficePhone }]}]";
			return new Tuple<string, string, string, string>(POut.String(name),POut.String(subject),POut.String(bodyText),POut.String(bodyHtml));
		}

		private static Tuple<string,string,string,string> GetCovidOption3Template() {
			string name="COVID-19: Office Re-Opening (option 3)";
			string subject="[{[{ OfficeName }]}] Re-Opening Update";
			string bodyHtml=@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
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
			string bodyText=@"Good morning,

We want to extend a thank you to all of our patients affected by the closure of our office for your patience during these unprecedented times. We are looking forward to seeing you again.

Our office will be re-opening soon. If you have not rescheduled your previous appointment, please contact us to do so.

We will be taking many precautions as we re-open, following local and state guidelines. This includes social distancing measures and increased sanitizing of the office. Please check our website for updates.

Thank you, and we look forward to seeing you soon,


Dr. Jane Smith
[{[{ OfficeName }]}]
[{[{ OfficePhone }]}]";
			return new Tuple<string, string, string, string>(POut.String(name),POut.String(subject),POut.String(bodyText),POut.String(bodyHtml));
		}

		private static Tuple<string,string,string,string> GetPromotionTemplate() {
			string name="Generic Promotion: Percentage off Treatment";
			string subject="Special Treatment Offer";
			string bodyHtml=@"Want a whiter smile? [{[{ OfficeName }]}] is now offering 15% off whitening treatments, now through the end of the month.
Give us a call to schedule an appointment today!";
			string bodyText=@"Want a whiter smile? [{[{ OfficeName }]}] is now offering 15% off whitening treatments, now through the end of the month.
Give us a call to schedule an appointment today!";
			return new Tuple<string, string, string, string>(POut.String(name),POut.String(subject),POut.String(bodyText),POut.String(bodyHtml));
		}

		private static Tuple<string,string,string,string> GetPatientPortalTemplate() {
			string name="Patient Portal";
			string subject="Patient Portal";
			string bodyHtml=@"Did you know that you can view recommended treatment, make payments, and communicate with your provider, all through our secure Patient Portal? 
Contact us at [{[{ OfficePhone }]}] today for access to your Patient Portal account. 
Thanks!";
			string bodyText=@"Did you know that you can view recommended treatment, make payments, and communicate with your provider, all through our secure Patient Portal? 
Contact us at [{[{ OfficePhone }]}] today for access to your Patient Portal account. 
Thanks!";
			return new Tuple<string, string, string, string>(POut.String(name),POut.String(subject),POut.String(bodyText),POut.String(bodyHtml));
		}

		private static Tuple<string,string,string,string> GetBirthdayTemplate() {
			string name="Birthday";
			string subject="Happy Birthday!";
			string bodyHtml=@"<b>[[font:verdana|Happy Birthday!]]</b>

Wishing you a happy and healthy Birthday! 
Hope your day is full of smiles and memorable moments. 

From your friends at
[{[{ OfficeName }]}]";
			string bodyText=@"Wishing you a happy and healthy Birthday! 
Hope your day is full of smiles and memorable moments. 

From your friends at
[{[{ OfficeName }]}]";
			return new Tuple<string, string, string, string>(POut.String(name),POut.String(subject),POut.String(bodyText),POut.String(bodyHtml));
		}

		private static Tuple<string,string,string,string> GetInsuranceTemplate() {
			string name="End of Year Insurance";
			string subject="End of Year Insurance";
			string bodyHtml=@"Your insurance benefits will renew soon.  You have insurance remaining amounts that can be applied towards important dental treatment.  
Our records show that you have treatment that still needs to be completed.
Please call our office at your earliest convenience to schedule an appointment.";
			string bodyText=@"Your insurance benefits will renew soon.  You have insurance remaining amounts that can be applied towards important dental treatment.  Our records show that you have treatment that still needs to be completed.
Please call our office at your earliest convenience to schedule an appointment.";
			return new Tuple<string, string, string, string>(POut.String(name),POut.String(subject),POut.String(bodyText),POut.String(bodyHtml));
		}

		///<summary>Used to check if two doubles are "equal" based on some epsilon. 
		/// Epsilon is 0.0000001f and will return true if the absolute value of (val - val2) is less than that.</summary>
		private static bool AreNumsEqual(double val,double val2) {
			return IsNumZero(val-val2);
		}

		///<summary>Used to check if a double is "equal" to zero based on some epsilon. 
		/// Epsilon is 0.0000001f and will return true if the absolute value of the double is less than that.</summary>
		private static bool IsNumZero(double val) {
			return Math.Abs(val)<=0.0000001f;
		}

		///<summary>Look for self reported medications that may not have sent correctly to DoseSpot due to either multi-line notes or notes > 500 characters.
		///Reset the sent status for these medications so we try to sync them again, where we will remove newlines and truncate the note at 500 characters.</summary>
		private static void DoseSpotSelfReportedInvalidNote() {
			string command;
			command="SELECT Enabled FROM program WHERE ProgName='eRx';";
			//eRx is enabled.
			if(Db.GetInt(command)==1) {
				command="SELECT PropertyValue FROM programproperty WHERE PropertyDesc='eRx Option';";
				int option=Db.GetInt(command);
				//1 is they use DoseSpot, 2 is they use DoseSpot with Legacy eRx.
				if(option==1 || option==2) {
					//If the PatNote contains line breaks or carriage returns or is longer than 500 characters and the medication originated in OD and has been sent to eRx.
					//Clear the eRx Guid so it will resync the medication with DoseSpot.
					command=@"UPDATE medicationpat SET ErxGuid='' 
							WHERE ErxGuid LIKE '%OD_SelfReported%' 
							AND (PatNote LIKE CONCAT('%', CHAR(10), '%') OR PatNote LIKE CONCAT('%', CHAR(13), '%') OR LENGTH(PatNote) > 500);";
					Db.NonQ(command);
				}
			}
		}

		#endregion Helper Functions/Variables

		private static void To18_3_1() {
			string command;
			DataTable table;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimTrackingStatusExcludesNone','0')";
			Db.NonQ(command);			
			//command="ALTER TABLE sheetfield ADD TabOrderMobile int NOT NULL"; //Moved and combined into AlterLargeTable call below
			//Db.NonQ(command);
			//command="ALTER TABLE sheetfield ADD UiLabelMobile varchar(255) NOT NULL";//Moved and combined into AlterLargeTable call below
			//Db.NonQ(command);
			//Adding three columns to sheetfield at once using AlterLargeTable() for performance reasons.
			//No translation in convert script.
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.3.1 - Adding TabOrderMobile,UiLabelMobile,UiLabelMobileRadioButton to sheetfield.");
			LargeTableHelper.AlterLargeTable("sheetfield","SheetFieldNum",new List<Tuple<string,string>> { Tuple.Create("TabOrderMobile","int NOT NULL"),
				Tuple.Create("UiLabelMobile","varchar(255) NOT NULL"),Tuple.Create("UiLabelMobileRadioButton","varchar(255) NOT NULL") });
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.3.1");//No translation in convert script.
			command="ALTER TABLE sheetfielddef ADD TabOrderMobile int NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE sheetfielddef ADD UiLabelMobile varchar(255) NOT NULL";
			Db.NonQ(command);
			//Add edit archived patient permission to everyone
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				   +"VALUES("+POut.Long(groupNum)+",165)";//165 is ArchivedPatientEdit
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.3.1 - Adding credit card frequency");//No translation in convert script.
			command="ALTER TABLE creditcard ADD ChargeFrequency varchar(150) NOT NULL";
			Db.NonQ(command);
			command="UPDATE creditcard SET ChargeFrequency=CONCAT('0"//0 for ChargeFrequencyType.FixedDayOfMonth
				+"|',DAY(DateStart)) WHERE YEAR(DateStart) > 1880";
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.3.1");//No translation in convert script.
			command="ALTER TABLE insfilingcode ADD GroupType bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE insfilingcode ADD INDEX (GroupType)";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('OpenDentalServiceHeartbeat','0001-01-01 00:00:00')";
			Db.NonQ(command);
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes' AND IsHQCategory=1";
			string alertCategoryNum=Db.GetScalar(command);
			command="INSERT INTO alertcategorylink(AlertCategoryNum,AlertType) VALUES('"+alertCategoryNum+"','20')";//20 for alerttype OpenDentalServiceDown
			Db.NonQ(command);
			command="ALTER TABLE rxdef ADD PatientInstruction text NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE rxpat ADD PatientInstruction text NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultRxInstructions','0')";
			Db.NonQ(command);
			//Insert Midway Dental Supply bridge----------------------------------------------------------------- 
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
			   +") VALUES("
			   +"'Midway', "
			   +"'Midway Dental', "
			   +"'0', "
			   +"'"+POut.String(@"http://www.midwaydental.com/")+"', "
			   +"'"+"', "//No command line args
			   +"'')";
			long programNum=Db.NonQ(command,true);
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
			   +"VALUES ("
			   +"'"+POut.Long(programNum)+"', "
			   +"'7', "//ToolBarsAvail.MainToolbar
			   +"'Midway Dental')";
			Db.NonQ(command);
			//end Midway Dental Supply bridge
			command="INSERT INTO preference (PrefName,ValueString) VALUES('InsPayNoWriteoffMoreThanProc','1')";//InsPayNoWriteoffMoreThanProc-default to true
			Db.NonQ(command);
			#region Sales Tax
			//Insert new columns for sales tax properties
			command="ALTER TABLE adjustment ADD TaxTransID bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE adjustment ADD INDEX (TaxTransID)";
			Db.NonQ(command);
			//command="ALTER TABLE procedurelog ADD TaxAmt double NOT NULL"; //Moved to AlterLargeTable call below.
			//Db.NonQ(command); 
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.3.1 - Adding TaxAmt to procedurelog.");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("procedurelog","ProcNum",new List<Tuple<string,string>> { Tuple.Create("TaxAmt","double NOT NULL") });
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.3.1");//No translation in convert script.
			command="ALTER TABLE procedurecode ADD TaxCode varchar(16) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE proctp ADD TaxAmt double NOT NULL";
			Db.NonQ(command);
			//Create the Avalara Program Bridge
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				+") VALUES(" 
				+"'AvaTax', " 
				+"'AvaTax from Avalara.com', " 
				+"'0', " 
				+"'', " 
				+"'', "//leave blank if none 
				+"'')"; 
			programNum=Db.NonQ(command,true);
			long defNum=0;
			//Check to see if they already have a 'Sales Tax' Adjustment Type.  If they do, use that definition, otherwise insert a new definition.
			command="SELECT DefNum FROM definition WHERE Category=1 "//DefCat.AdjTypes
				+"AND LOWER(ItemName)='sales tax'";
			table=Db.GetTable(command);
			if(table.Rows.Count > 0) {
				defNum=PIn.Long(table.Rows[0][0].ToString());
			}
			if(defNum==0) {//We didn't find a definition named 'Sales Tax'.
				//Insert new status for 'Sales Tax'
				command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=1";
				int maxOrder=Db.GetInt(command);
				command="INSERT INTO definition (Category,ItemOrder,ItemName) VALUES (1,"+POut.Int(maxOrder)+",'Sales Tax')";
				defNum=Db.NonQ(command,true);
			}
			else {
				//Now set the def to not hidden (in case they already had a sales tax def that was hidden)
				command="UPDATE definition SET IsHidden=0 WHERE DefNum="+POut.Long(defNum);
				Db.NonQ(command);
			}
			//Avalara Program Property - SalesTaxAdjustmentType
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Sales Tax Adjustment Type', " 
				 +"'"+POut.Long(defNum)+"')"; 
			Db.NonQ(command);
			//Avalara Program Property - SalesTaxStates
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Taxable States', " 
				 +"'')"; 
			Db.NonQ(command);
			//Avalara Program Property - Username
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Username', " 
				 +"'')";  
			Db.NonQ(command);
			//Avalara Program Property - Password
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Password', " 
				 +"'')";
			Db.NonQ(command);
			//Avalara Program Property - CompanyCode
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Company Code', " 
				 +"'')"; 
			Db.NonQ(command); 
			//Avalara Program Property - Environment
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Test (T) or Production (P)', " 
				 +"'P')"; 
			Db.NonQ(command); 
			//Avalara Program Property - Log Level
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Log Level', " //0 - error, 1- information, 2- verbose 
				 +"'0')"; 
			Db.NonQ(command); 
			//Avalara Program Property - PrepayProcCodes
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Prepay Proc Codes', " //comma-separate list of proccodes we will allow users to pre-pay for
				 +"'')"; 
			Db.NonQ(command);
			//Avalara Program Property -  DiscountedProcCodes
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				 +") VALUES("
				 +"'"+POut.Long(programNum)+"', "
				 +"'Discount Proc Codes', " //comma-separate list of proccodes we give users discounts on when they prepay.
				 +"'')";
			Db.NonQ(command);
			#endregion Sales Tax
			command="ALTER TABLE substitutionlink ADD SubstitutionCode VARCHAR(25) NOT NULL,ADD SubstOnlyIf int NOT NULL";
			Db.NonQ(command);
			//Set all current substitutionlink.SubstOnlyIf to SubstitutionCondition.Never
			//The rows currently indicate substitutions for codes/conditions that will be ignored for the PlanNum.
			//Setting them to Never maintains this functionality
			command="UPDATE substitutionlink SET SubstOnlyIf=3";//SubstitutionCondition.Never;
			Db.NonQ(command);
			#region HQ Only
			//We are running this section of code for HQ only
			//This is very uncommon and normally manual queries should be run instead of doing a convert script.
			command="SELECT ValueString FROM preference WHERE PrefName='DockPhonePanelShow'";
			table=Db.GetTable(command);
			if(table.Rows.Count > 0 && PIn.Bool(table.Rows[0][0].ToString())) {
				command="DROP TABLE IF EXISTS jobnotification";
				Db.NonQ(command);
				command=@"CREATE TABLE jobnotification (
					JobNotificationNum BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
					JobNum BIGINT NOT NULL,
					UserNum BIGINT NOT NULL,
					Changes TINYINT(4) NOT NULL,
					INDEX(JobNum),
					INDEX(UserNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			#endregion
			//command="ALTER TABLE sheetfield ADD UiLabelMobileRadioButton varchar(255) NOT NULL"; //Moved to AlterLargeTable call above.
			//Db.NonQ(command);
			command="ALTER TABLE sheetfielddef ADD UiLabelMobileRadioButton varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE sheet ADD HasMobileLayout tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE sheetdef ADD HasMobileLayout tinyint NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('PromptForSecondaryClaim','1')";//default to true.
			Db.NonQ(command);
			command="ALTER TABLE repeatcharge ADD ChargeAmtAlt double NOT NULL";
			Db.NonQ(command);
		}

		private static void To18_3_2() {
			string command;
			command="UPDATE repeatcharge SET ChargeAmtAlt=-1";//This indicates that they have not started using Zipwhip yet.
			Db.NonQ(command);
		}

		private static void To18_3_4() {
			string command;
			command="ALTER TABLE claimform ADD Width int NOT NULL";
			Db.NonQ(command);
			command="UPDATE claimform SET Width=850";
			Db.NonQ(command);
			command="ALTER TABLE claimform ADD Height int NOT NULL";
			Db.NonQ(command);
			command="UPDATE claimform SET Height=1100";
			Db.NonQ(command);
		}

		private static void To18_3_8() {
			string command;
			command="SELECT COUNT(*) FROM program WHERE ProgName='PandaPeriodAdvanced'";
			//We may have already added this bridge in 18.2.29
			if(Db.GetCount(command)=="0") {
				//Update current PandaPerio description to Panda Perio (simple)
				command="UPDATE program SET ProgDesc='Panda Perio (simple) from www.pandaperio.com' WHERE ProgName='PandaPerio'";
				Db.NonQ(command);
				//Insert PandaPeriodAdvanced bridge----------------------------------------------------------------- 
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					 +") VALUES("
					 +"'PandaPeriodAdvanced', "
					 +"'Panda Perio (advanced) from www.pandaperio.com', "
					 +"'0', "
					 +"'"+POut.String(@"C:\Program Files (x86)\Panda Perio\Panda.exe")+"', "
					 +"'', "//leave blank if none 
					 +"'')";
				long programNum=Db.NonQ(command,true);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					 +") VALUES("
					 +"'"+POut.Long(programNum)+"', "
					 +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					 +"'0')";
				Db.NonQ(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					 +"VALUES ("
					 +"'"+POut.Long(programNum)+"', "
					 +"'2', "//ToolBarsAvail.ChartModule 
					 +"'PandaPeriodAdvanced')";
				Db.NonQ(command);
				//end PandaPeriodAdvanced bridge 
			}
		}

		private static void To18_3_9() {
			string command;
			//Add Avalara Program Property for Tax Exempt Pat Field
			command="SELECT ProgramNum FROM program WHERE ProgName='AvaTax'";
			long programNum=Db.GetLong(command);
			command="SELECT COUNT(*) FROM programproperty WHERE ProgramNum='"+POut.Long(programNum)+"' AND PropertyDesc='Tax Exempt Pat Field Def'";
			string taxExemptProperty=Db.GetCount(command);
			if(taxExemptProperty=="0") {
				//Avalara Program Property - SalesTaxAdjustmentType
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
						+") VALUES(" 
						+"'"+POut.Long(programNum)+"', " 
						+"'Tax Exempt Pat Field Def', " 
						+"'')"; 
				Db.NonQ(command);
			}
		}

		private static void To18_3_15() {
			string command;
			command="ALTER TABLE emailtemplate ADD IsHtml tinyint NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PrintStatementsAlphabetically','0')";
			Db.NonQ(command);
		}

		private static void To18_3_18() {
			string command;
			//Some of the default Web Sched Notify templates had '[ApptDate]' where they should have had '[ApptTime]'.
			foreach(string prefName in new List<string> { "WebSchedVerifyRecallText","WebSchedVerifyNewPatText","WebSchedVerifyASAPText" }) {
				command="SELECT ValueString FROM preference WHERE PrefName='"+POut.String(prefName)+"'";
				string curValue=Db.GetScalar(command);
				if(curValue=="Appointment scheduled for [FName] on [ApptDate] [ApptDate] at [OfficeName], [OfficeAddress]") {
					string newValue="Appointment scheduled for [FName] on [ApptDate] [ApptTime] at [OfficeName], [OfficeAddress]";
					command="UPDATE preference SET ValueString='"+POut.String(newValue)+"' WHERE PrefName='"+POut.String(prefName)+"'";
					Db.NonQ(command);
				}
			}
			//Remove supplyorders with supplynum=0 and update supplyorder totals
			command="SELECT DISTINCT SupplyOrderNum FROM supplyorderitem WHERE SupplyNum=0";
			List<long> supplyOrderNums=Db.GetListLong(command);
			for(int i=0;i<supplyOrderNums.Count;i++) {
				command="UPDATE supplyorder SET AmountTotal="
					+"(SELECT SUM(Qty*Price) FROM supplyorderitem WHERE SupplyOrderNum="+POut.Long(supplyOrderNums[i])+" AND SupplyNum!=0) "
					+"WHERE SupplyOrderNum="+POut.Long(supplyOrderNums[i]);
				Db.NonQ(command);
			}
			command="DELETE FROM supplyorderitem WHERE SupplyNum=0";
			Db.NonQ(command);
		}

		private static void To18_3_19() {
			string command;
			command="SELECT ProgramNum,Enabled FROM program WHERE ProgName='eRx'";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count>0 && PIn.Bool(table.Rows[0]["Enabled"].ToString())) {
				long programErx=PIn.Long(table.Rows[0]["ProgramNum"].ToString());
				command=$"SELECT PropertyValue FROM programproperty WHERE PropertyDesc='eRx Option' AND ProgramNum={programErx}";
				//0 is the enum value of Legacy, 1 is the enum value of DoseSpot
				bool isNewCrop=PIn.Int(Db.GetScalar(command))==0;
				if(isNewCrop) {//Only update rows if the office has eRx enabled and is using NewCrop.
					command="UPDATE provider SET IsErxEnabled=2 WHERE IsErxEnabled=1";
					Db.NonQ(command);
				}
			}
			//check databasemaintenance for TransactionsWithFutureDates, insert if not there and set IsOld to True or update to set IsOld to true
			command="SELECT MethodName FROM databasemaintenance WHERE MethodName='TransactionsWithFutureDates'";
			string methodName=Db.GetScalar(command);
			if(methodName=="") {//didn't find row in table, insert
				command="INSERT INTO databasemaintenance (MethodName,IsOld) VALUES ('TransactionsWithFutureDates',1)";
			}
			else {//found row, update IsOld
				command="UPDATE databasemaintenance SET IsOld = 1 WHERE MethodName = 'TransactionsWithFutureDates'";
			}
			Db.NonQ(command);
			//Add Avalara Program Property for TaxCodeOverrides
			command="SELECT ProgramNum FROM program WHERE ProgName='AvaTax'";
			long programNum=Db.GetLong(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(programNum)+"', " 
				+"'Tax Code Overrides', " 
				+"'')"; 
			Db.NonQ(command);
		}

		private static void To18_3_21() {
			string command;
			command="INSERT INTO preference (PrefName,ValueString) VALUES('SaveDXCAttachments','1')";
			Db.NonQ(command);
		}

		private static void To18_3_22() {
			string command;
			//We are running this section of code for HQ only
			//This is very uncommon and normally manual queries should be run instead of doing a convert script.
			command="SELECT ValueString FROM preference WHERE PrefName='DockPhonePanelShow'";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count > 0 && PIn.Bool(table.Rows[0][0].ToString())) {
				//Change TimeEstimate to TimeEstimateDevelopment
				command="ALTER TABLE job CHANGE COLUMN TimeEstimate TimeEstimateDevelopment bigint(20) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE job ADD TimeEstimateConcept bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE job ADD INDEX (TimeEstimateConcept)";
				Db.NonQ(command);
				command="ALTER TABLE job ADD TimeEstimateWriteup bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE job ADD INDEX (TimeEstimateWriteup)";
				Db.NonQ(command);
				command="ALTER TABLE job ADD TimeEstimateReview bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE job ADD INDEX (TimeEstimateReview)";
				Db.NonQ(command);
			}
		}

		private static void To18_3_23() {
			string command;
			command="ALTER TABLE confirmationrequest ADD DoNotResend tinyint NOT NULL";
			Db.NonQ(command);
		}

		private static void To18_3_24() {
			string command;
			command="UPDATE displayfield SET InternalName='Country' WHERE InternalName='Contry' AND Category=16;";
			Db.NonQ(command);
		}

		private static void To18_3_26() {
			string command;
			FixB11013();
			//Moving codes to the Obsolete category that were deleted in CDT 2019.
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				//Move deprecated codes to the Obsolete procedure code category.
				//Make sure the procedure code category exists before moving the procedure codes.
				string procCatDescript="Obsolete";
				long defNum=0;
				command="SELECT DefNum FROM definition WHERE Category=11 AND ItemName='"+POut.String(procCatDescript)+"'";//11 is DefCat.ProcCodeCats
				DataTable dtDef=Db.GetTable(command);
				if(dtDef.Rows.Count==0) { //The procedure code category does not exist, add it
					command="SELECT COUNT(*) FROM definition WHERE Category=11";//11 is DefCat.ProcCodeCats
					int countCats=PIn.Int(Db.GetCount(command));
					command="INSERT INTO definition (Category,ItemName,ItemOrder) "
								+"VALUES (11"+",'"+POut.String(procCatDescript)+"',"+POut.Int(countCats)+")";//11 is DefCat.ProcCodeCats
					defNum=Db.NonQ(command,true);
				}
				else { //The procedure code category already exists, get the existing defnum
					defNum=PIn.Long(dtDef.Rows[0]["DefNum"].ToString());
				}
				string[] cdtCodesDeleted=new string[] {
					"D1515",
					"D1525",
					"D5281",
					"D9940"
				};
				//Change the procedure codes' category to Obsolete.
				command="UPDATE procedurecode SET ProcCat="+POut.Long(defNum)
					+" WHERE ProcCode IN('"+string.Join("','",cdtCodesDeleted.Select(x => POut.String(x)))+"') ";
				Db.NonQ(command);
			}//end United States CDT codes update
		}

		private static void To18_3_30() {
			string command;
			command=@"UPDATE preference SET ValueString='https://www.patientviewer.com:49997/OpenDentalWebServiceHQ/WebServiceMainHQ.asmx' 
				WHERE PrefName='WebServiceHQServerURL' AND ValueString='http://www.patientviewer.com:49999/OpenDentalWebServiceHQ/WebServiceMainHQ.asmx'";
			Db.NonQ(command);
		}

		private static void To18_3_49() {
			string command;
			command="UPDATE preference SET ValueString='https://opendentalsoft.com:1943/WebServiceCustomerUpdates/Service1.asmx' "
				+"WHERE PrefName='UpdateServerAddress' AND ValueString='http://opendentalsoft.com:1942/WebServiceCustomerUpdates/Service1.asmx'";
			Db.NonQ(command);
		}

		private static void To18_4_1() {
			string command;
			DataTable table;
			command="ALTER TABLE rxpat ADD ClinicNum bigint NOT NULL,ADD INDEX (ClinicNum)";
			Db.NonQ(command);
			//Set rxpat's ClinicNum to default patient's ClinicNum
			command=@"UPDATE rxpat
				INNER JOIN patient ON rxpat.PatNum=patient.PatNum
				SET rxpat.ClinicNum=patient.ClinicNum";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS pharmclinic";
			Db.NonQ(command);
			command=@"CREATE TABLE pharmclinic (
					PharmClinicNum bigint NOT NULL auto_increment PRIMARY KEY,
					PharmacyNum bigint NOT NULL,
					ClinicNum bigint NOT NULL,
					INDEX(PharmacyNum),
					INDEX(ClinicNum)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			//Fill table with current combinations of ClinicNums and PharmacyNums that appear in the rxpat table
			command=@"INSERT INTO pharmclinic (PharmacyNum,ClinicNum)
				SELECT DISTINCT PharmacyNum,ClinicNum FROM rxpat WHERE PharmacyNum != 0";
			Db.NonQ(command);
			//Convert appt reminder rules' plain text email templates into HTML templates.
			command=@"SELECT ApptReminderRuleNum,TemplateEmail,TemplateEmailAggShared,TemplateEmailAggPerAppt,TypeCur
				FROM apptreminderrule";
			table=Db.GetTable(command);
			command="SELECT ValueString FROM preference WHERE PrefName='EmailDisclaimerIsOn'";
			bool isEmailDisclaimerOn=(Db.GetScalar(command)=="1");
			Func<string,bool,string> fConvertReminderRule=new Func<string, bool, string>((template,includeDisclaimer) =>
				template
					.Replace(">","&>")
					.Replace("<","&<")
					+(includeDisclaimer && isEmailDisclaimerOn ? "\r\n\r\n\r\n[EmailDisclaimer]" : "")
			);
			foreach(DataRow row in table.Rows) {
				string templateEmail=fConvertReminderRule(PIn.String(row["TemplateEmail"].ToString()),true);
				if(PIn.Int(row["TypeCur"].ToString())==1) {//Confirmation
					templateEmail=templateEmail.Replace("[ConfirmURL]","<a href=\"[ConfirmURL]\">[ConfirmURL]</a>");
				}
				if(PIn.Int(row["TypeCur"].ToString())==3) {//Patient Portal Invites
					templateEmail=templateEmail.Replace("[PatientPortalURL]","<a href=\"[PatientPortalURL]\">[PatientPortalURL]</a>");
				}
				string templateEmailAggShared=fConvertReminderRule(PIn.String(row["TemplateEmailAggShared"].ToString()),true);
				if(PIn.Int(row["TypeCur"].ToString())==1) {//Confirmation
					templateEmailAggShared=templateEmailAggShared.Replace("[ConfirmURL]","<a href=\"[ConfirmURL]\">[ConfirmURL]</a>");
				}
				if(PIn.Int(row["TypeCur"].ToString())==3) {//Patient Portal Invites
					templateEmailAggShared=templateEmailAggShared.Replace("[PatientPortalURL]","<a href=\"[PatientPortalURL]\">[PatientPortalURL]</a>");
				}
				string templateEmailAggPerAppt=fConvertReminderRule(PIn.String(row["TemplateEmailAggPerAppt"].ToString()),false);
				command=@"UPDATE apptreminderrule SET 
					TemplateEmail='"+POut.String(templateEmail)+@"',
					templateEmailAggShared='"+POut.String(templateEmailAggShared)+@"',
					TemplateEmailAggPerAppt='"+POut.String(templateEmailAggPerAppt)+@"'
					WHERE ApptReminderRuleNum="+POut.Long(PIn.Long(row["ApptReminderRuleNum"].ToString()));
				Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EraIncludeWOPercCoPay','1')";
			Db.NonQ(command);
			command="ALTER TABLE tsitranslog ADD ClinicNum bigint NOT NULL,ADD INDEX (ClinicNum)";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReplaceExistingBlockout','0')";
			Db.NonQ(command);
			command="ALTER TABLE etrans835attach ADD DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('ArchiveKey','')";
			Db.NonQ(command);		
			#region DentalXChange Patient Credit Score Bridge
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				+") VALUES(" 
				+"'DXCPatientCreditScore', " 
				+"'DentalXChange Patient Credit Score from register.dentalxchange.com', "
				+"'0', " 
				+"'', "//Takes to web portal. No local executable
				+"'', "
				+"'')"; 
			long programNum=Db.NonQ(command,true); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
			   +") VALUES("
			   +"'"+POut.Long(programNum)+"', "
			   +"'Disable Advertising', "
			   +"'0')";
			Db.NonQ(command);
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
			   +"VALUES ("
			   +"'"+POut.Long(programNum)+"', "
			   +"'0', "//ToolBarsAvail.AccoutModule
			   +"'DXC Patient Credit Score')";
			Db.NonQ(command);
			#endregion
			command="INSERT INTO preference (PrefName,ValueString) VALUES('InsEstRecalcReceived','0')";
			Db.NonQ(command);	
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.4.1 - Adding index to appointment.Priority.");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("appointment","AptNum",null,
				new List<Tuple<string,string>> { Tuple.Create("Priority","") });//no need to send index name. only adds index if not exists
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.4.1");//No translation in convert script.
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			foreach(DataRow row in table.Rows) {
				 groupNum=PIn.Long(row["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",169)";  //169 is InsuranceVerification
				 Db.NonQ(command);
			}
			//Add columns to confirmationrequest
			command="ALTER TABLE confirmationrequest ADD SmsSentOk tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE confirmationrequest ADD EmailSentOk tinyint NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('UnscheduledListNoRecalls','1')";//True by default.
			Db.NonQ(command);	
			command="INSERT INTO preference (PrefName,ValueString) VALUES('EnterpriseApptList','0')";
			Db.NonQ(command);
			//Insurance History preferences
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistExamCodes',ValueString FROM preference WHERE PrefName='InsBenExamCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistProphyCodes',ValueString FROM preference WHERE PrefName='InsBenProphyCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistBWCodes',ValueString FROM preference WHERE PrefName='InsBenBWCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistPanoCodes',ValueString FROM preference WHERE PrefName='InsBenPanoCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistPerioURCodes',ValueString FROM preference WHERE PrefName='InsBenSRPCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistPerioULCodes',ValueString FROM preference WHERE PrefName='InsBenSRPCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistPerioLRCodes',ValueString FROM preference WHERE PrefName='InsBenSRPCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistPerioLLCodes',ValueString FROM preference WHERE PrefName='InsBenSRPCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistPerioMaintCodes',ValueString FROM preference WHERE PrefName='InsBenPerioMaintCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) SELECT 'InsHistDebridementCodes',ValueString FROM preference WHERE PrefName='InsBenFullDebridementCodes'";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('ApptAutoRefreshRange','4')";//Default to 4
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('RecurringChargesPayType','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('InsPlanUseUcrFeeForExclusions','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('InsPlanExclusionsMarkDoNotBillIns','0')";
			Db.NonQ(command);
			command="ALTER TABLE insplan ADD ExclusionFeeRule tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE procbutton ADD IsMultiVisit tinyint NOT NULL";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS procmultivisit";
			Db.NonQ(command);
			command=@"CREATE TABLE procmultivisit (
				ProcMultiVisitNum bigint NOT NULL auto_increment PRIMARY KEY,
				GroupProcMultiVisitNum bigint NOT NULL,
				ProcNum bigint NOT NULL,
				ProcStatus tinyint NOT NULL,
				IsInProcess tinyint NOT NULL,
				INDEX(GroupProcMultiVisitNum),
				INDEX(ProcNum),
				INDEX(IsInProcess)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			//Create an Enterprise Setup Window
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ShowFeatureEnterprise','0')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('RepeatingChargesAutomated','0')";//False by default
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('RepeatingChargesAutomatedTime','2018-01-01 08:00:00')";//8:00am by default(date portion not used).
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('RepeatingChargesRunAging','1')";//True by default
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('RepeatingChargesLastDateTime','0001-01-01 00:00:00')";//Initialize to MinDate.
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('RecurringChargesBeginDateTime','')";//Initialize to empty.
			Db.NonQ(command);
			command="ALTER TABLE automation ADD PatStatus tinyint NOT NULL";
			Db.NonQ(command);
		}//End of 18_4_1() method

		private static void To18_4_16() {
			string command;
			//Add Avalara Program Property for Sales Tax Return Adjustment Type
			command="SELECT ProgramNum FROM program WHERE ProgName='AvaTax'";
			long programNum=Db.GetLong(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(programNum)+"', " 
				+"'Sales Tax Return Adjustment Type', " 
				+"'')"; 
			Db.NonQ(command);
			//Add Avalara Program Property for Tax Lock Date
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(programNum)+"', " 
				+"'Tax Lock Date', " 
				+"'')"; 
			Db.NonQ(command);
		}//End of 18_4_16() method
	
		private static void To18_4_17() {
			string command;
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistExamCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistProphyCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistBWCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistPanoCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistPerioURCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistPerioULCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistPerioLRCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistPerioLLCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistPerioMaintCodes'";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString=SUBSTRING_INDEX(ValueString,',',1) WHERE PrefName='InsHistDebridementCodes'";
			Db.NonQ(command);
		}//End of 18_4_17() method

		private static void To18_4_22() {
			string command;
			command="UPDATE preference SET ValueString='https://opendentalsoft.com:1943/WebServiceCustomerUpdates/Service1.asmx' "
				+"WHERE PrefName='UpdateServerAddress' AND ValueString='http://opendentalsoft.com:1942/WebServiceCustomerUpdates/Service1.asmx'";
			Db.NonQ(command);
		}

		private static void To18_4_29() {
			string command;
			command="ALTER TABLE tsitranslog ADD AggTransLogNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE tsitranslog ADD INDEX (AggTransLogNum)";
			Db.NonQ(command);
		}

		private static void To19_1_1() {
			string command;
			//Oryx bridge-------------------------------------------------
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				+") VALUES("
				+"'Oryx', "
				+"'Oryx from oryxdentalsoftware.com', "
				+"'0', "
				+"'', "//Opens website. No local executable.
				+"'', "
				+"'')";
			long programNum=Db.NonQ(command,true);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				 +") VALUES("
				 +"'"+POut.Long(programNum)+"', "
				 +"'Client URL', "
				 +"'')";
			Db.NonQ(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				 +") VALUES("
				 +"'"+POut.Long(programNum)+"', "
				 +"'Disable Advertising', "
				 +"'0')";
			Db.NonQ(command);
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				 +"VALUES ("
				 +"'"+POut.Long(programNum)+"', "
				 +"'2', "//ToolBarsAvail.ChartModule
				 +"'Oryx')";
			Db.NonQ(command);
			//End Oryx bridge---------------------------------------------
			command="INSERT INTO preference(PrefName,ValueString) VALUES('FeesUseCache','0')";
			Db.NonQ(command);
			command="ALTER TABLE providerclinic ADD StateLicense varchar(50) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE providerclinic ADD StateRxID varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE providerclinic ADD StateWhereLicensed varchar(15) NOT NULL";
			Db.NonQ(command);
			command="SELECT ProvNum FROM providerclinic WHERE ClinicNum=0 GROUP BY ProvNum";
			List<long> listDefaultProvClinicsProvNums=Db.GetListLong(command);
			command="SELECT ProvNum,DEANum,StateLicense,StateRxID,StateWhereLicensed FROM provider";
			Dictionary<long,DataRow> dictProvs=Db.GetTable(command).Select().ToDictionary(x=> PIn.Long(x["ProvNum"].ToString()));
			foreach(KeyValuePair<long,DataRow> kvp in dictProvs) {
				string stateRxId=kvp.Value["StateRxID"].ToString();
				string stateLicense=kvp.Value["StateLicense"].ToString();
				string stateWhereLicensed=kvp.Value["StateWhereLicensed"].ToString();
				if(ListTools.In(kvp.Key,listDefaultProvClinicsProvNums)) {
					//A default providerClinic already exist for this provider. Update the providerclinic row for this provider with ClinicNum=0
					command="UPDATE providerclinic SET StateLicense='"+POut.String(stateLicense)+"', "
						+"StateRxID='"+POut.String(stateRxId)+"', "
						+"StateWhereLicensed='"+POut.String(stateWhereLicensed)+"' "
						+"WHERE ProvNum="+POut.Long(kvp.Key)+" AND ClinicNum=0";
				}
				else {
					//Providerclinic doesn't exist for this provider. Add a new default providerclinic for ClinicNum=0
					command="INSERT INTO providerclinic (ProvNum,ClinicNum,DEANum,StateLicense,StateRxID,StateWhereLicensed) "
						+"VALUES ("+POut.Long(kvp.Key)+","//ProvNum
							+"0,"//ClinicNum
							+"'"+POut.String(kvp.Value["DEANum"].ToString())+"',"
							+"'"+POut.String(stateLicense)+"',"
							+"'"+POut.String(stateRxId)+"',"
							+"'"+POut.String(stateWhereLicensed)+"')";
				}
				Db.NonQ(command);
			}
			command="SELECT ProgramNum FROM program WHERE ProgName='PayConnect'";
			long payConnectProgNum=PIn.Long(Db.GetScalar(command));
			command="SELECT DISTINCT ClinicNum FROM programproperty WHERE ProgramNum="+POut.Long(payConnectProgNum);
			List<long> listClinicNums=Db.GetListLong(command);
			foreach(long clinicNum in listClinicNums) {
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum"
					 +") VALUES("
					 +"'"+POut.Long(payConnectProgNum)+"', "
					 +"'Patient Portal Payments Token', "
					 +"'',"
					 +"'',"
					 +POut.Long(clinicNum)+")";
				Db.NonQ(command);
			}
			command="SELECT MAX(displayreport.ItemOrder) FROM displayreport WHERE displayreport.Category = 2"; //monthly
			long itemorder = Db.GetLong(command)+1; //get the next available ItemOrder for the Monthly Category to put this new report last.
			command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) "
				 +"VALUES('ODProcOverpaid',"+POut.Long(itemorder)+",'Procedures Overpaid',2,0)";
			long reportNumNew=Db.NonQ(command,getInsertID:true);
			long reportNum=Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODProcsNotBilled'");
			List<long> listGroupNums=Db.GetListLong("SELECT UserGroupNum FROM grouppermission WHERE PermType=22 AND FKey="+POut.Long(reportNum));
			foreach(long groupNumCur in listGroupNums) {
				command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
					 +"VALUES('0001-01-01',0,"+POut.Long(groupNumCur)+",22,"+POut.Long(reportNumNew)+")";
				Db.NonQ(command);
			}
			//Add preference WebSchedRecallDoubleBooking, default to 1 which will preserve old behavior by preventing double booking.
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedRecallDoubleBooking','1')";
			Db.NonQ(command);
			//Add preference WebSchedNewPatApptDoubleBooking, default to 1 which will preserve old behavior by preventing double booking.
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedNewPatApptDoubleBooking','1')";
			Db.NonQ(command);
			//Add user column to alertItem table
			command="ALTER TABLE alertitem ADD UserNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE alertitem ADD INDEX (UserNum)";
			Db.NonQ(command);
			long itemOrder=Db.GetLong("SELECT ItemOrder FROM displayreport WHERE InternalName='ODProviderPayrollSummary'")+1;
			command="UPDATE displayreport SET ItemOrder="+POut.Long(itemOrder)+" WHERE InternalName='ODProviderPayrollSummary'";
			Db.NonQ(command);//Move down provider payroll summary
			itemOrder=Db.GetLong("SELECT ItemOrder FROM displayreport WHERE InternalName='ODProviderPayrollDetailed'")+1;
			command="UPDATE displayreport SET ItemOrder="+POut.Long(itemOrder)+" WHERE InternalName='ODProviderPayrollDetailed'";
			Db.NonQ(command);//Move down provider payroll detailed
			itemOrder=Db.GetLong("SELECT displayreport.ItemOrder FROM displayreport WHERE displayreport.InternalName='ODMoreOptions'")+1;
			command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden,IsVisibleInSubMenu) "
					+"VALUES('ODMonthlyProductionGoal',"+POut.Long(itemOrder)+",'Monthly Prod Inc Goal',0,0,0)";//Insert Monthly Production Goal in the new space we just made after More Options
			Db.NonQ(command);
			long moreOptionsFKey=Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODMoreOptions'");
			DataTable userGroupTable=Db.GetTable("SELECT UserGroupNum FROM grouppermission WHERE PermType=22 AND FKey="+POut.Long(moreOptionsFKey));
			reportNum=Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODMonthlyProductionGoal'");
			foreach(DataRow row in userGroupTable.Rows) {
					command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES('0001-01-01',0,"+POut.Long(PIn.Long(row["UserGroupNum"].ToString()))+",22,"+POut.Long(reportNum)+")";
					Db.NonQ(command);
			}
			//Create the Reactivatione Table
			command="DROP TABLE IF EXISTS reactivation";
				Db.NonQ(command);
				command=@"CREATE TABLE reactivation (
					ReactivationNum bigint NOT NULL auto_increment PRIMARY KEY,
					PatNum bigint NOT NULL,
					ReactivationStatus bigint NOT NULL,
					ReactivationNote text NOT NULL,
					DoNotContact tinyint NOT NULL,
					INDEX(PatNum),
					INDEX(ReactivationStatus)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			//Add Reactivation Preferences
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ShowFeatureReactivations','0')";
			Db.NonQ(command);
			//Add preference ReactivationContactInterval
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationContactInterval','0')";
			Db.NonQ(command);
			//Add preference ReactivationCountContactMax
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationCountContactMax','0')";
			Db.NonQ(command);
			//Add preference ReactivationDaysPast
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationDaysPast','1095')";
			Db.NonQ(command);
			//Add preference ReactivationEmailFamMsg
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationEmailFamMsg','')";
			Db.NonQ(command);
			//Add preference ReactivationEmailMessage
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationEmailMessage','')";
			Db.NonQ(command);
			//Add preference ReactivationEmailSubject
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationEmailSubject','')";
			Db.NonQ(command);
			//Add preference ReactivationGroupByFamily
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationGroupByFamily','0')";
			Db.NonQ(command);
			//Add preference ReactivationPostcardFamMsg
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationPostcardFamMsg','')";
			Db.NonQ(command);
			//Add preference ReactivationPostcardMessage
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationPostcardMessage','')";
			Db.NonQ(command);
			//Add preference ReactivationPostcardsPerSheet
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationPostcardsPerSheet','3')";
			Db.NonQ(command);
			//Add preference ReactivationStatusEmailed
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationStatusEmailed','0')";
			Db.NonQ(command);
			//Add preference ReactivationStatusEmailedTexted
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationStatusEmailedTexted','0')";
			Db.NonQ(command);
			//Add preference ReactivationStatusMailed
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationStatusMailed','0')";
			Db.NonQ(command);
			//Add preference ReactivationStatusTexted
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReactivationStatusTexted','0')";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS mobileappdevice";
			Db.NonQ(command);
			command=@"CREATE TABLE mobileappdevice (
					MobileAppDeviceNum bigint NOT NULL auto_increment PRIMARY KEY,
					ClinicNum bigint NOT NULL,
					DeviceName varchar(255) NOT NULL,
					UniqueID varchar(255) NOT NULL,
					IsAllowed tinyint NOT NULL,
					LastAttempt datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					LastLogin datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					INDEX(ClinicNum)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DatabaseMode','0')";//Default to 'Normal'
			Db.NonQ(command);
			command="ALTER TABLE dunning ADD IsSuperFamily tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE registrationkey ADD HasEarlyAccess tinyint NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('Ins834DropExistingPatPlans','0')";//Default to false
			Db.NonQ(command);
			LargeTableHelper.AlterLargeTable("treatplan","TreatPlanNum",new List<Tuple<string,string>> {
				Tuple.Create("SignaturePractice","text NOT NULL"),
				Tuple.Create("DateTSigned","datetime NOT NULL DEFAULT '0001-01-01 00:00:00'"),
				Tuple.Create("DateTPracticeSigned","datetime NOT NULL DEFAULT '0001-01-01 00:00:00'"),
				Tuple.Create("SignatureText","varchar(255) NOT NULL"),
				Tuple.Create("SignaturePracticeText","varchar(255) NOT NULL")
			});
			command="DROP TABLE IF EXISTS providercliniclink";
			Db.NonQ(command);
			command=@"CREATE TABLE providercliniclink (
				ProviderClinicLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
				ProvNum bigint NOT NULL,
				ClinicNum bigint NOT NULL,
				INDEX(ProvNum),
				INDEX(ClinicNum)
			) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			//PaySimple ACH Payment Type
			command="SELECT ProgramNum FROM program WHERE ProgName='PaySimple'";
			programNum=Db.GetLong(command);
			command=$@"SELECT ClinicNum,PropertyValue FROM programproperty 
				WHERE ProgramNum={POut.Long(programNum)} AND PropertyDesc='PaySimple Payment Type'";
			DataTable table=Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				command=$@"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ClinicNum
					) VALUES(
					{POut.Long(programNum)},
					'PaySimple Payment Type ACH',
					'{POut.String(PIn.String(row["PropertyValue"].ToString()))}',
					{POut.Long(PIn.Long(row["ClinicNum"].ToString()))})";
				Db.NonQ(command);
			}
			//Rename 'PaySimple Payment Type' program property to make it clear it's only for credit cards.
			command=$@"UPDATE programproperty SET PropertyDesc='PaySimple Payment Type CC' 
				WHERE ProgramNum={POut.Long(programNum)} AND PropertyDesc='PaySimple Payment Type'";
			Db.NonQ(command);
			//Rename 'RecurringChargesPayType' preference to make it clear it's only for credit cards.
			command=$@"UPDATE preference SET PrefName='RecurringChargesPayTypeCC' WHERE PrefName='RecurringChargesPayType'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ThemeSetByUser','0')";
			Db.NonQ(command);
			//Transworld Adjustment
			command="SELECT ProgramNum FROM program WHERE ProgName='Transworld'";
			long tsiProgNum=Db.GetLong(command);
			command="SELECT DISTINCT ClinicNum FROM programproperty WHERE ProgramNum="+POut.Long(tsiProgNum);//copied from different example, but found
			//that if program is not yet enabled, but using clinics, properties will not get inserted for all clinics if you decide to enable feature later. 
			List<long> listProgClinicNums=Db.GetListLong(command);
			for(int i=0;i < listProgClinicNums.Count;i++) {
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("+POut.Long(tsiProgNum)+",'SyncExcludePosAdjType','0','',"+POut.Long(listProgClinicNums[i])+"),"
					+"("+POut.Long(tsiProgNum)+",'SyncExcludeNegAdjType','0','',"+POut.Long(listProgClinicNums[i])+")";
				Db.NonQ(command);
			}
			command="ALTER TABLE sheetdef ADD UserNum bigint NOT NULL";//New column UserNum will default to 0.
			Db.NonQ(command);
			command="ALTER TABLE sheetdef ADD INDEX (UserNum)";
			Db.NonQ(command);
			command="ALTER TABLE sheetfielddef ADD LayoutMode tinyint NOT NULL";//New column SheetTypeMode will default to 0 (SheetTypeMode.Default)
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('SendEmailsInDiffProcess','0')";//Default to 'False'
			Db.NonQ(command);
			command="ALTER TABLE tasklist ADD GlobalTaskFilterType tinyint NOT NULL";
			Db.NonQ(command);
			command="UPDATE tasklist SET GlobalTaskFilterType=1";//GlobalTaskFilterType.Default
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('TasksGlobalFilterType','0')";//Require offices to turn this on.
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS payconnectresponseweb";
			Db.NonQ(command);
			command=@"CREATE TABLE payconnectresponseweb (
				PayConnectResponseWebNum bigint NOT NULL auto_increment PRIMARY KEY,
				PatNum bigint NOT NULL,
				PayNum bigint NOT NULL,
				AccountToken varchar(255) NOT NULL,
				PaymentToken varchar(255) NOT NULL,
				ProcessingStatus tinyint NOT NULL,
				DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				DateTimePending datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				DateTimeCompleted datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				DateTimeExpired datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				DateTimeLastError datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				LastResponseStr text NOT NULL,
				INDEX(PatNum),
				INDEX(PayNum)
			) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('GlobalUpdateWriteOffLastClinicCompleted','')";
			Db.NonQ(command);
			command="ALTER TABLE paysplit ADD AdjNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE paysplit ADD INDEX (AdjNum)";
			Db.NonQ(command);
		}//End of 19_1_1() method

		private static void To19_1_3() {
			string command;
			command="ALTER TABLE repeatcharge ADD UnearnedTypes varchar(4000) NOT NULL";
			Db.NonQ(command);
		}

		private static void To19_1_4() {
			string command;
			command="ALTER TABLE payconnectresponseweb ADD CCSource tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE payconnectresponseweb ADD Amount double NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE payconnectresponseweb ADD PayNote varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE payconnectresponseweb MODIFY ProcessingStatus VARCHAR(255) NOT NULL";
			Db.NonQ(command);
		}

		private static void To19_1_6() {
			string command;
			command="ALTER TABLE sheetdef DROP COLUMN UserNum";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('ChartDefaultLayoutSheetDefNum','0')";
			Db.NonQ(command);
		}

		private static void To19_1_7() {
			string command;
			command="ALTER TABLE laboratory ADD IsHidden tinyint NOT NULL";
			Db.NonQ(command);
		}

		private static void To19_1_9() {
			string command;
			command="UPDATE displayreport SET Description='Monthly Production Goal' WHERE InternalName='ODMonthlyProductionGoal'";
			Db.NonQ(command);
			DataTable table;
			//Add permission to everyone------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			foreach(DataRow row in table.Rows) {
				 groupNum=PIn.Long(row["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,NewerDays,PermType) "
						+"VALUES("+POut.Long(groupNum)+",1,174)";//174 - NewClaimsProcNotBilled, default lock days to 1 day.
				 Db.NonQ(command);
			}
		}

		private static void To19_1_21() {
			string command;
			command="INSERT INTO preference (PrefName,ValueString) VALUES('CanadaCreatePpoLabEst','1')";//Enabled by default.
			Db.NonQ(command);
		}

		private static void To19_1_23() {
			string command;			
			command="INSERT INTO preference (PrefName,ValueString) VALUES('PatientSelectSearchMinChars','1')";//1 char by default, to maintain current behavior
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('PatientSelectSearchPauseMs','1')";//1 ms by default, to maintain current behavior
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('PatientSelectSearchWithEmptyParams','0')";//0 for Unknown (YN enum), to maintain current behavior
			Db.NonQ(command);
		}
		private static void To19_1_25() {
			string command;
			command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES (1,21)";//eservices, WebMailRecieved
			Db.NonQ(command);
		}

		private static void To19_1_31() {
			string command;
			command="INSERT INTO preference (PrefName,ValueString) VALUES('EnterpriseNoneApptViewDefaultDisabled','0')";//Default to false
			Db.NonQ(command);
		}

		private static void To19_1_34() {
			string command;
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedRecallApptSearchAfterDays','')";//Default to no days
			Db.NonQ(command);
		}

		private static void To19_2_1() {
			string command;
			DataTable table;
			//TSI - ArManager Excluded default preferences - The values are set to the unsent tab defaults
			command="INSERT INTO preference(PrefName,ValueString) "
				+"SELECT 'ArManagerExcludedExcludeBadAddresses',ValueString FROM preference WHERE PrefName='ArManagerExcludeBadAddresses'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) "
				+"SELECT 'ArManagerExcludedExcludeIfUnsentProcs',ValueString FROM preference WHERE PrefName='ArManagerExcludeIfUnsentProcs'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) "
				+"SELECT 'ArManagerExcludedExcludeInsPending',ValueString FROM preference WHERE PrefName='ArManagerExcludeInsPending'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) "
				+"SELECT 'ArManagerExcludedDaysSinceLastPay',ValueString FROM preference WHERE PrefName='ArManagerUnsentDaysSinceLastPay'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) "
				+"SELECT 'ArManagerExcludedMinBal',ValueString FROM preference WHERE PrefName='ArManagerUnsentMinBal'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) "
				+"SELECT 'ArManagerExcludedAgeOfAccount',ValueString FROM preference WHERE PrefName='ArManagerUnsentAgeOfAccount'";
			Db.NonQ(command);
			//Add the new AccountingCashPaymentType preference
			string payTypeDescript="Cash";
			long defNum=0;
			command=$"SELECT DefNum,IsHidden FROM definition WHERE Category=10 AND ItemName='{POut.String(payTypeDescript)}'";//10 is DefCat.PaymentTypes
			table=Db.GetTable(command);
			if(table.Rows.Count==0) { //The cash payment type does not exist, add it
				command="SELECT COUNT(*) FROM definition WHERE Category=10";//10 is DefCat.PaymentTypes
				int countCats=PIn.Int(Db.GetCount(command));
				command=$"INSERT INTO definition (Category,ItemName,ItemOrder) VALUES (10,'{POut.String(payTypeDescript)}',{POut.Int(countCats)})";//10 is DefCat.PaymentTypes
				defNum=Db.NonQ(command,true);
			}
			else { //The cash payment type already exists, get the existing DefNum
				defNum=PIn.Long(table.Rows[0]["DefNum"].ToString());
				if(PIn.Bool(table.Rows[0]["IsHidden"].ToString())) {
					//Unhide if Cash payment type is hidden.
					command=$"UPDATE definition SET IsHidden=0 WHERE DefNum={POut.Long(defNum)} AND Category=10";
					Db.NonQ(command);
				}
			}
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('AccountingCashPaymentType','{POut.Long(defNum)}')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('ApptModuleUses2019Overhaul','0')";
			Db.NonQ(command);
			command="ALTER TABLE userod ADD MobileWebPin varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE userod ADD MobileWebPinFailedAttempts tinyint unsigned NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE proctp ADD ProvNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE proctp ADD INDEX (ProvNum)";
			Db.NonQ(command);
			command="ALTER TABLE proctp ADD DateTP date NOT NULL DEFAULT '0001-01-01'";
			Db.NonQ(command);
			command="ALTER TABLE proctp ADD ClinicNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE proctp ADD INDEX (ClinicNum)";
			Db.NonQ(command);
			command="ALTER TABLE carrier ADD TrustedEtransFlags tinyint NOT NULL";
			Db.NonQ(command);
			//Convert plain text email templates into HTML templates.
			command=@"SELECT PrefName,ValueString FROM preference 
				WHERE PrefName IN('WebSchedMessage','WebSchedMessage2','WebSchedMessage3','WebSchedAggregatedEmailBody','WebSchedVerifyRecallEmailBody',
					'WebSchedVerifyNewPatEmailBody','WebSchedVerifyASAPEmailBody','WebSchedAsapEmailTemplate','PatientPortalNotifyBody')";
			table=Db.GetTable(command);
			command="SELECT ValueString FROM preference WHERE PrefName='EmailDisclaimerIsOn'";
			bool isEmailDisclaimerOn=(Db.GetScalar(command)=="1");
			string ConvertEmailTemplate(string template) {
				return template
					.Replace(">","&>")
					.Replace("<","&<")
					+(isEmailDisclaimerOn ? "\r\n\r\n\r\n[EmailDisclaimer]" : "");
			}
			foreach(DataRow row in table.Rows) {
				string prefName=PIn.String(row["PrefName"].ToString());
				string valueString=ConvertEmailTemplate(PIn.String(row["ValueString"].ToString()))
					.Replace("[OfficePhone]","<a href=\"tel:[OfficePhone]\">[OfficePhone]</a>");
				switch(prefName) {
					case "WebSchedMessage":
					case "WebSchedMessage2":
					case "WebSchedMessage3":
					case "PatientPortalNotifyBody":
						valueString=valueString.Replace("[URL]","<a href=\"[URL]\">[URL]</a>");
						break;
					case "WebSchedAsapEmailTemplate":
						valueString=valueString.Replace("[AsapURL]","<a href=\"[AsapURL]\">[AsapURL]</a>");
						break;
				}
				command=$"UPDATE preference SET ValueString='{POut.String(valueString)}' WHERE PrefName='{POut.String(prefName)}'";
				Db.NonQ(command);
			}
			command=@"SELECT ClinicNum,PrefName,ValueString FROM clinicpref 
				WHERE PrefName IN('WebSchedVerifyRecallEmailBody','WebSchedVerifyNewPatEmailBody','WebSchedVerifyASAPEmailBody','WebSchedAsapEmailTemplate')";
			table=Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				long clinicNum=PIn.Long(row["ClinicNum"].ToString());
				string prefName=PIn.String(row["PrefName"].ToString());
				string valueString=ConvertEmailTemplate(PIn.String(row["ValueString"].ToString()))
					.Replace("[OfficePhone]","<a href=\"tel:[OfficePhone]\">[OfficePhone]</a>");
				switch(prefName) {
					case "WebSchedAsapEmailTemplate":
						valueString=valueString.Replace("[AsapURL]","<a href=\"[AsapURL]\">[AsapURL]</a>");
						break;
				}
				command=$@"UPDATE clinicpref SET ValueString='{POut.String(valueString)}' WHERE PrefName='{POut.String(prefName)}' 
					AND ClinicNum={POut.Long(clinicNum)}";
				Db.NonQ(command);
			}
			//Previously www.open-dent.com/updates/ redirected to www.opendental.com/updates/. Since we want to use HTTPS now, we will simply go directly
			//to www.opendental.com.
			command="UPDATE preference SET ValueString='https://www.opendental.com/updates/' WHERE PrefName='UpdateWebsitePath' "
				+"AND ValueString='http://www.open-dent.com/updates/'";
			Db.NonQ(command);
			command="ALTER TABLE payment ADD ExternalId varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE payment ADD PaymentStatus tinyint NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ArchiveDoBackupFirst','1')";//Default to 'True'
			Db.NonQ(command);
			command="ALTER TABLE emailtemplate CHANGE IsHtml TemplateType tinyint(4) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE emailmessage CHANGE IsHtml HtmlType tinyint(4) NOT NULL";
			Db.NonQ(command);
			//eClipboad Prefs
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardCreateMissingFormsOnCheckIn','1')";//Default to 'True'
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardAllowSelfCheckIn','1')";//Default to 'True'
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardMessageComplete','You have successfully checked in. Please return this device to the front desk.')";//Default to 'False'
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardPopupKioskOnCheckIn','0')";//Default to 'False'
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardPresentAvailableFormsOnCheckIn','1')";//Default to 'True'
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardAllowSelfPortraitOnCheckIn','0')";//Default to 'False'
			Db.NonQ(command);
			//Default to 'True' (A new clinic with no entry in ClinicPref will implicitly have EClipboardUseDefaults==true)
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardUseDefaults','1')";
			Db.NonQ(command);
			//Set the EClipboardUseDefaults pref to true for all clinics
			command="SELECT ClinicNum FROM clinic";
			List<long> listClinicNums=Db.GetListLong(command);
			if(listClinicNums.Count>0) {
				foreach(long clinicNum in listClinicNums) {
					command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'EClipboardUseDefaults',1)"; //Default to true
					Db.NonQ(command);
				}
			}
			//add EClipboardSheetDef
			command="DROP TABLE IF EXISTS eclipboardsheetdef";
			Db.NonQ(command);
			command=@"CREATE TABLE eclipboardsheetdef (
				EClipboardSheetDefNum bigint NOT NULL auto_increment PRIMARY KEY,
				SheetDefNum bigint NOT NULL,
				ClinicNum bigint NOT NULL,
				ResubmitInterval bigint NOT NULL,
				ItemOrder int NOT NULL,
				INDEX(SheetDefNum),
				INDEX(ClinicNum),
				INDEX(ResubmitInterval)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			//There was a spelling typo in the FieldName for the X835.TransactionHandlingDescript field for ERA sheet defs.
			//Sheet defs of type ERA are not saved to the database so the only thing that needs to be updated are custom sheet field defs.
			//This typo correction does not need to be backported because the misspelled word does not get printed.  It is only used in the sheet setup.
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - ERA SheetFieldDefs");
			command="UPDATE sheetfielddef SET FieldName='TransHandlingDesc' WHERE FieldName='TransHandilingDesc'";
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1");
			//Changes to mobileappdevice
			command="ALTER TABLE mobileappdevice ADD PatNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mobileappdevice ADD INDEX (PatNum)";
			Db.NonQ(command);
			command="ALTER TABLE mobileappdevice ADD LastCheckInActivity datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
			Db.NonQ(command);
			command="SELECT DefNum FROM definition WHERE Category=38 AND ItemName='ServiceError'";//38 is DefCat.InsuranceVerificationStatus
			DataTable dtDef=Db.GetTable(command);
			int itemOrder;
			if(dtDef.Rows.Count==0) { //Def does not exist
				command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=38";//38 is DefCat.InsuranceVerificationStatus
				itemOrder=PIn.Int(Db.GetCount(command));
				command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemValue) "
					+"VALUES (38"+",'ServiceError',"+POut.Int(itemOrder)+",'SE')";//38 is DefCat.InsuranceVerificationStatus
				Db.NonQ(command);
			}
			//Insert PDMP bridge-----------------------------------------------------------------
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				+") VALUES(" 
				+"'PDMP', " 
				+"'PDMP', " 
				+"'0', " 
				+"'', "//Opens website. No local executable.
				+"'', "//leave blank if none 
				+"'')"; 
			long programNum=Db.NonQ(command,true); 
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
			   +"VALUES ("
			   +"'"+POut.Long(programNum)+"', "
			   +"'7', "//ToolBarsAvail.MainToolbar
			   +"'PDMP')";
			Db.NonQ(command);
			//Insert Illinois PDMP program properties--------------------------------------------
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(programNum)+"', " 
				+"'Illinois PDMP FacilityID', " 
				+"'')"; 
			Db.NonQ(command); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(programNum)+"', " 
				+"'Illinois PDMP Username', " 
				+"'')"; 
			Db.NonQ(command); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(programNum)+"', " 
				+"'Illinois PDMP Password', " 
				+"'')"; 
			Db.NonQ(command); 
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EraShowControlIdFilter','0')";//Default to false
			Db.NonQ(command);
			//add ScheduledProcess			
			command="DROP TABLE IF EXISTS scheduledprocess";
			Db.NonQ(command);
			command=@"CREATE TABLE scheduledprocess (
					ScheduledProcessNum bigint NOT NULL auto_increment PRIMARY KEY,
					ScheduledAction varchar(50) NOT NULL,
					TimeToRun datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					FrequencyToRun varchar(50) NOT NULL,
					LastRanDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00'
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('EClipboardClinicsSignedUp','')";
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=8";//Setup
			table=Db.GetTable(command);
			long groupNum;
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					 +"VALUES("+POut.Long(groupNum)+",177)";  //FeatureRequestEdit
				Db.NonQ(command);
			}
			#region TsiTransLogs Excluded Adjustment Type
			command=@"SELECT programproperty.ClinicNum,
				MAX(CASE WHEN programproperty.PropertyDesc='SyncExcludePosAdjType' THEN programproperty.PropertyValue END) SyncExcludePosAdjType,
				MAX(CASE WHEN programproperty.PropertyDesc='SyncExcludeNegAdjType' THEN programproperty.PropertyValue END) SyncExcludeNegAdjType
				FROM programproperty
				INNER JOIN program ON programproperty.ProgramNum=program.ProgramNum AND program.ProgName='Transworld' AND program.Enabled
				WHERE programproperty.PropertyDesc IN('SyncExcludePosAdjType','SyncExcludeNegAdjType')
				GROUP BY programproperty.ClinicNum";
			//dictionary key=ClinicNum, value=string of comma delimited adj type longs SyncExcludePosAdjType and SyncExcludeNegAdjType, i.e. "3,2"
			Dictionary<long,string> dictClinExclAdjTypes=Db.GetTable(command).Select()
				.ToDictionary(x => PIn.Long(x["ClinicNum"].ToString()),
					x => POut.Long(PIn.Long(x["SyncExcludePosAdjType"].ToString()))+","+POut.Long(PIn.Long(x["SyncExcludeNegAdjType"].ToString())));
			if(dictClinExclAdjTypes.Count>0) {
				if(dictClinExclAdjTypes.ContainsKey(0)) {//if HQ clinic has props set, use them for any clinic with no props
					command="SELECT ClinicNum FROM clinic";
					Db.GetListLong(command)
						.FindAll(x => !dictClinExclAdjTypes.ContainsKey(x))
						.ForEach(x => dictClinExclAdjTypes[x]=dictClinExclAdjTypes[0]);
				}
				command=@"SELECT tsitranslog.TsiTransLogNum
					FROM tsitranslog
					INNER JOIN adjustment ON adjustment.AdjNum=tsitranslog.FKey
					WHERE tsitranslog.FKeyType=0 "//TsiFKeyType.Adjustment
					+"AND tsitranslog.TransType=-1 "//TsiTransType.None
					+$@"AND ({
						string.Join(" OR ",dictClinExclAdjTypes
							.GroupBy(x => x.Value,x => x.Key)
							.Select(x => $@"(tsitranslog.ClinicNum IN ({string.Join(",",x.Select(y => POut.Long(y)))}) AND adjustment.AdjType IN ({x.Key}))"))
					})";
				List<long> listLogNumsToUpdate=Db.GetListLong(command);
				if(listLogNumsToUpdate.Count>0) {
					command="UPDATE tsitranslog SET TransType=10 "//TsiTransType.Excluded
						+$@"WHERE TsiTransLogNum IN({string.Join(",",listLogNumsToUpdate)})";
					Db.NonQ(command);
				}
			}
			#endregion TsiTransLogs Excluded Adjustment Type
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptPreventChangesToCompleted','0')";//Off by default
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EraAllowTotalPayments','1')";//Default to enabled for backward compatibility
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS loginattempt";
			Db.NonQ(command);
			command=@"CREATE TABLE loginattempt (
					LoginAttemptNum bigint NOT NULL auto_increment PRIMARY KEY,
					UserName varchar(255) NOT NULL,
					LoginType tinyint NOT NULL,
					DateTFail datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					INDEX(UserName(10))
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			//This was incorrectly implemented should've created a new Trojan Express Collect program link instead of enabling the Trojan program link.
			//And it shouldn't have disabled the show feature, it should have left the show feature setting and enabled the program link accordingly.
			//command="SELECT ValueString FROM preference WHERE PrefName='AccountShowTrojanExpressCollect'";
			//if(PIn.Bool(Db.GetScalar(command))) {//They had it enabled, enable the Trojan program link
			//	command="UPDATE program SET Enabled=1 WHERE ProgName='Trojan'";
			//	Db.NonQ(command);
			//	command="UPDATE preference SET ValueString='0' WHERE PrefName='AccountShowTrojanExpressCollect'";
			//	Db.NonQ(command);
			//}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('RecurringChargesAllowedWhenNoPatBal','1')";//Default to 'True'
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - Adding CanChargeWhenNoBal to creditcard");
			command="ALTER TABLE creditcard ADD CanChargeWhenNoBal tinyint NOT NULL";
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - claim table structure");
			command=@"ALTER TABLE claim ADD DateIllnessInjuryPreg date NOT NULL DEFAULT '0001-01-01',
				ADD DateIllnessInjuryPregQualifier tinyint NOT NULL,
				ADD DateOther date NOT NULL DEFAULT '0001-01-01',
				ADD DateOtherQualifier tinyint NOT NULL,
				ADD IsOutsideLab tinyint NOT NULL,
				ADD ResubmissionCode tinyint NOT NULL";
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1");
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PrePayAllowedForTpProcs','0')";//default to Off. 
			Db.NonQ(command);
			command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=29";//29 is PaySplitUnearnedType
			int order=PIn.Int(Db.GetCount(command));
			command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemValue) "
				+"VALUES (29,'Treat Plan Prepayment',"+POut.Int(order)+",'X')";//29 is PaySplitUnearnedType
			defNum=Db.NonQ(command,true);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('TpUnearnedType','"+POut.Long(defNum)+"')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('TpPrePayIsNonRefundable','0')";
			Db.NonQ(command);
			command="SELECT MAX(displayreport.ItemOrder)+1 FROM displayreport WHERE displayreport.Category=3";//DisplayReportCategory.Lists
			itemOrder=Db.GetInt(command);
			command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES ('ODHiddenPaySplits',"+POut.Int(itemOrder)
				+",'Hidden Payment Splits',3,0)";//3 is DisplayReportCategory.Lists
			long reportNumNew=Db.NonQ(command,getInsertID:true);
			long reportNum=Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODPayments'");
			List<long> listGroupNums=Db.GetListLong("SELECT UserGroupNum FROM grouppermission WHERE PermType=22 AND FKey="+POut.Long(reportNum));
			foreach(long groupNumCur in listGroupNums) {
				command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
				+"VALUES('0001-01-01',0,"+POut.Long(groupNumCur)+",22,"+POut.Long(reportNumNew)+")";
				Db.NonQ(command);
			}
			To19_2_1_LargeTableScripts();//IMPORTANT: Leave the large table scripts at the end of To19_2_0.
		}//End of 19_2_1() method

		private static void To19_2_1_LargeTableScripts() {
			//We want it to be the last thing done so that if it fails for a large customer and we have to manually finish the update script we will only
			//have to verify that these large table scripts have run.
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - claimproc table structure");//No translation in convert script.
			//If adding an index with the same name as an existing index, but with different column(s), the existing index will be dropped before adding the
			//new one.  If an index exists that has the same column(s) as an index being added, the index won't be added again regardless of the index names
			LargeTableHelper.AlterLargeTable("claimproc","ClaimProcNum",null,
				new List<Tuple<string,string>> {
					Tuple.Create("ClaimNum,ClaimPaymentNum,InsPayAmt,DateCP","indexOutClaimCovering"),
					Tuple.Create("Status,PatNum,DateCP,PayPlanNum,InsPayAmt,WriteOff,InsPayEst,ProcDate,ProcNum","indexAgingCovering")
				});
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - securitylog table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("securitylog","SecurityLogNum",null,
				new List<Tuple<string, string>> { Tuple.Create("PermType","") });//no need to send index name. only adds index if not exists
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - benefit table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("benefit","BenefitNum",
				new List<Tuple<string,string>> {
					Tuple.Create("SecDateTEntry","datetime NOT NULL DEFAULT '0001-01-01 00:00:00'"),
					Tuple.Create("SecDateTEdit","timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
				},
				new List<Tuple<string,string>> { Tuple.Create("SecDateTEntry",""),Tuple.Create("SecDateTEdit","") });//no need to send index name
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - insverify table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("insverify","InsVerifyNum",
				new List<Tuple<string,string>> { Tuple.Create("SecDateTEdit","timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP") },
				new List<Tuple<string,string>> { Tuple.Create("SecDateTEdit","") });//no need to send index name
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - insverifyhist table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("insverifyhist","InsVerifyHistNum",
				new List<Tuple<string,string>> { Tuple.Create("SecDateTEdit","timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP") },
				new List<Tuple<string,string>> { Tuple.Create("SecDateTEdit","") });//no need to send index name
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - patientnote table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("patientnote","PatNum",
				new List<Tuple<string,string>> {
					Tuple.Create("SecDateTEntry","datetime NOT NULL DEFAULT '0001-01-01 00:00:00'"),
					Tuple.Create("SecDateTEdit","timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
				},
				new List<Tuple<string,string>> { Tuple.Create("SecDateTEntry",""),Tuple.Create("SecDateTEdit","") });//no need to send index name
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - task table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("task","TaskNum",
				new List<Tuple<string,string>> { Tuple.Create("SecDateTEdit","timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP") },
				new List<Tuple<string,string>> { Tuple.Create("SecDateTEdit","") });//no need to send index name
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - taskhist table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("taskhist","TaskHistNum",
				new List<Tuple<string,string>> { Tuple.Create("SecDateTEdit","timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP") },
				new List<Tuple<string,string>> { Tuple.Create("SecDateTEdit","") });//no need to send index name
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1 - procedurelog table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("procedurelog","ProcNum",new List<Tuple<string,string>>{ Tuple.Create("Urgency","tinyint NOT NULL") });
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.2.1");//No translation in convert script.
		}

		private static void To19_2_2() {
			string command;
			DataTable table;
			//XDR locationID for headquarters, LocationIDs for other clinics will be generated through the UI
			command="SELECT ProgramNum FROM program WHERE ProgName='XDR'";
			long programNum=Db.GetLong(command);
			command=$@"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue) VALUES(
				'{POut.Long(programNum)}', 
				'Location ID', 
				'')";
			Db.NonQ(command);
			command="SELECT ValueString FROM preference WHERE PrefName='RigorousAccounting'";
			int intValue=Db.GetInt(command);//Rigorous accounting has three selection options Don't Enforce(2), AutoSplitOnly(1), EnforceFully(0)
			if(intValue==1 || intValue==2) {//if 1 or 2, prepayments to providers were always allowed. 
				command="UPDATE preference SET ValueString='1' WHERE PrefName='AllowPrepayProvider'";
				Db.NonQ(command);
			}
			command="ALTER TABLE claim MODIFY DateIllnessInjuryPregQualifier smallint NOT NULL,MODIFY DateOtherQualifier smallint NOT NULL";
			Db.NonQ(command);
		}

		private static void To19_2_3() {
			string command;
			DataTable table;
			#region HQ Only
			//We are running this section of code for HQ only
			//This is very uncommon and normally manual queries should be run instead of doing a convert script.
			command="SELECT ValueString FROM preference WHERE PrefName='DockPhonePanelShow'";
			table=Db.GetTable(command);
			if(table.Rows.Count > 0 && PIn.Bool(table.Rows[0][0].ToString())) {
				//Add new columns
				command="ALTER TABLE job ADD DateTimeConceptApproval datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
				command="ALTER TABLE job ADD DateTimeJobApproval datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
				command="ALTER TABLE job ADD DateTimeImplemented datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
				//Use JobLog rows to update correct info
				command=@"UPDATE job j 
									INNER JOIN(
										SELECT jl.JobNum,MAX(jl.DateTimeEntry) SetDate
										FROM JobLog jl
										WHERE jl.Description LIKE '%Concept approved.%'
										GROUP BY jl.JobNum) AS jl2 ON j.JobNum=jl2.JobNum
									SET DateTimeConceptApproval=jl2.SetDate";
				Db.NonQ(command);
				command=@"UPDATE job j 
									INNER JOIN (
										SELECT jl.JobNum,MAX(jl.DateTimeEntry) SetDate
										FROM JobLog jl
										WHERE jl.Description LIKE '%Job approved.%' OR jl.Description LIKE '%Changes approved.%'
										GROUP BY jl.JobNum) AS jl2 ON j.JobNum=jl2.JobNum
									SET DateTimeJobApproval=jl2.SetDate";
				Db.NonQ(command);
				command=@"UPDATE job j 
									INNER JOIN (
										SELECT jl.JobNum,MAX(jl.DateTimeEntry) SetDate
										FROM JobLog jl
										WHERE jl.Description LIKE '%Job implemented.%' OR jl.Description LIKE '%to Complete.%'
										GROUP BY jl.JobNum) AS jl2 ON j.JobNum=jl2.JobNum
									SET DateTimeImplemented=jl2.SetDate";
				Db.NonQ(command);
			}
			#endregion
			//Change the type of sheetfielddef.UiLabelMobile from varchar to text
			LargeTableHelper.AlterLargeTable("sheetfielddef","SheetFieldDefNum",new List<Tuple<string, string>>() { 
				Tuple.Create("UiLabelMobile","text NOT NULL"),
				Tuple.Create("UiLabelMobileRadioButton","text NOT NULL")
			});
			LargeTableHelper.AlterLargeTable("sheetfield","SheetFieldNum",new List<Tuple<string, string>>() { 
				Tuple.Create("UiLabelMobile","text NOT NULL"),
				Tuple.Create("UiLabelMobileRadioButton","text NOT NULL")
			});
		}

		private static void To19_2_4() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailAlertMaxConsecutiveFails','3')";
			Db.NonQ(command);
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes' AND IsHQCategory=1";
			long alertCategoryNum=Db.GetLong(command);
			//22 for alerttype EconnectorEmailTooManySendFails
			command="INSERT INTO alertcategorylink(AlertCategoryNum,AlertType) VALUES("+POut.Long(alertCategoryNum)+",'22')";
			Db.NonQ(command);
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='eServices' AND IsHQCategory=1";
			alertCategoryNum=Db.GetLong(command);
			//22 for alerttype EconnectorEmailTooManySendFails
			command="INSERT INTO alertcategorylink(AlertCategoryNum,AlertType) VALUES("+POut.Long(alertCategoryNum)+",'22')";
			Db.NonQ(command);
			command="ALTER TABLE insverify ADD INDEX (DateTimeEntry)";
			Db.NonQ(command);
			command="ALTER TABLE task ADD INDEX (DateTimeOriginal)";
			Db.NonQ(command);
			command="ALTER TABLE taskhist ADD INDEX (DateTStamp)";
			Db.NonQ(command);
		}

		private static void To19_2_7() {
			long chartDefaultDefNum=PIn.Long(Db.GetScalar("SELECT ValueString FROM preference WHERE PrefName='ChartDefaultLayoutSheetDefNum'"),false);
			Db.NonQ("DELETE FROM userodpref WHERE FKeyType=18 AND Fkey="+POut.Long(chartDefaultDefNum));//18 => DynamicChartLayout
		}

		private static void To19_2_8() {
			string command="INSERT INTO preference(PrefName,ValueString) VALUES('OpenDentalServiceComputerName','')";
			Db.NonQ(command);
			command="UPDATE claim SET CorrectionType=1 WHERE ResubmissionCode=7 AND CorrectionType=0";//CorrectionType 1 and ResubmissionCode 7 are 'Replacement'
			Db.NonQ(command);
			command="UPDATE claim SET CorrectionType=2 WHERE ResubmissionCode=8 AND CorrectionType=0";//CorrectionType 2 and ResubmissionCode 8 are 'Void'
			Db.NonQ(command);
			command="UPDATE claimformitem SET FieldName='CorrectionType' WHERE FieldName='ResubmissionCode'";
			Db.NonQ(command);
			command="ALTER TABLE claim DROP COLUMN ResubmissionCode";
			Db.NonQ(command);
		}

		private static void To19_2_15() {
			string command="INSERT INTO preference(PrefName,ValueString) VALUES('AgingProcLifo','0')";
			Db.NonQ(command);//Unset by default (same effect as Off for now, for backwards compatibility).
			command=$@"SELECT c.TABLE_NAME tableName,c.COLUMN_NAME colName,c1.priKey
				FROM information_schema.COLUMNS c
				INNER JOIN (
					SELECT TABLE_NAME,MAX(COLUMN_NAME) priKey
					FROM information_schema.COLUMNS
					WHERE TABLE_SCHEMA=DATABASE()
					AND TABLE_NAME IN ({string.Join(",",_listTableNames.Concat(_listLargeTableNames).Select(x => "'"+POut.String(x)+"'"))})
					AND COLUMN_KEY='PRI'
					GROUP BY TABLE_NAME
				) c1 ON c1.TABLE_NAME=c.TABLE_NAME
				WHERE c.TABLE_SCHEMA=DATABASE()
				AND c.DATA_TYPE='timestamp'
				AND c.TABLE_NAME IN ({string.Join(",",_listTableNames.Concat(_listLargeTableNames).Select(x => "'"+POut.String(x)+"'"))})
				AND (
					c.IS_NULLABLE='YES'
					OR (c.COLUMN_DEFAULT!='CURRENT_TIMESTAMP' AND c.COLUMN_DEFAULT!='CURRENT_TIMESTAMP()')
					OR (c.EXTRA!='ON UPDATE CURRENT_TIMESTAMP' AND c.EXTRA!='ON UPDATE CURRENT_TIMESTAMP()')
				)";
			DataTable table=Db.GetTable(command);
			string tableName;
			string columnName;
			foreach(DataRow row in table.Rows) {
				tableName=PIn.String(row["tableName"].ToString()).ToLower();
				columnName=PIn.String(row["colName"].ToString());
				if(_listLargeTableNames.Contains(tableName)) {
					LargeTableHelper.AlterLargeTable(tableName,PIn.String(row["priKey"].ToString()),
						new List<Tuple<string,string>> { Tuple.Create(columnName,"timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP") });
				}
				else if(_listTableNames.Contains(tableName)) {
					command=$@"ALTER TABLE `{tableName}` MODIFY `{columnName}` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP";
					Db.NonQ(command);
				}
			}
		}

		private static void To19_2_29() {
			string command;
			#region Move Trojan Express Collect to Program Link
			command="SELECT ValueString FROM preference WHERE PrefName='TrojanExpressCollectPath'";
			string trojanPath=PIn.String(Db.GetScalar(command));
			command="SELECT ValueString FROM preference WHERE PrefName='TrojanExpressCollectBillingType'";
			long trojanBillType=PIn.Long(Db.GetScalar(command));
			command="SELECT ValueString FROM preference WHERE PrefName='TrojanExpressCollectPassword'";
			string trojanPwd=PIn.String(Db.GetScalar(command));
			command="SELECT ValueString FROM preference WHERE PrefName='TrojanExpressCollectPreviousFileNumber'";
			int trojanPrevFileNum=PIn.Int(Db.GetScalar(command));
			command="SELECT ValueString FROM preference WHERE PrefName='AccountShowTrojanExpressCollect'";
			bool isTrojanEnabled=PIn.Bool(Db.GetScalar(command));
			if(!isTrojanEnabled) {
				//The convert script for 19.2.1.0 set the show feature pref to disabled, so if they had previously updated to 19.2.1.0 or above we need to
				//check for values in the other prefs to determine whether or not to enable this program link.
				command="SELECT ProgramVersion FROM updatehistory ORDER BY DateTimeUpdated DESC LIMIT 1";
				string programVersion=Db.GetScalar(command);
				if(string.IsNullOrEmpty(programVersion) || new Version(programVersion)>=new Version(19,2,1,0)) {
					isTrojanEnabled=trojanBillType>0 || trojanPrevFileNum>0 || !string.IsNullOrEmpty(trojanPath) || !string.IsNullOrEmpty(trojanPwd);
				}
			}
			//Create the Trojan Express Collect program using the existing show feature prefs
			command=$@"INSERT INTO program (ProgName,ProgDesc,Enabled,Note)
				VALUES('TrojanExpressCollect','Trojan Express Collect',{POut.Bool(isTrojanEnabled)},'')"; 
			long programNum=Db.NonQ(command,true);
			//Trojan Express Collect Program Property - Folder Path
			command=$@"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue)
				VALUES({POut.Long(programNum)},'FolderPath','{POut.String(trojanPath)}')"; 
			Db.NonQ(command);
			//Trojan Express Collect Program Property - Billing Type
			command=$@"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue)
				VALUES({POut.Long(programNum)},'BillingType','{POut.Long(trojanBillType)}')"; 
			Db.NonQ(command);
			//Trojan Express Collect Program Property - Password
			command=$@"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue)
				VALUES({POut.Long(programNum)},'Password','{POut.String(trojanPwd)}')"; 
			Db.NonQ(command);
			//Trojan Express Collect Program Property - Previous File Number
			command=$@"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue)
				VALUES({POut.Long(programNum)},'PreviousFileNumber','{POut.Int(trojanPrevFileNum)}')"; 
			Db.NonQ(command);
			command=$@"INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText)
				VALUES ({POut.Long(programNum)},0,'TrojanCollect')";//ToolBarsAvail.AccountModule
			Db.NonQ(command);
			#endregion Move Trojan Express Collect to Program Link
		}

		private static void To19_2_36() {
			//Add hidden pref UseAlternateOpenFileDialogWindow if it hasn't been added.
			string command = "SELECT * FROM preference WHERE preference.PrefName = 'UseAlternateOpenFileDialogWindow'";
			if(Db.GetTable(command).Rows.Count == 0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('UseAlternateOpenFileDialogWindow','0')"; //Default to false.
				Db.NonQ(command);
			}
		}

		private static void To19_2_42() {
			LargeTableHelper.AlterLargeTable("claimproc","ClaimProcNum",
					new List<Tuple<string,string>> { Tuple.Create("IsTransfer","tinyint NOT NULL") });
		}

		private static void To19_2_57() {
			//There was a 19.2.57 method backported to the 19.2 solution but it doesn't actually do anything so preserving the code isn't important.
			//This method stub is here as an indicator that there was in fact a 19.2.57 convert script method.
			//What this convert script method was supposed to do can be found in the 19.3.33 convert script method.
		}

		private static void To19_2_62() {
			DetachTransferClaimProcsFromClaimPayments();
		}

		private static void To19_3_1() {
			string command;
			DataTable table;
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.3.1 - SMS Short Codes");
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ShortCodeEConfirmationTemplate','')";//Default to blank
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.3.1");
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptsAllowOverlap','0')";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString='1' WHERE PrefName='ApptModuleUses2019Overhaul'";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('PatientPhoneUsePhonenumberTable','0')";//0 for Unknown (YN enum), behaves the same as No
			Db.NonQ(command);
			command=@"ALTER TABLE phonenumber ADD PhoneNumberDigits varchar(30) NOT NULL,
				ADD PhoneType tinyint NOT NULL,
				ADD INDEX PatPhoneDigits (PatNum,PhoneNumberDigits)";
			Db.NonQ(command);
			command="ALTER TABLE apptview ADD WidthOpMinimum smallint unsigned NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('ApptFontSize','8')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('ApptProvbarWidth','11')";
			Db.NonQ(command);
			LargeTableHelper.AlterLargeTable("appointment","AptNum",
				new List<Tuple<string,string>> { Tuple.Create("ProvBarText","varchar(60) NOT NULL") });
			LargeTableHelper.AlterLargeTable("histappointment","HistApptNum",
				new List<Tuple<string,string>> { Tuple.Create("ProvBarText","varchar(60) NOT NULL") });
			command="ALTER TABLE rxpat ADD INDEX (ProvNum)";
			Db.NonQ(command);
			command="ALTER TABLE orthocharttablink ADD ColumnWidthOverride int NOT NULL";
			Db.NonQ(command);
			if(!LargeTableHelper.IndexExists("task","TaskStatus")) {
				command="ALTER TABLE task ADD INDEX (TaskStatus)";
				Db.NonQ(command);
			}
			command="DROP TABLE IF EXISTS orthocase";
			Db.NonQ(command);
			command=@"CREATE TABLE orthocase (
				OrthoCaseNum bigint NOT NULL auto_increment PRIMARY KEY,
				PatNum bigint NOT NULL,
				ProvNum bigint NOT NULL,
				ClinicNum bigint NOT NULL,
				Fee double NOT NULL,
				FeeIns double NOT NULL,
				FeePat double NOT NULL,
				BandingDate date NOT NULL DEFAULT '0001-01-01',
				DebondDate date NOT NULL DEFAULT '0001-01-01',
				DebondDateExpected date NOT NULL DEFAULT '0001-01-01',
				IsTransfer tinyint NOT NULL,
				OrthoType bigint NOT NULL,
				SecDateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				SecUserNumEntry bigint NOT NULL,
				SecDateTEdit timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
				INDEX(PatNum),
				INDEX(ProvNum),
				INDEX(ClinicNum),
				INDEX(OrthoType),
				INDEX(SecUserNumEntry)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS orthoplanlink";
			Db.NonQ(command);
			command=@"CREATE TABLE orthoplanlink (
				OrthoPlanLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
				OrthoCaseNum bigint NOT NULL,
				LinkType tinyint NOT NULL,
				FKey bigint NOT NULL,
				IsActive tinyint NOT NULL,
				SecDateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				SecUserNumEntry date NOT NULL DEFAULT '0001-01-01',
				INDEX(OrthoCaseNum),
				INDEX(FKey)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS orthoproclink";
			Db.NonQ(command);
			command=@"CREATE TABLE orthoproclink (
				OrthoProcLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
				OrthoCaseNum bigint NOT NULL,
				ProcNum bigint NOT NULL,
				SecDateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				SecUserNumEntry bigint NOT NULL,
				INDEX(OrthoCaseNum),
				INDEX(ProcNum),
				INDEX(SecUserNumEntry)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS orthoschedule";
			Db.NonQ(command);
			command=@"CREATE TABLE orthoschedule (
				OrthoScheduleNum bigint NOT NULL auto_increment PRIMARY KEY,
				BandingDateOverride date NOT NULL DEFAULT '0001-01-01',
				DebondDateOverride date NOT NULL DEFAULT '0001-01-01',
				BandingPercent double NOT NULL,
				VisitPercent double NOT NULL,
				DebondPercent double NOT NULL,
				IsActive tinyint NOT NULL,
				SecDateTEdit timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="ALTER TABLE payplan ADD IsDynamic tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE payplancharge ADD FKey bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE payplancharge ADD INDEX (FKey)";
			Db.NonQ(command);
			command="ALTER TABLE payplancharge ADD LinkType tinyint NOT NULL";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS payplanlink";
			Db.NonQ(command);
			command=@"CREATE TABLE payplanlink (
				PayPlanLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
				PayPlanNum bigint NOT NULL,
				LinkType tinyint NOT NULL,
				FKey bigint NOT NULL,
				AmountOverride double NOT NULL,
				SecDateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				INDEX(PayPlanNum),
				INDEX(FKey)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="ALTER TABLE tasklist ADD TaskListStatus tinyint NOT NULL";//Default to 0 (Active).
			Db.NonQ(command);
			command="SELECT ProgramNum FROM program WHERE ProgName = 'XVWeb'";
			long xvWebProgramNum=PIn.Long(Db.GetScalar(command));
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				+") VALUES("
				+"'"+POut.Long(xvWebProgramNum)+"', "//xvweb
				+"'Image Quality', "
				+"'Moderate')";//Default to Moderate
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ODMobileCacheDurationHours','6')";//Defaults to 6 hours.
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EConnectorSmsNotificationFrequency','5')";//Defaults to 5 seconds.
			Db.NonQ(command);
			command="ALTER TABLE signalod ADD INDEX (IType)";
			Db.NonQ(command);
			command="ALTER TABLE smsfrommobile ADD SecDateTEdit timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP";
			Db.NonQ(command);
			//Exising rows set to DateTime the message was inserted into the DB.
			command="UPDATE smsfrommobile SET SecDateTEdit = DateTimeReceived";
			Db.NonQ(command);
			command="ALTER TABLE smsfrommobile ADD INDEX (SecDateTEdit)";
			Db.NonQ(command);
			command="ALTER TABLE smstomobile ADD SecDateTEdit timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP";
			Db.NonQ(command);
			//Exising rows set to DateTime the message was either successfully delivered or failed.
			command="UPDATE smstomobile SET SecDateTEdit = DateTimeTerminated";
			Db.NonQ(command);
			command="ALTER TABLE smstomobile ADD INDEX (SecDateTEdit)";
			Db.NonQ(command);
			if(!LargeTableHelper.IndexExists("refattach","ReferralNum")) {
				command="ALTER TABLE refattach ADD INDEX (ReferralNum)";
				Db.NonQ(command);
			}
			if(!LargeTableHelper.IndexExists("sheetfield","SheetNum,FieldType")) {
				LargeTableHelper.AlterLargeTable("sheetfield","SheetFieldNum",null,indexColsAndNames:new List<Tuple<string,string>> {
					Tuple.Create("SheetNum,FieldType","SheetNumFieldType")
				});
			}
			LargeTableHelper.AlterLargeTable("patient","PatNum",new List<Tuple<string,string>> { Tuple.Create("HasSignedTil","tinyint NOT NULL") });
			command="SELECT ValueString FROM preference WHERE PrefName='BackupReminderLastDateRun'";
			DateTime.TryParse(Db.GetScalar(command),out DateTime dateBackupReminderLastRun);
			command="SELECT ValueString FROM preference WHERE PrefName='UpdateStreamLinePassword'";
			DataTable tableStreamline=Db.GetTable(command);
			bool isStreamline=false;
			if(tableStreamline.Rows.Count > 0) {
				isStreamline=(tableStreamline.Rows[0]["ValueString"].ToString()=="abracadabra");
			}
			command="INSERT INTO preference (PrefName,ValueString) VALUES('SupplementalBackupEnabled',"
				+((isStreamline || dateBackupReminderLastRun > DateTime.Today.AddYears(10))?"'0'":"'1'")+")";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('SupplementalBackupDateLastComplete','0001-01-01 00:00:00')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('SupplementalBackupNetworkPath','')";
			Db.NonQ(command);
			command="ALTER TABLE registrationkey ADD DateTBackupScheduled datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
			Db.NonQ(command);
			command="ALTER TABLE registrationkey ADD BackupPassCode varchar(32) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE userod ADD DateTLastLogin datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
			Db.NonQ(command);
			command="ALTER TABLE payplan ADD ChargeFrequency tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE paysplit ADD PayPlanChargeNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE paysplit ADD INDEX (PayPlanChargeNum)";
			Db.NonQ(command);
			command="ALTER TABLE payplan ADD DatePayPlanStart date NOT NULL DEFAULT '0001-01-01'";
			Db.NonQ(command);
			command="ALTER TABLE sheetdef ADD DateTCreated datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
			Db.NonQ(command);
			command="UPDATE preference SET PrefName='SheetsDefaultRxInstruction' WHERE PrefName='SheetsDefaultRxInstructions'";
			Db.NonQ(command);
			command="UPDATE preference SET PrefName='SheetsDefaultChartModule' WHERE PrefName='ChartDefaultLayoutSheetDefNum'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DynamicPayPlanRunTime','2019-01-01 09:00:00')";//9 AM
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DynamicPayPlanLastDateTime','0001-01-01 00:00:00')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DynamicPayPlanStartDateTime','')";
			Db.NonQ(command);
			command="ALTER TABLE payplan ADD IsLocked tinyint NOT NULL";
			Db.NonQ(command);
		}//End of 19_3_1() method

		private static void To19_3_3() {
			//Rename the prog name to the correct name. It was misspelled when it was initially created. 
			string command="UPDATE program SET ProgName='PandaPerioAdvanced' WHERE ProgName='PandaPeriodAdvanced'";
			Db.NonQ(command);
			command="UPDATE toolbutitem SET ButtonText='PandaPerioAdvanced' WHERE ButtonText='PandaPeriodAdvanced'";
			Db.NonQ(command);
		}

		private static void To19_3_9() {
			string command="";
			#region Theme Conversion
			//Convert preference ColorTheme for theme changes. See conversion below.
			//Original(0) and Blue(1) will be converted to Gradient(0)
			//MonoFlatGray(2) and MonoFlatBlue(3) will be converted to Flat(1)
			command="SELECT ValueString FROM preference WHERE PrefName='ColorTheme'";
			long curThemeVal=Db.GetLong(command);
			int newThemeVal=(curThemeVal>1) ? 1 : 0;
			command=$"UPDATE preference SET ValueString='{POut.Int(newThemeVal)}' WHERE PrefName='ColorTheme'";
			Db.NonQ(command);
			//Convert UserTheme userodpref's to work with theme changes.
			command="SELECT * FROM userodpref WHERE FkeyType=17";//17=UserTheme
			DataTable userThemePrefs=Db.GetTable(command);
			foreach(DataRow row in userThemePrefs.Rows) {
				long priKey=PIn.Long(row["UserOdPrefNum"].ToString());
				int themeVal=PIn.Int(row["FKey"].ToString());
				int newVal=(themeVal>1) ? 1 : 0;
				Db.NonQ($"UPDATE userodpref SET Fkey={newVal} WHERE UserOdPrefNum={priKey}");
			}
			#endregion
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('MobileWebClinicsSignedUp','')";
			Db.NonQ(command);
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes' AND IsHQCategory=1";
			long odAllTypesAlertCategoryNum=Db.GetLong(command);
			if(odAllTypesAlertCategoryNum > 0) {
				//23 for SupplementalBackups
				command=$"INSERT INTO alertcategorylink(AlertCategoryNum,AlertType) VALUES({POut.Long(odAllTypesAlertCategoryNum)},23)";
				Db.NonQ(command);
			}
		}

		private static void To19_3_10() {
			string command="";
			//Prophy image on appt will no longer appear, so change the default color to something visible instead of empty
			command="UPDATE apptviewitem SET ElementColor=-6859558 WHERE ElementDesc='Prophy/PerioPastDue[P]'";
			Db.NonQ(command);
			//Recall image on appt will no longer appear, so change the default color to something visible instead of empty
			command="UPDATE apptviewitem SET ElementColor=-1822956 WHERE ElementDesc='RecallPastDue[R]'";
			Db.NonQ(command);
		}

		private static void To19_3_11() {
			string command="ALTER TABLE xwebresponse ADD OrderId varchar(255) NOT NULL";
			Db.NonQ(command);
		}

		private static void To19_3_14() {
			//Add hidden pref UseAlternateOpenFileDialogWindow if it hasn't been added.
			string command = "SELECT * FROM preference WHERE preference.PrefName = 'UseAlternateOpenFileDialogWindow'";
			if(Db.GetTable(command).Rows.Count == 0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('UseAlternateOpenFileDialogWindow','0')"; //Default to false.
				Db.NonQ(command);
			}
		}

		private static void To19_3_18() {
			if(!LargeTableHelper.ColumnExists(LargeTableHelper.GetCurrentDatabase(),"IsTransfer","claimproc")) {
				LargeTableHelper.AlterLargeTable("claimproc","ClaimProcNum",new List<Tuple<string,string>>{ Tuple.Create("IsTransfer","tinyint NOT NULL") });
			}
		}

		private static void To19_3_22() {
			string command="ALTER TABLE reseller ADD BillingType bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE reseller ADD INDEX (BillingType)";
			Db.NonQ(command);
			command="ALTER TABLE reseller ADD VotesAllotted int NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE reseller ADD Note varchar(4000) NOT NULL";
			Db.NonQ(command);
			//42 is 'No Support: Developer/Reseller'
			command="UPDATE reseller SET BillingType=42,Note='This is a customer of a reseller.  We do not directly support this customer.'";
			Db.NonQ(command);
		}

		private static void To19_3_25() {
			string command=$@"INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note)
				VALUES ('DexisIntegrator','Dexis Integrator from www.kavo.com','0','','','')";
			long programNum=Db.NonQ(command,true);
			command=$@"SELECT ProgramNum FROM program WHERE ProgName='Dexis'";
			long programNumDexis=Db.GetLong(command);
			command=$@"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue)
				SELECT {POut.Long(programNum)},PropertyDesc,PropertyValue
				FROM programproperty
				WHERE ProgramNum={POut.Long(programNumDexis)}
				AND PropertyDesc='Enter 0 to use PatientNum, or 1 to use ChartNum'";
			Db.NonQ(command);
			command=$@"INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText)
				SELECT {POut.Long(programNum)},ToolBar,'DexisIntegrator'
				FROM toolbutitem
				WHERE ProgramNum={POut.Long(programNumDexis)}";
			Db.NonQ(command);
		}

		private static void To19_3_26() {
			string command;
			if(!LargeTableHelper.IndexExists("statement","IsSent")) {
				command="ALTER TABLE statement ADD INDEX (IsSent)";
				Db.NonQ(command);
			}
			command="ALTER TABLE claim MODIFY COLUMN ClaimNote VARCHAR (400)";
			Db.NonQ(command);
		}

		private static void To19_3_29() {
			string command;
			command="ALTER TABLE timeadjust ADD PtoDefNum bigint NOT NULL DEFAULT 0";
			Db.NonQ(command);
			command="ALTER TABLE timeadjust ADD INDEX (PtoDefNum)";
			Db.NonQ(command);
			command="ALTER TABLE timeadjust ADD PtoHours time NOT NULL DEFAULT '00:00:00'";
			Db.NonQ(command);
		}

		private static void To19_3_33() {
			string command;
			//Moving codes to the Obsolete category that were deleted in CDT 2020.
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				//Move deprecated codes to the Obsolete procedure code category.
				//Make sure the procedure code category exists before moving the procedure codes.
				string procCatDescript="Obsolete";
				long defNumForCat=0;
				command="SELECT DefNum FROM definition WHERE Category=11 AND ItemName='"+POut.String(procCatDescript)+"'";//11 is DefCat.ProcCodeCats
				DataTable dtDef=Db.GetTable(command);
				if(dtDef.Rows.Count==0) { //The procedure code category does not exist, add it
					command="SELECT COUNT(*) FROM definition WHERE Category=11";//11 is DefCat.ProcCodeCats
					int countCats=PIn.Int(Db.GetCount(command));
					command="INSERT INTO definition (Category,ItemName,ItemOrder) "
								+"VALUES (11"+",'"+POut.String(procCatDescript)+"',"+POut.Int(countCats)+")";//11 is DefCat.ProcCodeCats
					defNumForCat=Db.NonQ(command,true);
				}
				else { //The procedure code category already exists, get the existing defnum
					defNumForCat=PIn.Long(dtDef.Rows[0]["DefNum"].ToString());
				}
				string[] cdtCodesDeleted=new string[] {
					"D1550",
					"D1555",
					"D8691",
					"D8692",
					"D8693",
					"D8694"
				};
				//Change the procedure codes' category to Obsolete.
				command="UPDATE procedurecode SET ProcCat="+POut.Long(defNumForCat)
					+" WHERE ProcCode IN('"+string.Join("','",cdtCodesDeleted.Select(x => POut.String(x)))+"') ";
				Db.NonQ(command);
			}//end United States CDT codes update
		}

		private static void To19_3_36() {
			string command="";
			//Add pref ReportsDoShowHiddenTPPrepayments.
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReportsDoShowHiddenTPPrepayments','0')";//Boolean, false by default.
			Db.NonQ(command);
		}

		private static void To19_3_43() {
			DetachTransferClaimProcsFromClaimPayments();
		}

		private static void To19_3_51() {//This method did not truly get added until 19.3.52.
			string command;
			if(!LargeTableHelper.IndexExists("smstomobile","GuidMessage")) {
				command="ALTER TABLE smstomobile ADD INDEX (GuidMessage)";
				Db.NonQ(command);
			}
		}

		private static void To19_3_60() {
			//The method To19_3_51() was committed just as 19.3.51 was being released, so it did not make it in until 19.3.52. We will run this again for
			//any offices that updated to 19.3.51.
			string command;
			if(!LargeTableHelper.IndexExists("smstomobile","GuidMessage")) {
				command="ALTER TABLE smstomobile ADD INDEX (GuidMessage)";
				Db.NonQ(command);
			}
		}

		private static void To19_3_62() {
			string command;
			command="INSERT INTO preference (PrefName,ValueString) VALUES('AlertCheckFrequencySeconds','180');";
			Db.NonQ(command);
			command="SELECT ValueString FROM preference WHERE PrefName='SignalInactiveMinutes'";
			string signalInactiveMinutes=Db.GetScalar(command);
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('AlertInactiveMinutes','{POut.String(signalInactiveMinutes)}');";
			Db.NonQ(command);
		}

		private static void To19_4_1() {
			string command;
			DataTable table;
			command="ALTER TABLE patientnote ADD Consent tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE labcase ADD InvoiceNum varchar(255) NOT NULL";
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.4.1 - claimproc table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("claimproc","ClaimProcNum",
				indexColsAndNames:new[] { Tuple.Create("InsSubNum,ProcNum,Status,ProcDate,PatNum,InsPayAmt,InsPayEst","indexTxFinder") }.ToList());
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.4.1 - apptreminderrule");//No translation in convert script.
			command="ALTER TABLE apptreminderrule ADD IsEnabled tinyint NOT NULL";
			Db.NonQ(command);
			command="UPDATE apptreminderrule SET IsEnabled=TSPrior!=0";//Previously, IsEnabled => TSPrior!=TimeSpan.Zero
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.4.1 - apptthankyousent");//No translation in convert script.
			command="DROP TABLE IF EXISTS apptthankyousent";
			Db.NonQ(command);
			command=@"CREATE TABLE apptthankyousent (
				ApptThankYouSentNum bigint NOT NULL auto_increment PRIMARY KEY,
				ApptNum bigint NOT NULL,
				ApptDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				ApptSecDateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				TSPrior bigint NOT NULL,
				ApptReminderRuleNum bigint NOT NULL,
				IsSmsSent tinyint NOT NULL,
				IsEmailSent tinyint NOT NULL,
				IsForSms tinyint NOT NULL,
				IsForEmail tinyint NOT NULL,
				ClinicNum bigint NOT NULL,
				PatNum bigint NOT NULL,
				PhonePat varchar(255) NOT NULL,
				ResponseDescript text NOT NULL,
				GuidMessageToMobile text NOT NULL,
				MsgTextToMobileTemplate text NOT NULL,
				MsgTextToMobile text NOT NULL,
				EmailSubjTemplate text NOT NULL,
				EmailSubj text NOT NULL,
				EmailTextTemplate text NOT NULL,
				EmailText text NOT NULL,
				DateTimeThankYouTransmit datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				Status tinyint NOT NULL,
				ShortGuidEmail varchar(255) NOT NULL,
				ShortGUID varchar(255) NOT NULL,
				INDEX(ApptNum),
				INDEX(TSPrior),
				INDEX(ApptReminderRuleNum),
				INDEX(ClinicNum),
				INDEX(PatNum),
				INDEX(ApptDateTime)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('ApptThankYouAutoEnabled','0')";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS mobiledatabyte";
			Db.NonQ(command);
			command=@"CREATE TABLE mobiledatabyte (
				MobileDataByteNum bigint NOT NULL auto_increment PRIMARY KEY,
				RawBase64Data mediumtext NOT NULL,
				RawBase64Code mediumtext NOT NULL,
				RawBase64Tag mediumtext NOT NULL,
				PatNum bigint NOT NULL,
				ActionType tinyint NOT NULL,
				DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				DateTimeExpires datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				INDEX(PatNum),
				INDEX(RawBase64Code(16))
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('MobileAutoUnlockCode','1')";//Default to true
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.4.1 - appointment SecDateTEntry");//No translation in convert script.
			command=$"ALTER TABLE appointment CHANGE COLUMN SecDateEntry SecDateTEntry DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00'";
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.4.1 - histappointment SecDateTEntry");//No translation in convert script.
			command=$"ALTER TABLE histappointment CHANGE COLUMN SecDateEntry SecDateTEntry DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00'";
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.4.1 - patient table structure");//No translation in convert script.
			LargeTableHelper.AlterLargeTable("patient","PatNum",indexColsAndNames:new[] { Tuple.Create("PriProv",""),Tuple.Create("SecProv","") }.ToList());
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 19.4.1 - proctp CatPercUCR");//No translation in convert script.
			command="ALTER TABLE proctp ADD CatPercUCR double NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE computerpref ADD HelpButtonXAdjustment double NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE centralconnection ADD HasClinicBreakdownReports tinyint(4) NOT NULL";
			Db.NonQ(command);
			command="UPDATE centralconnection SET HasClinicBreakdownReports=1";//Default to true
			Db.NonQ(command);
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('PatientSelectFilterRestrictedClinics','0')";//Default to false
			Db.NonQ(command);
			//Add PatientSSNView permission to everyone------------------------------------------------------
			//Grant the PatientSSNView permission to all user groups to preserve old behavior.
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				   +"VALUES("+POut.Long(groupNum)+",181)";//PatientSSNView
				Db.NonQ(command);
			}
			//Add PatientSSNMasked preference, on by default for US only.
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				command="INSERT INTO preference(PrefName,ValueString) VALUES('PatientSSNMasked','1')";
			}
			else {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('PatientSSNMasked','0')";
			}
			Db.NonQ(command);
			//Widen the default SSN column for the Patient Select window from 65 to 70 because we will now format the SSN field with hyphens for US.
			//displayfield.Category of 1 is PatientSelect.
			command=@"UPDATE displayfield SET ColumnWidth=70
				WHERE Category=1
				AND InternalName='SSN'
				AND ColumnWidth=65";
			Db.NonQ(command);
			command="ALTER TABLE apptreminderrule ADD TemplateAutoReply text NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE apptreminderrule ADD TemplateAutoReplyAgg text NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE apptreminderrule ADD IsAutoReplyEnabled tinyint NOT NULL";
			Db.NonQ(command);
			command="UPDATE apptreminderrule SET TemplateAutoReply='Thank you for confirming your appointment with [OfficeName].  We look forward to seeing you.' WHERE TypeCur=1";//ConfirmationFurtureDay
			Db.NonQ(command);
			command="UPDATE apptreminderrule SET TemplateAutoReplyAgg='Thank you for confirming your appointments with [OfficeName].  We look forward to seeing you.' WHERE TypeCur=1";//ConfirmationFurtureDay
			Db.NonQ(command);
			command="ALTER TABLE confirmationrequest ADD ApptReminderRuleNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE confirmationrequest ADD INDEX (ApptReminderRuleNum)";
			Db.NonQ(command);
			//Add PatientDOBView permission to everyone------------------------------------------------------
			//Grant the PatientDOBView permission to all user groups to preserve old behavior.
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			groupNum=0;
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				   +"VALUES("+POut.Long(groupNum)+",182)";//PatientDOBView
				Db.NonQ(command);
			}
			//Add PatientDOBMasked preference, off by default.
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PatientDOBMasked','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('CentralManagerUseDynamicMode','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('CentralManagerIsAutoLogon','0')";
			Db.NonQ(command);
			command="ALTER TABLE orthocase ADD IsActive tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE orthoproclink ADD ProcLinkType tinyint NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('OrthoBandingCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('OrthoDebondCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('OrthoVisitCodes','')";
			Db.NonQ(command);
			command="ALTER TABLE orthoschedule CHANGE VisitPercent VisitAmount double NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE orthoschedule CHANGE BandingPercent BandingAmount double NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE orthoschedule CHANGE DebondPercent DebondAmount double NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE orthoplanlink MODIFY SecUserNumEntry bigint NOT NULL";
			Db.NonQ(command);
			LargeTableHelper.AlterLargeTable("appointment","AptNum",
				new List<Tuple<string,string>> { Tuple.Create("PatternSecondary","varchar(255) NOT NULL") });
			LargeTableHelper.AlterLargeTable("histappointment","HistApptNum",
				new List<Tuple<string,string>> { Tuple.Create("PatternSecondary","varchar(255) NOT NULL") });
			command="ALTER TABLE supply ADD OrderQty int NOT NULL";
			Db.NonQ(command);
			if(!LargeTableHelper.IndexExists("rxpat","PatNum")) {
				command="ALTER TABLE rxpat ADD INDEX (PatNum)";
				Db.NonQ(command);
			}
			command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=10";//10 is PaymentTypes
			int maxOrder=PIn.Int(Db.GetCount(command));
			command="INSERT INTO definition (Category,ItemOrder,ItemName) VALUES (10,"+POut.Int(maxOrder)+",'From API')";//10 is PaymentTypes
			long defNum=Db.NonQ(command,true);
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('ApiPaymentType','{defNum}')";
			Db.NonQ(command);
			command="ALTER TABLE payconnectresponseweb ADD IsTokenSaved tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE payconnectresponseweb ADD PayToken varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE payconnectresponseweb ADD ExpDateToken varchar(255) NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PaymentsTransferPatientIncomeOnly','0')";//Boolean, false by default.
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('SecurityLogOffAllowUserOverride','0')";//Boolean, false by default.
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('BrokenApptRequiredOnMove','0')";//Boolean, false by default.
			Db.NonQ(command);
			command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=27";//27 is CommLogTypes
			maxOrder=PIn.Int(Db.GetCount(command));
			command=$@"INSERT INTO definition (Category,ItemName,ItemOrder,ItemValue) 
				VALUES(27,'FHIR',{POut.Int(maxOrder)},'FHIR')";//27 is CommLogTypes
			Db.NonQ(command);
			command="ALTER TABLE supplyorder ADD DateReceived date NOT NULL DEFAULT '0001-01-01'";
			Db.NonQ(command);
			command="UPDATE supplyorder SET DateReceived = CURDATE()";
			Db.NonQ(command);
			//FeeSchedGroup
			command="DROP TABLE IF EXISTS feeschedgroup";
			Db.NonQ(command);
			command=@"CREATE TABLE feeschedgroup (
					FeeSchedGroupNum bigint NOT NULL auto_increment PRIMARY KEY,
					Description varchar(255) NOT NULL,
					FeeSchedNum bigint NOT NULL,
					ClinicNums varchar(255) NOT NULL,
					INDEX(FeeSchedNum)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('ShowFeeSchedGroups','0')";
			Db.NonQ(command);
		}//End of 19_4_1() method

		private static void To19_4_2() {
			string command="";
			command="ALTER TABLE payconnectresponseweb ADD RefNumber varchar(255) NOT NULL";
			Db.NonQ(command);
		}

		private static void To19_4_3() {
			string command="";
			//Add pref ReportsDoShowHiddenTPPrepayments if it hasn't been added.
			command = "SELECT * FROM preference WHERE preference.PrefName = 'ReportsDoShowHiddenTPPrepayments'";
			if(Db.GetTable(command).Rows.Count == 0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ReportsDoShowHiddenTPPrepayments','0')";//Boolean, false by default.
				Db.NonQ(command);
			}
			if(!LargeTableHelper.IndexExists("preference","PrefName")) {
				command="ALTER TABLE preference ADD INDEX (PrefName)";
				Db.NonQ(command);
			}
			command="ALTER TABLE payconnectresponseweb ADD TransType varchar(255) NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ShortCodeOptInOnApptComplete','1')";//Boolean, true by default.
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ShortCodeOptInScript','')";//String, can be updated by HQ via eServices sync.
			Db.NonQ(command);
			LargeTableHelper.AlterLargeTable("patient","PatNum",new List<Tuple<string,string>> { Tuple.Create("ShortCodeOptIn","tinyint NOT NULL") });
		}

		private static void To19_4_9() {
			string command="";
			command="ALTER TABLE orthocase CHANGE FeeIns FeeInsPrimary double NOT NULL, ADD FeeInsSecondary double NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ShortCodeOptedOutScript','')";//String, will be updated by HQ via eServices sync.
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ShortCodeOptInClinicTitle','')";//String, empty defaults to PracticeTitle.
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ShortCodeOptInOnApptCompleteOffScript','')";//String, will be updated by HQ via eServices sync.
			Db.NonQ(command);
			DetachTransferClaimProcsFromClaimPayments();
			command="INSERT INTO preference (PrefName,ValueString) VALUES('TreatPlanPromptSave','1')"; //Boolean, true by default.
			Db.NonQ(command);
		}

		private static void To19_4_12() {
			string command=@"INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note)
				 VALUES(
				 'BencoPracticeManagement', 
				 'Benco Practice Management', 
				 '0', 
				 'https://identity.benco.com/auth/login',
				 '',
				 '');";
			long programNum=Db.NonQ(command,true);
			command=$"INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) VALUES ({POut.Long(programNum)},7,'Benco');"; //7 = Main Toolbar
			Db.NonQ(command);
			//We are running this section of code for HQ only
			//This is very uncommon and normally manual queries should be run instead of doing a convert script.
			command="SELECT ValueString FROM preference WHERE PrefName='DockPhonePanelShow'";
			DataTable tableTriage=Db.GetTable(command);
			if(tableTriage.Rows.Count > 0 && PIn.Bool(tableTriage.Rows[0][0].ToString())) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('TriageCallsWarning','35')";
				Db.NonQ(command);
			}
		}

		private static void To19_4_15() {
			//Gmail OAuth2.0
			string command="ALTER TABLE emailaddress ADD AccessToken varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE emailaddress ADD RefreshToken varchar(255) NOT NULL";
			Db.NonQ(command);
		}

		private static void To19_4_17() {
			string command;
			if(!LargeTableHelper.IndexExists("smstomobile","GuidMessage")) {
				command="ALTER TABLE smstomobile ADD INDEX (GuidMessage)";
				Db.NonQ(command);
			}
		}

		private static void To19_4_20() {
			string command;
			command="SELECT COUNT(*) FROM preference WHERE PrefName='AlertCheckFrequencySeconds'";
			//This preference might have already been added in 19.3.62.
			if(Db.GetScalar(command)=="0") {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('AlertCheckFrequencySeconds','180');";
				Db.NonQ(command);
				command="SELECT ValueString FROM preference WHERE PrefName='SignalInactiveMinutes'";
				string signalInactiveMinutes=Db.GetScalar(command);
				command=$"INSERT INTO preference (PrefName,ValueString) VALUES('AlertInactiveMinutes','{POut.String(signalInactiveMinutes)}');";
				Db.NonQ(command);
			}
		}

		private static void To19_4_30() {
			string command;
			//Remove "From API" PaymentType
			command="SELECT COALESCE(MIN(DefNum),0) FROM definition WHERE ItemName='From API' AND Category=10";//10 is PaymentTypes
			long defNum=PIn.Long(Db.GetScalar(command));
			if(defNum > 0) {
				//The definition still exists.
				//Update their "ApiPaymentType" pref to the first in the category
				command="SELECT DefNum FROM definition WHERE Category=10 AND IsHidden=0 ORDER BY ItemOrder LIMIT 1";
				long newDef=PIn.Long(Db.GetScalar(command));
				command=$"UPDATE preference SET ValueString='{POut.Long(newDef)}' WHERE PrefName='ApiPaymentType' AND ValueString='{POut.Long(defNum)}'";
				Db.NonQ(command);
				command=$"SELECT COUNT(*) FROM payment WHERE PayType={POut.Long(defNum)}";
				if(Db.GetScalar(command)=="0") {
					//There are no payments using this pay type, so delete "From API"
					command=$"DELETE FROM definition WHERE DefNum={POut.Long(defNum)}";
					Db.NonQ(command);
				}
			}
		}

		private static void To20_1_1() {
			string command;
			DataTable table;
			command="ALTER TABLE apptthankyousent DROP COLUMN IsSmsSent;";
			Db.NonQ(command);
			command="ALTER TABLE apptthankyousent DROP COLUMN IsEmailSent;";
			Db.NonQ(command);
			command="ALTER TABLE apptthankyousent DROP COLUMN Status;";
			Db.NonQ(command);
			command="ALTER TABLE apptthankyousent ADD SmsSentStatus tinyint NOT NULL;";
			Db.NonQ(command);
			command="ALTER TABLE apptthankyousent ADD EmailSentStatus tinyint NOT NULL;";
			Db.NonQ(command);
			command=@"SELECT GROUP_CONCAT(preference.ValueString)
				FROM preference
				WHERE preference.PrefName IN ('AppointmentTimeArrivedTrigger','AppointmentTimeSeatedTrigger','AppointmentTimeDismissedTrigger')
				AND preference.ValueString!=''";
			string apptTriggerDefNums=Db.GetScalar(command);
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('ApptConfirmExcludeEThankYou','{apptTriggerDefNums}');";
			Db.NonQ(command);
			//Any patient configured to opt out of eReminders and eConfirmations for TEXT and/or EMAIL should also be opted out for eThankYous.
			command="INSERT INTO commoptout (PatNum,CommType,CommMode) SELECT PatNum,3 AS CommType,CommMode FROM commoptout GROUP BY PatNum,CommMode;";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS inseditpatlog";
			Db.NonQ(command);
			command=@"CREATE TABLE inseditpatlog (
				InsEditPatLogNum bigint NOT NULL auto_increment PRIMARY KEY,
				FKey bigint NOT NULL,
				LogType tinyint NOT NULL,
				FieldName varchar(255) NOT NULL,
				OldValue varchar(255) NOT NULL,
				NewValue varchar(255) NOT NULL,
				UserNum bigint NOT NULL,
				DateTStamp timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
				ParentKey bigint NOT NULL,
				Description varchar(255) NOT NULL,
				INDEX FkLogType (FKey,LogType),
				INDEX(UserNum),
				INDEX(ParentKey)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="ALTER TABLE mountitemdef ADD ItemOrder int NOT NULL;";
			Db.NonQ(command);
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('ApptThankYouCalendarTitle','Dental Appointment');";
			Db.NonQ(command);
			command="ALTER TABLE apptthankyousent ADD DoNotResend tinyint NOT NULL";
			Db.NonQ(command);
			command=@"INSERT INTO clearinghouse (Description,ExportPath,Payors,Eformat,ISA05,SenderTin,ISA07,ISA08,ISA15,Password,ResponsePath,CommBridge,
				ClientProgram,LastBatchNumber,ModemPort,LoginID,SenderName,SenderTelephone,GS03,ISA02,ISA04,ISA16,SeparatorData,SeparatorSegment,IsClaimExportAllowed)
				VALUES ('EdsMedical','','',6,'ZZ','','ZZ','EDS','P','','',19,'',0,0,'','','','EDS','','','','','',0)";
			Db.NonQ(command);
			command="UPDATE clearinghouse SET HqClearinghouseNum=ClearinghouseNum WHERE HqClearinghouseNum=0";
			Db.NonQ(command);
			//==Jordan I can see remnants of a bug, where many toothchart drawings in the database are duplicated.
			//I can't find in the code how this could happen, so I assume it's an old bug that has already been fixed.
			//I decided to fix the problem here, once, instead of adding clutter to dbmaint.
			command=@"DELETE t1 FROM toothinitial t1
				INNER JOIN toothinitial t2 
				WHERE t1.InitialType=9 AND t2.InitialType=9
				AND t1.PatNum = t2.PatNum
				AND t1.ToothInitialNum < t2.ToothInitialNum 
				AND t1.DrawingSegment = t2.DrawingSegment";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PatientSelectShowInactive','1')";
			Db.NonQ(command);
			command="ALTER TABLE emailmessage ADD SecDateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00'," +
				"ADD SecDateTEdit timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP," +
				"ADD INDEX (SecDateTEntry)," +
				"ADD INDEX (SecDateTEdit)";
			Db.NonQ(command);
			command="ALTER TABLE patplan ADD SecDateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00'," +
				"ADD SecDateTEdit timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP," +
				"ADD INDEX (SecDateTEdit)," +
				"ADD INDEX (SecDateTEntry)";
			Db.NonQ(command);
			command="ALTER TABLE procmultivisit ADD SecDateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00'," +
				"ADD SecDateTEdit timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP," +
				"ADD INDEX (SecDateTEntry)," +
				"ADD INDEX (SecDateTEdit)";
			Db.NonQ(command);
			LargeTableHelper.AlterLargeTable("periomeasure","PerioMeasureNum",
				new List<Tuple<string,string>> { Tuple.Create("SecDateTEntry","datetime NOT NULL DEFAULT '0001-01-01 00:00:00'"),
					Tuple.Create("SecDateTEdit","timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP") }
				,new List<Tuple<string,string>> {Tuple.Create("SecDateTEntry",""),Tuple.Create("SecDateTEdit","") });
			LargeTableHelper.AlterLargeTable("toothinitial","ToothInitialNum",
				new List<Tuple<string,string>> { Tuple.Create("SecDateTEntry","datetime NOT NULL DEFAULT '0001-01-01 00:00:00'"),
					Tuple.Create("SecDateTEdit","timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP") }
				,new List<Tuple<string, string>>{ Tuple.Create("SecDateTEntry",""),Tuple.Create("SecDateTEdit","")});
			command="UPDATE document SET ImgType=1 WHERE ImgType=4";//getting rid of an unused enumeration item
			Db.NonQ(command);
			command="ALTER TABLE mount DROP ImgType";
			Db.NonQ(command);
			command=@"ALTER TABLE mount 
				CHANGE Description Description VARCHAR(255) NOT NULL,
				CHANGE Note Note TEXT NOT NULL,
				CHANGE Width Width INT NOT NULL,
				CHANGE Height Height INT NOT NULL";
			Db.NonQ(command);
			command=@"ALTER TABLE mountitem 
				CHANGE Width Width INT NOT NULL,
				CHANGE Height Height INT NOT NULL";
			Db.NonQ(command);
			command=@"ALTER TABLE mountitem 
				CHANGE OrdinalPos ItemOrder INT NOT NULL";
			Db.NonQ(command);
			command=@"ALTER TABLE mountdef
				DROP IsRadiograph";
			Db.NonQ(command);
			command=@"ALTER TABLE mount
				CHANGE DateCreated DateCreated datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
			Db.NonQ(command);
			if(!LargeTableHelper.IndexExists("webschedrecall","GuidMessageToMobile")) {
				command="ALTER TABLE webschedrecall ADD INDEX (GuidMessageToMobile(10))";//Guids are 36 characters.
				Db.NonQ(command);
			}
			//PreXion Viewer Bridge
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
			 +") VALUES(" 
			 +"'PreXionViewer', " 
			 +"'PreXion Viewer', " 
			 +"'0', " 
			 +"'"+POut.String(@"C:\PreXion3DViewer\PreXion3DViewer.exe")+"', " 
			 +"'', "//leave blank if none 
			 +"'')"; 
			long programNum=Db.NonQ(command,true); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Enter 0 to use PatientNum, or 1 to use ChartNum', " 
				 +"'1')"; 
			Db.NonQ(command); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Username', " 
				 +"'shared')"; 
			Db.NonQ(command); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Password', " 
				 +"'shared')"; 
			Db.NonQ(command); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Server Name', " 
				 +"'server')"; 
			Db.NonQ(command); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Port', " 
				 +"'1200')"; 
			Db.NonQ(command); 
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
				 +"VALUES (" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'2', "//ToolBarsAvail.ChartModule 
				 +"'PreXion Viewer')"; 
			Db.NonQ(command); 
			//PreXion Acquire Bridge
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				 +") VALUES(" 
				 +"'PreXionAquire', " 
				 +"'PreXion Aquire', " 
				 +"'0', " 
				 +"'"+POut.String(@"C:\PX2Console\PX2Console.exe")+"', " 
				 +"'', "//leave blank if none 
				 +"'')"; 
			programNum=Db.NonQ(command,true); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Enter 0 to use PatientNum, or 1 to use ChartNum', " 
				 +"'0')"; 
			Db.NonQ(command); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Birthdate format', " 
				 +"'yyyy/MM/dd')"; 
			Db.NonQ(command); 
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
				 +"VALUES (" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'2', "//ToolBarsAvail.ChartModule 
				 +"'PreXion Aquire')"; 
			Db.NonQ(command); 
			command="SELECT MAX(displayreport.ItemOrder) FROM displayreport WHERE displayreport.Category = 2"; //monthly 
			long itemorder=Db.GetLong(command)+1; //get the next available ItemOrder for the Production Category to put this new report last.
			command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden,IsVisibleInSubMenu) "
				 +"VALUES('ODDynamicPayPlanOvercharged',"+POut.Long(itemorder)+",'Dynamic Payment Plans Overcharged',2,0,0)";
			long reportNumNew=Db.NonQ(command,getInsertID:true);
			long reportNum=Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODPaymentPlans'");
			List<long> listGroupNums=Db.GetListLong("SELECT UserGroupNum FROM grouppermission WHERE PermType=22 AND FKey="+POut.Long(reportNum));
			foreach(long groupNumCur in listGroupNums) {
				command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
					 +"VALUES('0001-01-01',0,"+POut.Long(groupNumCur)+",22,"+POut.Long(reportNumNew)+")";
				Db.NonQ(command);
			}
			//There was a bug introduced that created an invalid tooth range when the user selected teeth from both mandibular and maxillary teeth.
			//The bug was fixed in v19.2.23 with job #16655 but databases were left in an invalid state.  This method should correct toothrange when stored 
			//as a Number situations. This will not address any merged Letters for primary teeth.  Use DBM tool for that rare situation.  We 
			//can correct this issue because we always store tooth ranges in US nomenclature (predictable numbers).
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 20.1.1 - Proc ToothRange");//No translation in convert script.
			command="SELECT ProcNum,ToothRange FROM procedurelog WHERE ToothRange REGEXP '[0-9]{3}'";//3 or 4 numbers
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				long procNum=PIn.Long(table.Rows[i]["ProcNum"].ToString());
				string toothRangeOriginal=PIn.String(table.Rows[i]["ToothRange"].ToString());
				string toothRangeUpdate="";
				foreach(string toothRange in toothRangeOriginal.Split(',')) {
					string toothRangeCur;
					if(toothRange.Length>2) {
						toothRangeCur=toothRange.Insert(toothRange.Length-2,",");
					}
					else {
						toothRangeCur=toothRange;
					}
					toothRangeUpdate+=toothRangeCur+",";
				}
				command="UPDATE procedurelog SET ToothRange='"+POut.String(toothRangeUpdate.TrimEnd(','))+"' "
					+"WHERE ProcNum="+POut.Long(procNum);
				Db.NonQ(command);
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 20.1.1");//No translation in convert script.
			}
			command="ALTER TABLE apptreminderrule ADD Language varchar(255) NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('NotesProviderSignatureOnly','0')";//false by default
			Db.NonQ(command);
			command="ALTER TABLE payplan ADD DateInterestStart date NOT NULL DEFAULT '0001-01-01'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ImagesModuleUsesOld2020','0')";//false by default
			Db.NonQ(command);
			//Add permission to groups with existing permission------------------------------------------------------
			//F18498 features a permission overhaul that removes the concepts of full and limited permissions and created new permissions to take the
			//place of those two choices. We now have separate permissions for completed procedures that include the addition of adding an adjustment,
			//editing the note on a completed procedure, and checking off some misc fields. 
			command="SELECT UserGroupNum,NewerDate,NewerDays FROM grouppermission WHERE PermType=10";//ProcComplEdit
			table=Db.GetTable(command);
			List<long> listUserGroupNumsFullPerm=new List<long>();
			foreach(DataRow row in table.Rows) {
				long userGroupNum=PIn.Long(row["UserGroupNum"].ToString());
				DateTime groupNewerDate=PIn.Date(row["NewerDate"].ToString());
				long groupNewerDays=PIn.Long(row["NewerDays"].ToString());
				if(listUserGroupNumsFullPerm.Contains(userGroupNum)) {
					//Following GroupPermissions.HasPermission cache pattern, only addressing first UserGroupNum in list
					continue;
				}
				listUserGroupNumsFullPerm.Add(userGroupNum);
				command=$"INSERT INTO grouppermission (UserGroupNum,NewerDate,NewerDays,PermType) VALUES ";
				//The next four are new permissions and will be inherited by usergroups with Full permission.
				command+=$"({POut.Long(userGroupNum)},{POut.Date(groupNewerDate)},{POut.Long(groupNewerDays)},185),"//edit status of procedure permission
					+$"({POut.Long(userGroupNum)},{POut.Date(groupNewerDate)},{POut.Long(groupNewerDays)},186),"//add adjustment permission
					+$"({POut.Long(userGroupNum)},{POut.Date(groupNewerDate)},{POut.Long(groupNewerDays)},187),"//misc edit permission
					+$"({POut.Long(userGroupNum)},{POut.Date(groupNewerDate)},{POut.Long(groupNewerDays)},188),"//completed procedure note edit
					+$"({POut.Long(userGroupNum)},{POut.Date(groupNewerDate)},{POut.Long(groupNewerDays)},189);";//completed procedure edit
				Db.NonQ(command);
			}
			command="SELECT UserGroupNum,NewerDate,NewerDays FROM grouppermission WHERE PermType=117";//ProcComplEditLimited permission
			table=Db.GetTable(command);
			List<long> listUserGroupNumsLimitedPerm=new List<long>();
			foreach(DataRow row in table.Rows) {
				long userGroupNum=PIn.Long(row["UserGroupNum"].ToString());
				DateTime groupNewerDate=PIn.Date(row["NewerDate"].ToString());
				long groupNewerDays=PIn.Long(row["NewerDays"].ToString());
				if(listUserGroupNumsLimitedPerm.Contains(userGroupNum) || listUserGroupNumsFullPerm.Contains(userGroupNum)) {
					//Following GroupPermissions.HasPermission cache pattern, only addressing first UserGroupNum in list
					//Or, if we already added these permission to this usergroup when migrating usergroups with Full permission.
					continue;
				}
				listUserGroupNumsLimitedPerm.Add(userGroupNum);
				//Limited 3 new permissions
				command=$"INSERT INTO grouppermission (UserGroupNum,NewerDate,NewerDays,PermType) VALUES "
					+$"({POut.Long(userGroupNum)},{POut.Date(groupNewerDate)},{POut.Long(groupNewerDays)},186),"//adjustment edit permission
					+$"({POut.Long(userGroupNum)},{POut.Date(groupNewerDate)},{POut.Long(groupNewerDays)},187),"//misc edit permission
					+$"({POut.Long(userGroupNum)},{POut.Date(groupNewerDate)},{POut.Long(groupNewerDays)},188);";//completed procedure note edit
				Db.NonQ(command);
			}
			command="ALTER TABLE sheetfielddef ADD Language varchar(255) NOT NULL";
			Db.NonQ(command);
		}//End of 20_1_1() method
		
		private static void To20_1_3() {
			string command;
			DataTable table;
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('MassEmailStatus','0')";//not activated
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS emailhostingtemplate";
			Db.NonQ(command);
			command=@"CREATE TABLE emailhostingtemplate (
				EmailHostingTemplateNum bigint NOT NULL auto_increment PRIMARY KEY,
				TemplateName varchar(255) NOT NULL,
				Subject text NOT NULL,
				BodyPlainText text NOT NULL,
				BodyHTML text NOT NULL,
				TemplateId bigint NOT NULL,
				ClinicNum bigint NOT NULL,
				INDEX(TemplateId),
				INDEX(ClinicNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS promotionlog";
			Db.NonQ(command);
			command=@"CREATE TABLE promotionlog (
				PromotionLogNum bigint NOT NULL auto_increment PRIMARY KEY,
				PatNum bigint NOT NULL,
				CommlogNum bigint NOT NULL,
				DateTimePromotion datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				DateTimeSent datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				PromotionStatus tinyint NOT NULL,
				TypePromotion tinyint NOT NULL,
				INDEX(PatNum),
				INDEX(CommlogNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				+") VALUES("
				+"'CareCredit', "
				+"'CareCredit from https://www.carecredit.com/', "
				+"'0', "
				+"'', "
				+"'', "
				+"'')";
			long programNum=Db.NonQ(command,true);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				 +") VALUES("
				 +"'"+POut.Long(programNum)+"', "
				 +"'QSBatchEnabled', "
				 +"'0')";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS carecreditwebresponse";
			Db.NonQ(command);
			command=@"CREATE TABLE carecreditwebresponse (
					CareCreditWebResponseNum bigint NOT NULL auto_increment PRIMARY KEY,
					PatNum bigint NOT NULL,
					PayNum bigint NOT NULL,
					RefNumber varchar(255) NOT NULL,
					Amount double NOT NULL,
					WebToken varchar(255) NOT NULL,
					ProcessingStatus varchar(255) NOT NULL,
					DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					DateTimePending datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					DateTimeCompleted datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					DateTimeExpired datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					DateTimeLastError datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					LastResponseStr text NOT NULL,
					INDEX(PatNum),
					INDEX(PayNum)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="ALTER TABLE employee ADD WirelessPhone varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE employee ADD EmailWork varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE employee ADD EmailPersonal varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE employee ADD IsFurloughed tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE employee ADD IsWorkingHome tinyint NOT NULL";
			Db.NonQ(command);
		}

		private static void To20_1_5() {
			string command;
			DataTable table;
			List<string> listAlters=new List<string>();
			//Appointment
			if(!LargeTableHelper.IndexExists("appointment","UnschedStatus")) {
				listAlters.Add("ADD INDEX (UnschedStatus)");
			}
			if(!LargeTableHelper.IndexExists("appointment","ClinicNum")) {
				listAlters.Add("ADD INDEX (ClinicNum)");
			}
			if(listAlters.Count>0) {
				command="ALTER TABLE appointment "+string.Join(",",listAlters);
				Db.NonQ(command);
				listAlters.Clear();
			}
			//ApptViewItem
			if(!LargeTableHelper.IndexExists("apptviewitem","ProvNum")) {
				command="ALTER TABLE apptviewitem ADD INDEX (ProvNum)";
				Db.NonQ(command);
			}
			//Carrier
			if(!LargeTableHelper.IndexExists("carrier","CanadianNetworkNum")) {
				listAlters.Add("ADD INDEX (CanadianNetworkNum)");
			}
			if(!LargeTableHelper.IndexExists("carrier","CarrierGroupName")) {
				listAlters.Add("ADD INDEX (CarrierGroupName)");
			}
			if(listAlters.Count>0) {
				command="ALTER TABLE carrier "+string.Join(",",listAlters);
				Db.NonQ(command);
				listAlters.Clear();
			}
			//Claim
			if(!LargeTableHelper.IndexExists("claim","ProvBill")) {
				listAlters.Add("ADD INDEX (ProvBill)");
			}
			if(!LargeTableHelper.IndexExists("claim","ProvTreat")) {
				listAlters.Add("ADD INDEX (ProvTreat)");
			}
			if(!LargeTableHelper.IndexExists("claim","ClinicNum")) {
				listAlters.Add("ADD INDEX (ClinicNum)");
			}
			if(listAlters.Count>0) {
				command="ALTER TABLE claim "+string.Join(",",listAlters);
				Db.NonQ(command);
				listAlters.Clear();
			}
			//ClaimAttach
			if(!LargeTableHelper.IndexExists("claimattach","ClaimNum")) {
				command="ALTER TABLE claimattach ADD INDEX (ClaimNum)";
				Db.NonQ(command);
			}
			//ClaimPayment
			if(!LargeTableHelper.IndexExists("claimpayment","ClinicNum")) {
				command="ALTER TABLE claimpayment ADD INDEX (ClinicNum)";
				Db.NonQ(command);
			}
			//Clinic
			if(!LargeTableHelper.IndexExists("clinic","InsBillingProv")) {
				command="ALTER TABLE clinic ADD INDEX (InsBillingProv)";
				Db.NonQ(command);
			}
			//Commlog
			if(!LargeTableHelper.IndexExists("commlog","UserNum")) {
				command="ALTER TABLE commlog ADD INDEX (UserNum)";
				Db.NonQ(command);
			}
			//Covspan
			if(!LargeTableHelper.IndexExists("covspan","CovCatNum")) {
				command="ALTER TABLE covspan ADD INDEX (CovCatNum)";
				Db.NonQ(command);
			}
			//Etrans
			if(!LargeTableHelper.IndexExists("etrans","ClearinghouseNum")) {
				listAlters.Add("ADD INDEX (ClearinghouseNum)");
			}
			if(!LargeTableHelper.IndexExists("etrans","PatNum")) {
				listAlters.Add("ADD INDEX (PatNum)");
			}
			if(!LargeTableHelper.IndexExists("etrans","AckEtransNum")) {
				listAlters.Add("ADD INDEX (AckEtransNum)");
			}
			if(listAlters.Count>0) {
				command="ALTER TABLE etrans "+string.Join(",",listAlters);
				Db.NonQ(command);
				listAlters.Clear();
			}
			//Fee
			if(!LargeTableHelper.IndexExists("fee","ProvNum")) {
				command="ALTER TABLE fee ADD INDEX (ProvNum)";
				Db.NonQ(command);
			}
			//Grouppermission
			if(!LargeTableHelper.IndexExists("grouppermission","UserGroupNum")) {
				command="ALTER TABLE grouppermission ADD INDEX (UserGroupNum)";
				Db.NonQ(command);
			}
			//Insplan
			if(!LargeTableHelper.IndexExists("insplan","FeeSched")) {
				listAlters.Add("ADD INDEX (FeeSched)");
			}
			if(!LargeTableHelper.IndexExists("insplan","CopayFeeSched")) {
				listAlters.Add("ADD INDEX (CopayFeeSched)");
			}
			if(listAlters.Count>0) {
				command="ALTER TABLE insplan "+string.Join(",",listAlters);
				Db.NonQ(command);
				listAlters.Clear();
			}
			//Operatory
			if(!LargeTableHelper.IndexExists("operatory","ProvDentist")) {
				listAlters.Add("ADD INDEX (ProvDentist)");
			}
			if(!LargeTableHelper.IndexExists("operatory","ProvHygienist")) {
				listAlters.Add("ADD INDEX (ProvHygienist)");
			}
			if(!LargeTableHelper.IndexExists("operatory","ClinicNum")) {
				listAlters.Add("ADD INDEX (ClinicNum)");
			}
			if(listAlters.Count>0) {
				command="ALTER TABLE operatory "+string.Join(",",listAlters);
				Db.NonQ(command);
				listAlters.Clear();
			}
			//Payment
			if(!LargeTableHelper.IndexExists("payment","ClinicNum")) {
				command="ALTER TABLE payment ADD INDEX (ClinicNum)";
				Db.NonQ(command);
			}
			//Payplan
			if(!LargeTableHelper.IndexExists("payplan","PatNum")) {
				listAlters.Add("ADD INDEX (PatNum)");
			}
			if(!LargeTableHelper.IndexExists("payplan","Guarantor")) {
				listAlters.Add("ADD INDEX (Guarantor)");
			}
			if(!LargeTableHelper.IndexExists("payplan","PlanNum")) {
				listAlters.Add("ADD INDEX (PlanNum)");
			}
			if(listAlters.Count>0) {
				command="ALTER TABLE payplan "+string.Join(",",listAlters);
				Db.NonQ(command);
				listAlters.Clear();
			}
			//Paysplit
			if(!LargeTableHelper.IndexExists("paysplit","ProvNum")) {
				command="ALTER TABLE paysplit ADD INDEX (ProvNum)";
				Db.NonQ(command);
			}
			//Procedurelog
			if(!LargeTableHelper.IndexExists("procedurelog","Priority")) {
				command="ALTER TABLE procedurelog ADD INDEX (Priority)";
				Db.NonQ(command);
			}
			//Proctp
			if(!LargeTableHelper.IndexExists("proctp","ProcNumOrig")) {
				command="ALTER TABLE proctp ADD INDEX (ProcNumOrig)";
				Db.NonQ(command);
			}
			//Provider
			if(!LargeTableHelper.IndexExists("provider","FeeSched")) {
				command="ALTER TABLE provider ADD INDEX (FeeSched)";
				Db.NonQ(command);
			}
			//Schedule
			if(!LargeTableHelper.IndexExists("schedule","BlockoutType")) {
				listAlters.Add("ADD INDEX (BlockoutType)");
			}
			if(!LargeTableHelper.IndexExists("schedule","EmployeeNum,SchedDate,SchedType,StopTime")) {
				listAlters.Add("ADD INDEX EmpDateTypeStopTime (EmployeeNum,SchedDate,SchedType,StopTime)");
				if(LargeTableHelper.IndexExists("schedule","EmployeeNum")) {
					listAlters.Add("DROP INDEX EmployeeNum");
				}
			}
			if(listAlters.Count>0) {
				command="ALTER TABLE schedule "+string.Join(",",listAlters);
				Db.NonQ(command);
				listAlters.Clear();
			}
			//Task
			if(!LargeTableHelper.IndexExists("task","UserNum")) {
				command="ALTER TABLE task ADD INDEX (UserNum)";
				Db.NonQ(command);
			}
			//Usergroup
			if(!LargeTableHelper.IndexExists("usergroup","UserGroupNumCEMT")) {
				command="ALTER TABLE usergroup ADD INDEX (UserGroupNumCEMT)";
				Db.NonQ(command);
			}
			//Usergroupattach
			if(!LargeTableHelper.IndexExists("usergroupattach","UserNum")) {
				command="ALTER TABLE usergroupattach ADD INDEX (UserNum)";
				Db.NonQ(command);
			}
			//Userod
			if(!LargeTableHelper.IndexExists("userod","UserGroupNum")) {
				listAlters.Add("ADD INDEX (UserGroupNum)");
			}
			if(!LargeTableHelper.IndexExists("userod","ClinicNum")) {
				listAlters.Add("ADD INDEX (ClinicNum)");
			}
			if(!LargeTableHelper.IndexExists("userod","ProvNum")) {
				listAlters.Add("ADD INDEX (ProvNum)");
			}
			if(listAlters.Count>0) {
				command="ALTER TABLE userod "+string.Join(",",listAlters);
				Db.NonQ(command);
				listAlters.Clear();
			}
			command="DROP TABLE IF EXISTS promotion";
			Db.NonQ(command);
			command=@"CREATE TABLE promotion (
					PromotionNum bigint NOT NULL auto_increment PRIMARY KEY,
					PromotionName varchar(255) NOT NULL,
					DateTimeCreated date NOT NULL DEFAULT '0001-01-01',
					ClinicNum bigint NOT NULL,
					TypePromotion tinyint NOT NULL,
					INDEX(ClinicNum)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			//This table was added in the 20_1_3 convert script. However, it needed drastic changes and it was unused.
			//Droping and readding.
			command="DROP TABLE IF EXISTS promotionlog";
			Db.NonQ(command);
			command=@"CREATE TABLE promotionlog (
					PromotionLogNum bigint NOT NULL auto_increment PRIMARY KEY,
					PromotionNum bigint NOT NULL,
					PatNum bigint NOT NULL,
					EmailMessageNum bigint NOT NULL,
					EmailHostingFK bigint NOT NULL,
					DateTimeSent datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					PromotionStatus tinyint NOT NULL,
					INDEX(PromotionNum),
					INDEX(PatNum),
					INDEX(EmailMessageNum),
					INDEX(EmailHostingFK)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('MassEmailGuid','')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('MassEmailSecret','')";
			Db.NonQ(command);
			//End of adding indexes
			//Remove "From API" PaymentType
			command="SELECT COALESCE(MIN(DefNum),0) FROM definition WHERE ItemName='From API' AND Category=10";//10 is PaymentTypes
			long defNum=PIn.Long(Db.GetScalar(command));
			if(defNum > 0) {
				//The definition still exists.
				//Update their "ApiPaymentType" pref to the first in the category
				command="SELECT DefNum FROM definition WHERE Category=10 AND IsHidden=0 ORDER BY ItemOrder LIMIT 1";
				long newDef=PIn.Long(Db.GetScalar(command));
				command=$"UPDATE preference SET ValueString='{POut.Long(newDef)}' WHERE PrefName='ApiPaymentType' AND ValueString='{POut.Long(defNum)}'";
				Db.NonQ(command);
				command=$"SELECT COUNT(*) FROM payment WHERE PayType={POut.Long(defNum)}";
				if(Db.GetScalar(command)=="0") {
					//There are no payments using this pay type, so delete "From API"
					command=$"DELETE FROM definition WHERE DefNum={POut.Long(defNum)}";
					Db.NonQ(command);
				}
			}
		}//End of 20_1_5() method

		private static void To20_1_17() {
			string command;
			command="ALTER TABLE emailhostingtemplate ADD EmailTemplateType varchar(255) NOT NULL";
			Db.NonQ(command);
		}//End of 20_1_17() method

		private static void To20_1_18() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DomainObjectGuid','')";
			Db.NonQ(command);
		}//End of 20_1_18() method

		private static void To20_1_21() {
			string command;
			string templateName,subject,bodyText,bodyHtml;
			Tuple<string,string,string,string> templateTuple=GetCovidOption1Template();
			templateName=templateTuple.Item1;
			subject=templateTuple.Item2;
			bodyText=templateTuple.Item3;
			bodyHtml=templateTuple.Item4;
			command="INSERT INTO emailhostingtemplate(TemplateName,Subject,BodyPlainText,BodyHTML,TemplateId,ClinicNum,EmailTemplateType) VALUES" +
				$"('{templateName}','{subject}','{bodyText}','{bodyHtml}',0,0,'Html')";
			Db.NonQ(command);
			templateTuple=GetCovidOption2Template();
			templateName=templateTuple.Item1;
			subject=templateTuple.Item2;
			bodyText=templateTuple.Item3;
			bodyHtml=templateTuple.Item4;
			command="INSERT INTO emailhostingtemplate(TemplateName,Subject,BodyPlainText,BodyHTML,TemplateId,ClinicNum,EmailTemplateType) VALUES" +
				$"('{templateName}','{subject}','{bodyText}','{bodyHtml}',0,0,'Html')";
			Db.NonQ(command);
			templateTuple=GetPromotionTemplate();
			templateName=templateTuple.Item1;
			subject=templateTuple.Item2;
			bodyText=templateTuple.Item3;
			bodyHtml=templateTuple.Item4;
			command="INSERT INTO emailhostingtemplate(TemplateName,Subject,BodyPlainText,BodyHTML,TemplateId,ClinicNum,EmailTemplateType) VALUES" +
				$"('{templateName}','{subject}','{bodyText}','{bodyHtml}',0,0,'Html')";
			Db.NonQ(command);
			templateTuple=GetPatientPortalTemplate();
			templateName=templateTuple.Item1;
			subject=templateTuple.Item2;
			bodyText=templateTuple.Item3;
			bodyHtml=templateTuple.Item4;
			command="INSERT INTO emailhostingtemplate(TemplateName,Subject,BodyPlainText,BodyHTML,TemplateId,ClinicNum,EmailTemplateType) VALUES" +
				$"('{templateName}','{subject}','{bodyText}','{bodyHtml}',0,0,'Html')";
			Db.NonQ(command);
			templateTuple=GetInsuranceTemplate();
			templateName=templateTuple.Item1;
			subject=templateTuple.Item2;
			bodyText=templateTuple.Item3;
			bodyHtml=templateTuple.Item4;
			command="INSERT INTO emailhostingtemplate(TemplateName,Subject,BodyPlainText,BodyHTML,TemplateId,ClinicNum,EmailTemplateType) VALUES" +
				$"('{templateName}','{subject}','{bodyText}','{bodyHtml}',0,0,'Html')";
			Db.NonQ(command);
			templateTuple=GetBirthdayTemplate();
			templateName=templateTuple.Item1;
			subject=templateTuple.Item2;
			bodyText=templateTuple.Item3;
			bodyHtml=templateTuple.Item4;
			command="INSERT INTO emailhostingtemplate(TemplateName,Subject,BodyPlainText,BodyHTML,TemplateId,ClinicNum,EmailTemplateType) VALUES" +
				$"('{templateName}','{subject}','{bodyText}','{bodyHtml}',0,0,'Html')";
			Db.NonQ(command);
			templateTuple=GetCovidOption3Template();
			templateName=templateTuple.Item1;
			subject=templateTuple.Item2;
			bodyText=templateTuple.Item3;
			bodyHtml=templateTuple.Item4;
			command="INSERT INTO emailhostingtemplate(TemplateName,Subject,BodyPlainText,BodyHTML,TemplateId,ClinicNum,EmailTemplateType) VALUES" +
				$"('{templateName}','{subject}','{bodyText}','{bodyHtml}',0,0,'RawHtml')";
			Db.NonQ(command);
		}//End of 20_1_21() method

		private static void To20_1_24() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DirectX11ToothChartUseIfAvail','1')";
			Db.NonQ(command);	
		}//End of 20_1_24() method

		private static void To20_1_27() {
			string command;
			command="SELECT COUNT(*) FROM preference WHERE preference.PrefName='ApptArrivalAutoEnabled';";
			if(PIn.Int(Db.GetCount(command))==0) {
				//Arrivals have not been added yet, add preferences and columns
				command=$"INSERT INTO preference (PrefName,ValueString) VALUES('ApptArrivalAutoEnabled','0')";//Off by default.
				Db.NonQ(command);	
				command="ALTER TABLE apptreminderrule ADD TemplateComeInMessage text NOT NULL";
				Db.NonQ(command);
				//FormDefEdit will automatically include the corresponding Def.DefNum for these preferences.
				command="SELECT preference.ValueString FROM preference WHERE preference.PrefName IN ('AppointmentTimeArrivedTrigger'," +
					"'AppointmentTimeSeatedTrigger','AppointmentTimeDismissedTrigger')";
				string arrivedTriggers=string.Join(",",Db.GetListLong(command,hasExceptions:false).Where(x => x!=0).Distinct().Select(x => POut.Long(x)));
				command=$"INSERT INTO preference(PrefName,ValueString) VALUES('ApptConfirmExcludeArrivalSend','{arrivedTriggers}')";
				Db.NonQ(command);	
				command=$"INSERT INTO preference(PrefName,ValueString) VALUES('ApptConfirmExcludeArrivalResponse','{arrivedTriggers}')";
				Db.NonQ(command);	
			}
		}

		private static void To20_1_43() {
			string command;
			DataTable table;
			//26 for alerttype PatientArrival
			int patientArrivalAlertType=26;
			//Add an alertcategory link from OdAllTypes and eServices to PatientArrival AlertType, if not already existing.
			foreach(string internalAlertCategory in new string [] { "OdAllTypes","eServices" }) {
				command=@"SELECT alertcategory.AlertCategoryNum,alertcategorylink.AlertType 
					FROM alertcategory 
					LEFT JOIN alertcategorylink ON alertcategorylink.AlertCategoryNum=alertcategory.AlertCategoryNum
					WHERE InternalName='"+POut.String(internalAlertCategory)+"' AND IsHQCategory=1";
				table=Db.GetTable(command);
				if(table.Rows.Count>0 && !table.Rows.AsEnumerable<DataRow>().Any(x => PIn.Int(x["AlertType"].ToString())==patientArrivalAlertType)) {
					//Alert category exists and there aren't already any links to the PatientArrival AlertType
					long alertCategoryNum=PIn.Long(table.Rows.AsEnumerable<DataRow>().First()["AlertCategoryNum"].ToString());
					command="INSERT INTO alertcategorylink(AlertCategoryNum,AlertType) VALUES("+POut.Long(alertCategoryNum)+","+POut.Int(patientArrivalAlertType)+")";
					Db.NonQ(command);
				}
			}
		}

		private static void To20_1_47() {
			string command="INSERT INTO preference (PrefName,ValueString) VALUES('EnableEmailAddressAutoComplete','1');";//Default to true
			Db.NonQ(command);
		}

		private static void To20_2_1() {
			string command;
			DataTable table;
			command="ALTER TABLE mountdef ADD ColorBack int NOT NULL";
			Db.NonQ(command);
			command="UPDATE mountdef SET ColorBack="+System.Drawing.Color.Black.ToArgb();
			Db.NonQ(command);
			command="ALTER TABLE mount ADD ColorBack int NOT NULL";
			Db.NonQ(command);
			command="UPDATE mount SET ColorBack="+System.Drawing.Color.Black.ToArgb();
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('TimecardUsersCantEditPastPayPeriods','0')";//false by default, preserves current behavior
			Db.NonQ(command);
			command="ALTER TABLE carecreditwebresponse ADD ClinicNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE carecreditwebresponse ADD INDEX (ClinicNum)";
			Db.NonQ(command);
			command="ALTER TABLE carecreditwebresponse ADD ServiceType varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE carecreditwebresponse ADD TransType varchar(255) NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PatientMaintainedOnUserChange','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PortalWebEmailTemplateType','1')";//Html
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedAsapEmailTemplateType','1')";//Html
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedRecallEmailTemplateType','1')";//Html
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedRecallEmailTemplateType2','1')";//Html
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedRecallEmailTemplateType3','1')";//Html
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedRecallEmailTemplateTypeAgg','1')";//Html
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedVerifyRecallEmailTemplateType','1')";//Html
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedVerifyNewPatEmailTemplateType','1')";//Html
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedVerifyAsapEmailTemplateType','1')";//Html
			Db.NonQ(command);
			command="ALTER TABLE apptreminderrule ADD EmailTemplateType varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE apptreminderrule ADD AggEmailTemplateType varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE asapcomm ADD EmailTemplateType varchar(255) NOT NULL";
			Db.NonQ(command);
		}//End of 20_2_1() method

		private static void To20_2_4() {
			string command;
			//check to see if the templates already exist before adding
			string templateName,subject,bodyText,bodyHtml;
			Tuple<string,string,string,string> templateTuple=GetCovidOption1Template();
			templateName=templateTuple.Item1;
			subject=templateTuple.Item2;
			bodyText=templateTuple.Item3;
			bodyHtml=templateTuple.Item4;
			command="SELECT COUNT(*) FROM emailhostingtemplate WHERE emailhostingtemplate.TemplateName='"+templateName+"'";
			if(PIn.Int(Db.GetCount(command))==0) {
				command="INSERT INTO emailhostingtemplate(TemplateName,Subject,BodyPlainText,BodyHTML,TemplateId,ClinicNum,EmailTemplateType) VALUES" +
				$"('{templateName}','{subject}','{bodyText}','{bodyHtml}',0,0,'Html')";
				Db.NonQ(command);
			}
			templateTuple=GetCovidOption2Template();
			templateName=templateTuple.Item1;
			subject=templateTuple.Item2;
			bodyText=templateTuple.Item3;
			bodyHtml=templateTuple.Item4;
			command="SELECT COUNT(*) FROM emailhostingtemplate WHERE emailhostingtemplate.TemplateName='"+templateName+"'";
			if(PIn.Int(Db.GetCount(command))==0) {
				command="INSERT INTO emailhostingtemplate(TemplateName,Subject,BodyPlainText,BodyHTML,TemplateId,ClinicNum,EmailTemplateType) VALUES" +
				$"('{templateName}','{subject}','{bodyText}','{bodyHtml}',0,0,'Html')";
				Db.NonQ(command);
			}
			templateTuple=GetPromotionTemplate();
			templateName=templateTuple.Item1;
			subject=templateTuple.Item2;
			bodyText=templateTuple.Item3;
			bodyHtml=templateTuple.Item4;
			command="SELECT COUNT(*) FROM emailhostingtemplate WHERE emailhostingtemplate.TemplateName='"+templateName+"'";
			if(PIn.Int(Db.GetCount(command))==0) {
				command="INSERT INTO emailhostingtemplate(TemplateName,Subject,BodyPlainText,BodyHTML,TemplateId,ClinicNum,EmailTemplateType) VALUES" +
				$"('{templateName}','{subject}','{bodyText}','{bodyHtml}',0,0,'Html')";
				Db.NonQ(command);
			}
			templateTuple=GetPatientPortalTemplate();
			templateName=templateTuple.Item1;
			subject=templateTuple.Item2;
			bodyText=templateTuple.Item3;
			bodyHtml=templateTuple.Item4;
			command="SELECT COUNT(*) FROM emailhostingtemplate WHERE emailhostingtemplate.TemplateName='"+templateName+"'";
			if(PIn.Int(Db.GetCount(command))==0) {
				command="INSERT INTO emailhostingtemplate(TemplateName,Subject,BodyPlainText,BodyHTML,TemplateId,ClinicNum,EmailTemplateType) VALUES" +
				$"('{templateName}','{subject}','{bodyText}','{bodyHtml}',0,0,'Html')";
				Db.NonQ(command);
			}
			templateTuple=GetInsuranceTemplate();
			templateName=templateTuple.Item1;
			subject=templateTuple.Item2;
			bodyText=templateTuple.Item3;
			bodyHtml=templateTuple.Item4;
			command="SELECT COUNT(*) FROM emailhostingtemplate WHERE emailhostingtemplate.TemplateName='"+templateName+"'";
			if(PIn.Int(Db.GetCount(command))==0) {
				command="INSERT INTO emailhostingtemplate(TemplateName,Subject,BodyPlainText,BodyHTML,TemplateId,ClinicNum,EmailTemplateType) VALUES" +
				$"('{templateName}','{subject}','{bodyText}','{bodyHtml}',0,0,'Html')";
				Db.NonQ(command);
			}
			templateTuple=GetBirthdayTemplate();
			templateName=templateTuple.Item1;
			subject=templateTuple.Item2;
			bodyText=templateTuple.Item3;
			bodyHtml=templateTuple.Item4;
			command="SELECT COUNT(*) FROM emailhostingtemplate WHERE emailhostingtemplate.TemplateName='"+templateName+"'";
			if(PIn.Int(Db.GetCount(command))==0) {
				command="INSERT INTO emailhostingtemplate(TemplateName,Subject,BodyPlainText,BodyHTML,TemplateId,ClinicNum,EmailTemplateType) VALUES" +
				$"('{templateName}','{subject}','{bodyText}','{bodyHtml}',0,0,'Html')";//EmailTemplateType comes from EmailType enum (as string)
				Db.NonQ(command);
			}
			templateTuple=GetCovidOption3Template();
			templateName=templateTuple.Item1;
			subject=templateTuple.Item2;
			bodyText=templateTuple.Item3;
			bodyHtml=templateTuple.Item4;
			command="SELECT COUNT(*) FROM emailhostingtemplate WHERE emailhostingtemplate.TemplateName='"+templateName+"'";
			if(PIn.Int(Db.GetCount(command))==0) {
				command="INSERT INTO emailhostingtemplate(TemplateName,Subject,BodyPlainText,BodyHTML,TemplateId,ClinicNum,EmailTemplateType) VALUES" +
				$"('{templateName}','{subject}','{bodyText}','{bodyHtml}',0,0,'RawHtml')";//EmailTemplateType comes from EmailType enum (as string)
				Db.NonQ(command);
			}
		}//End of 20_2_4() method

		private static void To20_2_8() {
			string command;
			command="UPDATE apptreminderrule SET EmailTemplateType='Html' WHERE EmailTemplateType=''";
			Db.NonQ(command);
			command="UPDATE apptreminderrule SET AggEmailTemplateType='Html' WHERE AggEmailTemplateType=''";
			Db.NonQ(command);
		}//End of 20_2_8() method

		private static void To20_2_10() {
			string command;
			command="SELECT COUNT(*) FROM preference WHERE PrefName='DirectX11ToothChartUseIfAvail'";
			//This preference might have already been added in 20.1.24.
			if(Db.GetScalar(command)=="0") {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('DirectX11ToothChartUseIfAvail','1');";
				Db.NonQ(command);
			}
			command="ALTER TABLE creditcard ADD PaymentType bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE creditcard ADD INDEX (PaymentType)";
			Db.NonQ(command);
		}//End of 20_2_10() method

		private static void To20_2_11() {
			string command;
			command="ALTER TABLE computerpref ADD GraphicsUseDirectX11 tinyint NOT NULL DEFAULT 0";
			Db.NonQ(command);
		}//End of 20_2_11() method

		private static void To20_2_14() {
			string command;
			command="SELECT COUNT(*) FROM preference WHERE preference.PrefName='ApptArrivalAutoEnabled';";
			if(PIn.Int(Db.GetCount(command))==0) {
				//Arrivals have not been added yet, add preferences and columns
				command=$"INSERT INTO preference (PrefName,ValueString) VALUES('ApptArrivalAutoEnabled','0')";//Off by default.
				Db.NonQ(command);	
				command="ALTER TABLE apptreminderrule ADD TemplateComeInMessage text NOT NULL";
				Db.NonQ(command);
				//FormDefEdit will automatically include the corresponding Def.DefNum for these preferences.
				command="SELECT preference.ValueString FROM preference WHERE preference.PrefName IN ('AppointmentTimeArrivedTrigger'," +
					"'AppointmentTimeSeatedTrigger','AppointmentTimeDismissedTrigger')";
				string arrivedTriggers=string.Join(",",Db.GetListLong(command,hasExceptions:false).Where(x => x!=0).Distinct().Select(x => POut.Long(x)));
				command=$"INSERT INTO preference(PrefName,ValueString) VALUES('ApptConfirmExcludeArrivalSend','{arrivedTriggers}')";
				Db.NonQ(command);	
				command=$"INSERT INTO preference(PrefName,ValueString) VALUES('ApptConfirmExcludeArrivalResponse','{arrivedTriggers}')";
				Db.NonQ(command);	
			}
		}//End of 20_2_12() method

		private static void To20_2_17() {
			string command;
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('CloudPasswordNeedsReset','0')";//0 = YN.Unknown
			Db.NonQ(command);
		}

		private static void To20_2_29() {
			string command;
			DataTable table;
			//26 for alerttype PatientArrival
			int patientArrivalAlertType=26;
			//Add an alertcategory link from OdAllTypes and eServices to PatientArrival AlertType, if not already existing.
			foreach(string internalAlertCategory in new string [] { "OdAllTypes","eServices" }) {
				command=@"SELECT alertcategory.AlertCategoryNum,alertcategorylink.AlertType 
					FROM alertcategory 
					LEFT JOIN alertcategorylink ON alertcategorylink.AlertCategoryNum=alertcategory.AlertCategoryNum
					WHERE InternalName='"+POut.String(internalAlertCategory)+"' AND IsHQCategory=1";
				table=Db.GetTable(command);
				if(table.Rows.Count>0 && !table.Rows.AsEnumerable<DataRow>().Any(x => PIn.Int(x["AlertType"].ToString())==patientArrivalAlertType)) {
					//Alert category exists and there aren't already any links to the PatientArrival AlertType
					long alertCategoryNum=PIn.Long(table.Rows.AsEnumerable<DataRow>().First()["AlertCategoryNum"].ToString());
					command="INSERT INTO alertcategorylink(AlertCategoryNum,AlertType) VALUES("+POut.Long(alertCategoryNum)+","+POut.Int(patientArrivalAlertType)+")";
					Db.NonQ(command);
				}
			}
			//WebServiceMainHQProxy.GetEServiceSetupFull() was incorrectly upserting clinicprefs for the 0 clinic, even though a preference entry already exists.
			command="SELECT ValueString FROM clinicpref WHERE ClinicNum=0 AND PrefName='MassEmailStatus'";
			List<string> listStringPrefVals=Db.GetListString(command);
			if(listStringPrefVals.Count>0) {				
				command=$"UPDATE preference SET ValueString='{POut.String(listStringPrefVals[0])}' WHERE PrefName='MassEmailStatus'";
				Db.NonQ(command);
				command="DELETE FROM clinicpref WHERE ClinicNum=0 AND PrefName='MassEmailStatus'";
				Db.NonQ(command);
			}
			command="SELECT ValueString FROM clinicpref WHERE ClinicNum=0 AND PrefName='MassEmailGuid'";
			listStringPrefVals=Db.GetListString(command);			
			if(listStringPrefVals.Count>0) {
				command=$"UPDATE preference SET ValueString='{POut.String(listStringPrefVals[0])}' WHERE PrefName='MassEmailGuid'";
				Db.NonQ(command);
				command="DELETE FROM clinicpref WHERE ClinicNum=0 AND PrefName='MassEmailGuid'";
				Db.NonQ(command);
			}
			command="SELECT ValueString FROM clinicpref WHERE ClinicNum=0 AND PrefName='MassEmailSecret'";
			listStringPrefVals=Db.GetListString(command);			
			if(listStringPrefVals.Count>0) {
				command=$"UPDATE preference SET ValueString='{POut.String(listStringPrefVals[0])}' WHERE PrefName='MassEmailSecret'";
				Db.NonQ(command);
				command="DELETE FROM clinicpref WHERE ClinicNum=0 AND PrefName='MassEmailSecret'";
				Db.NonQ(command);
			}
		}

		private static void To20_2_34() {
			string command;
			command="INSERT INTO preference (PrefName,ValueString) VALUES('ClaimCobInsPaidBehavior','1');";//Default to "ClaimLevel"
			Db.NonQ(command);
			command="SELECT COUNT(*) FROM preference WHERE PrefName='EnableEmailAddressAutoComplete'";
			if(!PIn.Bool(Db.GetCount(command))) {
				//Not found.
				command="INSERT INTO preference (PrefName,ValueString) VALUES('EnableEmailAddressAutoComplete','1');";//Default to true
				Db.NonQ(command);
			}
		}

		private static void To20_2_38() {
			string command;
			command="ALTER TABLE emailhostingtemplate MODIFY COLUMN BodyPlainText MEDIUMTEXT NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE emailhostingtemplate MODIFY COLUMN BodyHTML MEDIUMTEXT NOT NULL";
			Db.NonQ(command);
		}

		private static void To20_2_49() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseExactMatchPhone','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseExactMatchPhoneNumDigits','10')";
			Db.NonQ(command);
		}

		private static void To20_3_1() {
			string command;
			DataTable table;
			command="SELECT MAX(ItemOrder)+1 FROM patfielddef";
			int maxOrder=Db.GetInt(command);
			command=$"INSERT INTO patfielddef (FieldName,FieldType,ItemOrder) VALUES ('{POut.String("CareCredit Approval Status")}',6,{POut.Int(maxOrder)})";//CareCreditStatus
			long patDefNum=Db.NonQ(command,true);
			command="SELECT ProgramNum FROM program WHERE ProgName='CareCredit'";
			long programNum=Db.GetLong(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue) "
				 +"VALUES("
				 +POut.Long(programNum)+", "
				 +"'CareCreditPatField', "
				 +"'"+POut.Long(patDefNum)+"')";
			Db.NonQ(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue) "
				 +"VALUES("
				 +POut.Long(programNum)+", "
				 +"'CareCreditIsMerchantNumberByProv', "
				 +"'0')";//disabled by default
			Db.NonQ(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue) "
				 +"VALUES("
				 +POut.Long(programNum)+", "
				 +"'CareCreditMerchantNumber', "
				 +"'')";
			Db.NonQ(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue) "
				 +"VALUES("
				 +POut.Long(programNum)+", "
				 +"'Disable Advertising', "
				 +"'0')";//Default to display advertising
			Db.NonQ(command);
			command="ALTER TABLE providerclinic ADD CareCreditMerchantId varchar(20) NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO definition (Category,ItemName,ItemOrder) VALUES (51,'Emergency/Tooth Pain',0),(51,'Cosmetic Whitening',1)";
			Db.NonQ(command);
			command="ALTER TABLE carecreditwebresponse ADD MerchantNumber varchar(20) NOT NULL";
			Db.NonQ(command);
			//Insert tables for Blue Book feature.
			command="DROP TABLE IF EXISTS insbluebook";
			Db.NonQ(command);
			//Type of GroupNum field for this table is 'varchar(25)' to match the type of insplan.GroupNum.
			command=@"CREATE TABLE insbluebook (
				InsBlueBookNum bigint NOT NULL auto_increment PRIMARY KEY,
				ProcCodeNum bigint NOT NULL,
				CarrierNum bigint NOT NULL,
				PlanNum bigint NOT NULL,
				GroupNum varchar(25) NOT NULL,
				InsPayAmt double NOT NULL,
				AllowedOverride double NOT NULL,
				DateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				INDEX(ProcCodeNum),
				INDEX(CarrierNum),
				INDEX(PlanNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS insbluebooklog";
			Db.NonQ(command);
			//Type of Description field for this table is 'text' to match the type of securitylog.LogText per Allen.
			command=@"CREATE TABLE insbluebooklog (
				InsBlueBookLogNum bigint NOT NULL auto_increment PRIMARY KEY,
				ClaimProcNum bigint NOT NULL,
				AllowedFee double NOT NULL,
				DateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				Description text NOT NULL,
				INDEX(ClaimProcNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS insbluebookrule";
			Db.NonQ(command);
			command=@"CREATE TABLE insbluebookrule (
				InsBlueBookRuleNum bigint NOT NULL auto_increment PRIMARY KEY,
				ItemOrder smallint NOT NULL,
				RuleType tinyint NOT NULL,
				LimitValue int NOT NULL,
				LimitType tinyint NOT NULL
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PriClaimAllowSetToHoldUntilPriReceived','1')";//defaults to true
			Db.NonQ(command);
			command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor) VALUES (21,11,'Main Border',-12493158)";//dark gray blue
			Db.NonQ(command);
			command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor) VALUES (21,12,'Main Border Outline',-16777216)";//black
			Db.NonQ(command);
			command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor) VALUES (21,13,'Main Border Text',-1)";//white
			Db.NonQ(command);
			//check databasemaintenance for ProviderHiddenOnAppointmentView, insert if not there and set IsOld to True or update to set IsOld to true
			command="INSERT INTO databasemaintenance (MethodName,IsHidden,IsOld) VALUES ('ProviderHiddenOnAppointmentView',0,0)";
			Db.NonQ(command);
			command="SELECT ProgramNum FROM program WHERE ProgName='CareCredit'";
			programNum=Db.GetLong(command);
			string butText=" CareCredit";//The space in the beginning is intentional. CareCredit required us to add more space between the icon image and text. 
			command=$"INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) VALUES ({POut.Long(programNum)},0,'{POut.String(butText)}')";//Add to Account Module
			Db.NonQ(command);
			command=$"INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) VALUES ({POut.Long(programNum)},5,'{POut.String(butText)}')";//Add to Treatment Plan Module
			Db.NonQ(command);
			command="UPDATE program SET ButtonImage='iVBORw0KGgoAAAANSUhEUgAAABYAAAAWCAYAAADEtGw7AAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAuIwAALiMBeKU/dgAAAv5JREFUSEu10UlME1EYwPFPUYTOtNAW1IgminLRyKIkxgVDjAUPxmjQsopAC2mRpaxSolFZJCqopZCwY5ECXaYUIlRASFpogydFEw968ODB5UCMiXrw8Pw6BUqkMVrh8G8776W/fO8NEELWJI+Lq5HHxdWI/fCXpYJvWxNkOSRwe+YQVFpP/qko7BW23fncMBMNieNZ4KPXA5/pAgHT6TVswwimrrCKQGWPgjNP5AA6BlHv4WTMiRKlVfSy2nYMqmzHYcdgE/ga+lnUG5iH/cRYGHvd7NgP8RYFTmsGIdPhNTyFOSdl4SZHeI10MpG9W9rYk4HwHm/gAoxFr1pPELUjYk42mQB+Bu3mjYYBSxDTThBs/Fd4nxOtsMaRKlsMouGm5IkMnFQXu0Gvmw82sagzjcCI1zHY/VewD075yQk32A8S3MuOHS3HOzUpNxn6yMKkrowdN4SPe0E43LMSliyDlWyiF7emDxOVI3Isd+rc1p1mFR8Ghma4Ro0bNXYQgamLBE0Ywvj6NuBW5q6EpY4sFkbQp3b6yLNGe+Q3vIJTMSNKDh67APQMwZfExqIICi39RDiuTwtsvwt0QTpQkgQ3zJGnwPqWFvYqNLO7/R7YD+SUW+PviCxFYQFMtxin/Mw19pAgE4Lmh0Q4onVib/D7Cl/bDLyaUqCk54MpWZKCLsqsX4IpeTJQnWrYNdEJZ8flgvix4lIEVMCMEM6wkQRZ+ohwTPcBpzPgy5HyH6lDAxqrgHe9cB2Vm3IaUTNdeOk7ooRWZCC5CMuSQNBaB/5PLVvAPJoJpqH7HEarFepby/i9TXGBHfV7EdrGqy0LoStk0VRemgIxK5UjJnh8sgguZF2CcVNMl0jqeNUlGu7N4nvca/kSrlIupkul6bhXjifqpLIvPMcI/nZhbuj3ItxwUeZROi+tFf/4zjkFnsAFXE4ldP5Fgsf0BPzeDywJY00XjA/LCsUSsVrMgM1ib7GP2BfsKzaPvcfmMDNWioVgrLEEr0UeF1cjj4v/H4FfZIn5BqItMj4AAAAASUVORK5CYII=' "
				+$"WHERE ProgramNum={POut.Long(programNum)}";
			Db.NonQ(command);
			//Insert EdgeExpress bridge----------------------------------------------------------------- 
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				 +") VALUES("
				 +"'EdgeExpress', "
				 +"'EdgeExpress from www.globalpaymentsintegrated.com', "
				 +"'0', "
				 +"'', "
				 +"'', "
				 +"'')";
			programNum=Db.NonQ(command,true);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				 +") VALUES("
				 +"'"+POut.Long(programNum)+"', "
				 +"'EdgeExpressPaymentType', "
				 +"'0')";
			Db.NonQ(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				 +") VALUES("
				 +"'"+POut.Long(programNum)+"', "
				 +"'EdgeExpressXWebID', "
				 +"'')";
			Db.NonQ(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				 +") VALUES("
				 +"'"+POut.Long(programNum)+"', "
				 +"'EdgeExpressXWebAuthKey', "
				 +"'')";
			Db.NonQ(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				 +") VALUES("
				 +"'"+POut.Long(programNum)+"', "
				 +"'EdgeExpressXWebTerminalID', "
				 +"'')";
			Db.NonQ(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				 +") VALUES("
				 +"'"+POut.Long(programNum)+"', "
				 +"'EdgeExpressForceRecurringCharge', "
				 +"'0')";
			Db.NonQ(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				 +") VALUES("
				 +"'"+POut.Long(programNum)+"', "
				 +"'EdgeExpressPreventSavingNewCC', "
				 +"'0')";
			Db.NonQ(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				 +") VALUES("
				 +"'"+POut.Long(programNum)+"', "
				 +"'EdgeExpressPromptSignature', "
				 +"'1')";
			Db.NonQ(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				 +") VALUES("
				 +"'"+POut.Long(programNum)+"', "
				 +"'EdgeExpressPrintReceipt', "
				 +"'1')";
			Db.NonQ(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				 +") VALUES("
				 +"'"+POut.Long(programNum)+"', "
				 +"'EdgeExpressIsOnlinePaymentsEnabled', "
				 +"'0')";
			Db.NonQ(command);
			//end EdgeExpress bridge
			command="ALTER TABLE rxpat ADD UserNum bigint NOT NULL,ADD INDEX (UserNum),ADD RxType tinyint NOT NULL,ADD INDEX PatNumRxType (PatNum,RxType),DROP INDEX PatNum";
			Db.NonQ(command);
			command="ALTER TABLE insbluebook ADD ProcNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE insbluebook ADD INDEX (ProcNum)";
			Db.NonQ(command);
			command="ALTER TABLE insbluebook ADD ProcDate date NOT NULL DEFAULT '0001-01-01'";
			Db.NonQ(command);
			//claim.ClaimType is a varchar(255), but it should be much smaller because the largest ClaimType is 'PreAuth'.
			command="ALTER TABLE insbluebook ADD ClaimType varchar(10) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE insbluebook ADD ClaimNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE insbluebook ADD INDEX (ClaimNum)";
			Db.NonQ(command);
			//Script populates insbluebook table with last 12 months of data for payments made on primary claims for out-of-network dental insurance plans.
			//Only includes data for claimprocs of status received or supplemental. Only includes data for insplans of type category percentage that have
			//an out-of-network fee schedule. Excludes procedures that have received insurance payments that sum to more than they are worth. Excludes
			//procedures that have a net negative sum of insurance payments. The claimproc table is joined a second time so that we can select the
			//AllowedOverride and ProcDate from the claimproc of status 'Received' for each procedure on each claim.
			command=@"
				INSERT INTO insbluebook (
					ProcCodeNum,CarrierNum,PlanNum,GroupNum,InsPayAmt,
					AllowedOverride,DateTEntry,ProcNum,ProcDate,ClaimType,ClaimNum
				)
				SELECT
					_result.CodeNum,_result.CarrierNum,_result.PlanNum,_result.GroupNum,_result.InsPayAmtSum,
					CASE WHEN _result.AllowedOverride=-1 THEN -1
						ELSE ROUND(_result.AllowedOverride/(_result.UnitQty+_result.BaseUnits),2) END,
					NOW(),_result.ProcNum,_result.ProcDate,_result.ClaimType,_result.ClaimNum
				FROM (
					SELECT 
						procedurelog.CodeNum,insplan.CarrierNum,insplan.PlanNum,insplan.GroupNum,SUM(claimproc1.InsPayAmt) 'InsPayAmtSum',
						MAX(claimproc2.AllowedOverride) 'AllowedOverride',procedurelog.ProcNum,MAX(claimproc2.ProcDate) 'ProcDate',
						claim.ClaimType,claim.ClaimNum,procedurelog.ProcFee,procedurelog.UnitQty,procedurelog.BaseUnits
					FROM insplan
					INNER JOIN claim
						ON insplan.PlanNum=claim.PlanNum
							AND claim.ClaimType='P'
					INNER JOIN claimproc claimproc1
						ON claim.ClaimNum=claimproc1.ClaimNum
							AND claimproc1.ProcNum!=0
							AND claimproc1.Status IN (1,4)
							AND claimproc1.ProcDate > DATE_SUB(NOW(),INTERVAL 12 MONTH)
					INNER JOIN procedurelog
						ON claimproc1.ProcNum=procedurelog.ProcNum
					LEFT JOIN claimproc claimproc2
						ON claimproc1.ClaimProcNum=claimproc2.ClaimProcNum
							AND claimproc1.Status=1
					WHERE insplan.PlanType=''
						AND insplan.AllowedFeeSched!=0
					GROUP BY claim.ClaimNum,procedurelog.ProcNum
				) _result
				WHERE _result.ProcFee*(_result.UnitQty+_result.BaseUnits) >= _result.InsPayAmtSum
					AND _result.InsPayAmtSum >= 0";
			Db.NonQ(command);
			//This was added back in v20.1.3. We defaulted it to disabled but after talking to CareCredit, they wanted it to be enabled by default.
			//Nathan approved this being enabled by default.
			//Since we know CareCredit was not available in 20.1.3(Only available in 20.3.1), we can simply update the QSBatchEnabled program property value to 1(enabled)
			command="SELECT ProgramNum FROM program WHERE ProgName='CareCredit'";
			programNum=Db.GetLong(command);
			command=$"UPDATE programproperty SET PropertyValue='1' WHERE ProgramNum={POut.Long(programNum)} AND PropertyDesc='QSBatchEnabled'";
			Db.NonQ(command);
			//Any patient configured to opt out of eReminders/eConfirmations/ThankYous for TEXT and/or EMAIL will now be opted out for WebSchedRecall,
			//WebSchedASAP,PatientPortalInvites,WebSchedNewPat,Verify,VerifyWSNP,Statements,Arrivals.  As we have now added so many entries to the 
			//CommOptOutType enum, it becomes unmanagable to continue to insert a unique row per patient per CommOptOutMode per CommOptOutType.  We will
			//convert to a one-to-one relationship between Patient and CommOptOut by changing CommOptOut to have a column for OptOutSms and OptOutEmail,
			//each being populated by a Flags enum representing the various species of automated messaging to opt out from.  At this point in time, opted
			//out for email or text means opted out for all species of automated messaging, so, for ease of future convert scripts, we will indicate this
			//with an All bit in the CommOptOutType Flags enum.
			command=$"SELECT PatNum,CommMode FROM commoptout GROUP BY PatNum,CommMode;";
			var listCommOptOuts=Db.GetTable(command).Rows.AsEnumerable<DataRow>()
				//The query result has one row per Patient per CommOptOutMode
				.Select(x =>
					new {
						PatNum=PIn.Long(x["PatNum"].ToString()),
						OptOutSms=PIn.Long(x["CommMode"].ToString())==0 ? 1 : 0,//CommOptOutMode.Text=0; CommOptOutType.All=1
						OptOutEmail=PIn.Long(x["CommMode"].ToString())==1 ? 1 : 0,//CommOptOutMode.Email=1; CommOptOutType.All=1
					}
				)
				//Group down into one row per Patient
				.GroupBy(x => x.PatNum)
				.Select(x => new {
					PatNum=x.Key,//PatNum
					OptOutSms=x.ToList().Any(x => x.OptOutSms==1) ? 1 : 0,//CommOptOutType.All=1
					OptOutEmail=x.ToList().Any(x => x.OptOutEmail==1) ? 1 : 0,// CommOptOutType.All=1
				}
			).ToList();
			command="DROP TABLE IF EXISTS commoptout";
			Db.NonQ(command);
			command=@"CREATE TABLE commoptout (
				CommOptOutNum bigint NOT NULL auto_increment PRIMARY KEY,
				PatNum bigint NOT NULL,
				OptOutSms int NOT NULL,
				OptOutEmail int NOT NULL,
				INDEX(PatNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			if(listCommOptOuts.Count>0) {
				command="INSERT INTO commoptout (PatNum,OptOutSms,OptOutEmail) VALUES "
					+string.Join(",",listCommOptOuts.Select(x => $"({POut.Long(x.PatNum)},{POut.Long(x.OptOutSms)},{POut.Long(x.OptOutEmail)})"));
				Db.NonQ(command);
			}
			//InsBlueBooUsePlanNumOverride is Bool. Defaults to true.
			command="INSERT INTO preference(PrefName,ValueString) VALUES ('InsBlueBookUsePlanNumOverride','1')";
			Db.NonQ(command);
			//InsBlueBookAllowedFeeMethod is Enum. Defaults to 2=MostRecent.
			command="INSERT INTO preference(PrefName,ValueString) VALUES ('InsBlueBookAllowedFeeMethod','2')";
			Db.NonQ(command);
			//InsBlueBookAnonShareEnable is Bool. Defaults to false.
			command="INSERT INTO preference(PrefName,ValueString) VALUES ('InsBlueBookAnonShareEnable','0')";
			Db.NonQ(command);
			//Fill insbluebookrule table with defaults for all six rules.
			command="INSERT INTO insbluebookrule(ItemOrder,RuleType,LimitValue,LimitType) VALUES (0,4,0,0)";//(0,ManualBlueBookSchedule,0,None)
			Db.NonQ(command);
			command="INSERT INTO insbluebookrule(ItemOrder,RuleType,LimitValue,LimitType) VALUES (1,0,1,1)";//(1,InsurancePlan,1,Years)
			Db.NonQ(command);
			command="INSERT INTO insbluebookrule(ItemOrder,RuleType,LimitValue,LimitType) VALUES (2,1,1,1)";//(2,GroupNumber,1,Years)
			Db.NonQ(command);
			command="INSERT INTO insbluebookrule(ItemOrder,RuleType,LimitValue,LimitType) VALUES (3,2,1,1)";//(3,InsuranceCarrier,1,Years)
			Db.NonQ(command);
			command="INSERT INTO insbluebookrule(ItemOrder,RuleType,LimitValue,LimitType) VALUES (4,3,1,1)";//(4,InsuranceCarrierGroup,1,Years)
			Db.NonQ(command);
			command="INSERT INTO insbluebookrule(ItemOrder,RuleType,LimitValue,LimitType) VALUES (5,5,0,0)";//(5,UcrFee,0,None)
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardEnableByodSms','0')";//default to false
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardAppendByodToArrivalResponseSms','0')";//default to false.
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardByodSmsTemplate','Check-In [NameF] at [ApptTime]: [eClipboardLink]')";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS eserviceshortguid";
			Db.NonQ(command);
			command=@"CREATE TABLE eserviceshortguid (
				EServiceShortGuidNum bigint NOT NULL auto_increment PRIMARY KEY,
				EServiceCode varchar(255) NOT NULL,
				ShortGuid varchar(255) NOT NULL,
				ShortURL varchar(255) NOT NULL,
				FKey bigint NOT NULL,
				FKeyType varchar(255) NOT NULL,
				DateTimeExpiration datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				DateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				INDEX(FKey),
				INDEX(ShortGUID)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="ALTER TABLE insplan ADD ManualFeeSchedNum bigint NOT NULL DEFAULT 0";
			Db.NonQ(command);
			command="ALTER TABLE insplan ADD INDEX (ManualFeeSchedNum)";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimEditShowPatResponsibility','0')";//YN_DEFAULT_TRUE, insert as '0 - unknown'
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimEditShowPayTracking','0')";//YN_DEFAULT_TRUE, insert as '0 - unknown'
			Db.NonQ(command);
			//Set preference for all clinics to use default
			command="SELECT ClinicNum FROM clinic";
			List<long> listClinicNums=Db.GetListLong(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('BirthdayPromotionsUseDefaults','1')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('BirthdayPromotionsEnabled','0')";
			Db.NonQ(command);
			//Set preference for all clinics to use default and be disabled
			foreach(long clinicNum in listClinicNums) {
				command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES ("+POut.Long(clinicNum)+",'BirthdayPromotionsUseDefaults','1')";
				Db.NonQ(command);
				command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES ("+POut.Long(clinicNum)+", 'BirthdayPromotionsEnabled','0')";
				Db.NonQ(command);
			}
			command="ALTER TABLE apptreminderrule ADD IsSendForMinorsBirthday tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE apptreminderrule ADD EmailHostingTemplateNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE apptreminderrule ADD INDEX (EmailHostingTemplateNum)";
			Db.NonQ(command);
			command="ALTER TABLE apptreminderrule ADD MinorAge int NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE emailhostingtemplate ADD TemplateType varchar(255) NOT NULL";
			Db.NonQ(command);
			//Default Birthday Appointment Reminder Rule and cooresponding Hosting Remplate
			command="INSERT INTO emailhostingtemplate (TemplateName,Subject,BodyPlainText,BodyHTML,TemplateId,ClinicNum,EmailTemplateType,TemplateType)" +
				"VALUES ('Automated Birthday','Happy Birthday','Wishing you a happy and healthy Birthday! Hope your day is full of smiles and memorable moments. From your friends at [{[{ OfficeName }]}]','',0,0,'Regular','Birthday')";
			long num=Db.NonQ(command,true);
			command="SELECT MAX(EmailHostingTemplateNum) FROM emailhostingtemplate WHERE TemplateType='Birthday'";
			long templateNum=Db.GetLong(command);
			command="INSERT INTO apptreminderrule (TypeCur,ClinicNum,EmailHostingTemplateNum,MinorAge) VALUES (6,0,"+templateNum+",14)";
			Db.NonQ(command);
		}//End of 20_3_1() method

		private static void To20_3_2() {
			string command;
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('InsBlueBookUcrFeePercent','80')";
			Db.NonQ(command);
			//procedurelog.DiscountPlanAmt
			LargeTableHelper.AlterLargeTable("procedurelog","ProcNum",
					new List<Tuple<string,string>> { Tuple.Create("DiscountPlanAmt","double NOT NULL") });
		}//End of 20_3_2() method

		private static void To20_3_3() {
			string command;
			DataTable table;
			command="SELECT MAX(EmailHostingTemplateNum) FROM emailhostingtemplate WHERE TemplateType='Birthday'";
			long templateNum=Db.GetLong(command);
			command=$"UPDATE emailhostingtemplate SET EmailTemplateType='Html',BodyHtml='{GetBirthdayTemplate().Item4}' WHERE EmailHostingTemplateNum={templateNum}";
			Db.NonQ(command);
			//26 for alerttype PatientArrival
			int patientArrivalAlertType=26;
			//Add an alertcategory link from OdAllTypes and eServices to PatientArrival AlertType, if not already existing.
			foreach(string internalAlertCategory in new string [] { "OdAllTypes","eServices" }) {
				command=@"SELECT alertcategory.AlertCategoryNum,alertcategorylink.AlertType 
					FROM alertcategory 
					LEFT JOIN alertcategorylink ON alertcategorylink.AlertCategoryNum=alertcategory.AlertCategoryNum
					WHERE InternalName='"+POut.String(internalAlertCategory)+"' AND IsHQCategory=1";
				table=Db.GetTable(command);
				if(table.Rows.Count>0 && !table.Rows.AsEnumerable<DataRow>().Any(x => PIn.Int(x["AlertType"].ToString())==patientArrivalAlertType)) {
					//Alert category exists and there aren't already any links to the PatientArrival AlertType
					long alertCategoryNum=PIn.Long(table.Rows.AsEnumerable<DataRow>().First()["AlertCategoryNum"].ToString());
					command="INSERT INTO alertcategorylink(AlertCategoryNum,AlertType) VALUES("+POut.Long(alertCategoryNum)+","+POut.Int(patientArrivalAlertType)+")";
					Db.NonQ(command);
				}
			}
			//WebServiceMainHQProxy.GetEServiceSetupFull() was incorrectly upserting clinicprefs for the 0 clinic, even though a preference entry already exists.
			command="SELECT ValueString FROM clinicpref WHERE ClinicNum=0 AND PrefName='MassEmailStatus'";
			List<string> listStringPrefVals=Db.GetListString(command);
			if(listStringPrefVals.Count>0) {				
				command=$"UPDATE preference SET ValueString='{POut.String(listStringPrefVals[0])}' WHERE PrefName='MassEmailStatus'";
				Db.NonQ(command);
				command="DELETE FROM clinicpref WHERE ClinicNum=0 AND PrefName='MassEmailStatus'";
				Db.NonQ(command);
			}
			command="SELECT ValueString FROM clinicpref WHERE ClinicNum=0 AND PrefName='MassEmailGuid'";
			listStringPrefVals=Db.GetListString(command);			
			if(listStringPrefVals.Count>0) {
				command=$"UPDATE preference SET ValueString='{POut.String(listStringPrefVals[0])}' WHERE PrefName='MassEmailGuid'";
				Db.NonQ(command);
				command="DELETE FROM clinicpref WHERE ClinicNum=0 AND PrefName='MassEmailGuid'";
				Db.NonQ(command);
			}
			command="SELECT ValueString FROM clinicpref WHERE ClinicNum=0 AND PrefName='MassEmailSecret'";
			listStringPrefVals=Db.GetListString(command);			
			if(listStringPrefVals.Count>0) {
				command=$"UPDATE preference SET ValueString='{POut.String(listStringPrefVals[0])}' WHERE PrefName='MassEmailSecret'";
				Db.NonQ(command);
				command="DELETE FROM clinicpref WHERE ClinicNum=0 AND PrefName='MassEmailSecret'";
				Db.NonQ(command);
			}
			command="UPDATE preference SET ValueString=REPLACE(ValueString,'[eClipboardLink]','[eClipboardBYOD]') WHERE PrefName='EClipboardByodSmsTemplate' LIMIT 1";
			Db.NonQ(command);
			command="UPDATE clinicpref SET ValueString=REPLACE(ValueString,'[eClipboardLink]','[eClipboardBYOD]') WHERE PrefName='EClipboardByodSmsTemplate'";
			Db.NonQ(command);
		}//End of 20_3_3() method

		private static void To20_3_9() {
			string command;
			DataTable table;
			//Add an alertcategory link from OdAllTypes and eServices to AlertType.EConnectorMySqlTime, if not already existing.
			foreach(string internalAlertCategory in new string [] { "OdAllTypes","eServices" }) {
				command=@"SELECT alertcategory.AlertCategoryNum,alertcategorylink.AlertType 
					FROM alertcategory 
					LEFT JOIN alertcategorylink ON alertcategorylink.AlertCategoryNum=alertcategory.AlertCategoryNum
					WHERE InternalName='"+POut.String(internalAlertCategory)+"' AND IsHQCategory=1";
				table=Db.GetTable(command);
				if(table.Rows.Count>0 && !table.Rows.AsEnumerable<DataRow>().Any(x => PIn.Int(x["AlertType"].ToString())==24)) {//AlertType.EConnectorMySqlTime=24
					//Alert category exists and there aren't already any links to the EConnectorMySqlTime AlertType
					long alertCategoryNum=PIn.Long(table.Rows[0]["AlertCategoryNum"].ToString());
					command="INSERT INTO alertcategorylink(AlertCategoryNum,AlertType) VALUES("+POut.Long(alertCategoryNum)+",24)";
					Db.NonQ(command);
				}
			}
		}//End of 20_3_9() method

		private static void To20_3_11() {
			string command;
			DataTable table;
			//Add an alert for offices who have sheets with multiple languages to notify that multiple languages feature is available in Web forms as well.
			string alertDescriptStr=@"You have sheets with support for multiple languages. 
Web Forms now support multiple languages and you can update your Web Forms to take advantage of this new feature.
If you are not currently utilizing Web Forms, you can disregard this message.";
			command="SELECT COUNT(*) from sheetfielddef WHERE Language != ''";
			if(Db.GetInt(command)>0) {
				//create the alert item for clinic 0
				command="INSERT INTO alertitem (Type,Actions,Severity,ClinicNum,Description) VALUES("
					+"0,"//Generic
					+"5,"//ActionType.Delete|ActionType.MarkAsRead
					+"1,"//SeverityType.Low
					+"0,"//ClinicNum
					+"'" + alertDescriptStr + "')";
				Db.NonQ(command);
				//Create alert items for all additional clinics
				command=@"INSERT INTO alertitem (Type,Actions,Severity,ClinicNum,Description)
					SELECT 0,5,1,clinic.ClinicNum," +"'"+alertDescriptStr+"'" +
					"FROM clinic WHERE clinic.IsHidden=0";
				Db.NonQ(command);
			}
			command="SELECT * FROM preference WHERE PrefName='ClaimCobInsPaidBehavior'";
			table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ClaimCobInsPaidBehavior','1');";//Default to "ClaimLevel"
				Db.NonQ(command);
			}
			command="ALTER TABLE carrier ADD CobInsPaidBehaviorOverride tinyint NOT NULL";
			Db.NonQ(command);
			command="SELECT COUNT(*) FROM preference WHERE PrefName='EnableEmailAddressAutoComplete'";
			if(!PIn.Bool(Db.GetCount(command))) {
				//Not found.
				command="INSERT INTO preference (PrefName,ValueString) VALUES('EnableEmailAddressAutoComplete','1');";//Default to true
				Db.NonQ(command);
			}
		}//End of 20_3_11() method

		private static void To20_3_14() {
			string command;
			command="UPDATE displayfield SET ColumnWidth=300 WHERE ColumnWidth=0 AND Category=0";
			Db.NonQ(command);
		}

		private static void To20_3_15() {
			string command;
			command="ALTER TABLE employee ADD ReportsTo bigint NOT NULL";
			Db.NonQ(command);
		}

		private static void To20_3_17() {
			string command;
			command="UPDATE apptreminderrule SET apptreminderrule.SendOrder='2' WHERE apptreminderrule.TypeCur=6";//set to Email where BirthdayReminderRule
			Db.NonQ(command);
		}

		private static void To20_3_18() {
			string command;
			command="ALTER TABLE emailhostingtemplate MODIFY COLUMN BodyPlainText MEDIUMTEXT NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE emailhostingtemplate MODIFY COLUMN BodyHTML MEDIUMTEXT NOT NULL";
			Db.NonQ(command);
		}

		private static void To20_3_19() {
			string command;
			//Get the current values for some of the New Pat Appt 
			command="SELECT ValueString FROM preference WHERE PrefName='WebSchedNewPatApptDoubleBooking'";
			string allowDoubleBooking=Db.GetScalar(command);
			command="SELECT ValueString FROM preference WHERE PrefName='WebSchedNewPatAllowProvSelection'";
			string allowProvSelection=Db.GetScalar(command);
			command="SELECT ValueString FROM preference WHERE PrefName='WebSchedNewPatApptSearchAfterDays'";
			string searchAfterDays=Db.GetScalar(command);
			command="SELECT ValueString FROM preference WHERE PrefName='WebSchedNewPatConfirmStatus'";
			string confirmStatus=Db.GetScalar(command);
			command="SELECT ValueString FROM preference WHERE PrefName='WebSchedVerifyNewPatType'";
			string verifyType=Db.GetScalar(command);
			command=@$"INSERT INTO preference (PrefName,ValueString) VALUES 
								('WebSchedExistingPatDoubleBooking', '{allowDoubleBooking}'),
								('WebSchedExistingPatMessage', ''),
								('WebSchedExistingPatAllowProvSelection', '{allowProvSelection}'),
								('WebSchedExistingPatSearchAfterDays', '{searchAfterDays}'),
								('WebSchedExistingPatConfirmStatus', '{confirmStatus}'),
								('WebSchedVerifyExistingPatType','{verifyType}'),
								('WebSchedVerifyExistingPatText', 'Appointment scheduled for [FName] on [ApptDate] [ApptDate] at [OfficeName], [OfficeAddress]'),
								('WebSchedVerifyExistingPatEmailSubj', 'Appointment Scheduling Confirmation'),
								('WebSchedVerifyExistingPatEmailBody', 'Hello [FName] [LName]. Your appointment has been scheduled on [ApptDate] [ApptTime] at [OfficeName], [OfficeAddress]. Please call [OfficePhone] if you have any questions about your appointment. We look forward to seeing you.'),
								('WebSchedVerifyExistingPatEmailTemplateType', '0'),
								('WebSchedExistingPatIgnoreBlockoutTypes', '')";
			Db.NonQ(command);
		}
	
		private static void To20_3_24() {
			string command;
			DataTable table;
			command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedExistingPatWebFormsURL','');";//empty by default
			Db.NonQ(command);
			command="ALTER TABLE timeadjust ADD IsUnpaidProtectedLeave tinyint NOT NULL DEFAULT 0,ADD SecuUserNumEntry bigint NOT NULL DEFAULT 0,ADD INDEX (SecuUserNumEntry)";
			Db.NonQ(command);
			//Add new ProtectedLeaveAdjustmentEdit (190) permission for all groups that already have TimeCardsEditAll (29) permission.
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=29";
			table=Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				long groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					 +"VALUES("+POut.Long(groupNum)+",190)";
				Db.NonQ(command);
			}
		}

		private static void To20_3_34() {
			string command;
			DataTable table;
			command="SELECT FieldName FROM patfielddef WHERE FieldType=6";//CareCreditStatus
			List<string> listCareCreditFieldDefs=Db.GetListString(command);
			if(listCareCreditFieldDefs.Count>0) {
				string inClause=string.Join(",",listCareCreditFieldDefs.Select(x => $"'{POut.String(x)}'"));
				command=$"UPDATE patfield SET FieldValue='Unable to Pre-Approve - Refer Patient to Credit Application' WHERE FieldName IN({inClause}) AND FieldValue='UNABLE TO PRE-APPROVE'";
				Db.NonQ(command);
				command=$"UPDATE patfield SET FieldValue='Pre-Approved' WHERE FieldName IN({inClause}) AND FieldValue='PRE-APPROVED'";
				Db.NonQ(command);
				command=$"UPDATE patfield SET FieldValue='Cardholder' WHERE FieldName IN({inClause}) AND FieldValue='CARDHOLDER'";
				Db.NonQ(command);
				command=$"UPDATE patfield SET FieldValue='Call For Auth' WHERE FieldName IN({inClause}) AND FieldValue='CALL FOR AUTH'";
				Db.NonQ(command);
				command="UPDATE patfield SET FieldName='CareCredit Pre-Approval Status' WHERE FieldName='CareCredit Approval Status'";
				Db.NonQ(command);
				//Change the default CareCredit patfielddef
				command="UPDATE patfielddef SET FieldName='CareCredit Pre-Approval Status' WHERE FieldType=6 AND FieldName='CareCredit Approval Status'";
				Db.NonQ(command);
			}
		}

		private static void To20_3_38() {
			string command;
			command="SELECT COUNT(*) FROM preference WHERE PrefName='EnterpriseExactMatchPhone'";
			//This preference might have already been added in 20.2.49.
			if(Db.GetScalar(command)=="0") {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseExactMatchPhone','0')";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseExactMatchPhoneNumDigits','10')";
				Db.NonQ(command);
			}
		}

		private static void To20_3_41() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseAllowRefreshWhileTyping','1')";
			Db.NonQ(command);
		}

		private static void To20_3_44() {
			string command;
			command="ALTER TABLE phonenumber ADD INDEX (PhoneNumberDigits)";
			Db.NonQ(command);
			command="SELECT COUNT(*) FROM preference WHERE PrefName='EmailSendExternalTimeoutMs'";
			if(Db.GetScalar(command)=="0") {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailSendExternalTimeoutMs','60000')";//default to 1 minute
				Db.NonQ(command);				
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailSendExternalManagerTimeoutMs','240000')";//default to 4 minutes
				Db.NonQ(command);
			}
		}

		private static void To20_3_48() {
			string command;
			//Moving codes to the Obsolete category that were deleted in CDT 2021.
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				//Move deprecated codes to the Obsolete procedure code category.
				//Make sure the procedure code category exists before moving the procedure codes.
				string procCatDescript="Obsolete";
				long defNum=0;
				command="SELECT DefNum FROM definition WHERE Category=11 AND ItemName='"+POut.String(procCatDescript)+"'";//11 is DefCat.ProcCodeCats
				DataTable dtDef=Db.GetTable(command);
				if(dtDef.Rows.Count==0) { //The procedure code category does not exist, add it
					command="SELECT COUNT(*) FROM definition WHERE Category=11";//11 is DefCat.ProcCodeCats
					int countCats=PIn.Int(Db.GetCount(command));
						command="INSERT INTO definition (Category,ItemName,ItemOrder) "
								+"VALUES (11"+",'"+POut.String(procCatDescript)+"',"+POut.Int(countCats)+")";//11 is DefCat.ProcCodeCats
					defNum=Db.NonQ(command,true);
				}
				else { //The procedure code category already exists, get the existing defnum
					defNum=PIn.Long(dtDef.Rows[0]["DefNum"].ToString());
				}
				string[] cdtCodesDeleted=new string[] {
					"D3427",
					"D5994",
					"D6052",
					"D7960"
				};
				//Change the procedure codes' category to Obsolete.
				command="UPDATE procedurecode SET ProcCat="+POut.Long(defNum)
					+" WHERE ProcCode IN('"+string.Join("','",cdtCodesDeleted.Select(x => POut.String(x)))+"') ";
				Db.NonQ(command);
			}//end United States CDT codes update
		}

		private static void To20_3_54() {
			string command;
			command="INSERT INTO preference (PrefName,ValueString) VALUES('PdfLaunchWindow','0');";//false by default
			Db.NonQ(command);
		}

		private static void To20_4_1() {
			string command;
			DataTable table;
			command="ALTER TABLE computerpref ADD Zoom int NOT NULL DEFAULT 0";
			Db.NonQ(command);
			LargeTableHelper.AlterLargeTable("recall","RecallNum",
					new List<Tuple<string,string>> { Tuple.Create("TimePatternOverride","varchar(255) NOT NULL") });
			//Add Query Monitor permission to everyone.
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					   +"VALUES("+POut.Long(groupNum)+",192)";//192 - QueryMonitor 
				Db.NonQ(command);
			}
			//check databasemaintenance for GroupNoteWithInvalidAptNum, insert if not there and set IsOld to True or update to set IsOld to true
			command="SELECT MethodName FROM databasemaintenance WHERE MethodName='GroupNoteWithInvalidAptNum'";
			string methodName=Db.GetScalar(command);
			if(methodName=="") {//didn't find row in table, insert
				command="INSERT INTO databasemaintenance (MethodName,IsOld) VALUES ('GroupNoteWithInvalidAptNum',1)";
			}
			else {//found row, update IsOld
				command="UPDATE databasemaintenance SET IsOld = 1 WHERE MethodName = 'GroupNoteWithInvalidAptNum'";
			}
			Db.NonQ(command);
			//Add CommlogCreate permission to everyone who has CommlogEdit.
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=43";
			table=Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES("+POut.Long(groupNum)+",193)";
				Db.NonQ(command);
			}
			command="DROP TABLE IF EXISTS emailsecure";
			Db.NonQ(command);
			command=@"CREATE TABLE emailsecure (
				EmailSecureNum bigint NOT NULL auto_increment PRIMARY KEY,
				ClinicNum bigint NOT NULL,
				PatNum bigint NOT NULL,
				EmailMessageNum bigint NOT NULL,
				EmailChainFK bigint NOT NULL,
				EmailFK bigint NOT NULL,
				DateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				SecDateTEdit timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
				INDEX(ClinicNum),
				INDEX(PatNum),
				INDEX(EmailMessageNum),
				INDEX(EmailChainFK),
				INDEX(EmailFK)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('EmailSecureDownloadInterval','10');";//Default to 10 minutes
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('EmailSecureDownloadTimeoutSeconds','300');";//Default to 5 minutes
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('EmailSecureAlert','1');";//Default to true
			Db.NonQ(command);
			//Add an alertcategory link from OdAllTypes and eServices for AlertType.EmailSecure
			foreach(string internalAlertCategory in new string[] { "OdAllTypes","eServices" }) {
				command=@"SELECT alertcategory.AlertCategoryNum,alertcategorylink.AlertType 
					FROM alertcategory 
					LEFT JOIN alertcategorylink ON alertcategorylink.AlertCategoryNum=alertcategory.AlertCategoryNum
					WHERE InternalName='"+POut.String(internalAlertCategory)+"' AND IsHQCategory=1";
				table=Db.GetTable(command);
				if(table.Rows.Count>0 && !table.Rows.AsEnumerable<DataRow>().Any(x => PIn.Int(x["AlertType"].ToString())==27)) {//AlertType.EmailSecure=27
					//Alert category exists and there aren't already any links to the EConnectorMySqlTime AlertType
					long alertCategoryNum=PIn.Long(table.Rows[0]["AlertCategoryNum"].ToString());
					command="INSERT INTO alertcategorylink(AlertCategoryNum,AlertType) VALUES("+POut.Long(alertCategoryNum)+",27)";
					Db.NonQ(command);
				}
			}
			command="ALTER TABLE programproperty MODIFY PropertyValue TEXT";
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=24"; //SecurityAdmin - Pulling all security admins
			table=Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					+"VALUES ("+POut.Long(groupNum)+",194)"; //WebFormAccess - Setting webforms permission for security admins 
				Db.NonQ(command);
			}
			command="ALTER TABLE program ADD IsDisabledByHq tinyint NOT NULL";//All default to false.
			Db.NonQ(command);
			command="SELECT ProgramNum FROM program WHERE ProgName='PDMP'";
			long programNum=Db.GetLong(command);
			command=$"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue) VALUES "+
				$"({POut.Long(programNum)},'Illinois PDMP Url','')," +//Other Illinois PDMP properties already exist
				$"({POut.Long(programNum)},'Washington PDMP FacilityID',''), " +
				$"({POut.Long(programNum)},'Washington PDMP Username',''), " +
				$"({POut.Long(programNum)},'Washington PDMP Password',''), " +
				$"({POut.Long(programNum)},'Washington PDMP Url',''), " +
				$"({POut.Long(programNum)},'Maryland PDMP FacilityID',''), " +
				$"({POut.Long(programNum)},'Maryland PDMP Username',''), " +
				$"({POut.Long(programNum)},'Maryland PDMP Password',''), " +
				$"({POut.Long(programNum)},'Maryland PDMP Url',''), " +
				$"({POut.Long(programNum)},'California PDMP FacilityID',''), " +
				$"({POut.Long(programNum)},'California PDMP Username',''), " +
				$"({POut.Long(programNum)},'California PDMP Password',''), " +
				$"({POut.Long(programNum)},'California PDMP Url',''), " +
				$"({POut.Long(programNum)},'Kentucky PDMP FacilityID',''), " +
				$"({POut.Long(programNum)},'Kentucky PDMP Username',''), " +
				$"({POut.Long(programNum)},'Kentucky PDMP Password',''), " +
				$"({POut.Long(programNum)},'Kentucky PDMP Url',''), " +
				$"({POut.Long(programNum)},'Utah PDMP FacilityID',''), " +
				$"({POut.Long(programNum)},'Utah PDMP Username',''), " +
				$"({POut.Long(programNum)},'Utah PDMP Password',''), " +
				$"({POut.Long(programNum)},'Utah PDMP Url',''), " +
				$"({POut.Long(programNum)},'PDMP Provider License Field','')";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS activeinstance";
			Db.NonQ(command);
			command=@"CREATE TABLE activeinstance (
					ActiveInstanceNum bigint NOT NULL auto_increment PRIMARY KEY,
					ComputerNum bigint NOT NULL,
					UserNum bigint NOT NULL,
					ProcessId bigint NOT NULL,
					DateTimeLastActive datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					SecDateTEdit timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
					INDEX(ComputerNum),
					INDEX(UserNum),
					INDEX(ProcessId)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			//Get the User Groups with Setup permission
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=8"; //PermType 8 is Setup
			table=Db.GetTable(command);
			//For each user group, add that group to a list of values that will receive the new permission
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					 +"VALUES("+POut.Long(groupNum)+",195)";  //195 is CloseOtherSessions
				Db.NonQ(command);
			}
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('CloudSessionLimit','15')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('CloudAlertWithinLimit','3')";
			Db.NonQ(command);
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes' AND IsHQCategory=1";
			string alertCategoryNumString=Db.GetScalar(command);
			command="INSERT INTO alertcategorylink(AlertCategoryNum,AlertType) VALUES('"+alertCategoryNumString+"','29')"; //29 - Cloud Session Limit
			Db.NonQ(command);
			if(!LargeTableHelper.IndexExists("taskhist","KeyNum")) {
				command="ALTER TABLE taskhist ADD INDEX (KeyNum)";
				Db.NonQ(command);
			}
			command="ALTER TABLE sheet ADD RevID int NOT NULL";
			Db.NonQ(command);
			command="UPDATE sheet SET RevID=1";
			Db.NonQ(command);
			command="ALTER TABLE sheetdef ADD RevID int NOT NULL";
			Db.NonQ(command);
			command="UPDATE sheetdef SET RevID=1";
			Db.NonQ(command);
			command="ALTER TABLE eclipboardsheetdef ADD PrefillStatus tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE provider ADD PreferredName varchar(100) NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('EConnectorCleanupLoggerIntervalDays','30')";//Defaults to 30 days like the BroadcastMonitor Logger Cleanup
			Db.NonQ(command);
			command="ALTER TABLE perioexam ADD Note text NOT NULL";
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 20.4.1 - claimproc table structure");
			LargeTableHelper.AlterLargeTable("claimproc","ClaimProcNum",
				indexColsAndNames:new List<Tuple<string,string>>() { Tuple.Create("ClaimNum,ClaimPaymentNum,InsPayAmt,DateCP,IsTransfer","indexOutClaimCovering") });
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 20.4.1 - task table structure");
			LargeTableHelper.AlterLargeTable("task","TaskNum",
				colNamesAndDefs:new List<Tuple<string,string>> { Tuple.Create("DescriptOverride","varchar(255) NOT NULL") });
			LargeTableHelper.AlterLargeTable("taskhist","TaskHistNum",
				colNamesAndDefs:new List<Tuple<string,string>> { Tuple.Create("DescriptOverride","varchar(255) NOT NULL") });
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 20.4.1");
			command="DROP TABLE IF EXISTS sessiontoken";
			Db.NonQ(command);
			command=@"CREATE TABLE sessiontoken (
					SessionTokenNum bigint NOT NULL auto_increment PRIMARY KEY,
					SessionTokenHash varchar(255) NOT NULL,
					Expiration datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					TokenType tinyint NOT NULL,
					FKey bigint NOT NULL,
					INDEX(FKey),
					INDEX(SessionTokenHash(20)),
					INDEX(Expiration)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
		} //End of 20_4_1() method

		private static void To20_4_4() {
			string command="DROP TABLE IF EXISTS emailsecureattach";
			Db.NonQ(command);
			command=@"CREATE TABLE emailsecureattach (
				EmailSecureAttachNum bigint NOT NULL auto_increment PRIMARY KEY,
				ClinicNum bigint NOT NULL,
				EmailAttachNum bigint NOT NULL,
				EmailSecureNum bigint NOT NULL,
				AttachmentGuid varchar(50) NOT NULL,
				DisplayedFileName varchar(255) NOT NULL,
				Extension varchar(255) NOT NULL,
				DateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				SecDateTEdit timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
				INDEX(ClinicNum),
				INDEX(EmailAttachNum),
				INDEX(EmailSecureNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT FieldName FROM patfielddef WHERE FieldType=6";//CareCreditStatus
			List<string> listCareCreditFieldDefs=Db.GetListString(command);
			if(listCareCreditFieldDefs.Count>0) {
				string inClause=string.Join(",",listCareCreditFieldDefs.Select(x => $"'{POut.String(x)}'"));
				command=$"UPDATE patfield SET FieldValue='Unable to Pre-Approve - Refer Patient to Credit Application' WHERE FieldName IN({inClause}) AND FieldValue='UNABLE TO PRE-APPROVE'";
				Db.NonQ(command);
				command=$"UPDATE patfield SET FieldValue='Pre-Approved' WHERE FieldName IN({inClause}) AND FieldValue='PRE-APPROVED'";
				Db.NonQ(command);
				command=$"UPDATE patfield SET FieldValue='Cardholder' WHERE FieldName IN({inClause}) AND FieldValue='CARDHOLDER'";
				Db.NonQ(command);
				command=$"UPDATE patfield SET FieldValue='Call For Auth' WHERE FieldName IN({inClause}) AND FieldValue='CALL FOR AUTH'";
				Db.NonQ(command);
				command="UPDATE patfield SET FieldName='CareCredit Pre-Approval Status' WHERE FieldName='CareCredit Approval Status'";
				Db.NonQ(command);
				//Change the default CareCredit patfielddef
				command="UPDATE patfielddef SET FieldName='CareCredit Pre-Approval Status' WHERE FieldType=6 AND FieldName='CareCredit Approval Status'";
				Db.NonQ(command);
			}
		}

		private static void To20_4_5() {
			string command;
			//Insert 3Shape bridge-----------------------------------------------------------------
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				+") VALUES("
				+"'ThreeShape', "
				+"'3Shape from www.3shape.com/en', "
				+"'0', "
				+"'"+POut.String(@"C:\Program Files\3Shape\Dental Desktop\Plugins\ThreeShape.PMSIntegration\DentalDesktopCmd.exe")+"', "
				+"'"+"', "//No command line args
				+"'')";
			long programNum=Db.NonQ(command,true);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				+") VALUES("
				+"'"+POut.Long(programNum)+"', "
				+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				+"'0')";
			Db.NonQ(command);
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				+"VALUES ("
				+"'"+POut.Long(programNum)+"', "
				+"'2', "//ToolBarsAvail.ChartModule
				+"'3Shape')";
			Db.NonQ(command);
			//End 3Shape bridge-----------------------------------------------------------------
			//Insert QuickBooksOnline bridge----------------------------------------------------------------- 
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				 +") VALUES("
				 +"'QuickBooksOnline', "
				 +"'Web-based QuickBooks', "
				 +"'0', "
				 +"'', "
				 +"'', "
				 +"'')";
			programNum=Db.NonQ(command,true);
			command=$"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue) VALUES "+
				$"({POut.Long(programNum)},'Deposit Accounts',''), " +
				$"({POut.Long(programNum)},'Income Accounts',''), " +
				$"({POut.Long(programNum)},'Class Refs',''), " +
				$"({POut.Long(programNum)},'Access Token',''), " +
				$"({POut.Long(programNum)},'Refresh Token',''), "+
				$"({POut.Long(programNum)},'Realm ID','')";
			Db.NonQ(command);
		} //End of 20_4_5() method

		private static void To20_4_6() {
			string command;
			//Insert Appriss bridge-------------------------------------------------------------
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note) "+
								"VALUES ('Appriss','Appriss','0','','','')";
			long apprissProgNum=Db.NonQ(command,getInsertID:true);
			command=$@"INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText)
			   VALUES ('{POut.Long(apprissProgNum)}','7','Appriss PDMP')";//ToolBarsAvail.MainToolbar
			Db.NonQ(command);
			command=$@"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue)
								VALUES ({POut.Long(apprissProgNum)},'Appriss Username','')";
			Db.NonQ(command);
			command=$@"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue)
								VALUES ({POut.Long(apprissProgNum)},'Appriss Password','')";
			Db.NonQ(command);
			command=$@"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue)
								VALUES ({POut.Long(apprissProgNum)},'Appriss FacilityID','')";
			Db.NonQ(command);
			command=$@"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue)
								VALUES ({POut.Long(apprissProgNum)},'Appriss Url','')";
			Db.NonQ(command);
			command=$@"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue)
								VALUES ({POut.Long(apprissProgNum)},'Appriss Client Key','')";
			Db.NonQ(command);
			command=$@"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue)
								VALUES ({POut.Long(apprissProgNum)},'Appriss Client Password','')";
			Db.NonQ(command);
			command=$@"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue)
								VALUES ({POut.Long(apprissProgNum)},'PDMP Provider License Field','')";
			//End Appriss bridge----------------------------------------------------------------------------
			Db.NonQ(command);
		}
		
		private static void To20_4_8() {
			string command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailHostingSignatureHtml','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailHostingSignaturePlainText','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailHostingAlias','')";
			Db.NonQ(command);
			command="ALTER TABLE activeinstance CHANGE SecDateTEdit DateTRecorded datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
			Db.NonQ(command);
		}

		private static void To20_4_10() {
			string command;
			command="SELECT COUNT(*) FROM preference WHERE PrefName='EnterpriseExactMatchPhone'";
			//This preference might have already been added in 20.2.49.
			if(Db.GetScalar(command)=="0") {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseExactMatchPhone','0')";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseExactMatchPhoneNumDigits','10')";
				Db.NonQ(command);
			}
			//Add Zoom permission to everyone.
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			DataTable table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					   +"VALUES("+POut.Long(groupNum)+",199)";//199 = Zoom 
				Db.NonQ(command);
			}
		}
		private static void To20_4_11() {
			string command;
			string charSet="\r\n !#$%\'\"()*+,-./:;<=>?@_¡£¥§¿&¤0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzÄÅÆÇÉÑØøÜßÖàäåæèéìñòöùüΔΦΓΛΩΠΨΣΘΞ";
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES ('GsmCharSet','{POut.String(charSet)}')";//will be retrieved from HQ
			Db.NonQ(command);
			string charSetExtended="|^€{}[]~\\";
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES ('GsmExtendedCharSet','{POut.String(charSetExtended)}')";//will be retrieved from HQ
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('BytesPerSmsHeader','6')";//will be retrieved from HQ
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('BytesPerSmsMessagePart','140')";
			Db.NonQ(command);
		}

		private static void To20_4_17() {
			string command;
			command="SELECT COUNT(*) FROM preference WHERE PrefName='EnterpriseAllowRefreshWhileTyping'";
			//This preference might have already been added in 20.3.41
			if(Db.GetScalar(command)=="0") {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseAllowRefreshWhileTyping','1')";
				Db.NonQ(command);
			}
		}

		private static void To20_4_20() {
			string command;
			command="UPDATE program SET program.ProgDesc='QuickBooks Online' "
				+"WHERE program.ProgName='QuickBooksOnline' AND program.ProgDesc='Web-based QuickBooks'";
			Db.NonQ(command);
		}

		private static void To20_4_21() {
			string command;
			if(!LargeTableHelper.IndexExists("phonenumber","PhoneNumberDigits")) {//index may have been added in version 20.3.44
				command="ALTER TABLE phonenumber ADD INDEX (PhoneNumberDigits)";
				Db.NonQ(command);
			}
		}

		private static void To20_4_22() {
			string command="SELECT COUNT(*) FROM preference WHERE PrefName='EmailSendExternalTimeoutMs'";
			//This preference might have already been added in 20.3.44
			if(Db.GetScalar(command)=="0") {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailSendExternalTimeoutMs','60000')";//default to 1 minute
				Db.NonQ(command);				
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailSendExternalManagerTimeoutMs','240000')";//default to 4 minutes
				Db.NonQ(command);
			}
		}

		private static void To20_4_28() {
			string command;
			DataTable table;
			//Moving codes to the Obsolete category that were deleted in CDT 2021.
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				//Move deprecated codes to the Obsolete procedure code category.
				//Make sure the procedure code category exists before moving the procedure codes.
				string procCatDescript="Obsolete";
				long defNum=0;
				command="SELECT DefNum FROM definition WHERE Category=11 AND ItemName='"+POut.String(procCatDescript)+"'";//11 is DefCat.ProcCodeCats
				DataTable dtDef=Db.GetTable(command);
				if(dtDef.Rows.Count==0) { //The procedure code category does not exist, add it
					command="SELECT COUNT(*) FROM definition WHERE Category=11";//11 is DefCat.ProcCodeCats
					int countCats=PIn.Int(Db.GetCount(command));
						command="INSERT INTO definition (Category,ItemName,ItemOrder) "
								+"VALUES (11"+",'"+POut.String(procCatDescript)+"',"+POut.Int(countCats)+")";//11 is DefCat.ProcCodeCats
					defNum=Db.NonQ(command,true);
				}
				else { //The procedure code category already exists, get the existing defnum
					defNum=PIn.Long(dtDef.Rows[0]["DefNum"].ToString());
				}
				string[] cdtCodesDeleted=new string[] {
					"D3427",
					"D5994",
					"D6052",
					"D7960"
				};
				//Change the procedure codes' category to Obsolete.
				command="UPDATE procedurecode SET ProcCat="+POut.Long(defNum)
					+" WHERE ProcCode IN('"+string.Join("','",cdtCodesDeleted.Select(x => POut.String(x)))+"') ";
				Db.NonQ(command);
			}//end United States CDT codes update
			 //Add an alertcategory link from OdAllTypes and eServices for AlertType.WebSchedExistingPatApptCreated
			foreach(string internalAlertCategory in new string[] { "OdAllTypes","eServices" }) {
				command=@"SELECT alertcategory.AlertCategoryNum,alertcategorylink.AlertType 
					FROM alertcategory 
					LEFT JOIN alertcategorylink ON alertcategorylink.AlertCategoryNum=alertcategory.AlertCategoryNum
					WHERE InternalName='"+POut.String(internalAlertCategory)+"' AND IsHQCategory=1";
				table=Db.GetTable(command);
				if(table.Rows.Count>0 && !table.Rows.AsEnumerable<DataRow>().Any(x => PIn.Int(x["AlertType"].ToString())==28)) {//AlertType.WebSchedExistingPatApptCreated=28
					//Alert category exists and there aren't already any links to the AlertType.WebSchedExistingPatApptCreated
					long alertCategoryNum=PIn.Long(table.Rows[0]["AlertCategoryNum"].ToString());
					command="INSERT INTO alertcategorylink(AlertCategoryNum,AlertType) VALUES("+POut.Long(alertCategoryNum)+",28)";
					Db.NonQ(command);
				}
			}
		}//end of 20_4_28() method

		private static void To20_4_29() {
			string command;
			DataTable table;
			command="ALTER TABLE deposit ADD IsSentToQuickBooksOnline tinyint NOT NULL";
			Db.NonQ(command);
		}

		private static void To20_4_38() {
			string command;
			command="SELECT COUNT(*) FROM preference WHERE preference.PrefName='PdfLaunchWindow';";
			if(PIn.Int(Db.GetCount(command))==0) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('PdfLaunchWindow','0');";//false by default
				Db.NonQ(command);
			}
		}

		private static void To20_4_56() {
			DoseSpotSelfReportedInvalidNote();
		}
	} 
}	
