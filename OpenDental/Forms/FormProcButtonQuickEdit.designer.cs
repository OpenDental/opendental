namespace OpenDental{
	partial class FormProcButtonQuickEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcButtonQuickEdit));
			this.checkIsLabel = new System.Windows.Forms.CheckBox();
			this.textProcedureCode = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textDescript = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.butPickProc = new System.Windows.Forms.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelSurfaces = new System.Windows.Forms.Label();
			this.textSurfaces = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// checkIsLabel
			// 
			this.checkIsLabel.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsLabel.Location = new System.Drawing.Point(84, 91);
			this.checkIsLabel.Name = "checkIsLabel";
			this.checkIsLabel.Size = new System.Drawing.Size(111, 16);
			this.checkIsLabel.TabIndex = 3;
			this.checkIsLabel.Text = "Display as Label";
			this.checkIsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsLabel.UseVisualStyleBackColor = true;
			this.checkIsLabel.CheckedChanged += new System.EventHandler(this.checkIsLabel_CheckedChanged);
			// 
			// textProcedureCode
			// 
			this.textProcedureCode.Location = new System.Drawing.Point(181, 38);
			this.textProcedureCode.Name = "textProcedureCode";
			this.textProcedureCode.Size = new System.Drawing.Size(167, 20);
			this.textProcedureCode.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(82, 41);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(98, 14);
			this.label2.TabIndex = 58;
			this.label2.Text = "Procedure Code";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescript
			// 
			this.textDescript.Location = new System.Drawing.Point(181, 12);
			this.textDescript.Name = "textDescript";
			this.textDescript.Size = new System.Drawing.Size(167, 20);
			this.textDescript.TabIndex = 0;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(82, 15);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(98, 14);
			this.label3.TabIndex = 60;
			this.label3.Text = "Display Text";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPickProc
			// 
			this.butPickProc.Location = new System.Drawing.Point(354, 38);
			this.butPickProc.Name = "butPickProc";
			this.butPickProc.Size = new System.Drawing.Size(24, 21);
			this.butPickProc.TabIndex = 62;
			this.butPickProc.Text = "...";
			this.butPickProc.UseVisualStyleBackColor = true;
			this.butPickProc.Click += new System.EventHandler(this.butPickProc_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 139);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(81, 26);
			this.butDelete.TabIndex = 6;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(273, 141);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(354, 141);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelSurfaces
			// 
			this.labelSurfaces.Location = new System.Drawing.Point(82, 67);
			this.labelSurfaces.Name = "labelSurfaces";
			this.labelSurfaces.Size = new System.Drawing.Size(98, 14);
			this.labelSurfaces.TabIndex = 55;
			this.labelSurfaces.Text = "Surfaces";
			this.labelSurfaces.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSurfaces
			// 
			this.textSurfaces.Location = new System.Drawing.Point(181, 64);
			this.textSurfaces.Name = "textSurfaces";
			this.textSurfaces.Size = new System.Drawing.Size(167, 20);
			this.textSurfaces.TabIndex = 2;
			// 
			// FormProcButtonQuickEdit
			// 
			this.ClientSize = new System.Drawing.Size(441, 175);
			this.Controls.Add(this.butPickProc);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textDescript);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textProcedureCode);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.checkIsLabel);
			this.Controls.Add(this.textSurfaces);
			this.Controls.Add(this.labelSurfaces);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormProcButtonQuickEdit";
			this.Text = "Edit Quick Procedure Button";
			this.Load += new System.EventHandler(this.FormProcButtonQuickEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkIsLabel;
		private System.Windows.Forms.TextBox textProcedureCode;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textDescript;
		private System.Windows.Forms.Label label3;
		private UI.Button butDelete;
		private System.Windows.Forms.Button butPickProc;
		private System.Windows.Forms.Label labelSurfaces;
		private System.Windows.Forms.TextBox textSurfaces;
	}
}