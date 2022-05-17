using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using CodeBase;
using OpenDentBusiness;
using PoorMansTSqlFormatterLib;
using PoorMansTSqlFormatterLib.Formatters;
using PoorMansTSqlFormatterLib.Parsers;
using PoorMansTSqlFormatterLib.Tokenizers;

namespace SlowQueryTool {
	class QueryLog {
		public string FilePath;
		public string MySqlVersion;
		public DateTime FirstQueryDate;
		public DateTime LastQueryDate;
		public TimeSpan HourOpen;
		public TimeSpan HourClose;
		public DateTime DateFrom;
		public DateTime DateTo;
		public Test Grade;
		public List<Query> ListQueries=new List<Query>();
		public List<QueryGroup> ListQueryGroups=new List<QueryGroup>();
		public Action<double> OnProgress=null;
		private bool _isInterrupted=false;
		
		public void Stop() {
			_isInterrupted=true;
		}

		public Test AnalyzeRowsExamined() {
			Test testResult=new Test();
			float totalQueries=ListQueries.Count;
			float queriesWithLowRowExamined=ListQueries.Where(x => x.RowsExamined < 5000).Count();//Anything lower than 5000 is likely too low
			decimal percent=(decimal)Math.Round(queriesWithLowRowExamined/totalQueries*100,2);
			testResult.ResultText+="Percent of queries with RowsExamined below 5000: "+percent+"%";
			if(percent > 20 && ListQueries.Count < 100) {//if the percent is high but the data is low
				testResult.ResultText+="\r\nOut of the "+totalQueries+" queries analyzed, "
					+queriesWithLowRowExamined+" queries had a low number of Rows Examined. However, because there were so few queries looked at, "
					+"this data does not tell us much. Look at the individual queries with a low number of Rows Examined and see if they should be "
					+"as low as they are.";
				testResult.Result=TestResults.INDECISIVE;
				testResult.PenColor=Color.YellowGreen;
			}
			else if(percent > 20) {//if over 15% of queries have less than 5000 rows examined
				testResult.ResultText+="\r\nOut of the "+totalQueries+" queries analyzed, "
					+queriesWithLowRowExamined+" queries had a low number of Rows Examined. The server may be unable to take the load. Check to see if "
					+"the queries with a low number of rows examined are accessing large tables.";
				testResult.Result=TestResults.FAILED;
				testResult.PenColor=Color.Red;
			}
			else {
				testResult.Result=TestResults.PASSED;
				testResult.PenColor=Color.Green;
			}
			testResult.ResultText+="\r\n\r\n";
			return testResult;
		}

		public Test AnalyzeThirdPartyQueries() {
			Test testResult=new Test();
			float totalQueries=ListQueries.Count;
			float numNonODQueries=ListQueries.Where(x => x.IsLikelyThirdParty).Count();
			decimal percent=(decimal)Math.Round(numNonODQueries/totalQueries*100,2);
			testResult.ResultText+="Percent of queries that do not match Open Dental Patterns: "+percent+"%\r\nNote: Some queries may be third party"
				+", but are not caught by our pattern check. Use best judgement.";
			if(percent > 15) {//if over 15% of queries are flagged as likely non-OD
				testResult.ResultText+="\r\nOut of the "+totalQueries+" queries analyzed, "+numNonODQueries+" queries did not follow our patterns. "
					+"This means that the slowness may be caused by a third party plugin. Double check that the \"Third Party\" queries look like "
					+"they do not come from our source code.";
				testResult.Result=TestResults.FAILED;
				testResult.PenColor=Color.Red;
			}
			else {
				testResult.Result=TestResults.PASSED;
				testResult.PenColor=Color.Green;
			}
			testResult.ResultText+="\r\n\r\n";
			return testResult;
		}

		public Test AnalyzeVictimQueries() {
			Test testResult=new Test();
			float totalQueries=ListQueries.Count;
			float numVictimQueries=ListQueries.Where(x => x.IsVictim).Count();
			decimal percent=(decimal)Math.Round(numVictimQueries/totalQueries*100,2);
			testResult.ResultText+="Percent of queries that were marked as \"Victim\": "+percent+"%";
			if(numVictimQueries > 0) {//Fail
				testResult.ResultText+="\r\nThere was at least one query that was marked as a victim query. This means that a short query was run, "
					+"examined few rows, and took a while to execute. Within 10s of that query, there was a large query that took a while to run. "
					+"Look at this large query as it may be causing a slow down.";
				testResult.Result=TestResults.FAILED;
				testResult.PenColor=Color.Red;
			}
			else {
				testResult.Result=TestResults.PASSED;
				testResult.PenColor=Color.Green;
			}
			testResult.ResultText+="\r\n\r\n";
			return testResult;
		}

