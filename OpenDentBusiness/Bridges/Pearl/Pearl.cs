using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using CodeBase;
using System.Web;
using OpenDentBusiness.Pearl;

namespace OpenDentBusiness.Bridges{
	/// <summary></summary>
	public class Pearl:IDisposable{
/*
Pearl result:
		annotations (caries, bone loss, etc)
				category (possible example Caries)
				category_id (
				contourBox (x,y,w,h) (example green square outline)
				polygon (array x,y coords)
				relationships (rows of text in the pink boxes)
						prop_value
						metric_value
				line_segment (x1,x2,y1,y2,size)
				text (x,y,text)
		toothParts (always just background like enamel or dentin. No analysis)
				contour (x,y,w,h) (alternative to polygon)
				polygon (array x,y coords)
				color (including alpha)

Examples:
1. ToothParts.
		Background colors on teeth like dentin, enamel, and bone come in as toothParts. 
		We store them as ImageDraw polygons with some transparency.
		They must not contain Details or they will not be considered ToothParts in OD. I don't see any text in the schema that could go here.
2. Annotations. 
		Two examples are listed below.
			They can all have Category, which is a description.
			They can all have relationships, which are pairs of info.
			For example, Dentin 21%, Enamel 79%.
			Those are two relationships. Dentin and Enamel are prop_value. Percentages are metric_value.
			When we store them, each relationship is separated by CR so that all relationships end up as a single chunk of text.
			We will make this text show when hovering over a polygon or contour box.
		A. Caries
			Polygon
			In the same ImageDraw row, we store the relationships as described above.
			So as far as we are concerned, this is exactly the same as a toothPart, but with the addition of an annotation.
		B. Calculus
			Contour box (rectangle outline)
			Since we don't have unfilled polygons, this is stored as a line segment of 4 lines (5 points), ending at the starting point.
			In the same ImageDraw row, we store the relationships as described above.
3. Line with text
		line_segment
		annotation.text contains the text to show by the line.
		We store a line with text as two separate unconnected rows in ImageDraw.
		One ImageDraw row is a line 
		Second ImageDraw row is text. 
		Font is tricky since we get no hits at all about fontsize to use.
		We have added a ProgramProperty with default font height in pixels.
		Users can change this default at any time.
		For now, we will always draw to this default size, and we will not have any way to override per text item. This might change.
		Since we use a global font size, leave ImageDraw.FontSize as 0.
*/
		public Patient Patient_;
		///<summary>Contains the bitmap to be sent to Pearl.</summary>
		public Bitmap Bitmap_;
		///<summary>For a single document. In this case, the MountNum will be zero.</summary>
		public Document Document_;
		///<summary>Only used for a mount. We need this to determine the offset for each bitmap in the mount. Since we will get back drawings for an individual bitmap, we need to shift them all. We store drawings for a mount all relative to the mount origin, kind of like a big giant bitmap.</summary>
		public MountItem MountItem_;
		///<summary>PearlRequest used in polling.</summary>
		private PearlRequest _pearlRequest;
		///<summary>Used to trigger a UI refresh when the thread is complete.</summary>
		public static event EventHandler EventRefreshDisplay;

		//constants for ProgramProperties
		//Public so that ControlImages.cs and PearlApiClient.cs have access to these properties.
		//These should NOT be changed without a convert script update, because they are hardcoded in ConvertDatabases8.cs
		public const string PEARL_FONT_SIZE_PROPERTY="Text font size";
		public const string PEARL_CLIENT_ID_PROPERTY="Client ID";
		public const string PEARL_CLIENT_SECRET_PROPERTY="Client Secret";
		public const string PEARL_ORGANIZATION_ID_PROPERTY="Organization ID";
		public const string PEARL_OFFICE_ID_PROPERTY="Office ID";
		public const string PEARL_SHOW_ANNOTATIONS_BY_DEFAULT_PROPERTY="Show Pearl annotations by default";
		public const string PEARL_CATEGORIES_FOR_AUTO_UPLOAD="Image categories for automatic upload";

		//Used to set button labels and to differentiate between which button was clicked in ControlImages.ToolBarProgram_Click
		public const string PEARL_BUTTON_SEND_LABEL="Send to Pearl";
		public const string PEARL_BUTTON_LAYERS_LABEL="Show Layers";

