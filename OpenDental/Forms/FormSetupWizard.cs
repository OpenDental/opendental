using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using CodeBase;

namespace OpenDental {
	public partial class FormSetupWizard:FormODBase {
		private List<SetupWizard.SetupWizClass> _listSetupWizItems;

		public FormSetupWizard() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSetupWizard_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillListSetupItems() {
			_listSetupWizItems= new List<SetupWizard.SetupWizClass>();
			_listSetupWizItems.Add(new SetupWizard.RegKeySetup());
			_listSetupWizItems.Add(new SetupWizard.FeatureSetup());
			_listSetupWizItems.Add(new SetupWizard.ProvSetup());
			_listSetupWizItems.Add(new SetupWizard.EmployeeSetup());
			_listSetupWizItems.Add(new SetupWizard.FeeSchedSetup());
			if(PrefC.HasClinicsEnabled) {
				_listSetupWizItems.Add(new SetupWizard.ClinicSetup());
			}
			_listSetupWizItems.Add(new SetupWizard.OperatorySetup());
			_listSetupWizItems.Add(new SetupWizard.PracticeSetup());
			_listSetupWizItems.Add(new SetupWizard.PrinterSetup());
			_listSetupWizItems.Add(new SetupWizard.DefinitionSetup());
			//_listSetupWizItems.Add(new SetupWizard.ScheduleSetup());
			//_listSetupWizItems.Add(new SetupWizard.CarrierSetup());
			//_listSetupWizItems.Add(new SetupWizard.ClearinghouseSetup());
			//Add more here.
		}

