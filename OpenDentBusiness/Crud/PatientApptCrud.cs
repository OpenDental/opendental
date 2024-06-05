using OpenDentBusiness.Crud;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Data;

namespace OpenDentBusiness.Crud
{
    public class PatientApptCrud
    {
        public static PatientAppointment SelectOne(long patNum, long aptNum)
        {
            string command = "SELECT p.*, a.* FROM patient p "
                             + "JOIN Appointment a ON p.PatNum = a.PatNum "
                             + "WHERE p.PatNum = " + POut.Long(patNum)
                             + " AND a.AptNum = " + POut.Long(aptNum);
            List<PatientAppointment> list = SelectMany(command);
            return list.FirstOrDefault();
        }

        public static PatientAppointment SelectOne(string command)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                throw new ApplicationException("Not allowed to send SQL directly. Rewrite the calling class to not use this query:\r\n" + command);
            }
            List<PatientAppointment> list = SelectMany(command);
            return list.FirstOrDefault();
        }

        public static List<PatientAppointment> SelectMany(string command)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                throw new ApplicationException("Not allowed to send SQL directly.");
            }

            DataTable table = Db.GetTable(command);

            List<Patient> patients = PatientCrud.TableToList(table);
            List<Appointment> appointments = AppointmentCrud.TableToList(table);

            List<PatientAppointment> patientAppointments = patients
                .Zip(appointments, (p, a) => new PatientAppointment { Patient = p, Appointment = a })
                .Where(pa => pa.Patient != null && pa.Appointment != null)
                .ToList();
            return patientAppointments;
        }
    }
}