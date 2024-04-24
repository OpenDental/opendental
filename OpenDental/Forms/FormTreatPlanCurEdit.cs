using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormTreatPlanCurEdit:FormODBase {
		public TreatPlan TreatPlanCur;
		///<summary>The patient's current unassigned treatment plan or a new treatment plan if the patient does not have an unassigned plan.</summary>
		private TreatPlan _treatPlanUnassigned;
		///<summary>All TreatPlanAttaches for this patient.</summary>
		private List<TreatPlanAttach> _listTreatPlanAttachesAll;
		///<summary>All TreatPlanAttaches in _listTpAttachesAll with TreatPlanCur.TreatPlanNum.</summary>
		private List<TreatPlanAttach> _listTreatPlanAttaches;
		///<summary>All procedures for the patient with ProcStatus of TP or TPi.</summary>
		private List<Procedure> _listProceduresAll;
		///<summary>All procedures in _listTpProcsAll with either a TreatPlanAttach with the same ProcNum
		///or, if TreatPlanCur is the Active TP, procedures with AptNum>0 or PlannedAptNum>0.</summary>
		private List<Procedure> _listProcedures;
		private List<Appointment> _listAppointments;
		private long _aptNum;
		private long _aptNumPlanned;
		///<summary>Set to true if the "Make Active Treatment Plan" button is pressed.</summary>
		private bool _makeActive=false;

		public FormTreatPlanCurEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTreatPlanCurEdit_Load(object sender,EventArgs e) {
			if(TreatPlanCur==null || (TreatPlanCur.TPStatus!=TreatPlanStatus.Active && TreatPlanCur.TPStatus!=TreatPlanStatus.Inactive)) {
				throw new Exception("No treatment plan loaded.");
			}
			_treatPlanUnassigned=TreatPlans.GetUnassigned(TreatPlanCur.PatNum);
			this.Text=TreatPlanCur.Heading+" - {"+Lans.g(this,TreatPlanCur.TPStatus.ToString())+"}";
			_listTreatPlanAttachesAll=TreatPlanAttaches.GetAllForPatNum(TreatPlanCur.PatNum);
			_listTreatPlanAttaches=_listTreatPlanAttachesAll.FindAll(x => x.TreatPlanNum==TreatPlanCur.TreatPlanNum);
			_listProceduresAll=Procedures.GetProcsByStatusForPat(TreatPlanCur.PatNum,new[] { ProcStat.TP,ProcStat.TPi });
			ProcedureLogic.SortProcedures(_listProceduresAll);
			_listProcedures=_listProceduresAll.FindAll(x => _listTreatPlanAttaches.Any(y => x.ProcNum==y.ProcNum) 
				|| (TreatPlanCur.TPStatus==TreatPlanStatus.Active && (x.AptNum>0 || x.PlannedAptNum>0)));
			_listAppointments=Appointments.GetMultApts(_listProceduresAll.SelectMany(x => new[] { x.AptNum,x.PlannedAptNum }).Distinct().Where(x => x>0).ToList());
			textHeading.Text=TreatPlanCur.Heading;
			textNote.Text=TreatPlanCur.Note;
			FillGrids();
			if(TreatPlanCur.TPStatus==TreatPlanStatus.Inactive && TreatPlanCur.Heading==Lan.g("TreatPlan","Unassigned")) {
				gridTP.Title=Lan.g("TreatPlan","Unassigned Procedures");
				labelHeading.Visible=false;
				textHeading.Visible=false;
				labelNote.Visible=false;
				textNote.Visible=false;
				gridAll.Visible=false;
				butLeft.Visible=false;
				butRight.Visible=false;
				butOK.Enabled=false;
				butDelete.Visible=false;
			}
			if(TreatPlanCur.TPStatus==TreatPlanStatus.Active) {
				butMakeActive.Enabled=false;
				butDelete.Enabled=false;
			}
			comboPlanType.Items.AddList(Enum.GetNames(typeof(TreatPlanType)));
			comboPlanType.SelectedIndex=(int)TreatPlanCur.TPType;
		}

		private void FillGrids() {
			FillGrid(ref gridTP);
			FillGrid(ref gridAll);
		}

		///<summary>Both grid s should be filled with the same columns. This method prevents the need for two nearly identical fill grid patterns.</summary>
		private void FillGrid(ref GridOD grid) {
			grid.BeginUpdate();
			grid.Columns.Clear();
			grid.Columns.Add(new GridColumn(Lan.g(this,"Status"),40));
			grid.Columns.Add(new GridColumn(Lan.g(this,"Tth"),30));
			grid.Columns.Add(new GridColumn(Lan.g(this,"Surf"),40));
			grid.Columns.Add(new GridColumn(Lan.g(this,"Code"),40));
			grid.Columns.Add(new GridColumn(Lan.g(this,"Description"),195));
			grid.Columns.Add(new GridColumn(Lan.g(this,"TPs"),35));
			grid.Columns.Add(new GridColumn(Lan.g(this,"Apt"),40));
			grid.ListGridRows.Clear();
			GridRow row;
			List<Procedure> listProcedures;
			if(grid==gridAll) {
				listProcedures=_listProceduresAll.FindAll(x => _listProcedures.All(y => x.ProcNum!=y.ProcNum));
			}
			else {
				listProcedures=_listProcedures;
			}
			for(int i = 0;i<listProcedures.Count;i++) {
				row=new GridRow();
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(listProcedures[i].CodeNum);
				string description=ProcedureCodes.GetLaymanTerm(listProcedures[i].CodeNum);
				if(procedureCode.IsCanadianLab &&listProcedures[i].ProcNumLab!=0) {
					description="^ ^ "+description;
				}
				row.Cells.Add(listProcedures[i].ProcStatus.ToString());
				row.Cells.Add(Tooth.Display(listProcedures[i].ToothNum));
				string displaySurf;
				if(procedureCode.TreatArea==TreatmentArea.Sextant) {
					displaySurf=Tooth.GetSextant(listProcedures[i].Surf,(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
				}
				else {
					displaySurf=Tooth.SurfTidyFromDbToDisplay(listProcedures[i].Surf,listProcedures[i].ToothNum);
				}
				row.Cells.Add(displaySurf);
				row.Cells.Add(procedureCode.ProcCode);
				row.Cells.Add(description);
				row.Cells.Add(_listTreatPlanAttachesAll.FindAll(x => x.ProcNum==listProcedures[i].ProcNum && x.TreatPlanNum!=_treatPlanUnassigned.TreatPlanNum).Count.ToString());
				string aptStatus="";
				List<long> listAptNums = new List<long>();
				listAptNums.Add(listProcedures[i].AptNum);
				listAptNums.Add(listProcedures[i].PlannedAptNum);
				listAptNums = listAptNums.FindAll(x => x>0);
				for(int j = 0;j<listAptNums.Count;j++) {
					Appointment appointment =_listAppointments.Find(x => x.AptNum==listAptNums[j]);
					if(appointment!=null) {
						switch(appointment.AptStatus) {
							case ApptStatus.UnschedList:
								aptStatus+="U";
								break;
							case ApptStatus.Scheduled:
								aptStatus+="S";
								break;
							case ApptStatus.Complete:
								aptStatus+="C";
								break;
							case ApptStatus.Broken:
								aptStatus+="B";
								break;
							case ApptStatus.Planned:
								aptStatus+="P";
								break;
							case ApptStatus.PtNote:
							case ApptStatus.PtNoteCompleted:
							case ApptStatus.None:
							default:
								aptStatus+="!"; //should never happen
								break;
						}
					}
				}
				row.Cells.Add(aptStatus);
				row.Tag=listProcedures[i];
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
		}

		private void SelectLabFees(ref GridOD grid) {
			List<long> listProcNumsSelected=grid.SelectedTags<Procedure>().Select(x => x.ProcNum).ToList();
			listProcNumsSelected.AddRange(grid.SelectedTags<Procedure>().Where(x => x.ProcNumLab > 0).Select(x => x.ProcNumLab).ToList());
			//Go through the entire grid and select any procedures that have a ProcNum that matches selected ProcNums or ProcNumLabs.
			List<Procedure> listProceduresAll=grid.GetTags<Procedure>();
			for(int i=0;i < listProceduresAll.Count;i++) {
				if(listProcNumsSelected.Contains(listProceduresAll[i].ProcNum) || listProcNumsSelected.Contains(listProceduresAll[i].ProcNumLab)) {
					grid.SetSelected(i,true);//Either a selected procedure or one of the labs associated to a selected procedure.
				}
			}
		}

		private void butLeft_Click(object sender,EventArgs e) {
			if(gridAll.GetSelectedIndex()<0) {
				return;
			}
			for(int i = 0;i<gridAll.SelectedIndices.Length;i++) {
				Procedure procedure =(Procedure)gridAll.ListGridRows[gridAll.SelectedIndices[i]].Tag;
				if(procedure==null) {
					continue;
				}
				_listProcedures.Add(procedure);
			}
			FillGrids();
		}

		private void butRight_Click(object sender,EventArgs e) {
			if(gridTP.GetSelectedIndex()<0) {
				return;
			}
			bool hasMsgShow=false;
			for(int i = 0;i<gridTP.SelectedIndices.Length;i++) {
				Procedure procedure =(Procedure)gridTP.ListGridRows[gridTP.SelectedIndices[i]].Tag;
				if(procedure==null || (TreatPlanCur.TPStatus==TreatPlanStatus.Active && (procedure.AptNum>0 || procedure.PlannedAptNum>0))) {
					//if active TP, don't allow scheduled procedures to me moved off the TP.
					if(!hasMsgShow) {//Only show msgbox once.
						string msg="Not allowed to move procedures to Available Procedures that are attached to an Appointment.";
						 if(procedure.PlannedAptNum>0) {
							msg="Not allowed to move procedures to Available Procedures that are attached to a Planned Appointment.";
						}
						MsgBox.Show(this,msg);
						hasMsgShow=true;
					}
					continue;
				}
				_listProcedures.Remove(procedure);
			}
			FillGrids();
		}

		private void grids_CellClick(object sender,ODGridClickEventArgs e) {
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return;
			}
			GridOD grid=(GridOD)sender;
			SelectLabFees(ref grid);
		}

		private void grids_MouseDown(object sender,MouseEventArgs e) {
			contextMenuProcs.Items.Clear();
			gridAll.ContextMenu=null;
			gridTP.ContextMenu=null;
			GridOD grid=(GridOD)sender;
			int row=grid.PointToRow(e.Y);
			if(row<0 || row>=grid.ListGridRows.Count) {
				return;
			}
			Procedure procedure=(Procedure)grid.ListGridRows[row].Tag;
			if(procedure==null) {
				return;//should never happen
			}
			if(procedure.AptNum>0) {
				contextMenuProcs.Items.Add(menuItemGotToAppt);
				_aptNum=procedure.AptNum;
			}
			if(procedure.PlannedAptNum>0) {
				contextMenuProcs.Items.Add(menuItemGoToPlanned);
				_aptNumPlanned=procedure.PlannedAptNum;
			}
			if(contextMenuProcs.Items.Count!=0) {
				grid.ContextMenuStrip=contextMenuProcs;
			}
		}

		private void menuItemGotToAppt_Click(object sender,EventArgs e) {
			using FormApptEdit formApptEdit=new FormApptEdit(_aptNum);
			formApptEdit.ShowDialog();
			//consider refreshing data
		}

		private void menuItemGoToPlanned_Click(object sender,EventArgs e) {
			using FormApptEdit formApptEdit=new FormApptEdit(_aptNumPlanned);
			formApptEdit.ShowDialog();
			//consider refreshing data
		}

		private void butMakeActive_Click(object sender,EventArgs e) {
			_makeActive=true;//affects the OK click
			if(TreatPlanCur.TPStatus==TreatPlanStatus.Inactive && TreatPlanCur.Heading==Lan.g("TreatPlan","Unassigned")) {
				_treatPlanUnassigned=new TreatPlan();
				TreatPlanCur.Heading=Lan.g("TreatPlan","Active Treatment Plan");
				textHeading.Text=TreatPlanCur.Heading;
			}
			if(TreatPlanCur.TPStatus==TreatPlanStatus.Inactive && TreatPlanCur.Heading==Lan.g("TreatPlan","Inactive Treatment Plan")) {
				TreatPlanCur.Heading=Lan.g("TreatPlan","Active Treatment Plan");
				textHeading.Text=TreatPlanCur.Heading;
			}
			TreatPlanCur.TPStatus=TreatPlanStatus.Active;
			butMakeActive.Enabled=false;
			gridTP.Title=Lan.g("TreatPlan","Treatment Planned Procedures");
			labelHeading.Visible=true;
			textHeading.Visible=true;
			labelNote.Visible=true;
			textNote.Visible=true;
			gridAll.Visible=true;
			butLeft.Visible=true;
			butRight.Visible=true;
			butOK.Enabled=true;
			butDelete.Visible=true;
			butDelete.Enabled=false;
			_listProcedures.RemoveAll(x => x.AptNum>0 || x.PlannedAptNum>0); //to prevent duplicate additions
			_listProcedures.AddRange(_listProceduresAll.FindAll(x => x.AptNum>0 || x.PlannedAptNum>0));
			FillGrids();
		}

		private void butOK_Click(object sender,EventArgs e) {
			#region Validation
			if(string.IsNullOrWhiteSpace(textHeading.Text)) {
				MsgBox.Show(this,"Header name cannot be empty.");
				return;
			}
			#endregion Validation
			TreatPlanCur.Heading=textHeading.Text;
			TreatPlanCur.Note=textNote.Text;
			TreatPlanCur.UserNumPresenter=0;
			TreatPlanCur.TPType=(TreatPlanType)comboPlanType.SelectedIndex;
			if(TreatPlanCur.TreatPlanNum==0) {
				TreatPlanCur.TreatPlanNum=TreatPlans.Insert(TreatPlanCur);
			}
			else {
				TreatPlans.Update(TreatPlanCur);
			}
			TreatPlans.SyncTreatPlanStatusWithProcs(TreatPlanCur,_makeActive,_listTreatPlanAttaches,_listTreatPlanAttachesAll,_listProcedures);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(TreatPlanCur.TPStatus==TreatPlanStatus.Active) {
				MsgBox.Show(this,"Cannot delete active treatment plan."); //Should never happen.
				return;
			}
			if(TreatPlanCur.TreatPlanNum!=0) {
				try {
					TreatPlans.Delete(TreatPlanCur);
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
			}
			DialogResult=DialogResult.OK;
		}
	}
}