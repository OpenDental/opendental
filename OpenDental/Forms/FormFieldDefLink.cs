using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormFieldDefLink:FormODBase {
		private List<FieldDefLink> _listFieldDefLinks;
		private List<ApptFieldDef> _listApptFieldDefs;
		private List<PatFieldDef> _listPatFieldDefs;
		private FieldLocations _fieldLocation;

		public FormFieldDefLink(FieldLocations fieldLocation=FieldLocations.Account) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_fieldLocation=fieldLocation;
		}

		private void FormFieldDefLink_Load(object sender,EventArgs e) {
			string[] arrayFieldLocations=Enum.GetNames(typeof(FieldLocations));
			for(int i=0;i<arrayFieldLocations.Length;i++) {
				comboFieldLocation.Items.Add(Lan.g("enumFieldLocations",arrayFieldLocations[i]));
				if(i==(int)_fieldLocation) {
					comboFieldLocation.SelectedIndex=i;
				}
			}
			_listFieldDefLinks=FieldDefLinks.GetAll();
			_listApptFieldDefs=ApptFieldDefs.GetDeepCopy();
			_listPatFieldDefs=PatFieldDefs.GetDeepCopy(true);
			FillGridDisplayed();
			FillGridHidden();
		}

		///<summary>Fills the displayed grid with all field defs that should show for the location selected.
		///This should be enhanced in the future to include indication rows when there is a potential for one location to serve multiple types.</summary>
		private void FillGridDisplayed() {
			//Find all FieldDefLinks for the currently selected location, then find the apptfield/patfield for each, to display.
			int selectedIdx=gridDisplayed.GetSelectedIndex();
			gridDisplayed.BeginUpdate();
			gridDisplayed.ListGridColumns.Clear();
			gridDisplayed.ListGridColumns.Add(new GridColumn("",20){ IsWidthDynamic=true });
			gridDisplayed.ListGridRows.Clear();
			GridRow row;
			switch((FieldLocations)comboFieldLocation.SelectedIndex) 
			{
				case FieldLocations.Account:
				case FieldLocations.Chart:
				case FieldLocations.Family:
				case FieldLocations.GroupNote:
				case FieldLocations.OrthoChart:
					foreach(PatFieldDef patField in _listPatFieldDefs) {
						if(_listFieldDefLinks.Exists(x => x.FieldDefNum==patField.PatFieldDefNum 
							&& x.FieldLocation==(FieldLocations)comboFieldLocation.SelectedIndex 
							&& x.FieldDefType==FieldDefTypes.Patient)) 
						{
							continue; //If there is already a link for the patfield for the selected location, don't display the patfield in "display" grid.
						}
						row=new GridRow();
						row.Cells.Add(patField.FieldName);
						row.Tag=patField;
						gridDisplayed.ListGridRows.Add(row);
					}
					break;
				case FieldLocations.AppointmentEdit://AppointmentEdit is the only place where ApptFields are used.
					foreach(ApptFieldDef apptField in _listApptFieldDefs) {
						if(_listFieldDefLinks.Exists(x => x.FieldDefNum==apptField.ApptFieldDefNum 
							&& x.FieldLocation==(FieldLocations)comboFieldLocation.SelectedIndex 
							&& x.FieldDefType==FieldDefTypes.Appointment)) 
						{
							continue; //If there is already a link for the apptfield for the selected location, don't display the apptfield in "display" grid.
						}
						row=new GridRow();
						row.Cells.Add(apptField.FieldName);
						row.Tag=apptField;
						gridDisplayed.ListGridRows.Add(row);
					}
					break;
			}
			gridDisplayed.EndUpdate();
			if(gridDisplayed.ListGridRows.Count-1>=selectedIdx) {
				gridDisplayed.SetSelected(selectedIdx,true);
			}
		}

		///<summary>Fills the hidden grid with all hidden field defs for the location selected.
		///This should be enhanced in the future to include indication rows when there is a potential for one location to serve multiple types.</summary>
		private void FillGridHidden() {
			int selectedIdx=gridHidden.GetSelectedIndex();
			gridHidden.BeginUpdate();
			gridHidden.ListGridColumns.Clear();
			gridHidden.ListGridColumns.Add(new GridColumn("",20){ IsWidthDynamic=true });
			gridHidden.ListGridRows.Clear();
			GridRow row;
			List<FieldDefLink> listFieldDefLinksForLoc=_listFieldDefLinks.FindAll(x => x.FieldLocation==(FieldLocations)comboFieldLocation.SelectedIndex);
			List<FieldDefLink> listLinksToDelete=new List<FieldDefLink>();//Some links could exists to deleted FieldDefs prior to 18.1
			foreach(FieldDefLink fieldDefLink in listFieldDefLinksForLoc) {
				switch(fieldDefLink.FieldDefType) {
					case FieldDefTypes.Patient:
						PatFieldDef patFieldDef=_listPatFieldDefs.Find(x => x.PatFieldDefNum==fieldDefLink.FieldDefNum);
						if(patFieldDef==null) {//orphanded FK link to deleted PatFieldDef
							listLinksToDelete.Add(fieldDefLink);//orphanded FK link to deleted PatFieldDef
							continue;
						}
						row=new GridRow();
						row.Cells.Add(patFieldDef.FieldName);
						row.Tag=fieldDefLink;
						gridHidden.ListGridRows.Add(row);
					break;
					case FieldDefTypes.Appointment:
						ApptFieldDef apptFieldDef=_listApptFieldDefs.Find(x => x.ApptFieldDefNum==fieldDefLink.FieldDefNum);
						if(apptFieldDef==null) {//orphaned FK link to deleted ApptFieldDef
							listLinksToDelete.Add(fieldDefLink);//orphaned FK link to deleted ApptFieldDef
							continue;
						}
						row=new GridRow();
						row.Cells.Add(apptFieldDef.FieldName);
						row.Tag=fieldDefLink;
						gridHidden.ListGridRows.Add(row);
					break;
				}
			}
			//Remove all orphaned links
			foreach(FieldDefLink fieldDefLink in listLinksToDelete) {
				_listFieldDefLinks.Remove(fieldDefLink);
			}
			gridHidden.EndUpdate();
			if(gridHidden.ListGridRows.Count-1>=selectedIdx) {
				gridHidden.SetSelected(selectedIdx,true);
			}
		}

		private void butRight_Click(object sender,EventArgs e) {
			//Add the selected field def from the display grid to the _listFieldDefLinks as a new link.  Refill grid.
			if(gridDisplayed.GetSelectedIndex()<0) {
				return;//Button pressed with nothing selected
			}
			FieldDefLink fieldDefLink=new FieldDefLink();
			switch((FieldLocations)comboFieldLocation.SelectedIndex) 
			{
				case FieldLocations.Account:
				case FieldLocations.Chart:
				case FieldLocations.Family:
				case FieldLocations.GroupNote:
				case FieldLocations.OrthoChart:
					PatFieldDef patFieldDef=(PatFieldDef)gridDisplayed.ListGridRows[gridDisplayed.GetSelectedIndex()].Tag;
					fieldDefLink=new FieldDefLink();
					fieldDefLink.FieldDefNum=patFieldDef.PatFieldDefNum;
					fieldDefLink.FieldDefType=FieldDefTypes.Patient;
					fieldDefLink.FieldLocation=(FieldLocations)comboFieldLocation.SelectedIndex;
					_listFieldDefLinks.Add(fieldDefLink);
					break;
				case FieldLocations.AppointmentEdit://AppointmentEdit is the only place where ApptFields are used.
					ApptFieldDef apptFieldDef=(ApptFieldDef)gridDisplayed.ListGridRows[gridDisplayed.GetSelectedIndex()].Tag;
					fieldDefLink=new FieldDefLink();
					fieldDefLink.FieldDefNum=apptFieldDef.ApptFieldDefNum;
					fieldDefLink.FieldDefType=FieldDefTypes.Appointment;
					fieldDefLink.FieldLocation=(FieldLocations)comboFieldLocation.SelectedIndex;
					_listFieldDefLinks.Add(fieldDefLink);
					break;
			}
			FillGridDisplayed();
			FillGridHidden();
		}

		private void butLeft_Click(object sender,EventArgs e) {
			//Remove the selected field def from the hidden grid.
			if(gridHidden.SelectedIndices.Length<1) {//Nothing selected
				return;
			}
			_listFieldDefLinks.Remove((FieldDefLink)gridHidden.ListGridRows[gridHidden.GetSelectedIndex()].Tag);
			FillGridDisplayed();
			FillGridHidden();
		}

		private void comboFieldLocation_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGridDisplayed();
			FillGridHidden();
		}

		private void butOK_Click(object sender,EventArgs e) {
			FieldDefLinks.Sync(_listFieldDefLinks);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}