		///<summary>Must be called before SendOnThread. Image can be given as a bitmap or a file path. Set mountItem if image is part of a mount. Returns null if filePath couldn't be converted to bitmap or image was already sent to Pearl.</summary>
		public static Pearl SetupPearlForSendingSingle(Patient patient,Document document,Bitmap bitmap=null,string filePath="",MountItem mountItem=null) {
			if(bitmap==null) {
				try {
					bitmap=new Bitmap(filePath);
				}
				catch {
					return null;//File couldn't be converted to bitmap
				}
			}
			Pearl pearl=new Pearl();
			pearl.Patient_=patient;
			if(mountItem!=null) {
				pearl.Bitmap_=ImageHelper.ApplyDocumentSettingsToImage(document,bitmap,ImageSettingFlags.ALL);//Creates copy of bitmap
			}
			else {
				pearl.Bitmap_=(Bitmap)bitmap.Clone();//Creates copy of bitmap
			}
			pearl.MountItem_=mountItem;
			pearl.Document_=document;
			PearlRequest pearlRequest=PearlRequests.GetOneByDocNum(pearl.Document_.DocNum);
			if(pearlRequest!=null && pearlRequest.RequestStatus!=EnumPearlStatus.TimedOut) {
				return null;
			}
			return pearl;
		}

		public void Dispose() {
			Bitmap_?.Dispose();
		}

		///<summary>Returns whether an image category should automatically upload an image to Pearl. Returns false if the Pearl program property containing the image category names is not found.</summary>
		public static bool DoAutoUploadForImageCategory(long defNumCategory) {
			if(!Programs.IsEnabled(ProgramName.Pearl)) {
				return false;
			}
			string categoriesStr=ProgramProperties.GetPropVal(ProgramName.Pearl,PEARL_CATEGORIES_FOR_AUTO_UPLOAD);
			List<string> listCategories=categoriesStr.Split(',').ToList();
			string categoryName=Defs.GetName(DefCat.ImageCats,defNumCategory);
			if(listCategories.Contains(categoryName)) {
				return true;
			}
			return false;
		}

		///<summary>SetupPearlForSending must be called first. Spawns a new ODThread that uploads images to Pearl and polls for their results.</summary>
		public void SendOnThread() {
			ODThread oDThreadPearl=new ODThread(this.SendOnThreadWorker);
			//Swallow all exceptions and allow thread to exit gracefully.
			oDThreadPearl.AddExceptionHandler(new ODThread.ExceptionDelegate((Exception ex) => { }));
			oDThreadPearl.AddExitHandler(new ODThread.WorkerDelegate((o) => {
				EventRefreshDisplay?.Invoke(this,new EventArgs());
				Dispose();
			}));
			oDThreadPearl.Start(isAutoCleanup:true);
		}

		///<summary>This is the worker thread.</summary>
		public void SendOnThreadWorker(ODThread oDThread){
			_pearlRequest=null;
			if(Document_ is null){
				throw new Exception("Document must be specified.");
			}
			//immediately send one image for a single document
			PearlRequest pearlRequest=null;
			try {
				if(Bitmap_ is null) {
					return;
				}
				//Look for existing request for this document and skip uploading the image if one is found.
				pearlRequest=PearlRequests.GetOneByDocNum(Document_.DocNum);
				if(pearlRequest!=null) {
					return;
				}
				//Send image to Pearl
				string requestId=null;
				if(ODBuild.IsDebug()&&!ODBuild.IsUnitTest) {
					oDThread.Wait(2000);//simulated wait for getting URL and uploading image
					requestId=SimulateSendOneImageToPearl(Document_.DocNum,Bitmap_);
				}
				else {
					//Get AWS presigned URL, upload image to it, then send to Pearl
					try {
						requestId=PearlApiClient.Inst.SendOneImageToPearl(Document_.DocNum,Bitmap_,Patient_);
					}
					catch(Exception ex) {
						ODEvent.Fire(ODEventType.ProgressBar,"There was an error uploading an image:\r\n"+ex.Message);
						return;
					}
				}
				if(string.IsNullOrWhiteSpace(requestId)) {
					ODEvent.Fire(ODEventType.ProgressBar,"An image failed to upload.");
					return;
				}
				pearlRequest=PearlRequests.GetOneByRequestId(requestId);
			}
			finally {
				_pearlRequest=pearlRequest;
			}
			if(_pearlRequest is null || PearlRequests.IsRequestHandled(_pearlRequest)) {
				return;//Don't poll if image was not uploaded or was already handled.
			}
			//Begin polling the API until all images have been processed, or this thread is canceled.
			DateTime dtStart=DateTime.Now;
			bool isComplete;
			List<PearlRequest> listPearlRequests=new List<PearlRequest> { _pearlRequest };
			PearlRequests.UpdateStatusForRequests(listPearlRequests,EnumPearlStatus.Polling);
			while(!oDThread.HasQuit) {
				isComplete=PollRequest();
				if(isComplete) {
					return;
				}
				int pollRateMs=2_000;
				if(DateTime.Now.Subtract(dtStart)>TimeSpan.FromMinutes(1)) {
					pollRateMs=30_000;//After a minute of polling every 2 seconds, start polling every 30 seconds for 10 min, then stop.
				}
				int pollTimeoutSecs=600;//10 minutes
				if((DateTime.Now-dtStart).TotalSeconds>=pollTimeoutSecs) {
					if(!PearlRequests.IsRequestHandled(_pearlRequest)) {
						PearlRequests.UpdateStatusForRequests(listPearlRequests,EnumPearlStatus.TimedOut);
					}
					return;
				}
				oDThread.Wait(pollRateMs);
			}
		}
		
