using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using OpenDental.ReportingComplex;
using CodeBase;
using System.Linq;

namespace OpenDental{
///<summary></summary>
	public class FormRpWriteoffSheet : FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.MonthCalendar date2;
		private System.Windows.Forms.MonthCalendar date1;
		private System.Windows.Forms.Label labelTO;
		private System.ComponentModel.Container components = null;
		private OpenDental.UI.ListBoxOD listProv;
		private Label label1;
		private GroupBox groupBox3;
		private Label label5;
		private RadioButton radioWriteoffProc;
		private RadioButton radioWriteoffPay;
		private CheckBox checkAllProv;
		private CheckBox checkAllClin;
		private OpenDental.UI.ListBoxOD listClin;
		private Label labelClin;
		private List<Clinic> _listClinics;
		private List<Provider> _listProviders;
		private RadioButton radioWriteoffClaim;
		private bool _hasClinicsEnabled;

		///<summary></summary>
		public FormRpWriteoffSheet(){
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpWriteoffSheet));
			this.date2 = new System.Windows.Forms.MonthCalendar();
			this.date1 = new System.Windows.Forms.MonthCalendar();
			this.labelTO = new System.Windows.Forms.Label();
			this.listProv = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.radioWriteoffClaim = new System.Windows.Forms.RadioButton();
			this.label5 = new System.Windows.Forms.Label();
			this.radioWriteoffProc = new System.Windows.Forms.RadioButton();
			this.radioWriteoffPay = new System.Windows.Forms.RadioButton();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.checkAllProv = new System.Windows.Forms.CheckBox();
			this.checkAllClin = new System.Windows.Forms.CheckBox();
			this.listClin = new OpenDental.UI.ListBoxOD();
			this.labelClin = new System.Windows.Forms.Label();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// date2
			// 
			this.date2.Location = new System.Drawing.Point(285, 41);
			this.date2.Name = "date2";
			this.date2.TabIndex = 2;
			// 
			// date1
			// 
			this.date1.Location = new System.Drawing.Point(32, 41);
			this.date1.Name = "date1";
			this.date1.TabIndex = 1;
			// 
			// labelTO
			// 
			this.labelTO.Location = new System.Drawing.Point(246, 37);
			this.labelTO.Name = "labelTO";
			this.labelTO.Size = new System.Drawing.Size(51, 23);
			this.labelTO.TabIndex = 28;
			this.labelTO.Text = "TO";
			this.labelTO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// listProv
			// 
			this.listProv.Location = new System.Drawing.Point(529, 56);
			this.listProv.Name = "listProv";
			this.listProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProv.Size = new System.Drawing.Size(181, 147);
			this.listProv.TabIndex = 36;
			this.listProv.Click += new System.EventHandler(this.listProv_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(527, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 35;
			this.label1.Text = "Providers";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.radioWriteoffClaim);
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.radioWriteoffProc);
			this.groupBox3.Controls.Add(this.radioWriteoffPay);
			this.groupBox3.Location = new System.Drawing.Point(32, 246);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(281, 135);
			this.groupBox3.TabIndex = 47;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Show Insurance Write-offs";
			// 
			// radioWriteoffClaim
			// 
			this.radioWriteoffClaim.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffClaim.Location = new System.Drawing.Point(9, 62);
			this.radioWriteoffClaim.Name = "radioWriteoffClaim";
			this.radioWriteoffClaim.Size = new System.Drawing.Size(244, 43);
			this.radioWriteoffClaim.TabIndex = 3;
			this.radioWriteoffClaim.Text = "Using initial claim date for write-off estimates, insurance pay date for write-of" +
    "f adjustments\r\n";
			this.radioWriteoffClaim.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffClaim.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 108);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(269, 17);
			this.label5.TabIndex = 2;
			this.label5.Text = "(This is discussed in the PPO section of the manual)";
			// 
			// radioWriteoffProc
			// 
			this.radioWriteoffProc.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffProc.Location = new System.Drawing.Point(9, 41);
			this.radioWriteoffProc.Name = "radioWriteoffProc";
			this.radioWriteoffProc.Size = new System.Drawing.Size(244, 23);
			this.radioWriteoffProc.TabIndex = 1;
			this.radioWriteoffProc.Text = "Using procedure date.";
			this.radioWriteoffProc.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffProc.UseVisualStyleBackColor = true;
			// 
			// radioWriteoffPay
			// 
			this.radioWriteoffPay.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffPay.Checked = true;
			this.radioWriteoffPay.Location = new System.Drawing.Point(9, 20);
			this.radioWriteoffPay.Name = "radioWriteoffPay";
			this.radioWriteoffPay.Size = new System.Drawing.Size(244, 23);
			this.radioWriteoffPay.TabIndex = 0;
			this.radioWriteoffPay.TabStop = true;
			this.radioWriteoffPay.Text = "Using insurance payment date.";
			this.radioWriteoffPay.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffPay.UseVisualStyleBackColor = true;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(632, 379);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(632, 347);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// checkAllProv
			// 
			this.checkAllProv.Checked = true;
			this.checkAllProv.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllProv.Location = new System.Drawing.Point(529, 37);
			this.checkAllProv.Name = "checkAllProv";
			this.checkAllProv.Size = new System.Drawing.Size(95, 16);
			this.checkAllProv.TabIndex = 48;
			this.checkAllProv.Text = "All";
			this.checkAllProv.Click += new System.EventHandler(this.checkAllProv_Click);
			// 
			// checkAllClin
			// 
			this.checkAllClin.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllClin.Location = new System.Drawing.Point(361, 227);
			this.checkAllClin.Name = "checkAllClin";
			this.checkAllClin.Size = new System.Drawing.Size(151, 16);
			this.checkAllClin.TabIndex = 54;
			this.checkAllClin.Text = "All (Includes hidden)";
			this.checkAllClin.Click += new System.EventHandler(this.checkAllClin_Click);
			// 
			// listClin
			// 
			this.listClin.Location = new System.Drawing.Point(361, 246);
			this.listClin.Name = "listClin";
			this.listClin.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClin.Size = new System.Drawing.Size(151, 147);
			this.listClin.TabIndex = 53;
			this.listClin.Click += new System.EventHandler(this.listClin_Click);
			// 
			// labelClin
			// 
			this.labelClin.Location = new System.Drawing.Point(358, 209);
			this.labelClin.Name = "labelClin";
			this.labelClin.Size = new System.Drawing.Size(104, 16);
			this.labelClin.TabIndex = 52;
			this.labelClin.Text = "Clinics";
			this.labelClin.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormRpWriteoffSheet
			// 
			this.ClientSize = new System.Drawing.Size(741, 431);
			this.Controls.Add(this.checkAllClin);
			this.Controls.Add(this.listClin);
			this.Controls.Add(this.labelClin);
			this.Controls.Add(this.checkAllProv);
			this.Controls.Add(this.groupBox3);
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
			this.Name = "FormRpWriteoffSheet";
			this.ShowInTaskbar = false;
			this.Text = "Daily Writeoff Report";
			this.Load += new System.EventHandler(this.FormDailyWriteoff_Load);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormDailyWriteoff_Load(object sender, System.EventArgs e) {
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
				_hasClinicsEnabled=false;
			}
			else {
				_listClinics=Clinics.GetForUserod(Security.CurUser);
				_hasClinicsEnabled=true;
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
			switch(PrefC.GetInt(PrefName.ReportsPPOwriteoffDefaultToProcDate)){
				case 0: radioWriteoffPay.Checked=true; break;
				case 1: radioWriteoffProc.Checked=true; break;
				case 2: radioWriteoffClaim.Checked=true; break;
				default:
					radioWriteoffClaim.Checked=true; break;
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

		private void butOK_Click(object sender,System.EventArgs e) {
			if(date2.SelectionStart<date1.SelectionStart) {
				MsgBox.Show(this,"End date cannot be before start date.");
				return;
			}
			if(!checkAllProv.Checked && listProv.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			if(_hasClinicsEnabled) {
				if(!checkAllClin.Checked && listClin.SelectedIndices.Count==0) {
					MsgBox.Show(this,"At least one clinic must be selected.");
					return;
				}
			}
			List<long> listClinicNums=new List<long>();
			for(int i=0;i<listClin.SelectedIndices.Count;i++) {
				if(Security.CurUser.ClinicIsRestricted) {
					listClinicNums.Add(_listClinics[listClin.SelectedIndices[i]].ClinicNum);//we know that the list is a 1:1 to _listClinics
				}
				else {
					if(listClin.SelectedIndices[i]==0) {
						listClinicNums.Add(0);
					}
					else {
						listClinicNums.Add(_listClinics[listClin.SelectedIndices[i]-1].ClinicNum);//Minus 1 from the selected index
					}
				}
			}
			if(checkAllClin.Checked) {//All Clinics selected; add all visible or hidden unrestricted clinics to the list
				listClinicNums=listClinicNums.Union(Clinics.GetAllForUserod(Security.CurUser).Select(x => x.ClinicNum)).ToList();
			}
			List<string> listProvNames=new List<string>();
			List<long> listProvNums=new List<long>();
			if(checkAllProv.Checked) {
				for(int i = 0;i<_listProviders.Count;i++) {
					listProvNums.Add(_listProviders[i].ProvNum);
					listProvNames.Add(_listProviders[i].Abbr);
				}	
			}
			else {
				for(int i=0;i<listProv.SelectedIndices.Count;i++) {
					listProvNums.Add(_listProviders[listProv.SelectedIndices[i]].ProvNum);
					listProvNames.Add(_listProviders[listProv.SelectedIndices[i]].Abbr);
				}
			}
			ReportComplex report=new ReportComplex(true,false);
			PPOWriteoffDateCalc writeoffType=GetWriteoffType();
			DataTable table=RpWriteoffSheet.GetWriteoffTable(date1.SelectionStart,date2.SelectionStart,listProvNums,listClinicNums
				,checkAllClin.Checked,_hasClinicsEnabled,writeoffType);
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Daily Writeoffs");
			report.AddTitle("Title",Lan.g(this,"Daily Writeoffs"),fontTitle);
			report.AddSubTitle("PracticeTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Date SubTitle",date1.SelectionStart.ToString("d")+" - "+date2.SelectionStart.ToString("d"),fontSubTitle);
			if(checkAllProv.Checked) {
				report.AddSubTitle("Providers",Lan.g(this,"All Providers")); 
			}
			else {
				report.AddSubTitle("Providers",string.Join(", ",listProvNames));
			}
			if(_hasClinicsEnabled) {
				if(checkAllClin.Checked) {
					report.AddSubTitle("Clinics",Lan.g(this,"All Clinics"));
				}
				else {
					string clinNames="";
					for(int i=0;i<listClin.SelectedIndices.Count;i++) {
						if(i>0) {
							clinNames+=", ";
						}
						if(Security.CurUser.ClinicIsRestricted) {
							clinNames+=_listClinics[listClin.SelectedIndices[i]].Abbr;
						}
						else {
							if(listClin.SelectedIndices[i]==0) {
								clinNames+=Lan.g(this,"Unassigned");
							}
							else {
								clinNames+=_listClinics[listClin.SelectedIndices[i]-1].Abbr;//Minus 1 from the selected index
							}
						}
					}
					report.AddSubTitle("Clinics",clinNames);
				}
			}
			QueryObject query=report.AddQuery(table,Lan.g(this,"Date")+": "+DateTime.Today.ToString("d"));
			query.AddColumn("Date",90,FieldValueType.Date);
			query.AddColumn("Patient Name",150);
			query.AddColumn("Carrier",150);
			query.AddColumn("Provider",60);
			if(_hasClinicsEnabled) {
				query.AddColumn("Clinic",80);
				
			}
			if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
				query.AddColumn("Writeoff Estimate",70,FieldValueType.Number,font);
				query.AddColumn("Writeoff Adjustment",80,FieldValueType.Number,font);
			}
			query.AddColumn("Writeoff",70,FieldValueType.Number);
			query.GetColumnDetail("Writeoff").ContentAlignment=ContentAlignment.MiddleRight;
			if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
				query.AddGroupSummaryField("Writeoff (Writeoff Estimate + Writeoff Adjustment)","Provider","$writeoff",SummaryOperation.Sum,new List<int>(){0},Color.Black,new Font("Tahoma",9,FontStyle.Bold),0,50);
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
