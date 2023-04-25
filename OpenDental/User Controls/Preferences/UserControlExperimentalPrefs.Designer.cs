
namespace OpenDental {
	partial class UserControlExperimentalPrefs {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlExperimentalPrefs));
			this.labelCommLogAutoSaveDetails = new System.Windows.Forms.Label();
			this.butAgingProcLifoDetails = new OpenDental.UI.Button();
			this.checkAgingProcLifo = new OpenDental.UI.CheckBox();
			this.SuspendLayout();
			// 
			// labelCommLogAutoSaveDetails
			// 
			this.labelCommLogAutoSaveDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelCommLogAutoSaveDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelCommLogAutoSaveDetails.Location = new System.Drawing.Point(25, 20);
			this.labelCommLogAutoSaveDetails.Name = "labelCommLogAutoSaveDetails";
			this.labelCommLogAutoSaveDetails.Size = new System.Drawing.Size(692, 65);
			this.labelCommLogAutoSaveDetails.TabIndex = 322;
			this.labelCommLogAutoSaveDetails.Text = resources.GetString("labelCommLogAutoSaveDetails.Text");
			// 
			// butAgingProcLifoDetails
			// 
			this.butAgingProcLifoDetails.ForeColor = System.Drawing.Color.Black;
			this.butAgingProcLifoDetails.Location = new System.Drawing.Point(479, 107);
			this.butAgingProcLifoDetails.Name = "butAgingProcLifoDetails";
			this.butAgingProcLifoDetails.Size = new System.Drawing.Size(64, 21);
			this.butAgingProcLifoDetails.TabIndex = 371;
			this.butAgingProcLifoDetails.Text = "Details";
			this.butAgingProcLifoDetails.Click += new System.EventHandler(this.butAgingProcLifoDetails_Click);
			// 
			// checkAgingProcLifo
			// 
			this.checkAgingProcLifo.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAgingProcLifo.Location = new System.Drawing.Point(25, 110);
			this.checkAgingProcLifo.Name = "checkAgingProcLifo";
			this.checkAgingProcLifo.Size = new System.Drawing.Size(435, 17);
			this.checkAgingProcLifo.TabIndex = 323;
			this.checkAgingProcLifo.Text = "Transactions attached to a procedure offset each other before aging";
			this.checkAgingProcLifo.ThreeState = true;
			// 
			// UserControlExperimentalPrefs
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.butAgingProcLifoDetails);
			this.Controls.Add(this.checkAgingProcLifo);
			this.Controls.Add(this.labelCommLogAutoSaveDetails);
			this.Name = "UserControlExperimentalPrefs";
			this.Size = new System.Drawing.Size(974, 624);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelCommLogAutoSaveDetails;
		private OpenDental.UI.CheckBox checkAgingProcLifo;
		private UI.Button butAgingProcLifoDetails;
	}
}