		public Test AnalyzeUserQueries() {
			Test testResult=new Test();
			float totalQueries=ListQueries.Count;
			float numUserQueries=ListQueries.Where(x => x.IsLikelyUserQuery).Count();
			decimal percent=(decimal)Math.Round(numUserQueries/totalQueries*100,2);
			testResult.ResultText+="Percent of queries that were marked as \"User Query\": "+percent+"%";
			if(percent > 5) {//Fail
				testResult.ResultText+="\r\nOut of the "+totalQueries+" queries analyzed, "+numUserQueries+" queries were marked as user queries. "
					+"This means that the slowness may be caused by an employee running a user query frequently."+" View the user queries to verify "
					+"that they are in fact user queries.";
				testResult.Result=TestResults.FAILED;
				testResult.PenColor=Color.Red;
			}
			else {
				testResult.Result=TestResults.PASSED;
				testResult.PenColor=Color.Green;
			}
			testResult.ResultText+="\r\n\r\n";
			return testResult;
		}

		///<summary>Returns the worst queryGroups based on a ranking system explained in CalculateGroupWeight.</summary>
		public List<QueryGroup> AnalyzeWorstQueries() {
			return ListQueryGroups.Take(3).ToList();//already sorted by worst
		}

		///<summary>Fills the information for the individual queries being run.</summary>
		public void FillQueries() {
			string unformattedQuery="";
			string administratorCommand="";
			Query query=null;
			DateTime timeQueryRan=new DateTime();
			bool skipEntry=true;
			long lineNum=0;
			DateTime dateTimeLower=DateFrom.Date.Add(HourOpen);
			DateTime dateTimeUpper=DateTo.Date.Add(HourClose);
			double curPercentDone=0;
			if(!IsFileValid(FilePath)) {
				throw new Exception("Cannot access file.");
			}
			void addQueryIfNecessary(){
				if(query==null) {
					return;
				}
				//Add the last query
				query.FormatQuery(unformattedQuery);
				if(query.FormattedQuery.Trim()!="" && MiscUtils.Between(query.TimeRan.TimeOfDay,HourOpen,HourClose)) {
					//1-based counting.
					query.QueryNum=ListQueries.Count+1;
					ListQueries.Add(query);
				}				
			}
			bool getMySqlVersion(string line){
				if(lineNum!=1) {
					return false;
				}
				//Extract SQL version.
				if(line.Contains("5.5")) {
					MySqlVersion="5.5";
				}
				else if(line.Contains("5.6")) {
					MySqlVersion="5.6";
				}
				else {
					MySqlVersion="";
				}
				return true;				
			}
			using(var sr = new StreamReader(FilePath)) {
				if(sr.Peek() < 0){
					throw new Exception("File is empty");
				}
				long fileLength=sr.BaseStream.Length;
				while(sr.Peek()>=0){
					if(_isInterrupted) {
						throw new Exception("Analyzer stopped prematurely!");
					}
					++lineNum;
					string line=sr.ReadLine();
					if(getMySqlVersion(line)){
						continue;
					}
					double actualPercentDone=(sr.BaseStream.Position/(double)fileLength)*100;
					if(Math.Ceiling(actualPercentDone)!=Math.Ceiling(curPercentDone)) { //Throttle to prevent choking out the UI handler.
						curPercentDone=actualPercentDone;
						OnProgress?.Invoke(curPercentDone);
					}

					if(skipEntry) {
						if(line.StartsWith("# Time:")) {
							timeQueryRan=ExtractTimeRan(line);
							if(!MiscUtils.Between(timeQueryRan,dateTimeLower,dateTimeUpper)){ //Not in time range, keep skipping.
								continue;
							}
							//Time is in range so stop skipping and let this line fall through below.
							skipEntry=false;
						}
						else {//keep skipping.
							continue;
						}
					}
					//We found a # Time header.
					if(line.StartsWith("# Time:")) {
						timeQueryRan=ExtractTimeRan(line);
						if(!MiscUtils.Between(timeQueryRan,dateTimeLower,dateTimeUpper)) { //Not in time range, start skipping.
							skipEntry=true;
							continue;
						}
						if(FirstQueryDate==null || FirstQueryDate.Year < 1880) {
							FirstQueryDate=timeQueryRan;
						}
						LastQueryDate=timeQueryRan;
					}
					else if(line.StartsWith("# User@Host:")) {
						//It is the beginning of a new query, add the old one to the list
						addQueryIfNecessary();
						//We got this far for an entry so it is within our time range. Create new Query object and record datetime with current "timeQueryRan".
						query=new Query();
						unformattedQuery=administratorCommand;
						administratorCommand="";
						query.TimeRan=timeQueryRan;
						query.ComputerName=ExtractComputerName(line);
						query.ComputerIP=ExtractComputerIP(line);
					}
					else if(line.StartsWith("# Query_time:")) {
						query.QueryExecutionTime=ExtractQueryExecutionTime(line);
						query.LockTime=ExtractLockTime(line);
						query.RowsSent=ExtractRowsSent(line);
						query.RowsExamined=ExtractRowsExamined(line);
					}
					else if(line.StartsWith("# administrator")) {
						//Administrator commands should be recorded. Example: Init DB; taking a long time. In this case, the command we actually want
						//to record in the "query" object is on this line. The query part of the query is generally just a SET timestamp query.
						administratorCommand=ExtractAdministratorCommand(line);
					}
					else if(!line.StartsWith("#")) {//Filters unnecessary # line. Grabs the rest of the formatted queries.
						unformattedQuery+=(unformattedQuery=="" ? "" : "\r\n")+line;
					}
				}
			}
			//Add the last query.
			addQueryIfNecessary();
			if(DateFrom.Year < 1880 && DateTo.Year < 1880) {//if both are blank, show last two week only.
				DateTo=LastQueryDate.Date;
				DateFrom=(FirstQueryDate.Date < LastQueryDate.AddDays(-14).Date ? LastQueryDate.AddDays(-14).Date : FirstQueryDate.Date);
			}
			ListQueries=ListQueries.Where(x => x.TimeRan.Date.Between(DateFrom,DateTo)).ToList();
			CalculateIsVictim();		
		}

