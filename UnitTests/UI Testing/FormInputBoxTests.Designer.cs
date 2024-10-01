namespace UnitTests
{
	partial class FormInputBoxTests
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.butSimple = new OpenDental.UI.Button();
			this.butMultilineString = new OpenDental.UI.Button();
			this.butComboBox = new OpenDental.UI.Button();
			this.butComplex = new OpenDental.UI.Button();
			this.butValids = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butSimple
			// 
			this.butSimple.Location = new System.Drawing.Point(30, 65);
			this.butSimple.Name = "butSimple";
			this.butSimple.Size = new System.Drawing.Size(94, 24);
			this.butSimple.TabIndex = 76;
			this.butSimple.Text = "Simple String";
			this.butSimple.UseVisualStyleBackColor = true;
			this.butSimple.Click += new System.EventHandler(this.butSimpleString_Click);
			// 
			// butMultilineString
			// 
			this.butMultilineString.Location = new System.Drawing.Point(30, 106);
			this.butMultilineString.Name = "butMultilineString";
			this.butMultilineString.Size = new System.Drawing.Size(94, 24);
			this.butMultilineString.TabIndex = 77;
			this.butMultilineString.Text = "Multiline String";
			this.butMultilineString.UseVisualStyleBackColor = true;
			this.butMultilineString.Click += new System.EventHandler(this.butMultilineString_Click);
			// 
			// butComboBox
			// 
			this.butComboBox.Location = new System.Drawing.Point(30, 148);
			this.butComboBox.Name = "butComboBox";
			this.butComboBox.Size = new System.Drawing.Size(94, 24);
			this.butComboBox.TabIndex = 78;
			this.butComboBox.Text = "ComboBox";
			this.butComboBox.UseVisualStyleBackColor = true;
			this.butComboBox.Click += new System.EventHandler(this.butComboBox_Click);
			// 
			// butComplex
			// 
			this.butComplex.Location = new System.Drawing.Point(30, 189);
			this.butComplex.Name = "butComplex";
			this.butComplex.Size = new System.Drawing.Size(94, 24);
			this.butComplex.TabIndex = 79;
			this.butComplex.Text = "Complex";
			this.butComplex.UseVisualStyleBackColor = true;
			this.butComplex.Click += new System.EventHandler(this.butComplex_Click);
			// 
			// butValids
			// 
			this.butValids.Location = new System.Drawing.Point(30, 228);
			this.butValids.Name = "butValids";
			this.butValids.Size = new System.Drawing.Size(94, 24);
			this.butValids.TabIndex = 80;
			this.butValids.Text = "Valids";
			this.butValids.UseVisualStyleBackColor = true;
			this.butValids.Click += new System.EventHandler(this.butValids_Click);
			// 
			// FormInputBoxTests
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(603, 441);
			this.Controls.Add(this.butValids);
			this.Controls.Add(this.butComplex);
			this.Controls.Add(this.butComboBox);
			this.Controls.Add(this.butMultilineString);
			this.Controls.Add(this.butSimple);
			this.Name = "FormInputBoxTests";
			this.Text = "FormProgressTestscs";
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSimple;
		private OpenDental.UI.Button butMultilineString;
		private OpenDental.UI.Button butComboBox;
		private OpenDental.UI.Button butComplex;
		private OpenDental.UI.Button butValids;
	}
}