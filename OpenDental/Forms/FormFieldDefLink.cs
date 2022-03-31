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
			gridDisplayed.Columns.Clear();
			gridDisplayed.Columns.Add(new GridColumn("",20){ IsWidthDynamic=true });
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
			gridHidden.Columns.Clear();
			gridHidden.Columns.Add(new GridColumn("",20){ IsWidthDynamic=true });
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
					if(DefInUse(fieldDefLink)) {
						return; 
					}
					_listFieldDefLinks.Add(fieldDefLink);
					break;
				case FieldLocations.AppointmentEdit://AppointmentEdit is the only place where ApptFields are used.
					ApptFieldDef apptFieldDef=(ApptFieldDef)gridDisplayed.ListGridRows[gridDisplayed.GetSelectedIndex()].Tag;
					fieldDefLink=new FieldDefLink();
					fieldDefLink.FieldDefNum=apptFieldDef.ApptFieldDefNum;
					fieldDefLink.FieldDefType=FieldDefTypes.Appointment;
					fieldDefLink.FieldLocation=(FieldLocations)comboFieldLocation.SelectedIndex;
					if(DefInUse(fieldDefLink)) {
						return;
					}
					_listFieldDefLinks.Add(fieldDefLink);
					break;
			}
			FillGridDisplayed();
			FillGridHidden();
		}

		private bool DefInUse(FieldDefLink fieldDefLink) {
			string fieldName;
			switch(fieldDefLink.FieldDefType) {
				case FieldDefTypes.Patient:
					fieldName = PatFieldDefs.GetFieldName(fieldDefLink.FieldDefNum);
					if(PatFields.IsFieldNameInUse(fieldName)) {
						ShowInUseWarning(fieldDefLink,fieldName);
						return true;
					}
					return false;
				case FieldDefTypes.Appointment:
					fieldName = ApptFieldDefs.GetFieldName(fieldDefLink.FieldDefNum);
					if(ApptFields.IsFieldNameInUse(fieldName)) {
						ShowInUseWarning(fieldDefLink,fieldName);
						return true;
					}
					return false;
				default:
					return false;
			}
		}

		private void ShowInUseWarning(FieldDefLink fieldDefLink,string fieldName) {
			StringBuilder message = new StringBuilder();
			List<long> listApptViewNums = ApptViewItems.GetWhere(x => 
				(x.PatFieldDefNum == fieldDefLink.FieldDefNum && fieldDefLink.FieldDefType == FieldDefTypes.Patient)
				|| (x.ApptFieldDefNum == fieldDefLink.FieldDefNum && fieldDefLink.FieldDefType == FieldDefTypes.Appointment))
				.Select(x => x.ApptViewNum)
				.Distinct()
				.ToList();
			List<ApptView> listApptViews = ApptViews.GetWhere(x =>  listApptViewNums.Contains(x.ApptViewNum));
			message.Append(Lan.g(this,"Unable to hide field"));
			message.AppendLine($" \"{fieldName}\".");
			message.AppendLine(Lan.g(this,"It is currently in use in the following places:"));
			if(listApptViews.Count > 0) {
				message.AppendLine(Lan.g(this,"-Appointment Views:"));
			}
			listApptViews.ForEach(x => message.AppendLine($" *{x.Description}"));
			if(fieldDefLink.FieldDefType == FieldDefTypes.Patient) {
				message.AppendLine("\n"+Lan.g(this,"Patient Fields may also be in use on one or more patients."));
			}
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

		private void butOK_Click(object sender,EventArgs e) {
			FieldDefLinks.Sync(_listFieldDefLinks);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}