		///<summary>Groups the queries by similar queries. Then it calculate statistics and weights for each group.</summary>
		public void AnalyzeGroups() {
			ListQueries.ForEach(x => x.QueryGroupNum=FindQueryGroup(x));
			CalculateGroupStatistics(ListQueryGroups);
			CalculateGroupWeight(ListQueryGroups);
			ListQueryGroups=ListQueryGroups.OrderByDescending(x => x.GroupWeightRaw).ToList();
			for(int i=0;i<ListQueryGroups.Count;i++) {
				ListQueryGroups[i].GroupWeightRank=i+1;
			}
			CalculateGrade();
		}

		private void CalculateGrade() {
			DateTime firstQueryDate=ListQueries.OrderBy(x => x.TimeRan).First().TimeRan.Date;
			DateTime lastQueryDate=ListQueries.OrderBy(x => x.TimeRan).Last().TimeRan.Date;
			float numDays=Math.Max((float)(lastQueryDate-firstQueryDate).TotalDays,1);//total number of days in filtered list
			float totalWeights=ListQueryGroups.Sum(x => x.GroupWeightRaw);
			float executionTimeCutOff=(float)Math.Floor(ListQueries.OrderBy(x => x.QueryExecutionTime).First().QueryExecutionTime);
			if(executionTimeCutOff > 10) {//the range of supported cutoffs is between 2 and 10
				executionTimeCutOff=10;
			}
			else if(executionTimeCutOff < 2) {
				executionTimeCutOff=2;
			}
			//This adjusts the weight based on what they set the execution cutoff to. The total weight and weight calculations will be different if
			//lower execution time queries are not included. Looking at data from different total weights vs execution cutoff, the following equation
			//adjusts the total weight based on a cutoff between 2 and 10.
			if(executionTimeCutOff > 2) {
				totalWeights=totalWeights/(float)(0.022394185056*Math.Pow((executionTimeCutOff-2),3)-0.33641225252*Math.Pow((executionTimeCutOff-2),2)
					+1.4471042946*(executionTimeCutOff-2)+0.15617982398);
			}
			//Find the earliest a slow queries was run and the latest in a day
			float numberOfHours=numberOfHours=Math.Max(ListQueries.OrderBy(x => x.TimeRan.TimeOfDay).Last().TimeRan.Hour
					-(ListQueries.OrderBy(x => x.TimeRan.TimeOfDay).First().TimeRan.Hour),1);
			float weightPerDayPerHourOpen=totalWeights/Math.Max((float)Math.Log(numDays,2),1)/numberOfHours;
			Grade=new Test();
			if(weightPerDayPerHourOpen>=200) {
				Grade.Result=TestResults.F;
				Grade.PenColor=Color.DarkRed;
			}
			else if(weightPerDayPerHourOpen>=80) {
				Grade.Result=TestResults.D;
				Grade.PenColor=Color.OrangeRed;
			}
			else if(weightPerDayPerHourOpen>=60) {
				Grade.Result=TestResults.C;
				Grade.PenColor=Color.Orange;
			}
			else if(weightPerDayPerHourOpen>=30) {
				Grade.Result=TestResults.B;
				Grade.PenColor=Color.Green;
			}
			else {//Less than 30
				Grade.Result=TestResults.A;
				Grade.PenColor=Color.DarkGreen;
			}
		}