		#region Simulate methods
		///<summary>Returns a fake Pearl requestId and inserts a PearlRequest into the DB.</summary>
		private string SimulateSendOneImageToPearl(long docNum,Bitmap bitmap) {
			// 1 - Get AWS presigned URL
			string requestId=Guid.NewGuid().ToString();
			// 2 - Upload image to AWS presigned URL
			// 3 - Create PearlRequest
			PearlRequest pearlRequest=new PearlRequest();
			pearlRequest.RequestId=requestId;
			pearlRequest.RequestStatus=EnumPearlStatus.Polling;
			pearlRequest.DocNum=docNum;
			pearlRequest.DateTSent=DateTime.Now;
			PearlRequests.Insert(pearlRequest);
			return pearlRequest.RequestId;
		}

		///<summary>Returns a fake Pearl result. Does not include every field from the API response, only those necessary for testing.</summary>
		private OpenDentBusiness.Pearl.Result SimulateGetResultsForOneImage(string requestId,Bitmap bitmap) {
			OpenDentBusiness.Pearl.Result result=new OpenDentBusiness.Pearl.Result();
			result.width=bitmap.Width;
			result.height=bitmap.Height;
			List<ToothPart> listToothParts=new List<ToothPart>();
			List<Annotation> listAnnotations=new List<Annotation>();
			//Make Annotation polygons for each EnumCategory type
			List<EnumCategory> listEnumCategories=((EnumCategory[])Enum.GetValues(typeof(EnumCategory))).ToList();
			listEnumCategories.Add(EnumCategory.Caries);//Add Caries a second time for Caries - Incipient
			for(int i=0;i<listEnumCategories.Count;i++) {
				if(listEnumCategories[i]==EnumCategory.None) {
					continue;
				}
				float xAnnotation=(float)i/(float)listEnumCategories.Count;//Equally spread out each annotation polygon (filled squares)
				float yAnnotation=0.2f;
				float sizeAnnotation=0.5f/(float)listEnumCategories.Count;
				Annotation annotation=new Annotation();
				annotation.category_id=listEnumCategories[i];
				annotation.polygon=new Polygon[] {
					new Polygon() { x=xAnnotation,								y=yAnnotation },
					new Polygon() { x=xAnnotation+sizeAnnotation,	y=yAnnotation },
					new Polygon() { x=xAnnotation+sizeAnnotation,	y=yAnnotation+sizeAnnotation },
					new Polygon() { x=xAnnotation,								y=yAnnotation+sizeAnnotation },
				};
				annotation.stroke_color="#54FFFF54";
				float dentinMetricVal=0.7875f;
				if(i==listEnumCategories.Count-1 && listEnumCategories[i]==EnumCategory.Caries) {
					dentinMetricVal=0.0f;//Make second Caries a CariesProgressed
				}
				annotation.relationships=new Relationship[] {
					new Relationship { metric_value=0.4562f,					prop_value="conditions.anatomy.enamel"},
					new Relationship { metric_value=dentinMetricVal,	prop_value="conditions.anatomy.dentin"},
					new Relationship { metric_value=0.0f,							prop_value="background"}
				};
				listAnnotations.Add(annotation);
			}
			//Make Toothpart polygons for each EnumCategory type
			List<EnumCondition> listEnumConditions=((EnumCondition[])Enum.GetValues(typeof(EnumCondition))).ToList();
			for(int i=0;i<listEnumConditions.Count;i++) {
				if(listEnumConditions[i]==EnumCondition.None) {
					continue;
				}
				float xToothPart=(float)i/(float)listEnumConditions.Count;//Equally spread out each tooth part polygon (filled squares)
				float yToothPart=0.8f;
				float sizeToothPart=0.5f/(float)listEnumCategories.Count;
				ToothPart toothPart=new ToothPart();
				toothPart.condition_id=listEnumConditions[i];
				toothPart.category_id=EnumCategory.ToothParts;
				toothPart.polygon=new Polygon[] {
					new Polygon() { x=xToothPart,								y=yToothPart },
					new Polygon() { x=xToothPart+sizeToothPart,	y=yToothPart },
					new Polygon() { x=xToothPart+sizeToothPart,	y=yToothPart+sizeToothPart },
					new Polygon() { x=xToothPart,								y=yToothPart+sizeToothPart },
				};
				toothPart.color=new OpenDentBusiness.Pearl.Color();
				toothPart.color.fill_color="#54FF5454";
				listToothParts.Add(toothPart);
			}
			//Make measurement line
			Annotation annotationLine=new Annotation();
			annotationLine.category_id=EnumCategory.Measurements;
			annotationLine.line_segment=new LineSegment();
			annotationLine.line_segment.x1=100;
			annotationLine.line_segment.y1=100;
			annotationLine.line_segment.x2=150;
			annotationLine.line_segment.y2=150;
			annotationLine.line_segment.size=0.1f;
			annotationLine.text=new Text();
			annotationLine.text.text="3.27mm";
			annotationLine.text.x=150;
			annotationLine.text.y=150;
			annotationLine.stroke_color="#FFFFFFFF";
			listAnnotations.Add(annotationLine);
			//Make contour box
			Annotation annotationContour=new Annotation();
			annotationContour.category_id=EnumCategory.PeriapicalRadiolucency;
			annotationContour.contour_box=new ContourBox { };
			annotationContour.contour_box.x=400;
			annotationContour.contour_box.y=100;
			annotationContour.contour_box.width=300;
			annotationContour.contour_box.height=150;
			annotationContour.stroke_color="#FF5454FF";
			listAnnotations.Add(annotationContour);
			result.toothParts=listToothParts.ToArray();
			result.annotations=listAnnotations.ToArray();
			result.is_completed=true;
			return result;
		}
		#endregion Simulate methods

