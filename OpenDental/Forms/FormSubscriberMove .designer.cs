namespace OpenDental{
	partial class FormSubscriberMove {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSubscriberMove));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butViewInsPlanInto = new OpenDental.UI.Button();
			this.butPickInsPlanInto = new OpenDental.UI.Button();
			this.textCarrierNameInto = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.butViewInsPlanFrom = new OpenDental.UI.Button();
			this.butPickInsPlanFrom = new OpenDental.UI.Button();
			this.textCarrierNameFrom = new System.Windows.Forms.TextBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.butViewInsPlanInto);
			this.groupBox1.Controls.Add(this.butPickInsPlanInto);
			this.groupBox1.Controls.Add(this.textCarrierNameInto);
			this.groupBox1.Location = new System.Drawing.Point(9, 32);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(478, 50);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Ins plan to move subscribers to.";
			// 
			// butViewInsPlanInto
			// 
			this.butViewInsPlanInto.Location = new System.Drawing.Point(397, 18);
			this.butViewInsPlanInto.Name = "butViewInsPlanInto";
			this.butViewInsPlanInto.Size = new System.Drawing.Size(75, 24);
			this.butViewInsPlanInto.TabIndex = 3;
			this.butViewInsPlanInto.Text = "View";
			this.butViewInsPlanInto.Click += new System.EventHandler(this.butViewInsPlanInto_Click);
			// 
			// butPickInsPlanInto
			// 
			this.butPickInsPlanInto.Location = new System.Drawing.Point(369, 18);
			this.butPickInsPlanInto.Name = "butPickInsPlanInto";
			this.butPickInsPlanInto.Size = new System.Drawing.Size(25, 24);
			this.butPickInsPlanInto.TabIndex = 2;
			this.butPickInsPlanInto.Text = "...";
			this.butPickInsPlanInto.Click += new System.EventHandler(this.butChangePatientInto_Click);
			// 
			// textCarrierNameInto
			// 
			this.textCarrierNameInto.Location = new System.Drawing.Point(93, 20);
			this.textCarrierNameInto.Name = "textCarrierNameInto";
			this.textCarrierNameInto.ReadOnly = true;
			this.textCarrierNameInto.Size = new System.Drawing.Size(273, 20);
			this.textCarrierNameInto.TabIndex = 1;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.butViewInsPlanFrom);
			this.groupBox2.Controls.Add(this.butPickInsPlanFrom);
			this.groupBox2.Controls.Add(this.textCarrierNameFrom);
			this.groupBox2.Location = new System.Drawing.Point(9, 88);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(478, 50);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Ins plan to move subscribers from.  Hidden after subscribers are moved.";
			// 
			// butViewInsPlanFrom
			// 
			this.butViewInsPlanFrom.Location = new System.Drawing.Point(397, 18);
			this.butViewInsPlanFrom.Name = "butViewInsPlanFrom";
			this.butViewInsPlanFrom.Size = new System.Drawing.Size(75, 24);
			this.butViewInsPlanFrom.TabIndex = 3;
			this.butViewInsPlanFrom.Text = "View";
			this.butViewInsPlanFrom.Click += new System.EventHandler(this.butViewInsPlanFrom_Click);
			// 
			// butPickInsPlanFrom
			// 
			this.butPickInsPlanFrom.Location = new System.Drawing.Point(369, 18);
			this.butPickInsPlanFrom.Name = "butPickInsPlanFrom";
			this.butPickInsPlanFrom.Size = new System.Drawing.Size(25, 24);
			this.butPickInsPlanFrom.TabIndex = 2;
			this.butPickInsPlanFrom.Text = "...";
			this.butPickInsPlanFrom.Click += new System.EventHandler(this.butChangePatientFrom_Click);
			// 
			// textCarrierNameFrom
			// 
			this.textCarrierNameFrom.Location = new System.Drawing.Point(93, 20);
			this.textCarrierNameFrom.Name = "textCarrierNameFrom";
			this.textCarrierNameFrom.ReadOnly = true;
			this.textCarrierNameFrom.Size = new System.Drawing.Size(273, 20);
			this.textCarrierNameFrom.TabIndex = 1;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(328, 154);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(406, 154);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 7);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(471, 20);
			this.label7.TabIndex = 0;
			this.label7.Text = "Globally move subscribers of one insurance plan over to another insurance plan.";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormSubscriberMove
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(496, 190);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSubscriberMove";
			this.Text = "Move Subscribers";
			this.Load += new System.EventHandler(this.FormSubscriberMove_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textCarrierNameInto;
		private System.Windows.Forms.GroupBox groupBox2;
		private OpenDental.UI.Button butPickInsPlanInto;
		private OpenDental.UI.Button butPickInsPlanFrom;
		private System.Windows.Forms.TextBox textCarrierNameFrom;
		private System.Windows.Forms.Label label7;
		private UI.Button butViewInsPlanInto;
		private UI.Button butViewInsPlanFrom;
	}
}