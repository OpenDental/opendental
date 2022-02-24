using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormDisplayFieldOrthoEdit {
		/// <summary>Required designer variable.</summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDisplayFieldOrthoEdit));
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textWidthMin = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.labelPickList = new System.Windows.Forms.Label();
			this.textPickList = new System.Windows.Forms.TextBox();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.textWidth = new OpenDental.ValidNum();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textDescriptionOverride = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.labelDefaultMode = new System.Windows.Forms.Label();
			this.groupBoxOD1 = new OpenDental.UI.GroupBoxOD();
			this.radioText = new System.Windows.Forms.RadioButton();
			this.radioPickList = new System.Windows.Forms.RadioButton();
			this.radioSignature = new System.Windows.Forms.RadioButton();
			this.radioProvider = new System.Windows.Forms.RadioButton();
			this.groupBoxOD1.SuspendLayout();
			this.SuspendLayout();
			// 
			// textDescription
			// 
			this.textDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDescription.Location = new System.Drawing.Point(142, 40);
			this.textDescription.MaxLength = 255;
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(249, 20);
			this.textDescription.TabIndex = 5;
			this.textDescription.TextChanged += new System.EventHandler(this.textInternalName_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 41);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(134, 17);
			this.label2.TabIndex = 4;
			this.label2.Text = "Description";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 93);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(134, 17);
			this.label3.TabIndex = 6;
			this.label3.Text = "Column Width";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textWidthMin
			// 
			this.textWidthMin.Location = new System.Drawing.Point(142, 66);
			this.textWidthMin.Name = "textWidthMin";
			this.textWidthMin.ReadOnly = true;
			this.textWidthMin.Size = new System.Drawing.Size(71, 20);
			this.textWidthMin.TabIndex = 9;
			this.textWidthMin.TabStop = false;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(9, 67);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(131, 17);
			this.label4.TabIndex = 8;
			this.label4.Text = "Minimum Width";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(216, 67);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(175, 17);
			this.label5.TabIndex = 10;
			this.label5.Text = "(based on text above)";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPickList
			// 
			this.labelPickList.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelPickList.Location = new System.Drawing.Point(8, 224);
			this.labelPickList.Name = "labelPickList";
			this.labelPickList.Size = new System.Drawing.Size(130, 48);
			this.labelPickList.TabIndex = 89;
			this.labelPickList.Text = "Pick List\r\n(one entry per line)";
			this.labelPickList.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textPickList
			// 
			this.textPickList.AcceptsReturn = true;
			this.textPickList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textPickList.HideSelection = false;
			this.textPickList.Location = new System.Drawing.Point(142, 223);
			this.textPickList.Multiline = true;
			this.textPickList.Name = "textPickList";
			this.textPickList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textPickList.Size = new System.Drawing.Size(249, 250);
			this.textPickList.TabIndex = 20;
			// 
			// butDown
			// 
			this.butDown.AdjustImageLocation = new System.Drawing.Point(1, 0);
			this.butDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.Location = new System.Drawing.Point(445, 224);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(25, 24);
			this.butDown.TabIndex = 30;
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butUp
			// 
			this.butUp.AdjustImageLocation = new System.Drawing.Point(1, 0);
			this.butUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.Location = new System.Drawing.Point(414, 224);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(25, 24);
			this.butUp.TabIndex = 25;
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// textWidth
			// 
			this.textWidth.Location = new System.Drawing.Point(142, 92);
			this.textWidth.MaxVal = 2000;
			this.textWidth.MinVal = 1;
			this.textWidth.Name = "textWidth";
			this.textWidth.Size = new System.Drawing.Size(71, 20);
			this.textWidth.TabIndex = 10;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(414, 424);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 35;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(414, 459);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 40;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textDescriptionOverride
			// 
			this.textDescriptionOverride.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDescriptionOverride.Location = new System.Drawing.Point(142, 14);
			this.textDescriptionOverride.MaxLength = 255;
			this.textDescriptionOverride.Name = "textDescriptionOverride";
			this.textDescriptionOverride.Size = new System.Drawing.Size(249, 20);
			this.textDescriptionOverride.TabIndex = 93;
			this.textDescriptionOverride.TextChanged += new System.EventHandler(this.textDisplayName_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(134, 17);
			this.label1.TabIndex = 92;
			this.label1.Text = "Description Override";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.Location = new System.Drawing.Point(393, 15);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(101, 17);
			this.label6.TabIndex = 94;
			this.label6.Text = "(optional)";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelDefaultMode
			// 
			this.labelDefaultMode.Location = new System.Drawing.Point(216, 92);
			this.labelDefaultMode.Name = "labelDefaultMode";
			this.labelDefaultMode.Size = new System.Drawing.Size(175, 17);
			this.labelDefaultMode.TabIndex = 95;
			this.labelDefaultMode.Text = "(default)";
			this.labelDefaultMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBoxOD1
			// 
			this.groupBoxOD1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxOD1.Controls.Add(this.radioProvider);
			this.groupBoxOD1.Controls.Add(this.radioSignature);
			this.groupBoxOD1.Controls.Add(this.radioPickList);
			this.groupBoxOD1.Controls.Add(this.radioText);
			this.groupBoxOD1.Location = new System.Drawing.Point(130, 118);
			this.groupBoxOD1.Name = "groupBoxOD1";
			this.groupBoxOD1.Size = new System.Drawing.Size(107, 98);
			this.groupBoxOD1.TabIndex = 96;
			this.groupBoxOD1.TabStop = false;
			this.groupBoxOD1.Text = "Type";
			// 
			// radioText
			// 
			this.radioText.AutoCheck = false;
			this.radioText.Location = new System.Drawing.Point(12, 19);
			this.radioText.Name = "radioText";
			this.radioText.Size = new System.Drawing.Size(88, 18);
			this.radioText.TabIndex = 0;
			this.radioText.TabStop = true;
			this.radioText.Text = "Text";
			this.radioText.UseVisualStyleBackColor = true;
			this.radioText.Click += new System.EventHandler(this.radioText_Click);
			// 
			// radioPickList
			// 
			this.radioPickList.AutoCheck = false;
			this.radioPickList.Location = new System.Drawing.Point(12, 37);
			this.radioPickList.Name = "radioPickList";
			this.radioPickList.Size = new System.Drawing.Size(88, 18);
			this.radioPickList.TabIndex = 1;
			this.radioPickList.TabStop = true;
			this.radioPickList.Text = "Pick List";
			this.radioPickList.UseVisualStyleBackColor = true;
			this.radioPickList.Click += new System.EventHandler(this.radioPickList_Click);
			// 
			// radioSignature
			// 
			this.radioSignature.AutoCheck = false;
			this.radioSignature.Location = new System.Drawing.Point(12, 55);
			this.radioSignature.Name = "radioSignature";
			this.radioSignature.Size = new System.Drawing.Size(88, 18);
			this.radioSignature.TabIndex = 2;
			this.radioSignature.TabStop = true;
			this.radioSignature.Text = "Signature";
			this.radioSignature.UseVisualStyleBackColor = true;
			this.radioSignature.Click += new System.EventHandler(this.radioSignature_Click);
			// 
			// radioProvider
			// 
			this.radioProvider.AutoCheck = false;
			this.radioProvider.Location = new System.Drawing.Point(12, 73);
			this.radioProvider.Name = "radioProvider";
			this.radioProvider.Size = new System.Drawing.Size(88, 18);
			this.radioProvider.TabIndex = 3;
			this.radioProvider.TabStop = true;
			this.radioProvider.Text = "Provider";
			this.radioProvider.UseVisualStyleBackColor = true;
			this.radioProvider.Click += new System.EventHandler(this.radioProvider_Click);
			// 
			// FormDisplayFieldOrthoEdit
			// 
			this.ClientSize = new System.Drawing.Size(510, 497);
			this.Controls.Add(this.groupBoxOD1);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelDefaultMode);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textDescriptionOverride);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.labelPickList);
			this.Controls.Add(this.textPickList);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textWidthMin);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textWidth);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormDisplayFieldOrthoEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Ortho Display Field";
			this.Load += new System.EventHandler(this.FormDisplayFieldOrthoEdit_Load);
			this.groupBoxOD1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private TextBox textDescription;
		private Label label2;
		private Label label3;
		private ValidNum textWidth;
		private TextBox textWidthMin;
		private Label label4;
		private Label label5;
		private Label labelPickList;
		private TextBox textPickList;
		private UI.Button butDown;
		private UI.Button butUp;
		private TextBox textDescriptionOverride;
		private Label label1;
		private Label label6;
		private Label labelDefaultMode;
		private UI.GroupBoxOD groupBoxOD1;
		private RadioButton radioProvider;
		private RadioButton radioSignature;
		private RadioButton radioPickList;
		private RadioButton radioText;
	}
}
