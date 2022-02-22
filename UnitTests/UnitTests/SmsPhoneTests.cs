using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;

namespace UnitTests.SmsPhone_Tests {
	[TestClass]
	public class SmsPhoneTests {
		
		private string charGsm=$"{'\r'}{'\n'}"+@" !#$%'""()*+,-./:;<=>?@_¡£¥§¿&¤0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzÄÅÆÇÉÑØøÜßÖàäåæèéìñòöùüΔΦΓΛΩΠΨΣΘΞ";
		private string charGsmExtended=@"|^€{}[]~\";
		private int countBytesForHeader=6;
		private int countBytesPerMessage=140;

		[TestMethod]
		public void SmsPhones_GetMessageType_Unicode() {
			string text="ABCDEFGHIJKLMNOP⏪";
			SmsPhones.MessageCharSet msgType=SmsPhones.GetMessageCharSet(text,charGsm,charGsmExtended);
			Assert.AreEqual(SmsPhones.MessageCharSet.Unicode,msgType);
		}

		[TestMethod]
		public void SmsPhones_GetMessageType_StandardGsm() {
			string text=charGsm;
			SmsPhones.MessageCharSet msgType=SmsPhones.GetMessageCharSet(text,charGsm,charGsmExtended);
			Assert.AreEqual(SmsPhones.MessageCharSet.Text,msgType);
		}

		[TestMethod]
		public void SmsPhones_GetMessageType_StandardGsmExt() {
			string text=charGsmExtended;
			SmsPhones.MessageCharSet msgType=SmsPhones.GetMessageCharSet(text,charGsm,charGsmExtended);
			Assert.AreEqual(SmsPhones.MessageCharSet.Text,msgType);
		}

		[TestMethod]
		public void SmsPhones_GetMessageBytes() {
			double gsmBytesExpected=Math.Ceiling((charGsm.Length*7) / 8.0);
			double gsmExtendedBytesExpected=Math.Ceiling((charGsmExtended.Length*14) / 8.0);
			Assert.AreEqual(gsmBytesExpected,SmsPhones.GetCountBytesForMessage(charGsm,charGsm,charGsmExtended));
			Assert.AreEqual(gsmExtendedBytesExpected,(SmsPhones.GetCountBytesForMessage(charGsmExtended,charGsm,charGsmExtended)));
			string test="⏪";
			double unicodeBytesExpected=test.Length*2;
			Assert.AreEqual(unicodeBytesExpected,SmsPhones.GetCountBytesForMessage(test,charGsm,charGsmExtended));
		}

		[TestMethod]
		///<summary>
		///160 gsm characters => 1 part
		///161 gsm chars => 2
		///306 gsm chars => 2
		///307 gsm chars => 3
		///</summary>
		public void SmsPhones_CalculateMessageParts_StandardAscii() {
			string testStr=new string(Enumerable.Range(0,160).Select(x=>'a').ToArray());
			int numMessages=SmsPhones.CalculateMessageParts(testStr,countBytesForHeader,charGsm,charGsmExtended,countBytesPerMessage);
			Assert.AreEqual(1,numMessages);
			testStr=new string(Enumerable.Range(0,161).Select(x=>'a').ToArray());
			Assert.AreEqual(2,SmsPhones.CalculateMessageParts(testStr,countBytesForHeader,charGsm,charGsmExtended,countBytesPerMessage));
			testStr=new string(Enumerable.Range(0,306).Select(x=>'a').ToArray());
			Assert.AreEqual(2,SmsPhones.CalculateMessageParts(testStr,countBytesForHeader,charGsm,charGsmExtended,countBytesPerMessage));
			testStr=new string(Enumerable.Range(0,307).Select(x=>'a').ToArray());
			Assert.AreEqual(3,SmsPhones.CalculateMessageParts(testStr,countBytesForHeader,charGsm,charGsmExtended,countBytesPerMessage));
		}
		

		[TestMethod]
		///<summary>
		///158 gsm characters + 1 extended => 1 part
		///159 gsm charaters + 1 extended => 2 
		///</summary>
		public void SmsPhones_CalculateMessageParts_StandardAsciiExtended() {
			string testStr=new string(Enumerable.Range(0,158).Select(x=>'a').ToArray());
			testStr+="}";
			Assert.AreEqual(1,SmsPhones.CalculateMessageParts(testStr,countBytesForHeader,charGsm,charGsmExtended,countBytesPerMessage));
			testStr=new string(Enumerable.Range(0,159).Select(x=>'a').ToArray());
			testStr+="{";
			Assert.AreEqual(2,SmsPhones.CalculateMessageParts(testStr,countBytesForHeader,charGsm,charGsmExtended,countBytesPerMessage));
		}

		[TestMethod]
		///<summary>
		///70 unicode => 1
		///71 unicode => 2
		///134 unicde => 2
		///135 unicode => 3
		///</summary>
		public void SmsPhones_CalculateMessageParts_Unicode() {
			string text=new string(Enumerable.Range(0,70).Select(x=>'⏪').ToArray());
			Assert.AreEqual(1,SmsPhones.CalculateMessageParts(text,countBytesForHeader,charGsm,charGsmExtended,countBytesPerMessage));
			text=new string(Enumerable.Range(0,71).Select(x=>'⏪').ToArray());
			Assert.AreEqual(2,SmsPhones.CalculateMessageParts(text,countBytesForHeader,charGsm,charGsmExtended,countBytesPerMessage));
			text=new string(Enumerable.Range(0,134).Select(x=>'⏪').ToArray());
			Assert.AreEqual(2,SmsPhones.CalculateMessageParts(text,countBytesForHeader,charGsm,charGsmExtended,countBytesPerMessage));
			text=new string(Enumerable.Range(0,135).Select(x=>'⏪').ToArray());
			Assert.AreEqual(3,SmsPhones.CalculateMessageParts(text,countBytesForHeader,charGsm,charGsmExtended,countBytesPerMessage));
		}
		
		///<summary>
		///1 unicode+69 gsm => 1
		///1 unicode+70 gsm => 2 
		///</summary>
		[TestMethod]
		public void SmsPhones_CalculateMessageParts_StandardAndUnicode() {
			string text=new string(Enumerable.Range(0,69).Select(x=>'a').ToArray());
			text+="⏪";
			Assert.AreEqual(1,SmsPhones.CalculateMessageParts(text,countBytesForHeader,charGsm,charGsmExtended,countBytesPerMessage));
			text=new string(Enumerable.Range(0,70).Select(x=>'a').ToArray());
			text+="⏪";
			Assert.AreEqual(2,SmsPhones.CalculateMessageParts(text,countBytesForHeader,charGsm,charGsmExtended,countBytesPerMessage));
		}
	}
}
