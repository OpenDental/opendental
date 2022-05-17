using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTerminal {
		private System.ComponentModel.IContainer components;

		///<summary></summary>
		protected override void Dispose(bool disposing){
			if(disposing){
				components?.Dispose();
				_formSheetFillEdit?.ForceClose();//includes dispose
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.labelWelcome = new System.Windows.Forms.Label();
			this.labelConnection = new System.Windows.Forms.Label();
			this.listForms = new OpenDental.UI.ListBoxOD();
			this.labelForms = new System.Windows.Forms.Label();
			this.panelClose = new System.Windows.Forms.Panel();
			this.labelThankYou = new System.Windows.Forms.Label();
			this.butDone = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// timer1
			// 
			this.timer1.Interval = 4000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// labelWelcome
			// 
			this.labelWelcome.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.labelWelcome.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelWelcome.Location = new System.Drawing.Point(235, 149);
			this.labelWelcome.Name = "labelWelcome";
			this.labelWelcome.Size = new System.Drawing.Size(366, 169);
			this.labelWelcome.TabIndex = 3;
			this.labelWelcome.Text = "Welcome!\r\n\r\nThis kiosk is used for filling out forms.\r\n\r\nThe receptionist will pr" +
    "epare the screen for you.";
			this.labelWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelConnection
			// 
			this.labelConnection.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.labelConnection.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelConnection.Location = new System.Drawing.Point(196, 650);
			this.labelConnection.Name = "labelConnection";
			this.labelConnection.Size = new System.Drawing.Size(444, 29);
			this.labelConnection.TabIndex = 4;
			this.labelConnection.Text = "Connection to server has been lost";
			this.labelConnection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// listForms
			// 
			this.listForms.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.listForms.Location = new System.Drawing.Point(343, 361);
			this.listForms.Name = "listForms";
			this.listForms.Size = new System.Drawing.Size(149, 173);
			this.listForms.TabIndex = 5;
			this.listForms.DoubleClick += new System.EventHandler(this.listForms_DoubleClick);
			// 
			// labelForms
			// 
			this.labelForms.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.labelForms.Location = new System.Drawing.Point(329, 341);
			this.labelForms.Name = "labelForms";
			this.labelForms.Size = new System.Drawing.Size(175, 17);
			this.labelForms.TabIndex = 6;
			this.labelForms.Text = "Forms - Double click to edit";
			this.labelForms.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// panelClose
			// 
			this.panelClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.panelClose.Location = new System.Drawing.Point(799, 628);
			this.panelClose.Name = "panelClose";
			this.panelClose.Size = new System.Drawing.Size(66, 59);
			this.panelClose.TabIndex = 7;
			this.panelClose.Click += new System.EventHandler(this.panelClose_Click);
			// 
			// labelThankYou
			// 
			this.labelThankYou.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.labelThankYou.Location = new System.Drawing.Point(367, 534);
			this.labelThankYou.Name = "labelThankYou";
			this.labelThankYou.Size = new System.Drawing.Size(100, 17);
			this.labelThankYou.TabIndex = 8;
			this.labelThankYou.Text = "Thank You";
			this.labelThankYou.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelThankYou.Visible = false;
			// 
			// butDone
			// 
			this.butDone.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.butDone.Location = new System.Drawing.Point(380, 551);
			this.butDone.Name = "butDone";
			this.butDone.Size = new System.Drawing.Size(75, 24);
			this.butDone.TabIndex = 1;
			this.butDone.Text = "Done";
			this.butDone.Click += new System.EventHandler(this.butDone_Click);
			// 
			// FormTerminal
			// 
			this.ClientSize = new System.Drawing.Size(864, 686);
			this.Controls.Add(this.butDone);
			this.Controls.Add(this.listForms);
			this.Controls.Add(this.labelThankYou);
			this.Controls.Add(this.panelClose);
			this.Controls.Add(this.labelForms);
			this.Controls.Add(this.labelConnection);
			this.Controls.Add(this.labelWelcome);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.HasHelpButton = false;
			this.IsBorderLocked = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTerminal";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTerminal_FormClosing);
			this.Load += new System.EventHandler(this.FormTerminal_Load);
			this.Shown += new System.EventHandler(this.FormTerminal_Shown);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butDone;
		private Timer timer1;
		private Label labelConnection;
		private Label labelForms;
		private Label labelThankYou;
		private Label labelWelcome;
		private Panel panelClose;
		private OpenDental.UI.ListBoxOD listForms;
	}
}
