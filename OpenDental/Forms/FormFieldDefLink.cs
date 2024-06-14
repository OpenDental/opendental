using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using System.Text;

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
			string[] stringArrayFieldLocations=Enum.GetNames(typeof(FieldLocations));
			for(int i=0;i<stringArrayFieldLocations.Length;i++) {
				comboFieldLocation.Items.Add(Lan.g("enumFieldLocations",stringArrayFieldLocations[i]));
				if(i==(int)_fieldLocation) {
					comboFieldLocation.SelectedIndex=i;
				}
			}
			_listFieldDefLinks=FieldDefLinks.GetDeepCopy();
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
			gridDisplayed.Columns.Clear();
			GridColumn col=new GridColumn("",20);
			col.IsWidthDynamic=true; 
			gridDisplayed.Columns.Add(col);
			gridDisplayed.ListGridRows.Clear();
			GridRow row;
			switch((FieldLocations)comboFieldLocation.SelectedIndex) 
			{
				case FieldLocations.Account:
				case FieldLocations.Chart:
				case FieldLocations.Family:
				case FieldLocations.GroupNote:
				case FieldLocations.OrthoChart:
					for(int i=0;i<_listPatFieldDefs.Count;i++) {
						if(_listFieldDefLinks.Exists(x => x.FieldDefNum==_listPatFieldDefs[i].PatFieldDefNum
							&& x.FieldLocation==(FieldLocations)comboFieldLocation.SelectedIndex
							&& x.FieldDefType==FieldDefTypes.Patient))
						{
							continue; //If there is already a link for the patfield for the selected location, don't display the patfield in "display" grid.
						}
						row=new GridRow();
						row.Cells.Add(_listPatFieldDefs[i].FieldName);
						row.Tag=_listPatFieldDefs[i];
						gridDisplayed.ListGridRows.Add(row);
					}
					break;
				case FieldLocations.AppointmentEdit://AppointmentEdit is the only place where ApptFields are used.
					for(int i=0;i<_listApptFieldDefs.Count;i++) {
						if(_listFieldDefLinks.Exists(x => x.FieldDefNum==_listApptFieldDefs[i].ApptFieldDefNum
							&& x.FieldLocation==(FieldLocations)comboFieldLocation.SelectedIndex
							&& x.FieldDefType==FieldDefTypes.Appointment))
						{
							continue; //If there is already a link for the apptfield for the selected location, don't display the apptfield in "display" grid.
						}
						row=new GridRow();
						row.Cells.Add(_listApptFieldDefs[i].FieldName);
						row.Tag=_listApptFieldDefs[i];
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
			gridHidden.Columns.Clear();
			GridColumn col=new GridColumn("",20);
			col.IsWidthDynamic=true; 
			gridHidden.Columns.Add(col);
			gridHidden.ListGridRows.Clear();
			GridRow row;
			List<FieldDefLink> listFieldDefLinksForLoc=_listFieldDefLinks.FindAll(x => x.FieldLocation==(FieldLocations)comboFieldLocation.SelectedIndex);
			List<FieldDefLink> listFieldDefLinksToDelete=new List<FieldDefLink>();//Some links could exists to deleted FieldDefs prior to 18.1
			for(int i=0;i<listFieldDefLinksForLoc.Count;i++) {
				switch(listFieldDefLinksForLoc[i].FieldDefType) {
					case FieldDefTypes.Patient:
						PatFieldDef patFieldDef=_listPatFieldDefs.Find(x => x.PatFieldDefNum==listFieldDefLinksForLoc[i].FieldDefNum);
						if(patFieldDef==null) {//orphanded FK link to deleted PatFieldDef
							listFieldDefLinksToDelete.Add(listFieldDefLinksForLoc[i]);//orphanded FK link to deleted PatFieldDef
							continue;
						}
						row=new GridRow();
						row.Cells.Add(patFieldDef.FieldName);
						row.Tag=listFieldDefLinksForLoc[i];
						gridHidden.ListGridRows.Add(row);
					break;
					case FieldDefTypes.Appointment:
						ApptFieldDef apptFieldDef=_listApptFieldDefs.Find(x => x.ApptFieldDefNum==listFieldDefLinksForLoc[i].FieldDefNum);
						if(apptFieldDef==null) {//orphaned FK link to deleted ApptFieldDef
							listFieldDefLinksToDelete.Add(listFieldDefLinksForLoc[i]);//orphaned FK link to deleted ApptFieldDef
							continue;
						}
						row=new GridRow();
						row.Cells.Add(apptFieldDef.FieldName);
						row.Tag=listFieldDefLinksForLoc[i];
						gridHidden.ListGridRows.Add(row);
					break;
				}
			}
			//Remove all orphaned links
			for(int i=0;i<listFieldDefLinksToDelete.Count;i++) {
				_listFieldDefLinks.Remove(listFieldDefLinksToDelete[i]);
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
					if(ApptFieldDefInUseOnApptView(fieldDefLink)) {
						return;
					}
					_listFieldDefLinks.Add(fieldDefLink);
					break;
			}
			FillGridDisplayed();
			FillGridHidden();
		}

		//Hiding appt field defs on the appointment edit window while they are in use on an appointment view can lead to a confusing disconnect (an item hidden in the appt edit window, but showing on the appointment itself). Stopping users from hiding field from the appointment edit window that are already in use on an appointment view helps to prevent this disconnect.
		private bool ApptFieldDefInUseOnApptView(FieldDefLink fieldDefLink) {
			if(ApptViewItems.GetWhere(x => x.ApptFieldDefNum == fieldDefLink.FieldDefNum,true).Count > 0) {
				string fieldName = ApptFieldDefs.GetFieldName(fieldDefLink.FieldDefNum);
				ShowInUseWarning(fieldDefLink,fieldName);
				return true;
			}
			return false;
		}

		private void ShowInUseWarning(FieldDefLink fieldDefLink,string fieldName) {
			StringBuilder message = new StringBuilder();
			List<long> listApptViewNums = ApptViewItems.GetWhere(x => x.ApptFieldDefNum == fieldDefLink.FieldDefNum && fieldDefLink.FieldDefType == FieldDefTypes.Appointment)
				.Select(x => x.ApptViewNum)
				.Distinct()
				.ToList();
			List<ApptView> listApptViews = ApptViews.GetWhere(x => listApptViewNums.Contains(x.ApptViewNum));
			message.Append(Lan.g(this,"Unable to hide field"));
			message.AppendLine($" \"{fieldName}\".");
			message.AppendLine(Lan.g(this,"It is currently in use in the following Appointment Views:"));
			listApptViews.ForEach(x => message.AppendLine($" *{x.Description}"));
			MsgBox.Show(message.ToString());
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

		private void butSave_Click(object sender,EventArgs e) {
			FieldDefLinks.Sync(_listFieldDefLinks);
			DataValid.SetInvalid(InvalidType.PatFields);
			DialogResult=DialogResult.OK;
		}

	}
}