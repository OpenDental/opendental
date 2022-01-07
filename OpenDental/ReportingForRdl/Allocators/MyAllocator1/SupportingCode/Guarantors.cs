using System;
using System.Collections.Generic;
using System.Text;
//using PaymentDistributor;using ManagedConnection;

namespace OpenDental.Reporting.Allocators.MyAllocator1
{
	/// <summary>
	/// Simple object that holds the Guarantor Patient Relationship
	/// </summary>
    class Guarantors : IComparable
    {
		#region private variables
        private int m_GuarantorNum;
		private int[] m_Patients;
#endregion

		#region Public objects - Excluded Guarantors and Patients
		/// <summary>
        /// Used to hold Guarantors that have a problem with compatability.
        /// ie Payment is -ve or Charge is -ve
        /// </summary>
        public static int[] ExcludedGuarantors = null;
        /// <summary>
        /// See ExcludedGuarantors
        /// </summary>
        public static int[] ExcludedPatients = null;
#endregion
        
        #region Public Properties
        public int[] PATIENTS
        {
            get { return this.m_Patients; }
            set { this.m_Patients = value; }
        }
        public int Number
        {
            get { return this.m_GuarantorNum; }
            set { this.m_GuarantorNum = value; }
        }
        #endregion

        #region Operators  used to build the Guarantor Object's Guarantor/Patient relationship
        /// <summary>
        /// Should add NewPatient to the G1.PATIENTS.  
        /// if G1.PATIENTS already contains NewPatient then nothing is done. 
        /// </summary>
        public static Guarantors operator +(Guarantors G1, int NewPatient)
        {
            return Guarantors.Add(G1, NewPatient); 
        }
        /// <summary>
        /// Should add NewPatient to the G1.PATIENTS.  
        /// if G1.PATIENTS already contains NewPatient then nothing is done. 
        /// </summary>
        public static Guarantors operator +(int NewPatient,Guarantors G1)
        {
            return Guarantors.Add(G1, NewPatient); 
        }
        /// <summary>
        /// Basic method used in operator+  Basically helps generate the Guarantor Object.
        /// </summary>
        /// <param name="G1"></param>
        /// <param name="NewPatient"></param>
        /// <returns></returns>
        public static Guarantors Add(Guarantors G1, int NewPatient)
        {
            if (G1 == null)
                throw new Exception("Null reference passed to Guarnator Operator -dwk");

            if (G1.PATIENTS == null)
            {
                G1.PATIENTS = new int[1];
                G1.PATIENTS[0] = NewPatient;
                return G1;
            }
            for (int i = 0; i < G1.PATIENTS.Length; i++)
                if (G1.PATIENTS[i] == NewPatient)
                    return G1; // ignore the attempt to add
            int[] Temp1 = new int[G1.PATIENTS.Length + 1];
            ArrayCopySmalltoLarge( G1.PATIENTS,  Temp1);
            Temp1[Temp1.Length-1] = NewPatient;
            G1.PATIENTS = Temp1;
            return G1;
        }
     
