namespace OpenDental{
	partial class FormERouting {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormERouting));
			this.gridActions = new OpenDental.UI.GridOD();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textDate = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textPatName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// gridActions
			// 
			this.gridActions.Location = new System.Drawing.Point(12, 98);
			this.gridActions.Name = "gridActions";
			this.gridActions.Size = new System.Drawing.Size(575, 238);
			this.gridActions.TabIndex = 4;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(115, 64);
			this.textDescription.Name = "textDescription";
			this.textDescription.ReadOnly = true;
			this.textDescription.Size = new System.Drawing.Size(205, 20);
			this.textDescription.TabIndex = 5;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(112, 24);
			this.label1.TabIndex = 6;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(22, 35);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(90, 24);
			this.label3.TabIndex = 12;
			this.label3.Text = "Date";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(115, 38);
			this.textDate.Name = "textDate";
			this.textDate.ReadOnly = true;
			this.textDate.Size = new System.Drawing.Size(99, 20);
			this.textDate.TabIndex = 11;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(22, 12);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(90, 24);
			this.label4.TabIndex = 10;
			this.label4.Text = "Name";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatName
			// 
			this.textPatName.Location = new System.Drawing.Point(115, 12);
			this.textPatName.Name = "textPatName";
			this.textPatName.ReadOnly = true;
			this.textPatName.Size = new System.Drawing.Size(205, 20);
			this.textPatName.TabIndex = 9;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 341);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(433, 24);
			this.label2.TabIndex = 13;
			this.label2.Text = "Actions can only be marked complete from the app";
			// 
			// FormERouting
			// 
			this.ClientSize = new System.Drawing.Size(599, 391);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textPatName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.gridActions);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormERouting";
			this.Text = "eRouting";
			this.Load += new System.EventHandler(this.FormPatientFlow_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private UI.GridOD gridActions;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textDate;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textPatName;
		private System.Windows.Forms.Label label2;
	}
}