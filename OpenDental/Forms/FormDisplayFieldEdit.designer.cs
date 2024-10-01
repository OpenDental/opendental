using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormDisplayFieldEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDisplayFieldEdit));
			this.labelInternalName = new System.Windows.Forms.Label();
			this.textInternalName = new System.Windows.Forms.TextBox();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textWidthMin = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textWidth = new OpenDental.ValidNum();
			this.butSave = new OpenDental.UI.Button();
			this.labelDescriptionOption = new System.Windows.Forms.Label();
			this.labelDescriptionOverrideOption = new System.Windows.Forms.Label();
			this.textDescriptionOverride = new System.Windows.Forms.TextBox();
			this.labelDescriptionOverride = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblInternalName
			// 
			this.labelInternalName.Location = new System.Drawing.Point(-1, 16);
			this.labelInternalName.Name = "lblInternalName";
			this.labelInternalName.Size = new System.Drawing.Size(140, 17);
			this.labelInternalName.TabIndex = 2;
			this.labelInternalName.Text = "Internal Name";
			this.labelInternalName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInternalName
			// 
			this.textInternalName.Location = new System.Drawing.Point(141, 15);
			this.textInternalName.Name = "textInternalName";
			this.textInternalName.ReadOnly = true;
			this.textInternalName.Size = new System.Drawing.Size(348, 20);
			this.textInternalName.TabIndex = 3;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(141, 41);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(348, 20);
			this.textDescription.TabIndex = 5;
			this.textDescription.TextChanged += new System.EventHandler(this.textDescription_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(-1, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(140, 17);
			this.label2.TabIndex = 4;
			this.label2.Text = "Description";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(-1, 120);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(140, 17);
			this.label3.TabIndex = 6;
			this.label3.Text = "Column Width";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textWidthMin
			// 
			this.textWidthMin.Location = new System.Drawing.Point(141, 93);
			this.textWidthMin.Name = "textWidthMin";
			this.textWidthMin.ReadOnly = true;
			this.textWidthMin.Size = new System.Drawing.Size(71, 20);
			this.textWidthMin.TabIndex = 9;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(2, 94);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(137, 17);
			this.label4.TabIndex = 8;
			this.label4.Text = "Minimum Width";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(215, 94);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(175, 17);
			this.label5.TabIndex = 10;
			this.label5.Text = "(based on text above)";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textWidth
			// 
			this.textWidth.Location = new System.Drawing.Point(141, 119);
			this.textWidth.MaxVal = 2000;
			this.textWidth.MinVal = 1;
			this.textWidth.Name = "textWidth";
			this.textWidth.Size = new System.Drawing.Size(71, 20);
			this.textWidth.TabIndex = 7;
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(510, 116);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 26);
			this.butSave.TabIndex = 1;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// lblDescOption
			// 
			this.labelDescriptionOption.Location = new System.Drawing.Point(491, 42);
			this.labelDescriptionOption.Name = "lblDescOption";
			this.labelDescriptionOption.Size = new System.Drawing.Size(101, 17);
			this.labelDescriptionOption.TabIndex = 95;
			this.labelDescriptionOption.Text = "(optional)";
			this.labelDescriptionOption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblDescOverrideOption
			// 
			this.labelDescriptionOverrideOption.Location = new System.Drawing.Point(491, 68);
			this.labelDescriptionOverrideOption.Name = "lblDescOverrideOption";
			this.labelDescriptionOverrideOption.Size = new System.Drawing.Size(101, 17);
			this.labelDescriptionOverrideOption.TabIndex = 98;
			this.labelDescriptionOverrideOption.Text = "(optional)";
			this.labelDescriptionOverrideOption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textDescriptionOverride
			// 
			this.textDescriptionOverride.Location = new System.Drawing.Point(141, 67);
			this.textDescriptionOverride.Name = "textDescriptionOverride";
			this.textDescriptionOverride.Size = new System.Drawing.Size(348, 20);
			this.textDescriptionOverride.TabIndex = 97;
			this.textDescriptionOverride.TextChanged += new System.EventHandler(this.textDescriptionOverride_TextChanged);
			// 
			// lblDescOverride
			// 
			this.labelDescriptionOverride.Location = new System.Drawing.Point(-1, 68);
			this.labelDescriptionOverride.Name = "lblDescOverride";
			this.labelDescriptionOverride.Size = new System.Drawing.Size(140, 17);
			this.labelDescriptionOverride.TabIndex = 96;
			this.labelDescriptionOverride.Text = "Description Override";
			this.labelDescriptionOverride.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormDisplayFieldEdit
			// 
			this.ClientSize = new System.Drawing.Size(597, 154);
			this.Controls.Add(this.labelDescriptionOverrideOption);
			this.Controls.Add(this.textDescriptionOverride);
			this.Controls.Add(this.labelDescriptionOverride);
			this.Controls.Add(this.labelDescriptionOption);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textWidthMin);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textWidth);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textInternalName);
			this.Controls.Add(this.labelInternalName);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormDisplayFieldEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Display Field";
			this.Load += new System.EventHandler(this.FormDisplayFieldEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		private OpenDental.UI.Button butSave;
		private Label labelInternalName;
		private TextBox textInternalName;
		private TextBox textDescription;
		private Label label2;
		private Label label3;
		private ValidNum textWidth;
		private TextBox textWidthMin;
		private Label label4;
		private Label label5;
		private Label labelDescriptionOption;
		private Label labelDescriptionOverrideOption;
		private TextBox textDescriptionOverride;
		private Label labelDescriptionOverride;
	}
}
