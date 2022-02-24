namespace OpenDental{
	partial class FormJobEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJobEdit));
			this.userControlJobManagerEditor = new OpenDental.UserControlJobManagerEditor();
			this.SuspendLayout();
			// 
			// userControlJobManagerEditor
			// 
			this.userControlJobManagerEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.userControlJobManagerEditor.JobCur = null;
			this.userControlJobManagerEditor.Location = new System.Drawing.Point(0, 0);
			this.userControlJobManagerEditor.Name = "userControlJobManagerEditor";
			this.userControlJobManagerEditor.Size = new System.Drawing.Size(1213, 885);
			this.userControlJobManagerEditor.TabIndex = 0;
			// 
			// FormJobEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1213, 885);
			this.Controls.Add(this.userControlJobManagerEditor);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormJobEdit";
			this.Text = "Job Edit";
			this.Activated += new System.EventHandler(this.FormJobEdit_Activated);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormJobEdit_FormClosing);
			this.ResumeLayout(false);

		}

		#endregion

		private UserControlJobManagerEditor userControlJobManagerEditor;
	}
}