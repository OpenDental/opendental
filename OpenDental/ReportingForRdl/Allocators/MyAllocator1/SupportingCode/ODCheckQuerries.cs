using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDental.Reporting.Allocators.MyAllocator1
{
    class ODDataCheckQuerry
	{
		#region Private Static Objects
		#region RawData Don't use by itself because it will select 70,000+ records
		/// <summary>
        /// Selects all payments, Adjustments, and charges
        /// </summary>
        private static readonly string RawData = @"SELECT 
 '2' as Type, 
      patient.Guarantor, 
      patient.PatNum as Patient, 
      adjustment.ProvNum, 
      adjustment.ADjDate as Date, 
      adjustment.AdjAmt as Ammount, 
       Adjustment.AdjNum,
       1 as ODTable
 FROM adjustment, patient 
 WHERE 
      adjustment.PatNum = Patient.PatNum 
      && adjustment.AdjAmt <= 0

    && adjustment.IsPulledToDansLedger = 0
 UNION ALL
SELECT 
 '3', 
      patient.Guarantor, 
      patient.PatNum, 
      adjustment.ProvNum, 
      adjustment.ADjDate, 
      adjustment.AdjAmt, 
       adjustment.AdjNum,
       1
 FROM adjustment, patient 
 WHERE 
      adjustment.PatNum = Patient.PatNum 
      && adjustment.AdjAmt > 0

    && adjustment.IsPulledToDansLedger = 0
 UNION ALL
SELECT 
      '2', 
      patient.Guarantor, 
      patient.PatNum, 
      claimproc.ProvNum, 
      claimpayment.CheckDate, 
      -(claimproc.WriteOff), 
       claimproc.ClaimProcNum,
       2
 FROM claimpayment,claimproc, patient 
 WHERE 
      claimproc.PatNum = Patient.PatNum 
      && claimproc.ClaimPaymentNum = claimpayment.ClaimPaymentNum 
      && (claimproc.Status = 1 OR Claimproc.Status = 4)

    && claimproc.IsPulledToDansLedger = 0
 UNION ALL
SELECT 
      '1', 
      patient.Guarantor, 
      patient.PatNum, 
      0, 
      paysplit.DatePay, 
      -paysplit.SplitAmt, 
       paysplit.SplitNum,
       3
 FROM paysplit,patient 
 WHERE 
      paysplit.PatNum = Patient.PatNum 

    && paysplit.IsPulledToDansLedger = 0
 UNION ALL
SELECT 
      '1', 
      patient.Guarantor, 
      patient.PatNum, 
      0, 
      claimpayment.CheckDate, 
      -claimproc.InsPayAmt, 
       claimproc.ClaimProcNum,
       2
 FROM claimproc,patient,claimpayment 
 WHERE 
      claimproc.PatNum = Patient.PatNum 
      && claimpayment.ClaimPaymentNum = claimproc.ClaimPaymentNum 

    && claimproc.IsPulledToDansLedger = 0
 UNION ALL
SELECT 
    0,
    Patient.Guarantor,
    Patient.PatNum,
    Procedurelog.ProvNum,  
    Procedurelog.ProcDate ,  
    Procedurelog.ProcFee, 
    Procedurelog.ProcNum ,
       0
FROM 
     Procedurelog, Patient
WHERE 
     Patient.PatNum = Procedurelog.Patnum 
     && Procedurelog.ProcStatus = 2

    && Procedurelog.IsPulledToDansLedger = 0
";



#endregion
		#endregion

		#region Public Static Objects
		#region CheckForNegPayments
		/// <summary>
        /// Fields Selected Include:
        /// Patient.LName, 
        /// Patient.FName, 
        /// Patient.PatNum,
        /// tbl1.Date,
        /// tbl1.Ammount
        /// </summary>
        public static readonly string CheckForNegPayments = ""
            +"SELECT Patient.LName, Patient.FName, Patient.PatNum, "
            +" tbl1.Date,tbl1.Ammount \nFROM ( " + ODDataCheckQuerry.RawData 
            + " ) as tbl1, Patient \nWHERE Patient.PatNum = tbl1.Patient "
            + "\n&& tbl1.Type = '1' "
            + "\n&& tbl1.Ammount > 0";
        #endregion
        #region CheckFor -ve Charges
        /// <summary>
        /// Fields Selected Include:
        /// Patient.LName, 
        /// Patient.FName, 
        /// Patient.PatNum,
        /// tbl1.Date,
        /// tbl1.Ammount
        /// </summary>
        public static readonly string CheckForNegCharges = ""
            + "SELECT Patient.LName, Patient.FName, Patient.PatNum, "
            + " tbl1.Date,tbl1.Ammount\n FROM( " + ODDataCheckQuerry.RawData 
            + " ) as tbl1, Patient\nWHERE Patient.PatNum = tbl1.Patient\n"
            + "\n&& tbl1.Type = '0' "
            + "\n&& tbl1.Ammount < 0";
             #endregion

        #region  Get Guarantors of Specific Patients
        /// <summary>
        /// Finds all the guarantors associated with Patients
        /// </summary>
        public static  string GuarantorsOfPatient(uint[] Patients)
        {
            string command = "SELECT DISTINCT(Patient.Guarantor) \nFROM Patient \nWHERE ";
            for (int i = 0; i < Patients.Length; i++)
            {
                command += "Patient.PatNum = " + Patients[i].ToString() + " ";
                if (i < Patients.Length - 1)
                    command += " \n&& ";


            }
            return command;

        }
        #endregion
		#endregion
	}
}
