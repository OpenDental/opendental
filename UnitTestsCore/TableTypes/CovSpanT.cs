using OpenDentBusiness;

namespace UnitTestsCore {
	public class CovSpanT {

		///<summary>Inserts and returns a CovSpan. Refreshes the CovSpans cache.</summary>
		public static CovSpan CreateCovSpan(long covCatNum,string fromCode="D9001",string toCode="D9999") {
			CovSpan covSpan=new CovSpan();
			covSpan.CovCatNum=covCatNum;
			covSpan.FromCode=fromCode;
			covSpan.ToCode=toCode;
			CovSpans.Insert(covSpan);
			CovSpans.RefreshCache();
			return covSpan;
		}

		///<summary>Deletes everything from the covspan table where PK > 16 and refreshes cache. CovSpanNum 1-16 are included in the dump file.</summary>
		public static void ClearCovSpanTable() {
			string command="DELETE FROM covspan WHERE CovSpanNum > 16";
			DataCore.NonQ(command);
			CovSpans.RefreshCache();
		}
	}

}
