using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using Ionic.Zip;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness {
	///<summary>Used in Ehr quality measures.</summary>
	public class QualityMeasures {
		#if DEBUG
		[ThreadStatic]
		private static string _elapsedtimetext;
		#endif
		///<summary>OID: 2.16.840.1.113883.6.1</summary>
		private const string strCodeSystemLoinc="2.16.840.1.113883.6.1";
		///<summary>LOINC</summary>
		private const string strCodeSystemNameLoinc="LOINC";
		///<summary>OID: 2.16.840.1.113883.6.96</summary>
		private const string strCodeSystemSnomed="2.16.840.1.113883.6.96";
		///<summary>SNOMED CT</summary>
		private const string strCodeSystemNameSnomed="SNOMED CT";
		///<summary>OID: 2.16.840.1.113883.6.88</summary>
		///<summary>Set each time GenerateQrda_xml() is called. Used by helper functions to avoid sending the provider as a parameter to each helper function.</summary>
		[ThreadStatic]
		private static Provider _provOutQrda=null;
		///<summary>Instantiated each time GenerateQrda_xml() is called. Used by helper functions to avoid sending the writer as a parameter to each helper function.</summary>
		[ThreadStatic]
		private static XmlWriter _w=null;
		///<summary>Instantiated each time GenerateQrda_xml() is called. Used by helper functions to avoid sending the writer as a parameter to each helper function.  Second writer to generate patient data entries in text form and xml form simultaneously.  The helper functions will take a bool to determine which variable to modify.</summary>
		[ThreadStatic]
		private static XmlWriter _x=null;
		///<summary>This global variable tells the helper functions which of the above XmlWriters to use.  If true, then the _w writer is modified, false=_x.</summary>
		[ThreadStatic]
		private static bool _isWriterW=true;
		//These are the internal OID roots used for "id" entries in GenerateQrda_xml().
		[ThreadStatic]
		private static string _strOIDInternalRoot;
		[ThreadStatic]
		private static string _strOIDInternalCQMRoot;
		[ThreadStatic]
		private static string _strOIDInternalPatRoot;
		[ThreadStatic]
		private static string _strOIDInternalProvRoot;
		///<summary>Instantiated each time GenerateQrda_xml() is called. Used to generate unique "id" element "root" attribute identifiers. The Ids in this list are random GUIDs which are 36 characters in length.  They are used to uniquely identify the QRDA documents.</summary>
		[ThreadStatic]
		private static HashSet<string> _hashQrdaGuids;
		[ThreadStatic]
		private static List<int> _listExtraPopIndxs;
		[ThreadStatic]
		private static long _provNumLegal;

		///<summary>Generates a list of all the quality measures for 2011.  Performs all calculations and manipulations.  Returns list for viewing/output.</summary>
		public static List<QualityMeasure> GetAll(DateTime dateStart,DateTime dateEnd,long provNum) {
			//No remoting role check; no call to db
			//This method will call a method in a loop that has to go over middle tier. However, doing a remoting role check at the beginning of this 
			//method is likely to cause a connection timeout because the quality measure calculations take so long.
			List<QualityMeasure> list=new List<QualityMeasure>();
			//add one of each type
			QualityMeasure measure;
			for(int i=0;i<Enum.GetValues(typeof(QualityType)).Length;i++) {
				measure=new QualityMeasure();
				measure.Type=(QualityType)i;
				measure.Id=GetId(measure.Type);
				measure.Descript=GetDescript(measure.Type);
				DataTable table=GetTable(measure.Type,dateStart,dateEnd,provNum);
				if(table!=null) {
					measure.Denominator=table.Rows.Count;
					measure.Numerator=CalcNumerator(table);
					measure.Exclusions=CalcExclusions(table);
					measure.NotMet=measure.Denominator-measure.Exclusions-measure.Numerator;
					measure.ReportingRate=100;
					measure.PerformanceRate=0;
					if(measure.Numerator > 0) {
						measure.PerformanceRate=(int)((float)(measure.Numerator*100)/(float)(measure.Numerator+measure.NotMet));
					}
					measure.DenominatorExplain=GetDenominatorExplain(measure.Type);
					measure.NumeratorExplain=GetNumeratorExplain(measure.Type);
					measure.ExclusionsExplain=GetExclusionsExplain(measure.Type);
				}
				list.Add(measure);
			}
			return list;
		}

		///<summary>Generates a list of all the quality measures for 2014.  Performs all calculations and manipulations.  Returns list for viewing/output.</summary>
		public static List<QualityMeasure> GetAll2014(DateTime dateStart,DateTime dateEnd,long provNum) {
			//No remoting role check; no call to db
			//This method will call a method in a loop that has to go over middle tier. However, doing a remoting role check at the beginning of this 
			//method is likely to cause a connection timeout because the quality measure calculations take so long.
			#region Get Local Cache of Weight Measure Data
			#region Debug Timer
			System.Diagnostics.Stopwatch s =new System.Diagnostics.Stopwatch();
			System.Diagnostics.Stopwatch stot =new System.Diagnostics.Stopwatch();
			#if DEBUG
				_elapsedtimetext="Elapsed time for each measure.\r\n";
				s.Start();
				stot.Start();
			#endif
			#endregion Debug Timer
			QualityMeasure weightCqmAll=GetEhrCqmData(QualityType2014.WeightChild_1_1,dateStart,dateEnd,provNum);//used for WeightChild_1_x
			#region Debug Timer
			TimeSpan weight1TimeSpan=s.Elapsed;
			if(ODBuild.IsDebug()) {
				s.Stop();
				s.Restart();
			}
			#endregion Debug Timer
			QualityMeasure weightCqm3To11=GetEhrCqmData(QualityType2014.WeightChild_2_1,dateStart,dateEnd,provNum);//used for WeightChild_2_x
			#region Debug Timer
			TimeSpan weight2TimeSpan=s.Elapsed;
			if(ODBuild.IsDebug()) {
				s.Stop();
				s.Restart();
			}
			#endregion Debug Timer
			QualityMeasure weightCqm12To16=GetEhrCqmData(QualityType2014.WeightChild_3_1,dateStart,dateEnd,provNum);//used for WeightChild_3_x
			#region Debug Timer
			TimeSpan weight3TimeSpan=s.Elapsed;
			if(ODBuild.IsDebug()) {
				s.Stop();
				s.Restart();
			}
			#endregion Debug Timer
			#endregion Get Local Cache of Weight Measure Data
			List<QualityMeasure> list=new List<QualityMeasure>();
			//add one of each type
			QualityMeasure measureCur;
			for(int i=0;i<Enum.GetValues(typeof(QualityType2014)).Length;i++) {
				QualityType2014 qtype=(QualityType2014)i;
				switch(qtype) {
					case QualityType2014.WeightChild_1_1:
					case QualityType2014.WeightChild_1_2:
					case QualityType2014.WeightChild_1_3:
						measureCur=weightCqmAll.Copy();
						break;
					case QualityType2014.WeightChild_2_1:
					case QualityType2014.WeightChild_2_2:
					case QualityType2014.WeightChild_2_3:
						measureCur=weightCqm3To11.Copy();
						break;
					case QualityType2014.WeightChild_3_1:
					case QualityType2014.WeightChild_3_2:
					case QualityType2014.WeightChild_3_3:
						measureCur=weightCqm12To16.Copy();
						break;
					default:
						measureCur=GetEhrCqmData(qtype,dateStart,dateEnd,provNum);
						break;
				}
				//this will mark the patients in ListEhrPats as Numerator, Exclusion, or Exception, with explanation
				ClassifyPatients(measureCur,qtype,dateStart,dateEnd);//modifies measureCur dataset
				measureCur.Type2014=qtype;
				measureCur.Id=GetId2014(qtype);
				measureCur.Descript=GetDescript2014(qtype);
				if(measureCur.ListEhrPats==null) {
					list.Add(measureCur.Copy());
					continue;
				}
				//ListEhrPats is not null from here down
				if(qtype==QualityType2014.Influenza) {
					//only have to count IsDenominator pats for influenza measure, all other measures will have every patient marked IsDenominator (denom=initial pat population)
					//influenza measure applies additional restriction to initial pat population, but ListEhrPats will still have IPP in it, but only some will be in denominator
					measureCur.Denominator=CalcDenominator2014(measureCur.ListEhrPats);
				}
				else if(qtype==QualityType2014.MedicationsEntered) {
					//we have to count the number of encounters, not the number of patients, for the denominator count
					measureCur.Denominator=CalcDenominator2014_Encs(measureCur.DictPatNumListEncounters);
				}
				else {
					measureCur.Denominator=measureCur.ListEhrPats.Count;
				}
				if(qtype==QualityType2014.MedicationsEntered) {
					measureCur.Numerator=CalcNumerator2014_Encs(measureCur.DictPatNumListEncounters);
					measureCur.Exceptions=CalcException2014_Encs(measureCur.DictPatNumListEncounters);
					measureCur.Exclusions=0;
				}
				else {
					measureCur.Numerator=CalcNumerator2014(measureCur.ListEhrPats);
					measureCur.Exclusions=CalcExclusions2014(measureCur.ListEhrPats);
					measureCur.Exceptions=CalcExceptions2014(measureCur.ListEhrPats);
				}
				measureCur.NotMet=measureCur.Denominator-measureCur.Exclusions-measureCur.Numerator-measureCur.Exceptions;
				//Reporting rate is (Numerator+Exclusions+Exceptions)/Denominator.  Percentage of qualifying pats classified in one of the three groups Numerator, Exception, Exclusion.
				measureCur.ReportingRate=0;
				if(measureCur.Denominator>0) {
					measureCur.ReportingRate=Math.Round(((decimal)((measureCur.Numerator+measureCur.Exclusions+measureCur.Exceptions)*100)/(decimal)(measureCur.Denominator)),1,MidpointRounding.AwayFromZero);
				}
				//Performance rate is Numerator/(Denominator-Exclusions-Exceptions).  Percentage of qualifying pats (who were not in the Exclusions or Exceptions) who were in the Numerator.
				measureCur.PerformanceRate=0;
				if(measureCur.Numerator>0) {
					measureCur.PerformanceRate=Math.Round(((decimal)(measureCur.Numerator*100)/(decimal)(measureCur.Denominator-measureCur.Exclusions-measureCur.Exceptions)),1,MidpointRounding.AwayFromZero);
				}
				measureCur.DenominatorExplain=GetDenominatorExplain2014(qtype);
				measureCur.NumeratorExplain=GetNumeratorExplain2014(qtype);
				measureCur.ExclusionsExplain=GetExclusionsExplain2014(qtype);
				measureCur.ExceptionsExplain=GetExceptionsExplain2014(qtype);
				measureCur.eMeasureNum=GetEMeasureNum(qtype);
				measureCur.eMeasureTitle=GetEMeasureTitle(qtype);
				measureCur.eMeasureVersion=GetEMeasureVersion(qtype);
				measureCur.eMeasureVNeutralId=GetEMeasureVNeutralId(qtype);
				measureCur.eMeasureVSpecificId=GetEMeasureVSpecificId(qtype);
				measureCur.eMeasureSetId=GetEMeasureSetId(qtype);
				measureCur.eMeasureIppId=GetEMeasureIppId(qtype);
				measureCur.eMeasureDenomId=GetEMeasureDenomId(qtype);
				measureCur.eMeasureDenexId=GetEMeasureDenexId(qtype);
				measureCur.eMeasureDenexcepId=GetEMeasureDenexcepId(qtype);
				measureCur.eMeasureNumerId=GetEMeasureNumerId(qtype);
				list.Add(measureCur.Copy());
			#region Debug Timer
				#if DEBUG
					s.Stop();
					TimeSpan tsMeasure=s.Elapsed;
					if(qtype==QualityType2014.WeightChild_1_1) {
						tsMeasure+=weight1TimeSpan;
					}
					else if(qtype==QualityType2014.WeightChild_2_1) {
						tsMeasure+=weight2TimeSpan;
					}
					else if(qtype==QualityType2014.WeightChild_3_1) {
						tsMeasure+=weight3TimeSpan;
					}
					_elapsedtimetext+=(qtype).ToString()+": "+tsMeasure.ToString()+"\r\n";
					s.Restart();
				#endif
			#endregion Debug Timer
			}
			#region Debug Timer
			#if DEBUG
				stot.Stop();
				_elapsedtimetext+="Total elapsed time: "+stot.Elapsed.ToString();
				System.Windows.Forms.MessageBox.Show(_elapsedtimetext);
			#endif
			#endregion Debug Timer
			return list;
		}

		private static string GetId(QualityType qtype){
			switch(qtype) {
				case QualityType.WeightOver65:
					return "0421a";
				case QualityType.WeightAdult:
					return "0421b";
				case QualityType.Hypertension:
					return "0013";
				case QualityType.TobaccoUse:
					return "0028a";
				case QualityType.TobaccoCessation:
					return "0028b";
				case QualityType.InfluenzaAdult:
					return "0041";
				case QualityType.WeightChild_1_1:
					return "0024-1.1";
				case QualityType.WeightChild_1_2:
					return "0024-1.2";
				case QualityType.WeightChild_1_3:
					return "0024-1.3";
				case QualityType.WeightChild_2_1:
					return "0024-2.1";
				case QualityType.WeightChild_2_2:
					return "0024-2.2";
				case QualityType.WeightChild_2_3:
					return "0024-2.3";
				case QualityType.WeightChild_3_1:
					return "0024-3.1";
				case QualityType.WeightChild_3_2:
					return "0024-3.2";
				case QualityType.WeightChild_3_3:
					return "0024-3.3";
				case QualityType.ImmunizeChild_1:
					return "0038-1";
				case QualityType.ImmunizeChild_2:
					return "0038-2";
				case QualityType.ImmunizeChild_3:
					return "0038-3";
				case QualityType.ImmunizeChild_4:
					return "0038-4";
				case QualityType.ImmunizeChild_5:
					return "0038-5";
				case QualityType.ImmunizeChild_6:
					return "0038-6";
				case QualityType.ImmunizeChild_7:
					return "0038-7";
				case QualityType.ImmunizeChild_8:
					return "0038-8";
				case QualityType.ImmunizeChild_9:
					return "0038-9";
				case QualityType.ImmunizeChild_10:
					return "0038-10";
				case QualityType.ImmunizeChild_11:
					return "0038-11";
				case QualityType.ImmunizeChild_12:
					return "0038-12";		
				case QualityType.Pneumonia:
					return "0043";
				case QualityType.DiabetesBloodPressure:
					return "0061";
				case QualityType.BloodPressureManage:
					return "0018";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetId2014(QualityType2014 qtype) {
			switch(qtype) {
				case QualityType2014.MedicationsEntered:
					return "68";
				case QualityType2014.WeightOver65:
					return "69AgeOver64";
				case QualityType2014.WeightAdult:
					return "69Age18To64";
				case QualityType2014.CariesPrevent:
					return "74All";
				case QualityType2014.CariesPrevent_1:
					return "74Age0To5";
				case QualityType2014.CariesPrevent_2:
					return "74Age6To12";
				case QualityType2014.CariesPrevent_3:
					return "74Age13To20";
				case QualityType2014.ChildCaries:
					return "75";
				case QualityType2014.Pneumonia:
					return "127";
				case QualityType2014.TobaccoCessation:
					return "138";
				case QualityType2014.Influenza:
					return "147";
				case QualityType2014.WeightChild_1_1:
					return "155_1All";
				case QualityType2014.WeightChild_1_2:
					return "155_2All";
				case QualityType2014.WeightChild_1_3:
					return "155_3All";
				case QualityType2014.WeightChild_2_1:
					return "155_1Age3To11";
				case QualityType2014.WeightChild_2_2:
					return "155_2Age3To11";
				case QualityType2014.WeightChild_2_3:
					return "155_3Age3To11";
				case QualityType2014.WeightChild_3_1:
					return "155_1Age12To17";
				case QualityType2014.WeightChild_3_2:
					return "155_2Age12To17";
				case QualityType2014.WeightChild_3_3:
					return "155_3Age12To17";
				case QualityType2014.BloodPressureManage:
					return "165";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		///<summary>Used in reporting, and only for certain types.</summary>
		public static string GetPQRIMeasureNumber(QualityType qtype) {
			switch(qtype) {
				case QualityType.WeightOver65:
					return "128";//"0421";
				case QualityType.Hypertension:
					return "0013";
				case QualityType.TobaccoUse:
					return "114";//"0028";
				case QualityType.InfluenzaAdult:
					return "110";//"0041";
				case QualityType.WeightChild_1_1:
					return "0024";
				case QualityType.ImmunizeChild_1:
					return "0038";
				case QualityType.Pneumonia:
					return "111";//"0043";
				case QualityType.DiabetesBloodPressure:
					return "3";//"0061";
				case QualityType.BloodPressureManage:
					return "0018";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetDescript(QualityType qtype) {
			switch(qtype) {
				case QualityType.WeightOver65:
					return "Weight, Adult, 65+";
				case QualityType.WeightAdult:
					return "Weight, Adult, 18 to 64";
				case QualityType.Hypertension:
					return "Hypertension";
				case QualityType.TobaccoUse:
					return "Tobacco Use Assessment";
				case QualityType.TobaccoCessation:
					return "Tobacco Cessation Intervention";
				case QualityType.InfluenzaAdult:
					return "Influenza Immunization, 50+";
				case QualityType.WeightChild_1_1:
					return "Weight, Child 2-16, BMI";
				case QualityType.WeightChild_1_2:
					return "Weight, Child 2-16, nutrition";
				case QualityType.WeightChild_1_3:
					return "Weight, Child 2-16, physical";
				case QualityType.WeightChild_2_1:
					return "Weight, Child 2-10, BMI";
				case QualityType.WeightChild_2_2:
					return "Weight, Child 2-10, nutrition";
				case QualityType.WeightChild_2_3:
					return "Weight, Child 2-10, physical";
				case QualityType.WeightChild_3_1:
					return "Weight, Child 11-16, BMI";
				case QualityType.WeightChild_3_2:
					return "Weight, Child 11-16, nutrition";
				case QualityType.WeightChild_3_3:
					return "Weight, Child 11-16, physical";
				case QualityType.ImmunizeChild_1:
					return "Immun Status, Child, DTaP";
				case QualityType.ImmunizeChild_2:
					return "Immun Status, Child, IPV";
				case QualityType.ImmunizeChild_3:
					return "Immun Status, Child, MMR";
				case QualityType.ImmunizeChild_4:
					return "Immun Status, Child, HiB";
				case QualityType.ImmunizeChild_5:
					return "Immun Status, Child, hep B";
				case QualityType.ImmunizeChild_6:
					return "Immun Status, Child, VZV";
				case QualityType.ImmunizeChild_7:
					return "Immun Status, Child, pneumococcal";
				case QualityType.ImmunizeChild_8:
					return "Immun Status, Child, hep A";
				case QualityType.ImmunizeChild_9:
					return "Immun Status, Child, rotavirus";
				case QualityType.ImmunizeChild_10:
					return "Immun Status, Child, influenza";
				case QualityType.ImmunizeChild_11:
					return "Immun Status, Child, 1-6";
				case QualityType.ImmunizeChild_12:
					return "Immun Status, Child, 1-7";
				case QualityType.Pneumonia:
					return "Pneumonia Immunization, 64+";
				case QualityType.DiabetesBloodPressure:
					return "Diabetes: BP Management";
				case QualityType.BloodPressureManage:
					return "Controlling High BP";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetDescript2014(QualityType2014 qtype) {
			switch(qtype) {
				case QualityType2014.MedicationsEntered:
					return "Document Current Medications";
				case QualityType2014.WeightOver65:
					return "BMI Screening and Follow-Up, 65+";
				case QualityType2014.WeightAdult:
					return "BMI Screening and Follow-Up, 18-64";
				case QualityType2014.CariesPrevent:
					return "Caries Prevention, 0-20";
				case QualityType2014.CariesPrevent_1:
					return "Caries Prevention, 0-5";
				case QualityType2014.CariesPrevent_2:
					return "Caries Prevention, 6-12";
				case QualityType2014.CariesPrevent_3:
					return "Caries Prevention, 13-20";
				case QualityType2014.ChildCaries:
					return "Child Dental Decay, 0-20";
				case QualityType2014.Pneumonia:
					return "Pneumococcal Vaccination, 65+";
				case QualityType2014.TobaccoCessation:
					return "Tobacco Cessation Intervention";
				case QualityType2014.Influenza:
					return "Influenza Immunization, 6 months+";
				case QualityType2014.WeightChild_1_1:
					return "BMI Assessment, 3-17";
				case QualityType2014.WeightChild_1_2:
					return "Nutrition Counseling, 3-17";
				case QualityType2014.WeightChild_1_3:
					return "Physical Activity Counseling, 3-17";
				case QualityType2014.WeightChild_2_1:
					return "BMI Assessment, 3-11";
				case QualityType2014.WeightChild_2_2:
					return "Nutrition Counseling, 3-11";
				case QualityType2014.WeightChild_2_3:
					return "Physical Activity Counseling, 3-11";
				case QualityType2014.WeightChild_3_1:
					return "BMI Assessment, 12-17";
				case QualityType2014.WeightChild_3_2:
					return "Nutrition Counseling, 12-17";
				case QualityType2014.WeightChild_3_3:
					return "Physical Activity Counseling, 12-17";
				case QualityType2014.BloodPressureManage:
					return "Controlling High BP";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		public static DataTable GetTable(QualityType qtype,DateTime dateStart,DateTime dateEnd,long provNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),qtype,dateStart,dateEnd,provNum);
			}
			//these queries only work for mysql
			Random rnd=new Random();
			string rndStr=rnd.Next(1000000).ToString();
			string command="";
			DataTable tableRaw=new DataTable();
			switch(qtype) {
				#region WeightOver65
				case QualityType.WeightOver65:
					//WeightOver65-------------------------------------------------------------------------------------------------------------------
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					command=@"CREATE TABLE tempehrquality"+rndStr+@" (
						PatNum bigint NOT NULL PRIMARY KEY,
						LName varchar(255) NOT NULL,
						FName varchar(255) NOT NULL,
						DateVisit date NOT NULL DEFAULT '0001-01-01',
						Height float NOT NULL,
						Weight float NOT NULL,
						HasFollowupPlan tinyint NOT NULL,
						IsIneligible tinyint NOT NULL,
						Documentation varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
					command="INSERT INTO tempehrquality"+rndStr+@" (PatNum,LName,FName,DateVisit) SELECT patient.PatNum,LName,FName,"
						+"MAX(ProcDate) "//on the first pass, all we can obtain is the date of the visit
						+"FROM patient "
						+"INNER JOIN procedurelog "//because we want to restrict to only results with procedurelog
						+"ON Patient.PatNum=procedurelog.PatNum "
						+"AND procedurelog.ProcStatus=2 "//complete
						+"AND procedurelog.ProvNum="+POut.Long(provNum)+" "
						+"AND procedurelog.ProcDate >= "+POut.Date(dateStart)+" "
						+"AND procedurelog.ProcDate <= "+POut.Date(dateEnd)+" "
						+"WHERE Birthdate > '1880-01-01' AND Birthdate <= "+POut.Date(DateTime.Today.AddYears(-65))+" "//65 or older
						+"GROUP BY patient.PatNum";//there will frequently be multiple procedurelog events
					Db.NonQ(command);
					//now, find BMIs within 6 months of each visit date. No logic for picking one of multiple BMIs.
					command="UPDATE tempehrquality"+rndStr+@",vitalsign "
						+"SET tempehrquality"+rndStr+@".Height=vitalsign.Height, "
						+"tempehrquality"+rndStr+@".Weight=vitalsign.Weight, "//we could also easily get the BMI date if we wanted.
						+"tempehrquality"+rndStr+@".HasFollowupPlan=vitalsign.HasFollowupPlan, "
						+"tempehrquality"+rndStr+@".IsIneligible=vitalsign.IsIneligible, "
						+"tempehrquality"+rndStr+@".Documentation=vitalsign.Documentation "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND vitalsign.DateTaken <= tempehrquality"+rndStr+@".DateVisit "
						+"AND vitalsign.DateTaken >= DATE_SUB(tempehrquality"+rndStr+@".DateVisit,INTERVAL 6 MONTH)";
					Db.NonQ(command);
					command="SELECT * FROM tempehrquality"+rndStr+@"";
					tableRaw=Db.GetTable(command);
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					break;
				#endregion
				#region WeightAdult
				case QualityType.WeightAdult:
					//WeightAdult---------------------------------------------------------------------------------------------------------------------
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					command=@"CREATE TABLE tempehrquality"+rndStr+@" (
						PatNum bigint NOT NULL PRIMARY KEY,
						LName varchar(255) NOT NULL,
						FName varchar(255) NOT NULL,
						DateVisit date NOT NULL DEFAULT '0001-01-01',
						Height float NOT NULL,
						Weight float NOT NULL,
						HasFollowupPlan tinyint NOT NULL,
						IsIneligible tinyint NOT NULL,
						Documentation varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
					command="INSERT INTO tempehrquality"+rndStr+@" (PatNum,LName,FName,DateVisit) SELECT patient.PatNum,LName,FName,"
						+"MAX(ProcDate) "//on the first pass, all we can obtain is the date of the visit
						+"FROM patient "
						+"INNER JOIN procedurelog "//because we want to restrict to only results with procedurelog
						+"ON Patient.PatNum=procedurelog.PatNum "
						+"AND procedurelog.ProcStatus=2 "//complete
						+"AND procedurelog.ProvNum="+POut.Long(provNum)+" "
						+"AND procedurelog.ProcDate >= "+POut.Date(dateStart)+" "
						+"AND procedurelog.ProcDate <= "+POut.Date(dateEnd)+" "
						+"WHERE Birthdate <= "+POut.Date(DateTime.Today.AddYears(-18))+" "//18+
						+"AND Birthdate > "+POut.Date(DateTime.Today.AddYears(-65))+" "//less than 65
						+"GROUP BY patient.PatNum";//there will frequently be multiple procedurelog events
					Db.NonQ(command);
					//now, find BMIs within 6 months of each visit date. No logic for picking one of multiple BMIs.
					command="UPDATE tempehrquality"+rndStr+@",vitalsign "
						+"SET tempehrquality"+rndStr+@".Height=vitalsign.Height, "
						+"tempehrquality"+rndStr+@".Weight=vitalsign.Weight, "
						+"tempehrquality"+rndStr+@".HasFollowupPlan=vitalsign.HasFollowupPlan, "
						+"tempehrquality"+rndStr+@".IsIneligible=vitalsign.IsIneligible, "
						+"tempehrquality"+rndStr+@".Documentation=vitalsign.Documentation "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND vitalsign.DateTaken <= tempehrquality"+rndStr+@".DateVisit "
						+"AND vitalsign.DateTaken >= DATE_SUB(tempehrquality"+rndStr+@".DateVisit,INTERVAL 6 MONTH)";
					Db.NonQ(command);
					command="SELECT * FROM tempehrquality"+rndStr+@"";
					tableRaw=Db.GetTable(command);
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					break;
				#endregion
				#region Hypertension
				case QualityType.Hypertension:
					//Hypertension---------------------------------------------------------------------------------------------------------------------
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					command=@"CREATE TABLE tempehrquality"+rndStr+@" (
						PatNum bigint NOT NULL PRIMARY KEY,
						LName varchar(255) NOT NULL,
						FName varchar(255) NOT NULL,
						DateVisit date NOT NULL DEFAULT '0001-01-01',
						VisitCount int NOT NULL,
						Icd9Code varchar(255) NOT NULL,
						DateBpEntered date NOT NULL DEFAULT '0001-01-01'
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
					command="INSERT INTO tempehrquality"+rndStr+@" (PatNum,LName,FName,DateVisit,VisitCount,Icd9Code) "
						+"SELECT patient.PatNum,LName,FName,"
						+"MAX(ProcDate), "// most recent visit
						+"COUNT(DISTINCT ProcDate),diseasedef.ICD9Code "
						+"FROM patient "
						+"INNER JOIN procedurelog "
						+"ON Patient.PatNum=procedurelog.PatNum "
						+"AND procedurelog.ProcStatus=2 "//complete
						+"AND procedurelog.ProvNum="+POut.Long(provNum)+" "
						+"LEFT JOIN disease ON disease.PatNum=patient.PatNum "
						+"AND disease.DiseaseDefNum IN (SELECT DiseaseDefNum FROM diseasedef WHERE ICD9Code REGEXP '^40[1-4]') "//starts with 401 through 404
						//+"LEFT JOIN icd9 ON icd9.ICD9Num=disease.ICD9Num "
						+"LEFT JOIN diseasedef ON diseasedef.DiseaseDefNum=disease.DiseaseDefNum "
						//+"AND icd9.ICD9Code REGEXP '^40[1-4]' "//starts with 401 through 404
						+"WHERE Birthdate <= "+POut.Date(DateTime.Today.AddYears(-18))+" "//18+
						+"GROUP BY patient.PatNum";
					Db.NonQ(command);
					//now, find BMIs in measurement period.
					command="UPDATE tempehrquality"+rndStr+@",vitalsign "
						+"SET tempehrquality"+rndStr+@".DateBpEntered=vitalsign.DateTaken "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND vitalsign.BpSystolic != 0 "
						+"AND vitalsign.BpDiastolic != 0 "
						+"AND vitalsign.DateTaken >= "+POut.Date(dateStart)+" "
						+"AND vitalsign.DateTaken <= "+POut.Date(dateEnd);
					Db.NonQ(command);
					command="SELECT * FROM tempehrquality"+rndStr+@"";
					tableRaw=Db.GetTable(command);
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					break;
				#endregion
				#region TobaccoUse
				case QualityType.TobaccoUse:
					//TobaccoUse---------------------------------------------------------------------------------------------------------------------
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					command=@"CREATE TABLE tempehrquality"+rndStr+@" (
						PatNum bigint NOT NULL PRIMARY KEY,
						LName varchar(255) NOT NULL,
						FName varchar(255) NOT NULL,
						DateVisit date NOT NULL DEFAULT '0001-01-01',
						VisitCount int NOT NULL,
						DateAssessment date NOT NULL DEFAULT '0001-01-01'
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
					command="INSERT INTO tempehrquality"+rndStr+@" (PatNum,LName,FName,DateVisit,VisitCount) "
						+"SELECT patient.PatNum,LName,FName,"
						+"MAX(ProcDate), "// most recent visit
						+"COUNT(DISTINCT ProcDate) "
						+"FROM patient "
						+"INNER JOIN procedurelog "
						+"ON Patient.PatNum=procedurelog.PatNum "
						+"AND procedurelog.ProcStatus=2 "//complete
						+"AND procedurelog.ProvNum="+POut.Long(provNum)+" "
						+"WHERE Birthdate <= "+POut.Date(DateTime.Today.AddYears(-18))+" "//18+
						+"GROUP BY patient.PatNum";
					Db.NonQ(command);
					//now, find most recent tobacco assessment date.  We will check later that it is within 2 years of last exam.
					command="UPDATE tempehrquality"+rndStr+@" "//,ehrmeasureevent "
						+"SET tempehrquality"+rndStr+@".DateAssessment=(SELECT MAX(DATE(ehrmeasureevent.DateTEvent)) "
						+"FROM ehrmeasureevent "
						+"WHERE tempehrquality"+rndStr+@".PatNum=ehrmeasureevent.PatNum "
						+"AND ehrmeasureevent.EventType="+POut.Int((int)EhrMeasureEventType.TobaccoUseAssessed)+")";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@" SET DateAssessment='0001-01-01' WHERE DateAssessment='0000-00-00'";
					Db.NonQ(command);
					command="SELECT * FROM tempehrquality"+rndStr+@"";
					tableRaw=Db.GetTable(command);
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					break;
				#endregion
				#region TobaccoCessation
				case QualityType.TobaccoCessation:
					//TobaccoCessation----------------------------------------------------------------------------------------------------------------
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					command=@"CREATE TABLE tempehrquality"+rndStr+@" (
						PatNum bigint NOT NULL PRIMARY KEY,
						LName varchar(255) NOT NULL,
						FName varchar(255) NOT NULL,
						DateVisit date NOT NULL DEFAULT '0001-01-01',
						DateAssessment date NOT NULL DEFAULT '0001-01-01',
						DateCessation date NOT NULL DEFAULT '0001-01-01',
						Documentation varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
					command="INSERT INTO tempehrquality"+rndStr+@" (PatNum,LName,FName,DateVisit) "
						+"SELECT patient.PatNum,LName,FName,"
						+"MAX(ProcDate) "// most recent visit
						+"FROM patient "
						+"INNER JOIN procedurelog "
						+"ON Patient.PatNum=procedurelog.PatNum "
						+"AND procedurelog.ProcStatus=2 "//complete
						+"AND procedurelog.ProvNum="+POut.Long(provNum)+" "
						+"WHERE Birthdate <= "+POut.Date(DateTime.Today.AddYears(-18))+" "//18+
						+"AND patient.SmokingSnoMed IN('"+POut.String(SmokingSnoMed._449868002.ToString().Substring(1))+"','"
						+POut.String(SmokingSnoMed._428041000124106.ToString().Substring(1))+"','"
						+POut.String(SmokingSnoMed._428061000124105.ToString().Substring(1))+"','"
						+POut.String(SmokingSnoMed._428071000124103.ToString().Substring(1))+"') "//CurrentEveryDay,CurrentSomeDay,LightSmoker,HeavySmoker
						+"GROUP BY patient.PatNum";
					Db.NonQ(command);
					//find most recent tobacco assessment date.
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET tempehrquality"+rndStr+@".DateAssessment=(SELECT MAX(DATE(ehrmeasureevent.DateTEvent)) "
						+"FROM ehrmeasureevent "
						+"WHERE tempehrquality"+rndStr+@".PatNum=ehrmeasureevent.PatNum "
						+"AND ehrmeasureevent.EventType="+POut.Int((int)EhrMeasureEventType.TobaccoUseAssessed)+")";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@" SET DateAssessment='0001-01-01' WHERE DateAssessment='0000-00-00'";
					Db.NonQ(command);
					//find most recent tobacco cessation date.
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET tempehrquality"+rndStr+@".DateCessation=(SELECT MAX(DATE(ehrmeasureevent.DateTEvent)) "
						+"FROM ehrmeasureevent "
						+"WHERE tempehrquality"+rndStr+@".PatNum=ehrmeasureevent.PatNum "
						+"AND ehrmeasureevent.EventType="+POut.Int((int)EhrMeasureEventType.TobaccoCessation)+")";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@" SET DateCessation='0001-01-01' WHERE DateCessation='0000-00-00'";
					Db.NonQ(command);
					//Pull the documentation based on date
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET Documentation=(SELECT ehrmeasureevent.MoreInfo "
						+"FROM ehrmeasureevent "
						+"WHERE tempehrquality"+rndStr+@".PatNum=ehrmeasureevent.PatNum "
						+"AND ehrmeasureevent.EventType="+POut.Int((int)EhrMeasureEventType.TobaccoCessation)+" "
						+"AND DATE(ehrmeasureevent.DateTEvent)=tempehrquality"+rndStr+@".DateCessation) "
						+"WHERE DateCessation > '1880-01-01'";
					Db.NonQ(command);
					command="SELECT * FROM tempehrquality"+rndStr+@"";
					tableRaw=Db.GetTable(command);
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					break;
				#endregion
				#region InfluenzaAdult
				case QualityType.InfluenzaAdult:
					//InfluenzaAdult----------------------------------------------------------------------------------------------------------------
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					command=@"CREATE TABLE tempehrquality"+rndStr+@" (
						PatNum bigint NOT NULL PRIMARY KEY,
						LName varchar(255) NOT NULL,
						FName varchar(255) NOT NULL,
						DateVaccine date NOT NULL,
						NotGiven tinyint NOT NULL,
						Documentation varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
					command="INSERT INTO tempehrquality"+rndStr+@" (PatNum,LName,FName) SELECT patient.PatNum,LName,FName "
						+"FROM patient "
						+"INNER JOIN procedurelog "
						+"ON Patient.PatNum=procedurelog.PatNum "
						+"AND procedurelog.ProcStatus=2 "//complete
						+"AND procedurelog.ProvNum="+POut.Long(provNum)+" "
						+"AND procedurelog.ProcDate >= "+POut.Date(dateStart)+" "
						+"AND procedurelog.ProcDate <= "+POut.Date(dateEnd)+" "
						+"WHERE Birthdate > '1880-01-01' AND Birthdate <= "+POut.Date(DateTime.Today.AddYears(-50))+" "//50 or older
						+"GROUP BY patient.PatNum";
					Db.NonQ(command);
					//find most recent vaccine date
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET tempehrquality"+rndStr+@".DateVaccine=(SELECT MAX(DATE(vaccinepat.DateTimeStart)) "
						+"FROM vaccinepat,vaccinedef "
						+"WHERE vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"AND tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND vaccinedef.CVXCode IN('135','15'))";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@" SET DateVaccine='0001-01-01' WHERE DateVaccine='0000-00-00'";
					Db.NonQ(command);
					//pull documentation on vaccine exclusions based on date.
					command="UPDATE tempehrquality"+rndStr+@",vaccinepat,vaccinedef "
						+"SET Documentation=Note, "
						+"tempehrquality"+rndStr+@".NotGiven=vaccinepat.NotGiven "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"AND DATE(vaccinepat.DateTimeStart)=tempehrquality"+rndStr+@".DateVaccine "
						+"AND vaccinedef.CVXCode IN('135','15')";
					Db.NonQ(command);
					command="SELECT * FROM tempehrquality"+rndStr+@"";
					tableRaw=Db.GetTable(command);
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					break;
				#endregion
				#region WeightChild_1
				case QualityType.WeightChild_1_1:
				case QualityType.WeightChild_1_2:
				case QualityType.WeightChild_1_3:
					//WeightChild_1-----------------------------------------------------------------------------------------------------------------
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					command=@"CREATE TABLE tempehrquality"+rndStr+@" (
						PatNum bigint NOT NULL PRIMARY KEY,
						LName varchar(255) NOT NULL,
						FName varchar(255) NOT NULL,
						IsPregnant tinyint NOT NULL,
						HasBMI tinyint NOT NULL,
						ChildGotNutrition tinyint NOT NULL,
						ChildGotPhysCouns tinyint NOT NULL				
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
					command="INSERT INTO tempehrquality"+rndStr+@" (PatNum,LName,FName) SELECT patient.PatNum,LName,FName "
						+"FROM patient "
						+"INNER JOIN procedurelog "
						+"ON Patient.PatNum=procedurelog.PatNum "
						+"AND procedurelog.ProcStatus=2 "//complete
						+"AND procedurelog.ProvNum="+POut.Long(provNum)+" "
						+"AND procedurelog.ProcDate >= "+POut.Date(dateStart)+" "
						+"AND procedurelog.ProcDate <= "+POut.Date(dateEnd)+" "
						+"WHERE Birthdate <= "+POut.Date(DateTime.Today.AddYears(-2))+" "//2+
						+"AND Birthdate > "+POut.Date(DateTime.Today.AddYears(-17))+" "//less than 17
						+"GROUP BY patient.PatNum";//there will frequently be multiple procedurelog events
					Db.NonQ(command);
					//find any BMIs within the period that indicate pregnancy
					command="UPDATE tempehrquality"+rndStr+@",vitalsign "
						+"SET tempehrquality"+rndStr+@".IsPregnant=1 "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND vitalsign.DateTaken >= "+POut.Date(dateStart)+" "
						+"AND vitalsign.DateTaken <= "+POut.Date(dateEnd)+" "
						+"AND vitalsign.IsIneligible=1";
					Db.NonQ(command);
					//find any BMIs within the period with a valid BMI
					command="UPDATE tempehrquality"+rndStr+@",vitalsign "
						+"SET tempehrquality"+rndStr+@".HasBMI=1 "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND vitalsign.DateTaken >= "+POut.Date(dateStart)+" "
						+"AND vitalsign.DateTaken <= "+POut.Date(dateEnd)+" "
						+"AND vitalsign.Height > 0 "
						+"AND vitalsign.Weight > 0";
					Db.NonQ(command);
					//find any BMIs within the period that indicate ChildGotNutrition
					command="UPDATE tempehrquality"+rndStr+@",vitalsign "
						+"SET tempehrquality"+rndStr+@".ChildGotNutrition=1 "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND vitalsign.DateTaken >= "+POut.Date(dateStart)+" "
						+"AND vitalsign.DateTaken <= "+POut.Date(dateEnd)+" "
						+"AND vitalsign.ChildGotNutrition=1";
					Db.NonQ(command);
					//find any BMIs within the period that indicate ChildGotPhysCouns
					command="UPDATE tempehrquality"+rndStr+@",vitalsign "
						+"SET tempehrquality"+rndStr+@".ChildGotPhysCouns=1 "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND vitalsign.DateTaken >= "+POut.Date(dateStart)+" "
						+"AND vitalsign.DateTaken <= "+POut.Date(dateEnd)+" "
						+"AND vitalsign.ChildGotPhysCouns=1";
					Db.NonQ(command);
					command="SELECT * FROM tempehrquality"+rndStr+@"";
					tableRaw=Db.GetTable(command);
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					break;
				#endregion
				#region WeightChild_2
				case QualityType.WeightChild_2_1:
				case QualityType.WeightChild_2_2:
				case QualityType.WeightChild_2_3:
					//WeightChild_2-----------------------------------------------------------------------------------------------------------------
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					command=@"CREATE TABLE tempehrquality"+rndStr+@" (
						PatNum bigint NOT NULL PRIMARY KEY,
						LName varchar(255) NOT NULL,
						FName varchar(255) NOT NULL,
						IsPregnant tinyint NOT NULL,
						HasBMI tinyint NOT NULL,
						ChildGotNutrition tinyint NOT NULL,
						ChildGotPhysCouns tinyint NOT NULL				
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
					command="INSERT INTO tempehrquality"+rndStr+@" (PatNum,LName,FName) SELECT patient.PatNum,LName,FName "
						+"FROM patient "
						+"INNER JOIN procedurelog "
						+"ON Patient.PatNum=procedurelog.PatNum "
						+"AND procedurelog.ProcStatus=2 "//complete
						+"AND procedurelog.ProvNum="+POut.Long(provNum)+" "
						+"AND procedurelog.ProcDate >= "+POut.Date(dateStart)+" "
						+"AND procedurelog.ProcDate <= "+POut.Date(dateEnd)+" "
						+"WHERE Birthdate <= "+POut.Date(DateTime.Today.AddYears(-2))+" "//2+
						+"AND Birthdate > "+POut.Date(DateTime.Today.AddYears(-11))+" "//less than 11
						+"GROUP BY patient.PatNum";//there will frequently be multiple procedurelog events
					Db.NonQ(command);
					//find any BMIs within the period that indicate pregnancy
					command="UPDATE tempehrquality"+rndStr+@",vitalsign "
						+"SET tempehrquality"+rndStr+@".IsPregnant=1 "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND vitalsign.DateTaken >= "+POut.Date(dateStart)+" "
						+"AND vitalsign.DateTaken <= "+POut.Date(dateEnd)+" "
						+"AND vitalsign.IsIneligible=1";
					Db.NonQ(command);
					//find any BMIs within the period with a valid BMI
					command="UPDATE tempehrquality"+rndStr+@",vitalsign "
						+"SET tempehrquality"+rndStr+@".HasBMI=1 "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND vitalsign.DateTaken >= "+POut.Date(dateStart)+" "
						+"AND vitalsign.DateTaken <= "+POut.Date(dateEnd)+" "
						+"AND vitalsign.Height > 0 "
						+"AND vitalsign.Weight > 0";
					Db.NonQ(command);
					//find any BMIs within the period that indicate ChildGotNutrition
					command="UPDATE tempehrquality"+rndStr+@",vitalsign "
						+"SET tempehrquality"+rndStr+@".ChildGotNutrition=1 "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND vitalsign.DateTaken >= "+POut.Date(dateStart)+" "
						+"AND vitalsign.DateTaken <= "+POut.Date(dateEnd)+" "
						+"AND vitalsign.ChildGotNutrition=1";
					Db.NonQ(command);
					//find any BMIs within the period that indicate ChildGotPhysCouns
					command="UPDATE tempehrquality"+rndStr+@",vitalsign "
						+"SET tempehrquality"+rndStr+@".ChildGotPhysCouns=1 "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND vitalsign.DateTaken >= "+POut.Date(dateStart)+" "
						+"AND vitalsign.DateTaken <= "+POut.Date(dateEnd)+" "
						+"AND vitalsign.ChildGotPhysCouns=1";
					Db.NonQ(command);
					command="SELECT * FROM tempehrquality"+rndStr+@"";
					tableRaw=Db.GetTable(command);
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					break;
				#endregion
				#region WeightChild_3
				case QualityType.WeightChild_3_1:
				case QualityType.WeightChild_3_2:
				case QualityType.WeightChild_3_3:
					//WeightChild_3-----------------------------------------------------------------------------------------------------------------
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					command=@"CREATE TABLE tempehrquality"+rndStr+@" (
						PatNum bigint NOT NULL PRIMARY KEY,
						LName varchar(255) NOT NULL,
						FName varchar(255) NOT NULL,
						IsPregnant tinyint NOT NULL,
						HasBMI tinyint NOT NULL,
						ChildGotNutrition tinyint NOT NULL,
						ChildGotPhysCouns tinyint NOT NULL			
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
					command="INSERT INTO tempehrquality"+rndStr+@" (PatNum,LName,FName) SELECT patient.PatNum,LName,FName "
						+"FROM patient "
						+"INNER JOIN procedurelog "
						+"ON Patient.PatNum=procedurelog.PatNum "
						+"AND procedurelog.ProcStatus=2 "//complete
						+"AND procedurelog.ProvNum="+POut.Long(provNum)+" "
						+"AND procedurelog.ProcDate >= "+POut.Date(dateStart)+" "
						+"AND procedurelog.ProcDate <= "+POut.Date(dateEnd)+" "
						+"WHERE Birthdate <= "+POut.Date(DateTime.Today.AddYears(-11))+" "//11+
						+"AND Birthdate > "+POut.Date(DateTime.Today.AddYears(-17))+" "//less than 17
						+"GROUP BY patient.PatNum";//there will frequently be multiple procedurelog events
					Db.NonQ(command);
					//find any BMIs within the period that indicate pregnancy
					command="UPDATE tempehrquality"+rndStr+@",vitalsign "
						+"SET tempehrquality"+rndStr+@".IsPregnant=1 "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND vitalsign.DateTaken >= "+POut.Date(dateStart)+" "
						+"AND vitalsign.DateTaken <= "+POut.Date(dateEnd)+" "
						+"AND vitalsign.IsIneligible=1";
					Db.NonQ(command);
					//find any BMIs within the period with a valid BMI
					command="UPDATE tempehrquality"+rndStr+@",vitalsign "
						+"SET tempehrquality"+rndStr+@".HasBMI=1 "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND vitalsign.DateTaken >= "+POut.Date(dateStart)+" "
						+"AND vitalsign.DateTaken <= "+POut.Date(dateEnd)+" "
						+"AND vitalsign.Height > 0 "
						+"AND vitalsign.Weight > 0";
					Db.NonQ(command);
					//find any BMIs within the period that indicate ChildGotNutrition
					command="UPDATE tempehrquality"+rndStr+@",vitalsign "
						+"SET tempehrquality"+rndStr+@".ChildGotNutrition=1 "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND vitalsign.DateTaken >= "+POut.Date(dateStart)+" "
						+"AND vitalsign.DateTaken <= "+POut.Date(dateEnd)+" "
						+"AND vitalsign.ChildGotNutrition=1";
					Db.NonQ(command);
					//find any BMIs within the period that indicate ChildGotPhysCouns
					command="UPDATE tempehrquality"+rndStr+@",vitalsign "
						+"SET tempehrquality"+rndStr+@".ChildGotPhysCouns=1 "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND vitalsign.DateTaken >= "+POut.Date(dateStart)+" "
						+"AND vitalsign.DateTaken <= "+POut.Date(dateEnd)+" "
						+"AND vitalsign.ChildGotPhysCouns=1";
					Db.NonQ(command);
					command="SELECT * FROM tempehrquality"+rndStr+@"";
					tableRaw=Db.GetTable(command);
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					break;
				#endregion
				#region ImmunizeChild
				case QualityType.ImmunizeChild_1:
				case QualityType.ImmunizeChild_2:
				case QualityType.ImmunizeChild_3:
				case QualityType.ImmunizeChild_4:
				case QualityType.ImmunizeChild_5:
				case QualityType.ImmunizeChild_6:
				case QualityType.ImmunizeChild_7:
				case QualityType.ImmunizeChild_8:
				case QualityType.ImmunizeChild_9:
				case QualityType.ImmunizeChild_10:
				case QualityType.ImmunizeChild_11:
				case QualityType.ImmunizeChild_12:
					//ImmunizeChild----------------------------------------------------------------------------------------------------------------
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					command=@"CREATE TABLE tempehrquality"+rndStr+@" (
						PatNum bigint NOT NULL PRIMARY KEY,
						LName varchar(255) NOT NULL,
						FName varchar(255) NOT NULL,
						Birthdate date NOT NULL,
						Count1 tinyint NOT NULL,
						NotGiven1 tinyint NOT NULL,
						Documentation1 varchar(255) NOT NULL,
						Count2 tinyint NOT NULL,
						NotGiven2 tinyint NOT NULL,
						Documentation2 varchar(255) NOT NULL,
						Count3 tinyint NOT NULL,
						NotGiven3 tinyint NOT NULL,
						Documentation3 varchar(255) NOT NULL,
						Count3a tinyint NOT NULL,
						NotGiven3a tinyint NOT NULL,
						Documentation3a varchar(255) NOT NULL,
						Count3b tinyint NOT NULL,
						NotGiven3b tinyint NOT NULL,
						Documentation3b varchar(255) NOT NULL,
						Count3c tinyint NOT NULL,
						NotGiven3c tinyint NOT NULL,
						Documentation3c varchar(255) NOT NULL,
						Count4 tinyint NOT NULL,
						NotGiven4 tinyint NOT NULL,
						Documentation4 varchar(255) NOT NULL,
						Count5 tinyint NOT NULL,
						NotGiven5 tinyint NOT NULL,
						Documentation5 varchar(255) NOT NULL,
						Count6 tinyint NOT NULL,
						NotGiven6 tinyint NOT NULL,
						Documentation6 varchar(255) NOT NULL,
						Count7 tinyint NOT NULL,
						NotGiven7 tinyint NOT NULL,
						Documentation7 varchar(255) NOT NULL,
						Count8 tinyint NOT NULL,
						NotGiven8 tinyint NOT NULL,
						Documentation8 varchar(255) NOT NULL,
						Count9 tinyint NOT NULL,
						NotGiven9 tinyint NOT NULL,
						Documentation9 varchar(255) NOT NULL,
						Count10 tinyint NOT NULL,
						NotGiven10 tinyint NOT NULL,
						Documentation10 varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
					command="INSERT INTO tempehrquality"+rndStr+@" (PatNum,LName,FName,Birthdate) SELECT patient.PatNum,LName,FName,Birthdate "
						+"FROM patient "
						+"INNER JOIN procedurelog "
						+"ON Patient.PatNum=procedurelog.PatNum "
						+"AND procedurelog.ProcStatus=2 "//complete
						+"AND procedurelog.ProvNum="+POut.Long(provNum)+" "
						+"AND procedurelog.ProcDate >= "+POut.Date(dateStart)+" "
						+"AND procedurelog.ProcDate <= "+POut.Date(dateEnd)+" "
						+"WHERE DATE_ADD(Birthdate,INTERVAL 2 YEAR) >= "+POut.Date(dateStart)+" "//second birthdate is in meas period
						+"AND DATE_ADD(Birthdate,INTERVAL 2 YEAR) <= "+POut.Date(dateEnd)+" "
						+"GROUP BY patient.PatNum";
					Db.NonQ(command);
					#region DTaP
					//Count1, DTaP
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET Count1=(SELECT COUNT(DISTINCT VaccinePatNum) FROM vaccinepat "
						+"LEFT JOIN vaccinedef ON vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=0 "
						+"AND DATE(DateTimeStart) >= DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 42 DAY) "
						+"AND DATE(DateTimeStart) < DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 2 YEAR) "
						+"AND vaccinedef.CVXCode IN('110','120','20','50'))";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@",vaccinepat,vaccinedef "
						+"SET NotGiven1=1,Documentation1=Note "
						+"WHERE vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"AND tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=1 "
						+"AND vaccinedef.CVXCode IN('110','120','20','50')";
					Db.NonQ(command);
					#endregion
					#region IPV
					//Count2, IPV
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET Count2=(SELECT COUNT(DISTINCT VaccinePatNum) FROM vaccinepat "
						+"LEFT JOIN vaccinedef ON vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=0 "
						+"AND DATE(DateTimeStart) >= DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 42 DAY) "
						+"AND DATE(DateTimeStart) < DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 2 YEAR) "
						+"AND vaccinedef.CVXCode IN('10','120'))";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@",vaccinepat,vaccinedef "
						+"SET NotGiven2=1,Documentation2=Note "
						+"WHERE vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"AND tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=1 "
						+"AND vaccinedef.CVXCode IN('10','120')";
					Db.NonQ(command);
					#endregion
					#region MMR
					//Count3, MMR
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET Count3=(SELECT COUNT(DISTINCT VaccinePatNum) FROM vaccinepat "
						+"LEFT JOIN vaccinedef ON vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=0 "
						+"AND DATE(DateTimeStart) < DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 2 YEAR) "
						+"AND vaccinedef.CVXCode IN('03','94'))";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@",vaccinepat,vaccinedef "
						+"SET NotGiven3=1,Documentation3=Note "
						+"WHERE vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"AND tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=1 "
						+"AND vaccinedef.CVXCode IN('03','94')";
					Db.NonQ(command);
					#endregion
					#region measles
					//Count3a, measles
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET Count3a=(SELECT COUNT(DISTINCT VaccinePatNum) FROM vaccinepat "
						+"LEFT JOIN vaccinedef ON vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=0 "
						+"AND DATE(DateTimeStart) < DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 2 YEAR) "
						+"AND vaccinedef.CVXCode IN('05'))";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@",vaccinepat,vaccinedef "
						+"SET NotGiven3a=1,Documentation3a=Note "
						+"WHERE vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"AND tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=1 "
						+"AND vaccinedef.CVXCode IN('05')";
					Db.NonQ(command);
					#endregion
					#region mumps
					//Count3b, mumps
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET Count3b=(SELECT COUNT(DISTINCT VaccinePatNum) FROM vaccinepat "
						+"LEFT JOIN vaccinedef ON vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=0 "
						+"AND DATE(DateTimeStart) < DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 2 YEAR) "
						+"AND vaccinedef.CVXCode IN('07'))";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@",vaccinepat,vaccinedef "
						+"SET NotGiven3b=1,Documentation3b=Note "
						+"WHERE vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"AND tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=1 "
						+"AND vaccinedef.CVXCode IN('07')";
					Db.NonQ(command);
					#endregion
					#region rubella
					//Count3c, rubella
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET Count3c=(SELECT COUNT(DISTINCT VaccinePatNum) FROM vaccinepat "
						+"LEFT JOIN vaccinedef ON vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=0 "
						+"AND DATE(DateTimeStart) < DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 2 YEAR) "
						+"AND vaccinedef.CVXCode IN('06'))";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@",vaccinepat,vaccinedef "
						+"SET NotGiven3c=1,Documentation3c=Note "
						+"WHERE vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"AND tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=1 "
						+"AND vaccinedef.CVXCode IN('06')";
					Db.NonQ(command);
					#endregion
					#region HiB
					//Count4, HiB
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET Count4=(SELECT COUNT(DISTINCT VaccinePatNum) FROM vaccinepat "
						+"LEFT JOIN vaccinedef ON vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=0 "
						+"AND DATE(DateTimeStart) >= DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 42 DAY) "
						+"AND DATE(DateTimeStart) < DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 2 YEAR) "
						+"AND vaccinedef.CVXCode IN('120','46','47','48','49','50','51'))";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@",vaccinepat,vaccinedef "
						+"SET NotGiven4=1,Documentation4=Note "
						+"WHERE vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"AND tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=1 "
						+"AND vaccinedef.CVXCode IN('120','46','47','48','49','50','51')";
					Db.NonQ(command);
					#endregion
					#region HepB
					//Count5, HepB
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET Count5=(SELECT COUNT(DISTINCT VaccinePatNum) FROM vaccinepat "
						+"LEFT JOIN vaccinedef ON vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=0 "
						+"AND DATE(DateTimeStart) < DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 2 YEAR) "
						+"AND vaccinedef.CVXCode IN('08','110','44','51'))";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@",vaccinepat,vaccinedef "
						+"SET NotGiven5=1,Documentation5=Note "
						+"WHERE vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"AND tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=1 "
						+"AND vaccinedef.CVXCode IN('08','110','44','51')";
					Db.NonQ(command);
					#endregion
					#region VZV
					//Count6, VZV
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET Count6=(SELECT COUNT(DISTINCT VaccinePatNum) FROM vaccinepat "
						+"LEFT JOIN vaccinedef ON vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=0 "
						+"AND DATE(DateTimeStart) < DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 2 YEAR) "
						+"AND vaccinedef.CVXCode IN('21','94'))";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@",vaccinepat,vaccinedef "
						+"SET NotGiven6=1,Documentation6=Note "
						+"WHERE vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"AND tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=1 "
						+"AND vaccinedef.CVXCode IN('21','94')";
					Db.NonQ(command);
					#endregion
					#region pneumococcal
					//Count7, pneumococcal
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET Count7=(SELECT COUNT(DISTINCT VaccinePatNum) FROM vaccinepat "
						+"LEFT JOIN vaccinedef ON vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=0 "
						+"AND DATE(DateTimeStart) >= DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 42 DAY) "
						+"AND DATE(DateTimeStart) < DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 2 YEAR) "
						+"AND vaccinedef.CVXCode IN('100','133'))";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@",vaccinepat,vaccinedef "
						+"SET NotGiven7=1,Documentation7=Note "
						+"WHERE vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"AND tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=1 "
						+"AND vaccinedef.CVXCode IN('100','133')";
					Db.NonQ(command);
					#endregion
					#region HepA
					//Count8, HepA
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET Count8=(SELECT COUNT(DISTINCT VaccinePatNum) FROM vaccinepat "
						+"LEFT JOIN vaccinedef ON vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=0 "
						+"AND DATE(DateTimeStart) < DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 2 YEAR) "
						+"AND vaccinedef.CVXCode IN('83'))";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@",vaccinepat,vaccinedef "
						+"SET NotGiven8=1,Documentation8=Note "
						+"WHERE vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"AND tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=1 "
						+"AND vaccinedef.CVXCode IN('83')";
					Db.NonQ(command);
					#endregion
					#region rotavirus
					//Count9, rotavirus
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET Count9=(SELECT COUNT(DISTINCT VaccinePatNum) FROM vaccinepat "
						+"LEFT JOIN vaccinedef ON vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=0 "
						+"AND DATE(DateTimeStart) >= DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 42 DAY) "
						+"AND DATE(DateTimeStart) < DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 2 YEAR) "
						+"AND vaccinedef.CVXCode IN('116','119'))";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@",vaccinepat,vaccinedef "
						+"SET NotGiven9=1,Documentation9=Note "
						+"WHERE vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"AND tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=1 "
						+"AND vaccinedef.CVXCode IN('116','119')";
					Db.NonQ(command);
					#endregion
					#region influenza
					//Count10, influenza
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET Count10=(SELECT COUNT(DISTINCT VaccinePatNum) FROM vaccinepat "
						+"LEFT JOIN vaccinedef ON vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=0 "
						+"AND DATE(DateTimeStart) >= DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 180 DAY) "
						+"AND DATE(DateTimeStart) < DATE_ADD(tempehrquality"+rndStr+@".Birthdate,INTERVAL 2 YEAR) "
						+"AND vaccinedef.CVXCode IN('135','15'))";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@",vaccinepat,vaccinedef "
						+"SET NotGiven10=1,Documentation10=Note "
						+"WHERE vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"AND tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND NotGiven=1 "
						+"AND vaccinedef.CVXCode IN('135','15')";
					Db.NonQ(command);
					#endregion
					command="SELECT * FROM tempehrquality"+rndStr+@"";
					tableRaw=Db.GetTable(command);
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					break;
				#endregion
				#region Pneumonia
				case QualityType.Pneumonia:
					//Pneumonia----------------------------------------------------------------------------------------------------------------
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					command=@"CREATE TABLE tempehrquality"+rndStr+@" (
						PatNum bigint NOT NULL PRIMARY KEY,
						LName varchar(255) NOT NULL,
						FName varchar(255) NOT NULL,
						DateVaccine date NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
					command="INSERT INTO tempehrquality"+rndStr+@" (PatNum,LName,FName) SELECT patient.PatNum,LName,FName "
						+"FROM patient "
						+"INNER JOIN procedurelog "
						+"ON Patient.PatNum=procedurelog.PatNum "
						+"AND procedurelog.ProcStatus=2 "//complete
						+"AND procedurelog.ProvNum="+POut.Long(provNum)+" "
						+"AND procedurelog.ProcDate >= "+POut.Date(dateEnd.AddYears(-1))+" "
						+"AND procedurelog.ProcDate <= "+POut.Date(dateEnd)+" "
						+"WHERE Birthdate > '1880-01-01' AND Birthdate <= "+POut.Date(dateStart.AddYears(-65))+" "//65 or older as of dateEnd
						+"GROUP BY patient.PatNum";
					Db.NonQ(command);
					//find most recent vaccine date
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET tempehrquality"+rndStr+@".DateVaccine=(SELECT MAX(DATE(vaccinepat.DateTimeStart)) "
						+"FROM vaccinepat,vaccinedef "
						+"WHERE vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
						+"AND tempehrquality"+rndStr+@".PatNum=vaccinepat.PatNum "
						+"AND vaccinepat.NotGiven=0 "
						+"AND vaccinedef.CVXCode IN('33','100','133'))";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@" SET DateVaccine='0001-01-01' WHERE DateVaccine='0000-00-00'";
					Db.NonQ(command);
					command="SELECT * FROM tempehrquality"+rndStr+@"";
					tableRaw=Db.GetTable(command);
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					break;
				#endregion
				#region DiabetesBloodPressure
				case QualityType.DiabetesBloodPressure:
					//DiabetesBloodPressure-------------------------------------------------------------------------------------------------------
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					command=@"CREATE TABLE tempehrquality"+rndStr+@" (
						PatNum bigint NOT NULL PRIMARY KEY,
						LName varchar(255) NOT NULL,
						FName varchar(255) NOT NULL,
						HasMedication tinyint NOT NULL,
						HasDiagnosisDiabetes tinyint NOT NULL,
						DateBP date NOT NULL,
						Systolic int NOT NULL,
						Diastolic int NOT NULL,
						HasDiagnosisPolycystic tinyint NOT NULL,
						HasDiagnosisAcuteDiabetes tinyint NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
					command="INSERT INTO tempehrquality"+rndStr+@" (PatNum,LName,FName) SELECT patient.PatNum,LName,FName "
						+"FROM patient "
						+"INNER JOIN procedurelog "
						+"ON Patient.PatNum=procedurelog.PatNum "
						+"AND procedurelog.ProcStatus=2 "//complete
						+"AND procedurelog.ProvNum="+POut.Long(provNum)+" "
						+"AND procedurelog.ProcDate >= "+POut.Date(dateStart)+" "
						+"AND procedurelog.ProcDate <= "+POut.Date(dateEnd)+" "
						+"WHERE Birthdate <= "+POut.Date(dateStart.AddYears(-17))+" "
						+"AND Birthdate >= "+POut.Date(dateStart.AddYears(-74))+" "//17-74 before dateStart
						+"GROUP BY patient.PatNum";
					Db.NonQ(command);
					//Medication
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET HasMedication = 1 "
						+"WHERE (SELECT COUNT(*) "
						+"FROM medicationpat "
						+"WHERE medicationpat.PatNum=tempehrquality"+rndStr+@".PatNum "
						+"AND medicationpat.RxCui IN(199149, 199150, 200132, 205329, 205330, 205331, 401938,"//alph-glucosidas
						+"200256, 200257, 200258, 311919, 314142, 389139, 861035, 861039, 861042, 861044, 861787, 861790,"//amylin analogs
						+"744863 , 847910 , 847915,"//antidiabetic
						+"602544, 602549, 602550, 647237, 647239, 706895, 706896, 861731, 861736, 861740, 861743, 861748, 861753, 861760, 861763, 861769, 861783, 861787, 861790, 861795, 861806, 861816, 861819, 861822,"//antidiabetic combos
						+"665033, 665038, 665042, 860975, 860978, 860981, 860984, 860996, 860999, 861004, 861007, 861010, 861021, 861025, 861731, 861736, 861740, 861743, 861748, 861753, 861760, 861763, 861769, 861783, 861787, 861790, 861795, 861806, 861816, 861819, 861822,"//Biguanides
						+"205314, 237527, 242120, 242916, 242917, 259111, 260265, 283394, 311040, 311041, 311053, 311054, 311055, 311056, 311057, 311058, 311059, 311060, 311061, 314038, 317800, 351297, 358349, 484322, 485210, 544614, 763002, 763007, 763013, 763014, 833159, 847191, 847207, 847211, 847230, 847239, 847252, 847259, 847263, 847416,"//insulin
						+"200256, 200257, 200258, 311919, 314142, 389139,"//meglitinides
						+"105374, 153842, 197306, 197307, 197495, 197496, 197737, 198291, 198292, 198293, 198294, 199245, 199246, 199247, 199825, 199984, 199985, 200065, 252960, 310488, 310489, 310490, 310534, 310536, 310537, 310539, 312440, 312441, 312859, 312860, 312861, 313418, 313419, 314000, 314006, 315107, 315239, 317573, 379804, 389137, 602544, 602549, 602550, 647237, 647239, 706895, 706896, 757710, 757712, 844809, 844824, 844827, 861731, 861736, 861740, 861743, 861748, 861753, 861760, 861763, 861783, 861795, 861806, 861816, 861822,"//Sulfonylureas
						+"312440, 312441, 312859, 312860, 312861, 317573) "//Thiazolidinediones
						+"AND (DateStop >= "+POut.Date(dateEnd.AddYears(-2))+" "//med active <= 2 years before or simultaneous to end date
						+"OR DateStop < '1880-01-01') "//or still active
						+") > 0";
					Db.NonQ(command);
					//HasDiagnosisDiabetes
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET HasDiagnosisDiabetes = 1 "
						+"WHERE (SELECT COUNT(*) "
						+"FROM disease,diseasedef "
						+"WHERE disease.DiseaseDefNum=diseasedef.DiseaseDefNum "
						+"AND disease.PatNum=tempehrquality"+rndStr+@".PatNum "
						+"AND (diseasedef.ICD9Code LIKE '250%' "
						+"OR diseasedef.ICD9Code LIKE '357.2' "
						+"OR diseasedef.ICD9Code LIKE '362.0%' "
						+"OR diseasedef.ICD9Code LIKE '366.41' "
						+"OR diseasedef.ICD9Code LIKE '648.0%') "
						+"AND (disease.DateStart <= "+POut.Date(dateEnd)+" "//if there is a start date, it can't be after the period end.
						+"OR disease.DateStart < '1880-01-01') "//no startdate
						+"AND (disease.DateStop >= "+POut.Date(dateEnd.AddYears(-2))+" "//if there's a datestop, it can't have stopped more than 2 years ago.
							//Specs say: diagnosis active <= 2 years before or simultaneous to end date
						+"OR disease.DateStop < '1880-01-01') "//or still active
						+") > 0";
					Db.NonQ(command);
					//DateBP
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET tempehrquality"+rndStr+@".DateBP=(SELECT MAX(vitalsign.DateTaken) "
						+"FROM vitalsign "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND vitalsign.BpSystolic != 0 "
						+"AND vitalsign.BpDiastolic != 0 "
						+"GROUP BY vitalsign.PatNum)";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@" SET DateBP='0001-01-01' WHERE DateBP='0000-00-00'";
					Db.NonQ(command);
					//Systolic and diastolic
					command="UPDATE tempehrquality"+rndStr+@",vitalsign "
						+"SET Systolic=BpSystolic, "
						+"Diastolic=BpDiastolic "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND tempehrquality"+rndStr+@".DateBP=vitalsign.DateTaken";
					Db.NonQ(command);
					//HasDiagnosisPolycystic
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET HasDiagnosisPolycystic = 1 "
						+"WHERE (SELECT COUNT(*) "
						+"FROM disease,diseasedef "
						+"WHERE disease.DiseaseDefNum=diseasedef.DiseaseDefNum "
						+"AND disease.PatNum=tempehrquality"+rndStr+@".PatNum "
						+"AND diseasedef.ICD9Code = '256.4' "
						+"AND (disease.DateStart <= "+POut.Date(dateEnd)+" "//if there's a datestart, it can't be after period end
						+"OR disease.DateStart < '1880-01-01') "
						//no restrictions on datestop.  It could still be active or could have stopped before or after the period end.
						+") > 0";
					Db.NonQ(command);
					//HasDiagnosisAcuteDiabetes
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET HasDiagnosisAcuteDiabetes = 1 "
						+"WHERE (SELECT COUNT(*) "
						+"FROM disease,diseasedef "
						+"WHERE disease.DiseaseDefNum=diseasedef.DiseaseDefNum "
						+"AND disease.PatNum=tempehrquality"+rndStr+@".PatNum "
						+"AND (diseasedef.ICD9Code LIKE '249%' OR diseasedef.ICD9Code='251.8' OR diseasedef.ICD9Code='962.0' "//steroid induced
						+"OR diseasedef.ICD9Code LIKE '648.8%') "//gestational
						+"AND (disease.DateStart <= "+POut.Date(dateEnd)+" "//if there's a datestart, it can't be after period end
						+"OR disease.DateStart < '1880-01-01') "
						//no restrictions on datestop.  It could still be active or could have stopped before or after the period end.
						+") > 0";
					Db.NonQ(command);
					command="SELECT * FROM tempehrquality"+rndStr+@"";
					tableRaw=Db.GetTable(command);
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					break;
				#endregion
				#region BloodPressureManage
				case QualityType.BloodPressureManage:
					//DiabetesBloodPressure-------------------------------------------------------------------------------------------------------
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					command=@"CREATE TABLE tempehrquality"+rndStr+@" (
						PatNum bigint NOT NULL PRIMARY KEY,
						LName varchar(255) NOT NULL,
						FName varchar(255) NOT NULL,
						HasDiagnosisHypertension tinyint NOT NULL,
						HasProcedureESRD tinyint NOT NULL,
						HasDiagnosisPregnancy tinyint NOT NULL,
						HasDiagnosisESRD tinyint NOT NULL,
						DateBP date NOT NULL,
						Systolic int NOT NULL,
						Diastolic int NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
					command="INSERT INTO tempehrquality"+rndStr+@" (PatNum,LName,FName) SELECT patient.PatNum,LName,FName "
						+"FROM patient "
						+"INNER JOIN procedurelog "
						+"ON Patient.PatNum=procedurelog.PatNum "
						+"AND procedurelog.ProcStatus=2 "//complete
						+"AND procedurelog.ProvNum="+POut.Long(provNum)+" "
						+"AND procedurelog.ProcDate >= "+POut.Date(dateStart)+" "
						+"AND procedurelog.ProcDate <= "+POut.Date(dateEnd)+" "
						+"WHERE Birthdate <= "+POut.Date(dateStart.AddYears(-17))+" "
						+"AND Birthdate >= "+POut.Date(dateStart.AddYears(-74))+" "//17-74 before dateStart
						+"GROUP BY patient.PatNum";
					Db.NonQ(command);
					//HasDiagnosisHypertension
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET HasDiagnosisHypertension = 1 "
						+"WHERE (SELECT COUNT(*) "
						+"FROM disease,diseasedef "
						+"WHERE disease.DiseaseDefNum=diseasedef.DiseaseDefNum "
						+"AND disease.PatNum=tempehrquality"+rndStr+@".PatNum "
						+"AND diseasedef.ICD9Code LIKE '401%' "
						+"AND (disease.DateStart <= "+POut.Date(dateStart.AddMonths(6))+" "//if there is a start date, it can't be after this point
						+"OR disease.DateStart < '1880-01-01') "//no startdate
						//no restrictions on datestop.  It could still be active or could have stopped before or after the period end.
						+") > 0";
					Db.NonQ(command);
					command="DELETE FROM tempehrquality"+rndStr+@" WHERE HasDiagnosisHypertension=0";//for speed
					Db.NonQ(command);
					//HasProcedureESRD
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET HasProcedureESRD = 1 "
						+"WHERE (SELECT COUNT(*) "
						+"FROM procedurelog,procedurecode "
						+"WHERE procedurelog.CodeNum=procedurecode.CodeNum "
						+"AND procedurelog.PatNum=tempehrquality"+rndStr+@".PatNum "
						+"AND procedurecode.ProcCode IN ('36145','36147','36148','36800', '36810','36815','36818','36819','36820', '36821','36831', '36832', '36833', '50300', '50320','50340','50360','50365','50370', '50380','90920','90921','90924','90925', '90935','90937', '90940','90945', '90947', '90957', '90958','90959','90960','90961','90962','90965','90966','90969','90970','90989','90993','90997','90999','99512') "//ESRD
						+"AND procedurelog.ProcDate >= "+POut.Date(dateStart)+" "
						+"AND procedurelog.ProcDate <= "+POut.Date(dateEnd)+" "
						+") > 0";
					Db.NonQ(command);
					//HasDiagnosisPregnancy
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET HasDiagnosisPregnancy = 1 "
						+"WHERE (SELECT COUNT(*) "
						+"FROM disease,diseasedef "
						+"WHERE disease.DiseaseDefNum=diseasedef.DiseaseDefNum "
						+"AND disease.PatNum=tempehrquality"+rndStr+@".PatNum "
						+"AND (diseasedef.ICD9Code LIKE '63%' "
						+"OR diseasedef.ICD9Code LIKE '64%' "
						+"OR diseasedef.ICD9Code LIKE '65%' "
						+"OR diseasedef.ICD9Code LIKE '66%' "
						+"OR diseasedef.ICD9Code LIKE '67%' "
						+"OR diseasedef.ICD9Code LIKE 'V22%' "
						+"OR diseasedef.ICD9Code LIKE 'V23%' "
						+"OR diseasedef.ICD9Code LIKE 'V28%') "
						//active during the period
						+"AND (disease.DateStart <= "+POut.Date(dateEnd)+" "//if there is a start date, it can't be after the period end.
						+"OR disease.DateStart < '1880-01-01') "//no startdate
						+"AND (disease.DateStop >= "+POut.Date(dateStart)+" "//if there's a datestop, it can't have stopped before the period.
						+"OR disease.DateStop < '1880-01-01') "//or still active
						+") > 0";
					Db.NonQ(command);
					//HasDiagnosisESRD
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET HasDiagnosisESRD = 1 "
						+"WHERE (SELECT COUNT(*) "
						+"FROM disease,diseasedef "
						+"WHERE disease.DiseaseDefNum=diseasedef.DiseaseDefNum "
						+"AND disease.PatNum=tempehrquality"+rndStr+@".PatNum "
						+"AND (diseasedef.ICD9Code LIKE '38.95' "
						+"OR diseasedef.ICD9Code LIKE '39%' "
						+"OR diseasedef.ICD9Code LIKE '54.98' "
						+"OR diseasedef.ICD9Code LIKE '55.6%' "
						+"OR diseasedef.ICD9Code LIKE '585%' "
						+"OR diseasedef.ICD9Code LIKE 'V42.0%' "
						+"OR diseasedef.ICD9Code LIKE 'V45.1%' "
						+"OR diseasedef.ICD9Code LIKE 'V56%') "
						//active during the period
						+"AND (disease.DateStart <= "+POut.Date(dateEnd)+" "
						+"OR disease.DateStart < '1880-01-01') "
						+"AND (disease.DateStop >= "+POut.Date(dateStart)+" "
						+"OR disease.DateStop < '1880-01-01') "
						+") > 0";
					Db.NonQ(command);
					//DateBP
					command="UPDATE tempehrquality"+rndStr+@" "
						+"SET tempehrquality"+rndStr+@".DateBP=(SELECT MAX(vitalsign.DateTaken) "
						+"FROM vitalsign "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND vitalsign.BpSystolic != 0 "
						+"AND vitalsign.BpDiastolic != 0 "
						+"GROUP BY vitalsign.PatNum)";
					Db.NonQ(command);
					command="UPDATE tempehrquality"+rndStr+@" SET DateBP='0001-01-01' WHERE DateBP='0000-00-00'";
					Db.NonQ(command);
					//Systolic and diastolic
					command="UPDATE tempehrquality"+rndStr+@",vitalsign "
						+"SET Systolic=BpSystolic, "
						+"Diastolic=BpDiastolic "
						+"WHERE tempehrquality"+rndStr+@".PatNum=vitalsign.PatNum "
						+"AND tempehrquality"+rndStr+@".DateBP=vitalsign.DateTaken";
					Db.NonQ(command);
					command="SELECT * FROM tempehrquality"+rndStr+@"";
					tableRaw=Db.GetTable(command);
					command="DROP TABLE IF EXISTS tempehrquality"+rndStr+@"";
					Db.NonQ(command);
					break;
				#endregion
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
			//PatNum, PatientName, Numerator(X), and Exclusion(X).
			DataTable table=new DataTable("audit");
			DataRow row;
			table.Columns.Add("PatNum");
			table.Columns.Add("patientName");
			table.Columns.Add("numerator");//X
			table.Columns.Add("exclusion");//X
			table.Columns.Add("explanation");
			List<DataRow> rows=new List<DataRow>();
			Patient pat;
			//string explanation;
			for(int i=0;i<tableRaw.Rows.Count;i++) {
				row=table.NewRow();
				row["PatNum"]=tableRaw.Rows[i]["PatNum"].ToString();
				pat=new Patient();
				pat.LName=tableRaw.Rows[i]["LName"].ToString();
				pat.FName=tableRaw.Rows[i]["FName"].ToString();
				pat.Preferred="";
				row["patientName"]=pat.GetNameLF();
				row["numerator"]="";
				row["exclusion"]="";
				row["explanation"]="";
				float weight=0;
				float height=0;
				float bmi=0;
				DateTime dateVisit;
				int visitCount;
				switch(qtype) {
					#region WeightOver65
					case QualityType.WeightOver65:
						//WeightOver65-----------------------------------------------------------------------------------------------------------------
						weight=PIn.Float(tableRaw.Rows[i]["Weight"].ToString());
						height=PIn.Float(tableRaw.Rows[i]["Height"].ToString());
						bmi=Vitalsigns.CalcBMI(weight,height);
						bool hasFollowupPlan=PIn.Bool(tableRaw.Rows[i]["HasFollowupPlan"].ToString());
						bool isIneligible=PIn.Bool(tableRaw.Rows[i]["IsIneligible"].ToString());
						string documentation=tableRaw.Rows[i]["Documentation"].ToString();
						if(bmi==0){
							row["explanation"]="No BMI";
						}
						else if(bmi < 22) {
							row["explanation"]="Underweight";
							if(hasFollowupPlan) {
								row["explanation"]+=", has followup plan: "+documentation;
								row["numerator"]="X";
							}
						}
						else if(bmi < 30) {
							row["numerator"]="X";
							row["explanation"]="Normal weight";
						}
						else {
							row["explanation"]="Overweight";
							if(hasFollowupPlan) {
								row["explanation"]+=", has followup plan: "+documentation;
								row["numerator"]="X";
							}
						}
						if(isIneligible) {
							row["exclusion"]="X";
							row["explanation"]+=", "+documentation;
						}
						break;
					#endregion
					#region WeightAdult
					case QualityType.WeightAdult:
						//WeightAdult-----------------------------------------------------------------------------------------------------------------
						weight=PIn.Float(tableRaw.Rows[i]["Weight"].ToString());
						height=PIn.Float(tableRaw.Rows[i]["Height"].ToString());
						bmi=Vitalsigns.CalcBMI(weight,height);
						hasFollowupPlan=PIn.Bool(tableRaw.Rows[i]["HasFollowupPlan"].ToString());
						isIneligible=PIn.Bool(tableRaw.Rows[i]["IsIneligible"].ToString());
						documentation=tableRaw.Rows[i]["Documentation"].ToString();
						if(bmi==0){
							row["explanation"]="No BMI";
						}
						else if(bmi < 18.5f) {
							row["explanation"]="Underweight";
							if(hasFollowupPlan) {
								row["explanation"]+=", has followup plan: "+documentation;
								row["numerator"]="X";
							}
						}
						else if(bmi < 25) {
							row["numerator"]="X";
							row["explanation"]="Normal weight";
						}
						else {
							row["explanation"]="Overweight";
							if(hasFollowupPlan) {
								row["explanation"]+=", has followup plan: "+documentation;
								row["numerator"]="X";
							}
						}
						if(isIneligible) {
							row["exclusion"]="X";
							row["explanation"]+=", "+documentation;
						}
						break;
					#endregion
					#region Hypertension
					case QualityType.Hypertension:
						//Hypertension---------------------------------------------------------------------------------------------------------------------
						dateVisit=PIn.Date(tableRaw.Rows[i]["DateVisit"].ToString());
						visitCount=PIn.Int(tableRaw.Rows[i]["VisitCount"].ToString());
						string icd9code=tableRaw.Rows[i]["Icd9Code"].ToString();
						DateTime datePbEntered=PIn.Date(tableRaw.Rows[i]["DateBpEntered"].ToString());
						if(dateVisit<dateStart || dateVisit>dateEnd) {//no visits in the measurement period
							continue;//don't add this row.  Not part of denominator.
						}
						if(visitCount<2) {
							continue;
						}
						if(icd9code=="") {
							continue;
						}
						if(datePbEntered.Year<1880) {//no bp entered
							row["explanation"]="No BP entered";
						}
						else {
							row["numerator"]="X";
							row["explanation"]="BP entered";
						}
						break;
					#endregion
					#region TobaccoUse
					case QualityType.TobaccoUse:
						//TobaccoUse---------------------------------------------------------------------------------------------------------------------
						dateVisit=PIn.Date(tableRaw.Rows[i]["DateVisit"].ToString());
						//visitCount=PIn.Int(tableRaw.Rows[i]["VisitCount"].ToString());
						DateTime dateAssessment=PIn.Date(tableRaw.Rows[i]["DateAssessment"].ToString());
						if(dateVisit<dateStart || dateVisit>dateEnd) {//no visits in the measurement period
							continue;//don't add this row.  Not part of denominator.
						}
						//if(visitCount<2) {//no, as explained in comments in GetDenominatorExplain().
						//	continue;
						//}
						if(dateAssessment.Year<1880) {
							row["explanation"]="No tobacco use entered.";
						}
						else if(dateAssessment < dateVisit.AddYears(-2)) {
							row["explanation"]="No tobacco use entered within timeframe.";
						}
						else{
							row["numerator"]="X";
							row["explanation"]="Tobacco use entered.";
						}
						break;
					#endregion
					#region TobaccoCessation
					case QualityType.TobaccoCessation:
						//TobaccoCessation----------------------------------------------------------------------------------------------------------------
						dateVisit=PIn.Date(tableRaw.Rows[i]["DateVisit"].ToString());
						dateAssessment=PIn.Date(tableRaw.Rows[i]["DateAssessment"].ToString());
						DateTime DateCessation=PIn.Date(tableRaw.Rows[i]["DateCessation"].ToString());
						documentation=tableRaw.Rows[i]["Documentation"].ToString();
						if(dateVisit<dateStart || dateVisit>dateEnd) {//no visits in the measurement period
							continue;//don't add this row.  Not part of denominator.
						}
						else if(dateAssessment < dateVisit.AddYears(-2)) {
							continue;//no assessment within 24 months, so not part of denominator.
						}
						else if(DateCessation.Year<1880) {
							row["explanation"]="No tobacco cessation entered.";
						}
						else if(DateCessation < dateVisit.AddYears(-2)) {
							row["explanation"]="No tobacco cessation entered within timeframe.";
						}
						else {
							row["numerator"]="X";
							row["explanation"]="Tobacco cessation: "+documentation;
						}
						break;
					#endregion
					#region InfluenzaAdult
					case QualityType.InfluenzaAdult:
						//InfluenzaAdult----------------------------------------------------------------------------------------------------------------
						DateTime DateVaccine=PIn.Date(tableRaw.Rows[i]["DateVaccine"].ToString());
						bool notGiven=PIn.Bool(tableRaw.Rows[i]["NotGiven"].ToString());
						documentation=tableRaw.Rows[i]["Documentation"].ToString();
						if(DateVaccine.Year<1880) {
							row["explanation"]="No influenza vaccine given";
						}
						else if(notGiven) {
							row["exclusion"]="X";
							row["explanation"]+="No influenza vaccine given, "+documentation;
						}
						else {
							row["numerator"]="X";
							row["explanation"]="Influenza vaccine given";
						}
						break;
					#endregion
					#region WeightChild_1_1
					case QualityType.WeightChild_1_1:
						//WeightChild_1_1----------------------------------------------------------------------------------------------------------------
						bool isPregnant=PIn.Bool(tableRaw.Rows[i]["IsPregnant"].ToString());
						bool hasBMI=PIn.Bool(tableRaw.Rows[i]["HasBMI"].ToString());
						if(isPregnant) {
							continue;
						}
						if(hasBMI) {
							row["numerator"]="X";
							row["explanation"]="BMI entered";
						}
						else {
							row["explanation"]="No BMI entered";
						}
						break;
					#endregion
					#region WeightChild_1_2
					case QualityType.WeightChild_1_2:
						//WeightChild_1_2----------------------------------------------------------------------------------------------------------------
						isPregnant=PIn.Bool(tableRaw.Rows[i]["IsPregnant"].ToString());
						bool ChildGotNutrition=PIn.Bool(tableRaw.Rows[i]["ChildGotNutrition"].ToString());
						if(isPregnant) {
							continue;
						}
						if(ChildGotNutrition) {
							row["numerator"]="X";
							row["explanation"]="Counseled for nutrition";
						}
						else {
							row["explanation"]="Not counseled for nutrition";
						}
						break;
					#endregion
					#region WeightChild_1_3
					case QualityType.WeightChild_1_3:
						//WeightChild_1_3----------------------------------------------------------------------------------------------------------------
						isPregnant=PIn.Bool(tableRaw.Rows[i]["IsPregnant"].ToString());
						bool ChildGotPhysCouns=PIn.Bool(tableRaw.Rows[i]["ChildGotPhysCouns"].ToString());
						if(isPregnant) {
							continue;
						}
						if(ChildGotPhysCouns) {
							row["numerator"]="X";
							row["explanation"]="Counseled for physical activity";
						}
						else {
							row["explanation"]="Not counseled for physical activity";
						}
						break;
					#endregion
					#region WeightChild_2_1
					case QualityType.WeightChild_2_1:
						//WeightChild_2_1----------------------------------------------------------------------------------------------------------------
						isPregnant=PIn.Bool(tableRaw.Rows[i]["IsPregnant"].ToString());
						hasBMI=PIn.Bool(tableRaw.Rows[i]["HasBMI"].ToString());
						if(isPregnant) {
							continue;
						}
						if(hasBMI) {
							row["numerator"]="X";
							row["explanation"]="BMI entered";
						}
						else {
							row["explanation"]="No BMI entered";
						}
						break;
					#endregion
					#region WeightChild_2_2
					case QualityType.WeightChild_2_2:
						//WeightChild_2_2----------------------------------------------------------------------------------------------------------------
						isPregnant=PIn.Bool(tableRaw.Rows[i]["IsPregnant"].ToString());
						ChildGotNutrition=PIn.Bool(tableRaw.Rows[i]["ChildGotNutrition"].ToString());
						if(isPregnant) {
							continue;
						}
						if(ChildGotNutrition) {
							row["numerator"]="X";
							row["explanation"]="Counseled for nutrition";
						}
						else {
							row["explanation"]="Not counseled for nutrition";
						}
						break;
					#endregion
					#region WeightChild_2_3
					case QualityType.WeightChild_2_3:
						//WeightChild_2_3----------------------------------------------------------------------------------------------------------------
						isPregnant=PIn.Bool(tableRaw.Rows[i]["IsPregnant"].ToString());
						ChildGotPhysCouns=PIn.Bool(tableRaw.Rows[i]["ChildGotPhysCouns"].ToString());
						if(isPregnant) {
							continue;
						}
						if(ChildGotPhysCouns) {
							row["numerator"]="X";
							row["explanation"]="Counseled for physical activity";
						}
						else {
							row["explanation"]="Not counseled for physical activity";
						}
						break;
					#endregion
					#region WeightChild_3_1
					case QualityType.WeightChild_3_1:
						//WeightChild_3_1----------------------------------------------------------------------------------------------------------------
						isPregnant=PIn.Bool(tableRaw.Rows[i]["IsPregnant"].ToString());
						hasBMI=PIn.Bool(tableRaw.Rows[i]["HasBMI"].ToString());
						if(isPregnant) {
							continue;
						}
						if(hasBMI) {
							row["numerator"]="X";
							row["explanation"]="BMI entered";
						}
						else {
							row["explanation"]="No BMI entered";
						}
						break;
					#endregion
					#region WeightChild_3_2
					case QualityType.WeightChild_3_2:
						//WeightChild_3_2----------------------------------------------------------------------------------------------------------------
						isPregnant=PIn.Bool(tableRaw.Rows[i]["IsPregnant"].ToString());
						ChildGotNutrition=PIn.Bool(tableRaw.Rows[i]["ChildGotNutrition"].ToString());
						if(isPregnant) {
							continue;
						}
						if(ChildGotNutrition) {
							row["numerator"]="X";
							row["explanation"]="Counseled for nutrition";
						}
						else {
							row["explanation"]="Not counseled for nutrition";
						}
						break;
					#endregion
					#region WeightChild_3_3
					case QualityType.WeightChild_3_3:
						//WeightChild_3_3----------------------------------------------------------------------------------------------------------------
						isPregnant=PIn.Bool(tableRaw.Rows[i]["IsPregnant"].ToString());
						ChildGotPhysCouns=PIn.Bool(tableRaw.Rows[i]["ChildGotPhysCouns"].ToString());
						if(isPregnant) {
							continue;
						}
						if(ChildGotPhysCouns) {
							row["numerator"]="X";
							row["explanation"]="Counseled for physical activity";
						}
						else {
							row["explanation"]="Not counseled for physical activity";
						}
						break;
					#endregion
					#region ImmunizeChild_1
					case QualityType.ImmunizeChild_1:
						//ImmunizeChild_1--------------------------------------------------------------------------------------------------------------
						int count=PIn.Int(tableRaw.Rows[i]["Count1"].ToString());
						notGiven=PIn.Bool(tableRaw.Rows[i]["NotGiven1"].ToString());
						documentation=PIn.String(tableRaw.Rows[i]["Documentation1"].ToString());
						if(notGiven) {
							row["exclusion"]="X";
							row["explanation"]+="No DTaP vaccine given, "+documentation;
						}
						else if(count>=4) {
							row["numerator"]="X";
							row["explanation"]="DTaP vaccinations: "+count.ToString();
						}
						else {
							row["explanation"]="DTaP vaccinations: "+count.ToString();
						}
						break;
					#endregion
					#region ImmunizeChild_2
					case QualityType.ImmunizeChild_2:
						//ImmunizeChild_2--------------------------------------------------------------------------------------------------------------
						count=PIn.Int(tableRaw.Rows[i]["Count2"].ToString());
						notGiven=PIn.Bool(tableRaw.Rows[i]["NotGiven2"].ToString());
						documentation=PIn.String(tableRaw.Rows[i]["Documentation2"].ToString());
						if(notGiven) {
							row["exclusion"]="X";
							row["explanation"]+="No IPV vaccine given, "+documentation;
						}
						else if(count>=3) {
							row["numerator"]="X";
							row["explanation"]="IPV vaccinations: "+count.ToString();
						}
						else {
							row["explanation"]="IPV vaccinations: "+count.ToString();
						}
						break;
					#endregion
					#region ImmunizeChild_3
					case QualityType.ImmunizeChild_3:
						//ImmunizeChild_3--------------------------------------------------------------------------------------------------------------
						count=PIn.Int(tableRaw.Rows[i]["Count3"].ToString());
						notGiven=PIn.Bool(tableRaw.Rows[i]["NotGiven3"].ToString());
						documentation=PIn.String(tableRaw.Rows[i]["Documentation3"].ToString());
						int count3a=PIn.Int(tableRaw.Rows[i]["Count3a"].ToString());
						bool notGiven3a=PIn.Bool(tableRaw.Rows[i]["NotGiven3a"].ToString());
						string documentation3a=PIn.String(tableRaw.Rows[i]["Documentation3a"].ToString());
						int count3b=PIn.Int(tableRaw.Rows[i]["Count3b"].ToString());
						bool notGiven3b=PIn.Bool(tableRaw.Rows[i]["NotGiven3b"].ToString());
						string documentation3b=PIn.String(tableRaw.Rows[i]["Documentation3b"].ToString());
						int count3c=PIn.Int(tableRaw.Rows[i]["Count3c"].ToString());
						bool notGiven3c=PIn.Bool(tableRaw.Rows[i]["NotGiven3c"].ToString());
						string documentation3c=PIn.String(tableRaw.Rows[i]["Documentation3c"].ToString());
						if(notGiven) {
							row["exclusion"]="X";
							row["explanation"]+="No MMR vaccine given, "+documentation;
						}
						else if(notGiven3a) {
							row["exclusion"]="X";
							row["explanation"]+="No measles vaccine given, "+documentation3a;
						}
						else if(notGiven3b) {
							row["exclusion"]="X";
							row["explanation"]+="No mumps vaccine given, "+documentation3b;
						}
						else if(notGiven3c) {
							row["exclusion"]="X";
							row["explanation"]+="No rubella vaccine given, "+documentation3c;
						}
						else if(count>=1) {
							row["numerator"]="X";
							row["explanation"]="MMR vaccinations: "+count.ToString();
						}
						else if(count3a>=1 && count3b>=1 && count3c>=1) {
							row["numerator"]="X";
							row["explanation"]="MMR individual vaccinations given.";
						}
						else {
							row["explanation"]="MMR vaccination not given";
						}
						break;
					#endregion
					#region ImmunizeChild_4
					case QualityType.ImmunizeChild_4:
						//ImmunizeChild_4--------------------------------------------------------------------------------------------------------------
						count=PIn.Int(tableRaw.Rows[i]["Count4"].ToString());
						notGiven=PIn.Bool(tableRaw.Rows[i]["NotGiven4"].ToString());
						documentation=PIn.String(tableRaw.Rows[i]["Documentation4"].ToString());
						if(notGiven) {
							row["exclusion"]="X";
							row["explanation"]+="No HiB vaccine given, "+documentation;
						}
						else if(count>=2) {
							row["numerator"]="X";
							row["explanation"]="HiB vaccinations: "+count.ToString();
						}
						else {
							row["explanation"]="HiB vaccinations: "+count.ToString();
						}
						break;
					#endregion
					#region ImmunizeChild_5
					case QualityType.ImmunizeChild_5:
						//ImmunizeChild_5--------------------------------------------------------------------------------------------------------------
						count=PIn.Int(tableRaw.Rows[i]["Count5"].ToString());
						notGiven=PIn.Bool(tableRaw.Rows[i]["NotGiven5"].ToString());
						documentation=PIn.String(tableRaw.Rows[i]["Documentation5"].ToString());
						if(notGiven) {
							row["exclusion"]="X";
							row["explanation"]+="No hepatitis B vaccine given, "+documentation;
						}
						else if(count>=3) {
							row["numerator"]="X";
							row["explanation"]="hepatitis B vaccinations: "+count.ToString();
						}
						else {
							row["explanation"]="hepatitis B vaccinations: "+count.ToString();
						}
						break;
					#endregion
					#region ImmunizeChild_6
					case QualityType.ImmunizeChild_6:
						//ImmunizeChild_6--------------------------------------------------------------------------------------------------------------
						count=PIn.Int(tableRaw.Rows[i]["Count6"].ToString());
						notGiven=PIn.Bool(tableRaw.Rows[i]["NotGiven6"].ToString());
						documentation=PIn.String(tableRaw.Rows[i]["Documentation6"].ToString());
						if(notGiven) {
							row["exclusion"]="X";
							row["explanation"]+="No VZV vaccine given, "+documentation;
						}
						else if(count>=1) {
							row["numerator"]="X";
							row["explanation"]="VZV vaccinations: "+count.ToString();
						}
						else {
							row["explanation"]="VZV vaccinations: "+count.ToString();
						}
						break;
					#endregion
					#region ImmunizeChild_7
					case QualityType.ImmunizeChild_7:
						//ImmunizeChild_7--------------------------------------------------------------------------------------------------------------
						count=PIn.Int(tableRaw.Rows[i]["Count7"].ToString());
						notGiven=PIn.Bool(tableRaw.Rows[i]["NotGiven7"].ToString());
						documentation=PIn.String(tableRaw.Rows[i]["Documentation7"].ToString());
						if(notGiven) {
							row["exclusion"]="X";
							row["explanation"]+="No pneumococcal vaccine given, "+documentation;
						}
						else if(count>=4) {
							row["numerator"]="X";
							row["explanation"]="pneumococcal vaccinations: "+count.ToString();
						}
						else {
							row["explanation"]="pneumococcal vaccinations: "+count.ToString();
						}
						break;
					#endregion
					#region ImmunizeChild_8
					case QualityType.ImmunizeChild_8:
						//ImmunizeChild_8--------------------------------------------------------------------------------------------------------------
						count=PIn.Int(tableRaw.Rows[i]["Count8"].ToString());
						notGiven=PIn.Bool(tableRaw.Rows[i]["NotGiven8"].ToString());
						documentation=PIn.String(tableRaw.Rows[i]["Documentation8"].ToString());
						if(notGiven) {
							row["exclusion"]="X";
							row["explanation"]+="No hepatitis A vaccine given, "+documentation;
						}
						else if(count>=2) {
							row["numerator"]="X";
							row["explanation"]="hepatitis A vaccinations: "+count.ToString();
						}
						else {
							row["explanation"]="hepatitis A vaccinations: "+count.ToString();
						}
						break;
					#endregion
					#region ImmunizeChild_9
					case QualityType.ImmunizeChild_9:
						//ImmunizeChild_9--------------------------------------------------------------------------------------------------------------
						count=PIn.Int(tableRaw.Rows[i]["Count9"].ToString());
						notGiven=PIn.Bool(tableRaw.Rows[i]["NotGiven9"].ToString());
						documentation=PIn.String(tableRaw.Rows[i]["Documentation9"].ToString());
						if(notGiven) {
							row["exclusion"]="X";
							row["explanation"]+="No rotavirus vaccine given, "+documentation;
						}
						else if(count>=2) {
							row["numerator"]="X";
							row["explanation"]="rotavirus vaccinations: "+count.ToString();
						}
						else {
							row["explanation"]="rotavirus vaccinations: "+count.ToString();
						}
						break;
					#endregion
					#region ImmunizeChild_10
					case QualityType.ImmunizeChild_10:
						//ImmunizeChild_10--------------------------------------------------------------------------------------------------------------
						count=PIn.Int(tableRaw.Rows[i]["Count10"].ToString());
						notGiven=PIn.Bool(tableRaw.Rows[i]["NotGiven10"].ToString());
						documentation=PIn.String(tableRaw.Rows[i]["Documentation10"].ToString());
						if(notGiven) {
							row["exclusion"]="X";
							row["explanation"]+="No influenza vaccine given, "+documentation;
						}
						else if(count>=2) {
							row["numerator"]="X";
							row["explanation"]="influenza vaccinations: "+count.ToString();
						}
						else {
							row["explanation"]="influenza vaccinations: "+count.ToString();
						}
						break;
					#endregion
					#region ImmunizeChild_11
					case QualityType.ImmunizeChild_11:
						int count1=PIn.Int(tableRaw.Rows[i]["Count1"].ToString());
						int count2=PIn.Int(tableRaw.Rows[i]["Count2"].ToString());
						int count3=PIn.Int(tableRaw.Rows[i]["Count3"].ToString());
						count3a=PIn.Int(tableRaw.Rows[i]["Count3a"].ToString());
						count3b=PIn.Int(tableRaw.Rows[i]["Count3b"].ToString());
						count3c=PIn.Int(tableRaw.Rows[i]["Count3c"].ToString());
						int count4=PIn.Int(tableRaw.Rows[i]["Count4"].ToString());
						int count5=PIn.Int(tableRaw.Rows[i]["Count5"].ToString());
						int count6=PIn.Int(tableRaw.Rows[i]["Count6"].ToString());
						bool notGiven1=PIn.Bool(tableRaw.Rows[i]["NotGiven1"].ToString());
						bool notGiven2=PIn.Bool(tableRaw.Rows[i]["NotGiven2"].ToString());
						bool notGiven3=PIn.Bool(tableRaw.Rows[i]["NotGiven3"].ToString());
						notGiven3a=PIn.Bool(tableRaw.Rows[i]["NotGiven3a"].ToString());
						notGiven3b=PIn.Bool(tableRaw.Rows[i]["NotGiven3b"].ToString());
						notGiven3c=PIn.Bool(tableRaw.Rows[i]["NotGiven3c"].ToString());
						bool notGiven4=PIn.Bool(tableRaw.Rows[i]["NotGiven4"].ToString());
						bool notGiven5=PIn.Bool(tableRaw.Rows[i]["NotGiven5"].ToString());
						bool notGiven6=PIn.Bool(tableRaw.Rows[i]["NotGiven6"].ToString());
						if(notGiven1 || notGiven2 || notGiven3 || notGiven3a || notGiven3b || notGiven3c || notGiven4 || notGiven5 || notGiven6) {
							row["exclusion"]="X";
							row["explanation"]+="Not given.";//too complicated to document.
						}
						else if(count1>=4 && count2>=3 
							&& (count3>=1 || (count3a>=1 && count3b>=1 && count3c>=1)) && count4>=2 && count5>=3 && count6>=1) {
							row["numerator"]="X";
							row["explanation"]="All vaccinations given.";
						}
						else {
							row["explanation"]="Missing vaccinations.";
						}
						break;
					#endregion
					#region ImmunizeChild_12
					case QualityType.ImmunizeChild_12:
						//ImmunizeChild_12--------------------------------------------------------------------------------------------------------------
						count1=PIn.Int(tableRaw.Rows[i]["Count1"].ToString());
						count2=PIn.Int(tableRaw.Rows[i]["Count2"].ToString());
						count3=PIn.Int(tableRaw.Rows[i]["Count3"].ToString());
						count3a=PIn.Int(tableRaw.Rows[i]["Count3a"].ToString());
						count3b=PIn.Int(tableRaw.Rows[i]["Count3b"].ToString());
						count3c=PIn.Int(tableRaw.Rows[i]["Count3c"].ToString());
						count4=PIn.Int(tableRaw.Rows[i]["Count4"].ToString());
						count5=PIn.Int(tableRaw.Rows[i]["Count5"].ToString());
						count6=PIn.Int(tableRaw.Rows[i]["Count6"].ToString());
						int count7=PIn.Int(tableRaw.Rows[i]["Count7"].ToString());
						notGiven1=PIn.Bool(tableRaw.Rows[i]["NotGiven1"].ToString());
						notGiven2=PIn.Bool(tableRaw.Rows[i]["NotGiven2"].ToString());
						notGiven3=PIn.Bool(tableRaw.Rows[i]["NotGiven3"].ToString());
						notGiven3a=PIn.Bool(tableRaw.Rows[i]["NotGiven3a"].ToString());
						notGiven3b=PIn.Bool(tableRaw.Rows[i]["NotGiven3b"].ToString());
						notGiven3c=PIn.Bool(tableRaw.Rows[i]["NotGiven3c"].ToString());
						notGiven4=PIn.Bool(tableRaw.Rows[i]["NotGiven4"].ToString());
						notGiven5=PIn.Bool(tableRaw.Rows[i]["NotGiven5"].ToString());
						notGiven6=PIn.Bool(tableRaw.Rows[i]["NotGiven6"].ToString());
						bool notGiven7=PIn.Bool(tableRaw.Rows[i]["NotGiven7"].ToString());
						if(notGiven1 || notGiven2 || notGiven3 || notGiven3a || notGiven3b || notGiven3c || notGiven4 || notGiven5 || notGiven6 || notGiven7) {
							row["exclusion"]="X";
							row["explanation"]+="Not given.";//too complicated to document.
						}
						else if(count1>=4 && count2>=3 
							&& (count3>=1 || (count3a>=1 && count3b>=1 && count3c>=1)) && count4>=2 && count5>=3 && count6>=1 && count7>=4) {
							row["numerator"]="X";
							row["explanation"]="All vaccinations given.";
						}
						else {
							row["explanation"]="Missing vaccinations.";
						}
						break;
					#endregion
					#region Pneumonia
					case QualityType.Pneumonia:
						//Pneumonia----------------------------------------------------------------------------------------------------------------
						DateVaccine=PIn.Date(tableRaw.Rows[i]["DateVaccine"].ToString());
						if(DateVaccine.Year<1880) {
							row["explanation"]="No pneumococcal vaccine given";
						}
						else {
							row["numerator"]="X";
							row["explanation"]="Pneumococcal vaccine given";
						}
						break;
					#endregion
					#region DiabetesBloodPressure
					case QualityType.DiabetesBloodPressure:
						//DiabetesBloodPressure---------------------------------------------------------------------------------------------------
						bool hasMedication=PIn.Bool(tableRaw.Rows[i]["HasMedication"].ToString());
						bool HasDiagnosisDiabetes=PIn.Bool(tableRaw.Rows[i]["HasDiagnosisDiabetes"].ToString());
						DateTime DateBP=PIn.Date(tableRaw.Rows[i]["DateBP"].ToString());
						int systolic=PIn.Int(tableRaw.Rows[i]["Systolic"].ToString());
						int diastolic=PIn.Int(tableRaw.Rows[i]["Diastolic"].ToString());
						bool HasDiagnosisPolycystic=PIn.Bool(tableRaw.Rows[i]["HasDiagnosisPolycystic"].ToString());
						bool HasDiagnosisAcuteDiabetes=PIn.Bool(tableRaw.Rows[i]["HasDiagnosisAcuteDiabetes"].ToString());
						if(!hasMedication && !HasDiagnosisDiabetes) {
							continue;//not part of denominator
						}
						if(HasDiagnosisPolycystic && !HasDiagnosisDiabetes) {
							row["exclusion"]="X";
							row["explanation"]+="polycystic ovaries";
						}
						else if(HasDiagnosisAcuteDiabetes && hasMedication && !HasDiagnosisDiabetes) {
							row["exclusion"]="X";
							row["explanation"]+="gestational or steroid induced diabetes";
						}
						else if(DateBP.Year<1880) {
							row["explanation"]="No BP entered";
						}
						else if(systolic < 90 && diastolic < 140) {
							row["numerator"]="X";
							row["explanation"]="Controlled blood pressure: "+systolic.ToString()+"/"+diastolic.ToString();
						}
						else {
							row["explanation"]="High blood pressure: "+systolic.ToString()+"/"+diastolic.ToString();
						}
						break;
					#endregion
					#region BloodPressureManage
					case QualityType.BloodPressureManage:
						//BloodPressureManage-------------------------------------------------------------------------------------------------------
						bool HasDiagnosisHypertension=PIn.Bool(tableRaw.Rows[i]["HasDiagnosisHypertension"].ToString());
						bool HasProcedureESRD=PIn.Bool(tableRaw.Rows[i]["HasProcedureESRD"].ToString());
						bool HasDiagnosisPregnancy=PIn.Bool(tableRaw.Rows[i]["HasDiagnosisPregnancy"].ToString());
						bool HasDiagnosisESRD=PIn.Bool(tableRaw.Rows[i]["HasDiagnosisESRD"].ToString());
						DateBP=PIn.Date(tableRaw.Rows[i]["DateBP"].ToString());
						systolic=PIn.Int(tableRaw.Rows[i]["Systolic"].ToString());
						diastolic=PIn.Int(tableRaw.Rows[i]["Diastolic"].ToString());
						if(!HasDiagnosisHypertension) {
							continue;//not part of denominator
						}
						if(HasProcedureESRD || HasDiagnosisPregnancy || HasDiagnosisESRD) {
							continue;//not part of denominator
						}
						if(DateBP.Year<1880) {
							row["explanation"]="No BP entered";
						}
						else if(systolic < 90 && diastolic < 140) {
							row["numerator"]="X";
							row["explanation"]="Controlled blood pressure: "+systolic.ToString()+"/"+diastolic.ToString();
						}
						else {
							row["explanation"]="High blood pressure: "+systolic.ToString()+"/"+diastolic.ToString();
						}
						break;
					#endregion
					default:
						throw new ApplicationException("Type not found: "+qtype.ToString());
				}
				rows.Add(row);
			}
			for(int i=0;i<rows.Count;i++) {
				table.Rows.Add(rows[i]);
			}
			return table;
		}
		
		///<summary>Only called from GetAll2014.  Once the EhrCqmData object is created, all of the data relevant to the measure and required by the QRDA category 1 and category 3 reporting is part of the object.</summary>
		public static QualityMeasure GetEhrCqmData(QualityType2014 qtype,DateTime dateStart,DateTime dateEnd,long provNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<QualityMeasure>(MethodBase.GetCurrentMethod(),qtype,dateStart,dateEnd,provNum);
			}
			//these queries only work for mysql
			string command="SELECT GROUP_CONCAT(provider.ProvNum) FROM provider "
				+"WHERE provider.LName=(SELECT pv.LName FROM provider pv WHERE pv.ProvNum="+POut.Long(provNum)+")"
				+"AND provider.FName=(SELECT pv.FName FROM provider pv WHERE pv.ProvNum="+POut.Long(provNum)+")";
			string provs=Db.GetScalar(command);
			QualityMeasure measureCur=new QualityMeasure();
			List<string> listOneOfEncOIDs=new List<string>();
			List<string> listTwoOfEncOIDs=new List<string>();
			List<string> listValueSetOIDs;
			List<string> listReasonOIDs;
			List<long> listEhrPatNums;
			//This adultEncQuery is used by several CQMs
			//All encounters in the date range by the provider (based on ehrkey, so may be list of providers) for patients over 18 at the start of the measurement period
			string encounterSelectWhere="SELECT encounter.* FROM encounter "
				+"INNER JOIN patient ON patient.PatNum=encounter.PatNum "
				+"WHERE YEAR(patient.Birthdate)>1880 "//valid birthdate
				+"AND encounter.ProvNum IN("+POut.String(provs)+") "
				+"AND DATE(encounter.DateEncounter) BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" ";
			string encounterWhereAdults="AND patient.Birthdate<="+POut.Date(dateStart)+"-INTERVAL 18 YEAR ";//18 or over at start of measurement period
			string encounterOrder="ORDER BY encounter.PatNum,encounter.DateEncounter DESC";
			string adultEncCommand=encounterSelectWhere+encounterWhereAdults+encounterOrder;
			switch(qtype) {
				#region MedicationsEntered
				//this is our only measure that is not patient based, but episode of care based
				//denominator is all encounters that match the criteria in the date range, not all patients with those encounters
				case QualityType2014.MedicationsEntered:
					#region Get Initial Patient Population
					#region Get Valid Encounters
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.600.1.1834");//Medications Encounter Code Set Grouping Value Set
					//measureCur.ListEncounters will include all encounters that belong to the OneOf and TwoOf lists, so a patient will appear more than once
					//if they had more than one encounter from those sets in date range
					measureCur.DictPatNumListEncounters=GetEncountersWithOneOfAndTwoOfOIDs(adultEncCommand,listOneOfEncOIDs,listTwoOfEncOIDs);
					//if no patients had valid one of or two or more of the two of encounters, no need to continue calculating measure
					if(measureCur.DictPatNumListEncounters.Count==0) {
						break;
					}
					#endregion
					#region Get Patient Data
					//Denominator is equal to inital patient population for this measure, no exclusions
					measureCur.ListEhrPats=GetEhrPatsFromEncsOrProcs(measureCur.DictPatNumListEncounters);
					listEhrPatNums=GetListPatNums(measureCur.ListEhrPats);
					#endregion
					#endregion
					#region Get Current Medications Documented Procedures
					//Get procedures from the value set that occurred during measurement period
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.600.1.462" };//Current Medications Documented SNMD SNOMED-CT Value Set
					//Only one procedure code in the value set for this measure, SNOMEDCT - 428191000124101 - Documentation of current medications (procedure)
					measureCur.DictPatNumListMeasureEvents=GetMedDocumentedProcs(listEhrPatNums,listValueSetOIDs,dateStart,dateEnd);
					#endregion
					#region Get Medication Procs Not Performed
					//Get a list of all not performed items from the value set that occurred during the measurement period with valid readson
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.600.1.462" };//Current Medications Documented SNMD SNOMED-CT Value Set
					listReasonOIDs=new List<string>() { "2.16.840.1.113883.3.600.1.1502" };//Medical or Other reason not done SNOMED-CT Value Set
					measureCur.DictPatNumListNotPerfs=GetNotPerformeds(listEhrPatNums,listValueSetOIDs,listReasonOIDs,dateStart,dateEnd);
					#endregion
					break;
				#endregion
				#region WeightAdult And WeightOver65
				//The two populations are >= 18 and < 64 at the start of the measurement period and >= 65 at the start of the measurement period.
				//These two populations exclude patients who are 64 at the start of the measurement period apparently on purpose.
				case QualityType2014.WeightAdult:
				case QualityType2014.WeightOver65:
					//Strategy: Get all eligible encounters for patients 65 and over for Over65, >= 18 and < 64 for Adult, at the start of the measurement period
					//Get Not Performed items for BMI exams with valid reason ("Medical or Other" and "Patient" reasons)
					//Remove from the encounter list any encounters that have a Not Performed item for a BMI exam on the same day
					//Get from disease table all palliative care 'procedures' (not likely ever going to be any, but these 'procedure orders' will be stored in the disease table)
					//Remove from the encounter list any encounters that occurred for patients who have a palliative care order that starts before or during the encounter
					//Get patient data from the remaining encounters (for reporting), these patients are the initial patient population.
					//The problem list will contain pregnancies as well as palliative care problems
					//If the pregnancy starts before or during measurement period and does not end before start of measurement period, the patient is excluded
					//Numerator - MOST RECENT physical exam that is within 6 months of the specific encounter with:
						//1.) BMI >= 23 kg/m2 and < 30 kg/m2 for Over65, >= 18.5 and < 25 for Adult, OR
						//2.) BMI >= 30 kg/m2 for Over65, >= 25 for Adult, with an intervention or medication order for 'Overweight' within 6 months of the specific encounter
						//3.) BMI < 23 kg/m2 for Over65, < 18.5 for Adult, with an intervention or medication order for 'Underweight' within 6 months of the specific encounter
					#region Get Initial Patient Population
					#region Get Raw Encounters
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.600.1.1751");//BMI Encounter Code Set Grouping Value Set
					string encsWhere65="AND patient.Birthdate<"+POut.Date(dateStart)+"-INTERVAL 65 YEAR ";//65 or over at the start of the measurement period
					string encsWhereLessThan64="AND patient.Birthdate>"+POut.Date(dateStart)+"-INTERVAL 64 YEAR ";//< 64 years old at the start of the measurement period
					string encCommand="";
					if(qtype==QualityType2014.WeightOver65) {
						encCommand=encounterSelectWhere+encsWhere65+encounterOrder;
					}
					if(qtype==QualityType2014.WeightAdult) {
						encCommand=encounterSelectWhere+encounterWhereAdults+encsWhereLessThan64+encounterOrder;
					}
					measureCur.DictPatNumListEncounters=GetEncountersWithOneOfAndTwoOfOIDs(encCommand,listOneOfEncOIDs,listTwoOfEncOIDs);
					//if no patients had valid one of or two or more of the two of encounters, no need to continue calculating measure
					if(measureCur.DictPatNumListEncounters.Count==0) {
						break;
					}
					#endregion
					#region Get Pregnancy And Palliative Care Problems
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.600.1.1579" };//Palliative Care Grouping Value Set
					listValueSetOIDs.Add("2.16.840.1.113883.3.600.1.1623");//Pregnancy Dx Grouping Value Set
					List<long> listRawPatNums=new List<long>(measureCur.DictPatNumListEncounters.Keys);
					measureCur.DictPatNumListProblems=GetProblems(listRawPatNums,listValueSetOIDs,dateStart,dateEnd);
					#endregion
					#region Get Not Performed
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.600.1.681" };//BMI LOINC Value LOINC Value Set
					listReasonOIDs=new List<string>() { "2.16.840.1.113883.3.600.1.1502" };//Medical or Other reason not done SNOMED-CT Value Set
					listReasonOIDs.Add("2.16.840.1.113883.3.600.1.1503");//Patient Reason Refused SNOMED-CT Value Set
					measureCur.DictPatNumListNotPerfs=GetNotPerformeds(listRawPatNums,listValueSetOIDs,listReasonOIDs,dateStart,dateEnd);
					#endregion
					#region Remove If Palliative Care Order Exists Prior To Encounter Date
					//if the patient with eligible encounter list has a palliative care order that starts before or during the encounter, remove the encounter
					//if all encounters end up removed, remove the PatNum key from the dictionary
					foreach(KeyValuePair<long,List<EhrCqmEncounter>> patNumListEncs in measureCur.DictPatNumListEncounters) {//loop through every patient with an encounter list
						if(!measureCur.DictPatNumListProblems.ContainsKey(patNumListEncs.Key)) {//if no palliative care problem, move to next patient
							continue;
						}
						List<EhrCqmProblem> listProbsCur=measureCur.DictPatNumListProblems[patNumListEncs.Key];
						for(int i=patNumListEncs.Value.Count-1;i>-1;i--) {//loop through encounter list for this patient
							for(int j=0;j<listProbsCur.Count;j++) {//loop through palliative care problem list for this patient\
								if(listProbsCur[j].ValueSetOID!="2.16.840.1.113883.3.600.1.1579") {//if not Palliative Care, move to next problem (problem list for this measure can contain pregnancy dx or palliative care 'procedures')
									continue;
								}
								if(listProbsCur[j].DateStart.Date<=patNumListEncs.Value[i].DateEncounter.Date) {//if palliative care dx starts before or on encounter date
									patNumListEncs.Value.RemoveAt(i);//remove the encounter
									break;//break out of problem list loop, move to next encounter
								}
							}
						}
					}
					#endregion
					#region Remove If Not Performed Exists On Encounter Date
					//if the patient with eligible encounter list also has a not performed on the same day, remove the encounter
					//this patient will not be in the initial patient population for this encounter but may still be for a different encounter date
					foreach(KeyValuePair<long,List<EhrCqmEncounter>> patNumListEncs in measureCur.DictPatNumListEncounters) {//loop through every patient with an encounter list
						if(!measureCur.DictPatNumListNotPerfs.ContainsKey(patNumListEncs.Key)) {//if no not performed items, move to next patient
							continue;
						}
						List<EhrCqmNotPerf> listNotPerfsCur=measureCur.DictPatNumListNotPerfs[patNumListEncs.Key];//the not performed items are guaranteed to have valid reasons
						for(int i=patNumListEncs.Value.Count-1;i>-1;i--) {//loop through encounters for this patient
							for(int j=0;j<listNotPerfsCur.Count;j++) {//loop through not performed items for this patient
								if(listNotPerfsCur[j].DateEntry.Date==patNumListEncs.Value[i].DateEncounter.Date) {//compare encounter date to not perfomed date
									patNumListEncs.Value.RemoveAt(i);//remove encounter if not performed item on same date
									break;//break out of not performed loop to move to next encounter
								}
							}
						}
					}
					//if all encounters for this patient have been removed, remove the PatNum key so the patient is not in the denominator
					List<long> allKeys=new List<long>(measureCur.DictPatNumListEncounters.Keys);
					for(int i=0;i<allKeys.Count;i++) {
						if(measureCur.DictPatNumListEncounters[allKeys[i]].Count==0) {
							measureCur.DictPatNumListEncounters.Remove(allKeys[i]);
						}
					}
					//no patients with encounters left, no need to go on
					if(measureCur.DictPatNumListEncounters.Count==0) {
						break;
					}
					#endregion
					#region Get Patient Data
					//encounters are now eligible and only if no palliative care order before or on encounter date and no eligible not perforemed item on same date
					measureCur.ListEhrPats=GetEhrPatsFromEncsOrProcs(measureCur.DictPatNumListEncounters);
					listEhrPatNums=GetListPatNums(measureCur.ListEhrPats);
					#endregion
					#endregion
					#region Get Vital Sign Exams
					//get all vitalsign exams with valid height and weight in the date range, we have to subtract 6 months from dateStart, since encounter must be in measurement period, but exam can be before measurement period as long as it is within 6 months of the encounter
					//each exam will have a calculated BMI, which is (weight*703)/(height*height)
					measureCur.DictPatNumListVitalsigns=GetVitalsignsForBMI(listEhrPatNums,dateStart.AddMonths(-6),dateEnd);
					#endregion
					#region Get Interventions
					//Get all interventions for eligible value sets that occurred within 6 months of the start of the measurement period up to the end of the measurement period
					command="SELECT * FROM intervention "
						+"WHERE DATE(DateEntry) BETWEEN "+POut.Date(dateStart)+"-INTERVAL 6 MONTH AND "+POut.Date(dateEnd)+" ";
					if(listEhrPatNums!=null && listEhrPatNums.Count>0) {
						command+="AND intervention.PatNum IN("+string.Join(",",listEhrPatNums)+") ";
					}
					command+="ORDER BY PatNum,DateEntry DESC";
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.600.1.1525" };//Above Normal Follow-up Grouping Value Set
					listValueSetOIDs.Add("2.16.840.1.113883.3.600.1.1528");//Below Normal Follow up Grouping Value Set
					listValueSetOIDs.Add("2.16.840.1.113883.3.600.1.1527");//Referrals where weight assessment may occur Grouping Value Set
					measureCur.DictPatNumListInterventions=GetInterventions(command,listValueSetOIDs);
					#endregion
					#region Get MedicationPats
					//Get all medicationpats (check for start date and instructions when calculating to make sure they are 'Orders') that started within 6 months of start date
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.600.1.1498" };//Above Normal Medications RxNorm Value Set
					listValueSetOIDs.Add("2.16.840.1.113883.3.600.1.1499");//Below Normal Medications RxNorm Value Set
					measureCur.DictPatNumListMedPats=GetMedPats(listEhrPatNums,listValueSetOIDs,dateStart.AddMonths(-6),dateEnd);
					#endregion
					break;
				#endregion
				#region CariesPrevent age >= 0 and < 20
				//These four will be the same encounter/procedure codes, just 4 different age groups: 0-19, 0-5, 6-12, and 13-19
				case QualityType2014.CariesPrevent:
				case QualityType2014.CariesPrevent_1:
				case QualityType2014.CariesPrevent_2:
				case QualityType2014.CariesPrevent_3:
					//Strategy: Get all encounters in date range from eligible value sets for patients in age range
					//No exclusions, initial patient population is denominator
					//Get Flouride varnish application procedures that occurred during the measurement period
					//If encounter and procedure, 'Numerator'
					//No exceptions
					#region Get Initial Patient Population
					#region Get All Eligible Encounters
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1001");//Office Visit Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.125.12.1003");//Clinical Oral Evaluation Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1022");//Preventive Care- Initial Office Visit, 0 to 17 Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1023");//Preventive Care Services-Initial Office Visit, 18 and Up Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1024");//Preventive Care - Established Office Visit, 0 to 17 Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1025");//Preventive Care Services - Established Office Visit, 18 and Up Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1048");//Face-to-Face Interaction Grouping Value Set
					string child0To19Command=encounterSelectWhere
						+"AND patient.Birthdate<="+POut.Date(dateStart)+" "//age >= 0 at start of measurement period
						+"AND patient.Birthdate>"+POut.Date(dateStart)+"-INTERVAL 20 YEAR "//age < 20 at start of measurement period
						+encounterOrder;
					string child0To5Command=encounterSelectWhere
						+"AND patient.Birthdate<="+POut.Date(dateStart)+" "//age >= 0 at start of measurement period
						+"AND patient.Birthdate>"+POut.Date(dateStart)+"-INTERVAL 6 YEAR "//age <= 5 at start of measurement period
						+encounterOrder;
					string child6To12Command=encounterSelectWhere
						+"AND patient.Birthdate<="+POut.Date(dateStart)+"-INTERVAL 6 YEAR "//age >= 6 at start of measurement period
						+"AND patient.Birthdate>"+POut.Date(dateStart)+"-INTERVAL 13 YEAR "//age <= 12 at start of measurement period
						+encounterOrder;
					string child13To19Command=encounterSelectWhere
						+"AND patient.Birthdate<="+POut.Date(dateStart)+"-INTERVAL 13 YEAR "//age >= 13 at start of measurement period
						+"AND patient.Birthdate>"+POut.Date(dateStart)+"-INTERVAL 20 YEAR "//age < 20 at start of measurement period
						+encounterOrder;
					if(qtype==QualityType2014.CariesPrevent) {
						measureCur.DictPatNumListEncounters=GetEncountersWithOneOfAndTwoOfOIDs(child0To19Command,listOneOfEncOIDs,listTwoOfEncOIDs);
					}
					else if(qtype==QualityType2014.CariesPrevent_1) {
						measureCur.DictPatNumListEncounters=GetEncountersWithOneOfAndTwoOfOIDs(child0To5Command,listOneOfEncOIDs,listTwoOfEncOIDs);
					}
					else if(qtype==QualityType2014.CariesPrevent_2) {
						measureCur.DictPatNumListEncounters=GetEncountersWithOneOfAndTwoOfOIDs(child6To12Command,listOneOfEncOIDs,listTwoOfEncOIDs);
					}
					else if(qtype==QualityType2014.CariesPrevent_3) {
						measureCur.DictPatNumListEncounters=GetEncountersWithOneOfAndTwoOfOIDs(child13To19Command,listOneOfEncOIDs,listTwoOfEncOIDs);
					}
					//if no eligilble encounters, no need to go on
					if(measureCur.DictPatNumListEncounters.Count==0) {
						break;
					}
					#endregion
					#region Get Patient Data From Encounters
					measureCur.ListEhrPats=GetEhrPatsFromEncsOrProcs(measureCur.DictPatNumListEncounters);
					listEhrPatNums=GetListPatNums(measureCur.ListEhrPats);
					#endregion
					#endregion
					#region Get Flouride Varnish Application Procedures
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.464.1003.125.12.1002" };//Fluoride Varnish Application for Children Grouping Value Set
					measureCur.DictPatNumListProcs=GetProcs(listEhrPatNums,listValueSetOIDs,dateStart,dateEnd);
					#endregion
					break;
				#endregion
				#region ChildCaries
				case QualityType2014.ChildCaries:
					//Strategy: Get all encounters in date range from eligible value sets for patients in age range
					//No exclusions, initial patient population is denominator
					//Get Dental Caries diagnoses that were active during any of the measurement period (started before or during period and did NOT end before the start of period)
					//No exceptions
					#region Get Initial Patient Population
					#region Get All Eligible Encounters
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1001");//Office Visit Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.125.12.1003");//Clinical Oral Evaluation Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1022");//Preventive Care- Initial Office Visit, 0 to 17 Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1023");//Preventive Care Services-Initial Office Visit, 18 and Up Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1024");//Preventive Care - Established Office Visit, 0 to 17 Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1025");//Preventive Care Services - Established Office Visit, 18 and Up Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1048");//Face-to-Face Interaction Grouping Value Set
					child0To19Command=encounterSelectWhere
						+"AND patient.Birthdate<="+POut.Date(dateStart)+" "//age >= 0 at start of measurement period (born before or on start date)
						+"AND patient.Birthdate>"+POut.Date(dateStart)+"-INTERVAL 20 YEAR "//age < 20 at start of measurement period
						+encounterOrder;
					measureCur.DictPatNumListEncounters=GetEncountersWithOneOfAndTwoOfOIDs(child0To19Command,listOneOfEncOIDs,listTwoOfEncOIDs);
					//if no eligible encounters, no need to go on
					if(measureCur.DictPatNumListEncounters.Count==0) {
						break;
					}
					#endregion
					#region Get Patient Data From Encounters
					measureCur.ListEhrPats=GetEhrPatsFromEncsOrProcs(measureCur.DictPatNumListEncounters);
					listEhrPatNums=GetListPatNums(measureCur.ListEhrPats);
					#endregion
					#endregion
					#region Get Dental Caries Diagnoses
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.464.1003.125.12.1004" };//Dental Caries Grouping Value Set
					measureCur.DictPatNumListProblems=GetProblems(listEhrPatNums,listValueSetOIDs,dateStart,dateEnd);
					#endregion
					break;
				#endregion
				#region Pneumonia
				case QualityType2014.Pneumonia:
					//Strategy: Get encounters from eligible value sets for patients >= 65 years old before the start of the measurement period
					//Those patients will be initial patient population and denominator
					//No exclusions
					//Get vaccinepats with eligible code, only one code, CVX - 33 - pneumococcal polysaccharide vaccine, 23 valent
					//Get procs with eligible code, SNOMEDCT - 12866006 - Pneumococcal vaccination (procedure) and SNOMEDCT - 394678003 - Booster pneumococcal vaccination (procedure)
					//Get problems with eligible code, only one code, SNOMEDCT - 310578008 - Pneumococcal vaccination given (finding)
					#region Get Initial Patient Population
					#region Get Raw Encounters
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.526.3.1240");//Annual Wellness Visit Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1001");//Office Visit Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1016");//Home Healthcare Services Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1023");//Preventive Care Services-Initial Office Visit, 18 and Up Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1025");//Preventive Care Services - Established Office Visit, 18 and Up Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1048");//Face-to-Face Interaction Grouping Value Set
					encCommand=encounterSelectWhere
						+"AND patient.Birthdate<"+POut.Date(dateStart)+"-INTERVAL 65 YEAR "//65 or over at start of measurement period
						+encounterOrder;
					measureCur.DictPatNumListEncounters=GetEncountersWithOneOfAndTwoOfOIDs(encCommand,listOneOfEncOIDs,listTwoOfEncOIDs);
					//if no eligible encounters, no need to go on
					if(measureCur.DictPatNumListEncounters.Count==0) {
						break;
					}
					#endregion
					#region Get Patient Data
					measureCur.ListEhrPats=GetEhrPatsFromEncsOrProcs(measureCur.DictPatNumListEncounters);
					listEhrPatNums=GetListPatNums(measureCur.ListEhrPats);//for restricting the following list of procs, problems, and vaccinepats
					#endregion
					#endregion
					#region Get Vaccinepats
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.464.1003.110.12.1027" };//Pneumococcal Vaccine Grouping Value Set
					measureCur.DictPatNumListMedPats=GetVaccines(listEhrPatNums,listValueSetOIDs,DateTime.MinValue,dateEnd,true);
					#endregion
					#region Get Procs
					//Get procs that are Pneumococcal vaccine administered SNOMEDCT - 12866006
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.464.1003.110.12.1034" };//Pneumococcal Vaccine Administered Grouping Value Set
					measureCur.DictPatNumListProcs=GetProcs(listEhrPatNums,listValueSetOIDs,DateTime.MinValue,dateEnd);
					#endregion
					#region Get Problems
					//Get problems that are history of pheumococcal vaccine recoreded, one code allowed, SNOMEDCT - 310578008
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.464.1003.110.12.1028" };//History of Pneumococcal Vaccine Grouping Value Set
					measureCur.DictPatNumListProblems=GetProblems(listEhrPatNums,listValueSetOIDs,DateTime.MinValue,dateEnd);
					#endregion
					break;
				#endregion
				#region TobaccoCessation
				case QualityType2014.TobaccoCessation:
					#region Get Valid Encounters
					//add one of encounter OIDs to list
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.526.3.1240");//Annual Wellness Visit Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1023");//Preventive Care Services-Initial Office Visit, 18 and Up Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1025");//Preventive Care Services - Established Office Visit, 18 and Up Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1026");//Preventive Care Services-Individual Counseling Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1027");//Preventive Care Services - Group Counseling Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1030");//Preventive Care Services - Other Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1048");//Face-to-Face Interaction Grouping Value Set
					//add two of encounter OIDs to list
					listTwoOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1001");//Office Visit Grouping Value Set
					listTwoOfEncOIDs.Add("2.16.840.1.113883.3.526.3.1011");//Occupational Therapy Evaluation Grouping Value Set
					listTwoOfEncOIDs.Add("2.16.840.1.113883.3.526.3.1020");//Health & Behavioral Assessment - Individual Grouping Value Set
					listTwoOfEncOIDs.Add("2.16.840.1.113883.3.526.3.1141");//Psychoanalysis Grouping Value Set
					listTwoOfEncOIDs.Add("2.16.840.1.113883.3.526.3.1245");//Health and Behavioral Assessment - Initial Grouping Value Set
					listTwoOfEncOIDs.Add("2.16.840.1.113883.3.526.3.1285");//Ophthalmological Services Grouping Value Set
					listTwoOfEncOIDs.Add("2.16.840.1.113883.3.526.3.1492");//Psych Visit - Diagnostic Evaluation Grouping Value Set
					listTwoOfEncOIDs.Add("2.16.840.1.113883.3.526.3.1496");//Psych Visit - Psychotherapy Grouping Value Set
					//measureCur.ListEncounters will include all encounters that belong to the OneOf and TwoOf lists, so a patient will appear more than once
					//if they had more than one encounter from those sets in date range
					measureCur.DictPatNumListEncounters=GetEncountersWithOneOfAndTwoOfOIDs(adultEncCommand,listOneOfEncOIDs,listTwoOfEncOIDs);
					//if no eligible encounters, no need to go on
					if(measureCur.DictPatNumListEncounters.Count==0) {
						break;
					}
					#endregion
					#region Get Initial Patient Population
					//Denominator is equal to initial patient population for this measure
					//the Inital Patient Population will be unique patients in ListEncounters, loop through and count unique patients
					measureCur.ListEhrPats=GetEhrPatsFromEncsOrProcs(measureCur.DictPatNumListEncounters);
					listEhrPatNums=GetListPatNums(measureCur.ListEhrPats);
					#endregion
					#region Get Tobacco Assessments
					//Get a list of all tobacco assessment events that happened within 24 of end of measurement period
					measureCur.DictPatNumListMeasureEvents=GetTobaccoAssessmentEvents(listEhrPatNums,dateEnd);
					#endregion
					#region Get Tobacco Cessation Interventions
					//Get all interventions within 24 months of end of measurement period
					command="SELECT * FROM intervention "
						+"WHERE DATE(DateEntry) BETWEEN "+POut.Date(dateEnd)+"-INTERVAL 24 MONTH AND "+POut.Date(dateEnd)+" ";
					if(listEhrPatNums!=null && listEhrPatNums.Count>0) {
						command+="AND intervention.PatNum IN("+string.Join(",",listEhrPatNums)+") ";
					}
					command+="ORDER BY PatNum,DateEntry DESC";
					//Tobacco Use Cessation Counseling Grouping and Tobacco Use Cessation Pharmacotherapy Grouping Value Sets
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.526.3.509","2.16.840.1.113883.3.526.3.1190" };
					measureCur.DictPatNumListInterventions=GetInterventions(command,listValueSetOIDs);
					#endregion
					#region Get Tobacco Cessation Meds
					//Get a list of all tobacco cessation meds active/ordered within 24 months of end of measurement period
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.526.3.1190" };////Tobacco Use Cessation Pharmacotherapy Grouping Value Set
					measureCur.DictPatNumListMedPats=GetMedPats(listEhrPatNums,listValueSetOIDs,dateEnd.AddMonths(-24),dateEnd);
					#endregion
					#region Get Tobacco Screenings Not Performed
					//Get a list of all tobacco assessment not performed items that happened in the measurement period that belong to the value set
					//that also have a valid medical reason attached from the above value set
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.526.3.1278" };//Tobacco Use Screening Grouping Value Set
					listReasonOIDs=new List<string>() { "2.16.840.1.113883.3.526.3.1007" };//Medical Reason Grouping Value Set
					measureCur.DictPatNumListNotPerfs=GetNotPerformeds(listEhrPatNums,listValueSetOIDs,listReasonOIDs,dateStart,dateEnd);
					#endregion
					#region Get Limited Life Expectancy Probs
					listValueSetOIDs=new List<string>() {"2.16.840.1.113883.3.526.3.1259"};//Limited Life Expectancy Grouping Value Set
					//Get a list of all limited life expectancy diagnoses in the measurement period that belong to the above value set
					measureCur.DictPatNumListProblems=GetProblems(listEhrPatNums,listValueSetOIDs,dateStart,dateEnd);
					#endregion
					break;
				#endregion
				#region Influenza
				case QualityType2014.Influenza:
					//Strategy: Get encounters from one of and two of lists for patients >= 6 months before start of measurement period
					//Denominator: Those in the list of encounters who also had an encounter <= 92 days BEFORE the start of the measurement period from three code sets
					//OR an encounter from the same three code sets <= 91 days AFTER the start of the measurement period
					//No exclusions
					//Numerator: Get all flu vaccine procs and flu vaccine medications (vaccinepats, these will include the communication from patient to provider of previous receipt of vaccine)
					//that happened during the encounter <= 92 days before start of period or <= 91 days after start of period
					//Exceptions: 1. communication from patient to prov declining vaccine (vaccinepats with CompletionStatus=1 (refused))
					//2. procedure or medication NotPerformed for medical, patient, or system reason
					//Both 1 and 2 have to be during the encounter in the <= 92 before start or <= 91 days after start of period date range
					//OR active diagnosis of allergy to eggs, allergy to flu vaccine, or intolerance to flu vaccine
					//OR medication allergy or intolerance to flu vaccine
					//OR procedure intolerance to vaccine
					//Those have to start before or during the encounter in the date range.
					#region Get Initial Patient Population
					#region Get Raw Encounters
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.526.3.1240");//Annual Wellness Visit Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1012");//Nursing Facility Visit Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1013");//Discharge Services - Nursing Facility Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1022");//Preventive Care- Initial Office Visit, 0 to 17 Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1023");//Preventive Care Services-Initial Office Visit, 18 and Up Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1024");//Preventive Care - Established Office Visit, 0 to 17 Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1025");//Preventive Care Services - Established Office Visit, 18 and Up Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1026");//Preventive Care Services-Individual Counseling Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1027");//Preventive Care Services - Group Counseling Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1030");//Preventive Care Services - Other Grouping Value Set
					listTwoOfEncOIDs.Add("2.16.840.1.113883.3.526.3.1012");//Patient Provider Interaction Grouping Value Set
					listTwoOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1001");//Office Visit Grouping Value Set
					listTwoOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1008");//Outpatient Consultation Grouping Value Set
					listTwoOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1014");//Care Services in Long-Term Residential Facility Grouping Value Set
					listTwoOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1016");//Home Healthcare Services Grouping Value Set
					encCommand=encounterSelectWhere
						+"AND patient.Birthdate<"+POut.Date(dateStart)+"-INTERVAL 6 MONTH "//>= 6 months at start of measurement period
						+encounterOrder;
					measureCur.DictPatNumListEncounters=GetEncountersWithOneOfAndTwoOfOIDs(encCommand,listOneOfEncOIDs,listTwoOfEncOIDs);
					#endregion
					#region Get Patients Who Had Initial Population Proc
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.526.3.1083" };//Hemodialysis Grouping Value Set
					listValueSetOIDs.Add("2.16.840.1.113883.3.526.3.1084");//Peritoneal Dialysis Grouping Value Set
					measureCur.DictPatNumListProcs=GetProcs(null,listValueSetOIDs,dateStart,dateEnd);
					#endregion
					#region Get Patient Data
					measureCur.ListEhrPats=GetEhrPatsFromEncsOrProcs(measureCur.DictPatNumListEncounters,measureCur.DictPatNumListProcs);
					listEhrPatNums=GetListPatNums(measureCur.ListEhrPats);//for restricting the following list of procs, problems, and vaccinepats
					#endregion
					#endregion
					#region Apply Additional Denominator Requirements
					//These are additional requirements for the patient to be in the denominator.
					//The patient must have one of the eligible encounters/procedures <= 92 days before the start of the period or <= 91 days after the start of the period to be in denominator.
					Dictionary<long,List<EhrCqmEncounter>> dictPatNumListDenomEncs;
					if(listEhrPatNums!=null && listEhrPatNums.Count==0) {
						dictPatNumListDenomEncs=new Dictionary<long,List<EhrCqmEncounter>>();
					}
					else {
						listOneOfEncOIDs=new List<string>() { "2.16.840.1.113883.3.526.3.1252" };//Encounter-Influenza Grouping Value Set
						listTwoOfEncOIDs=new List<string>();
						encCommand="SELECT encounter.* FROM encounter "
						+"INNER JOIN patient ON patient.PatNum=encounter.PatNum "
						+"WHERE YEAR(patient.Birthdate)>1880 "//valid birthdate
						+"AND encounter.ProvNum IN("+POut.String(provs)+") "
						+"AND DATE(encounter.DateEncounter) BETWEEN "+POut.Date(dateStart.AddDays(-92))+" AND "+POut.Date(dateStart.AddDays(91))+" ";
						if(listEhrPatNums!=null && listEhrPatNums.Count>0) {
							command+="AND encounter.PatNum IN("+string.Join(",",listEhrPatNums)+") ";
						}
						command+="ORDER BY encounter.PatNum,encounter.DateEncounter DESC";
						dictPatNumListDenomEncs=GetEncountersWithOneOfAndTwoOfOIDs(encCommand,listOneOfEncOIDs,listTwoOfEncOIDs);
					}
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.526.3.1083" };//Hemodialysis Grouping Value Set
					listValueSetOIDs.Add("2.16.840.1.113883.3.526.3.1084");//Peritoneal Dialysis Grouping Value Set
					Dictionary<long,List<EhrCqmProc>> dictPatNumListDenomProcs=GetProcs(listEhrPatNums,listValueSetOIDs,dateStart.AddDays(-92),dateStart.AddDays(91));
					//for each patient in initial pat population ListEhrPats, make sure they have an encounter in dictPatNumListDenomEncs or a procedure in dictPatNumListDenomProcs
					//those lists will have the eligible encounters or procedures in the <= 92 days starts before start of period and <= 91 days starts after start of period
					//i.e. the encounter or procedure took place between October 1st of year prior to measurement period and March 31st of year of measurement period
					//this date range is their definition of the "influenza season"
					for(int i=0;i<measureCur.ListEhrPats.Count;i++) {
						long patNumCur=measureCur.ListEhrPats[i].EhrCqmPat.PatNum;
						bool isDenom=false;
						if(dictPatNumListDenomEncs.ContainsKey(patNumCur)) {
							isDenom=true;
							if(measureCur.DictPatNumListEncounters.ContainsKey(patNumCur)) {
								measureCur.DictPatNumListEncounters[patNumCur].AddRange(dictPatNumListDenomEncs[patNumCur]);
							}
							else {//no encounters from initial patient population, must be in ipp from procedure, add new list to include these encounters
								measureCur.DictPatNumListEncounters.Add(patNumCur,dictPatNumListDenomEncs[patNumCur]);
							}
						}
						if(dictPatNumListDenomProcs.ContainsKey(patNumCur)) {
							isDenom=true;
							if(measureCur.DictPatNumListProcs.ContainsKey(patNumCur)) {
								measureCur.DictPatNumListProcs[patNumCur].AddRange(dictPatNumListDenomProcs[patNumCur]);
							}
							else {
								measureCur.DictPatNumListProcs.Add(patNumCur,dictPatNumListDenomProcs[patNumCur]);
							}
						}
						if(!isDenom) {
							//this is the only place the bool IsDenominator is ever set to false
							measureCur.ListEhrPats[i].IsDenominator=false;
						}
					}
					#endregion
					#region Get Numerator Data
					#region Get Influenza Vaccination Procedures
					//These will actually be in the procedurelog table, the user will have to manually add the correct code to the proccode table and then chart the procedure
					//Codes are CPT or SNOMEDCT
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.526.3.402" };//Influenza Vaccination Grouping Value Set
					Dictionary<long,List<EhrCqmProc>> dictPatNumListNumeProcs=GetProcs(listEhrPatNums,listValueSetOIDs,dateStart.AddDays(-92),dateStart.AddDays(91));
					for(int i=0;i<measureCur.ListEhrPats.Count;i++) {
						long patNumCur=measureCur.ListEhrPats[i].EhrCqmPat.PatNum;
						if(dictPatNumListNumeProcs.ContainsKey(patNumCur)) {
							if(measureCur.DictPatNumListProcs.ContainsKey(patNumCur)) {
								measureCur.DictPatNumListProcs[patNumCur].AddRange(dictPatNumListNumeProcs[patNumCur]);
							}
							else {
								measureCur.DictPatNumListProcs.Add(patNumCur,dictPatNumListNumeProcs[patNumCur]);
							}
						}
					}
					#endregion
					#region Get Influenza Vaccination Medications
					//These will be the CVX codes that will be in the vaccinepat table
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.526.3.1254" };//Influenza Vaccine Grouping Value Set
					measureCur.DictPatNumListMedPats=GetVaccines(listEhrPatNums,listValueSetOIDs,DateTime.MinValue,dateStart.AddDays(91),false);//false means could be vaccines given or not given
					//Not given vaccines will be used in denominator exceptions and assumed medication intolerance or allergy.
					//Vaccines given will have to have occurred during "Occurrence A of..." but Not Given means intolerance or allergy and can be any time before or during "Occurrence A of..."
					#endregion
					#region Get Problems For Numerator And Exceptions
					//These will be in the patient's 'Problem' list, like the Pneumonia vaccine communication of previous receipt, 4 possible SNOMEDCT codes
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.526.3.1185" };//Previous Receipt of Influenza Vaccine Grouping Value Set
					//this code is only one eligible SNOMEDCT code used for exceptions, but we will add it to the list here
					listValueSetOIDs.Add("2.16.840.1.113883.3.526.3.1255");//Influenza Vaccination Declined Grouping Value Set
					measureCur.DictPatNumListProblems=GetProblems(listEhrPatNums,listValueSetOIDs,dateStart.AddDays(-92),dateStart.AddDays(91));
					//these value sets are for exception criteria and are all SNOMEDCT codes, we will add to the list here
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.526.3.1253" };//Diagnosis, Active: Allergy to eggs Grouping Value Set, all SNOMEDCT codes
					listValueSetOIDs.Add("2.16.840.1.113883.3.526.3.1256");//Diagnosis, Active: Allergy to Influenza Vaccine Grouping Value Set, SNOMEDCT codes
					listValueSetOIDs.Add("2.16.840.1.113883.3.526.3.1257");//Diagnosis, Active: Intolerance to Influenza Vaccine Grouping Value Set, SNOMEDCT codes
					Dictionary<long,List<EhrCqmProblem>> dictPatNumListProblems=GetProblems(listEhrPatNums,listValueSetOIDs,DateTime.MinValue,dateStart.AddDays(91));
					//The previous receipt of vaccine and declined vaccine exception must occur during "Occurrence A of..."
					//but the allergy and intolerance diagnoses can be any time before or during "Occurrence A of..."
					//Add allergy and intolerance problems to the previous receipt and declined vaccine problem list of measureCur
					List<long> listPatNums=new List<long>(dictPatNumListProblems.Keys);
					for(int i=0;i<listPatNums.Count;i++) {
						if(measureCur.DictPatNumListProblems.ContainsKey(listPatNums[i])) {
							measureCur.DictPatNumListProblems[listPatNums[i]].AddRange(dictPatNumListProblems[listPatNums[i]]);
						}
						else {
							measureCur.DictPatNumListProblems.Add(listPatNums[i],dictPatNumListProblems[listPatNums[i]]);
						}
					}
					#endregion
					#endregion
					#region Get Exceptions
					//All other exception items, besides the not performed items below, have been retrieved in the regions above
					#region Get Not Performed Items
					//These will also have to have taken place during the "Occurrence A of" encounter/procedure so limited to Oct 1 of previous year to March 31 of period year
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.526.3.1254" };//Medication, Administered: Influenza Vaccine Grouping Value Set
					listValueSetOIDs.Add("2.16.840.1.113883.3.526.3.402");//Procedure, Performed: Influenza Vaccination Grouping Value Set
					listReasonOIDs=new List<string>() { "2.16.840.1.113883.3.526.3.1007" };//Medical Reason Grouping Value Set
					listReasonOIDs.Add("2.16.840.1.113883.3.526.3.1008");//Patient Reason Grouping Value Set
					listReasonOIDs.Add("2.16.840.1.113883.3.526.3.1009");//System Reason Grouping Value Set
					measureCur.DictPatNumListNotPerfs=GetNotPerformeds(listEhrPatNums,listValueSetOIDs,listReasonOIDs,dateStart.AddDays(-92),dateStart.AddDays(91));
					#endregion
					#endregion
					break;
				#endregion
				#region WeightChild_X_1 Height, Weight, and BMI Exams
				case QualityType2014.WeightChild_1_1:
				case QualityType2014.WeightChild_2_1:
				case QualityType2014.WeightChild_3_1:
				case QualityType2014.WeightChild_1_2://the WeightChild_x_2 and _3 measures shouldn't get here, they're filled above with a copy of _1 measures
				case QualityType2014.WeightChild_2_2://left here just in case the calling order is switched and they happen to be called before the _1's
				case QualityType2014.WeightChild_3_2:
				case QualityType2014.WeightChild_1_3:
				case QualityType2014.WeightChild_2_3:
				case QualityType2014.WeightChild_3_3:
					//For each group of three measures, the first of the group will get all encounters, patient data, pregnancy diagnoses, vitalsign exams and interventions for that age group.
					//All three will use the same patient data.  Example 1_1 will get all data used by 1_1, 1_2, and 1_3.
					//Strategy for the weight child measures: Get eligible encounters for patients in age range that occurred during the measurement period
					//These patients are the initial patient population
					//Exclusion: Pregnant during any of the measurement period
					//Numerator 1: There is a vital sign exam with height, weight, and BMI percentile recorded during the measurement period
					//Numerator 2: There is an intervention for nutrition counseling during the measurement period
					//Numerator 3: There is an intervention for physical activity during the measurement period
					#region Get Initial Patient Population
					#region Get All Eligible Encounters
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1048");//Face-to-Face Interaction Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1016");//Home Healthcare Services Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1001");//Office Visit Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1024");//Preventive Care - Established Office Visit, 0 to 17 Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1027");//Preventive Care Services - Group Counseling Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1026");//Preventive Care Services-Individual Counseling Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1022");//Preventive Care- Initial Office Visit, 0 to 17 Grouping Value Set
					string child3To16Command=encounterSelectWhere
						+"AND patient.Birthdate<="+POut.Date(dateStart)+"-INTERVAL 3 YEAR "//age >= 3 at start of measurement period
						+"AND patient.Birthdate>"+POut.Date(dateStart)+"-INTERVAL 17 YEAR "//age < 17 at start of measurement period
						+encounterOrder;
					string child3To11Command=encounterSelectWhere
						+"AND patient.Birthdate<="+POut.Date(dateStart)+"-INTERVAL 3 YEAR "//age >= 3 at start of measurement period
						+"AND patient.Birthdate>"+POut.Date(dateStart)+"-INTERVAL 12 YEAR "//age <= 11 at start of measurement period
						+encounterOrder;
					string child12To16Command=encounterSelectWhere
						+"AND patient.Birthdate<="+POut.Date(dateStart)+"-INTERVAL 12 YEAR "//age >= 12 at start of measurement period
						+"AND patient.Birthdate>"+POut.Date(dateStart)+"-INTERVAL 17 YEAR "//age < 17 at start of measurement period
						+encounterOrder;
					if(new[] { QualityType2014.WeightChild_1_1,QualityType2014.WeightChild_1_2,QualityType2014.WeightChild_1_3 }.Contains(qtype)) {
						measureCur.DictPatNumListEncounters=GetEncountersWithOneOfAndTwoOfOIDs(child3To16Command,listOneOfEncOIDs,listTwoOfEncOIDs);
					}
					else if(new[] { QualityType2014.WeightChild_2_1,QualityType2014.WeightChild_2_2,QualityType2014.WeightChild_2_3 }.Contains(qtype)) {
						measureCur.DictPatNumListEncounters=GetEncountersWithOneOfAndTwoOfOIDs(child3To11Command,listOneOfEncOIDs,listTwoOfEncOIDs);
					}
					else if(new[] { QualityType2014.WeightChild_3_1,QualityType2014.WeightChild_3_2,QualityType2014.WeightChild_3_3 }.Contains(qtype)) {
						measureCur.DictPatNumListEncounters=GetEncountersWithOneOfAndTwoOfOIDs(child12To16Command,listOneOfEncOIDs,listTwoOfEncOIDs);
					}
					#endregion
					#region Get Patient Data From Encounters
					measureCur.ListEhrPats=GetEhrPatsFromEncsOrProcs(measureCur.DictPatNumListEncounters);
					listEhrPatNums=GetListPatNums(measureCur.ListEhrPats);
					#endregion
					#endregion
					#region Get Pregnancy Diagnoses
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.526.3.378" };//Pregnancy Grouping Value Set
					measureCur.DictPatNumListProblems=GetProblems(listEhrPatNums,listValueSetOIDs,dateStart,dateEnd);
					#endregion
					#region Get Vitalsign Exams
					measureCur.DictPatNumListVitalsigns=GetVitalsignsForBMI(listEhrPatNums,dateStart,dateEnd);
					#endregion
					#region Get Interventions
					if(listEhrPatNums!=null && listEhrPatNums.Count==0) {
						measureCur.DictPatNumListInterventions=new SerializableDictionary<long,List<EhrCqmIntervention>>();
					}
					else {
						//DictPatNumListInterventions will hold the phys activity and Nutrition counseling interventions
						command="SELECT * FROM intervention "
							+"WHERE DATE(DateEntry) BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" ";
						if(listEhrPatNums!=null && listEhrPatNums.Count>0) {
							command+="AND intervention.PatNum IN("+string.Join(",",listEhrPatNums)+") ";
						}
						command+="ORDER BY PatNum,DateEntry DESC";
						listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.464.1003.195.12.1003" };//Counseling for Nutrition Grouping Value Set
						listValueSetOIDs.Add("2.16.840.1.113883.3.464.1003.118.12.1035");//Counseling for Physical Activity Grouping Value Set
						measureCur.DictPatNumListInterventions=GetInterventions(command,listValueSetOIDs);
					}
					#endregion
					break;
				#endregion
				#region BloodPressureManage
				case QualityType2014.BloodPressureManage:
					#region Get Initial Patient Population
					#region Get Raw Encounters
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.526.3.1240");//Annual Wellness Visit Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1001");//Office Visit Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1008");//Outpatient Consultation Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1016");//Home Healthcare Services Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1023");//Preventive Care Services-Initial Office Visit, 18 and Up Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1025");//Preventive Care Services - Established Office Visit, 18 and Up Grouping Value Set
					listOneOfEncOIDs.Add("2.16.840.1.113883.3.464.1003.101.12.1048");//Face-to-Face Interaction Grouping Value Set
					string encsWhere18To85=encounterSelectWhere
						+"AND patient.Birthdate>"+POut.Date(dateStart)+"-INTERVAL 85 YEAR "//< 85 years old at the start of the measurement period
						+encounterOrder;
					measureCur.DictPatNumListEncounters=GetEncountersWithOneOfAndTwoOfOIDs(encsWhere18To85,listOneOfEncOIDs,listTwoOfEncOIDs);
					#endregion
					#region Get Essential Hypertension Problems
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.464.1003.104.12.1011" };//Essential Hypertension Grouping Value Set
					//hypertension problem must start within 6 months of start of measurement period
					//if measurement period end date is less than 6 months after the start date,
					//problem start date is limited by end date not 6 months after start date as that would start after the end of the measurement period
					DateTime probDateEnd=dateEnd;
					if(dateStart.AddMonths(6)<dateEnd) {
						probDateEnd=dateStart.AddMonths(6);
					}
					Dictionary<long,List<EhrCqmProblem>> dictPatNumListHypertension=GetProblems(null,listValueSetOIDs,dateStart,probDateEnd);
					#endregion
					#region Remove If No Hypertension Diagnosis
					//if the patient with eligible encounter list has not been diagnosed with hypertension within 6 months of the start of the measurement period, remove from IPP
					//(diagnosis <= 6 months starts after dateStart) OR (diagnosis starts before dateStart AND NOT diagnosis ends before dateStart)
					List<long> allEncKeys=new List<long>(measureCur.DictPatNumListEncounters.Keys);
					for(int i=0;i<allEncKeys.Count;i++) {
						if(!dictPatNumListHypertension.ContainsKey(allEncKeys[i])) {
							measureCur.DictPatNumListEncounters.Remove(allEncKeys[i]);
						}
					}
					#endregion
					#region Get Patient Data
					//encounters are now eligible and only if hypertension diagnosis within 6 months of start of measurement period (or before endDate if sooner than 6 months after start date)
					measureCur.ListEhrPats=GetEhrPatsFromEncsOrProcs(measureCur.DictPatNumListEncounters);
					listEhrPatNums=GetListPatNums(measureCur.ListEhrPats);
					#endregion
					#endregion
					#region Get Exclusion Items
					#region Get Exclusion Diagnoses And Hypertension Diagnoses
					//ListEhrPats is now initial patient population, get hyptertension, pregnancy, end stage renal disease, and chronic kidney disease diagnoses
					//This time the hypertension diagnoses will be for the entire measurement period, not just within the 6 months after the start date.
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.464.1003.104.12.1011" };//Essential Hypertension Grouping Value Set
					listValueSetOIDs.Add("2.16.840.1.113883.3.526.3.378");//Pregnancy Grouping Value Set
					listValueSetOIDs.Add("2.16.840.1.113883.3.526.3.353");//End Stage Renal Disease Grouping Value Set
					listValueSetOIDs.Add("2.16.840.1.113883.3.526.3.1002");//Chronic Kidney Disease, Stage 5 Grouping Value Set
					measureCur.DictPatNumListProblems=GetProblems(listEhrPatNums,listValueSetOIDs,dateStart,dateEnd);
					#endregion
					#region Get Interventions For Exclusion
					if(listEhrPatNums!=null && listEhrPatNums.Count==0) {
						measureCur.DictPatNumListInterventions=new SerializableDictionary<long,List<EhrCqmIntervention>>();
					}
					else {
						listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.464.1003.109.12.1015" };//Other Services Related to Dialysis Grouping Value Set
						listValueSetOIDs.Add("2.16.840.1.113883.3.464.1003.109.12.1016");//Dialysis Education Grouping Value Set
						//Get all interventions for eligible value sets that occurred before or during the measurement period
						command="SELECT * FROM intervention "
							+"WHERE DATE(DateEntry)<="+POut.Date(dateEnd)+" ";
						if(listEhrPatNums!=null && listEhrPatNums.Count>0) {
							command+="AND intervention.PatNum IN("+string.Join(",",listEhrPatNums)+") ";
						}
						command+="ORDER BY PatNum,DateEntry DESC";
						measureCur.DictPatNumListInterventions=GetInterventions(command,listValueSetOIDs);
					}
					#endregion
					#region Get Procedures For Exclusion
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.464.1003.109.12.1011" };//Vascular Access for Dialysis Grouping Value Set
					listValueSetOIDs.Add("2.16.840.1.113883.3.464.1003.109.12.1012");//Kidney Transplant Grouping Value Set
					listValueSetOIDs.Add("2.16.840.1.113883.3.464.1003.109.12.1013");//Dialysis Services Grouping Value Set
					measureCur.DictPatNumListProcs=GetProcs(listEhrPatNums,listValueSetOIDs,DateTime.MinValue,dateEnd);
					#endregion
					#region Get Encounters For Exclusion
					//Get encounters that occurred before or during the measurement period that belong to the value set and add them to DictPatNumListEncounters
					//these encounters will be only for those patients already in the IPP, and will simply be added to the list of encounters for that patient
					//they will be used for exclusion in the Classify function, if one exists the patient is in the IPP and denominator but excluded from the measure
					listValueSetOIDs=new List<string>() { "2.16.840.1.113883.3.464.1003.109.12.1014" };//ESRD Monthly Outpatient Services Grouping Value Set
					command="SELECT encounter.* FROM encounter "
						+"INNER JOIN patient ON patient.PatNum=encounter.PatNum "
						+"WHERE YEAR(patient.Birthdate)>1880 "//valid birthdate
						+"AND encounter.ProvNum IN("+POut.String(provs)+") "
						+"AND DATE(encounter.DateEncounter)<="+POut.Date(dateEnd)+" ";
					if(listEhrPatNums!=null && listEhrPatNums.Count>0) {
						command+="AND encounter.PatNum IN("+string.Join(",",listEhrPatNums)+") ";
					}
					SerializableDictionary<long,List<EhrCqmEncounter>> dictPatNumListEncs=GetEncountersWithOneOfAndTwoOfOIDs(command,listValueSetOIDs,listTwoOfEncOIDs);
					//add to the list of encounters for all patients in the IPP any end stage renal disease encounters
					allKeys=new List<long>(dictPatNumListEncs.Keys);
					for(int i=0;i<allKeys.Count;i++) {
						if(measureCur.DictPatNumListEncounters.ContainsKey(allKeys[i])) {
							measureCur.DictPatNumListEncounters[allKeys[i]].AddRange(dictPatNumListEncs[allKeys[i]]);
						}
					}
					#endregion
					#endregion
					#region Get Vitalsign Exams For BP
					measureCur.DictPatNumListVitalsigns=GetVitalsignsForBP(listEhrPatNums,dateStart,dateEnd);
					#endregion
					break;
				#endregion
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
			return measureCur;
		}

		///<summary>Simple helper function to get a list of PatNums from a list of EhrCqmPatient objects.  Used to limit the number of records returned in other Get functions below, like GetProcs.</summary>
		private static List<long> GetListPatNums(List<EhrCqmPatient> listEhrCqmPats) {
			List<long> retval=new List<long>();
			for(int i=0;i<listEhrCqmPats.Count;i++) {
				retval.Add(listEhrCqmPats[i].EhrCqmPat.PatNum);
			}
			return retval;
		}

		///<summary>The string command will retrieve all unique encounters in the date range, for the provider (based on provider first and last name, so may be more than one ProvNum), with age limitation or other restrictions applied.  The encounters will then be required to belong to the value sets identified by the oneOf and twoOf lists of OID's (Object Identifiers), and the patient will have to have had one or more of the oneOf encounters or two or more of the two of encounters in the list returned by the string command.  We will return a dictionary with PatNum as the key that links to a list of all EhrCqmEncounter objects for that patient with all of the required elements for creating the QRDA Category I and III documents.</summary>
		private static SerializableDictionary<long,List<EhrCqmEncounter>> GetEncountersWithOneOfAndTwoOfOIDs(string command,List<string> listOneOfEncOIDs,List<string> listTwoOfEncOIDs) {
			SerializableDictionary<long,List<EhrCqmEncounter>> retval=new SerializableDictionary<long,List<EhrCqmEncounter>>();
			List<Encounter> listEncs=Crud.EncounterCrud.SelectMany(command);
			if(listEncs.Count==0) {
				return retval;
			}
			List<EhrCode> listOneOfEncs=EhrCodes.GetForValueSetOIDs(listOneOfEncOIDs,false);
			List<EhrCode> listTwoOfEncs=EhrCodes.GetForValueSetOIDs(listTwoOfEncOIDs,false);
			Dictionary<long,int> dictPatNumAndTwoOfCount=new Dictionary<long,int>();
			List<long> listPatNums=new List<long>();
			Dictionary<long,EhrCode> dictEncNumEhrCode=new Dictionary<long,EhrCode>();
			//Remove any encounters that are not one of the allowed types and create a list of patients who had 1 or more of the OneOf encounters and a dictionary with PatNum,Count
			//for counting the number of TwoOf encounters for each patient
			for(int i=listEncs.Count-1;i>-1;i--) {
				bool isOneOf=false;
				for(int j=0;j<listOneOfEncs.Count;j++) {
					if(listEncs[i].CodeValue==listOneOfEncs[j].CodeValue && listEncs[i].CodeSystem==listOneOfEncs[j].CodeSystem) {
						if(!listPatNums.Contains(listEncs[i].PatNum)) {
							listPatNums.Add(listEncs[i].PatNum);
						}
						dictEncNumEhrCode.Add(listEncs[i].EncounterNum,listOneOfEncs[j]);
						isOneOf=true;
						break;
					}
				}
				if(isOneOf) {
					continue;
				}
				bool isTwoOf=false;
				for(int j=0;j<listTwoOfEncs.Count;j++) {
					if(listEncs[i].CodeValue==listTwoOfEncs[j].CodeValue && listEncs[i].CodeSystem==listTwoOfEncs[j].CodeSystem) {
						if(dictPatNumAndTwoOfCount.ContainsKey(listEncs[i].PatNum)) {
							dictPatNumAndTwoOfCount[listEncs[i].PatNum]++;
						}
						else {
							dictPatNumAndTwoOfCount.Add(listEncs[i].PatNum,1);
						}
						dictEncNumEhrCode.Add(listEncs[i].EncounterNum,listTwoOfEncs[j]);
						isTwoOf=true;
						break;
					}
				}
				if(!isTwoOf) {//not oneOf or twoOf encounter, remove from list
					listEncs.RemoveAt(i);//not an eligible encounter
				}
			}
			//add the patients who had 2 or more of the TwoOf encounters to the list of patients
			foreach(KeyValuePair<long,int> kpairCur in dictPatNumAndTwoOfCount) {
				if(listPatNums.Contains(kpairCur.Key)) {
					continue;
				}
				if(kpairCur.Value>1) {
					listPatNums.Add(kpairCur.Key);
				}
			}
			//remove any encounters from the list for patients who did not have a OneOf or two or more of the TwoOf encounters.
			for(int i=listEncs.Count-1;i>-1;i--) {
				if(!listPatNums.Contains(listEncs[i].PatNum)) {
					listEncs.RemoveAt(i);
				}
			}
			//listEncs is now all encounters returned by the command (should be date restricted, age restricted, provider restricted, etc.) that belong to the OneOf or TwoOf list
			//for patients who had one or more of the OneOf encounters and/or two or more of the TwoOf encounters
			//dictEncNumEhrCode links an EncounterNum to an EhrCode object.  This will be an easy way to get the ValueSetOID, ValueSetName, and CodeSystemOID for the encounter.
			for(int i=0;i<listEncs.Count;i++) {
				EhrCqmEncounter ehrEncCur=new EhrCqmEncounter();
				ehrEncCur.EhrCqmEncounterNum=listEncs[i].EncounterNum;
				ehrEncCur.PatNum=listEncs[i].PatNum;
				ehrEncCur.ProvNum=listEncs[i].ProvNum;
				ehrEncCur.CodeValue=listEncs[i].CodeValue;
				ehrEncCur.CodeSystemName=listEncs[i].CodeSystem;
				ehrEncCur.DateEncounter=listEncs[i].DateEncounter;
				EhrCode ehrCodeCur=dictEncNumEhrCode[listEncs[i].EncounterNum];
				ehrEncCur.ValueSetName=ehrCodeCur.ValueSetName;
				ehrEncCur.ValueSetOID=ehrCodeCur.ValueSetOID;
				ehrEncCur.CodeSystemOID=ehrCodeCur.CodeSystemOID;
				string descript="";
				descript=ehrEncCur.Description;//in case not in table default to EhrCode object description
				//to get description, first determine which table the code is from.  Encounter is only allowed to be a CDT, CPT, HCPCS, and SNOMEDCT.
				switch(ehrEncCur.CodeSystemName) {
					case "CDT":
						descript=ProcedureCodes.GetProcCode(ehrEncCur.CodeValue).Descript;
						break;
					case "CPT":
						Cpt cptCur=Cpts.GetByCode(ehrEncCur.CodeValue);
						if(cptCur!=null) {
							descript=cptCur.Description;
						}
						break;
					case "HCPCS":
						Hcpcs hCur=Hcpcses.GetByCode(ehrEncCur.CodeValue);
						if(hCur!=null) {
							descript=hCur.DescriptionShort;
						}
						break;
					case "SNOMEDCT":
						Snomed sCur=Snomeds.GetByCode(ehrEncCur.CodeValue);
						if(sCur!=null) {
							descript=sCur.Description;
						}
						break;
				}
				ehrEncCur.Description=descript;
				if(retval.ContainsKey(ehrEncCur.PatNum)) {
					retval[ehrEncCur.PatNum].Add(ehrEncCur);
				}
				else {
					retval.Add(ehrEncCur.PatNum,new List<EhrCqmEncounter>() { ehrEncCur });
				}
			}
			return retval;
		}

		private static List<EhrCqmPatient> GetEhrPatsFromEncsOrProcs(Dictionary<long,List<EhrCqmEncounter>> dictPatNumListEncounters) {
			return GetEhrPatsFromEncsOrProcs(dictPatNumListEncounters,null);
		}

		///<summary>Get relevant demographic and supplemental patient data required for CQM reporting for each unique patient in the list of eligible encounters in the dictionary of PatNums linked to a list of encounters for each PatNum.</summary>
		private static List<EhrCqmPatient> GetEhrPatsFromEncsOrProcs(Dictionary<long,List<EhrCqmEncounter>> dictPatNumListEncounters,Dictionary<long,List<EhrCqmProc>> dictPatNumListProcs) {
			//get list of distinct PatNums from the keys in the incoming dictionary
			List<long> listPatNums=new List<long>(dictPatNumListEncounters.Keys);
			if(dictPatNumListProcs!=null) {
				List<long> procKeys=new List<long>(dictPatNumListProcs.Keys);
				for(int i=0;i<procKeys.Count;i++) {
					if(listPatNums.Contains(procKeys[i])) {
						continue;
					}
					listPatNums.Add(procKeys[i]);
				}
			}			
			List<EhrCqmPatient> retval=new List<EhrCqmPatient>();
			if(listPatNums.Count==0) {
				return retval;
			}
			Patient[] uniquePats=Patients.GetMultPats(listPatNums); 
			//All PayerType codes in ehrcode list are SOP codes
			List<EhrCode> listEhrCodesForPayerTypeOID=EhrCodes.GetForValueSetOIDs(new List<string>() { "2.16.840.1.114222.4.11.3591" },false);
			for(int i=0;i<uniquePats.Length;i++) {
				EhrCqmPatient ehrPatCur=new EhrCqmPatient();
				ehrPatCur.EhrCqmPat=uniquePats[i];
				ehrPatCur.ListPatientRaces=new List<PatientRace>();
				//we will set all patients to denominator.  The only measure where the list of patients (which is the initial patient population) is not the same as the denominator is the influenza vaccine measure, CMS147v2, and we will set those not in the denominator to false after we determine they are in the IPP but not the denominator
				ehrPatCur.IsDenominator=true;
				List<PatientRace> listPatRaces=PatientRaces.GetForPatient(ehrPatCur.EhrCqmPat.PatNum);
				for(int j=0;j<listPatRaces.Count;j++) {
					if(listPatRaces[j].IsEthnicity) {
						ehrPatCur.Ethnicity=listPatRaces[j];
						continue;
					}
					ehrPatCur.ListPatientRaces.Add(listPatRaces[j]);
				}
				PayorType payerTypeCur=PayorTypes.GetCurrentType(ehrPatCur.EhrCqmPat.PatNum);
				ehrPatCur.PayorSopCode="";
				ehrPatCur.PayorDescription="";
				ehrPatCur.PayorValueSetOID="";
				if(payerTypeCur!=null) {
					for(int j=0;j<listEhrCodesForPayerTypeOID.Count;j++) {
						if(listEhrCodesForPayerTypeOID[j].CodeValue==payerTypeCur.SopCode) {//add payer information if it is in the value set
							ehrPatCur.PayorSopCode=payerTypeCur.SopCode;
							ehrPatCur.PayorDescription=Sops.GetDescriptionFromCode(payerTypeCur.SopCode);
							ehrPatCur.PayorValueSetOID=listEhrCodesForPayerTypeOID[j].ValueSetOID;
							break;
						}
					}
				}
				retval.Add(ehrPatCur);
			}
			return retval;
		}

		///<summary>Get ehrmeasureevents of type TobaccoAssessment where the event code is in the Tobacco Use Screening value set and the assessment resulted in categorizing the patient as a user or non-user and the screening was within 24 months of the measurement period end date.  Ordered by PatNum, DateTEvent.</summary>
		private static SerializableDictionary<long,List<EhrCqmMeasEvent>> GetTobaccoAssessmentEvents(List<long> listPatNums,DateTime dateEnd) {
			SerializableDictionary<long,List<EhrCqmMeasEvent>> retval=new SerializableDictionary<long,List<EhrCqmMeasEvent>>();
			//if no patients, return a new empty dictionary
			if(listPatNums!=null && listPatNums.Count==0) {
				return retval;
			}
			string command="SELECT ehrmeasureevent.*,COALESCE(snomed.Description,'') AS Description "
				+"FROM ehrmeasureevent "
				+"LEFT JOIN snomed ON snomed.SnomedCode=ehrmeasureevent.CodeValueResult AND ehrmeasureevent.CodeSystemResult='SNOMEDCT' "
				+"WHERE EventType="+POut.Int((int)EhrMeasureEventType.TobaccoUseAssessed)+" " 
				+"AND "+DbHelper.DtimeToDate("DateTEvent")+">="+POut.Date(dateEnd)+"-INTERVAL 24 MONTH ";
			if(listPatNums!=null && listPatNums.Count>0) {
				command+="AND ehrmeasureevent.PatNum IN("+string.Join(",",listPatNums)+") ";
			}
			command+="ORDER BY ehrmeasureevent.PatNum,ehrmeasureevent.DateTEvent DESC";
			DataTable tableEvents=Db.GetTable(command);
			if(tableEvents.Rows.Count==0) {
				return retval;
			}
			Dictionary<string,string> dictEventCodesAndSystems=EhrCodes.GetCodeAndCodeSystem(new List<string>() { "2.16.840.1.113883.3.526.3.1278" },false);//Tobacco Use Screening Value Set
			List<string> listTobaccoStatusOIDs=new List<string>();
			listTobaccoStatusOIDs.Add("2.16.840.1.113883.3.526.3.1170");//Tobacco User Grouping Value Set
			listTobaccoStatusOIDs.Add("2.16.840.1.113883.3.526.3.1189");//Tobacco Non-User Grouping Value Set
			List<EhrCode> listTobaccoStatusCodes=EhrCodes.GetForValueSetOIDs(listTobaccoStatusOIDs,false);
			Dictionary<long,EhrCode> dictEventNumEhrCode=new Dictionary<long,EhrCode>();
			for(int i=tableEvents.Rows.Count-1;i>-1;i--) {
				bool isValidEvent=false;
				if(dictEventCodesAndSystems.ContainsKey(tableEvents.Rows[i]["CodeValueEvent"].ToString())
					&& dictEventCodesAndSystems[tableEvents.Rows[i]["CodeValueEvent"].ToString()]==tableEvents.Rows[i]["CodeSystemEvent"].ToString())
				{
					isValidEvent=true;
				}
				int indexStatus=-1;
				for(int j=0;j<listTobaccoStatusCodes.Count;j++) {
					if(listTobaccoStatusCodes[j].CodeValue==tableEvents.Rows[i]["CodeValueResult"].ToString()
						&& listTobaccoStatusCodes[j].CodeSystem==tableEvents.Rows[i]["CodeSystemResult"].ToString())
					{
						indexStatus=j;
						break;
					}
				}
				if(isValidEvent && indexStatus>-1) {
					dictEventNumEhrCode.Add(PIn.Long(tableEvents.Rows[i]["EhrMeasureEventNum"].ToString()),listTobaccoStatusCodes[indexStatus]);
					continue;
				}
				tableEvents.Rows.RemoveAt(i);
			}
			for(int i=0;i<tableEvents.Rows.Count;i++) {
				EhrCqmMeasEvent tobaccoAssessCur=new EhrCqmMeasEvent();
				tobaccoAssessCur.EhrCqmMeasEventNum=PIn.Long(tableEvents.Rows[i]["EhrMeasureEventNum"].ToString());
				tobaccoAssessCur.EventType=EhrMeasureEventType.TobaccoUseAssessed;
				tobaccoAssessCur.PatNum=PIn.Long(tableEvents.Rows[i]["PatNum"].ToString());
				tobaccoAssessCur.CodeValue=tableEvents.Rows[i]["CodeValueResult"].ToString();
				tobaccoAssessCur.CodeSystemName=tableEvents.Rows[i]["CodeSystemResult"].ToString();
				tobaccoAssessCur.DateTEvent=PIn.DateT(tableEvents.Rows[i]["DateTEvent"].ToString());
				string descript=tableEvents.Rows[i]["Description"].ToString();//if code is not in snomed table we will use description of EhrCode object
				EhrCode ehrCodeCur=dictEventNumEhrCode[tobaccoAssessCur.EhrCqmMeasEventNum];
				tobaccoAssessCur.CodeSystemOID=ehrCodeCur.CodeSystemOID;
				tobaccoAssessCur.ValueSetName=ehrCodeCur.ValueSetName;
				tobaccoAssessCur.ValueSetOID=ehrCodeCur.ValueSetOID;
				//all statuses for tobacco use are SNOMEDCT codes
				if(descript=="") {
					descript=ehrCodeCur.Description;//default to description of EhrCode object
				}
				tobaccoAssessCur.Description=descript;
				if(retval.ContainsKey(tobaccoAssessCur.PatNum)) {
					retval[tobaccoAssessCur.PatNum].Add(tobaccoAssessCur);
				}
				else {
					retval.Add(tobaccoAssessCur.PatNum,new List<EhrCqmMeasEvent>() { tobaccoAssessCur });
				}
			}
			return retval;
		}

		///<summary>Get all data needed for reporting QRDA's for interventions from the supplied command where the code belongs to the value set(s) sent in.  Command orders interventions by patnum, then date entered so the first one found for patient is most recent intervention when looping through table.</summary>
		private static SerializableDictionary<long,List<EhrCqmIntervention>> GetInterventions(string command,List<string> listValueSetOIDs) {
			SerializableDictionary<long,List<EhrCqmIntervention>> retval=new SerializableDictionary<long,List<EhrCqmIntervention>>();
			List<Intervention> listInterventions=Crud.InterventionCrud.SelectMany(command);
			if(listInterventions.Count==0) {
				return retval;
			}
			//remove any interventions that are not in the Tobacco Use Cessation Counseling Grouping Value Set
			List<EhrCode> listAllInterventionCodes=EhrCodes.GetForValueSetOIDs(listValueSetOIDs,false);//Tobacco Use Cessation Counseling Grouping Value Set
			Dictionary<long,EhrCode> dictInterventionNumEhrCode=new Dictionary<long,EhrCode>();
			for(int i=listInterventions.Count-1;i>-1;i--) {
				bool isValidIntervention=false;
				for(int j=0;j<listAllInterventionCodes.Count;j++) {
					if(listAllInterventionCodes[j].ValueSetOID!="2.16.840.1.113883.3.526.3.1190"//Tobacco Use Cessation Pharmacotherapy Grouping Value Set
						&& listInterventions[i].CodeValue==listAllInterventionCodes[j].CodeValue
						&& listInterventions[i].CodeSystem==listAllInterventionCodes[j].CodeSystem)
					{
						isValidIntervention=true;
						dictInterventionNumEhrCode.Add(listInterventions[i].InterventionNum,listAllInterventionCodes[j]);
						break;
					}
					else if(listAllInterventionCodes[j].ValueSetOID=="2.16.840.1.113883.3.526.3.1190"//Tobacco Use Cessation Pharmacotherapy Grouping Value Set
						&& listInterventions[i].CodeValue==listAllInterventionCodes[j].CodeValue
						&& listInterventions[i].CodeSystem==listAllInterventionCodes[j].CodeSystem
						&& listInterventions[i].IsPatDeclined)
					{
						//include any medication interventions if they are declined medications inserted as interventions
						isValidIntervention=true;
						dictInterventionNumEhrCode.Add(listInterventions[i].InterventionNum,listAllInterventionCodes[j]);
						break;
					}
				}
				if(!isValidIntervention) {
					listInterventions.RemoveAt(i);
				}
			}
			for(int i=0;i<listInterventions.Count;i++) {
				EhrCqmIntervention interventionCur=new EhrCqmIntervention();
				interventionCur.EhrCqmInterventionNum=listInterventions[i].InterventionNum;
				interventionCur.PatNum=listInterventions[i].PatNum;
				interventionCur.ProvNum=listInterventions[i].ProvNum;
				interventionCur.CodeValue=listInterventions[i].CodeValue;
				interventionCur.CodeSystemName=listInterventions[i].CodeSystem;
				interventionCur.DateEntry=listInterventions[i].DateEntry;
				EhrCode ehrCodeCur=dictInterventionNumEhrCode[listInterventions[i].InterventionNum];
				interventionCur.CodeSystemOID=ehrCodeCur.CodeSystemOID;
				interventionCur.ValueSetName=ehrCodeCur.ValueSetName;
				interventionCur.ValueSetOID=ehrCodeCur.ValueSetOID;
				string descript=ehrCodeCur.Description;//if not in table or not a CPT, ICD9CM, ICD10CM, HCPCS, or SNOMEDCT code, default to EhrCode object description
				switch(listInterventions[i].CodeSystem) {
					case "CPT":
						Cpt cCur=Cpts.GetByCode(listInterventions[i].CodeValue);
						if(cCur!=null) {
							descript=cCur.Description;
						}
						break;
					case "HCPCS":
						Hcpcs hCur=Hcpcses.GetByCode(listInterventions[i].CodeValue);
						if(hCur!=null) {
							descript=hCur.DescriptionShort;
						}
						break;
					case "ICD9CM":
						ICD9 i9Cur=ICD9s.GetByCode(listInterventions[i].CodeValue);
						if(i9Cur!=null) {
							descript=i9Cur.Description;
						}
						break;
					case "ICD10CM":
						Icd10 i10Cur=Icd10s.GetByCode(listInterventions[i].CodeValue);
						if(i10Cur!=null) {
							descript=i10Cur.Description;
						}
						break;
					case "SNOMEDCT":
						Snomed sCur=Snomeds.GetByCode(listInterventions[i].CodeValue);
						if(sCur!=null) {
							descript=sCur.Description;
						}
						break;
				}
				interventionCur.Description=descript;
				if(retval.ContainsKey(interventionCur.PatNum)) {
					retval[interventionCur.PatNum].Add(interventionCur);
				}
				else {
					retval.Add(interventionCur.PatNum,new List<EhrCqmIntervention>() { interventionCur });
				}
			}
			return retval;
		}

		///<summary>Get the medication information for medications where the code belongs to one of the value sets in the supplied list and the medication start date is in the supplied date range.  Ordered by PatNum,DateStart so first found for patient is most recent to make calculation easier.  If there is a PatNote, this is a Medication Order.  If there is no note and there is either no stop date or the stop date is after the measurement period end date, this is an active Medication.</summary>
		private static SerializableDictionary<long,List<EhrCqmMedicationPat>> GetMedPats(List<long> listPatNums,List<string> listValueSetOIDs,DateTime dateStart,DateTime dateEnd) {
			SerializableDictionary<long,List<EhrCqmMedicationPat>> retval=new SerializableDictionary<long,List<EhrCqmMedicationPat>>();
			//if no patients, return a new empty dictionary
			if(listPatNums!=null && listPatNums.Count==0) {
				return retval;
			}
			List<EhrCode> listEhrCodes=EhrCodes.GetForValueSetOIDs(listValueSetOIDs,false);
			string command="SELECT medicationpat.MedicationPatNum,medicationpat.PatNum,medicationpat.DateStart,medicationpat.DateStop,medicationpat.PatNote,"
				+"(CASE WHEN medication.RxCui IS NULL THEN medicationpat.RxCui ELSE medication.RxCui END) AS RxCui "
				+"FROM medicationpat "
				+"LEFT JOIN medication ON medication.MedicationNum=medicationpat.MedicationNum "
				+"WHERE (medication.RxCui IS NOT NULL OR medicationpat.RxCui>0) "
				+"AND DATE(medicationpat.DateStart) BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" ";
				//not going to check stop date, the measures only specify 'starts before or during' without any reference to whether or not the medication has stopped
				//+"AND (YEAR(medicationpat.DateStop)<1880 OR medicationpat.DateStop>"+POut.Date(dateEnd)+") "//no valid stop date or stop date after measurement period end date
			if(listPatNums!=null && listPatNums.Count>0) {
				command+="AND medicationpat.PatNum IN("+string.Join(",",listPatNums)+") ";
			}
			if(listEhrCodes.Count>0) {
				string rxcuiCodes=string.Join(",",listEhrCodes.Select(x => "'"+POut.String(x.CodeValue)+"'"));
				command+="AND (medicationpat.RxCui IN("+rxcuiCodes+") OR medication.RxCui IN("+rxcuiCodes+")) ";
			}
			command+="ORDER BY medicationpat.PatNum,medicationpat.DateStart DESC";
			DataTable tableAllMedPats=Db.GetTable(command);
			if(tableAllMedPats.Rows.Count==0) {
				return retval;
			}
			Dictionary<long,EhrCode> dictMedicationPatNumEhrCode=new Dictionary<long,EhrCode>();
			for(int i=tableAllMedPats.Rows.Count-1;i>-1;i--) {
				for(int j=0;j<listEhrCodes.Count;j++) {
					if(tableAllMedPats.Rows[i]["RxCui"].ToString()==listEhrCodes[j].CodeValue) {
						dictMedicationPatNumEhrCode.Add(PIn.Long(tableAllMedPats.Rows[i]["MedicationPatNum"].ToString()),listEhrCodes[j]);
						break;
					}
				}
			}
			for(int i=0;i<tableAllMedPats.Rows.Count;i++) {
				EhrCqmMedicationPat ehrMedPatCur=new EhrCqmMedicationPat();
				ehrMedPatCur.EhrCqmMedicationPatNum=PIn.Long(tableAllMedPats.Rows[i]["MedicationPatNum"].ToString());
				ehrMedPatCur.EhrCqmVaccinePatNum=0;
				ehrMedPatCur.PatNum=PIn.Long(tableAllMedPats.Rows[i]["PatNum"].ToString());
				ehrMedPatCur.PatNote=tableAllMedPats.Rows[i]["PatNote"].ToString();
				ehrMedPatCur.RxCui=PIn.Long(tableAllMedPats.Rows[i]["RxCui"].ToString());
				ehrMedPatCur.DateStart=PIn.Date(tableAllMedPats.Rows[i]["DateStart"].ToString());
				ehrMedPatCur.DateStop=PIn.Date(tableAllMedPats.Rows[i]["DateStop"].ToString());
				EhrCode ehrCodeCur=dictMedicationPatNumEhrCode[ehrMedPatCur.EhrCqmMedicationPatNum];
				ehrMedPatCur.CodeSystemName=ehrCodeCur.CodeSystem;
				ehrMedPatCur.CodeSystemOID=ehrCodeCur.CodeSystemOID;
				ehrMedPatCur.ValueSetName=ehrCodeCur.ValueSetName;
				ehrMedPatCur.ValueSetOID=ehrCodeCur.ValueSetOID;
				string descript=ehrCodeCur.Description;
				RxNorm rCur=RxNorms.GetByRxCUI(ehrMedPatCur.RxCui.ToString());
				if(rCur!=null) {
					descript=rCur.Description;
				}
				ehrMedPatCur.Description=descript;//description either from rxnorm table or, if not in table, default to EhrCode object description
				if(retval.ContainsKey(ehrMedPatCur.PatNum)) {
					retval[ehrMedPatCur.PatNum].Add(ehrMedPatCur);
				}
				else {
					retval.Add(ehrMedPatCur.PatNum,new List<EhrCqmMedicationPat>() { ehrMedPatCur });
				}
			}
			return retval;
		}

		///<summary>Get all NotPerformed items that belong to one of the ValueSetOIDs in listItemOIDs, with valid reasons that belong to one of the ValueSetOIDs in listReasonOIDs, that were entered between dateStart and dateStop.  For QRDA reporting, the resulting list must include the item not performed, a code for 'reason', and the code for the specific reason.  Example: If not administering a flu vaccine, you would have the code not being done (like CVX 141 "Influenza, seasonal, injectable"), the code for 'reason' (like SNOMEDCT 281000124100 "Patient reason for exclusion from performance measure (observable entity)"), and the code for the specific reason (like SNOMEDCT 105480006 "Refusal of treatment by patient (situation)").  Not fun.</summary>
		private static SerializableDictionary<long,List<EhrCqmNotPerf>> GetNotPerformeds(List<long> listPatNums,List<string> listValueSetOIDs,List<string> listReasonOIDs,DateTime dateStart,DateTime dateEnd) {
			SerializableDictionary<long,List<EhrCqmNotPerf>> retval=new SerializableDictionary<long,List<EhrCqmNotPerf>>();
			//if no patients, return a new empty dictionary
			if(listPatNums!=null && listPatNums.Count==0) {
				return retval;
			}
			List<EhrCode> listItems=EhrCodes.GetForValueSetOIDs(listValueSetOIDs,false);
			List<EhrCode> listReasons=EhrCodes.GetForValueSetOIDs(listReasonOIDs,false);
			//Reasons not done come from these value sets for our 9 CQMs:
			//Medical Reason Grouping Value Set 2.16.840.1.113883.3.526.3.1007
			//Patient Reason Grouping Value Set 2.16.840.1.113883.3.526.3.1008
			//System Reason Grouping Value Set 2.16.840.1.113883.3.526.3.1009
			//Patient Reason Refused SNOMED-CT Value Set 2.16.840.1.113883.3.600.1.1503 (this is a sub-set of Patient Reason ...1008 above)
			//Medical or Other reason not done SNOMED-CT Value Set 2.16.840.1.113883.3.600.1.1502 (this is a sub-set of Medical Reason ...1007 above)
			string command="SELECT ehrnotperformed.*, "
				+"COALESCE(snomed.Description,loinc.NameLongCommon,cpt.Description,cvx.Description,'') AS Description, "
				+"COALESCE(sReason.Description,'') AS DescriptionReason "
				+"FROM ehrnotperformed "
				+"LEFT JOIN cpt ON cpt.CptCode=ehrnotperformed.CodeValue AND ehrnotperformed.CodeSystem='CPT' "
				+"LEFT JOIN cvx ON cvx.CvxCode=ehrnotperformed.CodeValue AND ehrnotperformed.CodeSystem='CVX' "
				+"LEFT JOIN loinc ON loinc.LoincCode=ehrnotperformed.CodeValue AND ehrnotperformed.CodeSystem='LOINC' "
				+"LEFT JOIN snomed ON snomed.SnomedCode=ehrnotperformed.CodeValue AND ehrnotperformed.CodeSystem='SNOMEDCT' "
				+"LEFT JOIN snomed sReason ON sReason.SnomedCode=ehrnotperformed.CodeValueReason AND ehrnotperformed.CodeSystemReason='SNOMEDCT' "
				+"WHERE ehrnotperformed.DateEntry BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" ";
			if(listPatNums!=null && listPatNums.Count>0) {
				command+="AND ehrnotperformed.PatNum IN("+string.Join(",",listPatNums)+") ";
			}
			if(listItems.Count>0) {
				command+="AND ehrnotperformed.CodeValue IN("+string.Join(",",listItems.Select(x => "'"+POut.String(x.CodeValue)+"'"))+") ";
			}
			if(listReasons.Count>0) {
				command+="AND ehrnotperformed.CodeValueReason IN("+string.Join(",",listReasons.Select(x => "'"+POut.String(x.CodeValue)+"'"))+") ";
			}
			command+="GROUP BY ehrnotperformed.EhrNotPerformedNum "//just in case a code was in one of the code system tables more than once, should never happen
				+"ORDER BY ehrnotperformed.PatNum,ehrnotperformed.DateEntry DESC";
			DataTable tableNotPerfs=Db.GetTable(command);
			if(tableNotPerfs.Rows.Count==0) {
				return retval;
			}
			Dictionary<long,EhrCode> dictItemNumEhrCode=new Dictionary<long,EhrCode>();
			Dictionary<long,EhrCode> dictReasonNumEhrCode=new Dictionary<long,EhrCode>();
			//loop through items and remove if not in valid value set or if reason is not in valid reason value set
			//link the item to the EhrCode object for both the item code and the reason code using dictionaries for retrieving required data for QRDA reports
			for(int i=tableNotPerfs.Rows.Count-1;i>-1;i--) {
				for(int j=0;j<listItems.Count;j++) {
					if(tableNotPerfs.Rows[i]["CodeValue"].ToString()==listItems[j].CodeValue
						&& tableNotPerfs.Rows[i]["CodeSystem"].ToString()==listItems[j].CodeSystem)
					{
						dictItemNumEhrCode.Add(PIn.Long(tableNotPerfs.Rows[i]["EhrNotPerformedNum"].ToString()),listItems[j]);
						break;
					}
				}
				for(int j=0;j<listReasons.Count;j++) {
					if(tableNotPerfs.Rows[i]["CodeValueReason"].ToString()==listReasons[j].CodeValue
						&& tableNotPerfs.Rows[i]["CodeSystemReason"].ToString()==listReasons[j].CodeSystem)
					{
						dictReasonNumEhrCode.Add(PIn.Long(tableNotPerfs.Rows[i]["EhrNotPerformedNum"].ToString()),listReasons[j]);
						break;
					}
				}
			}
			for(int i=0;i<tableNotPerfs.Rows.Count;i++) {
				EhrCqmNotPerf ehrNotPerfCur=new EhrCqmNotPerf();
				ehrNotPerfCur.EhrCqmNotPerfNum=PIn.Long(tableNotPerfs.Rows[i]["EhrNotPerformedNum"].ToString());
				ehrNotPerfCur.PatNum=PIn.Long(tableNotPerfs.Rows[i]["PatNum"].ToString());
				ehrNotPerfCur.CodeValue=tableNotPerfs.Rows[i]["CodeValue"].ToString();
				ehrNotPerfCur.CodeSystemName=tableNotPerfs.Rows[i]["CodeSystem"].ToString();
				ehrNotPerfCur.CodeValueReason=tableNotPerfs.Rows[i]["CodeValueReason"].ToString();
				ehrNotPerfCur.CodeSystemNameReason=tableNotPerfs.Rows[i]["CodeSystemReason"].ToString();
				ehrNotPerfCur.DateEntry=PIn.Date(tableNotPerfs.Rows[i]["DateEntry"].ToString());
				EhrCode itemEhrCode=dictItemNumEhrCode[ehrNotPerfCur.EhrCqmNotPerfNum];
				ehrNotPerfCur.CodeSystemOID=itemEhrCode.CodeSystemOID;
				ehrNotPerfCur.ValueSetName=itemEhrCode.ValueSetName;
				ehrNotPerfCur.ValueSetOID=itemEhrCode.ValueSetOID;
				EhrCode reasonEhrCode=dictReasonNumEhrCode[ehrNotPerfCur.EhrCqmNotPerfNum];
				ehrNotPerfCur.CodeSystemOIDReason=reasonEhrCode.CodeSystemOID;
				ehrNotPerfCur.ValueSetNameReason=reasonEhrCode.ValueSetName;
				ehrNotPerfCur.ValueSetOIDReason=reasonEhrCode.ValueSetOID;
				string descript=tableNotPerfs.Rows[i]["Description"].ToString();
				if(descript=="") {//just in case not found in table, will default to description of EhrCode object
					descript=itemEhrCode.Description;
				}
				ehrNotPerfCur.Description=descript;
				string reasonDescript=tableNotPerfs.Rows[i]["DescriptionReason"].ToString();
				if(reasonDescript=="") {//just in case not found in table, will default to description of EhrCode object
					reasonDescript=reasonEhrCode.Description;
				}
				ehrNotPerfCur.DescriptionReason=reasonDescript;
				if(retval.ContainsKey(ehrNotPerfCur.PatNum)) {
					retval[ehrNotPerfCur.PatNum].Add(ehrNotPerfCur);
				}
				else {
					retval.Add(ehrNotPerfCur.PatNum,new List<EhrCqmNotPerf>() { ehrNotPerfCur });
				}
			}
			return retval;
		}

		///<summary>Get all problems that started before or during the date range, that have a code that belong to the value sets in listProbOIDs, and that either have no stop date or the stop date is after dateStart.</summary>
		private static SerializableDictionary<long,List<EhrCqmProblem>> GetProblems(List<long> listPatNums,List<string> listValueSetOIDs,DateTime dateStart,DateTime dateEnd) {
			SerializableDictionary<long,List<EhrCqmProblem>> retval=new SerializableDictionary<long,List<EhrCqmProblem>>();
			//if no patients, return a new empty dictionary
			if(listPatNums!=null && listPatNums.Count==0) {
				return retval;
			}
			List<EhrCode> listValidProbs=EhrCodes.GetForValueSetOIDs(listValueSetOIDs,false);
			string icd9CodeList="";
			string icd10CodeList="";
			string snomedCodeList="";
			for(int i=0;i<listValidProbs.Count;i++) {
				if(listValidProbs[i].CodeSystem=="ICD9CM") {
					if(icd9CodeList!="") {
						icd9CodeList+=",";
					}
					icd9CodeList+="'"+listValidProbs[i].CodeValue+"'";
				}
				else if(listValidProbs[i].CodeSystem=="ICD10CM") {
					if(icd10CodeList!="") {
						icd10CodeList+=",";
					}
					icd10CodeList+="'"+listValidProbs[i].CodeValue+"'";
				}
				else if(listValidProbs[i].CodeSystem=="SNOMEDCT") {
					if(snomedCodeList!="") {
						snomedCodeList+=",";
					}
					snomedCodeList+="'"+listValidProbs[i].CodeValue+"'";
				}
			}
			string command="SELECT disease.DiseaseNum,disease.PatNum,disease.DateStart,disease.DateStop,"
				+"diseasedef.SnomedCode,diseasedef.ICD9Code,diseasedef.Icd10Code,"
				+"COALESCE(snomed.Description,icd9.Description,icd10.Description,diseasedef.DiseaseName) AS Description "
				+"FROM disease INNER JOIN diseasedef ON disease.DiseaseDefNum=diseasedef.DiseaseDefNum "
				+"LEFT JOIN snomed ON snomed.SnomedCode=diseasedef.SnomedCode "
				+"LEFT JOIN icd9 ON icd9.ICD9Code=diseasedef.ICD9Code "
				+"LEFT JOIN icd10 ON icd10.Icd10Code=diseasedef.Icd10Code "
				+"WHERE disease.DateStart<="+POut.Date(dateEnd)+" "
				+"AND (YEAR(disease.DateStop)<1880 OR disease.DateStop>"+POut.Date(dateStart)+") ";
			if(listPatNums!=null && listPatNums.Count>0) {
				command+="AND disease.PatNum IN("+string.Join(",",listPatNums)+") ";
			}
			if(icd9CodeList!="" || icd10CodeList!="" || snomedCodeList!="") {
				command+="AND (";
				if(icd9CodeList!="") {
					command+="diseasedef.ICD9Code IN("+icd9CodeList+") ";
				}
				if(icd10CodeList!="") {
					if(icd9CodeList!="") {
						command+="OR ";
					}
					command+="diseasedef.ICD10Code IN("+icd10CodeList+") ";
				}
				if(snomedCodeList!="") {
					if(icd9CodeList!="" || icd10CodeList!="") {
						command+="OR ";
					}
					command+="diseasedef.SnomedCode IN("+snomedCodeList+") ";
				}
				command+=") ";
			}
			command+="ORDER BY disease.PatNum,disease.DateStart DESC";
			DataTable tableAllProbs=Db.GetTable(command);
			if(tableAllProbs.Rows.Count==0) {
				return retval;
			}
			Dictionary<long,EhrCode> dictDiseaseNumEhrCode=new Dictionary<long,EhrCode>();
			for(int i=tableAllProbs.Rows.Count-1;i>-1;i--) {
				for(int j=0;j<listValidProbs.Count;j++) {//all problems are either SNOMED, ICD9, or ICD10 codes
					if((listValidProbs[j].CodeSystem=="SNOMEDCT" && tableAllProbs.Rows[i]["SnomedCode"].ToString()==listValidProbs[j].CodeValue)
						|| (listValidProbs[j].CodeSystem=="ICD9CM" && tableAllProbs.Rows[i]["ICD9Code"].ToString()==listValidProbs[j].CodeValue)
						|| (listValidProbs[j].CodeSystem=="ICD10CM" && tableAllProbs.Rows[i]["Icd10Code"].ToString()==listValidProbs[j].CodeValue)) {
						dictDiseaseNumEhrCode.Add(PIn.Long(tableAllProbs.Rows[i]["DiseaseNum"].ToString()),listValidProbs[j]);//link the problem to the EhrCode object for retrieving information
						break;
					}
				}
			}
			for(int i=0;i<tableAllProbs.Rows.Count;i++) {
				EhrCqmProblem ehrProblemCur=new EhrCqmProblem();
				ehrProblemCur.EhrCqmProblemNum=PIn.Long(tableAllProbs.Rows[i]["DiseaseNum"].ToString());
				ehrProblemCur.PatNum=PIn.Long(tableAllProbs.Rows[i]["PatNum"].ToString());
				ehrProblemCur.DateStart=PIn.Date(tableAllProbs.Rows[i]["DateStart"].ToString());
				ehrProblemCur.DateStop=PIn.Date(tableAllProbs.Rows[i]["DateStop"].ToString());
				ehrProblemCur.Description=tableAllProbs.Rows[i]["Description"].ToString();
				EhrCode ehrCodeCur=dictDiseaseNumEhrCode[ehrProblemCur.EhrCqmProblemNum];
				ehrProblemCur.CodeValue=ehrCodeCur.CodeValue;//use the code value from the ehrcode object because diseasedef can have an ICD9CM, ICD10CM, and SNOMEDCT code, and the codes do not have to be for the same thing, so use the code that belongs to the ValueSetOID that makes it valid for this measure
				ehrProblemCur.CodeSystemName=ehrCodeCur.CodeSystem;
				ehrProblemCur.CodeSystemOID=ehrCodeCur.CodeSystemOID;
				ehrProblemCur.ValueSetName=ehrCodeCur.ValueSetName;
				ehrProblemCur.ValueSetOID=ehrCodeCur.ValueSetOID;
				if(retval.ContainsKey(ehrProblemCur.PatNum)) {
					retval[ehrProblemCur.PatNum].Add(ehrProblemCur);
				}
				else {
					retval.Add(ehrProblemCur.PatNum,new List<EhrCqmProblem>() { ehrProblemCur });
				}
			}
			return retval;
		}

		///<summary>Get all medication documented procedures that happened in the date range that belong to the value set OIDs.  These 'procedures' are actually in the ehrmeasureevent table and can only possibly be one code (restricted by value set OID), SNOMEDCT - 428191000124101 - Documentation of current medications (procedure).  Ordered by PatNum, DateTEvent DESC for making CQM calc easier, most recent 'procedure' will be the first one found for the patient in list.</summary>
		private static SerializableDictionary<long,List<EhrCqmMeasEvent>> GetMedDocumentedProcs(List<long> listPatNums,List<string> listValueSetOIDs,DateTime dateStart,DateTime dateEnd) {
			SerializableDictionary<long,List<EhrCqmMeasEvent>> retval=new SerializableDictionary<long,List<EhrCqmMeasEvent>>();
			//if no patients, return a new empty dictionary
			if(listPatNums!=null && listPatNums.Count==0) {
				return retval;
			}
			string command="SELECT ehrmeasureevent.*,COALESCE(snomed.Description,'') AS Description "
				+"FROM ehrmeasureevent "
				+"LEFT JOIN snomed ON snomed.SnomedCode=ehrmeasureevent.CodeValueEvent AND ehrmeasureevent.CodeSystemEvent='SNOMEDCT' "
				+"WHERE EventType="+POut.Int((int)EhrMeasureEventType.CurrentMedsDocumented)+" "
				+"AND DATE(DateTEvent) BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" ";
			if(listPatNums!=null && listPatNums.Count>0) {
				command+="AND ehrmeasureevent.PatNum IN("+string.Join(",",listPatNums)+") ";
			}
			command+="ORDER BY PatNum,DateTEvent DESC";
			DataTable tableEvents=Db.GetTable(command);
			if(tableEvents.Rows.Count==0) {
				return retval;
			}
			List<EhrCode> listEhrCodes=EhrCodes.GetForValueSetOIDs(listValueSetOIDs,false);
			Dictionary<long,EhrCode> dictEhrMeasureEventNumEhrCode=new Dictionary<long,EhrCode>();
			//remove 'procs' from table of ehrmeasureevents if not in the value set OIDs in listEhrCodes
			for(int i=tableEvents.Rows.Count-1;i>-1;i--) {
				bool isValid=false;
				for(int j=0;j<listEhrCodes.Count;j++) {//currently this can only be one code, SNOMEDCT - 428191000124101, but we will treat it like a list in case that changes
					if(tableEvents.Rows[i]["CodeValueEvent"].ToString()==listEhrCodes[j].CodeValue && tableEvents.Rows[i]["CodeSystemEvent"].ToString()==listEhrCodes[j].CodeSystem) {
						dictEhrMeasureEventNumEhrCode.Add(PIn.Long(tableEvents.Rows[i]["EhrMeasureEventNum"].ToString()),listEhrCodes[j]);
						isValid=true;
						break;
					}
				}
				if(!isValid) {
					tableEvents.Rows.RemoveAt(i);
				}
			}
			for(int i=0;i<tableEvents.Rows.Count;i++) {
				EhrCqmMeasEvent ehrProcCur=new EhrCqmMeasEvent();
				ehrProcCur.EhrCqmMeasEventNum=PIn.Long(tableEvents.Rows[i]["EhrMeasureEventNum"].ToString());
				ehrProcCur.EventType=EhrMeasureEventType.CurrentMedsDocumented;
				ehrProcCur.PatNum=PIn.Long(tableEvents.Rows[i]["PatNum"].ToString());
				ehrProcCur.CodeValue=tableEvents.Rows[i]["CodeValueEvent"].ToString();
				ehrProcCur.CodeSystemName=tableEvents.Rows[i]["CodeSystemEvent"].ToString();
				ehrProcCur.DateTEvent=PIn.DateT(tableEvents.Rows[i]["DateTEvent"].ToString());
				string descript=tableEvents.Rows[i]["Description"].ToString();//if code is not in snomed table we will use description of EhrCode object
				EhrCode ehrCodeCur=dictEhrMeasureEventNumEhrCode[ehrProcCur.EhrCqmMeasEventNum];
				ehrProcCur.CodeSystemOID=ehrCodeCur.CodeSystemOID;
				ehrProcCur.ValueSetName=ehrCodeCur.ValueSetName;
				ehrProcCur.ValueSetOID=ehrCodeCur.ValueSetOID;
				if(descript=="") {
					descript=ehrCodeCur.Description;//default to description of EhrCode object
				}
				ehrProcCur.Description=descript;
				if(retval.ContainsKey(ehrProcCur.PatNum)) {
					retval[ehrProcCur.PatNum].Add(ehrProcCur);
				}
				else {
					retval.Add(ehrProcCur.PatNum,new List<EhrCqmMeasEvent>() { ehrProcCur });
				}
			}
			return retval;
		}

		///<summary>Used in measure 69, BMI Screening and Follow-up and measure 155, Weight Assessment and Counseling for Nutrition and Physical Activity for Children and Adolescents.  Get all vitalsigns with DateTaken in the date range with valid height and weight.  Only one code available for a BMI exam - LOINC 39156-5 Body mass index (BMI) [Ratio].  Any vitalsign object with valid height and weight is assumed to be a LOINC 39156-5, not stored explicitly.  Results ordered by PatNum then DateTaken DESC, so MOST RECENT for each patient will be the first one in the list for that pat (i.e. dict[PatNum][0]).</summary>
		private static SerializableDictionary<long,List<EhrCqmVitalsign>> GetVitalsignsForBMI(List<long> listPatNums,DateTime dateStart,DateTime dateEnd) {
			SerializableDictionary<long,List<EhrCqmVitalsign>> retval=new SerializableDictionary<long,List<EhrCqmVitalsign>>();
			//if no patients, return a new empty dictionary
			if(listPatNums!=null && listPatNums.Count==0) {
				return retval;
			}
			string command="SELECT * FROM vitalsign "
				+"WHERE DATE(DateTaken) BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" "
				+"AND vitalsign.Height>0 AND vitalsign.Weight>0 ";
			if(listPatNums!=null && listPatNums.Count>0) {
				command+="AND vitalsign.PatNum IN("+string.Join(",",listPatNums)+") ";
			}
			command+="ORDER BY vitalsign.PatNum,vitalsign.DateTaken DESC";
			List<Vitalsign> listVitalsigns=Crud.VitalsignCrud.SelectMany(command);
			if(listVitalsigns.Count==0) {
				return retval;
			}
			//every row in the table has valid height and weight, so they are all in the value set for BMI LOINC Value LOINC Value Set - 2.16.840.1.113883.3.600.1.681 which is one code: LOINC 39156-5 Body mass index (BMI) [Ratio].
			for(int i=0;i<listVitalsigns.Count;i++) {
				EhrCqmVitalsign ehrVitalsignCur=new EhrCqmVitalsign();
				ehrVitalsignCur.BpDiastolic=-1;
				ehrVitalsignCur.BpSystolic=-1;
				ehrVitalsignCur.EhrCqmVitalsignNum=listVitalsigns[i].VitalsignNum;
				ehrVitalsignCur.PatNum=listVitalsigns[i].PatNum;
				float h=listVitalsigns[i].Height;
				ehrVitalsignCur.Height=h;
				float w=listVitalsigns[i].Weight;
				ehrVitalsignCur.Weight=w;
				ehrVitalsignCur.BMI=Math.Round((decimal)((w*703)/(h*h)),2,MidpointRounding.AwayFromZero);
				ehrVitalsignCur.HeightExamCode=listVitalsigns[i].HeightExamCode;
				Loinc lCur=Loincs.GetByCode(ehrVitalsignCur.HeightExamCode);
				if(lCur!=null) {
					ehrVitalsignCur.HeightExamDescript=lCur.NameLongCommon;
				}
				ehrVitalsignCur.WeightExamCode=listVitalsigns[i].WeightExamCode;
				lCur=Loincs.GetByCode(ehrVitalsignCur.WeightExamCode);
				if(lCur!=null) {
					ehrVitalsignCur.WeightExamDescript=lCur.NameLongCommon;
				}
				ehrVitalsignCur.BMIPercentile=listVitalsigns[i].BMIPercentile;
				ehrVitalsignCur.BMIExamCode=listVitalsigns[i].BMIExamCode;//percentile code
				lCur=Loincs.GetByCode(ehrVitalsignCur.BMIExamCode);
				if(lCur!=null) {
					ehrVitalsignCur.BMIPercentileDescript=lCur.NameLongCommon;
				}
				ehrVitalsignCur.DateTaken=listVitalsigns[i].DateTaken;
				if(retval.ContainsKey(ehrVitalsignCur.PatNum)) {
					retval[ehrVitalsignCur.PatNum].Add(ehrVitalsignCur);
				}
				else {
					retval.Add(ehrVitalsignCur.PatNum,new List<EhrCqmVitalsign>() { ehrVitalsignCur });
				}
			}
			return retval;
		}

		///<summary>Used in measure 165, Controlling High Blood Pressure.  Get all vitalsigns with DateTaken in the date range with valid BP.  Only one code available for Systolic BP, LOINC 8480-6, and one code for Diastolic, LOINC 8462-4.  Results ordered by PatNum then DateTaken DESC, so MOST RECENT for each patient will be the first one in the list for that pat (i.e. dict[PatNum][0]).</summary>
		private static SerializableDictionary<long,List<EhrCqmVitalsign>> GetVitalsignsForBP(List<long> listPatNums,DateTime dateStart,DateTime dateEnd) {
			SerializableDictionary<long,List<EhrCqmVitalsign>> retval=new SerializableDictionary<long,List<EhrCqmVitalsign>>();
			//if no patients, return a new empty dictionary
			if(listPatNums!=null && listPatNums.Count==0) {
				return retval;
			}
			string command="SELECT * FROM vitalsign "
				+"WHERE DATE(DateTaken) BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" "
				+"AND vitalsign.BpSystolic>0 AND vitalsign.BpDiastolic>0 ";
			if(listPatNums!=null && listPatNums.Count>0) {
				command+="AND vitalsign.PatNum IN("+string.Join(",",listPatNums)+") ";
			}
			command+="ORDER BY vitalsign.PatNum,vitalsign.DateTaken DESC";
			List<Vitalsign> listVitalsigns=Crud.VitalsignCrud.SelectMany(command);
			if(listVitalsigns.Count==0) {
				return retval;
			}
			//No need to get EhrCode objects from dll, every vitalsign exam with valid Systolic BP is assumed LOINC 8480-6 and valid Diastolic BP is assumed LOINC 8462-4
			for(int i=0;i<listVitalsigns.Count;i++) {
				EhrCqmVitalsign ehrVitalsignCur=new EhrCqmVitalsign();
				ehrVitalsignCur.BMI=-1;
				ehrVitalsignCur.BMIPercentile=listVitalsigns[i].BMIPercentile;
				ehrVitalsignCur.EhrCqmVitalsignNum=listVitalsigns[i].VitalsignNum;
				ehrVitalsignCur.PatNum=listVitalsigns[i].PatNum;
				ehrVitalsignCur.BpSystolic=listVitalsigns[i].BpSystolic;//LOINC 8480-6
				ehrVitalsignCur.BpDiastolic=listVitalsigns[i].BpDiastolic;//LOINC 8462-4
				ehrVitalsignCur.DateTaken=listVitalsigns[i].DateTaken;
				if(retval.ContainsKey(ehrVitalsignCur.PatNum)) {
					retval[ehrVitalsignCur.PatNum].Add(ehrVitalsignCur);
				}
				else {
					retval.Add(ehrVitalsignCur.PatNum,new List<EhrCqmVitalsign>() { ehrVitalsignCur });
				}
			}
			return retval;
		}

		///<summary>Get all procedures with ProcDate in the date range and ProcCode in the list of codes that belong to one of the value sets in listValueSetOIDs.</summary>
		private static SerializableDictionary<long,List<EhrCqmProc>> GetProcs(List<long> listPatNums,List<string> listValueSetOIDs,DateTime dateStart,DateTime dateEnd) {
			SerializableDictionary<long,List<EhrCqmProc>> retval=new SerializableDictionary<long,List<EhrCqmProc>>();
			//if no patients, return a new empty dictionary
			if(listPatNums!=null && listPatNums.Count==0) {
				return retval;
			}
			List<EhrCode> listValidProcs=EhrCodes.GetForValueSetOIDs(listValueSetOIDs,false);
			string codeList="";
			for(int i=0;i<listValidProcs.Count;i++) {
				if(i>0) {
					codeList+=",";
				}
				codeList+="'"+listValidProcs[i].CodeValue+"'";
			}
			string command="SELECT procedurelog.ProcNum,procedurelog.PatNum,procedurelog.ProvNum,procedurelog.ProcDate,"
				+"procedurecode.ProcCode,procedurecode.Descript FROM procedurelog "
				+"INNER JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum "
				+"WHERE procedurelog.ProcStatus=2 "
				+"AND procedurelog.ProcDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" ";
			if(listPatNums!=null && listPatNums.Count>0) {
				command+="AND procedurelog.PatNum IN ("+string.Join(",",listPatNums)+")";
			}
			if(codeList!="") {
				command+="AND procedurecode.ProcCode IN("+codeList+") ";
			}
			command+="ORDER BY procedurelog.PatNum,procedurelog.ProcDate DESC";
			DataTable tableAllProcs=Db.GetTable(command);
			if(tableAllProcs.Rows.Count==0) {
				return retval;
			}
			Dictionary<long,EhrCode> dictProcNumEhrCode=new Dictionary<long,EhrCode>();
			for(int i=tableAllProcs.Rows.Count-1;i>-1;i--) {
				for(int j=0;j<listValidProcs.Count;j++) {
					if(tableAllProcs.Rows[i]["ProcCode"].ToString()==listValidProcs[j].CodeValue) {
						dictProcNumEhrCode.Add(PIn.Long(tableAllProcs.Rows[i]["ProcNum"].ToString()),listValidProcs[j]);
						break;
					}
				}
			}
			for(int i=0;i<tableAllProcs.Rows.Count;i++) {
				EhrCqmProc ehrProcCur=new EhrCqmProc();
				ehrProcCur.EhrCqmProcNum=PIn.Long(tableAllProcs.Rows[i]["ProcNum"].ToString());
				ehrProcCur.PatNum=PIn.Long(tableAllProcs.Rows[i]["PatNum"].ToString());
				ehrProcCur.ProvNum=PIn.Long(tableAllProcs.Rows[i]["ProvNum"].ToString());
				ehrProcCur.ProcDate=PIn.Date(tableAllProcs.Rows[i]["ProcDate"].ToString());
				ehrProcCur.ProcCode=tableAllProcs.Rows[i]["ProcCode"].ToString();
				ehrProcCur.Description=tableAllProcs.Rows[i]["Descript"].ToString();
				EhrCode ehrCodeCur=dictProcNumEhrCode[ehrProcCur.EhrCqmProcNum];
				ehrProcCur.CodeSystemName=ehrCodeCur.CodeSystem;
				ehrProcCur.CodeSystemOID=ehrCodeCur.CodeSystemOID;
				ehrProcCur.ValueSetName=ehrCodeCur.ValueSetName;
				ehrProcCur.ValueSetOID=ehrCodeCur.ValueSetOID;
				if(retval.ContainsKey(ehrProcCur.PatNum)) {
					retval[ehrProcCur.PatNum].Add(ehrProcCur);
				}
				else {
					retval.Add(ehrProcCur.PatNum,new List<EhrCqmProc>() { ehrProcCur });
				}
			}
			return retval;
		}

		///<summary>Returns all vaccinepat objects for pneumonia and influenza CQMs.  These are basically just medicationpat objects, so we will use the same object with two optional fields to identify them as vaccinepats instead of medicationpats.  The isGiven bool set to true will return only those with the CompletionStatus=0 (complete).  If isGiven=false, all will be returned and the logic to determine whether it was given or not will take place in calculation.  The only time we want vaccines with a status other than complete is when we are looking for vaccines NotAdministered in the influenza vaccine measure.  These NotAdministered vaccines will be due to an intolerance or allergy and entered as such.</summary>
		private static SerializableDictionary<long,List<EhrCqmMedicationPat>> GetVaccines(List<long> listPatNums,List<string> listValueSetOIDs,DateTime dateStart,DateTime dateEnd,bool isGiven) {
			SerializableDictionary<long,List<EhrCqmMedicationPat>> retval=new SerializableDictionary<long,List<EhrCqmMedicationPat>>();
			//if no patients, return a new empty dictionary
			if(listPatNums!=null && listPatNums.Count==0) {
				return retval;
			}
			string command="SELECT vaccinepat.VaccinePatNum,vaccinepat.PatNum,vaccinepat.DateTimeStart,vaccinepat.DateTimeEnd,vaccinedef.CVXCode,vaccinepat.CompletionStatus "
				+"FROM vaccinepat "
				+"INNER JOIN vaccinedef ON vaccinepat.VaccineDefNum=vaccinedef.VaccineDefNum "
				+"WHERE DATE(vaccinepat.DateTimeStart) BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" ";
			if(isGiven) {
				command+="AND vaccinepat.CompletionStatus=0 ";//CompletionStatus=0 (complete)
			}
			else {
				command+="AND vaccinepat.CompletionStatus IN(0,2) ";//CompletionStatus=0 (complete) or 2 (NotAdministered), we do not care about refused or partially administered for our measures
			}
			if(listPatNums!=null && listPatNums.Count>0) {
				command+="AND vaccinepat.PatNum IN("+string.Join(",",listPatNums)+") ";
			}
			command+="ORDER BY vaccinepat.PatNum,vaccinepat.DateTimeStart DESC";
			DataTable tableAllVaccinePats=Db.GetTable(command);
			if(tableAllVaccinePats.Rows.Count==0) {
				return retval;
			}
			List<EhrCode> listEhrCodes=EhrCodes.GetForValueSetOIDs(listValueSetOIDs,false);//this list will only contain one code, CVX 33 - pneumococcal polysaccharide vaccine, 23 valent
			Dictionary<long,EhrCode> dictVaccinePatNumEhrCode=new Dictionary<long,EhrCode>();
			for(int i=tableAllVaccinePats.Rows.Count-1;i>-1;i--) {
				bool isValidVaccine=false;
				for(int j=0;j<listEhrCodes.Count;j++) {
					if(tableAllVaccinePats.Rows[i]["CVXCode"].ToString()==listEhrCodes[j].CodeValue) {
						dictVaccinePatNumEhrCode.Add(PIn.Long(tableAllVaccinePats.Rows[i]["VaccinePatNum"].ToString()),listEhrCodes[j]);
						isValidVaccine=true;
						break;
					}
				}
				if(!isValidVaccine) {
					tableAllVaccinePats.Rows.RemoveAt(i);
				}
			}
			for(int i=0;i<tableAllVaccinePats.Rows.Count;i++) {
				EhrCqmMedicationPat ehrVacPatCur=new EhrCqmMedicationPat();
				ehrVacPatCur.EhrCqmMedicationPatNum=0;
				ehrVacPatCur.EhrCqmVaccinePatNum=PIn.Long(tableAllVaccinePats.Rows[i]["VaccinePatNum"].ToString());
				ehrVacPatCur.PatNum=PIn.Long(tableAllVaccinePats.Rows[i]["PatNum"].ToString());
				ehrVacPatCur.CVXCode=tableAllVaccinePats.Rows[i]["CVXCode"].ToString();
				ehrVacPatCur.CompletionStatus=(VaccineCompletionStatus)PIn.Int(tableAllVaccinePats.Rows[i]["CompletionStatus"].ToString());
				ehrVacPatCur.DateStart=PIn.DateT(tableAllVaccinePats.Rows[i]["DateTimeStart"].ToString());
				ehrVacPatCur.DateStop=PIn.DateT(tableAllVaccinePats.Rows[i]["DateTimeEnd"].ToString());
				EhrCode ehrCodeCur=dictVaccinePatNumEhrCode[ehrVacPatCur.EhrCqmVaccinePatNum];
				ehrVacPatCur.CodeSystemName=ehrCodeCur.CodeSystem;
				ehrVacPatCur.CodeSystemOID=ehrCodeCur.CodeSystemOID;
				ehrVacPatCur.ValueSetName=ehrCodeCur.ValueSetName;
				ehrVacPatCur.ValueSetOID=ehrCodeCur.ValueSetOID;
				string descript=ehrCodeCur.Description;
				Cvx cvxCur=Cvxs.GetByCode(ehrVacPatCur.CVXCode);
				if(cvxCur!=null) {
					descript=cvxCur.Description;
				}
				ehrVacPatCur.Description=descript;//description either from cvx table or, if not in table, default to EhrCode object description
				if(retval.ContainsKey(ehrVacPatCur.PatNum)) {
					retval[ehrVacPatCur.PatNum].Add(ehrVacPatCur);
				}
				else {
					retval.Add(ehrVacPatCur.PatNum,new List<EhrCqmMedicationPat>() { ehrVacPatCur });
				}
			}
			return retval;
		}

		///<summary>Using the data in alldata, determine if the patients in alldata.ListEhrPats are in the 'Numerator', 'Exclusion', or 'Exception' category for this measure and enter an explanation if applicable.  All of the patients in ListEhrPats are the initial patient population (almost always equal to the Denominator).</summary>
		private static void ClassifyPatients(QualityMeasure alldata,QualityType2014 qtype,DateTime dateStart,DateTime dateEnd) {
			switch(qtype) {
				#region MedicationsEntered
				case QualityType2014.MedicationsEntered:
					//THIS IS ENCOUNTER BASED, NOT PATIENT BASED
					//alldata.ListEhrPats: All unique patients with necessary reporting data.  This is the initial patient population (Denominator).
					//alldata.DictPatNumListEncounters:  PatNums linked to a list of all encounters from the eligble value sets for patients 18 or over at the start of the measurement period.
					//alldata.DictPatNumListMeasureEvents: All Current Meds Documented 'procedures' that occurred during the measurement period
					//alldata.DictPatNumListNotPerfs: All Current Meds Documented events not performed with valid reasons that occurred during the measurement period
					//No exclusions for this measure
					//Strategy: For each patient in ListEhrPats, loop through the encounters in dictionary DictPatNumListEncounters; key=PatNum, value=List<EhrCqmEncounter>
					//For each encounter, loop through the measure events in DictPatNumListMeasureEvents and try to locate a meds documented 'proc' on the same date.
					//If one exists for that encounter, mark the encounter as being in the numerator
					//If no procedure exists, look for a not performed item for Exception
					//Otherwise unclassified
					bool isClassified=false;//used to indicate that the encounter has been included in the numerator, so do not try to apply exception
					foreach(KeyValuePair<long,List<EhrCqmEncounter>> kvpair in alldata.DictPatNumListEncounters) {
						List<EhrCqmMeasEvent> listMedsDocProcsCur=new List<EhrCqmMeasEvent>();
						if(alldata.DictPatNumListMeasureEvents.ContainsKey(kvpair.Key)) {
							listMedsDocProcsCur=alldata.DictPatNumListMeasureEvents[kvpair.Key];
						}
						List<EhrCqmNotPerf> listNotPerfCur=new List<EhrCqmNotPerf>();
						if(alldata.DictPatNumListNotPerfs.ContainsKey(kvpair.Key)) {
							listNotPerfCur=alldata.DictPatNumListNotPerfs[kvpair.Key];
						}
						//first try to apply the numerator criteria to the encounters, if there is no ehrmeasureevent on the same day, try to apply the exception criteria
						for(int i=0;i<kvpair.Value.Count;i++) {//for every encounter for this patient
							isClassified=false;
							for(int j=0;j<listMedsDocProcsCur.Count;j++) {//this might be an empty list if no meds documented procs for this patient
								if(kvpair.Value[i].DateEncounter.Date==listMedsDocProcsCur[j].DateTEvent.Date) {//if meds documented proc on the same day as the encounter, then numerator
									isClassified=true;
									kvpair.Value[i].IsNumerator=true;
									kvpair.Value[i].Explanation="There is an encounter on "+kvpair.Value[i].DateEncounter.ToShortDateString()+" with a current medications documented procedure on the same date.";
									break;
								}
							}
							if(isClassified) {//don't try to apply exception criteria if already in the numerator
								continue;
							}
							for(int j=0;j<listNotPerfCur.Count;j++) {//if encounter is not in the numerator, try to apply the exception criteria
								if(kvpair.Value[i].DateEncounter.Date==listNotPerfCur[j].DateEntry.Date) {
									kvpair.Value[i].IsException=true;
									kvpair.Value[i].Explanation="There is an encounter on "+kvpair.Value[i].DateEncounter.ToShortDateString()+" with a not performed item with valid reason on the same date.";
									isClassified=true;
									break;
								}
							}
							if(isClassified) {//it is an exception encounter, move to next encounter
								continue;
							}
							//otherwise the encounter is NotMet
							kvpair.Value[i].Explanation="There is an encounter on "+kvpair.Value[i].DateEncounter.ToShortDateString()+" but no current medications documented procedure or not performed item on the same date.";
						}
					}
					break;
				#endregion
				#region WeightAdultAndOver65
				case QualityType2014.WeightAdult:
				case QualityType2014.WeightOver65:
					//Strategy: All patients in alldata.ListEhrPats are the initial patient population for this measure
					//Denominator - Inital Patient Population
					//Exclusions - Pregnant during any of the measurement period
					//Find the most recent vitalsign exam date such that there is a valid encounter within the 6 months after the exam
					//If there is more than one encounter within that 6 months, one of them must meet the numerator criteria for the patient to be in the numerator
					//If that most recent exam found the patient with BMI >= 23 kg/m2 and < 30 kg/m2 for Over65, >= 18.5 and < 25 for Adult, 'Numerator'
					//If the most recent exam found the patient with BMI < 23 kg/m2 or >= 30 kg/m2 for Over65, < 18.5 or >= 25 for Adult, check for Intervention or Medication Order
					//The intervention/medication order must be within 6 months of one of the encounters, which are within 6 months of the exam
					//If order exists for any encounter, 'Numerator'
					//If no intervention/medication order for any of the encounters for the exam, not classified
					for(int i=0;i<alldata.ListEhrPats.Count;i++) {
						long patNumCur=alldata.ListEhrPats[i].EhrCqmPat.PatNum;
						bool isCategorized=false;
						//first apply pregnancy exclusion
						List<EhrCqmProblem> listProbsCur=new List<EhrCqmProblem>();
						if(alldata.DictPatNumListProblems.ContainsKey(patNumCur)) {
							listProbsCur=alldata.DictPatNumListProblems[patNumCur];
						}
						for(int j=0;j<listProbsCur.Count;j++) {
							if(listProbsCur[j].ValueSetOID=="2.16.840.1.113883.3.600.1.1623") {//Pregnancy Dx Grouping Value Set
								alldata.ListEhrPats[i].IsExclusion=true;
								alldata.ListEhrPats[i].Explanation="The patient had a pregnancy diagnosis that started on "+listProbsCur[j].DateStart.ToShortDateString()+" that is either still active or ended after the start of the measurement period.";
								isCategorized=true;
								break;
							}
						}
						if(isCategorized) {
							continue;
						}
						//get vital sign exams that took place in the measurement period for the patient.  Ordered by DateTaken DESC, so index 0 will hold the most recent exam
						List<EhrCqmVitalsign> listVitalsignsCur=new List<EhrCqmVitalsign>();
						if(alldata.DictPatNumListVitalsigns.ContainsKey(patNumCur)) {
							listVitalsignsCur=alldata.DictPatNumListVitalsigns[patNumCur];
						}
						if(listVitalsignsCur.Count==0) {//no vitalsign exams within 6 months of start of measurement period, but encounters exist or they would not be in the IPP
							alldata.ListEhrPats[i].Explanation="Valid encounters exist, but there are no BMI vital sign exams within 6 months of the measurement period.";
							continue;
						}
						//get eligible enounters that took place in the measurement period for the patient.  Ordered by DateEncounter DESC, so index 0 will hold the most recent.
						List<EhrCqmEncounter> listEncsCur=new List<EhrCqmEncounter>();
						if(alldata.DictPatNumListEncounters.ContainsKey(patNumCur)) {
							listEncsCur=alldata.DictPatNumListEncounters[patNumCur];
						}
						//Find the most recent exam date such that there is an eligible encounter on that date or within the 6 months after the exam date
						DateTime dateMostRecentExam=DateTime.MinValue;
						int indexMostRecentExam=-1;
						for(int j=0;j<listVitalsignsCur.Count;j++) {
							if(dateMostRecentExam.Date>listVitalsignsCur[j].DateTaken.Date) {//most recent exam date already set and set to a date more recent than current, continue
								continue;
							}
							for(int k=0;k<listEncsCur.Count;k++) {
								if(listVitalsignsCur[j].DateTaken.Date<=listEncsCur[k].DateEncounter.Date
									&& listVitalsignsCur[j].DateTaken.Date>=listEncsCur[k].DateEncounter.AddMonths(-6).Date
									&& listVitalsignsCur[j].DateTaken.Date>=dateMostRecentExam.Date)
								{
									dateMostRecentExam=listVitalsignsCur[j].DateTaken;
									indexMostRecentExam=j;
									break;
								}
							}
						}
						//If there are no exams that occurred within 6 months of an eligible encounter, not classified
						if(indexMostRecentExam==-1) {
							alldata.ListEhrPats[i].Explanation="Valid encounters exist and BMI vital sign exams exist, but no BMI exam date is within 6 months of a valid encounter in the measurement period.";
							continue;
						}
						//If WeightOver65 measure AND the most recent BMI was in the allowed range of >=23 and <30 kg/m2, then no intervention required, patient in 'Numerator'
						if(qtype==QualityType2014.WeightOver65 && listVitalsignsCur[indexMostRecentExam].BMI>=23m && listVitalsignsCur[indexMostRecentExam].BMI<30m) {//'m' converts to decimal
							alldata.ListEhrPats[i].IsNumerator=true;
							alldata.ListEhrPats[i].Explanation="BMI in normal range.  Most recent BMI exam date: "+listVitalsignsCur[indexMostRecentExam].DateTaken.ToShortDateString()+".  BMI result: "+listVitalsignsCur[indexMostRecentExam].BMI.ToString();
							continue;
						}
						//If WeightAdult measure AND the most recent BMI was in the allowed range of >=23 and <30 kg/m2, then no intervention required, patient in 'Numerator'
						if(qtype==QualityType2014.WeightAdult && listVitalsignsCur[indexMostRecentExam].BMI>=18.5m && listVitalsignsCur[indexMostRecentExam].BMI<25m) {//'m' converts to decimal
							alldata.ListEhrPats[i].IsNumerator=true;
							alldata.ListEhrPats[i].Explanation="BMI in normal range.  Most recent BMI exam date: "+listVitalsignsCur[indexMostRecentExam].DateTaken.ToShortDateString()+".  BMI result: "+listVitalsignsCur[indexMostRecentExam].BMI.ToString();
							continue;
						}
						//BMI must be out of range, for each encounter of which this exam is within the previous 6 months, look for an intervention/medication order that took place within the 6 months prior to the encounter
						//If an encounter and intervention/medication order exist for this encounter, 'Numerator'
						List<EhrCqmIntervention> listInterventionsCur=new List<EhrCqmIntervention>();
						if(alldata.DictPatNumListInterventions.ContainsKey(patNumCur)) {
							listInterventionsCur=alldata.DictPatNumListInterventions[patNumCur];
						}
						List<EhrCqmMedicationPat> listMedPatsCur=new List<EhrCqmMedicationPat>();
						if(alldata.DictPatNumListMedPats.ContainsKey(patNumCur)) {
							listMedPatsCur=alldata.DictPatNumListMedPats[patNumCur];
						}
						for(int j=0;j<listEncsCur.Count;j++) {
							//if encounter is before exam or more than 6 months after the exam, move to next encounter
							if(listEncsCur[j].DateEncounter.Date<listVitalsignsCur[indexMostRecentExam].DateTaken.Date
								|| listEncsCur[j].DateEncounter.Date>listVitalsignsCur[indexMostRecentExam].DateTaken.AddMonths(6).Date)
							{
								continue;
							}
							for(int k=0;k<listInterventionsCur.Count;k++) {
								//if intervention order is within 6 months of the encounter, classify as 'Numerator'
								if(listInterventionsCur[k].DateEntry.Date<=listEncsCur[j].DateEncounter.Date
									&& listInterventionsCur[k].DateEntry.Date>=listEncsCur[j].DateEncounter.AddMonths(-6).Date)
								{
									//encounter within 6 months of the most recent exam and intervention within 6 months of encounter, 'Numerator'
									alldata.ListEhrPats[i].IsNumerator=true;
									alldata.ListEhrPats[i].Explanation="Most recent exam on "+listVitalsignsCur[indexMostRecentExam].DateTaken.ToShortDateString()+" with encounter on "
										+listEncsCur[j].DateEncounter.ToShortDateString()+" resulted in a BMI of "+listVitalsignsCur[indexMostRecentExam].BMI.ToString()+" "
										+"and intervention on "+listInterventionsCur[k].DateEntry.ToShortDateString()+".";
									isCategorized=true;
									break;
								}
							}
							if(isCategorized) {
								break;
							}
							for(int k=0;k<listMedPatsCur.Count;k++) {
								//if medication order is within 6 months of the encounter, classify as 'Numerator'
								if(listMedPatsCur[k].DateStart.Date<=listEncsCur[j].DateEncounter.Date
									&& listMedPatsCur[k].DateStart.Date>=listEncsCur[j].DateEncounter.AddMonths(-6).Date)
								{
									alldata.ListEhrPats[i].IsNumerator=true;
									alldata.ListEhrPats[i].Explanation="Most recent exam on "+listVitalsignsCur[indexMostRecentExam].DateTaken.ToShortDateString()+" with encounter on "
										+listEncsCur[j].DateEncounter.ToShortDateString()+" resulted in a BMI of "+listVitalsignsCur[indexMostRecentExam].BMI.ToString()+" "
										+"and medication order on "+listMedPatsCur[k].DateStart.ToShortDateString()+".";
									isCategorized=true;
									break;
								}
							}
							if(isCategorized) {
								break;
							}
						}
						if(isCategorized) {
							continue;
						}
						//If we get here, the most recent BMI exam that had eligible encounters on the exam date or within the 6 months after that date resulted in a BMI outside of normal range
						//but there were no intervention/medication orders entered
						alldata.ListEhrPats[i].Explanation="Most recent exam on "+listVitalsignsCur[indexMostRecentExam].DateTaken.ToShortDateString()+" with BMI of "
							+listVitalsignsCur[indexMostRecentExam].BMI.ToString()+" had valid encounters within 6 months of the exam but no valid intervention or medication order.";
					}
					break;
				#endregion
				#region CariesPrevent
				case QualityType2014.CariesPrevent:
				case QualityType2014.CariesPrevent_1:
				case QualityType2014.CariesPrevent_2:
				case QualityType2014.CariesPrevent_3:
					//Strategy: alldata.ListEhrPats will be the initial patient population, already restricted by appropriate age for all, _1, _2 and _3 stratification
					//No Exclusions, denominator is initial patient population
					//alldata.DictPatNumListProcs will hold the eligible procs in the measurement period for the patients in the initial patient population
					//if Dict contains PatNum, 'Numerator'
					for(int i=0;i<alldata.ListEhrPats.Count;i++) {
						long patNumCur=alldata.ListEhrPats[i].EhrCqmPat.PatNum;
						if(alldata.DictPatNumListProcs.ContainsKey(patNumCur)) {
							alldata.ListEhrPats[i].IsNumerator=true;
							alldata.ListEhrPats[i].Explanation="This patient had an eligible encounter in the measurement period and a flouride varnish application procedure with code "
								+alldata.DictPatNumListProcs[patNumCur][0].ProcCode+" on "+alldata.DictPatNumListProcs[patNumCur][0].ProcDate.ToShortDateString()+".";
							continue;
						}
						alldata.ListEhrPats[i].Explanation="This patient had an eligible encounter in the measurement period, but did not have a flouride varnish application procedure with a valid code in the measurement period.";
					}
					break;
				#endregion
				#region ChildCaries
				case QualityType2014.ChildCaries:
					//This measure is misleading, A lower score indicates better quality, so a high percentage means they have a lot of kids with cavities or dental decay
					//Strategy: alldata.ListEhrPats will be the initial patient population, already restricted by age >=0 and < 20 at start of measurement period
					//No Exclusions, denominator is initial patient population
					//alldata.DictPatNumListProblems will hold all eligible problems for dental caries with a start date before the end of the measurement period and either no end date or an end date after the start of the measurement period
					//if patient is in ListEhrPats and Dict contains PatNum, 'Numerator'
					for(int i=0;i<alldata.ListEhrPats.Count;i++) {
						long patNumCur=alldata.ListEhrPats[i].EhrCqmPat.PatNum;
						if(alldata.DictPatNumListProblems.ContainsKey(patNumCur)) {
							alldata.ListEhrPats[i].IsNumerator=true;
							alldata.ListEhrPats[i].Explanation="This patient had an eligible encounter in the measurement period and had an active diagnosis of caries with code "
								+alldata.DictPatNumListProblems[patNumCur][0].CodeValue+" that started on "+alldata.DictPatNumListProblems[patNumCur][0].DateStart.ToShortDateString()+".";
							continue;
						}
						alldata.ListEhrPats[i].Explanation="This patient had an eligible encounter in the measurement period, but did not have an active diagnosis of caries with a valid code in the measurement period.";
					}
					break;
				#endregion
				#region Pneumonia
				case QualityType2014.Pneumonia:
					//Strategy: alldata.ListEhrPats is IPP and denominator, non exclusions
					//Numerator: if medicationpat, procedure, or problem exists, numerator
					//Otherwise: not categorized
					for(int i=0;i<alldata.ListEhrPats.Count;i++) {
						long patNumCur=alldata.ListEhrPats[i].EhrCqmPat.PatNum;
						bool isCategorized=false;
						//first see if medicationpat exists
						List<EhrCqmMedicationPat> listMedPatsCur=new List<EhrCqmMedicationPat>();
						if(alldata.DictPatNumListMedPats.ContainsKey(patNumCur)) {
							listMedPatsCur=alldata.DictPatNumListMedPats[patNumCur];
						}
						for(int j=0;j<listMedPatsCur.Count;j++) {
							if(listMedPatsCur[j].ValueSetOID=="2.16.840.1.113883.3.464.1003.110.12.1027") {//Pneumococcal Vaccine Grouping Value Set
								alldata.ListEhrPats[i].IsNumerator=true;
								alldata.ListEhrPats[i].Explanation="The patient had a Pneumococcal medication administered on "+listMedPatsCur[j].DateStart.ToShortDateString()+".";
								isCategorized=true;
								break;
							}
						}
						if(isCategorized) {
							continue;
						}
						List<EhrCqmProc> listProcsCur=new List<EhrCqmProc>();
						if(alldata.DictPatNumListProcs.ContainsKey(patNumCur)) {
							listProcsCur=alldata.DictPatNumListProcs[patNumCur];
						}
						for(int j=0;j<listProcsCur.Count;j++) {
							if(listProcsCur[j].ValueSetOID=="2.16.840.1.113883.3.464.1003.110.12.1034") {//Pneumococcal Vaccine Administered Grouping Value Set
								alldata.ListEhrPats[i].IsNumerator=true;
								alldata.ListEhrPats[i].Explanation="The patient had a Pneumococcal administered procedure on "+listProcsCur[j].ProcDate.ToShortDateString()+".";
								isCategorized=true;
								break;
							}
						}
						if(isCategorized) {
							continue;
						}
						List<EhrCqmProblem> listProbsCur=new List<EhrCqmProblem>();
						if(alldata.DictPatNumListProblems.ContainsKey(patNumCur)) {
							listProbsCur=alldata.DictPatNumListProblems[patNumCur];
						}
						for(int j=0;j<listProbsCur.Count;j++) {
							if(listProbsCur[j].ValueSetOID=="2.16.840.1.113883.3.464.1003.110.12.1028") {//History of Pneumococcal Vaccine Grouping Value Set
								alldata.ListEhrPats[i].IsNumerator=true;
								alldata.ListEhrPats[i].Explanation="The patient has a history of Pneumococcal vaccination on "+listProbsCur[j].DateStart.ToShortDateString()+".";
								isCategorized=true;
								break;
							}
						}
						if(isCategorized) {
							continue;
						}
						alldata.ListEhrPats[i].Explanation="The patient had eligible encounters in the date range but no Pneumococcal vaccine was administered and no history of having had the vaccine was recorded.";
					}
					break;
				#endregion
				#region TobaccoCessation
				case QualityType2014.TobaccoCessation:
					//alldata.ListEhrPats: All unique patients with necessary reporting data.  This is the initial patient population (Denominator).
					//alldata.ListEncounters:  All encounters from the eligble value sets for patients 18 or over at the start of the measurement period.
					//alldata.ListMeasureEvents: All tobacco use assessment ehrmeasureevents with recorded status that occurred within 24 months of the end of the measurement period
					//alldata.ListInterventions: All eligible interventions performed within 24 months of the end of the measurement period
					//alldata.ListMedPats: All eligible medications, active or ordered, that started within 24 months of the end of the measurement period
					//alldata.ListNotPerfs: All tobacco assessment events not performed with valid reasons that occurred during the measurement period
					//alldata.ListProblems: All eligible problems that were active during any of the measurement period.
					for(int i=0;i<alldata.ListEhrPats.Count;i++) {
						long patNumCur=alldata.ListEhrPats[i].EhrCqmPat.PatNum;
						//No exclusions for this measure
						//Strategy: Find the most recent tobacco assessment for the patient.
						//If Non-User, this patient is in the Numerator.
						//If User, check for intervention, medication active or order, if one exists, then Numerator.
						//If User and no intervention/med, or no assessment at all, then check notperformed and problems for possible Exception.
						//Finally, if none of the above, then only fill the Explanation column with the appropriate text.
						#region Get most recent assessment date and value set OID for patient
						DateTime mostRecentAssessDate=DateTime.MinValue;
						string mostRecentAssessValueSetOID="";
						List<EhrCqmMeasEvent> listMeasEventsCur=new List<EhrCqmMeasEvent>();
						if(alldata.DictPatNumListMeasureEvents.ContainsKey(patNumCur)) {
							listMeasEventsCur=alldata.DictPatNumListMeasureEvents[patNumCur];
						}
						for(int j=0;j<listMeasEventsCur.Count;j++) {
							if(listMeasEventsCur[j].DateTEvent>mostRecentAssessDate) {
								mostRecentAssessDate=listMeasEventsCur[j].DateTEvent;
								mostRecentAssessValueSetOID=listMeasEventsCur[j].ValueSetOID;
							}
						}
						#endregion
						//if most recently (all assessments in our list are in the last 24 months prior to the measurement period end date) assessed Non-User, then Numerator
						#region Most recently assessed Non-User
						if(mostRecentAssessDate>DateTime.MinValue && mostRecentAssessValueSetOID=="2.16.840.1.113883.3.526.3.1189") {//Non-User
							alldata.ListEhrPats[i].IsNumerator=true;
							alldata.ListEhrPats[i].Explanation="Patient categorized as Non-User on "+mostRecentAssessDate.Date.ToShortDateString();
							continue;
						}
						#endregion
						//if most recently assessed User, check for intervention or medication active/order
						#region Most recently assessed User
						if(mostRecentAssessDate>DateTime.MinValue && mostRecentAssessValueSetOID=="2.16.840.1.113883.3.526.3.1170") {//User
							//check for intervention.  If in the list, it is already guaranteed to be valid and in the date range and order by PatNum and DateEntry, so first one found will be the most recent for the patient
							List<EhrCqmIntervention> listIntervensCur=new List<EhrCqmIntervention>();
							if(alldata.DictPatNumListInterventions.ContainsKey(patNumCur)) {
								listIntervensCur=alldata.DictPatNumListInterventions[patNumCur];
							}
							if(listIntervensCur.Count>0) {
								alldata.ListEhrPats[i].IsNumerator=true;
								alldata.ListEhrPats[i].Explanation="Patient categorized as User with an intervention on "+listIntervensCur[0].DateEntry.ToShortDateString();
								continue;
							}
							//check for medication.  If there is one in the list, it is guaranteed to be valid for tobacco cessation and in the date range and ordered by PatNum and DateStart, so first found is most recent
							List<EhrCqmMedicationPat> listMedPatsCur=new List<EhrCqmMedicationPat>();
							if(alldata.DictPatNumListMedPats.ContainsKey(patNumCur)) {
								listMedPatsCur=alldata.DictPatNumListMedPats[patNumCur];
							}
							if(listMedPatsCur.Count>0) {
								alldata.ListEhrPats[i].IsNumerator=true;
								string explain="Patient categorized as User with medication ";
								string activeOrOrder="active";
								if(listMedPatsCur[0].PatNote!="") {//PatNote means Medication Order, otherwise Medication Active
									activeOrOrder="order";
								}
								alldata.ListEhrPats[i].Explanation=explain+activeOrOrder+" with start date "+listMedPatsCur[0].DateStart.ToShortDateString();
								continue;
							}
						}
						#endregion
						//if we get here, there is either no valid assessment date in the date range or the patient was most recently categorized as User with no intervention or medication
						//check for valid NotPerformed item, for exception
						#region Check for not performed
						//alldata.ListNotPerf is ordered by PatNum, DateEntry DESC so first one found is the most recent for the patient
						List<EhrCqmNotPerf> listNotPerfsCur=new List<EhrCqmNotPerf>();
						if(alldata.DictPatNumListNotPerfs.ContainsKey(patNumCur)) {
							listNotPerfsCur=alldata.DictPatNumListNotPerfs[patNumCur];
						}
						if(listNotPerfsCur.Count>0) {
							alldata.ListEhrPats[i].IsException=true;
							alldata.ListEhrPats[i].Explanation="Assessment not done for valid medical reason on "+listNotPerfsCur[0].DateEntry.ToShortDateString();
							continue;
						}
						#endregion
						//last, check for limited life expectancy, for exception
						#region Check for active diagnosis of limited life expectancy
						List<EhrCqmProblem> listProbsCur=new List<EhrCqmProblem>();
						if(alldata.DictPatNumListProblems.ContainsKey(patNumCur)) {
							listProbsCur=alldata.DictPatNumListProblems[patNumCur];
						}
						if(listProbsCur.Count>0) {
							alldata.ListEhrPats[i].IsException=true;
							string explain="Assessment not done due to an active limited life expectancy diagnosis";
							if(listProbsCur[0].DateStart.Year>1880) {
								explain+=" with start date "+listProbsCur[0].DateStart.ToShortDateString();
							}
							alldata.ListEhrPats[i].Explanation=explain;
							continue;
						}
						#endregion
						//still not categorized, put note in explanation, could be due to no assessment in date range or categorized User with no intervention/medication
						#region Not met explanation
						if(mostRecentAssessDate==DateTime.MinValue) {
							alldata.ListEhrPats[i].Explanation="No tobacco use assessment entered";
						}
						else if(mostRecentAssessValueSetOID=="2.16.840.1.113883.3.526.3.1170") {//User
							alldata.ListEhrPats[i].Explanation="Patient categorized as User on "+mostRecentAssessDate.ToShortDateString()+" without an intervention";
						}
						#endregion
					}
					break;
				#endregion
				#region Influenza
				case QualityType2014.Influenza:
					//Strategy: alldata.ListEhrPats will hold all initial patient population
					//Denominator: alldata.ListEhrPats where IsDenominator=true
					//No Exclusions
					//Numerator: if proc, med, or previous receipt during any eligible encounter or proc in the flu season, then numerator
					//Exceptions: if not Numerator, check for vaccine declined or not performed item with valid reason during any eligible encounter or proc in the flu season
					//OR check for allergy or intolerance before or during the eligible encounter or proc
					for(int i=0;i<alldata.ListEhrPats.Count;i++) {
						if(!alldata.ListEhrPats[i].IsDenominator) {//if not in denominator, they will not show in grid of FormEhrQualityMeasureEdit2014, skip this patient, only in IPP
							continue;
						}
						long patNumCur=alldata.ListEhrPats[i].EhrCqmPat.PatNum;
						bool isCategorized=false;
						//The list of encounters will contain the one-of and two-of encounters during the measurement period that qualify patient for IPP
						//PLUS the Encounter, Performed: Encounter-Influenza encs that took place <= 92 days before start of measurement period or <= 91 days after start of measurement period.
						//Each of these influenza encs will be the "Occurrence A of..." that we will be using to try to match the numerator procs/meds/communications.
						List<EhrCqmEncounter> listAllEncsCur=new List<EhrCqmEncounter>();
						if(alldata.DictPatNumListEncounters.ContainsKey(patNumCur)) {
							listAllEncsCur=alldata.DictPatNumListEncounters[patNumCur];
						}
						//The list of procs will contain procs that qualify pat for the IPP, plus procs that qualify pat for the denominator, plus any procs that may qualify pat for the numerator
						List<EhrCqmProc> listAllProcsCur=new List<EhrCqmProc>();
						if(alldata.DictPatNumListProcs.ContainsKey(patNumCur)) {
							listAllProcsCur=alldata.DictPatNumListProcs[patNumCur];
						}
						//The list of medicationpats will contain only meds from the value set 2.16.840.1.113883.3.526.3.1254 - Influenza Vaccine Grouping Value Set
						//But will be for any date in date range 0001-01-01 through 91 days after measurement period start date
						//Those with CompletionStatus=Complete, and took place <= 92 days before start through <= 91 days after start date range and during "Occurrence A of.." event will be used for numerator
						//Those with CompletionStatus=NotAdministered will be considered allergy or intolerance and used in exceptions
						List<EhrCqmMedicationPat> listAllMedPatsCur=new List<EhrCqmMedicationPat>();
						if(alldata.DictPatNumListMedPats.ContainsKey(patNumCur)) {
							listAllMedPatsCur=alldata.DictPatNumListMedPats[patNumCur];
						}
						//The list of problems will contain the communication of previous receipt items for use in numerator that occurred during "Occurrence A of..."
						//AND the communication of vaccine declined for use in exceptions that occurred during "Occurrence A of..."
						//AND the allergy/intolerance to eggs/vaccine diagnoses that occurred any time before or during "Occurrence A of..."
						List<EhrCqmProblem> listAllProbsCur=new List<EhrCqmProblem>();
						if(alldata.DictPatNumListProblems.ContainsKey(patNumCur)) {
							listAllProbsCur=alldata.DictPatNumListProblems[patNumCur];
						}
						//create list of "Occurrence A of..." encounter/procedure dates
						List<DateTime> listOccurrenceADates=new List<DateTime>();
						for(int j=0;j<listAllEncsCur.Count;j++) {
							if(listAllEncsCur[j].ValueSetOID!="2.16.840.1.113883.3.526.3.1252") {//Encounter-Influenza Grouping Value Set
								continue;
							}
							if(listAllEncsCur[j].DateEncounter.Date>=dateStart.AddDays(-92).Date && listAllEncsCur[j].DateEncounter.Date<=dateStart.AddDays(91).Date) {
								//encounter is in value set and date range, one of the "Occurrence A of..." encounters
								listOccurrenceADates.Add(listAllEncsCur[j].DateEncounter);
							}
						}
						for(int j=0;j<listAllProcsCur.Count;j++) {
							if(listAllProcsCur[j].ValueSetOID!="2.16.840.1.113883.3.526.3.1083"//Hemodialysis Grouping Value Set
								&& listAllProcsCur[j].ValueSetOID!="2.16.840.1.113883.3.526.3.1084")//Peritoneal Dialysis Grouping Value Set
							{
								continue;
							}
							if(listAllProcsCur[j].ProcDate.Date>=dateStart.AddDays(-92).Date && listAllProcsCur[j].ProcDate<=dateStart.AddDays(91).Date) {
								//procedure is in one of the eligible value sets and in flu season date range, one of the "Occurrence A of..." procedures
								if(!listOccurrenceADates.Contains(listAllProcsCur[j].ProcDate)) {
									listOccurrenceADates.Add(listAllProcsCur[j].ProcDate);
								}
							}
						}
						//apply numerator procs
						for(int j=0;j<listAllProcsCur.Count;j++) {
							if(listAllProcsCur[j].ValueSetOID!="2.16.840.1.113883.3.526.3.402") {//Influenza Vaccination Grouping Value Set
								continue;
							}
							for(int k=0;k<listOccurrenceADates.Count;k++) {
								if(listAllProcsCur[j].ProcDate.Date==listOccurrenceADates[k].Date) {
									alldata.ListEhrPats[i].IsNumerator=true;
									alldata.ListEhrPats[i].Explanation="The patient had an eligible procedure or encounter on "+listOccurrenceADates[k].ToShortDateString()+" and had an influenza vaccination procedure on that date.";
									isCategorized=true;
									break;
								}
							}
							if(isCategorized) {
								break;
							}
						}
						if(isCategorized) {
							continue;
						}
						//apply numerator meds
						for(int j=0;j<listAllMedPatsCur.Count;j++) {//already only consists of meds from value set 2.16.840.1.113883.3.526.3.1254 - Influenza Vaccine Grouping Value Set
							if(listAllMedPatsCur[j].CompletionStatus==VaccineCompletionStatus.NotAdministered) {//status other than complete, skip
								continue;
							}
							for(int k=0;k<listOccurrenceADates.Count;k++) {
								if(listOccurrenceADates[k].Date==listAllMedPatsCur[j].DateStart.Date) {
									alldata.ListEhrPats[i].IsNumerator=true;
									alldata.ListEhrPats[i].Explanation="The patient had an eligible procedure or encounter on "+listOccurrenceADates[k].ToShortDateString()+" and had an influenza vaccine medication administered on that date.";
									isCategorized=true;
									break;
								}
							}
							if(isCategorized) {
								break;
							}
						}
						if(isCategorized) {
							continue;
						}
						//apply communication of previous receipt of vaccine 'problems'
						for(int j=0;j<listAllProbsCur.Count;j++) {
							if(listAllProbsCur[j].ValueSetOID!="2.16.840.1.113883.3.526.3.1185") {//Previous Receipt of Influenza Vaccine Grouping Value Set
								continue;
							}
							for(int k=0;k<listOccurrenceADates.Count;k++) {
								if(listOccurrenceADates[k].Date==listAllProbsCur[j].DateStart.Date) {
									alldata.ListEhrPats[i].IsNumerator=true;
									alldata.ListEhrPats[i].Explanation="The patient had an eligible procedure or encounter on "+listOccurrenceADates[k].ToShortDateString()+" and communicated having previous receipt of the influenza vaccine to the provider on that date.";
									isCategorized=true;
									break;
								}
							}
							if(isCategorized) {
								break;
							}
						}
						if(isCategorized) {
							continue;
						}
						//Not in numerator, try to apply exceptions
						for(int j=0;j<listAllProbsCur.Count;j++) {
							if(listAllProbsCur[j].ValueSetOID!="2.16.840.1.113883.3.526.3.1255") {//Influenza Vaccination Declined Grouping Value Set
								continue;
							}
							for(int k=0;k<listOccurrenceADates.Count;k++) {
								if(listOccurrenceADates[k].Date==listAllProbsCur[j].DateStart.Date) {
									alldata.ListEhrPats[i].IsException=true;
									alldata.ListEhrPats[i].Explanation="The patient had an eligible procedure or encounter on "+listOccurrenceADates[k].ToShortDateString()+" and declined the influenza vaccination on that date.";
									isCategorized=true;
									break;
								}
							}
							if(isCategorized) {
								break;
							}
						}
						if(isCategorized) {
							continue;
						}
						//The list of not perfs will contain the procs/meds not performed for valid med/pat/system reason in the <= 92 days before start through <= 91 days after start date range
						List<EhrCqmNotPerf> listNotPerfsCur=new List<EhrCqmNotPerf>();
						if(alldata.DictPatNumListNotPerfs.ContainsKey(patNumCur)) {
							listNotPerfsCur=alldata.DictPatNumListNotPerfs[patNumCur];
						}
						for(int j=0;j<listNotPerfsCur.Count;j++) {
							for(int k=0;k<listOccurrenceADates.Count;k++) {
								if(listNotPerfsCur[j].DateEntry.Date==listOccurrenceADates[k].Date) {
									alldata.ListEhrPats[i].IsException=true;
									alldata.ListEhrPats[i].Explanation="The patient had an eligible procedure or encounter on "+listOccurrenceADates[k].ToShortDateString()+" and the influenza vaccine procedure or medication was not performed for valid reason on that date.";
									isCategorized=true;
									break;
								}
							}
							if(isCategorized) {
								break;
							}
						}
						if(isCategorized) {
							continue;
						}
						//check for allergies or intolerances in problem list or medpat list that happened any time before "Occurrence A of..." or during that occurrence
						//problem list has the problems for these three value sets that occurred from '0001-01-01' through dateStart + 91 days
						for(int j=0;j<listAllProbsCur.Count;j++) {
							if(listAllProbsCur[j].ValueSetOID!="2.16.840.1.113883.3.526.3.1253"//Allergy to eggs Grouping Value Set
								&& listAllProbsCur[j].ValueSetOID!="2.16.840.1.113883.3.526.3.1256"//Allergy to Influenza Vaccine Grouping Value Set
								&& listAllProbsCur[j].ValueSetOID!="2.16.840.1.113883.3.526.3.1257")//Intolerance to Influenza Vaccine Grouping Value Set
							{
								continue;
							}
							for(int k=0;k<listOccurrenceADates.Count;k++) {
								if(listAllProbsCur[j].DateStart.Date<=listOccurrenceADates[k].Date) {
									alldata.ListEhrPats[i].IsException=true;
									alldata.ListEhrPats[i].Explanation="The patient had an eligible procedure or encounter on "+listOccurrenceADates[k].ToShortDateString()+" and had a diagnosis of allergy or intolerance to the influenza vaccine or an allergy to eggs prior to or on that date.";
									isCategorized=true;
									break;
								}
							}
							if(isCategorized) {
								break;
							}
						}
						if(isCategorized) {
							continue;
						}
						for(int j=0;j<listAllMedPatsCur.Count;j++) {
							if(listAllMedPatsCur[j].CompletionStatus==VaccineCompletionStatus.Complete) {//Only looking for NotAdministered, if status=Complete, skip
								continue;
							}
							for(int k=0;k<listOccurrenceADates.Count;k++) {
								if(listAllMedPatsCur[j].DateStart.Date<=listOccurrenceADates[k].Date) {
									alldata.ListEhrPats[i].IsException=true;
									alldata.ListEhrPats[i].Explanation="The patient had an eligible procedure or encounter on "+listOccurrenceADates[k].ToShortDateString()+" and had an influenza vaccine not given due to an allergy or intolerance prior to or on that date.";
									isCategorized=true;
									break;
								}
							}
							if(isCategorized) {
								break;
							}
						}
						if(isCategorized) {
							continue;
						}
						//if we get here, the patient is in the denominator but does not fit the numerator or exception criteria, Not Met
						alldata.ListEhrPats[i].Explanation="The patient had an eligible procedure or encounter but did not have an influenza vaccine administered or valid reason documented for not administering the vaccine.";
					}
					break;
				#endregion
				#region WeightChild_X_1
				//All the _1 measures will calculate BMI, height, and weight exams the same, the ListEhrPats will be limited by the age groups already
				case QualityType2014.WeightChild_1_1:
				case QualityType2014.WeightChild_2_1:
				case QualityType2014.WeightChild_3_1:
					//Strategy: alldata.ListEhrPats will be initial patient population
					//First check for pregnancy diagnosis, alldata.DictPatNumListProblems will hold all of the pregnancy problems that were active during any of the measurement period.
					//Exclusion if active pregnancy dx exists
					//alldata.DictPatNumListVitalsigns holds all vitalsign exams in the date range with a valid height and weight value.
					//Make sure the vitalsign.BMIExamCode, vitalsign.HeightExamCode, and vitalsign.WeightExamCode are in the allowed value sets
					//If the codes in BMI, height and weight exam code fields are in the value sets, 'Numerator'
					//No exceptions
					for(int i=0;i<alldata.ListEhrPats.Count;i++) {
						long patNumCur=alldata.ListEhrPats[i].EhrCqmPat.PatNum;
						bool isCategorized=false;
						//first apply pregnancy exclusion
						List<EhrCqmProblem> listProbsCur=new List<EhrCqmProblem>();
						if(alldata.DictPatNumListProblems.ContainsKey(patNumCur)) {
							listProbsCur=alldata.DictPatNumListProblems[patNumCur];
						}
						for(int j=0;j<listProbsCur.Count;j++) {
							if(listProbsCur[j].ValueSetOID=="2.16.840.1.113883.3.526.3.378") {//Pregnancy Grouping Value Set
								alldata.ListEhrPats[i].IsExclusion=true;
								alldata.ListEhrPats[i].Explanation="The patient had a pregnancy diagnosis that started on "+listProbsCur[j].DateStart.ToShortDateString()+" that is either still active or ended after the start of the measurement period.";
								isCategorized=true;
								break;
							}
						}
						if(isCategorized) {
							continue;
						}
						//get vital sign exams that took place in the measurement period for the patient.  Ordered by DateTaken DESC, so index 0 will hold the most recent exam
						List<EhrCqmVitalsign> listVitalsignsCur=new List<EhrCqmVitalsign>();
						if(alldata.DictPatNumListVitalsigns.ContainsKey(patNumCur)) {
							listVitalsignsCur=alldata.DictPatNumListVitalsigns[patNumCur];
						}
						if(listVitalsignsCur.Count==0) {//no vitalsign exams within 6 months of start of measurement period, but encounters exist or they would not be in the IPP
							alldata.ListEhrPats[i].Explanation="There is a valid encounter for this patient, but there are no BMI vital sign exams within 6 months of the measurement period.";
							continue;
						}
						//all three sets of exam codes come from the LOINC table only
						List<EhrCode> listBMIExamCodes=EhrCodes.GetForValueSetOIDs(new List<string>() { "2.16.840.1.113883.3.464.1003.121.12.1012" },false);//BMI percentile Grouping Value Set
						List<EhrCode> listHeightExamCodes=EhrCodes.GetForValueSetOIDs(new List<string>() { "2.16.840.1.113883.3.464.1003.121.12.1014" },false);//Height Grouping Value Set
						List<EhrCode> listWeightExamCodes=EhrCodes.GetForValueSetOIDs(new List<string>() { "2.16.840.1.113883.3.464.1003.121.12.1015" },false);//Weight Grouping Value Set
						//loop through vitalsign exams looking for valid height, weight, and BMI exam codes (percentile) based on value set OIDs
						for(int j=0;j<listVitalsignsCur.Count;j++) {
							bool isBMIExamCodeValid=false;
							bool isHeightExamCodeValid=false;
							bool isWeightExamCodeValid=false;
							for(int k=0;k<listBMIExamCodes.Count;k++) {
								if(listVitalsignsCur[j].BMIExamCode!=listBMIExamCodes[k].CodeValue) {
									continue;
								}
								if(listVitalsignsCur[j].BMIPercentile==-1) {//-1 if not in age range or if BMI percentile was not calculated correctly
									continue;
								}
								isBMIExamCodeValid=true;
								break;
							}
							for(int k=0;k<listHeightExamCodes.Count;k++) {
								if(listVitalsignsCur[j].HeightExamCode!=listHeightExamCodes[k].CodeValue) {
									continue;
								}
								isHeightExamCodeValid=true;
								break;
							}
							for(int k=0;k<listWeightExamCodes.Count;k++) {
								if(listVitalsignsCur[j].WeightExamCode!=listWeightExamCodes[k].CodeValue) {
									continue;
								}
								isWeightExamCodeValid=true;
								break;
							}
							if(isBMIExamCodeValid && isHeightExamCodeValid && isWeightExamCodeValid) {
								alldata.ListEhrPats[i].IsNumerator=true;
								alldata.ListEhrPats[i].Explanation="The vitalsign exam on "+listVitalsignsCur[j].DateTaken.ToShortDateString()+" has valid height, weight, and BMI Percentile code.";
								isCategorized=true;
								break;
							}
						}
						if(isCategorized) {
							continue;
						}
						alldata.ListEhrPats[i].Explanation="There is a valid encounter for this patient, but no valid vitalsign exam recording height, weight, and BMI in the measurement period.";
					}
					break;
				#endregion
				#region WeightChild_X_2 and WeightChild_X_3
				//All _2 and _3 measures will calculate the Nutrition/Physical Activity counseling interventions the same, already limited by the appropriate age groups
				case QualityType2014.WeightChild_1_2:
				case QualityType2014.WeightChild_2_2:
				case QualityType2014.WeightChild_3_2:
				case QualityType2014.WeightChild_1_3:
				case QualityType2014.WeightChild_2_3:
				case QualityType2014.WeightChild_3_3:
					//Strategy: alldata.ListEhrPats will be initial patient population
					//First check for pregnancy diagnosis, alldata.DictPatNumListProblems will hold all of the pregnancy problems that were active during any of the measurement period.
					//Exclusion if active pregnancy dx exists
					//alldata.DictPatNumListInterventions will hold all of the nutrition and physical activity counseling interventions that apply
					//if intervention exists, 'Numerator'
					string interventionType="physical activity";//used for generating explanation string, 1_3, 2_3, and 3_3 are phys activity
					string valueSetCur="2.16.840.1.113883.3.464.1003.118.12.1035";//Counseling for Physical Activity Grouping Value Set
					if(qtype==QualityType2014.WeightChild_1_2 || qtype==QualityType2014.WeightChild_2_2 || qtype==QualityType2014.WeightChild_3_2) {
						interventionType="nutrition";
						valueSetCur="2.16.840.1.113883.3.464.1003.195.12.1003";//Counseling for Nutrition Grouping Value Set
					}
					for(int i=0;i<alldata.ListEhrPats.Count;i++) {
						long patNumCur=alldata.ListEhrPats[i].EhrCqmPat.PatNum;
						bool isCategorized=false;
						//first apply pregnancy exclusion
						List<EhrCqmProblem> listProbsCur=new List<EhrCqmProblem>();
						if(alldata.DictPatNumListProblems.ContainsKey(patNumCur)) {
							listProbsCur=alldata.DictPatNumListProblems[patNumCur];
						}
						for(int j=0;j<listProbsCur.Count;j++) {
							if(listProbsCur[j].ValueSetOID=="2.16.840.1.113883.3.526.3.378") {//Pregnancy Grouping Value Set
								alldata.ListEhrPats[i].IsExclusion=true;
								alldata.ListEhrPats[i].Explanation="The patient had a pregnancy diagnosis that started on "+listProbsCur[j].DateStart.ToShortDateString()+" that is either still active or ended after the start of the measurement period.";
								isCategorized=true;
								break;
							}
						}
						if(isCategorized) {
							continue;
						}
						List<EhrCqmIntervention> listIntervenCur=new List<EhrCqmIntervention>();
						if(alldata.DictPatNumListInterventions.ContainsKey(patNumCur)) {
							listIntervenCur=alldata.DictPatNumListInterventions[patNumCur];
						}
						//loop through interventions, if one for the current value set (could be nutrition or physical activity) exists, 'Numerator'
						for(int j=0;j<listIntervenCur.Count;j++) {
							if(listIntervenCur[j].ValueSetOID==valueSetCur) {
								alldata.ListEhrPats[i].IsNumerator=true;
								alldata.ListEhrPats[i].Explanation="This patient was counseled for "+interventionType+" on "+listIntervenCur[0].DateEntry.ToShortDateString()+".";
								isCategorized=true;
								break;
							}
						}
						if(isCategorized) {
							continue;
						}
						//eligible encounters exist, but no intervention for nutrition/physical activity counseling, not met
						alldata.ListEhrPats[i].Explanation="There is a valid encounter for this patient, but no "+interventionType+" counseling intervention in the measurement period.";
					}
					break;
				#endregion
				#region BloodPressureManage
				case QualityType2014.BloodPressureManage:
					//Strategy: alldata.ListEhrPats will be the initial patient population, DictPatNumListEncounters holds all encounters for each patient in ListEhrPats
					//Denominator: initial patient population
					//Exclusions: alldata.DictPatNumListProblems holds all diagnoses for the patient that started before the measurement period end date
					//and either have no stop date or the stop date is after the period start date
						//DictPatNumListProblems will also hold the hypertension dx's, so only exclude if problem is pregnancy, end stage renal disease, or chronic kidney disease stage 5
						//DictPatNumListInterventions holds all interventions that exclude the patient from this measure in the date range, if intervention exists, exclude
						//DictPatNumListProcs holds all procedures that exclude the patient form this measure in the date range, if proc exists, exclude
						//DictPatNumListEncounters will have any ESRD Monthly Outpatient Services encounters in the list, if one of these encounters exist, exclude
					//Numerator: During most recent encounter (that is not an ESRD Monthly... encounter from exclusion ValueSetOID), BPDiastolic<90 mmHg, BPSystolic<140 mmHg
					for(int i=0;i<alldata.ListEhrPats.Count;i++) {
						long patNumCur=alldata.ListEhrPats[i].EhrCqmPat.PatNum;
						bool isCategorized=false;
						//first apply exclusion diagnoses, problem list will hold hypertension dx's, so have to check for valid exlusion dx
						List<EhrCqmProblem> listProbsCur=new List<EhrCqmProblem>();
						if(alldata.DictPatNumListProblems.ContainsKey(patNumCur)) {
							listProbsCur=alldata.DictPatNumListProblems[patNumCur];
						}
						for(int j=0;j<listProbsCur.Count;j++) {
							if(listProbsCur[j].ValueSetOID=="2.16.840.1.113883.3.526.3.378") {//Pregnancy Grouping Value Set
								alldata.ListEhrPats[i].IsExclusion=true;
								alldata.ListEhrPats[i].Explanation="The patient had a pregnancy diagnosis that started on "+listProbsCur[j].DateStart.ToShortDateString()+" that is either still active or ended after the start of the measurement period.";
								isCategorized=true;
								break;
							}
							if(listProbsCur[j].ValueSetOID=="2.16.840.1.113883.3.526.3.353") {//End Stage Renal Disease Grouping Value Set
								alldata.ListEhrPats[i].IsExclusion=true;
								alldata.ListEhrPats[i].Explanation="The patient had an end stage renal disease diagnosis that started on "+listProbsCur[j].DateStart.ToShortDateString()+" that is either still active or ended after the start of the measurement period.";
								isCategorized=true;
								break;
							}
							if(listProbsCur[j].ValueSetOID=="2.16.840.1.113883.3.526.3.1002") {//Chronic Kidney Disease, Stage 5 Grouping Value Set
								alldata.ListEhrPats[i].IsExclusion=true;
								alldata.ListEhrPats[i].Explanation="The patient had a chronic kidney disease, stage 5 diagnosis that started on "+listProbsCur[j].DateStart.ToShortDateString()+" that is either still active or ended after the start of the measurement period.";
								isCategorized=true;
								break;
							}
						}
						if(isCategorized) {
							continue;
						}
						//next apply exclusion interventions, only valid interventions will be in the list
						if(alldata.DictPatNumListInterventions.ContainsKey(patNumCur)) {
							alldata.ListEhrPats[i].IsExclusion=true;
							alldata.ListEhrPats[i].Explanation="The patient had an intervention for dialysis education or other services related to dialysis on "+alldata.DictPatNumListInterventions[patNumCur][0].DateEntry.ToShortDateString()+".";
							continue;
						}
						//next apply exclusion procedures, only valid procs will be in the list
						if(alldata.DictPatNumListProcs.ContainsKey(patNumCur)) {
							alldata.ListEhrPats[i].IsExclusion=true;
							alldata.ListEhrPats[i].Explanation="The patient had a procedure performed for vascular access for dialysis, kidney transplant, or dialysis services on "+alldata.DictPatNumListProcs[patNumCur][0].ProcDate.ToShortDateString()+".";
							continue;
						}
						//finally apply exclusion encounters, have to loop through encounters and look for any ESRD Monthly Outpatient Services encounters
						//while looping through encounters, get the most recent encounter that is not an ESRD encounter
						DateTime dateMostRecentEnc=DateTime.MinValue;
						List<EhrCqmEncounter> listEncountersCur=alldata.DictPatNumListEncounters[patNumCur];
						for(int j=0;j<listEncountersCur.Count;j++) {
							if(listEncountersCur[j].ValueSetOID=="2.16.840.1.113883.3.464.1003.109.12.1014") {//ESRD Monthly Outpatient Services Grouping Value Set
								alldata.ListEhrPats[i].IsExclusion=true;
								alldata.ListEhrPats[i].Explanation="The patient had an end stage renal disease monthly outpatient services encounter on "+alldata.DictPatNumListEncounters[patNumCur][j].DateEncounter.ToShortDateString()+".";
								isCategorized=true;
								continue;
							}
							if(listEncountersCur[j].DateEncounter.Date>dateMostRecentEnc.Date) {
								dateMostRecentEnc=listEncountersCur[j].DateEncounter;
							}
						}
						if(isCategorized) {
							continue;
						}
						//patient not excluded, try to match numerator criteria
						//using dateMostRecentEnc set above, try to find a vitalsign exam on the same date with recorded Diastolic BP < 90 mmHg and Systolic BP < 140 mmHg
						List<EhrCqmVitalsign> listVitalsignsCur=new List<EhrCqmVitalsign>();
						if(alldata.DictPatNumListVitalsigns.ContainsKey(patNumCur)) {
							listVitalsignsCur=alldata.DictPatNumListVitalsigns[patNumCur];
						}
						int recentSystolic=0;
						int recentDiastolic=0;
						for(int j=0;j<listVitalsignsCur.Count;j++) {
							if(listVitalsignsCur[j].DateTaken.Date!=dateMostRecentEnc.Date) {
								continue;
							}
							if(j==0) {
								recentSystolic=listVitalsignsCur[j].BpSystolic;
								recentDiastolic=listVitalsignsCur[j].BpDiastolic;
							}
							if(listVitalsignsCur[j].BpSystolic<recentSystolic) {
								recentSystolic=listVitalsignsCur[j].BpSystolic;
							}
							if(listVitalsignsCur[j].BpDiastolic<recentDiastolic) {
								recentDiastolic=listVitalsignsCur[j].BpDiastolic;
							}
						}
						if(recentSystolic>0 && recentSystolic<140 && recentDiastolic>0 && recentDiastolic<90) {
							alldata.ListEhrPats[i].IsNumerator=true;
							alldata.ListEhrPats[i].Explanation="The patient had a vitalsign exam on "+dateMostRecentEnc.ToShortDateString()
								+" with systolic blood pressure "+recentSystolic.ToString()+" mmHg and diastolic blood pressure "+recentDiastolic.ToString()+" mmHg.";
							continue;
						}
						//no exceptions, if not in the numerator, patient had a hypertension dx within 6 months of the measurement period start date or any time before the measurement period that did not end before the measurement period start date, was not excluded, and either did not have a vitalsign exam recording BP during their most recent encounter or their most recent encounter with vitalsign exam recorded BP above the allowed range (diastolic<140, systolic<90)
						alldata.ListEhrPats[i].Explanation="The patient's most recent qualifying encounter on "+dateMostRecentEnc.ToShortDateString();
						if(recentSystolic==0 || recentDiastolic==0) {//BP was not recorded during most recent encounter
							alldata.ListEhrPats[i].Explanation+=" did not have a corresponding vitalsign exam recording systolic and diastolic blood pressure with the same date.";
						}
						else {
							alldata.ListEhrPats[i].Explanation+=" had a corresponding vitalsign exam with systolic blood pressure "
								+recentSystolic.ToString()+" mmHg and diastolic blood pressure "+recentDiastolic.ToString()+" mmHg.";
						}
					}
					break;
				#endregion
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		///<summary>Just counts up the number of rows with an X in the numerator column.  Very simple.</summary>
		public static int CalcNumerator(DataTable table) {
			//No need to check RemotingRole; no call to db.
			int retVal=0;
			for(int i=0;i<table.Rows.Count;i++) {
				if(table.Rows[i]["numerator"].ToString()=="X") {
					retVal++;
				}
			}
			return retVal;
		}

		///<summary>Just counts up the number of EhrPatients with IsNumerator=true.</summary>
		private static int CalcNumerator2014(List<EhrCqmPatient> listPats) {
			//No need to check RemotingRole; no call to db.
			int retval=0;
			for(int i=0;i<listPats.Count;i++) {
				if(listPats[i].IsNumerator) {
					retval++;
				}
			}
			return retval;
		}

		///<summary>Just counts up the number of EhrPatients with IsDenominator=true.  The only measure that may have some patients in the initial patient population that are not in the denominator is the influenza vaccine measure, CMS147v2.</summary>
		private static int CalcDenominator2014(List<EhrCqmPatient> listPats) {
			//No need to check RemotingRole; no call to db.
			int retval=0;
			for(int i=0;i<listPats.Count;i++) {
				if(listPats[i].IsDenominator) {
					retval++;
				}
			}
			return retval;
		}

		///<summary>This is only used by measure 68, current meds documented.  This is the only measure that is not patient based, but encounter based.  So every eligible encounter is the denominator/ipp.  We will have to handle this case carefully.</summary>
		private static int CalcDenominator2014_Encs(Dictionary<long,List<EhrCqmEncounter>> dictPatNumListEncs) {
			//No need to check RemotingRole; no call to db.
			int retval=0;
			foreach(KeyValuePair<long,List<EhrCqmEncounter>> kvpair in dictPatNumListEncs) {
				for(int i=0;i<kvpair.Value.Count;i++) {
					retval++;
				}
			}
			return retval;
		}

		///<summary>Just counts up the number of rows with an X in the exclusion column.  Very simple.</summary>
		public static int CalcExclusions(DataTable table) {
			//No need to check RemotingRole; no call to db.
			int retVal=0;
			for(int i=0;i<table.Rows.Count;i++) {
				if(table.Rows[i]["exclusion"].ToString()=="X") {
					retVal++;
				}
			}
			return retVal;
		}

		///<summary>Just counts up the number of EhrPatients with IsException=true.</summary>
		private static int CalcExceptions2014(List<EhrCqmPatient> listPats) {
			//No need to check RemotingRole; no call to db.
			int retval=0;
			for(int i=0;i<listPats.Count;i++) {
				if(listPats[i].IsException) {
					retval++;
				}
			}
			return retval;
		}

		///<summary>This is only used by measure 68, current meds documented.  This is the only measure that is not patient based, but encounter based.  Simply loop through all encounters for all patients in the dictionary and count up the encounters with IsNumerator=true.</summary>
		private static int CalcNumerator2014_Encs(Dictionary<long,List<EhrCqmEncounter>> dictPatNumListEncs) {
			//No need to check RemotingRole; no call to db.
			int retval=0;
			foreach(KeyValuePair<long,List<EhrCqmEncounter>> kvpair in dictPatNumListEncs) {
				for(int i=0;i<kvpair.Value.Count;i++) {
					if(kvpair.Value[i].IsNumerator) {
						retval++;
					}
				}
			}
			return retval;
		}

		///<summary>This is only used by measure 68, current meds documented.  This is the only measure that is not patient based, but encounter based.  Simply loop through all encounters for all patients in the dictionary and count up the encounters with IsException=true.</summary>
		private static int CalcException2014_Encs(Dictionary<long,List<EhrCqmEncounter>> dictPatNumListEncs) {
			//No need to check RemotingRole; no call to db.
			int retval=0;
			foreach(KeyValuePair<long,List<EhrCqmEncounter>> kvpair in dictPatNumListEncs) {
				for(int i=0;i<kvpair.Value.Count;i++) {
					if(kvpair.Value[i].IsException) {
						retval++;
					}
				}
			}
			return retval;
		}

		///<summary>Just counts up the number of EhrPatients with IsExclusion=true.</summary>
		private static int CalcExclusions2014(List<EhrCqmPatient> listPats) {
			//No need to check RemotingRole; no call to db.
			int retval=0;
			for(int i=0;i<listPats.Count;i++) {
				if(listPats[i].IsExclusion) {
					retval++;
				}
			}
			return retval;
		}

		private static string GetDenominatorExplain(QualityType qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType.WeightOver65:
					return "All patients 65+ with at least one visit during the measurement period.";
				case QualityType.WeightAdult:
					return "All patients 18 to 64 with at least one visit during the measurement period.";
				case QualityType.Hypertension:
					return "All patients 18+ with ICD9 hypertension(401-404) and at least two visits, one during the measurement period.";
				case QualityType.TobaccoUse:
					//The original manual that these specs came from stated ALL patients seen during the period, and did not say anything about needing two visits.
					return "All patients 18+ with at least one visit during the measurement period.";
				case QualityType.TobaccoCessation:
					//It's inconsistent.  Sometimes it says 24 months from now (which doesn't make sense).  
					//Other times it says 24 months from last visit.  We're going with that.
					//Again, we will ignore the part about needing two visits.
					return "All patients 18+ with at least one visit during the measurement period; and identified as tobacco users within the 24 months prior to the last visit.";
				case QualityType.InfluenzaAdult:
					//the documentation is very sloppy.  It's including a flu season daterange in the denominator that is completely illogical.
					return "All patients 50+ with a visit during the measurement period.";
				case QualityType.WeightChild_1_1:
				case QualityType.WeightChild_1_2:
				case QualityType.WeightChild_1_3:
					return "All patients 2-16 with a visit during the measurement period, unless pregnant.";
				case QualityType.WeightChild_2_1:
				case QualityType.WeightChild_2_2:
				case QualityType.WeightChild_2_3:
					return "All patients 2-10 with a visit during the measurement period, unless pregnant.";
				case QualityType.WeightChild_3_1:
				case QualityType.WeightChild_3_2:
				case QualityType.WeightChild_3_3:
					return "All patients 11-16 with a visit during the measurement period, unless pregnant.";
				case QualityType.ImmunizeChild_1:
				case QualityType.ImmunizeChild_2:
				case QualityType.ImmunizeChild_3:
				case QualityType.ImmunizeChild_4:
				case QualityType.ImmunizeChild_5:
				case QualityType.ImmunizeChild_6:
				case QualityType.ImmunizeChild_7:
				case QualityType.ImmunizeChild_8:
				case QualityType.ImmunizeChild_9:
				case QualityType.ImmunizeChild_10:
				case QualityType.ImmunizeChild_11:
				case QualityType.ImmunizeChild_12:
					return "All patients with a visit during the measurement period who turned 2 during the measurement period.";
				case QualityType.Pneumonia:
					return "All patients 65+ during the measurement period with a visit within 1 year before the measurement end date.";
				case QualityType.DiabetesBloodPressure:
					return "All patients 17-74 before the measurement period, who either have a diabetes-related medication dispensed, or who have an active diagnosis of diabetes.";
				case QualityType.BloodPressureManage:
					return "All patients 17-74 before the measurement period who have an active diagnosis of hypertension and who are not pregnant or have ESRD.";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetNumeratorExplain(QualityType qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType.WeightOver65:
					return @"BMI < 22 or >= 30 with Followup documented.
BMI 22-30.";
				case QualityType.WeightAdult:
					return @"BMI < 18.5 or >= 25 with Followup documented.
BMI 18.5-25.";
				case QualityType.Hypertension:
					return "Blood pressure entered during measurement period.";
				case QualityType.TobaccoUse:
					return "Tobacco use recorded within the 24 months prior to the last visit.";
				case QualityType.TobaccoCessation:
					return "Tobacco cessation entry within the 24 months prior to the last visit.";
				case QualityType.InfluenzaAdult:
					return "Influenza vaccine administered.";
				case QualityType.WeightChild_1_1:
					return "BMI recorded during measurement period.";
				case QualityType.WeightChild_1_2:
					return "Counseling for nutrition during measurement period.";
				case QualityType.WeightChild_1_3:
					return "Counseling for physical activity during measurement period.";
				case QualityType.WeightChild_2_1:
					return "BMI recorded during measurement period.";
				case QualityType.WeightChild_2_2:
					return "Counseling for nutrition during measurement period.";
				case QualityType.WeightChild_2_3:
					return "Counseling for physical activity during measurement period.";
				case QualityType.WeightChild_3_1:
					return "BMI recorded during measurement period.";
				case QualityType.WeightChild_3_2:
					return "Counseling for nutrition during measurement period.";
				case QualityType.WeightChild_3_3:
					return "Counseling for physical activity during measurement period.";
				case QualityType.ImmunizeChild_1:
					return "4 DTaP vaccinations between 42 days and 2 years of age. CVX=110,120,20,50";
				case QualityType.ImmunizeChild_2:
					return "3 IPV vaccinations between 42 days and 2 years of age. CVX=10,120";
				case QualityType.ImmunizeChild_3:
					return "1 MMR vaccination before 2 years of age. CVX=03,94\r\n"
						+"OR 1 measles(05), 1 mumps(07), and 1 rubella(06).";
				case QualityType.ImmunizeChild_4:
					//the intro paragraph states 4 HiB.  They have a typo someplace.
					return "2 HiB vaccinations between 42 days and 2 years of age. CVX=120,46,47,48,49,50,51";
				case QualityType.ImmunizeChild_5:
					return "3 hepatitis B vaccinations before 2 years of age. CVX=08,110,44,51";
				case QualityType.ImmunizeChild_6:
					return "1 VZV vaccination before 2 years of age. CVX=21,94";
				case QualityType.ImmunizeChild_7:
					return "4 pneumococcal vaccinations between 42 days and 2 years of age. CVX=100,133";
				case QualityType.ImmunizeChild_8:
					return "2 hepatitis A vaccinations before 2 years of age. CVX=83";
				case QualityType.ImmunizeChild_9:
					return "2 rotavirus vaccinations between 42 days and 2 years of age. CVX=116,119";
				case QualityType.ImmunizeChild_10:
					return "2 influenza vaccinations between 180 days and 2 years of age. CVX=135,15";
				case QualityType.ImmunizeChild_11:
					return "All vaccinations 1-6.";
				case QualityType.ImmunizeChild_12:
					return "All vaccinations 1-7.";
				case QualityType.Pneumonia:
					return "Pneumococcal vaccine before measurement end date. CVX=33,100,133";
				case QualityType.DiabetesBloodPressure:
					return "Diastolic < 90 and systolic < 140 at most recent encounter.";
				case QualityType.BloodPressureManage:
					return "Diastolic < 90 and systolic < 140 at most recent encounter.";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetExclusionsExplain(QualityType qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType.WeightOver65:
					return "Marked ineligible within 6 months prior to the last visit.";
				case QualityType.WeightAdult:
					return "Terminal ineligible within 6 months prior to the last visit.";
				case QualityType.Hypertension:
					return "N/A";
				case QualityType.TobaccoUse:
					return "N/A";
				case QualityType.TobaccoCessation:
					return "N/A";
				case QualityType.InfluenzaAdult:
					return "A valid reason was entered for medication not given.";
				case QualityType.WeightChild_1_1:
				case QualityType.WeightChild_1_2:
				case QualityType.WeightChild_1_3:
				case QualityType.WeightChild_2_1:
				case QualityType.WeightChild_2_2:
				case QualityType.WeightChild_2_3:
				case QualityType.WeightChild_3_1:
				case QualityType.WeightChild_3_2:
				case QualityType.WeightChild_3_3:
					return "N/A";
				case QualityType.ImmunizeChild_1:
				case QualityType.ImmunizeChild_2:
				case QualityType.ImmunizeChild_3:
				case QualityType.ImmunizeChild_4:
				case QualityType.ImmunizeChild_5:
				case QualityType.ImmunizeChild_6:
				case QualityType.ImmunizeChild_7:
				case QualityType.ImmunizeChild_8:
				case QualityType.ImmunizeChild_9:
				case QualityType.ImmunizeChild_10:
				case QualityType.ImmunizeChild_11:
				case QualityType.ImmunizeChild_12:
					return "Contraindicated due to specific allergy or disease.";
				case QualityType.Pneumonia:
					return "N/A";
				case QualityType.DiabetesBloodPressure:
					return "1. Diagnosis polycystic ovaries and not active diagnosis of diabetes.\r\n"
						+"or 2. Medication was due to an accute episode, and not active diagnosis of diabetes.";
				case QualityType.BloodPressureManage:
					return "N/A";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetDenominatorExplain2014(QualityType2014 qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType2014.MedicationsEntered:
					return "All eligible encounters occurring during the reporting period for patients age 18+ at the start of the measurement period.";
				case QualityType2014.WeightOver65:
					return "All patients age 65+ at the start of the measurement period with an eligible encounter during the measurement period.  Not including encounters where the patient is receiving palliative care, the patient refuses measurement of height and/or weight, the patient is in an urgent or emergent medical situation, or there is any other qualified reason documenting why BMI measurement was not appropriate.";
				case QualityType2014.WeightAdult:
					return "All patients age 18 to 64 at the start of the measurement period with an eligible encounter during the measurement period. Not including encounters where the patient is receiving palliative care, the patient refuses measurement of height and/or weight, the patient in an urgent or emergent medical situation, or there is any other qualified reason documenting why BMI measurement was not appropriate.";
				case QualityType2014.CariesPrevent:
					return "Children age 0-19 at the start of the measurement period with an eligible encounter during the measurement period.";
				case QualityType2014.CariesPrevent_1:
					return "Children age 0-5 at the start of the measurement period with an eligible encounter during the measurement period.";
				case QualityType2014.CariesPrevent_2:
					return "Children age 6-12 at the start of the measurement period with an eligible encounter during the measurement period.";
				case QualityType2014.CariesPrevent_3:
					return "Children age 13-19 at the start of the measurement period with an eligible encounter during the measurement period.";
				case QualityType2014.ChildCaries:
					return "Childred age 0-19 at the start of the measurement period with an eligible encounter during the measurement period.";
				case QualityType2014.Pneumonia:
					return "All patients age 65+ at the start of the measurement period with an eligible encounter during the measurement period.";
				case QualityType2014.TobaccoCessation:
					return "All patients age 18+ at the start of the measurement period with an eligible encounter(s) during the measurement period.";
				case QualityType2014.Influenza:
					return "All patients 6 months+ at the start of the measurement period with eligible influenza encounter between October 1 of the year before the measurement period and March 31 of the measurement period and an eligible encounter during the measurement period.";
				case QualityType2014.WeightChild_1_1:
				case QualityType2014.WeightChild_1_2:
				case QualityType2014.WeightChild_1_3:
					return "All patients age 3-16 at the start of the measurement period with an eligible encounter during the measurement period.";
				case QualityType2014.WeightChild_2_1:
				case QualityType2014.WeightChild_2_2:
				case QualityType2014.WeightChild_2_3:
					return "All patients age 3-11 at the start of the measurement period with an eligible encounter during the measurement period.";
				case QualityType2014.WeightChild_3_1:
				case QualityType2014.WeightChild_3_2:
				case QualityType2014.WeightChild_3_3:
					return "All patients age 12-16 at the start of the measurement period with an eligible encounter during the measurement period.";
				case QualityType2014.BloodPressureManage:
					return "All patients age 18-84 at the start of the measurement period with an eligible encounter during the measurement period who have an active diagnosis of hypertension that starts before or within the first 6 months of the start of the measurement period.";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetNumeratorExplain2014(QualityType2014 qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType2014.MedicationsEntered:
					return "Encounters during which the provider attests to documenting a list of current medications to the best of his/her knowledge and ability by completing the current medications documented procedure.";
				case QualityType2014.WeightOver65:
					return "Patients who, for every encounter in the measurement period, the most recent physical examination documenting BMI occurred during the encounter or during the previous six months.  If the BMI was outside of normal parameters at any exam, a follow-up plan was documented during the exam or during the previous six months.";
				case QualityType2014.WeightAdult:
					return "Patients who, for every encounter in the measurement period, the most recent physical examination documenting BMI occurred during the encounter or during the previous six months.  If the BMI was outside of normal parameters at any exam, a follow-up plan was documented during the exam or during the previous six months.";
				case QualityType2014.CariesPrevent:
					return "Childred with an eligible flouride varnish procedure performed during the measurement period.";
				case QualityType2014.CariesPrevent_1:
					return "Childred with an eligible flouride varnish procedure performed during the measurement period.";
				case QualityType2014.CariesPrevent_2:
					return "Childred with an eligible flouride varnish procedure performed during the measurement period.";
				case QualityType2014.CariesPrevent_3:
					return "Childred with an eligible flouride varnish procedure performed during the measurement period.";
				case QualityType2014.ChildCaries:
					return "Childred with a diagnosis of caries with an eligible code during the measurement period.";
				case QualityType2014.Pneumonia:
					return "Patients with a Pneumococcal vaccination with qualified code administered or a history of the vaccination documented before or during the measurement period.";
				case QualityType2014.TobaccoCessation:
					return "Patients whose most recent assessment of tobacco use was within 24 month of the measurement end date and if characterized as a user also received tobacco cessation counseling intervention.";
				case QualityType2014.Influenza:
					return "Patients who received the influenza vaccination during the eligible influenza encounter or reported previous receipt of the vaccination during the encounter.";
				case QualityType2014.WeightChild_1_1:
				case QualityType2014.WeightChild_2_1:
				case QualityType2014.WeightChild_3_1:
					return "Patiens who had height, weight, and BMI percentile recorded during measurement period.";
				case QualityType2014.WeightChild_1_2:
				case QualityType2014.WeightChild_2_2:
				case QualityType2014.WeightChild_3_2:
					return "Patients who were counseled for nutrition during measurement period.";
				case QualityType2014.WeightChild_1_3:
				case QualityType2014.WeightChild_2_3:
				case QualityType2014.WeightChild_3_3:
					return "Patients who were counseled for physical activity during measurement period.";
				case QualityType2014.BloodPressureManage:
					return "Patients whose blood pressure was recorded during the most recent eligible encounter, and the results were diastolic < 90 mmHg and systolic < 140 mmHg.";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		///<summary>The exclusions and exceptions are very similar, in fact no measure from our set of 9 CQM's has both an exclusion and an exception.  Possibly combine these into one text box called "Exclusions/Exceptions" to save space on FormQualityMeasureEdit2014.  We will have to report them in the correctly labeled section in our QRDA III report, so we probably have to keep them in separate functions.  The difference between them is when to take each into account in calculating measure.  Find denominator, apply exclusions, then classify using numerator criteria, then only if not in numerator apply exceptions and subtract from denominator.  Exceptions only apply if the patient/encounter doesn't meet the numerator criteria, don't immediately subtract from the denominator population.</summary>
		private static string GetExclusionsExplain2014(QualityType2014 qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType2014.MedicationsEntered:
					return "N/A";
				case QualityType2014.WeightOver65:
				case QualityType2014.WeightAdult:
					return "Patients who were pregnant during any of the measurement period.";
				case QualityType2014.CariesPrevent:
				case QualityType2014.CariesPrevent_1:
				case QualityType2014.CariesPrevent_2:
				case QualityType2014.CariesPrevent_3:
				case QualityType2014.ChildCaries:
				case QualityType2014.Pneumonia:
				case QualityType2014.TobaccoCessation:
				case QualityType2014.Influenza:
					return "N/A";
				case QualityType2014.WeightChild_1_1:
				case QualityType2014.WeightChild_1_2:
				case QualityType2014.WeightChild_1_3:
				case QualityType2014.WeightChild_2_1:
				case QualityType2014.WeightChild_2_2:
				case QualityType2014.WeightChild_2_3:
				case QualityType2014.WeightChild_3_1:
				case QualityType2014.WeightChild_3_2:
				case QualityType2014.WeightChild_3_3:
					return "Patients who are pregnant during any of the measurement period.";
				case QualityType2014.BloodPressureManage:
					return "Patients who are pregnant during any of the measurement period.  Patients who had an active diagnosis of end stage renal disease, chronic kidney disease, or were undergoing dialysis during the measurement period.";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetExceptionsExplain2014(QualityType2014 qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType2014.MedicationsEntered:
					return "Encounters where the current medications documented procedure was not performed due to an eligible medical or other reason.";
				case QualityType2014.WeightOver65:
				case QualityType2014.WeightAdult:
				case QualityType2014.CariesPrevent:
				case QualityType2014.CariesPrevent_1:
				case QualityType2014.CariesPrevent_2:
				case QualityType2014.CariesPrevent_3:
				case QualityType2014.ChildCaries:
				case QualityType2014.Pneumonia:
					return "N/A";
				case QualityType2014.TobaccoCessation:
					return "Patients with a diagnosis of limited life expectancy during the measurement period or who have another eligible medical reason documented for not performing the screening.";
				case QualityType2014.Influenza:
					return @"Patients who have a documented medical reason (e.g. patient allergy), system reason (e.g. drug not available), or patient reason (e.g. medication refused) for not receiving the immunization.";
				case QualityType2014.WeightChild_1_1:
				case QualityType2014.WeightChild_1_2:
				case QualityType2014.WeightChild_1_3:
				case QualityType2014.WeightChild_2_1:
				case QualityType2014.WeightChild_2_2:
				case QualityType2014.WeightChild_2_3:
				case QualityType2014.WeightChild_3_1:
				case QualityType2014.WeightChild_3_2:
				case QualityType2014.WeightChild_3_3:
				case QualityType2014.BloodPressureManage:
					return "N/A";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetEMeasureTitle(QualityType2014 qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType2014.MedicationsEntered:
					return "Documentation of Current Medications in the Medical Record";
				case QualityType2014.WeightOver65://: age 65 and over//: age 18 to 64
				case QualityType2014.WeightAdult:
					return "Preventive Care and Screening: Body Mass Index (BMI) Screening and Follow-Up";
				case QualityType2014.CariesPrevent:
				case QualityType2014.CariesPrevent_1://: age 0 to 5
				case QualityType2014.CariesPrevent_2://: age 6 to 12
				case QualityType2014.CariesPrevent_3://: age 13 to 20
					return "Primary Caries Prevention Intervention as Offered by Primary Care Providers, including Dentists";
				case QualityType2014.ChildCaries:
					return "Children Who Have Dental Decay or Cavities";
				case QualityType2014.Pneumonia:
					return "Pneumonia Vaccination Status for Older Adults";
				case QualityType2014.TobaccoCessation:
					return "Preventive Care and Screening: Tobacco Use: Screening and Cessation Intervention";
				case QualityType2014.Influenza:
					return "Preventive Care and Screening: Influenza Immunization";
				case QualityType2014.WeightChild_1_1://: Height, Weight, and BMI: age 3 to 16
				case QualityType2014.WeightChild_2_1://: Height, Weight, and BMI: age 3 to 11
				case QualityType2014.WeightChild_3_1://: Height, Weight, and BMI: age 12 to 16
				case QualityType2014.WeightChild_1_2://: Counseled for Nutrition: age 3 to 16
				case QualityType2014.WeightChild_2_2://: Counseled for Nutrition: age 3 to 11
				case QualityType2014.WeightChild_3_2://: Counseled for Nutrition: age 12 to 16
				case QualityType2014.WeightChild_1_3://: Counseled for Physical Activity: age 3 to 16
				case QualityType2014.WeightChild_2_3://: Counseled for Physical Activity: age 3 to 11
				case QualityType2014.WeightChild_3_3://: Counseled for Physical Activity: age 12 to 16
					return "Weight Assessment and Counseling for Nutrition and Physical Activity for Children and Adolescents";
				case QualityType2014.BloodPressureManage:
					return "Controlling High Blood Pressure";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetEMeasureNum(QualityType2014 qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType2014.MedicationsEntered:
					return "68";
				case QualityType2014.WeightOver65:
				case QualityType2014.WeightAdult:
					return "69";
				case QualityType2014.CariesPrevent:
				case QualityType2014.CariesPrevent_1:
				case QualityType2014.CariesPrevent_2:
				case QualityType2014.CariesPrevent_3:
					return "74";
				case QualityType2014.ChildCaries:
					return "75";
				case QualityType2014.Pneumonia:
					return "127";
				case QualityType2014.TobaccoCessation:
					return "138";
				case QualityType2014.Influenza:
					return "147";
				case QualityType2014.WeightChild_1_1:
				case QualityType2014.WeightChild_2_1:
				case QualityType2014.WeightChild_3_1:
				case QualityType2014.WeightChild_1_2:
				case QualityType2014.WeightChild_2_2:
				case QualityType2014.WeightChild_3_2:
				case QualityType2014.WeightChild_1_3:
				case QualityType2014.WeightChild_2_3:
				case QualityType2014.WeightChild_3_3:
					return "155";
				case QualityType2014.BloodPressureManage:
					return "165";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetEMeasureVSpecificId(QualityType2014 qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType2014.MedicationsEntered:
					return "40280381-3E93-D1AF-013E-A36090B72CC8";
				case QualityType2014.WeightOver65:
				case QualityType2014.WeightAdult:
					return "40280381-3E93-D1AF-013E-D6E2B772150D";
				case QualityType2014.CariesPrevent:
				case QualityType2014.CariesPrevent_1:
				case QualityType2014.CariesPrevent_2:
				case QualityType2014.CariesPrevent_3:
					return "40280381-3D61-56A7-013E-8ACBFE873BCB";
				case QualityType2014.ChildCaries:
					return "40280381-3D61-56A7-013E-8F2F5AD054E2";
				case QualityType2014.Pneumonia:
					return "40280381-3D61-56A7-013E-66A79D4A4A23";
				case QualityType2014.TobaccoCessation:
					return "40280381-3D61-56A7-013E-5CD94A4D64FA";
				case QualityType2014.Influenza:
					return "40280381-3D61-56A7-013E-57F49972361A";
				case QualityType2014.WeightChild_1_1:
				case QualityType2014.WeightChild_2_1:
				case QualityType2014.WeightChild_3_1:
				case QualityType2014.WeightChild_1_2:
				case QualityType2014.WeightChild_2_2:
				case QualityType2014.WeightChild_3_2:
				case QualityType2014.WeightChild_1_3:
				case QualityType2014.WeightChild_2_3:
				case QualityType2014.WeightChild_3_3:
					return "40280381-3D61-56A7-013E-5D530FD26D47";
				case QualityType2014.BloodPressureManage:
					return "40280381-3D61-56A7-013E-66BC02DA4DEE";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetEMeasureVNeutralId(QualityType2014 qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType2014.MedicationsEntered:
					return "9a032d9c-3d9b-11e1-8634-00237d5bf174";
				case QualityType2014.WeightOver65:
				case QualityType2014.WeightAdult:
					return "9a031bb8-3d9b-11e1-8634-00237d5bf174";
				case QualityType2014.CariesPrevent:
				case QualityType2014.CariesPrevent_1:
				case QualityType2014.CariesPrevent_2:
				case QualityType2014.CariesPrevent_3:
					return "0b81b6ba-3b30-41bf-a2f3-95bdc9f558f2";
				case QualityType2014.ChildCaries:
					return "61947125-4376-4a7b-ab7a-ac2be9bd9138";
				case QualityType2014.Pneumonia:
					return "59657b9b-01bf-4979-a090-8534da1d0516";
				case QualityType2014.TobaccoCessation:
					return "e35791df-5b25-41bb-b260-673337bc44a8";
				case QualityType2014.Influenza:
					return "a244aa29-7d11-4616-888a-86e376bfcc6f";
				case QualityType2014.WeightChild_1_1:
				case QualityType2014.WeightChild_2_1:
				case QualityType2014.WeightChild_3_1:
				case QualityType2014.WeightChild_1_2:
				case QualityType2014.WeightChild_2_2:
				case QualityType2014.WeightChild_3_2:
				case QualityType2014.WeightChild_1_3:
				case QualityType2014.WeightChild_2_3:
				case QualityType2014.WeightChild_3_3:
					return "0b63f730-25d6-4248-b11f-8c09c66a04eb";
				case QualityType2014.BloodPressureManage:
					return "abdc37cc-bac6-4156-9b91-d1be2c8b7268";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetEMeasureVersion(QualityType2014 qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType2014.MedicationsEntered:
				case QualityType2014.CariesPrevent:
				case QualityType2014.CariesPrevent_1:
				case QualityType2014.CariesPrevent_2:
				case QualityType2014.CariesPrevent_3:
					return "3";
				case QualityType2014.WeightOver65:
				case QualityType2014.WeightAdult:
				case QualityType2014.ChildCaries:
				case QualityType2014.Pneumonia:
				case QualityType2014.TobaccoCessation:
				case QualityType2014.Influenza:
				case QualityType2014.WeightChild_1_1:
				case QualityType2014.WeightChild_2_1:
				case QualityType2014.WeightChild_3_1:
				case QualityType2014.WeightChild_1_2:
				case QualityType2014.WeightChild_2_2:
				case QualityType2014.WeightChild_3_2:
				case QualityType2014.WeightChild_1_3:
				case QualityType2014.WeightChild_2_3:
				case QualityType2014.WeightChild_3_3:
				case QualityType2014.BloodPressureManage:
					return "2";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetEMeasureSetId(QualityType2014 qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType2014.MedicationsEntered:
					return "680ffd36-c4e4-4145-b6c3-92418b037069";
				case QualityType2014.CariesPrevent:
				case QualityType2014.CariesPrevent_1:
				case QualityType2014.CariesPrevent_2:
				case QualityType2014.CariesPrevent_3:
					return "75d957a0-bdaf-49ad-b592-0e8fbb69d619";
				case QualityType2014.WeightOver65:
				case QualityType2014.WeightAdult:
					return "ffd4afd7-4981-48c2-92dd-73b764987409";
				case QualityType2014.ChildCaries:
					return "c4951021-42c7-4eb1-b1f3-55f2f0952b13";
				case QualityType2014.Pneumonia:
					return "097e91e6-bb8d-4f16-9acf-f7d9dce26799";
				case QualityType2014.TobaccoCessation:
					return "b2bef869-23a1-42e2-ba8f-44819daedf4e";
				case QualityType2014.Influenza:
					return "40d83c98-aaa9-4436-9c12-54b0896b0b2c";
				case QualityType2014.WeightChild_1_1:
				case QualityType2014.WeightChild_2_1:
				case QualityType2014.WeightChild_3_1:
				case QualityType2014.WeightChild_1_2:
				case QualityType2014.WeightChild_2_2:
				case QualityType2014.WeightChild_3_2:
				case QualityType2014.WeightChild_1_3:
				case QualityType2014.WeightChild_2_3:
				case QualityType2014.WeightChild_3_3:
					return "2b59c988-1e6a-44d7-a39d-aab90ad3a33a";
				case QualityType2014.BloodPressureManage:
					return "9d6d8dd1-120d-4f21-aecd-f447cc78171d";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetEMeasureIppId(QualityType2014 qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType2014.MedicationsEntered:
					return "FED9910E-07EF-40E1-B8F0-8B4331CBE8F0";
				case QualityType2014.WeightOver65:
					return "3FA95131-7882-49C0-9830-2ACB82E914D8";
				case QualityType2014.WeightAdult:
					return "95D45B98-C6A1-42F4-A0F5-617DF414A2AE";
				case QualityType2014.CariesPrevent:
					return "E91779A3-28D7-44F9-A150-6C8504809B1F";
				case QualityType2014.CariesPrevent_1:
					return "2C028996-D897-4162-ADBE-8619EDBE4E7D";
				case QualityType2014.CariesPrevent_2:
					return "59140979-7876-4A93-85EB-1276E2ED7584";
				case QualityType2014.CariesPrevent_3:
					return "154FFAC4-61BF-49CE-87C4-4FC415465F68";
				case QualityType2014.ChildCaries:
					return "35814E51-399D-4DA3-AC4F-67EA95140F9B";
				case QualityType2014.Pneumonia:
					return "8719C1EE-81BB-45B1-B05C-2CB6593E12F1";
				case QualityType2014.TobaccoCessation:
					return "F130623A-0505-466B-A2B7-996AC6D9C05A";
				case QualityType2014.Influenza:
					return "34F4027D-C59C-4DC5-834B-89B1E34C219D";
				case QualityType2014.WeightChild_1_1:
				case QualityType2014.WeightChild_1_2:
				case QualityType2014.WeightChild_1_3:
					return "A75126FB-5455-476C-AD34-68AE645535F1";
				case QualityType2014.WeightChild_2_1:
				case QualityType2014.WeightChild_2_2:
				case QualityType2014.WeightChild_2_3:
					return "445A44EE-98BC-4614-B8B6-7A9521CD0794";
				case QualityType2014.WeightChild_3_1:
				case QualityType2014.WeightChild_3_2:
				case QualityType2014.WeightChild_3_3:
					return "AA764699-1039-4E99-817C-6ED6E9E93735";
				case QualityType2014.BloodPressureManage:
					return "B936D6B5-151F-47FB-97D8-A5AB9AB00656";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetEMeasureDenomId(QualityType2014 qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType2014.MedicationsEntered:
					return "E7F35F6D-6963-48F5-8F73-8ECE4B7F8F50";
				case QualityType2014.WeightOver65:
					return "47F98343-B180-4CD6-9622-1D913DE255B2";
				case QualityType2014.WeightAdult:
					return "73F6ACC4-B252-42CA-8C3B-330EC37F1294";
				case QualityType2014.CariesPrevent:
					return "81DE4056-93B1-43ED-BD3E-0A93CCC70828";
				case QualityType2014.CariesPrevent_1:
					return "2C028996-D897-4162-ADBE-8619EDBE4E7D";
				case QualityType2014.CariesPrevent_2:
					return "59140979-7876-4A93-85EB-1276E2ED7584";
				case QualityType2014.CariesPrevent_3:
					return "154FFAC4-61BF-49CE-87C4-4FC415465F68";
				case QualityType2014.ChildCaries:
					return "EE53E909-57C7-4B9E-A751-DED6F4DBD53E";
				case QualityType2014.Pneumonia:
					return "32BF25B7-7448-4B88-8CBC-AEFA213F84ED";
				case QualityType2014.TobaccoCessation:
					return "4A7D841F-0C3A-43DB-8220-ED6B1A971AC9";
				case QualityType2014.Influenza:
					return "2F7CF599-3DD4-43C0-BF34-622DAE9617AE";
				case QualityType2014.WeightChild_1_1:
				case QualityType2014.WeightChild_1_2:
				case QualityType2014.WeightChild_1_3:
					return "2E95A790-B16B-4679-AAC0-B70B46932304";
				case QualityType2014.WeightChild_2_1:
				case QualityType2014.WeightChild_2_2:
				case QualityType2014.WeightChild_2_3:
					return "445A44EE-98BC-4614-B8B6-7A9521CD0794";
				case QualityType2014.WeightChild_3_1:
				case QualityType2014.WeightChild_3_2:
				case QualityType2014.WeightChild_3_3:
					return "AA764699-1039-4E99-817C-6ED6E9E93735";
				case QualityType2014.BloodPressureManage:
					return "2A53CBF0-510A-429D-814D-CBCA4DC0E2A8";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetEMeasureDenexId(QualityType2014 qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType2014.MedicationsEntered:
				case QualityType2014.CariesPrevent:
				case QualityType2014.CariesPrevent_1:
				case QualityType2014.CariesPrevent_2:
				case QualityType2014.CariesPrevent_3:
				case QualityType2014.ChildCaries:
				case QualityType2014.Pneumonia:
				case QualityType2014.TobaccoCessation:
				case QualityType2014.Influenza:
					return "";
				case QualityType2014.WeightOver65:
					return "6f3694a9-6ae2-4fed-923a-198489ab47b8";
				case QualityType2014.WeightAdult:
					return "d98c4592-bdce-4bdf-8466-2339a98ad00b";
				case QualityType2014.WeightChild_1_1:
				case QualityType2014.WeightChild_1_2:
				case QualityType2014.WeightChild_1_3:
					return "6A98DA9A-C352-4A4D-8B69-B2D08619E8A8";
				case QualityType2014.WeightChild_2_1:
				case QualityType2014.WeightChild_2_2:
				case QualityType2014.WeightChild_2_3:
					return "445A44EE-98BC-4614-B8B6-7A9521CD0794";
				case QualityType2014.WeightChild_3_1:
				case QualityType2014.WeightChild_3_2:
				case QualityType2014.WeightChild_3_3:
					return "AA764699-1039-4E99-817C-6ED6E9E93735";
				case QualityType2014.BloodPressureManage:
					return "57F3D1D7-F9FA-437F-A2B9-406A1498E8C0";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetEMeasureDenexcepId(QualityType2014 qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType2014.MedicationsEntered:
					return "171D644F-3B45-4EBA-9A8C-31D3BA193088";
				case QualityType2014.TobaccoCessation:
					return "F8B92E13-AEC3-4B52-B381-E1BD98164F84";
				case QualityType2014.Influenza:
					return "D3C49689-06FB-4702-8079-F09C074B42B7";
				case QualityType2014.WeightOver65:
				case QualityType2014.WeightAdult:
				case QualityType2014.CariesPrevent:
				case QualityType2014.CariesPrevent_1:
				case QualityType2014.CariesPrevent_2:
				case QualityType2014.CariesPrevent_3:
				case QualityType2014.ChildCaries:
				case QualityType2014.Pneumonia:
				case QualityType2014.WeightChild_1_1:
				case QualityType2014.WeightChild_1_2:
				case QualityType2014.WeightChild_1_3:
				case QualityType2014.WeightChild_2_1:
				case QualityType2014.WeightChild_2_2:
				case QualityType2014.WeightChild_2_3:
				case QualityType2014.WeightChild_3_1:
				case QualityType2014.WeightChild_3_2:
				case QualityType2014.WeightChild_3_3:
				case QualityType2014.BloodPressureManage:
					return "";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		private static string GetEMeasureNumerId(QualityType2014 qtype) {
			//No need to check RemotingRole; no call to db.
			switch(qtype) {
				case QualityType2014.MedicationsEntered:
					return "C54E0789-B833-438D-A2A0-63BBB60C13E6";
				case QualityType2014.WeightOver65:
					return "22aab4cb-4452-4ba0-8a45-b340a318092d";
				case QualityType2014.WeightAdult:
					return "875d8e62-44e5-4249-803d-1b36b57dc2da";
				case QualityType2014.CariesPrevent:
					return "25DAB30B-E300-43BE-9558-B76B71B6E3A9";
				case QualityType2014.CariesPrevent_1:
					return "2C028996-D897-4162-ADBE-8619EDBE4E7D";
				case QualityType2014.CariesPrevent_2:
					return "59140979-7876-4A93-85EB-1276E2ED7584";
				case QualityType2014.CariesPrevent_3:
					return "154FFAC4-61BF-49CE-87C4-4FC415465F68";
				case QualityType2014.ChildCaries:
					return "F0D11476-E49B-4C70-B557-F9A5E670248A";
				case QualityType2014.Pneumonia:
					return "43479DAE-01AD-4540-AB02-C02B78D26810";
				case QualityType2014.TobaccoCessation:
					return "FB081A01-4184-43CB-9ADE-0698C1B82082";
				case QualityType2014.Influenza:
					return "9C2D94BD-12B7-4918-9EE5-8C4DEF3AF212";
				case QualityType2014.WeightChild_1_1:
					return "2D775CD0-CAC4-44D1-9185-9E6AB12835FA";
				case QualityType2014.WeightChild_1_2:
					return "36B4858A-821C-40EE-90F1-007A164CD93E";
				case QualityType2014.WeightChild_1_3:
					return "B11C09BA-C864-430D-9DEC-16647793774C";
				case QualityType2014.WeightChild_2_1:
				case QualityType2014.WeightChild_2_2:
				case QualityType2014.WeightChild_2_3:
					return "445A44EE-98BC-4614-B8B6-7A9521CD0794";
				case QualityType2014.WeightChild_3_1:
				case QualityType2014.WeightChild_3_2:
				case QualityType2014.WeightChild_3_3:
					return "AA764699-1039-4E99-817C-6ED6E9E93735";
				case QualityType2014.BloodPressureManage:
					return "5EE417A9-32DB-452F-BAA0-74C5831A4457";
				default:
					throw new ApplicationException("Type not found: "+qtype.ToString());
			}
		}

		///<summary>listQMs will be a list of 9 QualityMeasure objects for the selected CQMs.
		///Those objects will have every encounter, procedure, problem, etc. required for creating the category I and category III QRDA documents.
		///Category I is the patient level documents, and will be one document for every patient in the initial patient population for the specific measure.
		///Each encounter, procedure, etc. may appear in the Category I document multiple times if, for instance, the encounter falls into more than one value set
		///and qualifies the patient for inclusion in multiple measures.
		///The Category I files will be zipped per measure, so a patient's file be in the zip for every measure to which they belong.
		///The Category III document contains the aggregate information for each of the selected measures.</summary>
		public static void GenerateQRDA(List<QualityMeasure> listQMs,long provNum,DateTime dateStart,DateTime dateEnd,string folderRoot,long provNumLegal) {
			//Set global variables for use by the GenerateCatOne and GenerateCatThree functions
			if(listQMs==null || listQMs.Count==0) {//this should never happen
				throw new ApplicationException("No measures to report on.");
			}
			_provNumLegal=provNumLegal;
			string strErrors=ValidateSettings();
			if(strErrors!="") {
				throw new ApplicationException(strErrors);
			}
			_provOutQrda=Providers.GetProv(provNum);
			if(_provOutQrda==null) {
				throw new ApplicationException("Invalid provider selected.");
			}
			_strOIDInternalRoot=OIDInternals.GetForType(IdentifierType.Root).IDRoot;
			if(_strOIDInternalRoot=="") {
				throw new ApplicationException("Internal OID root is missing.",new Exception("true"));//use inner exception to load the EHR OIDs Internal form from CQM form
			}
			_strOIDInternalCQMRoot=OIDInternals.GetForType(IdentifierType.CqmItem).IDRoot;
			if(_strOIDInternalCQMRoot=="") {
				throw new ApplicationException("Internal CQM item OID root is missing.",new Exception("true"));
			}
			_strOIDInternalProvRoot=OIDInternals.GetForType(IdentifierType.Provider).IDRoot;
			if(_strOIDInternalProvRoot=="") {
				throw new ApplicationException("Internal Provider OID root is missing.",new Exception("true"));
			}
			_strOIDInternalPatRoot=OIDInternals.GetForType(IdentifierType.Patient).IDRoot;
			if(_strOIDInternalPatRoot=="") {
				throw new ApplicationException("Internal Patient OID root is missing.",new Exception("true"));
			}
			_hashQrdaGuids=new HashSet<string>();//The GUIDs are only used to uniquely identify the documents themselves, for other IDs we are using the OIDInternal root and extension
			//create a list of all CQM indexes that are considered stratifications or additional populations of a measure
			//these will be skipped for certain circumstances, like when listing all of the eMeasures or creating certain sections of the QRDA
			_listExtraPopIndxs=new List<int>();
			_listExtraPopIndxs.Add((int)QualityType2014.CariesPrevent_1);
			_listExtraPopIndxs.Add((int)QualityType2014.CariesPrevent_2);
			_listExtraPopIndxs.Add((int)QualityType2014.CariesPrevent_3);
			_listExtraPopIndxs.Add((int)QualityType2014.WeightAdult);
			_listExtraPopIndxs.Add((int)QualityType2014.WeightChild_1_2);
			_listExtraPopIndxs.Add((int)QualityType2014.WeightChild_1_3);
			_listExtraPopIndxs.Add((int)QualityType2014.WeightChild_2_1);
			_listExtraPopIndxs.Add((int)QualityType2014.WeightChild_2_2);
			_listExtraPopIndxs.Add((int)QualityType2014.WeightChild_2_3);
			_listExtraPopIndxs.Add((int)QualityType2014.WeightChild_3_1);
			_listExtraPopIndxs.Add((int)QualityType2014.WeightChild_3_2);
			_listExtraPopIndxs.Add((int)QualityType2014.WeightChild_3_3);
			GenerateQRDACatOne(listQMs,dateStart,dateEnd,folderRoot);
			GenerateQRDACatThree(listQMs,dateStart,dateEnd,folderRoot);
		}

		///<summary>This method generates the entire Category III aggregate information document and places it in the path supplied in folderRoot\QRDA_Category_III.xml.</summary>
		private static void GenerateQRDACatThree(List<QualityMeasure> listQMs,DateTime dateStart,DateTime dateEnd,string folderRoot) {
			XmlWriterSettings xmlSettings=new XmlWriterSettings();
			xmlSettings.Encoding=Encoding.UTF8;
			xmlSettings.OmitXmlDeclaration=true;
			xmlSettings.Indent=true;
			xmlSettings.IndentChars="\t";
			using(_w=XmlWriter.Create(folderRoot+"\\QRDA_Category_III.xml",xmlSettings)) {
				//Begin Clinical Document
				_w.WriteProcessingInstruction("xml-stylesheet","type=\"text/xsl\" href=\"qrda.xsl\"");
				_w.WriteWhitespace("\r\n");
				_w.WriteStartElement("ClinicalDocument","urn:hl7-org:v3");
				_w.WriteAttributeString("xmlns","xsi",null,"http://www.w3.org/2001/XMLSchema-instance");
				_w.WriteAttributeString("xsi","schemaLocation",null,"urn:./CDA.xsd");
				_w.WriteAttributeString("xmlns","voc",null,"urn:hl7-org:v3/voc");
				#region QRDA III Header
				_w.WriteComment("QRDA III Header");
				StartAndEnd("realmCode","code","US");
				StartAndEnd("typeId","root","2.16.840.1.113883.1.3","extension","POCD_HD000040");//template id to assert use of the CDA standard
				_w.WriteComment("QRDA Category III Release 1 Template");
				TemplateId("2.16.840.1.113883.10.20.27.1.1");
				_w.WriteComment("This is the globally unique identifier for this QRDA III document");
				Guid();
				_w.WriteComment("QRDA III document type code");
				StartAndEnd("code","code","55184-6","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","Quality Reporting Document Architecture Calculated Summary Report");
				_w.WriteElementString("title","QRDA Calculated Summary Report for CMS Measures 68, 69, 74, 75, 127, 138, 147, 155, and 165");
				_w.WriteComment("This is the document creation time");
				TimeElement("effectiveTime",DateTime.Now);
				StartAndEnd("confidentialityCode","code","N","codeSystem","2.16.840.1.113883.5.25");//Fixed value.  Confidentiality Code System.  Codes: N=(Normal), R=(Restricted),V=(Very Restricted)
				StartAndEnd("languageCode","code","en-US");
				#region recordTarget
				_w.WriteComment("Record Target and ID - but ID is nulled to NA. This is an aggregate summary report. Therefore CDA's required patient identifier is nulled.");
				Start("recordTarget");
				Start("patientRole");
				StartAndEnd("id","nullFlavor","NA");
				End("patientRole");
				End("recordTarget");
				#endregion recordTarget
				#region comments
				//The author element represents the creator of the clinical document.  The author may be a device, or a person.
				//Participant Scenarios in a QRDA Category III Document (section 2.3, page 29)
				//Several possible scenarios given, the first sounds like it applies to us
				//Scenario - QRDA is wholly constructed automatically by device
				//Author - Device
				//Custodian - Organization that owns and reports the data (e.g. hospital, dental practice)
				//Legal Authenticator - A designated person in the organization (may be assigned to the report automatically)
				//We will generate a device author element, a practice custodian element, and a Legal Authenticator element using the practice default provider
				#endregion comments
				#region author
				Start("author");
				TimeElement("time",DateTime.Now);
				Start("assignedAuthor");
				StartAndEnd("id","root","2.16.840.1.113883.3.4337","assigningAuthorityName","HL7 OID Registry");
				Start("assignedAuthoringDevice");
				_w.WriteElementString("softwareName","Open Dental version "+PrefC.GetString(PrefName.ProgramVersion));
				End("assignedAuthoringDevice");
				Start("representedOrganization");
				_w.WriteElementString("name",PrefC.GetString(PrefName.PracticeTitle));//Validated
				End("representedOrganizaion");
				End("assignedAuthor");
				End("author");
				#endregion author
				#region custodian
				//Represents the organization in charge of maintaining the document.
				//The custodian is the steward that is entrusted with the care of the document. Every CDA document has exactly one custodian.
				Start("custodian");
				Start("assignedCustodian");
				Start("representedCustodianOrganization");
				StartAndEnd("id","root",_strOIDInternalRoot);//This is the root assigned to the practice, based on the OD root 2.16.840.1.113883.3.4337
				_w.WriteElementString("name",PrefC.GetString(PrefName.PracticeTitle));//Validated
				End("representedCustodianOrganization");
				End("assignedCustodian");
				End("custodian");
				#endregion custodian
				#region legalAuthenticator
				//This element identifies the single person legally responsible for the document and must be present if the document has been legally authenticated.
				Start("legalAuthenticator");
				TimeElement("time",DateTime.Now);
				StartAndEnd("signatureCode","code","S");
				Start("assignedEntity");
				Provider provLegal=Providers.GetProv(_provNumLegal);
				StartAndEnd("id","root","2.16.840.1.113883.4.6","extension",provLegal.NationalProvID,"assigningAuthorityName","NPI");//Validated NPI
				Start("representedOrganization");
				StartAndEnd("id","root",_strOIDInternalRoot);//This is the root assigned to the practice, based on the OD root 2.16.840.1.113883.3.4337
				_w.WriteElementString("name",PrefC.GetString(PrefName.PracticeTitle));//Validated
				End("representedOrganization");
				End("assignedEntity");
				End("legalAuthenticator");
				#endregion legalAuthenticator
				#region documentationOf
				//The documentationOf service event can contain identifiers for all of the (one or more) providers involved, using the serviceEvent/performer elements.
				//A serviceEvent/performer element must be present for each performer reporting data to a quality organization.
				Start("documentationOf","typeCode","DOC");
				Start("serviceEvent","classCode","PCPR");//PCPR is HL7ActClass code for Care Provision
				_w.WriteComment("Care Provision");
				Start("effectiveTime");
				DateElement("low",dateStart);
				DateElement("high",dateEnd);
				End("effectiveTime");
				Start("performer","typeCode","PRF");
				Start("time");
				StartAndEnd("low","value",dateStart.ToString("yyyyMMdd")+"000000");
				StartAndEnd("high","value",dateEnd.ToString("yyyyMMdd")+"235959");
				End("time");
				Start("assignedEntity");
				if(_provOutQrda.NationalProvID!="") {
					_w.WriteComment("This is the provider NPI");
					StartAndEnd("id","root","2.16.840.1.113883.4.6","extension",_provOutQrda.NationalProvID);
				}
				if(_provOutQrda.UsingTIN && _provOutQrda.SSN!="") {
					_w.WriteComment("This is the provider TIN");
					StartAndEnd("id","root","2.16.840.1.113883.4.2","extension",_provOutQrda.SSN);
				}
				_w.WriteComment("This is the practice OID provider root and Open Dental assigned ProvNum extension");
				StartAndEnd("id","root",_strOIDInternalProvRoot,"extension",_provOutQrda.ProvNum.ToString());
				Start("representedOrganization");
				//we don't currently have an organization level TIN or an organization Facility CMS Certification Number (CCN)
				//both id's are identified as "SHOULD" elements.  We will include the practice name
				_w.WriteElementString("name",PrefC.GetString(PrefName.PracticeTitle));//Validated
				End("representedOrganization");
				End("assignedEntity");
				End("performer");
				End("serviceEvent");
				End("documentationOf");
				#endregion participant
				#endregion QRDA III Header
				#region QRDA III Body
				Start("component");
				Start("structuredBody");
				#region reportingParameters component
				_w.WriteComment("Reporting Parameters Component");
				Start("component");
				Start("section");
				_w.WriteComment("Reporting Parameters Section Template");
				TemplateId("2.16.840.1.113883.10.20.17.2.1");
				_w.WriteComment("QRDA Category III Reporting Parameters Section Template");
				TemplateId("2.16.840.1.113883.10.20.27.2.2");
				StartAndEnd("code","code","55187-9","displayName","Reporting Parameters","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
				_w.WriteElementString("title","Reporting Parameters");
				Start("text");
				Start("list");
				_w.WriteElementString("item","Reporting period: "+dateStart.ToString("MMMM dd, yyyy")+" 00:00 - "+dateEnd.ToString("MMMM dd, yyyy")+" 23:59");
				End("list");
				End("text");
				Start("entry","typeCode","DRIV");
				Start("act","classCode","ACT","moodCode","EVN");
				_w.WriteComment("Reporting Parameters Act Template");
				TemplateId("2.16.840.1.113883.10.20.17.3.8");
				StartAndEnd("code","code","252116004","displayName","Observation Parameters","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
				Start("effectiveTime");
				_w.WriteComment("The first day of the reporting period");
				DateElement("low",dateStart);
				_w.WriteComment("The last day of the reporting period");
				DateElement("high",dateEnd);
				End("effectiveTime");
				End("act");
				End("entry");
				End("section");
				End("component");
				#endregion reportingParameters component
				#region measure component
				_w.WriteComment("Measures Component");
				Start("component");
				Start("section");
				//structuredBody[component[section(reportingParameters)]][component2[section(measureSection)]]]
				_w.WriteComment("Measure Section Template");
				TemplateId("2.16.840.1.113883.10.20.24.2.2");
				_w.WriteComment("QRDA Category III Measure Section Template");
				TemplateId("2.16.840.1.113883.10.20.27.2.1");
				StartAndEnd("code","code","55186-1","displayName","Measure Section","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
				_w.WriteElementString("title","Measure Section");
				#region TEXT SECTION
				Start("text");
				List<Dictionary<string,int>> listPopCountDicts=new List<Dictionary<string,int>>();
				//get the indexes of the three stratifications
				int s1Indx=(int)QualityType2014.CariesPrevent_1;
				int s2Indx=(int)QualityType2014.CariesPrevent_2;
				int s3Indx=(int)QualityType2014.CariesPrevent_3;
				//get the index of the WeightAdult (population 2)
				int pop2Indx=(int)QualityType2014.WeightAdult;
				//get the indexes of the additional two numerators and 2 stratifications
				int numer2=(int)QualityType2014.WeightChild_1_2;
				int numer3=(int)QualityType2014.WeightChild_1_3;
				int s1numer1Indx=(int)QualityType2014.WeightChild_2_1;
				int s1numer2Indx=(int)QualityType2014.WeightChild_2_2;
				int s1numer3Indx=(int)QualityType2014.WeightChild_2_3;
				int s2numer1Indx=(int)QualityType2014.WeightChild_3_1;
				int s2numer2Indx=(int)QualityType2014.WeightChild_3_2;
				int s2numer3Indx=(int)QualityType2014.WeightChild_3_3;
				int numPops;//if set to two or three, this will generate additional sets of population data (measure 69 has two populations, measure 155 has 3 numerators)
				for(int i=0;i<listQMs.Count;i++) {
					//_listExtraPopIndxs will contain all of the Type2014 enum types that are stratifications or extra populations of other measures, so skip them here
					if(_listExtraPopIndxs.Contains(i)) {
						continue;
					}
					numPops=1;
					switch(i) {
						case (int)QualityType2014.WeightOver65:
							//this wil create 2 populations, one for weight ages 18-64 and one for weight 65+
							numPops=2;
							break;
						case (int)QualityType2014.WeightChild_1_1:
							//this will create 3 data sections, one for each numerator
							//We will have Numerator 1 with ipp,denom,except,exclus,numer data, Numerator 2 data, and Numerator 3 data, each with their own stratifications
							numPops=3;
							break;
					}
					if(listQMs[i].Type2014==QualityType2014.MedicationsEntered) {
						listPopCountDicts=FillDictPopCounts(listQMs[i].ListEhrPats,listQMs[i].DictPatNumListEncounters);
					}
					else {
						listPopCountDicts=FillDictPopCounts(listQMs[i].ListEhrPats,null);
					}
					Start("table","border","1","width","100%");
					Start("thead");
					Start("tr");
					_w.WriteElementString("th","eMeasure Identifier (MAT)");
					_w.WriteElementString("th","eMeasure Title");
					_w.WriteElementString("th","Version neutral identifier");
					_w.WriteElementString("th","eMeasure Version Number");
					_w.WriteElementString("th","Version specific identifier");
					End("tr");
					End("thead");
					Start("tbody");
					MeasureTextTableRow(listQMs[i].eMeasureTitle,listQMs[i].eMeasureNum,listQMs[i].eMeasureVNeutralId,listQMs[i].eMeasureVersion,listQMs[i].eMeasureVSpecificId);
					End("tbody");
					End("table");
					for(int p=0;p<numPops;p++) {//usually only runs 1 loop, measure 69 has two populations, and measure 155 has 3 numerators, in which case it will run more than once
						int measureIndx=i;
						if(numPops==2) {
							if(p==0) {
								StartAndEnd("br");
							}
							Start("content","styleCode","Bold");
							_w.WriteString("Population "+(p+1).ToString()+":");
							End("content");
							if(p>0) {//fill stratification data with supplemental data from other population (only used by measure 69)
								measureIndx=pop2Indx;
								listPopCountDicts=FillDictPopCounts(listQMs[measureIndx].ListEhrPats,null);
							}
						}
						else if(numPops==3) {
							if(p==0) {
								StartAndEnd("br");
							}
							Start("content","styleCode","Bold");
							_w.WriteString("Numerator "+(p+1).ToString()+":");
							End("content");
							if(p==1) {//second iteration, fill dictionaries with numerator 2 data for stratifications
								measureIndx=numer2;
								listPopCountDicts=FillDictPopCounts(listQMs[measureIndx].ListEhrPats,null);
							}
							if(p==2) {//third iteration, fill dictionaries with numerator 3 data for stratifications
								measureIndx=numer3;
								listPopCountDicts=FillDictPopCounts(listQMs[measureIndx].ListEhrPats,null);
							}
						}
						Start("list");
						#region Performeance and Reporting Rate TEXT VERSION
						Start("item");
						Start("content","styleCode","Bold");
						_w.WriteString("Performance Rate");
						End("content");
						if(listQMs[measureIndx].ListEhrPats.Count==0) {
							_w.WriteString(": NA");
						}
						else {
							_w.WriteString(": "+listQMs[measureIndx].PerformanceRate.ToString("0.00")+"%");
						}
						End("item");
						Start("item");
						Start("content","styleCode","Bold");
						_w.WriteString("Reporting Rate");
						End("content");
						if(listQMs[measureIndx].ListEhrPats.Count==0) {
							_w.WriteString(": NA");
						}
						else {
							_w.WriteString(": "+listQMs[measureIndx].ReportingRate.ToString("0.00")+"%");
						}
						End("item");
						#endregion Performeance and Reporting Rate TEXT VERSION
						#region IPP TEXT VERSION
						//Start Initial Patient Population text
						Start("item");
						Start("content","styleCode","Bold");
						_w.WriteString("Initial Patient Population");
						End("content");
						_w.WriteString(": "+listPopCountDicts[0]["All"].ToString());
						if(listPopCountDicts[0]["All"]>0) {
							Start("list");
							#region Stratification
							if(i==(int)QualityType2014.CariesPrevent || i==(int)QualityType2014.WeightChild_1_1) {
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString("Reporting Stratum 1");
								End("content");
								if(p==0) {
									if(i==(int)QualityType2014.CariesPrevent) {
										_w.WriteString(": "+listQMs[s1Indx].ListEhrPats.Count.ToString());
									}
									else {
										_w.WriteString(": "+listQMs[s1numer1Indx].ListEhrPats.Count.ToString());
									}
								}
								else if(p==1) {
									_w.WriteString(": "+listQMs[s1numer2Indx].ListEhrPats.Count.ToString());
								}
								else if(p==2) {
									_w.WriteString(": "+listQMs[s1numer3Indx].ListEhrPats.Count.ToString());
								}
								End("item");
							}
							if(i==(int)QualityType2014.CariesPrevent || i==(int)QualityType2014.WeightChild_1_1) {
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString("Reporting Stratum 2");
								End("content");
								if(p==0) {
									if(i==(int)QualityType2014.CariesPrevent) {
										_w.WriteString(": "+listQMs[s2Indx].ListEhrPats.Count.ToString());
									}
									else {
										_w.WriteString(": "+listQMs[s2numer1Indx].ListEhrPats.Count.ToString());
									}
								}
								else if(p==1) {
									_w.WriteString(": "+listQMs[s2numer2Indx].ListEhrPats.Count.ToString());
								}
								else if(p==2) {
									_w.WriteString(": "+listQMs[s2numer3Indx].ListEhrPats.Count.ToString());
								}
								End("item");
							}
							if(p==0 && i==(int)QualityType2014.CariesPrevent) {
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString("Reporting Stratum 3");
								End("content");
								_w.WriteString(": "+listQMs[s3Indx].ListEhrPats.Count.ToString());
								End("item");
							}
							#endregion
							foreach(KeyValuePair<string,int> kvpair in listPopCountDicts[0]) {
								if(kvpair.Value==0 || kvpair.Key=="All") {
									continue;
								}
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString(GetSupplementDataPrintString(kvpair.Key));
								End("content");
								_w.WriteString(": "+kvpair.Value.ToString());
								End("item");
							}
							End("list");
						}
						End("item");
						#endregion IPP TEXT VERSION
						#region DENOM TEXT VERSION
						//Start Denominator text
						Start("item");
						Start("content","styleCode","Bold");
						_w.WriteString("Denominator");
						End("content");
						_w.WriteString(": "+listPopCountDicts[1]["All"]);
						if(listPopCountDicts[1]["All"]>0) {
							Start("list");
							#region Stratification
							if(i==(int)QualityType2014.CariesPrevent || i==(int)QualityType2014.WeightChild_1_1) {
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString("Reporting Stratum 1");
								End("content");
								if(p==0) {
									if(i==(int)QualityType2014.CariesPrevent) {
										_w.WriteString(": "+listQMs[s1Indx].Denominator.ToString());
									}
									else {
										_w.WriteString(": "+listQMs[s1numer1Indx].Denominator.ToString());
									}
								}
								else if(p==1) {
									_w.WriteString(": "+listQMs[s1numer2Indx].Denominator.ToString());
								}
								else if(p==2) {
									_w.WriteString(": "+listQMs[s1numer3Indx].Denominator.ToString());
								}
								End("item");
							}
							if(i==(int)QualityType2014.CariesPrevent || i==(int)QualityType2014.WeightChild_1_1) {
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString("Reporting Stratum 2");
								End("content");
								if(p==0) {
									if(i==(int)QualityType2014.CariesPrevent) {
										_w.WriteString(": "+listQMs[s2Indx].Denominator.ToString());
									}
									else {
										_w.WriteString(": "+listQMs[s2numer1Indx].Denominator.ToString());
									}
								}
								else if(p==1) {
									_w.WriteString(": "+listQMs[s2numer2Indx].Denominator.ToString());
								}
								else if(p==2) {
									_w.WriteString(": "+listQMs[s2numer3Indx].Denominator.ToString());
								}
								End("item");
							}
							if(p==0 && i==(int)QualityType2014.CariesPrevent) {
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString("Reporting Stratum 3");
								End("content");
								_w.WriteString(": "+listQMs[s3Indx].Denominator.ToString());
								End("item");
							}
							#endregion
							foreach(KeyValuePair<string,int> kvpair in listPopCountDicts[1]) {
								if(kvpair.Value==0 || kvpair.Key=="All") {
									continue;
								}
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString(GetSupplementDataPrintString(kvpair.Key));
								End("content");
								_w.WriteString(": "+kvpair.Value.ToString());
								End("item");
							}
							End("list");
						}
						End("item");
						#endregion DENOM TEXT VERSION
						#region DENEXCL TEXT VERSION
						//Start Denominator Exclusion text
						Start("item");
						Start("content","styleCode","Bold");
						_w.WriteString("Denominator Exclusion");
						End("content");
						_w.WriteString(": "+listPopCountDicts[2]["All"].ToString());
						if(listPopCountDicts[2]["All"]>0) {
							Start("list");
							#region Stratification
							if(i==(int)QualityType2014.CariesPrevent || i==(int)QualityType2014.WeightChild_1_1) {
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString("Reporting Stratum 1");
								End("content");
								if(p==0) {
									if(i==(int)QualityType2014.CariesPrevent) {
										_w.WriteString(": "+listQMs[s1Indx].Exclusions.ToString());
									}
									else {
										_w.WriteString(": "+listQMs[s1numer1Indx].Exclusions.ToString());
									}
								}
								else if(p==1) {
									_w.WriteString(": "+listQMs[s1numer2Indx].Exclusions.ToString());
								}
								else if(p==2) {
									_w.WriteString(": "+listQMs[s1numer3Indx].Exclusions.ToString());
								}
								End("item");
							}
							if(i==(int)QualityType2014.CariesPrevent || i==(int)QualityType2014.WeightChild_1_1) {
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString("Reporting Stratum 2");
								End("content");
								if(p==0) {
									if(i==(int)QualityType2014.CariesPrevent) {
										_w.WriteString(": "+listQMs[s2Indx].Exclusions.ToString());
									}
									else {
										_w.WriteString(": "+listQMs[s2numer1Indx].Exclusions.ToString());
									}
								}
								else if(p==1) {
									_w.WriteString(": "+listQMs[s2numer2Indx].Exclusions.ToString());
								}
								else if(p==2) {
									_w.WriteString(": "+listQMs[s2numer3Indx].Exclusions.ToString());
								}
								End("item");
							}
							if(p==0 && i==(int)QualityType2014.CariesPrevent) {
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString("Reporting Stratum 3");
								End("content");
								_w.WriteString(": "+listQMs[s3Indx].Exclusions.ToString());
								End("item");
							}
							#endregion
							foreach(KeyValuePair<string,int> kvpair in listPopCountDicts[2]) {
								if(kvpair.Value==0 || kvpair.Key=="All") {
									continue;
								}
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString(GetSupplementDataPrintString(kvpair.Key));
								End("content");
								_w.WriteString(": "+kvpair.Value.ToString());
								End("item");
							}
							End("list");
						}
						End("item");
						#endregion DENEXCL TEXT VERSION
						#region NUMER TEXT VERSION
						//Start Numerator text
						Start("item");
						Start("content","styleCode","Bold");
						_w.WriteString("Numerator");
						End("content");
						_w.WriteString(": "+listPopCountDicts[3]["All"].ToString());
						if(listPopCountDicts[3]["All"]>0) {
							Start("list");
							#region Stratification
							if(i==(int)QualityType2014.CariesPrevent || i==(int)QualityType2014.WeightChild_1_1) {
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString("Reporting Stratum 1");
								End("content");
								if(p==0) {									
									if(i==(int)QualityType2014.CariesPrevent) {
										_w.WriteString(": "+listQMs[s1Indx].Numerator.ToString());
									}
									else {
										_w.WriteString(": "+listQMs[s1numer1Indx].Numerator.ToString());
									}
								}
								else if(p==1) {
									_w.WriteString(": "+listQMs[s1numer2Indx].Numerator.ToString());
								}
								else if(p==2) {
									_w.WriteString(": "+listQMs[s1numer3Indx].Numerator.ToString());
								}
								End("item");
							}
							if(i==(int)QualityType2014.CariesPrevent || i==(int)QualityType2014.WeightChild_1_1) {
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString("Reporting Stratum 2");
								End("content");
								if(p==0) {									
									if(i==(int)QualityType2014.CariesPrevent) {
										_w.WriteString(": "+listQMs[s2Indx].Numerator.ToString());
									}
									else {
										_w.WriteString(": "+listQMs[s2numer1Indx].Numerator.ToString());
									}
								}
								else if(p==1) {
									_w.WriteString(": "+listQMs[s2numer2Indx].Numerator.ToString());
								}
								else if(p==2) {
									_w.WriteString(": "+listQMs[s2numer3Indx].Numerator.ToString());
								}
								End("item");
							}
							if(p==0 && i==(int)QualityType2014.CariesPrevent) {
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString("Reporting Stratum 3");
								End("content");
								_w.WriteString(": "+listQMs[s3Indx].Numerator.ToString());
								End("item");
							}
							#endregion
							foreach(KeyValuePair<string,int> kvpair in listPopCountDicts[3]) {
								if(kvpair.Value==0 || kvpair.Key=="All") {
									continue;
								}
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString(GetSupplementDataPrintString(kvpair.Key));
								End("content");
								_w.WriteString(": "+kvpair.Value.ToString());
								End("item");
							}
							End("list");
						}
						End("item");
						#endregion NUMER TEXT VERSION
						#region DENEXCEP TEXT VERSION
						//Start Denominator Exception text
						Start("item");
						Start("content","styleCode","Bold");
						_w.WriteString("Denominator Exception");
						End("content");
						_w.WriteString(": "+listPopCountDicts[4]["All"].ToString());
						if(listPopCountDicts[4]["All"]>0) {
							Start("list");
							#region Stratification
							if(i==(int)QualityType2014.CariesPrevent || i==(int)QualityType2014.WeightChild_1_1) {
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString("Reporting Stratum 1");
								End("content");
								if(p==0) {
									if(i==(int)QualityType2014.CariesPrevent) {
										_w.WriteString(": "+listQMs[s1Indx].Exceptions.ToString());
									}
									else {
										_w.WriteString(": "+listQMs[s1numer1Indx].Exceptions.ToString());
									}
								}
								else if(p==1) {
									_w.WriteString(": "+listQMs[s1numer2Indx].Exceptions.ToString());
								}
								else if(p==2) {
									_w.WriteString(": "+listQMs[s1numer3Indx].Exceptions.ToString());
								}
								End("item");
							}
							if(i==(int)QualityType2014.CariesPrevent || i==(int)QualityType2014.WeightChild_1_1) {
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString("Reporting Stratum 2");
								End("content");
								if(p==0) {
									if(i==(int)QualityType2014.CariesPrevent) {
										_w.WriteString(": "+listQMs[s2Indx].Exceptions.ToString());
									}
									else {
										_w.WriteString(": "+listQMs[s2numer1Indx].Exceptions.ToString());
									}
								}
								else if(p==1) {
									_w.WriteString(": "+listQMs[s2numer2Indx].Exceptions.ToString());
								}
								else if(p==2) {
									_w.WriteString(": "+listQMs[s2numer3Indx].Exceptions.ToString());
								}
								End("item");
							}
							if(p==0 && i==(int)QualityType2014.CariesPrevent) {
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString("Reporting Stratum 3");
								End("content");
								_w.WriteString(": "+listQMs[s3Indx].Exceptions.ToString());
								End("item");
							}
							#endregion
							foreach(KeyValuePair<string,int> kvpair in listPopCountDicts[4]) {
								if(kvpair.Value==0 || kvpair.Key=="All") {
									continue;
								}
								Start("item");
								Start("content","styleCode","Bold");
								_w.WriteString(GetSupplementDataPrintString(kvpair.Key));
								End("content");
								_w.WriteString(": "+kvpair.Value.ToString());
								End("item");
							}
							End("list");
						}
						End("item");
						#endregion DENEXCEP TEXT VERSION
						End("list");
					}
				}
				End("text");
				#endregion TEXT SECTION
				#region ENTRY SECTION
				Dictionary<string,Cdcrec> dictRaces=Cdcrecs.GetAll().ToDictionary(x => x.CdcrecCode,x => x);//Used to find HierarchicalCodes and Descriptions
				for(int i=0;i<listQMs.Count;i++) {
					//_listExtraPopIndxs will contain all of the Type2014 enum types that are stratifications or extra populations of other measures, so skip them here
					if(_listExtraPopIndxs.Contains(i)) {
						continue;
					}
					numPops=1;
					switch(i) {
						case (int)QualityType2014.WeightOver65:
							//this wil create 2 populations, one for weight ages 18-64 and one for weight 65+
							numPops=2;
							break;
						case (int)QualityType2014.WeightChild_1_1:
							//this will create 3 data sections, one for each numerator
							//We will have Numerator 1 with ipp,denom,except,exclus,numer data, Numerator 2 data, and Numerator 3 data, each with their own stratifications
							numPops=3;
							break;
					}
					if(listQMs[i].Type2014==QualityType2014.MedicationsEntered) {
						listPopCountDicts=FillDictPopCounts(listQMs[i].ListEhrPats,listQMs[i].DictPatNumListEncounters);
					}
					else {
						listPopCountDicts=FillDictPopCounts(listQMs[i].ListEhrPats,null);
					}
					_w.WriteComment("***************PROPORTION MEASURE ENTRIES (1 entry per population per measure)***************");
					for(int p=0;p<numPops;p++) {//usually only runs 1 loop, measure 69 has two populations, and measure 155 has 3 numerators, in which case it will run more than once
						int measureIndx=i;
						if(numPops==2 && p>0) {//fill stratification data with supplemental data from other population
							measureIndx=pop2Indx;
							listPopCountDicts=FillDictPopCounts(listQMs[measureIndx].ListEhrPats,null);
						}
						if(numPops==3 && p>0) {//only 3 populations for measure 155, Numerators 1-3
							if(p==1) {//second iteration, fill dictionaries with numerator 2 data for stratifications
								measureIndx=numer2;
								listPopCountDicts=FillDictPopCounts(listQMs[measureIndx].ListEhrPats,null);
							}
							if(p==2) {//third iteration, fill dictionaries with numerator 3 data for stratifications
								measureIndx=numer3;
								listPopCountDicts=FillDictPopCounts(listQMs[measureIndx].ListEhrPats,null);
							}
						}
						Start("entry");
						#region Measure Reference
						//start of Measure Reference and Results entries (measureSection)section[entry 1..*]
						Start("organizer","classCode","CLUSTER","moodCode","EVN");
						_w.WriteComment("Measure Reference Template");
						TemplateId("2.16.840.1.113883.10.20.24.3.98");
						_w.WriteComment("Measure Reference and Results Template");
						TemplateId("2.16.840.1.113883.10.20.27.3.1");
						StartAndEnd("statusCode","code","completed");
						Start("reference","typeCode","REFR");
						Start("externalDocument","classCode","DOC","moodCode","EVN");
						StartAndEnd("id","root",listQMs[measureIndx].eMeasureVSpecificId);//version specific id
						StartAndEnd("code","code","57024-2","displayName","Health Quality Measure Document","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
						_w.WriteElementString("text",listQMs[measureIndx].eMeasureTitle);
						StartAndEnd("setId","root",listQMs[measureIndx].eMeasureVNeutralId);//version neutral id
						StartAndEnd("versionNumber","value",listQMs[measureIndx].eMeasureVersion);
						End("externalDocument");
						End("reference");
						Start("reference","typeCode","REFR");
						Start("externalObservation");
						StartAndEnd("id","root",listQMs[measureIndx].eMeasureSetId);
						StartAndEnd("code","code","55185-3","displayName","measure set","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
						if(listQMs[measureIndx].Type2014==QualityType2014.MedicationsEntered) {//Measure 68 is the only one with a title that is not "None" or "NA"
							_w.WriteElementString("text","CLINICAL QUALITY MEASURE SET 2014");
						}
						else if(listQMs[measureIndx].Type2014==QualityType2014.CariesPrevent || listQMs[measureIndx].Type2014==QualityType2014.ChildCaries) {
							_w.WriteElementString("text","Not Applicable");
						}
						else {
							_w.WriteElementString("text","None");
						}
						End("externalObservation");
						End("reference");
						#endregion Measure Reference
						#region Performance Rate Component
						_w.WriteComment("***************Performance Rate 1 per entry (entry=Measure or Population within a measure)***************");
						Start("component");
						//Performance rate=Numerator/(Denominator-Exclusions-Exceptions)
						Start("observation","classCode","OBS","moodCode","EVN");
						_w.WriteComment("Performance Rate for Proportion Measure Template");
						TemplateId("2.16.840.1.113883.10.20.27.3.14");
						StartAndEnd("code","code","72510-1","displayName","Performance Rate","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
						StartAndEnd("statusCode","code","completed");
						Start("value");
						_w.WriteAttributeString("xsi","type",null,"REAL");
						if(listQMs[measureIndx].PerformanceRate==0) {
							Attribs("nullFlavor","NA");//if no patients in denominator, then performance rate is null
						}
						else {
							Attribs("value",listQMs[measureIndx].PerformanceRate.ToString("0.00"));
						}
						End("value");
						End("observation");
						End("component");
						#endregion Performance Rate Component
						#region Reporting Rate Component
						_w.WriteComment("***************Reporting Rate 1 per entry (entry=Measure or Population within a measure)***************");
						Start("component");
						//Reporting rate=(Numerator+Exclusions+Exceptions)/Denominator
						Start("observation","classCode","OBS","moodCode","EVN");
						_w.WriteComment("Reporting Rate for Proportion Measure Template");
						TemplateId("2.16.840.1.113883.10.20.27.3.15");
						StartAndEnd("code","code","72509-3","displayName","Reporting Rate","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
						StartAndEnd("statusCode","code","completed");
						Start("value");
						_w.WriteAttributeString("xsi","type",null,"REAL");
						if(listQMs[measureIndx].PerformanceRate==0) {
							Attribs("nullFlavor","NA");//if no patients in denominator, then reporting rate is null
						}
						else {
							Attribs("value",listQMs[measureIndx].ReportingRate.ToString("0.00"));
						}
						End("value");
						End("observation");
						End("component");
						#endregion Reporting Rate Component
						//***************************************MEASURE DATA COMPONENTS*******************************
						_w.WriteComment("***************Measure Data component entries (1..*) represent aggregate counts IPP,DENOM,DENEX,DENEXCEP,NUMER***************");
						#region Measure Data
						for(int j=0;j<listPopCountDicts.Count;j++) {
							switch(j) {
								case 0:
									_w.WriteComment("Initial Patient Population component");
									break;
								case 1:
									_w.WriteComment("Denominator component");
									break;
								case 2:
									if(listQMs[measureIndx].eMeasureDenexId=="") {//if no exclusions defined for this measure, continue
										continue;
									}
									_w.WriteComment("Denominator Exclusion component");
									break;
								case 3:
									_w.WriteComment("Numerator component");
									break;
								case 4:
									if(listQMs[measureIndx].eMeasureDenexcepId=="") {//if no exceptions defined for this measure, continue
										continue;
									}
									_w.WriteComment("Denominator Exception component");
									break;
								default:
									throw new ApplicationException("Error in creating QRDA Category III in Measure Data Components section.");
							}
							Start("component");
							Start("observation","classCode","OBS","moodCode","EVN");
							_w.WriteComment("Measure Data Template");
							TemplateId("2.16.840.1.113883.10.20.27.3.5");
							StartAndEnd("code","code","ASSERTION","displayName","Assertion","codeSystem","2.16.840.1.113883.5.4","codeSystemName","ActCode");
							StartAndEnd("statusCode","code","completed");
							Start("value");
							_w.WriteAttributeString("xsi","type",null,"CD");
							switch(j) {
								case 0:
									Attribs("code","IPP","displayName","Initial Patient Population");
									break;
								case 1:
									Attribs("code","DENOM","displayName","Denominator");
									break;
								case 2:
									Attribs("code","DENEX","displayName","Denominator Exclusions");
									break;
								case 3:
									Attribs("code","NUMER","displayName","Numerator");
									break;
								case 4:
									Attribs("code","DENEXCEP","displayName","Denominator Exceptions");
									break;
								default:
									throw new ApplicationException("Error in creating QRDA Category III in Measure Data Components section.");
							}
							Attribs("codeSystem","2.16.840.1.113883.5.1063","codeSystemName","ObservationValue");
							End("value");
							#region Measure Count entry
							//*************Aggregate count entry**************
							_w.WriteComment("Aggregate Count entryRelationship");
							Start("entryRelationship","typeCode","SUBJ","inversionInd","true");
							Start("observation","classCode","OBS","moodCode","EVN");
							_w.WriteComment("Aggregate Count Template");
							TemplateId("2.16.840.1.113883.10.20.27.3.3");
							StartAndEnd("code","code","MSRAGG","displayName","rate aggregation","codeSystem","2.16.840.1.113883.5.4","codeSystemName","ActCode");
							Start("value");
							_w.WriteAttributeString("xsi","type",null,"INT");
							Attribs("value",listPopCountDicts[j]["All"].ToString());
							End("value");
							StartAndEnd("methodCode","code","COUNT","displayName","Count","codeSystem","2.16.840.1.113883.5.84","codeSystemName","ObservationMethod");
							End("observation");
							End("entryRelationship");
							#endregion Measure Count entry
							#region Stratification entries
							//**************Reporting Stratum entryRelationship**************
							//Measure 155 (wight counseling child) has two Strata, Measure 74 (primary caries prevention) has three Strata
							int iterations=0;
							if(i==(int)QualityType2014.CariesPrevent) {
								iterations=3;
							}
							else if(i==(int)QualityType2014.WeightChild_1_1) {
								iterations=2;
							}
							//most measures do not have strata, so these entryRelationships will not usually be created.  Only Measure 155 and 74 have stratification items.
							for(int s=0;s<iterations;s++) {
								_w.WriteComment("***************Reporting Stratum entry***************");
								Start("entryRelationship","typeCode","COMP");
								Start("observation","classCode","OBS","moodCode","EVN");
								_w.WriteComment("Reporting Stratum Template");
								TemplateId("2.16.840.1.113883.10.20.27.3.4");
								StartAndEnd("code","code","ASSERTION","displayName","Assertion","codeSystem","2.16.840.1.113883.5.4","codeSystemName","ActCode");
								StartAndEnd("statusCode","code","completed");
								Start("value");
								_w.WriteAttributeString("xsi","type",null,"CD");
								Attribs("nullFlavor","OTH");
								End("value");
								_w.WriteComment("Stratum");
								Start("entryRelationship","typeCode","SUBJ","inversionInd","true");
								Start("observation","classCode","OBS","moodCode","EVN");
								_w.WriteComment("Aggregate Count Template");
								TemplateId("2.16.840.1.113883.10.20.27.3.3");
								StartAndEnd("code","code","MSRAGG","displayName","rate aggregation","codeSystem","2.16.840.1.113883.5.4","codeSystemName","ActCode");
								Start("value");
								_w.WriteAttributeString("xsi","type",null,"INT");
								int stratIndx=0;
								if(i==(int)QualityType2014.CariesPrevent) {
									if(s==0) {//first strata
										stratIndx=s1Indx;
									}
									if(s==1) {//second strata
										stratIndx=s2Indx;
									}
									if(s==2) {//third strata
										stratIndx=s3Indx;
									}
								}
								else if(i==(int)QualityType2014.WeightChild_1_1) {
									if(p==0) {
										if(s==0) {//second numerator
											stratIndx=s1numer1Indx;
										}
										if(s==1) {//third numerator
											stratIndx=s2numer1Indx;
										}
									}
									if(p==1) {
										if(s==0) {//first strata, second numerator
											stratIndx=s1numer2Indx;
										}
										if(s==1) {//second strata, second population
											stratIndx=s2numer2Indx;
										}
									}
									if(p==2) {
										if(s==0) {//first strata, third population
											stratIndx=s1numer3Indx;
										}
										if(s==1) {//second strata, third population
											stratIndx=s2numer3Indx;
										}
									}
								}
								switch(j) {
									case 0:
										Attribs("value",listQMs[stratIndx].ListEhrPats.Count.ToString());
										break;
									case 1:
										Attribs("value",listQMs[stratIndx].Denominator.ToString());
										break;
									case 2:
										Attribs("value",listQMs[stratIndx].Exclusions.ToString());
										break;
									case 3:
										Attribs("value",listQMs[stratIndx].Numerator.ToString());
										break;
									case 4:
										Attribs("value",listQMs[stratIndx].Exceptions.ToString());
										break;
								}
								End("value");
								StartAndEnd("methodCode","code","COUNT","displayName","Count","codeSystem","2.16.840.1.113883.5.84","codeSystemName","ObservationMethod");
								End("observation");
								End("entryRelationship");
								//end of aggregate count
								//Start reference to strata in eMeasure
								Start("reference","typeCode","REFR");
								_w.WriteComment("Reference to the relevant strata in the eMeasure");
								Start("externalObservation","classCode","OBS","moodCode","EVN");
								switch(j) {
									case 0:
										StartAndEnd("id","root",listQMs[stratIndx].eMeasureIppId);
										break;
									case 1:
										StartAndEnd("id","root",listQMs[stratIndx].eMeasureDenomId);
										break;
									case 2:
										StartAndEnd("id","root",listQMs[stratIndx].eMeasureDenexId);
										break;
									case 3:
										StartAndEnd("id","root",listQMs[stratIndx].eMeasureNumerId);
										break;
									case 4:
										StartAndEnd("id","root",listQMs[stratIndx].eMeasureDenexcepId);
										break;
								}								
								End("externalObservation");
								End("reference");
								//end of reference to eMeasure
								End("observation");								
								End("entryRelationship");
							}
							#endregion Stratification entries
							#region SUPPLEMENTAL DATA STRATUM
							//loop through each of the population counts for this population and generate relevant supplemental data entries
							foreach(KeyValuePair<string,int> kvpair in listPopCountDicts[j]) {
								if(kvpair.Key=="All" && kvpair.Value==0) {//do not need to have these supplemental data entries if no initial patient population, break out of loop
									break;
								}
								if(kvpair.Value==0 || kvpair.Key=="All") {
									continue;
								}
								switch(kvpair.Key) {
									case "Male":
									case "Female":
									case "Unknown":
										//**************Sex Supplemental Data entry**************
										#region Sex Supplemental Data Entry
										_w.WriteComment("Sex Supplemental Data entryRelationship");
										Start("entryRelationship","typeCode","COMP");
										Start("observation","classCode","OBS","moodCode","EVN");
										_w.WriteComment("Sex Supplemental Data Element Template");
										TemplateId("2.16.840.1.113883.10.20.27.3.6");
										StartAndEnd("code","code","184100006","displayName","Patient sex","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
										StartAndEnd("statusCode","code","completed");
										Start("value");
										_w.WriteAttributeString("xsi","type",null,"CD");
										if(kvpair.Key==PatientGender.Male.ToString()) {
											Attribs("code","M","displayName","Male");
										}
										else if(kvpair.Key==PatientGender.Female.ToString()) {
											Attribs("code","F","displayName","Female");
										}
										else {
											Attribs("code","UN","displayName","Undifferentiated");
										}
										Attribs("codeSystem","2.16.840.1.113883.5.1","codeSystemName","AdministrativeGender");
										End("value");
										#endregion Sex Supplemental Data Entry
										break;											
									case "Medicare":
									case "Medicaid":
									case "Other Gvmt":
									case "Private":
									case "Self-pay":
										//**************Payer Supplemental Data**************
										#region Payer Supplemental Data Entry
										_w.WriteComment("Payer Supplemental Data entryRelationship");
										Start("entryRelationship","typeCode","COMP");
										Start("observation","classCode","OBS","moodCode","EVN");
										_w.WriteComment("Patient Characteristic Payer Template");
										TemplateId("2.16.840.1.113883.10.20.24.3.55");
										_w.WriteComment("Payer Supplemental Data Element Template");
										TemplateId("2.16.840.1.113883.10.20.27.3.9");
										Guid();
										StartAndEnd("code","code","48768-6","displayName","Payment source","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
										StartAndEnd("statusCode","code","completed");
										Start("value");
										_w.WriteAttributeString("xsi","type",null,"CD");
										if(kvpair.Key=="Medicare") {
											Attribs("code","1","displayName","Medicare");
										}
										else if(kvpair.Key=="Medicaid") {
											Attribs("code","2","displayName","Medicaid");
										}
										else if(kvpair.Key=="Other Gvmt") {
											Attribs("code","349","displayName","Other Government (Federal/State/Local) (excluding Department of Corrections)");
										}
										else if(kvpair.Key=="Private") {
											Attribs("code","5","displayName","Private Health Insurance");
										}
										else {
											Attribs("code","8","displayName","No Payment from an Organization/Agency/Program/Private Payer Listed");
										}
										Attribs("codeSystem","2.16.840.1.113883.3.221.5","codeSystemName","Source of Payment Typology");
										End("value");
										#endregion Payer Supplemental Data Entry
										break;
									default://All race and ethnicity measures
										Cdcrec cdcRec;
										if(!dictRaces.TryGetValue(kvpair.Key,out cdcRec)) {//dictRaces's key is cdcrec.CdcrecCode
											throw new ApplicationException("Error in creating QRDA Category III in Measure Data Components section. Unknown CDCREC code.");
										}
										if(cdcRec.HeirarchicalCode.StartsWith("E")) {
											//**************Ethnicity Supplemental Data**************
											#region Ethnicity Supplemental Data Entry
											_w.WriteComment("Ethnicity Supplemental Data entryRelationship");
											Start("entryRelationship","typeCode","COMP");
											Start("observation","classCode","OBS","moodCode","EVN");
											_w.WriteComment("Ethnicity Supplemental Data Element Template");
											TemplateId("2.16.840.1.113883.10.20.27.3.7");
											StartAndEnd("code","code","364699009","displayName","Ethnic Group","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
											StartAndEnd("statusCode","code","completed");
											Start("value");
											_w.WriteAttributeString("xsi","type",null,"CD");
											Attribs("code",cdcRec.CdcrecCode,"displayName",cdcRec.Description);
											Attribs("codeSystem","2.16.840.1.113883.6.238","codeSystemName","Race &amp; Ethnicity - CDC");
											End("value");
											#endregion Ethnicity Supplemental Data Entry
										}
										else if(cdcRec.HeirarchicalCode.StartsWith("R")) {
											//**************Race Supplemental Data**************
											#region Race Supplemental Data Entry
											_w.WriteComment("Race Supplemental Data entryRelationship");
											Start("entryRelationship","typeCode","COMP");
											Start("observation","classCode","OBS","moodCode","EVN");
											_w.WriteComment("Race Supplemental Data Element Template");
											TemplateId("2.16.840.1.113883.10.20.27.3.8");
											StartAndEnd("code","code","103579009","displayName","Race","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
											StartAndEnd("statusCode","code","completed");
											Start("value");
											_w.WriteAttributeString("xsi","type",null,"CD");
											Attribs("code",cdcRec.CdcrecCode,"displayName",cdcRec.Description);
											Attribs("codeSystem","2.16.840.1.113883.6.238","codeSystemName","Race &amp; Ethnicity - CDC");
											End("value");
											#endregion Race Supplemental Data Entry
											break;
										}
										else {//Unexpected start to HierarchicalCode
											throw new ApplicationException("Error in creating QRDA Category III in Measure Data Components section. Unrecognized Hierarchical Code");
										}
										break;
								}
								_w.WriteComment("Aggregate Count entryRelationship");
								Start("entryRelationship","typeCode","SUBJ","inversionInd","true");
								_w.WriteComment("Aggregate Count Template");
								Start("observation","classCode","OBS","moodCode","EVN");
								TemplateId("2.16.840.1.113883.10.20.27.3.3");
								StartAndEnd("code","code","MSRAGG","displayName","rate aggregation","codeSystem","2.16.840.1.113883.5.4","codeSystemName","ActCode");
								Start("value");
								_w.WriteAttributeString("xsi","type",null,"INT");
								Attribs("value",kvpair.Value.ToString());
								End("value");
								StartAndEnd("methodCode","code","COUNT","displayName","Count","codeSystem","2.16.840.1.113883.5.84","codeSystemName","ObservationMethod");
								End("observation");
								End("entryRelationship");
								End("observation");
								End("entryRelationship");
							}
							#endregion SUPPLEMENTAL DATA STRATUM
							//*************eMeasure Reference**************
							_w.WriteComment("eMeasure reference");
							Start("reference","typeCode","REFR");
							Start("externalObservation","classCode","OBS","moodCode","EVN");
							switch(j) {
								case 0:
									StartAndEnd("id","root",listQMs[measureIndx].eMeasureIppId);
									break;
								case 1:
									StartAndEnd("id","root",listQMs[measureIndx].eMeasureDenomId);
									break;
								case 2:
									StartAndEnd("id","root",listQMs[measureIndx].eMeasureDenexId);
									break;
								case 3:
									StartAndEnd("id","root",listQMs[measureIndx].eMeasureNumerId);
									break;
								case 4:
									StartAndEnd("id","root",listQMs[measureIndx].eMeasureDenexcepId);
									break;
							}
							End("externalObservation");
							End("reference");
							End("observation");
							End("component");
						}
					#endregion Measure Data
					End("organizer");
					End("entry");
					}
				}
				End("section");
				#endregion ENTRY SECTION
				End("component");
				#endregion measure component
				End("structuredBody");
				End("component");
				#endregion QRDA III Body
				End("ClinicalDocument");
				_w.Flush();
				_w.Close();
			}
		}

		private static void GenerateQRDACatOne(List<QualityMeasure> listQMs,DateTime dateStart,DateTime dateEnd,string folderRoot) {
			//create a list of all unique patients who belong to the initial patient population for any of the 9 CQMs
			List<EhrCqmPatient> listAllEhrPats=new List<EhrCqmPatient>();
			//create a dictionary linking a patient to the measures to which they belong
			Dictionary<long,List<QualityMeasure>> dictPatNumListQMs=new Dictionary<long,List<QualityMeasure>>();
			//list of all 9 unique emeasure numbers, used to generate the zip files
			List<string> listAllEMeasureNums=new List<string>();
			//dictionary links a patient to any of the 9 emeasure nums they are in the initial patient population for, used to generate the zip files
			Dictionary<long,List<string>> dictPatNumListEMeasureNums=new Dictionary<long,List<string>>();
			for(int i=0;i<listQMs.Count;i++) {
				if(!listAllEMeasureNums.Contains(listQMs[i].eMeasureNum)) {
					listAllEMeasureNums.Add(listQMs[i].eMeasureNum);
				}
				for(int j=0;j<listQMs[i].ListEhrPats.Count;j++) {
					if(!dictPatNumListQMs.ContainsKey(listQMs[i].ListEhrPats[j].EhrCqmPat.PatNum)) {
						dictPatNumListQMs.Add(listQMs[i].ListEhrPats[j].EhrCqmPat.PatNum,new List<QualityMeasure>() { listQMs[i] });
						listAllEhrPats.Add(listQMs[i].ListEhrPats[j]);
					}
					else {
						dictPatNumListQMs[listQMs[i].ListEhrPats[j].EhrCqmPat.PatNum].Add(listQMs[i]);
					}
					if(!dictPatNumListEMeasureNums.ContainsKey(listQMs[i].ListEhrPats[j].EhrCqmPat.PatNum)) {
						dictPatNumListEMeasureNums.Add(listQMs[i].ListEhrPats[j].EhrCqmPat.PatNum,new List<string>() { listQMs[i].eMeasureNum });
					}
					else {
						dictPatNumListEMeasureNums[listQMs[i].ListEhrPats[j].EhrCqmPat.PatNum].Add(listQMs[i].eMeasureNum);
					}
				}
			}
			//listAllPats contains every unique patient who belongs to one of the initial patient population for any of the 9 CQMs
			//dictPatNumListQMs is a dictionary linking a patient to a list of measure for which they are in the initial patient population
			StringBuilder strBuilder=new StringBuilder();
			StringBuilder strBuilderPatDataEntries=new StringBuilder();
			//this dictionary links a PatNum key to the patient's Cat I xml dictionary value
			Dictionary<long,string> dictPatNumXml=new Dictionary<long,string>();
			System.Diagnostics.Stopwatch swGenerateXMLs=new System.Diagnostics.Stopwatch();
			System.Diagnostics.Stopwatch swCreateZipFiles=new System.Diagnostics.Stopwatch();
			if(ODBuild.IsDebug()) {
				swGenerateXMLs.Restart();
			}
			#region Cateogry I QRDA Documents
			for(int i=0;i<listAllEhrPats.Count;i++) {
				strBuilder=new StringBuilder();
				strBuilderPatDataEntries=new StringBuilder();
				EhrCqmPatient ehrPatCur=listAllEhrPats[i];
				Patient patCur=ehrPatCur.EhrCqmPat;//just to make referencing easier
				long patNumCur=ehrPatCur.EhrCqmPat.PatNum;//just to make referencing easier
				XmlWriterSettings xmlSettings=new XmlWriterSettings();
				xmlSettings.Encoding=Encoding.UTF8;
				xmlSettings.OmitXmlDeclaration=true;
				xmlSettings.Indent=true;
				xmlSettings.IndentChars="   ";
				xmlSettings.ConformanceLevel=ConformanceLevel.Fragment;
				using(_w=XmlWriter.Create(strBuilder,xmlSettings)) {
					//Begin Clinical Document
					_w.WriteRaw("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n");
					_w.WriteProcessingInstruction("xml-stylesheet","type=\"text/xsl\" href=\"cda.xsl\"");
					_w.WriteWhitespace("\r\n");
					_w.WriteStartElement("ClinicalDocument","urn:hl7-org:v3");
					_w.WriteAttributeString("xmlns","xsi",null,"http://www.w3.org/2001/XMLSchema-instance");
					_w.WriteAttributeString("xsi","schemaLocation",null,"urn:./CDA.xsd");
					_w.WriteAttributeString("xmlns","voc",null,"urn:hl7-org:v3/voc");
					_w.WriteAttributeString("xmlns","sdtc",null,"urn:hl7-org:sdtc");
					#region QRDA I Header
					_w.WriteComment("QRDA Header");
					StartAndEnd("realmCode","code","US");
					StartAndEnd("typeId","root","2.16.840.1.113883.1.3","extension","POCD_HD000040");//template id to assert use of the CDA standard
					_w.WriteComment("US General Header Template");
					TemplateId("2.16.840.1.113883.10.20.22.1.1");
					_w.WriteComment("QRDA Template");
					TemplateId("2.16.840.1.113883.10.20.24.1.1");
					_w.WriteComment("QDM-based QRDA Template");
					TemplateId("2.16.840.1.113883.10.20.24.1.2");
					_w.WriteComment("This is the globally unique identifier for this QRDA document");
					Guid();
					_w.WriteComment("QRDA document type code");
					StartAndEnd("code","code","55182-0","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","Quality Measure Report");
					_w.WriteElementString("title","QRDA Incidence Report");
					_w.WriteComment("This is the document creation time");
					TimeElement("effectiveTime",DateTime.Now);
					StartAndEnd("confidentialityCode","code","N","codeSystem","2.16.840.1.113883.5.25");//Fixed value.  Confidentiality Code System.  Codes: N=(Normal), R=(Restricted),V=(Very Restricted)
					StartAndEnd("languageCode","code","en-US");
					#region recordTarget
					_w.WriteComment("Reported patient");
					Start("recordTarget");
					#region patientRole
					Start("patientRole");
					//According to the QDM-Based QRDA template, the patientRole SHALL contain exactly 1 id such that it SHOULD contain zero or one Medicare HIC number
					//No other id's are allowed, so if they do not have the Medicaid ID field in the patient info window filled in, the id will have nullFlavor=NI (No Information)
					//see section 5.2 in the QRDA category I implementation guide, release 2
					//located here: \\SERVERFILES\storage\EHR\Quality Measures\QRDA\CDAR2_QRDA_DSTUR2_2012JUL\CDAR2_QRDA_DSTUR2_2012JUL.docx
					//if(patCur.SSN.Trim().Length==9) {
					//	_w.WriteComment("This is the patient's SSN using the HL7 SSN OID");
					//	StartAndEnd("id","root","2.16.840.1.113883.4.1","extension",patCur.SSN.Trim());//HL7 SSN OID root with patient's SSN if they have a valid one
					//}
					//_w.WriteComment("The extension is the patient's Open Dental number, the root is the assigning authority");
					//StartAndEnd("id","root",_strOIDInternalPatRoot,"extension",patNumCur.ToString());
					string strHICNum=ValidateMedicaidID(patCur.MedicaidID);
					if(strHICNum!="") {
						_w.WriteComment("This is the patient's Medicare HIC (Health Insurance Claim) number");
						StartAndEnd("id","root","2.16.840.1.113883.4.572","extension",strHICNum);
					}
					else {
						_w.WriteComment("The patient does not have a Medicare HIC (Health Insurance Claim) number assigned, use nullFlavor NI = No Information");
						StartAndEnd("id","nullFlavor","NI");
					}
					AddressUnitedStates(patCur.Address,patCur.Address2,patCur.City,patCur.State,patCur.Zip,"HP");//Validated
					if(patCur.WirelessPhone.Trim()!="") {//There is at least one phone, due to validation.
						StartAndEnd("telecom","use","MC","value","tel:"+patCur.WirelessPhone.Trim());
						_w.WriteComment("MC is \"mobile contact\" from codeSystem 2.16.840.1.113883.5.1119");
					}
					else if(patCur.HmPhone.Trim()!="") {
						StartAndEnd("telecom","use","HP","value","tel:"+patCur.HmPhone.Trim());
						_w.WriteComment("HP is \"primary home\" from codeSystem 2.16.840.1.113883.5.1119");
					}
					else if(patCur.WkPhone.Trim()!="") {
						StartAndEnd("telecom","use","WP","value","tel:"+patCur.WkPhone.Trim());
						_w.WriteComment("WP is \"work place\" from codeSystem 2.16.840.1.113883.5.1119");
					}
					#region patient
					Start("patient");
					#region name
					Start("name","use","L");
					_w.WriteComment("L is \"Legal\" from codeSystem 2.16.840.1.113883.5.45");
					_w.WriteElementString("given",patCur.FName);//Validated
					if(patCur.MiddleI!="") {
						_w.WriteElementString("given",patCur.MiddleI);
					}
					_w.WriteElementString("family",patCur.LName);//Validated
					if(patCur.Title!="") {
						Start("suffix","qualifier","TITLE");
						_w.WriteString(patCur.Title);
						End("suffix");
					}
					End("name");
					#endregion name
					//Will always be present, because there are only 3 options and the user is forced to make a choice in the UI.
					if(patCur.Gender==PatientGender.Female) {
						StartAndEnd("administrativeGenderCode","code","F","codeSystem","2.16.840.1.113883.5.1","displayName","Female","codeSystemName","HL7 AdministrativeGender");
					}
					else if(patCur.Gender==PatientGender.Male) {
						StartAndEnd("administrativeGenderCode","code","M","codeSystem","2.16.840.1.113883.5.1","displayName","Male","codeSystemName","HL7 AdministrativeGender");
					}
					else {
						StartAndEnd("administrativeGenderCode","code","UN","codeSystem","2.16.840.1.113883.5.1","displayName","Undifferentiated","codeSystemName","HL7 AdministrativeGender");
					}
					DateElement("birthTime",patCur.Birthdate);//Validated
					if(patCur.Position==PatientPosition.Divorced) {
						StartAndEnd("maritalStatusCode","code","D","displayName","Divorced","codeSystem","2.16.840.1.113883.5.2","codeSystemName","MaritalStatusCode");
					}
					else if(patCur.Position==PatientPosition.Married) {
						StartAndEnd("maritalStatusCode","code","M","displayName","Married","codeSystem","2.16.840.1.113883.5.2","codeSystemName","MaritalStatusCode");
					}
					else if(patCur.Position==PatientPosition.Widowed) {
						StartAndEnd("maritalStatusCode","code","W","displayName","Widowed","codeSystem","2.16.840.1.113883.5.2","codeSystemName","MaritalStatusCode");
					}
					else {//Single and child
						StartAndEnd("maritalStatusCode","code","S","displayName","Never Married","codeSystem","2.16.840.1.113883.5.2","codeSystemName","MaritalStatusCode");
					}
					#region raceCode
					string strRaceDescript="";
					string strRaceCode="2131-1";//default will be "Other Race"
					//if declined to specify race, null flavor is "ASKU" - Asked, but not known. Information was sought, but not found
					//if race is not present, null flavor is "UNK" - Unknown. A proper value is applicable, but is not known.
					//These will appear as <raceCode nullFlavor=”ASKU”/> or <raceCode nullFlavor=”UNK”/> respectively
					bool isRaceNull=false;
					if(ehrPatCur.ListPatientRaces.Count==0) {
						isRaceNull=true;
						strRaceDescript="UNK";
					}
					else if(ehrPatCur.ListPatientRaces.Count==1) {//if only one race is entered (already separated out the ethnicities) then we can construct the raceCode element with selected race
						strRaceDescript=ehrPatCur.ListPatientRaces[0].Description;
						strRaceCode=ehrPatCur.ListPatientRaces[0].CdcrecCode;
						if(strRaceCode==PatientRace.DECLINE_SPECIFY_RACE_CODE) {
							isRaceNull=true;
							strRaceDescript="ASKU";
						}
					}
					//"If there are multiple race values reported for a patient, count as ‘Other Race’ value" from downloaded implementation guide found:
					//\\SERVERFILES\storage\EHR\Quality Measures\QRDA\QRDAIII_CMS_EP_2014_ImplementationGuide_Vol1\QRDA_III_CMS_EP_2014_IG_Vol 2.pdf page 49
					else {
						strRaceDescript="Other Race";
					}
					if(isRaceNull) {//either declined to specify (flavor "ASKU") or no race recorded (flavor "UNK")
						StartAndEnd("raceCode","nullFlavor",strRaceDescript);
					}
					else {
						StartAndEnd("raceCode","code",strRaceCode,"displayName",strRaceDescript,"codeSystem","2.16.840.1.113883.6.238","codeSystemName","CDC Race and Ethnicity");
					}
					#endregion raceCode
					#region ethnicityGroupCode
					string strEthnicityCode="";
					string strEthnicityDescript="";
					bool isDeclinedEthnicity=false;
					if(ehrPatCur.Ethnicity!=null) {
						if(ehrPatCur.Ethnicity.CdcrecCode==PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE) {
							isDeclinedEthnicity=true;
						}
						else { 
							strEthnicityDescript=ehrPatCur.Ethnicity.Description;
							strEthnicityCode=ehrPatCur.Ethnicity.CdcrecCode;
						}
					}
					if(ehrPatCur.Ethnicity==null) {
						StartAndEnd("ethnicGroupCode","nullFlavor","UNK");
					}
					else if(isDeclinedEthnicity) {
						StartAndEnd("ethnicGroupCode","nullFlavor","ASKU");
					}
					else if(strEthnicityCode!="" && strEthnicityDescript!=""){
						StartAndEnd("ethnicGroupCode","code",strEthnicityCode,"displayName",strEthnicityDescript,"codeSystem","2.16.840.1.113883.6.238","codeSystemName","CDC Race and Ethnicity");
					}
					#endregion ethnicityGroupCode
					End("patient");
					#endregion patient
					End("patientRole");
					#endregion patientRole
					End("recordTarget");
					#endregion recordTarget
					#region comments
					//The author element represents the creator of the clinical document.  The author may be a device, or a person.  Section 4.1.2, page 69.
					//Participant Scenarios in a QRDA Category I Document (section 5.1.5.3, page 94)
					//Three possible scenarios given, the first sounds like it applies to us
					//1.) QRDA is wholly constructed automatically by device
					//Author - Device
					//Custodian - Organization that owns and reports the data (e.g. hospital, dental practice)
					//Informant - N/A
					//Legal Authenticator - A designated person in the organization (may be assigned to the report automatically)
					//2.) QRDA is partially constructed automatically by device, partially constructed by quality manager
					//3.) QRDA is constructed manually (e.g. by an organization that doesn't have an EHR)
					//We will generate a device author element, a practice custodian element, no informant element, and a Legal Authenticator element using the practice default provider
					#endregion
					#region author
					Start("author");
					TimeElement("time",DateTime.Now);
					Start("assignedAuthor");
					StartAndEnd("id","root","2.16.840.1.113883.3.4337","assigningAuthorityName","HL7 OID Registry");
					AddressUnitedStates("3995 Fairview Industrial Dr. SE","Suite 110","Salem","OR","97302","WP");
					StartAndEnd("telecom","use","WP","value","tel:(503)363-5432");
					Start("assignedAuthoringDevice");
					_w.WriteElementString("manufacturerModelName","Open Dental version "+PrefC.GetString(PrefName.ProgramVersion));
					_w.WriteElementString("softwareName","Open Dental Software Inc.");
					End("assignedAuthoringDevice");
					End("assignedAuthor");
					End("author");
					#endregion author
					#region custodian
					//"Represents the organization in charge of maintaining the document." Section 4.1.5, page 77
					//The custodian is the steward that is entrusted with the care of the document. Every CDA document has exactly one custodian.
					Start("custodian");
					Start("assignedCustodian");
					Start("representedCustodianOrganization");
					StartAndEnd("id","root",_strOIDInternalRoot);//This is the root assigned to the practice, based on the OD root 2.16.840.1.113883.3.4337
					_w.WriteElementString("name",PrefC.GetString(PrefName.PracticeTitle));//Validated
					string strPracticePhone=PrefC.GetString(PrefName.PracticePhone);//Validated
					strPracticePhone="("+strPracticePhone.Substring(0,3)+")"+strPracticePhone.Substring(3,3)+"-"+strPracticePhone.Substring(6);
					StartAndEnd("telecom","use","WP","value","tel:"+strPracticePhone);//Validated
					AddressUnitedStates(PrefC.GetString(PrefName.PracticeAddress),PrefC.GetString(PrefName.PracticeAddress2),PrefC.GetString(PrefName.PracticeCity),PrefC.GetString(PrefName.PracticeST),PrefC.GetString(PrefName.PracticeZip),"WP");//Validated
					End("representedCustodianOrganization");
					End("assignedCustodian");
					End("custodian");
					#endregion custodian
					#region legalAuthenticator
					//This element identifies the single person legally responsible for the document and must be present if the document has been legally authenticated.
					Start("legalAuthenticator");
					TimeElement("time",DateTime.Now);
					StartAndEnd("signatureCode","code","S");
					Start("assignedEntity");
					Provider provLegal=Providers.GetProv(_provNumLegal);
					StartAndEnd("id","root","2.16.840.1.113883.4.6","extension",provLegal.NationalProvID,"assigningAuthorityName","NPI");//Validated NPI
					AddressUnitedStates(PrefC.GetString(PrefName.PracticeAddress),PrefC.GetString(PrefName.PracticeAddress2),PrefC.GetString(PrefName.PracticeCity),PrefC.GetString(PrefName.PracticeST),PrefC.GetString(PrefName.PracticeZip),"WP");//Validated
					StartAndEnd("telecom","use","WP","value","tel:"+strPracticePhone);//Validated
					Start("assignedPerson");
					Start("name");
					_w.WriteElementString("given",provLegal.FName);//Validated
					_w.WriteElementString("family",provLegal.LName);//Validated
					End("name");
					End("assignedPerson");
					End("assignedEntity");
					End("legalAuthenticator");
					#endregion legalAuthenticator
					#region documentationOf
					Start("documentationOf","typeCode","DOC");
					_w.WriteComment("care provision");
					Start("serviceEvent","classCode","PCPR");
					Start("effectiveTime");
					DateElement("low",dateStart);
					DateElement("high",dateEnd);
					End("effectiveTime");
					#region performer
					Start("performer","typeCode","PRF");
					Start("assignedEntity");
					if(_provOutQrda.NationalProvID!="") {
						_w.WriteComment("This is the provider NPI");
						StartAndEnd("id","root","2.16.840.1.113883.4.6","extension",_provOutQrda.NationalProvID);
					}
					if(_provOutQrda.UsingTIN && _provOutQrda.SSN!="") {
						_w.WriteComment("This is the provider TIN");
						StartAndEnd("id","root","2.16.840.1.113883.4.2","extension",_provOutQrda.SSN);
					}
					_w.WriteComment("This is the practice OID provider root and Open Dental assigned ProvNum extension");
					StartAndEnd("id","root",_strOIDInternalProvRoot,"extension",_provOutQrda.ProvNum.ToString());
					Start("representedOrganization");
					//we don't currently have an organization level TIN or an organization Facility CMS Certification Number (CCN)
					//both id's are identified as "SHOULD" elements.  We will include the practice name
					//_w.WriteComment("This is the organization TIN");
					//_w.WriteComment("This is the organization CCN");
					_w.WriteElementString("name",PrefC.GetString(PrefName.PracticeTitle));//Validated
					End("representedOrganization");
					End("assignedEntity");
					End("performer");
					#endregion performer
					End("serviceEvent");
					End("documentationOf");
					#endregion documentationOf
					#endregion QRDA I Header
					#region QRDA I Body
					_w.WriteComment("QRDA Body");
					Start("component");
					Start("structuredBody");
					#region Reporting Parameters
					Start("component");
					Start("section");
					_w.WriteComment(@"
					  *****************************************************************
					  Reporting Parameters Section
					  *****************************************************************
					  ");
					_w.WriteComment("Reporting Parameters Section Template");
					TemplateId("2.16.840.1.113883.10.20.17.2.1");
					StartAndEnd("code","code","55187-9","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","Reporting Parameters");
					_w.WriteElementString("title","Reporting Parameters");
					Start("text");
					Start("list");
					_w.WriteElementString("item","Reporting period: "+dateStart.ToString("MMMM dd, yyyy")+" 00:00 - "+dateEnd.ToString("MMMM dd, yyyy")+" 23:59");
					End("list");
					End("text");
					Start("entry","typeCode","DRIV");
					Start("act","classCode","ACT","moodCode","EVN");
					_w.WriteComment("Reporting Parameteres Act Template");
					TemplateId("2.16.840.1.113883.10.20.17.3.8");
					StartAndEnd("code","code","252116004","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed,"displayName","Observation Parameters");
					Start("effectiveTime");
					StartAndEnd("low","value",dateStart.ToString("yyyyMMdd")+"000000");
					StartAndEnd("high","value",dateEnd.ToString("yyyyMMdd")+"235959");
					End("effectiveTime");
					End("act");
					End("entry");
					End("section");
					End("component");
					#endregion Reporting Parameters
					#region Measure Section
					Start("component");
					Start("section");
					_w.WriteComment(@"
					  *****************************************************************
					  Measure Section
					  *****************************************************************
					  ");
					_w.WriteComment("Measure Section Template");
					TemplateId("2.16.840.1.113883.10.20.24.2.2");
					_w.WriteComment("Measure Section QDM Template");
					TemplateId("2.16.840.1.113883.10.20.24.2.3");
					_w.WriteComment("This is the LOINC code for \"Measure document\". This stays the same for all measure sections required by QRDA standard");
					StartAndEnd("code","code","55186-1","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","Measure Document");
					_w.WriteElementString("title","Measure Section");
					#region text version of eMeasures as table
					Start("text");
					Start("table","border","1","width","100%");
					Start("thead");
					Start("tr");
					_w.WriteElementString("th","eMeasure Identifier (MAT)");
					_w.WriteElementString("th","eMeasure Title");
					_w.WriteElementString("th","Version neutral identifier");
					_w.WriteElementString("th","eMeasure Version Number");
					_w.WriteElementString("th","Version specific identifier");
					End("tr");
					End("thead");
					Start("tbody");
					bool isInMeas155=false;
					for(int j=0;j<listQMs.Count;j++) {
						//_listExtraPopIndxs will contain all of the Type2014 enum types that are stratifications or extra populations of other measures, so skip them here
						if(_listExtraPopIndxs.Contains(j)
							&& j!=(int)QualityType2014.WeightAdult //patient will be in either WeightAdult or WeightOver65 or neither, if in either group add measure
							&& j!=(int)QualityType2014.WeightChild_1_2
							&& j!=(int)QualityType2014.WeightChild_1_3)//if in any numerator for measure 155, Height and Weight, counseling for nutrition, or counseling for physical activity, add the measure
						{
							continue;
						}
						for(int k=0;k<dictPatNumListQMs[patNumCur].Count;k++) {
							if((int)dictPatNumListQMs[patNumCur][k].Type2014!=j) {
								continue;
							}
							if(j==(int)QualityType2014.WeightChild_1_1 || j==(int)QualityType2014.WeightChild_1_2 || j==(int)QualityType2014.WeightChild_1_3) {
								if(isInMeas155) {
									break;
								}
								isInMeas155=true;
							}
							MeasureTextTableRow(listQMs[j].eMeasureTitle,listQMs[j].eMeasureNum,listQMs[j].eMeasureVNeutralId,listQMs[j].eMeasureVersion,listQMs[j].eMeasureVSpecificId);
							break;
						}
					}
					End("tbody");
					End("table");
					End("text");
					#endregion text version of eMeasures as table
					#region eMeasure entries
					_w.WriteComment("1..* Organizers, each containing a reference to an eMeasure");
					isInMeas155=false;
					for(int j=0;j<listQMs.Count;j++) {
						//_listExtraPopIndxs will contain all of the Type2014 enum types that are stratifications or extra populations of other measures, so skip them here
						if(_listExtraPopIndxs.Contains(j)
							&& j!=(int)QualityType2014.WeightAdult //patient will be in either WeightAdult or WeightOver65 or neither, if in either group add measure
							&& j!=(int)QualityType2014.WeightChild_1_2
							&& j!=(int)QualityType2014.WeightChild_1_3)//if in any numerator for measure 155, Height and Weight, counseling for nutrition, or counseling for physical activity, add the measure
						{
							continue;
						}
						for(int k=0;k<dictPatNumListQMs[patNumCur].Count;k++) {
							if((int)dictPatNumListQMs[patNumCur][k].Type2014!=j) {
								continue;
							}
							if(j==(int)QualityType2014.WeightChild_1_1 || j==(int)QualityType2014.WeightChild_1_2 || j==(int)QualityType2014.WeightChild_1_3) {
								if(isInMeas155) {
									break;
								}
								isInMeas155=true;
							}
							MeasureEntry(listQMs[j].eMeasureVSpecificId,listQMs[j].eMeasureTitle,listQMs[j].eMeasureVNeutralId,listQMs[j].eMeasureVersion);
							break;
						}
					}
					#endregion eMeasure entries
					End("section");
					End("component");
					#endregion Measure Section
					#region Patient Data
					Start("component");
					Start("section");
					_w.WriteComment(@"
					  *****************************************************************
					  Patient Data Section
					  *****************************************************************
					  ");
					_w.WriteComment("Patient Data Section Template");
					TemplateId("2.16.840.1.113883.10.20.17.2.4");
					_w.WriteComment("Patient Data QDM Section Template");
					TemplateId("2.16.840.1.113883.10.20.24.2.1");
					StartAndEnd("code","code","55188-7","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","Patient Data");
					_w.WriteElementString("title","Patient Data");
					Start("text");
					Start("table","border","1","width","100%");
					Start("thead");
					Start("tr");
					_w.WriteElementString("th","Description");
					_w.WriteElementString("th","Value Set");
					_w.WriteElementString("th","Code");
					_w.WriteElementString("th","Date/Time");
					_w.WriteElementString("th","Results");
					End("tr");
					End("thead");
					Start("tbody");
					using(_x=XmlWriter.Create(strBuilderPatDataEntries,xmlSettings)) {
						_x.WriteStartElement("ClinicalDocument","urn:hl7-org:v3");
						_x.WriteAttributeString("xmlns","xsi",null,"http://www.w3.org/2001/XMLSchema-instance");
						_x.WriteAttributeString("xsi","schemaLocation",null,"urn:./CDA.xsd");
						_x.WriteAttributeString("xmlns","voc",null,"urn:hl7-org:v3/voc");
						_x.WriteAttributeString("xmlns","sdtc",null,"urn:hl7-org:sdtc");
						List<QualityMeasure> listQMsCur=dictPatNumListQMs[patNumCur];
						//this list of unique item ids is used when building the plain text version so that the encounter/procedure/item will only be in the plain text version once
						//we will add the item to the entries multiple times since the item can belong to different value sets
						//and must have the value set specific to the measure for which it qualifies the patient
						List<string> listUniqueItemExtensionsWithValueSetOIDs=new List<string>();
						List<string> listUniqueItemExtensions=new List<string>();
						//create encounter entries for each measure for this patient
						#region encounters
						for(int j=0;j<listQMsCur.Count;j++) {
							if(!listQMsCur[j].DictPatNumListEncounters.ContainsKey(patNumCur)) {
								continue;
							}
							for(int k=0;k<listQMsCur[j].DictPatNumListEncounters[patNumCur].Count;k++) {
								EhrCqmEncounter encCur=listQMsCur[j].DictPatNumListEncounters[patNumCur][k];
								//if in this list with ValueSetOID, then it must be in the non-ValueSetOID list, so just continue
								string extensCur=CqmItemAbbreviation.Enc.ToString()+encCur.ValueSetOID+"Num"+encCur.EhrCqmEncounterNum.ToString();
								if(!listUniqueItemExtensionsWithValueSetOIDs.Contains(extensCur)) {
									listUniqueItemExtensionsWithValueSetOIDs.Add(extensCur);
									GenerateEncounterEntry(encCur);
								}
								extensCur=CqmItemAbbreviation.Enc.ToString()+encCur.EhrCqmEncounterNum.ToString();
								if(listUniqueItemExtensions.Contains(extensCur)) {
									continue;
								}
								listUniqueItemExtensions.Add(extensCur);
								#region BuildPlainTextVersion
								string descript="Encounter, Performed: "+encCur.ValueSetName;
								PatientDataTextTableRow(descript,encCur.ValueSetOID,encCur.CodeSystemName,encCur.CodeValue,encCur.DateEncounter,encCur.DateEncounter,"");
								#endregion BuildPlainTextVersion
							}
						}
						#endregion encounters
						//create intervention entries for each measure for this patient
						#region interventions
						for(int j=0;j<listQMsCur.Count;j++) {
							if(!listQMsCur[j].DictPatNumListInterventions.ContainsKey(patNumCur)) {
								continue;
							}
							for(int k=0;k<listQMsCur[j].DictPatNumListInterventions[patNumCur].Count;k++) {
								EhrCqmIntervention iCur=listQMsCur[j].DictPatNumListInterventions[patNumCur][k];
								string extensCur=CqmItemAbbreviation.Ivn.ToString()+iCur.ValueSetOID+"Num"+iCur.EhrCqmInterventionNum.ToString();
								if(!listUniqueItemExtensionsWithValueSetOIDs.Contains(extensCur)) {
									listUniqueItemExtensionsWithValueSetOIDs.Add(extensCur);
									GenerateInterventionEntry(iCur);
								}
								extensCur=CqmItemAbbreviation.Ivn.ToString()+iCur.EhrCqmInterventionNum.ToString();
								if(listUniqueItemExtensions.Contains(extensCur)) {
									continue;
								}
								listUniqueItemExtensions.Add(extensCur);
								#region BuildPlainTextVersion
								string descript="";
								if(listQMsCur[j].Type2014==QualityType2014.WeightAdult || listQMsCur[j].Type2014==QualityType2014.WeightOver65) {//interventions in these two measures are ordered
									descript="Intervention, Order: ";
								}
								else {
									descript="Intervention, Performed: ";
								}
								descript+=iCur.ValueSetName;
								PatientDataTextTableRow(descript,iCur.ValueSetOID,iCur.CodeSystemName,iCur.CodeValue,iCur.DateEntry,iCur.DateEntry,"");
								#endregion BuildPlainTextVersion
							}
						}
						#endregion interventions
						//create ehrmeasureevents for each measure for this patient
						#region ehrmeasureevents
						for(int j=0;j<listQMsCur.Count;j++) {
							if(!listQMsCur[j].DictPatNumListMeasureEvents.ContainsKey(patNumCur)) {
								continue;
							}
							for(int k=0;k<listQMsCur[j].DictPatNumListMeasureEvents[patNumCur].Count;k++) {
								EhrCqmMeasEvent mCur=listQMsCur[j].DictPatNumListMeasureEvents[patNumCur][k];
								string extensCur=CqmItemAbbreviation.MeasEvn.ToString()+mCur.ValueSetOID+"Num"+mCur.EhrCqmMeasEventNum.ToString();
								if(!listUniqueItemExtensionsWithValueSetOIDs.Contains(extensCur)) {
									listUniqueItemExtensionsWithValueSetOIDs.Add(extensCur);
									GenerateMeasEventEntry(mCur);
								}
								extensCur=CqmItemAbbreviation.MeasEvn.ToString()+mCur.EhrCqmMeasEventNum.ToString();
								if(listUniqueItemExtensions.Contains(extensCur)) {
									continue;
								}
								listUniqueItemExtensions.Add(extensCur);
								#region BuildPlainTextVersion
								string descript="";
								if(mCur.EventType==EhrMeasureEventType.TobaccoUseAssessed) {
									descript="Patient Characteristic: ";
								}
								else if(mCur.EventType==EhrMeasureEventType.CurrentMedsDocumented) {
									descript="Procedure, Performed: ";
								}
								descript+=mCur.ValueSetName+" - "+mCur.Description;
								PatientDataTextTableRow(descript,mCur.ValueSetOID,mCur.CodeSystemName,mCur.CodeValue,mCur.DateTEvent,mCur.DateTEvent,"");
								#endregion BuildPlainTextVersion
							}
						}
						#endregion ehrmeasureevents
						//create medicationpats for each measure for this patient
						#region medicationpats
						for(int j=0;j<listQMsCur.Count;j++) {
							//have to check for CompletionStatus=NotAdministered if vaccine, vaccines given are procedures performed, not given are medication allergy or intolerance
							if(!listQMsCur[j].DictPatNumListMedPats.ContainsKey(patNumCur)) {
								continue;
							}
							for(int k=0;k<listQMsCur[j].DictPatNumListMedPats[patNumCur].Count;k++) {
								EhrCqmMedicationPat mPatCur=listQMsCur[j].DictPatNumListMedPats[patNumCur][k];
								string extensCur=CqmItemAbbreviation.MedPat.ToString()+mPatCur.ValueSetOID+"Num";								
								if(mPatCur.EhrCqmMedicationPatNum!=0) {
									extensCur+=mPatCur.EhrCqmMedicationPatNum.ToString();
								}
								else {
									extensCur+=mPatCur.EhrCqmVaccinePatNum.ToString();
								}
								if(!listUniqueItemExtensionsWithValueSetOIDs.Contains(extensCur)) {
									listUniqueItemExtensionsWithValueSetOIDs.Add(extensCur);
									GenerateMedPatsEntry(mPatCur);
								}
								extensCur=CqmItemAbbreviation.MedPat.ToString();	
								if(mPatCur.EhrCqmMedicationPatNum!=0) {
									extensCur+=mPatCur.EhrCqmMedicationPatNum.ToString();
								}
								else {
									extensCur+=mPatCur.EhrCqmVaccinePatNum.ToString();
								}
								if(listUniqueItemExtensions.Contains(extensCur)) {
									continue;
								}
								listUniqueItemExtensions.Add(extensCur);
								#region BuildPlainTextVersion
								string descript="";
								string codeValue="";
								//represents a medication.  Used by tobacco cessation or above/below normal weight meds
								if(mPatCur.EhrCqmMedicationPatNum!=0) {
									codeValue=mPatCur.RxCui.ToString();
									//Will be Medication, Active/Order
									if(mPatCur.PatNote!="") {//PatNote means an order
										descript="Medication, Ordered: ";
									}
									else {
										descript="Medication, Active: ";
									}
								}
								//EhrCqmVaccinePatNum will only be set if pneumonia or influenza measure and it represents a vaccine
								else {
									codeValue=mPatCur.CVXCode;
									if(mPatCur.CompletionStatus==VaccineCompletionStatus.NotAdministered) {//NotAdministered is due to an allergy or intolerance
										descript="Medication, Allergy: ";
									}
									else {//only getting CompletionStatus=Complete or NotAdministered, the else is for Complete
										descript="Medication, Administered: ";
									}
								}
								descript+=mPatCur.ValueSetName+" - "+mPatCur.Description;
								PatientDataTextTableRow(descript,mPatCur.ValueSetOID,mPatCur.CodeSystemName,codeValue,mPatCur.DateStart,mPatCur.DateStop,"");
								#endregion BuildPlainTextVersion
							}
						}
						#endregion medicationpats
						//create ehrnotperformeds for each measure for this patient
						#region ehrnotperformeds
						for(int j=0;j<listQMsCur.Count;j++) {
							//have to determine what is not being performed, Procedure (flu vaccine, current meds documented), Medication (flu vaccine), Physical Exam, Tobacco screening
							//if ValueSetOID=2.16.840.1.113883.3.526.3.1254, then Medication, Administered not done: Medical/Patient/System Reason (flu vaccine med)
							//if ValueSetOID=2.16.840.1.113883.3.526.3.402, then Procedure, Performed not done: Medical/Patient/System Reason (flu vaccine proc)
							//if ValueSetOID=2.16.840.1.113883.3.600.1.462, then Procedure, Performed not done: Medical or Other reason not done (current meds documented proc)
							//if ValueSetOID=2.16.840.1.113883.3.600.1.681, then Physical Exam, Performed not done: Medical or Other reason not done/Patient Reason Refused (vitalsign exam)
							//if ValueSetOID=2.16.840.1.113883.3.526.3.1278, then Risk Category Assessment not done: Medical Reason (tobacco assessment)
							//Then use the negationInd="true" attribute to indicate that it was not performed
							if(!listQMsCur[j].DictPatNumListNotPerfs.ContainsKey(patNumCur)) {
								continue;
							}
							for(int k=0;k<listQMsCur[j].DictPatNumListNotPerfs[patNumCur].Count;k++) {
								EhrCqmNotPerf npCur=listQMsCur[j].DictPatNumListNotPerfs[patNumCur][k];
								string extensCur=CqmItemAbbreviation.NotPerf.ToString()+npCur.ValueSetOID+"Num"+npCur.EhrCqmNotPerfNum.ToString();
								if(!listUniqueItemExtensionsWithValueSetOIDs.Contains(extensCur)) {
									listUniqueItemExtensionsWithValueSetOIDs.Add(extensCur);
									GenerateNotPerfEntry(npCur);
								}
								extensCur=CqmItemAbbreviation.NotPerf.ToString()+npCur.EhrCqmNotPerfNum.ToString();
								if(listUniqueItemExtensions.Contains(extensCur)) {
									continue;
								}
								listUniqueItemExtensionsWithValueSetOIDs.Add(extensCur);
								#region BuildPlainTextVersion
								string descript="";
								if(npCur.ValueSetOID=="2.16.840.1.113883.3.526.3.1254") {
									descript="Medication, Administered not done: ";
								}
								else if(npCur.ValueSetOID=="2.16.840.1.113883.3.526.3.402" || npCur.ValueSetOID=="2.16.840.1.113883.3.600.1.462") {
									descript="Procedure, Performed not done: ";
								}
								else if(npCur.ValueSetOID=="2.16.840.1.113883.3.600.1.681") {
									descript="Physical Exam, Performed not done: ";
								}
								else {//must be a 2.16.840.1.113883.3.526.3.1278, not performed items restricted to one of these 5 value sets
									descript="Risk Category Assessment not done: ";
								}
								descript+=npCur.ValueSetName+" - "+npCur.DescriptionReason;
								PatientDataTextTableRow(descript,npCur.ValueSetOID,npCur.CodeSystemName,npCur.CodeValue,npCur.DateEntry,npCur.DateEntry,"");
								#endregion BuildPlainTextVersion
							}
						}
						#endregion ehrnotperformeds
						//create problems for each measure for this patient
						#region problems
						for(int j=0;j<listQMsCur.Count;j++) {
							//if ValueSetOID = 2.16.840.1.113883.3.526.3.1255, SNOMEDCT - 315640000 - Influenza vaccination declined (situation), then it is a Communication, From patient to provider:
							//if ValueSetOID = 2.16.840.1.113883.3.464.1003.110.12.1028, SNOMEDCT - 310578008 - Pneumococcal vaccination given (finding), then it is a Risk Category Assessment:
							//if ValueSetOID = 2.16.840.1.113883.3.600.1.1579 - Palliative Care, then it is a Procedure, Order:
							//Otherwise it is a Diagnosis, Active:
							if(!listQMsCur[j].DictPatNumListProblems.ContainsKey(patNumCur)) {
								continue;
							}
							for(int k=0;k<listQMsCur[j].DictPatNumListProblems[patNumCur].Count;k++) {
								EhrCqmProblem probCur=listQMsCur[j].DictPatNumListProblems[patNumCur][k];
								string extensCur=CqmItemAbbreviation.Prob.ToString()+probCur.ValueSetOID+"Num"+probCur.EhrCqmProblemNum.ToString();
								if(!listUniqueItemExtensionsWithValueSetOIDs.Contains(extensCur)) {
									listUniqueItemExtensionsWithValueSetOIDs.Add(extensCur);
									switch(probCur.ValueSetOID) {
										case "2.16.840.1.113883.3.526.3.1255":
											GenerateCommunicationEntry(probCur);
											break;
										case "2.16.840.1.113883.3.464.1003.110.12.1028":
											GenerateRiskAssessEntry(probCur);
											break;
										case "2.16.840.1.113883.3.600.1.1579":
											GenerateProcedureEntry(null,probCur);
											break;
										default:
											GenerateDiagnosisEntry(probCur);
											break;
									}
								}
								extensCur=CqmItemAbbreviation.Prob.ToString()+probCur.EhrCqmProblemNum.ToString();
								if(listUniqueItemExtensions.Contains(extensCur)) {
									continue;
								}
								listUniqueItemExtensions.Add(extensCur);
								#region BuildPlainTextVersion
								string descript="";
								switch(probCur.ValueSetOID) {
									case "2.16.840.1.113883.3.526.3.1255":
										descript="Communication, From patient to provider: ";
										break;
									case "2.16.840.1.113883.3.464.1003.110.12.1028":
										descript="Risk Category Assessment: ";
										break;
									case "2.16.840.1.113883.3.600.1.1579":
										descript="Procedure, Order: ";
										break;
									default:
										descript="Diagnosis, Active: ";
										break;
								}
								descript+=probCur.ValueSetName+" - "+probCur.Description;
								PatientDataTextTableRow(descript,probCur.ValueSetOID,probCur.CodeSystemName,probCur.CodeValue,probCur.DateStart,probCur.DateStop,"");
								#endregion BuildPlainTextVersion
							}
						}
						#endregion problems
						//create procs for each measure for this patient
						#region procs
						for(int j=0;j<listQMsCur.Count;j++) {
							if(!listQMsCur[j].DictPatNumListProcs.ContainsKey(patNumCur)) {
								continue;
							}
							for(int k=0;k<listQMsCur[j].DictPatNumListProcs[patNumCur].Count;k++) {
								EhrCqmProc procCur=listQMsCur[j].DictPatNumListProcs[patNumCur][k];
								string extensCur=CqmItemAbbreviation.Proc.ToString()+procCur.ValueSetOID+"Num"+procCur.EhrCqmProcNum.ToString();
								if(!listUniqueItemExtensionsWithValueSetOIDs.Contains(extensCur)) {
									listUniqueItemExtensionsWithValueSetOIDs.Add(extensCur);
									GenerateProcedureEntry(procCur,null);
								}
								extensCur=CqmItemAbbreviation.Proc.ToString()+procCur.EhrCqmProcNum.ToString();
								if(listUniqueItemExtensions.Contains(extensCur)) {
									continue;
								}
								listUniqueItemExtensions.Add(extensCur);
								#region BuildPlainTextVersion
								string descript="Procedure, Performed: "+procCur.ValueSetName+" - "+procCur.Description;
								PatientDataTextTableRow(descript,procCur.ValueSetOID,procCur.CodeSystemName,procCur.ProcCode,procCur.ProcDate,procCur.ProcDate,"");
								#endregion BuildPlainTextVersion
							}
						}
						#endregion procs
						//create vitalsigns for each measure for this patient
						#region vitalsigns
						for(int j=0;j<listQMsCur.Count;j++) {
							if(!listQMsCur[j].DictPatNumListVitalsigns.ContainsKey(patNumCur)) {
								continue;
							}
							for(int k=0;k<listQMsCur[j].DictPatNumListVitalsigns[patNumCur].Count;k++) {
								EhrCqmVitalsign vCur=listQMsCur[j].DictPatNumListVitalsigns[patNumCur][k];
								string extensCur="";
								if(vCur.BpDiastolic>0) {
									extensCur=CqmItemAbbreviation.Vital.ToString()+"2.16.840.1.113883.3.526.3.1033"+"Num"+vCur.EhrCqmVitalsignNum.ToString();
									if(!listUniqueItemExtensionsWithValueSetOIDs.Contains(extensCur)) {
										listUniqueItemExtensionsWithValueSetOIDs.Add(extensCur);
										GenerateVitalsignEntry(vCur,"BPd");
									}
								}
								if(vCur.BpSystolic>0) {
									extensCur=CqmItemAbbreviation.Vital.ToString()+"2.16.840.1.113883.3.526.3.1032"+"Num"+vCur.EhrCqmVitalsignNum.ToString();
									if(!listUniqueItemExtensionsWithValueSetOIDs.Contains(extensCur)) {
										listUniqueItemExtensionsWithValueSetOIDs.Add(extensCur);
										GenerateVitalsignEntry(vCur,"BPs");
									}
								}
								if(vCur.BMI>0) {
									extensCur=CqmItemAbbreviation.Vital.ToString()+"2.16.840.1.113883.3.464.1003.121.12.1014"+"Num"+vCur.EhrCqmVitalsignNum.ToString();
									if(!listUniqueItemExtensionsWithValueSetOIDs.Contains(extensCur)) {
										listUniqueItemExtensionsWithValueSetOIDs.Add(extensCur);
										GenerateVitalsignEntry(vCur,"Ht");
									}
									extensCur=CqmItemAbbreviation.Vital.ToString()+"2.16.840.1.113883.3.464.1003.121.12.1015"+"Num"+vCur.EhrCqmVitalsignNum.ToString();
									if(!listUniqueItemExtensionsWithValueSetOIDs.Contains(extensCur)) {
										listUniqueItemExtensionsWithValueSetOIDs.Add(extensCur);
										GenerateVitalsignEntry(vCur,"Wt");
									}
									extensCur=CqmItemAbbreviation.Vital.ToString()+"2.16.840.1.113883.3.600.1.681"+vCur.EhrCqmVitalsignNum.ToString();
									if(!listUniqueItemExtensionsWithValueSetOIDs.Contains(extensCur)) {
										listUniqueItemExtensionsWithValueSetOIDs.Add(extensCur);
										GenerateVitalsignEntry(vCur,"BMI");
									}
								}
								if(vCur.BMIPercentile>-1) {
									extensCur=CqmItemAbbreviation.Vital.ToString()+"2.16.840.1.113883.3.464.1003.121.12.1012"+"Num"+vCur.EhrCqmVitalsignNum.ToString();
									if(!listUniqueItemExtensionsWithValueSetOIDs.Contains(extensCur)) {
										listUniqueItemExtensionsWithValueSetOIDs.Add(extensCur);
										GenerateVitalsignEntry(vCur,"BMIp");
									}
								}
								extensCur=CqmItemAbbreviation.Vital.ToString()+vCur.EhrCqmVitalsignNum.ToString();
								if(listUniqueItemExtensions.Contains(extensCur)) {
									continue;
								}
								listUniqueItemExtensions.Add(extensCur);
								#region BuildPlainTextVersion
								string descript="";
								if(vCur.BpDiastolic>0) {
									descript="Physical Exam, Finding: Diastolic Blood Pressure";
									PatientDataTextTableRow(descript,"2.16.840.1.113883.3.526.3.1033","LOINC","8462-4",vCur.DateTaken,vCur.DateTaken,vCur.BpDiastolic.ToString()+" mmHg");
								}
								if(vCur.BpSystolic>0) {
									descript="Physical Exam, Finding: Systolic Blood Pressure";
									PatientDataTextTableRow(descript,"2.16.840.1.113883.3.526.3.1032","LOINC","8480-6",vCur.DateTaken,vCur.DateTaken,vCur.BpSystolic.ToString()+" mmHg");
								}
								if(vCur.BMI>0) {
									descript="Physical Exam, Finding: Height";
									PatientDataTextTableRow(descript,"2.16.840.1.113883.3.464.1003.121.12.1014","LOINC",vCur.HeightExamCode,vCur.DateTaken,vCur.DateTaken,vCur.Height.ToString()+" in");
									descript="Physical Exam, Finding: Weight";
									PatientDataTextTableRow(descript,"2.16.840.1.113883.3.464.1003.121.12.1015","LOINC",vCur.WeightExamCode,vCur.DateTaken,vCur.DateTaken,vCur.Weight.ToString()+" lb");
									descript="Physical Exam, Finding: BMI LOINC Value";
									PatientDataTextTableRow(descript,"2.16.840.1.113883.3.600.1.681","LOINC","39156-5",vCur.DateTaken,vCur.DateTaken,vCur.BMI.ToString("0.00")+" kg/m2");
								}
								if(vCur.BMIPercentile>-1) {
									descript="Physical Exam, Finding: BMI Percentile";
									PatientDataTextTableRow(descript,"2.16.840.1.113883.3.464.1003.121.12.1012","LOINC",vCur.BMIExamCode,vCur.DateTaken,vCur.DateTaken,vCur.BMIPercentile.ToString());
								}
								#endregion BuildPlainTextVersion
							}
						}
						#endregion vitalsigns
						_x.WriteEndElement();
					}
					End("tbody");
					End("table");
					End("text");
					XmlDocument doc=new XmlDocument();
					doc.LoadXml(strBuilderPatDataEntries.ToString());
					XmlElement elementClinicalDocument=doc.DocumentElement;
					for(int n=0;n<elementClinicalDocument.ChildNodes.Count;n++) {
						elementClinicalDocument.ChildNodes[n].WriteTo(_w);
					}
					Start("entry");
					Start("observation","classCode","OBS","moodCode","EVN");
					_w.WriteComment("Patient Characteristic Payer Template");
					TemplateId("2.16.840.1.113883.10.20.24.3.55");
					StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.Payer.ToString()+patCur.PatNum.ToString());
					StartAndEnd("code","code","48768-6","displayName","Payment source","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
					StartAndEnd("statusCode","code","completed");
					Start("value");
					_w.WriteAttributeString("xsi","type",null,"CD");
					Attribs("code",ehrPatCur.PayorSopCode,"displayName",ehrPatCur.PayorDescription,"codeSystem","2.16.840.1.113883.3.221.5","codeSystemName","Source of Payment Typology");
					_w.WriteAttributeString("sdtc","valueSet",null,ehrPatCur.PayorValueSetOID);
					End("value");
					End("observation");
					End("entry");
					End("section");
					End("component");
					#endregion Patient Data
					End("structuredBody");
					#endregion QRDA I Body
					End("component");
					End("ClinicalDocument");
				}
				string xmlResult=strBuilder.ToString();
				dictPatNumXml.Add(patNumCur,xmlResult);
				SecurityLogs.MakeLogEntry(Permissions.Copy,patNumCur,"QRDA Category I generated");//Create audit log entry.
			}
			_w.Flush();
			_w.Close();
			if(ODBuild.IsDebug()) {
				swGenerateXMLs.Stop();
				swCreateZipFiles.Restart();
			}
			for(int i=0;i<listAllEMeasureNums.Count;i++) {//make a file for each of our 9 eMeasures
				using(ZipFile zipCur=new ZipFile()) {
					for(int j=0;j<listAllEhrPats.Count;j++) {//go through list of all unique patients that are in any of the IPPs
						Patient patCur=listAllEhrPats[j].EhrCqmPat;
						if(!dictPatNumXml.ContainsKey(patCur.PatNum)) {//just in case, every patient will be in the dictionary.  Dictionary links a PatNum key to the cat I QRDA xml string value
							continue;
						}
						if(!dictPatNumListEMeasureNums.ContainsKey(patCur.PatNum)) {//just in case, every patient in listAllEhrPats will be in the dictionary linked to their eMeasureNums
							continue;
						}
						if(dictPatNumListEMeasureNums[patCur.PatNum].Contains(listAllEMeasureNums[i])) {
							zipCur.AddEntry(patCur.PatNum.ToString()+"_"+patCur.LName+"_"+patCur.FName+".xml",dictPatNumXml[patCur.PatNum]);
						}
					}
					zipCur.Save(folderRoot+"\\Measure_"+listAllEMeasureNums[i]+".zip");
				}
			}
			if(ODBuild.IsDebug()) {
				swCreateZipFiles.Stop();
				System.Windows.Forms.MessageBox.Show("Generating XMLs: "+swGenerateXMLs.Elapsed.ToString()+"\r\n"+"Creating zip files: "+swCreateZipFiles.Elapsed.ToString());
			}
			#endregion Cateogry I QRDA Documents
		}

		///<summary>List of dictionaries that hold the count of supplemental data for each population.
		///<para>Send in the measure IPP, which may be 0, and the list will be filled with supplemental counts for each population.</para>
		///<para>Use the index of the dictionary to get the counts desired.</para>
		///<para>0=IPP, 1=Denominator, 2=Denominator Exclusions, 3=Numerator, 4=Denominator Exceptions.</para>
		///<para>Example, for counts of supplemental data for the denominator, get the dict[1].  dict[1] will have relevant counts for race, gender, ethnicity, and payer.</para></summary>
		private static List<Dictionary<string,int>> FillDictPopCounts(List<EhrCqmPatient> listPats,Dictionary<long,List<EhrCqmEncounter>> dictPatNumListEncs) {
			List<Dictionary<string,int>> listDicts=new List<Dictionary<string,int>>();
			Dictionary<string,int> dictIPPCounts=new Dictionary<string,int>();
			Dictionary<string,int> dictDenomCounts=new Dictionary<string,int>();
			Dictionary<string,int> dictDenomExclusCounts=new Dictionary<string,int>();
			Dictionary<string,int> dictNumeCounts=new Dictionary<string,int>();
			Dictionary<string,int> dictDenomExceptCounts=new Dictionary<string,int>();
			List<EhrCqmPatient> listDenom=new List<EhrCqmPatient>();
			List<EhrCqmPatient> listDenomExclus=new List<EhrCqmPatient>();
			List<EhrCqmPatient> listNumerator=new List<EhrCqmPatient>();
			List<EhrCqmPatient> listDenomExcept=new List<EhrCqmPatient>();
			if(dictPatNumListEncs!=null) {//this will only be sent in for measure 68, current meds documented, since it is encounter based not patient based
				for(int i=0;i<listPats.Count;i++) {
					List<EhrCqmEncounter> listEncsCur=dictPatNumListEncs[listPats[i].EhrCqmPat.PatNum];
					for(int j=0;j<listEncsCur.Count;j++) {
						listDenom.Add(listPats[i].Copy());
						if(listEncsCur[j].IsException) {
							listDenomExcept.Add(listPats[i].Copy());
						}
						if(listEncsCur[j].IsNumerator) {
							listNumerator.Add(listPats[i].Copy());
						}
						//no exclusions for measure 68
					}
				}
			}
			else {
				for(int j=0;j<listPats.Count;j++) {
					if(listPats[j].IsDenominator) {
						listDenom.Add(listPats[j].Copy());
					}
					if(listPats[j].IsExclusion) {
						listDenomExclus.Add(listPats[j].Copy());
					}
					if(listPats[j].IsNumerator) {
						listNumerator.Add(listPats[j].Copy());
					}
					if(listPats[j].IsException) {
						listDenomExcept.Add(listPats[j].Copy());
					}
				}
			}
			if(dictPatNumListEncs!=null) {
				dictIPPCounts=GetSupplementalCounts(listDenom);
			}
			else {
				dictIPPCounts=GetSupplementalCounts(listPats);
			}
			dictDenomCounts=GetSupplementalCounts(listDenom);
			dictDenomExclusCounts=GetSupplementalCounts(listDenomExclus);
			dictNumeCounts=GetSupplementalCounts(listNumerator);
			dictDenomExceptCounts=GetSupplementalCounts(listDenomExcept);
			listDicts.Add(dictIPPCounts);
			listDicts.Add(dictDenomCounts);
			listDicts.Add(dictDenomExclusCounts);
			listDicts.Add(dictNumeCounts);
			listDicts.Add(dictDenomExceptCounts);
			return listDicts;
		}

		private static string GetSupplementDataPrintString(string s) {
			switch(s) {
				case "AfricanAmerican":
					return "Black or African American";
				case "AmericanIndian":
					return "American Indian or Alaska Native";
				case "HawaiiOrPacIsland":
					return "Native Hawaiian or Other Pacific Islander";
				case "Other":
					return "Other Race";
				case "Hispanic":
					return "Hispanic or Latino";
				case "NotHispanic":
					return "Not Hispanic or Latino";
				case "Unknown":
					return "Undifferentiated";
				case "Other Gvmt":
					return "Other Government (Federal/State/Local) (excluding Department of Corrections)";
				case "Private":
					return "Private Insurance";
				case PatientRace.DECLINE_SPECIFY_RACE_CODE:
				case PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE:
					return "ASKU";
				default:
					return s;
			}
		}

		///<summary>Gets count of gender, race, ethnicity, and payer type.</summary>
		private static Dictionary<string,int> GetSupplementalCounts(List<EhrCqmPatient> listPats) {
			Dictionary<string,int> dictCategoryCount=new Dictionary<string,int>();
			string otherRaceCode="2131-1";//CdcrecCode for 'OTHER RACE'
			//add races, ethnicities, and genders to the dictionary with starting counts of 0
			dictCategoryCount.Add("All",listPats.Count);
			dictCategoryCount.Add(otherRaceCode,0);
			dictCategoryCount.Add(PatientGender.Male.ToString(),0);
			dictCategoryCount.Add(PatientGender.Female.ToString(),0);
			dictCategoryCount.Add(PatientGender.Unknown.ToString(),0);
			dictCategoryCount.Add("Medicare",0);
			dictCategoryCount.Add("Medicaid",0);
			dictCategoryCount.Add("Other Gvmt",0);
			dictCategoryCount.Add("Private",0);
			dictCategoryCount.Add("Self-pay",0);
			List<long> listPatNumsCounted=new List<long>();
			for(int i=0;i<listPats.Count;i++) {
				if(listPatNumsCounted.Contains(listPats[i].EhrCqmPat.PatNum)) {//for measure 68, where listPats can contain the same patient multiple times for each encounter
					continue;
				}
				else {
					listPatNumsCounted.Add(listPats[i].EhrCqmPat.PatNum);
				}
				if(listPats[i].ListPatientRaces.Count==1
					&& listPats[i].ListPatientRaces[0].CdcrecCode!=PatientRace.DECLINE_SPECIFY_RACE_CODE
					&& listPats[i].ListPatientRaces[0].CdcrecCode!=PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE) 
				{
					if(dictCategoryCount.ContainsKey(listPats[i].ListPatientRaces[0].CdcrecCode)) {
						dictCategoryCount[listPats[i].ListPatientRaces[0].CdcrecCode]++;
					}
					else { 
						dictCategoryCount.Add(listPats[i].ListPatientRaces[0].CdcrecCode,1);
					}
				}
				else if(listPats[i].ListPatientRaces.Count>1) {
					dictCategoryCount[otherRaceCode]++;
				}
				if(listPats[i].Ethnicity!=null 
					&& listPats[i].Ethnicity.CdcrecCode!=PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE) 
				{
					if(dictCategoryCount.ContainsKey(listPats[i].Ethnicity.CdcrecCode)) {
						dictCategoryCount[listPats[i].Ethnicity.CdcrecCode]++;
					}
					else {
						dictCategoryCount.Add(listPats[i].Ethnicity.CdcrecCode,1);
					}
				}
				dictCategoryCount[listPats[i].EhrCqmPat.Gender.ToString()]++;
				if(listPats[i].PayorSopCode==null || listPats[i].PayorSopCode=="") {
					//no payer type set, Self-pay
					dictCategoryCount["Self-pay"]++;
				}
				else {
					switch(listPats[i].PayorSopCode.Substring(0,1)) {//Get hierarchical code representing payer type main category, first character is type
						case "1":
							dictCategoryCount["Medicare"]++;
							break;
						case "2":
							dictCategoryCount["Medicaid"]++;
							break;
						case "3":
							dictCategoryCount["Other Gvmt"]++;
							break;
						case "5":
							dictCategoryCount["Private"]++;
							break;
						case "8":
							dictCategoryCount["Self-pay"]++;
							break;
						default:
							dictCategoryCount["Self-pay"]++;
							break;
					}
				}
			}
			return dictCategoryCount;
		}

		///<summary>Generates one encounter xml entry and uses the global writer _x instead of the main writer _w.</summary>
		private static void GenerateEncounterEntry(EhrCqmEncounter encCur) {
			_isWriterW=false;
			Start("entry","typeCode","DRIV");
			Start("encounter","classCode","ENC","moodCode","EVN");
			_x.WriteComment("Encounter Activities Template");
			TemplateId("2.16.840.1.113883.10.20.22.4.49");
			_x.WriteComment("Encounter Performed Template");
			TemplateId("2.16.840.1.113883.10.20.24.3.23");
			StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.Enc.ToString()+encCur.EhrCqmEncounterNum.ToString());
			Start("code");
			Attribs("code",encCur.CodeValue,"displayName",encCur.Description,"codeSystem",encCur.CodeSystemOID,"codeSystemName",encCur.CodeSystemName);
			_x.WriteAttributeString("sdtc","valueSet",null,encCur.ValueSetOID);
			End("code");
			_x.WriteElementString("text","Encounter, Performed: "+encCur.ValueSetName);
			StartAndEnd("statusCode","code","completed");
			_x.WriteComment("Length of Stay");
			Start("effectiveTime");
			DateElement("low",encCur.DateEncounter);
			DateElement("high",encCur.DateEncounter);
			End("effectiveTime");
			End("encounter");
			End("entry");
			_isWriterW=true;
		}

		private static void GenerateVitalsignEntry(EhrCqmVitalsign vCur,string vType) {
			_isWriterW=false;
			Start("entry","typeCode","DRIV");
			Start("observation","classCode","OBS","moodCode","EVN");
			_x.WriteComment("Result Observation Template");
			TemplateId("2.16.840.1.113883.10.20.22.4.2");
			_x.WriteComment("Physical Exam Finding Template");
			TemplateId("2.16.840.1.113883.10.20.24.3.57");
			StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.Vital.ToString()+vType+vCur.EhrCqmVitalsignNum.ToString());
			switch(vType) {
				//for BP Diastolic exam: CodeValue=8462-4, CodeSystemName=LOINC, CodeSystemOID=2.16.840.1.113883.6.1, Description=Diastolic blood pressure, ValueSetName=Diastolic Blood Pressure, ValueSetOID=2.16.840.1.113883.3.526.3.1033
				case "BPd":
					Start("code","code","8462-4","displayName","Diastolic blood pressure","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
					_x.WriteAttributeString("sdtc","valueSet",null,"2.16.840.1.113883.3.526.3.1033");
					End("code");
					_x.WriteElementString("text","Physical Exam, Finding: Diastolic Blood Pressure");
					StartAndEnd("statusCode","code","completed");
					Start("effectiveTime");
					DateElement("low",vCur.DateTaken);
					DateElement("high",vCur.DateTaken);
					End("effectiveTime");
					Start("value");
					_x.WriteAttributeString("xsi","type",null,"PQ");
					Attribs("value",vCur.BpDiastolic.ToString(),"unit","mmHg");
					End("value");
					break;
				//for BP Systolic exam: CodeValue=8480-6, CodeSystemName=LOINC, CodeSystemOID=2.16.840.1.113883.6.1, Description=Systolic blood pressure, ValueSetName=Systolic Blood Pressure, ValueSetOID=2.16.840.1.113883.3.526.3.1032
				case "BPs":
					Start("code","code","8480-6","displayName","Systolic Blood Pressure","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
					_x.WriteAttributeString("sdtc","valueSet",null,"2.16.840.1.113883.3.526.3.1032");
					End("code");
					_x.WriteElementString("text","Physical Exam, Finding: Systolic Blood Pressure");
					StartAndEnd("statusCode","code","completed");
					Start("effectiveTime");
					DateElement("low",vCur.DateTaken);
					DateElement("high",vCur.DateTaken);
					End("effectiveTime");
					Start("value");
					_x.WriteAttributeString("xsi","type",null,"PQ");
					Attribs("value",vCur.BpSystolic.ToString(),"unit","mmHg");
					End("value");
					break;
				//ValueSetOID=2.16.840.1.113883.3.464.1003.121.12.1014
				case "Ht":
					Start("code","code",vCur.HeightExamCode,"displayName",vCur.HeightExamDescript,"codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
					_x.WriteAttributeString("sdtc","valueSet",null,"2.16.840.1.113883.3.464.1003.121.12.1014");
					End("code");
					_x.WriteElementString("text","Physical Exam, Finding: Height");
					StartAndEnd("statusCode","code","completed");
					Start("effectiveTime");
					DateElement("low",vCur.DateTaken);
					DateElement("high",vCur.DateTaken);
					End("effectiveTime");
					Start("value");
					_x.WriteAttributeString("xsi","type",null,"PQ");
					Attribs("value",vCur.Height.ToString(),"unit","[in_us]");
					End("value");
					break;
				//ValueSetOID=2.16.840.1.113883.3.464.1003.121.12.1015
				case "Wt":
					Start("code","code",vCur.WeightExamCode,"displayName",vCur.WeightExamDescript,"codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
					_x.WriteAttributeString("sdtc","valueSet",null,"2.16.840.1.113883.3.464.1003.121.12.1015");
					End("code");
					_x.WriteElementString("text","Physical Exam, Finding: Weight");
					StartAndEnd("statusCode","code","completed");
					Start("effectiveTime");
					DateElement("low",vCur.DateTaken);
					DateElement("high",vCur.DateTaken);
					End("effectiveTime");
					Start("value");
					_x.WriteAttributeString("xsi","type",null,"PQ");
					Attribs("value",vCur.Weight.ToString(),"unit","[lb_av]");
					End("value");
					break;
				//in kg/m2, if valid BMI value: CodeValue=39156-5, CodeSystemName=LOINC, CodeSystemOID=2.16.840.1.113883.6.1, Description=Body mass index (BMI) [Ratio], ValueSetName=BMI LOINC Value, ValueSetOID=2.16.840.1.113883.3.600.1.681
				case "BMI":
					Start("code","code","39156-5","displayName","Body mass index (BMI) [Ratio]","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
					_x.WriteAttributeString("sdtc","valueSet",null,"2.16.840.1.113883.3.600.1.681");
					End("code");
					_x.WriteElementString("text","Physical Exam, Finding: BMI LOINC Value");
					StartAndEnd("statusCode","code","completed");
					Start("effectiveTime");
					DateElement("low",vCur.DateTaken);
					DateElement("high",vCur.DateTaken);
					End("effectiveTime");
					Start("value");
					_x.WriteAttributeString("xsi","type",null,"PQ");
					Attribs("value",vCur.BMI.ToString(),"unit","kg/m2");
					End("value");
					break;
				//ValueSetOID=2.16.840.1.113883.3.464.1003.121.12.1012
				case "BMIp":
					Start("code","code",vCur.BMIExamCode,"displayName",vCur.BMIPercentileDescript,"codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
					_x.WriteAttributeString("sdtc","valueSet",null,"2.16.840.1.113883.3.464.1003.121.12.1012");
					End("code");
					_x.WriteElementString("text","Physical Exam, Finding: BMI Percentile");
					StartAndEnd("statusCode","code","completed");
					Start("effectiveTime");
					DateElement("low",vCur.DateTaken);
					DateElement("high",vCur.DateTaken);
					End("effectiveTime");
					Start("value");
					_x.WriteAttributeString("xsi","type",null,"PQ");
					Attribs("value",vCur.BMIPercentile.ToString(),"unit","{percentile}");
					End("value");
					break;
			}
			End("observation");
			End("entry");
			_isWriterW=true;
		}

		private static void GenerateDiagnosisEntry(EhrCqmProblem probCur) {
			_isWriterW=false;
			Start("entry","typeCode","DRIV");
			Start("observation","classCode","OBS","moodCode","EVN");
			_x.WriteComment("Problem Observation Template");
			TemplateId("2.16.840.1.113883.10.20.22.4.4");
			_x.WriteComment("Diagnosis Active Template");
			TemplateId("2.16.840.1.113883.10.20.24.3.11");
			StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.Prob.ToString()+probCur.EhrCqmProblemNum.ToString());
			StartAndEnd("code","code","282291009","displayName","diagnosis","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
			_x.WriteElementString("text","Diagnosis Active: "+probCur.ValueSetName);
			StartAndEnd("statusCode","code","completed");
			Start("effectiveTime");
			_x.WriteComment("Attribute: Start Datetime");
			DateElement("low",probCur.DateStart);
			_x.WriteComment("Attribute: Stop Datetime");
			DateElement("high",probCur.DateStop);
			End("effectiveTime");
			Start("value");
			_x.WriteAttributeString("xsi","type",null,"CD");
			Attribs("code",probCur.CodeValue,"displayName",probCur.Description,"codeSystem",probCur.CodeSystemOID,"codeSystemName",probCur.CodeSystemName);
			_x.WriteAttributeString("sdtc","valueSet",null,probCur.ValueSetOID);
			End("value");
			_x.WriteComment("Status");
			Start("entryRelationship","typeCode","REFR");
			Start("observation","classCode","OBS","moodCode","EVN");
			_x.WriteComment("Problem Status Template");
			TemplateId("2.16.840.1.113883.10.20.22.4.6");
			_x.WriteComment("Problem Status Active Template");
			TemplateId("2.16.840.1.113883.10.20.24.3.94");
			StartAndEnd("code","code","33999-4","displayName","Status","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
			StartAndEnd("statusCode","code","completed");
			Start("value");
			_x.WriteAttributeString("xsi","type",null,"CD");
			Attribs("code","55561003","displayName","active","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
			End("value");
			End("observation");
			End("entryRelationship");
			End("observation");
			End("entry");
			_isWriterW=true;
		}

		///<summary>Either a proc or a problem will be sent in, the other will be null.  If probCur then Procedure, Order: item, if procCur then Procedure, Perforemed: item.</summary>
		private static void GenerateProcedureEntry(EhrCqmProc procCur,EhrCqmProblem probCur) {
			_isWriterW=false;
			Start("entry","typeCode","DRIV");
			if(probCur!=null) {
				Start("procedure","classCode","PROC","moodCode","RQO");
				_x.WriteComment("Plan of Care Activity Procedure Template");
				TemplateId("2.16.840.1.113883.10.20.22.4.41");
				_x.WriteComment("QRDA Procedure Order Template");
				TemplateId("2.16.840.1.113883.10.20.24.3.63");
				StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.Prob.ToString()+probCur.EhrCqmProblemNum.ToString());
				Start("code","code",probCur.CodeValue,"displayName",probCur.Description,"codeSystem",probCur.CodeSystemOID,"codeSystemName",probCur.CodeSystemName);
				_x.WriteAttributeString("sdtc","valueSet",null,probCur.ValueSetOID);
				End("code");
				_x.WriteElementString("text","Procedure, Order: "+probCur.ValueSetName);
				StartAndEnd("statusCode","code","completed");
				_x.WriteComment("Attribute: datetime");
				Start("author");
				TimeElement("time",probCur.DateStart);
				Start("assignedAuthor");
				StartAndEnd("id","nullFlavor","NA");
				End("assignedAuthor");
				End("author");
				End("procedure");
			}
			if(procCur!=null) {
				Start("procedure","classCode","PROC","moodCode","EVN");
				_x.WriteComment("Procedure Activity Procedure Template");
				TemplateId("2.16.840.1.113883.10.20.22.4.14");
				_x.WriteComment("Procedure Performed Template");
				TemplateId("2.16.840.1.113883.10.20.24.3.64");
				StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.Proc.ToString()+procCur.EhrCqmProcNum.ToString());
				Start("code","code",procCur.ProcCode,"displayName",procCur.Description,"codeSystem",procCur.CodeSystemOID,"codeSystemName",procCur.CodeSystemName);
				_x.WriteAttributeString("sdtc","valueSet",null,procCur.ValueSetOID);
				End("code");
				_x.WriteElementString("text","Procedure, Performed: "+procCur.ValueSetName);
				StartAndEnd("statusCode","code","completed");
				Start("effectiveTime");
				_x.WriteComment("Attribute: Start Datetime");
				DateElement("low",procCur.ProcDate);
				_x.WriteComment("Attribute: Stop Datetime");
				DateElement("high",procCur.ProcDate);
				End("effectiveTime");
				End("procedure");
			}
			End("entry");
			_isWriterW=true;
		}

		private static void GenerateRiskAssessEntry(EhrCqmProblem probCur) {
			_isWriterW=false;
			Start("entry","typeCode","DRIV");
			Start("observation","classCode","OBS","moodCode","EVN");
			_x.WriteComment("Assessment Scale Observation Template");
			TemplateId("2.16.840.1.113883.10.20.22.4.69");
			_x.WriteComment("Risk Category Assessment Template");
			TemplateId("2.16.840.1.113883.10.20.24.3.69");
			StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.Prob.ToString()+probCur.EhrCqmProblemNum.ToString());
			Start("code","code",probCur.CodeValue,"displayName",probCur.Description,"codeSystem",probCur.CodeSystemOID,"codeSystemName",probCur.CodeSystemName);
			_x.WriteAttributeString("sdtc","valueSet",null,probCur.ValueSetOID);
			End("code");
			_x.WriteElementString("text","Risk Category Assessment: "+probCur.ValueSetName);
			StartAndEnd("statusCode","code","completed");
			Start("effectiveTime");
			_x.WriteComment("Attribute: Start Datetime");
			DateElement("low",probCur.DateStart);
			_x.WriteComment("Attribute: Stop Datetime");
			DateElement("high",probCur.DateStop);
			End("effectiveTime");
			End("observation");
			End("entry");
			_isWriterW=true;
		}

		private static void GenerateCommunicationEntry(EhrCqmProblem probCur) {
			_isWriterW=false;
			Start("entry","typeCode","DRIV");
			Start("act","classCode","ACT","moodCode","EVN");
			_x.WriteComment("Communication from Patient to Provider Template");
			TemplateId("2.16.840.1.113883.10.20.24.3.2");
			StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.Prob.ToString()+probCur.EhrCqmProblemNum.ToString());
			Start("code","code",probCur.CodeValue,"displayName",probCur.Description,"codeSystem",probCur.CodeSystemOID,"codeSystemName",probCur.CodeSystemName);
			_x.WriteAttributeString("sdtc","valueSet",null,probCur.ValueSetOID);
			End("code");
			_x.WriteElementString("text","Communication, From patient to provider: "+probCur.ValueSetName);
			StartAndEnd("statusCode","code","completed");
			Start("effectiveTime");
			DateElement("low",probCur.DateStart);
			End("effectiveTime");
			Start("participant","typeCode","AUT");
			Start("participantRole","classCode","PAT");
			StartAndEnd("code","code","116154003","displayName","Patient","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
			End("participantRole");
			End("participant");
			Start("participant","typeCode","IRCP");
			Start("participantRole","classCode","ASSIGNED");
			StartAndEnd("code","code","158965000","displayName","Medical Practitioner","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
			End("participantRole");
			End("participant");
			End("act");
			End("entry");
			_isWriterW=true;
		}

		///<summary>
		///<para>if ValueSetOID=2.16.840.1.113883.3.526.3.1254, then Medication, Administered not done: Medical/Patient/System Reason (flu vaccine med)</para>
		///<para>if ValueSetOID=2.16.840.1.113883.3.526.3.402, then Procedure, Performed not done: Medical/Patient/System Reason (flu vaccine proc)</para>
		///<para>if ValueSetOID=2.16.840.1.113883.3.600.1.462, then Procedure, Performed not done: Medical or Other reason not done (current meds documented proc)</para>
		///<para>if ValueSetOID=2.16.840.1.113883.3.600.1.681, then Physical Exam, Performed not done: Medical or Other reason not done/Patient Reason Refused (vitalsign exam)</para>
		///<para>if ValueSetOID=2.16.840.1.113883.3.526.3.1278, then Risk Category Assessment not done: Medical Reason (tobacco assessment)</para>
		///<para>Then use the negationInd="true" attribute to indicate that it was not performed</para></summary>
		private static void GenerateNotPerfEntry(EhrCqmNotPerf npCur) {
			_isWriterW=false;
			Start("entry","typeCode","DRIV");
			if(npCur.ValueSetOID=="2.16.840.1.113883.3.526.3.1254") {//flu vaccine medication administered not done
				Start("act","classCode","ACT","moodCode","EVN");
				_x.WriteComment("Medication Administered Template");
				TemplateId("2.16.840.1.113883.10.20.24.3.42");
				StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.NotPerf.ToString()+npCur.EhrCqmNotPerfNum.ToString());
				StartAndEnd("code","code","416118004","displayName","Administration","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
				StartAndEnd("statusCode","code","completed");
				StartAndEnd("effectiveTime","nullFlavor","NI");
				Start("entryRelationship","typeCode","COMP");
				Start("substanceAdministration","classCode","SBADM","moodCode","EVN","negationInd","true");
				_x.WriteComment("Medication Activity Template");
				TemplateId("2.16.840.1.113883.10.20.22.4.16");
				StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.NotPerf.ToString()+npCur.EhrCqmNotPerfNum.ToString()+npCur.PatNum.ToString());
				_x.WriteElementString("text","Medication Administered, not done: "+npCur.ValueSetName);
				StartAndEnd("statusCode","code","completed");
				Start("effectiveTime");
				DateElement("low",npCur.DateEntry);
				End("effectiveTime");
				Start("consumable");
				Start("manufacturedProduct","classCode","MANU");
				_x.WriteComment("Medication Information Template");
				TemplateId("2.16.840.1.113883.10.20.22.4.23");
				StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.NotPerf.ToString()+npCur.EhrCqmNotPerfNum.ToString()+npCur.CodeValue.ToString());
				Start("manufacturedMaterial");
				Start("code","code",npCur.CodeValue,"displayName",npCur.Description,"codeSystem",npCur.CodeSystemOID,"codeSystemName",npCur.CodeSystemName);
				_x.WriteAttributeString("sdtc","valueSet",null,npCur.ValueSetOID);
				End("code");
				End("manufacturedMaterial");
				End("manufacturedProduct");
				End("consumable");
				Start("entryRelationship","typeCode","RSON");
				Start("observation","classCode","OBS","moodCode","EVN");
				_x.WriteComment("Reason Observation Template");
				TemplateId("2.16.840.1.113883.10.20.24.3.88");
				StartAndEnd("code","code","410666004","displayName","reason","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
				Start("value");
				_x.WriteAttributeString("xsi","type",null,"CD");
				Attribs("code",npCur.CodeValueReason,"displayName",npCur.DescriptionReason,"codeSystem",npCur.CodeSystemOIDReason,"codeSystemName",npCur.CodeSystemNameReason);
				_x.WriteAttributeString("sdtc","valueSet",null,npCur.ValueSetOIDReason);
				End("value");
				End("observation");
				End("entryRelationship");
				End("substanceAdministration");
				End("entryRelationship");
				End("act");
			}
			else if(npCur.ValueSetOID=="2.16.840.1.113883.3.526.3.402" || npCur.ValueSetOID=="2.16.840.1.113883.3.600.1.462") {//flu vaccine proc or current meds documented proc performed not done
				Start("procedure","classCode","PROC","moodCode","EVN","negationInd","true");
				_x.WriteComment("Procedure Activity Procedure Template");
				TemplateId("2.16.840.1.113883.10.20.22.4.14");
				_x.WriteComment("Procedure Performed Template");
				TemplateId("2.16.840.1.113883.10.20.24.3.64");
				StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.Proc.ToString()+npCur.EhrCqmNotPerfNum.ToString());
				Start("code","code",npCur.CodeValue,"displayName",npCur.Description,"codeSystem",npCur.CodeSystemOID,"codeSystemName",npCur.CodeSystemName);
				_x.WriteAttributeString("sdtc","valueSet",null,npCur.ValueSetOID);
				End("code");
				_x.WriteElementString("text","Procedure Performed, not done: "+npCur.Description);
				StartAndEnd("statusCode","code","completed");
				Start("effectiveTime");
				DateElement("low",npCur.DateEntry);
				DateElement("high",npCur.DateEntry);
				End("effectiveTime");
				Start("entryRelationship","typeCode","RSON");
				Start("observation","classCode","OBS","moodCode","EVN");
				_x.WriteComment("Reason Template");
				TemplateId("2.16.840.1.113883.10.20.24.3.88");
				StartAndEnd("code","code","410666004","displayName","reason","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
				StartAndEnd("statusCode","code","completed");
				Start("effectiveTime");
				DateElement("low",npCur.DateEntry);
				End("effectiveTime");
				Start("value");
				_x.WriteAttributeString("xsi","type",null,"CD");
				Attribs("code",npCur.CodeValueReason,"displayName",npCur.DescriptionReason,"codeSystem",npCur.CodeSystemOIDReason,"codeSystemName",npCur.CodeSystemNameReason);
				_x.WriteAttributeString("sdtc","valueSet",null,npCur.ValueSetOIDReason);
				End("value");
				End("observation");
				End("entryRelationship");
				End("procedure");
			}
			else if(npCur.ValueSetOID=="2.16.840.1.113883.3.600.1.681") {//BMI exam performed not done
				_x.WriteComment("Physical Exam Finding");
				Start("observation","classCode","OBS","moodCode","EVN","negationInd","true");
				_x.WriteComment("Result Observation Template");
				TemplateId("2.16.840.1.113883.10.20.22.4.2");
				_x.WriteComment("Physical Exam Finding Template");
				TemplateId("2.16.840.1.113883.10.20.24.3.57");
				StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.Vital.ToString()+npCur.EhrCqmNotPerfNum.ToString());
				Start("code","code","39156-5","displayName","Body mass index (BMI) [Ratio]","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
				_x.WriteAttributeString("sdtc","valueSet",null,"2.16.840.1.113883.3.600.1.681");
				End("code");
				_x.WriteElementString("text","Physical Exam Performed, not done: BMI LOINC Value");
				StartAndEnd("statusCode","code","completed");
				Start("effectiveTime");
				DateElement("low",npCur.DateEntry);
				End("effectiveTime");
				StartAndEnd("value","nullFlavor","NI");
				Start("entryRelationship","typeCode","RSON");
				Start("observation","classCode","OBS","moodCode","EVN");
				_x.WriteComment("Reason Template");
				TemplateId("2.16.840.1.113883.10.20.24.3.88");
				StartAndEnd("code","code","410666004","displayName","reason","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
				StartAndEnd("statusCode","code","completed");
				Start("effectiveTime");
				DateElement("low",npCur.DateEntry);
				End("effectiveTime");
				Start("value");
				_x.WriteAttributeString("xsi","type",null,"CD");
				Attribs("code",npCur.CodeValueReason,"displayName",npCur.DescriptionReason,"codeSystem",npCur.CodeSystemOIDReason,"codeSystemName",npCur.CodeSystemNameReason);
				_x.WriteAttributeString("sdtc","valueSet",null,npCur.ValueSetOIDReason);
				End("value");
				End("observation");
				End("entryRelationship");
				End("observation");
			}
			else {//must be a tobacco assessment not done
				Start("observation","classCode","OBS","moodCode","EVN","negationInd","true");
				_x.WriteComment("Tobacco Use Template");
				TemplateId("2.16.840.1.113883.10.20.22.4.85");
				StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.MeasEvn.ToString()+npCur.EhrCqmNotPerfNum.ToString());
				StartAndEnd("code","code","ASSERTION","displayName","Assertion","codeSystem","2.16.840.1.113883.5.4","codeSystemName","ActCode");
				_x.WriteElementString("text","Risk Category Assessment, not done: "+npCur.Description);
				StartAndEnd("statusCode","code","completed");
				Start("effectiveTime");
				DateElement("low",npCur.DateEntry);
				End("effectiveTime");
				Start("value");
				_x.WriteAttributeString("xsi","type",null,"CD");
				Attribs("code",npCur.CodeValue,"displayName",npCur.Description,"codeSystem",npCur.CodeSystemOID,"codeSystemName",npCur.CodeSystemName);
				_x.WriteAttributeString("sdtc","valueSet",null,npCur.ValueSetOID);
				End("value");
				Start("entryRelationship","typeCode","RSON");
				Start("observation","classCode","OBS","moodCode","EVN");
				_x.WriteComment("Reason Template");
				TemplateId("2.16.840.1.113883.10.20.24.3.88");
				StartAndEnd("code","code","410666004","displayName","reason","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
				StartAndEnd("statusCode","code","completed");
				Start("effectiveTime");
				DateElement("low",npCur.DateEntry);
				End("effectiveTime");
				Start("value");
				_x.WriteAttributeString("xsi","type",null,"CD");
				Attribs("code",npCur.CodeValueReason,"displayName",npCur.DescriptionReason,"codeSystem",npCur.CodeSystemOIDReason,"codeSystemName",npCur.CodeSystemNameReason);
				_x.WriteAttributeString("sdtc","valueSet",null,npCur.ValueSetOIDReason);
				End("value");
				End("observation");
				End("entryRelationship");				
				End("observation");
			}
			End("entry");
			_isWriterW=true;			
		}

		private static void GenerateMedPatsEntry(EhrCqmMedicationPat mPatCur) {
			_isWriterW=false;
			Start("entry","typeCode","DRIV");
			if(mPatCur.EhrCqmMedicationPatNum!=0) {//either Medication, Active: or Medication, Ordered:
				if(mPatCur.PatNote!="") {//Medication, Order:
					Start("substanceAdministration","classCode","SBADM","moodCode","RQO");
					_x.WriteComment("Plan of Care Activity Substance Administration Template");
					TemplateId("2.16.840.1.113883.10.20.22.4.42");
					_x.WriteComment("Medication Order Template");
					TemplateId("2.16.840.1.113883.10.20.24.3.47");
					StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.MedPat.ToString()+mPatCur.EhrCqmMedicationPatNum.ToString());
					_x.WriteElementString("text","Medication Order: "+mPatCur.ValueSetName);
					StartAndEnd("statusCode","code","new");
				}
				else {//Medication, Active:
					Start("substanceAdministration","classCode","SBADM","moodCode","EVN");
					_x.WriteComment("Medication Activity Template");
					TemplateId("2.16.840.1.113883.10.20.22.4.16");
					StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.MedPat.ToString()+mPatCur.EhrCqmMedicationPatNum.ToString());
					_x.WriteElementString("text","Medication Active: "+mPatCur.ValueSetName);
					StartAndEnd("statusCode","code","completed");
				}
				Start("effectiveTime");
				_x.WriteAttributeString("xsi","type",null,"IVL_TS");
				DateElement("low",mPatCur.DateStart);
				DateElement("high",mPatCur.DateStop);
				End("effectiveTime");
				Start("consumable");
				Start("manufacturedProduct","classCode","MANU");
				_x.WriteComment("Medication Information Template");
				TemplateId("2.16.840.1.113883.10.20.22.4.23");
				StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.MedPat.ToString()+mPatCur.EhrCqmMedicationPatNum.ToString()+mPatCur.CVXCode);
				Start("manufacturedMaterial");
				Start("code","code",mPatCur.RxCui.ToString(),"displayName",mPatCur.Description,"codeSystem",mPatCur.CodeSystemOID,"codeSystemName",mPatCur.CodeSystemName);
				_x.WriteAttributeString("sdtc","valueSet",null,mPatCur.ValueSetOID);
				End("code");
				End("manufacturedMaterial");
				End("manufacturedProduct");
				End("consumable");
				End("substanceAdministration");
			}
			else {//if EhrCqmMedicationPat==0 then it is a vaccine, so EhrCqmVaccinePatNum!=0
				//if NotAdministered then it is a Medication Allergy/Intolerance
				if(mPatCur.CompletionStatus==VaccineCompletionStatus.NotAdministered) {
					Start("observation","classCode","OBS","moodCode","EVN");
					_x.WriteComment("Substance or Device Allergy - Intolerance Observation");
					TemplateId("2.16.840.1.113883.10.20.24.3.90");
					_x.WriteComment("Allergy - Intolerance Observation Template");
					TemplateId("2.16.840.1.113883.10.20.22.4.7");
					_x.WriteComment("Medication Allergy Template");
					TemplateId("2.16.840.1.113883.10.20.24.3.44");
					StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.MedPat.ToString()+mPatCur.EhrCqmVaccinePatNum.ToString());
					StartAndEnd("code","code","ASSERTION","displayName","Assertion","codeSystem","2.16.840.1.113883.5.4","codeSystemName","ActCode");
					StartAndEnd("statusCode","code","completed");
					Start("effectiveTime");
					DateElement("low",mPatCur.DateStart);
					DateElement("high",mPatCur.DateStop);
					End("effectiveTime");
					Start("value");
					_x.WriteAttributeString("xsi","type",null,"CD");
					Attribs("code","416098002","displayName","Drug allergy","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
					End("value");
					Start("participant","typeCode","CSM");
					Start("participantRole","classCode","MANU");
					Start("playingEntity","classCode","MMAT");
					Start("code","code",mPatCur.CVXCode,"displayName",mPatCur.Description,"codeSystem",mPatCur.CodeSystemOID,"codeSystemName",mPatCur.CodeSystemName);
					_x.WriteAttributeString("sdtc","valueSet",null,mPatCur.ValueSetOID);
					End("code");
					_x.WriteElementString("text","Medication Allergy: "+mPatCur.ValueSetName);
					End("playingEntity");
					End("participantRole");
					End("participant");
					End("observation");
				}
				else {//otherwise it is a Medication Administered
					Start("act","classCode","ACT","moodCode","EVN");
					_x.WriteComment("Medication Administered Template");
					TemplateId("2.16.840.1.113883.10.20.24.3.42");
					StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.MedPat.ToString()+mPatCur.EhrCqmVaccinePatNum.ToString());
					StartAndEnd("code","code","416118004","displayName","Administration","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
					StartAndEnd("statusCode","code","completed");
					Start("effectiveTime");
					_x.WriteAttributeString("xsi","type",null,"IVL_TS");
					DateElement("low",mPatCur.DateStart);
					DateElement("high",mPatCur.DateStop);
					End("effectiveTime");
					Start("entryRelationship","typeCode","COMP");
					Start("substanceAdministration","classCode","SBADM","moodCode","EVN");
					_x.WriteComment("Medication Activity");
					TemplateId("2.16.840.1.113883.10.20.22.4.16");
					StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.MedPat.ToString()+mPatCur.EhrCqmVaccinePatNum.ToString()+mPatCur.PatNum.ToString());
					_x.WriteElementString("text","Medication Administered: "+mPatCur.ValueSetName);
					StartAndEnd("statusCode","code","completed");
					Start("effectiveTime");
					_x.WriteAttributeString("xsi","type",null,"IVL_TS");
					DateElement("low",mPatCur.DateStart);
					DateElement("high",mPatCur.DateStop);
					End("effectiveTime");
					Start("consumable");
					Start("manufacturedProduct","classCode","MANU");
					_x.WriteComment("Medication Information Template");
					TemplateId("2.16.840.1.113883.10.20.22.4.23");
					StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.MedPat.ToString()+mPatCur.EhrCqmMedicationPatNum.ToString()+mPatCur.CVXCode);
					Start("manufacturedMaterial");
					Start("code","code",mPatCur.CVXCode,"displayName",mPatCur.Description,"codeSystem",mPatCur.CodeSystemOID,"codeSystemName",mPatCur.CodeSystemName);
					_x.WriteAttributeString("sdtc","valueSet",null,mPatCur.ValueSetOID);
					End("code");
					End("manufacturedMaterial");
					End("manufacturedProduct");
					End("consumable");
					End("substanceAdministration");
					End("entryRelationship");
					End("act");
				}
			}
			End("entry");
			_isWriterW=true;			
		}

		private static void GenerateMeasEventEntry(EhrCqmMeasEvent mCur) {
			_isWriterW=false;
			Start("entry","typeCode","DRIV");
			if(mCur.EventType==EhrMeasureEventType.TobaccoUseAssessed) {
				Start("observation","classCode","OBS","moodCode","EVN");
				_x.WriteComment("Tobacco Use Template");
				TemplateId("2.16.840.1.113883.10.20.22.4.85");
				StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.MeasEvn.ToString()+mCur.EhrCqmMeasEventNum.ToString());
				StartAndEnd("code","code","ASSERTION","displayName","Assertion","codeSystem","2.16.840.1.113883.5.4","codeSystemName","ActCode");
				_x.WriteElementString("text","Patient Characteristic: "+mCur.Description);//Description is of the tobacco assessment result, not the description of the assessment itself.  Example: Tobacco Assessment description = History of tobacco use Narrative, but description of result is Smoker (finding).  We want to display the result, Smoker.
				StartAndEnd("statusCode","code","completed");
				Start("effectiveTime");
				DateElement("low",mCur.DateTEvent);
				End("effectiveTime");
				Start("value");
				_x.WriteAttributeString("xsi","type",null,"CD");
				Attribs("code",mCur.CodeValue,"displayName",mCur.Description,"codeSystem",mCur.CodeSystemOID,"codeSystemName",mCur.CodeSystemName);
				_x.WriteAttributeString("sdtc","valueSet",null,mCur.ValueSetOID);
				End("value");
				End("observation");
			}
			else if(mCur.EventType==EhrMeasureEventType.CurrentMedsDocumented) {
				Start("procedure","classCode","PROC","moodCode","EVN");
				_x.WriteComment("Procedure Activity Procedure Template");
				TemplateId("2.16.840.1.113883.10.20.22.4.14");
				_x.WriteComment("Procedure Performed Template");
				TemplateId("2.16.840.1.113883.10.20.24.3.64");
				StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.MeasEvn.ToString()+mCur.EhrCqmMeasEventNum.ToString());
				Start("code","code",mCur.CodeValue,"displayName",mCur.Description,"codeSystem",mCur.CodeSystemOID,"codeSystemName",mCur.CodeSystemName);
				_x.WriteAttributeString("sdtc","valueSet",null,mCur.ValueSetOID);
				End("code");
				_x.WriteElementString("text","Procedure, Performed: "+mCur.Description);
				StartAndEnd("statusCode","code","completed");
				Start("effectiveTime");
				_x.WriteComment("Attribute: Start Datetime");
				DateElement("low",mCur.DateTEvent);
				_x.WriteComment("Attribute: Stop Datetime");
				DateElement("high",mCur.DateTEvent);
				End("effectiveTime");
				End("procedure");
			}
			End("entry");
			_isWriterW=true;
		}

		private static void GenerateInterventionEntry(EhrCqmIntervention iCur) {
			_isWriterW=false;
			Start("entry","typeCode","DRIV");
			//these are the value sets used by BMI for adults, interventions for above/below weight follow up or referrals for weight assessment
			//these are Intervention, Order
			if(iCur.ValueSetOID=="2.16.840.1.113883.3.600.1.1525" || iCur.ValueSetOID=="2.16.840.1.113883.3.600.1.1527" || iCur.ValueSetOID=="2.16.840.1.113883.3.600.1.1528") {
				Start("act","classCode","ACT","moodCode","RQO");
				_x.WriteComment("Plan of Care Activity Act Template");
				TemplateId("2.16.840.1.113883.10.20.22.4.39");
				_x.WriteComment("Intervention Order Template");
				TemplateId("2.16.840.1.113883.10.20.24.3.63");
				StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.Ivn.ToString()+iCur.EhrCqmInterventionNum.ToString());
				Start("code","code",iCur.CodeValue,"displayName",iCur.Description,"codeSystem",iCur.CodeSystemOID,"codeSystemName",iCur.CodeSystemName);
				_x.WriteAttributeString("sdtc","valueSet",null,iCur.ValueSetOID);
				End("code");
				_x.WriteElementString("text","Intervention Order: "+iCur.ValueSetName);
				StartAndEnd("statusCode","code","completed");
				Start("effectiveTime");
				DateElement("low",iCur.DateEntry);
				DateElement("high",iCur.DateEntry);
				End("effectiveTime");
				Start("author");
				DateElement("time",iCur.DateEntry);
				Start("assignedAuthor");
				StartAndEnd("id","root",_strOIDInternalProvRoot,"extension",iCur.ProvNum.ToString());
				End("assignedAuthor");
				End("author");
				End("act");
			}
			//all others are Intervention, Performed
			else {
				Start("act","classCode","ACT","moodCode","EVN");
				_x.WriteComment("Procedure Activity Act Template");
				TemplateId("2.16.840.1.113883.10.20.22.4.12");
				_x.WriteComment("Intervention Performed Template");
				TemplateId("2.16.840.1.113883.10.20.24.3.32");
				StartAndEnd("id","root",_strOIDInternalCQMRoot,"extension",CqmItemAbbreviation.Ivn.ToString()+iCur.EhrCqmInterventionNum.ToString());
				Start("code","code",iCur.CodeValue,"displayName",iCur.Description,"codeSystem",iCur.CodeSystemOID,"codeSystemName",iCur.CodeSystemName);
				_x.WriteAttributeString("sdtc","valueSet",null,iCur.ValueSetOID);
				End("code");
				_x.WriteElementString("text","Intervention Performed: "+iCur.ValueSetName);
				StartAndEnd("statusCode","code","completed");
				Start("effectiveTime");
				DateElement("low",iCur.DateEntry);
				DateElement("high",iCur.DateEntry);
				End("effectiveTime");
				End("act");
			}
			End("entry");
			_isWriterW=true;			
		}

		///<summary>Helper for GenerateQRDA(). Builds an "id" element and writes a 36 character GUID string into the "root" attribute.
		///An example of how the uid might look: "5b010313-eff2-432c-9909-6193d8416fac"</summary>
		private static void Guid() {
			Guid uuid=System.Guid.NewGuid();
			while(_hashQrdaGuids.Contains(uuid.ToString())) {
				uuid=System.Guid.NewGuid();
			}
			_hashQrdaGuids.Add(uuid.ToString());
			StartAndEnd("id","root",uuid.ToString());
		}

		///<summary>Helper for GenerateQRDA().  Modifies the global _w variable.  Performs a WriteStartElement, followed by any attributes.  Attributes must be in pairs: name, value.</summary>
		private static void Start(string elementName,params string[] attributes) {
			if(_isWriterW) {
				_w.WriteStartElement(elementName);
				for(int i=0;i<attributes.Length;i+=2) {
					_w.WriteAttributeString(attributes[i],attributes[i+1]);
				}
			}
			else {
				_x.WriteStartElement(elementName);
				for(int i=0;i<attributes.Length;i+=2) {
					_x.WriteAttributeString(attributes[i],attributes[i+1]);
				}
			}
		}

		///<summary>Helper for GenerateQRDA().  Modifies the global _w variable.  Performs a WriteEndElement.  The specified elementName is for readability only.</summary>
		private static void End(string elementName) {
			if(_isWriterW) {
				_w.WriteEndElement();
			}
			else {
				_x.WriteEndElement();
			}
		}

		///<summary>Helper for GenerateQRDA().  Modifies the global _w variable.  Performs a WriteStartElement, followed by any attributes, followed by a WriteEndElement.  Attributes must be in pairs: name, value.</summary>
		private static void StartAndEnd(string elementName,params string[] attributes) {
			if(_isWriterW) {
				_w.WriteStartElement(elementName);
				for(int i=0;i<attributes.Length;i+=2) {
					_w.WriteAttributeString(attributes[i],attributes[i+1]);
				}
				_w.WriteEndElement();
			}
			else {
				_x.WriteStartElement(elementName);
				for(int i=0;i<attributes.Length;i+=2) {
					_x.WriteAttributeString(attributes[i],attributes[i+1]);
				}
				_x.WriteEndElement();
			}
		}

		///<summary>Helper for GenerateQRDA().  Modifies the global _w variable.</summary>
		private static void TemplateId(string rootNumber) {
			TemplateId(rootNumber,"");
		}

		///<summary>Helper for GenerateQRDA().  Modifies the global _w variable.</summary>
		private static void TemplateId(string rootNumber,string authorityName) {
			if(_isWriterW) {
				_w.WriteStartElement("templateId");
				_w.WriteAttributeString("root",rootNumber);
				if(authorityName!="") {
					_w.WriteAttributeString("assigningAuthorityName",authorityName);
				}
				_w.WriteEndElement();
			}
			else {
				_x.WriteStartElement("templateId");
				_x.WriteAttributeString("root",rootNumber);
				if(authorityName!="") {
					_x.WriteAttributeString("assigningAuthorityName",authorityName);
				}
				_x.WriteEndElement();
			}
		}

		///<summary>Use for HTML tables. Writes the element strElement name and writes the dateTime string in the required date format.  Will not write if year is before 1880.</summary>
		private static void DateText(string strElementName,DateTime dateTime) {
			Start(strElementName);
			if(dateTime.Year>1880) {
				_w.WriteString(dateTime.ToString("yyyyMMdd"));
			}
			End(strElementName);
		}

		///<summary>Use for XML. Writes the element strElement name and writes the dateTime in the required date format into the value attribute.
		///Will write nullFlavor="UNK" instead of value if year is before 1880.</summary>
		private static void DateElement(string strElementName,DateTime dateTime) {
			Start(strElementName);
			if(dateTime.Year<1880) {
				Attribs("nullFlavor","UNK");
			}
			else {
				Attribs("value",dateTime.ToString("yyyyMMdd"));
			}
			End(strElementName);
		}

		///<summary>Helper for GenerateQrda().  Performs a WriteAttributeString for each attribute.  Attributes must be in pairs: name, value.</summary>
		private static void Attribs(params string[] attributes) {
			if(_isWriterW) {
				for(int i=0;i<attributes.Length;i+=2) {
					_w.WriteAttributeString(attributes[i],attributes[i+1]);
				}
			}
			else {
				for(int i=0;i<attributes.Length;i+=2) {
					_x.WriteAttributeString(attributes[i],attributes[i+1]);
				}
			}
		}

		///<summary>Writes the element strElement name and writes the dateTime in the required date format into the value attribute.
		///Will write nullFlavor="UNK" instead of value if year is before 1880.</summary>
		private static void TimeElement(string strElementName,DateTime dateTime) {
			Start(strElementName);
			if(dateTime.Year<1880) {
				Attribs("nullFlavor","UNK");
			}
			else {
				Attribs("value",dateTime.ToString("yyyyMMddHHmmsszzz").Replace(":",""));
			}
			End(strElementName);
		}

		private static void AddressUnitedStates(string strAddress1,string strAddress2,string strCity,string strState,string strZip,string strUse) {
			Start("addr","use",strUse);
			if(strUse=="HP") {
				_w.WriteComment("HP is \"primary home\" from codeSystem 2.16.840.1.113883.5.1119");
			}
			else if(strUse=="WP") {
				_w.WriteComment("WP is \"work place\" from codeSystem 2.16.840.1.113883.5.1119");
			}
			else if(strUse=="H") {
				_w.WriteComment("H is \"home address\" from codeSystem 2.16.840.1.113883.5.1119");
			}
			_w.WriteElementString("streetAddressLine",strAddress1);
			if(strAddress2!="") {
				_w.WriteElementString("streetAddressLine",strAddress2);
			}
			_w.WriteElementString("city",strCity);
			_w.WriteElementString("state",strState);
			if(strZip.Length>4) {
				_w.WriteElementString("postalCode",strZip.Substring(0,5));//postalCode is required if the country is US.  If country is not specified, it's assumed to be US. If country is something other than US, the postalCode MAY be present but MAY be bound to different vocabularies. Validated to be at least 5 characters long.
			}
			_w.WriteElementString("country","US");
			End("addr");
		}

		private static void MeasureEntry(string versionSpecificId,string measureTitle,string versionNeutralId,string versionNum) {
			Start("entry");
			Start("organizer","classCode","CLUSTER","moodCode","EVN");
			_w.WriteComment("Measure Reference Template");
			TemplateId("2.16.840.1.113883.10.20.24.3.98");
			_w.WriteComment("eMeasure Reference QDM Template");
			TemplateId("2.16.840.1.113883.10.20.24.3.97");
			StartAndEnd("statusCode","code","completed");
			Start("reference","typeCode","REFR");
			Start("externalDocument","classCode","DOC","moodCode","EVN");
			_w.WriteComment("This is the version specific identifier for eMeasure: QualityMeasureDocument/id it is a GUID");
			StartAndEnd("id","root",versionSpecificId);
			_w.WriteComment("This is the title of the eMeasure");
			_w.WriteElementString("text",measureTitle);
			_w.WriteComment("setId is the eMeasure version neutral id");
			StartAndEnd("setId","root",versionNeutralId);
			_w.WriteComment("This is the sequential eMeasure Version number");
			StartAndEnd("versionNumber","value",versionNum);
			End("externalDocument");
			End("reference");
			End("organizer");
			End("entry");
		}

		private static void MeasureTextTableRow(string measureTitle,string cmsId,string versionNeutralId,string versionNum,string versionSpecificId) {
			Start("tr");
			_w.WriteElementString("td",cmsId);
			_w.WriteElementString("td",measureTitle);
			_w.WriteElementString("td",versionNeutralId);
			_w.WriteElementString("td",versionNum);
			_w.WriteElementString("td",versionSpecificId);
			End("tr");
		}

		private static void PatientDataTextTableRow(string description,string valueSetOID,string codeSystemName,string codeValue,DateTime dateStart,DateTime dateEnd,string result) {
			Start("tr");
			_w.WriteElementString("td",description);
			_w.WriteElementString("td",valueSetOID);
			_w.WriteElementString("td",codeSystemName+": "+codeValue);
			if(dateEnd.Date.Year>1880) {
				//if start and end date the same, we will not bother with date range, simply one date
				if(dateEnd.Date==dateStart.Date) {
					_w.WriteElementString("td",dateStart.ToString("MMMM dd, yyyy"));
				}
				else {
					_w.WriteElementString("td",dateStart.ToString("MMMM dd, yyyy")+" - "+dateEnd.ToString("MMMM dd, yyyy"));
				}
			}
			else {//no end date, considered currently still 'active'
				_w.WriteElementString("td",dateStart.ToString("MMMM dd, yyyy")+" - present");
			}
			_w.WriteElementString("td",result);
			End("tr");
		}

		///<summary>Checks data values for preferences and practice information to ensure required data is available for QRDA creation.
		///Returns empty string if no errors, otherwise returns a string containing error messages.</summary>
		private static string ValidateSettings() {
			string strErrors="";
			if(PrefC.GetString(PrefName.PracticeTitle).Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing practice title.";
			}
			if(PrefC.GetString(PrefName.PracticePhone).Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing practice phone.";
			}
			if(PrefC.GetString(PrefName.PracticeAddress).Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing practice address line 1.";
			}
			if(PrefC.GetString(PrefName.PracticeCity).Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing practice city.";
			}
			if(PrefC.GetString(PrefName.PracticeST).Trim().Length!=2) {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Invalid practice state.  Must be two letters.";
			}
			if(PrefC.GetString(PrefName.PracticeZip).Trim().Length<5) {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Invalid practice zip.  Must be at least 5 digits long.";
			}
			//The Legal Authenticator must have a valid first name, last name, and NPI number and is the "single person legally responsible for the document" and "must be a person".
			//Guaranteed to be a provider that is not marked 'Not a Person' when we get here.
			Provider provLegal=Providers.GetProv(_provNumLegal);
			if(provLegal.FName.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing Legal Authenticator "+provLegal.Abbr+" first name.";
			}
			if(provLegal.LName.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing Legal Authenticator "+provLegal.Abbr+" last name.";
			}
			if(provLegal.NationalProvID.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing Legal Authenticator "+provLegal.Abbr+" NPI.";
			}
			if(Snomeds.GetCodeCount()==0) {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing SNOMED codes.  Go to Setup | Chart | EHR | Code System Importer to download.";
			}
			if(Cvxs.GetCodeCount()==0) {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing CVX codes.  Go to Setup | Chart | EHR | Code System Importer to download.";
			}
			return strErrors;
		}

		///<summary>Checks data values for pat as well as primary provider information to ensure required data is available for CCD creation.
		///Returns empty string if no errors, otherwise returns a string containing error messages.</summary>
		private static string ValidatePatient(Patient pat) {
			string strErrors="";
			if(pat.FName.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing patient first name.";
			}
			if(pat.LName.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing patient last name.";
			}
			if(pat.Address.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing patient address line 1.";
			}
			if(pat.City.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing patient city.";
			}
			if(pat.State.Trim().Length!=2) {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Invalid patient state.  Must be two letters.";
			}
			//postalCode is required if the country is US.
			//If country is not specified, it's assumed to be US. If country is something other than US, the postalCode MAY be present but MAY be bound to different vocabularies
			//we are inserting all patient's country as US, so must have zip code.  Can be 5 or 9 digit zip code.
			if(pat.Zip.Trim().Length<5) {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Invalid patient zip.  Must be at least 5 digits.";
			}
			if(pat.Birthdate.Year<1880) {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing patient birth date.";
			}
			if(pat.HmPhone.Trim()=="" && pat.WirelessPhone.Trim()=="" && pat.WkPhone.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing patient phone. Must have home, wireless, or work phone.";
			}
			Provider provPri=Providers.GetProv(pat.PriProv);
			if(provPri.FName.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing provider "+provPri.Abbr+" first name.";
			}
			if(provPri.LName.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing provider "+provPri.Abbr+" last name.";
			}
			if(provPri.NationalProvID.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing provider "+provPri.Abbr+" NPI.";
			}
			return strErrors;
		}

		///<summary>Validation rules:
		///<para>1. No embedded dashes or spaces.</para>
		///<para>2. Must be alphanumeric.</para>
		///<para>3. Alpha characters must be upper case.</para>
		///<para>4. Length can't be > 12 or less than 7.</para>
		///<para>5. If alphanumeric, all numbers cannot be 9s.</para>
		///<para>6. If length 7: must be 1 alpha + 6 numeric.</para>
		///<para>7. If length 8: must be 2 alpha + 6 numeric.</para>
		///<para>8. If length 9: must be 3 alpha + 6 numeric.</para>
		///<para>9. If length 10: can either be 1 alpha + 9 numeric, or 9 numeric + 1 alpha.</para>
		///<para>10. If length 11: must be 2 alpha + 9 numeric, or 9 numeric + 1 alpha + 1 numeric, or 9 numeric + 2 alpha.</para>
		///<para>11. If length 12: must be 3 alpha + 9 numeric.</para></summary>
		private static string ValidateMedicaidID(string medicaidId) {
			string retval=medicaidId.Trim();
			if(!Regex.IsMatch(retval,@"[0-8]")) {
				return "";
			}
			if(Regex.IsMatch(retval,@"^[A-Z]{1,3}[0-9]{6}$")
				|| Regex.IsMatch(retval,@"^[A-Z]{1,3}[0-9]{9}$")
				|| Regex.IsMatch(retval,@"^[0-9]{9}[A-Z][A-Z0-9]{0,1}$")) 
			{
					return retval;
			}
			return "";
		}


	}

}

