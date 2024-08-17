using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using System.Globalization;


namespace OpenDental {
	public partial class FormApptTypes:FormODBase {
		private List<AppointmentType> _listAppointmentTypes;
		///<summary>Stale deep copy of _listApptTypes to use with sync.</summary>
		private List<AppointmentType> _listAppointmentTypesOld;
		private bool _isChanged=false;
		public bool IsNoneAllowed;
		public bool IsSelectionMode;
		///<summary>Set to true when IsSelectionMode is true and the user will be able to select multiple appointment types instead of just one.
		///ListAppointmentTypesSelected will contain all of the types that the user selected.</summary>
		public bool AllowMultipleSelections;
		///<summary>The appointment type that was selected if IsSelectionMode is true.
		///If IsSelectionMode is true and this object is prefilled with an appointment type then the grid will preselect that type if possible.
		///It is not guaranteed that the appointment type will be selected.
		///This object should only be read from externally after DialogResult.OK has been returned.  Can be null.</summary></summary>
		public AppointmentType AppointmentTypeSelected;
		///<summary>Contains all of the selected appointment types if IsSelectionMode is true.
		///If IsSelectionMode and AllowMultiple are true, this object can be prefilled with appointment types which will be preselected if possible.
		///It is not guaranteed that all appointment types will be selected (due to hidden).
		///This list should only be read from externally after DialogResult.OK has been returned.</summary>
		public List<AppointmentType> ListAppointmentTypesSelected=new List<AppointmentType>();

		public FormApptTypes() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listAppointmentTypes=new List<AppointmentType>();
		}

		private void FormApptTypes_Load(object sender,EventArgs e) {
			if(IsSelectionMode) {
				butOK.Visible=true;
				butAdd.Visible=false;
				butDown.Visible=false;
				butUp.Visible=false;
				checkWarn.Visible=false;
				checkPrompt.Visible=false;
				if(AllowMultipleSelections) {
					this.Text=Lan.g(this,"Select Appointment Types");
					gridMain.SelectionMode=GridSelectionMode.MultiExtended;
				}
				else {
					this.Text=Lan.g(this,"Select Appointment Type");
				}
				gridMain.Location=new Point(8,6);
				gridMain.Size=new Size(320,447);
			}
			checkPrompt.Checked=PrefC.GetBool(PrefName.AppointmentTypeShowPrompt);
			checkWarn.Checked=PrefC.GetBool(PrefName.AppointmentTypeShowWarning);
			//don't show hidden appointment types in selection mode
			_listAppointmentTypes=AppointmentTypes.GetDeepCopy(IsSelectionMode);
			_listAppointmentTypesOld=AppointmentTypes.GetDeepCopy();
			FillMain();
			//Preselect the corresponding appointment type(s) once on load.  Do not do this within FillMain().
			if(IsSelectionMode) {
				if(AppointmentTypeSelected!=null) {
					ListAppointmentTypesSelected.Add(AppointmentTypeSelected);
				}
				for(int i=0;i<gridMain.ListGridRows.Count;i++) {
					if(((AppointmentType)gridMain.ListGridRows[i].Tag)!=null //The "None" option will always be null
						&& ListAppointmentTypesSelected.Any(x => x.AppointmentTypeNum==((AppointmentType)gridMain.ListGridRows[i].Tag).AppointmentTypeNum)) 
					{
						gridMain.SetSelected(i,true);
					}
				}
			}
		}

