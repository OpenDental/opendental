using OpenDental.ReportingComplex;
using OpenDentBusiness;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpDentalSealantMeasure:FormODBase {

		public FormRpDentalSealantMeasure() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRpDentalSealantMeasure_Load(object sender,EventArgs e) {
			textYear.Text=DateTime.Today.Year.ToString();
		}

		private void butOK_Click(object sender,EventArgs e) {
			//create the report
			if(!textYear.IsValid()) {
				MsgBox.Show(this,"Please enter a valid year.");
				return;
			}
			//The query shows, per provider, two numbers with a percentage.  Then there is a totals row at the bottom.
			ReportComplex report=new ReportComplex(true,false);
			DataTable tableMeasures=RpDentalSealantMeasure.GetDentalSealantMeasureTable(textYear.Text);
			DataTable tableTotals=tableMeasures.Clone();
			DataRow totalsRow=tableTotals.NewRow();
			int numerator=0;
			int denominator=0;
			foreach(DataRow row in tableMeasures.Rows) {
				numerator+=PIn.Int(row["Numerator"].ToString());
				denominator+=PIn.Int(row["Denominator"].ToString());
			}
			totalsRow["Provider"]="";
			totalsRow["Numerator"]=numerator.ToString();
			totalsRow["Denominator"]=denominator.ToString();
			if(denominator > 0) {
				totalsRow["Percentage Met"]=(numerator/(double)denominator).ToString("P");
			}
			else {
				totalsRow["Percentage Met"]=0.ToString("P");
			}
			tableTotals.Rows.Add(totalsRow);
			report.ReportName="Dental Sealant Measure";
			report.AddTitle("Title",Lan.g(this,"Dental Sealant Measure"));
			report.AddSubTitle("Date",textYear.Text);
			//setup query
			QueryObject query;
			query=report.AddQuery(tableMeasures,"Measure Count By Provider","",SplitByKind.None,1,true);
			query.AddColumn("Provider",150,FieldValueType.String);
			query.AddColumn("Numerator",150,FieldValueType.Integer);
			query.AddColumn("Denominator",150,FieldValueType.Integer);
			query.AddColumn("Percentage Met",150,FieldValueType.String);
			query.GetColumnDetail("Percentage Met").StringFormat="N2";
			query=report.AddQuery(tableTotals,"Totals","",SplitByKind.None,1,true);
			query.AddColumn("",150,FieldValueType.String);
			query.AddColumn("Total Numerator",150,FieldValueType.Integer);
			query.AddColumn("Total Denominator",150,FieldValueType.Integer);
			query.AddColumn("Total Percentage Met",150,FieldValueType.String);
			query.GetColumnDetail("Total Percentage Met").StringFormat="N2";
			report.AddPageNum();
			// execute query
			if(!report.SubmitQueries()) {
				return;
			}
			// display report
			using FormReportComplex FormR=new FormReportComplex(report);
			//FormR.MyReport=report;
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}