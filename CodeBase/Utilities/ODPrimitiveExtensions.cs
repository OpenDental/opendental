using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeBase {
	///<summary>Extension methods are extremely rare in OD.  Jordan must approve any new ones.</summary>
	public static class ODPrimitiveExtensions{
		///<summary>Returns a string array that contains the substrings in this string that are delimited by the specified string.</summary>
		///<param name="separator">A string that delimits the substrings in this string.</param>
		///<param name="options">StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or StringSplitOptions.None to include empty array elements in the array returned.</param>
		public static string[] Split(this string stringToSplit,string separator,StringSplitOptions options) {
			//jordan Approved.  .NET Core includes this overload, and it's an obvious omission.  Designed to mimic the other overloads.
			return stringToSplit.Split(new string[] { separator },options);
		}

		///<summary>Returns true if the List is null or the count is 0.</summary>
		public static bool IsNullOrEmpty<T>(this List<T> list) {
			//jordan Approved. 
			return list==null || list.Count()==0;
		}

		///<summary>Returns true if the Array is null or the length is 0.</summary>
		public static bool IsNullOrEmpty(this Array array) {
			//jordan Approved. 
			return array==null || array.Length==0;
		}

		///<summary>Returns true if the string is null or empty.</summary>
		public static bool IsNullOrEmpty(this string str) {
			//jordan Approved.
			return string.IsNullOrEmpty(str);
		}

		///<summary>Returns the Description attribute, if available. If not, returns enum.ToString().  Pass it through translation after retrieving from here.</summary>
		public static string GetDescription(this Enum value,bool useShortVersionIfAvailable = false) {
			//jordan Approved.
			Type type = value.GetType();
			string name = Enum.GetName(type,value);
			if(name==null) {
				return value.ToString();
			}
			FieldInfo fieldInfo = type.GetField(name);
			if(fieldInfo==null) {
				return value.ToString();
			}
			if(useShortVersionIfAvailable) {
				ShortDescriptionAttribute attrShort=(ShortDescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo,typeof(ShortDescriptionAttribute));
				if(attrShort!=null) {
					return attrShort.ShortDesc;
				}
			}
			DescriptionAttribute attr=(DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo,typeof(DescriptionAttribute));
			if(attr==null) {
				return value.ToString();
			}
			return attr.Description;
		}
	}

	public class DateTools{
		///<summary>Adds x number of week days to the given DateTime. This assumes Saturday and Sunday are the weekend days.</summary>
		public static DateTime AddWeekDays(DateTime dateT,int numberOfDays) {
			int numberOfDaysToAdd=0;
			for(int i=0;i<numberOfDays;i++) {
				numberOfDaysToAdd++;
				if(dateT.AddDays(numberOfDaysToAdd).DayOfWeek==DayOfWeek.Saturday
					|| dateT.AddDays(numberOfDaysToAdd).DayOfWeek==DayOfWeek.Sunday) 
				{
					i--;
				}
			}
			dateT=dateT.AddDays(numberOfDaysToAdd);
			return dateT;
		}

		public static DateTime ToBeginningOfSecond(DateTime dateT) {
			return new DateTime(dateT.Year,dateT.Month,dateT.Day,dateT.Hour,dateT.Minute,dateT.Second,dateT.Kind);
		}
		
		public static DateTime ToBeginningOfMinute(DateTime dateT) {
			return new DateTime(dateT.Year,dateT.Month,dateT.Day,dateT.Hour,dateT.Minute,0,dateT.Kind);
		}
		
		public static DateTime ToBeginningOfMonth(DateTime dateT) {
			return new DateTime(dateT.Year,dateT.Month,1,0,0,0,dateT.Kind);
		}
		
		public static DateTime ToEndOfMonth(DateTime dateT) {
			return new DateTime(dateT.Year,dateT.Month,DateTime.DaysInMonth(dateT.Year,dateT.Month),23,59,59,dateT.Kind);
		}
		
		public static DateTime ToEndOfMinute(DateTime dateT) {
			return new DateTime(dateT.Year,dateT.Month,dateT.Day,dateT.Hour,dateT.Minute,59,dateT.Kind).AddMilliseconds(999);
		}

		///<summary>Returns true if the difference between now and the given datetime is greater than the timeSpan.</summary>
		public static bool IsOlderThan(DateTime dateT,TimeSpan timeSpan) {
			return (DateTime_.Now-dateT) > timeSpan;
		}

		public static string ToStringDHM(TimeSpan ts) {
			return string.Format("{0:%d} Days {0:%h} Hours {0:%m} Minutes",ts);
		}
		
		public static string ToStringDH(TimeSpan ts) {
			return string.Format("{0:%d} Days {0:%h} Hours",ts);
		}

		///<summary>Returns true if the difference between now and the given datetime is less than the timeSpan.</summary>
		public static bool IsNewerThan(DateTime dateT,TimeSpan timeSpan) {
			return (DateTime_.Now-dateT) < timeSpan;
		}
	}

	public class StringTools{
		///<summary>Removes end of string to limit it to a specified number of characters.  Optional ... elipsis.  Handles null, etc.</summary>
		public static string Truncate(string s,int maxCharacters,bool hasElipsis=false) {
			if(s==null || string.IsNullOrEmpty(s) || maxCharacters<1) {
				return "";
			}
			if(s.Length>maxCharacters) {
				if(hasElipsis && maxCharacters>4) {
					return s.Substring(0,maxCharacters-3)+"...";
				}
				return s.Substring(0,maxCharacters);
			}
			return s;
		}
		
		///<summary>Removes characters from the beginning of a string, up to maxCharacters.</summary>
		public static string TruncateBeginning(string s,int maxCharacters) {
			if(s==null || string.IsNullOrEmpty(s) || maxCharacters<1) {
				return "";
			}
			if(s.Length>maxCharacters) {
				return s.Substring(s.Length-maxCharacters,maxCharacters);
			}
			return s;
		}

		///<summary>Convert the first char in the string to upper case. The rest of the string will be lower case.</summary>
		public static string ToUpperFirstOnly(string value) {
			if(string.IsNullOrEmpty(value)) {
				return value;
			}
			if(value.Length==1) {
				return value.ToUpper();
			}
			return value.Substring(0,1).ToUpper()+value.Substring(1,value.Length-1).ToLower();
		}

		///<summary>Removes all characters from the string that are not digits.</summary>
		public static string StripNonDigits(string value) {
			if(string.IsNullOrEmpty(value)) {
				return value;
			}
			return new string(Array.FindAll(value.ToCharArray(),y => char.IsDigit(y)));
		}

		///<summary>Adds a new line if the string is not empty and appends the addition.</summary>
		public static string AppendLine(string orig,string addition) {
			if(string.IsNullOrEmpty(orig)){
				return addition;
			}
			if(orig!="") {
				orig+="\r\n";
			}
			return orig+addition;
		}

		///<summary>Returns everything in the string after the "beforeThis" string. Throws exception if "beforeThis" is not present in the string.</summary>
		///<exception cref="IndexOutOfRangeException" />
		public static string SubstringBefore(string value,string beforeThis) {
			return value.Substring(0,value.IndexOf(beforeThis));
		}

		///<summary>Returns everything in the value string before the targetCount number of target string.
		///TargetCount is 1 based.
		///Returns empty string if not found or if targetCount is greater then the number of occurances of target in value.</summary>
		public static string SubstringBefore(string value,char target,int targetCount) {
			if(string.IsNullOrEmpty(value)) {
				return value;
			}
			List<string> listValues=value.Split(target).ToList();
			if(listValues.Count<targetCount) {//targetCount is greater then the number of occurances of target in value.
				return value;
			}
			return string.Join(target.ToString(),listValues.GetRange(0,targetCount));
		}

		///<summary>Returns everything in the string after the "afterThis" string. Throws exception if "afterThis" is not present in the string.</summary>
		public static string SubstringAfter(string value,string afterThis,bool isCaseSensitive=true) {
			int idxStart;
			if(isCaseSensitive) {
				idxStart=value.IndexOf(afterThis);
			}
			else {
				idxStart=value.ToLower().IndexOf(afterThis.ToLower());
			}
			return value.Substring(idxStart+afterThis.Length);
		}

		///<summary>Use regular expressions to do an in-situ string replacement. Default behavior is case insensitive.</summary>
		/// <param name="pattern">Must be a REGEX compatible pattern.</param>
		/// <param name="replacement">The string that should be used to replace each occurance of the pattern.</param>
		/// <param name="regexOptions">IgnoreCase by default, allows others.</param>
		public static void RegReplace(StringBuilder stringBuilder, string pattern, string replacement,RegexOptions regexOptions=RegexOptions.IgnoreCase) {
			string newVal=Regex.Replace(stringBuilder.ToString(),pattern,replacement,regexOptions);
			stringBuilder.Clear();
			stringBuilder.Append(newVal);
		}

		///<summary>Returns the given email masked. If there is an error with this email, will return blank.</summary>
		public static string MaskEmail(string email) {
			if(string.IsNullOrEmpty(email)) {
				return "";
			}
			string emailUserName="";
			ODException.SwallowAnyException(() => {
				//This can throw if @ does not exist in the email.
				emailUserName=SubstringBefore(email,"@");
			});
			//There username should be at least 2 characters.
			if(emailUserName.Length < 2) {
				return "";
			}
			//Show first two characters.
			string emailMasked=$"{emailUserName[0]}{emailUserName[1]}";
			//X for every character after that.
			for(int i = 2;i<emailUserName.Length;i++) {
				emailMasked+="X";
			}
			emailMasked+=$"@{SubstringAfter(email,"@")}";
			return emailMasked;
		}

		///<summary>Returns the given phone number masked. If there is an error with this phone number, will return blank.</summary>
		public static string MaskPhoneNumber(string phoneNumber) {
			if(phoneNumber.Length < 10) {//Phone numbers must be at least 10 numbers long.
				return "";
			}
			//If there is a country code or some other code, grab the last 10 digits.
			string phoneNumberStr=phoneNumber.Substring(phoneNumber.Length-10);
			return $"XXX-XXX-{phoneNumberStr[6]}{phoneNumberStr[7]}{phoneNumberStr[8]}{phoneNumberStr[9]}";
		}
	}

	public class CompareDecimal{
		///<summary>Used to check if a decimal number is "equal" to zero based on some epsilon. Epsilon is 0.0000001M and will return true if the absolute value of the decimal is less than that.</summary>
		public static bool IsZero(decimal val) {
			return Math.Abs(val)<=0.0000001M;
		}

		///<summary>Used to check if a decimal number is "less than" zero based on some epsilon.</summary>
		public static bool IsLessThanZero(decimal val) {
			return val < -0.0000001M;
		}

		///<summary>Used to check if a decimal number is "less than" zero based on some epsilon.</summary>
		public static bool IsLessThanOrEqualToZero(decimal val) {
			return (val < -0.0000001M || Math.Abs(val)<=0.0000001M);
		}

		///<summary>Used to check if a double number is "greater than" zero based on some epsilon.</summary>
		public static bool IsGreaterThanOrEqualToZero(double val) {
			return (val > 0.0000001f || Math.Abs(val)<=0.0000001f);
		}

		///<summary>Used to check if a decimal number is "greater than" zero based on some epsilon.</summary>
		public static bool IsGreaterThanOrEqualToZero(decimal val) {
			return (val > 0.0000001M || Math.Abs(val)<=0.0000001M);
		}

		///<summary>Used to check if a decimal is "greater than" another decimal based on some epsilon.</summary>
		public static bool IsGreaterThan(decimal val,decimal val2) {
			return val-val2 > 0.0000001M;
		}

		///<summary>Used to check if a decimal number is "greater than" zero based on some epsilon.</summary>
		public static bool IsGreaterThanZero(decimal val) {
			return val > 0.0000001M;
		}

		///<summary>Used to check if a double number is "greater than" zero based on some epsilon.</summary>
		public static bool IsGreaterThanZero(double val) {
			return val > 0.0000001f;
		}
		
		public static bool IsEqual(decimal val,decimal val2) {
			return Math.Abs(val-val2) < 0.0000001M;
		}
	}

	public class CompareDouble{
		///<summary>Used to check if a floating point number is "equal" to zero based on some epsilon.  Epsilon is 0.0000001f and will return true if the absolute value of the double is less than that.</summary>
		public static bool IsZero(double val) {
			return Math.Abs(val)<=0.0000001f;
		}

		public static bool IsEqual(double val,double val2) {
			return IsZero(val-val2);
		}

		///<summary>Used to check if a double is "less than" another double based on some epsilon.</summary>
		public static bool IsLessThan(double val,double val2) {
			return val2-val > 0.0000001f;
		}

		///<summary>Used to check if a double is "greater than" another double based on some epsilon.</summary>
		public static bool IsGreaterThan(double val,double val2) {
			return val-val2 > 0.0000001f;
		}

		///<summary>Used to check if a double number is "less than" zero based on some epsilon.</summary>
		public static bool IsLessThanZero(double val) {
			return val < -0.0000001f;
		}

		///<summary>Used to check if a double number is "less than" zero based on some epsilon.</summary>
		public static bool IsLessThanOrEqualToZero(double val) {
			return (val < -0.0000001f || Math.Abs(val)<=0.0000001f);
		}
	}

	public class CompareFloat{
		///<summary>Used to check if a floating point number is "equal" to zero based on some epsilon. Epsilon is 0.0000001f and will return true if the absolute value of the double is less than that.</summary>
		public static bool IsZero(float val) {
			return Math.Abs(val)<=0.0000001f;
		}
		
		public static bool IsEqual(float val,float val2) {
			return IsZero(val-val2);
		}

		///<summary>Returns true if the number contains a partial cent. E.g.: "0.035" returns true, "0.03" returns false.</summary>
		public static bool HasPartialCent(float val) {
			return Math.Round(val,2)!=Math.Round(val,3); 
		}
	}

	public class EnumTools{
		///<summary>Returns the attribute for the enum value if available. If not, returns the default value for the attribute.</summary>
		public static T GetAttributeOrDefault<T>(Enum value) where T:Attribute,new() {
			Type type=value.GetType();
			string name=Enum.GetName(type,value);
			if(name==null) {
				return new T();
			}
			FieldInfo field=type.GetField(name);
			if(field==null) {
				return new T();
			}
			T attr=Attribute.GetCustomAttribute(field,typeof(T)) as T;
			if(attr==null) {
				return new T();
			}
			return attr;
		}

		///<summary>Returns true if the enum value matches any of the flags passed in.</summary>
		public static bool HasAnyFlag(Enum value,params Enum[] flags) {
			long valLong=Convert.ToInt64(value);
			if(valLong==0) {
				return flags.Contains(value);
			}
			return flags.Any(x => (valLong & Convert.ToInt64(x)) > 0);
		}

		///<summary>Returns the enum value with the passed in flags added.</summary>
		public static T AddFlag<T>(Enum value,params T[] flags) {
			long valLong=Convert.ToInt64(value);
			foreach(T flagToAdd in flags) {
				valLong=valLong | Convert.ToInt64(flagToAdd);
			}
			return (T)Enum.ToObject(typeof(T),valLong);
		}

		///<summary>Returns the enum value with the passed in flags removed.</summary>
		public static T RemoveFlag<T>(Enum value,params T[] flags) {
			long valLong=Convert.ToInt64(value);
			foreach(T flagToRemove in flags) {
				valLong=valLong & ~Convert.ToInt64(flagToRemove);
			}
			return (T)Enum.ToObject(typeof(T),valLong);
		}

		///<summary>Returns a list of flags that this enum value has.  Ignores 0b0 flag if defined.</summary>
		public static IEnumerable<T> GetFlags<T>(T value) where T:Enum {
			foreach(T flag in Enum.GetValues(value.GetType()).Cast<T>().Where(x => Convert.ToInt64(x)!=0)) {
				if(value.HasFlag(flag)) {
					yield return flag;
				}
			}
		}
	}

	public class GenericTools{
		///<summary>Deep copy of the source item to a new instance of the target return type.</summary>
		public static TTarget DeepCopy<TSource,TTarget>(TSource sourceObj) where TTarget:TSource {
			if(typeof(TTarget).IsInterface) {
				throw new Exception("Cannot deep copy to an interface.  TTarget must be a concrete class.");
			}
			return (TTarget)DeepClone(sourceObj,typeof(TTarget));
		}

		///<summary>Clone the object Properties and its children recursively</summary>
		public static object DeepClone(object sourceObject,Type targetType=null) {
			if(sourceObject is null) {
				return null;
			}
			BindingFlags binding=BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
			targetType=targetType??sourceObject.GetType();
			object targetObject;
			ArgumentException ex=new ArgumentException($"Failed to copy {targetType.FullName}");
			if(sourceObject is string sourceAsString) {
				char[] targetString=new char[sourceAsString.Length];
				sourceAsString.CopyTo(0,targetString,0,sourceAsString.Length);
				targetObject=new string(targetString);
			}
			else if(sourceObject is Array sourceAsArray) {
				Array array=Array.CreateInstance(targetType.GetElementType(),sourceAsArray.Length);
				for(int i=0;i<sourceAsArray.Length;i++) {
					array.SetValue(DeepClone(sourceAsArray.GetValue(i)),i);
				}
				targetObject=array;
			}
			else if(sourceObject is IList sourceAsIList) {				
				if(sourceAsIList.IsReadOnly) {
					throw ex;//Should we just skip these?
				}
				IList list=(IList)Activator.CreateInstance(
					targetType.IsGenericTypeDefinition ? targetType.MakeGenericType(targetType.GenericTypeArguments) : targetType
				);
				foreach(var item in sourceAsIList) {
					list.Add(DeepClone(item,targetType.GenericTypeArguments.Single()));
				}
				targetObject=list;
			}
			else if(sourceObject is IDictionary sourceAsIDict) {
				if(sourceAsIDict.IsReadOnly) {
					throw ex;//Should we just skip these?
				}
				IDictionary dict=(IDictionary)Activator.CreateInstance(
					targetType.IsGenericTypeDefinition ? targetType.MakeGenericType(targetType.GenericTypeArguments) : targetType
				);
				foreach(DictionaryEntry entry in sourceAsIDict) {
					dict.Add(DeepClone(entry.Key),DeepClone(entry.Value));
				}
				targetObject=dict;
			}
			else {
				// Create an empty object and ignore its constructor.
				targetObject=FormatterServices.GetUninitializedObject(targetType);
				Type sourceType=sourceObject.GetType();
				#region Copy Properties
				foreach(PropertyInfo property in sourceType.GetProperties(binding).Where(x => x.CanWrite && x.CanRead)) {
					var value=property.GetValue(sourceObject,null);
					if(!property.PropertyType.IsPrimitive) {
						value=DeepClone(value);
					}
					targetType.GetProperty(property.Name,binding)?.SetValue(targetObject,value,null);
				}
				#endregion
				#region Copy Fields
				foreach(FieldInfo field in sourceType.GetFields(binding)) {//Only writable fields
					var value=field.GetValue(sourceObject);
					if(!field.FieldType.IsPrimitive) {
						value=DeepClone(value);
					}
					targetType.GetField(field.Name,binding)?.SetValue(targetObject,value);
				}
				#endregion
			}
			return targetObject;
		}

		///<summary>The IsODHQAttribute can  be applied to a field, and checked. Returns true if the class or field is marked ODHQ only.</summary>
		public static bool IsODHQ<T>(T val) {
			try {
				FieldInfo field=typeof(T).GetField(val.ToString());
				if(field==null) {
					return false;
				}
				IsODHQAttribute ODHQ=(IsODHQAttribute)Attribute.GetCustomAttribute(field,typeof(IsODHQAttribute));
				if(ODHQ==null) {
					return false;
				}
				return ODHQ.IsODHQ;
			}
			catch(Exception e) {
				e.DoNothing();
				return false;
			}
		}
	}

	public class ListTools{
		///<summary>Converts a single item to a List containing one item.</summary>
		public static List<T> FromSingle<T>(T item) {
			return new List<T>(1) { item };
		}

		///<summary>Compares 2 lists. Returns true if size and items are exactly the same in each list. Throws if all list items match, otherwise returns silently.</summary>
		private static void CompareList<TSource>(IEnumerable<TSource> first,IEnumerable<TSource> second,Func<TSource,TSource,bool> funcCompare = null) {
			if(first.Count()!=second.Count()) { //In case there are duplicates in either list.
				throw new Exception("Item count mismatch");
			}
			//No duplicates so any non-intersecting items indicates not a match.
			if(funcCompare==null) { //Use the default comparison func (usually for primitives only).
				if(first.Except(second).Union(second.Except(first)).Count()==0) {
					return; //Match.
				}
			}
			else { //Use the custom comparison func.
				ODEqualityComparer<TSource> compare=new ODEqualityComparer<TSource>(funcCompare);
				if(first.Except(second,compare).Union(second.Except(first,compare)).Count()==0) {
					return; //Match.
				}
			}
			throw new Exception("Items do not match");
		}

		///<summary>Deep copy each list items of the source list to a new list instance of the target return type.</summary>
		public static List<TTarget> DeepCopy<TSource, TTarget>(List<TSource> source) where TTarget : TSource {
			if(typeof(TTarget).IsInterface) {
				throw new Exception("Cannot deep copy to an interface.  TTarget must be a concrete class.");
			}
			return (List<TTarget>)GenericTools.DeepClone(source,typeof(List<TTarget>));
		}

		///<summary>Checks whether the first parameter is in the list of the other parameters. Use like this: if(!ListTools.In(x,2,3,61,71))</summary>
		public static bool In<T>(T item,params T[] list) {
			return list.Contains(item);
		}

		///<summary>You probably don't want to use this.  Try List<>.Contains() instead.  Use like this: if(!ListTools.In(x,list))</summary>
		public static bool In<T>(T item,IEnumerable<T> list) {
			return list.Contains(item);
		}

		///<summary>Compares 2 lists. Returns true if size and items are exactly the same in each list. Uses .Equals() of the given TSource type to compare so beware if comparing anything but primitives.  Returns true if lists are the same, otherwise returns false.
		///<see cref="http://stackoverflow.com/a/5620298"/></summary>
		///<typeparam name="TSource">The type being compared. Will use .Equals() to compare.</typeparam>
		public static bool TryCompareList<TSource>(IEnumerable<TSource> first,IEnumerable<TSource> second,Func<TSource,TSource,bool> funcCompare=null) {
			try {
				CompareList(first,second,funcCompare);
				return true;
			}
			catch(Exception e) {
				e.DoNothing();
			}
			return false;
		}

		///<summary>Allows for custom comparison of TSource. Implements IEqualityComparer, which is required by LINQ for inline comparisons.</summary>
		public class ODEqualityComparer<TSource>:IEqualityComparer<TSource> {
			private Func<TSource,TSource,bool> _funcCompare;

			public ODEqualityComparer(Func<TSource,TSource,bool> funcCompare) {
				this._funcCompare=funcCompare;
			}

			public bool Equals(TSource x,TSource y) {
				return _funcCompare(x,y);
			}

			public int GetHashCode(TSource obj) {				
				//Do not use obj.GetHashCode(). This will return a non-determinant value and cause .Equals() to be skipped in most cases.
				//Always return the same value (0 is acceptable). This will defer to the Equals override as the tie-breaker, which is what we want in this case.
				return 0;
			}
		}
	}

	public class ShortDescriptionAttribute:Attribute {
		public ShortDescriptionAttribute() : this("") {

		}
		public ShortDescriptionAttribute(string shortDesc) {
			ShortDesc=shortDesc;
		}

		private string _shortDesc="";
		public string ShortDesc {
			get { return _shortDesc; }
			set { _shortDesc=value; }
		}
	}

	///<summary>The IsODHQAttribute can  be applied to a field, and checked. Use in tandem with PrefC.IsODHQ to hide HQ only features.</summary>
	public class IsODHQAttribute:Attribute {
		private bool _isODHQ=false;

		///<summary></summary>
		public IsODHQAttribute() {
			IsODHQ=true;
		}

		///<summary>The class or field is only used at OD HQ. Defaults to false.</summary>
		public bool IsODHQ {
			get {
				return _isODHQ;
			}
			set {
				_isODHQ=value;
			}
		}
	}

}
