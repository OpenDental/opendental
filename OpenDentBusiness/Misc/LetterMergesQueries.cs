using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	public class LetterMergesQueries {

		///<summary>Throws exceptions.</summary>
		public static DataTable GetLetterMergeInfo(Patient PatCur,LetterMerge letter) {
			//Throw explicit arguement exceptions so that we can hopefully get more information as to what is actually failing for our users.
			if(PatCur==null) {
				throw new ArgumentException("Invalid patient","PatCur");
			}
			if(letter==null) {
				throw new ArgumentException("Invalid letter","letter");
			}
			if(letter.Fields==null) {
				throw new ArgumentException("Invalid letter fields","letter.Fields");
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),PatCur,letter);
			}
			//jsparks- This is messy and prone to bugs.  It needs to be reworked to work just like
			//in SheetFiller.FillFieldsInStaticText.  Just grab a bunch of separate objects
			//instead of one result row.
			string command;
			//We need a very small table that tells us which tp is the most recent.
			//command="DROP TABLE IF EXISTS temptp;";
			//Db.NonQ(command);
			//command=@"CREATE TABLE temptp(
			//	DateTP date NOT NULL default '0001-01-01')";
			//Db.NonQ(command);
			//command+=@"CREATE TABLE temptp
			//	SELECT MAX(treatplan.DateTP) DateTP
			//	FROM treatplan
			//	WHERE PatNum="+POut.PInt(PatCur.PatNum)+";";
			//Db.NonQ(command);
			command="SET @maxTpDate=(SELECT MAX(treatplan.DateTP) FROM treatplan WHERE PatNum="+POut.Long(PatCur.PatNum)+");";
			command+="SELECT ";
			for(int i=0;i<letter.Fields.Count;i++) {
				if(i>0) {
					command+=",";
				}
				if(letter.Fields[i]=="NextAptNum") {
					command+="MAX(plannedappt.AptNum) NextAptNum";
				}
					//other:
				else if(letter.Fields[i]=="TPResponsPartyNameFL") {
					command+=DbHelper.Concat("MAX(patResp.FName)","' '","MAX(patResp.LName)")+" TPResponsPartyNameFL";
				} 
				else if(letter.Fields[i]=="TPResponsPartyAddress") {
					command+="MAX(patResp.Address) TPResponsPartyAddress";
				} 
				else if(letter.Fields[i]=="TPResponsPartyCityStZip") {
					command+=DbHelper.Concat("MAX(patResp.City)","', '","MAX(patResp.State)","' '","MAX(patResp.Zip)")+" TPResponsPartyCityStZip";
				} 
				else if(letter.Fields[i]=="SiteDescription") {
					command+="MAX(site.Description) SiteDescription";
				} 
				else if(letter.Fields[i]=="DateOfLastSavedTP") {
					command+=DbHelper.DtimeToDate("MAX(treatplan.DateTP)")+" DateOfLastSavedTP";
				} 
				else if(letter.Fields[i]=="DateRecallDue") {
					command+="MAX(recall.DateDue)  DateRecallDue";
				} 
				else if(letter.Fields[i]=="CarrierName") {
					command+="MAX(CarrierName) CarrierName";
				} 
				else if(letter.Fields[i]=="CarrierAddress") {
					command+="MAX(carrier.Address) CarrierAddress";
				}
				else if(letter.Fields[i]=="CarrierAddress2") {
					command+="MAX(carrier.Address2) CarrierAddress2";
				}
				else if(letter.Fields[i]=="CarrierCityStZip") {
					command+=DbHelper.Concat("MAX(carrier.City)","', '","MAX(carrier.State)","' '","MAX(carrier.Zip)")+" CarrierCityStZip";
				} 
				else if(letter.Fields[i]=="SubscriberNameFL") {
					command+=DbHelper.Concat("MAX(patSubsc.FName)","' '","MAX(patSubsc.LName)")+" SubscriberNameFL";
				} 
				else if(letter.Fields[i]=="SubscriberID") {
					command+="MAX(inssub.SubscriberID) SubscriberID";
				} 
				else if(letter.Fields[i]=="NextSchedAppt") {
					command+="MIN(appointment.AptDateTime) NextSchedAppt";
				}
				else if(letter.Fields[i]=="Age") {
					command+="MAX(patient.Birthdate) BirthdateForAge";
				}
				else if(letter.Fields[i]=="Guarantor") {
					command+=DbHelper.Concat("MAX(patGuar.FName)","' '","MAX(patGuar.LName)")+" Guarantor";
				}
				else if(letter.Fields[i]=="GradeSchool"){
					command+="MAX(site.Description) GradeSchool";
				}
				else if(letter.Fields[i].StartsWith("referral.")) {
					command+="MAX(referral."+letter.Fields[i].Substring(9)+") "+letter.Fields[i].Substring(9);
				}
				else if(letter.Fields[i]=="Race") {//This is to accomodate the deprecated patient.Race column that no longer exists
					command+="'"+POut.String(string.Join(",",PatientRaces.GetForPatient(PatCur.PatNum).Select(x => x.Description)))+"'"+" Race";
				}
				else {
					command+="MAX(patient."+letter.Fields[i]+") "+letter.Fields[i];
				}
			}
			command+=" FROM patient "
				+"LEFT JOIN refattach ON patient.PatNum=refattach.PatNum AND refattach.RefType="+POut.Int((int)ReferralType.RefFrom)+" "
				+"LEFT JOIN referral ON refattach.ReferralNum=referral.ReferralNum "
				+"LEFT JOIN plannedappt ON plannedappt.PatNum=patient.PatNum AND plannedappt.ItemOrder=1 "
				+"LEFT JOIN site ON patient.SiteNum=site.SiteNum "
				+"LEFT JOIN treatplan ON patient.PatNum=treatplan.PatNum AND DateTP=@maxTpDate "
				+"LEFT JOIN patient patResp ON treatplan.ResponsParty=patResp.PatNum "
				+"LEFT JOIN recall ON recall.PatNum=patient.PatNum "
					+"AND (recall.RecallTypeNum="+POut.Long(PrefC.GetLong(PrefName.RecallTypeSpecialProphy))
					+" OR recall.RecallTypeNum="+POut.Long(PrefC.GetLong(PrefName.RecallTypeSpecialPerio))+") "
				+"LEFT JOIN patplan ON patplan.PatNum=patient.PatNum AND Ordinal=1 "
				+"LEFT JOIN inssub ON patplan.InsSubNum=inssub.InsSubNum "
				+"LEFT JOIN insplan ON inssub.PlanNum=insplan.PlanNum "
				+"LEFT JOIN carrier ON carrier.CarrierNum=insplan.CarrierNum "
				+"LEFT JOIN patient patSubsc ON patSubsc.PatNum=inssub.Subscriber "
				+"LEFT JOIN appointment ON appointment.PatNum=patient.PatNum "
					+"AND AptStatus="+POut.Long((int)ApptStatus.Scheduled)+" "
					+"AND AptDateTime > "+DbHelper.Now()+" "
				+"LEFT JOIN patient patGuar ON patGuar.PatNum=patient.Guarantor "
				+"WHERE patient.PatNum="+POut.Long(PatCur.PatNum)
				+" GROUP BY patient.PatNum "
				+"ORDER BY refattach.ItemOrder";
			return Db.GetTable(command);
		}

	}
}
