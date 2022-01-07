using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace OpenDentBusiness{
	/// <summary></summary>
	public class Tooth{
		///<summary></summary>
		public Tooth(){
			
		}

		public static String[] labelsUniversal = new String[] { "1",  "2",  "3",  "4",  "5",  "6",  "7",  "8",  "9", "10", "11", "12", "13", "14", "15", "16", 
																"32", "31", "30", "29", "28", "27", "26", "25", "24", "23", "22", "21", "20", "19", "18", "17",
																                   "A",  "B",  "C",  "D",  "E",  "F",  "G",  "H",  "I",  "J",
																				   "T",  "S",  "R",  "Q",  "P",  "O",  "N",  "M",  "L",  "K"
																};

		private static String[] labelsFDI = new String[] {"18", "17", "16", "15", "14", "13", "12", "11", "21", "22", "23", "24", "25", "26", "27", "28", 
																"48", "47", "46", "45", "44", "43", "42", "41", "31", "32", "33", "34", "35", "36", "37", "38",
																                  "55", "54", "53", "52", "51", "61", "62", "63", "64", "65",
																				  "85", "84", "83", "82", "81", "71", "72", "73", "74", "75"
																};

		private static String[] labelsHaderup = new String[] { "8+",  "7+",  "6+",  "5+",  "4+",  "3+",  "2+",  "1+",  "+1", "+2", "+3", "+4", "+5", "+6", "+7", "+8", 
																 "8-",  "7-",  "6-",  "5-",  "4-",  "3-",  "2-",  "1-",  "-1", "-2", "-3", "-4", "-5", "-6", "-7", "-8", 
																                   "A",  "B",  "C",  "D",  "E",  "F",  "G",  "H",  "I",  "J",
																				   "T",  "S",  "R",  "Q",  "P",  "O",  "N",  "M",  "L",  "K"
																};

		private static String[] labelsPalmer = new String[] {
			"UR8","UR7","UR6","UR5","UR4","UR3","UR2","UR1","UL1","UL2","UL3","UL4","UL5","UL6","UL7","UL8",
			"LR8","LR7","LR6","LR5","LR4","LR3","LR2","LR1","LL1","LL2","LL3","LL4","LL5","LL6","LL7","LL8",
			"URE","URD","URC","URB","URA","ULA","ULB","ULC","ULD","ULE",
			"LRE","LRD","LRC","LRB","LRA","LLA","LLB","LLC","LLD","LLE"};

		public static String[] labelsPalmerSimple = new String[] {
			"8","7","6","5","4","3","2","1","1","2","3","4","5","6","7","8",
			"8","7","6","5","4","3","2","1","1","2","3","4","5","6","7","8",
			"E","D","C","B","A","A","B","C","D","E",
			"E","D","C","B","A","A","B","C","D","E"};

		///<summary></summary>
		public static bool IsAnterior(string tooth_id) {
			if(!IsValidDB(tooth_id)) {
				return false;
			}
			int intTooth=ToInt(tooth_id);
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
				return true;
			return false;
		}

		///<summary></summary>
		public static bool IsPreMolar(int intTooth){
			string toothNum=FromInt(intTooth);
			return IsPreMolar(toothNum);
		}

		///<summary>Sometimes validated by IsValidDB before coming here, otherwise an invalid toothnum .  This should be run on all displayed tooth numbers. It will handle checking for whether user is using international tooth numbers.  All tooth numbers are passed in american values until the very last moment.  Just before display, the string is converted using this method.</summary>
		public static string GetToothLabel(string tooth_id,ToothNumberingNomenclature nomenclature) {
			if(tooth_id==null || tooth_id==""){
				return ""; 
			}
			if(nomenclature == ToothNumberingNomenclature.Universal) {
				return tooth_id; 
			}
			int index = Array.IndexOf(labelsUniversal, tooth_id);
			if(index==-1){
				if(nomenclature == ToothNumberingNomenclature.FDI
					&& CultureInfo.CurrentCulture.Name.EndsWith("CA")
					&& tooth_id=="51")
				{
					return "99";//supernumerary tooth: It is documented in the cdha website that 99 is the only valid number for supernumerary teeth.
				}
				return "-";
			}
			if(nomenclature == ToothNumberingNomenclature.FDI) {
				return labelsFDI[index];
			}
			else if(nomenclature == ToothNumberingNomenclature.Haderup) { 
				return labelsHaderup[index];
			}
			else if(nomenclature == ToothNumberingNomenclature.Palmer) { 
				return labelsPalmer[index];
			}
			return "-"; // Should never happen
		}

		public static string GetToothLabel(string tooth_id) {
			return GetToothLabel(tooth_id,(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
		}

		///<summary>Identical to GetToothLabel, but just used in the 3D tooth chart because with Palmer, we don't want the UR, UL, etc.</summary>
		public static string GetToothLabelGraphic(string tooth_id,ToothNumberingNomenclature nomenclature){
			if(tooth_id==null || tooth_id==""){
				return ""; 
			}
			if(nomenclature==ToothNumberingNomenclature.Universal) {
				return tooth_id;
			}
			int index = Array.IndexOf(labelsUniversal, tooth_id);
			if(index==-1){
				return "-";
			}
			if(nomenclature == ToothNumberingNomenclature.FDI) {
				return labelsFDI[index];
			}
			else if(nomenclature == ToothNumberingNomenclature.Haderup) { 
				return labelsHaderup[index];
			}
			else if(nomenclature == ToothNumberingNomenclature.Palmer) {
				return labelsPalmerSimple[index];
			}
			return "-"; // Should never happen
		}

		public static string GetToothLabelGraphic(string tooth_id) {
			return GetToothLabelGraphic(tooth_id,(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
		}

		///<summary>MUST be validated by IsValidEntry before coming here. All user entered toothnumbers are run through this method which automatically checks to see if using international toothnumbers. So the procedurelog class will always contain the american toothnum.</summary>
		public static string GetToothId(string tooth_label) {
			ToothNumberingNomenclature nomenclature=(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			return GetToothId(tooth_label,nomenclature);
		}

		///<summary>MUST be validated by IsValidEntry before coming here.</summary>
		public static string GetToothId(string tooth_label,ToothNumberingNomenclature nomenclature) {
			if(nomenclature == ToothNumberingNomenclature.Universal) {
				return tooth_label;
			}
			int index = 0;
			if(nomenclature == ToothNumberingNomenclature.FDI) {
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && tooth_label=="99") {
					return "51";//supernumerary tooth: It is documented in the cdha website that 99 is the only valid number for supernumerary teeth.
				}
				index = Array.IndexOf(labelsFDI, tooth_label);
			}
			else if(nomenclature == ToothNumberingNomenclature.Haderup) { 
				index = Array.IndexOf(labelsHaderup, tooth_label);
			}
			else if(nomenclature == ToothNumberingNomenclature.Palmer) { 
				index = Array.IndexOf(labelsPalmer, tooth_label);
			}
			return labelsUniversal[index];
		}
		
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
			foreach(string toothNum in listToothNums.Distinct()) {
				string quad=GetQuadrant(toothNum);
				if(quad!="") {
					listQuads.Add(quad);
				}
			}
			//Return distinct list just in case we added a tooth twice while looping.
			return listQuads.Distinct().ToList();
		}

		/// <summary>Returns a string list of distinct arches that contain the specified teeth. If no teeth are selected, it will return an empty list.</summary>
		public static List<string> GetArchesForTeeth(List<string> listToothNums) {
			if(listToothNums.Count==0) {
				return new List<string>();
			}
			List<string> listQuads=new List<string>();
			if(listToothNums.Where(x => IsMaxillary(x)).Count()>0) {
				listQuads.Add("U");
			}
			if(listToothNums.Where(x => IsValidDB(x) && !IsMaxillary(x)).Count()>0) {
				listQuads.Add("L");
			}
			return listQuads;
		}

		///<summary>For nomenclature, use pref UseInternationalToothNumbers, or a hard coded value.</summary>
		public static string GetSextant(string surf,ToothNumberingNomenclature nomenclature) {
			if(nomenclature==ToothNumberingNomenclature.Universal) {
				return surf;
			}
			if(nomenclature==ToothNumberingNomenclature.FDI) {
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

		///<summary>Sometimes validated by IsValidDB before coming here, otherwise an invalid toothnum.  This should be run on all displayed tooth numbers. It will handle checking for whether user is using international tooth numbers.  All tooth numbers are passed in american values until the very last moment.  Just before display, the string is converted using this method. Checks the preference cache for proper nomenclature.</summary>
		public static string ToInternat(string toothNum) { 
			return ToInternat(toothNum,(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
		}

		///<summary>Sometimes validated by IsValidDB before coming here, otherwise an invalid toothnum.  This should be run on all displayed tooth numbers. It will handle checking for whether user is using international tooth numbers.  All tooth numbers are passed in american values until the very last moment.  Just before display, the string is converted using this method.</summary>
		public static string ToInternat(string toothNum,ToothNumberingNomenclature nomenclature) {
			return GetToothLabel(toothNum,nomenclature);
		}

		///<summary>MUST be validated by IsValidEntry before coming here.  All user entered toothnumbers are run through this method which automatically checks to see if using international toothnumbers.  So the procedurelog class will always contain the american toothnum.</summary>
		public static string FromInternat(string toothNum){
			return GetToothId(toothNum);
		}

		///<summary>The supplied toothNumbers will be a series of tooth numbers separated by commas.  They will be in american format.  For display purposes, ranges will use dashes, and international numbers will be used.</summary>
		public static string FormatRangeForDisplay(string toothNumbers) {
			if(toothNumbers==null) {
				return "";
			}
			toothNumbers=toothNumbers.Replace(" ","");//remove all spaces
			if(toothNumbers=="") {
				return "";
			}
			string[] toothArray=toothNumbers.Split(',');
			if(toothArray.Length==1) {
				return Tooth.GetToothLabel(toothArray[0]);
			}
			else if(toothArray.Length==2) {
				return Tooth.GetToothLabel(toothArray[0])+","+Tooth.GetToothLabel(toothArray[1]);//just two numbers separated by comma
			}
			Array.Sort<string>(toothArray, new ToothComparer());
			StringBuilder strbuild=new StringBuilder();
			//List<string> toothList=new List<string>();
			//strbuild.Append(Tooth.ToInternat(toothArray[0]));//always show the first number
			int currentNum;
			int nextNum;
			int numberInaRow=1;//must have 3 in a row to trigger dash
			for(int i=0;i<toothArray.Length-1;i++) {
				//in each loop, we are comparing the current number with the next number
				currentNum=Tooth.ToOrdinal(toothArray[i]);
				nextNum=Tooth.ToOrdinal(toothArray[i+1]);
				if(nextNum-currentNum==1 && currentNum!=16 && currentNum!=32) {//if sequential (sequences always break at end of arch)
					numberInaRow++;
				}
				else {
					numberInaRow=1;
				}
				if(numberInaRow<3) {//the next number is not sequential,or if it was a sequence, and it's now broken
					if(strbuild.Length>0 && strbuild[strbuild.Length-1]!='-') {
						strbuild.Append(",");
					}
					strbuild.Append(Tooth.GetToothLabel(toothArray[i]));
				}
				else if(numberInaRow==3) {//this way, the dash only gets added exactly once
					strbuild.Append("-");
				}
				//else do nothing
			}
			if(strbuild.Length>0 && strbuild[strbuild.Length-1]!='-') {
				strbuild.Append(",");
			}
			strbuild.Append(Tooth.GetToothLabel(toothArray[toothArray.Length-1]));//always show the last number
			return strbuild.ToString();
		}

		///<summary>Takes a user entered string and validates/formats it for the database.  Throws an ApplicationException if any formatting errors.  User string can contain spaces, dashes, and commas, too.</summary>
		public static string FormatRangeForDb(string toothNumbers){
			if(toothNumbers==null) {
				return "";
			}
			toothNumbers=toothNumbers.Replace(" ","");//remove all spaces
			if(toothNumbers=="") {
				return "";
			}
			string[] toothArray=toothNumbers.Split(',');//some items will contain dashes
			List<string> toothList=new List<string>();
			string rangebegin;
			string rangeend;
			int beginint;
			int endint;
			//not sure how to handle supernumerary.  Probably just not acceptable.
			for(int i=0;i<toothArray.Length;i++){
				if(toothArray[i].Contains("-")){
					rangebegin=toothArray[i].Split('-')[0].ToUpper();
					rangeend=toothArray[i].Split('-')[1].ToUpper();
					if(!IsValidEntry(rangebegin)) {
						throw new ApplicationException(rangebegin+" "+Lans.g("Tooth","is not a valid tooth number."));
					}
					if(!IsValidEntry(rangeend)) {
						throw new ApplicationException(rangeend+" "+Lans.g("Tooth","is not a valid tooth number."));
					}
					beginint=Tooth.ToOrdinal(GetToothId(rangebegin));
					endint=Tooth.ToInt(GetToothId(rangeend));
					if(endint<beginint){
						throw new ApplicationException("Range specified is impossible.");
					}
					while(beginint<=endint){
						toothList.Add(Tooth.FromOrdinal(beginint));
						beginint++;
					}
				}
				else{
					string toothNum=toothArray[i].ToUpper();
					if(!IsValidEntry(toothNum)){
						throw new ApplicationException(toothNum+" "+Lans.g("Tooth","is not a valid tooth number."));
					}
					toothList.Add(GetToothId(toothNum));
				}
			}
			toothList.Sort(new ToothComparer());
			string retVal="";
			for(int i=0;i<toothList.Count;i++){
				if(i>0){
					retVal+=",";
				}
				retVal+=toothList[i];
			}
			return retVal;
		}

		///<summary>Used every time user enters toothNum in procedure box. Must be followed with FromInternat. These are the *ONLY* methods that are designed to accept user input.  Can also handle international toothnum</summary>
		public static bool IsValidEntry(string toothNum){
			int nomenclature = PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			if(nomenclature==0){//Universal,american
				//tooth numbers validated the same as they are in db.
				return IsValidDB(toothNum);
			}
			else if(nomenclature==1){// FDI
				if(toothNum==null || toothNum==""){
					return false;
				}
				if(Regex.IsMatch(toothNum,"^[1-4][1-8]$")){//perm teeth: matches firt digit 1-4 and second digit 1-8,9 would be supernumerary?
					return true;
				}
				if(Regex.IsMatch(toothNum,"^[5-8][1-5]$")){//pri teeth: matches firt digit 5-8 and second digit 1-5
					return true;
				}
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && toothNum=="99") {//supernumerary tooth: It is documented in the cdha website that 99 is the only valid number for supernumerary teeth.
					return true;
				}
				return false;
			}
			else if(nomenclature==2) {//Haderup
				if(toothNum==null || toothNum=="") {
					return false;
				}
				for(int i=0;i<labelsHaderup.Length;i++) {
					if(labelsHaderup[i]==toothNum) {
						return true;
					}
				}
				return false;
			}
			else{// if(nomenclature==3) {// Palmer
				if(toothNum==null || toothNum=="") {
					return false;
				}
				for(int i=0;i<labelsPalmer.Length;i++) {
					if(labelsPalmer[i]==toothNum) {
						return true;
					}
				}
				return false;
			}			
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
			if(intTooth<=32)
				return false;
			if(intTooth>=51 && intTooth<=82)//supernumerary
				return true;	
			return false;
		}

		///<summary>Returns 1-32, or -1.  The toothNum should be validated before coming here, but it won't crash if invalid.  Primary or perm are ok.  Empty and null are also ok.  Supernumerary are also ok.</summary>
		public static int ToInt(string tooth_id){
			if(tooth_id==null || tooth_id=="")
				return -1;
			try{
				if(IsPrimary(tooth_id)) {
					return Convert.ToInt32(PriToPerm(tooth_id));
				}
				else if(IsSuperNum(tooth_id)) {
					return Convert.ToInt32(SupToPerm(tooth_id));
				}
				else {
					return Convert.ToInt32(tooth_id);
				}
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

		///<summary>Returns true if A-T or AS-TS.  Otherwise, returns false.</summary>
		public static bool IsPrimary(string tooth_id) {
			if(string.IsNullOrEmpty(tooth_id)) {
				return false;
			}
			if(Regex.IsMatch(tooth_id,"^[A-T]$")) {
				return true;
			}
			if(Regex.IsMatch(tooth_id,"^[A-T]S$")) {
				return true;
			}
			return false;
		}

		///<summary></summary>
		public static string PermToPri(string tooth_id) {
			switch(tooth_id) {
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
			string tooth_id=FromInt(intTooth);
			return PermToPri(tooth_id);
		}

		///<summary></summary>
		public static string PriToPerm(string tooth_id) {
			switch(tooth_id) {
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

		///<summary>Converts supernumerary teeth to permanent.</summary>
		public static string SupToPerm(string tooth_id) {
			switch(tooth_id) {
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
			
		///<summary></summary>
		public static bool IsMaxillary(int intTooth){
			string toothNum=FromInt(intTooth);
			return IsMaxillary(toothNum);
		}

		///<summary>Primary, permanent, and supernumerary tooth_ids are all accepted.</summary>
		public static bool IsMaxillary(string tooth_id) {
			if(!IsValidDB(tooth_id)) {
				return false;
			}
			int intTooth=ToInt(tooth_id);
			if(intTooth>=1 && intTooth<=16) {
				return true;
			}
			return false;
		}

		///<summary>Handles direct user input and tidies according to rules.  ToothNum might be empty, and a tidy should still be attempted.  Otherwise, toothNum must be valid.</summary>
		public static string SurfTidyForDisplay(string surf,string toothNum){
			bool isCanadian=CultureInfo.CurrentCulture.Name.EndsWith("CA");//Canadian. en-CA or fr-CA
			//Canadian valid=MOIDBLV
			if(surf==null){
				surf="";
			}
      string surfTidy="";
      ArrayList al=new ArrayList();
      for(int i=0;i<surf.Length;i++){
        al.Add(surf.Substring(i,1).ToUpper());
      }
			//M----------------------------------------
      if(al.Contains("M")){
        surfTidy+="M";
      }
			//O-------------------------------------------
			if(toothNum=="" || IsPosterior(toothNum)){
				if(al.Contains("O")){
					surfTidy+="O";
				}
			}
			//I---------------------------------
			if(toothNum=="" || IsAnterior(toothNum)){
				if(al.Contains("I")) {
					surfTidy+="I";
				}
			}
      //D---------------------------------------
      if(al.Contains((string)"D")){
        surfTidy+="D";
      }
			//B------------------------------------------------
			if(toothNum=="" || IsPosterior(toothNum)) {
				if(al.Contains("B")) {
					surfTidy+="B";
				}
			}
			//F-----------------------------------------
			if(isCanadian) {
				if(toothNum=="" || IsAnterior(toothNum)){
					if(al.Contains("V")) {//Canadian equivalent of F
						surfTidy+="V";
					}
				}
			}
			else {
				if(toothNum=="" || IsAnterior(toothNum)) {
					if(al.Contains("F")) {
						surfTidy+="F";
					}
				}
			}
			//V-----------------------------------------
			if(isCanadian) {
				if(al.Contains("5")) {//Canadian equivalent of V
					surfTidy+="5";
				}
			}
			else {
				if(al.Contains("V")) {
					surfTidy+="V";
				}
			}
			//L-----------------------------------------
      if(al.Contains((string)"L")){
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
			ArrayList al=new ArrayList();
			for(int i=0;i<surf.Length;i++) {
				al.Add(surf.Substring(i,1).ToUpper());
			}
			//M----------------------------------------
			if(al.Contains("M")) {
				surfTidy+="M";
			}
			//O-------------------------------------------
			if(IsPosterior(toothNum)) {
				if(al.Contains("O")) {
					surfTidy+="O";
				}
			}
			//I---------------------------------
			if(IsAnterior(toothNum)) {
				if(al.Contains("I")) {
					surfTidy+="I";
				}
			}
			//D---------------------------------------
			if(al.Contains((string)"D")) {
				surfTidy+="D";
			}
			//B------------------------------------------------
			//if(isCanadian) {//not needed because db to claim behavior is identical.  It's only in the UI where the V would show as 5
			if(IsPosterior(toothNum)) {
				if(al.Contains("B") || al.Contains("V")) {
					surfTidy+="B";
				}
			}
			//F-----------------------------------------
			if(IsAnterior(toothNum)) {
				if(al.Contains("F") || al.Contains("V")) {
					if(isCanadian) {
						surfTidy+="V";//Vestibular
					}
					else {
						surfTidy+="F";
					}
				}
			}
			//L-----------------------------------------
			if(al.Contains((string)"L")) {
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
			ArrayList al=new ArrayList();
			for(int i=0;i<surf.Length;i++) {
				al.Add(surf.Substring(i,1).ToUpper());
			}
			//M----------------------------------------
			if(al.Contains("M")) {
				surfTidy+="M";
			}
			//O-------------------------------------------
			if(toothNum=="" || IsPosterior(toothNum)) {
				if(al.Contains("O")) {
					surfTidy+="O";
				}
			}
			//I---------------------------------
			if(toothNum=="" || IsAnterior(toothNum)) {
				if(al.Contains("I")) {
					surfTidy+="I";
				}
			}
			//D---------------------------------------
			if(al.Contains((string)"D")) {
				surfTidy+="D";
			}
			//B------------------------------------------------
			if(toothNum=="" || IsPosterior(toothNum)) {
				if(al.Contains("B")) {
					surfTidy+="B";
				}
			}
			//F-----------------------------------------
			if(isCanadian) {
				if(toothNum=="" || IsAnterior(toothNum)) {
					if(al.Contains("V")) {//Canadian equivalent of F
						surfTidy+="F";//for db
					}
				}
			}
			else {
				if(toothNum=="" || IsAnterior(toothNum)) {
					if(al.Contains("F")) {
						surfTidy+="F";
					}
				}
			}
			//V-----------------------------------------
			if(isCanadian) {
				if(al.Contains("5")) {//Canadian equivalent of V
					surfTidy+="V";//for db
				}
			}
			else {
				if(al.Contains("V")) {
					surfTidy+="V";
				}
			}
			//L-----------------------------------------
			if(al.Contains((string)"L")) {
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

		/// <summary>This will be deleted as soon as it's no longer in use by DirectX chart.</summary>
		public static float PerioShiftMm(string tooth_id) {
			return 0;
		}


	}

	public enum ToothNumberingNomenclature {
		///<summary>0- American</summary>
		Universal,
		///<summary>1- International</summary>
		FDI,
		///<summary>2- </summary>
		Haderup,
		///<summary>3- Ortho</summary>
		Palmer
	}
}
