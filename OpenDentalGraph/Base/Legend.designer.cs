using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentalGraph {
	public partial class Legend {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.panelDraw = new OpenDentalGraph.Legend.PanelNoFlicker();
			this.butScrollEnd = new System.Windows.Forms.Button();
			this.butScrollStart = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.butScrollUpStep = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.butScrollDownStep = new System.Windows.Forms.Button();
			this.timerStepDown = new System.Windows.Forms.Timer(this.components);
			this.timerStepUp = new System.Windows.Forms.Timer(this.components);
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelDraw
			// 
			this.panelDraw.AutoScroll = true;
			this.panelDraw.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelDraw.Location = new System.Drawing.Point(68, 0);
			this.panelDraw.Margin = new System.Windows.Forms.Padding(0);
			this.panelDraw.Name = "panelDraw";
			this.panelDraw.Size = new System.Drawing.Size(106, 22);
			this.panelDraw.TabIndex = 0;
			this.panelDraw.Paint += new System.Windows.Forms.PaintEventHandler(this.Legend_Paint);
			this.panelDraw.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelDraw_MouseDown);
			this.panelDraw.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelDraw_MouseMove);
			this.panelDraw.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelDraw_MouseUp);
			// 
			// butScrollEnd
			// 
			this.butScrollEnd.BackColor = System.Drawing.SystemColors.Control;
			this.butScrollEnd.Location = new System.Drawing.Point(36, 1);
			this.butScrollEnd.Name = "butScrollEnd";
			this.butScrollEnd.Size = new System.Drawing.Size(30, 21);
			this.butScrollEnd.TabIndex = 9;
			this.butScrollEnd.Text = ">>";
			this.butScrollEnd.UseVisualStyleBackColor = false;
			this.butScrollEnd.Click += new System.EventHandler(this.butScrollEnd_Click);
			// 
			// butScrollStart
			// 
			this.butScrollStart.BackColor = System.Drawing.SystemColors.Control;
			this.butScrollStart.Location = new System.Drawing.Point(1, 1);
			this.butScrollStart.Name = "butScrollStart";
			this.butScrollStart.Size = new System.Drawing.Size(30, 21);
			this.butScrollStart.TabIndex = 10;
			this.butScrollStart.Text = "<<";
			this.butScrollStart.UseVisualStyleBackColor = false;
			this.butScrollStart.Click += new System.EventHandler(this.butScrollStart_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
			this.tableLayoutPanel1.Controls.Add(this.panelDraw, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel2, 2, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(242, 22);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.butScrollUpStep);
			this.panel1.Controls.Add(this.butScrollStart);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(68, 22);
			this.panel1.TabIndex = 12;
			// 
			// butScrollUpStep
			// 
			this.butScrollUpStep.BackColor = System.Drawing.SystemColors.Control;
			this.butScrollUpStep.Location = new System.Drawing.Point(35, 1);
			this.butScrollUpStep.Name = "butScrollUpStep";
			this.butScrollUpStep.Size = new System.Drawing.Size(30, 21);
			this.butScrollUpStep.TabIndex = 11;
			this.butScrollUpStep.Text = "<";
			this.butScrollUpStep.UseVisualStyleBackColor = false;
			this.butScrollUpStep.Click += new System.EventHandler(this.butScrollUpStep_Click);
			this.butScrollUpStep.MouseDown += new System.Windows.Forms.MouseEventHandler(this.butScrollUpStep_MouseDown);
			this.butScrollUpStep.MouseUp += new System.Windows.Forms.MouseEventHandler(this.butScrollUpStep_MouseUp);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.butScrollDownStep);
			this.panel2.Controls.Add(this.butScrollEnd);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(174, 0);
			this.panel2.Margin = new System.Windows.Forms.Padding(0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(68, 22);
			this.panel2.TabIndex = 13;
			// 
			// butScrollDownStep
			// 
			this.butScrollDownStep.BackColor = System.Drawing.SystemColors.Control;
			this.butScrollDownStep.Location = new System.Drawing.Point(3, 1);
			this.butScrollDownStep.Name = "butScrollDownStep";
			this.butScrollDownStep.Size = new System.Drawing.Size(30, 21);
			this.butScrollDownStep.TabIndex = 10;
			this.butScrollDownStep.Text = ">";
			this.butScrollDownStep.UseVisualStyleBackColor = false;
			this.butScrollDownStep.Click += new System.EventHandler(this.butScrollDownStep_Click);
			this.butScrollDownStep.MouseDown += new System.Windows.Forms.MouseEventHandler(this.butScrollDownStep_MouseDown);
			this.butScrollDownStep.MouseUp += new System.Windows.Forms.MouseEventHandler(this.butScrollDownStep_MouseUp);
			// 
			// timerStepDown
			// 
			this.timerStepDown.Interval = 200;
			this.timerStepDown.Tick += new System.EventHandler(this.timerStepDown_Tick);
			// 
			// timerStepUp
			// 
			this.timerStepUp.Interval = 200;
			this.timerStepUp.Tick += new System.EventHandler(this.timerStepUp_Tick);
			// 
			// Legend
			// 
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "Legend";
			this.Size = new System.Drawing.Size(242, 22);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		private System.Windows.Forms.Button butScrollStart;
		private System.Windows.Forms.Button butScrollEnd;
		private PanelNoFlicker panelDraw;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button butScrollUpStep;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button butScrollDownStep;
		private System.Windows.Forms.Timer timerStepDown;
		private System.Windows.Forms.Timer timerStepUp;
	}
}
