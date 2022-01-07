using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTrojanCollectSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTrojanCollectSetup));
			this.labelExportFolder = new System.Windows.Forms.Label();
			this.textExportFolder = new System.Windows.Forms.TextBox();
			this.labelBillType = new System.Windows.Forms.Label();
			this.comboBillType = new System.Windows.Forms.ComboBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.labelPassword = new System.Windows.Forms.Label();
			this.butBrowse = new OpenDental.UI.Button();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// labelExportFolder
			// 
			this.labelExportFolder.Location = new System.Drawing.Point(12, 42);
			this.labelExportFolder.Name = "labelExportFolder";
			this.labelExportFolder.Size = new System.Drawing.Size(250, 29);
			this.labelExportFolder.TabIndex = 4;
			this.labelExportFolder.Text = "Export Folder.  Should be a shared network folder and must be the same for all co" +
    "mputers.";
			this.labelExportFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textExportFolder
			// 
			this.textExportFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textExportFolder.Location = new System.Drawing.Point(263, 46);
			this.textExportFolder.Name = "textExportFolder";
			this.textExportFolder.Size = new System.Drawing.Size(216, 20);
			this.textExportFolder.TabIndex = 0;
			// 
			// labelBillType
			// 
			this.labelBillType.Location = new System.Drawing.Point(12, 75);
			this.labelBillType.Name = "labelBillType";
			this.labelBillType.Size = new System.Drawing.Size(250, 18);
			this.labelBillType.TabIndex = 6;
			this.labelBillType.Text = "Billing type for patients sent to collections";
			this.labelBillType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBillType
			// 
			this.comboBillType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBillType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBillType.FormattingEnabled = true;
			this.comboBillType.Location = new System.Drawing.Point(263, 74);
			this.comboBillType.MaxDropDownItems = 50;
			this.comboBillType.Name = "comboBillType";
			this.comboBillType.Size = new System.Drawing.Size(216, 21);
			this.comboBillType.TabIndex = 2;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(404, 135);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(485, 135);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(263, 103);
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(68, 20);
			this.textPassword.TabIndex = 3;
			// 
			// labelPassword
			// 
			this.labelPassword.Location = new System.Drawing.Point(12, 104);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(250, 18);
			this.labelPassword.TabIndex = 28;
			this.labelPassword.Text = "Trojan Collection Services password";
			this.labelPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butBrowse
			// 
			this.butBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butBrowse.Location = new System.Drawing.Point(484, 44);
			this.butBrowse.Name = "butBrowse";
			this.butBrowse.Size = new System.Drawing.Size(76, 25);
			this.butBrowse.TabIndex = 1;
			this.butBrowse.Text = "&Browse";
			this.butBrowse.Click += new System.EventHandler(this.ButBrowse_Click);
			// 
			// checkEnabled
			// 
			this.checkEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEnabled.Location = new System.Drawing.Point(12, 12);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(92, 18);
			this.checkEnabled.TabIndex = 76;
			this.checkEnabled.Text = "Enabled";
			// 
			// FormTrojanCollectSetup
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(572, 173);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.butBrowse);
			this.Controls.Add(this.textPassword);
			this.Controls.Add(this.labelPassword);
			this.Controls.Add(this.comboBillType);
			this.Controls.Add(this.labelBillType);
			this.Controls.Add(this.textExportFolder);
			this.Controls.Add(this.labelExportFolder);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTrojanCollectSetup";
			this.ShowInTaskbar = false;
			this.Text = "Setup Trojan Express Collect";
			this.Load += new System.EventHandler(this.FormTrojanCollectSetup_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label labelExportFolder;
		private TextBox textExportFolder;
		private Label labelBillType;
		private ComboBox comboBillType;
		private TextBox textPassword;
		private Label labelPassword;
		private UI.Button butBrowse;
		private CheckBox checkEnabled;
	}
}
