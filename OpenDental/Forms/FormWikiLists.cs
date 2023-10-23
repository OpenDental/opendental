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
			using FormWikiListEdit formWikiListEdit = new FormWikiListEdit();
			formWikiListEdit.WikiListCurName=listWikiLists.Items.GetTextShowingAt(listWikiLists.SelectedIndex);
			formWikiListEdit.ShowDialog();
			FillList();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.WikiListSetup)) {
				return;
			}
			using InputBox inputBox = new InputBox("New List Name");
			inputBox.ShowDialog();
			if(inputBox.DialogResult!=DialogResult.OK) {
				return;
			}
			//Format input as it would be saved in the database--------------------------------------------
			inputBox.textResult.Text=inputBox.textResult.Text.ToLower().Replace(" ","");
			//Validate list name---------------------------------------------------------------------------
			if(DbHelper.isMySQLReservedWord(inputBox.textResult.Text)) {
				//Can become an issue when retrieving column header names.
				MsgBox.Show(this,"List name is a reserved word in MySQL.");
				return;
			}
			if(inputBox.textResult.Text=="") {
				MsgBox.Show(this,"List name cannot be blank.");
				return;
			}
			if(WikiLists.CheckExists(inputBox.textResult.Text)) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"List already exists with that name. Would you like to edit existing list?")) {
					return;
				}
			}
			using FormWikiListEdit formWikiListEdit = new FormWikiListEdit();
			formWikiListEdit.WikiListCurName = inputBox.textResult.Text;
			//FormWLE.IsNew=true;//set within the form.
			formWikiListEdit.ShowDialog();
			FillList();
		}
	}
}