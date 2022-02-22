using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormEtrans835s:FormODBase {
	
		///<summary>Start date used to populate _listEtranss.</summary>
		private DateTime _dateFrom=DateTime.MaxValue;
		///<summary>End date used to populate _listEtranss.</summary>
		private DateTime _dateTo=DateTime.MaxValue;
		private List<X835Status> _listX835Statuses=new List<X835Status>();

		public FormEtrans835s() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		
		private void FormEtrans835s_Load(object sender,EventArgs e) {
			base.SetFilterControlsAndAction((() => FilterAndFillGrid()),
				textRangeMin,textRangeMax,textControlId,textCarrier,textCheckTrace,comboClinics,checkShowFinalizedOnly,listStatus,checkAutomatableCarriersOnly,dateRangePicker
			);
			dateRangePicker.SetDateTimeFrom(DateTime.Today.AddDays(-7));
			dateRangePicker.SetDateTimeTo(DateTime.Today);
			#region User Clinics
			if(PrefC.HasClinicsEnabled) {
				comboClinics.IsAllSelected=true;//Defaults to 'All' so that 835s with missing clinic will show.
			}
			#endregion
			#region Statuses
			if(PrefC.GetBool(PrefName.EraShowStatusAndClinic)) {
				checkShowFinalizedOnly.Visible=false;
			}
			else {
				labelStatus.Visible=false;
				listStatus.Visible=false;
				checkAutomatableCarriersOnly.Visible=false;
				comboClinics.Visible=false;
      }
			for(int i=0;i<Enum.GetValues(typeof(X835Status)).Length;i++) {
				X835Status x835Status=(X835Status)i;
				if(ListTools.In(x835Status,X835Status.None,X835Status.FinalizedSomeDetached,X835Status.FinalizedAllDetached)) {
					//FinalizedSomeDetached and FinalizedAllDetached are shown via Finalized.
					continue;
				}
				listStatus.Items.Add(Lan.g(this,x835Status.GetDescription()));
				_listX835Statuses.Add(x835Status);
				bool isSelected=true;
				if(x835Status==X835Status.Finalized) {
					isSelected=false;
				}
				listStatus.SetSelected(listStatus.Items.Count-1,isSelected);
			}
			#endregion
		}

		private void FormEtrans835s_Shown(object sender,EventArgs e) {
			if(PrefC.GetBool(PrefName.EraRefreshOnLoad)) {
				//This must be in Shown due to the progress bar forcing this window behind other windows.
				FilterAndFillGrid();
			}
		}

		///<summary>Allows you to pass in predetermined filter options.</summary>
		private void FillGrid(List<long> listSelectedClinicNums,string carrierName,string checkTraceNum,
			string amountMin,string amountMax,string controlId,bool doShowAutomatableCarriersOnly)
		{
			Cursor=Cursors.WaitCursor;
			labelControlId.Visible=PrefC.GetBool(PrefName.EraShowControlIdFilter);
			textControlId.Visible=PrefC.GetBool(PrefName.EraShowControlIdFilter);
			bool showStatusAndClinics=PrefC.GetBool(PrefName.EraShowStatusAndClinic);
			_dateFrom=dateRangePicker.GetDateTimeFrom();
			_dateTo=dateRangePicker.GetDateTimeTo(isDefaultMaxDateT:true);
			UI.ProgressOD progressOD=new UI.ProgressOD();
			progressOD.ActionMain=() => {
				EtransL.AddMissingEtrans835s(_dateFrom,_dateTo);
			};
			progressOD.ShowDialogProgress();
			progressOD=new UI.ProgressOD();
			progressOD.ActionMain=() => {
#region Filters
				if(PrefC.HasClinicsEnabled) {
					if(comboClinics.ListSelectedClinicNums.Count==0){
						comboClinics.IsAllSelected=true;//All clinics.
					}
				}
				List<X835Status> listX835Statuses=new List<X835Status>();
				if(showStatusAndClinics) {
					for(int i=0;i<listStatus.SelectedIndices.Count;i++) {//Add the selected statuses to the list.
						listX835Statuses.Add(_listX835Statuses[listStatus.SelectedIndices[i]]);
					}
					if(listX835Statuses.Contains(X835Status.Finalized)) {//Our list in the UI only allows the user to select "Finalized" thus we include all 3 finalized statuses.
						listX835Statuses.Add(X835Status.FinalizedAllDetached);
						listX835Statuses.Add(X835Status.FinalizedSomeDetached);
					}
				}
				else if(checkShowFinalizedOnly.Checked) {
					listX835Statuses=new List<X835Status>(new X835Status[] { X835Status.Finalized,X835Status.FinalizedAllDetached,X835Status.FinalizedSomeDetached });
				}
				else {
					listX835Statuses=new List<X835Status>(new X835Status[] { X835Status.NotFinalized,X835Status.Partial,X835Status.Unprocessed });
				}
				EraData eraDataFiltered=EtransL.GetEraDataFiltered(showStatusAndClinics,listX835Statuses,listSelectedClinicNums,amountMin,amountMax,
					_dateFrom,_dateTo,carrierName,checkTraceNum,controlId,doShowAutomatableCarriersOnly,comboClinics.IsAllSelected);
#endregion Filters
				gridMain.Invoke(gridMain.BeginUpdate);
#region Initilize columns
				gridMain.Columns.Clear();
				gridMain.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","Patient Name"),250));
				gridMain.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","Carrier Name"),190));
				if(showStatusAndClinics) {
					gridMain.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","Status"),80));
				}
				gridMain.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","Date"),80,GridSortingStrategy.DateParse));
				gridMain.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","Amount"),80,GridSortingStrategy.AmountParse));
				if(showStatusAndClinics && PrefC.HasClinicsEnabled) {
					gridMain.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","Clinic"),70));
				}
				gridMain.Columns.Add(new GridColumn(Lan.g("TableEtrans835s","Code"),37,HorizontalAlignment.Center));
				GridColumn col;
				if(PrefC.GetBool(PrefName.EraShowControlIdFilter)) {
					col=new GridColumn(Lan.g("TableEtrans835s","ControlID"),70);
					col.IsWidthDynamic=true;
					gridMain.Columns.Add(col);
				}
				col=new GridColumn(Lan.g("TableEtrans835s","Note"),250);
				col.IsWidthDynamic=true;
				col.DynamicWeight=2;
				gridMain.Columns.Add(col);
#endregion
#region Fill Rows
				gridMain.ListGridRows.Clear();
				for(int i=0;i<eraDataFiltered.ListEtrans.Count;i++) {
					ProgressBarEvent.Fire(ODEventType.ProgressBar,Lan.g(this,"Filling grid rows ")+(i+1)+"/"+eraDataFiltered.ListEtrans.Count);
					Etrans835 etrans835=eraDataFiltered.ListEtrans835s[i];
					GridRow row=new GridRow();
					row.Cells.Add(etrans835.PatientName);
					row.Cells.Add(etrans835.PayerName);
					if(showStatusAndClinics) {
						string status=Lan.g(this,etrans835.Status.GetDescription());
						row.Cells.Add(status);
					}
					row.Cells.Add(POut.Date(eraDataFiltered.ListEtrans[i].DateTimeTrans));
					row.Cells.Add(POut.Double(etrans835.InsPaid));
#region Column: Clinic
					if(showStatusAndClinics && PrefC.HasClinicsEnabled) {
						List<long> listClinicNums=eraDataFiltered.ListAttached.FindAll(x => x.EtransNum==eraDataFiltered.ListEtrans[i].EtransNum)
							.Select(x => x.ClinicNum).Distinct().ToList();
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
						row.Cells.Add(clinicAbbr);
					}
#endregion
					row.Cells.Add(etrans835.PaymentMethodCode);
					if(PrefC.GetBool(PrefName.EraShowControlIdFilter)) {
						row.Cells.Add(etrans835.ControlId);
					}
					row.Cells.Add(eraDataFiltered.ListEtrans[i].Note);
					row.Tag=eraDataFiltered.ListEtrans[i];
					gridMain.ListGridRows.Add(row);
				}
#endregion Fill Rows
				gridMain.Invoke(gridMain.EndUpdate);
			};
			progressOD.ShowDialogProgress();
			Cursor=Cursors.Default;
		}

		///<summary>Called when we need to filter the current in memory contents in _listEtrans. Calls FillGrid()</summary>
		private void FilterAndFillGrid() {
			List<long> listClinicNums=null;//A null signifies that clinics are disabled.
			if(PrefC.HasClinicsEnabled) {
				listClinicNums=comboClinics.ListSelectedClinicNums;
			}
			FillGrid(
				listSelectedClinicNums:					listClinicNums,
				carrierName:										textCarrier.Text,
				checkTraceNum:									textCheckTrace.Text,
				amountMin:											textRangeMin.Text,
				amountMax:											textRangeMax.Text,
				controlId:											textControlId.Text,
				doShowAutomatableCarriersOnly:	checkAutomatableCarriersOnly.Checked
			);
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FilterAndFillGrid();
		}

		private void gridMain_DoubleClick(object sender,EventArgs e) {
			int index=gridMain.GetSelectedIndex();
			if(index==-1) {//Clicked in empty space. 
				return;
			}
			//Mimics FormClaimsSend.gridHistory_CellDoubleClick(...)
			Cursor=Cursors.WaitCursor;
			Etrans etrans=(Etrans)gridMain.ListGridRows[index].Tag;
			//Sadly this is needed due to FormEtrans835Edit calling Etranss.Update .
			//See Etranss.RefreshHistory(...), this query does not select all etrans columns.
			//Mimics FormClaimsSend.gridHistory_CellDoubleClick(...)
			etrans=Etranss.GetEtrans(etrans.EtransNum);
			if(etrans==null) {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"ERA could not be found, it was most likely deleted.");
				FilterAndFillGrid();
				return;
			}
			EtransL.ViewFormForEra(etrans,this);
			Cursor=Cursors.Default;
		}

		///<summary>User will be blocked if they don't have permission to access the EraAutoProcessed report.</summary>
		private void butAutoProcessedEras_Click(object sender,EventArgs e) {
			DisplayReport displayReportEraAutoProcessed=DisplayReports.GetByInternalName(DisplayReports.ReportNames.EraAutoProcessed);
			if(displayReportEraAutoProcessed==null) {
				MsgBox.Show(this,"The "+DisplayReports.ReportNames.EraAutoProcessed+" report could not be found.");
				return;
			}
			if(!Security.IsAuthorized(Permissions.Reports) || !Security.IsAuthorized(Permissions.Reports,displayReportEraAutoProcessed.DisplayReportNum,suppressMessage:false)) {
				return;
			}
			FormRpEraAutoProcessed formRpEraAutoProcessed=new FormRpEraAutoProcessed();
			formRpEraAutoProcessed.Show();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
			Close();
		}

	}

}
