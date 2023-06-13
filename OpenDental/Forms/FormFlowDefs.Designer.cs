namespace OpenDental{
	partial class FormERoutingDefs {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormERoutingDefs));
			this.butClose = new OpenDental.UI.Button();
			this.gridERouting = new OpenDental.UI.GridOD();
			this.butAdd = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.checkUseDefault = new System.Windows.Forms.CheckBox();
			this.butDuplicate = new OpenDental.UI.Button();
			this.labelUseDefaults = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(401, 540);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridERouting
			// 
			this.gridERouting.Location = new System.Drawing.Point(103, 74);
			this.gridERouting.Name = "gridERouting";
			this.gridERouting.Size = new System.Drawing.Size(242, 434);
			this.gridERouting.TabIndex = 4;
			this.gridERouting.Title = "eRouting Defs";
			this.gridERouting.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.GridCell_DoubleClick);
			// 
			// butAdd
			// 
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(351, 74);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 5;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "HQ Default";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(66, 22);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(200, 21);
			this.comboClinic.TabIndex = 0;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.ComboClinic_SelectionChangeCommitted);
			// 
			// checkUseDefault
			// 
			this.checkUseDefault.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseDefault.Location = new System.Drawing.Point(8, 49);
			this.checkUseDefault.Name = "checkUseDefault";
			this.checkUseDefault.Size = new System.Drawing.Size(109, 18);
			this.checkUseDefault.TabIndex = 6;
			this.checkUseDefault.Text = "Use Default";
			this.checkUseDefault.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseDefault.UseVisualStyleBackColor = true;
			this.checkUseDefault.Click += new System.EventHandler(this.checkUseDefault_Click);
			// 
			// butDuplicate
			// 
			this.butDuplicate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDuplicate.Location = new System.Drawing.Point(351, 113);
			this.butDuplicate.Name = "butDuplicate";
			this.butDuplicate.Size = new System.Drawing.Size(75, 24);
			this.butDuplicate.TabIndex = 7;
			this.butDuplicate.Text = "&Duplicate";
			this.butDuplicate.Click += new System.EventHandler(this.butDuplicate_ClicK);
			// 
			// labelUseDefaults
			// 
			this.labelUseDefaults.ForeColor = System.Drawing.Color.Firebrick;
			this.labelUseDefaults.Location = new System.Drawing.Point(166, 45);
			this.labelUseDefaults.Name = "labelUseDefaults";
			this.labelUseDefaults.Size = new System.Drawing.Size(167, 23);
			this.labelUseDefaults.TabIndex = 8;
			this.labelUseDefaults.Text = "Using Defaults";
			this.labelUseDefaults.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelUseDefaults.Visible = false;
			// 
			// FormERoutingDefs
			// 
			this.ClientSize = new System.Drawing.Size(488, 576);
			this.Controls.Add(this.labelUseDefaults);
			this.Controls.Add(this.butDuplicate);
			this.Controls.Add(this.checkUseDefault);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridERouting);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormERoutingDefs";
			this.Text = "eRouting Defs";
			this.Load += new System.EventHandler(this.FormERouting_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridERouting;
		private UI.Button butAdd;
		private UI.ComboBoxClinicPicker comboClinic;
		private System.Windows.Forms.CheckBox checkUseDefault;
		private UI.Button butDuplicate;
		private System.Windows.Forms.Label labelUseDefaults;
	}
}