using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormSigElementDefEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSigElementDefEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.listType = new OpenDental.UI.ListBoxOD();
			this.label2 = new System.Windows.Forms.Label();
			this.textSigText = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.butColor = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butRecordNew = new OpenDental.UI.Button();
			this.butDeleteSound = new OpenDental.UI.Button();
			this.butRecord = new OpenDental.UI.Button();
			this.butExport = new OpenDental.UI.Button();
			this.butImport = new OpenDental.UI.Button();
			this.butPlay = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.textLightRow = new OpenDental.ValidNum();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.ImageAlign = System.Drawing.ContentAlignment.TopRight;
			this.label1.Location = new System.Drawing.Point(63, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 20);
			this.label1.TabIndex = 2;
			this.label1.Text = "Type";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listType
			// 
			this.listType.Location = new System.Drawing.Point(168, 12);
			this.listType.Name = "listType";
			this.listType.Size = new System.Drawing.Size(106, 43);
			this.listType.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.ImageAlign = System.Drawing.ContentAlignment.TopRight;
			this.label2.Location = new System.Drawing.Point(63, 70);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 20);
			this.label2.TabIndex = 4;
			this.label2.Text = "Text";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textSigText
			// 
			this.textSigText.Location = new System.Drawing.Point(169, 67);
			this.textSigText.Name = "textSigText";
			this.textSigText.Size = new System.Drawing.Size(105, 20);
			this.textSigText.TabIndex = 0;
			// 
			// label3
			// 
			this.label3.ImageAlign = System.Drawing.ContentAlignment.TopRight;
			this.label3.Location = new System.Drawing.Point(281, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(304, 40);
			this.label3.TabIndex = 6;
			this.label3.Text = "This is the text as it should show in the list or the username.  Typed messages a" +
    "re handled separately.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label5
			// 
			this.label5.ImageAlign = System.Drawing.ContentAlignment.TopRight;
			this.label5.Location = new System.Drawing.Point(62, 253);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 20);
			this.label5.TabIndex = 8;
			this.label5.Text = "Light Row";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label6
			// 
			this.label6.ImageAlign = System.Drawing.ContentAlignment.TopRight;
			this.label6.Location = new System.Drawing.Point(62, 293);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(100, 20);
			this.label6.TabIndex = 9;
			this.label6.Text = "Light Color";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butColor
			// 
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColor.Location = new System.Drawing.Point(168, 289);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(30, 20);
			this.butColor.TabIndex = 11;
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			// 
			// label7
			// 
			this.label7.ImageAlign = System.Drawing.ContentAlignment.TopRight;
			this.label7.Location = new System.Drawing.Point(281, 275);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(278, 73);
			this.label7.TabIndex = 12;
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butRecordNew);
			this.groupBox1.Controls.Add(this.butDeleteSound);
			this.groupBox1.Controls.Add(this.butRecord);
			this.groupBox1.Controls.Add(this.butExport);
			this.groupBox1.Controls.Add(this.butImport);
			this.groupBox1.Controls.Add(this.butPlay);
			this.groupBox1.Location = new System.Drawing.Point(121, 93);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(464, 141);
			this.groupBox1.TabIndex = 17;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Sound";
			// 
			// butRecordNew
			// 
			this.butRecordNew.Location = new System.Drawing.Point(142, 50);
			this.butRecordNew.Name = "butRecordNew";
			this.butRecordNew.Size = new System.Drawing.Size(105, 23);
			this.butRecordNew.TabIndex = 20;
			this.butRecordNew.Text = "Record New";
			this.butRecordNew.Click += new System.EventHandler(this.butRecordNew_Click);
			// 
			// butDeleteSound
			// 
			this.butDeleteSound.Location = new System.Drawing.Point(47, 49);
			this.butDeleteSound.Name = "butDeleteSound";
			this.butDeleteSound.Size = new System.Drawing.Size(55, 23);
			this.butDeleteSound.TabIndex = 19;
			this.butDeleteSound.Text = "Delete";
			this.butDeleteSound.Click += new System.EventHandler(this.butDeleteSound_Click);
			// 
			// butRecord
			// 
			this.butRecord.Location = new System.Drawing.Point(142, 21);
			this.butRecord.Name = "butRecord";
			this.butRecord.Size = new System.Drawing.Size(105, 23);
			this.butRecord.TabIndex = 18;
			this.butRecord.Text = "Windows Recorder";
			this.butRecord.Click += new System.EventHandler(this.butRecord_Click);
			// 
			// butExport
			// 
			this.butExport.Location = new System.Drawing.Point(47, 107);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(55, 23);
			this.butExport.TabIndex = 17;
			this.butExport.Text = "Export";
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// butImport
			// 
			this.butImport.Location = new System.Drawing.Point(47, 78);
			this.butImport.Name = "butImport";
			this.butImport.Size = new System.Drawing.Size(55, 23);
			this.butImport.TabIndex = 16;
			this.butImport.Text = "Import";
			this.butImport.Click += new System.EventHandler(this.butImport_Click);
			// 
			// butPlay
			// 
			this.butPlay.Location = new System.Drawing.Point(47, 20);
			this.butPlay.Name = "butPlay";
			this.butPlay.Size = new System.Drawing.Size(55, 23);
			this.butPlay.TabIndex = 15;
			this.butPlay.Text = "Play";
			this.butPlay.Click += new System.EventHandler(this.butPlay_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(46, 351);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(82, 26);
			this.butDelete.TabIndex = 14;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textLightRow
			// 
			this.textLightRow.Location = new System.Drawing.Point(168, 250);
			this.textLightRow.MaxVal = 255;
			this.textLightRow.MinVal = 0;
			this.textLightRow.Name = "textLightRow";
			this.textLightRow.Size = new System.Drawing.Size(51, 20);
			this.textLightRow.TabIndex = 1;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(442, 351);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(544, 351);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormSigElementDefEdit
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(671, 402);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.butColor);
			this.Controls.Add(this.textLightRow);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textSigText);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listType);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormSigElementDefEdit";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Message Element";
			this.Load += new System.EventHandler(this.FormSigElementDefEdit_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label label1;
		private OpenDental.UI.ListBoxOD listType;
		private Label label2;
		private TextBox textSigText;
		private Label label3;
		private Label label5;
		private Label label6;
		private ValidNum textLightRow;
		private Button butColor;
		private Label label7;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.Button butPlay;
		private OpenDental.UI.Button butImport;
		private GroupBox groupBox1;
		private OpenDental.UI.Button butRecord;
		private OpenDental.UI.Button butExport;
		private OpenDental.UI.Button butDeleteSound;
		private UI.Button butRecordNew;
	}
}
