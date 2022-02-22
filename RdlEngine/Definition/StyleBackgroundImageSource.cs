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
	/// Style Background image source enumeration
	///</summary>

	internal enum StyleBackgroundImageSourceEnum
	{
		External,		// The Value contains a constant or
		// expression that evaluates to for the location
		// of the image
		Embedded,		// The Value contains a constant
		// or expression that evaluates to the name of
		// an EmbeddedImage within the report
		Database,		// The Value contains an expression
		// (a field in the database) that evaluates to the
		// binary data for the image.
		Unknown			// Illegal (or no) value specified
	}

	internal class StyleBackgroundImageSource
	{
		static internal StyleBackgroundImageSourceEnum GetStyle(string s)
		{
			StyleBackgroundImageSourceEnum rs;

			switch (s)
			{		
				case "External":
					rs = StyleBackgroundImageSourceEnum.External;
					break;
				case "Embedded":
					rs = StyleBackgroundImageSourceEnum.Embedded;
					break;
				case "Database":
					rs = StyleBackgroundImageSourceEnum.Database;
					break;
				default:		// user error just force to normal TODO
					rs = StyleBackgroundImageSourceEnum.External;
					break;
			}
			return rs;
		}
	}

}
