using CodeBase;
using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Shapes;
using System.Net.Http;
using System.Xml;
using Task = System.Threading.Tasks.Task;
using OpenDentBusiness;
using DataConnectionBase;

namespace OpenDentBusiness.ODSMS
{
    public static class ODSMS
    {
        // Set variables here and they'll be used by the program.    
        public static bool USE_ODSMS = true; // Set to false to disable ODSMS completely, and try to use the Open Dental eServices
        public static bool SEND_SMS = true;   // Set to false for disabling all sending.  Not set anywhere except here
        public static bool WRITE_TO_DATABASE = true;   // Set to false for disabling writing in the comm log, or updating appointments. Generally try to 'say we are doing something but not actually do it', as much as we can.

        // These are maintained internal state.  You shouldn't need to touch them
        public static bool wasSmsBroken = false; // Default this to false, so that we don't panic if SMS starts working
        public static bool initialStartup = true; // Set to false after the first call to the hourly/daily job
        public static HttpClient sharedClient = null;

        // Variables from the configuraton file
        public static string AUTH;  // username/password for diafaan
        public static string URL;   // URL for diafaan
        public static string DEBUG_NUMBER = "";   // Set in the config.  Leave empty and everything works like normal.  If not empty all SMS go to this number 
        public static bool RUN_SCHEDULED_TASKS = false;   // should we receive text on this computer? Send birthday reminders? Set based on the configuration file
        public static string PRACTICE_PHONE_NUMBER = ""; // The phone number of the practice in international format

        public static string sms_folder_path = @"L:\msg_guids\";

        static ODSMS()
        {
            string MachineName = Environment.MachineName;
            // Not ODEnvironment.MachineNmae - if you remote into a machine and run OD there, treat it as that machine

            // Set up event log -- must be run as Administrator
            if (!EventLog.SourceExists("ODSMS"))
            {
                EventLog.CreateEventSource("ODSMS", "Application");
                Console.WriteLine("Event source 'ODSMS' created successfully.");
            }
            else
            {
                Console.WriteLine("Event source 'ODSMS' already exists.");
            }

            EventLog.WriteEntry("ODSMS", "Running custom build of Open Dental on " + MachineName, EventLogEntryType.Information, 101, 1, new byte[10]);

            string configPath = @"L:\odsms.txt";

            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(configPath)))
            {
                throw new DirectoryNotFoundException($"Directory not found: {System.IO.Path.GetDirectoryName(configPath)}");
            }

            if (!System.IO.File.Exists(configPath))
            {
                throw new FileNotFoundException("Config file not found", configPath);
            }

            try
            {
                foreach (string line in System.IO.File.ReadLines(configPath))
                {
                    if (line.StartsWith("AUTH:"))
                        AUTH = line.Replace("AUTH:", "");
                    else if (line.StartsWith("DISABLE:"))
                        USE_ODSMS = false;
                    else if (line.StartsWith("DEBUG:"))
                        DEBUG_NUMBER = line.Replace("DEBUG:", "");
                    else if (line.StartsWith("PHONE:"))
                        PRACTICE_PHONE_NUMBER = line.Replace("PHONE:", "");
                    else if (line.StartsWith("URL:"))
                        URL = line.Replace("URL:", "");
                    if (line.StartsWith("RECEIVER:"))
                    {
                        string receiver_name = line.Replace("RECEIVER:", "");
                        if (receiver_name == MachineName)
                        {
                            RUN_SCHEDULED_TASKS = true;
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                EventLog.WriteEntry("ODSMS", "odsms.txt config file could not be read - stuff is about to break", EventLogEntryType.Error, 101, 1, new byte[10]);
                throw;
            }

            if (AUTH == null || URL == null)
            {
                throw new ArgumentNullException("AUTH or URL", "One or both of AUTH or URL was not set in odsms.txt");
            }

            if (PRACTICE_PHONE_NUMBER.IsNullOrEmpty())
            {
                throw new ArgumentNullException("Forgot to sent PHONE: in the configuraton file");
            }

            sharedClient = new HttpClient();
            sharedClient.BaseAddress = new Uri(URL);

            if (RUN_SCHEDULED_TASKS)
            {
                EventLog.WriteEntry("ODSMS", "Name matches, enabling SMS reception", EventLogEntryType.Information, 101, 1, new byte[10]);
            }
            else
            {
                EventLog.WriteEntry("ODSMS", "Not receiving SMS on this computer:" + MachineName, EventLogEntryType.Information, 101, 1, new byte[10]);
            }

            EventLog.WriteEntry("ODSMS", "Successfully loaded odsms.txt config file", EventLogEntryType.Information, 101, 1, new byte[10]);
        }

        public static async System.Threading.Tasks.Task<bool> CheckSMSConnection()
        {
            try
            {
                string checkStr = "http/request-server-status?" + AUTH;
                HttpResponseMessage httpResponseMessage = await sharedClient.GetAsync(checkStr);
                var text = await httpResponseMessage.Content.ReadAsStringAsync();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(text);
                XmlNode gsmModemGateway = xmlDoc.SelectSingleNode("//Gateway[@Name='GSM Modem Gateway']");
                if (gsmModemGateway != null)
                {
                    string active = gsmModemGateway.Attributes["Active"].InnerText;
                    string available = gsmModemGateway.Attributes["Available"].InnerText;
                    string sendEnabled = gsmModemGateway.Attributes["SendEnabled"].InnerText;
                    string receiveEnabled = gsmModemGateway.Attributes["ReceiveEnabled"].InnerText;

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
                    ODSMSLogger.Instance.Log("GSM Modem Gateway not found", EventLogEntryType.Error, logToFile: false);
                }
            }
            catch (Exception ex)
            {
                ODSMSLogger.Instance.Log($"Error checking Diafaan status: {ex.Message}", EventLogEntryType.Error, logToFile: false);
            }

            return false;
        }

        public static async System.Threading.Tasks.Task WaitForDatabaseAndUserInitialization()
        {
            while (!DataConnection.HasDatabaseConnection)
            {
                ODSMSLogger.Instance.Log("Waiting for database connection...", EventLogEntryType.Information, logToEventLog: false, logToFile: false);
                await System.Threading.Tasks.Task.Delay(5000);
            }

            while (Security.CurUser == null || Security.CurUser.UserNum == 0)
            {
                ODSMSLogger.Instance.Log("Waiting for user information to be initialized...", EventLogEntryType.Information, logToEventLog: false, logToFile: false);
                await System.Threading.Tasks.Task.Delay(5000);
            }
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
                s = s.Replace("[date]", a.AptDateTime.ToString("dddd, d MMMM yyyy"))
                     .Replace("[time]", a.AptDateTime.ToString("h:mm tt"));
            }
            return s;
        }

        public static void EnsureSmsFolderExists()
        {
            if (!System.IO.Directory.Exists(sms_folder_path))
            {
                ODSMSLogger.Instance.Log("SMS MSG GUIDs folder not found - creating", EventLogEntryType.Warning);
                MsgBox.Show("SMS folder not found - creating. If this is at the practice then quit OpenDental and contact Corrin");
                System.IO.Directory.CreateDirectory(sms_folder_path);
            }
        }
    }
}