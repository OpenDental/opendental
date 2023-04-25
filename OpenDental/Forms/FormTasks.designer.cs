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
			this.userControlTasks1 = new OpenDental.UserControlTasks();
			this.gridWebChatSessions = new OpenDental.UI.GridOD();
			this.splitContainer = new OpenDental.UI.SplitContainer();
			this.splitterPanel1 = new OpenDental.UI.SplitterPanel();
			this.splitterPanel2 = new OpenDental.UI.SplitterPanel();
			this.splitContainer.SuspendLayout();
			this.splitterPanel1.SuspendLayout();
			this.splitterPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// userControlTasks1
			// 
			this.userControlTasks1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.userControlTasks1.Location = new System.Drawing.Point(0, 0);
			this.userControlTasks1.Name = "userControlTasks1";
			this.userControlTasks1.Size = new System.Drawing.Size(1230, 539);
			this.userControlTasks1.TabIndex = 0;
			this.userControlTasks1.TaskTab = OpenDental.UserControlTasksTab.ForUser;
			this.userControlTasks1.FillGridEvent += new OpenDental.UserControlTasks.FillGridEventHandler(this.UserControlTasks1_FillGridEvent);
			this.userControlTasks1.Resize += new System.EventHandler(this.userControlTasks1_Resize);
			// 
			// gridWebChatSessions
			// 
			this.gridWebChatSessions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridWebChatSessions.Location = new System.Drawing.Point(0, 0);
			this.gridWebChatSessions.Name = "gridWebChatSessions";
			this.gridWebChatSessions.Size = new System.Drawing.Size(1230, 153);
			this.gridWebChatSessions.TabIndex = 1;
			this.gridWebChatSessions.Title = "Web Chat Sessions";
			this.gridWebChatSessions.TranslationName = "gridWebChatSessions";
			this.gridWebChatSessions.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridWebChatSessions_CellDoubleClick);
			// 
			// splitContainer
			// 
			this.splitContainer.Controls.Add(this.splitterPanel1);
			this.splitContainer.Controls.Add(this.splitterPanel2);
			this.splitContainer.Cursor = System.Windows.Forms.Cursors.Default;
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.splitContainer.Panel1 = this.splitterPanel1;
			this.splitContainer.Panel2 = this.splitterPanel2;
			this.splitContainer.Size = new System.Drawing.Size(1230, 696);
			this.splitContainer.SplitterDistance = 539;
			this.splitContainer.TabIndex = 12;
			// 
			// splitterPanel1
			// 
			this.splitterPanel1.Controls.Add(this.userControlTasks1);
			this.splitterPanel1.Location = new System.Drawing.Point(0, 0);
			this.splitterPanel1.Name = "splitterPanel1";
			this.splitterPanel1.Size = new System.Drawing.Size(1230, 539);
			this.splitterPanel1.TabIndex = 13;
			// 
			// splitterPanel2
			// 
			this.splitterPanel2.Controls.Add(this.gridWebChatSessions);
			this.splitterPanel2.Location = new System.Drawing.Point(0, 543);
			this.splitterPanel2.Name = "splitterPanel2";
			this.splitterPanel2.Size = new System.Drawing.Size(1230, 153);
			this.splitterPanel2.TabIndex = 14;
			// 
			// FormTasks
			// 
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.splitContainer);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTasks";
			this.Text = "Tasks";
			this.Load += new System.EventHandler(this.FormTasks_Load);
			this.splitContainer.ResumeLayout(false);
			this.splitterPanel1.ResumeLayout(false);
			this.splitterPanel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private UI.GridOD gridWebChatSessions;
		private UserControlTasks userControlTasks1;
		private UI.SplitContainer splitContainer;
		private UI.SplitterPanel splitterPanel1;
		private UI.SplitterPanel splitterPanel2;
	}
}
