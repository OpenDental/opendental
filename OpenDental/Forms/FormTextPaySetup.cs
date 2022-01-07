using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormTextPaySetup:FormODBase {
		///<summary>Helper to manager prefs relating to textPaymentLink and getting them to/from the db.</summary>
		private ClinicPrefHelper _clinicPrefHelper=new ClinicPrefHelper(
			PrefName.TextPaymentLinkAppointmentBalance,
			PrefName.TextPaymentLinkAccountBalance);
		private string templateOld;
		private long clinicNumOld;
		private PrefName prefNameOld;

		private long _selectedClinicNum {
			get {
				return comboBoxClinicPicker.GetSelectedClinic()==null ? 0 : comboBoxClinicPicker.GetSelectedClinic().ClinicNum;
			}
		}	

		public FormTextPaySetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTextPaySetup_Load(object sender,EventArgs e) {
			_clinicPrefHelper.SyncAllPrefs();
			comboBoxClinicPicker.IncludeUnassigned=true;
			comboBoxClinicPicker.HqDescription="Default";
			comboBoxTemplates.Items.Add("Appointment Balance",PrefName.TextPaymentLinkAppointmentBalance);
			comboBoxTemplates.Items.Add("Patient Balance",PrefName.TextPaymentLinkAccountBalance);
			comboBoxTemplates.SetSelected(0);
			textMessageTemplate.Text=_clinicPrefHelper.GetStringVal(comboBoxTemplates.GetSelected<PrefName>(),_selectedClinicNum);
			templateOld=textMessageTemplate.Text;
			prefNameOld=comboBoxTemplates.GetSelected<PrefName>();
			clinicNumOld=_selectedClinicNum;
		}

		private void saveTemplate(PrefName prefName,long clinicNum,string template) {
			if(clinicNum==0) {
				Prefs.UpdateString(prefName,template);
				Prefs.RefreshCache();
				_clinicPrefHelper=new ClinicPrefHelper(
					PrefName.TextPaymentLinkAppointmentBalance,
					PrefName.TextPaymentLinkAccountBalance);
			}
			else {
				_clinicPrefHelper.ValChangedByUser(prefNameOld,clinicNumOld,textMessageTemplate.Text); //Save the template
			}
			_clinicPrefHelper.SyncAllPrefs();
		}

		private void CheckUIChanges() {
			if((comboBoxTemplates.GetSelected<PrefName>()!=prefNameOld || _selectedClinicNum!=clinicNumOld)
				&& (templateOld!=textMessageTemplate.Text || checkUseDefaults.Checked==_clinicPrefHelper.ClinicHasClinicPref(prefNameOld,clinicNumOld))
				&& MsgBox.Show(MsgBoxButtons.YesNo,"Changes made have not been saved, would you like to save them?")) 
			{
				if(checkUseDefaults.Checked==true) {
					_clinicPrefHelper.DeleteClinicPref(prefNameOld,clinicNumOld);
				}
				else {
					saveTemplate(prefNameOld,clinicNumOld,textMessageTemplate.Text);
				}
			}
			//Handles checkbox visibility when changing to the 'Default' option.
			if(_selectedClinicNum!=0) {
				checkUseDefaults.Visible=true;
				checkUseDefaults.Checked=!_clinicPrefHelper.ClinicHasClinicPref(comboBoxTemplates.GetSelected<PrefName>(),_selectedClinicNum);
			}
			else {
				checkUseDefaults.Visible=false;
				checkUseDefaults.Checked=false;
			}
			textMessageTemplate.Enabled=!checkUseDefaults.Checked;
		}

		private void RefreshView() {
			clinicNumOld=_selectedClinicNum;
			prefNameOld=comboBoxTemplates.GetSelected<PrefName>();
			textMessageTemplate.Text=_clinicPrefHelper.GetStringVal(comboBoxTemplates.GetSelected<PrefName>(),_selectedClinicNum);
			templateOld=textMessageTemplate.Text;
		}

		private void checkUseDefaults_Click(object sender,EventArgs e) {
			textMessageTemplate.Enabled=!checkUseDefaults.Checked;
		}

		private void comboBoxClinicPicker_SelectionChangeCommitted(object sender,EventArgs e) {
			CheckUIChanges();
			RefreshView();
		}

		private void comboBoxTemplates_SelectionChangeCommitted(object sender,EventArgs e) {
			CheckUIChanges();
			RefreshView();
		}

		/// <summary>This form uses a generic contextMenu for textboxes, so we need to use these event handlers to override the default menu.</summary>
		private void toolStripMenuItemCopy_Click(object sender,EventArgs e) {
			ToolStripItem toolStripItem=(ToolStripItem)sender;
			ContextMenuStrip contextMenu=(ContextMenuStrip)toolStripItem.Owner;
			((TextBox)contextMenu.SourceControl).Copy();
		}

		/// <summary>This form uses a generic contextMenu for textboxes, so we need to use these event handlers to override the default menu.</summary>
		private void toolStripMenuItemCut_Click(object sender,EventArgs e) {
			ToolStripItem toolStripItem=(ToolStripItem)sender;
			ContextMenuStrip contextMenu=(ContextMenuStrip)toolStripItem.Owner;
			((TextBox)contextMenu.SourceControl).Cut();
		}

		/// <summary>This form uses a generic contextMenu for textboxes, so we need to use these event handlers to override the default menu.</summary>
		private void toolStripMenuItemPaste_Click(object sender,EventArgs e) {
			ToolStripItem toolStripItem=(ToolStripItem)sender;
			ContextMenuStrip contextMenu=(ContextMenuStrip)toolStripItem.Owner;
			((TextBox)contextMenu.SourceControl).Paste();
		}

		/// <summary>Opens FormMessageReplacements to allow the user to select from replaceable tags to include in the templates.</summary>
		private void toolStripMenuItemInsertFields_Click(object sender,EventArgs e) {
			ToolStripItem toolStripItem=(ToolStripItem)sender;
			ContextMenuStrip contextMenu=(ContextMenuStrip)toolStripItem.Owner;
			TextBox textBox=((TextBox)contextMenu.SourceControl);
			using FormMessageReplacements FormMR=new FormMessageReplacements(
				MessageReplaceType.Appointment 
				| MessageReplaceType.Office 
				| MessageReplaceType.Patient 
				| MessageReplaceType.Misc
				,false);
			FormMR.MessageReplacementSystemType=MessageReplacementSystemType.SMS;
			FormMR.IsSelectionMode=true;
			FormMR.ShowDialog();
			if(FormMR.DialogResult==DialogResult.OK) {
				textBox.SelectedText=FormMR.Replacement;
			}
		}

		/// <summary>This form uses a generic contextMenu for textboxes, so we need to use these event handlers to override the default menu.</summary>
		private void toolStripMenuItemSelectAll_Click(object sender,EventArgs e) {
			ToolStripItem toolStripItem=(ToolStripItem)sender;
			ContextMenuStrip contextMenu=(ContextMenuStrip)toolStripItem.Owner;
			((TextBox)contextMenu.SourceControl).SelectAll();
		}

		/// <summary>This form uses a generic contextMenu for textboxes, so we need to use these event handlers to override the default menu.</summary>
		private void toolStripMenuItemUndo_Click(object sender,EventArgs e) {
			ToolStripItem toolStripItem=(ToolStripItem)sender;
			ContextMenuStrip contextMenu=(ContextMenuStrip)toolStripItem.Owner;
			((TextBox)contextMenu.SourceControl).Undo();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(checkUseDefaults.Checked==true) {
				_clinicPrefHelper.DeleteClinicPref(comboBoxTemplates.GetSelected<PrefName>(),_selectedClinicNum);
			}
			else {
				saveTemplate(comboBoxTemplates.GetSelected<PrefName>(),_selectedClinicNum,textMessageTemplate.Text);
			}
			Prefs.RefreshCache();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			Prefs.RefreshCache();
			DialogResult=DialogResult.Cancel;
		}
	}
}