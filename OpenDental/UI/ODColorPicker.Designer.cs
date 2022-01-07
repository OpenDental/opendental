namespace OpenDental.UI {
	partial class ODColorPicker {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.butColor = new System.Windows.Forms.Button();
			this.butNone = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butColor
			// 
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColor.Location = new System.Drawing.Point(1, 0);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(30, 20);
			this.butColor.TabIndex = 1;
			this.butColor.UseVisualStyleBackColor = true;
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			// 
			// butNone
			// 
			this.butNone.Location = new System.Drawing.Point(34, 0);
			this.butNone.Name = "butNone";
			this.butNone.Size = new System.Drawing.Size(40, 21);
			this.butNone.TabIndex = 0;
			this.butNone.Text = "None";
			this.butNone.UseVisualStyleBackColor = true;
			this.butNone.Visible = false;
			this.butNone.Click += new System.EventHandler(this.butNone_Click);
			// 
			// ODColorPicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.butColor);
			this.Controls.Add(this.butNone);
			this.Name = "ODColorPicker";
			this.Size = new System.Drawing.Size(74, 21);
			this.ResumeLayout(false);

		}

		#endregion

		private Button butNone;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.Button butColor;
	}
}
