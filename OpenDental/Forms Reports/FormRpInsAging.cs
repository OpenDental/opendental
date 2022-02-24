using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	///<summary></summary>
	public class FormRpInsAging : FormODBase {
		#region Designer Variables
		private System.ComponentModel.Container components = null;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.GroupBox groupAgeOfAccount;
		private System.Windows.Forms.Label label1;
		private OpenDental.ValidDate textDate;
		private OpenDental.UI.ListBoxOD listBillType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton radio30;
		private System.Windows.Forms.RadioButton radio90;
		private System.Windows.Forms.RadioButton radio60;
		private OpenDental.UI.ListBoxOD listProv;
		private Label label3;
		private CheckBox checkProvAll;
		private CheckBox checkBillTypesAll;
		private CheckBox checkAllClin;
		private OpenDental.UI.ListBoxOD listClin;
		private Label labelClin;
		private System.Windows.Forms.RadioButton radioAny;
		private GroupBox groupGroupBy;
		private RadioButton radioGroupByPat;
		private RadioButton radioGroupByFam;
		private CheckBox checkOnlyShowPatsOutstandingClaims;
		private Label labelFutureTrans;
		private GroupBox groupFilter;
		private TextBox textGroupName;
		private TextBox textCarrier;
		private Label labelGroupName;
		private Label labelCarrier;
		#endregion
		private List<Provider> _listProviders;
		private List<Def> _listBillingTypeDefs;

		///<summary></summary>
		public FormRpInsAging(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		///<summary></summary>
		protected override void Dispose(bool disposing){
			if(disposing){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpInsAging));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.groupAgeOfAccount = new System.Windows.Forms.GroupBox();
			this.radio30 = new System.Windows.Forms.RadioButton();
			this.radio90 = new System.Windows.Forms.RadioButton();
			this.radio60 = new System.Windows.Forms.RadioButton();
			this.radioAny = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.textDate = new OpenDental.ValidDate();
			this.listBillType = new OpenDental.UI.ListBoxOD();
			this.label2 = new System.Windows.Forms.Label();
			this.listProv = new OpenDental.UI.ListBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.checkProvAll = new System.Windows.Forms.CheckBox();
			this.checkBillTypesAll = new System.Windows.Forms.CheckBox();
			this.checkAllClin = new System.Windows.Forms.CheckBox();
			this.listClin = new OpenDental.UI.ListBoxOD();
			this.labelClin = new System.Windows.Forms.Label();
			this.groupGroupBy = new System.Windows.Forms.GroupBox();
			this.radioGroupByPat = new System.Windows.Forms.RadioButton();
			this.radioGroupByFam = new System.Windows.Forms.RadioButton();
			this.labelFutureTrans = new System.Windows.Forms.Label();
			this.checkOnlyShowPatsOutstandingClaims = new System.Windows.Forms.CheckBox();
			this.groupFilter = new System.Windows.Forms.GroupBox();
			this.textGroupName = new System.Windows.Forms.TextBox();
			this.textCarrier = new System.Windows.Forms.TextBox();
			this.labelGroupName = new System.Windows.Forms.Label();
			this.labelCarrier = new System.Windows.Forms.Label();
			this.groupAgeOfAccount.SuspendLayout();
			this.groupGroupBy.SuspendLayout();
			this.groupFilter.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(677, 438);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 11;
			this.butCancel.Text = "&Cancel";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(596, 438);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 10;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// groupAgeOfAccount
			// 
			this.groupAgeOfAccount.Controls.Add(this.radio30);
			this.groupAgeOfAccount.Controls.Add(this.radio90);
			this.groupAgeOfAccount.Controls.Add(this.radio60);
			this.groupAgeOfAccount.Controls.Add(this.radioAny);
			this.groupAgeOfAccount.Location = new System.Drawing.Point(12, 42);
			this.groupAgeOfAccount.Name = "groupAgeOfAccount";
			this.groupAgeOfAccount.Size = new System.Drawing.Size(233, 110);
			this.groupAgeOfAccount.TabIndex = 2;
			this.groupAgeOfAccount.TabStop = false;
			this.groupAgeOfAccount.Text = "Age of Account";
			// 
			// radio30
			// 
			this.radio30.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radio30.Location = new System.Drawing.Point(6, 40);
			this.radio30.Name = "radio30";
			this.radio30.Size = new System.Drawing.Size(197, 18);
			this.radio30.TabIndex = 1;
			this.radio30.Text = "Over 30 Days";
			// 
			// radio90
			// 
			this.radio90.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radio90.Location = new System.Drawing.Point(6, 84);
			this.radio90.Name = "radio90";
			this.radio90.Size = new System.Drawing.Size(197, 18);
			this.radio90.TabIndex = 3;
			this.radio90.Text = "Over 90 Days";
			// 
			// radio60
			// 
			this.radio60.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radio60.Location = new System.Drawing.Point(6, 62);
			this.radio60.Name = "radio60";
			this.radio60.Size = new System.Drawing.Size(197, 18);
			this.radio60.TabIndex = 2;
			this.radio60.Text = "Over 60 Days";
			// 
			// radioAny
			// 
			this.radioAny.Checked = true;
			this.radioAny.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioAny.Location = new System.Drawing.Point(6, 18);
			this.radioAny.Name = "radioAny";
			this.radioAny.Size = new System.Drawing.Size(197, 18);
			this.radioAny.TabIndex = 0;
			this.radioAny.TabStop = true;
			this.radioAny.Text = "Any Balance";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(125, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "As Of Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(18, 14);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(106, 20);
			this.textDate.TabIndex = 1;
			// 
			// listBillType
			// 
			this.listBillType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBillType.Location = new System.Drawing.Point(251, 48);
			this.listBillType.Name = "listBillType";
			this.listBillType.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBillType.Size = new System.Drawing.Size(163, 368);
			this.listBillType.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(247, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(163, 17);
			this.label2.TabIndex = 0;
			this.label2.Text = "Billing Types";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listProv
			// 
			this.listProv.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listProv.Location = new System.Drawing.Point(420, 48);
			this.listProv.Name = "listProv";
			this.listProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProv.Size = new System.Drawing.Size(163, 368);
			this.listProv.TabIndex = 7;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(416, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(163, 17);
			this.label3.TabIndex = 0;
			this.label3.Text = "Providers";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkProvAll
			// 
			this.checkProvAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkProvAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkProvAll.Location = new System.Drawing.Point(420, 27);
			this.checkProvAll.Name = "checkProvAll";
			this.checkProvAll.Size = new System.Drawing.Size(163, 18);
			this.checkProvAll.TabIndex = 6;
			this.checkProvAll.Text = "All";
			this.checkProvAll.CheckedChanged += new System.EventHandler(this.checkProvAll_CheckedChanged);
			// 
			// checkBillTypesAll
			// 
			this.checkBillTypesAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBillTypesAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBillTypesAll.Location = new System.Drawing.Point(251, 27);
			this.checkBillTypesAll.Name = "checkBillTypesAll";
			this.checkBillTypesAll.Size = new System.Drawing.Size(163, 18);
			this.checkBillTypesAll.TabIndex = 4;
			this.checkBillTypesAll.Text = "All";
			this.checkBillTypesAll.CheckedChanged += new System.EventHandler(this.checkBillTypesAll_CheckedChanged);
			// 
			// checkAllClin
			// 
			this.checkAllClin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAllClin.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllClin.Location = new System.Drawing.Point(589, 27);
			this.checkAllClin.Name = "checkAllClin";
			this.checkAllClin.Size = new System.Drawing.Size(163, 18);
			this.checkAllClin.TabIndex = 8;
			this.checkAllClin.Text = "All";
			this.checkAllClin.CheckedChanged += new System.EventHandler(this.checkAllClin_CheckedChanged);
			// 
			// listClin
			// 
			this.listClin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listClin.Location = new System.Drawing.Point(589, 48);
			this.listClin.Name = "listClin";
			this.listClin.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClin.Size = new System.Drawing.Size(163, 368);
			this.listClin.TabIndex = 9;
			this.listClin.Click += new System.EventHandler(this.listClin_Click);
			// 
			// labelClin
			// 
			this.labelClin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelClin.Location = new System.Drawing.Point(585, 8);
			this.labelClin.Name = "labelClin";
			this.labelClin.Size = new System.Drawing.Size(163, 17);
			this.labelClin.TabIndex = 0;
			this.labelClin.Text = "Clinics";
			this.labelClin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupGroupBy
			// 
			this.groupGroupBy.Controls.Add(this.radioGroupByPat);
			this.groupGroupBy.Controls.Add(this.radioGroupByFam);
			this.groupGroupBy.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupGroupBy.Location = new System.Drawing.Point(12, 158);
			this.groupGroupBy.Name = "groupGroupBy";
			this.groupGroupBy.Size = new System.Drawing.Size(233, 64);
			this.groupGroupBy.TabIndex = 3;
			this.groupGroupBy.TabStop = false;
			this.groupGroupBy.Text = "Group By";
			// 
			// radioGroupByPat
			// 
			this.radioGroupByPat.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioGroupByPat.Location = new System.Drawing.Point(6, 40);
			this.radioGroupByPat.Name = "radioGroupByPat";
			this.radioGroupByPat.Size = new System.Drawing.Size(197, 18);
			this.radioGroupByPat.TabIndex = 1;
			this.radioGroupByPat.Text = "Individual";
			this.radioGroupByPat.UseVisualStyleBackColor = true;
			// 
			// radioGroupByFam
			// 
			this.radioGroupByFam.Checked = true;
			this.radioGroupByFam.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioGroupByFam.Location = new System.Drawing.Point(6, 18);
			this.radioGroupByFam.Name = "radioGroupByFam";
			this.radioGroupByFam.Size = new System.Drawing.Size(197, 18);
			this.radioGroupByFam.TabIndex = 0;
			this.radioGroupByFam.TabStop = true;
			this.radioGroupByFam.Text = "Family";
			this.radioGroupByFam.UseVisualStyleBackColor = true;
			// 
			// labelFutureTrans
			// 
			this.labelFutureTrans.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelFutureTrans.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.labelFutureTrans.Location = new System.Drawing.Point(323, 442);
			this.labelFutureTrans.Name = "labelFutureTrans";
			this.labelFutureTrans.Size = new System.Drawing.Size(267, 18);
			this.labelFutureTrans.TabIndex = 0;
			this.labelFutureTrans.Text = "Future dated transactions are allowed";
			this.labelFutureTrans.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelFutureTrans.Visible = false;
			// 
			// checkOnlyShowPatsOutstandingClaims
			// 
			this.checkOnlyShowPatsOutstandingClaims.Location = new System.Drawing.Point(18, 222);
			this.checkOnlyShowPatsOutstandingClaims.Name = "checkOnlyShowPatsOutstandingClaims";
			this.checkOnlyShowPatsOutstandingClaims.Size = new System.Drawing.Size(227, 41);
			this.checkOnlyShowPatsOutstandingClaims.TabIndex = 12;
			this.checkOnlyShowPatsOutstandingClaims.Text = "Only show patients with outstanding claims";
			this.checkOnlyShowPatsOutstandingClaims.UseVisualStyleBackColor = true;
			this.checkOnlyShowPatsOutstandingClaims.CheckedChanged += new System.EventHandler(this.checkShowBreakdownOptions_CheckedChanged);
			// 
			// groupFilter
			// 
			this.groupFilter.Controls.Add(this.textGroupName);
			this.groupFilter.Controls.Add(this.textCarrier);
			this.groupFilter.Controls.Add(this.labelGroupName);
			this.groupFilter.Controls.Add(this.labelCarrier);
			this.groupFilter.Location = new System.Drawing.Point(12, 269);
			this.groupFilter.Name = "groupFilter";
			this.groupFilter.Size = new System.Drawing.Size(221, 71);
			this.groupFilter.TabIndex = 14;
			this.groupFilter.TabStop = false;
			this.groupFilter.Text = "Filter";
			// 
			// textGroupName
			// 
			this.textGroupName.Location = new System.Drawing.Point(115, 39);
			this.textGroupName.Name = "textGroupName";
			this.textGroupName.Size = new System.Drawing.Size(100, 20);
			this.textGroupName.TabIndex = 3;
			// 
			// textCarrier
			// 
			this.textCarrier.Location = new System.Drawing.Point(115, 14);
			this.textCarrier.Name = "textCarrier";
			this.textCarrier.Size = new System.Drawing.Size(100, 20);
			this.textCarrier.TabIndex = 2;
			// 
			// labelGroupName
			// 
			this.labelGroupName.Location = new System.Drawing.Point(6, 43);
			this.labelGroupName.Name = "labelGroupName";
			this.labelGroupName.Size = new System.Drawing.Size(108, 13);
			this.labelGroupName.TabIndex = 5;
			this.labelGroupName.Text = "Group Name like";
			this.labelGroupName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCarrier
			// 
			this.labelCarrier.Location = new System.Drawing.Point(6, 18);
			this.labelCarrier.Name = "labelCarrier";
			this.labelCarrier.Size = new System.Drawing.Size(108, 13);
			this.labelCarrier.TabIndex = 4;
			this.labelCarrier.Text = "Carrier Name like";
			this.labelCarrier.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormRpInsAging
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(764, 476);
			this.Controls.Add(this.groupFilter);
			this.Controls.Add(this.checkOnlyShowPatsOutstandingClaims);
			this.Controls.Add(this.labelFutureTrans);
			this.Controls.Add(this.groupGroupBy);
			this.Controls.Add(this.checkAllClin);
			this.Controls.Add(this.listClin);
			this.Controls.Add(this.labelClin);
			this.Controls.Add(this.checkBillTypesAll);
			this.Controls.Add(this.checkProvAll);
			this.Controls.Add(this.listProv);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listBillType);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupAgeOfAccount);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpInsAging";
			this.ShowInTaskbar = false;
			this.Text = "Insurance Aging Report";
			this.Load += new System.EventHandler(this.FormRpInsAging_Load);
			this.groupAgeOfAccount.ResumeLayout(false);
			this.groupGroupBy.ResumeLayout(false);
			this.groupFilter.ResumeLayout(false);
			this.groupFilter.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormRpInsAging_Load(object sender, System.EventArgs e) {
			_listProviders=Providers.GetListReports();
			DateTime lastAgingDate=PrefC.GetDate(PrefName.DateLastAging);
			if(lastAgingDate.Year<1880) {
				textDate.Text="";
			}
			else if(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)){
				textDate.Text=lastAgingDate.ToShortDateString();
			}
			else{
				textDate.Text=DateTime.Today.ToShortDateString();
			}
			_listBillingTypeDefs=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			listBillType.Items.AddList(_listBillingTypeDefs,x => x.ItemName);
			listBillType.SelectedIndex=(listBillType.Items.Count>0?0:-1);
			checkBillTypesAll.Checked=true; //all billing types by default, event handler will set visibility
			listProv.Items.AddList(_listProviders,x => x.GetLongDesc());
			listProv.SelectedIndex=(listProv.Items.Count>0?0:-1);
			checkProvAll.Checked=true; //all provs by default, event handler will set visibility
			if(!PrefC.HasClinicsEnabled) {
				checkAllClin.Visible=false;//event handler may set listClin to visible, so hide explicitly after setting unchecked just in case
				listClin.Visible=false;
				labelClin.Visible=false;
			}
			else {
				List<Clinic> listClinics=Clinics.GetForUserod(Security.CurUser,true,"Unassigned").ToList();
				if(!listClinics.Exists(x => x.ClinicNum==Clinics.ClinicNum)) {//Could have a hidden clinic selected
					listClinics.Add(Clinics.GetClinic(Clinics.ClinicNum));
				}
				listClin.Items.AddList(listClinics,x => x.Abbr+(x.IsHidden?(" "+Lan.g(this,"(hidden)")):""));
				listClin.SelectedIndex=listClinics.FindIndex(x => x.ClinicNum==Clinics.ClinicNum);//FindIndex could return -1, which is fine
				checkAllClin.Checked=(Clinics.ClinicNum==0);//event handler will set visibility
			}
			if(PrefC.GetBool(PrefName.FutureTransDatesAllowed) || PrefC.GetBool(PrefName.AccountAllowFutureDebits) 
				|| PrefC.GetBool(PrefName.AllowFutureInsPayments)) 
			{
				labelFutureTrans.Visible=true;//Set to false in designer
			}
			if(!checkOnlyShowPatsOutstandingClaims.Checked) {
				groupFilter.Visible=false;
			}
		}

		private void checkBillTypesAll_CheckedChanged(object sender,EventArgs e) {
			listBillType.Visible=!checkBillTypesAll.Checked;
		}

		private void checkProvAll_CheckedChanged(object sender,EventArgs e) {
			listProv.Visible=!checkProvAll.Checked;
		}

		private void checkAllClin_CheckedChanged(object sender,EventArgs e) {
			listClin.Visible=!checkAllClin.Checked;
		}

		private void listClin_Click(object sender,EventArgs e) {
			checkAllClin.Checked=false;//will not clear all selected indices, event handler will hide listClin
		}

		///<summary>Sets parameters/fills lists based on form controls.</summary>
		private RpAgingParamObject GetParamsFromForm() {
			RpAgingParamObject rpo=new RpAgingParamObject();
			rpo.AsOfDate=PIn.Date(textDate.Text);
			if(rpo.AsOfDate.Year<1880) {
				rpo.AsOfDate=DateTime.Today;
			}
			rpo.IsHistoric=(rpo.AsOfDate.Date!=DateTime.Today);
			rpo.IsGroupByFam=radioGroupByFam.Checked;
			rpo.IsInsPayWoCombined=false;
			if(!checkBillTypesAll.Checked) {
				rpo.ListBillTypes=listBillType.SelectedIndices.OfType<int>().Select(x => _listBillingTypeDefs[x].DefNum).ToList();
			}
			if(!checkProvAll.Checked) {
				rpo.ListProvNums=listProv.SelectedIndices.OfType<int>().Select(x => _listProviders[x].ProvNum).ToList();
			}
			if(PrefC.HasClinicsEnabled) {
				//if "All" is selected and the user is not restricted, show ALL clinics, including the 0 clinic.
				if(checkAllClin.Checked && !Security.CurUser.ClinicIsRestricted){
					rpo.ListClinicNums.Clear();
					rpo.ListClinicNums.Add(0);
					rpo.ListClinicNums.AddRange(Clinics.GetDeepCopy().Select(x => x.ClinicNum));
				}
				else {
					rpo.ListClinicNums=listClin.GetListSelected<Clinic>().Select(x => x.ClinicNum).ToList();
				}
			}
			rpo.AccountAge=AgeOfAccount.Any;
			if(radio30.Checked) {
				rpo.AccountAge=AgeOfAccount.Over30;
			}
			else if(radio60.Checked) {
				rpo.AccountAge=AgeOfAccount.Over60;
			}
			else if(radio90.Checked) {
				rpo.AccountAge=AgeOfAccount.Over90;
			}
			if(checkOnlyShowPatsOutstandingClaims.Checked) {
				rpo.IsDetailedBreakdown=true;
			}
			rpo.GroupByCarrier=rpo.IsDetailedBreakdown;
			rpo.GroupByGroupName=rpo.IsDetailedBreakdown;
			rpo.CarrierNameFilter=textCarrier.Text;
			rpo.GroupNameFilter=textGroupName.Text;
			rpo.IsWoAged=true;
			rpo.IsIncludeNeg=true;
			rpo.IsForInsAging=true;
			rpo.IsIncludeInsNoBal=true;
			return rpo;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!checkBillTypesAll.Checked && listBillType.SelectedIndices.Count==0){
				MsgBox.Show(this,"At least one billing type must be selected.");
				return;
			}
			if(!checkProvAll.Checked && listProv.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			if(PrefC.HasClinicsEnabled && !checkAllClin.Checked && listClin.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one clinic must be selected.");
				return;
			}
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Invalid date.");
				return;
			}
			RpAgingParamObject rpo=GetParamsFromForm();
			ReportComplex report=new ReportComplex(true,true); 
			DataTable tableAging=new DataTable();
			tableAging=RpInsAging.GetInsAgingTable(rpo);
			report.ReportName=Lan.g(this,"Insurance Aging Report");
			report.AddTitle("InsAging",Lan.g(this, "Insurance Aging Report"));
			report.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle));
			report.AddSubTitle("AsOf",Lan.g(this,"As of")+" "+rpo.AsOfDate.ToShortDateString());
			if(radioAny.Checked){
				report.AddSubTitle("Balance",Lan.g(this,"Any Balance"));
			}
			else if(radio30.Checked){
				report.AddSubTitle("Over30",Lan.g(this,"Over 30 Days"));
			}
			else if(radio60.Checked){
				report.AddSubTitle("Over60",Lan.g(this,"Over 60 Days"));
			}
			else if(radio90.Checked){
				report.AddSubTitle("Over90",Lan.g(this,"Over 90 Days"));
			}
			if(checkBillTypesAll.Checked){
				report.AddSubTitle("AllBillingTypes",Lan.g(this,"All Billing Types"));
			}
			else{
				report.AddSubTitle("",string.Join(", ",listBillType.SelectedIndices.OfType<int>().Select(x => _listBillingTypeDefs[x].ItemName)));//there must be at least one selected
			}
			if(checkProvAll.Checked) {
				report.AddSubTitle("Providers",Lan.g(this,"All Providers"));
			}
			else {
				report.AddSubTitle("Providers",string.Join(", ",listProv.SelectedIndices.OfType<int>().Select(x => _listProviders[x].Abbr)));
			}
			if(checkAllClin.Checked) {
				report.AddSubTitle("Clinics",Lan.g(this,"All Clinics"));
			}
			else {
				report.AddSubTitle("Clinics",string.Join(", ",listClin.GetListSelected<Clinic>().Select(x => x.Abbr)));
			}
			//Patient Account Aging Query-----------------------------------------------
			QueryObject query=report.AddQuery(tableAging,"Date "+DateTime.Today.ToShortDateString());
			query.AddColumn((radioGroupByFam.Checked ? "Guarantor" : "Patient"),150,FieldValueType.String);
			if(rpo.IsDetailedBreakdown) {
				query.AddColumn("Carrier",220,FieldValueType.String);
				query.AddColumn("Group Name",160,FieldValueType.String);
			}
			query.AddColumn("Ins Pay\r\nEst 0-30",75,FieldValueType.Number);
			query.AddColumn("Ins Pay\r\nEst 31-60",75,FieldValueType.Number);
			query.AddColumn("Ins Pay\r\nEst 61-90",70,FieldValueType.Number);
			query.AddColumn("Ins Pay\r\nEst >90",75,FieldValueType.Number);
			query.AddColumn("Ins Pay\r\nEst Total", 80,FieldValueType.Number);
			if(!rpo.IsDetailedBreakdown) {
				query.AddColumn("Pat Est\r\nBal 0-30",75,FieldValueType.Number);
				query.AddColumn("Pat Est\r\nBal 31-60",75,FieldValueType.Number);
				query.AddColumn("Pat Est\r\nBal 61-90",70,FieldValueType.Number);
				query.AddColumn("Pat Est\r\nBal >90",75,FieldValueType.Number);
				query.AddColumn("Pat Est\r\nBal Total",80,FieldValueType.Number);
				query.AddColumn("-W/O\r\nChange",70,FieldValueType.Number);
				query.AddColumn("=Pat Est\r\nAmt Due",80,FieldValueType.Number);
			}
			report.AddPageNum();
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;		
		}

		private void checkShowBreakdownOptions_CheckedChanged(object sender,EventArgs e) {
			groupFilter.Visible=checkOnlyShowPatsOutstandingClaims.Checked;
		}
	}

}