		private void FillMain() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableApptTypes","Name"),200);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableApptTypes","Color"),50,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableApptTypes","Hidden"),60,HorizontalAlignment.Center){ IsWidthDynamic=true };
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			_listAppointmentTypes.Sort(AppointmentTypes.SortItemOrder);
			for(int i=0;i<_listAppointmentTypes.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listAppointmentTypes[i].AppointmentTypeName);
				row.Cells.Add("");//color row, no text.
				row.Cells[1].ColorBackG=_listAppointmentTypes[i].AppointmentTypeColor;
				row.Cells.Add(_listAppointmentTypes[i].IsHidden ? "X" : "");
				row.Tag=_listAppointmentTypes[i];
				gridMain.ListGridRows.Add(row);
			}
			//Always add a None option to the end of the list when in selection mode.
			if(IsNoneAllowed) {
				row=new GridRow();
				row.Cells.Add(Lan.g(this,"None"));
				row.Cells.Add("");
				row.Cells.Add("");
				gridMain.ListGridRows.Add(row);
				row.Tag=null;
			}
			gridMain.EndUpdate();
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			if(gridMain.GetSelectedIndex()==0) {
				//Do nothing, the item is at the top of the list.
				return;
			}
			int index=gridMain.GetSelectedIndex();
			_isChanged=true;
			_listAppointmentTypes[index-1].ItemOrder+=1;
			_listAppointmentTypes[index].ItemOrder-=1;
			FillMain();
			index-=1;
			gridMain.SetSelected(index,true);
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			if(gridMain.GetSelectedIndex()==_listAppointmentTypes.Count-1) {
				//Do nothing, the item is at the bottom of the list.
				return;
			}
			int index=gridMain.GetSelectedIndex();
			_isChanged=true;
			_listAppointmentTypes[index+1].ItemOrder-=1;
			_listAppointmentTypes[index].ItemOrder+=1;
			FillMain();
			index+=1;
			gridMain.SetSelected(index,true);
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsSelectionMode) {
				ListAppointmentTypesSelected=gridMain.SelectedTags<AppointmentType>();
				AppointmentTypeSelected=ListAppointmentTypesSelected.FirstOrDefault();
				this.DialogResult=DialogResult.OK;
			}
			else {
				using FormApptTypeEdit formApptTypeEdit=new FormApptTypeEdit();
				formApptTypeEdit.AppointmentTypeCur=_listAppointmentTypes[e.Row];
				//This could happen if we haven't saved to the database after adding a new appointment type in FormApptTypeEdit
				//formApptTypeEdit.AppointmentTypeCur.IsNew=_listAppointmentTypes[e.Row].AppointmentTypeNum==0;
				formApptTypeEdit.ShowDialog();
				if(formApptTypeEdit.DialogResult!=DialogResult.OK) {
					return;
				}
				if(formApptTypeEdit.AppointmentTypeCur==null) {
					_listAppointmentTypes.RemoveAt(e.Row);
				}
				else {
					_listAppointmentTypes[e.Row]=formApptTypeEdit.AppointmentTypeCur;
				}
				_isChanged=true;
				FillMain();
			}
		}

		private void checkPrompt_CheckedChanged(object sender,EventArgs e) {
			_isChanged=true;
		}

		private void checkWarn_CheckedChanged(object sender,EventArgs e) {
			_isChanged=true;
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormApptTypeEdit formApptTypeEdit=new FormApptTypeEdit();
			formApptTypeEdit.AppointmentTypeCur=new AppointmentType();
			formApptTypeEdit.AppointmentTypeCur.ItemOrder=_listAppointmentTypes.Count;
			formApptTypeEdit.AppointmentTypeCur.IsNew=true;
			formApptTypeEdit.AppointmentTypeCur.AppointmentTypeColor=Color.FromArgb(0);//Default color to white, otherwise it picks a light gray.
			formApptTypeEdit.ShowDialog();
			if(formApptTypeEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			_listAppointmentTypes.Add(formApptTypeEdit.AppointmentTypeCur);
			_isChanged=true;
			FillMain();
		}

		/*
		///<summary>DEPRECATED - Originally writtento diagnose a bug in the middle teir that was solved by fixing Color serialization.
		///It appeared as though someonewasrepeatedly changing the appointment type colors.</summary>
		private void LogEntry(List<AppointmentType> listNew,List<AppointmentType> listDB) {
			listNew.Sort((AppointmentType x,AppointmentType y) => { return x.AppointmentTypeNum.CompareTo(y.AppointmentTypeNum); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.
			listDB.Sort((AppointmentType x,AppointmentType y) => { return x.AppointmentTypeNum.CompareTo(y.AppointmentTypeNum); });//Anonymous function, sorts by compairing PK.  Lambda expressions are not allowed, this is the one and only exception.  JS approved.
			int idxNew=0;
			int idxDB=0;
			AppointmentType fieldNew;
			AppointmentType fieldDB;
			//Because both lists have been sorted using the same criteria, we can now walk each list to determine which list contians the next element.  The next element is determined by Primary Key.
			//If the New list contains the next item it will be inserted.  If the DB contains the next item, it will be deleted.  If both lists contain the next item, the item will be updated.
			while(idxNew<listNew.Count || idxDB<listDB.Count) {
				fieldNew=null;
				if(idxNew<listNew.Count) {
					fieldNew=listNew[idxNew];//used to compare the new list to db list
				}
				fieldDB=null;				
				if(idxDB<listDB.Count) {
					fieldDB=listDB[idxDB];//used to compare the db list with new list
				}
				//begin compare
				if(fieldNew!=null && fieldDB==null) {//listNew has more items, listDB does not.
					SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Added new appointment type "+"'"+fieldNew.AppointmentTypeName+"'"+
						" Color '"+fieldNew.AppointmentTypeColor.ToKnownColor()+"',ItemOrder '"+fieldNew.ItemOrder+"', IsHidden '"+
						fieldNew.IsHidden+"'.");
					idxNew++;
					continue;
				}
				else if(fieldNew==null && fieldDB!=null) {//listDB has more items, listNew does not.
					SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Deleted appointment type "+"'"+fieldDB.AppointmentTypeName+"'"+" Color '"+
					fieldDB.AppointmentTypeColor.Name+"',ItemOrder '"+fieldDB.ItemOrder+"', IsHidden '"+fieldDB.IsHidden+"'.");
					idxDB++;
					continue;
				}
				else if(fieldNew.AppointmentTypeNum<fieldDB.AppointmentTypeNum) {//newPK less than dbPK, newItem is 'next'
					SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Added new appointment type "+"'"+fieldNew.AppointmentTypeName+"'"+
						" Color '"+fieldNew.AppointmentTypeColor.ToKnownColor()+"',ItemOrder '"+fieldNew.ItemOrder+"', IsHidden '"+
						fieldNew.IsHidden+"'.");
					idxNew++;
					continue;
				}
				else if(fieldNew.AppointmentTypeNum>fieldDB.AppointmentTypeNum) {//dbPK less than newPK, dbItem is 'next'
					SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Deleted appointment type "+"'"+fieldDB.AppointmentTypeName+"'"+" Color '"+
					fieldDB.AppointmentTypeColor.Name+"',ItemOrder '"+fieldDB.ItemOrder+"', IsHidden '"+fieldDB.IsHidden+"'.");
					idxDB++;
					continue;
				}
				//Both lists contain the 'next' item, update required
				string update="";
				if(fieldNew.AppointmentTypeName != fieldDB.AppointmentTypeName) {
					if(update!="") { update+=","; }
					update+="Name '"+fieldDB.AppointmentTypeName+"' to '"+fieldNew.AppointmentTypeName+"'";					
				}
				if(fieldNew.AppointmentTypeColor != fieldDB.AppointmentTypeColor) {
					if(update!="") { update+=","; }
					update+="Color '"+fieldDB.AppointmentTypeColor+"' to '"+fieldNew.AppointmentTypeColor+"'";
				}
				if(fieldNew.ItemOrder != fieldDB.ItemOrder) {
					if(update!="") { update+=","; }
					update+="Order '"+fieldDB.ItemOrder+"' to '"+fieldNew.ItemOrder+"'";
				}
				if(fieldNew.IsHidden != fieldDB.IsHidden) {
					if(update!="") { update+=","; }
					update+="Hidden '"+fieldDB.IsHidden+"' to '"+fieldNew.IsHidden+"'";
				}
				if(update=="") {
					//no changes
				}
				else {
					SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Updated appointment type '"+fieldDB.AppointmentTypeName+"' ApptTypeNum("
						+fieldDB.AppointmentTypeNum+"). Changes- "+update+".");
				}			
				idxNew++;
				idxDB++;
			}
		}
		*/

		private void butOK_Click(object sender,EventArgs e) {
			ListAppointmentTypesSelected=gridMain.SelectedTags<AppointmentType>();
			AppointmentTypeSelected=ListAppointmentTypesSelected.FirstOrDefault();
			this.DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender,EventArgs e) {
			if(IsSelectionMode) {
				DialogResult=DialogResult.Cancel;
			}
			Close();
		}

		private void FormApptTypes_FormClosing(object sender,FormClosingEventArgs e) {
			if(!IsSelectionMode) {
				if(_isChanged) {
					Prefs.UpdateBool(PrefName.AppointmentTypeShowPrompt,checkPrompt.Checked);
					Prefs.UpdateBool(PrefName.AppointmentTypeShowWarning,checkWarn.Checked);
					for(int i=0;i<_listAppointmentTypes.Count;i++) {
						_listAppointmentTypes[i].ItemOrder=i;
					}
					AppointmentTypes.Sync(_listAppointmentTypes,_listAppointmentTypesOld);
					List<AppointmentType> listAppointmentTypesOverlap=_listAppointmentTypesOld.FindAll(x => _listAppointmentTypes.Exists(y => x.AppointmentTypeNum==y.AppointmentTypeNum));
					for(int i=0;i<listAppointmentTypesOverlap.Count;i++) {
						long appointmentTypeNum=listAppointmentTypesOverlap[i].AppointmentTypeNum;
						AppointmentType appointmentTypeOld=listAppointmentTypesOverlap[i];
						AppointmentType appointmentType=_listAppointmentTypes.Find(x=>x.AppointmentTypeNum==appointmentTypeNum);
						string logEntry="";
						if(appointmentType.BlockoutTypes!=appointmentTypeOld.BlockoutTypes) {
							logEntry+="BlockoutTypes changed from '"+appointmentTypeOld.BlockoutTypes+"' to '"+appointmentType.BlockoutTypes+"'\r\n";
						}
						if(appointmentType.CodeStr!=appointmentTypeOld.CodeStr) {
							logEntry+="CodeStr changed from '"+appointmentTypeOld.CodeStr+"' to '"+appointmentType.CodeStr+"'\r\n";
						}
						if(appointmentType.CodeStrRequired!=appointmentTypeOld.CodeStrRequired) {
							logEntry+="CodeStrRequired changed from '"+appointmentTypeOld.CodeStrRequired+"' to '"+appointmentType.CodeStrRequired+"'\r\n";
						}
						if(!String.IsNullOrEmpty(logEntry)) {
							SecurityLogs.MakeLogEntry(Permissions.AppointmentTypeEdit,0,"Appointment Type \""+appointmentType.AppointmentTypeName+"\" edited.\r\n"+logEntry.Trim());
						}
					}
					//LogEntry(_listApptTypes,_listApptTypesOld);//no longer needed, originally used to diagnose a bugin the middle teir.
					DataValid.SetInvalid(InvalidType.AppointmentTypes);
					DataValid.SetInvalid(InvalidType.Prefs);
				}
				DialogResult=DialogResult.OK;
			}
		}
	}
}