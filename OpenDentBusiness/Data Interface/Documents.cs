using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CodeBase;
using DataConnectionBase;
using Microsoft.Win32;
using OpenDental.Thinfinity;
using PdfSharp.Pdf;

namespace OpenDentBusiness {
	///<summary>Handles documents and images for the Images module</summary>
	public class Documents {
		#region Insert

		///<summary>Inserts the Document and retrieves it immediately from the database.</summary>
		public static Document InsertAndGet(Document document,Patient patient) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Document>(MethodBase.GetCurrentMethod(),document,patient);
			}
			Insert(document,patient);
			return GetByNum(document.DocNum);
		}

		#endregion

		///<summary></summary>
		public static Document[] GetAllWithPat(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Document[]>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM document WHERE PatNum="+POut.Long(patNum)+" ORDER BY DateCreated";
			DataTable table=Db.GetTable(command);
			return Crud.DocumentCrud.TableToList(table).ToArray();
		}

		///<summary></summary>
		public static List<Document> GetPatientData(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Document>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM document WHERE PatNum="+POut.Long(patNum)+" ORDER BY DateCreated";
			DataTable table=Db.GetTable(command);
			return Crud.DocumentCrud.TableToList(table);
		}

		///<summary> Returns all Documents with an image capture type that is not Miscellaneous, in descending order by dateCreated. </summary>
		public static List<Document> GetOcrDocumentsForPat(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Document>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM document WHERE PatNum="+POut.Long(patNum)+" AND ImageCaptureType > 0 ORDER BY DateCreated DESC";
			return Crud.DocumentCrud.SelectMany(command);
		}

		///<summary>Gets all documents for a patient, returning a list of documents with the server's current DateTime. </summary>
		public static List<DocumentForApi> GetAllWithPatForApi(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<DocumentForApi>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM document WHERE PatNum="+POut.Long(patNum)+" ORDER BY DateCreated";
			string commandDateTime="SELECT "+DbHelper.Now();
			DateTime dateTimeServer=PIn.DateT(OpenDentBusiness.Db.GetScalar(commandDateTime));//run before appts for rigorous inclusion of appts
			List<Document> listDocuments=Crud.DocumentCrud.TableToList(Db.GetTable(command));
			List<DocumentForApi> listDocumentForApis=new List<DocumentForApi>();
			for(int i=0;i<listDocuments.Count;i++) {
				DocumentForApi documentForApi=new DocumentForApi();
				documentForApi.DocumentCur=listDocuments[i];
				documentForApi.DateTimeServer=dateTimeServer;
				listDocumentForApis.Add(documentForApi);
			}
			return listDocumentForApis;
		}

		///<summary>Gets the document with the specified document number.</summary>
		///<param name="doReturnNullIfNotFound">If false and there is no document with that docNum, will return a new Document.</param>
		public static Document GetByNum(long docNum,bool doReturnNullIfNotFound=false) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Document>(MethodBase.GetCurrentMethod(),docNum,doReturnNullIfNotFound);
			}
			Document document=Crud.DocumentCrud.SelectOne(docNum);
			if(document==null && !doReturnNullIfNotFound) {
				return new Document();
			}
			return document;
		}

		public static List<Document> GetByNums(List<long> listDocNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Document>>(MethodBase.GetCurrentMethod(),listDocNums);
			}
			if(listDocNums.Count<1) {
				return new List<Document>();
			}
			string command="SELECT * FROM document WHERE DocNum IN("+string.Join(",",listDocNums)+")";
			return Crud.DocumentCrud.SelectMany(command);
		}

		public static Document[] Fill(DataTable table){
			Meth.NoCheckMiddleTierRole();
			if(table==null){
				return new Document[0];
			}
			List<Document> listDocuments=Crud.DocumentCrud.TableToList(table);
			return listDocuments.ToArray();
		}

		/*
		private static Document[] RefreshAndFill(string command) {
			Meth.NoCheckMiddleTierRole();
			return Fill(Db.GetTable(command));
		}*/

		///<summary>Returns a unique filename for a previously inserted doc based on the pat's first and last name and docNum with the given extension.</summary>
		public static string GetUniqueFileNameForPatient(Patient patient,long docNum,string fileExtension) {
			Meth.NoCheckMiddleTierRole();
			string fileName=new string((patient.LName+patient.FName).Where(x => Char.IsLetter(x)).ToArray())+docNum.ToString()+fileExtension;//ensures unique name
			//there is still a slight chance that someone manually added a file with this name, so quick fix:
			List<string> listUsedNames=GetAllWithPat(patient.PatNum).Select(x => x.FileName).ToList();
			while(listUsedNames.Contains(fileName)) {
				fileName="x"+fileName;
			}
			return fileName;
		}

		///<summary>Usually, set just the extension before passing in the doc.  Inserts a new document into db, creates a filename based on Cur.DocNum, and then updates the db with this filename.  Should always refresh the document after calling this method in order to get the correct filename for RemotingRole.ClientWeb.</summary>
		public static long Insert(Document document,Patient patient) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				document.DocNum=Meth.GetLong(MethodBase.GetCurrentMethod(),document,patient);
				return document.DocNum;
			}
			document.DocNum=Crud.DocumentCrud.Insert(document);
			if(document.FileName!=Path.GetExtension(document.FileName)) {
				return document.DocNum;
			}
			//If the current filename is just an extension, then assign it a unique name.
			document.FileName=GenerateUniqueFileName(document.FileName,patient,document.DocNum.ToString());
			//there is still a slight chance that someone manually added a file with this name, so quick fix:
			string command="SELECT FileName FROM document WHERE PatNum="+POut.Long(document.PatNum);
			DataTable table=Db.GetTable(command);
			List<string> listUsedNames=new List<string>();
			for(int i=0;i<table.Rows.Count;i++) {
				listUsedNames.Add(PIn.String(table.Rows[i][0].ToString()));
			}
			while(true){
				bool hasMatch=false;
				for(int i=0;i<listUsedNames.Count;i++){
					if(listUsedNames[i]==document.FileName){
						hasMatch=true;
						break;
					}
				}
				if(!hasMatch){
					break;
				}
				document.FileName="x"+document.FileName;
			}
			/*Document[] documentArray=GetAllWithPat(document.PatNum);
			while(IsFileNameInList(document.FileName,documentArray)) {
				document.FileName="x"+document.FileName;
			}*/
			Update(document);
			return document.DocNum;
		}

		//Returns a unique file name with the given extension for the specified patient. If uniqueNum is not set, will generate a random number to ensure the filename is unique.
		public static string GenerateUniqueFileName(string extension, Patient patient, string uniqueIdentifier=null) {
			Meth.NoCheckMiddleTierRole();
			if(string.IsNullOrWhiteSpace(extension) || patient==null) {
				return "";
			}
			if(uniqueIdentifier.IsNullOrEmpty()) {//The current date/time stamp, to the 100,000th of a second
				uniqueIdentifier=DateTime.Now.ToString("yyMMddhhmmssfffff");
			}
			string patientNameLastFirst=patient.LName+patient.FName;
			string fileName=new string(patientNameLastFirst.Where(x => Char.IsLetter(x)).Select(x => x).ToArray());
			fileName+=uniqueIdentifier+extension;
			return fileName;
		}

		///<summary>This is a generic insert statement used to insert documents with custom file names.</summary>
		public static long Insert(Document document) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				document.DocNum=Meth.GetLong(MethodBase.GetCurrentMethod(),document);
				return document.DocNum;
			}
			return Crud.DocumentCrud.Insert(document);
		}

		///<summary></summary>
		public static void Update(Document document){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),document);
				return;
			}
			Crud.DocumentCrud.Update(document);
		}

		///<summary></summary>
		public static bool Update(Document document,Document documentOld){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),document,documentOld);
			}
			return Crud.DocumentCrud.Update(document,documentOld);
		}

		///<summary>Updates all of the mount's Document.DocCategory information when moving a mount.</summary>
		public static void UpdateDocCategoryForMountItems(long mountNum,long docCategory) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mountNum,docCategory);
				return;
			}
			List<MountItem> listMountItems=MountItems.GetItemsForMount(mountNum);
			List<Document> listDocumentsInMountDb=GetDocumentsForMountItems(listMountItems).Where(x => x!=null).ToList();
			if(listDocumentsInMountDb.Count<=0) {
				return;
			}
			for(int i=0;i<listDocumentsInMountDb.Count;i++) {
				Document documentOld=listDocumentsInMountDb[i].Copy();
				listDocumentsInMountDb[i].DocCategory=docCategory;
				Update(listDocumentsInMountDb[i],documentOld);
			}
		}

		///<summary></summary>
		public static void Delete(Document document){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),document);
				return;
			}
			Crud.DocumentCrud.Delete(document.DocNum);
		}

		///<summary>This is used by FormImageViewer to get a list of paths based on supplied list of DocNums. The reason is that later we will allow sharing of documents, so the paths may not be in the current patient folder.</summary>
		public static List<string> GetPaths(List<long> listDocNums,string atoZPath) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),listDocNums,atoZPath);
			}
			if(listDocNums.Count==0) {
				return new List<string>();
			}
			string command="SELECT document.DocNum,document.FileName,patient.ImageFolder "
				+"FROM document "
				+"LEFT JOIN patient ON patient.PatNum=document.PatNum "
				+"WHERE document.DocNum = '"+listDocNums[0].ToString()+"'";
			for(int i=1;i<listDocNums.Count;i++) {
				command+=" OR document.DocNum = '"+listDocNums[i].ToString()+"'";
			}
			//remember, they will not be in the correct order.
			DataTable table=Db.GetTable(command);
			Hashtable hashtable=new Hashtable();//key=docNum, value=path
			//one row for each document, but in the wrong order
			for(int i=0;i<table.Rows.Count;i++) {
				//We do not need to check if A to Z folders are being used here, because
				//thumbnails are not visible from the chart module when A to Z are disabled,
				//making it impossible to launch the form image viewer (the only place this
				//function is called from).
				hashtable.Add(PIn.Long(table.Rows[i][0].ToString()),
					ODFileUtils.CombinePaths(new string[] {	atoZPath,
						PIn.String(table.Rows[i][2].ToString()).Substring(0,1).ToUpper(),
						PIn.String(table.Rows[i][2].ToString()),
						PIn.String(table.Rows[i][1].ToString()),}));
			}
			List<string> listStrings=new List<string>();
			for(int i=0;i<listDocNums.Count;i++) {
				listStrings.Add((string)hashtable[listDocNums[i]]);
			}
			return listStrings;
		}

		///<summary>Will return null if no picture for this patient.</summary>
		public static Document GetPatPictFromDb(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Document>(MethodBase.GetCurrentMethod(),patNum);
			} 
			//first establish which category pat pics are in
			long defNumPicts=0;
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			for(int i=0;i<listDefs.Count;i++){
				if(Regex.IsMatch(listDefs[i].ItemValue,@"P")){
					defNumPicts=listDefs[i].DefNum;
					break;
				}
			}
			if(defNumPicts==0){//no category set for picts
				return null;
			}
			//then find, limit 1 to get the most recent
			string command="SELECT * FROM document "
				+"WHERE document.PatNum="+POut.Long(patNum)
				+" AND document.DocCategory="+POut.Long(defNumPicts)
				+" ORDER BY DateCreated DESC";
			command=DbHelper.LimitOrderBy(command,1);
			DataTable table=Db.GetTable(command);
			List<Document> listDocuments=Fill(table).ToList();
			if(listDocuments==null || listDocuments.Count<1){//no pictures
				return null;
			}
			return listDocuments[0];
		}

		///<summary>Gets all documents within the doc category designated as the "S"tatements directory via definitions.
		///Also gets dependents' statements if this patient is a guarantor (needed by the patient portal).</summary>
		public static List<Document> GetStatementsForPat(long patNum,List<long> listPatNumsDependents) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Document>>(MethodBase.GetCurrentMethod(),patNum,listPatNumsDependents);
			}
			string command="SELECT def.DefNum FROM  definition def "
				+"WHERE def.Category="+POut.Int((int)DefCat.ImageCats)+" "
				+"AND def.IsHidden=0  "
				+"AND def.ItemValue LIKE '%S%' ";//Statements category indicator
			List<long> listDefNums=Db.GetListLong(command);
			if(listDefNums.Count==0) {//There are no Statement image categories
				return new List<Document>();
			}
			if(!listPatNumsDependents.Contains(patNum)) {
				listPatNumsDependents.Add(patNum);
			}
			command="SELECT * FROM document d "
				+"WHERE d.PatNum IN ("+string.Join(",",listPatNumsDependents)+") "
				+"AND d.DocCategory IN ("+string.Join(",",listDefNums)+") "
				+"ORDER BY d.DateCreated DESC";
			return Crud.DocumentCrud.SelectMany(command);
		}

		///<summary>Get document info for all images linked to this patient.
		///Also gets dependents' images if this patient is a guarantor (needed by the patient portal).</summary>
		public static List<Document> GetPatientPortalDocsForPat(long patNum,List<long> listPatNumsDependents) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Document>>(MethodBase.GetCurrentMethod(),patNum,listPatNumsDependents);
			}
			string command="SELECT def.DefNum FROM  definition def "
				+"WHERE def.Category="+POut.Int((int)DefCat.ImageCats)+" "
				+"AND def.IsHidden=0  "
				+"AND def.ItemValue LIKE '%L%' ";//Patient Portal category indicator
			List<long> listDefNums=Db.GetListLong(command);
			if(listDefNums.Count==0) {//There are no Patient Portal image categories
				return new List<Document>();
			}
			if(!listPatNumsDependents.Contains(patNum)) {
				listPatNumsDependents.Add(patNum);
			}
			command="SELECT * FROM document d "
				+"WHERE d.PatNum IN ("+string.Join(",",listPatNumsDependents)+") "
				+"AND d.DocCategory IN ("+string.Join(",",listDefNums)+") "
				+"ORDER BY d.DateCreated DESC";
			return Crud.DocumentCrud.SelectMany(command);
		}

		/// <summary>Makes one call to the database to retrieve the document of the patient for the given patNum, then uses that document and the patFolder to load and process the patient picture so it appears the same way it did in the image module.  It first creates a 100x100 thumbnail if needed, then it uses the thumbnail. Can return null. Assumes WithPat will always be same as patnum.</summary>
		public static Bitmap GetPatPict(long patNum,string patFolder) {
			Meth.NoCheckMiddleTierRole();
			Document document=GetPatPictFromDb(patNum);
			Bitmap bitmap=GetPatPict(patNum,patFolder,document);
			return bitmap;
		}

		/// <summary>Uses the passed-in document and the patFolder to load and process the patient picture so it appears the same way it did in the image module.  It first creates a 100x100 thumbnail if needed, then it uses the thumbnail. Can return null. Assumes WithPat will always be same as patnum.</summary>
		public static Bitmap GetPatPict(long patNum,string patFolder,Document document) {
			Meth.NoCheckMiddleTierRole();
			if(document==null) {
				return null;
			}
			Bitmap bitmap=GetThumbnail(document,patFolder);
			return bitmap;
		}

		///<summary>Gets the thumbnail image for the given document. The thumbnail for every document is in a subfolder named 'Thumbnails' within each patient's images folder.  Always 100x100.</summary>
		public static Bitmap GetThumbnail(Document document,string patFolder){
			Meth.NoCheckMiddleTierRole();
			string fileName=document.FileName;
			//If no file name associated with the document, then there cannot be a thumbnail,
			//because thumbnails have the same name as the original image document.
			if(fileName.Length<1) {
				return NoAvailablePhoto();
			}
			string fullName=ODFileUtils.CombinePaths(patFolder,fileName);
			//If the document no longer exists, then there is no corresponding thumbnail image.
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && !File.Exists(fullName)) {
				return NoAvailablePhoto();
			}
			//If the specified document is not an image return 'not available'.
			if(!ImageHelper.HasImageExtension(fullName)) {
				return NoAvailablePhoto();
			}
			//Create Thumbnails folder if it does not already exist for this patient folder.
			string thumbPath=ODFileUtils.CombinePaths(patFolder,"Thumbnails");
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && !Directory.Exists(thumbPath)) {
				try {
					Directory.CreateDirectory(thumbPath);
				} 
				catch(Exception ex) {
					ex.DoNothing();
					return NoAvailablePhoto();
				}
			}
			string fileNameThumb=ODFileUtils.CombinePaths(patFolder,"Thumbnails",fileName);
			//Use the existing thumbnail if it already exists and it was created after the last document modification.
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && File.Exists(fileNameThumb)) {
				try {
					DateTime dateTimeThumbMod=File.GetLastWriteTime(fileNameThumb);
					if(dateTimeThumbMod>document.DateTStamp //thumbnail file is old
						&& !document.IsCropOld) //Prevents using the existing thumbnail file if the crop data has not yet been converted to the new style. New thumbnail will be created below.
					{	
						Bitmap bitmap = (Bitmap)Bitmap.FromFile(fileNameThumb);
						Bitmap bitmap2 = new Bitmap(bitmap);
						bitmap?.Dispose();//releases the file lock
						return bitmap2;
					}
				}
				catch{
					try {
						File.Delete(fileNameThumb); //File may be invalid, corrupted, or unavailable. This was a bug in previous versions.
					} 
					catch {
						//we tried our best, and it just wasn't good enough
					}
				}
			}
			#region Create New Thumbnail
			//Get the cropped/flipped/rotated image with any color filtering applied.
			Bitmap bitmapFullSize;
			try {
				bitmapFullSize=ImageHelper.GetImageCropped(document,patFolder);
			}
			catch(Exception ex) {
				ex.DoNothing();
				return NoAvailablePhoto();
			}
			if(bitmapFullSize is null){
				return NoAvailablePhoto();
			}
			Bitmap bitmapThumb=ImageHelper.GetBitmapSquare(bitmapFullSize,100);//Thumbnails saved in the thumbnails folder are always 100x100
			bitmapFullSize?.Dispose();
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {//Only save thumbnail to local directory if using local AtoZ
				try {
					bitmapThumb.Save(fileNameThumb);
				}
				catch(Exception ex) {
					ex.DoNothing();
					//Oh well, we can regenerate it next time if we have to!
				}
			}
			return bitmapThumb;
			#endregion Create New Thumbnail
		}

		public static Bitmap CreateNewThumbnail(Document document,Bitmap bitmap){
			Meth.NoCheckMiddleTierRole();
			//The bitmap passed in already has brightness/contrast applied
			Bitmap bitmapFullSize=ImageHelper.CopyWithCropRotate(document,bitmap);
			Bitmap bitmapThumb=ImageHelper.GetBitmapSquare(bitmapFullSize,100);//Thumbnails saved in the thumbnails folder are always 100x100
			bitmapFullSize?.Dispose();
			/*
			//If we also want to save it:
			string fileNameThumb=ODFileUtils.CombinePaths(patFolder,"Thumbnails",document.FileName);
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {//Only save thumbnail to local directory if using local AtoZ
				try {
					bitmapThumb.Save(fileNameThumb);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
			}*/
			return bitmapThumb;
		}

		public static Bitmap NoAvailablePhoto(){
			Meth.NoCheckMiddleTierRole();
			Bitmap bitmap=new Bitmap(100,100);
			using Graphics g=Graphics.FromImage(bitmap);
			g.InterpolationMode=InterpolationMode.High;
			g.TextRenderingHint=System.Drawing.Text.TextRenderingHint.AntiAlias;
			g.FillRectangle(Brushes.WhiteSmoke,0,0,bitmap.Width,bitmap.Height);
			using StringFormat notAvailFormat=new StringFormat();
			notAvailFormat.Alignment=StringAlignment.Center;
			notAvailFormat.LineAlignment=StringAlignment.Center;
			using Font font=new Font(FontFamily.GenericSansSerif,8f);
			g.DrawString("Thumbnail not available",font,Brushes.Black,new RectangleF(0,0,100,100),notAvailFormat);
			return bitmap;
		}

		///<summary>Returns the documents which correspond to the given mountitems. They should already be ordered by ItemOrder.</summary>
		public static Document[] GetDocumentsForMountItems(List<MountItem> listMountItems) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Document[]>(MethodBase.GetCurrentMethod(),listMountItems);
			}
			if(listMountItems==null || listMountItems.Count<1){
				return new Document[0];
			}
			string strMountItemNums=string.Join(",",listMountItems.Select(x=>POut.Long(x.MountItemNum)));
			string command="SELECT * FROM document WHERE MountItemNum IN("+strMountItemNums+")";
			DataTable table=Db.GetTable(command);
			List<Document> listDocuments=Crud.DocumentCrud.TableToList(table);
			Document[] documentArray=new Document[listMountItems.Count];
			for(int i=0;i<listMountItems.Count;i++){
				documentArray[i]=listDocuments.Find(x=>x.MountItemNum==listMountItems[i].MountItemNum);//frequently null
			}
			return documentArray;
		}

		///<summary>Returns the document for one mountitem. Can be null. Db call.</summary>
		public static Document GetDocumentForMountItem(long mountItemNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Document>(MethodBase.GetCurrentMethod(),mountItemNum);
			}
			string command="SELECT * FROM document WHERE MountItemNum='"+POut.Long(mountItemNum)+"'";
			Document document=Crud.DocumentCrud.SelectOne(command);
			return document;
		}

		///<summary>Used for displaying images attached to DXC claims.  Returns null if the document was not foundd.</summary>
		public static Document GetDocumentForDXC(ClaimAttach claimAttach) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Document>(MethodBase.GetCurrentMethod(),claimAttach);
			}
			string command=$"SELECT * FROM document WHERE FileName='{claimAttach.ActualFileName}'";
			return Crud.DocumentCrud.SelectOne(command);
		}

		///<summary>Any filenames mentioned in the listFiles which are not attached to the given patient are properly attached to that patient. Returns the total number of documents that were newly attached to the patient.</summary>
		public static int InsertMissing(Patient patient,List<string> listFiles) {
			Meth.NoCheckMiddleTierRole();
			int countAdded=0;
			List<long> listDefNumsImgCat=Defs.GetDefsForCategory(DefCat.ImageCats).Select(x => x.DefNum).ToList();
			DataTable table=Documents.GetFileNamesForPatient(patient.PatNum);
			for(int j=0;j<listFiles.Count;j++){
				string fileName=Path.GetFileName(listFiles[j]);
				if(!IsAcceptableFileName(fileName)){
					continue;
				}
				bool inList=false;
				for(int i=0;i<table.Rows.Count && !inList;i++){
					inList=(table.Rows[i]["FileName"].ToString()==fileName);
				}
				if(inList){
					continue;
				}
				//If fileName is not in listFiles
				Document document=new Document();
				string strPrefixResult="";
				Match match=Regex.Match(fileName,@"^_\d*_");//Check for specific category prefix information. Match should only be if we start with _###_ and not anywhere else.
				if(match.Success) {
					strPrefixResult=fileName.Substring(0,match.Length);
					fileName=fileName.Substring(strPrefixResult.Length);
					while(true){
						if(!File.Exists(Path.Combine(Path.GetDirectoryName(listFiles[j]),fileName))){
							break;
						}
						fileName="x"+fileName;
					}
					File.Move(listFiles[j],Path.Combine(Path.GetDirectoryName(listFiles[j]),fileName));
				}
				long prefixCategoryNum=PIn.Long(strPrefixResult.Trim('_'));
				document.DocCategory=Defs.GetFirstForCategory(DefCat.ImageCats,true).DefNum;
				//Check if the category exists, if so move to that category otherwise it will go into the first one
				if(listDefNumsImgCat.Contains(prefixCategoryNum)) {
					document.DocCategory=prefixCategoryNum;
				}
				if(fileName.ToLower().EndsWith(".dcm")) {//DICOM images come with additional metadata we need to collect
					document.ImgType=ImageType.Radiograph;
					BitmapDicom bitmapDicom=DicomHelper.GetFromFile(listFiles[j]);
					DicomHelper.CalculateWindowingOnImport(bitmapDicom);
					document.PrintHeading=true;
					document.WindowingMin=bitmapDicom.WindowingMin;
					document.WindowingMax=bitmapDicom.WindowingMax;
				}
				document.DateCreated=DateTime.Now;
				DateTime dateTPrevious=document.DateTStamp;
				document.Description=fileName;
				document.FileName=fileName;
				document.PatNum=patient.PatNum;
				Insert(document,patient);
				countAdded++;
				string strDocCategory=Defs.GetDef(DefCat.ImageCats,document.DocCategory).ItemName;
				SecurityLogs.MakeLogEntry(EnumPermType.ImageEdit,patient.PatNum,Lans.g("ContrImages","Document Created: A file")+", "+document.FileName+", "
					+Lans.g("ContrImages","placed into the patient's AtoZ images folder from outside of the program was detected and a record automatically inserted into the first image category")+", "+strDocCategory,document.DocNum,dateTPrevious);
			}
			return countAdded;
		}

		///<summary>Returns a datatable containing all filenames of the documents for the supplied patnum.</summary>
		public static DataTable GetFileNamesForPatient(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT FileName FROM document WHERE PatNum='"+patNum+"' ORDER BY FileName";
			return Db.GetTable(command);
		}

		///<Summary>isImagingOrderDescending sorts images by DateCreated (descending). False by default.</Summary>
		public static DataSet RefreshForPatient(long patNum,bool isImagingOrderDescending=false) {
			Meth.NoCheckMiddleTierRole();
			DataSet dataSet=new DataSet();
			dataSet.Tables.Add(GetTreeListTableForPatient(patNum,isImagingOrderDescending));
			return dataSet;
		}

		public static DataTable GetTreeListTableForPatient(long patNum,bool isImagingOrderDescending){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum,isImagingOrderDescending);
			}
			DataConnection dataConnection=new DataConnection();
			DataTable table=new DataTable("DocumentList");
			DataRow dataRow;
			DataTable tableRaw;
			string command;
			//Rows are first added to listObjectsDataRows so they can be sorted at the end as a larger group, then
			//they are placed in the datatable to be returned.
			List<Object> listObjectsDataRows=new List<Object>();
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("DocNum");
			table.Columns.Add("MountNum");
			table.Columns.Add("DocCategory");
			table.Columns.Add("DateCreated");
			table.Columns.Add("idxCategory");//Was previously called docFolder. The index of the category within Defs.
			table.Columns.Add("description");
			table.Columns.Add("ImgType");
			//Move all documents which are invisible to the first document category.
			//Why would a DocCategory ever be set to -1?  Where does that happen?
			//Also finds all document rows for the patient where the DocCategory is not a valid Image Category
			List<Def> listDefs=Defs.GetCatList((int)DefCat.ImageCats).ToList();
			command="SELECT DocNum FROM document WHERE PatNum="+POut.Long(patNum)+" AND (DocCategory NOT IN (";
			for(int i=0;i<listDefs.Count;i++){
				if(i>0){
					command+=",";
				}
				command+=POut.Long(listDefs[i].DefNum);
			}
			command+=") OR DocCategory < 0)";
			tableRaw=dataConnection.GetTable(command);
			if(tableRaw.Rows.Count>0){//Are there any invisible documents?
				command="UPDATE document SET DocCategory='"+Defs.GetFirstForCategory(DefCat.ImageCats,true).DefNum
					+"' WHERE PatNum='"+POut.Long(patNum)+"' AND (";
				for(int i=0;i<tableRaw.Rows.Count;i++){
					command+="DocNum='"+PIn.Long(tableRaw.Rows[i]["DocNum"].ToString())+"' ";
					if(i<tableRaw.Rows.Count-1){
						command+="OR ";
					}
				}
				command+=")";
				dataConnection.NonQ(command);
			}
			//Load all documents into the result table.
			command="SELECT DocNum,DocCategory,DateCreated,Description,ImgType,MountItemNum FROM document WHERE PatNum='"+POut.Long(patNum)+"'";
			tableRaw=dataConnection.GetTable(command);
			for(int i=0;i<tableRaw.Rows.Count;i++){
				//Make sure hidden documents are never added (there is a small possibility that one is added after all are made visible).
				if(Defs.GetOrder(DefCat.ImageCats,PIn.Long(tableRaw.Rows[i]["DocCategory"].ToString()))<0){ 
					continue;
				}
				//Do not add individual documents which are part of a mount object.
				if(PIn.Long(tableRaw.Rows[i]["MountItemNum"].ToString())!=0) {
					continue;
				}
				dataRow=table.NewRow();
				dataRow["DocNum"]=PIn.Long(tableRaw.Rows[i]["DocNum"].ToString());
				dataRow["MountNum"]=0;
				dataRow["DocCategory"]=PIn.Long(tableRaw.Rows[i]["DocCategory"].ToString());
				dataRow["DateCreated"]=PIn.Date(tableRaw.Rows[i]["DateCreated"].ToString());
				dataRow["idxCategory"]=Defs.GetOrder(DefCat.ImageCats,PIn.Long(tableRaw.Rows[i]["DocCategory"].ToString()));
				dataRow["description"]=//PIn.Date(raw.Rows[i]["DateCreated"].ToString()).ToString("d")+": "+
					PIn.String(tableRaw.Rows[i]["Description"].ToString());
				dataRow["ImgType"]=PIn.Long(tableRaw.Rows[i]["ImgType"].ToString());
				listObjectsDataRows.Add(dataRow);
			}
			//Move all mounts which are invisible to the first document category.
			//Why would a DocCategory ever be set to -1?  Where does that happen?
			command="SELECT MountNum FROM mount WHERE PatNum='"+POut.Long(patNum)+"' AND "
				+"DocCategory<0";
			tableRaw=dataConnection.GetTable(command);
			if(tableRaw.Rows.Count>0) {//Are there any invisible mounts?
				command="UPDATE mount SET DocCategory='"+Defs.GetFirstForCategory(DefCat.ImageCats,true).DefNum
					+"' WHERE PatNum='"+POut.Long(patNum)+"' AND (";
				for(int i=0;i<tableRaw.Rows.Count;i++) {
					command+="MountNum='"+PIn.Long(tableRaw.Rows[i]["MountNum"].ToString())+"' ";
					if(i<tableRaw.Rows.Count-1) {
						command+="OR ";
					}
				}
				command+=")";
				dataConnection.NonQ(command);
			}
			//Load all mounts into the result table.
			command="SELECT MountNum,DocCategory,DateCreated,Description FROM mount WHERE PatNum='"+POut.Long(patNum)+"'";
			tableRaw=dataConnection.GetTable(command);
			for(int i=0;i<tableRaw.Rows.Count;i++){
				//Make sure hidden mounts are never added (there is a small possibility that one is added after all are made visible).
				if(Defs.GetOrder(DefCat.ImageCats,PIn.Long(tableRaw.Rows[i]["DocCategory"].ToString()))<0) {
					continue;
				}
				dataRow=table.NewRow();
				dataRow["DocNum"]=0;
				dataRow["MountNum"]=PIn.Long(tableRaw.Rows[i]["MountNum"].ToString());
				dataRow["DocCategory"]=PIn.Long(tableRaw.Rows[i]["DocCategory"].ToString());
				dataRow["DateCreated"]=PIn.Date(tableRaw.Rows[i]["DateCreated"].ToString());
				dataRow["idxCategory"]=Defs.GetOrder(DefCat.ImageCats,PIn.Long(tableRaw.Rows[i]["DocCategory"].ToString()));
				dataRow["description"]=//PIn.Date(raw.Rows[i]["DateCreated"].ToString()).ToString("d")+": "+
					PIn.String(tableRaw.Rows[i]["Description"].ToString());
				dataRow["ImgType"]=0;//Not an image type at all.  It's a mount.
				listObjectsDataRows.Add(dataRow);
			}
			//We must sort the results after they are returned from the database, because the database software (i.e. MySQL)
			//cannot return sorted results from two or more result sets like we have here.
			listObjectsDataRows.Sort(delegate(Object object1,Object object2) {
				DataRow dataRow1=(DataRow)object1;
				DataRow dataRow2=(DataRow)object2;
				int idxCategory1=Convert.ToInt32(dataRow1["idxCategory"].ToString());
				int idxCategory2=Convert.ToInt32(dataRow2["idxCategory"].ToString());
				if(idxCategory1<idxCategory2){
					return -1;
				}
				else if(idxCategory1>idxCategory2){
					return 1;
				}
				if(isImagingOrderDescending){
					return PIn.Date(dataRow2["DateCreated"].ToString()).CompareTo(PIn.Date(dataRow1["DateCreated"].ToString()));
				}
				return PIn.Date(dataRow1["DateCreated"].ToString()).CompareTo(PIn.Date(dataRow2["DateCreated"].ToString()));
			});
			//Finally, move the results from the list into a data table.
			for(int i=0;i<listObjectsDataRows.Count;i++){
				table.Rows.Add((DataRow)listObjectsDataRows[i]);
			}
			return table;
		}

		///<summary>Gets the documents and mounts for a patient, mimicking the tree list in the Images Module. The table returned contains all mounts and all the documents not associated with mounts.
		///Results are ordered by DateCreated and are paginated for the API.</summary>
		public static DataTable GetTreeListTableForPatientForApi(long patNum,string filePath,int limit,int offset,string dateTimeFormatString) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum,filePath,limit,offset,dateTimeFormatString);
			}
			DataTable tableReturn=new DataTable("DocumentListForApi");
			DataRow dataRow;
			DataTable tableDocuments;
			DataTable tableMounts;
			string command;
			//Rows are first added to the result list so they can be sorted at the end as a larger group, then
			//they are placed in the datatable to be returned.
			List<DataRow> listDataRows=new List<DataRow>();
			tableReturn.Columns.Add("DocNum");
			tableReturn.Columns.Add("MountNum");
			tableReturn.Columns.Add("filePath");
			tableReturn.Columns.Add("Description");
			tableReturn.Columns.Add("PatNum");
			tableReturn.Columns.Add("Note");
			tableReturn.Columns.Add("DateCreated");
			tableReturn.Columns.Add("docCategory");
			tableReturn.Columns.Add("DocCategory");
			tableReturn.Columns.Add("ToothNumbers");
			tableReturn.Columns.Add("ProvNum");
			tableReturn.Columns.Add("PrintHeading");
			tableReturn.Columns.Add("DateTStamp");
			tableReturn.Columns.Add("serverDateTime");
			command="SELECT "+DbHelper.Now();
			DateTime dateTimeServer=PIn.DateT(Db.GetScalar(command)); //run first for rigorous inclusion of documents
			command="SELECT DocNum,FileName,Description,PatNum,Note,DateCreated,DocCategory,ToothNumbers,ProvNum,PrintHeading,DateTStamp FROM document WHERE PatNum='"+POut.Long(patNum)+"' AND MountItemNum=0"; //select all documents not associated with mounts
			tableDocuments=Db.GetTable(command);
			command="SELECT MountNum,Description,PatNum,Note,DocCategory,DateCreated,ProvNum FROM mount WHERE PatNum='"+POut.Long(patNum)+"'"; //select all mounts for patient
			tableMounts=Db.GetTable(command);
			List<Def> listDefsImgCat=Defs.GetDefsForCategory(DefCat.ImageCats,isShort:false);
			//Add documents to results list
			for(int i=0;i<tableDocuments.Rows.Count;i++) {
				dataRow=tableReturn.NewRow();
				dataRow["DocNum"]=PIn.Long(tableDocuments.Rows[i]["DocNum"].ToString());
				dataRow["MountNum"]=0;
				dataRow["filePath"]=filePath+tableDocuments.Rows[i]["FileName"].ToString();
				dataRow["Description"]=PIn.Date(tableDocuments.Rows[i]["DateCreated"].ToString()).ToString("d")+": "+tableDocuments.Rows[i]["Description"];
				dataRow["PatNum"]=PIn.Long(tableDocuments.Rows[i]["PatNum"].ToString());
				dataRow["Note"]=tableDocuments.Rows[i]["Note"].ToString();
				dataRow["DateCreated"]=PIn.Date(tableDocuments.Rows[i]["DateCreated"].ToString()).ToString(dateTimeFormatString);//because API is expecting this format
				long docCategoryDefNum=PIn.Long(tableDocuments.Rows[i]["DocCategory"].ToString());
				string defName=Defs.GetName(DefCat.ImageCats,docCategoryDefNum,listDefsImgCat);
				dataRow["docCategory"]=defName;
				dataRow["DocCategory"]=docCategoryDefNum;
				dataRow["ToothNumbers"]=tableDocuments.Rows[i]["ToothNumbers"].ToString();
				dataRow["ProvNum"]=PIn.Long(tableDocuments.Rows[i]["ProvNum"].ToString());
				dataRow["PrintHeading"]=PIn.Bool(tableDocuments.Rows[i]["PrintHeading"].ToString()).ToString().ToLower();//Since we format bools as lowercase in API
				dataRow["DateTStamp"]=PIn.Date(tableDocuments.Rows[i]["DateTStamp"].ToString()).ToString(dateTimeFormatString);
				dataRow["serverDateTime"]=dateTimeServer.ToString(dateTimeFormatString);
				listDataRows.Add(dataRow);
			}
			//Add mounts to results list
			for(int i=0;i<tableMounts.Rows.Count;i++) {
				dataRow=tableReturn.NewRow();
				dataRow["DocNum"]=0;
				dataRow["MountNum"]=PIn.Long(tableMounts.Rows[i]["MountNum"].ToString());
				dataRow["PatNum"]=PIn.Long(tableMounts.Rows[i]["PatNum"].ToString());
				dataRow["filePath"]=""; //not a field for Mounts
				dataRow["Description"]=PIn.Date(tableMounts.Rows[i]["DateCreated"].ToString()).ToShortDateString()+": "+PIn.String(tableMounts.Rows[i]["Description"].ToString());
				dataRow["Note"]=tableMounts.Rows[i]["Note"].ToString();
				dataRow["DateCreated"]=PIn.Date(tableMounts.Rows[i]["DateCreated"].ToString()).ToString(dateTimeFormatString);
				long docCategoryDefNum=PIn.Long(tableMounts.Rows[i]["DocCategory"].ToString());
				string defName=Defs.GetName(DefCat.ImageCats,docCategoryDefNum,listDefsImgCat);
				dataRow["docCategory"]=defName;
				dataRow["DocCategory"]=docCategoryDefNum;
				dataRow["ToothNumbers"]="";//not a field for Mounts
				dataRow["ProvNum"]=PIn.Long(tableMounts.Rows[i]["ProvNum"].ToString());
				dataRow["PrintHeading"]=""; //not a field for Mounts
				dataRow["DateTStamp"]=""; //not a field for Mounts
				dataRow["serverDateTime"]=dateTimeServer.ToString(dateTimeFormatString);
				listDataRows.Add(dataRow);
			}
			listDataRows=listDataRows.OrderBy(x => x["DateCreated"]).ToList();
			//Move the results from the list into a data table, paginate results.
			for(int i=offset;i<listDataRows.Count;i++) {
				if(i>=offset+limit) {
					break;
				}
				tableReturn.Rows.Add(listDataRows[i]);
			}
			return tableReturn;
		}

		///<summary>Returns false if the file is a specific short file name that is not accepted.</summary>
		public static bool IsAcceptableFileName(string fileName) {
			Meth.NoCheckMiddleTierRole();
			List<string> listBadFileNames=new List<string>();
			listBadFileNames.Add("thumbs.db");
			for(int i=0;i<listBadFileNames.Count;i++) {
				if(fileName.Length>=listBadFileNames[i].Length && 
					fileName.Substring(fileName.Length-listBadFileNames[i].Length,
					listBadFileNames[i].Length).ToLower()==listBadFileNames[i]) 
				{
					return false;
				}
			}
			if(fileName.StartsWith(".")) {//Extension-only file, ignore.
				return false;
			}
			return true;
		}

		//public static string GetFull

		///<summary>Only documents listed in the corresponding rows of the statement table are uploaded</summary>
		public static List<long> GetChangedSinceDocumentNums(DateTime dateT,List<long> listStatementNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),dateT,listStatementNums);
			}
			string strStatementNums="";
			DataTable table;
			if(listStatementNums.Count>0) {
				for(int i=0;i<listStatementNums.Count;i++) {
					if(i>0) {
						strStatementNums+="OR ";
					}
					strStatementNums+="StatementNum='"+listStatementNums[i].ToString()+"' ";
				}
				string command="SELECT DocNum FROM document WHERE  DateTStamp > "+POut.DateT(dateT)+" AND DocNum IN ( SELECT DocNum FROM statement WHERE "+strStatementNums+")";
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			List<long> listDocNums = new List<long>(table.Rows.Count);
			for(int i=0;i<table.Rows.Count;i++) {
				listDocNums.Add(PIn.Long(table.Rows[i]["DocNum"].ToString()));
			}
			return listDocNums;
		}

		///<summary>Used along with GetChangedSinceDocumentNums</summary>
		public static List<Document> GetMultDocuments(List<long> listDocNums,string atoZpath) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Document>>(MethodBase.GetCurrentMethod(),listDocNums,atoZpath);
			}
			string strDocNums="";
			DataTable table;
			if(listDocNums.Count>0) {
				for(int i=0;i<listDocNums.Count;i++) {
					if(i>0) {
						strDocNums+="OR ";
					}
					strDocNums+="DocNum='"+listDocNums[i].ToString()+"' ";
				}
				string command="SELECT * FROM document WHERE "+strDocNums;
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			List<Document> listDocuments=Crud.DocumentCrud.TableToList(table);
			for(int i=0;i<listDocuments.Count;i++){
				if(!string.IsNullOrEmpty(listDocuments[i].RawBase64)){
					continue;
				}
				Patient patient=Patients.GetPat(listDocuments[i].PatNum);
				string filePathAndName=ImageStore.GetFilePath(Documents.GetByNum(listDocuments[i].DocNum),atoZpath);
				if(!File.Exists(filePathAndName)){
					continue;
				}
				FileStream fileStream=new FileStream(filePathAndName,FileMode.Open,FileAccess.Read);
				byte[] byteArray=new byte[fileStream.Length];
				fileStream.Read(byteArray,0,(int)fileStream.Length);
				fileStream.Close();
				listDocuments[i].RawBase64=Convert.ToBase64String(byteArray);
			}
			return listDocuments;
		}

		///<summary>Changes the value of the DateTStamp column to the current time stamp for all documents of a patient</summary>
		public static void ResetTimeStamps(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			string command="UPDATE document SET DateTStamp = CURRENT_TIMESTAMP WHERE PatNum ="+POut.Long(patNum);
			Db.NonQ(command);
		}

		///<summary>Moves one document from one patient to another and updates the file name accordingly.
		///Only call when physically storing images in a folder share and after the physical images have been successfully copied over to the "to patient" folder.</summary>
		public static void MergePatientDocument(long patNumFrom,long patNumTo,string fileNameOld,string fileNameNew) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNumFrom,patNumTo,fileNameOld,fileNameNew);
				return;
			}
			string command="UPDATE document"
				+" SET PatNum="+POut.Long(patNumTo)+","
				+" FileName='"+POut.String(fileNameNew)+"'"
				+" WHERE PatNum="+POut.Long(patNumFrom)
				+" AND FileName='"+POut.String(fileNameOld)+"'";
			Db.NonQ(command);
		}

		///<summary>Moves all documents from one patient to another.
		///Only call when physically storing images in a folder share and only if every document.Filename matches a file in patNumTo's folder.</summary>
		public static void MergePatientDocuments(long patNumFrom,long patNumTo) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNumFrom,patNumTo);
				return;
			}
			string command="UPDATE document"
				+" SET PatNum="+POut.Long(patNumTo)
				+" WHERE PatNum="+POut.Long(patNumFrom);
			Db.NonQ(command);
		}

		///<summary>Attempts to open the document using the default program. If not using AtoZfolder saves a local temp file and opens it. Returns empty string on success, otherwise returns error message.</summary>
		public static string OpenDoc(long docNum) {
			Meth.NoCheckMiddleTierRole();
			Document document=Documents.GetByNum(docNum);
			if(document.DocNum==0) {
				return "";
			}
			Patient patient=Patients.GetPat(document.PatNum);
			if(patient==null) {
				return "";
			}
			string documentPath;
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				documentPath=ImageStore.GetFilePath(document,ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath()));
			}
			else if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				//Some programs require a file on disk and cannot open in memory files. Save to temp file from DB.
				documentPath=PrefC.GetRandomTempFile(ImageStore.GetExtension(document));
				File.WriteAllBytes(documentPath,Convert.FromBase64String(document.RawBase64));
			}
			else {//Cloud storage
				//Download file to temp directory
				byte[] byteArray=CloudStorage.Download(ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath())
					,document.FileName);
				documentPath=PrefC.GetRandomTempFile(ImageStore.GetExtension(document));
				File.WriteAllBytes(documentPath,byteArray);
				if(ODBuild.IsThinfinity()) {
					ThinfinityUtils.HandleFile(documentPath);
					return "";
				}
			}
			try { 
				Process.Start(documentPath);
			}
			catch(Exception ex) {
				return Lans.g("Documents","Error occurred while attempting to open document.\r\n"+
					"Verify a default application has been selected to open files of type: ")+
					Path.GetExtension(document.FileName)+"\r\n"+ex.Message;
			}
			return "";
		}

		//Checks to see if the document exists in the correct location, or checks DB for stored content.
		public static bool DocExists(long docNum) {
			Meth.NoCheckMiddleTierRole();
			Document document=Documents.GetByNum(docNum);
			if(document.DocNum==0) {
				return false;
			}
			Patient patient=Patients.GetPat(document.PatNum);
			if(patient==null) {
				return false;
			}
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				return File.Exists(ImageStore.GetFilePath(document,ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath())));
			}
			else if(CloudStorage.IsCloudStorage) {
				return CloudStorage.FileExists(ODFileUtils.CombinePaths(ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath()),document.FileName,'/'));
			}
			return !string.IsNullOrEmpty(document.RawBase64);
		}

		///<summary>Returns true if a Document with the external GUID is found in the database.</summary>
		public static bool DocExternalExists(string externalGUID,ExternalSourceType externalSourceType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),externalGUID,externalSourceType);
			}
			string command="SELECT COUNT(*) FROM document"
				+" WHERE document.ExternalGUID='"+POut.String(externalGUID)+"'"
				+" AND document.ExternalSource='"+POut.String(externalSourceType.ToString())+"'";
			if(Db.GetCount(command)!="0") {
				return true;
			}
			return false;
		}

		///<summary>Returns the filepath of the document if using AtoZfolder. If storing files in DB or third party storage, saves document to local temp file and returns filepath.
		/// Empty string if not found.</summary>
		public static string GetPath(long docNum) {
			Meth.NoCheckMiddleTierRole();
			Document document=Documents.GetByNum(docNum);
			if(document.DocNum==0) {
				return "";
			}
			Patient patient=Patients.GetPat(document.PatNum);
			if(patient==null) {
				return "";
			}
			string documentPath;
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				documentPath=ImageStore.GetFilePath(document,ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath()));
			}
			else if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				//Some programs require a file on disk and cannot open in memory files. Save to temp file from DB.
				documentPath=PrefC.GetRandomTempFile(ImageStore.GetExtension(document));
				File.WriteAllBytes(documentPath,Convert.FromBase64String(document.RawBase64));
			}
			else {//Cloud storage
				//Download file to temp directory
				byte[] byteArray=CloudStorage.Download(ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath())
					,document.FileName);
				documentPath=PrefC.GetRandomTempFile(ImageStore.GetExtension(document));
				File.WriteAllBytes(documentPath,byteArray);
			}
			return documentPath;
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching docNum as FKey and are related to Document.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the Document table type.</summary>
		public static void ClearFkey(long docNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),docNum);
				return;
			}
			Crud.DocumentCrud.ClearFkey(docNum);
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching docNums as FKey and are related to Document.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the Document table type.</summary>
		public static void ClearFkey(List<long> listDocNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listDocNums);
				return;
			}
			Crud.DocumentCrud.ClearFkey(listDocNums);
		}

		#region Xam Methods
		/// <summary>Returns a bool indicating if the file was created sucessfully for a temporary PDF file for passed in payment plan.</summary>
		public static bool TryCreatePayPlanPdfFile(PayPlan paymentPlan, out string filePath) {
			Meth.NoCheckMiddleTierRole();
			try {
				SheetDrawingJob sheetDrawingJob=new SheetDrawingJob();
				Sheet sheetPayPlan=PayPlans.PayPlanToSheet(paymentPlan);
				SheetParameter.SetParameter(sheetPayPlan, "keyData", PayPlans.GetKeyDataForSignature(paymentPlan));
				SheetUtil.CalculateHeights(sheetPayPlan);
				filePath=PrefC.GetRandomTempFile(".pdf");
				PdfDocument pdfDocument=sheetDrawingJob.CreatePdf(sheetPayPlan);
				SheetDrawingJob.SavePdfToFile(pdfDocument,filePath);
			}
			catch(Exception e) {
				Logger.WriteException(e, "PaymentPlansCreatePDF");
				filePath="";
				return false;
			}
			//Create a PDF with the given sheet and file. The other parameters can remain null, because they aren't used for PayPlan sheets.
			return true;
		}

		///<summary>Will create a payment plan PDF without any references to OpenDental. This method is only used in areas that don't have access to 
		///OpenDental and should only be used in such a case. Returns true if it successfully saved a document.</summary>
		public static bool CreateAndSavePaymentPlanPDF(PayPlan paymentPlan) {
			Meth.NoCheckMiddleTierRole();
			try {
				Patient patient=Patients.GetPat(paymentPlan.PatNum);
				//Determine each of the document categories that this PP should be saved to.
				//"A"==PaymentPlan; See DefCat.ImageCats 
				List<Def> listDefsImageCat=Defs.GetDefsForCategory(DefCat.ImageCats,true);
				List<long> listDocCategories=listDefsImageCat.Where(x => x.ItemValue.Contains("A")).Select(x=>x.DefNum).ToList();
				if(listDocCategories.Count==0) {
					//we must save at least one document, pick first non-hidden image category.
					Def defImgCat=listDefsImageCat.Find(x => !x.IsHidden);
					if(defImgCat==null) {
						Logger.WriteLine("There are currently no image categories","PaymentPlans");
						return false;
					}
					listDocCategories.Add(defImgCat.DefNum);
				}
				if(!TryCreatePayPlanPdfFile(paymentPlan,out string tempFile)){
					return false;
				}
				string rawBase64="";
				if(PrefC.AtoZfolderUsed!=DataStorageType.LocalAtoZ) {
					//Convert the pdf into its raw bytes
					rawBase64=Convert.ToBase64String(System.IO.File.ReadAllBytes(tempFile));
				}
				for(int i=0;i<listDocCategories.Count;i++) {//usually only one, but do allow them to be saved once per image category.
					Document documentToSave=new Document();
					documentToSave.DocNum=Insert(documentToSave);
					string fileName="PayPlanArchive"+documentToSave.DocNum;
					documentToSave.ImgType=ImageType.Document;
					documentToSave.DateCreated=DateTime.Now;
					documentToSave.PatNum=paymentPlan.PatNum;
					documentToSave.DocCategory=listDocCategories[i];
					documentToSave.Description=fileName;//no extension.
					documentToSave.RawBase64=rawBase64;//blank if using AtoZfolder
					if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
						string filePath=ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
						while(File.Exists(filePath+"\\"+fileName+".pdf")) {
							fileName+="x";
						}
						File.Copy(tempFile,filePath+"\\"+fileName+".pdf");
					}
					else if(CloudStorage.IsCloudStorage) {
						//Upload file to patient's AtoZ folder
						CloudStorage.Upload(ImageStore.GetPatientFolder(patient,"")
							,fileName+".pdf"
							,File.ReadAllBytes(tempFile));
					}
					documentToSave.FileName=fileName+".pdf";//file extension used for both DB images and AtoZ images
					Update(documentToSave);
				}
			}
			catch(Exception ex) {
				Logger.WriteException(ex,"PaymentPlans");
				return false;
			}
			return true;
		}

		/// <summary>Returns empty string if the file was created sucessfully for a temporary PDF file for passed in treatment plan. Or returns error message if the file was not created.</summary>
		public static string TryCreateTreatmentPlanPdfFile(TreatPlan treatPlan, out string filePath) {
			Meth.NoCheckMiddleTierRole();
			try { 
				SheetDrawingJob sheetDrawingJob=new SheetDrawingJob();
				Sheet sheetTP=TreatPlans.CreateSheetFromTreatmentPlan(treatPlan);
				filePath=PrefC.GetRandomTempFile(".pdf");
				//Create a PDF with the given sheet and file. The other parameters can remain null, because they aren't used for TreatPlan sheets.
				PdfDocument pdfDocument=sheetDrawingJob.CreatePdf(sheetTP);
				SheetDrawingJob.SavePdfToFile(pdfDocument,filePath);
			}
			catch(Exception e) {
				Logger.WriteException(e,"TreatmentPlansCreatePDF");
				filePath="";
				return e.Message;
			}
			return "";
		}

		///<summary>Will create a treatment plan PDF without any references to OpenDental. This method is only used in areas that don't have access to 
		///OpenDental and should only be used in such a case. Returns empty string if it successfully saved a document.</summary>
		public static string CreateAndSaveTreatmentPlanPDF(TreatPlan treatPlan) {
			Meth.NoCheckMiddleTierRole();
			string errorMessage="";
			try {
				Patient patient=Patients.GetPat(treatPlan.PatNum);
				List<Def> listDefsImageCat=Defs.GetDefsForCategory(DefCat.ImageCats,true);
				List<long> listDocCategories=listDefsImageCat.Where(x => x.ItemValue.Contains("R")).Select(x=>x.DefNum).ToList();
				if(listDocCategories.Count==0) {
					//we must save at least one document, pick first non-hidden image category.
					Def defImgCat=listDefsImageCat.Find(x => !x.IsHidden);
					if(defImgCat==null) {
						errorMessage="There are currently no image categories.";
						Logger.WriteLine(errorMessage,"TreatmentPlans");
						return errorMessage;
					}
					listDocCategories.Add(defImgCat.DefNum);
				}
				errorMessage=TryCreateTreatmentPlanPdfFile(treatPlan,out string tempFile);
				if(!string.IsNullOrWhiteSpace(errorMessage)) {
					return errorMessage;
				}
				string rawBase64="";
				if(PrefC.AtoZfolderUsed!=DataStorageType.LocalAtoZ) {
					//Convert the pdf into its raw bytes
					rawBase64=Convert.ToBase64String(System.IO.File.ReadAllBytes(tempFile));
				}
				for(int i=0;i<listDocCategories.Count;i++) {//usually only one, but do allow them to be saved once per image category.
					Document documentToSave=new Document();
					documentToSave.DocNum=Insert(documentToSave);
					string fileName="TPArchive"+documentToSave.DocNum;
					documentToSave.ImgType=ImageType.Document;
					documentToSave.DateCreated=DateTime.Now;
					documentToSave.PatNum=treatPlan.PatNum;
					documentToSave.DocCategory=listDocCategories[i];
					documentToSave.Description=fileName;//no extension.
					documentToSave.RawBase64=rawBase64;//blank if using AtoZfolder
					if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
						string filePath=ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
						while(File.Exists(filePath+"\\"+fileName+".pdf")) {
							fileName+="x";
						}
						File.Copy(tempFile,filePath+"\\"+fileName+".pdf");
					}
					else if(CloudStorage.IsCloudStorage) {
						//Upload file to patient's AtoZ folder
						CloudStorage.Upload(ImageStore.GetPatientFolder(patient,"")
							,fileName+".pdf"
							,File.ReadAllBytes(tempFile));
					}
					documentToSave.FileName=fileName+".pdf";//file extension used for both DB images and AtoZ images
					Update(documentToSave);
				}
			}
			catch(Exception ex) {
				Logger.WriteException(ex,"TreatmentPlans");
				return ex.Message;
			}
			return "";
		}

		///<summary>Throws exception. Creates and Saves a PDF document for the given statement.</summary>
		public static DataSet CreateAndSaveStatementPDF(Statement statement,SheetDef sheetDef,bool isLimitedCustom,bool showLName,bool excludeTxfr,List<Def> listDefsImageCat,string pdfFileName="",Sheet sheet=null,DataSet dataSet=null,string description="") {
			Meth.NoCheckMiddleTierRole();
			string tempPath;
			if(statement==null || sheetDef==null) {
				return null;
			}
			if(dataSet==null) {
				if(isLimitedCustom) {
					dataSet=AccountModules.GetSuperFamAccount(statement,doIncludePatLName:showLName,doShowHiddenPaySplits:statement.IsReceipt,doExcludeTxfrs:excludeTxfr);
				}
				else {
					long patNum=Statements.GetPatNumForGetAccount(statement);
					dataSet=AccountModules.GetAccount(patNum,statement,doIncludePatLName:showLName,doShowHiddenPaySplits:statement.IsReceipt,doExcludeTxfrs:excludeTxfr);
				}
			}
			if(pdfFileName=="") {
				if(sheet==null) {
					sheet=SheetUtil.CreateSheet(sheetDef,statement.PatNum,statement.HidePayment);
					sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=statement });
					SheetFiller.FillFields(sheet,dataSet,statement);
					SheetUtil.CalculateHeights(sheet,dataSet,statement);
				}
				tempPath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),statement.PatNum.ToString()+".pdf");
				SheetPrinting.CreatePdf(sheet,tempPath,statement,dataSet,null);
			}
			else {
				tempPath=pdfFileName;
			}
			long docCategory=0;
			for(int i=0;i<listDefsImageCat.Count;i++) {
				if(Regex.IsMatch(listDefsImageCat[i].ItemValue,@"S")) {
					docCategory=listDefsImageCat[i].DefNum;
					break;
				}
			}
			if(docCategory==0) {
				docCategory=listDefsImageCat[0].DefNum;//put it in the first category.
			}
			//create doc--------------------------------------------------------------------------------------
			Document document=null;
			try {
				document=ImageStore.Import(tempPath,docCategory,Patients.GetPat(statement.PatNum));
			}
			catch {
				throw;
			}
			finally {
				//Delete the temp file since we don't need it anymore.
				try {
					if(pdfFileName=="") {//If they're passing in a PDF file name, they probably have it open somewhere else.
						File.Delete(tempPath);
					}
				}
				catch {
					//Do nothing.  This file will likely get cleaned up later.
				}
			}
			document.ImgType=ImageType.Document;
			document.Description=description;
			//Some customers have wanted to sort their statements in the images module by date and time.
			//We would need to enhance DateSent to include the time portion.
			statement.DateSent=document.DateCreated;
			statement.DocNum=document.DocNum;//this signals the calling class that the pdf was created successfully.
			Statements.AttachDoc(statement.StatementNum,document);
			Statements.SyncStatementProdsForStatement(dataSet,statement.StatementNum,statement.DocNum);
			return dataSet;
		}

		public static bool IsVC2015Installed(){
			Meth.NoCheckMiddleTierRole();
			string dependenciesPath=@"SOFTWARE\Classes\Installer\Dependencies";
			using RegistryKey registryKeyDependencies=Registry.LocalMachine.OpenSubKey(dependenciesPath);
			if(registryKeyDependencies==null) {
				return false;
			}
			List<string> listSubKeyNames=registryKeyDependencies.GetSubKeyNames().Where(n => !n.ToLower().Contains("dotnet") && !n.ToLower().Contains("microsoft")).ToList();
			for(int i=0;i<listSubKeyNames.Count;i++){
				using RegistryKey registryKeySubDir=Registry.LocalMachine.OpenSubKey(dependenciesPath + "\\" +listSubKeyNames[i]);
				string displayNameValue=registryKeySubDir.GetValue("DisplayName")?.ToString()??null;
				if(string.IsNullOrEmpty(displayNameValue)) {
					continue;
				}
				if(Regex.IsMatch(displayNameValue,@"C\+\+ 2015.*\((x86|x64)\)")) { 
					return true;
				}
			}
			return false;
		}
		#endregion Xam Methods

		///<summary>Will create a treatment plan PDF without any references to OpenDental. This method is only used for the API. 
		///Returns a list of DocNums when successfully saved a document. Throws errors, surround with a Try / Catch.</summary>
		public static List<long> CreateAndSaveTreatmentPlanPdfForApi(TreatPlan treatPlan) {
			Meth.NoCheckMiddleTierRole();
			string errorMessage="";
			List<long> listDocNums=new List<long>();
			Patient patient=Patients.GetPat(treatPlan.PatNum);
			List<Def> listDefsImageCat=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			List<long> listDocCategories=listDefsImageCat.Where(x => x.ItemValue.Contains("R")).Select(x=>x.DefNum).ToList();
			if(listDocCategories.Count==0) {
				//we must save at least one document, pick first non-hidden image category.
				Def defImgCat=listDefsImageCat.Find(x => !x.IsHidden);
				if(defImgCat==null) {
					throw new Exception("There are currently no image categories.");
				}
				listDocCategories.Add(defImgCat.DefNum);
			}
			errorMessage=TryCreateTreatmentPlanPdfFile(treatPlan,out string tempFile);
			if(!string.IsNullOrWhiteSpace(errorMessage)) {
				throw new Exception(errorMessage);
			}
			string rawBase64="";
			if(PrefC.AtoZfolderUsed!=DataStorageType.LocalAtoZ) {
				//Convert the pdf into its raw bytes
				rawBase64=Convert.ToBase64String(System.IO.File.ReadAllBytes(tempFile));
			}
			for(int i=0;i<listDocCategories.Count;i++) {//usually only one, but do allow them to be saved once per image category.
				Document documentToSave=new Document();
				documentToSave.DocNum=Insert(documentToSave);
				string fileName="TPArchive"+documentToSave.DocNum;
				documentToSave.ImgType=ImageType.Document;
				documentToSave.DateCreated=DateTime.Now;
				documentToSave.PatNum=treatPlan.PatNum;
				documentToSave.DocCategory=listDocCategories[i];
				documentToSave.Description=fileName;//no extension.
				documentToSave.RawBase64=rawBase64;//blank if using AtoZfolder
				if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
					string filePath=ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
					while(File.Exists(filePath+"\\"+fileName+".pdf")) {
						fileName+="x";
					}
					File.Copy(tempFile,filePath+"\\"+fileName+".pdf");
				}
				else if(CloudStorage.IsCloudStorage) {
					//Upload file to patient's AtoZ folder
					CloudStorage.Upload(ImageStore.GetPatientFolder(patient,"")
						,fileName+".pdf"
						,File.ReadAllBytes(tempFile));
				}
				documentToSave.FileName=fileName+".pdf";//file extension used for both DB images and AtoZ images
				Update(documentToSave);
				listDocNums.Add(documentToSave.DocNum);
			}
			return listDocNums;
		}

	}

	public class DocumentForApi {
		public Document DocumentCur;
		public DateTime DateTimeServer;
	}
}