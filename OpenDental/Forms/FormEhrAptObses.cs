using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDentBusiness.HL7;

namespace OpenDental {
	public partial class FormEhrAptObses:FormODBase {

		private Appointment _appt=null;

		public FormEhrAptObses(Appointment appt) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_appt=appt;
		}

		private void FormEhrAptObses_Load(object sender,EventArgs e) {
			FillGridObservations();
		}

		private void FillGridObservations() {
			gridObservations.BeginUpdate();
			gridObservations.ListGridColumns.Clear();
			gridObservations.ListGridColumns.Add(new UI.GridColumn("Observation",200));//0
			gridObservations.ListGridColumns.Add(new UI.GridColumn("Value Type",200));//1
			gridObservations.ListGridColumns.Add(new UI.GridColumn("Value",100){ IsWidthDynamic=true });//2
			gridObservations.ListGridRows.Clear();
			List<EhrAptObs> listEhrAptObses=EhrAptObses.Refresh(_appt.AptNum);
			for(int i=0;i<listEhrAptObses.Count;i++) {
				EhrAptObs obs=listEhrAptObses[i];
				UI.GridRow row=new UI.GridRow();
				row.Tag=obs;
				row.Cells.Add(obs.IdentifyingCode.ToString());//0 Observation
				if(obs.ValType==EhrAptObsType.Coded) {
					row.Cells.Add(obs.ValType.ToString()+" - "+obs.ValCodeSystem);//1 Value Type
					if(obs.ValCodeSystem=="LOINC") {
						Loinc loincValue=Loincs.GetByCode(obs.ValReported);
						row.Cells.Add(loincValue.NameShort);//2 Value
					}
					else if(obs.ValCodeSystem=="SNOMEDCT") {
						Snomed snomedValue=Snomeds.GetByCode(obs.ValReported);
						row.Cells.Add(snomedValue.Description);//2 Value
					}
					else if(obs.ValCodeSystem=="ICD9") {
						ICD9 icd9Value=ICD9s.GetByCode(obs.ValReported);
						row.Cells.Add(icd9Value.Description);//2 Value
					}
					else if(obs.ValCodeSystem=="ICD10") {
						Icd10 icd10Value=Icd10s.GetByCode(obs.ValReported);
						row.Cells.Add(icd10Value.Description);//2 Value
					}
				}
				else if(obs.ValType==EhrAptObsType.Address) {
					string sendingFacilityAddress1=PrefC.GetString(PrefName.PracticeAddress);
					string sendingFacilityAddress2=PrefC.GetString(PrefName.PracticeAddress2);
					string sendingFacilityCity=PrefC.GetString(PrefName.PracticeCity);
					string sendingFacilityState=PrefC.GetString(PrefName.PracticeST);
					string sendingFacilityZip=PrefC.GetString(PrefName.PracticeZip);
					if(PrefC.HasClinicsEnabled && _appt.ClinicNum!=0) {//Using clinics and a clinic is assigned.
						Clinic clinic=Clinics.GetClinic(_appt.ClinicNum);
						sendingFacilityAddress1=clinic.Address;
						sendingFacilityAddress2=clinic.Address2;
						sendingFacilityCity=clinic.City;
						sendingFacilityState=clinic.State;
						sendingFacilityZip=clinic.Zip;
					}
					row.Cells.Add(obs.ValType.ToString());//1 Value Type
					row.Cells.Add(sendingFacilityAddress1+" "+sendingFacilityAddress2+" "+sendingFacilityCity+" "+sendingFacilityState+" "+sendingFacilityZip);//2 Value
				}
				else {
					row.Cells.Add(obs.ValType.ToString());//1 Value Type
					row.Cells.Add(obs.ValReported);//2 Value
				}
				gridObservations.ListGridRows.Add(row);
			}
			gridObservations.EndUpdate();
		}

		private void gridObservations_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			EhrAptObs obs=(EhrAptObs)gridObservations.ListGridRows[e.Row].Tag;
			using FormEhrAptObsEdit formE=new FormEhrAptObsEdit(obs);
			if(formE.ShowDialog()==DialogResult.OK) {
				if(obs.EhrAptObsNum!=0) {//Was not deleted.
					EhrAptObses.Update(obs);
				}
				FillGridObservations();
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			EhrAptObs obs=new EhrAptObs();
			obs.IsNew=true;
			obs.AptNum=_appt.AptNum;
			using FormEhrAptObsEdit formE=new FormEhrAptObsEdit(obs);
			if(formE.ShowDialog()==DialogResult.OK) {
				EhrAptObses.Insert(obs);
				FillGridObservations();
			}
		}

		private void butExportHL7_Click(object sender,EventArgs e) {
			EhrADT_A01 adt_a03=null;
			try {
				adt_a03=new EhrADT_A01(_appt);
			}
			catch(Exception ex) {//Exception happens when validation fails.
				MessageBox.Show(ex.Message);//Show validation error messages.
				return;
			}
			string outputStr=adt_a03.GenerateMessage();
			SaveFileDialog dlg=new SaveFileDialog();
			dlg.FileName="adt.txt";
			DialogResult result=dlg.ShowDialog();
			if(result!=DialogResult.OK) {
				return;
			}
			if(File.Exists(dlg.FileName)) {
				if(MessageBox.Show("Overwrite existing file?","",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
					return;
				}
			}
			File.WriteAllText(dlg.FileName,outputStr);
			MessageBox.Show("Saved");
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}