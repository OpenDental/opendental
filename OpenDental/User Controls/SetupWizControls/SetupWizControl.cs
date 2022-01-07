using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using OpenDental;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental.User_Controls.SetupWizard {
	public partial class SetupWizControl:UserControl {
		///<summary>Set IsDone to true when the user has completed setting the control up. 
		///This must be set to true at some point within the control that extends this.</summary>
		public bool IsDone = false;
		///<summary>Make sure to translate before setting this string.</summary>
		public string StrIncomplete = "Please fill in the missing fields first.";
		///<summary>Add an event to this if something needs to be done before the user exits the control, such as validation or error checking.
		///Return a boolean.</summary>
		public delegate bool ControlValidated(object sender, EventArgs e);
		public ControlValidated OnControlValidated;
		///<summary>Add an event to this if something needs to be done when the user exits the control.
		///For example, if database calls need to be made when the user clicks "Next".</summary>
		public EventHandler OnControlDone;

	}
}
