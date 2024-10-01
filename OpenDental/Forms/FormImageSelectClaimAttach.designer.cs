using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormImageSelectClaimAttach {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImageSelectClaimAttach));
			this.butOK = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.labelPasteImage = new System.Windows.Forms.Label();
			this.butPasteImage = new OpenDental.UI.Button();
			this.butSnipTool = new OpenDental.UI.Button();
			this.butImport = new OpenDental.UI.Button();
			this.timerMonitorClipboard = new System.Windows.Forms.Timer(this.components);
			this.timerKillSnipToolProcesses = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(481, 513);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(441, 527);
			this.gridMain.TabIndex = 2;
			this.gridMain.Title = "Existing Images";
			this.gridMain.TranslationName = "FormImageSelect";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// labelPasteImage
			// 
			this.labelPasteImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPasteImage.Location = new System.Drawing.Point(459, 135);
			this.labelPasteImage.Name = "labelPasteImage";
			this.labelPasteImage.Size = new System.Drawing.Size(110, 18);
			this.labelPasteImage.TabIndex = 20;
			this.labelPasteImage.Text = "(from clipboard)";
			this.labelPasteImage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// butPasteImage
			// 
			this.butPasteImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPasteImage.Location = new System.Drawing.Point(471, 108);
			this.butPasteImage.Name = "butPasteImage";
			this.butPasteImage.Size = new System.Drawing.Size(85, 24);
			this.butPasteImage.TabIndex = 19;
			this.butPasteImage.Text = "Paste Image";
			this.butPasteImage.UseVisualStyleBackColor = true;
			this.butPasteImage.Click += new System.EventHandler(this.butPasteImage_Click);
			// 
			// butSnipTool
			// 
			this.butSnipTool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSnipTool.Location = new System.Drawing.Point(471, 78);
			this.butSnipTool.Name = "butSnipTool";
			this.butSnipTool.Size = new System.Drawing.Size(85, 24);
			this.butSnipTool.TabIndex = 17;
			this.butSnipTool.Text = "Snipping Tool";
			this.butSnipTool.UseVisualStyleBackColor = true;
			this.butSnipTool.Click += new System.EventHandler(this.butSnipTool_Click);
			// 
			// butImport
			// 
			this.butImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butImport.Location = new System.Drawing.Point(471, 49);
			this.butImport.Name = "butImport";
			this.butImport.Size = new System.Drawing.Size(85, 24);
			this.butImport.TabIndex = 18;
			this.butImport.Text = "Import";
			this.butImport.UseVisualStyleBackColor = true;
			this.butImport.Click += new System.EventHandler(this.butImport_Click);
			// 
			// timerMonitorClipboard
			// 
			this.timerMonitorClipboard.Interval = 250;
			this.timerMonitorClipboard.Tick += new System.EventHandler(this.timerMonitorClipboard_Tick);
			// 
			// timerKillSnipToolProcesses
			// 
			this.timerKillSnipToolProcesses.Tick += new System.EventHandler(this.timerKillSnipToolProcesses_Tick);
			// 
			// FormImageSelectClaimAttach
			// 
			this.ClientSize = new System.Drawing.Size(579, 564);
			this.Controls.Add(this.labelPasteImage);
			this.Controls.Add(this.butPasteImage);
			this.Controls.Add(this.butSnipTool);
			this.Controls.Add(this.butImport);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormImageSelectClaimAttach";
			this.ShowInTaskbar = false;
			this.Text = "Select Image for Claim Attachment";
			this.Load += new System.EventHandler(this.FormImageSelect_Load);
			this.ResumeLayout(false);

		}
		#endregion
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.GridOD gridMain;
		private Label labelPasteImage;
		private UI.Button butPasteImage;
		private UI.Button butSnipTool;
		private UI.Button butImport;
		private System.Windows.Forms.Timer timerMonitorClipboard;
		private System.Windows.Forms.Timer timerKillSnipToolProcesses;
	}
}
