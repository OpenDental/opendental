using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness{
	///<summary>For storing docs/images in database.  This table is for the various miscellaneous documents that are not in the normal patient subfolders.</summary>
	[Serializable]
	public class DocumentMisc:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long DocMiscNum;
		/// <summary>Date created.</summary>
		public DateTime DateCreated;
		///<summary>The name the file would have if it was not in the database. Does not include any directory info.
		///DocumentMisc rows that store the contents of the UpdateFiles folder will set this column to an "item order".
		///Due to severe limitations with sending large amounts of data all in one query we are going to store the UpdateFiles over several rows.
		///The FileName column will store the order of which the UpdateFiles need to go back into when we try to reconstruct it.</summary>
		public string FileName;
		/// <summary>Enum:DocumentMiscType Corresponds to the same subfolder within AtoZ folder. eg. UpdateFiles</summary>
		public DocumentMiscType DocMiscType;
		///<summary>The raw file data encoded as base64.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string RawBase64;

		///<summary>Returns a copy of this DocumentMisc.</summary>
		public DocumentMisc Copy() {
			return (DocumentMisc)this.MemberwiseClone();
		}
	}

	///<summary>More types will be added to correspond to most of the subfolders inside the AtoZ folder.  But no point adding them until we implement.</summary>
	public enum DocumentMiscType {
		///<summary>0- There will just be zero or one row of this type.  It will contain a zipped archive.</summary>
		UpdateFiles,
		///<summary>1- Entries with this doc type hold segments of the UpdateFiles RawBase64 zip contents that will be pieced back together later.
		///Storing the entire Update Files contents into one row was exceeding MySQL max_allowed_packet limitations so this new type is required.
		///Each row of this type will contain ~1MB of RawBase64 data.</summary>
		UpdateFilesSegment,
		///<summary>2- Entires of this doc type are segments of OpenDentalShareScreen.exe from our website.
		///File names are formatted like version{guid}segment# (ex 12.11.2022.501{45e744a7-f6dd-4e55-9f9f-11d1f746eefe}13).
		///An OpenDentalShareScreen.exe is complete when a record is present without a segment# (ex 12.11.2022.501{45e744a7-f6dd-4e55-9f9f-11d1f746eefe}).</summary>
		ShareScreenExeSegment,
	}
	

}
