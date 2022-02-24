using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Drawing.Printing;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEhrLabPanelEdit:FormODBase {
		public List<LabResult> listResults;
		public LabPanel PanelCur;
		public bool IsNew;

		public FormEhrLabPanelEdit() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormLabPanelEdit_Load(object sender,EventArgs e) {
			textRawMsg.Text=PanelCur.RawMessage;
			Patient pat=Patients.GetLim(PanelCur.PatNum);
			textName.Text=pat.GetNameFL();
			textServiceID.Text=PanelCur.ServiceId;
			textServiceName.Text=PanelCur.ServiceName;
			textLabName.Text=PanelCur.LabNameAddress;
			textSpecimenCondition.Text=PanelCur.SpecimenCondition;
			if(PanelCur.SpecimenSource!="") {
				string[] components=PanelCur.SpecimenSource.Split('&');
				if(components.Length==3) {
					textSpecimenSourceCode.Text=components[0];
					textSpecimenLocation.Text=components[1];
				}
			}
			FillGrid();
		}
	
		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Test Date",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("LOINC",75);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Test Performed",130);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("ResultVal",60);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Units",45);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Range",55);
			gridMain.ListGridColumns.Add(col);
			listResults = LabResults.GetForPanel(PanelCur.LabPanelNum);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listResults.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listResults[i].DateTimeTest.ToShortDateString());
				row.Cells.Add(listResults[i].TestID);
				row.Cells.Add(listResults[i].TestName);
				row.Cells.Add(listResults[i].ObsValue);
				row.Cells.Add(listResults[i].ObsUnits);
				row.Cells.Add(listResults[i].ObsRange);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEhrLabResultEdit FormLRE=new FormEhrLabResultEdit();
			FormLRE.LabCur = listResults[e.Row];
			FormLRE.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(IsNew) {
				MessageBox.Show("Lab results can only be added to saved or existing lab panels.");
				return;
			}
			using FormEhrLabResultEdit FormLRE=new FormEhrLabResultEdit();
			FormLRE.IsNew=true;
			FormLRE.LabCur=new LabResult();
			FormLRE.LabCur.LabPanelNum=PanelCur.LabPanelNum;
			FormLRE.LabCur.DateTimeTest=DateTime.Now;
			FormLRE.ShowDialog();
			FillGrid();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(MessageBox.Show("Delete?","",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
				return;
			}
			LabPanels.Delete(PanelCur.LabPanelNum);
			LabResults.DeleteForPanel(PanelCur.LabPanelNum);
			DialogResult=DialogResult.OK;
		}

		private void butOk_Click(object sender,EventArgs e) {
			if(textSpecimenSourceCode.Text!="" && textSpecimenLocation.Text=="") {
				MessageBox.Show("If specimen code is entered, then specimen location must be entered."); 
				return;
			}
			if(textSpecimenSourceCode.Text=="" && textSpecimenLocation.Text!="") {
				MessageBox.Show("If specimen location is entered, then specimen code must be entered.");
				return;
			}
			PanelCur.ServiceId=textServiceID.Text;
			PanelCur.ServiceName=textServiceName.Text;
			PanelCur.LabNameAddress=textLabName.Text;
			PanelCur.SpecimenCondition=textSpecimenCondition.Text;
			if(textSpecimenSourceCode.Text=="") {
				PanelCur.SpecimenSource="";
			}
			else {
				PanelCur.SpecimenSource=textSpecimenSourceCode.Text+"&"+textSpecimenLocation.Text+"&HL70070";
			}
			if(IsNew) {
				LabPanels.Insert(PanelCur);
			}
			else {
				LabPanels.Update(PanelCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

	}
}
