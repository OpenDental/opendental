using CodeBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class MedicationPats{
		///<summary>Normally, includeDiscontinued is false.  User needs to check a box to include discontinued.</summary>
		public static List<MedicationPat> Refresh(long patNum,bool includeDiscontinued) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MedicationPat>>(MethodBase.GetCurrentMethod(),patNum,includeDiscontinued);
			}
			string command ="SELECT * FROM medicationpat WHERE PatNum = "+POut.Long(patNum);
			if(includeDiscontinued) {//this only happens when user checks box to show discontinued or for MU.
				//no restriction on DateStop
			}
			else {//exclude discontinued.  This is the default.
				command+=" AND (DateStop < "+POut.Date(new DateTime(1880,1,1))//include all the meds that are not discontinued.
					+" OR DateStop >= ";
				command+=DbHelper.Curdate()+")";//Show medications that are today or a future stopdate - they are not yet discontinued.
			}
			return Crud.MedicationPatCrud.SelectMany(command);
		}

		///<summary>Gets all active medications for the patient.  Exactly like Refresh() except this does not return medications when DateStop has today's date.  Currently only called from FormReconcileMedication.</summary>
		public static List<MedicationPat> GetMedPatsForReconcile(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MedicationPat>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command ="SELECT * FROM medicationpat WHERE PatNum = "+POut.Long(patNum)
					+" AND (DateStop < "+POut.Date(new DateTime(1880,1,1))//include all the meds that are not discontinued.
					+" OR DateStop > CURDATE())";//Show medications that are a future stopdate.
			return Crud.MedicationPatCrud.SelectMany(command);
		}

		///<summary></summary>
		public static MedicationPat GetOne(long medicationPatNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<MedicationPat>(MethodBase.GetCurrentMethod(),medicationPatNum);
			}
			string command ="SELECT * FROM medicationpat WHERE MedicationPatNum = "+POut.Long(medicationPatNum);
			return Crud.MedicationPatCrud.SelectOne(command);
		}

		///<summary></summary>
		public static void Update(MedicationPat medicationPat){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),medicationPat);
				return;
			}
			Crud.MedicationPatCrud.Update(medicationPat);
		}

		///<summary></summary>
		public static long Insert(MedicationPat medicationPat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				medicationPat.MedicationPatNum=Meth.GetLong(MethodBase.GetCurrentMethod(),medicationPat);
				return medicationPat.MedicationPatNum;
			}
			return Crud.MedicationPatCrud.Insert(medicationPat);
		}

		///<summary>For CPOE.  Used for both manual rx and eRx through NewCrop.  Creates or updates a medical order using the given prescription information.
		///Since rxCui is not part of the prescription, it must be passed in as a separate parameter.
		///If isProvOrder is true, then the medical order provNum will be set to the prescription provNum.  If isProvOrder is false, then the medical order provNum will be set to 0.
		///The MedDescript and ErxGuid will always be copied from the prescription to the medical order and the medical order MedicationNum will be set to 0.
		///This method return the medOrderNum for the new/updated medicationPat. Unlike most medical orders this does not create an entry in the medical order table.</summary>
		public static long InsertOrUpdateMedOrderForRx(RxPat rxPat,long rxCui,bool isProvOrder,bool hasRx=true) {
			long medOrderNum;
			MedicationPat medicationPatOld=null;
			if(!string.IsNullOrWhiteSpace(rxPat.ErxGuid)) {//This check prevents an extra db call when making a new prescription manually inside OD.
				medicationPatOld=MedicationPats.GetMedicationOrderByErxIdAndPat(rxPat.ErxGuid,rxPat.PatNum);
			}
			MedicationPat medicationPat=null;//The medication order corresponding to the prescription.
			if(medicationPatOld==null) {
				medicationPat=new MedicationPat();
			}
			else {
				medicationPat=medicationPatOld.Clone();//Maintain primary key and medicationnum for the update below.
			}
			if(hasRx) {//If there is no prescription attached, don't give a start/stop date.  We can't simply check rxPat==null because we needed the info from rxPat elsewhere.
				medicationPat.DateStart=rxPat.RxDate;//Only if actually has Rx
				int numDays=PrefC.GetInt(PrefName.MedDefaultStopDays);
				if(numDays!=0) {
					medicationPat.DateStop=rxPat.RxDate.AddDays(numDays);//Only if actually has Rx
				}
			}
			medicationPat.MedDescript=rxPat.Drug;
			medicationPat.RxCui=rxCui;
			if(rxCui!=0) {
				//The customer may not have a medication entered for this RxCui the first few times they get this particular medication back from eRx.
				//Once the customer adds the medication to their medication list, then we can automatically attach the order to the medication.
				//The reason we decided not to automatically create the medication if one does not already exist is because we do not want to
				//accidentally bloat the medication list, if for example, the user has the medication entered but has not set the RxCui on it yet.
				List <Medication> listMedications=Medications.GetAllMedsByRxCui(rxCui);
				if(listMedications.Count > 0) {
					medicationPat.MedicationNum=listMedications[0].MedicationNum;
				}
			}
			medicationPat.ErxGuid=rxPat.ErxGuid;
			medicationPat.PatNote=rxPat.Sig;
			medicationPat.PatNum=rxPat.PatNum;
			if(isProvOrder) {
				medicationPat.ProvNum=rxPat.ProvNum;
				medicationPat.IsCpoe=true;
			}
			if(medicationPatOld==null) {
				medicationPat.IsNew=true;//Might not be necessary, but does not hurt.
				medicationPat.MedicationPatNum=MedicationPats.Insert(medicationPat);
				//If the ErxGuid has not been set, and it is a new medication, set the ErxGuid so that the medication can be sent to DoseSpot
				if(Erx.IsManualRx(rxPat.ErxGuid)) {
					try {
						int medPatNumAsInt=(int)medicationPat.MedicationPatNum;
						rxPat.ErxGuid=Erx.UnsentPrefix+medPatNumAsInt;
						medicationPat.ErxGuid=rxPat.ErxGuid;
						RxPats.Update(rxPat);
						MedicationPats.Update(medicationPat);
					}
					catch(Exception ex) {
						//If we cannot downgrade a long to an int for the ErxGuid we can simply ignore trying to update the rxPat.
						//This is because this medication would never be sent to eRx because we would attempt this downgrade again in the exact same manner.
						ex.DoNothing();
					}
				}
			}
			else {//The medication order was already in our database. Update it.
				medicationPat.MedicationPatNum=medicationPatOld.MedicationPatNum;
				MedicationPats.Update(medicationPat);
			}
			return medicationPat.MedicationPatNum;
		}

		///<summary></summary>
		public static void Delete(MedicationPat medicationPat){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),medicationPat);
				return;
			}
			string command = "DELETE from medicationpat WHERE medicationpatNum = '"
				+medicationPat.MedicationPatNum.ToString()+"'";
			Db.NonQ(command);
		}

		public static List<long> GetChangedSinceMedicationPatNums(DateTime dateTimeChangedSince,List<long> listPatNumsEligibleForUpload) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),dateTimeChangedSince,listPatNumsEligibleForUpload);
			}
			string strPatNumsEligibleForUpload="";
			DataTable tableMedicationPatNums;
			if(listPatNumsEligibleForUpload.Count>0) {
				for(int i=0;i<listPatNumsEligibleForUpload.Count;i++) {
					if(i>0) {
						strPatNumsEligibleForUpload+="OR ";
					}
					strPatNumsEligibleForUpload+="PatNum='"+listPatNumsEligibleForUpload[i].ToString()+"' ";
				}
				string command="SELECT MedicationPatNum FROM medicationpat WHERE DateTStamp > "+POut.DateT(dateTimeChangedSince)+" AND ("+strPatNumsEligibleForUpload+")";
				tableMedicationPatNums=Db.GetTable(command);
			}
			else {
				tableMedicationPatNums=new DataTable();
			}
			List<long> listMedicationPatNums = new List<long>(tableMedicationPatNums.Rows.Count);
			for(int i=0;i<tableMedicationPatNums.Rows.Count;i++) {
				listMedicationPatNums.Add(PIn.Long(tableMedicationPatNums.Rows[i]["MedicationPatNum"].ToString()));
			}
			return listMedicationPatNums;
		}

		///<summary>Used along with GetChangedSinceMedicationPatNums</summary>
		public static List<MedicationPat> GetMultMedicationPats(List<long> listMedicationPatNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MedicationPat>>(MethodBase.GetCurrentMethod(),listMedicationPatNums);
			}
			string strMedicationPatNums="";
			DataTable tableMedicationPats;
			if(listMedicationPatNums.Count>0) {
				for(int i=0;i<listMedicationPatNums.Count;i++) {
					if(i>0) {
						strMedicationPatNums+="OR ";
					}
					strMedicationPatNums+="MedicationPatNum='"+listMedicationPatNums[i].ToString()+"' ";
				}
				string command="SELECT * FROM medicationpat WHERE "+strMedicationPatNums;
				tableMedicationPats=Db.GetTable(command);
			}
			else {
				tableMedicationPats=new DataTable();
			}
			MedicationPat[] arrayMedicationPats=Crud.MedicationPatCrud.TableToList(tableMedicationPats).ToArray();
			List<MedicationPat> listMedicationPats=new List<MedicationPat>(arrayMedicationPats);
			return listMedicationPats;
		}

		///<summary>Get list of MedicationPats by MedicationNum for a particular patient.</summary>
		public static List<MedicationPat> GetMedicationPatsByMedicationNum(long medicationNum,long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MedicationPat>>(MethodBase.GetCurrentMethod(),medicationNum,patNum);
			}
			string command="SELECT * FROM medicationpat WHERE PatNum="+POut.Long(patNum)+" AND MedicationNum="+POut.Long(medicationNum);
			return Crud.MedicationPatCrud.SelectMany(command);
		}


		///<summary>Changes the value of the DateTStamp column to the current time stamp for all medicationpats of a patient</summary>
		public static void ResetTimeStamps(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			string command="UPDATE medicationpat SET DateTStamp = CURRENT_TIMESTAMP WHERE PatNum ="+POut.Long(patNum);
			Db.NonQ(command);
		}

		///<summary>Changes the value of the DateTStamp column to the current time stamp for all medicationpats of a patient that are the status specified.</summary>
		public static void ResetTimeStamps(long patNum,bool onlyActive) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum,onlyActive);
				return;
			}
			string command="UPDATE medicationpat SET DateTStamp = CURRENT_TIMESTAMP WHERE PatNum = "+POut.Long(patNum);
				if(onlyActive) {
					command+=" AND (DateStop > 1880 OR DateStop <= CURDATE())";
			}
			Db.NonQ(command);
		}

		///<summary>Retrieve one medicationpat for the passed in ErxGuid and patnum.  Used by NewCrop and DoseSpot.</summary>
		public static MedicationPat GetMedicationOrderByErxIdAndPat(string erxGuid,long patNum=0) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<MedicationPat>(MethodBase.GetCurrentMethod(),erxGuid,patNum);
			}
			string command="SELECT * FROM medicationpat WHERE ErxGuid='"+POut.String(erxGuid)+"'";
			if(patNum!=0) {
				command+=" AND PatNum="+POut.Long(patNum);
			}
			List<MedicationPat> listMedicationPats=Crud.MedicationPatCrud.SelectMany(command);
			if(listMedicationPats.Count==0) {
				return null;
			}
			return listMedicationPats[0];
		}

		///<summary>Used to synch medication.RxCui with medicationpat.RxCui.  Updates all medicationpat.RxCui to the given value for those medication pats linked to the given medication num.</summary>
		public static void UpdateRxCuiForMedication(long medicationNum,long rxCui) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),medicationNum,rxCui);
				return;
			}
			string command="UPDATE medicationpat SET RxCui="+POut.Long(rxCui)+" WHERE MedicationNum="+POut.Long(medicationNum);
			Db.NonQ(command);
		}

		public static bool IsMedActive(MedicationPat medicationPat) {
			if(medicationPat.DateStop.Year<1880 || medicationPat.DateStop>=DateTime.Today) {
				return true;
			}
			return false;
		}

		///<summary>Gets list of RxCui code strings for medications with RxCui in the supplied list ordered for patients in the last year.
		///"Ordered" is based on there being a DateStart.  Result list is grouped by RxCui.</summary>
		public static List<string> GetAllForRxCuis(List<string> listRxCuis) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),listRxCuis);
			}
			if(listRxCuis==null || listRxCuis.Count==0) {
				return new List<string>();
			}
			string command="SELECT RxCui FROM medicationpat WHERE RxCui IN("+string.Join(",",listRxCuis)+") "
				+"AND "+DbHelper.DtimeToDate("DateStart")+">="+POut.Date(MiscData.GetNowDateTime().AddYears(-1))+" "
				+"GROUP BY RxCui";
			return Db.GetListString(command);
		}

		public static List<MedicationPat> GetForRxCuis(List<string> listRxCuis) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MedicationPat>>(MethodBase.GetCurrentMethod(),listRxCuis);
			}
			if(listRxCuis==null || listRxCuis.Count==0) {
				return new List<MedicationPat>();
			}
			string command="SELECT * FROM medicationpat WHERE RxCui IN("+string.Join(",",listRxCuis)+")";
			return Crud.MedicationPatCrud.SelectMany(command);
		}

		///<summary>These can currently only exist if the MedicationPats were created when importing from NewCrop.</summary>
		public static List<MedicationPat> GetAllMissingMedications() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MedicationPat>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM medicationpat WHERE MedicationNum=0";
			return Crud.MedicationPatCrud.SelectMany(command);
		}

		public static void UpdateMedicationNumForMany(long medicationNum,List<long> listMedicationPatNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),medicationNum,listMedicationPatNums);
				return;
			}
			if(listMedicationPatNums==null || listMedicationPatNums.Count < 1) {
				return;
			}
			string command="UPDATE medicationpat SET MedicationNum="+POut.Long(medicationNum)+" "
				+"WHERE MedicationPatNum IN ("+string.Join(",",listMedicationPatNums.Select(x =>POut.Long(x)))+")";
			Db.NonQ(command);
		}
	}

	





}










