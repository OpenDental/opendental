using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormApptFieldDefEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptFieldDefEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.textName = new System.Windows.Forms.TextBox();
			this.buttonDelete = new OpenDental.UI.Button();
			this.labelStatus = new System.Windows.Forms.Label();
			this.labelWarning = new System.Windows.Forms.Label();
			this.textPickList = new System.Windows.Forms.TextBox();
			this.labelFieldType = new System.Windows.Forms.Label();
			this.comboFieldType = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(254, 451);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(254, 416);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(20, 27);
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(308, 20);
			this.textName.TabIndex = 0;
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.buttonDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonDelete.Location = new System.Drawing.Point(20, 451);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(82, 24);
			this.buttonDelete.TabIndex = 3;
			this.buttonDelete.Text = "&Delete";
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// labelStatus
			// 
			this.labelStatus.Location = new System.Drawing.Point(17, 9);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(143, 15);
			this.labelStatus.TabIndex = 80;
			this.labelStatus.Text = "Field Name";
			this.labelStatus.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelWarning
			// 
			this.labelWarning.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelWarning.Location = new System.Drawing.Point(203, 71);
			this.labelWarning.Name = "labelWarning";
			this.labelWarning.Size = new System.Drawing.Size(101, 14);
			this.labelWarning.TabIndex = 89;
			this.labelWarning.Text = "One Per Line";
			this.labelWarning.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.labelWarning.Visible = false;
			// 
			// textPickList
			// 
			this.textPickList.AcceptsReturn = true;
			this.textPickList.Location = new System.Drawing.Point(20, 96);
			this.textPickList.MaxLength = 4000;
			this.textPickList.Multiline = true;
			this.textPickList.Name = "textPickList";
			this.textPickList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textPickList.Size = new System.Drawing.Size(309, 301);
			this.textPickList.TabIndex = 88;
			// 
			// labelFieldType
			// 
			this.labelFieldType.Location = new System.Drawing.Point(17, 50);
			this.labelFieldType.Name = "labelFieldType";
			this.labelFieldType.Size = new System.Drawing.Size(143, 15);
			this.labelFieldType.TabIndex = 87;
			this.labelFieldType.Text = "Field Type";
			this.labelFieldType.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// comboFieldType
			// 
			this.comboFieldType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboFieldType.DropDownWidth = 177;
			this.comboFieldType.FormattingEnabled = true;
			this.comboFieldType.Location = new System.Drawing.Point(20, 68);
			this.comboFieldType.MaxDropDownItems = 30;
			this.comboFieldType.Name = "comboFieldType";
			this.comboFieldType.Size = new System.Drawing.Size(177, 21);
			this.comboFieldType.TabIndex = 86;
			this.comboFieldType.SelectedIndexChanged += new System.EventHandler(this.comboFieldType_SelectedIndexChanged);
			// 
			// FormApptFieldDefEdit
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(349, 493);
			this.Controls.Add(this.labelWarning);
			this.Controls.Add(this.textPickList);
			this.Controls.Add(this.labelFieldType);
			this.Controls.Add(this.comboFieldType);
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.buttonDelete);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormApptFieldDefEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Appointment Field Def";
			this.Load += new System.EventHandler(this.FormApptFieldDefEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.TextBox textName;
		private OpenDental.UI.Button buttonDelete;
		private Label labelStatus;
		private Label labelWarning;
		private TextBox textPickList;
		private Label labelFieldType;
		private ComboBox comboFieldType;
	}
}