		///<summary>Determines the victim queries within the list. listQueries should be ordered by TimeRan before being passed in.</summary>
		private void CalculateIsVictim() {
			foreach(Query query in ListQueries) {
				if(query.RowsExamined > 1000 || query.QueryExecutionTime < 3) {//not small enough or did not take long enough to be a victim
					continue;
				}
				//Find queries ran 0-10 seconds after this query.
				List<Query> listQueriesInRange=ListQueries.Where(x => x.QueryNum!=query.QueryNum).
					Where(x => MiscUtils.Between(x.TimeRan,query.TimeRan,query.TimeRan.AddSeconds(10))).ToList();
				//if there is a large victim that took awhile to run, this one is a victim query
				query.IsVictim=listQueriesInRange.Any(x => x.RowsExamined > 100000 && x.QueryExecutionTime > 5);
				if(query.IsVictim) {
					query.Perpetrator=listQueriesInRange.First(x => x.RowsExamined > 100000 && x.QueryExecutionTime > 5).QueryNum;
				}			
			}
		}

		///<summary>Finds the appropriate query group to insert the query into.</summary>
		private long FindQueryGroup(Query query) {
			foreach(QueryGroup queryGroup in ListQueryGroups) {
				List<Query>listQueriesInGroup=queryGroup.ListQueriesInGroup;
				//Check the first three queries in the group (if there are 3)
				int lengthOfQueryInGroup=listQueriesInGroup[0].FormattedQuery.Length;
				int lengthOfQuery=query.FormattedQuery.Length;
				if(ComputeSimilarity(listQueriesInGroup[0].FormattedQuery.Substring(0,(lengthOfQueryInGroup > 100 ? 100 : lengthOfQueryInGroup)),
					query.FormattedQuery.Substring(0,(lengthOfQuery > 100 ? 100 : lengthOfQuery))) < 15)  
				{
					//if the first 100 characters (for efficiency) are close in similarity
					listQueriesInGroup.Add(query);
					return queryGroup.QueryGroupNum;
				}
			}
			//No match was found. Create a new group and insert the group
			QueryGroup queryGroupNew=new QueryGroup();
			queryGroupNew.ListQueriesInGroup.Add(query);
			queryGroupNew.QueryGroupNum=ListQueryGroups.Count+1;
			ListQueryGroups.Add(queryGroupNew);
			return queryGroupNew.QueryGroupNum;//Inserted into the group at the end
		}

		private void CalculateGroupWeight(List<QueryGroup> listQueryGroups) {
			//Ranking is done by weighting. The higher this weight is, the "worse" the query is. We rank the query groups as these contain queries that
			//are essentialy the same. The ranking takes into account the number of queries and the average execution time.
			//Higher number of queries increases the weight.
			//Higher average execution time increase the weight.
			foreach(QueryGroup group in ListQueryGroups) {
				//The number of queries is based on the magnitude. E.G. if 3035 are in the slow query log, the weight will be 4. 67 will be 2. 
				//Then it is multiplied by 2 to more closely equal the execution time in weight.
				float numOfQueriesWeight=(float)Math.Max(Math.Log(group.ListQueriesInGroup.Count,2),1);
				//The average execution time is increased to the power of ^1.2 as it is one of the key piece. This will give far more weight to 
				//queries with long execution time.
				float queryExecutionWeight=(float)Math.Pow((double)group.ExecutionTimeMedian,1.1);
				group.GroupWeightRaw=numOfQueriesWeight*queryExecutionWeight;
			}
		}

