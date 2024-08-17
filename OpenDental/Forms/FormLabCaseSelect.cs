using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormLabCaseSelect : FormODBase {
		public long PatNum;
		///<summary>This only has a value when DialogResult=OK.</summary>
		public long LabCaseNumSelected;
		private List<LabCase> _listLabCases;
		public bool IsPlanned;

		///<summary></summary>
		public FormLabCaseSelect(){
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormLabCaseSelect_Load(object sender, System.EventArgs e) {
			FillGrid();
			if(_listLabCases.Count>0){
				gridMain.SetSelected(0,true);
			}
		}

		private void FillGrid(){
			_listLabCases=LabCases.GetForPat(PatNum,IsPlanned);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableLabCaseSelect","Date Created"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableLabCaseSelect","Lab"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableLabCaseSelect","Phone"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableLabCaseSelect","Instructions"),200);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			DateTime dateCreated;
			Laboratory laboratory;
			for(int i=0;i<_listLabCases.Count;i++){
				row=new GridRow();
				dateCreated=_listLabCases[i].DateTimeCreated;
				row.Cells.Add(dateCreated.ToString("ddd")+" "+dateCreated.ToShortDateString()+" "+dateCreated.ToShortTimeString());
				laboratory=Laboratories.GetOne(_listLabCases[i].LaboratoryNum);
				if(laboratory==null) {//Lab wasn't found in the db, but we only require the LabCaseNum later.
					row.Cells.Add(Lan.g(this,"Lab Not Found"));
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(laboratory.Description);
					row.Cells.Add(laboratory.Phone);
				}
				row.Cells.Add(_listLabCases[i].Instructions);
				row.Tag=_listLabCases[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			LabCase labCase=new LabCase();
			labCase.PatNum=PatNum;
			Patient patient=Patients.GetPat(PatNum);
			labCase.ProvNum=Patients.GetProvNum(patient);
			labCase.DateTimeCreated=MiscData.GetNowDateTime();
			LabCases.Insert(labCase);
			using FormLabCaseEdit formLabCaseEdit=new FormLabCaseEdit();
			formLabCaseEdit.LabCaseCur=labCase;
			formLabCaseEdit.IsNew=true;
			formLabCaseEdit.ShowDialog();
			if(formLabCaseEdit.DialogResult!=DialogResult.OK){
				return;
			}
			LabCaseNumSelected=formLabCaseEdit.LabCaseCur.LabCaseNum;
			DialogResult=DialogResult.OK;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			LabCaseNumSelected=gridMain.SelectedTag<LabCase>().LabCaseNum;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			LabCaseNumSelected=gridMain.SelectedTag<LabCase>().LabCaseNum;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	

		

		

		

		


	}
}





















