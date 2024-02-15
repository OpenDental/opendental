using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
  ///<summary></summary>
	public class ScreenGroups{
		///<summary></summary>
		public static List<ScreenGroup> Refresh(DateTime dateFrom,DateTime dateTo){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ScreenGroup>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo);
			}
			string command =
				"SELECT * from screengroup "
				+"WHERE SGDate >= "+POut.DateT(dateFrom)+" "
				+"AND SGDate < "+POut.DateT(dateTo.AddDays(1))+" "//Was including entries form the next day. Changed from <= to <.
				//added one day since it's calculated based on midnight.
				+"ORDER BY SGDate,ScreenGroupNum";
			return Crud.ScreenGroupCrud.SelectMany(command);
		}

		public static ScreenGroup GetScreenGroup(long screenGroupNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ScreenGroup>(MethodBase.GetCurrentMethod(),screenGroupNum);
			}
			string command=
				"SELECT * FROM screengroup WHERE ScreenGroupNum="+POut.Long(screenGroupNum);
			return Crud.ScreenGroupCrud.SelectOne(command);
		}

		///<summary></summary>
		public static long Insert(ScreenGroup screenGroup) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				screenGroup.ScreenGroupNum=Meth.GetLong(MethodBase.GetCurrentMethod(),screenGroup);
				return screenGroup.ScreenGroupNum;
			}
			return Crud.ScreenGroupCrud.Insert(screenGroup);
		}

		///<summary></summary>
		public static void Update(ScreenGroup screenGroup){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),screenGroup);
				return;
			}
			Crud.ScreenGroupCrud.Update(screenGroup);
		}

		///<summary>This will also delete all screen items, so may need to ask user first.</summary>
		public static void Delete(ScreenGroup screenGroup){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),screenGroup);
				return;
			}
			string command="SELECT SheetNum FROM screen WHERE ScreenGroupNum="+POut.Long(screenGroup.ScreenGroupNum)+" AND SheetNum!=0";
			DataTable table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {//Delete any attached sheets if the screen gets deleted.
				Sheets.Delete(PIn.Long(table.Rows[i]["SheetNum"].ToString()));
			}
			command="DELETE FROM screen WHERE ScreenGroupNum ='"+POut.Long(screenGroup.ScreenGroupNum)+"'";
			Db.NonQ(command);
			command="DELETE FROM screengroup WHERE ScreenGroupNum ='"+POut.Long(screenGroup.ScreenGroupNum)+"'";
			Db.NonQ(command);
		}
	}

}