		///<summary>Returns true when Pearl result is returned. Only called for a single image, not a full mount.</summary>
		private bool PollRequest() {
			OpenDentBusiness.Pearl.Result result=null;
			if(PearlRequests.IsRequestHandled(_pearlRequest)) {
				return true;
			}
			if(ODBuild.IsDebug()&&!ODBuild.IsUnitTest) {
				result=SimulateGetResultsForOneImage(_pearlRequest.RequestId,Bitmap_);
				Thread.Sleep(200);//simulated wait for getting API response
			}
			else {
				try {
					result=PearlApiClient.Inst.GetOneImageFromPearl(_pearlRequest.RequestId);
				}
				catch (Exception ex) {
					//API error, we didn't get a result so we will try again on next polling loop.
					ODEvent.Fire(ODEventType.ProgressBar,"There was an error polling an image:\r\n"+ex.Message);
				}
			}
			//Order matters for the checks below. We will weed out the negative results first and be left with complete (synonymous with success in Pearl's vocab).
			if(result==null) { 
				//No result (api exception) or result is not complete (still processing), try again.
				return false;
			}
			if(result.is_deleted || result.is_rejected) {
				//We got a result, and it is an error.
				_pearlRequest.RequestStatus=EnumPearlStatus.Error;
				PearlRequests.Update(_pearlRequest);
				//Todo: Pearl error. Should we show a message? Log the error? Resend the image?
				return true;
			}
			if(result.is_completed) {
				//We got a result, and it is complete/ok.
				_pearlRequest.RequestStatus=EnumPearlStatus.Received;
				PearlRequests.Update(_pearlRequest);
				ProcessResultsForOneImage(result,Document_.DocNum,MountItem_,Bitmap_);
				return true;
			}
			//May still be processing and we don't have a valid result yet. Try again.
			return false;
		}

