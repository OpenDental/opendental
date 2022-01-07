using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormCarriers {
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCarriers));
			this.butAdd = new OpenDental.UI.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.butCombine = new OpenDental.UI.Button();
			this.butItransUpdateCarriers = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.checkCDAnet = new System.Windows.Forms.CheckBox();
			this.checkShowHidden = new System.Windows.Forms.CheckBox();
			this.textCarrier = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.textPhone = new OpenDental.ValidPhone();
			this.labelPhone = new System.Windows.Forms.Label();
			this.butRefresh = new OpenDental.UI.Button();
			this.groupItrans = new System.Windows.Forms.GroupBox();
			this.checkItransMissing = new System.Windows.Forms.CheckBox();
			this.checkItransName = new System.Windows.Forms.CheckBox();
			this.checkItransAddress = new System.Windows.Forms.CheckBox();
			this.checkITransPhone = new System.Windows.Forms.CheckBox();
			this.textElectId = new System.Windows.Forms.TextBox();
			this.labelElectId = new System.Windows.Forms.Label();
			this.groupItrans.SuspendLayout();
			this.SuspendLayout();
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(830, 435);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(90, 26);
			this.butAdd.TabIndex = 7;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butCombine
			// 
			this.butCombine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCombine.Location = new System.Drawing.Point(830, 471);
			this.butCombine.Name = "butCombine";
			this.butCombine.Size = new System.Drawing.Size(90, 26);
			this.butCombine.TabIndex = 8;
			this.butCombine.Text = "Co&mbine";
			this.toolTip1.SetToolTip(this.butCombine, "Combines multiple Employers");
			this.butCombine.Click += new System.EventHandler(this.butCombine_Click);
			// 
			// butItransUpdateCarriers
			// 
			this.butItransUpdateCarriers.Location = new System.Drawing.Point(8, 94);
			this.butItransUpdateCarriers.Name = "butItransUpdateCarriers";
			this.butItransUpdateCarriers.Size = new System.Drawing.Size(90, 26);
			this.butItransUpdateCarriers.TabIndex = 4;
			this.butItransUpdateCarriers.Text = "Update Carriers";
			this.toolTip1.SetToolTip(this.butItransUpdateCarriers, "Updates carriers using iTrans 2.0");
			this.butItransUpdateCarriers.Click += new System.EventHandler(this.butItransUpdateCarriers_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(830, 623);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(90, 26);
			this.butCancel.TabIndex = 11;
			this.butCancel.Text = "Close";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(11, 29);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(796, 620);
			this.gridMain.TabIndex = 9;
			this.gridMain.Title = "Carriers";
			this.gridMain.TranslationName = "TableCarriers";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// checkCDAnet
			// 
			this.checkCDAnet.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkCDAnet.Location = new System.Drawing.Point(633, 6);
			this.checkCDAnet.Name = "checkCDAnet";
			this.checkCDAnet.Size = new System.Drawing.Size(96, 17);
			this.checkCDAnet.TabIndex = 4;
			this.checkCDAnet.Text = "CDAnet Only";
			this.checkCDAnet.Click += new System.EventHandler(this.checkCDAnet_Click);
			// 
			// checkShowHidden
			// 
			this.checkShowHidden.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowHidden.Location = new System.Drawing.Point(527, 6);
			this.checkShowHidden.Name = "checkShowHidden";
			this.checkShowHidden.Size = new System.Drawing.Size(96, 17);
			this.checkShowHidden.TabIndex = 3;
			this.checkShowHidden.Text = "Show Hidden";
			this.checkShowHidden.Click += new System.EventHandler(this.checkShowHidden_Click);
			// 
			// textCarrier
			// 
			this.textCarrier.Location = new System.Drawing.Point(67, 4);
			this.textCarrier.Name = "textCarrier";
			this.textCarrier.Size = new System.Drawing.Size(140, 20);
			this.textCarrier.TabIndex = 0;
			this.textCarrier.TextChanged += new System.EventHandler(this.textCarrier_TextChanged);
			// 
			// label2
			// 
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label2.Location = new System.Drawing.Point(13, 7);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 17);
			this.label2.TabIndex = 102;
			this.label2.Text = "Carrier";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(830, 587);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(90, 26);
			this.butOK.TabIndex = 10;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// textPhone
			// 
			this.textPhone.Location = new System.Drawing.Point(381, 4);
			this.textPhone.Name = "textPhone";
			this.textPhone.Size = new System.Drawing.Size(140, 20);
			this.textPhone.TabIndex = 2;
			this.textPhone.TextChanged += new System.EventHandler(this.textPhone_TextChanged);
			// 
			// labelPhone
			// 
			this.labelPhone.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelPhone.Location = new System.Drawing.Point(331, 7);
			this.labelPhone.Name = "labelPhone";
			this.labelPhone.Size = new System.Drawing.Size(49, 17);
			this.labelPhone.TabIndex = 105;
			this.labelPhone.Text = "Phone";
			this.labelPhone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(732, 2);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 23);
			this.butRefresh.TabIndex = 5;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// groupItrans
			// 
			this.groupItrans.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupItrans.Controls.Add(this.checkItransMissing);
			this.groupItrans.Controls.Add(this.checkItransName);
			this.groupItrans.Controls.Add(this.checkItransAddress);
			this.groupItrans.Controls.Add(this.checkITransPhone);
			this.groupItrans.Controls.Add(this.butItransUpdateCarriers);
			this.groupItrans.Location = new System.Drawing.Point(812, 29);
			this.groupItrans.Name = "groupItrans";
			this.groupItrans.Size = new System.Drawing.Size(106, 125);
			this.groupItrans.TabIndex = 6;
			this.groupItrans.TabStop = false;
			this.groupItrans.Text = "Web Import";
			this.groupItrans.Visible = false;
			// 
			// checkItransMissing
			// 
			this.checkItransMissing.Location = new System.Drawing.Point(8, 73);
			this.checkItransMissing.Name = "checkItransMissing";
			this.checkItransMissing.Size = new System.Drawing.Size(90, 17);
			this.checkItransMissing.TabIndex = 3;
			this.checkItransMissing.Text = "Add Missing";
			this.checkItransMissing.UseVisualStyleBackColor = true;
			// 
			// checkItransName
			// 
			this.checkItransName.Location = new System.Drawing.Point(8, 19);
			this.checkItransName.Name = "checkItransName";
			this.checkItransName.Size = new System.Drawing.Size(90, 17);
			this.checkItransName.TabIndex = 0;
			this.checkItransName.Text = "Name";
			this.checkItransName.UseVisualStyleBackColor = true;
			// 
			// checkItransAddress
			// 
			this.checkItransAddress.Location = new System.Drawing.Point(8, 37);
			this.checkItransAddress.Name = "checkItransAddress";
			this.checkItransAddress.Size = new System.Drawing.Size(90, 16);
			this.checkItransAddress.TabIndex = 1;
			this.checkItransAddress.Text = "Address";
			this.checkItransAddress.UseVisualStyleBackColor = true;
			// 
			// checkITransPhone
			// 
			this.checkITransPhone.Location = new System.Drawing.Point(8, 55);
			this.checkITransPhone.Name = "checkITransPhone";
			this.checkITransPhone.Size = new System.Drawing.Size(90, 17);
			this.checkITransPhone.TabIndex = 2;
			this.checkITransPhone.Text = "Phone";
			this.checkITransPhone.UseVisualStyleBackColor = true;
			// 
			// textElectId
			// 
			this.textElectId.Location = new System.Drawing.Point(266, 4);
			this.textElectId.Name = "textElectId";
			this.textElectId.Size = new System.Drawing.Size(59, 20);
			this.textElectId.TabIndex = 1;
			this.textElectId.TextChanged += new System.EventHandler(this.textElectId_TextChanged);
			// 
			// labelElectId
			// 
			this.labelElectId.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelElectId.Location = new System.Drawing.Point(212, 7);
			this.labelElectId.Name = "labelElectId";
			this.labelElectId.Size = new System.Drawing.Size(53, 17);
			this.labelElectId.TabIndex = 111;
			this.labelElectId.Text = "ElectID";
			this.labelElectId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormCarriers
			// 
			this.ClientSize = new System.Drawing.Size(927, 672);
			this.Controls.Add(this.textElectId);
			this.Controls.Add(this.labelElectId);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupItrans);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.textPhone);
			this.Controls.Add(this.labelPhone);
			this.Controls.Add(this.textCarrier);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.checkShowHidden);
			this.Controls.Add(this.checkCDAnet);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butCombine);
			this.Controls.Add(this.butAdd);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormCarriers";
			this.ShowInTaskbar = false;
			this.Text = "Carriers";
			this.Load += new System.EventHandler(this.FormCarriers_Load);
			this.groupItrans.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butAdd;
		private System.Windows.Forms.ToolTip toolTip1;
		private OpenDental.UI.Button butCombine;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.GridOD gridMain;
		private CheckBox checkCDAnet;
		private CheckBox checkShowHidden;
		public TextBox textCarrier;
		private Label label2;
		private UI.Button butOK;//keeps track of whether an update is necessary.
		public ValidPhone textPhone;
		private Label labelPhone;
		private UI.Button butRefresh;
		private UI.Button butItransUpdateCarriers;
		private GroupBox groupItrans;
		private CheckBox checkITransPhone;
		private CheckBox checkItransName;
		private CheckBox checkItransAddress;
		private CheckBox checkItransMissing;
		public TextBox textElectId;
		private Label labelElectId;
	}
}
