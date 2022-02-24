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
	public partial class FormEhrQualityMeasureEdit2014:FormODBase {
		public QualityMeasure MeasureCur;
		///<summary>Used to pass a patnum back up to the chart module.</summary>
		public long selectedPatNum;

		public FormEhrQualityMeasureEdit2014() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormQualityEdit2014_Load(object sender,EventArgs e) {
			string exceptNALabel="Exceptions.";
			string exceptLabel=exceptNALabel+"  Subtracted from the denominator only if not in the numerator.";
			string exclusNALabel="Exclusions.";
			string exclusLabel=exclusNALabel+"  Subtracted from the denominator before applying numerator criteria.";
			textId.Text=MeasureCur.Id;
			textDescription.Text=MeasureCur.Descript;
			FillGrid();
			textDenominator.Text=MeasureCur.Denominator.ToString();
			textNumerator.Text=MeasureCur.Numerator.ToString();
			//Our 9 measures only have an exclusion or an exception, not both.  Use the same explain text box for either explanation and fill the appropriate text box and label.
			if(MeasureCur.ExclusionsExplain.ToString()=="N/A") {//No exclusions, only exceptions are possible, may be N/A as well
				//top box is exception, bottom box is exclusion, 
				textExclusExceptExplain.Text=MeasureCur.ExceptionsExplain.ToString();
				labelExclusExceptNA.Text=exclusNALabel;
				if(MeasureCur.ExceptionsExplain.ToString()=="N/A") {//also no exceptions
					textExclusExcept.Text="None";
					labelExclusExcept.Text=exceptNALabel;
					textExclusExcept.BackColor=System.Drawing.SystemColors.Control;
				}
				else {
					textExclusExcept.Text=MeasureCur.Exceptions.ToString();
					labelExclusExcept.Text=exceptLabel;
					textExclusExcept.BackColor=System.Drawing.SystemColors.ControlLightLight;
				}
			}
			else {
				//there is a valid exclusion explanation for this measure, top box is exclusion, bottom box is exception
				textExclusExceptExplain.Text=MeasureCur.ExclusionsExplain.ToString();
				textExclusExcept.Text=MeasureCur.Exclusions.ToString();
				textExclusExcept.BackColor=System.Drawing.SystemColors.ControlLightLight;
				labelExclusExcept.Text=exclusLabel;
				labelExclusExceptNA.Text=exceptNALabel;
			}
			textNotMet.Text=MeasureCur.NotMet.ToString();
			//Reporting rate represents the percentage of patients in the denominator who fall into one of the other sub-populations. Rate=(Numerator + Exclusions + Exceptions)/(Denominator)
			textReportingRate.Text=MeasureCur.ReportingRate.ToString()+"%";
			textPerformanceRate.Text=MeasureCur.Numerator.ToString()+"/"+(MeasureCur.Numerator+MeasureCur.NotMet).ToString()
					+"  = "+MeasureCur.PerformanceRate.ToString()+"%";
			textDenominatorExplain.Text=MeasureCur.DenominatorExplain;
			textNumeratorExplain.Text=MeasureCur.NumeratorExplain;
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("PatNum",50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Patient Name",140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Numerator",65,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Exclusion",60,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Exception",60,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Explanation",140);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			if(MeasureCur.Type2014==QualityType2014.MedicationsEntered) {
				foreach(KeyValuePair<long,List<EhrCqmEncounter>> kvpair in MeasureCur.DictPatNumListEncounters) {
					for(int i=0;i<kvpair.Value.Count;i++) {
						row=new GridRow();
						row.Cells.Add(kvpair.Key.ToString());
						row.Cells.Add(Patients.GetPat(kvpair.Key).GetNameLF());
						string categoryCur="";
						if(kvpair.Value[i].IsNumerator) {
							categoryCur="X";
						}
						row.Cells.Add(categoryCur);
						categoryCur="";
						row.Cells.Add(categoryCur);//no exclusions for this measure
						if(kvpair.Value[i].IsException) {
							categoryCur="X";
						}
						row.Cells.Add(categoryCur);
						row.Cells.Add(kvpair.Value[i].Explanation);
						row.Tag=kvpair.Key.ToString();
						gridMain.ListGridRows.Add(row);
					}
				}
			}
			else {
				for(int i=0;i<MeasureCur.ListEhrPats.Count;i++) {
					if(!MeasureCur.ListEhrPats[i].IsDenominator) {
						continue;
					}
					row=new GridRow();
					row.Cells.Add(MeasureCur.ListEhrPats[i].EhrCqmPat.PatNum.ToString());
					row.Cells.Add(MeasureCur.ListEhrPats[i].EhrCqmPat.GetNameLF());
					string categoryCur="";
					if(MeasureCur.ListEhrPats[i].IsNumerator) {
						categoryCur="X";
					}
					row.Cells.Add(categoryCur);
					categoryCur="";
					if(MeasureCur.ListEhrPats[i].IsExclusion) {
						categoryCur="X";
					}
					row.Cells.Add(categoryCur);
					categoryCur="";
					if(MeasureCur.ListEhrPats[i].IsException) {
						categoryCur="X";
					}
					row.Cells.Add(categoryCur);
					row.Cells.Add(MeasureCur.ListEhrPats[i].Explanation);
					row.Tag=MeasureCur.ListEhrPats[i].EhrCqmPat.PatNum.ToString();
					gridMain.ListGridRows.Add(row);
				}
			}
			gridMain.EndUpdate();
		}

		private void gotoPatientRecordToolStripMenuItem_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				return;
			}
			selectedPatNum=0;
			try {
				selectedPatNum=PIn.Long(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag.ToString());
			}
			catch { }
			if(selectedPatNum==0) {
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}

	

		

		

	}
}
