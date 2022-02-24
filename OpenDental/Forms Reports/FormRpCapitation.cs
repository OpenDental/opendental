using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDentBusiness;
using System.Data;

namespace OpenDental{
	///<summary></summary>
	public class FormRpCapitation : FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private ValidDate textDateStart;
		private Label label2;
		private ValidDate textDateEnd;
		private Label label1;
		public TextBox textCarrier;
		private Label label3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		///<summary></summary>
		public FormRpCapitation()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.C("All", new System.Windows.Forms.Control[] {
				butOK,
				butCancel,
			});
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpCapitation));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.textDateStart = new OpenDental.ValidDate();
			this.label2 = new System.Windows.Forms.Label();
			this.textDateEnd = new OpenDental.ValidDate();
			this.label1 = new System.Windows.Forms.Label();
			this.textCarrier = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(518, 112);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(427, 112);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(308, 45);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(100, 20);
			this.textDateStart.TabIndex = 45;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(222, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 18);
			this.label2.TabIndex = 44;
			this.label2.Text = "From Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDateEnd
			// 
			this.textDateEnd.Location = new System.Drawing.Point(308, 71);
			this.textDateEnd.Name = "textDateEnd";
			this.textDateEnd.Size = new System.Drawing.Size(100, 20);
			this.textDateEnd.TabIndex = 47;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(222, 73);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 18);
			this.label1.TabIndex = 46;
			this.label1.Text = "To Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textCarrier
			// 
			this.textCarrier.Location = new System.Drawing.Point(308, 16);
			this.textCarrier.Name = "textCarrier";
			this.textCarrier.Size = new System.Drawing.Size(285, 20);
			this.textCarrier.TabIndex = 48;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7, 19);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(295, 13);
			this.label3.TabIndex = 49;
			this.label3.Text = "Enter a few letters of the name of the insurance carrier";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormRpCapitation
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(605, 150);
			this.Controls.Add(this.textCarrier);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDateEnd);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textDateStart);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpCapitation";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Capitation Utilization Report";
			this.Load += new System.EventHandler(this.FormRpCapitation_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormRpCapitation_Load(object sender, System.EventArgs e) {
			DateTime today = DateTime.Today;
			DateTime endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
			textDateStart.Text=new DateTime(today.Year,today.Month,1).ToShortDateString();
			textDateEnd.Text=endOfMonth.ToShortDateString();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			ExecuteReport();
		}

		private void ExecuteReport(){
			DateTime dateStart;
			DateTime dateEnd;
			bool isMedOrClinic;
			if(!DateTime.TryParse(textDateStart.Text,out dateStart)) {
				MsgBox.Show(this,"Please input a valid date.");
				return;
			}
			if(!DateTime.TryParse(textDateEnd.Text,out dateEnd)) {
				MsgBox.Show(this,"Please input a valid date.");
				return;
			}
			if(String.IsNullOrWhiteSpace(textCarrier.Text)) {
				MsgBox.Show(this,"Carrier can not be blank. Please input a value for carrier.");
				return;
			}
			ReportComplex report=new ReportComplex(true,true);
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				isMedOrClinic=true;
			}
			else {
				isMedOrClinic=false;
			}
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.AddTitle("Title",Lan.g(this,"Capitation Utilization"),fontTitle);
			report.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Date",textDateStart.Text+" - "+textDateEnd.Text,fontSubTitle);
			DataTable table=RpCapitation.GetCapitationTable(dateStart,dateEnd,textCarrier.Text,isMedOrClinic);
			QueryObject query=report.AddQuery(table,"","",SplitByKind.None,1,true);
			query.AddColumn("Carrier",150,FieldValueType.String,font);
			query.GetColumnDetail("Carrier").SuppressIfDuplicate=true;
			query.AddColumn("Subscriber",120,FieldValueType.String,font);
			query.GetColumnDetail("Subscriber").SuppressIfDuplicate=true;
			query.AddColumn("Subsc SSN",70,FieldValueType.String,font);
			query.GetColumnDetail("Subsc SSN").SuppressIfDuplicate=true;
			query.AddColumn("Patient",120,FieldValueType.String,font);
			query.AddColumn("Pat DOB",80,FieldValueType.Date,font);
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				query.AddColumn("Code",140,FieldValueType.String,font);
				query.AddColumn("Proc Description",120,FieldValueType.String,font);
				query.AddColumn("Date",80,FieldValueType.Date,font);
				query.AddColumn("UCR Fee",60,FieldValueType.Number,font);
				query.AddColumn("Co-Pay",60,FieldValueType.Number,font);
			}
			else {
				query.AddColumn("Code",50,FieldValueType.String,font);
				query.AddColumn("Proc Description",120,FieldValueType.String,font);
				query.AddColumn("Tth",30,FieldValueType.String,font);
				query.AddColumn("Surf",40,FieldValueType.String,font);
				query.AddColumn("Date",80,FieldValueType.Date,font);
				query.AddColumn("UCR Fee",70,FieldValueType.Number,font);
				query.AddColumn("Co-Pay",70,FieldValueType.Number,font);
			}
			if(!report.SubmitQueries()) {
				//DialogResult=DialogResult.Cancel;
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




















