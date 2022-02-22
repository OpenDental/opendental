using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Supplies {
		///<summary>Gets all Supplies, ordered by category and itemOrder.  Use listCategories=null to indicate all not hidden.  Use listSupplierNums=null to indicate all suppliers.</summary>
		public static List<Supply> GetList(List<long> listSupplierNums,bool showHidden,string textFind,List<long> listCategories) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Supply>>(MethodBase.GetCurrentMethod(),listSupplierNums,showHidden,textFind,listCategories);
			}
			string command="SELECT supply.* "
				+"FROM supply,definition "
				+"WHERE definition.DefNum=supply.Category ";
			if(listSupplierNums!=null){
				command+="AND SupplierNum IN ("+String.Join(",",listSupplierNums.ConvertAll(x=>x.ToString()))+") ";
			}
			if(!showHidden){
				command+="AND supply.IsHidden=0 ";
			}
			if(textFind!=""){
				command+="AND (supply.Descript LIKE '%"+POut.String(textFind)+"%' "
					+"OR supply.CatalogNumber LIKE '%"+POut.String(textFind)+"%' "
					+")";
			}
			if(listCategories!=null && listCategories.Count>0){
				command+="AND supply.Category IN ("+String.Join(",",listCategories.ConvertAll(x=>x.ToString()))+") ";
			}
			command+="ORDER BY definition.ItemOrder,supply.ItemOrder";
			return Crud.SupplyCrud.SelectMany(command);
		}

		///<Summary>Gets one supply from the database.  Used for display in SupplyOrderItemEdit window.</Summary>
		public static Supply GetSupply(long supplyNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Supply>(MethodBase.GetCurrentMethod(),supplyNum);
			}
			return Crud.SupplyCrud.SelectOne(supplyNum);
		}

		///<summary></summary>
		public static long Insert(Supply supply){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				supply.SupplyNum=Meth.GetLong(MethodBase.GetCurrentMethod(),supply);
				return supply.SupplyNum;
			}
			return Crud.SupplyCrud.Insert(supply);
		}

		///<summary></summary>
		public static void Update(Supply supply) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),supply);
				return;
			}
			Crud.SupplyCrud.Update(supply);
		}

		///<summary>Surround with try-catch.  Handles ItemOrders below this supply.</summary>
		public static void DeleteObject(Supply supply){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),supply);
				return;
			}
			//validate that not already in use.
			string command="SELECT COUNT(*) FROM supplyorderitem WHERE SupplyNum="+POut.Long(supply.SupplyNum);
			int count=PIn.Int(Db.GetCount(command));
			if(count>0){
				throw new ApplicationException(Lans.g("Supplies","Supply is already in use on an order. Not allowed to delete."));
			}
			Crud.SupplyCrud.Delete(supply.SupplyNum);
			command="UPDATE supply SET ItemOrder=(ItemOrder-1) WHERE Category="+POut.Long(supply.Category)+" AND ItemOrder>"+POut.Int(supply.ItemOrder);
			Db.NonQ(command);
		}

		///<summary>Uses single query to subtract a count, typically 1, from each supply in the list.</summary>
		public static void OrderSubtract(List<long> listSupplyNums,int countMove){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSupplyNums,countMove);
				return;
			}
			if(listSupplyNums.Count==0){
				return;
			}
			string command="UPDATE supply SET ItemOrder=(ItemOrder-"+countMove.ToString()+") WHERE SupplyNum IN("
				+String.Join(",",listSupplyNums.ConvertAll(x=>x.ToString()))+")";
			Db.NonQ(command);

		}

		///<summary>Uses single query to add a count, typically 1, to each supply in the list.</summary>
		public static void OrderAdd(List<long> listSupplyNums,int countMove){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSupplyNums,countMove);
				return;
			}
			if(listSupplyNums.Count==0){
				return;
			}
			string command="UPDATE supply SET ItemOrder=(ItemOrder+"+countMove.ToString()+") WHERE SupplyNum IN("
				+String.Join(",",listSupplyNums.ConvertAll(x=>x.ToString()))+")";
			Db.NonQ(command);

		}

		///<summary>Uses single query to add one to each supply.ItemOrder that has an itemOrder >= than specified.</summary>
		public static void OrderAddOneGreater(int itemOrder,long category,long excludeSupplyNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),itemOrder,category,excludeSupplyNum);
				return;
			}
			string command="UPDATE supply SET ItemOrder=(ItemOrder+1) "
				+"WHERE ItemOrder >= "+POut.Int(itemOrder)
				+" AND Category="+POut.Long(category)
				+" AND SupplyNum !="+POut.Long(excludeSupplyNum);
			Db.NonQ(command);
		}

		///<summary>Gets the last ItemOrder for a category.  -1 if none in that category yet.</summary>
		public static int GetLastItemOrder(long category){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),category);
			}
			string command="SELECT MAX(ItemOrder) FROM supply "
				+"WHERE Category="+POut.Long(category);
			string result=Db.GetScalar(command);
			if(result==""){
				return -1;
			}
			return PIn.Int(result);
		}
	}
}









