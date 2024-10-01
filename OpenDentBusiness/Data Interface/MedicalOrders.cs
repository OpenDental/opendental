using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;

namespace OpenDentBusiness{
	///<summary></summary>
	public class MedicalOrders{
		///<summary></summary>
		public static DataTable GetOrderTable(long patNum,bool includeDiscontinued){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum,includeDiscontinued);
			}
			DataTable table=new DataTable("orders");
			DataRow dataRow;
			table.Columns.Add("date");
			table.Columns.Add("DateTime",typeof(DateTime));
			table.Columns.Add("description");
			table.Columns.Add("MedicalOrderNum");
			table.Columns.Add("MedicationPatNum");
			table.Columns.Add("prov");
			table.Columns.Add("status");
			table.Columns.Add("type");
			List<DataRow> listDataRows=new List<DataRow>();
			string command="SELECT DateTimeOrder,Description,IsDiscontinued,MedicalOrderNum,MedOrderType,ProvNum "
				+"FROM medicalorder WHERE PatNum = "+POut.Long(patNum);
			if(!includeDiscontinued) {//only include current orders
				command+=" AND IsDiscontinued=0";//false
			}
			DataTable tableRaw=Db.GetTable(command);
			for(int i=0;i<tableRaw.Rows.Count;i++) {
				dataRow=table.NewRow();
				DateTime dateT=PIn.DateT(tableRaw.Rows[i]["DateTimeOrder"].ToString());
				MedicalOrderType medicalOrderType=(MedicalOrderType)PIn.Int(tableRaw.Rows[i]["MedOrderType"].ToString());
				long medicalOrderNum=PIn.Long(tableRaw.Rows[i]["MedicalOrderNum"].ToString());
				dataRow["DateTime"]=dateT;
				dataRow["date"]=dateT.ToShortDateString();
				dataRow["description"]=PIn.String(tableRaw.Rows[i]["Description"].ToString());
				if(medicalOrderType==MedicalOrderType.Laboratory) {
					List<LabPanel> listLabPanelsForOrder=LabPanels.GetPanelsForOrder(medicalOrderNum);
					for(int p=0;p<listLabPanelsForOrder.Count;p++){
						dataRow["description"]+="\r\n     ";//new row for each panel
						List<LabResult> listLabResults=LabResults.GetForPanel(listLabPanelsForOrder[p].LabPanelNum);
						if(listLabResults.Count>0){
							dataRow["description"]+=listLabResults[0].DateTimeTest.ToShortDateString()+" - ";
						}
						dataRow["description"]+=listLabPanelsForOrder[p].ServiceName;
					}
				}
				dataRow["MedicalOrderNum"]=medicalOrderNum.ToString();
				dataRow["MedicationPatNum"]="0";
				dataRow["prov"]=Providers.GetAbbr(PIn.Long(tableRaw.Rows[i]["ProvNum"].ToString()));
				bool isDiscontinued=PIn.Bool(tableRaw.Rows[i]["IsDiscontinued"].ToString());
				if(isDiscontinued) {
					dataRow["status"]="Discontinued";
				}
				else {
					dataRow["status"]="Active";
				}
				dataRow["type"]=medicalOrderType.ToString();
				listDataRows.Add(dataRow);
			}
			//Medications are deprecated for 2014 edition
			//MedicationPats
			//command="SELECT DateStart,DateStop,MedicationPatNum,CASE WHEN medication.MedName IS NULL THEN medicationpat.MedDescript ELSE medication.MedName END MedName,PatNote,ProvNum "
			//	+"FROM medicationpat "
			//	+"LEFT OUTER JOIN medication ON medication.MedicationNum=medicationpat.MedicationNum "
			//	+"WHERE PatNum = "+POut.Long(patNum);
			//if(!includeDiscontinued) {//exclude invalid orders
			//	command+=" AND DateStart > "+POut.Date(new DateTime(1880,1,1))+" AND PatNote !='' "
			//		+"AND (DateStop < "+POut.Date(new DateTime(1880,1,1))+" "//no date stop
			//		+"OR DateStop >= "+POut.Date(DateTime.Today)+")";//date stop hasn't happened yet
			//}
			//DataTable rawMed=Db.GetTable(command);
			//DateTime dateStop;
			//for(int i=0;i<rawMed.Rows.Count;i++) {
			//	row=table.NewRow();
			//	dateT=PIn.DateT(rawMed.Rows[i]["DateStart"].ToString());
			//	row["DateTime"]=dateT;
			//	if(dateT.Year<1880) {
			//		row["date"]="";
			//	}
			//	else {
			//		row["date"]=dateT.ToShortDateString();
			//	}
			//	row["description"]=PIn.String(rawMed.Rows[i]["MedName"].ToString())+", "
			//		+PIn.String(rawMed.Rows[i]["PatNote"].ToString());
			//	row["MedicalOrderNum"]="0";
			//	row["MedicationPatNum"]=rawMed.Rows[i]["MedicationPatNum"].ToString();
			//	row["prov"]=Providers.GetAbbr(PIn.Long(rawMed.Rows[i]["ProvNum"].ToString()));
			//	dateStop=PIn.Date(rawMed.Rows[i]["DateStop"].ToString());
			//	if(dateStop.Year<1880 || dateStop>DateTime.Today) {//not stopped or in the future
			//		row["status"]="Active";
			//	}
			//	else {
			//		row["status"]="Discontinued";
			//	}
			//	row["type"]="Medication";
			//	rows.Add(row);
			//}
			//Sorting-----------------------------------------------------------------------------------------
			listDataRows=listDataRows.OrderBy(x => (DateTime)x["DateTime"]).ToList();
			for(int i=0;i<listDataRows.Count;i++) {
				table.Rows.Add(listDataRows[i]);
			}
			return table;
		}

		/*
		///<summary></summary>
		public static int GetCountMedical(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetInt(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT COUNT(*) FROM medicalorder WHERE MedOrderType="+POut.Int((int)MedicalOrderType.Medication)+" "
				+"AND PatNUm="+POut.Long(patNum);
			return PIn.Int(Db.GetCount(command));
		}*/

		///<summary></summary>
		public static List<MedicalOrder> GetAllLabs(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<MedicalOrder>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM medicalorder WHERE MedOrderType="+POut.Int((int)MedicalOrderType.Laboratory)+" "
				+"AND PatNum="+POut.Long(patNum);
			//NOT EXISTS(SELECT * FROM labpanel WHERE labpanel.MedicalOrderNum=medicalorder.MedicalOrderNum)";
			return Crud.MedicalOrderCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<MedicalOrder> GetLabsByDate(long patNum,DateTime dateStart,DateTime dateStop) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<MedicalOrder>>(MethodBase.GetCurrentMethod(),patNum,dateStart,dateStop);
			}
			string command="SELECT * FROM medicalorder WHERE MedOrderType="+POut.Int((int)MedicalOrderType.Laboratory)+" "
				+"AND PatNum="+POut.Long(patNum)+" "
				+"AND DATE(DateTimeOrder) >= "+POut.Date(dateStart)+" "
				+"AND DATE(DateTimeOrder) <= "+POut.Date(dateStop);
			//NOT EXISTS(SELECT * FROM labpanel WHERE labpanel.MedicalOrderNum=medicalorder.MedicalOrderNum)";
			return Crud.MedicalOrderCrud.SelectMany(command);
		}

		///<summary></summary>
		public static bool LabHasResultsAttached(long medicalOrderNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),medicalOrderNum);
			}
			string command= "SELECT COUNT(*) FROM labpanel WHERE MedicalOrderNum = "+POut.Long(medicalOrderNum);
			if(Db.GetCount(command)=="0") {
				return false;
			}
			else {
				return true;
			}
		}

		///<summary>Gets one MedicalOrder from the db.</summary>
		public static MedicalOrder GetOne(long medicalOrderNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<MedicalOrder>(MethodBase.GetCurrentMethod(),medicalOrderNum);
			}
			return Crud.MedicalOrderCrud.SelectOne(medicalOrderNum);
		}

		///<summary></summary>
		public static long Insert(MedicalOrder medicalOrder){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				medicalOrder.MedicalOrderNum=Meth.GetLong(MethodBase.GetCurrentMethod(),medicalOrder);
				return medicalOrder.MedicalOrderNum;
			}
			return Crud.MedicalOrderCrud.Insert(medicalOrder);
		}

		///<summary></summary>
		public static void Update(MedicalOrder medicalOrder){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),medicalOrder);
				return;
			}
			Crud.MedicalOrderCrud.Update(medicalOrder);
		}

		///<summary></summary>
		public static void Delete(long medicalOrderNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),medicalOrderNum);
				return;
			}
			string command;
			//validation
			command="SELECT COUNT(*) FROM labpanel WHERE MedicalOrderNum="+POut.Long(medicalOrderNum);
			if(Db.GetCount(command)!="0") {
				throw new ApplicationException(Lans.g("MedicalOrders","Not allowed to delete a lab order that has attached lab panels."));
			}
			//end of validation
			command = "DELETE FROM medicalorder WHERE MedicalOrderNum = "+POut.Long(medicalOrderNum);
			Db.NonQ(command);
		}
	}
}