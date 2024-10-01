using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Suppliers {
		///<summary>Gets all Suppliers.</summary>
		public static List<Supplier> GetAll() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Supplier>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM supplier ORDER BY Name";
			return Crud.SupplierCrud.SelectMany(command);
		}

		///<summary>Gets one Supplier by num.</summary>
		public static Supplier GetOne(long supplierNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Supplier>(MethodBase.GetCurrentMethod(),supplierNum);
			}
			return Crud.SupplierCrud.SelectOne(supplierNum);
		}

		///<summary></summary>
		public static long Insert(Supplier supplier) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				supplier.SupplierNum=Meth.GetLong(MethodBase.GetCurrentMethod(),supplier);
				return supplier.SupplierNum;
			}
			return Crud.SupplierCrud.Insert(supplier);
		}

		///<summary></summary>
		public static void Update(Supplier supplier) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),supplier);
				return;
			}
			Crud.SupplierCrud.Update(supplier);
		}

		///<summary>Surround with try-catch.</summary>
		public static void DeleteObject(Supplier supplier){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),supplier);
				return;
			}
			//validate that not already in use.
			string command="SELECT COUNT(*) FROM supplyorder WHERE SupplierNum="+POut.Long(supplier.SupplierNum);
			int count=PIn.Int(Db.GetCount(command));
			if(count>0) {
				throw new ApplicationException(Lans.g("Supplies","Supplier is already in use on an order. Not allowed to delete."));
			}
			command="SELECT COUNT(*) FROM supply WHERE SupplierNum="+POut.Long(supplier.SupplierNum);
			count=PIn.Int(Db.GetCount(command));
			if(count>0) {
				throw new ApplicationException(Lans.g("Supplies","Supplier is already in use on a supply. Not allowed to delete."));
			}
			Crud.SupplierCrud.Delete(supplier.SupplierNum);
		}

		public static string GetName(List<Supplier> listSuppliers,long supplierNum) {
			Meth.NoCheckMiddleTierRole();
			for(int i=0;i<listSuppliers.Count;i++){
				if(listSuppliers[i].SupplierNum==supplierNum){
					return listSuppliers[i].Name;
				}
			}
			return "";
		}
	}

	


	


}