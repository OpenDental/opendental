namespace OpenDental{
	partial class FormFieldDefLink {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFieldDefLink));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.gridDisplayed = new OpenDental.UI.GridOD();
			this.gridHidden = new OpenDental.UI.GridOD();
			this.butRight = new OpenDental.UI.Button();
			this.butLeft = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.comboFieldLocation = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(470, 464);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(551, 464);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// gridDisplayed
			// 
			this.gridDisplayed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridDisplayed.Location = new System.Drawing.Point(16, 87);
			this.gridDisplayed.Name = "gridDisplayed";
			this.gridDisplayed.Size = new System.Drawing.Size(270, 360);
			this.gridDisplayed.TabIndex = 4;
			this.gridDisplayed.Title = "Visible Fields";
			this.gridDisplayed.TranslationName = "TableVisible";
			// 
			// gridHidden
			// 
			this.gridHidden.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridHidden.Location = new System.Drawing.Point(355, 87);
			this.gridHidden.Name = "gridHidden";
			this.gridHidden.Size = new System.Drawing.Size(270, 360);
			this.gridHidden.TabIndex = 5;
			this.gridHidden.Title = "Hidden Fields ";
			this.gridHidden.TranslationName = "TableHidden";
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(303, 205);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(34, 23);
			this.butRight.TabIndex = 6;
			this.butRight.UseVisualStyleBackColor = true;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butLeft
			// 
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(303, 252);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(34, 23);
			this.butLeft.TabIndex = 7;
			this.butLeft.UseVisualStyleBackColor = true;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(13, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(612, 48);
			this.label1.TabIndex = 8;
			this.label1.Text = "Select fields on the left and move them to the right in order to hide the selecte" +
    "d field from the specified location in the software.";
			// 
			// comboFieldLocation
			// 
			this.comboFieldLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboFieldLocation.FormattingEnabled = true;
			this.comboFieldLocation.Location = new System.Drawing.Point(125, 60);
			this.comboFieldLocation.Name = "comboFieldLocation";
			this.comboFieldLocation.Size = new System.Drawing.Size(161, 21);
			this.comboFieldLocation.TabIndex = 10;
			this.comboFieldLocation.SelectionChangeCommitted += new System.EventHandler(this.comboFieldLocation_SelectionChangeCommitted);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(14, 60);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(105, 18);
			this.label2.TabIndex = 11;
			this.label2.Text = "Field Location";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormFieldDefLink
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(638, 500);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboFieldLocation);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butLeft);
			this.Controls.Add(this.butRight);
			this.Controls.Add(this.gridHidden);
			this.Controls.Add(this.gridDisplayed);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFieldDefLink";
			this.Text = "Field Display";
			this.Load += new System.EventHandler(this.FormFieldDefLink_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridDisplayed;
		private UI.GridOD gridHidden;
		private UI.Button butRight;
		private UI.Button butLeft;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboFieldLocation;
		private System.Windows.Forms.Label label2;
	}
}