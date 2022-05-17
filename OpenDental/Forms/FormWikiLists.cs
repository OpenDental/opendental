using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWikiLists:FormODBase {

		public FormWikiLists() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWikiLists_Load(object sender,EventArgs e) {
			FillList();
		}

		private void FillList() {
			listWikiLists.Items.Clear();
			listWikiLists.Items.AddList(WikiLists.GetAllLists(),x => x.Substring(9));
		}

		private void listWikiLists_DoubleClick(object sender,EventArgs e) {
			if(listWikiLists.SelectedIndex==-1) {
				return;
			}
			using FormWikiListEdit FormWLE = new FormWikiListEdit();
			FormWLE.WikiListCurName=listWikiLists.Items.GetTextShowingAt(listWikiLists.SelectedIndex);
			FormWLE.ShowDialog();
			FillList();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.WikiListSetup)) {
				return;
			}
			using InputBox inputListName = new InputBox("New List Name");
			inputListName.ShowDialog();
			if(inputListName.DialogResult!=DialogResult.OK) {
				return;
			}
			//Format input as it would be saved in the database--------------------------------------------
			inputListName.textResult.Text=inputListName.textResult.Text.ToLower().Replace(" ","");
			//Validate list name---------------------------------------------------------------------------
			if(DbHelper.isMySQLReservedWord(inputListName.textResult.Text)) {
				//Can become an issue when retrieving column header names.
				MsgBox.Show(this,"List name is a reserved word in MySQL.");
				return;
			}
			if(inputListName.textResult.Text=="") {
				MsgBox.Show(this,"List name cannot be blank.");
				return;
			}
			if(WikiLists.CheckExists(inputListName.textResult.Text)) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"List already exists with that name. Would you like to edit existing list?")) {
					return;
				}
			}
			using FormWikiListEdit FormWLE = new FormWikiListEdit();
			FormWLE.WikiListCurName = inputListName.textResult.Text;
			//FormWLE.IsNew=true;//set within the form.
			FormWLE.ShowDialog();
			FillList();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}