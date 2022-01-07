using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormLabCaseSelect : FormODBase {
		public long PatNum;
		///<summary>This only has a value when DialogResult=OK.</summary>
		public long SelectedLabCaseNum;
		private List<LabCase> labCaseList;
		public bool IsPlanned;

		///<summary></summary>
		public FormLabCaseSelect()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormLabCaseSelect_Load(object sender, System.EventArgs e) {
			FillGrid();
			if(labCaseList.Count>0){
				gridMain.SetSelected(0,true);
			}
		}

		private void FillGrid(){
			labCaseList=LabCases.GetForPat(PatNum,IsPlanned);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableLabCaseSelect","Date Created"),80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableLabCaseSelect","Lab"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableLabCaseSelect","Phone"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableLabCaseSelect","Instructions"),200);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			DateTime dateCreated;
			Laboratory lab;
			foreach(LabCase labCase in labCaseList) { 
				row=new GridRow();
				dateCreated=labCase.DateTimeCreated;
				row.Cells.Add(dateCreated.ToString("ddd")+" "+dateCreated.ToShortDateString()+" "+dateCreated.ToShortTimeString());
				lab=Laboratories.GetOne(labCase.LaboratoryNum);
				if(lab==null) {//Lab wasn't found in the db, but we only require the LabCaseNum later.
					row.Cells.Add(Lan.g(this,"Lab Not Found"));
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(lab.Description);
					row.Cells.Add(lab.Phone);
				}				
				row.Cells.Add(labCase.Instructions);
				row.Tag=labCase;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			LabCase lab=new LabCase();
			lab.PatNum=PatNum;
			Patient pat=Patients.GetPat(PatNum);
			lab.ProvNum=Patients.GetProvNum(pat);
			lab.DateTimeCreated=MiscData.GetNowDateTime();
			LabCases.Insert(lab);
			using FormLabCaseEdit FormL=new FormLabCaseEdit();
			FormL.CaseCur=lab;
			FormL.IsNew=true;
			FormL.ShowDialog();
			if(FormL.DialogResult!=DialogResult.OK){
				return;
			}
			SelectedLabCaseNum=FormL.CaseCur.LabCaseNum;
			DialogResult=DialogResult.OK;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SelectedLabCaseNum=gridMain.SelectedTag<LabCase>().LabCaseNum;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			SelectedLabCaseNum=gridMain.SelectedTag<LabCase>().LabCaseNum;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	

		

		

		

		


	}
}





















