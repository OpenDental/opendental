using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Windows.Forms.DataVisualization.Charting;

namespace OpenDental {
	public partial class FormEServicesWebSchedNotify:FormODBase {
		#region Fields
		///<summary>The fake clinic num used for the "Default" clinic option in the WebSchedVerify clinic combo box.  We need to change this to the constant that's in the combobox for clarity.</summary>
		private const int CLINIC_NUM_DEFAULT=0;
		///<summary>The in-memory list of updated ClinicPrefs related to WebSchedVerify, used to track changes made while the window is open.</summary>
		private List<ClinicPref> _listClinicPrefsWebSchedVerify=new List<ClinicPref>();
		///<summary>The in-memory list of original ClinicPrefs related to WebSchedVerify, used to compare changes when saving on window close.</summary>
		private List<ClinicPref> _listClinicPrefsWebSchedVerify_Old;
		///<summary>The list of prefNames that will be modified.</summary>
		private List<PrefName> _listPrefNamesWebSchedVerify=new List<PrefName>();
		///<summary>The list of preferences to use for ASAP.</summary>
		private List<PrefName> _listPrefNamesWebSchedVerifyAsap=new List<PrefName>() {
				PrefName.WebSchedVerifyASAPEmailBody,
				PrefName.WebSchedVerifyASAPEmailSubj,
				PrefName.WebSchedVerifyAsapEmailTemplateType,
				PrefName.WebSchedVerifyASAPText,
				PrefName.WebSchedVerifyASAPType,
		};
		///<summary>The list of preferences to use for ExistingPat.</summary>
		private List<PrefName> _listPrefNamesWebSchedVerifyExistingPat=new List<PrefName> () {
				PrefName.WebSchedVerifyExistingPatEmailBody,
				PrefName.WebSchedVerifyExistingPatEmailSubj,
				PrefName.WebSchedVerifyExistingPatEmailTemplateType,
				PrefName.WebSchedVerifyExistingPatText,
				PrefName.WebSchedVerifyExistingPatType,
		};
		///<summary>The list of preferences to use for NewPat.</summary>
		private List<PrefName> _listPrefNamesWebSchedVerifyNewPat=new List<PrefName>() {
				PrefName.WebSchedVerifyNewPatEmailBody,
				PrefName.WebSchedVerifyNewPatEmailSubj,
				PrefName.WebSchedVerifyNewPatEmailTemplateType,
				PrefName.WebSchedVerifyNewPatText,
				PrefName.WebSchedVerifyNewPatType,
		};
		///<summary>The list of preferences to use for Recall.</summary>
		private List<PrefName> _listPrefNamesWebSchedVerifyRecall=new List<PrefName>() {
				PrefName.WebSchedVerifyRecallEmailBody,
				PrefName.WebSchedVerifyRecallEmailSubj,
				PrefName.WebSchedVerifyRecallEmailTemplateType,
				PrefName.WebSchedVerifyRecallText,
				PrefName.WebSchedVerifyRecallType,
		};

		private PrefName _prefNameEmailBody;
		private PrefName _prefNameEmailSubj;
		private PrefName _prefNameEmailType;
		private PrefName _prefNameTextTemplate;
		private PrefName _prefNameType;
		///<summary>The type of notification to load / save</summary>
		private WebSchedNotifyType _webSchedType;
		#endregion Fields

		public FormEServicesWebSchedNotify(WebSchedNotifyType webSchedType) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_webSchedType=webSchedType;
		}

