using System;
using System.Reflection;
using CodeBase;
using System.Collections.Generic;
using System.Linq;

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
			string command=$"UPDATE {CrudTableAttribute.GetTableName(autoCommType)} SET {nameof(IAutoCommSent.SendStatus)}={POut.Int((int)autoCommStatus)} " +
				$"WHERE {nameof(IAutoCommSent.MessageFk)}='{POut.Long(sms.SmsToMobileNum)}' AND {nameof(IAutoCommSent.MessageType)}={POut.Int((int)CommType.Text)}";
			Db.NonQ(command);
		}

		public static void SetMessageBody<T>(List<T> listSentObjs) where T : IAutoCommSent {
			List<long> listEmailMessageNums=listSentObjs.Where(x=>x.MessageFk!=0 && x.MessageType==CommType.Email).Select(x=>x.MessageFk).Distinct().ToList();
			List<long> listSmsToMobileNums=listSentObjs.Where(x=>x.MessageFk!=0 && x.MessageType==CommType.Text).Select(x=>x.MessageFk).Distinct().ToList();
			List<SmsToMobile> listSmsMessages=SmsToMobiles.GetMessagesByPk(listSmsToMobileNums);
			List<EmailMessage> listEmailMessages=EmailMessages.GetMessgesByPk(listEmailMessageNums);
			listSentObjs.ForEach(x => { 
				x.Message="";
				if(x.MessageType==CommType.Email) {
					x.Message=listEmailMessages.FirstOrDefault(email=>email.EmailMessageNum==x.MessageFk)?.BodyText??"";
				}
				else if(x.MessageType==CommType.Text) {
					x.Message=listSmsMessages.FirstOrDefault(text=>text.SmsToMobileNum==x.MessageFk)?.MsgText??"";
				}
			});
		}
	}
}
