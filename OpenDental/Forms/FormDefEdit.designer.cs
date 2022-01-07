using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormDefEdit {
		private System.ComponentModel.IContainer components = null;// Required designer variable.

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDefEdit));
			this.labelName = new System.Windows.Forms.Label();
			this.labelValue = new System.Windows.Forms.Label();
			this.textName = new System.Windows.Forms.TextBox();
			this.textValue = new System.Windows.Forms.TextBox();
			this.butColor = new System.Windows.Forms.Button();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.labelColor = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkHidden = new System.Windows.Forms.CheckBox();
			this.butDelete = new OpenDental.UI.Button();
			this.checkIncludeSend = new System.Windows.Forms.CheckBox();
			this.checkIncludeConfirm = new System.Windows.Forms.CheckBox();
			this.groupEConfirm = new System.Windows.Forms.GroupBox();
			this.butSelect = new OpenDental.UI.Button();
			this.butClearValue = new OpenDental.UI.Button();
			this.groupBoxEReminders = new System.Windows.Forms.GroupBox();
			this.checkIncludeRemind = new System.Windows.Forms.CheckBox();
			this.checkNoColor = new System.Windows.Forms.CheckBox();
			this.groupBoxEThanks = new System.Windows.Forms.GroupBox();
			this.checkIncludeThanks = new System.Windows.Forms.CheckBox();
			this.groupBoxArrivals = new System.Windows.Forms.GroupBox();
			this.checkIncludeArrivalSend = new System.Windows.Forms.CheckBox();
			this.checkIncludeArrivalResponse = new System.Windows.Forms.CheckBox();
			this.groupBoxEClipboard = new System.Windows.Forms.GroupBox();
			this.checkIncludeEClipboard = new System.Windows.Forms.CheckBox();
			this.checkByod = new System.Windows.Forms.CheckBox();			
			this.groupEConfirm.SuspendLayout();
			this.groupBoxEReminders.SuspendLayout();
			this.groupBoxEThanks.SuspendLayout();
			this.groupBoxArrivals.SuspendLayout();
			this.groupBoxEClipboard.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(12, 45);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(178, 17);
			this.labelName.TabIndex = 0;
			this.labelName.Text = "Name";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelValue
			// 
			this.labelValue.Location = new System.Drawing.Point(190, 34);
			this.labelValue.Name = "labelValue";
			this.labelValue.Size = new System.Drawing.Size(178, 28);
			this.labelValue.TabIndex = 1;
			this.labelValue.Text = "Value";
			this.labelValue.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(12, 64);
			this.textName.Multiline = true;
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(178, 64);
			this.textName.TabIndex = 0;
			// 
			// textValue
			// 
			this.textValue.Location = new System.Drawing.Point(190, 64);
			this.textValue.MaxLength = 256;
			this.textValue.Multiline = true;
			this.textValue.Name = "textValue";
			this.textValue.Size = new System.Drawing.Size(178, 64);
			this.textValue.TabIndex = 1;
			// 
			// butColor
			// 
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColor.Location = new System.Drawing.Point(371, 64);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(30, 20);
			this.butColor.TabIndex = 2;
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			// 
			// colorDialog1
			// 
			this.colorDialog1.FullOpen = true;
			// 
			// labelColor
			// 
			this.labelColor.Location = new System.Drawing.Point(371, 46);
			this.labelColor.Name = "labelColor";
			this.labelColor.Size = new System.Drawing.Size(74, 16);
			this.labelColor.TabIndex = 5;
			this.labelColor.Text = "Color";
			this.labelColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(255, 319);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 25);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(336, 319);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 25);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkHidden
			// 
			this.checkHidden.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHidden.Location = new System.Drawing.Point(12, 12);
			this.checkHidden.Name = "checkHidden";
			this.checkHidden.Size = new System.Drawing.Size(157, 18);
			this.checkHidden.TabIndex = 3;
			this.checkHidden.Text = "Hidden";
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 319);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(79, 25);
			this.butDelete.TabIndex = 6;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// checkIncludeSend
			// 
			this.checkIncludeSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkIncludeSend.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIncludeSend.Location = new System.Drawing.Point(6, 19);
			this.checkIncludeSend.Name = "checkIncludeSend";
			this.checkIncludeSend.Size = new System.Drawing.Size(165, 18);
			this.checkIncludeSend.TabIndex = 7;
			this.checkIncludeSend.Text = "Send";
			// 
			// checkIncludeConfirm
			// 
			this.checkIncludeConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkIncludeConfirm.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIncludeConfirm.Location = new System.Drawing.Point(6, 40);
			this.checkIncludeConfirm.Name = "checkIncludeConfirm";
			this.checkIncludeConfirm.Size = new System.Drawing.Size(165, 18);
			this.checkIncludeConfirm.TabIndex = 8;
			this.checkIncludeConfirm.Text = "Change status";
			// 
			// groupEConfirm
			// 
			this.groupEConfirm.Controls.Add(this.checkIncludeSend);
			this.groupEConfirm.Controls.Add(this.checkIncludeConfirm);
			this.groupEConfirm.Location = new System.Drawing.Point(12, 130);
			this.groupEConfirm.Name = "groupEConfirm";
			this.groupEConfirm.Size = new System.Drawing.Size(177, 64);
			this.groupEConfirm.TabIndex = 9;
			this.groupEConfirm.Text = "eConfirmations";
			// 
			// butSelect
			// 
			this.butSelect.Location = new System.Drawing.Point(371, 106);
			this.butSelect.Name = "butSelect";
			this.butSelect.Size = new System.Drawing.Size(21, 22);
			this.butSelect.TabIndex = 200;
			this.butSelect.Text = "...";
			this.butSelect.Click += new System.EventHandler(this.butSelect_Click);
			// 
			// butClearValue
			// 
			this.butClearValue.AdjustImageLocation = new System.Drawing.Point(1, 0);
			this.butClearValue.Image = global::OpenDental.Properties.Resources.deleteX18;
			this.butClearValue.Location = new System.Drawing.Point(395, 106);
			this.butClearValue.Name = "butClearValue";
			this.butClearValue.Size = new System.Drawing.Size(21, 22);
			this.butClearValue.TabIndex = 201;
			this.butClearValue.Click += new System.EventHandler(this.butClearValue_Click);
			// 
			// groupBoxEReminders
			// 
			this.groupBoxEReminders.Controls.Add(this.checkIncludeRemind);
			this.groupBoxEReminders.Location = new System.Drawing.Point(12, 197);
			this.groupBoxEReminders.Name = "groupBoxEReminders";
			this.groupBoxEReminders.Size = new System.Drawing.Size(177, 46);
			this.groupBoxEReminders.TabIndex = 202;
			this.groupBoxEReminders.Text = "eReminders";
			// 
			// checkIncludeRemind
			// 
			this.checkIncludeRemind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkIncludeRemind.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIncludeRemind.Location = new System.Drawing.Point(6, 19);
			this.checkIncludeRemind.Name = "checkIncludeRemind";
			this.checkIncludeRemind.Size = new System.Drawing.Size(165, 18);
			this.checkIncludeRemind.TabIndex = 7;
			this.checkIncludeRemind.Text = "Send";
			// 
			// checkNoColor
			// 
			this.checkNoColor.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNoColor.Location = new System.Drawing.Point(260, 16);
			this.checkNoColor.Name = "checkNoColor";
			this.checkNoColor.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkNoColor.Size = new System.Drawing.Size(141, 18);
			this.checkNoColor.TabIndex = 203;
			this.checkNoColor.Text = "No Color";
			this.checkNoColor.Visible = false;
			this.checkNoColor.CheckedChanged += new System.EventHandler(this.checkNoColor_CheckedChanged);
			// 
			// groupBoxEThanks
			// 
			this.groupBoxEThanks.Controls.Add(this.checkIncludeThanks);
			this.groupBoxEThanks.Location = new System.Drawing.Point(191, 197);
			this.groupBoxEThanks.Name = "groupBoxEThanks";
			this.groupBoxEThanks.Size = new System.Drawing.Size(177, 46);
			this.groupBoxEThanks.TabIndex = 203;
			this.groupBoxEThanks.Text = "Automated Thank-You";
			// 
			// checkIncludeThanks
			// 
			this.checkIncludeThanks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkIncludeThanks.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIncludeThanks.Location = new System.Drawing.Point(6, 19);
			this.checkIncludeThanks.Name = "checkIncludeThanks";
			this.checkIncludeThanks.Size = new System.Drawing.Size(165, 18);
			this.checkIncludeThanks.TabIndex = 7;
			this.checkIncludeThanks.Text = "Send";
			// 
			// groupBoxArrivals
			// 
			this.groupBoxArrivals.Controls.Add(this.checkIncludeArrivalSend);
			this.groupBoxArrivals.Controls.Add(this.checkIncludeArrivalResponse);
			this.groupBoxArrivals.Location = new System.Drawing.Point(191, 130);
			this.groupBoxArrivals.Name = "groupBoxArrivals";
			this.groupBoxArrivals.Size = new System.Drawing.Size(177, 64);
			this.groupBoxArrivals.TabIndex = 204;
			this.groupBoxArrivals.Text = "Arrivals";
			// 
			// checkIncludeArrivalSend
			// 
			this.checkIncludeArrivalSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkIncludeArrivalSend.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIncludeArrivalSend.Location = new System.Drawing.Point(6, 19);
			this.checkIncludeArrivalSend.Name = "checkIncludeArrivalSend";
			this.checkIncludeArrivalSend.Size = new System.Drawing.Size(165, 18);
			this.checkIncludeArrivalSend.TabIndex = 7;
			this.checkIncludeArrivalSend.Text = "Send";
			// 
			// checkIncludeArrivalResponse
			// 
			this.checkIncludeArrivalResponse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkIncludeArrivalResponse.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIncludeArrivalResponse.Location = new System.Drawing.Point(6, 40);
			this.checkIncludeArrivalResponse.Name = "checkIncludeArrivalResponse";
			this.checkIncludeArrivalResponse.Size = new System.Drawing.Size(165, 18);
			this.checkIncludeArrivalResponse.TabIndex = 8;
			this.checkIncludeArrivalResponse.Text = "Send response";
			// 
			// groupBoxEClipboard
			// 
			this.groupBoxEClipboard.Controls.Add(this.checkIncludeEClipboard);
			this.groupBoxEClipboard.Controls.Add(this.checkByod);
			this.groupBoxEClipboard.Location = new System.Drawing.Point(13, 246);
			this.groupBoxEClipboard.Name = "groupBoxEClipboard";
			this.groupBoxEClipboard.Size = new System.Drawing.Size(177, 64);
			this.groupBoxEClipboard.TabIndex = 205;
			this.groupBoxEClipboard.Text = "eClipboard";
			// 
			// checkIncludeEClipboard
			// 
			this.checkIncludeEClipboard.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIncludeEClipboard.Location = new System.Drawing.Point(5, 19);
			this.checkIncludeEClipboard.Name = "checkIncludeEClipboard";
			this.checkIncludeEClipboard.Size = new System.Drawing.Size(165, 18);
			this.checkIncludeEClipboard.TabIndex = 1;
			this.checkIncludeEClipboard.Text = "Change on check-in";
			this.checkIncludeEClipboard.UseVisualStyleBackColor = true;
			// 
			// checkByod
			// 
			this.checkByod.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkByod.Location = new System.Drawing.Point(5, 40);
			this.checkByod.Name = "checkByod";
			this.checkByod.Size = new System.Drawing.Size(165, 18);
			this.checkByod.TabIndex = 0;
			this.checkByod.Text = "Enable BYOD";
			this.checkByod.UseVisualStyleBackColor = true;
			// 
			// FormDefEdit
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(423, 352);
			this.Controls.Add(this.groupBoxEClipboard);
			this.Controls.Add(this.groupBoxArrivals);
			this.Controls.Add(this.groupBoxEThanks);
			this.Controls.Add(this.checkNoColor);
			this.Controls.Add(this.groupBoxEReminders);
			this.Controls.Add(this.butClearValue);
			this.Controls.Add(this.groupEConfirm);
			this.Controls.Add(this.butSelect);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.checkHidden);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butColor);
			this.Controls.Add(this.textValue);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.labelValue);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.labelColor);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormDefEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Definition";
			this.Load += new System.EventHandler(this.FormDefEdit_Load);
			this.groupEConfirm.ResumeLayout(false);
			this.groupBoxEReminders.ResumeLayout(false);
			this.groupBoxEThanks.ResumeLayout(false);
			this.groupBoxArrivals.ResumeLayout(false);
			this.groupBoxEClipboard.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.Label labelValue;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.TextBox textValue;
		private System.Windows.Forms.Button butColor;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.Button butSelect;
		private System.Windows.Forms.Label labelColor;
		private System.Windows.Forms.CheckBox checkHidden;
		private OpenDental.UI.Button butDelete;
		private CheckBox checkIncludeSend;
		private CheckBox checkIncludeConfirm;
		private GroupBox groupEConfirm;
		private UI.Button butClearValue;
		private GroupBox groupBoxEReminders;
		private CheckBox checkIncludeRemind;
		private CheckBox checkNoColor;
		private GroupBox groupBoxEThanks;
		private CheckBox checkIncludeThanks;
		private GroupBox groupBoxArrivals;
		private CheckBox checkIncludeArrivalSend;
		private CheckBox checkIncludeArrivalResponse;
		private GroupBox groupBoxEClipboard;
		private CheckBox checkIncludeEClipboard;
		private CheckBox checkByod;
	}
}
