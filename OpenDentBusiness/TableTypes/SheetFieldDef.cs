using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Newtonsoft.Json;
using PdfSharp.Drawing;
using CodeBase;

namespace OpenDentBusiness{
	///<summary>One field on a sheetDef.</summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class SheetFieldDef:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SheetFieldDefNum;
		///<summary>FK to sheetdef.SheetDefNum.</summary>
		public long SheetDefNum;
		///<summary>Enum:SheetFieldType  OutputText, InputField, StaticText,Parameter(only used for SheetField, not SheetFieldDef),Image,Drawing,Line,Rectangle,CheckBox,SigBox,PatImage.</summary>
		public SheetFieldType FieldType;
		/// <summary>FieldName is used differently for different FieldTypes. 
		/// <para>For OutputText, each sheet typically has a main datatable type. For example statements correspond to the statment table. See SheetFieldsAvailable.GetList() for available values.</para>
		/// <para>     If the output field exactly matches a column from the main table this will be the &lt;ColumnName>. For example, "FName" on patient Forms.</para>
		/// <para>     If the output field exactly matches a column from a different table this will be the &lt;tablename>.&lt;ColumnName>. For example, appt.Note on Routing Slips.</para>
		/// <para>     If the output field is not a database column it must start with a lowercase letter. For example, "statementReceiptInvoice" on Statements.</para>
		/// <para>For InputField, these are hardcoded to correspond to DB fields, for example "FName" corresponsds to patient.FName. See SheetFieldsAvailable.GetList() for available values.</para>
		/// <para>For Image, this file name with extention, for example "image1.jpg". Some image names are handled specially, for example "Patient Info.gif". Images are stored in &lt;imagefolder>\SheetImages\image1.jpg.</para>
		/// <para>For CheckBox, this groups checkboxes together so that only one per group can be checked.</para>
		/// <para>For PatImage, this is the name of the DocCategory.</para>
		/// <para>For Special, identifies the type of special field. Currently only ToothChart and ToothChartLegend.</para>
		/// <para>For Grid, this is the specific type of grid. See SheetUtil.GetDataTableForGridType() for values. For example "StatementPayPlan".</para>
		/// <para>For all other fieldtypes, FieldName is blank or irrelevant.</para></summary>
		public string FieldName; //note, the indenting in the summary above is a non-breaking space. Alt + 255.
		///<summary>For StaticText, this text can include bracketed fields, like [nameLF].
		///<para>For OutputText and InputField, this will be blank.  </para>
		///<para>For CheckBoxes, either X or blank.  Even if the checkbox is set to behave like a radio button.  </para>
		///<para>For Pat Images, this is blank.  The filename of a PatImage will later be stored in SheetField.FieldValue.</para>
		///<para>For ComboBoxes, the chosen option, semicolon, then a pipe delimited list of options such as: March;January|February|March|April</para>
		///<para>For ScreenCharts, a semicolon delimited list of comma separated surfaces.  It may look like S,P,N;S,S,S;... etc.</para></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string FieldValue;
		///<summary>The fontSize for this field regardless of the default for the sheet.  The actual font must be saved with each sheetField.</summary>
		public float FontSize;
		///<summary>The fontName for this field regardless of the default for the sheet.  The actual font must be saved with each sheetField.</summary>
		public string FontName;
		///<summary>.</summary>
		public bool FontIsBold;
		///<summary>In pixels.</summary>
		public int XPos;
		///<summary>In pixels.</summary>
		public int YPos;
		///<summary>The field will be constrained horizontally to this size.  Not allowed to be zero.
		///When SheetType is associated to a dynamic layout def and GrowthBehavior is set to a dynamic value this value represents the corresponding controls minimum width.</summary>
		public int Width;
		///<summary>The field will be constrained vertically to this size.  Not allowed to be 0.  It's not allowed to be zero so that it will be visible on the designer.
		///When SheetType is associated to a dynamic layout def and GrowthBehavior is set to a dynamic value this value represents the corresponding controls minimum height.</summary>
		public int Height;
		///<summary>Enum:GrowthBehaviorEnum</summary>
		public GrowthBehaviorEnum GrowthBehavior;
		///<summary>This is only used for checkboxes that you want to behave like radiobuttons.  Set the FieldName the same for each Checkbox in the group.  The FieldValue will likely be X for one of them and empty string for the others.  Each of them will have a different RadioButtonValue.  Whichever box has X, the RadioButtonValue for that box will be used when importing.  This field is not used for "misc" radiobutton groups.</summary>
		public string RadioButtonValue;
		///<summary>Name which identifies the group within which the radio button belongs. FieldName must be set to "misc" in order for the group to take effect.</summary>
		public string RadioButtonGroup;
		///<summary>Set to true if this field is required to have a value before the sheet is closed.</summary>
		public bool IsRequired;
		///<summary>The Bitmap should be converted to Base64 using POut.Bitmap() before placing in this field.  Not stored in the database.  Only used when uploading SheetDefs to the web server.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string ImageData;
		///<summary>Tab stop order for all fields. One-based.  Only checkboxes and input fields can have values other than 0.</summary>
		public int TabOrder;
		///<summary>Allows reporting on misc fields.</summary>
		public string ReportableName;
		///<summary>Text Alignment for text fields.</summary>
		public HorizontalAlignment TextAlign;
		///<summary>Used to determine if the field should be hidden when printing statements.</summary>
		public bool IsPaymentOption;
		///<summary>If a sheet field is locked, it stops a user from editing the text when presented in a sheet.</summary>
		public bool IsLocked;
		///<summary>Text color, line color, rectangle color.</summary>
		[XmlIgnore]
		public Color ItemColor;
		///<summary>Tab stop order for all fields of a mobile sheet. One-based.  Only mobile fields can have values other than 0. If all SheetFieldDefs for a given SheetField are 0 then assume that this sheet has no mobile-specific view.</summary>
		public int TabOrderMobile;
		///<summary>Each input field for a mobile will need a corresponding UI label. This is what the user sees as the label describing what this input is for. EG "First Name:, Last Name:, Address, etc."</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string UiLabelMobile;
		///<summary>Human readable label that will be displayed for radio button or checkbox item in mobile mode.  Cannot use UiLabelMobile for this purpose as it is already dedicated to the radio group header that groups radio button items together.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string UiLabelMobileRadioButton;
		///<summary>Enum:SheetFieldLayoutMode Used when there are multiple modes for associated sheetDef, like chart module modes (base,treatment plan,ecw,orion,...).  SheetFieldLayoutMode.Default for all sheetdefs that do not have multiple modes.</summary>
		public SheetFieldLayoutMode LayoutMode;
		///<summary>Blank by default. When set patient.Language will attempt to match to SheetFieldDefs with a matching Language value.</summary>
		public string Language;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ItemColor",typeof(int))]
		public int ItemColorXml {
			get {
				return ItemColor.ToArgb();
			}
			set {
				ItemColor=Color.FromArgb(value);
			}
		}

