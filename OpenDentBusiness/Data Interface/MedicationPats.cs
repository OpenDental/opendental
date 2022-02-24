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
		public static void Update(MedicationPat Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			Crud.MedicationPatCrud.Update(Cur);
		}

		///<summary></summary>
		public static long Insert(MedicationPat Cur) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Cur.MedicationPatNum=Meth.GetLong(MethodBase.GetCurrentMethod(),Cur);
				return Cur.MedicationPatNum;
			}
			return Crud.MedicationPatCrud.Insert(Cur);
		}

		///<summary>For CPOE.  Used for both manual rx and eRx through NewCrop.  Creates or updates a medical order using the given prescription information.
		///Since rxCui is not part of the prescription, it must be passed in as a separate parameter.
		///If isProvOrder is true, then the medical order provNum will be set to the prescription provNum.  If isProvOrder is false, then the medical order provNum will be set to 0.
		///The MedDescript and ErxGuid will always be copied from the prescription to the medical order and the medical order MedicationNum will be set to 0.
		///This method return the medOrderNum for the new/updated medicationPat. Unlike most medical orders this does not create an entry in the medical order table.</summary>
		public static long InsertOrUpdateMedOrderForRx(RxPat rxPat,long rxCui,bool isProvOrder,bool hasRx=true) {
			long medOrderNum;
			MedicationPat medOrderOld=null;
			if(!string.IsNullOrWhiteSpace(rxPat.ErxGuid)) {//This check prevents an extra db call when making a new prescription manually inside OD.
				medOrderOld=MedicationPats.GetMedicationOrderByErxIdAndPat(rxPat.ErxGuid,rxPat.PatNum);
			}
			MedicationPat medOrder=null;//The medication order corresponding to the prescription.
			if(medOrderOld==null) {
				medOrder=new MedicationPat();
			}
			else {
				medOrder=medOrderOld.Clone();//Maintain primary key and medicationnum for the update below.
			}
			if(hasRx) {//If there is no prescription attached, don't give a start/stop date.  We can't simply check rxPat==null because we needed the info from rxPat elsewhere.
				medOrder.DateStart=rxPat.RxDate;//Only if actually has Rx
				int numDays=PrefC.GetInt(PrefName.MedDefaultStopDays);
				if(numDays!=0) {
					medOrder.DateStop=rxPat.RxDate.AddDays(numDays);//Only if actually has Rx
				}
			}
			medOrder.MedDescript=rxPat.Drug;
			medOrder.RxCui=rxCui;
			if(rxCui!=0) {
				//The customer may not have a medication entered for this RxCui the first few times they get this particular medication back from eRx.
				//Once the customer adds the medication to their medication list, then we can automatically attach the order to the medication.
				//The reason we decided not to automatically create the medication if one does not already exist is because we do not want to
				//accidentally bloat the medication list, if for example, the user has the medication entered but has not set the RxCui on it yet.
				List <Medication> listMeds=Medications.GetAllMedsByRxCui(rxCui);
				if(listMeds.Count > 0) {
					medOrder.MedicationNum=listMeds[0].MedicationNum;
				}
			}
			medOrder.ErxGuid=rxPat.ErxGuid;
			medOrder.PatNote=rxPat.Sig;
			medOrder.PatNum=rxPat.PatNum;
			if(isProvOrder) {
				medOrder.ProvNum=rxPat.ProvNum;
				medOrder.IsCpoe=true;
			}
			if(medOrderOld==null) {
				medOrder.IsNew=true;//Might not be necessary, but does not hurt.
				medOrder.MedicationPatNum=MedicationPats.Insert(medOrder);
				//If the ErxGuid has not been set, and it is a new medication, set the ErxGuid so that the medication can be sent to DoseSpot
				if(Erx.IsManualRx(rxPat.ErxGuid)) {
					try {
						int medPatNumAsInt=(int)medOrder.MedicationPatNum;
						rxPat.ErxGuid=Erx.UnsentPrefix+medPatNumAsInt;
						medOrder.ErxGuid=rxPat.ErxGuid;
						RxPats.Update(rxPat);
						MedicationPats.Update(medOrder);
					}
					catch(Exception ex) {
						//If we cannot downgrade a long to an int for the ErxGuid we can simply ignore trying to update the rxPat.
						//This is because this medication would never be sent to eRx because we would attempt this downgrade again in the exact same manner.
						ex.DoNothing();
					}
				}
			}
			else {//The medication order was already in our database. Update it.
				medOrder.MedicationPatNum=medOrderOld.MedicationPatNum;
				MedicationPats.Update(medOrder);
			}
			return medOrder.MedicationPatNum;
		}

		///<summary></summary>
		public static void Delete(MedicationPat Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			string command = "DELETE from medicationpat WHERE medicationpatNum = '"
				+Cur.MedicationPatNum.ToString()+"'";
			Db.NonQ(command);
		}

		public static List<long> GetChangedSinceMedicationPatNums(DateTime changedSince,List<long> eligibleForUploadPatNumList) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),changedSince,eligibleForUploadPatNumList);
			}
			string strEligibleForUploadPatNums="";
			DataTable table;
			if(eligibleForUploadPatNumList.Count>0) {
				for(int i=0;i<eligibleForUploadPatNumList.Count;i++) {
					if(i>0) {
						strEligibleForUploadPatNums+="OR ";
					}
					strEligibleForUploadPatNums+="PatNum='"+eligibleForUploadPatNumList[i].ToString()+"' ";
				}
				string command="SELECT MedicationPatNum FROM medicationpat WHERE DateTStamp > "+POut.DateT(changedSince)+" AND ("+strEligibleForUploadPatNums+")";
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			List<long> medicationpatnums = new List<long>(table.Rows.Count);
			for(int i=0;i<table.Rows.Count;i++) {
				medicationpatnums.Add(PIn.Long(table.Rows[i]["MedicationPatNum"].ToString()));
			}
			return medicationpatnums;
		}

		///<summary>Used along with GetChangedSinceMedicationPatNums</summary>
		public static List<MedicationPat> GetMultMedicationPats(List<long> medicationPatNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MedicationPat>>(MethodBase.GetCurrentMethod(),medicationPatNums);
			}
			string strMedicationPatNums="";
			DataTable table;
			if(medicationPatNums.Count>0) {
				for(int i=0;i<medicationPatNums.Count;i++) {
					if(i>0) {
						strMedicationPatNums+="OR ";
					}
					strMedicationPatNums+="MedicationPatNum='"+medicationPatNums[i].ToString()+"' ";
				}
				string command="SELECT * FROM medicationpat WHERE "+strMedicationPatNums;
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			MedicationPat[] multMedicationPats=Crud.MedicationPatCrud.TableToList(table).ToArray();
			List<MedicationPat> medicationPatList=new List<MedicationPat>(multMedicationPats);
			return medicationPatList;
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
			List<MedicationPat> medicationOrderNewCrop=Crud.MedicationPatCrud.SelectMany(command);
			if(medicationOrderNewCrop.Count==0) {
				return null;
			}
			return medicationOrderNewCrop[0];
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










