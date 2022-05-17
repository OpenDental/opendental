namespace OpenDental{
	partial class FormStateAbbrEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStateAbbrEdit));
			this.textAbbr = new System.Windows.Forms.TextBox();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelMedIDLength = new System.Windows.Forms.Label();
			this.textMedIDLength = new OpenDental.ValidNum();
			this.SuspendLayout();
			// 
			// textAbbr
			// 
			this.textAbbr.Location = new System.Drawing.Point(103, 47);
			this.textAbbr.Name = "textAbbr";
			this.textAbbr.Size = new System.Drawing.Size(100, 20);
			this.textAbbr.TabIndex = 2;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(103, 21);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(281, 20);
			this.textDescription.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(9, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 20);
			this.label1.TabIndex = 18;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(85, 20);
			this.label2.TabIndex = 19;
			this.label2.Text = "Abbreviation";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(15, 113);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 6;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(243, 113);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(324, 113);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelMedIDLength
			// 
			this.labelMedIDLength.Location = new System.Drawing.Point(12, 73);
			this.labelMedIDLength.Name = "labelMedIDLength";
			this.labelMedIDLength.Size = new System.Drawing.Size(85, 33);
			this.labelMedIDLength.TabIndex = 21;
			this.labelMedIDLength.Text = "Medicaid ID Length";
			this.labelMedIDLength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMedIDLength
			// 
			this.textMedIDLength.Location = new System.Drawing.Point(103, 73);
			this.textMedIDLength.MaxVal = 255;
			this.textMedIDLength.MinVal = 0;
			this.textMedIDLength.Name = "textMedIDLength";
			this.textMedIDLength.Size = new System.Drawing.Size(100, 20);
			this.textMedIDLength.TabIndex = 3;
			// 
			// FormStateAbbrEdit
			// 
			this.ClientSize = new System.Drawing.Size(411, 150);
			this.Controls.Add(this.textMedIDLength);
			this.Controls.Add(this.labelMedIDLength);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.textAbbr);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormStateAbbrEdit";
			this.Text = "State Abbreviation Edit";
			this.Load += new System.EventHandler(this.FormStateAbbrEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.Button butDelete;
		private System.Windows.Forms.TextBox textAbbr;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelMedIDLength;
		private ValidNum textMedIDLength;
	}
}