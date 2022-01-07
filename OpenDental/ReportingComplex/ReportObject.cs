using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental.ReportingComplex {
	///<summary>There is one ReportObject for each element of an ODReport that gets printed on the page.  There are many different kinds of reportObjects.</summary>
	public class ReportObject{
		private AreaSectionType _sectionType;
		private Point _location;
		private Size _size;
		private string _name;
		private ReportObjectType _reportObjectType;
		///<summary>We could implement IDisposable on this base class for this font.  It looks like too much work.  We would need to run thousands/millions of reports for the memory problem to be noticeable.</summary>
		private Font _font;
		private ContentAlignment _contentAlignment;
		private Color _foreColor;
		private string _staticText;
		private string _stringFormat;
		private bool _suppressIfDuplicate;
		private float _floatLineThickness;
		private FieldDefKind _fieldDefKind;
		private FieldValueType _fieldValueType;
		private SpecialFieldType _specialFieldType;
		private SummaryOperation _summaryOperation;
		private LineOrientation _lineOrientation;
		private LinePosition _linePosition;
		private int _intLinePercent;
		private int _offSetX;
		private int _offSetY;
		private bool _isUnderlined;
		private string _summarizedFieldName;
		private string _dataFieldName;
		private SummaryOrientation _summaryOrientation;
		private List<int> _summaryGroupValues;
		

		#region Properties
		///<summary>The section to which this object is attached.  For lines and boxes that span multiple sections, this is the section in which the upper part of the object resides.</summary>
		public AreaSectionType SectionType {
			get{
				return _sectionType;
			}
			set{
				_sectionType=value;
			}
		}

		///<summary>Location within the section. Frequently, y=0</summary>
		public Point Location{
			get{
				return _location;
			}
			set{
				_location=value;
			}
		}

		///<summary>Typically not set since this is set by a helper function when important properties for size change.</summary>
		public Size Size{
			get{
				return _size;
			}
			set{
				_size=value;
			}
		}

		///<summary>The unique name of the ReportObject.</summary>
		public string Name{
			get{
				return _name;
			}
			set{
				_name=value;
			}
		}

		///<summary>For instance, FieldObject, or TextObject.</summary>
		public ReportObjectType ObjectType{
			get{
				return _reportObjectType;
			}
			set{
				_reportObjectType=value;
			}
		}

		///<summary>Setting this will also set the size.</summary>
		public Font Font{
			get{
				return _font;
			}
			set{
				_font=value;
				_size=CalculateNewSize(_staticText,_font);
			}
		}

		///<summary>Horizontal alignment of the text.</summary>
		public ContentAlignment ContentAlignment{
			get{
				return _contentAlignment;
			}
			set{
				_contentAlignment=value;
			}
		}

		///<summary>Can be used for text color or for line color.</summary>
		public Color ForeColor{
			get{
				return _foreColor;
			}
			set{
				_foreColor=value;
			}
		}

		///<summary>The text to display for a TextObject. Setting this will also set the size.</summary>
		public string StaticText{
			get{
				return _staticText;
			}
			set{
				_staticText=value;
				_size=CalculateNewSize(_staticText,_font);
			}
		}

		///<summary>For a FieldObject, a C# format string that specifies how to print dates, times, numbers, and currency based on the country or on a custom format.</summary>
		///<remarks>There are a LOT of options for this string.  Look in C# help under Standard Numeric Format Strings, Custom Numeric Format Strings, Standard DateTime Format Strings, Custom DateTime Format Strings, and Enumeration Format Strings.  Once users are allowed to edit reports, we will assemble a help page with all of the common options. The best options are "n" for number, and "d" for date.</remarks>
		public string StringFormat{
			get{
				return _stringFormat;
			}
			set{
				_stringFormat=value;
			}
		}

		///<summary>Suppresses this field if the field for the previous record was the same.  Only used with data fields.  E.g. So that a query ordered by a date column doesn't print the same date over and over.</summary>
		public bool SuppressIfDuplicate{
			get{
				return _suppressIfDuplicate;
			}
			set{
				_suppressIfDuplicate=value;
			}
		}

		///<summary></summary>
		public float FloatLineThickness{
			get{
				return _floatLineThickness;
			}
			set{
				_floatLineThickness=value;
			}
		}

		///<summary>Used to determine whether the line is vertical or horizontal.</summary>
		public LineOrientation LineOrientation {
			get {
				return _lineOrientation;
			}
			set {
				_lineOrientation=value;
			}
		}

		///<summary>Used to determine intial starting position of the line.</summary>
		public LinePosition LinePosition {
			get {
				return _linePosition;
			}
			set {
				_linePosition=value;
			}
		}

		///<summary>Used to determine what percentage of the section the line will draw on.</summary>
		public int IntLinePercent {
			get {
				return _intLinePercent;
			}
			set {
				_intLinePercent=value;
			}
		}

		///<summary>Used to offset lines, boxes, and text by a specific number of pixels.</summary>
		public int OffSetX {
			get {
				return _offSetX;
			}
			set {
				_offSetX=value;
			}
		}

		///<summary>Used to offset lines, boxes, and text by a specific number of pixels.</summary>
		public int OffSetY {
			get {
				return _offSetY;
			}
			set {
				_offSetY=value;
			}
		}

		///<summary>Used to underline text objects and titles.</summary>
		public bool IsUnderlined {
			get {
				return _isUnderlined;
			}
			set {
				_isUnderlined=value;
			}
		}

		///<summary>The kind of field, like FormulaField, SummaryField, or DataTableField.</summary>
		public FieldDefKind FieldDefKind{
			get{
				return _fieldDefKind;
			}
			set{
				_fieldDefKind=value;
			}
		}

		///<summary>The value type of field, like string or datetime.</summary>
		public FieldValueType FieldValueType{
			get{
				return _fieldValueType;
			}
			set{
				_fieldValueType=value;
			}
		}

		///<summary>For FieldKind=FieldDefKind.SpecialField, this is the type.  eg. pagenumber</summary>
		public SpecialFieldType SpecialFieldType{
			get{
				return _specialFieldType;
			}
			set{
				_specialFieldType=value;
			}
		}

		///<summary>For FieldKind=FieldDefKind.SummaryField, the summary operation type.</summary>
		public SummaryOperation SummaryOperation{
			get{
				return _summaryOperation;
			}
			set{
				_summaryOperation=value;
			}
		}

		///<summary>For FieldKind=FieldDefKind.SummaryField, the name of the dataField that is being summarized.  This might later be changed to refer to a ReportObject name instead (or maybe not).</summary>
		public string SummarizedField{
			get{
				return _summarizedFieldName;
			}
			set{
				_summarizedFieldName=value;
			}
		}

		///<summary>For objectKind=ReportObjectKind.FieldObject, the name of the dataField column.</summary>
		public string DataField{
			get{
				return _dataFieldName;
			}
			set{
				_dataFieldName=value;
			}
		}

		///<summary>The location of the summary label around the summary field</summary>
		public SummaryOrientation SummaryOrientation {
			get {
				return _summaryOrientation;
			}
			set {
				_summaryOrientation=value;
			}
		}

		///<summary>The numeric value of the QueryGroup. Used when summarizing groups of queries.</summary>
		public List<int> SummaryGroups {
			get {
				return _summaryGroupValues;
			}
			set {
				_summaryGroupValues=value;
			}
		}

#endregion

		#region Constructors
		///<summary>Default constructor.</summary>
		public ReportObject(){

		}

		///<summary>Creates a TextObject with the specified name, section, location and size.  The staticText and font will determine what and how it displays, while the contentAlignment will determine the relative location in the text area.</summary>
		public ReportObject(string name,AreaSectionType sectionType,Point location,Size size,string staticText,Font font,ContentAlignment contentAlignment)
			: this(name,sectionType,location,size,staticText,font,contentAlignment,0,0) {

		}

		///<summary>Creates a TextObject with the specified name, section, location and size.  The staticText and font will determine what and how it displays, while the contentAlignment will determine the relative location in the text area.  The text will be offset of its position in pixels according to the given X/Y values.</summary>
		public ReportObject(string name,AreaSectionType sectionType,Point location,Size size,string staticText,Font font,ContentAlignment contentAlignment,int offSetX,int offSetY) {
			_name=name;
			_sectionType=sectionType;
			_location=location;
			_size=size;
			_staticText=staticText;
			_font=font;
			_contentAlignment=contentAlignment;
			_offSetX=offSetX;
			_offSetY=offSetY;
			_foreColor=Color.Black;
			_reportObjectType=ReportObjectType.TextObject;
		}

		///<summary>Creates a BoxObject with the specified name, section, color and line thickness.</summary>
		public ReportObject(string name,AreaSectionType sectionType,Color color,float lineThickness)
			: this(name,sectionType,color,lineThickness,0,0) {

		}

		///<summary>Creates a BoxObject with the specified name, section, color and line thickness.  The box will be offset of its position in pixels according to the given X/Y values.</summary>
		public ReportObject(string name,AreaSectionType sectionType,Color color,float lineThickness,int offSetX,int offSetY) {
			_name=name;
			_sectionType=sectionType;
			_foreColor=color;
			_floatLineThickness=lineThickness;
			_offSetX=offSetX;
			_offSetY=offSetY;
			_reportObjectType=ReportObjectType.BoxObject;
		}

		///<summary>Creates a LineObject with the specified name, section, color, line thickness, line orientation, line position and percent.  Orientation determines whether the line is horizontal or vertical.  Position determines which side of the section the line draws on.  Percent determines how much of available space the line will take up.</summary>
		public ReportObject(string name,AreaSectionType sectionType,Color color,float lineThickness,LineOrientation lineOrientation,LinePosition linePosition,int linePercent)
			: this(name,sectionType,color,lineThickness,lineOrientation,linePosition,linePercent,0,0) {

		}

		///<summary>Creates a LineObject with the specified name, section, color, line thickness, line orientation, line position and percent.  Orientation determines whether the line is horizontal or vertical.  Position determines which side of the section the line draws on.  Percent determines how much of available space the line will take up.  The line will be offset of its position in pixels according to the given X/Y values.</summary>
		public ReportObject(string name,AreaSectionType sectionType,Color color,float lineThickness,LineOrientation lineOrientation,LinePosition linePosition,int linePercent,int offSetX,int offSetY) {
			_name=name;
			_sectionType=sectionType;
			_foreColor=color;
			_floatLineThickness=lineThickness;
			_lineOrientation=lineOrientation;
			_linePosition=linePosition;
			_intLinePercent=linePercent;
			_offSetX=offSetX;
			_offSetY=offSetY;
			_reportObjectType=ReportObjectType.LineObject;
		}

		///<summary>Mainly used from inside QueryObject.  Creates a DataTableFieldObject with the specified name, section, location, size, dataFieldName, fieldValueType, font, contentAlignment and stringFormat.  DataFieldName determines what the field will be filled with from the table.  FieldValueType determines how the field will be filled with data (i.e Number will be formatted as a number and have a summary added to the bottom of a column).  ContentAlignment determines where the text will be drawn in the box.  StringFormat is used to determined how a ToString() method call will format the field text.</summary>
		public ReportObject(string name,AreaSectionType sectionType,Point location,Size size
			,string dataFieldName,FieldValueType fieldValueType
			,Font font,ContentAlignment contentAlignment,string stringFormat) {
			_name=name;
			_sectionType=sectionType;
			_location=location;
			_size=size;
			_font=font;
			_contentAlignment=contentAlignment;
			_stringFormat=stringFormat;
			_fieldDefKind=FieldDefKind.DataTableField;
			_dataFieldName=dataFieldName;
			_fieldValueType=fieldValueType;
			//defaults:
			_foreColor=Color.Black;
			_reportObjectType=ReportObjectType.FieldObject;
		}

		///<summary>Mainly used from inside QueryObject.  Creates a SummaryFieldObject with the specified name, section, location, size, summaryOperation, summarizedFieldName, font, contentAlignment and stringFormat.  SummaryOperation determines what calculation will be used when summarizing the column.  SummarizedFieldName determines the field that will be summarized at the bottom of the column.  ContentAlignment determines where the text will be drawn in the box.  StringFormat is used to determined how a ToString() method call will format the field text.</summary>
		public ReportObject(string name,AreaSectionType sectionType,Point location,Size size,SummaryOperation summaryOperation,string summarizedFieldName,Font font,ContentAlignment contentAlignment,string stringFormat) {
			_name=name;
			_sectionType=sectionType;
			_location=location;
			_size=size;
			_font=font;
			_contentAlignment=contentAlignment;
			_stringFormat=stringFormat;
			_fieldDefKind=FieldDefKind.SummaryField;
			_fieldValueType=FieldValueType.Number;
			_summaryOperation=summaryOperation;
			_summarizedFieldName=summarizedFieldName;
			//defaults:
			_foreColor=Color.Black;
			_reportObjectType=ReportObjectType.FieldObject;
		}

		///<summary>Mainly used from inside QueryObject.  Creates a GroupSummaryObject with the specified name, section, location, size, color, summaryOperation, summarizedFieldName, font, datafield, and offsets.  SummaryOperation determines what calculation will be used when summarizing the group of column.  SummarizedFieldName determines the field that will be summarized and must be the same in each of the queries.  Datafield determines which column the summary will draw under.  The summary will be offset of its position in pixels according to the given X/Y values.</summary>
		public ReportObject(string name,AreaSectionType sectionType,Point location,Size size,Color color,SummaryOperation summaryOperation,string summarizedFieldName,Font font,ContentAlignment contentAlignment,string datafield,int offSetX,int offSetY,string stringFormat="") {
			_name=name;
			_sectionType=sectionType;
			_location=location;
			_size=size;
			_dataFieldName=datafield;
			_font=font;
			_fieldDefKind=FieldDefKind.SummaryField;
			_fieldValueType=FieldValueType.Number;
			_stringFormat=stringFormat;
			_summaryOperation=summaryOperation;
			_summarizedFieldName=summarizedFieldName;
			_offSetX=offSetX;
			_offSetY=offSetY;
			_foreColor=color;
			//defaults:
			_contentAlignment=contentAlignment;
			_reportObjectType=ReportObjectType.TextObject;
		}

		///<summary>Currently only used for page numbers.</summary>
		public ReportObject(string name,AreaSectionType sectionType,Point location,Size size,FieldValueType fieldValueType,SpecialFieldType specialType,Font font,ContentAlignment contentAlignment,string stringFormat) {
			_name=name;
			_sectionType=sectionType;
			_location=location;
			_size=size;
			_font=font;
			_contentAlignment=contentAlignment;
			_stringFormat=stringFormat;
			_fieldDefKind=FieldDefKind.SpecialField;
			_fieldValueType=fieldValueType;
			_specialFieldType=specialType;
			//defaults:
			_foreColor=Color.Black;
			_reportObjectType=ReportObjectType.FieldObject;
		}
		#endregion Constructors

		///<summary>Converts contentAlignment into a combination of StringAlignments used to format strings.  This method is mostly called for drawing text on reportObjects.</summary>
		public static StringFormat GetStringFormatAlignment(ContentAlignment contentAlignment){
			if(!Enum.IsDefined(typeof(ContentAlignment),(int)contentAlignment))
				throw new System.ComponentModel.InvalidEnumArgumentException(
					"contentAlignment",(int)contentAlignment,typeof(ContentAlignment));
			StringFormat stringFormat = new StringFormat();
			switch (contentAlignment){
				case ContentAlignment.MiddleCenter:
					stringFormat.LineAlignment = StringAlignment.Center;
					stringFormat.Alignment = StringAlignment.Center;
					break;
				case ContentAlignment.MiddleLeft:
					stringFormat.LineAlignment = StringAlignment.Center;
					stringFormat.Alignment = StringAlignment.Near;
					break;
				case ContentAlignment.MiddleRight:
					stringFormat.LineAlignment = StringAlignment.Center;
					stringFormat.Alignment = StringAlignment.Far;
					break;
				case ContentAlignment.TopCenter:
					stringFormat.LineAlignment = StringAlignment.Near;
					stringFormat.Alignment = StringAlignment.Center;
					break;
				case ContentAlignment.TopLeft:
					stringFormat.LineAlignment = StringAlignment.Near;
					stringFormat.Alignment = StringAlignment.Near;
					break;
				case ContentAlignment.TopRight:
					stringFormat.LineAlignment = StringAlignment.Near;
					stringFormat.Alignment = StringAlignment.Far;
					break;
				case ContentAlignment.BottomCenter:
					stringFormat.LineAlignment = StringAlignment.Far;
					stringFormat.Alignment = StringAlignment.Center;
					break;
				case ContentAlignment.BottomLeft:
					stringFormat.LineAlignment = StringAlignment.Far;
					stringFormat.Alignment = StringAlignment.Near;
					break;
				case ContentAlignment.BottomRight:
					stringFormat.LineAlignment = StringAlignment.Far;
					stringFormat.Alignment = StringAlignment.Far;
					break;
			}
			return stringFormat;
		}

		///<summary>Used to copy a report object when creating new QueryObjects.</summary>
		public ReportObject DeepCopyReportObject() {
			ReportObject reportObj=new ReportObject();
			reportObj._sectionType=this._sectionType;
			reportObj._location=new Point(this._location.X,this._location.Y);
			reportObj._size=new Size(this._size.Width,this._size.Height);
			reportObj._name=this._name;
			reportObj._reportObjectType=this._reportObjectType;
			reportObj._font=(Font)this._font.Clone();
			reportObj._contentAlignment=this._contentAlignment;
			reportObj._foreColor=this._foreColor;
			reportObj._staticText=this._staticText;
			reportObj._stringFormat=this._stringFormat;
			reportObj._suppressIfDuplicate=this._suppressIfDuplicate;
			reportObj._floatLineThickness=this._floatLineThickness;
			reportObj._fieldDefKind=this._fieldDefKind;
			reportObj._fieldValueType=this._fieldValueType;
			reportObj._specialFieldType=this._specialFieldType;
			reportObj._summaryOperation=this._summaryOperation;
			reportObj._lineOrientation=this._lineOrientation;
			reportObj._linePosition=this._linePosition;
			reportObj._intLinePercent=this._intLinePercent;
			reportObj._offSetX=this._offSetX;
			reportObj._offSetY=this._offSetY;
			reportObj._isUnderlined=this._isUnderlined;
			reportObj._summarizedFieldName=this._summarizedFieldName;
			reportObj._dataFieldName=this._dataFieldName;
			reportObj._summaryOrientation=this._summaryOrientation;
			List<int> summaryGroupsNew=new List<int>();
			if(this._summaryGroupValues!=null) {
				for(int i=0;i<this._summaryGroupValues.Count;i++) {
					summaryGroupsNew.Add(this._summaryGroupValues[i]);
				}
			}
			reportObj._summaryGroupValues=summaryGroupsNew;
			return reportObj;
		}

		///<summary>Once a dataTable has been set, this method can be run to get the summary value of this field.  It will still need to be formatted.  It loops through all records to get this value.</summary>
		public double GetSummaryValue(DataTable dataTable,int col){
			double retVal=0;
			for(int i=0;i<dataTable.Rows.Count;i++) {
				if(SummaryOperation==SummaryOperation.Sum) {
					retVal+=PIn.Double(dataTable.Rows[i][col].ToString());
				}
				else if(SummaryOperation==SummaryOperation.Count) {
					retVal++;
				}
				else if(SummaryOperation==SummaryOperation.Average) {
					retVal+=(PIn.Double(dataTable.Rows[i][col].ToString())/dataTable.Rows.Count);
				}
			}
			return retVal;
		}

		///<summary>Used to automatically calculate the new size when something important changes. Also recalculates location for report headers.</summary>
		private Size CalculateNewSize(string text,Font font) {
			Graphics grfx=Graphics.FromImage(new Bitmap(1,1));
			Size size;
			if(_sectionType==AreaSectionType.GroupHeader || _sectionType==AreaSectionType.GroupFooter || _sectionType==AreaSectionType.Detail) {
				size=new Size(_size.Width,(int)(grfx.MeasureString(text,font).Height/grfx.DpiY*100+2));
			}
			else {
				size=new Size((int)(grfx.MeasureString(text,font).Width/grfx.DpiX*100+2),(int)(grfx.MeasureString(text,font).Height/grfx.DpiY*100+2));
			}
			if(_sectionType==AreaSectionType.ReportHeader) {
				_location.X+=(_size.Width/2);
				_location.X-=(size.Width/2);
			}
			return size;
		}

	}

	///<summary>Specifies the field kind in the FieldKind property of the ReportObject class.  Used in Queries and Datatables.</summary>
	public enum FieldDefKind{
		///<summary>Basic informational cell/field for a Datatable.</summary>
		DataTableField,
		///<summary>Currently not in use.</summary>
		FormulaField,
		///<summary>Used in conjunction with SpecialFieldType to determine special logic for certain objects.  Currently only used for PageNumbers</summary>
		SpecialField,
		///<summary>Used when creating summaries of a column.  Uses the current SummaryOperation to determine which calculation to make.</summary>
		SummaryField
		//RunningTotalField
		//GroupNameField
	}

	///<summary>Used in the Kind field of each ReportObject to provide a quick way to tell what kind of reportObject.</summary>
	public enum ReportObjectType{
		///<summary>Object is a box and will draw a rectangle with the specified parameters.</summary>
		BoxObject,
		///<summary>Object is a field object and will be used in drawing datatables.</summary>
		FieldObject,
		///<summary>Object is a line and will draw a straight line with the specified parameters.</summary>
		LineObject,
		///<summary>Object is a special subset of ReportObject.  Contains its own list of ReportObjects and always contains a query or datatable of information that will be drawn in the report.</summary>
		QueryObject,
		///<summary>Object is a text object.  Can be placed anywhere and is used in multiple sections.  Not to be confused with Datatable cell/field objects.</summary>
		TextObject
		//Not Implemented--------------------------------------------------------------
		//BlobFieldObject Object is a blob field. 
		//ChartObject Object is a chart. 
		//CrossTabObject Object is a cross tab. 
		//PictureObject Object is a picture. 
		//SubreportObject Object is a subreport.
	}

	///<summary>Specifies the special field type in the SpecialType property of the ReportObject class.</summary>
	public enum SpecialFieldType{
		///<summary>Field returns "Page [current page number] of [total page count]" formula. Not functional yet.</summary>
		PageNofM,
		///<summary>Field returns the current page number.</summary>
		PageNumber,
		///<summary>Field returns the current date.  Currently not in use.</summary>
		PrintDate
	}

	///<summary></summary>
	public enum SummaryOperation{
		///<summary>Summary counts the number of values, from the field.</summary>
		Count,
		///<summary>Summary returns the total of all the values for the field.</summary>
		Sum,
		///<summary>Summary returns the average of a field</summary>
		Average,
		//Not Implemented--------------------------------------------------------------
		//DistinctCount Summary returns the number of none repeating values, from the field. 
		//Maximum Summary returns the largest value from the field. 
		//Median Summary returns the middle value in a sequence of numeric values. 
		//Minimum Summary returns the smallest value from the field. 
		//Percentage Summary returns as a percentage of the grand total summary. 
	}

	///<summary>Used to determine how a line draws in a section.</summary>
	public enum LineOrientation{
		///<summary></summary>
		Horizontal,
		///<summary></summary>
		Vertical
		//Not Implemented--------------------------------------------------------------
		//Diagnonal
	}

	///<summary>Used to determine where a line draws in a section.</summary>
	public enum LinePosition{
		///<summary>Used in Horizontal and Vertical Orientation</summary>
		Center,
		///<summary>Used in Vertical Orientation</summary>
		East,
		///<summary>Used in Horizontal Orientation</summary>
		North,
		///<summary>Used in Horizontal Orientation</summary>
		South,
		///<summary>Used in Vertical Orientation</summary>
		West
	}

	///<summary>This determines what type of column the table will be splitting on. Default is none.</summary>
	public enum SplitByKind {
		///<summary></summary>
		None,
		///<summary>1</summary>
		Date,
		///<summary>2</summary>
		Enum,
		///<summary>3</summary>
		Definition,
		///<summary>4</summary>
		Value
	}

	///<summary>This determines which side of the summaryfield the label will be drawn on.</summary>
	public enum SummaryOrientation {
		///<summary>0</summary>
		None,
		///<summary>1</summary>
		North,
		///<summary>2</summary>
		South,
		///<summary>3</summary>
		East,
		///<summary>4</summary>
		West
	}

	



}
