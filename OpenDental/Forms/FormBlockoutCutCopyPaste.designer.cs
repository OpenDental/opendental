using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormBlockoutCutCopyPaste {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBlockoutCutCopyPaste));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textClipboard = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.butCopyWeek = new OpenDental.UI.Button();
			this.butCopyDay = new OpenDental.UI.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.butRepeat = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.checkReplace = new System.Windows.Forms.CheckBox();
			this.textRepeat = new System.Windows.Forms.TextBox();
			this.butPaste = new OpenDental.UI.Button();
			this.checkWeekend = new System.Windows.Forms.CheckBox();
			this.butClearDay = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textClipboard);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.butCopyWeek);
			this.groupBox1.Controls.Add(this.butCopyDay);
			this.groupBox1.Location = new System.Drawing.Point(26, 50);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(158, 198);
			this.groupBox1.TabIndex = 40;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Copy";
			// 
			// textClipboard
			// 
			this.textClipboard.Location = new System.Drawing.Point(6, 113);
			this.textClipboard.Name = "textClipboard";
			this.textClipboard.ReadOnly = true;
			this.textClipboard.Size = new System.Drawing.Size(146, 20);
			this.textClipboard.TabIndex = 30;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(149, 80);
			this.label1.TabIndex = 47;
			this.label1.Text = "Copying only applies to the visible operatories for the current appointment view." +
    " It also does not copy to a different operatory.";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 96);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(146, 14);
			this.label3.TabIndex = 29;
			this.label3.Text = "Clipboard Contents";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butCopyWeek
			// 
			this.butCopyWeek.Location = new System.Drawing.Point(6, 165);
			this.butCopyWeek.Name = "butCopyWeek";
			this.butCopyWeek.Size = new System.Drawing.Size(75, 24);
			this.butCopyWeek.TabIndex = 28;
			this.butCopyWeek.Text = "Copy Week";
			this.butCopyWeek.Click += new System.EventHandler(this.butCopyWeek_Click);
			// 
			// butCopyDay
			// 
			this.butCopyDay.Location = new System.Drawing.Point(6, 138);
			this.butCopyDay.Name = "butCopyDay";
			this.butCopyDay.Size = new System.Drawing.Size(75, 24);
			this.butCopyDay.TabIndex = 27;
			this.butCopyDay.Text = "Copy Day";
			this.butCopyDay.Click += new System.EventHandler(this.butCopyDay_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.butRepeat);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.checkReplace);
			this.groupBox2.Controls.Add(this.textRepeat);
			this.groupBox2.Controls.Add(this.butPaste);
			this.groupBox2.Location = new System.Drawing.Point(26, 263);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(158, 97);
			this.groupBox2.TabIndex = 45;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Paste";
			// 
			// butRepeat
			// 
			this.butRepeat.Location = new System.Drawing.Point(6, 64);
			this.butRepeat.Name = "butRepeat";
			this.butRepeat.Size = new System.Drawing.Size(75, 24);
			this.butRepeat.TabIndex = 30;
			this.butRepeat.Text = "Repeat";
			this.butRepeat.Click += new System.EventHandler(this.butRepeat_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(70, 70);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(37, 14);
			this.label4.TabIndex = 32;
			this.label4.Text = "#";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// checkReplace
			// 
			this.checkReplace.Checked = true;
			this.checkReplace.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkReplace.Location = new System.Drawing.Point(6, 14);
			this.checkReplace.Name = "checkReplace";
			this.checkReplace.Size = new System.Drawing.Size(146, 18);
			this.checkReplace.TabIndex = 31;
			this.checkReplace.Text = "Replace Existing";
			this.checkReplace.UseVisualStyleBackColor = true;
			// 
			// textRepeat
			// 
			this.textRepeat.Location = new System.Drawing.Point(110, 67);
			this.textRepeat.Name = "textRepeat";
			this.textRepeat.Size = new System.Drawing.Size(39, 20);
			this.textRepeat.TabIndex = 31;
			this.textRepeat.Text = "1";
			// 
			// butPaste
			// 
			this.butPaste.Location = new System.Drawing.Point(6, 37);
			this.butPaste.Name = "butPaste";
			this.butPaste.Size = new System.Drawing.Size(75, 24);
			this.butPaste.TabIndex = 29;
			this.butPaste.Text = "Paste";
			this.butPaste.Click += new System.EventHandler(this.butPaste_Click);
			// 
			// checkWeekend
			// 
			this.checkWeekend.Location = new System.Drawing.Point(123, 16);
			this.checkWeekend.Name = "checkWeekend";
			this.checkWeekend.Size = new System.Drawing.Size(143, 18);
			this.checkWeekend.TabIndex = 46;
			this.checkWeekend.Text = "Include Weekends";
			this.checkWeekend.UseVisualStyleBackColor = true;
			// 
			// butClearDay
			// 
			this.butClearDay.Location = new System.Drawing.Point(32, 12);
			this.butClearDay.Name = "butClearDay";
			this.butClearDay.Size = new System.Drawing.Size(75, 24);
			this.butClearDay.TabIndex = 48;
			this.butClearDay.Text = "Clear Day";
			this.butClearDay.Click += new System.EventHandler(this.butClearDay_Click);
			// 
			// FormBlockoutCutCopyPaste
			// 
			this.ClientSize = new System.Drawing.Size(290, 383);
			this.Controls.Add(this.butClearDay);
			this.Controls.Add(this.checkWeekend);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBox2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormBlockoutCutCopyPaste";
			this.ShowInTaskbar = false;
			this.Text = "Blockout Cut-Copy-Paste";
			this.Load += new System.EventHandler(this.FormBlockoutCutCopyPaste_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private GroupBox groupBox1;
		private OpenDental.UI.Button butCopyWeek;
		private OpenDental.UI.Button butCopyDay;
		private GroupBox groupBox2;
		private OpenDental.UI.Button butRepeat;
		private Label label4;
		private CheckBox checkReplace;
		private TextBox textRepeat;
		private OpenDental.UI.Button butPaste;
		private CheckBox checkWeekend;
		private TextBox textClipboard;
		private Label label3;
		private Label label1;
		private OpenDental.UI.Button butClearDay;
	}
}