        /// <summary>
        /// Calls the first overload
        /// </summary>
        public static Guarantors Add(int NewPatient, Guarantors G1)
        {
            return Guarantors.Add(G1, NewPatient);
        }
        /// <summary>
        /// Removes NewPatient from G1.PATIENTS if it NewPatient is present in G1.PATIENTS.
        /// </summary>
        public static Guarantors operator -(Guarantors G1, int NewPatient)
        {
            return Subtract( G1,  NewPatient);
        }
        /// <summary>
        /// Removes NewPatient from G1.PATIENTS if it NewPatient is present in G1.PATIENTS.
        /// </summary>
        public static Guarantors operator -(int NewPatient,Guarantors G1)
        {
            return Subtract(G1, NewPatient);
        }
        /// <summary>
        /// Removes NewPatient from G1.PATIENTS if it NewPatient is present in G1.PATIENTS.
        /// </summary>
        public static Guarantors Subtract(int NewPatient, Guarantors G1)
        {
            return Subtract(G1, NewPatient);
        }
        /// <summary>
        /// Removes NewPatient from G1.PATIENTS if it NewPatient is present in G1.PATIENTS.
        /// </summary>
        public static Guarantors Subtract(Guarantors G1, int NewPatient)
        {
            if (G1 == null)
                throw new Exception("Operator - in Guarantors cannot apply to a null reference -dwk");
            int IndexToRemove = -1;
            if(G1.PATIENTS == null)
                return G1; // ie NewPatient is obviously not present

            for (int i = 0; i < G1.PATIENTS.Length; i++)
            {
                if (G1.PATIENTS[i] == NewPatient)
                    IndexToRemove = i;
            }
            if (IndexToRemove == -1)
                return G1; // NewPatient not present in G1.PATIENTS 
            int[] Temp = new int[G1.PATIENTS.Length - 1];
            int index = 0;
            for (int i = 0; i < G1.PATIENTS.Length; i++)
            {
                if (i != IndexToRemove)
                {
                    Temp[index] = G1.PATIENTS[i];
                    index++;
                }
            }
            G1.PATIENTS = Temp;
            return G1;


        }
        #endregion

        #region ArrayCopyMethods
        /// <summary>
        /// Copies arrays from Smallersize to Larger Size
        /// Leaving the last element(s) of the larger array empty
        /// </summary>
        /// <param name="?"></param>
		private static void ArrayCopySmalltoLarge(int[] FromSmall, int[] ToLarge)
        {
            if (FromSmall.Length > ToLarge.Length)
                throw new Exception("Smaller array is bigger than Larger array -dwk ");
            for (int i = 0; i < FromSmall.Length; i++)
            {
                ToLarge[i] = FromSmall[i];
            }
        }
        /// <summary>
        /// Copies arrays from Smallersize to Larger Size
        /// Dropping the last element(s) of the larger array empty
        /// </summary>
        /// <param name="?"></param>
        private static void ArrayCopyLargetoSmall( int[] ToSmall,  int[] FromLarge)
        {
            if (ToSmall.Length > FromLarge.Length)
                throw new Exception("Smaller array is bigger than Larger array -dwk ");
            for (int i = 0; i < ToSmall.Length; i++)
            {
                ToSmall[i] = FromLarge[i];
            }
        }
        #endregion

        #region IComparable Members
        /// <summary>
        /// Return 0 if Guarantors are identical
        /// other wise returns comparison between guarantor numbers.
        /// Throws exception if obj is not type Guarantor or 
        /// if obj == null
        /// Note PATIENTS element of Guarantor must be in same order
        /// for them to be counted as the same.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            Guarantors G1 = this;
            Guarantors G2 = null;

            if (obj is Guarantors)
                G2 = (Guarantors)obj;
            else
                throw new Exception("Object passed to Guarantors.CompareTo(object) is not Type Guarantor -dwk");
            if (G2 == null)
                throw new Exception("Null reference passed to Guarantors.CompareTo(object). -dwk ");
            
            if (G1.Number != G2.Number)
                return G1.Number.CompareTo(G2.Number);
            // Guarantor Numbers Equal so compare arrays
            return CompareArrays(G1.PATIENTS, G2.PATIENTS); 
                
        }
        /// <summary>
        /// Note Arrays must be identical not same elements in different order
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        private int CompareArrays(int[] a1, int [] a2)
        {
            if (a1 == null && a2 == null)
                return 0;
            if (a1 != null && a2 == null || a1 == null && a2 != null)
                return -1;
            if (a1.Length != a2.Length)
                return -1;
            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return -1;
            return 0;

        }

        #endregion

        public static bool GuarantorIsInExcludedList(uint Guarantor)
        {
            bool found = false;
            if (ExcludedGuarantors != null)
            {
                for (int i = 0; i < ExcludedGuarantors.Length; i++)
                {
                    if (ExcludedGuarantors[i] == Guarantor)
                    {
                        found = true;
                        i = ExcludedGuarantors.Length;
                    }

                }
            }
            return found;
		}
		public void Refresh()
		{

		}

