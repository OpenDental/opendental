using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using OpenDental.ReportingComplex;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public class FormRpReferralAnalysis:FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.ComponentModel.Container components = null;
		private OpenDental.UI.ListBoxOD listProv;
		private Label label1;
		private GroupBox groupBox2;
		private OpenDental.UI.Button butRight;
		private OpenDental.UI.Button butThis;
		private Label label2;
		private ValidDate textDateFrom;
		private ValidDate textDateTo;
		private Label label3;
		private OpenDental.UI.Button butLeft;
		private TextBox textToday;
		private Label label4;
		private CheckBox checkAddress;
		private Label label5;
		private CheckBox checkNewPat;
		private CheckBox checkLandscape;
		private List<Provider> _listProviders;

		///<summary></summary>
		public FormRpReferralAnalysis() {
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

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpReferralAnalysis));
			this.listProv = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
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
			this.checkAddress = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.checkNewPat = new System.Windows.Forms.CheckBox();
			this.checkLandscape = new System.Windows.Forms.CheckBox();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// listProv
			// 
			this.listProv.Location = new System.Drawing.Point(42,95);
			this.listProv.Name = "listProv";
			this.listProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProv.Size = new System.Drawing.Size(165,186);
			this.listProv.TabIndex = 36;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(41,76);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104,16);
			this.label1.TabIndex = 35;
			this.label1.Text = "Providers";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
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
			this.groupBox2.Location = new System.Drawing.Point(271,89);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(281,144);
			this.groupBox2.TabIndex = 46;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Date Range";
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(205,33);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(45,24);
			this.butRight.TabIndex = 46;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butThis
			// 
			this.butThis.Location = new System.Drawing.Point(95,33);
			this.butThis.Name = "butThis";
			this.butThis.Size = new System.Drawing.Size(101,24);
			this.butThis.TabIndex = 45;
			this.butThis.Text = "This Month";
			this.butThis.Click += new System.EventHandler(this.butThis_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9,79);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82,18);
			this.label2.TabIndex = 37;
			this.label2.Text = "From";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDateFrom
			// 
			this.textDateFrom.Location = new System.Drawing.Point(95,77);
			this.textDateFrom.Name = "textDateFrom";
			this.textDateFrom.Size = new System.Drawing.Size(100,20);
			this.textDateFrom.TabIndex = 43;
			// 
			// textDateTo
			// 
			this.textDateTo.Location = new System.Drawing.Point(95,104);
			this.textDateTo.Name = "textDateTo";
			this.textDateTo.Size = new System.Drawing.Size(100,20);
			this.textDateTo.TabIndex = 44;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7,105);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82,18);
			this.label3.TabIndex = 39;
			this.label3.Text = "To";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butLeft
			// 
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(41,33);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(45,24);
			this.butLeft.TabIndex = 44;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// textToday
			// 
			this.textToday.Location = new System.Drawing.Point(366,63);
			this.textToday.Name = "textToday";
			this.textToday.ReadOnly = true;
			this.textToday.Size = new System.Drawing.Size(100,20);
			this.textToday.TabIndex = 45;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(237,66);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(127,20);
			this.label4.TabIndex = 44;
			this.label4.Text = "Today\'s Date";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkAddress
			// 
			this.checkAddress.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkAddress.Location = new System.Drawing.Point(42,307);
			this.checkAddress.Name = "checkAddress";
			this.checkAddress.Size = new System.Drawing.Size(424,18);
			this.checkAddress.TabIndex = 47;
			this.checkAddress.Text = "Include address information.  Useful if you are exporting for letter merge.";
			this.checkAddress.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkAddress.UseVisualStyleBackColor = true;
			this.checkAddress.CheckedChanged += new System.EventHandler(this.checkAddress_CheckedChanged);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(41,9);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(585,36);
			this.label5.TabIndex = 48;
			this.label5.Text = "This report is based on procedures completed rather than appointments.  Also, the" +
    " production numbers will only include production for patients with referrals, no" +
    "t necessarily everyone.";
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(569,371);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,24);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(569,339);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// checkNewPat
			// 
			this.checkNewPat.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkNewPat.Location = new System.Drawing.Point(42,344);
			this.checkNewPat.Name = "checkNewPat";
			this.checkNewPat.Size = new System.Drawing.Size(350,20);
			this.checkNewPat.TabIndex = 49;
			this.checkNewPat.Text = "Only include new patients.";
			this.checkNewPat.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkNewPat.UseVisualStyleBackColor = true;
			// 
			// checkLandscape
			// 
			this.checkLandscape.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkLandscape.Location = new System.Drawing.Point(42,325);
			this.checkLandscape.Name = "checkLandscape";
			this.checkLandscape.Size = new System.Drawing.Size(350,20);
			this.checkLandscape.TabIndex = 51;
			this.checkLandscape.Text = "Run as landscape";
			this.checkLandscape.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkLandscape.UseVisualStyleBackColor = true;
			// 
			// FormRpReferralAnalysis
			// 
			this.ClientSize = new System.Drawing.Size(678,423);
			this.Controls.Add(this.checkLandscape);
			this.Controls.Add(this.checkNewPat);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.checkAddress);
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
			this.Name = "FormRpReferralAnalysis";
			this.ShowInTaskbar = false;
			this.Text = "Referral Analysis";
			this.Load += new System.EventHandler(this.FormReferralAnalysis_Load);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormReferralAnalysis_Load(object sender, System.EventArgs e) {
			checkLandscape.Visible=false;
			_listProviders=Providers.GetListReports();
			textToday.Text=DateTime.Today.ToShortDateString();
			//always defaults to the current month
			textDateFrom.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).ToShortDateString();
			textDateTo.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month
				,DateTime.DaysInMonth(DateTime.Today.Year,DateTime.Today.Month)).ToShortDateString();
			listProv.Items.Add(Lan.g(this,"All"));
			for(int i=0;i<_listProviders.Count;i++){
				listProv.Items.Add(_listProviders[i].GetLongDesc());
			}
			listProv.SetSelected(0);
		}

		private void butThis_Click(object sender,EventArgs e) {
			textDateFrom.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).ToShortDateString();
			textDateTo.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month
				,DateTime.DaysInMonth(DateTime.Today.Year,DateTime.Today.Month)).ToShortDateString();
		}

		private void checkAddress_CheckedChanged(object sender,EventArgs e) {
			if(checkAddress.Checked) {
				checkLandscape.Visible=true;
			}
			else {
				checkLandscape.Visible=false;
			}
		}

		private void butLeft_Click(object sender,EventArgs e) {
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			DateTime dateFrom=PIn.Date(textDateFrom.Text);
			DateTime dateTo=PIn.Date(textDateTo.Text);
			bool toLastDay=false;
			if(CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dateTo.Year,dateTo.Month)==dateTo.Day){
				toLastDay=true;
			}
			textDateFrom.Text=dateFrom.AddMonths(-1).ToShortDateString();
			textDateTo.Text=dateTo.AddMonths(-1).ToShortDateString();
			dateTo=PIn.Date(textDateTo.Text);
			if(toLastDay){
				textDateTo.Text=new DateTime(dateTo.Year,dateTo.Month,
					CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dateTo.Year,dateTo.Month))
					.ToShortDateString();
			}
		}

		private void butRight_Click(object sender,EventArgs e) {
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			DateTime dateFrom=PIn.Date(textDateFrom.Text);
			DateTime dateTo=PIn.Date(textDateTo.Text);
			bool toLastDay=false;
			if(CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dateTo.Year,dateTo.Month)==dateTo.Day){
				toLastDay=true;
			}
			textDateFrom.Text=dateFrom.AddMonths(1).ToShortDateString();
			textDateTo.Text=dateTo.AddMonths(1).ToShortDateString();
			dateTo=PIn.Date(textDateTo.Text);
			if(toLastDay){
				textDateTo.Text=new DateTime(dateTo.Year,dateTo.Month,
					CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dateTo.Year,dateTo.Month))
					.ToShortDateString();
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			if(listProv.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			if(listProv.SelectedIndices[0]==0 && listProv.SelectedIndices.Count>1) {
				MsgBox.Show(this,"You cannot select 'All' providers as well as specific providers.");
				return;
			}
			DateTime dateFrom=PIn.Date(textDateFrom.Text);
			DateTime dateTo=PIn.Date(textDateTo.Text);
			if(dateTo<dateFrom) {
				MsgBox.Show(this,"To date cannot be before From date.");
				return;
			}
			List<long> listProvNums=new List<long>();
			List<string> listProvNames=new List<string>();
			if(listProv.SelectedIndices[0]==0) {
				listProv.ClearSelected();
				for(int i=1;i<listProv.Items.Count;i++) {//Start i at 1 due to the 'All' option.
					listProv.SetSelected(i);
				}
			}
			for(int i=0;i<listProv.SelectedIndices.Count;i++) {
				//Minus 1 due to the 'All' option.
				listProvNums.Add(_listProviders[listProv.SelectedIndices[i]-1].ProvNum);
				listProvNames.Add(_listProviders[listProv.SelectedIndices[i]-1].Abbr);
			}
			ReportComplex report;
			if(checkLandscape.Checked) {
				report=new ReportComplex(true,true);
			}
			else {
				report=new ReportComplex(true,false);
			}
			DataTable table=RpReferralAnalysis.GetReferralTable(dateFrom,dateTo,listProvNums,checkAddress.Checked,checkNewPat.Checked);
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Referral Analysis");
			report.AddTitle("Title",Lan.g(this,"Referral Analysis"),fontTitle);
			report.AddSubTitle("PracticeTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Date SubTitle",dateFrom.ToString("d")+" - "+dateTo.ToString("d"),fontSubTitle);
			if(listProv.SelectedIndices[0]==0){
				report.AddSubTitle("Provider Subtitle",Lan.g(this,"All Providers"));
			}
			else if(listProv.SelectedIndices.Count==1) {
				report.AddSubTitle("Provider SubTitle",Lan.g(this,"Prov:")+" "+_listProviders[listProv.SelectedIndices[0]-1].GetLongDesc());
			}
			else {
				report.AddSubTitle("Provider SubTitle",string.Join(", ",listProvNames));
			}
			QueryObject query=report.AddQuery(table,Lan.g(this,"Date")+": "+DateTime.Today.ToString("d"));
			query.AddColumn("Last Name",100,FieldValueType.String);
			query.AddColumn("First Name",100,FieldValueType.String);
			query.AddColumn("Count",40,FieldValueType.Integer);
			query.AddColumn("Production",75,FieldValueType.Number);
			query.GetColumnDetail("Production").ContentAlignment=ContentAlignment.MiddleRight;
			if(checkAddress.Checked){
				query.AddColumn("Title",40);
				if(checkLandscape.Checked) {
					query.AddColumn("Address",160,FieldValueType.String);
					query.AddColumn("Add2",140,FieldValueType.String);
					query.AddColumn("City",90,FieldValueType.String);
				}
				else {
					query.AddColumn("Address",140,FieldValueType.String);
					query.AddColumn("Add2",80,FieldValueType.String);
					query.AddColumn("City",70,FieldValueType.String);
				}
				query.AddColumn("State",40,FieldValueType.String);
				query.AddColumn("Zip",70,FieldValueType.String);
				if(checkLandscape.Checked) {
					query.AddColumn("Specialty",90,FieldValueType.String);
				}
				else {
					query.AddColumn("Specialty",60,FieldValueType.String);
				}
			}
			report.AddPageNum(font);
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
