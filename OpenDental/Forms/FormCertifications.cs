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
		private List<Cert> _listCerts;
		///<summary>Only ordered by cert.ItemOrder.</summary>
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
			_listEmployees.Remove(_listEmployees.Find(x => x.FName==" Escalate As Needed"));
			listBoxEmployee.Items.AddList(_listEmployees.FindAll(x => x.IsHidden==false),x => (x.FName+" "+x.LName));
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
			listBoxCertification.Items.AddList(_listCerts.FindAll(x => !x.IsHidden && _listDefs.Find(y => y.DefNum==x.CertCategoryNum)!=null)
				,x => x.Description);//Make sure that the cert itself is not hidden nor is the category it is associated with hidden
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
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormCertifications","Category"),80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Certification"),175);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Wiki Page"),175);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Date"),65);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Note"),207);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			string lastCategoryName="";
			List<CertEmployee> listCertEmployee=new List<CertEmployee>();
			if(listBoxEmployee.SelectedIndex>-1) {//Check when loading and no employee selected
				listCertEmployee=CertEmployees.GetAllForEmployee(listBoxEmployee.GetSelected<Employee>().EmployeeNum)
					.OrderBy(x => x.DateCompleted).ToList();
			}
			Func<Cert,DateTime> funcSort=delegate(Cert x) {
				if(listCertEmployee.Find(y => y.CertNum==x.CertNum)==null) {
					return DateTime.MinValue;
				}
				else {
					return listCertEmployee.Find(y => y.CertNum==x.CertNum).DateCompleted;
				}
			};
			List<Cert> listCertByDateCompleted=_listCerts.FindAll(x => !x.IsHidden && _listDefs.Any(y => y.DefNum==x.CertCategoryNum))
				.OrderByDescending(funcSort)
				.ThenBy(x => _listDefs.Find(y => y.DefNum==x.CertCategoryNum).ItemOrder)
				.ThenBy(x => x.ItemOrder).ToList();
			for(int i=0;i<listCertByDateCompleted.Count;i++) {
				CertEmployee certEmployee=listCertEmployee.Find(x=>x.CertNum==listCertByDateCompleted[i].CertNum);
				Def def=_listDefs.Find(x => x.DefNum==listCertByDateCompleted[i].CertCategoryNum);
				if(listBoxCategories.SelectedIndices.Contains(0)) {
					//"All" is selected, so no filter.
				}
				else{
					if(!listBoxCategories.GetListSelected<Def>().Contains(def)) {
						continue;
					}
				}
				row=new GridRow();
				string categoryName=_listDefs.FirstOrDefault(x => x.DefNum==listCertByDateCompleted[i].CertCategoryNum).ItemName;
				if(lastCategoryName==categoryName) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(categoryName);
					lastCategoryName=categoryName;
				}
				row.Cells.Add(listCertByDateCompleted[i].Description);
				row.Cells.Add(listCertByDateCompleted[i].WikiPageLink);
				if(certEmployee==null) {
					row.Cells.Add("");
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(certEmployee.DateCompleted.ToShortDateString());
					row.Cells.Add(certEmployee.Note);
				}
				row.Tag=listCertByDateCompleted[i];//for double click
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void FillGridByCategoryItemOrder() { 
			//columns: Category, Certification, Date, Note
			//No more than one employee can ever be selected.
			//2 loops: first by def, then by cert.ItemOrder. 
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormCertifications","Category"),80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Certification"),175);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Wiki Page"),175);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Date"),65);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Note"),207);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			string lastCategoryName="";
			List<CertEmployee> listCertEmployee=new List<CertEmployee>();
			if(listBoxEmployee.SelectedIndex>-1) {//Check when loading and no employee selected
				listCertEmployee=CertEmployees.GetAllForEmployee(listBoxEmployee.GetSelected<Employee>().EmployeeNum)
					.OrderBy(x => x.DateCompleted).ToList();
			}
			for(int i=0;i<_listDefs.Count;i++) { 
				List<Cert> listCertsForCategory=_listCerts.FindAll(x => x.CertCategoryNum==_listDefs[i].DefNum && !x.IsHidden)
					.OrderBy(x => x.ItemOrder).ToList();
				for(int j=0;j<listCertsForCategory.Count;j++) {
					CertEmployee certEmployee=listCertEmployee.Find(x=>x.CertNum==listCertsForCategory[j].CertNum);
					if(checkIncomplete.Checked && certEmployee!=null) {//Only show incomplete Certs if checked
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
					if(certEmployee==null) {
						row.Cells.Add("");
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(certEmployee.DateCompleted.ToShortDateString());
						row.Cells.Add(certEmployee.Note);
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
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormCertifications","Certification"),175);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Wiki Page"),175);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Employee"),120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Date"),65);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormCertifications","Note"),167);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			string lastCertName="";
			Cert cert=listBoxCertification.GetSelected<Cert>();
			if(cert==null) {
				gridMain.EndUpdate();
			}
			else {
				List<CertEmployee> listCertEmployee=CertEmployees.GetAllForCert(cert.CertNum);
				if(checkSortDateCertComplete.Checked) {
					listCertEmployee=listCertEmployee.OrderByDescending(x => x.DateCompleted).ToList();
				}
				else {
					listCertEmployee=listCertEmployee.OrderBy(x => _listEmployees.Find(y => y.EmployeeNum==x.EmployeeNum).FName).ToList();
				}
				for(int i=0;i<listCertEmployee.Count;i++) {
					Employee employee=_listEmployees.Find(x => x.EmployeeNum==listCertEmployee[i].EmployeeNum);
					GridRow row=new GridRow();
					if(cert.IsHidden){
						continue;
					}
					if(employee.IsHidden) {
						continue;
					}
					if(lastCertName==cert.Description) {
						row.Cells.Add("");
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(cert.Description);
						row.Cells.Add(cert.WikiPageLink);
						lastCertName=cert.Description;
					}
					row.Cells.Add(employee.FName+" "+employee.LName);
					row.Cells.Add(listCertEmployee[i].DateCompleted.ToShortDateString());
					row.Cells.Add(listCertEmployee[i].Note);
					gridMain.ListGridRows.Add(row);
				}
				gridMain.EndUpdate();
			}
		}

		private void comboSupervisor_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboSupervisor.SelectedIndex==0) {//If "Any" selected then reset
				listBoxEmployee.Items.Clear();
				listBoxEmployee.Items.AddList(_listEmployees.FindAll(x => x.IsHidden==false),x => (x.FName+" "+x.LName));
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
			listBoxEmployee.Items.AddList(listEmployeesFiltered.FindAll(x => x.IsHidden==false),x => (x.FName+" "+x.LName));
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
			if(listBoxCategories2.SelectedIndices.Contains(0)) {//If "All" selected then reset
				listBoxCertification.Items.Clear();
				listBoxCertification.Items.AddList(_listCerts.FindAll(x => !x.IsHidden && _listDefs.Find(y => y.DefNum==x.CertCategoryNum)!=null)
					,x => x.Description);//Make sure that the cert itself is not hidden nor is the category it is associated with hidden
				FillGrid();
				return;
			}
			List<Def> listDefsFiltered=listBoxCategories2.GetListSelected<Def>();
			List<Cert> listCertsFilterd=new List<Cert>();
			for(int i=0;i<listDefsFiltered.Count;i++) {
				listCertsFilterd.AddRange(_listCerts.FindAll(x =>x.CertCategoryNum==listDefsFiltered[i].DefNum).Where(x => x.IsHidden==false)
					.Select(x => x).ToList());
			}
			listCertsFilterd=listCertsFilterd.Select(x => x).OrderBy(x => x.ItemOrder).Distinct().ToList();
			listBoxCertification.Items.Clear();
			listBoxCertification.Items.AddList(listCertsFilterd.FindAll(x => x.IsHidden==false),x => x.Description);
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
			if(certEmployee!=null) {//If found we are editing
				formCertEmployee.CertEmployee=certEmployee;		
			}
			else {//Is new so create a new instance
				formCertEmployee.CertEmployee=new CertEmployee();
				formCertEmployee.CertEmployee.IsNew=true;
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
			if(!_isHeadingPrinted) {
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