using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness; 
using CodeBase;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormClinics:FormODBase {

		#region Private Variables
		///<summary>Set to true to open the form in selection mode. This will cause the 'Show hidden' checkbox to be hidden.</summary>
		public bool IsSelectionMode;
		public long ClinicNumSelected;
		///<summary>Set this list prior to loading this window to use a custom list of clinics.  Otherwise, uses the cache.</summary>
		public List<Clinic> ListClinics;
		///<summary>This list will be a copy of ListClinics and is used for syncing on window closed.</summary>
		private List<Clinic> _listClinicsOld;
		private long _clinicNumTo=-1;
		///<summary>Set to true prior to loading to include a 'Headquarters' option.</summary>
		public bool DoIncludeHQInList;
		private List<Clinics.ClinicCount> _listClinicCounts;
		private List<DefLink> _listDefLinksSpecialties;
		private bool _clinicChanged;
		///<summary>Pass in a list of clinics that should be pre-selected. 
		///When this form is closed, this list will be the list of clinics that the user selected.</summary>
		public List<long> ListClinicNumsSelected=new List<long>();
		///<summary>Set to true if the user can select multiple clinics.</summary>
		public bool IsMultiSelect;
		#endregion

		///<summary></summary>
		public FormClinics()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormClinics_Load(object sender, System.EventArgs e) {
			checkOrderAlphabetical.Checked=PrefC.GetBool(PrefName.ClinicListIsAlphabetical);
			if(ListClinics==null) {
				ListClinics=Clinics.GetAllForUserod(Security.CurUser);
				if(DoIncludeHQInList) {
					ListClinics.Insert(0,new Clinic() { ClinicNum=0,Description=Lan.g(this,"Headquarters"),Abbr=Lan.g(this,"HQ") });
				}
				//if alphabetical checkbox is checked/unchecked it triggers a pref cache refresh, but does not refill the clinic cache, so we need to sort here
				//in case the pref was changed since last time this was opened.
				ListClinics.Sort(ClinicSort);
			}
			_listClinicsOld=ListClinics.Select(x => x.Copy()).ToList();
			_listClinicCounts=new List<Clinics.ClinicCount>();
			//_listDefLinksClinicsAllOld=DefLinks.GetDefLinksByType(DefLinkType.Clinic);
			_clinicChanged=false;
			if(IsSelectionMode) {
				butAdd.Visible=false;
				groupClinicOrder.Visible=false;
				groupMovePats.Visible=false;
				checkShowHidden.Visible=false;
				checkShowHidden.Checked=false;
			}
			else {
				if(checkOrderAlphabetical.Checked) {
					butUp.Enabled=false;
					butDown.Enabled=false;
				}
				_listClinicCounts=Clinics.GetListClinicPatientCount();
				butSave.Visible=false;
			}
			if(IsMultiSelect) {
				butSelectAll.Visible=true;
				butSelectNone.Visible=true;
				gridMain.SelectionMode=GridSelectionMode.MultiExtended;
			}
			FillGrid(false);
			if(!ListClinicNumsSelected.IsNullOrEmpty()) {
				for(int i=0;i<gridMain.ListGridRows.Count;i++) {
					if(ListClinicNumsSelected.Contains(((Clinic)gridMain.ListGridRows[i].Tag).ClinicNum)) {
						gridMain.SetSelected(i,true);
					}
				}
			}
		}

		private void FillGrid(bool doReselctRows=true) {
			List<long> listClinicNumsSelected=new List<long>();
			if(doReselctRows) {
				listClinicNumsSelected=gridMain.SelectedTags<Clinic>().Select(x => x.ClinicNum).ToList();
			}
			_listDefLinksSpecialties=DefLinks.GetDefLinksByType(DefLinkType.ClinicSpecialty);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClinics","Abbr"),120));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClinics","Description"),200));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClinics","Specialty"),150));
			if(!IsSelectionMode) {
				gridMain.Columns.Add(new GridColumn(Lan.g("TableClinics","Pat Count"),80,HorizontalAlignment.Center));
				gridMain.Columns.Add(new GridColumn(Lan.g("TableClinics","Hidden"),40,HorizontalAlignment.Center){ IsWidthDynamic=true });
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			List<int> listIndicesToReselect=new List<int>();
			for(int i=0;i<ListClinics.Count;i++) {
				if(!checkShowHidden.Checked && ListClinics[i].IsHidden) {
					continue;
				}
				row=new GridRow();
				row.Cells.Add((ListClinics[i].ClinicNum==0?"":ListClinics[i].Abbr));
				row.Cells.Add(ListClinics[i].Description);
				List<long> listDefNums=_listDefLinksSpecialties.FindAll(x => x.FKey==ListClinics[i].ClinicNum).Select(x=> x.DefNum).ToList();
				List<string> listDescripts=Defs.GetDefs(DefCat.ClinicSpecialty,listDefNums)
					.Where(x=>!string.IsNullOrWhiteSpace(x.ItemName))
					.Select(x=>x.ItemName).ToList();
				string specialties=string.Join(",",listDescripts);
				row.Cells.Add(specialties);
				if(!IsSelectionMode) {//selection mode means no IsHidden or Pat Count columns
					Clinics.ClinicCount clinicCount=_listClinicCounts.FirstOrDefault(x=>x.ClinicNum==ListClinics[i].ClinicNum);
					if(clinicCount is null) {
						row.Cells.Add("0");
					}
					else {
						row.Cells.Add(POut.Int(clinicCount.Count));
					}
					row.Cells.Add(ListClinics[i].IsHidden?"X":"");
				}
				row.Tag=ListClinics[i];
				gridMain.ListGridRows.Add(row);
				if(listClinicNumsSelected.Contains(ListClinics[i].ClinicNum)) {
					listIndicesToReselect.Add(gridMain.ListGridRows.Count-1);
				}
			}
			gridMain.EndUpdate();
			if(doReselctRows && listIndicesToReselect.Count>0) {
				gridMain.SetSelected(listIndicesToReselect.ToArray(),true);
			}
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.ClinicEdit)) {
				return;
			}
			Clinic clinic=new Clinic();
			clinic.IsNew=true;
			if(PrefC.GetBool(PrefName.PracticeIsMedicalOnly)) {
				clinic.IsMedicalOnly=true;
			}
			clinic.ItemOrder=gridMain.ListGridRows.Count-(DoIncludeHQInList?1:0);//Set it last in the last position (minus 1 for HQ)
			using FormClinicEdit formClinicEdit=new FormClinicEdit(clinic,ListClinics);
			if(formClinicEdit.ShowDialog()==DialogResult.OK) {
				clinic.ClinicNum=Clinics.Insert(clinic);//inserting this here so we have a ClinicNum; the user cannot cancel and undo this anyway
				ListClinics.Add(clinic);
				_listClinicsOld.Add(clinic.Copy());//add to both lists so the sync doesn't try to insert it again or delete it.
				Clinics.ClinicCount clinicCount=new Clinics.ClinicCount();
				clinicCount.ClinicNum=clinic.ClinicNum;
				clinicCount.Count=0;
				_listClinicCounts.Add(clinicCount);
				ListClinics.Sort(ClinicSort);
				formClinicEdit.ListDefLinksSpecialties.ForEach(x => x.FKey=clinic.ClinicNum);//Change ClinicNum to match inserted clinic before Inserting
				formClinicEdit.ListDefLinksSpecialties.ForEach(x => DefLinks.Insert(x));
				_clinicChanged=true;
			}
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!IsSelectionMode && !Security.IsAuthorized(EnumPermType.ClinicEdit)) {
				return;
			}
			if(gridMain.ListGridRows.Count==0){
				return;
			}
			if(IsSelectionMode) {
				ClinicNumSelected=((Clinic)gridMain.ListGridRows[e.Row].Tag).ClinicNum;
				DialogResult=DialogResult.OK;
				return;
			}
			if(DoIncludeHQInList && e.Row==0) {
				return;
			}
			Clinic clinicOld=(Clinic)gridMain.ListGridRows[e.Row].Tag;
			using FormClinicEdit formClinicEdit=new FormClinicEdit(clinicOld.Copy(),ListClinics);
			if(formClinicEdit.ShowDialog()==DialogResult.OK) {
				Clinic clinicNew=formClinicEdit.ClinicCur;
				if(clinicNew==null) {//Clinic was deleted
					//Fix ItemOrders
					ListClinics.FindAll(x => x.ItemOrder>clinicOld.ItemOrder)
						.ForEach(x => x.ItemOrder--);
					ListClinics.Remove(clinicOld);
					_clinicChanged=true;
				}
				else {
					List<DefLink> listDefLinksOldSpecialties=DefLinks.GetDefLinksForClinicSpecialties(clinicOld.ClinicNum);
					formClinicEdit.ListDefLinksSpecialties.ForEach(x => x.FKey=clinicNew.ClinicNum);//Update to inserted ClinicNum
					//Delete DefLinks that have been removed
					for(int i=0;i<listDefLinksOldSpecialties.Count;i++) {
						if(formClinicEdit.ListDefLinksSpecialties.Select(x=>x.DefNum).Contains(listDefLinksOldSpecialties[i].DefNum)) {
							continue;
						}
						DefLinks.Delete(listDefLinksOldSpecialties[i].DefLinkNum);
						_clinicChanged=true;
					}
					//Insert DefLinks that aren't already in database
					for(int i=0;i<formClinicEdit.ListDefLinksSpecialties.Count;i++) {
						if(listDefLinksOldSpecialties.Select(x=>x.DefNum).Contains(formClinicEdit.ListDefLinksSpecialties[i].DefNum)) {
							continue;
						}
						DefLinks.Insert(formClinicEdit.ListDefLinksSpecialties[i]);
						_clinicChanged=true;
					}
					ListClinics[ListClinics.IndexOf(clinicOld)]=clinicNew;
				}
			}
			FillGrid();			
		}
		
		private void butClinicPick_Click(object sender,EventArgs e) {
			List<Clinic> listClinics=gridMain.GetTags<Clinic>();
			using FormClinics formClinics=new FormClinics();
			formClinics.ListClinics=listClinics;
			formClinics.IsSelectionMode=true;
			if(formClinics.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_clinicNumTo=formClinics.ClinicNumSelected;
			textMoveTo.Text=(listClinics.FirstOrDefault(x => x.ClinicNum==_clinicNumTo)?.Abbr??"");
		}

		private void butMovePats_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length<1) {
				MsgBox.Show(this,"You must select at least one clinic to move patients from.");
				return;
			}
			if(_clinicNumTo==-1){
				MsgBox.Show(this,"You must pick a 'To' clinic in the box above to move patients to.");
				return;
			}
			List<Clinic> listClinicsFrom=gridMain.SelectedTags<Clinic>();
			Clinic clinicTo=gridMain.GetTags<Clinic>().FirstOrDefault(x => x.ClinicNum==_clinicNumTo);
			if(clinicTo==null) {
				MsgBox.Show(this,"The clinic could not be found.");
				return;
			}
			if(listClinicsFrom.Any(x=>x.ClinicNum==clinicTo.ClinicNum)){
				MsgBox.Show(this,"The 'To' clinic should not also be one of the 'From' clinics.");
				return;
			}
			List<Clinics.ClinicCount> listClinicCountsAll=Clinics.GetListClinicPatientCount(true);
			List<Clinics.ClinicCount> listClinicCountsSelected=listClinicCountsAll.FindAll(x=>listClinicsFrom.Any(y=>y.ClinicNum==x.ClinicNum));
			if(listClinicCountsSelected.Sum(x => x.Count)==0) {
				MsgBox.Show(this,"There are no patients assigned to the selected clinics.");
				return;
			}
			string msg=Lan.g(this,"This will move all patients to")+" "+clinicTo.Abbr+" "+Lan.g(this,"from the following clinics")+":\r\n"
				+string.Join("\r\n",listClinicsFrom.Select(x => x.Abbr))+"\r\n"+Lan.g(this,"Continue?");
			if(MessageBox.Show(msg,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
				return;
			}
			ProgressWin progressOD=new ProgressWin();
			progressOD.ActionMain=() => { 
				int patsMoved=0;
				int countTotal=listClinicCountsSelected.Sum(x => x.Count);
				List<Action> listActions=listClinicCountsSelected.Select(x => new Action(() => {
					Patients.ChangeClinicsForAll(x.ClinicNum,clinicTo.ClinicNum);//update all clinicNums to newClinic
					SecurityLogs.MakeLogEntry(EnumPermType.PatientEdit,0,"Clinic changed for "+x.Count.ToString()+" patients from "
						+Clinics.GetAbbr(x.ClinicNum)+" to "+clinicTo.Abbr+".");
					patsMoved+=x.Count;
					ODEvent.Fire(ODEventType.ProgressBar,Lan.g(this,"Moved patients")+": "+patsMoved+" "+Lan.g(this,"out of total")+" "
						+countTotal.ToString());
				})).ToList();
				ODThread.RunParallel(listActions,TimeSpan.FromMinutes(2));
			};
			progressOD.StartingMessage=Lan.g(this,"Moving patients")+"...";
			progressOD.TestSleep=true;
			progressOD.ShowDialog();
			_listClinicCounts=Clinics.GetListClinicPatientCount();
			FillGrid();
			if(progressOD.IsCancelled){
				return;//we already refreshed in the lines above, just in case some patients had been moved.
			}
			MsgBox.Show(this,"Done");
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a clinic first.");
				return;
			}
			int selectedIdx=gridMain.GetSelectedIndex();
			//Already at the top of the list or the clinic just below the HQ 'clinic' is selected, moving up does nothing
			if(selectedIdx==0 || (DoIncludeHQInList && selectedIdx==1)) {
				return;
			}
			//Swap clinic ItemOrders
			Clinic clinicSource=((Clinic)gridMain.ListGridRows[selectedIdx].Tag);
			Clinic clinicDest=((Clinic)gridMain.ListGridRows[selectedIdx-1].Tag);
			if(clinicSource.ItemOrder==clinicDest.ItemOrder) {
				clinicSource.ItemOrder--;
			}
			else {
				int sourceOrder=clinicSource.ItemOrder;
				clinicSource.ItemOrder=clinicDest.ItemOrder;
				clinicDest.ItemOrder=sourceOrder;
			}
			//Move selected clinic up
			ListClinics.Sort(ClinicSort);
			FillGrid();
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a clinic first.");
				return;
			}
			int selectedIdx=gridMain.GetSelectedIndex();
			//Already at the bottom of the list or the HQ 'clinic' is selected, moving down does nothing
			if(selectedIdx==gridMain.ListGridRows.Count-1 || (DoIncludeHQInList && selectedIdx==0)) {
				return;
			}
			//Swap clinic ItemOrders
			Clinic clinicSource=((Clinic)gridMain.ListGridRows[selectedIdx].Tag);
			Clinic clinicDest=((Clinic)gridMain.ListGridRows[selectedIdx+1].Tag);
			if(clinicSource.ItemOrder==clinicDest.ItemOrder) {//just in case, an issue in the past would cause ItemOrder inconsitencies, shouldn't happen
				clinicSource.ItemOrder++;
			}
			else {
				int sourceOrder=clinicSource.ItemOrder;
				clinicSource.ItemOrder=clinicDest.ItemOrder;
				clinicDest.ItemOrder=sourceOrder;
			}
			//Move selected clinic down
			ListClinics.Sort(ClinicSort);
			FillGrid();
		}

		private void checkOrderAlphabetical_Click(object sender,EventArgs e) {
			if(checkOrderAlphabetical.Checked) {
				butUp.Enabled=false;
				butDown.Enabled=false;
			}
			else {
				butUp.Enabled=true;
				butDown.Enabled=true;
			}
			ListClinics.Sort(ClinicSort);//Sorts based on the status of the checkbox.
			FillGrid();
		}

		private int ClinicSort(Clinic x,Clinic y) {
			if(DoIncludeHQInList) {//always keep the HQ clinic at the top if it's in the list
				if(x.ClinicNum==0) {
					return -1;
				}
				if(y.ClinicNum==0) {
					return 1;
				}
			}
			int retval=0;
			if(checkOrderAlphabetical.Checked) {//order alphabetical by Abbr
				retval=x.Abbr.CompareTo(y.Abbr);
			}
			else {//not alphabetical, order by ItemOrder
				retval=x.ItemOrder.CompareTo(y.ItemOrder);
			}
			if(retval==0) {//if Abbr's are alphabetically the same or ItemOrder's are the same, order alphabetical by Description
				retval=x.Description.CompareTo(y.Description);
			}
			if(retval==0) {//if Abbrs/ItemOrders are the same and Descriptions are alphabetically the same, order by ClinicNum (guaranteed deterministic)
				retval=x.ClinicNum.CompareTo(y.ClinicNum);
			}
			return retval;
		}

		///<summary>Does nothing and returns false if checkOrderAlphabetical is checked.  Uses ClinicSort to put the clinics in the correct order and then
		///updates the ItemOrder for all clinics.  Includes hidden clinics and clinics the user does not have permission to access.  It must include all
		///clinics for the ordering to be correct for all users.  This method corrects item ordering issues that were caused by past code and is just a
		///precaution.  After this runs once, there shouldn't be any ItemOrder inconsistencies moving forward, so this should generally just return false.
		///Returns true if the db was changed.</summary>
		private bool CorrectItemOrders() {
			if(checkOrderAlphabetical.Checked) {
				return false;
			}
			List<Clinic> listClinicsAllDbs=Clinics.GetClinicsNoCache();//get all clinics, even hidden ones, in order to set the ItemOrders correctly
			List<Clinic> listClinicsAllNew=listClinicsAllDbs.Select(x => x.Copy()).ToList();
			bool isHqInList=DoIncludeHQInList;
			DoIncludeHQInList=false;
			listClinicsAllNew.Sort(ClinicSort);
			DoIncludeHQInList=isHqInList;
			for(int i=0;i<listClinicsAllNew.Count;i++) {
				listClinicsAllNew[i].ItemOrder=i+1;//1 based ItemOrder because the HQ 'clinic' has ItemOrder 0
			}
			return Clinics.Sync(listClinicsAllNew,listClinicsAllDbs);
		}

		private void checkShowHidden_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void butSelectAll_Click(object sender,EventArgs e) {
			gridMain.SetAll(true);
			gridMain.Focus();//Allows user to use ODGrid CTRL functionality.
		}

		private void butSelectNone_Click(object sender,EventArgs e) {
			gridMain.SetAll(false);
			gridMain.Focus();//Allows user to use ODGrid CTRL functionality.
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(IsSelectionMode && gridMain.SelectedIndices.Length>0) {
				ClinicNumSelected=gridMain.SelectedTag<Clinic>()?.ClinicNum??0;
				ListClinicNumsSelected=gridMain.SelectedTags<Clinic>().Select(x => x.ClinicNum).ToList();
				DialogResult=DialogResult.OK;
			}
			Close();
		}

		private void FormClinics_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DialogResult==DialogResult.Cancel){
				ClinicNumSelected=0;
				ListClinicNumsSelected=new List<long>();
			}
			if(IsSelectionMode) {
				return;
			}
			if(Prefs.UpdateBool(PrefName.ClinicListIsAlphabetical,checkOrderAlphabetical.Checked)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			_clinicChanged|=Clinics.Sync(ListClinics,_listClinicsOld);//returns true if clinics were updated/inserted/deleted
			_clinicChanged|=CorrectItemOrders();
			//Joe - Now that we have called sync on ListClinics we want to make sure that each clinic has program properties for PayConnect and XCharge
			//We are doing this because of a previous bug that caused some customers to have over 3.4 million duplicate rows in their programproperty table
			long payConnectProgNum=Programs.GetProgramNum(ProgramName.PayConnect);
			long xChargeProgNum=Programs.GetProgramNum(ProgramName.Xcharge);
			//Don't need to do this for PaySimple, because these will get generated as needed in FormPaySimpleSetup
			bool hasProgramChanges=ProgramProperties.InsertForClinic(payConnectProgNum,
				ListClinics.Select(x => x.ClinicNum).Where(x => ProgramProperties.GetListForProgramAndClinic(payConnectProgNum,x).Count==0).ToList());
			hasProgramChanges|=ProgramProperties.InsertForClinic(xChargeProgNum,
				ListClinics.Select(x => x.ClinicNum).Where(x => ProgramProperties.GetListForProgramAndClinic(xChargeProgNum,x).Count==0).ToList());
			if(hasProgramChanges) {
				DataValid.SetInvalid(InvalidType.Programs);
			}
			if(_clinicChanged) {
				DataValid.SetInvalid(InvalidType.Providers);
			}
		}

	}
}