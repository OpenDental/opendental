namespace OpenDental{
	partial class FormOrthoChart {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOrthoChart));
			this.butPrint = new OpenDental.UI.Button();
			this.gridOrtho = new OpenDental.UI.GridOD();
			this.tabControl = new OpenDental.UI.TabControl();
			this.tabPage1 = new OpenDental.UI.TabPage();
			this.tabPage2 = new OpenDental.UI.TabPage();
			this.signatureBoxWrapper = new OpenDental.UI.SignatureBoxWrapper();
			this.butOK = new OpenDental.UI.Button();
			this.butAudit = new OpenDental.UI.Button();
			this.butUseAutoNote = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.gridPat = new OpenDental.UI.GridOD();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butChangeUser = new OpenDental.UI.Button();
			this.textUser = new System.Windows.Forms.TextBox();
			this.labelUser = new System.Windows.Forms.Label();
			this.labelPermAlert = new System.Windows.Forms.Label();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.butDelete = new OpenDental.UI.Button();
			this.labelLocked = new System.Windows.Forms.Label();
			this.timerLocked = new System.Windows.Forms.Timer(this.components);
			this.buttonTakeControl = new OpenDental.UI.Button();
			this.textDateRange = new System.Windows.Forms.TextBox();
			this.butShowDateRange = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.tabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(725, 654);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(85, 25);
			this.butPrint.TabIndex = 118;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// gridOrtho
			// 
			this.gridOrtho.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gridOrtho.HeadersVisible = false;
			this.gridOrtho.Location = new System.Drawing.Point(640, 31);
			this.gridOrtho.Name = "gridOrtho";
			this.gridOrtho.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridOrtho.Size = new System.Drawing.Size(364, 148);
			this.gridOrtho.TabIndex = 112;
			this.gridOrtho.Title = "Ortho Info";
			this.gridOrtho.TranslationName = "TableOrthoInfo";
			this.gridOrtho.Visible = false;
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tabPage1);
			this.tabControl.Controls.Add(this.tabPage2);
			this.tabControl.Location = new System.Drawing.Point(10, 320);
			this.tabControl.Name = "tabControl";
			this.tabControl.Size = new System.Drawing.Size(992, 21);
			this.tabControl.TabIndex = 111;
			this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.Location = new System.Drawing.Point(2, 21);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(988, 0);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "tabPage1";
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(2, 21);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(988, 0);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "tabPage2";
			// 
			// signatureBoxWrapper
			// 
			this.signatureBoxWrapper.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.signatureBoxWrapper.BackColor = System.Drawing.SystemColors.ControlDark;
			this.signatureBoxWrapper.Location = new System.Drawing.Point(640, 215);
			this.signatureBoxWrapper.Name = "signatureBoxWrapper";
			this.signatureBoxWrapper.SignatureMode = OpenDental.UI.SignatureBoxWrapper.SigMode.OrthoChart;
			this.signatureBoxWrapper.Size = new System.Drawing.Size(364, 81);
			this.signatureBoxWrapper.TabIndex = 110;
			this.signatureBoxWrapper.UserSig = null;
			this.signatureBoxWrapper.ClearSignatureClicked += new System.EventHandler(this.signatureBoxWrapper_ClearSignatureClicked);
			this.signatureBoxWrapper.SignTopazClicked += new System.EventHandler(this.signatureBoxWrapper_SignTopazClicked);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(846, 654);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 109;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butAudit
			// 
			this.butAudit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAudit.Location = new System.Drawing.Point(578, 654);
			this.butAudit.Name = "butAudit";
			this.butAudit.Size = new System.Drawing.Size(80, 24);
			this.butAudit.TabIndex = 108;
			this.butAudit.Text = "Audit Trail";
			this.butAudit.Click += new System.EventHandler(this.butAudit_Click);
			// 
			// butUseAutoNote
			// 
			this.butUseAutoNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butUseAutoNote.Location = new System.Drawing.Point(96, 654);
			this.butUseAutoNote.Name = "butUseAutoNote";
			this.butUseAutoNote.Size = new System.Drawing.Size(80, 24);
			this.butUseAutoNote.TabIndex = 107;
			this.butUseAutoNote.Text = "Auto Note";
			this.butUseAutoNote.Click += new System.EventHandler(this.butUseAutoNote_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Location = new System.Drawing.Point(10, 654);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(80, 24);
			this.butAdd.TabIndex = 9;
			this.butAdd.Text = "Add Row";
			this.butAdd.UseVisualStyleBackColor = true;
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(927, 654);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 7;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// gridPat
			// 
			this.gridPat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridPat.Location = new System.Drawing.Point(10, 31);
			this.gridPat.Name = "gridPat";
			this.gridPat.Size = new System.Drawing.Size(622, 263);
			this.gridPat.TabIndex = 6;
			this.gridPat.Title = "Patient Fields";
			this.gridPat.TranslationName = "TablePatientFields";
			this.gridPat.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPat_CellDoubleClick);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(10, 340);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridMain.Size = new System.Drawing.Size(992, 303);
			this.gridMain.TabIndex = 5;
			this.gridMain.Title = "Ortho Chart";
			this.gridMain.TranslationName = "TableOrthoChart";
			this.gridMain.CellSelectionCommitted += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellSelectionCommitted);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			this.gridMain.CellLeave += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellLeave);
			this.gridMain.CellEnter += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellEnter);
			// 
			// butChangeUser
			// 
			this.butChangeUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butChangeUser.Location = new System.Drawing.Point(794, 190);
			this.butChangeUser.Name = "butChangeUser";
			this.butChangeUser.Size = new System.Drawing.Size(91, 24);
			this.butChangeUser.TabIndex = 185;
			this.butChangeUser.Text = "Change User";
			this.butChangeUser.Click += new System.EventHandler(this.butChangeUser_Click);
			// 
			// textUser
			// 
			this.textUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textUser.Location = new System.Drawing.Point(676, 193);
			this.textUser.Name = "textUser";
			this.textUser.ReadOnly = true;
			this.textUser.Size = new System.Drawing.Size(116, 20);
			this.textUser.TabIndex = 184;
			// 
			// labelUser
			// 
			this.labelUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelUser.Location = new System.Drawing.Point(633, 194);
			this.labelUser.Name = "labelUser";
			this.labelUser.Size = new System.Drawing.Size(41, 16);
			this.labelUser.TabIndex = 183;
			this.labelUser.Text = "User";
			this.labelUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPermAlert
			// 
			this.labelPermAlert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPermAlert.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPermAlert.ForeColor = System.Drawing.Color.DarkRed;
			this.labelPermAlert.Location = new System.Drawing.Point(638, 299);
			this.labelPermAlert.Name = "labelPermAlert";
			this.labelPermAlert.Size = new System.Drawing.Size(282, 20);
			this.labelPermAlert.TabIndex = 211;
			this.labelPermAlert.Text = "Notes can only be signed by providers.";
			this.labelPermAlert.Visible = false;
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(1014, 24);
			this.menuMain.TabIndex = 212;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(182, 654);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(80, 24);
			this.butDelete.TabIndex = 213;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// labelLocked
			// 
			this.labelLocked.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelLocked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelLocked.ForeColor = System.Drawing.Color.DarkRed;
			this.labelLocked.Location = new System.Drawing.Point(285, 660);
			this.labelLocked.Name = "labelLocked";
			this.labelLocked.Size = new System.Drawing.Size(142, 20);
			this.labelLocked.TabIndex = 214;
			this.labelLocked.Text = "Locked by UserName";
			// 
			// timerLocked
			// 
			this.timerLocked.Enabled = true;
			this.timerLocked.Interval = 4000;
			this.timerLocked.Tick += new System.EventHandler(this.timerLocked_Tick);
			// 
			// buttonTakeControl
			// 
			this.buttonTakeControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonTakeControl.Location = new System.Drawing.Point(443, 654);
			this.buttonTakeControl.Name = "buttonTakeControl";
			this.buttonTakeControl.Size = new System.Drawing.Size(85, 24);
			this.buttonTakeControl.TabIndex = 215;
			this.buttonTakeControl.Text = "Take Control";
			this.buttonTakeControl.Click += new System.EventHandler(this.buttonTakeControl_Click);
			// 
			// textDateRange
			// 
			this.textDateRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDateRange.BackColor = System.Drawing.SystemColors.Control;
			this.textDateRange.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
			this.textDateRange.Location = new System.Drawing.Point(460, 298);
			this.textDateRange.Name = "textDateRange";
			this.textDateRange.ReadOnly = true;
			this.textDateRange.Size = new System.Drawing.Size(148, 19);
			this.textDateRange.TabIndex = 216;
			// 
			// butShowDateRange
			// 
			this.butShowDateRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butShowDateRange.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
			this.butShowDateRange.Location = new System.Drawing.Point(609, 296);
			this.butShowDateRange.Name = "butShowDateRange";
			this.butShowDateRange.Size = new System.Drawing.Size(24, 22);
			this.butShowDateRange.TabIndex = 217;
			this.butShowDateRange.Text = "...";
			this.butShowDateRange.UseVisualStyleBackColor = true;
			this.butShowDateRange.Click += new System.EventHandler(this.butShowDateRange_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(383, 300);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(76, 16);
			this.label1.TabIndex = 218;
			this.label1.Text = "Date Range";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormOrthoChart
			// 
			this.ClientSize = new System.Drawing.Size(1014, 687);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butShowDateRange);
			this.Controls.Add(this.textDateRange);
			this.Controls.Add(this.buttonTakeControl);
			this.Controls.Add(this.labelLocked);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelPermAlert);
			this.Controls.Add(this.butChangeUser);
			this.Controls.Add(this.textUser);
			this.Controls.Add(this.labelUser);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.gridOrtho);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.signatureBoxWrapper);
			this.Controls.Add(this.butAudit);
			this.Controls.Add(this.butUseAutoNote);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridPat);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormOrthoChart";
			this.Text = "Ortho Chart";
			this.CloseXClicked += new System.ComponentModel.CancelEventHandler(this.FormOrthoChart_CloseXClicked);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormOrthoChart_FormClosing);
			this.Load += new System.EventHandler(this.FormOrthoChart_Load);
			this.tabControl.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.Button butAdd;
		private UI.Button butCancel;
		private UI.GridOD gridPat;
		private UI.GridOD gridMain;
		private UI.Button butUseAutoNote;
		private UI.Button butAudit;
		private UI.Button butOK;
		private UI.SignatureBoxWrapper signatureBoxWrapper;
		private OpenDental.UI.TabControl tabControl;
		private OpenDental.UI.TabPage tabPage1;
		private OpenDental.UI.TabPage tabPage2;
		private UI.GridOD gridOrtho;
		private UI.Button butPrint;
		private UI.Button butChangeUser;
		private System.Windows.Forms.TextBox textUser;
		private System.Windows.Forms.Label labelUser;
		private System.Windows.Forms.Label labelPermAlert;
		private UI.MenuOD menuMain;
		private UI.Button butDelete;
		private System.Windows.Forms.Label labelLocked;
		private System.Windows.Forms.Timer timerLocked;
		private UI.Button buttonTakeControl;
		private System.Windows.Forms.TextBox textDateRange;
		private UI.Button butShowDateRange;
		private System.Windows.Forms.Label label1;
	}
}