		/// <summary>Either DocNum or MountNum must be specified.</summary>
		public void ProcessResultsForOneImage(OpenDentBusiness.Pearl.Result result,long docNum,MountItem mountItem,Bitmap bitmap){
			Point pointMountPos=new Point(0,0);
			long mountNum=0;
			int width=bitmap.Width;
			int height=bitmap.Height;
			float scale=1f;
			int paddingX=0;
			int paddingY=0;
			if(mountItem!=null) {
				//Scale points when bitmap aspect ratio doesn't match mount item's aspect ratio. 
				float ratioWtoHMountItem=(float)mountItem.Width/mountItem.Height;
				float ratioWtoHBitmap=(float)bitmap.Width/bitmap.Height;
				if(ratioWtoHBitmap > ratioWtoHMountItem) {
					//The bitmap is wider in shape than the mount item. Scale points based on the width, as it is the limiting dimension.
					scale=(float)mountItem.Width/bitmap.Width;
					paddingY=(int)(mountItem.Height-(bitmap.Height * scale)) / 2;
				}
				else {
					//The bitmap is taller in shape than the mount item. Scale points based on the height, as it is the limiting dimension.
					scale=(float)mountItem.Height/bitmap.Height;
					paddingX=(int)(mountItem.Width-(bitmap.Width*scale))/2;
				}
				//Add padding to line annotations up with centered image.
				pointMountPos.X=mountItem.Xpos+paddingX;
				pointMountPos.Y=mountItem.Ypos+paddingY;
				width=(int)(width*scale);
				height=(int)(height*scale);
				mountNum=mountItem.MountNum;
			}
			if(result.annotations.IsNullOrEmpty() && result.toothParts.IsNullOrEmpty()) {
				return;
			}
			List<Annotation> listAnnotations=result.annotations.ToList();
			List<ToothPart> listToothParts=result.toothParts.ToList();
			//Loop through annotations, create ImageDraw for each.
			for(int i=0;i<listAnnotations.Count;i++) {
				ImageDraw imageDraw=new ImageDraw();
				imageDraw.DocNum=docNum;
				imageDraw.MountNum=mountNum;
				imageDraw.ColorDraw=PearlColorToSystemColor(listAnnotations[i]?.stroke_color);
				imageDraw.ImageAnnotVendor=EnumImageAnnotVendor.Pearl;
				imageDraw.PearlLayer=CategoryToODCategory(listAnnotations[i].category_id);
				if(imageDraw.PearlLayer==EnumCategoryOD.CariesProgressed 
					&& listAnnotations[i].relationships.Any(x => x.prop_value.Contains("dentin") && x.metric_value>0.0f))
				{
					imageDraw.PearlLayer=EnumCategoryOD.CariesIncipient;
				}
				List<PointF> listPointFsAnnotation=new List<PointF>();
				//Use polygon if provided, otherwise use line segment and text if provided, otherwise use contour box.
				if(!listAnnotations[i].polygon.IsNullOrEmpty()) {
					//Process polygon
					for(int j=0;j<listAnnotations[i].polygon.Count();j++) {
						//Polygon points are a relative coordinate system (0-1)
						PointF pointF=PearlCoordsToImageCoords(listAnnotations[i].polygon[j],pointMountPos,width,height);
						listPointFsAnnotation.Add(pointF);
					}
					imageDraw.DrawType=ImageDrawType.Polygon;
					imageDraw.Details=RelationshipsToReadableString(listAnnotations[i].relationships.ToList(),imageDraw.PearlLayer);
				}
				else if(listAnnotations[i].line_segment!=null && listAnnotations[i].text!=null) {
					//Process line segment
					PointF pointFLine1=new PointF();
					pointFLine1.X=(float)listAnnotations[i].line_segment.x1;//Line segment points are given in pixels.
					pointFLine1.Y=(float)listAnnotations[i].line_segment.y1;
					listPointFsAnnotation.Add(pointFLine1);
					PointF pointFLine2=new PointF();
					pointFLine2.X=(float)listAnnotations[i].line_segment.x2;
					pointFLine2.Y=(float)listAnnotations[i].line_segment.y2;
					listPointFsAnnotation.Add(pointFLine2);
					listPointFsAnnotation=ScalePointsToMountItem(listPointFsAnnotation,scale);
					listPointFsAnnotation=TranslatePointsToMountItem(listPointFsAnnotation,pointMountPos);
					imageDraw.DrawType=ImageDrawType.Line;
					//Process text
					ImageDraw imageDrawText=imageDraw.Copy();
					imageDrawText.DrawType=ImageDrawType.Text;
					PointF pointFText=new PointF();
					pointFText.X=(int)listAnnotations[i].text.x;//Text point is given in pixels.
					pointFText.Y=(int)listAnnotations[i].text.y;
					List<PointF> listPointFsText=new List<PointF>() { pointFText };
					listPointFsText=ScalePointsToMountItem(listPointFsText,scale);
					listPointFsText=TranslatePointsToMountItem(listPointFsText,pointMountPos);
					Point pointText=Point.Round(listPointFsText[0]);
					imageDrawText.SetLocAndText(pointText,listAnnotations[i].text.text);
					ImageDraws.Insert(imageDrawText);
				}
				else {
					if(imageDraw.PearlLayer==EnumCategoryOD.Measurements) {
						continue;//Skip measurement contour boxes. They only outline each tooth.
					}
					//Process contour box 
					int contourX=listAnnotations[i].contour_box.x;//Contour box point is given in pixels
					int contourY=listAnnotations[i].contour_box.y;
					int contourWidth=listAnnotations[i].contour_box.width;
					int contourHeight=listAnnotations[i].contour_box.height;
					//Show contour box as four connecting lines. (5 points, first and last are equal)
					List<PointF> listPointFs=new List<PointF>();
					listPointFs.Add(new PointF() { X=contourX,							Y=contourY });
					listPointFs.Add(new PointF() { X=contourX+contourWidth,	Y=contourY });
					listPointFs.Add(new PointF() { X=contourX+contourWidth,	Y=contourY+contourHeight });
					listPointFs.Add(new PointF() { X=contourX,							Y=contourY+contourHeight });
					listPointFs.Add(new PointF() { X=contourX,							Y=contourY });
					listPointFsAnnotation.AddRange(listPointFs);
					listPointFsAnnotation=ScalePointsToMountItem(listPointFsAnnotation,scale);
					listPointFsAnnotation=TranslatePointsToMountItem(listPointFsAnnotation,pointMountPos);
					imageDraw.DrawType=ImageDrawType.Line;
					imageDraw.Details=imageDraw.PearlLayer.ToString();
				}
				imageDraw.SetDrawingSegment(listPointFsAnnotation);
				ImageDraws.Insert(imageDraw);
			}
			//Loop through tooth parts, create ImageDraw for each
			for(int i=0;i<listToothParts.Count;i++) {
				List<PointF> listPointFsToothPart=new List<PointF>();
				for(int j=0;j<listToothParts[i].polygon.Count();j++) {
					PointF pointF=PearlCoordsToImageCoords(listToothParts[i].polygon[j],pointMountPos,width,height);
					listPointFsToothPart.Add(pointF);
				}
				//Create ImageDraw
				ImageDraw imageDraw=new ImageDraw();
				imageDraw.DocNum=docNum;
				imageDraw.MountNum=mountNum;
				imageDraw.ColorDraw=PearlColorToSystemColor(listToothParts[i]?.color?.fill_color);//toothpart.color can be null
				imageDraw.DrawType=ImageDrawType.Polygon;
				imageDraw.SetDrawingSegment(listPointFsToothPart); 
				imageDraw.ImageAnnotVendor=EnumImageAnnotVendor.Pearl;
				imageDraw.PearlLayer=ConditionToODCategory(listToothParts[i].condition_id);
				ImageDraws.Insert(imageDraw);
			}
		}

