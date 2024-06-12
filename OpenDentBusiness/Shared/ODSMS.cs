using CodeBase;
using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Shapes;
using System.Net.Http;

namespace OpenDental
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

        static ODSMS()
        {

            string MachineName = Environment.MachineName; 
            // Not ODEnvironment.MachineNmae - if you remote into a machine and run OD there, treat it as that machine

            // Set up event log -- must be run as Administrator
            try
            {
                EventLog.CreateEventSource("ODSMS", "Application");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Event source already exists, or we don't have permission to create it.");
                EventLog.WriteEntry("ODSMS", "Event source already exists, or we don't have permission to create it", EventLogEntryType.Information, 101, 1, new byte[10]);
            }

            EventLog.WriteEntry("ODSMS", "Running custom build of Open Dental on " + MachineName, EventLogEntryType.Information, 101, 1, new byte[10]);


            string configPath = @"L:\odsms.txt";
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
            catch (FileNotFoundException) {
                EventLog.WriteEntry("ODSMS", "odsms.txt config file not found - stuff is about to break", EventLogEntryType.Error, 101, 1, new byte[10]);
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
            } else
            {
                EventLog.WriteEntry("ODSMS", "Not receiving SMS on this computer:" + MachineName, EventLogEntryType.Information, 101, 1, new byte[10]);
            }

            EventLog.WriteEntry("ODSMS", "Successfully loaded odsms.txt config file", EventLogEntryType.Information, 101, 1, new byte[10]);
        }
    }

}
