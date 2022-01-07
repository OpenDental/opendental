/* ====================================================================
    Copyright (C) 2004-2006  fyiReporting Software, LLC

    This file is part of the fyiReporting RDL project.
	
    This library is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 2.1 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA

    For additional information, email info@fyireporting.com or visit
    the website www.fyiReporting.com.
*/
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;
using System.IO;
using fyiReporting.RDL;

namespace fyiReporting.RDL
{
	///<summary>
	/// Handle SQL configuration and connections
	///</summary>
	public class RdlEngineConfig
	{
		static internal IDictionary SqlEntries=new ListDictionary();	// list of entries
        static internal IDictionary<string, CustomReportItemEntry> CustomReportItemEntries = new System.Collections.Generic.Dictionary<string, CustomReportItemEntry>(5);	// list of entries

		// Compression entries
		static CompressionConfig _Compression=null;

		// Constructor
		static RdlEngineConfig()
		{
			string optFileName = AppDomain.CurrentDomain.BaseDirectory + "RdlEngineConfig.xml";
			bool bLoaded=false;
			
			try
			{
				XmlDocument xDoc = new XmlDocument();
				xDoc.PreserveWhitespace = false;
				try
				{
					xDoc.Load(optFileName);
					bLoaded = true;
				}
				catch
				{
					string relative = AppDomain.CurrentDomain.RelativeSearchPath;
					if (relative != null && relative != String.Empty)
					{
						optFileName = AppDomain.CurrentDomain.BaseDirectory + relative + Path.DirectorySeparatorChar + "RdlEngineConfig.xml";
					
						try
						{
							xDoc.Load(optFileName);
							bLoaded = true;
						}
						catch
						{	 // ok use a hard coded version of the configuration file
						}
					}
				}

				if (!bLoaded)	// we couldn't find the configuration so we'll 
					xDoc.InnerXml = @"
<config>
	<DataSources>
		<DataSource>
			<DataProvider>MySQL.NET</DataProvider>
			<CodeModule>MySql.Data.dll</CodeModule>
			<ClassName>MySql.Data.MySqlClient.MySqlConnection</ClassName>
			<TableSelect>show tables</TableSelect>
			<Interface>SQL</Interface>
			<ReplaceParameters>true</ReplaceParameters>
		</DataSource>
		<DataSource>
			<DataProvider>SQL</DataProvider>
			<TableSelect>SELECT TABLE_NAME, TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES ORDER BY 2, 1</TableSelect>
			<Interface>SQL</Interface>
		</DataSource>
		<DataSource>
			<DataProvider>ODBC</DataProvider>
			<TableSelect>SELECT TABLE_NAME, TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES ORDER BY 2, 1</TableSelect>
			<Interface>SQL</Interface>
			<ReplaceParameters>true</ReplaceParameters>
		</DataSource>
		<DataSource>
			<DataProvider>OLEDB</DataProvider>
			<TableSelect>SELECT TABLE_NAME, TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES ORDER BY 2, 1</TableSelect>
			<Interface>SQL</Interface>
		</DataSource>
		<DataSource>
			<DataProvider>XML</DataProvider>
			<CodeModule>DataProviders.dll</CodeModule>
			<ClassName>fyiReporting.Data.XmlConnection</ClassName>
			<TableSelect></TableSelect>
			<Interface>File</Interface>
		</DataSource>
		<DataSource>
			<DataProvider>WebService</DataProvider>
			<CodeModule>DataProviders.dll</CodeModule>
			<ClassName>fyiReporting.Data.WebServiceConnection</ClassName>
			<TableSelect></TableSelect>
			<Interface>WebService</Interface>
		</DataSource>
		<DataSource>
			<DataProvider>WebLog</DataProvider>
			<CodeModule>DataProviders.dll</CodeModule>
			<ClassName>fyiReporting.Data.LogConnection</ClassName>
			<TableSelect></TableSelect>
			<Interface>File</Interface>
		</DataSource>
		<DataSource>
			<DataProvider>Text</DataProvider>
			<CodeModule>DataProviders.dll</CodeModule>
			<ClassName>fyiReporting.Data.TxtConnection</ClassName>
			<TableSelect></TableSelect>
			<Interface>File</Interface>
		</DataSource>
		<DataSource>
			<DataProvider>FileDirectory</DataProvider>
			<CodeModule>DataProviders.dll</CodeModule>
			<ClassName>fyiReporting.Data.FileDirConnection</ClassName>
			<TableSelect></TableSelect>
			<Interface>File</Interface>
		</DataSource>
	</DataSources>
  <CustomReportItems>
    <CustomReportItem>
      <Type>BarCode</Type>
      <CodeModule>RdlCri.dll</CodeModule>
      <ClassName>fyiReporting.CRI.BarCode</ClassName>
    </CustomReportItem>
  </CustomReportItems>
</config>";
				
				XmlNode xNode;
				xNode = xDoc.SelectSingleNode("//config");

				// Loop thru all the child nodes
				foreach(XmlNode xNodeLoop in xNode.ChildNodes)
				{
					if (xNodeLoop.NodeType != XmlNodeType.Element)
						continue;
					switch (xNodeLoop.Name)
					{
						case "DataSources":
							GetDataSources(xNodeLoop);
							break;
						case "Compression":
							GetCompression(xNodeLoop);
							break;
                        case "CustomReportItems":
                            GetCustomReportItems(xNodeLoop);
                            break;
						default:
							break;
					}
				}
			}
			catch
			{		// Didn't sucessfully get the startup state; but nobody to complain to
			}

			return;
		}
		
