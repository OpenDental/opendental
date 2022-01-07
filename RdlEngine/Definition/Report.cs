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
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using fyiReporting.RDL;

namespace fyiReporting.RDL
{
	/// <summary>
	/// Delegate used to ask for a Data Source Reference password used to decrypt the file.
	/// </summary>
	public delegate string NeedPassword();

	///<summary>
	/// Main Report definition; this is the top of the tree that contains the complete
	/// definition of a instance of a report.
	///</summary>
	[Serializable]
	public class Report
	{
		internal ReportLog rl;	// report log
		Name _Name;				// Name of the report
		string _Description;	// Description of the report
		string _Author;			// Author of the report
		int _AutoRefresh;		// Rate at which the report page automatically refreshes, in seconds.  Must be nonnegative.
		//    If omitted or zero, the report page should not automatically refresh.
		//    Max: 2147483647
		DataSources _DataSources;	// Describes the data sources from which
		public NeedPassword GetDataSourceReferencePassword=null;

		//		data sets are taken for this report.
		DataSets _DataSets;	// Describes the data that is displayed as
		// part of the report
		Body _Body;				// Describes how the body of the report is structured
		ReportParameters _ReportParameters;	// Parameters for the report
		Custom _Customer;		// Custom information to be handed to the report engine
		RSize _Width;			// Width of the report
		PageHeader _PageHeader;	// The header that is output at the top of each page of the report.
		PageFooter _PageFooter;	// The footer that is output at the bottom of each page of the report.
		RSize _PageHeight;		// Default height for the report.  Default is 11 in.
		RSize _PageWidth;		// Default width for the report. Default is 8.5 in.
		RSize _LeftMargin;		// Width of the left margin. Default: 0 in
		RSize _RightMargin;		// Width of the right margin. Default: 0 in
		RSize _TopMargin;		// Width of the top margin. Default: 0 in
		RSize _BottomMargin;	// Width of the bottom margin. Default: 0 in
		EmbeddedImages _EmbeddedImages;	// Images embedded within the report
		Expression _Language;	// The primary language of the text. Default is server language.
		CodeModules _CodeModules;	// Code modules to make available to the
		//		report for use in expressions.
		Classes _Classes;		// Classes 0-1 Element Classes to instantiate during report initialization
		string _DataTransform;	// The location to a transformation to apply
		// to a report data rendering. This can be a full folder path (e.g. “/xsl/xfrm.xsl”),
		// relative path (e.g. “xfrm.xsl”).
		string _DataSchema;		// The schema or namespace to use for a report data rendering.
		string _DataElementName;	// Name of a top level element that
		//		represents the report data. Default: Report.
		DataElementStyleEnum _DataElementStyle;		//Indicates whether textboxes should
		//		render as elements or attributes.

		// Following variables used for parsing/evaluating expressions
		ArrayList _DataCache;	// contains all function that implement ICacheData
		IDictionary _LUGlobals;	// contains global and user properties
		IDictionary _LUUser;	// contains global and user properties
		IDictionary _LUReportItems;	// all TextBoxes in the report	IDictionary _LUGlobalsUser;		// contains global and user properties
		IDictionary _LUAggrScope;	// Datasets, Dataregions, grouping names
		IDictionary _LUEmbeddedImages;	// Embedded images

		// Globals
		FunctionPageNumber _PageNumber;		// Current page number expression
		FunctionTotalPages _TotalPages;		// Total pages expression
		FunctionExecutionTime _ExecutionTime;	// The DateTime the report began executing
		FunctionReportFolder _ReportFolder;	// Implementation specific path of the folder containing the report
		//   e.g. /salesreport/budgeting or c:\sales\budget
		FunctionReportName _ReportName;		// Name of the report -- see _Name
		// User 
		IExpr _UserID;			// ID of the user executing the report
		IExpr _ClientLanguage;	// Language code of the client executing the report.

		// Runtime names; occasionally a routine needs a globally unique name that 
		[NonSerialized] int _RuntimeName;		// used for the generation of unique runtime names
		[NonSerialized] IDictionary _LURuntimeName;		// Runtime names
		[NonSerialized] ICollection _UserParameters;	// User parameters
		[NonSerialized] Subreport _Subreport;	// for subreports the owner report; null if top level report