		private void FormEServicesWebSchedNotify_Load(object sender,EventArgs e) {
			//Load in an existing list of clinicprefs so we can keep in in-memory record of changes
			ClinicPrefs.RefreshCache();
			//Set the preferences we will use based on who called
			switch(_webSchedType) {
				case WebSchedNotifyType.ASAP:
					_prefNameEmailBody=PrefName.WebSchedVerifyASAPEmailBody;
					_prefNameEmailSubj=PrefName.WebSchedVerifyASAPEmailSubj;
					_prefNameEmailType=PrefName.WebSchedVerifyAsapEmailTemplateType;
					_prefNameTextTemplate=PrefName.WebSchedVerifyASAPText;
					_prefNameType=PrefName.WebSchedVerifyASAPType;
					_listPrefNamesWebSchedVerify=_listPrefNamesWebSchedVerifyAsap;
					groupBox.Text="ASAP";
					this.Text+=" - ASAP";
					break;
				case WebSchedNotifyType.ExistingPat:
					_prefNameEmailBody=PrefName.WebSchedVerifyExistingPatEmailBody;
					_prefNameEmailSubj=PrefName.WebSchedVerifyExistingPatEmailSubj;
					_prefNameEmailType=PrefName.WebSchedVerifyExistingPatEmailTemplateType;
					_prefNameTextTemplate=PrefName.WebSchedVerifyExistingPatText;
					_prefNameType=PrefName.WebSchedVerifyExistingPatType;
					_listPrefNamesWebSchedVerify=_listPrefNamesWebSchedVerifyExistingPat;
					groupBox.Text="Existing Patient";
					this.Text+=" - Existing Patient";
					break;
				case WebSchedNotifyType.NewPat:
					_prefNameEmailBody=PrefName.WebSchedVerifyNewPatEmailBody;
					_prefNameEmailSubj=PrefName.WebSchedVerifyNewPatEmailSubj;
					_prefNameEmailType=PrefName.WebSchedVerifyNewPatEmailTemplateType;
					_prefNameTextTemplate=PrefName.WebSchedVerifyNewPatText;
					_prefNameType=PrefName.WebSchedVerifyNewPatType;
					_listPrefNamesWebSchedVerify=_listPrefNamesWebSchedVerifyNewPat;
					groupBox.Text="New Patient";
					this.Text+=" - New Patient";
					break;
				case WebSchedNotifyType.Recall:
					_prefNameEmailBody=PrefName.WebSchedVerifyRecallEmailBody;
					_prefNameEmailSubj=PrefName.WebSchedVerifyRecallEmailSubj;
					_prefNameEmailType=PrefName.WebSchedVerifyRecallEmailTemplateType;
					_prefNameTextTemplate=PrefName.WebSchedVerifyRecallText;
					_prefNameType=PrefName.WebSchedVerifyRecallType;
					_listPrefNamesWebSchedVerify=_listPrefNamesWebSchedVerifyRecall;
					groupBox.Text="Recall";
					this.Text+=" - Recall";
					break;
			}
			for(int i=0;i<_listPrefNamesWebSchedVerify.Count;i++) {
				_listClinicPrefsWebSchedVerify.AddRange(ClinicPrefs.GetPrefAllClinics(_listPrefNamesWebSchedVerify[i]));
				Pref pref=Prefs.GetPref(_listPrefNamesWebSchedVerify[i].ToString());
				_listClinicPrefsWebSchedVerify.Add(new ClinicPref() { ClinicNum=CLINIC_NUM_DEFAULT,PrefName=_listPrefNamesWebSchedVerify[i],ValueString=pref.ValueString });
			}
			_listClinicPrefsWebSchedVerify_Old=_listClinicPrefsWebSchedVerify.Select(x => x.Clone()).ToList();
			//Fill in the UI
			checkUseDefaultsVerify.Visible=false;
			if(PrefC.HasClinicsEnabled && comboClinicVerify.SelectedClinicNum!=0) {
				checkUseDefaultsVerify.Visible=true;
			}
			FillTemplate();
		}

		#region Methods - Private
		private void RefreshEmail(WebBrowser emailBody,string emailText,bool isRawHtml) {
			if(isRawHtml) {
				emailBody.DocumentText=emailText;
				return;//text is already in HTML, it does not need to be translated. 
			}
			ODException.SwallowAnyException(() => {
				string text=MarkupEdit.TranslateToXhtml(emailText,isPreviewOnly:true,hasWikiPageTitles:false,isEmail:true);
				emailBody.DocumentText=text;
			});
		}

		///<summary>Fill in the template data for the current clinic and caller.</summary>
		private void FillTemplate() {
			SetRadioButtonVal(_prefNameType);
			textMessageTemplate.Text=GetTemplateVal(_prefNameTextTemplate);
			textEmailSubj.Text=GetTemplateVal(_prefNameEmailSubj);
			RefreshEmail(browserEmailBody,GetTemplateVal(_prefNameEmailBody),(EmailType)GetInt(_prefNameEmailType)==EmailType.RawHtml);
		}
		#endregion Methods - Private

