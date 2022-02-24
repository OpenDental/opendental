using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormConfirmationSetup:FormODBase {
		private List<Def> _listApptConfirmedDefs;

		public FormConfirmationSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}


		public void FormConfirmationSetup_Load(object sender,System.EventArgs e) {
			FillTabManualConfirmation();
			Plugins.HookAddCode(this,"FormConfirmationSetup.Load_end");
		}

		//===============================================================================================
		#region Confirmations

		///<summary>Called on load to initially load confirmation with values from the database.  Calls FillGrid at the end.</summary>
		private void FillTabManualConfirmation() {
			_listApptConfirmedDefs=Defs.GetDefsForCategory(DefCat.ApptConfirmed,true);
			for(int i=0;i<_listApptConfirmedDefs.Count;i++) {
				comboStatusEmailedConfirm.Items.Add(_listApptConfirmedDefs[i].ItemName);
				if(_listApptConfirmedDefs[i].DefNum==PrefC.GetLong(PrefName.ConfirmStatusEmailed)) {
					comboStatusEmailedConfirm.SelectedIndex=i;
				}
			}
			for(int i=0;i<_listApptConfirmedDefs.Count;i++) {
				comboStatusTextMessagedConfirm.Items.Add(_listApptConfirmedDefs[i].ItemName);
				if(_listApptConfirmedDefs[i].DefNum==PrefC.GetLong(PrefName.ConfirmStatusTextMessaged)) {
					comboStatusTextMessagedConfirm.SelectedIndex=i;
				}
			}
			checkGroupFamilies.Checked=PrefC.GetBool(PrefName.ConfirmGroupByFamily);
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableConfirmMsgs","Mode"),61);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("",300);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmMsgs","Message"),500);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			#region Confirmation
			//Confirmation---------------------------------------------------------------------------------------------
			row=new GridRow();
			row.Cells.Add(Lan.g(this,"Postcard"));
			row.Cells.Add(Lan.g(this,"Confirmation message. Available variables: [NameF], [date], [time]."));
			row.Cells.Add(PrefC.GetString(PrefName.ConfirmPostcardMessage));
			row.Tag=PrefName.ConfirmPostcardMessage;
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add(Lan.g(this,"Postcard"));
			row.Cells.Add(Lan.g(this,"For multiple patients in one family. Available variables: [FamilyApptList]."));
			row.Cells.Add(PrefC.GetString(PrefName.ConfirmPostcardFamMessage));
			row.Tag=PrefName.ConfirmPostcardFamMessage;
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add(Lan.g(this,"E-mail"));
			row.Cells.Add(Lan.g(this,"Confirmation subject line."));
			row.Cells.Add(PrefC.GetString(PrefName.ConfirmEmailSubject));
			row.Tag=PrefName.ConfirmEmailSubject;
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add(Lan.g(this,"E-mail"));
			row.Cells.Add(Lan.g(this,"Confirmation message. Available variables: [NameF], [date], [time]."));
			row.Cells.Add(PrefC.GetString(PrefName.ConfirmEmailMessage));
			row.Tag=PrefName.ConfirmEmailMessage;
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add(Lan.g(this,"E-Mail"));
			row.Cells.Add(Lan.g(this,"For multiple patients in one family. Available variables: [FamilyApptList]."));
			row.Cells.Add(PrefC.GetString(PrefName.ConfirmEmailFamMessage));
			row.Tag=PrefName.ConfirmEmailFamMessage;
			gridMain.ListGridRows.Add(row);
			#endregion
			#region Text Messaging
			//Text Messaging----------------------------------------------------------------------------------------------
			row=new GridRow();
			row.Cells.Add(Lan.g(this,"Text"));
			row.Cells.Add(Lan.g(this,"Confirmation message. Available variables: [NameF], [date], [time]."));
			row.Cells.Add(PrefC.GetString(PrefName.ConfirmTextMessage));
			row.Tag=PrefName.ConfirmTextMessage;
			gridMain.ListGridRows.Add(row);
			//
			row=new GridRow();
			row.Cells.Add(Lan.g(this,"Text"));
			row.Cells.Add(Lan.g(this,"For multiple patients in one family. Available variables: [FamilyApptList]."));
			row.Cells.Add(PrefC.GetString(PrefName.ConfirmTextFamMessage));
			row.Tag=PrefName.ConfirmTextFamMessage;
			gridMain.ListGridRows.Add(row);
			#endregion
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			PrefName prefName=(PrefName)gridMain.ListGridRows[e.Row].Tag;
			using FormRecallMessageEdit FormR=new FormRecallMessageEdit(prefName);
			FormR.MessageVal=PrefC.GetString(prefName);
			FormR.ShowDialog();
			if(FormR.DialogResult!=DialogResult.OK) {
				return;
			}
			Prefs.UpdateString(prefName,FormR.MessageVal);
			//Prefs.RefreshCache();//above line handles it.
			FillGrid();
		}

		#endregion Confirmations
		//===============================================================================================


		private void butSetup_Click(object sender,EventArgs e) {
			using FormEServicesAutoMsging formESECR=new FormEServicesAutoMsging();
			formESECR.ShowDialog();
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(comboStatusEmailedConfirm.SelectedIndex==-1) {
				Prefs.UpdateLong(PrefName.ConfirmStatusEmailed,0);
			}
			else {
				Prefs.UpdateLong(PrefName.ConfirmStatusEmailed,_listApptConfirmedDefs[comboStatusEmailedConfirm.SelectedIndex].DefNum);
			}
			if(comboStatusTextMessagedConfirm.SelectedIndex==-1) {
				Prefs.UpdateLong(PrefName.ConfirmStatusTextMessaged,0);
			}
			else {
				Prefs.UpdateLong(PrefName.ConfirmStatusTextMessaged,_listApptConfirmedDefs[comboStatusTextMessagedConfirm.SelectedIndex].DefNum);
			}
			Prefs.UpdateBool(PrefName.ConfirmGroupByFamily,checkGroupFamilies.Checked);
			//If we want to take the time to check every Update and see if something changed 
			//then we could move this to a FormClosing event later.
			DataValid.SetInvalid(InvalidType.Prefs);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}

}