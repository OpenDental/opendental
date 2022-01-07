using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	///<summary>Display fields specifically for the ortho chart.</summary>
	public partial class FormDisplayFieldsOrthoChart : FormODBase {
		private bool changed;
		///<summary>The outter list represents each tab.  Within each tab, there can be multiple display fields.
		///An individual display field record can be associated to multiple tabs (the exact same object can exist in multiple lists).</summary>
		private List<OrthoChartTabFields> _listTabDisplayFields=new List<OrthoChartTabFields>();
		///<summary>The list of existing display fields which are not currently in use within the selected ortho chart tab.</summary>
		private List <DisplayField> _listDisplayFieldsAvail=null;
		///<summary>All ortho chart display fields available which includes "orphaned" display fields.  Filled on load.</summary>
		private List<DisplayField> _listAllDisplayFields=null;
		private List<OrthoChartTab> _listOrthoChartTabs;
		///<summary>Local copy of cached OrthoChartTabLinks. Ensures that any cached changes cause by others will not effect what we see.</summary>
		private List<OrthoChartTabLink> _listOrthoChartTabLinks;

		public FormDisplayFieldsOrthoChart() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDisplayFields_Load(object sender,EventArgs e) {
			_listOrthoChartTabs=OrthoChartTabs.GetDeepCopy(true);
			_listOrthoChartTabLinks=OrthoChartTabLinks.GetDeepCopy();
			LoadDisplayFields();
			FillComboOrthoChartTabs();
			FillGrids();
		}

		private void LoadDisplayFields() {
			OrthoChartTabs.RefreshCache();
			OrthoChartTabLinks.RefreshCache();
			DisplayFields.RefreshCache();
			_listAllDisplayFields=DisplayFields.GetAllAvailableList(DisplayFieldCategory.OrthoChart);
			_listTabDisplayFields=new List<OrthoChartTabFields>();
			//Add all fields that are actively associated to a tab to our class wide list of tabs and fields.
			for(int i=0;i<_listOrthoChartTabs.Count;i++) {
				OrthoChartTabFields orthoChartTabFields=new OrthoChartTabFields();
				orthoChartTabFields.OrthoChartTab=_listOrthoChartTabs[i];
				orthoChartTabFields.ListDisplayFields=new List<DisplayField>();
				List<OrthoChartTabLink> listOrthoChartTabLinks=_listOrthoChartTabLinks.FindAll(
					x => x.OrthoChartTabNum==_listOrthoChartTabs[i].OrthoChartTabNum
				);
				listOrthoChartTabLinks.OrderBy(x => x.ItemOrder);
				foreach(OrthoChartTabLink orthoChartTabLink in listOrthoChartTabLinks) {
					orthoChartTabFields.ListDisplayFields.AddRange(_listAllDisplayFields.FindAll(x => x.DisplayFieldNum==orthoChartTabLink.DisplayFieldNum));
				}
				_listTabDisplayFields.Add(orthoChartTabFields);
			}
			//Add a dummy OrthoChartTabFields object to the list that represents available fields that are not part of any tab.
			//These "display fields" were previously used at least once. A patient has info for this field, then the office removed the field from all tabs.
			List<DisplayField> listOrphanedFields=_listAllDisplayFields.FindAll(x => x.DisplayFieldNum==0
				|| !OrthoChartTabLinks.GetExists(y => y.DisplayFieldNum==x.DisplayFieldNum));
			if(listOrphanedFields!=null && listOrphanedFields.Count > 0) {
				OrthoChartTabFields orphanedFields=new OrthoChartTabFields();
				orphanedFields.OrthoChartTab=null;//These are fields not associated to any tab.  Purposefully use null.
				orphanedFields.ListDisplayFields=new List<DisplayField>();
				orphanedFields.ListDisplayFields.AddRange(listOrphanedFields);
				_listTabDisplayFields.Add(orphanedFields);
			}
		}

		private void FillComboOrthoChartTabs() {
			//We can't remember the previously selected tab (unless we did it by name?) so just refill the combo box and select the first item in the list.
			comboOrthoChartTabs.Items.Clear();
			for(int i=0;i<_listOrthoChartTabs.Count;i++) {
				comboOrthoChartTabs.Items.Add(_listOrthoChartTabs[i].TabName);
			}
			comboOrthoChartTabs.SelectedIndex=0;
		}

		private void FillGrids() {
			labelCategory.Text=_listOrthoChartTabs[0].TabName;//Placed here so that Up/Down buttons will affect the label text.			
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("FormDisplayFields","Description"),200);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormDisplayFields","Type"),80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormDisplayFields","Width"),60);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			OrthoChartTabFields orthoChartTabFields=GetSelectedFields();
			List<OrthoChartTabLink> listLinks=_listOrthoChartTabLinks.FindAll(x => x.OrthoChartTabNum==orthoChartTabFields.OrthoChartTab.OrthoChartTabNum);
			_listDisplayFieldsAvail=GetAllFields();
			for(int i=0;i<orthoChartTabFields.ListDisplayFields.Count;i++){
				_listDisplayFieldsAvail.Remove(orthoChartTabFields.ListDisplayFields[i]);
				GridRow row=new GridRow();
				row.Tag=orthoChartTabFields.ListDisplayFields[i];
				string description=orthoChartTabFields.ListDisplayFields[i].Description;
				if(!string.IsNullOrEmpty(orthoChartTabFields.ListDisplayFields[i].DescriptionOverride)) {
					description+=" ("+orthoChartTabFields.ListDisplayFields[i].DescriptionOverride+")";
				}
				row.Cells.Add(description);
				if(orthoChartTabFields.ListDisplayFields[i].PickList!=""){
					row.Cells.Add("PickList");
				}
				else if(orthoChartTabFields.ListDisplayFields[i].InternalName=="Signature"){
					row.Cells.Add("Signature");
				}
				else if(orthoChartTabFields.ListDisplayFields[i].InternalName=="Provider"){
					row.Cells.Add("Provider");
				}
				else{
					row.Cells.Add("Text");
				}
				int columnWidth=orthoChartTabFields.ListDisplayFields[i].ColumnWidth;
				OrthoChartTabLink link=listLinks.FirstOrDefault(x => x.DisplayFieldNum==orthoChartTabFields.ListDisplayFields[i].DisplayFieldNum);
				if(orthoChartTabFields.ListDisplayFields[i].IsNew) {//All new fields have a DisplayFieldNum of 0, so we have to compare by a different value
					link=listLinks.FirstOrDefault(x => x.IsNew && ((DisplayField)x.TagOD).Description==orthoChartTabFields.ListDisplayFields[i].Description);
				}
				if(link!=null && link.ColumnWidthOverride>0) {
					columnWidth=link.ColumnWidthOverride;
				}
				row.Cells.Add(POut.Int(columnWidth));
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			listAvailable.Items.Clear();
			for(int i=0;i<_listDisplayFieldsAvail.Count;i++) {
				listAvailable.Items.Add(_listDisplayFieldsAvail[i].Description);
			}
		}

		///<summary>Get the list of all unique display fields accross all ortho chart tabs.
		///Set hasOrphanedFields to false to exclude any "available" fields that are not currently associated to a tab.</summary>
		private List<DisplayField> GetAllFields(bool hasOrphanedFields=true) {
			List<DisplayField> listDisplayFields=new List<DisplayField>();
			foreach(OrthoChartTabFields orthoChartTabFields in _listTabDisplayFields) {
				if(!hasOrphanedFields && orthoChartTabFields.OrthoChartTab==null) {
					continue;//Do not include orphaned fields.
				}
				foreach(DisplayField df in orthoChartTabFields.ListDisplayFields) {
					if(!listDisplayFields.Contains(df)) {
						listDisplayFields.Add(df);
					}
				}
			}
			return listDisplayFields;
		}

		///<summary>Gets the currently selected tab information.</summary>
		private OrthoChartTabFields GetSelectedFields() {
			long orthoChartTabNum=_listOrthoChartTabs[comboOrthoChartTabs.SelectedIndex].OrthoChartTabNum;
			return _listTabDisplayFields.FirstOrDefault(x => x.OrthoChartTab!=null && x.OrthoChartTab.OrthoChartTabNum==orthoChartTabNum);
		}

		private void comboOrthoChartTabs_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrids();
		}

		private void butSetupTabs_Click(object sender,EventArgs e) {
			using FormOrthoChartSetup form=new FormOrthoChartSetup();
			if(form.ShowDialog()!=DialogResult.OK) {
				return;
			}
			List<OrthoChartTabFields> listOldTabDisplayFields=_listTabDisplayFields;
			_listTabDisplayFields=new List<OrthoChartTabFields>();
			//The tab order may have changed.  Also new tabs may have been added.  Tabs were not removed, because they can only be hidden not deleted.
			//If orthocharttabs order changed or new tabs were added, then the cache was updated in FormOrthoChartSetup in OK click.
			_listOrthoChartTabs=OrthoChartTabs.GetDeepCopy(true);
			foreach(OrthoChartTab orthoChartTab in _listOrthoChartTabs) {
				OrthoChartTabFields orthoChartTabFieldsOld=listOldTabDisplayFields.FirstOrDefault(
					x => x.OrthoChartTab!=null && x.OrthoChartTab.OrthoChartTabNum==orthoChartTab.OrthoChartTabNum);
				OrthoChartTabFields orthoChartTabFields=new OrthoChartTabFields();
				orthoChartTabFields.OrthoChartTab=orthoChartTab;
				if(orthoChartTabFieldsOld==null) {//Either a new tab was added or a hidden tab was un-hidden.
					orthoChartTabFields.ListDisplayFields=new List<DisplayField>();
					//Hidden OrthoChartTabLinks were already included on loading this form.
					List<OrthoChartTabLink> listOrthoChartTabLinks=_listOrthoChartTabLinks.FindAll(x => x.OrthoChartTabNum==orthoChartTab.OrthoChartTabNum);
					foreach(OrthoChartTabLink orthoChartTabLink in listOrthoChartTabLinks) {
						orthoChartTabFields.ListDisplayFields.AddRange(_listAllDisplayFields.FindAll(x => x.DisplayFieldNum==orthoChartTabLink.DisplayFieldNum));
					}
				}
				else {//The tab already existed.  Maintain the display field names and order within the tab, especially if the tab order changed.
					orthoChartTabFields.ListDisplayFields=orthoChartTabFieldsOld.ListDisplayFields;
				}
				_listTabDisplayFields.Add(orthoChartTabFields);
			}
			//Always add the orphaned OrthoChartTabFields back because they can not be affected from the Tab Setup window.
			_listTabDisplayFields.AddRange(listOldTabDisplayFields.FindAll(x => x.OrthoChartTab==null));
			//Refresh the combo box and the grid because there is a chance that the user was viewing a different tab than the first in the list.
			FillComboOrthoChartTabs();
			FillGrids();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			DisplayField df=(DisplayField)gridMain.ListGridRows[e.Row].Tag;
			//OrthoChartTabFields orthoChartTabFields=GetSelectedFields();
			long orthoChartTabNum=_listOrthoChartTabs[comboOrthoChartTabs.SelectedIndex].OrthoChartTabNum;
			OrthoChartTabLink linkCur=_listOrthoChartTabLinks.FirstOrDefault(x => x.OrthoChartTabNum==orthoChartTabNum
				&& x.DisplayFieldNum==df.DisplayFieldNum);
			if(df.IsNew) {
				linkCur=_listOrthoChartTabLinks.FirstOrDefault(x => x.IsNew && x.OrthoChartTabNum==orthoChartTabNum 
					&& ((DisplayField)x.TagOD).Description==df.Description);
			}
			bool isAddingNewLink=false;
			if(linkCur==null) {//Avoid thrown exception in FormDisplayFieldOrthoEdit_Load(...) when adding a new link and then attempting to edit it.
				isAddingNewLink=true;
				linkCur=new OrthoChartTabLink() {
					OrthoChartTabNum=orthoChartTabNum,
					//ItemOrder will be set when FormDisplayFieldsOrthoChart closes/syncs.
					DisplayFieldNum=df.DisplayFieldNum,
					ColumnWidthOverride=0,
					TagOD=df,//There is no way of telling apart new links because all new display fields have a DisplayFieldNum of 0, so add a tag.
					IsNew=true
				};
			}
			using FormDisplayFieldOrthoEdit form=new FormDisplayFieldOrthoEdit(df,GetAllFields(),linkCur);
			form.ShowDialog();
			if(form.DialogResult!=DialogResult.OK) {
				return;
			}
			if(isAddingNewLink) {//Do not add new link to list if user clicks cancel because we want to act like nothing happened
				_listOrthoChartTabLinks.Add(linkCur);
			}
			FillGrids();
			changed=true;
		}

		private void butLeft_Click(object sender,EventArgs e) {
			if(listAvailable.SelectedIndices.Count==0 && textCustomField.Text=="") {
				MsgBox.Show(this,"Please select an item in the list on the right or create a new field first.");
				return;
			}
			OrthoChartTabFields orthoChartTabFields=GetSelectedFields();
			if(textCustomField.Text!="") {//Add new ortho chart field
				//Block adding new field if already showing.
				for(int i=0;i<orthoChartTabFields.ListDisplayFields.Count;i++) {
					if(textCustomField.Text==orthoChartTabFields.ListDisplayFields[i].Description) {
						MsgBox.Show(this,"That field is already displaying.");
						return;
					}
				}
				//Use available field if user typed in a "new" field name which matches an available field that is not already showing.
				List<DisplayField> listAllFields=GetAllFields();
				foreach(DisplayField df in listAllFields) {
					if(textCustomField.Text==df.Description) {
						orthoChartTabFields.ListDisplayFields.Add(df);
						textCustomField.Text="";
						changed=true;
						FillGrids();
						return;
					}
				}
				//The new field name is unique.  Create a cached copy in memory to be saved when OK is clicked.
				//This gives the user the option to remove the field before it ever reaches the database.
				DisplayField dfNew=new DisplayField("",100,DisplayFieldCategory.OrthoChart);
				dfNew.Description=textCustomField.Text;
				dfNew.IsNew=true;
				orthoChartTabFields.ListDisplayFields.Add(dfNew);
				textCustomField.Text="";
			}
			else {//Add existing ortho chart field(s).
				OrthoChartTabFields orphanedTab=_listTabDisplayFields.Find(x => x.OrthoChartTab==null);
				for(int i=0;i<listAvailable.SelectedIndices.Count;i++) {
					DisplayField df=_listDisplayFieldsAvail[listAvailable.SelectedIndices[i]];
					orthoChartTabFields.ListDisplayFields.Add(df);
					if(orphanedTab!=null) {
						//Remove the display field from the orphaned list if there is one.
						orphanedTab.ListDisplayFields.Remove(df);
					}
				}
			}
			changed=true;
			FillGrids();
		}

		private void butRight_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid on the left first.");
				return;
			}
			List<DisplayField> listRemovedFields=new List<DisplayField>();
			OrthoChartTabFields orthoChartTabFields=GetSelectedFields();
			for(int i=gridMain.SelectedIndices.Length-1;i>=0;i--) {//go backwards
				int index=gridMain.SelectedIndices[i];
				DisplayField df=(DisplayField)gridMain.ListGridRows[index].Tag;
				listRemovedFields.Add(df);//Keep track of all display fields removed.
				orthoChartTabFields.ListDisplayFields.Remove(df);
			}
			//Now we need to check all removed fields and see if any are still associated with a tab.
			//If they are not associated with any tabs then we need to remove them from our list.
			//They will show up in the available fields list if a patient in the database has a value for the field.
			foreach(DisplayField field in listRemovedFields) {
				bool isFieldOrphaned=true;
				foreach(OrthoChartTabFields tabFields in _listTabDisplayFields) {
					if(tabFields.OrthoChartTab==null) {
						continue;//Do not consider the orphaned tab.
					}
					if(tabFields.ListDisplayFields.Exists(x => x.DisplayFieldNum==field.DisplayFieldNum)) {
						isFieldOrphaned=false;
						break;//The field that was removed is still associated with a different tab so no action needed.
					}
				}
				if(!isFieldOrphaned) {
					continue;
				}
				//No tab has this display field so move it to the list of fields associated with the null tab
				OrthoChartTabFields orphanedTab=_listTabDisplayFields.Find(x => x.OrthoChartTab==null);
				if(orphanedTab==null) {//An orphaned list doesn't exist yet so create one.
					OrthoChartTabFields orphanedFields=new OrthoChartTabFields();
					orphanedFields.OrthoChartTab=null;//These are fields not associated to any tab.  Purposefully use null.
					orphanedFields.ListDisplayFields=new List<DisplayField>();
					orphanedFields.ListDisplayFields.Add(field);
					_listTabDisplayFields.Add(orphanedFields);
				}
				else {
					orphanedTab.ListDisplayFields.Add(field);
				}
			}
			FillGrids();
			changed=true;
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			if(gridMain.SelectedIndices[0]==0) {
				return;
			}
			int[] arrayIndicies=new int[gridMain.SelectedIndices.Length];
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				arrayIndicies[i]=gridMain.SelectedIndices[i];
			}
			OrthoChartTabFields orthoChartTabFields=GetSelectedFields();
			for(int i=0;i<arrayIndicies.Length;i++){
				orthoChartTabFields.ListDisplayFields.Reverse(arrayIndicies[i]-1,2);
			}
			FillGrids();
			for(int i=0;i<arrayIndicies.Length;i++){
				gridMain.SetSelected(arrayIndicies[i]-1,true);
			}
			changed=true;
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			OrthoChartTabFields orthoChartTabFields=GetSelectedFields();
			if(gridMain.SelectedIndices[gridMain.SelectedIndices.Length-1]==orthoChartTabFields.ListDisplayFields.Count-1) {
				return;
			}
			int[] arrayIndicies=new int[gridMain.SelectedIndices.Length];
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				arrayIndicies[i]=gridMain.SelectedIndices[i];
			}
			for(int i=arrayIndicies.Length-1;i>=0;i--) {//go backwards
				orthoChartTabFields.ListDisplayFields.Reverse(arrayIndicies[i],2);
			}
			FillGrids();
			for(int i=0;i<arrayIndicies.Length;i++) {
				gridMain.SetSelected(arrayIndicies[i]+1,true);
			}
			changed=true;
		}

		private void listAvailable_Click(object sender,EventArgs e) {
			textCustomField.Text="";
		}

		private void textCustomField_Click(object sender,EventArgs e) {
			listAvailable.SelectedIndex=-1;
		}

		private void butOK_Click(object sender,EventArgs e) {
			OrthoChartTabFields orphanedTab=_listTabDisplayFields.Find(x => x.OrthoChartTab==null);
			//No need to do anything if nothing changed and there are no 'orphaned' display fields to delete.
			if(!changed && (orphanedTab!=null && orphanedTab.ListDisplayFields.All(x => x.DisplayFieldNum==0))) {
				DialogResult=DialogResult.OK;
				return;
			}
			//Get all fields associated to a tab in order to sync with the database later.
			List<DisplayField> listAllFields=GetAllFields(false);
			if(listAllFields.Count(x=>x.InternalName=="Signature") > 1) {
				MessageBox.Show(Lan.g(this,"Only one display field can be a signature field.  Fields that have the signature field checkbox checked:")+" "
					+string.Join(", ",listAllFields.FindAll(x => x.InternalName=="Signature").Select(x => x.Description)));
				return;
			}
			//Ensure all new displayfields have a primary key so that tab links can be created below.  Update existing displayfields.
			foreach(DisplayField df in listAllFields) {
				if(df.DisplayFieldNum==0) {//New displayfield
					DisplayFields.Insert(df);
				}
				else {//Existing displayfield.
					DisplayFields.Update(df);
				}
			}
			//Remove tab links which no longer exist.  Update tab link item order for tab links which still belong to the same tab.
			for(int i=_listOrthoChartTabLinks.Count-1;i>=0;i--) {
				OrthoChartTabLink orthoChartTabLink=_listOrthoChartTabLinks[i];
				OrthoChartTabFields orthoChartTabFields=_listTabDisplayFields.FirstOrDefault(
					x => x.OrthoChartTab!=null && x.OrthoChartTab.OrthoChartTabNum==orthoChartTabLink.OrthoChartTabNum);
				if(orthoChartTabFields==null) {
					continue;//The tab was hidden and we are going to leave the tab links alone.
				}
				DisplayField df=orthoChartTabFields.ListDisplayFields.FirstOrDefault(x => x.DisplayFieldNum==orthoChartTabLink.DisplayFieldNum);
				if(orthoChartTabLink.IsNew) {
					df=orthoChartTabFields.ListDisplayFields.FirstOrDefault(x => x.Description==((DisplayField)orthoChartTabLink.TagOD).Description);
				}
				if(df==null) {//The tab link no longer exists (was removed).
					_listOrthoChartTabLinks.RemoveAt(i);
				}
				else {//The tab link still exists.  Update the link with any changes.
					orthoChartTabLink.ItemOrder=orthoChartTabFields.ListDisplayFields.IndexOf(df);
				}
			}
			//Add new tab links which were just created.
			foreach(OrthoChartTabFields orthoChartTabFields in _listTabDisplayFields) {
				//Skip "orphaned" fields that just show in the available fields list.
				if(orthoChartTabFields.OrthoChartTab==null) {
					continue;
				}
				foreach(DisplayField df in orthoChartTabFields.ListDisplayFields) {
					OrthoChartTabLink orthoChartTabLink=_listOrthoChartTabLinks.FirstOrDefault(
						x => x.OrthoChartTabNum==orthoChartTabFields.OrthoChartTab.OrthoChartTabNum && x.DisplayFieldNum==df.DisplayFieldNum);
					if(df.IsNew) {
						orthoChartTabLink=_listOrthoChartTabLinks.FirstOrDefault(x => x.IsNew 
							&& x.OrthoChartTabNum==orthoChartTabFields.OrthoChartTab.OrthoChartTabNum && ((DisplayField)x.TagOD).Description==df.Description);
						if(orthoChartTabLink!=null) {
							orthoChartTabLink.DisplayFieldNum=df.DisplayFieldNum;
						}
					}
					if(orthoChartTabLink!=null) {
						continue;
					}
					orthoChartTabLink=new OrthoChartTabLink();
					orthoChartTabLink.ItemOrder=orthoChartTabFields.ListDisplayFields.IndexOf(df);
					orthoChartTabLink.OrthoChartTabNum=orthoChartTabFields.OrthoChartTab.OrthoChartTabNum;
					orthoChartTabLink.DisplayFieldNum=df.DisplayFieldNum;
					_listOrthoChartTabLinks.Add(orthoChartTabLink);
				}
			}
			//Delete any display fields that have a valid PK and are in the "orphaned" list.  
			//This is fine to do because the field will show back up in the available list of display fields if a patient is still using the field.
			//This is because we link the ortho chart display fields by their name instead of by their PK.
			if(orphanedTab!=null) {//An orphaned list actually exists.
				//Look for any display fields that have a valid PK (this means the user removed this field from every tab and we need to delete it).
				List<DisplayField> listFieldsToDelete=orphanedTab.ListDisplayFields.FindAll(x => x.DisplayFieldNum!=0);
				listFieldsToDelete.ForEach(x => DisplayFields.Delete(x.DisplayFieldNum));
			}
			OrthoChartTabLinks.Sync(_listOrthoChartTabLinks,OrthoChartTabLinks.GetDeepCopy());
			DataValid.SetInvalid(InvalidType.OrthoChartTabs);
			DataValid.SetInvalid(InvalidType.DisplayFields);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}

	internal class OrthoChartTabFields {
		///<summary>Set or leave OrthoChartTab to null when storing the "orphaned" available fields (used in past and not associated to a tab).</summary>
		public OrthoChartTab OrthoChartTab=null;
		public List<DisplayField> ListDisplayFields=null;
	}

}