		internal static CompressionConfig GetCompression()
		{
			return _Compression;
		}

		static void GetCompression(XmlNode xNode)
		{
			// loop thru looking to process all the datasource elements
			string cm=null;
			string cn=null;
			string fn=null;
            bool bEnable = true;
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				switch (xNodeLoop.Name)
				{
					case "CodeModule":
                        if (xNodeLoop.InnerText.Length > 0)
						    cm = xNodeLoop.InnerText;
						break;
					case "ClassName":
                        if (xNodeLoop.InnerText.Length > 0)
                            cn = xNodeLoop.InnerText;
						break;
					case "Finish":
                        if (xNodeLoop.InnerText.Length > 0)
                            fn = xNodeLoop.InnerText;
						break;
                    case "Enable":
                        if (xNodeLoop.InnerText.ToLower() == "false")
                            bEnable = false;
                        break;
				}
				
			}	
			if (bEnable)
				_Compression = new CompressionConfig(cm, cn, fn);
		}
		
		static void GetDataSources(XmlNode xNode)
		{
			// loop thru looking to process all the datasource elements
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				if (xNodeLoop.Name != "DataSource")
					continue;
				GetDataSource(xNodeLoop);
			}	
		}

		static void GetDataSource(XmlNode xNode)
		{
			string provider=null;
			string codemodule=null;
			string cname=null;
			string inter="SQL";
			string tselect=null;
			bool replaceparameters=false;
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				switch (xNodeLoop.Name)
				{
					case "DataProvider":
						provider = xNodeLoop.InnerText;
						break;
					case "CodeModule":
						codemodule = xNodeLoop.InnerText;
						break;
					case "Interface":
						inter = xNodeLoop.InnerText;
						break;
					case "ClassName":
						cname = xNodeLoop.InnerText;
						break;
					case "TableSelect":
						tselect = xNodeLoop.InnerText;
						break;
					case "ReplaceParameters":
						if (xNodeLoop.InnerText.ToLower() == "true")
							replaceparameters = true;
						break;
					default:
						break;
				}
			}
			if (provider == null)
				return;			// nothing to do if no provider specified

			SqlConfigEntry sce;
			try
			{   // load the module early; saves problems with concurrency later
				string msg=null;
				Assembly la=null;
				if (codemodule != null && cname != null)
				{
					la = XmlUtil.AssemblyLoadFrom(codemodule);
					if (la == null)
						msg = string.Format("{0} could not be loaded", codemodule);
				}
				sce = new SqlConfigEntry(provider, cname, la, tselect, msg);
				SqlEntries.Add(provider, sce);
			}
			catch (Exception e)
			{		// keep exception;  if this DataProvided is ever useed we will see the message
				sce = new SqlConfigEntry(provider, cname, null, tselect, e.Message);
				SqlEntries.Add(provider, sce);
			}
			sce.ReplaceParameters = replaceparameters;
		}

		public static IDbConnection GetConnection(string provider, string cstring)
		{
			IDbConnection cn = null;
			switch (provider.ToLower())
			{
				case "sql":
					// can't connect unless information provided; 
					//   when user wants to set the connection programmatically this they should do this
					if (cstring.Length > 0)	
						cn = new SqlConnection(cstring);
					break;
				case "odbc":
					cn = new OdbcConnection(cstring);
					break;
				case "oledb":
					cn = new OleDbConnection(cstring);
					break;
				default:
					SqlConfigEntry sce = SqlEntries[provider] as SqlConfigEntry;
					if (sce == null || sce.CodeModule == null)
					{
						if (sce != null && sce.ErrorMsg != null)	// error during initialization??
							throw new Exception(sce.ErrorMsg);
						break;
					}
					object[] args = new object[] {cstring};
					Assembly asm = sce.CodeModule;
					object o = asm.CreateInstance(sce.ClassName, false, 
						BindingFlags.CreateInstance, null, args, null,null);
					if (o == null)
						throw new Exception(string.Format("Unable to create instance of '{0}' for provider '{1}'", sce.ClassName, provider));
					cn = o as IDbConnection;
					break;
			}

			return cn;
		}
		
		static public string GetTableSelect(string provider)
		{
			return GetTableSelect(provider, null);
		}

		static public bool DoParameterReplacement(string provider, IDbConnection cn)
		{
			SqlConfigEntry sce = SqlEntries[provider] as SqlConfigEntry;
			return sce == null? false: sce.ReplaceParameters;
		}

		static public string GetTableSelect(string provider, IDbConnection cn)
		{
			SqlConfigEntry sce = SqlEntries[provider] as SqlConfigEntry;
			if (sce == null)
			{
				if (cn != null)
				{
					OdbcConnection oc = cn as OdbcConnection;
					if (oc != null && oc.Driver.ToLower().IndexOf("my") >= 0)	// not a good way but ...
						return "SHOW TABLES";					// mysql syntax is non-standard 
				}
				return "SELECT TABLE_NAME, TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES ORDER BY 2, 1";
			}
			if (cn != null)
			{
				OdbcConnection oc = cn as OdbcConnection;
				if (oc != null && oc.Driver.ToLower().IndexOf("my") >= 0)	// not a good way but ...
					return "SHOW TABLES";					// mysql syntax is non-standard 
			}
			return sce.TableSelect;
		}

		static public string[] GetProviders()
		{
			if (SqlEntries.Count == 0)
				return null;
			string[] items = new string[SqlEntries.Count];
			int i=0;
			foreach (SqlConfigEntry sce in SqlEntries.Values)
			{
				items[i++] = sce.Provider;
			}
			return items;
		}

        static public string[] GetCustomReportTypes()
        {
            if (CustomReportItemEntries.Count == 0)
                return null;
            string[] items = new string[CustomReportItemEntries.Count];
            int i = 0;
            foreach (CustomReportItemEntry crie in CustomReportItemEntries.Values)
            {
                items[i++] = crie.Type;
            }
            return items;
        }

        static void GetCustomReportItems(XmlNode xNode)
        {
            // loop thru looking to process all the datasource elements
            foreach (XmlNode xNodeLoop in xNode.ChildNodes)
            {
                if (xNodeLoop.NodeType != XmlNodeType.Element)
                    continue;
                if (xNodeLoop.Name != "CustomReportItem")
                    continue;
                GetCustomReportItem(xNodeLoop);
            }
        }

        static void GetCustomReportItem(XmlNode xNode)
        {
            string type = null;
            string codemodule = null;
            string classname = null;
            foreach (XmlNode xNodeLoop in xNode.ChildNodes)
            {
                if (xNodeLoop.NodeType != XmlNodeType.Element)
                    continue;
                switch (xNodeLoop.Name)
                {
                    case "Type":
                        type = xNodeLoop.InnerText;
                        break;
                    case "CodeModule":
                        codemodule = xNodeLoop.InnerText;
                        break;
                    case "ClassName":
                        classname = xNodeLoop.InnerText;
                        break;
                    default:
                        break;
                }
            }
            if (type == null)
                return;			// nothing to do if no provider specified

            CustomReportItemEntry crie;
            try
            {   // load the module early; saves problems with concurrency later
                string msg = null;
                Assembly la = null;
                if (codemodule != null && classname != null)
                {
                    la = XmlUtil.AssemblyLoadFrom(codemodule);
                    if (la == null)
                        msg = string.Format("{0} could not be loaded", codemodule);
                }
                crie = new CustomReportItemEntry(type, classname, la, msg);
                CustomReportItemEntries.Add(type, crie);
            }
            catch (Exception e)
            {		// keep exception;  if this CustomReportItem is ever used we will see the message
                crie = new CustomReportItemEntry(type, classname, null, e.Message);
                CustomReportItemEntries.Add(type, crie);
            }
        }

        public static ICustomReportItem CreateCustomReportItem(string type)
        {
            CustomReportItemEntry crie = null; 
            try { crie = CustomReportItemEntries[type]; }
            catch // KeyNotFoundException typically
            {
                throw new Exception(string.Format("{0} is not a known CustomReportItem type", type));
            }
            if (crie.CodeModule == null)
            {
                if (crie != null && crie.ErrorMsg != null)	// error during initialization??
                    throw new Exception(crie.ErrorMsg);
                else
                    throw new Exception(string.Format("{0} is not a known CustomReportItem type", type));
            }

            Assembly asm = crie.CodeModule;
            object o = asm.CreateInstance(crie.ClassName, false,
                        BindingFlags.CreateInstance, null, null, null, null);
            if (o == null)
                throw new Exception(string.Format("Unable to create instance of '{0}' for CustomReportType '{1}'", crie.ClassName, type));

            return o as ICustomReportItem;
        }
    }

	internal class CompressionConfig
	{
		int _UseCompression=-1;
		Assembly _Assembly=null;
		string _CodeModule;
		string _ClassName;
		string _Finish;
		MethodInfo _FinishMethodInfo;	//	if there is a finish method
		string _ErrorMsg;				// error encountered loading compression

		internal CompressionConfig(string cm, string cn, string fn)
		{
			_CodeModule = cm;
			_ClassName = cn;
			_Finish = fn;
            if (cm == null || cn == null || fn == null)
                _UseCompression = 2;
		}

		internal bool CanCompress
		{
			get
			{
				if (_UseCompression >= 1)	// we've already successfully inited
					return true;
				if (_UseCompression == 0)	// we've tried to init and failed
					return false;
				Init();						// initialize compression
				return _UseCompression == 1;	// and return the status
			}
		}

		internal void CallStreamFinish(Stream strm)
		{
			if (_FinishMethodInfo == null)
				return;

			object returnVal = _FinishMethodInfo.Invoke(strm, null);

			return;
		}

		internal Stream GetStream(Stream str)
		{
            if (_UseCompression == 2)
            {   // use the built-in compression .NET 2 provides
                System.IO.Compression.DeflateStream cs = 
                    new System.IO.Compression.DeflateStream(str, System.IO.Compression.CompressionMode.Compress);
                return cs;
            }

			if (_UseCompression == 0)
				return null;
			if (_UseCompression == -1)	// make sure we're init'ed
			{
				Init();	
				if (_UseCompression != 1)
					return null;
			}

			try
			{
				object[] args = new object[] {str};

				Stream so = _Assembly.CreateInstance(_ClassName, false, 
					BindingFlags.CreateInstance, null, args, null,null) as Stream;
				return so;
			}
			catch 
			{
				return null;
			}
		}

		internal string ErrorMsg
		{
			get {return _ErrorMsg;}
		}

		void Init()
		{
			lock (this)
			{
				if (_UseCompression != -1)
					return;
				_UseCompression = 0;		// assume failure

				try
				{
					// Load the assembly
					_Assembly = XmlUtil.AssemblyLoadFrom(_CodeModule);

					// Load up a test stream to make sure it will work
					object[] args = new object[] {new MemoryStream()};

					Stream so = _Assembly.CreateInstance(_ClassName, false, 
						BindingFlags.CreateInstance, null, args, null,null) as Stream;

					if (so != null)
					{		// we've successfully inited
						so.Close();
						_UseCompression = 1;
					}
					else
						_Assembly = null;

					if (_Finish != null)
					{
						Type theClassType= so.GetType();
						this._FinishMethodInfo = theClassType.GetMethod(_Finish);
					}
				}
				catch (Exception e)
				{
					_ErrorMsg = e.InnerException == null? e.Message: e.InnerException.Message;
				}

			}
		}
	}
	
	internal class SqlConfigEntry
	{
		internal string Provider;
		internal Assembly CodeModule;
		internal string ClassName;
		internal string TableSelect;
		internal string ErrorMsg;
		internal bool ReplaceParameters;
		internal SqlConfigEntry(string provider, string cname, Assembly codemodule, string tselect, string msg)
		{
			Provider = provider;
			CodeModule = codemodule;
			ClassName = cname;
			TableSelect = tselect;
			ErrorMsg = msg;
			ReplaceParameters = false;
		}
	}

    internal class CustomReportItemEntry
    {
        internal string Type;
        internal Assembly CodeModule;
        internal string ClassName;
        internal string ErrorMsg;
        internal CustomReportItemEntry(string type, string cname, Assembly codemodule, string msg)
        {
            Type = type;
            CodeModule = codemodule;
            ClassName = cname;
            ErrorMsg = msg;
        }
    }

}
