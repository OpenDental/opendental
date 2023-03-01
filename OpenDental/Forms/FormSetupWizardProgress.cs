using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using OpenDental.User_Controls.SetupWizard;
using System.Windows.Documents;
using CodeBase;

namespace OpenDental {
	public partial class FormSetupWizardProgress:FormODBase {
		private List<OpenDental.SetupWizard.SetupWizClass> _listSetupWizClasses;
		///<summary>The current setup control that is being viewed.  Used in conjunction with _listSetupClasses.</summary>
		private int _indexSetupClasses=0;
		private bool _isSetupAll;

		public FormSetupWizardProgress(List<SetupWizard.SetupWizClass> listSetupWizClasses, bool isSetupAll) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_isSetupAll=isSetupAll;
			_listSetupWizClasses=listSetupWizClasses;
		}

		private void FormSetupWizardProgress_Load(object sender,EventArgs e) {
			for(int i=0;i<_listSetupWizClasses.Count;i++) {
				_listSetupWizClasses[i].SetupControl.Dock=DockStyle.Fill;
				LayoutManager.AddUnscaled(_listSetupWizClasses[i].SetupControl,splitContainer2.Panel2);
			}
			LayoutManager.LayoutControlBoundsAndFonts(splitContainer2.Panel2);
			SetControlCurrent();
			butNext.Focus();
		}

		private void FormSetupWizardProgress_SizeChanged(object sender,EventArgs e) {
			splitContainer1.SplitterDistance=splitContainer1.Height-LayoutManager.Scale(29);
		}

		private void butBack_Click(object sender,EventArgs e) {
			_indexSetupClasses--;
			SetControlCurrent();
		}

		private void butNext_Click(object sender,EventArgs e) {
			if(!ControlValidated()) {
				return;
			}
			if(!_listSetupWizClasses[_indexSetupClasses].SetupControl.IsDone) {
				string strMsg=Lan.g("FormSetupWizard","You have not finished setting this section up yet.") 
					+"\r\n"+_listSetupWizClasses[_indexSetupClasses].SetupControl.StrIncomplete;
				strMsg+="\r\n"+Lan.g("FormSetupWizard","Click 'Skip' if you do not wish to finish setting this section up at this time.");
				MessageBox.Show(strMsg);
				return;
			}
			//Call the Control Done method for the setup class.
			ControlDone();
			_indexSetupClasses++;
			if(_listSetupWizClasses.Count-1<_indexSetupClasses) {
				MsgBox.Show("FormSetupWizard","You have finished setup.");
				Close();
				return;
			}
			SetControlCurrent();
		}

		///<summary>Any validation should be done here.</summary>
		private bool ControlValidated() {
			return _listSetupWizClasses[_indexSetupClasses].SetupControl.OnControlValidated?.Invoke(this,new EventArgs())??true;
		}

		///<summary>Any conditional relational setup should be done here. 
		///Eg, clinic setup should be added if the user the user is setting up "All" and they checked "Clinics" when setting up Basic Features.</summary>
		private void ControlDone() {
			_listSetupWizClasses[_indexSetupClasses].SetupControl.OnControlDone?.Invoke(this,new EventArgs());
			if(!_isSetupAll) {
				return;
			}
			if(!Security.IsAuthorized(Permissions.ClinicEdit,true)) {
				return;
			}
			#region Clinic Show Feature
			if(_listSetupWizClasses[_indexSetupClasses].GetType()==typeof(SetupWizard.FeatureSetup)) {
				return;
			}
			SetupWizard.ClinicSetup clinicSetup=new SetupWizard.ClinicSetup();
			//if Clinics got enabled but there is no clinic setup item, add it.
			if(PrefC.HasClinicsEnabled && _listSetupWizClasses.Where(x => x.Name==clinicSetup.Name).Count()==0) {
				int endCat=_indexSetupClasses;
				for(int i=_indexSetupClasses;i<_listSetupWizClasses.Count;i++) {
					if(_listSetupWizClasses[i].GetType()==typeof(SetupWizard.ProvSetup)) {
						endCat+=2;
						break;
					}
					endCat++;
				}
				_listSetupWizClasses.Insert(endCat++,new SetupWizard.SetupIntro(clinicSetup.Name,clinicSetup.GetDescript));
				_listSetupWizClasses.Insert(endCat++,clinicSetup);
				_listSetupWizClasses.Insert(endCat,new SetupWizard.SetupComplete(clinicSetup.Name));
			}
			//otherwise, if clinics got disabled and there is a clinic setup item, remove it.
			else if(!PrefC.HasClinicsEnabled && _listSetupWizClasses.Where(x => x.Name==clinicSetup.Name).Count()!=0) {
				_listSetupWizClasses.RemoveAll(x => x.Name==clinicSetup.Name);
			}
			#endregion
		}

		private void butSkip_Click(object sender,EventArgs e) {
			//find the next Complete, then add one.
			for(int i=_indexSetupClasses;i<_listSetupWizClasses.Count;i++) {
				if(_listSetupWizClasses[i].GetType()==typeof(SetupWizard.SetupComplete)) {
					_indexSetupClasses++;
					break;
				}
				_indexSetupClasses++;
			}
			if(_listSetupWizClasses.Count-1<_indexSetupClasses) {
				MsgBox.Show("FormSetupWizard","You have finished setup.");
				Close();
				return;
			}
			SetControlCurrent();
		}

		private void SetControlCurrent() {
			if(_listSetupWizClasses.IsNullOrEmpty()) {
				return;
			}
			int indexSetupClasses=Math.Max(0,_indexSetupClasses);
			indexSetupClasses=Math.Min(indexSetupClasses,_listSetupWizClasses.Count);
			_listSetupWizClasses[indexSetupClasses].SetupControl.BringToFront();
			if(_indexSetupClasses==0) {
				butBack.Enabled=false;
			}
			else {
				butBack.Enabled=true;
			}
			labelTitle.Text=Lan.g("FormSetupWizard",_listSetupWizClasses[indexSetupClasses].Name+" Setup");
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}