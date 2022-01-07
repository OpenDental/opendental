namespace OpenDental{
	partial class FormRecordAudio {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRecordAudio));
			this.labelTimer = new System.Windows.Forms.Label();
			this.timerRecord = new System.Windows.Forms.Timer(this.components);
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.butCancel = new OpenDental.UI.Button();
			this.butPlay = new OpenDental.UI.Button();
			this.butStart = new OpenDental.UI.Button();
			this.butSave = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelTimer
			// 
			this.labelTimer.ImageAlign = System.Drawing.ContentAlignment.TopRight;
			this.labelTimer.Location = new System.Drawing.Point(126, 20);
			this.labelTimer.Name = "labelTimer";
			this.labelTimer.Size = new System.Drawing.Size(75, 20);
			this.labelTimer.TabIndex = 7;
			this.labelTimer.Text = "0:00:00";
			this.labelTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// timerRecord
			// 
			this.timerRecord.Tick += new System.EventHandler(this.timerRecord_Tick);
			// 
			// imageListMain
			// 
			this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
			this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMain.Images.SetKeyName(0, "record.gif");
			this.imageListMain.Images.SetKeyName(1, "stop.gif");
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(216, 85);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 8;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butPlay
			// 
			this.butPlay.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPlay.Location = new System.Drawing.Point(37, 52);
			this.butPlay.Name = "butPlay";
			this.butPlay.Size = new System.Drawing.Size(75, 24);
			this.butPlay.TabIndex = 6;
			this.butPlay.Text = "Play";
			this.butPlay.Click += new System.EventHandler(this.butPlay_Click);
			// 
			// butStart
			// 
			this.butStart.Image = global::OpenDental.Properties.Resources.record;
			this.butStart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butStart.Location = new System.Drawing.Point(36, 18);
			this.butStart.Name = "butStart";
			this.butStart.Size = new System.Drawing.Size(76, 24);
			this.butStart.TabIndex = 5;
			this.butStart.Text = "Record";
			this.butStart.Click += new System.EventHandler(this.butStart_Click);
			// 
			// butSave
			// 
			this.butSave.Location = new System.Drawing.Point(37, 85);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 4;
			this.butSave.Text = "Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(216, 52);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormRecordAudio
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(303, 121);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelTimer);
			this.Controls.Add(this.butPlay);
			this.Controls.Add(this.butStart);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRecordAudio";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Record Audio";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormRecordAudio_FormClosing);
			this.Load += new System.EventHandler(this.FormRecordAudio_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private UI.Button butSave;
		private UI.Button butStart;
		private UI.Button butPlay;
		private System.Windows.Forms.Label labelTimer;
		private System.Windows.Forms.Timer timerRecord;
		private UI.Button butCancel;
		private System.Windows.Forms.ImageList imageListMain;
	}
}