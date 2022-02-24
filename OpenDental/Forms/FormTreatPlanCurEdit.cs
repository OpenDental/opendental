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
		private List<TreatPlanAttach> _listTpAttachesAll;
		///<summary>All TreatPlanAttaches in _listTpAttachesAll with TreatPlanCur.TreatPlanNum.</summary>
		private List<TreatPlanAttach> _listTpAttachesCur;
		///<summary>All procedures for the patient with ProcStatus of TP or TPi.</summary>
		private List<Procedure> _listTpProcsAll;
		///<summary>All procedures in _listTpProcsAll with either a TreatPlanAttach with the same ProcNum
		///or, if TreatPlanCur is the Active TP, procedures with AptNum>0 or PlannedAptNum>0.</summary>
		private List<Procedure> _listTpProcsCur;
		private List<Appointment> _listAppointments;
		private long _apptNum;
		private long _apptNumPlanned;
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
			_listTpAttachesAll=TreatPlanAttaches.GetAllForPatNum(TreatPlanCur.PatNum);
			_listTpAttachesCur=_listTpAttachesAll.FindAll(x => x.TreatPlanNum==TreatPlanCur.TreatPlanNum);
			_listTpProcsAll=Procedures.GetProcsByStatusForPat(TreatPlanCur.PatNum,new[] { ProcStat.TP,ProcStat.TPi });
			ProcedureLogic.SortProcedures(ref _listTpProcsAll);
			_listTpProcsCur=_listTpProcsAll.FindAll(x => _listTpAttachesCur.Any(y => x.ProcNum==y.ProcNum) 
				|| (TreatPlanCur.TPStatus==TreatPlanStatus.Active && (x.AptNum>0 || x.PlannedAptNum>0)));
			_listAppointments=Appointments.GetMultApts(_listTpProcsAll.SelectMany(x => new[] { x.AptNum,x.PlannedAptNum }).Distinct().Where(x => x>0).ToList());
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
			comboPlanType.Items.AddRange(Enum.GetNames(typeof(TreatPlanType)));
			comboPlanType.SelectedIndex=(int)TreatPlanCur.TPType;
		}

		private void FillGrids() {
			FillGrid(ref gridTP);
			FillGrid(ref gridAll);
		}

		///<summary>Both grid s should be filled with the same columns. This method prevents the need for two nearly identical fill grid patterns.</summary>
		private void FillGrid(ref GridOD grid) {
			grid.BeginUpdate();
			grid.ListGridColumns.Clear();
			grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"Status"),40));
			grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"Tth"),30));
			grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"Surf"),40));
			grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"Code"),40));
			grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"Description"),195));
			grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"TPs"),35));
			grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"Apt"),40));
			grid.ListGridRows.Clear();
			GridRow row;
			List<Procedure> listProcs;
			if(grid==gridAll) {
				listProcs=_listTpProcsAll.FindAll(x => _listTpProcsCur.All(y => x.ProcNum!=y.ProcNum));
			}
			else {
				listProcs=_listTpProcsCur;
			}
			foreach(Procedure proc in listProcs) {
				row=new GridRow();
				ProcedureCode proccode=ProcedureCodes.GetProcCode(proc.CodeNum);
				string description=ProcedureCodes.GetLaymanTerm(proc.CodeNum);
				if(proccode.IsCanadianLab && proc.ProcNumLab!=0) {
					description="^ ^ "+description;
				}
				row.Cells.Add(proc.ProcStatus.ToString());
				row.Cells.Add(Tooth.ToInternat(proc.ToothNum));
				row.Cells.Add(proc.Surf);
				row.Cells.Add(proccode.ProcCode);
				row.Cells.Add(description);
				row.Cells.Add(_listTpAttachesAll.FindAll(x => x.ProcNum==proc.ProcNum && x.TreatPlanNum!=_treatPlanUnassigned.TreatPlanNum).Count.ToString());
				string aptStatus="";
				foreach(long aptNum in new[] {proc.AptNum,proc.PlannedAptNum}.Where(x => x>0)) {
					Appointment apt=_listAppointments.FirstOrDefault(x => x.AptNum==aptNum);
					if(apt!=null) {
						switch(apt.AptStatus) {
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
				row.Tag=proc;
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
		}

		private void SelectLabFees(ref GridOD grid) {
			List<long> listSelectedProcNums=grid.SelectedTags<Procedure>().Select(x => x.ProcNum).ToList();
			listSelectedProcNums.AddRange(grid.SelectedTags<Procedure>().Where(x => x.ProcNumLab > 0).Select(x => x.ProcNumLab).ToList());
			//Go through the entire grid and select any procedures that have a ProcNum that matches selected ProcNums or ProcNumLabs.
			List<Procedure> listAllProcs=grid.GetTags<Procedure>();
			for(int i=0;i < listAllProcs.Count;i++) {
				if(ListTools.In(listAllProcs[i].ProcNum,listSelectedProcNums) || ListTools.In(listAllProcs[i].ProcNumLab,listSelectedProcNums)) {
					grid.SetSelected(i,true);//Either a selected procedure or one of the labs associated to a selected procedure.
				}
			}
		}

		private void butLeft_Click(object sender,EventArgs e) {
			if(gridAll.GetSelectedIndex()<0) {
				return;
			}
			foreach(int idx in gridAll.SelectedIndices) {
				Procedure procCur=(Procedure)gridAll.ListGridRows[idx].Tag;
				if(procCur==null) {
					continue;
				}
				_listTpProcsCur.Add(procCur);
			}
			FillGrids();
		}

		private void butRight_Click(object sender,EventArgs e) {
			if(gridTP.GetSelectedIndex()<0) {
				return;
			}
			bool hasMsgShow=false;
			foreach(int idx in gridTP.SelectedIndices) {
				Procedure procCur=(Procedure)gridTP.ListGridRows[idx].Tag;
				if(procCur==null || (TreatPlanCur.TPStatus==TreatPlanStatus.Active && (procCur.AptNum>0 || procCur.PlannedAptNum>0))) {
					//if active TP, don't allow scheduled procedures to me moved off the TP.
					if(!hasMsgShow) {//Only show msgbox once.
						string msg="Not allowed to move prodecures to Available Procedures that are attached to an Appointment.";
						 if(procCur.PlannedAptNum>0) {
							msg="Not allowed to move prodecures to Available Procedures that are attached to a Planned Appointment.";
						}
						MsgBox.Show(this,msg);
						hasMsgShow=true;
					}
					continue;
				}
				_listTpProcsCur.Remove(procCur);
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
			Procedure proc=(Procedure)grid.ListGridRows[row].Tag;
			if(proc==null) {
				return;//should never happen
			}
			if(proc.AptNum>0) {
				contextMenuProcs.Items.Add(menuItemGotToAppt);
				_apptNum=proc.AptNum;
			}
			if(proc.PlannedAptNum>0) {
				contextMenuProcs.Items.Add(menuItemGoToPlanned);
				_apptNumPlanned=proc.PlannedAptNum;
			}
			if(contextMenuProcs.Items.Count!=0) {
				grid.ContextMenuStrip=contextMenuProcs;
			}
		}

		private void menuItemGotToAppt_Click(object sender,EventArgs e) {
			using FormApptEdit FormAPT=new FormApptEdit(_apptNum);
			FormAPT.ShowDialog();
			//consider refreshing data
		}

		private void menuItemGoToPlanned_Click(object sender,EventArgs e) {
			using FormApptEdit FormAPT=new FormApptEdit(_apptNumPlanned);
			FormAPT.ShowDialog();
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
			_listTpProcsCur.RemoveAll(x => x.AptNum>0 || x.PlannedAptNum>0); //to prevent duplicate additions
			_listTpProcsCur.AddRange(_listTpProcsAll.FindAll(x => x.AptNum>0 || x.PlannedAptNum>0));
			FillGrids();
		}

		private void butOK_Click(object sender,EventArgs e) {
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
			//get all TPAttaches for this TP where there is either a procedure with a TPAttach linking it to this TP
			//or, if this TP is active, a procedure linked to an appt by AptNum or PlannedAptNun
			List<TreatPlanAttach> listNew=_listTpAttachesCur.FindAll(x => _listTpProcsCur.Any(y => x.ProcNum==y.ProcNum));
			_listTpProcsCur.FindAll(x => !listNew.Any(y => x.ProcNum==y.ProcNum))
				.ForEach(x => listNew.Add(new TreatPlanAttach() { TreatPlanNum=TreatPlanCur.TreatPlanNum,ProcNum=x.ProcNum,Priority=0 }));
			TreatPlanAttaches.Sync(listNew,TreatPlanCur.TreatPlanNum);
			if(_makeActive) {
				TreatPlans.SetOtherActiveTPsToInactive(TreatPlanCur);
			}
			if(TreatPlanCur.TPStatus==TreatPlanStatus.Active) {
				//we have to this whether we just made this the active or it was already active, otherwise any procs we move off of the active plan will
				//retain the TP status and AuditPlans will throw them back on this TP.
				//Changing the status to TPi of any procs that are not on this plan prevents that from happening.
				Procedures.SetTPActive(TreatPlanCur.PatNum,listNew.Select(x => x.ProcNum).ToList());
			}
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