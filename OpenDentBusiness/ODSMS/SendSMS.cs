using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using DataConnectionBase;
using OpenDentBusiness.Crud;
using SystemTask = System.Threading.Tasks.Task;

namespace OpenDentBusiness.ODSMS
{

    public enum ReminderFilterType
    {
        OneDay,
        OneWeek,
        TwoWeeks
    };


    public static class SendSMS
    {


        private static int GetMinutesUntilQuarterPast(DateTime now)
        {
            int minutesToNextQuarter = (15 - now.Minute % 15) % 60;
            if (minutesToNextQuarter == 0)
            {
                minutesToNextQuarter = 60;
            }

            return Math.Max(1, minutesToNextQuarter);
        }


        private static List<Patient> GetPatientsWithBirthdayToday()
        {
            string select = "SELECT p.* ";
            string from = "FROM patient AS p ";
            string where_true = "WHERE TRUE ";
            string where_active = $"AND p.PatStatus IN ({(int)OpenDentBusiness.PatientStatus.Patient}) ";
            string where_allow_sms = "AND p.TxtMsgOk < 2 ";
            string where_birthday = "AND MONTH(p.Birthdate) = MONTH(CURRENT_DATE()) AND DAY(p.Birthdate) = DAY(CURRENT_DATE()) ";
            string where_not_contacted = @"AND NOT EXISTS (
                                    SELECT 1 
                                    FROM CommLog m 
                                    WHERE m.PatNum = p.PatNum 
                                    AND m.Note LIKE '%Birthday%' 
                                    AND m.CommDateTime > DATE_SUB(NOW(), INTERVAL 3 DAY)) ";
            string where_mobile_phone = "AND LENGTH(COALESCE(p.WirelessPhone,'')) > 7 ";

            string command = select + from + where_true + where_active + where_birthday + where_not_contacted + where_allow_sms + where_mobile_phone;
            Console.WriteLine(command);

            List<Patient> listPats = OpenDentBusiness.Crud.PatientCrud.SelectMany(command);
            return listPats;
        }

        private static string GetReminderMessageTemplate(ReminderFilterType filterType)
        {
            return filterType switch
            {
                ReminderFilterType.OneDay => PrefC.GetString(PrefName.ConfirmTextMessage),
                ReminderFilterType.OneWeek => PrefC.GetString(PrefName.ConfirmPostcardMessage),
                ReminderFilterType.TwoWeeks => PrefC.GetString(PrefName.ConfirmPostcardFamMessage),
                _ => throw new ArgumentOutOfRangeException(nameof(filterType), filterType, "Invalid ReminderFilterType value."),
            };
        }



        public static async SystemTask PerformRegularSendSMSTasks()
        {
            bool smsIsWorking = await ODSMS.CheckSMSConnection();
            bool remindersSent = false;
            bool birthdaySent = false;

            ODSMSLogger.Instance.Log("Performing regular SMS sending", EventLogEntryType.Information);

            if (smsIsWorking)
            {
                if (ODSMS.wasSmsBroken)
                {
                    System.Windows.MessageBox.Show("SMS is restored. Birthday texts and reminders will be sent now.");
                    ODSMSLogger.Instance.Log("SMS has been restored", EventLogEntryType.Information);
                    ODSMS.USE_ODSMS = true;
                }
                else if (ODSMS.initialStartup)
                {
                    ODSMSLogger.Instance.Log("SMS is working during initial startup", EventLogEntryType.Information);
                    ODSMS.USE_ODSMS = true;
                    ODSMS.initialStartup = false;
                }

                remindersSent = SendReminderTexts();
                birthdaySent = SendBirthdayTexts();
                await ReceiveSMS.FetchAndProcessSmsMessages("&remove=1");
            }
            else
            {
                if (ODSMS.wasSmsBroken)
                {
                    ODSMSLogger.Instance.Log("SMS is still down", EventLogEntryType.Warning);
                }
                else
                {
                    System.Windows.MessageBox.Show("Failure checking Diafaan! SMS will not send or receive until fixed. Please go to the phone and check the GSM Modem Gateway app and ensure Wi-Fi is enabled. Failing that, check Diafaan manually.");
                    ODSMSLogger.Instance.Log("SMS is down", EventLogEntryType.Error);
                    ODSMS.USE_ODSMS = false;
                }
            }

            ODSMS.wasSmsBroken = !smsIsWorking;
        }

        private static bool SendBirthdayTexts()
        {
            var currentTime = DateTime.Now;
            var birthdaySent = false;

            ODSMS.SanityCheckConstants();

            if (currentTime.Hour < 7)
            {
                return birthdaySent;
            }

            string birthdayMessageTemplate = PrefC.GetString(PrefName.BirthdayPostcardMsg);
            var patientsWithBirthday = GetPatientsWithBirthdayToday();

            List<SmsToMobile> messagesToSend = PrepareBirthdayMessages(patientsWithBirthday, birthdayMessageTemplate);

            if (messagesToSend.Any())
            {
                foreach (var sms in messagesToSend)
                {
                    Console.WriteLine($"To: {sms.MobilePhoneNumber}, Message: {sms.MsgText}");
                }
                if (ODSMS.SEND_SMS)
                {
                    SmsToMobiles.SendSmsMany(messagesToSend);
                    birthdaySent = true;
                }
                else
                {
                    ODSMSLogger.Instance.Log("SMS sending is disabled. Not sending any messages", EventLogEntryType.Warning);
                }
            }
            return birthdaySent;
        }

