using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPerioGraphicalSetup:FormODBase {

		public FormPerioGraphicalSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPerioGraphicalSetup_Load(object sender,EventArgs e) {
			this.butColorCal.BackColor=PrefC.GetColor(PrefName.PerioColorCAL);
			this.butColorFurc.BackColor=PrefC.GetColor(PrefName.PerioColorFurcations);
			this.butColorFurcRed.BackColor=PrefC.GetColor(PrefName.PerioColorFurcationsRed);
			this.butColorGM.BackColor=PrefC.GetColor(PrefName.PerioColorGM);
			this.butColorMGJ.BackColor=PrefC.GetColor(PrefName.PerioColorMGJ);	
			this.butColorProbing.BackColor=PrefC.GetColor(PrefName.PerioColorProbing);
			this.butColorProbingRed.BackColor=PrefC.GetColor(PrefName.PerioColorProbingRed);
		}

		private void butColorCal_Click(object sender,EventArgs e) {
			this.colorPicker.Color=this.butColorCal.BackColor;
			if(this.colorPicker.ShowDialog()==DialogResult.OK) {
				this.butColorCal.BackColor=this.colorPicker.Color;
			}
			colorPicker.Dispose();
		}

		private void butColorFurc_Click(object sender,EventArgs e) {
			this.colorPicker.Color=this.butColorFurc.BackColor;
			if(this.colorPicker.ShowDialog()==DialogResult.OK) {
				this.butColorFurc.BackColor=this.colorPicker.Color;
			}
			colorPicker.Dispose();
		}

		private void butColorFurcRed_Click(object sender,EventArgs e) {
			this.colorPicker.Color=this.butColorFurcRed.BackColor;
			if(this.colorPicker.ShowDialog()==DialogResult.OK) {
				this.butColorFurcRed.BackColor=this.colorPicker.Color;
			}
			colorPicker.Dispose();
		}

		private void butColorGM_Click(object sender,EventArgs e) {
			this.colorPicker.Color=this.butColorGM.BackColor;
			if(this.colorPicker.ShowDialog()==DialogResult.OK){
				this.butColorGM.BackColor=this.colorPicker.Color;
			}
			colorPicker.Dispose();
		}

		private void butColorMGJ_Click(object sender,EventArgs e) {
			this.colorPicker.Color=this.butColorMGJ.BackColor;
			if(this.colorPicker.ShowDialog()==DialogResult.OK){
				this.butColorMGJ.BackColor=this.colorPicker.Color;
			}
			colorPicker.Dispose();
		}

		private void butColorProbing_Click(object sender,EventArgs e) {
			this.colorPicker.Color=this.butColorProbing.BackColor;
			if(this.colorPicker.ShowDialog()==DialogResult.OK){
				this.butColorProbing.BackColor=this.colorPicker.Color;
			}
			colorPicker.Dispose();
		}

		private void butColorProbingRed_Click(object sender,EventArgs e) {
			this.colorPicker.Color=this.butColorProbingRed.BackColor;
			if(this.colorPicker.ShowDialog()==DialogResult.OK){
				this.butColorProbingRed.BackColor=this.colorPicker.Color;
			}
			colorPicker.Dispose();
		}

		private void butOK_Click(object sender,EventArgs e) {
			Prefs.UpdateLong(PrefName.PerioColorCAL,this.butColorCal.BackColor.ToArgb());
			Prefs.UpdateLong(PrefName.PerioColorFurcations,this.butColorFurc.BackColor.ToArgb());
			Prefs.UpdateLong(PrefName.PerioColorFurcationsRed,this.butColorFurcRed.BackColor.ToArgb());
			Prefs.UpdateLong(PrefName.PerioColorGM,this.butColorGM.BackColor.ToArgb());
			Prefs.UpdateLong(PrefName.PerioColorMGJ,this.butColorMGJ.BackColor.ToArgb());
			Prefs.UpdateLong(PrefName.PerioColorProbing,this.butColorProbing.BackColor.ToArgb());
			Prefs.UpdateLong(PrefName.PerioColorProbingRed,this.butColorProbingRed.BackColor.ToArgb());
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}