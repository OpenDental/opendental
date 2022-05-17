using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Collections.Generic;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Contacts{
		//<summary></summary>
		//public static Contact[] List;//for one category only. Not refreshed with local data

		///<summary></summary>
		public static List<Contact> Refresh(long category) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Contact>>(MethodBase.GetCurrentMethod(),category);
			}
			string command="SELECT * from contact WHERE category = '"+category+"'"
				+" ORDER BY LName";
			return Crud.ContactCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(Contact Cur) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Cur.ContactNum=Meth.GetLong(MethodBase.GetCurrentMethod(),Cur);
				return Cur.ContactNum;
			}
			return Crud.ContactCrud.Insert(Cur);
		}

		///<summary></summary>
		public static void Update(Contact Cur){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			Crud.ContactCrud.Update(Cur);
		}

		///<summary></summary>
		public static void Delete(Contact Cur){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			string command = "DELETE FROM contact WHERE contactnum = '"+Cur.ContactNum.ToString()+"'";
			Db.NonQ(command);
		}
	}

	
}