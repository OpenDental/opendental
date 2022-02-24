namespace OpenDental{
	partial class FormGradingScaleItemEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGradingScaleItemEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.labelGradeNumber = new System.Windows.Forms.Label();
			this.textGradeShowing = new System.Windows.Forms.TextBox();
			this.labelGradeShowing = new System.Windows.Forms.Label();
			this.labelDescription = new System.Windows.Forms.Label();
			this.labelNumber = new System.Windows.Forms.Label();
			this.textGradeNumber = new System.Windows.Forms.TextBox();
			this.butDelete = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(237, 111);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 5;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(318, 111);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 6;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(129, 69);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(199, 20);
			this.textDescription.TabIndex = 3;
			// 
			// labelGradeNumber
			// 
			this.labelGradeNumber.Location = new System.Drawing.Point(4, 40);
			this.labelGradeNumber.Name = "labelGradeNumber";
			this.labelGradeNumber.Size = new System.Drawing.Size(125, 18);
			this.labelGradeNumber.TabIndex = 72;
			this.labelGradeNumber.Text = "Grade Number";
			this.labelGradeNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textGradeShowing
			// 
			this.textGradeShowing.Location = new System.Drawing.Point(129, 12);
			this.textGradeShowing.Name = "textGradeShowing";
			this.textGradeShowing.Size = new System.Drawing.Size(61, 20);
			this.textGradeShowing.TabIndex = 1;
			// 
			// labelGradeShowing
			// 
			this.labelGradeShowing.Location = new System.Drawing.Point(4, 12);
			this.labelGradeShowing.Name = "labelGradeShowing";
			this.labelGradeShowing.Size = new System.Drawing.Size(125, 18);
			this.labelGradeShowing.TabIndex = 69;
			this.labelGradeShowing.Text = "Grade Showing";
			this.labelGradeShowing.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDescription
			// 
			this.labelDescription.Location = new System.Drawing.Point(4, 69);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(125, 18);
			this.labelDescription.TabIndex = 70;
			this.labelDescription.Text = "Description";
			this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelNumber
			// 
			this.labelNumber.Location = new System.Drawing.Point(196, 35);
			this.labelNumber.Name = "labelNumber";
			this.labelNumber.Size = new System.Drawing.Size(205, 33);
			this.labelNumber.TabIndex = 76;
			this.labelNumber.Text = "*Required";
			this.labelNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textGradeNumber
			// 
			this.textGradeNumber.Location = new System.Drawing.Point(129, 40);
			this.textGradeNumber.Name = "textGradeNumber";
			this.textGradeNumber.Size = new System.Drawing.Size(61, 20);
			this.textGradeNumber.TabIndex = 2;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(6, 111);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(78, 24);
			this.butDelete.TabIndex = 4;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// FormGradingScaleItemEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(405, 147);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textGradeNumber);
			this.Controls.Add(this.labelNumber);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.labelGradeNumber);
			this.Controls.Add(this.textGradeShowing);
			this.Controls.Add(this.labelGradeShowing);
			this.Controls.Add(this.labelDescription);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormGradingScaleItemEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Grading Scale Item Edit";
			this.Load += new System.EventHandler(this.FormGradingScaleItemEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label labelGradeNumber;
		private System.Windows.Forms.TextBox textGradeShowing;
		private System.Windows.Forms.Label labelGradeShowing;
		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.Label labelNumber;
		private System.Windows.Forms.TextBox textGradeNumber;
		private UI.Button butDelete;
	}
}