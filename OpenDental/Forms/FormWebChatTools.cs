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

		public FormWebChatTools() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWebChatTools_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(() => FillGridWebChats(),
				checkShowEndedSessions,dateRangeWebChat,comboUsers,textChatTextContains,textSessionNum);
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
			if(gridWebChatSessions.ListGridColumns.Count==0) {
				gridWebChatSessions.ListGridColumns.Add(new GridColumn("Start DateTime",90,HorizontalAlignment.Center));
				gridWebChatSessions.ListGridColumns.Add(new GridColumn("End DateTime",90,HorizontalAlignment.Center));
				gridWebChatSessions.ListGridColumns.Add(new GridColumn("Owner",80,HorizontalAlignment.Left));
				gridWebChatSessions.ListGridColumns.Add(new GridColumn("PatNum",80,HorizontalAlignment.Right));
				gridWebChatSessions.ListGridColumns.Add(new GridColumn("Patient Name",180,HorizontalAlignment.Left));
				gridWebChatSessions.ListGridColumns.Add(new GridColumn("SessionNum",90,HorizontalAlignment.Right));
				gridWebChatSessions.ListGridColumns.Add(new GridColumn("Question",100,HorizontalAlignment.Left){ IsWidthDynamic=true });
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
				List<string> listSelectedUsernames=listSelectedUsers.Select(x => x.UserName).ToList();
				string searchText=textChatTextContains.Text.ToLower();
				Dictionary<long,string> dictPatName=Patients.GetPatientNames(listChatSessions.Select(x => x.PatNum).Distinct().ToList());
				foreach(WebChatSession webChatSession in listChatSessions) {
					bool isRelevantSession=false;
					if(string.IsNullOrEmpty(webChatSession.TechName)) {
						isRelevantSession=true;//Unclaimed web chat sessions are visible to all technicians, so they can consider taking ownership.
					}
					else if(listSelectedUsernames.Count==0) {
						isRelevantSession=true;//Filter for usernames is empty.  Show chat sessions for all users.
					}
					else if(listSelectedUsernames.Contains(webChatSession.TechName)) {
						isRelevantSession=true;
					}
					else if(listChatMessages.Exists(x => x.WebChatSessionNum==webChatSession.WebChatSessionNum && listSelectedUsernames.Contains(x.UserName))) {
						isRelevantSession=true;
					}
					if(!isRelevantSession) {
						continue;
					}
					List <string> listMessagesForSession=listChatMessages
						.Where(x => x.WebChatSessionNum==webChatSession.WebChatSessionNum)
						.Select(x => x.MessageText.ToLower())
						.ToList();
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

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

		
	}
}