        private static List<SmsToMobile> PrepareBirthdayMessages(List<Patient> patientsWithBirthday, string birthdayMessageTemplate)
        {
            return patientsWithBirthday.Select(patient =>
                new SmsToMobile
                {
                    PatNum = patient.PatNum,
                    SmsPhoneNumber = ODSMS.PRACTICE_PHONE_NUMBER,
                    MobilePhoneNumber = patient.WirelessPhone,
                    MsgText = ODSMS.RenderReminder(birthdayMessageTemplate, patient, null),
                    MsgType = SmsMessageSource.GeneralMessage,
                    SmsStatus = SmsDeliveryStatus.Pending,
                    MsgParts = 1,
                }).ToList();
        }

        private static List<SmsToMobile> PrepareReminderMessages(List<PatientAppointment> patientsNeedingApptReminder, string reminderMessageTemplate, ReminderFilterType filterType)
        {
            return patientsNeedingApptReminder.Select(pat_appt =>
                new SmsToMobile
                {
                    PatNum = pat_appt.Patient.PatNum,
                    SmsPhoneNumber = ODSMS.PRACTICE_PHONE_NUMBER,
                    MobilePhoneNumber = pat_appt.Patient.WirelessPhone,
                    MsgText = ODSMS.RenderReminder(reminderMessageTemplate, pat_appt.Patient, pat_appt.Appointment),
                    MsgType = SmsMessageSource.Reminder,
                    SmsStatus = SmsDeliveryStatus.Pending,
                    MsgParts = 1,
                }).ToList();
        }

