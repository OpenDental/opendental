namespace OpenDental{
	partial class FormBillingTypeMerge {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBillingTypeMerge));
			this.butMerge = new OpenDental.UI.Button();
			this.groupBoxInto = new OpenDental.UI.GroupBox();
			this.textItemValueInto = new System.Windows.Forms.TextBox();
			this.labelItemValueInto = new System.Windows.Forms.Label();
			this.textNameInto = new System.Windows.Forms.TextBox();
			this.labelNameInto = new System.Windows.Forms.Label();
			this.butChangeInto = new OpenDental.UI.Button();
			this.textDefNumInto = new System.Windows.Forms.TextBox();
			this.labelDefNumInto = new System.Windows.Forms.Label();
			this.groupBoxFrom = new OpenDental.UI.GroupBox();
			this.textItemValueFrom = new System.Windows.Forms.TextBox();
			this.labelItemValueFrom = new System.Windows.Forms.Label();
			this.textNameFrom = new System.Windows.Forms.TextBox();
			this.labelNameFrom = new System.Windows.Forms.Label();
			this.butChangeFrom = new OpenDental.UI.Button();
			this.textDefNumFrom = new System.Windows.Forms.TextBox();
			this.labelDefNumFrom = new System.Windows.Forms.Label();
			this.groupBoxInto.SuspendLayout();
			this.groupBoxFrom.SuspendLayout();
			this.SuspendLayout();
			// 
			// butMerge
			// 
			this.butMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butMerge.Enabled = false;
			this.butMerge.Location = new System.Drawing.Point(510, 202);
			this.butMerge.Name = "butMerge";
			this.butMerge.Size = new System.Drawing.Size(75, 24);
			this.butMerge.TabIndex = 2;
			this.butMerge.Text = "Merge";
			this.butMerge.Click += new System.EventHandler(this.butMerge_Click);
			// 
			// groupBoxInto
			// 
			this.groupBoxInto.BackColor = System.Drawing.Color.White;
			this.groupBoxInto.Controls.Add(this.textItemValueInto);
			this.groupBoxInto.Controls.Add(this.labelItemValueInto);
			this.groupBoxInto.Controls.Add(this.textNameInto);
			this.groupBoxInto.Controls.Add(this.labelNameInto);
			this.groupBoxInto.Controls.Add(this.butChangeInto);
			this.groupBoxInto.Controls.Add(this.textDefNumInto);
			this.groupBoxInto.Controls.Add(this.labelDefNumInto);
			this.groupBoxInto.Location = new System.Drawing.Point(13, 12);
			this.groupBoxInto.Name = "groupBoxInto";
			this.groupBoxInto.Size = new System.Drawing.Size(572, 77);
			this.groupBoxInto.TabIndex = 0;
			this.groupBoxInto.Text = "Billing Type to merge into. The Billing Type below will be merged into this Billi" +
    "ng Type.";
			// 
			// textItemValueInto
			// 
			this.textItemValueInto.Location = new System.Drawing.Point(368, 37);
			this.textItemValueInto.Name = "textItemValueInto";
			this.textItemValueInto.ReadOnly = true;
			this.textItemValueInto.Size = new System.Drawing.Size(112, 20);
			this.textItemValueInto.TabIndex = 2;
			// 
			// labelItemValueInto
			// 
			this.labelItemValueInto.AutoSize = true;
			this.labelItemValueInto.Location = new System.Drawing.Point(365, 21);
			this.labelItemValueInto.Name = "labelItemValueInto";
			this.labelItemValueInto.Size = new System.Drawing.Size(49, 13);
			this.labelItemValueInto.TabIndex = 5;
			this.labelItemValueInto.Text = "Behavior";
			// 
			// textNameInto
			// 
			this.textNameInto.Location = new System.Drawing.Point(127, 37);
			this.textNameInto.Name = "textNameInto";
			this.textNameInto.ReadOnly = true;
			this.textNameInto.Size = new System.Drawing.Size(237, 20);
			this.textNameInto.TabIndex = 1;
			// 
			// labelNameInto
			// 
			this.labelNameInto.AutoSize = true;
			this.labelNameInto.Location = new System.Drawing.Point(124, 21);
			this.labelNameInto.Name = "labelNameInto";
			this.labelNameInto.Size = new System.Drawing.Size(35, 13);
			this.labelNameInto.TabIndex = 3;
			this.labelNameInto.Text = "Name";
			// 
			// butChangeInto
			// 
			this.butChangeInto.Location = new System.Drawing.Point(486, 33);
			this.butChangeInto.Name = "butChangeInto";
			this.butChangeInto.Size = new System.Drawing.Size(75, 24);
			this.butChangeInto.TabIndex = 3;
			this.butChangeInto.Text = "Change";
			this.butChangeInto.UseVisualStyleBackColor = true;
			this.butChangeInto.Click += new System.EventHandler(this.butChangeInto_Click);
			// 
			// textDefNumInto
			// 
			this.textDefNumInto.Location = new System.Drawing.Point(7, 37);
			this.textDefNumInto.Name = "textDefNumInto";
			this.textDefNumInto.ReadOnly = true;
			this.textDefNumInto.Size = new System.Drawing.Size(117, 20);
			this.textDefNumInto.TabIndex = 0;
			// 
			// labelDefNumInto
			// 
			this.labelDefNumInto.AutoSize = true;
			this.labelDefNumInto.Location = new System.Drawing.Point(4, 21);
			this.labelDefNumInto.Name = "labelDefNumInto";
			this.labelDefNumInto.Size = new System.Drawing.Size(46, 13);
			this.labelDefNumInto.TabIndex = 0;
			this.labelDefNumInto.Text = "DefNum";
			// 
			// groupBoxFrom
			// 
			this.groupBoxFrom.BackColor = System.Drawing.Color.White;
			this.groupBoxFrom.Controls.Add(this.textItemValueFrom);
			this.groupBoxFrom.Controls.Add(this.labelItemValueFrom);
			this.groupBoxFrom.Controls.Add(this.textNameFrom);
			this.groupBoxFrom.Controls.Add(this.labelNameFrom);
			this.groupBoxFrom.Controls.Add(this.butChangeFrom);
			this.groupBoxFrom.Controls.Add(this.textDefNumFrom);
			this.groupBoxFrom.Controls.Add(this.labelDefNumFrom);
			this.groupBoxFrom.Location = new System.Drawing.Point(13, 106);
			this.groupBoxFrom.Name = "groupBoxFrom";
			this.groupBoxFrom.Size = new System.Drawing.Size(572, 77);
			this.groupBoxFrom.TabIndex = 1;
			this.groupBoxFrom.Text = "Billing Type to merge from. This Billing Type will be merged into the Billing Typ" +
    "e above.";
			// 
			// textItemValueFrom
			// 
			this.textItemValueFrom.Location = new System.Drawing.Point(368, 37);
			this.textItemValueFrom.Name = "textItemValueFrom";
			this.textItemValueFrom.ReadOnly = true;
			this.textItemValueFrom.Size = new System.Drawing.Size(112, 20);
			this.textItemValueFrom.TabIndex = 2;
			// 
			// labelItemValueFrom
			// 
			this.labelItemValueFrom.AutoSize = true;
			this.labelItemValueFrom.Location = new System.Drawing.Point(365, 21);
			this.labelItemValueFrom.Name = "labelItemValueFrom";
			this.labelItemValueFrom.Size = new System.Drawing.Size(49, 13);
			this.labelItemValueFrom.TabIndex = 7;
			this.labelItemValueFrom.Text = "Behavior";
			// 
			// textNameFrom
			// 
			this.textNameFrom.Location = new System.Drawing.Point(127, 37);
			this.textNameFrom.Name = "textNameFrom";
			this.textNameFrom.ReadOnly = true;
			this.textNameFrom.Size = new System.Drawing.Size(237, 20);
			this.textNameFrom.TabIndex = 1;
			// 
			// labelNameFrom
			// 
			this.labelNameFrom.AutoSize = true;
			this.labelNameFrom.Location = new System.Drawing.Point(124, 21);
			this.labelNameFrom.Name = "labelNameFrom";
			this.labelNameFrom.Size = new System.Drawing.Size(35, 13);
			this.labelNameFrom.TabIndex = 5;
			this.labelNameFrom.Text = "Name";
			// 
			// butChangeFrom
			// 
			this.butChangeFrom.Location = new System.Drawing.Point(486, 33);
			this.butChangeFrom.Name = "butChangeFrom";
			this.butChangeFrom.Size = new System.Drawing.Size(75, 24);
			this.butChangeFrom.TabIndex = 3;
			this.butChangeFrom.Text = "Change";
			this.butChangeFrom.UseVisualStyleBackColor = true;
			this.butChangeFrom.Click += new System.EventHandler(this.butChangeFrom_Click);
			// 
			// textDefNumFrom
			// 
			this.textDefNumFrom.Location = new System.Drawing.Point(7, 37);
			this.textDefNumFrom.Name = "textDefNumFrom";
			this.textDefNumFrom.ReadOnly = true;
			this.textDefNumFrom.Size = new System.Drawing.Size(117, 20);
			this.textDefNumFrom.TabIndex = 0;
			// 
			// labelDefNumFrom
			// 
			this.labelDefNumFrom.AutoSize = true;
			this.labelDefNumFrom.Location = new System.Drawing.Point(4, 21);
			this.labelDefNumFrom.Name = "labelDefNumFrom";
			this.labelDefNumFrom.Size = new System.Drawing.Size(46, 13);
			this.labelDefNumFrom.TabIndex = 2;
			this.labelDefNumFrom.Text = "DefNum";
			// 
			// FormBillingTypeMerge
			// 
			this.ClientSize = new System.Drawing.Size(607, 238);
			this.Controls.Add(this.groupBoxFrom);
			this.Controls.Add(this.groupBoxInto);
			this.Controls.Add(this.butMerge);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormBillingTypeMerge";
			this.Text = "Merge Billing Types";
			this.groupBoxInto.ResumeLayout(false);
			this.groupBoxInto.PerformLayout();
			this.groupBoxFrom.ResumeLayout(false);
			this.groupBoxFrom.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butMerge;
		private OpenDental.UI.GroupBox groupBoxInto;
		private UI.Button butChangeInto;
		private System.Windows.Forms.TextBox textDefNumInto;
		private System.Windows.Forms.Label labelDefNumInto;
		private OpenDental.UI.GroupBox groupBoxFrom;
		private UI.Button butChangeFrom;
		private System.Windows.Forms.TextBox textDefNumFrom;
		private System.Windows.Forms.Label labelDefNumFrom;
		private System.Windows.Forms.TextBox textItemValueInto;
		private System.Windows.Forms.Label labelItemValueInto;
		private System.Windows.Forms.TextBox textNameInto;
		private System.Windows.Forms.Label labelNameInto;
		private System.Windows.Forms.TextBox textItemValueFrom;
		private System.Windows.Forms.Label labelItemValueFrom;
		private System.Windows.Forms.TextBox textNameFrom;
		private System.Windows.Forms.Label labelNameFrom;
	}
}