using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;

namespace ODR
{
	///<summary>Holds information about a parameter used in the report.</summary>
	///<remarks>A parameter is a string that can be used in the WHERE clause of a query that will be replaced by user-provided data before the query is sent.  For instance, "?date1" might be replaced with "(ProcDate = '2004-02-17' OR ProcDate = '2004-02-18')".  The output value can be multiple items connected with OR's as in the example, or it can be a single value.  The Snippet represents one of the multiple values.  In this example, it would be "ProcDate = '?'".  The ? in the Snippet will be replaced by the values provided by the user.</remarks>
	public class Parameter{
		private string name;
		private string outputValue;
		private ParamValueType valueType;
		private ArrayList currentValues;
		//private ArrayList defaultValues;
		private string prompt;
		private string snippet;
		private EnumType enumerationType;
		private string queryText;

#region Properties
		///<summary>This is the name as it will show in the query, but without the preceding question mark.</summary>
		public string Name{
			get{
				return name;
			}
			set{
				name=value;
			}
		}
		///<summary>The value, in text form, as it will be substituted into the main query and sent to the database.</summary>
		public string OutputValue{
			get{
				return outputValue;
			}
			set{
				outputValue=value;
			}
		}
		///<summary>The type of value that the parameter can accept.</summary>
		public ParamValueType ValueType{
			get{
				return valueType;
			}
			set{
				valueType=value;
			}
		}
		///<summary>The values of the parameter, typically just one. Each value can be a string, date, number, currency, Boolean, etc.  If the length of the ArrayList is 0, then the value is blank and will not be used in the query.  CurrentValues can be set ahead of time in the report, so in this usage, they might be thought of as default values.</summary>
		public ArrayList CurrentValues{
			get{
				return currentValues;
			}
			set{
				currentValues=value;
			}
		}
		/*
		///<summary>These values will show in the dialog preselected when it asks the user for values.  The length of the ArrayList can be 0 to specify that the initial value is blank.</summary>
		public ArrayList DefaultValues{
			get{
				return defaultValues;
			}
			set{
				defaultValues=value;
			}
		}*/
		///<summary>The text that prompts the user what to enter for this parameter.</summary>
		public string Prompt{
			get{
				return prompt;
			}
			set{
				prompt=value;
			}
		}
		///<summary>The snippet of SQL that will be repeated once for each value supplied by the user, connected with OR's, and surrounded by parentheses.</summary>
		public string Snippet{
			get{
				return snippet;
			}
			set{
				snippet=value;
			}
		}
		///<summary>If the ValueType is Enum, then this specifies which type of enum. It is the string name of the type.</summary>
		public EnumType EnumerationType{
			get{
				return enumerationType;
			}
			set{
				enumerationType=value;
			}
		}
		///<summary>If ValueType is QueryData, then this contains the query to use to get the parameter list.</summary>
		public string QueryText{
			get{
				return queryText;
			}
			set{
				queryText=value;
			}
		}
#endregion

		///<summary>Default constructor.</summary>
		public Parameter(){
			name="";
			outputValue="1";
			valueType=ParamValueType.String;
			currentValues=new ArrayList();
			//defaultValues=new ArrayList();
			prompt="";
			snippet="";
			enumerationType=EnumType.ApptStatus;//arbitrary
			queryText="";
		}

		///<summary>CurrentValues must be set first.  Then, this applies the values to the parameter to create the outputValue.  The currentValues is usually just a single value.  The only time there will be multiple values is for an Enum or QueryText.  For example, if a user selects multiple items from a listbox for this parameter, then each item is connected by an OR.  The entire OutputValue is surrounded by parentheses.</summary>
		public void FillOutputValue(){
			outputValue="(";
			if(currentValues.Count==0){//if there are no values
				outputValue+="1";//display a 1 (true) to effectively exclude this snippet
			}
			for(int i=0;i<currentValues.Count;i++){
				if(i>0){
					outputValue+=" OR ";
				}
				if(valueType==ParamValueType.Boolean){
					if((bool)currentValues[i]){
						outputValue+=snippet;//snippet will show. There is no ? substitution
					}
					else{
						outputValue+="1";//instead of the snippet, a 1 will show
					}
				}
				else if(valueType==ParamValueType.Date){
					outputValue+=Regex.Replace(snippet,@"\?",POut.Date((DateTime)currentValues[i],false));
				}
				else if(valueType==ParamValueType.Enum){
					outputValue+=Regex.Replace(snippet,@"\?",POut.Long((int)currentValues[i]));
				}
				else if(valueType==ParamValueType.Integer){
					outputValue+=Regex.Replace(snippet,@"\?",POut.Long((int)currentValues[i]));
				}
				else if(valueType==ParamValueType.String){
					outputValue+=Regex.Replace(snippet,@"\?",POut.String((string)currentValues[i]));
				}
				else if(valueType==ParamValueType.Number){
					outputValue+=Regex.Replace(snippet,@"\?",POut.Double((double)currentValues[i]));
				}
				else if(valueType==ParamValueType.QueryData){
					outputValue+=Regex.Replace(snippet,@"\?",POut.String((string)currentValues[i]));
				}
			}
			outputValue+=")";
			//MessageBox.Show(outputValue);
		}	

		
		
	}

	
	

	///<summary>Specifies the type of value that the parameter will accept.  Also used in the ContrMultInput control to determine what kind of input to display.</summary>
	public enum ParamValueType{
		///<summary>Parameter takes a date/time value.</summary>
		Date,
		///<summary>Parameter takes a string value.</summary>
		String,
		///<summary>Parameter takes a boolean value.  If false, then the snippet will not even be included. Because of the way this is implemented, the snippet can specify a true or false value, and the user can select whether to include the snippet.  So the parameter can specify whether to include a false value among many other possibilities.  There should not be a ? in a boolean snippet.</summary>
		Boolean,
		///<summary>Parameter takes an integer value.</summary>
		Integer,
		///<summary>Parameter takes a number(double) value which can include a decimal.</summary>
		Number,
		///<summary>Parameter takes an enumeration value(s).  User must select from a list.</summary>
		Enum,
		///<summary>A list will be presented to the user based on the results of this query.  Column one of the query results should contain the values, and column two should contain the display text.  One typical use is when choosing providers: "SELECT ProvNum,Abbr FROM provider WHERE IsHidden=0 ORDER BY ItemOrder"</summary>
		QueryData
	}

	

}
