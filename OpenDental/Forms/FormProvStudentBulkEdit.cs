using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormProvStudentBulkEdit:FormODBase {
		private List<SchoolClass> _listSchoolClasses;

		public FormProvStudentBulkEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormProvStudentBulkEdit_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(() => FillGrid(),
				(int)TimeSpan.FromSeconds(0.5).TotalMilliseconds,
				textProvNum);
			comboClass.Items.Add(Lan.g(this,"All"));
			comboClass.SelectedIndex=0;
			_listSchoolClasses=SchoolClasses.GetDeepCopy();
			for(int i=0;i<_listSchoolClasses.Count;i++) {
				comboClass.Items.Add(SchoolClasses.GetDescript(_listSchoolClasses[i]));
			}
			FillGrid();
		}

		private void FillGrid() {
			long selectedProvNum=0;
			long schoolClass=0;
			if(comboClass.SelectedIndex>0) {
				schoolClass=_listSchoolClasses[comboClass.SelectedIndex-1].SchoolClassNum;
			}
			DataTable table=Providers.RefreshForDentalSchool(schoolClass,"","",textProvNum.Text,false,false);
			gridStudents.BeginUpdate();
			gridStudents.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableProviderSetup","ProvNum"),60);
			gridStudents.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProviderSetup","Last Name"),90);
			gridStudents.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProviderSetup","First Name"),90);
			gridStudents.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProviderSetup","Class"),100);
			gridStudents.ListGridColumns.Add(col);
			gridStudents.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<table.Rows.Count;i++) {
				row=new GridRow();
				if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
					row.Cells.Add(table.Rows[i]["ProvNum"].ToString());
				}
				row.Cells.Add(table.Rows[i]["LName"].ToString());
				row.Cells.Add(table.Rows[i]["FName"].ToString());
				if(table.Rows[i]["GradYear"].ToString()!="") {
						row.Cells.Add(table.Rows[i]["GradYear"].ToString()+"-"+table.Rows[i]["Descript"].ToString());
				}
				else {
						row.Cells.Add("");
				}

				gridStudents.ListGridRows.Add(row);
			}
			gridStudents.EndUpdate();
			for(int i=0;i<table.Rows.Count;i++) {
				if(table.Rows[i]["ProvNum"].ToString()==selectedProvNum.ToString()) {
					gridStudents.SetSelected(i,true);
					break;
				}
			}
		}

		private void comboClass_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butColor_Click(object sender,System.EventArgs e) {
			colorDialog1.Color=butColor.BackColor;
			colorDialog1.ShowDialog();
			butColor.BackColor=colorDialog1.Color;
			colorDialog1.Dispose();
		}

		private void butOutlineColor_Click(object sender,System.EventArgs e) {
			colorDialog1.Color=butOutlineColor.BackColor;
			colorDialog1.ShowDialog();
			butOutlineColor.BackColor=colorDialog1.Color;
			colorDialog1.Dispose();
		}

		private void butBulkEdit_Click(object sender,EventArgs e) {
			for(int i=0;i<gridStudents.SelectedIndices.Length;i++) {
				Provider studSelected=Providers.GetProv(PIn.Long(gridStudents.ListGridRows[i].Cells[0].Text));
				studSelected.ProvColor=butColor.BackColor;
				studSelected.OutlineColor=butOutlineColor.BackColor;
				Providers.Update(studSelected);
			}
			MsgBox.Show(this,"Selected students have been updated.");
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}



	}
}