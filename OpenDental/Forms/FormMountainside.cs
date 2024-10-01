using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary> </summary>
	public partial class FormMountainside:FormODBase {
		/// <summary>This Program link is new.</summary>
		public bool IsNew;
		public Program ProgramCur;
		private List<ProgramProperty> _listProgramProperties;
		//private static Thread thread;

		///<summary></summary>
		public FormMountainside() {
			components=new System.ComponentModel.Container();
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEClinicalWorks_Load(object sender, System.EventArgs e) {
			FillForm();
		}

		private void FillForm(){
			ProgramProperties.RefreshCache();
			_listProgramProperties=ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			textProgName.Text=ProgramCur.ProgName;
			textProgDesc.Text=ProgramCur.ProgDesc;
			checkEnabled.Checked=ProgramCur.Enabled;
			textHL7FolderOut.Text=PrefC.GetString(PrefName.HL7FolderOut);
		}

		private void checkEnabled_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"You will need to restart Open Dental to see the effects.");
		}

		private bool SaveToDb(){
			if(textProgDesc.Text==""){
				MsgBox.Show(this,"Description may not be blank.");
				return false;
			}
			if(textHL7FolderOut.Text=="") {
				MsgBox.Show(this,"HL7 out folder may not be blank.");
				return false;
			}
			ProgramCur.ProgDesc=textProgDesc.Text;
			ProgramCur.Enabled=checkEnabled.Checked;
			Programs.Update(ProgramCur);
			Prefs.UpdateString(PrefName.HL7FolderOut,textHL7FolderOut.Text);
			DataValid.SetInvalid(InvalidType.Programs,InvalidType.Prefs);
			return true;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(!SaveToDb()){
				return;
			}
			DialogResult=DialogResult.OK;
		}

	}
}