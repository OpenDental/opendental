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


namespace fyiReporting.RDL
{
	///<summary>
	/// Filter operators
	///</summary>
	internal enum DataElementOutputEnum
	{
		Output,		// Indicates the item should appear in the output
		NoOutput,	// Indicates the item should not appear in the output
		ContentsOnly,	// Indicates the item should not appear in the XML, but its contents should be
						// rendered as if they were in this item’s
						// container. Only applies to Lists.
		Auto		// (Default): Will behave as NoOutput for
					// Textboxes with constant values,
					// ContentsOnly for Rectangles and Output for
					// all other items		

	}

	internal class DataElementOutput
	{
		static internal DataElementOutputEnum GetStyle(string s, ReportLog rl)
		{
			DataElementOutputEnum rs;

			switch (s)
			{		
				case "Output":
					rs = DataElementOutputEnum.Output;
					break;
				case "NoOutput":
					rs = DataElementOutputEnum.NoOutput;
					break;
				case "ContentsOnly":
					rs = DataElementOutputEnum.ContentsOnly;
					break;
				case "Auto":
					rs = DataElementOutputEnum.Auto;
					break;
				default:		
					rl.LogError(4, "Unknown DataElementOutput '" + s + "'.  Auto assumed.");
					rs = DataElementOutputEnum.Auto;
					break;
			}
			return rs;
		}
	}

}
