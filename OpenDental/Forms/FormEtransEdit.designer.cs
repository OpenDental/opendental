using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormEtransEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEtransEdit));
			this.textMessageText = new System.Windows.Forms.RichTextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textClaimNum = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textBatchNumber = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textTransSetNum = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textAckCode = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.groupAck = new System.Windows.Forms.GroupBox();
			this.label10 = new System.Windows.Forms.Label();
			this.butPrintAck = new OpenDental.UI.Button();
			this.textAckNote = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textAckDateTime = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textAckMessage = new System.Windows.Forms.RichTextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textDateTimeTrans = new System.Windows.Forms.TextBox();
			this.checkAttachments = new System.Windows.Forms.CheckBox();
			this.butPrint = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupAck.SuspendLayout();
			this.SuspendLayout();
			// 
			// textMessageText
			// 
			this.textMessageText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.textMessageText.BackColor = System.Drawing.SystemColors.Window;
			this.textMessageText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.textMessageText.Location = new System.Drawing.Point(12, 52);
			this.textMessageText.Name = "textMessageText";
			this.textMessageText.ReadOnly = true;
			this.textMessageText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textMessageText.Size = new System.Drawing.Size(455, 379);
			this.textMessageText.TabIndex = 2;
			this.textMessageText.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(455, 17);
			this.label1.TabIndex = 3;
			this.label1.Text = "Message Text Sent";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textClaimNum
			// 
			this.textClaimNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textClaimNum.Location = new System.Drawing.Point(112, 463);
			this.textClaimNum.Name = "textClaimNum";
			this.textClaimNum.ReadOnly = true;
			this.textClaimNum.Size = new System.Drawing.Size(100, 20);
			this.textClaimNum.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.Location = new System.Drawing.Point(11, 464);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 17);
			this.label2.TabIndex = 5;
			this.label2.Text = "ClaimNum";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.Location = new System.Drawing.Point(11, 485);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 17);
			this.label3.TabIndex = 7;
			this.label3.Text = "BatchNumber";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBatchNumber
			// 
			this.textBatchNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBatchNumber.Location = new System.Drawing.Point(112, 484);
			this.textBatchNumber.Name = "textBatchNumber";
			this.textBatchNumber.ReadOnly = true;
			this.textBatchNumber.Size = new System.Drawing.Size(100, 20);
			this.textBatchNumber.TabIndex = 6;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.Location = new System.Drawing.Point(11, 506);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 17);
			this.label4.TabIndex = 9;
			this.label4.Text = "TransSetNum";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTransSetNum
			// 
			this.textTransSetNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textTransSetNum.Location = new System.Drawing.Point(112, 505);
			this.textTransSetNum.Name = "textTransSetNum";
			this.textTransSetNum.ReadOnly = true;
			this.textTransSetNum.Size = new System.Drawing.Size(100, 20);
			this.textTransSetNum.TabIndex = 8;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.Location = new System.Drawing.Point(11, 527);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 17);
			this.label5.TabIndex = 11;
			this.label5.Text = "AckCode";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAckCode
			// 
			this.textAckCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textAckCode.Location = new System.Drawing.Point(112, 526);
			this.textAckCode.Name = "textAckCode";
			this.textAckCode.ReadOnly = true;
			this.textAckCode.Size = new System.Drawing.Size(100, 20);
			this.textAckCode.TabIndex = 10;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label6.Location = new System.Drawing.Point(11, 566);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(100, 17);
			this.label6.TabIndex = 13;
			this.label6.Text = "Note";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textNote.Location = new System.Drawing.Point(112, 565);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(355, 59);
			this.textNote.TabIndex = 12;
			// 
			// groupAck
			// 
			this.groupAck.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.groupAck.Controls.Add(this.label10);
			this.groupAck.Controls.Add(this.butPrintAck);
			this.groupAck.Controls.Add(this.textAckNote);
			this.groupAck.Controls.Add(this.label9);
			this.groupAck.Controls.Add(this.textAckDateTime);
			this.groupAck.Controls.Add(this.label7);
			this.groupAck.Controls.Add(this.textAckMessage);
			this.groupAck.Location = new System.Drawing.Point(473, 15);
			this.groupAck.Name = "groupAck";
			this.groupAck.Size = new System.Drawing.Size(404, 615);
			this.groupAck.TabIndex = 14;
			this.groupAck.TabStop = false;
			this.groupAck.Text = "Acknowledgement";
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label10.Location = new System.Drawing.Point(4, 551);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(38, 17);
			this.label10.TabIndex = 21;
			this.label10.Text = "Note";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPrintAck
			// 
			this.butPrintAck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrintAck.Image = global::OpenDental.Properties.Resources.butPreview;
			this.butPrintAck.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrintAck.Location = new System.Drawing.Point(317, 425);
			this.butPrintAck.Name = "butPrintAck";
			this.butPrintAck.Size = new System.Drawing.Size(81, 24);
			this.butPrintAck.TabIndex = 19;
			this.butPrintAck.Text = "Preview";
			this.butPrintAck.Click += new System.EventHandler(this.butPrintAck_Click);
			// 
			// textAckNote
			// 
			this.textAckNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textAckNote.Location = new System.Drawing.Point(43, 550);
			this.textAckNote.Multiline = true;
			this.textAckNote.Name = "textAckNote";
			this.textAckNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textAckNote.Size = new System.Drawing.Size(355, 59);
			this.textAckNote.TabIndex = 20;
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label9.Location = new System.Drawing.Point(3, 428);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(100, 17);
			this.label9.TabIndex = 18;
			this.label9.Text = "DateTime Ack";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAckDateTime
			// 
			this.textAckDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textAckDateTime.Location = new System.Drawing.Point(104, 427);
			this.textAckDateTime.Name = "textAckDateTime";
			this.textAckDateTime.ReadOnly = true;
			this.textAckDateTime.Size = new System.Drawing.Size(200, 20);
			this.textAckDateTime.TabIndex = 17;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(12, 17);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(386, 17);
			this.label7.TabIndex = 5;
			this.label7.Text = "Message Text Received";
			this.label7.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textAckMessage
			// 
			this.textAckMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.textAckMessage.BackColor = System.Drawing.SystemColors.Window;
			this.textAckMessage.Location = new System.Drawing.Point(12, 37);
			this.textAckMessage.Name = "textAckMessage";
			this.textAckMessage.ReadOnly = true;
			this.textAckMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textAckMessage.Size = new System.Drawing.Size(386, 379);
			this.textAckMessage.TabIndex = 4;
			this.textAckMessage.Text = "";
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label8.Location = new System.Drawing.Point(11, 443);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(100, 17);
			this.label8.TabIndex = 16;
			this.label8.Text = "DateTime Trans";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateTimeTrans
			// 
			this.textDateTimeTrans.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textDateTimeTrans.Location = new System.Drawing.Point(112, 442);
			this.textDateTimeTrans.Name = "textDateTimeTrans";
			this.textDateTimeTrans.ReadOnly = true;
			this.textDateTimeTrans.Size = new System.Drawing.Size(200, 20);
			this.textDateTimeTrans.TabIndex = 15;
			// 
			// checkAttachments
			// 
			this.checkAttachments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkAttachments.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAttachments.Enabled = false;
			this.checkAttachments.Location = new System.Drawing.Point(1, 547);
			this.checkAttachments.Name = "checkAttachments";
			this.checkAttachments.Size = new System.Drawing.Size(125, 18);
			this.checkAttachments.TabIndex = 19;
			this.checkAttachments.Text = "Attachments Sent";
			this.checkAttachments.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAttachments.UseVisualStyleBackColor = true;
			this.checkAttachments.Visible = false;
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(386, 440);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(81, 24);
			this.butPrint.TabIndex = 18;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(721, 636);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(802, 636);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormEtransEdit
			// 
			this.ClientSize = new System.Drawing.Size(889, 672);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.checkAttachments);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textDateTimeTrans);
			this.Controls.Add(this.groupAck);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textAckCode);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textTransSetNum);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBatchNumber);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textClaimNum);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textMessageText);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormEtransEdit";
			this.ShowInTaskbar = false;
			this.Text = "Etrans Edit";
			this.Load += new System.EventHandler(this.FormEtransEdit_Load);
			this.groupAck.ResumeLayout(false);
			this.groupAck.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private RichTextBox textMessageText;
		private Label label1;
		private TextBox textClaimNum;
		private Label label2;
		private Label label3;
		private TextBox textBatchNumber;
		private Label label4;
		private TextBox textTransSetNum;
		private Label label5;
		private TextBox textAckCode;
		private Label label6;
		private TextBox textNote;
		private GroupBox groupAck;
		private Label label7;
		private RichTextBox textAckMessage;
		private Label label9;
		private TextBox textAckDateTime;
		private Label label8;
		private TextBox textDateTimeTrans;
		private OpenDental.UI.Button butPrint;
		private CheckBox checkAttachments;
		private OpenDental.UI.Button butPrintAck;
		private Label label10;
		private TextBox textAckNote;
	}
}
