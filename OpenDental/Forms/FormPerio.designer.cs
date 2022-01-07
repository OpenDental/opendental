using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPerio {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPerio));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioCustom = new System.Windows.Forms.RadioButton();
			this.radioRight = new System.Windows.Forms.RadioButton();
			this.radioLeft = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.butColorBleed = new System.Windows.Forms.Button();
			this.butColorPus = new System.Windows.Forms.Button();
			this.butColorCalculus = new System.Windows.Forms.Button();
			this.butColorPlaque = new System.Windows.Forms.Button();
			this.checkThree = new System.Windows.Forms.CheckBox();
			this.checkGingMarg = new System.Windows.Forms.CheckBox();
			this.butCalcIndex = new OpenDental.UI.Button();
			this.butCalculus = new OpenDental.UI.Button();
			this.butPlaque = new OpenDental.UI.Button();
			this.butSkip = new OpenDental.UI.Button();
			this.butCount = new OpenDental.UI.Button();
			this.butPus = new OpenDental.UI.Button();
			this.butBleed = new OpenDental.UI.Button();
			this.but10 = new OpenDental.UI.Button();
			this.checkShowCurrent = new System.Windows.Forms.CheckBox();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textCountMob = new System.Windows.Forms.TextBox();
			this.updownMob = new System.Windows.Forms.DomainUpDown();
			this.textRedMob = new System.Windows.Forms.TextBox();
			this.textCountFurc = new System.Windows.Forms.TextBox();
			this.updownFurc = new System.Windows.Forms.DomainUpDown();
			this.textRedFurc = new System.Windows.Forms.TextBox();
			this.textCountCAL = new System.Windows.Forms.TextBox();
			this.updownCAL = new System.Windows.Forms.DomainUpDown();
			this.textRedCAL = new System.Windows.Forms.TextBox();
			this.textCountGing = new System.Windows.Forms.TextBox();
			this.updownGing = new System.Windows.Forms.DomainUpDown();
			this.textRedGing = new System.Windows.Forms.TextBox();
			this.textCountMGJ = new System.Windows.Forms.TextBox();
			this.updownMGJ = new System.Windows.Forms.DomainUpDown();
			this.textRedMGJ = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.textCountProb = new System.Windows.Forms.TextBox();
			this.updownProb = new System.Windows.Forms.DomainUpDown();
			this.label13 = new System.Windows.Forms.Label();
			this.textRedProb = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textIndexPlaque = new System.Windows.Forms.TextBox();
			this.textIndexSupp = new System.Windows.Forms.TextBox();
			this.textIndexBleeding = new System.Windows.Forms.TextBox();
			this.textIndexCalculus = new System.Windows.Forms.TextBox();
			this.printDialog2 = new System.Windows.Forms.PrintDialog();
			this.printPreviewDlg = new System.Windows.Forms.PrintPreviewDialog();
			this.labelPlaqueHistory = new System.Windows.Forms.Label();
			this.listPlaqueHistory = new OpenDental.UI.ListBoxOD();
			this.textInputBox = new System.Windows.Forms.TextBox();
			this.butCopyPrevious = new OpenDental.UI.Button();
			this.gridP = new OpenDental.ContrPerio();
			this.butSave = new OpenDental.UI.Button();
			this.butGraphical = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.but0 = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.but8 = new OpenDental.UI.Button();
			this.but4 = new OpenDental.UI.Button();
			this.but5 = new OpenDental.UI.Button();
			this.but9 = new OpenDental.UI.Button();
			this.but6 = new OpenDental.UI.Button();
			this.but1 = new OpenDental.UI.Button();
			this.but2 = new OpenDental.UI.Button();
			this.but3 = new OpenDental.UI.Button();
			this.but7 = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butListen = new OpenDental.UI.Button();
			this.labelListening = new System.Windows.Forms.Label();
			this.gridODExam = new OpenDental.UI.GridOD();
			this.labelNotes = new System.Windows.Forms.Label();
			this.textExamNotes = new OpenDental.ODtextBox();
			this.butCopyNote = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioCustom);
			this.groupBox1.Controls.Add(this.radioRight);
			this.groupBox1.Controls.Add(this.radioLeft);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(1006, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(213, 43);
			this.groupBox1.TabIndex = 13;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Auto Advance";
			// 
			// radioCustom
			// 
			this.radioCustom.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioCustom.Location = new System.Drawing.Point(147, 20);
			this.radioCustom.Name = "radioCustom";
			this.radioCustom.Size = new System.Drawing.Size(61, 18);
			this.radioCustom.TabIndex = 2;
			this.radioCustom.Text = "Custom";
			this.radioCustom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioCustom.Click += new System.EventHandler(this.RadioCustom_Click);
			// 
			// radioRight
			// 
			this.radioRight.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioRight.Location = new System.Drawing.Point(11, 20);
			this.radioRight.Name = "radioRight";
			this.radioRight.Size = new System.Drawing.Size(61, 18);
			this.radioRight.TabIndex = 1;
			this.radioRight.Text = "Right";
			this.radioRight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioRight.Click += new System.EventHandler(this.radioRight_Click);
			// 
			// radioLeft
			// 
			this.radioLeft.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioLeft.Checked = true;
			this.radioLeft.Location = new System.Drawing.Point(74, 20);
			this.radioLeft.Name = "radioLeft";
			this.radioLeft.Size = new System.Drawing.Size(60, 18);
			this.radioLeft.TabIndex = 0;
			this.radioLeft.TabStop = true;
			this.radioLeft.Text = "Left";
			this.radioLeft.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioLeft.Click += new System.EventHandler(this.radioLeft_Click);
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(369, 94);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(18, 23);
			this.label2.TabIndex = 35;
			this.label2.Text = "F";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(369, 542);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(18, 23);
			this.label3.TabIndex = 36;
			this.label3.Text = "F";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(369, 408);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(18, 23);
			this.label4.TabIndex = 37;
			this.label4.Text = "L";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(369, 215);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(18, 23);
			this.label5.TabIndex = 38;
			this.label5.Text = "L";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butColorBleed
			// 
			this.butColorBleed.BackColor = System.Drawing.Color.Red;
			this.butColorBleed.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColorBleed.Location = new System.Drawing.Point(1091, 306);
			this.butColorBleed.Name = "butColorBleed";
			this.butColorBleed.Size = new System.Drawing.Size(12, 24);
			this.butColorBleed.TabIndex = 43;
			this.toolTip1.SetToolTip(this.butColorBleed, "Edit Color");
			this.butColorBleed.UseVisualStyleBackColor = false;
			this.butColorBleed.Click += new System.EventHandler(this.butColorBleed_Click);
			// 
			// butColorPus
			// 
			this.butColorPus.BackColor = System.Drawing.Color.Gold;
			this.butColorPus.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColorPus.Location = new System.Drawing.Point(1091, 336);
			this.butColorPus.Name = "butColorPus";
			this.butColorPus.Size = new System.Drawing.Size(12, 24);
			this.butColorPus.TabIndex = 50;
			this.toolTip1.SetToolTip(this.butColorPus, "Edit Color");
			this.butColorPus.UseVisualStyleBackColor = false;
			this.butColorPus.Click += new System.EventHandler(this.butColorPus_Click);
			// 
			// butColorCalculus
			// 
			this.butColorCalculus.BackColor = System.Drawing.Color.Green;
			this.butColorCalculus.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColorCalculus.Location = new System.Drawing.Point(1091, 276);
			this.butColorCalculus.Name = "butColorCalculus";
			this.butColorCalculus.Size = new System.Drawing.Size(12, 24);
			this.butColorCalculus.TabIndex = 67;
			this.toolTip1.SetToolTip(this.butColorCalculus, "Edit Color");
			this.butColorCalculus.UseVisualStyleBackColor = false;
			this.butColorCalculus.Click += new System.EventHandler(this.butColorCalculus_Click);
			// 
			// butColorPlaque
			// 
			this.butColorPlaque.BackColor = System.Drawing.Color.RoyalBlue;
			this.butColorPlaque.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColorPlaque.Location = new System.Drawing.Point(1091, 246);
			this.butColorPlaque.Name = "butColorPlaque";
			this.butColorPlaque.Size = new System.Drawing.Size(12, 24);
			this.butColorPlaque.TabIndex = 66;
			this.toolTip1.SetToolTip(this.butColorPlaque, "Edit Color");
			this.butColorPlaque.UseVisualStyleBackColor = false;
			this.butColorPlaque.Click += new System.EventHandler(this.butColorPlaque_Click);
			// 
			// checkThree
			// 
			this.checkThree.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkThree.Location = new System.Drawing.Point(1006, 53);
			this.checkThree.Name = "checkThree";
			this.checkThree.Size = new System.Drawing.Size(100, 19);
			this.checkThree.TabIndex = 57;
			this.checkThree.Text = "Triplets";
			this.toolTip1.SetToolTip(this.checkThree, "Enter numbers three at a time");
			this.checkThree.Click += new System.EventHandler(this.checkThree_Click);
			// 
			// checkGingMarg
			// 
			this.checkGingMarg.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkGingMarg.Location = new System.Drawing.Point(1112, 53);
			this.checkGingMarg.Name = "checkGingMarg";
			this.checkGingMarg.Size = new System.Drawing.Size(99, 19);
			this.checkGingMarg.TabIndex = 80;
			this.checkGingMarg.Text = "Ging Marg +";
			this.toolTip1.SetToolTip(this.checkGingMarg, "Or hold down the Ctrl key while you type numbers.  Affects gingival margins only." +
        "  Used to input positive gingival margins (hyperplasia).");
			this.checkGingMarg.Click += new System.EventHandler(this.checkGingMarg_CheckedChanged);
			// 
			// butCalcIndex
			// 
			this.butCalcIndex.Location = new System.Drawing.Point(1104, 219);
			this.butCalcIndex.Name = "butCalcIndex";
			this.butCalcIndex.Size = new System.Drawing.Size(84, 24);
			this.butCalcIndex.TabIndex = 74;
			this.butCalcIndex.Text = "Calc Index %";
			this.toolTip1.SetToolTip(this.butCalcIndex, "Calculate the Index for all four types");
			this.butCalcIndex.Click += new System.EventHandler(this.butCalcIndex_Click);
			// 
			// butCalculus
			// 
			this.butCalculus.Location = new System.Drawing.Point(1019, 276);
			this.butCalculus.Name = "butCalculus";
			this.butCalculus.Size = new System.Drawing.Size(72, 24);
			this.butCalculus.TabIndex = 16;
			this.butCalculus.Text = "Calculus";
			this.toolTip1.SetToolTip(this.butCalculus, "C on your keyboard");
			this.butCalculus.Click += new System.EventHandler(this.butCalculus_Click);
			// 
			// butPlaque
			// 
			this.butPlaque.Location = new System.Drawing.Point(1019, 246);
			this.butPlaque.Name = "butPlaque";
			this.butPlaque.Size = new System.Drawing.Size(72, 24);
			this.butPlaque.TabIndex = 15;
			this.butPlaque.Text = "Plaque";
			this.toolTip1.SetToolTip(this.butPlaque, "P on your keyboard");
			this.butPlaque.Click += new System.EventHandler(this.butPlaque_Click);
			// 
			// butSkip
			// 
			this.butSkip.Location = new System.Drawing.Point(1005, 584);
			this.butSkip.Name = "butSkip";
			this.butSkip.Size = new System.Drawing.Size(88, 24);
			this.butSkip.TabIndex = 19;
			this.butSkip.Text = "SkipTeeth";
			this.toolTip1.SetToolTip(this.butSkip, "Toggle the selected teeth as skipped");
			this.butSkip.Click += new System.EventHandler(this.butSkip_Click);
			// 
			// butCount
			// 
			this.butCount.Location = new System.Drawing.Point(99, 18);
			this.butCount.Name = "butCount";
			this.butCount.Size = new System.Drawing.Size(84, 24);
			this.butCount.TabIndex = 6;
			this.butCount.Text = "Count Teeth";
			this.toolTip1.SetToolTip(this.butCount, "Count all six types");
			this.butCount.Click += new System.EventHandler(this.butCount_Click);
			// 
			// butPus
			// 
			this.butPus.Location = new System.Drawing.Point(1019, 336);
			this.butPus.Name = "butPus";
			this.butPus.Size = new System.Drawing.Size(72, 24);
			this.butPus.TabIndex = 18;
			this.butPus.Text = "Suppuration";
			this.toolTip1.SetToolTip(this.butPus, "S on your keyboard");
			this.butPus.Click += new System.EventHandler(this.butPus_Click);
			// 
			// butBleed
			// 
			this.butBleed.Location = new System.Drawing.Point(1019, 306);
			this.butBleed.Name = "butBleed";
			this.butBleed.Size = new System.Drawing.Size(72, 24);
			this.butBleed.TabIndex = 17;
			this.butBleed.Text = "Bleeding";
			this.toolTip1.SetToolTip(this.butBleed, "Space bar or B on your keyboard");
			this.butBleed.Click += new System.EventHandler(this.butBleed_Click);
			// 
			// but10
			// 
			this.but10.Location = new System.Drawing.Point(1074, 179);
			this.but10.Name = "but10";
			this.but10.Size = new System.Drawing.Size(32, 32);
			this.but10.TabIndex = 14;
			this.but10.Text = "10";
			this.toolTip1.SetToolTip(this.but10, "Or hold down the Ctrl key");
			this.but10.Click += new System.EventHandler(this.but10_Click);
			// 
			// checkShowCurrent
			// 
			this.checkShowCurrent.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowCurrent.Location = new System.Drawing.Point(218, 255);
			this.checkShowCurrent.Name = "checkShowCurrent";
			this.checkShowCurrent.Size = new System.Drawing.Size(150, 19);
			this.checkShowCurrent.TabIndex = 83;
			this.checkShowCurrent.Text = "Show current exam only";
			this.toolTip1.SetToolTip(this.checkShowCurrent, "Only show measurements for the currently selected exam");
			this.checkShowCurrent.Click += new System.EventHandler(this.checkShowCurrent_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textCountMob);
			this.groupBox2.Controls.Add(this.updownMob);
			this.groupBox2.Controls.Add(this.textRedMob);
			this.groupBox2.Controls.Add(this.textCountFurc);
			this.groupBox2.Controls.Add(this.updownFurc);
			this.groupBox2.Controls.Add(this.textRedFurc);
			this.groupBox2.Controls.Add(this.textCountCAL);
			this.groupBox2.Controls.Add(this.updownCAL);
			this.groupBox2.Controls.Add(this.textRedCAL);
			this.groupBox2.Controls.Add(this.textCountGing);
			this.groupBox2.Controls.Add(this.updownGing);
			this.groupBox2.Controls.Add(this.textRedGing);
			this.groupBox2.Controls.Add(this.textCountMGJ);
			this.groupBox2.Controls.Add(this.updownMGJ);
			this.groupBox2.Controls.Add(this.textRedMGJ);
			this.groupBox2.Controls.Add(this.label14);
			this.groupBox2.Controls.Add(this.textCountProb);
			this.groupBox2.Controls.Add(this.updownProb);
			this.groupBox2.Controls.Add(this.label13);
			this.groupBox2.Controls.Add(this.textRedProb);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.butCount);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(1003, 375);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(221, 201);
			this.groupBox2.TabIndex = 49;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Numbers in red";
			// 
			// textCountMob
			// 
			this.textCountMob.Location = new System.Drawing.Point(148, 170);
			this.textCountMob.Name = "textCountMob";
			this.textCountMob.ReadOnly = true;
			this.textCountMob.Size = new System.Drawing.Size(34, 20);
			this.textCountMob.TabIndex = 26;
			// 
			// updownMob
			// 
			this.updownMob.InterceptArrowKeys = false;
			this.updownMob.Location = new System.Drawing.Point(104, 170);
			this.updownMob.Name = "updownMob";
			this.updownMob.Size = new System.Drawing.Size(19, 20);
			this.updownMob.TabIndex = 25;
			this.updownMob.MouseDown += new System.Windows.Forms.MouseEventHandler(this.updownRed_MouseDown);
			// 
			// textRedMob
			// 
			this.textRedMob.Location = new System.Drawing.Point(77, 170);
			this.textRedMob.Name = "textRedMob";
			this.textRedMob.ReadOnly = true;
			this.textRedMob.Size = new System.Drawing.Size(27, 20);
			this.textRedMob.TabIndex = 24;
			// 
			// textCountFurc
			// 
			this.textCountFurc.Location = new System.Drawing.Point(148, 150);
			this.textCountFurc.Name = "textCountFurc";
			this.textCountFurc.ReadOnly = true;
			this.textCountFurc.Size = new System.Drawing.Size(34, 20);
			this.textCountFurc.TabIndex = 23;
			// 
			// updownFurc
			// 
			this.updownFurc.InterceptArrowKeys = false;
			this.updownFurc.Location = new System.Drawing.Point(104, 150);
			this.updownFurc.Name = "updownFurc";
			this.updownFurc.Size = new System.Drawing.Size(19, 20);
			this.updownFurc.TabIndex = 22;
			this.updownFurc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.updownRed_MouseDown);
			// 
			// textRedFurc
			// 
			this.textRedFurc.Location = new System.Drawing.Point(77, 150);
			this.textRedFurc.Name = "textRedFurc";
			this.textRedFurc.ReadOnly = true;
			this.textRedFurc.Size = new System.Drawing.Size(27, 20);
			this.textRedFurc.TabIndex = 21;
			// 
			// textCountCAL
			// 
			this.textCountCAL.Location = new System.Drawing.Point(148, 130);
			this.textCountCAL.Name = "textCountCAL";
			this.textCountCAL.ReadOnly = true;
			this.textCountCAL.Size = new System.Drawing.Size(34, 20);
			this.textCountCAL.TabIndex = 20;
			// 
			// updownCAL
			// 
			this.updownCAL.InterceptArrowKeys = false;
			this.updownCAL.Location = new System.Drawing.Point(104, 130);
			this.updownCAL.Name = "updownCAL";
			this.updownCAL.Size = new System.Drawing.Size(19, 20);
			this.updownCAL.TabIndex = 19;
			this.updownCAL.MouseDown += new System.Windows.Forms.MouseEventHandler(this.updownRed_MouseDown);
			// 
			// textRedCAL
			// 
			this.textRedCAL.Location = new System.Drawing.Point(77, 130);
			this.textRedCAL.Name = "textRedCAL";
			this.textRedCAL.ReadOnly = true;
			this.textRedCAL.Size = new System.Drawing.Size(27, 20);
			this.textRedCAL.TabIndex = 18;
			// 
			// textCountGing
			// 
			this.textCountGing.Location = new System.Drawing.Point(148, 110);
			this.textCountGing.Name = "textCountGing";
			this.textCountGing.ReadOnly = true;
			this.textCountGing.Size = new System.Drawing.Size(34, 20);
			this.textCountGing.TabIndex = 17;
			// 
			// updownGing
			// 
			this.updownGing.InterceptArrowKeys = false;
			this.updownGing.Location = new System.Drawing.Point(104, 110);
			this.updownGing.Name = "updownGing";
			this.updownGing.Size = new System.Drawing.Size(19, 20);
			this.updownGing.TabIndex = 16;
			this.updownGing.MouseDown += new System.Windows.Forms.MouseEventHandler(this.updownRed_MouseDown);
			// 
			// textRedGing
			// 
			this.textRedGing.Location = new System.Drawing.Point(77, 110);
			this.textRedGing.Name = "textRedGing";
			this.textRedGing.ReadOnly = true;
			this.textRedGing.Size = new System.Drawing.Size(27, 20);
			this.textRedGing.TabIndex = 15;
			// 
			// textCountMGJ
			// 
			this.textCountMGJ.Location = new System.Drawing.Point(148, 90);
			this.textCountMGJ.Name = "textCountMGJ";
			this.textCountMGJ.ReadOnly = true;
			this.textCountMGJ.Size = new System.Drawing.Size(34, 20);
			this.textCountMGJ.TabIndex = 14;
			// 
			// updownMGJ
			// 
			this.updownMGJ.InterceptArrowKeys = false;
			this.updownMGJ.Location = new System.Drawing.Point(104, 90);
			this.updownMGJ.Name = "updownMGJ";
			this.updownMGJ.Size = new System.Drawing.Size(19, 20);
			this.updownMGJ.TabIndex = 13;
			this.updownMGJ.MouseDown += new System.Windows.Forms.MouseEventHandler(this.updownRed_MouseDown);
			// 
			// textRedMGJ
			// 
			this.textRedMGJ.Location = new System.Drawing.Point(77, 90);
			this.textRedMGJ.Name = "textRedMGJ";
			this.textRedMGJ.ReadOnly = true;
			this.textRedMGJ.Size = new System.Drawing.Size(27, 20);
			this.textRedMGJ.TabIndex = 12;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(132, 49);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(52, 16);
			this.label14.TabIndex = 11;
			this.label14.Text = "# Teeth";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCountProb
			// 
			this.textCountProb.Location = new System.Drawing.Point(148, 70);
			this.textCountProb.Name = "textCountProb";
			this.textCountProb.ReadOnly = true;
			this.textCountProb.Size = new System.Drawing.Size(34, 20);
			this.textCountProb.TabIndex = 10;
			// 
			// updownProb
			// 
			this.updownProb.InterceptArrowKeys = false;
			this.updownProb.Location = new System.Drawing.Point(104, 70);
			this.updownProb.Name = "updownProb";
			this.updownProb.Size = new System.Drawing.Size(19, 20);
			this.updownProb.TabIndex = 9;
			this.updownProb.MouseDown += new System.Windows.Forms.MouseEventHandler(this.updownRed_MouseDown);
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(14, 50);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(84, 16);
			this.label13.TabIndex = 8;
			this.label13.Text = "Red if >=";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRedProb
			// 
			this.textRedProb.Location = new System.Drawing.Point(77, 70);
			this.textRedProb.Name = "textRedProb";
			this.textRedProb.ReadOnly = true;
			this.textRedProb.Size = new System.Drawing.Size(27, 20);
			this.textRedProb.TabIndex = 0;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(13, 152);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(64, 16);
			this.label12.TabIndex = 7;
			this.label12.Text = "Furc";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(13, 172);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(64, 16);
			this.label7.TabIndex = 6;
			this.label7.Text = "Mobility";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(13, 132);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(64, 16);
			this.label11.TabIndex = 5;
			this.label11.Text = "CAL";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(13, 112);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(64, 16);
			this.label10.TabIndex = 4;
			this.label10.Text = "Ging Marg";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(13, 92);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(64, 16);
			this.label9.TabIndex = 3;
			this.label9.Text = "MGJ (<=)";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(13, 72);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(64, 16);
			this.label8.TabIndex = 2;
			this.label8.Text = "Probing";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(998, 645);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(123, 42);
			this.label6.TabIndex = 54;
			this.label6.Text = "(All exams are saved automatically)";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textIndexPlaque
			// 
			this.textIndexPlaque.Location = new System.Drawing.Point(1109, 249);
			this.textIndexPlaque.Name = "textIndexPlaque";
			this.textIndexPlaque.ReadOnly = true;
			this.textIndexPlaque.Size = new System.Drawing.Size(38, 20);
			this.textIndexPlaque.TabIndex = 70;
			this.textIndexPlaque.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textIndexSupp
			// 
			this.textIndexSupp.Location = new System.Drawing.Point(1109, 339);
			this.textIndexSupp.Name = "textIndexSupp";
			this.textIndexSupp.ReadOnly = true;
			this.textIndexSupp.Size = new System.Drawing.Size(38, 20);
			this.textIndexSupp.TabIndex = 71;
			this.textIndexSupp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textIndexBleeding
			// 
			this.textIndexBleeding.Location = new System.Drawing.Point(1109, 309);
			this.textIndexBleeding.Name = "textIndexBleeding";
			this.textIndexBleeding.ReadOnly = true;
			this.textIndexBleeding.Size = new System.Drawing.Size(38, 20);
			this.textIndexBleeding.TabIndex = 72;
			this.textIndexBleeding.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textIndexCalculus
			// 
			this.textIndexCalculus.Location = new System.Drawing.Point(1109, 279);
			this.textIndexCalculus.Name = "textIndexCalculus";
			this.textIndexCalculus.ReadOnly = true;
			this.textIndexCalculus.Size = new System.Drawing.Size(38, 20);
			this.textIndexCalculus.TabIndex = 73;
			this.textIndexCalculus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// printPreviewDlg
			// 
			this.printPreviewDlg.AutoScrollMargin = new System.Drawing.Size(0, 0);
			this.printPreviewDlg.AutoScrollMinSize = new System.Drawing.Size(0, 0);
			this.printPreviewDlg.ClientSize = new System.Drawing.Size(400, 300);
			this.printPreviewDlg.Enabled = true;
			this.printPreviewDlg.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDlg.Icon")));
			this.printPreviewDlg.Name = "printPreviewDlg";
			this.printPreviewDlg.Visible = false;
			// 
			// labelPlaqueHistory
			// 
			this.labelPlaqueHistory.Location = new System.Drawing.Point(14, 521);
			this.labelPlaqueHistory.Name = "labelPlaqueHistory";
			this.labelPlaqueHistory.Size = new System.Drawing.Size(126, 19);
			this.labelPlaqueHistory.TabIndex = 78;
			this.labelPlaqueHistory.Text = "Plaque Index History";
			this.labelPlaqueHistory.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.labelPlaqueHistory.Visible = false;
			// 
			// listPlaqueHistory
			// 
			this.listPlaqueHistory.Location = new System.Drawing.Point(16, 543);
			this.listPlaqueHistory.Name = "listPlaqueHistory";
			this.listPlaqueHistory.SelectionMode = UI.SelectionMode.None;
			this.listPlaqueHistory.Size = new System.Drawing.Size(124, 130);
			this.listPlaqueHistory.TabIndex = 79;
			this.listPlaqueHistory.Visible = false;
			// 
			// textInputBox
			// 
			this.textInputBox.Location = new System.Drawing.Point(390, 8);
			this.textInputBox.Name = "textInputBox";
			this.textInputBox.Size = new System.Drawing.Size(53, 20);
			this.textInputBox.TabIndex = 81;
			this.textInputBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.textInputBox.TextChanged += new System.EventHandler(this.textInputBox_TextChanged);
			this.textInputBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textInputBox_KeyDown);
			this.textInputBox.Leave += new System.EventHandler(this.textInputBox_Leave);
			// 
			// butCopyPrevious
			// 
			this.butCopyPrevious.Icon = OpenDental.UI.EnumIcons.Add;
			this.butCopyPrevious.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCopyPrevious.Location = new System.Drawing.Point(100, 218);
			this.butCopyPrevious.Name = "butCopyPrevious";
			this.butCopyPrevious.Size = new System.Drawing.Size(77, 24);
			this.butCopyPrevious.TabIndex = 1;
			this.butCopyPrevious.Text = "Copy";
			this.butCopyPrevious.Click += new System.EventHandler(this.butCopyPrevious_Click);
			// 
			// gridP
			// 
			this.gridP.BackColor = System.Drawing.SystemColors.Window;
			this.gridP.DoShowCurrentExamOnly = true;
			this.gridP.Location = new System.Drawing.Point(390, 8);
			this.gridP.Name = "gridP";
			this.gridP.SelectedExam = 0;
			this.gridP.Size = new System.Drawing.Size(608, 685);
			this.gridP.TabIndex = 75;
			this.gridP.Text = "contrPerio2";
			this.gridP.DirectionChangedRight += new System.EventHandler(this.gridP_DirectionChangedRight);
			this.gridP.DirectionChangedLeft += new System.EventHandler(this.gridP_DirectionChangedLeft);
			this.gridP.Click += new System.EventHandler(this.gridP_Click);
			// 
			// butSave
			// 
			this.butSave.Location = new System.Drawing.Point(1005, 620);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(88, 24);
			this.butSave.TabIndex = 21;
			this.butSave.Text = "Save to Images";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butGraphical
			// 
			this.butGraphical.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butGraphical.Location = new System.Drawing.Point(1126, 584);
			this.butGraphical.Name = "butGraphical";
			this.butGraphical.Size = new System.Drawing.Size(75, 24);
			this.butGraphical.TabIndex = 20;
			this.butGraphical.Text = "Graphical";
			this.butGraphical.Click += new System.EventHandler(this.butGraphical_Click);
			// 
			// butPrint
			// 
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(1126, 620);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 24);
			this.butPrint.TabIndex = 22;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butAdd
			// 
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(17, 219);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(77, 24);
			this.butAdd.TabIndex = 0;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// but0
			// 
			this.but0.Location = new System.Drawing.Point(1004, 179);
			this.but0.Name = "but0";
			this.but0.Size = new System.Drawing.Size(67, 32);
			this.but0.TabIndex = 13;
			this.but0.Text = "0";
			this.but0.Click += new System.EventHandler(this.but0_Click);
			// 
			// butDelete
			// 
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(291, 219);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(77, 24);
			this.butDelete.TabIndex = 2;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// but8
			// 
			this.but8.Location = new System.Drawing.Point(1039, 74);
			this.but8.Name = "but8";
			this.but8.Size = new System.Drawing.Size(32, 32);
			this.but8.TabIndex = 11;
			this.but8.Text = "8";
			this.but8.Click += new System.EventHandler(this.but8_Click);
			// 
			// but4
			// 
			this.but4.Location = new System.Drawing.Point(1004, 109);
			this.but4.Name = "but4";
			this.but4.Size = new System.Drawing.Size(32, 32);
			this.but4.TabIndex = 7;
			this.but4.Text = "4";
			this.but4.Click += new System.EventHandler(this.but4_Click);
			// 
			// but5
			// 
			this.but5.Location = new System.Drawing.Point(1039, 109);
			this.but5.Name = "but5";
			this.but5.Size = new System.Drawing.Size(32, 32);
			this.but5.TabIndex = 8;
			this.but5.Text = "5";
			this.but5.Click += new System.EventHandler(this.but5_Click);
			// 
			// but9
			// 
			this.but9.Location = new System.Drawing.Point(1074, 74);
			this.but9.Name = "but9";
			this.but9.Size = new System.Drawing.Size(32, 32);
			this.but9.TabIndex = 12;
			this.but9.Text = "9";
			this.but9.Click += new System.EventHandler(this.but9_Click);
			// 
			// but6
			// 
			this.but6.Location = new System.Drawing.Point(1074, 109);
			this.but6.Name = "but6";
			this.but6.Size = new System.Drawing.Size(32, 32);
			this.but6.TabIndex = 9;
			this.but6.Text = "6";
			this.but6.Click += new System.EventHandler(this.but6_Click);
			// 
			// but1
			// 
			this.but1.Location = new System.Drawing.Point(1004, 144);
			this.but1.Name = "but1";
			this.but1.Size = new System.Drawing.Size(32, 32);
			this.but1.TabIndex = 4;
			this.but1.Text = "1";
			this.but1.Click += new System.EventHandler(this.but1_Click);
			// 
			// but2
			// 
			this.but2.Location = new System.Drawing.Point(1039, 144);
			this.but2.Name = "but2";
			this.but2.Size = new System.Drawing.Size(32, 32);
			this.but2.TabIndex = 5;
			this.but2.Text = "2";
			this.but2.Click += new System.EventHandler(this.but2_Click);
			// 
			// but3
			// 
			this.but3.Location = new System.Drawing.Point(1074, 144);
			this.but3.Name = "but3";
			this.but3.Size = new System.Drawing.Size(32, 32);
			this.but3.TabIndex = 6;
			this.but3.Text = "3";
			this.but3.Click += new System.EventHandler(this.but3_Click);
			// 
			// but7
			// 
			this.but7.Location = new System.Drawing.Point(1004, 74);
			this.but7.Name = "but7";
			this.but7.Size = new System.Drawing.Size(32, 32);
			this.but7.TabIndex = 10;
			this.but7.Text = "7";
			this.but7.Click += new System.EventHandler(this.but7_Click);
			// 
			// butClose
			// 
			this.butClose.Location = new System.Drawing.Point(1126, 662);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 23;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butListen
			// 
			this.butListen.Image = global::OpenDental.Properties.Resources.Microphone_22px;
			this.butListen.Location = new System.Drawing.Point(17, 248);
			this.butListen.Name = "butListen";
			this.butListen.Size = new System.Drawing.Size(30, 30);
			this.butListen.TabIndex = 3;
			this.butListen.Click += new System.EventHandler(this.butListen_Click);
			// 
			// labelListening
			// 
			this.labelListening.ForeColor = System.Drawing.Color.ForestGreen;
			this.labelListening.Location = new System.Drawing.Point(52, 254);
			this.labelListening.Name = "labelListening";
			this.labelListening.Size = new System.Drawing.Size(98, 19);
			this.labelListening.TabIndex = 84;
			this.labelListening.Text = "Listening";
			this.labelListening.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelListening.Visible = false;
			// 
			// gridODExam
			// 
			this.gridODExam.Location = new System.Drawing.Point(17, 13);
			this.gridODExam.Name = "gridODExam";
			this.gridODExam.Size = new System.Drawing.Size(351, 199);
			this.gridODExam.TabIndex = 85;
			this.gridODExam.Title = "Exams";
			this.gridODExam.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridODExam_CellDoubleClick);
			this.gridODExam.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridODExam_CellClick);
			// 
			// labelNotes
			// 
			this.labelNotes.AutoSize = true;
			this.labelNotes.Location = new System.Drawing.Point(14, 286);
			this.labelNotes.Name = "labelNotes";
			this.labelNotes.Size = new System.Drawing.Size(64, 14);
			this.labelNotes.TabIndex = 87;
			this.labelNotes.Text = "Exam Notes";
			// 
			// textExamNotes
			// 
			this.textExamNotes.AcceptsTab = true;
			this.textExamNotes.BackColor = System.Drawing.SystemColors.Window;
			this.textExamNotes.DetectLinksEnabled = false;
			this.textExamNotes.DetectUrls = false;
			this.textExamNotes.EditMode = true;
			this.textExamNotes.HasAutoNotes = true;
			this.textExamNotes.Location = new System.Drawing.Point(17, 303);
			this.textExamNotes.Name = "textExamNotes";
			this.textExamNotes.QuickPasteType = OpenDentBusiness.QuickPasteType.Procedure;
			this.textExamNotes.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textExamNotes.Size = new System.Drawing.Size(351, 215);
			this.textExamNotes.TabIndex = 88;
			this.textExamNotes.Text = "";
			this.textExamNotes.Leave += new System.EventHandler(this.textExamNotes_Leave);
			// 
			// butCopyNote
			// 
			this.butCopyNote.Icon = OpenDental.UI.EnumIcons.Add;
			this.butCopyNote.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCopyNote.Location = new System.Drawing.Point(271, 525);
			this.butCopyNote.Name = "butCopyNote";
			this.butCopyNote.Size = new System.Drawing.Size(97, 24);
			this.butCopyNote.TabIndex = 89;
			this.butCopyNote.Text = "Copy Note";
			this.butCopyNote.Click += new System.EventHandler(this.butCopyNote_Click);
			// 
			// FormPerio
			// 
			this.ClientSize = new System.Drawing.Size(1231, 700);
			this.Controls.Add(this.butCopyNote);
			this.Controls.Add(this.textExamNotes);
			this.Controls.Add(this.labelNotes);
			this.Controls.Add(this.gridODExam);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.labelListening);
			this.Controls.Add(this.butListen);
			this.Controls.Add(this.checkShowCurrent);
			this.Controls.Add(this.butCopyPrevious);
			this.Controls.Add(this.gridP);
			this.Controls.Add(this.textInputBox);
			this.Controls.Add(this.checkGingMarg);
			this.Controls.Add(this.listPlaqueHistory);
			this.Controls.Add(this.labelPlaqueHistory);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.butGraphical);
			this.Controls.Add(this.butCalcIndex);
			this.Controls.Add(this.textIndexCalculus);
			this.Controls.Add(this.textIndexBleeding);
			this.Controls.Add(this.textIndexSupp);
			this.Controls.Add(this.textIndexPlaque);
			this.Controls.Add(this.butColorCalculus);
			this.Controls.Add(this.butColorPlaque);
			this.Controls.Add(this.butCalculus);
			this.Controls.Add(this.butPlaque);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butSkip);
			this.Controls.Add(this.checkThree);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butColorPus);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.butColorBleed);
			this.Controls.Add(this.butPus);
			this.Controls.Add(this.butBleed);
			this.Controls.Add(this.but10);
			this.Controls.Add(this.but0);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.but8);
			this.Controls.Add(this.but4);
			this.Controls.Add(this.but5);
			this.Controls.Add(this.but9);
			this.Controls.Add(this.but6);
			this.Controls.Add(this.but1);
			this.Controls.Add(this.but2);
			this.Controls.Add(this.but3);
			this.Controls.Add(this.but7);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPerio";
			this.ShowInTaskbar = false;
			this.Text = "Perio Chart";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormPerio_Closing);
			this.Load += new System.EventHandler(this.FormPerio_Load);
			this.Shown += new System.EventHandler(this.FormPerio_Shown);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		#region Controls
		private OpenDental.UI.Button but7;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioLeft;
		private System.Windows.Forms.RadioButton radioRight;
		private OpenDental.UI.Button but3;
		private OpenDental.UI.Button but2;
		private OpenDental.UI.Button but1;
		private OpenDental.UI.Button but6;
		private OpenDental.UI.Button but9;
		private OpenDental.UI.Button but5;
		private OpenDental.UI.Button but4;
		private OpenDental.UI.Button but8;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button butColorBleed;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button butColorPus;
		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butAdd;
		private System.Windows.Forms.Label label6;
		private OpenDental.UI.Button but0;
		private OpenDental.UI.Button but10;
		private OpenDental.UI.Button butBleed;
		private OpenDental.UI.Button butPus;
		private System.Windows.Forms.CheckBox checkThree;
		private OpenDental.UI.Button butSkip;
		private OpenDental.UI.Button butPrint;
		private System.Windows.Forms.Button butColorCalculus;
		private System.Windows.Forms.Button butColorPlaque;
		private OpenDental.UI.Button butCalculus;
		private OpenDental.UI.Button butPlaque;
		private System.Windows.Forms.TextBox textIndexPlaque;
		private System.Windows.Forms.TextBox textIndexSupp;
		private System.Windows.Forms.TextBox textIndexBleeding;
		private System.Windows.Forms.TextBox textIndexCalculus;
		private OpenDental.UI.Button butCalcIndex;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textRedProb;
		private OpenDental.UI.Button butCount;
		private System.Windows.Forms.DomainUpDown updownProb;
		private System.Windows.Forms.TextBox textCountProb;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox textCountMGJ;
		private System.Windows.Forms.DomainUpDown updownMGJ;
		private System.Windows.Forms.TextBox textRedMGJ;
		private System.Windows.Forms.TextBox textCountGing;
		private System.Windows.Forms.DomainUpDown updownGing;
		private System.Windows.Forms.TextBox textRedGing;
		private System.Windows.Forms.TextBox textCountCAL;
		private System.Windows.Forms.DomainUpDown updownCAL;
		private System.Windows.Forms.TextBox textRedCAL;
		private System.Windows.Forms.TextBox textCountFurc;
		private System.Windows.Forms.DomainUpDown updownFurc;
		private System.Windows.Forms.TextBox textRedFurc;
		private System.Windows.Forms.TextBox textCountMob;
		private System.Windows.Forms.DomainUpDown updownMob;
		private System.Windows.Forms.TextBox textRedMob;
		private RadioButton radioCustom;
		private System.Windows.Forms.PrintDialog printDialog2;
		private System.Windows.Forms.PrintPreviewDialog printPreviewDlg;
		private OpenDental.UI.Button butGraphical;
		private OpenDental.UI.Button butSave;
		private Label labelPlaqueHistory;
		private OpenDental.UI.ListBoxOD listPlaqueHistory;
		private CheckBox checkGingMarg;
		private TextBox textInputBox;
		private UI.Button butCopyPrevious;
		private UI.Button butListen;
		private Label labelListening;
		private CheckBox checkShowCurrent;
		private UI.GridOD gridODExam;
		private Label labelNotes;
		private ODtextBox textExamNotes;
		private UI.Button butCopyNote;
		private OpenDental.ContrPerio gridP;
		/// <summary>This control is located behind gridP in the upper left corner.  It is used to allow text voice activated perio charting.  This also allows text to be pasted in the perio chart.</summary>
		//private OpenDental.ContrPerio gridP;
		//private OpenDental.ContrPerio contrPerio1;
		#endregion Controls
	}
}
