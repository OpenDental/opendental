using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTimeCard {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTimeCard));
			this.textTotal = new System.Windows.Forms.TextBox();
			this.labelRegularTime = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.timerUpdateBreak = new System.Windows.Forms.Timer(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelNote = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.textDatePaycheck = new System.Windows.Forms.TextBox();
			this.textDateStop = new System.Windows.Forms.TextBox();
			this.textDateStart = new System.Windows.Forms.TextBox();
			this.butRight = new OpenDental.UI.Button();
			this.butLeft = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.labelOvertime = new System.Windows.Forms.Label();
			this.textOvertime = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioBreaks = new System.Windows.Forms.RadioButton();
			this.radioTimeCard = new System.Windows.Forms.RadioButton();
			this.textOvertime2 = new System.Windows.Forms.TextBox();
			this.textTotal2 = new System.Windows.Forms.TextBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.textRateTwo2 = new System.Windows.Forms.TextBox();
			this.labelRateTwo = new System.Windows.Forms.Label();
			this.textRateTwo = new System.Windows.Forms.TextBox();
			this.butCalcDaily = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.butCalcWeekOT = new OpenDental.UI.Button();
			this.butAdj = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.groupEmployee = new System.Windows.Forms.GroupBox();
			this.butPrevEmp = new OpenDental.UI.Button();
			this.butNextEmp = new OpenDental.UI.Button();
			this.labelPTO = new System.Windows.Forms.Label();
			this.textPTO = new System.Windows.Forms.TextBox();
			this.textPTO2 = new System.Windows.Forms.TextBox();
			this.textUnpaidProtectedLeave = new System.Windows.Forms.TextBox();
			this.textUnpaidProtectedLeave2 = new System.Windows.Forms.TextBox();
			this.labelUnpaidProtectedLeave = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupEmployee.SuspendLayout();
			this.SuspendLayout();
			// 
			// textTotal
			// 
			this.textTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textTotal.Location = new System.Drawing.Point(491, 623);
			this.textTotal.Name = "textTotal";
			this.textTotal.Size = new System.Drawing.Size(66, 20);
			this.textTotal.TabIndex = 3;
			this.textTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelRegularTime
			// 
			this.labelRegularTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelRegularTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelRegularTime.Location = new System.Drawing.Point(385, 624);
			this.labelRegularTime.Name = "labelRegularTime";
			this.labelRegularTime.Size = new System.Drawing.Size(100, 17);
			this.labelRegularTime.TabIndex = 4;
			this.labelRegularTime.Text = "Regular Time";
			this.labelRegularTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(107, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(73, 18);
			this.label2.TabIndex = 6;
			this.label2.Text = "Start Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(110, 28);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(70, 18);
			this.label3.TabIndex = 8;
			this.label3.Text = "End Date";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// timerUpdateBreak
			// 
			this.timerUpdateBreak.Enabled = true;
			this.timerUpdateBreak.Interval = 1000;
			this.timerUpdateBreak.Tick += new System.EventHandler(this.timerUpdateBreak_Tick);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.labelNote);
			this.groupBox1.Controls.Add(this.textNote);
			this.groupBox1.Controls.Add(this.textDatePaycheck);
			this.groupBox1.Controls.Add(this.textDateStop);
			this.groupBox1.Controls.Add(this.textDateStart);
			this.groupBox1.Controls.Add(this.butRight);
			this.groupBox1.Controls.Add(this.butLeft);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(18, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(611, 71);
			this.groupBox1.TabIndex = 14;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Pay Period";
			// 
			// labelNote
			// 
			this.labelNote.Location = new System.Drawing.Point(284, 28);
			this.labelNote.Name = "labelNote";
			this.labelNote.Size = new System.Drawing.Size(46, 18);
			this.labelNote.TabIndex = 16;
			this.labelNote.Text = "Note";
			this.labelNote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(331, 28);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(274, 40);
			this.textNote.TabIndex = 15;
			// 
			// textDatePaycheck
			// 
			this.textDatePaycheck.Location = new System.Drawing.Point(505, 8);
			this.textDatePaycheck.Name = "textDatePaycheck";
			this.textDatePaycheck.ReadOnly = true;
			this.textDatePaycheck.Size = new System.Drawing.Size(100, 20);
			this.textDatePaycheck.TabIndex = 14;
			// 
			// textDateStop
			// 
			this.textDateStop.Location = new System.Drawing.Point(181, 28);
			this.textDateStop.Name = "textDateStop";
			this.textDateStop.ReadOnly = true;
			this.textDateStop.Size = new System.Drawing.Size(100, 20);
			this.textDateStop.TabIndex = 13;
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(181, 8);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.ReadOnly = true;
			this.textDateStart.Size = new System.Drawing.Size(100, 20);
			this.textDateStart.TabIndex = 12;
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(63, 18);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(39, 24);
			this.butRight.TabIndex = 11;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butLeft
			// 
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(13, 18);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(39, 24);
			this.butLeft.TabIndex = 10;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(387, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(117, 18);
			this.label4.TabIndex = 9;
			this.label4.Text = "Paycheck Date";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelOvertime
			// 
			this.labelOvertime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelOvertime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelOvertime.Location = new System.Drawing.Point(385, 644);
			this.labelOvertime.Name = "labelOvertime";
			this.labelOvertime.Size = new System.Drawing.Size(100, 17);
			this.labelOvertime.TabIndex = 17;
			this.labelOvertime.Text = "Overtime";
			this.labelOvertime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textOvertime
			// 
			this.textOvertime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textOvertime.Location = new System.Drawing.Point(491, 643);
			this.textOvertime.Name = "textOvertime";
			this.textOvertime.Size = new System.Drawing.Size(66, 20);
			this.textOvertime.TabIndex = 16;
			this.textOvertime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.radioBreaks);
			this.groupBox2.Controls.Add(this.radioTimeCard);
			this.groupBox2.Location = new System.Drawing.Point(747, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(122, 71);
			this.groupBox2.TabIndex = 20;
			this.groupBox2.TabStop = false;
			// 
			// radioBreaks
			// 
			this.radioBreaks.Location = new System.Drawing.Point(14, 27);
			this.radioBreaks.Name = "radioBreaks";
			this.radioBreaks.Size = new System.Drawing.Size(97, 19);
			this.radioBreaks.TabIndex = 1;
			this.radioBreaks.Text = "Breaks";
			this.radioBreaks.UseVisualStyleBackColor = true;
			this.radioBreaks.Click += new System.EventHandler(this.radioBreaks_Click);
			// 
			// radioTimeCard
			// 
			this.radioTimeCard.Checked = true;
			this.radioTimeCard.Location = new System.Drawing.Point(14, 10);
			this.radioTimeCard.Name = "radioTimeCard";
			this.radioTimeCard.Size = new System.Drawing.Size(97, 19);
			this.radioTimeCard.TabIndex = 0;
			this.radioTimeCard.TabStop = true;
			this.radioTimeCard.Text = "Time Card";
			this.radioTimeCard.UseVisualStyleBackColor = true;
			this.radioTimeCard.Click += new System.EventHandler(this.radioTimeCard_Click);
			// 
			// textOvertime2
			// 
			this.textOvertime2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textOvertime2.Location = new System.Drawing.Point(563, 643);
			this.textOvertime2.Name = "textOvertime2";
			this.textOvertime2.Size = new System.Drawing.Size(66, 20);
			this.textOvertime2.TabIndex = 22;
			this.textOvertime2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textTotal2
			// 
			this.textTotal2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textTotal2.Location = new System.Drawing.Point(563, 623);
			this.textTotal2.Name = "textTotal2";
			this.textTotal2.Size = new System.Drawing.Size(66, 20);
			this.textTotal2.TabIndex = 21;
			this.textTotal2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(18, 80);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(851, 540);
			this.gridMain.TabIndex = 13;
			this.gridMain.Title = "Time Card";
			this.gridMain.TranslationName = "TableTimeCard";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// textRateTwo2
			// 
			this.textRateTwo2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textRateTwo2.Location = new System.Drawing.Point(563, 663);
			this.textRateTwo2.Name = "textRateTwo2";
			this.textRateTwo2.Size = new System.Drawing.Size(66, 20);
			this.textRateTwo2.TabIndex = 26;
			this.textRateTwo2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelRateTwo
			// 
			this.labelRateTwo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelRateTwo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelRateTwo.Location = new System.Drawing.Point(385, 664);
			this.labelRateTwo.Name = "labelRateTwo";
			this.labelRateTwo.Size = new System.Drawing.Size(100, 17);
			this.labelRateTwo.TabIndex = 25;
			this.labelRateTwo.Text = "Rate2";
			this.labelRateTwo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRateTwo
			// 
			this.textRateTwo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textRateTwo.Location = new System.Drawing.Point(491, 663);
			this.textRateTwo.Name = "textRateTwo";
			this.textRateTwo.Size = new System.Drawing.Size(66, 20);
			this.textRateTwo.TabIndex = 24;
			this.textRateTwo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// butCalcDaily
			// 
			this.butCalcDaily.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCalcDaily.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCalcDaily.Location = new System.Drawing.Point(139, 681);
			this.butCalcDaily.Name = "butCalcDaily";
			this.butCalcDaily.Size = new System.Drawing.Size(78, 24);
			this.butCalcDaily.TabIndex = 23;
			this.butCalcDaily.Text = "Calc Daily";
			this.butCalcDaily.Click += new System.EventHandler(this.butCalcDaily_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(691, 681);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(86, 24);
			this.butPrint.TabIndex = 19;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butCalcWeekOT
			// 
			this.butCalcWeekOT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCalcWeekOT.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCalcWeekOT.Location = new System.Drawing.Point(223, 681);
			this.butCalcWeekOT.Name = "butCalcWeekOT";
			this.butCalcWeekOT.Size = new System.Drawing.Size(90, 24);
			this.butCalcWeekOT.TabIndex = 18;
			this.butCalcWeekOT.Text = "Calc Week OT";
			this.butCalcWeekOT.Click += new System.EventHandler(this.butCalcWeekOT_Click);
			// 
			// butAdj
			// 
			this.butAdj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdj.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdj.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdj.Location = new System.Drawing.Point(18, 681);
			this.butAdj.Name = "butAdj";
			this.butAdj.Size = new System.Drawing.Size(115, 24);
			this.butAdj.TabIndex = 15;
			this.butAdj.Text = "Add Adjustment";
			this.butAdj.Click += new System.EventHandler(this.butAdj_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(794, 681);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// groupEmployee
			// 
			this.groupEmployee.Controls.Add(this.butPrevEmp);
			this.groupEmployee.Controls.Add(this.butNextEmp);
			this.groupEmployee.Location = new System.Drawing.Point(635, 3);
			this.groupEmployee.Name = "groupEmployee";
			this.groupEmployee.Size = new System.Drawing.Size(106, 71);
			this.groupEmployee.TabIndex = 121;
			this.groupEmployee.TabStop = false;
			this.groupEmployee.Text = "Employee";
			this.groupEmployee.Visible = false;
			// 
			// butPrevEmp
			// 
			this.butPrevEmp.Image = global::OpenDental.Properties.Resources.Left;
			this.butPrevEmp.Location = new System.Drawing.Point(8, 18);
			this.butPrevEmp.Name = "butPrevEmp";
			this.butPrevEmp.Size = new System.Drawing.Size(39, 24);
			this.butPrevEmp.TabIndex = 127;
			this.butPrevEmp.Click += new System.EventHandler(this.butBrowseEmp_Click);
			// 
			// butNextEmp
			// 
			this.butNextEmp.Image = global::OpenDental.Properties.Resources.Right;
			this.butNextEmp.Location = new System.Drawing.Point(59, 18);
			this.butNextEmp.Name = "butNextEmp";
			this.butNextEmp.Size = new System.Drawing.Size(39, 24);
			this.butNextEmp.TabIndex = 128;
			this.butNextEmp.Click += new System.EventHandler(this.butBrowseEmp_Click);
			// 
			// labelPTO
			// 
			this.labelPTO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelPTO.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPTO.Location = new System.Drawing.Point(385, 684);
			this.labelPTO.Name = "labelPTO";
			this.labelPTO.Size = new System.Drawing.Size(100, 17);
			this.labelPTO.TabIndex = 122;
			this.labelPTO.Text = "PTO";
			this.labelPTO.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPTO
			// 
			this.textPTO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textPTO.Location = new System.Drawing.Point(491, 683);
			this.textPTO.Name = "textPTO";
			this.textPTO.Size = new System.Drawing.Size(66, 20);
			this.textPTO.TabIndex = 123;
			this.textPTO.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textPTO2
			// 
			this.textPTO2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textPTO2.Location = new System.Drawing.Point(563, 683);
			this.textPTO2.Name = "textPTO2";
			this.textPTO2.Size = new System.Drawing.Size(66, 20);
			this.textPTO2.TabIndex = 124;
			this.textPTO2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textUnpaidProtectedLeave2
			// 
			this.textUnpaidProtectedLeave2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textUnpaidProtectedLeave2.Location = new System.Drawing.Point(563, 703);
			this.textUnpaidProtectedLeave2.Name = "textProtectedLeave2";
			this.textUnpaidProtectedLeave2.Size = new System.Drawing.Size(66, 20);
			this.textUnpaidProtectedLeave2.TabIndex = 127;
			this.textUnpaidProtectedLeave2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textUnpaidProtectedLeave
			// 
			this.textUnpaidProtectedLeave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textUnpaidProtectedLeave.Location = new System.Drawing.Point(491, 703);
			this.textUnpaidProtectedLeave.Name = "textProtectedLeave";
			this.textUnpaidProtectedLeave.Size = new System.Drawing.Size(66, 20);
			this.textUnpaidProtectedLeave.TabIndex = 126;
			this.textUnpaidProtectedLeave.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelUnpaidProtectedLeave
			// 
			this.labelUnpaidProtectedLeave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelUnpaidProtectedLeave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelUnpaidProtectedLeave.Location = new System.Drawing.Point(334, 704);
			this.labelUnpaidProtectedLeave.Name = "labelProtectedLeave";
			this.labelUnpaidProtectedLeave.Size = new System.Drawing.Size(151, 17);
			this.labelUnpaidProtectedLeave.TabIndex = 125;
			this.labelUnpaidProtectedLeave.Text = "Protected Leave";
			this.labelUnpaidProtectedLeave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormTimeCard
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(891, 723);
			this.Controls.Add(this.textUnpaidProtectedLeave2);
			this.Controls.Add(this.textUnpaidProtectedLeave);
			this.Controls.Add(this.labelUnpaidProtectedLeave);
			this.Controls.Add(this.textPTO2);
			this.Controls.Add(this.textPTO);
			this.Controls.Add(this.labelPTO);
			this.Controls.Add(this.groupEmployee);
			this.Controls.Add(this.textRateTwo2);
			this.Controls.Add(this.labelRateTwo);
			this.Controls.Add(this.textRateTwo);
			this.Controls.Add(this.butCalcDaily);
			this.Controls.Add(this.textOvertime2);
			this.Controls.Add(this.textTotal2);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butCalcWeekOT);
			this.Controls.Add(this.labelOvertime);
			this.Controls.Add(this.textOvertime);
			this.Controls.Add(this.butAdj);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.labelRegularTime);
			this.Controls.Add(this.textTotal);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTimeCard";
			this.ShowInTaskbar = false;
			this.Text = "Time Card";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTimeCard_FormClosing);
			this.Load += new System.EventHandler(this.FormTimeCard_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupEmployee.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label labelRegularTime;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.TextBox textTotal;
		private System.Windows.Forms.Timer timerUpdateBreak;
		private OpenDental.UI.GridOD gridMain;
		private GroupBox groupBox1;
		private OpenDental.UI.Button butRight;
		private OpenDental.UI.Button butLeft;
		private Label label4;
		private TextBox textDateStart;
		private TextBox textDatePaycheck;
		private TextBox textDateStop;
		private OpenDental.UI.Button butAdj;
		private Label labelOvertime;
		private TextBox textOvertime;
		private OpenDental.UI.Button butCalcWeekOT;
		private OpenDental.UI.Button butPrint;
		private GroupBox groupBox2;
		private RadioButton radioBreaks;
		private RadioButton radioTimeCard;
		private TextBox textOvertime2;
		private TextBox textTotal2;
		private OpenDental.UI.Button butCalcDaily;
		private TextBox textRateTwo2;
		private Label labelRateTwo;
		private TextBox textRateTwo;
		private GroupBox groupEmployee;
		private UI.Button butPrevEmp;
		private UI.Button butNextEmp;
		private Label labelNote;
		private TextBox textNote;
		private Label labelPTO;
		private TextBox textPTO;
		private TextBox textPTO2;
		private TextBox textUnpaidProtectedLeave;
		private TextBox textUnpaidProtectedLeave2;
		private Label labelUnpaidProtectedLeave;
	}
}
