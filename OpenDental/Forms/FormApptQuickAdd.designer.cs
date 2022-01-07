namespace OpenDental{
	partial class FormApptQuickAdd {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptQuickAdd));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.listQuickAdd = new OpenDental.UI.ListBoxOD();
			this.labelQuickAdd = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(292, 384);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(292, 414);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// listQuickAdd
			// 
			this.listQuickAdd.IntegralHeight = false;
			this.listQuickAdd.Location = new System.Drawing.Point(70, 63);
			this.listQuickAdd.Name = "listQuickAdd";
			this.listQuickAdd.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listQuickAdd.Size = new System.Drawing.Size(186, 375);
			this.listQuickAdd.TabIndex = 152;
			this.listQuickAdd.DoubleClick += new System.EventHandler(this.listQuickAdd_DoubleClick);
			// 
			// labelQuickAdd
			// 
			this.labelQuickAdd.Location = new System.Drawing.Point(1, 9);
			this.labelQuickAdd.Name = "labelQuickAdd";
			this.labelQuickAdd.Size = new System.Drawing.Size(378, 45);
			this.labelQuickAdd.TabIndex = 151;
			this.labelQuickAdd.Text = "Double click on an item in the list to add procedures to this appointment.\r\nor\r\nH" +
    "old down the Ctrl key while selecting multiple items, then click OK.";
			this.labelQuickAdd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// FormApptQuickAdd
			// 
			this.ClientSize = new System.Drawing.Size(379, 450);
			this.Controls.Add(this.listQuickAdd);
			this.Controls.Add(this.labelQuickAdd);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormApptQuickAdd";
			this.Text = "Add Procedures";
			this.Load += new System.EventHandler(this.FormApptQuickAdd_Load);
			this.Shown += new System.EventHandler(this.FormApptQuickAdd_Shown);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.ListBoxOD listQuickAdd;
		private System.Windows.Forms.Label labelQuickAdd;
	}
}