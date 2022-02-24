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
			return new AccountApi(guid,secret);
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
			string plainTextSignature;
			string htmlSignature;
			if(clinicNum==0) {
				htmlSignature=PrefC.GetString(PrefName.EmailHostingSignatureHtml);
				plainTextSignature=PrefC.GetString(PrefName.EmailHostingSignaturePlainText);
			}
			else {
				htmlSignature=ClinicPrefs.GetPrefValue(PrefName.EmailHostingSignatureHtml,clinicNum);
				plainTextSignature=ClinicPrefs.GetPrefValue(PrefName.EmailHostingSignaturePlainText,clinicNum);
			}
			UpdateSignatureRequest updateSignatureRequest=new UpdateSignatureRequest(){ SignatureHtml=htmlSignature, SignaturePlainText=plainTextSignature };
			UpdateSignatureResponse updateSignatureResponse=api.UpdateSignature(updateSignatureRequest);
			#endregion
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