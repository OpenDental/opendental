using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormApptViewEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptViewEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.labelOps = new System.Windows.Forms.Label();
			this.listOps = new OpenDental.UI.ListBoxOD();
			this.listProv = new OpenDental.UI.ListBoxOD();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.butLeft = new OpenDental.UI.Button();
			this.butRight = new OpenDental.UI.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.textRowsPerIncr = new System.Windows.Forms.TextBox();
			this.checkOnlyScheduledProvs = new System.Windows.Forms.CheckBox();
			this.textBeforeTime = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelAfterTime = new System.Windows.Forms.Label();
			this.textAfterTime = new System.Windows.Forms.TextBox();
			this.labelBeforeTime = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.listStackLR = new OpenDental.UI.ListBoxOD();
			this.label4 = new System.Windows.Forms.Label();
			this.listStackUR = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.gridLR = new OpenDental.UI.GridOD();
			this.gridUR = new OpenDental.UI.GridOD();
			this.gridMain = new OpenDental.UI.GridOD();
			this.label8 = new System.Windows.Forms.Label();
			this.gridAvailable = new OpenDental.UI.GridOD();
			this.gridApptFieldDefs = new OpenDental.UI.GridOD();
			this.gridPatFieldDefs = new OpenDental.UI.GridOD();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.textScrollTime = new System.Windows.Forms.TextBox();
			this.labelStartTime = new System.Windows.Forms.Label();
			this.checkDynamicScroll = new System.Windows.Forms.CheckBox();
			this.checkApptBubblesDisabled = new System.Windows.Forms.CheckBox();
			this.textWidthOpMinimum = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.listWaitingRmNameFormat = new OpenDental.UI.ListBoxOD();
			this.label7 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(752, 662);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(652, 662);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(13, 662);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(87, 24);
			this.butDelete.TabIndex = 38;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// labelOps
			// 
			this.labelOps.Location = new System.Drawing.Point(32, 160);
			this.labelOps.Name = "labelOps";
			this.labelOps.Size = new System.Drawing.Size(182, 16);
			this.labelOps.TabIndex = 39;
			this.labelOps.Text = "View Operatories";
			this.labelOps.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listOps
			// 
			this.listOps.Location = new System.Drawing.Point(32, 177);
			this.listOps.Name = "listOps";
			this.listOps.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listOps.Size = new System.Drawing.Size(120, 225);
			this.listOps.TabIndex = 40;
			// 
			// listProv
			// 
			this.listProv.Location = new System.Drawing.Point(32, 426);
			this.listProv.Name = "listProv";
			this.listProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProv.Size = new System.Drawing.Size(120, 225);
			this.listProv.TabIndex = 42;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(32, 405);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(128, 18);
			this.label2.TabIndex = 41;
			this.label2.Text = "View Provider Bars";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(19, 4);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(217, 20);
			this.label3.TabIndex = 43;
			this.label3.Text = "Description";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(236, 4);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(160, 20);
			this.textDescription.TabIndex = 44;
			// 
			// butDown
			// 
			this.butDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(297, 497);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(71, 24);
			this.butDown.TabIndex = 50;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butUp
			// 
			this.butUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(219, 497);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(71, 24);
			this.butUp.TabIndex = 51;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butLeft
			// 
			this.butLeft.AdjustImageLocation = new System.Drawing.Point(-1, 0);
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(389, 328);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(35, 26);
			this.butLeft.TabIndex = 52;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(389, 294);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(35, 26);
			this.butRight.TabIndex = 53;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(19, 25);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(217, 20);
			this.label6.TabIndex = 54;
			this.label6.Text = "Rows per time increment (usually 1)";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRowsPerIncr
			// 
			this.textRowsPerIncr.Location = new System.Drawing.Point(236, 25);
			this.textRowsPerIncr.Name = "textRowsPerIncr";
			this.textRowsPerIncr.Size = new System.Drawing.Size(46, 20);
			this.textRowsPerIncr.TabIndex = 55;
			this.textRowsPerIncr.Validating += new System.ComponentModel.CancelEventHandler(this.textRowsPerIncr_Validating);
			// 
			// checkOnlyScheduledProvs
			// 
			this.checkOnlyScheduledProvs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOnlyScheduledProvs.Location = new System.Drawing.Point(2, 17);
			this.checkOnlyScheduledProvs.Name = "checkOnlyScheduledProvs";
			this.checkOnlyScheduledProvs.Size = new System.Drawing.Size(208, 20);
			this.checkOnlyScheduledProvs.TabIndex = 56;
			this.checkOnlyScheduledProvs.Text = "Only show ops for scheduled provs";
			this.checkOnlyScheduledProvs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOnlyScheduledProvs.UseVisualStyleBackColor = true;
			this.checkOnlyScheduledProvs.Click += new System.EventHandler(this.checkOnlyScheduledProvs_Click);
			// 
			// textBeforeTime
			// 
			this.textBeforeTime.Location = new System.Drawing.Point(195, 38);
			this.textBeforeTime.Name = "textBeforeTime";
			this.textBeforeTime.Size = new System.Drawing.Size(56, 20);
			this.textBeforeTime.TabIndex = 57;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.labelAfterTime);
			this.groupBox1.Controls.Add(this.textAfterTime);
			this.groupBox1.Controls.Add(this.labelBeforeTime);
			this.groupBox1.Controls.Add(this.textBeforeTime);
			this.groupBox1.Controls.Add(this.checkOnlyScheduledProvs);
			this.groupBox1.Location = new System.Drawing.Point(578, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(259, 93);
			this.groupBox1.TabIndex = 58;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Display Filter";
			// 
			// labelAfterTime
			// 
			this.labelAfterTime.Location = new System.Drawing.Point(69, 62);
			this.labelAfterTime.Name = "labelAfterTime";
			this.labelAfterTime.Size = new System.Drawing.Size(125, 17);
			this.labelAfterTime.TabIndex = 60;
			this.labelAfterTime.Text = "Only if after time";
			this.labelAfterTime.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textAfterTime
			// 
			this.textAfterTime.Location = new System.Drawing.Point(195, 62);
			this.textAfterTime.Name = "textAfterTime";
			this.textAfterTime.Size = new System.Drawing.Size(56, 20);
			this.textAfterTime.TabIndex = 59;
			// 
			// labelBeforeTime
			// 
			this.labelBeforeTime.Location = new System.Drawing.Point(69, 38);
			this.labelBeforeTime.Name = "labelBeforeTime";
			this.labelBeforeTime.Size = new System.Drawing.Size(125, 17);
			this.labelBeforeTime.TabIndex = 58;
			this.labelBeforeTime.Text = "Only if before time";
			this.labelBeforeTime.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.listStackLR);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.listStackUR);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.gridLR);
			this.groupBox2.Controls.Add(this.gridUR);
			this.groupBox2.Controls.Add(this.gridMain);
			this.groupBox2.Controls.Add(this.butUp);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.butDown);
			this.groupBox2.Location = new System.Drawing.Point(430, 122);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(397, 529);
			this.groupBox2.TabIndex = 59;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Rows Displayed (double click to edit or to move to another corner)";
			// 
			// listStackLR
			// 
			this.listStackLR.Location = new System.Drawing.Point(192, 283);
			this.listStackLR.Name = "listStackLR";
			this.listStackLR.Size = new System.Drawing.Size(175, 30);
			this.listStackLR.TabIndex = 66;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(190, 264);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(175, 17);
			this.label4.TabIndex = 65;
			this.label4.Text = "LR stack behavior";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listStackUR
			// 
			this.listStackUR.Location = new System.Drawing.Point(192, 213);
			this.listStackUR.Name = "listStackUR";
			this.listStackUR.Size = new System.Drawing.Size(175, 30);
			this.listStackUR.TabIndex = 64;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(190, 194);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(175, 17);
			this.label1.TabIndex = 63;
			this.label1.Text = "UR stack behavior";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// gridLR
			// 
			this.gridLR.Location = new System.Drawing.Point(192, 317);
			this.gridLR.Name = "gridLR";
			this.gridLR.Size = new System.Drawing.Size(175, 174);
			this.gridLR.TabIndex = 62;
			this.gridLR.Title = "Lower Right Corner";
			this.gridLR.TranslationName = "TableLowerRight";
			this.gridLR.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridLR_CellDoubleClick);
			this.gridLR.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridLR_CellClick);
			// 
			// gridUR
			// 
			this.gridUR.Location = new System.Drawing.Point(192, 18);
			this.gridUR.Name = "gridUR";
			this.gridUR.Size = new System.Drawing.Size(175, 174);
			this.gridUR.TabIndex = 61;
			this.gridUR.Title = "Upper Right Corner";
			this.gridUR.TranslationName = "TableUpperRight";
			this.gridUR.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridUR_CellDoubleClick);
			this.gridUR.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridUR_CellClick);
			// 
			// gridMain
			// 
			this.gridMain.Location = new System.Drawing.Point(11, 18);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(175, 473);
			this.gridMain.TabIndex = 60;
			this.gridMain.Title = "Main List";
			this.gridMain.TranslationName = "TableMainList";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label8.Location = new System.Drawing.Point(11, 500);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(209, 17);
			this.label8.TabIndex = 59;
			this.label8.Text = "Move any item within its own list:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridAvailable
			// 
			this.gridAvailable.Location = new System.Drawing.Point(207, 177);
			this.gridAvailable.Name = "gridAvailable";
			this.gridAvailable.Size = new System.Drawing.Size(175, 255);
			this.gridAvailable.TabIndex = 61;
			this.gridAvailable.Title = "Available Rows";
			this.gridAvailable.TranslationName = "TableAvailableRows";
			this.gridAvailable.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridAvailable_CellClick);
			// 
			// gridApptFieldDefs
			// 
			this.gridApptFieldDefs.Location = new System.Drawing.Point(207, 435);
			this.gridApptFieldDefs.Name = "gridApptFieldDefs";
			this.gridApptFieldDefs.Size = new System.Drawing.Size(175, 106);
			this.gridApptFieldDefs.TabIndex = 62;
			this.gridApptFieldDefs.Title = "Appt Field Defs";
			this.gridApptFieldDefs.TranslationName = "TableApptFieldDefs";
			this.gridApptFieldDefs.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridApptFieldDefs_CellClick);
			// 
			// gridPatFieldDefs
			// 
			this.gridPatFieldDefs.Location = new System.Drawing.Point(207, 544);
			this.gridPatFieldDefs.Name = "gridPatFieldDefs";
			this.gridPatFieldDefs.Size = new System.Drawing.Size(175, 106);
			this.gridPatFieldDefs.TabIndex = 63;
			this.gridPatFieldDefs.Title = "Patient Field Defs";
			this.gridPatFieldDefs.TranslationName = "TablePatFieldDefs";
			this.gridPatFieldDefs.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPatFieldDefs_CellClick);
			// 
			// comboClinic
			// 
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(196, 125);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(200, 21);
			this.comboClinic.TabIndex = 133;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// textScrollTime
			// 
			this.textScrollTime.Location = new System.Drawing.Point(236, 67);
			this.textScrollTime.Name = "textScrollTime";
			this.textScrollTime.Size = new System.Drawing.Size(56, 20);
			this.textScrollTime.TabIndex = 134;
			// 
			// labelStartTime
			// 
			this.labelStartTime.Location = new System.Drawing.Point(19, 67);
			this.labelStartTime.Name = "labelStartTime";
			this.labelStartTime.Size = new System.Drawing.Size(217, 20);
			this.labelStartTime.TabIndex = 135;
			this.labelStartTime.Text = "View start time on load";
			this.labelStartTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkDynamicScroll
			// 
			this.checkDynamicScroll.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDynamicScroll.Location = new System.Drawing.Point(20, 90);
			this.checkDynamicScroll.Name = "checkDynamicScroll";
			this.checkDynamicScroll.Size = new System.Drawing.Size(230, 17);
			this.checkDynamicScroll.TabIndex = 61;
			this.checkDynamicScroll.Text = "Dynamic start time based on schedule";
			this.checkDynamicScroll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDynamicScroll.UseVisualStyleBackColor = true;
			// 
			// checkApptBubblesDisabled
			// 
			this.checkApptBubblesDisabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptBubblesDisabled.Location = new System.Drawing.Point(20, 107);
			this.checkApptBubblesDisabled.Name = "checkApptBubblesDisabled";
			this.checkApptBubblesDisabled.Size = new System.Drawing.Size(230, 17);
			this.checkApptBubblesDisabled.TabIndex = 136;
			this.checkApptBubblesDisabled.Text = "Disable appointment bubbles";
			this.checkApptBubblesDisabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptBubblesDisabled.UseVisualStyleBackColor = true;
			// 
			// textWidthOpMinimum
			// 
			this.textWidthOpMinimum.Location = new System.Drawing.Point(236, 46);
			this.textWidthOpMinimum.Name = "textWidthOpMinimum";
			this.textWidthOpMinimum.Size = new System.Drawing.Size(46, 20);
			this.textWidthOpMinimum.TabIndex = 138;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(1, 46);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(235, 20);
			this.label5.TabIndex = 137;
			this.label5.Text = "Minimum Op width (default 0) (turns on hscroll)";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listWaitingRmNameFormat
			// 
			this.listWaitingRmNameFormat.Location = new System.Drawing.Point(431, 21);
			this.listWaitingRmNameFormat.Name = "listWaitingRmNameFormat";
			this.listWaitingRmNameFormat.Size = new System.Drawing.Size(120, 43);
			this.listWaitingRmNameFormat.TabIndex = 139;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(429, 1);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(150, 18);
			this.label7.TabIndex = 140;
			this.label7.Text = "Waiting Room Name Format";
			this.label7.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormApptViewEdit
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(852, 695);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.listWaitingRmNameFormat);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.textWidthOpMinimum);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.checkApptBubblesDisabled);
			this.Controls.Add(this.checkDynamicScroll);
			this.Controls.Add(this.labelStartTime);
			this.Controls.Add(this.textScrollTime);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.gridPatFieldDefs);
			this.Controls.Add(this.gridApptFieldDefs);
			this.Controls.Add(this.gridAvailable);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.textRowsPerIncr);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.butRight);
			this.Controls.Add(this.butLeft);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.listProv);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listOps);
			this.Controls.Add(this.labelOps);
			this.Controls.Add(this.butDelete);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormApptViewEdit";
			this.ShowInTaskbar = false;
			this.Text = "Appointment View Edit";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormApptViewEdit_Closing);
			this.Load += new System.EventHandler(this.FormApptViewEdit_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.Label labelOps;
		private OpenDental.UI.ListBoxOD listOps;
		private OpenDental.UI.ListBoxOD listProv;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textDescription;
		private OpenDental.UI.Button butDown;
		private OpenDental.UI.Button butUp;
		private OpenDental.UI.Button butLeft;
		private OpenDental.UI.Button butRight;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textRowsPerIncr;
		private CheckBox checkOnlyScheduledProvs;
		private TextBox textBeforeTime;
		private GroupBox groupBox1;
		private Label labelBeforeTime;
		private Label labelAfterTime;
		private TextBox textAfterTime;
		private GroupBox groupBox2;
		private Label label8;
		private OpenDental.UI.GridOD gridLR;
		private OpenDental.UI.GridOD gridUR;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.GridOD gridAvailable;
		private OpenDental.UI.ListBoxOD listStackLR;
		private Label label4;
		private OpenDental.UI.ListBoxOD listStackUR;
		private Label label1;
		private OpenDental.UI.GridOD gridApptFieldDefs;
		private OpenDental.UI.GridOD gridPatFieldDefs;
		private OpenDental.UI.ComboBoxClinicPicker comboClinic;
		private TextBox textScrollTime;
		private Label labelStartTime;
		private CheckBox checkDynamicScroll;
		private CheckBox checkApptBubblesDisabled;
		private TextBox textWidthOpMinimum;
		private Label label5;
		private UI.ListBoxOD listWaitingRmNameFormat;
		private Label label7;
	}
}
