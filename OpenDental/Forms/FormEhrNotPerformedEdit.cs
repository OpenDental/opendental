using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrNotPerformedEdit:FormODBase {
		public EhrNotPerformed EhrNotPerfCur;
		public int SelectedItemIndex;
		public bool IsDateReadOnly;
		private List<EhrCode> listEhrCodesReason;

		public FormEhrNotPerformedEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			SelectedItemIndex=-1;//this will be set to the value of the enum EhrNotPerformedItem when this form is called
			Lan.F(this);
		}

		///<summary>If using the Add button on FormEhrNotPerformed, an input box will allow the user to select from the list of available items that are not being performed.  The SelectedItemIndex will hold the index of the item selected wich corresponds to the enum EhrNotPerformedItem.  We will use this selected item index to set the EhrNotPerformed code and code system.</summary>
		private void FormEhrNotPerformedEdit_Load(object sender,EventArgs e) {
			if(IsDateReadOnly) {
				textDate.ReadOnly=true;
			}
			List<string> listValueSetOIDs=new List<string>();
			switch(SelectedItemIndex) {
				case 0://BMIExam
					listValueSetOIDs=new List<string>{"2.16.840.1.113883.3.600.1.681"};//'BMI LOINC Value' value set
					break;
				case 1://InfluenzaVaccination
					listValueSetOIDs=new List<string> { "2.16.840.1.113883.3.526.3.402","2.16.840.1.113883.3.526.3.1254" };//'Influenza Vaccination' and 'Influenza Vaccine' value sets
					radioMedReason.Visible=true;
					radioPatReason.Visible=true;
					radioSysReason.Visible=true;
					break;
				case 2://TobaccoScreening
					listValueSetOIDs=new List<string> { "2.16.840.1.113883.3.526.3.1278" };//'Tobacco Use Screening' value set
					break;
				case 3://DocumentCurrentMeds
					listValueSetOIDs=new List<string> { "2.16.840.1.113883.3.600.1.462" };//'Current Medications Documented SNMD' value set
					break;
				default://should never happen
					break;
			}
			List<EhrCode> listEhrCodes=EhrCodes.GetForValueSetOIDs(listValueSetOIDs,true);
			if(listEhrCodes.Count==0) {//This should only happen if the EHR.dll does not exist or if the codes in the ehrcode list do not exist in the corresponding table
				MsgBox.Show(this,"The codes used for Not Performed items do not exist in the table in your database.  You should run the Code System Importer tool in Setup | Chart | EHR.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(EhrNotPerfCur.IsNew) {//if new, CodeValue and CodeSystem are not set, might have to select one
				if(listEhrCodes.Count==1) {//only one code in the selected value set, use it
					EhrNotPerfCur.CodeValue=listEhrCodes[0].CodeValue;
					EhrNotPerfCur.CodeSystem=listEhrCodes[0].CodeSystem;
				}
				else {
					List<string> listCodeDescripts=new List<string>();
					for(int i=0;i<listEhrCodes.Count;i++) {
						listCodeDescripts.Add(listEhrCodes[i].CodeValue+" - "+listEhrCodes[i].Description);
					}
					using InputBox chooseItem=new InputBox(Lan.g(this,"Select the "+Enum.GetNames(typeof(EhrNotPerformedItem))[SelectedItemIndex]+" not being performed from the list below."),listCodeDescripts);
					if(SelectedItemIndex==(int)EhrNotPerformedItem.InfluenzaVaccination) {
						//chooseItem.comboSelection.DropDownWidth=730;
					}
					if(chooseItem.ShowDialog()!=DialogResult.OK) {
						DialogResult=DialogResult.Cancel;
						return;
					}
					if(chooseItem.comboSelection.SelectedIndex==-1) {
						MsgBox.Show(this,"You must select the "+Enum.GetNames(typeof(EhrNotPerformedItem))[SelectedItemIndex]+" not being performed.");
						DialogResult=DialogResult.Cancel;
						return;
					}
					EhrNotPerfCur.CodeValue=listEhrCodes[chooseItem.comboSelection.SelectedIndex].CodeValue;
					EhrNotPerfCur.CodeSystem=listEhrCodes[chooseItem.comboSelection.SelectedIndex].CodeSystem;
				}
			}
			for(int i=0;i<listEhrCodes.Count;i++) {
				if(listEhrCodes[i].CodeValue==EhrNotPerfCur.CodeValue && listEhrCodes[i].CodeSystem==EhrNotPerfCur.CodeSystem) {
					textDescription.Text=listEhrCodes[i].Description;
				}
			}
			textCode.Text=EhrNotPerfCur.CodeValue;
			textCodeSystem.Text=EhrNotPerfCur.CodeSystem;
			textDate.Text=EhrNotPerfCur.DateEntry.ToShortDateString();
			textNote.Text=EhrNotPerfCur.Note;
			FillReasonList();
			if(comboCodeReason.SelectedIndex>0) {
				textCodeSystemReason.Text=listEhrCodesReason[comboCodeReason.SelectedIndex-1].CodeSystem;
				textDescriptionReason.Text=listEhrCodesReason[comboCodeReason.SelectedIndex-1].Description;
			}
		}

		private void FillReasonList() {
			List<string> listValueSetOIDsReason=new List<string>();
			string medicalReason="2.16.840.1.113883.3.526.3.1007";//'Medical Reason' value set
			string patientReason="2.16.840.1.113883.3.526.3.1008";//'Patient Reason' value set
			string systemReason="2.16.840.1.113883.3.526.3.1009";//'System Reason' value set
			string patientRefusedReason="2.16.840.1.113883.3.600.1.1503";//'Patient Reason Refused' value set
			string medicalOrOtherReason="2.16.840.1.113883.3.600.1.1502";//'Medical or Other reason not done' value set
			string limitedLifeExpectancy="2.16.840.1.113883.3.526.3.1259";//'Limited Life Expectancy' value set
			switch(SelectedItemIndex) {
				case 0://BMIExam
					listValueSetOIDsReason=new List<string> { patientRefusedReason,medicalOrOtherReason };
					break;
				case 1://InfluenzaVaccination
					if(radioPatReason.Checked) {
						listValueSetOIDsReason=new List<string> { patientReason };
					}
					else if(radioSysReason.Checked) {
						listValueSetOIDsReason=new List<string> { systemReason };
					}
					else if(radioMedReason.Checked) {
						listValueSetOIDsReason=new List<string> { medicalReason };
					}
					else {//if new or loading a previously saved item not performed, no radio is selected, set the appropriate radio and fill the list
						if(EhrNotPerfCur.IsNew) {
							radioMedReason.Checked=true;
							listValueSetOIDsReason=new List<string> { medicalReason };//default to medical reason list if new and no radio selected yet
						}
						else {//if previously saved, find the sub list this reason belongs to
							List<List<string>> listSublists=new List<List<string>> { new List<string> { medicalReason },new List<string> { patientReason },new List<string> { systemReason } };
							bool found=false;
							for(int i=0;i<listSublists.Count;i++) {
								listEhrCodesReason=EhrCodes.GetForValueSetOIDs(listSublists[i],true);
								for(int j=0;j<listEhrCodesReason.Count;j++) {
									if(listEhrCodesReason[j].CodeValue==EhrNotPerfCur.CodeValueReason && listEhrCodesReason[j].CodeSystem==EhrNotPerfCur.CodeSystemReason) {
										found=true;
										break;
									}
								}
								if(found) {
									if(i==0) {
										radioMedReason.Checked=true;
									}
									else if(i==1) {
										radioPatReason.Checked=true;
									}
									else {
										radioSysReason.Checked=true;
									}
									listValueSetOIDsReason=listSublists[i];
									break;
								}
							}
						}
					}
					break;
				case 2://TobaccoScreening
					listValueSetOIDsReason=new List<string> { medicalReason,limitedLifeExpectancy };
					break;
				case 3://DocumentCurrentMeds
					listValueSetOIDsReason=new List<string> { medicalOrOtherReason };
					break;
				default://should never happen
					break;
			}
			listEhrCodesReason=EhrCodes.GetForValueSetOIDs(listValueSetOIDsReason,true);//these are all SNOMEDCT codes and will only show if they exist in the snomed table.
			if(listEhrCodesReason.Count==0) {
				MsgBox.Show(this,"There are no codes in the database for reasons not performed.  You must run the Code System Importer tool in Setup | Chart | EHR to import the SNOMEDCT table in order to enter a valid reason.");
			}
			comboCodeReason.Items.Clear();
			comboCodeReason.Items.Add("none");
			comboCodeReason.SelectedIndex=0;//default to 'none' if no reason set for the not performed item
			for(int i=0;i<listEhrCodesReason.Count;i++) {
				comboCodeReason.Items.Add(listEhrCodesReason[i].CodeValue);
				if(EhrNotPerfCur.CodeValueReason==listEhrCodesReason[i].CodeValue && EhrNotPerfCur.CodeSystemReason==listEhrCodesReason[i].CodeSystem) {
					comboCodeReason.SelectedIndex=i+1;//+1 for 'none'
					textCodeSystemReason.Text=listEhrCodesReason[i].CodeSystem;
					textDescriptionReason.Text=listEhrCodesReason[i].Description;
				}
			}
		}

		private void radioReasonMed_Click(object sender,EventArgs e) {
			textCodeSystemReason.Clear();
			textDescriptionReason.Clear();
			FillReasonList();
		}

		private void radioReasonPat_Click(object sender,EventArgs e) {
			textCodeSystemReason.Clear();
			textDescriptionReason.Clear();
			FillReasonList();
		}

		private void radioReasonSys_Click(object sender,EventArgs e) {
			textCodeSystemReason.Clear();
			textDescriptionReason.Clear();
			FillReasonList();
		}

		private void comboReasonCode_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboCodeReason.SelectedIndex==0) {
				textCodeSystemReason.Clear();
				textDescriptionReason.Clear();
			}
			else {
				textCodeSystemReason.Text=listEhrCodesReason[comboCodeReason.SelectedIndex-1].CodeSystem;
				textDescriptionReason.Text=listEhrCodesReason[comboCodeReason.SelectedIndex-1].Description;
			}
		}

		private void comboCodeReason_DropDown(object sender,EventArgs e) {
			int selectedIndex=comboCodeReason.SelectedIndex;
			comboCodeReason.Items.Clear();
			comboCodeReason.Items.Add("none");
			for(int i=0;i<listEhrCodesReason.Count;i++) {
				comboCodeReason.Items.Add(listEhrCodesReason[i].CodeValue+" - "+listEhrCodesReason[i].Description);
			}
			comboCodeReason.SelectedIndex=selectedIndex;
		}

		private void comboCodeReason_DropDownClosed(object sender,EventArgs e) {
			int selectedIndex=comboCodeReason.SelectedIndex;
			comboCodeReason.Items.Clear();
			comboCodeReason.Items.Add("none");
			for(int i=0;i<listEhrCodesReason.Count;i++) {
				comboCodeReason.Items.Add(listEhrCodesReason[i].CodeValue);
			}
			comboCodeReason.SelectedIndex=selectedIndex;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(EhrNotPerfCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			Vitalsign vitCur=Vitalsigns.GetFromEhrNotPerformedNum(EhrNotPerfCur.EhrNotPerformedNum);
			if(vitCur!=null) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Deleting this will remove it from the vitalsign exam it refers to.\r\nDelete anyway?")) {
					return;
				}
				vitCur.EhrNotPerformedNum=0;
				Vitalsigns.Update(vitCur);
			}
			EhrNotPerformeds.Delete(EhrNotPerfCur.EhrNotPerformedNum);
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//validate--------------------------------------
			DateTime date;
			if(textDate.Text=="") {
				MsgBox.Show(this,"Please enter a date.");
				return;
			}
			try {
				date=DateTime.Parse(textDate.Text);
			}
			catch {
				MsgBox.Show(this,"Please fix date first.");
				return;
			}
			//we force the date to match the item not being performed (like vitalsign exam) by making the date text box read only if launched from other item.  Users can still manually add a not performed item from FormEhrNotPerformed by pressing Add and choose any valid date they wish, but it will not be linked to an item.
			string codeValReas="";
			string codeSysReas="";
			if(comboCodeReason.SelectedIndex<1) {//selected 'none' or possibly still -1 (although -1 should never happen)
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"If you do not select one of the reasons provided it may be harder to meet your Clinical Quality Measures.  Are you sure you want to continue without selecting a valid reason for not performing the "+Enum.GetNames(typeof(EhrNotPerformedItem))[SelectedItemIndex]+"?")) {
					return;
				}
				codeValReas="";
				codeSysReas="";
			}
			else {
				codeValReas=listEhrCodesReason[comboCodeReason.SelectedIndex-1].CodeValue;//SelectedIndex-1 to account for 'none'
				codeSysReas=listEhrCodesReason[comboCodeReason.SelectedIndex-1].CodeSystem;
			}
			//save--------------------------------------
			EhrNotPerfCur.DateEntry=date;
			EhrNotPerfCur.CodeValueReason=codeValReas;
			EhrNotPerfCur.CodeSystemReason=codeSysReas;
			EhrNotPerfCur.Note=textNote.Text;
			if(EhrNotPerfCur.IsNew) {
				EhrNotPerfCur.EhrNotPerformedNum=EhrNotPerformeds.Insert(EhrNotPerfCur);
			}
			else {
				EhrNotPerformeds.Update(EhrNotPerfCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}