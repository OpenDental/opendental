using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormUpdateSetup {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUpdateSetup));
			this.textWebsitePath = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textRegKey = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textUpdateServerAddress = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textMultiple = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textWebProxyPassword = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textWebProxyUserName = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textWebProxyAddress = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.checkShowMsi = new System.Windows.Forms.CheckBox();
			this.label10 = new System.Windows.Forms.Label();
			this.labelRecopy = new System.Windows.Forms.Label();
			this.butRecopy = new OpenDental.UI.Button();
			this.butChangeRegKey = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butChangeTime = new OpenDental.UI.Button();
			this.textUpdateTime = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// textWebsitePath
			// 
			this.textWebsitePath.Location = new System.Drawing.Point(249, 44);
			this.textWebsitePath.Name = "textWebsitePath";
			this.textWebsitePath.Size = new System.Drawing.Size(434, 20);
			this.textWebsitePath.TabIndex = 36;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(69, 45);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(180, 19);
			this.label3.TabIndex = 37;
			this.label3.Text = "Website Path for Updates";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRegKey
			// 
			this.textRegKey.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textRegKey.Location = new System.Drawing.Point(249, 188);
			this.textRegKey.Name = "textRegKey";
			this.textRegKey.ReadOnly = true;
			this.textRegKey.Size = new System.Drawing.Size(193, 20);
			this.textRegKey.TabIndex = 40;
			this.textRegKey.TextChanged += new System.EventHandler(this.textRegKey_TextChanged);
			this.textRegKey.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textRegKey_KeyUp);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(69, 189);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(180, 19);
			this.label2.TabIndex = 41;
			this.label2.Text = "Registration Key";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(521, 185);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(204, 33);
			this.label4.TabIndex = 42;
			this.label4.Text = "Valid for one office ONLY.  This is tracked.";
			// 
			// textUpdateServerAddress
			// 
			this.textUpdateServerAddress.Location = new System.Drawing.Point(249, 18);
			this.textUpdateServerAddress.Name = "textUpdateServerAddress";
			this.textUpdateServerAddress.Size = new System.Drawing.Size(434, 20);
			this.textUpdateServerAddress.TabIndex = 43;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(69, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(180, 19);
			this.label1.TabIndex = 44;
			this.label1.Text = "Server Address for Updates";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMultiple
			// 
			this.textMultiple.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textMultiple.Location = new System.Drawing.Point(249, 221);
			this.textMultiple.Multiline = true;
			this.textMultiple.Name = "textMultiple";
			this.textMultiple.Size = new System.Drawing.Size(266, 41);
			this.textMultiple.TabIndex = 45;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(69, 222);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(180, 40);
			this.label5.TabIndex = 46;
			this.label5.Text = "Simultaneously update other databases";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(521, 221);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(210, 41);
			this.label6.TabIndex = 47;
			this.label6.Text = "Separate with commas.  Do not include this database.";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textWebProxyPassword);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.textWebProxyUserName);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.textWebProxyAddress);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Location = new System.Drawing.Point(69, 75);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(614, 100);
			this.groupBox1.TabIndex = 48;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Web Proxy (most users will ignore this section)";
			// 
			// textWebProxyPassword
			// 
			this.textWebProxyPassword.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textWebProxyPassword.Location = new System.Drawing.Point(180, 71);
			this.textWebProxyPassword.Name = "textWebProxyPassword";
			this.textWebProxyPassword.PasswordChar = '*';
			this.textWebProxyPassword.Size = new System.Drawing.Size(127, 20);
			this.textWebProxyPassword.TabIndex = 46;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(25, 72);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(155, 19);
			this.label9.TabIndex = 47;
			this.label9.Text = "Password";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textWebProxyUserName
			// 
			this.textWebProxyUserName.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textWebProxyUserName.Location = new System.Drawing.Point(180, 45);
			this.textWebProxyUserName.Name = "textWebProxyUserName";
			this.textWebProxyUserName.Size = new System.Drawing.Size(127, 20);
			this.textWebProxyUserName.TabIndex = 44;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(25, 46);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(155, 19);
			this.label8.TabIndex = 45;
			this.label8.Text = "UserName";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textWebProxyAddress
			// 
			this.textWebProxyAddress.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textWebProxyAddress.Location = new System.Drawing.Point(180, 19);
			this.textWebProxyAddress.Name = "textWebProxyAddress";
			this.textWebProxyAddress.Size = new System.Drawing.Size(363, 20);
			this.textWebProxyAddress.TabIndex = 42;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(25, 20);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(155, 19);
			this.label7.TabIndex = 43;
			this.label7.Text = "Address";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkShowMsi
			// 
			this.checkShowMsi.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowMsi.Location = new System.Drawing.Point(69, 270);
			this.checkShowMsi.Name = "checkShowMsi";
			this.checkShowMsi.Size = new System.Drawing.Size(194, 24);
			this.checkShowMsi.TabIndex = 51;
			this.checkShowMsi.Text = "Show buttons for msi";
			this.checkShowMsi.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowMsi.UseVisualStyleBackColor = true;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(269, 272);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(246, 19);
			this.label10.TabIndex = 52;
			this.label10.Text = "(most users will not check this option)";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelRecopy
			// 
			this.labelRecopy.Location = new System.Drawing.Point(12, 337);
			this.labelRecopy.Name = "labelRecopy";
			this.labelRecopy.Size = new System.Drawing.Size(237, 68);
			this.labelRecopy.TabIndex = 53;
			this.labelRecopy.Text = "Copies all files from the local installation directory to the shared Update Files" +
    " folder and puts a zipped copy into the database.\r\n";
			this.labelRecopy.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butRecopy
			// 
			this.butRecopy.Location = new System.Drawing.Point(249, 345);
			this.butRecopy.Name = "butRecopy";
			this.butRecopy.Size = new System.Drawing.Size(67, 23);
			this.butRecopy.TabIndex = 54;
			this.butRecopy.Text = "Recopy";
			this.butRecopy.Click += new System.EventHandler(this.butRecopy_Click);
			// 
			// butChangeRegKey
			// 
			this.butChangeRegKey.Location = new System.Drawing.Point(448, 186);
			this.butChangeRegKey.Name = "butChangeRegKey";
			this.butChangeRegKey.Size = new System.Drawing.Size(67, 23);
			this.butChangeRegKey.TabIndex = 50;
			this.butChangeRegKey.Text = "Change";
			this.butChangeRegKey.Click += new System.EventHandler(this.butChangeRegKey_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(640, 320);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(640, 353);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butChangeTime
			// 
			this.butChangeTime.Location = new System.Drawing.Point(448, 298);
			this.butChangeTime.Name = "butChangeTime";
			this.butChangeTime.Size = new System.Drawing.Size(67, 23);
			this.butChangeTime.TabIndex = 58;
			this.butChangeTime.Text = "Change";
			this.butChangeTime.Click += new System.EventHandler(this.butChangeTime_Click);
			// 
			// textUpdateTime
			// 
			this.textUpdateTime.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textUpdateTime.Location = new System.Drawing.Point(249, 300);
			this.textUpdateTime.Name = "textUpdateTime";
			this.textUpdateTime.ReadOnly = true;
			this.textUpdateTime.Size = new System.Drawing.Size(193, 20);
			this.textUpdateTime.TabIndex = 55;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(72, 301);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(177, 19);
			this.label12.TabIndex = 56;
			this.label12.Text = "Update Notification Time";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormUpdateSetup
			// 
			this.ClientSize = new System.Drawing.Size(734, 392);
			this.Controls.Add(this.butChangeTime);
			this.Controls.Add(this.textUpdateTime);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.butRecopy);
			this.Controls.Add(this.labelRecopy);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.checkShowMsi);
			this.Controls.Add(this.butChangeRegKey);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textMultiple);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textUpdateServerAddress);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textRegKey);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textWebsitePath);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormUpdateSetup";
			this.ShowInTaskbar = false;
			this.Text = "Update Setup";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormUpdateSetup_FormClosing);
			this.Load += new System.EventHandler(this.FormUpdateSetup_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private TextBox textWebsitePath;
		private Label label3;
		private TextBox textRegKey;
		private Label label2;
		private Label label4;
		private TextBox textUpdateServerAddress;
		private Label label1;
		private TextBox textMultiple;
		private Label label5;
		private Label label6;
		private GroupBox groupBox1;
		private TextBox textWebProxyPassword;
		private Label label9;
		private TextBox textWebProxyUserName;
		private Label label8;
		private TextBox textWebProxyAddress;
		private Label label7;
		private OpenDental.UI.Button butChangeRegKey;
		private CheckBox checkShowMsi;
		private Label label10;
		private Label labelRecopy;
		private OpenDental.UI.Button butRecopy;
		private UI.Button butChangeTime;
		private TextBox textUpdateTime;
		private Label label12;
	}
}
