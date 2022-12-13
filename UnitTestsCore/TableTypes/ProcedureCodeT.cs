using System;
using System.Collections.Generic;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ProcedureCodeT {

		///<summary>Returns a procedure code object that utilizes the procCode passed in.
		///Either returns the pre-existing code from the cache or creates a new one.  Throws an exception if procCode is longer than 15 chars.</summary>
		public static ProcedureCode CreateProcCode(string procCode,bool isCanadianLab=false,string abbrDesc="",long procCat=0,TreatmentArea treatmentArea=TreatmentArea.None,bool isTreatmentAreaAlsoToothRange=false) {
			//The ProcCode column on the procedurecode table is a VARCHAR(15).  MySQL will not throw an exception but will instead truncate the ProcCode.
			//Engineers might not be expecting this and might write an invalid unit test assuming that this method did what they told it to do.
			if(procCode.Length > 15) {
				throw new ODException("Invalid procCode passed into ProcedureCodeT.CreateProcCode(); Must be less than 15 characters.");
			}
			AddIfNotPresent(procCode,isCanadianLab,abbrDesc,procCat,treatmentArea:treatmentArea,isTreatmentAreaAlsoToothRange:isTreatmentAreaAlsoToothRange);
			return ProcedureCodes.GetOne(procCode);
		}
		///<summary>Returns a list of procedure code objects, each with unique TreatAreas.</summary>
		public static List<ProcedureCode> CreateProcCodesForEachTreatArea() {
			List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>();
			listProcedureCodes.Add(CreateProcCode("NONE",treatmentArea:TreatmentArea.None));
			listProcedureCodes.Add(CreateProcCode("SURF",treatmentArea:TreatmentArea.Surf));
			listProcedureCodes.Add(CreateProcCode("TOOTH",treatmentArea:TreatmentArea.Tooth));
			listProcedureCodes.Add(CreateProcCode("MOUTH",treatmentArea:TreatmentArea.Mouth));
			listProcedureCodes.Add(CreateProcCode("QUAD",treatmentArea:TreatmentArea.Quad));
			listProcedureCodes.Add(CreateProcCode("QUADRANGE",treatmentArea:TreatmentArea.Quad,isTreatmentAreaAlsoToothRange:true));
			listProcedureCodes.Add(CreateProcCode("SEXTANT",treatmentArea:TreatmentArea.Sextant));
			listProcedureCodes.Add(CreateProcCode("ARCH",treatmentArea:TreatmentArea.Arch));
			listProcedureCodes.Add(CreateProcCode("ARCHRANGE",treatmentArea:TreatmentArea.Arch,isTreatmentAreaAlsoToothRange:true));
			listProcedureCodes.Add(CreateProcCode("TOOTHRANGE",treatmentArea:TreatmentArea.ToothRange));
			return listProcedureCodes;
		}

		public static void SetIsTaxed(string procCode, bool isTaxed=true) {
			ProcedureCode procedureCode=ProcedureCodes.GetProcCode(procCode);
			procedureCode.IsTaxed=isTaxed;
			Update(procedureCode);
		}
		
		public static void Update(ProcedureCode procCode) {
			ProcedureCodes.Update(procCode);
			ProcedureCodes.RefreshCache();
		}

		/// <summary>Returns true if a procedureCode was added.</summary>
		public static bool AddIfNotPresent(string procCode,bool isCanadianLab=false,string abbrDesc="",long procCat=0,ToothPaintingType toothPaintingType=ToothPaintingType.None,TreatmentArea treatmentArea=TreatmentArea.None,bool isTreatmentAreaAlsoToothRange=false) {
			if(!ProcedureCodes.GetContainsKey(procCode)) {
				ProcedureCodes.Insert(new ProcedureCode {
					AbbrDesc=abbrDesc,
					ProcCode=procCode,
					IsCanadianLab=isCanadianLab,
					ProcCat=procCat,
					PaintType=toothPaintingType,
					TreatArea=treatmentArea,
					AreaAlsoToothRange=isTreatmentAreaAlsoToothRange
				});
				ProcedureCodes.RefreshCache();
				return true;
			}
			return false;
		}

		///<summary>Clears the procedurecode table.  Does not truncate as to not let the PKs repeat.</summary>
		public static void ClearProcedureCodeTable() {
			string command="DELETE FROM procedurecode";
			DataCore.NonQ(command);
			ProcedureCodes.RefreshCache();
		}

		///<summary>Returns a procedure code num that utilizes the procCode passed in. Creates a new procedure code if no match found.</summary>
		public static long GetCodeNum(string procCode) {
			return CreateProcCode(procCode).CodeNum;
		}
	}
}
