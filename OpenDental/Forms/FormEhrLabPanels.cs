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
using CodeBase;

namespace OpenDental {
	public partial class FormEhrLabPanels:FormODBase {
		private List<LabPanel> listLP;
		public Patient PatCur;
		public bool IsSelectionMode;
		public long SelectedLabPanelNum;

		public FormEhrLabPanels() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormLabPanels_Load(object sender,EventArgs e) {
			if(!IsSelectionMode) {
				butOK.Visible=false;
				butCancel.Text="Close";
			}
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("Date Time",135);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Service",200);
			gridMain.ListGridColumns.Add(col);
			listLP = LabPanels.Refresh(PatCur.PatNum);
			List<LabResult> listResults;
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listLP.Count;i++) {
				row=new GridRow();
				listResults=LabResults.GetForPanel(listLP[i].LabPanelNum);
				if(listResults.Count==0) {
					row.Cells.Add(" ");//to avoid a very short row
				}
				else {
					row.Cells.Add(listResults[0].DateTimeTest.ToString());
				}
				row.Cells.Add(listLP[i].ServiceName);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEhrLabPanelEdit FormLPE=new FormEhrLabPanelEdit();
			FormLPE.PanelCur = listLP[e.Row];
			FormLPE.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormEhrLabPanelEdit FormLPE=new FormEhrLabPanelEdit();
			FormLPE.IsNew=true;
			FormLPE.PanelCur=new LabPanel();
			FormLPE.PanelCur.PatNum=PatCur.PatNum;
			FormLPE.PanelCur.SpecimenSource="";
			FormLPE.ShowDialog();
			FillGrid();
		}

		private void butSubmit_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MessageBox.Show("Please select lab panels first.");
				return;
			}
			List<LabPanel> panels=new List<LabPanel>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				panels.Add(listLP[gridMain.SelectedIndices[i]]);
			}
			OpenDentBusiness.HL7.EhrORU oru=new OpenDentBusiness.HL7.EhrORU();
			Cursor=Cursors.WaitCursor;
			try {
				oru.Initialize(panels);
			}
			catch(ApplicationException ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				return;
			}
			string outputStr=oru.GenerateMessage();
			try {
				EmailMessages.SendTestUnsecure("Public Health","oru.txt",outputStr);
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				return;
			}
			Cursor=Cursors.Default;
			MessageBox.Show("Sent");
		}

		private void butShow_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MessageBox.Show("Please select lab panels first.");
				return;
			}
			List<LabPanel> panels=new List<LabPanel>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				panels.Add(listLP[gridMain.SelectedIndices[i]]);
			}
			OpenDentBusiness.HL7.EhrORU oru=new OpenDentBusiness.HL7.EhrORU();
			Cursor=Cursors.WaitCursor;
			try {
				oru.Initialize(panels);
			}
			catch(ApplicationException ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				return;
			}
			string outputStr=oru.GenerateMessage();
			Cursor=Cursors.Default;
			using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(outputStr);
			msgbox.ShowDialog();
		}

		private void butOK_Click(object sender,EventArgs e) {
			//not visible unless in selectionMode
			if(gridMain.SelectedIndices.Length!=1) {
				MessageBox.Show("Please select exactly one lab panel.");
				return;
			}
			SelectedLabPanelNum=listLP[gridMain.SelectedIndices[0]].LabPanelNum;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		



	}
}
