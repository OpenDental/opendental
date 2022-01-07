using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Drawing;

namespace OpenDental.UI {
	class ODInternalCustomerGrids : UserControl {
		private TabControl tabHqBugControl;
		private TabPage tabBugsFixed;
		private GridOD gridBugsFixed;
		private TabPage tabBugSubs;
		private GridOD gridBugSubmissions;
		private Dictionary<long,string> _dictHqBugSubNames=new Dictionary<long, string>();
		private Button butRefresh;
		private Patient _patCur;
		private List<BugSubmission> _listBugSubs=new List<BugSubmission>();
		private Label label1;
		private TabPage tabBugsInProgress;
		private GridOD gridBugsInProgress;
		private ODDateRangePicker datePickerHqBugSub;
		private List<JobLink> _listJobLinks=new List<JobLink>();

		[Browsable(false)]
		public Patient PatCur {
			get {
				return _patCur;
			}
			set {
				bool needsRefresh=(value?.PatNum!=_patCur?.PatNum);
				_patCur=value;
				if(needsRefresh) {
					FillGrids();
				}
			}
		}

		public ODInternalCustomerGrids() {
			InitializeComponent();
		}
		
		private void InitializeComponent() {
			this.tabHqBugControl = new System.Windows.Forms.TabControl();
			this.tabBugsInProgress = new System.Windows.Forms.TabPage();
			this.gridBugsInProgress = new OpenDental.UI.GridOD();
			this.tabBugsFixed = new System.Windows.Forms.TabPage();
			this.gridBugsFixed = new OpenDental.UI.GridOD();
			this.tabBugSubs = new System.Windows.Forms.TabPage();
			this.gridBugSubmissions = new OpenDental.UI.GridOD();
			this.label1 = new System.Windows.Forms.Label();
			this.butRefresh = new OpenDental.UI.Button();
			this.datePickerHqBugSub = new OpenDental.UI.ODDateRangePicker();
			this.tabHqBugControl.SuspendLayout();
			this.tabBugsInProgress.SuspendLayout();
			this.tabBugsFixed.SuspendLayout();
			this.tabBugSubs.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabHqBugControl
			// 
			this.tabHqBugControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabHqBugControl.Controls.Add(this.tabBugsInProgress);
			this.tabHqBugControl.Controls.Add(this.tabBugsFixed);
			this.tabHqBugControl.Controls.Add(this.tabBugSubs);
			this.tabHqBugControl.Location = new System.Drawing.Point(0, 49);
			this.tabHqBugControl.Name = "tabHqBugControl";
			this.tabHqBugControl.SelectedIndex = 0;
			this.tabHqBugControl.Size = new System.Drawing.Size(410, 285);
			this.tabHqBugControl.TabIndex = 214;
			// 
			// tabBugsInProgress
			// 
			this.tabBugsInProgress.Controls.Add(this.gridBugsInProgress);
			this.tabBugsInProgress.Location = new System.Drawing.Point(4, 22);
			this.tabBugsInProgress.Name = "tabBugsInProgress";
			this.tabBugsInProgress.Size = new System.Drawing.Size(402, 259);
			this.tabBugsInProgress.TabIndex = 2;
			this.tabBugsInProgress.Text = "In Progress";
			this.tabBugsInProgress.UseVisualStyleBackColor = true;
			// 
			// gridBugsInProgress
			// 
			this.gridBugsInProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridBugsInProgress.Location = new System.Drawing.Point(3, 6);
			this.gridBugsInProgress.Name = "gridBugsInProgress";
			this.gridBugsInProgress.Size = new System.Drawing.Size(395, 246);
			this.gridBugsInProgress.TabIndex = 1;
			this.gridBugsInProgress.Title = "";
			this.gridBugsInProgress.TitleVisible = false;
			this.gridBugsInProgress.TranslationName = "BugsGrid";
			this.gridBugsInProgress.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridBugs_CellDoubleClick);
			// 
			// tabBugsFixed
			// 
			this.tabBugsFixed.Controls.Add(this.gridBugsFixed);
			this.tabBugsFixed.Location = new System.Drawing.Point(4, 22);
			this.tabBugsFixed.Name = "tabBugsFixed";
			this.tabBugsFixed.Padding = new System.Windows.Forms.Padding(3);
			this.tabBugsFixed.Size = new System.Drawing.Size(402, 259);
			this.tabBugsFixed.TabIndex = 0;
			this.tabBugsFixed.Text = "Fixed";
			this.tabBugsFixed.UseVisualStyleBackColor = true;
			// 
			// gridBugsFixed
			// 
			this.gridBugsFixed.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridBugsFixed.Location = new System.Drawing.Point(3, 6);
			this.gridBugsFixed.Name = "gridBugsFixed";
			this.gridBugsFixed.Size = new System.Drawing.Size(395, 246);
			this.gridBugsFixed.TabIndex = 0;
			this.gridBugsFixed.Title = "";
			this.gridBugsFixed.TitleVisible = false;
			this.gridBugsFixed.TranslationName = "BugsGrid";
			this.gridBugsFixed.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridBugs_CellDoubleClick);
			// 
			// tabBugSubs
			// 
			this.tabBugSubs.Controls.Add(this.gridBugSubmissions);
			this.tabBugSubs.Location = new System.Drawing.Point(4, 22);
			this.tabBugSubs.Name = "tabBugSubs";
			this.tabBugSubs.Padding = new System.Windows.Forms.Padding(3);
			this.tabBugSubs.Size = new System.Drawing.Size(402, 259);
			this.tabBugSubs.TabIndex = 1;
			this.tabBugSubs.Text = "All";
			this.tabBugSubs.UseVisualStyleBackColor = true;
			// 
			// gridBugSubmissions
			// 
			this.gridBugSubmissions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridBugSubmissions.Location = new System.Drawing.Point(3, 6);
			this.gridBugSubmissions.Name = "gridBugSubmissions";
			this.gridBugSubmissions.Size = new System.Drawing.Size(395, 246);
			this.gridBugSubmissions.TabIndex = 1;
			this.gridBugSubmissions.Title = "";
			this.gridBugSubmissions.TitleVisible = false;
			this.gridBugSubmissions.TranslationName = "BugSubsGrid";
			this.gridBugSubmissions.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridBugSubmissions_CellDoubleClick);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(4, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(316, 17);
			this.label1.TabIndex = 218;
			this.label1.Text = "Unhandled Exceptions (Auto Reported)";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(331, 2);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 23);
			this.butRefresh.TabIndex = 217;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// datePickerHqBugSub
			// 
			this.datePickerHqBugSub.BackColor = System.Drawing.Color.Transparent;
			this.datePickerHqBugSub.EnableWeekButtons = false;
			this.datePickerHqBugSub.Location = new System.Drawing.Point(-3, 26);
			this.datePickerHqBugSub.MaximumSize = new System.Drawing.Size(0, 185);
			this.datePickerHqBugSub.MinimumSize = new System.Drawing.Size(453, 22);
			this.datePickerHqBugSub.Name = "datePickerHqBugSub";
			this.datePickerHqBugSub.Size = new System.Drawing.Size(453, 22);
			this.datePickerHqBugSub.TabIndex = 215;
			this.datePickerHqBugSub.CalendarClosed += new OpenDental.UI.CalendarClosedHandler(this.datePickerHqBugSub_CalendarClosed);
			// 
			// ODInternalCustomerGrids
			// 
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.datePickerHqBugSub);
			this.Controls.Add(this.tabHqBugControl);
			this.Name = "ODInternalCustomerGrids";
			this.Size = new System.Drawing.Size(410, 334);
			this.Load += new System.EventHandler(this.ODInternalCustomerGrids_Load);
			this.tabHqBugControl.ResumeLayout(false);
			this.tabBugsInProgress.ResumeLayout(false);
			this.tabBugsFixed.ResumeLayout(false);
			this.tabBugSubs.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private void ODInternalCustomerGrids_Load(object sender,EventArgs e) {
			datePickerHqBugSub.SetDateTimeFrom(DateTime.Today.AddDays(-90));
			datePickerHqBugSub.SetDateTimeTo(DateTime.Today);
			FillGrids();
			gridBugsFixed.ContextMenu=new ContextMenu(new [] { new MenuItem("View Tasks",viewTaskRightClickFixed)});
			gridBugsInProgress.ContextMenu=new ContextMenu(new[] { new MenuItem("View Tasks",viewTaskRightClickInProgress) });
		}

		private void viewTaskRightClickFixed(object sender, EventArgs e) {
			TryViewTask(gridBugsFixed);
		}

		private void viewTaskRightClickInProgress(object sender, EventArgs e) {
			TryViewTask(gridBugsInProgress);
		}

		private void TryViewTask(GridOD grid) { 
			int index=grid.GetSelectedIndex();
			if(index==-1) {
				return;
			}
			Bug selectedBug=(Bug)grid.ListGridRows[index].Tag;
			List<long> listTaskNums=JobLinks.GetTaskNumsForBug(selectedBug.BugId);
			switch(listTaskNums.Count) {
				case 0:
					MsgBox.Show(this,"There are not task assocaited to a job for this bug.");
					return;
				case 1:
					FormTaskEdit formTaskEdit=new FormTaskEdit(Tasks.GetOne(listTaskNums[0]));
					formTaskEdit.Show();
				break;
				default:
					using(FormTaskSearch formTaskSearch=new FormTaskSearch(listTaskNums)) {
						formTaskSearch.ShowDialog();
					}
				break;
			}
		}

		private void FillGrids(bool isRefreshNeeded=true) {
			if(_patCur==null || gridBugsFixed==null) {//Just in case.
				return;
			}
			gridBugsInProgress.BeginUpdate();
			gridBugsFixed.BeginUpdate();
			gridBugSubmissions.BeginUpdate();
			#region Init Columns
			if(gridBugsFixed.ListGridColumns.Count==0) {
				gridBugsInProgress.ListGridColumns.Add(new GridColumn("Description",80){ IsWidthDynamic=true });
				gridBugsInProgress.ListGridColumns.Add(new GridColumn("Expert",55,HorizontalAlignment.Center));
				gridBugsInProgress.ListGridColumns.Add(new GridColumn("Has Job",55,HorizontalAlignment.Center));
				gridBugsFixed.ListGridColumns.Add(new GridColumn("Description",80){ IsWidthDynamic=true });
				gridBugsFixed.ListGridColumns.Add(new GridColumn("Vers.",55,HorizontalAlignment.Center));
				gridBugsFixed.ListGridColumns.Add(new GridColumn("Has Job",55,HorizontalAlignment.Center));
				gridBugSubmissions.ListGridColumns.Add(new GridColumn("Description",100){ IsWidthDynamic=true });
				gridBugSubmissions.ListGridColumns.Add(new GridColumn("Vers.",55,HorizontalAlignment.Center));
				gridBugSubmissions.ListGridColumns.Add(new GridColumn("Has Job",55,HorizontalAlignment.Center));
			}
			#endregion
			if(isRefreshNeeded) {
				List<string> listKeys=RegistrationKeys.GetForPatient(_patCur.PatNum).Select(x => x.RegKey).ToList();
				try {
					_listBugSubs=BugSubmissions.GetBugSubsForRegKeys(listKeys,datePickerHqBugSub.GetDateTimeFrom(),datePickerHqBugSub.GetDateTimeTo());
				}
				catch(Exception ex) {//Should only occur if running in debug at HQ and no bugs database on local machine.
					ex.DoNothing();
					label1.Text="NO BUGS DB";//Inform dev of issue.
				}
				_listJobLinks=JobLinks.GetManyForType(JobLinkType.Bug,_listBugSubs.Select(x => x.BugId).ToList());
				List<long> listNewBugUserIds= _listBugSubs.Where(x => x.BugObj!=null && !_dictHqBugSubNames.ContainsKey(x.BugObj.Submitter))
					.Select(x => x.BugObj.Submitter).ToList();
				if(listNewBugUserIds.Count>0) {
					Bugs.GetDictSubmitterNames(listNewBugUserIds).ToList()
						.ForEach(x => _dictHqBugSubNames[x.Key]=x.Value);
				}
			}
			gridBugsInProgress.ListGridRows.Clear();
			gridBugsFixed.ListGridRows.Clear();
			gridBugSubmissions.ListGridRows.Clear();
			List<long> listBugNums=new List<long>();//Keep track of BubIds so that we do not show duplicates.
			foreach(BugSubmission sub in _listBugSubs) {
				bool hasJob=(_listJobLinks.Any(x => x.FKey==sub.BugObj?.BugId));
				string expertName=sub.BugObj==null ? "" : StringTools.ToUpperFirstOnly(_dictHqBugSubNames[sub.BugObj.Submitter]);
				if(sub.BugObj!=null && !listBugNums.Contains(sub.BugObj.BugId)) {//BugSubmission has an associated bug and it is not a duplicate.
					if(string.IsNullOrEmpty(sub.BugObj.VersionsFixed)) {//Bug is not fixed
						#region In Progress grid
						listBugNums.Add(sub.BugObj.BugId);
						GridRow rowBug=new GridRow(sub.BugObj.Description,
							expertName,
							(hasJob?"X":"")
						);
						rowBug.Tag=sub.BugObj;
						gridBugsInProgress.ListGridRows.Add(rowBug);
						#endregion
					}
					else {//Bug is fixed
						#region Fixed grid
						listBugNums.Add(sub.BugObj.BugId);
						GridRow rowBug=new GridRow(sub.BugObj.Description+"\r\nEXPERT: "+expertName,
							sub.BugObj.VersionsFixed.Replace(';','\n'),
							(hasJob?"X":"")
						);
						rowBug.Tag=sub.BugObj;
						gridBugsFixed.ListGridRows.Add(rowBug);
						#endregion
					}
				}
				#region All grid
				GridRow rowSub=new GridRow(sub.ExceptionMessageText+"\r\n"
						+sub.SubmissionDateTime.ToString("MM/dd/yyyy HH:mm:ss",CultureInfo.InvariantCulture)
						+"\r\nEXPERT: "+expertName,
					sub.TryGetPrefValue(PrefName.ProgramVersion,"0.0.0.0"),
					(hasJob?"X":"")
				);
				rowSub.Tag=sub;
				gridBugSubmissions.ListGridRows.Add(rowSub);
				#endregion
			}
			gridBugsInProgress.EndUpdate();
			gridBugsFixed.EndUpdate();
			gridBugSubmissions.EndUpdate();
		}
		
		private void gridBugs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(e.Row==-1) {
				return;
			}
			FormBugEdit formBE=new FormBugEdit();
			formBE.BugCur=(Bug)((GridOD)sender).ListGridRows[e.Row].Tag;
			formBE.Show();
		}
		
		private void gridBugSubmissions_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(e.Row==-1) {
				return;
			}
			FormBugSubmission formBS=new FormBugSubmission((BugSubmission)gridBugSubmissions.ListGridRows[e.Row].Tag);
			formBS.Show();
		}
		
		private void datePickerHqBugSub_CalendarClosed(object sender,EventArgs e) {
			FillGrids();
		}
		
		private void butRefresh_Click(object sender,EventArgs e) {
			TryHideGrids();
			FillGrids();
		}

		public bool TryHideGrids() {
			if(datePickerHqBugSub.IsCalendarExpanded) {
				datePickerHqBugSub.HideCalendars();
				return true;
			}
			return false;
		}
	}
}
