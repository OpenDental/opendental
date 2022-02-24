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
	public partial class FormEhrQualityMeasureEdit:FormODBase {
		public QualityMeasure Qcur;
		private DataTable table;
		public DateTime DateStart;
		public DateTime DateEnd;
		public long ProvNum;

		public FormEhrQualityMeasureEdit() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormQualityEdit_Load(object sender,EventArgs e) {
			textId.Text=Qcur.Id;
			textDescription.Text=Qcur.Descript;
			FillGrid();
			textDenominator.Text=Qcur.Denominator.ToString();
			textNumerator.Text=Qcur.Numerator.ToString();
			textExclusions.Text=Qcur.Exclusions.ToString();
			textNotMet.Text=Qcur.NotMet.ToString();
			textReportingRate.Text=Qcur.ReportingRate.ToString()+"%";
			textPerformanceRate.Text=Qcur.Numerator.ToString()+"/"+(Qcur.Numerator+Qcur.NotMet).ToString()
					+"  = "+Qcur.PerformanceRate.ToString()+"%";
			textDenominatorExplain.Text=Qcur.DenominatorExplain;
			textNumeratorExplain.Text=Qcur.NumeratorExplain;
			textExclusionsExplain.Text=Qcur.ExclusionsExplain;
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("PatNum",50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Patient Name",140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Numerator",60,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Exclusion",60,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Explanation",200);
			gridMain.ListGridColumns.Add(col);
			table=QualityMeasures.GetTable(Qcur.Type,DateStart,DateEnd,ProvNum);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<table.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["PatNum"].ToString());
				row.Cells.Add(table.Rows[i]["patientName"].ToString());
				row.Cells.Add(table.Rows[i]["numerator"].ToString());
				row.Cells.Add(table.Rows[i]["exclusion"].ToString());
				row.Cells.Add(table.Rows[i]["explanation"].ToString());
				//if(table.Rows[i]["met"].ToString()=="X") {
				//	row.ColorBackG=Color.LightGreen;
				//}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}

	

		

		

	}
}
