using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormUAppoint {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUAppoint));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textProgName = new System.Windows.Forms.TextBox();
			this.textProgDesc = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textPath = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textUsername = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textWorkstationName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textIntervalSeconds = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textDateTimeLastUploaded1 = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textSynchStatus = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.textDateTimeLastUploaded2 = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.butStart = new OpenDental.UI.Button();
			this.butViewLog = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(577,419);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,24);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(577,385);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,24);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// checkEnabled
			// 
			this.checkEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEnabled.Location = new System.Drawing.Point(161,60);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(98,18);
			this.checkEnabled.TabIndex = 41;
			this.checkEnabled.Text = "Enabled";
			this.checkEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(58,10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(187,18);
			this.label1.TabIndex = 44;
			this.label1.Text = "Internal Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProgName
			// 
			this.textProgName.Location = new System.Drawing.Point(246,9);
			this.textProgName.Name = "textProgName";
			this.textProgName.ReadOnly = true;
			this.textProgName.Size = new System.Drawing.Size(275,20);
			this.textProgName.TabIndex = 45;
			// 
			// textProgDesc
			// 
			this.textProgDesc.Location = new System.Drawing.Point(246,34);
			this.textProgDesc.Name = "textProgDesc";
			this.textProgDesc.Size = new System.Drawing.Size(275,20);
			this.textProgDesc.TabIndex = 47;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(57,35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(187,18);
			this.label2.TabIndex = 46;
			this.label2.Text = "Description";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPath
			// 
			this.textPath.Location = new System.Drawing.Point(246,81);
			this.textPath.Name = "textPath";
			this.textPath.Size = new System.Drawing.Size(275,20);
			this.textPath.TabIndex = 49;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(13,83);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(231,18);
			this.label3.TabIndex = 48;
			this.label3.Text = "URL of UAppoint server";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUsername
			// 
			this.textUsername.Location = new System.Drawing.Point(246,107);
			this.textUsername.Name = "textUsername";
			this.textUsername.Size = new System.Drawing.Size(169,20);
			this.textUsername.TabIndex = 51;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(13,109);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(231,18);
			this.label4.TabIndex = 50;
			this.label4.Text = "Username";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(246,133);
			this.textPassword.Name = "textPassword";
			this.textPassword.Size = new System.Drawing.Size(251,20);
			this.textPassword.TabIndex = 53;
			this.textPassword.TextChanged += new System.EventHandler(this.textPassword_TextChanged);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(13,135);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(231,18);
			this.label5.TabIndex = 52;
			this.label5.Text = "Password";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textWorkstationName
			// 
			this.textWorkstationName.Location = new System.Drawing.Point(246,159);
			this.textWorkstationName.Name = "textWorkstationName";
			this.textWorkstationName.Size = new System.Drawing.Size(169,20);
			this.textWorkstationName.TabIndex = 55;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(13,161);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(231,18);
			this.label6.TabIndex = 54;
			this.label6.Text = "Name of workstation used to synch";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textIntervalSeconds
			// 
			this.textIntervalSeconds.Location = new System.Drawing.Point(246,185);
			this.textIntervalSeconds.Name = "textIntervalSeconds";
			this.textIntervalSeconds.Size = new System.Drawing.Size(37,20);
			this.textIntervalSeconds.TabIndex = 57;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(13,187);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(231,18);
			this.label7.TabIndex = 56;
			this.label7.Text = "Synch interval in seconds";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateTimeLastUploaded1
			// 
			this.textDateTimeLastUploaded1.Location = new System.Drawing.Point(246,211);
			this.textDateTimeLastUploaded1.Name = "textDateTimeLastUploaded1";
			this.textDateTimeLastUploaded1.ReadOnly = true;
			this.textDateTimeLastUploaded1.Size = new System.Drawing.Size(169,20);
			this.textDateTimeLastUploaded1.TabIndex = 59;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(13,213);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(231,18);
			this.label8.TabIndex = 58;
			this.label8.Text = "DateTime last uploaded";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSynchStatus
			// 
			this.textSynchStatus.Location = new System.Drawing.Point(246,270);
			this.textSynchStatus.Multiline = true;
			this.textSynchStatus.Name = "textSynchStatus";
			this.textSynchStatus.ReadOnly = true;
			this.textSynchStatus.Size = new System.Drawing.Size(275,44);
			this.textSynchStatus.TabIndex = 61;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(58,270);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(187,18);
			this.label9.TabIndex = 60;
			this.label9.Text = "Synch Status";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(83,323);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(162,17);
			this.label10.TabIndex = 64;
			this.label10.Text = "Notes";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(246,320);
			this.textNote.MaxLength = 255;
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(275,80);
			this.textNote.TabIndex = 63;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(417,236);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(232,31);
			this.label11.TabIndex = 65;
			this.label11.Text = "DateTime may be manually backdated to trigger resending of old synch data.";
			// 
			// textDateTimeLastUploaded2
			// 
			this.textDateTimeLastUploaded2.Location = new System.Drawing.Point(246,237);
			this.textDateTimeLastUploaded2.Name = "textDateTimeLastUploaded2";
			this.textDateTimeLastUploaded2.Size = new System.Drawing.Size(169,20);
			this.textDateTimeLastUploaded2.TabIndex = 67;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(13,238);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(231,18);
			this.label12.TabIndex = 66;
			this.label12.Text = "Set DateTime last uploaded";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butStart
			// 
			this.butStart.Location = new System.Drawing.Point(180,292);
			this.butStart.Name = "butStart";
			this.butStart.Size = new System.Drawing.Size(62,22);
			this.butStart.TabIndex = 62;
			this.butStart.Text = "Restart";
			this.butStart.Click += new System.EventHandler(this.butStart_Click);
			// 
			// butViewLog
			// 
			this.butViewLog.Location = new System.Drawing.Point(527,292);
			this.butViewLog.Name = "butViewLog";
			this.butViewLog.Size = new System.Drawing.Size(67,22);
			this.butViewLog.TabIndex = 68;
			this.butViewLog.Text = "View Log";
			this.butViewLog.Click += new System.EventHandler(this.butViewLog_Click);
			// 
			// FormUAppoint
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(664,455);
			this.Controls.Add(this.butViewLog);
			this.Controls.Add(this.textDateTimeLastUploaded2);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.butStart);
			this.Controls.Add(this.textSynchStatus);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.textDateTimeLastUploaded1);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textIntervalSeconds);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textWorkstationName);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textPassword);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textUsername);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textPath);
			this.Controls.Add(this.textProgDesc);
			this.Controls.Add(this.textProgName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormUAppoint";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "UAppoint Setup";
			this.Load += new System.EventHandler(this.FormUAppoint_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.CheckBox checkEnabled;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textProgName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textProgDesc;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textPath;
		private TextBox textUsername;
		private Label label4;
		private TextBox textPassword;
		private Label label5;
		private TextBox textWorkstationName;
		private Label label6;
		private TextBox textIntervalSeconds;
		private Label label7;
		private TextBox textDateTimeLastUploaded1;
		private Label label8;
		private TextBox textSynchStatus;
		private Label label9;
		private Label label10;
		private TextBox textNote;
		private Label label11;
		private TextBox textDateTimeLastUploaded2;
		private Label label12;
		private OpenDental.UI.Button butStart;
		private OpenDental.UI.Button butViewLog;
	}
}
