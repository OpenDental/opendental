using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ToothGridCells{
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<ToothGridCell> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ToothGridCell>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM toothgridcell WHERE PatNum = "+POut.Long(patNum);
			return Crud.ToothGridCellCrud.SelectMany(command);
		}

		///<summary>Gets one ToothGridCell from the db.</summary>
		public static ToothGridCell GetOne(long toothGridCellNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<ToothGridCell>(MethodBase.GetCurrentMethod(),toothGridCellNum);
			}
			return Crud.ToothGridCellCrud.SelectOne(toothGridCellNum);
		}

		///<summary></summary>
		public static long Insert(ToothGridCell toothGridCell){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				toothGridCell.ToothGridCellNum=Meth.GetLong(MethodBase.GetCurrentMethod(),toothGridCell);
				return toothGridCell.ToothGridCellNum;
			}
			return Crud.ToothGridCellCrud.Insert(toothGridCell);
		}

		///<summary></summary>
		public static void Update(ToothGridCell toothGridCell){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),toothGridCell);
				return;
			}
			Crud.ToothGridCellCrud.Update(toothGridCell);
		}

		///<summary></summary>
		public static void Delete(long toothGridCellNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),toothGridCellNum);
				return;
			}
			string command= "DELETE FROM toothgridcell WHERE ToothGridCellNum = "+POut.Long(toothGridCellNum);
			Db.NonQ(command);
		}
		*/
	}
}