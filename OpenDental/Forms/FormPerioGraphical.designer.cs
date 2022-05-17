namespace OpenDental {
	partial class FormPerioGraphical {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPerioGraphical));
			this.toothChartWrapper = new SparksToothChart.ToothChartWrapper();
			this.butPrint = new OpenDental.UI.Button();
			this.butSave = new OpenDental.UI.Button();
			this.butSetup = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// toothChartWrapper
			// 
			this.toothChartWrapper.AutoFinish = false;
			this.toothChartWrapper.ColorBackground = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(145)))), ((int)(((byte)(152)))));
			this.toothChartWrapper.Cursor = System.Windows.Forms.Cursors.Default;
			this.toothChartWrapper.CursorTool = SparksToothChart.CursorTool.Pointer;
			this.toothChartWrapper.DeviceFormat = null;
			this.toothChartWrapper.DrawMode = OpenDentBusiness.DrawingMode.Simple2D;
			this.toothChartWrapper.Location = new System.Drawing.Point(0, 0);
			this.toothChartWrapper.Name = "toothChartWrapper";
			this.toothChartWrapper.PerioMode = false;
			this.toothChartWrapper.PreferredPixelFormatNumber = 0;
			this.toothChartWrapper.Size = new System.Drawing.Size(649, 696);
			this.toothChartWrapper.TabIndex = 198;
			this.toothChartWrapper.UseHardware = false;
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(672, 108);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(84, 24);
			this.butPrint.TabIndex = 220;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(672, 56);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(84, 24);
			this.butSave.TabIndex = 219;
			this.butSave.Text = "Save to Images";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butSetup
			// 
			this.butSetup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSetup.Location = new System.Drawing.Point(672, 5);
			this.butSetup.Name = "butSetup";
			this.butSetup.Size = new System.Drawing.Size(84, 24);
			this.butSetup.TabIndex = 221;
			this.butSetup.Text = "Setup Colors";
			this.butSetup.Click += new System.EventHandler(this.butSetup_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(681, 660);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 222;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.Red;
			this.label1.Location = new System.Drawing.Point(664, 211);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 166);
			this.label1.TabIndex = 223;
			this.label1.Text = "Do NOT\r\nchange the\r\nsize of this \r\nform or the\r\nmain control\r\nto the left\r\n-Jorda" +
    "n";
			this.label1.Visible = false;
			// 
			// FormPerioGraphical
			// 
			this.ClientSize = new System.Drawing.Size(768, 696);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butSetup);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.toothChartWrapper);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPerioGraphical";
			this.Text = "Graphical Perio Chart";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPerioGraphical_FormClosing);
			this.Load += new System.EventHandler(this.FormPerioGraphic_Load);
			this.ResizeEnd += new System.EventHandler(this.FormPerioGraphical_ResizeEnd);
			this.ResumeLayout(false);

		}

		#endregion

		private SparksToothChart.ToothChartWrapper toothChartWrapper;
		private OpenDental.UI.Button butSave;
		private OpenDental.UI.Button butSetup;
		private OpenDental.UI.Button butPrint;
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.Label label1;
	}
}