using System.Drawing;
using System.Windows.Forms;

namespace OpenDental.UI {
	partial class SignatureBoxWrapper {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.butTopazSign = new Button();
			this.butClearSig = new Button();
			this.butESign = new Button();
			this.labelInvalidSig = new System.Windows.Forms.Label();
			this.sigBox = new OpenDental.UI.SignatureBox();
			this.SuspendLayout();
			// 
			// butTopazSign
			// 
			this.butTopazSign.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butTopazSign.Image = global::OpenDentBusiness.Properties.Resources.topazPen10;
			this.butTopazSign.Location = new System.Drawing.Point(326, 0);
			this.butTopazSign.Name = "butTopazSign";
			this.butTopazSign.Size = new System.Drawing.Size(20, 20);
			this.butTopazSign.TabIndex = 82;
			this.toolTip1.SetToolTip(this.butTopazSign, "Sign Topaz");
			this.butTopazSign.Click += new System.EventHandler(this.butTopazSign_Click);
			this.butClearSig.Padding = new Padding(0);
			this.butClearSig.Margin = new Padding(0);
			// 
			// butClearSig
			// 
			this.butClearSig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butClearSig.Image = global::OpenDentBusiness.Properties.Resources.deleteX10;
			this.butClearSig.Location = new System.Drawing.Point(344, 0);
			this.butClearSig.Name = "butClearSig";
			this.butClearSig.Size = new System.Drawing.Size(20, 20);
			this.butClearSig.TabIndex = 81;
			this.toolTip1.SetToolTip(this.butClearSig, "Clear Signature");
			this.butClearSig.Click += new System.EventHandler(this.butClearSig_Click);
			this.butClearSig.Padding = new Padding(0);
			this.butClearSig.Margin = new Padding(0);
			// 
			// butESign
			// 
			this.butESign.Image = global::OpenDentBusiness.Properties.Resources.eSig;
			this.butESign.Location = new System.Drawing.Point(0, 0);
			this.butESign.Name = "butESign";
			this.butESign.Size = new System.Drawing.Size(20, 20);
			this.butESign.TabIndex = 111;
			this.toolTip1.SetToolTip(this.butESign, "Digitally Sign");
			this.butESign.Visible = false;
			this.butESign.Click += new System.EventHandler(this.butESign_Click);
			this.butClearSig.Padding = new Padding(0);
			this.butClearSig.Margin = new Padding(0);
			// 
			// labelInvalidSig
			// 
			this.labelInvalidSig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInvalidSig.BackColor = System.Drawing.SystemColors.Window;
			this.labelInvalidSig.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelInvalidSig.Location = new System.Drawing.Point(84, 11);
			this.labelInvalidSig.Name = "labelInvalidSig";
			this.labelInvalidSig.Size = new System.Drawing.Size(196, 59);
			this.labelInvalidSig.TabIndex = 110;
			this.labelInvalidSig.Text = "Invalid Signature";
			this.labelInvalidSig.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelInvalidSig.Visible = false;
			// 
			// sigBox
			// 
			this.sigBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.sigBox.Location = new System.Drawing.Point(1, 1);
			this.sigBox.Name = "sigBox";
			this.sigBox.Size = new System.Drawing.Size(362, 79);
			this.sigBox.TabIndex = 87;
			this.sigBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sigBox_MouseDown);
			this.sigBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sigBox_MouseUp);
			// 
			// SignatureBoxWrapper
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Controls.Add(this.butESign);
			this.Controls.Add(this.labelInvalidSig);
			this.Controls.Add(this.butTopazSign);
			this.Controls.Add(this.butClearSig);
			this.Controls.Add(this.sigBox);
			this.Name = "SignatureBoxWrapper";
			this.Size = new System.Drawing.Size(364, 81);
			this.ResumeLayout(false);

		}

		#endregion

		private Button butClearSig;
		private System.Windows.Forms.ToolTip toolTip1;
		private Button butTopazSign;
		private SignatureBox sigBox;
		private System.Windows.Forms.Label labelInvalidSig;
		private Button butESign;
	}
}
