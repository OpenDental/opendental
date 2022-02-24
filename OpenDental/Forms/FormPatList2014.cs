using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormPatList2014:Form {
		public List<EhrPatListElement2014> ElementList;

		public FormPatList2014() {
			InitializeComponent();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			ODGridColumn col;
			col=new ODGridColumn("Restriction",70);
			gridMain.Columns.Add(col);
			col=new ODGridColumn("Compare string",120);
			gridMain.Columns.Add(col);
			col=new ODGridColumn("Operand",120);
			gridMain.Columns.Add(col);
			col=new ODGridColumn("Lab value",80);
			gridMain.Columns.Add(col);
			col=new ODGridColumn("After Date",120);
			gridMain.Columns.Add(col);
			col=new ODGridColumn("Before Date",120);
			gridMain.Columns.Add(col);
			//col=new ODGridColumn("Order",120,HorizontalAlignment.Center);
			//gridMain.Columns.Add(col);
			gridMain.Rows.Clear();
			ODGridRow row;
			for(int i=0;i<ElementList.Count;i++) {
				row=new ODGridRow();
				row.Cells.Add(ElementList[i].Restriction.ToString());
				row.Cells.Add(ElementList[i].CompareString);
				if(ElementList[i].Restriction==EhrRestrictionType.Gender
					|| ElementList[i].Restriction==EhrRestrictionType.Problem
					|| ElementList[i].Restriction==EhrRestrictionType.Medication
					|| ElementList[i].Restriction==EhrRestrictionType.CommPref
					|| ElementList[i].Restriction==EhrRestrictionType.Allergy) 
				{
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(ElementList[i].Operand.ToString());
				}
				row.Cells.Add(ElementList[i].LabValue);
				if(ElementList[i].StartDate.Year>1880) {
					row.Cells.Add(ElementList[i].StartDate.ToShortDateString());
				}
				else {
					row.Cells.Add("");
				}
				if(ElementList[i].EndDate.Year>1880) {
					row.Cells.Add(ElementList[i].EndDate.ToShortDateString());
				}
				else {
					row.Cells.Add("");
				}
				//if(ElementList[i].OrderBy) {
				//  row.Cells.Add("X");
				//}
				//else {
				//  row.Cells.Add("");
				//}
				gridMain.Rows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void AddElement(EhrRestrictionType restriction) {
			FormPatListElementEdit2014 FormPLEE=new FormPatListElementEdit2014();
			FormPLEE.Element=new EhrPatListElement2014();
			FormPLEE.Element.Restriction=restriction;
			FormPLEE.IsNew=true;
			FormPLEE.ShowDialog();
			if(FormPLEE.DialogResult==DialogResult.OK) {
				ElementList.Add(FormPLEE.Element);
			}
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			int index=gridMain.GetSelectedIndex();
			if(index==-1) {
				MessageBox.Show("Please select a data element first.");
				return;
			}
			FormPatListElementEdit2014 FormPLEE=new FormPatListElementEdit2014();
			FormPLEE.Element=ElementList[index];
			FormPLEE.ShowDialog();
			if(FormPLEE.DialogResult==DialogResult.Cancel && FormPLEE.Delete) {
				ElementList.Remove(ElementList[index]);
			}
			FillGrid();
		}

		private void butResults_Click(object sender,EventArgs e) {
			if(gridMain.Rows.Count<1) {
				MessageBox.Show(Lans.g(this,"Please add a data element."));
				return;
			}
			//bool hasOrder=false;
			//for(int i=0;i<ElementList.Count;i++) {
			//  if(hasOrder && ElementList[i].OrderBy) {
			//    MessageBox.Show(Lans.g(this,"You can only 'Order By' exactly one data element."));
			//    return;
			//  }
			//  if(ElementList[i].OrderBy) {
			//    hasOrder=true;
			//  }
			//}
			FormPatListResults2014 FormPLR14=new FormPatListResults2014(ElementList);
			FormPLR14.ShowDialog();
		}

		private void butClear_Click(object sender,EventArgs e) {
			ElementList.Clear();
			FillGrid();
		}

		private void butBirthdate_Click(object sender,EventArgs e) {
			AddElement(EhrRestrictionType.Birthdate);
		}

		private void butDisease_Click(object sender,EventArgs e) {
			AddElement(EhrRestrictionType.Problem);
		}

		private void butMedication_Click(object sender,EventArgs e) {
			AddElement(EhrRestrictionType.Medication);
		}

		private void butLabResult_Click(object sender,EventArgs e) {
			AddElement(EhrRestrictionType.LabResult);
		}

		private void butGender_Click(object sender,EventArgs e) {
			AddElement(EhrRestrictionType.Gender);
		}

		private void butAllergy_Click(object sender,EventArgs e) {
			AddElement(EhrRestrictionType.Allergy);
		}

		private void butCommPref_Click(object sender,EventArgs e) {
			AddElement(EhrRestrictionType.CommPref);
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
