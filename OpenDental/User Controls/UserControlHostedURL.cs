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
		private WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService _signup;

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
			Font=LayoutManagerForms.FontInitial;
			AddContextMenu(textWebFormToLaunchNewPat);
			AddContextMenu(textWebFormToLaunchExistingPat);
			AddContextMenu(textSchedulingURL);
			Signup=signup;
			FillControl();
		}

		public string GetPrefValue(PrefName prefName) {
			switch(prefName) {
				case PrefName.WebSchedNewPatAllowChildren:
					return checkAllowChildren.Checked.ToString();
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
			textSchedulingURL.Text=Signup.HostedUrl;
			string strWebSchedNewPatWebFormsURL="";
			string strWebSchedExistingPatWebFormsURL="";
			if(Signup.ClinicNum==0) { //HQ always uses pref.
				strWebSchedNewPatWebFormsURL=PrefC.GetString(PrefName.WebSchedNewPatWebFormsURL);
				strWebSchedExistingPatWebFormsURL=PrefC.GetString(PrefName.WebSchedExistingPatWebFormsURL);
			}
			else { //Clinic should not default back to HQ version of URL. This is unlike typical ClinicPref behavior.
				ClinicPref prefNewPat=ClinicPrefs.GetPref(PrefName.WebSchedNewPatWebFormsURL,Signup.ClinicNum);
				if(prefNewPat!=null) {
					strWebSchedNewPatWebFormsURL=prefNewPat.ValueString;
				}
				ClinicPref prefExistingPat=ClinicPrefs.GetPref(PrefName.WebSchedExistingPatWebFormsURL,Signup.ClinicNum);
				if(prefExistingPat!=null) {
					strWebSchedExistingPatWebFormsURL=prefExistingPat.ValueString;
				}
			}
			textWebFormToLaunchNewPat.Text=strWebSchedNewPatWebFormsURL;
			textWebFormToLaunchExistingPat.Text=strWebSchedExistingPatWebFormsURL;
			checkAllowChildren.Checked=ClinicPrefs.GetBool(PrefName.WebSchedNewPatAllowChildren,Signup.ClinicNum);
			checkNewPatEmail.Checked=ClinicPrefs.GetBool(PrefName.WebSchedNewPatDoAuthEmail,Signup.ClinicNum);
			checkNewPatText.Checked=IsTextingEnabled?ClinicPrefs.GetBool(PrefName.WebSchedNewPatDoAuthText,Signup.ClinicNum):false;
			checkExistingPatEmail.Checked=ClinicPrefs.GetBool(PrefName.WebSchedExistingPatDoAuthEmail,Signup.ClinicNum);
			checkExistingPatText.Checked=ClinicPrefs.GetBool(PrefName.WebSchedExistingPatDoAuthText,Signup.ClinicNum);
			if(!checkExistingPatEmail.Checked && !checkExistingPatText.Checked) {
				checkExistingPatEmail.Checked=true;
			}
		}

		private static void AddContextMenu(System.Windows.Forms.TextBox text) {
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
					textWebFormToLaunchNewPat.Text=formWebFormSetup.SheetURLs;
				}
				else {
					textWebFormToLaunchExistingPat.Text=formWebFormSetup.SheetURLs;
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

		private void ClearHelper(bool isNewPat) {
			if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will clear the formed URL and you will have to click Edit to create a new one. " +
				"Continue?","Clear Webform URL"))
			{
				if(isNewPat) {
					textWebFormToLaunchNewPat.Text="";
				}
				else {
					textWebFormToLaunchExistingPat.Text="";
				}
			}
		}

		private void CheckHelper() {
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

		private void checkExistingPatEmail_CheckedChanged(object sender,EventArgs e) {
			CheckHelper();
		}

		private void checkExistingPatText_CheckedChanged(object sender,EventArgs e) {
			CheckHelper();
		}
	}
}
