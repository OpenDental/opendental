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
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;
using System.IO;

namespace fyiReporting.RDL
{
	///<summary>
	/// Information about the data source (e.g. a database connection string).
	///</summary>
	[Serializable]
	public class DataSource
	{
		Report _rpt;	// Runtime report
		DataSourceDefn _dsd;	// DataSource definition

		internal DataSource(Report rpt, DataSourceDefn dsd)
		{
			_rpt = rpt;
			_dsd = dsd;
		}

		/// <summary>
		/// Get/Set the database connection.  User must handle closing of connection.
		/// </summary>
		public IDbConnection UserConnection
		{
			get {return _dsd.IsUserConnection(_rpt)? _dsd.GetConnection(_rpt): null;}	// never reveal connection internally connected
			set
			{
				_dsd.CleanUp(_rpt);					// clean up prior connection if necessary

				_dsd.SetUserConnection(_rpt, value);
			}
		}
	}
}
