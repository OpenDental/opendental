using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataConnectionBase;
using OpenDentBusiness;
using OpenDentBusiness.Crud;
using OpenDentBusiness.Mobile;

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
        private static List<Def> _listDefsApptConfirmed;
        private static long _defNumTwoWeekConfirmed;
        private static long _defNumOneWeekConfirmed;
        private static long _defNumConfirmed;
        private static long _defNumNotCalled;
        private static long _defNumUnconfirmed;
        private static long _defNumTwoWeekSent;
        private static long _defNumOneWeekSent;
        private static long _defNumTexted;
        private static long _defNumWebSched;

        public static async System.Threading.Tasks.Task InitializeSendSMS()
        {
            while (!DataConnection.HasDatabaseConnection)
            {
                Console.WriteLine("Waiting for database connection...");
                await System.Threading.Tasks.Task.Delay(5000);
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
        }

        private static long GetConfirmationStatus(int daysUntilAppointment)
        {
            if (daysUntilAppointment >= 14 && daysUntilAppointment < 22)
            {
                return _defNumTwoWeekConfirmed;
            }
            else if (daysUntilAppointment >= 7)
            {
                return _defNumOneWeekConfirmed;
            }
            else if (daysUntilAppointment >= 0 && daysUntilAppointment <= 4)
            {
                return _defNumConfirmed;
            }
            else
            {
                string logMessage = $"Received YES to an appointment that is {daysUntilAppointment} days away. This is unexpected.";
                ODSMSLogger.Instance.Log(logMessage, EventLogEntryType.Warning);
                return 0;
            }
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
            switch (filterType)
            {
                case ReminderFilterType.OneDay:
                    return PrefC.GetString(PrefName.ConfirmTextMessage);
                case ReminderFilterType.OneWeek:
                    return PrefC.GetString(PrefName.ConfirmPostcardMessage);
                case ReminderFilterType.TwoWeeks:
                    return PrefC.GetString(PrefName.ConfirmPostcardFamMessage);
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterType), filterType, "Invalid ReminderFilterType value.");
            }
        }

        private static bool IsAppointmentAlreadyConfirmed(Appointment appointment)
        {
            return appointment.Confirmed != _defNumTexted &&
                    appointment.Confirmed != _defNumOneWeekSent &&
                    appointment.Confirmed != _defNumTwoWeekSent;
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

        public static async System.Threading.Tasks.Task PerformDailySMSTasks()
        {
            bool smsIsWorking = await ODSMS.CheckSMSConnection();
            bool remindersSent = false;
            bool birthdaySent = false;

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
                    MsgText = ODSMS.renderReminder(birthdayMessageTemplate, patient, null),
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
                    MsgText = ODSMS.renderReminder(reminderMessageTemplate, pat_appt.Patient, pat_appt.Appointment),
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
            switch (filterType)
            {
                case ReminderFilterType.OneDay:
                    return (int)_defNumTexted;
                case ReminderFilterType.OneWeek:
                    return (int)_defNumOneWeekSent;
                case ReminderFilterType.TwoWeeks:
                    return (int)_defNumTwoWeekSent;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterType), filterType, "Invalid ReminderFilterType value.");
            }
        }

        private static bool SendReminderTexts()
        {
            var currentTime = DateTime.Now;
            var patientsTexted = false;

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



        public static bool HandleAutomatedConfirmation(List<Patient> patientList)
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
                        return false;
                    }

                    string appointmentQuery = $"SELECT * FROM Appointment WHERE PatNum = {PatNum} AND AptDateTime = '{appointmentTime.Value.ToString("yyyy-MM-dd HH:mm:ss")}'";
                    Appointment originalAppt = OpenDentBusiness.Crud.AppointmentCrud.SelectOne(appointmentQuery);

                    if (originalAppt != null)
                    {
                        if (IsAppointmentAlreadyConfirmed(originalAppt))
                        {
                            ODSMSLogger.Instance.Log("OOPS! Patient just replied yes to an appointment that is already confirmed. Ignoring", EventLogEntryType.Warning);
                            return false;
                        }
                        else
                        {
                            return UpdateAppointmentStatus(originalAppt, confirmationStatus);
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

        private static List<PatientAppointment> GetPatientsWithAppointmentsTwoWeeks(ReminderFilterType filterType)
        {
            int textMessageValue = (int)OpenDentBusiness.ContactMethod.TextMessage;
            int noPreferenceValue = (int)OpenDentBusiness.ContactMethod.None;

            string aptDateTimeRange;
            DateTime now = DateTime.Now;

            switch (filterType)
            {
                case ReminderFilterType.OneDay:
                    aptDateTimeRange = now.DayOfWeek == DayOfWeek.Friday
                        ? "DATE(a.AptDateTime) IN (DATE(DATE_ADD(NOW(), INTERVAL 1 DAY)), DATE(DATE_ADD(NOW(), INTERVAL 3 DAY)))"
                        : "DATE(a.AptDateTime) = DATE(DATE_ADD(NOW(), INTERVAL 1 DAY))";
                    break;
                case ReminderFilterType.OneWeek:
                    aptDateTimeRange = "DATE(a.AptDateTime) = DATE(DATE_ADD(NOW(), INTERVAL 1 WEEK))";
                    break;
                case ReminderFilterType.TwoWeeks:
                    aptDateTimeRange = "DATE(a.AptDateTime) = DATE(DATE_ADD(NOW(), INTERVAL 2 WEEK))";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterType), filterType, "Invalid ReminderFilterType value.");
            }

            string select = "SELECT p.*, a.* ";
            string from = "FROM patient AS p JOIN Appointment as a using (PatNum) ";
            string where_true = "WHERE TRUE ";
            string where_allow_sms = "AND p.TxtMsgOk < 2 ";
            string where_confirm_not_sms = $"AND p.PreferConfirmMethod IN ({noPreferenceValue}, {textMessageValue}) ";
            string where_no_intermediate_appointments = "AND NOT EXISTS (SELECT 1 FROM Appointment a2 WHERE a2.AptDateTime > NOW() AND a2.AptDateTime < a.AptDateTime AND a2.PatNum = a.PatNum) ";
            string where_mobile_phone = "AND LENGTH(COALESCE(p.WirelessPhone,'')) > 7 ";
            string where_appointment_date = $"AND {aptDateTimeRange} ";
            string where_appointment_confirmed = GetAppointmentConfirmedWhereClause(filterType);
            string where_scheduled = $"AND a.AptStatus = {(int)OpenDentBusiness.ApptStatus.Scheduled} ";

            string command = select + from + where_true + where_appointment_date + where_appointment_confirmed + where_mobile_phone + where_allow_sms + where_confirm_not_sms + where_no_intermediate_appointments + where_scheduled;
            Console.WriteLine(command);
            List<PatientAppointment> listPatAppts = OpenDentBusiness.Crud.PatientApptCrud.SelectMany(command);
            return listPatAppts;
        }

        private static string GetAppointmentConfirmedWhereClause(ReminderFilterType filterType)
        {
            switch (filterType)
            {
                case ReminderFilterType.OneDay:
                    return $"AND a.Confirmed IN ({_defNumWebSched},{_defNumNotCalled}, {_defNumUnconfirmed}, {_defNumOneWeekConfirmed}, {_defNumTwoWeekConfirmed}, {_defNumOneWeekSent}, {_defNumTwoWeekSent}) ";
                case ReminderFilterType.OneWeek:
                    return $"AND a.Confirmed IN ({_defNumWebSched},{_defNumNotCalled}, {_defNumUnconfirmed}, {_defNumTwoWeekConfirmed}, {_defNumTwoWeekSent}) ";
                case ReminderFilterType.TwoWeeks:
                    return $"AND a.Confirmed IN ({_defNumWebSched},{_defNumNotCalled}, {_defNumUnconfirmed}) ";
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterType), filterType, "Invalid ReminderFilterType value.");
            }
        }
    }
};

