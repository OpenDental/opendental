namespace OpenDental {
	partial class JobManagerUserOverview {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.labelActiveJobs = new System.Windows.Forms.Label();
			this.dataJobs = new System.Windows.Forms.DataGridView();
			this.Priority = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DateEntered = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.LastPhaseChange = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Title = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.panel2 = new System.Windows.Forms.Panel();
			this.groupOverview = new System.Windows.Forms.GroupBox();
			this.textQuotedTotal = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.textHighPrio = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textJobsNoEst = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textDevHours = new System.Windows.Forms.Label();
			this.textReviewRequests = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.textLongestHourEst = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textActiveAdvisorJobs = new System.Windows.Forms.Label();
			this.textConceptJobs = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textJobsOnHold = new System.Windows.Forms.Label();
			this.textDevelopmentJobs = new System.Windows.Forms.Label();
			this.textWriteupJobs = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.butAssignOnDeck = new OpenDental.UI.Button();
			this.panel1.SuspendLayout();
			this.panel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataJobs)).BeginInit();
			this.panel2.SuspendLayout();
			this.groupOverview.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.panel3);
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(959, 188);
			this.panel1.TabIndex = 0;
			// 
			// panel3
			// 
			this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel3.Controls.Add(this.labelActiveJobs);
			this.panel3.Controls.Add(this.dataJobs);
			this.panel3.Location = new System.Drawing.Point(0, 0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(549, 188);
			this.panel3.TabIndex = 20;
			// 
			// labelActiveJobs
			// 
			this.labelActiveJobs.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelActiveJobs.ForeColor = System.Drawing.SystemColors.Window;
			this.labelActiveJobs.Location = new System.Drawing.Point(0, 0);
			this.labelActiveJobs.Name = "labelActiveJobs";
			this.labelActiveJobs.Size = new System.Drawing.Size(549, 13);
			this.labelActiveJobs.TabIndex = 20;
			this.labelActiveJobs.Text = "labelActiveJobs";
			this.labelActiveJobs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// dataJobs
			// 
			this.dataJobs.AllowUserToAddRows = false;
			this.dataJobs.AllowUserToDeleteRows = false;
			this.dataJobs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataJobs.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.dataJobs.BackgroundColor = System.Drawing.SystemColors.ControlDarkDark;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataJobs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataJobs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataJobs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Priority,
            this.DateEntered,
            this.LastPhaseChange,
            this.Title});
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.ControlLight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataJobs.DefaultCellStyle = dataGridViewCellStyle2;
			this.dataJobs.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.dataJobs.Location = new System.Drawing.Point(0, 16);
			this.dataJobs.MultiSelect = false;
			this.dataJobs.Name = "dataJobs";
			this.dataJobs.ReadOnly = true;
			this.dataJobs.RowHeadersVisible = false;
			this.dataJobs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataJobs.Size = new System.Drawing.Size(549, 172);
			this.dataJobs.TabIndex = 19;
			this.dataJobs.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataJobs_CellDoubleClick);
			// 
			// Priority
			// 
			this.Priority.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.Priority.HeaderText = "Priority";
			this.Priority.Name = "Priority";
			this.Priority.ReadOnly = true;
			// 
			// DateEntered
			// 
			this.DateEntered.HeaderText = "Date Entered";
			this.DateEntered.Name = "DateEntered";
			this.DateEntered.ReadOnly = true;
			// 
			// LastPhaseChange
			// 
			this.LastPhaseChange.FillWeight = 150F;
			this.LastPhaseChange.HeaderText = "Last Phase Change";
			this.LastPhaseChange.Name = "LastPhaseChange";
			this.LastPhaseChange.ReadOnly = true;
			// 
			// Title
			// 
			this.Title.FillWeight = 400F;
			this.Title.HeaderText = "Title";
			this.Title.Name = "Title";
			this.Title.ReadOnly = true;
			// 
			// panel2
			// 
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.panel2.AutoSize = true;
			this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel2.Controls.Add(this.groupOverview);
			this.panel2.Controls.Add(this.groupBox1);
			this.panel2.Location = new System.Drawing.Point(550, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(409, 175);
			this.panel2.TabIndex = 18;
			// 
			// groupOverview
			// 
			this.groupOverview.AutoSize = true;
			this.groupOverview.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupOverview.BackColor = System.Drawing.SystemColors.Window;
			this.groupOverview.Controls.Add(this.textQuotedTotal);
			this.groupOverview.Controls.Add(this.label12);
			this.groupOverview.Controls.Add(this.textHighPrio);
			this.groupOverview.Controls.Add(this.label7);
			this.groupOverview.Controls.Add(this.label8);
			this.groupOverview.Controls.Add(this.textJobsNoEst);
			this.groupOverview.Controls.Add(this.label4);
			this.groupOverview.Controls.Add(this.textDevHours);
			this.groupOverview.Controls.Add(this.textReviewRequests);
			this.groupOverview.Controls.Add(this.label10);
			this.groupOverview.Controls.Add(this.label11);
			this.groupOverview.Controls.Add(this.textLongestHourEst);
			this.groupOverview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupOverview.Location = new System.Drawing.Point(0, 0);
			this.groupOverview.Name = "groupOverview";
			this.groupOverview.Size = new System.Drawing.Size(213, 175);
			this.groupOverview.TabIndex = 15;
			this.groupOverview.TabStop = false;
			this.groupOverview.Text = "Allen Overview";
			// 
			// textQuotedTotal
			// 
			this.textQuotedTotal.Location = new System.Drawing.Point(125, 136);
			this.textQuotedTotal.Name = "textQuotedTotal";
			this.textQuotedTotal.Size = new System.Drawing.Size(82, 23);
			this.textQuotedTotal.TabIndex = 20;
			this.textQuotedTotal.Text = "$4";
			this.textQuotedTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(5, 136);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(111, 23);
			this.label12.TabIndex = 19;
			this.label12.Text = "Quoted Total";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textHighPrio
			// 
			this.textHighPrio.Location = new System.Drawing.Point(125, 113);
			this.textHighPrio.Name = "textHighPrio";
			this.textHighPrio.Size = new System.Drawing.Size(82, 23);
			this.textHighPrio.TabIndex = 18;
			this.textHighPrio.Text = "4";
			this.textHighPrio.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(5, 113);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(111, 23);
			this.label7.TabIndex = 17;
			this.label7.Text = "High Priority Jobs";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(2, 66);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(114, 23);
			this.label8.TabIndex = 15;
			this.label8.Text = "Jobs With No Est.";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textJobsNoEst
			// 
			this.textJobsNoEst.Location = new System.Drawing.Point(125, 66);
			this.textJobsNoEst.Name = "textJobsNoEst";
			this.textJobsNoEst.Size = new System.Drawing.Size(82, 23);
			this.textJobsNoEst.TabIndex = 16;
			this.textJobsNoEst.Text = "3";
			this.textJobsNoEst.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(2, 20);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(114, 23);
			this.label4.TabIndex = 13;
			this.label4.Text = "Total Dev Hours ";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDevHours
			// 
			this.textDevHours.Location = new System.Drawing.Point(125, 20);
			this.textDevHours.Name = "textDevHours";
			this.textDevHours.Size = new System.Drawing.Size(82, 23);
			this.textDevHours.TabIndex = 14;
			this.textDevHours.Text = "200";
			this.textDevHours.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textReviewRequests
			// 
			this.textReviewRequests.Location = new System.Drawing.Point(125, 89);
			this.textReviewRequests.Name = "textReviewRequests";
			this.textReviewRequests.Size = new System.Drawing.Size(82, 23);
			this.textReviewRequests.TabIndex = 12;
			this.textReviewRequests.Text = "4";
			this.textReviewRequests.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(5, 89);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(111, 23);
			this.label10.TabIndex = 11;
			this.label10.Text = "Review Requests";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(2, 43);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(114, 23);
			this.label11.TabIndex = 7;
			this.label11.Text = "Longest Hour Est.";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLongestHourEst
			// 
			this.textLongestHourEst.Location = new System.Drawing.Point(125, 43);
			this.textLongestHourEst.Name = "textLongestHourEst";
			this.textLongestHourEst.Size = new System.Drawing.Size(82, 23);
			this.textLongestHourEst.TabIndex = 8;
			this.textLongestHourEst.Text = "200";
			this.textLongestHourEst.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBox1
			// 
			this.groupBox1.AutoSize = true;
			this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupBox1.BackColor = System.Drawing.SystemColors.Window;
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.textActiveAdvisorJobs);
			this.groupBox1.Controls.Add(this.textConceptJobs);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.textJobsOnHold);
			this.groupBox1.Controls.Add(this.textDevelopmentJobs);
			this.groupBox1.Controls.Add(this.textWriteupJobs);
			this.groupBox1.Controls.Add(this.label14);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
			this.groupBox1.Location = new System.Drawing.Point(213, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(196, 175);
			this.groupBox1.TabIndex = 13;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Owner Job Count";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(33, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 23);
			this.label2.TabIndex = 5;
			this.label2.Text = "Concept Jobs";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textActiveAdvisorJobs
			// 
			this.textActiveAdvisorJobs.Location = new System.Drawing.Point(118, 87);
			this.textActiveAdvisorJobs.Name = "textActiveAdvisorJobs";
			this.textActiveAdvisorJobs.Size = new System.Drawing.Size(72, 23);
			this.textActiveAdvisorJobs.TabIndex = 12;
			this.textActiveAdvisorJobs.Text = "4";
			this.textActiveAdvisorJobs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textConceptJobs
			// 
			this.textConceptJobs.Location = new System.Drawing.Point(118, 18);
			this.textConceptJobs.Name = "textConceptJobs";
			this.textConceptJobs.Size = new System.Drawing.Size(72, 23);
			this.textConceptJobs.TabIndex = 6;
			this.textConceptJobs.Text = "1";
			this.textConceptJobs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(3, 87);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(102, 23);
			this.label6.TabIndex = 11;
			this.label6.Text = "Active Advisor Jobs";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(36, 41);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 23);
			this.label1.TabIndex = 7;
			this.label1.Text = "Writeup Jobs";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textJobsOnHold
			// 
			this.textJobsOnHold.Location = new System.Drawing.Point(120, 111);
			this.textJobsOnHold.Name = "textJobsOnHold";
			this.textJobsOnHold.Size = new System.Drawing.Size(69, 23);
			this.textJobsOnHold.TabIndex = 10;
			this.textJobsOnHold.Text = "5";
			this.textJobsOnHold.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textDevelopmentJobs
			// 
			this.textDevelopmentJobs.Location = new System.Drawing.Point(118, 64);
			this.textDevelopmentJobs.Name = "textDevelopmentJobs";
			this.textDevelopmentJobs.Size = new System.Drawing.Size(72, 23);
			this.textDevelopmentJobs.TabIndex = 10;
			this.textDevelopmentJobs.Text = "3";
			this.textDevelopmentJobs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textWriteupJobs
			// 
			this.textWriteupJobs.Location = new System.Drawing.Point(118, 41);
			this.textWriteupJobs.Name = "textWriteupJobs";
			this.textWriteupJobs.Size = new System.Drawing.Size(72, 23);
			this.textWriteupJobs.TabIndex = 8;
			this.textWriteupJobs.Text = "2";
			this.textWriteupJobs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(6, 111);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(99, 23);
			this.label14.TabIndex = 9;
			this.label14.Text = "Jobs On Hold";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(9, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(96, 23);
			this.label3.TabIndex = 9;
			this.label3.Text = "Development Jobs";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butAssignOnDeck
			// 
			this.butAssignOnDeck.BackColor = System.Drawing.SystemColors.ControlLight;
			this.butAssignOnDeck.Location = new System.Drawing.Point(0, 0);
			this.butAssignOnDeck.Name = "butAssignOnDeck";
			this.butAssignOnDeck.Size = new System.Drawing.Size(75, 23);
			this.butAssignOnDeck.TabIndex = 0;
			this.butAssignOnDeck.UseVisualStyleBackColor = false;
			// 
			// JobManagerUserOverview
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.panel1);
			this.Name = "JobManagerUserOverview";
			this.Size = new System.Drawing.Size(959, 188);
			this.Load += new System.EventHandler(this.JobManagerUserOverview_Load);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataJobs)).EndInit();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.groupOverview.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.GroupBox groupOverview;
		private System.Windows.Forms.Label textReviewRequests;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label textJobsOnHold;
		private System.Windows.Forms.Label textLongestHourEst;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label textActiveAdvisorJobs;
		private System.Windows.Forms.Label textConceptJobs;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label textDevelopmentJobs;
		private System.Windows.Forms.Label textWriteupJobs;
		private System.Windows.Forms.Label label3;
		private UI.Button butAssignOnDeck;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label textJobsNoEst;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label textDevHours;
		private System.Windows.Forms.Label textQuotedTotal;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label textHighPrio;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.DataGridView dataJobs;
		private System.Windows.Forms.Label labelActiveJobs;
		private System.Windows.Forms.DataGridViewTextBoxColumn Priority;
		private System.Windows.Forms.DataGridViewTextBoxColumn DateEntered;
		private System.Windows.Forms.DataGridViewTextBoxColumn LastPhaseChange;
		private System.Windows.Forms.DataGridViewTextBoxColumn Title;
	}
}
