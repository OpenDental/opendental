using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeBase {
	public class ConvertWebCore {
		///<summary>Convert byte array to a string.
		///outputAsHex: If true then returned string hex, which doubles the size, but can safely be sent over SOAP. In the form of a hex string, EG "D1FF34DE".
		///If false then returned string is roughly the same size as the unencrypted string (plus 16 bytes or so) and is NOT safe to send over SOAP. 
		///The characters are ascii between 0-255, EG "☺☻⌡≡M♫".
		///If you aren't sure use outputAsHex=true. outputAsHex=false is typically only used for socket transmissions and encryption.</summary>
		public static string BytesToString(byte[] asBytes,bool outputAsHex) {
			if(asBytes==null || asBytes.Length<=0) {
				return "";
			}
			if(outputAsHex) { //Output to legible hex string.
				return BitConverter.ToString(asBytes).Replace("-","");
			}
			else { //Output to UTF8 bytes (0-255).
				StringBuilder sb=new StringBuilder();
				foreach(byte b in asBytes) {
					sb.Append((char)b);
				}
				return sb.ToString();
			}
		}

		///<summary>Convert ledgible hex string to byte array.
		///inputIs7BitAscii: If true then input string must be ledible ascii (characters 0-127), EG "AB12D1FF34DE".
		///If false then input string can be UTF8 (characters 0-255). 
		///If you aren't sure use inputIs7BitAscii=true. inputIs7BitAscii=false is typically only used for socket transmissions and encryption.</summary>
		public static byte[] StringToBytes(string asString,bool inputIs7BitAscii) {
			if(asString==null || asString.Length<=0) {
				return new byte[0];
			}
			byte[] bytes;
			if(inputIs7BitAscii) { //Inputs are ASCII encoded (0-127) and are ledgible.
				int numChars=asString.Length/2;
				bytes = new byte[numChars];
				using(StringReader reader=new StringReader(asString)) {
					for(int i = 0;i<numChars;i++) {
						bytes[i]=Convert.ToByte(new string(new char[2] { (char)reader.Read(),(char)reader.Read() }),16);
					}
				}
			}
			else { //Input characters are UTF8 encoded (0-255) and not necessarily ledgible.
				char[] asChars=asString.ToCharArray();
				bytes=new byte[asChars.Length];
				for(int i=0;i<asChars.Length;i++){
					bytes[i]=(byte)asChars[i];
				}
			}
			return bytes;
		}

		///<summary>Convert plain text to illedgible text.  Just a simple way to keep user from tampering with arguments sent over URL.  GWT app has a reverse algorigthm to convert back to plain text.</summary>
		public static string GetObfuscatedString(string plainText) {
			return BytesToString(Encoding.ASCII.GetBytes(plainText),true);
		}

		///<summary>Convert obfuscated string back to plain text. Text must be originally obfuscated using GetObfuscatedString().</summary>
		public static string DeobfuscateString(string obfuscated) {
			byte[] asBytes=StringToBytes(obfuscated,true);
			string ret="";
			for(int i=0;i<asBytes.Length;i++) {
				ret+=Char.ToString((char)asBytes[i]);
			}
			return ret;
		}

		///<summary>Convert a string to a relatively unique numeric representation.  GWT app has a similar algorithm that will allow simple checksum compare of strings.  Assures that strings have not been tampered with.</summary>
		public static int CreateSimpleCheckSum(string input) {
			if(string.IsNullOrEmpty(input)) {
				return 0;
			}
			//This algorithm could be improved. Right now it sill returns the same checksum if digits are swapped.
			int ret=0;
			byte[] asBytes=Encoding.ASCII.GetBytes(input);
			foreach(byte b in asBytes) {
				ret+=(int)b;
			}
			return ret;
		}

		///<summary>Create a checksum from the input string and validate it against the input checksum. Returns true if match, false is mismatch.</summary>
		public static bool ValidateCheckSum(string input,int checksum) {
			return checksum==CreateSimpleCheckSum(input);
		}

		public static int ReadIntFromByteArray(byte[] array,ref int position) {
			int ret=BitConverter.ToInt32(array,position);
			position+=4;
			return ret;
		}

		public static ushort ReadUShortFromByteArray(byte[] array,ref int position) {
			ushort ret=BitConverter.ToUInt16(array,position);
			position+=2;
			return ret;
		}

		public static void WriteUShortToByteArray(byte[] array,ref int position,ushort value) {
			Buffer.BlockCopy(BitConverter.GetBytes(value),0,array,position,2);
			position+=2;
		}

		public static void WriteIntToByteArray(byte[] array,ref int position,int value) {
			Buffer.BlockCopy(BitConverter.GetBytes(value),0,array,position,4);
			position+=4;
		}

		public static DataSet TableListToDataSet(List<DataTable> tables) {
			DataSet ds=new DataSet();
			ds.Tables.AddRange(tables.ToArray());
			return ds;
		}
	}

	///<summary>Convert objects to strings. Useful for logging.</summary>
	public static class DebugLogging {

		///<summary>Convert any object type to a loggable string for debugging. Keeps the string to one single line. Use overload to define prefix and suffix of each line.</summary>
		public static string DebugString<T>(T obj) {
			return DebugString<T>(obj,""," - ");
		}

		public static string DebugString<T>(T obj,string beforeEachField,string afterEachField) {
			return DebugString<T>(obj,beforeEachField,afterEachField," ");
		}
		
		///<summary>Convert any list of any object type to a loggable string for debugging. Each member can be prefixed with any string and suffixed with any string.</summary>
		public static string DebugString<T>(T obj,string beforeEachField,string afterEachField,string betweenEachField) {
			string ret="";
			FieldInfo[] fields=obj.GetType().GetFields();
			foreach(FieldInfo field in fields) {
				string val=field.GetValue(obj)==null?"null":field.GetValue(obj).ToString();
				if(field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
					val="";
					try {
						IDictionary iDict=(IDictionary)field.GetValue(obj);
						foreach(object key in iDict.Keys) {
							val+="{ "+key.ToString()+", "+iDict[key].ToString()+" }   ";
						}
					}
					catch {
						val="Serialization failed!";						
					}
				}
				ret+=beforeEachField+field.Name+":"+betweenEachField+val+afterEachField;				
			}
			return ret;
		}

		///<summary>Convert any list of any object type to a loggable string for debugging. Each member is prefixed with spaces and suffixed with new line.</summary>
		public static string DebugStringForList<T>(this List<T> all) {
			string ret="List<"+typeof(T).Name+"> count: "+all.Count.ToString()+" [";
			foreach(T vln in all) {
				ret+="\r\n"+DebugString<T>(vln,"  ","\r\n");
			}
			ret+="]\r\n";
			return ret;
		}

		public static string DebugPropertyStringForType(Type type,object obj,BindingFlags flags,bool doSort) {
			List<Tuple<string,string>> list=DebugPropertyPairsForType(type,obj,flags,doSort);
			StringBuilder strb=new StringBuilder();
			list.ForEach(x => strb.AppendLine(x.Item1+": "+x.Item2));
			return strb.ToString();
		}

		public static List<Tuple<string,string>> DebugPropertyPairsForType(Type type,object obj,BindingFlags flags,bool doSort) {
			List<Tuple<string,string>> ret=new List<Tuple<string,string>>();
			List<PropertyInfo> props=type.GetProperties(flags).ToList();
			foreach(PropertyInfo prop in props) {
				string val="N\\A";
				try {
					val=Convert.ToString(prop.GetValue(obj,null));
				}
				catch(Exception e) {
					e.GetType();//Can't use DoNothing() because ODCrypt is linking this file
				}
				ret.Add(new Tuple<string,string>(prop.Name,val));
			}
			List<FieldInfo> fields=type.GetFields(flags).ToList();
			foreach(FieldInfo field in fields) {
				string val="N\\A";
				try {
					val=Convert.ToString(field.GetValue(obj));
				}
				catch { }
				ret.Add(new Tuple<string,string>(field.Name,val));
			}
			if(doSort) {
				return ret.OrderBy(x => x.Item1).ToList();
			}
			return ret;
		}
	}

	///<summary>A conversion class that helps change objects of one type to the other type.
	///<para>Used when C#'s "Convert" class or simply casting to other types is not sufficient.  E.g. converting a list of objects to a list of another object type.</para></summary>
	///<typeparam name="TypeCur">The type of the object that you are converting from.</typeparam>
	///<typeparam name="TypeTo">The type of the object that you are converting to.</typeparam>
	public class Conv<TypeCur,TypeTo> {
		///<summary>List conversion.  Converts one list to a list of another type.  
		///<para>If the type of object being converted cannot convert to the desired type, a runtime exception will occur instead of a compile exception.</para>
		///<para>Make sure that any use of this method is tested before releasing code to the public.</para></summary>
		///<param name="listTypeCur">The list of objects that need to be converted to a list of another type.  Typically a list of objects.</param>
		///<returns>A list of the desired type.</returns>
		public static List<TypeTo> L(List<TypeCur> listTypeCur) {
			List<TypeTo> retVal=new List<TypeTo>();
			for(int i=0;i<listTypeCur.Count;i++) {
				retVal.Add((TypeTo)Convert.ChangeType(listTypeCur[i],typeof(TypeTo)));
			}
			return retVal;
		}
	}
}
