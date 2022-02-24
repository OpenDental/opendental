using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.ReportingComplex;
using CodeBase;
using System.Linq;

namespace OpenDental{
///<summary></summary>
	public class FormRpProcSheet : FormODBase {
		private System.ComponentModel.Container components = null;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.MonthCalendar date2;
		private System.Windows.Forms.MonthCalendar date1;
		private System.Windows.Forms.Label labelTO;
		private OpenDental.UI.ListBoxOD listProv;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton radioIndividual;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioGrouped;
		private Label label2;
		private TextBox textCode;
		private CheckBox checkAllProv;
		private CheckBox checkAllClin;
		private OpenDental.UI.ListBoxOD listClin;
		private Label labelClin;
		private List<Clinic> _listClinics;
		private List<long> _listClinicNums;
		private List<long> _listProvNums;
		private List<Provider> _listProviders;

		///<summary></summary>
		public FormRpProcSheet(){
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpProcSheet));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.date2 = new System.Windows.Forms.MonthCalendar();
			this.date1 = new System.Windows.Forms.MonthCalendar();
			this.labelTO = new System.Windows.Forms.Label();
			this.listProv = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.radioIndividual = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioGrouped = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.textCode = new System.Windows.Forms.TextBox();
			this.checkAllProv = new System.Windows.Forms.CheckBox();
			this.checkAllClin = new System.Windows.Forms.CheckBox();
			this.listClin = new OpenDental.UI.ListBoxOD();
			this.labelClin = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(634, 382);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(634, 346);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// date2
			// 
			this.date2.Location = new System.Drawing.Point(284, 33);
			this.date2.Name = "date2";
			this.date2.TabIndex = 2;
			// 
			// date1
			// 
			this.date1.Location = new System.Drawing.Point(28, 33);
			this.date1.Name = "date1";
			this.date1.TabIndex = 1;
			// 
			// labelTO
			// 
			this.labelTO.Location = new System.Drawing.Point(234, 41);
			this.labelTO.Name = "labelTO";
			this.labelTO.Size = new System.Drawing.Size(72, 23);
			this.labelTO.TabIndex = 22;
			this.labelTO.Text = "TO";
			this.labelTO.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// listProv
			// 
			this.listProv.Location = new System.Drawing.Point(534, 48);
			this.listProv.Name = "listProv";
			this.listProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProv.Size = new System.Drawing.Size(175, 147);
			this.listProv.TabIndex = 33;
			this.listProv.Click += new System.EventHandler(this.listProv_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(532, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 32;
			this.label1.Text = "Providers";
			// 
			// radioIndividual
			// 
			this.radioIndividual.Checked = true;
			this.radioIndividual.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioIndividual.Location = new System.Drawing.Point(11, 17);
			this.radioIndividual.Name = "radioIndividual";
			this.radioIndividual.Size = new System.Drawing.Size(207, 21);
			this.radioIndividual.TabIndex = 35;
			this.radioIndividual.TabStop = true;
			this.radioIndividual.Text = "Individual Procedures";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioGrouped);
			this.groupBox1.Controls.Add(this.radioIndividual);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(28, 229);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(239, 70);
			this.groupBox1.TabIndex = 36;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Type";
			// 
			// radioGrouped
			// 
			this.radioGrouped.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioGrouped.Location = new System.Drawing.Point(11, 40);
			this.radioGrouped.Name = "radioGrouped";
			this.radioGrouped.Size = new System.Drawing.Size(215, 21);
			this.radioGrouped.TabIndex = 36;
			this.radioGrouped.Text = "Grouped By Procedure Code";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(26, 324);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(290, 20);
			this.label2.TabIndex = 37;
			this.label2.Text = "Only for procedure codes similar to:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textCode
			// 
			this.textCode.Location = new System.Drawing.Point(28, 348);
			this.textCode.Name = "textCode";
			this.textCode.Size = new System.Drawing.Size(100, 20);
			this.textCode.TabIndex = 38;
			// 
			// checkAllProv
			// 
			this.checkAllProv.Checked = true;
			this.checkAllProv.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllProv.Location = new System.Drawing.Point(534, 30);
			this.checkAllProv.Name = "checkAllProv";
			this.checkAllProv.Size = new System.Drawing.Size(95, 16);
			this.checkAllProv.TabIndex = 48;
			this.checkAllProv.Text = "All";
			this.checkAllProv.Click += new System.EventHandler(this.checkAllProv_Click);
			// 
			// checkAllClin
			// 
			this.checkAllClin.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllClin.Location = new System.Drawing.Point(322, 227);
			this.checkAllClin.Name = "checkAllClin";
			this.checkAllClin.Size = new System.Drawing.Size(154, 16);
			this.checkAllClin.TabIndex = 54;
			this.checkAllClin.Text = "All (Includes hidden)";
			this.checkAllClin.Click += new System.EventHandler(this.checkAllClin_Click);
			// 
			// listClin
			// 
			this.listClin.Location = new System.Drawing.Point(322, 246);
			this.listClin.Name = "listClin";
			this.listClin.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClin.Size = new System.Drawing.Size(154, 147);
			this.listClin.TabIndex = 53;
			this.listClin.Click += new System.EventHandler(this.listClin_Click);
			// 
			// labelClin
			// 
			this.labelClin.Location = new System.Drawing.Point(319, 209);
			this.labelClin.Name = "labelClin";
			this.labelClin.Size = new System.Drawing.Size(104, 16);
			this.labelClin.TabIndex = 52;
			this.labelClin.Text = "Clinics";
			this.labelClin.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormRpProcSheet
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(743, 437);
			this.Controls.Add(this.checkAllClin);
			this.Controls.Add(this.listClin);
			this.Controls.Add(this.labelClin);
			this.Controls.Add(this.checkAllProv);
			this.Controls.Add(this.textCode);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.listProv);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.date2);
			this.Controls.Add(this.date1);
			this.Controls.Add(this.labelTO);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpProcSheet";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Daily Procedures Report";
			this.Load += new System.EventHandler(this.FormDailySummary_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormDailySummary_Load(object sender, System.EventArgs e) {
			_listProviders=Providers.GetListReports();
			date1.SelectionStart=DateTime.Today;
			date2.SelectionStart=DateTime.Today;
			if(!Security.IsAuthorized(Permissions.ReportDailyAllProviders,true)) {
				//They either have permission or have a provider at this point.  If they don't have permission they must have a provider.
				_listProviders=_listProviders.FindAll(x => x.ProvNum==Security.CurUser.ProvNum);
				checkAllProv.Checked=false;
				checkAllProv.Enabled=false;
			}
			listProv.Items.AddList(_listProviders,x => x.GetLongDesc());
			if(checkAllProv.Enabled==false && _listProviders.Count>0) {
				listProv.SetSelected(0);
			}
			if(!PrefC.HasClinicsEnabled) {
				listClin.Visible=false;
				labelClin.Visible=false;
				checkAllClin.Visible=false;
			}
			else {
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

		private void butOK_Click(object sender,System.EventArgs e) {
			if(date2.SelectionStart<date1.SelectionStart) {
				MsgBox.Show(this,"End date cannot be before start date.");
				return;
			}
			if(!checkAllProv.Checked && listProv.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			if(PrefC.HasClinicsEnabled) {
				if(!checkAllClin.Checked && listClin.SelectedIndices.Count==0) {
					MsgBox.Show(this,"At least one clinic must be selected.");
					return;
				}
			}
			_listProvNums=new List<long>();
			_listClinicNums=new List<long>();
			for(int i=0;i<listProv.SelectedIndices.Count;i++) {
				_listProvNums.Add(_listProviders[listProv.SelectedIndices[i]].ProvNum);
			}
			if(PrefC.HasClinicsEnabled) {
				for(int i=0;i<listClin.SelectedIndices.Count;i++) {
					if(Security.CurUser.ClinicIsRestricted) {
						_listClinicNums.Add(_listClinics[listClin.SelectedIndices[i]].ClinicNum);//we know that the list is a 1:1 to _listClinics
					}
					else {
						if(listClin.SelectedIndices[i]==0) {
							_listClinicNums.Add(0);
						}
						else {
							_listClinicNums.Add(_listClinics[listClin.SelectedIndices[i]-1].ClinicNum);//Minus 1 from the selected index
						}
					}
				}
				if(checkAllClin.Checked) {//All Clinics selected; add all visible or hidden unrestricted clinics to the list
					_listClinicNums=_listClinicNums.Union(Clinics.GetAllForUserod(Security.CurUser).Select(x => x.ClinicNum)).ToList();
				}
			}
			if(radioIndividual.Checked){
				CreateIndividual();
			}
			else{
				CreateGrouped();
			}
		}

		private void CreateIndividual() {
			ReportComplex report=new ReportComplex(true,false);
			bool isAnyClinicMedical=false;//Used to determine whether or not to display 'Tooth' column
			if(AnyClinicSelectedIsMedical()) {
				isAnyClinicMedical=true;
			}
			DataTable table=new DataTable();
			try { 
				table=RpProcSheet.GetIndividualTable(date1.SelectionStart,date2.SelectionStart,_listProvNums,_listClinicNums,textCode.Text,
					isAnyClinicMedical,checkAllProv.Checked,PrefC.HasClinicsEnabled);
			}
			catch (Exception ex) {
				report.CloseProgressBar();
				string text=Lan.g(this,"Error getting report data:")+" "+ex.Message+"\r\n\r\n"+ex.StackTrace;
				using MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(text);
				msgBox.ShowDialog();
				return;
			}
			if(table.Columns.Contains("ToothNum")) {
				foreach(DataRow row in table.Rows) {
					row["ToothNum"]=Tooth.GetToothLabel(row["ToothNum"].ToString());
				}
			}
			string subtitleProvs=ConstructProviderSubtitle();
			string subtitleClinics=ConstructClinicSubtitle();
			Font font=new Font("Tahoma",9);
			Font fontBold=new Font("Tahoma",9,FontStyle.Bold);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Daily Procedures");
			report.AddTitle("Title",Lan.g(this,"Daily Procedures"),fontTitle);
			report.AddSubTitle("Practice Title",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Dates of Report",date1.SelectionStart.ToString("d")+" - "+date2.SelectionStart.ToString("d"),fontSubTitle);
			report.AddSubTitle("Providers",subtitleProvs,fontSubTitle);
			if(PrefC.HasClinicsEnabled) {
				report.AddSubTitle("Clinics",subtitleClinics,fontSubTitle);
			}
			QueryObject query=report.AddQuery(table,Lan.g(this,"Date")+": "+DateTime.Today.ToString("d"));
			query.AddColumn(Lan.g(this,"Date"),90,FieldValueType.Date,font);
			query.GetColumnDetail(Lan.g(this,"Date")).StringFormat="d";
			query.AddColumn(Lan.g(this,"Patient Name"),150,FieldValueType.String,font);
			if(isAnyClinicMedical) {
				query.AddColumn(Lan.g(this,"Code"),140,FieldValueType.String,font);
			}
			else {
				query.AddColumn(Lan.g(this,"Code"),70,FieldValueType.String,font);
				query.AddColumn("Tooth",40,FieldValueType.String,font);
			}
			query.AddColumn(Lan.g(this,"Description"),140,FieldValueType.String,font);
			query.AddColumn(Lan.g(this,"Provider"),80,FieldValueType.String,font);
			if(PrefC.HasClinicsEnabled) {
				query.AddColumn(Lan.g(this,"Clinic"),100,FieldValueType.String,font);
			}
			query.AddColumn(Lan.g(this,"Fee"),80,FieldValueType.Number,font);
			report.AddPageNum(font);
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}

		private void CreateGrouped() {
			ReportComplex report=new ReportComplex(true,false);
			DataTable table=RpProcSheet.GetGroupedTable(date1.SelectionStart,date2.SelectionStart,_listProvNums,_listClinicNums,textCode.Text,checkAllProv.Checked);
			string subtitleProvs=ConstructProviderSubtitle();
			string subtitleClinics=ConstructClinicSubtitle();
			Font font=new Font("Tahoma",9);
			Font fontBold=new Font("Tahoma",9,FontStyle.Bold);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Procedures By Procedure Code");
			report.AddTitle("Title",Lan.g(this,"Procedures By Procedure Code"),fontTitle);
			report.AddSubTitle("Practice Title",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Dates of Report",date1.SelectionStart.ToString("d")+" - "+date2.SelectionStart.ToString("d"),fontSubTitle);
			report.AddSubTitle("Providers",subtitleProvs,fontSubTitle);
			if(PrefC.HasClinicsEnabled) {
				report.AddSubTitle("Clinics",subtitleClinics,fontSubTitle);
			}
			QueryObject query=report.AddQuery(table,Lan.g(this,"Date")+": "+DateTime.Today.ToString("d"));
			query.AddColumn(Lan.g(this,"Category"),150,FieldValueType.String,font);
			query.AddColumn(Lan.g(this,"Code"),130,FieldValueType.String,font);
			query.AddColumn(Lan.g(this,"Description"),140,FieldValueType.String,font);
			query.AddColumn(Lan.g(this,"Quantity"),60,FieldValueType.Integer,font);
			query.GetColumnDetail(Lan.g(this,"Quantity")).ContentAlignment=ContentAlignment.MiddleRight;
			query.AddColumn(Lan.g(this,"Average Fee"),110,FieldValueType.String,font);
			query.GetColumnDetail(Lan.g(this,"Average Fee")).ContentAlignment=ContentAlignment.MiddleRight;
			query.AddColumn(Lan.g(this,"Total Fees"),110,FieldValueType.Number,font);
			report.AddPageNum(font);
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}

		///<summary>Returns 'All Providers' or comma separated string of clinics providers selected.</summary>
		private string ConstructProviderSubtitle() {
			string subtitleProvs="";
			if(checkAllProv.Checked) {
				return Lan.g(this,"All Providers");
			}
			for(int i=0;i<listProv.SelectedIndices.Count;i++) {
				if(i>0) {
					subtitleProvs+=", ";
				}
				subtitleProvs+=_listProviders[listProv.SelectedIndices[i]].Abbr;
			}
			return subtitleProvs;
		}

		///<summary>Returns 'All Clinics' or comma separated string of clinics selected.</summary>
		private string ConstructClinicSubtitle() {
			string subtitleClinics="";
			if(!PrefC.HasClinicsEnabled) {
				return subtitleClinics;
			}
			if(checkAllClin.Checked) {
				return Lan.g(this,"All Clinics");
			}
			for(int i=0;i<listClin.SelectedIndices.Count;i++) {
				if(i>0) {
					subtitleClinics+=", ";
				}
				if(Security.CurUser.ClinicIsRestricted) {
					subtitleClinics+=_listClinics[listClin.SelectedIndices[i]].Abbr;
				}
				else {
					if(listClin.SelectedIndices[i]==0) {
						subtitleClinics+=Lan.g(this,"Unassigned");
					}
					else {
						subtitleClinics+=_listClinics[listClin.SelectedIndices[i]-1].Abbr;//Minus 1 from the selected index to account for 'Unassigned' 
					}
				}
			}
			return subtitleClinics;
		}

		private bool AnyClinicSelectedIsMedical() {
			if(!PrefC.HasClinicsEnabled) {
				return Clinics.IsMedicalPracticeOrClinic(0);//Check if the practice is medical
			}
			if(Security.CurUser.ClinicIsRestricted) {//User can only view one clinic
				return Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum);
			}
			for(int i=0;i<listClin.SelectedIndices.Count;i++) {
				if(listClin.SelectedIndices[i]==0 //The user selected 'Unassigned' 
					&& Clinics.IsMedicalPracticeOrClinic(0)) //And the practice is medical
				{
					return true;
				}
				//if(Clinics.IsMedicalPracticeOrClinic(_listClinics[listClin.SelectedIndices[i]-1].ClinicNum)) {//Minus 1 from the selected index
				if(listClin.SelectedIndices[i]!=0 && Clinics.IsMedicalPracticeOrClinic(_listClinics[listClin.SelectedIndices[i]-1].ClinicNum)) {//Minus 1 from the selected index
					return true;
				}
			}
			return false;
		}
		
	}
}


