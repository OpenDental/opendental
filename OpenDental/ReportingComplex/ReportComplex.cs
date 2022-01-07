using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CodeBase;
using System.Linq;
using OpenDental.UI;

namespace OpenDental.ReportingComplex {
	/// <summary>This class is loosely modeled after CrystalReports.ReportDocument, but with less inheritence and heirarchy.</summary>
	public class ReportComplex {
		private SectionCollection _sections=new SectionCollection();
		private ReportObjectCollection _reportObjects=new ReportObjectCollection();
		private ParameterFieldCollection _parameterFields=new ParameterFieldCollection();
		private bool _isLandscape;
		private bool _hasGridLines;
		private string _reportName;
		private string _description;
		private string _authorID;
		private int _letterOrder;
		///<summary>This is simply used to measure strings for alignment purposes.</summary>
		private Graphics _grfx;
		private int _totalRows;
		///<summary>Can be changed on a per report basis.</summary>
		public Margins PrintMargins=new Margins(30,0,50,50);

		#region Properties

		///<summary>Collection of Sections.</summary>
		public SectionCollection Sections{
			get{
				return _sections;
			}
			set{
				_sections=value;
			}
		}
		///<summary>A collection of ReportObjects</summary>
		public ReportObjectCollection ReportObjects{
			get{
				return _reportObjects;
			}
			set{
				_reportObjects=value;
			}
		}
		///<summary>Collection of ParameterFields that are available for the query.</summary>
		public ParameterFieldCollection ParameterFields{
			get{
				return _parameterFields;
			}
			set{
				_parameterFields=value;
			}
		}
		///<summary>Margins will be null unless set by user.  When printing, if margins are null, the defaults will depend on the page orientation.</summary>
		public Margins ReportMargins{
			get{
				//return reportMargins; //reportMargins is always null!
				return null;
			}
		}
		///<summary></summary>
		public bool IsLandscape{
			get{
				return _isLandscape;
			}
			set{
				_isLandscape=value;
			}
		}
		///<summary>The name to display in the menu.</summary>
		public string ReportName{
			get{
				return _reportName;
			}
			set{
				_reportName=value;
			}
		}
		///<summary>Gives the user a description and some guidelines about what they can expect from this report.</summary>
		public string Description{
			get{
				return _description;
			}
			set{
				_description=value;
			}
		}
		///<summary>For instance OD12 or JoeDeveloper9.  If you are a developer releasing reports, then this should be your name or company followed by a unique number.  This will later make it easier to maintain your reports for your customers.  All reports that we release will be of the form OD##.  Reports that the user creates will have this field blank.</summary>
		public string AuthorID{
			get{
				return _authorID;
			}
			set{
				_authorID=value;
			}
		}
		///<summary>The 1-based order to show in the Letter menu, or 0 to not show in that menu.</summary>
		public int LetterOrder{
			get{
				return _letterOrder;
			}
			set{
				_letterOrder=value;
			}
		}
		///<summary>The total number of rows to print.</summary>
		public int TotalRows{
			get{
				return _totalRows;
			}
			set{
				_totalRows=value;
			}
		}
		
		#endregion

		///<summary>This can add a title, subtitle, grid lines, and page nums to the report using defaults.  If the parameters are blank or false the object will not be added.  Set showProgress false to hide the progress window from showing up when generating the report.</summary>
		public ReportComplex(bool hasGridLines,bool isLandscape,bool showProgress=true) {
			//if(showProgress) {
			//	_actionCloseReportProgress=ODProgress.Show(ODEventType.ReportComplex
			//		,typeof(ReportComplexEvent)
			//		,startingMessage: Lan.g("ReportComplex","Running Report Query...")
			//		,hasHistory:PrefC.GetBool(PrefName.ReportsShowHistory));
			//}
			_grfx=Graphics.FromImage(new Bitmap(1,1));
			_isLandscape=isLandscape;
			if(hasGridLines) {
				AddGridLines();
			}
			if(_sections[AreaSectionType.ReportHeader]==null) {
				_sections.Add(new Section(AreaSectionType.ReportHeader,0));
			}
			if(_sections[AreaSectionType.PageHeader]==null) {
				_sections.Add(new Section(AreaSectionType.PageHeader,0));
			}
			if(_sections[AreaSectionType.PageFooter]==null) {
				_sections.Add(new Section(AreaSectionType.PageFooter,0));
			}
			if(_sections[AreaSectionType.ReportFooter]==null) {
				_sections.Add(new Section(AreaSectionType.ReportFooter,0));
			}
		}

