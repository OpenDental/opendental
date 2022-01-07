using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormEClipboardSheetRules:FormODBase {
		EClipboardSheetDef _eSheet;

		public FormEClipboardSheetRules(EClipboardSheetDef eClipboardSheetDef) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_eSheet=eClipboardSheetDef;
		}

		private void FormEClipboardSheetRules_Load(object sender,EventArgs e) {
			//Get the name of the sheet
			textSheet.Text=SheetDefs.GetDescription(_eSheet.SheetDefNum);
			comboBehavior.Items.AddEnums<PrefillStatuses>();
			comboBehavior.SetSelectedEnum(_eSheet.PrefillStatus);
			textFrequency.Text=_eSheet.ResubmitInterval.TotalDays.ToString();
			checkMinAge.Checked=false;
			textMinAge.Enabled=false;
			if(_eSheet.MinAge>0) {
				checkMinAge.Checked=true;
				textMinAge.Enabled=true;
				textMinAge.Text=_eSheet.MinAge.ToString();
			}
			checkMaxAge.Checked=false;
			textMaxAge.Enabled=false;
			if(_eSheet.MaxAge>0) {
				checkMaxAge.Checked=true;
				textMaxAge.Enabled=true;
				textMaxAge.Text=_eSheet.MaxAge.ToString();
			}
		}

		private void checkMinAge_CheckedChanged(object sender,EventArgs e) {
			textMinAge.Enabled=checkMinAge.Checked;
			if(!checkMinAge.Checked) {
				textMinAge.Text="";
			}
		}

		private void checkMaxAge_CheckedChanged(object sender,EventArgs e) {
			textMaxAge.Enabled=checkMaxAge.Checked;
			if(!checkMaxAge.Checked) {
				textMaxAge.Text="";
			}
		}

		private void comboBehavior_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboBehavior.GetSelected<PrefillStatuses>()==PrefillStatuses.Once) {
				textFrequency.Text="0";
			}
		}

		private void textFrequency_TextChanged(object sender,EventArgs e) {
			if(textFrequency.Text=="0") {
				comboBehavior.SetSelectedEnum(PrefillStatuses.Once);
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender,EventArgs e) {
			int days;
			if(!int.TryParse(textFrequency.Text,out days)) {
				MsgBox.Show(this,"Frequency (days) must be a valid whole number.");
				return;
			}
			if(days<0) {
				MsgBox.Show(this,"Frequency (days) must be greater than -1.");
				return;
			}
			int minAge=-1;
			if(checkMinAge.Checked && !int.TryParse(textMinAge.Text,out minAge)) {
				MsgBox.Show(this,"The minimum age must be a valid whole number.");
				return;
			}
			if(checkMinAge.Checked && minAge<1) {
				MsgBox.Show(this,"The minimum age must be greater than 0.");
				return;
			}
			int maxAge=-1;
			if(checkMaxAge.Checked && !int.TryParse(textMaxAge.Text,out maxAge)) {
				MsgBox.Show(this,"The maximum age must be a valid whole number.");
				return;
			}
			if(checkMaxAge.Checked && maxAge<1) {
				MsgBox.Show(this,"The maximum age must be greater than 0.");
				return;
			}
			if(checkMaxAge.Checked && checkMinAge.Checked && maxAge<minAge) {
				MsgBox.Show(this,"The maximum age must be greater than the minimum age.");
				return;
			}
			_eSheet.PrefillStatus=comboBehavior.GetSelected<PrefillStatuses>();
			if(_eSheet.PrefillStatus==PrefillStatuses.Once) {
				days=0;
			}
			_eSheet.MinAge=minAge;
			_eSheet.MaxAge=maxAge;
			_eSheet.ResubmitInterval=TimeSpan.FromDays(days);
			//EClipboardSheetDefs.Update(_eSheet);
			DialogResult=DialogResult.OK;
		}

	}
}
