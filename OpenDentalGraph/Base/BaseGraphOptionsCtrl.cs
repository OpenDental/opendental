using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenDentalGraph {
	public partial class BaseGraphOptionsCtrl:UserControl {
		public event EventHandler InputsChanged;
		public BaseGraphOptionsCtrl() {
			InitializeComponent();			
		}
		
		protected void OnBaseInputsChanged(object sender,EventArgs e) {
			if((sender is RadioButton)&&!((RadioButton)sender).Checked) { //Another event is coming shorts that will be for the newly checked radio button. Wait for that one to avoid double processing.
				return;
			}	
			if(InputsChanged!=null) {
				InputsChanged(this,new EventArgs());
			}
		}

		public virtual int GetPanelHeight() {
			return 63;
		}

		///<summary>If you override this and your override can return true, make sure you check to see if Clinics are enabled before showing the grouping options.</summary>
		public virtual bool HasGroupOptions {
			get {
				if(OpenDentBusiness.PrefC.HasClinicsEnabled) { //reference to OpenDentBusiness
					return true;
				}
				else {
					return false;
				}
			}
		}
	}
}
