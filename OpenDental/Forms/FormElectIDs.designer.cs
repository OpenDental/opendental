using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormElectIDs {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )	{
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormElectIDs));
			this.gridElectIDs = new OpenDental.UI.GridOD();
			this.butOK = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.labelCommBridge = new System.Windows.Forms.Label();
			this.comboCommBridge = new OpenDental.UI.ComboBox();
			this.SuspendLayout();
			// 
			// gridElectIDs
			// 
			this.gridElectIDs.AllowSelection = false;
			this.gridElectIDs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridElectIDs.Location = new System.Drawing.Point(7, 39);
			this.gridElectIDs.Name = "gridElectIDs";
			this.gridElectIDs.Size = new System.Drawing.Size(900, 640);
			this.gridElectIDs.TabIndex = 140;
			this.gridElectIDs.TranslationName = "TableApptProcs";
			this.gridElectIDs.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridElectIDs_CellDoubleClick);
			this.gridElectIDs.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridElectIDs_CellClick);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(832, 692);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 25);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(7, 692);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 25);
			this.butAdd.TabIndex = 141;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// labelCommBridge
			// 
			this.labelCommBridge.Location = new System.Drawing.Point(7, 16);
			this.labelCommBridge.Name = "labelCommBridge";
			this.labelCommBridge.Size = new System.Drawing.Size(84, 13);
			this.labelCommBridge.TabIndex = 280;
			this.labelCommBridge.Text = "CommBridge";
			this.labelCommBridge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboCommBridge
			// 
			this.comboCommBridge.Location = new System.Drawing.Point(93, 12);
			this.comboCommBridge.Name = "comboCommBridge";
			this.comboCommBridge.Size = new System.Drawing.Size(160, 21);
			this.comboCommBridge.TabIndex = 279;
			this.comboCommBridge.SelectionChangeCommitted += new System.EventHandler(this.comboCommBridge_SelectionChangeCommitted);
			// 
			// FormElectIDs
			// 
			this.ClientSize = new System.Drawing.Size(913, 731);
			this.Controls.Add(this.labelCommBridge);
			this.Controls.Add(this.comboCommBridge);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridElectIDs);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormElectIDs";
			this.ShowInTaskbar = false;
			this.Text = "Electronic Payer ID\'s";
			this.Load += new System.EventHandler(this.FormElectIDs_Load);
			this.ResumeLayout(false);

		}
		#endregion
		private OpenDental.UI.Button butOK;
		private UI.GridOD gridElectIDs;
		private UI.Button butAdd;
		private Label labelCommBridge;
		private UI.ComboBox comboCommBridge;
	}
}
