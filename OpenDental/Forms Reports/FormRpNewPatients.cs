using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.ReportingComplex;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public class FormRpNewPatients:FormODBase {
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
		private CheckBox checkProd;
		private List<Provider> _listProviders;

		///<summary></summary>
		public FormRpNewPatients() {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpNewPatients));
			this.listProv = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textToday = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.checkAddress = new System.Windows.Forms.CheckBox();
			this.checkProd = new System.Windows.Forms.CheckBox();
			this.butRight = new OpenDental.UI.Button();
			this.butThis = new OpenDental.UI.Button();
			this.textDateFrom = new OpenDental.ValidDate();
			this.textDateTo = new OpenDental.ValidDate();
			this.butLeft = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// listProv
			// 
			this.listProv.Location = new System.Drawing.Point(42,61);
			this.listProv.Name = "listProv";
			this.listProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProv.Size = new System.Drawing.Size(165,186);
			this.listProv.TabIndex = 36;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(41,42);
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
			this.groupBox2.Location = new System.Drawing.Point(271,55);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(281,144);
			this.groupBox2.TabIndex = 46;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Date Range";
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
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7,105);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82,18);
			this.label3.TabIndex = 39;
			this.label3.Text = "To";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textToday
			// 
			this.textToday.Location = new System.Drawing.Point(366,29);
			this.textToday.Name = "textToday";
			this.textToday.ReadOnly = true;
			this.textToday.Size = new System.Drawing.Size(100,20);
			this.textToday.TabIndex = 45;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(237,32);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(127,20);
			this.label4.TabIndex = 44;
			this.label4.Text = "Today\'s Date";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkAddress
			// 
			this.checkAddress.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkAddress.Location = new System.Drawing.Point(42,273);
			this.checkAddress.Name = "checkAddress";
			this.checkAddress.Size = new System.Drawing.Size(402,35);
			this.checkAddress.TabIndex = 47;
			this.checkAddress.Text = "Include address information.  Doesn\'t fit easily on a printout, but useful if you" +
    " are exporting for letter merge.";
			this.checkAddress.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkAddress.UseVisualStyleBackColor = true;
			// 
			// checkProd
			// 
			this.checkProd.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkProd.Location = new System.Drawing.Point(42,313);
			this.checkProd.Name = "checkProd";
			this.checkProd.Size = new System.Drawing.Size(381,22);
			this.checkProd.TabIndex = 48;
			this.checkProd.Text = "Exclude patients with no production.";
			this.checkProd.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkProd.UseVisualStyleBackColor = true;
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
			// butLeft
			// 
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(41,33);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(45,24);
			this.butLeft.TabIndex = 44;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(542,316);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,24);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(542,284);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormRpNewPatients
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(651,368);
			this.Controls.Add(this.checkProd);
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
			this.Name = "FormRpNewPatients";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "New Patients";
			this.Load += new System.EventHandler(this.FormNewPatients_Load);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormNewPatients_Load(object sender, System.EventArgs e) {
			_listProviders=Providers.GetListReports();
			textToday.Text=DateTime.Today.ToShortDateString();
			//always defaults to the current month
			textDateFrom.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).ToShortDateString();
			textDateTo.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month
				,DateTime.DaysInMonth(DateTime.Today.Year,DateTime.Today.Month)).ToShortDateString();
			listProv.Items.Add(Lan.g(this,"all"));
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
			if(!textDateFrom.IsValid()|| !textDateTo.IsValid()) {
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
			if(listProv.SelectedIndices[0]==0 && listProv.SelectedIndices.Count>1){
				MsgBox.Show(this,"You cannot select 'all' providers as well as specific providers.");
				return;
			}
			DateTime dateFrom=PIn.Date(textDateFrom.Text);
			DateTime dateTo=PIn.Date(textDateTo.Text);
			if(dateTo<dateFrom) {
				MsgBox.Show(this,"To date cannot be before From date.");
				return;
			}
			ReportComplex report;
			if(checkAddress.Checked) {
				report=new ReportComplex(true,true);
			}
			else {
				report=new ReportComplex(true,false);
			}
			List<long> listProvNums=new List<long>();
			List<Provider> listProvs=Providers.GetListReports();
			string subtitleProvs="";
			if(listProv.SelectedIndices[0]==0) {//'All' is selected
				for(int i=0;i<listProvs.Count;i++) {
					listProvNums.Add(listProvs[i].ProvNum);
					subtitleProvs=Lan.g(this,"All Providers");
				}
			}
			else {
				for(int i=0;i<listProv.SelectedIndices.Count;i++) {
					listProvNums.Add(listProvs[listProv.SelectedIndices[i]-1].ProvNum);//Minus 1 from the selected index to account for 'All' option
					if(i>0) {
						subtitleProvs+=", ";
					}
					subtitleProvs+=listProvs[listProv.SelectedIndices[i]-1].Abbr;//Minus 1 from the selected index to account for 'All' option
				}
			}
			DataTable table=RpNewPatients.GetNewPatients(dateFrom,dateTo,listProvNums,checkAddress.Checked,checkProd.Checked,listProv.SelectedIndices[0]==0);
			Font font=new Font("Tahoma",9);
			Font fontBold=new Font("Tahoma",9,FontStyle.Bold);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"New Patients");
			report.AddTitle("Title",Lan.g(this,"New Patients"),fontTitle);
			report.AddSubTitle("Practice Title",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Providers",subtitleProvs,fontSubTitle);
			report.AddSubTitle("Dates of Report",dateFrom.ToString("d")+" - "+dateTo.ToString("d"),fontSubTitle);
			QueryObject query=report.AddQuery(table,Lan.g(this,"Date")+": "+DateTime.Today.ToString("d"));
			query.AddColumn(Lan.g(this,"#"),40,FieldValueType.String,font);
			query.AddColumn(Lan.g(this,"Date"),90,FieldValueType.Date,font);
			query.AddColumn(Lan.g(this,"Last Name"),120,FieldValueType.String,font);
			query.AddColumn(Lan.g(this,"First Name"),120,FieldValueType.String,font);
			query.AddColumn(Lan.g(this,"Referral"),140,FieldValueType.String,font);
			query.AddColumn(Lan.g(this,"Production Fee"),90,FieldValueType.Number,font);
			if(checkAddress.Checked){
				query.AddColumn(Lan.g(this,"Pref'd"),90,FieldValueType.String,font);
				query.AddColumn(Lan.g(this,"Address"),100,FieldValueType.String,font);
				query.AddColumn(Lan.g(this,"Add2"),80,FieldValueType.String,font);
				query.AddColumn(Lan.g(this,"City"),100,FieldValueType.String,font); 
				query.AddColumn(Lan.g(this,"ST"),30,FieldValueType.String,font);
				query.AddColumn(Lan.g(this,"Zip"),55,FieldValueType.String,font);
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
