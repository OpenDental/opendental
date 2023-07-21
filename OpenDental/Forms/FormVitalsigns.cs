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
		private List<Vitalsign> _listVitalsigns;
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
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("Date",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("Pulse",55);
			gridMain.Columns.Add(col);
			col=new GridColumn("Height",55);
			gridMain.Columns.Add(col);
			col=new GridColumn("Weight",55);
			gridMain.Columns.Add(col);
			col=new GridColumn("BP",55);
			gridMain.Columns.Add(col);
			col=new GridColumn("BMI",55);
			gridMain.Columns.Add(col);
			col=new GridColumn("Documentation for Followup or Ineligible",150);
			gridMain.Columns.Add(col);
			_listVitalsigns=Vitalsigns.Refresh(PatNum);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listVitalsigns.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listVitalsigns[i].DateTaken.ToShortDateString());
				row.Cells.Add(_listVitalsigns[i].Pulse.ToString()+" bpm");
				row.Cells.Add(_listVitalsigns[i].Height.ToString()+" in.");
				row.Cells.Add(_listVitalsigns[i].Weight.ToString()+" lbs.");
				row.Cells.Add(_listVitalsigns[i].BpSystolic.ToString()+"/"+_listVitalsigns[i].BpDiastolic.ToString());
				//BMI = (lbs*703)/(in^2)
				float bmi=Vitalsigns.CalcBMI(_listVitalsigns[i].Weight,_listVitalsigns[i].Height);
				if(bmi!=0) {
					row.Cells.Add(bmi.ToString("n1"));
				}
				else {//leave cell blank because there is not a valid bmi
					row.Cells.Add("");
				}
				row.Cells.Add(_listVitalsigns[i].Documentation);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			long vitalsignNum=_listVitalsigns[e.Row].VitalsignNum;
			//change for EHR 2014
			using FormVitalsignEdit2014 formVitalsignEdit2014=new FormVitalsignEdit2014();
			//FormEhrVitalsignEdit FormVSE=new FormEhrVitalsignEdit();
			formVitalsignEdit2014.VitalsignCur=Vitalsigns.GetOne(vitalsignNum);
			formVitalsignEdit2014.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			//change for EHR 2014
			using FormVitalsignEdit2014 formVitalsignEdit2014=new FormVitalsignEdit2014();
			//FormEhrVitalsignEdit FormVSE=new FormEhrVitalsignEdit();
			formVitalsignEdit2014.VitalsignCur=new Vitalsign();
			formVitalsignEdit2014.VitalsignCur.PatNum=PatNum;
			formVitalsignEdit2014.VitalsignCur.DateTaken=DateTime.Today;
			formVitalsignEdit2014.VitalsignCur.IsNew=true;
			formVitalsignEdit2014.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}

		///<summary>Hidden if BPOnly vital sign measure.</summary>
		private void butGrowthChart_Click(object sender,EventArgs e) {
			using FormEhrGrowthCharts formEhrGrowthCharts=new FormEhrGrowthCharts();
			formEhrGrowthCharts.PatNum=PatNum;
			formEhrGrowthCharts.ShowDialog();
		}


	}
}
