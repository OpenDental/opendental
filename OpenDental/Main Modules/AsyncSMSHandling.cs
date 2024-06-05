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
using System.Windows.Forms;
using System.Runtime.Serialization;

// Questions for Nathan
//
// We have Open Dental\ Main Modules\ Async SMS Handling
// This currently only has receive SMS async (and the Check SMS connection I wrote)
// Where to put the once-an-hour tasks (check for birthday texts, check SMS connection, check missing SMS)

namespace OpenDental.Main_Modules
{

    [Serializable]
    internal class DailySMSTasksException : Exception
    {
        public DailySMSTasksException()
        {
            LogException(this);
        }

        public DailySMSTasksException(string message) : base(message)
        {
            LogException(this);
        }

        public DailySMSTasksException(string message, Exception innerException) : base(message, innerException)
        {
            LogException(this);
        }

        protected DailySMSTasksException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            LogException(this);
        }

        private static void LogException(Exception ex)
        {
            string eventSource = "ODSMS";
            string logMessage = $"Exception occurred: {ex.Message}";

            if (!EventLog.SourceExists(eventSource))
            {
                EventLog.CreateEventSource(eventSource, "Application");
            }

            EventLog.WriteEntry(eventSource, logMessage, EventLogEntryType.Error);
        }
    }

    internal static class AsyncSMSHandling
    {
        private static HttpClient sharedClient = ODSMS.sharedClient;
        private static string auth = ODSMS.AUTH;
        private static String sms_folder_path = @"L:\msg_guids\";

        public enum ReminderFilterType
        {
            OneDay,
            OneWeek,
            TwoWeeks
        }

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
            catch (DailySMSTasksException ex)
            {
                Console.WriteLine($"Error checking Diafaan status: {ex.Message}");
                EventLog.WriteEntry("ODSMS", "Something bad happened checking Diafaan status", EventLogEntryType.Error, 101, 1, new byte[10]);
            }

            return false;
            // We're looking for a line like: <Gateway Name="GSM Modem Gateway" Id="1" Active="1" Available="1" SendEnabled="1" ReceiveEnabled="1">

        }

        async public static void PerformDailySMSTasks()
        {
            bool smsIsWorking = await CheckSMSConnection();

            if (smsIsWorking)
            {
                // SMS is working
                if (ODSMS.wasSmsBroken)
                {
                    // SMS was previously broken but is now restored
                    MsgBox.Show("SMS is restored. Birthday texts and reminders will be sent now.");
                    EventLog.WriteEntry("ODSMS", "SMS has been restored", EventLogEntryType.Information, 101, 1, new byte[10]);
                    ODSMS.USE_ODSMS = true;
                    sendReminderTexts();
                    sendBirthdayTexts();
                    // receiveSMS();  // Let's leave this to the 5 minute job
                }
                else if (ODSMS.initialStartup)
                {
                    // Initial startup and SMS is working
                    EventLog.WriteEntry("ODSMS", "SMS is working during initial startup", EventLogEntryType.Information, 101, 1, new byte[10]);
                    ODSMS.USE_ODSMS = true;
                    ODSMS.initialStartup = false;
                }

                sendReminderTexts();
                sendBirthdayTexts();
               // receiveSMS();  // Let's leave this to the 5 minute job
            }
            else
            {
                // SMS is not working
                if (ODSMS.wasSmsBroken)
                {
                    // SMS was already broken
                    Console.WriteLine("SMS continues to be down");
                    EventLog.WriteEntry("ODSMS", "SMS is still down", EventLogEntryType.Warning, 101, 1, new byte[10]);
                }
                else
                {
                    // SMS has just gone down
                    MsgBox.Show("Failure checking Diafaan. SMS functionality is currently unavailable. Please check the GSM Modem Gateway and ensure Wi-Fi is enabled. Restart Open Dental to re-attempt connection.");
                    EventLog.WriteEntry("ODSMS", "SMS is down", EventLogEntryType.Error, 101, 1, new byte[10]);
                    ODSMS.USE_ODSMS = false;
                }
            }

            // Update the previous state for the next call
            ODSMS.wasSmsBroken = !smsIsWorking;
        }

        async public static void OLDonceAnHour()
        {
            bool smsIsWorking = await CheckSMSConnection();

                if (smsIsWorking)

            if (smsIsWorking == false)
            {
                if (ODSMS.wasSmsBroken == true)
                {
                    MsgBox.Show("SMS is restored.  Birthday texts and reminders will be sent now.");
                    EventLog.WriteEntry("ODSMS", "SMS has been restored", EventLogEntryType.Information, 101, 1, new byte[10]);
                    ODSMS.USE_ODSMS = true;
                }
                sendReminderTexts();
                sendBirthdayTexts();
                receiveSMS();
            }
            else
            {
                if (ODSMS.wasSmsBroken == false)
                {
                    MsgBox.Show("Failure checking Diafaan. SMS functionality is currently unavailable. Please check the GSM Modem Gateway and ensure Wi-Fi is enabled. Restart Open Dental to re-attempt connection.");
                    EventLog.WriteEntry("ODSMS", "SMS is down", EventLogEntryType.Error, 101, 1, new byte[10]);
                    ODSMS.USE_ODSMS = false;
                }
                else
                {
                    Console.WriteLine("SMS continues to be down");
                    EventLog.WriteEntry("ODSMS", "SMS is still down", EventLogEntryType.Warning, 101, 1, new byte[10]);
                }
            }

            // Update the previous state for the next call
            ODSMS.wasSmsBroken = !smsIsWorking;
        }

        public static string renderReminder(string reminderTemplate, Patient p, Appointment a)
        {
            string s = reminderTemplate
                .Replace("[NamePreferredOrFirst]", p.GetNameFirstOrPreferred())
                .Replace("?NamePreferredOrFirst", p.GetNameFirstOrPreferred())
                .Replace("[FName]", p.FName)
                .Replace("?FName", p.FName);

            if (a != null)
            {
                s = s.Replace("[date]", a.AptDateTime.ToString("d MMMM yyyy"))
                     .Replace("[time]", a.AptDateTime.ToString("HH:mm"));
            }
            return s;
        }
        private static void sendReminderTexts()
        {
            var currentTime = DateTime.Now; // Using local time
            if (currentTime.Hour < 7)
            {
                return; // It's before 7 AM, do not proceed
            }

            // Get all enum values
            var enumValues = Enum.GetValues(typeof(ReminderFilterType));

            // Iterate over enum values
            foreach (ReminderFilterType enumValue in enumValues)
            {
                // Call GetPatientsWithAppointmentsTwoWeeks with each value
                List<PatientAppointment> patientsNeedingApptReminder  = GetPatientsWithAppointmentsTwoWeeks(enumValue);

                // Lookup the appropriate value from the database
                string reminderMessageTemplate;
                switch (enumValue)
                {
                    case ReminderFilterType.OneDay:
                        reminderMessageTemplate = PrefC.GetString(PrefName.ConfirmTextMessage);
                        break;
                    case ReminderFilterType.OneWeek:
                        reminderMessageTemplate = PrefC.GetString(PrefName.ConfirmPostcardMessage);
                        break;
                    case ReminderFilterType.TwoWeeks:
                        reminderMessageTemplate = PrefC.GetString(PrefName.ConfirmPostcardFamMessage);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // Prepare SMS messages
                List<SmsToMobile> messagesToSend = patientsNeedingApptReminder.Select(pat_appt =>
                    new SmsToMobile
                    {
                        PatNum = pat_appt.Patient.PatNum, // Assuming Patient class has PatNum property
                        SmsPhoneNumber = "+64275598699", // Set to your sending phone number in international format
                        MobilePhoneNumber = pat_appt.Patient.WirelessPhone, // Assuming Patient class has PhoneNumber property
                        MsgText = renderReminder(reminderMessageTemplate, pat_appt.Patient, pat_appt.Appointment),
                        MsgType = SmsMessageSource.Reminder, 
                        SmsStatus = SmsDeliveryStatus.Pending, 
                        MsgParts = 1, // Assuming the message fits into one part
                                      // Set any other necessary properties
                    }).ToList();

                if (messagesToSend.Any())
                {
                    foreach (var sms in messagesToSend)
                    {
                        EventLog.WriteEntry("ODSMS", $"To: {sms.MobilePhoneNumber}, Message: {sms.MsgText}", EventLogEntryType.Information, 101, 1, new byte[10]);
                    }
                    if (ODSMS.SEND_SMS)
                    {
                        SmsToMobiles.SendSmsMany(messagesToSend);
                    }
                    else
                    {
                        EventLog.WriteEntry("ODSMS", "SMS sending is disabled. Not sending any messages", EventLogEntryType.Warning, 101, 1, new byte[10]);

                    }
                }

            }
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
                        MsgText = birthdayMessageTemplate.Replace("[?NamePreferredOrFirst]", patient.GetNameFirstOrPreferred()).Replace("[?FName]", patient.FName),
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
                if (ODSMS.SEND_SMS)
                {
                    SmsToMobiles.SendSmsMany(messagesToSend);
                }
                else
                {
                    Console.WriteLine("SMS sending is disabled. Not sending any messages.");
                }
            }

        }

        public static List<PatientAppointment> GetPatientsWithAppointmentsTwoWeeks(ReminderFilterType filterType)
        {
            /* I can't get this working
             *  long defNumAppointmentConfirmed = _listDefsApptConfirmed.FirstOrDefault(d => d.ItemName.ToLower() == "appointment confirmed")?.DefNum ?? 0;
            */

            List<Def> _listDefsApptConfirmed = Defs.GetDefsForCategory(DefCat.ApptConfirmed, isShort: true);
            int textMessageValue = (int)ContactMethod.TextMessage;
            int noPreferenceValue = (int)ContactMethod.None;

            // Get the DefNum for the "Unconfirmed" status
            long defNumUnconfirmed = _listDefsApptConfirmed.FirstOrDefault(d => d.ItemName.ToLower() == "not called")?.DefNum ?? 0;
            string aptDateTimeRange;

            switch (filterType)
            {
                case ReminderFilterType.OneDay:
                    aptDateTimeRange = $"DATE(a.AptDateTime) = DATE(DATE_ADD(NOW(), INTERVAL 1 DAY))";
                    break;
                case ReminderFilterType.OneWeek:
                    aptDateTimeRange = $"DATE(a.AptDateTime) = DATE(DATE_ADD(NOW(), INTERVAL 1 WEEK))";
                    break;
                case ReminderFilterType.TwoWeeks:
                    aptDateTimeRange = $"DATE(a.AptDateTime) = DATE(DATE_ADD(NOW(), INTERVAL 2 WEEK))";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterType), filterType, "Invalid ReminderFilterType value.");
            }

            string select = "SELECT p.*, a.* ";
            string from = "FROM patient AS p JOIN Appointment as a using (PatNum) ";
            string where_true = "WHERE TRUE ";
           //  string where_active = $"AND p.PatStatus IN ({(int)PatientStatus.Patient}) "; // Do not filter for active patients if they have an appointment
            string where_allow_sms = $"AND p.TxtMsgOk < 2 "; // Only text patients with SMS permission
            string where_confirm_not_sms = $"AND p.PreferConfirmMethod IN  ({noPreferenceValue}, {textMessageValue}) "; // Prefearred confirm method = SMS OR NOT SET


            // Filter to exclude patients who have been contacted in the last 24 hours
            // Note: This is both unnecessary and can lead to bugs
            // It refuses to send a reminder if the patient has been contacted in the last day.
            // It does that as a safety net.  Let's say the update to set the appointmnet status to texted fails  
            //string where_not_contacted = "AND NOT EXISTS (" +
            //                             "SELECT 1 " +
            //                             "FROM CommLog m " +
            //                             "WHERE m.PatNum = p.PatNum " +
            //                             "AND m.Note LIKE 'Text message sent%reminder%' " +
            //                             "AND m.CommType = m.CommType " + // // TODO: Something like m.CommType = Commlogs.GetTypeAuto(CommItemTypeAuto.APPT);
            //                             "AND m.CommDateTime > DATE_SUB(NOW(), INTERVAL 1 DAY)) ";

            string where_mobile_phone = "AND LENGTH(COALESCE(p.WirelessPhone,'')) > 7 ";

            string where_appointment = $"AND ({aptDateTimeRange} AND a.Confirmed = {defNumUnconfirmed}) ";

            // Combine all parts to form the final command
            string command = select + from + where_true + where_appointment + where_mobile_phone + where_allow_sms + where_confirm_not_sms;
            Console.WriteLine(command);
            List<PatientAppointment> listPatAppts = OpenDentBusiness.Crud.PatientApptCrud.SelectMany(command);
            return listPatAppts;
        }

        public static List<Patient> GetPatientsWithBirthdayToday()
        {
            string select = "SELECT p.* ";
            string from = "FROM patient AS p ";
            // Start with a WHERE TRUE to allow all subsequent conditions to use AND
            string where_true = "WHERE TRUE ";
            string where_active = $"AND p.PatStatus IN ({(int)PatientStatus.Patient}) "; // Only text active patients
            string where_allow_sms = $"AND p.TxtMsgOk < 2 "; // Do not text people that have opted out

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
            string command = select + from + where_true + where_active + where_birthday + where_not_contacted + where_allow_sms + where_mobile_phone;
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
            log.UserNum = 1; // Corrin 2024-04-25.  This is the admin user
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


            if (msgText.ToUpper() == "YES" || msgText.ToUpper() == "Y")
            {
                if (patients.Count == 1)  // Only consider automated replies if a single patient matches
                {
                    bool wasHandled = handleAutomatedConfirmation(patients[0]);
                    if (wasHandled) 
                        sms.SmsStatus = SmsFromStatus.ReceivedRead;
                } else
                {
                    EventLog.WriteEntry("ODSMS", "'YES' received matching multiple patients - process manually", EventLogEntryType.Information, 101, 1, new byte[10]);

                }
            }

            OpenDentBusiness.SmsFromMobiles.Insert(sms);


            //Alert ODMobile where applicable.
            PushNotificationUtils.ODM_NewTextMessage(sms, sms.PatNum);
            Console.WriteLine("Finished ODM New Text Message");
        }

        private static bool handleAutomatedConfirmation(Patient p)
        {
            // Patient p has just texted YES.  Let's see the most recent text to work out what they're confirming
            // We need to find the appointment that they're confirming
            // and mark it as confirmed
            // If we are unsure then it's safer to return false and leave it to the receptionist

            long PatNum = p.PatNum;

            // Firstly we want to get the most recent text we sent.  That includes the appointment details
            // Then we need to look in Appointment for that appointment and make sure it matches

            string latestSMS = "SELECT * from CommLog where PatNum = " + PatNum + " AND Note LIKE 'Text message sent%reminder%' ORDER BY CommDateTime DESC LIMIT 1";
            Commlog latestComm = OpenDentBusiness.Crud.CommlogCrud.SelectOne(latestSMS);
            string test_message = "Hi Shelly , Your dental appointment is on Friday, 31 May 2024 at 2:00 pm at Massey Smiles. Please reply \"YES\" ASAP to confirm  (or call 09 833 8182 straight away if there are any issues).  Please reschedule if you are at all unwell.  We're looking forward to seeing you. Massey Smiles";
            DateTime appointmentTime;
            // Not called
            //
            // If you send a text messasge
            // then you change the appointment status to texted

            if (latestComm != null)
            {
                string pattern = "on ddd, d MMM yyyy at h:mm tt";

                string dateStr = latestComm.Note.Substring(latestComm.Note.IndexOf("on"));

                try
                {
                    appointmentTime = DateTime.ParseExact(dateStr, pattern, null);
                    Console.WriteLine(appointmentTime);
                    // TODO: Mark as 2 week confirmed
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Error parsing date: " + ex.Message);
                    EventLog.WriteEntry("ODSMS", $"Error parsing date from communication log note: {ex.Message}", EventLogEntryType.Error, 101, 1, new byte[10]);
                }
            }
            else
            {
                string logMessage = $"'Yes' received, but no matching appointment found for patient {PatNum}.";
                EventLog.WriteEntry("ODSMS", logMessage, EventLogEntryType.Warning, 101, 1, new byte[10]);
            }



            // 
            //             string where_appointment = $"AND EXISTS (SELECT 1 FROM Appointment AS a WHERE p.PatNum = a.PatNum AND {aptDateTimeRange} AND a.Confirmed = a.Confirmed) ";

            return false;
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
                MsgBox.Show("SMS folder not found - creating.  If this is at the practice then quit OpenDental and contact Corrin");
                Directory.CreateDirectory(sms_folder_path);
            }

            int count;
            int offset = 0;
            string removeStr;
            string logMessage;

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
                    logMessage = $"SMS from {msgFrom} at time {msgTime} with body {msgText} - GUID: {msgGUID}";
                    Console.WriteLine(logMessage);
                    EventLog.WriteEntry("ODSMS", logMessage, EventLogEntryType.Information, 101, 1);


                    string guidFilePath = Path.Combine(sms_folder_path, msgGUID);

                    if (File.Exists(guidFilePath)) // True if we have we processed this message before
                    {
                        string errorMessage = "Must've already processed this SMS";
                        Console.WriteLine(errorMessage);
                        EventLog.WriteEntry("ODSMS", errorMessage, EventLogEntryType.Warning, 101, 1, new byte[10]);
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
                            logMessage = $"An exception occurred: {ex.Message}\nStack Trace: {ex.StackTrace}";
                            EventLog.WriteEntry("ODSMS", logMessage, EventLogEntryType.Error, 101, 1);
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

        async public static void smsDebugTasks()
        {
            PerformDailySMSTasks();
            await Task.Delay(TimeSpan.FromMinutes(1));
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

            bool todaysJobsSucceeded = false;
            DateTime lastRunDate = DateTime.MinValue;

            while (true)
            {

                DateTime currentTime = DateTime.Now;
                bool isNewDay = currentTime.Date != lastRunDate.Date;


                if (isNewDay)
                {
                    todaysJobsSucceeded = false;
                    lastRunDate = currentTime;

                    if (!todaysJobsSucceeded && currentTime.Hour >= 8)
                    {
                        Console.WriteLine("Performing daily SMS related tasks");
                        EventLog.WriteEntry("ODSMS", "Performing daily SMS related tasks", EventLogEntryType.Information, 101, 1, new byte[10]);

                        try
                        {
                            // Perform your daily SMS tasks here
                            PerformDailySMSTasks();

                            todaysJobsSucceeded = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error performing daily SMS tasks: {ex.Message}");
                            EventLog.WriteEntry("ODSMS", $"Error performing daily SMS tasks: {ex.Message}", EventLogEntryType.Error, 101, 1, new byte[10]);
                        }
                    }

                    // normally we want to wait an hour between SMS tasks (reminding patients, birthdays, etc).  But...
                    // if SMS was down then we switch to checking that every minute in case it's come back up


                    bool shouldCheckFrequently = !ODSMS.USE_ODSMS && ODSMS.wasSmsBroken;

                    // Decide the delay based on whether the SMS service was working and now is not
                    int delayMinutes = shouldCheckFrequently ? 1 : 60;  // Check every minute if SMS was working and now isn't, else check every hour

                    await Task.Delay(TimeSpan.FromMinutes(delayMinutes));
                }
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