		private static void CalculateGroupStatistics(List<QueryGroup> listQueryGroups) {
			foreach(QueryGroup queryGroup in listQueryGroups) {
				//Execution Time Stats
				queryGroup.ExecutionTimeMax=(decimal)Math.Round(queryGroup.ListQueriesInGroup.Max(x => x.QueryExecutionTime),2);
				queryGroup.ExecutionTimeMin=(decimal)Math.Round(queryGroup.ListQueriesInGroup.Min(x => x.QueryExecutionTime),2);
				//Calculate the median
				float numQueries=queryGroup.ListQueriesInGroup.Count;
				List<Query> listOrderedQueries=queryGroup.ListQueriesInGroup.OrderBy(x => x.QueryExecutionTime).ToList();
				if((numQueries%2)==0) {//if it is even
					queryGroup.ExecutionTimeMedian=(decimal)Math.Round((listOrderedQueries[(listOrderedQueries.Count/2)-1].QueryExecutionTime
						+listOrderedQueries[(listOrderedQueries.Count/2)].QueryExecutionTime)/2,2);
				}
				else {//if it is odd
					//integer division will round down. e.g. 5 / 2 will be 2. 2 is the index of the middle element
					queryGroup.ExecutionTimeMedian=(decimal)Math.Round(listOrderedQueries[(listOrderedQueries.Count / 2)].QueryExecutionTime,2);
				}
				queryGroup.ExecutionTimeMean=(decimal)Math.Round(queryGroup.ListQueriesInGroup.Average(x => x.QueryExecutionTime),2);
				queryGroup.ExecutionTimeTotalTime=(decimal)Math.Round(queryGroup.ListQueriesInGroup.Sum(x => x.QueryExecutionTime),2);
				//RowsExaming
				queryGroup.RowsExaminedMax=queryGroup.ListQueriesInGroup.Max(x => x.RowsExamined);
				queryGroup.RowsExaminedMin=queryGroup.ListQueriesInGroup.Min(x => x.RowsExamined);
				listOrderedQueries=queryGroup.ListQueriesInGroup.OrderBy(x => x.RowsExamined).ToList();
				//Calculate the median
				if((numQueries%2)==0) {//if it is even
					queryGroup.RowsExaminedMedian=(listOrderedQueries[(listOrderedQueries.Count/2)-1].RowsExamined
						+listOrderedQueries[(listOrderedQueries.Count/2)].RowsExamined)/2;
				}
				else {//if it is odd
					  //integer division will round down. e.g. 5 / 2 will be 2. 2 is the index of the middle element
					queryGroup.RowsExaminedMedian=listOrderedQueries[listOrderedQueries.Count/2].RowsExamined;
				}
				queryGroup.RowsExaminedMean=(decimal)Math.Round(queryGroup.ListQueriesInGroup.Average(x => x.RowsExamined),2);
				//Locktime
				queryGroup.LockTimeMax=(decimal)Math.Round(queryGroup.ListQueriesInGroup.Max(x => x.LockTime),2);
				queryGroup.LockTimeMin=(decimal)Math.Round(queryGroup.ListQueriesInGroup.Min(x => x.LockTime),2);
				listOrderedQueries=queryGroup.ListQueriesInGroup.OrderBy(x => x.LockTime).ToList();
				//Calculate the median
				if((numQueries%2)==0) {//if it is even
					queryGroup.LockTimeMedian=(decimal)Math.Round((listOrderedQueries[(listOrderedQueries.Count/2)-1].LockTime
						+listOrderedQueries[(listOrderedQueries.Count/2)].LockTime)/2,2);
				}
				else {//if it is odd
					//integer division will round down. e.g. 5 / 2 will be 2. 2 is the index of the middle element
					queryGroup.LockTimeMedian=(decimal)Math.Round(listOrderedQueries[(listOrderedQueries.Count/2)].LockTime,2);
				}
				queryGroup.LockTimeMean=(decimal)Math.Round(queryGroup.ListQueriesInGroup.Average(x => x.LockTime),2);
				queryGroup.LockTimeTotalTime=(decimal)Math.Round(queryGroup.ListQueriesInGroup.Sum(x => x.LockTime),2);
				//Average Time between queries
				double totalTimeInSeconds=0;
				double totalSpans=0;
				for(int i=1;i<queryGroup.ListQueriesInGroup.Count;i++) {//Begin at second element
					totalSpans++;
					totalTimeInSeconds+=(queryGroup.ListQueriesInGroup[i].TimeRan-queryGroup.ListQueriesInGroup[i-1].TimeRan).TotalSeconds;
				}
				if(queryGroup.ListQueriesInGroup.Count > 1) {
					queryGroup.AverageTimeBetweenQueries=new TimeSpan(0,0,(int)(totalTimeInSeconds/totalSpans));
				}
				else {
					queryGroup.AverageTimeBetweenQueries=new TimeSpan();
				}
			}
		}

