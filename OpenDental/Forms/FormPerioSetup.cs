using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPerioSetup:FormODBase {

		public FormPerioSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			FillTextBoxes();
		}

		private void FillTextBoxes() {
			string rawPerioMeasures=PrefC.GetString(PrefName.PerioDefaultProbeDepths);
			if(string.IsNullOrEmpty(rawPerioMeasures) || rawPerioMeasures.Length!=192) {
				return;
			}
			textF1.Text=GetProbingDepthsWithCommas(rawPerioMeasures.Substring(0,48));
			textL1.Text=GetProbingDepthsWithCommas(rawPerioMeasures.Substring(48,48));
			textL32.Text=GetProbingDepthsWithCommas(rawPerioMeasures.Substring(96,48));
			textF32.Text=GetProbingDepthsWithCommas(rawPerioMeasures.Substring(144));
		}

		///<summary>Simply returns the string passed in but with a comma added after every third character.</summary>
		private string GetProbingDepthsWithCommas(string probingDepthsAll) {
			string retVal="";
			for(int i=0;i<probingDepthsAll.Length;i++) {
				retVal+=probingDepthsAll[i];
				if((i+1)%3==0 && i!=47) {//Check for every third number to insert comma.
					retVal+=",";
				}
			}
			return retVal;
		}

		///<summary>Inserts a comma into the current textbox after every 3 values. Only triggers when typing at the end of the textbox and if the key press was a numeric value.</summary>
		private void OnKeyUp(object sender,KeyEventArgs e) {
			//value is already changed in the textbox at this point
			base.OnKeyUp(e);
			if(!(e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
				&& !(e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
			{
				return;
			}
			TextBox textBox=(TextBox)sender;
			if(textBox.TextLength%4==3 && textBox.SelectionStart==textBox.TextLength && textBox.TextLength!=63) {
				textBox.Text+=',';
				textBox.SelectionStart=textBox.TextLength;
			}
			ShiftFocus();
		}

		///<summary>Shifts the texbox focus to the end of the next non-full textbox. If all are filled, shifts focus to the end of the last textbox. Occurs when the current textbox is full and cursor is at the end of the textbox.</summary>
		private void ShiftFocus() {
			if(textF1.Focused && textF1.TextLength==63 && textF1.SelectionStart==textF1.TextLength) {
				textL1.Focus();
				textL1.SelectionStart=textL1.TextLength;
			}
			if(textL1.Focused && textL1.TextLength==63 && textL1.SelectionStart==textL1.TextLength) {
				textL32.Focus();
				textL32.SelectionStart=textL32.TextLength;
			}
			if(textL32.Focused && textL32.TextLength==63 && textL32.SelectionStart==textL32.TextLength) {
				textF32.Focus();
				textF32.SelectionStart=textF32.TextLength;
			}
		}

		private bool IsValid() {
			if(textF1.TextLength==0 && textF32.TextLength==0 && textL1.TextLength==0 && textL32.TextLength==0) { //All textboxes empty.
				return true;
			}
			if(!(textF1.TextLength==63 && textF32.TextLength==63 && textL1.TextLength==63 && textL32.TextLength==63)) {//63 for 48 digits + 15 commas.
				MsgBox.Show(this,"Invalid entry. Please fill out all 4 rows and enter 3 values for each tooth.");
				return false;
			}
			for(int i=0;i<63;i++) {
				if((i+1)%4==0) { //Comma character check.
					if(textF1.Text[i]!=',' 
						|| textF32.Text[i]!=','
						|| textL1.Text[i]!=',' 
						|| textL32.Text[i]!=',') 
					{
						MsgBox.Show(this,"Invalid entry. Please make sure there is a comma every 3 values.");
						return false;
					}
					continue;
				}
				//Numeric character check.
				if(!Char.IsDigit(textF1.Text[i]) 
					|| !Char.IsDigit(textF32.Text[i]) 
					|| !Char.IsDigit(textL1.Text[i]) 
					|| !Char.IsDigit(textL32.Text[i])) 
				{
					MsgBox.Show(this,"Invalid entry. Please ensure all non-comma values are numeric [0-9].");
					return false;
				}
			}
			return true;
		}

		private void butAll323_Click(object sender,EventArgs e) {
			textF1.Text="323,323,323,323,323,323,323,323,323,323,323,323,323,323,323,323";
			textF32.Text="323,323,323,323,323,323,323,323,323,323,323,323,323,323,323,323";
			textL1.Text="323,323,323,323,323,323,323,323,323,323,323,323,323,323,323,323";
			textL32.Text="323,323,323,323,323,323,323,323,323,323,323,323,323,323,323,323";
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			if(textF1.Text=="" && textF32.Text=="" && textL1.Text=="" && textL32.Text=="") {
				if(Prefs.UpdateString(PrefName.PerioDefaultProbeDepths,"")) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
				DialogResult=DialogResult.OK;
				return;
			}
			string formattedPerioMesureRow=textF1.Text.Replace(",","");
			formattedPerioMesureRow+=textL1.Text.Replace(",","");
			formattedPerioMesureRow+=textL32.Text.Replace(",","");
			formattedPerioMesureRow+=textF32.Text.Replace(",","");
			if(Prefs.UpdateString(PrefName.PerioDefaultProbeDepths,formattedPerioMesureRow)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}