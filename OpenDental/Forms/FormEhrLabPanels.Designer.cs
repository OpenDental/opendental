namespace OpenDental {
	partial class FormEhrLabPanels {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrLabPanels));
			this.butCancel = new System.Windows.Forms.Button();
			this.butAdd = new System.Windows.Forms.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butSubmit = new System.Windows.Forms.Button();
			this.butOK = new System.Windows.Forms.Button();
			this.butShow = new System.Windows.Forms.Button();
			this.groupHL7 = new System.Windows.Forms.GroupBox();
			this.groupHL7.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(327, 293);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 1;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butAdd
			// 
			this.butAdd.Location = new System.Drawing.Point(21, 12);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 2;
			this.butAdd.Text = "Add Panel";
			this.butAdd.UseVisualStyleBackColor = true;
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(21, 42);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(381, 228);
			this.gridMain.TabIndex = 0;
			this.gridMain.Title = "Lab Panels";
			this.gridMain.TranslationName = "TablePanels";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butSubmit
			// 
			this.butSubmit.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butSubmit.Location = new System.Drawing.Point(9, 17);
			this.butSubmit.Name = "butSubmit";
			this.butSubmit.Size = new System.Drawing.Size(66, 24);
			this.butSubmit.TabIndex = 3;
			this.butSubmit.Text = "Submit";
			this.butSubmit.UseVisualStyleBackColor = true;
			this.butSubmit.Click += new System.EventHandler(this.butSubmit_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(246, 293);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butShow
			// 
			this.butShow.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butShow.Location = new System.Drawing.Point(81, 17);
			this.butShow.Name = "butShow";
			this.butShow.Size = new System.Drawing.Size(66, 24);
			this.butShow.TabIndex = 5;
			this.butShow.Text = "Show";
			this.butShow.UseVisualStyleBackColor = true;
			this.butShow.Click += new System.EventHandler(this.butShow_Click);
			// 
			// groupHL7
			// 
			this.groupHL7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupHL7.Controls.Add(this.butShow);
			this.groupHL7.Controls.Add(this.butSubmit);
			this.groupHL7.Location = new System.Drawing.Point(21, 276);
			this.groupHL7.Name = "groupHL7";
			this.groupHL7.Size = new System.Drawing.Size(156, 47);
			this.groupHL7.TabIndex = 6;
			this.groupHL7.TabStop = false;
			this.groupHL7.Text = "HL7 Msg";
			// 
			// FormEhrLabPanels
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(418, 331);
			this.Controls.Add(this.groupHL7);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrLabPanels";
			this.Text = "Lab Panels";
			this.Load += new System.EventHandler(this.FormLabPanels_Load);
			this.groupHL7.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.GridOD gridMain;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butAdd;
		private System.Windows.Forms.Button butSubmit;
		private System.Windows.Forms.Button butOK;
		private System.Windows.Forms.Button butShow;
		private System.Windows.Forms.GroupBox groupHL7;
	}
}