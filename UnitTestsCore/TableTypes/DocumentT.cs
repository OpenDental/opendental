using System;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class DocumentT {

		///<summary>Creates and inserts a new Document</summary>
		public static Document CreateDocument(Patient patient,string description="",DateTime dateCreated=default,long docCategory=0,string fileName="File.dat",ImageType imgType=ImageType.Document,string toothNumbers="",string note="",long provNum=0,bool printHeading=false,bool doInsert=true)
		{
			Document document=new Document();
			document.Description=description;
			document.DateCreated=dateCreated;
			document.DocCategory=docCategory;
			document.PatNum=patient.PatNum;
			document.FileName=fileName;
			document.ImgType=imgType;
			document.ToothNumbers=toothNumbers;
			document.Note=note;
			document.ProvNum=provNum;
			document.PrintHeading=printHeading;
			if(doInsert) {
				Documents.Insert(document);
			}
			return document;
		}

		///<summary>Deletes everything from the document table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearDocumentTable() {
			string command="DELETE FROM document";
			DataCore.NonQ(command);
		}

	}
}
