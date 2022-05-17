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
using System.IO;
using System.Collections;
using System.Text;

namespace fyiReporting.RDL
{
	///<summary>
	/// The definition of a Subreport (report name, parameters, ...).
	///</summary>
	[Serializable]
	internal class Subreport : ReportItem
	{
		string _ReportName;		// The full path (e.g. “/salesreports/orderdetails”) or
								// relative path (e.g. “orderdetails”) to a subreport.
								// Relative paths start in the same folder as the current
								// Report output formats unable to support FitProportional or Clip should output as Fit instead.
								// (parent) report.
								// Cannot be an empty string (ignoring whitespace)
		SubReportParameters _Parameters;	//Parameters to the Subreport
								// If the subreport is executed without parameters
								// (and contains no Toggle elements), it will only be
								// executed once (even if it appears inside of a list,
								// table or matrix)
		Expression _NoRows;		// (string)	Message to display in the subreport (instead of the
								// region layout) when no rows of data are available
								// in any data set in the subreport
								// Note: Style information on the subreport applies to
								// this text.
		bool _MergeTransactions;	// Indicates that transactions in the subreport should
								//be merged with transactions in the parent report
								//(into a single transaction for the entire report) if the
								//data sources use the same connection.	
	
		ReportDefn _ReportDefn;	// loaded report definition

		internal Subreport(ReportDefn r, ReportLink p, XmlNode xNode) :base(r, p, xNode)
		{
			_ReportName=null;
			_Parameters=null;
			_NoRows=null;
			_MergeTransactions=true;

			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				switch (xNodeLoop.Name)
				{
					case "ReportName":
						_ReportName = xNodeLoop.InnerText;
						break;
					case "Parameters":
						_Parameters = new SubReportParameters(r, this, xNodeLoop);
						break;
					case "NoRows":
						_NoRows = new Expression(r, this, xNodeLoop, ExpressionType.String);
						break;
					case "MergeTransactions":
						_MergeTransactions = XmlUtil.Boolean(xNodeLoop.InnerText, OwnerReport.rl);
						break;
					default:	
						if (ReportItemElement(xNodeLoop))	// try at ReportItem level
							break;
						// don't know this element - log it
						OwnerReport.rl.LogError(4, "Unknown Image element " + xNodeLoop.Name + " ignored.");
						break;
				}
			}
		
			if (_ReportName == null)
				OwnerReport.rl.LogError(8, "Subreport requires the ReportName element.");
			
			OwnerReport.ContainsSubreport = true;	// owner report contains a subreport
		}

		// Handle parsing of function in final pass
		override internal void FinalPass()
		{
			base.FinalPass();

			// Subreports aren't allowed in PageHeader or PageFooter; 
			if (this.InPageHeaderOrFooter())
				OwnerReport.rl.LogError(8, String.Format("The Subreport '{0}' is not allowed in a PageHeader or PageFooter", this.Name == null? "unknown": Name.Nm) );

			if (_Parameters != null)
				_Parameters.FinalPass();
			if (_NoRows != null)
				_NoRows.FinalPass();

			_ReportDefn = GetReport(OwnerReport.ParseFolder);
            if (_ReportDefn != null)    // only null in error case (e.g. subreport not found)
			    _ReportDefn.Subreport = this;
			return;
		}

		override internal void Run(IPresent ip, Row row)
		{
			Report r = ip.Report();
			base.Run(ip, row);

			// need to save the owner report and nest in this defintion
			ReportDefn saveReport = r.ReportDefinition;
			r.SetReportDefinition(_ReportDefn);
			r.Folder = _ReportDefn.ParseFolder;		// folder needs to get set since the id of the report is used by the cache
			DataSourcesDefn saveDS = r.ParentConnections;
			if (this.MergeTransactions)
				r.ParentConnections = saveReport.DataSourcesDefn;
			else
				r.ParentConnections = null;

			if (_Parameters == null)
			{	// When no parameters we only retrieve data once
				if (r.Cache.Get(this, "report") == null)
				{
					r.RunGetData(null);
					r.Cache.Add(this, "report", this);
				}
			}
			else
			{
				SetSubreportParameters(r, row);
				r.RunGetData(null);
			}

			ip.Subreport(this, row);

			r.SetReportDefinition(saveReport);			// restore the current report
			r.ParentConnections = saveDS;				// restore the data connnections
		}

