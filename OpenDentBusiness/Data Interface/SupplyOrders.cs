using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class SupplyOrders {
		/*///<summary>Gets all SupplyOrders for one supplier, ordered by date.</summary>
		public static List<SupplyOrder> CreateObjects(long supplierNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SupplyOrder>>(MethodBase.GetCurrentMethod(),supplierNum);
			}
			string command="SELECT * FROM supplyorder "
				+"WHERE SupplierNum="+POut.Long(supplierNum)
				+" ORDER BY DatePlaced";
			return Crud.SupplyOrderCrud.SelectMany(command);
		}

		
		///<summary>Gets all SupplyOrders, ordered by date.</summary>
		public static List<SupplyOrder> GetAll() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SupplyOrder>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM supplyorder ORDER BY DatePlaced";
			return Crud.SupplyOrderCrud.SelectMany(command);
		}*/

		///<summary>Use supplierNum=0 for all suppliers.</summary>
		public static List<SupplyOrder> GetList(long supplierNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SupplyOrder>>(MethodBase.GetCurrentMethod(),supplierNum);
			}
			string command="SELECT * FROM supplyorder ";
			if(supplierNum>0){
				command+="WHERE SupplierNum="+POut.Long(supplierNum)+" ";
			}
			command+="ORDER BY DatePlaced";
			return Crud.SupplyOrderCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(SupplyOrder supplyOrder) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				supplyOrder.SupplyOrderNum=Meth.GetLong(MethodBase.GetCurrentMethod(),supplyOrder);
				return supplyOrder.SupplyOrderNum;
			}
			return Crud.SupplyOrderCrud.Insert(supplyOrder);
		}

		///<summary></summary>
		public static void Update(SupplyOrder supplyOrder) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),supplyOrder);
				return;
			}
			Crud.SupplyOrderCrud.Update(supplyOrder);
		}

		///<summary>No need to surround with try-catch.  Also deletes supplyOrderItems.</summary>
		public static void DeleteObject(SupplyOrder supplyOrder){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),supplyOrder);
				return;
			}
			//validate that not already in use-no
			//delete associated orderItems
			string command="DELETE FROM supplyorderitem WHERE SupplyOrderNum="+POut.Long(supplyOrder.SupplyOrderNum);
			Db.NonQ(command);
			Crud.SupplyOrderCrud.Delete(supplyOrder.SupplyOrderNum);
		}

		//Retotals all items attached to order and updates AmountTotal.
		public static SupplyOrder UpdateOrderPrice(long orderNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<SupplyOrder>(MethodBase.GetCurrentMethod(),orderNum);	
			}
			string command="SELECT SUM(Qty*Price) FROM supplyorderitem WHERE SupplyOrderNum="+orderNum;
			double amountTotal=PIn.Double(Db.GetScalar(command));
			command="SELECT * FROM supplyorder WHERE SupplyOrderNum="+orderNum;
			SupplyOrder supplyOrder=Crud.SupplyOrderCrud.SelectOne(command);
			supplyOrder.AmountTotal=amountTotal;
			SupplyOrders.Update(supplyOrder);
			return supplyOrder;
		}
	}

	


	


}









