using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using DataConnectionBase;
using OpenDental;
using OpenDental.UI;
using OpenDentBusiness;

namespace CentralManager {
	public partial class FormCentralPatientSearch:Form {
		#region Fields - Public
		public bool AutoLogIn;
		///<summary>List of connections with a status of OK.</summary>
		public List<CentralConnection> ListConnsOK;
		//<summary>Selection mode returns a DataRow of the patient selected. The tag in the selected DataRow is a ODGridRowTag.</summary>
		//public bool IsSelectionMode;
		public DataRow DataRowSelectedPatient;
		///<summary>Indicates when launching a database whether to use dynamic mode or not.</summary>
		public bool UseDynamicMode;
		#endregion Fields - Public

		#region Fields -Private
		/// <summary>Dataset containing tables of patients for each connection.</summary>
		private DataSet _dataSetPats;
		private object _lockObj=new object();
		private int _complConnCount;
		private string _invalidConnsLog;
		private bool _hasWarningShown=false;
		#endregion Fields - Private

		public FormCentralPatientSearch() {
			//ListConnsOK=new List<CentralConnection>();
			InitializeComponent();
		}

		private void FormCentralPatientSearch_Load(object sender,System.EventArgs e) {
			DisplayFields.RefreshCache();
			_complConnCount=0;
			_dataSetPats=new DataSet();
			_invalidConnsLog="";
			//if(IsSelectionMode) {//always selection mode
			gridPats.Title="Double Click to Select Patient";
			StartThreadsForConns();
		}

		///<summary>Loops through all connections passed in and spawns a thread for each to go fetch patient data from each db using the given filters.</summary>
		private void StartThreadsForConns() {
			_dataSetPats.Tables.Clear();
			bool hasConnsSkipped=false;
			for(int i=0;i<ListConnsOK.Count;i++) {
				//Filter the threads by their connection name
				string connName="";
				if(ListConnsOK[i].DatabaseName=="") {//uri
					connName=ListConnsOK[i].ServiceURI;
				}
				else {
					connName=ListConnsOK[i].ServerName+", "+ListConnsOK[i].DatabaseName;
				}
				if(!connName.Contains(textConn.Text)) {
					//Do NOT spawn a thread to go fetch data for this connection because the user has filtered it out.
					//Increment the completed thread count and continue.
					hasConnsSkipped=true;
					lock(_lockObj) {
						_complConnCount++;
					}
					continue;
				}
				//At this point we know the connection has not been filtered out, so fire up a thread to go get the patient data table for the search.
				ODThread odThread=new ODThread(GetDataTablePatForConn,new object[]{ListConnsOK[i]});
				odThread.GroupName="FetchPats";
				odThread.Start();
			}
			if(hasConnsSkipped) {
				//There is a chance that some threads finished (by failing, etc) before the end of the loop where we spawned the threads
				//so we want to guarantee that the failure message shows if any connection was skipped.
				//This is required because FillGrid contains code that only shows a warning message when all connections have finished.
				FillGridPats();
			}
		}

		private void GetDataTablePatForConn(ODThread odThread) {
			CentralConnection connection=(CentralConnection)odThread.Parameters[0];
			//Filter the threads by their connection name
			string connName="";
			if(connection.DatabaseName=="") {//uri
				connName=connection.ServiceURI;
			}
			else {
				connName=connection.ServerName+", "+connection.DatabaseName;
			}
			if(!CentralConnectionHelper.SetCentralConnection(connection,false)) {
				lock(_lockObj) {
					_invalidConnsLog+="\r\n"+connName;
					_complConnCount++;
				}
				connection.ConnectionStatus="OFFLINE";
				BeginInvoke((Action)FillGridPats);
				return;
			}
			List<DisplayField> fields=DisplayFields.GetForCategory(DisplayFieldCategory.CEMTSearchPatients);
			bool hasNextLastVisit=fields.Any(x => ListTools.In(x.InternalName,"NextVisit","LastVisit"));
			DataTable table=new DataTable();
			try {
				PtTableSearchParams ptTableSearchParams=new PtTableSearchParams(checkLimit.Checked,textLName.Text,textFName.Text,textPhone.Text,
					textAddress.Text,checkHideInactive.Checked,textCity.Text,textState.Text,textSSN.Text,textPatNum.Text,textChartNumber.Text,0,
					checkGuarantors.Checked,!checkHideArchived.Checked,//checkHideArchived is opposite label for what this function expects, but hideArchived makes more sense
					SIn.DateT(textBirthdate.Text),0,textSubscriberID.Text,textEmail.Text,textCountry.Text,"","","","",hasNextLastVisit:hasNextLastVisit);
				table=Patients.GetPtDataTable(ptTableSearchParams);
			}
			catch(ThreadAbortException tae) {
				throw tae;//ODThread needs to clean up after an abort exception is thrown.
			}
			catch(Exception) {
				//This can happen if the connection to the server was severed somehow during the execution of the query.
				lock(_lockObj) {
					_invalidConnsLog+="\r\n"+connName+"  -GetPtDataTable";
					_complConnCount++;
				}
				BeginInvoke((Action)FillGridPats);//Pops up a message box if this was the last thread to finish.
				return;
			}
			table.TableName=connName;
			odThread.Tag=table;
			lock(_lockObj) {
				_complConnCount++;
				_dataSetPats.Tables.Add((DataTable)odThread.Tag);
			}
			BeginInvoke((Action)FillGridPats);
		}

