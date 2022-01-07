using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormApptRuleEdit {
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
			OpenDental.UI.Button butDelete;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptRuleEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.textRuleDesc = new System.Windows.Forms.TextBox();
			this.textCodeStart = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textCodeEnd = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkIsEnabled = new System.Windows.Forms.CheckBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			butDelete = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butDelete
			// 
			butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			butDelete.Location = new System.Drawing.Point(15, 178);
			butDelete.Name = "butDelete";
			butDelete.Size = new System.Drawing.Size(75, 26);
			butDelete.TabIndex = 16;
			butDelete.Text = "&Delete";
			butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(111, 20);
			this.label1.TabIndex = 11;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRuleDesc
			// 
			this.textRuleDesc.Location = new System.Drawing.Point(125, 25);
			this.textRuleDesc.Name = "textRuleDesc";
			this.textRuleDesc.Size = new System.Drawing.Size(297, 20);
			this.textRuleDesc.TabIndex = 0;
			// 
			// textCodeStart
			// 
			this.textCodeStart.Location = new System.Drawing.Point(125, 51);
			this.textCodeStart.Name = "textCodeStart";
			this.textCodeStart.Size = new System.Drawing.Size(113, 20);
			this.textCodeStart.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(111, 20);
			this.label2.TabIndex = 18;
			this.label2.Text = "Code Start";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCodeEnd
			// 
			this.textCodeEnd.Location = new System.Drawing.Point(125, 77);
			this.textCodeEnd.Name = "textCodeEnd";
			this.textCodeEnd.Size = new System.Drawing.Size(113, 20);
			this.textCodeEnd.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 76);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(111, 20);
			this.label3.TabIndex = 20;
			this.label3.Text = "Code End";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsEnabled
			// 
			this.checkIsEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsEnabled.Location = new System.Drawing.Point(9, 103);
			this.checkIsEnabled.Name = "checkIsEnabled";
			this.checkIsEnabled.Size = new System.Drawing.Size(130, 21);
			this.checkIsEnabled.TabIndex = 3;
			this.checkIsEnabled.Text = "Is Enabled";
			this.checkIsEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsEnabled.UseVisualStyleBackColor = true;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(347, 146);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(347, 178);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormApptRuleEdit
			// 
			this.ClientSize = new System.Drawing.Size(448, 222);
			this.Controls.Add(this.checkIsEnabled);
			this.Controls.Add(this.textCodeEnd);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textCodeStart);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textRuleDesc);
			this.Controls.Add(butDelete);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormApptRuleEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Appointment Rule";
			this.Load += new System.EventHandler(this.FormApptRuleEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label label1;
		private TextBox textRuleDesc;
		private TextBox textCodeStart;
		private Label label2;
		private TextBox textCodeEnd;
		private Label label3;
		private CheckBox checkIsEnabled;
	}
}
