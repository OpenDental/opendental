using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary></summary>
	public class UploadS3ObjectRequest {

		///<summary>The name of the file/object.</summary>
		public string FileName { get; set; }

		///<summary>The extension of the object (i.e. png).</summary>
		public string Extension { get; set; }

		///<summary>The bytes for this object encoded in base64.</summary>
		public string ObjectBytesBase64 { get; set; }

		///<summary>The type of object being uploaded.</summary>
		public S3ObjectType ObjectType { get; set; }

		///<summary>The purpose of the s3 object.</summary>
		public S3ObjectPurpose ObjectPurpose { get; set; }

	}
}