		///<summary>Extracts the time the query was run.</summary>
		private DateTime ExtractTimeRan(string line) {
			try {
				int currentIndex="# Time: ".Length;
				int year=PIn.Int("20"+line.Substring(currentIndex,2));
				currentIndex+=2;
				int month=PIn.Int(line.Substring(currentIndex,2));
				currentIndex+=2;
				int day=PIn.Int(line.Substring(currentIndex,2));
				currentIndex+=3;//3 as space between date and time
				int hour=PIn.Int(line.Substring(currentIndex,2));
				currentIndex+=3;//3 as to account for :
				int minute=PIn.Int(line.Substring(currentIndex,2));
				currentIndex+=3;//3 as to account for :
				int second=PIn.Int(line.Substring(currentIndex,2));
				return new DateTime(year,month,day,hour,minute,second);
			}
			catch {
				throw new Exception("Could not parse time for one of the queries.");
			}
		}

		private string ExtractComputerName(string line) {
			try {
				return line.Substring(line.LastIndexOf("@")+2,line.LastIndexOf("[")-(line.LastIndexOf("@")+2)-1);//-1 for the spaces
			}
			catch {
				throw new Exception("Could not parse Computer Name for one of the queries.");
			}
		}

		private string ExtractComputerIP(string line) {
			try {
				if(line.Contains("localhost")) {
					return "localhost";
				}
				else {
					return line.Substring(line.LastIndexOf("[")+1,line.LastIndexOf("]")-(line.LastIndexOf("[")+1));
				}
			}
			catch {
				throw new Exception("Could not parse Computer IP for one of the queries.");
			}
		}

		private float ExtractQueryExecutionTime(string line) {
			try {
				int indexStart=line.IndexOf("# Query_time: ")+"# Query_time: ".Length;
				return PIn.Float(line.Substring(indexStart,line.IndexOf("Lock_time:")-indexStart-2));//-2 for two spaces
			}
			catch {
				throw new Exception("Could not parse for Query Execution time.");
			}
		}

		private float ExtractLockTime(string line) {
			try {
				int indexStart=line.IndexOf("Lock_time: ")+"Lock_time: ".Length;
				return PIn.Float(line.Substring(indexStart,line.IndexOf("Rows_sent:")-indexStart-1));
			}
			catch {
				throw new Exception("Could not parse Computer Name for one of the queries.");
			}
		}

		private long ExtractRowsSent(string line) {
			try {
				int indexStart=line.IndexOf("Rows_sent: ")+"Rows_sent: ".Length;
				return PIn.Long(line.Substring(indexStart,line.IndexOf("Rows_examined:")-indexStart-2));
			}
			catch {
				throw new Exception("Could not parse Computer Name for one of the queries.");
			}
		}

		private long ExtractRowsExamined(string line) {
			try {
				int indexStart=line.IndexOf("Rows_examined: ")+"Rows_examined: ".Length;
				return PIn.Long(line.Substring(indexStart));
			}
			catch {
				throw new Exception("Could not parse Computer Name for one of the queries.");
			}
		}

		private string ExtractAdministratorCommand(string line) {
			try {
				int indexStart=line.IndexOf("administrator command: ")+"administrator command: ".Length;
				return line.Substring(indexStart);
			}
			catch {
				throw new Exception("Could not parse Administrator command for line: "+line);
			}
		}

		private static bool IsFileValid(string filePath) {
			try {
				var file=File.Open(filePath,FileMode.Open);
				file.Close();
				return true;
			}
			catch {
				return false;
			}
		}