		// Constructor
		internal Report(XmlNode xNode, ReportLog replog, string folder, NeedPassword getpswd)		// report has no parents
		{
			rl = replog;				// used for error reporting
			GetDataSourceReferencePassword = getpswd;
			_Description = null;
			_Author = null;		
			_AutoRefresh = -1;
			_DataSources = null;
			_DataSets = null;	
			_Body = null;		
			_Width = null;		
			_PageHeader = null;	
			_PageFooter = null;	
			_PageHeight = null;	
			_PageWidth = null;	
			_LeftMargin = null;	
			_RightMargin = null;
			_TopMargin = null;	
			_BottomMargin = null;
			_EmbeddedImages = null;
			_Language = null;	
			_CodeModules = null;	
			_Classes = null;	
			_DataTransform = null;	
			_DataSchema = null;		
			_DataElementName = null;
			_DataElementStyle = DataElementStyleEnum.AttributeNormal;
			_LUReportItems = new Hashtable();		// to hold all the textBoxes
			_LUAggrScope = new ListDictionary();	// to hold all dataset, dataregion, grouping names
			_RuntimeName =0;
			_LURuntimeName = new ListDictionary();	// shouldn't be very many of these
			_LUEmbeddedImages = new ListDictionary();	// probably not very many
			_DataCache = new ArrayList();

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
					case "Description":
						_Description = xNodeLoop.InnerText;
						break;
					case "Author":
						_Author = xNodeLoop.InnerText;
						break;
					case "AutoRefresh":
						_AutoRefresh = XmlUtil.Integer(xNodeLoop.InnerText);
						break;
					case "DataSources":
						_DataSources = new DataSources(this, null, xNodeLoop);
						break;
					case "DataSets":
						_DataSets = new DataSets(this, null, xNodeLoop);
						break;
					case "Body":
						_Body = new Body(this, null, xNodeLoop);
						break;
					case "ReportParameters":
						_ReportParameters = new ReportParameters(this, null, xNodeLoop);
						break;
					case "Width":
						_Width = new RSize(this, xNodeLoop);
						break;
					case "PageHeader":
						_PageHeader = new PageHeader(this, null, xNodeLoop);
						break;
					case "PageFooter":
						_PageFooter = new PageFooter(this, null, xNodeLoop);
						break;
					case "PageHeight":
						_PageHeight = new RSize(this, xNodeLoop);
						break;
					case "PageWidth":
						_PageWidth = new RSize(this, xNodeLoop);
						break;
					case "LeftMargin":
						_LeftMargin = new RSize(this, xNodeLoop);
						break;
					case "RightMargin":
						_RightMargin = new RSize(this, xNodeLoop);
						break;
					case "TopMargin":
						_TopMargin = new RSize(this, xNodeLoop);
						break;
					case "BottomMargin":
						_BottomMargin = new RSize(this, xNodeLoop);
						break;
					case "EmbeddedImages":
						_EmbeddedImages = new EmbeddedImages(this, null, xNodeLoop);
						break;
					case "Language":
						_Language =  new Expression(this, null, xNodeLoop, ExpressionType.String);
						break;
					case "CodeModules":
						_CodeModules = new CodeModules(this, null, xNodeLoop);
						break;
					case "Classes":
						_Classes = new Classes(this, null, xNodeLoop);
						break;
					case "DataTransform":
						_DataTransform = xNodeLoop.InnerText;
						break;
					case "DataSchema":
						_DataSchema = xNodeLoop.InnerText;
						break;
					case "DataElementName":
						_DataElementName = xNodeLoop.InnerText;
						break;
					case "DataElementStyle":
						_DataElementStyle = fyiReporting.RDL.DataElementStyle.GetStyle(xNodeLoop.InnerText, this.rl);
						break;
					default:
						// don't know this element - log it
						this.rl.LogError(4, "Unknown Report element '" + xNodeLoop.Name + "' ignored.");
						break;
				}
			}

			if (_Body == null)
				rl.LogError(8, "Body not specified for report.");

