using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDentBusiness;
using CodeBase;
using System.Linq;

namespace OpenDental{
///<summary></summary>
	public class FormRpInsOverpaid:FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label label1;
		private RadioButton radioGroupByProc;
		private RadioButton radioGroupByPat;
		private CheckBox checkAllClin;
		private OpenDental.UI.ListBoxOD listClin;
		private Label labelClin;
		private System.ComponentModel.Container components = null;
		private MonthCalendar dateEnd;
		private MonthCalendar dateStart;
		private List<Clinic> _listClinics;

		///<summary></summary>
		public FormRpInsOverpaid() {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpInsOverpaid));
            this.butCancel = new OpenDental.UI.Button();
            this.butOK = new OpenDental.UI.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.radioGroupByProc = new System.Windows.Forms.RadioButton();
            this.radioGroupByPat = new System.Windows.Forms.RadioButton();
            this.checkAllClin = new System.Windows.Forms.CheckBox();
            this.listClin = new OpenDental.UI.ListBoxOD();
            this.labelClin = new System.Windows.Forms.Label();
            this.dateEnd = new System.Windows.Forms.MonthCalendar();
            this.dateStart = new System.Windows.Forms.MonthCalendar();
            this.SuspendLayout();
            // 
            // butCancel
            // 
            this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butCancel.Location = new System.Drawing.Point(445, 454);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(75, 26);
            this.butCancel.TabIndex = 19;
            this.butCancel.Text = "&Cancel";
            // 
            // butOK
            // 
            this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butOK.Location = new System.Drawing.Point(349, 454);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(75, 26);
            this.butOK.TabIndex = 18;
            this.butOK.Text = "&OK";
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(21, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(501, 38);
            this.label1.TabIndex = 20;
            this.label1.Text = "Helps find situations where the insurance payment plus any writeoff exceeds the f" +
    "ee for procedures in the date range.  See the manual for suggestions on how to h" +
    "andle the results.";
            // 
            // radioGroupByProc
            // 
            this.radioGroupByProc.Checked = true;
            this.radioGroupByProc.Location = new System.Drawing.Point(255, 268);
            this.radioGroupByProc.Name = "radioGroupByProc";
            this.radioGroupByProc.Size = new System.Drawing.Size(194, 17);
            this.radioGroupByProc.TabIndex = 21;
            this.radioGroupByProc.TabStop = true;
            this.radioGroupByProc.Text = "Filter results by procedure (default)";
            this.radioGroupByProc.UseVisualStyleBackColor = true;
            // 
            // radioGroupByPat
            // 
            this.radioGroupByPat.Location = new System.Drawing.Point(255, 291);
            this.radioGroupByPat.Name = "radioGroupByPat";
            this.radioGroupByPat.Size = new System.Drawing.Size(160, 47);
            this.radioGroupByPat.TabIndex = 21;
            this.radioGroupByPat.Text = "Filter results by patient and date (will show different results, see manual)";
            this.radioGroupByPat.UseVisualStyleBackColor = true;
            // 
            // checkAllClin
            // 
            this.checkAllClin.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkAllClin.Location = new System.Drawing.Point(82, 249);
            this.checkAllClin.Name = "checkAllClin";
            this.checkAllClin.Size = new System.Drawing.Size(154, 16);
            this.checkAllClin.TabIndex = 57;
            this.checkAllClin.Text = "All (Includes hidden)";
            this.checkAllClin.Click += new System.EventHandler(this.checkAllClin_Click);
            // 
            // listClin
            // 
            this.listClin.Location = new System.Drawing.Point(82, 268);
            this.listClin.Name = "listClin";
            this.listClin.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
            this.listClin.Size = new System.Drawing.Size(154, 212);
            this.listClin.TabIndex = 56;
            this.listClin.Click += new System.EventHandler(this.listClin_Click);
            // 
            // labelClin
            // 
            this.labelClin.Location = new System.Drawing.Point(79, 231);
            this.labelClin.Name = "labelClin";
            this.labelClin.Size = new System.Drawing.Size(104, 16);
            this.labelClin.TabIndex = 55;
            this.labelClin.Text = "Clinics";
            this.labelClin.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // dateEnd
            // 
            this.dateEnd.Location = new System.Drawing.Point(293, 60);
            this.dateEnd.Name = "dateEnd";
            this.dateEnd.TabIndex = 59;
            // 
            // dateStart
            // 
            this.dateStart.Location = new System.Drawing.Point(26, 60);
            this.dateStart.Name = "dateStart";
            this.dateStart.TabIndex = 58;
            // 
            // FormRpInsOverpaid
            // 
            this.AcceptButton = this.butOK;
            this.CancelButton = this.butCancel;
            this.ClientSize = new System.Drawing.Size(545, 496);
            this.Controls.Add(this.dateEnd);
            this.Controls.Add(this.dateStart);
            this.Controls.Add(this.checkAllClin);
            this.Controls.Add(this.listClin);
            this.Controls.Add(this.labelClin);
            this.Controls.Add(this.radioGroupByPat);
            this.Controls.Add(this.radioGroupByProc);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormRpInsOverpaid";
            this.ShowInTaskbar = false;
            this.Text = "Insurance Overpaid Report";
            this.Load += new System.EventHandler(this.FormRpInsOverpaid_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void FormRpInsOverpaid_Load(object sender, System.EventArgs e) {
			dateStart.SelectionStart=DateTime.Today.AddMonths(-1);
			dateStart.SelectionEnd=DateTime.Today.AddMonths(-1);
			dateEnd.SelectionStart=DateTime.Today;
			dateEnd.SelectionEnd=DateTime.Today;
			if(PrefC.HasClinicsEnabled) {
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
						listClin.ClearSelected();
						listClin.SetSelected(listClin.Items.Count-1);
					}
				}
			}
			else {
				listClin.Visible=false;
				labelClin.Visible=false;
				checkAllClin.Visible=false;
				//Adjust the location of the window size to make up for the clinic list being invisible
				this.Height=412;
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

		private void butOK_Click(object sender, System.EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				if(!checkAllClin.Checked && listClin.SelectedIndices.Count==0) {
					MsgBox.Show(this,"At least one clinic must be selected.");
					return;
				}
			}
			ReportComplex report=new ReportComplex(true,false);
			Cursor=Cursors.WaitCursor;
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
			DataTable tableOverpaid=RpInsOverpaid.GetInsuranceOverpaid(dateStart.SelectionStart,dateEnd.SelectionStart,listClinicNums,
				radioGroupByProc.Checked);
			Cursor=Cursors.Default;
			string subtitleClinics="";
			if(PrefC.HasClinicsEnabled) {
				if(checkAllClin.Checked) {
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
			report.ReportName=Lan.g(this,"Insurance Overpaid");
			report.AddTitle("Title",Lan.g(this,"Insurance Overpaid"));
			report.AddSubTitle("Practice Name",PrefC.GetString(PrefName.PracticeTitle));
			if(PrefC.HasClinicsEnabled) {
				report.AddSubTitle("Clinics",subtitleClinics);
			}
			QueryObject query=report.AddQuery(tableOverpaid,DateTime.Today.ToShortDateString());
			query.AddColumn("Pat Name",200,FieldValueType.String);
			query.AddColumn("Date",90,FieldValueType.Date);
			query.GetColumnDetail("Date").StringFormat="d";
			query.AddColumn("Fee",100,FieldValueType.Number);
			query.AddColumn("InsPaid+W/O",120,FieldValueType.Number);
			report.AddPageNum();
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}

	}
}
