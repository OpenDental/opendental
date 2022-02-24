using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary></summary>
	public class Questions {
		///<summary>Gets a list of all Questions for a given patient.  Sorted by ItemOrder.</summary>
		public static Question[] Refresh(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Question[]>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM question WHERE PatNum="+POut.Long(patNum)
				+" ORDER BY ItemOrder";
			return Crud.QuestionCrud.SelectMany(command).ToArray();
		}	

		///<summary></summary>
		public static void Update(Question quest) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),quest);
				return;
			}
			Crud.QuestionCrud.Update(quest);
		}

		///<summary></summary>
		public static long Insert(Question quest) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				quest.QuestionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),quest);
				return quest.QuestionNum;
			}
			return Crud.QuestionCrud.Insert(quest);
		}	
	}

}










