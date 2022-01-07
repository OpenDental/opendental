using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEhrVaccines:FormODBase {
		public Patient PatCur;
		private List<VaccinePat> VaccineList;
		private EhrPatient _ehrPatientCur;

		public FormEhrVaccines() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormVaccines_Load(object sender,EventArgs e) {
			FillGridVaccine();
			_ehrPatientCur=EhrPatients.Refresh(PatCur.PatNum);
			listVacShareOk.SelectedIndex=(int)_ehrPatientCur.VacShareOk;
		}

		private void FillGridVaccine() {
			gridVaccine.BeginUpdate();
			gridVaccine.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Date",90);
			gridVaccine.ListGridColumns.Add(col);
			col=new GridColumn("Vaccine",100);
			gridVaccine.ListGridColumns.Add(col);
			VaccineList=VaccinePats.Refresh(PatCur.PatNum);
			gridVaccine.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<VaccineList.Count;i++) {
				row=new GridRow();
				if(VaccineList[i].DateTimeStart.Year<1880){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(VaccineList[i].DateTimeStart.ToShortDateString());
				}
				string str="";
				if(VaccineList[i].VaccineDefNum==0) {
					str="Not administered: "+VaccineList[i].Note;
				}
				else { 				
					str=VaccineDefs.GetOne(VaccineList[i].VaccineDefNum).VaccineName;
				}
				row.Cells.Add(str);
				gridVaccine.ListGridRows.Add(row);
			}
			gridVaccine.EndUpdate();
		}

		private void gridVaccine_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			if(gridVaccine.GetSelectedIndex()==-1) {
				return;
			}
			using FormEhrVaccinePatEdit FormV=new FormEhrVaccinePatEdit();
			FormV.VaccinePatCur=VaccineList[gridVaccine.GetSelectedIndex()];
			FormV.ShowDialog();
			FillGridVaccine();
		}

		private void butAddVaccine_Click(object sender,EventArgs e) {
			using FormEhrVaccinePatEdit FormV=new FormEhrVaccinePatEdit();
			FormV.VaccinePatCur=new VaccinePat();
			FormV.VaccinePatCur.PatNum=PatCur.PatNum;
			FormV.VaccinePatCur.DateTimeStart=DateTime.Now;
			FormV.VaccinePatCur.DateTimeEnd=DateTime.Now;
			FormV.IsNew=true;
			FormV.ShowDialog();
			FillGridVaccine();
			}

		private void butExport_Click(object sender,EventArgs e) {
			if(gridVaccine.SelectedIndices.Length==0) {
				MessageBox.Show("Please select at least one vaccine.");
				return;
			}
			List<VaccinePat> vaccines=new List<VaccinePat>();
			for(int i=0;i<gridVaccine.SelectedIndices.Length;i++) {
				vaccines.Add(VaccineList[gridVaccine.SelectedIndices[i]]);
			}
			OpenDentBusiness.HL7.EhrVXU vxu=null;
			try {
				vxu=new OpenDentBusiness.HL7.EhrVXU(PatCur,vaccines);
			}
			catch(Exception ex) {//Exception happens when validation fails.
				MessageBox.Show(ex.Message);//Show validation error messages.
				return;
			}
			string outputStr=vxu.GenerateMessage();
			SaveFileDialog dlg=new SaveFileDialog();
			dlg.FileName="vxu.txt";
			DialogResult result=dlg.ShowDialog();
			if(result!=DialogResult.OK){
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

		private void butSubmitImmunization_Click(object sender,EventArgs e) {
			if(gridVaccine.SelectedIndices.Length==0) {
				MessageBox.Show("Please select at least one vaccine.");
				return;
			}
			List<VaccinePat> vaccines=new List<VaccinePat>();
			for(int i=0;i<gridVaccine.SelectedIndices.Length;i++) {
				vaccines.Add(VaccineList[gridVaccine.SelectedIndices[i]]);
			}
			OpenDentBusiness.HL7.EhrVXU vxu=null;
			try {
				vxu=new OpenDentBusiness.HL7.EhrVXU(PatCur,vaccines);
			}
			catch(Exception ex) {//Exception happens when validation fails.
				MessageBox.Show(ex.Message);//Show validation error messages.
				return;
			}
			string outputStr=vxu.GenerateMessage();
			Cursor=Cursors.WaitCursor;
			try {
				EmailMessages.SendTestUnsecure("Immunization Submission","vxu.txt",outputStr);
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				return;
			}
			Cursor=Cursors.Default;
			MessageBox.Show("Sent");
		}

		private void listVacShareOk_MouseClick(object sender,MouseEventArgs e) {
			_ehrPatientCur.VacShareOk=(YN)listVacShareOk.SelectedIndex;
			EhrPatients.Update(_ehrPatientCur);
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		

	}
}
