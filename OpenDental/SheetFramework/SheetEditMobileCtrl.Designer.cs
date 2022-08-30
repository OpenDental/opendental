namespace OpenDental {
	partial class SheetEditMobileCtrl {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.labelDragToOrder = new System.Windows.Forms.Label();
			this.scrollTimer = new System.Windows.Forms.Timer(this.components);
			this.checkUseMobileLayout = new System.Windows.Forms.CheckBox();
			this.butAddHeader = new System.Windows.Forms.Button();
			this.butOrderFields = new System.Windows.Forms.Button();
			this.panelPreview = new OpenDental.SheetEditMobilePreviewPanel();
			this.SuspendLayout();
			// 
			// labelDragToOrder
			// 
			this.labelDragToOrder.Location = new System.Drawing.Point(3, 13);
			this.labelDragToOrder.Name = "labelDragToOrder";
			this.labelDragToOrder.Size = new System.Drawing.Size(234, 16);
			this.labelDragToOrder.TabIndex = 0;
			this.labelDragToOrder.Text = "Mobile order will not affect desktop tab order.";
			this.labelDragToOrder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// scrollTimer
			// 
			this.scrollTimer.Interval = 10;
			this.scrollTimer.Tick += new System.EventHandler(this.scrollTimer_Tick);
			// 
			// checkUseMobileLayout
			// 
			this.checkUseMobileLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkUseMobileLayout.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseMobileLayout.Location = new System.Drawing.Point(258, 14);
			this.checkUseMobileLayout.Name = "checkUseMobileLayout";
			this.checkUseMobileLayout.Size = new System.Drawing.Size(118, 16);
			this.checkUseMobileLayout.TabIndex = 88;
			this.checkUseMobileLayout.Text = "Use Mobile Layout";
			this.checkUseMobileLayout.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseMobileLayout.UseVisualStyleBackColor = true;
			this.checkUseMobileLayout.CheckedChanged += new System.EventHandler(this.checkUseMobileLayout_CheckedChanged);
			// 
			// butAddHeader
			// 
			this.butAddHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddHeader.Location = new System.Drawing.Point(382, 10);
			this.butAddHeader.Name = "butAddHeader";
			this.butAddHeader.Size = new System.Drawing.Size(75, 23);
			this.butAddHeader.TabIndex = 89;
			this.butAddHeader.Text = "Add Header";
			this.butAddHeader.UseVisualStyleBackColor = true;
			this.butAddHeader.Click += new System.EventHandler(this.butAddHeader_Click);
			// 
			// butOrderFields
			// 
			this.butOrderFields.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butOrderFields.Location = new System.Drawing.Point(463, 10);
			this.butOrderFields.Name = "butOrderFields";
			this.butOrderFields.Size = new System.Drawing.Size(177, 23);
			this.butOrderFields.TabIndex = 90;
			this.butOrderFields.Text = "Order Fields from Desktop";
			this.butOrderFields.UseVisualStyleBackColor = true;
			this.butOrderFields.Click += new System.EventHandler(this.butOrderFields_Click);
			// 
			// panelPreview
			// 
			this.panelPreview.AllowDrop = true;
			this.panelPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelPreview.AutoScroll = true;
			this.panelPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelPreview.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.panelPreview.Location = new System.Drawing.Point(2, 40);
			this.panelPreview.Margin = new System.Windows.Forms.Padding(0);
			this.panelPreview.Name = "panelPreview";
			this.panelPreview.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this.panelPreview.Size = new System.Drawing.Size(656, 343);
			this.panelPreview.TabIndex = 87;
			this.panelPreview.WrapContents = false;
			this.panelPreview.DragDrop += new System.Windows.Forms.DragEventHandler(this.panel_DragDrop);
			this.panelPreview.DragEnter += new System.Windows.Forms.DragEventHandler(this.panel_DragEnter);
			this.panelPreview.Resize += new System.EventHandler(this.panelPreview_Resize);
			// 
			// SheetEditMobileCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.butOrderFields);
			this.Controls.Add(this.butAddHeader);
			this.Controls.Add(this.checkUseMobileLayout);
			this.Controls.Add(this.panelPreview);
			this.Controls.Add(this.labelDragToOrder);
			this.Name = "SheetEditMobileCtrl";
			this.Size = new System.Drawing.Size(658, 385);
			this.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.SheetEditMobileCtrl_GiveFeedback);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Label labelDragToOrder;
		private SheetEditMobilePreviewPanel panelPreview;
		private System.Windows.Forms.Timer scrollTimer;
		private System.Windows.Forms.CheckBox checkUseMobileLayout;
		private System.Windows.Forms.Button butAddHeader;
		private System.Windows.Forms.Button butOrderFields;
	}
}
