namespace xCrudGenerator {
	partial class Form1 {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.label1 = new System.Windows.Forms.Label();
			this.butRun = new System.Windows.Forms.Button();
			this.textDb = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.comboType = new System.Windows.Forms.ComboBox();
			this.butSnippet = new System.Windows.Forms.Button();
			this.textSnippet = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.listClass = new System.Windows.Forms.ListBox();
			this.checkRun = new System.Windows.Forms.CheckBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.checkRunSchema = new System.Windows.Forms.CheckBox();
			this.label10 = new System.Windows.Forms.Label();
			this.checkRunQueries = new System.Windows.Forms.CheckBox();
			this.checkAddToConvertScript = new System.Windows.Forms.CheckBox();
			this.butAddPrefs = new System.Windows.Forms.Button();
			this.labelPortNum = new System.Windows.Forms.Label();
			this.textPortNum = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(406, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(421, 54);
			this.label1.TabIndex = 0;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// butRun
			// 
			this.butRun.Location = new System.Drawing.Point(868, 14);
			this.butRun.Name = "butRun";
			this.butRun.Size = new System.Drawing.Size(75, 23);
			this.butRun.TabIndex = 1;
			this.butRun.Text = "Run";
			this.butRun.UseVisualStyleBackColor = true;
			this.butRun.Click += new System.EventHandler(this.butRun_Click);
			// 
			// textDb
			// 
			this.textDb.Location = new System.Drawing.Point(105, 48);
			this.textDb.Name = "textDb";
			this.textDb.Size = new System.Drawing.Size(127, 20);
			this.textDb.TabIndex = 5;
			this.textDb.Text = "development212";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(17, 49);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(87, 17);
			this.label3.TabIndex = 7;
			this.label3.Text = "Database";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(160, 137);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(163, 17);
			this.label2.TabIndex = 9;
			this.label2.Text = "Snippet Generator";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(9, 260);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(125, 17);
			this.label4.TabIndex = 10;
			this.label4.Text = "Class";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(9, 215);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(125, 17);
			this.label5.TabIndex = 12;
			this.label5.Text = "Type";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// comboType
			// 
			this.comboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboType.FormattingEnabled = true;
			this.comboType.Location = new System.Drawing.Point(11, 235);
			this.comboType.MaxDropDownItems = 100;
			this.comboType.Name = "comboType";
			this.comboType.Size = new System.Drawing.Size(144, 21);
			this.comboType.TabIndex = 11;
			// 
			// butSnippet
			// 
			this.butSnippet.Location = new System.Drawing.Point(59, 189);
			this.butSnippet.Name = "butSnippet";
			this.butSnippet.Size = new System.Drawing.Size(96, 23);
			this.butSnippet.TabIndex = 13;
			this.butSnippet.Text = "Create Snippet";
			this.butSnippet.UseVisualStyleBackColor = true;
			this.butSnippet.Click += new System.EventHandler(this.butSnippet_Click);
			// 
			// textSnippet
			// 
			this.textSnippet.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textSnippet.Location = new System.Drawing.Point(161, 157);
			this.textSnippet.Multiline = true;
			this.textSnippet.Name = "textSnippet";
			this.textSnippet.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textSnippet.Size = new System.Drawing.Size(988, 529);
			this.textSnippet.TabIndex = 14;
			this.textSnippet.WordWrap = false;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(1, 139);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(157, 43);
			this.label6.TabIndex = 15;
			this.label6.Text = "A copy of the snippet will automatically be placed on the clipboard";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listClass
			// 
			this.listClass.FormattingEnabled = true;
			this.listClass.Location = new System.Drawing.Point(11, 280);
			this.listClass.Name = "listClass";
			this.listClass.Size = new System.Drawing.Size(144, 407);
			this.listClass.TabIndex = 16;
			// 
			// checkRun
			// 
			this.checkRun.Checked = true;
			this.checkRun.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkRun.Location = new System.Drawing.Point(239, 50);
			this.checkRun.Name = "checkRun";
			this.checkRun.Size = new System.Drawing.Size(39, 17);
			this.checkRun.TabIndex = 19;
			this.checkRun.UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(223, 29);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(48, 17);
			this.label8.TabIndex = 21;
			this.label8.Text = "Run";
			this.label8.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(59, 83);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(176, 17);
			this.label9.TabIndex = 22;
			this.label9.Text = "Schema (no db needed)";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkRunSchema
			// 
			this.checkRunSchema.Location = new System.Drawing.Point(239, 84);
			this.checkRunSchema.Name = "checkRunSchema";
			this.checkRunSchema.Size = new System.Drawing.Size(39, 17);
			this.checkRunSchema.TabIndex = 23;
			this.checkRunSchema.UseVisualStyleBackColor = true;
			// 
			// label10
			// 
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.Location = new System.Drawing.Point(406, 73);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(421, 33);
			this.label10.TabIndex = 24;
			this.label10.Text = "This CrudGenerator project MUST be set to compile each time it\'s run even if chan" +
    "ges were only to OpenDentBusiness.";
			// 
			// checkRunQueries
			// 
			this.checkRunQueries.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRunQueries.Location = new System.Drawing.Point(32, 118);
			this.checkRunQueries.Name = "checkRunQueries";
			this.checkRunQueries.Size = new System.Drawing.Size(221, 17);
			this.checkRunQueries.TabIndex = 26;
			this.checkRunQueries.Text = "Run queries generated for convert script";
			this.checkRunQueries.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRunQueries.UseVisualStyleBackColor = true;
			// 
			// checkAddToConvertScript
			// 
			this.checkAddToConvertScript.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAddToConvertScript.Checked = true;
			this.checkAddToConvertScript.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAddToConvertScript.Location = new System.Drawing.Point(32, 101);
			this.checkAddToConvertScript.Name = "checkAddToConvertScript";
			this.checkAddToConvertScript.Size = new System.Drawing.Size(221, 17);
			this.checkAddToConvertScript.TabIndex = 27;
			this.checkAddToConvertScript.Text = "Add queries to convert script";
			this.checkAddToConvertScript.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAddToConvertScript.UseVisualStyleBackColor = true;
			// 
			// butAddPrefs
			// 
			this.butAddPrefs.Location = new System.Drawing.Point(409, 109);
			this.butAddPrefs.Name = "butAddPrefs";
			this.butAddPrefs.Size = new System.Drawing.Size(96, 23);
			this.butAddPrefs.TabIndex = 28;
			this.butAddPrefs.Text = "Add Prefs";
			this.butAddPrefs.UseVisualStyleBackColor = true;
			this.butAddPrefs.Click += new System.EventHandler(this.butAddPrefs_Click);
			// 
			// labelPortNum
			// 
			this.labelPortNum.Location = new System.Drawing.Point(17, 24);
			this.labelPortNum.Name = "labelPortNum";
			this.labelPortNum.Size = new System.Drawing.Size(87, 17);
			this.labelPortNum.TabIndex = 30;
			this.labelPortNum.Text = "Port Number";
			this.labelPortNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPortNum
			// 
			this.textPortNum.Location = new System.Drawing.Point(105, 23);
			this.textPortNum.Name = "textPortNum";
			this.textPortNum.Size = new System.Drawing.Size(64, 20);
			this.textPortNum.TabIndex = 29;
			this.textPortNum.Text = "3306";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1161, 910);
			this.Controls.Add(this.labelPortNum);
			this.Controls.Add(this.textPortNum);
			this.Controls.Add(this.butAddPrefs);
			this.Controls.Add(this.checkAddToConvertScript);
			this.Controls.Add(this.checkRunQueries);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.checkRunSchema);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.checkRun);
			this.Controls.Add(this.listClass);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textSnippet);
			this.Controls.Add(this.butSnippet);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.comboType);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDb);
			this.Controls.Add(this.butRun);
			this.Controls.Add(this.label1);
			this.Name = "Form1";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Crud Generator";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button butRun;
		private System.Windows.Forms.TextBox textDb;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox comboType;
		private System.Windows.Forms.Button butSnippet;
		private System.Windows.Forms.TextBox textSnippet;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ListBox listClass;
		private System.Windows.Forms.CheckBox checkRun;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.CheckBox checkRunSchema;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox checkRunQueries;
		private System.Windows.Forms.CheckBox checkAddToConvertScript;
		private System.Windows.Forms.Button butAddPrefs;
		private System.Windows.Forms.Label labelPortNum;
		private System.Windows.Forms.TextBox textPortNum;
	}
}

