using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental.User_Controls {
	public partial class UserControlHostedURL:UserControl {
		public LayoutManagerForms LayoutManager;
		private bool _isExpanded;
		private WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService _signup;
		private const int VERIFYTXT_COL=3;//The verify text column of the options grid can only be checked if texting is enabled, keep track of its index

		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				_isExpanded=value;
				butExpander.Text=_isExpanded ? "-" : "+";
				if(LayoutManager!=null){
					if(_isExpanded){
						Height=LayoutManager.Scale(300);
						LayoutManager.MoveHeight(this,Height);
					}
					else{
						Height=LayoutManager.Scale(25);
						LayoutManager.MoveHeight(this,Height);
					}
				}
			}
		}

		///<summary>Set to true to hide the expander button.</summary>
		public bool DoHideExpandButton {
			set {
				butExpander.Visible=!value;
			}
		}

		private bool IsTextingEnabled {
			get {
			return (!PrefC.HasClinicsEnabled && SmsPhones.IsIntegratedTextingEnabled()) ||  
				(PrefC.HasClinicsEnabled && Clinics.IsTextingEnabled(Signup.ClinicNum));
			}
		}

		public WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService Signup
		{
			get { return _signup; }
			set { _signup=value; }
		}

		public UserControlHostedURL(WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService signup) {
			InitializeComponent();
			IsExpanded=false;
			AddContextMenu(textWebFormToLaunchNewPat);
			AddContextMenu(textWebFormToLaunchExistingPat);
			AddContextMenu(textSchedulingURL);
			Signup=signup;
			FillControl();
		}

		public string GetPrefValue(PrefName prefName) {
			switch(prefName) {
				case PrefName.WebSchedNewPatAllowChildren:
					return FromGridCell(0);
				case PrefName.WebSchedNewPatDoAuthEmail:
					return checkNewPatEmail.Checked.ToString();
				case PrefName.WebSchedNewPatDoAuthText:
					return checkNewPatText.Checked.ToString();
				case PrefName.WebSchedExistingPatDoAuthEmail:
					return checkExistingPatEmail.Checked.ToString();
				case PrefName.WebSchedExistingPatDoAuthText:
					return checkExistingPatText.Checked.ToString();
				case PrefName.WebSchedNewPatWebFormsURL:
					return textWebFormToLaunchNewPat.Text;
				case PrefName.WebSchedExistingPatWebFormsURL:
					return textWebFormToLaunchExistingPat.Text;
				default: return "";
			}
		}

		public long GetClinicNum() {
			return Signup.ClinicNum;
		}

		private void FillControl() {
			labelClinicName.Text=Signup.ClinicNum!=0 ? Clinics.GetDesc(Signup.ClinicNum) : Lan.g(this,"Headquarters");
			labelEnabled.Text=Signup.IsEnabled ? Lan.g(this,"Enabled") : Lan.g(this,"Disabled");
			textSchedulingURL.Text=Signup.HostedUrl;
			FillGrid();
			checkNewPatEmail.Checked=ClinicPrefs.GetBool(PrefName.WebSchedNewPatDoAuthEmail,Signup.ClinicNum);
			checkNewPatText.Checked=IsTextingEnabled?ClinicPrefs.GetBool(PrefName.WebSchedNewPatDoAuthText,Signup.ClinicNum):false;
			checkExistingPatEmail.Checked=ClinicPrefs.GetBool(PrefName.WebSchedExistingPatDoAuthEmail,Signup.ClinicNum);
			checkExistingPatText.Checked=ClinicPrefs.GetBool(PrefName.WebSchedExistingPatDoAuthText,Signup.ClinicNum);
			if(!checkExistingPatEmail.Checked && !checkExistingPatText.Checked) {
				checkExistingPatEmail.Checked=true;
			}
		}

		private void FillGrid() {
			gridOptions.BeginUpdate();
			//Columns
			gridOptions.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Allow Children"),95,HorizontalAlignment.Center);
			gridOptions.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Launch WebForm on New Pat Complete"),250,HorizontalAlignment.Center);
			gridOptions.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Launch WebForm on Existing Pat Complete"),260,HorizontalAlignment.Center);
			gridOptions.ListGridColumns.Add(col);
			//Rows
			gridOptions.ListGridRows.Clear();
			GridRow row=new GridRow();
			row.Cells.Add(ToGridStr(ClinicPrefs.GetBool(PrefName.WebSchedNewPatAllowChildren,Signup.ClinicNum)));
			string urlNewPat="";
			string urlExistingPat="";
			if(Signup.ClinicNum==0) { //HQ always uses pref.
				urlNewPat=PrefC.GetString(PrefName.WebSchedNewPatWebFormsURL);
				urlExistingPat=PrefC.GetString(PrefName.WebSchedExistingPatWebFormsURL);
			}
			else { //Clinic should not default back to HQ version of URL. This is unlike typical ClinicPref behavior.
				ClinicPref prefNewPat=ClinicPrefs.GetPref(PrefName.WebSchedNewPatWebFormsURL,Signup.ClinicNum);
				if(prefNewPat!=null) {
					urlNewPat=prefNewPat.ValueString;
				}
				ClinicPref prefExistingPat=ClinicPrefs.GetPref(PrefName.WebSchedExistingPatWebFormsURL,Signup.ClinicNum);
				if(prefExistingPat!=null) {
					urlExistingPat=prefExistingPat.ValueString;
				}
			}
			row.Cells.Add(ToGridStr(!string.IsNullOrWhiteSpace(urlNewPat)));
			row.Cells.Add(ToGridStr(!string.IsNullOrWhiteSpace(urlExistingPat)));
			gridOptions.ListGridRows.Add(row);
			gridOptions.EndUpdate();
			UpdateNewPatCol(urlNewPat);
			UpdateExistingPatCol(urlExistingPat);
		}

		private string ToGridStr(bool value) {
			return value ? "X" : "";
		}

		private string FromGridCell(int cellIdx) {
			return gridOptions.ListGridRows[0].Cells[cellIdx].Text=="X" ? "1" : "0";
		}

		private static void AddContextMenu(TextBox text) {
			if(text.ContextMenuStrip==null) {
				ContextMenuStrip menu=new ContextMenuStrip();
				ToolStripMenuItem browse = new ToolStripMenuItem("Browse");
				browse.Click+=(sender, e) => {
					if(!string.IsNullOrWhiteSpace(text.Text)) {
						System.Diagnostics.Process.Start(text.Text);
					}
				};
				menu.Items.Add(browse);
				ToolStripMenuItem copy = new ToolStripMenuItem("Copy");
				copy.Click += (sender, e) => text.Copy();
				menu.Items.Add(copy);
				text.ContextMenuStrip = menu;
			}
		}

		private void UpdateNewPatCol(string formURL) {
			textWebFormToLaunchNewPat.Text=formURL;
			gridOptions.Refresh();
		}

		private void UpdateExistingPatCol(string formURL) {
			textWebFormToLaunchExistingPat.Text=formURL;
			gridOptions.Refresh();
		}

		private void butExpander_Click(object sender,EventArgs e) {
			IsExpanded=!IsExpanded;
		}

		private void butEditNewPat_Click(object sender,EventArgs e) {
			EditHelper(true,textWebFormToLaunchNewPat.Text);
		}

		private void butEditExistingPat_Click(object sender,EventArgs e) {
			EditHelper(false,textWebFormToLaunchExistingPat.Text);
		}

		private void EditHelper(bool isNewPat,string text) {
			using FormWebFormSetup formWebFormSetup=new FormWebFormSetup(Signup.ClinicNum,text,true);
			formWebFormSetup.ShowDialog();
			if(formWebFormSetup.DialogResult==DialogResult.OK) {
				if(isNewPat) {
					UpdateNewPatCol(formWebFormSetup.SheetURLs);
				}
				else {
					UpdateExistingPatCol(formWebFormSetup.SheetURLs);
				}
			}
		}

		private void butCopy_Click(object sender,EventArgs e) {
			try {
				ODClipboard.SetClipboard(textSchedulingURL.Text);
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Unable to copy to clipboard."),ex);
			}
		}

		private void gridOptions_CellClick(object sender,ODGridClickEventArgs e) {
			//Cell coordinates are [e.Row][e.Col]
			switch(e.Col) {
				case VERIFYTXT_COL:
					if(!IsTextingEnabled) {
						MsgBox.Show(this,"Texting not enabled"+(PrefC.HasClinicsEnabled?" for this clinic":""));
						return;
					}
					break;
				default: break;
			}
			string cellTextCur=gridOptions.ListGridRows[e.Row].Cells[e.Col].Text;
			string cellTextNew=(cellTextCur=="X" ? "" : "X");
			gridOptions.ListGridRows[e.Row].Cells[e.Col].Text=cellTextNew;
			gridOptions.Refresh();
		}

		private void ClearHelper(bool isNewPat) {
			if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will clear the formed URL and you will have to click Edit to create a new one. " +
				"Continue?","Clear Webform URL"))
			{
				if(isNewPat) {
					UpdateNewPatCol("");
				}
				else {
					UpdateExistingPatCol("");
				}
			}
		}

		private void checkExistingPat_CheckedChanged(object sender,EventArgs e) {
			if(!checkExistingPatEmail.Checked && !checkExistingPatText.Checked) {
				checkExistingPatEmail.Checked=true;
				MsgBox.Show(this,MsgBoxButtons.OKCancel,"At least one Two-Factor option must be selected for Existing Patient. Defaulting to email.");
			}
		}

		private void butClearNewPat_Click(object sender,EventArgs e) {
			ClearHelper(true);
		}

		private void butClearExistingPat_Click(object sender,EventArgs e) {
			ClearHelper(false);
		}
	}
}
