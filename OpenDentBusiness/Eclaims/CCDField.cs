using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace OpenDentBusiness.Eclaims {
	public class CCDField{

		///<summary>Returns the length requirement for this field, possibly based on other fields.</summary>
		private abstract class LengthRequirement{
			///<summary>Return the length required to input this field in bytes. Negative value indicates error.</summary>
			public abstract int CalcLength(CCDFieldInputter formData);
		};

		private class ConstLengthRequirement:LengthRequirement{
			int len;
			public ConstLengthRequirement(int length){
				len=length;
			}
			public override int CalcLength(CCDFieldInputter formData){
				return len;
			}
		};

		private class LengthFromAnotherField:LengthRequirement{
			string otherFieldId;
			public LengthFromAnotherField(string pOtherFieldId) {
				otherFieldId=pOtherFieldId;
			}
			public override int CalcLength(CCDFieldInputter formData){
				CCDField lengthField=formData.GetFieldById(otherFieldId);
				if(lengthField==null){
					return -1;				
				}
				if(!Regex.IsMatch(lengthField.valuestr,"^[0-9]+$")){
					MessageBox.Show(this.ToString()+".CalcLength: Internal Error, cannot load field length from non-integer field value!");
					return -1;
				}
				//Now the value string of the field is garanteed to be a valid numerical string.
				return Convert.ToInt32(lengthField.valuestr);
			}
		};

		///<summary>This field does not exist when the other specified field does not exist. If this field does not exist, the returned length is 0, so that no error occurs, but also to avoid input.</summary>
		private class ConstLengthWhenOtherFieldExists:LengthRequirement{
			string otherFieldId;
			int valueWhenExists;
			public ConstLengthWhenOtherFieldExists(string pOtherFieldId,int pValueWhenExists) {
				otherFieldId=pOtherFieldId;
				valueWhenExists=pValueWhenExists;
			}
			public override int CalcLength(CCDFieldInputter formData){
				CCDField lengthField=formData.GetFieldById(otherFieldId);
				if(lengthField==null) {
					return 0;
				}
				return valueWhenExists;
			}
		};

		private class ConstLengthWhenOtherFieldHasValue:LengthRequirement{
			string otherFieldId;
			string otherFieldValue;
			int valueWhenExists;
			public ConstLengthWhenOtherFieldHasValue(string pOtherFieldId,string pOtherFieldValue,int pValueWhenExists) {
				otherFieldId=pOtherFieldId;
				otherFieldValue=pOtherFieldValue;
				valueWhenExists=pValueWhenExists;
			}
			public override int CalcLength(CCDFieldInputter formData){
				CCDField lengthField=formData.GetFieldById(otherFieldId);
				if(lengthField==null) {
					return -1;
				}
				if(lengthField.valuestr==otherFieldValue){
					return valueWhenExists;
				}
				return 0;
			}
		};

		///<summary>Ensure that the input value meets a requirement.</summary>
		private abstract class ValueRequirement{
			public abstract bool MeetsRequirement(CCDFieldInputter formData,string value);
		};

		private class DiscreteValueRequirement:ValueRequirement{
			private string[] acceptedValues;
			public DiscreteValueRequirement(string[] pAcceptedValues) {
				acceptedValues=pAcceptedValues;
			}
			public override bool MeetsRequirement(CCDFieldInputter formData,string value) {
				for(int i=0;i<acceptedValues.Length;i++){
					if(value==acceptedValues[i]){
						return true;
					}
				}
				return false;
			}
		};

		private class DiscreteValueExclusionRequirement:ValueRequirement {
			private string[] excludedValues;
			public DiscreteValueExclusionRequirement(string[] pExcludedValues) {
				excludedValues=pExcludedValues;
			}
			public override bool MeetsRequirement(CCDFieldInputter formData,string value) {
				for(int i=0;i<excludedValues.Length;i++) {
					if(value==excludedValues[i]) {
						return false;
					}
				}
				return true;
			}
		};

		private class RangeValueRequirement:ValueRequirement {
			int minVal;
			int maxVal;
			public RangeValueRequirement(int pMinVal,int pMaxVal) {
				minVal=pMinVal;
				maxVal=pMaxVal;
			}
			public override bool MeetsRequirement(CCDFieldInputter formData,string value) {
				if(!Regex.IsMatch(value,"^[0-9]+$")) {
					MessageBox.Show(this.ToString()+".MeetsRequirements: Internal Error, cannot check range requirement against "+
													"non-integer value '"+value+"'");
					return false;
				}
				int numVal=Convert.ToInt32(value);
				return(numVal >= minVal && numVal <= maxVal);
			}
		};

		private class RegexValueRequirement:ValueRequirement{
			string pattern;
			public RegexValueRequirement(string pPattern) {
				pattern=pPattern;
			}
			public override bool MeetsRequirement(CCDFieldInputter formData,string value){
				return Regex.IsMatch(value,pattern);
			}
		};

		///<summary>A many to many mapping of discrete values.</summary>
		public class ValueMap {						
				public string[] inputValues;					//The values expected in the other field.
				public string[] discreteValues;				//The discrete values allowed for the inputValue of the other field.
				public ValueMap(string[] pInputValues,string[] pDiscreteValues) {
					inputValues=pInputValues;
					discreteValues=pDiscreteValues;
				}
			};

		private class DiscreteValuesBasedOnOtherField:ValueRequirement{
			string otherFieldId;
			ValueMap[] valueMaps;
			public DiscreteValuesBasedOnOtherField(string pOtherFieldId,ValueMap[] pValueMaps) {
				otherFieldId=pOtherFieldId;
				valueMaps=pValueMaps;
			}
			public override bool MeetsRequirement(CCDFieldInputter formData,string value) {
				CCDField otherField=formData.GetFieldById(otherFieldId);
				if(otherField==null){
					MessageBox.Show(this.ToString()+".MeetsRequirement: Internal error, field id "+otherFieldId+" does not exist!");
				}
				foreach(ValueMap valMap in valueMaps){
					foreach(string valstr in valMap.inputValues){
						if(otherField.valuestr==valstr){
							foreach(string str in valMap.discreteValues){
								if(value==str){
									return true;
								}
							}
						}
					}
				}
				return false;
			}
		};

		private LengthRequirement lengthRequirement;//Used to determine the length of the field for input.
		private List <ValueRequirement> valueRequirements=new List<ValueRequirement>();
		//Used to check the value of the field during the input process.

		public string fieldId;
		public string fieldName;
		public string frenchFieldName;//The equivalent of the fieldName in French.
		public string format;
		public string valuestr;//null indicates unused. Final value set elsewhere.

		public CCDField(CCDField RHS) {
			this.fieldId=RHS.fieldId;
			this.fieldName=RHS.fieldName;
			this.frenchFieldName=RHS.frenchFieldName;
			this.format=RHS.format;
			this.valuestr=RHS.valuestr;
		}

		public string GetFieldName(bool useFrench) {
			if(useFrench&&frenchFieldName!=null) {
				return frenchFieldName;
			}
			else {
				return fieldName;
			}
		}

		public static bool IsValidId_v2(string str) {
			if(	str.Length==3 && 
					str[0]>='A' && str[0]<='G' && 
					str[1]>='0' && str[1]<='9' && 
					str[2]>='0' && str[2]<='9'){
				int num=Convert.ToInt32(str.Substring(1,2));
				if(num<1){
					return false;
				}
				switch (str[0]){
					case('A'):
						return num<=8;
					case('B'):
						return num<=2;
					case('C'):
						return num<=11;
					case('D'):
						return num<=10;
					case('E'):
						return num<=6;
					case('F'):
						return num<=15;
					case('G'):
						return num<=30;
				}
			}
			return false;
		}

		public static bool IsValidId_v4(string str) {
			if(	str.Length==3 && 
					str[0]>='A' && str[0]<='G' && 
					str[1]>='0' && str[1]<='9' && 
					str[2]>='0' && str[2]<='9'){
				int num=Convert.ToInt32(str.Substring(1,2));
				if(num<1){
					return false;
				}
				switch (str[0]){
					case('A'):
						return num<=11;
					case('B'):
						return num<=8;
					case('C'):
						return num<=19;
					case('D'):
						return num<=11;
					case('E'):
						return num<=20;
					case('F'):
						return num<=49;
					case('G'):
						return num<=62;
				}
			}
			return false;
		}

		///<summary>Returns the length required to input the field. This value may depend on other fields. Negative return value indicates error.</summary>
		public int GetRequiredLength(CCDFieldInputter formData){
			return lengthRequirement.CalcLength(formData);
		}

		///<summary>Checks to be sure that the value string set in this field meets the value requirements. This function can depend on the values of other form fields.</summary>
		public bool CheckValue(CCDFieldInputter formData,string value){
			if(value==null){
				return false;
			}
			//Db format is correct. Now check specific values.
			foreach(ValueRequirement valReq in valueRequirements){
				if(!valReq.MeetsRequirement(formData,value)){
					return false;
				}
			}
			return true;
		}

		///<summary>Create a CCD field using the field id. A field id uniquely identifies the field type, contents, etc...</summary>
		public CCDField(string pFieldId,bool isVersion02){
			pFieldId=pFieldId.ToUpper();
			if(isVersion02){
				if(!IsValidId_v2(pFieldId)){
					if(IsValidId_v4(pFieldId)){
						SetValuesUsingFieldId_v4(pFieldId);
					}
					else{
						MessageBox.Show("Cannot construct version 2 field with invalid field id: "+pFieldId);
						return;
					}
				}else{
					SetValuesUsingFieldId_v2(pFieldId);
				}
			}
			else{
				if(!IsValidId_v4(pFieldId)){
					MessageBox.Show("Cannot construct version 4 field with invalid field id: "+pFieldId);
					return;
				}
				SetValuesUsingFieldId_v4(pFieldId);
			}
			fieldId=pFieldId;
			format=format.ToUpper();//just in case an error is made above
			valuestr=null;//Must be checked and set later.
			if(valueRequirements.Count==0) {//Only add format requirement if does not already have specifically defined valid value(s).
				switch(format) {
					case "N":
						valueRequirements.Add(new RegexValueRequirement(@"^[0-9]+$"));
						break;
					case "A":
						valueRequirements.Add(new RegexValueRequirement(@"^[a-zA-Z'\-,]+$"));
						break;
					case "AE":
						//valueRequirements.Add(new RegexValueRequirement(@"^[a-zA-Z'\-,\x80-\xFF]+$"));
						break;
					case "A/N":
						//We consider character values less than 128 printable if they are in the range 32-126 (0x20-0x7E) inclusive.
						//http://msdn2.microsoft.com/en-us/library/60ecse8t(VS.80).aspx
						//valueRequirements.Add(new RegexValueRequirement(@"^[\x20-\x7E]+$"));
						break;
					case "AE/N":
						//valueRequirements.Add(new RegexValueRequirement(@"^[\x20-\x7E\x80-\xFF]+$"));
						break;
					case "D":
						valueRequirements.Add(new RegexValueRequirement(@"^[+-]?[0-9]+$"));
						break;
					default:
						MessageBox.Show(this.ToString()+".CCDField: Internal error, unrecognized field format: "+format);
						break;
				}
			}
		}

		private void SetValuesUsingFieldId_v2(string pFieldId){
			switch(pFieldId)
			{
				case "A01":
					fieldName="Transaction Prefix";
					frenchFieldName="Préfixe de transaction";
					format="A/N"; 
					lengthRequirement=new ConstLengthRequirement(12);
					break;
				case "A02":
					//Provider's Sequence Number
					fieldName="DENTAL OFFICE CLAIM REFERENCE NO";
					frenchFieldName="NO DE TRANSACTION DU CABINET";
					format="N";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "A03":
					fieldName="Format Version Number";
					frenchFieldName="Nombre de version de format";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"02","04"}));
					break;
				case "A04":
					fieldName="Transaction Code";
					frenchFieldName="Code de transaction";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] { "00","10","01","11","21","31","02","12","03","13","23","04" }));
					break;
				case "A05":
					fieldName="Carrier Identification Number";
					frenchFieldName="Numéro d'identification de porteur";
					format="N";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "A06":
					fieldName="Software System ID";
					frenchFieldName="Système logiciel identification";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(3);
					break;
				case "A07":
					fieldName="Message Length";
					frenchFieldName="Longueur de message";
					format="N";
					lengthRequirement=new ConstLengthRequirement(4);
					break;
				case "A08":
					fieldName="E-Mail Flag";
					frenchFieldName="Drapeau E-Mail";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"0","1","2"}));
					break;
				case "B01":
					//CDA Provider Number
					fieldName="UNIQUE ID NO";
					frenchFieldName="NO DU DENTISTE";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(9);
					break;
				case "B02":
					//Provider Office Number
					fieldName="OFFICE NO";
					frenchFieldName="NO DU CABINET";
					format="N";
					lengthRequirement=new ConstLengthRequirement(4);
					break;
				case "C01":
					//Primary Policy/Plan Number
					fieldName="POLICY#";
					frenchFieldName="NO DE POLICE";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "C02":
					//Subscriber Identification Number
					fieldName="CERTIFICATE NO";
					frenchFieldName="NO DE CERTIFICAT";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(11);
					break;
				case "C03":
					fieldName="Relationship Code";
					frenchFieldName="Code de rapport";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(1,5));
					break;
				case "C04":
					fieldName="Patient's Sex";
					frenchFieldName="Le sexe du patient";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"M","F"}));
					break;
				case "C05":
					fieldName="Patient's Birthday";
					frenchFieldName="L'anniversaire du patient";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "C06":
					fieldName="Patient's Last Name";
					frenchFieldName="Le dernier nom du patient";
					format="A";
					lengthRequirement=new ConstLengthRequirement(25);
					break;
				case "C07":
					fieldName="Patient's First Name";
					frenchFieldName="Le prénom du patient";
					format="A";
					lengthRequirement=new ConstLengthRequirement(15);
					break;
				case "C08":
					fieldName="Patient's Middle Initial";
					frenchFieldName="L'initiale moyenne du patient";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					break;
				case "C09":
					fieldName="Eligibility Exception Code";
					frenchFieldName="Code d'exception d'acceptabilité";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(0,3));
					break;
				case "C10":
					fieldName="Name of School";
					frenchFieldName="Nom d'école";
					format="A";
					lengthRequirement=new ConstLengthRequirement(25);
					break;
				case "C11":
					fieldName="DIVISION/SECTION NO";
					frenchFieldName="NO DE DIVISION/SECTION";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(10);
					break;
				case "D01":
					fieldName="Subscriber's Birthday";
					frenchFieldName="L'anniversaire de l'abonné";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "D02":
					fieldName="Subscriber's Last Name";
					frenchFieldName="Le dernier nom de l'abonné";
					format="A";
					lengthRequirement=new ConstLengthRequirement(25);
					break;
				case "D03":
					fieldName="Subscriber's First Name";
					frenchFieldName="Le prénom de l'abonné";
					format="A";
					lengthRequirement=new ConstLengthRequirement(15);
					break;
				case "D04":
					fieldName="Subscriber's Middle Initial";
					frenchFieldName="L'initiale moyenne de l'abonné";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1); 
					break;
				case "D05":
					fieldName="Subscriber's Address Line 1";
					frenchFieldName="Ligne 1 de l'adresse de l'abonné";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(30);
					break;
				case "D06":
					fieldName="Subscriber's Address Line 2";
					frenchFieldName="Ligne 2 de l'adresse de l'abonné";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(30);
					break;
				case "D07":
					fieldName="Subscriber's City";
					frenchFieldName="La ville de l'abonné";
					format="A";
					lengthRequirement=new ConstLengthRequirement(20);
					break;
				case "D08":
					fieldName="Subscriber's Province/State Code";
					frenchFieldName="Code de la province/état de l'abonné";
					format="A";
					lengthRequirement=new ConstLengthRequirement(2);
					//Includes US states and Canadian provinces.
					//http://www.nrcan.gc.ca/earth-sciences/geography-boundary/geographical-name/translators/5782
					valueRequirements.Add(new DiscreteValueRequirement(new string [] {
						//Canadian province codes.
						"AB",//Alberta
						"BC",//Britich Columbia
						"MB",//Manitoba
						"NB",//New Brunswick
						"NL",//Newfoundland and Labrador
						"NS",//Nova Scotia
						"NT",//Northwest Territories
						"NU",//Nunavut
						"ON",//Ontario
						"PE",//Prince Edward Island
						"QC",//Quebec
						"SK",//Saskatchewan
						"YT", //Yukon
						//Traditional Canadian province codes which somehow made it into our application, but we are going to leave them because they are probably harmless.
						"LB",//Newfoundland and Labrador - This appeared in Canada Post publications (e.g., The Canadian Postal Code Directory) for the mainland section of the province of Newfoundland and Labrador.
						"NF",//Newfoundland and Labrador - Nfld. and later NF (the two-letter abbreviation used before the province's name changed to Newfoundland and Labrador) and T.-N. (French version, for Terre-Neuve)
						"PQ",//Quebec	- Que. and P.Q. (French version, for Province du Québec); later, PQ evolved from P.Q. as the first two-letter non-punctuated abbreviation.
						//US state codes.
						"AK","AL","AR","AZ","CA","CO","CT","DC","DE","FL",
						"GA","HI","IA","ID","IL","IN","KS","KY","LA","MA",
						"MD","ME","MI","MN","MO","MS","MT","NC","ND","NE",
						"NH","NJ","NM","NV","NY","OH","OK","OR","PA","RI",
						"SC","SD","TX","UT","VA","VT","WA","WI","WV","WY"}));
					break;
				case "D09":
					fieldName="Subscriber's Postal/ZIP Code";
					frenchFieldName="Code du Postal/ZIP de l'abonné";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "D10":
					fieldName="Language of the Insured";
					frenchFieldName="Langue des assurés";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"E","F"}));
					break;
				case "E01":
					fieldName="Secondary Carrier Unique ID Number";
					frenchFieldName="Nombre unique d'identification de porteur secondaire";
					format="N";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "E02":
					fieldName="Secondary Policy/Plan";
					frenchFieldName="Politique/plan secondaires";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "E03":
					fieldName="Secondary Plan Subscriber ID";
					frenchFieldName="Identification secondaire d'abonné de plan";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(11);
					break;
				case "E04":
					//Spouse/Significant Other Birtday
					fieldName="Secondary Subscriber's Birthday";
					frenchFieldName="L'anniversaire de l'abonné secondaire";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "E05":
					fieldName="Secondary Division/Section Number";
					frenchFieldName="Nombre secondaire de Division/section";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(10);
					break;
				case "E06":
					fieldName="Secondary Relationship Code";
					frenchFieldName="Code secondaire de rapport";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(1,5));
					break;
				case "F01":
					fieldName="Payee Code";
					frenchFieldName="Code de bénéficiaire";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(0,2));
					break;
				case "F02":
					fieldName="Accident Date";
					frenchFieldName="Date d'accidents";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "F03":
					fieldName="Predetermination Number";
					frenchFieldName="Nombre de prédétermination";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(14);
					break;
				case "F04":
					fieldName="Date of Initial Placement Upper";
					frenchFieldName="Date de haut initial de placement";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "F05":
					fieldName="Treatment Required for orthodontic";
					frenchFieldName="Traitement requis pour orthodontique";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"Y","N"}));
					break;
				case "F06":
					fieldName="Number of Procedures Performed";
					frenchFieldName="Nombre de procédures exécutées";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(1,7));
					break;
				case "F07":
					fieldName="Procedure Line Number";
					frenchFieldName="Ligne nombre de procédé";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(0,7));
					break;
				case "F08":
					fieldName="Procedure Code";
					frenchFieldName="Code de procédé";
					format="N";
					lengthRequirement=new ConstLengthRequirement(5);
					break;
				case "F09":
					fieldName="Date of Service";
					frenchFieldName="Date de service";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "F10":
					fieldName="International Tooth,Sextant, Quad or Arch";
					frenchFieldName="Dent, sextant, quadruple ou voûte international";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					break;
				case "F11":
					fieldName="Tooth Surface";
					frenchFieldName="Surface de dent";
					format="A";
					lengthRequirement=new ConstLengthRequirement(5);
					valueRequirements.Add(new RegexValueRequirement("^[MOIDBLV ]+$"));
					break;
				case "F12":
					fieldName="Dentist's Fee Claimed";
					frenchFieldName="Les honoraires du dentiste réclamés";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "F13":
					fieldName="Lab Procedure Fee # 1";
					frenchFieldName="Honoraires # 1 de procédé de laboratoire";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "F14":
					fieldName="Unit of Time";
					frenchFieldName="Unité de temps";
					format="D";
					lengthRequirement=new ConstLengthRequirement(4);
					break;
				case "F15":
					fieldName="Is this an Initial Placement Upper";
					frenchFieldName="Est c'un premier haut de placement";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"Y","N","X"}));
					break;
				case "G01":
					//Transaction Reference Number
					fieldName="CARRIER CLAIM NO";
					frenchFieldName="NO DE RÉFÉRENCE DE TRANSACTION";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(14);
					break;
				case "G02":
					fieldName="Employer Certified Flag";
					frenchFieldName="";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"Y","N" }));
					break;
				case "G03":
					fieldName="Expected Payment Date";
					frenchFieldName="Date prévue de paiement";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "G04":
					fieldName="Total Amount of Service";
					frenchFieldName="Montant total de service";
					format="D";
					lengthRequirement=new ConstLengthRequirement(7);
					break;
				case "G05":
					fieldName="Response Status";
					frenchFieldName="Statut de réponse";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"A","E","R","H","B","C","N","M","X"}));
					break;
				case "G06":
					fieldName="Number of Error Codes";
					frenchFieldName="Nombre de codes d'erreur";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					valueRequirements.Add(new RangeValueRequirement(0,10));
					break;
				case "G07":
					//Disposition message
					fieldName="DISPOSITION";
					frenchFieldName="SPÉCIFICATIONS";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(75);
					break;
				case "G08":
					fieldName="Error Code";
					frenchFieldName="Code d'erreur";
					format="N";
					lengthRequirement=new ConstLengthRequirement(3);
					break;
				case "G09":
					fieldName="E-Mail Flag";
					frenchFieldName="Drapeau E-Mail";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"0","1","2"}));
					break;
				case "G10":
					fieldName="Number of Carrier Issued Procedure Codes";
					frenchFieldName="Le nombre de porteur a publié des codes de procédé";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(0,6));
					break;
				case "G11":
					fieldName="Number of Note Lines";
					frenchFieldName="Nombre de lignes de note";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					valueRequirements.Add(new RangeValueRequirement(0,12));
					break;
				case "G12":
					fieldName="Eligible Amount";
					frenchFieldName="Quantité éligible";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "G13":
					fieldName="Deductible Amount";
					frenchFieldName="Quantité déductible";
					format="D";
					lengthRequirement=new ConstLengthRequirement(5);
					break;
				case "G14":
					fieldName="Eligible Percentage";
					frenchFieldName="Pourcentage éligible";
					format="N";
					lengthRequirement=new ConstLengthRequirement(3);
					valueRequirements.Add(new RangeValueRequirement(0,100));
					break;
				case "G15":
					fieldName="Benefit Amount for the Procedure";
					frenchFieldName="Quantité d'avantage pour le procédé";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "G16":
					fieldName="Explanation Note Number 1";
					frenchFieldName="Note numéro 1 d'explication";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					break;
				case "G17":
					fieldName="Explanation Note Number 2";
					frenchFieldName="Note numéro 2 d'explication";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					break;
				case "G18":
					fieldName="Reference to Line Number of the Submitted Procedure";
					frenchFieldName="Référence à la ligne nombre du procédé soumis";
					format="N";
					lengthRequirement=new ConstLengthRequirement(7);
					valueRequirements.Add(new RegexValueRequirement("^[0-7]+$"));
					break;
				case "G19":
					fieldName="Additional Procedure Code";
					frenchFieldName="Code additionnel de procédé";
					format="N";
					lengthRequirement=new ConstLengthRequirement(5);
					break;
				case "G20":
					fieldName="Eligible Amount for the Additional Procedure";
					frenchFieldName="Quantité éligible pour le procédé additionnel";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "G21":
					fieldName="Dedutible for the Additional Procedure";
					frenchFieldName="";
					format="D";
					lengthRequirement=new ConstLengthRequirement(5);
					break;
				case "G22":
					fieldName="Eligible Percentage";
					frenchFieldName="Pourcentage éligible";
					format="N";
					lengthRequirement=new ConstLengthRequirement(3);
					valueRequirements.Add(new RangeValueRequirement(0,100));
					break;
				case "G23":
					fieldName="Benefit Amount for the Additional Procedure";
					frenchFieldName="Quantité d'avantage pour le procédé additionnel";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "G24":
					fieldName="Explanation Note Number 1 for the Additional Procedure";
					frenchFieldName="Note d'explication numéro 1 pour le procédé additionnel";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					break;
				case "G25":
					fieldName="Explanation Note Number 2 for the Additional Procedure";
					frenchFieldName="Note d'explication numéro 2 pour le procédé additionnel";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					break;
				case "G26":
					fieldName="Note Text";
					frenchFieldName="Noter le texte";
					format="A";
					lengthRequirement=new ConstLengthRequirement(75);
					break;
				case "G27":
					fieldName="Language of the Insured";
					frenchFieldName="Langue des assurés";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"E","F"}));
					break;
				case "G28":
					fieldName="Total Benefit Amount";
					frenchFieldName="Quantité totale d'avantage";
					format="D";
					lengthRequirement=new ConstLengthRequirement(7);
					break;
				case "G29":
					fieldName="Deductible amount unallocated";
					frenchFieldName="La quantité déductible a désassigné";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "G30":
					//Transaction Validation Code
					fieldName="VERIFICATION NO";
					frenchFieldName="CODE DE VALIDATION";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(10);
					break;
				default:
					MessageBox.Show("Internal Error, unknown version 2 CCD field ID during construction: "+pFieldId);
					return;//error
			}
		}

		private void SetValuesUsingFieldId_v4(string pFieldId){
			//Includes US states and Canadian provinces.
			//http://www.nrcan.gc.ca/earth-sciences/geography-boundary/geographical-name/translators/5782
			string[] stateCodes=new string [] {	
				//Canadian province codes.
				"AB",//Alberta
				"BC",//Britich Columbia
				"MB",//Manitoba
				"NB",//New Brunswick
				"NL",//Newfoundland and Labrador
				"NS",//Nova Scotia
				"NT",//Northwest Territories
				"NU",//Nunavut
				"ON",//Ontario
				"PE",//Prince Edward Island
				"QC",//Quebec
				"SK",//Saskatchewan
				"YT", //Yukon
				//Traditional Canadian province codes which somehow made it into our application, but we are going to leave them because they are probably harmless.
				"LB",//Newfoundland and Labrador - This appeared in Canada Post publications (e.g., The Canadian Postal Code Directory) for the mainland section of the province of Newfoundland and Labrador.
				"NF",//Newfoundland and Labrador - Nfld. and later NF (the two-letter abbreviation used before the province's name changed to Newfoundland and Labrador) and T.-N. (French version, for Terre-Neuve)
				"PQ",//Quebec	- Que. and P.Q. (French version, for Province du Québec); later, PQ evolved from P.Q. as the first two-letter non-punctuated abbreviation.
				//US state codes.
				"AK","AL","AR","AZ","CA","CO","CT","DC","DE","FL",
				"GA","HI","IA","ID","IL","IN","KS","KY","LA","MA",
				"MD","ME","MI","MN","MO","MS","MT","NC","ND","NE",
				"NH","NJ","NM","NV","NY","OH","OK","OR","PA","RI",
				"SC","SD","TX","UT","VA","VT","WA","WI","WV","WY"};
			string[] languageCodes=new string[] {"A","E","F"};
			ValueMap valueMap;
			//Values in the following table were taken from the data dictionary of the CCD doc (about page 70).
			switch(pFieldId)
			{
				case "A01":
					fieldName="Transaction Prefix";
					frenchFieldName="Préfixe de transaction";
					format="A/N"; 
					lengthRequirement=new ConstLengthRequirement(12);
					break;
				case "A02":
					//DENTAL OFFICE CLAIM REFERENCE NO
					fieldName="DENTAL OFFICE CLAIM REFERENCE NO";
					frenchFieldName="NO DE TRANSACTION DU CABINET";
					format="N";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "A03":
					fieldName="Format Version Number";
					frenchFieldName="Nombre de version de format";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"02","04"}));
					break;
				case "A04":
					fieldName="Transaction Code";
					frenchFieldName="Code de transaction";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {	"01","11","21","02","12","03","13","23","04","14",
																																						"24","05","15","06","16","07","08","18"}));
					break;
				case "A05":
					fieldName="Carrier Identification Number";
					frenchFieldName="Numéro d'identification de porteur";
					format="N";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "A06":
					fieldName="Software System ID";
					frenchFieldName="Système logiciel identification";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(3);
					break;
				case "A07":
					fieldName="Message Length";
					frenchFieldName="Longueur de message";
					format="N";
					lengthRequirement=new ConstLengthRequirement(5);
					break;
				case "A08":
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(1);
					fieldName="Materials Forwarded";
					frenchFieldName="Les matériaux ont expédié";
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {" ","E","C","M","X","I","A","B","D","F","G",
						"H","J","K","L","N","O","P","Q","R","T","U"}));
					break;
				case "A09"://Not in version 2.
					fieldName="Carrier Transaction Counter";
					frenchFieldName="Compteur de transaction de porteur";
					format="N";
					lengthRequirement=new ConstLengthRequirement(5);
					break;
				case "A10"://Not in version 2.
					fieldName="Encryption Method";
					frenchFieldName="Méthode de chiffrage";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(1,3));
					break;
				case "A11"://Not in version 2.
					fieldName="Mailbox Indicator";
					frenchFieldName="Indicateur de boîte aux lettres";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"Y","O","N"}));
					break;
				case "B01":
					//CDA Provider Number
					fieldName="UNIQUE ID NO";
					frenchFieldName="NO DU DENTISTE";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(9);
					break;
				case "B02":
					//Provider Office Number
					fieldName="OFFICE NO";
					frenchFieldName="NO DU CABINET";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(4);
					break;
				case "B03"://Not in version 2.
					fieldName="BILLING PROVIDER NUMBER";
					frenchFieldName="NOMBRE DE FOURNISSEUR DE FACTURATION";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(9);
					break;
				case "B04"://Not in version 2.
					fieldName="BILLING OFFICE NUMBER";
					frenchFieldName="NOMBRE D'OFFICE DE FACTURATION";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(4);
					break;
				case "B05"://Not in version 2.
					fieldName="Referring Provider Number";
					frenchFieldName="Référence du nombre de fournisseur";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(10);
					break;
				case "B06"://Not in version 2.
					fieldName="Referral Reason Code";
					frenchFieldName="Code complémentaire de référence";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					valueRequirements.Add(new RangeValueRequirement(0,13));
					break;
				case "B07"://Not in version 2.
					fieldName="Receiving Provider Number";
					frenchFieldName="Réception du nombre de fournisseur";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(9);
					break;
				case "B08"://Not in version 2.
					fieldName="Receiving Office Number";
					frenchFieldName="Réception du nombre d'Office";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(4);
					break;
				case "C01":
					//Primary Policy/Plan Number
					fieldName="POLICY#";
					frenchFieldName="NO DE POLICE";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(12);
					break;
				case "C02":
					//Subscriber Identification Number
					fieldName="CERTIFICATE NO";
					frenchFieldName="NO DE CERTIFICAT";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(12);
					break;
				case "C03":
					fieldName="Relationship Code";
					frenchFieldName="Code de rapport";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(1,5));
					break;
				case "C04":
					fieldName="Patient's Sex";
					frenchFieldName="Le sexe du patient";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"M","F"}));
					break;
				case "C05":
					fieldName="Patient's Birthday";
					frenchFieldName="L'anniversaire du patient";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "C06":
					fieldName="Patient's Last Name";
					frenchFieldName="Le dernier nom du patient";
					format="AE";
					lengthRequirement=new ConstLengthRequirement(25);
					break;
				case "C07":
					fieldName="Patient's First Name";
					frenchFieldName="Le prénom du patient";
					format="AE";
					lengthRequirement=new ConstLengthRequirement(15);
					break;
				case "C08":
					fieldName="Patient's Middle Initial";
					frenchFieldName="L'initiale moyenne du patient";
					format="AE";
					lengthRequirement=new ConstLengthRequirement(1);
					break;
				case "C09":
					fieldName="Eligibility Exception Code";
					frenchFieldName="Code d'exception d'acceptabilité";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(1,4));
					break;
				case "C10":
					fieldName="Name of School";
					frenchFieldName="Nom d'école";
					format="AE/N";
					lengthRequirement=new ConstLengthRequirement(25);
					break;
				case "C11":
					fieldName="DIVISION/SECTION NO";
					frenchFieldName="NO DE DIVISION/SECTION";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(10);
					break;
				case "C12"://Not in version 2.
					fieldName="Plan Flag";
					frenchFieldName="Drapeau de plan";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {" ","A","V","N"}));
					break;
				case "C13"://Not in version 2.
					fieldName="Band Number";
					frenchFieldName="Nombre de bande";
					format="N";
					lengthRequirement=new ConstLengthRequirement(3);
					break;
				case "C14"://Not in version 2.
					fieldName="Family Number";
					frenchFieldName="Nombre de famille";
					format="N";
					lengthRequirement=new ConstLengthRequirement(5);
					break;
				case "C15"://Not in version 2.
					fieldName="Missing Teeth";
					frenchFieldName="Dents absentes";
					format="A";
					lengthRequirement=new ConstLengthRequirement(11);
					break;
				case "C16"://Not in version 2.
					fieldName="Eligibility Date";
					frenchFieldName="Date d'acceptabilité";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "C17"://Not in version 2.
					fieldName="Primary Dependant Code";
					frenchFieldName="Code dépendant primaire";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					break;
				case "C18"://Not in version 2.
					fieldName="Plan Record Count";
					frenchFieldName="Plan Coun record";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(0,1));
					//valueMap=new ValueMap(new string[] {"A","N"},new string[] {"1"});
					//valueRequirements.Add(new DiscreteValuesBasedOnOtherField("C12",new ValueMap[] {valueMap}));
					break;
				case "C19"://Not in version 2.
					fieldName="Plan Record";
					frenchFieldName="Disque de plan";
					format="AN";
					lengthRequirement=new ConstLengthRequirement(30);
					break;
				case "D01":
					fieldName="Subscriber's Birthday";
					frenchFieldName="L'anniversaire de l'abonné";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "D02":
					fieldName="Subscriber's Last Name";
					frenchFieldName="Le dernier nom de l'abonné";
					format="AE";
					lengthRequirement=new ConstLengthRequirement(25);
					break;
				case "D03":
					fieldName="Subscriber's First Name";
					frenchFieldName="Le prénom de l'abonné";
					format="AE";
					lengthRequirement=new ConstLengthRequirement(15);
					break;
				case "D04":
					fieldName="Subscriber's Middle Initial";
					frenchFieldName="L'initiale moyenne de l'abonné";
					format="AE";
					lengthRequirement=new ConstLengthRequirement(1); 
					break;
				case "D05":
					fieldName="Subscriber's Address Line 1";
					frenchFieldName="Ligne 1 de l'adresse de l'abonné";
					format="AE/N";
					lengthRequirement=new ConstLengthRequirement(30);
					break;
				case "D06":
					fieldName="Subscriber's Address Line 2";
					frenchFieldName="Ligne 2 de l'adresse de l'abonné";
					format="AE/N";
					lengthRequirement=new ConstLengthRequirement(30);
					break;
				case "D07":
					fieldName="Subscriber's City";
					frenchFieldName="La ville de l'abonné";
					format="AE/N";
					lengthRequirement=new ConstLengthRequirement(20);
					break;
				case "D08":
					fieldName="Subscriber's Province/State Code";
					frenchFieldName="Code de la province/état de l'abonné";
					format="A";
					lengthRequirement=new ConstLengthRequirement(2);
					valueRequirements.Add(new DiscreteValueRequirement(stateCodes));
					break;
				case "D09":
					fieldName="Subscriber's Postal/ZIP Code";
					frenchFieldName="Code du Postal/ZIP de l'abonné";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(9);
					break;
				case "D10":
					fieldName="Language of the Insured";
					frenchFieldName="Langue des assurés";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(languageCodes));
					break;
				case "D11"://Not in version 2.
					fieldName="Card Sequence/Version Number";
					frenchFieldName="Ordre de carte/nombre de version";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					valueRequirements.Add(new RangeValueRequirement(0,99));//This field is optional.  Is set to 0 when not used.
					break;
				case "E01":
					fieldName="Secondary Carrier Unique ID Number";
					frenchFieldName="Nombre unique d'identification de porteur secondaire";
					format="N";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "E02":
					fieldName="Secondary Policy/Plan";
					frenchFieldName="Politique/plan secondaires";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(12);
					break;
				case "E03":
					fieldName="Secondary Plan Subscriber ID";
					frenchFieldName="Identification secondaire d'abonné de plan";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(12);
					break;
				case "E04":
					fieldName="Secondary Subscriber's Birthday";
					frenchFieldName="L'anniversaire de l'abonné secondaire";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "E05":
					fieldName="Secondary Division/Section Number";
					frenchFieldName="Nombre secondaire de Division/section";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(10);
					break;
				case "E06":
					fieldName="Secondary Relationship Code";
					frenchFieldName="Code secondaire de rapport";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(1,5));
					break;
				case "E07"://Not in version 2.
					fieldName="Secondary Card Sequence/Version Number";
					frenchFieldName="Ordre de carte secondaire/nombre de version";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					break;
				case "E08"://Not in version 2.
					fieldName="Secondary Subscriber's Last Name";
					frenchFieldName="Le dernier nom de l'abonné secondaire";
					format="AE";
					lengthRequirement=new ConstLengthRequirement(25);
					break;
				case "E09"://Not in version 2.
					fieldName="Secondary Subscriber's First Name";
					frenchFieldName="Le prénom de l'abonné secondaire";
					format="AE";
					lengthRequirement=new ConstLengthRequirement(15);
					break;
				case "E10"://Not in version 2.
					fieldName="Secondary Subscriber's Middle Initial";
					frenchFieldName="L'initiale moyenne de l'abonné secondaire";
					format="AE";
					lengthRequirement=new ConstLengthRequirement(1);
					break;
				case "E11"://Not in version 2.
					fieldName="Secondary Subscriber's Address Line 1";
					frenchFieldName="Ligne 1 de l'adresse de l'abonné secondaire";
					format="AE/N";
					lengthRequirement=new ConstLengthRequirement(30);
					break;
				case "E12"://Not in version 2.
					fieldName="Secondary Subscriber's Address Line 2";
					frenchFieldName="Ligne 2 de l'adresse de l'abonné secondaire";
					format="AE/N";
					lengthRequirement=new ConstLengthRequirement(30);
					break;
				case "E13"://Not in version 2.
					fieldName="Secondary Subscriber's City";
					frenchFieldName="La ville de l'abonné secondaire";
					format="AE/N";
					lengthRequirement=new ConstLengthRequirement(20);
					break;
				case "E14"://Not in version 2.
					fieldName="Secondary Subscriber's Province/State Code";
					frenchFieldName="Code de la province/état de l'abonné secondaire";
					format="A";
					lengthRequirement=new ConstLengthRequirement(2);
					valueRequirements.Add(new DiscreteValueRequirement(stateCodes));
					break;
				case "E15"://Not in version 2.
					fieldName="Secondary Subscriber's Postal/ZIP Code";
					frenchFieldName="Code du Postal/ZIP de l'abonné secondaire";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(9);
					break;
				case "E16"://Not in version 2.
					fieldName="Secondary Language";
					frenchFieldName="Langue secondaire";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(languageCodes));
					break;
				case "E17"://Not in version 2.
					fieldName="Secondary Dependant Code";
					frenchFieldName="Code dépendant secondaire";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2); 
					break;
				case "E18"://Not in version 2.
					fieldName="Secondary Coverage";
					frenchFieldName="Assurance secondaire";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"Y","O","N","X"}));
					break;
				case "E19"://Not in version 2.
					fieldName="Secondary Carrier Transaction Counter";
					frenchFieldName="Compteur secondaire de transaction de porteur";
					format="N";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "E20"://Not in version 2.
					fieldName="Secondary Record Count";
					frenchFieldName="Compte record secondaire";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(0,1));
					//valueMap=new ValueMap(new string[] {"Y","O"},new string[] {"1"});
					//valueRequirements.Add(new DiscreteValuesBasedOnOtherField("E18",new ValueMap[]{valueMap}));
					break;
				case "F01":
					fieldName="Payee Code";
					frenchFieldName="Code de bénéficiaire";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(1,4));
					break;
				case "F02":
					fieldName="Accident Date";
					frenchFieldName="Date d'accidents";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "F03":
					fieldName="Predetermination Number";
					frenchFieldName="Nombre de prédétermination";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(14);
					break;
				case "F04":
					fieldName="Date of Initial Placement Upper";
					frenchFieldName="Date de haut initial de placement";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "F05":
					fieldName="Treatment Required for orthodontic";
					frenchFieldName="Traitement requis pour orthodontique";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"Y","O","N"}));
					break;
				case "F06":
					fieldName="Number of Procedures Performed";
					frenchFieldName="Nombre de procédures exécutées";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(1,7));
					break;
				case "F07":
					fieldName="Procedure Line Number";
					frenchFieldName="Ligne nombre de procédé";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(0,7));
					break;
				case "F08":
					fieldName="Procedure Code";
					frenchFieldName="Code de procédé";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(5);
					break;
				case "F09":
					fieldName="Date of Service";
					frenchFieldName="Date de service";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "F10":
					fieldName="International Tooth,Sextant, Quad or Arch";
					frenchFieldName="Dent, sextant, quadruple ou voûte international";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					break;
				case "F11":
					fieldName="Tooth Surface";
					frenchFieldName="Surface de dent";
					format="A";
					lengthRequirement=new ConstLengthRequirement(5);
					valueRequirements.Add(new RegexValueRequirement("^[MOIDBLV ]+$"));
					break;
				case "F12":
					fieldName="Dentist's Fee Claimed";
					frenchFieldName="Les honoraires du dentiste réclamés";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "F13":
					fieldName="Lab Procedure Fee # 1";
					frenchFieldName="Honoraires # 1 de procédé de laboratoire";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				//case "F14": // Does not exist in data dictionary!
				//	break;
				case "F15":
					fieldName="Is this an Initial Placement Upper";
					frenchFieldName="Est c'un premier haut de placement";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"Y","O","N","X"}));
					break;
				case "F16"://Not in version 2.
					fieldName="Procedure Type Codes";
					frenchFieldName="Type codes de procédé";
					format="A";
					lengthRequirement=new ConstLengthRequirement(5);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"A    ","B    ","C    ","E    ","L    ","S    ","X    "}));
					break;
				case "F17"://Not in version 2.
					fieldName="Remarks Code";
					frenchFieldName="Code de remarques";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					break;
				case "F18"://Not in version 2.
					fieldName="Is this an Initial Placement Lower";
					frenchFieldName="Est c'un premier placement inférieur";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] { "Y","O","N","X" }));
					break;
				case "F19"://Not in version 2.
					fieldName="Date of Initial Placement Lower";
					frenchFieldName="La date du placement initial s'abaissent";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "F20"://Not in version 2.
					fieldName="Maxillary Prosthesis Material";
					frenchFieldName="Matériel maxillaire de prothèse";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(0,6));//This field is optional.  Is set to 0 when not used.
					break;
				case "F21"://Not in version 2.
					fieldName="Mandibular Prosthesis Material";
					frenchFieldName="Matériel mandibulaire de prothèse";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(0,6));//This field is optional.  Is set to 0 when not used.
					break;
				case "F22"://Not in version 2.
					fieldName="Extracted Teeth Count";
					frenchFieldName="Compte extrait de dents";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					valueRequirements.Add(new RangeValueRequirement(0,50));
					break;
				case "F23"://Not in version 2.
					fieldName="Extracted Tooth Number";
					frenchFieldName="Nombre extrait de dent";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					break;
				case "F24"://Not in version 2.
					fieldName="Extraction Date";
					frenchFieldName="Date d'extraction";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "F25"://Not in version 2.
					fieldName="Orthodontic Record Flag";
					frenchFieldName="Drapeau record orthodontique";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(0,1));
					break;
				case "F26"://Not in version 2.
					fieldName="First Examination Fee";
					frenchFieldName="Premiers honoraires d'examen";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "F27"://Not in version 2.
					fieldName="Diagnostic Phase Fee";
					frenchFieldName="Honoraires diagnostiques de phase";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "F28"://Not in version 2.
					fieldName="Initial Payment";
					frenchFieldName="Paiement initial";
					format="D";
					lengthRequirement=new ConstLengthWhenOtherFieldHasValue("F25","1",6);
					break;
				case "F29"://Not in version 2.
					fieldName="Payment Mode";
					frenchFieldName="Mode de paiement";
					format="N";
					lengthRequirement=new ConstLengthWhenOtherFieldHasValue("F25","1",1);
					valueRequirements.Add(new RangeValueRequirement(1,4));
					break;
				case "F30"://Not in version 2.
					fieldName="Treatment Duration";
					frenchFieldName="Durée de traitement";
					format="N";
					lengthRequirement=new ConstLengthWhenOtherFieldHasValue("F25","1",2);
					break;
				case "F31"://Not in version 2.
					fieldName="Number of Anticipated Payments";
					frenchFieldName="Nombre de paiements prévus";
					format="N";
					lengthRequirement=new ConstLengthWhenOtherFieldHasValue("F25","1",2);
					break;
				case "F32"://Not in version 2.
					fieldName="Anticipated Payment Amount";
					frenchFieldName="Quantité prévue de paiement";
					format="D";
					lengthRequirement=new ConstLengthWhenOtherFieldHasValue("F25","1",6);
					break;
				case "F33"://Not in version 2.
					fieldName="Reconciliation Date";
					frenchFieldName="Date de réconciliation";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "F34"://Not in version 2.
					fieldName="Lab Procedure Code # 1";
					frenchFieldName="Code # 1 de procédé de laboratoire";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(5);
					break;
				case "F35"://Not in version 2.
					fieldName="Lab Procedure Code # 2";
					frenchFieldName="Code # 2 de procédé de laboratoire";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(5);
					break;
				case "F36"://Not in version 2.
					fieldName="Lab Procedure Fee # 2";
					frenchFieldName="Honoraires # 2 de procédé de laboratoire";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "F37"://Not in version 2.
					fieldName="Estimated Treatment Start Date";
					frenchFieldName="Date estimée de début de traitement";
					format="N";
					lengthRequirement=new ConstLengthWhenOtherFieldHasValue("F25","1",8);
					break;
				case "F38"://Not in version 2.
					fieldName="Current Reconciliation Page Number";
					frenchFieldName="Numéro de page courant de réconciliation";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(1,9));
					break;
				case "F39"://Not in version 2.
					fieldName="Diagnostic Code";
					frenchFieldName="Code diagnostique";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "F40"://Not in version 2.
					fieldName="Institution Code";
					frenchFieldName="Code d'établissement";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "F41"://Not in version 2.
					fieldName="Original DENTAL OFFICE CLAIM REFERENCE NO";
					frenchFieldName="Nombre d'ordre original d'Office";
					format="N";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "F42"://Not in version 2.
					fieldName="Original Transaction Reference Number";
					frenchFieldName="Numéro de référence original de transaction";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(14);
					break;
				case "F43"://Not in version 2.
					fieldName="Attachment Source";
					frenchFieldName="Source d'attachement";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"I","U"}));
					break;
				case "F44"://Not in version 2.
					fieldName="Attachment Count";
					frenchFieldName="Compte d'attachement";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					valueRequirements.Add(new RangeValueRequirement(1,30));
					break;
				case "F45"://Not in version 2.
					fieldName="Attachment Type";
					frenchFieldName="Type d'attachement";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(3);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"JPG","DIC","TXT","DOC"}));
					break;
				case "F46"://Not in version 2.
					fieldName="Attachment Length";
					frenchFieldName="Longueur d'attachement";
					format="N";
					lengthRequirement=new ConstLengthRequirement(7);
					break;
				case "F47"://Not in version 2.
					fieldName="Attachment";
					frenchFieldName="Attachement";
					format="N";
					lengthRequirement=new LengthFromAnotherField("F46");
					break;
				case "F48"://Not in version 2.
					fieldName="Attachment File Date";
					frenchFieldName="Date de dossier d'attachement";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "F49"://Not in version 2.
					fieldName="Attachment Number";
					frenchFieldName="Nombre d'attachement";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(2);
					break;
				case "G01":
					//Transaction Reference Number
					fieldName="CARRIER CLAIM NO";
					frenchFieldName="NO DE RÉFÉRENCE DE TRANSACTION";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(14);
					break;
				case "G02":
					fieldName="Eligible Amount for Lab Procedure Code #2";
					frenchFieldName="Quantité éligible pour le code #2 de procédé de laboratoire";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "G03":
					fieldName="Expected Payment Date";
					frenchFieldName="Date prévue de paiement";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "G04":
					fieldName="Total Amount of Service";
					frenchFieldName="Montant total de service";
					format="D";
					lengthRequirement=new ConstLengthRequirement(7);
					break;
				case "G05":
					fieldName="Response Status";
					frenchFieldName="Statut de réponse";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string[] {"A","E","R","H","B","C","N","M","X"}));
					break;
				case "G06":
					fieldName="Number of Error Codes";
					frenchFieldName="Nombre de codes d'erreur";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					valueRequirements.Add(new RangeValueRequirement(0,10));
					break;
				case "G07":
					//Disposition message
					fieldName="DISPOSITION";
					frenchFieldName="SPÉCIFICATIONS";
					format="AE/N";
					lengthRequirement=new ConstLengthRequirement(75);
					break;
				case "G08":
					fieldName="Error Code";
					frenchFieldName="Code d'erreur";
					format="N";
					lengthRequirement=new ConstLengthRequirement(3);
					break;
				//case "G09": // Does not exist
					//break;
				case "G10":
					fieldName="Number of Carrier Issued Procedure Codes";
					frenchFieldName="Le nombre de porteur a publié des codes de procédé";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(0,6));
					break;
				case "G11":
					fieldName="Number of Note Lines";
					frenchFieldName="Nombre de lignes de note";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					valueRequirements.Add(new RangeValueRequirement(0,32));
					break;
				case "G12":
					fieldName="Eligible Amount";
					frenchFieldName="Quantité éligible";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "G13":
					fieldName="Deductible Amount";
					frenchFieldName="Quantité déductible";
					format="D";
					lengthRequirement=new ConstLengthRequirement(5);
					break;
				case "G14":
					fieldName="Eligible Percentage";
					frenchFieldName="Pourcentage éligible";
					format="N";
					lengthRequirement=new ConstLengthRequirement(3);
					valueRequirements.Add(new RangeValueRequirement(0,100));
					break;
				case "G15":
					fieldName="Benefit Amount for the Procedure";
					frenchFieldName="Quantité d'avantage pour le procédé";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "G16":
					fieldName="Explanation Note Number 1";
					frenchFieldName="Note numéro 1 d'explication";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					break;
				case "G17":
					fieldName="Explanation Note Number 2";
					frenchFieldName="Note numéro 2 d'explication";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					break;
				case "G18":
					fieldName="Reference to Line Number of the Submitted Procedure";
					frenchFieldName="Référence à la ligne nombre du procédé soumis";
					format="N";
					lengthRequirement=new ConstLengthRequirement(7);
					valueRequirements.Add(new RegexValueRequirement("^[0-7]+$"));
					break;
				case "G19":
					fieldName="Additional Procedure Code";
					frenchFieldName="Code additionnel de procédé";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(5);
					break;
				case "G20":
					fieldName="Eligible Amount for the Additional Procedure";
					frenchFieldName="Quantité éligible pour le procédé additionnel";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "G21":
					fieldName="Dedutible for the Additional Procedure";
					frenchFieldName="";
					format="D";
					lengthRequirement=new ConstLengthRequirement(5);
					break;
				case "G22":
					fieldName="Eligible Percentage";
					frenchFieldName="Pourcentage éligible";
					format="N";
					lengthRequirement=new ConstLengthRequirement(3);
					valueRequirements.Add(new RangeValueRequirement(0,100));
					break;
				case "G23":
					fieldName="Benefit Amount for the Additional Procedure";
					frenchFieldName="Quantité d'avantage pour le procédé additionnel";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "G24":
					fieldName="Explanation Note Number 1 for the Additional Procedure";
					frenchFieldName="Note d'explication numéro 1 pour le procédé additionnel";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					break;
				case "G25":
					fieldName="Explanation Note Number 2 for the Additional Procedure";
					frenchFieldName="Note d'explication numéro 2 pour le procédé additionnel";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					break;
				case "G26":
					fieldName="Note Text";
					frenchFieldName="Noter le texte";
					format="AE/N";
					lengthRequirement=new ConstLengthRequirement(75);
					break;
				case "G27":
					fieldName="Language of the Insured";
					frenchFieldName="Langue des assurés";
					format="A";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new DiscreteValueRequirement(new string [3] {"A","E","F"}));
					break;
				case "G28":
					fieldName="Total Benefit Amount";
					frenchFieldName="Quantité totale d'avantage";
					format="D";
					lengthRequirement=new ConstLengthRequirement(7);
					break;
				case "G29":
					fieldName="Deductible amount unallocated";
					frenchFieldName="La quantité déductible a désassigné";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "G30":
					fieldName="VERIFICATION NO";
					frenchFieldName="CODE DE VALIDATION";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(10);
					break;
				case "G31"://Not in version 2.
					fieldName="Display Message Count";
					frenchFieldName="Compte de message d'affichage";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					valueRequirements.Add(new RangeValueRequirement(0,40));
					break;
				case "G32"://Not in version 2.
					fieldName="Display Message";
					frenchFieldName="Message d'affichage";
					format="AE/N";
					lengthRequirement=new ConstLengthRequirement(75);
					break;
				case "G33"://Not in version 2.
					fieldName="PAYMENT ADJUSTMENT AMOUNT";
					frenchFieldName="MONTANT D'ADAPTATION DE PAIEMENT";
					format="D";
					lengthRequirement=new ConstLengthRequirement(7);
					break;
				case "G34"://Not in version 2.
					fieldName="PAYMENT REFERENCE";
					frenchFieldName="RÉFÉRENCE DE PAIEMENT";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(10);
					break;
				case "G35"://Not in version 2.
					fieldName="PAYMENT DATE";
					frenchFieldName="DATE DE PAIEMENT";
					format="N";
					lengthRequirement=new ConstLengthRequirement(8);
					break;
				case "G36"://Not in version 2.
					fieldName="PAYMENT AMOUNT";
					frenchFieldName="QUANTITÉ DE PAIEMENT";
					format="D";
					lengthRequirement=new ConstLengthRequirement(7);
					break;
				case "G37"://Not in version 2.
					fieldName="Payment Detail Count";
					frenchFieldName="Compte de détail de paiement";
					format="N";
					lengthRequirement=new ConstLengthRequirement(3);
					valueRequirements.Add(new RangeValueRequirement(0,250));
					break;
				case "G38"://Not in version 2.
					fieldName="Transaction Payment";
					frenchFieldName="Paiement de transaction";
					format="D";
					lengthRequirement=new ConstLengthRequirement(7);
					break;
				case "G39"://Not in version 2.
					fieldName="Embedded Transaction Length";
					frenchFieldName="Longueur incluse de transaction";
					format="N";
					lengthRequirement=new ConstLengthRequirement(4);
					break;
				case "G40"://Not in version 2.
					fieldName="Embedded Transaction";
					frenchFieldName="Transaction incluse";
					format="AE/N";
					lengthRequirement=new LengthFromAnotherField("G39");
					break;
				case "G41"://Not in version 2.
					fieldName="Message Output Flag";
					frenchFieldName="Drapeau de rendement de message";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(0,2));
					break;
				case "G42"://Not in version 2.
					fieldName="Form ID";
					frenchFieldName="Former l'identification";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					valueRequirements.Add(new RangeValueRequirement(1,8));
					break;
				case "G43"://Not in version 2.
					fieldName="Eligible Amount for Lab Procedure Code # 1";
					frenchFieldName="Quantité éligible pour le code # 1 de procédé de laboratoire";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "G44"://Not in version 2.
					fieldName="Eligible Lab Amount for the Additional Procedure";
					frenchFieldName="Quantité éligible de laboratoire pour le procédé additionnel";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "G45"://Not in version 2.
					fieldName="Note Number";
					frenchFieldName="Noter le nombre";
					format="N";
					lengthRequirement=new ConstLengthRequirement(3);
					break;
				case "G46"://Not in version 2.
					fieldName="Current Predetermination Page Number";
					frenchFieldName="Numéro de page courant de prédétermination";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					break;
				case "G47"://Not in version 2.
					fieldName="Last Predetermination Page Number";
					frenchFieldName="Numéro de page courant de prédétermination";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					break;
				case "G48"://Not in version 2.
					fieldName="E-Mail Office Number";
					frenchFieldName="Nombre d'Office d'E-mail";
					format="N";
					lengthRequirement=new ConstLengthRequirement(4);
					break;
				case "G49"://Not in version 2.
					//E-mail to
					fieldName="TO";
					frenchFieldName="DESTINATAIRE";
					format="AE/N";
					lengthRequirement=new ConstLengthRequirement(60);
					break;
				case "G50"://Not in version 2.
					//E-mail from
					fieldName="FROM";
					frenchFieldName="EXPÉDITEUR";
					format="AE/N";
					lengthRequirement=new ConstLengthRequirement(60);
					break;
				case "G51"://Not in version 2.
					fieldName="SUBJECT";
					frenchFieldName="OBJET";
					format="AE/N";
					lengthRequirement=new ConstLengthRequirement(60);
					break;
				case "G52"://Not in version 2.
					fieldName="Number of E-mail Note Lines";
					frenchFieldName="Nombre de lignes de note d'E-mail";
					format="N";
					lengthRequirement=new ConstLengthRequirement(2);
					valueRequirements.Add(new RangeValueRequirement(1,50));
					break;
				case "G53"://Not in version 2.
					fieldName="E-Mail Note Line";
					frenchFieldName="Ligne de note d'E-mail";
					format="AE/N";
					lengthRequirement=new ConstLengthRequirement(75);
					break;
				case "G54"://Not in version 2.
					//Email reference number
					fieldName="REFERENCE";
					frenchFieldName="RÉFÉRENCE";
					format="A/N";
					lengthRequirement=new ConstLengthRequirement(10);
					break;
				case "G55"://Not in version 2.
					fieldName="Total Payable";
					frenchFieldName="Payable total";
					format="D";
					lengthRequirement=new ConstLengthRequirement(7);
					break;
				case "G56"://Not in version 2.
					fieldName="Deductible Amount for Lab Procedure Code # 1";
					frenchFieldName="Quantité déductible pour le code # 1 de procédé de laboratoire";
					format="D";
					lengthRequirement=new ConstLengthRequirement(5);
					break;
				case "G57"://Not in version 2.
					fieldName="Eligible Percentage for Lab Procedure # 1";
					frenchFieldName="Pourcentage éligible pour le procédé # 1 de laboratoire";
					format="N";
					lengthRequirement=new ConstLengthRequirement(3);
					break;
				case "G58"://Not in version 2.
					fieldName="Benefit Amount for Lab Procedure Code #1";
					frenchFieldName="Quantité d'avantage pour le code #1 de procédé de laboratoire";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "G59"://Not in version 2.
					fieldName="Deductible Amount for Lab Procedure Code # 2";
					frenchFieldName="Quantité déductible pour le code # 2 de procédé de laboratoire";
					format="D";
					lengthRequirement=new ConstLengthRequirement(5);
					break;
				case "G60"://Not in version 2.
					fieldName="Eligible Percentage for Lab Procedure Code # 2";
					frenchFieldName="Pourcentage éligible pour le code # 2 de procédé de laboratoire";
					format="N";
					lengthRequirement=new ConstLengthRequirement(3);
					break;
				case "G61"://Not in version 2.
					fieldName="Benefit Amount for Lab Procedure Code # 2";
					frenchFieldName="Bénéficier la quantité pour le code # 2 de procédé de laboratoire";
					format="D";
					lengthRequirement=new ConstLengthRequirement(6);
					break;
				case "G62"://Not in version 2.
					fieldName="Last Reconciliation Page Number";
					frenchFieldName="Dernier numéro de page de réconciliation";
					format="N";
					lengthRequirement=new ConstLengthRequirement(1);
					valueRequirements.Add(new RangeValueRequirement(1,9));
					break;
				default:
					MessageBox.Show("Internal Error, unknown version 4 CCD field ID during construction: "+pFieldId);
					return;//error
			}
		}

	}
}