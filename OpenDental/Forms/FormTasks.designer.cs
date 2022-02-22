using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTasks {
		private System.ComponentModel.IContainer components=null;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTasks));
			this.splitter = new System.Windows.Forms.SplitContainer();
			this.userControlTasks1 = new OpenDental.UserControlTasks();
			this.gridWebChatSessions = new OpenDental.UI.GridOD();
			((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
			this.splitter.Panel1.SuspendLayout();
			this.splitter.Panel2.SuspendLayout();
			this.splitter.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitter
			// 
			this.splitter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitter.Location = new System.Drawing.Point(0, 0);
			this.splitter.Name = "splitter";
			this.splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitter.Panel1
			// 
			this.splitter.Panel1.Controls.Add(this.userControlTasks1);
			this.splitter.Panel1MinSize = 150;
			// 
			// splitter.Panel2
			// 
			this.splitter.Panel2.Controls.Add(this.gridWebChatSessions);
			this.splitter.Panel2MinSize = 150;
			this.splitter.Size = new System.Drawing.Size(1230, 696);
			this.splitter.SplitterDistance = 540;
			this.splitter.TabIndex = 0;
			this.splitter.TabStop = false;
			this.splitter.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitter_SplitterMoved);
			// 
			// userControlTasks1
			// 
			this.userControlTasks1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.userControlTasks1.Location = new System.Drawing.Point(0, 0);
			this.userControlTasks1.Name = "userControlTasks1";
			this.userControlTasks1.Size = new System.Drawing.Size(1228, 538);
			this.userControlTasks1.TabIndex = 0;
			this.userControlTasks1.TaskTab = OpenDental.UserControlTasksTab.ForUser;
			this.userControlTasks1.FillGridEvent += new OpenDental.UserControlTasks.FillGridEventHandler(this.UserControlTasks1_FillGridEvent);
			this.userControlTasks1.Resize += new System.EventHandler(this.userControlTasks1_Resize);
			// 
			// gridWebChatSessions
			// 
			this.gridWebChatSessions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridWebChatSessions.Location = new System.Drawing.Point(0, 3);
			this.gridWebChatSessions.Name = "gridWebChatSessions";
			this.gridWebChatSessions.Size = new System.Drawing.Size(1228, 147);
			this.gridWebChatSessions.TabIndex = 1;
			this.gridWebChatSessions.Title = "Web Chat Sessions";
			this.gridWebChatSessions.TranslationName = "gridWebChatSessions";
			this.gridWebChatSessions.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridWebChatSessions_CellDoubleClick);
			// 
			// FormTasks
			// 
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.splitter);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTasks";
			this.Text = "Tasks";
			this.Load += new System.EventHandler(this.FormTasks_Load);
			this.splitter.Panel1.ResumeLayout(false);
			this.splitter.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
			this.splitter.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private UI.GridOD gridWebChatSessions;
		private SplitContainer splitter;
		private UserControlTasks userControlTasks1;
	}
}
