using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	
	///<summary></summary>
	public class QuickPasteNote{
		///<summary>Primary key.</summary>
		public int QuickPasteNoteNum;
		///<summary>FK to quickpastecat.QuickPasteCatNum.  Keeps track of which category this note is in.</summary>
		public int QuickPasteCatNum;
		///<summary>The order of this note within it's category. 0-based.</summary>
		public int ItemOrder;
		///<summary>The actual note. Can be multiple lines and possibly very long.</summary>
		public string Note;
		///<summary>The abbreviation which will automatically substitute when preceded by a ?.</summary>
		public string Abbreviation;

		///<summary></summary>
		public void Insert(){
			if(PrefB.RandomKeys){
				QuickPasteNoteNum=MiscData.GetKey("quickpastenote","QuickPasteNoteNum");
			}
			string command= "INSERT INTO quickpastenote (";
			if(PrefB.RandomKeys){
				command+="QuickPasteNoteNum,";
			}
			command+="QuickPasteCatNum,ItemOrder,Note,Abbreviation) VALUES(";
			if(PrefB.RandomKeys){
				command+="'"+POut.PInt(QuickPasteNoteNum)+"', ";
			}
			command+=
				 "'"+POut.PInt   (QuickPasteCatNum)+"', "
				+"'"+POut.PInt   (ItemOrder)+"', "
				+"'"+POut.PString(Note)+"', "
				+"'"+POut.PString(Abbreviation)+"')";
 			if(PrefB.RandomKeys){
				General.NonQ(command);
			}
			else{
 				QuickPasteNoteNum=General.NonQ(command,true);
			}
		}

		///<summary></summary>
		public void Update(){
			string command="UPDATE quickpastenote SET "
				+"QuickPasteCatNum='" +POut.PInt   (QuickPasteCatNum)+"'"
				+",ItemOrder = '"     +POut.PInt   (ItemOrder)+"'"
				+",Note = '"          +POut.PString(Note)+"'"
				+",Abbreviation = '"  +POut.PString(Abbreviation)+"'"
				+" WHERE QuickPasteNoteNum = '"+POut.PInt (QuickPasteNoteNum)+"'";
 			General.NonQ(command);
		}

		
		///<summary></summary>
		public void Delete(){
			string command="DELETE from quickpastenote WHERE QuickPasteNoteNum = '"
				+POut.PInt(QuickPasteNoteNum)+"'";
 			General.NonQ(command);
		}

		///<summary>When saving an abbrev, this makes sure that the abbreviation is not already in use.</summary>
		public bool AbbrAlreadyInUse(){
			string command="SELECT * FROM quickpastenote WHERE "
				+"Abbreviation='"+POut.PString(Abbreviation)+"' "
				+"AND QuickPasteNoteNum != '"+POut.PInt (QuickPasteNoteNum)+"'";
 			DataTable table=General.GetTable(command);
			if(table.Rows.Count==0){
				return false;
			}
			return true;
		}




	}	

	/*=========================================================================================
	=================================== class QuickPasteNotes======================================*/

	///<summary></summary>
	public class QuickPasteNotes{
		///<summary>list of all notes for all categories. Not very useful.</summary>
		private static QuickPasteNote[] List;

		///<summary></summary>
		public static void Refresh(){
			string command=
				"SELECT * from quickpastenote "
				+"ORDER BY ItemOrder";
 			DataTable table=General.GetTable(command);
			List=new QuickPasteNote[table.Rows.Count];
			for(int i=0;i<List.Length;i++){
				List[i]=new QuickPasteNote();
				List[i].QuickPasteNoteNum= PIn.PInt   (table.Rows[i][0].ToString());
				List[i].QuickPasteCatNum = PIn.PInt   (table.Rows[i][1].ToString());
				List[i].ItemOrder        = PIn.PInt   (table.Rows[i][2].ToString());	
				List[i].Note             = PIn.PString(table.Rows[i][3].ToString());
				List[i].Abbreviation     = PIn.PString(table.Rows[i][4].ToString());
			}
		}

		///<summary>Only used from FormQuickPaste to get all notes for the selected cat.</summary>
		public static QuickPasteNote[] GetForCat(int cat){
			ArrayList ALnotes=new ArrayList();
			for(int i=0;i<List.Length;i++){
				if(List[i].QuickPasteCatNum==cat){
					ALnotes.Add(List[i]);
				}
			}
			QuickPasteNote[] retArray=new QuickPasteNote[ALnotes.Count];
			for(int i=0;i<ALnotes.Count;i++){
				retArray[i]=(QuickPasteNote)ALnotes[i];
			}
			return retArray;
		}

		///<summary>Called on KeyUp from various textBoxes in the program to look for a ?abbrev and attempt to substitute.  Substitutes the text if found.</summary>
		public static string Substitute(string text,QuickPasteType type){
			int typeIndex=QuickPasteCats.GetDefaultType(type);
			for(int i=0;i<List.Length;i++){
				if(List[i].Abbreviation==""){
					continue;
				}
				if(List[i].QuickPasteCatNum!=QuickPasteCats.List[typeIndex].QuickPasteCatNum){
					continue;
				}
				text=Regex.Replace(text,@"\?"+List[i].Abbreviation,List[i].Note);
			}
			return text;
		}


		


		


	}

	


}









