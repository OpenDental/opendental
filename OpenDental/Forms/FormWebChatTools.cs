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
		private int _countPagesPrinted;
		private int _heightHeadingPrint;
		public FormWebChatTools() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWebChatTools_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(() => FillGridWebChats(),
				checkShowEndedSessions,dateRangeWebChat,comboUsers,comboSupervisors,textChatTextContains,textSessionNum,checkShowAi);
			dateRangeWebChat.SetDateTimeFrom(DateTime.Today.AddDays(-7));
			dateRangeWebChat.SetDateTimeTo(DateTime.Today.AddYears(1));
			comboUsers.Items.Clear();
			List<Userod> listUserods=Userods.GetUsers();
			for(int i=0;i<listUserods.Count;i++) {
				comboUsers.Items.Add(listUserods[i].UserName,listUserods[i]);
				if(Security.CurUser.UserNum==listUserods[i].UserNum) {//Select the current user by default.
					comboUsers.SetSelected(comboUsers.Items.Count-1,true);
				}
			}
			comboUsers.IncludeAll=true;
			comboSupervisors.Items.Clear();
			_listEmployees=Employees.GetDeepCopy(isShort:true);
			List<Employee> listEmployeesSupervisors=new List<Employee>();
			for(int i=0;i<_listEmployees.Count;i++){ //Creates a list of all the supervisors i.e. every employee that has an employee that reports to them
				if(_listEmployees[i].ReportsTo==0){
					continue;
				}
				if(listEmployeesSupervisors.Any(x=>x.EmployeeNum==_listEmployees[i].ReportsTo)){
					continue;
				}
				listEmployeesSupervisors.Add(Employees.GetEmp(_listEmployees[i].ReportsTo));
			}
			listEmployeesSupervisors=listEmployeesSupervisors.OrderBy(x=>x.FName).ToList();
			comboSupervisors.Items.AddList(listEmployeesSupervisors,x=>x.FName);
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
			WebChatSession session=(WebChatSession)gridWebChatSessions.ListGridRows[e.Row].Tag;
			switch(session.SessionType) {
				case WebChatSessionType.WebSupport:
					FormWebChatSession formWebChatSession=new FormWebChatSession(session,
						() => {
							FillGridWebChats();//Refresh to show if session ended or if tech has been assigned to the session.
						});
					formWebChatSession.Show();
					break;
				case WebChatSessionType.OpenAi:
					FormAiChatSession formAiSession=new FormAiChatSession(session,() => {
							FillGridWebChats();//Refresh to show if session ended or if tech has been assigned to the session.
						});
					formAiSession.Show();
					break;
			}
		}

		///<summary>Only for ODHQ triage.</summary>
		private void FillGridWebChats() {
			gridWebChatSessions.BeginUpdate();
			gridWebChatSessions.ListGridRows.Clear();
			gridWebChatSessions.Columns.Clear();
			if(checkShowAi.Checked) {
				gridWebChatSessions.Columns.Add(new GridColumn("AI",50,HorizontalAlignment.Center));
			}
			gridWebChatSessions.Columns.Add(new GridColumn("Start DateTime",90,HorizontalAlignment.Center));
			gridWebChatSessions.Columns.Add(new GridColumn("End DateTime",90,HorizontalAlignment.Center));
			gridWebChatSessions.Columns.Add(new GridColumn("Owner",80,HorizontalAlignment.Left));
			gridWebChatSessions.Columns.Add(new GridColumn("PatNum",80,HorizontalAlignment.Right));
			gridWebChatSessions.Columns.Add(new GridColumn("Patient Name",180,HorizontalAlignment.Left));
			gridWebChatSessions.Columns.Add(new GridColumn("SessionNum",90,HorizontalAlignment.Right));
			gridWebChatSessions.Columns.Add(new GridColumn("Question",100,HorizontalAlignment.Left){ IsWidthDynamic=true });
			List<WebChatSessionType> listSessionsTypes=new List<WebChatSessionType>() { WebChatSessionType.WebSupport };
			if(checkShowAi.Checked) {
				listSessionsTypes.Add(WebChatSessionType.OpenAi);
			}
			List<WebChatSession> listWebChatSessions=null;
			List<WebChatMessage> listWebChatMessages=null;
			//If connection to webchat is lost or not visible from a specific network location, then continue, in order to keep the call center operational.
			ODException.SwallowAnyException(() => {
				listWebChatSessions=WebChatSessions.GetSessions(checkShowEndedSessions.Checked,dateRangeWebChat.GetDateTimeFrom(),dateRangeWebChat.GetDateTimeTo(),listSessionsTypes.ToArray());
				listWebChatMessages=WebChatMessages.GetAllForSessions(listWebChatSessions.Select(x => x.WebChatSessionNum).ToArray());
			});
			if(listWebChatSessions!=null) {//Will only be null if connection to webchat database failed.
				List<Userod> listUserodsSelected=comboUsers.GetListSelected<Userod>();
				if(!comboSupervisors.IsAllSelected) { 
					List<Employee> listEmployeesSupervisorsSelected=comboSupervisors.GetListSelected<Employee>();
					List<Employee> listEmployeesReportees=_listEmployees.Where(x => listEmployeesSupervisorsSelected.Any(y => y.EmployeeNum==x.ReportsTo)).ToList();
					listUserodsSelected=listUserodsSelected.Where(x => listEmployeesReportees.Any(y => y.EmployeeNum==x.EmployeeNum)).ToList();
				}
				List<string> listUsernamesSelected=listUserodsSelected.Select(x => x.UserName).ToList();
				string searchText=textChatTextContains.Text.ToLower();
				Dictionary<long,string> dictPatName=Patients.GetPatientNames(listWebChatSessions.Select(x => x.PatNum).Distinct().ToList());
				//11-08-2021 Jason Salmon - Jordan approved dictionarySessionNumWebChatMessages for speed improvements. See B32544.
				Dictionary<long,List<WebChatMessage>> dictionarySessionNumWebChatMessages=listWebChatMessages.GroupBy(x => x.WebChatSessionNum).ToDictionary(x => x.Key,x => x.ToList());
				for(int i=0;i<listWebChatSessions.Count;i++) {
					bool isRelevantSession=false;
					if(!dictionarySessionNumWebChatMessages.TryGetValue(listWebChatSessions[i].WebChatSessionNum,out List<WebChatMessage> listChatMessagesForSession)) {
						listChatMessagesForSession=new List<WebChatMessage>();
					}
					if(string.IsNullOrEmpty(listWebChatSessions[i].TechName) && !checkShowEndedSessions.Checked) {
						isRelevantSession=true;//Unclaimed web chat sessions are visible to all technicians, so they can consider taking ownership.
					}
					else if(listUsernamesSelected.Contains(listWebChatSessions[i].TechName)) {
						isRelevantSession=true;
					}
					else if(listChatMessagesForSession.Exists(x => listUsernamesSelected.Contains(x.UserName))) {
						isRelevantSession=true;
					}
					if(!isRelevantSession) {
						continue;
					}
					List<string> listMessagesForSession=listChatMessagesForSession.Select(x => x.MessageText.ToLower()).ToList();
					if(!string.IsNullOrEmpty(textSessionNum.Text) && !listWebChatSessions[i].WebChatSessionNum.ToString().Contains(textSessionNum.Text)) {
						continue;
					}
					if(!string.IsNullOrEmpty(searchText) && !listWebChatSessions[i].QuestionText.ToLower().Contains(searchText)
						&& !listMessagesForSession.Exists(x => x.Contains(searchText)))
					{
						continue;
					}
					GridRow row=new GridRow();
					row.Tag=listWebChatSessions[i];
					if(checkShowAi.Checked) {
						string value=(listWebChatSessions[i].SessionType==WebChatSessionType.OpenAi?"X":"");
						if(listChatMessagesForSession.Any(x => x.NeedsFollowUp)) {
							value+="(f)";
						}
						row.Cells.Add(value);
					}
					row.Cells.Add(listWebChatSessions[i].DateTcreated.ToString());
					if(listWebChatSessions[i].DateTend.Year < 1880) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(listWebChatSessions[i].DateTend.ToString());
					}
					TimeSpan timeSpanWebChatAge=new TimeSpan(DateTime.Now.Ticks-listWebChatSessions[i].DateTcreated.Ticks);
					Color colorBackground=FormTasks.ColorLightRed;
					if(timeSpanWebChatAge.TotalMinutes>2) {
						//Change the color to red if the time since it was created is greater than 2 minutes.
						colorBackground=Color.Red;
					}
					if(string.IsNullOrEmpty(listWebChatSessions[i].TechName)) {
						row.Cells.Add("NEEDS TECH");
						row.Bold=true;
						row.ColorBackG=colorBackground;
						row.ColorText=Color.White;
					}
					else {
						row.Cells.Add(listWebChatSessions[i].TechName);
					}
					if(listWebChatSessions[i].PatNum==0) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(listWebChatSessions[i].PatNum.ToString());
					}
					if(listWebChatSessions[i].PatNum==0) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(dictPatName[listWebChatSessions[i].PatNum]);
					}
					row.Cells.Add(listWebChatSessions[i].WebChatSessionNum.ToString());
					row.Cells.Add(listWebChatSessions[i].QuestionText);
					gridWebChatSessions.ListGridRows.Add(row);
				}
			}
			gridWebChatSessions.EndUpdate();
			labelCountValue.Text=gridWebChatSessions.ListGridRows.Count.ToString();
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle rectangleBounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			using Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			using Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=rectangleBounds.Top;
			int center=rectangleBounds.X+rectangleBounds.Width/2;
			#region printHeading
			if(!_isHeadingPrinted) {
				text=Lan.g(this,"Web Chat Sessions");
				g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				text=dateRangeWebChat.GetDateTimeFrom().ToShortDateString()+" "+Lan.g(this,"to")+" "+dateRangeWebChat.GetDateTimeTo().ToShortDateString();
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=25;
				_isHeadingPrinted=true;
				_heightHeadingPrint=yPos;
			}
			#endregion
			yPos=gridWebChatSessions.PrintPage(g,_countPagesPrinted,rectangleBounds,_heightHeadingPrint);
			_countPagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
		}

		private void butPrint_Click(object sender,EventArgs e) {
			List<GridRow> listRows=gridWebChatSessions.ListGridRows;
			for(int i = 0;i<listRows.Count;i++) {
				listRows[i].ColorText=Color.Black;
				//Sets text of rows with red/light red backgrounds to black so they appear in printout
			}
			int widthGrid=gridWebChatSessions.Width;
			gridWebChatSessions.Width=1040; // Grid is too large to print, decreases width size to fit landscape orientation. Approved by Jordan.
			_countPagesPrinted=0;
			_isHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Web chat sessions printed."),PrintoutOrientation.Landscape);
			gridWebChatSessions.Width=widthGrid; //Automatically reset grid width back to original size once print is done.
			gridWebChatSessions.ScrollValue=0;//After printing, extra whitespace at bottom of grid. Recommended fix by Jordan. Scroll user to top to clear the extra whitespace.
			// Resets the text of rows with red/light red backgrounds back to white
			listRows=gridWebChatSessions.ListGridRows.FindAll(x => x.ColorBackG==Color.Red || x.ColorBackG==FormTasks.ColorLightRed);
			for(int i=0;i<listRows.Count;i++) {
				listRows[i].ColorText=Color.White;
			}
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

		private void menuItemAiChat_Click(object sender, EventArgs e) {
			if(gridWebChatSessions.SelectedGridRows.Count==0) {
				return;
			}
			if(!(gridWebChatSessions.SelectedGridRows[0].Tag is WebChatSession session)) {
				return;
			}
			InputBox inputBox=new InputBox(Lans.g(this,"Confirm AI Chat Prompt"),session.QuestionText);
			inputBox.ShowDialog();
			if(inputBox.IsDialogOK && !inputBox.StringResult.IsNullOrEmpty()) {
				FormAiChatSession formAiSession=new FormAiChatSession(inputBox.StringResult,() => {
					FillGridWebChats();//Refresh to show if session ended or if tech has been assigned to the session.
				});
				formAiSession.Show();
			}
		}
	}
}