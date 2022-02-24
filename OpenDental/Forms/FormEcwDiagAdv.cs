using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Security.Cryptography;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEcwDiagAdv:FormODBase {
		private string connString;
		private string username="ecwUser";
		private string password="l69Rr4Rmj4CjiCTLxrIblg==";//encrypted
		private string server;
		private string port;
		private string command;
		private string dummyConnString;
		DataTable queryResult=new DataTable();
		DataTable dbTables=new DataTable();
		private StringBuilder arbitraryStringName=new StringBuilder();

		public FormEcwDiagAdv() {
			InitializeComponent();
			InitializeLayoutManager();
			server=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"eCWServer");//this property will not exist if using Oracle, eCW will never use Oracle
			port=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"eCWPort");//this property will not exist if using Oracle, eCW will never use Oracle
			buildConnectionString();
			Lan.F(this);
		}

		private void FormEcwDiagAdv_Load(object sender,EventArgs e) {
			fillQueryList();
			server=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"eCWServer");//this property will not exist if using Oracle, eCW will never use Oracle
			port=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"eCWPort");//this property will not exist if using Oracle, eCW will never use Oracle
			buildConnectionString();
			dummyConnString=
				"Server="+server+";"
				+"Port="+port+";"//although this does seem to cause a bug in Mono.  We will revisit this bug if needed to exclude the port option only for Mono.
				+"Database=;"//ecwMaster;"
				//+"Connect Timeout=20;"
				+"User ID="+username+";"
				+"Password=;"//no password information.
				+"SslMode=none;"
				+"CharSet=utf8;"
				+"Treat Tiny As Boolean=false;"
				+"Allow User Variables=true;"
				+"Default Command Timeout=300;"//default is 30seconds
				+"Pooling=false"
				;
			textConnString.Text=
				"Server="+server+";"
				+"Port="+port+";"//although this does seem to cause a bug in Mono.  We will revisit this bug if needed to exclude the port option only for Mono.
				+"Database=;"//ecwMaster;"
				//+"Connect Timeout=20;"
				+"User ID="+username+";"
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
				dbTables=MySqlHelper.ExecuteDataset(connString,"SHOW TABLES").Tables[0];
			}
			catch(Exception ex) {
				MsgBox.Show(this,ex.Message);
			}
			gridTables.BeginUpdate();
			gridTables.ListGridColumns.Clear();
			GridColumn col;
			foreach(DataColumn colCur in dbTables.Columns) {
				List<int> colWidths=new List<int>();
				colWidths.Add(colCur.ColumnName.Length);
				foreach(DataRow dr in dbTables.Rows) {
					colWidths.Add(dr[colCur.ColumnName].ToString().Length);
				}
				colWidths.Sort();
				//int colWidth=Math.Max(colCur.ColumnName.Length,queryResult.Rows[0][colCur.ColumnName].ToString().Length)*8;//8px per character based on the length of either column contents or col name.
				col=new GridColumn(colCur.ColumnName,(int)(colWidths[colWidths.Count-1]*5.5));//longest string * 5.8px
				gridTables.ListGridColumns.Add(col);
			}
			gridTables.ListGridRows.Clear();
			GridRow row;
			int colCount=dbTables.Columns.Count;//to speed things up and not have to re-reference queryResults.Columns.Count for every row found.
			foreach(DataRow rowCur in dbTables.Rows) {
				row=new GridRow();
				for(int c=0;c<colCount;c++) {
					row.Cells.Add(rowCur[c].ToString());
				}
				gridTables.ListGridRows.Add(row);
			}
			gridTables.EndUpdate();
		}

		///<summary>Used to construct a default construction string.</summary>
		private void buildConnectionString() {
			if(textConnString.Text!=dummyConnString && textConnString.Text!="") {//use the user provided connection string
				connString=textConnString.Text;
				return;
			}
			connString=
				"Server="+server+";"
				+"Port="+port+";"//although this does seem to cause a bug in Mono.  We will revisit this bug if needed to exclude the port option only for Mono.
				+"Database=mobiledoc;"//ecwMaster;"
				//+"Connect Timeout=20;"
				+"User ID="+username+";"
				+"Password="+Decrypt(password)+";"
				+"CharSet=utf8;"
				+"Treat Tiny As Boolean=false;"
				+"Allow User Variables=true;"
				+"Default Command Timeout=300;"//default is 30seconds
				+"Pooling=false"
				;
		}

		private string Decrypt(string encString) {
			try {
				byte[] encrypted=Convert.FromBase64String(encString);
				MemoryStream ms=null;
				CryptoStream cs=null;
				StreamReader sr=null;
				Aes aes=new AesCryptoServiceProvider();
				UTF8Encoding enc=new UTF8Encoding();
				aes.Key=enc.GetBytes("AKQjlLUjlcABVbqp");//Random string will be key
				aes.IV=new byte[16];
				ICryptoTransform decryptor=aes.CreateDecryptor(aes.Key,aes.IV);
				ms=new MemoryStream(encrypted);
				cs=new CryptoStream(ms,decryptor,CryptoStreamMode.Read);
				sr=new StreamReader(cs);
				string decrypted=sr.ReadToEnd();
				ms.Dispose();
				cs.Dispose();
				sr.Dispose();
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
			if(queryResult==null) {
				return;
			}
			//DataTable Table=queryResult;
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			foreach(DataColumn colCur in queryResult.Columns) {
				List<int> colWidths=new List<int>();
				colWidths.Add(5);//min width of 27px=(int)(5*5.5)
				colWidths.Add(colCur.ColumnName.Length+5);
				foreach(DataRow dr in queryResult.Rows) {
					colWidths.Add(dr[colCur.ColumnName].ToString().Length+1);
				}
				colWidths.Sort();
				//int colWidth=Math.Max(colCur.ColumnName.Length,queryResult.Rows[0][colCur.ColumnName].ToString().Length)*8;//8px per character based on the length of either column contents or col name.
				col=new GridColumn(colCur.ColumnName,(int)(colWidths[colWidths.Count-1]*5.8));//longest string * 5.8px
				gridMain.ListGridColumns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			int colCount=queryResult.Columns.Count;//to speed things up and not have to re-reference queryResults.Columns.Count for every row found.
			foreach(DataRow rowCur in queryResult.Rows) {
				row=new GridRow();
				for(int c=0;c<colCount;c++) {
					row.Cells.Add(rowCur[c].ToString());
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			FillTables();
		}

		private void gridTables_KeyPress(object sender,KeyPressEventArgs e) {
			char pressed=e.KeyChar;
			for(int i=0;i<gridTables.ListGridRows.Count;i++) {
				if(gridTables.ListGridRows[i].Cells[0].Text[0]==pressed) {
					gridTables.ScrollToEnd();
					float rowHeight=(float)gridTables.ScrollValue/(float)gridTables.ListGridRows.Count;
					gridTables.ScrollValue=i*(int)Math.Round(rowHeight,0,MidpointRounding.AwayFromZero);
					return;
				}
			}
		}

		///<summary>Double clicking on a table name will run "select * from" that table.</summary>
		private void gridTables_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Cursor=Cursors.WaitCursor;
			Application.DoEvents();
			try{
				command="SELECT * FROM "+gridTables.ListGridRows[e.Row].Cells[0].Text+" ;";
				queryResult=MySqlHelper.ExecuteDataset(connString,command).Tables[0];
				FillGrid();
			}
			catch(Exception ex) {
				MsgBox.Show(this,ex.Message);
			}
			Cursor=Cursors.Default;
			Application.DoEvents();

		}

		private void butRunQ_Click(object sender,EventArgs e) {
			RunQuery();
		}

		private void RunQuery() {
			Cursor=Cursors.WaitCursor;
			Application.DoEvents();
			try {
				buildConnectionString();
				command=textQuery.Text;
				queryResult=MySqlHelper.ExecuteDataset(connString,command).Tables[0];
				FillGrid();
			}
			catch(Exception ex) {
				MsgBox.Show(this,ex.Message);
			}
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

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}