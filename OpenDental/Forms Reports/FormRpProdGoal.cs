using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.ReportingComplex;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public class FormRpProdGoal : FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.ListBoxOD listProv;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textToday;
		private OpenDental.ValidDate textDateFrom;
		private OpenDental.ValidDate textDateTo;
		private System.Windows.Forms.GroupBox groupBox2;
		private OpenDental.UI.Button butThis;
		private OpenDental.UI.Button butLeft;
		private OpenDental.UI.Button butRight;
		private DateTime _dateFrom;
		private OpenDental.UI.ListBoxOD listClin;
		private Label labelClin;
		private DateTime _dateTo;
		private CheckBox checkAllProv;
		private CheckBox checkAllClin;
		private CheckBox checkClinicBreakdown;
		private List<Clinic> _listClinics;
		///<summary>Includes hidden and hidden on reports providers.
		///This is used instead of Providers.GetListReports() because we need the full list of providers when running All Providers
		///This is also so we can show provider specific information in the report.
		///Includes providers that share the same name as the provider currently logged if user has the ReportProdIncAllProviders permission.</summary>
		private List<Provider> _listProviders;
		private GroupBox groupShowInsWriteoffs;
		private RadioButton radioWriteoffClaim;
		private Label label5;
		private RadioButton radioWriteoffProc;
		private RadioButton radioWriteoffPay;

		///<summary>Includes hidden providers, excludes hidden on reports providers.</summary>
		///This list directly resembles all providers that are showing within the providers list box that is showing to the user.</summary>
		private List<Provider> _listFilteredProviders;

		///<summary></summary>
		public FormRpProdGoal(){
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
		}

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
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
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpProdGoal));
			this.checkClinicBreakdown = new System.Windows.Forms.CheckBox();
			this.checkAllClin = new System.Windows.Forms.CheckBox();
			this.checkAllProv = new System.Windows.Forms.CheckBox();
			this.listClin = new OpenDental.UI.ListBoxOD();
			this.labelClin = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.butRight = new OpenDental.UI.Button();
			this.butThis = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textDateFrom = new OpenDental.ValidDate();
			this.textDateTo = new OpenDental.ValidDate();
			this.label3 = new System.Windows.Forms.Label();
			this.butLeft = new OpenDental.UI.Button();
			this.textToday = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.listProv = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.groupShowInsWriteoffs = new System.Windows.Forms.GroupBox();
			this.radioWriteoffClaim = new System.Windows.Forms.RadioButton();
			this.label5 = new System.Windows.Forms.Label();
			this.radioWriteoffProc = new System.Windows.Forms.RadioButton();
			this.radioWriteoffPay = new System.Windows.Forms.RadioButton();
			this.groupBox2.SuspendLayout();
			this.groupShowInsWriteoffs.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkClinicBreakdown
			// 
			this.checkClinicBreakdown.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClinicBreakdown.Location = new System.Drawing.Point(215, 250);
			this.checkClinicBreakdown.Name = "checkClinicBreakdown";
			this.checkClinicBreakdown.Size = new System.Drawing.Size(154, 16);
			this.checkClinicBreakdown.TabIndex = 9;
			this.checkClinicBreakdown.Text = "Show Clinic Breakdown";
			// 
			// checkAllClin
			// 
			this.checkAllClin.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllClin.Location = new System.Drawing.Point(215, 44);
			this.checkAllClin.Name = "checkAllClin";
			this.checkAllClin.Size = new System.Drawing.Size(154, 16);
			this.checkAllClin.TabIndex = 5;
			this.checkAllClin.Text = "All (Includes hidden)";
			this.checkAllClin.Click += new System.EventHandler(this.checkAllClin_Click);
			// 
			// checkAllProv
			// 
			this.checkAllProv.Checked = true;
			this.checkAllProv.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllProv.Location = new System.Drawing.Point(37, 44);
			this.checkAllProv.Name = "checkAllProv";
			this.checkAllProv.Size = new System.Drawing.Size(154, 16);
			this.checkAllProv.TabIndex = 3;
			this.checkAllProv.Text = "All";
			this.checkAllProv.Click += new System.EventHandler(this.checkAllProv_Click);
			// 
			// listClin
			// 
			this.listClin.Location = new System.Drawing.Point(215, 63);
			this.listClin.Name = "listClin";
			this.listClin.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClin.Size = new System.Drawing.Size(154, 186);
			this.listClin.TabIndex = 6;
			this.listClin.Click += new System.EventHandler(this.listClin_Click);
			// 
			// labelClin
			// 
			this.labelClin.Location = new System.Drawing.Point(212, 26);
			this.labelClin.Name = "labelClin";
			this.labelClin.Size = new System.Drawing.Size(154, 16);
			this.labelClin.TabIndex = 44;
			this.labelClin.Text = "Clinics";
			this.labelClin.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.butRight);
			this.groupBox2.Controls.Add(this.butThis);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.textDateFrom);
			this.groupBox2.Controls.Add(this.textDateTo);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.butLeft);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(390, 56);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(281, 144);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Date Range";
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(205, 30);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(45, 26);
			this.butRight.TabIndex = 2;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butThis
			// 
			this.butThis.Location = new System.Drawing.Point(95, 30);
			this.butThis.Name = "butThis";
			this.butThis.Size = new System.Drawing.Size(101, 26);
			this.butThis.TabIndex = 1;
			this.butThis.Text = "This Month";
			this.butThis.Click += new System.EventHandler(this.butThis_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 79);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 18);
			this.label2.TabIndex = 37;
			this.label2.Text = "From";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDateFrom
			// 
			this.textDateFrom.Location = new System.Drawing.Point(95, 77);
			this.textDateFrom.Name = "textDateFrom";
			this.textDateFrom.Size = new System.Drawing.Size(100, 20);
			this.textDateFrom.TabIndex = 3;
			// 
			// textDateTo
			// 
			this.textDateTo.Location = new System.Drawing.Point(95, 104);
			this.textDateTo.Name = "textDateTo";
			this.textDateTo.Size = new System.Drawing.Size(100, 20);
			this.textDateTo.TabIndex = 4;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7, 105);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82, 18);
			this.label3.TabIndex = 39;
			this.label3.Text = "To";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butLeft
			// 
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(41, 30);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(45, 26);
			this.butLeft.TabIndex = 0;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// textToday
			// 
			this.textToday.Location = new System.Drawing.Point(485, 24);
			this.textToday.Name = "textToday";
			this.textToday.ReadOnly = true;
			this.textToday.Size = new System.Drawing.Size(100, 20);
			this.textToday.TabIndex = 1;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(356, 24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(127, 20);
			this.label4.TabIndex = 41;
			this.label4.Text = "Today\'s Date";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listProv
			// 
			this.listProv.Location = new System.Drawing.Point(37, 63);
			this.listProv.Name = "listProv";
			this.listProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProv.Size = new System.Drawing.Size(154, 186);
			this.listProv.TabIndex = 4;
			this.listProv.Click += new System.EventHandler(this.listProv_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(34, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(154, 16);
			this.label1.TabIndex = 29;
			this.label1.Text = "Providers";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(693, 307);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 11;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(693, 272);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 10;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// groupShowInsWriteoffs
			// 
			this.groupShowInsWriteoffs.Controls.Add(this.radioWriteoffClaim);
			this.groupShowInsWriteoffs.Controls.Add(this.label5);
			this.groupShowInsWriteoffs.Controls.Add(this.radioWriteoffProc);
			this.groupShowInsWriteoffs.Controls.Add(this.radioWriteoffPay);
			this.groupShowInsWriteoffs.Location = new System.Drawing.Point(390, 206);
			this.groupShowInsWriteoffs.Name = "groupShowInsWriteoffs";
			this.groupShowInsWriteoffs.Size = new System.Drawing.Size(281, 142);
			this.groupShowInsWriteoffs.TabIndex = 45;
			this.groupShowInsWriteoffs.TabStop = false;
			this.groupShowInsWriteoffs.Text = "Show Insurance Write-offs";
			// 
			// radioWriteoffClaim
			// 
			this.radioWriteoffClaim.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffClaim.Location = new System.Drawing.Point(9, 71);
			this.radioWriteoffClaim.Name = "radioWriteoffClaim";
			this.radioWriteoffClaim.Size = new System.Drawing.Size(256, 35);
			this.radioWriteoffClaim.TabIndex = 4;
			this.radioWriteoffClaim.Text = "Use initial claim date for write-off estimates, ins pay date for write-off adjust" +
    "ments";
			this.radioWriteoffClaim.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffClaim.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 109);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(259, 24);
			this.label5.TabIndex = 2;
			this.label5.Text = "(This is discussed in the PPO section of the manual)";
			// 
			// radioWriteoffProc
			// 
			this.radioWriteoffProc.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffProc.Location = new System.Drawing.Point(9, 45);
			this.radioWriteoffProc.Name = "radioWriteoffProc";
			this.radioWriteoffProc.Size = new System.Drawing.Size(256, 23);
			this.radioWriteoffProc.TabIndex = 1;
			this.radioWriteoffProc.Text = "Using procedure date";
			this.radioWriteoffProc.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffProc.UseVisualStyleBackColor = true;
			// 
			// radioWriteoffPay
			// 
			this.radioWriteoffPay.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffPay.Checked = true;
			this.radioWriteoffPay.Location = new System.Drawing.Point(9, 21);
			this.radioWriteoffPay.Name = "radioWriteoffPay";
			this.radioWriteoffPay.Size = new System.Drawing.Size(256, 23);
			this.radioWriteoffPay.TabIndex = 0;
			this.radioWriteoffPay.TabStop = true;
			this.radioWriteoffPay.Text = "Using insurance payment date";
			this.radioWriteoffPay.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffPay.UseVisualStyleBackColor = true;
			// 
			// FormRpProdGoal
			// 
			this.ClientSize = new System.Drawing.Size(801, 359);
			this.Controls.Add(this.groupShowInsWriteoffs);
			this.Controls.Add(this.checkClinicBreakdown);
			this.Controls.Add(this.checkAllClin);
			this.Controls.Add(this.checkAllProv);
			this.Controls.Add(this.listClin);
			this.Controls.Add(this.labelClin);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.textToday);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.listProv);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpProdGoal";
			this.ShowInTaskbar = false;
			this.Text = "Monthly Production Goal";
			this.Load += new System.EventHandler(this.FormRpProdGoal_Load);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupShowInsWriteoffs.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private void FormRpProdGoal_Load(object sender, System.EventArgs e) {
			_listProviders=Providers.GetListReports();
			_listFilteredProviders=new List<Provider>();
			textToday.Text=DateTime.Today.ToShortDateString();
			textDateFrom.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).ToShortDateString();
			textDateTo.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month
				,DateTime.DaysInMonth(DateTime.Today.Year,DateTime.Today.Month)).ToShortDateString();
			if(!Security.IsAuthorized(Permissions.ReportProdIncAllProviders,true)) {
				//They either have permission or have a provider at this point.  If they don't have permission they must have a provider.
				_listProviders=_listProviders.FindAll(x => x.ProvNum==Security.CurUser.ProvNum);
				Provider prov=_listProviders.FirstOrDefault();
				if(prov!=null) {
					_listProviders.AddRange(Providers.GetWhere(x => x.FName == prov.FName && x.LName == prov.LName && x.ProvNum != prov.ProvNum));
				}
				checkAllProv.Checked=false;
				checkAllProv.Enabled=false;
			}
			//Fill the short list of providers, ignoring those marked "hidden on reports"
			for(int i=0;i<_listProviders.Count;i++) {
				if(_listProviders[i].IsHiddenReport) {
					continue;
				}
				listProv.Items.Add(_listProviders[i].GetLongDesc());
				_listFilteredProviders.Add(_listProviders[i].Copy());
			}
			//If the user is not allowed to run the report for all providers, default the selection to the first in the list box.
			if(checkAllProv.Enabled==false && listProv.Items.Count>0) {
				listProv.SetSelected(0);
			}
			//If the user cannot run this report for any other provider, every single provider available in the list will be the provider logged in.
			if(!Security.IsAuthorized(Permissions.ReportProdIncAllProviders,true)) {
				listProv.SetAll(true);
			}
			if(!PrefC.HasClinicsEnabled) {
				listClin.Visible=false;
				labelClin.Visible=false;
				checkAllClin.Visible=false;
				checkClinicBreakdown.Visible=false;
			}
			else {
				checkClinicBreakdown.Checked=PrefC.GetBool(PrefName.ReportPandIhasClinicBreakdown);
				_listClinics=Clinics.GetForUserod(Security.CurUser);
				if(!Security.CurUser.ClinicIsRestricted) {
					listClin.Items.Add(Lan.g(this,"Unassigned"));
					listClin.SetSelected(0);
				}
				for(int i=0;i<_listClinics.Count;i++) {
					listClin.Items.Add(_listClinics[i].Abbr);
					if(Clinics.ClinicNum==0) {
						listClin.SetSelected(listClin.Items.Count-1);
						checkAllClin.Checked=true;
					}
					if(_listClinics[i].ClinicNum==Clinics.ClinicNum) {
						listClin.SelectedIndices.Clear();
						listClin.SetSelected(listClin.Items.Count-1);
					}
				}
			}
			switch(PrefC.GetInt(PrefName.ReportsPPOwriteoffDefaultToProcDate)) {
				case 0:	radioWriteoffPay.Checked=true; break;
				case 1:	radioWriteoffProc.Checked=true; break;
				case 2:	radioWriteoffClaim.Checked=true; break;
				default:
					radioWriteoffClaim.Checked=true; break;
			}
			Text+=PrefC.ReportingServer.DisplayStr=="" ? "" : " - "+Lan.g(this,"Reporting Server:") +" "+ PrefC.ReportingServer.DisplayStr;
		}

		private PPOWriteoffDateCalc GetWriteoffType() {
			if(radioWriteoffPay.Checked) {
				return PPOWriteoffDateCalc.InsPayDate;
			}
			else if(radioWriteoffClaim.Checked) {
				return PPOWriteoffDateCalc.ClaimPayDate;
			}
			else {
				return PPOWriteoffDateCalc.ProcDate;
			}
		} 

		private void checkAllProv_Click(object sender,EventArgs e) {
			if(checkAllProv.Checked) {
				listProv.ClearSelected();
			}
		}

		private void listProv_Click(object sender,EventArgs e) {
			if(listProv.SelectedIndices.Count>0) {
				checkAllProv.Checked=false;
			}
		}

		private void checkAllClin_Click(object sender,EventArgs e) {
			if(checkAllClin.Checked) {
				listClin.SetAll(true);
			}
			else {
				listClin.ClearSelected();
			}
		}

		private void listClin_Click(object sender,EventArgs e) {
			if(listClin.SelectedIndices.Count>0) {
				checkAllClin.Checked=false;
			}
		}

		private void butThis_Click(object sender, System.EventArgs e) {
			textDateFrom.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).ToShortDateString();
			textDateTo.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month
				,DateTime.DaysInMonth(DateTime.Today.Year,DateTime.Today.Month)).ToShortDateString();
		}

		private void butLeft_Click(object sender, System.EventArgs e) {
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			_dateFrom=PIn.Date(textDateFrom.Text);
			textDateFrom.Text=_dateFrom.AddMonths(-1).ToShortDateString();
			_dateTo=PIn.Date(textDateTo.Text);
			bool toLastDay=false;
			if(CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(_dateTo.Year,_dateTo.Month)==_dateTo.Day){
				toLastDay=true;
			}
			_dateTo=_dateTo.AddMonths(-1);
			if(toLastDay){
				_dateTo=new DateTime(_dateTo.Year,_dateTo.Month,
					CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(_dateTo.Year,_dateTo.Month));
			}
			textDateTo.Text=_dateTo.ToShortDateString();
		}

		private void butRight_Click(object sender, System.EventArgs e) {
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			_dateFrom=PIn.Date(textDateFrom.Text);
			textDateFrom.Text=_dateFrom.AddMonths(1).ToShortDateString();
			_dateTo=PIn.Date(textDateTo.Text);
			bool toLastDay=false;
			if(CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(_dateTo.Year,_dateTo.Month)==_dateTo.Day){
				toLastDay=true;
			}
			_dateTo=_dateTo.AddMonths(1);
			if(toLastDay) {
				_dateTo=new DateTime(_dateTo.Year,_dateTo.Month,
					CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(_dateTo.Year,_dateTo.Month));
			}
			textDateTo.Text=_dateTo.ToShortDateString();
		}

		private void RunProdGoal() {
			//If adding the unearned column, need more space. Set report to landscape.
			if(checkAllProv.Checked) {
				listProv.SetAll(true);
			}
			if(checkAllClin.Checked) {
				listClin.SetAll(true);
			}
			_dateFrom=PIn.Date(textDateFrom.Text);
			_dateTo=PIn.Date(textDateTo.Text);
			List<Provider> listProvs=new List<Provider>();
			if(checkAllProv.Checked){
				listProvs=_listProviders;
			}
			else if(listProv.SelectedIndices.Count>0){
				listProvs=listProv.SelectedIndices.Select(x => _listFilteredProviders[x]).ToList();
			}
			List<Clinic> listClinics=new List<Clinic>();
			List<long> listSelectedClinicNums=new List<long>();
			if(PrefC.HasClinicsEnabled) {
				if(listClin.SelectedIndices.Count>0) {
					int offset=Security.CurUser.ClinicIsRestricted?0:1;
					listClinics.AddRange(listClin.SelectedIndices.Select(x => offset==1 && x==0?new Clinic { ClinicNum=0,Abbr=Lan.g(this,"Unassigned") }:_listClinics[x-offset]));
				}
				if(checkAllClin.Checked) {
					//Add all remaining non-restricted clinics to the list
					listClinics.AddRange(Clinics.GetAllForUserod(Security.CurUser).Where(x => !listClinics.Select(y => y.ClinicNum).Contains(x.ClinicNum)));
				}
				//Check here for multi clinic schedule overlap and give notification.
				listSelectedClinicNums=listClinics.Select(x => x.ClinicNum).ToList();
				var listConflicts=listProvs
					.Select(x => new { x.Abbr,listScheds=Schedules.GetClinicOverlapsForProv(_dateFrom,_dateTo,x.ProvNum,listSelectedClinicNums) })
					.Where(x => x.listScheds.Count>0).ToList();
				if(listConflicts.Count>0) {
					string errorMsg="This report is designed to show production goals by clinic and provider.  You have one or more providers during the "
						+"specified period that are scheduled in more than one clinic at the same time.  Due to this, production goals cannot be reported "
						+"accurately.\r\nTo run this report, please fix your scheduling so each provider is only scheduled at one clinic at a time, or select "
						+"different providers or clinics.\r\nIn the mean time, you can run regular production and income reports instead.\r\n\r\n"
						+"Conflicts:\r\n"
						+string.Join("\r\n",listConflicts
							.SelectMany(x => x.listScheds
								.Select(y => x.Abbr+" "+y.SchedDate.ToShortDateString()+" "+y.StartTime.ToShortTimeString()+" - "+y.StopTime.ToShortTimeString())));
					using MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(errorMsg);
					msgBox.ShowDialog();
					return;
				}
			}
			ReportComplex report=new ReportComplex(true,false);
			bool hasAllClinics=checkAllClin.Checked && listSelectedClinicNums.Contains(0)
				&& Clinics.GetDeepCopy().Select(x => x.ClinicNum).All(x => ListTools.In(x,listSelectedClinicNums));
			using(DataSet ds=RpProdGoal.GetData(_dateFrom,_dateTo,listProvs,listClinics,checkAllProv.Checked,hasAllClinics,GetWriteoffType()))
			using(DataTable dt=ds.Tables["Total"])
			using(DataTable dtClinic=PrefC.HasClinicsEnabled?ds.Tables["Clinic"]:new DataTable())
			using(Font font=new Font("Tahoma",8,FontStyle.Regular)) {
				report.ReportName="MonthlyP&IGoals";
				report.AddTitle("Title",Lan.g(this,"Monthly Production Goal"));
				report.AddSubTitle("PracName",PrefC.GetString(PrefName.PracticeTitle));
				report.AddSubTitle("Date",_dateFrom.ToShortDateString()+" - "+_dateTo.ToShortDateString());
				report.AddSubTitle("Providers",checkAllProv.Checked?Lan.g(this,"All Providers"):listProvs.Count==0?"":string.Join(", ",listProvs.Select(x => x.Abbr)));
				if(PrefC.HasClinicsEnabled) {
					report.AddSubTitle("Clinics",hasAllClinics?Lan.g(this,"All Clinics"):listClinics.Count==0?"":string.Join(", ",listClinics.Select(x => x.Abbr)));
				}
				//setup query
				QueryObject query;
				if(PrefC.HasClinicsEnabled && checkClinicBreakdown.Checked) {
					query=report.AddQuery(dtClinic,"","Clinic",SplitByKind.Value,1,true);
				}
				else {
					query=report.AddQuery(dt,"","",SplitByKind.None,1,true);
				}
				// add columns to report
				int dateWidth=70;
				int weekdayWidth=65;
				int prodWidth=90;
				int prodGoalWidth=90;
				int schedWidth=85;
				int adjWidth=85;
				int writeoffWidth=95;
				int writeoffestwidth=95;
				int writeoffadjwidth=70;
				int totProdWidth=90;
				int summaryOffSetY=30;
				int groups=1;
				if(PrefC.HasClinicsEnabled && listClin.SelectedIndices.Count>1 && checkClinicBreakdown.Checked) {
					groups=2;
				}
				for(int i=0;i<groups;i++) {//groups will be 1 or 2 if there are clinic breakdowns
					if(i>0) {//If more than one clinic selected, we want to add a table to the end of the report that totals all the clinics together
						query=report.AddQuery(dt,"Totals","",SplitByKind.None,2,true);
					}
					query.AddColumn("Date",dateWidth,FieldValueType.String,font);
					query.AddColumn("Weekday",weekdayWidth,FieldValueType.String,font);
					query.AddColumn("Production",prodWidth,FieldValueType.Number,font);
					query.AddColumn("Prod Goal",prodGoalWidth,FieldValueType.Number,font);
					query.AddColumn("Scheduled",schedWidth,FieldValueType.Number,font);
					query.AddColumn("Adjusts",adjWidth,FieldValueType.Number,font);
					if(GetWriteoffType()==PPOWriteoffDateCalc.ClaimPayDate) {
						query.AddColumn("Writeoff Est",writeoffestwidth,FieldValueType.Number,font);
						query.AddColumn("Writeoff Adj",writeoffadjwidth,FieldValueType.Number,font);
					}
					else {
						query.AddColumn("Writeoff",writeoffWidth,FieldValueType.Number,font);
					}
					query.AddColumn("Tot Prod",totProdWidth,FieldValueType.Number,font);
				}
				string colNameAlign="Writeoff"+(GetWriteoffType()==PPOWriteoffDateCalc.ClaimPayDate?" Est":"");//Column used to align the summary fields.
				string summaryText="Total Production (Production + Scheduled + Adjustments - Writeoff"
					+(GetWriteoffType()==PPOWriteoffDateCalc.ClaimPayDate?" Ests - Writeoff Adjs":"s")+"): ";
				query.AddGroupSummaryField(summaryText,colNameAlign,"Tot Prod",SummaryOperation.Sum,new List<int> { groups },Color.Black,
					new Font("Tahoma",9,FontStyle.Bold),75,summaryOffSetY);
				report.AddPageNum();
				// execute query
				if(!report.SubmitQueries()) {
					return;
				}
				// display report
				using(FormReportComplex FormR=new FormReportComplex(report)) {
					FormR.ShowDialog();
				}
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(!checkAllProv.Checked && listProv.SelectedIndices.Count==0){
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			if(PrefC.HasClinicsEnabled) {
				if(!checkAllClin.Checked && listClin.SelectedIndices.Count==0) {
					MsgBox.Show(this,"At least one clinic must be selected.");
					return;
				}
			}
			_dateFrom=PIn.Date(textDateFrom.Text);
			_dateTo=PIn.Date(textDateTo.Text);
			if(_dateTo<_dateFrom) {
				MsgBox.Show(this,"To date cannot be before From date.");
				return;
			}
			RunProdGoal();
			//DialogResult=DialogResult.OK;//Stay here so that a series of similar reports can be run
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}








