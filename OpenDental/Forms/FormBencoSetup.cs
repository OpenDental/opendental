using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormBencoSetup:FormODBase {
		private Program _program;
		private List<ToolButItem> _listToolButItems;

		public FormBencoSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			_program=Programs.GetCur(ProgramName.BencoPracticeManagement);
			_listToolButItems=ToolButItems.GetForProgram(_program.ProgramNum); //Initially only set up for Main Toolbar
		}

		private void FormBencoSetup_Load(object sender,EventArgs e) {
			checkEnable.Checked=_program.Enabled;
			textProgDesc.Text=_program.ProgDesc;
			textPath.Text=_program.Path;
			textButText.Text=_listToolButItems.FirstOrDefault()?.ButtonText??"Benco";
			FillToolBars();
		}

		private void FillToolBars() {
			listToolBars.Items.Clear();
			listToolBars.Items.AddEnums<ToolBarsAvail>();
			for(int i=0;i<listToolBars.Items.Count;i++) {
				if(_listToolButItems.Any(x => x.ToolBar==(ToolBarsAvail)listToolBars.Items.GetObjectAt(i))) {
					listToolBars.SetSelected(i);
				}
			}
		}

		/// <summary>Updates some preferences in Open Dental according to the enabled state of the Benco bridge</summary>
		private void UpdateBencoSettings() {
			bool hasPrefChanged=false;
			string odTitle="Open Dental";
			string odSoftware="Open Dental Software";
			string bencoTitle="Benco Practice Management powered by Open Dental";
			string bencoSoftware="Benco Practice Management";
			if(_program.Enabled) {
				if(PrefC.GetString(PrefName.MainWindowTitle)==odTitle) {
					hasPrefChanged|=Prefs.UpdateString(PrefName.MainWindowTitle,bencoTitle);
				}
				if(PrefC.GetString(PrefName.SoftwareName)!=bencoSoftware) {
					hasPrefChanged|=Prefs.UpdateString(PrefName.SoftwareName,bencoSoftware);
				}
			}
			else {
				if(PrefC.GetString(PrefName.MainWindowTitle)==bencoTitle) {
					hasPrefChanged|=Prefs.UpdateString(PrefName.MainWindowTitle,odTitle);
				}
				if(PrefC.GetString(PrefName.SoftwareName)!=odSoftware) {
					hasPrefChanged|=Prefs.UpdateString(PrefName.SoftwareName,odSoftware);
				}
			}
			if(hasPrefChanged) {
				Signalods.SetInvalid(InvalidType.Prefs);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			//Program
			_program.Enabled=checkEnable.Checked;
			_program.ProgDesc=textProgDesc.Text;
			_program.Path=textPath.Text;
			Programs.Update(_program);
			//Toolbar button
			ToolButItems.DeleteAllForProgram(_program.ProgramNum);
			List<ToolBarsAvail> listToolBarsAvails=listToolBars.GetListSelected<ToolBarsAvail>();
			for(int i=0;i<listToolBarsAvails.Count;++i) {
				ToolButItem newBut=new ToolButItem();
				newBut.ProgramNum=_program.ProgramNum;
				newBut.ToolBar=listToolBarsAvails[i];
				newBut.ButtonText=textButText.Text;
				ToolButItems.Insert(newBut);
			}
			//Update settings as necessary
			UpdateBencoSettings();
			MsgBox.Show(this,"You will need to restart Open Dental for these changes to take effect.");
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
