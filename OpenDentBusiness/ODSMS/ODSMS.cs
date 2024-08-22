using CodeBase;
using System;
using System.IO;
using System.Diagnostics;
using System.Net.Http;
using System.Xml;
using System.Threading.Tasks;
using OpenDentBusiness;
using DataConnectionBase;

namespace OpenDentBusiness.ODSMS
{
    public static class ODSMS
    {
        // Configuration variables
        public static bool USE_ODSMS = true;
        public static bool SEND_SMS = true;
        public static bool WRITE_TO_DATABASE = true;

        // Internal state
        public static bool wasSmsBroken = false;
        public static bool initialStartup = true;
        public static HttpClient sharedClient = null;

        // Variables from the configuration file
        public static string AUTH;
        public static string URL;
        public static string DEBUG_NUMBER = "";
        public static bool RUN_SCHEDULED_TASKS = false;
        public static string PRACTICE_PHONE_NUMBER = "";

        public static string sms_folder_path = @"L:\msg_guids\";

        static ODSMS()
        {
            string MachineName = Environment.MachineName;

            InitializeEventLog();

            string configPath = @"L:\odsms.txt";
            ValidateConfigPath(configPath);
            LoadConfiguration(configPath, MachineName);

            sharedClient = new HttpClient();
            sharedClient.BaseAddress = new Uri(URL);

            LogConfigurationStatus(MachineName);
        }

        private static void InitializeEventLog()
        {
            if (!EventLog.SourceExists("ODSMS"))
            {
                EventLog.CreateEventSource("ODSMS", "Application");
                Console.WriteLine("Event source 'ODSMS' created successfully.");
            }
            else
            {
                Console.WriteLine("Event source 'ODSMS' already exists.");
            }

            EventLog.WriteEntry("ODSMS", "Running custom build of Open Dental on " + Environment.MachineName, EventLogEntryType.Information, 101, 1, new byte[10]);
        }

        private static void ValidateConfigPath(string configPath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(configPath)))
            {
                throw new DirectoryNotFoundException($"Directory not found: {Path.GetDirectoryName(configPath)}");
            }

            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException("Config file not found", configPath);
            }
        }

        private static void LoadConfiguration(string configPath, string MachineName)
        {
            try
            {
                foreach (string line in File.ReadLines(configPath))
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

            ValidateConfiguration();
        }

        private static void ValidateConfiguration()
        {
            if (AUTH == null || URL == null)
            {
                throw new ArgumentNullException("AUTH or URL", "One or both of AUTH or URL was not set in odsms.txt");
            }

            if (string.IsNullOrEmpty(PRACTICE_PHONE_NUMBER))
            {
                throw new ArgumentNullException("Forgot to set PHONE: in the configuration file");
            }
        }

        private static void LogConfigurationStatus(string MachineName)
        {
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

        public static async Task<bool> CheckSMSConnection()
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
            if (!Directory.Exists(sms_folder_path))
            {
                ODSMSLogger.Instance.Log("SMS MSG GUIDs folder not found - creating", EventLogEntryType.Warning);
                System.Windows.MessageBox.Show("SMS folder not found - creating. If this is at the practice then quit OpenDental and contact Corrin");
                Directory.CreateDirectory(sms_folder_path);
            }
        }
    }
}