		///<summary>Computes similarity between two strings. It returns a number representing their similarity. The closer to 0 the return value is,
		///the closer the strings are. The scale is from 0-100.</summary>
		public static int ComputeSimilarity(string s,string t) {
			if(string.IsNullOrEmpty(s)) {
				if(string.IsNullOrEmpty(t))
					return 0;
				return t.Length;
			}
			if(string.IsNullOrEmpty(t)) {
				return s.Length;
			}
			int n=s.Length;
			int m=t.Length;
			int[,] d=new int[n+1,m+1];
			// initialize the top and right of the table to 0, 1, 2, ...
			for(int i=0;i<=n;d[i,0]=i++);
			for(int j=1;j<=m;d[0,j]=j++);
			for(int i=1;i<=n;i++) {
				for(int j=1;j<=m;j++) {
					int cost=(t[j-1]==s[i-1]) ? 0 : 1;
					int min1=d[i-1,j]+1;
					int min2=d[i,j-1]+1;
					int min3=d[i-1,j-1]+cost;
					d[i,j]=Math.Min(Math.Min(min1,min2),min3);
				}
			}
			return d[n,m];
		}
	}

	public class Test {
		public string ResultText="";
		public Color PenColor=Color.Black;
		public TestResults Result;
	}

	public enum TestResults {
		PASSED,
		FAILED,
		INDECISIVE,
		A,
		B,
		C,
		D,
		F,
	}

	///<summary>A list of reasons a query was marked as third party.</summary>
	public enum ThirdPartyReason {
		///<summary>None.</summary>
		[Description("N/A")]
		None,
		///<summary>In our source code, we do not use ` to wrap around table names and columns. The exception for this is INSERT INTOs can have 
		///these.</summary>
		[Description("We do not use ` to wrap around table names and columns.")]
		GraveAccent,
		///<summary>We do not use regular joins in Open Dental queries.</summary>
		[Description("We do not use regular joins in our queries.")]
		Joins,
		///<summary>Key words such as select in our source code are never lower case. If we so these lower case key words, the query will be marked 
		///as third party</summary>
		[Description("We do not use lowercase keywords such as select.")]
		LowerCaseKeyWords,
		///<summary>When there are many underscores in aliases, we mark it as third party. We generally don't do this. Some P&I queries may get
		///flagged due to this.</summary>
		[Description("We do not use underscores in our table and column aliases for the most part.")]
		UnderscoreInAlias,
	}

	public class QueryGroup {
		///<summary>Primary key for a given query group.</summary>
		public long QueryGroupNum;
		///<summary>Statistics for a query group.</summary>
		public TimeSpan AverageTimeBetweenQueries;
		///<summary>Represents how "bad" the query group is. Only used when printing the worst queries.</summary>
		public float GroupWeightRaw;
		public float GroupWeightRank;
		public decimal ExecutionTimeMax;
		public decimal ExecutionTimeMin;
		public decimal ExecutionTimeMedian;
		public decimal ExecutionTimeMean;
		public decimal ExecutionTimeTotalTime;
		public decimal RowsExaminedMax;
		public decimal RowsExaminedMin;
		public decimal RowsExaminedMedian;
		public decimal RowsExaminedMean;
		public decimal LockTimeMax;
		public decimal LockTimeMin;
		public decimal LockTimeMedian;
		public decimal LockTimeMean;
		public decimal LockTimeTotalTime;
		///<summary>The queries themselves.</summary>
		public List<Query> ListQueriesInGroup=new List<Query>();
	}

	public class Query {
		///<summary>Represents the query. The query nums will be assigned by how the queries appear in the file. This will be be ordered by timeran.
		///</summary>
		public long QueryNum;
		///<summary>Represents the group a query is in.</summary>
		public long QueryGroupNum;
		public DateTime TimeRan;
		public string ComputerName;
		public string ComputerIP;
		public float QueryExecutionTime;
		public float LockTime;
		public long RowsSent;
		public long RowsExamined;
		public bool IsLikelyThirdParty;
		public ThirdPartyReason ThirdPartyReason=ThirdPartyReason.None;
		public bool IsLikelyUserQuery;
		public bool IsVictim;
		public long Perpetrator;
		public string UnformattedQuery="";
		public string FormattedQuery="";
		
		private static List<string> listQueryKeywords=new List<string> { "select ","where ","in ","from ","left join ","inner join","order by",
			"group by","having "};

