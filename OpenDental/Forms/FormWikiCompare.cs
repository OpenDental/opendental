using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWikiCompare:FormODBase {
		private string _wikiPageContentLeft="";
		private string _wikiPageContentRight="";
		private string _wikiPageTitleLeft="";
		private string _wikiPageTitleRight="";

		public FormWikiCompare() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		public void InitializeRevisionsSelected(WikiPageHist WikiPageHistLeft, WikiPageHist WikiPageHistRight) {
			_wikiPageContentLeft=WikiPages.GetWikiPageContentWithWikiPageTitles(WikiPageHistLeft.PageContent);
			_wikiPageContentRight=WikiPages.GetWikiPageContentWithWikiPageTitles(WikiPageHistRight.PageContent);
			_wikiPageTitleLeft=WikiPageHistLeft.PageTitle+" "+WikiPageHistLeft.DateTimeSaved.ToString();
			_wikiPageTitleRight=WikiPageHistRight.PageTitle+" "+WikiPageHistRight.DateTimeSaved.ToString();
		}

		private void FormWikiCompare_Load(object sender,EventArgs e) {
			ResizeControls();
			UpdateODcodeBoxes();
		}

		private void ResizeControls() {
			//if(ClientSize.Width<20 || ClientSize.Height<20) {
			//	return;//stop trying to resize
			//}
			int xDivider=ClientSize.Width/2;
			//set 4 objects
			LayoutManager.MoveSize(textContentLeft,new Size(xDivider-15,textContentLeft.Height));
			if(textContentLeft.Width<=20) {
				LayoutManager.MoveSize(textContentLeft,new Size(21,textContentLeft.Height));
			}
			if(textContentLeft.Height<=20) {
				LayoutManager.MoveSize(textContentLeft,new Size(xDivider-15,21));
			}
			LayoutManager.MoveSize(textContentRight,new Size(xDivider-15,textContentRight.Height));
			if(textContentRight.Width<=20) {
				LayoutManager.MoveSize(textContentRight,new Size(21,textContentRight.Height));
			}
			if(textContentRight.Height<=20) {
				LayoutManager.MoveSize(textContentRight,new Size(xDivider-15,21));
			}
			LayoutManager.MoveLocation(textContentRight,new Point(xDivider+3,textContentRight.Location.Y));
			LayoutManager.MoveLocation(labelHelpNew,new Point(xDivider+15,labelHelpNew.Location.Y));
			LayoutManager.MoveLocation(labelTitleNew,new Point(xDivider+15,labelTitleNew.Location.Y));
		}

		private void UpdateODcodeBoxes() {
			//Add Title and Content Text
			textContentLeft.Text=_wikiPageContentLeft;
			textContentRight.Text=_wikiPageContentRight;
			label1.Text=_wikiPageTitleLeft;
			labelTitleNew.Text=_wikiPageTitleRight;
			List<string> listContentLeft=_wikiPageContentLeft.Split('\n').ToList();
			List<string> listContentRight=_wikiPageContentRight.Split('\n').ToList();
			CompareCodeboxes(listContentLeft,listContentRight,textContentLeft,textContentRight,Color.FromArgb(255, 192, 192),Color.FromArgb(192, 255, 192));
		}

		private void CompareCodeboxes(List<string> listContentOld,List<string> listContentNew,ODcodeBox textContentOld,ODcodeBox textContentNew,Color highlighterOld,Color highlighterNew) {
			for(int i=0;i<listContentOld.Count;i++) {
				string lineOld = listContentOld[i];
				if(!listContentNew.Exists(x => x==lineOld)) {
					textContentOld.HighlightLine(i,0,lineOld.Length,highlighterOld);
				}
			}
			for(int i=0;i<listContentNew.Count;i++) {
				string lineNew = listContentNew[i];
				if(!listContentOld.Exists(x => x==lineNew)) {
					textContentNew.HighlightLine(i,0,lineNew.Length,highlighterNew);
				}
			}
		}

		private void FormWikiCompare_SizeChange(object sender,EventArgs e) {
			ResizeControls();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
