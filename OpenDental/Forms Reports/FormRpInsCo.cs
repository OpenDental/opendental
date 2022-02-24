using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.ReportingComplex;
using System.Data;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public class FormRpInsCo : FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label labelPatName;
		private System.Windows.Forms.TextBox textBoxCarrier;
		private System.ComponentModel.Container components = null;
		private FormQuery FormQuery2;
		private string carrier;

		///<summary></summary>
		public FormRpInsCo(){
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpInsCo));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.labelPatName = new System.Windows.Forms.Label();
			this.textBoxCarrier = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(490,173);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,26);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(490,138);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelPatName
			// 
			this.labelPatName.Location = new System.Drawing.Point(24,7);
			this.labelPatName.Name = "labelPatName";
			this.labelPatName.Size = new System.Drawing.Size(256,70);
			this.labelPatName.TabIndex = 37;
			this.labelPatName.Text = "Enter the first few letters of the Insurance Carrier name, or leave blank to view" +
    " all carriers:";
			this.labelPatName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxCarrier
			// 
			this.textBoxCarrier.Location = new System.Drawing.Point(281,33);
			this.textBoxCarrier.Name = "textBoxCarrier";
			this.textBoxCarrier.Size = new System.Drawing.Size(100,20);
			this.textBoxCarrier.TabIndex = 0;
			// 
			// FormRpInsCo
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(591,229);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelPatName);
			this.Controls.Add(this.textBoxCarrier);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpInsCo";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Insurance Plan Report";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void butOK_Click(object sender, System.EventArgs e) {
			carrier= PIn.String(textBoxCarrier.Text);
			ReportComplex report=new ReportComplex(true,false);
			DataTable table=RpInsCo.GetInsCoTable(carrier);
			Font fontMain=new Font("Tahoma",8);
			Font fontTitle=new Font("Tahoma",15,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Insurance Plan List");
			report.AddTitle("Title",Lan.g(this,"Insurance Plan List"),fontTitle);
			report.AddSubTitle("PracticeTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			QueryObject query=report.AddQuery(table,Lan.g(this,"Date")+": "+DateTime.Today.ToString("d"));
			query.AddColumn("Carrier Name",230,font:fontMain);
			query.AddColumn("Subscriber Name",175,font:fontMain);
			query.AddColumn("Carrier Phone#",175,font:fontMain);
			query.AddColumn("Group Name",165,font:fontMain);
			report.AddPageNum(fontMain);
			if(!report.SubmitQueries()) {
				return;
			}
			report.AddFooterText("Total","Total: "+report.TotalRows.ToString(),fontMain,10,ContentAlignment.MiddleRight);
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;		
		}
	}
}


















