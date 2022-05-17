using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace OpenDental{

	///<summary>A list of query favorites that users can run.</summary>
	public struct UserQuery{
		///<summary>Primary key.</summary>
		public int QueryNum;
		///<summary>Description.</summary>
		public string Description;
		///<summary>The name of the file to export to.</summary>
		public string FileName;
		///<summary>The text of the query.</summary>
		public string QueryText;
	}

	/*=========================================================================================
		=================================== class UserQueries ==========================================*/
///<summary></summary>
	public class UserQueries{
		///<summary></summary>
		public static UserQuery[] List;
		///<summary></summary>
		public static UserQuery Cur;
		///<summary></summary>
		public static bool IsSelected;

		///<summary></summary>
		public static void Refresh(){
			string command =
				"SELECT querynum,description,filename,querytext"
				+" FROM userquery"
				//+" WHERE hidden != '1'";
				+" ORDER BY description";
			DataTable table=General.GetTable(command);
			List=new UserQuery[table.Rows.Count];
			for(int i=0;i<table.Rows.Count;i++){
				List[i].QueryNum    = PIn.PInt   (table.Rows[i][0].ToString());
				List[i].Description = PIn.PString(table.Rows[i][1].ToString());
				List[i].FileName    = PIn.PString(table.Rows[i][2].ToString());
				List[i].QueryText   = PIn.PString(table.Rows[i][3].ToString());
			}
		}

		///<summary></summary>
		public static void InsertCur(){
			string command="INSERT INTO userquery (description,filename,querytext) VALUES("
				+"'"+POut.PString(Cur.Description)+"', "
				+"'"+POut.PString(Cur.FileName)+"', "
				+"'"+POut.PString(Cur.QueryText)+"')";
			//MessageBox.Show(command);
			General.NonQ(command);
		}
		
		///<summary></summary>
		public static void DeleteCur(){
			string command = "DELETE from userquery WHERE querynum = '"+POut.PInt(Cur.QueryNum)+"'";
			General.NonQ(command);
		}

		///<summary></summary>
		public static void UpdateCur(){
			string command = "UPDATE userquery SET "
				+ "description = '" +POut.PString(Cur.Description)+"'"
				+ ",filename = '"    +POut.PString(Cur.FileName)+"'"
				+",querytext = '"   +POut.PString(Cur.QueryText)+"'"
				+" WHERE querynum = '"+POut.PInt(Cur.QueryNum)+"'";
			General.NonQ(command);
		}
	}

	

	
}