		///<summary>A convenient place to store the image of a field so that it does not have to be repeatedly downloaded from the cloud. Be sure to dispose this field to prevent resource leaks.</summary>
		[XmlIgnore]
		[JsonIgnore]
		[CrudColumn(IsNotDbColumn=true)]
		public Image ImageField;

		public SheetFieldDef(){//required for use as a generic.
			RadioButtonGroup="";
			ImageData="";
		}

		///<summary>DEPRECATED. Simple constructor, useful in conjunction with initializer.</summary>
		public SheetFieldDef(SheetFieldType fieldType,int x,int y,int width,int height,Font font=null,string fieldValue="") {
			this.RadioButtonGroup="";
			this.ImageData="";
			this.FieldType=fieldType;
			this.XPos=x;
			this.YPos=y;
			this.Width=width;
			this.Height=height;
			if(font!=null) {
				this.FontName=font.Name;
				this.FontSize=font.SizeInPoints;
				this.FontIsBold=font.Bold;
			}
			this.FieldValue=fieldValue;
			this.ItemColor=Color.Black;
		}

		private SheetFieldDef(SheetFieldType fieldType,string fieldName,string fieldValue,
			float fontSize,string fontName,bool fontIsBold,
			int xPos,int yPos,int width,int height,
			GrowthBehaviorEnum growthBehavior,string radioButtonValue,bool isPaymentOption=false,KnownColor itemColor=KnownColor.Black,HorizontalAlignment textAlign=HorizontalAlignment.Left,bool isNew=false,string language="") 
		{
			FieldType=fieldType;
			FieldName=fieldName;
			FieldValue=fieldValue;
			FontSize=fontSize;
			FontName=fontName;
			FontIsBold=fontIsBold;
			XPos=xPos;
			YPos=yPos;
			Width=width;
			Height=height;
			GrowthBehavior=growthBehavior;
			RadioButtonValue=radioButtonValue;
			IsNew=isNew;
			IsPaymentOption=isPaymentOption;
			ItemColor=Color.FromKnownColor(itemColor);
			TextAlign=textAlign;
			RadioButtonGroup="";
			ImageData="";
			ReportableName="";
			UiLabelMobile="";
			UiLabelMobileRadioButton="";
			Language=language;
		}

