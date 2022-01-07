using CodeBase;
using OpenDentBusiness.FileIO;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDentBusiness{
	public class SheetPrinting{
		private static Margins _printMargin=new Margins(0,0,40,60);//jordan static only because it's an unchanging val.

		#region Methods - Drawing
		public static void DrawFieldSpecial(Sheet sheet,SheetField field,Graphics g,XGraphics gx,int yPosPrint) {
			switch(field.FieldName) {
				case "toothChart":
					Image toothChart=(Image)SheetParameter.GetParamByName(sheet.Parameters,"toothChartImg").ParamValue;
					DrawScaledImage(field.XPos,field.YPos-yPosPrint,field.Width,field.Height,g,gx,toothChart);
					break;
				case "toothChartLegend":
					List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors,true);
					DrawToothChartLegend(field.XPos,field.YPos,sheet.Width,yPosPrint,listDefs,g,gx);
					break;
				default:
					//do nothing
					break;
			}
		}

		///<summary>Uses dimension parameters to scale and draw an image within their bounds.</summary>
		public static Rectangle DrawScaledImage(int x,int y,int width,int height,Graphics g,XGraphics gx,Image img) {
			Rectangle rectBoundingBox=GetBoundingBox(x,y,width,height,img.Width,img.Height);
			if(gx==null) {
				g.DrawImage(img,rectBoundingBox);
			}
			else {
				gx.DrawImage(XImage.FromGdiPlusImage(img)
					,new Rectangle((int)p(rectBoundingBox.X),(int)p(rectBoundingBox.Y),(int)p(rectBoundingBox.Width),(int)p(rectBoundingBox.Height)));
			}
			return rectBoundingBox;
		}

		///<summary>Draws the legend for the toothchart using the supplied dimesions, definitions, and graphics.</summary>
		public static void DrawToothChartLegend(int x,int y,int width,int yPosPrint,List<Def> listDefs,Graphics g,XGraphics gx,bool isInDashboard=false) {
			using(Brush brushEx=new SolidBrush(listDefs[3].ItemColor))
			using(Brush brushEc=new SolidBrush(listDefs[2].ItemColor))
			using(Brush brushCo=new SolidBrush(listDefs[1].ItemColor))
			using(Brush brushRo=new SolidBrush(listDefs[4].ItemColor))
			using(Brush brushTp=new SolidBrush(listDefs[0].ItemColor))
			using(Font bodyFont=new Font("Arial",9f,FontStyle.Regular,GraphicsUnit.Point))
			if(gx==null) {
				float yPos=y-yPosPrint;
				float xPos;
				if(isInDashboard) {
					xPos=x;
				}
				else {
					//Always centered on page.
					xPos=0.5f*(width-
														(TextRenderer.MeasureText(Lans.g("ContrTreat","Existing"),bodyFont).Width
														+TextRenderer.MeasureText(Lans.g("ContrTreat","Complete"),bodyFont).Width
														+TextRenderer.MeasureText(Lans.g("ContrTreat","Referred Out"),bodyFont).Width
														+TextRenderer.MeasureText(Lans.g("ContrTreat","Treatment Planned"),bodyFont).Width
														+123)); //inter-field spacing
				}
				g.FillRectangle(Brushes.White,new Rectangle((int)xPos,y-yPosPrint,width-2*(int)xPos+10,14)); //buffer the image for smooth drawing.
				//Existing
				g.FillRectangle(brushEx,xPos,yPos,14,14);
				g.DrawString(Lans.g("ContrTreat","Existing"),bodyFont,Brushes.Black,xPos+16,yPos);
				xPos+=TextRenderer.MeasureText(Lans.g("ContrTreat","Existing"),bodyFont).Width+23+16;
				//Complete/ExistingComplete
				g.FillRectangle(brushCo,xPos,yPos,7,14);
				g.FillRectangle(brushEc,xPos+7,yPos,7,14);
				g.DrawString(Lans.g("ContrTreat","Complete"),bodyFont,Brushes.Black,xPos+16,yPos);
				xPos+=TextRenderer.MeasureText(Lans.g("ContrTreat","Complete"),bodyFont).Width+23+16;
				//ReferredOut
				g.FillRectangle(brushRo,xPos,yPos,14,14);
				g.DrawString(Lans.g("ContrTreat","Referred Out"),bodyFont,Brushes.Black,xPos+16,yPos);
				xPos+=TextRenderer.MeasureText(Lans.g("ContrTreat","Referred Out"),bodyFont).Width+23+16;
				//TreatmentPlanned
				g.FillRectangle(brushTp,xPos,yPos,14,14);
				g.DrawString(Lans.g("ContrTreat","Treatment Planned"),bodyFont,Brushes.Black,xPos+16,yPos);
			}
			else {
				XFont bodyFontX;
				if(string.IsNullOrEmpty(bodyFont.SystemFontName)) {
					bodyFontX=new Font("Arial",9f,FontStyle.Regular,GraphicsUnit.World);
				}
				else {
					bodyFontX=new XFont(bodyFont.SystemFontName,bodyFont.Size,XFontStyle.Regular);
				}
				float yPos=y-yPosPrint;
				float xPos;
				if(isInDashboard) {
					xPos = x;
				}
				else {
					//Always centered on page.
					xPos=0.5f*(width-
							          (TextRenderer.MeasureText(Lans.g("ContrTreat","Existing"),bodyFont).Width
							          +TextRenderer.MeasureText(Lans.g("ContrTreat","Complete"),bodyFont).Width
							          +TextRenderer.MeasureText(Lans.g("ContrTreat","Referred Out"),bodyFont).Width
							          +TextRenderer.MeasureText(Lans.g("ContrTreat","Treatment Planned"),bodyFont).Width
							          +123)); //inter-field spacing
				}
				gx.DrawRectangle(XBrushes.White,new RectangleF((float)p(xPos),(float)p(y-yPosPrint),(float)p(width-2*xPos+10),(float)p(14))); //buffer the image for smooth drawing.
				//Existing
				gx.DrawRectangle(brushEx,p(xPos),p(yPos),p(14),p(14));
				GraphicsHelper.DrawStringX(gx,Lans.g("ContrTreat","Existing"),bodyFontX,XBrushes.Black,
					new RectangleF(xPos+16,yPos-1,TextRenderer.MeasureText(Lans.g("ContrTreat","Existing"),bodyFont).Width,14),HorizontalAlignment.Left);
				//gx.DrawString(Lans.g("ContrTreat","Existing"),bodyFontX,Brushes.Black,p(xPos+16),p(yPos));
				xPos+=TextRenderer.MeasureText(Lans.g("ContrTreat","Existing"),bodyFont).Width+23+16;
				//Complete/ExistingComplete
				gx.DrawRectangle(brushCo,p(xPos),p(yPos),p(7),p(14));
				gx.DrawRectangle(brushEc,p(xPos+7),p(yPos),p(7),p(14));
				GraphicsHelper.DrawStringX(gx,Lans.g("ContrTreat","Complete"),bodyFontX,XBrushes.Black,
					new RectangleF(xPos+16,yPos-1,TextRenderer.MeasureText(Lans.g("ContrTreat","Complete"),bodyFont).Width,14),HorizontalAlignment.Left);
				//gx.DrawString(Lans.g("ContrTreat","Complete"),bodyFontX,Brushes.Black,p(xPos+16),p(yPos));
				xPos+=TextRenderer.MeasureText(Lans.g("ContrTreat","Complete"),bodyFont).Width+23+16;
				//ReferredOut
				gx.DrawRectangle(brushRo,p(xPos),p(yPos),p(14),p(14));
				GraphicsHelper.DrawStringX(gx,Lans.g("ContrTreat","Referred Out"),bodyFontX,XBrushes.Black,
					new RectangleF(xPos+16,yPos-1,TextRenderer.MeasureText(Lans.g("ContrTreat","Referred Out"),bodyFont).Width,14),HorizontalAlignment.Left);
				//gx.DrawString(Lans.g("ContrTreat","Referred Out"),bodyFontX,Brushes.Black,p(xPos+16),p(yPos));
				xPos+=TextRenderer.MeasureText(Lans.g("ContrTreat","Referred Out"),bodyFont).Width+23+16;
				//TreatmentPlanned
				gx.DrawRectangle(brushTp,p(xPos),p(yPos),p(14),p(14));
				GraphicsHelper.DrawStringX(gx,Lans.g("ContrTreat","Treatment Planned"),bodyFontX,XBrushes.Black,
					new RectangleF(xPos+16,yPos-1,TextRenderer.MeasureText(Lans.g("ContrTreat","Treatment Planned"),bodyFont).Width,14),HorizontalAlignment.Left);
				//gx.DrawString(Lans.g("ContrTreat","Treatment Planned"),bodyFontX,Brushes.Black,p(xPos+16),p(yPos));
			}
		}

		///<summary>Draws all images from the sheet onto the graphic passed in.  Used when printing, exporting to pdfs, or rendering the sheet fill edit window.  graphic should be null for pdfs and xgraphic should be null for printing and rendering the sheet fill edit window.</summary>
		public static void DrawImages(Sheet sheet,Graphics graphic,bool drawAll,ref int yPosPrint) {
			XGraphics xGraphic=null;
			Sheets.SetPageMargin(sheet,_printMargin);
			Bitmap bmpOriginal=null;
			if(drawAll){// || _forceSinglePage) {//reset _yPosPrint because we are drawing all.
				yPosPrint=0;
			}
			foreach(SheetField field in sheet.SheetFields) {
				if(!drawAll ){//&& !_forceSinglePage) {
					if(field.YPos<yPosPrint) {
						continue; //skip if on previous page
					}
					if(field.Bounds.Bottom>yPosPrint+sheet.HeightPage-_printMargin.Bottom
						&& field.YPos!= yPosPrint+_printMargin.Top) {
						break; //Skip if on next page
					} 
				}
				if(field.Height==0 || field.Width==0) {
					continue;//might be possible with really old sheets.
				}
				#region Get the path for the image
				string filePathAndName="";
				switch(field.FieldType) {
					case SheetFieldType.Image:
						filePathAndName=FileAtoZ.CombinePaths(SheetUtil.GetClinicImagePath(field.FieldName),field.FieldName);
						break;
					case SheetFieldType.PatImage:
						if(field.FieldValue=="") {
							//There is no document object to use for display, but there may be a baked in image and that situation is dealt with below.
							filePathAndName="";
							break;
						}
						Document patDoc=Documents.GetByNum(PIn.Long(field.FieldValue));
						List<string> paths=Documents.GetPaths(new List<long> { patDoc.DocNum },ImageStore.GetPreferredAtoZpath());
						if(paths.Count < 1) {//No path was found so we cannot draw the image.
							continue;
						}
						filePathAndName=paths[0];
						break;
					default:
						//not an image field
						continue;
				}
				#endregion
				#region Load the image into bmpOriginal
				if(field.FieldName=="Patient Info.gif") {
					bmpOriginal=OpenDentBusiness.Properties.Resources.Patient_Info;
				}
				else if(CloudStorage.IsCloudStorage) {
					try {
						bmpOriginal=FileAtoZ.GetImage(filePathAndName);
						if(bmpOriginal==null) {
							continue;
						}
					}
					catch(Exception ex) {
						ex.DoNothing();
						continue;//If the image is not an actual image file, leave the image field blank.
					}
				}
				else if(File.Exists(filePathAndName)) {//Local AtoZ
					try {
						bmpOriginal=new Bitmap(filePathAndName);
					}
					catch {
						continue;//If the image is not an actual image file, leave the image field blank.
					}
				}
				else {
					continue;
				}
				#endregion
				#region Calculate the image ratio and location, set values for imgDrawWidth and imgDrawHeight
				//inscribe image in field while maintaining aspect ratio.
				float imgRatio=(float)bmpOriginal.Width/(float)bmpOriginal.Height;
				float fieldRatio=(float)field.Width/(float)field.Height;
				float imgDrawHeight=field.Height;//drawn size of image
				float imgDrawWidth=field.Width;//drawn size of image
				int adjustY=0;//added to YPos
				int adjustX=0;//added to XPos
				//For patient images, we need to make sure the images will fit and can maintain aspect ratio.
				if(field.FieldType==SheetFieldType.PatImage && imgRatio>fieldRatio) {//image is too wide
					//X pos and width of field remain unchanged
					//Y pos and height must change
					imgDrawHeight=(float)bmpOriginal.Height*((float)field.Width/(float)bmpOriginal.Width);//img.Height*(width based scale) This also handles images that are too small.
					adjustY=(int)((field.Height-imgDrawHeight)/2f);//adjustY= half of the unused vertical field space
				}
				else if(field.FieldType==SheetFieldType.PatImage && imgRatio<fieldRatio) {//image is too tall
					//X pos and width must change
					//Y pos and height remain unchanged
					imgDrawWidth=(float)bmpOriginal.Width*((float)field.Height/(float)bmpOriginal.Height);//img.Height*(width based scale) This also handles images that are too small.
					adjustX=(int)((field.Width-imgDrawWidth)/2f);//adjustY= half of the unused horizontal field space
				}
				else {//image ratio == field ratio
					//do nothing
				}
				#endregion
				#region Draw the image
				if(xGraphic!=null) {//Drawing an image to a pdf.
					XImage xI=XImage.FromGdiPlusImage((Bitmap)bmpOriginal.Clone());
					xGraphic.DrawImage(xI,p(field.XPos+adjustX),p(field.YPos-yPosPrint+adjustY),p(imgDrawWidth),p(imgDrawHeight));
					if(xI!=null) {//should always happen
						xI.Dispose();
						xI=null;
					}
				}
				else if(graphic!=null) {//Drawing an image to a printer or the sheet fill edit window.
					graphic.DrawImage(bmpOriginal,field.XPos+adjustX,field.YPos+adjustY-yPosPrint,imgDrawWidth,imgDrawHeight);
				}
				#endregion
			}
			if(bmpOriginal!=null) {
				bmpOriginal.Dispose();
				bmpOriginal=null;
			}
		}

		public static Rectangle GetBoundingBox(int xPos,int yPos,int fieldWidth,int fieldHeight,int contrWidth,int contrHeight) {
			Rectangle boundingBox=new Rectangle(xPos,yPos,fieldWidth,fieldHeight);
			float widthFactor=(float)boundingBox.Width/(float)contrWidth;
			float heightFactor=(float)boundingBox.Height/(float)contrHeight;
			int x,y,width,height;
			if(widthFactor<heightFactor) {
				//use width factor
				//img width will equal box width
				//offset height.
				x=xPos;
				y=yPos+(fieldHeight-(int)(contrHeight*widthFactor))/2;
				height=(int)(contrHeight*widthFactor);
				width=fieldWidth+1; //+1 to include the pixels
			}
			else {
				//use height factor
				//img height will equal box height
				//offset width
				x=xPos+(fieldWidth-(int)(contrWidth*heightFactor))/2;
				y=yPos;
				height=fieldHeight+1;
				width=(int)(contrWidth*heightFactor);
			}
			return new Rectangle(x,y,width,height);
		}
		#endregion Methods - Drawing

		#region Methods - Public
		public static int CompareProcListFiltered(Procedure proc1,Procedure proc2) {
			if(proc1.ProcDate!=proc2.ProcDate) {
				return proc1.ProcDate.CompareTo(proc2.ProcDate);
			}
			return GetProcStatusIdx(proc1.ProcStatus).CompareTo(GetProcStatusIdx(proc2.ProcStatus));
		}

		///<summary>Returns a subset of listProceduresAll based on ProcStatus and treatPlan data.</summary>
		public static List<Procedure> FilterProceduresForToothChart(List<Procedure> listProceduresAll,TreatPlan treatPlan,bool showCompleted) {
			if(listProceduresAll==null) {
				return null;
			}
			//always show referred and conditions
			List<Procedure> listProceduresFiltered=listProceduresAll.FindAll(x => new[] { ProcStat.R,ProcStat.Cn }.Contains(x.ProcStatus));
			if(showCompleted) {
				listProceduresFiltered.AddRange(listProceduresAll.FindAll(x => new[] {ProcStat.C,ProcStat.EC,ProcStat.EO}.Contains(x.ProcStatus)));//show complete
			}
			if(treatPlan!=null) {
				foreach(ProcTP procTP in treatPlan.ListProcTPs) {//Add procs for TP.
					Procedure procDummy=listProceduresAll.FirstOrDefault(x => x.ProcNum==procTP.ProcNumOrig)??new Procedure();
					if(Tooth.IsValidEntry(procTP.ToothNumTP)) {
						procDummy.ToothNum=Tooth.FromInternat(procTP.ToothNumTP);
					}
					if(ProcedureCodes.GetProcCode(procTP.ProcCode).TreatArea==TreatmentArea.Surf) {
						procDummy.Surf=Tooth.SurfTidyFromDisplayToDb(procTP.Surf,procDummy.ToothNum);
					}
					else {
						procDummy.Surf=procTP.Surf;//for quad, arch, etc.
					}
					if(procDummy.ToothRange==null) {
						procDummy.ToothRange="";
					}
					procDummy.ProcStatus=ProcStat.TP;
					procDummy.CodeNum=ProcedureCodes.GetProcCode(procTP.ProcCode).CodeNum;
					listProceduresFiltered.Add(procDummy);
				}
			}
			return listProceduresFiltered;
		}

		///<summary>Returns index for sorting based on this order: Cn,TP,R,EO,EC,C,D</summary>
		private static int GetProcStatusIdx(ProcStat procStat) {
			switch(procStat) {
				case ProcStat.Cn:
					return 0;
				case ProcStat.TP:
					return 1;
				case ProcStat.R:
					return 2;
				case ProcStat.EO:
					return 3;
				case ProcStat.EC:
					return 4;
				case ProcStat.C:
					return 5;
				case ProcStat.D:
					return 6;
			}
			return 0;
		}
		#endregion Methods - Public

		#region Methods - Private
		///<summary>Deprecated: See GraphicsHelper.PixelsToPoints().  Converts pixels used by us to points used by PdfSharp.</summary>
		private static double p(int pixels){
			XUnit xunit=XUnit.FromInch((double)pixels/100d);//100 ppi
			return xunit.Point;
			//XUnit.FromInch((double)pixels/100);
		}

		///<summary>Deprecated: See GraphicsHelper.PixelsToPoints().  Converts pixels used by us to points used by PdfSharp.</summary>
		private static double p(float pixels){
			XUnit xunit=XUnit.FromInch((double)pixels/100d);//100 ppi
			return xunit.Point;
		}
		#endregion Methods - Private

		///<summary>Calculates the bottom of the current page assuming a 40px top margin (except for MedLabResults sheets which have a 120 top margin) and 60px bottom margin.</summary>
		public static int BottomCurPage(int yPos,Sheet sheet,out int pageCount) {
			Sheets.SetPageMargin(sheet,_printMargin);
			pageCount=Sheets.CalculatePageCount(sheet,_printMargin);
			if(pageCount==1 && sheet.SheetType!=SheetTypeEnum.MedLabResults) {
				return sheet.HeightPage;
			}
			int retVal=sheet.HeightPage-_printMargin.Bottom;//First page bottom is not changed by top margin. Example: 1100px page height, 60px bottom, 1040px is first page bottom
			pageCount=1;
			while(retVal<yPos){
				pageCount++;
				//each page bottom after the first, 1040px is first page break+1100px page height-top margin-bottom margin=2040px if top is 40px, 1960 if top is 120px
				retVal+=sheet.HeightPage-_printMargin.Bottom-_printMargin.Top;
			}
			return retVal;
		}



	}
}
