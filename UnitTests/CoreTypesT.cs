using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using OpenDentBusiness;
using System.IO;
using System.Drawing.Imaging;
using DataConnectionBase;

namespace UnitTests {
	public class CoreTypesT {
		/// <summary></summary>
		public static string CreateTempTable(string serverAddr, string port, string userName, string password, bool isOracle) {
			string retVal="";
			UnitTestsCore.DatabaseTools.SetDbConnection(TestBase.UnitTestDbName,serverAddr,port,userName,password,isOracle);
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS tempcore";
				DataCore.NonQ(command);
				command=@"CREATE TABLE tempcore (
					TempCoreNum bigint NOT NULL,
					TimeOfDayTest time NOT NULL DEFAULT '00:00:00',
					TimeStampTest timestamp,
					DateTest date NOT NULL DEFAULT '0001-01-01',
					DateTimeTest datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					TimeSpanTest time NOT NULL DEFAULT '00:00:00',
					CurrencyTest double NOT NULL,
					BoolTest tinyint NOT NULL,
					TextTinyTest varchar(255) NOT NULL, 
					TextSmallTest text NOT NULL,
					TextMediumTest text NOT NULL,
					TextLargeTest mediumtext NOT NULL,
					VarCharTest varchar(255) NOT NULL
					) DEFAULT CHARSET=utf8";
				DataCore.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE tempcore'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				DataCore.NonQ(command);
				command=@"CREATE TABLE tempcore (
					TempCoreNum number(20),
					TimeOfDayTest date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD'),
					TimeStampTest timestamp DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD'),
					DateTest date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD'),
					DateTimeTest date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD'),
					TimeSpanTest varchar2(255),
					CurrencyTest number(38,8),
					BoolTest number(3),
					TextTinyTest varchar2(255),
					TextSmallTest varchar2(4000),
					TextMediumTest clob,
					TextLargeTest clob,
					VarCharTest varchar2(255)
					)";
				DataCore.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS tempgroupconcat";
				DataCore.NonQ(command);
				command=@"CREATE TABLE tempgroupconcat (
					Names varchar(255)
					) DEFAULT CHARSET=utf8";
				DataCore.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE tempgroupconcat'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				DataCore.NonQ(command);
				command=@"CREATE TABLE tempgroupconcat (
					Names varchar2(255)
					)";
				DataCore.NonQ(command);
			}
			retVal+="Temp tables created.\r\n";
			//retVal+="Temp tables cannot yet be created.\r\n";
			return retVal;
		}

		/// <summary></summary>
		public static string RunAll() {
			string retVal="";
			//Things that we might later add to this series of tests:
			//Special column types such as timestamp
			//Computer set to other region, affecting string parsing of types such dates and decimals
			//Test types without casting back and forth to strings.
			//Retrieval using a variety of techniques, such as getting a table, scalar, and reading a row.
			//Blobs
			string command="";
			DataTable table;
			TimeSpan timespan;
			TimeSpan timespan2;
			string varchar1;
			string varchar2;
			//timespan(timeOfDay)----------------------------------------------------------------------------------------------
			timespan=new TimeSpan(1,2,3);//1hr,2min,3sec
			command="INSERT INTO tempcore (TimeOfDayTest) VALUES ("+POut.Time(timespan)+")";
			DataCore.NonQ(command);
			command="SELECT TimeOfDayTest FROM tempcore";
			table=DataCore.GetTable(command);
			timespan2=PIn.Time(table.Rows[0]["TimeOfDayTest"].ToString());
			if(timespan!=timespan2) {
				throw new Exception();
			}
			command="DELETE FROM tempcore";
			DataCore.NonQ(command);
			retVal+="TimeSpan (time of day): Passed.\r\n";
			//timespan, negative------------------------------------------------------------------------------------
			timespan=new TimeSpan(0,-36,0);//This particular timespan value was found to fail in mysql with the old connector.
			//Don't know what's so special about this one value.  There are probably other values failing as well, but it doesn't matter.
			//Oracle does not seem to like negative values.
			command="INSERT INTO tempcore (TimeSpanTest) VALUES ('"+POut.TSpan(timespan)+"')";
			DataCore.NonQ(command);
			command="SELECT TimeSpanTest FROM tempcore";
			table=DataCore.GetTable(command);
			string tempVal=table.Rows[0]["TimeSpanTest"].ToString();
			timespan2=PIn.TSpan(tempVal);
			if(timespan!=timespan2) {
				throw new Exception();
			}
			command="DELETE FROM tempcore";
			DataCore.NonQ(command);
			retVal+="TimeSpan, negative: Passed.\r\n";
			//timespan, over 24 hours-----------------------------------------------------------------------------
			timespan=new TimeSpan(432,5,17);
			command="INSERT INTO tempcore (TimeSpanTest) VALUES ('"+POut.TSpan(timespan)+"')";
			DataCore.NonQ(command);
			command="SELECT TimeSpanTest FROM tempcore";
			table=DataCore.GetTable(command);
			timespan2=PIn.TSpan(table.Rows[0]["TimeSpanTest"].ToString());
			if(timespan!=timespan2) {
				throw new Exception();
			}
			command="DELETE FROM tempcore";
			DataCore.NonQ(command);
			retVal+="TimeSpan, large: Passed.\r\n";
			//date----------------------------------------------------------------------------------------------
			DateTime date1;
			DateTime date2;
			date1=new DateTime(2003,5,23);
			command="INSERT INTO tempcore (DateTest) VALUES ("+POut.Date(date1)+")";
			DataCore.NonQ(command);
			command="SELECT DateTest FROM tempcore";
			table=DataCore.GetTable(command);
			date2=PIn.Date(table.Rows[0]["DateTest"].ToString());
			if(date1!=date2) {
				throw new Exception();
			}
			command="DELETE FROM tempcore";
			DataCore.NonQ(command);
			retVal+="Date: Passed.\r\n";
			//datetime------------------------------------------------------------------------------------------
			DateTime datet1;
			DateTime datet2;
			datet1=new DateTime(2003,5,23,10,18,0);
			command="INSERT INTO tempcore (DateTimeTest) VALUES ("+POut.DateT(datet1)+")";
			DataCore.NonQ(command);
			command="SELECT DateTimeTest FROM tempcore";
			table=DataCore.GetTable(command);
			datet2=PIn.DateT(table.Rows[0]["DateTimeTest"].ToString());
			if(datet1!=datet2) {
				throw new Exception();
			}
			command="DELETE FROM tempcore";
			DataCore.NonQ(command);
			retVal+="Date/Time: Passed.\r\n";
			//currency------------------------------------------------------------------------------------------
			double double1;
			double double2;
			double1=12.34d;
			command="INSERT INTO tempcore (CurrencyTest) VALUES ("+POut.Double(double1)+")";
			DataCore.NonQ(command);
			command="SELECT CurrencyTest FROM tempcore";
			table=DataCore.GetTable(command);
			double2=PIn.Double(table.Rows[0]["CurrencyTest"].ToString());
			if(double1!=double2) {
				throw new Exception();
			}
			command="DELETE FROM tempcore";
			DataCore.NonQ(command);
			retVal+="Currency: Passed.\r\n";
			//group_concat------------------------------------------------------------------------------------
			command="INSERT INTO tempgroupconcat VALUES ('name1')";
			DataCore.NonQ(command);
			command="INSERT INTO tempgroupconcat VALUES ('name2')";
			DataCore.NonQ(command);
			command="SELECT "+DbHelper.GroupConcat("Names")+" allnames FROM tempgroupconcat";
			table=DataCore.GetTable(command);
			string allnames=PIn.ByteArray(table.Rows[0]["allnames"].ToString());
			//if(DataConnection.DBtype==DatabaseType.Oracle) {
			//	allnames=allnames.TrimEnd(',');//known issue.  Should already be fixed:
				//Use RTRIM(REPLACE(REPLACE(XMLAgg(XMLElement("x",column_name)),'<x>'),'</x>',','))
			//}
			if(allnames!="name1,name2") {
				throw new Exception();
			}
			command="DELETE FROM tempgroupconcat";
			DataCore.NonQ(command);
			retVal+="Group_concat: Passed.\r\n";
			//bool,pos------------------------------------------------------------------------------------
			bool bool1;
			bool bool2;
			bool1=true;
			command="INSERT INTO tempcore (BoolTest) VALUES ("+POut.Bool(bool1)+")";
			DataCore.NonQ(command);
			command="SELECT BoolTest FROM tempcore";
			table=DataCore.GetTable(command);
			bool2=PIn.Bool(table.Rows[0]["BoolTest"].ToString());
			if(bool1!=bool2) {
				throw new Exception();
			}
			command="DELETE FROM tempcore";
			DataCore.NonQ(command);
			retVal+="Bool, true: Passed.\r\n";
			//bool,neg------------------------------------------------------------------------------------
			bool1=false;
			command="INSERT INTO tempcore (BoolTest) VALUES ("+POut.Bool(bool1)+")";
			DataCore.NonQ(command);
			command="SELECT BoolTest FROM tempcore";
			table=DataCore.GetTable(command);
			bool2=PIn.Bool(table.Rows[0]["BoolTest"].ToString());
			if(bool1!=bool2) {
				throw new Exception();
			}
			command="DELETE FROM tempcore";
			DataCore.NonQ(command);
			retVal+="Bool, false: Passed.\r\n";
			//varchar255 Nonstandard Characters-----------------------------------------------------------
			varchar1=@"'!@#$%^&*()-+[{]}\`~,<.>/?'"";:=_"+"\r\n\t";
			varchar2="";
			command="INSERT INTO tempcore (TextTinyTest) VALUES ('"+POut.String(varchar1)+"')";
			DataCore.NonQ(command);
			command="SELECT TextTinyTest FROM tempcore";
			table=DataCore.GetTable(command);
			varchar2=PIn.String(table.Rows[0]["TextTinyTest"].ToString());
			if(varchar1!=varchar2) {
				throw new Exception();
			}
			command="DELETE FROM tempcore";
			DataCore.NonQ(command);
			retVal+="VarChar(255): Passed.\r\n";
			//VARCHAR2(4000)------------------------------------------------------------------------------
			varchar1=CreateRandomAlphaNumericString(4000); //Tested 4001 and it was too large as expected.
			command="INSERT INTO tempcore (TextSmallTest) VALUES ('"+POut.String(varchar1)+"')";
			DataCore.NonQ(command);
			command="SELECT TextSmallTest FROM tempcore";
			table=DataCore.GetTable(command);
			varchar2=PIn.String(table.Rows[0]["TextSmallTest"].ToString());
			if(varchar1!=varchar2) {
				throw new Exception();
			}
			command="DELETE FROM tempcore";
			DataCore.NonQ(command);
			retVal+="VarChar2(4000): Passed.\r\n";
			//clob:-----------------------------------------------------------------------------------------
			//tested up to 20MB in oracle.  (50MB however was failing: Chunk size error)
			//mysql mediumtext maxes out at about 16MB.
			string clobstring1=CreateRandomAlphaNumericString(10485760); //10MB should be larger than anything we store.
			string clobstring2="";
			OdSqlParameter param=new OdSqlParameter("param1",OdDbType.Text,clobstring1);
			command="INSERT INTO tempcore (TextLargeTest) VALUES ("+DbHelper.ParamChar+"param1)";
			DataCore.NonQ(command,param);
			command="SELECT TextLargeTest FROM tempcore";
			table=DataCore.GetTable(command);
			clobstring2=PIn.String(table.Rows[0]["TextLargeTest"].ToString());
			if(clobstring1!=clobstring2) {
				throw new Exception();
			}
			command="DELETE FROM tempcore";
			DataCore.NonQ(command);
			retVal+="Clob, Alpha-Numeric 10MB: Passed.\r\n";
			//clob:non-standard----------------------------------------------------------------------------------
			clobstring1=CreateRandomNonStandardString(8000000); //8MB is the max because many chars takes 2 bytes, and mysql maxes out at 16MB
			clobstring2="";
			param=new OdSqlParameter("param1",OdDbType.Text,clobstring1);
			command="INSERT INTO tempcore (TextLargeTest) VALUES ("+DbHelper.ParamChar+"param1)";
			DataCore.NonQ(command,param);
			command="SELECT TextLargeTest FROM tempcore";
			table=DataCore.GetTable(command);
			clobstring2=PIn.String(table.Rows[0]["TextLargeTest"].ToString());
			if(clobstring1!=clobstring2) {
				throw new Exception();
			}
			command="DELETE FROM tempcore";
			DataCore.NonQ(command);
			retVal+="Clob, Symbols and Chinese: Passed.\r\n";
			//clob:Rick Roller----------------------------------------------------------------------------------
			clobstring1=RickRoller(10485760); //10MB should be larger than anything we store.
			clobstring2="";
			param=new OdSqlParameter("param1",OdDbType.Text,clobstring1);
			command="INSERT INTO tempcore (TextLargeTest) VALUES ("+DbHelper.ParamChar+"param1)";
			DataCore.NonQ(command,param);
			command="SELECT TextLargeTest FROM tempcore";
			table=DataCore.GetTable(command);
			clobstring2=PIn.String(table.Rows[0]["TextLargeTest"].ToString());
			if(clobstring1!=clobstring2) {
				throw new Exception();
			}
			command="DELETE FROM tempcore";
			DataCore.NonQ(command);
			retVal+="Clob, Rick Roller: Passed.\r\n";
			//SHOW CREATE TABLE -----------------------------------------------------------------------
			//This command is needed in order to perform a backup.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="SHOW CREATE TABLE account";
				table=DataCore.GetTable(command);
				string createResult=PIn.ByteArray(table.Rows[0][1]);
				if(!createResult.StartsWith("CREATE TABLE")) {
					throw new Exception();
				}
				retVal+="SHOW CREATE TABLE: Passed.\r\n";
			}
			else {
				retVal+="SHOW CREATE TABLE: Not applicable to Oracle.\r\n";
			}
			//Single Command Split-------------------------------------------------------------------------
			varchar1="';\"";
			varchar2=";'';;;;\"\"\"\"'asdfsadsdaf'";
			command="INSERT INTO tempcore (TextTinyTest,TextSmallTest) VALUES ('"+POut.String(varchar1)+"','"+POut.String(varchar2)+"');";
			DataCore.NonQ(command);//Test the split function.
			command="SELECT TextTinyTest,TextSmallTest FROM tempcore";
			table=DataCore.GetTable(command);
			if(PIn.String(table.Rows[0]["TextTinyTest"].ToString())!=varchar1 || PIn.String(table.Rows[0]["TextSmallTest"].ToString())!=varchar2) {
				throw new ApplicationException();
			}
			command="DELETE FROM tempcore";
			DataCore.NonQ(command);
			retVal+="Single Command Split: Passed.";
			//Run multiple non-queries in one transaction--------------------------------------------------
			varchar1="A";
			varchar2="B";
			command="INSERT INTO tempcore (TextTinyTest) VALUES ('"+POut.String(varchar1)+"'); DELETE FROM tempcore; INSERT INTO tempcore (TextTinyTest) VALUES ('"+POut.String(varchar2)+"')";
			DataCore.NonQ(command);
			command="SELECT TextTinyTest FROM tempcore";
			table=DataCore.GetTable(command);
			if(PIn.String(table.Rows[0][0].ToString())!=varchar2) {
				throw new ApplicationException();
			}
			command="DELETE FROM tempcore";
			DataCore.NonQ(command);
			retVal+="Multi-Non-Queries: Passed.\r\n";
			//Cleanup---------------------------------------------------------------------------------------
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS tempcore";
				DataCore.NonQ(command);
				command="DROP TABLE IF EXISTS tempgroupconcat";
				DataCore.NonQ(command);
			}
			else {
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE tempcore'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				DataCore.NonQ(command);
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE tempgroupconcat'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				DataCore.NonQ(command);
			}
			retVal+="CoreTypes test done.\r\n";
			return retVal;
		}

		public static string CreateRandomAlphaNumericString(int length){
			StringBuilder result=new StringBuilder(length);
			Random rand=new Random();
			string randChrs="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			for(int i=0;i<length;i++){
				result.Append(randChrs[rand.Next(0,randChrs.Length-1)]);
			}
			return result.ToString();
		}

		public static string CreateRandomNonStandardString(int length) {
			StringBuilder result=new StringBuilder(length);
			Random rand=new Random();
			string randChrs="'!@#$%^&*()-+[{]}\\`~,<.>/?'\";:=_是像电子和质子这样的亚原子粒子之间的产生排斥力和吸引";
			for(int i=0;i<length;i++) {
				result.Append(randChrs[rand.Next(0,randChrs.Length-1)]);
			}
			return result.ToString();
		}

		public static string RickRoller(int length) {
			StringBuilder result=new StringBuilder(length);
			Random rand=new Random();
			string randChrs="I just couldn't take it anymore.  Kept getting the d--- song stuck in my head.";
			for(int i=0;i<length;i++) {
				result.Append(randChrs[i % randChrs.Length]);
			}
			return result.ToString();
		}

	}
}
