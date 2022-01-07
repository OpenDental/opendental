using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Contacts{
		//<summary></summary>
		//public static Contact[] List;//for one category only. Not refreshed with local data

		///<summary></summary>
		public static Contact[] Refresh(long category) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Contact[]>(MethodBase.GetCurrentMethod(),category);
			}
			string command="SELECT * from contact WHERE category = '"+category+"'"
				+" ORDER BY LName";
			return Crud.ContactCrud.SelectMany(command).ToArray();
		}

		///<summary></summary>
		public static long Insert(Contact Cur) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Cur.ContactNum=Meth.GetLong(MethodBase.GetCurrentMethod(),Cur);
				return Cur.ContactNum;
			}
			return Crud.ContactCrud.Insert(Cur);
		}

		///<summary></summary>
		public static void Update(Contact Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			Crud.ContactCrud.Update(Cur);
		}

		///<summary></summary>
		public static void Delete(Contact Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			string command = "DELETE FROM contact WHERE contactnum = '"+Cur.ContactNum.ToString()+"'";
			Db.NonQ(command);
		}
	}

	
}