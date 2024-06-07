using System;
using System.Globalization;


namespace OpenDental.Main_Modules
{

    internal static class AppointmentHelper
    {
        public static DateTime? ExtractAppointmentDate(string note)
        {
            string pattern = "on ddd, d MMM yyyy at h:mm tt";
            int index = note.IndexOf("on");
            if (index == -1)
            {
                return null;
            }

            string dateStr = note.Substring(index);

            try
            {
                DateTime appointmentTime = DateTime.ParseExact(dateStr, pattern, CultureInfo.InvariantCulture);
                return appointmentTime;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public static bool TestAppointmentHelper()
        {
            bool allPass = true;
            string[] testNotes = new string[]
            {
            "Hi Riana, Your dental appointment is on Saturday, 8 June 2024 at 4:20 pm at Massey Smiles. Please reply \"YES\" ASAP to confirm  (or call 09 833 8182 immediately if there are any issues).  Please reschedule if you are at all unwell.  We're looking forward to seeing you. Massey Smiles",
            "Hi Riana, Could you please urgently confirm your dental appointment for tomorrow, Saturday, 8th June 2024, at 4:20 pm? Please reply ASAP YES  or call us on 833 8182. Thanks, Leanne Massey Smiles. Massey Smiles",
            "Hi Bhawesh, This is Leanne from Massey Smiles. Could you please confirm your dental appointment for tomorrow, Friday, 7 June 2024, at 4:20 pm at Massey Smiles? Please reply \"YES\" ASAP to confirm  (or call 09 833 8182 )",
            "Hi Todd, Massey Smiles here; this is a two-week reminder for your pre-booked dental appointment on Monday, 17 June 2024 at 9:20 am. Please check your schedule now, and let us know if there are any issues via text or phone at 098338182. We will send another text reminder next week and a final one the day before confirmation. We ask for a minimum of 48 hours notice should you wish to reschedule, as this gives us time to offer the appointment to others in need. , regards Leanne",
            "Hi Todd, a sooner appointment has come up on Monday, 10 June 2024 at 8:00 am at Massey Smiles . Please call (09) 833 8182 if you'd like this slot.  This appointment time may have been offered to more than one person and will be allocated on a first come, first served basis"
            };

            DateTime?[] expectedDates = new DateTime?[]
            {
            new DateTime(2024, 6, 8, 16, 20, 0),
            new DateTime(2024, 6, 8, 16, 20, 0),
            new DateTime(2024, 6, 7, 16, 20, 0),
            new DateTime(2024, 6, 17, 9, 20, 0),
            new DateTime(2024, 6, 10, 8, 0, 0)
            };

            for (int i = 0; i < testNotes.Length; i++)
            {
                DateTime? actualDate = ExtractAppointmentDate(testNotes[i]);
                Console.WriteLine($"Test {i + 1}:");
                Console.WriteLine($"Expected: {expectedDates[i]}");
                Console.WriteLine($"Actual: {actualDate}");
                Console.WriteLine($"Result: {(expectedDates[i] == actualDate ? "Pass" : "Fail")}");
                if (expectedDates[i] != actualDate)
                {
                    allPass = false;
                }   
                Console.WriteLine();
            }
            return allPass;
        }
    }

}