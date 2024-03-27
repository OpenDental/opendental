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
using OpenDentBusiness.Crud;

using Dicom.Imaging.LUT;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Security.Policy;
using System.Collections.Generic;
using System.Linq;
using Serilog.Events;
using DataConnectionBase;

// Questions for Nathan
//
// We have Open Dental\ Main Modules\ Async SMS Handling
// This currently only has receive SMS async (and the Check SMS connection I wrote)
// Where to put the once-an-hour tasks (check for birthday texts, check SMS connection, check missing SMS)

namespace OpenDental.Main_Modules
{
    internal static class AsyncSMSHandling
    {
        private static HttpClient sharedClient = ODSMS.sharedClient;
        private static string auth = ODSMS.AUTH;
        private static String sms_folder_path = @"L:\msg_guids\";

        public static async System.Threading.Tasks.Task<bool> CheckSMSConnection()
        {
            try
            {
                string checkStr = "http/request-server-status?" + auth;
                HttpResponseMessage httpResponseMessage = await sharedClient.GetAsync(checkStr);
                var text = await httpResponseMessage.Content.ReadAsStringAsync();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(text);
                XmlNode gsmModemGateway = xmlDoc.SelectSingleNode("//Gateway[@Name='GSM Modem Gateway']");
                if (gsmModemGateway != null)
                {
                    // Extract the attributes 'Active', 'Available', 'SendEnabled', and 'ReceiveEnabled'
                    string active = gsmModemGateway.Attributes["Active"].InnerText;
                    string available = gsmModemGateway.Attributes["Available"].InnerText;
                    string sendEnabled = gsmModemGateway.Attributes["SendEnabled"].InnerText;
                    string receiveEnabled = gsmModemGateway.Attributes["ReceiveEnabled"].InnerText;

                    // Convert attribute values to boolean for easier interpretation
                    bool isActive = active == "1";
                    bool isAvailable = available == "1";
                    bool isSendEnabled = sendEnabled == "1";
                    bool isReceiveEnabled = receiveEnabled == "1";

                    Console.WriteLine($"GSM Modem Gateway Status:");
                    Console.WriteLine($"Active: {isActive}");
                    Console.WriteLine($"Available: {isAvailable}");
                    Console.WriteLine($"Send Enabled: {isSendEnabled}");
                    Console.WriteLine($"Receive Enabled: {isReceiveEnabled}");

                    return isActive && isAvailable && isSendEnabled && isReceiveEnabled;

                }
                else
                {
                    Console.WriteLine("GSM Modem Gateway not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking Diafaan status: {ex.Message}");
                EventLog.WriteEntry("ODSMS", "Something bad happened checking Diafaan status", EventLogEntryType.Error, 101, 1, new byte[10]);
            }

            return false;
            // We're looking for a line like: <Gateway Name="GSM Modem Gateway" Id="1" Active="1" Available="1" SendEnabled="1" ReceiveEnabled="1">

        }

        async public static void onceAnHour()
        {
            bool smsIsWorking = await CheckSMSConnection();
            if (!smsIsWorking)
            {
                MsgBox.Show("Failure checking Diafaan.  SMS is currently brken.");
                EventLog.WriteEntry("ODSMS", "Diafaan says that SMS is not working", EventLogEntryType.Error, 101, 1, new byte[10]);
                ODSMS.USE_ODSMS = false;        // Disable the plugin
            }

            sendBirthdayTexts();

            receiveSMS();

        }

        private static void sendBirthdayTexts()
        {
            // Step 1: Check time
            var currentTime = DateTime.Now; // Using local time
            if (currentTime.Hour < 7)
            {
                return; // It's before 7 AM, do not proceed
            }

            string birthdayMessageTemplate = PrefC.GetString(PrefName.BirthdayPostcardMsg);


            // Step 2: Find patients with birthdays today
            var patientsWithBirthday = GetPatientsWithBirthdayToday();

            // Step 3: Prepare SMS messages
            List<SmsToMobile> messagesToSend = patientsWithBirthday.Select(patient =>
                new SmsToMobile
                {
                    PatNum = patient.PatNum, // Assuming Patient class has PatNum property
                    SmsPhoneNumber = "+64275598699", // Set to your sending phone number in international format
                    MobilePhoneNumber = patient.WirelessPhone, // Assuming Patient class has PhoneNumber property
                    MsgText = birthdayMessageTemplate.Replace("?FName", patient.FName), // The birthday message you want to send
                    MsgType = SmsMessageSource.GeneralMessage, // Set the source based on your SmsMessageSource enum
                    SmsStatus = SmsDeliveryStatus.Pending, // Initial status, assuming you have a Pending status
                    MsgParts = 1, // Assuming the message fits into one part
                                  // Set any other necessary properties
                }).ToList();

            // Step 4: Send the SMS messages
            if (messagesToSend.Any())
            {
                foreach (var sms in messagesToSend)
                {
                    Console.WriteLine($"To: {sms.MobilePhoneNumber}, Message: {sms.MsgText}");

                    // Print any other relevant properties you want to check
                }
                SmsToMobiles.SendSmsMany(messagesToSend);
            }

        }

        public static List<Patient> GetPatientsWithBirthdayToday()
        {
            string select = "SELECT p.* ";
            string from = "FROM patient AS p ";
            // Start with a WHERE TRUE to allow all subsequent conditions to use AND
            string where_true = "WHERE TRUE ";
            string where_active = $"AND p.PatStatus IN ({(int)PatientStatus.Patient}) "; // Only text active patients

            // Filter for patients with a birthday today
            string where_birthday = $"AND MONTH(p.Birthdate) = MONTH(CURRENT_DATE()) " +
                                    $"AND DAY(p.Birthdate) = DAY(CURRENT_DATE()) ";

            // Filter to exclude patients who have been contacted in the last 72 hours
            string where_not_contacted = "AND NOT EXISTS (" +
                                         "SELECT 1 " +
                                         "FROM CommLog m " +
                                         "WHERE m.PatNum = p.PatNum " +
                                         "AND m.Note LIKE '%Birthday%' " +
                                         "AND m.CommDateTime > DATE_SUB(NOW(), INTERVAL 3 DAY)) ";

            string where_mobile_phone = "AND LENGTH(COALESCE(p.WirelessPhone,'')) > 7 ";

            // Combine all parts to form the final command
            string command = select + from + where_true + where_active + where_birthday + where_not_contacted + where_mobile_phone;
            Console.WriteLine(command);

            List<Patient> listPats = OpenDentBusiness.Crud.PatientCrud.SelectMany(command);
            return listPats;
        }

        async public static void processOneSMS(string msgText, string msgTime, string msgFrom, string msgGUID)
        {

            EventLog.WriteEntry("ODSMS", "SMS inner loop - downloaded a single SMS", EventLogEntryType.Information, 101, 1, new byte[10]);
            string guidFilePath = Path.Combine(sms_folder_path, msgGUID);

            byte[] bytesToWrite = Encoding.UTF8.GetBytes(msgText);

            using (var fileStream = File.Create(guidFilePath))
            {
                await fileStream.WriteAsync(bytesToWrite, 0, bytesToWrite.Length);
            }

            var patients = Patients.GetPatientsByPhone(msgFrom.Substring(3), "+64", new List<PhoneType> { PhoneType.WirelessPhone });
            var time = DateTime.Parse(msgTime);

            Console.WriteLine($"Number of matching patients: {patients.Count}");
            if (patients.Count > 20)
            {
                EventLog.WriteEntry("ODSMS", "Too many patients match this number! Assuming a dummy entry", EventLogEntryType.Information, 101, 1, new byte[10]);
                return;
            }

            Commlog log = new Commlog();
            if (patients.Count != 0)  // Assume the first patient with a matching SMS is the right one
            {
                log.PatNum = patients[0].PatNum;
            }
            log.Note = "Text message received: " + msgText;
            log.Mode_ = CommItemMode.Text;
            log.CommDateTime = time;
            log.SentOrReceived = CommSentOrReceived.Received;
            log.CommType = Commlogs.GetTypeAuto(CommItemTypeAuto.TEXT);
            if (Security.CurUser != null)
            {
                log.UserNum = Security.CurUser.UserNum;
            }
            else
            {
                EventLog.WriteEntry("ODSMS", "Got logged out so can't check SMS", EventLogEntryType.Information, 101, 1, new byte[10]);
                return;
            }
            var sms = new SmsFromMobile();
            sms.CommlogNum = Commlogs.Insert(log);
            sms.MobilePhoneNumber = msgFrom;
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
            Console.WriteLine("Finished ODM New Text Message");


        }

        async public static void receiveSMS()
        {
            if (String.IsNullOrEmpty(auth))
            {
                throw new ArgumentException("ODSMS has not been initialised.");
            }

            string checkSMSstring = "http/request-received-messages?&order=newest&" + auth;
            const int getAllCount = 1000;

            bool folderExists = Directory.Exists(sms_folder_path);
            if (!folderExists)
            {
                EventLog.WriteEntry("ODSMS", "SMS MSG GUIDs folder not found - creating", EventLogEntryType.Warning, 101, 1, new byte[10]);
                Directory.CreateDirectory(sms_folder_path);
            }

            int count;
            int offset = 0;
            string removeStr;

            count = getAllCount;
            removeStr = "&remove=1";

            try
            {
                var request = checkSMSstring + "&limit=" + count.ToString() + removeStr;
                Console.WriteLine(request);
                var response = await sharedClient.GetAsync(request);

                var text = await response.Content.ReadAsStringAsync();
                //EventLog.WriteEntry("ODSMS", text, EventLogEntryType.Information);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(text);
                var list = xmlDoc.ChildNodes[1];        // the list of all SMS received (up to count)
                int c = 0; // Corrin: What is C?
                EventLog.WriteEntry("ODSMS", "About to loop through SMS", EventLogEntryType.Information, 101, 1, new byte[10]);

                if (list.ChildNodes.Count < offset)
                {
                    return;
                }
                foreach (XmlElement child in list.ChildNodes)
                {
                    Console.WriteLine("count");
                    Console.WriteLine(count.ToString());
                    if (c < offset)
                    {
                        c++;  // Skip the first offset messages
                        continue;
                    }
                    string msgFrom = child.ChildNodes[0].InnerText; // diafaan doesn't use xml properly so its not nice and looks like this.
                    string msgText = child.ChildNodes[2].InnerText;
                    string msgTime = child.ChildNodes[11].InnerText;
                    string msgGUID = child.ChildNodes[4].InnerText;
                    Console.WriteLine($"SMS from {msgFrom} at time {msgTime} with body {msgText} - GUID: {msgGUID}");

                    string guidFilePath = Path.Combine(sms_folder_path, msgGUID);

                    if (File.Exists(guidFilePath)) // True if we have we processed this message before
                    {
                        Console.WriteLine("Must've already processed this SMS");
                    }
                    else
                    {
                        try
                        {
                            processOneSMS(msgText, msgTime, msgFrom, msgGUID);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An exception occurred: {ex.Message}");
                            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                        }


                    }
                }
            }
            catch (Exception e)
            {
                MsgBox.Show("Receiving patient texts failed.");
                EventLog.WriteEntry("ODSMS", e.ToString(), EventLogEntryType.Error, 101, 1, new byte[10]);
            }

        }
        async public static void smsHourlyTasks()
        {


            while (Security.CurUser == null || Security.CurUser.UserNum == 0) // Assuming UserNum is an integer and 0 or null indicates uninitialized
            {
                Console.WriteLine("Waiting for user information to be initialized...");
                await Task.Delay(5000); // Wait for 5 seconds before checking again
            }

            while (!DataConnection.HasDatabaseConnection)
            {
                Console.WriteLine("Waiting for database connection...");
                await Task.Delay(5000); // Wait for 5 seconds before checking again
            }

            while (true)
            {
                Console.WriteLine("Performing hourly SMS related tasks");
                onceAnHour();
                await Task.Delay(60 * 60 * 1000); // Perform hourly tasks


            }
        }

        async public static void receiveSMSforever()
        {

            while (!DataConnection.HasDatabaseConnection)
            {
                Console.WriteLine("Waiting for database connection...");
                await Task.Delay(5000); // Wait for 5 seconds before checking again
            }

            while (Security.CurUser == null || Security.CurUser.UserNum == 0) // Assuming UserNum is an integer and 0 or null indicates uninitialized
            {
                Console.WriteLine("Waiting for user information to be initialized...");
                await Task.Delay(5000); // Wait for 5 seconds before checking again
            }

            while (true)
            {
                await Task.Delay(60 * 1000); // Check SMS once a minute
                Console.WriteLine("Checking for new SMS now");
                receiveSMS();
            }
        }
    }
}
