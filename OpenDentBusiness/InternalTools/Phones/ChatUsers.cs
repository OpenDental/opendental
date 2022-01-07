using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ChatUsers{

		

		///<summary></summary>
		public static List<ChatUser> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ChatUser>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM chatuser";
			return Crud.ChatUserCrud.SelectMany(command);
		}

		///<summary>Gets one ChatUser from the db.</summary>
		public static ChatUser GetOne(long chatUserNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<ChatUser>(MethodBase.GetCurrentMethod(),chatUserNum);
			}
			return Crud.ChatUserCrud.SelectOne(chatUserNum);
		}

		///<summary></summary>
		public static long Insert(ChatUser chatUser){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				chatUser.ChatUserNum=Meth.GetLong(MethodBase.GetCurrentMethod(),chatUser);
				return chatUser.ChatUserNum;
			}
			return Crud.ChatUserCrud.Insert(chatUser);
		}

		///<summary></summary>
		public static void Update(ChatUser chatUser){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),chatUser);
				return;
			}
			Crud.ChatUserCrud.Update(chatUser);
		}

		///<summary></summary>
		public static void Delete(long chatUserNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),chatUserNum);
				return;
			}
			Crud.ChatUserCrud.Delete(chatUserNum);
		}

		///<summary>Truncates the chatuser table. NBD.</summary>
		public static void Truncate() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command="TRUNCATE chatuser";
			Db.NonQ(command);
		}

		public static ChatUser GetFromExt(int extension) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ChatUser>(MethodBase.GetCurrentMethod(),extension);
			}
			string command="SELECT * FROM chatuser WHERE chatuser.Extension = " + POut.Int(extension);
			return Crud.ChatUserCrud.SelectOne(command);
		}
	}
}