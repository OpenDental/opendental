namespace OpenDental {
	partial class FormEhrLabPanelImport {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrLabPanelImport));
			this.textHL7Raw = new System.Windows.Forms.TextBox();
			this.textPatName = new System.Windows.Forms.TextBox();
			this.textPatIDNum = new System.Windows.Forms.TextBox();
			this.textPatAccountNum = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.butCancel = new System.Windows.Forms.Button();
			this.butOk = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textDateTimeTest = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textServiceID = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textServiceName = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.textPatName2 = new System.Windows.Forms.TextBox();
			this.butChangePat = new System.Windows.Forms.Button();
			this.butReceive = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// textHL7Raw
			// 
			this.textHL7Raw.AcceptsReturn = true;
			this.textHL7Raw.AcceptsTab = true;
			this.textHL7Raw.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textHL7Raw.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textHL7Raw.Location = new System.Drawing.Point(136, 41);
			this.textHL7Raw.Multiline = true;
			this.textHL7Raw.Name = "textHL7Raw";
			this.textHL7Raw.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textHL7Raw.Size = new System.Drawing.Size(747, 205);
			this.textHL7Raw.TabIndex = 1;
			this.textHL7Raw.WordWrap = false;
			this.textHL7Raw.TextChanged += new System.EventHandler(this.textHL7Raw_TextChanged);
			// 
			// textPatName
			// 
			this.textPatName.Location = new System.Drawing.Point(124, 24);
			this.textPatName.Name = "textPatName";
			this.textPatName.ReadOnly = true;
			this.textPatName.Size = new System.Drawing.Size(301, 20);
			this.textPatName.TabIndex = 1;
			// 
			// textPatIDNum
			// 
			this.textPatIDNum.Location = new System.Drawing.Point(124, 50);
			this.textPatIDNum.Name = "textPatIDNum";
			this.textPatIDNum.ReadOnly = true;
			this.textPatIDNum.Size = new System.Drawing.Size(79, 20);
			this.textPatIDNum.TabIndex = 2;
			// 
			// textPatAccountNum
			// 
			this.textPatAccountNum.Location = new System.Drawing.Point(124, 76);
			this.textPatAccountNum.Name = "textPatAccountNum";
			this.textPatAccountNum.ReadOnly = true;
			this.textPatAccountNum.Size = new System.Drawing.Size(110, 20);
			this.textPatAccountNum.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(17, 42);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(118, 17);
			this.label1.TabIndex = 4;
			this.label1.Text = "Raw HL7 Message";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 25);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(118, 17);
			this.label2.TabIndex = 5;
			this.label2.Text = "Patient Name";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(5, 51);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(118, 17);
			this.label3.TabIndex = 6;
			this.label3.Text = "Patient ID";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(5, 77);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(118, 17);
			this.label4.TabIndex = 7;
			this.label4.Text = "Patient Account";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(807, 501);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 23);
			this.butCancel.TabIndex = 8;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.Location = new System.Drawing.Point(726, 501);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(75, 23);
			this.butOk.TabIndex = 0;
			this.butOk.Text = "Ok";
			this.butOk.UseVisualStyleBackColor = true;
			this.butOk.Click += new System.EventHandler(this.butOk_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.textDateTimeTest);
			this.groupBox1.Controls.Add(this.groupBox2);
			this.groupBox1.Controls.Add(this.textPatName);
			this.groupBox1.Controls.Add(this.textPatIDNum);
			this.groupBox1.Controls.Add(this.textPatAccountNum);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Location = new System.Drawing.Point(12, 262);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(435, 228);
			this.groupBox1.TabIndex = 13;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Info From Incoming Message";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(55, 104);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(67, 17);
			this.label8.TabIndex = 10;
			this.label8.Text = "Date";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateTimeTest
			// 
			this.textDateTimeTest.Location = new System.Drawing.Point(124, 102);
			this.textDateTimeTest.Name = "textDateTimeTest";
			this.textDateTimeTest.ReadOnly = true;
			this.textDateTimeTest.Size = new System.Drawing.Size(106, 20);
			this.textDateTimeTest.TabIndex = 9;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textServiceID);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.textServiceName);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Location = new System.Drawing.Point(44, 133);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(381, 80);
			this.groupBox2.TabIndex = 8;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Service";
			// 
			// textServiceID
			// 
			this.textServiceID.Location = new System.Drawing.Point(80, 22);
			this.textServiceID.Name = "textServiceID";
			this.textServiceID.ReadOnly = true;
			this.textServiceID.Size = new System.Drawing.Size(106, 20);
			this.textServiceID.TabIndex = 0;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 24);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(70, 17);
			this.label5.TabIndex = 2;
			this.label5.Text = "LOINC";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textServiceName
			// 
			this.textServiceName.Location = new System.Drawing.Point(80, 48);
			this.textServiceName.Name = "textServiceName";
			this.textServiceName.ReadOnly = true;
			this.textServiceName.Size = new System.Drawing.Size(284, 20);
			this.textServiceName.TabIndex = 1;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 50);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(70, 17);
			this.label7.TabIndex = 3;
			this.label7.Text = "Name";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(467, 257);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(334, 32);
			this.label6.TabIndex = 14;
			this.label6.Text = "Attach these lab results to a lab order by selecting a patient first, and then se" +
    "lecting one lab order from the list.";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// gridMain
			// 
			this.gridMain.Location = new System.Drawing.Point(470, 319);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(412, 171);
			this.gridMain.TabIndex = 10;
			this.gridMain.Title = "Lab Orders for the Patient Above";
			this.gridMain.TranslationName = "TablePatientOrders";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// textPatName2
			// 
			this.textPatName2.Location = new System.Drawing.Point(470, 292);
			this.textPatName2.Name = "textPatName2";
			this.textPatName2.ReadOnly = true;
			this.textPatName2.Size = new System.Drawing.Size(349, 20);
			this.textPatName2.TabIndex = 15;
			// 
			// butChangePat
			// 
			this.butChangePat.Location = new System.Drawing.Point(823, 290);
			this.butChangePat.Name = "butChangePat";
			this.butChangePat.Size = new System.Drawing.Size(59, 23);
			this.butChangePat.TabIndex = 16;
			this.butChangePat.Text = "Change";
			this.butChangePat.UseVisualStyleBackColor = true;
			this.butChangePat.Click += new System.EventHandler(this.butChangePat_Click);
			// 
			// butReceive
			// 
			this.butReceive.Location = new System.Drawing.Point(136, 12);
			this.butReceive.Name = "butReceive";
			this.butReceive.Size = new System.Drawing.Size(94, 23);
			this.butReceive.TabIndex = 17;
			this.butReceive.Text = "Receive";
			this.butReceive.UseVisualStyleBackColor = true;
			this.butReceive.Click += new System.EventHandler(this.butReceive_Click);
			// 
			// FormEhrLabPanelImport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(895, 536);
			this.Controls.Add(this.butReceive);
			this.Controls.Add(this.butChangePat);
			this.Controls.Add(this.textPatName2);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textHL7Raw);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrLabPanelImport";
			this.Text = "Import Lab Results";
			this.Load += new System.EventHandler(this.FormLabPanelImport_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textHL7Raw;
		private System.Windows.Forms.TextBox textPatName;
		private System.Windows.Forms.TextBox textPatIDNum;
		private System.Windows.Forms.TextBox textPatAccountNum;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butOk;
		private OpenDental.UI.GridOD gridMain;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textServiceID;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textServiceName;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textDateTimeTest;
		private System.Windows.Forms.TextBox textPatName2;
		private System.Windows.Forms.Button butChangePat;
		private System.Windows.Forms.Button butReceive;
	}
}