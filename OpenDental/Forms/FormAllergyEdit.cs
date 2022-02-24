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
		private List<AllergyDef> allergyDefList;
		private Snomed snomedReaction;

		public FormAllergyEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAllergyEdit_Load(object sender,EventArgs e) {
			int allergyIndex=0;
			allergyDefList=AllergyDefs.GetAll(false);
			if(allergyDefList.Count<1) {
				MsgBox.Show(this,"Need to set up at least one Allergy from EHR setup window.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			for(int i=0;i<allergyDefList.Count;i++) {
				comboAllergies.Items.Add(allergyDefList[i].Description);
				if(!AllergyCur.IsNew && allergyDefList[i].AllergyDefNum==AllergyCur.AllergyDefNum) {
					allergyIndex=i;
				}
			}
			snomedReaction=Snomeds.GetByCode(AllergyCur.SnomedReaction);
			if(snomedReaction!=null) {
				textSnomedReaction.Text=snomedReaction.Description;
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
		}

		private void butSnomedReactionSelect_Click(object sender,EventArgs e) {
			using FormSnomeds formS=new FormSnomeds();
			formS.IsSelectionMode=true;
			if(formS.ShowDialog()==DialogResult.OK) {
				snomedReaction=formS.SelectedSnomed;
				textSnomedReaction.Text=snomedReaction.Description;
			}
		}

		private void butNoneSnomedReaction_Click(object sender,EventArgs e) {
			snomedReaction=null;
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
			AllergyCur.AllergyDefNum=allergyDefList[comboAllergies.SelectedIndex].AllergyDefNum;
			AllergyCur.Reaction=textReaction.Text;
			AllergyCur.SnomedReaction="";
			if(snomedReaction!=null) {
				AllergyCur.SnomedReaction=snomedReaction.SnomedCode;
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