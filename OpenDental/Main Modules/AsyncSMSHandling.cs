using OpenDentBusiness;

using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml;
using Task = System.Threading.Tasks.Task;
using System.Diagnostics;
using OpenDentBusiness.Crud;

using System.Collections.Generic;
using System.Linq;
using DataConnectionBase;
using System.Runtime.Serialization;
using CodeBase;

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
        private static List<Def> _listDefsApptConfirmed;
        private static long _defNumTwoWeekConfirmed;
        private static long _defNumOneWeekConfirmed;
        private static long _defNumConfirmed;
        private static long _defNumNotCalled;
        private static long _defNumUnconfirmed;
        private static long _defNumTwoWeekSent;
        private static long _defNumOneWeekSent;
        private static long _defNumTexted;

        /// <summary>
        /// This sets up the constants which map 'texted' to the Appointment Confirmed category for texted, etc.
        /// </summary>
        /// <returns></returns>
        public static async Task InitializeAsyncSMSHandling()
        {

            while (!DataConnection.HasDatabaseConnection)
            {
                Console.WriteLine("Waiting for database connection...");
                await Task.Delay(5000); // Wait for 5 seconds before checking again
            }

            _listDefsApptConfirmed = Defs.GetDefsForCategory(DefCat.ApptConfirmed, isShort: true);
            _defNumTexted = _listDefsApptConfirmed.FirstOrDefault(d => d.ItemName.ToLower() == "texted")?.DefNum ?? 0;
            _defNumTwoWeekSent = _listDefsApptConfirmed.FirstOrDefault(d => d.ItemName.ToLower() == "2 week sent")?.DefNum ?? 0;
            _defNumOneWeekSent = _listDefsApptConfirmed.FirstOrDefault(d => d.ItemName.ToLower() == "1 week sent")?.DefNum ?? 0;
            _defNumTwoWeekConfirmed = _listDefsApptConfirmed.FirstOrDefault(d => d.ItemName.ToLower() == "2 week confirmed")?.DefNum ?? 0;
            _defNumOneWeekConfirmed = _listDefsApptConfirmed.FirstOrDefault(d => d.ItemName.ToLower() == "1 week confirmed")?.DefNum ?? 0;
            _defNumConfirmed = _listDefsApptConfirmed.FirstOrDefault(d => d.ItemName.ToLower() == "appointment confirmed")?.DefNum ?? 0;
            _defNumNotCalled = _listDefsApptConfirmed.FirstOrDefault(d => d.ItemName.ToLower() == "not called")?.DefNum ?? 0;
            _defNumUnconfirmed = _listDefsApptConfirmed.FirstOrDefault(d => d.ItemName.ToLower() == "unconfirmed")?.DefNum ?? 0;


            checkAppointmentTypeFound("2 week confirmed", _defNumTwoWeekConfirmed);
            checkAppointmentTypeFound("1 week confirmed", _defNumOneWeekConfirmed);
            checkAppointmentTypeFound("Appointment Confirmed", _defNumConfirmed);
            checkAppointmentTypeFound("2 week sent", _defNumTwoWeekConfirmed);
            checkAppointmentTypeFound("1 week sent", _defNumOneWeekConfirmed);
            checkAppointmentTypeFound("texted", _defNumTexted);
            checkAppointmentTypeFound("not called", _defNumNotCalled);
            checkAppointmentTypeFound("unconfirmed", _defNumUnconfirmed);
        }


        public enum ReminderFilterType
        {
            OneDay,
            OneWeek,
            TwoWeeks
        }

        /// <summary>
        /// This queries Diafaan to ask if the modem is connected.  IF not it puts up an alert and stops sending/receiving texts.
        /// </summary>
        /// <returns></returns>
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
                    EventLog.WriteEntry("ODSMS", "GSM Modem Gateway not found", EventLogEntryType.Error, 101, 1, new byte[10]);

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

        /// <summary>
        /// This sends out the daily reminders and birthday texts.
        /// It's called once an hour from SMSDailyTasks.  I.e. it's the expensive part of the hourly loop
        /// </summary>
        async public static void PerformDailySMSTasks()
        {
            bool smsIsWorking = await CheckSMSConnection();
            bool remindersSent = false;
            bool birthdaySent = false;

            if (smsIsWorking)
            {
                // SMS is working
                if (ODSMS.wasSmsBroken)
                {
                    // SMS was previously broken but is now restored
                    MsgBox.Show("SMS is restored. Birthday texts and reminders will be sent now.");
                    EventLog.WriteEntry("ODSMS", "SMS has been restored", EventLogEntryType.Information, 101, 1, new byte[10]);
                    ODSMS.USE_ODSMS = true;
                }
                else if (ODSMS.initialStartup)
                {
                    // Initial startup and SMS is working
                    EventLog.WriteEntry("ODSMS", "SMS is working during initial startup", EventLogEntryType.Information, 101, 1, new byte[10]);
                    ODSMS.USE_ODSMS = true;
                    ODSMS.initialStartup = false;
                }

                remindersSent = SendReminderTexts();
                birthdaySent = SendBirthdayTexts(); // Yes, I'm ignoring hte return value for now

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
                    MsgBox.Show("Failure checking Diafaan! SMS will not send or receive until fixed.  Please go to the phone and check the GSM Modem Gateway app and ensure Wi-Fi is enabled. Failing that, check Diafaan manually.");
                    EventLog.WriteEntry("ODSMS", "SMS is down", EventLogEntryType.Error, 101, 1, new byte[10]);
                    ODSMS.USE_ODSMS = false;
                }
            }

            // Update the previous state for the next call
            ODSMS.wasSmsBroken = !smsIsWorking;
            return;
        }

        /// <summary>
        /// Friendly helper function, should really go to a different file.
        /// Replaces [?FName] with the patient's first name, etc.
        /// </summary>
        /// <param name="reminderTemplate"></param>
        /// <param name="p">The patient.  Fields like name, DOB, etc</param>
        /// <param name="a">The appointment.  Fields like time, provider, etc.</param>
        /// <returns></returns>
        public static string renderReminder(string reminderTemplate, Patient p, Appointment a)
        {
            string s = reminderTemplate
                .Replace("[NamePreferredOrFirst]", p.GetNameFirstOrPreferred())
                .Replace("?NamePreferredOrFirst", p.GetNameFirstOrPreferred())
                .Replace("[FName]", p.FName)
                .Replace("?FName", p.FName);

            if (a != null)
            {
                s = s.Replace("[date]", a.AptDateTime.ToString("dddd, d MMMM yyyy"))
                     .Replace("[time]", a.AptDateTime.ToString("h:mm tt"));
            }
            return s;
        }

        /// <summary>
        /// Helper function.  Checks that we were able to get a value from the database.  So that if one is renamed etc then we break very obviously
        /// </summary>
        /// <param name="reminderType"></param>
        /// <param name="preferenceName"></param>
        /// <exception cref="Exception"></exception>
        private static void GetReminderTemplateCheck(string reminderType, string preferenceName)
        {
            string errorMessage = $"The {reminderType} reminder text ({preferenceName}) is missing or invalid";
            EventLog.WriteEntry("ODSMS", errorMessage, EventLogEntryType.Error, 101, 1, new byte[10]);
            MsgBox.Show(errorMessage);
            throw new Exception(errorMessage); // or raiseError, depending on your framework
        }

        /// <summary>
        /// This is where the logic around sending reminders is handled.  Loops through 2w, 1w and 1d, and calls SendSmsMany.  Then marks the appointment confirmaton status as sent
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static bool SendReminderTexts()
        {
            var currentTime = DateTime.Now; // Using local time
            var patientsTexted = false;  // Have we successfully set patients to texted?

            if (currentTime.Hour < 7)
            {
                return patientsTexted; // It's before 7 AM, do not proceed
            }

            // Get all enum values
            var potentialReminderMessages = Enum.GetValues(typeof(ReminderFilterType));

            // Iterate over enum values
            foreach (ReminderFilterType currentReminder in potentialReminderMessages)
            {
                // Call GetPatientsWithAppointmentsTwoWeeks with each value
                List<PatientAppointment> patientsNeedingApptReminder  = GetPatientsWithAppointmentsTwoWeeks(currentReminder);

                // Lookup the appropriate value from the database
                string reminderMessageTemplate;
                switch (currentReminder)
                {
                    case ReminderFilterType.OneDay:
                        reminderMessageTemplate = PrefC.GetString(PrefName.ConfirmTextMessage);
                        if (reminderMessageTemplate.Length < 10)
                        {
                            GetReminderTemplateCheck("One Day", "ConfirmTextMessage");
                        }
                        break;
                    case ReminderFilterType.OneWeek:
                        reminderMessageTemplate = PrefC.GetString(PrefName.ConfirmPostcardMessage);
                        if (reminderMessageTemplate.Length < 10)
                        {
                            GetReminderTemplateCheck("One Week", "ConfirmPostcardMessage");
                        }
                        break;
                    case ReminderFilterType.TwoWeeks:
                        reminderMessageTemplate = PrefC.GetString(PrefName.ConfirmPostcardFamMessage);
                        if (reminderMessageTemplate.Length < 10)
                        {
                            GetReminderTemplateCheck("Two Weeks", "ConfirmPostcardFamMessage");
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // Prepare SMS messages
                List<SmsToMobile> messagesToSend = patientsNeedingApptReminder.Select(pat_appt =>
                    new SmsToMobile
                    {
                        PatNum = pat_appt.Patient.PatNum, // Assuming Patient class has PatNum property
                        SmsPhoneNumber = ODSMS.PRACTICE_PHONE_NUMBER,
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
                        List<SmsToMobile> sentMessages = SmsToMobiles.SendSmsMany(listMessages: messagesToSend, makeCommLog: ODSMS.WRITE_TO_DATABASE);
                        List<Appointment> appts = patientsNeedingApptReminder
                            .Where(patapt => sentMessages.Any(msg => msg.PatNum == patapt.Patient.PatNum &&
                                (msg.SmsStatus == SmsDeliveryStatus.Pending ||
                                 msg.SmsStatus == SmsDeliveryStatus.DeliveryConf ||
                                 msg.SmsStatus == SmsDeliveryStatus.DeliveryUnconf)))
                            .Select(patapt => patapt.Appointment)
                            .ToList();

                        foreach (Appointment originalAppt in appts)
                        {
                            Appointment updatedAppt = originalAppt.Copy();
                            if (currentReminder == ReminderFilterType.OneDay)
                            {
                                updatedAppt.Confirmed = (int)AsyncSMSHandling._defNumTexted;
                            }
                            else if (currentReminder == ReminderFilterType.OneWeek)
                            {
                                updatedAppt.Confirmed = (int)AsyncSMSHandling._defNumOneWeekSent;
                            }
                            else if (currentReminder == ReminderFilterType.TwoWeeks)
                            {
                                updatedAppt.Confirmed = (int)AsyncSMSHandling._defNumTwoWeekSent;
                            }

                            bool updateSucceeded = true;

                            if (ODSMS.WRITE_TO_DATABASE)
                            {
                                updateSucceeded = AppointmentCrud.Update(updatedAppt, originalAppt);
                            } 
                            else
                            {
                                EventLog.WriteEntry("ODSMS", $"Not updating appointment {originalAppt.AptNum} on patient {originalAppt.PatNum} from {originalAppt.Confirmed} to {updatedAppt.Confirmed} as running in debug mode", EventLogEntryType.Warning, 101, 1, new byte[10]);
                            }
                            if (updateSucceeded)
                            {
                                patientsTexted = true;
                            }
                            else
                            {
                                EventLog.WriteEntry("ODSMS", "Failure updating patient details!", EventLogEntryType.Warning, 101, 1, new byte[10]);
                            }
                        }
                    }
                    else
                    {
                        EventLog.WriteEntry("ODSMS", "SMS sending is disabled. Not sending any messages", EventLogEntryType.Warning, 101, 1, new byte[10]);

                    }
                }

            }
            return patientsTexted; 
        }

        /// <summary>
        /// THis sends the birthday messages. Similar to SendReminderTexts but simpler.
        /// </summary>
        /// <returns></returns>
        private static bool SendBirthdayTexts()
        {
            // Step 1: Check time
            var currentTime = DateTime.Now; // Using local time
            var birthdaySent = false;

            if (currentTime.Hour < 7)
            {
                return birthdaySent; // It's before 7 AM, do not proceed
            }

            string birthdayMessageTemplate = PrefC.GetString(PrefName.BirthdayPostcardMsg);


            // Step 2: Find patients with birthdays today
            var patientsWithBirthday = GetPatientsWithBirthdayToday();

            // Step 3: Prepare SMS messages
            List<SmsToMobile> messagesToSend = patientsWithBirthday.Select(patient =>
                    new SmsToMobile
                    {
                        PatNum = patient.PatNum, // Assuming Patient class has PatNum property
                        SmsPhoneNumber = ODSMS.PRACTICE_PHONE_NUMBER, // Set to your sending phone number in international format
                        MobilePhoneNumber = patient.WirelessPhone, // Assuming Patient class has PhoneNumber property
                        MsgText = renderReminder(birthdayMessageTemplate, patient, null),
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
                    birthdaySent = true;
                }
                else
                {
                    Console.WriteLine("SMS sending is disabled. Not sending any messages.");
                }
            }
            return birthdaySent; // It's before 7 AM, do not proceed

        }

        /// <summary>
        /// A helper function for SendReminderTexts.  This queries the database for people with upcoming appointments.
        /// </summary>
        /// <param name="filterType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<PatientAppointment> GetPatientsWithAppointmentsTwoWeeks(ReminderFilterType filterType)
        {
           
            int textMessageValue = (int)ContactMethod.TextMessage;
            int noPreferenceValue = (int)ContactMethod.None;

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


            string where_no_intermediate_appointments = $"AND NOT EXISTS (SELECT 1 FROM Appointment a2 WHERE a2.AptDateTime > NOW() AND a2.AptDateTime < a.AptDateTime AND a2.PatNum = a.PatNum) ";

            string where_mobile_phone = "AND LENGTH(COALESCE(p.WirelessPhone,'')) > 7 ";

            string where_appointment_date = $"AND {aptDateTimeRange} ";
            string where_appointment_confirmed = "" ;
            
            if (filterType == ReminderFilterType.OneDay)
            {
                where_appointment_confirmed = $"AND a.Confirmed IN ({AsyncSMSHandling._defNumNotCalled}, {AsyncSMSHandling._defNumUnconfirmed}, {AsyncSMSHandling._defNumOneWeekConfirmed}, {AsyncSMSHandling._defNumTwoWeekConfirmed}, {AsyncSMSHandling._defNumOneWeekSent}, {AsyncSMSHandling._defNumTwoWeekSent}) ";
            }
            else if (filterType == ReminderFilterType.OneWeek)
            {
                where_appointment_confirmed = $"AND a.Confirmed IN ({AsyncSMSHandling._defNumNotCalled}, {AsyncSMSHandling._defNumUnconfirmed}, {AsyncSMSHandling._defNumTwoWeekConfirmed}, {AsyncSMSHandling._defNumTwoWeekSent}) ";
            }
            else if (filterType == ReminderFilterType.TwoWeeks)
            {
                where_appointment_confirmed = $"AND a.Confirmed IN ({AsyncSMSHandling._defNumNotCalled}, {AsyncSMSHandling._defNumUnconfirmed}) ";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(filterType), filterType, "Invalid ReminderFilterType value.");
            }


            
            // Combine all parts to form the final command
            string command = select + from + where_true + where_appointment_date + where_appointment_confirmed + where_mobile_phone + where_allow_sms + where_confirm_not_sms + where_no_intermediate_appointments;
            Console.WriteLine(command);
            List<PatientAppointment> listPatAppts = OpenDentBusiness.Crud.PatientApptCrud.SelectMany(command);
            return listPatAppts;
        }

        /// <summary>
        /// A helper function for SendReminderTexts. This queries the databaes for people with a birthday today.
        /// </summary>
        /// <returns></returns>        
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

        /// <summary>
        /// This is the main function that receives a single SMS.  It writes the SMS to a file, then looks up the patient and creates a CommLog and SmsFromMobile entry.
        /// </summary>
        /// <param name="msgText"></param>
        /// <param name="msgTime"></param>
        /// <param name="msgFrom"></param>
        /// <param name="msgGUID"></param>
        /// <returns></returns>
        async public static Task processOneReceivedSMS(string msgText, string msgTime, string msgFrom, string msgGUID)
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
            sms.Flags = " ";

            if (msgText.ToUpper() == "YES" || msgText.ToUpper() == "Y")
            {
                if (patients.Count < 10)  // Only consider automated replies if a single patient matches
                {
                    bool wasHandled = handleAutomatedConfirmation(patients);
                    if (wasHandled)
                    {
                        sms.SmsStatus = SmsFromStatus.ReceivedRead;
                    }
                    else
                    {
                        // We're not sure what they're confirming, so we'll ask them to call the practice
                        string matchSMSmessage = "Thank you for your reply but we've had trouble matching it to an appointment. Could you please give us a call?";
                        SmsToMobile matchSMS = new SmsToMobile
                        {
                            PatNum = patients[0].PatNum,
                            SmsPhoneNumber = ODSMS.PRACTICE_PHONE_NUMBER,
                            MobilePhoneNumber = msgFrom,
                            MsgText = matchSMSmessage,
                            MsgType = SmsMessageSource.GeneralMessage,
                            SmsStatus = SmsDeliveryStatus.Pending,
                            MsgParts = 1
                        };
                        await SmsToMobiles.SendSmsMessageAsync(matchSMS);
                    }
                } else
                {
                    EventLog.WriteEntry("ODSMS", "'YES' received matching more than ten patients - process manually", EventLogEntryType.Information, 101, 1, new byte[10]);

                }
            }

            OpenDentBusiness.SmsFromMobiles.Insert(sms);

            // Temporary - re-enable!!

            //Alert ODMobile where applicable.
            // PushNotificationUtils.ODM_NewTextMessage(sms, sms.PatNum);
            Console.WriteLine("Finished OD New Text Message");
        }

        /// <summary>
        /// Another helper function to check the appointment type doesn't have a typo or has been renamed.  Similar to GetReminderTemplateCheck
        /// </summary>
        /// <param name="name"></param>
        /// <param name="status"></param>
        /// <exception cref="Exception"></exception>
        private static void checkAppointmentTypeFound(string name, long status)
        {
            if (status == 0)
            {
                string s = $"The '{name}' appointment status was not found.";
                EventLog.WriteEntry("ODSMS", s, EventLogEntryType.Error, 101, 1, new byte[10]);
                MsgBox.Show(s);
                throw new Exception(s); // or raiseError, depending on your framework
            }
        }

        /// <summary>
        /// If attempts to update the appointment status to Confirmed, 1w_confirmed, etc when a patient texts back yes
        /// Complications come because we need to use the text to work out which appointment is referenced.
        /// </summary>
        /// <param name="patientList"></param>
        /// <returns></returns>
        private static bool handleAutomatedConfirmation(List<Patient> patientList)
        {
            // One of hte patients in patientLIst has just texted YES. Let's see the most recent text to work out what they're confirming
            // We need to find the appointment that they're confirming and mark it as confirmed
            // If we are unsure then it's safer to return false and leave it to the receptionist

            string patNums = String.Join(",", patientList.Select(p => p.PatNum.ToString()).ToArray());




            // Firstly we want to get the most recent text we sent that includes the appointment details
            // Then we need to look in Appointment for that appointment and make sure it matches

            string latestSMS = "SELECT * from CommLog where PatNum IN (" + patNums + ") AND Note REGEXP 'Text message sent.*(reply|respond).*YES' AND CommDateTime >= DATE_SUB(NOW(), INTERVAL 21 DAY) ORDER BY CommDateTime DESC LIMIT 1";
            Commlog latestComm = OpenDentBusiness.Crud.CommlogCrud.SelectOne(latestSMS);

            if (latestComm != null)
            {
                long PatNum = latestComm.PatNum;
                Patient p = patientList.FirstOrDefault(p => p.PatNum == latestComm.PatNum);
                if (p != null)
                {
                    // Patient found, proceed with further processing
                    Console.WriteLine("Matched Patient: " + p.GetNameFirstOrPreferred());
                }
                else
                {
                    // No match found, handle accordingly
                    Console.WriteLine("No matching patient found.");
                    return false;
                }


                DateTime? appointmentTime = AppointmentHelper.ExtractAppointmentDate(latestComm.Note);

                if (appointmentTime.HasValue)
                {
                    Console.WriteLine(appointmentTime.Value);

                    // Calculate the difference between the appointment time and the message sent time
                    DateTime messageSentTime = latestComm.CommDateTime;
                    TimeSpan timeUntilAppointment = appointmentTime.Value - messageSentTime;
                    int daysUntilAppointment = (int)Math.Ceiling(timeUntilAppointment.TotalDays);
                    Console.WriteLine($"Time until appointment: {timeUntilAppointment.TotalDays} days");

                    // Determine the confirmation status based on the appointment time
                    long confirmationStatus;
                    if (daysUntilAppointment >= 14 && daysUntilAppointment < 21)
                    {
                        confirmationStatus = _defNumTwoWeekConfirmed;
                    }
                    else if (daysUntilAppointment >= 7)
                    {
                        confirmationStatus = _defNumOneWeekConfirmed; 
                    }
                    else if (daysUntilAppointment >= 0 && daysUntilAppointment <= 3)
                    {
                        confirmationStatus = _defNumConfirmed;
                    }
                    else
                    {
                        // Strange.  We've received YES to an appointment but it's not within the expected time frame
                        string logMessage = $"Received YES to an appointment that is {timeUntilAppointment.TotalDays} days away.  This is unexpected.";
                        EventLog.WriteEntry("ODSMS", logMessage, EventLogEntryType.Warning, 101, 1, new byte[10]);
                        return false;
                    }

                    // Find the corresponding appointment
                    string appointmentQuery = $"SELECT * FROM Appointment WHERE PatNum = {PatNum} AND AptDateTime = '{appointmentTime.Value.ToString("yyyy-MM-dd HH:mm:ss")}'";
                    Appointment originalAppt = OpenDentBusiness.Crud.AppointmentCrud.SelectOne(appointmentQuery);

                    if (originalAppt != null)
                    {
                        if (originalAppt.Confirmed != AsyncSMSHandling._defNumTexted)
                        {
                            EventLog.WriteEntry("ODSMS", "OOPS! Patient just replied yes to an appointment that is already confirmed.  Ignoring", EventLogEntryType.Warning, 101, 1, new byte[10]);
                            return false;
                        }
                        else
                        {
                            // Update the appointment status in the database
                            Appointment updatedAppt = originalAppt.Copy();
                            updatedAppt.Confirmed = confirmationStatus;

                            bool updateSucceeded = AppointmentCrud.Update(updatedAppt, originalAppt);
                            if (updateSucceeded)
                            {
                                Console.WriteLine("Appointment status updated successfully.");
                                return true;
                            }
                            else
                            {
                                EventLog.WriteEntry("ODSMS", "Failure updating appointment status!", EventLogEntryType.Warning, 101, 1, new byte[10]);
                            }
                        }
                    }
                    else
                    {
                        string logMessage = $"No matching appointment found for patient {PatNum} at {appointmentTime.Value}.";
                        EventLog.WriteEntry("ODSMS", logMessage, EventLogEntryType.Warning, 101, 1, new byte[10]);
                    }
                }
                else
                {
                    Console.WriteLine("Error parsing date from communication log note.");
                    EventLog.WriteEntry("ODSMS", "Error parsing date from communication log note.", EventLogEntryType.Error, 101, 1, new byte[10]);
                }
            }
            else
            {
                string logMessage = $"'Yes' received, but no matching appointment found for any of the patients {patNums}.";
                EventLog.WriteEntry("ODSMS", logMessage, EventLogEntryType.Warning, 101, 1, new byte[10]);
            }

            return false;
        }

        /// <summary>
        /// Checks that the ODSMS class has been initialized.  Handy in case we call things out of order.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private static void EnsureInitialized()
        {
            if (String.IsNullOrEmpty(auth))
            {
                throw new ArgumentException("ODSMS has not been initialised.");
            }
        }

        /// <summary>
        /// Checks that the L:\msg_guids folder is found.  Handy in case the network path is down, etc.
        /// </summary>
        private static void EnsureSmsFolderExists()
        {
            if (!Directory.Exists(sms_folder_path))
            {
                EventLog.WriteEntry("ODSMS", "SMS MSG GUIDs folder not found - creating", EventLogEntryType.Warning, 101, 1, new byte[10]);
                MsgBox.Show("SMS folder not found - creating. If this is at the practice then quit OpenDental and contact Corrin");
                Directory.CreateDirectory(sms_folder_path);
            }
        }

        /// <summary>
        /// This processes all the SMS messages received since it as last called (5 minutes is the default timer) - typically just 1 SMS.  It calls ProcessSmsMessage for each SMS
        /// </summary>
        /// <returns></returns>
        private static async Task FetchAndProcessSmsMessages()
        {
            const int getAllCount = 1000;
            int count = getAllCount;
            // int offset = 0;     // The old offset logic has been removed.  It was designed to skip a bunch of SMS that have already been processed
            // int c = 0; // More offset logic
            string removeStr = "&remove=1";
            // removeStr = ""; // Stop removing messages, it doesn't seem to help
            string checkSMSstring = "http/request-received-messages?&order=newest&" + auth;

            var request = checkSMSstring + "&limit=" + count.ToString() + removeStr;
            Console.WriteLine(request);
            var response = await sharedClient.GetAsync(request);
            var text = await response.Content.ReadAsStringAsync();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(text);
            var list = xmlDoc.ChildNodes[1]; // the list of all SMS received (up to count)

            var smsCount = list.ChildNodes.Count;
            if (smsCount == 0)
            {
                Console.WriteLine("No SMS messages received.");
                return;
            } else { 
                Console.WriteLine($"Received {smsCount} SMS messages.");
                EventLog.WriteEntry("ODSMS", $"About to loop through {smsCount} SMS messages", EventLogEntryType.Information, 101, 1, new byte[10]);

            }

            // Part of the old offset logic.  Should never be true

            // if (list.ChildNodes.Count < offset)     
            // {
            //    return;
            // }

            foreach (XmlElement child in list.ChildNodes)
            {
                // if (c < offset)
                // {
                //    c++; // Skip the first offset messages
                //    continue;
                // }

                string msgFrom = child.ChildNodes[0].InnerText;
                string msgText = child.ChildNodes[2].InnerText;
                string msgTime = child.ChildNodes[11].InnerText;
                string msgGUID = child.ChildNodes[4].InnerText;

                await ProcessSmsMessage(msgFrom, msgText, msgTime, msgGUID);
            }
        }

        /// <summary>
        /// Probably shouldn't exist as a seperate method - lasagne code and all that.  Called in one place.  Calls just one thing.
        /// This does a bit of sanity testing / logging, and the calls processOneReceivedSMS with the message.
        /// </summary>
        /// <param name="msgFrom"></param>
        /// <param name="msgText"></param>
        /// <param name="msgTime"></param>
        /// <param name="msgGUID"></param>
        /// <returns></returns>
        private static async Task ProcessSmsMessage(string msgFrom, string msgText, string msgTime, string msgGUID)
        {
            string logMessage = $"SMS from {msgFrom} at time {msgTime} with body {msgText} - GUID: {msgGUID}";
            Console.WriteLine(logMessage);
            EventLog.WriteEntry("ODSMS", logMessage, EventLogEntryType.Information, 101, 1);

            string guidFilePath = Path.Combine(sms_folder_path, msgGUID);

            if (File.Exists(guidFilePath)) // True if we have processed this message before
            {
                string errorMessage = "Must've already processed this SMS";
                Console.WriteLine(errorMessage);
                EventLog.WriteEntry("ODSMS", errorMessage, EventLogEntryType.Warning, 101, 1, new byte[10]);
            }
            else
            {
                try
                {
                    await processOneReceivedSMS(msgText, msgTime, msgFrom, msgGUID);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An exception occurred: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    string fullLogMessage = $"An exception occurred: {ex.Message}\nStack Trace: {ex.StackTrace}";
                    EventLog.WriteEntry("ODSMS", fullLogMessage, EventLogEntryType.Error, 101, 1);
                }
            }
        }


        /// <summary>
        ///  This is the outermost loop for the hourly (=daily) job.  
        ///  It's the one responsible for actually doing a while(true) 
        ///  Internally it calls PerformDailySMSTasks
        /// </summary>
        async public static void SMSDailyTasks()
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

            DateTime lastRunDate = DateTime.MinValue;

            while (true)
            {
                DateTime currentTime = DateTime.Now;
                bool isNewDay = currentTime.Date != lastRunDate.Date;   // Note that this logic doesn't do anything currently.  It's there from when I wanted a safety net on sending birthday texts multiple times a day.

                if (isNewDay)
                {
                    lastRunDate = currentTime;
                }

                if (currentTime.Hour >= 8)
                    //if (!todaysJobsSucceeded && currentTime.Hour >= 8)
                {
                    Console.WriteLine("Performing daily SMS related tasks");
                    EventLog.WriteEntry("ODSMS", "Performing daily SMS related tasks", EventLogEntryType.Information, 101, 1, new byte[10]);

                    try
                    {
                        // Perform your daily SMS tasks here
                        PerformDailySMSTasks();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error performing daily SMS tasks: {ex.Message}");
                        EventLog.WriteEntry("ODSMS", $"Error performing daily SMS tasks: {ex.Message}", EventLogEntryType.Error, 101, 1, new byte[10]);
                    }
                }

                // Always wait for an hour between checks
                if (ODSMS.DEBUG_NUMBER.IsNullOrEmpty())
                {
                    await Task.Delay(TimeSpan.FromHours(1));
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMinutes(3));
                }
                }
        }

        /// <summary>
        /// Similar to SMSDailyTasks, this is the outermost loop for the five minute job.
        /// Internally it calls FetchAndProcessSmsMessages and then waits for 5 minutes.
        /// </summary>
        async public static void receiveSMSforever()
        {
            bool smsIsWorking = await CheckSMSConnection();


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

                try
                {
                    if (smsIsWorking)
                    {
                        try
                        {
                            EnsureInitialized();
                            EnsureSmsFolderExists();
                            await FetchAndProcessSmsMessages();
                        }
                        catch (Exception ex)
                        {
                            MsgBox.Show("Receiving patient texts failed.");
                            EventLog.WriteEntry("ODSMS", ex.ToString(), EventLogEntryType.Error, 101, 1, new byte[10]);
                        }
                    }
                    else
                    {
                        if (ODSMS.wasSmsBroken)
                        {
                            Console.WriteLine("SMS continues to be down");
                        }
                        else
                        {
                            MsgBox.Show("Failure checking Diafaan! SMS will not send or receive until fixed.  Please go to the phone and check the GSM Modem Gateway app and ensure Wi-Fi is enabled. Failing that, check Diafaan manually.");
                            EventLog.WriteEntry("ODSMS", "SMS is down", EventLogEntryType.Error, 101, 1, new byte[10]);
                            ODSMS.wasSmsBroken = true;
                            ODSMS.USE_ODSMS = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving SMS: {ex.Message}");
                    EventLog.WriteEntry("ODSMS", "Error receiving SMS", EventLogEntryType.Error, 101, 1, new byte[10]);
                    smsIsWorking = false;
                    ODSMS.wasSmsBroken = true;
                    ODSMS.USE_ODSMS = false;
                }

                if (!smsIsWorking && await CheckSMSConnection())
                {
                    Console.WriteLine("SMS connection restored");
                    EventLog.WriteEntry("ODSMS", "SMS connection restored", EventLogEntryType.Information, 101, 1, new byte[10]);
                    smsIsWorking = true;
                    ODSMS.wasSmsBroken = false;
                    ODSMS.USE_ODSMS = true;
                    MsgBox.Show("SMS is restored. Birthday texts and reminders will be sent now.");
                }
            }
        }
    }
}
