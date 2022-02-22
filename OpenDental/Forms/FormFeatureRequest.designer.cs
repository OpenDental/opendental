using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormFeatureRequest {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFeatureRequest));
			this.label1 = new System.Windows.Forms.Label();
			this.labelVote = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textSearch = new System.Windows.Forms.TextBox();
			this.butSearch = new OpenDental.UI.Button();
			this.buttonAdd = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butClose = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butEdit = new OpenDental.UI.Button();
			this.checkMine = new System.Windows.Forms.CheckBox();
			this.checkMyVotes = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 23);
			this.label1.TabIndex = 0;
			// 
			// labelVote
			// 
			this.labelVote.Location = new System.Drawing.Point(218, 9);
			this.labelVote.Name = "labelVote";
			this.labelVote.Size = new System.Drawing.Size(511, 16);
			this.labelVote.TabIndex = 51;
			this.labelVote.Text = "Vote for your favorite features here. Please remember that we cannot ever give an" +
    "y time estimates.";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(5, 33);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(90, 18);
			this.label5.TabIndex = 56;
			this.label5.Text = "Search terms";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSearch
			// 
			this.textSearch.Location = new System.Drawing.Point(94, 33);
			this.textSearch.Name = "textSearch";
			this.textSearch.Size = new System.Drawing.Size(167, 20);
			this.textSearch.TabIndex = 0;
			// 
			// butSearch
			// 
			this.butSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSearch.Location = new System.Drawing.Point(267, 30);
			this.butSearch.Name = "butSearch";
			this.butSearch.Size = new System.Drawing.Size(75, 24);
			this.butSearch.TabIndex = 1;
			this.butSearch.Text = "Search";
			this.butSearch.Click += new System.EventHandler(this.butSearch_Click);
			// 
			// buttonAdd
			// 
			this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.buttonAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonAdd.Location = new System.Drawing.Point(12, 630);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(75, 24);
			this.buttonAdd.TabIndex = 2;
			this.buttonAdd.Text = "Add";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HasMultilineHeaders = true;
			this.gridMain.Location = new System.Drawing.Point(12, 59);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(861, 566);
			this.gridMain.TabIndex = 59;
			this.gridMain.Title = "Feature Requests";
			this.gridMain.TranslationName = "TableRequests";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(798, 630);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 4;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(717, 630);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butEdit
			// 
			this.butEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEdit.Location = new System.Drawing.Point(485, 630);
			this.butEdit.Name = "butEdit";
			this.butEdit.Size = new System.Drawing.Size(75, 24);
			this.butEdit.TabIndex = 62;
			this.butEdit.Text = "Edit";
			this.butEdit.Click += new System.EventHandler(this.butEdit_Click);
			// 
			// checkMine
			// 
			this.checkMine.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkMine.Location = new System.Drawing.Point(352, 33);
			this.checkMine.Name = "checkMine";
			this.checkMine.Size = new System.Drawing.Size(71, 18);
			this.checkMine.TabIndex = 63;
			this.checkMine.Text = "Mine";
			this.checkMine.UseVisualStyleBackColor = true;
			this.checkMine.CheckedChanged += new System.EventHandler(this.checkMine_CheckedChanged);
			// 
			// checkMyVotes
			// 
			this.checkMyVotes.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkMyVotes.Location = new System.Drawing.Point(411, 33);
			this.checkMyVotes.Name = "checkMyVotes";
			this.checkMyVotes.Size = new System.Drawing.Size(86, 18);
			this.checkMyVotes.TabIndex = 64;
			this.checkMyVotes.Text = "My Votes";
			this.checkMyVotes.UseVisualStyleBackColor = true;
			this.checkMyVotes.CheckedChanged += new System.EventHandler(this.checkMyVotes_CheckedChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(93, 628);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(355, 29);
			this.label2.TabIndex = 65;
			this.label2.Text = "Voting on existing requests is more effective than adding new requests.\r\nIf you a" +
    "dd a request, put votes on it right away.";
			// 
			// FormFeatureRequest
			// 
			this.AcceptButton = this.butSearch;
			this.ClientSize = new System.Drawing.Size(882, 657);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.checkMyVotes);
			this.Controls.Add(this.checkMine);
			this.Controls.Add(this.butEdit);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butSearch);
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.textSearch);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.labelVote);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFeatureRequest";
			this.Text = "Feature Requests";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormUpdate_FormClosing);
			this.Load += new System.EventHandler(this.FormFeatureRequest_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.Label label1;
		private Label labelVote;
		private Label label5;
		private TextBox textSearch;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button buttonAdd;
		private OpenDental.UI.Button butSearch;
		private UI.Button butOK;
		private UI.Button butEdit;
		private CheckBox checkMine;
		private CheckBox checkMyVotes;
		private Label label2;
	}
}
