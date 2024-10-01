using CodeBase;
using System;
using System.Collections.Generic;
using System.Text;
using WebServiceSerializer;

namespace OpenDentBusiness.AutoComm {
	public class MsgToPayTagReplacer : TagReplacer {
		public const string MONTHLY_CARD_TAG="[monthlyCardsOnFile]";
		public const string NAME_PREF_TAG="[namePref]";
		public const string PATNUM_TAG="[PatNum]";
		public const string CURMONTH_TAG="[currentMonth]";
		#region URL Tags
		///<summary>Tag is replaced by a medium URL to the patient portal to view statements.</summary>
		public const string STATEMENT_URL_TAG="[StatementURL]";
		///<summary>Tag is replaced by a short URL to the patient portal to view statements.</summary>
		public const string STATEMENT_SHORT_TAG="[StatementShortURL]";
		///<summary>Tag is replaced by a short URL to the payment portal to pay outstanding balances.</summary>
		public const string MSG_TO_PAY_TAG="[MsgToPayURL]";
		#endregion URL Tags
		///<summary>Tag is replaced by the current balance on the associated statement.</summary>
		public const string STATEMENT_BALANCE_TAG="[StatementBalance]";
		///<summary>Tag is replaced by the current insurance estimate on the associated statement.</summary>
		public const string STATEMENT_INS_EST_TAG="[StatementInsuranceEst]";
		#region Email Tags
		public const string NAMEFL_NOPREF_TAG="[nameFLnoPref]";
		public const string NAMEF_NOPREF_TAG="[nameFnoPref]";
		#endregion Email Tags

		public MsgToPayTagReplacer() { }

		protected override void ReplaceTagsChild(StringBuilder sbTemplate,AutoCommObj autoCommObj,bool isEmail) {
			base.ReplaceTagsChild(sbTemplate,autoCommObj,isEmail);
			if(sbTemplate.ToString().Contains(MONTHLY_CARD_TAG)) {
				ReplaceOneTag(sbTemplate,MONTHLY_CARD_TAG,CreditCards.GetMonthlyCardsOnFile(autoCommObj.PatNum),isEmail);
			}
			ReplaceOneTag(sbTemplate,NAME_PREF_TAG,autoCommObj.NamePreferred,isEmail);
			ReplaceOneTag(sbTemplate,PATNUM_TAG,autoCommObj.PatNum.ToString(),isEmail);
			ReplaceOneTag(sbTemplate,CURMONTH_TAG,DateTime_.Now.ToString("MMMM"),isEmail);
			Patient patient=Patients.GetPat(autoCommObj.PatNum);
			Statement statement=Statements.GetStatement(autoCommObj.StatementNum);//Retrieve statement from DB to update with a ShortGuid from WSHQ and replace balance totals
			if(statement!=null) {
				ReplaceOneTag(sbTemplate,STATEMENT_BALANCE_TAG,statement.BalTotal.ToString("0.00"),isEmail);
				ReplaceOneTag(sbTemplate,STATEMENT_INS_EST_TAG,statement.InsEst.ToString("0.00"),isEmail);
			}
			if(sbTemplate.ToString().ToLower().Contains(STATEMENT_URL_TAG.ToLower())//This tag triggers a call to WSHQ to generate a ShortGuid for the statement
				|| sbTemplate.ToString().ToLower().Contains(STATEMENT_SHORT_TAG.ToLower())//This tag triggers a call to WSHQ to generate a ShortGuid for the statement
				|| sbTemplate.ToString().Contains(MSG_TO_PAY_TAG))//This tag triggers a call to WSHQ to generate a ShortGuid for the statement which will then also be used for MsgToPay redirection.
			{
				if(statement!=null) {
					//This goes to WSHQ to generate a ShortGuidLookup at HQ and URLs for the statement. Statements are updated in the db in this method.
					Statements.AssignURLsIfNecessary(statement,patient);
					ReplaceOneTag(sbTemplate,STATEMENT_URL_TAG,statement.StatementURL,isEmail);
					ReplaceOneTag(sbTemplate,STATEMENT_SHORT_TAG,statement.StatementShortURL,isEmail);
					if(!string.IsNullOrWhiteSpace(statement.ShortGUID) && sbTemplate.ToString().Contains(MSG_TO_PAY_TAG)) {
						List<PayloadItem> listPayloadItems=new List<PayloadItem> {
							new PayloadItem(statement.ShortGUID,"ShortGuid"),
						};
						//Explicitly chose to make this WSHQ call a seperate call from AssignURLs because in case of backwards compatability issues with the WebMethod
						string payload=PayloadHelper.CreatePayload(PayloadHelper.CreatePayloadContent(listPayloadItems),eServiceCode.PaymentPortalApi);
						string msgToPayUrl=WebSerializer.DeserializePrimitive<string>(WebServiceMainHQProxy.GetWebServiceMainHQInstance().GetMsgToPayShortUrl(payload));
						ReplaceOneTag(sbTemplate,MSG_TO_PAY_TAG,msgToPayUrl,isEmail);
					}
				}
			}
			if(isEmail) {//Tags that existed for ReplaceVarsForEmail only
				ReplaceOneTag(sbTemplate,NAMEFL_NOPREF_TAG,patient.GetNameFLnoPref(),isEmail);
				ReplaceOneTag(sbTemplate,NAMEF_NOPREF_TAG,patient.FName,isEmail);
			}
		}

		///<summary>Replaces variable tags with the information from the patient passed in. Throws exceptions if attempt to get short guids from ODHQ has issues.</summary>
		public string ReplaceTagsForStatement(string messageTemplate,Patient patient,Statement statement,Clinic clinic=null,bool includeInsEst=false,bool isEmail=false) {
			AutoCommObj autoCommObj=new AutoCommObj();
			autoCommObj.PatNum=patient.PatNum;
			autoCommObj.NameF=patient.FName;
			autoCommObj.NamePreferredOrFirst=patient.GetNameFirstOrPreferred();
			autoCommObj.NamePreferred=patient.Preferred;
			autoCommObj.ProvNum=patient.PriProv;
			autoCommObj.StatementNum=statement.StatementNum;
			if(clinic==null) {
				clinic=Clinics.GetClinic(patient.ClinicNum)??Clinics.GetPracticeAsClinicZero();
			}
			//Replace Statement Balance here
			StringBuilder sb=new StringBuilder();
			sb.Append(messageTemplate);
			messageTemplate=sb.ToString();
			return ReplaceTags(messageTemplate,autoCommObj,clinic,isEmailBody:isEmail);
		}

		///<summary>Replaces text to pay tags with a short Url by building a new ShortUrl with the generated shortGuid.</summary>
		public static string ReplaceUrlTags(string msgTextIn,string msgToPayUrl,string statementMediumUrl,string statementShortUrl) {
			if(string.IsNullOrWhiteSpace(msgTextIn)) {
				return "";
			}
			StringBuilder sb=new StringBuilder();
			sb.Append(msgToPayUrl);
			StringTools.RegReplace(sb,GetPatternFromTag(MSG_TO_PAY_TAG),msgToPayUrl);
			StringTools.RegReplace(sb,GetPatternFromTag(STATEMENT_URL_TAG),statementMediumUrl);
			StringTools.RegReplace(sb,GetPatternFromTag(STATEMENT_SHORT_TAG),statementShortUrl);
			return sb.ToString();
		}
	}
}
