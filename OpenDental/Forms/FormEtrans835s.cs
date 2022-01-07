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
		private DateTime _reportDateFrom=DateTime.MaxValue;
		///<summary>End date used to populate _listEtranss.</summary>
		private DateTime _reportDateTo=DateTime.MaxValue;
		private List<X835Status> _listStatuses=new List<X835Status>();

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
			foreach(X835Status status in Enum.GetValues(typeof(X835Status))) {
				if(ListTools.In(status,X835Status.None,X835Status.FinalizedSomeDetached,X835Status.FinalizedAllDetached)) {
					//FinalizedSomeDetached and FinalizedAllDetached are shown via Finalized.
					continue;
				}
				listStatus.Items.Add(Lan.g(this,status.GetDescription()));
				_listStatuses.Add(status);
				bool isSelected=true;
				if(status==X835Status.Finalized) {
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
		private void FillGrid(List<long> listSelectedClinicNums,string carrierName,string checkTraceNum,string amountMin,string amountMax,string controlId) {
			Cursor=Cursors.WaitCursor;
			labelControlId.Visible=PrefC.GetBool(PrefName.EraShowControlIdFilter);
			textControlId.Visible=PrefC.GetBool(PrefName.EraShowControlIdFilter);
			bool showStatusAndClinics=PrefC.GetBool(PrefName.EraShowStatusAndClinic);
			List<Etrans> listEtransFiltered=new List<Etrans>();
			List<Etrans835> listFilteredEtrans835s=new List<Etrans835>();
			_reportDateFrom=dateRangePicker.GetDateTimeFrom();
				_reportDateTo=dateRangePicker.GetDateTimeTo(true);
			UI.ProgressOD progressOD=new UI.ProgressOD();
			progressOD.ActionMain=() => {
#region Add Missing Etrans835s
				List<long> listEtransNumsMissingEtrans835s=Etranss.GetErasMissingEtrans835(_reportDateFrom,_reportDateTo);
				List<Etrans> listEtransMissingEtrans835s=Etranss.GetMany(listEtransNumsMissingEtrans835s.ToArray());
				//Running a set of queries/commands here for each ERA is fine because this will only happen once for each ERA and we have a progress bar.
				for(int i=0;i<listEtransMissingEtrans835s.Count;i++) {
					ProgressBarEvent.Fire(ODEventType.ProgressBar,Lan.g(this,"Creating Etrans835 records")+" "+(i+1)+"/"+listEtransMissingEtrans835s.Count);
					string messageText835=EtransMessageTexts.GetMessageText(listEtransMissingEtrans835s[i].EtransMessageTextNum);
					List<Etrans835Attach> listAttachedTo835=Etrans835Attaches.GetForEtrans(listEtransMissingEtrans835s[i].EtransNum);
					X835 x835=new X835(listEtransMissingEtrans835s[i],messageText835,listEtransMissingEtrans835s[i].TranSetId835,listAttachedTo835,true);
					Etrans835 etrans835=new Etrans835();
					etrans835.EtransNum=listEtransMissingEtrans835s[i].EtransNum;
					Etrans835s.Upsert(etrans835,x835);
				}
#endregion Add Missing Etrans835s
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
				ProgressBarEvent.Fire(ODEventType.ProgressBar,Lan.g(this,"Refreshing history."));
				List<X835Status> listStatuses=new List<X835Status>();
				if(showStatusAndClinics) {
					for(int i=0;i<listStatus.SelectedIndices.Count;i++) {//Add the selected statuses to the list.
						listStatuses.Add(_listStatuses[listStatus.SelectedIndices[i]]);
					}
					if(listStatuses.Contains(X835Status.Finalized)) {//Our list in the UI only allows the user to select "Finalized" thus we include all 3 finalized statuses.
						listStatuses.Add(X835Status.FinalizedAllDetached);
						listStatuses.Add(X835Status.FinalizedSomeDetached);
					}
				}
				else if(checkShowFinalizedOnly.Checked) {
					listStatuses=new List<X835Status>(new X835Status[] { X835Status.Finalized,X835Status.FinalizedAllDetached,X835Status.FinalizedSomeDetached });
				}
				else {
					listStatuses=new List<X835Status>(new X835Status[] { X835Status.NotFinalized,X835Status.Partial,X835Status.Unprocessed });
				}
				ProgressBarEvent.Fire(ODEventType.ProgressBar,Lan.g(this,"Performing basic filters."));
				decimal insPaidMin=-1;
				if(amountMin!="") {
					insPaidMin=PIn.Decimal(amountMin);
				}
				decimal insPaidMax=-1;
				if(amountMax!="") {
					insPaidMax=PIn.Decimal(amountMax);
				}
				listFilteredEtrans835s=Etrans835s.GetFiltered(_reportDateFrom,_reportDateTo,carrierName,checkTraceNum,insPaidMin,insPaidMax,controlId,listStatuses.ToArray());
				List<Etrans> listEtrans=Etranss.GetMany(listFilteredEtrans835s.Select(x => x.EtransNum).ToArray());
				listEtransFiltered=listEtrans;
				List<Etrans835Attach> listAttached=Etrans835Attaches.GetForEtrans(false,listEtrans.Select(x => x.EtransNum).ToArray());//Gets non-db columns also.
				if(showStatusAndClinics && PrefC.HasClinicsEnabled && !comboClinics.IsAllSelected) {//Filter by selected clinics if applicable.
					ProgressBarEvent.Fire(ODEventType.ProgressBar,Lan.g(this,"Filtering by clinic."));
					listAttached=listAttached.FindAll(x => listSelectedClinicNums.Contains(x.ClinicNum));
					listEtransFiltered=Etranss.GetMany(listAttached.Select(x => x.EtransNum).ToArray());
				}
				//Ensure listFilteredEtrans835s are listed in same order and with same total count as the listEtransFiltered.
				listFilteredEtrans835s=listEtransFiltered.Select(x => listFilteredEtrans835s.FirstOrDefault(y => y.EtransNum==x.EtransNum)).ToList();
				if(checkAutomatableCarriersOnly.Checked) {
					ProgressBarEvent.Fire(ODEventType.ProgressBar,Lan.g(this,"Filtering by automatable carriers."));
					FilterAutomatableERAs(listEtransFiltered,listFilteredEtrans835s,listAttached);
				}
#endregion Filters
				gridMain.Invoke(gridMain.BeginUpdate);
#region Initilize columns
				gridMain.ListGridColumns.Clear();
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableEtrans835s","Patient Name"),250));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableEtrans835s","Carrier Name"),190));
				if(showStatusAndClinics) {
					gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableEtrans835s","Status"),80));
				}
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableEtrans835s","Date"),80,GridSortingStrategy.DateParse));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableEtrans835s","Amount"),80,GridSortingStrategy.AmountParse));
				if(showStatusAndClinics && PrefC.HasClinicsEnabled) {
					gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableEtrans835s","Clinic"),70));
				}
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableEtrans835s","Code"),37,HorizontalAlignment.Center));
				if(PrefC.GetBool(PrefName.EraShowControlIdFilter)) {
					gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableEtrans835s","ControlID"),70){ IsWidthDynamic=true });
				}
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableEtrans835s","Note"),250){ IsWidthDynamic=true,DynamicWeight=2 });
#endregion
#region Fill Rows
				gridMain.ListGridRows.Clear();
				for(int i=0;i<listEtransFiltered.Count;i++) {
					ProgressBarEvent.Fire(ODEventType.ProgressBar,Lan.g(this,"Filling grid rows ")+(i+1)+"/"+listEtransFiltered.Count);
					Etrans835 etrans835=listFilteredEtrans835s[i];
					GridRow row=new GridRow();
					row.Cells.Add(etrans835.PatientName);
					row.Cells.Add(etrans835.PayerName);
					if(showStatusAndClinics) {
						string status=Lan.g(this,etrans835.Status.GetDescription());
						row.Cells.Add(status);
					}
					row.Cells.Add(POut.Date(listEtransFiltered[i].DateTimeTrans));
					row.Cells.Add(POut.Double(etrans835.InsPaid));
#region Column: Clinic
					if(showStatusAndClinics && PrefC.HasClinicsEnabled) {
						List<long> listClinicNums=listAttached.FindAll(x => x.EtransNum==listEtransFiltered[i].EtransNum).Select(x => x.ClinicNum).Distinct().ToList();
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
					row.Cells.Add(listEtransFiltered[i].Note);
					row.Tag=listEtransFiltered[i];
					gridMain.ListGridRows.Add(row);
				}
#endregion Fill Rows
				gridMain.Invoke(() => gridMain.EndUpdate());
			};
			progressOD.ShowDialogProgress();
			Cursor=Cursors.Default;
		}

		///<summary>Returns a list of Etrans that are for carriers that allow ERA automation. Removes the status from the list of statuses for 
		///any Etrans that is not added to the list that is returned.</summary>
		private void FilterAutomatableERAs(List<Etrans> listEtrans,List<Etrans835> listFilteredEtrans835s,List<Etrans835Attach> listAttached) {
			List<Carrier> listCarriersForAllEtrans=Carriers.GetCarriers(listAttached.Select(x => x.CarrierNum).Distinct().ToList());
			for(int i=listEtrans.Count-1;i>=0;i--) {
				List<long> listCarrierNumsForEtrans=listAttached.FindAll(x => x.EtransNum==listEtrans[i].EtransNum).Select(x => x.CarrierNum).ToList();
				List<Carrier> listCarriersForEtrans=listCarriersForAllEtrans.FindAll(x => listCarrierNumsForEtrans.Contains(x.CarrierNum));
				Etrans835 etrans835=listFilteredEtrans835s.FirstOrDefault(x => x.EtransNum==listEtrans[i].EtransNum);
				bool isEtransAutomatable=Etranss.IsEtransAutomatable(listCarriersForEtrans,etrans835.PayerName,isFullyAutomatic:false);
				if(isEtransAutomatable) {
					continue;//Keep the etrans and its status in the lists.
				}
				//Otherwise, remove the etrans and its etrans835 from the lists if it cannot be automated.
				listEtrans.RemoveAt(i);
				listFilteredEtrans835s.RemoveAt(i);
			}
		}

		///<summary>Called when we need to filter the current in memory contents in _listEtrans. Calls FillGrid()</summary>
		private void FilterAndFillGrid() {
			List<long> listClinicNums=null;//A null signifies that clinics are disabled.
			if(PrefC.HasClinicsEnabled) {
				listClinicNums=comboClinics.ListSelectedClinicNums;
			}
			FillGrid(
				listSelectedClinicNums:	listClinicNums,
				carrierName:						textCarrier.Text,
				checkTraceNum:					textCheckTrace.Text,
				amountMin:							textRangeMin.Text,
				amountMax:							textRangeMax.Text,
				controlId:							textControlId.Text
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

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
			Close();
		}

	}

}
