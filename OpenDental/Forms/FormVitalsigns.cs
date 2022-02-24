using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormVitalsigns:FormODBase {
		private List<Vitalsign> listVs;
		public long PatNum;

		public FormVitalsigns() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormVitalsigns_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Date",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Pulse",55);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Height",55);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Weight",55);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("BP",55);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("BMI",55);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Documentation for Followup or Ineligible",150);
			gridMain.ListGridColumns.Add(col);
			listVs=Vitalsigns.Refresh(PatNum);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listVs.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listVs[i].DateTaken.ToShortDateString());
				row.Cells.Add(listVs[i].Pulse.ToString()+" bpm");
				row.Cells.Add(listVs[i].Height.ToString()+" in.");
				row.Cells.Add(listVs[i].Weight.ToString()+" lbs.");
				row.Cells.Add(listVs[i].BpSystolic.ToString()+"/"+listVs[i].BpDiastolic.ToString());
				//BMI = (lbs*703)/(in^2)
				float bmi=Vitalsigns.CalcBMI(listVs[i].Weight,listVs[i].Height);
				if(bmi!=0) {
					row.Cells.Add(bmi.ToString("n1"));
				}
				else {//leave cell blank because there is not a valid bmi
					row.Cells.Add("");
				}
				row.Cells.Add(listVs[i].Documentation);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			long vitalNum=listVs[e.Row].VitalsignNum;
			//change for EHR 2014
			using FormVitalsignEdit2014 FormVSE=new FormVitalsignEdit2014();
			//FormEhrVitalsignEdit FormVSE=new FormEhrVitalsignEdit();
			FormVSE.VitalsignCur=Vitalsigns.GetOne(vitalNum);
			FormVSE.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			//change for EHR 2014
			using FormVitalsignEdit2014 FormVSE=new FormVitalsignEdit2014();
			//FormEhrVitalsignEdit FormVSE=new FormEhrVitalsignEdit();
			FormVSE.VitalsignCur=new Vitalsign();
			FormVSE.VitalsignCur.PatNum=PatNum;
			FormVSE.VitalsignCur.DateTaken=DateTime.Today;
			FormVSE.VitalsignCur.IsNew=true;
			FormVSE.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}

		///<summary>Hidden if BPOnly vital sign measure.</summary>
		private void butGrowthChart_Click(object sender,EventArgs e) {
			using FormEhrGrowthCharts FormGC=new FormEhrGrowthCharts();
			FormGC.PatNum=PatNum;
			FormGC.ShowDialog();
		}


	}
}