		#region ImageDraw helpers
		///<summary>Converts from Pearl's relative coordinate system to pixels. Translates point to account for mount position.</summary>
		public static PointF PearlCoordsToImageCoords(Polygon polygonPoint,Point pointMountPos,int width,int height) {
			PointF pointF=new PointF();
			float pearlX=(float)polygonPoint.x;
			float pearlY=(float)polygonPoint.y;
			//Convert from relative coordinates to pixels
			pointF.X=(int)(pearlX*(float)width);
			pointF.Y=(int)(pearlY*(float)height);
			//Translate to mount item's position on mount
			pointF.X+=pointMountPos.X;
			pointF.Y+=pointMountPos.Y;
			return pointF;
		}

		///<summary>For text + line and contour boxes, Pearl uses pixels as coordinates relative to the original image. If the image was in a mount with different pixel size, we need to scale the pixels accordingly.</summary>
		private static List<PointF> ScalePointsToMountItem(List<PointF> listPointFs,float scale) {
			List<PointF> listPointFsScaled=new List<PointF>();
			for(int i=0;i<listPointFs.Count;i++) {
				PointF pointFScaled=new PointF();
				pointFScaled.X=listPointFs[i].X * scale;
				pointFScaled.Y=listPointFs[i].Y * scale;
				listPointFsScaled.Add(pointFScaled);
			}
			return listPointFsScaled;
		}

