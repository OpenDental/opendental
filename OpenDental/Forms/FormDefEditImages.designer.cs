using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormDefEditImages {
		private System.ComponentModel.IContainer components = null;// Required designer variable.

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDefEditImages));
			this.labelName = new System.Windows.Forms.Label();
			this.textName = new System.Windows.Forms.TextBox();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkHidden = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkAutoSaveForm = new System.Windows.Forms.CheckBox();
			this.checkLabCases = new System.Windows.Forms.CheckBox();
			this.checkClaimAttachments = new System.Windows.Forms.CheckBox();
			this.checkPaymentPlans = new System.Windows.Forms.CheckBox();
			this.checkPatientPortal = new System.Windows.Forms.CheckBox();
			this.checkTreatmentPlans = new System.Windows.Forms.CheckBox();
			this.checkPatientForms = new System.Windows.Forms.CheckBox();
			this.checkToothCharts = new System.Windows.Forms.CheckBox();
			this.checkStatements = new System.Windows.Forms.CheckBox();
			this.checkPatientPictures = new System.Windows.Forms.CheckBox();
			this.checkChartModule = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(47, 12);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(150, 16);
			this.labelName.TabIndex = 0;
			this.labelName.Text = "Name";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(32, 28);
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(178, 20);
			this.textName.TabIndex = 0;
			// 
			// colorDialog1
			// 
			this.colorDialog1.FullOpen = true;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(396, 251);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 25);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(491, 251);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 25);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkHidden
			// 
			this.checkHidden.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHidden.Location = new System.Drawing.Point(449, 26);
			this.checkHidden.Name = "checkHidden";
			this.checkHidden.Size = new System.Drawing.Size(99, 24);
			this.checkHidden.TabIndex = 3;
			this.checkHidden.Text = "Hidden";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkAutoSaveForm);
			this.groupBox1.Controls.Add(this.checkLabCases);
			this.groupBox1.Controls.Add(this.checkClaimAttachments);
			this.groupBox1.Controls.Add(this.checkPaymentPlans);
			this.groupBox1.Controls.Add(this.checkPatientPortal);
			this.groupBox1.Controls.Add(this.checkTreatmentPlans);
			this.groupBox1.Controls.Add(this.checkPatientForms);
			this.groupBox1.Controls.Add(this.checkToothCharts);
			this.groupBox1.Controls.Add(this.checkStatements);
			this.groupBox1.Controls.Add(this.checkPatientPictures);
			this.groupBox1.Controls.Add(this.checkChartModule);
			this.groupBox1.Location = new System.Drawing.Point(228, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(215, 220);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Usage";
			// 
			// checkAutoSaveForm
			// 
			this.checkAutoSaveForm.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAutoSaveForm.Location = new System.Drawing.Point(8, 196);
			this.checkAutoSaveForm.Name = "checkAutoSaveForm";
			this.checkAutoSaveForm.Size = new System.Drawing.Size(183, 18);
			this.checkAutoSaveForm.TabIndex = 15;
			this.checkAutoSaveForm.Text = "Auto-Save Forms  (only one)";
			this.checkAutoSaveForm.CheckedChanged += new System.EventHandler(this.checkAutoSaveForm_CheckedChanged);
			// 
			// checkLabCases
			// 
			this.checkLabCases.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkLabCases.Location = new System.Drawing.Point(8, 178);
			this.checkLabCases.Name = "checkLabCases";
			this.checkLabCases.Size = new System.Drawing.Size(183, 18);
			this.checkLabCases.TabIndex = 14;
			this.checkLabCases.Text = "Lab Cases (only one)";
			this.checkLabCases.CheckedChanged += new System.EventHandler(this.checkLabCases_CheckedChanged);
			// 
			// checkClaimAttachments
			// 
			this.checkClaimAttachments.AutoSize = true;
			this.checkClaimAttachments.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClaimAttachments.Location = new System.Drawing.Point(8, 160);
			this.checkClaimAttachments.Name = "checkClaimAttachments";
			this.checkClaimAttachments.Size = new System.Drawing.Size(168, 18);
			this.checkClaimAttachments.TabIndex = 13;
			this.checkClaimAttachments.Text = "Claim Attachments (only one)";
			this.checkClaimAttachments.CheckedChanged += new System.EventHandler(this.checkClaimAttachments_CheckedChanged);
			// 
			// checkPaymentPlans
			// 
			this.checkPaymentPlans.AutoSize = true;
			this.checkPaymentPlans.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPaymentPlans.Location = new System.Drawing.Point(8, 142);
			this.checkPaymentPlans.Name = "checkPaymentPlans";
			this.checkPaymentPlans.Size = new System.Drawing.Size(151, 18);
			this.checkPaymentPlans.TabIndex = 12;
			this.checkPaymentPlans.Text = "Payment Plans (only one)";
			this.checkPaymentPlans.CheckedChanged += new System.EventHandler(this.checkPaymentPlans_CheckedChanged);
			// 
			// checkPatientPortal
			// 
			this.checkPatientPortal.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPatientPortal.Location = new System.Drawing.Point(8, 52);
			this.checkPatientPortal.Name = "checkPatientPortal";
			this.checkPatientPortal.Size = new System.Drawing.Size(201, 18);
			this.checkPatientPortal.TabIndex = 10;
			this.checkPatientPortal.Text = "Show in Patient Portal";
			// 
			// checkTreatmentPlans
			// 
			this.checkTreatmentPlans.AutoSize = true;
			this.checkTreatmentPlans.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkTreatmentPlans.Location = new System.Drawing.Point(8, 124);
			this.checkTreatmentPlans.Name = "checkTreatmentPlans";
			this.checkTreatmentPlans.Size = new System.Drawing.Size(158, 18);
			this.checkTreatmentPlans.TabIndex = 9;
			this.checkTreatmentPlans.Text = "Treatment Plans (only one)";
			this.checkTreatmentPlans.UseVisualStyleBackColor = true;
			this.checkTreatmentPlans.CheckedChanged += new System.EventHandler(this.checkTreatmentPlans_CheckedChanged);
			// 
			// checkPatientForms
			// 
			this.checkPatientForms.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPatientForms.Location = new System.Drawing.Point(8, 34);
			this.checkPatientForms.Name = "checkPatientForms";
			this.checkPatientForms.Size = new System.Drawing.Size(201, 18);
			this.checkPatientForms.TabIndex = 8;
			this.checkPatientForms.Text = "Show in Patient Forms";
			// 
			// checkToothCharts
			// 
			this.checkToothCharts.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkToothCharts.Location = new System.Drawing.Point(8, 106);
			this.checkToothCharts.Name = "checkToothCharts";
			this.checkToothCharts.Size = new System.Drawing.Size(201, 18);
			this.checkToothCharts.TabIndex = 7;
			this.checkToothCharts.Text = "Graphical Tooth Charts (only one)";
			this.checkToothCharts.CheckedChanged += new System.EventHandler(this.checkToothCharts_CheckedChanged);
			// 
			// checkStatements
			// 
			this.checkStatements.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkStatements.Location = new System.Drawing.Point(8, 88);
			this.checkStatements.Name = "checkStatements";
			this.checkStatements.Size = new System.Drawing.Size(201, 18);
			this.checkStatements.TabIndex = 6;
			this.checkStatements.Text = "Statements (only one)";
			this.checkStatements.CheckedChanged += new System.EventHandler(this.checkStatements_CheckedChanged);
			// 
			// checkPatientPictures
			// 
			this.checkPatientPictures.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPatientPictures.Location = new System.Drawing.Point(8, 70);
			this.checkPatientPictures.Name = "checkPatientPictures";
			this.checkPatientPictures.Size = new System.Drawing.Size(201, 18);
			this.checkPatientPictures.TabIndex = 5;
			this.checkPatientPictures.Text = "Patient Pictures (only one)";
			this.checkPatientPictures.CheckedChanged += new System.EventHandler(this.checkPatientPictures_CheckedChanged);
			// 
			// checkChartModule
			// 
			this.checkChartModule.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkChartModule.Location = new System.Drawing.Point(8, 16);
			this.checkChartModule.Name = "checkChartModule";
			this.checkChartModule.Size = new System.Drawing.Size(201, 18);
			this.checkChartModule.TabIndex = 4;
			this.checkChartModule.Text = "Show in Chart module";
			// 
			// FormDefEditImages
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(578, 288);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.checkHidden);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.labelName);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormDefEditImages";
			this.ShowInTaskbar = false;
			this.Text = "Edit Image Category";
			this.Load += new System.EventHandler(this.FormDefEdit_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkHidden;
		private CheckBox checkToothCharts;
		private CheckBox checkStatements;
		private CheckBox checkPatientPictures;
		private CheckBox checkChartModule;
		private CheckBox checkPatientForms;
		private CheckBox checkTreatmentPlans;
		private CheckBox checkPatientPortal;
		private CheckBox checkPaymentPlans;
		private CheckBox checkClaimAttachments;
		private CheckBox checkLabCases;
		private CheckBox checkAutoSaveForm;
		private GroupBox groupBox1;
	}
}
