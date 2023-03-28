using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormRpEraAutoProcessed:FormODBase {
		///<summary>A list of all X835Status values.</summary>
		List<X835Status> _listX835StatusesAll;
		///<summary>The data for the ERA grid's currently selected row.</summary>
		private EraRowData _eraRowDataSelected;
		///<summary>The data for the Claim grid's currently selected row.</summary>
		private ClaimRowData _claimRowDataSelected;

		///<summary></summary>
		public FormRpEraAutoProcessed() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		///<summary></summary>
		private void FormRpEraAutoProcessed_Load(object sender,EventArgs e) {
			base.SetFilterControlsAndAction(() => RefreshGridEras(),dateRangePicker,textCarrier,textCheckTrace,comboClinics,listAutoProcessStatuses,checkShowAcknowledged);
			SetFiltersAndDefaults();
			BuildContextMenuGridEras();
			BuildContextMenuGridClaims();
			FillGridEras();
			FillGridClaims(null);//Just to init columns for the Claims and Claim Adjustment Procedures grids.
		}

		///<summary>Populate filters and set default selections.</summary>
		private void SetFiltersAndDefaults() {
			//We always get X835s for all X835Statuses, so we fill this list once on load.
			_listX835StatusesAll=Enum.GetValues(typeof(X835Status)).AsEnumerable<X835Status>().ToList();
			if(PrefC.HasClinicsEnabled) {
				comboClinics.IsAllSelected=true;//Defaults to 'All' so that 835s with missing clinic will show.
			}
			if(!PrefC.GetBool(PrefName.EraShowStatusAndClinic)) {
				comboClinics.Visible=false;
			}
			dateRangePicker.SetDateTimeFrom(DateTime.Today.AddDays(-7));
			dateRangePicker.SetDateTimeTo(DateTime.Today);
			List<X835AutoProcessed> listX835AutoProcessedValues=Enum.GetValues(typeof(X835AutoProcessed)).AsEnumerable<X835AutoProcessed>().ToList();
			for(int i=0;i<listX835AutoProcessedValues.Count;i++) {
				if(listX835AutoProcessedValues[i]==X835AutoProcessed.None) {
					continue;
				}
				listAutoProcessStatuses.Items.Add(Lan.g(this,listX835AutoProcessedValues[i].GetDescription()),listX835AutoProcessedValues[i]);
			}
			listAutoProcessStatuses.SetAll(true);
		}

		///<summary>Builds the context menu for the ERAs grid.</summary>
		private void BuildContextMenuGridEras() {
			List<MenuItem> listMenuItems=new List<MenuItem>();
			listMenuItems.Add(new MenuItem(Lan.g(this,"Acknowledge"),new EventHandler(MenuItemAcknowledge_Click)));
			ContextMenu contextMenu=new ContextMenu(listMenuItems.ToArray());
			gridEras.ContextMenu=contextMenu;
		}

		///<summary>Builds the context menu for the Claims grid.</summary>
		private void BuildContextMenuGridClaims() {
			List<MenuItem> listMenuItems=new List<MenuItem>();
			listMenuItems.Add(new MenuItem(Lan.g(this,"Go to Account"),new EventHandler(MenuItemGoToAccount_Click)));
			listMenuItems.Add(new MenuItem(Lan.g(this,"Process Unsent Secondary Claims"),new EventHandler(MenuItemProcessSecondaryClaims_Click)));
			ContextMenu contextMenu=new ContextMenu(listMenuItems.ToArray());
			gridClaims.ContextMenu=contextMenu;
		}

		///<summary>Fill the ERAs grid. Gets data from the DB every time it is called.</summary>
		private void FillGridEras() {
			bool showStatusAndClinics=PrefC.GetBool(PrefName.EraShowStatusAndClinic);
			gridEras.BeginUpdate();
			gridEras.Columns.Clear();
			gridEras.ListGridRows.Clear();
			#region Initilize columns
			gridEras.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","Patient Name"),250));
			gridEras.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","Carrier Name"),250));
			if(showStatusAndClinics) {
				gridEras.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","Status"),80,HorizontalAlignment.Center));
			}
			gridEras.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","Date"),80,GridSortingStrategy.DateParse));
			if(showStatusAndClinics && PrefC.HasClinicsEnabled) {
				gridEras.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","Clinic"),80,HorizontalAlignment.Center));
			}
			gridEras.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","EraPaid"),63,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridEras.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","InsEst"),63,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridEras.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","InsPaid"),63,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridEras.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","InsVar"),63,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridEras.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","W/O Est"),63,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridEras.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","Writeoff"),63,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridEras.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","W/O Var"),63,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			#endregion Initilize columns
			#region Get data
			DateTime dateFrom=dateRangePicker.GetDateTimeFrom();
			DateTime dateTo=dateRangePicker.GetDateTimeTo(true);
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				EtransL.AddMissingEtrans835s(dateFrom,dateTo);
			};
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled) {
				gridEras.EndUpdate();
				MsgBox.Show(this,"Cancel was clicked before the ERA table could finish loading. Click Refresh to load ERAs.");
				return;
			}
			List<long> listClinicNumsSelected=GetSelectedClinicNums();
			string carrierName=textCarrier.Text;
			string checkTraceNum=textCheckTrace.Text;
			List<X835AutoProcessed> listAutoProcessStatusesSelected=listAutoProcessStatuses.GetListSelected<X835AutoProcessed>();
			bool doIncludeAcknowledged=checkShowAcknowledged.Checked;
			bool areAllClinicsSelected=comboClinics.IsAllSelected;
			EraData eraData=new EraData();
			List<Hx835_ShortClaimProc> listShortClaimProcs=new List<Hx835_ShortClaimProc>();
			progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				eraData=EtransL.GetEraDataFiltered(showStatusAndClinics,_listX835StatusesAll,listClinicNumsSelected,
					amountMin:"",amountMax:"",dateFrom,dateTo,carrierName,checkTraceNum,controlId:"",doIncludeAutomatableCarriersOnly:false,
					areAllClinicsSelected,listAutoProcessStatusesSelected,doIncludeAcknowledged);
				List<long> listClaimNums=eraData.ListAttached.Select(x => x.ClaimNum).ToList();
				listShortClaimProcs=Hx835_ShortClaimProc.RefreshForClaims(listClaimNums);
			};
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled) {
				gridEras.EndUpdate();
				MsgBox.Show(this,"Cancel was clicked before the ERA table could finish loading. Click Refresh to load ERAs.");
				return;
			}
			#endregion Get data
			#region Fill rows
			for(int i=0;i<eraData.ListEtrans.Count;i++) {
				Etrans835 etrans835=eraData.ListEtrans835s[i];
				List<Etrans835Attach> listAttachedForRow=eraData.ListAttached.FindAll(x => x.EtransNum==etrans835.EtransNum);
				List<long> listClaimNumsForRow=listAttachedForRow.Select(x => x.ClaimNum).ToList();
				List<Hx835_ShortClaimProc> listShortClaimProcsForRow=listShortClaimProcs.FindAll(x => listClaimNumsForRow.Contains(x.ClaimNum));
				double insEstSum=listShortClaimProcsForRow.Sum(x => x.InsPayEst);
				double insPaidSum=listShortClaimProcsForRow.Sum(x => x.InsPayAmt);
				double writeOffSum=listShortClaimProcsForRow.Sum(x => x.WriteOff);
				double writeOffEstSum=CalcWriteOffEstSumForRow(listShortClaimProcsForRow);
				GridRow row=new GridRow();
				row.Cells.Add(etrans835.PatientName);
				row.Cells.Add(etrans835.PayerName);
				if(showStatusAndClinics) {
					row.Cells.Add(etrans835.Status.GetDescription());
				}
				row.Cells.Add(eraData.ListEtrans[i].DateTimeTrans.ToShortDateString());
				if(showStatusAndClinics && PrefC.HasClinicsEnabled) {
					string clinicAbbr=GetClinicAbbrForEraGridRow(listAttachedForRow,eraData.ListEtrans[i]);
					row.Cells.Add(clinicAbbr);
				}
				row.Cells.Add(etrans835.InsPaid.ToString("f"));
				row.Cells.Add(insEstSum.ToString("f"));
				row.Cells.Add(insPaidSum.ToString("f"));
				row.Cells.Add((insEstSum-insPaidSum).ToString("f"));
				row.Cells.Add(writeOffEstSum.ToString("f"));
				row.Cells.Add(writeOffSum.ToString("f"));
				row.Cells.Add((writeOffEstSum-writeOffSum).ToString("f"));
				row.Tag=new EraRowData(etrans835,eraData.ListEtrans[i]);
				gridEras.ListGridRows.Add(row);
			}
			#endregion Fill rows
			gridEras.EndUpdate();
		}

		///<summary>If no clinics are selected, we select All.</summary>
		private List<long> GetSelectedClinicNums() {
			List<long> listClinicNums=new List<long>();
			if(PrefC.HasClinicsEnabled) {
				if(comboClinics.ListSelectedClinicNums.Count==0) {
					comboClinics.IsAllSelected=true;//All clinics.
				}
				listClinicNums=comboClinics.ListSelectedClinicNums;
			}
			return listClinicNums;
		}

		///<summary>Determines the string to put in the Clinic column of the ERAs grid. ERAs may have claims from multiple cliinics.</summary>
		private string GetClinicAbbrForEraGridRow(List<Etrans835Attach> listAttached,Etrans etrans) {
			List<long> listClinicNums=listAttached.Select(x => x.ClinicNum).Distinct().ToList();
			string clinicAbbr="";
			if(listClinicNums.Count==1) {
				if(listClinicNums[0]==0) {
					clinicAbbr=Lan.g(this,"Unassigned");
				}
				else {
					clinicAbbr=Clinics.GetAbbr(listClinicNums[0]);
				}
			}
			else if(listClinicNums.Count>1) {
				clinicAbbr="("+Lan.g(this,"Multiple")+")";
			}
			return clinicAbbr;
		}

		///<summary>Sums the estimated writeoffs for the list of ClaimProcs. Uses the WriteOffEstOverride if it isn't -1.</summary>
		private double CalcWriteOffEstSumForRow(List<Hx835_ShortClaimProc> listShortClaimProcs) {
			double sum=0;
			for(int i=0;i<listShortClaimProcs.Count;i++) {
				if(!CompareDouble.IsEqual(listShortClaimProcs[i].WriteOffEstOverride,-1)) {
					sum+=listShortClaimProcs[i].WriteOffEstOverride;
				}
				else if(!CompareDouble.IsEqual(listShortClaimProcs[i].WriteOffEst,-1)) {
					sum+=listShortClaimProcs[i].WriteOffEst;
				}
			}
			return sum;
		}

		///<summary></summary>
		private void butRefresh_Click(object sender,EventArgs e) {
			RefreshGridEras();
		}

		private void RefreshGridEras() {
			gridEras.SetAll(false);
			FillGridClaims(null);//Passing null will clear the Claims and Claim Procedure Adjustments grids.
			FillGridEras();
		}

		///<summary>Updates Etrans835.IsApproved to true for each selected row in the ERA grid.</summary>
		private void MenuItemAcknowledge_Click(object sender,EventArgs e) {
			AcknowledgeErasSelected();
		}

		///<summary>Updates Etrans835.IsApproved to true for each selected row in the ERA grid.</summary>
		private void butAcknowledge_Click(object sender,EventArgs e) {
			AcknowledgeErasSelected();
		}

		///<summary>Sets Etrans835.IsApproved to true for all selected ERAs and updates them.
		///If checkShowAcknowledged isn't checked, acknowledged rows are removed from the grid.</summary>
		private void AcknowledgeErasSelected() {
			List<int> listIndicesSelected=gridEras.SelectedIndices.ToList();
			if(listIndicesSelected.IsNullOrEmpty()) {
				MsgBox.Show(this,"No ERAs are selected.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			gridEras.BeginUpdate();
			bool wasUpdateMade=false;
			for(int i=listIndicesSelected.Count-1;i>=0;i--) {
				int index=listIndicesSelected[i];
				EraRowData eraRowData=(EraRowData)gridEras.ListGridRows[index].Tag;
				if(eraRowData.Etrans835ForRow.IsApproved) {
					continue;//Already acknowledged and checkShowAcknowledged must be checked.
				}
				Etrans835 etrans835Old=eraRowData.Etrans835ForRow.Copy();
				eraRowData.Etrans835ForRow.IsApproved=true;
				Etrans835s.Update(eraRowData.Etrans835ForRow,etrans835Old);
				wasUpdateMade=true;
				if(!checkShowAcknowledged.Checked) {
					gridEras.ListGridRows.RemoveAt(index);
				}
			}
			gridEras.EndUpdate();
			FillGridClaims(null);//Passing null will clear the Claims and Claim Procedure Adjustments grids.
			Cursor=Cursors.Default;
			if(!wasUpdateMade) {
				MsgBox.Show(this,"All selected ERAs are already acknowledged.");
			}
		}

		///<summary>Opens the ERA that was double clicked.</summary>
		private void gridEras_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			EraRowData eraRowDataSelected=gridEras.SelectedTag<EraRowData>();
			EtransL.ViewFormForEra(eraRowDataSelected.EtransForRow,this);
		}

		///<summary>Refreshes the Claims grid to show claims for the selected ERA. If multiple ERAs are selected, claims for the first selected row are shown.</summary>
		private void gridEras_MouseUp(object sender,MouseEventArgs e) {
			RefreshGridClaims();
		}

		///<summary>This button is only visible when the selected ERA has multiple EOBs.
		///Forces a refresh of the Claims grid even though the Selected ERA has not changed so that the user can select a new EOB.</summary>
		private void butSelectEob_Click(object sender,EventArgs e) {
			RefreshGridClaims(doForceRefresh:true);
		}

		///<summary>Only refreshes if new ERA selected is different from the old one, or if we are forcing a refresh to allow the user to select a new EOB.</summary>
		private void RefreshGridClaims(bool doForceRefresh=false) {
			EraRowData eraRowDataSelectedNew=gridEras.SelectedTag<EraRowData>();
			if(!doForceRefresh 
				&& _eraRowDataSelected!=null 
				&& eraRowDataSelectedNew!=null 
				&& _eraRowDataSelected.EtransForRow.EtransNum==eraRowDataSelectedNew.EtransForRow.EtransNum)
			{
				return;//Same ERA was selected, so we don't need to refill the claims grid.
			}
			FillGridClaims(eraRowDataSelectedNew);
		}

		///<summary>Passing null will clear the Claims Claim Procedure Adjustments grids. Gets data from the DB each time it is filled.</summary>
		private void FillGridClaims(EraRowData eraRowData) {
			Cursor=Cursors.WaitCursor;
			butSelectEob.Visible=false;
			_eraRowDataSelected=eraRowData;
			gridClaims.BeginUpdate();
			gridClaims.Columns.Clear();
			gridClaims.ListGridRows.Clear();
			#region Init columns
			gridClaims.Columns.Add(new GridColumn(Lan.g("TableEtrans835Claims","Recd"),65,HorizontalAlignment.Center));
			gridClaims.Columns.Add(new GridColumn(Lan.g("TableEtrans835Claims","Patient Name"),250));
			gridClaims.Columns.Add(new GridColumn(Lan.g("TableEtrans835Claims","Type"),80,HorizontalAlignment.Center));
			gridClaims.Columns.Add(new GridColumn(Lan.g("TableEtrans835Claims","DateService"),80,GridSortingStrategy.DateParse));
			if(PrefC.HasClinicsEnabled) {
				gridClaims.Columns.Add(new GridColumn(Lan.g("TableEtrans835Claims","Clinic"),80,HorizontalAlignment.Center));//Consider translation category name.
			}
			gridClaims.Columns.Add(new GridColumn(Lan.g("TableEtrans835Claims","EraPaid"),60,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridClaims.Columns.Add(new GridColumn(Lan.g("TableEtrans835Claims","InsEst"),60,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridClaims.Columns.Add(new GridColumn(Lan.g("TableEtrans835Claims","InsPaid"),60,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridClaims.Columns.Add(new GridColumn(Lan.g("TableEtrans835Claims","InsVar"),60,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridClaims.Columns.Add(new GridColumn(Lan.g("TableEtrans835Claims","W/O Est"),60,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridClaims.Columns.Add(new GridColumn(Lan.g("TableEtrans835Claims","Writeoff"),60,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridClaims.Columns.Add(new GridColumn(Lan.g("TableEtrans835Claims","W/O Var"),60,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridClaims.Columns.Add(new GridColumn(Lan.g("TableEtrans835Claims","Supp?"),65,HorizontalAlignment.Center));
			gridClaims.Columns.Add(new GridColumn(Lan.g("TableEtrans835Claims","ClaimAdj?"),65,HorizontalAlignment.Center));
			gridClaims.Columns.Add(new GridColumn(Lan.g("TableEtrans835Claims","MoreClaims?"),65,HorizontalAlignment.Center));
			#endregion Init columns
			if(eraRowData==null) {
				gridClaims.EndUpdate();
				FillGridClaimProcedureAdjustments(null);
				Cursor=Cursors.Default;
				return;
			}
			#region Get ERA data 
			string messageText835=EtransMessageTexts.GetMessageText(eraRowData.EtransForRow.EtransMessageTextNum,false);
			Etrans etrans=eraRowData.EtransForRow;
			string tranSetIdSelected=GetTranSetIdForEobSelected(etrans,messageText835);
			if(tranSetIdSelected==null) {//Only null if user was prompted to select an EOB and they canceled instead of choosing one.
				_eraRowDataSelected=null;
				gridClaims.EndUpdate();
				FillGridClaimProcedureAdjustments(null);
				Cursor=Cursors.Default;
				return;
			}
			List<Etrans835Attach> listAttached=Etrans835Attaches.GetForEtrans(etrans.EtransNum);
			X835 x835=new X835(etrans,messageText835,tranSetIdSelected,listAttached);
			x835.RefreshAttachesAndClaimProcsFromDb(out listAttached,out List<Hx835_ShortClaimProc> listShortClaimProcs);
			List<long> listClaimNums=x835.ListClaimsPaid.Select(x => x.ClaimNum).Where(x => x!=0).ToList();
			List<Hx835_ShortClaim> listShortClaims=Hx835_ShortClaim.GetClaimsFromClaimNums(listClaimNums);
			List<long> listProcNumsOnClaims=listShortClaimProcs.Select(x => x.ProcNum).Where(x => x!=0).ToList();
			List<Hx835_ShortClaimProc> listShortClaimProcsForUnsentClaimsAll=Hx835_ShortClaimProc.GetUnsentForProcNums(listProcNumsOnClaims);
			#endregion Get ERA data
			#region Fill rows
			for(int i=0;i<x835.ListClaimsPaid.Count;i++) {
				Hx835_Claim claimPaid=x835.ListClaimsPaid[i];
				Hx835_ShortClaim shortClaim=listShortClaims.FirstOrDefault(x => x.ClaimNum==claimPaid.ClaimNum);//Will be null if no claim is attached.
				string claimStatus="";
				bool isClaimPaidProcessed=false;
				if(claimPaid.IsProcessed(listShortClaimProcs,listAttached)) {
					isClaimPaidProcessed=true;
					claimStatus="X";
				}
				else if((claimPaid.IsAttachedToClaim && claimPaid.ClaimNum==0) || !claimPaid.IsAttachedToClaim) {
					claimStatus=Lan.g("TableETrans835Claims","No Claim Attached");
				}
				string stringDateService=claimPaid.DateServiceStart.ToShortDateString();
				if(claimPaid.DateServiceEnd>claimPaid.DateServiceStart) {
					stringDateService+=" to "+claimPaid.DateServiceEnd.ToShortDateString();
				}
				string stringClaimType="";
				string stringClinic="";
				string stringInsEst="";
				string stringInsPaid="";
				string stringInsEstVar="";
				string stringWriteoff="";
				string stringWriteoffEst="";
				string stringWriteOffVar="";
				string stringOtherUnsentClaims="";
				List<Hx835_ShortClaimProc> listShortClaimProcsForClaim=new List<Hx835_ShortClaimProc>();
				List<Hx835_ShortClaimProc> listShortClaimProcsForUnsentClaims=new List<Hx835_ShortClaimProc>();
				if(shortClaim!=null) {
					listShortClaimProcsForClaim=listShortClaimProcs.FindAll(x => x.ClaimNum==shortClaim.ClaimNum);
					Enum.TryParse(shortClaim.ClaimType,out EnumClaimType claimType);
					stringClaimType=claimType.GetDescription();
					stringClinic=Clinics.GetAbbr(shortClaim.ClinicNum);
					if(stringClinic.IsNullOrEmpty()) {
						stringClinic=Lan.g("TableEtrans835Claims","Unassigned");
					}
					double insPaySum=listShortClaimProcsForClaim.Sum(x => x.InsPayAmt);
					double insEstSum=listShortClaimProcsForClaim.Sum(x => x.InsPayEst);
					double writeOffSum=listShortClaimProcsForClaim.Sum(x => x.WriteOff);
					double writeOffEstSum=CalcWriteOffEstSumForRow(listShortClaimProcsForClaim);
					stringInsEst=insEstSum.ToString("f");
					stringInsPaid=insPaySum.ToString("f");
					stringInsEstVar=(insEstSum-insPaySum).ToString("f");
					stringWriteoff=writeOffSum.ToString("f");
					stringWriteoffEst=writeOffEstSum.ToString("f");
					stringWriteOffVar=(writeOffEstSum-writeOffSum).ToString("f");
					listShortClaimProcsForUnsentClaims=FilterClaimProcsForUnsentClaims(listShortClaimProcsForUnsentClaimsAll,listShortClaimProcsForClaim);
					if(listShortClaimProcsForUnsentClaims.Count>0) {
						stringOtherUnsentClaims="X";
					}
				}
				string stringSupplemental="";
				if(claimPaid.GetIsSupplemental(listAttached,listShortClaimProcs)) {
					stringSupplemental="X";
				}
				string stringClaimAdjustments="";
				//If any procedures on the claim have a claim adjustment on the ERA
				if(claimPaid.ListProcs.SelectMany(x => x.ListProcAdjustments).Count()>0) {
					stringClaimAdjustments="X";
				}
				GridRow row=new GridRow();
				row.Cells.Add(claimStatus);
				row.Cells.Add(claimPaid.PatientName.ToString());
				row.Cells.Add(stringClaimType);
				row.Cells.Add(stringDateService);
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(stringClinic);
				}
				row.Cells.Add(claimPaid.InsPaid.ToString("f"));
				row.Cells.Add(stringInsEst);
				row.Cells.Add(stringInsPaid);
				row.Cells.Add(stringInsEstVar);
				row.Cells.Add(stringWriteoffEst);
				row.Cells.Add(stringWriteoff);
				row.Cells.Add(stringWriteOffVar);
				row.Cells.Add(stringSupplemental);
				row.Cells.Add(stringClaimAdjustments);
				row.Cells.Add(stringOtherUnsentClaims);
				row.Tag=new ClaimRowData(claimPaid,isClaimPaidProcessed,shortClaim,listShortClaimProcsForUnsentClaims);
				gridClaims.ListGridRows.Add(row);
			}
			#endregion Fill rows
			gridClaims.EndUpdate();
			Cursor=Cursors.Default;
		}

		///<summary>Takes a list of Hx835_ShortClaimProcs for unsent claims that share one or more procedures with any claim on the selected ERA,
		///and the list of Hx835_shortClaimProcs for the current claim being processed on the claim grid. Returns a list of Hx835_ShortClaimProcs for unsent claims that share one
		///or more procedures with the current claim.///</summary>
		private List<Hx835_ShortClaimProc> FilterClaimProcsForUnsentClaims(List<Hx835_ShortClaimProc> listShortClaimProcsForUnsentClaims,
			List<Hx835_ShortClaimProc> listShortClaimProcsForClaim)
		{
			if(listShortClaimProcsForUnsentClaims.Count==0 || listShortClaimProcsForClaim.Count==0) {
				return new List<Hx835_ShortClaimProc>();
			}
			List<long> listProcNumsForClaim=listShortClaimProcsForClaim.Select(x => x.ProcNum).ToList();
			//Get all claimprocs for unsent claims. Make sure they are not for our current claim.
			return listShortClaimProcsForUnsentClaims.FindAll(x => x.ClaimNum!=listShortClaimProcsForClaim[0].ClaimNum && listProcNumsForClaim.Contains(x.ProcNum));
		}

		///<summary>Etrans from 14.2 or older versions can represent multiple EOBs. These should be rare, but we need to prompt the user
		///when they select one of these etrans to ask which EOB they want to populate the claims grid with.
		///If a blank string is returned, the user was prompted to select an EOB and hit cancel.</summary>
		private string GetTranSetIdForEobSelected(Etrans etrans,string messageText835) {
			X12object x835=new X12object(messageText835);
			List<string> listTranSetIds=x835.GetTranSetIds();
			string transSetIdSelected=etrans.TranSetId835;
			if(listTranSetIds.Count>=2 && etrans.TranSetId835=="") {
				using FormEtrans835PickEob formPickEob=new FormEtrans835PickEob(listTranSetIds,messageText835,etrans,doOpenEtrans835:false);
				Cursor=Cursors.Default;
				formPickEob.ShowDialog();
				Cursor=Cursors.WaitCursor;
				transSetIdSelected=formPickEob.TransSetIdSelected;//May be null if user closed without picking an EOB.
				butSelectEob.Visible=true;
			}
			return transSetIdSelected;
		}

		///<summary>Goes to the patients account for the selected claim.</summary>
		private void MenuItemGoToAccount_Click(object sender,EventArgs e) {
			Hx835_Claim claimPaid=gridClaims.SelectedTag<ClaimRowData>().ClaimPaid;
			if(claimPaid==null) {
				MsgBox.Show(this,"No claim is selected.");
				return;
			}
			EtransL.GoToAccountForHx835_Claim(claimPaid);
		}

		///<summary>Validates that the selected claim has procedures that are on one or more secondary claims with a status of Unsent or Hold Unti Pri Recieved.
		///If so, the user is prompted to send these claims, change there status to "Waiting to send", or do nothing.</summary>
		private void MenuItemProcessSecondaryClaims_Click(object sender,EventArgs e) {
			ClaimRowData claimRowDataSelected=gridClaims.SelectedTag<ClaimRowData>();
			if(claimRowDataSelected==null) {
				MsgBox.Show(this,"No claim is selected.");
				return;
			}
			List<Hx835_ShortClaimProc> listShortClaimProcsForUnsentClaims=claimRowDataSelected.ListShortClaimProcsForUnsentClaims;
			if(listShortClaimProcsForUnsentClaims.IsNullOrEmpty()) {
				MsgBox.Show(this,"There are no unsent claims for the procedures on the selected claim.");
				return;
			}
			List<long> listClaimNums=listShortClaimProcsForUnsentClaims.Select(x => x.ClaimNum).ToList();
			List<Hx835_ShortClaim> shortClaimsUnsent=Hx835_ShortClaim.GetClaimsFromClaimNums(listClaimNums);
			if(!shortClaimsUnsent.Any(x => x.ClaimType=="S" && x.ClaimStatus.In("U","H","I"))) {
				MsgBox.Show(this,"There are unsent claim(s) for one or more procedures on the selected claim, " 
					+"but none of them are for secondary claims with a status of 'Unsent', 'Hold Until Pri Received', or 'Hold for In Process'. "
					+"Please go to the account to send them.");
				return;
			}
			Hx835_ShortClaim shortClaim=claimRowDataSelected.ShortClaim;
			//The selected claim must be recieved and processed properly before we can send secondary claims.
			if(shortClaim.ClaimStatus!="R" || !claimRowDataSelected.IsProcessed) {//R=Received
				MsgBox.Show(this,"The selected claim must be received before secondary claims can be sent.");
				return;
			}
			List<ClaimProc> listClaimProcsForClaim=ClaimProcs.RefreshForClaim(shortClaim.ClaimNum);
			ClaimL.PromptForSecondaryClaim(listClaimProcsForClaim);
		}

		///<summary>Opens the Edit Claim window for the claim if one is attached to the Hx835_Claim for the row.</summary>
		private void gridClaims_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Hx835_Claim claimPaid=gridClaims.SelectedTag<ClaimRowData>().ClaimPaid;
			Claim claim=claimPaid.GetClaimFromDb();
			if(claim==null) {
				MsgBox.Show(this,"The selected claim payment is not attached to a claim.");
				return;
			}
			else if(Security.IsAuthorized(Permissions.ClaimView)) {
				Patient pat=Patients.GetPat(claim.PatNum);
				Family fam=Patients.GetFamily(claim.PatNum);
				using FormClaimEdit formClaimEdit=new FormClaimEdit(claim,pat,fam);
				formClaimEdit.ShowDialog();//Modal, because the user could edit information in this window.
			}
		}

		///<summary>Only refreshes the Claim Procedure Adjustments grid if the new claim selected is different from the previous one.</summary>
		private void gridClaims_SelectionCommitted(object sender,EventArgs e) {
			ClaimRowData claimRowDataSelectedNew=gridClaims.SelectedTag<ClaimRowData>();
			if(_claimRowDataSelected!=null
				&& claimRowDataSelectedNew!=null
				&& _claimRowDataSelected.ClaimPaid.Era.EtransSource.EtransNum==claimRowDataSelectedNew.ClaimPaid.Era.EtransSource.EtransNum
				&& _claimRowDataSelected.ClaimPaid.ClpSegmentIndex==claimRowDataSelectedNew.ClaimPaid.ClpSegmentIndex)
			{
				return;//Same claim was selected, so we don't need to refill the claim procedure detail grid.
			}
			FillGridClaimProcedureAdjustments(claimRowDataSelectedNew);
		}

		///<summary>Passing null will clear the grid.</summary>
		private void FillGridClaimProcedureAdjustments(ClaimRowData claimRowData) {
			_claimRowDataSelected=claimRowData;
			gridClaimProcedureAdjustments.BeginUpdate();
			gridClaimProcedureAdjustments.Columns.Clear();
			gridClaimProcedureAdjustments.ListGridRows.Clear();
			#region Init columns
			int widthProcColumn=35;
			int widthProcCodeColumn=70;
			int widthDescriptionColumn=200;
			int widthAdjAmtColumn=65;
			int widthReasonColumn=gridClaimProcedureAdjustments.Width-10-widthProcColumn-widthProcCodeColumn-widthDescriptionColumn-widthAdjAmtColumn;
			gridClaimProcedureAdjustments.Columns.Add(new GridColumn(Lan.g("TableEtrans835ClaimProcedureAdjustments","Proc"),widthProcColumn,HorizontalAlignment.Center));
			gridClaimProcedureAdjustments.Columns.Add(new GridColumn(Lan.g("TableEtrans835ClaimProcedureAdjustments","ProcCode"),widthProcCodeColumn));
			gridClaimProcedureAdjustments.Columns.Add(new GridColumn(Lan.g("TableEtrans835ClaimProcedureAdjustments","Description"),widthDescriptionColumn));
			gridClaimProcedureAdjustments.Columns.Add(new GridColumn(Lan.g("TableEtrans835ClaimProcedureAdjustments","Reason"),widthReasonColumn));
			gridClaimProcedureAdjustments.Columns.Add(new GridColumn(Lan.g("TableEtrans835ClaimProcedureAdjustments","AdjAmt"),
				widthAdjAmtColumn,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			#endregion Init columns
			if(claimRowData==null) {
				gridClaimProcedureAdjustments.EndUpdate();
				return;
			}
			#region Fill rows
			int procCount=1;
			for(int i=0;i<claimRowData.ClaimPaid.ListProcs.Count;i++) {
				Hx835_Proc proc=claimRowData.ClaimPaid.ListProcs[i];
				if(proc.ListProcAdjustments.Count==0) {
					continue;
				}
				for(int j=0;j<proc.ListProcAdjustments.Count;j++) {
					Hx835_Adj adj=proc.ListProcAdjustments[j];
					GridRow row=new GridRow();
					row.Cells.Add((procCount).ToString());
					row.Cells.Add(proc.ProcCodeBilled);
					row.Cells.Add(adj.AdjustRemarks);
					row.Cells.Add(adj.ReasonDescript);
					row.Cells.Add(adj.AdjAmt.ToString("f"));
					gridClaimProcedureAdjustments.ListGridRows.Add(row);
				}
				procCount++;
			}
			#endregion Fill rows
			gridClaimProcedureAdjustments.EndUpdate();
		}

		///<summary></summary>
		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
			Close();
		}

		///<summary>Holds data for the rows of the ERA grid.</summary>
		private class EraRowData {
			public Etrans835 Etrans835ForRow;
			public Etrans EtransForRow;

			///<summary></summary>
			public EraRowData(Etrans835 etrans835,Etrans etrans) {
				Etrans835ForRow=etrans835;
				EtransForRow=etrans;
			}
		}

		///<summary>Holds data for the rows of the Claims grid.</summary>
		private class ClaimRowData {
			public Hx835_Claim ClaimPaid;
			public bool IsProcessed;
			public List<Hx835_ShortClaimProc> ListShortClaimProcsForUnsentClaims=new List<Hx835_ShortClaimProc>();
			public Hx835_ShortClaim ShortClaim;

			///<summary></summary>
			public ClaimRowData(Hx835_Claim claimPaid,bool isProcessed,Hx835_ShortClaim shortClaim,List<Hx835_ShortClaimProc> listShortClaimProcsForUnsentClaims) {
				ClaimPaid=claimPaid;
				IsProcessed=isProcessed;
				ShortClaim=shortClaim;
				ListShortClaimProcsForUnsentClaims=listShortClaimProcsForUnsentClaims;
			}
		}

		private void FormRpEraAutoProcessed_Shown(object sender,EventArgs e) {
			SecurityLogs.MakeLogEntry(Permissions.InsPayCreate,0,"Window 'ERA's Automatically Processed' opened.");
		}
	}
}