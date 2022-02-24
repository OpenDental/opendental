namespace OpenDental{
	partial class FormWikiTableViewEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWikiTableViewEdit));
			this.labelView = new System.Windows.Forms.Label();
			this.textViewName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.listAvail = new OpenDental.UI.ListBoxOD();
			this.listShowing = new OpenDental.UI.ListBoxOD();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.comboOrderBy = new System.Windows.Forms.ComboBox();
			this.butDelete = new OpenDental.UI.Button();
			this.butLeft = new OpenDental.UI.Button();
			this.butRight = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelView
			// 
			this.labelView.Location = new System.Drawing.Point(12, 11);
			this.labelView.Name = "labelView";
			this.labelView.Size = new System.Drawing.Size(94, 18);
			this.labelView.TabIndex = 35;
			this.labelView.Text = "View Name";
			this.labelView.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textViewName
			// 
			this.textViewName.Location = new System.Drawing.Point(112, 11);
			this.textViewName.Name = "textViewName";
			this.textViewName.Size = new System.Drawing.Size(119, 20);
			this.textViewName.TabIndex = 36;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(14, 59);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 18);
			this.label1.TabIndex = 37;
			this.label1.Text = "Available Columns";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listAvail
			// 
			this.listAvail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listAvail.Location = new System.Drawing.Point(15, 82);
			this.listAvail.Name = "listAvail";
			this.listAvail.Size = new System.Drawing.Size(161, 446);
			this.listAvail.TabIndex = 38;
			// 
			// listShowing
			// 
			this.listShowing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listShowing.Location = new System.Drawing.Point(282, 82);
			this.listShowing.Name = "listShowing";
			this.listShowing.Size = new System.Drawing.Size(161, 446);
			this.listShowing.TabIndex = 42;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(281, 59);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(94, 18);
			this.label2.TabIndex = 41;
			this.label2.Text = "Showing Columns";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 38);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(94, 18);
			this.label3.TabIndex = 43;
			this.label3.Text = "Order By";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label3.Visible = false;
			// 
			// comboOrderBy
			// 
			this.comboOrderBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboOrderBy.FormattingEnabled = true;
			this.comboOrderBy.Location = new System.Drawing.Point(112, 35);
			this.comboOrderBy.Name = "comboOrderBy";
			this.comboOrderBy.Size = new System.Drawing.Size(163, 21);
			this.comboOrderBy.TabIndex = 44;
			this.comboOrderBy.Visible = false;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(15, 541);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 45;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butLeft
			// 
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(206, 227);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(44, 24);
			this.butLeft.TabIndex = 40;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(206, 193);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(44, 24);
			this.butRight.TabIndex = 39;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(356, 541);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(437, 541);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormWikiTableViewEdit
			// 
			this.ClientSize = new System.Drawing.Size(524, 573);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.comboOrderBy);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.listShowing);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butLeft);
			this.Controls.Add(this.butRight);
			this.Controls.Add(this.listAvail);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textViewName);
			this.Controls.Add(this.labelView);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWikiTableViewEdit";
			this.Text = "Edit Wiki Table View";
			this.Load += new System.EventHandler(this.FormWikiTableViewEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelView;
		private System.Windows.Forms.TextBox textViewName;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.ListBoxOD listAvail;
		private UI.Button butRight;
		private UI.Button butLeft;
		private OpenDental.UI.ListBoxOD listShowing;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboOrderBy;
		private UI.Button butDelete;
	}
}