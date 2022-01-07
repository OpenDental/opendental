using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary></summary>
	public class QuestionDefs {
		///<summary>Gets a list of all QuestionDefs.</summary>
		public static QuestionDef[] Refresh() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<QuestionDef[]>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM questiondef ORDER BY ItemOrder";
			return Crud.QuestionDefCrud.SelectMany(command).ToArray();
		}

		///<summary></summary>
		public static void Update(QuestionDef def) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),def);
				return;
			}
			Crud.QuestionDefCrud.Update(def);
		}

		///<summary></summary>
		public static long Insert(QuestionDef def) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				def.QuestionDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),def);
				return def.QuestionDefNum;
			}
			return Crud.QuestionDefCrud.Insert(def);
		}

		///<summary>Ok to delete whenever, because no patients are tied to this table by any dependencies.</summary>
		public static void Delete(QuestionDef def) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),def);
				return;
			}
			string command="DELETE FROM questiondef WHERE QuestionDefNum ="+POut.Long(def.QuestionDefNum);
			Db.NonQ(command);
		}



		///<summary>Moves the selected item up in the list.</summary>
		public static void MoveUp(int selected,QuestionDef[] List){
			//No need to check RemotingRole; no call to db.
			if(selected<0) {
				throw new ApplicationException(Lans.g("QuestionDefs","Please select an item first."));
			}
			if(selected==0) {//already at top
				return;
			}
			if(selected>List.Length-1){
				throw new ApplicationException(Lans.g("QuestionDefs","Invalid selection."));
			}
			SetOrder(selected-1,List[selected].ItemOrder,List);
			SetOrder(selected,List[selected].ItemOrder-1,List);
		}

		///<summary></summary>
		public static void MoveDown(int selected,QuestionDef[] List) {
			//No need to check RemotingRole; no call to db.
			if(selected<0) {
				throw new ApplicationException(Lans.g("QuestionDefs","Please select an item first."));
			}
			if(selected==List.Length-1){//already at bottom
				return;
			}
			if(selected>List.Length-1) {
				throw new ApplicationException(Lans.g("QuestionDefs","Invalid selection."));
			}
			SetOrder(selected+1,List[selected].ItemOrder,List);
			SetOrder(selected,List[selected].ItemOrder+1,List);
			//selected+=1;
		}

		///<summary>Used by MoveUp and MoveDown.</summary>
		private static void SetOrder(int mySelNum,int myItemOrder,QuestionDef[] List) {
			//No need to check RemotingRole; no call to db.
			QuestionDef temp=List[mySelNum];
			temp.ItemOrder=myItemOrder;
			QuestionDefs.Update(temp);
		}
	}
	
}










