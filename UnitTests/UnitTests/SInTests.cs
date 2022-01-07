using DataConnectionBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests.SIn_Tests {
	[TestClass]
	public class SInTests:TestBase {

		[TestMethod]
		public void SInTests_SInEnum_Int() {
			SinTestEnum enumExpected=SinTestEnum.Third;
			int sinTestEnumInt=(int)enumExpected;
			SinTestEnum enumActual=SIn.Enum<SinTestEnum>(sinTestEnumInt);
			Assert.AreEqual(enumExpected,enumActual);
		}

		[TestMethod]
		public void SInTests_SInEnum_IntNoMatch() {
			int intExpected=(int)SinTestEnum.Second+(int)SinTestEnum.Fourth;
			//There is no Sixth enum value. However, the enum will still 'parse' and will preserve the value of 6.
			SinTestEnum enumActual=SIn.Enum<SinTestEnum>(intExpected);
			Assert.IsNotNull(enumActual);
			int intActual=(int)enumActual;
			Assert.AreEqual(intExpected,intActual);
		}

		[TestMethod]
		public void SInTests_SInEnum_AsString() {
			SinTestEnum enumExpected=SinTestEnum.First;
			string strExpected=enumExpected.ToString();
			SinTestEnum enumActual=SIn.Enum<SinTestEnum>(strExpected,isEnumAsString:true);
			Assert.AreEqual(enumExpected,enumActual);
		}

		[TestMethod]
		public void SInTests_SInEnum_AsStringNoMatch() {
			SinTestEnum enumDefaultValueExpected=SinTestEnum.Second;
			string strInvalidEnumValue="NinetyNinth";
			//NinetyNinth is an invalid enum value and cannot be parsed into anything intelligible. Therefore, the default value will be returned.
			SinTestEnum enumActual=SIn.Enum(strInvalidEnumValue,isEnumAsString:true,defaultEnumOption:enumDefaultValueExpected);
			Assert.AreEqual(enumDefaultValueExpected,enumActual);
		}

		[TestMethod]
		public void SInTests_SInEnum_IntAsString() {
			SinTestEnum enumExpected=SinTestEnum.First;
			string strExpectedAsInt=((int)enumExpected).ToString();
			SinTestEnum enumActual=SIn.Enum<SinTestEnum>(strExpectedAsInt,isEnumAsString:false);
			Assert.AreEqual(enumExpected,enumActual);
		}

		[TestMethod]
		public void SInTests_SInEnum_IntAsStringNoMatch() {
			SinTestEnum enumDefaultValueExpected=SinTestEnum.Second;
			string strInvalidEnumValue="NinetyNinth";
			//NinetyNinth is an invalid integer. The string representation of 99 should have been used instead. Therefore, the default value will return.
			SinTestEnum enumActual=SIn.Enum(strInvalidEnumValue,isEnumAsString:false,defaultEnumOption:enumDefaultValueExpected);
			Assert.AreEqual(enumDefaultValueExpected,enumActual);
		}

		[TestMethod]
		public void SInTests_SInEnum_IntAsValidStringNoMatch() {
			SinTestEnum enumDefaultEnumOption=SinTestEnum.Second;//Won't be used when enum is treated as an integer.
			int intExpected=99;
			string strInvalidEnumValue=intExpected.ToString();
			//The string representation of 99 is not a valid value in the enumeration. However, the designated value of 99 should be preserved.
			SinTestEnum enumActual=SIn.Enum(strInvalidEnumValue,isEnumAsString:false,defaultEnumOption:enumDefaultEnumOption);
			Assert.IsNotNull(enumActual);
			Assert.AreEqual(intExpected,(int)enumActual);
		}

		[TestMethod]
		public void SInTests_SInEnum_Flags() {
			SinTestFlagsEnum enumExpected=SinTestFlagsEnum.Eight | SinTestFlagsEnum.Four;
			int intEnumExpected=(int)enumExpected;
			Assert.AreEqual(enumExpected,SIn.Enum<SinTestFlagsEnum>(intEnumExpected));
		}

		[TestMethod]
		public void SInTests_SInEnum_FlagsNoMatch() {
			int intEnumExpected=int.MaxValue;
			//MaxValue is not a valid value for the flags enumeration. However, the value should be preserved after parsing.
			SinTestFlagsEnum enumActual=SIn.Enum<SinTestFlagsEnum>(intEnumExpected);
			Assert.IsNotNull(enumActual);
			Assert.AreEqual(intEnumExpected,(int)enumActual);
		}

		[TestMethod]
		public void SInTests_SInEnum_FlagsNoMatchIntAsString() {
			SinTestFlagsEnum enumDefaultEnumOption=SinTestFlagsEnum.Zero;
			int intEnumExpected=int.MaxValue;
			//MaxValue is not a valid value for the flags enumeration. However, the value should be preserved after parsing.
			SinTestFlagsEnum enumActual=SIn.Enum(intEnumExpected.ToString(),defaultEnumOption:enumDefaultEnumOption);
			Assert.IsNotNull(enumActual);
			Assert.AreEqual(intEnumExpected,(int)enumActual);
		}

		[TestMethod]
		public void SInTests_SInEnum_FlagsAsString() {
			SinTestFlagsEnum enumExpected=SinTestFlagsEnum.Eight | SinTestFlagsEnum.Four;
			Assert.AreEqual(enumExpected,SIn.Enum<SinTestFlagsEnum>(enumExpected.ToString(),isEnumAsString:true));
		}

		private enum SinTestEnum {
			None,
			First,
			Second,
			Third,
			Fourth,
		}

		[Flags]
		private enum SinTestFlagsEnum {
			Zero=0,
			One=1,
			Two=2,
			Four=4,
			Eight=8,
		}

	}
}
