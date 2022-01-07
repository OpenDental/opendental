namespace OpenDental{
	partial class FormMountDefGenerate {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMountDefGenerate));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.listType = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.textHeight = new OpenDental.ValidNum();
			this.label4 = new System.Windows.Forms.Label();
			this.textWidth = new OpenDental.ValidNum();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBoxOD1 = new OpenDental.UI.GroupBoxOD();
			this.textRows = new OpenDental.ValidNum();
			this.label6 = new System.Windows.Forms.Label();
			this.textColumns = new OpenDental.ValidNum();
			this.label7 = new System.Windows.Forms.Label();
			this.groupBoxOD1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(420, 274);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "Generate";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(501, 274);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(27, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(61, 17);
			this.label2.TabIndex = 15;
			this.label2.Text = "Type";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listType
			// 
			this.listType.Items.AddRange(new object[] {
            "FMX with horiz BW",
            "FMX with vert BW",
            "4BW horiz",
            "4BW vert",
            "Comparison (2 side by side)",
            "Blank",
            "Photo Grid"});
			this.listType.Location = new System.Drawing.Point(90, 56);
			this.listType.Name = "listType";
			this.listType.Size = new System.Drawing.Size(158, 95);
			this.listType.TabIndex = 14;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(445, 36);
			this.label1.TabIndex = 16;
			this.label1.Text = "It\'s time consuming to manually lay out all the items on a new mount.  This tool " +
    "automates much of the initial work.";
			// 
			// textHeight
			// 
			this.textHeight.Location = new System.Drawing.Point(88, 200);
			this.textHeight.MaxVal = 9000000;
			this.textHeight.MinVal = 1;
			this.textHeight.Name = "textHeight";
			this.textHeight.Size = new System.Drawing.Size(48, 20);
			this.textHeight.TabIndex = 33;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(4, 200);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(82, 17);
			this.label4.TabIndex = 32;
			this.label4.Text = "Item Height";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textWidth
			// 
			this.textWidth.Location = new System.Drawing.Point(88, 177);
			this.textWidth.MaxVal = 9000000;
			this.textWidth.MinVal = 1;
			this.textWidth.Name = "textWidth";
			this.textWidth.Size = new System.Drawing.Size(48, 20);
			this.textWidth.TabIndex = 31;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(4, 177);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82, 17);
			this.label3.TabIndex = 30;
			this.label3.Text = "Item Width";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(139, 175);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(449, 67);
			this.label5.TabIndex = 34;
			this.label5.Text = resources.GetString("label5.Text");
			// 
			// groupBoxOD1
			// 
			this.groupBoxOD1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxOD1.Controls.Add(this.textRows);
			this.groupBoxOD1.Controls.Add(this.label6);
			this.groupBoxOD1.Controls.Add(this.textColumns);
			this.groupBoxOD1.Controls.Add(this.label7);
			this.groupBoxOD1.Location = new System.Drawing.Point(281, 79);
			this.groupBoxOD1.Name = "groupBoxOD1";
			this.groupBoxOD1.Size = new System.Drawing.Size(158, 72);
			this.groupBoxOD1.TabIndex = 35;
			this.groupBoxOD1.TabStop = false;
			this.groupBoxOD1.Text = "Photo Grid";
			// 
			// textRows
			// 
			this.textRows.Location = new System.Drawing.Point(75, 42);
			this.textRows.MaxVal = 20;
			this.textRows.Name = "textRows";
			this.textRows.ShowZero = false;
			this.textRows.Size = new System.Drawing.Size(48, 20);
			this.textRows.TabIndex = 37;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(12, 42);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(61, 17);
			this.label6.TabIndex = 36;
			this.label6.Text = "Rows";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textColumns
			// 
			this.textColumns.Location = new System.Drawing.Point(75, 19);
			this.textColumns.MaxVal = 20;
			this.textColumns.Name = "textColumns";
			this.textColumns.ShowZero = false;
			this.textColumns.Size = new System.Drawing.Size(48, 20);
			this.textColumns.TabIndex = 35;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(12, 19);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(61, 17);
			this.label7.TabIndex = 34;
			this.label7.Text = "Columns";
			this.label7.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// FormMountDefGenerate
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(588, 310);
			this.Controls.Add(this.groupBoxOD1);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textHeight);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textWidth);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listType);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMountDefGenerate";
			this.Text = "Generate Mount";
			this.Load += new System.EventHandler(this.FormMountDefGenerate_Load);
			this.groupBoxOD1.ResumeLayout(false);
			this.groupBoxOD1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.ListBoxOD listType;
		private System.Windows.Forms.Label label1;
		private ValidNum textHeight;
		private System.Windows.Forms.Label label4;
		private ValidNum textWidth;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private UI.GroupBoxOD groupBoxOD1;
		private ValidNum textRows;
		private System.Windows.Forms.Label label6;
		private ValidNum textColumns;
		private System.Windows.Forms.Label label7;
	}
}