		public SheetFieldDef Copy(){
			//Not copying ImageField in order to reduce the likelihood of memory leaks.
			return (SheetFieldDef)this.MemberwiseClone();
		}

		public override string ToString() {
			return FieldName+" "+FieldValue;
		}

		///<Summary></Summary>
		public Font GetFont(){
			FontStyle style=FontStyle.Regular;
			if(FontIsBold){
				style=FontStyle.Bold;
			}
			return new Font(FontName,FontSize,style);
		}

		//public static SheetFieldDef NewOutput(string fieldName,float fontSize,string fontName,bool fontIsBold,
		//	int xPos,int yPos,int width,int height)
		//{
		//	return new SheetFieldDef(SheetFieldType.OutputText,fieldName,"",fontSize,fontName,fontIsBold,
		//		xPos,yPos,width,height,GrowthBehaviorEnum.None,"");
		//}

		public static SheetFieldDef NewOutput(string fieldName,float fontSize,string fontName,bool fontIsBold,
			int xPos,int yPos,int width,int height,GrowthBehaviorEnum growthBehavior=GrowthBehaviorEnum.None,KnownColor itemColor=KnownColor.Black)
		{
			return new SheetFieldDef(SheetFieldType.OutputText,fieldName,"",fontSize,fontName,fontIsBold,
				xPos,yPos,width,height,growthBehavior,"",false,itemColor,isNew:true);
		}

		public static SheetFieldDef NewOutput(string fieldName,float fontSize,string fontName,bool fontIsBold,
			int xPos,int yPos,int width,int height,HorizontalAlignment textAlign, KnownColor itemColor=KnownColor.Black,
			GrowthBehaviorEnum growthBehavior=GrowthBehaviorEnum.None)
		{
			return new SheetFieldDef(SheetFieldType.OutputText,fieldName,"",fontSize,fontName,fontIsBold,
				xPos,yPos,width,height,growthBehavior,"",false,itemColor,textAlign,isNew:true);
		}

		//public static SheetFieldDef NewStaticText(string fieldValue,float fontSize,string fontName,bool fontIsBold,
		//	int xPos,int yPos,int width,int height) {
		//	return new SheetFieldDef(SheetFieldType.StaticText,"",fieldValue,fontSize,fontName,fontIsBold,
		//		xPos,yPos,width,height,GrowthBehaviorEnum.None,"");
		//}

		///<summary>Use named parameters if you only need to use some of the optional parameters.</summary>
		public static SheetFieldDef NewStaticText(string fieldValue,float fontSize,string fontName,bool fontIsBold,
			int xPos,int yPos,int width,int height,GrowthBehaviorEnum growthBehavior=GrowthBehaviorEnum.None,bool isPaymentOption=false,KnownColor itemColor=KnownColor.Black,
			HorizontalAlignment textAlign=HorizontalAlignment.Left) 
		{
			return new SheetFieldDef(SheetFieldType.StaticText,"",fieldValue,fontSize,fontName,fontIsBold,
				xPos,yPos,width,height,growthBehavior,"",isPaymentOption,itemColor,textAlign,isNew:true);
		}

		public static SheetFieldDef NewInput(string fieldName,float fontSize,string fontName,bool fontIsBold,
			int xPos,int yPos,int width,int height)
		{
			return new SheetFieldDef(SheetFieldType.InputField,fieldName,"",fontSize,fontName,fontIsBold,
				xPos,yPos,width,height,GrowthBehaviorEnum.None,"",isNew:true);
		}

		public static SheetFieldDef NewImage(string fileName,int xPos,int yPos,int width,int height) {
			return new SheetFieldDef(SheetFieldType.Image,fileName,"",0,"",false,
				xPos,yPos,width,height,GrowthBehaviorEnum.None,"",isNew:true);
		}

