namespace OpenDental.Reporting.Allocators.MyAllocator1
{
	partial class FormWarnToCloseComputers
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWarnToCloseComputers));
			this.lblMessage = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.butYes = new System.Windows.Forms.Button();
			this.butNoCancel = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.imageAnimator1 = new OpenDental.Reporting.Allocators.MyAllocator1.Images.ImageAnimator();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblMessage
			// 
			this.lblMessage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblMessage.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblMessage.ForeColor = System.Drawing.Color.Maroon;
			this.lblMessage.Location = new System.Drawing.Point(54, 34);
			this.lblMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(675, 130);
			this.lblMessage.TabIndex = 0;
			this.lblMessage.Text = resources.GetString("lblMessage.Text");
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.Maroon;
			this.label1.Location = new System.Drawing.Point(116, 244);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(368, 34);
			this.label1.TabIndex = 1;
			this.label1.Text = "Is this the only computer running?";
			// 
			// butYes
			// 
			this.butYes.Location = new System.Drawing.Point(492, 244);
			this.butYes.Margin = new System.Windows.Forms.Padding(4);
			this.butYes.Name = "butYes";
			this.butYes.Size = new System.Drawing.Size(64, 34);
			this.butYes.TabIndex = 2;
			this.butYes.Text = "Yes";
			this.butYes.UseVisualStyleBackColor = true;
			this.butYes.Click += new System.EventHandler(this.butYes_Click);
			// 
			// butNoCancel
			// 
			this.butNoCancel.Location = new System.Drawing.Point(564, 244);
			this.butNoCancel.Margin = new System.Windows.Forms.Padding(4);
			this.butNoCancel.Name = "butNoCancel";
			this.butNoCancel.Size = new System.Drawing.Size(117, 34);
			this.butNoCancel.TabIndex = 3;
			this.butNoCancel.Text = "No/Cancel";
			this.butNoCancel.UseVisualStyleBackColor = true;
			this.butNoCancel.Click += new System.EventHandler(this.butNoCancel_Click);
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.imageAnimator1);
			this.panel1.Location = new System.Drawing.Point(58, 167);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(671, 61);
			this.panel1.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.Maroon;
			this.label2.Location = new System.Drawing.Point(98, 12);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(368, 34);
			this.label2.TabIndex = 5;
			this.label2.Text = "Close Other OD Programs Now!!";
			// 
			// imageAnimator1
			// 
			this.imageAnimator1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.imageAnimator1.Location = new System.Drawing.Point(5, 4);
			this.imageAnimator1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.imageAnimator1.Name = "imageAnimator1";
			this.imageAnimator1.SET_IMAGES = null;
			this.imageAnimator1.Size = new System.Drawing.Size(50, 50);
			this.imageAnimator1.TabIndex = 5;
			// 
			// FormWarnToCloseComputers
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(783, 297);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.butNoCancel);
			this.Controls.Add(this.butYes);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblMessage);
			this.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ForeColor = System.Drawing.Color.Maroon;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "FormWarnToCloseComputers";
			this.Text = "Warning";
			this.Load += new System.EventHandler(this.FormWarnToCloseComputers_Load);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblMessage;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button butYes;
		private System.Windows.Forms.Button butNoCancel;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label2;
		private OpenDental.Reporting.Allocators.MyAllocator1.Images.ImageAnimator imageAnimator1;
	}
}