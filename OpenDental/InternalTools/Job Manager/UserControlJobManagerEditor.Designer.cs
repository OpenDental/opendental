using OpenDental.InternalTools.Job_Manager;

namespace OpenDental {
	partial class UserControlJobManagerEditor {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.userControlJobEdit = new OpenDental.InternalTools.Job_Manager.UserControlJobEdit();
			this.userControlMarketingEdit = new OpenDental.InternalTools.Job_Manager.UserControlMarketingEdit();
			this.userControlQueryEdit = new OpenDental.InternalTools.Job_Manager.UserControlQueryEdit();
			this.SuspendLayout();
			// 
			// userControlJobEdit
			// 
			this.userControlJobEdit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.userControlJobEdit.IsOverride = false;
			this.userControlJobEdit.Location = new System.Drawing.Point(0, 0);
			this.userControlJobEdit.Name = "userControlJobEdit";
			this.userControlJobEdit.Size = new System.Drawing.Size(1220, 623);
			this.userControlJobEdit.TabIndex = 0;
			this.userControlJobEdit.SaveClick += new System.EventHandler(this.userControlJobEdit_SaveClick);
			this.userControlJobEdit.RequestJob += new OpenDental.InternalTools.Job_Manager.UserControlJobEdit.RequestJobEvent(this.userControlJobEdit_RequestJob);
			// 
			// userControlMarketingEdit
			// 
			this.userControlMarketingEdit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.userControlMarketingEdit.IsOverride = false;
			this.userControlMarketingEdit.Location = new System.Drawing.Point(0, 0);
			this.userControlMarketingEdit.Name = "userControlMarketingEdit";
			this.userControlMarketingEdit.Size = new System.Drawing.Size(1220, 623);
			this.userControlMarketingEdit.TabIndex = 1;
			this.userControlMarketingEdit.SaveClick += new System.EventHandler(this.userControlMarketingEdit_SaveClick);
			// 
			// userControlQueryEdit
			// 
			this.userControlQueryEdit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.userControlQueryEdit.IsOverride = false;
			this.userControlQueryEdit.Location = new System.Drawing.Point(0, 0);
			this.userControlQueryEdit.Name = "userControlQueryEdit";
			this.userControlQueryEdit.Size = new System.Drawing.Size(1220, 623);
			this.userControlQueryEdit.TabIndex = 2;
			this.userControlQueryEdit.SaveClick += new System.EventHandler(this.userControlQueryEdit_SaveClick);
			// 
			// UserControlJobManagerEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.userControlJobEdit);
			this.Controls.Add(this.userControlQueryEdit);
			this.Controls.Add(this.userControlMarketingEdit);
			this.Name = "UserControlJobManagerEditor";
			this.Size = new System.Drawing.Size(1220, 623);
			this.ResumeLayout(false);

		}

		#endregion

		private UserControlJobEdit userControlJobEdit;
		private UserControlMarketingEdit userControlMarketingEdit;
		private UserControlQueryEdit userControlQueryEdit;
	}
}
