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
	public partial class FormProgramLinkHideClinics:FormODBase {
		private readonly long _programNum;
		private List<Clinic> _listClinics;

		public FormProgramLinkHideClinics(Program program,List<ProgramProperty> listProgramProperties,List<Clinic> listClinicsUser) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			showClinicStateWarning(Security.CurUser.ClinicIsRestricted);
			checkOrderAlphabetical.Checked=PrefC.GetBool(PrefName.ClinicListIsAlphabetical);
			_programNum=program.ProgramNum;
			_listClinics=listClinicsUser;
			List<Clinic> listClinicsVisible=listClinicsUser.Select(x => x.Copy()).ToList();//Copy so we don't affect the source list.
			List<Clinic> listClinicsHidden=new List<Clinic>();
			if(!listProgramProperties.IsNullOrEmpty()) {
				//If a ProgramProperty exists within listProgProp (hide for these clinics) then we move the corresponding clinic to listHideButtonsForClinics
				for(int i=listClinicsVisible.Count-1;i>=0;i--) {//Count backwards to allow for clean removal from lists.
					if(listProgramProperties.Any(x => x.ClinicNum==listClinicsVisible[i].ClinicNum)) {//Means the buttons are hidden for this clinic.
						listClinicsHidden.Add(listClinicsVisible[i]);//If removing from the "Visible" list then it goes into the "Hidden".
						listClinicsVisible.Remove(listClinicsVisible[i]);//Then remove from "Visible". (add/remove order matters).
					}
				}
			}
			//Populate both listboxes based on above lists.
			listClinicsHidden.Sort(NaturalSort);//For Alphabetical Sort
			listClinicsVisible.Sort(NaturalSort);
			listboxVisibleClinics.Items.AddList(listClinicsVisible,x => x.Abbr);//Sets the buttons based on sorted lists
			listboxHiddenClinics.Items.AddList(listClinicsHidden,x => x.Abbr);
		}

		///<summary>Show label warning the user clinics may not be visible due to clinic restriction if they are in fact clinic restricted.</summary>
		private void showClinicStateWarning(bool isUserClinicRestricted) {
			if(!isUserClinicRestricted) {
				labelClinicStateWarning.Visible=false;
			}
		}

		///<summary>Syncs any changes made by the user to the list of Program Properties that indicates this Program Link's button should be hidden
		///per clinic.  Only syncs changes made to ProgramProperties for clinics the user has access to.</summary>
		private void SyncHiddenProgramProperties() {
			//Get the users total list of unrestricted clinics, then acquire their list of ProgramProperties so we can tell which PL buttons 
			//should be hidden based upon the ProgramProperty PropertyDesc indicator. 
			List<Clinic> listClinicsUser=Clinics.GetForUserod(Security.CurUser,doIncludeHQ:true,hqClinicName:Lan.g(this,"HQ"));
			//Get the cached list of button hiding ProgramProperties for clinics this user has access to, i.e. the "Old" list.
			List<ProgramProperty> listProgramPropertiesHidden=ProgramProperties.GetForProgram(_programNum)
				.Where(x => x.PropertyDesc==ProgramProperties.PropertyDescs.ClinicHideButton 
					&& listClinicsUser.Select(y => y.ClinicNum).Contains(x.ClinicNum)).ToList();
			// A list of ProgramProperties populated by the listboxHiddenClinics listbox
			List<ProgramProperty> listProgramPropertiesHiddenClinics= new List<ProgramProperty>();
			List<Clinic> listClinics=listboxHiddenClinics.Items.GetAll<Clinic>();
			for(int i=0;i<listClinics.Count;i++){
				ProgramProperty programProperty=new ProgramProperty();
				programProperty.ProgramNum=_programNum;
				programProperty.PropertyDesc=ProgramProperties.PropertyDescs.ClinicHideButton;
				programProperty.ClinicNum=listClinics[i].ClinicNum;
				listProgramPropertiesHiddenClinics.Add(programProperty);
			}
			//Compares the old list of ProgramProperties to the new one, if a clinic exists in the old list but not the new list then it was deleted by the 
			//user and we remove it from the db.
			for(int i=0;i<listProgramPropertiesHidden.Count;i++) {
				if(!listProgramPropertiesHiddenClinics.Select(x => x.ProgramPropertyNum).Contains(listProgramPropertiesHidden[i].ProgramPropertyNum)) {//Clinic was Removed from List
					ProgramProperties.Delete(listProgramPropertiesHidden[i]);//Remove from ProgramProperty
				}
			}
			//Compares the new list of ProgramProperties to the old one, if a clinic exists in the new list but not the old one then it was added by the 
			//user and we should add it to the db.
			for(int i=0;i<listProgramPropertiesHiddenClinics.Count;i++) {
				if(!listProgramPropertiesHidden.Select(x => x.ProgramPropertyNum).Contains(listProgramPropertiesHiddenClinics[i].ProgramPropertyNum)) {//Clinic was Added to List
					ProgramProperties.Insert(listProgramPropertiesHiddenClinics[i]);//Insert ProgramProperty
				}
			}
		}

		///<summary>A sort to switch list sorting between ASCIIbetical and Alphabetical. This method is ClinicSort(...) from FormClinics.cs.</summary>
		private int NaturalSort(Clinic clinicX,Clinic clinicY) {
			if(clinicX.ClinicNum==0) {
					return -1;
			}
			if(clinicY.ClinicNum==0) {
					return 1;
			}
			int retval=0;
			if(checkOrderAlphabetical.Checked) {//order alphabetical by Abbr
				retval=clinicX.Abbr.CompareTo(clinicY.Abbr);
			}
			else {//not alphabetical, order by ItemOrder
				retval=clinicX.ItemOrder.CompareTo(clinicY.ItemOrder);
			}
			if(retval==0) {//if Abbr's are alphabetically the same, order alphabetical by Description
				retval=clinicX.Description.CompareTo(clinicY.Description);
			}
			if(retval==0) {//if Abbrs are the same and Descriptions are alphabetically the same, order by ClinicNum (guaranteed deterministic)
				retval=clinicX.ClinicNum.CompareTo(clinicY.ClinicNum);
			}
			return retval;
		}

		private void ListboxVisibleClinics_MouseClick(object sender,MouseEventArgs e) {
			listboxHiddenClinics.ClearSelected();
		}

		private void ListboxHiddenClinics_MouseClick(object sender,MouseEventArgs e) {
			listboxVisibleClinics.ClearSelected();
		}

		///<summary>Moving a clinic from Hidden to Visible</summary>
		private void butRight_Click(object sender,EventArgs e) {
			List<long> listClinicNumsMoved=MoveClinics(listBoxFrom:listboxHiddenClinics,listBoxTo:listboxVisibleClinics);
			ReselectClinics(listboxVisibleClinics,listClinicNumsMoved);
		}

		/// <summary>Moving a clinic from Visible to Hidden</summary>
		private void butLeft_Click(object sender,EventArgs e) {
			List<long> listClinicNumsMoved=MoveClinics(listBoxFrom:listboxVisibleClinics,listBoxTo:listboxHiddenClinics);
			ReselectClinics(listboxHiddenClinics,listClinicNumsMoved);
		}

		///<summary>Method reads in a ListBox to move items from, and another ListBox to move items into. It checks the first list's selected indices and then, if it 
		///finds a matching clinic it removes it from the "from" list and moves it to the "to" list.</summary>
		private List<long> MoveClinics(UI.ListBox listBoxFrom,UI.ListBox listBoxTo) {
			if(listBoxFrom.SelectedIndices.Count==0) {
				return new List<long>();//If nothing is selected, change nothing so the UI doesn't refresh.
			}
			List<long> listClinicNumsToMove=listBoxFrom.GetListSelected<Clinic>().Select(x => x.ClinicNum).ToList();
			List<long> listClinicNumsFrom=listBoxFrom.Items.GetAll<Clinic>().Select(x => x.ClinicNum).ToList();
			listBoxFrom.Items.Clear();
			listBoxTo.Items.Clear();
			for(int i=0;i<_listClinics.Count;i++) {
				if(listClinicNumsToMove.Contains(_listClinics[i].ClinicNum)) {
					listBoxTo.Items.Add(_listClinics[i].Abbr,_listClinics[i]);
					continue;
				}
				if(listClinicNumsFrom.Contains(_listClinics[i].ClinicNum)) {
					listBoxFrom.Items.Add(_listClinics[i].Abbr,_listClinics[i]);
					continue;
				}
				listBoxTo.Items.Add(_listClinics[i].Abbr,_listClinics[i]);
			}
			return listClinicNumsToMove;
		}

		///<summary>If the ordering has changed, update the list and maintain selection.</summary>
		private void CheckOrderAlphabetical_CheckedChanged(object sender,EventArgs e) {
			List<Clinic> listClinicsVisible=listboxVisibleClinics.Items.GetAll<Clinic>();//Acquire the full list of tags for SetItems.
			List<Clinic> listClinicsHidden=listboxHiddenClinics.Items.GetAll<Clinic>();
			List<Clinic> listClinicsVisibleSelected=listboxVisibleClinics.GetListSelected<Clinic>();//Aciquire the Selected tags for SetItems.
			List<Clinic> listClinicsHiddenSelected=listboxHiddenClinics.GetListSelected<Clinic>();
			listClinicsVisible.Sort(NaturalSort);//Sorts based on the status of the checkbox.
			listClinicsHidden.Sort(NaturalSort);
			listboxVisibleClinics.Items.Clear();
			listboxVisibleClinics.Items.AddList<Clinic>(listClinicsVisible,x => x.Abbr);
			for(int i=0; i<listClinicsVisible.Count;i++) {
				if(listClinicsVisibleSelected.Select(y => y.ClinicNum).Contains(listClinicsVisible[i].ClinicNum)) {
					listboxVisibleClinics.SetSelected(i,true);
				}
			}
			listboxHiddenClinics.Items.Clear();
			listboxHiddenClinics.Items.AddList<Clinic>(listClinicsHidden,x => x.Abbr);
			for(int i=0; i<listClinicsHidden.Count;i++) {
				if(listClinicsHiddenSelected.Select(y => y.ClinicNum).Contains(listClinicsHidden[i].ClinicNum)) {
					listboxHiddenClinics.SetSelected(i,true);
				}
			}
		}

		///<summary>Maintain the selection of the user when either moving items between sides or changing sort typyes.</summary>
		private void ReselectClinics(UI.ListBox listBox,List<long> listClinicNumsSelected) {
			for(int i=0; i<listBox.Items.Count;i++) {
				if(listClinicNumsSelected.Select(y => y).Contains(((Clinic)listBox.Items.GetObjectAt(i)).ClinicNum)) {
					listBox.SetSelected(i,true);
				}
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			//ProgramProperties will only sync in the case of the OK click, on a cancel they will not be saved.
			SyncHiddenProgramProperties();
			DialogResult=DialogResult.OK;
		}

	}
}