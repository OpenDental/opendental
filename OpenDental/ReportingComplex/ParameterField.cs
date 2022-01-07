using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental.ReportingComplex
{
	///<summary>Holds information about a parameter used in the report.</summary>
	///<remarks>A parameter is a string that can be used in a query that will be replaced by user-provided data before the query is sent.  For instance, "?date1" might be replaced with "(ProcDate = '2004-02-17' OR ProcDate = '2004-02-18')".  The output value can be multiple items connected with OR's as in the example, or it can be a single value.  The Snippet represents one of the multiple values.  In this example, it would be "ProcDate = '?'".  The ? in the Snippet will be replaced by the values provided by the user.</remarks>
	public class ParameterField{
		private string name;
		private string outputValue;
		private FieldValueType valueType;
		private ArrayList currentValues;
		private ArrayList defaultValues;
		private string promptingText;
		private string snippet;
		private EnumType enumerationType;
		private DefCat defCategory;
		private ReportFKType fKeyType;

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
		public FieldValueType ValueType{
			get{
				return valueType;
			}
			set{
				valueType=value;
			}
		}
		///<summary>The values of the parameter, typically just one. Each value can be a string, date, number, currency, Boolean, etc.  If the length of the ArrayList is 0, then the value is blank and will not be used in the query.</summary>
		public ArrayList CurrentValues{
			get{
				return currentValues;
			}
			set{
				currentValues=value;
			}
		}
		///<summary>These values are stored between sessions in the database and will show in the dialog prefilled when it asks the user for values.  The length of the ArrayList can be 0 to specify that the initial value is blank.</summary>
		public ArrayList DefaultValues{
			get{
				return defaultValues;
			}
			set{
				defaultValues=value;
			}
		}
		///<summary>The text that prompts the user what to enter for this parameter.</summary>
		public string PromptingText{
			get{
				return promptingText;
			}
			set{
				promptingText=value;
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
		///<summary>If the ValueKind is EnumField, then this specifies which type of enum. It is the string name of the type.</summary>
		public EnumType EnumerationType{
			get{
				return enumerationType;
			}
			set{
				enumerationType=value;
			}
		}
		///<summary>If ValueKind is DefParameter, then this specifies which DefCat.</summary>
		public DefCat DefCategory{
			get{
				return defCategory;
			}
			set{
				defCategory=value;
			}
		}
		///<summary>If ValueKind is ForeignKey, then this specifies which one.</summary>
		public ReportFKType FKeyType{
			get{
				return fKeyType;
			}
			set{
				fKeyType=value;
			}
		}
#endregion

		///<summary>Default constructor. Used when retrieving data from db.</summary>
		public ParameterField(){
			
		}

		///<summary>This is how parameters are generally added.  The currentValues and outputValue will be determined during the Report.SubmitQuery call.</summary>
		public ParameterField(string thisName,FieldValueType thisValueType,object thisDefaultValue,string thisPromptingText,string thisSnippet){
			name=thisName;
			valueType=thisValueType;
			defaultValues=new ArrayList();
			defaultValues.Add(thisDefaultValue);
			promptingText=thisPromptingText;
			snippet=thisSnippet;
			enumerationType=EnumType.ApptStatus;//arbitrary
			defCategory=DefCat.AccountColors;//arbitrary
			fKeyType=ReportFKType.None;
		}

		///<summary>Overload for ValueKind Enum.</summary>
		public ParameterField(string thisName,FieldValueType thisValueType,ArrayList theseDefaultValues,string thisPromptingText,string thisSnippet,EnumType thisEnumerationType){
			name=thisName;
			valueType=thisValueType;
			defaultValues=theseDefaultValues;
			promptingText=thisPromptingText;
			snippet=thisSnippet;
			enumerationType=thisEnumerationType;
			defCategory=DefCat.AccountColors;//arbitrary
			fKeyType=ReportFKType.None;
		}

		///<summary>Overload for ValueKind DefCat.</summary>
		public ParameterField(string thisName,FieldValueType thisValueType,ArrayList theseDefaultValues,string thisPromptingText,string thisSnippet,DefCat thisDefCategory){
			name=thisName;
			valueType=thisValueType;
			defaultValues=theseDefaultValues;
			promptingText=thisPromptingText;
			snippet=thisSnippet;
			enumerationType=EnumType.ApptStatus;//arbitrary
			defCategory=thisDefCategory;
			fKeyType=ReportFKType.None;
		}

		///<summary>Overload for ValueKind ForeignKey.</summary>
		public ParameterField(string thisName,FieldValueType thisValueType,ArrayList theseDefaultValues,string thisPromptingText,string thisSnippet,ReportFKType thisReportFKType){
			name=thisName;
			valueType=thisValueType;
			defaultValues=theseDefaultValues;
			promptingText=thisPromptingText;
			snippet=thisSnippet;
			enumerationType=EnumType.ApptStatus;//arbitrary
			defCategory=DefCat.AccountColors;//arbitrary
			fKeyType=thisReportFKType;
		}

		/*
		///<summary></summary>
		public ParameterDef(string name, ParameterValueKind valueKind, ArrayList myValues){
			Name=name;
			ParamValueKind=valueKind;
			if(!ApplyParamValue(myValues))
				MessageBox.Show("Invalid values.");
		}*/



		///<summary>Applies a value to the specified parameter field of a report.  The currentValues is usually just a single value.  The only time there will be multiple values is for a def or an enum.  For example, if a user selects multiple items from a dropdown box for this parameter, then each item is connected by an OR.  The entire output value is surrounded by parentheses.</summary>
		public void ApplyParamValues(){
			outputValue="(";
			if(currentValues.Count==0){//if there are no values
				outputValue+="1";//display a 1 (true) to effectively exclude this snippet
			}
			for(int i=0;i<currentValues.Count;i++){
				if(i>0){
					outputValue+=" OR";
				}
				if(valueType==FieldValueType.Boolean){
					if((bool)currentValues[i]){
						outputValue+=snippet;//snippet will show. There is no ? substitution
					}
					else{
						outputValue+="1";//instead of the snippet, a 1 will show
					}
				}
				else if(valueType==FieldValueType.Date){
					outputValue+=" "+Regex.Replace(snippet,@"\?",POut.Date((DateTime)currentValues[i],false));
				}
				else if(valueType==FieldValueType.Def){
					outputValue+=" "+Regex.Replace(snippet,@"\?",POut.Long((int)currentValues[i]));
				}
				else if(valueType==FieldValueType.Enum){
					outputValue+=" "+Regex.Replace(snippet,@"\?",POut.Long((int)currentValues[i]));
				}
				else if(valueType==FieldValueType.Integer){
					outputValue+=" "+Regex.Replace(snippet,@"\?",POut.Long((int)currentValues[i]));
				}
				else	if(valueType==FieldValueType.String){
					outputValue+=" "+Regex.Replace(snippet,@"\?",POut.String((string)currentValues[i]));
				}
				else if(valueType==FieldValueType.Number){
					outputValue+=" "+Regex.Replace(snippet,@"\?",POut.Double((double)currentValues[i]));
				}
			}//for i
			outputValue+=")";
			//MessageBox.Show(outputValue);
		}	

		
		
	}

	
	

	///<summary>Specifies the type of value that the field will accept.  Used in ParameterDef.ValueType and ReportObject.ValueType properties.  Also used in the ContrMultInput control to determine what kind of input to display.</summary>
	public enum FieldValueType{
		///<summary>Field takes a date value.</summary>
		Date,
		///<summary>Field takes a string value.</summary>
		String,
		///<summary>Field takes a boolean value.  For a Parameter, if false, then the snippet will not even be included. Because of the way this is implemented, the snippet can specify a true or false value, and the user can select whether to include the snippet.  So the parameter can specify whether to include a false value among many other possibilities.  There should not be a ? in a boolean snippet.</summary>
		Boolean,
		///<summary>Field takes an integer value.</summary>
		Integer,
		///<summary>Field takes a number(double) value which can include a decimal.</summary>
		Number,
		///<summary>Field takes an enumeration value(s), usually in int form from a dropdown list.</summary>
		Enum,
		///<summary>Field takes definition.DefNum value from a def category. Presented to user as a dropdown list for that category.</summary>
		Def,
		///<summary>Only used in ReportObject. When a table comes back from the database, if the expected value is an age, then this column type should be used.  Just retreive the birthdate and the program will convert it to an age.</summary>
		Age,
		///<summary></summary>
		ForeignKey,
		///<summary>Only used in FormQuestionnaire.</summary>
		YesNoUnknown
	}

	

}
