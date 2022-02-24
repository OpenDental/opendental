using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormBilling {
	private System.ComponentModel.IContainer components = null;// Required designer variable.

		///<summary></summary>
		protected override void Dispose(bool disposing){
			if(disposing){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBilling));
			this.butClose = new OpenDental.UI.Button();
			this.butSend = new OpenDental.UI.Button();
			this.butNone = new OpenDental.UI.Button();
			this.butAll = new OpenDental.UI.Button();
			this.gridBill = new OpenDental.UI.GridOD();
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemGoTo = new System.Windows.Forms.ToolStripMenuItem();
			this.labelTotal = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.radioUnsent = new System.Windows.Forms.RadioButton();
			this.radioSent = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelTexted = new System.Windows.Forms.Label();
			this.labelSentElect = new System.Windows.Forms.Label();
			this.labelEmailed = new System.Windows.Forms.Label();
			this.labelPrinted = new System.Windows.Forms.Label();
			this.labelSelected = new System.Windows.Forms.Label();
			this.butEdit = new OpenDental.UI.Button();
			this.textDateEnd = new OpenDental.ValidDate();
			this.textDateStart = new OpenDental.ValidDate();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.butRefresh = new OpenDental.UI.Button();
			this.comboOrder = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.butPrintList = new OpenDental.UI.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.comboEmailFrom = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.butDefaults = new OpenDental.UI.Button();
			this.contextMenu.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(802, 656);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 1;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butSend
			// 
			this.butSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSend.BackColor = System.Drawing.SystemColors.Control;
			this.butSend.Location = new System.Drawing.Point(802, 622);
			this.butSend.Name = "butSend";
			this.butSend.Size = new System.Drawing.Size(75, 24);
			this.butSend.TabIndex = 0;
			this.butSend.Text = "Send";
			this.butSend.UseVisualStyleBackColor = false;
			this.butSend.Click += new System.EventHandler(this.butSend_Click);
			// 
			// butNone
			// 
			this.butNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butNone.Location = new System.Drawing.Point(103, 656);
			this.butNone.Name = "butNone";
			this.butNone.Size = new System.Drawing.Size(75, 24);
			this.butNone.TabIndex = 23;
			this.butNone.Text = "&None";
			this.butNone.Click += new System.EventHandler(this.butNone_Click);
			// 
			// butAll
			// 
			this.butAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAll.Location = new System.Drawing.Point(12, 656);
			this.butAll.Name = "butAll";
			this.butAll.Size = new System.Drawing.Size(75, 24);
			this.butAll.TabIndex = 22;
			this.butAll.Text = "&All";
			this.butAll.Click += new System.EventHandler(this.butAll_Click);
			// 
			// gridBill
			// 
			this.gridBill.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridBill.ContextMenuStrip = this.contextMenu;
			this.gridBill.Location = new System.Drawing.Point(12, 58);
			this.gridBill.Name = "gridBill";
			this.gridBill.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridBill.Size = new System.Drawing.Size(772, 590);
			this.gridBill.TabIndex = 28;
			this.gridBill.Title = "Bills";
			this.gridBill.TranslationName = "TableBilling";
			this.gridBill.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridBill_CellDoubleClick);
			this.gridBill.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridBill_CellClick);
			this.gridBill.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridBill_MouseDown);
			// 
			// contextMenu
			// 
			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemGoTo});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new System.Drawing.Size(105, 26);
			// 
			// menuItemGoTo
			// 
			this.menuItemGoTo.Name = "menuItemGoTo";
			this.menuItemGoTo.Size = new System.Drawing.Size(104, 22);
			this.menuItemGoTo.Text = "Go To";
			this.menuItemGoTo.Click += new System.EventHandler(this.menuItemGoTo_Click);
			// 
			// labelTotal
			// 
			this.labelTotal.Location = new System.Drawing.Point(4, 19);
			this.labelTotal.Name = "labelTotal";
			this.labelTotal.Size = new System.Drawing.Size(89, 16);
			this.labelTotal.TabIndex = 29;
			this.labelTotal.Text = "Total=20";
			this.labelTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(787, 520);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(99, 99);
			this.label1.TabIndex = 30;
			this.label1.Text = "Immediately compiles selected bills into a PDF, sends them electronically, or ema" +
    "ils them";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// radioUnsent
			// 
			this.radioUnsent.Checked = true;
			this.radioUnsent.Location = new System.Drawing.Point(31, 7);
			this.radioUnsent.Name = "radioUnsent";
			this.radioUnsent.Size = new System.Drawing.Size(75, 20);
			this.radioUnsent.TabIndex = 31;
			this.radioUnsent.TabStop = true;
			this.radioUnsent.Text = "Unsent";
			this.radioUnsent.UseVisualStyleBackColor = true;
			this.radioUnsent.Click += new System.EventHandler(this.radioUnsent_Click);
			// 
			// radioSent
			// 
			this.radioSent.Location = new System.Drawing.Point(31, 29);
			this.radioSent.Name = "radioSent";
			this.radioSent.Size = new System.Drawing.Size(75, 20);
			this.radioSent.TabIndex = 32;
			this.radioSent.Text = "Sent";
			this.radioSent.UseVisualStyleBackColor = true;
			this.radioSent.Click += new System.EventHandler(this.radioSent_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.labelTexted);
			this.groupBox1.Controls.Add(this.labelSentElect);
			this.groupBox1.Controls.Add(this.labelEmailed);
			this.groupBox1.Controls.Add(this.labelPrinted);
			this.groupBox1.Controls.Add(this.labelSelected);
			this.groupBox1.Controls.Add(this.labelTotal);
			this.groupBox1.Location = new System.Drawing.Point(790, 97);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(96, 136);
			this.groupBox1.TabIndex = 33;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Counts";
			// 
			// labelTexted
			// 
			this.labelTexted.Location = new System.Drawing.Point(4, 114);
			this.labelTexted.Name = "labelTexted";
			this.labelTexted.Size = new System.Drawing.Size(89, 16);
			this.labelTexted.TabIndex = 34;
			this.labelTexted.Text = "Texted=20";
			this.labelTexted.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelSentElect
			// 
			this.labelSentElect.Location = new System.Drawing.Point(4, 95);
			this.labelSentElect.Name = "labelSentElect";
			this.labelSentElect.Size = new System.Drawing.Size(89, 16);
			this.labelSentElect.TabIndex = 33;
			this.labelSentElect.Text = "SentElect=20";
			this.labelSentElect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelEmailed
			// 
			this.labelEmailed.Location = new System.Drawing.Point(4, 76);
			this.labelEmailed.Name = "labelEmailed";
			this.labelEmailed.Size = new System.Drawing.Size(89, 16);
			this.labelEmailed.TabIndex = 32;
			this.labelEmailed.Text = "Emailed=20";
			this.labelEmailed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPrinted
			// 
			this.labelPrinted.Location = new System.Drawing.Point(4, 57);
			this.labelPrinted.Name = "labelPrinted";
			this.labelPrinted.Size = new System.Drawing.Size(89, 16);
			this.labelPrinted.TabIndex = 31;
			this.labelPrinted.Text = "Printed=20";
			this.labelPrinted.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelSelected
			// 
			this.labelSelected.Location = new System.Drawing.Point(4, 38);
			this.labelSelected.Name = "labelSelected";
			this.labelSelected.Size = new System.Drawing.Size(89, 16);
			this.labelSelected.TabIndex = 30;
			this.labelSelected.Text = "Selected=20";
			this.labelSelected.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butEdit
			// 
			this.butEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butEdit.Location = new System.Drawing.Point(796, 67);
			this.butEdit.Name = "butEdit";
			this.butEdit.Size = new System.Drawing.Size(82, 24);
			this.butEdit.TabIndex = 34;
			this.butEdit.Text = "Edit Selected";
			this.butEdit.Click += new System.EventHandler(this.butEdit_Click);
			// 
			// textDateEnd
			// 
			this.textDateEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDateEnd.Location = new System.Drawing.Point(694, 32);
			this.textDateEnd.Name = "textDateEnd";
			this.textDateEnd.Size = new System.Drawing.Size(77, 20);
			this.textDateEnd.TabIndex = 38;
			// 
			// textDateStart
			// 
			this.textDateStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDateStart.Location = new System.Drawing.Point(695, 8);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(77, 20);
			this.textDateStart.TabIndex = 37;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(628, 35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 14);
			this.label2.TabIndex = 36;
			this.label2.Text = "End Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(622, 11);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(70, 14);
			this.label3.TabIndex = 35;
			this.label3.Text = "Start Date";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(796, 7);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(82, 24);
			this.butRefresh.TabIndex = 39;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// comboOrder
			// 
			this.comboOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboOrder.Location = new System.Drawing.Point(196, 8);
			this.comboOrder.MaxDropDownItems = 40;
			this.comboOrder.Name = "comboOrder";
			this.comboOrder.Size = new System.Drawing.Size(173, 21);
			this.comboOrder.TabIndex = 41;
			this.comboOrder.SelectionChangeCommitted += new System.EventHandler(this.comboOrder_SelectionChangeCommitted);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(112, 12);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(82, 14);
			this.label4.TabIndex = 40;
			this.label4.Text = "Order by";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPrintList
			// 
			this.butPrintList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrintList.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrintList.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrintList.Location = new System.Drawing.Point(196, 656);
			this.butPrintList.Name = "butPrintList";
			this.butPrintList.Size = new System.Drawing.Size(88, 24);
			this.butPrintList.TabIndex = 42;
			this.butPrintList.Text = "Print List";
			this.butPrintList.Click += new System.EventHandler(this.butPrintList_Click);
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.Location = new System.Drawing.Point(288, 651);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(165, 29);
			this.label5.TabIndex = 43;
			this.label5.Text = "Does not print individual bills.  Just prints the list of bills.";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// comboClinic
			// 
			this.comboClinic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClinic.IncludeAll = true;
			this.comboClinic.Location = new System.Drawing.Point(390, 8);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(226, 21);
			this.comboClinic.TabIndex = 44;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// comboEmailFrom
			// 
			this.comboEmailFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboEmailFrom.Location = new System.Drawing.Point(196, 32);
			this.comboEmailFrom.MaxDropDownItems = 40;
			this.comboEmailFrom.Name = "comboEmailFrom";
			this.comboEmailFrom.Size = new System.Drawing.Size(173, 21);
			this.comboEmailFrom.TabIndex = 47;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(112, 36);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(82, 14);
			this.label6.TabIndex = 46;
			this.label6.Text = "Email From";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDefaults
			// 
			this.butDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDefaults.BackColor = System.Drawing.SystemColors.Control;
			this.butDefaults.Location = new System.Drawing.Point(802, 493);
			this.butDefaults.Name = "butDefaults";
			this.butDefaults.Size = new System.Drawing.Size(74, 24);
			this.butDefaults.TabIndex = 48;
			this.butDefaults.Text = "Defaults";
			this.butDefaults.UseVisualStyleBackColor = false;
			this.butDefaults.Click += new System.EventHandler(this.butDefaults_Click);
			// 
			// FormBilling
			// 
			this.AcceptButton = this.butSend;
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(888, 688);
			this.Controls.Add(this.butSend);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butDefaults);
			this.Controls.Add(this.comboEmailFrom);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.comboOrder);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.butPrintList);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butEdit);
			this.Controls.Add(this.textDateStart);
			this.Controls.Add(this.textDateEnd);
			this.Controls.Add(this.radioUnsent);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.radioSent);
			this.Controls.Add(this.gridBill);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butNone);
			this.Controls.Add(this.butAll);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormBilling";
			this.Text = "Bills";
			this.CloseXClicked += new System.ComponentModel.CancelEventHandler(this.FormBilling_CloseXClicked);
			this.Activated += new System.EventHandler(this.FormBilling_Activated);
			this.Load += new System.EventHandler(this.FormBilling_Load);
			this.contextMenu.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butAll;
		private OpenDental.UI.Button butNone;
		private OpenDental.UI.Button butSend;
		private OpenDental.UI.GridOD gridBill;
		private Label labelTotal;
		private Label label1;
		private RadioButton radioUnsent;
		private RadioButton radioSent;
		private GroupBox groupBox1;
		private Label labelSelected;
		private Label labelEmailed;
		private Label labelPrinted;
		private OpenDental.UI.Button butEdit;
		private ValidDate textDateEnd;
		private ValidDate textDateStart;
		private Label label2;
		private Label label3;
		private OpenDental.UI.Button butRefresh;
		private ComboBox comboOrder;
		private Label label4;
		private OpenDental.UI.Button butPrintList;
		private ContextMenuStrip contextMenu;
		private ToolStripMenuItem menuItemGoTo;
		private Label label5;
		private Label labelSentElect;
		private OpenDental.UI.ComboBoxClinicPicker comboClinic;
		private ComboBox comboEmailFrom;
		private Label label6;
		private UI.Button butDefaults;
		private Label labelTexted;
	}
}
