namespace OpenDental {
	partial class FormSupplyInventory {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSupplyInventory));
			this.gridMain = new OpenDental.UI.GridOD();
			this.butAddNeeded = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butEquipment = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.butOrders = new OpenDental.UI.Button();
			this.butSupplies = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 155);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(450, 439);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = "Supplies Needed";
			this.gridMain.TranslationName = "TableSupplies";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridNeeded_CellDoubleClick);
			// 
			// butAddNeeded
			// 
			this.butAddNeeded.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddNeeded.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddNeeded.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddNeeded.Location = new System.Drawing.Point(12, 602);
			this.butAddNeeded.Name = "butAddNeeded";
			this.butAddNeeded.Size = new System.Drawing.Size(80, 26);
			this.butAddNeeded.TabIndex = 5;
			this.butAddNeeded.Text = "Add";
			this.butAddNeeded.Click += new System.EventHandler(this.butAddNeeded_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(387, 602);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butEquipment
			// 
			this.butEquipment.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEquipment.Location = new System.Drawing.Point(12, 27);
			this.butEquipment.Name = "butEquipment";
			this.butEquipment.Size = new System.Drawing.Size(80, 24);
			this.butEquipment.TabIndex = 15;
			this.butEquipment.Text = "Equipment";
			this.butEquipment.Click += new System.EventHandler(this.butEquipment_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(158, 602);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(80, 26);
			this.butPrint.TabIndex = 24;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butOrders
			// 
			this.butOrders.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butOrders.Location = new System.Drawing.Point(12, 57);
			this.butOrders.Name = "butOrders";
			this.butOrders.Size = new System.Drawing.Size(80, 24);
			this.butOrders.TabIndex = 14;
			this.butOrders.Text = "Orders";
			this.butOrders.Click += new System.EventHandler(this.butOrders_Click);
			// 
			// butSupplies
			// 
			this.butSupplies.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSupplies.Location = new System.Drawing.Point(12, 87);
			this.butSupplies.Name = "butSupplies";
			this.butSupplies.Size = new System.Drawing.Size(80, 24);
			this.butSupplies.TabIndex = 13;
			this.butSupplies.Text = "Supplies";
			this.butSupplies.Click += new System.EventHandler(this.butSupplies_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(98, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(364, 18);
			this.label1.TabIndex = 25;
			this.label1.Text = "For keeping track of equipment for payment of property tax";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(98, 60);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(364, 18);
			this.label2.TabIndex = 26;
			this.label2.Text = "Orders placed or new orders pending";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(98, 90);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(364, 18);
			this.label3.TabIndex = 27;
			this.label3.Text = "List of all supplies.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(12, 123);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(450, 29);
			this.label4.TabIndex = 28;
			this.label4.Text = "If staff notices that an item is running low, it can be temporarily indicated bel" +
    "ow.  But most supply shortages are instead noticed by taking regular inventory.";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(474, 24);
			this.menuMain.TabIndex = 29;
			// 
			// FormSupplyInventory
			// 
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.ClientSize = new System.Drawing.Size(474, 638);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butAddNeeded);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butEquipment);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butOrders);
			this.Controls.Add(this.butSupplies);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSupplyInventory";
			this.Text = "Supply Inventory";
			this.Load += new System.EventHandler(this.FormInventory_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butAddNeeded;
		private OpenDental.UI.Button butPrint;
		private UI.Button butSupplies;
		private UI.Button butOrders;
		private UI.Button butEquipment;
		private UI.GridOD gridMain;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private UI.MenuOD menuMain;
	}
}