		public void FormatQuery(string unformattedQuery) {
			//Remove all new lines and tabs
			unformattedQuery=unformattedQuery.Replace("\r\n"," ");
			unformattedQuery=unformattedQuery.Replace("\t","");
			//Break seperate queries into lines by ;
			unformattedQuery=unformattedQuery.Replace("; ",";\r\n");
			if(unformattedQuery.ToLower().Contains("use ")) {
				unformattedQuery=unformattedQuery.Substring(unformattedQuery.IndexOf(";",unformattedQuery.ToLower().IndexOf("use "))+1).Trim();
			}
			if(unformattedQuery.Contains("SET timestamp=")){
				unformattedQuery=(unformattedQuery.Substring(0,unformattedQuery.IndexOf("SET timestamp="))
					+unformattedQuery.Substring(unformattedQuery.IndexOf(";",unformattedQuery.IndexOf("SET timestamp="))+1)).Trim();
			}
			if(unformattedQuery.Trim()!="Init DB;") {//if this is a part of another query, just remove it. It's not the important slow part.
				unformattedQuery=unformattedQuery.Replace("Init DB;","").Trim();
			}
			unformattedQuery=unformattedQuery.Substring(0,unformattedQuery.LastIndexOf(";") + 1); // remove anything trailing after last semi colon
			UnformattedQuery=unformattedQuery;
			IsLikelyUserQuery=UnformattedQuery.Contains("@") || UnformattedQuery.Contains("Modified By");
			ThirdParty();
			SqlFormattingManager sqlFormatter=new SqlFormattingManager(new TSqlStandardTokenizer(),new TSqlStandardParser(),
				new TSqlStandardFormatter("\t",1,1000,false,true,false,true,true,false,true,true,false,false));
			sqlFormatter.Formatter.ErrorOutputPrefix="";
			FormattedQuery=sqlFormatter.Format(UnformattedQuery);
		}

		private void ThirdParty() {
			if(IsLikelyUserQuery) {
				return;//if its a user query, we are not gonna flag it as third party.
			}
			if(listQueryKeywords.Count(x => UnformattedQuery.Contains(x)) >= 3) {
				IsLikelyThirdParty=true;
				ThirdPartyReason=ThirdPartyReason.LowerCaseKeyWords;
				return;
			}
			if(UnformattedQuery.Contains("`") && !UnformattedQuery.StartsWith("INSERT INTO")) {
				ThirdPartyReason=ThirdPartyReason.GraveAccent;
				IsLikelyThirdParty=true;
				return;
			}
			if(!(UnformattedQuery.Contains("left join") || UnformattedQuery.Contains("LEFT JOIN"))//if they use regular joins, probably third party
				&& !(UnformattedQuery.Contains("right join") || UnformattedQuery.Contains("RIGHT JOIN"))
				&& !(UnformattedQuery.Contains("inner join") || UnformattedQuery.Contains("INNER JOIN"))
				&& !(UnformattedQuery.Contains("cross join") || UnformattedQuery.Contains("CROSS JOIN"))
				&& (UnformattedQuery.Contains(" join ") || UnformattedQuery.Contains(" JOIN ")))
			{
				ThirdPartyReason=ThirdPartyReason.Joins;
				IsLikelyThirdParty=true;
				return;
			}
			List<string> listAliases=new List<string>();
			List<int> positions=new List<int>();
			int pos=0;
			while((pos < UnformattedQuery.Length) && (pos=UnformattedQuery.IndexOf(" AS ",pos))!=-1) {
				positions.Add(pos);
				pos+=" AS ".Length;
			}
			foreach(int index in positions) {
				listAliases.Add(UnformattedQuery.Substring(index + 4,(UnformattedQuery.IndexOf(" ",index+4)==-1 ? UnformattedQuery.Length-1-index-4
					: UnformattedQuery.IndexOf(" ",index+4)-index-4)));
			}
			if(listAliases.Count(x => x.Contains("_")) > 4) {
				ThirdPartyReason=ThirdPartyReason.UnderscoreInAlias;
				IsLikelyThirdParty=true;
				return;
			}
		}

		///<summary>Converts a string into a list of its lines.</summary>
		private List<string> ToListOfLines(string str) {
			List<string> lines=new List<string>();
			using(StringReader reader=new StringReader(str)) {
				string line="";
				while(line!=null) {
					line=reader.ReadLine();
					if(line!=null) {
						lines.Add(line);
					}
				}
			}
			return lines;
		}

		///<summary>Converts a list of strings to one string.</summary>
		private string ToStringFromLines(List<string> listOfLines) {
			if(listOfLines.Count==0) {
				return "";
			}
			StringBuilder str=new StringBuilder();
			foreach(string line in listOfLines) {
				str.AppendLine(line);
			}
			string text=str.ToString();
			text=text.Substring(0,text.Length-2);
			return text;//Removes extra new line
		}
	}
}
