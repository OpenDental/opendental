using EhrLaboratories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	///<summary>Used in Ehr patient lists.</summary>
	public class EhrPatListElements {

		public static DataTable GetListOrderBy2014(List<EhrPatListElement2014> elementList) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),elementList);
			}
			DataTable table=new DataTable();
			string select="SELECT patient.PatNum,patient.LName,patient.FName";
			string from="FROM patient";
			string where="WHERE TRUE ";//Makes formatting easier when adding additional clauses because they will all be AND clauses.
			for(int i=0;i<elementList.Count;i++) {
				switch(elementList[i].Restriction) {
					case EhrRestrictionType.Birthdate://---------------------------------------------------------------------------------------------------------------------------
						select+=",patient.BirthDate, ((YEAR(CURDATE())-YEAR(DATE(patient.Birthdate))) - (RIGHT(CURDATE(),5)<RIGHT(DATE(patient.Birthdate),5))) AS Age";
						from+="";//only selecting from patient table
						where+="AND ((YEAR(CURDATE())-YEAR(DATE(patient.Birthdate))) - (RIGHT(CURDATE(),5)<RIGHT(DATE(patient.Birthdate),5)))"+GetOperandText(elementList[i].Operand)+""+PIn.String(elementList[i].CompareString)+" ";
						break;
					case EhrRestrictionType.Gender://------------------------------------------------------------------------------------------------------------------------------
						select+=",patient.Gender";//will look odd if user adds multiple gender columns, enum needs to be "decoded" when filling grid.
						break;
					case EhrRestrictionType.LabResult://---------------------------------------------------------------------------------------------------------------------------
						//TODO Units
						from+=",ehrlab AS ehrlab"+i+",ehrlabresult AS ehrlabresult"+i+" ";
						where+="AND ehrlab"+i+".PatNum=patient.PatNum AND ehrlab"+i+".EhrLabNum=ehrlabresult"+i+".EhrLabNum ";//join
						where+="AND ('"+elementList[i].CompareString+"'=ehrlabresult"+i+".ObservationIdentifierID OR '" 
							+elementList[i].CompareString+"'=ehrlabresult"+i+".ObservationIdentifierIDAlt) ";//filter, LOINC of lab observation
						if(elementList[i].StartDate!=null && elementList[i].StartDate.Year>1880) {
							where+="AND ehrlabresult"+i+".ObservationDateTime >="+POut.Date(elementList[i].StartDate)+" ";//on or after this date
						}
						if(elementList[i].EndDate!=null && elementList[i].EndDate.Year>1880) {
							where+="AND ehrlabresult"+i+".ObservationDateTime <="+POut.Date(elementList[i].EndDate)+" ";//on or before this date
						}
						switch(elementList[i].LabValueType) {
							//CE and CWE should be SNOMEDCT codes, string compare elementList[i].LabValue to ehrlabresult.ObservationValueCodedElementID or ObservationValueCodedElementIDAlt
							case HL70125.CE:
							case HL70125.CWE:
								select+=",(CASE WHEN ehrlabresult"+i+".ObservationValueCodedElementID='' THEN ehrlabresult"+i+".ObservationValueCodedElementIDAlt ELSE ehrlabresult"+i+".ObservationValueCodedElementID END) AS LabValue";
								where+="AND (ehrlabresult"+i+".ObservationValueCodedElementID='"+elementList[i].LabValue+"' OR "
									+"ehrlabresult"+i+".ObservationValueCodedElementIDAlt='"+elementList[i].LabValue+"') "
									+"AND (ehrlabresult"+i+".ValueType='CWE' OR ehrlabresult"+i+".ValueType='CE') ";
								break;
							//DT is stored as a string in ehrlabresult.ObservationValueDateTime as YYYY[MM[DD]]
							case HL70125.DT:
								select+=",ehrlabresult"+i+".ObservationValueDateTime ";//+DbHelper.DateFormatColumn("RPAD(ehrlabresult"+i+".ObservationValueDateTime,8,'01')","%m/%d/%Y");
								where+="AND "+DbHelper.DtimeToDate("RPAD(ehrlabresult"+i+".ObservationValueDateTime,8,'01')")
									+GetOperandText(elementList[i].Operand)+"'"+POut.String(elementList[i].LabValue)+"' "
									+"AND ehrlabresult"+i+".ValueType='DT' ";
								break;
							//TS is YYYYMMDDHHMMSS, string compare
							case HL70125.TS:
								select+=",ehrlabresult"+i+".ObservationValueDateTime ";//+DbHelper.DateTFormatColumn("ehrlabresult"+i+".ObservationValueDateTime","%m/%d/%Y %H:%i:%s");
								where+="AND ehrlabresult"+i+".ObservationValueDateTime "//+POut.DateT(PIn.DateT(DbHelper.DateTFormatColumn("ehrlabresult"+i+".ObservationValueDateTime","%m/%d/%Y %H:%i:%s")))
									+GetOperandText(elementList[i].Operand)+"'"+POut.String(elementList[i].LabValue)+"' "
									+"AND ehrlabresult"+i+".ValueType='TS' ";
								break;
							//00:00:00
							case HL70125.TM:
								select+=",ehrlabresult"+i+".ObservationValueTime";
								where+="AND ehrlabresult"+i+".ObservationValueTime"+GetOperandText(elementList[i].Operand)+"'"+POut.TSpan(PIn.TSpan(elementList[i].LabValue))+"' "
									+"AND ehrlabresult"+i+".ValueType='TM' ";
								break;
							case HL70125.SN:
								select+=",CONCAT(CONCAT(CONCAT(ehrlabresult"+i+".ObservationValueComparator,ehrlabresult"+i+".ObservationValueNumber1),ehrlabresult"+i+".ObservationValueSeparatorOrSuffix),ehrlabresult"+i+".ObservationValueNumber2)";
								where+="AND ehrlabresult"+i+".ValueType='SN' ";
								break;
							case HL70125.NM:
								select+=",ehrlabresult"+i+".ObservationValueNumeric";
								where+="AND ehrlabresult"+i+".ObservationValueNumeric"+GetOperandText(elementList[i].Operand)+POut.Double(PIn.Double(elementList[i].LabValue))+" "
									+"AND ehrlabresult"+i+".ValueType='NM' ";
								break;
							case HL70125.FT:
							case HL70125.ST:
							case HL70125.TX:
								select+=",ehrlabresult"+i+".ObservationValueText";
								//where+="AND ehrlabresult"+i+".ObservationValueText"+GetOperandText(elementList[i].Operand)+POut.String(elementList[i].LabValue)+" "
								where+="AND (ehrlabresult"+i+".ValueType='FT' OR ehrlabresult"+i+".ValueType='ST' OR ehrlabresult"+i+".ValueType='TX') ";
								break;
						}
						select+=",ehrlabresult"+i+".ObservationDateTime ";

						//select+=",labresult"+i+".ObsValue,labresult"+i+".DateTimeTest";//format column name when filling grid.
						//from+=",labresult AS labresult"+i+", labpanel AS labpanel"+i;
						//where+="AND labpanel"+i+".LabpanelNum=labresult"+i+".LabpanelNum AND patient.PatNum=labpanel"+i+".PatNum ";//join
						//where+="AND labresult"+i+".TestId='"+elementList[i].CompareString+"' "
						//			+"AND labresult"+i+".ObsValue"+GetOperandText(elementList[i].Operand)+"'"+PIn.String(elementList[i].LabValue)+"' ";//filter
						//if(elementList[i].StartDate!=null && elementList[i].StartDate.Year>1880) {
						//	where+="AND labresult"+i+".DateTimeTest>"+POut.Date(elementList[i].StartDate)+" ";//after this date
						//}
						//if(elementList[i].EndDate!=null && elementList[i].EndDate.Year>1880) {
						//	where+="AND labresult"+i+".DateTimeTest<"+POut.Date(elementList[i].EndDate)+" ";//before this date
						//}
						break;
					case EhrRestrictionType.Medication://--------------------------------------------------------------------------------------------------------------------------
						select+=",medicationpat"+i+".DateStart";//Name of medication will be in column title.
						from+=",medication AS medication"+i+", medicationpat AS medicationpat"+i;
						where+="AND medicationpat"+i+".PatNum=patient.PatNum ";//join
						//This is unusual.  Part of the join logic is in the code below because medicationPat.MedicationNum might be 0 if it came from newcrop.
						where+="AND ((medication"+i+".MedicationNum=MedicationPat"+i+".MedicationNum AND medication"+i+".MedName LIKE '%"+PIn.String(elementList[i].CompareString)+"%') "
						      +"  OR (medication"+i+".MedicationNum=0 AND medicationpat"+i+".MedDescript LIKE '%"+PIn.String(elementList[i].CompareString)+"%')) ";
						if(elementList[i].StartDate!=null && elementList[i].StartDate.Year>1880) {
							where+="AND medicationpat"+i+".DateStart>"+POut.Date(elementList[i].StartDate)+" ";//after this date
						}
						if(elementList[i].EndDate!=null && elementList[i].EndDate.Year>1880) {
							where+="AND medicationpat"+i+".DateStart<"+POut.Date(elementList[i].EndDate)+" ";//before this date
						}
						break;
					case EhrRestrictionType.Problem://-----------------------------------------------------------------------------------------------------------------------------
						select+=",disease"+i+".DateStart";//Name of problem will be in column title.
						from+=",disease AS disease"+i+", diseasedef AS diseasedef"+i;
						where+="AND diseasedef"+i+".DiseaseDefNum=disease"+i+".DiseaseDefNum AND disease"+i+".PatNum=patient.PatNum ";//join
						where+="AND (diseasedef"+i+".ICD9Code='"+PIn.String(elementList[i].CompareString)+"' OR diseasedef"+i+".SnomedCode='"+PIn.String(elementList[i].CompareString)+"') ";//filter
						if(elementList[i].StartDate!=null && elementList[i].StartDate.Year>1880) {
							where+="AND disease"+i+".DateStart>"+POut.Date(elementList[i].StartDate)+" ";//after this date
						}
						if(elementList[i].EndDate!=null && elementList[i].EndDate.Year>1880) {
							where+="AND disease"+i+".DateStart<"+POut.Date(elementList[i].EndDate)+" ";//before this date
						}
						break;
					case EhrRestrictionType.Allergy://-----------------------------------------------------------------------------------------------------------------------------
						select+=",allergy"+i+".DateAdverseReaction";//Name of allergy will be in column title.
						from+=",allergy AS allergy"+i+", allergydef AS allergydef"+i;
						where+="AND allergydef"+i+".AllergyDefNum=allergy"+i+".AllergyDefNum AND allergy"+i+".PatNum=patient.PatNum ";//join
						where+="AND allergydef"+i+".Description='"+PIn.String(elementList[i].CompareString)+"' ";//filter
						if(elementList[i].StartDate!=null && elementList[i].StartDate.Year>1880) {
							where+="AND allergy"+i+".DateAdverseReaction>"+POut.Date(elementList[i].StartDate)+" ";//after this date
						}
						if(elementList[i].EndDate!=null && elementList[i].EndDate.Year>1880) {
							where+="AND allergy"+i+".DateAdverseReaction<"+POut.Date(elementList[i].EndDate)+" ";//before this date
						}
						break;
					case EhrRestrictionType.CommPref://----------------------------------------------------------------------------------------------------------------------------
						select+=",patient.PreferContactConfidential";
						from+="";//only selecting from patient table
						where+="AND patient.PreferContactConfidential="+PIn.Int(contactMethodHelper(elementList[i].CompareString))+" ";
						break;
					default:
						//should never happen.
						continue;
				}
			}
			string command=select+" "+from+" "+where;
			return Db.GetTable(command);
		}

		///<summary>This is a potential fix to be backported to 13.2 so that patient lists can be used for MU1 2013. on large databases these queries take way to long to run. (At least several minutes).</summary>
		public static DataTable GetListOrderBy2014Retro(List<EhrPatListElement> elementList) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),elementList);
			}
			DataTable table=new DataTable();
			string select="SELECT patient.PatNum,patient.LName,patient.FName";
			string from="FROM patient";
			string where="WHERE TRUE ";//Makes formatting easier when adding additional clauses because they will all be AND clauses.
			for(int i=0;i<elementList.Count;i++) {
				switch(elementList[i].Restriction) {
					case EhrRestrictionType.Birthdate://---------------------------------------------------------------------------------------------------------------------------
						select+=",patient.BirthDate ";
						from+="";//only selecting from patient table
						where+="AND ((YEAR(CURDATE())-YEAR(DATE(patient.Birthdate))) - (RIGHT(CURDATE(),5)<RIGHT(DATE(patient.Birthdate),5)))"+GetOperandText(elementList[i].Operand)+""+PIn.String(elementList[i].CompareString)+" ";
						break;
					case EhrRestrictionType.Gender://------------------------------------------------------------------------------------------------------------------------------
						select+=",patient.Gender";//will look odd if user adds multiple gender columns, enum needs to be "decoded" when filling grid.
						break;
					case EhrRestrictionType.LabResult://---------------------------------------------------------------------------------------------------------------------------
						select+=",labresult"+i+".ObsValue";//format column name when filling grid.
						from+=",labresult AS labresult"+i+", labpanel AS labpanel"+i;
						where+="AND labpanel"+i+".LabpanelNum=labresult"+i+".LabpanelNum AND patient.PatNum=labpanel"+i+".PatNum ";//join
						where+="AND labresult"+i+".TestId='"+elementList[i].CompareString+"' "
									+"AND labresult"+i+".ObsValue"+GetOperandText(elementList[i].Operand)+"'"+PIn.String(elementList[i].LabValue)+"' ";//filter
						break;
					case EhrRestrictionType.Medication://--------------------------------------------------------------------------------------------------------------------------
						select+=",'X'";//select+=",medicationpat"+i+".DateStart";
						from+=",medication AS medication"+i+", medicationpat AS medicationpat"+i;
						where+="AND medicationpat"+i+".PatNum=patient.PatNum ";//join
						//This is unusual.  Part of the join logic is in the code below because medicationPat.MedicationNum might be 0 if it came from newcrop.
						where+="AND ((medication"+i+".MedicationNum=MedicationPat"+i+".MedicationNum AND medication"+i+".MedName LIKE '%"+PIn.String(elementList[i].CompareString)+"%') "
						      +"  OR (medication"+i+".MedicationNum=0 AND medicationpat"+i+".MedDescript LIKE '%"+PIn.String(elementList[i].CompareString)+"%')) ";
						break;
					case EhrRestrictionType.Problem://-----------------------------------------------------------------------------------------------------------------------------
						select+=",Concat('(',diseasedef"+i+".ICD9Code,') - ',diseasedef"+i+".DiseaseName) as Disease"+i+" ";//Name of problem will be in column title.
						from+=",disease AS disease"+i+", diseasedef AS diseasedef"+i;
						where+="AND diseasedef"+i+".DiseaseDefNum=disease"+i+".DiseaseDefNum AND disease"+i+".PatNum=patient.PatNum ";//join
						where+="AND diseasedef"+i+".ICD9Code='"+PIn.String(elementList[i].CompareString)+"' ";//filter
						break;
					default:
						//Can happen because EhrRestrictionType was expanded for 2014.
						//If we reach this point in the code, we will effectively just ignore the pat list element.
						continue;
				}
			}
			string command=select+" "+from+" "+where+" ORDER BY Patient.LName, Patient.FName";
			return Db.GetTable(command);
		}

		///<summary>Tries to match input string to enum name of ContactMethod and returns an int(as a string). Returns empty string if no match.</summary>
		private static string contactMethodHelper(string contactEnumName) {
			string[] names=Enum.GetNames(typeof(ContactMethod));
			for(int i=0;i<names.Length;i++) {
				if(names[i]==contactEnumName) {
					return i.ToString();
				}
			}
			return "";
		}

		///<summary>generate list of PatNums in the format "(#,#,#,#,...)" for use in an "IN" clause. Works with empty list.</summary>
		private static string patNumHelper(List<long> listPatNums) {
			StringBuilder strb = new StringBuilder();
			strb.Append("(");
			for(int i=0;i<listPatNums.Count;i++) {
				strb.Append((i==0?"":",")+listPatNums[i]);
			}
			strb.Append(")");
			return strb.ToString();
		}

		/////<summary>Returns a list of PatNums, all of which meet all of the EhrPatListElemet2014 requirements.</summary>
		//private static List<long> getPatientsHelper(List<EhrPatListElement2014> elementList) {
		//  List<long> retval = new List<long>();
		//  //retval.Add(1); 
		//  //retval.Add(3);
		//  //retval.Add(5);
		//  //return retval;
		//  string command="SELECT patient.PatNum,patient.LName,patient.FName FROM patient ";
		//  for(int i=0;i<elementList.Count;i++) {
		//    if(i==0) {
		//      command+="WHERE patient.Patnum IN(";
		//    }
		//    else {
		//      command+="AND patient.Patnum IN(";
		//    }
		//    string compStr=elementList[i].CompareString;
		//    switch(elementList[i].Restriction) {
		//      case EhrRestrictionType.Birthdate:
		//        if(DataConnection.DBtype==DatabaseType.MySql) {
		//          command+="SELECT patient.patnum FROM patient WHERE (YEAR(CURDATE())-YEAR(Birthdate)) - (RIGHT(CURDATE(),5)<RIGHT(Birthdate,5))"+GetOperandText(elementList[i].Operand)+PIn.Int(compStr);//works with >, <, and =
		//        }
		//        else {
		//          command+="SELECT patient.patnum FROM patient WHERE TRUNC(MONTHS_BETWEEN(SYSDATE,Birthdate)/12)"+GetOperandText(elementList[i].Operand)+PIn.Int(compStr);//works with >, <, and =
		//        }
		//        break;
		//      case EhrRestrictionType.Problem:
		//        command+="SELECT disease.patnum FROM disease, diseasedef "
		//          +"WHERE disease.diseaseDefNum=diseaseDef.diseaseDefNum "
		//          +"AND (diseasedef.ICD9Code LIKE '"+PIn.String(compStr)+"%' OR diseasedef.SnomedCode LIKE '"+PIn.String(compStr)+"%') ";
		//        if(elementList[i].StartDate!=null && elementList[i].StartDate!=DateTime.MinValue) {//check start date
		//          if(DataConnection.DBtype==DatabaseType.MySql) {
		//            command+="AND disease.DateStart>'"+elementList[i].StartDate.ToString("yyyy-MM-dd")+"' ";
		//          }
		//          else {
		//            command+="TODO";
		//          }
		//        }
		//        if(elementList[i].EndDate!=null && elementList[i].EndDate!=DateTime.MinValue) {//check end date
		//          if(DataConnection.DBtype==DatabaseType.MySql) {
		//            command+="AND disease.DateStart<'"+elementList[i].EndDate.ToString("yyyy-MM-dd")+"' ";
		//            command+="AND disease.DateStart>'1800-01-01' ";//don't grab minDate values, but also do not remove results from above.
		//          }
		//          else {
		//            command+="TODO";
		//          }
		//        }

		//        //disease.DateStart>"elementList[i];
		//        //command+=",(SELECT disease.ICD9Num FROM disease WHERE disease.PatNum=patient.PatNum AND disease.ICD9Num IN (SELECT ICD9Num FROM icd9 WHERE ICD9Code LIKE '"+compStr+"%') "
		//        //  +DbHelper.LimitAnd(1)+") `"+compStr+"` ";
		//        //command+=",(SELECT diseasedef.ICD9Num, diseasedef.SnomedCode FROM diseasedef,disease WHERE disease.PatNum=patient.PatNum AND diseasedef.diseasedefnum=disease.patnum AND diseaseDef.DiseaseDefNum IN (SELECT diseasedef.diseasedefnum WHERE diseasedef.ICD9Num LIKE '"+compStr+"%' OR diseasedef.SnomedCode LIKE '"+compStr+"%') "
		//        //  +DbHelper.LimitAnd(1)+") `"+compStr+"` ";
		//        break;
		//      case EhrRestrictionType.LabResult:
		//        command+=",(SELECT IFNULL(MAX(ObsValue),0) FROM labresult,labpanel WHERE labresult.LabPanelNum=labpanel.LabPanelNum AND labpanel.PatNum=patient.PatNum AND labresult.TestName='"+compStr+"') `"+compStr+"` ";
		//        break;
		//      case EhrRestrictionType.Medication:
		//        command+=",(SELECT COUNT(*) FROM medication,medicationpat WHERE medicationpat.PatNum=patient.PatNum AND medication.MedicationNum=medicationpat.MedicationNum AND medication.MedName LIKE '%"+compStr+"%') `"+compStr+"` ";
		//        break;
		//      case EhrRestrictionType.Gender:
		//        command+=",patient.Gender ";
		//        break;
		//    }
		//    command+=") ";
		//  }
		//  return retval;
		//}

		//public static DataTable GetListOrderBy(List<EhrPatListElement> elementList,bool isAsc) {
		//	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
		//		return Meth.GetTable(MethodBase.GetCurrentMethod(),elementList,isAsc);
		//	}
		//	DataTable table=new DataTable();
		//	Random rnd=new Random();
		//	string rndStr=rnd.Next(1000000).ToString();
		//	string command="DROP TABLE IF EXISTS tempehrlist"+rndStr+@"";
		//	Db.NonQ(command);
		//	command="CREATE TABLE tempehrlist"+rndStr+@" SELECT patient.PatNum,patient.LName,patient.FName";
		//	for(int i=0;i<elementList.Count;i++) {
		//		string compStr=elementList[i].CompareString;
		//		switch(elementList[i].Restriction) {
		//			case EhrRestrictionType.Birthdate:
		//				command+=",patient.Birthdate ";
		//				break;
		//			case EhrRestrictionType.Problem:
		//				command+=",(SELECT diseasedef.ICD9Code FROM disease,diseasedef "
		//					+"WHERE disease.PatNum=patient.PatNum "
		//					+"AND disease.DiseaseDefNum=diseasedef.DiseaseDefNum "
		//					+"AND diseasedef.Icd9code LIKE '"+compStr+"%') `"+compStr+"` ";
		//				break;
		//			case EhrRestrictionType.LabResult:
		//				command+=",(SELECT IFNULL(MAX(ObsValue),0) FROM labresult,labpanel WHERE labresult.LabPanelNum=labpanel.LabPanelNum AND labpanel.PatNum=patient.PatNum AND labresult.TestName='"+compStr+"') `"+compStr+"` ";
		//				break;
		//			case EhrRestrictionType.Medication:
		//				command+=",(SELECT COUNT(*) FROM medication,medicationpat WHERE medicationpat.PatNum=patient.PatNum AND medication.MedicationNum=medicationpat.MedicationNum AND medication.MedName LIKE '%"+compStr+"%') `"+compStr+"` ";
		//				break;
		//			case EhrRestrictionType.Gender:
		//				command+=",patient.Gender ";
		//				break;
		//		}
		//	}
		//	command+="FROM patient";
		//	Db.NonQ(command);
		//	string order="";
		//	command="SELECT * FROM tempehrlist"+rndStr+@" ";
		//	for(int i=0;i<elementList.Count;i++) {
		//		if(i<1) {
		//			command+="WHERE "+GetFilteringText(elementList[i]);
		//		}
		//		else {
		//			command+="AND "+GetFilteringText(elementList[i]);
		//		}
		//		if(elementList[i].OrderBy) {
		//			if(elementList[i].Restriction==EhrRestrictionType.Birthdate) {
		//				order="ORDER BY Birthdate"+GetOrderBy(isAsc);
		//			}
		//			else if(elementList[i].Restriction==EhrRestrictionType.Gender) {
		//				order="ORDER BY Gender"+GetOrderBy(isAsc);
		//			}
		//			else {
		//				order="ORDER BY `"+POut.String(elementList[i].CompareString)+"`"+GetOrderBy(isAsc);
		//			}
		//		}
		//	}
		//	command+=order;
		//	table=Db.GetTable(command);
		//	command="DROP TABLE IF EXISTS tempehrlist"+rndStr+@"";
		//	Db.NonQ(command);
		//	return table;
		//}

		private static string GetOrderBy(bool isAsc) {
			if(isAsc) {
				return " ASC";
			}
			return " DESC";
		}

		///<summary>Returns lt, gt, or equals</summary>
		private static string GetOperandText(EhrOperand ehrOp){
			string operand="";
			switch(ehrOp) {
				case EhrOperand.Equals:
					operand="=";
					break;
				case EhrOperand.GreaterThan:
					operand=">";
					break;
				case EhrOperand.LessThan:
					operand="<";
					break;
			}
			return operand;
		}

		///<summary>Returns text used in WHERE clause of query for tempehrlist.</summary>
		private static string GetFilteringText(EhrPatListElement element) {
			string filter="";
			string compStr=POut.String(element.CompareString);
			string labStr=POut.String(element.LabValue);
			switch(element.Restriction) {
				case EhrRestrictionType.Birthdate:
					filter="DATE_SUB(CURDATE(),INTERVAL "+compStr+" YEAR)"+GetOperandText(element.Operand)+"Birthdate ";
					break;
				case EhrRestrictionType.Problem:
					filter="`"+compStr+"`"+" IS NOT NULL ";//Has the disease.
					break;
				case EhrRestrictionType.LabResult:
					filter="`"+compStr+"`"+GetOperandText(element.Operand)+labStr+" ";
					break;
				case EhrRestrictionType.Medication:
					filter="`"+compStr+"`"+">0 ";//Count greater than 0 (is taking the med).
					break;
				case EhrRestrictionType.Gender:
					filter="Gender>-1 ";//Just so WHERE clause won't fail.
					break;
			}
			return filter;
		}
	}
}
