using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental.ReportingComplex {

	///<summary>For every query added to a report there will be at least one QueryObject.</summary>
	public class QueryObject:ReportObject  {
		#region Fields
		private SectionCollection _sections=new SectionCollection();
		private ArrayList _arrDataFields=new ArrayList();
		private ReportObjectCollection _reportObjects=new ReportObjectCollection();
		private string _columnNameToSplitOn;
		private string _stringQuery;
		private DataTable _reportTable;
		private DataTable _exportTable;
		private List<int> _rowHeightValues;
		private List<string> _listEnumNames;
		private Dictionary<long,string> _dictDefNames;
		private SplitByKind _splitByKind;
		private int _queryGroupValue;
		private int _queryWidth;
		private bool _isCentered;
		private bool _suppressHeaders;
		private bool _isLastSplit;
		private bool _isNegativeSummary;
		public bool IsPrinted;
		# endregion Fields

		#region Constructors
		///<summary>Default constructor.  Do not use.  Only used from DeepCopy()</summary>
		public QueryObject() {
		}

		///<summary>Specify either a table or a query.  Specifying a column name and split type will cause the query to be broken into separate tables according to changes in that column.  Specifying a query group will allow GroupSummary objects to total up only queries in the specified group.</summary>
		public QueryObject(string title,DataTable tableQuery=null,string stringQuery="",Font font=null,bool isCentered=true,int queryGroupValue=0,string columnNameToSplitOn="",SplitByKind splitByKind=SplitByKind.None,List<string> listEnumNames=null,Dictionary<long,string> dictDefNames=null) {
			if(tableQuery==null) {
				ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Adding Query To Report..."));
			}
			else {
				ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Adding Table To Report..."));
			}
			Graphics grfx=Graphics.FromImage(new Bitmap(1,1));
			if(font==null) {
				font=new Font("Tahoma",9);
			}
			_columnNameToSplitOn=columnNameToSplitOn;
			_reportTable=tableQuery;
			_stringQuery=stringQuery;
			SectionType=AreaSectionType.Query;
			Name="Query";
			_splitByKind=splitByKind;
			_listEnumNames=listEnumNames;
			_dictDefNames=dictDefNames;
			_queryGroupValue=queryGroupValue;
			_isCentered=isCentered;
			ObjectType=ReportObjectType.QueryObject;
			_sections.Add(new Section(AreaSectionType.GroupTitle,0));
			if(tableQuery==null) {
				_reportObjects.Add(new ReportObject("Group Title",AreaSectionType.GroupTitle,new Point(0,0),new Size((int)(grfx.MeasureString(title,font).Width/grfx.DpiX*100+2),(int)(grfx.MeasureString(title,font).Height/grfx.DpiY*100+2)),title,font,ContentAlignment.MiddleLeft,0,0));
			}
			else {
				_reportObjects.Add(new ReportObject("Group Title",AreaSectionType.GroupTitle,new Point(0,0),new Size((int)(grfx.MeasureString(title,font).Width/grfx.DpiX*100+2),(int)(grfx.MeasureString(title,font).Height/grfx.DpiY*100+2)),title,font,ContentAlignment.MiddleLeft));
			}
			_reportObjects["Group Title"].IsUnderlined=true;
			_sections.Add(new Section(AreaSectionType.GroupHeader,0));
			_sections.Add(new Section(AreaSectionType.Detail,0));
			_sections.Add(new Section(AreaSectionType.GroupFooter,0));
			_queryWidth=0;
			_suppressHeaders=true;
			_isLastSplit=true;
			_exportTable=new DataTable();
			grfx.Dispose();
		}
		#endregion Constructors

		#region Properties
		public SectionCollection Sections {
			get {
				return _sections;
			}
		}

		public ArrayList ArrDataFields {
			get {
				return _arrDataFields;
			}
		}

		///<summary>A collection of report objects that comprise a single query.  This will contain a title, column headers, data fields, etc.</summary>
		public ReportObjectCollection ReportObjects {
			get {
				return _reportObjects;
			}
		}

		///<summary>When the content of the data field changes within the column that has this name a new table will be created.  E.g. splitting up one query into multiple tables by payment types.</summary>
		public string ColumnNameToSplitOn {
			get {
				return _columnNameToSplitOn;
			}
		}

		///<summary>A table that represents the raw results of the query.</summary>
		public DataTable ReportTable {
			get {
				return _reportTable;
			}
			set {
				_reportTable=value;
			}
		}

		///<summary>A table that only contains columns from the ReportTable that will be displayed.</summary>
		public DataTable ExportTable {
			get {
				return _exportTable;
			}
			set {
				_exportTable=value;
			}
		}

		public List<string> ListEnumNames {
			get {
				return _listEnumNames;
			}
		}

		public Dictionary<long,string> DictDefNames {
			get {
				return _dictDefNames;
			}
		}

		public SplitByKind SplitByKind {
			get {
				return _splitByKind;
			}
		}

		public List<int> RowHeightValues {
			get {
				return _rowHeightValues;
			}
			set {
				_rowHeightValues=value;
			}
		}

		public int QueryGroupValue {
			get {
				return _queryGroupValue;
			}
			set {
				_queryGroupValue=value;
			}
		}

		public bool IsCentered {
			get {
				return _isCentered;
			}
			set {
				_isCentered=value;
			}
		}

		public int QueryWidth {
			get {
				return _queryWidth;
			}
			set {
				_queryWidth=value;
			}
		}

		public bool SuppressHeaders {
			get {
				return _suppressHeaders;
			}
			set {
				_suppressHeaders=value;
			}
		}

		public bool IsLastSplit {
			get {
				return _isLastSplit;
			}
			set {
				_isLastSplit=value;
			}
		}

		public bool IsNegativeSummary {
			get {
				return _isNegativeSummary;
			}
			set {
				_isNegativeSummary=value;
			}
		}
		#endregion Properties

		#region Methods
		///<summary>Adds a datafield column with the specified type, width and font.  If the column is type Double, then the alignment is set right and a total field is added. Also, default formatstrings are set for dates and doubles.  Does not add lines or shading.</summary>
		public void AddColumn(string dataField,int width,FieldValueType fieldValueType=FieldValueType.String,Font font=null,string formatString="") {
			ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Adding Column To Table..."));
			Graphics grfx=Graphics.FromImage(new Bitmap(1,1));
			_arrDataFields.Add(dataField);
			if(font==null){ 
				font=new Font("Tahoma",9);
			}
			Font fontHeader=new Font(font.FontFamily,font.Size-1,FontStyle.Bold);
			Font fontFooter=new Font(font.FontFamily,font.Size,FontStyle.Bold);
			ContentAlignment textAlign;
			if(fieldValueType==FieldValueType.Number) {
				textAlign=ContentAlignment.TopRight;
			}
			else {
				textAlign=ContentAlignment.TopLeft;
			}
			if(formatString==""){
				if(fieldValueType==FieldValueType.Number) {
					formatString="n";
				}
				if(fieldValueType==FieldValueType.Date) {
					formatString="d";
				}
			}
			_queryWidth+=width;
			//add textobject for column header
			Size sizeHeader=new Size((int)grfx.MeasureString(dataField,fontHeader,(int)(width/grfx.DpiX*100+2)).Width,(int)grfx.MeasureString(dataField,fontHeader,(int)(width/grfx.DpiY*100+2)).Height);
			Size sizeDetail=new Size((int)grfx.MeasureString(dataField,font,(int)(width/grfx.DpiX*100+2)).Width,(int)grfx.MeasureString(dataField,font,(int)(width/grfx.DpiY*100+2)).Height);
			Size sizeFooter=new Size((int)grfx.MeasureString(dataField,fontFooter,(int)(width/grfx.DpiX*100+2)).Width,(int)grfx.MeasureString(dataField,fontFooter,(int)(width/grfx.DpiY*100+2)).Height);
			int xPos=0;
			//find next available xPos
			foreach(ReportObject reportObject in _reportObjects) {
				if(reportObject.SectionType!=AreaSectionType.GroupHeader) {
					continue;
				}
				if(reportObject.Location.X+reportObject.Size.Width > xPos) {
					xPos=reportObject.Location.X+reportObject.Size.Width;
				}
			}
			_reportObjects.Add(new ReportObject(dataField+"Header",AreaSectionType.GroupHeader
				,new Point(xPos,0),new Size(width,sizeHeader.Height),dataField,fontHeader,textAlign));
			//add fieldObject for rows in details section
			_reportObjects.Add(new ReportObject(dataField+"Detail",AreaSectionType.Detail
				,new Point(xPos,0),new Size(width,sizeDetail.Height)
				,dataField,fieldValueType
				,font,textAlign,formatString));
			//add fieldObject for total in GroupFooter
			if(fieldValueType==FieldValueType.Number) {
				//use same size as already set for otherFieldObjects above
				_reportObjects.Add(new ReportObject(dataField+"Footer",AreaSectionType.GroupFooter
					,new Point(xPos,0),new Size(width,sizeFooter.Height)
					,SummaryOperation.Sum,dataField
					,fontFooter,textAlign,formatString));
			}
			_exportTable.Columns.Add(dataField);
			grfx.Dispose();
			return;
		}

		///<summary>Adds a summary object, of the specified color, for the specified group of queries, giving it a label with the given text and font.  The summary is placed under the specified column and summarizes the specified datafield.  Choosing a summaryOperation will change the displayed value calculation.  The summary will be offset of its position in pixels according to the given X/Y values.</summary>
		public void AddGroupSummaryField(string staticText,string columnName,string dataFieldName,SummaryOperation summaryOperation,List<int> queryGroupValues=null,Color color=default(Color),Font font=null,int offSetX=0,int offSetY=0,string formatString="") {
			ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Adding Group Summary To Tables..."));
			Graphics grfx=Graphics.FromImage(new Bitmap(1,1));
			if(queryGroupValues==null) {
				queryGroupValues=new List<int>(){0};
			}
			if(color==default) {
				color=Color.Black;
			}
			if(font==null) {
				font=new Font("Tahoma",8,FontStyle.Bold);
			}
			Point location=GetObjectByName(columnName+"Header").Location;
			Size labelSize=new Size((int)(grfx.MeasureString(staticText,font).Width/grfx.DpiX*100+2)
				,(int)(grfx.MeasureString(staticText,font).Height/grfx.DpiY*100+2));
			int i=_reportObjects.Add(new ReportObject(columnName+"GroupSummaryLabel",AreaSectionType.GroupFooter,new Point(location.X-labelSize.Width,0),labelSize,staticText,font,ContentAlignment.MiddleRight,offSetX,offSetY));
			_reportObjects[i].DataField=dataFieldName;
			_reportObjects[i].SummaryGroups=queryGroupValues;
			_sections[AreaSectionType.GroupFooter].Height+=(int)((grfx.MeasureString(staticText,font)).Height/grfx.DpiY*100+2)+offSetY;
			i=_reportObjects.Add(new ReportObject(columnName+"GroupSummaryText",AreaSectionType.GroupFooter,location,new Size(0,0),color,summaryOperation,columnName,font,ContentAlignment.MiddleLeft,dataFieldName,offSetX,offSetY,formatString));
			_reportObjects[i].SummaryGroups=queryGroupValues;
			grfx.Dispose();
		}
		
		///<summary>Do not use. Only used when splitting a table on a column.</summary>
		public void AddInitialHeader(string title,Font font) {
			ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Adding Initial Header To Table..."));
			Graphics grfx=Graphics.FromImage(new Bitmap(1,1));
			Font newFont=new Font(font.FontFamily,font.Size+2,font.Style);
			_reportObjects.Insert(0,new ReportObject("Initial Group Title",AreaSectionType.GroupTitle,new Point(0,0),new Size((int)(grfx.MeasureString(title,newFont).Width/grfx.DpiX*100+2),(int)(grfx.MeasureString(title,newFont).Height/grfx.DpiY*100+2)),title,newFont,ContentAlignment.MiddleLeft));
			_reportObjects["Initial Group Title"].IsUnderlined=true;
			grfx.Dispose();
		}

		///<summary>Adds a line to the specified section with the specified orientation and position.  The line will be offset of its position in pixels according to the given X/Y values.</summary>
		public void AddLine(string name,AreaSectionType sectionType,LineOrientation lineOrientation,LinePosition linePosition,Color color,float floatLineThickness,int linePercentValue,int offSetX,int offSetY) {
			ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Adding Line To Table..."));
			_reportObjects.Add(new ReportObject(name,sectionType,color,floatLineThickness,lineOrientation,linePosition,linePercentValue,offSetX,offSetY));
		}

		///<summary>Add a label with the given text and font to the summary value of a column, based on the orientation given.  True will cause the label to wrap within the bounds of the column.</summary>
		public void AddSummaryLabel(string dataFieldName,string summaryText,SummaryOrientation summaryOrientation,bool hasWordWrap,Font font) {
			ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Adding Summary To Table..."));
			Graphics grfx=Graphics.FromImage(new Bitmap(1,1));
			ReportObject summaryField=GetObjectByName(dataFieldName+"Footer");
			Size size;
			if(hasWordWrap) {
				size=new Size(summaryField.Size.Width,(int)(grfx.MeasureString(summaryText,font,summaryField.Size.Width).Height/grfx.DpiY*100+2));
			}
			else {
				size=new Size((int)(grfx.MeasureString(summaryText,font).Width/grfx.DpiX*100+2),(int)(grfx.MeasureString(summaryText,font).Height/grfx.DpiY*100+2));
			}
			if(summaryOrientation==SummaryOrientation.North) {
				ReportObject summaryLabel=new ReportObject(dataFieldName+"Label",AreaSectionType.GroupFooter
						,summaryField.Location
						,size
						,summaryText
						,font
						,summaryField.ContentAlignment);
				summaryLabel.DataField=dataFieldName;
				summaryLabel.SummaryOrientation=summaryOrientation;
				_reportObjects.Insert(_reportObjects.IndexOf(summaryField),summaryLabel);
			}
			else if(summaryOrientation==SummaryOrientation.South) {
				ReportObject summaryLabel=new ReportObject(dataFieldName+"Label",AreaSectionType.GroupFooter
						,summaryField.Location
						,size
						,summaryText
						,font
						,summaryField.ContentAlignment);
				summaryLabel.DataField=dataFieldName;
				summaryLabel.SummaryOrientation=summaryOrientation;
				_reportObjects.Add(summaryLabel);
			}
			else if(summaryOrientation==SummaryOrientation.West) {
				ReportObject summaryLabel=new ReportObject(dataFieldName+"Label",AreaSectionType.GroupFooter
						,new Point(summaryField.Location.X-size.Width)
						,size
						,summaryText
						,font
						,summaryField.ContentAlignment);
				summaryLabel.DataField=dataFieldName;
				summaryLabel.SummaryOrientation=summaryOrientation;
				_reportObjects.Insert(_reportObjects.IndexOf(summaryField),summaryLabel);
			}
			else {
				ReportObject summaryLabel=new ReportObject(dataFieldName+"Label",AreaSectionType.GroupFooter
						,new Point(summaryField.Location.X+size.Width+summaryField.Size.Width)
						,size
						,summaryText
						,font
						,summaryField.ContentAlignment);
				summaryLabel.DataField=dataFieldName;
				summaryLabel.SummaryOrientation=summaryOrientation;
				_reportObjects.Insert(_reportObjects.IndexOf(summaryField)+1,summaryLabel);
			}
			grfx.Dispose();
		}

    public void CalculateRowHeights(bool isWrapping) {
			ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Creating Query In Report..."));
      Graphics g=Graphics.FromImage(new Bitmap(1,1));
      _rowHeightValues=new List<int>();
      for(int i=0;i<_reportTable.Rows.Count;i++) {
				string rawText;
				string displayText="";
				string prevDisplayText="";
				int rowHeight=0;
				foreach(ReportObject reportObject in _reportObjects) {
					if(reportObject.SectionType!=AreaSectionType.Detail) {
						continue;
					}
					if(reportObject.ObjectType==ReportObjectType.FieldObject) {
						rawText=_reportTable.Rows[i][_arrDataFields.IndexOf(reportObject.DataField)].ToString();
						if(String.IsNullOrWhiteSpace(rawText)) {
							continue;
						}
						List<string> listString=GetDisplayString(rawText,prevDisplayText,reportObject,i);
						displayText=listString[0];
						prevDisplayText=listString[1];
            int curCellHeight=0;
            if(isWrapping) {
              curCellHeight=(int)((g.MeasureString(displayText,reportObject.Font,(int)(reportObject.Size.Width),
							  ReportObject.GetStringFormatAlignment(reportObject.ContentAlignment))).Height*(100f/96f));//due to pixel factor
            }
            else {
              curCellHeight=(int)((g.MeasureString(displayText,reportObject.Font,0,
							  ReportObject.GetStringFormatAlignment(reportObject.ContentAlignment))).Height*(100f/96f));//due to pixel factor
            }
						if(curCellHeight>rowHeight) {
							rowHeight=curCellHeight;
						}
					}
				}
				_rowHeightValues.Add(rowHeight);
			}
      g.Dispose();
    }

		public QueryObject DeepCopyQueryObject() {
			QueryObject queryObj=new QueryObject();
			queryObj.Name=this.Name;//Doesn't need to be a deep copy.
			queryObj.SectionType=this.SectionType;//Doesn't need to be a deep copy.
			queryObj.ObjectType=this.ObjectType;//Doesn't need to be a deep copy.
			queryObj._sections=this._sections;//Doesn't need to be a deep copy.
			queryObj._arrDataFields=this._arrDataFields;//Doesn't need to be a deep copy.
			queryObj._queryGroupValue=this._queryGroupValue;//Doesn't need to be a deep copy.
			queryObj._isCentered=this._isCentered;//Doesn't need to be a deep copy.
			queryObj._queryWidth=this._queryWidth;//Doesn't need to be a deep copy.
			queryObj._suppressHeaders=this._suppressHeaders;//Doesn't need to be a deep copy.
			queryObj._columnNameToSplitOn=this._columnNameToSplitOn;//Doesn't need to be a deep copy.
			queryObj._splitByKind=this._splitByKind;//Doesn't need to be a deep copy.
			queryObj.IsPrinted=this.IsPrinted;//Doesn't need to be a deep copy.
			queryObj.SummaryOrientation=this.SummaryOrientation;//Doesn't need to be a deep copy.
			queryObj.SummaryGroups=this.SummaryGroups;//Doesn't need to be a deep copy.
			queryObj._isLastSplit=this._isLastSplit;//Doesn't need to be a deep copy.
			queryObj._rowHeightValues=new List<int>();
			queryObj._isNegativeSummary=this._isNegativeSummary;
			for(int i=0;i<this._rowHeightValues.Count;i++) {
				queryObj._rowHeightValues.Add(this._rowHeightValues[i]);
			}
			ReportObjectCollection reportObjectsNew=new ReportObjectCollection();
			for(int i=0;i<this._reportObjects.Count;i++) {
				reportObjectsNew.Add(_reportObjects[i].DeepCopyReportObject());
			}
			queryObj._reportObjects=reportObjectsNew;
			//queryObj._query=this._query;
			queryObj._reportTable=new DataTable();
			//We only care about column headers at this point.  There is no easy way to copy an entire DataTable.
			for(int i=0;i<this.ReportTable.Columns.Count;i++) {
				queryObj._reportTable.Columns.Add(new DataColumn(this.ReportTable.Columns[i].ColumnName));
			}
			queryObj._exportTable=new DataTable();
			//We only care about column headers at this point.  There is no easy way to copy an entire DataTable.
			for(int i=0;i<this._exportTable.Columns.Count;i++) {
				queryObj._exportTable.Columns.Add(new DataColumn(this._exportTable.Columns[i].ColumnName));
			}
			List<string> enumNamesNew=new List<string>();
			if(this._listEnumNames!=null) {
				for(int i=0;i<this._listEnumNames.Count;i++) {
					enumNamesNew.Add(this._listEnumNames[i]);
				}
			}
			queryObj._listEnumNames=enumNamesNew;
			Dictionary<long,string> defNamesNew=new Dictionary<long,string>();
			if(this._dictDefNames!=null) {
				foreach(long defNum in _dictDefNames.Keys) {
					defNamesNew.Add(defNum,this._dictDefNames[defNum]);
				}
			}
			queryObj._dictDefNames=defNamesNew;
			return queryObj;
		}

		public ReportObject GetColumnDetail(string columnName) {
			return ReportObjects[columnName+"Detail"];
		}

		public ReportObject GetColumnFooter(string columnName) {
			return ReportObjects[columnName+"Footer"];
		}

		public ReportObject GetColumnHeader(string columnName) {
			return ReportObjects[columnName+"Header"];
		}

		private List<string> GetDisplayString(string rawText,string prevDisplayText,ReportObject reportObject,int i) {
			string displayText="";
			List<string> retVals=new List<string>();
			if(reportObject.FieldValueType==FieldValueType.Age) {
				displayText=Patients.AgeToString(Patients.DateToAge(PIn.Date(rawText)));//(fieldObject.FormatString);
			}
			else if(reportObject.FieldValueType==FieldValueType.Boolean) {
				displayText=PIn.Bool(_reportTable.Rows[i][_arrDataFields.IndexOf(reportObject.DataField)].ToString()).ToString();//(fieldObject.FormatString);
				if(i>0 && reportObject.SuppressIfDuplicate) {
					prevDisplayText=PIn.Bool(_reportTable.Rows[i-1][_arrDataFields.IndexOf(reportObject.DataField)].ToString()).ToString();
				}
			}
			else if(reportObject.FieldValueType==FieldValueType.Date) {
				DateTime rowDateTime=PIn.DateT(_reportTable.Rows[i][_arrDataFields.IndexOf(reportObject.DataField)].ToString());
				if(rowDateTime.Year>1880) {
					displayText=rowDateTime.ToString(reportObject.StringFormat);
				}
				if(i>0 && reportObject.SuppressIfDuplicate) {
					rowDateTime=PIn.DateT(_reportTable.Rows[i-1][_arrDataFields.IndexOf(reportObject.DataField)].ToString());
					prevDisplayText="";
					if(rowDateTime.Year>1880) {
						prevDisplayText=rowDateTime.ToString(reportObject.StringFormat);
					}
				}
			}
			else if(reportObject.FieldValueType==FieldValueType.Integer) {
				displayText=PIn.Long(_reportTable.Rows[i][_arrDataFields.IndexOf(reportObject.DataField)].ToString()).ToString(reportObject.StringFormat);
				if(i>0 && reportObject.SuppressIfDuplicate) {
					prevDisplayText=PIn.Long(_reportTable.Rows[i-1][_arrDataFields.IndexOf(reportObject.DataField)].ToString()).ToString(reportObject.StringFormat);
				}
			}
			else if(reportObject.FieldValueType==FieldValueType.Number) {
				displayText=PIn.Double(_reportTable.Rows[i][_arrDataFields.IndexOf(reportObject.DataField)].ToString()).ToString(reportObject.StringFormat);
				if(i>0 && reportObject.SuppressIfDuplicate) {
					prevDisplayText=PIn.Double(_reportTable.Rows[i-1][_arrDataFields.IndexOf(reportObject.DataField)].ToString()).ToString(reportObject.StringFormat);
				}
			}
			else if(reportObject.FieldValueType==FieldValueType.String) {
				displayText=rawText;
				if(i>0 && reportObject.SuppressIfDuplicate) {
					prevDisplayText=_reportTable.Rows[i-1][_arrDataFields.IndexOf(reportObject.DataField)].ToString();
				}
			}
			retVals.Add(displayText);
			retVals.Add(prevDisplayText);
			return retVals;
		}

		public ReportObject GetGroupTitle() {
			return ReportObjects["Group Title"];
		}

		public ReportObject GetObjectByName(string name) {
			for(int i=_reportObjects.Count-1;i>=0;i--) {//search from the end backwards
				if(_reportObjects[i].Name==name) {
					return ReportObjects[i];
				}
			}
			MessageBox.Show("end of loop");
			return null;
		}

		///<summary>If the specified section exists, then this returns its height. Otherwise it returns 0.</summary>
		public int GetSectionHeight(AreaSectionType sectionType) {
			return _sections[sectionType].Height;
		}

		public int GetTotalHeight() {
			int height=0;
			height+=_sections[AreaSectionType.GroupTitle].Height;
			height+=_sections[AreaSectionType.GroupHeader].Height;
			height+=_sections[AreaSectionType.Detail].Height;
			height+=_sections[AreaSectionType.GroupFooter].Height;
			return height;
		}

		///<summary>Submits the Query to the database and fills ReportTable with the results.  Returns false if the query fails.</summary>
		public bool SubmitQuery() {
			if(String.IsNullOrWhiteSpace(_stringQuery)) {
				//The programmer must have prefilled the data table already, so no reason to try and run a query.
			}
			else {
				try {
					_reportTable=ReportsComplex.GetTable(_stringQuery);
					//_reportTable=ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(_stringQuery)); //submit query on different thread to the reporting server.
				}
				catch(Exception) {
					return false;
				}
			}
      _rowHeightValues=new List<int>();
			DataRow row;
			for(int i=0;i<_reportTable.Rows.Count;i++) {
				row=_exportTable.NewRow();
				for(int j=0;j<_exportTable.Columns.Count;j++) {
					row[j]=_reportTable.Rows[i][j];
				}
				_exportTable.Rows.Add(row);
			}
			return true;
		}
		#endregion Methods






	}
}
