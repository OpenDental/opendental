using CodeBase;
using OpenDentBusiness.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenDentBusiness {
	/// <summary></summary>
	public class ImageStore {

		///<summary>Only makes a call to the database on startup.  After that, just uses cached data.  Does not validate that the path exists except if the main one is used.  ONLY used from Client layer or S class methods that have "No need to check RemotingRole; no call to db" and which also make sure PrefC.AtoZfolderUsed.  Returns Cloud AtoZ path if CloudStorage.IsCloudStorage</summary>
		public static string GetPreferredAtoZpath() {
			//There were so many references to the current function that we decided to temporarily call the FileAtoZ version here.
			return FileAtoZ.GetPreferredAtoZpath();
		}

		///<summary>Throw exceptions. Returns patient's AtoZ folder if local AtoZ used, blank if database is used, 
		///or Cloud AtoZ path if CloudStorage.IsCloudStorage. Will validate that folder exists. Will create folder if needed. 
		///It will set the pat.ImageFolder if pat.ImageFolder is blank.</summary>
		public static string GetPatientFolder(Patient pat,string AtoZpath) {
			string retVal="";
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				return retVal;
			}
			if(CloudStorage.IsCloudStorage) {
				AtoZpath=CloudStorage.AtoZPath;
			}
			Patient PatOld=pat.Copy();
			if(string.IsNullOrEmpty(pat.ImageFolder)) {//creates new folder for patient if none present
				pat.ImageFolder=GetImageFolderName(pat);
			}
			if(CloudStorage.IsCloudStorage) {
				retVal=ODFileUtils.CombinePaths(AtoZpath,
					pat.ImageFolder.Substring(0,1).ToUpper(),
					pat.ImageFolder,'/');//use '/' char instead of Path.DirectorySeparatorChar
			}
			else if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				retVal=ODFileUtils.CombinePaths(AtoZpath,
					pat.ImageFolder.Substring(0,1).ToUpper(),
					pat.ImageFolder);//use Path.DirectorySeparatorChar
				try {
					if(string.IsNullOrEmpty(AtoZpath)) {
						//If AtoZpath parameter was null or empty string and DataStorageType is LocalAtoZ, don't create a directory since retVal would then be
						//considered a relative path. Example: If AtoZpath is null, retVal will be like "P\PatientAustin1" after ODFileUtils.CombinePaths.
						//CreateDirectory treats this as a relative path and the full path would be "C:\Program Files (x86)\Open Dental\P\PatientAustin1".
						throw new ApplicationException("AtoZpath was null or empty");
					}
					if(!Directory.Exists(retVal)) {
						Directory.CreateDirectory(retVal);
					}
				}
				catch(Exception ex) {
					throw new ApplicationException(Lans.g("ContrDocs","Error.  Could not create folder for patient:")+" "+retVal,ex);
				}
			}
			if(string.IsNullOrEmpty(PatOld.ImageFolder)) {
				Patients.Update(pat,PatOld);
			}
			return retVal;
		}

		///<summary>Returns the name of the ImageFolder. Removes any non letter to the patient's name.</summary>
		public static string GetImageFolderName(Patient pat) {
			string name=pat.LName+pat.FName;
			string folder="";
			for(int i=0;i<name.Length;i++) {
				if(Char.IsLetter(name,i)) {
					folder+=name.Substring(i,1);
				}
			}
			folder+=pat.PatNum.ToString();//ensures unique name
			return folder;
		}

		///<summary>Will create folder if needed.  Will validate that folder exists.</summary>
		public static string GetEobFolder() {
			string retVal="";
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				return retVal;
			}
			string AtoZPath=GetPreferredAtoZpath();
			retVal=ODFileUtils.CombinePaths(AtoZPath,"EOBs");
			if(CloudStorage.IsCloudStorage) {
				retVal=retVal.Replace("\\","/");
			}
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && !Directory.Exists(retVal)) {
				if(string.IsNullOrEmpty(AtoZPath)) {
					throw new ApplicationException(Lans.g("ContrDocs","Could not find the path for the AtoZ folder."));
				}
				Directory.CreateDirectory(retVal);
			}
			return retVal;
		}

		///<summary>Will create folder if needed.  Will validate that folder exists.</summary>
		public static string GetAmdFolder() {
			string retVal="";
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				return retVal;
			}
			string AtoZPath=GetPreferredAtoZpath();
			retVal=ODFileUtils.CombinePaths(AtoZPath,"Amendments");
			if(CloudStorage.IsCloudStorage) {
				retVal=retVal.Replace("\\","/");
			}
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && !Directory.Exists(retVal)) {
				if(string.IsNullOrEmpty(AtoZPath)) {
					throw new ApplicationException(Lans.g("ContrDocs","Could not find the path for the AtoZ folder."));
				}
				Directory.CreateDirectory(retVal);
			}
			return retVal;
		}

		///<summary>Gets the folder name where provider images are stored. Will create folder if needed.</summary>
		public static string GetProviderImagesFolder() {
			string retVal="";
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				return retVal;
			}
			string AtoZPath=GetPreferredAtoZpath();
			retVal=FileAtoZ.CombinePaths(AtoZPath,"ProviderImages");
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && !Directory.Exists(retVal)) {
				if(string.IsNullOrEmpty(AtoZPath)) {
					throw new ApplicationException(Lans.g("ContrDocs","Could not find the path for the AtoZ folder."));
				}
				Directory.CreateDirectory(retVal);
			}
			return retVal;
		}
		
		///<summary>Surround with try/catch.  Typically returns something similar to \\SERVER\OpenDentImages\EmailImages.
		///This is the location of the email html template images.  The images are stored in this central location in order to
		///make them reusable on multiple email messages.  These images are not patient specific, therefore are in a different
		///location than the email attachments.  For location of patient attachments, see EmailAttaches.GetAttachPath().</summary>
		public static string GetEmailImagePath() {
			//No need to check RemotingRole; no call to db.
			string emailPath;
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				throw new ApplicationException(Lans.g("WikiPages","Must be using AtoZ folders."));
			}
			emailPath=FileAtoZ.CombinePaths(GetPreferredAtoZpath(),"EmailImages");
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && !Directory.Exists(emailPath)) {
				Directory.CreateDirectory(emailPath);
			}
			return emailPath;
		} 

		///<summary>When the Image module is opened, this loads newly added files.</summary>
		public static void AddMissingFilesToDatabase(Patient pat) {
			//There is no such thing as adding files from any directory when not using AtoZ
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				return;
			}
			string patFolder=GetPatientFolder(pat,GetPreferredAtoZpath());
			List<string> fileList=new List<string>();
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				DirectoryInfo di = new DirectoryInfo(patFolder);
				List<FileInfo> fiList = di.GetFiles().Where(x => !x.Attributes.HasFlag(FileAttributes.Hidden)).ToList();
				fileList.AddRange(fiList.Select(x => x.FullName));
			}
			else {//Cloud
				OpenDentalCloud.Core.TaskStateListFolders state=CloudStorage.ListFolderContents(patFolder);
				List<string> listFiles=state.ListFolderPathsDisplay;
				List<Document> listDocs=Documents.GetAllWithPat(pat.PatNum).ToList();
				listFiles=listFiles.Select(x => Path.GetFileName(x)).ToList();
				foreach(string fileName in listFiles) {
					if(!listDocs.Exists(x => x.FileName==fileName)) {
						fileList.Add(fileName);
					}
				}
			}
			int countAdded=Documents.InsertMissing(pat,fileList);//Automatically detects and inserts files that are in the patient's folder that aren't present in the database. Logs entries.
			//should notify user
			//if(countAdded > 0) {
			//	Debug.WriteLine(countAdded.ToString() + " documents found and added to the first category.");
			//}
			//it will refresh in FillDocList
		}

		public static string GetHashString(Document doc,string patFolder) {
			//the key data is the bytes of the file, concatenated with the bytes of the note.
			byte[] textbytes;
			byte[] filebytes=new byte[1];
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase){
				patFolder=ODFileUtils.CombinePaths(Path.GetTempPath(),"opendental");
				byte[] rawData=Convert.FromBase64String(doc.RawBase64);
				using(FileStream file=new FileStream(ODFileUtils.CombinePaths(patFolder,doc.FileName),FileMode.Create,FileAccess.Write)) {
					file.Write(rawData,0,rawData.Length);
					file.Close();
				}
			}
			else if(CloudStorage.IsCloudStorage) {
				OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.Download(patFolder.Replace("\\","/")
					,doc.FileName);
				filebytes=state.FileContent;
			}
			if(doc.Note == null) {
				textbytes = Encoding.UTF8.GetBytes("");
			}
			else {
				textbytes = Encoding.UTF8.GetBytes(doc.Note);
			}
			if(CloudStorage.IsCloudStorage) {
				filebytes=GetBytes(doc,patFolder);
			}
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				try {
					File.Delete(ODFileUtils.CombinePaths(patFolder,doc.FileName));//Delete temp file
				}
				catch { }//Should never happen since the file was just created and the permissions were there moments ago when the file was created.
			}
			int fileLength = filebytes.Length;
			byte[] buffer = new byte[textbytes.Length + filebytes.Length];
			Array.Copy(filebytes,0,buffer,0,fileLength);
			Array.Copy(textbytes,0,buffer,fileLength,textbytes.Length);
			return Encoding.ASCII.GetString(ODCrypt.MD5.Hash(buffer));
		}

		///<summary>Can be null. Analogous to OpenImage.</summary>
		public static BitmapDicom OpenBitmapDicom(Document doc,string patFolder,string localPath=""){
			if(!doc.FileName.EndsWith(".dcm")){
				return null;
			}
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				string srcFileName = ODFileUtils.CombinePaths(patFolder,doc.FileName);
				return DicomHelper.GetFromFile(srcFileName);
			}
			else if(CloudStorage.IsCloudStorage) {
				if(localPath!="") {
					return DicomHelper.GetFromFile(localPath);
				}
				BitmapDicom bitmapDicom=null;
				try {
					OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.Download(patFolder.Replace("\\","/"),doc.FileName,true);
					using(MemoryStream ms=new MemoryStream(state.FileContent)) {
						bitmapDicom=DicomHelper.GetFromStream(ms);//Warning! Untested!
					}
				}
				catch(Exception e) {
					e.DoNothing();
				}
				return bitmapDicom;
			}
			else {//db
				return DicomHelper.GetFromBase64(doc.RawBase64);//Warning! Untested!
			}
		}

		public static Collection<Bitmap> OpenImages(IList<Document> documents,string patFolder,string localPath="") {
			//string patFolder=GetPatientFolder(pat);
			Collection<Bitmap> bitmaps = new Collection<Bitmap>();
			foreach(Document document in documents) {
				if(document == null) {
					bitmaps.Add(null);
				}
				else {
					bitmaps.Add(OpenImage(document,patFolder,localPath));
				}
			}
			return bitmaps;
		}

		///<summary>Individual bitmaps can be null.</summary>
		public static Bitmap[] OpenImages(Document[] documents,string patFolder,string localPath="") {
			//Bitmap[] arrayBitmaps = new Bitmap[documents.Length];
			Collection<Bitmap> collectionBitmaps = OpenImages(new Collection<Document>(documents),patFolder,localPath);
			//collectionBitmaps.CopyTo(arrayBitmaps,0);
			return collectionBitmaps.ToArray();
			//return arrayBitmaps;
		}

		///<summary>Can be null.</summary>
		public static Bitmap OpenImage(Document doc,string patFolder,string localPath="") {
			//todo: use a stream so that the returned bitmap does not have a file lock.
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				string srcFileName = ODFileUtils.CombinePaths(patFolder,doc.FileName);
				if(HasImageExtension(srcFileName)) {
					//if(File.Exists(srcFileName) && HasImageExtension(srcFileName)) {
					try {
						return new Bitmap(srcFileName);
					}
					catch {
						return null;
					}
				}
				else {
					return null;
				}
			}
			else if(CloudStorage.IsCloudStorage) {
				if(HasImageExtension(doc.FileName)) {
					Bitmap bmp=null;
					if(localPath!="") {
						bmp=new Bitmap(localPath);
					}
					else {
						try {
							OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.Download(patFolder.Replace("\\","/")
								,doc.FileName,true);
							using(MemoryStream ms=new MemoryStream(state.FileContent)) {
								bmp=new Bitmap(ms);
							}
						}
						catch(Exception e) {
							e.DoNothing();
						}
					}
					return bmp;
				}
				else {
					return null;
				}
			}
			else {
				if(HasImageExtension(doc.FileName)) {
					return PIn.Bitmap(doc.RawBase64);
				}
				else {
					return null;
				}
			}
		}

		public static Bitmap[] OpenImagesEob(EobAttach eob,string localPath="") {
			Bitmap[] values = new Bitmap[1];
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				string eobFolder=GetEobFolder();
				string srcFileName = ODFileUtils.CombinePaths(eobFolder,eob.FileName);
				if(HasImageExtension(srcFileName)) {
					if(File.Exists(srcFileName)) {
						try {
							values[0]=new Bitmap(srcFileName);
						}
						catch(Exception ex) {
							throw new ApplicationException(Lans.g("ImageStore","File found but could not be opened:")+" "+srcFileName,ex);
						}
					}
					else {
						throw new ApplicationException(Lans.g("ImageStore","File not found:")+" "+srcFileName);
					}
				}
				else {
					values[0]= null;
				}
			}
			else if(CloudStorage.IsCloudStorage) {
				if(HasImageExtension(eob.FileName)) {
					Bitmap bmp=null;
					try {
						if(localPath!="") {
							bmp=new Bitmap(localPath);
						}
						else {
							OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.Download(GetEobFolder()
							,eob.FileName);
							using(MemoryStream ms = new MemoryStream(state.FileContent)) {
								bmp=new Bitmap(ms);
							}
						}
					}
					catch(Exception ex) {
						throw new ApplicationException(Lans.g("ImageStore","File could not be opened:")+" "+eob.FileName,ex);
					}
					values[0]=bmp;
				}
				else {
					values[0]=null;
				}
			}
			else {
				if(HasImageExtension(eob.FileName)) {
					values[0]= PIn.Bitmap(eob.RawBase64);
				}
				else {
					values[0]= null;
				}
			}
			return values;
		}

		public static Bitmap[] OpenImagesAmd(EhrAmendment amd) {
			Bitmap[] values = new Bitmap[1];
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				string amdFolder=GetAmdFolder();
				string srcFileName = ODFileUtils.CombinePaths(amdFolder,amd.FileName);
				if(HasImageExtension(srcFileName)) {
					if(File.Exists(srcFileName)) {
						try {
							values[0]=new Bitmap(srcFileName);
						}
						catch(Exception ex) {
							throw new ApplicationException(Lans.g("ImageStore","File found but could not be opened:")+" "+srcFileName,ex);
						}
					}
					else {
						throw new ApplicationException(Lans.g("ImageStore","File not found:")+" "+srcFileName);
					}
				}
				else {
					values[0]= null;
				}
			}
			else if(CloudStorage.IsCloudStorage) {
				if(HasImageExtension(amd.FileName)) {
					try {
						OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.Download(GetAmdFolder()
						,amd.FileName);
						Bitmap bmp=null;
						using(MemoryStream ms = new MemoryStream(state.FileContent)) {
							bmp=new Bitmap(ms);
						}
						values[0]=bmp;
					}
					catch(Exception ex) {
						throw new ApplicationException(Lans.g("ImageStore","File could not be opened:")+" "+amd.FileName,ex);
					}
				}
				else {
					values[0]=null;
				}
			}
			else {
				if(HasImageExtension(amd.FileName)) {
					values[0]= PIn.Bitmap(amd.RawBase64);
				}
				else {
					values[0]= null;
				}
			}
			return values;
		}

		///<summary>Takes in a mount object and finds all the images pertaining to the mount, then combines them together into one image of the specified size. For use in other modules.</summary>
		public static void GetMountImage(Mount mount,string patFolder,Size size) {
			//string patFolder=GetPatientFolder(pat);
			//List<MountItem> mountItems = MountItems.GetItemsForMount(mount.MountNum);
			//Document[] documents = Documents.GetDocumentsForMountItems(mountItems);
			//Bitmap[] originalImages = OpenImages(documents,patFolder);
			//Bitmap mountImage = new Bitmap(mount.Width,mount.Height);
			ImageHelper.RenderMountImage(null,null,null,null,-1);//Placeholder
			//return mountImage;
		}

		public static byte[] GetBytes(Document doc,string patFolder) {
			/*if(ImageStoreIsDatabase) {not supported
				byte[] buffer;
				using(IDbConnection connection = DataSettings.GetConnection())
				using(IDbCommand command = connection.CreateCommand()) {
					command.CommandText =	@"SELECT Data FROM files WHERE DocNum = ?DocNum";
					IDataParameter docNumParameter = command.CreateParameter();
					docNumParameter.ParameterName = "?DocNum";
					docNumParameter.Value = doc.DocNum;
					command.Parameters.Add(docNumParameter);
					connection.Open();
					buffer = (byte[])command.ExecuteScalar();
					connection.Close();
				}
				return buffer;
			}
			else {*/
			string path = ODFileUtils.CombinePaths(patFolder,doc.FileName);
			if(!File.Exists(path)) {
				return new byte[] { };
			}
			byte[] buffer;
			using(FileStream fs = new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read)) {
				int fileLength = (int)fs.Length;
				buffer = new byte[fileLength];
				fs.Read(buffer,0,fileLength);
			}
			return buffer;
		}

		/// <summary>Imports any document, not just images.  Also handles dicom by calculating initial windowing.  Also processes Exif rotation info on jpg files.</summary>
		public static Document Import(string pathImportFrom,long docCategory,Patient pat) {
			string patFolder="";
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ || CloudStorage.IsCloudStorage)  {
				patFolder=GetPatientFolder(pat,GetPreferredAtoZpath());
			}
			Document doc = new Document();
			//Document.Insert will use this extension when naming:
			if(Path.GetExtension(pathImportFrom)=="") {//If the file has no extension
				try {
					Bitmap bmp=new Bitmap(pathImportFrom);//check to see if file is an image and add .jpg extension
					bmp.Dispose();//release file lock
					doc.FileName=".jpg";
				}
				catch(Exception ex) {
					ex.DoNothing(); //catch the error and do nothing. Default the file to .txt to prevent errors.
					doc.FileName=".txt";
				}
			}
			else {
				doc.FileName = Path.GetExtension(pathImportFrom);
			}
			doc.DateCreated=File.GetLastWriteTime(pathImportFrom); // Per Jordan, use lastwritetime instead of DateTime.Now/Today.
			doc.PatNum = pat.PatNum;
			if(HasImageExtension(doc.FileName)) {
				doc.ImgType=ImageType.Photo;
				if(pathImportFrom.ToLower().EndsWith("jpg") || pathImportFrom.ToLower().EndsWith("jpeg")){
					Image image=Image.FromFile(pathImportFrom);
					PropertyItem propertyItem=image.PropertyItems.FirstOrDefault(x=>x.Id==0x0112);//Exif orientation
					if(propertyItem!=null && propertyItem.Value.Length>0){
						//if(propertyItem.Value[0]==1)//no rotation. Do nothing
						if(propertyItem.Value[0]==6){
							doc.DegreesRotated=90;
						}
						else if(propertyItem.Value[0]==3){
							doc.DegreesRotated=180;
						}
						else if(propertyItem.Value[0]==8){
							doc.DegreesRotated=270;
						}
					}
					image.Dispose();//releases file lock
				}
			}
			else if(doc.FileName.EndsWith(".dcm")){
				doc.ImgType=ImageType.Radiograph;
				BitmapDicom bitmapDicom=DicomHelper.GetFromFile(pathImportFrom);
				DicomHelper.CalculateWindowingOnImport(bitmapDicom);
				doc.WindowingMin=bitmapDicom.WindowingMin;
				doc.WindowingMax=bitmapDicom.WindowingMax;
			}
			else {
				doc.ImgType=ImageType.Document;
			}
			doc.DocCategory = docCategory;
			doc=Documents.InsertAndGet(doc,pat);//this assigns a filename and saves to db
			try {
				SaveDocument(doc,pathImportFrom,patFolder);//Makes log entry
				if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
					Documents.Update(doc);//Because SaveDocument() modified doc.RawBase64
				}
			}
			catch (Exception ex){
				Documents.Delete(doc);
				throw ex;
			}
			return doc;
		}

		/// <summary>Saves to AtoZ folder, Cloud, or to db.  Saves image as a jpg.  Compression will differ depending on imageType.</summary>
		public static Document Import(Bitmap image,long docCategory,ImageType imageType,Patient pat,string mimeType="image/jpeg") {
			string patFolder="";
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ || CloudStorage.IsCloudStorage) {
				patFolder=GetPatientFolder(pat,GetPreferredAtoZpath());
			}
			Document doc = new Document();
			doc.ImgType = imageType;
			doc.FileName=GetImageFileExtensionByMimeType(mimeType);
			doc.DateCreated = DateTime.Now;
			doc.PatNum = pat.PatNum;
			doc.DocCategory = docCategory;
			Documents.Insert(doc,pat);//creates filename and saves to db
			doc=Documents.GetByNum(doc.DocNum);
			long qualityL = 0;
			if(ListTools.In(imageType,ImageType.Radiograph,ImageType.Photo,ImageType.Attachment)) {
				qualityL=100;
			}
			else {//Assume document
						//Possible values 0-100?
				qualityL=(long)ComputerPrefs.LocalComputer.ScanDocQuality;
			}
			ImageCodecInfo imageCodecInfo;
			ImageCodecInfo[] encoders;
			encoders = ImageCodecInfo.GetImageEncoders();
			imageCodecInfo = null;
			for(int j = 0;j < encoders.Length;j++) {
				if(encoders[j].MimeType==mimeType) {
					imageCodecInfo = encoders[j];
				}
			}
			EncoderParameters encoderParameters = new EncoderParameters(1);
			EncoderParameter encoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality,qualityL);
			encoderParameters.Param[0] = encoderParameter;
			//AutoCrop()?
			try {
				SaveDocument(doc,image,imageCodecInfo,encoderParameters,patFolder);//Makes log entry
				if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
					Documents.Update(doc);//because SaveDocument stuck the image in doc.RawBase64.
					//no thumbnail yet
				}
			}
			catch(Exception e) {
				e.DoNothing();
				Documents.Delete(doc);
				throw;
			}
			return doc;
		}

		/// <summary>Saves to AtoZ folder, Cloud, or to db.  Saves document based off of the mimeType passed in.</summary>
		public static Document Import(byte[] arrayBytes,long docCategory,ImageType imageType,Patient pat,string mimeType="image/jpeg") {
			string patFolder="";
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ || CloudStorage.IsCloudStorage) {
				patFolder=GetPatientFolder(pat,GetPreferredAtoZpath());
			}
			Document doc=new Document();
			doc.ImgType=imageType;
			doc.FileName=GetImageFileExtensionByMimeType(mimeType);
			doc.DateCreated=DateTime.Now;
			doc.PatNum=pat.PatNum;
			doc.DocCategory=docCategory;
			Documents.Insert(doc,pat);//creates filename and saves to db
			doc=Documents.GetByNum(doc.DocNum);
			try {
				SaveDocument(doc,arrayBytes,patFolder);//Makes log entry
				if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
					Documents.Update(doc);//because SaveDocument stuck the image in doc.RawBase64.
					//no thumbnail yet
				}
			}
			catch(Exception e) {
				e.DoNothing();
				Documents.Delete(doc);
				throw;
			}
			return doc;
		}

		///<summary>Returns the file extension for the passed in mime type.</summary>
		private static string GetImageFileExtensionByMimeType(string mimeType) {
			switch(mimeType) {
				case "image/jpeg":
					return ".jpg";
				case "image/png":
					return ".png";
				case "image/tiff":
					return ".tif";
				default:
					throw new NotSupportedException();
			}
		}

		/// <summary>Obviously no support for db storage</summary>
		public static Document ImportForm(string form,long docCategory,Patient pat) {
			string patFolder=GetPatientFolder(pat,GetPreferredAtoZpath());
			string pathSourceFile=CloudStorage.PathTidy(ODFileUtils.CombinePaths(GetPreferredAtoZpath(),"Forms",form));
			if(!FileAtoZ.Exists(pathSourceFile)) {
				throw new Exception(Lans.g("ContrDocs","Could not find file: ") + pathSourceFile);
			}
			Document doc = new Document();
			doc.FileName = Path.GetExtension(pathSourceFile);
			doc.DateCreated = DateTime.Now;
			doc.DocCategory = docCategory;
			doc.PatNum = pat.PatNum;
			doc.ImgType = ImageType.Document;
			Documents.Insert(doc,pat);//this assigns a filename and saves to db
			doc=Documents.GetByNum(doc.DocNum);
			try {
				if(CloudStorage.IsCloudStorage) {
					//byte[] bytes=OpenDentBusiness.FileIO.FileAtoZ.ReadAllBytes(pathSourceFile);
					//OpenDentBusiness.FileIO.FileAtoZ.WriteAllBytes(CloudStorage.PathTidy(ODFileUtils.CombinePaths(patFolder,doc.FileName)),bytes);
					CloudStorage.Copy(pathSourceFile,CloudStorage.PathTidy(ODFileUtils.CombinePaths(patFolder,doc.FileName)));
					ImageStore.LogDocument(Lans.g("ContrImages","Document Created")+": ",Permissions.ImageEdit,doc,DateTime.MinValue); //new doc, min date.
				}
				else {
					SaveDocument(doc,pathSourceFile,patFolder);//Makes log entry
				}
			}
			catch {
				Documents.Delete(doc);
				throw;
			}
			return doc;
		}

		/// <summary>Always saves as bmp.  So the 'paste to mount' logic needs to be changed to prevent conversion to bmp.</summary>
		public static Document ImportImageToMount(Bitmap image,short rotationAngle,long mountItemNum,long docCategory,Patient pat) {
			string patFolder=GetPatientFolder(pat,GetPreferredAtoZpath());
			string fileExtention = ".bmp";//The file extention to save the greyscale image as.
			Document doc = new Document();
			doc.MountItemNum = mountItemNum;
			doc.DegreesRotated = rotationAngle;
			doc.ImgType = ImageType.Radiograph;
			doc.FileName = fileExtention;
			doc.DateCreated = DateTime.Now;
			doc.PatNum = pat.PatNum;
			doc.DocCategory = docCategory;
			doc.WindowingMin = PrefC.GetInt(PrefName.ImageWindowingMin);
			doc.WindowingMax = PrefC.GetInt(PrefName.ImageWindowingMax);
			Documents.Insert(doc,pat);//creates filename and saves to db
			doc=Documents.GetByNum(doc.DocNum);
			try {
				SaveDocument(doc,image,ImageFormat.Bmp,patFolder);//Makes log entry
			}
			catch {
				Documents.Delete(doc);
				throw;
			}
			return doc;
		}

		/// <summary>Saves to either AtoZ folder or to db.  Saves image as a jpg.  Compression will be according to user setting.</summary>
		public static EobAttach ImportEobAttach(Bitmap image,long claimPaymentNum) {
			string eobFolder="";
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ || CloudStorage.IsCloudStorage) {
				eobFolder=GetEobFolder();
			}
			EobAttach eob=new EobAttach();
			eob.FileName=".jpg";
			eob.DateTCreated = DateTime.Now;
			eob.ClaimPaymentNum=claimPaymentNum;
			EobAttaches.Insert(eob);//creates filename and saves to db
			eob=EobAttaches.GetOne(eob.EobAttachNum);
			long qualityL=(long)ComputerPrefs.LocalComputer.ScanDocQuality;
			ImageCodecInfo myImageCodecInfo;
			ImageCodecInfo[] encoders;
			encoders = ImageCodecInfo.GetImageEncoders();
			myImageCodecInfo = null;
			for(int j = 0;j < encoders.Length;j++) {
				if(encoders[j].MimeType == "image/jpeg") {
					myImageCodecInfo = encoders[j];
				}
			}
			EncoderParameters myEncoderParameters = new EncoderParameters(1);
			EncoderParameter myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality,qualityL);
			myEncoderParameters.Param[0] = myEncoderParameter;
			try {
				SaveEobAttach(eob,image,myImageCodecInfo,myEncoderParameters,eobFolder);
				if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
					EobAttaches.Update(eob);//because SaveEobAttach stuck the image in EobAttach.RawBase64.
					//no thumbnail
				}
				//No security log for creation of EOB's because they don't show up in the images module.
			}
			catch {
				EobAttaches.Delete(eob.EobAttachNum);
				throw;
			}
			return eob;
		}

		/// <summary></summary>
		public static EobAttach ImportEobAttach(string pathImportFrom,long claimPaymentNum) {
			string eobFolder="";
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ || CloudStorage.IsCloudStorage) {
				eobFolder=GetEobFolder();
			}
			EobAttach eob=new EobAttach();
			if(Path.GetExtension(pathImportFrom)=="") {//If the file has no extension
				eob.FileName=".jpg";
			}
			else {
				eob.FileName=Path.GetExtension(pathImportFrom);
			}
			eob.DateTCreated=File.GetLastWriteTime(pathImportFrom);
			eob.ClaimPaymentNum=claimPaymentNum;
			EobAttaches.Insert(eob);//creates filename and saves to db
			eob=EobAttaches.GetOne(eob.EobAttachNum);
			try {
				SaveEobAttach(eob,pathImportFrom,eobFolder);
				//No security log for creation of EOB's because they don't show up in the images module.
				if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
					EobAttaches.Update(eob);
				}
			}
			catch {
				EobAttaches.Delete(eob.EobAttachNum);
				throw;
			}
			return eob;
		}

		public static EhrAmendment ImportAmdAttach(Bitmap image,EhrAmendment amd) {
			string amdFolder="";
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ || CloudStorage.IsCloudStorage) {
				amdFolder=GetAmdFolder();
			}
			amd.FileName=DateTime.Now.ToString("yyyyMMdd_HHmmss_")+amd.EhrAmendmentNum;
			amd.FileName+=".jpg";
			amd.DateTAppend=DateTime.Now;
			EhrAmendments.Update(amd);
			amd=EhrAmendments.GetOne(amd.EhrAmendmentNum);
			long qualityL=(long)ComputerPrefs.LocalComputer.ScanDocQuality;
			ImageCodecInfo myImageCodecInfo;
			ImageCodecInfo[] encoders;
			encoders = ImageCodecInfo.GetImageEncoders();
			myImageCodecInfo = null;
			for(int j = 0;j < encoders.Length;j++) {
				if(encoders[j].MimeType == "image/jpeg") {
					myImageCodecInfo = encoders[j];
				}
			}
			EncoderParameters myEncoderParameters = new EncoderParameters(1);
			EncoderParameter myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality,qualityL);
			myEncoderParameters.Param[0] = myEncoderParameter;
			try {
				SaveAmdAttach(amd,image,myImageCodecInfo,myEncoderParameters,amdFolder);
				//No security log for creation of AMD Attaches because they don't show up in the images module
			}
			catch {
				//EhrAmendments.Delete(amd.EhrAmendmentNum);
				throw;
			}
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				//EhrAmendments.Update(amd);
				//no thumbnail
			}
			return amd;
		}

		public static EhrAmendment ImportAmdAttach(string pathImportFrom,EhrAmendment amd) {
			string amdFolder="";
			string amdFilename="";
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ || CloudStorage.IsCloudStorage) {
				amdFolder=GetAmdFolder();
				amdFilename=amd.FileName;
			}
			amd.FileName=DateTime.Now.ToString("yyyyMMdd_HHmmss_")+amd.EhrAmendmentNum+Path.GetExtension(pathImportFrom);
			if(Path.GetExtension(pathImportFrom)=="") {//If the file has no extension
				amd.FileName+=".jpg";
			}
			//EhrAmendments.Update(amd);
			//amd=EhrAmendments.GetOne(amd.EhrAmendmentNum);
			try {
				SaveAmdAttach(amd,pathImportFrom,amdFolder);
				//No security log for creation of AMD Attaches because they don't show up in the images module
			}
			catch {
				//EhrAmendments.Delete(amd.EhrAmendmentNum);
				throw;
			}
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ || CloudStorage.IsCloudStorage) {
				amd.DateTAppend=DateTime.Now;
				EhrAmendments.Update(amd);
				CleanAmdAttach(amdFilename);
			}
			return amd;
		}

		///<summary>Saves image obtained from eClipboard as well as inserting it into the database. Call ImageCaptureXs.SaveImageCaptureXs() to properly set up parameters</summary>
		public static void ImportImage(byte[] imageBytes,string header,Patient pat,long defNum,EncoderParameters encoderParams,ImageCodecInfo imgCodecInfo) {
			//Format the description and part of the file name to be used when saving the image. Appends a number at the end if the file name already exists
			string getDocDescription(string header,string fileFolderPath,long patNum,string date) {
				string cleanFileName=ODFileUtils.CleanFileName($@"{patNum}_{header}_{date}.bmp");
				if(!File.Exists($@"{fileFolderPath}\{cleanFileName}")) {
					return ODFileUtils.CleanFileName($"{header}_{date}");
				}
				int x=0;
				string docDesc="";
				do {
					x++;
					docDesc=ODFileUtils.CleanFileName($"{header}_{date}_{x}");
				}
				while(File.Exists($@"{fileFolderPath}\{patNum}_{docDesc}.bmp"));
				return docDesc;
			}
			Document doc=new Document();
			string patFolder=GetPatientFolder(pat,GetPreferredAtoZpath());
			string dateForDoc=DateTime.Today.ToString("yyyyMMdd");
			string docDescription=getDocDescription(header,patFolder,pat.PatNum,dateForDoc);
			doc.Description=docDescription;
			doc.DocCategory=defNum;
			doc.FileName=$"{pat.PatNum}_{docDescription}.bmp";
			doc.PatNum=pat.PatNum;
			doc.ImgType=ImageType.Photo;
			doc.DateCreated=DateTime.Now;
			long docNum=Documents.Insert(doc);
			doc=Documents.GetByNum(docNum);
			using (MemoryStream mStream=new MemoryStream(imageBytes))
			using (Bitmap bp=new Bitmap(mStream)) {
				SaveDocument(doc,bp,imgCodecInfo,encoderParams,patFolder);
			}
		}

		///<summary> Save a Document to another location on the disk (outside of Open Dental). </summary>
		public static void Export(string saveToPath,Document doc,Patient pat) {
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				byte[] rawData=Convert.FromBase64String(doc.RawBase64);
				using(FileStream file=new FileStream(saveToPath,FileMode.Create,FileAccess.Write)) {
					file.Write(rawData,0,rawData.Length);
					file.Close();
				}
			}
			else {//Using an AtoZ folder
				string docPath=FileAtoZ.CombinePaths(GetPatientFolder(pat,GetPreferredAtoZpath()),doc.FileName);
				FileAtoZ.Copy(docPath,saveToPath,FileAtoZSourceDestination.AtoZToLocal);
			}
		}

		///<summary> Save an Eob to another location on the disk (outside of Open Dental). </summary>
		public static void ExportEobAttach(string saveToPath,EobAttach eob) {
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				byte[] rawData=Convert.FromBase64String(eob.RawBase64);
				Image image=null;
				using(MemoryStream stream=new MemoryStream()) {
					stream.Read(rawData,0,rawData.Length);
					image=Image.FromStream(stream);
				}
				image.Save(saveToPath);
			}
			else {//Using an AtoZ folder
				string eobPath=ODFileUtils.CombinePaths(GetEobFolder(),eob.FileName);
				FileAtoZ.Copy(eobPath,saveToPath,FileAtoZSourceDestination.AtoZToLocal);
			}
		}

		///<summary> Save an EHR amendment to another location on the disk (outside of Open Dental). </summary>
		public static void ExportAmdAttach(string saveToPath,EhrAmendment amd) {
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				byte[] rawData=Convert.FromBase64String(amd.RawBase64);
				Image image=null;
				using(MemoryStream stream=new MemoryStream()) {
					stream.Read(rawData,0,rawData.Length);
					image=Image.FromStream(stream);
				}
				image.Save(saveToPath);
			}
			else {//Using an AtoZ folder
				string eobPath=ODFileUtils.CombinePaths(GetAmdFolder(),amd.FileName);
				FileAtoZ.Copy(eobPath,saveToPath,FileAtoZSourceDestination.AtoZToLocal);
			}
		}

		///<summary>If using AtoZ folder, then patFolder must be fully qualified and valid.  
		///If not using AtoZ folder, this uploads to Cloud or fills the doc.RawBase64 which must then be updated to db.  
		///The image format can be bmp, jpg, etc, but this overload does not allow specifying jpg compression quality.</summary>
		public static void SaveDocument(Document doc,Bitmap image,ImageFormat format,string patFolder) {
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				string pathFileOut = ODFileUtils.CombinePaths(patFolder,doc.FileName);
				image.Save(pathFileOut);
			}
			else if(CloudStorage.IsCloudStorage) {
				using(MemoryStream stream=new MemoryStream()) {
					image.Save(stream,format);
					CloudStorage.Upload(patFolder,doc.FileName,stream.ToArray());
				}
			}
			else {//saving to db
				using(MemoryStream stream=new MemoryStream()) {
					image.Save(stream,format);
					byte[] rawData=stream.ToArray();
					doc.RawBase64=Convert.ToBase64String(rawData);
				}
			}
			LogDocument(Lans.g("ContrImages","Document Created")+": ",Permissions.ImageEdit,doc,DateTime.MinValue); //a brand new document is always passed-in
		}

		///<summary>If usingAtoZfoler, then patFolder must be fully qualified and valid.  If not usingAtoZ folder, this uploads to Cloud or fills the doc.RawBase64 which must then be updated to db.</summary>
		public static void SaveDocument(Document doc,Bitmap image,ImageCodecInfo codec,EncoderParameters encoderParameters,string patFolder) {
			//Had to reassign image to new bitmap due to a possible C# bug. Would sometimes cause UE: "A generic error occurred in GDI+."
			using(Bitmap bitmap=new Bitmap(image)) {
				if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {//if saving to AtoZ folder
					bitmap.Save(ODFileUtils.CombinePaths(patFolder,doc.FileName),codec,encoderParameters);
				}
				else if(CloudStorage.IsCloudStorage) {
					using(MemoryStream stream=new MemoryStream()) {
						bitmap.Save(stream,codec,encoderParameters);
						CloudStorage.Upload(patFolder,doc.FileName,stream.ToArray());
					}
				}
				else {//if saving to db
					using(MemoryStream stream=new MemoryStream()) {
						bitmap.Save(stream,codec,encoderParameters);
						byte[] rawData=stream.ToArray();
						doc.RawBase64=Convert.ToBase64String(rawData);
					}
				}
			}
			LogDocument(Lans.g("ContrImages",doc.ImgType+" Created")+": ",Permissions.ImageCreate,doc,DateTime.MinValue); //a brand new document is always passed-in
		}

		///<summary>If using AtoZfolder, then patFolder must be fully qualified and valid.  If not using AtoZfolder, this uploads to Cloud or fills the eob.RawBase64 which must then be updated to db.</summary>
		public static void SaveDocument(Document doc,string pathSourceFile,string patFolder) {
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				File.Copy(pathSourceFile,ODFileUtils.CombinePaths(patFolder,doc.FileName));
			}
			else if(CloudStorage.IsCloudStorage) {
				CloudStorage.Upload(patFolder,doc.FileName,File.ReadAllBytes(pathSourceFile));
			}
			else {//saving to db
				byte[] rawData=File.ReadAllBytes(pathSourceFile);
				doc.RawBase64=Convert.ToBase64String(rawData);
			}
			LogDocument(Lans.g("ContrImages",doc.ImgType+" Created")+": ",Permissions.ImageCreate,doc,DateTime.MinValue); //a brand new document is always passed-in
		}

		///<summary>If using AtoZfolder, then patFolder must be fully qualified and valid.  If not using AtoZfolder, this uploads to Cloud or fills the doc.RawBase64 which must then be updated to db.</summary>
		public static void SaveDocument(Document doc,byte[] arrayBytes,string patFolder) {
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				File.WriteAllBytes(ODFileUtils.CombinePaths(patFolder,doc.FileName),arrayBytes);
			}
			else if(CloudStorage.IsCloudStorage) {
				CloudStorage.Upload(patFolder,doc.FileName,arrayBytes);
			}
			else {//Assume DataStorageType.InDatabase
				doc.RawBase64=Convert.ToBase64String(arrayBytes);
			}
			LogDocument(Lans.g("ContrImages",doc.ImgType+" Created")+": ",Permissions.ImageCreate,doc,DateTime.MinValue); //a brand new document is always passed-in
		}

		///<summary>If using AtoZfolder, then patFolder must be fully qualified and valid.  If not using AtoZfolder, this fills the eob.RawBase64 which must then be updated to db.</summary>
		public static void SaveEobAttach(EobAttach eob,Bitmap image,ImageCodecInfo codec,EncoderParameters encoderParameters,string eobFolder) {
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				image.Save(ODFileUtils.CombinePaths(eobFolder,eob.FileName),codec,encoderParameters);
			}
			else if(CloudStorage.IsCloudStorage) {
				using(MemoryStream stream=new MemoryStream()) {
					image.Save(stream,codec,encoderParameters);
					CloudStorage.Upload(eobFolder,eob.FileName,stream.ToArray());
				}
			}
			else {//saving to db
				using(MemoryStream stream=new MemoryStream()) {
					image.Save(stream,codec,encoderParameters);
					byte[] rawData=stream.ToArray();
					eob.RawBase64=Convert.ToBase64String(rawData);
				}
			}
			//No security log for creation of EOB because they don't show up in the images module.
		}

		///<summary>If using AtoZfolder, then patFolder must be fully qualified and valid.  If not using AtoZfolder, this fills the eob.RawBase64 which must then be updated to db.</summary>
		public static void SaveAmdAttach(EhrAmendment amd,Bitmap image,ImageCodecInfo codec,EncoderParameters encoderParameters,string amdFolder) {
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				image.Save(ODFileUtils.CombinePaths(amdFolder,amd.FileName),codec,encoderParameters);
			}
			else if(CloudStorage.IsCloudStorage) {
				using(MemoryStream stream=new MemoryStream()) {
					image.Save(stream,codec,encoderParameters);
					CloudStorage.Upload(amdFolder,amd.FileName,stream.ToArray());
				}
			}
			else {//saving to db
				using(MemoryStream stream=new MemoryStream()) {
					image.Save(stream,codec,encoderParameters);
					byte[] rawData=stream.ToArray();
					amd.RawBase64=Convert.ToBase64String(rawData);
					EhrAmendments.Update(amd);
				}
			}
			//No security log for creation of AMD Attaches because they don't show up in the images module
		}

		///<summary>If using AtoZfolder, then patFolder must be fully qualified and valid.  If not using AtoZfolder, this fills the eob.RawBase64 which must then be updated to db.</summary>
		public static void SaveEobAttach(EobAttach eob,string pathSourceFile,string eobFolder) {
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				File.Copy(pathSourceFile,ODFileUtils.CombinePaths(eobFolder,eob.FileName));
			}
			else if(CloudStorage.IsCloudStorage) {
				CloudStorage.Upload(eobFolder,eob.FileName,File.ReadAllBytes(pathSourceFile));
			}
			else {//saving to db
				byte[] rawData=File.ReadAllBytes(pathSourceFile);
				eob.RawBase64=Convert.ToBase64String(rawData);
			}
			//No security log for creation of EOB because they don't show up in the images module
		}

		///<summary>If using AtoZfolder, then patFolder must be fully qualified and valid.  If not using AtoZfolder, this fills the eob.RawBase64 which must then be updated to db.</summary>
		public static void SaveAmdAttach(EhrAmendment amd,string pathSourceFile,string amdFolder) {
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				File.Copy(pathSourceFile,ODFileUtils.CombinePaths(amdFolder,amd.FileName));
			}
			else if(CloudStorage.IsCloudStorage) {
				CloudStorage.Upload(amdFolder,amd.FileName,File.ReadAllBytes(pathSourceFile));
			}
			else {//saving to db
				byte[] rawData=File.ReadAllBytes(pathSourceFile);
				amd.RawBase64=Convert.ToBase64String(rawData);
				EhrAmendments.Update(amd);
			}
			//No security log for creation of AMD Attaches because they don't show up in the images module
		}

		///<summary>For each of the documents in the list, deletes row from db and image from AtoZ folder if needed.  Throws exception if the file cannot be deleted.  Surround in try/catch.</summary>
		public static void DeleteDocuments(IList<Document> documents,string patFolder) {
			for(int i=0;i<documents.Count;i++) {
				if(documents[i]==null) {
					continue;
				}
				//Check if document is referenced by a sheet. (PatImages)
				List<Sheet> sheetRefList=Sheets.GetForDocument(documents[i].DocNum);
				if(sheetRefList.Count!=0) {
					//throw Exception with error message.
					string msgText=Lans.g("ContrImages","Cannot delete image, it is referenced by sheets with the following dates")+":";
					foreach(Sheet sheet in sheetRefList) {
						msgText+="\r\n"+sheet.DateTimeSheet.ToShortDateString();
					}
					throw new Exception(msgText);
				}
				//Attempt to delete the file.
				if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
					try {
						string filePath = ODFileUtils.CombinePaths(patFolder,documents[i].FileName);
						if(File.Exists(filePath)) {
							File.Delete(filePath);
							LogDocument(Lans.g("ContrImages","Document Deleted")+": ",Permissions.ImageDelete,documents[i],documents[i].DateTStamp);
						}
					}
					catch {
						throw new Exception(Lans.g("ContrImages","Could not delete file.  It may be in use by another program, flagged as read-only, or you might not have sufficient permissions."));
					}
				}
				else if(CloudStorage.IsCloudStorage) {
					CloudStorage.Delete(ODFileUtils.CombinePaths(patFolder,documents[i].FileName,'/'));
				}
				//Row from db.  This deletes the "image file" also if it's stored in db.
				Documents.Delete(documents[i]);
			}//end documents
		}

		///<summary>Also handles deletion of db object.</summary>
		public static void DeleteEobAttach(EobAttach eob) {
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				string eobFolder=GetEobFolder();
				string filePath=ODFileUtils.CombinePaths(eobFolder,eob.FileName);
				if(File.Exists(filePath)) {
					try {
						File.Delete(filePath);
						//No security log for deletion of EOB's because they don't show up in the images module.
					}
					catch { }//file seems to be frequently locked.
				}
			}
			else if(CloudStorage.IsCloudStorage) {
				CloudStorage.Delete(ODFileUtils.CombinePaths(GetEobFolder(),eob.FileName,'/'));
			}
			//db
			EobAttaches.Delete(eob.EobAttachNum);
		}

		///<summary>Also handles deletion of db object.</summary>
		public static void DeleteAmdAttach(EhrAmendment amendment) {
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				string amdFolder=GetAmdFolder();
				string filePath=ODFileUtils.CombinePaths(amdFolder,amendment.FileName);
				if(File.Exists(filePath)) {
					try {
						File.Delete(filePath);
						//No security log for deletion of AMD Attaches because they don't show up in the images module.
					}
					catch {
						MessageBox.Show("Delete was unsuccessful. The file may be in use.");
						return;
					}//file seems to be frequently locked.
				}
			}
			else if(CloudStorage.IsCloudStorage) {
				CloudStorage.Delete(ODFileUtils.CombinePaths(GetAmdFolder(),amendment.FileName,'/'));
			}
			//db
			amendment.DateTAppend=DateTime.MinValue;
			amendment.FileName="";
			amendment.RawBase64="";
			EhrAmendments.Update(amendment);
		}

		///<summary>Attempts to delete the file for the given filePath, return true if no exception occurred (doesnt mean a file was deleted necessarily).
		///actInUseException is invoked with an exception message. Up to developer on if/what they would like to do anything with it.</summary>
		public static bool TryDeleteFile(string filePath,Action<string> actInUseException=null) {
			try {
				File.Delete(filePath);
			}
			catch(Exception ex) {
				if(!ex.Message.ToLower().Contains("being used by another process")) {
					throw ex;//Currently we only care about the above exception. Other exceptions should be brought to our attention still.
				}
				actInUseException?.Invoke(ex.Message);
				return false;
			}
			return true;
		}

		///<summary>Cleans up unreferenced Amendments</summary>
		public static void CleanAmdAttach(string amdFileName) {
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				string amdFolder=GetAmdFolder();
				string filePath=ODFileUtils.CombinePaths(amdFolder,amdFileName);
				if(File.Exists(filePath)) {
					try {
						File.Delete(filePath);
						//No security log for deletion of AMD Attaches because they don't show up in the images module.
					}
					catch {
						//MessageBox.Show("Delete was unsuccessful. The file may be in use.");
						return;
					}//file seems to be frequently locked.
				}
			}
			else if(CloudStorage.IsCloudStorage) {
				CloudStorage.Delete(ODFileUtils.CombinePaths(GetAmdFolder(),amdFileName,'/'));
			}
		}

		///<summary></summary>
		public static void DeleteThumbnailImage(Document doc,string patFolder) {
			/*if(ImageStoreIsDatabase) {
				using(IDbConnection connection = DataSettings.GetConnection())
				using(IDbCommand command = connection.CreateCommand()) {
					command.CommandText =
					@"UPDATE files SET Thumbnail = NULL WHERE DocNum = ?DocNum";

					IDataParameter docNumParameter = command.CreateParameter();
					docNumParameter.ParameterName = "?DocNum";
					docNumParameter.Value = doc.DocNum;
					command.Parameters.Add(docNumParameter);

					connection.Open();
					command.ExecuteNonQuery();
					connection.Close();
				}
			}
			else {*/
			//string patFolder=GetPatientFolder(pat);
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				string thumbnailFile=ODFileUtils.CombinePaths(patFolder,"Thumbnails",doc.FileName);
				if(File.Exists(thumbnailFile)) {
					try {
						File.Delete(thumbnailFile);
					}
					catch {
						//Two users *might* edit the same image at the same time, so the image might already be deleted.
					}
				}
			}
			else if(CloudStorage.IsCloudStorage) {
				CloudStorage.Delete(ODFileUtils.CombinePaths(patFolder,"Thumbnails",doc.FileName,'/'));
			}
		}

		public static string GetExtension(Document doc) {
			return Path.GetExtension(doc.FileName).ToLower();
		}

		public static string GetFilePath(Document doc,string patFolder) {
			//string patFolder=GetPatientFolder(pat);
			return FileAtoZ.CombinePaths(patFolder,doc.FileName);
		}

		/*
		public static bool IsImageFile(string filename) {
			try {
				Bitmap bitmap = new Bitmap(filename);
				bitmap.Dispose();
				bitmap=null;
				return true;
			}
			catch {
				return false;
			}
		}*/

		///<summary>Returns true if the given filename contains a supported file image extension.</summary>
		public static bool HasImageExtension(string fileName) {
			string ext = Path.GetExtension(fileName).ToLower();
			//The following supported bitmap types were found on a microsoft msdn page:
			//==02/25/2014 - Added .tig as an accepted image extention for tigerview enhancement.
			return (ext == ".jpg" || ext == ".jpeg" || ext == ".tga" || ext == ".bmp" || ext == ".tif" ||
				ext == ".tiff" || ext == ".gif" || ext == ".emf" || ext == ".exif" || ext == ".ico" || ext == ".png" || ext == ".wmf" || ext == ".tig");
		}

		///<summary>Makes log entry for documents.  Supply beginning text, permission, document, and the DateTStamp that the document was previously last 
		///edited.</summary>
		public static void LogDocument(string logMsgStart,Permissions perm,Document doc, DateTime secDatePrevious) {
			string logMsg=logMsgStart+doc.FileName;
			if(doc.Description!="") {
				string descriptDoc=doc.Description;
				if(descriptDoc.Length>50) {
					descriptDoc=descriptDoc.Substring(0,50);
				}
				logMsg+=" "+Lans.g("ContrImages","with description")+" "+descriptDoc;
			}
			Def docCat=Defs.GetDef(DefCat.ImageCats,doc.DocCategory);
			logMsg+=" "+Lans.g("ContrImages","with category")+" "+docCat.ItemName;
			SecurityLogs.MakeLogEntry(perm,doc.PatNum,logMsg,doc.DocNum,secDatePrevious);
		}

	}
}
