namespace OpenDental{
	partial class FormSupplies {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSupplies));
			this.textFind = new System.Windows.Forms.TextBox();
			this.butUp = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.labelSuppliers = new System.Windows.Forms.Label();
			this.comboSuppliers = new OpenDental.UI.ComboBoxOD();
			this.checkShowHidden = new System.Windows.Forms.CheckBox();
			this.butAdd = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.label2 = new System.Windows.Forms.Label();
			this.comboCategories = new OpenDental.UI.ComboBoxOD();
			this.labelPrint = new System.Windows.Forms.Label();
			this.labelCreateOrder = new System.Windows.Forms.Label();
			this.butCreateOrders = new OpenDental.UI.Button();
			this.groupCreateOrders = new System.Windows.Forms.GroupBox();
			this.butCreateOrdersQty = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.checkEnterQty = new System.Windows.Forms.CheckBox();
			this.timerFillGrid = new System.Windows.Forms.Timer(this.components);
			this.groupCreateOrders.SuspendLayout();
			this.SuspendLayout();
			// 
			// textFind
			// 
			this.textFind.Location = new System.Drawing.Point(303, 3);
			this.textFind.Name = "textFind";
			this.textFind.Size = new System.Drawing.Size(168, 20);
			this.textFind.TabIndex = 1;
			this.textFind.TextChanged += new System.EventHandler(this.textFind_TextChanged);
			// 
			// butUp
			// 
			this.butUp.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(12, 592);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(67, 24);
			this.butUp.TabIndex = 28;
			this.butUp.Text = "Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butDown
			// 
			this.butDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(12, 622);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(67, 24);
			this.butDown.TabIndex = 29;
			this.butDown.Text = "Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(594, 622);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 27;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(675, 622);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 26;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(122, 622);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(80, 24);
			this.butPrint.TabIndex = 25;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(253, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(49, 18);
			this.label1.TabIndex = 18;
			this.label1.Text = "Search";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSuppliers
			// 
			this.labelSuppliers.Location = new System.Drawing.Point(4, 5);
			this.labelSuppliers.Name = "labelSuppliers";
			this.labelSuppliers.Size = new System.Drawing.Size(67, 18);
			this.labelSuppliers.TabIndex = 14;
			this.labelSuppliers.Text = "Suppliers";
			this.labelSuppliers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSuppliers
			// 
			this.comboSuppliers.Location = new System.Drawing.Point(73, 4);
			this.comboSuppliers.Name = "comboSuppliers";
			this.comboSuppliers.SelectionModeMulti = true;
			this.comboSuppliers.Size = new System.Drawing.Size(170, 21);
			this.comboSuppliers.TabIndex = 13;
			this.comboSuppliers.SelectionChangeCommitted += new System.EventHandler(this.comboSupplier_SelectionChangeCommitted);
			// 
			// checkShowHidden
			// 
			this.checkShowHidden.Location = new System.Drawing.Point(477, 5);
			this.checkShowHidden.Name = "checkShowHidden";
			this.checkShowHidden.Size = new System.Drawing.Size(99, 18);
			this.checkShowHidden.TabIndex = 12;
			this.checkShowHidden.Text = "Show Hidden";
			this.checkShowHidden.UseVisualStyleBackColor = true;
			this.checkShowHidden.Click += new System.EventHandler(this.checkShowHidden_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(657, 23);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(93, 24);
			this.butAdd.TabIndex = 11;
			this.butAdd.Text = "Add Supply";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 51);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(738, 490);
			this.gridMain.TabIndex = 5;
			this.gridMain.Title = "Supplies";
			this.gridMain.TranslationName = "TableSupplies";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellLeave += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellLeave);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(4, 27);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 18);
			this.label2.TabIndex = 32;
			this.label2.Text = "Categories";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboCategories
			// 
			this.comboCategories.Location = new System.Drawing.Point(73, 26);
			this.comboCategories.Name = "comboCategories";
			this.comboCategories.SelectionModeMulti = true;
			this.comboCategories.Size = new System.Drawing.Size(170, 21);
			this.comboCategories.TabIndex = 31;
			this.comboCategories.SelectionChangeCommitted += new System.EventHandler(this.comboCategories_SelectionChangeCommitted);
			// 
			// labelPrint
			// 
			this.labelPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelPrint.Location = new System.Drawing.Point(206, 621);
			this.labelPrint.Name = "labelPrint";
			this.labelPrint.Size = new System.Drawing.Size(344, 30);
			this.labelPrint.TabIndex = 33;
			this.labelPrint.Text = "It\'s common to print a filtered list to use for taking inventory, regardless of w" +
    "hether that order then gets entered back into the software";
			// 
			// labelCreateOrder
			// 
			this.labelCreateOrder.Location = new System.Drawing.Point(89, 23);
			this.labelCreateOrder.Name = "labelCreateOrder";
			this.labelCreateOrder.Size = new System.Drawing.Size(184, 18);
			this.labelCreateOrder.TabIndex = 35;
			this.labelCreateOrder.Text = "from all the selected supplies";
			// 
			// butCreateOrders
			// 
			this.butCreateOrders.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCreateOrders.Location = new System.Drawing.Point(6, 18);
			this.butCreateOrders.Name = "butCreateOrders";
			this.butCreateOrders.Size = new System.Drawing.Size(81, 24);
			this.butCreateOrders.TabIndex = 34;
			this.butCreateOrders.Text = "Selected";
			this.butCreateOrders.Click += new System.EventHandler(this.butCreateOrders_Click);
			// 
			// groupCreateOrders
			// 
			this.groupCreateOrders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupCreateOrders.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.groupCreateOrders.Controls.Add(this.butCreateOrdersQty);
			this.groupCreateOrders.Controls.Add(this.label3);
			this.groupCreateOrders.Controls.Add(this.butCreateOrders);
			this.groupCreateOrders.Controls.Add(this.labelCreateOrder);
			this.groupCreateOrders.Location = new System.Drawing.Point(115, 544);
			this.groupCreateOrders.Name = "groupCreateOrders";
			this.groupCreateOrders.Size = new System.Drawing.Size(316, 72);
			this.groupCreateOrders.TabIndex = 36;
			this.groupCreateOrders.Text = "Create new Orders, one for each supplier";
			// 
			// butCreateOrdersQty
			// 
			this.butCreateOrdersQty.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCreateOrdersQty.Location = new System.Drawing.Point(6, 44);
			this.butCreateOrdersQty.Name = "butCreateOrdersQty";
			this.butCreateOrdersQty.Size = new System.Drawing.Size(81, 24);
			this.butCreateOrdersQty.TabIndex = 36;
			this.butCreateOrdersQty.Text = "With Qty";
			this.butCreateOrdersQty.Click += new System.EventHandler(this.butCreateOrdersQty_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(89, 49);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(220, 18);
			this.label3.TabIndex = 37;
			this.label3.Text = "from all supplies with Quantities entered";
			// 
			// checkEnterQty
			// 
			this.checkEnterQty.Location = new System.Drawing.Point(477, 26);
			this.checkEnterQty.Name = "checkEnterQty";
			this.checkEnterQty.Size = new System.Drawing.Size(121, 18);
			this.checkEnterQty.TabIndex = 37;
			this.checkEnterQty.Text = "Enter Quantities";
			this.checkEnterQty.UseVisualStyleBackColor = true;
			this.checkEnterQty.Click += new System.EventHandler(this.checkEnterQty_Click);
			// 
			// timerFillGrid
			// 
			this.timerFillGrid.Interval = 1000;
			this.timerFillGrid.Tick += new System.EventHandler(this.timerFillGrid_Tick);
			// 
			// FormSupplies
			// 
			this.ClientSize = new System.Drawing.Size(762, 657);
			this.Controls.Add(this.checkEnterQty);
			this.Controls.Add(this.groupCreateOrders);
			this.Controls.Add(this.labelPrint);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboCategories);
			this.Controls.Add(this.textFind);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelSuppliers);
			this.Controls.Add(this.comboSuppliers);
			this.Controls.Add(this.checkShowHidden);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSupplies";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Supplies";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSupplies_FormClosing);
			this.Load += new System.EventHandler(this.FormSupplies_Load);
			this.groupCreateOrders.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.GridOD gridMain;
		private System.Windows.Forms.Label labelSuppliers;
		private UI.ComboBoxOD comboSuppliers;
		private System.Windows.Forms.CheckBox checkShowHidden;
		private OpenDental.UI.Button butAdd;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textFind;
		private UI.Button butPrint;
		private UI.Button butOK;
		private UI.Button butCancel;
		private UI.Button butUp;
		private UI.Button butDown;
		private System.Windows.Forms.Label label2;
		private UI.ComboBoxOD comboCategories;
		private System.Windows.Forms.Label labelPrint;
		private System.Windows.Forms.Label labelCreateOrder;
		private UI.Button butCreateOrders;
		private System.Windows.Forms.GroupBox groupCreateOrders;
		private UI.Button butCreateOrdersQty;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkEnterQty;
		private System.Windows.Forms.Timer timerFillGrid;
	}
}