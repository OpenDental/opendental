namespace OpenDental{
	partial class FormMountAndAcquire {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMountAndAcquire));
			this.butMount = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.listViewMounts = new System.Windows.Forms.ListView();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.label2 = new System.Windows.Forms.Label();
			this.butAcquire = new OpenDental.UI.Button();
			this.listDevices = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.butMountAndAcquire = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butMount
			// 
			this.butMount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butMount.Location = new System.Drawing.Point(613, 482);
			this.butMount.Name = "butMount";
			this.butMount.Size = new System.Drawing.Size(84, 24);
			this.butMount.TabIndex = 3;
			this.butMount.Text = "Create Mount";
			this.butMount.Click += new System.EventHandler(this.butMount_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(870, 540);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// listViewMounts
			// 
			this.listViewMounts.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.listViewMounts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewMounts.HideSelection = false;
			this.listViewMounts.Location = new System.Drawing.Point(28, 40);
			this.listViewMounts.MultiSelect = false;
			this.listViewMounts.Name = "listViewMounts";
			this.listViewMounts.Size = new System.Drawing.Size(669, 436);
			this.listViewMounts.TabIndex = 4;
			this.listViewMounts.UseCompatibleStateImageBehavior = false;
			this.listViewMounts.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewMounts_MouseDoubleClick);
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(722, 19);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 18);
			this.label2.TabIndex = 39;
			this.label2.Text = "Device";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butAcquire
			// 
			this.butAcquire.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAcquire.Location = new System.Drawing.Point(837, 388);
			this.butAcquire.Name = "butAcquire";
			this.butAcquire.Size = new System.Drawing.Size(75, 24);
			this.butAcquire.TabIndex = 41;
			this.butAcquire.Text = "Acquire";
			this.butAcquire.Click += new System.EventHandler(this.butAcquire_Click);
			// 
			// listDevices
			// 
			this.listDevices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listDevices.Location = new System.Drawing.Point(725, 40);
			this.listDevices.Name = "listDevices";
			this.listDevices.Size = new System.Drawing.Size(187, 342);
			this.listDevices.TabIndex = 42;
			this.listDevices.Text = "listDevice";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(25, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 18);
			this.label1.TabIndex = 43;
			this.label1.Text = "Mount";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butMountAndAcquire
			// 
			this.butMountAndAcquire.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butMountAndAcquire.Location = new System.Drawing.Point(825, 471);
			this.butMountAndAcquire.Name = "butMountAndAcquire";
			this.butMountAndAcquire.Size = new System.Drawing.Size(87, 35);
			this.butMountAndAcquire.TabIndex = 44;
			this.butMountAndAcquire.Text = "Create Mount and Acquire";
			this.butMountAndAcquire.Click += new System.EventHandler(this.butMountAndAcquire_Click);
			// 
			// FormMountAndAcquire
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(957, 576);
			this.Controls.Add(this.butMountAndAcquire);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listDevices);
			this.Controls.Add(this.butAcquire);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listViewMounts);
			this.Controls.Add(this.butMount);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMountAndAcquire";
			this.Text = "Select Mount and Acquire";
			this.Load += new System.EventHandler(this.FormMountPick_Load);
			this.Shown += new System.EventHandler(this.FormMountSelect_Shown);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butMount;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.ListView listViewMounts;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Label label2;
		private UI.Button butAcquire;
		private UI.ListBoxOD listDevices;
		private System.Windows.Forms.Label label1;
		private UI.Button butMountAndAcquire;
	}
}