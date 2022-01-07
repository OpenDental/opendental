using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UnitTests {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new Form2dDrawingTests());
			//Application.Run(new Form4kTests());
			//Application.Run(new FormApptProvSliderTest());
			//Application.Run(new FormButtonTest());
			//Application.Run(new FormComboTests());
			//Application.Run(new FormDatePickerTests());
			//Application.Run(new FormGridTest());
			//Application.Run(new FormGroupBoxTests());
			//Application.Run(new FormMonthCalendarTests());
			//Application.Run(new FormImageSelectorTests());
			//Application.Run(new FormProgressTests());
			//Application.Run(new FormSandboxJordan());
			Application.Run(new FormValidTextTests());
		}
	}
}