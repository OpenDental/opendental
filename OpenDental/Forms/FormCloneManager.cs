using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary></summary>
	public partial class FormCloneManager:FormODBase {
		//private Patient PatCur;
		private List<Patient> _listPatClones;
		private bool _isSelected; //currently not necessary since using inversion method. 
		///<summary>Invoke this action in order to close the patient clone progress window.</summary>
		private Action _actionCloseCloneFixProgress=null;
		/// <summary></summary>
		public FormCloneManager() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
	
		}

		private void FormCloneFix_Load(object sender,EventArgs e) {
			//Jason to add code here
			//FillGrids(); //remove after debugging.
		}

		private void FillGrids() {
			_listPatClones=Patients.GetAllPatients(); //change this to get a list of all patients WITH possible clones
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			List<DisplayField> fields=DisplayFields.GetForCategory(DisplayFieldCategory.PatientInformation);
			col=new GridColumn("First Name",115);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Last Name",115);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Middle",65);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Gender",65);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Birthdate",75);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("PriProv",135);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("SecProv",135);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listPatClones.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listPatClones[i].FName.ToString());
				row.Cells.Add(_listPatClones[i].LName.ToString());
				row.Cells.Add(_listPatClones[i].MiddleI.ToString());
				row.Cells.Add(_listPatClones[i].Gender.ToString());
				row.Cells.Add(_listPatClones[i].Birthdate.ToShortDateString());
				row.Cells.Add(Providers.GetLongDesc(Patients.GetProvNum(_listPatClones[i])));
				row.Cells.Add(Providers.GetLongDesc(_listPatClones[i].SecProv));
				row.Tag=_listPatClones[i];
				gridMain.ListGridRows.Add(row);	
			}
			gridMain.EndUpdate();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrids();
			//create new ODEvent Class - no idea what's currently happening here. 
			//_actionCloseCloneFixProgress=ODProgress.ShowProgressStatus("CloneFixEvent"
			//	,typeof(CloneFixEvent)
			//	,tag: new ProgressBarHelper(Lan.g(this,"Running Clone Fix")+"...",null,0,100,ProgBarStyle.Marquee,"Header")); //tag is what consumer needs. What is needed here? 
			//ODEvent.Fire(new ODEventArgs("CloneFixEvent",Lan.g("Prefs","Removing old update files...")));
			//_actionCloseCloneFixProgress?.Invoke();
		}

		private void butRun_Click(object sender,EventArgs e) {
			//runs logic to add all clone entries that are associated with their 'parent' into PatientCloneLink table.
			//get all selected rows and insert them into the patientclonelink table.
			for(int i = 0;i<gridMain.SelectedIndices.Count();i++) { //Jason just wants it to loop through selected indices.
				if(true) {//gridMain.Rows.Contains(gridMain.Rows[gridMain.SelectedIndices[i]].Tag)) {
					continue;
				}
				else {
					//insert into patient clone link
				}
			}
			//create new ODEvent Class
			//_actionCloseCloneFixProgress=ODProgress.ShowProgressStatus("CloneFixEvent"
			//	,typeof(CloneFixEvent)
			//	,tag: new ProgressBarHelper(Lan.g(this,"Running Clone Fix")+"...",null,0,100,ProgBarStyle.Marquee,"Header"));
			//ODEvent.Fire(new ODEventArgs("CloneFixEvent",Lan.g("Prefs","Removing old update files...")));
			//_actionCloseCloneFixProgress?.Invoke();			
		}

		private void InvertCurSelected(int index) {
			bool isSelected=gridMain.SelectedIndices.Contains(index);
			gridMain.SetSelected(index,!isSelected);//Invert selection.
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			InvertCurSelected(e.Row);
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormCloneFix_FormClosing(object sender,FormClosingEventArgs e) {

		}
	}
}