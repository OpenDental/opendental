namespace OpenDentalGraph {
	partial class FormDashboardEditCell {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDashboardEditCell));
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.labelChanges = new System.Windows.Forms.Label();
			this.butPrintPreview = new System.Windows.Forms.Button();
			this.butCancel = new System.Windows.Forms.Button();
			this.butOk = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer.IsSplitterFixed = true;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Margin = new System.Windows.Forms.Padding(0);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.labelChanges);
			this.splitContainer.Panel2.Controls.Add(this.butPrintPreview);
			this.splitContainer.Panel2.Controls.Add(this.butCancel);
			this.splitContainer.Panel2.Controls.Add(this.butOk);
			this.splitContainer.Size = new System.Drawing.Size(812, 500);
			this.splitContainer.SplitterDistance = 474;
			this.splitContainer.SplitterWidth = 1;
			this.splitContainer.TabIndex = 0;
			// 
			// labelChanges
			// 
			this.labelChanges.Location = new System.Drawing.Point(79, 5);
			this.labelChanges.Name = "labelChanges";
			this.labelChanges.Size = new System.Drawing.Size(574, 18);
			this.labelChanges.TabIndex = 9;
			this.labelChanges.Text = "Changes will not be saved. You must be in setup mode to save changes to the defau" +
    "lt view.";
			this.labelChanges.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelChanges.Visible = false;
			// 
			// butPrintPreview
			// 
			this.butPrintPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrintPreview.BackColor = System.Drawing.SystemColors.Control;
			this.butPrintPreview.Image = global::OpenDentalGraph.Properties.Resources.printpreview;
			this.butPrintPreview.Location = new System.Drawing.Point(3, 0);
			this.butPrintPreview.Name = "butPrintPreview";
			this.butPrintPreview.Size = new System.Drawing.Size(28, 28);
			this.butPrintPreview.TabIndex = 8;
			this.butPrintPreview.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip.SetToolTip(this.butPrintPreview, "Print/Export");
			this.butPrintPreview.UseVisualStyleBackColor = false;
			this.butPrintPreview.Click += new System.EventHandler(this.butPrintPreview_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(734, 2);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 23);
			this.butCancel.TabIndex = 1;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.butOk.Location = new System.Drawing.Point(653, 2);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(75, 23);
			this.butOk.TabIndex = 0;
			this.butOk.Text = "OK";
			this.butOk.UseVisualStyleBackColor = true;
			// 
			// FormDashboardEditCell
			// 
			this.AcceptButton = this.butOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(812, 500);
			this.Controls.Add(this.splitContainer);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDashboardEditCell";
			this.Text = "Edit Cell";
			this.splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butOk;
		private System.Windows.Forms.Button butPrintPreview;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Label labelChanges;
	}
}