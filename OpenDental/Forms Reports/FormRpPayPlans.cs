using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Globalization;
using System.Drawing.Printing;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;
using System.Collections.Generic;
using System.Linq;

namespace OpenDental
{
	/// <summary>
	/// Summary description for FormRpApptWithPhones.
	/// </summary>
	public class FormRpPayPlans:FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.ComponentModel.Container components = null;
		private CheckBox checkHideCompletePlans;
		private CheckBox checkAllProv;
		private OpenDental.UI.ListBoxOD listProv;
		private Label label1;
		private GroupBox groupBox1;
		private RadioButton radioBoth;
		private RadioButton radioPatient;
		private RadioButton radioInsurance;
		private CheckBox checkShowFamilyBalance;
		private CheckBox checkAllClin;
		private OpenDental.UI.ListBoxOD listClin;
		private Label labelClin;
		private List<Clinic> _listClinics;
		private CheckBox checkHasDateRange;
		private DateTimePicker dateStart;
		private DateTimePicker dateEnd;
		private Label label2;
		private Label label3;
		//private int pagesPrinted;
		private ErrorProvider errorProvider1=new ErrorProvider();
		//private DataTable BirthdayTable;
		//private int patientsPrinted;
		//private PrintDocument pd;
		//private OpenDental.UI.PrintPreview printPreview;
		private List<Provider> _listProviders;

