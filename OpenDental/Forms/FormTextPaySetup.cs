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
		private string _templateOld;
		private long _clinicNumOld;
		private PrefName _prefNameOld;

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
			long clinicNum=comboBoxClinicPicker.GetSelectedClinic()?.ClinicNum??0;
			textMessageTemplate.Text=_clinicPrefHelper.GetStringVal(comboBoxTemplates.GetSelected<PrefName>(),clinicNum);
			_templateOld=textMessageTemplate.Text;
			_prefNameOld=comboBoxTemplates.GetSelected<PrefName>();
			_clinicNumOld=clinicNum;
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
				_clinicPrefHelper.ValChangedByUser(_prefNameOld,_clinicNumOld,textMessageTemplate.Text); //Save the template
			}
			_clinicPrefHelper.SyncAllPrefs();
		}

		private void CheckUIChanges() {
			long clinicNumSelected=comboBoxClinicPicker.GetSelectedClinic()?.ClinicNum??0;
			if (comboBoxTemplates.GetSelected<PrefName>()!=_prefNameOld || clinicNumSelected!=_clinicNumOld) {
				if (_templateOld!=textMessageTemplate.Text || checkUseDefaults.Checked==_clinicPrefHelper.ClinicHasClinicPref(_prefNameOld,_clinicNumOld)) {
					if(MsgBox.Show(MsgBoxButtons.YesNo,"Changes made have not been saved, would you like to save them?")) {
						if(checkUseDefaults.Checked) {
							_clinicPrefHelper.DeleteClinicPref(_prefNameOld,_clinicNumOld);
						}
						else {
							saveTemplate(_prefNameOld,_clinicNumOld,textMessageTemplate.Text);
						}
					}
				}
			}
			//Handles checkbox visibility when changing to the 'Default' option.
			if(clinicNumSelected!=0) {
				checkUseDefaults.Visible=true;
				checkUseDefaults.Checked=!_clinicPrefHelper.ClinicHasClinicPref(comboBoxTemplates.GetSelected<PrefName>(),clinicNumSelected);
			}
			else {
				checkUseDefaults.Visible=false;
				checkUseDefaults.Checked=false;
			}
			textMessageTemplate.Enabled=!checkUseDefaults.Checked;
		}

		private void RefreshView() {
			long clinicNumSelected=comboBoxClinicPicker.GetSelectedClinic()?.ClinicNum??0;
			_clinicNumOld=clinicNumSelected;
			_prefNameOld=comboBoxTemplates.GetSelected<PrefName>();
			textMessageTemplate.Text=_clinicPrefHelper.GetStringVal(comboBoxTemplates.GetSelected<PrefName>(),clinicNumSelected);
			_templateOld=textMessageTemplate.Text;
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
			ContextMenuStrip contextMenuStrip=(ContextMenuStrip)toolStripItem.Owner;
			((System.Windows.Forms.TextBox)contextMenuStrip.SourceControl).Copy();
		}

		/// <summary>This form uses a generic contextMenu for textboxes, so we need to use these event handlers to override the default menu.</summary>
		private void toolStripMenuItemCut_Click(object sender,EventArgs e) {
			ToolStripItem toolStripItem=(ToolStripItem)sender;
			ContextMenuStrip contextMenuStrip=(ContextMenuStrip)toolStripItem.Owner;
			((System.Windows.Forms.TextBox)contextMenuStrip.SourceControl).Cut();
		}

		/// <summary>This form uses a generic contextMenu for textboxes, so we need to use these event handlers to override the default menu.</summary>
		private void toolStripMenuItemPaste_Click(object sender,EventArgs e) {
			ToolStripItem toolStripItem=(ToolStripItem)sender;
			ContextMenuStrip contextMenuStrip=(ContextMenuStrip)toolStripItem.Owner;
			((System.Windows.Forms.TextBox)contextMenuStrip.SourceControl).Paste();
		}

		/// <summary>Opens FormMessageReplacements to allow the user to select from replaceable tags to include in the templates.</summary>
		private void toolStripMenuItemInsertFields_Click(object sender,EventArgs e) {
			ToolStripItem toolStripItem=(ToolStripItem)sender;
			ContextMenuStrip contextMenuStrip=(ContextMenuStrip)toolStripItem.Owner;
			System.Windows.Forms.TextBox textBox=((System.Windows.Forms.TextBox)contextMenuStrip.SourceControl);
			List<MessageReplaceType> listMessageReplaceTypes=new List<MessageReplaceType>();
			listMessageReplaceTypes.Add(MessageReplaceType.Appointment);
			listMessageReplaceTypes.Add(MessageReplaceType.Office);
			listMessageReplaceTypes.Add(MessageReplaceType.Patient);
			listMessageReplaceTypes.Add(MessageReplaceType.Misc);
			using FormMessageReplacements formMessageReplacements=new FormMessageReplacements(listMessageReplaceTypes,false);
			formMessageReplacements.MessageReplacementSystemType=MessageReplacementSystemType.SMS;
			formMessageReplacements.IsSelectionMode=true;
			formMessageReplacements.ShowDialog();
			if(formMessageReplacements.DialogResult==DialogResult.OK) {
				textBox.SelectedText=formMessageReplacements.Replacement;
			}
		}

		/// <summary>This form uses a generic contextMenu for textboxes, so we need to use these event handlers to override the default menu.</summary>
		private void toolStripMenuItemSelectAll_Click(object sender,EventArgs e) {
			ToolStripItem toolStripItem=(ToolStripItem)sender;
			ContextMenuStrip contextMenuStrip=(ContextMenuStrip)toolStripItem.Owner;
			((System.Windows.Forms.TextBox)contextMenuStrip.SourceControl).SelectAll();
		}

		/// <summary>This form uses a generic contextMenu for textboxes, so we need to use these event handlers to override the default menu.</summary>
		private void toolStripMenuItemUndo_Click(object sender,EventArgs e) {
			ToolStripItem toolStripItem=(ToolStripItem)sender;
			ContextMenuStrip contextMenuStrip=(ContextMenuStrip)toolStripItem.Owner;
			((System.Windows.Forms.TextBox)contextMenuStrip.SourceControl).Undo();
		}

		private void butOK_Click(object sender,EventArgs e) {
			long clinicNumSelected=comboBoxClinicPicker.GetSelectedClinic()?.ClinicNum??0;
			if(checkUseDefaults.Checked==true) {
				_clinicPrefHelper.DeleteClinicPref(comboBoxTemplates.GetSelected<PrefName>(),clinicNumSelected);
			}
			else {
				saveTemplate(comboBoxTemplates.GetSelected<PrefName>(),clinicNumSelected,textMessageTemplate.Text);
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