		///<summary>Adds a ReportObject, Tahoma font, 17-point and bold, to the top-center of the Report Header Section.  Should only be done once, and done before any subTitles.</summary>
		public void AddTitle(string name,string title) {
			AddTitle(name,title,new Font("Tahoma",17,FontStyle.Bold));
		}

		///<summary>Adds a ReportObject with the given font, to the top-center of the Report Header Section.  Should only be done once, and done before any subTitles.</summary>
		public void AddTitle(string name,string title,Font font){
			ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Adding Title To Report")+"...");
			SizeF sizeF=_grfx.MeasureString(title,font);
			Size size=new Size((int)(sizeF.Width/_grfx.DpiX*100+2),(int)(sizeF.Height/_grfx.DpiY*100));
			int xPos;
			if(_isLandscape) {
				xPos=1100/2;
				xPos-=50;
			}
			else {
				xPos=850/2;
				xPos-=30;
			}
			xPos-=size.Width/2;
			if(_sections[AreaSectionType.ReportHeader]==null) {
				_sections.Add(new Section(AreaSectionType.ReportHeader,0));
			}
			_reportObjects.Add(new ReportObject(name,AreaSectionType.ReportHeader,new Point(xPos,0),size,title,font,ContentAlignment.MiddleCenter));
			//this is the only place a white buffer is added to a header.
			_sections[AreaSectionType.ReportHeader].Height=size.Height+10;
		}

		///<summary>Adds a ReportObject, Tahoma font, 10-point and bold, at the bottom-center of the Report Header Section.
		///Should only be done after AddTitle.  You can add as many subtitles as you want.</summary>
		public void AddSubTitle(string name,string subTitle) {
			AddSubTitle(name,subTitle,new Font("Tahoma",10,FontStyle.Bold));
		}

		///<summary>Adds a ReportObject, Tahoma font, 10-point and bold, at the bottom-center of the Report Header Section.
		///Should only be done after AddTitle.  You can add as many subtitles as you want.  Padding is added to the height only of the subtitle.</summary>
		public void AddSubTitle(string name,string subTitle,int padding) {
			AddSubTitle(name,subTitle,new Font("Tahoma",10,FontStyle.Bold),padding);
		}

		///<summary>Adds a ReportObject with the given font, at the bottom-center of the Report Header Section.
		///Should only be done after AddTitle.  You can add as many subtitles as you want.</summary>
		public void AddSubTitle(string name,string subTitle,Font font) {
			//The original rendition of this subtitle method forced all subtitles with a padding of 5.
			//This is simply here to keep that functionality around for the majority of the reports.
			AddSubTitle(name,subTitle,font,0);
		}

		///<summary>Adds a ReportObject with the given font, at the bottom-center of the Report Header Section.
		///Should only be done after AddTitle.  You can add as many subtitles as you want.  Padding is added to the height only of the subtitle.</summary>
		public void AddSubTitle(string name,string subTitle,Font font,int padding) {
			ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Adding SubTitle To Report")+"...");
			SizeF sizeF=_grfx.MeasureString(subTitle,font);
			Size size=new Size((int)(sizeF.Width/_grfx.DpiX*100+2),(int)(sizeF.Height/_grfx.DpiY*100));
			int xPos;
			if(_isLandscape) {
				xPos=1100/2;
				xPos-=50;
			}
			else {
				xPos=850/2;
				xPos-=30;
			}
			xPos-=size.Width/2;
			if(_sections[AreaSectionType.ReportHeader]==null) {
				_sections.Add(new Section(AreaSectionType.ReportHeader,0));
			}
			//find the yPos+Height of the last reportObject in the Report Header section
			int yPos=_reportObjects.OfType<ReportObject>()
				.Where(x => x.SectionType==AreaSectionType.ReportHeader)
				.Select(x => x.Location.Y+x.Size.Height).Where(x => x>0).DefaultIfEmpty(0).Max();
			_reportObjects.Add(new ReportObject(name,AreaSectionType.ReportHeader,new Point(xPos,yPos+padding),size,subTitle,font,ContentAlignment.MiddleCenter));
			_sections[AreaSectionType.ReportHeader].Height+=size.Height+padding;
		}

