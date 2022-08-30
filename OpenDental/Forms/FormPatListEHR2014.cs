using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormPatListEHR2014:FormODBase {
		private List<EhrPatListElement2014> _listEhrPatListElement2014s;

		public FormPatListEHR2014() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormPatListEHR2014_Load(object sender,EventArgs e) {
			_listEhrPatListElement2014s=new List<EhrPatListElement2014>();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn("Restriction",70);
			gridMain.Columns.Add(col);
			col=new GridColumn("Compare string",120);
			gridMain.Columns.Add(col);
			col=new GridColumn("Operand",120);
			gridMain.Columns.Add(col);
			col=new GridColumn("Lab value",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("After Date",120);
			gridMain.Columns.Add(col);
			col=new GridColumn("Before Date",120);
			gridMain.Columns.Add(col);
			//col=new ODGridColumn("Order",120,HorizontalAlignment.Center);
			//gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listEhrPatListElement2014s.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listEhrPatListElement2014s[i].Restriction.ToString());
				if(_listEhrPatListElement2014s[i].Restriction==EhrRestrictionType.Problem) {
					if(Snomeds.CodeExists(_listEhrPatListElement2014s[i].CompareString)) {
						row.Cells.Add(_listEhrPatListElement2014s[i].CompareString+" - "+Snomeds.GetByCode(_listEhrPatListElement2014s[i].CompareString).Description);
					}
					else {
						row.Cells.Add(_listEhrPatListElement2014s[i].CompareString+" - NON-SNOMED CT CODE");
					}
				}
				else {
					row.Cells.Add(_listEhrPatListElement2014s[i].CompareString);
				}
				if(_listEhrPatListElement2014s[i].Restriction==EhrRestrictionType.Gender
					|| _listEhrPatListElement2014s[i].Restriction==EhrRestrictionType.Problem
					|| _listEhrPatListElement2014s[i].Restriction==EhrRestrictionType.Medication
					|| _listEhrPatListElement2014s[i].Restriction==EhrRestrictionType.CommPref
					|| _listEhrPatListElement2014s[i].Restriction==EhrRestrictionType.Allergy) 
				{
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listEhrPatListElement2014s[i].Operand.ToString());
				}
				row.Cells.Add(_listEhrPatListElement2014s[i].LabValue);
				if(_listEhrPatListElement2014s[i].StartDate.Year>1880) {
					row.Cells.Add(_listEhrPatListElement2014s[i].StartDate.ToShortDateString());
				}
				else {
					row.Cells.Add("");
				}
				if(_listEhrPatListElement2014s[i].EndDate.Year>1880) {
					row.Cells.Add(_listEhrPatListElement2014s[i].EndDate.ToShortDateString());
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
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void AddElement(EhrRestrictionType ehrRestrictionType) {
			using FormPatListElementEditEHR2014 formPatListElementEditEHR2014=new FormPatListElementEditEHR2014();
			formPatListElementEditEHR2014.EhrPatListElement2014Cur=new EhrPatListElement2014();
			formPatListElementEditEHR2014.EhrPatListElement2014Cur.Restriction=ehrRestrictionType;
			formPatListElementEditEHR2014.IsNew=true;
			formPatListElementEditEHR2014.ShowDialog();
			if(formPatListElementEditEHR2014.DialogResult==DialogResult.OK) {
				_listEhrPatListElement2014s.Add(formPatListElementEditEHR2014.EhrPatListElement2014Cur);
			}
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			int index=gridMain.GetSelectedIndex();
			if(index==-1) {
				MessageBox.Show("Please select a data element first.");
				return;
			}
			using FormPatListElementEditEHR2014 formPatListElementEditEHR2014=new FormPatListElementEditEHR2014();
			formPatListElementEditEHR2014.EhrPatListElement2014Cur=_listEhrPatListElement2014s[index];
			formPatListElementEditEHR2014.ShowDialog();
			if(formPatListElementEditEHR2014.DialogResult==DialogResult.Cancel && formPatListElementEditEHR2014.DoDelete) {
				_listEhrPatListElement2014s.Remove(_listEhrPatListElement2014s[index]);
			}
			FillGrid();
		}

		private void butResults_Click(object sender,EventArgs e) {
			if(gridMain.ListGridRows.Count<1) {
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
			using FormPatListResultsEHR2014 formPatListResultsEHR2014=new FormPatListResultsEHR2014(_listEhrPatListElement2014s);
			formPatListResultsEHR2014.ShowDialog();
		}

		private void butClear_Click(object sender,EventArgs e) {
			_listEhrPatListElement2014s.Clear();
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
