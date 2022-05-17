using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormSheetFieldChart:FormODBase {
		///<summary>This is the object we are editing.</summary>
		public SheetFieldDef SheetFieldDefCur;
		///<summary>We need access to a few other fields of the sheetDef.</summary>
		public SheetDef SheetDefCur;
		///<summary>Ignored. Not available for mobile</summary>
		public bool IsEditMobile;
		public bool IsReadOnly;

		public FormSheetFieldChart() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetFieldChart_Load(object sender,EventArgs e) {
			if(IsReadOnly){
				butOK.Enabled=false;
				butDelete.Enabled=false;
			}
			if(SheetFieldDefCur.FieldValue[0]!='0' && SheetFieldDefCur.FieldValue[0]!='1') {
				SheetFieldDefCur.FieldValue="0;"+SheetFieldDefCur.FieldValue;//For sheets created previously that have no Primary or Permanent chart type
			}
			if(SheetFieldDefCur.FieldValue[0]=='0') {
				radioPermanent.Checked=true;
			}
			else {
				radioPrimary.Checked=true;
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			SheetFieldDefCur=null;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(radioPermanent.Checked) {
				SheetFieldDefCur.FieldValue="0;"+SheetFieldDefCur.FieldValue.Substring(2);
			}
			else {
				//Switching from permanent tooth chart to primary tooth chart.  Primary tooth charts need 4 more tooth values.
				SheetFieldDefCur.FieldValue="1;,,;,,;,,;,,;,,;,,;,,;,,;,,;,,;,,;,,;,,;,,;,,;,,;,,;,,;,,;,,;,,;";
			}
			SheetFieldDefCur.IsNew=false;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	

		

		
	}
}