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
		public long SelectedClinicNum;
		///<summary>Set this list prior to loading this window to use a custom list of clinics.  Otherwise, uses the cache.</summary>
		public List<Clinic> ListClinics;
		///<summary>This list will be a copy of ListClinics and is used for syncing on window closed.</summary>
		private List<Clinic> _listClinicsOld;
		private long _clinicNumTo=-1;
		///<summary>Set to true prior to loading to include a 'Headquarters' option.</summary>
		public bool IncludeHQInList;
		private SerializableDictionary<long,int> _dictClinicalCounts;
		private List<DefLink> _listClinicDefLinksAllOld;
		///<summary>Pass in a list of clinics that should be pre-selected. 
		///When this form is closed, this list will be the list of clinics that the user selected.</summary>
		public List<long> ListSelectedClinicNums = new List<long>();
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
				if(IncludeHQInList) {
					ListClinics.Insert(0,new Clinic() { ClinicNum=0,Description=Lan.g(this,"Headquarters"),Abbr=Lan.g(this,"HQ") });
				}
				//if alphabetical checkbox is checked/unchecked it triggers a pref cache refresh, but does not refill the clinic cache, so we need to sort here
				//in case the pref was changed since last time this was opened.
				ListClinics.Sort(ClinicSort);
			}
			_listClinicsOld=ListClinics.Select(x => x.Copy()).ToList();
			_listClinicDefLinksAllOld=DefLinks.GetDefLinksByType(DefLinkType.Clinic);
			_dictClinicalCounts=new SerializableDictionary<long,int>();
			if(IsSelectionMode) {
				butAdd.Visible=false;
				butOK.Visible=true;
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
				_dictClinicalCounts=Clinics.GetClinicalPatientCount();
			}
			if(IsMultiSelect) {
				butSelectAll.Visible=true;
				butSelectNone.Visible=true;
				gridMain.SelectionMode=GridSelectionMode.MultiExtended;
			}
			FillGrid(false);
			if(!ListSelectedClinicNums.IsNullOrEmpty()) {
				for(int i=0;i<gridMain.ListGridRows.Count;i++) {
					if(ListSelectedClinicNums.Contains(((Clinic)gridMain.ListGridRows[i].Tag).ClinicNum)) {
						gridMain.SetSelected(i,true);
					}
				}
			}
		}

		private void FillGrid(bool doReselctRows=true) {
			List<long> listSelectedClinicNums=new List<long>();
			if(doReselctRows) {
				listSelectedClinicNums=gridMain.SelectedTags<Clinic>().Select(x => x.ClinicNum).ToList();
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableClinics","Abbr"),120));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableClinics","Description"),200));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableClinics","Specialty"),150));
			if(!IsSelectionMode) {
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableClinics","Pat Count"),80,HorizontalAlignment.Center));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableClinics","Hidden"),40,HorizontalAlignment.Center){ IsWidthDynamic=true });
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			Dictionary<long,string> dictClinicSpecialtyDescripts=Defs.GetDefsForCategory(DefCat.ClinicSpecialty).ToDictionary(x => x.DefNum,x => x.ItemName);
			List<int> listIndicesToReselect=new List<int>();
			foreach(Clinic clinCur in ListClinics) {
				if(!checkShowHidden.Checked && clinCur.IsHidden) {
					continue;
				}
				row=new GridRow();
				row.Cells.Add((clinCur.ClinicNum==0?"":clinCur.Abbr));
				row.Cells.Add(clinCur.Description);
				string specialty="";
				string specialties=string.Join(",",clinCur.ListClinicSpecialtyDefLinks
					.Select(x => dictClinicSpecialtyDescripts.TryGetValue(x.DefNum,out specialty)?specialty:"")
					.Where(x => !string.IsNullOrWhiteSpace(x)));
				row.Cells.Add(specialties);
				if(!IsSelectionMode) {//selection mode means no IsHidden or Pat Count columns
					int patCount=0;
					_dictClinicalCounts.TryGetValue(clinCur.ClinicNum,out patCount);
					row.Cells.Add(POut.Int(patCount));
					row.Cells.Add(clinCur.IsHidden?"X":"");
				}
				row.Tag=clinCur;
				gridMain.ListGridRows.Add(row);
				if(listSelectedClinicNums.Contains(clinCur.ClinicNum)) {
					listIndicesToReselect.Add(gridMain.ListGridRows.Count-1);
				}
			}
			gridMain.EndUpdate();
			if(doReselctRows && listIndicesToReselect.Count>0) {
				gridMain.SetSelected(listIndicesToReselect.ToArray(),true);
			}
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			Clinic clinicCur=new Clinic();
			clinicCur.IsNew=true;
			if(PrefC.GetBool(PrefName.PracticeIsMedicalOnly)) {
				clinicCur.IsMedicalOnly=true;
			}
			clinicCur.ItemOrder=gridMain.ListGridRows.Count-(IncludeHQInList?1:0);//Set it last in the last position (minus 1 for HQ)
			using FormClinicEdit FormCE=new FormClinicEdit(clinicCur);
			if(FormCE.ShowDialog()==DialogResult.OK) {
				clinicCur.ClinicNum=Clinics.Insert(clinicCur);//inserting this here so we have a ClinicNum; the user cannot cancel and undo this anyway
				//ClinicCur.ListClinicSpecialtyDefLinks FKeys are set in FormClosing to ClinicCur.ClinicNum
				ListClinics.Add(clinicCur);
				_listClinicsOld.Add(clinicCur.Copy());//add to both lists so the sync doesn't try to insert it again or delete it.
				_dictClinicalCounts[clinicCur.ClinicNum]=0;
				ListClinics.Sort(ClinicSort);
			}
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridMain.ListGridRows.Count==0){
				return;
			}
			if(IsSelectionMode) {
				SelectedClinicNum=((Clinic)gridMain.ListGridRows[e.Row].Tag).ClinicNum;
				DialogResult=DialogResult.OK;
				return;
			}
			if(IncludeHQInList && e.Row==0) {
				return;
			}
			Clinic clinicOld=(Clinic)gridMain.ListGridRows[e.Row].Tag;
			using FormClinicEdit FormCE=new FormClinicEdit(clinicOld.Copy());
			if(FormCE.ShowDialog()==DialogResult.OK) {
				Clinic clinicNew=FormCE.ClinicCur;
				if(clinicNew==null) {//Clinic was deleted
					//Fix ItemOrders
					ListClinics.FindAll(x => x.ItemOrder>clinicOld.ItemOrder)
						.ForEach(x => x.ItemOrder--);
					ListClinics.Remove(clinicOld);
				}
				else {
					ListClinics[ListClinics.IndexOf(clinicOld)]=clinicNew;
				}
			}
			FillGrid();			
		}
		
		private void butClinicPick_Click(object sender,EventArgs e) {
			List<Clinic> listClinics=gridMain.GetTags<Clinic>();
			using FormClinics formC=new FormClinics();
			formC.ListClinics=listClinics;
			formC.IsSelectionMode=true;
			if(formC.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_clinicNumTo=formC.SelectedClinicNum;
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
			Dictionary<long,Clinic> dictClinicsFrom=gridMain.SelectedTags<Clinic>().ToDictionary(x => x.ClinicNum);
			Clinic clinicTo=gridMain.GetTags<Clinic>().FirstOrDefault(x => x.ClinicNum==_clinicNumTo);
			if(clinicTo==null) {
				MsgBox.Show(this,"The clinic could not be found.");
				return;
			}
			if(dictClinicsFrom.ContainsKey(clinicTo.ClinicNum)) {
				MsgBox.Show(this,"The 'To' clinic should not also be one of the 'From' clinics.");
				return;
			}
			Dictionary<long,int> dictClinFromCounts=Clinics.GetClinicalPatientCount(true)
				.Where(x => dictClinicsFrom.ContainsKey(x.Key)).ToDictionary(x => x.Key,x => x.Value);
			if(dictClinFromCounts.Sum(x => x.Value)==0) {
				MsgBox.Show(this,"There are no patients assigned to the selected clinics.");
				return;
			}
			string msg=Lan.g(this,"This will move all patients to")+" "+clinicTo.Abbr+" "+Lan.g(this,"from the following clinics")+":\r\n"
				+string.Join("\r\n",dictClinFromCounts.Select(x => dictClinicsFrom[x.Key].Abbr))+"\r\n"+Lan.g(this,"Continue?");
			if(MessageBox.Show(msg,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
				return;
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				int patsMoved=0;
				List<Action> listActions=dictClinFromCounts.Select(x => new Action(() => {
					Patients.ChangeClinicsForAll(x.Key,clinicTo.ClinicNum);//update all clinicNums to new clinic
					Clinic clinicCur;
					SecurityLogs.MakeLogEntry(Permissions.PatientEdit,0,"Clinic changed for "+x.Value+" patients from "
						+(dictClinicsFrom.TryGetValue(x.Key,out clinicCur)?clinicCur.Abbr:"")+" to "+clinicTo.Abbr+".");
					patsMoved+=x.Value;
					ProgressBarEvent.Fire(ODEventType.ProgressBar,Lan.g(this,"Moved patients")+": "+patsMoved+" "+Lan.g(this,"out of")+" "
						+dictClinFromCounts.Sum(y => y.Value));
				})).ToList();
				ODThread.RunParallel(listActions,TimeSpan.FromMinutes(2));
			};
			progressOD.StartingMessage=Lan.g(this,"Moving patients")+"...";
			progressOD.TestSleep=true;
			progressOD.ShowDialogProgress();
			_dictClinicalCounts=Clinics.GetClinicalPatientCount();
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
			if(selectedIdx==0 || (IncludeHQInList && selectedIdx==1)) {
				return;
			}
			//Swap clinic ItemOrders
			Clinic sourceClin=((Clinic)gridMain.ListGridRows[selectedIdx].Tag);
			Clinic destClin=((Clinic)gridMain.ListGridRows[selectedIdx-1].Tag);
			if(sourceClin.ItemOrder==destClin.ItemOrder) {
				sourceClin.ItemOrder--;
			}
			else {
				int sourceOrder=sourceClin.ItemOrder;
				sourceClin.ItemOrder=destClin.ItemOrder;
				destClin.ItemOrder=sourceOrder;
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
			if(selectedIdx==gridMain.ListGridRows.Count-1 || (IncludeHQInList && selectedIdx==0)) {
				return;
			}
			//Swap clinic ItemOrders
			Clinic sourceClin=((Clinic)gridMain.ListGridRows[selectedIdx].Tag);
			Clinic destClin=((Clinic)gridMain.ListGridRows[selectedIdx+1].Tag);
			if(sourceClin.ItemOrder==destClin.ItemOrder) {//just in case, an issue in the past would cause ItemOrder inconsitencies, shouldn't happen
				sourceClin.ItemOrder++;
			}
			else {
				int sourceOrder=sourceClin.ItemOrder;
				sourceClin.ItemOrder=destClin.ItemOrder;
				destClin.ItemOrder=sourceOrder;
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
			if(IncludeHQInList) {//always keep the HQ clinic at the top if it's in the list
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
			List<Clinic> listAllClinicsDb=Clinics.GetClinicsNoCache();//get all clinics, even hidden ones, in order to set the ItemOrders correctly
			List<Clinic> listAllClinicsNew=listAllClinicsDb.Select(x => x.Copy()).ToList();
			bool isHqInList=IncludeHQInList;
			IncludeHQInList=false;
			listAllClinicsNew.Sort(ClinicSort);
			IncludeHQInList=isHqInList;
			for(int i=0;i<listAllClinicsNew.Count;i++) {
				listAllClinicsNew[i].ItemOrder=i+1;//1 based ItemOrder because the HQ 'clinic' has ItemOrder 0
			}
			return Clinics.Sync(listAllClinicsNew,listAllClinicsDb);
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

		private void butOK_Click(object sender,EventArgs e) {
			if(IsSelectionMode && gridMain.SelectedIndices.Length>0) {
				SelectedClinicNum=gridMain.SelectedTag<Clinic>()?.ClinicNum??0;
				ListSelectedClinicNums=gridMain.SelectedTags<Clinic>().Select(x => x.ClinicNum).ToList();
				DialogResult=DialogResult.OK;
			}
			Close();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			SelectedClinicNum=0;
			ListSelectedClinicNums=new List<long>();
			Close();
		}

		private void FormClinics_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(IsSelectionMode) {
				return;
			}
			if(Prefs.UpdateBool(PrefName.ClinicListIsAlphabetical,checkOrderAlphabetical.Checked)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			bool hasClinicChanges=Clinics.Sync(ListClinics,_listClinicsOld);//returns true if clinics were updated/inserted/deleted
			//Update the ClinicNum on all specialties associated to each clinic.
			ListClinics.ForEach(x => x.ListClinicSpecialtyDefLinks.ForEach(y => y.FKey=x.ClinicNum));
			List<DefLink> listAllClinicSpecialtyDefLinks=ListClinics.SelectMany(x => x.ListClinicSpecialtyDefLinks).ToList();
			hasClinicChanges|=DefLinks.Sync(listAllClinicSpecialtyDefLinks,_listClinicDefLinksAllOld);
			hasClinicChanges|=CorrectItemOrders();
			//Joe - Now that we have called sync on ListClinics we want to make sure that each clinic has program properties for PayConnect and XCharge
			//We are doing this because of a previous bug that caused some customers to have over 3.4 million duplicate rows in their programproperty table
			long payConnectProgNum=Programs.GetProgramNum(ProgramName.PayConnect);
			long xChargeProgNum=Programs.GetProgramNum(ProgramName.Xcharge);
			//Don't need to do this for PaySimple, because these will get generated as needed in FormPaySimpleSetup
			bool hasChanges=ProgramProperties.InsertForClinic(payConnectProgNum,
				ListClinics.Select(x => x.ClinicNum).Where(x => ProgramProperties.GetListForProgramAndClinic(payConnectProgNum,x).Count==0).ToList());
			hasChanges|=ProgramProperties.InsertForClinic(xChargeProgNum,
				ListClinics.Select(x => x.ClinicNum).Where(x => ProgramProperties.GetListForProgramAndClinic(xChargeProgNum,x).Count==0).ToList());
			if(hasChanges) {
				DataValid.SetInvalid(InvalidType.Programs);
			}
			if(hasClinicChanges) {
				DataValid.SetInvalid(InvalidType.Providers);
			}
		}
	}
}





















