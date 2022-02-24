using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormAutomationEdit {
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
			OpenDental.UI.Button butDelete;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutomationEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textProcCodes = new System.Windows.Forms.TextBox();
			this.labelProcCodes = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.labelActionObject = new System.Windows.Forms.Label();
			this.labelMessage = new System.Windows.Forms.Label();
			this.textMessage = new System.Windows.Forms.TextBox();
			this.comboTrigger = new System.Windows.Forms.ComboBox();
			this.comboAction = new System.Windows.Forms.ComboBox();
			this.comboActionObject = new OpenDental.UI.ComboBoxOD();
			this.butAdd = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butProcCode = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			butDelete = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butDelete
			// 
			butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			butDelete.Location = new System.Drawing.Point(48, 393);
			butDelete.Name = "butDelete";
			butDelete.Size = new System.Drawing.Size(75, 24);
			butDelete.TabIndex = 16;
			butDelete.Text = "&Delete";
			butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(48, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(111, 20);
			this.label1.TabIndex = 11;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(161, 25);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(316, 20);
			this.textDescription.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(48, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(111, 20);
			this.label2.TabIndex = 18;
			this.label2.Text = "Trigger";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProcCodes
			// 
			this.textProcCodes.Location = new System.Drawing.Point(161, 77);
			this.textProcCodes.Name = "textProcCodes";
			this.textProcCodes.Size = new System.Drawing.Size(316, 20);
			this.textProcCodes.TabIndex = 2;
			// 
			// labelProcCodes
			// 
			this.labelProcCodes.Location = new System.Drawing.Point(13, 76);
			this.labelProcCodes.Name = "labelProcCodes";
			this.labelProcCodes.Size = new System.Drawing.Size(146, 29);
			this.labelProcCodes.TabIndex = 20;
			this.labelProcCodes.Text = "Procedure Code(s)\r\n(separated with commas)";
			this.labelProcCodes.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 256);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(143, 17);
			this.label4.TabIndex = 21;
			this.label4.Text = "Action";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelActionObject
			// 
			this.labelActionObject.Location = new System.Drawing.Point(16, 282);
			this.labelActionObject.Name = "labelActionObject";
			this.labelActionObject.Size = new System.Drawing.Size(143, 17);
			this.labelActionObject.TabIndex = 22;
			this.labelActionObject.Text = "Action Object";
			this.labelActionObject.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelMessage
			// 
			this.labelMessage.Location = new System.Drawing.Point(16, 307);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(143, 17);
			this.labelMessage.TabIndex = 25;
			this.labelMessage.Text = "Message";
			this.labelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMessage
			// 
			this.textMessage.Location = new System.Drawing.Point(161, 308);
			this.textMessage.Multiline = true;
			this.textMessage.Name = "textMessage";
			this.textMessage.Size = new System.Drawing.Size(316, 73);
			this.textMessage.TabIndex = 26;
			// 
			// comboTrigger
			// 
			this.comboTrigger.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboTrigger.FormattingEnabled = true;
			this.comboTrigger.Location = new System.Drawing.Point(161, 50);
			this.comboTrigger.Name = "comboTrigger";
			this.comboTrigger.Size = new System.Drawing.Size(183, 21);
			this.comboTrigger.TabIndex = 27;
			this.comboTrigger.SelectedIndexChanged += new System.EventHandler(this.comboTrigger_SelectedIndexChanged);
			// 
			// comboAction
			// 
			this.comboAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboAction.FormattingEnabled = true;
			this.comboAction.Location = new System.Drawing.Point(161, 255);
			this.comboAction.Name = "comboAction";
			this.comboAction.Size = new System.Drawing.Size(183, 21);
			this.comboAction.TabIndex = 28;
			this.comboAction.SelectedIndexChanged += new System.EventHandler(this.comboAction_SelectedIndexChanged);
			// 
			// comboActionObject
			// 
			this.comboActionObject.Location = new System.Drawing.Point(161, 281);
			this.comboActionObject.Name = "comboActionObject";
			this.comboActionObject.Size = new System.Drawing.Size(183, 21);
			this.comboActionObject.TabIndex = 31;
			// 
			// butAdd
			// 
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(677, 225);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(65, 24);
			this.butAdd.TabIndex = 35;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// gridMain
			// 
			this.gridMain.Location = new System.Drawing.Point(161, 103);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridMain.Size = new System.Drawing.Size(510, 146);
			this.gridMain.TabIndex = 34;
			this.gridMain.Title = "Conditions";
			this.gridMain.TranslationName = "TableConditions";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butProcCode
			// 
			this.butProcCode.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butProcCode.Location = new System.Drawing.Point(479, 75);
			this.butProcCode.Name = "butProcCode";
			this.butProcCode.Size = new System.Drawing.Size(23, 24);
			this.butProcCode.TabIndex = 32;
			this.butProcCode.Text = "...";
			this.butProcCode.Click += new System.EventHandler(this.butProcCode_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(589, 393);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(677, 393);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormAutomationEdit
			// 
			this.ClientSize = new System.Drawing.Size(778, 437);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butProcCode);
			this.Controls.Add(this.comboActionObject);
			this.Controls.Add(this.comboAction);
			this.Controls.Add(this.comboTrigger);
			this.Controls.Add(this.textMessage);
			this.Controls.Add(this.labelMessage);
			this.Controls.Add(this.labelActionObject);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textProcCodes);
			this.Controls.Add(this.labelProcCodes);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(butDelete);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAutomationEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Automation";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAutomationEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormAutomationEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label label1;
		private TextBox textDescription;
		private Label label2;
		private TextBox textProcCodes;
		private Label labelProcCodes;
		private Label label4;
		private Label labelActionObject;
		private Label labelMessage;
		private TextBox textMessage;
		private ComboBox comboTrigger;
		private ComboBox comboAction;
		private UI.ComboBoxOD comboActionObject;
		private OpenDental.UI.Button butProcCode;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button butAdd;
	}
}
