namespace OpenDentalGraph {
	partial class HqMessagesRealTimeOptionsCtrl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.radioCustomer = new System.Windows.Forms.RadioButton();
			this.radioMsgType = new System.Windows.Forms.RadioButton();
			this.radioCountryCode = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// radioCustomer
			// 
			this.radioCustomer.Location = new System.Drawing.Point(11, 21);
			this.radioCustomer.Name = "radioCustomer";
			this.radioCustomer.Size = new System.Drawing.Size(106, 17);
			this.radioCustomer.TabIndex = 0;
			this.radioCustomer.TabStop = true;
			this.radioCustomer.Text = "Customer";
			this.radioCustomer.UseVisualStyleBackColor = true;
			this.radioCustomer.CheckedChanged += new System.EventHandler(this.OnBrokenApptGraphOptionsChanged);
			// 
			// radioMsgType
			// 
			this.radioMsgType.Location = new System.Drawing.Point(11, 43);
			this.radioMsgType.Name = "radioMsgType";
			this.radioMsgType.Size = new System.Drawing.Size(106, 17);
			this.radioMsgType.TabIndex = 1;
			this.radioMsgType.TabStop = true;
			this.radioMsgType.Text = "MsgType";
			this.radioMsgType.UseVisualStyleBackColor = true;
			this.radioMsgType.CheckedChanged += new System.EventHandler(this.OnBrokenApptGraphOptionsChanged);
			// 
			// radioCountryCode
			// 
			this.radioCountryCode.Location = new System.Drawing.Point(123, 43);
			this.radioCountryCode.Name = "radioCountryCode";
			this.radioCountryCode.Size = new System.Drawing.Size(106, 17);
			this.radioCountryCode.TabIndex = 3;
			this.radioCountryCode.TabStop = true;
			this.radioCountryCode.Text = "Country Code";
			this.radioCountryCode.UseVisualStyleBackColor = true;
			this.radioCountryCode.CheckedChanged += new System.EventHandler(this.OnBrokenApptGraphOptionsChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioCustomer);
			this.groupBox1.Controls.Add(this.radioMsgType);
			this.groupBox1.Controls.Add(this.radioCountryCode);
			this.groupBox1.Location = new System.Drawing.Point(9, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(244, 73);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Group By";
			// 
			// HqMessagesRealTimeOptionsCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Name = "HqMessagesRealTimeOptionsCtrl";
			this.Size = new System.Drawing.Size(269, 83);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RadioButton radioCountryCode;
		private System.Windows.Forms.RadioButton radioMsgType;
		private System.Windows.Forms.RadioButton radioCustomer;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}
