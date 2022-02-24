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
	public class FormRpAdjSheet : FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.MonthCalendar date2;
		private System.Windows.Forms.MonthCalendar date1;
		private System.Windows.Forms.Label labelTO;
		private System.ComponentModel.Container components = null;
		private OpenDental.UI.ListBoxOD listProv;
		private Label label1;
		private OpenDental.UI.ListBoxOD listType;
		private Label label2;
		private CheckBox checkAllProv;
		private CheckBox checkAllClin;
		private OpenDental.UI.ListBoxOD listClin;
		private Label labelClin;
		private List<Clinic> _listClinics;
		private List<Provider> _listProviders;
		private bool _hasClinicsEnabled;
		private CheckBox checkAllAdjs;
		///<summary>Holds all adjustment types, does NOT include hidden.</summary>
		private List<Def> _listAdjTypeDefs;

		///<summary></summary>
		public FormRpAdjSheet(){
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpAdjSheet));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.date2 = new System.Windows.Forms.MonthCalendar();
			this.date1 = new System.Windows.Forms.MonthCalendar();
			this.labelTO = new System.Windows.Forms.Label();
			this.listProv = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.listType = new OpenDental.UI.ListBoxOD();
			this.label2 = new System.Windows.Forms.Label();
			this.checkAllProv = new System.Windows.Forms.CheckBox();
			this.checkAllClin = new System.Windows.Forms.CheckBox();
			this.listClin = new OpenDental.UI.ListBoxOD();
			this.labelClin = new System.Windows.Forms.Label();
			this.checkAllAdjs = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(634, 390);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(634, 358);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// date2
			// 
			this.date2.Location = new System.Drawing.Point(288, 37);
			this.date2.Name = "date2";
			this.date2.TabIndex = 2;
			// 
			// date1
			// 
			this.date1.Location = new System.Drawing.Point(32, 37);
			this.date1.Name = "date1";
			this.date1.TabIndex = 1;
			// 
			// labelTO
			// 
			this.labelTO.Location = new System.Drawing.Point(248, 37);
			this.labelTO.Name = "labelTO";
			this.labelTO.Size = new System.Drawing.Size(51, 23);
			this.labelTO.TabIndex = 28;
			this.labelTO.Text = "TO";
			this.labelTO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// listProv
			// 
			this.listProv.Location = new System.Drawing.Point(533, 51);
			this.listProv.Name = "listProv";
			this.listProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProv.Size = new System.Drawing.Size(181, 147);
			this.listProv.TabIndex = 36;
			this.listProv.Click += new System.EventHandler(this.listProv_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(530, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 35;
			this.label1.Text = "Providers";
			// 
			// listType
			// 
			this.listType.Location = new System.Drawing.Point(32, 245);
			this.listType.Name = "listType";
			this.listType.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listType.Size = new System.Drawing.Size(227, 147);
			this.listType.TabIndex = 39;
			this.listType.Click += new System.EventHandler(this.listType_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(29, 209);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(120, 16);
			this.label2.TabIndex = 38;
			this.label2.Text = "Adjustment Types";
			// 
			// checkAllProv
			// 
			this.checkAllProv.Checked = true;
			this.checkAllProv.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllProv.Location = new System.Drawing.Point(533, 34);
			this.checkAllProv.Name = "checkAllProv";
			this.checkAllProv.Size = new System.Drawing.Size(95, 16);
			this.checkAllProv.TabIndex = 48;
			this.checkAllProv.Text = "All";
			this.checkAllProv.Click += new System.EventHandler(this.checkAllProv_Click);
			// 
			// checkAllClin
			// 
			this.checkAllClin.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllClin.Location = new System.Drawing.Point(288, 226);
			this.checkAllClin.Name = "checkAllClin";
			this.checkAllClin.Size = new System.Drawing.Size(154, 16);
			this.checkAllClin.TabIndex = 51;
			this.checkAllClin.Text = "All (Includes hidden)";
			this.checkAllClin.Click += new System.EventHandler(this.checkAllClin_Click);
			// 
			// listClin
			// 
			this.listClin.Location = new System.Drawing.Point(288, 245);
			this.listClin.Name = "listClin";
			this.listClin.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClin.Size = new System.Drawing.Size(154, 147);
			this.listClin.TabIndex = 50;
			this.listClin.Click += new System.EventHandler(this.listClin_Click);
			// 
			// labelClin
			// 
			this.labelClin.Location = new System.Drawing.Point(285, 208);
			this.labelClin.Name = "labelClin";
			this.labelClin.Size = new System.Drawing.Size(104, 16);
			this.labelClin.TabIndex = 49;
			this.labelClin.Text = "Clinics";
			this.labelClin.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkAllAdjs
			// 
			this.checkAllAdjs.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllAdjs.Location = new System.Drawing.Point(32, 226);
			this.checkAllAdjs.Name = "checkAllAdjs";
			this.checkAllAdjs.Size = new System.Drawing.Size(95, 16);
			this.checkAllAdjs.TabIndex = 52;
			this.checkAllAdjs.Text = "All";
			this.checkAllAdjs.Click += new System.EventHandler(this.checkAllAdjs_Click);
			// 
			// FormRpAdjSheet
			// 
			this.ClientSize = new System.Drawing.Size(743, 442);
			this.Controls.Add(this.checkAllAdjs);
			this.Controls.Add(this.checkAllClin);
			this.Controls.Add(this.listClin);
			this.Controls.Add(this.labelClin);
			this.Controls.Add(this.checkAllProv);
			this.Controls.Add(this.listType);
			this.Controls.Add(this.label2);
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
			this.Name = "FormRpAdjSheet";
			this.ShowInTaskbar = false;
			this.Text = "Daily Adjustment Report";
			this.Load += new System.EventHandler(this.FormDailyAdjustment_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormDailyAdjustment_Load(object sender, System.EventArgs e) {
			date1.SelectionStart=DateTime.Today;
			date2.SelectionStart=DateTime.Today;
			_listProviders=Providers.GetListReports();
			if(!Security.IsAuthorized(Permissions.ReportDailyAllProviders,true)) {
				//They either have permission or have a provider at this point.  If they don't have permission they must have a provider.
				_listProviders=_listProviders.FindAll(x => x.ProvNum==Security.CurUser.ProvNum);
				checkAllProv.Checked=false;
				checkAllProv.Enabled=false;
			}
			listProv.Items.AddList(_listProviders,x => x.GetLongDesc());
			if(checkAllProv.Enabled==false && _listProviders.Count>0) {
				listProv.SetSelected(0,true);
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
			_listAdjTypeDefs=Defs.GetDefsForCategory(DefCat.AdjTypes,true);//Exclude hidden.
			checkAllAdjs.Checked=true;
			listType.Items.AddList(_listAdjTypeDefs,x => x.ItemName);
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

		private void listType_Click(object sender,EventArgs e) {
			if(listType.SelectedIndices.Count>0) {
				checkAllAdjs.Checked=false;
			}
		}

		private void checkAllAdjs_Click(object sender,EventArgs e) {
			if(checkAllAdjs.Checked) {
				listType.ClearSelected();
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
			if(!checkAllAdjs.Checked && listType.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one type must be selected.");
				return;
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
			List<long> listProvNums=new List<long>();
			if(checkAllProv.Checked) {
				for(int i = 0;i<_listProviders.Count;i++) {
					listProvNums.Add(_listProviders[i].ProvNum);
				}
			}
			else {
				for(int i=0;i<listProv.SelectedIndices.Count;i++) {
					listProvNums.Add(_listProviders[listProv.SelectedIndices[i]].ProvNum);
				}
			}
			List<string> listAdjType=new List<string>();
			if(checkAllAdjs.Checked) {
				//add all adjustment types, including hidden
				listAdjType=Defs.GetDefsForCategory(DefCat.AdjTypes).Select(x => POut.Long(x.DefNum)).ToList();
			}
			else {
				for(int i=0;i<listType.SelectedIndices.Count;i++) {//1:1
					listAdjType.Add(POut.Long(_listAdjTypeDefs[listType.SelectedIndices[i]].DefNum));
				}
			}
			ReportComplex report=new ReportComplex(true,false);	 
			DataTable table=RpAdjSheet.GetAdjTable(date1.SelectionStart,date2.SelectionStart,listProvNums,
				listClinicNums,listAdjType,checkAllClin.Checked,_hasClinicsEnabled);
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Daily Adjustments");
			report.AddTitle("Title",Lan.g(this,"Daily Adjustments"),fontTitle);
			report.AddSubTitle("PracticeTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Date SubTitle",date1.SelectionStart.ToString("d")+" - "+date2.SelectionStart.ToString("d"),fontSubTitle);
			if(checkAllProv.Checked) {
				report.AddSubTitle("Provider SubTitle",Lan.g(this,"All Providers"));
			}
			else {
				string provNames="";
				for(int i=0;i<listProv.SelectedIndices.Count;i++) {
					if(i>0) {
						provNames+=", ";
					}
					provNames+=_listProviders[listProv.SelectedIndices[i]].Abbr;
				}
				report.AddSubTitle("Provider SubTitle",provNames);
			}
			if(_hasClinicsEnabled) {
				if(checkAllClin.Checked) {
					report.AddSubTitle("Clinic SubTitle",Lan.g(this,"All Clinics"));
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
					report.AddSubTitle("Clinic SubTitle",clinNames);
				}
			}
			QueryObject query=report.AddQuery(table,Lan.g(this,"Date")+": "+DateTime.Today.ToString("d"));
			query.AddColumn("Date",90,FieldValueType.Date);
			query.AddColumn("Patient Name",130,FieldValueType.String);
			query.AddColumn("Prov",60,FieldValueType.String);
			if(_hasClinicsEnabled) {
				query.AddColumn("Clinic",70,FieldValueType.String);
			}
			query.AddColumn("AdjustmentType",150,FieldValueType.String);
			query.AddColumn("Note",180,FieldValueType.String);
			query.AddColumn("Amount",75,FieldValueType.Number);
			query.GetColumnDetail("Amount").ContentAlignment=ContentAlignment.MiddleRight;
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