		override internal void RunPage(Pages pgs, Row row)
		{
			Report r = pgs.Report;
			if (IsHidden(r, row))
				return;

			base.RunPage(pgs, row);

			// need to save the owner report and nest in this defintion
			ReportDefn saveReport = r.ReportDefinition;
			r.SetReportDefinition(_ReportDefn);
			r.Folder = _ReportDefn.ParseFolder;		// folder needs to get set since the id of the report is used by the cache
			DataSourcesDefn saveDS = r.ParentConnections;
			if (this.MergeTransactions)
				r.ParentConnections = saveReport.DataSourcesDefn;
			else
			    r.ParentConnections = null;

			if (_Parameters == null)
			{	// When no parameters we only retrieve data once
				if (r.Cache.Get(this, "report") == null)
				{
					r.RunGetData(null);
					r.Cache.Add(this, "report", this);	// just put something in cache to remember
				}
			}
			else
			{
				SetSubreportParameters(r, row);		// apply the parameters
				r.RunGetData(null);
			}

			SetPageLeft(r);				// Set the Left attribute since this will be the margin for this report

			SetPagePositionBegin(pgs);

			//
			// Run the subreport -- this is the major effort in creating the display objects in the page
			//
			r.ReportDefinition.Body.RunPage(pgs);		// create a the subreport items

			r.SetReportDefinition(saveReport);			// restore the current report
			r.ParentConnections = saveDS;				// restore the data connnections

			SetPagePositionEnd(pgs, pgs.CurrentPage.YOffset);
		}

		private ReportDefn GetReport(string folder)
		{
			string prog;
			string name;

			if (_ReportName[0] == Path.DirectorySeparatorChar ||
				_ReportName[0] == Path.AltDirectorySeparatorChar)
				name = _ReportName;
			else 
				name = folder + Path.DirectorySeparatorChar + _ReportName;

			name = name + ".rdl";			// TODO: shouldn't necessarily require this extension

			// Load and Compile the report
			RDLParser rdlp;
			Report r;
			ReportDefn rdefn=null;
			try
			{
				prog = GetRdlSource(name);
				rdlp =  new RDLParser(prog);
				rdlp.Folder = folder;
				r = rdlp.Parse(OwnerReport.GetObjectNumber());
				OwnerReport.SetObjectNumber(r.ReportDefinition.GetObjectNumber());
				if (r.ErrorMaxSeverity > 0) 
				{
					string err;
					if (r.ErrorMaxSeverity > 4)
						err = string.Format("Subreport {0} failed to compile with the following errors.", this._ReportName);
					else
						err = string.Format("Subreport {0} compiled with the following warnings.", this._ReportName);
					OwnerReport.rl.LogError(r.ErrorMaxSeverity, err);
					OwnerReport.rl.LogError(r.rl);	// log all these errors
					OwnerReport.rl.LogError(0, "End of Subreport errors");
				}
				// If we've loaded the report; we should tell it where it got loaded from
				if (r.ErrorMaxSeverity <= 4) 
				{	
					rdefn = r.ReportDefinition;
				}
			}
			catch (Exception ex)
			{
				OwnerReport.rl.LogError(8, string.Format("Subreport {0} failed with exception. {1}", this._ReportName, ex.Message));
			}
			return rdefn;
		}

		private string GetRdlSource(string name)
		{
			// TODO: at some point might want to provide interface so that read can be controlled
			//         by server:  would allow for caching etc.
			StreamReader fs=null;
			string prog=null;
			try
			{
				fs = new StreamReader(name);		
				prog = fs.ReadToEnd();
			}
			finally
			{
				if (fs != null)
					fs.Close();
			}

			return prog;
		}

		private void SetSubreportParameters(Report rpt, Row row)
		{
			UserReportParameter userp;
			foreach (SubreportParameter srp in _Parameters.Items)
			{
				userp=null;						
				foreach (UserReportParameter urp in rpt.UserReportParameters)
				{
					if (urp.Name == srp.Name.Nm)
					{
						userp = urp;
						break;
					}
				}
				if (userp == null)
				{	// parameter name not found
					throw new Exception(
						string.Format("Subreport {0} doesn't define parameter {1}.", _ReportName, srp.Name.Nm));
				}
				object v = srp.Value.Evaluate(rpt, row);
				userp.Value = v;
			}
		}

		internal string ReportName
		{
			get { return  _ReportName; }
			set {  _ReportName = value; }
		}

		internal ReportDefn ReportDefn
		{
			get { return _ReportDefn; }
		}

		internal SubReportParameters Parameters
		{
			get { return  _Parameters; }
			set {  _Parameters = value; }
		}

		internal Expression NoRows
		{
			get { return  _NoRows; }
			set {  _NoRows = value; }
		}

		internal bool MergeTransactions
		{
			get { return  _MergeTransactions; }
			set {  _MergeTransactions = value; }
		}
	}
}
