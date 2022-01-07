using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpPayPlans {
		private System.ComponentModel.IContainer components = null;

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

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
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
		private CheckBox checkHasDateRange;
		private DateTimePicker dateStart;
		private DateTimePicker dateEnd;
		private Label label2;
		private Label label3;
	}
}