		private void FillGrid() {
			FillListSetupItems();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			//ODGridColumn col = new ODGridColumn("Setup Item",250);
			gridMain.ListGridColumns.Add(new GridColumn("Setup Item",250));
			//col = new ODGridColumn("Status",100,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(new GridColumn("Status",100,HorizontalAlignment.Center));
			//col = new ODGridColumn("?",35,HorizontalAlignment.Center);
			//col.ImageList=imageList1;
			gridMain.ListGridColumns.Add(new GridColumn("?",35,HorizontalAlignment.Center) { ImageList=imageList1 });
			gridMain.ListGridRows.Clear();
			//Add the method rows to the grid.
			List<GridRow> listRows = ConstructGridRows();
			foreach(GridRow row in listRows) {
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private List<GridRow> ConstructGridRows() {
			//the Tag of a Parent Row is its ODSetupCategory.
			//the Tag of a Child Row is a SetupWizClass
			List<GridRow> listSetupRows = new List<GridRow>();
			List<GridRow> listCategoryRows = new List<GridRow>();
			List<GridRow> listRowsAll = new List<GridRow>();
			int statusCellNum = 0;
			foreach(SetupWizard.SetupWizClass setupItem in _listSetupWizItems) {
				GridRow row = new GridRow();
				row.Cells.Add("     "+setupItem.Name);
				row.Cells.Add(setupItem.GetStatus.GetDescription());
				statusCellNum=row.Cells.Count-1;
				row.Cells[statusCellNum].ColorBackG = SetupWizard.GetColor(setupItem.GetStatus);
				row.Cells.Add("0");
				//row.ColorBackG=SetupWizard.GetColor(setupItem.GetStatus);
				row.Tag=setupItem;
				listSetupRows.Add(row);
			}
			//now add parent rows to the list
			foreach(GridRow rowCur in listSetupRows) {
				ODSetupCategory catCur = ((SetupWizard.SetupWizClass)rowCur.Tag).GetCategory;
				//bool exists = false;
				////if the parent row doesn't exist..
				//foreach(ODGridRow parentRow in listCategoryRows) {
				//	if(parentRow.Tag.GetType() == typeof(ODSetupCategory)
				//		&& ((ODSetupCategory)parentRow.Tag) == catCur) {
				//		exists=true;
				//		break;
				//	}
				//}
				if(listCategoryRows.Any(x => x.Tag is ODSetupCategory && (ODSetupCategory)x.Tag == catCur)) {
					continue;
				}
				//add the parent row.
				GridRow row = new GridRow();
				row.Cells.Add("\r\n"+catCur.GetDescription()+"\r\n");
				row.Cells.Add("");
				row.Cells.Add("");
				row.Tag=catCur;
				row.Bold=true;
				//row.ColorLborder=Color.Black;
				listCategoryRows.Add(row);
				//}
				////for all children rows, find the parent row -- set it to the proper parent row.
				//foreach(ODGridRow parentRow in listParentRows) {
				//	if(parentRow.Tag.GetType() == typeof(ODSetupCategory)
				//		&& ((ODSetupCategory)parentRow.Tag) == catCur) {
				//		rowCur.DropDownParent=parentRow;
				//		break;
				//	}
				//}
			}
			//Assign colors to parent rows.
			foreach(GridRow rowCur in listCategoryRows) {
				if(listSetupRows.Where(x => ((SetupWizard.SetupWizClass)x.Tag).GetCategory == ((ODSetupCategory)rowCur.Tag))
					.All(x => ((SetupWizard.SetupWizClass)x.Tag).GetStatus == ODSetupStatus.Complete || ((SetupWizard.SetupWizClass)x.Tag).GetStatus == ODSetupStatus.Optional))
				{
					//rowCur.ColorBackG = SetupWizard.GetColor(ODSetupStatus.Complete);
					rowCur.Cells[statusCellNum].Text="\r\n"+ODSetupStatus.Complete.GetDescription();
					rowCur.Cells[statusCellNum].ColorBackG=SetupWizard.GetColor(ODSetupStatus.Complete);
					
				}
				else {
					//rowCur.ColorBackG = SetupWizard.GetColor(ODSetupStatus.NeedsAttention);
					rowCur.Cells[statusCellNum].Text="\r\n"+ODSetupStatus.NeedsAttention.GetDescription();
					rowCur.Cells[statusCellNum].ColorBackG=SetupWizard.GetColor(ODSetupStatus.NeedsAttention);
				}
			}
			foreach(GridRow rowCur in listCategoryRows) {
				listRowsAll.Add(rowCur);
				listSetupRows.Where(x => ((SetupWizard.SetupWizClass)x.Tag).GetCategory == ((ODSetupCategory)rowCur.Tag)).DefaultIfEmpty(new GridRow()).LastOrDefault().ColorLborder = Color.Black;
				listRowsAll.AddRange(listSetupRows.Where(x => ((SetupWizard.SetupWizClass)x.Tag).GetCategory == ((ODSetupCategory)rowCur.Tag)));
			}
			return listRowsAll;
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			GridRow clickedRow = gridMain.ListGridRows[e.Row];
			GridColumn clickedCol = gridMain.ListGridColumns[e.Col];
			if(clickedRow.Tag.GetType() == typeof(ODSetupCategory)) {
				for(int i = 0;i < gridMain.ListGridRows.Count;i++) {
					GridRow row = gridMain.ListGridRows[i];
					if(row.Tag is SetupWizard.SetupWizClass
						&& ((SetupWizard.SetupWizClass)row.Tag).GetCategory == (ODSetupCategory)clickedRow.Tag) {
						gridMain.SetSelected(i,true);
					}
				}
				return;
			}
			if(clickedRow.Tag.GetType().BaseType != typeof(SetupWizard.SetupWizClass)
				|| clickedCol.ImageList == null) {
				return;
			}
			MsgBox.Show(this,((SetupWizard.SetupWizClass)clickedRow.Tag).GetDescript);
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//Show a "Congatulations, you've already finished this!" section for finished sections.
			GridRow clickedRow = gridMain.ListGridRows[e.Row];
			List<SetupWizard.SetupWizClass> listSetupClasses = new List<SetupWizard.SetupWizClass>();
			if(clickedRow.Tag.GetType().BaseType != typeof(SetupWizard.SetupWizClass)) { //category clicked
				foreach(SetupWizard.SetupWizClass setupWizClass in _listSetupWizItems) { //for each row, add the row and an intro and complete class.
					if(setupWizClass.GetCategory == (ODSetupCategory)clickedRow.Tag) {
						SetupWizard.SetupIntro intro = new SetupWizard.SetupIntro(setupWizClass.Name, setupWizClass.GetDescript);
						SetupWizard.SetupComplete complete = new SetupWizard.SetupComplete(setupWizClass.Name);
						listSetupClasses.Add(intro);
						listSetupClasses.Add(setupWizClass);
						listSetupClasses.Add(complete);
					}
				}
				using FormSetupWizardProgress FormSWP=new FormSetupWizardProgress(listSetupClasses,true);
				FormSWP.ShowDialog();
			}
			else { //single row clicked
				SetupWizard.SetupWizClass setupWizClass = (SetupWizard.SetupWizClass)clickedRow.Tag;
				SetupWizard.SetupIntro intro = new SetupWizard.SetupIntro(setupWizClass.Name,setupWizClass.GetDescript);
				SetupWizard.SetupComplete complete = new SetupWizard.SetupComplete(setupWizClass.Name);
				listSetupClasses.Add(intro);
				listSetupClasses.Add(setupWizClass);
				listSetupClasses.Add(complete);
				using FormSetupWizardProgress FormSWP=new FormSetupWizardProgress(listSetupClasses,false);
				FormSWP.ShowDialog();
			}
			FillGrid();
		}

		private void butAll_Click(object sender,EventArgs e) {
			List<SetupWizard.SetupWizClass> listSetupClasses = new List<OpenDental.SetupWizard.SetupWizClass>();
			foreach(SetupWizard.SetupWizClass setupWizClass in _listSetupWizItems) { //for each row, add the row and an intro and complete class.
					SetupWizard.SetupIntro intro = new SetupWizard.SetupIntro(setupWizClass.Name,setupWizClass.GetDescript);
					SetupWizard.SetupComplete complete = new SetupWizard.SetupComplete(setupWizClass.Name);
					listSetupClasses.Add(intro);
					listSetupClasses.Add(setupWizClass);
					listSetupClasses.Add(complete);
			}
			using FormSetupWizardProgress FormSWP = new FormSetupWizardProgress(listSetupClasses,true);
			FormSWP.ShowDialog();
			FillGrid();
		}

		private void butSelected_Click(object sender,EventArgs e) {
			List<SetupWizard.SetupWizClass> listSetupClasses = new List<SetupWizard.SetupWizClass>();
			foreach(int rowNum in gridMain.SelectedIndices) {
				GridRow selectedRow = gridMain.ListGridRows[rowNum];
				if(selectedRow.Tag.GetType().BaseType != typeof(OpenDental.SetupWizard.SetupWizClass)) {
					continue;
				}
				SetupWizard.SetupWizClass setupWizClass = (SetupWizard.SetupWizClass)selectedRow.Tag;
				SetupWizard.SetupIntro intro = new SetupWizard.SetupIntro(setupWizClass.Name,setupWizClass.GetDescript);
				SetupWizard.SetupComplete complete = new SetupWizard.SetupComplete(setupWizClass.Name);
				listSetupClasses.Add(intro);
				listSetupClasses.Add(setupWizClass);
				listSetupClasses.Add(complete);
			}
			using FormSetupWizardProgress FormSWP = new FormSetupWizardProgress(listSetupClasses,false);
			FormSWP.ShowDialog();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}