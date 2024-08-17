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
		public static Document InsertAndGet(Document doc,Patient pat) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Document>(MethodBase.GetCurrentMethod(),doc,pat);
			}
			Insert(doc,pat);
			return GetByNum(doc.DocNum);
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
			Document doc=Crud.DocumentCrud.SelectOne(docNum);
			if(doc==null && !doReturnNullIfNotFound) {
				return new Document();
			}
			return doc;
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
			//No need to check MiddleTierRole; no call to db.
			if(table==null){
				return new Document[0];
			}
			List<Document> list=Crud.DocumentCrud.TableToList(table);
			return list.ToArray();
		}

		/*
		private static Document[] RefreshAndFill(string command) {
			//No need to check MiddleTierRole; no call to db.
			return Fill(Db.GetTable(command));
		}*/

		///<summary>Returns a unique filename for a previously inserted doc based on the pat's first and last name and docNum with the given extension.</summary>
		public static string GetUniqueFileNameForPatient(Patient pat,long docNum,string fileExtension) {
			string retval=new string((pat.LName+pat.FName).Where(x => Char.IsLetter(x)).ToArray())+docNum.ToString()+fileExtension;//ensures unique name
			//there is still a slight chance that someone manually added a file with this name, so quick fix:
			List<string> listUsedNames=GetAllWithPat(pat.PatNum).Select(x => x.FileName).ToList();
			while(listUsedNames.Contains(retval)) {
				retval="x"+retval;
			}
			return retval;
		}

		///<summary>Usually, set just the extension before passing in the doc.  Inserts a new document into db, creates a filename based on Cur.DocNum, and then updates the db with this filename.  Should always refresh the document after calling this method in order to get the correct filename for RemotingRole.ClientWeb.</summary>
		public static long Insert(Document doc,Patient pat) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				doc.DocNum=Meth.GetLong(MethodBase.GetCurrentMethod(),doc,pat);
				return doc.DocNum;
			}
			doc.DocNum=Crud.DocumentCrud.Insert(doc);
			//If the current filename is just an extension, then assign it a unique name.
			if(doc.FileName==Path.GetExtension(doc.FileName)) {
				doc.FileName=GenerateUniqueFileName(doc.FileName,pat,doc.DocNum.ToString());
				//there is still a slight chance that someone manually added a file with this name, so quick fix:
				string command="SELECT FileName FROM document WHERE PatNum="+POut.Long(doc.PatNum);
				DataTable table=Db.GetTable(command);
				string[] usedNames=new string[table.Rows.Count];
				for(int i=0;i<table.Rows.Count;i++) {
					usedNames[i]=PIn.String(table.Rows[i][0].ToString());
				}
				while(IsFileNameInList(doc.FileName,usedNames)) {
					doc.FileName="x"+doc.FileName;
				}
				/*Document[] docList=GetAllWithPat(doc.PatNum);
				while(IsFileNameInList(doc.FileName,docList)) {
					doc.FileName="x"+doc.FileName;
				}*/
				Update(doc);
			}
			return doc.DocNum;
		}

		//Returns a unique file name with the given extension for the specified patient. If uniqueNum is not set, will generate a random number to ensure the filename is unique.
		public static string GenerateUniqueFileName(string extension, Patient patient, string uniqueIdentifier=null) {
			//No need to check MiddleTierRole; no call to db.
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
		public static long Insert(Document doc) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				doc.DocNum=Meth.GetLong(MethodBase.GetCurrentMethod(),doc);
				return doc.DocNum;
			}
			return Crud.DocumentCrud.Insert(doc);
		}

		///<summary></summary>
		public static void Update(Document doc){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),doc);
				return;
			}
			Crud.DocumentCrud.Update(doc);
		}

		///<summary></summary>
		public static bool Update(Document doc,Document docOld){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),doc,docOld);
			}
			return Crud.DocumentCrud.Update(doc,docOld);
		}

		///<summary>Updates all of the mount's Document.DocCategory information when moving a mount.</summary>
		public static void UpdateDocCategoryForMountItems(long mountNum,long docCategory) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mountNum,docCategory);
				return;
			}
			List<MountItem> listMountItems=MountItems.GetItemsForMount(mountNum);
			List<Document> listDocumentsInMountDb=GetDocumentsForMountItems(listMountItems).Where(x => x!=null).ToList();
			if(listDocumentsInMountDb.Count>0) {
				for(int i=0;i<listDocumentsInMountDb.Count;i++) {
					Document docOld=listDocumentsInMountDb[i].Copy();
					listDocumentsInMountDb[i].DocCategory=docCategory;
					Update(listDocumentsInMountDb[i],docOld);
				}
			}
		}

		///<summary></summary>
		public static void Delete(Document doc){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),doc);
				return;
			}
			Crud.DocumentCrud.Delete(doc.DocNum);
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
			Def[] defs=Defs.GetDefsForCategory(DefCat.ImageCats,true).ToArray();
			for(int i=0;i<defs.Length;i++){
				if(Regex.IsMatch(defs[i].ItemValue,@"P")){
					defNumPicts=defs[i].DefNum;
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
			Document[] pictureDocs=Fill(table);
			if(pictureDocs==null || pictureDocs.Length<1){//no pictures
				return null;
			}
			return pictureDocs[0];
		}

		///<summary>Gets all documents within the doc category designated as the "S"tatements directory via definitions.
		///Also gets dependents' statements if this patient is a guarantor (needed by the patient portal).</summary>
		public static List<Document> GetStatementsForPat(long patNum,List<long> listDependents) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Document>>(MethodBase.GetCurrentMethod(),patNum,listDependents);
			}
			string command="SELECT def.DefNum FROM  definition def "
					+"WHERE def.Category="+POut.Int((int)DefCat.ImageCats)+" "
					+"AND def.IsHidden=0  "
					+"AND def.ItemValue LIKE '%S%' ";//Statements category indicator
			List<long> listDefNums=Db.GetListLong(command);
			if(listDefNums.Count==0) {//There are no Statement image categories
				return new List<Document>();
			}
			if(!listDependents.Contains(patNum)) {
				listDependents.Add(patNum);
			}
			command="SELECT * FROM document d "
			+"WHERE d.PatNum IN ("+string.Join(",",listDependents)+") "
				+"AND d.DocCategory IN ("+string.Join(",",listDefNums)+") "
			+"ORDER BY d.DateCreated DESC";
			return Crud.DocumentCrud.SelectMany(command);
		}

		///<summary>Get document info for all images linked to this patient.
		///Also gets dependents' images if this patient is a guarantor (needed by the patient portal).</summary>
		public static List<Document> GetPatientPortalDocsForPat(long patNum,List<long> listDependents) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Document>>(MethodBase.GetCurrentMethod(),patNum,listDependents);
			}
			string command="SELECT def.DefNum FROM  definition def "
					+"WHERE def.Category="+POut.Int((int)DefCat.ImageCats)+" "
					+"AND def.IsHidden=0  "
					+"AND def.ItemValue LIKE '%L%' ";//Patient Portal category indicator
			List<long> listDefNums=Db.GetListLong(command);
			if(listDefNums.Count==0) {//There are no Patient Portal image categories
				return new List<Document>();
			}
			if(!listDependents.Contains(patNum)) {
				listDependents.Add(patNum);
			}
			command="SELECT * FROM document d "
			+"WHERE d.PatNum IN ("+string.Join(",",listDependents)+") "
				+"AND d.DocCategory IN ("+string.Join(",",listDefNums)+") "
			+"ORDER BY d.DateCreated DESC";
			return Crud.DocumentCrud.SelectMany(command);
		}

		/// <summary>Makes one call to the database to retrieve the document of the patient for the given patNum, then uses that document and the patFolder to load and process the patient picture so it appears the same way it did in the image module.  It first creates a 100x100 thumbnail if needed, then it uses the thumbnail. Can return null. Assumes WithPat will always be same as patnum.</summary>
		public static Bitmap GetPatPict(long patNum,string patFolder) {
			//No need to check MiddleTierRole; no call to db.
			Document document=GetPatPictFromDb(patNum);
			Bitmap bitmap=GetPatPict(patNum,patFolder,document);
			return bitmap;
		}

		/// <summary>Uses the passed-in document and the patFolder to load and process the patient picture so it appears the same way it did in the image module.  It first creates a 100x100 thumbnail if needed, then it uses the thumbnail. Can return null. Assumes WithPat will always be same as patnum.</summary>
		public static Bitmap GetPatPict(long patNum,string patFolder,Document document) {
			//No need to check MiddleTierRole; no call to db.
			if(document==null) {
				return null;
			}
			Bitmap bitmap=GetThumbnail(document,patFolder);
			return bitmap;
		}

		///<summary>Gets the thumbnail image for the given document. The thumbnail for every document is in a subfolder named 'Thumbnails' within each patient's images folder.  Always 100x100.</summary>
		public static Bitmap GetThumbnail(Document doc,string patFolder){
			//No need to check MiddleTierRole; no call to db.
			string fileName=doc.FileName;
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
					DateTime thumbModifiedTime=File.GetLastWriteTime(fileNameThumb);
					if(thumbModifiedTime>doc.DateTStamp //thumbnail file is old
						&& !doc.IsCropOld) //Prevents using the existing thumbnail file if the crop data has not yet been converted to the new style. New thumbnail will be created below.
					{	
						Bitmap bitmap = (Bitmap)Bitmap.FromFile(fileNameThumb);
						Bitmap bitmap2 = new Bitmap(bitmap);
						bitmap.Dispose();//releases the file lock
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
				bitmapFullSize=ImageHelper.GetImageCropped(doc,patFolder);
			}
			catch(Exception ex) {
				ex.DoNothing();
				return NoAvailablePhoto();
			}
			if(bitmapFullSize is null){
				return NoAvailablePhoto();
			}
			Bitmap bitmapThumb=ImageHelper.GetBitmapSquare(bitmapFullSize,100);//Thumbnails saved in the thumbnails folder are always 100x100
			bitmapFullSize.Dispose();
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
			//The bitmap passed in already has brightness/contrast applied
			Bitmap bitmapFullSize=ImageHelper.CopyWithCropRotate(document,bitmap);
			Bitmap bitmapThumb=ImageHelper.GetBitmapSquare(bitmapFullSize,100);//Thumbnails saved in the thumbnails folder are always 100x100
			bitmapFullSize.Dispose();
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
			//No need to check MiddleTierRole; no call to db.
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
			string nums=string.Join(",",listMountItems.Select(x=>POut.Long(x.MountItemNum)));
			string command="SELECT * FROM document WHERE MountItemNum IN("+nums+")";
			DataTable table=Db.GetTable(command);
			List<Document> listDocuments=Crud.DocumentCrud.TableToList(table);
			Document[] documentArray=new Document[listMountItems.Count];
			for(int i=0;i<listMountItems.Count;i++){
				documentArray[i]=listDocuments.FirstOrDefault(x=>x.MountItemNum==listMountItems[i].MountItemNum);//frequently null
			}
			return documentArray;
		}

		///<summary>Returns the document for one mountitem. Can be null.</summary>
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

		///<summary>Any filenames mentioned in the fileList which are not attached to the given patient are properly attached to that patient. Returns the total number of documents that were newly attached to the patient.</summary>
		public static int InsertMissing(Patient patient,List<string> fileList) {
			//No need to check MiddleTierRole; no call to db.
			int countAdded=0;
			List<long> listImageDefNums=Defs.GetDefsForCategory(DefCat.ImageCats).Select(x => x.DefNum).ToList();
			DataTable table=Documents.GetFileNamesForPatient(patient.PatNum);
			for(int j=0;j<fileList.Count;j++){
				string fileName=Path.GetFileName(fileList[j]);
				if(!IsAcceptableFileName(fileName)){
					continue;
				}
				bool inList=false;
				for(int i=0;i<table.Rows.Count && !inList;i++){
					inList=(table.Rows[i]["FileName"].ToString()==fileName);
				}
				if(!inList){//OD found new images in the patient's folder that aren't part of the DB.
					Document doc=new Document();
					string resultPrefix="";
					Match match=Regex.Match(fileName,@"^_\d*_");//Check for specific category prefix information. Match should only be if we start with _###_ and not anywhere else.
					if(match.Success) {
						resultPrefix=fileName.Substring(0,match.Length);
						fileName=fileName.Substring(resultPrefix.Length);
						while(File.Exists(Path.Combine(Path.GetDirectoryName(fileList[j]),fileName))) {
							fileName="x"+fileName;
						}
						File.Move(fileList[j],Path.Combine(Path.GetDirectoryName(fileList[j]),fileName));						
					}
					long prefixCategoryNum=PIn.Long(resultPrefix.Trim('_'));
					doc.DocCategory=Defs.GetFirstForCategory(DefCat.ImageCats,true).DefNum;
					//Check if the category exists, if so move to that category otherwise it will go into the first one
					if(listImageDefNums.Contains(prefixCategoryNum)) {
						doc.DocCategory=prefixCategoryNum;
					}
					doc.DateCreated=DateTime.Now;
					DateTime datePrevious=doc.DateTStamp;
					doc.Description=fileName;
					doc.FileName=fileName;
					doc.PatNum=patient.PatNum;
					Insert(doc,patient);
					countAdded++;
					string docCat=Defs.GetDef(DefCat.ImageCats,doc.DocCategory).ItemName;
					SecurityLogs.MakeLogEntry(Permissions.ImageEdit,patient.PatNum,Lans.g("ContrImages","Document Created: A file")+", "+doc.FileName+", "
						+Lans.g("ContrImages","placed into the patient's AtoZ images folder from outside of the program was detected and a record automatically inserted into the first image category")+", "+docCat,doc.DocNum,datePrevious);
				}
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

		///<Summary>Parameters: 1:PatNum</Summary>
		public static DataSet RefreshForPatient(string[] parameters) {
			//No need to check MiddleTierRole; no call to db.
			//Syntax of parameter is a little odd, but seems harmless. Maybe there's a reason.
			DataSet retVal=new DataSet();
			retVal.Tables.Add(GetTreeListTableForPatient(parameters[0]));
			return retVal;
		}

		public static DataTable GetTreeListTableForPatient(string patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum);
			}
			DataConnection dcon=new DataConnection();
			DataTable table=new DataTable("DocumentList");
			DataRow row;
			DataTable raw;
			string command;
			//Rows are first added to the resultSet list so they can be sorted at the end as a larger group, then
			//they are placed in the datatable to be returned.
			List<Object> resultSet=new List<Object>();
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
			List<string> listImageCatDefs=new List<string>();
			Defs.GetCatList((int)DefCat.ImageCats).ForEach(x => listImageCatDefs.Add(x.DefNum.ToString())); // get a list of DefNums from the Image Category of definitions
			command=$"SELECT DocNum FROM document WHERE PatNum={patNum} AND (DocCategory NOT IN ({string.Join(",",listImageCatDefs.ToArray())}) OR DocCategory < 0)";
			raw=dcon.GetTable(command);
			if(raw.Rows.Count>0){//Are there any invisible documents?
				command="UPDATE document SET DocCategory='"+Defs.GetFirstForCategory(DefCat.ImageCats,true).DefNum
					+"' WHERE PatNum='"+patNum+"' AND (";
				for(int i=0;i<raw.Rows.Count;i++){
					command+="DocNum='"+PIn.Long(raw.Rows[i]["DocNum"].ToString())+"' ";
					if(i<raw.Rows.Count-1){
						command+="OR ";
					}
				}
				command+=")";
				dcon.NonQ(command);
			}
			//Load all documents into the result table.
			command="SELECT DocNum,DocCategory,DateCreated,Description,ImgType,MountItemNum FROM document WHERE PatNum='"+patNum+"'";
			raw=dcon.GetTable(command);
			for(int i=0;i<raw.Rows.Count;i++){
				//Make sure hidden documents are never added (there is a small possibility that one is added after all are made visible).
				if(Defs.GetOrder(DefCat.ImageCats,PIn.Long(raw.Rows[i]["DocCategory"].ToString()))<0){ 
					continue;
				}
				//Do not add individual documents which are part of a mount object.
				if(PIn.Long(raw.Rows[i]["MountItemNum"].ToString())!=0) {
					continue;
				}
				row=table.NewRow();
				row["DocNum"]=PIn.Long(raw.Rows[i]["DocNum"].ToString());
				row["MountNum"]=0;
				row["DocCategory"]=PIn.Long(raw.Rows[i]["DocCategory"].ToString());
				row["DateCreated"]=PIn.Date(raw.Rows[i]["DateCreated"].ToString());
				row["idxCategory"]=Defs.GetOrder(DefCat.ImageCats,PIn.Long(raw.Rows[i]["DocCategory"].ToString()));
				row["description"]=//PIn.Date(raw.Rows[i]["DateCreated"].ToString()).ToString("d")+": "+
					PIn.String(raw.Rows[i]["Description"].ToString());
				row["ImgType"]=PIn.Long(raw.Rows[i]["ImgType"].ToString());
				resultSet.Add(row);
			}
			//Move all mounts which are invisible to the first document category.
			//Why would a DocCategory ever be set to -1?  Where does that happen?
			command="SELECT MountNum FROM mount WHERE PatNum='"+patNum+"' AND "
				+"DocCategory<0";
			raw=dcon.GetTable(command);
			if(raw.Rows.Count>0) {//Are there any invisible mounts?
				command="UPDATE mount SET DocCategory='"+Defs.GetFirstForCategory(DefCat.ImageCats,true).DefNum
					+"' WHERE PatNum='"+patNum+"' AND (";
				for(int i=0;i<raw.Rows.Count;i++) {
					command+="MountNum='"+PIn.Long(raw.Rows[i]["MountNum"].ToString())+"' ";
					if(i<raw.Rows.Count-1) {
						command+="OR ";
					}
				}
				command+=")";
				dcon.NonQ(command);
			}
			//Load all mounts into the result table.
			command="SELECT MountNum,DocCategory,DateCreated,Description FROM mount WHERE PatNum='"+patNum+"'";
			raw=dcon.GetTable(command);
			for(int i=0;i<raw.Rows.Count;i++){
				//Make sure hidden mounts are never added (there is a small possibility that one is added after all are made visible).
				if(Defs.GetOrder(DefCat.ImageCats,PIn.Long(raw.Rows[i]["DocCategory"].ToString()))<0) {
					continue;
				}
				row=table.NewRow();
				row["DocNum"]=0;
				row["MountNum"]=PIn.Long(raw.Rows[i]["MountNum"].ToString());
				row["DocCategory"]=PIn.Long(raw.Rows[i]["DocCategory"].ToString());
				row["DateCreated"]=PIn.Date(raw.Rows[i]["DateCreated"].ToString());
				row["idxCategory"]=Defs.GetOrder(DefCat.ImageCats,PIn.Long(raw.Rows[i]["DocCategory"].ToString()));
				row["description"]=//PIn.Date(raw.Rows[i]["DateCreated"].ToString()).ToString("d")+": "+
					PIn.String(raw.Rows[i]["Description"].ToString());
				row["ImgType"]=0;//Not an image type at all.  It's a mount.
				resultSet.Add(row);
			}
			//We must sort the results after they are returned from the database, because the database software (i.e. MySQL)
			//cannot return sorted results from two or more result sets like we have here.
			resultSet.Sort(delegate(Object o1,Object o2) {
				DataRow r1=(DataRow)o1;
				DataRow r2=(DataRow)o2;
				int idxCategory1=Convert.ToInt32(r1["idxCategory"].ToString());
				int idxCategory2=Convert.ToInt32(r2["idxCategory"].ToString());
				if(idxCategory1<idxCategory2){
					return -1;
				}
				else if(idxCategory1>idxCategory2){
					return 1;
				}
				return PIn.Date(r1["DateCreated"].ToString()).CompareTo(PIn.Date(r2["DateCreated"].ToString()));
			});
			//Finally, move the results from the list into a data table.
			for(int i=0;i<resultSet.Count;i++){
				table.Rows.Add((DataRow)resultSet[i]);
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
			DataRow row;
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
			tableReturn.Columns.Add("Note");
			tableReturn.Columns.Add("DateCreated");
			tableReturn.Columns.Add("docCategory");
			tableReturn.Columns.Add("DocCategory");
			tableReturn.Columns.Add("DateTStamp");
			tableReturn.Columns.Add("serverDateTime");
			command="SELECT "+DbHelper.Now();
			DateTime dateTimeServer=PIn.DateT(Db.GetScalar(command)); //run first for rigorous inclusion of documents
			command="SELECT DocNum,FileName,Description,Note,DateCreated,DocCategory,DateTStamp FROM document WHERE PatNum='"+POut.Long(patNum)+"' AND MountItemNum=0"; //select all documents not associated with mounts
			tableDocuments=Db.GetTable(command);
			command="SELECT MountNum,Description,Note,DocCategory,DateCreated FROM mount WHERE PatNum='"+POut.Long(patNum)+"'"; //select all mounts for patient
			tableMounts=Db.GetTable(command);
			List<Def> listDefsForCategory=Defs.GetDefsForCategory(DefCat.ImageCats,isShort:false);
			//Add documents to results list
			for(int i=0;i<tableDocuments.Rows.Count;i++) {
				row=tableReturn.NewRow();
				row["DocNum"]=PIn.Long(tableDocuments.Rows[i]["DocNum"].ToString());
				row["MountNum"]=0;
				row["filePath"]=filePath+tableDocuments.Rows[i]["FileName"].ToString();
				row["Description"]=PIn.Date(tableDocuments.Rows[i]["DateCreated"].ToString()).ToString("d")+": "+tableDocuments.Rows[i]["Description"];
				row["Note"]=tableDocuments.Rows[i]["Note"].ToString();
				row["DateCreated"]=PIn.Date(tableDocuments.Rows[i]["DateCreated"].ToString()).ToString(dateTimeFormatString);//because API is expecting this format
				long docCategoryDefNum=PIn.Long(tableDocuments.Rows[i]["DocCategory"].ToString());
				string defName=Defs.GetName(DefCat.ImageCats,docCategoryDefNum,listDefsForCategory);
				row["docCategory"]=defName;
				row["DocCategory"]=docCategoryDefNum;
				row["DateTStamp"]=PIn.Date(tableDocuments.Rows[i]["DateTStamp"].ToString()).ToString(dateTimeFormatString);
				row["serverDateTime"]=dateTimeServer.ToString(dateTimeFormatString);
				listDataRows.Add(row);
			}
			//Add mounts to results list
			for(int i=0;i<tableMounts.Rows.Count;i++) {
				row=tableReturn.NewRow();
				row["DocNum"]=0;
				row["MountNum"]=PIn.Long(tableMounts.Rows[i]["MountNum"].ToString());
				row["filePath"]=""; //not a field for Mounts
				row["Description"]=PIn.Date(tableMounts.Rows[i]["DateCreated"].ToString()).ToShortDateString()+": "+PIn.String(tableMounts.Rows[i]["Description"].ToString());
				row["Note"]=tableMounts.Rows[i]["Note"].ToString();
				row["DateCreated"]=PIn.Date(tableMounts.Rows[i]["DateCreated"].ToString()).ToString(dateTimeFormatString);
				long docCategoryDefNum=PIn.Long(tableMounts.Rows[i]["DocCategory"].ToString());
				string defName=Defs.GetName(DefCat.ImageCats,docCategoryDefNum,listDefsForCategory);
				row["docCategory"]=defName;
				row["DocCategory"]=docCategoryDefNum;
				row["DateTStamp"]=""; //not a field for Mounts
				row["serverDateTime"]=dateTimeServer.ToString(dateTimeFormatString);
				listDataRows.Add(row);
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
		public static bool IsAcceptableFileName(string file) {
			//No need to check MiddleTierRole; no call to db.
			string[] specificBadFileNames=new string[] {
				"thumbs.db"
			};
			for(int i=0;i<specificBadFileNames.Length;i++) {
				if(file.Length>=specificBadFileNames[i].Length && 
					file.Substring(file.Length-specificBadFileNames[i].Length,
					specificBadFileNames[i].Length).ToLower()==specificBadFileNames[i]) {
					return false;
				}
			}
			if(file.StartsWith(".")) {//Extension-only file, ignore.
				return false;
			}
			return true;
		}

		///<summary>When first opening the image module, this tests to see whether a given filename is in the database. Also used when naming a new document to ensure unique name.</summary>
		public static bool IsFileNameInList(string fileName,string[] usedNames) {
			//No need to check MiddleTierRole; no call to db.
			for(int i=0;i<usedNames.Length;i++) {
				if(usedNames[i]==fileName)
					return true;
			}
			return false;
		}

		//public static string GetFull

		///<summary>Only documents listed in the corresponding rows of the statement table are uploaded</summary>
		public static List<long> GetChangedSinceDocumentNums(DateTime changedSince,List<long> statementNumList) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),changedSince,statementNumList);
			}
			string strStatementNums="";
			DataTable table;
			if(statementNumList.Count>0) {
				for(int i=0;i<statementNumList.Count;i++) {
					if(i>0) {
						strStatementNums+="OR ";
					}
					strStatementNums+="StatementNum='"+statementNumList[i].ToString()+"' ";
				}
				string command="SELECT DocNum FROM document WHERE  DateTStamp > "+POut.DateT(changedSince)+" AND DocNum IN ( SELECT DocNum FROM statement WHERE "+strStatementNums+")";
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			List<long> documentnums = new List<long>(table.Rows.Count);
			for(int i=0;i<table.Rows.Count;i++) {
				documentnums.Add(PIn.Long(table.Rows[i]["DocNum"].ToString()));
			}
			return documentnums;
		}

		///<summary>Used along with GetChangedSinceDocumentNums</summary>
		public static List<Document> GetMultDocuments(List<long> documentNums,string AtoZpath) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Document>>(MethodBase.GetCurrentMethod(),documentNums,AtoZpath);
			}
			string strDocumentNums="";
			DataTable table;
			if(documentNums.Count>0) {
				for(int i=0;i<documentNums.Count;i++) {
					if(i>0) {
						strDocumentNums+="OR ";
					}
					strDocumentNums+="DocNum='"+documentNums[i].ToString()+"' ";
				}
				string command="SELECT * FROM document WHERE "+strDocumentNums;
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			Document[] multDocuments=Crud.DocumentCrud.TableToList(table).ToArray();
			List<Document> documentList=new List<Document>(multDocuments);
			foreach(Document d in documentList) {
				if(string.IsNullOrEmpty(d.RawBase64)) {
					Patient pat=Patients.GetPat(d.PatNum);
					string filePathAndName=ImageStore.GetFilePath(Documents.GetByNum(d.DocNum),AtoZpath);
					if(File.Exists(filePathAndName)) {
						FileStream fs= new FileStream(filePathAndName,FileMode.Open,FileAccess.Read);
						byte[] rawData = new byte[fs.Length];
						fs.Read(rawData,0,(int)fs.Length);
						fs.Close();
						d.RawBase64=Convert.ToBase64String(rawData);
					}
				}
			}
			return documentList;
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
		public static void MergePatientDocument(long patFrom,long patTo,string oldFileName,string newFileName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patFrom,patTo,oldFileName,newFileName);
				return;
			}
			string command="UPDATE document"
				+" SET PatNum="+POut.Long(patTo)+","
				+" FileName='"+POut.String(newFileName)+"'"
				+" WHERE PatNum="+POut.Long(patFrom)
				+" AND FileName='"+POut.String(oldFileName)+"'";
			Db.NonQ(command);
		}

		///<summary>Moves all documents from one patient to another.
		///Only call when physically storing images in a folder share and only if every document.Filename matches a file in patTo's folder.</summary>
		public static void MergePatientDocuments(long patFrom,long patTo) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patFrom,patTo);
				return;
			}
			string command="UPDATE document"
				+" SET PatNum="+POut.Long(patTo)
				+" WHERE PatNum="+POut.Long(patFrom);
			Db.NonQ(command);
		}

		///<summary>Attempts to open the document using the default program. If not using AtoZfolder saves a local temp file and opens it. Returns empty string on success, otherwise returns error message.</summary>
		public static string OpenDoc(long docNum) {
			Document docCur=Documents.GetByNum(docNum);
			if(docCur.DocNum==0) {
				return "";
			}
			Patient patCur=Patients.GetPat(docCur.PatNum);
			if(patCur==null) {
				return "";
			}
			string docPath;
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				docPath=ImageStore.GetFilePath(docCur,ImageStore.GetPatientFolder(patCur,ImageStore.GetPreferredAtoZpath()));
			}
			else if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				//Some programs require a file on disk and cannot open in memory files. Save to temp file from DB.
				docPath=PrefC.GetRandomTempFile(ImageStore.GetExtension(docCur));
				File.WriteAllBytes(docPath,Convert.FromBase64String(docCur.RawBase64));
			}
			else {//Cloud storage
				//Download file to temp directory
				OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.Download(ImageStore.GetPatientFolder(patCur,ImageStore.GetPreferredAtoZpath())
					,docCur.FileName);
				docPath=PrefC.GetRandomTempFile(ImageStore.GetExtension(docCur));
				File.WriteAllBytes(docPath,state.FileContent);
				if(ODBuild.IsWeb()) {
					ThinfinityUtils.HandleFile(docPath);
					return "";
				}
			}
			try { 
				Process.Start(docPath);
			}
			catch(Exception ex) {
				return Lans.g("Documents","Error occurred while attempting to open document.\r\n"+
					"Verify a default application has been selected to open files of type: ")+
					Path.GetExtension(docCur.FileName)+"\r\n"+ex.Message;
			}
			return "";
		}

		//Checks to see if the document exists in the correct location, or checks DB for stored content.
		public static bool DocExists(long docNum) {
			Document docCur=Documents.GetByNum(docNum);
			if(docCur.DocNum==0) {
				return false;
			}
			Patient patCur=Patients.GetPat(docCur.PatNum);
			if(patCur==null) {
				return false;
			}
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				return File.Exists(ImageStore.GetFilePath(docCur,ImageStore.GetPatientFolder(patCur,ImageStore.GetPreferredAtoZpath())));
			}
			else if(CloudStorage.IsCloudStorage) {
				return CloudStorage.FileExists(ODFileUtils.CombinePaths(ImageStore.GetPatientFolder(patCur,ImageStore.GetPreferredAtoZpath()),docCur.FileName,'/'));
			}
			return !string.IsNullOrEmpty(docCur.RawBase64);
		}

		///<summary>Returns true if a Document with the external GUID is found in the database.</summary>
		public static bool DocExternalExists(string externalGUID,ExternalSourceType externalSource) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),externalGUID,externalSource);
			}
			string command="SELECT COUNT(*) FROM document"
				+" WHERE document.ExternalGUID='"+POut.String(externalGUID)+"'"
				+" AND document.ExternalSource='"+POut.String(externalSource.ToString())+"'";
			if(Db.GetCount(command)!="0") {
				return true;
			}
			return false;
		}

		///<summary>Returns the filepath of the document if using AtoZfolder. If storing files in DB or third party storage, saves document to local temp file and returns filepath.
		/// Empty string if not found.</summary>
		public static string GetPath(long docNum) {
			Document docCur=Documents.GetByNum(docNum);
			if(docCur.DocNum==0) {
				return "";
			}
			Patient patCur=Patients.GetPat(docCur.PatNum);
			if(patCur==null) {
				return "";
			}
			string docPath;
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				docPath=ImageStore.GetFilePath(docCur,ImageStore.GetPatientFolder(patCur,ImageStore.GetPreferredAtoZpath()));
			}
			else if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				//Some programs require a file on disk and cannot open in memory files. Save to temp file from DB.
				docPath=PrefC.GetRandomTempFile(ImageStore.GetExtension(docCur));
				File.WriteAllBytes(docPath,Convert.FromBase64String(docCur.RawBase64));
			}
			else {//Cloud storage
				//Download file to temp directory
				OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.Download(ImageStore.GetPatientFolder(patCur,ImageStore.GetPreferredAtoZpath())
					,docCur.FileName);
				docPath=PrefC.GetRandomTempFile(ImageStore.GetExtension(docCur));
				File.WriteAllBytes(docPath,state.FileContent);
			}
			return docPath;
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
			try {
				SheetDrawingJob sheetDrawingJob=new SheetDrawingJob();
				Sheet sheetPayPlan=PayPlans.PayPlanToSheet(paymentPlan);
				SheetParameter.SetParameter(sheetPayPlan, "keyData", PayPlans.GetKeyDataForSignature(paymentPlan));
				SheetUtil.CalculateHeights(sheetPayPlan);
				filePath=PrefC.GetRandomTempFile(".pdf");
				PdfDocument pdf=sheetDrawingJob.CreatePdf(sheetPayPlan);
				SheetDrawingJob.SavePdfToFile(pdf,filePath);
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
			try {
				Patient PatCur=Patients.GetPat(paymentPlan.PatNum);
				//Determine each of the document categories that this PP should be saved to.
				//"A"==PaymentPlan; See DefCat.ImageCats 
				List<Def> listImageCatDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
				List<long> categories=listImageCatDefs.Where(x => x.ItemValue.Contains("A")).Select(x=>x.DefNum).ToList();
				if(categories.Count==0) {
					//we must save at least one document, pick first non-hidden image category.
					Def imgCat=listImageCatDefs.FirstOrDefault(x => !x.IsHidden);
					if(imgCat==null) {
						Logger.WriteLine("There are currently no image categories","PaymentPlans");
						return false;
					}
					categories.Add(imgCat.DefNum);
				}
				if(!TryCreatePayPlanPdfFile(paymentPlan,out string tempFile)){
					return false;
				}
				string rawBase64="";
				if(PrefC.AtoZfolderUsed!=DataStorageType.LocalAtoZ) {
					//Convert the pdf into its raw bytes
					rawBase64=Convert.ToBase64String(System.IO.File.ReadAllBytes(tempFile));
				}
				foreach(long docCategory in categories) {//usually only one, but do allow them to be saved once per image category.
					Document docSave=new Document();
					docSave.DocNum=Insert(docSave);
					string fileName="PayPlanArchive"+docSave.DocNum;
					docSave.ImgType=ImageType.Document;
					docSave.DateCreated=DateTime.Now;
					docSave.PatNum=paymentPlan.PatNum;
					docSave.DocCategory=docCategory;
					docSave.Description=fileName;//no extension.
					docSave.RawBase64=rawBase64;//blank if using AtoZfolder
					if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
						string filePath=ImageStore.GetPatientFolder(PatCur,ImageStore.GetPreferredAtoZpath());
						while(File.Exists(filePath+"\\"+fileName+".pdf")) {
							fileName+="x";
						}
						File.Copy(tempFile,filePath+"\\"+fileName+".pdf");
					}
					else if(CloudStorage.IsCloudStorage) {
						//Upload file to patient's AtoZ folder
						OpenDentalCloud.Core.TaskStateUpload state=CloudStorage.Upload(ImageStore.GetPatientFolder(PatCur,"")
							,fileName+".pdf"
							,File.ReadAllBytes(tempFile));
					}
					docSave.FileName=fileName+".pdf";//file extension used for both DB images and AtoZ images
					Update(docSave);
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
			try { 
				SheetDrawingJob sheetDrawingJob=new SheetDrawingJob();
				Sheet sheetTP=TreatPlans.CreateSheetFromTreatmentPlan(treatPlan);
				filePath=PrefC.GetRandomTempFile(".pdf");
				//Create a PDF with the given sheet and file. The other parameters can remain null, because they aren't used for TreatPlan sheets.
				PdfDocument pdf=sheetDrawingJob.CreatePdf(sheetTP);
				SheetDrawingJob.SavePdfToFile(pdf,filePath);
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
			string errorMessage="";
			try {
				Patient PatCur=Patients.GetPat(treatPlan.PatNum);
				List<Def> listImageCatDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
				List<long> categories=listImageCatDefs.Where(x => x.ItemValue.Contains("R")).Select(x=>x.DefNum).ToList();
				if(categories.Count==0) {
					//we must save at least one document, pick first non-hidden image category.
					Def imgCat=listImageCatDefs.FirstOrDefault(x => !x.IsHidden);
					if(imgCat==null) {
						errorMessage="There are currently no image categories.";
						Logger.WriteLine(errorMessage,"TreatmentPlans");
						return errorMessage;
					}
					categories.Add(imgCat.DefNum);
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
				foreach(long docCategory in categories) {//usually only one, but do allow them to be saved once per image category.
					Document docSave=new Document();
					docSave.DocNum=Insert(docSave);
					string fileName="TPArchive"+docSave.DocNum;
					docSave.ImgType=ImageType.Document;
					docSave.DateCreated=DateTime.Now;
					docSave.PatNum=treatPlan.PatNum;
					docSave.DocCategory=docCategory;
					docSave.Description=fileName;//no extension.
					docSave.RawBase64=rawBase64;//blank if using AtoZfolder
					if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
						string filePath=ImageStore.GetPatientFolder(PatCur,ImageStore.GetPreferredAtoZpath());
						while(File.Exists(filePath+"\\"+fileName+".pdf")) {
							fileName+="x";
						}
						File.Copy(tempFile,filePath+"\\"+fileName+".pdf");
					}
					else if(CloudStorage.IsCloudStorage) {
						//Upload file to patient's AtoZ folder
						OpenDentalCloud.Core.TaskStateUpload state=CloudStorage.Upload(ImageStore.GetPatientFolder(PatCur,"")
							,fileName+".pdf"
							,File.ReadAllBytes(tempFile));
					}
					docSave.FileName=fileName+".pdf";//file extension used for both DB images and AtoZ images
					Update(docSave);
				}
			}
			catch(Exception ex) {
				Logger.WriteException(ex,"TreatmentPlans");
				return ex.Message;
			}
			return "";
		}

		public static bool IsVC2015Installed(){
			string dependenciesPath=@"SOFTWARE\Classes\Installer\Dependencies";
			using(RegistryKey dependencies=Registry.LocalMachine.OpenSubKey(dependenciesPath)){
				if(dependencies==null) {
					return false;
				}
				List<string> listSubKeyNames=dependencies.GetSubKeyNames().Where(n => !n.ToLower().Contains("dotnet") && !n.ToLower().Contains("microsoft")).ToList();
				for(int i=0;i<listSubKeyNames.Count;i++){ 
					using(RegistryKey subDir=Registry.LocalMachine.OpenSubKey(dependenciesPath + "\\" +listSubKeyNames[i])){
						string value=subDir.GetValue("DisplayName")?.ToString()??null;
						if(string.IsNullOrEmpty(value)) {
							continue;
						} 
						if(Regex.IsMatch(value,@"C\+\+ 2015.*\((x86|x64)\)")) { 
							return true;
						}
					}
				}
			}
			return false;
		}
		#endregion Xam Methods
	}

	public class DocumentForApi {
		public Document DocumentCur;
		public DateTime DateTimeServer;
	}
}