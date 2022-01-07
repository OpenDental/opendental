namespace OpenDental{
	partial class FormWikiSetup {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWikiSetup));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textMaster = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkDetectLinks = new System.Windows.Forms.CheckBox();
			this.checkCreatePageFromLinks = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(776, 717);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(857, 717);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textMaster
			// 
			this.textMaster.AcceptsTab = true;
			this.textMaster.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textMaster.Font = new System.Drawing.Font("Courier New", 9.5F);
			this.textMaster.Location = new System.Drawing.Point(12, 28);
			this.textMaster.Multiline = true;
			this.textMaster.Name = "textMaster";
			this.textMaster.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textMaster.Size = new System.Drawing.Size(920, 667);
			this.textMaster.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(154, 16);
			this.label1.TabIndex = 59;
			this.label1.Text = "Master Page:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkDetectLinks
			// 
			this.checkDetectLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkDetectLinks.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDetectLinks.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkDetectLinks.Location = new System.Drawing.Point(12, 701);
			this.checkDetectLinks.Name = "checkDetectLinks";
			this.checkDetectLinks.Size = new System.Drawing.Size(230, 19);
			this.checkDetectLinks.TabIndex = 76;
			this.checkDetectLinks.Text = "Detect wiki links in textboxes and grids";
			this.checkDetectLinks.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkCreatePageFromLinks
			// 
			this.checkCreatePageFromLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkCreatePageFromLinks.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCreatePageFromLinks.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkCreatePageFromLinks.Location = new System.Drawing.Point(12, 722);
			this.checkCreatePageFromLinks.Name = "checkCreatePageFromLinks";
			this.checkCreatePageFromLinks.Size = new System.Drawing.Size(230, 19);
			this.checkCreatePageFromLinks.TabIndex = 77;
			this.checkCreatePageFromLinks.Text = "Allow new wiki pages from links";
			this.checkCreatePageFromLinks.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormWikiSetup
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(944, 753);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.checkCreatePageFromLinks);
			this.Controls.Add(this.checkDetectLinks);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textMaster);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWikiSetup";
			this.Text = "Wiki Setup";
			this.Load += new System.EventHandler(this.FormWikiSetup_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textMaster;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkDetectLinks;
		private System.Windows.Forms.CheckBox checkCreatePageFromLinks;
	}
}