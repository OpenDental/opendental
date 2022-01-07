using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormClaimPayBatch {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClaimPayBatch));
			this.menuRightAttached = new System.Windows.Forms.ContextMenu();
			this.menuItemGotoAccount = new System.Windows.Forms.MenuItem();
			this.menuRightOut = new System.Windows.Forms.ContextMenu();
			this.menuItemGotoOut = new System.Windows.Forms.MenuItem();
			this.groupFilters = new System.Windows.Forms.GroupBox();
			this.butCarrierPick = new OpenDental.UI.Button();
			this.textCarrier = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.textName = new System.Windows.Forms.TextBox();
			this.textClaimID = new System.Windows.Forms.TextBox();
			this.butRefresh = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.textTotal = new OpenDental.ValidDouble();
			this.butAttach = new OpenDental.UI.Button();
			this.textEobIsScanned = new System.Windows.Forms.TextBox();
			this.butView = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.labelInstruct2 = new System.Windows.Forms.Label();
			this.butUp = new OpenDental.UI.Button();
			this.labelInstruct1 = new System.Windows.Forms.Label();
			this.butDetach = new OpenDental.UI.Button();
			this.gridOut = new OpenDental.UI.GridOD();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label13 = new System.Windows.Forms.Label();
			this.textPayGroup = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.textPayType = new System.Windows.Forms.TextBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.textDateIssued = new OpenDental.ValidDate();
			this.label2 = new System.Windows.Forms.Label();
			this.labelDateIssued = new System.Windows.Forms.Label();
			this.butClaimPayEdit = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textClinic = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textCarrierName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.textCheckNum = new System.Windows.Forms.TextBox();
			this.textBankBranch = new System.Windows.Forms.TextBox();
			this.textAmount = new OpenDental.ValidDouble();
			this.textDate = new OpenDental.ValidDate();
			this.gridAttached = new OpenDental.UI.GridOD();
			this.butDelete = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.butViewEra = new OpenDental.UI.Button();
			this.groupFilters.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuRightAttached
			// 
			this.menuRightAttached.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemGotoAccount});
			// 
			// menuItemGotoAccount
			// 
			this.menuItemGotoAccount.Index = 0;
			this.menuItemGotoAccount.Text = "Go to Account";
			this.menuItemGotoAccount.Click += new System.EventHandler(this.menuItemGotoAccount_Click);
			// 
			// menuRightOut
			// 
			this.menuRightOut.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemGotoOut});
			// 
			// menuItemGotoOut
			// 
			this.menuItemGotoOut.Index = 0;
			this.menuItemGotoOut.Text = "Go to Account";
			this.menuItemGotoOut.Click += new System.EventHandler(this.menuItemGotoOut_Click);
			// 
			// groupFilters
			// 
			this.groupFilters.Controls.Add(this.butCarrierPick);
			this.groupFilters.Controls.Add(this.textCarrier);
			this.groupFilters.Controls.Add(this.label12);
			this.groupFilters.Controls.Add(this.label9);
			this.groupFilters.Controls.Add(this.label11);
			this.groupFilters.Controls.Add(this.textName);
			this.groupFilters.Controls.Add(this.textClaimID);
			this.groupFilters.Location = new System.Drawing.Point(230, 367);
			this.groupFilters.Name = "groupFilters";
			this.groupFilters.Size = new System.Drawing.Size(732, 45);
			this.groupFilters.TabIndex = 119;
			this.groupFilters.TabStop = false;
			this.groupFilters.Text = "Filters";
			// 
			// butCarrierPick
			// 
			this.butCarrierPick.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCarrierPick.Location = new System.Drawing.Point(236, 15);
			this.butCarrierPick.Name = "butCarrierPick";
			this.butCarrierPick.Size = new System.Drawing.Size(19, 20);
			this.butCarrierPick.TabIndex = 120;
			this.butCarrierPick.Text = "...";
			this.butCarrierPick.Click += new System.EventHandler(this.butCarrierPick_Click);
			// 
			// textCarrier
			// 
			this.textCarrier.Location = new System.Drawing.Point(101, 15);
			this.textCarrier.Name = "textCarrier";
			this.textCarrier.Size = new System.Drawing.Size(132, 20);
			this.textCarrier.TabIndex = 112;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(235, 17);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(106, 16);
			this.label12.TabIndex = 118;
			this.label12.Text = "Name";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(6, 17);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(95, 16);
			this.label9.TabIndex = 113;
			this.label9.Text = "Carrier";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(479, 16);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(103, 16);
			this.label11.TabIndex = 117;
			this.label11.Text = "Claim ID";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(341, 15);
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(132, 20);
			this.textName.TabIndex = 113;
			// 
			// textClaimID
			// 
			this.textClaimID.Location = new System.Drawing.Point(582, 15);
			this.textClaimID.Name = "textClaimID";
			this.textClaimID.Size = new System.Drawing.Size(132, 20);
			this.textClaimID.TabIndex = 114;
			// 
			// butRefresh
			// 
			this.butRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRefresh.Location = new System.Drawing.Point(479, 341);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(54, 24);
			this.butRefresh.TabIndex = 114;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// butDown
			// 
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(271, 341);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(39, 24);
			this.butDown.TabIndex = 104;
			this.butDown.Text = "#";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// textTotal
			// 
			this.textTotal.Location = new System.Drawing.Point(863, 344);
			this.textTotal.MaxVal = 100000000D;
			this.textTotal.MinVal = -100000000D;
			this.textTotal.Name = "textTotal";
			this.textTotal.ReadOnly = true;
			this.textTotal.Size = new System.Drawing.Size(81, 20);
			this.textTotal.TabIndex = 0;
			this.textTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// butAttach
			// 
			this.butAttach.Image = global::OpenDental.Properties.Resources.up;
			this.butAttach.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAttach.Location = new System.Drawing.Point(539, 341);
			this.butAttach.Name = "butAttach";
			this.butAttach.Size = new System.Drawing.Size(71, 24);
			this.butAttach.TabIndex = 111;
			this.butAttach.Text = "Attach";
			this.butAttach.Click += new System.EventHandler(this.butAttach_Click);
			// 
			// textEobIsScanned
			// 
			this.textEobIsScanned.Location = new System.Drawing.Point(145, 580);
			this.textEobIsScanned.MaxLength = 25;
			this.textEobIsScanned.Name = "textEobIsScanned";
			this.textEobIsScanned.ReadOnly = true;
			this.textEobIsScanned.Size = new System.Drawing.Size(72, 20);
			this.textEobIsScanned.TabIndex = 110;
			// 
			// butView
			// 
			this.butView.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butView.Location = new System.Drawing.Point(145, 606);
			this.butView.Name = "butView";
			this.butView.Size = new System.Drawing.Size(72, 24);
			this.butView.TabIndex = 109;
			this.butView.Text = "View EOB";
			this.butView.Click += new System.EventHandler(this.butView_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(21, 583);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(123, 16);
			this.label1.TabIndex = 108;
			this.label1.Text = "EOB is Scanned";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(805, 646);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 107;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelInstruct2
			// 
			this.labelInstruct2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelInstruct2.Location = new System.Drawing.Point(10, 27);
			this.labelInstruct2.Name = "labelInstruct2";
			this.labelInstruct2.Size = new System.Drawing.Size(207, 523);
			this.labelInstruct2.TabIndex = 105;
			this.labelInstruct2.Text = resources.GetString("labelInstruct2.Text");
			// 
			// butUp
			// 
			this.butUp.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(230, 341);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(39, 24);
			this.butUp.TabIndex = 103;
			this.butUp.Text = "#";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// labelInstruct1
			// 
			this.labelInstruct1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelInstruct1.Location = new System.Drawing.Point(9, 1);
			this.labelInstruct1.Name = "labelInstruct1";
			this.labelInstruct1.Size = new System.Drawing.Size(177, 20);
			this.labelInstruct1.TabIndex = 102;
			this.labelInstruct1.Text = "Instructions";
			this.labelInstruct1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butDetach
			// 
			this.butDetach.Image = global::OpenDental.Properties.Resources.down;
			this.butDetach.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDetach.Location = new System.Drawing.Point(612, 341);
			this.butDetach.Name = "butDetach";
			this.butDetach.Size = new System.Drawing.Size(71, 24);
			this.butDetach.TabIndex = 101;
			this.butDetach.Text = "Detach";
			this.butDetach.Click += new System.EventHandler(this.butDetach_Click);
			// 
			// gridOut
			// 
			this.gridOut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridOut.Location = new System.Drawing.Point(230, 418);
			this.gridOut.Name = "gridOut";
			this.gridOut.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridOut.Size = new System.Drawing.Size(732, 212);
			this.gridOut.TabIndex = 99;
			this.gridOut.Title = "All Outstanding Claims";
			this.gridOut.TranslationName = "TableClaimPaySplits";
			this.gridOut.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridOut_CellDoubleClick);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label13);
			this.groupBox1.Controls.Add(this.textPayGroup);
			this.groupBox1.Controls.Add(this.label10);
			this.groupBox1.Controls.Add(this.textPayType);
			this.groupBox1.Controls.Add(this.labelClinic);
			this.groupBox1.Controls.Add(this.textDateIssued);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.labelDateIssued);
			this.groupBox1.Controls.Add(this.butClaimPayEdit);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.textClinic);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.textCarrierName);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.textNote);
			this.groupBox1.Controls.Add(this.textCheckNum);
			this.groupBox1.Controls.Add(this.textBankBranch);
			this.groupBox1.Controls.Add(this.textAmount);
			this.groupBox1.Controls.Add(this.textDate);
			this.groupBox1.Location = new System.Drawing.Point(230, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(731, 110);
			this.groupBox1.TabIndex = 98;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Payment Details";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(452, 86);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(81, 16);
			this.label13.TabIndex = 101;
			this.label13.Text = "Group";
			this.label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textPayGroup
			// 
			this.textPayGroup.Location = new System.Drawing.Point(537, 83);
			this.textPayGroup.MaxLength = 25;
			this.textPayGroup.Name = "textPayGroup";
			this.textPayGroup.ReadOnly = true;
			this.textPayGroup.Size = new System.Drawing.Size(97, 20);
			this.textPayGroup.TabIndex = 100;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(452, 65);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(81, 16);
			this.label10.TabIndex = 99;
			this.label10.Text = "Type";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textPayType
			// 
			this.textPayType.Location = new System.Drawing.Point(537, 62);
			this.textPayType.MaxLength = 25;
			this.textPayType.Name = "textPayType";
			this.textPayType.ReadOnly = true;
			this.textPayType.Size = new System.Drawing.Size(97, 20);
			this.textPayType.TabIndex = 98;
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(21, 22);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(86, 14);
			this.labelClinic.TabIndex = 91;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDateIssued
			// 
			this.textDateIssued.Location = new System.Drawing.Point(110, 61);
			this.textDateIssued.Name = "textDateIssued";
			this.textDateIssued.ReadOnly = true;
			this.textDateIssued.Size = new System.Drawing.Size(68, 20);
			this.textDateIssued.TabIndex = 96;
			this.textDateIssued.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(238, 41);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(104, 16);
			this.label2.TabIndex = 33;
			this.label2.Text = "Note";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDateIssued
			// 
			this.labelDateIssued.Location = new System.Drawing.Point(12, 65);
			this.labelDateIssued.Name = "labelDateIssued";
			this.labelDateIssued.Size = new System.Drawing.Size(96, 16);
			this.labelDateIssued.TabIndex = 97;
			this.labelDateIssued.Text = "Date Issued";
			this.labelDateIssued.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butClaimPayEdit
			// 
			this.butClaimPayEdit.Location = new System.Drawing.Point(650, 78);
			this.butClaimPayEdit.Name = "butClaimPayEdit";
			this.butClaimPayEdit.Size = new System.Drawing.Size(75, 24);
			this.butClaimPayEdit.TabIndex = 6;
			this.butClaimPayEdit.Text = "Edit";
			this.butClaimPayEdit.Click += new System.EventHandler(this.butClaimPayEdit_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(254, 85);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(91, 16);
			this.label3.TabIndex = 34;
			this.label3.Text = "Bank-Branch";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(253, 63);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(90, 16);
			this.label4.TabIndex = 35;
			this.label4.Text = "Check #";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textClinic
			// 
			this.textClinic.Location = new System.Drawing.Point(110, 19);
			this.textClinic.MaxLength = 25;
			this.textClinic.Name = "textClinic";
			this.textClinic.ReadOnly = true;
			this.textClinic.Size = new System.Drawing.Size(123, 20);
			this.textClinic.TabIndex = 93;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(13, 86);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(95, 16);
			this.label5.TabIndex = 36;
			this.label5.Text = "Amount";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textCarrierName
			// 
			this.textCarrierName.Location = new System.Drawing.Point(346, 19);
			this.textCarrierName.MaxLength = 25;
			this.textCarrierName.Name = "textCarrierName";
			this.textCarrierName.ReadOnly = true;
			this.textCarrierName.Size = new System.Drawing.Size(288, 20);
			this.textCarrierName.TabIndex = 93;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(12, 44);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(96, 16);
			this.label6.TabIndex = 37;
			this.label6.Text = "Payment Date";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(236, 21);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(109, 16);
			this.label7.TabIndex = 94;
			this.label7.Text = "Carrier Name";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(346, 40);
			this.textNote.MaxLength = 255;
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.ReadOnly = true;
			this.textNote.Size = new System.Drawing.Size(288, 20);
			this.textNote.TabIndex = 3;
			// 
			// textCheckNum
			// 
			this.textCheckNum.Location = new System.Drawing.Point(346, 61);
			this.textCheckNum.MaxLength = 25;
			this.textCheckNum.Name = "textCheckNum";
			this.textCheckNum.ReadOnly = true;
			this.textCheckNum.Size = new System.Drawing.Size(100, 20);
			this.textCheckNum.TabIndex = 1;
			// 
			// textBankBranch
			// 
			this.textBankBranch.Location = new System.Drawing.Point(346, 82);
			this.textBankBranch.MaxLength = 25;
			this.textBankBranch.Name = "textBankBranch";
			this.textBankBranch.ReadOnly = true;
			this.textBankBranch.Size = new System.Drawing.Size(100, 20);
			this.textBankBranch.TabIndex = 2;
			// 
			// textAmount
			// 
			this.textAmount.Location = new System.Drawing.Point(110, 82);
			this.textAmount.MaxVal = 100000000D;
			this.textAmount.MinVal = -100000000D;
			this.textAmount.Name = "textAmount";
			this.textAmount.ReadOnly = true;
			this.textAmount.Size = new System.Drawing.Size(68, 20);
			this.textAmount.TabIndex = 0;
			this.textAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(110, 40);
			this.textDate.Name = "textDate";
			this.textDate.ReadOnly = true;
			this.textDate.Size = new System.Drawing.Size(68, 20);
			this.textDate.TabIndex = 3;
			this.textDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// gridAttached
			// 
			this.gridAttached.Location = new System.Drawing.Point(230, 125);
			this.gridAttached.Name = "gridAttached";
			this.gridAttached.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridAttached.Size = new System.Drawing.Size(732, 209);
			this.gridAttached.TabIndex = 95;
			this.gridAttached.Title = "Attached to this Payment";
			this.gridAttached.TranslationName = "TableClaimPaySplits";
			this.gridAttached.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridAttached_CellDoubleClick);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(13, 646);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(79, 24);
			this.butDelete.TabIndex = 52;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(886, 646);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "Cancel";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(772, 347);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(92, 16);
			this.label8.TabIndex = 36;
			this.label8.Text = "Total Payments";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butViewEra
			// 
			this.butViewEra.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butViewEra.Location = new System.Drawing.Point(67, 606);
			this.butViewEra.Name = "butViewEra";
			this.butViewEra.Size = new System.Drawing.Size(72, 24);
			this.butViewEra.TabIndex = 120;
			this.butViewEra.Text = "View ERA";
			this.butViewEra.Click += new System.EventHandler(this.butViewEra_Click);
			// 
			// FormClaimPayBatch
			// 
			this.ClientSize = new System.Drawing.Size(974, 676);
			this.Controls.Add(this.butViewEra);
			this.Controls.Add(this.groupFilters);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.textTotal);
			this.Controls.Add(this.butAttach);
			this.Controls.Add(this.textEobIsScanned);
			this.Controls.Add(this.butView);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelInstruct2);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.labelInstruct1);
			this.Controls.Add(this.butDetach);
			this.Controls.Add(this.gridOut);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridAttached);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.label8);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimizeBox = false;
			this.Name = "FormClaimPayBatch";
			this.Text = "Insurance Payment (EOB)";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClaimPayBatch_FormClosing);
			this.Load += new System.EventHandler(this.FormClaimPayEdit_Load);
			this.groupFilters.ResumeLayout(false);
			this.groupFilters.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		
		private OpenDental.ValidDouble textAmount;
		private OpenDental.ValidDate textDate;
		private System.Windows.Forms.TextBox textBankBranch;
		private System.Windows.Forms.TextBox textCheckNum;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.Label labelClinic;
		private System.Windows.Forms.TextBox textCarrierName;
		private System.Windows.Forms.Label label7;
		private OpenDental.UI.GridOD gridAttached;
		private ValidDate textDateIssued;
		private Label labelDateIssued;
		private TextBox textClinic;
		private GroupBox groupBox1;
		private UI.Button butClaimPayEdit;
		private OpenDental.UI.GridOD gridOut;
		private UI.Button butDetach;
		private ValidDouble textTotal;
		private Label label8;
		private Label labelInstruct1;
		private UI.Button butDown;
		private UI.Button butUp;
		private Label labelInstruct2;
		private ContextMenu menuRightAttached;
		private MenuItem menuItemGotoAccount;
		private ContextMenu menuRightOut;
		private MenuItem menuItemGotoOut;
		private UI.Button butOK;
		private Label label1;
		private UI.Button butView;
		private TextBox textEobIsScanned;
		private UI.Button butAttach;
		private TextBox textCarrier;
		private Label label9;
		private UI.Button butRefresh;
		private Label label10;
		private TextBox textPayType;
		private TextBox textClaimID;
		private TextBox textName;
		private Label label11;
		private Label label12;
		private GroupBox groupFilters;
		private Label label13;
		private TextBox textPayGroup;
		private UI.Button butCarrierPick;
		private UI.Button butViewEra;
	}
}
