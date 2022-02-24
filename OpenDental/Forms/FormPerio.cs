using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI.Voice;
using OpenDentBusiness;
using SparksToothChart;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormPerio : FormODBase {
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
		private System.ComponentModel.IContainer components;
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
		//private OpenDental.ContrPerio gridP;
		//private OpenDental.ContrPerio contrPerio1;
		private OpenDental.ContrPerio gridP;
		private System.Windows.Forms.PrintDialog printDialog2;
		private System.Windows.Forms.PrintPreviewDialog printPreviewDlg;
		private OpenDental.UI.Button butGraphical;
		private OpenDental.UI.Button butSave;
		private Label labelPlaqueHistory;
		private OpenDental.UI.ListBoxOD listPlaqueHistory;
		private CheckBox checkGingMarg;
		/// <summary>This control is located behind gridP in the upper left corner.  It is used to allow text voice activated perio charting.  This also allows text to be pasted in the perio chart.</summary>
		private TextBox textInputBox;
		private UI.Button butCopyPrevious;
		private UI.Button butListen;
		private Label labelListening;
		private CheckBox checkShowCurrent;
		private UI.GridOD gridODExam;
		private Label labelNotes;
		private ODtextBox textExamNotes;
		private UI.Button butCopyNote;
		#endregion Controls

		#region Fields
		private bool localDefsChanged;
		private bool TenIsDown;
		//private int pagesPrinted;
		///<summary>Gets a list of missing teeth as strings on load. Includes "1"-"32", and "A"-"Z".</summary>
		private List<string> _listMissingTeeth;
		private Patient PatCur;
		///<summary>This is not a list of valid procedures.  The only values to be trusted in this list are the ToothNum and CodeNum.  Never used.</summary>
		private List<Procedure> _listProcedures;
		private PerioExam PerioExamCur;
		private UserOdPref _userPrefCurrentOnly;
		private UserOdPref _userPrefCustomAdvance;
		private VoiceController _voiceController;
		private PerioCell _prevLocation;
		private PerioCell _curLocation;
		private List<Def> _listMiscColorDefs;
		#endregion Fields

		///<summary></summary>
		public FormPerio(Patient patCur,List<Procedure> listProcedures)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			PatCur=patCur;
			_listProcedures=listProcedures;
			Lan.F(this);
		}

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

		private void FormPerio_Load(object sender, System.EventArgs e) {
			_listMiscColorDefs=Defs.GetDefsForCategory(DefCat.MiscColors,true);
			butColorBleed.BackColor=_listMiscColorDefs[(int)DefCatMiscColors.PerioBleeding].ItemColor;
			butColorPus.BackColor=_listMiscColorDefs[(int)DefCatMiscColors.PerioSuppuration].ItemColor;
			butColorPlaque.BackColor=_listMiscColorDefs[(int)DefCatMiscColors.PerioPlaque].ItemColor;
			butColorCalculus.BackColor=_listMiscColorDefs[(int)DefCatMiscColors.PerioCalculus].ItemColor;
			textRedProb.Text=PrefC.GetString(PrefName.PerioRedProb);
			textRedMGJ.Text =PrefC.GetString(PrefName.PerioRedMGJ);
			textRedGing.Text=PrefC.GetString(PrefName.PerioRedGing);
			textRedCAL.Text =PrefC.GetString(PrefName.PerioRedCAL);
			textRedFurc.Text=PrefC.GetString(PrefName.PerioRedFurc);
			textRedMob.Text =PrefC.GetString(PrefName.PerioRedMob);
			//Procedure[] procList=Procedures.Refresh(PatCur.PatNum);
			List<ToothInitial> initialList=ToothInitials.Refresh(PatCur.PatNum);
			_listMissingTeeth=ToothInitials.GetMissingOrHiddenTeeth(initialList);
			RefreshListExams();
			if(Programs.UsingOrion) {
				labelPlaqueHistory.Visible=true;
				listPlaqueHistory.Visible=true;
				RefreshListPlaque();
			}
			gridODExam.SetSelected(PerioExams.ListExams.Count-1,true);//this works even if no items.
			_userPrefCurrentOnly=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.PerioCurrentExamOnly).FirstOrDefault();
			if(_userPrefCurrentOnly != null && PIn.Bool(_userPrefCurrentOnly.ValueString)) {
				checkShowCurrent.Checked=true;
			}
			_userPrefCustomAdvance=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.PerioAutoAdvanceCustom).FirstOrDefault();
			if(_userPrefCustomAdvance!=null && PIn.Bool(_userPrefCustomAdvance.ValueString)) {
				radioCustom.Checked=true;
				gridP.Direction=AutoAdvanceDirection.Custom;
			}
			FillGrid();
			Plugins.HookAddCode(this,"FormPerio.Load_end");
		}

		///<summary>Goes to the first cell in the grid for this tooth.</summary>
		///<param name="isFacial">When false, equivalent to lingual.</param>
		private void GoToTooth(int toothNum,bool isFacial) {
			Point toothPoint=GetPointForTooth(toothNum,isFacial);
			gridP.SetNewCell(toothPoint.X,toothPoint.Y);
		}

		private void GoToToothSurface(int toothNum,bool isFacial,PerioSurface perioSurf) {
			Point toothPoint=GetSurfacePointForTooth(toothNum,isFacial,perioSurf);
			gridP.SetNewCell(toothPoint.X,toothPoint.Y);
		}

		///<summary>Sets the cursor to the first available open perio slot. The path used in this method follows the autoadvance i.e. 1-16 F,16-1 L,32-17 F, and 17-32 L</summary>
		private void ResumePath() {
			List<int> listSkippedTeeth=new List<int>();//int 1-32
			if(PerioExams.ListExams.Count > 0) {
				//set skipped teeth based on the last exam in the list: 
				listSkippedTeeth=PerioMeasures.GetSkipped(PerioExams.ListExams[PerioExams.ListExams.Count-1].PerioExamNum);
			}
			List<AutoAdvanceCustom> listPaths=AutoAdvanceCustom.GetDefaultPath();
			if(radioCustom.Checked) {
				listPaths=AutoAdvanceCustom.GetCustomPaths();
			}
			Point nextAvailablePoint;
			foreach(AutoAdvanceCustom tooth in listPaths) {
				if(tooth==null || listSkippedTeeth.Contains(tooth.ToothNum)) {
					continue;
				}
				if(radioCustom.Checked) {//Auto advance custom.
					if(!TryGetNextAvailablePointForToothCustom(tooth,ContrPerio.GetNext(tooth,listPaths),ContrPerio.GetPrevious(tooth,listPaths),
						out nextAvailablePoint)) 
					{
						continue;
					}
				}
				else {//Auto advance NOT custom.
					if(!TryGetNextAvailablePointForTooth(tooth.ToothNum,tooth.Surface==PerioSurface.Facial,out nextAvailablePoint)) {
						continue;
					}
					if(tooth.Surface==PerioSurface.Facial) {
						gridP.Direction=AutoAdvanceDirection.Left;
						radioLeft.Checked=true;
					}
					else {//lingual
						gridP.Direction=AutoAdvanceDirection.Right;
						radioRight.Checked=true;
					}
				}
				gridP.SetNewCell(nextAvailablePoint.X,nextAvailablePoint.Y);
				return;
			}
		}

		///<summary>Returns true if the current tooth passed in is empty. The out param gets set to the position of the tooth that is empty.
		///Checks the tooth's 3 position to find out if the tooth is empty.Otherwise returns false.</summary>
		private bool TryGetNextAvailablePointForToothCustom(AutoAdvanceCustom pathCur,AutoAdvanceCustom pathNext,AutoAdvanceCustom pathPrev,
			out Point nextAvailablePoint)
		{
			if(pathCur==null) {//This shouldn't happen.
				nextAvailablePoint=GetPointForTooth(1,true);
				return false;
			}
			//Get the default point for the current tooth.
			Point pointForTooth=GetPointForTooth(pathCur.ToothNum,pathCur.Surface==PerioSurface.Facial);
			PerioSequenceType seqType=gridP.GetSequenceForPoint(pointForTooth);
			//Get the first position(mesial or distal) for the tooth passed in. We need this because the first position for a custom path will not be the 
			//same as the default(not custom).
			pointForTooth=gridP.GetAutoAdvanceCustomPoint(pathCur.ToothNum,pathCur.Surface,seqType,isReverse:false);
			//Check all three positions for the tooth. It will return either an empty position or the last position of the tooth.
			nextAvailablePoint=new Point(FirstEmptyPositionX(pointForTooth),pointForTooth.Y);
			//Check the position that was returned above for measurements. This will be the last position of the current tooth.
			if(string.IsNullOrEmpty(gridP.GetCellText(nextAvailablePoint.X,nextAvailablePoint.Y))) {
				return true;//Tooth is empty.
			}
			return false;//Tooth is not empty.
		}

		///<summary>Sets bleeding flag on the specified surface for the current tooth. If the cursor is on a blank space and the previous tooth has 
		///probing entries, it will put the bleeding flag on the previous tooth.</summary>
		private void SetBleedingFlagForSurface(PerioSurface perioSurface,BleedingFlags type) {
			int toothNum=_curLocation.ToothNum;
			bool isFacial=_curLocation.Surface==PerioSurface.Facial;
			Point pointForTooth=GetPointForTooth(toothNum,isFacial);
			if(gridP.IsCellTextEmpty(pointForTooth)) {
				//Get the previous tooth. Skipped teeth will get considered. 
				pointForTooth=GetPreviousToothPoint(toothNum,isFacial,false);
				isFacial=IsFacialFromPoint(pointForTooth);
				toothNum=GetToothNumFromPoint(pointForTooth);
			}
			if(perioSurface==PerioSurface.Facial) {
				//User specified the facial point of the tooth.
				isFacial=true;
			}
			else if(perioSurface==PerioSurface.Lingual) {
				//User specified the lingual point of the tooth.
				isFacial=false;
			}
			//Get the point for the surface passed in. 
			pointForTooth=GetSurfacePointForTooth(toothNum,isFacial,perioSurface);
			gridP.SetBleedingFlagForPoint(pointForTooth,type);
		}

		///<summary>Sets the approprate Auto Advance radio button.</summary>
		private void SetAutoAdvance() {
			if(gridP.Direction==AutoAdvanceDirection.Custom) {
				return;
			}
			if(_prevLocation.Surface==PerioSurface.Facial && _curLocation.Surface==PerioSurface.Lingual) {
				gridP.Direction=AutoAdvanceDirection.Right;
				radioRight.Checked=true;
			}
			if(_prevLocation.Surface==PerioSurface.Lingual && _curLocation.Surface==PerioSurface.Facial) {
				gridP.Direction=AutoAdvanceDirection.Left;
				radioLeft.Checked=true;
			}
		}

		///<summary>Marks the tooth as skipped.</summary>
		///<param name="doVerifyByVoice">If true, will verbally ask the user if they want to skip the tooth.</param>
		///<returns>True if the user does choose to skip the tooth.</returns>
		private bool SkipTooth(int toothNum,bool doVerifyByVoice=true) {
			if(doVerifyByVoice) {
				_voiceController?.StopListening();
				if(!VoiceMsgBox.Show("Mark tooth "+toothNum+" as skipped?",MsgBoxButtons.YesNo)) {
					this.BeginInvoke(() => _voiceController?.StartListening());//Invoking because recognition events will run when the main thread is free.
					return false;
				}
				this.BeginInvoke(() => _voiceController?.StartListening());//Invoking because recognition events will run when the main thread is free.
			}
			Point curPoint=gridP.CurCell;
			bool radioRightChecked=radioRight.Checked;
			gridP.SaveCurExam(PerioExamCur);
			int selectedExam=gridP.SelectedExam;
			List<int> listSkippedTeeth=new List<int>();//int 1-32
			if(PerioExams.ListExams.Count > 0) {
				//set skipped teeth based on the last exam in the list: 
				listSkippedTeeth=PerioMeasures.GetSkipped(PerioExams.ListExams[PerioExams.ListExams.Count-1].PerioExamNum);
			}
			if(!listSkippedTeeth.Contains(toothNum)) {
				listSkippedTeeth.Add(toothNum);
			}
			PerioMeasures.SetSkipped(PerioExamCur.PerioExamNum,listSkippedTeeth);
			RefreshListExams();
			gridODExam.SetSelected(selectedExam,true);
			FillGrid();
			if(_curLocation.ToothNum==toothNum) {//Go to the next tooth if the _curLocation is on the tooth being skipped. 
				Point nextTooth=GetNextToothPoint(toothNum,_curLocation.Surface==PerioSurface.Facial);
				gridP.SetNewCell(nextTooth.X,nextTooth.Y);
			}
			else {
				gridP.SetNewCell(curPoint.X,curPoint.Y);
			}
			if(gridP.Direction!=AutoAdvanceDirection.Custom) {
				radioRight.Checked=radioRightChecked;
				gridP.Direction=radioRightChecked ? AutoAdvanceDirection.Right : AutoAdvanceDirection.Left;
			}
			return true;
		}

		///<summary>Moves the cursor to the probing row for the current tooth.</summary>
		private void GoToProbing() {
			int yVal;
			if(_curLocation.Surface==PerioSurface.Facial) {
				if(_curLocation.ToothNum<=16) {
					if(checkShowCurrent.Checked) {
						yVal=5;
					}
					else {
						yVal=6-Math.Min(6,gridP.SelectedExam+1);
					}
				}
				else {//ToothNum >= 17
					if(checkShowCurrent.Checked) {
						yVal=37;
					}
					else {
						yVal=36+Math.Min(6,gridP.SelectedExam+1);
					}
				}
			}
			else {//Lingual
				if(_curLocation.ToothNum<=16) {
					if(checkShowCurrent.Checked) {
						yVal=15;
					}
					else {
						yVal=14+Math.Min(6,gridP.SelectedExam+1);
					}
				}
				else {//ToothNum >= 17
					if(checkShowCurrent.Checked) {
						yVal=26;
					}
					else {
						yVal=27-Math.Min(6,gridP.SelectedExam+1);
					}
				}
			}
			gridP.SetNewCell(FirstEmptyPositionX(_curLocation.ToothNum,yVal),yVal);
		}

		///<summary>Moves the cursor to the mobility row for the current tooth.</summary>
		private void GoToMobility() {
			PerioCell curLoc=gridP.GetCurrentCell();
			int toothNum=curLoc.ToothNum;
			bool isFacial=_curLocation.Surface==PerioSurface.Facial;
			if(gridP.IsCellTextEmpty(GetPointForTooth(toothNum,isFacial))) {
				toothNum=GetToothNumFromPoint(GetPreviousToothPoint(toothNum,isFacial));
			}
			Point point=GetPointForMobility(toothNum);
			if(!gridP.IsCellTextEmpty(point)) {
				point=GetPointForMobility(curLoc.ToothNum);
			}
			gridP.SetNewCell(point.X,point.Y);
		}

		private Point GetPointForMobility(int toothNum) {
			int yVal;
			int xVal;
			if(toothNum <= 16) {
				xVal=(toothNum-1)*3+2;//Middle cell
				yVal=10;
			}
			else {//ToothNum >= 17
				xVal=47-(toothNum-17)*3;
				yVal=32;
			}
			return new Point(xVal,yVal);
		}

		///<summary>Moves the cursor to the gingival margin row for the current tooth.</summary>
		private void GoToGingivalMargin() {
			GoToSequenceType(PerioSequenceType.GingMargin);
		}

		///<summary>Moves the cursor to the furcation row for the current tooth.</summary>
		private void GoToFurcation() {
			GoToSequenceType(PerioSequenceType.Furcation);
		}

		///<summary>Moves the cursor to the muco gingival junction row for the current tooth.</summary>
		private void GoToMGJ() {
			GoToSequenceType(PerioSequenceType.MGJ);
		}

		///<summary>Moves the cursor to the passed in SequenceType for the current tooth.</summary>
		private void GoToSequenceType(PerioSequenceType probeType) {
			int toothNum=_curLocation.ToothNum;
			AutoAdvanceDirection directionOld=gridP.Direction;
			bool isFacial=_curLocation.Surface==PerioSurface.Facial;
			Point toothPoint=GetPointForTooth(toothNum,isFacial);
			if(gridP.IsCellTextEmpty(toothPoint)) {
				toothPoint=GetPreviousToothPoint(toothNum,isFacial);
				isFacial=IsFacialFromPoint(toothPoint);
				toothNum=GetToothNumFromPoint(toothPoint);
			}
			int yPos=-1;
			switch(probeType) {
				case PerioSequenceType.Furcation:
					if(toothNum<=16) {
						yPos=(isFacial ? 9 : 12);
					}
					else {//ToothNum >= 17
						yPos=(isFacial ? 33 : 30);
					}
					break;
				case PerioSequenceType.GingMargin:
					if(toothNum<=16) {
						yPos=(isFacial ? 7 : 14);
					}
					else {//ToothNum >= 17
						yPos=(isFacial ? 35 : 28);
					}
					break;
				case PerioSequenceType.MGJ:
					if(toothNum<=16) {
						yPos=6;
					}
					else {//ToothNum >= 17
						yPos=(isFacial ? 36 : 27);
					}
					break;
			}
			int xPos;
			//create a new point with the updated Y pos for the passed in sequence type. 
			Point pointForSequenceType=new Point(toothPoint.X,yPos);
			//Teeth 1-32 F isReverse=false, Teeth 1-32 L isReverse=true;
			//Find the next available point for the sequence type.
			if(!TryGetNextEmptyPoint(pointForSequenceType,!isFacial,out Point nextAvailablePoint) 
				|| toothNum!=GetToothNumFromPoint(nextAvailablePoint)) 
			{
				//No positions are empty or the next available point is on a new tooth,. Use the xPos from the toothOld.
				xPos=GetPointForTooth(_curLocation.ToothNum,_curLocation.Surface==PerioSurface.Facial).X;
				gridP.Direction=directionOld;//change this value back just in case we changed it above.
			}
			else {
				//The new point for the sequence was changed. Set the new xPos.
				xPos=FirstEmptyPositionX(toothNum,yPos);
			}
			//Set the cursor with the new points for the sequence.
			gridP.SetNewCell(xPos,yPos);
		}

		private bool IsFacialFromPoint(Point point) {
			return gridP.GetPerioCellFromPoint(point).Surface==PerioSurface.Facial;
		}

		///<summary>>Gets the previous tooth point. Takes into account skipped teeth.</summary>
		private Point GetPreviousToothPoint(int toothOld,bool isFacial,bool doSetDirection=true) {
			gridP.TryFindNextCell(GetPointForTooth(toothOld,isFacial),true,out Point prevPoint,doSetDirection);
			return prevPoint;
		}

		///<summary>Gets the next tooth point.  Takes into account skipped teeth.</summary>
		private Point GetNextToothPoint(int toothOld,bool isFacial) {
			gridP.TryFindNextCell(GetPointForTooth(toothOld,isFacial),false,out Point nextPoint);
			return nextPoint;
		}

		///<summary>Returns a point for the tooth's surface passed in.</summary>
		private Point GetSurfacePointForTooth(int toothNum,bool isFacial,PerioSurface perioSurf) {
			Point pointCur=GetPointForTooth(toothNum,isFacial);
			//Get set to either the right(Lingual) or left(Facial) position.
			int xPos=pointCur.X;
			//Midline=Looking at the Perio Chart(FormPerio.cs designer), this is the vertial line down the middle.
			//Facial and Lingual= will always be the middle cell of each of the cells in the perio chart.
			//Mesial=This is the position closest to the Midline. Teeth 1-8 and 25-32 right cell, teeth 9-24 left cell.
			//Distal=This is the postion furthest away from the midline. Teeth 1-8 and 25-32 left cell, teeth 9-24 right cell.
			switch(perioSurf) {
				case PerioSurface.Distal:
					if(toothNum.Between(9,24)) {
						//Get the right position.
						xPos=xPos + (isFacial ? 2 : 0);
					}
					else {
						//Teeth 1-8 and 25-32. Get the left position.
						xPos=xPos + (isFacial ? 0 : -2);
					}
					break;
				case PerioSurface.Facial:
				case PerioSurface.Lingual:
					xPos=xPos + (isFacial ? 1 : -1);//middle position
					break;
				case PerioSurface.Mesial:
					if(toothNum.Between(9,24)) {
						//Get the left position.
						xPos=xPos + (isFacial ? 0 : -2);
					}
					else {
						//Teeth 1-8 and 25-32 right position
						xPos=xPos + (isFacial ? 2 : 0);
					}
					break;
			}
			return new Point(xPos,pointCur.Y);
		}

		///<summary>Returns a point for the tooth's passed in.</summary>
		private Point GetPointForTooth(int toothNum,bool isFacial) {
			int x=-1;
			int y=-1;
			bool isLingual=!isFacial;
			if(isFacial && toothNum.Between(1,16)) {
				x=(toothNum)*3-2;
				if (checkShowCurrent.Checked) {
					y=5;
				}
				else {
					y=6-Math.Min(6,gridP.SelectedExam+1);
				}
			}
			else if(isFacial && toothNum.Between(17,32)) {
				x=(32-toothNum)*3+1;
				if (checkShowCurrent.Checked) {
					y=37;
				}
				else {
					y=36+Math.Min(6,gridP.SelectedExam+1);
				}
			}
			else if(isLingual && toothNum.Between(1,16)) {
				x=(toothNum)*3;
				if (checkShowCurrent.Checked) {
					y=15;
				}
				else {
					y=14+Math.Min(6,gridP.SelectedExam+1);
				}
			}
			else {//if(isLingual && toothNum.Between(17,32)) {
				x=(32-toothNum)*3+3;
				if (checkShowCurrent.Checked) {
					y=26;
				}
				else {
					y=27-Math.Min(6,gridP.SelectedExam+1);
				}
			}
			return new Point(x,y);
		}

		private int GetToothNumFromPoint(Point toothPoint) {
			PerioCell perioCell=gridP.GetPerioCellFromPoint(toothPoint);
			return perioCell.ToothNum;
		}

		///<summary>Gets the next available tooth available. Returns true if there is an empty cell. Returns false if all cells are filled.</summary>
		private bool TryGetNextAvailablePointForTooth(int toothNum,bool isFacial,out Point nextAvailablePoint) {
			//Get the first point for the tooth passed in.
			Point pointForTooth=GetPointForTooth(toothNum,isFacial);
			//Check all three positions for the tooth. It will return either an empty position or the last position of the tooth.
			nextAvailablePoint=new Point(FirstEmptyPositionX(pointForTooth),pointForTooth.Y);
			//Check the position that was returned above for measurements.
			if(!string.IsNullOrEmpty(gridP.GetCellText(nextAvailablePoint.X,nextAvailablePoint.Y))) {
				return false;//Tooth is not empty.
			}
			return true;
		}

		///<summary>Gets the next available point. Returns true if there is an empty cell. Returns false if all cells are filled.</summary>
		private bool TryGetNextEmptyPoint(Point pointForTooth,bool isReverse,out Point nextAvailablePoint) {
			if(string.IsNullOrEmpty(gridP.GetCellText(pointForTooth.X,pointForTooth.Y))) {
				nextAvailablePoint=pointForTooth;
				return true;//Tooth is not empty.
			}
			nextAvailablePoint=NextEmptyPointHelper(pointForTooth,isReverse);
			return true;
		}

		///<summary>Uses the AdvanceCell method to figure out the.</summary>
		private Point NextEmptyPointHelper(Point pointForTooth,bool isReverse) {
			gridP.TryFindNextCell(pointForTooth,isReverse,out Point nextCell);
			if(string.IsNullOrEmpty(gridP.GetCellText(nextCell.X,nextCell.Y))) {
				return nextCell;//Tooth is not empty.
			}
			int toothNum=GetToothNumFromPoint(nextCell);
			if(toothNum<1 || toothNum>32) {//I think we need <=1 and >=32
				return pointForTooth;
			}
			if(pointForTooth==nextCell) {
				return pointForTooth;//We are probably trying to advance into a skipped tooth that's the last tooth.
			}
			return NextEmptyPointHelper(nextCell,isReverse);
		}

		///<summary>Returns the first non-empty cell for the tooth. If all cells in the tooth are filled, returns the last cell of the tooth.</summary>
		private int FirstEmptyPositionX(int toothNum,int yVal) {
			int xVal;
			if(_curLocation.Surface==PerioSurface.Facial) {
				if(toothNum<=16) {
					xVal=(toothNum-1)*3+1;
				}
				else {//ToothNum >= 17
					xVal=46-(toothNum-17)*3;
				}
				if(string.IsNullOrEmpty(gridP.GetCellText(xVal,yVal))) {
					return xVal;
				}
				xVal++;
				if(string.IsNullOrEmpty(gridP.GetCellText(xVal,yVal))) {
					return xVal;
				}
				return ++xVal;
			}
			else {//Lingual
				if(toothNum<=16) {
					xVal=(toothNum-1)*3+3;
				}
				else {//ToothNum >= 17
					xVal=48-(toothNum-17)*3;
				}
				if(string.IsNullOrEmpty(gridP.GetCellText(xVal,yVal))) {
					return xVal;
				}
				xVal--;
				if(string.IsNullOrEmpty(gridP.GetCellText(xVal,yVal))) {
					return xVal;
				}
				return --xVal;
			}
		}

		///<summary>Returns the first non-empty cell for the Point passed in.</summary>
		private int FirstEmptyPositionX(Point pointForTooth) {
			int xVal=pointForTooth.X;
			int yVal=pointForTooth.Y;
			if(string.IsNullOrEmpty(gridP.GetCellText(xVal,yVal))) {
				return xVal;
			}
			//First position is not empty. Get the 2nd position of the point passed in.
			Point nextCellPos;
			if(!gridP.TryFindNextCell(pointForTooth,false,out nextCellPos,doSetDirection: false)) {
				return xVal;
			}
			xVal=nextCellPos.X;
			if(string.IsNullOrEmpty(gridP.GetCellText(xVal,yVal))) {
				return xVal;
			}
			//The section position is not empty. Get the last position of the point passed in.
			gridP.TryFindNextCell(nextCellPos,false,out nextCellPos,doSetDirection: false);
			return nextCellPos.X;//this should be the last position of the point passed in.
		}

		/// <summary>Used to force focus to the hidden textbox when showing this form.</summary>
		private void FormPerio_Shown(object sender,EventArgs e) {
			textInputBox.Focus();//This cannot go into load because focus must come after window has been shown.
		}

		///<summary>After this method runs, the selected index is usually set.</summary>
		private void RefreshListExams(bool skipRefreshMeasures=false) {
			//most recent date at the bottom
			PerioExams.Refresh(PatCur.PatNum);
			if(!skipRefreshMeasures) {
				PerioMeasures.Refresh(PatCur.PatNum,PerioExams.ListExams);
			}
			gridODExam.BeginUpdate();
			gridODExam.ListGridColumns.Clear();
			gridODExam.ListGridRows.Clear();
			OpenDental.UI.GridColumn col;
			col=new OpenDental.UI.GridColumn(Lan.g("TablePerioExam","Exam Date"),75,HorizontalAlignment.Center);
			gridODExam.ListGridColumns.Add(col);
			col=new OpenDental.UI.GridColumn(Lan.g("TablePerioExam","Provider"),75,HorizontalAlignment.Center);
			gridODExam.ListGridColumns.Add(col);
			col=new OpenDental.UI.GridColumn(Lan.g("TablePerioExam","Exam Notes"),150,HorizontalAlignment.Center);
			gridODExam.ListGridColumns.Add(col);
			for(int i=0;i<PerioExams.ListExams.Count;i++) {
				OpenDental.UI.GridRow row=new OpenDental.UI.GridRow();
				row.Cells.Add(PerioExams.ListExams[i].ExamDate.ToShortDateString());
				row.Cells.Add(Providers.GetAbbr(PerioExams.ListExams[i].ProvNum));
				int index5thNewLine=GetIndexCount(PerioExams.ListExams[i].Note,"\n",5);//Our goal is to get no more than 5 lines into the text box.
				int noteEnd=90;//90 characters is about what it takes to get most strings to fit the 5 lines.
				if(PerioExams.ListExams[i].Note.Length > noteEnd || index5thNewLine > -1) {
					if(index5thNewLine > -1) {
						noteEnd=Math.Min(noteEnd,index5thNewLine);
					}
					row.Cells.Add(PerioExams.ListExams[i].Note.Substring(0,noteEnd)+"(...)");
				} 
				else {
					row.Cells.Add(PerioExams.ListExams[i].Note);
				}
				gridODExam.ListGridRows.Add(row);
			}
			gridODExam.EndUpdate();
		}

		///<summary>Gets the index of the given target string, within the value string, where the count is the number of matched iterations. Returns -1 if there are no matches or less than count matches.</summary>
		private int GetIndexCount(string value,string target,int count) {
			int countOccurrences=0;
			for(int i=0;i<value.Length-target.Length+1;i++) {
				if(value.Substring(i,target.Length)==target) {
					countOccurrences++;
					if(countOccurrences==count) {
						return i;
					}
				}
			}
			return -1;
		}

		///<summary>Orion only.</summary>
		private void RefreshListPlaque() {
			PerioExams.Refresh(PatCur.PatNum);
			PerioMeasures.Refresh(PatCur.PatNum,PerioExams.ListExams);
			listPlaqueHistory.Items.Clear();
			for(int i=0;i<PerioExams.ListExams.Count;i++) {
				string ph="";
				ph=PerioExams.ListExams[i].ExamDate.ToShortDateString()+"\t";
				gridP.SelectedExam=i;
				gridP.LoadData();
				ph+=gridP.ComputeOrionPlaqueIndex();
				listPlaqueHistory.Items.Add(ph);
			}
			//Not sure if necessary but set it back to what it was
			gridP.SelectedExam=gridODExam.GetSelectedIndex();
		}

		///<summary>Usually set the selected index first</summary>
		private void FillGrid(bool doSelectCell=true) {
			if(gridODExam.GetSelectedIndex()!=-1){
				gridP.perioEdit=true;
				if(!Security.IsAuthorized(Permissions.PerioEdit,PerioExams.ListExams[gridODExam.GetSelectedIndex()].ExamDate,true)) {
					gridP.perioEdit=false;
				}
				PerioExamCur=PerioExams.ListExams[gridODExam.GetSelectedIndex()];
			}
			gridP.SelectedExam=gridODExam.GetSelectedIndex();
			gridP.DoShowCurrentExamOnly=checkShowCurrent.Checked;
			gridP.LoadData(doSelectCell);
			FillIndexes();
			FillCounts();
			gridP.Invalidate();
			gridP.Focus();//this still doesn't seem to work to enable first arrow click to move
			if(PerioExamCur!=null) {
				textExamNotes.Text=PerioExamCur.Note;
			}
		}
		
		private void gridODExam_CellClick(object sender,UI.ODGridClickEventArgs e) {
			if(gridODExam.GetSelectedIndex()==gridP.SelectedExam) { 
				return;
			}
			//Only continues if clicked on other than current exam
			gridP.SaveCurExam(PerioExamCur);
			//no need to RefreshListExams because it has not changed
			PerioExams.Refresh(PatCur.PatNum);//refresh instead
			PerioMeasures.Refresh(PatCur.PatNum,PerioExams.ListExams);
			FillGrid();
		}

		private void gridODExam_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			//remember that the first click may not have triggered the mouse down routine
			//and the second click will never trigger it.
			if(gridODExam.GetSelectedIndex()==-1) {
				return;
			}
			if(!Security.IsAuthorized(Permissions.PerioEdit,PerioExams.ListExams[gridODExam.GetSelectedIndex()].ExamDate)) {
				return;
			}
			//a PerioExam.Cur will always have been set through mousedown(or similar),then FillGrid
			gridP.SaveCurExam(PerioExamCur);
			PerioExams.Refresh(PatCur.PatNum);//list will not change
			PerioMeasures.Refresh(PatCur.PatNum,PerioExams.ListExams);
			using FormPerioEdit FormPE=new FormPerioEdit();
			FormPE.PerioExamCur=PerioExamCur;
			FormPE.ShowDialog();
			int curIndex=gridODExam.GetSelectedIndex();
			RefreshListExams();
			gridODExam.SetSelected(curIndex,true);
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.PerioEdit,MiscData.GetNowDateTime())){
				return;
			}
			if(gridODExam.GetSelectedIndex()!=-1){
				gridP.SaveCurExam(PerioExamCur);
			}
			CreateNewPerioChart();
			RefreshListExams();
			gridODExam.SetSelected(gridODExam.ListGridRows.Count-1,true);
			FillGrid();
			SecurityLogs.MakeLogEntry(Permissions.PerioEdit,PatCur.PatNum,"Perio exam created");
		}

		private void butCopyNote_Click(object sender,EventArgs e) {
			if(gridODExam.GetSelectedIndex()!=-1) {
				ODClipboard.SetClipboard(textExamNotes.Text);
			}
		}

		///<summary>Creates a new perio chart and marks any teeth missing as necessary.</summary>
		private void CreateNewPerioChart() {
			PerioExamCur=new PerioExam();
			PerioExamCur.PatNum=PatCur.PatNum;
			PerioExamCur.ExamDate=DateTime.Today;
			PerioExamCur.ProvNum=PatCur.PriProv;
			PerioExamCur.DateTMeasureEdit=MiscData.GetNowDateTime();
			PerioExams.Insert(PerioExamCur);
			PerioMeasures.SetSkipped(PerioExamCur.PerioExamNum,GetSkippedTeeth());
		}

		///<summary>Returns a list of skipped teeth.</summary>
		private List<int> GetSkippedTeeth() {
			List<int> listSkippedTeeth=new List<int>();
			if(PerioExams.ListExams.Count > 0) {
				//set skipped teeth based on the last exam in the list: 
				listSkippedTeeth=PerioMeasures.GetSkipped(PerioExams.ListExams[PerioExams.ListExams.Count-1].PerioExamNum);
			}
			//For patient's first perio chart, any teeth marked missing are automatically marked skipped.
			if(PerioExams.ListExams.Count==0 || PrefC.GetBool(PrefName.PerioSkipMissingTeeth)) {
				for(int i=0;i<_listMissingTeeth.Count;i++) {
					if(_listMissingTeeth[i].CompareTo("A") >= 0 && _listMissingTeeth[i].CompareTo("Z") <= 0) {//if is a letter (not a number)
						continue;//Skipped teeth are only recorded by tooth number within the perio exam.
					}
					int toothNum=PIn.Int(_listMissingTeeth[i]);
					//Check if this tooth has had an implant done AND the office has the preference to SHOW implants
					if(PrefC.GetBool(PrefName.PerioTreatImplantsAsNotMissing) && ContrPerio.IsImplant(toothNum)) {
						listSkippedTeeth.RemoveAll(x => x==toothNum);//Remove the tooth from the list of skipped teeth if it exists.
						continue;//We do note want to add it back to the list below.
					}
					//This tooth is missing and we know it is not an implant OR the office has the preference to ignore implants.
					//Simply add it to our list of skipped teeth.
					listSkippedTeeth.Add(toothNum);
				}
			}
			return listSkippedTeeth.Distinct().ToList();
		}

		private void butCopyPrevious_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.PerioEdit,MiscData.GetNowDateTime())) {
				return;
			}
			if(PerioExams.ListExams.Count==0){
				MsgBox.Show(this,"Please use the Add button to create an initial exam.");
				return;
			}
			if(gridODExam.GetSelectedIndex()==-1) {
				//with the current code, this will never actually get hit because we don't allow deselecting.
				MsgBox.Show(this,"Please select an existing exam first.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"A copy will be made of the previous exam.  Continue?")){
				return;
			}
			gridP.SaveCurExam(PerioExamCur);
			CreateNewPerioChart();
			//get meaures from last exam
			List<PerioMeasure> listPerio=PerioMeasures.GetAllForExam(PerioExams.ListExams[PerioExams.ListExams.Count-1].PerioExamNum);
			for(int i=0;i<listPerio.Count;i++) { //add all of the previous exam's measures to this perio exam.
				listPerio[i].PerioExamNum=PerioExamCur.PerioExamNum;
				PerioMeasures.Insert(listPerio[i]);
			}
			RefreshListExams();
			gridODExam.SetSelected(PerioExams.ListExams.Count-1,true); //select the exam that was just inserted.
			FillGrid();
			SecurityLogs.MakeLogEntry(Permissions.PerioEdit,PatCur.PatNum,"Perio exam copied.");
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(gridODExam.GetSelectedIndex()==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			if(!Security.IsAuthorized(Permissions.PerioEdit,PerioExams.ListExams[gridODExam.GetSelectedIndex()].ExamDate)) {
				return;
			}
			if(MessageBox.Show(Lan.g(this,"Delete Exam?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
				return;
			}
			int curselected=gridODExam.GetSelectedIndex();
			PerioExams.Delete(PerioExamCur);
			RefreshListExams();
			if(curselected < gridODExam.ListGridRows.Count) {
				gridODExam.SetSelected(curselected,true);
			}
			else {
				gridODExam.SetSelected(PerioExams.ListExams.Count-1,true);
			}	
			FillGrid();
			if(gridODExam.ListGridRows.Count==0) {
				textExamNotes.Text="";
			}
			SecurityLogs.MakeLogEntry(Permissions.PerioEdit,PatCur.PatNum,"Perio exam deleted");
		}

		private void butListen_Click(object sender,EventArgs e) {
			if(ODBuild.IsWeb()) {
				MsgBox.Show(this,"Voice Perio is not available for the Web version at this time.");
				return;
			}
			if(_voiceController!=null && _voiceController.IsListening) {
				_voiceController.StopListening();
				labelListening.Visible=false;
				return;
			}
			try {
				if(_voiceController==null) {
					_voiceController=new VoiceController(VoiceCommandArea.PerioChart);
					_voiceController.SpeechRecognized+=VoiceController_SpeechRecognized;
				}
				_voiceController.StartListening();
				labelListening.Visible=true;
			}
			catch(PlatformNotSupportedException) {
				MsgBox.Show(this,"The Voice Command feature does not work over server RDP sessions.");
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Unable to initialize audio input. Try plugging a different microphone into the computer."),ex);
			}
		}

		private void checkShowCurrent_Click(object sender,EventArgs e) {
			if(gridODExam.GetSelectedIndex()==-1) {
				return;
			}
			gridP.SaveCurExam(PerioExamCur);
			PerioExams.Refresh(PatCur.PatNum);
			PerioMeasures.Refresh(PatCur.PatNum,PerioExams.ListExams);
			FillGrid();
		}

		private void radioRight_Click(object sender, System.EventArgs e) {
			gridP.Direction=AutoAdvanceDirection.Right;
			gridP.Focus();
		}

		private void radioLeft_Click(object sender, System.EventArgs e) {
			gridP.Direction=AutoAdvanceDirection.Left;
			gridP.Focus();
		}

		private void RadioCustom_Click(object sender,EventArgs e) {
			gridP.Direction=AutoAdvanceDirection.Custom;
			gridP.Focus();
		}

		private void gridP_DirectionChangedRight(object sender, System.EventArgs e) {
			radioRight.Checked=true;
		}

		private void gridP_DirectionChangedLeft(object sender, System.EventArgs e) {
			radioLeft.Checked=true;
		}

		private void checkThree_Click(object sender, System.EventArgs e) {
			gridP.ThreeAtATime=checkThree.Checked;
		}
		
		private void but0_Click(object sender, System.EventArgs e) {
			NumberClicked(0);
		}

		private void but1_Click(object sender, System.EventArgs e) {
			NumberClicked(1);
		}

		private void but2_Click(object sender, System.EventArgs e) {
			NumberClicked(2);
		}

		private void but3_Click(object sender, System.EventArgs e) {
			NumberClicked(3);
		}

		private void but4_Click(object sender, System.EventArgs e) {
			NumberClicked(4);
		}

		private void but5_Click(object sender, System.EventArgs e) {
			NumberClicked(5);
		}

		private void but6_Click(object sender, System.EventArgs e) {
			NumberClicked(6);
		}

		private void but7_Click(object sender, System.EventArgs e) {
			NumberClicked(7);
		}

		private void but8_Click(object sender, System.EventArgs e) {
			NumberClicked(8);
		}

		private void but9_Click(object sender, System.EventArgs e) {
			NumberClicked(9);
		}

		///<summary>The only valid numbers are 0 through 9</summary>
		private void NumberClicked(int number){
			if(gridP.SelectedExam==-1) {
				MessageBox.Show(Lan.g(this,"Please add or select an exam first in the list to the left."));
				return;
			}
			if(TenIsDown) {
				gridP.ButtonPressed(10+number);
			}
			else {
				gridP.ButtonPressed(number);
			}
			TenIsDown=false;
			gridP.Focus();
		}

		private void but10_Click(object sender, System.EventArgs e) {
			TenIsDown=true;
		}

		private void butCalcIndex_Click(object sender, System.EventArgs e) {
			FillIndexes();
			if(listPlaqueHistory.Visible) {
				gridP.SaveCurExam(PerioExamCur);
				RefreshListPlaque();
				FillGrid();
			}
		}

		private void FillIndexes(){
			textIndexPlaque.Text=gridP.ComputeIndex(BleedingFlags.Plaque);
			textIndexCalculus.Text=gridP.ComputeIndex(BleedingFlags.Calculus);
			textIndexBleeding.Text=gridP.ComputeIndex(BleedingFlags.Blood);
			textIndexSupp.Text=gridP.ComputeIndex(BleedingFlags.Suppuration);
		}

		private void butBleed_Click(object sender, System.EventArgs e) {
			TenIsDown=false;
			gridP.ButtonPressed("b");
			gridP.Focus();
		}

		private void butPus_Click(object sender, System.EventArgs e) {
			TenIsDown=false;
			gridP.ButtonPressed("s");
			gridP.Focus();
		}

		private void butPlaque_Click(object sender, System.EventArgs e) {
			TenIsDown=false;
			gridP.ButtonPressed("p");
			gridP.Focus();
		}

		private void butCalculus_Click(object sender, System.EventArgs e) {
			TenIsDown=false;
			gridP.ButtonPressed("c");
			gridP.Focus();
		}

		private void butColorBleed_Click(object sender, System.EventArgs e) {
			colorDialog1.Color=butColorBleed.BackColor;
			if(colorDialog1.ShowDialog()!=DialogResult.OK){
				colorDialog1.Dispose();
				return;
			}
			butColorBleed.BackColor=colorDialog1.Color;
			Def DefCur=_listMiscColorDefs[(int)DefCatMiscColors.PerioBleeding].Copy();
			DefCur.ItemColor=colorDialog1.Color;
			Defs.Update(DefCur);
			Cache.Refresh(InvalidType.Defs);
			localDefsChanged=true;
			gridP.SetColors();
			gridP.Invalidate();
			gridP.Focus();
			colorDialog1.Dispose();
		}

		private void butColorPus_Click(object sender, System.EventArgs e) {
			colorDialog1.Color=butColorPus.BackColor;
			if(colorDialog1.ShowDialog()!=DialogResult.OK){
				colorDialog1.Dispose();
				return;
			}
			butColorPus.BackColor=colorDialog1.Color;
			Def DefCur=_listMiscColorDefs[(int)DefCatMiscColors.PerioSuppuration].Copy();
			DefCur.ItemColor=colorDialog1.Color;
			Defs.Update(DefCur);
			Cache.Refresh(InvalidType.Defs);
			localDefsChanged=true;
			gridP.SetColors();
			gridP.Invalidate();
			gridP.Focus();
			colorDialog1.Dispose();
		}

		private void butColorPlaque_Click(object sender, System.EventArgs e) {
			colorDialog1.Color=butColorPlaque.BackColor;
			if(colorDialog1.ShowDialog()!=DialogResult.OK){
				colorDialog1.Dispose();
				return;
			}
			butColorPlaque.BackColor=colorDialog1.Color;
			Def DefCur=_listMiscColorDefs[(int)DefCatMiscColors.PerioPlaque].Copy();
			DefCur.ItemColor=colorDialog1.Color;
			Defs.Update(DefCur);
			Cache.Refresh(InvalidType.Defs);
			localDefsChanged=true;
			gridP.SetColors();
			gridP.Invalidate();
			gridP.Focus();
			colorDialog1.Dispose();
		}

		private void butColorCalculus_Click(object sender, System.EventArgs e) {
			colorDialog1.Color=butColorCalculus.BackColor;
			if(colorDialog1.ShowDialog()!=DialogResult.OK){
				colorDialog1.Dispose();
				return;
			}
			butColorCalculus.BackColor=colorDialog1.Color;
			Def DefCur=_listMiscColorDefs[(int)DefCatMiscColors.PerioCalculus].Copy();
			DefCur.ItemColor=colorDialog1.Color;
			Defs.Update(DefCur);
			Cache.Refresh(InvalidType.Defs);
			localDefsChanged=true;
			gridP.SetColors();
			gridP.Invalidate();
			gridP.Focus();
			colorDialog1.Dispose();
		}

		private void butSkip_Click(object sender, System.EventArgs e) {
			if(gridODExam.GetSelectedIndex()<0){//PerioExamCur could still be set to a deleted exam and would not be null even if there is no exam.
				MessageBox.Show(Lan.g(this,"Please select an exam first."));
				return;
			}
			gridP.ToggleSkip(PerioExamCur.PerioExamNum);
		}

		private void updownRed_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			//this is necessary because Microsoft's updown control is too buggy to be useful
			Cursor=Cursors.WaitCursor;
			PrefName prefname=PrefName.PerioRedProb;
			if(sender==updownProb){
				prefname=PrefName.PerioRedProb;
			}
			else if(sender==updownMGJ) {
				prefname=PrefName.PerioRedMGJ;
			}
			else if(sender==updownGing) {
				prefname=PrefName.PerioRedGing;
			}
			else if(sender==updownCAL) {
				prefname=PrefName.PerioRedCAL;
			}
			else if(sender==updownFurc) {
				prefname=PrefName.PerioRedFurc;
			}
			else if(sender==updownMob) {
				prefname=PrefName.PerioRedMob;
			}
			int currentValue=PrefC.GetInt(prefname);
			if(e.Y<8){//up
				currentValue++;
			}
			else{//down
				if(currentValue==0){
					Cursor=Cursors.Default;
					return;
				}
				currentValue--;
			}
			Prefs.UpdateLong(prefname,currentValue);
			//pref.ValueString=currentValue.ToString();
			//Prefs.Update(pref);
			localDefsChanged=true;
			Cache.Refresh(InvalidType.Prefs);
			if(sender==updownProb){
				textRedProb.Text=currentValue.ToString();
				textCountProb.Text=gridP.CountTeeth(PerioSequenceType.Probing).Count.ToString();
			}
			else if(sender==updownMGJ){
				textRedMGJ.Text=currentValue.ToString();
				textCountMGJ.Text=gridP.CountTeeth(PerioSequenceType.MGJ).Count.ToString();
			}
			else if(sender==updownGing){
				textRedGing.Text=currentValue.ToString();
				textCountGing.Text=gridP.CountTeeth(PerioSequenceType.GingMargin).Count.ToString();
			}
			else if(sender==updownCAL){
				textRedCAL.Text=currentValue.ToString();
				textCountCAL.Text=gridP.CountTeeth(PerioSequenceType.CAL).Count.ToString();
			}
			else if(sender==updownFurc){
				textRedFurc.Text=currentValue.ToString();
				textCountFurc.Text=gridP.CountTeeth(PerioSequenceType.Furcation).Count.ToString();
			}
			else if(sender==updownMob){
				textRedMob.Text=currentValue.ToString();
				textCountMob.Text=gridP.CountTeeth(PerioSequenceType.Mobility).Count.ToString();
			}
			gridP.Invalidate();
			Cursor=Cursors.Default;
			gridP.Focus();
		}

		private void butCount_Click(object sender, System.EventArgs e) {
			FillCounts();
			gridP.Focus();
		}

		private void FillCounts(){
			textCountProb.Text=gridP.CountTeeth(PerioSequenceType.Probing).Count.ToString();
			textCountMGJ.Text=gridP.CountTeeth(PerioSequenceType.MGJ).Count.ToString();
			textCountGing.Text=gridP.CountTeeth(PerioSequenceType.GingMargin).Count.ToString();
			textCountCAL.Text=gridP.CountTeeth(PerioSequenceType.CAL).Count.ToString();
			textCountFurc.Text=gridP.CountTeeth(PerioSequenceType.Furcation).Count.ToString();
			textCountMob.Text=gridP.CountTeeth(PerioSequenceType.Mobility).Count.ToString();
		}

		private void butPrint_Click(object sender, System.EventArgs e) {
			if(this.gridODExam.GetSelectedIndex()<0) {
				MsgBox.Show(this,"Please select an exam first.");
				return;
			}
			//Store Font and Size information for the control so that we can adjust temporarily for printing.
			Font fontOriginal=gridP.Font;
			Size sizeContrOriginal=gridP.Size;
			gridP.LayoutManager.Is96dpi=true;//Tells the layoutManager not to scale the size and font again when the ContrPerio.OnResize event is called.
			gridP.Font=new Font("Microsoft Sans Serif", 8.25f);
			LayoutManager.MoveSize(gridP,new Size(602, 642));//Revert to default drawn grid size for printing, be sure to adjust this if the default size is changed on ContrPerio.cs, this will then trigger the ContrPerio.OnResize Event adjusting all of the formatting necessary for printing.
			PrinterL.TryPrint(pd2_PrintPage,
				Lan.g(this,"Perio chart from")+" "+PerioExamCur.ExamDate+" "+Lan.g(this,"printed"),
				PatCur.PatNum,
				PrintSituation.TPPerio,
				new Margins(0,0,0,0),
				PrintoutOrigin.AtMargin
			);
			gridP.LayoutManager.Is96dpi=false;
			gridP.Font=fontOriginal;
			LayoutManager.MoveSize(gridP,sizeContrOriginal);
			gridP.Focus();
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(this.gridODExam.GetSelectedIndex()<0){
				MessageBox.Show(Lan.g(this,"Please select an exam first."));
				return;
			}
			gridP.SaveCurExam(PerioExamCur);
			PerioExams.Refresh(PatCur.PatNum);
			PerioMeasures.Refresh(PatCur.PatNum,PerioExams.ListExams);
			FillGrid();
			Bitmap gridImage=null;
			Bitmap perioPrintImage=null;
			Graphics g=null;
			//Document doc=new Document();
			//try {
			perioPrintImage=new Bitmap(LayoutManager.Scale(750),LayoutManager.Scale(1000));
			perioPrintImage.SetResolution(96,96);
			g=Graphics.FromImage(perioPrintImage);
			g.Clear(Color.White);
			g.CompositingQuality=System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			g.InterpolationMode=System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			g.SmoothingMode=System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			g.TextRenderingHint=System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			string clinicName="";
			//This clinic name could be more accurate here in the future if we make perio exams clinic specific.
			//Perhaps if there were a perioexam.ClinicNum column.
			if(PatCur.ClinicNum!=0) {
				Clinic clinic=Clinics.GetClinic(PatCur.ClinicNum);
				clinicName=clinic.Description;
			} 
			else {
				clinicName=PrefC.GetString(PrefName.PracticeTitle);
			}
			float y=50f;
			SizeF m;
			//Title
			Font font=new Font("Arial",LayoutManager.Scale(15));
			string titleStr=Lan.g(this,"Periodontal Charting");
			m=g.MeasureString(titleStr,font);
			g.DrawString(titleStr,font,Brushes.Black,new PointF(perioPrintImage.Width/2f-m.Width/2f,y));
			y+=m.Height;
			//Clinic Name
			font=new Font("Arial",LayoutManager.Scale(11));
			m=g.MeasureString(clinicName,font);
			g.DrawString(clinicName,font,Brushes.Black,
				new PointF(perioPrintImage.Width/2f-m.Width/2f,y));
			y+=m.Height;
			//Patient name
			string patNameStr=PatCur.GetNameFLFormal();
			m=g.MeasureString(patNameStr,font);
			g.DrawString(patNameStr,font,Brushes.Black,new PointF(perioPrintImage.Width/2f-m.Width/2f,y));
			y+=m.Height;
			//Date
			//We put the current datetime here because the specific exam dates are listed in the form.
			string timeNowStr=MiscData.GetNowDateTime().ToShortDateString();
			m=g.MeasureString(timeNowStr,font);
			g.DrawString(timeNowStr,font,Brushes.Black,new PointF(perioPrintImage.Width/2f-m.Width/2f,y));
			y+=m.Height;
			//Now draw the grid
			gridImage=new Bitmap(gridP.RectBoundsShowing.Width,gridP.RectBoundsShowing.Height);
			gridP.DrawToBitmap(gridImage,gridP.RectBoundsShowing);
			g.DrawImageUnscaled(gridImage,(int)((perioPrintImage.Width-gridImage.Width)/2f),(int)y);
			long defNumCategory=Defs.GetImageCat(ImageCategorySpecial.T);
			if(defNumCategory==0) {
				MsgBox.Show(this,"No image category set for tooth charts in definitions.");
				perioPrintImage.Dispose();
				gridImage.Dispose();
				perioPrintImage=null;
				gridImage=null;
				g.Dispose();
				return;
			}
			try {
				ImageStore.Import(perioPrintImage,defNumCategory,ImageType.Photo,PatCur);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Unable to save file. ") + ex.Message);
				perioPrintImage.Dispose();
				gridImage.Dispose();
				perioPrintImage=null;
				gridImage=null;
				g.Dispose();
				return;
			}
			MsgBox.Show(this,"Saved.");
			/*
			string patImagePath=ImageStore.GetPatientFolder(PatCur);
			string filePath="";
			do {
				doc.DateCreated=MiscData.GetNowDateTime();
				doc.FileName="perioexam_"+doc.DateCreated.ToString("yyyy_MM_dd_hh_mm_ss")+".png";
				filePath=ODFileUtils.CombinePaths(patImagePath,doc.FileName);
			} while(File.Exists(filePath));//if a file with this name already exists, then it will stay in this loop
			doc.PatNum=PatCur.PatNum;
			doc.ImgType=ImageType.Photo;
			doc.DocCategory=Defs.GetByExactName(DefCat.ImageCats,"Tooth Charts");
			doc.Description="Perio Exam";
			Documents.Insert(doc,PatCur);
			docCreated=true;
			perioPrintImage.Save(filePath,System.Drawing.Imaging.ImageFormat.Png);
			MessageBox.Show(Lan.g(this,"Image saved."));*/
			/*} 
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Image failed to save: "+Environment.NewLine+ex.ToString()));
				if(docCreated) {
					Documents.Delete(doc);
				}
			} 
			finally {
				if(gridImage!=null){
					gridImage.Dispose();
				}
				if(g!=null) {
					g.Dispose();
					g=null;
				}
				if(perioPrintImage!=null) {
					perioPrintImage.Dispose();
					perioPrintImage=null;
				}
			}*/
			perioPrintImage.Dispose();
			gridImage.Dispose();
			perioPrintImage=null;
			gridImage=null;
			g.Dispose();
		}

		private void pd2_PrintPage(object sender, PrintPageEventArgs ev){//raised for each page to be printed.
			Graphics grfx=ev.Graphics;
			//MessageBox.Show(grfx.
			float yPos=67+25+20+20+20+6;
			float xPos=100;
			grfx.TranslateTransform(xPos,yPos);
			gridP.DrawChart(grfx);//have to print graphics first, or they cover up title.
			grfx.DrawString("F",new Font("Arial",15),Brushes.Black,new Point(-26,92));
			grfx.DrawString("L",new Font("Arial",15),Brushes.Black,new Point(-26,212));
			grfx.DrawString("L",new Font("Arial",15),Brushes.Black,new Point(-26,416));
			grfx.DrawString("F",new Font("Arial",15),Brushes.Black,new Point(-26,552));
			grfx.TranslateTransform(-xPos,-yPos);
			yPos=67;
			xPos=100;
			string clinicName="";
			//This clinic name could be more accurate here in the future if we make perio exams clinic specific.
			//Perhaps if there were a perioexam.ClinicNum column.
			if(PatCur.ClinicNum!=0) {
				Clinic clinic=Clinics.GetClinic(PatCur.ClinicNum);
				clinicName=clinic.Description;
			} 
			else {
				clinicName=PrefC.GetString(PrefName.PracticeTitle);
			}
			StringFormat format=new StringFormat();
			format.Alignment=StringAlignment.Center;
			grfx.DrawString(Lan.g(this,"Periodontal Charting"),new Font("Arial",15),Brushes.Black,
				new RectangleF(xPos,yPos,650,25),format);			
			yPos+=25;
			grfx.DrawString(clinicName,new Font("Arial",11),Brushes.Black
				,new RectangleF(xPos,yPos,650,25),format);
			yPos+=20;
			grfx.DrawString(PatCur.GetNameFL(),new Font("Arial",11),Brushes.Black
				,new RectangleF(xPos,yPos,650,25),format);
			yPos+=20;
			string timeCurString=MiscData.GetNowDateTime().ToShortDateString();
			grfx.DrawString(timeCurString,new Font("Arial",11),Brushes.Black,
				new RectangleF(xPos,yPos,650,25),format);
			yPos+=20;//Offset for printed text
			yPos+=688;//Offset for grid that we already drew.
			Font font=new Font("Arial",9);
			grfx.FillEllipse(new SolidBrush(butColorPlaque.BackColor),xPos,yPos+3,8,8);
			grfx.DrawString(Lan.g(this,"Plaque Index:")+" "+gridP.ComputeIndex(BleedingFlags.Plaque)+" %"
				,font,Brushes.Black,xPos+12,yPos);
			yPos+=20;
			grfx.FillEllipse(new SolidBrush(butColorCalculus.BackColor),xPos,yPos+3,8,8);
			grfx.DrawString(Lan.g(this,"Calculus Index:")+" "+gridP.ComputeIndex(BleedingFlags.Calculus)+" %"
				,font,Brushes.Black,xPos+12,yPos);
			yPos+=20;
			grfx.FillEllipse(new SolidBrush(butColorBleed.BackColor),xPos,yPos+3,8,8);
			grfx.DrawString(Lan.g(this,"Bleeding Index:")+" "+gridP.ComputeIndex(BleedingFlags.Blood)+" %"
				,font,Brushes.Black,xPos+12,yPos);
			yPos+=20;
			grfx.FillEllipse(new SolidBrush(butColorPus.BackColor),xPos,yPos+3,8,8);
			grfx.DrawString(Lan.g(this,"Suppuration Index:")+" "+gridP.ComputeIndex(BleedingFlags.Suppuration)+" %"
				,font,Brushes.Black,xPos+12,yPos);
			yPos+=20;
			grfx.DrawString(Lan.g(this,"Teeth with Probing greater than or equal to")+" "+textRedProb.Text+" mm: "
				+ConvertALtoString(gridP.CountTeeth(PerioSequenceType.Probing))
				,font,Brushes.Black,xPos,yPos);
			yPos+=20;
			grfx.DrawString(Lan.g(this,"Teeth with MGJ less than or equal to")+" "+textRedMGJ.Text+" mm: "
				+ConvertALtoString(gridP.CountTeeth(PerioSequenceType.MGJ))
				,font,Brushes.Black,xPos,yPos);
			yPos+=20;
			grfx.DrawString(Lan.g(this,"Teeth with Gingival Margin greater than or equal to")+" "+textRedGing.Text+" mm: "
				+ConvertALtoString(gridP.CountTeeth(PerioSequenceType.GingMargin))
				,font,Brushes.Black,xPos,yPos);
			yPos+=20;
			grfx.DrawString(Lan.g(this,"Teeth with CAL greater than or equal to")+" "+textRedCAL.Text+" mm: "
				+ConvertALtoString(gridP.CountTeeth(PerioSequenceType.CAL))
				,font,Brushes.Black,xPos,yPos);
			yPos+=20;
			grfx.DrawString(Lan.g(this,"Teeth with Furcations greater than or equal to class")+" "+textRedFurc.Text+": "
				+ConvertALtoString(gridP.CountTeeth(PerioSequenceType.Furcation))
				,font,Brushes.Black,xPos,yPos);
			yPos+=20;
			grfx.DrawString(Lan.g(this,"Teeth with Mobility greater than or equal to")+" "+textRedMob.Text+": "
				+ConvertALtoString(gridP.CountTeeth(PerioSequenceType.Mobility))
				,font,Brushes.Black,xPos,yPos);
			//pagesPrinted++;
			ev.HasMorePages=false;
			grfx.Dispose();
		}

		private string ConvertALtoString(ArrayList ALteeth){
			if(ALteeth.Count==0){
				return "none";
			}
			string retVal="";
			for(int i=0;i<ALteeth.Count;i++){
				if(i>0)
					retVal+=",";
				retVal+=ALteeth[i];
			}
			return retVal;
		}

//		private void butEditNotes_Click(object sender,EventArgs e) {
//			int examIndex=gridODExam.GetSelectedIndex();
//			if(examIndex==-1) {
//				return;
//			}
//			using InputBox inputBox = new InputBox("Edit Notes",true,PerioExamCur.Note,new Size(351,215));
//			if(inputBox.ShowDialog()!=DialogResult.OK){
//				return;
//			}
//			if(PerioExamCur.Note!=inputBox.textResult.Text) {
//				PerioExamCur.Note=inputBox.textResult.Text;
//				PerioExams.Update(PerioExamCur);
//			}
//			gridP.SaveCurExam(PerioExamCur);
//			FillGrid();
//			RefreshListExams();
//			gridODExam.SetSelected(examIndex,true);
//		}

		private void butGraphical_Click(object sender,EventArgs e) {
			if(!ToothChartRelay.IsSparks3DPresent){
				if(ComputerPrefs.LocalComputer.GraphicsSimple!=DrawingMode.DirectX) {
					MsgBox.Show(this,"In the Graphics setup window, you must first select DirectX.");
					return;
				}
			}
			if(gridODExam.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Exam must be selected first.");
				return;
			}
			if(localDefsChanged) {
				DataValid.SetInvalid(InvalidType.Defs,InvalidType.Prefs);
			}
			//if(listExams.SelectedIndex!=-1) {
			gridP.SaveCurExam(PerioExamCur);
			PerioExams.Refresh(PatCur.PatNum);//refresh instead
			PerioMeasures.Refresh(PatCur.PatNum,PerioExams.ListExams);
			FillGrid();
			using FormPerioGraphical formg=new FormPerioGraphical(PerioExams.ListExams[gridODExam.GetSelectedIndex()],PatCur);
			formg.ShowDialog();
		}

		private void checkGingMarg_CheckedChanged(object sender,EventArgs e) {
			gridP.GingMargPlus=checkGingMarg.Checked;
			gridP.Focus();
		}

		///<summary>This ensures that the textbox will always have focus when using FormPerio.</summary>
		private void textInputBox_Leave(object sender,EventArgs e) {
			if(!textExamNotes.Focused) {
				textInputBox.Focus();
			}
		}

		private void textExamNotes_Leave(object sender,EventArgs e) {
			if(PerioExamCur!=null) { 
				PerioExam perioExamOld=PerioExamCur.Copy();
				PerioExamCur.Note=textExamNotes.Text;
				if(PerioExams.Update(PerioExamCur,perioExamOld)) {
					int selectionOld=gridODExam.GetSelectedIndex();
					RefreshListExams(skipRefreshMeasures:true);
					gridODExam.SetSelected(selectionOld);
				}
			}
			textInputBox.Focus();
		}

		private void gridP_Click(object sender,EventArgs e) {
			textInputBox.Focus();
		}

		///<summary>Catches any non-alphanumeric keypresses so that they can be passed into the grid separately.</summary>
		private void textInputBox_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Left
				|| e.KeyCode==Keys.Right
				|| e.KeyCode==Keys.Up
				|| e.KeyCode==Keys.Down
				|| e.KeyCode==Keys.Delete
				|| e.KeyCode==Keys.Back
				|| e.Modifiers==Keys.Control) 
			{
				gridP.KeyPressed(e);
			}
		}

		///<summary>Used to force buttons to be pressed as characters are typed/dictated/pasted into the textbox.</summary>
		private void textInputBox_TextChanged(object sender,EventArgs e) {
			Char[] arrInputChars=textInputBox.Text.ToLower().ToCharArray();
			for(int i=0;i<arrInputChars.Length;i++) {
				if(arrInputChars[i]>=48 && arrInputChars[i]<=57) {
					NumberClicked(arrInputChars[i]-48);
				}
				else if(arrInputChars[i]=='b'
					|| arrInputChars[i]=='c'
					|| arrInputChars[i]=='s'
					|| arrInputChars[i]=='p') 
				{
					gridP.ButtonPressed(arrInputChars[i].ToString());
				}
				else if(arrInputChars[i]==' ') {
					gridP.ButtonPressed("b");
				}
			}
			textInputBox.Clear();
		}

		private void butClose_Click(object sender,System.EventArgs e) {
			if(_userPrefCurrentOnly==null) {
				UserOdPrefs.Insert(new UserOdPref() {
					UserNum=Security.CurUser.UserNum,
					FkeyType=UserOdFkeyType.PerioCurrentExamOnly,
					ValueString=POut.Bool(checkShowCurrent.Checked)
				});
			}
			else {
				if(_userPrefCurrentOnly.ValueString != POut.Bool(checkShowCurrent.Checked)) {//The user preference has changed.
					_userPrefCurrentOnly.ValueString=POut.Bool(checkShowCurrent.Checked);
					UserOdPrefs.Update(_userPrefCurrentOnly);
				}
			}
			if(_userPrefCustomAdvance==null) {
				UserOdPrefs.Insert(new UserOdPref() {
					UserNum=Security.CurUser.UserNum,
					FkeyType=UserOdFkeyType.PerioAutoAdvanceCustom,
					ValueString=POut.Bool(radioCustom.Checked)
				});
			}
			else {
				if(_userPrefCustomAdvance.ValueString != POut.Bool(radioCustom.Checked)) {//The user preference has changed.
					_userPrefCustomAdvance.ValueString=POut.Bool(radioCustom.Checked);
					UserOdPrefs.Update(_userPrefCustomAdvance);
				}
			}
			Close();
		}

		private void FormPerio_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(PerioExamCur!=null) { //When no Exam is selected or ExamList is empty we don't want to save ExamNotes.
				PerioExam perioExamOld=PerioExamCur.Copy();
				PerioExamCur.Note=textExamNotes.Text;
				PerioExams.Update(PerioExamCur,perioExamOld);
			}
			if(localDefsChanged){
				DataValid.SetInvalid(InvalidType.Defs, InvalidType.Prefs);
			}
			if(gridODExam.GetSelectedIndex()!=-1){
				gridP.SaveCurExam(PerioExamCur);
			}
			_voiceController?.Dispose();
			Plugins.HookAddCode(this,"FormPerio.Closing_end");
		}
	}
}





















