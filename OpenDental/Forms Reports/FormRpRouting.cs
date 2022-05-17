using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	/// <summary></summary>
	public partial class FormRpRouting:FormODBase {
		//<summary>This list of appointments gets filled.  Each appointment will result in one page when printing.</summary>
		//private List<Appointment> Appts;
		//private int pagesPrinted;
		//private PrintDocument pd;
		//private OpenDental.UI.PrintPreview printPreview;
		///<summary>The date that the user selected.</summary>
		private DateTime date;
		///<summary>If set externally beforehand, then the user will not see any choices, and only a routing slip for the one appt will print.</summary>
		public long AptNum;
		/// <summary>If ApptNum is set, then this should be set also.</summary>
		public long SheetDefNum;
		///<summary>If the butSelectedDay_Click occurs</summary>
		public bool IsAutoRunForDateSelected; 
		///<summary>If the butSelectedView_Click occurs</summary>
		public bool IsAutoRunForListAptNums;
		public List<long> ListAptNums;
		///<summary>Stores the date for the currently selected date from the appointment module.</summary>
		public DateTime DateSelected;
		private List<Clinic> _listClinics;
		private List<Provider> _listProviders;

		///<summary></summary>
		public FormRpRouting()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRpRouting_Load(object sender, System.EventArgs e) {
			_listProviders=Providers.GetListReports();
			if(IsAutoRunForDateSelected) {
				//Creates routing slips for the defined DateSelected, currently selected clinic (if Clinics is enabled), not filtered by providers.
				List<long> emptyProvNumList=new List<long>();
				List<long> listClinicNums=new List<long>();
				if(PrefC.HasClinicsEnabled) {
					listClinicNums.Add(Clinics.ClinicNum);
				}
				//Run for all providers and the currently selected day
				List<long> aptNums=RpRouting.GetRouting(DateSelected,emptyProvNumList,listClinicNums);
				PrintRoutingSlipsForAppts(aptNums);
				DialogResult=DialogResult.OK;
				return;
			}
			if(IsAutoRunForListAptNums) {
				//Creates routing slips for the entire view in ContrAppt
				PrintRoutingSlipsForAppts(ListAptNums);
				DialogResult=DialogResult.OK;
				return;
			}
			if(AptNum!=0){
				List<long> aptNums=new List<long>();
				aptNums.Add(AptNum);
				PrintRoutingSlips(aptNums,SheetDefNum);
				DialogResult=DialogResult.OK;
				return;
			}
			listProv.Items.AddList(_listProviders,x => x.GetLongDesc());
			checkProvAll.Checked=true;
			textDate.Text=DateTime.Today.ToShortDateString();
			if(!PrefC.HasClinicsEnabled) {
				listClin.Visible=false;
				listClin.Visible=false;
				checkClinAll.Visible=false;
				labelClin.Visible=false;
			}
			else {
				_listClinics=Clinics.GetForUserod(Security.CurUser,true,Lan.g(this,"Unassigned"));
				for(int i=0;i<_listClinics.Count;i++) {
					listClin.Items.Add(_listClinics[i].Abbr);
					listClin.SetSelected(listClin.Items.Count-1,(Clinics.ClinicNum!=0 && Clinics.ClinicNum==_listClinics[i].ClinicNum));
				}
				if(Clinics.ClinicNum==0) {
					checkClinAll.Checked=true;
				}
			}
		}

		private void checkProvAll_Click(object sender,EventArgs e) {
			if(checkProvAll.Checked) {
				listProv.ClearSelected();
			}
		}

		private void checkAllClin_Click(object sender,EventArgs e) {
			if(checkClinAll.Checked) {
				listClin.ClearSelected();
			}
		}

		private void listBoxClin_Click(object sender,EventArgs e) {
			if(listClin.SelectedIndices.Count>0) {
				checkClinAll.Checked=false;
			}
		}

		private void listProv_Click(object sender,EventArgs e) {
			if(listProv.SelectedIndices.Count>0) {
				checkProvAll.Checked=false;
			}
		}

		private void butToday_Click(object sender, System.EventArgs e) {
			textDate.Text=DateTime.Today.ToShortDateString();
		}

		private void butDisplayed_Click(object sender, System.EventArgs e) {
			textDate.Text=DateSelected.ToShortDateString();
		}

		/// <summary>Specify a sheetDefNum of 0 for the internal Routing slip.</summary>
		private void PrintRoutingSlips(List<long> aptNums,long sheetDefNum) {
			SheetDef sheetDef;
			if(sheetDefNum==0){
				sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.RoutingSlip);
			}
			else{
				sheetDef=SheetDefs.GetSheetDef(sheetDefNum);//includes fields and parameters
			}
			List<Sheet> sheetBatch=SheetUtil.TryCreateBatch(sheetDef,aptNums);
			if(sheetBatch.Count==0) {
				MsgBox.Show(this,"There are no routing slips to print.");
				return;
			}
			try {
				SheetPrinting.PrintBatch(sheetBatch);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void PrintRoutingSlipsForAppts(List<long> listAptNums) {
			SheetDef sheetDef;
			List<SheetDef> customSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.RoutingSlip);
			if(customSheetDefs.Count==0){
				sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.RoutingSlip);
			}
			else{
				sheetDef=customSheetDefs[0];//Instead of doing this, we could give the user a list to pick from on this form.
				//SheetDefs.GetFieldsAndParameters(sheetDef);
			}
			PrintRoutingSlips(listAptNums,sheetDef.SheetDefNum);
		}

		private void butOK_Click(object sender, System.EventArgs e){
			//validate user input
			if(!textDate.IsValid())	{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textDate.Text.Length==0){
				MessageBox.Show(Lan.g(this,"Date is required."));
				return;
			}
			date=PIn.Date(textDate.Text);
			if(listProv.SelectedIndices.Count==0 && !checkProvAll.Checked){
				MsgBox.Show(this,"You must select at least one provider.");
				return;
			}
			if(PrefC.HasClinicsEnabled && listClin.SelectedIndices.Count==0 && !checkClinAll.Checked){
				MsgBox.Show(this,"You must select at least one clinic.");
				return;
			}
			List<long> listProvNums=new List<long>();
			if(!checkProvAll.Checked) {
				listProvNums=listProv.SelectedIndices.Select(x => _listProviders[x].ProvNum).ToList();
			}
			List<long> listClinicNums=new List<long>();
			if(PrefC.HasClinicsEnabled) {
				if(checkClinAll.Checked) {
					listClinicNums=_listClinics.Select(x => x.ClinicNum).Distinct().ToList();
					listClinicNums=listClinicNums.Union(Clinics.GetAllForUserod(Security.CurUser).Select(x => x.ClinicNum)).ToList();
				}
				else {
					listClinicNums=listClin.SelectedIndices.Select(x => _listClinics[x].ClinicNum).ToList();
				}
			}
			List<long> aptNums=RpRouting.GetRouting(date,listProvNums,listClinicNums);
			PrintRoutingSlipsForAppts(aptNums);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, EventArgs e){
			DialogResult=DialogResult.Cancel;
		}

	}
}