		#region  Commented out code used to update Danstables
		///// <summary>
		///// How can items not be in OpenDental?  Good Question:
		///// 
		///// What happens is that in OD the Guarantor relationship will change so that 
		///// the person becomes a guarantor of another person.  (Say a child was on their own
		///// and got moved to their mother.)
		///// 
		///// Example:  
		/////         Patient1  : 10001     Patient2  : 20001
		/////         Guarantor1: 10001     Guarantor2: 20001
		/////       Guarantor1 changed from its own guarantor to 20001 so now
		/////         Patient1  : 10001     Patient2  : 20001
		/////         Guarantor1: 20001     Guarantor2: 20001
		///// 
		///// OpenDental:             DansLedger:
		/////             100             99
		/////             101             100
		/////             102             101
		/////             103             105
		///// 
		///// returns:  99,105
		///// 
		///// </summary>
		///// <returns></returns>
		
		//public static uint[] GuarantorsNotInOD()
		//{
		//    string command =  "SELECT DISCTINCT(Guarantor) FROM Patient";
		//    QueryResult r1 = QueryResult.RunQuery(command);

		//    string command2 = "SELECT DISCTINCT(Guarantor) FROM " + TableUtility.AvailableTables.dansledgeroriginaldata.ToString();
		//    QueryResult r2 = QueryResult.RunQuery(command2);

		//    List<uint> GntorNotInOD = new List<uint>();


		//    System.Collections.Hashtable htOD = new System.Collections.Hashtable();
		//    if (r1.Success && r2.Success)
		//    {
		//        if (r1.DataTbl.Rows.Count != 0)
		//        {
		//            for (int i = 0; i < r1.DataTbl.Rows.Count; i++)
		//                htOD[(uint)r1.DataTbl.Rows[i][0]] = r1.DataTbl.Rows[i][1];
		//        }
		//        foreach (System.Data.DataRow dr in r2.DataTbl.Rows)
		//            if (!htOD.ContainsKey((uint)dr[0]))
		//                GntorNotInOD.Add((uint)dr[0]);

		//    }
		//    return GntorNotInOD.ToArray();
		//}
		///// <summary>
		///// How does this happen?
		///// 
		///// Example:
		///// 
		///// OpenDental:             DansLedger:
		/////             100             99
		/////             101             100
		/////             102             101
		/////             103             105
		///// 
		///// returns:  102,103
		///// 
		///// Happens when ever:
		/////     1) New Guarantor is made
		/////             
		/////     can happen when an existing patient changes from patient to guarantor status
		///// </summary>
		///// <returns></returns>
		//public static uint[] GuarantorsNotInDL()
		//{
		//    string command = "SELECT DISCTINCT(Guarantor) FROM Patient";
		//    QueryResult r1 = MasterConnectionData.RunQuery(command);

		//    string command2 = "SELECT DISCTINCT(Guarantor) FROM " + TableUtility.AvailableTables.dansledgeroriginaldata.ToString();
		//    QueryResult r2 = MasterConnectionData.RunQuery(command2);

		//    List<uint> GntorNotInDL = new List<uint>();


		//    System.Collections.Hashtable htDL = new System.Collections.Hashtable();
		//    if (r1.Success && r2.Success)
		//    {
		//        if (r2.DataTbl.Rows.Count != 0)
		//        {
		//            for (int i = 0; i < r2.DataTbl.Rows.Count; i++)
		//                htDL[(uint)r2.DataTbl.Rows[i][0]] = r2.DataTbl.Rows[i][1];
		//        }
		//        foreach (System.Data.DataRow dr in r1.DataTbl.Rows)
		//            if (!htDL.ContainsKey((uint)dr[0]))
		//                GntorNotInDL.Add((uint)dr[0]);

		//    }
		//    return GntorNotInDL.ToArray();
		//}

		#endregion 
	}
}