		/// <summary>Adds a report object with the given font at the footer of the report, with the given alignment. </summary>
		public void AddFooterText(string name, string text, Font font, int padding, ContentAlignment contentAlign) {
			//Size size=new Size((int)(_grfx.MeasureString(text,font).Width/_grfx.DpiX*100+2)
			//	,(int)(_grfx.MeasureString(text,font).Height/_grfx.DpiY*100+2));
			Size size;
			int borderPadding;
			borderPadding=50;
			if(_isLandscape) {
				size=new Size((int)(1100 - (borderPadding * 2)),(int)(_grfx.MeasureString(text,font).Height/_grfx.DpiY*100+2));
			}
			else {
				size=new Size((int)(850  - (borderPadding * 2)),(int)(_grfx.MeasureString(text,font).Height/_grfx.DpiY*100+2));
			}
			if(_sections[AreaSectionType.ReportFooter]==null) {
				_sections.Add(new Section(AreaSectionType.ReportFooter,0));
			}
			//find the yPos+Height of the last reportObject in the Report Footer section
			int yPos=0;
			foreach(ReportObject reportObject in _reportObjects) {
				if(reportObject.SectionType!=AreaSectionType.ReportFooter) {
					continue;
				}
				if(reportObject.Location.Y+reportObject.Size.Height > yPos) {
					yPos=reportObject.Location.Y+reportObject.Size.Height;
				}
			}
			_reportObjects.Add(new ReportObject(name,AreaSectionType.ReportFooter,new Point(borderPadding,yPos+padding),size,text,font,contentAlign));
			_sections[AreaSectionType.ReportFooter].Height+=(int)size.Height+padding;
		}
		
		///<summary>Adds a Page Footer object with the given font with the given alignment. </summary>
		public void AddPageFooterText(string name,string text,Font font,int padding,ContentAlignment contentAlign) {
			Size size;
			int borderPadding;
			borderPadding=50;
			if(_isLandscape) {
				size=new Size((int)(1100 - (borderPadding * 2)),(int)(_grfx.MeasureString(text,font).Height/_grfx.DpiY*100+2));
			}
			else {
				size=new Size((int)(850  - (borderPadding * 2)),(int)(_grfx.MeasureString(text,font).Height/_grfx.DpiY*100+2));
			}
			if(_sections[AreaSectionType.PageFooter]==null) {
				_sections.Add(new Section(AreaSectionType.PageFooter,0));
			}
			//find the yPos+Height of the last reportObject in the Page Footer section
			int yPos=0;
			foreach(ReportObject reportObject in _reportObjects) {
				if(reportObject.SectionType!=AreaSectionType.PageFooter) {
					continue;
				}
				if(reportObject.Location.Y+reportObject.Size.Height > yPos) {
					yPos=reportObject.Location.Y+reportObject.Size.Height;
				}
			}
			_reportObjects.Add(new ReportObject(name,AreaSectionType.PageFooter,new Point(borderPadding,yPos+padding),size,text,font,contentAlign));
			_sections[AreaSectionType.PageFooter].Height+=(int)size.Height+padding;
		}

		public QueryObject AddQuery(string query,string title) {
			QueryObject queryObj=new QueryObject(title,stringQuery:query);
			_reportObjects.Add(queryObj);
			return queryObj;
		}

		public QueryObject AddQuery(string query,string title,string columnNameToSplitOn,SplitByKind splitByKind) {
			QueryObject queryObj=new QueryObject(title,stringQuery:query,columnNameToSplitOn:columnNameToSplitOn,splitByKind:splitByKind);
			_reportObjects.Add(queryObj);
			return queryObj;
		}

		public QueryObject AddQuery(string query,string title,string columnNameToSplitOn,SplitByKind splitByKind,int queryGroup) {
			QueryObject queryObj=new QueryObject(title,stringQuery:query,queryGroupValue:queryGroup,columnNameToSplitOn:columnNameToSplitOn,splitByKind:splitByKind);
			_reportObjects.Add(queryObj);
			return queryObj;
		}

