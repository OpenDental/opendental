using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenDentBusiness.AutoComm {
	public class NewPatientThankYouTagReplacer : ApptTagReplacer {
		///<summary>Gets the WebForm url tag and includes the webSheetDefID this message will link to. example return: "[NewPatWebFormURL(123)]"</summary>
		public static string GetNewPatWebFormURLTag(string webSheetDefID,bool isRegExPattern=false) {
			string tag=ApptNewPatThankYouSents.NEW_PAT_WEB_FORM_TAG.Replace("]",$"({webSheetDefID})]");
			if(isRegExPattern) {
				tag=tag.Replace("[","\\[")
					.Replace("(","\\(")
					.Replace(")","\\)");
			}
			return tag;
		}

		///<summary>Returns true if the WebForm tag is present with a webSheetDefID.</summary>
		public static bool DoesContainNewPatWebFormURLTag(string message) {
			return new Regex($"\\[NewPatWebFormURL\\(([0-9]+)(,[0-9]+)*\\)\\]").IsMatch(message);
		}

		///<summary>Returns the webSheetDefID within the WebForm tag. Returns empty list when no ID is found.</summary>
		public static List<string> GetWebSheetDefIDs(string message) {
			string sheetIDMatch="([0-9]+)(,[0-9]+)*";
			Regex regexSheetID=new Regex(sheetIDMatch);
			Match matchTag=new Regex($"\\[NewPatWebFormURL\\({sheetIDMatch}\\)\\]").Match(message);
			if(matchTag.Success) {
				//Extract the Web Form ID from the replacement tag
				string matchValue=regexSheetID.Match(matchTag.Value).Value;
				return matchValue.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).ToList();
			}
			return new List<string>();
		}

		protected override void ReplaceTagsChild(StringBuilder sbTemplate,AutoCommObj autoCommObj,bool isEmail) {
			base.ReplaceTagsChild(sbTemplate,autoCommObj,isEmail);
			ReplaceNewPatWebFormURLTag(sbTemplate,autoCommObj,isEmail);
		}

		public void ReplaceNewPatWebFormURLTag(StringBuilder sbTemplate,AutoCommObj aco,bool isEmailBody) {
			string webSheetDefIDs;
			bool useClinicPref=false;
			if(PrefC.HasClinicsEnabled) {//Check if customer has Clinics. If clinics are enabled check to see if the current clinic has chosen to use the HQ preference.
				List<ClinicPref> listClinicPrefs=ClinicPrefs.GetPrefAllClinics(PrefName.AutoMsgingUseDefaultPref,includeDefault:false);//Get all ClinicPrefs for to see if Clinic is Using Defaults.
				ClinicPref clinicPref=listClinicPrefs.FirstOrDefault(x => x.ClinicNum==aco.ClinicNum);
				//If a clinicPref is null, no pref has been inserted meaning they have not checked the Use Default checkbox.
				useClinicPref=clinicPref==null || !PIn.Bool(clinicPref.ValueString);//Pref value true means Clinic has chosen to use defaults, false means use the ClinicPref
			}
			if(useClinicPref) {//Clinic opted to use their own preference
				List<ClinicPref> listClinicPrefs=ClinicPrefs.GetPrefAllClinics(PrefName.ApptNewPatientThankYouWebSheetDefID,true);
				ClinicPref clinicPref=listClinicPrefs.FirstOrDefault(x => x.ClinicNum==aco.ClinicNum);
				webSheetDefIDs=PIn.String(clinicPref?.ValueString??listClinicPrefs.FirstOrDefault(x => x.ClinicNum==0)?.ValueString??"0");
			}
			else {//No clinics or clinic opted for using HQ preference
				webSheetDefIDs=PrefC.GetString(PrefName.ApptNewPatientThankYouWebSheetDefID);
			}
			string replaceValue=GetNewPatWebFormURLTag(webSheetDefIDs);
			ReplaceOneTag(sbTemplate,ApptNewPatThankYouSents.NEW_PAT_WEB_FORM_TAG,replaceValue,isEmailBody);
		}

		///<summary>Thank yous use the appointment scheduled time as the DateTimeEvent, need to explicitly use the AptDateTime.</summary>
		protected override List<AutoCommObj> GetAggregateGrouping(List<AutoCommObj> listAutoCommObjs) {
			List<ApptLite> listApptLites=listAutoCommObjs.OfType<ApptLite>().ToList();
			List<AutoCommObj> listAutoCommObjReturn=new List<AutoCommObj>();
			listAutoCommObjReturn.AddRange(listApptLites.GroupBy(x => new { x.PatNum,x.AptDateTime.Date }).Select(x => x.OrderBy(y => y.AptDateTime).First()));
			return listAutoCommObjReturn;
		}
	}
}

