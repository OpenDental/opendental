using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.MiscUtils_Tests {
	[TestClass]
	public class MiscUtilsTests:TestBase {

		[TestMethod]
		public void MiscUtils_CutStringIntoSimilarSizedChunks_CheckSizeWithEightChunksOfRandomASCIIChars() {
			string testString=CreateRandomASCIITestString(1024);
			List<string> listChunks=MiscUtils.CutStringIntoSimilarSizedChunks(testString,128);
			Assert.AreEqual(Encoding.UTF8.GetByteCount(listChunks[0]),128,$"Chunk failed test: {listChunks[0]}");
			Assert.AreEqual(Encoding.UTF8.GetByteCount(listChunks[1]),128,$"Chunk failed test: {listChunks[1]}");
			Assert.AreEqual(Encoding.UTF8.GetByteCount(listChunks[2]),128,$"Chunk failed test: {listChunks[2]}");
			Assert.AreEqual(Encoding.UTF8.GetByteCount(listChunks[3]),128,$"Chunk failed test: {listChunks[3]}");
			Assert.AreEqual(Encoding.UTF8.GetByteCount(listChunks[4]),128,$"Chunk failed test: {listChunks[4]}");
			Assert.AreEqual(Encoding.UTF8.GetByteCount(listChunks[5]),128,$"Chunk failed test: {listChunks[5]}");
			Assert.AreEqual(Encoding.UTF8.GetByteCount(listChunks[6]),128,$"Chunk failed test: {listChunks[6]}");
			Assert.AreEqual(Encoding.UTF8.GetByteCount(listChunks[7]),128,$"Chunk failed test: {listChunks[7]}");
		}

		[TestMethod]
		public void MiscUtils_CutStringIntoSimilarSizedChunks_BuildSplitCompareSixChunksOfPrefabbedStrings() {
			string testString1="lVqCqMcJpAV`IaSDmMbDuwOzC[MUNxLfbAKyWDqZpRKx\\cbLxF]HUXeoHrzWzNwspkyIWThjVIOclQ^[tAXI[DQJVVr`D[ZbVSIleYpeYmmMJBCrU`]zqnXl[omA\\Hgu";
			string testString2="LTMxRCepwFvBl`ogqmZQEH_jFoLgCXebJfoCBnCxzWgQvnBI]tOpauqDp^s^qyn_[UsFklfCk]Ri\\NoRmDJIJpAPplN^v_GAcIqauvYlOLFDcHcbjalnpfUeTcPy_HHL";
			string testString3="oAEaJbpmehi`sAmGBdInivbfdv\\YafcABmGq[OgGiTATOAZXSiT`ywbtYZxOFXMyEi_FuvfmxBEVuNWd]DFsSDCELNYAQYARHBPa\\T^WCacxsMyKDUmCRTgBf[ug^e\\_";
			string testString4="r^BlvbYw\\aaEqEtKARcpQwdA^aAU\\vwJtBbBlvuGLdrVK`zDcobuuspvuBNVaD\\adWKo^rYFX\\dhENpyaxmhTCOBLfHjh_`HIRmnUK`kV]]ZOIqgLPodRoMaIIcrkjXs";
			string testString5="rz\\Y[pUviwPrunjiqN\\^P[LzxpwtMuwYcsNQSrF_KwnbYaagtL]isHyXn][fs[jnzrOCaYWyIwTTheaXOQABXO`\\uq]pW[fjMzcbR_F\\o^avofjJvDWcZiWlRnHkvbch";
			string testString6="sAbxvCOnhWG^`qldVlzXtDkXFIzzbDHv[HtdCpFTDFZBNJnHEX_Vwv_oPKsnCOLeOLDYxIbagfTodjKwVrvK_ly_dETswSRotpmBFVKwqxjRbjKazvXKTBF^N_osoidU";
			string testString7="hqrgbSFic_kiBGCe[qbvlEicwxRq]juwVPyTmDOfBKmHyB_fRVbblVUcIOkeBPMseoSlIisXbwuaSaeuZf`GISKAuVL^Seemyt^Kjm^c^Jzp\\oPcepLWjFjRdRrm^UEx";
			string testString8="QXHShTSdMPWNUynEmeWZvYebRtTobTuG[ebYbSkIArjRgf]YcdIEnU`rnaAoz^CsUPnkWV][uNZEjTvgWvMxLqSFFkfBRwI^oqwWslCNGiKav[I[eUXEREh_xhfgj[Iy";
			string inputString=testString1+testString2+testString3+testString4+testString5+testString6+testString7+testString8;
			List<string> listChunks=MiscUtils.CutStringIntoSimilarSizedChunks(inputString,128);
			Assert.AreEqual(listChunks[0],testString1);
			Assert.AreEqual(listChunks[1],testString2);
			Assert.AreEqual(listChunks[2],testString3);
			Assert.AreEqual(listChunks[3],testString4);
			Assert.AreEqual(listChunks[4],testString5);
			Assert.AreEqual(listChunks[5],testString6);
			Assert.AreEqual(listChunks[6],testString7);
			Assert.AreEqual(listChunks[7],testString8);
		}

		[TestMethod]
		public void MiscUtils_CutStringINtoSimilarSizedChunks_SplitStringWithNonASCIIChars() {
			string utfString=CreateRandomUTFString(1024);
			List<string> listChunks=MiscUtils.CutStringIntoSimilarSizedChunks(utfString,128);
			string postChunkString="";
			foreach(string str in listChunks) {
				postChunkString+=str;
			}
			Assert.AreEqual(utfString,postChunkString,$"Chunk failed test: {utfString}");
		}

		[TestMethod]
		public void MiscUtils_GetDatesInRange() {
			DateTime dateTimeStart=DateTime.Today;
			//DateEnd less than DateStart should return an empty list
			DateTime dateTimeEnd=dateTimeStart.AddDays(-1);
			List<DateTime> listDateTimes=MiscUtils.GetDatesInRange(dateTimeStart,dateTimeEnd);
			Assert.AreEqual(0,listDateTimes.Count);
			//Equal dates should return one date
			dateTimeEnd=dateTimeStart;
			listDateTimes=MiscUtils.GetDatesInRange(dateTimeStart,dateTimeEnd);
			Assert.AreEqual(1,listDateTimes.Count);
			//A range should return the list including start / end dates
			dateTimeEnd=dateTimeStart.AddDays(7);
			listDateTimes=MiscUtils.GetDatesInRange(dateTimeStart,dateTimeEnd);
			Assert.AreEqual(8,listDateTimes.Count);
		}

		public string CreateRandomASCIITestString(int size) {
			string asciiString="";
			for(int i=33;i < 127;i++) {
				asciiString+=((char)i).ToString();
			}
			string testString="";
			Random ranNumber=new Random();
			for(int i=0;i < size;i++) {
				testString+=asciiString[ranNumber.Next(0,94)];
			}
			return testString;
		}

		public string CreateRandomUTFString(int size) {
			Random ranNumber=new Random();
			byte[] arrBytes=new byte[size];
			ranNumber.NextBytes(arrBytes);
			return Encoding.UTF8.GetString(arrBytes);
		}
	}
}
