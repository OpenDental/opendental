using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormProcTools {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcTools));
			this.checkAutocodes = new System.Windows.Forms.CheckBox();
			this.checkTcodes = new System.Windows.Forms.CheckBox();
			this.checkDcodes = new System.Windows.Forms.CheckBox();
			this.checkNcodes = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.checkProcButtons = new System.Windows.Forms.CheckBox();
			this.checkApptProcsQuickAdd = new System.Windows.Forms.CheckBox();
			this.checkRecallTypes = new System.Windows.Forms.CheckBox();
			this.butRun = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.labelLineOne = new System.Windows.Forms.Label();
			this.labelLineTwo = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// checkAutocodes
			// 
			this.checkAutocodes.Location = new System.Drawing.Point(15, 213);
			this.checkAutocodes.Name = "checkAutocodes";
			this.checkAutocodes.Size = new System.Drawing.Size(646, 36);
			this.checkAutocodes.TabIndex = 43;
			this.checkAutocodes.Text = "Autocodes - Deletes all current autocodes and then adds the default autocodes.  P" +
    "rocedure codes must have already been entered or they cannot be added as an auto" +
    "code.";
			this.checkAutocodes.UseVisualStyleBackColor = true;
			// 
			// checkTcodes
			// 
			this.checkTcodes.Location = new System.Drawing.Point(15, 66);
			this.checkTcodes.Name = "checkTcodes";
			this.checkTcodes.Size = new System.Drawing.Size(646, 36);
			this.checkTcodes.TabIndex = 44;
			this.checkTcodes.Text = "T codes - Remove temp codes, codes that start with \"T\", which were only needed fo" +
    "r the trial version.  If a T code has already been used, then this moves it to t" +
    "he obsolete category.";
			this.checkTcodes.UseVisualStyleBackColor = true;
			// 
			// checkDcodes
			// 
			this.checkDcodes.Checked = true;
			this.checkDcodes.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkDcodes.Location = new System.Drawing.Point(15, 141);
			this.checkDcodes.Name = "checkDcodes";
			this.checkDcodes.Size = new System.Drawing.Size(646, 36);
			this.checkDcodes.TabIndex = 45;
			this.checkDcodes.Text = "D codes - Add any missing 2022 ADA codes and fix descriptions and blank abbreviat" +
    "ions of existing codes.  This option does not work in the trial version or compi" +
    "led version.";
			this.checkDcodes.UseVisualStyleBackColor = true;
			// 
			// checkNcodes
			// 
			this.checkNcodes.Location = new System.Drawing.Point(15, 108);
			this.checkNcodes.Name = "checkNcodes";
			this.checkNcodes.Size = new System.Drawing.Size(646, 36);
			this.checkNcodes.TabIndex = 46;
			this.checkNcodes.Text = "N codes - Add any missing no-fee codes.";
			this.checkNcodes.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(12, 9);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(666, 54);
			this.label5.TabIndex = 48;
			this.label5.Text = resources.GetString("label5.Text");
			// 
			// checkProcButtons
			// 
			this.checkProcButtons.Location = new System.Drawing.Point(15, 255);
			this.checkProcButtons.Name = "checkProcButtons";
			this.checkProcButtons.Size = new System.Drawing.Size(646, 36);
			this.checkProcButtons.TabIndex = 49;
			this.checkProcButtons.Text = resources.GetString("checkProcButtons.Text");
			this.checkProcButtons.UseVisualStyleBackColor = true;
			// 
			// checkApptProcsQuickAdd
			// 
			this.checkApptProcsQuickAdd.Location = new System.Drawing.Point(15, 297);
			this.checkApptProcsQuickAdd.Name = "checkApptProcsQuickAdd";
			this.checkApptProcsQuickAdd.Size = new System.Drawing.Size(646, 36);
			this.checkApptProcsQuickAdd.TabIndex = 51;
			this.checkApptProcsQuickAdd.Text = "Appt Procs Quick Add - This is the list of procedures that you pick from within t" +
    "he appt edit window.  This resets the list to default.";
			this.checkApptProcsQuickAdd.UseVisualStyleBackColor = true;
			// 
			// checkRecallTypes
			// 
			this.checkRecallTypes.Location = new System.Drawing.Point(15, 339);
			this.checkRecallTypes.Name = "checkRecallTypes";
			this.checkRecallTypes.Size = new System.Drawing.Size(646, 36);
			this.checkRecallTypes.TabIndex = 52;
			this.checkRecallTypes.Text = "Recall Types - Resets the recall types and triggers to default.  Replaces any T c" +
    "odes with D codes.";
			this.checkRecallTypes.UseVisualStyleBackColor = true;
			// 
			// butRun
			// 
			this.butRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRun.Location = new System.Drawing.Point(477, 381);
			this.butRun.Name = "butRun";
			this.butRun.Size = new System.Drawing.Size(82, 26);
			this.butRun.TabIndex = 50;
			this.butRun.Text = "Run Now";
			this.butRun.Click += new System.EventHandler(this.butRun_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(586, 381);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(82, 26);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// labelLineOne
			// 
			this.labelLineOne.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelLineOne.Location = new System.Drawing.Point(15, 185);
			this.labelLineOne.Name = "labelLineOne";
			this.labelLineOne.Size = new System.Drawing.Size(670, 2);
			this.labelLineOne.TabIndex = 53;
			// 
			// labelLineTwo
			// 
			this.labelLineTwo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelLineTwo.Location = new System.Drawing.Point(15, 200);
			this.labelLineTwo.Name = "labelLineTwo";
			this.labelLineTwo.Size = new System.Drawing.Size(670, 2);
			this.labelLineTwo.TabIndex = 54;
			// 
			// FormProcTools
			// 
			this.ClientSize = new System.Drawing.Size(698, 431);
			this.Controls.Add(this.labelLineTwo);
			this.Controls.Add(this.labelLineOne);
			this.Controls.Add(this.checkRecallTypes);
			this.Controls.Add(this.checkApptProcsQuickAdd);
			this.Controls.Add(this.butRun);
			this.Controls.Add(this.checkProcButtons);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.checkNcodes);
			this.Controls.Add(this.checkDcodes);
			this.Controls.Add(this.checkTcodes);
			this.Controls.Add(this.checkAutocodes);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormProcTools";
			this.ShowInTaskbar = false;
			this.Text = "Procedure Code Tools";
			this.Load += new System.EventHandler(this.FormProcTools_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private CheckBox checkAutocodes;
		private CheckBox checkTcodes;
		private CheckBox checkDcodes;
		private CheckBox checkNcodes;
		private Label label5;
		private CheckBox checkProcButtons;
		private OpenDental.UI.Button butRun;
		private CheckBox checkApptProcsQuickAdd;
		private CheckBox checkRecallTypes;
		private Label labelLineOne;
		private Label labelLineTwo;
	}
}
