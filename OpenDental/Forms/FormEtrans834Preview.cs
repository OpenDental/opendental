using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormEtrans834Preview:FormODBase {

		private ODThread _thread=null;
		private X834 _x834;
		private List<PatientFor834Import> _listPatientLimiteds=null;
		private int _patNumCol;
		private int _sortedByColumnIdx;
		private bool _isSortAscending;

	public FormEtrans834Preview(X834 x834) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_x834=x834;
		}

		private void FormEtrans834Preview_Load(object sender,EventArgs e) {
			checkDropExistingIns.Checked=PrefC.GetBool(PrefName.Ins834DropExistingPatPlans);
			checkIsPatientCreate.Checked=PrefC.GetBool(PrefName.Ins834IsPatientCreate);
			checkIsEmployerCreate.Checked=PrefC.GetBool(PrefName.Ins834IsEmployerCreate);
			FillGridInsPlans();
		}

		///<summary>Shows current status to user in the progress label.  Useful for when processing for a few seconds or more.</summary>
		private void ShowStatus(string message) {
			labelProgress.Text=message;
			Application.DoEvents();
		}

		void FillGridInsPlans() {
			_sortedByColumnIdx=gridInsPlans.SortedByColumnIdx;
			_isSortAscending=gridInsPlans.SortedIsAscending;
			gridInsPlans.BeginUpdate();
			if(gridInsPlans.Columns.Count==0) {
				gridInsPlans.Columns.Clear();
				gridInsPlans.Columns.Add(new UI.GridColumn("Name",200,HorizontalAlignment.Left,UI.GridSortingStrategy.StringCompare));
				gridInsPlans.Columns.Add(new UI.GridColumn("Birthdate",74,HorizontalAlignment.Center,UI.GridSortingStrategy.DateParse));
				gridInsPlans.Columns.Add(new UI.GridColumn("SSN",66,HorizontalAlignment.Center,UI.GridSortingStrategy.StringCompare));
				_patNumCol=gridInsPlans.Columns.Count;
				gridInsPlans.Columns.Add(new UI.GridColumn("PatNum",68,HorizontalAlignment.Center,UI.GridSortingStrategy.StringCompare));
				gridInsPlans.Columns.Add(new UI.GridColumn("Date Begin",84,HorizontalAlignment.Center,UI.GridSortingStrategy.DateParse));
				gridInsPlans.Columns.Add(new UI.GridColumn("Date Term",84,HorizontalAlignment.Center,UI.GridSortingStrategy.DateParse));
				gridInsPlans.Columns.Add(new UI.GridColumn("Relation",70,HorizontalAlignment.Center,UI.GridSortingStrategy.StringCompare));
				gridInsPlans.Columns.Add(new UI.GridColumn("SubscriberID",96,HorizontalAlignment.Left,UI.GridSortingStrategy.StringCompare));
				gridInsPlans.Columns.Add(new UI.GridColumn("GroupNum",100,HorizontalAlignment.Left,UI.GridSortingStrategy.StringCompare));
				UI.GridColumn col=new UI.GridColumn("Payer",100,HorizontalAlignment.Left,UI.GridSortingStrategy.StringCompare);
				col.IsWidthDynamic=true;
				gridInsPlans.Columns.Add(col);
				_sortedByColumnIdx=0;//Sort by Patient Last Name by default.
				_isSortAscending=true;//Start with A and progress to Z.
			}
			gridInsPlans.EndUpdate();
			Application.DoEvents();//To show empty grid while the window is loading.
			if(_thread!=null) {
				_thread.QuitSync(0);
			}
			_thread=new ODThread(WorkerPreview834);
			_thread.Start();
		}

		private void WorkerPreview834(ODThread odThread) {
			Load834_Safe();
			odThread.QuitAsync();
		}

		///<summary>Call this from external thread. Invokes to main thread to avoid cross-thread collision.</summary>
		private void Load834_Safe() {
			try {
				this.BeginInvoke(() => Load834_Unsafe());
			}
			//most likely because form is no longer available to invoke to
			catch { }
		}

		private void Load834_Unsafe() {
			Cursor=Cursors.WaitCursor;
			ShowStatus("Loading patient information");
			const int previewLimitCount=40;
			gridInsPlans.BeginUpdate();
			gridInsPlans.ListGridRows.Clear();
			gridInsPlans.EndUpdate();
			Application.DoEvents();
			if(_listPatientLimiteds==null) {
				_listPatientLimiteds=Patients.GetAllPatsFor834Imports();//Testing this on an average sized database took about 1 second to run on a dev machine.
				_listPatientLimiteds.Sort();
			}
			int rowCount=0;
			for(int i=0;i<_x834.ListTransactions.Count;i++) {
				Hx834_Tran hx834_Tran=_x834.ListTransactions[i];
				for(int k=0;k<hx834_Tran.ListMembers.Count;k++) {
					rowCount++;
				}
			}
			for(int i=0;i<_x834.ListTransactions.Count;i++) {
				Hx834_Tran hx834_Tran=_x834.ListTransactions[i];
				for(int j=0;j<hx834_Tran.ListMembers.Count;j++) {
					Hx834_Member hx834_Member=hx834_Tran.ListMembers[j];
					ShowStatus("Loading "+(gridInsPlans.ListGridRows.Count+1).ToString().PadLeft(6)+"/"+rowCount.ToString().PadLeft(6)
						+"  Patient "+hx834_Member.Pat.GetNameLF());
					if(gridInsPlans.ListGridRows.Count < previewLimitCount) {
						gridInsPlans.BeginUpdate();
					}
					if(hx834_Member.ListHealthCoverage.Count==0) {
						UI.GridRow row=new UI.GridRow();
						gridInsPlans.ListGridRows.Add(row);
						FillGridRow(row,hx834_Member,null);
					}
					else {//There is at least one insurance plan.
						for(int a=0;a<hx834_Member.ListHealthCoverage.Count;a++) {
							Hx834_HealthCoverage hx834_healthCoverage=hx834_Member.ListHealthCoverage[a];
							UI.GridRow row=new UI.GridRow();
							gridInsPlans.ListGridRows.Add(row);
							FillGridRow(row,null,hx834_healthCoverage);
						}
					}
					if(gridInsPlans.ListGridRows.Count < previewLimitCount) {
						gridInsPlans.EndUpdate();//Also invalidates grid.
						Application.DoEvents();
					}
				}
			}
			gridInsPlans.BeginUpdate();
			gridInsPlans.SortForced(_sortedByColumnIdx,_isSortAscending);
			gridInsPlans.EndUpdate();//Also invalidates grid.
			ShowStatus("");
			Cursor=Cursors.Default;
			Application.DoEvents();
		}

		///<summary>The healthCoverage variable can be null.</summary>
		private void FillGridRow(UI.GridRow row,Hx834_Member hx834_Member,Hx834_HealthCoverage hx834_HealthCoverage) {
			row.Cells.Clear();
			if(hx834_HealthCoverage==null) {
				row.Tag=hx834_Member;
			}
			else {
				row.Tag=hx834_HealthCoverage;
				hx834_Member=hx834_HealthCoverage.Member;
			}
			row.Cells.Add(hx834_Member.Pat.GetNameLF());//Name
			if(hx834_Member.Pat.Birthdate.Year > 1880) {
				row.Cells.Add(hx834_Member.Pat.Birthdate.ToShortDateString());//Birthdate
			}
			else {
				row.Cells.Add("");//Birthdate
			}
			row.Cells.Add(hx834_Member.Pat.SSN);//SSN
			List<PatientFor834Import> listPatientMatches=PatientFor834Import.GetPatientLimitedsByNameAndBirthday(hx834_Member.Pat,_listPatientLimiteds);
			PatientFor834Import.FilterMatchingList(ref listPatientMatches);
			if(hx834_Member.Pat.PatNum==0 && listPatientMatches.Count==1) {
				hx834_Member.Pat.PatNum=listPatientMatches[0].PatNum;
			}
			if(hx834_Member.Pat.PatNum==0 && listPatientMatches.Count==0) {
				row.Cells.Add("");//PatNum
			}
			else if(hx834_Member.Pat.PatNum==0 && listPatientMatches.Count > 1) {
				row.Cells.Add("Multiple");//PatNum
			}
			else {//Either the patient was matched perfectly or the user chose the correct patient already.
				row.Cells.Add(hx834_Member.Pat.PatNum.ToString());//PatNum
			}
			if(hx834_HealthCoverage!=null && hx834_HealthCoverage.DateEffective.Year > 1880) {
				row.Cells.Add(hx834_HealthCoverage.DateEffective.ToShortDateString());//Date Begin
			}
			else {
				row.Cells.Add("");//Date Begin
			}
			if(hx834_HealthCoverage!=null && hx834_HealthCoverage.DateTerm.Year > 1880) {
				row.Cells.Add(hx834_HealthCoverage.DateTerm.ToShortDateString());//Date Term
			}
			else {
				row.Cells.Add("");//Date Term
			}
			row.Cells.Add(hx834_Member.PlanRelat.ToString());//Relation
			row.Cells.Add(hx834_Member.SubscriberId);//SubscriberID
			row.Cells.Add(hx834_Member.GroupNum);//GroupNum
			row.Cells.Add(hx834_Member.Tran.Payer.Name);//Payer
		}

		private void gridInsPlans_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			Hx834_Member hx834_Member=null;
			Hx834_HealthCoverage hx834_HealthCoverage=null;
			if(gridInsPlans.ListGridRows[e.Row].Tag is Hx834_Member) {
				hx834_Member=(Hx834_Member)gridInsPlans.ListGridRows[e.Row].Tag;
			}
			else {
				hx834_HealthCoverage=(Hx834_HealthCoverage)gridInsPlans.ListGridRows[e.Row].Tag;
				hx834_Member=hx834_HealthCoverage.Member;
			}
			using FormPatientSelect formPatientSelect=new FormPatientSelect(hx834_Member.Pat);
			if(formPatientSelect.ShowDialog()==DialogResult.OK) {
				hx834_Member.Pat.PatNum=formPatientSelect.SelectedPatNum;
				gridInsPlans.BeginUpdate();
				//Refresh all rows for this member to show the newly selected PatNum.
				//There will be multiple rows if there are multiple insurance plans for the member.
				for(int i=0;i<gridInsPlans.ListGridRows.Count;i++) {
					Hx834_Member hx834_MemberRefresh=null;
					if(gridInsPlans.ListGridRows[i].Tag is Hx834_Member) {
						hx834_MemberRefresh=(Hx834_Member)gridInsPlans.ListGridRows[i].Tag;
					}
					else {
						hx834_MemberRefresh=((Hx834_HealthCoverage)gridInsPlans.ListGridRows[i].Tag).Member;
					}
					if(hx834_MemberRefresh==hx834_Member) {
						FillGridRow(gridInsPlans.ListGridRows[e.Row],hx834_Member,hx834_HealthCoverage);
					}
				}
				gridInsPlans.EndUpdate();
			}
		}

		///<summary>Tries to import the 834. Will return false if the user cancelled out of importing.</summary>
		private bool TryImport834() {
			if(checkDropExistingIns.Checked 
				&& !MsgBox.Show(MsgBoxButtons.YesNo,"Insurance plans for patients will be automatically replaced with the new plans. Continue?"))
			{
				return false;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Importing insurance plans is a database intensive operation and can take 10 minutes or more to run.  "
				+"It is best to import insurance plans after hours or during another time period when database usage is otherwise low.\r\n"
				+"Click OK to import insurance plans now, or click Cancel."))
			{
				return false;
			}
			checkIsPatientCreate.Enabled=false;
			checkIsEmployerCreate.Enabled=false;
			checkDropExistingIns.Enabled=false;
			gridInsPlans.Enabled=false;
			butOK.Enabled=false;
			butCancel.Enabled=false;
			Cursor=Cursors.WaitCursor;
			Prefs.UpdateBool(PrefName.Ins834DropExistingPatPlans,checkDropExistingIns.Checked);
			Prefs.UpdateBool(PrefName.Ins834IsPatientCreate,checkIsPatientCreate.Checked);
			Prefs.UpdateBool(PrefName.Ins834IsEmployerCreate,checkIsEmployerCreate.Checked);
			//Create all of our count variables.
			int createdPatsCount,updatedPatsCount,skippedPatsCount,createdCarrierCount,createdInsPlanCount,updatedInsPlanCount,createdInsSubCount,
				updatedInsSubCount,createdPatPlanCount,droppedPatPlanCount,updatedPatPlanCount,createdEmployerCount;
			StringBuilder stringBuilderErrMsgs;
			EtransL.ImportInsurancePlansUsingPatientLimited(_x834,_listPatientLimiteds,checkIsPatientCreate.Checked,checkIsEmployerCreate.Checked,checkDropExistingIns.Checked,out createdPatsCount,
				out updatedPatsCount,out skippedPatsCount,out createdCarrierCount,out createdInsPlanCount,out updatedInsPlanCount,out createdInsSubCount,
				out updatedInsSubCount,out createdPatPlanCount,out droppedPatPlanCount,out updatedPatPlanCount,out createdEmployerCount,out stringBuilderErrMsgs,(rowIndex,pat) => {
				ShowStatus("Progress "+(rowIndex).ToString().PadLeft(6)+"/"+gridInsPlans.ListGridRows.Count.ToString().PadLeft(6)
					+"  Importing plans for patient "+pat.GetNameLF());
			});
			Cursor=Cursors.Default;
			#region summary output
			string msg=Lan.g(this,"Done.");
			if(createdPatsCount > 0) {
				msg+="\r\n"+Lan.g(this,"Number of patients created:")+" "+createdPatsCount;
			}
			if(updatedPatsCount > 0) {
				msg+="\r\n"+Lan.g(this,"Number of patients updated:")+" "+updatedPatsCount;
			}
			if(skippedPatsCount > 0) {
				msg+="\r\n"+Lan.g(this,"Number of patients skipped:")+" "+skippedPatsCount;
				msg+=stringBuilderErrMsgs.ToString();
			}
			if(createdCarrierCount > 0) {
				msg+="\r\n"+Lan.g(this,"Number of carriers created:")+" "+createdCarrierCount;
				msg+=stringBuilderErrMsgs.ToString();
			}
			if(createdInsPlanCount > 0) {
				msg+="\r\n"+Lan.g(this,"Number of insurance plans created:")+" "+createdInsPlanCount;
				msg+=stringBuilderErrMsgs.ToString();
			}
			if(updatedInsPlanCount > 0) {
				msg+="\r\n"+Lan.g(this,"Number of insurance plan updates:")+" "+updatedInsPlanCount;
				msg+=stringBuilderErrMsgs.ToString();
			}
			if(createdInsSubCount > 0) {
				msg+="\r\n"+Lan.g(this,"Number of insurance subscriptions created:")+" "+createdInsSubCount;
				msg+=stringBuilderErrMsgs.ToString();
			}
			if(updatedInsSubCount > 0) {
				msg+="\r\n"+Lan.g(this,"Number of insurance subscriptions updated:")+" "+updatedInsSubCount;
				msg+=stringBuilderErrMsgs.ToString();
			}
			if(createdPatPlanCount > 0) {
				msg+="\r\n"+Lan.g(this,"Number of patients added to insurance:")+" "+createdPatPlanCount;
				msg+=stringBuilderErrMsgs.ToString();
			}
			if(droppedPatPlanCount > 0) {
				msg+="\r\n"+Lan.g(this,"Number of patients dropped from insurance:")+" "+droppedPatPlanCount;
				msg+=stringBuilderErrMsgs.ToString();
			}
			if(updatedPatPlanCount > 0) {
				msg+="\r\n"+Lan.g(this,"Number of patients insurance information updated:")+" "+updatedPatPlanCount;
				msg+=stringBuilderErrMsgs.ToString();
			}
			if(createdEmployerCount > 0) {
				msg+="\r\n"+Lan.g(this,"Number of employers created:")+" "+createdEmployerCount;
				msg+=stringBuilderErrMsgs.ToString();
			}
			#endregion summary output
			using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(msg);
			msgBoxCopyPaste.ShowDialog();
			return true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!TryImport834()){
				return;
			}
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

	}

}