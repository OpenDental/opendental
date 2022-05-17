using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMobileBugEdit:Form {
		public MobileBug MobileBugCur;
		public bool IsNew;
		private List<MobileBugVersion> _listBugVersions=new List<MobileBugVersion>();

		///<summary></summary>
		public FormMobileBugEdit() {
			InitializeComponent();
		}

		private void FormMobileBugEdit_Load(object sender,EventArgs e) {
			if(MobileBugCur==null) {
				MsgBox.Show(this,"An invalid bug was attempted to be loaded.");
				DialogResult=DialogResult.Abort;
				Close();
				return;
			}
			textMobileBugNum.Text=MobileBugCur.MobileBugNum.ToString();
			textCreationDate.Text=MobileBugCur.DateTimeCreated.ToString();
			comboStatus.Text=MobileBugCur.BugStatus.ToString();
			textODVersionsFound.Text=MobileBugCur.ODVersionsFound;
			textODVersionsFixed.Text=MobileBugCur.ODVersionsFixed;
			textDescription.Text=MobileBugCur.Description;
			textSubmitter.Text=Bugs.GetSubmitterName(MobileBugCur.Submitter);
			if(IsNew) {
				checkiOS.Checked=true;
				checkAndroid.Checked=true;
				checkUWP.Checked=true;
			}
			else {
				checkiOS.Checked=MobileBugCur.Platforms.HasFlag(Platforms.iOS);
				checkAndroid.Checked=MobileBugCur.Platforms.HasFlag(Platforms.Android);
				checkUWP.Checked=MobileBugCur.Platforms.HasFlag(Platforms.UWP);
				_listBugVersions=MobileBugVersions.GetVersionsForMobileBugNum(MobileBugCur.MobileBugNum);
				List<AppType> affectedApps=_listBugVersions.Select(x => x.AppType).ToList();
				if(affectedApps.Contains(AppType.eClipboard)) {
					checkEClipboard.Checked=true;
					groupBoxEClipboard.Enabled=true;
					MobileBugVersion eClipboardVersion=_listBugVersions.Find(x => x.AppType==AppType.eClipboard);
					textEClipboardVersionFound.Text=eClipboardVersion.MobileVersionFound;
					textEClipboardVersionFixed.Text=eClipboardVersion.MobileVersionFixed;
				}
				if(affectedApps.Contains(AppType.ODMobile)) {
					checkODMobile.Checked=true;
					groupBoxODMobile.Enabled=true;
					MobileBugVersion odMobileVersion=_listBugVersions.Find(x => x.AppType==AppType.ODMobile);
					textODMobileVersionFound.Text=odMobileVersion.MobileVersionFound;
					textODMobileVersionFixed.Text=odMobileVersion.MobileVersionFixed;
				}
			}
		}

		#region Events
		#region Events Version Buttons
		private void butODVersionCopyDown_Click(object sender,EventArgs e) {
			textODVersionsFixed.Text=textODVersionsFound.Text;
		}

		private void butLast1found_Click(object sender,EventArgs e) {
			textODVersionsFound.Text=VersionReleases.GetLastReleases(1);
		}

		private void butLast2found_Click(object sender,EventArgs e) {
			textODVersionsFound.Text=VersionReleases.GetLastReleases(2);
		}

		private void butLast3found_Click(object sender,EventArgs e) {
			textODVersionsFound.Text=VersionReleases.GetLastReleases(3);
		}

		private void butLast1_Click(object sender,EventArgs e) {
			textODVersionsFixed.Text=VersionReleases.GetLastReleases(1);
		}

		private void butLast2_Click(object sender,EventArgs e) {
			textODVersionsFixed.Text=VersionReleases.GetLastReleases(2);
		}

		private void butLast3_Click(object sender,EventArgs e) {
			textODVersionsFixed.Text=VersionReleases.GetLastReleases(3);
		}

		private void butEClipboardLastVersion_Click(object sender,EventArgs e) {
			MobileRelease lastRelease=MobileReleases.GetLastReleaseForApp(AppType.eClipboard);
			textEClipboardVersionFound.Text=lastRelease.MajorNum+"."+lastRelease.MinorNum+"."+lastRelease.BuildNum;
		}

		private void butEClipboardNextVersion_Click(object sender,EventArgs e) {
			textEClipboardVersionFixed.Text=MobileReleases.GetNextVersionForApp(AppType.eClipboard);
		}

		private void butODMobileLastVersion_Click(object sender,EventArgs e) {
			MobileRelease lastRelease=MobileReleases.GetLastReleaseForApp(AppType.ODMobile);
			textODMobileVersionFound.Text=lastRelease.MajorNum+"."+lastRelease.MinorNum+"."+lastRelease.BuildNum;
		}

		private void butODMobileNextVersion_Click(object sender,EventArgs e) {
			textODMobileVersionFixed.Text=MobileReleases.GetNextVersionForApp(AppType.ODMobile);
		}
		#endregion Events Version Buttons

		private void checkEClipboard_CheckedChanged(object sender,EventArgs e) {
			groupBoxEClipboard.Enabled=checkEClipboard.Checked;
			if(!checkEClipboard.Checked) {
				textEClipboardVersionFound.Text="";
				textEClipboardVersionFixed.Text="";
			}
		}

		private void checkODMobile_CheckedChanged(object sender,EventArgs e) {
			groupBoxODMobile.Enabled=checkODMobile.Checked;
			if(!checkODMobile.Checked) {
				textODMobileVersionFound.Text="";
				textODMobileVersionFixed.Text="";
			}
		}
		#endregion Events

		private bool ValidateEClipboard() {
			if(checkEClipboard.Checked==false || string.IsNullOrEmpty(textEClipboardVersionFixed.Text)) {
				return true;
			}
			if(!Version.TryParse(textEClipboardVersionFixed.Text,out Version versionFixed) || !Version.TryParse(textEClipboardVersionFound.Text,out Version versionFound)) {
				return false;
			}
			return versionFixed>=versionFound;
		}

		private bool ValidateODMobile() {
			if(checkODMobile.Checked==false || string.IsNullOrEmpty(textODMobileVersionFixed.Text)) {
				return true;
			}
			if(!Version.TryParse(textODMobileVersionFixed.Text,out Version versionFixed) || !Version.TryParse(textODMobileVersionFound.Text,out Version versionFound)) {
				return false;
			}
			return versionFixed>=versionFound;
		}

		private void SaveToDb() {
			MobileBugCur.ODVersionsFound=textODVersionsFound.Text;
			MobileBugCur.ODVersionsFixed=textODVersionsFixed.Text;
			MobileBugCur.Description=textDescription.Text;
			Platforms savePlatforms;
			savePlatforms=checkiOS.Checked ? Platforms.iOS : 0;
			savePlatforms|=checkAndroid.Checked ? Platforms.Android : 0;
			savePlatforms|=checkUWP.Checked ? Platforms.UWP : 0;
			MobileBugCur.Platforms=savePlatforms;
			if(IsNew) {
				MobileBugs.Insert(MobileBugCur);
			}
			else {
				MobileBugs.Update(MobileBugCur);
			}
			MobileBugVersion bugVersion;
			bugVersion=_listBugVersions.FirstOrDefault(x => x.AppType==AppType.eClipboard);
			if(checkEClipboard.Checked) {
				if(bugVersion==null) {
					bugVersion=new MobileBugVersion();
				}
				bugVersion.MobileBugNum=MobileBugCur.MobileBugNum;
				bugVersion.AppType=AppType.eClipboard;
				bugVersion.BugStatus=MobileBugCur.BugStatus;
				bugVersion.MobileVersionFound=textEClipboardVersionFound.Text;
				bugVersion.MobileVersionFixed=textEClipboardVersionFixed.Text;
				if(bugVersion.MobileBugVersionNum!=0) {
					MobileBugVersions.Update(bugVersion);
				}
				else {
					MobileBugVersions.Insert(bugVersion);
				}
			}
			else {
				if(bugVersion!=null) {
					MobileBugVersions.Delete(bugVersion.MobileBugVersionNum);
				}
			}
			bugVersion=_listBugVersions.FirstOrDefault(x => x.AppType==AppType.ODMobile);
			if(checkODMobile.Checked) {
				if(bugVersion==null) {
					bugVersion=new MobileBugVersion();
				}
				bugVersion.MobileBugNum=MobileBugCur.MobileBugNum;
				bugVersion.AppType=AppType.ODMobile;
				bugVersion.BugStatus=MobileBugCur.BugStatus;
				bugVersion.MobileVersionFound=textODMobileVersionFound.Text;
				bugVersion.MobileVersionFixed=textODMobileVersionFixed.Text;
				if(bugVersion.MobileBugVersionNum!=0) {
					MobileBugVersions.Update(bugVersion);
				}
				else {
					MobileBugVersions.Insert(bugVersion);
				}
			}
			else {
				if(bugVersion!=null) {
					MobileBugVersions.Delete(bugVersion.MobileBugVersionNum);
				}
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				Close();
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This will also delete all attachments to jobs and bug submissions for this bug. Do you wish to continue?")) {
				return;
			}
			List<MobileBugVersion> listMobileBugVersions=MobileBugVersions.GetVersionsForMobileBugNum(MobileBugCur.MobileBugNum);
			for(int i = listMobileBugVersions.Count-1;i>=0;i--) {
				MobileBugVersions.Delete(listMobileBugVersions[i].MobileBugVersionNum);
			}
			MobileBugs.Delete(MobileBugCur.MobileBugNum);
			MobileBugCur=null;
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(MobileBugCur.Submitter==0) {
				MessageBox.Show("A valid submitter wasn't picked. Make sure the computer being used is associated to a buguser.");
				return;
			}
			if(!checkEClipboard.Checked && !checkODMobile.Checked && string.IsNullOrEmpty(textODVersionsFound.Text)) {
				MsgBox.Show("You must have at least one version found in OD, eClipboard or ODMobile.");
				return;
			}
			if(!Bugs.VersionsAreValid(textODVersionsFixed.Text)) {
				MsgBox.Show("Please fix your version format. Must be like '18.4.8.0;19.1.22.0;19.2.3.0'");
				return;
			}
			if(!ValidateEClipboard()) {
				MessageBox.Show("The eClipboard version fixed must be higher than or equal to the version found.");
				return;
			}
			if(!ValidateODMobile()) {
				MessageBox.Show("The ODMobile version fixed must be higher than or equal to the version found.");
				return;
			}
			bool odFixed=(string.IsNullOrEmpty(textODVersionsFound.Text) && string.IsNullOrEmpty(textODVersionsFixed.Text))
				|| (!string.IsNullOrEmpty(textODVersionsFound.Text) && !string.IsNullOrEmpty(textODVersionsFixed.Text));
			bool eClipboardFixed=!checkEClipboard.Checked || !string.IsNullOrEmpty(textEClipboardVersionFixed.Text);
			bool odMobileFixed=!checkODMobile.Checked || !string.IsNullOrEmpty(textODMobileVersionFixed.Text);
			if(odFixed && eClipboardFixed && odMobileFixed) {
				MobileBugCur.BugStatus=MobileBugStatus.Fixed;
			}
			else {
				MobileBugCur.BugStatus=MobileBugStatus.Found;
			}
			SaveToDb();
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}
	}
}
