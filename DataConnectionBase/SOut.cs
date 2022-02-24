using CodeBase;
using System;
#if !DOT_NET_STANDARD
using System.Drawing.Imaging;
#endif
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DataConnectionBase {

	///<summary>Converts various datatypes into strings formatted correctly for MySQL. 
	///"S" is short for Scrub because this class was written specifically to replace parameters in the mysql queries.</summary>
	public class SOut{

		///<summary></summary>
		public static string Bool(bool myBool){
			if (myBool==true){
				return "1";
			}
			else{
				return "0";
			}
		}

		///<summary></summary>
		public static string Byte(byte myByte){
			return myByte.ToString();
		}

		///<summary>Always encapsulates the result, depending on the current database connection.</summary>
		public static string DateT(DateTime myDateT) {
			return DateT(myDateT,true);
		}

		///<summary></summary>
		public static string DateT(DateTime myDateT,bool encapsulate){
			if(myDateT.Year<1880) {
				myDateT=DateTime.MinValue;
			}
			try{
				string outDate=myDateT.ToString("yyyy-MM-dd HH:mm:ss",CultureInfo.InvariantCulture);//new DateTimeFormatInfo());
				string frontCap="'";
				string backCap="'";
				if(DataConnection.DBtype==DatabaseType.Oracle) {
					frontCap="TO_DATE('";
					backCap="','YYYY-MM-DD HH24:MI:SS')";
				}
				if(encapsulate) {
					outDate=frontCap+outDate+backCap;
				}
				return outDate;
			}
			catch(Exception ex) {
				ex.DoNothing();
				return "";//this saves zeros to a mysql database
			}
		}

		///<summary>Converts a date to yyyy-MM-dd format which is the format required by MySQL. myDate is the date you want to convert. encapsulate is true for the first overload, making the result look like this: 'yyyy-MM-dd' for MySQL.</summary>
		public static string Date(DateTime myDate){
			return Date(myDate,true);
		}

		public static string Date(DateTime myDate,bool encapsulate){
			try{
				//the new DTFormatInfo is to prevent changes in year for Korea
				string outDate=myDate.ToString("yyyy-MM-dd",new DateTimeFormatInfo());
				string frontCap="'";
				string backCap="'";
				if(DataConnection.DBtype==DatabaseType.Oracle){
					frontCap="TO_DATE('";
					backCap="','YYYY-MM-DD')";
				}
				if(encapsulate){
					outDate=frontCap+outDate+backCap;
				}
				return outDate;
			}
			catch(Exception ex) {
				ex.DoNothing();
				return "";//this saves zeros to the database
			}
		}

		///<summary>Timespans that might be invalid time of day.  Can be + or - and can be up to 800+ hours.  Stored in Oracle as varchar2.  Never encapsulates</summary>
		public static string TSpan(TimeSpan myTimeSpan) {
			if(myTimeSpan==System.TimeSpan.Zero) {
				return "00:00:00"; ;
			}
			try {
				string retval="";
				if(myTimeSpan < System.TimeSpan.Zero) {
					retval+="-";
					myTimeSpan=myTimeSpan.Duration();
				}
				int hours=(myTimeSpan.Days*24)+myTimeSpan.Hours;
				retval+=hours.ToString().PadLeft(2,'0')+":"+myTimeSpan.Minutes.ToString().PadLeft(2,'0')+":"+myTimeSpan.Seconds.ToString().PadLeft(2,'0');
				return retval;
			} 
			catch(Exception ex) {
				ex.DoNothing();
				return "00:00:00";
			}
		}

		///<summary>Timespans that are guaranteed to always be a valid time of day.  No negatives or hours over 24.  Stored in Oracle as datetime.  Encapsulated by default.</summary>
		public static string Time(TimeSpan myTimeSpan) {
			return SOut.Time(myTimeSpan,true);
		}

		///<summary>Timespans that are guaranteed to always be a valid time of day.  No negatives or hours over 24.  Stored in Oracle as datetime.  Encapsulated by default.</summary>
		public static string Time(TimeSpan myTimeSpan,bool encapsulate) {
			string retval=myTimeSpan.Hours.ToString().PadLeft(2,'0')+":"+myTimeSpan.Minutes.ToString().PadLeft(2,'0')+":"+myTimeSpan.Seconds.ToString().PadLeft(2,'0');
			if(encapsulate) {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					return "'"+retval+"'";
				}
				else {//Oracle
					return "TO_TIMESTAMP('"+retval+"','HH24:MI:SS')";
				}
			}
			else {
				return retval;
			}
		}

		///<summary>By default, rounds input up to max of 2 decimal places. EG: .0047 will return "0.00"; .0051 will return "0.01".
		///Set doRounding false when the double passed in needs to be Multiple Precision Floating-Point Reliable (MPFR).
		///Set doUseEnUSFormat to true to use a period no matter what region.</summary>
		public static string Double(double myDouble,bool doRounding=true,bool doUseEnUSFormat=false) {
			try {
				if(doRounding) {
					//because decimal is a comma in Europe, this sends it to db with period instead 
					return myDouble.ToString("f",CultureInfo.InvariantCulture);
				}
				else if(doUseEnUSFormat) {
					NumberFormatInfo format=new NumberFormatInfo();
					format.NumberDecimalSeparator=".";
					format.NumberGroupSeparator=",";
					return myDouble.ToString(format);
				}
				//This will send the double to the database with a comma for some countries.  E.g. Europe uses commas instead of periods.
				return myDouble.ToString();
			}
			catch(Exception ex) {
				ex.DoNothing();
				return "0";
			}
		}

		///<summary>Rounds input up to max of 2 decimal places. EG: .0047 will return "0.00"; .0051 will return "0.01".</summary>
		public static string Decimal(decimal myDecimal){
			try{
				//because decimal is a comma in Europe, this sends it to db with period instead 
				return myDecimal.ToString("f",CultureInfo.InvariantCulture);
			}
			catch(Exception ex) {
				ex.DoNothing();
				return "0";
			}
		}

		///<summary></summary>
		public static string Long(long myLong){
			return myLong.ToString();
		}

		///<summary></summary>
		public static string Int(int myInt) {
			return myInt.ToString();
		}

		///<summary></summary>
		public static string Enum<T>(T enumValue) where T:Enum {
			return Int((int)(object)enumValue);
		}

		///<summary></summary>
		public static string Float(float myFloat){
			return myFloat.ToString(CultureInfo.InvariantCulture);//sends as comma in Europe.  (comes back from mysql later as a period)
		}

		///<summary>Escapes all necessary characters.</summary>
		public static string String(string myString){
			if(myString==null) {
				return "";
			}
			StringBuilder strBuild=new StringBuilder();
			myString=StringScrub(myString);
			for(int i=0;i<myString.Length;i++){
				if(DataConnection.DBtype==DatabaseType.Oracle) {
					switch(myString.Substring(i,1)) {
						case "'": strBuild.Append("''"); break;// ' replaced by ''
						default: strBuild.Append(myString.Substring(i,1)); break;
					}
				}
				else {
					switch(myString.Substring(i,1)) {
						//note. When using binary data, must escape ',",\, and nul(? haven't done nul)
						//_ and % are special characters in LIKE clauses.  But they need not be escaped.  Only a potential problem when using LIKE.
						case "'": strBuild.Append(@"\'"); break;// ' replaced by \'
						case "\"": strBuild.Append("\\\""); break;// " replaced by \"
						case @"\": strBuild.Append(@"\\"); break;//single \ replaced by \\
						case "\r": strBuild.Append(@"\r"); break;//carriage return(usually followed by new line)
						case "\n": strBuild.Append(@"\n"); break;//new line
						case "\t": strBuild.Append(@"\t"); break;//tab
						default: strBuild.Append(myString.Substring(i,1)); break;
					}
				}
			}
			return strBuild.ToString();
		}

		///<summary>Should never be used outside of the crud.  Used for large columns (i.e. text, mediumtext, longtext) where it is possible to enter too 
		///many consecutive new line characters for the windows control to draw.  This can cause a graphics memory error.  If there are more than 50 
		///consecutive new line characters, this will replace them with a single new line.  It will do the same with tabs.  Any null characters will also
		///removed.</summary>
		///<param name="doEscapeCharacters">Only needs to be true when using parameters to construct the query. When true, will call POut.String.</param>
		public static string StringNote(string myString,bool doEscapeCharacters=false) {
			if(myString==null) {
				return "";
			}
			myString=StringScrub(myString);
			myString=myString.Replace("\r\n","\n");
			myString=myString.Replace("\r","\n");
			myString=Regex.Replace(myString,@"[\s]{100,}"," ");//{100,} means 100 or more. \s is any whitespace.
			myString=Regex.Replace(myString,@"[\0]","");//take out null character. Any other characters that cause problems should be added here.
			myString=myString.Replace("\n","\r\n");
			if(doEscapeCharacters) {
				return String(myString);
			}
			return myString;//Do not use POut.String here.  Crud will handle via db parameters.
		}

		///<summary>Should never be used outside of the crud. Scrubs unwanted unicode symbols from SQL parameters.</summary>
		public static string StringParam(string myString) {
			if(myString==null) {
				return "";
			}
			myString=StringScrub(myString);
			return myString;//Do not use POut.String here.  Crud will handle via db parameters.
		}

#if !DOT_NET_STANDARD
		///<summary></summary>
		public static string Bitmap(System.Drawing.Bitmap bitmap,ImageFormat imageFormat) {
			if(bitmap==null) {
				return "";
			}
			using(MemoryStream stream=new MemoryStream()) {
				bitmap.Save(stream,imageFormat);//was Bmp, then Png, then user defined.  So there will be a mix of different kinds.
				byte[] rawData=stream.ToArray();
				return Convert.ToBase64String(rawData);
			}
		}
#endif

		///<summary>Converts the specified wav file into a string representation.  The timing of this is a little different than with the other "P" functions and is only used by the import button in FormSigElementDefEdit.  After that, the wav spends the rest of it's life as a string until "played" or exported.</summary>
		public static string Sound(string filename) {
			if(!File.Exists(filename)) {
				throw new ApplicationException("File does not exist.");
			}
			if(!filename.ToLower().EndsWith(".wav")) {
				throw new ApplicationException("Filename must end with .wav");
			}
			FileStream stream=new FileStream(filename,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
			byte[] rawData=new byte[stream.Length];
			stream.Read(rawData,0,(int)stream.Length);
			return Convert.ToBase64String(rawData);
		}

		///<summary>The supplied string should already be in safe base64 format, and should not need any special escaping.  The purpose of this function is to enforce that the supplied string meets these requirements.  This is done quickly.</summary>
		public static string Base64(string myString){
			if(myString==null){
				return "";
			}
			if(!Regex.IsMatch(myString,"[A-Z0-9]*")){
				throw new ApplicationException("Characters found that do not match base64 format.");
			}
			return myString;
		}

		///<summary>Removes unsupported UTF-8 characters from the string so that inserting into the database can preserve as much of the string as possible.</summary>
		private static string StringScrub(string myString) {
			//Explicitly check for exactly null in order to preserve old behavior of returning an empty string.
			if(myString==null) {
				return "";
			}
			//Return whatever was passed in if only white space was passed in to preserve old behavior (e.g. myString set to many tabs in a row).
			if(string.IsNullOrWhiteSpace(myString)) {
				return myString;
			}
			//Had an issue with emojis in text described in TaskNum #1196599. Any text after an emoji would be truncated when saved to db.
			//The following regular expressions check the entered text for any characters falling within specific unicode ranges and removes them.
			//Hex codes \u2600 to \u26FF is the range for Miscellaneous Symbols.
			//Hex codes \u2700 to \u27BF is the range for Dingbats. (there are no hex codes inbetween Misc Symbols and Dingbats)
			//Hex codes \u27C0 to \u27EF is the range for Miscellaneous Mathementical Symbols-A
			//Hex codes \u27F0 to \u27FF is the range for Supplemental Arrows-A
			myString=Regex.Replace(myString,@"[\u2600-\u27FF]","\uFFFD");
			//Hex code \uFFFD is the "replacement character" 
			//(The replacement character is the diamond with a ? in it to denote a symbol was entered that the program doesn't know how to interpret)
			//myString=Regex.Replace(myString,@"\uFFFD","");
			//The following regular expressions are for unicode symbols that are surrogate pairs
			//Documentation for surrogates in unicode: https://goo.gl/H9tCKq (MSDN docs link shortened with Google URL shortener)
			//Surrogate pair, D83C points to a set of unicode symbols, [\uDC00-\uDFFF] is a range of symbols within that set
			//The D83C surrogate range loosely translates to hex codes \u1F000 to \u1F3FF which covers the following unicode blocks:
			//Mahjong Tiles, Domino Tiles, Playing Cards, Enclosed Alphanumeric Supplement, Enclosed Ideographic Supplement
			//and half of Miscellaneous Symbols And Pictographs
			//The D83D surrogate range loosely translates to hex codes \u1F400 to \u1F7FF which covers the following unicode blocks:
			//2nd half of Miscellaneous Symbols And Pictographs, Emoticons, Ornamental Dingbats, Transport And Map Symbols,
			//Alchemical Symbols and Geometric Shapes Extended
			//The D83E surrogate range loosely translates to hex codes \u1F910 to \u1F9C0 which covers the Supplemental Symbols And Pictographs blocks:
			myString=Regex.Replace(myString,@"\uD83C[\uDC00-\uDFFF]","\uFFFD");
			myString=Regex.Replace(myString,@"\uD83D[\uDC00-\uDFFF]","\uFFFD");
			myString=Regex.Replace(myString,@"\uD83E[\uDD10-\uDDC0]","\uFFFD");
			return myString;
		}

		///<summary>Returns true if some characters would be stripped out of str before appending to a query.</summary>
		public static bool HasInjectionChars(string str) {
			string strShort=SOut.String(str);
			return(str!=strShort);
		}

	}
}