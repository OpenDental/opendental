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
			this.butSave = new OpenDental.UI.Button();
			this.checkHidden = new OpenDental.UI.CheckBox();
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.checkClaimResponses = new OpenDental.UI.CheckBox();
			this.checkThumbnails = new OpenDental.UI.CheckBox();
			this.checkTaskAttachments = new OpenDental.UI.CheckBox();
			this.checkAutoSaveForm = new OpenDental.UI.CheckBox();
			this.checkLabCases = new OpenDental.UI.CheckBox();
			this.checkClaimAttachments = new OpenDental.UI.CheckBox();
			this.checkPaymentPlans = new OpenDental.UI.CheckBox();
			this.checkPatientPortal = new OpenDental.UI.CheckBox();
			this.checkTreatmentPlans = new OpenDental.UI.CheckBox();
			this.checkPatientForms = new OpenDental.UI.CheckBox();
			this.checkToothCharts = new OpenDental.UI.CheckBox();
			this.checkStatements = new OpenDental.UI.CheckBox();
			this.checkPatientPictures = new OpenDental.UI.CheckBox();
			this.checkChartModule = new OpenDental.UI.CheckBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(38, 12);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(150, 16);
			this.labelName.TabIndex = 0;
			this.labelName.Text = "Name";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(23, 28);
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(178, 20);
			this.textName.TabIndex = 0;
			// 
			// colorDialog1
			// 
			this.colorDialog1.FullOpen = true;
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(535, 322);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 25);
			this.butSave.TabIndex = 4;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// checkHidden
			// 
			this.checkHidden.Location = new System.Drawing.Point(23, 58);
			this.checkHidden.Name = "checkHidden";
			this.checkHidden.Size = new System.Drawing.Size(99, 24);
			this.checkHidden.TabIndex = 3;
			this.checkHidden.Text = "Hidden";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkClaimResponses);
			this.groupBox1.Controls.Add(this.checkThumbnails);
			this.groupBox1.Controls.Add(this.checkTaskAttachments);
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
			this.groupBox1.Location = new System.Drawing.Point(228, 26);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(382, 274);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.Text = "Usage";
			// 
			// checkClaimResponses
			// 
			this.checkClaimResponses.Location = new System.Drawing.Point(8, 250);
			this.checkClaimResponses.Name = "checkClaimResponses";
			this.checkClaimResponses.Size = new System.Drawing.Size(201, 18);
			this.checkClaimResponses.TabIndex = 17;
			this.checkClaimResponses.Text = "(N) Claim Responses (only one)";
			this.checkClaimResponses.CheckedChanged += new System.EventHandler(this.checkClaimResponses_CheckedChanged);
			// 
			// checkThumbnails
			// 
			this.checkThumbnails.Location = new System.Drawing.Point(8, 34);
			this.checkThumbnails.Name = "checkThumbnails";
			this.checkThumbnails.Size = new System.Drawing.Size(370, 18);
			this.checkThumbnails.TabIndex = 17;
			this.checkThumbnails.Text = "(M) Show Thumbnails (usually for radiographs)";
			// 
			// checkTaskAttachments
			// 
			this.checkTaskAttachments.Location = new System.Drawing.Point(8, 232);
			this.checkTaskAttachments.Name = "checkTaskAttachments";
			this.checkTaskAttachments.Size = new System.Drawing.Size(201, 18);
			this.checkTaskAttachments.TabIndex = 16;
			this.checkTaskAttachments.Text = "(Y) Task Attachments  (only one)";
			this.checkTaskAttachments.CheckedChanged += new System.EventHandler(this.checkTaskAttachments_CheckedChanged);
			// 
			// checkAutoSaveForm
			// 
			this.checkAutoSaveForm.Location = new System.Drawing.Point(8, 214);
			this.checkAutoSaveForm.Name = "checkAutoSaveForm";
			this.checkAutoSaveForm.Size = new System.Drawing.Size(183, 18);
			this.checkAutoSaveForm.TabIndex = 15;
			this.checkAutoSaveForm.Text = "(U) Autosave Forms  (only one)";
			this.checkAutoSaveForm.CheckedChanged += new System.EventHandler(this.checkAutoSaveForm_CheckedChanged);
			// 
			// checkLabCases
			// 
			this.checkLabCases.Location = new System.Drawing.Point(8, 196);
			this.checkLabCases.Name = "checkLabCases";
			this.checkLabCases.Size = new System.Drawing.Size(183, 18);
			this.checkLabCases.TabIndex = 14;
			this.checkLabCases.Text = "(B) Lab Cases (only one)";
			this.checkLabCases.CheckedChanged += new System.EventHandler(this.checkLabCases_CheckedChanged);
			// 
			// checkClaimAttachments
			// 
			this.checkClaimAttachments.Location = new System.Drawing.Point(8, 178);
			this.checkClaimAttachments.Name = "checkClaimAttachments";
			this.checkClaimAttachments.Size = new System.Drawing.Size(190, 18);
			this.checkClaimAttachments.TabIndex = 13;
			this.checkClaimAttachments.Text = "(C) Claim Attachments (only one)";
			this.checkClaimAttachments.CheckedChanged += new System.EventHandler(this.checkClaimAttachments_CheckedChanged);
			// 
			// checkPaymentPlans
			// 
			this.checkPaymentPlans.Location = new System.Drawing.Point(8, 160);
			this.checkPaymentPlans.Name = "checkPaymentPlans";
			this.checkPaymentPlans.Size = new System.Drawing.Size(173, 18);
			this.checkPaymentPlans.TabIndex = 12;
			this.checkPaymentPlans.Text = "(A) Payment Plans (only one)";
			this.checkPaymentPlans.CheckedChanged += new System.EventHandler(this.checkPaymentPlans_CheckedChanged);
			// 
			// checkPatientPortal
			// 
			this.checkPatientPortal.Location = new System.Drawing.Point(8, 70);
			this.checkPatientPortal.Name = "checkPatientPortal";
			this.checkPatientPortal.Size = new System.Drawing.Size(201, 18);
			this.checkPatientPortal.TabIndex = 10;
			this.checkPatientPortal.Text = "(L) Show in Patient Portal";
			// 
			// checkTreatmentPlans
			// 
			this.checkTreatmentPlans.Location = new System.Drawing.Point(8, 142);
			this.checkTreatmentPlans.Name = "checkTreatmentPlans";
			this.checkTreatmentPlans.Size = new System.Drawing.Size(201, 18);
			this.checkTreatmentPlans.TabIndex = 9;
			this.checkTreatmentPlans.Text = "(R) Treatment Plans (only one)";
			this.checkTreatmentPlans.CheckedChanged += new System.EventHandler(this.checkTreatmentPlans_CheckedChanged);
			// 
			// checkPatientForms
			// 
			this.checkPatientForms.Location = new System.Drawing.Point(8, 52);
			this.checkPatientForms.Name = "checkPatientForms";
			this.checkPatientForms.Size = new System.Drawing.Size(370, 18);
			this.checkPatientForms.TabIndex = 8;
			this.checkPatientForms.Text = "(F) Show these documents in Patient Forms window";
			// 
			// checkToothCharts
			// 
			this.checkToothCharts.Location = new System.Drawing.Point(8, 124);
			this.checkToothCharts.Name = "checkToothCharts";
			this.checkToothCharts.Size = new System.Drawing.Size(216, 18);
			this.checkToothCharts.TabIndex = 7;
			this.checkToothCharts.Text = "(T) Graphical Tooth Charts (only one)";
			this.checkToothCharts.CheckedChanged += new System.EventHandler(this.checkToothCharts_CheckedChanged);
			// 
			// checkStatements
			// 
			this.checkStatements.Location = new System.Drawing.Point(8, 106);
			this.checkStatements.Name = "checkStatements";
			this.checkStatements.Size = new System.Drawing.Size(201, 18);
			this.checkStatements.TabIndex = 6;
			this.checkStatements.Text = "(S) Statements (only one)";
			this.checkStatements.CheckedChanged += new System.EventHandler(this.checkStatements_CheckedChanged);
			// 
			// checkPatientPictures
			// 
			this.checkPatientPictures.Location = new System.Drawing.Point(8, 88);
			this.checkPatientPictures.Name = "checkPatientPictures";
			this.checkPatientPictures.Size = new System.Drawing.Size(201, 18);
			this.checkPatientPictures.TabIndex = 5;
			this.checkPatientPictures.Text = "(P) Patient Pictures (only one)";
			this.checkPatientPictures.CheckedChanged += new System.EventHandler(this.checkPatientPictures_CheckedChanged);
			// 
			// checkChartModule
			// 
			this.checkChartModule.Location = new System.Drawing.Point(8, 16);
			this.checkChartModule.Name = "checkChartModule";
			this.checkChartModule.Size = new System.Drawing.Size(370, 18);
			this.checkChartModule.TabIndex = 4;
			this.checkChartModule.Text = "(X) Show in Chart module (usually for radiographs)";
			// 
			// FormDefEditImages
			// 
			this.AcceptButton = this.butSave;
			this.ClientSize = new System.Drawing.Size(644, 359);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.checkHidden);
			this.Controls.Add(this.butSave);
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
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private OpenDental.UI.Button butSave;
		private OpenDental.UI.CheckBox checkHidden;
		private OpenDental.UI.CheckBox checkToothCharts;
		private OpenDental.UI.CheckBox checkStatements;
		private OpenDental.UI.CheckBox checkPatientPictures;
		private OpenDental.UI.CheckBox checkChartModule;
		private OpenDental.UI.CheckBox checkPatientForms;
		private OpenDental.UI.CheckBox checkTreatmentPlans;
		private OpenDental.UI.CheckBox checkPatientPortal;
		private OpenDental.UI.CheckBox checkPaymentPlans;
		private OpenDental.UI.CheckBox checkClaimAttachments;
		private OpenDental.UI.CheckBox checkLabCases;
		private OpenDental.UI.CheckBox checkAutoSaveForm;
		private OpenDental.UI.GroupBox groupBox1;
		private OpenDental.UI.CheckBox checkTaskAttachments;
		private OpenDental.UI.CheckBox checkThumbnails;
		private UI.CheckBox checkClaimResponses;
	}
}
