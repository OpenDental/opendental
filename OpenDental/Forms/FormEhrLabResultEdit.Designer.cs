namespace OpenDental {
	partial class FormEhrLabResultEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrLabResultEdit));
			this.butCancel = new System.Windows.Forms.Button();
			this.butOk = new System.Windows.Forms.Button();
			this.butDelete = new System.Windows.Forms.Button();
			this.textDateTimeTest = new System.Windows.Forms.TextBox();
			this.textTestName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textTestID = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textObsValue = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.comboAbnormalFlag = new System.Windows.Forms.ComboBox();
			this.textObsUnits = new System.Windows.Forms.TextBox();
			this.textObsRange = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(324, 303);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 6;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.Location = new System.Drawing.Point(243, 303);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(75, 24);
			this.butOk.TabIndex = 5;
			this.butOk.Text = "Ok";
			this.butOk.UseVisualStyleBackColor = true;
			this.butOk.Click += new System.EventHandler(this.butOk_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(12, 303);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 7;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textDateTimeTest
			// 
			this.textDateTimeTest.Location = new System.Drawing.Point(133, 22);
			this.textDateTimeTest.Name = "textDateTimeTest";
			this.textDateTimeTest.Size = new System.Drawing.Size(152, 20);
			this.textDateTimeTest.TabIndex = 0;
			// 
			// textTestName
			// 
			this.textTestName.Location = new System.Drawing.Point(81, 48);
			this.textTestName.Name = "textTestName";
			this.textTestName.Size = new System.Drawing.Size(178, 20);
			this.textTestName.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(64, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(67, 17);
			this.label3.TabIndex = 8;
			this.label3.Text = "Date Time";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(9, 50);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(70, 17);
			this.label4.TabIndex = 3;
			this.label4.Text = "Name";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textTestID);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.textTestName);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Location = new System.Drawing.Point(52, 58);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(292, 80);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Test Performed";
			// 
			// textTestID
			// 
			this.textTestID.Location = new System.Drawing.Point(81, 22);
			this.textTestID.Name = "textTestID";
			this.textTestID.Size = new System.Drawing.Size(106, 20);
			this.textTestID.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(9, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "LOINC";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textObsValue
			// 
			this.textObsValue.Location = new System.Drawing.Point(98, 16);
			this.textObsValue.Name = "textObsValue";
			this.textObsValue.Size = new System.Drawing.Size(78, 20);
			this.textObsValue.TabIndex = 3;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(26, 18);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(70, 17);
			this.label5.TabIndex = 10;
			this.label5.Text = "Value";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(26, 45);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(70, 17);
			this.label6.TabIndex = 11;
			this.label6.Text = "Units";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.comboAbnormalFlag);
			this.groupBox2.Controls.Add(this.textObsUnits);
			this.groupBox2.Controls.Add(this.textObsRange);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.textObsValue);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Location = new System.Drawing.Point(52, 152);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(292, 133);
			this.groupBox2.TabIndex = 12;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Test Result";
			// 
			// comboAbnormalFlag
			// 
			this.comboAbnormalFlag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboAbnormalFlag.FormattingEnabled = true;
			this.comboAbnormalFlag.Location = new System.Drawing.Point(98, 95);
			this.comboAbnormalFlag.Name = "comboAbnormalFlag";
			this.comboAbnormalFlag.Size = new System.Drawing.Size(178, 21);
			this.comboAbnormalFlag.TabIndex = 15;
			this.comboAbnormalFlag.SelectedIndexChanged += new System.EventHandler(this.comboAbnormalFlag_SelectedIndexChanged);
			// 
			// textObsUnits
			// 
			this.textObsUnits.Location = new System.Drawing.Point(98, 42);
			this.textObsUnits.Name = "textObsUnits";
			this.textObsUnits.Size = new System.Drawing.Size(78, 20);
			this.textObsUnits.TabIndex = 14;
			// 
			// textObsRange
			// 
			this.textObsRange.Location = new System.Drawing.Point(98, 69);
			this.textObsRange.Name = "textObsRange";
			this.textObsRange.Size = new System.Drawing.Size(116, 20);
			this.textObsRange.TabIndex = 12;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(6, 99);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(86, 17);
			this.label7.TabIndex = 13;
			this.label7.Text = "Abnormal Flag";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(26, 71);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(70, 17);
			this.label2.TabIndex = 13;
			this.label2.Text = "Range";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormEhrLabResultEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(415, 338);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDateTimeTest);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrLabResultEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "FormLabResultEdit";
			this.Load += new System.EventHandler(this.FormLabResultEdit_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butOk;
		private System.Windows.Forms.Button butDelete;
		private System.Windows.Forms.TextBox textDateTimeTest;
		private System.Windows.Forms.TextBox textTestName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textTestID;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textObsValue;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textObsRange;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textObsUnits;
		private System.Windows.Forms.ComboBox comboAbnormalFlag;
		private System.Windows.Forms.Label label7;
	}
}