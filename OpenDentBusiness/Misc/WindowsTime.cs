using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class WindowsTime {

	[DllImport("kernel32.dll", SetLastError=true)]
	private extern static uint SetLocalTime(ref SYSTEMTIME lpSystemTime);

	[StructLayout(LayoutKind.Sequential)]
	private struct SYSTEMTIME {
		public ushort wYear;
		public ushort wMonth;
		public ushort wDayOfWeek;
		public ushort wDay;
		public ushort wHour;
		public ushort wMinute;
		public ushort wSecond;
		public ushort wMilliseconds;
	}

	public WindowsTime() {
	}

	///<summary>Set the windows system time.</summary>
	public static void SetTime(DateTime newTime) {
		// Call the native SetLocalTime method 
		// with the defined structure.
		SYSTEMTIME systime=new SYSTEMTIME();
		systime.wYear=(ushort)newTime.Year;
		systime.wMonth=(ushort)newTime.Month;
		systime.wDayOfWeek=(ushort)newTime.DayOfWeek;
		systime.wDay=(ushort)newTime.Day;
		systime.wHour=(ushort)newTime.Hour;
		systime.wMinute=(ushort)newTime.Minute;
		systime.wSecond=(ushort)newTime.Second;
		systime.wMilliseconds=(ushort)newTime.Millisecond;
		SetLocalTime(ref systime);
		string messageText="System date and time set to:  "+newTime.ToString("MM/dd/yyyy hh:mm:ss.fff tt")+".";
		EventLog.WriteEntry("OpenDental",messageText,EventLogEntryType.Information);
	}
}

