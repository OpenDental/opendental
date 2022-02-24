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
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace fyiReporting.RDL
{
	///<summary>
	/// StyleInfo (borders, fonts, background, padding, ...)
	///</summary>
	public class StyleInfo
	{
		// note: all sizes are expressed as points
		// _BorderColor
		public Color BColorLeft;		// (Color) Color of the left border
		public Color BColorRight;		// (Color) Color of the right border
		public Color BColorTop;		// (Color) Color of the top border
		public Color BColorBottom;	// (Color) Color of the bottom border
		// _BorderStyle
		public BorderStyleEnum BStyleLeft;	// (Enum BorderStyle) Style of the left border
		public BorderStyleEnum BStyleRight;	// (Enum BorderStyle) Style of the right border
		public BorderStyleEnum BStyleTop;		// (Enum BorderStyle) Style of the top border
		public BorderStyleEnum BStyleBottom;	// (Enum BorderStyle) Style of the bottom border
		// _BorderWdith
		public float BWidthLeft;	//(Size) Width of the left border. Max: 20 pt Min: 0.25 pt
		public float BWidthRight;	//(Size) Width of the right border. Max: 20 pt Min: 0.25 pt
		public float BWidthTop;		//(Size) Width of the top border. Max: 20 pt Min: 0.25 pt
		public float BWidthBottom;	//(Size) Width of the bottom border. Max: 20 pt Min: 0.25 pt

		public Color BackgroundColor;			//(Color) Color of the background
		public BackgroundGradientTypeEnum BackgroundGradientType;	// The type of background gradient
		public Color BackgroundGradientEndColor;	//(Color) End color for the background gradient. If
		public PageImage BackgroundImage;	// A background image for the report item.
		public FontStyleEnum FontStyle;		// (Enum FontStyle) Font style Default: Normal
		public string FontFamily;		//(string)Name of the font family Default: Arial
		public float FontSize;		//(Size) Point size of the font
		public FontWeightEnum FontWeight;		//(Enum FontWeight) Thickness of the font
		//		Expression _Format;			//(string) .NET Framework formatting string1
		public TextDecorationEnum TextDecoration;	// (Enum TextDecoration) Special text formatting Default: none
		public TextAlignEnum TextAlign;		// (Enum TextAlign) Horizontal alignment of the text Default: General
		public VerticalAlignEnum VerticalAlign;	// (Enum VerticalAlign)	Vertical alignment of the text Default: Top
		public Color Color;			// (Color) The foreground color	Default: Black
		public float PaddingLeft;	// (Size)Padding between the left edge of the
		public float PaddingRight;	// (Size) Padding between the right edge of the
		public float PaddingTop;		// (Size) Padding between the top edge of the
		public float PaddingBottom;	// (Size) Padding between the top edge of the
		public float LineHeight;		// (Size) Height of a line of text
		public DirectionEnum Direction;		// (Enum Direction) Indicates whether text is written left-to-right (default)
		public WritingModeEnum WritingMode;	// (Enum WritingMode) Indicates whether text is written
		public string Language;		// (Language) The primary language of the text.
		public UnicodeBiDirectionalEnum UnicodeBiDirectional;	// (Enum UnicodeBiDirection) 
		public CalendarEnum Calendar;		// (Enum Calendar)
		public string NumeralLanguage;	// (Language) The digit format to use as described by its
		public int NumeralVariant;	//(Integer) The variant of the digit format to use.

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
			FontFamily = "Arial";
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
	}

	public enum BackgroundGradientTypeEnum
	{
		None,
		LeftRight,
		TopBottom,
		Center,
		DiagonalLeft,
		DiagonalRight,
		HorizontalCenter,
		VerticalCenter
	}
	
	public enum FontStyleEnum
	{
		Normal,
		Italic
	}

	public enum FontWeightEnum
	{
		Lighter,
		Normal,
		Bold,
		Bolder,
		W100,
		W200,
		W300,
		W400,
		W500,
		W600,
		W700,
		W800,
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
