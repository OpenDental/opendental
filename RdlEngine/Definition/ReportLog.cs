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
using System.Collections;
using System.Collections.Generic;
using System.Xml;


namespace fyiReporting.RDL
{
	///<summary>
	/// Error logging (parse and runtime) within report.
	///</summary>
	[Serializable]
	internal class ReportLog
	{
		List<string> _ErrorItems=null;			// list of report items
		int _MaxSeverity=0;				// maximum severity encountered				

		internal ReportLog()
		{
		}

		internal ReportLog(ReportLog rl)
		{
			if (rl != null && rl.ErrorItems != null)
			{
				_MaxSeverity = rl.MaxSeverity;
                _ErrorItems = new List<string>(rl.ErrorItems);
			}
		}

		internal void LogError(ReportLog rl)
		{
			if (rl.ErrorItems.Count == 0)
				return;
			LogError(rl.MaxSeverity, rl.ErrorItems);
		}

		internal void LogError(int severity, string item)
		{
			if (_ErrorItems == null)			// create log if first time
                _ErrorItems = new List<string>();

			if (severity > _MaxSeverity)
				_MaxSeverity = severity;

			string msg = "Severity: " + Convert.ToString(severity) + " - " + item;

			_ErrorItems.Add(msg);

			if (severity >= 12)		
				throw new Exception(msg);		// terminate the processing

			return;
		}

		internal void LogError(int severity, List<string> list)
		{
			if (_ErrorItems == null)			// create log if first time
                _ErrorItems = new List<string>();

			if (severity > _MaxSeverity)
				_MaxSeverity = severity;

			_ErrorItems.AddRange(list);

			return;
		}

		internal void Reset()
		{
			_ErrorItems=null;
			if (_MaxSeverity < 8)				// we keep the severity to indicate we can't run report
				_MaxSeverity=0;
		}

        internal List<string> ErrorItems
		{
			get { return  _ErrorItems; }
		}


		internal int MaxSeverity
		{
			get { return  _MaxSeverity; }
			set {  _MaxSeverity = value; }
		}
	}
}
