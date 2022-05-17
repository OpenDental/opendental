/*=============================================================================================================
Open Dental GPL license Copyright (C) 2003  Jordan Sparks, DMD.  http://www.open-dent.com,  www.docsparks.com
See header in FormOpenDental.cs for complete text.  Redistributions must retain this text.
===============================================================================================================
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{

	///<summary>This is replacing the DataClass as the preferred way to interact with the database.</summary>
	public class DataConnection{//
		///<summary>This data adapter is used for all queries to the database.</summary>
		private MySqlDataAdapter da;
		///<summary>This is the connection that is used by the data adapter for all queries.</summary>
		private MySqlConnection con;
		///<summary>Used to get very small bits of data from the db when the data adapter would be overkill.  For instance retrieving the response after a command is sent.</summary>
		private MySqlDataReader dr;
		///<summary>Stores the string of the command that will be sent to the database.</summary>
		private MySqlCommand cmd;
		///<summary>After inserting a row, this variable will contain the primary key for the newly inserted row.  This can frequently save an additional query to the database.</summary>
		public int InsertID;

		///<summary>Constructor sets the connection values.</summary>
		public DataConnection(){
		  con=new MySqlConnection(FormChooseDatabase.GetConnectionString());
			//dr = null;
			cmd = new MySqlCommand();
			cmd.Connection=con;
			//table=new DataTable();
		}

		///<summary>Sets the connection to an alternate database for backup purposes.  Currently only used during conversions to do a quick backup first, and in FormConfig to get db names.</summary>
		public DataConnection(string db){
		  con= new MySqlConnection(FormChooseDatabase.GetAlternateConnStr(db));
			//dr = null;
			cmd = new MySqlCommand();
			cmd.Connection = con;
			//table=new DataTable(null);
		}

		///<summary>Tests to see if the connection with specified database is valid and accepting commands.</summary>
		public bool IsValid(){
			try{
				con.Open();
				cmd.CommandText="update preference set valuestring = '0' where valuestring = '0'";
				cmd.ExecuteNonQuery();
				con.Close();
			}
			catch{//(MySQLDriverCS.MySQLException ex){
				return false; 
			}
			return true;
		}

		public bool DbExists(){
			try {
				con.Open();
				con.Close();
			}
			catch {
				return false;
			}
			return true;
		}

		///<summary>Fills table with data from the database.</summary>
		public DataTable GetTable(string command){
			cmd.CommandText=command;
			DataTable table=new DataTable();
 			try{
 				da=new MySqlDataAdapter(cmd);
 				da.Fill(table);
 			}
			catch(MySql.Data.Types.MySqlConversionException){
				//MessageBox.Show(ex.Message);
				MsgBox.Show(this,"Invalid date found. Please fix dates by running the Database Maintenance tool.  Include the initial check.");
			}
			catch(MySqlException e){
				MsgBoxCopyPaste MB=new MsgBoxCopyPaste(Lan.g("DataConnection","Error in query:")+"\r\n"
					+e.Message+"\r\n"+"\r\n"
					+command);
				MB.ShowDialog();
				//MessageBox.Show(command);
			}
			//System.Net.Sockets.SocketException will be thrown if database connection is unavailable.
			//SocketException should be handled by the calling function.
			//usually only a problem for timed refreshes.
			//catch(MySqlException e){
			//	MessageBox.Show("Error: "+e.Message+","+cmd.CommandText);
			//}
			finally{
				con.Close();
			}
 			return table;
		}

		///<summary>Sends a non query command to the database and returns the number of rows affected. If true, then InsertID will be set to the value of the primary key of the newly inserted row.</summary>
		public int NonQ(string command){
			return NonQ(command,false);
		}

		///<summary>Sends a non query command to the database and returns the number of rows affected. If true, then InsertID will be set to the value of the primary key of the newly inserted row.</summary>
		public int NonQ(string command,bool getInsertID){
 			cmd.CommandText=command;
			int rowsChanged=0;
 			try{
				con.Open();
 				rowsChanged=cmd.ExecuteNonQuery();
 				if(getInsertID){
					cmd.CommandText="SELECT LAST_INSERT_ID()";
					dr=(MySqlDataReader)cmd.ExecuteReader();
					if(dr.Read())
						InsertID=PIn.PInt(dr[0].ToString());
				}
			}
			catch(MySqlException e){
				MsgBoxCopyPaste MB=new MsgBoxCopyPaste(Lan.g("DataConnection","Error in query:")+"\r\n"
					+e.Message+"\r\n"+"\r\n"
					+command);
				MB.ShowDialog();
				//MessageBox.Show("Error: "+e.Message+","+cmd.CommandText);
			}
			//catch{
			//	MessageBox.Show("Error: "+);
			//}
			finally{
				con.Close();
			}
 			return rowsChanged;
 		}

		///<summary>Submits an array of commands in sequence. Used in conversions. Throws an exception if unsuccessful.  Returns the number of rows affected</summary>
		public int NonQ(string[] commands){
			int rowsChanged=0;
			con.Open();
			for(int i=0;i<commands.Length;i++){
				cmd.CommandText=commands[i];
				//Debug.WriteLine(cmd.CommandText);
				rowsChanged+=cmd.ExecuteNonQuery();
			}
			con.Close();
			return rowsChanged;
		}

		///<summary>Although this might be slightly faster, its use is being dropped because data will always be returned in datasets.  Use this for count(*) queries.  They are always guaranteed to return one and only one value.  Use(d) datareader instead of datatable, so faster.  Can also be used when retrieving prefs manually, since they will also return exactly one value</summary>
		public string GetCount(string command){
			cmd.CommandText=command;
			con.Open();
			dr=(MySqlDataReader)cmd.ExecuteReader();
			dr.Read();
			string retVal=dr[0].ToString();
			con.Close();
			return retVal;
		}



	}



}









*/