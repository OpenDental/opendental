using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.UI {
	public class ODGridPageNav : UserControl {

		#region UI controls
		private TextBox textJumpToPage;
		private Panel panelPageLinks;
		private Button butFirst;
		private Button butLast;
		private LinkLabel linkOne;
		private LinkLabel linkTwo;
		private LinkLabel linkThree;
		private LinkLabel linkFour;
		private Button butRight;
		private Button butLeft;
		#endregion

		private GridOD _grid;
		private int _pageCur;
		
		[Category("Data"),Description("The grid that this control will navigate through.  This grid must have paging enabled.")]
		public GridOD Grid {
			get { return _grid; }
			set { _grid=value; }
		}

		public ODGridPageNav() {
			//This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}
		
		private void ODGridPageNav_Load(object sender,EventArgs e) {
			if(Grid==null) {//Should only be null when adding to the designer or drawing in FormSheetDefEdit.cs
				return;
			}
			Grid.PageChanged+=PagingChangeEventHandler;
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textJumpToPage = new System.Windows.Forms.TextBox();
			this.panelPageLinks = new System.Windows.Forms.Panel();
			this.linkOne = new System.Windows.Forms.LinkLabel();
			this.linkTwo = new System.Windows.Forms.LinkLabel();
			this.linkThree = new System.Windows.Forms.LinkLabel();
			this.linkFour = new System.Windows.Forms.LinkLabel();
			this.butFirst = new System.Windows.Forms.Button();
			this.butLast = new System.Windows.Forms.Button();
			this.butRight = new System.Windows.Forms.Button();
			this.butLeft = new System.Windows.Forms.Button();
			this.panelPageLinks.SuspendLayout();
			this.SuspendLayout();
			// 
			// textJumpToPage
			// 
			this.textJumpToPage.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.textJumpToPage.BackColor = System.Drawing.SystemColors.Window;
			this.textJumpToPage.Location = new System.Drawing.Point(123, 3);
			this.textJumpToPage.MaxLength = 10000;
			this.textJumpToPage.Name = "textJumpToPage";
			this.textJumpToPage.Size = new System.Drawing.Size(34, 20);
			this.textJumpToPage.TabIndex = 8;
			this.textJumpToPage.Text = "3";
			this.textJumpToPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.textJumpToPage.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textJumpToPage_KeyUp);
			// 
			// panelPageLinks
			// 
			this.panelPageLinks.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.panelPageLinks.Controls.Add(this.linkOne);
			this.panelPageLinks.Controls.Add(this.linkTwo);
			this.panelPageLinks.Controls.Add(this.linkThree);
			this.panelPageLinks.Controls.Add(this.linkFour);
			this.panelPageLinks.Location = new System.Drawing.Point(55, 3);
			this.panelPageLinks.Name = "panelPageLinks";
			this.panelPageLinks.Size = new System.Drawing.Size(170, 20);
			this.panelPageLinks.TabIndex = 6;
			// 
			// linkOne
			// 
			this.linkOne.Location = new System.Drawing.Point(1, 1);
			this.linkOne.Name = "linkOne";
			this.linkOne.Size = new System.Drawing.Size(33, 17);
			this.linkOne.TabIndex = 9;
			this.linkOne.TabStop = true;
			this.linkOne.Text = "1";
			this.linkOne.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.linkOne.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClickedEventHandler);
			// 
			// linkTwo
			// 
			this.linkTwo.Location = new System.Drawing.Point(34, 1);
			this.linkTwo.Name = "linkTwo";
			this.linkTwo.Size = new System.Drawing.Size(34, 17);
			this.linkTwo.TabIndex = 10;
			this.linkTwo.TabStop = true;
			this.linkTwo.Text = "2";
			this.linkTwo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.linkTwo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClickedEventHandler);
			// 
			// linkThree
			// 
			this.linkThree.Location = new System.Drawing.Point(102, 1);
			this.linkThree.Name = "linkThree";
			this.linkThree.Size = new System.Drawing.Size(34, 17);
			this.linkThree.TabIndex = 11;
			this.linkThree.TabStop = true;
			this.linkThree.Text = "4";
			this.linkThree.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.linkThree.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClickedEventHandler);
			// 
			// linkFour
			// 
			this.linkFour.Location = new System.Drawing.Point(136, 1);
			this.linkFour.Name = "linkFour";
			this.linkFour.Size = new System.Drawing.Size(33, 17);
			this.linkFour.TabIndex = 12;
			this.linkFour.TabStop = true;
			this.linkFour.Text = "5";
			this.linkFour.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.linkFour.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClickedEventHandler);
			// 
			// butFirst
			// 
			this.butFirst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butFirst.Location = new System.Drawing.Point(0, 0);
			this.butFirst.Name = "butFirst";
			this.butFirst.Size = new System.Drawing.Size(27, 26);
			this.butFirst.TabIndex = 4;
			this.butFirst.Text = "<<";
			this.butFirst.UseVisualStyleBackColor = true;
			this.butFirst.Click += new System.EventHandler(this.butFirst_Click);
			// 
			// butLast
			// 
			this.butLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butLast.Location = new System.Drawing.Point(253, 0);
			this.butLast.Name = "butLast";
			this.butLast.Size = new System.Drawing.Size(27, 26);
			this.butLast.TabIndex = 9;
			this.butLast.Text = ">>";
			this.butLast.UseVisualStyleBackColor = true;
			this.butLast.Click += new System.EventHandler(this.butLast_Click);
			// 
			// butRight
			// 
			this.butRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRight.Location = new System.Drawing.Point(227, 0);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(26, 26);
			this.butRight.TabIndex = 10;
			this.butRight.Text = ">";
			this.butRight.UseVisualStyleBackColor = true;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butLeft
			// 
			this.butLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butLeft.Location = new System.Drawing.Point(27, 0);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(26, 26);
			this.butLeft.TabIndex = 11;
			this.butLeft.Text = "<";
			this.butLeft.UseVisualStyleBackColor = true;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// ODGridPageNav
			// 
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.butLeft);
			this.Controls.Add(this.butRight);
			this.Controls.Add(this.butLast);
			this.Controls.Add(this.butFirst);
			this.Controls.Add(this.textJumpToPage);
			this.Controls.Add(this.panelPageLinks);
			this.MinimumSize = new System.Drawing.Size(143, 26);
			this.Name = "ODGridPageNav";
			this.Size = new System.Drawing.Size(280, 26);
			this.Load += new System.EventHandler(this.ODGridPageNav_Load);
			this.panelPageLinks.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Component Designer generated code

		#region UI navigation events
		public void LinkClickedEventHandler(object sender,LinkLabelLinkClickedEventArgs e) {
			if(e.Link.LinkData==null) {
				return;//Most likely clicked a link when no data had been loaded yet.
			}
			Grid.NavigateToPage((int)e.Link.LinkData);
		}

		private void butFirst_Click(object sender,EventArgs e) {
			Grid.NavigateToPage(-1);//Safe even if value is not valid page.
		}

		private void butLeft_Click(object sender,EventArgs e) {
			Grid.NavigateToPage(_pageCur-1);//Safe even if value is not valid page.
		}

		private void butRight_Click(object sender,EventArgs e) {
			Grid.NavigateToPage(_pageCur+1);//Safe even if value is not valid page.
		}

		private void butLast_Click(object sender,EventArgs e) {
			Grid.NavigateToPage(999999);//Safe even if value is not valid page.
		}
		
		private void textJumpToPage_KeyUp(object sender,KeyEventArgs e) {
			int newPage=0;
			switch(e.KeyCode) {
				case Keys.Left:
					newPage=(_pageCur-1);//Safe even if value is not valid page.
					break;
				case Keys.Right:
					newPage=(_pageCur+1);//Safe even if value is not valid page.;
					break;
				case Keys.Enter:
					newPage=PIn.Int(textJumpToPage.Text,false);//Safe even if value is not valid page.
					break;
				default:
				return;
			}
			Grid.NavigateToPage(newPage);
		}
		#endregion
		
		public void PagingChangeEventHandler(object sender, ODGridPageEventArgs e) {
			_pageCur=e.PageCur;
			textJumpToPage.Text=POut.Int(e.PageCur);
			//We reuse the same controls and just change thier text and data to avoid flicker in UI.
			for(int i=0;i<panelPageLinks.Controls.Count;i++) {
				LinkLabel pageLink=(LinkLabel)panelPageLinks.Controls[i];
				int pageLinkVal=e.ListLinkVals[i];
				if(pageLinkVal==-1) {
					pageLink.Visible=false;
					continue;
				}
				pageLink.Visible=true;
				pageLink.Text=pageLinkVal.ToString();
				pageLink.Links[0].LinkData=pageLinkVal;
			}
		}

	}


}
