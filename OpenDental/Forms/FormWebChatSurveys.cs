using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDental.UI;
using System.Drawing;

namespace OpenDental {
	public partial class FormWebChatSurveys:FormODBase {

		private int _pagesPrinted=0;

		public FormWebChatSurveys() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWebChatSurveys_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(() => { FillGridWebChatSurveys(); },textSessionNum,textSurveyTextContains,dateRangeWebChat,comboUsers);
			dateRangeWebChat.SetDateTimeFrom(DateTime.Now.AddDays(-7));
			dateRangeWebChat.SetDateTimeTo(DateTime.Now);
			comboUsers.Items.Clear();
			foreach(Userod user in Userods.GetUsers()) {
				comboUsers.Items.Add(user.UserName,user);
				if(Security.CurUser.UserNum==user.UserNum) {//Select the current user by default.
					comboUsers.SetSelected(comboUsers.Items.Count-1,true);
				}
			}
			FillGridWebChatSurveys();
		}

		private void FillGridWebChatSurveys() {
			gridWebChatSurveys.BeginUpdate();
			if(gridWebChatSurveys.ListGridColumns.Count==0) {
				gridWebChatSurveys.ListGridColumns.Add(new GridColumn("DateTime",80,HorizontalAlignment.Center,GridSortingStrategy.DateParse));
				gridWebChatSurveys.ListGridColumns.Add(new GridColumn("Owner",80,HorizontalAlignment.Left));
				gridWebChatSurveys.ListGridColumns.Add(new GridColumn("PatNum",80,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
				gridWebChatSurveys.ListGridColumns.Add(new GridColumn("SessionNum",90,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
				gridWebChatSurveys.ListGridColumns.Add(new GridColumn("Rating",80,HorizontalAlignment.Center));
				gridWebChatSurveys.ListGridColumns.Add(new GridColumn("Comment",100,HorizontalAlignment.Left){ IsWidthDynamic=true });
				gridWebChatSurveys.ListGridColumns.Add(new GridColumn("Experience",100,HorizontalAlignment.Left){ IsWidthDynamic=true });
			}
			gridWebChatSurveys.ListGridRows.Clear();
			List <WebChatSession> listWebChatSessions=null;
			List <WebChatSurvey> listWebChatSurveys=null;
			//If connection to webchat is lost or not visible from a specific network location, then continue, in order to keep the call center operational.
			ODException.SwallowAnyException(() => {
				listWebChatSessions=WebChatSessions.GetSessions(true,dateRangeWebChat.GetDateTimeFrom(),dateRangeWebChat.GetDateTimeTo());
				listWebChatSurveys=WebChatSurveys.GetSurveysForSessions(listWebChatSessions.Select(x => x.WebChatSessionNum).ToList());
			});
			if(listWebChatSessions==null || listWebChatSurveys==null) {
				return;
			}
			int totalPositive=0;
			int totalNeutral=0;
			int totalNegative=0;
			List<Userod> listSelectedUsers=comboUsers.GetListSelected<Userod>();
			List<string> listSelectedUsernames=listSelectedUsers.Select(x => x.UserName).ToList();
			foreach(WebChatSurvey webChatSurvey in listWebChatSurveys) {
				if(!string.IsNullOrEmpty(textSessionNum.Text) && !webChatSurvey.WebChatSessionNum.ToString().Contains(textSessionNum.Text)) {
					continue;
				}
				string searchText=textSurveyTextContains.Text.ToLower();
				if(!string.IsNullOrEmpty(searchText)
					&& !webChatSurvey.CustomerComment.ToLower().Contains(searchText) && !webChatSurvey.CustomerExperience.ToLower().Contains(searchText))
				{
					continue;
				}
				WebChatSession webChatSession=listWebChatSessions.First(x => x.WebChatSessionNum==webChatSurvey.WebChatSessionNum);
				if(listSelectedUsernames.Count > 0 && !listSelectedUsernames.Contains(webChatSession.TechName)) {
					continue;
				}
				if(webChatSurvey.TechRating==TechSurveyRating.Positive) {
					totalPositive++;
				}
				else if(webChatSurvey.TechRating==TechSurveyRating.Neutral) {
					totalNeutral++;
				}
				else if(webChatSurvey.TechRating==TechSurveyRating.Negative) {
					totalNegative++;
				}
				GridRow row=new GridRow();
				row.Tag=webChatSession;
				row.Cells.Add(webChatSession.DateTcreated.ToString());
				row.Cells.Add(webChatSession.TechName);
				row.Cells.Add((webChatSession.PatNum==0)?"":webChatSession.PatNum.ToString());
				row.Cells.Add(webChatSession.WebChatSessionNum.ToString());
				row.Cells.Add(webChatSurvey.TechRating.ToString());
				row.Cells.Add(webChatSurvey.CustomerComment);
				row.Cells.Add(webChatSurvey.CustomerExperience);
				gridWebChatSurveys.ListGridRows.Add(row);
			}
			gridWebChatSurveys.EndUpdate();
			labelTotals.Text="Totals: Positive ("+totalPositive+") Neutral ("+totalNeutral+") Negative ("+totalNegative+")";
		}

		private void gridWebChatSurveys_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			WebChatSession webChatSession=(WebChatSession)gridWebChatSurveys.ListGridRows[e.Row].Tag;
			using FormWebChatSession formWebChatSession=new FormWebChatSession(webChatSession,() => { });
			formWebChatSession.ShowDialog();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,"Web Chat Survey Report",PrintoutOrientation.Landscape);
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			if(_pagesPrinted==0) {//First page only
				float xPos=(int)(e.PageBounds.Left+e.PageBounds.Width/2-(float)gridWebChatSurveys.Width/2);//How grid left is calculated in ODGrid.PrintPage
				e.Graphics.DrawString(labelTotals.Text,Font,System.Drawing.Brushes.Black,new PointF(xPos,e.MarginBounds.Top));
			}
			if(gridWebChatSurveys.PrintPage(e.Graphics,_pagesPrinted,e.PageBounds,e.MarginBounds.Top+30)==-1) {
				e.HasMorePages=true;
				_pagesPrinted++;
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

	}
}