		#region Methods - Event Handlers
		private void butEditEmail_Click(object sender,EventArgs e) {
			using FormEmailEdit formEmailEdit=new FormEmailEdit {
				MarkupText=GetTemplateVal(_prefNameEmailBody),
				DoCheckForDisclaimer=true,
				IsRawAllowed=true,
				IsRaw=(EmailType)GetInt(_prefNameEmailType)==EmailType.RawHtml
			};
			formEmailEdit.ShowDialog();
			if(formEmailEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			UpdateClinicPref(_prefNameEmailBody,formEmailEdit.MarkupText);//update template text
			EmailType enumEmailtype=EmailType.Html;
			if(formEmailEdit.IsRaw) {
				enumEmailtype=EmailType.RawHtml;
			}
			UpdateClinicPref(_prefNameEmailType,((int)enumEmailtype).ToString());
			RefreshEmail(browserEmailBody,formEmailEdit.MarkupText,formEmailEdit.IsRaw);
		}

		/// <summary>All the user to undo all changes they have made to the currently selected clinic.</summary>
		private void butRestore_Click(object sender,EventArgs e) {
			bool isAccepted=MsgBox.Show(this,MsgBoxButtons.YesNo,"Undo all changes to the template in this clinic?");
			if(isAccepted) {
				for(int i=0;i<_listPrefNamesWebSchedVerify.Count;i++) {
					TryRestoreClinicPrefOld(_listPrefNamesWebSchedVerify[i]);
				}
				FillTemplate();
			}
		}

		///<summary>Event handler for CheckUseDefaults check changed.</summary>
		private void checkUseDefaults_CheckChanged(object sender,EventArgs e) {
			if(checkUseDefaultsVerify.Checked) {
				groupBox.Enabled=false;
				_listClinicPrefsWebSchedVerify.RemoveAll(x => x.ClinicNum==comboClinicVerify.SelectedClinicNum);
			}
			else {
				groupBox.Enabled=true;
				//Only do this logic if the check change result from the user manually checking the box, not from changing clinics
				if(!_listClinicPrefsWebSchedVerify.Any(x => x.ClinicNum==comboClinicVerify.SelectedClinicNum)) {
					for(int i=0;i<_listPrefNamesWebSchedVerify.Count;i++) {
						TryRestoreClinicPrefOld(_listPrefNamesWebSchedVerify[i]);
					}
				}
			}
			FillTemplate();
		}

		///<summary>Event handler for ComboClinics index changed.</summary>
		private void comboClinicVerify_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboClinicVerify.SelectedClinicNum==CLINIC_NUM_DEFAULT) {//'Default' is selected.
				checkUseDefaultsVerify.Visible=false;
				checkUseDefaultsVerify.Checked=false;
			}
			else {
				checkUseDefaultsVerify.Visible=true;
				if(!_listClinicPrefsWebSchedVerify.Exists(x => x.ClinicNum==comboClinicVerify.SelectedClinicNum)) {
					checkUseDefaultsVerify.Checked=true;
				}
				else {
					checkUseDefaultsVerify.Checked=false;
				}
			}
			FillTemplate();
		}

		private void textEmailSubj_Leave(object sender,EventArgs e) {
			TextBox textBox=(TextBox)sender;
			UpdateClinicPref(_prefNameEmailSubj,textBox.Text);
		}

		private void textMessageTemplate_Leave(object sender,EventArgs e) {
			TextBox textBox=(TextBox)sender;
			UpdateClinicPref(_prefNameTextTemplate,textBox.Text);
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
		private void toolStripMenuItemInsertReplacements_Click(object sender,EventArgs e) {
			ToolStripItem toolStripItem=(ToolStripItem)sender;
			ContextMenuStrip contextMenu=(ContextMenuStrip)toolStripItem.Owner;
			TextBox textBox=((TextBox)contextMenu.SourceControl);
			//PHI is not supposed to be communicated via text message.
			bool allowPHI=(!ListTools.In(textBox.Name,textMessageTemplate.Name));
			using FormMessageReplacements FormMR=new FormMessageReplacements(
				MessageReplaceType.Appointment | MessageReplaceType.Office | MessageReplaceType.Patient,allowPHI);
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

		///<summary>Event handler for RadioButtons check changed.</summary>
		private void WebSchedVerify_RadioButtonCheckChanged(object sender,EventArgs e) {
			RadioButton buttonCur=(RadioButton)sender;
			GroupBox groupBox=(GroupBox)buttonCur.Parent;
			if(buttonCur.Checked) {
				WebSchedVerifyType verifyType=(WebSchedVerifyType)buttonCur.Tag;
				if(_listClinicPrefsWebSchedVerify.Any(x => x.ClinicNum==comboClinicVerify.SelectedClinicNum)) {
					//We only want to do this part when the user manually checked this, not when the check-defaults forced it to change
					UpdateClinicPref(_prefNameType,POut.Int((int)verifyType));
				}
			}
		}
		#endregion Methods - Event Handlers

		#region Helpers
		private int GetInt(PrefName prefName) {
			return PIn.Int(GetTemplateVal(prefName));
		}

		///<summary>Returns the clinic pref value for the currently selected clinic and provided PrefName, or the default pref if there is none.</summary>
		private string GetTemplateVal(PrefName prefName) {
			ClinicPref clinicPref=_listClinicPrefsWebSchedVerify.FirstOrDefault(x => x.ClinicNum==comboClinicVerify.SelectedClinicNum && x.PrefName==prefName);
			ClinicPref clinicPrefDefault=_listClinicPrefsWebSchedVerify.FirstOrDefault(x => x.ClinicNum==CLINIC_NUM_DEFAULT && x.PrefName==prefName);
			//ClinicPref won't be available if it has not been created previously.
			string clinicPrefValueString=clinicPrefDefault.ValueString;
			if(clinicPref!=null) {
				clinicPrefValueString=clinicPref.ValueString;
			}
			return clinicPrefValueString;
		}

		///<summary>Checks the currently selected radio button for the given PrefName and groupBox, based on the radio button tags.</summary>
		private void SetRadioButtonVal(PrefName prefName) {
			WebSchedVerifyType verifyType=(WebSchedVerifyType)PIn.Int(GetTemplateVal(prefName));
			RadioButton buttonMatch=groupBoxRadio.Controls.OfType<RadioButton>().FirstOrDefault(x => (WebSchedVerifyType)x.Tag==verifyType);
			buttonMatch.Checked=true;
		}

		///<summary>Tries to get the original clinic pref that was loaded in when the form first opened, and reload it into the in-memory clinic pref 
		///list. If there is no old pref, this loads the default pref value for that clinic into the in-memory list.</summary>
		private void TryRestoreClinicPrefOld(PrefName prefName) {
			ClinicPref clinicPref=_listClinicPrefsWebSchedVerify_Old.FindAll(x => x.ClinicNum==comboClinicVerify.SelectedClinicNum && x.PrefName==prefName).FirstOrDefault();
			if(clinicPref==null) {
				clinicPref=_listClinicPrefsWebSchedVerify.FindAll(x => x.ClinicNum==CLINIC_NUM_DEFAULT && x.PrefName==prefName).First();
			}
			UpdateClinicPref(prefName,clinicPref.ValueString);
		}

		///<summary>Updates the in-memory clinic pref list with the given valueString for the provided prefName and currently selected clinic.</summary>
		private void UpdateClinicPref(PrefName prefName,string valueString) {
			ClinicPref clinicPref=_listClinicPrefsWebSchedVerify.FirstOrDefault(x => x.ClinicNum==comboClinicVerify.SelectedClinicNum && x.PrefName==prefName);
			if(clinicPref==null) {
				_listClinicPrefsWebSchedVerify.Add(new ClinicPref() { PrefName=prefName,ClinicNum=comboClinicVerify.SelectedClinicNum,ValueString=valueString });
			}
			else {
				clinicPref.ValueString=valueString;
			}
		}
		#endregion Helpers

		///<summary>Save template changes made in WebSchedVerify.</summary>
		private void SaveWebSchedVerify() {
			List<long> listClinics=Clinics.GetForUserod(Security.CurUser).Select(x => x.ClinicNum).ToList();
			for(int i = 0;i<_listPrefNamesWebSchedVerify.Count;i++) {
				for(int j = 0;j<listClinics.Count;j++) {
					ClinicPref clinicPrefNew=_listClinicPrefsWebSchedVerify.FirstOrDefault(x => x.PrefName==_listPrefNamesWebSchedVerify[i] && x.ClinicNum==listClinics[j]);
					ClinicPref clinicPrefOld=_listClinicPrefsWebSchedVerify_Old.FirstOrDefault(x => x.PrefName==_listPrefNamesWebSchedVerify[i] && x.ClinicNum==listClinics[j]);
					if(clinicPrefOld==null && clinicPrefNew==null) { //skip items not in either list
						continue;
					}
					else if(clinicPrefOld==null && clinicPrefNew!=null) { //insert items in the new list and not the old list
						ClinicPrefs.Insert(clinicPrefNew);
					}
					else if(clinicPrefOld!=null && clinicPrefNew==null) { //delete items in the old list and not the new list
						ClinicPrefs.Delete(clinicPrefOld.ClinicPrefNum);
					}
					else { //update items that have changed
						ClinicPrefs.Update(clinicPrefNew,clinicPrefOld);
					}
				}
				ClinicPref clinicPrefDefault=_listClinicPrefsWebSchedVerify.FirstOrDefault(x => x.PrefName==_listPrefNamesWebSchedVerify[i] && x.ClinicNum==CLINIC_NUM_DEFAULT);
				if(clinicPrefDefault!=null) {
					Prefs.UpdateString(_listPrefNamesWebSchedVerify[i],clinicPrefDefault.ValueString);
				}
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(Patients.DoesContainPHIField(textMessageTemplate.Text)) {
				MsgBox.Show(this,"Text Message template is not allowed to contain Protected Health Information.");
				return;
			}
			SaveWebSchedVerify();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}