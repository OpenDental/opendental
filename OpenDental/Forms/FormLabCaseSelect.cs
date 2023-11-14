using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormLabCaseSelect : FormODBase {
		public long PatNum;
		///<summary>This only has a value when DialogResult=OK.</summary>
		public List<long> ListLabCaseNumsSelected;
		public bool IsPlanned;
		public bool IsSelectingUnattached;
		public long AptNum;
		private List<LabCase> _listLabCases;

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
			if(IsSelectingUnattached) {
				labelInfo.Text="Select a lab case from the list below or create a new one.  This list will not show lab cases that are already attached to other appointments.";
			}
			else {
				Text="Lab Cases for Appointment";
				labelInfo.Text="";
			}
			FillGrid();
			if(_listLabCases.Count>0){
				gridMain.SetSelected(0,true);
			}
		}

		private void FillGrid(){
			if(IsSelectingUnattached) {
				_listLabCases=LabCases.GetForPat(PatNum,IsPlanned);
			}
			else if(IsPlanned) {
				_listLabCases=LabCases.GetForPlanned(AptNum);
			}
			else {
				_listLabCases=LabCases.GetForApt(AptNum);
			}
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
			if(IsSelectingUnattached) {
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
				ListLabCaseNumsSelected=new List<long>() { formLabCaseEdit.LabCaseCur.LabCaseNum };
				DialogResult=DialogResult.OK;
			}
			else {
				//so let user pick one to add
				using FormLabCaseSelect formLabCaseSelect=new FormLabCaseSelect();
				formLabCaseSelect.PatNum=PatNum;
				formLabCaseSelect.IsPlanned=IsPlanned;
				formLabCaseSelect.IsSelectingUnattached=true;
				formLabCaseSelect.ShowDialog();
				if(formLabCaseSelect.DialogResult!=DialogResult.OK) {
					return;
				}
				if(IsPlanned) {
					LabCases.AttachToPlannedAppt(formLabCaseSelect.ListLabCaseNumsSelected,AptNum);
				}
				else {
					LabCases.AttachToAppt(formLabCaseSelect.ListLabCaseNumsSelected,AptNum);
				}
				FillGrid();
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			LabCase labCaseSelected=gridMain.SelectedTag<LabCase>();
			if(IsSelectingUnattached) {
				ListLabCaseNumsSelected=new List<long>() { labCaseSelected.LabCaseNum };
				DialogResult=DialogResult.OK;
			}
			else {
				using FormLabCaseEdit formLabCaseEdit=new FormLabCaseEdit();
				formLabCaseEdit.LabCaseCur=labCaseSelected;
				formLabCaseEdit.ShowDialog();
				if(formLabCaseEdit.DialogResult!=DialogResult.OK) {
					return;
				}
				//Deleting or detaching labcase would have been done from in that window
				FillGrid();
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(IsSelectingUnattached) {
				if(gridMain.GetSelectedIndex()==-1){
					MsgBox.Show(this,"Please select an item first.");
					return;
				}
				ListLabCaseNumsSelected=gridMain.SelectedTags<LabCase>().Select(x => x.LabCaseNum).ToList();
				DialogResult=DialogResult.OK;
			}
			else {
				DialogResult=DialogResult.OK;
			}
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















