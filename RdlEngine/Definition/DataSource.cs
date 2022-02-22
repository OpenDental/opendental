/* ====================================================================
    Copyright (C) 2004-2005  fyiReporting Software, LLC

    This file is part of the fyiReporting RDL project.
	
    The RDL project is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    For additional information, email info@fyireporting.com or visit
    the website www.fyiReporting.com.
*/

using System;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace fyiReporting.RDL
{
	///<summary>
	/// Information about the data source (e.g. a database connection string).
	///</summary>
	[Serializable]
	public class DataSource : ReportLink
	{
		Name _Name;		// The name of the data source
						// Must be unique within the report
		bool _Transaction;	// Indicates the data sets that use this data
							// source should be executed in a single transaction.
		ConnectionProperties _ConnectionProperties;	//Information about how to connect to the data source
		string _DataSourceReference;	//The full path (e.g.
							// “/salesreports/salesdatabase”) or relative path
							// (e.g. “salesdatabase”) to a data source
							// reference. Relative paths start in the same
							// location as the report.		
	
		[NonSerialized] IDbConnection cnSQL;	// SQL Connection
		[NonSerialized] bool bUserConnection;   // User set connection
		//[NonSerialized] internal static string openDentalConnectionString;

		internal DataSource(Report r, ReportLink p, XmlNode xNode) : base(r, p)
		{
			_Name=null;
			_Transaction=false;
			_ConnectionProperties=null;
			_DataSourceReference=null;
			// Run thru the attributes
			foreach(XmlAttribute xAttr in xNode.Attributes)
			{
				switch (xAttr.Name)
				{
					case "Name":
						_Name = new Name(xAttr.Value);
						break;
				}
			}

			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				switch (xNodeLoop.Name)
				{
					case "Transaction":
						_Transaction = XmlUtil.Boolean(xNodeLoop.InnerText, OwnerReport.rl);
						break;
					case "ConnectionProperties":
						_ConnectionProperties = new ConnectionProperties(r, this, xNodeLoop);
						break;
					case "DataSourceReference":
						_DataSourceReference = xNodeLoop.InnerText;
						break;
					default:
						// don't know this element - log it
						OwnerReport.rl.LogError(4, "Unknown DataSource element '" + xNodeLoop.Name + "' ignored.");
						break;
				}
			}
			if (_Name == null)
				OwnerReport.rl.LogError(8, "DataSource Name is required but not specified.");
			else if (_ConnectionProperties == null && _DataSourceReference == null)
				OwnerReport.rl.LogError(8, string.Format("Either ConnectionProperties or DataSourceReference must be specified for DataSource {0}.", this._Name.Nm));
			else if (_ConnectionProperties != null && _DataSourceReference != null)
				OwnerReport.rl.LogError(8, string.Format("Either ConnectionProperties or DataSourceReference must be specified for DataSource {0} but not both.", this._Name.Nm));
		}
		
		override internal void FinalPass()
		{
			if (_ConnectionProperties != null)
				_ConnectionProperties.FinalPass();

			ConnectDataSource();
			return;
		}

		internal bool IsConnected
		{
			get
			{
				return cnSQL == null? false: true;
			}
		}

		///<summary>C</summary>
		internal bool ConnectDataSource(){
			if (cnSQL != null)
				return true;
			if (_DataSourceReference != null) //js this won't happen
				ConnectDataSourceReference();		// this will create a _ConnectionProperties
			if (_ConnectionProperties == null ||
				_ConnectionProperties.Connectstring == null)
				return false;

			bool rc = false;
			try 
			{
				// connect based on the provider specified
				/*
				switch (_ConnectionProperties.DataProvider.ToLower())
				{
					case "sql":
						// can't connect unless information provided; 
						//   when user wants to set the connection programmatically this they should do this
						if (_ConnectionProperties.Connectstring.Length > 0)	
							cnSQL = new SqlConnection(_ConnectionProperties.Connectstring);
						break;
					case "odbc":
						cnSQL = new OdbcConnection(_ConnectionProperties.Connectstring);
						break;
					case "oledb":
						cnSQL = new OleDbConnection(_ConnectionProperties.Connectstring);
						break;
				}*/
				cnSQL=new MySqlConnection(GetOpenDentalConnStr());
				if (cnSQL != null)
				{
					cnSQL.Open();
					rc = true;
				}
			}
			catch(Exception e)
			{
				OwnerReport.rl.LogError(4, "DataSource '" + _Name + "'.\r\n" + e.Message);
				if (cnSQL != null)
				{
					cnSQL.Close();
					cnSQL = null;
				}
			}

			return rc;
		}

		/// <summary>
		/// Get/Set the database connection.  User must handle closing of connection.
		/// </summary>
		public IDbConnection UserConnection
		{
			get {return bUserConnection? cnSQL: null;}	// never reveal connection internally connected
			set
			{
				this.CleanUp();					// clean up prior connection if necessary

				cnSQL = value;
				bUserConnection = value != null;
			}
		}

		///<summary></summary>
		private static string GetOpenDentalConnStr(){
			XmlDocument document=new XmlDocument();
			string path=Application.StartupPath+"\\"+"FreeDentalConfig.xml";
			//MessageBox.Show(path);
			if(!File.Exists(path)){
				return "";
			}
			string computerName="";
			string database="";
			string user="";
			string password="";
			try{
				document.Load(path);
				XmlNodeReader reader=new XmlNodeReader(document);
				string currentElement="";
				while(reader.Read()){
					if(reader.NodeType==XmlNodeType.Element){
						currentElement=reader.Name;
					}
					else if(reader.NodeType==XmlNodeType.Text){
						switch(currentElement){
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
						}
					}
				}
				reader.Close();
			}
			catch{
				return "";
			}
			return "Server="+computerName
				+";Database="+database
				+";User ID="+user
				+";Password="+password
				+";CharSet=utf8";
		}

		void ConnectDataSourceReference()
		{
			if (_ConnectionProperties != null)
				return;

			try
			{
				string file;
				if (_DataSourceReference[0] != Path.DirectorySeparatorChar)
					file = OwnerReport.Folder + Path.DirectorySeparatorChar + _DataSourceReference + ".dsr";
				else
					file = OwnerReport.Folder + _DataSourceReference + ".dsr";

				string pswd = OwnerReport.GetDataSourceReferencePassword == null? 
									null: OwnerReport.GetDataSourceReferencePassword();
				if (pswd == null)
					throw new Exception("No password provided for shared DataSource reference");

				string xml = RDL.DataSourceReference.Retrieve(file, pswd);
				XmlDocument xDoc = new XmlDocument();
				xDoc.LoadXml(xml);
				XmlNode xNodeLoop = xDoc.FirstChild;
				
				_ConnectionProperties = new ConnectionProperties(OwnerReport, this, xNodeLoop);
			}
			catch (Exception e)
			{
				OwnerReport.rl.LogError(4, e.Message);
				_ConnectionProperties = null;
			}
			return;
		}

		///<summary>Closes the connection and sets it to null.</summary>
		internal void CleanUp()
		{
			if (cnSQL == null || bUserConnection)
				return;
			try 
			{
				cnSQL.Close();
				// cnSQL.Dispose();		// not good for connection pooling
			}
			catch 
			{
			}
			cnSQL=null;
			return;
		}

		internal Name Name
		{
			get { return  _Name; }
			set {  _Name = value; }
		}

		internal bool Transaction
		{
			get { return  _Transaction; }
			set {  _Transaction = value; }
		}

		internal ConnectionProperties ConnectionProperties
		{
			get { return  _ConnectionProperties; }
			set {  _ConnectionProperties = value; }
		}

		internal IDbConnection SqlConnect
		{
			get { return cnSQL; }
			set { cnSQL=value; }//Jordan Sparks: Added this line
		}

		internal string DataSourceReference
		{
			get { return  _DataSourceReference; }
			set {  _DataSourceReference = value; }
		}
	}
}
