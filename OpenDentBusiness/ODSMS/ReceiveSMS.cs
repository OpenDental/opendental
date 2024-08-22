using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using OpenDentBusiness;
using OpenDentBusiness.Crud;
using OpenDentBusiness.Mobile;
using Google.Apis.Gmail.v1.Data;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenDentBusiness.ODSMS
{
    public static class ReceiveSMS
    {
        private static readonly SemaphoreSlim fetchAndProcessSemaphore = new SemaphoreSlim(1, 1);

        public static async System.Threading.Tasks.Task ReceiveSMSForever()
        {
            await ODSMS.WaitForDatabaseAndUserInitialization();

            while (true)
            {
                await System.Threading.Tasks.Task.Delay(60 * 1000); // Check SMS once a minute
                ODSMSLogger.Instance.Log("Checking for new SMS now", EventLogEntryType.Information, logToEventLog: false, logToFile: false);

                try
                {
                    bool smsIsWorking = await ODSMS.CheckSMSConnection();
                    if (smsIsWorking)
                    {
                        try
                        {
                            ODSMS.EnsureSmsFolderExists();
                            await FetchAndProcessSmsMessages();
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show("Receiving patient texts failed.");
                            ODSMSLogger.Instance.Log(ex.ToString(), EventLogEntryType.Error);
                        }
                    }
                    else
                    {
                        HandleSMSDowntime();
                    }
                }
                catch (Exception ex)
                {
                    ODSMSLogger.Instance.Log($"Error receiving SMS: {ex.Message}", EventLogEntryType.Error);
                    ODSMS.wasSmsBroken = true;
                    ODSMS.USE_ODSMS = false;
                }

                if (ODSMS.wasSmsBroken && await ODSMS.CheckSMSConnection())
                {
                    HandleSMSRestored();
                }
            }
        }

        private static void HandleSMSDowntime()
        {
            if (ODSMS.wasSmsBroken)
            {
                ODSMSLogger.Instance.Log("SMS continues to be down", EventLogEntryType.Information, logToEventLog: false, logToFile: false);
            }
            else
            {
                System.Windows.MessageBox.Show("Failure checking Diafaan! SMS will not send or receive until fixed. Please check the GSM Modem Gateway app and ensure Wi-Fi is enabled. Failing that, check Diafaan manually.");
                ODSMSLogger.Instance.Log("SMS is down", EventLogEntryType.Error);
                ODSMS.wasSmsBroken = true;
                ODSMS.USE_ODSMS = false;
            }
        }

        private static void HandleSMSRestored()
        {
            ODSMSLogger.Instance.Log("SMS connection restored", EventLogEntryType.Information);
            ODSMS.wasSmsBroken = false;
            ODSMS.USE_ODSMS = true;
            System.Windows.MessageBox.Show("SMS is restored. Birthday texts and reminders will be sent now.");
        }

        public static async System.Threading.Tasks.Task FetchAndProcessSmsMessages(string removeStr = "")
        {
            await fetchAndProcessSemaphore.WaitAsync();
            try
            {
                const int getAllCount = 1000;
                string checkSMSstring = "http/request-received-messages?&order=newest&" + ODSMS.AUTH;

                var request = checkSMSstring + "&limit=" + getAllCount.ToString() + removeStr;
                ODSMSLogger.Instance.Log($"Raw Diafaan API call: {request}", EventLogEntryType.Information);

                var response = await ODSMS.sharedClient.GetAsync(request);
                var text = await response.Content.ReadAsStringAsync();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(text);
                var list = xmlDoc.ChildNodes[1];

                var smsCount = list.ChildNodes.Count;
                if (smsCount == 0)
                {
                    Console.WriteLine("No SMS messages received.");
                    return;
                }

                ODSMSLogger.Instance.Log($"About to loop through {smsCount} SMS messages", EventLogEntryType.Information);

                foreach (XmlElement child in list.ChildNodes)
                {
                    string msgFrom = child.ChildNodes[0].InnerText;
                    string msgText = child.ChildNodes[2].InnerText;
                    string msgTime = child.ChildNodes[11].InnerText;
                    string msgGUID = child.ChildNodes[4].InnerText;

                    await ProcessSmsMessage(msgFrom, msgText, msgTime, msgGUID);
                }
            }
            finally
            {
                fetchAndProcessSemaphore.Release();
            }
        }

        private static async System.Threading.Tasks.Task ProcessSmsMessage(string msgFrom, string msgText, string msgTime, string msgGUID)
        {
            string logMessage = $"SMS from {msgFrom} at time {msgTime} with body {msgText} - GUID: {msgGUID}";
            ODSMSLogger.Instance.Log(logMessage, EventLogEntryType.Information);

            string guidFilePath = Path.Combine(ODSMS.sms_folder_path, msgGUID);

            if (File.Exists(guidFilePath))
            {
                ODSMSLogger.Instance.Log("Must've already processed this SMS", EventLogEntryType.Information);
            }
            else
            {
                try
                {
                    await ProcessOneReceivedSMS(msgText, msgTime, msgFrom, msgGUID);
                }
                catch (Exception ex)
                {
                    string fullLogMessage = $"An exception occurred: {ex.Message}\nStack Trace: {ex.StackTrace}";
                    ODSMSLogger.Instance.Log(fullLogMessage, EventLogEntryType.Error);
                }
            }
        }

        private static async System.Threading.Tasks.Task ProcessOneReceivedSMS(string msgText, string msgTime, string msgFrom, string msgGUID)
        {
            ODSMSLogger.Instance.Log("SMS inner loop - downloaded a single SMS", EventLogEntryType.Information, logToEventLog: false);
            string guidFilePath = Path.Combine(ODSMS.sms_folder_path, msgGUID);
            string cleanedText = Regex.Replace(msgText.ToUpper(), "[^A-Z]", "");

            byte[] bytesToWrite = Encoding.UTF8.GetBytes(msgText);

            using (var fileStream = File.Create(guidFilePath))
            {
                await fileStream.WriteAsync(bytesToWrite, 0, bytesToWrite.Length);
            }

            var patients = Patients.GetPatientsByPhone(msgFrom.Substring(3), "+64", new List<PhoneType> { PhoneType.WirelessPhone });
            var time = DateTime.Parse(msgTime);

            ODSMSLogger.Instance.Log($"Number of matching patients: {patients.Count}", EventLogEntryType.Information, logToEventLog: false);

            if (patients.Count > 20)
            {
                ODSMSLogger.Instance.Log("Too many patients match this number! Assuming a dummy entry", EventLogEntryType.Information);
                return;
            }

            Commlog log = CreateCommlog(patients, msgText, time);
            SmsFromMobile sms = CreateSmsFromMobile(log, msgFrom, time, msgText, patients.Count);

            if (cleanedText == "YES" || cleanedText == "Y")
            {
                if (patients.Count < 10)
                {
                    await HandleAutomatedConfirmation(patients, sms);
                }
                else
                {
                    ODSMSLogger.Instance.Log("'YES' received matching more than ten patients - process manually", EventLogEntryType.Information);
                }
            }

            OpenDentBusiness.SmsFromMobiles.Insert(sms);

            Console.WriteLine("Finished OD New Text Message");
        }

        private static Commlog CreateCommlog(List<Patient> patients, string msgText, DateTime time)
        {
            Commlog log = new Commlog
            {
                PatNum = patients.Count != 0 ? patients[0].PatNum : 0,
                Note = "Text message received: " + msgText,
                Mode_ = CommItemMode.Text,
                CommDateTime = time,
                SentOrReceived = CommSentOrReceived.Received,
                CommType = Commlogs.GetTypeAuto(CommItemTypeAuto.TEXT),
                UserNum = 1 // Admin user
            };
            return log;
        }

        private static SmsFromMobile CreateSmsFromMobile(Commlog log, string msgFrom, DateTime time, string msgText, int patientCount)
        {
            SmsFromMobile sms = new SmsFromMobile
            {
                CommlogNum = Commlogs.Insert(log),
                MobilePhoneNumber = msgFrom,
                PatNum = log.PatNum,
                DateTimeReceived = time,
                MsgText = msgText,
                SmsStatus = SmsFromStatus.ReceivedUnread,
                MsgTotal = 1,
                MatchCount = patientCount,
                ClinicNum = 0,
                MsgPart = 1,
                Flags = " "
            };
            return sms;
        }

        private static async System.Threading.Tasks.Task HandleAutomatedConfirmation(List<Patient> patients, SmsFromMobile sms)
        {
            ODSMSLogger.Instance.Log("About to handle automated SMS", EventLogEntryType.Information, logToEventLog: false);

            bool wasHandled = SendSMS.HandleAutomatedConfirmation(patients);
            if (wasHandled)
            {
                sms.SmsStatus = SmsFromStatus.ReceivedRead;
                ODSMSLogger.Instance.Log("Success handling automated SMS", EventLogEntryType.Information, logToEventLog: false);
            }
            else
            {
                ODSMSLogger.Instance.Log("Failure handling automated SMS", EventLogEntryType.Warning);
                await SendConfirmationFailureMessage(patients[0], sms.MobilePhoneNumber);
            }
        }

        private static async System.Threading.Tasks.Task SendConfirmationFailureMessage(Patient patient, string mobileNumber)
        {
            string matchSMSmessage = "Thank you for your reply but we've had trouble matching it to an appointment. Could you please give us a call?";
            SmsToMobile matchSMS = new SmsToMobile
            {
                PatNum = patient.PatNum,
                SmsPhoneNumber = ODSMS.PRACTICE_PHONE_NUMBER,
                MobilePhoneNumber = mobileNumber,
                MsgText = matchSMSmessage,
                MsgType = SmsMessageSource.GeneralMessage,
                SmsStatus = SmsDeliveryStatus.Pending,
                MsgParts = 1
            };
            await SmsToMobiles.SendSmsMessageAsync(matchSMS);
            InsertConfirmationFailureCommlog(matchSMS);
        }

        private static void InsertConfirmationFailureCommlog(SmsToMobile matchSMS)
        {
            Commlogs.Insert(new Commlog()
            {
                CommDateTime = matchSMS.DateTimeSent,
                Mode_ = CommItemMode.Text,
                Note = "Text message sent: " + matchSMS.MsgText,
                PatNum = matchSMS.PatNum,
                CommType = Commlogs.GetTypeAuto(CommItemTypeAuto.TEXT),
                SentOrReceived = CommSentOrReceived.Sent,
                UserNum = 0
            });
        }
    }
}