		public QueryObject AddQuery(string query,string title,string columnNameToSplitOn,SplitByKind splitByKind,int queryGroup,bool isCentered) {
			QueryObject queryObj=new QueryObject(title,stringQuery:query,isCentered:isCentered,queryGroupValue:queryGroup,columnNameToSplitOn:columnNameToSplitOn,splitByKind:splitByKind);
			_reportObjects.Add(queryObj);
			return queryObj;
		}

		public QueryObject AddQuery(string query,string title,string columnNameToSplitOn,SplitByKind splitByKind,int queryGroup,bool isCentered,List<string> enumNames,Font font) {
			QueryObject queryObj=new QueryObject(title,stringQuery:query,font:font,isCentered:isCentered,queryGroupValue:queryGroup,columnNameToSplitOn:columnNameToSplitOn,splitByKind:splitByKind,listEnumNames:enumNames,dictDefNames:null);
			_reportObjects.Add(queryObj);
			return queryObj;
		}

		public QueryObject AddQuery(string query,string title,string columnNameToSplitOn,SplitByKind splitByKind,int queryGroup,bool isCentered,Dictionary<long,string> dictDefNames,Font font) {
			QueryObject queryObj=new QueryObject(title,stringQuery:query,font:font,isCentered:isCentered,queryGroupValue:queryGroup,columnNameToSplitOn:columnNameToSplitOn,splitByKind:splitByKind,listEnumNames:null,dictDefNames:dictDefNames);
			_reportObjects.Add(queryObj);
			return queryObj;
		}

		public QueryObject AddQuery(DataTable query,string title) {
			QueryObject queryObj=new QueryObject(title,query);
			_reportObjects.Add(queryObj);
			return queryObj;
		}

		public QueryObject AddQuery(DataTable query,string title,string columnNameToSplitOn,SplitByKind splitByKind,int queryGroup) {
			QueryObject queryObj=new QueryObject(title,query,queryGroupValue:queryGroup,columnNameToSplitOn:columnNameToSplitOn,splitByKind:splitByKind);
			_reportObjects.Add(queryObj);
			return queryObj;
		}

		public QueryObject AddQuery(DataTable query,string title,string columnNameToSplitOn,SplitByKind splitByKind,int queryGroup,bool isCentered) {
			QueryObject queryObj=new QueryObject(title,query,isCentered:isCentered,queryGroupValue:queryGroup,columnNameToSplitOn:columnNameToSplitOn,splitByKind:splitByKind);
			_reportObjects.Add(queryObj);
			return queryObj;
		}

		public QueryObject AddQuery(DataTable query,string title,string columnNameToSplitOn,SplitByKind splitByKind,int queryGroup,bool isCentered,List<string> enumNames,Font font) {
			QueryObject queryObj=new QueryObject(title,query,font:font,isCentered:isCentered,queryGroupValue:queryGroup,columnNameToSplitOn:columnNameToSplitOn,splitByKind:splitByKind,listEnumNames:enumNames,dictDefNames:null);
			_reportObjects.Add(queryObj);
			return queryObj;
		}

		public QueryObject AddQuery(DataTable query,string title,string columnNameToSplitOn,SplitByKind splitByKind,int queryGroup,bool isCentered,Dictionary<long,string> dictDefNames,Font font) {
			QueryObject queryObj=new QueryObject(title,query,font:font,isCentered:isCentered,queryGroupValue:queryGroup,columnNameToSplitOn:columnNameToSplitOn,splitByKind:splitByKind,listEnumNames:null,dictDefNames:dictDefNames);
			_reportObjects.Add(queryObj);
			return queryObj;
		}

		/// <summary></summary>
		public void AddLine(string name,AreaSectionType sectionType,Color color,float lineThickness,LineOrientation lineOrientation,LinePosition linePosition,int linePercent,int offSetX,int offSetY) {
			_reportObjects.Add(new ReportObject(name,sectionType,color,lineThickness,lineOrientation,linePosition,linePercent,offSetX,offSetY));
		}

		/// <summary></summary>
		public void AddBox(string name,AreaSectionType sectionType,Color color,float lineThickness,int offSetX,int offSetY) {
			_reportObjects.Add(new ReportObject(name,sectionType,color,lineThickness,offSetX,offSetY));
		}

