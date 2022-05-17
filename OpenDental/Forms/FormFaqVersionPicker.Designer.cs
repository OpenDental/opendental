namespace OpenDental {
	partial class FormFaqVersionPicker {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFaqVersionPicker));
			this.listBoxMain = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.butCancel = new System.Windows.Forms.Button();
			this.butOk = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listBoxMain
			// 
			this.listBoxMain.Location = new System.Drawing.Point(12, 51);
			this.listBoxMain.Name = "listBoxMain";
			this.listBoxMain.SelectionMode = UI.SelectionMode.MultiExtended;
			this.listBoxMain.Size = new System.Drawing.Size(182, 225);
			this.listBoxMain.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(202, 39);
			this.label1.TabIndex = 1;
			this.label1.Text = "Select the version(s) of the manual the FAQ should be linked to.";
			// 
			// butCancel
			// 
			this.butCancel.Location = new System.Drawing.Point(200, 253);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.ButCancel_Click);
			// 
			// butOk
			// 
			this.butOk.Location = new System.Drawing.Point(200, 223);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(75, 24);
			this.butOk.TabIndex = 3;
			this.butOk.Text = "&OK";
			this.butOk.UseVisualStyleBackColor = true;
			this.butOk.Click += new System.EventHandler(this.ButOk_Click);
			// 
			// FormFaqVersionPicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(279, 288);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listBoxMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFaqVersionPicker";
			this.Text = "FormFaqVersionPicker";
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.ListBoxOD listBoxMain;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butOk;
	}
}