using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using OpenDental;

namespace OpenDental {
	public partial class FormEhrNotPerformed:FormODBase {
		private List<EhrNotPerformed> listNotPerf;
		public Patient PatCur;

		public FormEhrNotPerformed() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormEhrNotPerformed_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Date",70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Prov",50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Item Not Performed",130);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Code",102);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Code Description",150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Reason Code",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Reason Description",150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Note",150);
			gridMain.ListGridColumns.Add(col);
			listNotPerf=EhrNotPerformeds.Refresh(PatCur.PatNum);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listNotPerf.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listNotPerf[i].DateEntry.ToShortDateString());
				row.Cells.Add(Providers.GetAbbr(listNotPerf[i].ProvNum));
				//Item not performed------------------------------------------------------------
				switch(listNotPerf[i].CodeValue) {
					case "39156-5"://BMI exam
						row.Cells.Add(EhrNotPerformedItem.BMIExam.ToString());
						break;
					case "428191000124101"://CurrentMedsDocumented
						row.Cells.Add(EhrNotPerformedItem.DocumentCurrentMeds.ToString());
						break;
					case "11366-2"://History of tobacco use Narrative
					case "68535-4"://Have you used tobacco in the last 30 days
					case "68536-2"://Have you used smokeless tobacco in last 30 days
						row.Cells.Add(EhrNotPerformedItem.TobaccoScreening.ToString());
						break;
					default://We will default to Influenza Vaccine, there are 26 codes, for this item
						row.Cells.Add(EhrNotPerformedItem.InfluenzaVaccination.ToString());
						break;
				}
				//Code not performed------------------------------------------------------------
				row.Cells.Add(listNotPerf[i].CodeValue+" ("+listNotPerf[i].CodeSystem+")");
				//Description of code not performed---------------------------------------------
				string descript="";
				//to get description, first determine which table the code is from.  EhrNotPerformed is allowed to be CPT, CVX, LOINC, SNOMEDCT.
				switch(listNotPerf[i].CodeSystem) {
					case "CPT":
						Cpt cptCur=Cpts.GetByCode(listNotPerf[i].CodeValue);
						if(cptCur!=null) {
							descript=cptCur.Description;
						}
						break;
					case "CVX":
						Cvx cvxCur=Cvxs.GetOneFromDb(listNotPerf[i].CodeValue);
						if(cvxCur!=null) {
							descript=cvxCur.Description;
						}
						break;
					case "LOINC":
						Loinc lCur=Loincs.GetByCode(listNotPerf[i].CodeValue);
						if(lCur!=null) {
							descript=lCur.NameLongCommon;
						}
						break;
					case "SNOMEDCT":
						Snomed sCur=Snomeds.GetByCode(listNotPerf[i].CodeValue);
						if(sCur!=null) {
							descript=sCur.Description;
						}
						break;
				}
				row.Cells.Add(descript);
				//Reason Code-------------------------------------------------------------------
				row.Cells.Add(listNotPerf[i].CodeValueReason+" ("+listNotPerf[i].CodeSystemReason+")");
				//Reason Description------------------------------------------------------------
				descript="";
				if(listNotPerf[i].CodeValueReason!="") {
					//reason codes are only allowed to be SNOMEDCT codes
					Snomed sCur=Snomeds.GetByCode(listNotPerf[i].CodeValueReason);
					if(sCur!=null) {
						descript=sCur.Description;
					}
				}
				row.Cells.Add(descript);
				//Note--------------------------------------------------------------------------
				row.Cells.Add(listNotPerf[i].Note);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEhrNotPerformedEdit FormEE=new FormEhrNotPerformedEdit();
			FormEE.EhrNotPerfCur=listNotPerf[e.Row];
			FormEE.SelectedItemIndex=(int)Enum.Parse(typeof(EhrNotPerformedItem),gridMain.ListGridRows[e.Row].Cells[2].Text,true);//Parse the text displayed from the enum and convert to int
			FormEE.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			List<string> listItems=new List<string>();
			for(int i=0;i<Enum.GetNames(typeof(EhrNotPerformedItem)).Length;i++) {
				listItems.Add(Enum.GetNames(typeof(EhrNotPerformedItem))[i]);
			}
			using InputBox chooseItem=new InputBox(Lan.g(this,"Select the item not being performed from the list below."),listItems);
			if(chooseItem.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(chooseItem.comboSelection.SelectedIndex==-1) {
				MsgBox.Show(this,"You must select an item that is not being performed from the list of available items.");
				return;
			}
			using FormEhrNotPerformedEdit FormEE=new FormEhrNotPerformedEdit();
			FormEE.EhrNotPerfCur=new EhrNotPerformed();
			FormEE.EhrNotPerfCur.IsNew=true;
			FormEE.EhrNotPerfCur.PatNum=PatCur.PatNum;
			FormEE.EhrNotPerfCur.ProvNum=PatCur.PriProv;
			FormEE.EhrNotPerfCur.DateEntry=DateTime.Now;
			FormEE.SelectedItemIndex=chooseItem.comboSelection.SelectedIndex;//Send in the int of index of selected item
			FormEE.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}
	}
}