		public static SheetFieldDef NewPatImage(int xPos,int yPos,int width,int height) {
			return new SheetFieldDef(SheetFieldType.PatImage,"","",0,"",false,
				xPos,yPos,width,height,GrowthBehaviorEnum.None,"",isNew:true);
		}

		public static SheetFieldDef NewLine(int xPos,int yPos,int width,int height,bool isPaymentOption=false,KnownColor itemColor=KnownColor.Black) {
			return new SheetFieldDef(SheetFieldType.Line,"","",0,"",false,
				xPos,yPos,width,height,GrowthBehaviorEnum.None,"",isPaymentOption,itemColor,isNew:true);
		}

		public static SheetFieldDef NewRect(int xPos,int yPos,int width,int height) {
			return new SheetFieldDef(SheetFieldType.Rectangle,"","",0,"",false,
				xPos,yPos,width,height,GrowthBehaviorEnum.None,"",isNew:true);
		}

		public static SheetFieldDef NewCheckBox(string fieldName,int xPos,int yPos,int width,int height) {
			return new SheetFieldDef(SheetFieldType.CheckBox,fieldName,"",0,"",false,
				xPos,yPos,width,height,GrowthBehaviorEnum.None,"",isNew:true);
		}

		public static SheetFieldDef NewComboBox(string fieldName,string fieldValue,int xPos,int yPos) {
			return new SheetFieldDef(SheetFieldType.ComboBox,fieldName,fieldValue,0,"",false,
				xPos,yPos,155,19,GrowthBehaviorEnum.None,"",isNew:true);
		}

		public static SheetFieldDef NewScreenChart(string fieldName,string fieldValue,int xPos,int yPos) {
			return new SheetFieldDef(SheetFieldType.ScreenChart,fieldName,fieldValue,0,"",false,
				xPos,yPos,731,128,GrowthBehaviorEnum.None,"",isNew:true);
		}

		public static SheetFieldDef NewRadioButton(string fieldName,string radioButtonValue,int xPos,int yPos,int width,int height) {
			return new SheetFieldDef(SheetFieldType.CheckBox,fieldName,"",0,"",false,
				xPos,yPos,width,height,GrowthBehaviorEnum.None,radioButtonValue,isNew:true);
		}

		public static SheetFieldDef NewSigBox(int xPos,int yPos,int width,int height,SheetFieldType sigBox=SheetFieldType.SigBox) {
			return new SheetFieldDef(sigBox,"","",0,"",false,
				xPos,yPos,width,height,GrowthBehaviorEnum.None,"",isNew:true);
		}

		public static SheetFieldDef NewSpecial(string fieldName,int xPos,int yPos,int width,int height,string fieldValue="") {
			return new SheetFieldDef(SheetFieldType.Special,fieldName,fieldValue,0,"Calibri",false,
				xPos,yPos,width,height,GrowthBehaviorEnum.None,"",isNew:true);
		}

		public static SheetFieldDef NewGrid(string fieldName,int xPos,int yPos,int width,int height,float fontSize=8.5f,string fontName=""
			,GrowthBehaviorEnum growthBehavior=GrowthBehaviorEnum.DownGlobal)
		{
			SheetFieldDef retVal=new SheetFieldDef(SheetFieldType.Grid,fieldName,"",fontSize,fontName,false,
				xPos,yPos,width,height,growthBehavior,"",isNew:true);
			return retVal;
		}

		public static SheetFieldDef NewMobileHeader(string uiLabelMobile) {
			SheetFieldDef retVal=new SheetFieldDef(SheetFieldType.MobileHeader,"misc","",8.5f,"",false,
				0,0,0,0,GrowthBehaviorEnum.DownGlobal,"",isNew:true);
			retVal.UiLabelMobile=uiLabelMobile;
			return retVal;
		}

		///<Summary>Should only be called after FieldValue has been set, due to GrowthBehavior.</Summary>
		public Rectangle Bounds {
			get {
				return new Rectangle(XPos,YPos,Width,Height);
			}
		}
		
		///<Summary>Should only be called after FieldValue has been set, due to GrowthBehavior.</Summary>
		public RectangleF BoundsF {
			get {
				return new RectangleF(XPos,YPos,Width,Height);
			}
		}
	}

	

}
