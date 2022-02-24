using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPasswordReset {
		private System.ComponentModel.IContainer components = null;

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

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPasswordReset));
			this.textMasterPass = new System.Windows.Forms.TextBox();
			this.butOK = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textMasterPass
			// 
			this.textMasterPass.Location = new System.Drawing.Point(120,58);
			this.textMasterPass.MaxLength = 100;
			this.textMasterPass.Name = "textMasterPass";
			this.textMasterPass.Size = new System.Drawing.Size(212,20);
			this.textMasterPass.TabIndex = 35;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(384,96);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(76,26);
			this.butOK.TabIndex = 37;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(18,60);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100,50);
			this.label1.TabIndex = 38;
			this.label1.Text = "Master Password (you must call us)";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(384,132);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(76,26);
			this.butCancel.TabIndex = 39;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(54,5);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(406,26);
			this.label2.TabIndex = 40;
			this.label2.Text = "This form is obsolete and cannot be accessed from anywhere.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// FormPasswordReset
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(478,176);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textMasterPass);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPasswordReset";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Reset Password";
			this.Load += new System.EventHandler(this.FormRP_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textMasterPass;
		private OpenDental.UI.Button butCancel;
		private Label label2;
	}
}
