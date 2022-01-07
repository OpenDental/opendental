using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows.Forms;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Equipments {
		///<summary></summary>
		public static List<Equipment> GetList(DateTime fromDate,DateTime toDate,EnumEquipmentDisplayMode display,string snDesc) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Equipment>>(MethodBase.GetCurrentMethod(),fromDate,toDate,display,snDesc);
			}
			string command="";
			if(display==EnumEquipmentDisplayMode.Purchased){
				command="SELECT * FROM equipment "
					+"WHERE DatePurchased >= "+POut.Date(fromDate)
					+" AND DatePurchased <= "+POut.Date(toDate)
					+" AND (SerialNumber LIKE '%"+POut.String(snDesc)+"%' OR Description LIKE '%"+POut.String(snDesc)+"%' OR Location LIKE '%"+POut.String(snDesc)+"%')"
					+" ORDER BY DatePurchased";
			}
			if(display==EnumEquipmentDisplayMode.Sold) {
				command="SELECT * FROM equipment "
					+"WHERE DateSold >= "+POut.Date(fromDate)
					+" AND DateSold <= "+POut.Date(toDate)
					+" AND (SerialNumber LIKE '%"+POut.String(snDesc)+"%' OR Description LIKE '%"+POut.String(snDesc)+"%' OR Location LIKE '%"+POut.String(snDesc)+"%')"
					+" ORDER BY DatePurchased";
			}
			if(display==EnumEquipmentDisplayMode.All) {
				command="SELECT * FROM equipment "
					+"WHERE ((DatePurchased >= "+POut.Date(fromDate)+" AND DatePurchased <= "+POut.Date(toDate)+")"
						+" OR (DateSold >= "+POut.Date(fromDate)+" AND DateSold <= "+POut.Date(toDate)+"))"
					+" AND (SerialNumber LIKE '%"+POut.String(snDesc)+"%' OR Description LIKE '%"+POut.String(snDesc)+"%' OR Location LIKE '%"+POut.String(snDesc)+"%')"
					+" ORDER BY DatePurchased";
			}
			return Crud.EquipmentCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(Equipment equip) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				equip.EquipmentNum=Meth.GetLong(MethodBase.GetCurrentMethod(),equip);
				return equip.EquipmentNum;
			}
			return Crud.EquipmentCrud.Insert(equip);
		}

		///<summary></summary>
		public static void Update(Equipment equip) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),equip);
				return;
			}
			Crud.EquipmentCrud.Update(equip);
		}

		///<summary></summary>
		public static void Delete(Equipment equip) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),equip);
				return;
			}
			string command="DELETE FROM equipment" 
				+" WHERE EquipmentNum = "+POut.Long(equip.EquipmentNum);
 			Db.NonQ(command);
		}

		///<summary>Generates a unique 3 char alphanumeric serialnumber.  Checks to make sure it's not already in use.</summary>
		public static string GenerateSerialNum() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod());
			}
			string retVal="";
			bool isDuplicate=true;
			Random rand=new Random();
			while(isDuplicate){
				retVal="";
				for(int i=0;i<4;i++) {
					int r=rand.Next(0,34);
					if(r<9) {
						retVal+=(char)('1'+r);//1-9, no zero
					}
					else {
						retVal+=(char)('A'+r-9);
					}
				}
				string command="SELECT COUNT(*) FROM equipment WHERE SerialNumber = '"+POut.String(retVal)+"'";
				if(Db.GetScalar(command)=="0") {
					isDuplicate=false;
				}
			}
			return retVal;
		}
		
		///<summary>Checks the database for equipment that has the supplied serial number.</summary>
		public static bool HasExisting(Equipment equip) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),equip);
			}
			string command="SELECT COUNT(*) FROM equipment WHERE SerialNumber = '"+POut.String(equip.SerialNumber)+"' AND EquipmentNum != "+POut.Long(equip.EquipmentNum);
			if(Db.GetScalar(command)=="0") {
				return false;
			}
			return true;
		}
	}

	public enum EnumEquipmentDisplayMode {
		Purchased,
		Sold,
		All
	}
	


}













