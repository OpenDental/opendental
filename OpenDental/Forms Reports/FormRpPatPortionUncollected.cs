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
	public class FormRpPatPortionUncollected : FormODBase{
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.MonthCalendar date2;
		private System.Windows.Forms.MonthCalendar date1;
		private System.ComponentModel.Container components = null;
		private CheckBox checkAllClin;
		private OpenDental.UI.ListBoxOD listClin;
		private Label labelClin;
		private List<Clinic> _listClinics;
		///<summary></summary>
		public FormRpPatPortionUncollected(){
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpPatPortionUncollected));
			this.date2 = new System.Windows.Forms.MonthCalendar();
			this.date1 = new System.Windows.Forms.MonthCalendar();
			this.checkAllClin = new System.Windows.Forms.CheckBox();
			this.listClin = new OpenDental.UI.ListBoxOD();
			this.labelClin = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// date2
			// 
			this.date2.Location = new System.Drawing.Point(261, 50);
			this.date2.Name = "date2";
			this.date2.TabIndex = 2;
			// 
			// date1
			// 
			this.date1.Location = new System.Drawing.Point(18, 50);
			this.date1.Name = "date1";
			this.date1.TabIndex = 1;
			// 
			// checkAllClin
			// 
			this.checkAllClin.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllClin.Location = new System.Drawing.Point(18, 233);
			this.checkAllClin.Name = "checkAllClin";
			this.checkAllClin.Size = new System.Drawing.Size(160, 16);
			this.checkAllClin.TabIndex = 54;
			this.checkAllClin.Text = "All (Includes hidden)";
			this.checkAllClin.Click += new System.EventHandler(this.checkAllClin_Click);
			// 
			// listClin
			// 
			this.listClin.Location = new System.Drawing.Point(18, 252);
			this.listClin.Name = "listClin";
			this.listClin.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClin.Size = new System.Drawing.Size(160, 199);
			this.listClin.TabIndex = 53;
			this.listClin.Click += new System.EventHandler(this.listClin_Click);
			// 
			// labelClin
			// 
			this.labelClin.Location = new System.Drawing.Point(15, 215);
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
			this.butCancel.Location = new System.Drawing.Point(413, 425);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(332, 425);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormRpPatPortionUncollected
			// 
			this.ClientSize = new System.Drawing.Size(509, 466);
			this.Controls.Add(this.checkAllClin);
			this.Controls.Add(this.listClin);
			this.Controls.Add(this.labelClin);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.date2);
			this.Controls.Add(this.date1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpPatPortionUncollected";
			this.ShowInTaskbar = false;
			this.Text = "Patient Portion Uncollected Report";
			this.Load += new System.EventHandler(this.FormPaymentSheet_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormPaymentSheet_Load(object sender,System.EventArgs e) {
			date1.SelectionStart=DateTime.Today;
			date2.SelectionStart=DateTime.Today;
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
			if(PrefC.HasClinicsEnabled && !checkAllClin.Checked && listClin.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one clinic must be selected.");
				return;
			}
			ReportComplex report=new ReportComplex(true,false);
			List<long> listClinicNums=new List<long>();
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
			DataTable table=RpPatPortionUncollected.GetPatUncollected(date1.SelectionStart,date2.SelectionStart,listClinicNums);
			string subtitleClinics="";
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
			report.ReportName=Lan.g(this,"Patient Portion Uncollected");
			report.AddTitle("Title",Lan.g(this,"Patient Portion Uncollected"),fontTitle);
			report.AddSubTitle("Practice Title",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Dates of Report",date1.SelectionStart.ToString("d")+" - "+date2.SelectionStart.ToString("d"),fontSubTitle);
			if(PrefC.HasClinicsEnabled) {
				report.AddSubTitle("Clinics",subtitleClinics,fontSubTitle);
			}
			
			QueryObject query=report.AddQuery(table,"Patient Portion Uncollected");
			query.AddColumn("Date",90,FieldValueType.Date,font);
			query.GetColumnDetail("Date").StringFormat="d";
			query.AddColumn("Patient Name",150,FieldValueType.String,font);
			query.AddColumn("Procedure",80,FieldValueType.String,font);
			query.AddColumn("Fee",60,FieldValueType.Number,font);
			query.AddColumn("Patient",65,FieldValueType.Number,font);
			query.AddColumn("Adjustment",75,FieldValueType.Number,font);
			query.AddColumn("Patient Paid",90,FieldValueType.Number,font);
			query.AddColumn("Uncollected",80,FieldValueType.Number,font);
			report.AddPageNum(font);
			report.AddGridLines();
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;		
		}
	}
}
