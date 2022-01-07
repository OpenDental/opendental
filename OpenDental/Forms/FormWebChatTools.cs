using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWebChatTools:FormODBase {
		private List<Employee> _listEmployees;
		private bool _isHeadingPrinted;
		private int _pagesPrinted;
		private int _heightHeadingPrint;

		public FormWebChatTools() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWebChatTools_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(() => FillGridWebChats(),
				checkShowEndedSessions,dateRangeWebChat,comboUsers,comboSupervisors,textChatTextContains,textSessionNum);
			dateRangeWebChat.SetDateTimeFrom(DateTime.Today.AddDays(-7));
			dateRangeWebChat.SetDateTimeTo(DateTime.Today.AddYears(1));
			comboUsers.Items.Clear();
			foreach(Userod user in Userods.GetUsers()) {
				comboUsers.Items.Add(user.UserName,user);
				if(Security.CurUser.UserNum==user.UserNum) {//Select the current user by default.
					comboUsers.SetSelected(comboUsers.Items.Count-1,true);
				}
			}
			comboUsers.IncludeAll=true;
			comboSupervisors.Items.Clear();
			_listEmployees=Employees.GetDeepCopy(isShort:true);
			List<Employee> listEmployeeSupervisors=new List<Employee>();
			for(int i=0;i<_listEmployees.Count;i++){ //Creates a list of all the supervisors i.e. every employee that has an employee that reports to them
				if(_listEmployees[i].ReportsTo==0){
					continue;
				}
				if(listEmployeeSupervisors.Any(x=>x.EmployeeNum==_listEmployees[i].ReportsTo)){
					continue;
				}
				listEmployeeSupervisors.Add(Employees.GetEmp(_listEmployees[i].ReportsTo));
			}
			listEmployeeSupervisors=listEmployeeSupervisors.OrderBy(x=>x.FName).ToList();
			comboSupervisors.Items.AddList(listEmployeeSupervisors,x=>x.FName);
			comboSupervisors.IncludeAll=true;
			comboSupervisors.IsAllSelected=true;
			FillGridWebChats();
		}

		public override void ProcessSignalODs(List<Signalod> listSignals) {
			if(listSignals.Exists(x => x.IType==InvalidType.WebChatSessions)) {
				FillGridWebChats();
			}
		}

		private void gridWebChatSessions_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			FormWebChatSession form=new FormWebChatSession((WebChatSession)gridWebChatSessions.ListGridRows[e.Row].Tag,
				() => {
					FillGridWebChats();//Refresh to show if session ended or if tech has been assigned to the session.
				});
			form.Show();
		}

		///<summary>Only for ODHQ triage.</summary>
		private void FillGridWebChats() {
			gridWebChatSessions.BeginUpdate();
			gridWebChatSessions.ListGridRows.Clear();
			if(gridWebChatSessions.Columns.Count==0) {
				gridWebChatSessions.Columns.Add(new GridColumn("Start DateTime",90,HorizontalAlignment.Center));
				gridWebChatSessions.Columns.Add(new GridColumn("End DateTime",90,HorizontalAlignment.Center));
				gridWebChatSessions.Columns.Add(new GridColumn("Owner",80,HorizontalAlignment.Left));
				gridWebChatSessions.Columns.Add(new GridColumn("PatNum",80,HorizontalAlignment.Right));
				gridWebChatSessions.Columns.Add(new GridColumn("Patient Name",180,HorizontalAlignment.Left));
				gridWebChatSessions.Columns.Add(new GridColumn("SessionNum",90,HorizontalAlignment.Right));
				gridWebChatSessions.Columns.Add(new GridColumn("Question",100,HorizontalAlignment.Left){ IsWidthDynamic=true });
			}
			List <WebChatSession> listChatSessions=null;
			List <WebChatMessage> listChatMessages=null;
			//If connection to webchat is lost or not visible from a specific network location, then continue, in order to keep the call center operational.
			ODException.SwallowAnyException(() => {
				listChatSessions=WebChatSessions.GetSessions(checkShowEndedSessions.Checked,dateRangeWebChat.GetDateTimeFrom(),dateRangeWebChat.GetDateTimeTo());
				listChatMessages=WebChatMessages.GetAllForSessions(listChatSessions.Select(x => x.WebChatSessionNum).ToArray());
			});
			if(listChatSessions!=null) {//Will only be null if connection to webchat database failed.
				List<Userod> listSelectedUsers=comboUsers.GetListSelected<Userod>();
				if(!comboSupervisors.IsAllSelected) { 
					List<Employee> listSelectedSupervisors=comboSupervisors.GetListSelected<Employee>();
					List<Employee> listReportees=_listEmployees.Where(x => listSelectedSupervisors.Any(y => y.EmployeeNum==x.ReportsTo)).ToList();
					listSelectedUsers=listSelectedUsers.Where(x => listReportees.Any(y => y.EmployeeNum==x.EmployeeNum)).ToList();
				}
				List<string> listSelectedUsernames=listSelectedUsers.Select(x => x.UserName).ToList();
				string searchText=textChatTextContains.Text.ToLower();
				Dictionary<long,string> dictPatName=Patients.GetPatientNames(listChatSessions.Select(x => x.PatNum).Distinct().ToList());
				//11-08-2021 Jason Salmon - Jordan approved dictionarySessionNumWebChatMessages for speed improvements. See B32544.
				Dictionary<long,List<WebChatMessage>> dictionarySessionNumWebChatMessages=listChatMessages.GroupBy(x => x.WebChatSessionNum).ToDictionary(x => x.Key,x => x.ToList());
				foreach(WebChatSession webChatSession in listChatSessions) {
					bool isRelevantSession=false;
					if(!dictionarySessionNumWebChatMessages.TryGetValue(webChatSession.WebChatSessionNum,out List<WebChatMessage> listChatMessagesForSession)) {
						listChatMessagesForSession=new List<WebChatMessage>();
					}
					if(string.IsNullOrEmpty(webChatSession.TechName)) {
						isRelevantSession=true;//Unclaimed web chat sessions are visible to all technicians, so they can consider taking ownership.
					}
					else if(listSelectedUsernames.Contains(webChatSession.TechName)) {
						isRelevantSession=true;
					}
					else if(listChatMessagesForSession.Exists(x => listSelectedUsernames.Contains(x.UserName))) {
						isRelevantSession=true;
					}
					if(!isRelevantSession) {
						continue;
					}
					List <string> listMessagesForSession=listChatMessagesForSession.Select(x => x.MessageText.ToLower()).ToList();
					if(!string.IsNullOrEmpty(textSessionNum.Text) && !webChatSession.WebChatSessionNum.ToString().Contains(textSessionNum.Text)) {
						continue;
					}
					if(!string.IsNullOrEmpty(searchText) && !webChatSession.QuestionText.ToLower().Contains(searchText)
						&& !listMessagesForSession.Exists(x => x.Contains(searchText)))
					{
						continue;
					}
					GridRow row=new GridRow();
					row.Tag=webChatSession;
					row.Cells.Add(webChatSession.DateTcreated.ToString());
					row.Cells.Add((webChatSession.DateTend.Year < 1880)?"":webChatSession.DateTend.ToString());
					TimeSpan spanWebChatAge=new TimeSpan(DateTime.Now.Ticks-webChatSession.DateTcreated.Ticks);
					Color backgroundColor=Color.FromArgb(247,110,110);//Changing this color will also need to change color in FormTasks.cs
					if(spanWebChatAge.TotalMinutes>2) {
						//Change the color to red if the time since it was created is greater than 2 minutes.
						backgroundColor=Color.Red;
					}
					if(string.IsNullOrEmpty(webChatSession.TechName)) {
						row.Cells.Add("NEEDS TECH");
						row.Bold=true;
						row.ColorBackG=backgroundColor;
						row.ColorText=Color.White;
					}
					else {
						row.Cells.Add(webChatSession.TechName);
					}
					row.Cells.Add((webChatSession.PatNum==0)?"":webChatSession.PatNum.ToString());
					row.Cells.Add((webChatSession.PatNum==0)?"":dictPatName[webChatSession.PatNum]);
					row.Cells.Add(webChatSession.WebChatSessionNum.ToString());
					row.Cells.Add(webChatSession.QuestionText);
					gridWebChatSessions.ListGridRows.Add(row);
				}
			}
			gridWebChatSessions.EndUpdate();
			labelCountValue.Text=gridWebChatSessions.ListGridRows.Count.ToString();
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!_isHeadingPrinted) {
				text=Lan.g(this,"Web Chat Sessions");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				text=dateRangeWebChat.GetDateTimeFrom().ToShortDateString()+" "+Lan.g(this,"to")+" "+dateRangeWebChat.GetDateTimeTo().ToShortDateString();
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=25;
				_isHeadingPrinted=true;
				_heightHeadingPrint=yPos;
			}
			#endregion
			yPos=gridWebChatSessions.PrintPage(g,_pagesPrinted,bounds,_heightHeadingPrint);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}
		

		private void butPrint_Click(object sender,EventArgs e) {
			gridWebChatSessions.ListGridRows.OfType<GridRow>().ForEach(x=>x.ColorText=Color.Black); // Sets text of rows with red/light red backgrounds to black so they appear in printout
			int gridWidth=gridWebChatSessions.Width;
			gridWebChatSessions.Width=1040; // Grid is too large to print, decreases width size to fit landscape orientation. Approved by Jordan.
			_pagesPrinted=0;
			_isHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Web chat sessions printed."),PrintoutOrientation.Landscape);
			gridWebChatSessions.Width=gridWidth; //Automatically reset grid width back to original size once print is done.
			gridWebChatSessions.ScrollValue=0;//After printing, extra whitespace at bottom of grid. Recommended fix by Jordan. Scroll user to top to clear the extra whitespace.
			// Resets the text of rows with red/light red backgrounds back to white
			gridWebChatSessions.ListGridRows.OfType<GridRow>().Where(x => x.ColorBackG==Color.Red || x.ColorBackG==Color.FromArgb(247,110,110)).ForEach(x => x.ColorText=Color.White);
		}

		private void butClearFilters_Click(object sender,EventArgs e) {
			checkShowEndedSessions.Checked=false;
			textSessionNum.Text="";
			dateRangeWebChat.SetDateTimeFrom(DateTime.Today.AddDays(-7));
			dateRangeWebChat.SetDateTimeTo(DateTime.Today.AddYears(1));
			textChatTextContains.Text="";
			comboUsers.SelectedIndices.Clear();
			comboUsers.IsAllSelected=true;
			comboSupervisors.SelectedIndices.Clear();
			comboSupervisors.IsAllSelected=true;
		}

		private void butExport_Click(object sender,EventArgs e) {
			gridWebChatSessions.Export(gridWebChatSessions.Title);
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

		
	}
}