namespace OpenDental{
	partial class FormExamSheets {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExamSheets));
			this.listExamTypes = new OpenDental.UI.ListBoxOD();
			this.labelFilterTypes = new System.Windows.Forms.Label();
			this.butAdd = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butCancel = new OpenDental.UI.Button();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.SuspendLayout();
			// 
			// listExamTypes
			// 
			this.listExamTypes.Location = new System.Drawing.Point(12, 44);
			this.listExamTypes.Name = "listExamTypes";
			this.listExamTypes.Size = new System.Drawing.Size(196, 43);
			this.listExamTypes.TabIndex = 48;
			this.listExamTypes.SelectedIndexChanged += new System.EventHandler(this.listExamTypes_SelectedIndexChanged);
			// 
			// labelFilterTypes
			// 
			this.labelFilterTypes.Location = new System.Drawing.Point(10, 26);
			this.labelFilterTypes.Name = "labelFilterTypes";
			this.labelFilterTypes.Size = new System.Drawing.Size(156, 16);
			this.labelFilterTypes.TabIndex = 49;
			this.labelFilterTypes.Text = "Show Types Filter";
			this.labelFilterTypes.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(12, 489);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 6;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 93);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(397, 390);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = "Exam Sheets";
			this.gridMain.TranslationName = "FormPatientForms";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(334, 489);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "Close";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(441, 24);
			this.menuMain.TabIndex = 50;
			// 
			// FormExamSheets
			// 
			this.ClientSize = new System.Drawing.Size(441, 525);
			this.Controls.Add(this.labelFilterTypes);
			this.Controls.Add(this.listExamTypes);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormExamSheets";
			this.Text = "Exam Sheets";
			this.Load += new System.EventHandler(this.FormExamSheets_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.ListBoxOD listExamTypes;
		private System.Windows.Forms.Label labelFilterTypes;
		private UI.MenuOD menuMain;
	}
}