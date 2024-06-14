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
		private List<SetupWizard.SetupWizClass> _listSetupWizClassesItems;

		public FormSetupWizard() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSetupWizard_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillListSetupItems() {
			_listSetupWizClassesItems= new List<SetupWizard.SetupWizClass>();
			_listSetupWizClassesItems.Add(new SetupWizard.RegKeySetup());
			_listSetupWizClassesItems.Add(new SetupWizard.FeatureSetup());
			_listSetupWizClassesItems.Add(new SetupWizard.ProvSetup());
			_listSetupWizClassesItems.Add(new SetupWizard.EmployeeSetup());
			_listSetupWizClassesItems.Add(new SetupWizard.FeeSchedSetup());
			if(PrefC.HasClinicsEnabled) {
				_listSetupWizClassesItems.Add(new SetupWizard.ClinicSetup());
			}
			_listSetupWizClassesItems.Add(new SetupWizard.OperatorySetup());
			_listSetupWizClassesItems.Add(new SetupWizard.PracticeSetup());
			_listSetupWizClassesItems.Add(new SetupWizard.PrinterSetup());
			_listSetupWizClassesItems.Add(new SetupWizard.DefinitionSetup());
			//_listSetupWizItems.Add(new SetupWizard.ScheduleSetup());
			//_listSetupWizItems.Add(new SetupWizard.CarrierSetup());
			//_listSetupWizItems.Add(new SetupWizard.ClearinghouseSetup());
			//Add more here.
		}

		private void FillGrid() {
			FillListSetupItems();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			//ODGridColumn col = new ODGridColumn("Setup Item",250);
			gridMain.Columns.Add(new GridColumn("Setup Item",250));
			//col = new ODGridColumn("Status",100,HorizontalAlignment.Center);
			gridMain.Columns.Add(new GridColumn("Status",100,HorizontalAlignment.Center));
			//col = new ODGridColumn("?",35,HorizontalAlignment.Center);
			//col.ImageList=imageList1;
			gridMain.Columns.Add(new GridColumn("?",35,HorizontalAlignment.Center) { ImageList=imageList1 });
			gridMain.ListGridRows.Clear();
			//Add the method rows to the grid.
			List<GridRow> listRows = ConstructGridRows();
			for(int i=0;i<listRows.Count;i++) {
				gridMain.ListGridRows.Add(listRows[i]);
			}
			gridMain.EndUpdate();
		}

		private List<GridRow> ConstructGridRows() {
			//the Tag of a Parent Row is its ODSetupCategory.
			//the Tag of a Child Row is a SetupWizClass
			List<GridRow> listRowsSetup = new List<GridRow>();
			List<GridRow> listRowsCategory = new List<GridRow>();
			List<GridRow> listRowsAll = new List<GridRow>();
			int statusCellNum = 0;
			for(int i=0;i<_listSetupWizClassesItems.Count;i++) {
				GridRow row = new GridRow();
				row.Cells.Add("     "+_listSetupWizClassesItems[i].Name);
				row.Cells.Add(_listSetupWizClassesItems[i].GetStatus.GetDescription());
				statusCellNum=row.Cells.Count-1;
				row.Cells[statusCellNum].ColorBackG = SetupWizard.GetColor(_listSetupWizClassesItems[i].GetStatus);
				row.Cells.Add("0");
				//row.ColorBackG=SetupWizard.GetColor(setupItem.GetStatus);
				row.Tag=_listSetupWizClassesItems[i];
				listRowsSetup.Add(row);
			}
			//now add parent rows to the list
			for(int i=0;i<listRowsSetup.Count;i++) {
				ODSetupCategory odSetupCategory = ((SetupWizard.SetupWizClass)listRowsSetup[i].Tag).GetCategory;
				//bool exists = false;
				////if the parent row doesn't exist..
				//foreach(ODGridRow parentRow in listCategoryRows) {
				//	if(parentRow.Tag.GetType() == typeof(ODSetupCategory)
				//		&& ((ODSetupCategory)parentRow.Tag) == catCur) {
				//		exists=true;
				//		break;
				//	}
				//}
				if(listRowsCategory.Any(x => x.Tag is ODSetupCategory && (ODSetupCategory)x.Tag == odSetupCategory)) {
					continue;
				}
				//add the parent row.
				GridRow row = new GridRow();
				row.Cells.Add("\r\n"+odSetupCategory.GetDescription()+"\r\n");
				row.Cells.Add("");
				row.Cells.Add("");
				row.Tag=odSetupCategory;
				row.Bold=true;
				//row.ColorLborder=Color.Black;
				listRowsCategory.Add(row);
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
			for (int i=0;i<listRowsCategory.Count();i++) {
				if(listRowsSetup.FindAll(x => ((SetupWizard.SetupWizClass)x.Tag).GetCategory == ((ODSetupCategory)listRowsCategory[i].Tag))
					.All(x => ((SetupWizard.SetupWizClass)x.Tag).GetStatus == ODSetupStatus.Complete || ((SetupWizard.SetupWizClass)x.Tag).GetStatus == ODSetupStatus.Optional))
				{
					listRowsCategory[i].Cells[statusCellNum].Text="\r\n"+ODSetupStatus.Complete.GetDescription();
					listRowsCategory[i].Cells[statusCellNum].ColorBackG=SetupWizard.GetColor(ODSetupStatus.Complete);
					
				}
				else {
					listRowsCategory[i].Cells[statusCellNum].Text="\r\n"+ODSetupStatus.NeedsAttention.GetDescription();
					listRowsCategory[i].Cells[statusCellNum].ColorBackG=SetupWizard.GetColor(ODSetupStatus.NeedsAttention);
				}
			}
			for (int i=0;i<listRowsCategory.Count();i++) {
				listRowsAll.Add(listRowsCategory[i]);
				listRowsSetup.FindAll(x => ((SetupWizard.SetupWizClass)x.Tag).GetCategory == ((ODSetupCategory)listRowsCategory[i].Tag)).DefaultIfEmpty(new GridRow()).LastOrDefault().ColorLborder=Color.Black;
				listRowsAll.AddRange(listRowsSetup.FindAll(x => ((SetupWizard.SetupWizClass)x.Tag).GetCategory == ((ODSetupCategory)listRowsCategory[i].Tag)));
			}
			return listRowsAll;
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			GridRow rowClicked = gridMain.ListGridRows[e.Row];
			GridColumn colClicked = gridMain.Columns[e.Col];
			if(rowClicked.Tag.GetType() == typeof(ODSetupCategory)) {
				for(int i = 0;i < gridMain.ListGridRows.Count;i++) {
					GridRow row = gridMain.ListGridRows[i];
					if(row.Tag is SetupWizard.SetupWizClass
						&& ((SetupWizard.SetupWizClass)row.Tag).GetCategory == (ODSetupCategory)rowClicked.Tag) {
						gridMain.SetSelected(i,true);
					}
				}
				return;
			}
			if(rowClicked.Tag.GetType().BaseType != typeof(SetupWizard.SetupWizClass)
				|| colClicked.ImageList == null) {
				return;
			}
			MsgBox.Show(this,((SetupWizard.SetupWizClass)rowClicked.Tag).GetDescript);
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//Show a "Congatulations, you've already finished this!" section for finished sections.
			GridRow rowClicked = gridMain.ListGridRows[e.Row];
			List<SetupWizard.SetupWizClass> listSetupWizClasses = new List<SetupWizard.SetupWizClass>();
			if(rowClicked.Tag.GetType().BaseType != typeof(SetupWizard.SetupWizClass)) { //category clicked
				for (int i=0;i<_listSetupWizClassesItems.Count();i++) {
					if(_listSetupWizClassesItems[i].GetCategory != (ODSetupCategory)rowClicked.Tag) {
						continue;
					}
					SetupWizard.SetupIntro setupIntroCat = new SetupWizard.SetupIntro(_listSetupWizClassesItems[i].Name, _listSetupWizClassesItems[i].GetDescript);
					SetupWizard.SetupComplete setupCompleteCat = new SetupWizard.SetupComplete(_listSetupWizClassesItems[i].Name);
					listSetupWizClasses.Add(setupIntroCat);
					listSetupWizClasses.Add(_listSetupWizClassesItems[i]);
					listSetupWizClasses.Add(setupCompleteCat);
				}
				RemoveUnauthorizedWizClasses(listSetupWizClasses);
				if(listSetupWizClasses.Count == 0) {
					return;
				}
				using FormSetupWizardProgress formSetupWizardProgressCat=new FormSetupWizardProgress(listSetupWizClasses,true);
				formSetupWizardProgressCat.ShowDialog();
				FillGrid();
				return;
			}
			//single row clicked
			SetupWizard.SetupWizClass setupWizClass = (SetupWizard.SetupWizClass)rowClicked.Tag;
			SetupWizard.SetupIntro setupIntro = new SetupWizard.SetupIntro(setupWizClass.Name,setupWizClass.GetDescript);
			SetupWizard.SetupComplete setupComplete = new SetupWizard.SetupComplete(setupWizClass.Name);
			listSetupWizClasses.Add(setupIntro);
			listSetupWizClasses.Add(setupWizClass);
			listSetupWizClasses.Add(setupComplete);
			RemoveUnauthorizedWizClasses(listSetupWizClasses);
			if(listSetupWizClasses.Count == 0) {
				return;
			}
			using FormSetupWizardProgress formSetupWizardProgress=new FormSetupWizardProgress(listSetupWizClasses,false);
			formSetupWizardProgress.ShowDialog();
			FillGrid();
		}

		private void butAll_Click(object sender,EventArgs e) {
			List<SetupWizard.SetupWizClass> listSetupWizClasses = new List<OpenDental.SetupWizard.SetupWizClass>();
			for(int i=0;i<_listSetupWizClassesItems.Count();i++) {
				SetupWizard.SetupIntro setupIntro = new SetupWizard.SetupIntro(_listSetupWizClassesItems[i].Name,_listSetupWizClassesItems[i].GetDescript);
				SetupWizard.SetupComplete setupComplete = new SetupWizard.SetupComplete(_listSetupWizClassesItems[i].Name);
				listSetupWizClasses.Add(setupIntro);
				listSetupWizClasses.Add(_listSetupWizClassesItems[i]);
				listSetupWizClasses.Add(setupComplete);
			}
			RemoveUnauthorizedWizClasses(listSetupWizClasses);
			using FormSetupWizardProgress formSetupWizardProgress = new FormSetupWizardProgress(listSetupWizClasses,true);
			formSetupWizardProgress.ShowDialog();
			FillGrid();
		}

		private void butSelected_Click(object sender,EventArgs e) {
			List<SetupWizard.SetupWizClass> listSetupWizClasses = new List<SetupWizard.SetupWizClass>();
			for(int i=0;i<gridMain.SelectedIndices.Count();i++) {
				GridRow gridRowSelected = gridMain.ListGridRows[gridMain.SelectedIndices[i]];
				if(gridRowSelected.Tag.GetType().BaseType != typeof(OpenDental.SetupWizard.SetupWizClass)) {
					continue;
				}
				SetupWizard.SetupWizClass setupWizClass = (SetupWizard.SetupWizClass)gridRowSelected.Tag;
				SetupWizard.SetupIntro setupIntro = new SetupWizard.SetupIntro(setupWizClass.Name,setupWizClass.GetDescript);
				SetupWizard.SetupComplete setupComplete = new SetupWizard.SetupComplete(setupWizClass.Name);
				listSetupWizClasses.Add(setupIntro);
				listSetupWizClasses.Add(setupWizClass);
				listSetupWizClasses.Add(setupComplete);
			}
			RemoveUnauthorizedWizClasses(listSetupWizClasses);
			if(listSetupWizClasses.Count == 0) {
				return;
			}
			using FormSetupWizardProgress formSetupWizardProgress = new FormSetupWizardProgress(listSetupWizClasses,false);
			formSetupWizardProgress.ShowDialog();
		}

		public void RemoveUnauthorizedWizClasses(List<SetupWizard.SetupWizClass> listSetupClasses) {
			//Check permissions for each SetUpWizClass and remove from list if permissions are missing
			string message = Lans.g("Security","Not authorized for")+"\r\n";
			bool didRemove = false;
			if(!Security.IsAuthorized(EnumPermType.ShowFeatures,true) && listSetupClasses.Any(x => x.Name =="Basic Features")) {
				listSetupClasses.RemoveAll(x => x.Name == "Basic Features");
				message += "\r\n" + GroupPermissions.GetDesc(EnumPermType.ShowFeatures);
				didRemove=true;
			}
			if(!Security.IsAuthorized(EnumPermType.ProviderEdit,true) && listSetupClasses.Any(x => x.Name =="Providers")) {
				listSetupClasses.RemoveAll(x => x.Name == "Providers");
				message += "\r\n" + GroupPermissions.GetDesc(EnumPermType.ProviderEdit);
				didRemove=true;
			}
			if(!Security.IsAuthorized(EnumPermType.ClinicEdit,true) && listSetupClasses.Any(x => x.Name =="Clinics")) {
				listSetupClasses.RemoveAll(x => x.Name == "Clinics");
				message += "\r\n" + GroupPermissions.GetDesc(EnumPermType.ClinicEdit);
				didRemove=true;
			}
			if(!Security.IsAuthorized(EnumPermType.PrinterSetup,true) && listSetupClasses.Any(x => x.Name =="Printer/Scanner")) {
				listSetupClasses.RemoveAll(x => x.Name == "Printer/Scanner");
				message += "\r\n" + GroupPermissions.GetDesc(EnumPermType.PrinterSetup);
				didRemove=true;
			}
			if(didRemove) {
				MessageBox.Show(message);
			}
		}

	}
}