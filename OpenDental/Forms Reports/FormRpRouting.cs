using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	/// <summary></summary>
	public class FormRpRouting:FormODBase {
		private OpenDental.UI.ListBoxOD listProv;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private OpenDental.ValidDate textDate;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butDisplayed;
		private OpenDental.UI.Button butToday;
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
		private CheckBox checkClinAll;
		private OpenDental.UI.ListBoxOD listClin;
		private Label labelClin;
		private List<Clinic> _listClinics;
		private CheckBox checkProvAll;
		private List<Provider> _listProviders;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

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

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpRouting));
			this.listProv = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textDate = new OpenDental.ValidDate();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butToday = new OpenDental.UI.Button();
			this.butDisplayed = new OpenDental.UI.Button();
			this.checkClinAll = new System.Windows.Forms.CheckBox();
			this.listClin = new OpenDental.UI.ListBoxOD();
			this.labelClin = new System.Windows.Forms.Label();
			this.checkProvAll = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// listProv
			// 
			this.listProv.Location = new System.Drawing.Point(27, 41);
			this.listProv.Name = "listProv";
			this.listProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProv.Size = new System.Drawing.Size(120, 186);
			this.listProv.TabIndex = 33;
			this.listProv.Click += new System.EventHandler(this.listProv_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 1);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(75, 16);
			this.label1.TabIndex = 32;
			this.label1.Text = "Providers";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(150, 236);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(51, 18);
			this.label2.TabIndex = 37;
			this.label2.Text = "Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(207, 233);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(82, 20);
			this.textDate.TabIndex = 43;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(356, 244);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 44;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(356, 212);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 43;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butToday
			// 
			this.butToday.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butToday.Location = new System.Drawing.Point(335, 18);
			this.butToday.Name = "butToday";
			this.butToday.Size = new System.Drawing.Size(96, 23);
			this.butToday.TabIndex = 46;
			this.butToday.Text = "Today";
			this.butToday.Click += new System.EventHandler(this.butToday_Click);
			// 
			// butDisplayed
			// 
			this.butDisplayed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butDisplayed.Location = new System.Drawing.Point(335, 47);
			this.butDisplayed.Name = "butDisplayed";
			this.butDisplayed.Size = new System.Drawing.Size(96, 23);
			this.butDisplayed.TabIndex = 45;
			this.butDisplayed.Text = "Displayed";
			this.butDisplayed.Click += new System.EventHandler(this.butDisplayed_Click);
			// 
			// checkClinAll
			// 
			this.checkClinAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClinAll.Location = new System.Drawing.Point(156, 20);
			this.checkClinAll.Name = "checkClinAll";
			this.checkClinAll.Size = new System.Drawing.Size(118, 14);
			this.checkClinAll.TabIndex = 57;
			this.checkClinAll.Text = "All (Includes hidden)";
			this.checkClinAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClinAll.Click += new System.EventHandler(this.checkAllClin_Click);
			// 
			// listBoxClin
			// 
			this.listClin.Location = new System.Drawing.Point(156, 41);
			this.listClin.Name = "listBoxClin";
			this.listClin.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClin.Size = new System.Drawing.Size(133, 186);
			this.listClin.TabIndex = 56;
			this.listClin.Click += new System.EventHandler(this.listBoxClin_Click);
			// 
			// labelClin
			// 
			this.labelClin.Location = new System.Drawing.Point(153, 1);
			this.labelClin.Name = "labelClin";
			this.labelClin.Size = new System.Drawing.Size(87, 16);
			this.labelClin.TabIndex = 58;
			this.labelClin.Text = "Clinics";
			this.labelClin.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkProvAll
			// 
			this.checkProvAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkProvAll.Location = new System.Drawing.Point(27, 19);
			this.checkProvAll.Name = "checkProvAll";
			this.checkProvAll.Size = new System.Drawing.Size(50, 16);
			this.checkProvAll.TabIndex = 59;
			this.checkProvAll.Text = "All";
			this.checkProvAll.Click += new System.EventHandler(this.checkProvAll_Click);
			// 
			// FormRpRouting
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(471, 292);
			this.Controls.Add(this.checkProvAll);
			this.Controls.Add(this.labelClin);
			this.Controls.Add(this.checkClinAll);
			this.Controls.Add(this.listClin);
			this.Controls.Add(this.butToday);
			this.Controls.Add(this.butDisplayed);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listProv);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpRouting";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Routing Slips";
			this.Load += new System.EventHandler(this.FormRpRouting_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

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
