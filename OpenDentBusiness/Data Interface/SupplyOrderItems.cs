using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	
	///<summary></summary>
	public class SupplyOrderItems {
		///<summary>Items in the table are not SupplyOrderItems. Includes CatalogNumber and Descript</summary>
		public static DataTable GetItemsInfoForOrder(long orderNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),orderNum);
			}
			string command="SELECT CatalogNumber,Descript,Qty,supplyorderitem.Price,SupplyOrderItemNum,supplyorderitem.SupplyNum,supplyorderitem.DateReceived "
				+"FROM supplyorderitem,definition,supply "
				+"WHERE definition.DefNum=supply.Category "
				+"AND supply.SupplyNum=supplyorderitem.SupplyNum "
				+"AND supplyorderitem.SupplyOrderNum="+POut.Long(orderNum)+" "
				+"ORDER BY definition.ItemOrder,supply.ItemOrder";
			return Db.GetTable(command);
		}

		///<summary>Gets all SupplyOrderItems for the passed in OrderNum</summary>
		public static List<SupplyOrderItem> GetSupplyItemsForOrder(long orderNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SupplyOrderItem>>(MethodBase.GetCurrentMethod(),orderNum);
			}
			string command="SELECT * FROM supplyorderitem WHERE SupplyOrderNum="+POut.Long(orderNum);
			return Crud.SupplyOrderItemCrud.SelectMany(command);
		}

		public static SupplyOrderItem SelectOne(long supplyOrderItemNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<SupplyOrderItem>(MethodBase.GetCurrentMethod(),supplyOrderItemNum);
			}
			string command="SELECT * FROM supplyorderitem WHERE SupplyOrderItemNum="+POut.Long(supplyOrderItemNum);
			return Crud.SupplyOrderItemCrud.SelectOne(command);
		}

		///<summary></summary>
		public static long Insert(SupplyOrderItem supplyOrderItem){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				supplyOrderItem.SupplyOrderItemNum=Meth.GetLong(MethodBase.GetCurrentMethod(),supplyOrderItem);
				return supplyOrderItem.SupplyOrderItemNum;
			}
			return Crud.SupplyOrderItemCrud.Insert(supplyOrderItem);
		}

		///<summary></summary>
		public static void Update(SupplyOrderItem supplyOrderItem) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),supplyOrderItem);
				return;
			}
			Crud.SupplyOrderItemCrud.Update(supplyOrderItem);
		}

		///<summary>Surround with try-catch.</summary>
		public static void DeleteObject(SupplyOrderItem supplyOrderItem){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),supplyOrderItem);
				return;
			}
			//validate that not already in use.
			Crud.SupplyOrderItemCrud.Delete(supplyOrderItem.SupplyOrderItemNum);
		}
	}
	
	


	


}