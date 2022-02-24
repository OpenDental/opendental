using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{

	///<summary>Used in public health.  Programming note: There are many extra fields in common with the screen table, but they are only in this struct and not in the database itself, where that data is stored with the individual screen items. The data in this table is irrelevant in reports.  It is just used to help organize the user interface.</summary>
	public struct ScreenGroup{
		///<summary>Primary key</summary>
		public int ScreenGroupNum;
		///<summary>Up to the user.</summary>
		public string Description;
		///<summary>Date used to help order the groups.</summary>
		public DateTime SGDate;
		///<summary>Not a database column. Used if ProvNum=0.</summary>
		public string ProvName;
		///<summary>Not a database column. Foreign key to provider.ProvNum. Can be 0 if not a standard provider.  In that case, a ProvName should be entered.</summary>
		public int ProvNum;
		///<summary>Not a database column. See the PlaceOfService enum.</summary>
		public PlaceOfService PlaceService;
		///<summary>Not a database column. Foreign key to county.CountyName, although it will not crash if key absent.</summary>
		public string County;
		///<summary>Not a database column. Foreign key to school.SchoolName, although it will not crash if key absent.</summary>
		public string GradeSchool;
	}

	/*=========================================================================================
		=================================== class ScreenGroups ===========================================*/
  ///<summary></summary>
	public class ScreenGroups{
		///<summary></summary>
		public static ScreenGroup Cur;
		///<summary></summary>
		public static ScreenGroup[] List;

		///<summary></summary>
		public static void Refresh(DateTime fromDate,DateTime toDate){
			string command =
				"SELECT * from screengroup "
				+"WHERE SGDate >= '"+POut.PDateT(fromDate)+"' "
				+"&& SGDate <= '"+POut.PDateT(toDate.AddDays(1))+"' "
				//added one day since it's calculated based on midnight.
				+"ORDER BY SGDate,ScreenGroupNum";
			DataTable table=General.GetTable(command);
			List=new ScreenGroup[table.Rows.Count];
			for(int i=0;i<List.Length;i++){
				List[i].ScreenGroupNum =                  PIn.PInt   (table.Rows[i][0].ToString());
				List[i].Description    =                  PIn.PString(table.Rows[i][1].ToString());
				List[i].SGDate         =                  PIn.PDate  (table.Rows[i][2].ToString());
			}
		}

		///<summary></summary>
		public static void InsertCur(){
			if(PrefB.RandomKeys){
				Cur.ScreenGroupNum=MiscData.GetKey("screengroup","ScreenGroupNum");
			}
			string command="INSERT INTO screengroup (";
			if(PrefB.RandomKeys){
				command+="ScreenGroupNum,";
			}
			command+="Description,SGDate) VALUES(";
			if(PrefB.RandomKeys){
				command+="'"+POut.PInt(Cur.ScreenGroupNum)+"', ";
			}
			command+=
				 "'"+POut.PString(Cur.Description)+"', "
				+"'"+POut.PDate  (Cur.SGDate)+"')";
			if(PrefB.RandomKeys){
				General.NonQ(command);
			}
			else{
 				Cur.ScreenGroupNum=General.NonQ(command,true);
			}
		}

		///<summary></summary>
		public static void UpdateCur(){
			string command = "UPDATE screengroup SET "
				+"Description ='"  +POut.PString(Cur.Description)+"'"
				+",SGDate ='"      +POut.PDate  (Cur.SGDate)+"'"
				+" WHERE ScreenGroupNum = '" +POut.PInt(Cur.ScreenGroupNum)+"'";
			General.NonQ(command);
		}

		///<summary>This will also delete all screen items, so may need to ask user first.</summary>
		public static void DeleteCur(){
			string command="DELETE from screen WHERE ScreenGroupNum ='"+POut.PInt(Cur.ScreenGroupNum)+"'";
			General.NonQ(command);
			command="DELETE from screengroup WHERE ScreenGroupNum ='"+POut.PInt(Cur.ScreenGroupNum)+"'";
			General.NonQ(command);
		}


	}

	

}













