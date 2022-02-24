using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public class FormRpPaySheet : FormODBase{
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.MonthCalendar date2;
		private System.Windows.Forms.MonthCalendar date1;
		private System.ComponentModel.Container components = null;
		private OpenDental.UI.ListBoxOD listProv;
		private Label label1;
		private GroupBox groupBox1;
		private RadioButton radioPatient;
		private RadioButton radioCheck;
		private CheckBox checkPatientTypes;
		private OpenDental.UI.ListBoxOD listPatientTypes;
		private CheckBox checkInsuranceTypes;
		private CheckBox checkAllClin;
		private OpenDental.UI.ListBoxOD listClin;
		private Label labelClin;
		private OpenDental.UI.ListBoxOD listInsuranceTypes;
		private CheckBox checkAllProv;
		private List<Clinic> _listClinics;
		private CheckBox checkAllClaimPayGroups;
		private OpenDental.UI.ListBoxOD listClaimPayGroups;
		private CheckBox checkUnearned;
		private CheckBox checkShowProvSeparate;
		private List<Provider> _listProviders;
		private List<Def> _listInsDefs;
		private List<Def> _listPayDefs;
		private CheckBox checkReportDisplayUnearnedTP;
		private List<Def> _listClaimPayGroupDefs;

		///<summary></summary>
		public FormRpPaySheet(){
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpPaySheet));
			this.date2 = new System.Windows.Forms.MonthCalendar();
			this.date1 = new System.Windows.Forms.MonthCalendar();
			this.listProv = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.checkAllProv = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkShowProvSeparate = new System.Windows.Forms.CheckBox();
			this.radioPatient = new System.Windows.Forms.RadioButton();
			this.radioCheck = new System.Windows.Forms.RadioButton();
			this.checkPatientTypes = new System.Windows.Forms.CheckBox();
			this.listPatientTypes = new OpenDental.UI.ListBoxOD();
			this.checkInsuranceTypes = new System.Windows.Forms.CheckBox();
			this.checkAllClin = new System.Windows.Forms.CheckBox();
			this.listClin = new OpenDental.UI.ListBoxOD();
			this.labelClin = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.listInsuranceTypes = new OpenDental.UI.ListBoxOD();
			this.checkAllClaimPayGroups = new System.Windows.Forms.CheckBox();
			this.listClaimPayGroups = new OpenDental.UI.ListBoxOD();
			this.checkUnearned = new System.Windows.Forms.CheckBox();
			this.checkReportDisplayUnearnedTP = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// date2
			// 
			this.date2.Location = new System.Drawing.Point(252, 36);
			this.date2.Name = "date2";
			this.date2.TabIndex = 2;
			// 
			// date1
			// 
			this.date1.Location = new System.Drawing.Point(16, 36);
			this.date1.Name = "date1";
			this.date1.TabIndex = 1;
			// 
			// listProv
			// 
			this.listProv.Location = new System.Drawing.Point(493, 54);
			this.listProv.Name = "listProv";
			this.listProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProv.Size = new System.Drawing.Size(160, 199);
			this.listProv.TabIndex = 36;
			this.listProv.Click += new System.EventHandler(this.listProv_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(491, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 35;
			this.label1.Text = "Providers";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkAllProv
			// 
			this.checkAllProv.Checked = true;
			this.checkAllProv.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllProv.Location = new System.Drawing.Point(494, 35);
			this.checkAllProv.Name = "checkAllProv";
			this.checkAllProv.Size = new System.Drawing.Size(40, 16);
			this.checkAllProv.TabIndex = 43;
			this.checkAllProv.Text = "All";
			this.checkAllProv.Click += new System.EventHandler(this.checkAllProv_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkShowProvSeparate);
			this.groupBox1.Controls.Add(this.radioPatient);
			this.groupBox1.Controls.Add(this.radioCheck);
			this.groupBox1.Location = new System.Drawing.Point(18, 263);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(173, 101);
			this.groupBox1.TabIndex = 44;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Group By";
			// 
			// checkShowProvSeparate
			// 
			this.checkShowProvSeparate.Checked = true;
			this.checkShowProvSeparate.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowProvSeparate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowProvSeparate.Location = new System.Drawing.Point(8, 61);
			this.checkShowProvSeparate.Name = "checkShowProvSeparate";
			this.checkShowProvSeparate.Size = new System.Drawing.Size(159, 34);
			this.checkShowProvSeparate.TabIndex = 55;
			this.checkShowProvSeparate.Text = "Show splits by provider separately";
			this.checkShowProvSeparate.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			// 
			// radioPatient
			// 
			this.radioPatient.Location = new System.Drawing.Point(8, 37);
			this.radioPatient.Name = "radioPatient";
			this.radioPatient.Size = new System.Drawing.Size(104, 18);
			this.radioPatient.TabIndex = 1;
			this.radioPatient.Text = "Patient";
			this.radioPatient.UseVisualStyleBackColor = true;
			// 
			// radioCheck
			// 
			this.radioCheck.Checked = true;
			this.radioCheck.Location = new System.Drawing.Point(8, 18);
			this.radioCheck.Name = "radioCheck";
			this.radioCheck.Size = new System.Drawing.Size(104, 18);
			this.radioCheck.TabIndex = 0;
			this.radioCheck.TabStop = true;
			this.radioCheck.Text = "Check";
			this.radioCheck.UseVisualStyleBackColor = true;
			// 
			// checkPatientTypes
			// 
			this.checkPatientTypes.Checked = true;
			this.checkPatientTypes.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkPatientTypes.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPatientTypes.Location = new System.Drawing.Point(382, 263);
			this.checkPatientTypes.Name = "checkPatientTypes";
			this.checkPatientTypes.Size = new System.Drawing.Size(166, 16);
			this.checkPatientTypes.TabIndex = 47;
			this.checkPatientTypes.Text = "All patient payment types";
			this.checkPatientTypes.Click += new System.EventHandler(this.checkAllTypes_Click);
			// 
			// listPatientTypes
			// 
			this.listPatientTypes.Location = new System.Drawing.Point(382, 285);
			this.listPatientTypes.Name = "listPatientTypes";
			this.listPatientTypes.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listPatientTypes.Size = new System.Drawing.Size(163, 186);
			this.listPatientTypes.TabIndex = 46;
			// 
			// checkInsuranceTypes
			// 
			this.checkInsuranceTypes.Checked = true;
			this.checkInsuranceTypes.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkInsuranceTypes.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkInsuranceTypes.Location = new System.Drawing.Point(210, 263);
			this.checkInsuranceTypes.Name = "checkInsuranceTypes";
			this.checkInsuranceTypes.Size = new System.Drawing.Size(166, 16);
			this.checkInsuranceTypes.TabIndex = 48;
			this.checkInsuranceTypes.Text = "All insurance payment types";
			this.checkInsuranceTypes.Click += new System.EventHandler(this.checkIns_Click);
			// 
			// checkAllClin
			// 
			this.checkAllClin.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllClin.Location = new System.Drawing.Point(662, 35);
			this.checkAllClin.Name = "checkAllClin";
			this.checkAllClin.Size = new System.Drawing.Size(160, 16);
			this.checkAllClin.TabIndex = 54;
			this.checkAllClin.Text = "All (Includes hidden)";
			this.checkAllClin.Click += new System.EventHandler(this.checkAllClin_Click);
			// 
			// listClin
			// 
			this.listClin.Location = new System.Drawing.Point(662, 54);
			this.listClin.Name = "listClin";
			this.listClin.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClin.Size = new System.Drawing.Size(160, 199);
			this.listClin.TabIndex = 53;
			this.listClin.Click += new System.EventHandler(this.listClin_Click);
			// 
			// labelClin
			// 
			this.labelClin.Location = new System.Drawing.Point(659, 17);
			this.labelClin.Name = "labelClin";
			this.labelClin.Size = new System.Drawing.Size(104, 16);
			this.labelClin.TabIndex = 52;
			this.labelClin.Text = "Clinics";
			this.labelClin.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(747, 445);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(747, 410);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// listInsuranceTypes
			// 
			this.listInsuranceTypes.Location = new System.Drawing.Point(210, 285);
			this.listInsuranceTypes.Name = "listInsuranceTypes";
			this.listInsuranceTypes.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listInsuranceTypes.Size = new System.Drawing.Size(163, 186);
			this.listInsuranceTypes.TabIndex = 55;
			// 
			// checkAllClaimPayGroups
			// 
			this.checkAllClaimPayGroups.Checked = true;
			this.checkAllClaimPayGroups.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllClaimPayGroups.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllClaimPayGroups.Location = new System.Drawing.Point(551, 263);
			this.checkAllClaimPayGroups.Name = "checkAllClaimPayGroups";
			this.checkAllClaimPayGroups.Size = new System.Drawing.Size(166, 16);
			this.checkAllClaimPayGroups.TabIndex = 58;
			this.checkAllClaimPayGroups.Text = "All claim payment groups";
			this.checkAllClaimPayGroups.Click += new System.EventHandler(this.checkAllClaimPayGroups_Click);
			// 
			// listClaimPayGroups
			// 
			this.listClaimPayGroups.Location = new System.Drawing.Point(551, 285);
			this.listClaimPayGroups.Name = "listClaimPayGroups";
			this.listClaimPayGroups.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClaimPayGroups.Size = new System.Drawing.Size(163, 186);
			this.listClaimPayGroups.TabIndex = 57;
			// 
			// checkUnearned
			// 
			this.checkUnearned.Checked = true;
			this.checkUnearned.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkUnearned.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUnearned.Location = new System.Drawing.Point(535, 35);
			this.checkUnearned.Name = "checkUnearned";
			this.checkUnearned.Size = new System.Drawing.Size(118, 16);
			this.checkUnearned.TabIndex = 59;
			this.checkUnearned.Text = "Include Unearned";
			// 
			// checkReportDisplayUnearnedTP
			// 
			this.checkReportDisplayUnearnedTP.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkReportDisplayUnearnedTP.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReportDisplayUnearnedTP.Location = new System.Drawing.Point(26, 370);
			this.checkReportDisplayUnearnedTP.Name = "checkReportDisplayUnearnedTP";
			this.checkReportDisplayUnearnedTP.Size = new System.Drawing.Size(159, 54);
			this.checkReportDisplayUnearnedTP.TabIndex = 60;
			this.checkReportDisplayUnearnedTP.Text = "Include hidden treatment planned prepayments";
			this.checkReportDisplayUnearnedTP.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkReportDisplayUnearnedTP.UseVisualStyleBackColor = true;
			// 
			// FormRpPaySheet
			// 
			this.ClientSize = new System.Drawing.Size(844, 495);
			this.Controls.Add(this.checkReportDisplayUnearnedTP);
			this.Controls.Add(this.checkUnearned);
			this.Controls.Add(this.checkAllClaimPayGroups);
			this.Controls.Add(this.listClaimPayGroups);
			this.Controls.Add(this.listInsuranceTypes);
			this.Controls.Add(this.checkAllClin);
			this.Controls.Add(this.listClin);
			this.Controls.Add(this.labelClin);
			this.Controls.Add(this.checkInsuranceTypes);
			this.Controls.Add(this.checkPatientTypes);
			this.Controls.Add(this.listPatientTypes);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.checkAllProv);
			this.Controls.Add(this.listProv);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.date2);
			this.Controls.Add(this.date1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpPaySheet";
			this.ShowInTaskbar = false;
			this.Text = "Daily Payments Report";
			this.Load += new System.EventHandler(this.FormPaymentSheet_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormPaymentSheet_Load(object sender, System.EventArgs e) {
			_listProviders=Providers.GetListReports();
			_listProviders.Insert(0,Providers.GetUnearnedProv());
			date1.SelectionStart=DateTime.Today;
			date2.SelectionStart=DateTime.Today;
			if(!Security.IsAuthorized(Permissions.ReportDailyAllProviders,true)) {
				//They either have permission or have a provider at this point.  If they don't have permission they must have a provider.
				_listProviders=_listProviders.FindAll(x => x.ProvNum==Security.CurUser.ProvNum);
				Provider prov=_listProviders.FirstOrDefault();
				if(prov!=null) {
					_listProviders.AddRange(Providers.GetWhere(x => x.FName==prov.FName && x.LName==prov.LName && x.ProvNum!=prov.ProvNum));
				}
				checkAllProv.Checked=false;
				checkAllProv.Enabled=false;
			}
			listProv.Items.AddList(_listProviders,x => x.GetLongDesc());
			//If the user is not allowed to run the report for all providers, default the selection to the first in the list box.
			if(checkAllProv.Enabled==false && listProv.Items.Count > 0) {
				listProv.SetSelected(0);
			}
			if(!Security.IsAuthorized(Permissions.ReportDailyAllProviders,true) && listProv.Items.Count>0) {
				listProv.SetAll(true);
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
			checkReportDisplayUnearnedTP.Checked=PrefC.GetBool(PrefName.ReportsDoShowHiddenTPPrepayments);
			_listPayDefs=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			_listPayDefs.Add(new Def {
				ItemName="Income Transfer",
				DefNum=0,//Income Transfer's PayType is 0.
			});
			_listInsDefs=Defs.GetDefsForCategory(DefCat.InsurancePaymentType,true);
			_listClaimPayGroupDefs=Defs.GetDefsForCategory(DefCat.ClaimPaymentGroups,true);
			listPatientTypes.Items.AddList(_listPayDefs,x => x.ItemName);
			listPatientTypes.Visible=false;
			listInsuranceTypes.Items.AddList(_listInsDefs,x => x.ItemName);
			listInsuranceTypes.Visible=false;
			listClaimPayGroups.Items.AddList(_listClaimPayGroupDefs,x => x.ItemName);
			listClaimPayGroups.Visible=false;
			Plugins.HookAddCode(this,"FormPaymentSheet_Load_end");
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

		private void checkAllClaimPayGroups_Click(object sender,EventArgs e) {
			if(checkAllClaimPayGroups.Checked) {
				listClaimPayGroups.Visible=false;
			}
			else {
				listClaimPayGroups.Visible=true;
			}
		}

		private void checkAllTypes_Click(object sender,EventArgs e) {
			if(checkPatientTypes.Checked){
				listPatientTypes.Visible=false;
			}
			else{
				listPatientTypes.Visible=true;
			}
		}

		private void checkIns_Click(object sender,EventArgs e) {
			if(checkInsuranceTypes.Checked) {
				listInsuranceTypes.Visible=false;
			}
			else {
				listInsuranceTypes.Visible=true;
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(!checkAllProv.Checked && listProv.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			if(PrefC.HasClinicsEnabled && !checkAllClin.Checked && listClin.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one clinic must be selected.");
				return;
			}
			if(!checkPatientTypes.Checked && listPatientTypes.SelectedIndices.Count==0
				&& !checkInsuranceTypes.Checked && listInsuranceTypes.SelectedIndices.Count==0)
			{
				MsgBox.Show(this,"At least one type must be selected.");
				return;
			}
			if(!checkAllClaimPayGroups.Checked && listClaimPayGroups.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one claim payment group must be selected.");
				return;
			}
			ReportComplex report=new ReportComplex(true,false);
			List<long> listProvNums=new List<long>();
			List<long> listClinicNums=new List<long>();
			List<long> listInsTypes=new List<long>();
			List<long> listPatTypes=new List<long>();
			List<long> listSelectedClaimPayGroupNums=new List<long>();
			if(checkAllProv.Checked) {
				listProvNums=_listProviders.Select(x => x.ProvNum).ToList();
			}
			else {
				listProvNums=listProv.SelectedIndices.Select(x => _listProviders[x].ProvNum).ToList();
			}
			if(PrefC.HasClinicsEnabled) {
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
			}
			if(checkInsuranceTypes.Checked) {
				listInsTypes=_listInsDefs.Select(x => x.DefNum).ToList();
			}
			else {
				listInsTypes=listInsuranceTypes.SelectedIndices.Select(x => _listInsDefs[x].DefNum).ToList();
			}
			if(checkPatientTypes.Checked) {
				listPatTypes=_listPayDefs.Select(x => x.DefNum).ToList();
			}
			else {
				listPatTypes=listPatientTypes.SelectedIndices.Select(x => _listPayDefs[x].DefNum).ToList();
			}
			if(checkAllClaimPayGroups.Checked) {
				listSelectedClaimPayGroupNums=_listClaimPayGroupDefs.Select(x => x.DefNum).ToList();
			}
			else {
				listSelectedClaimPayGroupNums=listClaimPayGroups.SelectedIndices.Select(x => _listClaimPayGroupDefs[x].DefNum).ToList();
			}
			DataTable tableIns=new DataTable();
			if(listProvNums.Count!=0) {
				tableIns=RpPaySheet.GetInsTable(date1.SelectionStart,date2.SelectionStart,listProvNums,listClinicNums,listInsTypes,
					listSelectedClaimPayGroupNums,checkAllProv.Checked,checkAllClin.Checked,checkInsuranceTypes.Checked,radioPatient.Checked,
					checkAllClaimPayGroups.Checked,checkShowProvSeparate.Checked);
			}
			DataTable tablePat=RpPaySheet.GetPatTable(date1.SelectionStart,date2.SelectionStart,listProvNums,listClinicNums,listPatTypes,
				checkAllProv.Checked,checkAllClin.Checked,checkPatientTypes.Checked,radioPatient.Checked,checkUnearned.Checked,checkShowProvSeparate.Checked,
				checkReportDisplayUnearnedTP.Checked);
			string subtitleProvs="";
			string subtitleClinics="";
			if(checkAllProv.Checked) {
				subtitleProvs=Lan.g(this,"All Providers");
			}
			else {
				subtitleProvs+=string.Join(", ",listProv.SelectedIndices.Select(x => _listProviders[x].Abbr));
			}
			if(PrefC.HasClinicsEnabled) {
				if(checkAllClin.Checked && !Security.CurUser.ClinicIsRestricted) {
					subtitleClinics=Lan.g(this,"All Clinics");
				}
				else {
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
								subtitleClinics+=_listClinics[listClin.SelectedIndices[i]-1].Abbr;//Minus 1 from the selected index
							}
						}
					}
				}
			}
			Font font=new Font("Tahoma",9);
			Font fontBold=new Font("Tahoma",9,FontStyle.Bold);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Daily Payments");
			report.AddTitle("Title",Lan.g(this,"Daily Payments"),fontTitle);
			report.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Providers",subtitleProvs,fontSubTitle);
			if(PrefC.HasClinicsEnabled) {
				report.AddSubTitle("Clinics",subtitleClinics,fontSubTitle);
			}
			Dictionary<long,string> dictInsDefNames=new Dictionary<long,string>();
			Dictionary<long,string> dictPatDefNames=new Dictionary<long,string>();
			List<Def> insDefs=Defs.GetDefsForCategory(DefCat.InsurancePaymentType,true);
			List<Def> patDefs=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			for(int i=0;i<insDefs.Count;i++) {
				dictInsDefNames.Add(insDefs[i].DefNum,insDefs[i].ItemName);
			}
			for(int i=0;i<patDefs.Count;i++) {
				dictPatDefNames.Add(patDefs[i].DefNum,patDefs[i].ItemName);
			}
			dictPatDefNames.Add(0,"Income Transfer");//Otherwise income transfers show up with a payment type of "Undefined"
			int[] summaryGroups1= { 1 };
			int[] summaryGroups2= { 2 };
			int[] summaryGroups3= { 1,2 };
			//Insurance Payments Query-------------------------------------
			QueryObject query=report.AddQuery(tableIns,"Insurance Payments","PayType",SplitByKind.Definition,1,true,dictInsDefNames,fontSubTitle);
			query.AddColumn("Date",90,FieldValueType.Date,font);
			//query.GetColumnDetail("Date").SuppressIfDuplicate = true;
			query.GetColumnDetail("Date").StringFormat="d";
			query.AddColumn("Carrier",150,FieldValueType.String,font);
			query.AddColumn("Patient Name",150,FieldValueType.String,font);
			query.AddColumn("Provider",90,FieldValueType.String,font);
			if(PrefC.HasClinicsEnabled) {
				query.AddColumn("Clinic",120,FieldValueType.String,font);
			}
			query.AddColumn("Check#",75,FieldValueType.String,font);
			query.AddColumn("Amount",90,FieldValueType.Number,font);
			query.AddGroupSummaryField("Total Insurance Payments:","Amount","amt",SummaryOperation.Sum,new List<int>(summaryGroups1),Color.Black,fontBold,0,20);
			//Patient Payments Query---------------------------------------
			query=report.AddQuery(tablePat,"Patient Payments","PayType",SplitByKind.Definition,2,true,dictPatDefNames,fontSubTitle);
			query.AddColumn("Date",90,FieldValueType.Date,font);
			//query.GetColumnDetail("Date").SuppressIfDuplicate = true;
			query.GetColumnDetail("Date").StringFormat="d";
			query.AddColumn("Paying Patient",270,FieldValueType.String,font);
			query.AddColumn("Provider",90,FieldValueType.String,font);
			if(PrefC.HasClinicsEnabled) {
				query.AddColumn("Clinic",120,FieldValueType.String,font);
			}
			query.AddColumn("Check#",75,FieldValueType.String,font);
			query.AddColumn("Amount",120,FieldValueType.Number,font);
			query.AddGroupSummaryField("Total Patient Payments:","Amount","amt",SummaryOperation.Sum,new List<int>(summaryGroups2),Color.Black,fontBold,0,20);
			query.AddGroupSummaryField("Total All Payments:","Amount","amt",SummaryOperation.Sum,new List<int>(summaryGroups3),Color.Black,fontBold,0,4);
			report.AddPageNum(font);
			report.AddGridLines();
			report.AddPageFooterText("PageFooter","*Part of a bulk check, which can be located by going to the listed patient's account",font,0,
				ContentAlignment.MiddleLeft);
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;		
		}
	}
}
