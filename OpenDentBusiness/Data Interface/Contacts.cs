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
			string command="SELECT * from contact WHERE category = '"+POut.Long(category)+"'"
				+" ORDER BY LName";
			return Crud.ContactCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(Contact contact) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				contact.ContactNum=Meth.GetLong(MethodBase.GetCurrentMethod(),contact);
				return contact.ContactNum;
			}
			return Crud.ContactCrud.Insert(contact);
		}

		///<summary></summary>
		public static void Update(Contact contact){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),contact);
				return;
			}
			Crud.ContactCrud.Update(contact);
		}

		///<summary></summary>
		public static void Delete(Contact contact){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),contact);
				return;
			}
			string command = "DELETE FROM contact WHERE contactnum = '"+POut.String(contact.ContactNum.ToString())+"'";
			Db.NonQ(command);
		}
	}

	
}