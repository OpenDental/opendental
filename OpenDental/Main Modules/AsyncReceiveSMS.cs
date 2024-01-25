using OpenDentBusiness;
using OpenDentBusiness.Shared;

using OpenDentBusiness.WebTypes;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml;
using Task = System.Threading.Tasks.Task;
using System.Diagnostics;
using Dicom.Imaging.LUT;

namespace OpenDental.Main_Modules
{
    internal static class AsyncReceiveSMS
    {
        private static HttpClient sharedClient = null;
        private static string auth = ODSMS.AUTH;


        async public static void receiveSMS()
        {
            if (String.IsNullOrEmpty(auth))
            {
                throw new ArgumentException("ODSMS has not been initialised.");
            }

            if (sharedClient == null)
            {
                sharedClient = new HttpClient();
                sharedClient.BaseAddress = new Uri(ODSMS.URL);
            }

            string send = "http/request-received-messages?&order=newest&" + auth;
            while (true)
            {

                try
                {
                    await Task.Delay(60 * 1000); // this is at start because there are race conditions i don't want to fix. We need to wait for init to be done before we can use comlogs
                    Console.WriteLine("Checking for new SMS now");
                    bool done = false;
                    int count = 1;
                    int offset = 0;
                    while (!done)
                    {
                        var response = await sharedClient.GetAsync(send + "&" + count);
                        var text = await response.Content.ReadAsStringAsync();
                        //EventLog.WriteEntry("ODSMS", text, EventLogEntryType.Information);
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(text);
                        var list = xmlDoc.ChildNodes[1];
                        int c = 0;
                        EventLog.WriteEntry("ODSMS", "About to loop through SMS", EventLogEntryType.Information, 101, 1, new byte[10]);

                        String sms_folder_path = @"L:\msg_guids\";
                        bool folderExists = Directory.Exists(sms_folder_path);
                        if (!folderExists)
                        {
                            EventLog.WriteEntry("ODSMS", "SMS MSG GUIDs folder not found - creating", EventLogEntryType.Warning, 101, 1, new byte[10]);
                            Directory.CreateDirectory(sms_folder_path);
                        }

                        foreach (XmlElement child in list.ChildNodes)
                        {
                            if (list.ChildNodes.Count < offset)
                            {
                                break;
                            }
                            Console.WriteLine("count");
                            Console.WriteLine(count.ToString());
                            if (c < offset)
                            {
                                continue;
                            }
                            c++;
                            string from = child.ChildNodes[0].InnerText; // diafaan doesn't use xml properly so its not nice and looks like this.
                            Console.WriteLine(from);
                            string msgText = child.ChildNodes[2].InnerText;
                            Console.WriteLine(msgText);
                            string msgTime = child.ChildNodes[11].InnerText;
                            var time = DateTime.Parse(msgTime);
                            Console.WriteLine(msgText);
                            string guid = child.ChildNodes[4].InnerText;
                            EventLog.WriteEntry("ODSMS", "SMS inner loop - downloaded a single SMS", EventLogEntryType.Information, 101, 1, new byte[10]);

                            string guidFilePath = Path.Combine(sms_folder_path, guid);

                            if (File.Exists(guidFilePath))
                            {
                                done = true;
                                break;
                            }

                            byte[] bytesToWrite = Encoding.UTF8.GetBytes(msgText);

                            using (var fileStream = File.Create(guidFilePath))
                            {
                                await fileStream.WriteAsync(bytesToWrite, 0, bytesToWrite.Length);
                            }

                            var patients = Patients.GetPatientsByPhone(from.Substring(3), "+64");
                            Console.WriteLine(patients.Count);
                            Commlog log = new Commlog();
                            if (patients.Count != 0)
                            {
                                log.PatNum = patients[0].PatNum;
                            }
                            log.Note = "Text message received: " + msgText;
                            log.Mode_ = CommItemMode.Text;
                            log.CommDateTime = time;
                            log.SentOrReceived = CommSentOrReceived.Received;
                            log.CommType = Commlogs.GetTypeAuto(CommItemTypeAuto.TEXT);
                            log.UserNum = Security.CurUser.UserNum;
                            var sms = new SmsFromMobile();
                            sms.CommlogNum = Commlogs.Insert(log);
                            sms.MobilePhoneNumber = from;
                            sms.PatNum = log.PatNum;
                            sms.DateTimeReceived = time;
                            sms.MsgText = msgText;
                            sms.SmsStatus = SmsFromStatus.ReceivedUnread;
                            sms.MsgTotal = 1;
                            sms.MatchCount = patients.Count;
                            sms.ClinicNum = 0;
                            sms.MsgPart = 1;
                            OpenDentBusiness.SmsFromMobiles.Insert(sms);
                            //Alert ODMobile where applicable.
                            PushNotificationUtils.ODM_NewTextMessage(sms, sms.PatNum);
                        }
                        offset = count;
                        count *= 2;
                        Console.WriteLine(count);
                    }
                }
                catch (Exception e)
                {
                    MsgBox.Show("Receiving patient texts failed.");
                    EventLog.WriteEntry("ODSMS", e.ToString(), EventLogEntryType.Error, 101, 1, new byte[10]);
                }
            }
        }
    }
}
