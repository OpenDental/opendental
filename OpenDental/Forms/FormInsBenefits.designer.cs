﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormInsBenefits {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInsBenefits));
			this.groupYear = new OpenDental.UI.GroupBox();
			this.checkCalendarYear = new OpenDental.UI.CheckBox();
			this.textMonth = new OpenDental.ValidNum();
			this.label30 = new System.Windows.Forms.Label();
			this.checkSimplified = new OpenDental.UI.CheckBox();
			this.groupCategories = new OpenDental.UI.GroupBox();
			this.labelWaitingPeriod = new System.Windows.Forms.Label();
			this.textWaitProsth = new OpenDental.ValidNum();
			this.textWaitCrowns = new OpenDental.ValidNum();
			this.labelMonths = new System.Windows.Forms.Label();
			this.textWaitOralSurg = new OpenDental.ValidNum();
			this.textWaitPerio = new OpenDental.ValidNum();
			this.textWaitEndo = new OpenDental.ValidNum();
			this.textWaitRestorative = new OpenDental.ValidNum();
			this.textDeductDiagFam = new OpenDental.ValidDouble();
			this.textDeductXrayFam = new OpenDental.ValidDouble();
			this.textDeductDiag = new OpenDental.ValidDouble();
			this.label3 = new System.Windows.Forms.Label();
			this.textDeductXray = new OpenDental.ValidDouble();
			this.label29 = new System.Windows.Forms.Label();
			this.label27 = new System.Windows.Forms.Label();
			this.label26 = new System.Windows.Forms.Label();
			this.textDeductPreventFam = new OpenDental.ValidDouble();
			this.textXray = new OpenDental.ValidNum();
			this.label25 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.panel3 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.textStand4 = new OpenDental.ValidNum();
			this.textAccident = new OpenDental.ValidNum();
			this.label23 = new System.Windows.Forms.Label();
			this.textStand2 = new OpenDental.ValidNum();
			this.textMaxProsth = new OpenDental.ValidNum();
			this.textDeductPrevent = new OpenDental.ValidDouble();
			this.label22 = new System.Windows.Forms.Label();
			this.textStand1 = new OpenDental.ValidNum();
			this.textProsth = new OpenDental.ValidNum();
			this.label21 = new System.Windows.Forms.Label();
			this.textOralSurg = new OpenDental.ValidNum();
			this.label20 = new System.Windows.Forms.Label();
			this.textPerio = new OpenDental.ValidNum();
			this.label19 = new System.Windows.Forms.Label();
			this.textEndo = new OpenDental.ValidNum();
			this.label18 = new System.Windows.Forms.Label();
			this.textRoutinePrev = new OpenDental.ValidNum();
			this.label9 = new System.Windows.Forms.Label();
			this.textCrowns = new OpenDental.ValidNum();
			this.label15 = new System.Windows.Forms.Label();
			this.textRestorative = new OpenDental.ValidNum();
			this.label16 = new System.Windows.Forms.Label();
			this.textDiagnostic = new OpenDental.ValidNum();
			this.label17 = new System.Windows.Forms.Label();
			this.textSubscNote = new OpenDental.ODtextBox();
			this.labelSubscNote = new System.Windows.Forms.Label();
			this.gridBenefits = new OpenDental.UI.GridOD();
			this.butDelete = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.panelSimple = new System.Windows.Forms.Panel();
			this.textSealantAge = new OpenDental.ValidNum();
			this.label24 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.textAnnualMaxFam = new OpenDental.ValidDouble();
			this.textDeductibleFam = new OpenDental.ValidDouble();
			this.label13 = new System.Windows.Forms.Label();
			this.textAnnualMax = new OpenDental.ValidDouble();
			this.label1 = new System.Windows.Forms.Label();
			this.textFlo = new OpenDental.ValidNum();
			this.label2 = new System.Windows.Forms.Label();
			this.textDeductible = new OpenDental.ValidDouble();
			this.groupBox3 = new OpenDental.UI.GroupBox();
			this.textOrthoAge = new OpenDental.ValidNum();
			this.labelOrthoThroughAge = new System.Windows.Forms.Label();
			this.textOrthoPercent = new OpenDental.ValidNum();
			this.label11 = new System.Windows.Forms.Label();
			this.textOrthoMax = new OpenDental.ValidDouble();
			this.label10 = new System.Windows.Forms.Label();
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.butFrequencies = new OpenDental.UI.Button();
			this.comboExams = new OpenDental.UI.ComboBox();
			this.textExams = new OpenDental.ValidNum();
			this.label8 = new System.Windows.Forms.Label();
			this.comboPano = new OpenDental.UI.ComboBox();
			this.textPano = new OpenDental.ValidNum();
			this.label7 = new System.Windows.Forms.Label();
			this.comboBW = new OpenDental.UI.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textBW = new OpenDental.ValidNum();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.groupYear.SuspendLayout();
			this.groupCategories.SuspendLayout();
			this.panelSimple.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupYear
			// 
			this.groupYear.Controls.Add(this.checkCalendarYear);
			this.groupYear.Controls.Add(this.textMonth);
			this.groupYear.Controls.Add(this.label30);
			this.groupYear.Location = new System.Drawing.Point(141, 3);
			this.groupYear.Name = "groupYear";
			this.groupYear.Size = new System.Drawing.Size(181, 36);
			this.groupYear.TabIndex = 1;
			this.groupYear.Text = "Benefit Year";
			// 
			// checkCalendarYear
			// 
			this.checkCalendarYear.Location = new System.Drawing.Point(8, 16);
			this.checkCalendarYear.Name = "checkCalendarYear";
			this.checkCalendarYear.Size = new System.Drawing.Size(82, 17);
			this.checkCalendarYear.TabIndex = 0;
			this.checkCalendarYear.Text = "Calendar";
			this.checkCalendarYear.Click += new System.EventHandler(this.checkCalendarYear_Click);
			// 
			// textMonth
			// 
			this.textMonth.Location = new System.Drawing.Point(145, 13);
			this.textMonth.MaxVal = 12;
			this.textMonth.MinVal = 1;
			this.textMonth.Name = "textMonth";
			this.textMonth.ShowZero = false;
			this.textMonth.Size = new System.Drawing.Size(28, 20);
			this.textMonth.TabIndex = 1;
			// 
			// label30
			// 
			this.label30.Location = new System.Drawing.Point(89, 15);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(56, 16);
			this.label30.TabIndex = 0;
			this.label30.Text = "Month";
			this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkSimplified
			// 
			this.checkSimplified.Checked = true;
			this.checkSimplified.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkSimplified.Location = new System.Drawing.Point(12, 10);
			this.checkSimplified.Name = "checkSimplified";
			this.checkSimplified.Size = new System.Drawing.Size(123, 17);
			this.checkSimplified.TabIndex = 0;
			this.checkSimplified.Text = "Simplified View";
			this.checkSimplified.Click += new System.EventHandler(this.checkSimplified_Click);
			// 
			// groupCategories
			// 
			this.groupCategories.Controls.Add(this.labelWaitingPeriod);
			this.groupCategories.Controls.Add(this.textWaitProsth);
			this.groupCategories.Controls.Add(this.textWaitCrowns);
			this.groupCategories.Controls.Add(this.labelMonths);
			this.groupCategories.Controls.Add(this.textWaitOralSurg);
			this.groupCategories.Controls.Add(this.textWaitPerio);
			this.groupCategories.Controls.Add(this.textWaitEndo);
			this.groupCategories.Controls.Add(this.textWaitRestorative);
			this.groupCategories.Controls.Add(this.textDeductDiagFam);
			this.groupCategories.Controls.Add(this.textDeductXrayFam);
			this.groupCategories.Controls.Add(this.textDeductDiag);
			this.groupCategories.Controls.Add(this.label3);
			this.groupCategories.Controls.Add(this.textDeductXray);
			this.groupCategories.Controls.Add(this.label29);
			this.groupCategories.Controls.Add(this.label27);
			this.groupCategories.Controls.Add(this.label26);
			this.groupCategories.Controls.Add(this.textDeductPreventFam);
			this.groupCategories.Controls.Add(this.textXray);
			this.groupCategories.Controls.Add(this.label25);
			this.groupCategories.Controls.Add(this.label12);
			this.groupCategories.Controls.Add(this.panel3);
			this.groupCategories.Controls.Add(this.panel2);
			this.groupCategories.Controls.Add(this.panel1);
			this.groupCategories.Controls.Add(this.textStand4);
			this.groupCategories.Controls.Add(this.textAccident);
			this.groupCategories.Controls.Add(this.label23);
			this.groupCategories.Controls.Add(this.textStand2);
			this.groupCategories.Controls.Add(this.textMaxProsth);
			this.groupCategories.Controls.Add(this.textDeductPrevent);
			this.groupCategories.Controls.Add(this.label22);
			this.groupCategories.Controls.Add(this.textStand1);
			this.groupCategories.Controls.Add(this.textProsth);
			this.groupCategories.Controls.Add(this.label21);
			this.groupCategories.Controls.Add(this.textOralSurg);
			this.groupCategories.Controls.Add(this.label20);
			this.groupCategories.Controls.Add(this.textPerio);
			this.groupCategories.Controls.Add(this.label19);
			this.groupCategories.Controls.Add(this.textEndo);
			this.groupCategories.Controls.Add(this.label18);
			this.groupCategories.Controls.Add(this.textRoutinePrev);
			this.groupCategories.Controls.Add(this.label9);
			this.groupCategories.Controls.Add(this.textCrowns);
			this.groupCategories.Controls.Add(this.label15);
			this.groupCategories.Controls.Add(this.textRestorative);
			this.groupCategories.Controls.Add(this.label16);
			this.groupCategories.Controls.Add(this.textDiagnostic);
			this.groupCategories.Controls.Add(this.label17);
			this.groupCategories.Location = new System.Drawing.Point(371, 3);
			this.groupCategories.Name = "groupCategories";
			this.groupCategories.Size = new System.Drawing.Size(439, 362);
			this.groupCategories.TabIndex = 8;
			this.groupCategories.Text = "Categories";
			// 
			// labelWaitingPeriod
			// 
			this.labelWaitingPeriod.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelWaitingPeriod.Location = new System.Drawing.Point(253, 105);
			this.labelWaitingPeriod.Name = "labelWaitingPeriod";
			this.labelWaitingPeriod.Size = new System.Drawing.Size(180, 15);
			this.labelWaitingPeriod.TabIndex = 207;
			this.labelWaitingPeriod.Text = "Waiting Periods (if applicable)";
			this.labelWaitingPeriod.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textWaitProsth
			// 
			this.textWaitProsth.Location = new System.Drawing.Point(280, 252);
			this.textWaitProsth.Name = "textWaitProsth";
			this.textWaitProsth.ShowZero = false;
			this.textWaitProsth.Size = new System.Drawing.Size(60, 20);
			this.textWaitProsth.TabIndex = 22;
			// 
			// textWaitCrowns
			// 
			this.textWaitCrowns.Location = new System.Drawing.Point(280, 232);
			this.textWaitCrowns.Name = "textWaitCrowns";
			this.textWaitCrowns.ShowZero = false;
			this.textWaitCrowns.Size = new System.Drawing.Size(60, 20);
			this.textWaitCrowns.TabIndex = 20;
			// 
			// labelMonths
			// 
			this.labelMonths.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelMonths.Location = new System.Drawing.Point(277, 116);
			this.labelMonths.Name = "labelMonths";
			this.labelMonths.Size = new System.Drawing.Size(67, 22);
			this.labelMonths.TabIndex = 212;
			this.labelMonths.Text = "# Months";
			this.labelMonths.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textWaitOralSurg
			// 
			this.textWaitOralSurg.Location = new System.Drawing.Point(280, 205);
			this.textWaitOralSurg.Name = "textWaitOralSurg";
			this.textWaitOralSurg.ShowZero = false;
			this.textWaitOralSurg.Size = new System.Drawing.Size(60, 20);
			this.textWaitOralSurg.TabIndex = 17;
			// 
			// textWaitPerio
			// 
			this.textWaitPerio.Location = new System.Drawing.Point(280, 185);
			this.textWaitPerio.Name = "textWaitPerio";
			this.textWaitPerio.ShowZero = false;
			this.textWaitPerio.Size = new System.Drawing.Size(60, 20);
			this.textWaitPerio.TabIndex = 15;
			// 
			// textWaitEndo
			// 
			this.textWaitEndo.Location = new System.Drawing.Point(280, 165);
			this.textWaitEndo.Name = "textWaitEndo";
			this.textWaitEndo.ShowZero = false;
			this.textWaitEndo.Size = new System.Drawing.Size(60, 20);
			this.textWaitEndo.TabIndex = 13;
			// 
			// textWaitRestorative
			// 
			this.textWaitRestorative.Location = new System.Drawing.Point(280, 145);
			this.textWaitRestorative.Name = "textWaitRestorative";
			this.textWaitRestorative.ShowZero = false;
			this.textWaitRestorative.Size = new System.Drawing.Size(60, 20);
			this.textWaitRestorative.TabIndex = 11;
			// 
			// textDeductDiagFam
			// 
			this.textDeductDiagFam.Location = new System.Drawing.Point(348, 38);
			this.textDeductDiagFam.MaxVal = 100000000D;
			this.textDeductDiagFam.MinVal = -100000000D;
			this.textDeductDiagFam.Name = "textDeductDiagFam";
			this.textDeductDiagFam.Size = new System.Drawing.Size(62, 20);
			this.textDeductDiagFam.TabIndex = 2;
			// 
			// textDeductXrayFam
			// 
			this.textDeductXrayFam.Location = new System.Drawing.Point(348, 58);
			this.textDeductXrayFam.MaxVal = 100000000D;
			this.textDeductXrayFam.MinVal = -100000000D;
			this.textDeductXrayFam.Name = "textDeductXrayFam";
			this.textDeductXrayFam.Size = new System.Drawing.Size(62, 20);
			this.textDeductXrayFam.TabIndex = 5;
			// 
			// textDeductDiag
			// 
			this.textDeductDiag.Location = new System.Drawing.Point(280, 38);
			this.textDeductDiag.MaxVal = 100000000D;
			this.textDeductDiag.MinVal = -100000000D;
			this.textDeductDiag.Name = "textDeductDiag";
			this.textDeductDiag.Size = new System.Drawing.Size(62, 20);
			this.textDeductDiag.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(275, 3);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(143, 15);
			this.label3.TabIndex = 206;
			this.label3.Text = "Deductibles (if different)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textDeductXray
			// 
			this.textDeductXray.Location = new System.Drawing.Point(280, 58);
			this.textDeductXray.MaxVal = 100000000D;
			this.textDeductXray.MinVal = -100000000D;
			this.textDeductXray.Name = "textDeductXray";
			this.textDeductXray.Size = new System.Drawing.Size(62, 20);
			this.textDeductXray.TabIndex = 4;
			// 
			// label29
			// 
			this.label29.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label29.Location = new System.Drawing.Point(348, 21);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(58, 15);
			this.label29.TabIndex = 204;
			this.label29.Text = "Family";
			this.label29.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// label27
			// 
			this.label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label27.Location = new System.Drawing.Point(283, 21);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(58, 15);
			this.label27.TabIndex = 203;
			this.label27.Text = "Individual";
			this.label27.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// label26
			// 
			this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label26.Location = new System.Drawing.Point(148, 14);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(60, 22);
			this.label26.TabIndex = 202;
			this.label26.Text = "%";
			this.label26.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textDeductPreventFam
			// 
			this.textDeductPreventFam.Location = new System.Drawing.Point(348, 78);
			this.textDeductPreventFam.MaxVal = 100000000D;
			this.textDeductPreventFam.MinVal = -100000000D;
			this.textDeductPreventFam.Name = "textDeductPreventFam";
			this.textDeductPreventFam.Size = new System.Drawing.Size(62, 20);
			this.textDeductPreventFam.TabIndex = 8;
			// 
			// textXray
			// 
			this.textXray.Location = new System.Drawing.Point(148, 58);
			this.textXray.MaxVal = 100;
			this.textXray.Name = "textXray";
			this.textXray.ShowZero = false;
			this.textXray.Size = new System.Drawing.Size(60, 20);
			this.textXray.TabIndex = 3;
			this.textXray.Leave += new System.EventHandler(this.textXray_Leave);
			// 
			// label25
			// 
			this.label25.Location = new System.Drawing.Point(26, 57);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(120, 21);
			this.label25.TabIndex = 200;
			this.label25.Text = "X-Ray (if different)";
			this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label12
			// 
			this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label12.Location = new System.Drawing.Point(213, 14);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(67, 22);
			this.label12.TabIndex = 199;
			this.label12.Text = "Quick %";
			this.label12.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panel3.Location = new System.Drawing.Point(43, 275);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(234, 1);
			this.panel3.TabIndex = 198;
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panel2.Location = new System.Drawing.Point(43, 228);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(234, 1);
			this.panel2.TabIndex = 197;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panel1.Location = new System.Drawing.Point(43, 101);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(380, 1);
			this.panel1.TabIndex = 196;
			// 
			// textStand4
			// 
			this.textStand4.Location = new System.Drawing.Point(214, 244);
			this.textStand4.MaxVal = 100;
			this.textStand4.Name = "textStand4";
			this.textStand4.ShowZero = false;
			this.textStand4.Size = new System.Drawing.Size(60, 20);
			this.textStand4.TabIndex = 23;
			this.textStand4.TextChanged += new System.EventHandler(this.textStand4_TextChanged);
			// 
			// textAccident
			// 
			this.textAccident.Location = new System.Drawing.Point(148, 299);
			this.textAccident.MaxVal = 100;
			this.textAccident.Name = "textAccident";
			this.textAccident.ShowZero = false;
			this.textAccident.Size = new System.Drawing.Size(60, 20);
			this.textAccident.TabIndex = 25;
			// 
			// label23
			// 
			this.label23.Location = new System.Drawing.Point(26, 298);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(120, 21);
			this.label23.TabIndex = 194;
			this.label23.Text = "Accident";
			this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textStand2
			// 
			this.textStand2.Location = new System.Drawing.Point(214, 175);
			this.textStand2.MaxVal = 100;
			this.textStand2.Name = "textStand2";
			this.textStand2.ShowZero = false;
			this.textStand2.Size = new System.Drawing.Size(60, 20);
			this.textStand2.TabIndex = 18;
			this.textStand2.TextChanged += new System.EventHandler(this.textStand2_TextChanged);
			// 
			// textMaxProsth
			// 
			this.textMaxProsth.Location = new System.Drawing.Point(148, 279);
			this.textMaxProsth.MaxVal = 100;
			this.textMaxProsth.Name = "textMaxProsth";
			this.textMaxProsth.ShowZero = false;
			this.textMaxProsth.Size = new System.Drawing.Size(60, 20);
			this.textMaxProsth.TabIndex = 24;
			// 
			// textDeductPrevent
			// 
			this.textDeductPrevent.Location = new System.Drawing.Point(280, 78);
			this.textDeductPrevent.MaxVal = 100000000D;
			this.textDeductPrevent.MinVal = -100000000D;
			this.textDeductPrevent.Name = "textDeductPrevent";
			this.textDeductPrevent.Size = new System.Drawing.Size(62, 20);
			this.textDeductPrevent.TabIndex = 7;
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(26, 278);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(120, 21);
			this.label22.TabIndex = 192;
			this.label22.Text = "Maxillofacial Prosth";
			this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textStand1
			// 
			this.textStand1.Location = new System.Drawing.Point(214, 58);
			this.textStand1.MaxVal = 100;
			this.textStand1.Name = "textStand1";
			this.textStand1.ShowZero = false;
			this.textStand1.Size = new System.Drawing.Size(60, 20);
			this.textStand1.TabIndex = 9;
			this.textStand1.TextChanged += new System.EventHandler(this.textStand1_TextChanged);
			// 
			// textProsth
			// 
			this.textProsth.Location = new System.Drawing.Point(148, 252);
			this.textProsth.MaxVal = 100;
			this.textProsth.Name = "textProsth";
			this.textProsth.ShowZero = false;
			this.textProsth.Size = new System.Drawing.Size(60, 20);
			this.textProsth.TabIndex = 21;
			this.textProsth.Leave += new System.EventHandler(this.textProsth_Leave);
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(26, 251);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(120, 21);
			this.label21.TabIndex = 190;
			this.label21.Text = "Prosthodontics";
			this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textOralSurg
			// 
			this.textOralSurg.Location = new System.Drawing.Point(148, 205);
			this.textOralSurg.MaxVal = 100;
			this.textOralSurg.Name = "textOralSurg";
			this.textOralSurg.ShowZero = false;
			this.textOralSurg.Size = new System.Drawing.Size(60, 20);
			this.textOralSurg.TabIndex = 16;
			this.textOralSurg.Leave += new System.EventHandler(this.textOralSurg_Leave);
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(26, 204);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(120, 21);
			this.label20.TabIndex = 188;
			this.label20.Text = "Oral Surgery";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPerio
			// 
			this.textPerio.Location = new System.Drawing.Point(148, 185);
			this.textPerio.MaxVal = 100;
			this.textPerio.Name = "textPerio";
			this.textPerio.ShowZero = false;
			this.textPerio.Size = new System.Drawing.Size(60, 20);
			this.textPerio.TabIndex = 14;
			this.textPerio.Leave += new System.EventHandler(this.textPerio_Leave);
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(26, 184);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(120, 21);
			this.label19.TabIndex = 186;
			this.label19.Text = "Perio";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textEndo
			// 
			this.textEndo.Location = new System.Drawing.Point(148, 165);
			this.textEndo.MaxVal = 100;
			this.textEndo.Name = "textEndo";
			this.textEndo.ShowZero = false;
			this.textEndo.Size = new System.Drawing.Size(60, 20);
			this.textEndo.TabIndex = 12;
			this.textEndo.Leave += new System.EventHandler(this.textEndo_Leave);
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(26, 164);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(120, 21);
			this.label18.TabIndex = 184;
			this.label18.Text = "Endo";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRoutinePrev
			// 
			this.textRoutinePrev.Location = new System.Drawing.Point(148, 78);
			this.textRoutinePrev.MaxVal = 100;
			this.textRoutinePrev.Name = "textRoutinePrev";
			this.textRoutinePrev.ShowZero = false;
			this.textRoutinePrev.Size = new System.Drawing.Size(60, 20);
			this.textRoutinePrev.TabIndex = 6;
			this.textRoutinePrev.Leave += new System.EventHandler(this.textRoutinePrev_Leave);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(26, 77);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(120, 21);
			this.label9.TabIndex = 182;
			this.label9.Text = "Routine Preventive";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCrowns
			// 
			this.textCrowns.Location = new System.Drawing.Point(148, 232);
			this.textCrowns.MaxVal = 100;
			this.textCrowns.Name = "textCrowns";
			this.textCrowns.ShowZero = false;
			this.textCrowns.Size = new System.Drawing.Size(60, 20);
			this.textCrowns.TabIndex = 19;
			this.textCrowns.Leave += new System.EventHandler(this.textCrowns_Leave);
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(26, 231);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(120, 21);
			this.label15.TabIndex = 180;
			this.label15.Text = "Crowns";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRestorative
			// 
			this.textRestorative.Location = new System.Drawing.Point(148, 145);
			this.textRestorative.MaxVal = 100;
			this.textRestorative.Name = "textRestorative";
			this.textRestorative.ShowZero = false;
			this.textRestorative.Size = new System.Drawing.Size(60, 20);
			this.textRestorative.TabIndex = 10;
			this.textRestorative.Leave += new System.EventHandler(this.textRestorative_Leave);
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(26, 144);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(120, 21);
			this.label16.TabIndex = 178;
			this.label16.Text = "Restorative";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDiagnostic
			// 
			this.textDiagnostic.Location = new System.Drawing.Point(148, 38);
			this.textDiagnostic.MaxVal = 100;
			this.textDiagnostic.Name = "textDiagnostic";
			this.textDiagnostic.ShowZero = false;
			this.textDiagnostic.Size = new System.Drawing.Size(60, 20);
			this.textDiagnostic.TabIndex = 0;
			this.textDiagnostic.Leave += new System.EventHandler(this.textDiagnostic_Leave);
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(3, 37);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(143, 21);
			this.label17.TabIndex = 176;
			this.label17.Text = "Diagnostic (includes x-ray)";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSubscNote
			// 
			this.textSubscNote.AcceptsTab = true;
			this.textSubscNote.BackColor = System.Drawing.SystemColors.Window;
			this.textSubscNote.DetectLinksEnabled = false;
			this.textSubscNote.DetectUrls = false;
			this.textSubscNote.Location = new System.Drawing.Point(95, 590);
			this.textSubscNote.Name = "textSubscNote";
			this.textSubscNote.QuickPasteType = OpenDentBusiness.QuickPasteType.InsPlan;
			this.textSubscNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSubscNote.Size = new System.Drawing.Size(485, 98);
			this.textSubscNote.TabIndex = 6;
			this.textSubscNote.Text = "1 - Benefits\n2\n3 lines will show here in 46 vert.\n4 lines will show here in 59 ve" +
    "rt.\n5 lines in 72 vert\n6 lines in 85 vert\n7 lines in 98";
			// 
			// labelSubscNote
			// 
			this.labelSubscNote.Location = new System.Drawing.Point(21, 593);
			this.labelSubscNote.Name = "labelSubscNote";
			this.labelSubscNote.Size = new System.Drawing.Size(74, 41);
			this.labelSubscNote.TabIndex = 160;
			this.labelSubscNote.Text = "Notes";
			this.labelSubscNote.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// gridBenefits
			// 
			this.gridBenefits.Location = new System.Drawing.Point(6, 371);
			this.gridBenefits.Name = "gridBenefits";
			this.gridBenefits.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridBenefits.Size = new System.Drawing.Size(574, 213);
			this.gridBenefits.TabIndex = 3;
			this.gridBenefits.Title = "Other Benefits";
			this.gridBenefits.TranslationName = "TableInsBenefits";
			this.gridBenefits.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridBenefits_CellDoubleClick);
			// 
			// butDelete
			// 
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(586, 401);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(78, 24);
			this.butDelete.TabIndex = 5;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butClear_Click);
			// 
			// butAdd
			// 
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(586, 371);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(78, 24);
			this.butAdd.TabIndex = 4;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(749, 628);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 7;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(749, 658);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 8;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// panelSimple
			// 
			this.panelSimple.Controls.Add(this.textSealantAge);
			this.panelSimple.Controls.Add(this.label24);
			this.panelSimple.Controls.Add(this.label14);
			this.panelSimple.Controls.Add(this.textAnnualMaxFam);
			this.panelSimple.Controls.Add(this.textDeductibleFam);
			this.panelSimple.Controls.Add(this.label13);
			this.panelSimple.Controls.Add(this.textAnnualMax);
			this.panelSimple.Controls.Add(this.label1);
			this.panelSimple.Controls.Add(this.textFlo);
			this.panelSimple.Controls.Add(this.label2);
			this.panelSimple.Controls.Add(this.textDeductible);
			this.panelSimple.Controls.Add(this.groupBox3);
			this.panelSimple.Controls.Add(this.groupBox1);
			this.panelSimple.Controls.Add(this.label4);
			this.panelSimple.Location = new System.Drawing.Point(1, 45);
			this.panelSimple.Name = "panelSimple";
			this.panelSimple.Size = new System.Drawing.Size(367, 320);
			this.panelSimple.TabIndex = 161;
			// 
			// textSealantAge
			// 
			this.textSealantAge.Location = new System.Drawing.Point(166, 83);
			this.textSealantAge.MinVal = 1;
			this.textSealantAge.Name = "textSealantAge";
			this.textSealantAge.ShowZero = false;
			this.textSealantAge.Size = new System.Drawing.Size(39, 20);
			this.textSealantAge.TabIndex = 5;
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(18, 83);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(144, 21);
			this.label24.TabIndex = 185;
			this.label24.Text = "Sealants Through Age";
			this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(242, 6);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(76, 15);
			this.label14.TabIndex = 183;
			this.label14.Text = "Family";
			this.label14.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textAnnualMaxFam
			// 
			this.textAnnualMaxFam.Location = new System.Drawing.Point(243, 22);
			this.textAnnualMaxFam.MaxVal = 100000000D;
			this.textAnnualMaxFam.MinVal = -100000000D;
			this.textAnnualMaxFam.Name = "textAnnualMaxFam";
			this.textAnnualMaxFam.Size = new System.Drawing.Size(73, 20);
			this.textAnnualMaxFam.TabIndex = 1;
			// 
			// textDeductibleFam
			// 
			this.textDeductibleFam.Location = new System.Drawing.Point(243, 42);
			this.textDeductibleFam.MaxVal = 100000000D;
			this.textDeductibleFam.MinVal = -100000000D;
			this.textDeductibleFam.Name = "textDeductibleFam";
			this.textDeductibleFam.Size = new System.Drawing.Size(73, 20);
			this.textDeductibleFam.TabIndex = 3;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(165, 6);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(78, 15);
			this.label13.TabIndex = 179;
			this.label13.Text = "Individual";
			this.label13.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textAnnualMax
			// 
			this.textAnnualMax.Location = new System.Drawing.Point(166, 22);
			this.textAnnualMax.MaxVal = 100000000D;
			this.textAnnualMax.MinVal = -100000000D;
			this.textAnnualMax.Name = "textAnnualMax";
			this.textAnnualMax.Size = new System.Drawing.Size(73, 20);
			this.textAnnualMax.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(62, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 21);
			this.label1.TabIndex = 159;
			this.label1.Text = "Annual Max";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFlo
			// 
			this.textFlo.Location = new System.Drawing.Point(166, 63);
			this.textFlo.MinVal = 1;
			this.textFlo.Name = "textFlo";
			this.textFlo.ShowZero = false;
			this.textFlo.Size = new System.Drawing.Size(39, 20);
			this.textFlo.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(15, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(147, 21);
			this.label2.TabIndex = 163;
			this.label2.Text = "General Deductible";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDeductible
			// 
			this.textDeductible.Location = new System.Drawing.Point(166, 42);
			this.textDeductible.MaxVal = 100000000D;
			this.textDeductible.MinVal = -100000000D;
			this.textDeductible.Name = "textDeductible";
			this.textDeductible.Size = new System.Drawing.Size(73, 20);
			this.textDeductible.TabIndex = 2;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.textOrthoAge);
			this.groupBox3.Controls.Add(this.labelOrthoThroughAge);
			this.groupBox3.Controls.Add(this.textOrthoPercent);
			this.groupBox3.Controls.Add(this.label11);
			this.groupBox3.Controls.Add(this.textOrthoMax);
			this.groupBox3.Controls.Add(this.label10);
			this.groupBox3.Location = new System.Drawing.Point(67, 238);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(282, 77);
			this.groupBox3.TabIndex = 7;
			this.groupBox3.Text = "Ortho";
			// 
			// textOrthoAge
			// 
			this.textOrthoAge.Location = new System.Drawing.Point(142, 49);
			this.textOrthoAge.MinVal = 1;
			this.textOrthoAge.Name = "textOrthoAge";
			this.textOrthoAge.ShowZero = false;
			this.textOrthoAge.Size = new System.Drawing.Size(39, 20);
			this.textOrthoAge.TabIndex = 2;
			// 
			// labelOrthoThroughAge
			// 
			this.labelOrthoThroughAge.Location = new System.Drawing.Point(43, 48);
			this.labelOrthoThroughAge.Name = "labelOrthoThroughAge";
			this.labelOrthoThroughAge.Size = new System.Drawing.Size(99, 18);
			this.labelOrthoThroughAge.TabIndex = 176;
			this.labelOrthoThroughAge.Text = "Ortho Through Age";
			this.labelOrthoThroughAge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textOrthoPercent
			// 
			this.textOrthoPercent.Location = new System.Drawing.Point(142, 29);
			this.textOrthoPercent.MaxVal = 100;
			this.textOrthoPercent.Name = "textOrthoPercent";
			this.textOrthoPercent.ShowZero = false;
			this.textOrthoPercent.Size = new System.Drawing.Size(60, 20);
			this.textOrthoPercent.TabIndex = 1;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(72, 29);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(70, 20);
			this.label11.TabIndex = 174;
			this.label11.Text = "Percentage";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textOrthoMax
			// 
			this.textOrthoMax.Location = new System.Drawing.Point(142, 9);
			this.textOrthoMax.MaxVal = 100000000D;
			this.textOrthoMax.MinVal = -100000000D;
			this.textOrthoMax.Name = "textOrthoMax";
			this.textOrthoMax.Size = new System.Drawing.Size(73, 20);
			this.textOrthoMax.TabIndex = 0;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(47, 13);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(94, 14);
			this.label10.TabIndex = 163;
			this.label10.Text = "Lifetime Max";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butFrequencies);
			this.groupBox1.Controls.Add(this.comboExams);
			this.groupBox1.Controls.Add(this.textExams);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.comboPano);
			this.groupBox1.Controls.Add(this.textPano);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.comboBW);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.textBW);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Location = new System.Drawing.Point(67, 112);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(282, 120);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.Text = "Frequencies";
			// 
			// butFrequencies
			// 
			this.butFrequencies.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butFrequencies.Location = new System.Drawing.Point(142, 89);
			this.butFrequencies.Name = "butFrequencies";
			this.butFrequencies.Size = new System.Drawing.Size(75, 24);
			this.butFrequencies.TabIndex = 161;
			this.butFrequencies.Text = "&More";
			this.butFrequencies.Click += new System.EventHandler(this.butFrequencies_Click);
			// 
			// comboExams
			// 
			this.comboExams.Location = new System.Drawing.Point(142, 62);
			this.comboExams.Name = "comboExams";
			this.comboExams.Size = new System.Drawing.Size(136, 21);
			this.comboExams.TabIndex = 5;
			// 
			// textExams
			// 
			this.textExams.Location = new System.Drawing.Point(99, 62);
			this.textExams.MinVal = 1;
			this.textExams.Name = "textExams";
			this.textExams.ShowZero = false;
			this.textExams.Size = new System.Drawing.Size(39, 20);
			this.textExams.TabIndex = 4;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(27, 61);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(70, 21);
			this.label8.TabIndex = 176;
			this.label8.Text = "Exams";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPano
			// 
			this.comboPano.Location = new System.Drawing.Point(142, 41);
			this.comboPano.Name = "comboPano";
			this.comboPano.Size = new System.Drawing.Size(136, 21);
			this.comboPano.TabIndex = 3;
			// 
			// textPano
			// 
			this.textPano.Location = new System.Drawing.Point(99, 41);
			this.textPano.MinVal = 1;
			this.textPano.Name = "textPano";
			this.textPano.ShowZero = false;
			this.textPano.Size = new System.Drawing.Size(39, 20);
			this.textPano.TabIndex = 2;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(27, 40);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(70, 21);
			this.label7.TabIndex = 173;
			this.label7.Text = "Pano/FMX";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBW
			// 
			this.comboBW.Location = new System.Drawing.Point(142, 20);
			this.comboBW.Name = "comboBW";
			this.comboBW.Size = new System.Drawing.Size(136, 21);
			this.comboBW.TabIndex = 1;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(99, 6);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(39, 12);
			this.label6.TabIndex = 170;
			this.label6.Text = "#";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textBW
			// 
			this.textBW.Location = new System.Drawing.Point(99, 20);
			this.textBW.MinVal = 1;
			this.textBW.Name = "textBW";
			this.textBW.ShowZero = false;
			this.textBW.Size = new System.Drawing.Size(39, 20);
			this.textBW.TabIndex = 0;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(27, 19);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(70, 21);
			this.label5.TabIndex = 168;
			this.label5.Text = "BWs";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(18, 63);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(144, 21);
			this.label4.TabIndex = 167;
			this.label4.Text = "Fluoride Through Age";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormInsBenefits
			// 
			this.ClientSize = new System.Drawing.Size(836, 696);
			this.Controls.Add(this.panelSimple);
			this.Controls.Add(this.groupYear);
			this.Controls.Add(this.checkSimplified);
			this.Controls.Add(this.textSubscNote);
			this.Controls.Add(this.labelSubscNote);
			this.Controls.Add(this.gridBenefits);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupCategories);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormInsBenefits";
			this.ShowInTaskbar = false;
			this.Text = "Edit Benefits";
			this.Load += new System.EventHandler(this.FormInsBenefits_Load);
			this.groupYear.ResumeLayout(false);
			this.groupYear.PerformLayout();
			this.groupCategories.ResumeLayout(false);
			this.groupCategories.PerformLayout();
			this.panelSimple.ResumeLayout(false);
			this.panelSimple.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.GridOD gridBenefits;
		private OpenDental.UI.CheckBox checkSimplified;
		private ODtextBox textSubscNote;
		private Label labelSubscNote;
		private ValidDouble textDeductPrevent;
		private ValidNum textStand1;
		private ValidNum textStand2;
		private OpenDental.UI.GroupBox groupCategories;
		private ValidNum textOralSurg;
		private Label label20;
		private ValidNum textPerio;
		private Label label19;
		private ValidNum textEndo;
		private Label label18;
		private ValidNum textRoutinePrev;
		private Label label9;
		private ValidNum textCrowns;
		private Label label15;
		private ValidNum textRestorative;
		private Label label16;
		private ValidNum textDiagnostic;
		private Label label17;
		private ValidNum textProsth;
		private Label label21;
		private ValidNum textMaxProsth;
		private Label label22;
		private ValidNum textAccident;
		private Label label23;
		private ValidNum textStand4;
		private Label label12;
		private Panel panel3;
		private Panel panel2;
		private Panel panel1;
		private ValidDouble textDeductPreventFam;
		private ValidNum textXray;
		private Label label25;
		private Label label29;
		private Label label27;
		private Label label26;
		private ValidDouble textDeductDiagFam;
		private ValidDouble textDeductXrayFam;
		private ValidDouble textDeductDiag;
		private Label label3;
		private ValidDouble textDeductXray;
		private ValidNum textMonth;
		private Label label30;
		private OpenDental.UI.GroupBox groupYear;
		private OpenDental.UI.CheckBox checkCalendarYear;
		private Label labelMonths;
		private ValidNum textWaitOralSurg;
		private ValidNum textWaitPerio;
		private ValidNum textWaitEndo;
		private ValidNum textWaitRestorative;
		private Label labelWaitingPeriod;
		private ValidNum textWaitProsth;
		private ValidNum textWaitCrowns;
		private Panel panelSimple;
		private ValidNum textSealantAge;
		private Label label24;
		private Label label14;
		private ValidDouble textAnnualMaxFam;
		private ValidDouble textDeductibleFam;
		private Label label13;
		private ValidDouble textAnnualMax;
		private Label label1;
		private ValidNum textFlo;
		private Label label2;
		private ValidDouble textDeductible;
		private UI.GroupBox groupBox3;
		private ValidNum textOrthoAge;
		private Label labelOrthoThroughAge;
		private ValidNum textOrthoPercent;
		private Label label11;
		private ValidDouble textOrthoMax;
		private Label label10;
		private UI.GroupBox groupBox1;
		private UI.Button butFrequencies;
		private UI.ComboBox comboExams;
		private ValidNum textExams;
		private Label label8;
		private UI.ComboBox comboPano;
		private ValidNum textPano;
		private Label label7;
		private UI.ComboBox comboBW;
		private Label label6;
		private ValidNum textBW;
		private Label label5;
		private Label label4;
	}
}
