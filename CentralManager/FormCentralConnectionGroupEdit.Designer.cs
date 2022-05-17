namespace CentralManager {
	partial class FormCentralConnectionGroupEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCentralConnectionGroupEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.butOK = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butDelete = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.butRemove = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.gridAvail = new OpenDental.UI.GridOD();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(99, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Group Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(110, 12);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(237, 20);
			this.textDescription.TabIndex = 1;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(922, 531);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 6;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.Location = new System.Drawing.Point(12, 47);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(465, 457);
			this.gridMain.TabIndex = 2;
			this.gridMain.Title = "Connections for this Group";
			this.gridMain.TranslationName = "TableConnections";
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Image = ((System.Drawing.Image)(resources.GetObject("butDelete.Image")));
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 531);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(83, 24);
			this.butDelete.TabIndex = 8;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butAdd
			// 
			this.butAdd.Image = ((System.Drawing.Image)(resources.GetObject("butAdd.Image")));
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(489, 216);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(81, 24);
			this.butAdd.TabIndex = 219;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butRemove
			// 
			this.butRemove.Image = ((System.Drawing.Image)(resources.GetObject("butRemove.Image")));
			this.butRemove.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butRemove.Location = new System.Drawing.Point(489, 257);
			this.butRemove.Name = "butRemove";
			this.butRemove.Size = new System.Drawing.Size(81, 24);
			this.butRemove.TabIndex = 220;
			this.butRemove.Text = "Remove";
			this.butRemove.UseVisualStyleBackColor = true;
			this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(377, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(235, 20);
			this.label2.TabIndex = 221;
			this.label2.Text = "Connections can belong to multiple groups";
			// 
			// gridAvail
			// 
			this.gridAvail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridAvail.Location = new System.Drawing.Point(584, 47);
			this.gridAvail.Name = "gridAvail";
			this.gridAvail.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridAvail.Size = new System.Drawing.Size(465, 457);
			this.gridAvail.TabIndex = 222;
			this.gridAvail.Title = "Available Connections";
			this.gridAvail.TranslationName = "TableConnections";
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(1003, 531);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 223;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormCentralConnectionGroupEdit
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1106, 566);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.gridAvail);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butRemove);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(497, 376);
			this.Name = "FormCentralConnectionGroupEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Connection Group Edit";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCentralConnectionGroupEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormCentralConnectionGroupEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textDescription;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butRemove;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.GridOD gridAvail;
		private OpenDental.UI.Button butCancel;
	}
}