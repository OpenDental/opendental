using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTPsign {
		private System.ComponentModel.IContainer components = null;

		/// <summary>Clean up any resources being used.</summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTPsign));
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.textTypeSigPractice = new System.Windows.Forms.TextBox();
			this.labelTypeSigPractice = new System.Windows.Forms.Label();
			this.signatureBoxWrapperPractice = new OpenDental.UI.SignatureBoxWrapper();
			this.labelSigPractice = new System.Windows.Forms.Label();
			this.panelSig = new System.Windows.Forms.Panel();
			this.textTypeSig = new System.Windows.Forms.TextBox();
			this.labelTypeSig = new System.Windows.Forms.Label();
			this.signatureBoxWrapper = new OpenDental.UI.SignatureBoxWrapper();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.labelSig = new System.Windows.Forms.Label();
			this.ToolBarMain = new OpenDental.UI.ToolBarOD();
			this.previewContr = new System.Windows.Forms.PrintPreviewControl();
			this.panelSig.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageListMain
			// 
			this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
			this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMain.Images.SetKeyName(0, "");
			this.imageListMain.Images.SetKeyName(1, "");
			this.imageListMain.Images.SetKeyName(2, "");
			// 
			// textTypeSigPractice
			// 
			this.textTypeSigPractice.Location = new System.Drawing.Point(651, 86);
			this.textTypeSigPractice.Name = "textTypeSigPractice";
			this.textTypeSigPractice.Size = new System.Drawing.Size(331, 20);
			this.textTypeSigPractice.TabIndex = 228;
			this.textTypeSigPractice.Visible = false;
			this.textTypeSigPractice.TextChanged += new System.EventHandler(this.textTypeSigPractice_TextChanged);
			// 
			// labelTypeSigPractice
			// 
			this.labelTypeSigPractice.Location = new System.Drawing.Point(502, 87);
			this.labelTypeSigPractice.Name = "labelTypeSigPractice";
			this.labelTypeSigPractice.Size = new System.Drawing.Size(147, 17);
			this.labelTypeSigPractice.TabIndex = 229;
			this.labelTypeSigPractice.Text = "Type name here";
			this.labelTypeSigPractice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelTypeSigPractice.Visible = false;
			// 
			// signatureBoxWrapperPractice
			// 
			this.signatureBoxWrapperPractice.BackColor = System.Drawing.SystemColors.ControlDark;
			this.signatureBoxWrapperPractice.Location = new System.Drawing.Point(651, 3);
			this.signatureBoxWrapperPractice.Name = "signatureBoxWrapperPractice";
			this.signatureBoxWrapperPractice.SignatureMode = OpenDental.UI.SignatureBoxWrapper.SigMode.Default;
			this.signatureBoxWrapperPractice.Size = new System.Drawing.Size(331, 79);
			this.signatureBoxWrapperPractice.TabIndex = 182;
			this.signatureBoxWrapperPractice.UserSig = null;
			this.signatureBoxWrapperPractice.Visible = false;
			this.signatureBoxWrapperPractice.SignatureChanged += new System.EventHandler(this.signatureBoxWrapperPractice_SignatureChanged);
			// 
			// labelSigPractice
			// 
			this.labelSigPractice.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelSigPractice.Location = new System.Drawing.Point(496, 4);
			this.labelSigPractice.Name = "labelSigPractice";
			this.labelSigPractice.Size = new System.Drawing.Size(153, 41);
			this.labelSigPractice.TabIndex = 92;
			this.labelSigPractice.Text = "Practice Sign Here --->";
			this.labelSigPractice.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.labelSigPractice.Visible = false;
			// 
			// panelSig
			// 
			this.panelSig.Controls.Add(this.textTypeSigPractice);
			this.panelSig.Controls.Add(this.textTypeSig);
			this.panelSig.Controls.Add(this.labelTypeSigPractice);
			this.panelSig.Controls.Add(this.labelTypeSig);
			this.panelSig.Controls.Add(this.signatureBoxWrapperPractice);
			this.panelSig.Controls.Add(this.labelSigPractice);
			this.panelSig.Controls.Add(this.signatureBoxWrapper);
			this.panelSig.Controls.Add(this.butCancel);
			this.panelSig.Controls.Add(this.butOK);
			this.panelSig.Controls.Add(this.labelSig);
			this.panelSig.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelSig.Location = new System.Drawing.Point(0, 581);
			this.panelSig.Name = "panelSig";
			this.panelSig.Size = new System.Drawing.Size(1159, 115);
			this.panelSig.TabIndex = 92;
			// 
			// textTypeSig
			// 
			this.textTypeSig.Location = new System.Drawing.Point(162, 88);
			this.textTypeSig.Name = "textTypeSig";
			this.textTypeSig.Size = new System.Drawing.Size(331, 20);
			this.textTypeSig.TabIndex = 230;
			this.textTypeSig.Visible = false;
			this.textTypeSig.TextChanged += new System.EventHandler(this.textTypeSig_TextChanged);
			// 
			// labelTypeSig
			// 
			this.labelTypeSig.Location = new System.Drawing.Point(13, 89);
			this.labelTypeSig.Name = "labelTypeSig";
			this.labelTypeSig.Size = new System.Drawing.Size(147, 17);
			this.labelTypeSig.TabIndex = 231;
			this.labelTypeSig.Text = "Type name here";
			this.labelTypeSig.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelTypeSig.Visible = false;
			// 
			// signatureBoxWrapper
			// 
			this.signatureBoxWrapper.BackColor = System.Drawing.SystemColors.ControlDark;
			this.signatureBoxWrapper.Location = new System.Drawing.Point(162, 3);
			this.signatureBoxWrapper.Name = "signatureBoxWrapper";
			this.signatureBoxWrapper.SignatureMode = OpenDental.UI.SignatureBoxWrapper.SigMode.Default;
			this.signatureBoxWrapper.Size = new System.Drawing.Size(331, 79);
			this.signatureBoxWrapper.TabIndex = 182;
			this.signatureBoxWrapper.UserSig = null;
			this.signatureBoxWrapper.SignatureChanged += new System.EventHandler(this.signatureBoxWrapper_SignatureChanged);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(1012, 70);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 25);
			this.butCancel.TabIndex = 94;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(1012, 38);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 25);
			this.butOK.TabIndex = 93;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelSig
			// 
			this.labelSig.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelSig.Location = new System.Drawing.Point(7, 4);
			this.labelSig.Name = "labelSig";
			this.labelSig.Size = new System.Drawing.Size(153, 41);
			this.labelSig.TabIndex = 92;
			this.labelSig.Text = "Please Sign Here --->";
			this.labelSig.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.ToolBarMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.ToolBarMain.ImageList = this.imageListMain;
			this.ToolBarMain.Location = new System.Drawing.Point(0, 0);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(1159, 25);
			this.ToolBarMain.TabIndex = 5;
			this.ToolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// previewContr
			// 
			this.previewContr.AutoZoom = false;
			this.previewContr.Location = new System.Drawing.Point(10, 41);
			this.previewContr.Name = "previewContr";
			this.previewContr.Size = new System.Drawing.Size(806, 423);
			this.previewContr.TabIndex = 6;
			// 
			// FormTPsign
			// 
			this.ClientSize = new System.Drawing.Size(1099, 696);
			this.Controls.Add(this.panelSig);
			this.Controls.Add(this.ToolBarMain);
			this.Controls.Add(this.previewContr);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTPsign";
			this.Text = "Report";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTPsign_FormClosing);
			this.Load += new System.EventHandler(this.FormTPsign_Load);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.FormReport_Layout);
			this.panelSig.ResumeLayout(false);
			this.panelSig.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		///<summary>Currently the size of this control is (331,79). If this is ever changed then eClipboard needs to be considered.</summary>
		private UI.SignatureBoxWrapper signatureBoxWrapperPractice;
		///<summary>Currently the size of this control is (331,79). If this is ever changed then eClipboard needs to be considered.</summary>
		private UI.SignatureBoxWrapper signatureBoxWrapper;
		private OpenDental.UI.ToolBarOD ToolBarMain;
		private System.Windows.Forms.ImageList imageListMain;
		private System.Windows.Forms.PrintPreviewControl previewContr;
		private Panel panelSig;
		private Label labelSig;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label labelSigPractice;
		private TextBox textTypeSig;
		private Label labelTypeSig;
		private TextBox textTypeSigPractice;
		private Label labelTypeSigPractice;
	}
}
