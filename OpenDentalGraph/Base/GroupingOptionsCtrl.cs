using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenDentalGraph {
	public partial class GroupingOptionsCtrl:UserControl {
		public event EventHandler InputsChanged;
		public enum Grouping { provider, clinic };
		public Grouping CurGrouping{
			get {
				if(radioGroupProvs.Checked) {
					return Grouping.provider;
				}
				else {
					return Grouping.clinic;
				}
			}
			set {
				switch(value) {
					case Grouping.provider:
						radioGroupProvs.Checked=true;
						break;
					case Grouping.clinic:
						radioGroupClinics.Checked=true;
						break;
				}
			}
		}

		public GroupingOptionsCtrl() {
			InitializeComponent();			
		}

		protected void OnBaseInputsChanged(object sender,EventArgs e) {
			if(InputsChanged!=null) {
				InputsChanged(this,new EventArgs());
			}
		}
		
		private void radioGroupByChanged(object sender,EventArgs e) {
			if((sender is RadioButton) && !((RadioButton)sender).Checked) {
				return;
			}
			OnBaseInputsChanged(sender,e);
		}
	}
}
