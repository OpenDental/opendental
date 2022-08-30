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
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.VisualBasic;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace fyiReporting.RDL
{
	///<summary>
	/// Code represents the Code report element. 
	///</summary>
	[Serializable]
	internal class Code : ReportLink
	{
		string _Source;			// The source code
		string _Classname;		// Class name of generated class
		Assembly _Assembly;		// the compiled assembly
	
		internal Code(ReportDefn r, ReportLink p, XmlNode xNode) : base(r, p)
		{
			_Source=xNode.InnerText;
			_Assembly = GetAssembly();
		}
		
		override internal void FinalPass()
		{
			return;
		}

		private Assembly GetAssembly()
		{
			// Generate the proxy source code
            List<string> lines = new List<string>();		// hold lines in array in case of error

			VBCodeProvider vbcp =  new VBCodeProvider();
			StringBuilder sb = new StringBuilder();
			//  Generate code with the following general form

			//Imports System
			//Namespace fyiReportingvbgen
			//Public Class MyClassn	   // where n is a uniquely generated integer
			//Sub New()
			//End Sub
			//  ' this is the code in the <Code> tag
			//End Class
			//End Namespace
			string unique = Interlocked.Increment(ref Parser.Counter).ToString();
			lines.Add("Imports System");
			lines.Add("Namespace fyiReporting.vbgen");
			_Classname = "MyClass" + unique;
			lines.Add("Public Class " + _Classname);
			lines.Add("Sub New()");
			lines.Add("End Sub");
			// Read and write code as lines
			StringReader tr = new StringReader(_Source);
			while (tr.Peek() >= 0)
			{
				string line = tr.ReadLine();
				lines.Add(line);
			}
			tr.Close();
			lines.Add("End Class");
			lines.Add("End Namespace");
			foreach (string l in lines)
				sb.AppendFormat(l + "\r\n");

			string vbcode = sb.ToString();

			// debug code !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//						StreamWriter tsw = File.CreateText(@"c:\temp\vbcode.txt");
//						tsw.Write(vbcode);
//						tsw.Close();
			// debug code !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!   

			// Create Assembly
			CompilerParameters cp = new CompilerParameters();
			cp.ReferencedAssemblies.Add("System.dll");
			cp.GenerateExecutable = false;
			cp.GenerateInMemory = false;			// just loading into memory causes problems when instantiating
			cp.IncludeDebugInformation = false; 
			CompilerResults cr = vbcp.CompileAssemblyFromSource(cp, vbcode);
			if(cr.Errors.Count > 0)
			{
				StringBuilder err = new StringBuilder(string.Format("Code element has {0} error(s).  Line numbers are relative to Code element.", cr.Errors.Count));
				foreach (CompilerError ce in cr.Errors)
				{
					string l;
					if (ce.Line >= 1 && ce.Line <= lines.Count)
						l = lines[ce.Line - 1] as string;
					else
						l = "Unknown";
					err.AppendFormat("\r\nLine {0} '{1}' : {2} {3}", ce.Line - 5, l, ce.ErrorNumber, ce.ErrorText);
				}
				this.OwnerReport.rl.LogError(4, err.ToString());
				return null;
			}

			return Assembly.LoadFrom(cr.PathToAssembly);	// We need an assembly loaded from the file system
			//   or instantiation of object complains
		}

		internal Type CodeType()
		{
			if (_Assembly == null)
				return null;

			Type t=null;
			try
			{
				object instance = _Assembly.CreateInstance("fyiReporting.vbgen." + this._Classname, false); 
				t = instance.GetType();
			}
			catch (Exception e)
			{
				OwnerReport.rl.LogError(4, 
					string.Format("Unable to load instance of Code\r\n{0}", e.Message));
			}
			return t;
		}

		internal object Load(Report rpt)
		{
			WorkClass wc = GetWC(rpt);
			if (wc.bCreateFailed)		// We only try to create once.
				return wc.Instance;

			if (wc.Instance != null)	// Already loaded
				return wc.Instance;

			if (_Assembly == null)
			{
				wc.bCreateFailed = true;	// we don't have an assembly
				return null;
			}

			// Load an instance of the object
			string err="";
			try
			{
				wc.Instance = _Assembly.CreateInstance("fyiReporting.vbgen." + this._Classname, false); 
			}
			catch (Exception e)
			{
				wc.Instance = null;
				err = e.Message;
			}

			if (wc.Instance == null)
			{
				string e = String.Format("Unable to create instance of local code class.\r\n{0}", err);
				if (rpt == null)
					OwnerReport.rl.LogError(4, e);
				else
					rpt.rl.LogError(4, e);
				wc.bCreateFailed = true;
			}
			return wc.Instance;			
		}

		internal string Source
		{
			get { return  _Source; }
		}

		internal object Instance(Report rpt)
		{
			return Load(rpt);			// load if necessary
		}

		private WorkClass GetWC(Report rpt)
		{
			if (rpt == null)
				return new WorkClass();

			WorkClass wc = rpt.Cache.Get(this, "wc") as WorkClass;
			if (wc == null)
			{
				wc = new WorkClass();
				rpt.Cache.Add(this, "wc", wc);
			}
			return wc;
		}

		private void RemoveWC(Report rpt)
		{
			rpt.Cache.Remove(this, "wc");
		}

		class WorkClass
		{
			internal object Instance;
			internal bool bCreateFailed;
			internal WorkClass()
			{
				Instance=null;	// 
				bCreateFailed=false;
			}
		}
	}
}
