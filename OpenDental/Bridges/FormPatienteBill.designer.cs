/*=================================================================
Created by Practice-Web Inc. (R) 2009. http://www.practice-web.com
Retain this text in redistributions.
==================================================================*/
namespace OpenDental
{
    partial class FormPatienteBill
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
            this.tBStatus = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tBError = new System.Windows.Forms.TextBox();
            this.butClose = new OpenDental.UI.Button();
            this.SuspendLayout();
            // 
            // tBStatus
            // 
            this.tBStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBStatus.ForeColor = System.Drawing.Color.Blue;
            this.tBStatus.Location = new System.Drawing.Point(7, 23);
            this.tBStatus.Multiline = true;
            this.tBStatus.Name = "tBStatus";
            this.tBStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tBStatus.Size = new System.Drawing.Size(458, 197);
            this.tBStatus.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(7, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 14);
            this.label4.TabIndex = 41;
            this.label4.Text = "Progress...";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(7, 223);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 14);
            this.label1.TabIndex = 43;
            this.label1.Text = "Error...";
            // 
            // tBError
            // 
            this.tBError.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBError.ForeColor = System.Drawing.Color.Red;
            this.tBError.Location = new System.Drawing.Point(7, 240);
            this.tBError.Multiline = true;
            this.tBError.Name = "tBError";
            this.tBError.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tBError.Size = new System.Drawing.Size(458, 115);
            this.tBError.TabIndex = 44;
            // 
            // butClose
            // 
            this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butClose.Location = new System.Drawing.Point(199, 361);
            this.butClose.Name = "butClose";
            this.butClose.Size = new System.Drawing.Size(75, 24);
            this.butClose.TabIndex = 42;
            this.butClose.Text = "Close";
            // 
            // FormPatienteBill
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 391);
            this.Controls.Add(this.tBError);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.butClose);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tBStatus);
            this.Name = "FormPatienteBill";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormPatienteBill";
            this.Load += new System.EventHandler(this.FormPatienteBill_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tBStatus;
        private System.Windows.Forms.Label label4;
        private OpenDental.UI.Button butClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tBError;
    }
}