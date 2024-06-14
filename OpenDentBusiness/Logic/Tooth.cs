using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness{
	/// <summary></summary>
	public class Tooth{
		///<summary></summary>
		public Tooth(){
			
		}

		#region Fields
		private static List<string> _listUniversal = new List<string> { "1",  "2",  "3",  "4",  "5",  "6",  "7",  "8",  "9", "10", "11", "12", "13", "14", "15", "16", 
																"32", "31", "30", "29", "28", "27", "26", "25", "24", "23", "22", "21", "20", "19", "18", "17",
																                   "A",  "B",  "C",  "D",  "E",  "F",  "G",  "H",  "I",  "J",
																				   "T",  "S",  "R",  "Q",  "P",  "O",  "N",  "M",  "L",  "K"
																};

		private static List<string> _listFDI = new List<string> {"18", "17", "16", "15", "14", "13", "12", "11", "21", "22", "23", "24", "25", "26", "27", "28", 
																"48", "47", "46", "45", "44", "43", "42", "41", "31", "32", "33", "34", "35", "36", "37", "38",
																                  "55", "54", "53", "52", "51", "61", "62", "63", "64", "65",
																				  "85", "84", "83", "82", "81", "71", "72", "73", "74", "75"
																};

		private static List<string> _listHaderup = new List<string> { "8+",  "7+",  "6+",  "5+",  "4+",  "3+",  "2+",  "1+",  "+1", "+2", "+3", "+4", "+5", "+6", "+7", "+8", 
																 "8-",  "7-",  "6-",  "5-",  "4-",  "3-",  "2-",  "1-",  "-1", "-2", "-3", "-4", "-5", "-6", "-7", "-8", 
																                   "A",  "B",  "C",  "D",  "E",  "F",  "G",  "H",  "I",  "J",
																				   "T",  "S",  "R",  "Q",  "P",  "O",  "N",  "M",  "L",  "K"
																};

		private static List<string> _listPalmer = new List<string> {
			"UR8","UR7","UR6","UR5","UR4","UR3","UR2","UR1","UL1","UL2","UL3","UL4","UL5","UL6","UL7","UL8",
			"LR8","LR7","LR6","LR5","LR4","LR3","LR2","LR1","LL1","LL2","LL3","LL4","LL5","LL6","LL7","LL8",
			"URE","URD","URC","URB","URA","ULA","ULB","ULC","ULD","ULE",
			"LRE","LRD","LRC","LRB","LRA","LLA","LLB","LLC","LLD","LLE"};

		private static List<string> _listPalmerSimple = new List<string> {
			"8","7","6","5","4","3","2","1","1","2","3","4","5","6","7","8",
			"8","7","6","5","4","3","2","1","1","2","3","4","5","6","7","8",
			"E","D","C","B","A","A","B","C","D","E",
			"E","D","C","B","A","A","B","C","D","E"};
		#endregion Fields

		#region Methods - Areas
		///<summary></summary>
		public static bool IsAnterior(string toothNum) {
			if(!IsValidDB(toothNum)) {
				return false;
			}
			int intTooth=ToInt(toothNum);
			if(intTooth>=6 && intTooth<=11) {
				return true;
			}
			if(intTooth>=22 && intTooth<=27) {
				return true;
			}
			return false;
		}

		///<summary></summary>
		public static bool IsAnterior(int intTooth){
			string toothNum=FromInt(intTooth);
			return IsAnterior(toothNum);
		}

		///<summary></summary>
		public static bool IsPosterior(string toothNum){
			if(!IsValidDB(toothNum)){
				return false;
			}
			int intTooth=ToInt(toothNum);
			if(intTooth>=1 && intTooth<=5){
				return true;
			}
			if(intTooth>=12 && intTooth<=21){
				return true;
			}
			if(intTooth>=28 && intTooth<=32){
				return true;
			}
			return false;
		}

		///<summary></summary>
		public static bool IsPosterior(int intTooth){
			string toothNum=FromInt(intTooth);
			return IsPosterior(toothNum);
		}

		///<summary></summary>
		public static bool IsMaxillary(int intTooth){
			string toothNum=FromInt(intTooth);
			return IsMaxillary(toothNum);
		}

		///<summary>Primary, permanent, and supernumerary tooth_ids are all accepted.</summary>
		public static bool IsMaxillary(string toothNum) {
			if(!IsValidDB(toothNum)) {
				return false;
			}
			int intTooth=ToInt(toothNum);
			if(intTooth>=1 && intTooth<=16) {
				return true;
			}
			return false;
		}

		///<summary>toothNum gets validated here.</summary>
		public static bool IsMolar(string toothNum){
			if(!IsValidDB(toothNum)) {
				return false;
			}
			int intTooth=ToInt(toothNum);
			if(IsPrimary(toothNum)) {
				if(intTooth<=5 || intTooth>=28) {//AB, ST
			    return true;
			  }
			  if(intTooth>=12 && intTooth<=21) {//IJ, KL 
			    return true;
			  }
			  return false;
			}
			if(intTooth>=1 && intTooth<=3) {
				return true;
			}
			if(intTooth>=14 && intTooth<=19) {
				return true;
			}
			if(intTooth>=30 && intTooth<=32) {
				return true;
			}
			return false;
		}

		///<summary></summary>
		public static bool IsMolar(int intTooth){
			string toothNum=FromInt(intTooth);
			return IsMolar(toothNum);
		}

		///<summary>toothNum gets validated here. Used for FGC insurance substitutions.</summary>
		public static bool IsSecondMolar(string toothNum) {
			if(!IsValidDB(toothNum)){
				return false;
			}
			int intTooth=ToInt(toothNum);
			if(intTooth==2 || intTooth==15 || intTooth==18 || intTooth==31){
				return true;
			}
			return false;
		}

		///<summary></summary>
		public static bool IsPreMolar(string toothNum){
			if(!IsValidDB(toothNum)){
				return false;
			}
			int intTooth=ToInt(toothNum);
			if(intTooth==4 
				|| intTooth==5
				|| intTooth==12
				|| intTooth==13
				|| intTooth==20
				|| intTooth==21
				|| intTooth==28
				|| intTooth==29) 
			{
				return true;
			}
			return false;
		}

		///<summary></summary>
		public static bool IsPreMolar(int intTooth){
			string toothNum=FromInt(intTooth);
			return IsPreMolar(toothNum);
		}
		#endregion Methods - Areas

		#region Methods - Conversion to/from Db
		///<summary>Used every time user enters tooth number in a procedure box. Follow with Parse. These are the *ONLY* methods that are designed to accept user input.  Can also handle international toothnum.</summary>
		public static bool IsValidEntry(string toothLabel){
			ToothNumberingNomenclature toothNumberingNomenclature = (ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			return IsValidEntry(toothLabel,toothNumberingNomenclature);
		}

		///<summary>Used every time user enters tooth number in a procedure box. Follow with Parse. These are the *ONLY* methods that are designed to accept user input.  Can also handle international toothnum.</summary>
		public static bool IsValidEntry(string toothLabel,ToothNumberingNomenclature toothNumberingNomenclature){
			if(toothNumberingNomenclature==ToothNumberingNomenclature.Universal){//American
				//tooth numbers validated the same as they are in db.
				return IsValidDB(toothLabel);
			}
			if(toothNumberingNomenclature==ToothNumberingNomenclature.FDI){
				if(toothLabel==null || toothLabel==""){
					return false;
				}
				if(Regex.IsMatch(toothLabel,"^[1-4][1-8]$")){//perm teeth: matches firt digit 1-4 and second digit 1-8,9 would be supernumerary?
					return true;
				}
				if(Regex.IsMatch(toothLabel,"^[5-8][1-5]$")){//pri teeth: matches firt digit 5-8 and second digit 1-5
					return true;
				}
				if(toothLabel=="99") {//supernumerary tooth: It is documented in the cdha website that 99 is the only valid number for supernumerary teeth.
					return true;
				}
				return false;
			}
			if(toothNumberingNomenclature==ToothNumberingNomenclature.Haderup){
				if(toothLabel==null || toothLabel=="") {
					return false;
				}
				for(int i=0;i<_listHaderup.Count;i++) {
					if(_listHaderup[i]==toothLabel) {
						return true;
					}
				}
				return false;
			}
			// Palmer
			if(toothLabel==null || toothLabel=="") {
				return false;
			}
			for(int i=0;i<_listPalmer.Count;i++) {
				if(_listPalmer[i]==toothLabel) {
					return true;
				}
			}
			return false;
		}

		///<summary>Intended to validate toothNum coming in from database. Will not handle any international tooth nums since all database teeth are in US format.</summary>
		public static bool IsValidDB(string toothNum){
			if(toothNum==null || toothNum==""){
				return false;
			}
			if(Regex.IsMatch(toothNum,"^[A-T]$")){
				return true;
			}
			if(Regex.IsMatch(toothNum,"^[A-T]S$")){//supernumerary
				return true;
			}
			if(!Regex.IsMatch(toothNum,@"^[1-9]\d?$")){//matches 1 or 2 digits, leading 0 not allowed
				return false;
			}
			int intTooth=Convert.ToInt32(toothNum);
			if(intTooth<=32) {
				return true;
			}
			if(intTooth>=51 && intTooth<=82) {//supernumerary
				return true;
			}
			return false;
		}

		///<summary>Sometimes validated by IsValidDB before coming here. If invalid, will display dash. This should be used to displayed any tooth numbers. It will handle checking for whether user is using international tooth numbers.  All tooth numbers are passed around in American values until the very last moment.  Just before display, the string is converted using this method.</summary>
		public static string Display(string toothNum,ToothNumberingNomenclature toothNumberingNomenclature) {
			if(toothNum==null || toothNum==""){
				return ""; 
			}
			if(toothNumberingNomenclature == ToothNumberingNomenclature.Universal) {
				return toothNum; 
			}
			int index = _listUniversal.IndexOf(toothNum);
			if(index==-1){
				if(toothNumberingNomenclature == ToothNumberingNomenclature.FDI
					&& toothNum=="51")
				{
					return "99";//supernumerary tooth: It is documented in the cdha website that 99 is the only valid number for supernumerary teeth.
				}
				return "-";
			}
			if(toothNumberingNomenclature == ToothNumberingNomenclature.FDI) {
				return _listFDI[index];
			}
			else if(toothNumberingNomenclature == ToothNumberingNomenclature.Haderup) { 
				return _listHaderup[index];
			}
			else if(toothNumberingNomenclature == ToothNumberingNomenclature.Palmer) { 
				return _listPalmer[index];
			}
			return "-"; // Should never happen
		}

		public static string Display(string toothNum) {
			return Display(toothNum,(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
		}

		///<summary>Identical to Display, but just used in the 3D tooth chart because with Palmer, we don't want the UR, UL, etc.</summary>
		public static string DisplayGraphic(string toothNum,ToothNumberingNomenclature toothNumberingNomenclature){
			if(toothNum==null || toothNum==""){
				return ""; 
			}
			if(toothNumberingNomenclature==ToothNumberingNomenclature.Universal) {
				return toothNum;
			}
			int index = _listUniversal.IndexOf(toothNum);
			if(index==-1){
				return "-";
			}
			if(toothNumberingNomenclature == ToothNumberingNomenclature.FDI) {
				return _listFDI[index];
			}
			else if(toothNumberingNomenclature == ToothNumberingNomenclature.Haderup) { 
				return _listHaderup[index];
			}
			else if(toothNumberingNomenclature == ToothNumberingNomenclature.Palmer) {
				return _listPalmerSimple[index];
			}
			return "-"; // Should never happen
		}

		public static string GetDisplayGraphic(string toothNum) {
			return DisplayGraphic(toothNum,(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
		}

		///<summary>MUST be validated by IsValidEntry before coming here. All user entered toothnumbers are run through this method which automatically checks to see if using international toothnumbers. So the procedurelog class will always contain the American toothnum.</summary>
		public static string Parse(string toothLabel) {
			ToothNumberingNomenclature toothNumberingNomenclature=(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			return Parse(toothLabel,toothNumberingNomenclature);
		}

		///<summary>MUST be validated by IsValidEntry before coming here.</summary>
		public static string Parse(string toothLabel,ToothNumberingNomenclature toothNumberingNomenclature) {
			if(toothNumberingNomenclature == ToothNumberingNomenclature.Universal) {
				return toothLabel;
			}
			int index = 0;
			if(toothNumberingNomenclature == ToothNumberingNomenclature.FDI) {
				if(toothLabel=="99") {
					return "51";//supernumerary tooth: It is documented in the cdha website that 99 is the only valid number for supernumerary teeth.
				}
				index = _listFDI.IndexOf(toothLabel);
			}
			else if(toothNumberingNomenclature == ToothNumberingNomenclature.Haderup) { 
				index = _listHaderup.IndexOf(toothLabel);
			}
			else if(toothNumberingNomenclature == ToothNumberingNomenclature.Palmer) { 
				index = _listPalmer.IndexOf(toothLabel);
			}
			return _listUniversal[index];
		}
		#endregion Methods - Conversion to/from Db

		#region Methods - Groupings
		///<summary>Get quadrant returns "UR" for teeth 1-8, "LR" for 25-32, "UL" for 9-16, and "LL" for 17-24.</summary>
		public static string GetQuadrant(string toothNum){
			if(!IsValidDB(toothNum)){
				return "";
			}
			int intTooth=ToInt(toothNum);
			if(intTooth>=1 && intTooth<=8){
				return "UR";
			}
			if(intTooth>=9 && intTooth<=16){
				return "UL";
			}
			if(intTooth>=17 && intTooth<=24){
				return "LL";
			}
			if(intTooth>=25 && intTooth<=32){
				return "LR";
			}
			return "";
		}

		/// <summary>Returns string list of distinct quadrants that contain the specified teeth. If no teeth are selected, it will return an empty list.</summary>
		public static List<string> GetQuadsForTeeth(List<string> listToothNums) {
			if(listToothNums.Count==0) {
				return new List<string>();
			}
			List<string> listQuads=new List<string>();
			List<string> listToothNumsDistinct=listToothNums.Distinct().ToList();
			for(int i=0;i<listToothNumsDistinct.Count();i++) {
				string quad=GetQuadrant(listToothNumsDistinct[i]);
				if(quad!="") {
					listQuads.Add(quad);
				}
			}
			//Return distinct list just in case we added a tooth twice while looping.
			return listQuads.Distinct().ToList();
		}

		/// <summary>Returns a string list of distinct arches that contain the specified teeth. So either U or L or U,L. If no teeth are selected, it will return an empty list.</summary>
		public static List<string> GetArchesForTeeth(List<string> listToothNums) {
			if(listToothNums.Count==0) {
				return new List<string>();
			}
			List<string> listArches=new List<string>();
			if(listToothNums.Where(x => IsMaxillary(x)).Count()>0) {
				listArches.Add("U");
			}
			if(listToothNums.Where(x => IsValidDB(x) && !IsMaxillary(x)).Count()>0) {
				listArches.Add("L");
			}
			return listArches;
		}

		///<summary>For nomenclature, use pref UseInternationalToothNumbers, or a hard coded value.</summary>
		public static string GetSextant(string surf,ToothNumberingNomenclature toothNumberingNomenclature) {
			if(toothNumberingNomenclature==ToothNumberingNomenclature.Universal) {
				return surf;
			}
			if(toothNumberingNomenclature==ToothNumberingNomenclature.FDI) {
				if(surf=="1") {//Upper Right
					return "03";
				}
				else if(surf=="2") {//Upper Anterior
					return "04";
				}
				else if(surf=="3") {//Upper Left
					return "05";
				}
				else if(surf=="4") {//Lower Left
					return "06";
				}
				else if(surf=="5") {//Lower Anterior
					return "07";
				}
				else if(surf=="6") {//Lower Right
					return "08";
				}
				return "00";//Invalid or unspecified sextant.  This is also the number that represents "whole mouth" in Canada.
			}
			//Other nomenclatures not yet implemented
			return surf;
		}
		#endregion Methods - Groupings

		#region Methods - Ranges
		///<summary>The supplied toothNumbers will be a series of tooth numbers separated by commas.  They will be in American format.  For display purposes, ranges will use dashes, and international numbers will be used.</summary>
		public static string DisplayRange(string toothNumbers) {
			if(toothNumbers==null) {
				return "";
			}
			toothNumbers=toothNumbers.Replace(" ","");//remove all spaces
			if(toothNumbers=="") {
				return "";
			}
			List<string> listTeeth=toothNumbers.Split(',').ToList();
			if(listTeeth.Count==1) {
				return Tooth.Display(listTeeth[0]);
			}
			else if(listTeeth.Count==2) {
				return Tooth.Display(listTeeth[0])+","+Tooth.Display(listTeeth[1]);//just two numbers separated by comma
			}
			listTeeth.Sort(new ToothComparer());
			StringBuilder stringBuilder=new StringBuilder();
			//List<string> toothList=new List<string>();
			//strbuild.Append(Tooth.ToInternat(toothArray[0]));//always show the first number
			int intToothCurrent;
			int intToothNext;
			int numberInaRow=1;//must have 3 in a row to trigger dash
			for(int i=0;i<listTeeth.Count-1;i++) {
				//in each loop, we are comparing the current number with the next number
				intToothCurrent=Tooth.ToOrdinal(listTeeth[i]);
				intToothNext=Tooth.ToOrdinal(listTeeth[i+1]);
				if(intToothNext-intToothCurrent==1 && intToothCurrent!=16 && intToothCurrent!=32) {//if sequential (sequences always break at end of arch)
					numberInaRow++;
				}
				else {
					numberInaRow=1;
				}
				if(numberInaRow<3) {//the next number is not sequential,or if it was a sequence, and it's now broken
					if(stringBuilder.Length>0 && stringBuilder[stringBuilder.Length-1]!='-') {
						stringBuilder.Append(",");
					}
					stringBuilder.Append(Tooth.Display(listTeeth[i]));
				}
				else if(numberInaRow==3) {//this way, the dash only gets added exactly once
					stringBuilder.Append("-");
				}
				//else do nothing
			}
			if(stringBuilder.Length>0 && stringBuilder[stringBuilder.Length-1]!='-') {
				stringBuilder.Append(",");
			}
			stringBuilder.Append(Tooth.Display(listTeeth[listTeeth.Count-1]));//always show the last number
			return stringBuilder.ToString();
		}

		///<summary>Takes a user entered string and validates/formats it for the database.  Throws an ApplicationException if any formatting errors.  User string can contain spaces, dashes, and commas, too.</summary>
		public static string ParseRange(string toothNumbers){
			if(toothNumbers==null) {
				return "";
			}
			toothNumbers=toothNumbers.Replace(" ","");//remove all spaces
			if(toothNumbers=="") {
				return "";
			}
			List<string> listToothRanges=toothNumbers.Split(',').ToList();//some items will contain dashes
			List<string> listTeeth=new List<string>();
			string rangeBegin;
			string rangeEnd;
			int intBegin;
			int intEnd;
			//not sure how to handle supernumerary.  Probably just not acceptable.
			for(int i=0;i<listToothRanges.Count;i++){
				if(listToothRanges[i].Contains("-")){
					rangeBegin=listToothRanges[i].Split('-')[0].ToUpper();
					rangeEnd=listToothRanges[i].Split('-')[1].ToUpper();
					if(!IsValidEntry(rangeBegin)) {
						throw new ApplicationException(rangeBegin+" "+Lans.g("Tooth","is not a valid tooth number."));
					}
					if(!IsValidEntry(rangeEnd)) {
						throw new ApplicationException(rangeEnd+" "+Lans.g("Tooth","is not a valid tooth number."));
					}
					intBegin=Tooth.ToOrdinal(Parse(rangeBegin));
					intEnd=Tooth.ToInt(Parse(rangeEnd));
					if(intEnd<intBegin){
						throw new ApplicationException("Range specified is impossible.");
					}
					while(true){
						if(intBegin>intEnd){
							break;
						}
						listTeeth.Add(Tooth.FromOrdinal(intBegin));
						intBegin++;
					}
				}
				else{
					string toothNum=listToothRanges[i].ToUpper();
					if(!IsValidEntry(toothNum)){
						throw new ApplicationException(toothNum+" "+Lans.g("Tooth","is not a valid tooth number."));
					}
					listTeeth.Add(Parse(toothNum));
				}
			}
			listTeeth.Sort(new ToothComparer());
			string teethRetVal="";
			for(int i=0;i<listTeeth.Count;i++){
				if(i>0){
					teethRetVal+=",";
				}
				teethRetVal+=listTeeth[i];
			}
			return teethRetVal;
		}
		#endregion Methods - Ranges

		#region Methods - Supernumerary
		///<summary></summary>
		public static bool IsSuperNum(string toothNum){
			if(toothNum==null || toothNum==""){
				return false;
			}
			if(Regex.IsMatch(toothNum,"^[A-T]$")){
				return false;
			}
			if(Regex.IsMatch(toothNum,"^[A-T]S$")){//supernumerary
				return true;
			}
			if(!Regex.IsMatch(toothNum,@"^[1-9]\d?$")){//matches 1 or 2 digits, leading 0 not allowed
				return false;
			}
			int intTooth=Convert.ToInt32(toothNum);
			if(intTooth<=32) {
				return false;
			}
			if(intTooth>=51 && intTooth<=82) {//supernumerary
				return true;	
			}
			return false;
		}

		///<summary>Converts supernumerary teeth to permanent.</summary>
		public static string SupToPerm(string toothNum) {
			switch(toothNum) {
				default: return "";
				case "51": return "1";
				case "52": return "2";
				case "53": return "3";
				case "54": return "4";
				case "55": return "5";
				case "56": return "6";
				case "57": return "7";
				case "58": return "8";
				case "59": return "9";
				case "60": return "10";
				case "61": return "11";
				case "62": return "12";
				case "63": return "13";
				case "64": return "14";
				case "65": return "15";
				case "66": return "16";
				case "67": return "17";
				case "68": return "18";
				case "69": return "19";
				case "70": return "20";
				case "71": return "21";
				case "72": return "22";
				case "73": return "23";
				case "74": return "24";
				case "75": return "25";
				case "76": return "26";
				case "77": return "27";
				case "78": return "28";
				case "79": return "29";
				case "80": return "30";
				case "81": return "31";
				case "82": return "32";
				case "AS": return "4";
				case "BS": return "5";
				case "CS": return "6";
				case "DS": return "7";
				case "ES": return "8";
				case "FS": return "9";
				case "GS": return "10";
				case "HS": return "11";
				case "IS": return "12";
				case "JS": return "13";
				case "KS": return "20";
				case "LS": return "21";
				case "MS": return "22";
				case "NS": return "23";
				case "OS": return "24";
				case "PS": return "25";
				case "QS": return "26";
				case "RS": return "27";
				case "SS": return "28";
				case "TS": return "29";
			}
		}
		#endregion Methods - Supernumerary

		#region Methods - Primary
		///<summary>Returns true if A-T or AS-TS.  Otherwise, returns false.</summary>
		public static bool IsPrimary(string toothNum) {
			if(string.IsNullOrEmpty(toothNum)) {
				return false;
			}
			if(Regex.IsMatch(toothNum,"^[A-T]$")) {
				return true;
			}
			if(Regex.IsMatch(toothNum,"^[A-T]S$")) {
				return true;
			}
			return false;
		}

		///<summary></summary>
		public static string PermToPri(string toothNum) {
			switch(toothNum) {
				default: return "";
				case "4": return "A";
				case "5": return "B";
				case "6": return "C";
				case "7": return "D";
				case "8": return "E";
				case "9": return "F";
				case "10": return "G";
				case "11": return "H";
				case "12": return "I";
				case "13": return "J";
				case "20": return "K";
				case "21": return "L";
				case "22": return "M";
				case "23": return "N";
				case "24": return "O";
				case "25": return "P";
				case "26": return "Q";
				case "27": return "R";
				case "28": return "S";
				case "29": return "T";
				case "54": return "AS";
				case "55": return "BS";
				case "56": return "CS";
				case "57": return "DS";
				case "58": return "ES";
				case "59": return "FS";
				case "60": return "GS";
				case "61": return "HS";
				case "62": return "IS";
				case "63": return "JS";
				case "70": return "KS";
				case "71": return "LS";
				case "72": return "MS";
				case "73": return "NS";
				case "74": return "OS";
				case "75": return "PS";
				case "76": return "QS";
				case "77": return "RS";
				case "78": return "SS";
				case "79": return "TS";
			}
		}

		///<summary></summary>
		public static string PermToPri(int intTooth){
			string toothNum=FromInt(intTooth);
			return PermToPri(toothNum);
		}

		///<summary></summary>
		public static string PriToPerm(string toothNum) {
			switch(toothNum) {
				default: return "";
				case "A": return "4";
				case "B": return "5";
				case "C": return "6";
				case "D": return "7";
				case "E": return "8";
				case "F": return "9";
				case "G": return "10";
				case "H": return "11";
				case "I": return "12";
				case "J": return "13";
				case "K": return "20";
				case "L": return "21";
				case "M": return "22";
				case "N": return "23";
				case "O": return "24";
				case "P": return "25";
				case "Q": return "26";
				case "R": return "27";
				case "S": return "28";
				case "T": return "29";
				case "AS": return "4";
				case "BS": return "5";
				case "CS": return "6";
				case "DS": return "7";
				case "ES": return "8";
				case "FS": return "9";
				case "GS": return "10";
				case "HS": return "11";
				case "IS": return "12";
				case "JS": return "13";
				case "KS": return "20";
				case "LS": return "21";
				case "MS": return "22";
				case "NS": return "23";
				case "OS": return "24";
				case "PS": return "25";
				case "QS": return "26";
				case "RS": return "27";
				case "SS": return "28";
				case "TS": return "29";
			}
		}
		#endregion Methods - Primary

		#region Methods - Ordinals and Ints
		///<summary>Used to put perm and pri into a single array.  1-32 is perm.  33-52 is pri.</summary>
		public static int ToOrdinal(string toothNum){
			if(IsPrimary(toothNum)){
				switch(toothNum){
					default: return -1;
					case "A": return 33;
					case "B": return 34;
					case "C": return 35;
					case "D": return 36;
					case "E": return 37;
					case "F": return 38;
					case "G": return 39;
					case "H": return 40;
					case "I": return 41;
					case "J": return 42;
					case "K": return 43;
					case "L": return 44;
					case "M": return 45;
					case "N": return 46;
					case "O": return 47;
					case "P": return 48;
					case "Q": return 49;
					case "R": return 50;
					case "S": return 51;
					case "T": return 52;
				}
			}
			else{//perm
				return ToInt(toothNum);
			}
		}

		///<summary>Assumes ordinal is valid.</summary>
		public static string FromOrdinal(int ordinal){
			if(ordinal<1 || ordinal>52){
				return "1";//just so it won't crash.
			}
			if(ordinal<33){
				return ordinal.ToString();
			}
			if(ordinal<43){
				return Tooth.PermToPri(ordinal-29);
			}
			return Tooth.PermToPri(ordinal-23);
		}

		///<summary>Returns 1-32, or -1.  The toothNum should be validated before coming here, but it won't crash if invalid.  Primary or perm are ok.  Empty and null are also ok.  Supernumerary are also ok.</summary>
		public static int ToInt(string toothNum){
			if(toothNum==null || toothNum=="") {
				return -1;
			}
			if(IsPrimary(toothNum)) {
				toothNum=PriToPerm(toothNum);
			}
			else if(IsSuperNum(toothNum)) {
				toothNum=SupToPerm(toothNum);
			}
			try{
				return Convert.ToInt32(toothNum);
			}
			catch{
				return -1;
			}
		}

		///<summary></summary>
		public static string FromInt(int intTooth){
			//don't need much error checking.
			string retStr="";
			retStr=intTooth.ToString();
			return retStr;
		}
		#endregion Methods - Ordinals and Ints

		#region Methods - Surfaces
		///<summary>Handles direct user input and tidies according to rules.  ToothNum might be empty, and a tidy should still be attempted.  Otherwise, toothNum must be valid.</summary>
		public static string SurfTidyForDisplay(string surf,string toothNum){
			bool isCanadian=CultureInfo.CurrentCulture.Name.EndsWith("CA");//Canadian. en-CA or fr-CA
			//Canadian valid=MOIDBLV
			if(surf==null){
				surf="";
			}
			string surfTidy="";
			List<string> listSurfaces=new List<string>();
			for(int i=0;i<surf.Length;i++){
				listSurfaces.Add(surf.Substring(i,1).ToUpper());
			}
			//M----------------------------------------
			if(listSurfaces.Contains("M")){
				surfTidy+="M";
			}
			//O-------------------------------------------
			if(toothNum=="" || IsPosterior(toothNum)){
				if(listSurfaces.Contains("O")){
					surfTidy+="O";
				}
			}
			//I---------------------------------
			if(toothNum=="" || IsAnterior(toothNum)){
				if(listSurfaces.Contains("I")) {
					surfTidy+="I";
				}
			}
			//D---------------------------------------
			if(listSurfaces.Contains((string)"D")){
				surfTidy+="D";
			}
			//B------------------------------------------------
			if(toothNum=="" || IsPosterior(toothNum)) {
				if(listSurfaces.Contains("B")) {
					surfTidy+="B";
				}
			}
			//F-----------------------------------------
			if(isCanadian) {
				if(toothNum=="" || IsAnterior(toothNum)){
					if(listSurfaces.Contains("V")) {//Canadian equivalent of F
						surfTidy+="V";
					}
				}
			}
			else {
				if(toothNum=="" || IsAnterior(toothNum)) {
					if(listSurfaces.Contains("F")) {
						surfTidy+="F";
					}
				}
			}
			//V-----------------------------------------
			if(isCanadian) {
				if(listSurfaces.Contains("5")) {//Canadian equivalent of V
					surfTidy+="5";
				}
			}
			else {
				if(listSurfaces.Contains("V")) {
					surfTidy+="V";
				}
			}
			//L-----------------------------------------
			if(listSurfaces.Contains((string)"L")){
				surfTidy+="L";
			}
			return surfTidy;
		}

		///<summary>Converts the database value to a claim value.  Special handling for V surfaces.  ToothNum must be valid.</summary>
		public static string SurfTidyForClaims(string surf,string toothNum) {
			bool isCanadian=CultureInfo.CurrentCulture.Name.EndsWith("CA");//Canadian. en-CA or fr-CA
			//Canadian valid=MOIDBLV
			if(surf==null) {
				surf="";
			}
			string surfTidy="";
			List<string> listSurfaces=new List<string>();
			for(int i=0;i<surf.Length;i++) {
				listSurfaces.Add(surf.Substring(i,1).ToUpper());
			}
			//M----------------------------------------
			if(listSurfaces.Contains("M")) {
				surfTidy+="M";
			}
			//O-------------------------------------------
			if(IsPosterior(toothNum)) {
				if(listSurfaces.Contains("O")) {
					surfTidy+="O";
				}
			}
			//I---------------------------------
			if(IsAnterior(toothNum)) {
				if(listSurfaces.Contains("I")) {
					surfTidy+="I";
				}
			}
			//D---------------------------------------
			if(listSurfaces.Contains((string)"D")) {
				surfTidy+="D";
			}
			//B------------------------------------------------
			//if(isCanadian) {//not needed because db to claim behavior is identical.  It's only in the UI where the V would show as 5
			if(IsPosterior(toothNum)) {
				if(listSurfaces.Contains("B") || listSurfaces.Contains("V")) {
					surfTidy+="B";
				}
			}
			//F-----------------------------------------
			if(IsAnterior(toothNum)) {
				if(listSurfaces.Contains("F") || listSurfaces.Contains("V")) {
					if(isCanadian) {
						surfTidy+="V";//Vestibular
					}
					else {
						surfTidy+="F";
					}
				}
			}
			//L-----------------------------------------
			if(listSurfaces.Contains((string)"L")) {
				surfTidy+="L";
			}
			return surfTidy;
		}

		///<summary>Takes display string and converts it into Db string.  ToothNum does not need to be valid.</summary>
		public static string SurfTidyFromDisplayToDb(string surf,string toothNum) {
			bool isCanadian=CultureInfo.CurrentCulture.Name.EndsWith("CA");//Canadian. en-CA or fr-CA
			//Canadian valid=MOIDBLV
			if(surf==null) {
				surf="";
			}
			string surfTidy="";
			List<string> listSurfaces=new List<string>();
			for(int i=0;i<surf.Length;i++) {
				listSurfaces.Add(surf.Substring(i,1).ToUpper());
			}
			//M----------------------------------------
			if(listSurfaces.Contains("M")) {
				surfTidy+="M";
			}
			//O-------------------------------------------
			if(toothNum=="" || IsPosterior(toothNum)) {
				if(listSurfaces.Contains("O")) {
					surfTidy+="O";
				}
			}
			//I---------------------------------
			if(toothNum=="" || IsAnterior(toothNum)) {
				if(listSurfaces.Contains("I")) {
					surfTidy+="I";
				}
			}
			//D---------------------------------------
			if(listSurfaces.Contains((string)"D")) {
				surfTidy+="D";
			}
			//B------------------------------------------------
			if(toothNum=="" || IsPosterior(toothNum)) {
				if(listSurfaces.Contains("B")) {
					surfTidy+="B";
				}
			}
			//F-----------------------------------------
			if(isCanadian) {
				if(toothNum=="" || IsAnterior(toothNum)) {
					if(listSurfaces.Contains("V")) {//Canadian equivalent of F
						surfTidy+="F";//for db
					}
				}
			}
			else {
				if(toothNum=="" || IsAnterior(toothNum)) {
					if(listSurfaces.Contains("F")) {
						surfTidy+="F";
					}
				}
			}
			//V-----------------------------------------
			if(isCanadian) {
				if(listSurfaces.Contains("5")) {//Canadian equivalent of V
					surfTidy+="V";//for db
				}
			}
			else {
				if(listSurfaces.Contains("V")) {
					surfTidy+="V";
				}
			}
			//L-----------------------------------------
			if(listSurfaces.Contains((string)"L")) {
				surfTidy+="L";
			}
			return surfTidy;
		}

		///<summary>Takes surfaces from Db and converts them to appropriate culture for display.  Only Canada supported so far.  ToothNum does not need to be valid since minimal manipulation here.</summary>
		public static string SurfTidyFromDbToDisplay(string surf,string toothNum) {
			bool isCanadian=CultureInfo.CurrentCulture.Name.EndsWith("CA");//Canadian. en-CA or fr-CA
			//Canadian valid=MOIDBLV
			if(!isCanadian) {
				return surf;
			}
			//Canadian:
			if(surf==null) {
				return "";
			}
			string surfTidy=surf.Replace("V","5");//USA classV becomes 5 for Canadian display
			surfTidy=surfTidy.Replace("F","V");//USA Facial becomes Vestibular for Canadian display
			return surfTidy;
		}
		#endregion Methods - Surfaces

		#region Methods - Ortho
		///<summary>Input will be a few American tooth numbers, separated by commas. Output will be those same tooth numbers in a different nomenclature, still separated by commas.</summary>
		public static string DisplayOrthoCommas(string toothNumbers,ToothNumberingNomenclature toothNumberingNomenclature) {
			if(toothNumbers.IsNullOrEmpty()) {
				return "";
			}
			List<string> listTeeth=toothNumbers.Split(',').ToList();
			string strResult="";
			for(int i=0;i<listTeeth.Count;i++) {
				if(!IsValidDB(listTeeth[i])){
					return "";
				}
				if(i>0){
					strResult+=",";
				}
				strResult+=Display(listTeeth[i],toothNumberingNomenclature);
			}
			return strResult;
		}

		///<summary>Input will be exactly two American tooth numbers, separated by a hyphen. Output will be those same tooth numbers in a different nomenclature, still separated by a hyphen.</summary>
		public static string DisplayOrthoDash(string toothNumbers,ToothNumberingNomenclature toothNumberingNomenclature) {
			if(toothNumbers.IsNullOrEmpty()) {
				return "";
			}
			List<string> listTeeth=toothNumbers.Split('-').ToList();
			if(listTeeth.Count!=2){
				return "";
			}
			if(!IsValidDB(listTeeth[0])){
				return "";
			}
			if(!IsValidDB(listTeeth[1])){
				return "";
			}
			string strResult=Display(listTeeth[0],toothNumberingNomenclature)+"-"+Display(listTeeth[1],toothNumberingNomenclature);
			return strResult;
		}

		///<summary>Input will be a few tooth numbers in the specified nomenclature, separated by commas. Output will be those same tooth numbers in American format, still separated by commas. Throws an ApplicationException if any formatting errors.</summary>
		public static string ParseOrthoCommas(string toothNumbers,ToothNumberingNomenclature toothNumberingNomenclature){
			if(toothNumbers.IsNullOrEmpty()) {
				return "";
			}
			toothNumbers=toothNumbers.Replace(" ","");//remove all spaces
			List<string> listTeeth=toothNumbers.Split(',').ToList();
			string strResult="";
			for(int i=0;i<listTeeth.Count;i++) {
				if(!IsValidEntry(listTeeth[i],toothNumberingNomenclature)){
					throw new ApplicationException(listTeeth[i]+" "+Lans.g("Tooth","is not a valid tooth number."));
				}
				if(i>0){
					strResult+=",";
				}
				strResult+=Parse(listTeeth[i],toothNumberingNomenclature);
			}
			return strResult;
		}
		
		///<summary>Input will be exactly two tooth numbers in the specified nomenclature, separated by a hyphen. Output will be those same tooth numbers in American format, still separated by a hyphen. Throws an ApplicationException if any formatting errors.</summary>
		public static string ParseOrthoDash(string toothNumbers,ToothNumberingNomenclature toothNumberingNomenclature){
			if(toothNumbers.IsNullOrEmpty()) {
				return "";
			}
			toothNumbers=toothNumbers.Replace(" ","");//remove all spaces
			List<string> listTeeth=toothNumbers.Split('-').ToList();
			if(listTeeth.Count!=2){
				throw new ApplicationException(toothNumbers+" "+Lans.g("Tooth","is not in valid format. Only two teeth, separated with hyphen."));
			}
			if(!IsValidEntry(listTeeth[0],toothNumberingNomenclature)){
				throw new ApplicationException(listTeeth[0]+" "+Lans.g("Tooth","is not a valid tooth number."));
			}
			if(!IsValidEntry(listTeeth[1],toothNumberingNomenclature)){
				throw new ApplicationException(listTeeth[1]+" "+Lans.g("Tooth","is not a valid tooth number."));
			}
			string tooth1=Parse(listTeeth[0],toothNumberingNomenclature);
			string tooth2=Parse(listTeeth[1],toothNumberingNomenclature);
			bool isMaxillary1=IsMaxillary(tooth1);
			bool isMaxillary2=IsMaxillary(tooth2);
			if(isMaxillary1!=isMaxillary2){
				throw new ApplicationException(Lans.g("Tooth","Teeth must be in the same arch."));
			}
			string strResult=tooth1+"-"+tooth2;
			return strResult;
		}
		#endregion Methods - Ortho
	}

	public enum ToothNumberingNomenclature {
		///<summary>0- American</summary>
		Universal,
		///<summary>1- International</summary>
		FDI,
		///<summary>2- </summary>
		Haderup,
		///<summary>3- For ortho.</summary>
		Palmer
		//We could use these UTF8 characters for true Palmer, but we would have to manually change the font and placement each time, or it looks bad.
		//This is possible, but I suspect that computer users have already moved to UR, UL, etc, which is also less ambiguous for things like dictation and letters.
		//string ur="\u23CC";
		//string ul="\u23BF";
		//string ll="\u23BE";
		//string lr="\u23CB";
	}
}
