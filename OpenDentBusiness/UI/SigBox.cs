using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness.UI {
	public class SigBox {

		/// <summary>When we fetch webforms, they are not yet formated as our SheetField objects, so pass in as a datarow.</summary>
		public static string GetSignatureKeySheets(List<DataRow> listSheetRows) {
			List<SheetField> listSheetFields=new List<SheetField>();
			foreach(DataRow row in listSheetRows) {
				listSheetFields.Add(new SheetField()
				{
					FieldValue=PIn.String(row["FieldValue"].ToString()),
					FieldType=PIn.Enum<SheetFieldType>(row["FieldType"].ToString())
				});
			}
			return GetSignatureKeySheets(listSheetFields);
		}

		/// <summary>Get the key used to encrypt the signature in a sheet. The key is made up of all the field values in the sheet in the order they were inserted into the db. This method assumes the list of sheet fields was already sorted.</summary>
		public static string GetSignatureKeySheets(List<SheetField> listSheetFields) {
			StringBuilder strBuild=new StringBuilder();
			for(int i=0;i<listSheetFields.Count;i++) {
				if(listSheetFields[i].FieldValue=="") {
					continue;
				}
				if(ListTools.In(listSheetFields[i].FieldType,SheetFieldType.SigBox,SheetFieldType.SigBoxPractice)) {
					continue;
				}
				strBuild.Append(listSheetFields[i].FieldValue);
			}
			return strBuild.ToString();
		}

		/// <summary>Encrypts signature text and returns a base 64 string so that it can go directly into the database. Takes in a hashed key, and a string describing the signature using semi-colon separated points (i.e. "x1,y1;x2,y2;").</summary>
		public static string EncryptSigString(byte[] key, string sigAsPoints) {
			if(string.IsNullOrWhiteSpace(sigAsPoints)){
				return "";
			}
			byte[] sigBytes=Encoding.UTF8.GetBytes(sigAsPoints);
			MemoryStream ms=new MemoryStream();
			//Compression could have been done here, using DeflateStream
			//A decision was made not to use compression because it would have taken more time and not saved much space.
			//DeflateStream compressedzipStream = new DeflateStream(ms , CompressionMode.Compress, true);
			//Now, we have the compressed bytes.  Need to encrypt them.
			Rijndael crypt=Rijndael.Create();
			crypt.KeySize=128;//16 bytes;  Because this is 128 bits, it is the same as AES.
			crypt.Key=key;
			crypt.IV=new byte[16];
			CryptoStream cs=new CryptoStream(ms,crypt.CreateEncryptor(),CryptoStreamMode.Write);
			cs.Write(sigBytes,0,sigBytes.Length);
			cs.FlushFinalBlock();
			byte[] encryptedBytes=new byte[ms.Length];
			ms.Position=0;
			ms.Read(encryptedBytes,0,(int)ms.Length);
			cs.Dispose();
			ms.Dispose();			
			return Convert.ToBase64String(encryptedBytes); //signature is preceeded by a 0 or 1 to indicate whether or not this is a topaz signature
		}

	}
}