		///<summary>Translates a list of points to a position. Used to place points over the correct location in a mount.</summary>
		private static List<PointF> TranslatePointsToMountItem(List<PointF> listPointFs,PointF pointFMountItem) {
			List<PointF> listPointFsTranslated=new List<PointF>();
			for(int i=0;i<listPointFs.Count;i++) {
				PointF pointF=new PointF();
				pointF.X=listPointFs[i].X+pointFMountItem.X;
				pointF.Y=listPointFs[i].Y+pointFMountItem.Y;
				listPointFsTranslated.Add(pointF);
			}
			return listPointFsTranslated;
		}

		///<summary>Converts a hex string with the format #RRGGBBAA or #RRGGBB into a System.Drawing.Color. Returns Color.Empty if colorStr can't be converted to a Color.</summary>
		public static System.Drawing.Color PearlColorToSystemColor(string colorStr) {
			if(string.IsNullOrWhiteSpace(colorStr) || colorStr.Length>9) {
				return System.Drawing.Color.Empty;
			}
			//Pearl's API returns color in RGBA or RGB format. All built-in string to Color conversion methods expect ARGB format, so we have to break the 
			//string apart and pass in each part individually.
			System.Drawing.Color color;
			try {
				int r=Convert.ToInt32("0x"+colorStr.Substring(1,2),16);
				int g=Convert.ToInt32("0x"+colorStr.Substring(3,2),16);
				int b=Convert.ToInt32("0x"+colorStr.Substring(5,2),16);
				int a=255;
				if(colorStr.Length>7) {
					a=Convert.ToInt32("0x"+colorStr.Substring(7,2),16);
				}
				color=System.Drawing.Color.FromArgb(a,r,g,b);
			}
			catch {
				color=System.Drawing.Color.Empty;
			}
			return color;
		}

		///<summary>Converts a Relationship object into a readable string. The metric_values of all relationships in an annotation may exceed 1. 
		///Pearl's API documentation recommends totaling all metric_values and dividing each metric_value by the total to find true percentage.
		///</summary>
		public static string RelationshipsToReadableString(List<Relationship> listRelationships,EnumCategoryOD enumCategoryOD) {
			string retVal=enumCategoryOD.ToString();
			float totalMetricValue=0f;
			//Calculate total metric value
			for(int i=0;i<listRelationships.Count;i++) {
				totalMetricValue+=(float)listRelationships[i].metric_value;
			}
			//Create string for each relationship and add to retVal
			for(int i=0;i<listRelationships.Count;i++) {
				if(listRelationships[i].metric_value==0f) {
					continue;//Don't add relationships with 0%
				}
				string relationshipArea="";
				switch(listRelationships[i].prop_value) {
					case "conditions.anatomy.bone":
						relationshipArea="Bone";
						break;
					case "conditions.anatomy.cementum":
						relationshipArea="Cementum";
						break;
					case "conditions.anatomy.dentin":
						relationshipArea="Dentin";
						break;
					case "conditions.anatomy.enamel":
						relationshipArea="Enamel";
						break;
					case "conditions.anatomy.pulp":
						relationshipArea="Pulp";
						break;
					case "conditions.anatomy.restoration_e":
						relationshipArea="Restoration";
						break;
					case "conditions.anatomy.occlusal":
						relationshipArea="Occlusal";
						break;
					case "background":
					default:
						relationshipArea="Background";
						break;
				}
				float percentage=((float)listRelationships[i].metric_value / totalMetricValue) * 100f;
				retVal+="\n"+relationshipArea+": "+percentage.ToString("F0")+"%";//Ex: "Enamel: 45%"
			}
			return retVal;
		}

