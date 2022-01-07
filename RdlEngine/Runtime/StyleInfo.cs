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
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace fyiReporting.RDL
{
	///<summary>
	/// StyleInfo (borders, fonts, background, padding, ...)
	///</summary>
	public class StyleInfo: ICloneable
	{
		// note: all sizes are expressed as points
		// _BorderColor
		/// <summary>
		/// Color of the left border
		/// </summary>
		public Color BColorLeft;		// (Color) Color of the left border
		/// <summary>
		/// Color of the right border
		/// </summary>
		public Color BColorRight;		// (Color) Color of the right border
		/// <summary>
		/// Color of the top border
		/// </summary>
		public Color BColorTop;		// (Color) Color of the top border
		/// <summary>
		/// Color of the bottom border
		/// </summary>
		public Color BColorBottom;	// (Color) Color of the bottom border
		// _BorderStyle
		/// <summary>
		/// Style of the left border
		/// </summary>
		public BorderStyleEnum BStyleLeft;	// (Enum BorderStyle) Style of the left border
		/// <summary>
		/// Style of the left border
		/// </summary>
		public BorderStyleEnum BStyleRight;	// (Enum BorderStyle) Style of the left border
		/// <summary>
		/// Style of the top border
		/// </summary>
		public BorderStyleEnum BStyleTop;		// (Enum BorderStyle) Style of the top border
		/// <summary>
		/// Style of the bottom border
		/// </summary>
		public BorderStyleEnum BStyleBottom;	// (Enum BorderStyle) Style of the bottom border
		// _BorderWdith
		/// <summary>
		/// Width of the left border. Max: 20 pt Min: 0.25 pt
		/// </summary>
		public float BWidthLeft;	//(Size) Width of the left border. Max: 20 pt Min: 0.25 pt
		/// <summary>
		/// Width of the right border. Max: 20 pt Min: 0.25 pt
		/// </summary>
		public float BWidthRight;	//(Size) Width of the right border. Max: 20 pt Min: 0.25 pt
		/// <summary>
		/// Width of the right border. Max: 20 pt Min: 0.25 pt
		/// </summary>
		public float BWidthTop;		//(Size) Width of the right border. Max: 20 pt Min: 0.25 pt
		/// <summary>
		/// Width of the bottom border. Max: 20 pt Min: 0.25 pt
		/// </summary>
		public float BWidthBottom;	//(Size) Width of the bottom border. Max: 20 pt Min: 0.25 pt

		/// <summary>
		/// Color of the background
		/// </summary>
		public Color BackgroundColor;			//(Color) Color of the background
		/// <summary>
		/// The type of background gradient
		/// </summary>
		public BackgroundGradientTypeEnum BackgroundGradientType;	// The type of background gradient
		/// <summary>
		/// End color for the background gradient.
		/// </summary>
		public Color BackgroundGradientEndColor;	//(Color) End color for the background gradient.
		/// <summary>
		/// A background image for the report item.
		/// </summary>
		public PageImage BackgroundImage;	// A background image for the report item.
		/// <summary>
		/// Font style Default: Normal
		/// </summary>
		public FontStyleEnum FontStyle;		// (Enum FontStyle) Font style Default: Normal
		/// <summary>
		/// Name of the font family Default: Arial
		/// </summary>
		private string _FontFamily;			//(string)Name of the font family Default: Arial -- allow comma separated value?
		/// <summary>
		/// Point size of the font
		/// </summary>
		public float FontSize;		//(Size) Point size of the font
		/// <summary>
		/// Thickness of the font
		/// </summary>
		public FontWeightEnum FontWeight;		//(Enum FontWeight) Thickness of the font

		//		Expression _Format;			//(string) .NET Framework formatting string1
		/// <summary>
		/// Special text formatting Default: none
		/// </summary>
		public TextDecorationEnum TextDecoration;	// (Enum TextDecoration) Special text formatting Default: none
		/// <summary>
		/// Horizontal alignment of the text Default: General
		/// </summary>
		public TextAlignEnum TextAlign;		// (Enum TextAlign) Horizontal alignment of the text Default: General
		/// <summary>
		/// Vertical alignment of the text Default: Top
		/// </summary>
		public VerticalAlignEnum VerticalAlign;	// (Enum VerticalAlign)	Vertical alignment of the text Default: Top
		/// <summary>
		/// The foreground color	Default: Black
		/// </summary>
		public Color Color;			// (Color) The foreground color	Default: Black
		/// <summary>
		/// Padding between the left edge of the report item.
		/// </summary>
		public float PaddingLeft;	// (Size)Padding between the left edge of the report item.
		/// <summary>
		/// Padding between the right edge of the report item.
		/// </summary>
		public float PaddingRight;	// (Size) Padding between the right edge of the report item.
		/// <summary>
		/// Padding between the top edge of the report item.
		/// </summary>
		public float PaddingTop;		// (Size) Padding between the top edge of the report item.
		/// <summary>
		/// Padding between the bottom edge of the report item.
		/// </summary>
		public float PaddingBottom;	// (Size) Padding between the bottom edge of the report item.
		/// <summary>
		/// Height of a line of text.
		/// </summary>
		public float LineHeight;		// (Size) Height of a line of text
		/// <summary>
		/// Indicates whether text is written left-to-right (default)
		/// </summary>
		public DirectionEnum Direction;		// (Enum Direction) Indicates whether text is written left-to-right (default)
		/// <summary>
		/// Indicates the writing mode; e.g. left right top bottom or top bottom left right.
		/// </summary>
		public WritingModeEnum WritingMode;	// (Enum WritingMode) Indicates whether text is written
		/// <summary>
		/// The primary language of the text.
		/// </summary>
		public string Language;		// (Language) The primary language of the text.
		/// <summary>
		/// Unused.
		/// </summary>
		public UnicodeBiDirectionalEnum UnicodeBiDirectional;	// (Enum UnicodeBiDirection) 
		/// <summary>
		/// Calendar to use.
		/// </summary>
		public CalendarEnum Calendar;		// (Enum Calendar)
		/// <summary>
		/// The digit format to use.
		/// </summary>
		public string NumeralLanguage;	// (Language) The digit format to use as described by its
		/// <summary>
		/// The variant of the digit format to use.
		/// </summary>
		public int NumeralVariant;	//(Integer) The variant of the digit format to use.

		/// <summary>
		/// Constructor using all defaults for the style.
		/// </summary>
		public StyleInfo()
		{
			BColorLeft = BColorRight = BColorTop = BColorBottom = System.Drawing.Color.Black;	// (Color) Color of the bottom border
			BStyleLeft = BStyleRight = BStyleTop = BStyleBottom = BorderStyleEnum.None;
			// _BorderWdith
			BWidthLeft = BWidthRight = BWidthTop = BWidthBottom = 1;

			BackgroundColor = System.Drawing.Color.Empty;
			BackgroundGradientType = BackgroundGradientTypeEnum.None;
			BackgroundGradientEndColor = System.Drawing.Color.Empty;
			BackgroundImage = null;

			FontStyle = FontStyleEnum.Normal;
			_FontFamily = "Arial";
			FontSize = 10;
			FontWeight = FontWeightEnum.Normal;

			TextDecoration = TextDecorationEnum.None;
			TextAlign = TextAlignEnum.Left;
			VerticalAlign = VerticalAlignEnum.Top;
			Color = System.Drawing.Color.Black;
			PaddingLeft = PaddingRight = PaddingTop = PaddingBottom = 0;
			LineHeight = 0;
			Direction = DirectionEnum.LTR;
			WritingMode = WritingModeEnum.lr_tb;
			Language = "en-US";
			UnicodeBiDirectional = UnicodeBiDirectionalEnum.Normal;
			Calendar = CalendarEnum.Gregorian;
			NumeralLanguage = Language;
			NumeralVariant=1;
		}
		/// <summary>
		/// Name of the font family Default: Arial
		/// </summary>
		public string FontFamily
		{
			get
			{
				int i = _FontFamily.IndexOf(",");
				return i > 0? _FontFamily.Substring(0, i-1): _FontFamily;
			}
			set { _FontFamily = value; }
		}
		/// <summary>
		/// Name of the font family Default: Arial.  Support list of families separated by ','.
		/// </summary>
		public string FontFamilyFull
		{
			get {return _FontFamily;}
		}
		/// <summary>
		/// Gets the FontFamily instance using the FontFamily string.  This supports lists of fonts.
		/// </summary>
		/// <returns></returns>
		public FontFamily GetFontFamily()
		{
			return GetFontFamily(_FontFamily);
		}

		/// <summary>
		/// Gets the FontFamily instance using the passed face name.  This supports lists of fonts.
		/// </summary>
		/// <returns></returns>
		static public FontFamily GetFontFamily(string fface)
		{
			string[] choices = fface.Split(',');
			FontFamily ff=null;
			foreach (string val in choices)
			{
				try 
				{
					string font=null;
					// TODO: should be better way than to hard code; could put in config file??
					switch (val.Trim())
					{
						case "serif":
							font = "Times New Roman";
							break;
						case "sans-serif":
							font = "Arial";
							break;
						case "cursive":
							font = "Comic Sans MS";
							break;
						case "fantasy":
							font = "Impact";
							break;
						case "monospace":
							font = "Courier New";
							break;
						default:
							font = val;
							break;
					}
					ff = new FontFamily(font);
					if (ff != null)
						break;
				}
				catch {}	// if font doesn't exist we will go to the next
			}
			if (ff == null)
				ff = new FontFamily("Arial");
			return ff;
		}
		/// <summary>
		/// True if font is bold.
		/// </summary>
		/// <returns></returns>
		public bool IsFontBold()
		{
			switch(FontWeight)
			{
				case FontWeightEnum.Bold:
				case FontWeightEnum.Bolder:
				case FontWeightEnum.W500:
				case FontWeightEnum.W600:
				case FontWeightEnum.W700:
				case FontWeightEnum.W800:
				case FontWeightEnum.W900:
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Gets the enumerated font weight.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		static public FontWeightEnum GetFontWeight(string v, FontWeightEnum def)
		{
			FontWeightEnum fw;

			switch(v.ToLower())
			{
				case "Lighter":
					fw = FontWeightEnum.Lighter;
					break;
				case "Normal":
					fw = FontWeightEnum.Normal;
					break;
				case "bold":
					fw = FontWeightEnum.Bold;
					break;
				case "bolder":
					fw = FontWeightEnum.Bolder;
					break;
				case "500":
					fw = FontWeightEnum.W500;
					break;
				case "600":
					fw = FontWeightEnum.W600;
					break;
				case "700":
					fw = FontWeightEnum.W700;
					break;
				case "800":
					fw = FontWeightEnum.W800;
					break;
				case "900":
					fw = FontWeightEnum.W900;
					break;
				default:
					fw = def;
					break;
			}
			return fw;
		}

		/// <summary>
		/// Returns the font style (normal or italic).
		/// </summary>
		/// <param name="v"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		public static FontStyleEnum GetFontStyle(string v, FontStyleEnum def)
		{
			FontStyleEnum f;
			switch (v.ToLower())
			{
				case "normal":
					f = FontStyleEnum.Normal;
					break;
				case "italic":
					f = FontStyleEnum.Italic;
					break;
				default:
					f = def;
					break;
			}
			return f;
		}

		/// <summary>
		/// Gets the background gradient type.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		static public BackgroundGradientTypeEnum GetBackgroundGradientType(string v, BackgroundGradientTypeEnum def)
		{
			BackgroundGradientTypeEnum gt;
			switch(v.ToLower())
			{
				case "none":
					gt = BackgroundGradientTypeEnum.None;
					break;
				case "leftright":
					gt = BackgroundGradientTypeEnum.LeftRight;
					break;
				case "topbottom":
					gt = BackgroundGradientTypeEnum.TopBottom;
					break;
				case "center":
					gt = BackgroundGradientTypeEnum.Center;
					break;
				case "diagonalleft":
					gt = BackgroundGradientTypeEnum.DiagonalLeft;
					break;
				case "diagonalright":
					gt = BackgroundGradientTypeEnum.DiagonalRight;
					break;
				case "horizontalcenter":
					gt = BackgroundGradientTypeEnum.HorizontalCenter;
					break;
				case "verticalcenter":
					gt = BackgroundGradientTypeEnum.VerticalCenter;
					break;
				default:
					gt = def;
					break;
			}
			return gt;
		}

		/// <summary>
		/// Gets the text decoration.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		public static TextDecorationEnum GetTextDecoration(string v, TextDecorationEnum def)
		{
			TextDecorationEnum td;
			switch (v.ToLower())
			{
				case "underline":
					td = TextDecorationEnum.Underline;
					break;
				case "overline":
					td = TextDecorationEnum.Overline;
					break;
				case "linethrough":
					td = TextDecorationEnum.LineThrough;
					break;
				case "none":
					td = TextDecorationEnum.None;
					break;
				default:
					td = def;
					break;
			}
			return td;
		}

		/// <summary>
		/// Gets the text alignment.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		public static TextAlignEnum GetTextAlign(string v, TextAlignEnum def)
		{
			TextAlignEnum ta;
			switch(v.ToLower())
			{
				case "left":
					ta = TextAlignEnum.Left;
					break;
				case "right":
					ta = TextAlignEnum.Right;
					break;
				case "center":
					ta = TextAlignEnum.Center;
					break;
				case "general":
					ta = TextAlignEnum.General;
					break;
				default:
					ta = def;
					break;
			}
			return ta;
		}

		/// <summary>
		/// Gets the vertical alignment.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		public static VerticalAlignEnum GetVerticalAlign(string v, VerticalAlignEnum def)
		{
			VerticalAlignEnum va;
			switch (v.ToLower())
			{
				case "top":
					va = VerticalAlignEnum.Top;
					break;
				case "middle":
					va = VerticalAlignEnum.Middle;
					break;
				case "bottom":
					va = VerticalAlignEnum.Bottom;
					break;
				default:
					va = def;
					break;
			}
			return va;
		}

		/// <summary>
		/// Gets the direction of the text.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		public static DirectionEnum GetDirection(string v, DirectionEnum def)
		{
			DirectionEnum d;   
			switch(v.ToLower())
			{
				case "ltr":
					d = DirectionEnum.LTR;
					break;
				case "rtl":
					d = DirectionEnum.RTL;
					break;
				default:
					d = def;
					break;
			}
			return d;
		}
		/// <summary>
		/// Gets the writing mode; e.g. left right top bottom or top bottom left right.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		public static WritingModeEnum GetWritingMode(string v, WritingModeEnum def)
		{
			WritingModeEnum w;
			switch(v.ToLower())
			{
				case "lr-tb":
					w = WritingModeEnum.lr_tb;
					break;
				case "tb-rl":
					w = WritingModeEnum.tb_rl;
					break;
				default:
					w = def;
					break;
			}
			return w;
		}

		/// <summary>
		/// Gets the unicode BiDirectional.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		public static UnicodeBiDirectionalEnum GetUnicodeBiDirectional(string v, UnicodeBiDirectionalEnum def)
		{
			UnicodeBiDirectionalEnum u;
			switch (v.ToLower())
			{
				case "normal":
					u = UnicodeBiDirectionalEnum.Normal;
					break;
				case "embed":
					u = UnicodeBiDirectionalEnum.Embed;
					break;
				case "bidi-override":
					u = UnicodeBiDirectionalEnum.BiDi_Override;
					break;
				default:
					u = def;
					break;
			}
			return u;
		}

		/// <summary>
		/// Gets the calendar (e.g. Gregorian, GregorianArabic, and so on)
		/// </summary>
		/// <param name="v"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		public static CalendarEnum GetCalendar(string v, CalendarEnum def)
		{
			CalendarEnum c;

			switch (v.ToLower())
			{
				case "gregorian":
					c = CalendarEnum.Gregorian;
					break;
				case "gregorianarabic":
					c = CalendarEnum.GregorianArabic;
					break;
				case "gregorianmiddleeastfrench":
					c = CalendarEnum.GregorianMiddleEastFrench;
					break;
				case "gregoriantransliteratedenglish":
					c = CalendarEnum.GregorianTransliteratedEnglish;
					break;
				case "gregoriantransliteratedfrench":
					c = CalendarEnum.GregorianTransliteratedFrench;
					break;
				case "gregorianusenglish":
					c = CalendarEnum.GregorianUSEnglish;
					break;
				case "hebrew":
					c = CalendarEnum.Hebrew;
					break;
				case "hijri":
					c = CalendarEnum.Hijri;
					break;
				case "japanese":
					c = CalendarEnum.Japanese;
					break;
				case "korea":
					c = CalendarEnum.Korea;
					break;
				case "taiwan":
					c = CalendarEnum.Taiwan;
					break;
				case "thaibuddhist":
					c = CalendarEnum.ThaiBuddhist;
					break;
				default:
					c = def;
					break;
			}
			return c;

		}
		#region ICloneable Members

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		#endregion
	}

	/// <summary>
	/// The types of background gradients supported.
	/// </summary>
	public enum BackgroundGradientTypeEnum
	{
		/// <summary>
		/// No gradient
		/// </summary>
		None,
		/// <summary>
		/// Left Right gradient
		/// </summary>
		LeftRight,
		/// <summary>
		/// Top Bottom gradient
		/// </summary>
		TopBottom,
		/// <summary>
		/// Center gradient
		/// </summary>
		Center,
		/// <summary>
		/// Diagonal Left gradient
		/// </summary>
		DiagonalLeft,
		/// <summary>
		/// Diagonal Right gradient
		/// </summary>
		DiagonalRight,
		/// <summary>
		/// Horizontal Center gradient
		/// </summary>
		HorizontalCenter,
		/// <summary>
		/// Vertical Center
		/// </summary>
		VerticalCenter
	}
	/// <summary>
	/// Font styles supported
	/// </summary>
	public enum FontStyleEnum
	{
		/// <summary>
		/// Normal font
		/// </summary>
		Normal,
		/// <summary>
		/// Italic font
		/// </summary>
		Italic
	}

	/// <summary>
	/// Potential font weights
	/// </summary>
	public enum FontWeightEnum
	{
		/// <summary>
		/// Lighter font
		/// </summary>
		Lighter,
		/// <summary>
		/// Normal font
		/// </summary>
		Normal,
		/// <summary>
		/// Bold font
		/// </summary>
		Bold,
		/// <summary>
		/// Bolder font
		/// </summary>
		Bolder,
		/// <summary>
		/// W100 font
		/// </summary>
		W100,
		/// <summary>
		/// W200 font
		/// </summary>
		W200,
		/// <summary>
		/// W300 font
		/// </summary>
		W300,
		/// <summary>
		/// W400 font
		/// </summary>
		W400,
		/// <summary>
		/// W500 font
		/// </summary>
		W500,
		/// <summary>
		/// W600 font
		/// </summary>
		W600,
		/// <summary>
		/// W700 font
		/// </summary>
		W700,
		/// <summary>
		/// W800 font
		/// </summary>
		W800,
		/// <summary>
		/// W900 font
		/// </summary>
		W900
	}

	public enum TextDecorationEnum
	{
		Underline,
		Overline,
		LineThrough,
		None
	}

	public enum TextAlignEnum
	{
		Left,
		Center,
		Right,
		General
	}

	public enum VerticalAlignEnum
	{
		Top,
		Middle,
		Bottom
	}

	public enum DirectionEnum
	{
		LTR,				// left to right
		RTL					// right to left
	}

	public enum WritingModeEnum
	{
		lr_tb,				// left right - top bottom
		tb_rl				// top bottom - right left
	}

	public enum UnicodeBiDirectionalEnum
	{
		Normal,
		Embed,
		BiDi_Override
	}
		
	public enum CalendarEnum
	{
		Gregorian,
		GregorianArabic,
		GregorianMiddleEastFrench,
		GregorianTransliteratedEnglish,
		GregorianTransliteratedFrench,
		GregorianUSEnglish,
		Hebrew,
		Hijri,
		Japanese,
		Korea,
		Taiwan,
		ThaiBuddhist
	}
}