		///<summary></summary>
		public FormRpPayPlans()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		///<summary></summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				components?.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpPayPlans));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.checkHideCompletePlans = new System.Windows.Forms.CheckBox();
			this.checkAllProv = new System.Windows.Forms.CheckBox();
			this.listProv = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioBoth = new System.Windows.Forms.RadioButton();
			this.radioPatient = new System.Windows.Forms.RadioButton();
			this.radioInsurance = new System.Windows.Forms.RadioButton();
			this.checkShowFamilyBalance = new System.Windows.Forms.CheckBox();
			this.checkAllClin = new System.Windows.Forms.CheckBox();
			this.listClin = new OpenDental.UI.ListBoxOD();
			this.labelClin = new System.Windows.Forms.Label();
			this.checkHasDateRange = new System.Windows.Forms.CheckBox();
			this.dateStart = new System.Windows.Forms.DateTimePicker();
			this.dateEnd = new System.Windows.Forms.DateTimePicker();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(501, 325);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 44;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(420, 325);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 43;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// checkHideCompletePlans
			// 
			this.checkHideCompletePlans.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHideCompletePlans.Location = new System.Drawing.Point(31, 168);
			this.checkHideCompletePlans.Name = "checkHideCompletePlans";
			this.checkHideCompletePlans.Size = new System.Drawing.Size(216, 18);
			this.checkHideCompletePlans.TabIndex = 45;
			this.checkHideCompletePlans.Text = "Hide Completed Payment Plans";
			this.checkHideCompletePlans.UseVisualStyleBackColor = true;
			// 
			// checkAllProv
			// 
			this.checkAllProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllProv.Location = new System.Drawing.Point(252, 93);
			this.checkAllProv.Name = "checkAllProv";
			this.checkAllProv.Size = new System.Drawing.Size(95, 16);
			this.checkAllProv.TabIndex = 48;
			this.checkAllProv.Text = "All";
			this.checkAllProv.Click += new System.EventHandler(this.checkAllProv_Click);
			// 
			// listProv
			// 
			this.listProv.Location = new System.Drawing.Point(251, 113);
			this.listProv.Name = "listProv";
			this.listProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProv.Size = new System.Drawing.Size(163, 199);
			this.listProv.TabIndex = 47;
			this.listProv.Click += new System.EventHandler(this.listProv_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(249, 74);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 46;
			this.label1.Text = "Providers";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioBoth);
			this.groupBox1.Controls.Add(this.radioPatient);
			this.groupBox1.Controls.Add(this.radioInsurance);
			this.groupBox1.Location = new System.Drawing.Point(23, 75);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(173, 87);
			this.groupBox1.TabIndex = 49;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Payment Plan Types";
			// 
			// radioBoth
			// 
			this.radioBoth.Checked = true;
			this.radioBoth.Location = new System.Drawing.Point(8, 58);
			this.radioBoth.Name = "radioBoth";
			this.radioBoth.Size = new System.Drawing.Size(159, 18);
			this.radioBoth.TabIndex = 2;
			this.radioBoth.TabStop = true;
			this.radioBoth.Text = "Both";
			this.radioBoth.UseVisualStyleBackColor = true;
			// 
			// radioPatient
			// 
			this.radioPatient.Location = new System.Drawing.Point(8, 38);
			this.radioPatient.Name = "radioPatient";
			this.radioPatient.Size = new System.Drawing.Size(159, 18);
			this.radioPatient.TabIndex = 1;
			this.radioPatient.Text = "Patient";
			this.radioPatient.UseVisualStyleBackColor = true;
			// 
			// radioInsurance
			// 
			this.radioInsurance.Location = new System.Drawing.Point(8, 19);
			this.radioInsurance.Name = "radioInsurance";
			this.radioInsurance.Size = new System.Drawing.Size(159, 18);
			this.radioInsurance.TabIndex = 0;
			this.radioInsurance.Text = "Insurance";
			this.radioInsurance.UseVisualStyleBackColor = true;
			// 
			// checkShowFamilyBalance
			// 
			this.checkShowFamilyBalance.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowFamilyBalance.Location = new System.Drawing.Point(31, 189);
			this.checkShowFamilyBalance.Name = "checkShowFamilyBalance";
			this.checkShowFamilyBalance.Size = new System.Drawing.Size(216, 18);
			this.checkShowFamilyBalance.TabIndex = 52;
			this.checkShowFamilyBalance.Text = "Show Family Balance";
			this.checkShowFamilyBalance.UseVisualStyleBackColor = true;
			// 
			// checkAllClin
			// 
			this.checkAllClin.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllClin.Location = new System.Drawing.Point(420, 94);
			this.checkAllClin.Name = "checkAllClin";
			this.checkAllClin.Size = new System.Drawing.Size(154, 16);
			this.checkAllClin.TabIndex = 57;
			this.checkAllClin.Text = "All (Includes hidden)";
			this.checkAllClin.Click += new System.EventHandler(this.checkAllClin_Click);
			// 
			// listClin
			// 
			this.listClin.Location = new System.Drawing.Point(420, 113);
			this.listClin.Name = "listClin";
			this.listClin.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClin.Size = new System.Drawing.Size(154, 199);
			this.listClin.TabIndex = 56;
			this.listClin.Click += new System.EventHandler(this.listClin_Click);
			// 
			// labelClin
			// 
			this.labelClin.Location = new System.Drawing.Point(417, 76);
			this.labelClin.Name = "labelClin";
			this.labelClin.Size = new System.Drawing.Size(104, 16);
			this.labelClin.TabIndex = 55;
			this.labelClin.Text = "Clinics";
			this.labelClin.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkHasDateRange
			// 
			this.checkHasDateRange.Checked = true;
			this.checkHasDateRange.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkHasDateRange.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHasDateRange.Location = new System.Drawing.Point(31, 210);
			this.checkHasDateRange.Name = "checkHasDateRange";
			this.checkHasDateRange.Size = new System.Drawing.Size(216, 18);
			this.checkHasDateRange.TabIndex = 58;
			this.checkHasDateRange.Text = "Limit to Plans Created in Date Range";
			this.checkHasDateRange.UseVisualStyleBackColor = true;
			this.checkHasDateRange.Click += new System.EventHandler(this.checkHasDateRange_Click);
			// 
			// dateStart
			// 
			this.dateStart.Location = new System.Drawing.Point(47, 32);
			this.dateStart.Name = "dateStart";
			this.dateStart.Size = new System.Drawing.Size(224, 20);
			this.dateStart.TabIndex = 59;
			// 
			// dateEnd
			// 
			this.dateEnd.Location = new System.Drawing.Point(315, 32);
			this.dateEnd.Name = "dateEnd";
			this.dateEnd.Size = new System.Drawing.Size(224, 20);
			this.dateEnd.TabIndex = 60;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(47, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 23);
			this.label2.TabIndex = 61;
			this.label2.Text = "Date Start:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(315, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 23);
			this.label3.TabIndex = 62;
			this.label3.Text = "Date End:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormRpPayPlans
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(586, 361);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.dateEnd);
			this.Controls.Add(this.dateStart);
			this.Controls.Add(this.checkHasDateRange);
			this.Controls.Add(this.checkAllClin);
			this.Controls.Add(this.listClin);
			this.Controls.Add(this.labelClin);
			this.Controls.Add(this.checkShowFamilyBalance);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.checkAllProv);
			this.Controls.Add(this.listProv);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkHideCompletePlans);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpPayPlans";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Payment Plans Report";
			this.Load += new System.EventHandler(this.FormRpPayPlans_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormRpPayPlans_Load(object sender, System.EventArgs e){
			dateStart.Value=DateTime.Today;
			dateEnd.Value=DateTime.Today;
			checkHideCompletePlans.Checked=true;
			_listProviders=Providers.GetListReports();
			listProv.Items.AddList(_listProviders,x => x.GetLongDesc());
			listProv.SetAll(true);
			checkAllProv.Checked=true;
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
				listProv.SetAll(true);
			}
			else {
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

		private void checkHasDateRange_Click(object sender,EventArgs e) {
			if(checkHasDateRange.Checked) {
				dateStart.Enabled=true;
				dateEnd.Enabled=true;
			}
			else {
				dateStart.Enabled=false;
				dateEnd.Enabled=false;
			}
		}

		private void listClin_Click(object sender,EventArgs e) {
			if(listClin.SelectedIndices.Count>0) {
				checkAllClin.Checked=false;
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(listProv.SelectedIndices.Count==0) {
				MsgBox.Show(this,"Please select at least one provider.");
				return;
			}
			if(PrefC.HasClinicsEnabled) {//Using clinics
				if(listClin.SelectedIndices.Count==0) {
					MsgBox.Show(this,"Please select at least one clinic.");
					return;
				}
			}
			if(dateStart.Value>dateEnd.Value) {
				MsgBox.Show(this,"Start date cannot be greater than the end date.");
				return;
			}
			ReportComplex report=new ReportComplex(true,true);
			List<long> listProvNums=new List<long>();
			List<long> listClinicNums=new List<long>();
			for(int i=0;i<listProv.SelectedIndices.Count;i++) {
				listProvNums.Add(_listProviders[listProv.SelectedIndices[i]].ProvNum);
			}
			if(checkAllProv.Checked) {
				for(int i=0;i<_listProviders.Count;i++) {
					listProvNums.Add(_listProviders[i].ProvNum);
				}
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
			DisplayPayPlanType displayPayPlanType;
			if(radioInsurance.Checked) {
				displayPayPlanType=DisplayPayPlanType.Insurance;
			}
			else if(radioPatient.Checked) {
				displayPayPlanType=DisplayPayPlanType.Patient;
			}
			else {
				displayPayPlanType=DisplayPayPlanType.Both;
			}
			bool isPayPlanV2=(PrefC.GetInt(PrefName.PayPlansVersion)==2);
			DataSet ds=RpPayPlan.GetPayPlanTable(dateStart.Value,dateEnd.Value,listProvNums,listClinicNums,checkAllProv.Checked
					,displayPayPlanType,checkHideCompletePlans.Checked,checkShowFamilyBalance.Checked,checkHasDateRange.Checked,isPayPlanV2);
			DataTable table=ds.Tables["Clinic"];
			DataTable tableTotal=ds.Tables["Total"];
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"PaymentPlans");
			report.AddTitle("Title",Lan.g(this,"Payment Plans"),fontTitle);
			report.AddSubTitle("PracticeTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			if(checkHasDateRange.Checked) {
				report.AddSubTitle("Date SubTitle",dateStart.Value.ToShortDateString()+" - "+dateEnd.Value.ToShortDateString(),fontSubTitle);
			}
			else{
				report.AddSubTitle("Date SubTitle",DateTime.Today.ToShortDateString(),fontSubTitle);
			}
			QueryObject query;
			if(PrefC.HasClinicsEnabled) {
				query=report.AddQuery(table,"","clinicName",SplitByKind.Value,1,true);
			}
			else {
				query=report.AddQuery(table,"","",SplitByKind.None,1,true);
			}
			query.AddColumn("Provider",160,FieldValueType.String,font);
			query.AddColumn("Guarantor",160,FieldValueType.String,font);
			query.AddColumn("Ins",40,FieldValueType.String,font);
			query.GetColumnHeader("Ins").ContentAlignment=ContentAlignment.MiddleCenter;
			query.GetColumnDetail("Ins").ContentAlignment=ContentAlignment.MiddleCenter;
			query.AddColumn("Princ",100,FieldValueType.Number,font);
			query.GetColumnHeader("Princ").ContentAlignment=ContentAlignment.MiddleRight;
			query.GetColumnDetail("Princ").ContentAlignment=ContentAlignment.MiddleRight;
			query.AddColumn("Accum Int",100,FieldValueType.Number,font);
			query.GetColumnHeader("Accum Int").ContentAlignment=ContentAlignment.MiddleRight;
			query.GetColumnDetail("Accum Int").ContentAlignment=ContentAlignment.MiddleRight;
			query.AddColumn("Paid",100,FieldValueType.Number,font);
			query.GetColumnHeader("Paid").ContentAlignment=ContentAlignment.MiddleRight;
			query.GetColumnDetail("Paid").ContentAlignment=ContentAlignment.MiddleRight;
			query.AddColumn("Balance",100,FieldValueType.Number,font);
			query.GetColumnHeader("Balance").ContentAlignment=ContentAlignment.MiddleRight;
			query.GetColumnDetail("Balance").ContentAlignment=ContentAlignment.MiddleRight;
			query.AddColumn("Due Now",100,FieldValueType.Number,font);
			query.GetColumnHeader("Due Now").ContentAlignment=ContentAlignment.MiddleRight;
			query.GetColumnDetail("Due Now").ContentAlignment=ContentAlignment.MiddleRight;
			if(isPayPlanV2) {
				query.AddColumn("Bal Not Due",100,FieldValueType.Number,font);
				query.GetColumnHeader("Bal Not Due").ContentAlignment=ContentAlignment.MiddleRight;
				query.GetColumnDetail("Bal Not Due").ContentAlignment=ContentAlignment.MiddleRight;
			}
			if(checkShowFamilyBalance.Checked) {
				query.AddColumn("Fam Balance",100,FieldValueType.String,font);
				query.GetColumnHeader("Fam Balance").ContentAlignment=ContentAlignment.MiddleRight;
				query.GetColumnDetail("Fam Balance").ContentAlignment=ContentAlignment.MiddleRight;
				query.GetColumnDetail("Fam Balance").SuppressIfDuplicate=true;
			}
			if(PrefC.HasClinicsEnabled) {
				QueryObject queryTotals=report.AddQuery(tableTotal,"Totals");
				queryTotals.AddColumn("Clinic",360,FieldValueType.String,font);
				queryTotals.AddColumn("Princ",100,FieldValueType.Number,font);
				queryTotals.GetColumnHeader("Princ").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.GetColumnDetail("Princ").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.AddColumn("Accum Int",100,FieldValueType.Number,font);
				queryTotals.GetColumnHeader("Accum Int").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.GetColumnDetail("Accum Int").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.AddColumn("Paid",100,FieldValueType.Number,font);
				queryTotals.GetColumnHeader("Paid").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.GetColumnDetail("Paid").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.AddColumn("Balance",100,FieldValueType.Number,font);
				queryTotals.GetColumnHeader("Balance").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.GetColumnDetail("Balance").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.AddColumn("Due Now",100,FieldValueType.Number,font);
				queryTotals.GetColumnHeader("Due Now").ContentAlignment=ContentAlignment.MiddleRight;
				queryTotals.GetColumnDetail("Due Now").ContentAlignment=ContentAlignment.MiddleRight;
				if(isPayPlanV2) {
					queryTotals.AddColumn("Bal Not Due",100,FieldValueType.Number,font);
					queryTotals.GetColumnHeader("Bal Not Due").ContentAlignment=ContentAlignment.MiddleRight;
					queryTotals.GetColumnDetail("Bal Not Due").ContentAlignment=ContentAlignment.MiddleRight;
				}
				if(checkShowFamilyBalance.Checked) {
					queryTotals.AddColumn("Fam Balance",100,FieldValueType.String,font);
					queryTotals.GetColumnHeader("Fam Balance").ContentAlignment=ContentAlignment.MiddleRight;
					queryTotals.GetColumnDetail("Fam Balance").ContentAlignment=ContentAlignment.MiddleRight;
					queryTotals.GetColumnDetail("Fam Balance").SuppressIfDuplicate=true;
				}
			}
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		













		

		

		
	}
}