		private void FillGridPats() {
			Cursor=Cursors.WaitCursor;
			gridPats.BeginUpdate();
			List<DisplayField> fields=DisplayFields.GetForCategory(DisplayFieldCategory.CEMTSearchPatients);
			if(gridPats.ListGridColumns.Count==0) {//Init only once.
				foreach(DisplayField field in fields) {
					string heading=field.InternalName;
					if(!string.IsNullOrEmpty(field.Description)) {
						heading=field.Description;
					}
					gridPats.ListGridColumns.Add(new GridColumn(heading,field.ColumnWidth));
				}
			}
			gridPats.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_dataSetPats.Tables.Count;i++) {
				for(int j=0;j<_dataSetPats.Tables[i].Rows.Count;j++) {
					row=new GridRow();
					foreach(DisplayField field in fields) {
						switch(field.InternalName) {
							#region Row Cell Filling
							case "Conn":
								row.Cells.Add(_dataSetPats.Tables[i].TableName);
								break;
							case "PatNum":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["PatNum"].ToString());
								break;
							case "LName":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["LName"].ToString());
								break;
							case "FName":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["FName"].ToString());
								break;
							case "SSN":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["SSN"].ToString());
								break;
							case "PatStatus":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["PatStatus"].ToString());
								break;
							case "Age":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["age"].ToString());
								break;
							case "City":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["City"].ToString());
								break;
							case "State":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["State"].ToString());
								break;
							case "Address":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["Address"].ToString());
								break;
							case "Wk Phone":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["WkPhone"].ToString());
								break;
							case "Email":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["Email"].ToString());
								break;
							case "ChartNum":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["ChartNumber"].ToString());
								break;
							case "MI":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["MiddleI"].ToString());
								break;
							case "Pref Name":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["Preferred"].ToString());
								break;
							case "Hm Phone":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["HmPhone"].ToString());
								break;
							case "Bill Type":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["BillingType"].ToString());
								break;
							case "Pri Prov":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["PriProv"].ToString());
								break;
							case "Birthdate":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["Birthdate"].ToString());
								break;
							case "Site":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["site"].ToString());
								break;
							case "Clinic":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["clinic"].ToString());
								break;
							case "Wireless Ph":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["WirelessPhone"].ToString());
								break;
							case "Sec Prov":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["SecProv"].ToString());
								break;
							case "LastVisit":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["lastVisit"].ToString());
								break;
							case "NextVisit":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["nextVisit"].ToString());
								break;
							case "Country":
								row.Cells.Add(_dataSetPats.Tables[i].Rows[j]["Country"].ToString());
								break;
							#endregion
						}
					}
					//connection is no longer needed in this window
					//CentralConnection conn=ListConnsOK.Find(x => ((x.ServerName+", "+x.DatabaseName)==_dataSetPats.Tables[i].TableName || x.ServiceURI==_dataSetPats.Tables[i].TableName));
					//row.Tag=new ConnAndDataRow(conn,_dataSetPats.Tables[i].Rows[j]);
					row.Tag=_dataSetPats.Tables[i].Rows[j];
					gridPats.ListGridRows.Add(row);
				}
			}
			gridPats.EndUpdate();
			Cursor=Cursors.Default;
			if(_complConnCount==ListConnsOK.Count) {
				ODThread.QuitSyncThreadsByGroupName(1,"FetchPats");//Clean up finished threads.
				butSearch.Text=Lans.g(this,"Refresh");
				labelFetch.Visible=false;
				if(!_hasWarningShown && _invalidConnsLog!="") {
					_hasWarningShown=true;//Keeps the message box from showing up for subsequent threads.
					OpenDental.MessageBox.Show(this,Lan.g(this,"Could not connect to the following servers")+":"+_invalidConnsLog);
				}
			}
			else {
				butSearch.Text=Lans.g(this,"Stop Refresh");
				labelFetch.Visible=true;
			}
		}

		private void gridPats_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//ConnAndDataRow gridRowSelectedTag=(ConnAndDataRow)gridPats.ListGridRows[e.Row].Tag;
			//if(IsSelectionMode) {//always selection mode
			//DataRowSelectedPatient=gridRowSelectedTag.DataRow;
			DataRowSelectedPatient=(DataRow)gridPats.ListGridRows[e.Row].Tag;
			DialogResult=DialogResult.OK;
		}

		private void butSearchPats_Click(object sender,EventArgs e) {
			ODThread.JoinThreadsByGroupName(1,"FetchPats");//Stop fetching immediately
			_hasWarningShown=false;
			lock(_lockObj) {
				_invalidConnsLog="";
			}
			if(butSearch.Text==Lans.g(this,"Refresh")) {
				_dataSetPats.Clear();
				butSearch.Text=Lans.g(this,"Stop Refresh");
				labelFetch.Visible=true;
				_complConnCount=0;
				StartThreadsForConns();
			}
			else {
				butSearch.Text=Lans.g(this,"Refresh");
				labelFetch.Visible=false;
				_complConnCount=ListConnsOK.Count;
			}
		}

		private void buttonOK_Click(object sender, EventArgs e){
			if(gridPats.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select a patient, first.");
				return;
			}
			DataRowSelectedPatient=(DataRow)gridPats.ListGridRows[gridPats.GetSelectedIndex()].Tag;
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormCentralPatientSearch_FormClosing(object sender,FormClosingEventArgs e) {
			//User could have closed the window before all threads finished.  Make sure to abort all threads instantly.
			ODThread.QuitSyncThreadsByGroupName(1,"FetchPats");
		}

	
	}

	/*
	///<summary>Holds a CentralConnection and the DataRow. Used when form is in selected mode.</summary>
	public class ConnAndDataRow {

		public CentralConnection CentralConnection;
		public DataRow DataRow;

		public ConnAndDataRow(CentralConnection centralConn,DataRow row) {
			CentralConnection=centralConn;
			DataRow=row;
		}

	}*/
}