			if (_Width == null)
				rl.LogError(4, "Width not specified for report.  Assuming page width.");

			if (rl.MaxSeverity <= 4)	// don't do final pass if already have serious errors
			{
				FinalPass(folder);	// call final parser pass for expression resolution
			}

			// Cleanup any dangling resources
			if (_DataSources != null)
				_DataSources.CleanUp();
		}

		//
		void FinalPass(string folder)
		{
			// Now do some addition validation and final preparation

			// Prepare the Globals and User variables
			_PageNumber = new FunctionPageNumber();	
			_TotalPages = new FunctionTotalPages();	
			_ExecutionTime = new FunctionExecutionTime();	
			_ReportFolder = new FunctionReportFolder();	
			this.Folder = folder;						// set the folder
			_ReportName = new FunctionReportName();	
			_UserID = new FunctionUserID();		
			_ClientLanguage = new FunctionUserLanguage();	

			// Create the Globals and User lookup dictionaries
			_LUGlobals = new ListDictionary();	// if entries grow beyond 10; make hashtable
			_LUGlobals.Add("PageNumber", _PageNumber);
			_LUGlobals.Add("TotalPages", _TotalPages);
			_LUGlobals.Add("ExecutionTime", _ExecutionTime);
			_LUGlobals.Add("ReportFolder", _ReportFolder);
			_LUGlobals.Add("ReportName", _ReportName);
			_LUUser = new ListDictionary();		// if entries grow beyond 10; make hashtable
			_LUUser.Add("UserID", _UserID);
			_LUUser.Add("Language", _ClientLanguage);
			if (_CodeModules != null)
			{
				_CodeModules.FinalPass();
				_CodeModules.LoadModules();
			}
			if (_Classes != null)
			{
				_Classes.FinalPass();
				_Classes.Load();
			}

			if (_DataSources != null)
				_DataSources.FinalPass();
			if (_DataSets != null)
				_DataSets.FinalPass();
			_Body.FinalPass();
			if (_ReportParameters != null)
				_ReportParameters.FinalPass();
			if (_PageHeader != null)
				_PageHeader.FinalPass();
			if (_PageFooter != null)
				_PageFooter.FinalPass();
			if (_EmbeddedImages != null)
				_EmbeddedImages.FinalPass();
			if (_Language != null)
				_Language.FinalPass();

			_DataCache.TrimToSize();	// reduce size of array of expressions that cache data
			return;
		}

		private void ResetCachedData()
		{
			foreach (ICacheData icd in this._DataCache)
				icd.ClearCache();
		}

		internal void Run(IPresent ip)
		{
			if (_Subreport == null)
			{	// do true intialization
				_RuntimeName = 0;				// start names off at zero
				ip.Start();
			}

			if (ip.IsPagingNeeded())
			{
				RunPage(ip);
			}
			else
			{
				if (_PageHeader != null && !(ip is RenderXml))
					_PageHeader.Run(ip, null);
				_Body.Run(ip, null);
				if (_PageFooter != null && !(ip is RenderXml))
					_PageFooter.Run(ip, null);
			}

			if (_Subreport == null)
				ip.End();
		}

		// Obtain the data for the report
		public void RunGetData(IDictionary parms)
			//Called from RdlViewer.GetPages (line836)
		{
			_ExecutionTime.StartReport = DateTime.Now;

			// Step 1- set the parameter values for the runtime
			if (parms != null && ReportParameters != null)
				ReportParameters.SetRuntimeValues(parms);	// set the parameters

			// Step 2- prep the datasources (ie connect and execute the queries)
			if (DataSources != null)
				DataSources.ConnectDataSources();

			// Step 3- obtain the data; applying filters
			if (DataSets != null)
			{
				ResetCachedData();
				DataSets.GetData();
			}

			// Step 4- cleanup any DB connections
			if (DataSources != null)
				DataSources.CleanUp();

			return;
		}

		// Renders the report; RunGetData must be run before this.
		public void RunRender(IStreamGen sg, OutputPresentationType type)
		{
			if (sg == null)
				throw new ArgumentException("IStreamGen argument cannot be null.", "sg");

			PageNumber.RuntimePageNumber = 1;		// reset page numbers
			TotalPages.RuntimePageCount = 1;
			IPresent ip;
			MemoryStreamGen msg = null;
			switch (type)
			{
				case OutputPresentationType.PDF:
					ip = new RenderPdf(this, sg);
					break;
				case OutputPresentationType.XML:
					if (this.DataTransform != null && DataTransform.Length > 0)
					{
						msg = new MemoryStreamGen();
						ip = new RenderXml(this, msg);
					}
					else
						ip = new RenderXml(this, sg);    
					break;
				case OutputPresentationType.HTML:
				default:
					ip = new RenderHtml(this, sg);
					break;
			}
			Run(ip);

			// When msg isn't null we have to do a datatransform on the XML in the data stream
			if (msg != null)
			{
				try
				{
					string file;
					if (this.DataTransform[0] != Path.DirectorySeparatorChar)
						file = this.Folder + Path.DirectorySeparatorChar + _DataTransform;
					else
						file = this.Folder + _DataTransform;
					XmlUtil.XslTrans(file, msg.GetText(), sg.GetStream());
				}	
				catch (Exception ex)
				{
					this.rl.LogError(8, "Error processing DataTransform " + ex.Message + "\r\n" + ex.StackTrace);
				}
				finally 
				{
					msg.Dispose();
				}
			}

			sg.CloseMainStream();

			return;
		}

		internal void RunPage(IPresent ip)
		{
			Pages pgs = new Pages(this);
			try
			{
				Page p = new Page(1);				// kick it off with a new page
				pgs.AddPage(p);

				// Create all the pages
				_Body.RunPage(pgs);

				if (pgs.LastPage.IsEmpty())			// get rid of extraneous pages which
					pgs.RemoveLastPage();			//   can be caused by region page break at end

				// Now create the headers and footers for all the pages (as needed)
				if (_PageHeader != null)
					_PageHeader.RunPage(pgs);
				if (_PageFooter != null)
					_PageFooter.RunPage(pgs);

				ip.RunPages(pgs);
			}
			finally
			{
				pgs.CleanUp();		// always want to make sure we clean this up since 
			}

			return;
		}
		/// <summary>
		/// RunRenderPdf will render a Pdf given the page structure
		/// </summary>
		/// <param name="sg"></param>
		/// <param name="pgs"></param>
		public void RunRenderPdf(IStreamGen sg, Pages pgs)
		{
			PageNumber.RuntimePageNumber = 1;		// reset page numbers
			TotalPages.RuntimePageCount = 1;

			IPresent ip = new RenderPdf(this, sg);	
			try
			{
				ip.Start();
				ip.RunPages(pgs);
				ip.End();
			}
			finally
			{
				pgs.CleanUp();		// always want to make sure we cleanup to reduce resource usage
			}

			return;
		}

		public Pages BuildPages()
		{
			PageNumber.RuntimePageNumber = 1;		// reset page numbers
			TotalPages.RuntimePageCount = 1;

			Pages pgs = new Pages(this);
			pgs.PageHeight = this.PageHeight.Points;
			pgs.PageWidth = this.PageWidth.Points;
			try
			{
				Page p = new Page(1);				// kick it off with a new page
				pgs.AddPage(p);

				// Create all the pages
				_Body.RunPage(pgs);

				if (pgs.LastPage.IsEmpty() && pgs.PageCount > 1) // get rid of extraneous pages which
					pgs.RemoveLastPage();			//   can be caused by region page break at end

				// Now create the headers and footers for all the pages (as needed)
				if (_PageHeader != null)
					_PageHeader.RunPage(pgs);
				if (_PageFooter != null)
					_PageFooter.RunPage(pgs);
			}
			catch (Exception e)
			{
				rl.LogError(8, "Exception running report\r\n" + e.Message + "\r\n" + e.StackTrace);
			}
			finally
			{
				pgs.CleanUp();		// always want to make sure we clean this up since 
			}

			return pgs;
		}

		public string Description
		{
			get { return  _Description; }
			set {  _Description = value; }
		}

		public string Author
		{
			get { return  _Author; }
			set {  _Author = value; }
		}

		internal int AutoRefresh
		{
			get { return  _AutoRefresh; }
			set {  _AutoRefresh = value; }
		}

		internal ArrayList DataCache
		{
			get { return _DataCache; }
		}

		public DataSources DataSources
		{
			get { return  _DataSources; }
		}

		public DataSets DataSets
		{
			get { return  _DataSets; }
//			set {  _DataSets = value; }
		}

		internal Body Body
		{
			get { return  _Body; }
			set {  _Body = value; }
		}

		internal ReportParameters ReportParameters
		{
			get { return  _ReportParameters; }
			set {  _ReportParameters = value; }
		}
		/// <summary>
		/// User provided parameters to the report.  IEnumerable is a list of UserReportParameter.
		/// </summary>
		public ICollection UserReportParameters
		{
			get 
			{
				if (_UserParameters != null)	// only create this once
					return _UserParameters;		//  since it can be expensive to build

				if (_ReportParameters == null || _ReportParameters.Count <= 0)
				{
					_UserParameters = new ArrayList(1);
				}
				else
				{
					ArrayList parms = new ArrayList(_ReportParameters.Count);
					foreach (ReportParameter p in _ReportParameters)
					{
						UserReportParameter urp = new UserReportParameter(p);
						parms.Add(urp);
					}
					_UserParameters = parms;
				}
				return _UserParameters;
			}
		}

		internal Custom Customer
		{
			get { return  _Customer; }
			set {  _Customer = value; }
		}

		public string Folder
		{
			get 
			{
				return _ReportFolder.Folder == null? "": _ReportFolder.Folder;
			}
			set 
			{
				_ReportFolder.Folder = value;
			}
		}

		public string Name
		{
			get 
			{
				return _ReportName.Name;
			}
			set 
			{
				_ReportName.Name = value;
			}
		}

		internal RSize Width
		{
			get 
			{
				if (_Width == null)			// Shouldn't be need since technically Width is required (I let it slip)	
					_Width = PageWidth;		// Not specified; assume page width

				return  _Width; 
			}
			set {  _Width = value; }
		}

		internal PageHeader PageHeader
		{
			get { return  _PageHeader; }
			set {  _PageHeader = value; }
		}

		internal PageFooter PageFooter
		{
			get { return  _PageFooter; }
			set {  _PageFooter = value; }
		}

		internal RSize PageHeight
		{
			get 
			{
				if (this.Subreport != null)
					return Subreport.OwnerReport.PageHeight;

				if (_PageHeight == null)			// default height is 11 inches
					_PageHeight = new RSize(this, "11 in");
				return  _PageHeight; 
			}
			set {  _PageHeight = value; }
		}

		public float PageHeightPoints
		{
			get 
			{
				return PageHeight.Points;
			}
		}

		internal RSize PageWidth
		{
			get 
			{
				if (this.Subreport != null)
					return Subreport.OwnerReport.PageWidth;

				if (_PageWidth == null)				// default width is 8.5 inches
					_PageWidth = new RSize(this, "8.5 in");

				return  _PageWidth; 
			}
			set {  _PageWidth = value; }
		}

		public float PageWidthPoints
		{
			get 
			{
				return PageWidth.Points;
			}
		}

		internal RSize LeftMargin
		{
			get 
			{
				if (Subreport != null)
					return Subreport.Left;

				if (_LeftMargin == null)
					_LeftMargin = new RSize(this, "0 in");
				return  _LeftMargin; 
			}
			set {  _LeftMargin = value; }
		}

		internal RSize RightMargin
		{
			get 
			{ 
				if (Subreport != null)
					return Subreport.OwnerReport.RightMargin;

				if (_RightMargin == null)
					_RightMargin = new RSize(this, "0 in");
				return  _RightMargin; 
			}
			set {  _RightMargin = value; }
		}

		internal RSize TopMargin
		{
			get 
			{ 
				if (Subreport != null)
					return Subreport.OwnerReport.TopMargin;

				if (_TopMargin == null)
					_TopMargin = new RSize(this, "0 in");
				return  _TopMargin; 
			}
			set {  _TopMargin = value; }
		}

		internal float TopOfPage
		{
			get 
			{
				if (this.Subreport != null)
					return Subreport.OwnerReport.TopOfPage;

				float y = TopMargin.Points;
				if (this._PageHeader != null)
					y += _PageHeader.Height.Points;
				return y;
			}
		}

		internal RSize BottomMargin
		{
			get 
			{ 
				if (Subreport != null)
					return Subreport.OwnerReport.BottomMargin;

				if (_BottomMargin == null)
					_BottomMargin = new RSize(this, "0 in");
				return  _BottomMargin; 
			}
			set {  _BottomMargin = value; }
		}
 
		internal float BottomOfPage		// this is the y coordinate just above the page footer
		{
			get 
			{
				if (this.Subreport != null)
					return Subreport.OwnerReport.BottomOfPage;

				// calc size of bottom margin + footer
				float y = BottomMargin.Points;		
				if (this._PageFooter != null)
					y += _PageFooter.Height.Points;

				// now get the absolute coordinate
				y = PageHeight.Points - y;
				return y;
			}
		}

		internal EmbeddedImages EmbeddedImages
		{
			get { return  _EmbeddedImages; }
			set {  _EmbeddedImages = value; }
		}

		internal Expression Language
		{
			get { return  _Language; }
			set {  _Language = value; }
		}

		internal string EvalLanguage(Row r)
		{
			if (_Language == null)
			{
				CultureInfo ci = CultureInfo.CurrentCulture;
				return ci.Name;				
			}

			return _Language.EvaluateString(r);
		}

		internal CodeModules CodeModules
		{
			get { return  _CodeModules; }
			set {  _CodeModules = value; }
		}

		internal Classes Classes
		{
			get { return  _Classes; }
			set {  _Classes = value; }
		}

		internal string DataTransform
		{
			get { return  _DataTransform; }
			set {  _DataTransform = value; }
		}

		internal string DataSchema
		{
			get { return  _DataSchema; }
			set {  _DataSchema = value; }
		}

		internal string DataElementName
		{
			get 
			{
				return _DataElementName == null? "Report": _DataElementName;
			}
			set {  _DataElementName = value; }
		}

		internal DataElementStyleEnum DataElementStyle
		{
			get { return  _DataElementStyle; }
			set {  _DataElementStyle = value; }
		}

		internal IDictionary LUGlobals
		{
			get { return  _LUGlobals; }
		}

		internal IDictionary LUUser
		{
			get { return  _LUUser; }
		}
		
		internal IDictionary LUReportItems
		{
			get { return  _LUReportItems; }
		}
		
		internal IDictionary LUAggrScope
		{
			get { return  _LUAggrScope; }
		}

		internal IDictionary LUReportParameters
		{
			get 
			{
				if (_ReportParameters != null && 
					_ReportParameters.Items != null)
					return  _ReportParameters.Items; 
				else
					return null;
			}
		}

		internal IDictionary LURuntimeName
		{
			get { return _LURuntimeName; }
		}
 

		internal IDictionary LUEmbeddedImages
		{
			get { return _LUEmbeddedImages; }
		}

		internal string CreateRuntimeName(object ro)
		{
			if (this.Subreport != null)
				return Subreport.OwnerReport.CreateRuntimeName(ro);

			_RuntimeName++;					// increment the name generator
			string name = "o" + _RuntimeName.ToString();
			_LURuntimeName.Add(name, ro);
			return name;			
		}

		internal FunctionPageNumber PageNumber
		{
			get { return _PageNumber; }
		}

		internal Subreport Subreport
		{
			get { return _Subreport; }
			set { _Subreport = value; }
		}

		internal FunctionTotalPages TotalPages
		{
			get { return _TotalPages; }
		}
		
		public int ErrorMaxSeverity
		{
			get 
			{
				if (this.rl == null)
					return 0;
				else
					return rl.MaxSeverity;
			}
		}

		public IList ErrorItems
		{
			get
			{
				if (this.rl == null)
					return null;
				else
					return rl.ErrorItems;
			}
		}

		public void ErrorReset()
		{
			if (this.rl == null)
				return;
			rl.Reset();
			return;
		}
	}
}
