namespace OpenDental {
	partial class FormPayorTypeEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayorTypeEdit));
			this.butDelete = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.textNote = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.comboSopCode = new System.Windows.Forms.ComboBox();
			this.textDate = new ValidDate();
			this.SuspendLayout();
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(11, 208);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 127;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(537, 208);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 126;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(456, 208);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 125;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(95, 69);
			this.textNote.MaxLength = 2000;
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(490, 105);
			this.textNote.TabIndex = 134;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(87, 21);
			this.label2.TabIndex = 137;
			this.label2.Text = "Payor Type";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(7, 16);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(86, 17);
			this.label7.TabIndex = 136;
			this.label7.Text = "Start Date";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 69);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(87, 21);
			this.label5.TabIndex = 135;
			this.label5.Text = "Note";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSopCode
			// 
			this.comboSopCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSopCode.Location = new System.Drawing.Point(95, 42);
			this.comboSopCode.MaxDropDownItems = 30;
			this.comboSopCode.Name = "comboSopCode";
			this.comboSopCode.Size = new System.Drawing.Size(490, 21);
			this.comboSopCode.TabIndex = 138;
			this.comboSopCode.DropDown += new System.EventHandler(this.comboSopCode_DropDown);
			this.comboSopCode.SelectionChangeCommitted += new System.EventHandler(this.comboSopCode_SelectionChangeCommitted);
			this.comboSopCode.DropDownClosed += new System.EventHandler(this.comboSopCode_DropDownClosed);
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(95, 15);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(100, 20);
			this.textDate.TabIndex = 139;
			// 
			// FormPayorTypeEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(624, 244);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.comboSopCode);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPayorTypeEdit";
			this.Text = "Edit Payor Type";
			this.Load += new System.EventHandler(this.FormPayorTypeEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.Button butDelete;
		private UI.Button butCancel;
		private UI.Button butOK;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox comboSopCode;
		private ValidDate textDate;



	}
}