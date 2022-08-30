namespace UnitTests
{
	partial class FormProgressTests
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
			this.butOld = new OpenDental.UI.Button();
			this.butNew = new OpenDental.UI.Button();
			this.butHistory = new OpenDental.UI.Button();
			this.butChain = new OpenDental.UI.Button();
			this.butException = new OpenDental.UI.Button();
			this.butHideCancel = new OpenDental.UI.Button();
			this.butInnerException = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOld
			// 
			this.butOld.Location = new System.Drawing.Point(25, 56);
			this.butOld.Name = "butOld";
			this.butOld.Size = new System.Drawing.Size(75, 24);
			this.butOld.TabIndex = 76;
			this.butOld.Text = "Old";
			this.butOld.UseVisualStyleBackColor = true;
			this.butOld.Click += new System.EventHandler(this.butOld_Click);
			// 
			// butNew
			// 
			this.butNew.Location = new System.Drawing.Point(212, 56);
			this.butNew.Name = "butNew";
			this.butNew.Size = new System.Drawing.Size(75, 24);
			this.butNew.TabIndex = 77;
			this.butNew.Text = "New";
			this.butNew.UseVisualStyleBackColor = true;
			this.butNew.Click += new System.EventHandler(this.butNew_Click);
			// 
			// butHistory
			// 
			this.butHistory.Location = new System.Drawing.Point(293, 56);
			this.butHistory.Name = "butHistory";
			this.butHistory.Size = new System.Drawing.Size(75, 24);
			this.butHistory.TabIndex = 78;
			this.butHistory.Text = "History";
			this.butHistory.UseVisualStyleBackColor = true;
			this.butHistory.Click += new System.EventHandler(this.butHistory_Click);
			// 
			// butChain
			// 
			this.butChain.Location = new System.Drawing.Point(293, 86);
			this.butChain.Name = "butChain";
			this.butChain.Size = new System.Drawing.Size(75, 24);
			this.butChain.TabIndex = 79;
			this.butChain.Text = "Chain";
			this.butChain.UseVisualStyleBackColor = true;
			this.butChain.Click += new System.EventHandler(this.butChain_Click);
			// 
			// butException
			// 
			this.butException.Location = new System.Drawing.Point(374, 56);
			this.butException.Name = "butException";
			this.butException.Size = new System.Drawing.Size(75, 24);
			this.butException.TabIndex = 80;
			this.butException.Text = "Exception";
			this.butException.UseVisualStyleBackColor = true;
			this.butException.Click += new System.EventHandler(this.butException_Click);
			// 
			// butHideCancel
			// 
			this.butHideCancel.Location = new System.Drawing.Point(455, 56);
			this.butHideCancel.Name = "butHideCancel";
			this.butHideCancel.Size = new System.Drawing.Size(90, 24);
			this.butHideCancel.TabIndex = 81;
			this.butHideCancel.Text = "Hide Cancel";
			this.butHideCancel.UseVisualStyleBackColor = true;
			this.butHideCancel.Click += new System.EventHandler(this.butHideCancel_Click);
			// 
			// butInnerException
			// 
			this.butInnerException.Location = new System.Drawing.Point(374, 86);
			this.butInnerException.Name = "butInnerException";
			this.butInnerException.Size = new System.Drawing.Size(75, 24);
			this.butInnerException.TabIndex = 82;
			this.butInnerException.Text = "Inner Excp";
			this.butInnerException.UseVisualStyleBackColor = true;
			this.butInnerException.Click += new System.EventHandler(this.butInnerException_Click);
			// 
			// FormProgressTests
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(596, 192);
			this.Controls.Add(this.butInnerException);
			this.Controls.Add(this.butHideCancel);
			this.Controls.Add(this.butException);
			this.Controls.Add(this.butChain);
			this.Controls.Add(this.butHistory);
			this.Controls.Add(this.butNew);
			this.Controls.Add(this.butOld);
			this.Name = "FormProgressTests";
			this.Text = "FormProgressTestscs";
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOld;
		private OpenDental.UI.Button butNew;
		private OpenDental.UI.Button butHistory;
		private OpenDental.UI.Button butChain;
		private OpenDental.UI.Button butException;
		private OpenDental.UI.Button butHideCancel;
		private OpenDental.UI.Button butInnerException;
	}
}