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

		private int _countPagesPrinted=0;

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
			List<Userod> listUserods=Userods.GetUsers();
			for(int i=0;i<listUserods.Count;i++) {
				comboUsers.Items.Add(listUserods[i].UserName,listUserods[i]);
				if(Security.CurUser.UserNum==listUserods[i].UserNum) {//Select the current user by default.
					comboUsers.SetSelected(comboUsers.Items.Count-1,true);
				}
			}
			FillGridWebChatSurveys();
		}

		private void FillGridWebChatSurveys() {
			gridWebChatSurveys.BeginUpdate();
			if(gridWebChatSurveys.Columns.Count==0) {
				gridWebChatSurveys.Columns.Add(new GridColumn("DateTime",80,HorizontalAlignment.Center,GridSortingStrategy.DateParse));
				gridWebChatSurveys.Columns.Add(new GridColumn("Owner",80,HorizontalAlignment.Left));
				gridWebChatSurveys.Columns.Add(new GridColumn("PatNum",80,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
				gridWebChatSurveys.Columns.Add(new GridColumn("SessionNum",90,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
				gridWebChatSurveys.Columns.Add(new GridColumn("Rating",80,HorizontalAlignment.Center));
				gridWebChatSurveys.Columns.Add(new GridColumn("Comment",100,HorizontalAlignment.Left){ IsWidthDynamic=true });
				gridWebChatSurveys.Columns.Add(new GridColumn("Experience",100,HorizontalAlignment.Left){ IsWidthDynamic=true });
			}
			gridWebChatSurveys.ListGridRows.Clear();
			List<WebChatSession> listWebChatSessions=null;
			List<WebChatSurvey> listWebChatSurveys=null;
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
			List<Userod> listUserodsSelected=comboUsers.GetListSelected<Userod>();
			List<string> listUsernamesSelected=listUserodsSelected.Select(x => x.UserName).ToList();
			for(int i=0;i<listWebChatSurveys.Count;i++) {
				if(!string.IsNullOrEmpty(textSessionNum.Text) && !listWebChatSurveys[i].WebChatSessionNum.ToString().Contains(textSessionNum.Text)) {
					continue;
				}
				string searchText=textSurveyTextContains.Text.ToLower();
				if(!string.IsNullOrEmpty(searchText)
					&& !listWebChatSurveys[i].CustomerComment.ToLower().Contains(searchText) && !listWebChatSurveys[i].CustomerExperience.ToLower().Contains(searchText))
				{
					continue;
				}
				WebChatSession webChatSession=listWebChatSessions.First(x => x.WebChatSessionNum==listWebChatSurveys[i].WebChatSessionNum);
				if(listUsernamesSelected.Count > 0 && !listUsernamesSelected.Contains(webChatSession.TechName)) {
					continue;
				}
				if(listWebChatSurveys[i].TechRating==TechSurveyRating.Positive) {
					totalPositive++;
				}
				else if(listWebChatSurveys[i].TechRating==TechSurveyRating.Neutral) {
					totalNeutral++;
				}
				else if(listWebChatSurveys[i].TechRating==TechSurveyRating.Negative) {
					totalNegative++;
				}
				GridRow row=new GridRow();
				row.Tag=webChatSession;
				row.Cells.Add(webChatSession.DateTcreated.ToString());
				row.Cells.Add(webChatSession.TechName);
				row.Cells.Add((webChatSession.PatNum==0)?"":webChatSession.PatNum.ToString());
				row.Cells.Add(webChatSession.WebChatSessionNum.ToString());
				row.Cells.Add(listWebChatSurveys[i].TechRating.ToString());
				row.Cells.Add(listWebChatSurveys[i].CustomerComment);
				row.Cells.Add(listWebChatSurveys[i].CustomerExperience);
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
			_countPagesPrinted=0;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,"Web Chat Survey Report",PrintoutOrientation.Landscape);
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			if(_countPagesPrinted==0) {//First page only
				float xPos=(int)(e.PageBounds.Left+e.PageBounds.Width/2-(float)gridWebChatSurveys.Width/2);//How grid left is calculated in ODGrid.PrintPage
				e.Graphics.DrawString(labelTotals.Text,Font,System.Drawing.Brushes.Black,new PointF(xPos,e.MarginBounds.Top));
			}
			if(gridWebChatSurveys.PrintPage(e.Graphics,_countPagesPrinted,e.PageBounds,e.MarginBounds.Top+30)==-1) {
				e.HasMorePages=true;
				_countPagesPrinted++;
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

	}
}