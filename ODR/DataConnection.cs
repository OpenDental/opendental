/*=============================================================================================================
Open Dental GPL license Copyright (C) 2003  Jordan Sparks, DMD.  http://www.open-dent.com,  www.docsparks.com
See header in FormOpenDental.cs for complete text.  Redistributions must retain this text.
===============================================================================================================*/
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using OpenDentBusiness;
using CodeBase;

namespace ODR{

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
		public long InsertID;

		///<summary>Constructor sets the connection values.</summary>
		public DataConnection(){
		  con=new MySqlConnection(GetOpenDentalConnStr());
			//dr = null;
			cmd = new MySqlCommand();
			cmd.Connection=con;
			//table=new DataTable();
		}

		///<summary></summary>
		private string GetOpenDentalConnStr() {
			XmlDocument document=new XmlDocument();
			string path=ODFileUtils.CombinePaths(Application.StartupPath,"FreeDentalConfig.xml");
			if(!File.Exists(path)) {
				return "";
			}
			string computerName="";
			string database="";
			string user="";
			string password="";
			string passHash="";
			try {
				document.Load(path);
				XmlNodeReader reader=new XmlNodeReader(document);
				string currentElement="";
				while(reader.Read()) {
					if(reader.NodeType==XmlNodeType.Element) {
						currentElement=reader.Name;
					}
					else if(reader.NodeType==XmlNodeType.Text) {
						switch(currentElement) {
							case "ComputerName":
								computerName=reader.Value;
								break;
							case "Database":
								database=reader.Value;
								break;
							case "User":
								user=reader.Value;
								break;
							case "Password":
								password=reader.Value;
								break;
							case "MySQLPassHash":
								passHash=reader.Value;
								break;
						}
					}
				}
				reader.Close();
			}
			catch {
				return "";
			}
			string decryptedPwd;
			if(password=="" && passHash!="" && CDT.Class1.Decrypt(passHash,out decryptedPwd)) {
				//if plain text password is blank but encrypted pwd isn't and decryption is successful, use decryptedPwd
				password=decryptedPwd;
			}
			//example:
			//Server=localhost;Database=opendental;User ID=root;Password=;CharSet=utf8
			return "Server="+computerName
				+";Database="+database
				+";User ID="+user
				+";Password="+password
				+";SslMode=none"
				+";CharSet=utf8";
		}

		///<summary>The problem with this is that if multiple copies of OD are open at the same time, it might get data from only the most recently opened database.  This won't work for some users, so we will normally dynamically alter the connection string.</summary>
		public static string GetODConnStr() {
			//return "Server=localhost;Database=development54;User ID=root;Password=;CharSet=utf8";
			XmlDocument document=new XmlDocument();
			string path=ODFileUtils.CombinePaths(Application.StartupPath,"FreeDentalConfig.xml");
			if(!File.Exists(path)) {
				return "";
			}
			string computerName="";
			string database="";
			string user="";
			string password="";
			string passHash="";
			try {
				document.Load(path);
				XmlNodeReader reader=new XmlNodeReader(document);
				string currentElement="";
				while(reader.Read()) {
					if(reader.NodeType==XmlNodeType.Element) {
						currentElement=reader.Name;
					}
					else if(reader.NodeType==XmlNodeType.Text) {
						switch(currentElement) {
							case "ComputerName":
								computerName=reader.Value;
								break;
							case "Database":
								database=reader.Value;
								break;
							case "User":
								user=reader.Value;
								break;
							case "Password":
								password=reader.Value;
								break;
							case "MySQLPassHash":
								passHash=reader.Value;
								break;
						}
					}
				}
				reader.Close();
			}
			catch {
				return "";
			}
			string decryptedPwd;
			if(password=="" && passHash!="" && CDT.Class1.Decrypt(passHash,out decryptedPwd)) {
				//if plain text password is blank but encrypted pwd isn't and decryption is successful, use decryptedPwd
				password=decryptedPwd;
			}
			//example:
			//Server=localhost;Database=opendental;User ID=root;Password=;CharSet=utf8
			return "Server="+computerName
				+";Database="+database
				+";User ID="+user
				+";Password="+password
				+";SslMode=none"
				+";CharSet=utf8";
		}

		///<summary>Fills table with data from the database.</summary>
		public DataTable GetTable(string command){
			cmd.CommandText=command;
			DataTable table=new DataTable();
 			try{
 				da=new MySqlDataAdapter(cmd);
 				da.Fill(table);
 			}
			//catch(MySql.Data.Types.MySqlConversionException){
			//	MsgBox.Show(this,"Invalid date found. Please fix dates in the Check Database Integrity tool in your main menu under misc tools");
			//}
			catch(Exception){
				MessageBox.Show(command);
			}
			//catch(MySqlException e){
			//	MessageBox.Show("Error: "+e.Message+","+cmd.CommandText);
			//}
			finally{
				con.Close();
			}
 			return table;
		}

		///<summary>Sends a non query command to the database and returns the number of rows affected. If true, then InsertID will be set to the value of the primary key of the newly inserted row.</summary>
		public long NonQ(string command){
			return NonQ(command,false);
		}

		///<summary>Sends a non query command to the database and returns the number of rows affected. If true, then InsertID will be set to the value of the primary key of the newly inserted row.</summary>
		public long NonQ(string command,bool getInsertID){
 			cmd.CommandText=command;
			int rowsChanged=0;
 			try{
				con.Open();
 				rowsChanged=cmd.ExecuteNonQuery();
 				if(getInsertID){
					cmd.CommandText="SELECT LAST_INSERT_ID()";
					dr=(MySqlDataReader)cmd.ExecuteReader();
					if(dr.Read()) {
						InsertID=PIn.Long(dr[0].ToString());
					}
				}
			}
			catch(MySqlException e){
				MessageBox.Show("Error: "+e.Message+","+cmd.CommandText);
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

		///<summary>Use this for count(*) queries.  They are always guaranteed to return one and only one value.  Uses datareader instead of datatable, so faster.  Can also be used when retrieving prefs manually, since they will also return exactly one value</summary>
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









