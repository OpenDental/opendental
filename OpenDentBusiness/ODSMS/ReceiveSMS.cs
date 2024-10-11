using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml;
using SystemTask = System.Threading.Tasks.Task;
using System.Threading;
using System.Text.RegularExpressions;
using OpenDentBusiness;
using OpenDentBusiness.Crud;
using OpenDentBusiness.Mobile;
using Google.Apis.Gmail.v1.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenDentBusiness.ODSMS
{
    public static class ReceiveSMS
    {
        private static readonly SemaphoreSlim fetchAndProcessSemaphore = new SemaphoreSlim(1, 1);

        public static async SystemTask ReceiveSMSForever()
        {
            await ODSMS.WaitForDatabaseAndUserInitialization();

            while (true)
            {
                await SystemTask.Delay(60 * 1000); // Check SMS once a minute
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

        public static async SystemTask FetchAndProcessSmsMessages(string removeStr = "")
        {
            await fetchAndProcessSemaphore.WaitAsync();
            try
            {
                const int getAllCount = 1000;
                string checkSMSstring = "http/request-received-messages?&order=newest&" + ODSMS.AUTH;

                var request = checkSMSstring + "&limit=" + getAllCount.ToString() + removeStr;
                ODSMSLogger.Instance.Log($"Raw Diafaan API call: {request}", EventLogEntryType.Information, logToEventLog: false, logToFile: false);

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

                ODSMSLogger.Instance.Log($"About to loop through {smsCount} SMS messages", EventLogEntryType.Information, logToEventLog: false);

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

        private static async SystemTask ProcessSmsMessage(string msgFrom, string msgText, string msgTime, string msgGUID)
        {

            string guidFilePath = Path.Combine(ODSMS.sms_folder_path, msgGUID);

            if (File.Exists(guidFilePath))
            {
                ODSMSLogger.Instance.Log("Must've already processed this SMS", EventLogEntryType.Information, logToEventLog: false);
            }
            else
            {
                string logMessage = $"SMS from {msgFrom} at time {msgTime} with body {msgText} - GUID: {msgGUID}";
                ODSMSLogger.Instance.Log(logMessage, EventLogEntryType.Information);

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

        private static bool IsAppointmentAlreadyConfirmed(Appointment appointment)
        {
            return appointment.Confirmed != ODSMS._defNumTexted &&
                    appointment.Confirmed != ODSMS._defNumOneWeekSent &&
                    appointment.Confirmed != ODSMS._defNumTwoWeekSent;
        }

        private static bool UpdateAppointmentStatus(Appointment originalAppt, long confirmationStatus)
        {
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
                ODSMSLogger.Instance.Log("Failure updating appointment status!", EventLogEntryType.Error);
                return false;
            }
        }

        /// <summary>
        /// Determines the appropriate confirmation status for an appointment.
        /// </summary>
        /// <param name="daysUntilAppointment">The number of days remaining until the appointment.</param>
        /// <returns>
        /// The confirmation status to set based on the number of days:
        /// - Two-week reminder has been confirmed.
        /// - One-week reminder has been confirmed.
        /// - The appointment has been confirmed.
        /// Returns 0 if the days until the appointment are outside expected ranges.
        /// </returns>
        private static long GetConfirmationStatus(int daysUntilAppointment)
        {
            if (daysUntilAppointment >= 14 && daysUntilAppointment < 22)
            {
                return ODSMS._defNumTwoWeekConfirmed;
            }
            else if (daysUntilAppointment >= 7)
            {
                return ODSMS._defNumOneWeekConfirmed;
            }
            else if (daysUntilAppointment >= 0 && daysUntilAppointment <= 4)
            {
                return ODSMS._defNumConfirmed;
            }
            else
            {
                string logMessage = $"Received YES to an appointment that is {daysUntilAppointment} days away. This is unexpected.";
                ODSMSLogger.Instance.Log(logMessage, EventLogEntryType.Warning);
                return 0;
            }
        }
        public static bool HandleAutomatedConfirmationInternal(List<Patient> patientList)
        {
            string patNums = String.Join(",", patientList.Select(p => p.PatNum.ToString()).ToArray());
            string latestSMS = "SELECT * from CommLog where PatNum IN (" + patNums + ") AND Note REGEXP 'Text message sent.*(reply|respond).*YES' AND CommDateTime >= DATE_SUB(NOW(), INTERVAL 21 DAY) ORDER BY CommDateTime DESC LIMIT 1";
            Commlog latestComm = OpenDentBusiness.Crud.CommlogCrud.SelectOne(latestSMS);

            if (latestComm != null)
            {
                long PatNum = latestComm.PatNum;
                Patient p = patientList.FirstOrDefault(p => p.PatNum == latestComm.PatNum);
                if (p == null)
                {
                    ODSMSLogger.Instance.Log("No matching patient found.", EventLogEntryType.Information, logToEventLog: false);
                    return false;
                }

                DateTime? appointmentTime = AppointmentHelper.ExtractAppointmentDate(latestComm.Note);

                if (appointmentTime.HasValue)
                {
                    TimeSpan timeUntilAppointment = appointmentTime.Value - latestComm.CommDateTime;
                    int daysUntilAppointment = (int)Math.Ceiling(timeUntilAppointment.TotalDays);

                    long confirmationStatus = GetConfirmationStatus(daysUntilAppointment);
                    if (confirmationStatus == 0)
                    {
                        ODSMSLogger.Instance.Log("Confirmation status is 0. Ignoring.", EventLogEntryType.Warning);
                        return false;
                    }

                    string appointmentQuery = $"SELECT * FROM Appointment WHERE PatNum = {PatNum} AND AptDateTime = '{appointmentTime.Value.ToString("yyyy-MM-dd HH:mm:ss")}'";
                    Appointment originalAppt = AppointmentCrud.SelectOne(appointmentQuery);

                    if (originalAppt != null)
                    {
                        if (IsAppointmentAlreadyConfirmed(originalAppt))
                        {
                            ODSMSLogger.Instance.Log("OOPS! Patient just replied yes to an appointment that is already confirmed. Ignoring", EventLogEntryType.Warning);
                            return false;
                        }
                        else
                        {
                            bool updateSuccess = UpdateAppointmentStatus(originalAppt, confirmationStatus);
                            if (!updateSuccess)
                            {
                                ODSMSLogger.Instance.Log("Failed to update appointment status.", EventLogEntryType.Warning);
                            }
                            return updateSuccess;
                        }
                    }
                    else
                    {
                        string logMessage = $"No matching appointment found for patient {PatNum} at {appointmentTime.Value}.";
                        ODSMSLogger.Instance.Log(logMessage, EventLogEntryType.Warning);
                    }
                }
                else
                {
                    ODSMSLogger.Instance.Log($"Error parsing date from communication log note. {latestComm.Note ?? "Note is null"}", EventLogEntryType.Error);
                }
            }
            else
            {
                string logMessage = $"'Yes' received, but no matching appointment found for any of the patients {patNums}.";
                ODSMSLogger.Instance.Log(logMessage, EventLogEntryType.Warning);
            }

            return false;
        }

        private static async SystemTask ProcessOneReceivedSMS(string msgText, string msgTime, string msgFrom, string msgGUID)
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

        private static async SystemTask HandleAutomatedConfirmation(List<Patient> patients, SmsFromMobile sms)
        {
            ODSMSLogger.Instance.Log("About to handle automated SMS", EventLogEntryType.Information, logToEventLog: false);

            bool wasHandled = ReceiveSMS.HandleAutomatedConfirmationInternal(patients);
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

        private static async SystemTask SendConfirmationFailureMessage(Patient patient, string mobileNumber)
        {
            string matchSMSmessage = "Thank you for your response.\nWe couldn't find any appointments that need confirmation.\n" +
                                     "If this doesn’t seem right, please give us a call.";
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