		public ReportObject GetObjectByName(string name){
			for(int i=_reportObjects.Count-1;i>=0;i--){//search from the end backwards
				if(_reportObjects[i].Name==name) {
					return ReportObjects[i];
				}
			}
			MessageBox.Show("end of loop");
			return null;
		}

		public ReportObject GetTitle(string name) {
			//ReportObject ro=null;
			for(int i=_reportObjects.Count-1;i>=0;i--) {//search from the end backwards
				if(_reportObjects[i].Name==name) {
					return ReportObjects[i];
				}
			}
			MessageBox.Show("end of loop");
			return null;
		}

		public ReportObject GetSubTitle(string subName) {
			//ReportObject ro=null;
			for(int i=_reportObjects.Count-1;i>=0;i--) {//search from the end backwards
				if(_reportObjects[i].Name==subName) {
					return ReportObjects[i];
				}
			}
			MessageBox.Show("end of loop");
			return null;
		}

		public void AddPageNum() {
			AddPageNum(new Font("Tahoma",9));
		}
		/// <summary>Put a pagenumber object on lower left of page footer section. Object is named PageNum.</summary>
		public void AddPageNum(Font font){
			//add page number
			Size size=new Size(150,(int)(_grfx.MeasureString("anytext",font).Height/_grfx.DpiY*100+2));
			if(_sections[AreaSectionType.PageFooter]==null) {
				_sections.Add(new Section(AreaSectionType.PageFooter,0));	
			}
			if(_sections[AreaSectionType.PageFooter].Height==0) {
				_sections[AreaSectionType.PageFooter].Height=size.Height;
			}
			_reportObjects.Add(new ReportObject("PageNum",AreaSectionType.PageFooter
				,new Point(0,0),size
				,FieldValueType.String,SpecialFieldType.PageNumber
				,font,ContentAlignment.MiddleLeft,""));
		}

		public void AddGridLines() {
			_hasGridLines=true;
		}

		///<summary>Adds a parameterField which will be used in the query to represent user-entered data.</summary>
		///<param name="myName">The unique formula name of the parameter.</param>
		///<param name="myValueType">The data type that this parameter stores.</param>
		///<param name="myDefaultValue">The default value of the parameter</param>
		///<param name="myPromptingText">The text to prompt the user with.</param>
		///<param name="mySnippet">The SQL snippet that this parameter represents.</param>
		public void AddParameter(string myName,FieldValueType myValueType
			,object myDefaultValue,string myPromptingText,string mySnippet){
			_parameterFields.Add(new ParameterField(myName,myValueType,myDefaultValue,myPromptingText,mySnippet));
		}

		/// <summary>Overload for ValueKind enum.</summary>
		public void AddParameter(string myName,FieldValueType myValueType
			,ArrayList myDefaultValues,string myPromptingText,string mySnippet,EnumType myEnumerationType){
			_parameterFields.Add(new ParameterField(myName,myValueType,myDefaultValues,myPromptingText,mySnippet,myEnumerationType));
		}

		/// <summary>Overload for ValueKind defCat.</summary>
		public void AddParameter(string myName,FieldValueType myValueType
			,ArrayList myDefaultValues,string myPromptingText,string mySnippet,DefCat myDefCategory){
			_parameterFields.Add(new ParameterField(myName,myValueType,myDefaultValues,myPromptingText,mySnippet,myDefCategory));
		}

		/// <summary>Overload for ValueKind defCat.</summary>
		public void AddParameter(string myName,FieldValueType myValueType
			,ArrayList myDefaultValues,string myPromptingText,string mySnippet,ReportFKType myReportFKType){
			_parameterFields.Add(new ParameterField(myName,myValueType,myDefaultValues,myPromptingText,mySnippet,myReportFKType));
		}
		
