namespace OpenDental{
	partial class FormOrthoRxSelect {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOrthoRxSelect));
			this.butOK = new OpenDental.UI.Button();
			this.listBoxAvail = new OpenDental.UI.ListBox();
			this.listBoxSelected = new OpenDental.UI.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.butRemove = new OpenDental.UI.Button();
			this.butSelect = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(593, 486);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 5;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// listBoxAvail
			// 
			this.listBoxAvail.Location = new System.Drawing.Point(47, 59);
			this.listBoxAvail.Name = "listBoxAvail";
			this.listBoxAvail.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxAvail.Size = new System.Drawing.Size(234, 407);
			this.listBoxAvail.TabIndex = 6;
			this.listBoxAvail.Text = "listBoxOD1";
			// 
			// listBoxSelected
			// 
			this.listBoxSelected.Location = new System.Drawing.Point(397, 59);
			this.listBoxSelected.Name = "listBoxSelected";
			this.listBoxSelected.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxSelected.Size = new System.Drawing.Size(242, 407);
			this.listBoxSelected.TabIndex = 7;
			this.listBoxSelected.Text = "listBoxOD2";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(44, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(190, 16);
			this.label1.TabIndex = 113;
			this.label1.Text = "Available";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(394, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(190, 16);
			this.label2.TabIndex = 114;
			this.label2.Text = "Selected";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butRemove
			// 
			this.butRemove.Image = global::OpenDental.Properties.Resources.Left;
			this.butRemove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRemove.Location = new System.Drawing.Point(298, 186);
			this.butRemove.Name = "butRemove";
			this.butRemove.Size = new System.Drawing.Size(79, 24);
			this.butRemove.TabIndex = 115;
			this.butRemove.Text = "Remove";
			this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
			// 
			// butSelect
			// 
			this.butSelect.Image = global::OpenDental.Properties.Resources.Right;
			this.butSelect.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butSelect.Location = new System.Drawing.Point(298, 156);
			this.butSelect.Name = "butSelect";
			this.butSelect.Size = new System.Drawing.Size(79, 24);
			this.butSelect.TabIndex = 116;
			this.butSelect.Text = "Select";
			this.butSelect.Click += new System.EventHandler(this.butSelect_Click);
			// 
			// FormOrthoRxSelect
			// 
			this.ClientSize = new System.Drawing.Size(680, 522);
			this.Controls.Add(this.butSelect);
			this.Controls.Add(this.butRemove);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listBoxSelected);
			this.Controls.Add(this.listBoxAvail);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormOrthoRxSelect";
			this.Text = "Select Ortho Prescription";
			this.Load += new System.EventHandler(this.FormOrthoRxSelect_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.Button butOK;
		private UI.ListBox listBoxAvail;
		private UI.ListBox listBoxSelected;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private UI.Button butRemove;
		private UI.Button butSelect;
	}
}