using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormProcApptColorEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcApptColorEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.textCodeRange = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.panelColor = new System.Windows.Forms.Panel();
			this.labelBeforeTime = new System.Windows.Forms.Label();
			this.checkPrevDate = new System.Windows.Forms.CheckBox();
			this.butChange = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(51, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "Code Range";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCodeRange
			// 
			this.textCodeRange.Location = new System.Drawing.Point(136, 20);
			this.textCodeRange.Name = "textCodeRange";
			this.textCodeRange.Size = new System.Drawing.Size(200, 20);
			this.textCodeRange.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(140, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(92, 13);
			this.label2.TabIndex = 16;
			this.label2.Text = "Ex: D1050-D1060";
			// 
			// panelColor
			// 
			this.panelColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.panelColor.Location = new System.Drawing.Point(136, 64);
			this.panelColor.Name = "panelColor";
			this.panelColor.Size = new System.Drawing.Size(24, 24);
			this.panelColor.TabIndex = 127;
			// 
			// labelBeforeTime
			// 
			this.labelBeforeTime.Location = new System.Drawing.Point(54, 65);
			this.labelBeforeTime.Name = "labelBeforeTime";
			this.labelBeforeTime.Size = new System.Drawing.Size(80, 20);
			this.labelBeforeTime.TabIndex = 126;
			this.labelBeforeTime.Text = "Text Color";
			this.labelBeforeTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPrevDate
			// 
			this.checkPrevDate.AutoSize = true;
			this.checkPrevDate.Location = new System.Drawing.Point(136, 102);
			this.checkPrevDate.Name = "checkPrevDate";
			this.checkPrevDate.Size = new System.Drawing.Size(120, 17);
			this.checkPrevDate.TabIndex = 3;
			this.checkPrevDate.Text = "Show previous date";
			this.checkPrevDate.UseVisualStyleBackColor = true;
			// 
			// butChange
			// 
			this.butChange.Location = new System.Drawing.Point(170, 64);
			this.butChange.Name = "butChange";
			this.butChange.Size = new System.Drawing.Size(75, 24);
			this.butChange.TabIndex = 2;
			this.butChange.Text = "Change";
			this.butChange.Click += new System.EventHandler(this.butChange_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(20, 162);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(92, 24);
			this.butDelete.TabIndex = 6;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(335, 131);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(335, 162);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormProcApptColorEdit
			// 
			this.ClientSize = new System.Drawing.Size(433, 208);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.checkPrevDate);
			this.Controls.Add(this.butChange);
			this.Controls.Add(this.panelColor);
			this.Controls.Add(this.labelBeforeTime);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textCodeRange);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormProcApptColorEdit";
			this.Padding = new System.Windows.Forms.Padding(3);
			this.ShowInTaskbar = false;
			this.Text = "Edit Proc Appt Color";
			this.Load += new System.EventHandler(this.FormProcApptColorEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private ColorDialog colorDialog1;
		private TextBox textCodeRange;
		private Label label2;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.Button butChange;
		private Panel panelColor;
		private Label labelBeforeTime;
		private CheckBox checkPrevDate;
	}
}
