using System;
using System.Reflection;
using CodeBase;

namespace OpenDentBusiness.WebTypes.AutoComm {
	public class AutoCommSents {
		public static void UpdateAutoCommStatus(SmsToMobile sms,AutoCommStatus autoCommStatus) {			
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),sms,autoCommStatus);
				return;
			}
			Type autoCommType=EnumTools.GetAttributeOrDefault<AutoCommSmsAttribute>(sms.MsgType)?.AutoCommSentType;
			if(autoCommType is null) {
				return;
			}
			string command=$"UPDATE {CrudTableAttribute.GetTableName(autoCommType)} SET {nameof(AutoCommSent.SendStatus)}={POut.Int((int)autoCommStatus)} " +
				$"WHERE {nameof(AutoCommSent.MessageFk)}='{POut.Long(sms.SmsToMobileNum)}' AND {nameof(AutoCommSent.MessageType)}={POut.Int((int)CommType.Text)}";
			Db.NonQ(command);
		}
	}
}