		///<summary>Converts Pearl Category enum into EnumCategoryOD.</summary>
		public static EnumCategoryOD CategoryToODCategory(EnumCategory enumCategory) {
			switch(enumCategory) {
				case EnumCategory.Crown:                  return EnumCategoryOD.Crown;
				case EnumCategory.Filling:                return EnumCategoryOD.Filling;
				case EnumCategory.Implant:                return EnumCategoryOD.Implant;
				case EnumCategory.RootCanal:              return EnumCategoryOD.RootCanal;
				case EnumCategory.Bridge:                 return EnumCategoryOD.Bridge;
				case EnumCategory.Caries:                 return EnumCategoryOD.CariesProgressed;
				case EnumCategory.Calculus:               return EnumCategoryOD.Calculus;
				case EnumCategory.PeriapicalRadiolucency: return EnumCategoryOD.PeriapicalRadiolucency;
				case EnumCategory.NotableMargin:          return EnumCategoryOD.NotableMargin;
				case EnumCategory.Measurements:           return EnumCategoryOD.Measurements;
				case EnumCategory.ToothParts:             return EnumCategoryOD.Anatomy;
				default: 
					return EnumCategoryOD.None;
			}
		}

		///<summary>Converts Pearl Condition enum into EnumCategoryOD.</summary>
		public static EnumCategoryOD ConditionToODCategory(EnumCondition enumCondition) {
			switch(enumCondition) {
				case EnumCondition.Bone:                  return EnumCategoryOD.Bone;
				case EnumCondition.Enamel:                return EnumCategoryOD.Enamel;
				case EnumCondition.Dentin:                return EnumCategoryOD.Dentin;
				case EnumCondition.Pulp:                  return EnumCategoryOD.Pulp;
				case EnumCondition.Cementum:              return EnumCategoryOD.Cementum;
				case EnumCondition.Restoration:           return EnumCategoryOD.Restoration;
				case EnumCondition.InferiorAlveolarNerve: return EnumCategoryOD.InferiorAlveolarNerve;
				case EnumCondition.Sinus:                 return EnumCategoryOD.Sinus;
				case EnumCondition.NasalCavity:           return EnumCategoryOD.NasalCavity;
				case EnumCondition.Bo:                    return EnumCategoryOD.Bone;
				default: 
					return EnumCategoryOD.None;
			}
		}

		///<summary>Returns list of EnumCategoryODs considered to be "tooth parts".</summary>
		public static List<EnumCategoryOD> GetToothPartsCategoryODs() {
			List<EnumCategoryOD> listEnumCategoryODs=new List<EnumCategoryOD>();
			listEnumCategoryODs.Add(EnumCategoryOD.Bone);
			listEnumCategoryODs.Add(EnumCategoryOD.Cementum);
			listEnumCategoryODs.Add(EnumCategoryOD.Dentin);
			listEnumCategoryODs.Add(EnumCategoryOD.Enamel);
			listEnumCategoryODs.Add(EnumCategoryOD.Pulp);
			listEnumCategoryODs.Add(EnumCategoryOD.Restoration);
			listEnumCategoryODs.Add(EnumCategoryOD.InferiorAlveolarNerve);
			listEnumCategoryODs.Add(EnumCategoryOD.Sinus);
			listEnumCategoryODs.Add(EnumCategoryOD.NasalCavity);
			listEnumCategoryODs.Add(EnumCategoryOD.Anatomy);
			return listEnumCategoryODs;
		}

		///<summary>Returns list of all EnumCategoryODs not considered to be "tooth parts".</summary>
		public static List<EnumCategoryOD> GetAllCategoryODsExceptToothParts() {
			List<EnumCategoryOD> listEnumCategoryODsToothparts=GetToothPartsCategoryODs();
			List<EnumCategoryOD> listEnumCategoryODsRet=Enum.GetValues(typeof(EnumCategoryOD))
				.Cast<EnumCategoryOD>().Except(listEnumCategoryODsToothparts).ToList();
			return listEnumCategoryODsRet;
		}
		#endregion

	}
}

