namespace OpenDental{
	partial class FormSupplyOrders {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSupplyOrders));
			this.comboSupplier = new OpenDental.UI.ComboBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.gridItems = new OpenDental.UI.GridOD();
			this.gridOrders = new OpenDental.UI.GridOD();
			this.butPrint = new OpenDental.UI.Button();
			this.butAddSupply = new OpenDental.UI.Button();
			this.butNewOrder = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkShowReceived = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// comboSupplier
			// 
			this.comboSupplier.Location = new System.Drawing.Point(514, 10);
			this.comboSupplier.Name = "comboSupplier";
			this.comboSupplier.Size = new System.Drawing.Size(144, 21);
			this.comboSupplier.TabIndex = 13;
			this.comboSupplier.SelectionChangeCommitted += new System.EventHandler(this.comboSupplier_SelectionChangeCommitted);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(437, 10);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(75, 18);
			this.label3.TabIndex = 14;
			this.label3.Text = "Supplier";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridItems
			// 
			this.gridItems.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridItems.EditableEnterMovesDown = true;
			this.gridItems.Location = new System.Drawing.Point(12, 227);
			this.gridItems.Name = "gridItems";
			this.gridItems.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridItems.Size = new System.Drawing.Size(647, 438);
			this.gridItems.TabIndex = 17;
			this.gridItems.Title = "Supplies on One Order";
			this.gridItems.TranslationName = "TableSupplies";
			this.gridItems.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridOrderItem_CellDoubleClick);
			this.gridItems.CellLeave += new OpenDental.UI.ODGridClickEventHandler(this.gridItems_CellLeave);
			// 
			// gridOrders
			// 
			this.gridOrders.Location = new System.Drawing.Point(12, 37);
			this.gridOrders.Name = "gridOrders";
			this.gridOrders.Size = new System.Drawing.Size(737, 184);
			this.gridOrders.TabIndex = 15;
			this.gridOrders.Title = "Order History";
			this.gridOrders.TranslationName = "TableHistory";
			this.gridOrders.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridOrder_CellDoubleClick);
			this.gridOrders.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridOrder_CellClick);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(170, 672);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(80, 24);
			this.butPrint.TabIndex = 26;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butAddSupply
			// 
			this.butAddSupply.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddSupply.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddSupply.Location = new System.Drawing.Point(665, 227);
			this.butAddSupply.Name = "butAddSupply";
			this.butAddSupply.Size = new System.Drawing.Size(69, 24);
			this.butAddSupply.TabIndex = 25;
			this.butAddSupply.Text = "Add";
			this.butAddSupply.Click += new System.EventHandler(this.butAddSupply_Click);
			// 
			// butNewOrder
			// 
			this.butNewOrder.Icon = OpenDental.UI.EnumIcons.Add;
			this.butNewOrder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butNewOrder.Location = new System.Drawing.Point(12, 7);
			this.butNewOrder.Name = "butNewOrder";
			this.butNewOrder.Size = new System.Drawing.Size(95, 24);
			this.butNewOrder.TabIndex = 16;
			this.butNewOrder.Text = "New Order";
			this.butNewOrder.Click += new System.EventHandler(this.butNewOrder_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(678, 672);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butClose_Click);
			// 
			// checkShowReceived
			// 
			this.checkShowReceived.AutoSize = true;
			this.checkShowReceived.Location = new System.Drawing.Point(360, 12);
			this.checkShowReceived.Name = "checkShowReceived";
			this.checkShowReceived.Size = new System.Drawing.Size(97, 17);
			this.checkShowReceived.TabIndex = 27;
			this.checkShowReceived.Text = "Show received";
			this.checkShowReceived.UseVisualStyleBackColor = true;
			this.checkShowReceived.MouseClick += new System.Windows.Forms.MouseEventHandler(this.checkShowReceived_MouseClick);
			// 
			// FormSupplyOrders
			// 
			this.ClientSize = new System.Drawing.Size(761, 703);
			this.Controls.Add(this.checkShowReceived);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butAddSupply);
			this.Controls.Add(this.gridItems);
			this.Controls.Add(this.butNewOrder);
			this.Controls.Add(this.gridOrders);
			this.Controls.Add(this.comboSupplier);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = false;
			this.Name = "FormSupplyOrders";
			this.Text = "Supply Orders";
			this.Load += new System.EventHandler(this.FormSupplyOrders_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.Button butCancel;
		private UI.Button butNewOrder;
		private UI.GridOD gridOrders;
		private UI.ComboBoxOD comboSupplier;
		private System.Windows.Forms.Label label3;
		private UI.GridOD gridItems;
		private UI.Button butAddSupply;
		private UI.Button butPrint;
		private System.Windows.Forms.CheckBox checkShowReceived;
	}
}