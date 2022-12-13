namespace OpenDental{
	partial class FormScheduledProcesses {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormScheduledProcesses));
			this.butAdd = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butClose = new OpenDental.UI.Button();
			this.groupBoxInsVerifyChecks = new OpenDental.UI.GroupBoxOD();
			this.checkChangeEffectiveDates = new System.Windows.Forms.CheckBox();
			this.checkChangeInsHist = new System.Windows.Forms.CheckBox();
			this.checkCheckDeductable = new System.Windows.Forms.CheckBox();
			this.checkCheckAnnualMax = new System.Windows.Forms.CheckBox();
			this.checkCreateAdjustments = new System.Windows.Forms.CheckBox();
			this.groupBoxInsVerifyChecks.SuspendLayout();
			this.SuspendLayout();
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(567, 29);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 5;
			this.butAdd.Text = "&Add";
			this.butAdd.UseVisualStyleBackColor = true;
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(30, 29);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(521, 500);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = null;
			this.gridMain.TranslationName = "TableScheduledProcess";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.GridMain_CellDoubleClick);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(656, 509);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// groupBoxInsVerifyChecks
			// 
			this.groupBoxInsVerifyChecks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxInsVerifyChecks.Controls.Add(this.checkChangeEffectiveDates);
			this.groupBoxInsVerifyChecks.Controls.Add(this.checkChangeInsHist);
			this.groupBoxInsVerifyChecks.Controls.Add(this.checkCheckDeductable);
			this.groupBoxInsVerifyChecks.Controls.Add(this.checkCheckAnnualMax);
			this.groupBoxInsVerifyChecks.Controls.Add(this.checkCreateAdjustments);
			this.groupBoxInsVerifyChecks.Location = new System.Drawing.Point(560, 193);
			this.groupBoxInsVerifyChecks.Name = "groupBoxInsVerifyChecks";
			this.groupBoxInsVerifyChecks.Size = new System.Drawing.Size(171, 141);
			this.groupBoxInsVerifyChecks.TabIndex = 6;
			this.groupBoxInsVerifyChecks.Text = "Ins Verify Batch Preferences";
			// 
			// checkChangeEffectiveDates
			// 
			this.checkChangeEffectiveDates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkChangeEffectiveDates.Location = new System.Drawing.Point(14, 115);
			this.checkChangeEffectiveDates.Name = "checkChangeEffectiveDates";
			this.checkChangeEffectiveDates.Size = new System.Drawing.Size(140, 17);
			this.checkChangeEffectiveDates.TabIndex = 12;
			this.checkChangeEffectiveDates.Text = "Change Effective Dates";
			this.checkChangeEffectiveDates.UseVisualStyleBackColor = true;
			// 
			// checkChangeInsHist
			// 
			this.checkChangeInsHist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkChangeInsHist.Location = new System.Drawing.Point(14, 92);
			this.checkChangeInsHist.Name = "checkChangeInsHist";
			this.checkChangeInsHist.Size = new System.Drawing.Size(140, 17);
			this.checkChangeInsHist.TabIndex = 11;
			this.checkChangeInsHist.Text = "Change Ins History";
			this.checkChangeInsHist.UseVisualStyleBackColor = true;
			// 
			// checkCheckDeductable
			// 
			this.checkCheckDeductable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkCheckDeductable.Location = new System.Drawing.Point(14, 46);
			this.checkCheckDeductable.Name = "checkCheckDeductable";
			this.checkCheckDeductable.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.checkCheckDeductable.Size = new System.Drawing.Size(140, 17);
			this.checkCheckDeductable.TabIndex = 10;
			this.checkCheckDeductable.Text = "Check Deductible";
			this.checkCheckDeductable.UseVisualStyleBackColor = true;
			// 
			// checkCheckAnnualMax
			// 
			this.checkCheckAnnualMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkCheckAnnualMax.Location = new System.Drawing.Point(14, 69);
			this.checkCheckAnnualMax.Name = "checkCheckAnnualMax";
			this.checkCheckAnnualMax.Size = new System.Drawing.Size(140, 17);
			this.checkCheckAnnualMax.TabIndex = 9;
			this.checkCheckAnnualMax.Text = "Check Annual Max";
			this.checkCheckAnnualMax.UseVisualStyleBackColor = true;
			// 
			// checkCreateAdjustments
			// 
			this.checkCreateAdjustments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkCreateAdjustments.Location = new System.Drawing.Point(14, 23);
			this.checkCreateAdjustments.Name = "checkCreateAdjustments";
			this.checkCreateAdjustments.Size = new System.Drawing.Size(140, 17);
			this.checkCreateAdjustments.TabIndex = 8;
			this.checkCreateAdjustments.Text = "Create Adjustments";
			this.checkCreateAdjustments.UseVisualStyleBackColor = true;
			// 
			// FormScheduledProcesses
			// 
			this.ClientSize = new System.Drawing.Size(743, 545);
			this.Controls.Add(this.groupBoxInsVerifyChecks);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormScheduledProcesses";
			this.Text = "Scheduled Processes";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormScheduledProcesses_FormClosing);
			this.Load += new System.EventHandler(this.FormScheduledProcesses_Load);
			this.groupBoxInsVerifyChecks.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridMain;
		private UI.Button butAdd;
		private UI.GroupBoxOD groupBoxInsVerifyChecks;
		private System.Windows.Forms.CheckBox checkCheckDeductable;
		private System.Windows.Forms.CheckBox checkCheckAnnualMax;
		private System.Windows.Forms.CheckBox checkCreateAdjustments;
		private System.Windows.Forms.CheckBox checkChangeInsHist;
		private System.Windows.Forms.CheckBox checkChangeEffectiveDates;
	}
}