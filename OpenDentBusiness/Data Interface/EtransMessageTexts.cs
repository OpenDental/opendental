using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using OpenDentBusiness;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EtransMessageTexts {
		///<summary></summary>
		public static long Insert(EtransMessageText etransMessageText) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				etransMessageText.EtransMessageTextNum=Meth.GetLong(MethodBase.GetCurrentMethod(),etransMessageText);
				return etransMessageText.EtransMessageTextNum;
			}
			return Crud.EtransMessageTextCrud.Insert(etransMessageText);
		}

		///<summary>If the message text is X12, then it always normalizes it to include carriage returns for better readability.</summary>
		public static string GetMessageText(long etransMessageTextNum,bool isFormattingNeededX12=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),etransMessageTextNum,isFormattingNeededX12);
			}
			if(etransMessageTextNum==0) {
				return "";
			}
			string command="SELECT MessageText FROM etransmessagetext WHERE EtransMessageTextNum="+POut.Long(etransMessageTextNum);
			string msgText=Db.GetScalar(command);
			if(isFormattingNeededX12) {
				return TidyMessageTextX12(msgText);
			}
			return msgText;
		}

		///<summary>This function is used to enhance readabilty of the X12 message when displayed.
		///This function is specifically for X12 messages and not for other formats (ex not for Canadian).</summary>
		private static string TidyMessageTextX12(string msgText) {
			if(!X12object.IsX12(msgText)) {
				return msgText;
			}
			Match match=Regex.Match(msgText,"~[^(\n)(\r)]");
			while(match.Success){
				msgText=msgText.Substring(0,match.Index)+"~\r\n"+msgText.Substring(match.Index+1);
				match=Regex.Match(msgText,"~[^(\n)(\r)]");
			}
			return msgText;
		}
		
		///<summary>Returns dictionary such that the key is an etransMessageTextNum and the value is the MessageText.
		///If the message text is X12, then it always normalizes it to include carriage returns for better readability.</summary>
		public static SerializableDictionary<long,string> GetMessageTexts(List<long> listEtransMessageTextNums,bool isFormattingNeededX12=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SerializableDictionary<long,string>>(MethodBase.GetCurrentMethod(),listEtransMessageTextNums,isFormattingNeededX12);
			}
			SerializableDictionary<long,string> retVal=new SerializableDictionary<long, string>();
			if(listEtransMessageTextNums==null || listEtransMessageTextNums.Count==0) {
				return retVal;
			}
			string command="SELECT EtransMessageTextNum,MessageText FROM etransmessagetext WHERE EtransMessageTextNum IN("+string.Join(",",listEtransMessageTextNums)+")";
			DataTable dataTable=Db.GetTable(command);
			foreach(DataRow row in dataTable.Rows) {
				long msgNum=PIn.Long(row["EtransMessageTextNum"].ToString());
				string msgText=row["MessageText"].ToString();
				if(isFormattingNeededX12) {
					msgText=TidyMessageTextX12(msgText);
				}
				retVal.Add(msgNum,msgText);
			}
			return retVal;
		}

		///<summary>Returns any EtransMessageText where the MessageText is identical to the given messageText.
		///Otherwise if none returns null.</summary>
		public static EtransMessageText GetMostRecentForType(EtransType type) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<EtransMessageText>(MethodBase.GetCurrentMethod(),type);
			}
			string command="SELECT etransmessagetext.* FROM etransmessagetext "
			+"INNER JOIN etrans ON etrans.EtransMessageTextNum=etransmessagetext.EtransMessageTextNum "
			+"WHERE Etype="+POut.Int((int)type)+" "
			+"ORDER BY etrans.DateTimeTrans DESC";
			command=DbHelper.LimitOrderBy(command,1);//Most recent entry if any.
			return Crud.EtransMessageTextCrud.SelectOne(command);
		}

		/*
		///<summary></summary>
		public static void Update(EtransMessageText EtransMessageText) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),EtransMessageText);
				return;
			}
			string command= "UPDATE EtransMessageText SET "
				+"ClearingHouseNum = '"   +POut.PInt   (EtransMessageText.ClearingHouseNum)+"', "
				+"Etype= '"               +POut.PInt   ((int)EtransMessageText.Etype)+"', "
				+"Note= '"                +POut.PString(EtransMessageText.Note)+"', "
				+"EtransMessageTextMessageTextNum= '"+POut.PInt   (EtransMessageText.EtransMessageTextMessageTextNum)+"' "
				+"WHERE EtransMessageTextNum = "+POut.PInt(EtransMessageText.EtransMessageTextNum);
			Db.NonQ(command);
		}
*/

		///<summary></summary>
		public static void Delete(long etransMessageTextNum,long etransNum=0) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),etransMessageTextNum,etransNum);
				return;
			}
			if(etransMessageTextNum==0) {
				return;
			}
			string command;
			if(etransNum==0) {
				command="DELETE FROM etransmessagetext WHERE EtransMessageTextNum="+POut.Long(etransMessageTextNum);
			}
			else { 
				//When a etransNum is specified we cannot delete the EtransMessageText row if it is associated to any other etransNum.
				command="DELETE etransMessageText FROM etransMessageText "
					+"LEFT JOIN etrans ON etrans.EtransMessageTextNum=etransMessageText.EtransMessageTextNum AND etrans.EtransNum!="+POut.Long(etransNum)+" "
					+"WHERE etransMessageText.EtransMessageTextNum="+POut.Long(etransMessageTextNum)+" "
					+"AND etrans.EtransNum IS NULL";
			}
			Db.NonQ(command);
		}		
	}
}