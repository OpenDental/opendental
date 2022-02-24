using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormDefEditBlockout {
		private System.ComponentModel.IContainer components = null;// Required designer variable.

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDefEditBlockout));
			this.labelName = new System.Windows.Forms.Label();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkHidden = new System.Windows.Forms.CheckBox();
			this.groupBoxUsage = new System.Windows.Forms.GroupBox();
			this.checkOverlap = new System.Windows.Forms.CheckBox();
			this.checkCutCopyPaste = new System.Windows.Forms.CheckBox();
			this.butColor = new System.Windows.Forms.Button();
			this.labelColor = new System.Windows.Forms.Label();
			this.textName = new System.Windows.Forms.TextBox();
			this.groupBoxUsage.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(9, 38);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(150, 16);
			this.labelName.TabIndex = 0;
			this.labelName.Text = "Name";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// colorDialog1
			// 
			this.colorDialog1.FullOpen = true;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(325, 135);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 25);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(406, 135);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 25);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkHidden
			// 
			this.checkHidden.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHidden.Location = new System.Drawing.Point(12, 11);
			this.checkHidden.Name = "checkHidden";
			this.checkHidden.Size = new System.Drawing.Size(99, 24);
			this.checkHidden.TabIndex = 3;
			this.checkHidden.Text = "Hidden";
			// 
			// groupBoxUsage
			// 
			this.groupBoxUsage.Controls.Add(this.checkOverlap);
			this.groupBoxUsage.Controls.Add(this.checkCutCopyPaste);
			this.groupBoxUsage.Location = new System.Drawing.Point(196, 54);
			this.groupBoxUsage.Name = "groupBoxUsage";
			this.groupBoxUsage.Size = new System.Drawing.Size(215, 62);
			this.groupBoxUsage.TabIndex = 7;
			this.groupBoxUsage.TabStop = false;
			this.groupBoxUsage.Text = "Usage";
			// 
			// checkOverlap
			// 
			this.checkOverlap.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkOverlap.Location = new System.Drawing.Point(8, 17);
			this.checkOverlap.Name = "checkOverlap";
			this.checkOverlap.Size = new System.Drawing.Size(196, 18);
			this.checkOverlap.TabIndex = 0;
			this.checkOverlap.Text = "Block appointments scheduling";
			this.checkOverlap.UseVisualStyleBackColor = true;
			// 
			// checkCutCopyPaste
			// 
			this.checkCutCopyPaste.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkCutCopyPaste.Location = new System.Drawing.Point(8, 35);
			this.checkCutCopyPaste.Name = "checkCutCopyPaste";
			this.checkCutCopyPaste.Size = new System.Drawing.Size(201, 18);
			this.checkCutCopyPaste.TabIndex = 1;
			this.checkCutCopyPaste.Text = "Disable Cut/Copy/Paste";
			// 
			// butColor
			// 
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColor.Location = new System.Drawing.Point(419, 60);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(30, 20);
			this.butColor.TabIndex = 1;
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			// 
			// labelColor
			// 
			this.labelColor.Location = new System.Drawing.Point(419, 42);
			this.labelColor.Name = "labelColor";
			this.labelColor.Size = new System.Drawing.Size(74, 16);
			this.labelColor.TabIndex = 10;
			this.labelColor.Text = "Color";
			this.labelColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(12, 54);
			this.textName.Multiline = true;
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(178, 64);
			this.textName.TabIndex = 11;
			// 
			// FormDefEditBlockout
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(493, 172);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.butColor);
			this.Controls.Add(this.labelColor);
			this.Controls.Add(this.groupBoxUsage);
			this.Controls.Add(this.checkHidden);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelName);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormDefEditBlockout";
			this.ShowInTaskbar = false;
			this.Text = "Edit Blockout Type";
			this.Load += new System.EventHandler(this.FormDefEdit_Load);
			this.groupBoxUsage.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkHidden;
		private CheckBox checkCutCopyPaste;
		private CheckBox checkOverlap;
		private Button butColor;
		private Label labelColor;
		private TextBox textName;
		private GroupBox groupBoxUsage;
	}
}
