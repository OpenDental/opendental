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
		private List<Def> _listDefsApptConfirmed;

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
			_listDefsApptConfirmed=Defs.GetDefsForCategory(DefCat.ApptConfirmed,isShort:false);
			comboStatusEmailedConfirm.Items.AddDefs(_listDefsApptConfirmed);
			comboStatusEmailedConfirm.SetSelectedDefNum(PrefC.GetLong(PrefName.ConfirmStatusEmailed));
			comboStatusTextMessagedConfirm.Items.AddDefs(_listDefsApptConfirmed);
			comboStatusTextMessagedConfirm.SetSelectedDefNum(PrefC.GetLong(PrefName.ConfirmStatusTextMessaged));
			checkGroupFamilies.Checked=PrefC.GetBool(PrefName.ConfirmGroupByFamily);
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableConfirmMsgs","Mode"),61);
			gridMain.Columns.Add(col);
			col=new GridColumn("",300);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmMsgs","Message"),500);
			gridMain.Columns.Add(col);
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
			using FormRecallMessageEdit formRecallMessageEdit=new FormRecallMessageEdit(prefName);
			formRecallMessageEdit.MessageVal=PrefC.GetString(prefName);
			formRecallMessageEdit.ShowDialog();
			if(formRecallMessageEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			Prefs.UpdateString(prefName,formRecallMessageEdit.MessageVal);
			//Prefs.RefreshCache();//above line handles it.
			FillGrid();
		}

		#endregion Confirmations
		//===============================================================================================


		private void butSetup_Click(object sender,EventArgs e) {
			using FormEServicesAutoMsging formEServicesAutoMsging=new FormEServicesAutoMsging();
			formEServicesAutoMsging.ShowDialog();
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			Prefs.UpdateLong(PrefName.ConfirmStatusEmailed,comboStatusEmailedConfirm.GetSelectedDefNum());
			Prefs.UpdateLong(PrefName.ConfirmStatusTextMessaged,comboStatusTextMessagedConfirm.GetSelectedDefNum());
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