
namespace UnitTests {
	partial class FormToothChartTests {
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
			this.butToothChartBig = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioOpenGL = new System.Windows.Forms.RadioButton();
			this.radioSimple2D = new System.Windows.Forms.RadioButton();
			this.radioDirectX = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butToothChartBig
			// 
			this.butToothChartBig.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butToothChartBig.Location = new System.Drawing.Point(183, 253);
			this.butToothChartBig.Name = "butToothChartBig";
			this.butToothChartBig.Size = new System.Drawing.Size(135, 41);
			this.butToothChartBig.TabIndex = 77;
			this.butToothChartBig.Text = "Tooth Chart Big";
			this.butToothChartBig.UseVisualStyleBackColor = true;
			this.butToothChartBig.Click += new System.EventHandler(this.butToothChartBig_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.groupBox1.Controls.Add(this.radioOpenGL);
			this.groupBox1.Controls.Add(this.radioSimple2D);
			this.groupBox1.Controls.Add(this.radioDirectX);
			this.groupBox1.Location = new System.Drawing.Point(183, 86);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(135, 113);
			this.groupBox1.TabIndex = 78;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Drawing Mode";
			// 
			// radioOpenGL
			// 
			this.radioOpenGL.AutoSize = true;
			this.radioOpenGL.Location = new System.Drawing.Point(32, 76);
			this.radioOpenGL.Name = "radioOpenGL";
			this.radioOpenGL.Size = new System.Drawing.Size(65, 17);
			this.radioOpenGL.TabIndex = 2;
			this.radioOpenGL.Text = "OpenGL";
			this.radioOpenGL.UseVisualStyleBackColor = true;
			// 
			// radioSimple2D
			// 
			this.radioSimple2D.AutoSize = true;
			this.radioSimple2D.Checked = true;
			this.radioSimple2D.Location = new System.Drawing.Point(32, 52);
			this.radioSimple2D.Name = "radioSimple2D";
			this.radioSimple2D.Size = new System.Drawing.Size(70, 17);
			this.radioSimple2D.TabIndex = 1;
			this.radioSimple2D.TabStop = true;
			this.radioSimple2D.Text = "Simple2D";
			this.radioSimple2D.UseVisualStyleBackColor = true;
			// 
			// radioDirectX
			// 
			this.radioDirectX.AutoSize = true;
			this.radioDirectX.Location = new System.Drawing.Point(32, 28);
			this.radioDirectX.Name = "radioDirectX";
			this.radioDirectX.Size = new System.Drawing.Size(60, 17);
			this.radioDirectX.TabIndex = 0;
			this.radioDirectX.Text = "DirectX";
			this.radioDirectX.UseVisualStyleBackColor = true;
			// 
			// FormToothChartTests
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(500, 381);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butToothChartBig);
			this.Name = "FormToothChartTests";
			this.Text = "ToothChart Tests";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butToothChartBig;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioOpenGL;
		private System.Windows.Forms.RadioButton radioSimple2D;
		private System.Windows.Forms.RadioButton radioDirectX;
	}
}