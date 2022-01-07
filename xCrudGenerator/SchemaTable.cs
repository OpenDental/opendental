using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace xCrudGenerator {
	[CrudTable(TableName="tempcore")]
	class SchemaTable {//do not inherit from TableBase because it's not a real table
		/// <summary></summary>
		[CrudColumn(IsPriKey=true)]
		public long TempCoreNum;
		///<summary></summary>
		public TimeSpan TimeOfDayTest;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime TimeStampTest;
		///<summary></summary>
		public DateTime DateTest;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeTest;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanNeg)]
		public TimeSpan TimeSpanTest;
		///<summary></summary>
		public double CurrencyTest;
		///<summary></summary>
		public bool BoolTest;
		///<summary>The crud will create this as a varchar(255) and the programmer must change it manually.</summary>
		public string TextSmallTest;// >255 & <4k
		///<summary>The crud should generate mysql=text and oracle=clob</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string TextMediumTest;// >4k & <65k
		///<summary>The crud should generate mysql=text and oracle=clob</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string TextLargeTest;// >65k
		///<summary></summary>
		public string VarCharTest;// <255

		public void NoWarnings() {
			SchemaTable retval=new SchemaTable() {
				TempCoreNum=TempCoreNum,
				TimeOfDayTest=TimeOfDayTest,
				TimeStampTest=TimeStampTest,
				DateTest=DateTest,
				DateTimeTest=DateTimeTest,
				TimeSpanTest=TimeSpanTest,
				CurrencyTest=CurrencyTest,
				BoolTest=BoolTest,
				TextSmallTest=TextSmallTest,
				TextMediumTest=TextMediumTest,
				TextLargeTest=TextLargeTest,
				VarCharTest=VarCharTest
			};
		}

	}
}
