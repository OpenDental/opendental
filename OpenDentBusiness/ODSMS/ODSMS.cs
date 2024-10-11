using CodeBase;
using System;
using System.IO;
using System.Diagnostics;
using System.Net.Http;
using System.Xml;
using SystemTask = System.Threading.Tasks.Task;
using OpenDentBusiness;
using DataConnectionBase;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        private static List<Def> _listDefsApptConfirmed;
        public static long _defNumTwoWeekConfirmed;
        public static long _defNumOneWeekConfirmed;
        public static long _defNumConfirmed;
        public static long _defNumNotCalled;
        public static long _defNumUnconfirmed;
        public static long _defNumTwoWeekSent;
        public static long _defNumOneWeekSent;
        public static long _defNumTexted;
        public static long _defNumWebSched;

        static ODSMS()
        {
            string MachineName = Environment.MachineName;

            InitializeEventLog();

            string configPath = @"L:\odsms.txt";
            ValidateConfigPath(configPath);
            LoadConfiguration(configPath, MachineName);

            sharedClient = new HttpClient
            {
                BaseAddress = new Uri(URL)
            };

            LogConfigurationStatus(MachineName);
        }

        public static bool SanityCheckConstants()
        {
            var defNumList = new List<long>
            {
                _defNumTexted,
                _defNumTwoWeekSent,
                _defNumOneWeekSent,
                _defNumTwoWeekConfirmed,
                _defNumOneWeekConfirmed,
                _defNumConfirmed,
                _defNumNotCalled,
                _defNumUnconfirmed,
                _defNumWebSched
            };

            // Create a HashSet from the list
            var defNumSet = new HashSet<long>(defNumList);

            // Compare the size of the list to the size of the HashSet
            bool allUnique = defNumList.Count == defNumSet.Count;
            if (allUnique)
            {
                return true;
            }
            else
            {
                ODSMSLogger.Instance.Log("Database constants like _defNumOneWeekConfirmed have an issue", EventLogEntryType.Error);
                System.Windows.MessageBox.Show("Attempt to send SMS without the database!?.");
                return false;
            }
        }
        private static long GetAndCheckDefNum(string itemName, List<OpenDentBusiness.Def> listDefs)
        {
            var def = listDefs
                .FirstOrDefault(d => string.Equals(d.ItemName, itemName, StringComparison.OrdinalIgnoreCase));

            long defNum = def?.DefNum ?? 0;

            if (defNum == 0)
            {
                string s = $"The '{itemName}' appointment status was not found.";
                ODSMSLogger.Instance.Log(s, EventLogEntryType.Error);
                System.Windows.MessageBox.Show(s);
                throw new Exception(s);
            }

            return defNum;
        }


        public static async SystemTask InitializeSMS()
        {
            while (!DataConnection.HasDatabaseConnection)
            {
                Console.WriteLine("Waiting for database connection...");
                await SystemTask.Delay(5000);
            }

            _listDefsApptConfirmed = Defs.GetDefsForCategory(DefCat.ApptConfirmed, isShort: true);
            _defNumTexted = GetAndCheckDefNum("texted", _listDefsApptConfirmed);
            _defNumTwoWeekSent = GetAndCheckDefNum("2 week sent", _listDefsApptConfirmed);
            _defNumOneWeekSent = GetAndCheckDefNum("1 week sent", _listDefsApptConfirmed);
            _defNumTwoWeekConfirmed = GetAndCheckDefNum("2 week confirmed", _listDefsApptConfirmed);
            _defNumOneWeekConfirmed = GetAndCheckDefNum("1 week confirmed", _listDefsApptConfirmed);
            _defNumConfirmed = GetAndCheckDefNum("Appointment Confirmed", _listDefsApptConfirmed);
            _defNumNotCalled = GetAndCheckDefNum("not called", _listDefsApptConfirmed);
            _defNumUnconfirmed = GetAndCheckDefNum("unconfirmed", _listDefsApptConfirmed);
            _defNumWebSched = GetAndCheckDefNum("Created from Web Sched", _listDefsApptConfirmed);
            SanityCheckConstants();
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

        public static async SystemTask WaitForDatabaseAndUserInitialization()
        {
            while (!DataConnection.HasDatabaseConnection)
            {
                ODSMSLogger.Instance.Log("Waiting for database connection...", EventLogEntryType.Information, logToEventLog: false, logToFile: false);
                await SystemTask.Delay(5000);
            }

            while (Security.CurUser == null || Security.CurUser.UserNum == 0)
            {
                ODSMSLogger.Instance.Log("Waiting for user information to be initialized...", EventLogEntryType.Information, logToEventLog: false, logToFile: false);
                await SystemTask.Delay(5000);
            }
        }

        public static string RenderReminder(string reminderTemplate, Patient p, Appointment a)
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