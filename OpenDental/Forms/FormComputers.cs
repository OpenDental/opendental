using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public partial class FormComputers : FormODBase {
		//private Programs Programs=new Programs();
		private bool changed;

		///<summary></summary>
		public FormComputers(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormComputers_Load(object sender, System.EventArgs e) {
			FillList();
			if(!Security.IsAuthorized(Permissions.GraphicsEdit,true)) {
				butSetSimpleGraphics.Enabled=false;
			}
		}

		private void FillList(){
			Computers.RefreshCache();
			listComputer.Items.Clear();
			//Database Server----------------------------------------------------------		
			List<string> serviceList=Computers.GetServiceInfo();
			textName.Text=MiscData.GetODServer();//server name
			textService.Text=(serviceList[0].ToString());//service name
			textVersion.Text=(serviceList[3].ToString());//service version
			textServComment.Text=(serviceList[1].ToString());//service comment
			//workstation--------------------------------------------------------------
			textCurComp.Text=ODEnvironment.MachineName.ToUpper();//current computer name
			listComputer.Items.AddList(Computers.GetDeepCopy(),x => x.CompName); //+" ("+x.PrinterName+")";
		}

		private void listComputer_MouseDoubleClick(object sender,MouseEventArgs e) {
			if(!Security.IsAuthorized(Permissions.GraphicsEdit)) {
				return;
			}
			using FormGraphics FormG=new FormGraphics();
			FormG.ComputerPrefCur=ComputerPrefs.GetForComputer(listComputer.GetSelected<Computer>().CompName);
			FormG.ShowDialog();
		}

		///<summary>Set graphics for selected computer to simple.  Makes audit log entry.</summary>
		private void butSetSimpleGraphics_Click(object sender,EventArgs e) {
			if(listComputer.SelectedIndex==-1) {
				MsgBox.Show(this,"You must select a computer name first.");
				return;
			}
			ComputerPrefs.SetToSimpleGraphics(listComputer.GetSelected<Computer>().CompName);
			MsgBox.Show(this,"Done.");
			SecurityLogs.MakeLogEntry(Permissions.GraphicsEdit,0,"Set the graphics for computer "+listComputer.GetSelected<Computer>().CompName+" to simple");
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(listComputer.SelectedIndex==-1) {
				return;
			}
			Computers.Delete(listComputer.GetSelected<Computer>());
			changed=true;
			FillList();
		}

		/*private void listProgram_DoubleClick(object sender, System.EventArgs e) {
			if(listProgram.SelectedIndex==-1)
				return;
			Programs.Cur=Programs.List[listProgram.SelectedIndex];
			FormProgramLinkEdit FormPE=new FormProgramLinkEdit();
			FormPE.ShowDialog();
			FillList();
		}*/

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormComputers_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(changed){
				DataValid.SetInvalid(InvalidType.Computers);
			}
		}


	}
}
