using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormClockEventEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClockEventEdit));
			this.textTimeEntered1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textTimeDisplayed1 = new System.Windows.Forms.TextBox();
			this.listStatus = new OpenDental.UI.ListBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butNow1 = new OpenDental.UI.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textTimeEntered2 = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.butClear = new OpenDental.UI.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.butNow2 = new OpenDental.UI.Button();
			this.textTimeDisplayed2 = new System.Windows.Forms.TextBox();
			this.textOTimeHours = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textClockedTime = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textRegTime = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.groupTimeSpans = new System.Windows.Forms.GroupBox();
			this.textOTimeAuto = new System.Windows.Forms.TextBox();
			this.textAdjust = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.textAdjustAuto = new System.Windows.Forms.TextBox();
			this.textRate2Auto = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.textRate2Hours = new System.Windows.Forms.TextBox();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label14 = new System.Windows.Forms.Label();
			this.textTotalHours = new System.Windows.Forms.TextBox();
			this.textRate1Auto = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.groupRate2 = new System.Windows.Forms.GroupBox();
			this.label18 = new System.Windows.Forms.Label();
			this.textNote = new OpenDental.ODtextBox();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupTimeSpans.SuspendLayout();
			this.groupRate2.SuspendLayout();
			this.SuspendLayout();
			// 
			// textTimeEntered1
			// 
			this.textTimeEntered1.Location = new System.Drawing.Point(101, 19);
			this.textTimeEntered1.Name = "textTimeEntered1";
			this.textTimeEntered1.ReadOnly = true;
			this.textTimeEntered1.Size = new System.Drawing.Size(156, 20);
			this.textTimeEntered1.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "Entered";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(93, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "Displayed";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTimeDisplayed1
			// 
			this.textTimeDisplayed1.Location = new System.Drawing.Point(101, 43);
			this.textTimeDisplayed1.Name = "textTimeDisplayed1";
			this.textTimeDisplayed1.Size = new System.Drawing.Size(156, 20);
			this.textTimeDisplayed1.TabIndex = 4;
			this.textTimeDisplayed1.TextChanged += new System.EventHandler(this.textTimeDisplayed1_TextChanged);
			// 
			// listStatus
			// 
			this.listStatus.Location = new System.Drawing.Point(179, 135);
			this.listStatus.Name = "listStatus";
			this.listStatus.Size = new System.Drawing.Size(120, 43);
			this.listStatus.TabIndex = 8;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(72, 135);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(105, 16);
			this.label3.TabIndex = 9;
			this.label3.Text = "Out Status";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(72, 326);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(105, 16);
			this.label4.TabIndex = 10;
			this.label4.Text = "Note";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textTimeEntered1);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.butNow1);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.textTimeDisplayed1);
			this.groupBox1.Location = new System.Drawing.Point(79, 29);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(267, 100);
			this.groupBox1.TabIndex = 13;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Clock In Date and Time";
			// 
			// butNow1
			// 
			this.butNow1.Location = new System.Drawing.Point(101, 69);
			this.butNow1.Name = "butNow1";
			this.butNow1.Size = new System.Drawing.Size(70, 24);
			this.butNow1.TabIndex = 17;
			this.butNow1.Text = "Now";
			this.butNow1.Click += new System.EventHandler(this.butNow1_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textTimeEntered2);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.butClear);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.butNow2);
			this.groupBox2.Controls.Add(this.textTimeDisplayed2);
			this.groupBox2.Location = new System.Drawing.Point(363, 29);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(267, 100);
			this.groupBox2.TabIndex = 14;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Clock Out Date and Time";
			// 
			// textTimeEntered2
			// 
			this.textTimeEntered2.Location = new System.Drawing.Point(101, 19);
			this.textTimeEntered2.Name = "textTimeEntered2";
			this.textTimeEntered2.ReadOnly = true;
			this.textTimeEntered2.Size = new System.Drawing.Size(156, 20);
			this.textTimeEntered2.TabIndex = 2;
			this.textTimeEntered2.TextChanged += new System.EventHandler(this.textTimeEntered2_TextChanged);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 21);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(94, 16);
			this.label5.TabIndex = 3;
			this.label5.Text = "Entered";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butClear
			// 
			this.butClear.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClear.Location = new System.Drawing.Point(177, 69);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(80, 24);
			this.butClear.TabIndex = 16;
			this.butClear.Text = "Clear";
			this.butClear.Click += new System.EventHandler(this.butClear_Click);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(6, 45);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(93, 16);
			this.label6.TabIndex = 5;
			this.label6.Text = "Displayed";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butNow2
			// 
			this.butNow2.Location = new System.Drawing.Point(101, 69);
			this.butNow2.Name = "butNow2";
			this.butNow2.Size = new System.Drawing.Size(70, 24);
			this.butNow2.TabIndex = 15;
			this.butNow2.Text = "Now";
			this.butNow2.Click += new System.EventHandler(this.butNow2_Click);
			// 
			// textTimeDisplayed2
			// 
			this.textTimeDisplayed2.Location = new System.Drawing.Point(101, 43);
			this.textTimeDisplayed2.Name = "textTimeDisplayed2";
			this.textTimeDisplayed2.Size = new System.Drawing.Size(156, 20);
			this.textTimeDisplayed2.TabIndex = 4;
			this.textTimeDisplayed2.TextChanged += new System.EventHandler(this.textTimeDisplayed2_TextChanged);
			// 
			// textOTimeHours
			// 
			this.textOTimeHours.Location = new System.Drawing.Point(176, 79);
			this.textOTimeHours.Name = "textOTimeHours";
			this.textOTimeHours.Size = new System.Drawing.Size(68, 20);
			this.textOTimeHours.TabIndex = 7;
			this.textOTimeHours.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textOTimeHours.TextChanged += new System.EventHandler(this.textOvertime_TextChanged);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 79);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(92, 18);
			this.label7.TabIndex = 10;
			this.label7.Text = "- Overtime";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textClockedTime
			// 
			this.textClockedTime.Location = new System.Drawing.Point(100, 34);
			this.textClockedTime.Name = "textClockedTime";
			this.textClockedTime.ReadOnly = true;
			this.textClockedTime.Size = new System.Drawing.Size(68, 20);
			this.textClockedTime.TabIndex = 1;
			this.textClockedTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(8, 34);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(92, 18);
			this.label8.TabIndex = 8;
			this.label8.Text = "Clocked Time";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRegTime
			// 
			this.textRegTime.Location = new System.Drawing.Point(100, 101);
			this.textRegTime.Name = "textRegTime";
			this.textRegTime.ReadOnly = true;
			this.textRegTime.Size = new System.Drawing.Size(68, 20);
			this.textRegTime.TabIndex = 4;
			this.textRegTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(8, 101);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(92, 18);
			this.label9.TabIndex = 11;
			this.label9.Text = "Regular Time";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupTimeSpans
			// 
			this.groupTimeSpans.Controls.Add(this.textOTimeAuto);
			this.groupTimeSpans.Controls.Add(this.textAdjust);
			this.groupTimeSpans.Controls.Add(this.label12);
			this.groupTimeSpans.Controls.Add(this.label11);
			this.groupTimeSpans.Controls.Add(this.label10);
			this.groupTimeSpans.Controls.Add(this.textAdjustAuto);
			this.groupTimeSpans.Controls.Add(this.label8);
			this.groupTimeSpans.Controls.Add(this.textRegTime);
			this.groupTimeSpans.Controls.Add(this.label7);
			this.groupTimeSpans.Controls.Add(this.label9);
			this.groupTimeSpans.Controls.Add(this.textOTimeHours);
			this.groupTimeSpans.Controls.Add(this.textClockedTime);
			this.groupTimeSpans.Location = new System.Drawing.Point(79, 181);
			this.groupTimeSpans.Name = "groupTimeSpans";
			this.groupTimeSpans.Size = new System.Drawing.Size(267, 134);
			this.groupTimeSpans.TabIndex = 30;
			this.groupTimeSpans.TabStop = false;
			this.groupTimeSpans.Text = "Time Spans";
			// 
			// textOTimeAuto
			// 
			this.textOTimeAuto.Location = new System.Drawing.Point(100, 79);
			this.textOTimeAuto.Name = "textOTimeAuto";
			this.textOTimeAuto.ReadOnly = true;
			this.textOTimeAuto.Size = new System.Drawing.Size(68, 20);
			this.textOTimeAuto.TabIndex = 3;
			this.textOTimeAuto.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textAdjust
			// 
			this.textAdjust.Location = new System.Drawing.Point(176, 56);
			this.textAdjust.Name = "textAdjust";
			this.textAdjust.Size = new System.Drawing.Size(68, 20);
			this.textAdjust.TabIndex = 6;
			this.textAdjust.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textAdjust.TextChanged += new System.EventHandler(this.textAdjust_TextChanged);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(176, 13);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(68, 18);
			this.label12.TabIndex = 5;
			this.label12.Text = "Override";
			this.label12.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(100, 13);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(68, 18);
			this.label11.TabIndex = 0;
			this.label11.Text = "Calculated";
			this.label11.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(8, 56);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(92, 18);
			this.label10.TabIndex = 9;
			this.label10.Text = "+ Adj";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAdjustAuto
			// 
			this.textAdjustAuto.Location = new System.Drawing.Point(100, 56);
			this.textAdjustAuto.Name = "textAdjustAuto";
			this.textAdjustAuto.ReadOnly = true;
			this.textAdjustAuto.Size = new System.Drawing.Size(68, 20);
			this.textAdjustAuto.TabIndex = 2;
			this.textAdjustAuto.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textRate2Auto
			// 
			this.textRate2Auto.Location = new System.Drawing.Point(100, 57);
			this.textRate2Auto.Name = "textRate2Auto";
			this.textRate2Auto.ReadOnly = true;
			this.textRate2Auto.Size = new System.Drawing.Size(68, 20);
			this.textRate2Auto.TabIndex = 31;
			this.textRate2Auto.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(8, 57);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(92, 18);
			this.label13.TabIndex = 33;
			this.label13.Text = "- Rate 2 Time";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRate2Hours
			// 
			this.textRate2Hours.Location = new System.Drawing.Point(176, 57);
			this.textRate2Hours.Name = "textRate2Hours";
			this.textRate2Hours.Size = new System.Drawing.Size(68, 20);
			this.textRate2Hours.TabIndex = 32;
			this.textRate2Hours.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textRate2Hours.TextChanged += new System.EventHandler(this.textRate2Hours_TextChanged);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(31, 443);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(84, 24);
			this.butDelete.TabIndex = 12;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(464, 442);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(555, 442);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(8, 24);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(92, 36);
			this.label14.TabIndex = 35;
			this.label14.Text = "Total Time\r\n(clock+adj)";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTotalHours
			// 
			this.textTotalHours.Location = new System.Drawing.Point(100, 34);
			this.textTotalHours.Name = "textTotalHours";
			this.textTotalHours.ReadOnly = true;
			this.textTotalHours.Size = new System.Drawing.Size(68, 20);
			this.textTotalHours.TabIndex = 34;
			this.textTotalHours.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textRate1Auto
			// 
			this.textRate1Auto.Location = new System.Drawing.Point(100, 80);
			this.textRate1Auto.Name = "textRate1Auto";
			this.textRate1Auto.ReadOnly = true;
			this.textRate1Auto.Size = new System.Drawing.Size(68, 20);
			this.textRate1Auto.TabIndex = 12;
			this.textRate1Auto.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(8, 80);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(92, 18);
			this.label15.TabIndex = 13;
			this.label15.Text = "Rate 1 Time";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(173, 13);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(68, 18);
			this.label17.TabIndex = 37;
			this.label17.Text = "Override";
			this.label17.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// groupRate2
			// 
			this.groupRate2.Controls.Add(this.label18);
			this.groupRate2.Controls.Add(this.label17);
			this.groupRate2.Controls.Add(this.textRate2Hours);
			this.groupRate2.Controls.Add(this.label13);
			this.groupRate2.Controls.Add(this.textRate1Auto);
			this.groupRate2.Controls.Add(this.textRate2Auto);
			this.groupRate2.Controls.Add(this.label15);
			this.groupRate2.Controls.Add(this.textTotalHours);
			this.groupRate2.Controls.Add(this.label14);
			this.groupRate2.Location = new System.Drawing.Point(352, 181);
			this.groupRate2.Name = "groupRate2";
			this.groupRate2.Size = new System.Drawing.Size(267, 134);
			this.groupRate2.TabIndex = 31;
			this.groupRate2.TabStop = false;
			this.groupRate2.Text = "Rate 2";
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(100, 13);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(68, 18);
			this.label18.TabIndex = 38;
			this.label18.Text = "Calculated";
			this.label18.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(179, 326);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.CommLog;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(317, 110);
			this.textNote.TabIndex = 32;
			this.textNote.Text = "";
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "Headquarters";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(143, 5);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(203, 21);
			this.comboClinic.TabIndex = 124;
			// 
			// FormClockEventEdit
			// 
			this.ClientSize = new System.Drawing.Size(669, 485);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.groupRate2);
			this.Controls.Add(this.groupTimeSpans);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.listStatus);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormClockEventEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Clock Event";
			this.Load += new System.EventHandler(this.FormClockEventEdit_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupTimeSpans.ResumeLayout(false);
			this.groupTimeSpans.PerformLayout();
			this.groupRate2.ResumeLayout(false);
			this.groupRate2.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.TextBox textTimeEntered1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.ListBoxOD listStatus;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textTimeDisplayed1;
		private OpenDental.UI.Button butDelete;
		private GroupBox groupBox1;
		private GroupBox groupBox2;
		private TextBox textTimeEntered2;
		private Label label5;
		private Label label6;
		private TextBox textTimeDisplayed2;
		private OpenDental.UI.Button butNow2;
		private OpenDental.UI.Button butClear;
		private OpenDental.UI.Button butNow1;
		private TextBox textOTimeHours;
		private Label label7;
		private TextBox textClockedTime;
		private Label label8;
		private TextBox textRegTime;
		private Label label9;
		private GroupBox groupTimeSpans;
		private TextBox textOTimeAuto;
		private TextBox textAdjust;
		private Label label12;
		private Label label11;
		private Label label10;
		private TextBox textAdjustAuto;
		private TextBox textRate2Auto;
		private Label label13;
		private TextBox textRate2Hours;
		private Label label14;
		private TextBox textTotalHours;
		private TextBox textRate1Auto;
		private Label label15;
		private Label label17;
		private GroupBox groupRate2;
		private Label label18;
		private ODtextBox textNote;
		private UI.ComboBoxClinicPicker comboClinic;
	}
}
