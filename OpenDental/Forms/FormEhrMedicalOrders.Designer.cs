namespace OpenDental {
	partial class FormEhrMedicalOrders {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrMedicalOrders));
			this.butClose = new System.Windows.Forms.Button();
			this.checkBoxShowDiscontinued = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.gridMedOrders = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(634, 285);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 9;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// checkBoxShowDiscontinued
			// 
			this.checkBoxShowDiscontinued.Location = new System.Drawing.Point(12, 12);
			this.checkBoxShowDiscontinued.Name = "checkBoxShowDiscontinued";
			this.checkBoxShowDiscontinued.Size = new System.Drawing.Size(532, 17);
			this.checkBoxShowDiscontinued.TabIndex = 10;
			this.checkBoxShowDiscontinued.Text = "Show discontinued orders";
			this.checkBoxShowDiscontinued.UseVisualStyleBackColor = true;
			this.checkBoxShowDiscontinued.Click += new System.EventHandler(this.checkBoxShowDiscontinued_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.label1.ForeColor = System.Drawing.Color.Black;
			this.label1.Location = new System.Drawing.Point(9, 285);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(619, 28);
			this.label1.TabIndex = 34;
			this.label1.Text = "The grid above shows lab and radiology orders entered while using 2011 EHR.  \r\nLa" +
    "bs and radiology orders are now entered via the EHR window.";
			// 
			// gridMedOrders
			// 
			this.gridMedOrders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMedOrders.Location = new System.Drawing.Point(12, 35);
			this.gridMedOrders.Name = "gridMedOrders";
			this.gridMedOrders.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMedOrders.Size = new System.Drawing.Size(697, 244);
			this.gridMedOrders.TabIndex = 5;
			this.gridMedOrders.Title = "Lab and Radiology Orders";
			this.gridMedOrders.TranslationName = "TableOrders";
			this.gridMedOrders.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMedOrders_CellDoubleClick);
			// 
			// FormEhrMedicalOrders
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(721, 320);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkBoxShowDiscontinued);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.gridMedOrders);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrMedicalOrders";
			this.Text = "2011 Lab and Radiology Orders";
			this.Load += new System.EventHandler(this.FormMedicalOrders_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.GridOD gridMedOrders;
		private System.Windows.Forms.Button butClose;
		private System.Windows.Forms.CheckBox checkBoxShowDiscontinued;
		private System.Windows.Forms.Label label1;
	}
}