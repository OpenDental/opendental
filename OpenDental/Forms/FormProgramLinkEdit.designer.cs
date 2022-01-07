using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormProgramLinkEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProgramLinkEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.butDelete = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textProgName = new System.Windows.Forms.TextBox();
			this.textProgDesc = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textPath = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textCommandLine = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.listToolBars = new OpenDental.UI.ListBoxOD();
			this.label6 = new System.Windows.Forms.Label();
			this.textButtonText = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.textPluginDllName = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textOverride = new System.Windows.Forms.TextBox();
			this.labelOverride = new System.Windows.Forms.Label();
			this.butClear = new OpenDental.UI.Button();
			this.butImport = new OpenDental.UI.Button();
			this.label10 = new System.Windows.Forms.Label();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.butOutputFile = new OpenDental.UI.Button();
			this.checkHideButtons = new System.Windows.Forms.CheckBox();
			this.labelCloudMessage = new System.Windows.Forms.Label();
			this.butClinicLink = new OpenDental.UI.Button();
			this.groupboxPLbuttons = new System.Windows.Forms.GroupBox();
			this.labelClinicStateWarning = new System.Windows.Forms.Label();
			this.labelDisableForClinic = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.groupboxPLbuttons.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(673, 514);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(673, 480);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// checkEnabled
			// 
			this.checkEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEnabled.Location = new System.Drawing.Point(253, 61);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(98, 18);
			this.checkEnabled.TabIndex = 41;
			this.checkEnabled.Text = "Enabled";
			this.checkEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.CheckedChanged += new System.EventHandler(this.checkEnabled_CheckedChanged);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(15, 514);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 26);
			this.butDelete.TabIndex = 43;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(150, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(187, 18);
			this.label1.TabIndex = 44;
			this.label1.Text = "Internal Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProgName
			// 
			this.textProgName.Location = new System.Drawing.Point(338, 16);
			this.textProgName.Name = "textProgName";
			this.textProgName.ReadOnly = true;
			this.textProgName.Size = new System.Drawing.Size(275, 20);
			this.textProgName.TabIndex = 45;
			// 
			// textProgDesc
			// 
			this.textProgDesc.Location = new System.Drawing.Point(338, 37);
			this.textProgDesc.Name = "textProgDesc";
			this.textProgDesc.Size = new System.Drawing.Size(275, 20);
			this.textProgDesc.TabIndex = 47;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(149, 38);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(187, 18);
			this.label2.TabIndex = 46;
			this.label2.Text = "Description";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPath
			// 
			this.textPath.Location = new System.Drawing.Point(338, 101);
			this.textPath.Name = "textPath";
			this.textPath.Size = new System.Drawing.Size(410, 20);
			this.textPath.TabIndex = 49;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(105, 103);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(231, 18);
			this.label3.TabIndex = 48;
			this.label3.Text = "Path of file to open";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCommandLine
			// 
			this.textCommandLine.Location = new System.Drawing.Point(338, 143);
			this.textCommandLine.Name = "textCommandLine";
			this.textCommandLine.Size = new System.Drawing.Size(410, 20);
			this.textCommandLine.TabIndex = 52;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(95, 143);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(241, 37);
			this.label4.TabIndex = 51;
			this.label4.Text = "Optional command line arguments.  Leave this blank for most bridges.";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listToolBars
			// 
			this.listToolBars.Location = new System.Drawing.Point(19, 104);
			this.listToolBars.Name = "listToolBars";
			this.listToolBars.SelectionMode = UI.SelectionMode.MultiExtended;
			this.listToolBars.Size = new System.Drawing.Size(156, 121);
			this.listToolBars.TabIndex = 53;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(19, 80);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(157, 18);
			this.label6.TabIndex = 56;
			this.label6.Text = "Add a button to these toolbars";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textButtonText
			// 
			this.textButtonText.Location = new System.Drawing.Point(338, 224);
			this.textButtonText.Name = "textButtonText";
			this.textButtonText.Size = new System.Drawing.Size(195, 20);
			this.textButtonText.TabIndex = 58;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(253, 225);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(84, 18);
			this.label7.TabIndex = 57;
			this.label7.Text = "Text on button";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(338, 425);
			this.textNote.MaxLength = 4000;
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(323, 80);
			this.textNote.TabIndex = 59;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(338, 405);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(221, 17);
			this.label8.TabIndex = 60;
			this.label8.Text = "Notes";
			this.label8.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(336, 166);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(410, 55);
			this.label9.TabIndex = 61;
			this.label9.Text = resources.GetString("label9.Text");
			// 
			// gridMain
			// 
			this.gridMain.Location = new System.Drawing.Point(338, 269);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(410, 133);
			this.gridMain.TabIndex = 62;
			this.gridMain.Title = "Additional Properties";
			this.gridMain.TranslationName = "TableProperties";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// textPluginDllName
			// 
			this.textPluginDllName.Location = new System.Drawing.Point(338, 245);
			this.textPluginDllName.Name = "textPluginDllName";
			this.textPluginDllName.Size = new System.Drawing.Size(195, 20);
			this.textPluginDllName.TabIndex = 64;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(253, 246);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(84, 18);
			this.label5.TabIndex = 63;
			this.label5.Text = "Plug-in dll name";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textOverride
			// 
			this.textOverride.Location = new System.Drawing.Point(338, 122);
			this.textOverride.Name = "textOverride";
			this.textOverride.Size = new System.Drawing.Size(410, 20);
			this.textOverride.TabIndex = 66;
			// 
			// labelOverride
			// 
			this.labelOverride.Location = new System.Drawing.Point(105, 124);
			this.labelOverride.Name = "labelOverride";
			this.labelOverride.Size = new System.Drawing.Size(231, 18);
			this.labelOverride.TabIndex = 65;
			this.labelOverride.Text = "Local path override.  Usually left blank.";
			this.labelOverride.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butClear
			// 
			this.butClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClear.Location = new System.Drawing.Point(19, 264);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(75, 26);
			this.butClear.TabIndex = 70;
			this.butClear.Text = "Clear";
			this.butClear.Click += new System.EventHandler(this.butClear_Click);
			// 
			// butImport
			// 
			this.butImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butImport.Location = new System.Drawing.Point(100, 264);
			this.butImport.Name = "butImport";
			this.butImport.Size = new System.Drawing.Size(75, 26);
			this.butImport.TabIndex = 69;
			this.butImport.Text = "Import";
			this.butImport.Click += new System.EventHandler(this.butImport_Click);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(16, 234);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(138, 22);
			this.label10.TabIndex = 68;
			this.label10.Text = "Button Image (22x22)";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// pictureBox
			// 
			this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox.Location = new System.Drawing.Point(153, 234);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(22, 22);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBox.TabIndex = 72;
			this.pictureBox.TabStop = false;
			// 
			// butOutputFile
			// 
			this.butOutputFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butOutputFile.Location = new System.Drawing.Point(338, 511);
			this.butOutputFile.Name = "butOutputFile";
			this.butOutputFile.Size = new System.Drawing.Size(75, 26);
			this.butOutputFile.TabIndex = 73;
			this.butOutputFile.Text = "Output File";
			this.butOutputFile.Click += new System.EventHandler(this.butOutputFile_Click);
			// 
			// checkHideButtons
			// 
			this.checkHideButtons.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHideButtons.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHideButtons.Location = new System.Drawing.Point(188, 80);
			this.checkHideButtons.Name = "checkHideButtons";
			this.checkHideButtons.Size = new System.Drawing.Size(163, 18);
			this.checkHideButtons.TabIndex = 74;
			this.checkHideButtons.Text = "Hide Unused Button";
			this.checkHideButtons.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHideButtons.CheckedChanged += new System.EventHandler(this.checkHideButtons_CheckedChanged);
			// 
			// labelCloudMessage
			// 
			this.labelCloudMessage.ForeColor = System.Drawing.Color.DarkRed;
			this.labelCloudMessage.Location = new System.Drawing.Point(353, 60);
			this.labelCloudMessage.Name = "labelCloudMessage";
			this.labelCloudMessage.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelCloudMessage.Size = new System.Drawing.Size(166, 18);
			this.labelCloudMessage.TabIndex = 75;
			this.labelCloudMessage.Text = "Bridge disabled in Cloud mode";
			this.labelCloudMessage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelCloudMessage.Visible = false;
			// 
			// butClinicLink
			// 
			this.butClinicLink.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClinicLink.Location = new System.Drawing.Point(19, 17);
			this.butClinicLink.Name = "butClinicLink";
			this.butClinicLink.Size = new System.Drawing.Size(121, 26);
			this.butClinicLink.TabIndex = 76;
			this.butClinicLink.Text = "Hide Button for Clinics";
			this.butClinicLink.Click += new System.EventHandler(this.butClinicLink_Click);
			// 
			// groupboxPLbuttons
			// 
			this.groupboxPLbuttons.Controls.Add(this.labelClinicStateWarning);
			this.groupboxPLbuttons.Controls.Add(this.listToolBars);
			this.groupboxPLbuttons.Controls.Add(this.label6);
			this.groupboxPLbuttons.Controls.Add(this.butClear);
			this.groupboxPLbuttons.Controls.Add(this.butClinicLink);
			this.groupboxPLbuttons.Controls.Add(this.pictureBox);
			this.groupboxPLbuttons.Controls.Add(this.butImport);
			this.groupboxPLbuttons.Controls.Add(this.label10);
			this.groupboxPLbuttons.Location = new System.Drawing.Point(15, 177);
			this.groupboxPLbuttons.Name = "groupboxPLbuttons";
			this.groupboxPLbuttons.Size = new System.Drawing.Size(200, 304);
			this.groupboxPLbuttons.TabIndex = 0;
			this.groupboxPLbuttons.TabStop = false;
			this.groupboxPLbuttons.Text = "Button Settings";
			// 
			// labelClinicStateWarning
			// 
			this.labelClinicStateWarning.ForeColor = System.Drawing.Color.DarkRed;
			this.labelClinicStateWarning.Location = new System.Drawing.Point(19, 50);
			this.labelClinicStateWarning.Name = "labelClinicStateWarning";
			this.labelClinicStateWarning.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelClinicStateWarning.Size = new System.Drawing.Size(159, 33);
			this.labelClinicStateWarning.TabIndex = 76;
			this.labelClinicStateWarning.Text = "Program Link button is not visible for some clinics.";
			this.labelClinicStateWarning.Visible = false;
			// 
			// labelDisableForClinic
			// 
			this.labelDisableForClinic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDisableForClinic.ForeColor = System.Drawing.Color.DarkRed;
			this.labelDisableForClinic.Location = new System.Drawing.Point(15, 545);
			this.labelDisableForClinic.Name = "labelDisableForClinic";
			this.labelDisableForClinic.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelDisableForClinic.Size = new System.Drawing.Size(751, 20);
			this.labelDisableForClinic.TabIndex = 77;
			this.labelDisableForClinic.Text = "User is Clinic restricted, some functions of this window are disabled. To enable," +
    " contact an administrator.";
			this.labelDisableForClinic.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelDisableForClinic.Visible = false;
			// 
			// FormProgramLinkEdit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(768, 567);
			this.Controls.Add(this.labelDisableForClinic);
			this.Controls.Add(this.labelCloudMessage);
			this.Controls.Add(this.checkHideButtons);
			this.Controls.Add(this.butOutputFile);
			this.Controls.Add(this.textOverride);
			this.Controls.Add(this.labelOverride);
			this.Controls.Add(this.textPluginDllName);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.textButtonText);
			this.Controls.Add(this.textCommandLine);
			this.Controls.Add(this.textPath);
			this.Controls.Add(this.textProgDesc);
			this.Controls.Add(this.textProgName);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupboxPLbuttons);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormProgramLinkEdit";
			this.ShowInTaskbar = false;
			this.Text = "Program Link";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormProgramLinkEdit_Closing);
			this.Load += new System.EventHandler(this.FormProgramLinkEdit_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.groupboxPLbuttons.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.CheckBox checkEnabled;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textProgName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textProgDesc;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textPath;
		private System.Windows.Forms.TextBox textCommandLine;
		private OpenDental.UI.ListBoxOD listToolBars;
		private System.Windows.Forms.TextBox textButtonText;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textNote;
		private Label label9;// Required designer variable.
		private OpenDental.UI.GridOD gridMain;
		private TextBox textPluginDllName;
		private Label label5;
		private TextBox textOverride;
		private Label labelOverride;
		private UI.Button butClear;
		private UI.Button butImport;
		private Label label10;
		private PictureBox pictureBox;
		private UI.Button butOutputFile;
		private CheckBox checkHideButtons;
		private Label labelCloudMessage;
		private UI.Button butClinicLink;
		private GroupBox groupboxPLbuttons;
		private Label labelClinicStateWarning;
		private Label labelDisableForClinic;
	}
}
