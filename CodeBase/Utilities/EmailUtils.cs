using System.Net;
using System.Net.Mail;

namespace CodeBase {
	public class EmailUtils {

		public static void SendEmail(string toAddress,string subject,string emailBody, string fromAddress,string fromPassword,string fromDisplayName,
			string fromSMTPserver,int fromServerPort,bool enableSsl=false,bool isBodyHtml=true) 
		{
			SmtpClient client=new SmtpClient(fromSMTPserver,fromServerPort);
			client.Credentials=new NetworkCredential(fromAddress,fromPassword);
			client.DeliveryMethod=SmtpDeliveryMethod.Network;
			client.EnableSsl=enableSsl;
			client.Timeout=180000;//3 minutes
			MailMessage message=new MailMessage();
			message.From=new MailAddress(fromAddress.Trim(),fromDisplayName);
			if(!string.IsNullOrWhiteSpace(toAddress)) {
				message.To.Add(toAddress.Trim());
			}
			message.Subject=subject;
			message.Body=emailBody;
			message.IsBodyHtml=isBodyHtml;
			client.Send(message);
		}
	}
}
