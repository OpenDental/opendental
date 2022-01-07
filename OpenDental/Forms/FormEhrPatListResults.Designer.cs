namespace OpenDental {
	partial class FormEhrPatListResults {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrPatListResults));
			this.butClose = new System.Windows.Forms.Button();
			this.groupOrderBy = new System.Windows.Forms.GroupBox();
			this.radioDesc = new System.Windows.Forms.RadioButton();
			this.radioAsc = new System.Windows.Forms.RadioButton();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butPrint = new System.Windows.Forms.Button();
			this.groupOrderBy.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(799, 335);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 17;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// groupOrderBy
			// 
			this.groupOrderBy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupOrderBy.Controls.Add(this.radioDesc);
			this.groupOrderBy.Controls.Add(this.radioAsc);
			this.groupOrderBy.Location = new System.Drawing.Point(799, 12);
			this.groupOrderBy.Name = "groupOrderBy";
			this.groupOrderBy.Size = new System.Drawing.Size(75, 68);
			this.groupOrderBy.TabIndex = 18;
			this.groupOrderBy.TabStop = false;
			this.groupOrderBy.Text = "Order By";
			// 
			// radioDesc
			// 
			this.radioDesc.AutoSize = true;
			this.radioDesc.Location = new System.Drawing.Point(11, 39);
			this.radioDesc.Name = "radioDesc";
			this.radioDesc.Size = new System.Drawing.Size(50, 17);
			this.radioDesc.TabIndex = 20;
			this.radioDesc.Text = "Desc";
			this.radioDesc.UseVisualStyleBackColor = true;
			// 
			// radioAsc
			// 
			this.radioAsc.AutoSize = true;
			this.radioAsc.Location = new System.Drawing.Point(11, 19);
			this.radioAsc.Name = "radioAsc";
			this.radioAsc.Size = new System.Drawing.Size(43, 17);
			this.radioAsc.TabIndex = 19;
			this.radioAsc.Text = "Asc";
			this.radioAsc.UseVisualStyleBackColor = true;
			this.radioAsc.CheckedChanged += new System.EventHandler(this.radioOrderBy_CheckedChanged);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(2, 2);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(781, 367);
			this.gridMain.TabIndex = 10;
			this.gridMain.Title = "Results";
			this.gridMain.TranslationName = "FormPatientListResults";
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Location = new System.Drawing.Point(799, 279);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 23);
			this.butPrint.TabIndex = 20;
			this.butPrint.Text = "Print";
			this.butPrint.UseVisualStyleBackColor = true;
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// FormEhrPatListResults
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(886, 370);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.groupOrderBy);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrPatListResults";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Patient List Results";
			this.Load += new System.EventHandler(this.FormPatListResults_Load);
			this.groupOrderBy.ResumeLayout(false);
			this.groupOrderBy.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.GridOD gridMain;
		private System.Windows.Forms.Button butClose;
		private System.Windows.Forms.GroupBox groupOrderBy;
		private System.Windows.Forms.RadioButton radioDesc;
		private System.Windows.Forms.RadioButton radioAsc;
		private System.Windows.Forms.Button butPrint;


	}
}