		///<summary>Submits the queries to the database and makes query objects for each query with the results.  Returns false if one of the queries failed.</summary>
		public bool SubmitQueries(bool isShowMessage=false){
			bool hasRows=false;
			bool hasReportServer=!string.IsNullOrEmpty(PrefC.ReportingServer.DisplayStr);
			Graphics grfx=Graphics.FromImage(new Bitmap(1,1));
			string displayText;
			ReportObjectCollection newReportObjects=new ReportObjectCollection();
			_sections.Add(new Section(AreaSectionType.Query,0));
			for(int i=0;i<_reportObjects.Count;i++) {
				if(_reportObjects[i].ObjectType==ReportObjectType.QueryObject) {
					QueryObject query=(QueryObject)_reportObjects[i];
					bool wasSubmitted=false;
					ProgressOD progressOD=new ProgressOD();
					progressOD.ActionMain=() => { 
						wasSubmitted=query.SubmitQuery();
						};
					progressOD.ShowDialogProgress();
					if(!wasSubmitted) {
						MsgBox.Show(this,"There was an error generating this report."
							+ (hasReportServer ? "\r\nVerify or remove the report server connection settings and try again." : ""));
						return false;
					}
					if(progressOD.IsCancelled){
						return false;
					}
					if(query.ReportTable.Rows.Count==0) {
						continue;
					}
					hasRows=true;
					TotalRows+=query.ReportTable.Rows.Count;
					//Check if the query needs to be split up into sub queries.  E.g. one payment report query split up via payment type.
					if(!String.IsNullOrWhiteSpace(query.ColumnNameToSplitOn)) { 
						ReportComplexEvent.Fire(ODEventType.ReportComplex,Lan.g("ReportComplex","Creating Splits Based On")+" "+query.ColumnNameToSplitOn+"...");
						//The query needs to be split up into sub queries every time the ColumnNameToSplitOn cell changes.  
						//Therefore, we need to create a separate QueryObject for every time the cell value changes.
						string lastCellValue="";
						query.IsLastSplit=false;
						QueryObject newQuery=null;
						for(int j=0;j<query.ReportTable.Rows.Count;j++) {
							if(query.ReportTable.Rows[j][query.ColumnNameToSplitOn].ToString()==lastCellValue) {
								if(newQuery==null) {
									newQuery=query.DeepCopyQueryObject();
									newQuery.AddInitialHeader(newQuery.GetGroupTitle().StaticText,newQuery.GetGroupTitle().Font);
								}
								newQuery.ReportTable.ImportRow(query.ReportTable.Rows[j]);
							}
							else {
								//Must happen the first time through
								if(newQuery!=null) {
									switch(newQuery.SplitByKind) {
										case SplitByKind.None:
											return false;
										case SplitByKind.Enum:
											if(newQuery.ListEnumNames==null) {
												return false;
											}
											displayText=newQuery.ListEnumNames[PIn.Int(newQuery.ReportTable.Rows[0][query.ColumnNameToSplitOn].ToString())];
											newQuery.GetGroupTitle().Size=new Size((int)(grfx.MeasureString(displayText,newQuery.GetGroupTitle().Font).Width/grfx.DpiX*100+2),(int)(grfx.MeasureString(displayText,newQuery.GetGroupTitle().Font).Height/grfx.DpiY*100+2));
											newQuery.GetGroupTitle().StaticText=displayText;
											break;
										case SplitByKind.Definition:
											if(newQuery.DictDefNames==null) {
												return false;
											}
											if(newQuery.DictDefNames.ContainsKey(PIn.Long(newQuery.ReportTable.Rows[0][query.ColumnNameToSplitOn].ToString()))) {
												displayText=newQuery.DictDefNames[PIn.Long(newQuery.ReportTable.Rows[0][query.ColumnNameToSplitOn].ToString())];
												newQuery.GetGroupTitle().Size=new Size((int)(grfx.MeasureString(displayText,newQuery.GetGroupTitle().Font).Width/grfx.DpiX*100+2),(int)(grfx.MeasureString(displayText,newQuery.GetGroupTitle().Font).Height/grfx.DpiY*100+2));
												newQuery.GetGroupTitle().StaticText=displayText;
											}
											else {
												newQuery.GetGroupTitle().StaticText="Undefined";
											}
											break;
										case SplitByKind.Date:
											displayText=PIn.Date(newQuery.ReportTable.Rows[0][query.ColumnNameToSplitOn].ToString()).ToShortDateString();
											newQuery.GetGroupTitle().StaticText=displayText;
											newQuery.GetGroupTitle().Size=new Size((int)(grfx.MeasureString(displayText,newQuery.GetGroupTitle().Font).Width/grfx.DpiX*100+2),(int)(grfx.MeasureString(displayText,newQuery.GetGroupTitle().Font).Height/grfx.DpiY*100+2));
											break;
										case SplitByKind.Value:
											displayText=newQuery.ReportTable.Rows[0][query.ColumnNameToSplitOn].ToString();
											newQuery.GetGroupTitle().StaticText=displayText;
											newQuery.GetGroupTitle().Size=new Size((int)(grfx.MeasureString(displayText,newQuery.GetGroupTitle().Font).Width/grfx.DpiX*100+2),(int)(grfx.MeasureString(displayText,newQuery.GetGroupTitle().Font).Height/grfx.DpiY*100+2));
											break;
									}
									newQuery.SubmitQuery();
									newReportObjects.Add(newQuery);
								}
								if(newQuery==null && query.GetGroupTitle().StaticText!="") {
									newQuery=query.DeepCopyQueryObject();
									newQuery.ReportTable.ImportRow(query.ReportTable.Rows[j]);
									newQuery.AddInitialHeader(newQuery.GetGroupTitle().StaticText,newQuery.GetGroupTitle().Font);
								}
								else {
									newQuery=query.DeepCopyQueryObject();
									newQuery.ReportTable.ImportRow(query.ReportTable.Rows[j]);
								}
							}
							lastCellValue=query.ReportTable.Rows[j][query.ColumnNameToSplitOn].ToString();
						}
						switch(newQuery.SplitByKind) {
							case SplitByKind.None:
								return false;
							case SplitByKind.Enum:
								if(newQuery.ListEnumNames==null) {
									return false;
								}
								displayText=newQuery.ListEnumNames[PIn.Int(newQuery.ReportTable.Rows[0][query.ColumnNameToSplitOn].ToString())];
								newQuery.GetGroupTitle().Size=new Size((int)(grfx.MeasureString(displayText,newQuery.GetGroupTitle().Font).Width/grfx.DpiX*100+2),(int)(grfx.MeasureString(displayText,newQuery.GetGroupTitle().Font).Height/grfx.DpiY*100+2));
								newQuery.GetGroupTitle().StaticText=displayText;
								break;
							case SplitByKind.Definition:
								if(newQuery.DictDefNames==null) {
									return false;
								}
								if(newQuery.DictDefNames.ContainsKey(PIn.Long(newQuery.ReportTable.Rows[0][query.ColumnNameToSplitOn].ToString()))) {
									displayText=newQuery.DictDefNames[PIn.Long(newQuery.ReportTable.Rows[0][query.ColumnNameToSplitOn].ToString())];
									newQuery.GetGroupTitle().Size=new Size((int)(grfx.MeasureString(displayText,newQuery.GetGroupTitle().Font).Width/grfx.DpiX*100+2),(int)(grfx.MeasureString(displayText,newQuery.GetGroupTitle().Font).Height/grfx.DpiY*100+2));
									newQuery.GetGroupTitle().StaticText=displayText;
								}
								else {
									newQuery.GetGroupTitle().StaticText=Lans.g(this,"Undefined");
								}
								break;
							case SplitByKind.Date:
								newQuery.GetGroupTitle().StaticText=PIn.Date(newQuery.ReportTable.Rows[0][query.ColumnNameToSplitOn].ToString()).ToShortDateString();
								break;
							case SplitByKind.Value:
								newQuery.GetGroupTitle().StaticText=newQuery.ReportTable.Rows[0][query.ColumnNameToSplitOn].ToString();
								break;
						}
						newQuery.SubmitQuery();
						newQuery.IsLastSplit=true;
						newReportObjects.Add(newQuery);
					}
					else {
						newReportObjects.Add(_reportObjects[i]);
					}
				}
				else {
					newReportObjects.Add(_reportObjects[i]);
				}
			}
			if(!hasRows && isShowMessage) {
				MsgBox.Show(this,"The report has no results to show.");
				return false;
			}
			_reportObjects=newReportObjects;
			return true;
		}

		///<summary>If the specified section exists, then this returns its height. Otherwise it returns 0.</summary>
		public int GetSectionHeight(AreaSectionType sectionType) {
			if(!_sections.Contains(sectionType)) {
				return 0;
			}
			return _sections[sectionType].Height;
		}

		public bool HasGridLines() {
			return _hasGridLines;
		}

		///<summary>Closes the progress bar if it is open.</summary>
		public void CloseProgressBar() {
			//_actionCloseReportProgress?.Invoke();
		}

	}
}











