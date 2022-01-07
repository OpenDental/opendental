using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class MedLabFacilities{
		///<summary>Checks the database for a MedLabFacility with matching name, address, city, state, zip, phone, and director title/name.
		///If the facility doesn't exist, it's inserted.  Returns the MedLabFacilityNum for the facility inserted or found.
		///Doesn't need any indexes, this runs in under a second with 100k worst case scenario rows (identical data).</summary>
		public static long InsertIfNotInDb(MedLabFacility medLabFacility) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),medLabFacility);
			}
			string command="SELECT * FROM medlabfacility "
				+"WHERE FacilityName='"+POut.String(medLabFacility.FacilityName)+"' "
				+"AND Address='"+POut.String(medLabFacility.Address)+"' "
				+"AND City='"+POut.String(medLabFacility.City)+"' "
				+"AND State='"+POut.String(medLabFacility.State)+"' "
				+"AND Zip='"+POut.String(medLabFacility.Zip)+"' "
				+"AND Phone='"+POut.String(medLabFacility.Phone)+"' "
				+"AND DirectorTitle='"+POut.String(medLabFacility.DirectorTitle)+"' "
				+"AND DirectorLName='"+POut.String(medLabFacility.DirectorLName)+"' "
				+"AND DirectorFName='"+POut.String(medLabFacility.DirectorFName)+"'";
			MedLabFacility medLabFacilityDb=Crud.MedLabFacilityCrud.SelectOne(command);
			if(medLabFacilityDb==null) {
				return Crud.MedLabFacilityCrud.Insert(medLabFacility);
			}
			return medLabFacilityDb.MedLabFacilityNum;
		}

		///<summary>Gets one MedLabFacility from the db.</summary>
		public static MedLabFacility GetOne(long medLabFacilityNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<MedLabFacility>(MethodBase.GetCurrentMethod(),medLabFacilityNum);
			}
			return Crud.MedLabFacilityCrud.SelectOne(medLabFacilityNum);
		}

		///<summary>Returns a list of MedLabFacilityNums, the order in the list will be the facility ID on the report.  Basically a local re-numbering.
		///Each message has a facility or facilities with footnote IDs, e.g. 01, 02, etc.  The results each link to the facility that performed the test.
		///But if there are multiple messages for a test order, e.g. when there is a final result for a subset of the original test results,
		///the additional message may have a facility with footnote ID of 01 that is different than the original message facility with ID 01.
		///So each ID could link to multiple facilities.  We will re-number the facilities so that each will have a unique number for this report.</summary>
		public static List<MedLabFacility> GetFacilityList(List<MedLab> listMedLabs,out List<MedLabResult> listResults) {
			//No need to check RemotingRole; no call to db.
			listResults=listMedLabs.SelectMany(x => x.ListMedLabResults).ToList();
			for(int i=listResults.Count-1;i>-1;i--) {//loop through backward and only keep the most final/most recent result
				if(i==0) {
					break;
				}
				if(listResults[i].ObsID==listResults[i-1].ObsID && listResults[i].ObsIDSub==listResults[i-1].ObsIDSub) {
					listResults.RemoveAt(i);
				}
			}
			listResults.OrderBy(x => x.MedLabNum).ThenBy(x => x.MedLabResultNum);
			//listResults will now only contain the most recent or most final/corrected results, sorted by the order inserted in the db
			List<MedLabFacAttach> listFacAttaches=MedLabFacAttaches.GetAllForResults(listResults.Select(x => x.MedLabResultNum).Distinct().ToList());
			Dictionary<long,long> dictResultNumFacNum=listFacAttaches.ToDictionary(x => x.MedLabResultNum,x => x.MedLabFacilityNum);
			List<MedLabFacility> listFacilities=new List<MedLabFacility>();
			for(int i=0;i<listResults.Count;i++) {
				if(!dictResultNumFacNum.ContainsKey(listResults[i].MedLabResultNum)) {
					continue;
				}
				long facilityNumCur=dictResultNumFacNum[listResults[i].MedLabResultNum];
				if(!listFacilities.Exists(x => x.MedLabFacilityNum==facilityNumCur)) {
					listFacilities.Add(MedLabFacilities.GetOne(facilityNumCur));
				}
				listResults[i].FacilityID=(listFacilities.Select(x => x.MedLabFacilityNum).ToList().IndexOf(facilityNumCur)+1).ToString().PadLeft(2,'0');
			}
			return listFacilities;
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<MedLabFacility> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MedLabFacility>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM medlabfacility WHERE PatNum = "+POut.Long(patNum);
			return Crud.MedLabFacilityCrud.SelectMany(command);
		}
		
		///<summary></summary>
		public static long Insert(MedLabFacility medLabFacility){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				medLabFacility.MedLabFacilityNum=Meth.GetLong(MethodBase.GetCurrentMethod(),medLabFacility);
				return medLabFacility.MedLabFacilityNum;
			}
			return Crud.MedLabFacilityCrud.Insert(medLabFacility);
		}

		///<summary></summary>
		public static void Update(MedLabFacility medLabFacility){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),medLabFacility);
				return;
			}
			Crud.MedLabFacilityCrud.Update(medLabFacility);
		}

		///<summary></summary>
		public static void Delete(long medLabFacilityNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),medLabFacilityNum);
				return;
			}
			string command= "DELETE FROM medlabfacility WHERE MedLabFacilityNum = "+POut.Long(medLabFacilityNum);
			Db.NonQ(command);
		}
		*/
	}
}