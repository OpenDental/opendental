using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;
using System.Linq;

namespace UnitTestsCore {
	public class AutoNoteControlT {

		///<summary>Creates an autonotecontrol. Set doInsert to true to insert the new autonotecontrol.</summary>
		public static AutoNoteControl CreateAutoNoteControl(string descript="",string controlType="",string controlLabel="",string controlOptions="",bool doInsert=true) {
			AutoNoteControl autoNoteControl=new AutoNoteControl();
			autoNoteControl.Descript=descript;
			autoNoteControl.ControlType=controlType;
			autoNoteControl.ControlLabel=controlLabel;
			autoNoteControl.ControlOptions=controlOptions;
			if(doInsert) {
				AutoNoteControls.Insert(autoNoteControl);
			}
			return autoNoteControl;
		}

		///<summary>Deletes everything from the autonotecontrol table. Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearAutoNoteControlTable() {
			string command="DELETE FROM autonotecontrol WHERE AutoNoteControlNum > 0";
			DataCore.NonQ(command);
		}
	}
}
