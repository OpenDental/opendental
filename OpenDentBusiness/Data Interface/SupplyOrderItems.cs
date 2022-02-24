using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	
	///<summary></summary>
	public class SupplyOrderItems {
		///<summary>Items in the table are not SupplyOrderItems</summary>
		public static DataTable GetItemsForOrder(long orderNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),orderNum);
			}
			string command="SELECT CatalogNumber,Descript,Qty,supplyorderitem.Price,SupplyOrderItemNum,supplyorderitem.SupplyNum "
				+"FROM supplyorderitem,definition,supply "
				+"WHERE definition.DefNum=supply.Category "
				+"AND supply.SupplyNum=supplyorderitem.SupplyNum "
				+"AND supplyorderitem.SupplyOrderNum="+POut.Long(orderNum)+" "
				+"ORDER BY definition.ItemOrder,supply.ItemOrder";
			return Db.GetTable(command);
		}

		public static SupplyOrderItem SelectOne(long supplyOrderItemNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SupplyOrderItem>(MethodBase.GetCurrentMethod(),supplyOrderItemNum);
			}
			string command="SELECT * FROM supplyorderitem WHERE SupplyOrderItemNum="+POut.Long(supplyOrderItemNum);
			return Crud.SupplyOrderItemCrud.SelectOne(command);
		}

		///<summary></summary>
		public static long Insert(SupplyOrderItem supplyOrderItem){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				supplyOrderItem.SupplyOrderItemNum=Meth.GetLong(MethodBase.GetCurrentMethod(),supplyOrderItem);
				return supplyOrderItem.SupplyOrderItemNum;
			}
			return Crud.SupplyOrderItemCrud.Insert(supplyOrderItem);
		}

		///<summary></summary>
		public static void Update(SupplyOrderItem supplyOrderItem) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),supplyOrderItem);
				return;
			}
			Crud.SupplyOrderItemCrud.Update(supplyOrderItem);
		}

		///<summary>Surround with try-catch.</summary>
		public static void DeleteObject(SupplyOrderItem supp){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),supp);
				return;
			}
			//validate that not already in use.
			Crud.SupplyOrderItemCrud.Delete(supp.SupplyOrderItemNum);
		}
	}
	
	


	


}









