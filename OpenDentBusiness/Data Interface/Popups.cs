using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Popups {
		///<summary>Gets all active popups that should be displayed for a single patient.</summary>
		public static List<Popup> GetForPatient(Patient pat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Popup>>(MethodBase.GetCurrentMethod(),pat);
			}
			if(pat==null || pat.PatNum==0) {
				return new List<Popup>();
			}
			List<long> listFamPatNums=Patients.GetAllFamilyPatNumsForGuars(new List<long>() { pat.Guarantor }).FindAll(x => x!=pat.PatNum);
			List<long> listSuperFamPatNums=Patients.GetAllFamilyPatNumsForSuperFam(new List<long>() { pat.SuperFamily }).FindAll(x => x!=pat.PatNum);
			string command=@"SELECT * FROM popup
				WHERE IsDisabled=0
				AND IsArchived=0
				AND (
					PatNum="+POut.Long(pat.PatNum);
			if(listFamPatNums.Count>0) {
				command+=@"
					OR (PatNum IN ("+string.Join(",",listFamPatNums)+@")
						AND PopupLevel="+POut.Int((int)EnumPopupLevel.Family)+")";
			}
			if(listSuperFamPatNums.Count>0) {
				command+=@"
					OR (PatNum IN ("+string.Join(",",listSuperFamPatNums)+@")
						AND PopupLevel="+POut.Int((int)EnumPopupLevel.SuperFamily)+")";
			}
			command+=@"
				)";
			return Crud.PopupCrud.SelectMany(command);
		}

		///<summary>Gets current and disabled popups for a single family.  If patient is part of a superfamily, it will get all popups for the entire superfamily.</summary>
		public static List<Popup> GetForFamily(Patient pat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Popup>>(MethodBase.GetCurrentMethod(),pat);
			}
			string command="SELECT * FROM popup "
				+"WHERE (PatNum IN (SELECT PatNum FROM patient "
				+"WHERE Guarantor = "+POut.Long(pat.Guarantor)+") ";
			if(pat.SuperFamily!=0) {//They are part of a super family.
				command+="OR PatNum IN (SELECT PatNum FROM patient "
					+"WHERE SuperFamily = "+POut.Long(pat.SuperFamily)+") ";
			}
			command+=") "
				+"AND IsArchived = 0 "
				+"ORDER BY PopupLevel DESC, PatNum";
			return Crud.PopupCrud.SelectMany(command);
		}

		///<summary>Gets the most recent deleted and disabled popups for a single family.  If patient is part of a superfamily, it will get all popups for the entire superfamily. </summary>
		public static List<Popup> GetDeletedForFamily(Patient pat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Popup>>(MethodBase.GetCurrentMethod(),pat);
			}
			string command="SELECT * FROM popup "
				+"WHERE PatNum IN (SELECT PatNum FROM patient "
				+"WHERE Guarantor = "+POut.Long(pat.Guarantor)+") ";
			if(pat.SuperFamily!=0) {//They are part of a super family.
				command+="OR PatNum IN (SELECT PatNum FROM patient "
					+"WHERE SuperFamily = "+POut.Long(pat.SuperFamily)+") ";
			}
			command+="AND PopupNumArchive = 0 "//The most recent pop up in the archives.
				+"ORDER BY PopupLevel DESC, PatNum";
			return Crud.PopupCrud.SelectMany(command);
		}

		///<summary>Gets all archived popups for a single popup.</summary>
		public static List<Popup> GetArchivesForPopup(long popupNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Popup>>(MethodBase.GetCurrentMethod(),popupNum);
			}
			string command="SELECT * FROM popup"
				+" WHERE PopupNumArchive = "+POut.Long(popupNum)
				+" ORDER BY DateTimeEntry";
			return Crud.PopupCrud.SelectMany(command);
		}

		///<summary>Gets the most recent date and time that the popup was last edited.  Returns min value if no archive was found.</summary>
		public static DateTime GetLastEditDateTimeForPopup(long popupNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<DateTime>(MethodBase.GetCurrentMethod(),popupNum);
			}
			string command="SELECT DateTimeEntry FROM popup"
				+" WHERE PopupNumArchive = "+POut.Long(popupNum)
				+" ORDER BY DateTimeEntry DESC"
				+" LIMIT 1";
			DataTable rawTable=Db.GetTable(command);
			if(rawTable.Rows.Count==0) {
				return DateTime.MinValue;
			}
			return PIn.DateT(rawTable.Rows[0]["DateTimeEntry"].ToString());
		}

		/// <summary>Copies all family level popups when a family member leaves a family. Copies from other family members to patient, and from patient to guarantor.</summary>
		public static void CopyForMovingFamilyMember(Patient pat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pat);
				return;
			}
			//Get a list of all popups for the family
			string command="SELECT * FROM popup "
				+"WHERE PopupLevel = "+POut.Int((int)EnumPopupLevel.Family)+" "
				+"AND PatNum IN (SELECT PatNum FROM patient WHERE Guarantor = "+POut.Long(pat.Guarantor)+")"
				+"AND PopupNumArchive = 0 ";
			List<Popup> FamilyPopups=Crud.PopupCrud.SelectMany(command);
			Popup popupCur;
			for(int i=0;i<FamilyPopups.Count;i++) {
				popupCur=FamilyPopups[i].Copy();
				if(popupCur.PatNum==pat.PatNum) {//if popup is on the patient who's leaving, copy to guarantor of old family.
					popupCur.PatNum=pat.Guarantor;
				}
				else {//if popup is on some other family member, then copy to this patient.
					popupCur.PatNum=pat.PatNum;
				}
				DateTime oldDate=popupCur.DateTimeEntry;
				long newPk=Popups.Insert(popupCur);//changes the PK
				EditPopupDate(oldDate,newPk);
				List<Popup> archivePopups=GetArchivesForPopup(FamilyPopups[i].PopupNum);
				Popup popupArchive;
				for(int j=0;j<archivePopups.Count;j++) {
					popupArchive=archivePopups[j].Copy();
					if(popupArchive.PatNum==pat.PatNum) {//if popup is on the patient who's leaving, copy to guarantor of old family.
						popupArchive.PatNum=pat.Guarantor;
					}
					else {//if popup is on some other family member, then copy to this patient.
						popupArchive.PatNum=pat.PatNum;
					}
					popupArchive.PopupNumArchive=newPk;
					DateTime oldArchiveDate=popupArchive.DateTimeEntry;
					long newArchivePk=Popups.Insert(popupArchive);//changes the PK
					EditPopupDate(oldArchiveDate,newArchivePk);
				}
			}
		}
		
		/// <summary>When a patient leaves a superfamily, this copies the superfamily level popups to be in both places. Takes pat leaving, and new superfamily. If newSuperFamily is 0, superfamily popups will not be copied from the old superfamily.</summary>
		public static void CopyForMovingSuperFamily(Patient pat,long newSuperFamily) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pat,newSuperFamily);
				return;
			}
			//Get a list of all popups for the super family
			string command="SELECT * FROM popup "
				+"WHERE PopupLevel = "+POut.Int((int)EnumPopupLevel.SuperFamily)+" "
				+"AND PatNum IN (SELECT PatNum FROM patient WHERE SuperFamily = "+POut.Long(pat.SuperFamily)+")"
				+"AND PopupNumArchive = 0 ";
			//This includes all the archived ones as well
			List<Popup> SuperFamilyPopups=Crud.PopupCrud.SelectMany(command);
			Popup popupCur;
			for(int i=0;i<SuperFamilyPopups.Count;i++) {
				popupCur=SuperFamilyPopups[i].Copy();
				if(popupCur.PatNum==pat.PatNum) {//if popup is on the patient who's leaving, copy to superfamily head of old superfamily.
					popupCur.PatNum=pat.SuperFamily;
					if(newSuperFamily==0) {//If they are not going to a superfamily, set popup to family level
						string commandUpdateFam="UPDATE popup "
								+"SET PopupLevel = "+POut.Int((int)EnumPopupLevel.Family)+" "
								+"WHERE PopupNum = "+POut.Long(popupCur.PopupNum);
						Db.NonQ(commandUpdateFam);
					}
				}
				else {//if popup is on some other super family member, then copy to this patient.
					popupCur.PatNum=pat.PatNum;
					if(newSuperFamily==0) {//If they are not going to a superfamily, set popup to family level
						popupCur.PopupLevel=EnumPopupLevel.Family;
					}
				}
				DateTime oldDate=popupCur.DateTimeEntry;
				long newPk=Popups.Insert(popupCur);//changes the PK
				//Update the DateTimeEntry on the copy to correctly reflect when the original popup was created.
				EditPopupDate(oldDate,newPk);
				//Now we need to copy all of the archives of the original popup to point to the copy.
				List<Popup> archivePopups=GetArchivesForPopup(SuperFamilyPopups[i].PopupNum);
				Popup popupArchive;
				for(int j=0;j<archivePopups.Count;j++) {
					popupArchive=archivePopups[j].Copy();
					popupArchive.PopupNumArchive=newPk;
					DateTime oldArchiveDate=popupArchive.DateTimeEntry;
					long newArchivePk=Popups.Insert(popupArchive);//changes the PK
					EditPopupDate(oldArchiveDate,newArchivePk);
				}
			}
		}

		/// <summary>Moves all family and superfamily level popups for a patient being deleted so that those popups stay in the family/superfamily.</summary>
		public static void MoveForDeletePat(Patient pat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pat);
				return;
			}
			string command="UPDATE popup ";
			if(pat.PatNum==pat.Guarantor) {//When deleting the guarantor, move all superfamily popups to the superfamily head
				command+="SET PatNum = "+POut.Long(pat.SuperFamily)+" "
					+"WHERE PopupLevel = "+POut.Int((int)EnumPopupLevel.SuperFamily)+" "
					+"AND PatNum = "+POut.Long(pat.PatNum);
			}
			else{//Move all family/superfamily popups to the guarantor
				command+="SET PatNum = "+POut.Long(pat.Guarantor)+" "
					+"WHERE (PopupLevel = "+POut.Int((int)EnumPopupLevel.Family)+" "
					+"OR PopupLevel = "+POut.Int((int)EnumPopupLevel.SuperFamily)+") "
					+"AND PatNum = "+POut.Long(pat.PatNum);
			}
			Db.NonQ(command);
		}

		/// <summary>Popup dates are not normally changed.  This only occurs when creating exact copies of popups and their archives when moving a patient from a family or superfamily.</summary>
		private static void EditPopupDate(DateTime oldDate,long newPk) {
			//No need to check RemotingRole; private static.
			string commandUpdate="UPDATE popup "
					+"SET DateTimeEntry = "+POut.DateT(oldDate)+" "
					+"WHERE PopupNum = "+POut.Long(newPk);
			Db.NonQ(commandUpdate);
		}

		/// <summary>Brings all superfamily level popups for a superfamily being disbanded to the family level.</summary>
		public static void RemoveForDisbandingSuperFamily(Patient pat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pat);
				return;
			}
			string command="UPDATE popup "
					+"SET PopupLevel = "+POut.Int((int)EnumPopupLevel.Family)+" "
					+"WHERE PopupLevel = "+POut.Int((int)EnumPopupLevel.SuperFamily)+" "
					+"AND PatNum IN (SELECT PatNum FROM patient WHERE SuperFamily="+POut.Long(pat.SuperFamily)+") "
					+"AND PopupNumArchive = 0";
			Db.NonQ(command);
		}

		///<summary></summary>
		public static long Insert(Popup popup) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				popup.PopupNum=Meth.GetLong(MethodBase.GetCurrentMethod(),popup);
				return popup.PopupNum;
			}
			return Crud.PopupCrud.Insert(popup);
		}

		///<summary>Create an archive of the pop up before updating.</summary>
		public static void Update(Popup popup) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),popup);
				return;
			}
			Crud.PopupCrud.Update(popup);
		}

		///<summary>Only called when moving popups for a patient that is leaving a superfamily but not going to another superfamily.</summary>
		public static void DeleteObject(Popup popup){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),popup);
				return;
			}
			Crud.PopupCrud.Delete(popup.PopupNum);
		}
	}

	


	


}









