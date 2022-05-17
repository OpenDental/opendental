namespace OpenDental{
	partial class FormVoiceMails {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVoiceMails));
			this.menuVoiceMailsRightClick = new System.Windows.Forms.ContextMenu();
			this.menuSendToMe = new System.Windows.Forms.MenuItem();
			this.menuSendToMeCreateTask = new System.Windows.Forms.MenuItem();
			this.menuGoToChart = new System.Windows.Forms.MenuItem();
			this.menuGoogleNum = new System.Windows.Forms.MenuItem();
			this.checkShowDeleted = new System.Windows.Forms.CheckBox();
			this.labelError = new System.Windows.Forms.Label();
			this.labelDuration = new System.Windows.Forms.Label();
			this.axWindowsMediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
			this.gridVoiceMails = new OpenDental.UI.GridOD();
			this.butCreateTask = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer)).BeginInit();
			this.SuspendLayout();
			// 
			// menuVoiceMailsRightClick
			// 
			this.menuVoiceMailsRightClick.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuSendToMe,
            this.menuSendToMeCreateTask,
            this.menuGoToChart,
            this.menuGoogleNum});
			this.menuVoiceMailsRightClick.Popup += new System.EventHandler(this.menuVoiceMailsRightClick_Popup);
			// 
			// menuSendToMe
			// 
			this.menuSendToMe.Index = 0;
			this.menuSendToMe.Text = "Send to Me";
			this.menuSendToMe.Click += new System.EventHandler(this.menuSendToMe_Click);
			// 
			// menuSendToMeCreateTask
			// 
			this.menuSendToMeCreateTask.Index = 1;
			this.menuSendToMeCreateTask.Text = "Send to Me && Create Task";
			this.menuSendToMeCreateTask.Click += new System.EventHandler(this.menuSendToMeCreateTask_Click);
			// 
			// menuGoToChart
			// 
			this.menuGoToChart.Index = 2;
			this.menuGoToChart.Text = "Go to Chart";
			this.menuGoToChart.Click += new System.EventHandler(this.menuGoToChart_Click);
			// 
			// menuGoogleNum
			// 
			this.menuGoogleNum.Index = 3;
			this.menuGoogleNum.Text = "Google Number";
			this.menuGoogleNum.Click += new System.EventHandler(this.menuGoogleNum_Click);
			// 
			// checkShowDeleted
			// 
			this.checkShowDeleted.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowDeleted.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowDeleted.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowDeleted.Location = new System.Drawing.Point(751, 29);
			this.checkShowDeleted.Name = "checkShowDeleted";
			this.checkShowDeleted.Size = new System.Drawing.Size(95, 17);
			this.checkShowDeleted.TabIndex = 63;
			this.checkShowDeleted.TabStop = false;
			this.checkShowDeleted.Text = "Show Deleted";
			this.checkShowDeleted.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowDeleted.CheckedChanged += new System.EventHandler(this.checkShowDeleted_CheckedChanged);
			// 
			// labelError
			// 
			this.labelError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelError.ForeColor = System.Drawing.Color.Red;
			this.labelError.Location = new System.Drawing.Point(148, 479);
			this.labelError.Name = "labelError";
			this.labelError.Size = new System.Drawing.Size(599, 25);
			this.labelError.TabIndex = 62;
			this.labelError.Text = "Error Message";
			this.labelError.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.labelError.Visible = false;
			// 
			// labelDuration
			// 
			this.labelDuration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelDuration.Location = new System.Drawing.Point(12, 481);
			this.labelDuration.Name = "labelDuration";
			this.labelDuration.Size = new System.Drawing.Size(133, 17);
			this.labelDuration.TabIndex = 61;
			this.labelDuration.Text = "Duration:";
			this.labelDuration.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// axWindowsMediaPlayer
			// 
			this.axWindowsMediaPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.axWindowsMediaPlayer.Enabled = true;
			this.axWindowsMediaPlayer.Location = new System.Drawing.Point(12, 431);
			this.axWindowsMediaPlayer.Name = "axWindowsMediaPlayer";
			this.axWindowsMediaPlayer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer.OcxState")));
			this.axWindowsMediaPlayer.Size = new System.Drawing.Size(735, 45);
			this.axWindowsMediaPlayer.TabIndex = 58;
			// 
			// gridVoiceMails
			// 
			this.gridVoiceMails.AllowSortingByColumn = true;
			this.gridVoiceMails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridVoiceMails.HScrollVisible = true;
			this.gridVoiceMails.Location = new System.Drawing.Point(12, 12);
			this.gridVoiceMails.Name = "gridVoiceMails";
			this.gridVoiceMails.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridVoiceMails.Size = new System.Drawing.Size(735, 413);
			this.gridVoiceMails.TabIndex = 57;
			this.gridVoiceMails.TabStop = false;
			this.gridVoiceMails.Title = "Voice Mails";
			this.gridVoiceMails.TranslationName = "FormVoiceMails";
			this.gridVoiceMails.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridVoiceMails_CellClick);
			this.gridVoiceMails.CellLeave += new OpenDental.UI.ODGridClickEventHandler(this.gridVoiceMails_CellLeave);
			// 
			// butCreateTask
			// 
			this.butCreateTask.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.butCreateTask.Location = new System.Drawing.Point(768, 223);
			this.butCreateTask.Name = "butCreateTask";
			this.butCreateTask.Size = new System.Drawing.Size(78, 24);
			this.butCreateTask.TabIndex = 60;
			this.butCreateTask.Text = "Create &Task";
			this.butCreateTask.Click += new System.EventHandler(this.butCreateTask_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(768, 266);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(78, 24);
			this.butDelete.TabIndex = 59;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(768, 477);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(78, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormVoiceMails
			// 
			this.ClientSize = new System.Drawing.Size(858, 513);
			this.Controls.Add(this.checkShowDeleted);
			this.Controls.Add(this.labelError);
			this.Controls.Add(this.labelDuration);
			this.Controls.Add(this.butCreateTask);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.axWindowsMediaPlayer);
			this.Controls.Add(this.gridVoiceMails);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormVoiceMails";
			this.Text = "Voice Mails";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormVoiceMails_FormClosing);
			this.Load += new System.EventHandler(this.FormVoiceMails_Load);
			((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private UI.GridOD gridVoiceMails;
		private System.Windows.Forms.ContextMenu menuVoiceMailsRightClick;
		private System.Windows.Forms.MenuItem menuSendToMe;
		private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer;
		private UI.Button butDelete;
		private UI.Button butCreateTask;
		private System.Windows.Forms.Label labelDuration;
		private System.Windows.Forms.MenuItem menuGoToChart;
		private System.Windows.Forms.Label labelError;
		private System.Windows.Forms.MenuItem menuGoogleNum;
		private System.Windows.Forms.CheckBox checkShowDeleted;
		private System.Windows.Forms.MenuItem menuSendToMeCreateTask;
	}
}