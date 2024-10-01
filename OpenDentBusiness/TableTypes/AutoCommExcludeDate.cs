using System;
using System.ComponentModel;

namespace OpenDentBusiness {
    /// <summary>AutoComms are sent a certain number of days in advance. Clinicpref called eConfirmExcludeDays handles excluding weekends, and this table handles excluding holidays. So AutoComms only go out when office is open. (First iteration currently only applies to eConfirmations)</summary>
    public class AutoCommExcludeDate : TableBase {
        ///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey = true)]
        public long AutoCommExcludeDateNum;
        /// <summary>ClinicNum this row applies to. 0 for HQ</summary>
        public long ClinicNum;
        [CrudColumn(SpecialType=CrudSpecialColType.DateT)]
        /// <summary>Date for which Auto Communications will not be sent.</summary>
        public DateTime DateExclude;

        /// <summary>Bit flag for tracking excluded days of the week for AutoCommExclusions. Stored in preference and clinic pref as a single byte.
        /// One can translate a 0 based array of Days of the Week using calculations listed below.
        /// No one should ever have their preference set to include all 7 days. NEVER DO THIS.</summary>
        [Flags]
        public enum AutoCommExcludeDays {
            /// <summary>Indicates that no days are selected to be excluded</summary>
            [Description("None")]
            None = 0,
            [Description("Sunday")]
            /// <summary>2 ^ 0</summary>
            Sunday = 1,
            [Description("Monday")]
            /// <summary>2 ^ 1</summary>
            Monday = 2,
            [Description("Tuesday")]
            /// <summary>2 ^ 2</summary>
            Tuesday = 4,
            [Description("Wednesday")]
            /// <summary>2 ^ 3</summary>
            Wednesday = 8,
            [Description("Thursday")]
            /// <summary>2 ^ 4/summary>
            Thursday = 16,
            [Description("Friday")]
            /// <summary>2 ^ 5</summary>
            Friday = 32,
            [Description("Saturday")]
            /// <summary>2 ^ 6</summary>
            Saturday = 64,
        }


        public static AutoCommExcludeDays ConvertDayToAutoCommExcludeDays(DayOfWeek dt) {
            switch(dt) {
                case DayOfWeek.Sunday:
                    return AutoCommExcludeDays.Sunday;
                case DayOfWeek.Monday:
                    return AutoCommExcludeDays.Monday;
                case DayOfWeek.Tuesday:
                    return AutoCommExcludeDays.Tuesday;
                case DayOfWeek.Wednesday:
                    return AutoCommExcludeDays.Wednesday;
                case DayOfWeek.Thursday:
                    return AutoCommExcludeDays.Thursday;
                case DayOfWeek.Friday:
                    return AutoCommExcludeDays.Friday;
                case DayOfWeek.Saturday:
                    return AutoCommExcludeDays.Saturday;
                default:
                    return AutoCommExcludeDays.None;
            }
        }
    }
}
