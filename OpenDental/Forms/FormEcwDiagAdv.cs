using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using MySqlConnector;
using System.IO;
using System.Security.Cryptography;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEcwDiagAdv:FormODBase {
		private string _connectionString;
		private string _username="ecwUser";
		private string _password="l69Rr4Rmj4CjiCTLxrIblg==";//encrypted
		private string _server;
		private string _port;
		private string _command;
		private string _dummyConnString;
		DataTable _tableQueryResults=new DataTable();
		DataTable _table=new DataTable();
		private StringBuilder _stringBuilder=new StringBuilder();

		public FormEcwDiagAdv() {
			InitializeComponent();
			InitializeLayoutManager();
			_server=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"eCWServer");//this property will not exist if using Oracle, eCW will never use Oracle
			_port=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"eCWPort");//this property will not exist if using Oracle, eCW will never use Oracle
			buildConnectionString();
			Lan.F(this);
		}

		private void FormEcwDiagAdv_Load(object sender,EventArgs e) {
			fillQueryList();
			_server=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"eCWServer");//this property will not exist if using Oracle, eCW will never use Oracle
			_port=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"eCWPort");//this property will not exist if using Oracle, eCW will never use Oracle
			buildConnectionString();
			_dummyConnString=
				"Server="+_server+";"
				+"Port="+_port+";"//although this does seem to cause a bug in Mono.  We will revisit this bug if needed to exclude the port option only for Mono.
				+"Database=;"//ecwMaster;"
				//+"Connect Timeout=20;"
				+"User ID="+_username+";"
				+"Password=;"//no password information.
				+"SslMode=none;"
				+"CharSet=utf8;"
				+"Treat Tiny As Boolean=false;"
				+"Allow User Variables=true;"
				+"Default Command Timeout=300;"//default is 30seconds
				+"Pooling=false"
				;
			textConnString.Text=
				"Server="+_server+";"
				+"Port="+_port+";"//although this does seem to cause a bug in Mono.  We will revisit this bug if needed to exclude the port option only for Mono.
				+"Database=;"//ecwMaster;"
				//+"Connect Timeout=20;"
				+"User ID="+_username+";"
				+"Password=;"//no password information
				+"SslMode=none;"
				+"CharSet=utf8;"
				+"Treat Tiny As Boolean=false;"
				+"Allow User Variables=true;"
				+"Default Command Timeout=300;"//default is 30seconds
				+"Pooling=false"
				; ;
			//textQuery.Text="SHOW VARIABLES;";
			//Show some relevent variables
			textQuery.Text="SHOW VARIABLES "
										+"WHERE Variable_name IN "
										+"('basedir',"
										+" 'connect_timout',"
										+" 'datadir',"
										+" 'default_storage_engine',"
										+" 'general_log',"
										+" 'general_log_file',"
										+" 'hostname',"
										+" 'log_error',"
										+" 'pid_file',"
										+" 'port',"
										+" 'storage_engine',"
										+" 'tmpdir',"
										+" 'version',"
										+" 'version_compile_machine',"
										+" 'version_compile_os'"
										+");";
			RunQuery();
			FillTables();
		}

		private void fillQueryList() {
			listQuery.Items.Add("SELECT * FROM itemkeys WHERE NAME IN ('pwd', 'user', 'FtpPath', 'Administrator', 'ClientVersion', 'upgrade_sqlver', 'ReconcilePatientFlag', 'ReconciliationPath', 'InterfaceID', 'GenericResultsPath', 'IsSIUOutboundConfigured', 'IsSIUOutboundVirtualTelConfigured', 'IsADTOutboundConfigured', 'IsADTWithOutHL7Interface', 'DentalEMRAppPath', 'EnableDentalEMR', 'isSaasPractice');");
			listQuery.Items.Add("SELECT * FROM mobiledoc.pmitemkeys WHERE name LIKE '%_Filter_for_%';/*look at 'value' column. Look for values other than 'no'*/");
			listQuery.Items.Add("SELECT * FROM mobiledoc.pmitemkeys WHERE value LIKE '%HL7%';/*used to show ecw paths to HL7 folders*/");
			listQuery.Items.Add("SELECT * FROM mobiledoc.hl7segment_details;/*Look for AIG or PV1 segment in the SIU messages.*/");
			listQuery.Items.Add("SELECT * FROM mobiledoc.hl7segment_groups;/*Look at group definitions.*/");
			listQuery.Items.Add("SELECT * FROM mobiledoc.itemkeys WHERE NAME IN ('pwd', 'user', 'FtpPath', 'Administrator', 'ClientVersion', 'upgrade_sqlver', 'ReconcilePatientFlag', 'ReconciliationPath', 'InterfaceID', 'GenericResultsPath', 'IsSIUOutboundConfigured', 'IsSIUOutboundVirtualTelConfigured', 'IsADTOutboundConfigured', 'IsADTWithOutHL7Interface', 'DentalEMRAppPath', 'EnableDentalEMR', 'isSaasPractice');/*General settings that might be useful.*/");
			listQuery.Items.Add("SELECT * FROM pmcodes;");
			listQuery.Items.Add("SELECT * FROM visitcodes ORDER by dentalvisit DESC;");
			listQuery.Items.Add("SELECT * FROM visitcodes LEFT OUTER JOIN pmcodes ON visitcodes.Name=pmcodes.ecwcode WHERE dentalvisit=1;");
			//listQuery.Items.Add("");
		}

		private void FillTables() {
			try {
				_table=DataConnectionBase.MySqlHelper.ExecuteDataset(_connectionString,"SHOW TABLES").Tables[0];
			}
			catch(Exception ex) {
				MsgBox.Show(this,ex.Message);
			}
			gridTables.BeginUpdate();
			gridTables.Columns.Clear();
			GridColumn col;
			for(int c=0;c<_table.Columns.Count;c++){
				List<int> listWidthCols=new List<int>();
				listWidthCols.Add(_table.Columns[c].ColumnName.Length);
				for(int r=0;r<_table.Rows.Count;r++){
					listWidthCols.Add(_table.Rows[r][_table.Columns[c].ColumnName].ToString().Length);
				}
				listWidthCols.Sort();
				//int colWidth=Math.Max(colCur.ColumnName.Length,queryResult.Rows[0][colCur.ColumnName].ToString().Length)*8;//8px per character based on the length of either column contents or col name.
				col=new GridColumn(_table.Columns[c].ColumnName,(int)(listWidthCols[listWidthCols.Count-1]*5.5));//longest string * 5.8px
				gridTables.Columns.Add(col);
			}
			gridTables.ListGridRows.Clear();
			GridRow row;
			int countCol=_table.Columns.Count;//to speed things up and not have to re-reference queryResults.Columns.Count for every row found.
			for(int r=0;r<_table.Rows.Count;r++){
				row=new GridRow();
				for(int c=0;c<countCol;c++) {
					row.Cells.Add(_table.Rows[r][c].ToString());
				}
				gridTables.ListGridRows.Add(row);
			}
			gridTables.EndUpdate();
		}

		///<summary>Used to construct a default construction string.</summary>
		private void buildConnectionString() {
			if(textConnString.Text!=_dummyConnString && textConnString.Text!="") {//use the user provided connection string
				_connectionString=textConnString.Text;
				return;
			}
			_connectionString=
				"Server="+_server+";"
				+"Port="+_port+";"//although this does seem to cause a bug in Mono.  We will revisit this bug if needed to exclude the port option only for Mono.
				+"Database=mobiledoc;"//ecwMaster;"
				//+"Connect Timeout=20;"
				+"User ID="+_username+";"
				+"Password="+Decrypt(_password)+";"
				+"CharSet=utf8;"
				+"Treat Tiny As Boolean=false;"
				+"Allow User Variables=true;"
				+"Default Command Timeout=300;"//default is 30seconds
				+"Pooling=false"
				;
		}

		private string Decrypt(string encString) {
			UTF8Encoding utf8Encoding=new UTF8Encoding();
			try {
				byte[] byteArray=Convert.FromBase64String(encString);
				Aes aes=new AesCryptoServiceProvider();
				aes.Key=utf8Encoding.GetBytes("AKQjlLUjlcABVbqp");//Random string will be key
				aes.IV=new byte[16];
				ICryptoTransform iCryptoTransform=aes.CreateDecryptor(aes.Key,aes.IV);
				using MemoryStream memoryStream=new MemoryStream(byteArray);
				using CryptoStream cryptoStream=new CryptoStream(memoryStream,iCryptoTransform,CryptoStreamMode.Read);
				using StreamReader streamReader=new StreamReader(cryptoStream);
				string decrypted=streamReader.ReadToEnd();
				if(aes!=null) {
					aes.Clear();
				}
				return decrypted;
			}
			catch {
				MessageBox.Show("Text entered was not valid encrypted text.");
				return "";
			}
		}

		private void FillGrid() {
			if(_tableQueryResults==null) {
				return;
			}
			//DataTable Table=queryResult;
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			for(int i=0;i<_tableQueryResults.Columns.Count;i++){
				List<int> listWidthCols=new List<int>();
				listWidthCols.Add(5);//min width of 27px=(int)(5*5.5)
				listWidthCols.Add(_tableQueryResults.Columns[i].ColumnName.Length+5);
				for(int j=0;j>_tableQueryResults.Rows.Count;j++) {
					listWidthCols.Add(_tableQueryResults.Rows[j][_tableQueryResults.Columns[i].ColumnName].ToString().Length+1);
				}
				listWidthCols.Sort();
				//int colWidth=Math.Max(colCur.ColumnName.Length,queryResult.Rows[0][colCur.ColumnName].ToString().Length)*8;//8px per character based on the length of either column contents or col name.
				col=new GridColumn(_tableQueryResults.Columns[i].ColumnName,(int)(listWidthCols[listWidthCols.Count-1]*5.8));//longest string * 5.8px
				gridMain.Columns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			int countCol=_tableQueryResults.Columns.Count;//to speed things up and not have to re-reference queryResults.Columns.Count for every row found.
			for(int i=0;i<_tableQueryResults.Rows.Count;i++) {
				row=new GridRow();
				for(int c=0;c<countCol;c++) {
					row.Cells.Add(_tableQueryResults.Rows[i][c].ToString());
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			FillTables();
		}

		private void gridTables_KeyPress(object sender,KeyPressEventArgs e) {
			char c=e.KeyChar;
			for(int i=0;i<gridTables.ListGridRows.Count;i++) {
				if(gridTables.ListGridRows[i].Cells[0].Text[0]==c) {
					gridTables.ScrollToEnd();
					float heightRow=(float)gridTables.ScrollValue/(float)gridTables.ListGridRows.Count;
					gridTables.ScrollValue=i*(int)Math.Round(heightRow,0,MidpointRounding.AwayFromZero);
					return;
				}
			}
		}

		///<summary>Double clicking on a table name will run "select * from" that table.</summary>
		private void gridTables_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Cursor=Cursors.WaitCursor;
			Application.DoEvents();
			_command="SELECT * FROM "+gridTables.ListGridRows[e.Row].Cells[0].Text+" ;";
			try{
				_tableQueryResults=DataConnectionBase.MySqlHelper.ExecuteDataset(_connectionString,_command).Tables[0];
			}
			catch(Exception ex) {
				MsgBox.Show(this,ex.Message);
			}
			FillGrid();
			Cursor=Cursors.Default;
			Application.DoEvents();
		}

		private void butRunQ_Click(object sender,EventArgs e) {
			RunQuery();
		}

		private void RunQuery() {
			Cursor=Cursors.WaitCursor;
			Application.DoEvents();
			buildConnectionString();
			_command=textQuery.Text;
			try {
				_tableQueryResults=DataConnectionBase.MySqlHelper.ExecuteDataset(_connectionString,_command).Tables[0];
			}
			catch(Exception ex) {
				MsgBox.Show(this,ex.Message);
			}
			FillGrid();
			Cursor=Cursors.Default;
			Application.DoEvents();
		}

		private void textQuery_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.F9) {
				RunQuery();
			}
		}

		private void listQuery_SelectedIndexChanged(object sender,EventArgs e) {
			textQuery.Text=listQuery.SelectedItem.ToString();
			RunQuery();
		}

	}
}