using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormCertifications:FormODBase {

		private bool _isHeadingPrinted;
		///<summary>List of all certs, includes hidden.</summary>
		private List<Cert> _listCerts;
		///<summary>All non-hidden definitions related to DefCat.CertCategories.</summary>
		private List<Def> _listDefs;
		private List<Employee> _listEmployees;	
		private int _pageNum;
		
		public FormCertifications() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCertifications_Load(object sender, EventArgs e){
			//listBoxCategories needs to include All as first item.  It's multiselect, so in that case, you would just ignore any other selections.
			_listEmployees=Employees.GetDeepCopy();
			Employee employee=_listEmployees.Find(x => x.FName==" Escalate As Needed");
			_listEmployees.Remove(employee);
			List<Employee> listEmployees=_listEmployees.FindAll(x => x.IsHidden==false);
			listBoxEmployee.Items.AddList(listEmployees,x => (x.FName+" "+x.LName));
			comboSupervisor.Items.Add("Any",new Employee());
			List<Employee> listEmployeeSupers=new List<Employee>();
			for(int i=0;i<_listEmployees.Count;i++){
				if(_listEmployees[i].ReportsTo==0){
					continue;
				}
				if(listEmployeeSupers.Any(x=>x.EmployeeNum==_listEmployees[i].ReportsTo)){
					continue;
				}
				Employee supervisor = Employees.GetEmp(_listEmployees[i].ReportsTo);
				if(supervisor != null) {
					listEmployeeSupers.Add(supervisor);
				}
			}
			listEmployeeSupers=listEmployeeSupers.OrderBy(x => x.FName).ToList();
			comboSupervisor.Items.AddList(listEmployeeSupers,x => x.FName);
			comboSupervisor.SetSelected(0);
			_listDefs=Defs.GetDefsForCategory(DefCat.CertificationCategories,true);
			listBoxCategories.Items.Add("All");
			listBoxCategories.Items.AddList(_listDefs,x => x.ItemName);
			listBoxCategories2.Items.Add("All");
			listBoxCategories2.Items.AddList(_listDefs,x => x.ItemName);
			listBoxCategories.SetSelected(0);
			listBoxCategories2.SetSelected(0);
			_listCerts=Certs.GetAll(true);
			List<Cert> listCerts=GetCertsForCategories();
			//Make sure that the cert itself is not hidden nor is the category it is associated with hidden
			listBoxCertification.Items.AddList(listCerts,x => x.Description);
			labelCertification.Visible=false;
			listBoxCertification.Visible=false;
			labelCategories2.Visible=false;
			listBoxCategories2.Visible=false;
			checkSortDateCertComplete.Visible=false;
			LayoutManager.MoveLocation(labelCategories2,new Point(labelEmployee.Location.X,labelEmployee.Location.Y));
			LayoutManager.MoveLocation(listBoxCategories2,new Point(listBoxEmployee.Location.X,listBoxEmployee.Location.Y));
			LayoutManager.MoveLocation(labelCertification,new Point(labelCategories.Location.X,labelCategories.Location.Y));
			LayoutManager.MoveLocation(listBoxCertification,new Point(listBoxCategories.Location.X,listBoxCategories.Location.Y));
			LayoutManager.MoveLocation(checkSortDateCertComplete,new Point(checkIncomplete.Location.X,checkIncomplete.Location.Y));
			FillGrid();
		}

		private void FillGrid() {
			if(radioCategory.Checked) {//Based on groupBoxOrderBy
				if(checkSortDate.Checked && !checkIncomplete.Checked) {
					FillGridByCategoryCertCompletionDate();//Sorted by Cert Completion Date, then by Cert item order for incomplete Certs
				}
				else {
					FillGridByCategoryItemOrder();//Sorted by Category item order, then by Cert item order
				}
			}
			else{
				FillGridByCertComplete();
			}
		}

		private void FillGridByCategoryCertCompletionDate() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormCertifications","Category"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Certification"),175);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Wiki Page"),175);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Date"),65);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Note"),207);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			string lastCategoryName="";
			List<CertEmployee> listCertEmployees=new List<CertEmployee>();
			if(listBoxEmployee.SelectedIndex>-1) {//Check when loading and no employee selected
				listCertEmployees=CertEmployees.GetAllForEmployee(listBoxEmployee.GetSelected<Employee>().EmployeeNum)
					.OrderBy(x => x.DateCompleted).ToList();
			}
			Func<Cert,DateTime> funcSort=delegate(Cert x) {
				if(listCertEmployees.Find(y => y.CertNum==x.CertNum)==null) {
					return DateTime.MinValue;
				}
				else {
					return listCertEmployees.Find(y => y.CertNum==x.CertNum).DateCompleted;
				}
			};
			List<Cert> listCertsByDateCompleted=_listCerts.FindAll(x => !x.IsHidden && _listDefs.Any(y => y.DefNum==x.CertCategoryNum))
				.OrderByDescending(funcSort)
				.ThenBy(x => _listDefs.Find(y => y.DefNum==x.CertCategoryNum).ItemOrder)
				.ThenBy(x => x.ItemOrder).ToList();
			for(int i=0;i<listCertsByDateCompleted.Count;i++) {
				CertEmployee certEmployee=listCertEmployees.Find(x=>x.CertNum==listCertsByDateCompleted[i].CertNum);
				Def def=_listDefs.Find(x => x.DefNum==listCertsByDateCompleted[i].CertCategoryNum);
				if(listBoxCategories.SelectedIndices.Contains(0)) {
					//"All" is selected, so no filter.
				}
				else{
					if(!listBoxCategories.GetListSelected<Def>().Contains(def)) {
						continue;
					}
				}
				row=new GridRow();
				string categoryName=_listDefs.FirstOrDefault(x => x.DefNum==listCertsByDateCompleted[i].CertCategoryNum).ItemName;
				if(lastCategoryName==categoryName) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(categoryName);
					lastCategoryName=categoryName;
				}
				row.Cells.Add(listCertsByDateCompleted[i].Description);
				row.Cells.Add(listCertsByDateCompleted[i].WikiPageLink);
				if(certEmployee==null) {
					row.Cells.Add("");
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(certEmployee.DateCompleted.ToShortDateString());
					row.Cells.Add(certEmployee.Note);
				}
				row.Tag=listCertsByDateCompleted[i];//for double click
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void FillGridByCategoryItemOrder() { 
			//No more than one employee can ever be selected.
			//2 loops: first by def, then by cert.ItemOrder. 
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormCertifications","Category"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Certification"),175);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Wiki Page"),175);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Date"),65);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Note"),207);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			string lastCategoryName="";
			List<CertEmployee> listCertEmployees=new List<CertEmployee>();
			if(listBoxEmployee.SelectedIndex>-1) {//Check when loading and no employee selected
				listCertEmployees=CertEmployees.GetAllForEmployee(listBoxEmployee.GetSelected<Employee>().EmployeeNum)
					.OrderBy(x => x.DateCompleted).ToList();
			}
			for(int i=0;i<_listDefs.Count;i++) { 
				List<Cert> listCertsForCategory=_listCerts.FindAll(x => x.CertCategoryNum==_listDefs[i].DefNum && !x.IsHidden)
					.OrderBy(x => x.ItemOrder).ToList();
				for(int j=0;j<listCertsForCategory.Count;j++) {
					CertEmployee certEmployeeCur=listCertEmployees.Find(x=>x.CertNum==listCertsForCategory[j].CertNum);
					if(checkIncomplete.Checked && certEmployeeCur!=null) {//Only show incomplete Certs if checked
						continue;
					}
					if(listBoxCategories.SelectedIndices.Contains(0)) {
						//"All" is selected, so no filter.
					}
					else{
						if(!listBoxCategories.GetListSelected<Def>().Contains(_listDefs[i])) {
							continue;
						}
					}
					row=new GridRow();	
					string categoryName=_listDefs[i].ItemName;
					if(lastCategoryName==categoryName) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(categoryName);
						lastCategoryName=categoryName;
					}
					row.Cells.Add(listCertsForCategory[j].Description);
					row.Cells.Add(listCertsForCategory[j].WikiPageLink);
					if(certEmployeeCur==null) {
						row.Cells.Add("");
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(certEmployeeCur.DateCompleted.ToShortDateString());
						row.Cells.Add(certEmployeeCur.Note);
					}
					row.Tag=listCertsForCategory[j];//for double click
					gridMain.ListGridRows.Add(row);
				}
			}
			gridMain.EndUpdate();
		}

		private void FillGridByCertComplete() {
			//Ordered by cert.ItemOrder, then by employee.FName
			//Since we can only select 1 cert, there is only one loop, ordered by emp name.
			//Only shows completed
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormCertifications","Certification"),175);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Wiki Page"),175);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Employee"),120);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Date"),65);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Note"),167);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			string lastCertName="";
			Cert certSelected=listBoxCertification.GetSelected<Cert>();
			if(certSelected==null) {
				gridMain.EndUpdate();
			}
			else {
				List<CertEmployee> listCertEmployees=CertEmployees.GetAllForCert(certSelected.CertNum);
				if(checkSortDateCertComplete.Checked) {
					listCertEmployees=listCertEmployees.OrderByDescending(x => x.DateCompleted).ToList();
				}
				else {
					listCertEmployees=listCertEmployees.OrderBy(x => _listEmployees.Find(y => y.EmployeeNum==x.EmployeeNum).FName).ToList();
				}
				for(int i=0;i<listCertEmployees.Count;i++) {
					Employee employeeCur=_listEmployees.Find(x => x.EmployeeNum==listCertEmployees[i].EmployeeNum);
					GridRow row=new GridRow();
					if(certSelected.IsHidden){
						continue;
					}
					if(employeeCur.IsHidden) {
						continue;
					}
					if(lastCertName==certSelected.Description) {
						row.Cells.Add("");
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(certSelected.Description);
						row.Cells.Add(certSelected.WikiPageLink);
						lastCertName=certSelected.Description;
					}
					row.Cells.Add(employeeCur.FName+" "+employeeCur.LName);
					row.Cells.Add(listCertEmployees[i].DateCompleted.ToShortDateString());
					row.Cells.Add(listCertEmployees[i].Note);
					gridMain.ListGridRows.Add(row);
				}
				gridMain.EndUpdate();
			}
		}

		///<summary>Returns a list of certs that correspond to the selected CertCategories.</summary>
		private List<Cert> GetCertsForCategories() {
			List<Def> listDefsSelected;
			if(listBoxCategories2.SelectedIndices.Contains(0)) {//If "All" is not selected then filter the list of defs.
				listDefsSelected=new List<Def>(_listDefs);
			}
			else {//User selected specific defs.
				listDefsSelected=listBoxCategories2.GetListSelected<Def>();
			}
			List<Cert> listCertsFiltered=_listCerts.FindAll(x => !x.IsHidden && listDefsSelected.Any(y => y.DefNum==x.CertCategoryNum))
				.OrderBy(x => listDefsSelected.Find(y => y.DefNum==x.CertCategoryNum).ItemOrder)
				.ThenBy(x => x.ItemOrder).ToList();
			return listCertsFiltered;
		}

		private void comboSupervisor_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboSupervisor.SelectedIndex==0) {//If "Any" selected then reset
				listBoxEmployee.Items.Clear();
				List <Employee> listEmployees=_listEmployees.FindAll(x => x.IsHidden==false);
				listBoxEmployee.Items.AddList(listEmployees,x => (x.FName+" "+x.LName));
				FillGrid();
				return;
			}
			Employee employeeSuper=comboSupervisor.GetSelected<Employee>();
			if(employeeSuper==null) {//If clicking too fast this can happen
				return;
			}
			listBoxEmployee.Items.Clear();
			for(int i=0;i<_listEmployees.Count;i++) {
				if(_listEmployees[i].ReportsTo!=employeeSuper.EmployeeNum) {
					continue;
				}
				if(_listEmployees[i].IsHidden) {
					continue;
				}
				listBoxEmployee.Items.Add(_listEmployees[i].FName+" "+_listEmployees[i].LName,_listEmployees[i]);
			}
		}

		private void butSetup_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.CertificationSetup)) {
				return;
			}
			using FormCertificationSetup formCertificationSetup=new FormCertificationSetup();
			formCertificationSetup.ShowDialog();
			//Certs and CertLinkCategories need DB refresh, listBoxCert only needs a reorder
			_listCerts=Certs.GetAll(true);
			listBoxCategories2_SelectionChangeCommitted(this,e);
			FillGrid();
		}

		private void textEmpSearch_KeyUp(object sender,KeyEventArgs e) {
			comboSupervisor.SetSelected(0);
			string empNameSearch=PIn.String(textEmpSearch.Text).ToLower();
			List<Employee> listEmployeesFiltered=_listEmployees.FindAll(x => x.FName.ToLower().StartsWith(empNameSearch));
			listBoxEmployee.Items.Clear();
			listEmployeesFiltered=listEmployeesFiltered.FindAll(x => x.IsHidden==false);
			listBoxEmployee.Items.AddList(listEmployeesFiltered,x => (x.FName+" "+x.LName));
			if(listBoxEmployee.Items.Count==1) {
				listBoxEmployee.SelectedIndex=0;
				listBoxEmployee_SelectionChangeCommitted(this,e);
			}
		}

		private void radioCategory_Click(object sender,EventArgs e) {
			labelEmpSearch.Visible=true;
			textEmpSearch.Visible=true;
			labelCertification.Visible=false;
			listBoxCertification.Visible=false;
			labelCategories2.Visible=false;
			listBoxCategories2.Visible=false;
			labelCategories.Visible=true;
			listBoxCategories.Visible=true;
			labelEmployee.Visible=true;
			listBoxEmployee.Visible=true;
			labelReportsTo.Visible=true;
			comboSupervisor.Visible=true;
			checkIncomplete.Visible=true;
			checkSortDate.Visible=true;
			checkSortDateCertComplete.Visible=false;
			FillGrid();
		}

		private void radioCertification_Click(object sender,EventArgs e) {
			labelEmpSearch.Visible=false;
			textEmpSearch.Visible=false;
			labelCertification.Visible=true;
			listBoxCertification.Visible=true;
			labelCategories2.Visible=true;
			listBoxCategories2.Visible=true;
			labelCategories.Visible=false;
			listBoxCategories.Visible=false;
			labelEmployee.Visible=false;
			listBoxEmployee.Visible=false;
			labelReportsTo.Visible=false;
			comboSupervisor.Visible=false;
			checkIncomplete.Visible=false;
			checkSortDate.Visible=false;
			checkSortDateCertComplete.Visible=true;
			FillGrid();
		}

		private void listBoxEmployee_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void listBoxCategories_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void listBoxCategories2_SelectionChangeCommitted(object sender,EventArgs e) {
			List<Cert> listCertsFiltered=GetCertsForCategories();
			listBoxCertification.Items.Clear();
			listBoxCertification.Items.AddList(listCertsFiltered,x => x.Description);
			if(listBoxCertification.Items.Count>0) {
				listBoxCertification.SelectedIndex=0;
			}
			FillGrid();
		}

		private void listBoxCertification_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void checkIncomplete_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void checkSortDate_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void checkSortDateCertComplete_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(radioCertification.Checked) {
				return;
			}
			if(!Security.IsAuthorized(Permissions.CertificationEmployee)) {
				return;
			}
			if(listBoxEmployee.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an Employee first.");
				return;
			}
			Cert cert=Certs.GetOne(((Cert)gridMain.ListGridRows[e.Row].Tag).CertNum);
			CertEmployee certEmployee=CertEmployees.GetOne(cert.CertNum,listBoxEmployee.GetSelected<Employee>().EmployeeNum);
			using FormCertEmployee formCertEmployee=new FormCertEmployee();
			formCertEmployee.Employee=listBoxEmployee.GetSelected<Employee>();
			formCertEmployee.Cert=cert;
			if(certEmployee==null) {//Is new so create a new instance
				formCertEmployee.CertEmployee=new CertEmployee();
				formCertEmployee.CertEmployee.IsNew=true;	
			}
			else {//If found we are editing
				formCertEmployee.CertEmployee=certEmployee;	
			}
			formCertEmployee.ShowDialog();
			if(formCertEmployee.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_pageNum=0;
			_isHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Certifications printed"));
		}

		private void pd_PrintPage(object sender,PrintPageEventArgs e) {
			Rectangle rectangleBounds=e.MarginBounds;
			Graphics g=e.Graphics;//alias
			string headingText;
			Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			int yPosition=rectangleBounds.Top;
			int centerPosition=rectangleBounds.X+rectangleBounds.Width/2;
			int gridPrintPosition=0;
			if(_isHeadingPrinted) {
				//Heading has been printed, so do not create one.
			}
			else{
				headingText=Lan.g(this,"Certifications Completed");
				g.DrawString(headingText,fontHeading,Brushes.Black,centerPosition-g.MeasureString(headingText,fontHeading).Width/2,yPosition);
				yPosition+=25;
				_isHeadingPrinted=true;
				gridPrintPosition=yPosition;
			}
			yPosition=gridMain.PrintPage(g,_pageNum,rectangleBounds,gridPrintPosition);
			_pageNum++;
			if(yPosition==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			Close();
		}
	}
}