        private static bool SendAndUpdateAppointments(List<SmsToMobile> messagesToSend, List<PatientAppointment> patientsNeedingApptReminder, ReminderFilterType filterType)
        {
            foreach (var sms in messagesToSend)
            {
                ODSMSLogger.Instance.Log($"To: {sms.MobilePhoneNumber}, Message: {sms.MsgText}", EventLogEntryType.Information);
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
                    updatedAppt.Confirmed = GetUpdatedConfirmationStatus(filterType);

                    bool updateSucceeded = true;

                    if (ODSMS.WRITE_TO_DATABASE)
                    {
                        updateSucceeded = AppointmentCrud.Update(updatedAppt, originalAppt);
                    }
                    else
                    {
                        ODSMSLogger.Instance.Log($"Not updating appointment {originalAppt.AptNum} on patient {originalAppt.PatNum} from {originalAppt.Confirmed} to {updatedAppt.Confirmed} as running in debug mode", EventLogEntryType.Warning);
                    }
                    if (updateSucceeded)
                    {
                        ODSMSLogger.Instance.Log($"Updated {originalAppt.AptNum} on patient {originalAppt.PatNum} from {originalAppt.Confirmed} to {updatedAppt.Confirmed}", EventLogEntryType.Information);
                    }
                    else
                    {
                        ODSMSLogger.Instance.Log("Failure updating patient details!", EventLogEntryType.Warning);
                    }
                }

                return appts.Any();
            }
            else
            {
                ODSMSLogger.Instance.Log("SMS sending is disabled. Not sending any messages", EventLogEntryType.Warning);
                return false;
            }
        }

        private static int GetUpdatedConfirmationStatus(ReminderFilterType filterType)
        {
            return filterType switch
            {
                ReminderFilterType.OneDay => (int)ODSMS._defNumTexted,
                ReminderFilterType.OneWeek => (int)ODSMS._defNumOneWeekSent,
                ReminderFilterType.TwoWeeks => (int)ODSMS._defNumTwoWeekSent,
                _ => throw new ArgumentOutOfRangeException(nameof(filterType), filterType, "Invalid ReminderFilterType value."),
            };
        }

        private static bool SendReminderTexts()
        {
            var currentTime = DateTime.Now;
            var patientsTexted = false;

            ODSMS.SanityCheckConstants();

            if (currentTime.Hour < 7)
            {
                return patientsTexted;
            }

            var potentialReminderMessages = Enum.GetValues(typeof(ReminderFilterType));

            foreach (ReminderFilterType currentReminder in potentialReminderMessages)
            {
                List<PatientAppointment> patientsNeedingApptReminder = GetPatientsWithAppointmentsTwoWeeks(currentReminder);
                string reminderMessageTemplate = GetReminderMessageTemplate(currentReminder);

                List<SmsToMobile> messagesToSend = PrepareReminderMessages(patientsNeedingApptReminder, reminderMessageTemplate, currentReminder);

                if (messagesToSend.Any())
                {
                    patientsTexted = SendAndUpdateAppointments(messagesToSend, patientsNeedingApptReminder, currentReminder);
                }
            }

            return patientsTexted;
        }

        private static List<PatientAppointment> GetPatientsWithAppointmentsTwoWeeks(ReminderFilterType filterType)
        {
            int textMessageValue = (int)OpenDentBusiness.ContactMethod.TextMessage;
            int wirelessPhoneValue = (int)OpenDentBusiness.ContactMethod.WirelessPh;
            int noPreferenceValue = (int)OpenDentBusiness.ContactMethod.None;
            DateTime now = DateTime.Now;
            string aptDateTimeRange = filterType switch
            {
                ReminderFilterType.OneDay when now.DayOfWeek == DayOfWeek.Friday =>
                    "DATE(a.AptDateTime) IN (DATE(DATE_ADD(NOW(), INTERVAL 1 DAY)), DATE(DATE_ADD(NOW(), INTERVAL 3 DAY)))",
                ReminderFilterType.OneDay =>
                    "DATE(a.AptDateTime) = DATE(DATE_ADD(NOW(), INTERVAL 1 DAY))",
                ReminderFilterType.OneWeek => "DATE(a.AptDateTime) = DATE(DATE_ADD(NOW(), INTERVAL 1 WEEK))",
                ReminderFilterType.TwoWeeks => "DATE(a.AptDateTime) = DATE(DATE_ADD(NOW(), INTERVAL 2 WEEK))",
                _ => throw new ArgumentOutOfRangeException(nameof(filterType), filterType, "Invalid ReminderFilterType value."),
            };
            string select = "SELECT p.*, a.* ";
            string from = "FROM patient AS p JOIN Appointment as a using (PatNum) ";
            string where_true = "WHERE TRUE ";
            string where_allow_sms = "AND p.TxtMsgOk < 2 ";
            string where_confirm_not_sms = $"AND p.PreferConfirmMethod IN ({noPreferenceValue}, {wirelessPhoneValue}, {textMessageValue}) ";
            string where_no_intermediate_appointments = "AND NOT EXISTS (SELECT 1 FROM Appointment a2 WHERE a2.AptDateTime > NOW() AND a2.AptDateTime < a.AptDateTime AND a2.PatNum = a.PatNum) ";
            string where_mobile_phone = "AND LENGTH(COALESCE(p.WirelessPhone,'')) > 7 ";
            string where_appointment_date = $"AND {aptDateTimeRange} ";
            string where_appointment_confirmed = GetAppointmentConfirmedWhereClause(filterType);
            string where_scheduled = $"AND a.AptStatus = {(int)OpenDentBusiness.ApptStatus.Scheduled} ";

            string command = select + from + where_true + where_appointment_date + where_appointment_confirmed + where_mobile_phone + where_allow_sms + where_confirm_not_sms + where_no_intermediate_appointments + where_scheduled;
            ODSMSLogger.Instance.Log(command, EventLogEntryType.Information, logToEventLog: false);
            Console.WriteLine(command);
            List<PatientAppointment> listPatAppts = OpenDentBusiness.Crud.PatientApptCrud.SelectMany(command);
            return listPatAppts;
        }

        public static async SystemTask SendSMSForever()
        {
            while (true)
            {
                DateTime now = DateTime.Now;

                if (now.Minute >= 14 && now.Minute <= 16 && now.Hour >= 8 && now.Hour <= 17)
                {
                    await PerformRegularSendSMSTasks();
                }

                int minutesUntilNextQuarterPast = GetMinutesUntilQuarterPast(now);
                await SystemTask.Delay(TimeSpan.FromMinutes(minutesUntilNextQuarterPast));
            }
        }


        private static string GetAppointmentConfirmedWhereClause(ReminderFilterType filterType)
        {
            return filterType switch
            {
                ReminderFilterType.OneDay => $"AND a.Confirmed IN ({ODSMS._defNumWebSched},{ODSMS._defNumNotCalled}, {ODSMS._defNumUnconfirmed}, {ODSMS._defNumOneWeekConfirmed}, {ODSMS._defNumTwoWeekConfirmed}, {ODSMS._defNumOneWeekSent}, {ODSMS._defNumTwoWeekSent}) ",
                ReminderFilterType.OneWeek => $"AND a.Confirmed IN ({ODSMS._defNumWebSched},{ODSMS._defNumNotCalled}, {ODSMS._defNumUnconfirmed}, {ODSMS._defNumTwoWeekConfirmed}, {ODSMS._defNumTwoWeekSent}) ",
                ReminderFilterType.TwoWeeks => $"AND a.Confirmed IN ({ODSMS._defNumWebSched},{ODSMS._defNumNotCalled}, {ODSMS._defNumUnconfirmed}) ",
                _ => throw new ArgumentOutOfRangeException(nameof(filterType), filterType, "Invalid ReminderFilterType value."),
            };
        }
    }
};

