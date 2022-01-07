namespace OpenDental{
	partial class FormImageFloatWindows {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				components?.Dispose();
				_graphicsPathHalf_L?.Dispose();
				_graphicsPathHalf_R?.Dispose();
				_graphicsPathQuarter_UL?.Dispose();
				_graphicsPathQuarter_UR?.Dispose();
				_graphicsPathQuarter_LL?.Dispose();
				_graphicsPathQuarter_LR?.Dispose();
				_graphicsPathMax?.Dispose();
				_graphicsPathCenter?.Dispose();
				_graphicsPathHalf_L2?.Dispose();
				_graphicsPathHalf_R2?.Dispose();
				_graphicsPathQuarter_UL2?.Dispose();
				_graphicsPathQuarter_UR2?.Dispose();
				_graphicsPathQuarter_LL2?.Dispose();
				_graphicsPathQuarter_LR2?.Dispose();
				_graphicsPathMax2?.Dispose();
				_graphicsPathCenter2?.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImageFloatWindows));
			this.labelActions = new System.Windows.Forms.Label();
			this.labelWindows = new System.Windows.Forms.Label();
			this.listBoxWindows = new OpenDental.UI.ListBoxOD();
			this.listBoxActions = new OpenDental.UI.ListBoxOD();
			this.SuspendLayout();
			// 
			// labelActions
			// 
			this.labelActions.Location = new System.Drawing.Point(12, 82);
			this.labelActions.Name = "labelActions";
			this.labelActions.Size = new System.Drawing.Size(100, 18);
			this.labelActions.TabIndex = 1;
			this.labelActions.Text = "Actions";
			this.labelActions.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelWindows
			// 
			this.labelWindows.Location = new System.Drawing.Point(12, 148);
			this.labelWindows.Name = "labelWindows";
			this.labelWindows.Size = new System.Drawing.Size(100, 18);
			this.labelWindows.TabIndex = 3;
			this.labelWindows.Text = "Open Windows";
			this.labelWindows.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listBoxWindows
			// 
			this.listBoxWindows.ItemStrings = new string[0];
			this.listBoxWindows.Location = new System.Drawing.Point(12, 168);
			this.listBoxWindows.Name = "listBoxWindows";
			this.listBoxWindows.Size = new System.Drawing.Size(225, 17);
			this.listBoxWindows.TabIndex = 2;
			this.listBoxWindows.SelectionChangeCommitted += new System.EventHandler(this.listBoxWindows_SelectionChangeCommitted);
			// 
			// listBoxActions
			// 
			this.listBoxActions.ItemStrings = new string[] {
        "Dock This",
        "Close Others",
        "Show All"};
			this.listBoxActions.Location = new System.Drawing.Point(12, 102);
			this.listBoxActions.Name = "listBoxActions";
			this.listBoxActions.Size = new System.Drawing.Size(120, 43);
			this.listBoxActions.TabIndex = 2;
			this.listBoxActions.SelectionChangeCommitted += new System.EventHandler(this.listBoxActions_SelectionChangeCommitted);
			// 
			// FormImageFloatWindows
			// 
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.ClientSize = new System.Drawing.Size(264, 221);
			this.Controls.Add(this.labelWindows);
			this.Controls.Add(this.listBoxWindows);
			this.Controls.Add(this.labelActions);
			this.Controls.Add(this.listBoxActions);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormImageFloatWindows";
			this.ShowInTaskbar = false;
			this.Deactivate += new System.EventHandler(this.FormImageFloatWindows_Deactivate);
			this.Load += new System.EventHandler(this.FormImageFloatWindows_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormImageFloatWindows_Paint);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormImageFloatWindows_MouseDown);
			this.MouseLeave += new System.EventHandler(this.FormImageFloatWindows_MouseLeave);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormImageFloatWindows_MouseMove);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.ListBoxOD listBoxActions;
		private System.Windows.Forms.Label labelActions;
		private System.Windows.Forms.Label labelWindows;
		private UI.ListBoxOD listBoxWindows;
	}
}