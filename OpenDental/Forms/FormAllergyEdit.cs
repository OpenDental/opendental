using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAllergyEdit:FormODBase {
		public Allergy AllergyCur;
		private List<AllergyDef> _listAllergyDefs;
		private Snomed _snomedReaction;

		public FormAllergyEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAllergyEdit_Load(object sender,EventArgs e) {
			int allergyIndex=0;
			_listAllergyDefs=AllergyDefs.GetAll(false);
			if(_listAllergyDefs.Count<1) {
				MsgBox.Show(this,"Need to set up at least one Allergy from EHR setup window.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			for(int i=0;i<_listAllergyDefs.Count;i++) {
				comboAllergies.Items.Add(_listAllergyDefs[i].Description);
				if(!AllergyCur.IsNew && _listAllergyDefs[i].AllergyDefNum==AllergyCur.AllergyDefNum) {
					allergyIndex=i;
				}
			}
			_snomedReaction=Snomeds.GetByCode(AllergyCur.SnomedReaction);
			if(_snomedReaction!=null) {
				textSnomedReaction.Text=_snomedReaction.Description;
			}
			if(!AllergyCur.IsNew) {
				if(AllergyCur.DateAdverseReaction<DateTime.Parse("01-01-1880")) {
					textDate.Text="";
				}
				else {
					textDate.Text=AllergyCur.DateAdverseReaction.ToShortDateString();
				}
				comboAllergies.SelectedIndex=allergyIndex;
				textReaction.Text=AllergyCur.Reaction;
				checkActive.Checked=AllergyCur.StatusIsActive;
			}
			else {
				comboAllergies.SelectedIndex=0;
			}
			if(!Security.IsAuthorized(Permissions.PatAllergyListEdit)) {
				butOK.Enabled=false;
				butDelete.Enabled=false;
			}
		}

		private void butSnomedReactionSelect_Click(object sender,EventArgs e) {
			using FormSnomeds formSnomeds=new FormSnomeds();
			formSnomeds.IsSelectionMode=true;
			if(formSnomeds.ShowDialog()==DialogResult.OK) {
				_snomedReaction=formSnomeds.SnomedSelected;
				textSnomedReaction.Text=_snomedReaction.Description;
			}
		}

		private void butNoneSnomedReaction_Click(object sender,EventArgs e) {
			_snomedReaction=null;
			textSnomedReaction.Text="";
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(AllergyCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			Allergies.Delete(AllergyCur.AllergyNum);
			SecurityLogs.MakeLogEntry(Permissions.PatAllergyListEdit,AllergyCur.PatNum,AllergyDefs.GetDescription(AllergyCur.AllergyDefNum)+" deleted");
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//Validate
			if(textDate.Text!="") {
				try {
					DateTime.Parse(textDate.Text);
				}
				catch {
					MessageBox.Show("Please input a valid date.");
					return;
				}
			}
			//Save
			if(textDate.Text!="") {
				AllergyCur.DateAdverseReaction=DateTime.Parse(textDate.Text);
			}
			else {
				AllergyCur.DateAdverseReaction=DateTime.MinValue;
			}
			AllergyCur.AllergyDefNum=_listAllergyDefs[comboAllergies.SelectedIndex].AllergyDefNum;
			AllergyCur.Reaction=textReaction.Text;
			AllergyCur.SnomedReaction="";
			if(_snomedReaction!=null) {
				AllergyCur.SnomedReaction=_snomedReaction.SnomedCode;
			}
			AllergyCur.StatusIsActive=checkActive.Checked;
			if(AllergyCur.IsNew) {
				Allergies.Insert(AllergyCur);
				SecurityLogs.MakeLogEntry(Permissions.PatAllergyListEdit,AllergyCur.PatNum,AllergyDefs.GetDescription(AllergyCur.AllergyDefNum)+" added");
			}
			else {
				Allergies.Update(AllergyCur);
				SecurityLogs.MakeLogEntry(Permissions.PatAllergyListEdit,AllergyCur.PatNum,AllergyDefs.GetDescription(AllergyCur.AllergyDefNum)+" edited");
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}