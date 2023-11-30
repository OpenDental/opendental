using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using PoorMansTSqlFormatterLib;
using PoorMansTSqlFormatterLib.Formatters;
using PoorMansTSqlFormatterLib.Parsers;
using PoorMansTSqlFormatterLib.Tokenizers;
using SlowQueryLogTool;
using SlowQueryLogTool.UI;

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
		public Action<ProgressFillQueries> OnProgress=null;
		private bool _isInterrupted=false;
		private ProgressFillQueries _progressFillQueries=null;
		private List<QueryGroup> _listQueryGroupsDELETE=new List<QueryGroup>();
		private List<QueryGroup> _listQueryGroupsINSERT=new List<QueryGroup>();
		private List<QueryGroup> _listQueryGroupsSELECT=new List<QueryGroup>();
		private List<QueryGroup> _listQueryGroupsUPDATE=new List<QueryGroup>();
		private List<QueryGroup> _listQueryGroupsNonCRUD=new List<QueryGroup>();

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
			StringBuilder unformattedQuery=new StringBuilder();
			string administratorCommand="";
			Query query=null;
			DateTime timeQueryRan=new DateTime();
			bool isQueryBetweenBusinessHours=true;
			long lineNum=0;
			DateTime dateTimeLower=DateFrom.Date.Add(HourOpen);
			DateTime dateTimeUpper=DateTo.Date.Add(HourClose);
			if(!IsFileValid(FilePath)) {
				throw new Exception("Cannot access file.");
			}
			_progressFillQueries=new ProgressFillQueries();
			using(StreamReader streamReader=new StreamReader(FilePath)) {
				if(streamReader.Peek() < 0) {
					throw new Exception("File is empty");
				}
				long fileLength=streamReader.BaseStream.Length;
				while(streamReader.Peek()>=0) {
					if(_isInterrupted) {
						throw new Exception("Analyzer stopped prematurely!");
					}
					++lineNum;
					string line=streamReader.ReadLine();
					if(lineNum==1) {
						SetMySqlVersion(line);
						continue;
					}
					double percentDone=(streamReader.BaseStream.Position/(double)fileLength)*100;
					_progressFillQueries.PercentParsing=percentDone;
					OnProgress?.Invoke(_progressFillQueries);
					bool isTimeLine=line.StartsWith("# Time:");
					if(isTimeLine) {
						timeQueryRan=ExtractTimeRan(line);
						isQueryBetweenBusinessHours=(timeQueryRan.Between(dateTimeLower,dateTimeUpper) && timeQueryRan.TimeOfDay.Between(HourOpen,HourClose));
					}
					//Skip over any slow query that didn't run between business hours.
					if(!isQueryBetweenBusinessHours) {
						continue;
					}
					//We found a # Time header.
					if(isTimeLine) {
						if(FirstQueryDate==null || FirstQueryDate.Year < 1880) {
							FirstQueryDate=timeQueryRan;
						}
						LastQueryDate=timeQueryRan;
					}
					else if(line.StartsWith("# User@Host:")) {
						//It is the beginning of a new query, add the old one to the list
						AddQueryIfNecessary(query,unformattedQuery.ToString());
						//We got this far for an entry so it is within our time range. Create new Query object and record datetime with current "timeQueryRan".
						query=new Query();
						unformattedQuery.Clear();
						unformattedQuery.Append(administratorCommand);
						administratorCommand="";
						query.TimeRan=timeQueryRan;
						query.ComputerName=ExtractComputerName(line);
						query.UserName=ExtractUserName(line);
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
						if(unformattedQuery.Length > 0) {
							unformattedQuery.Append("\r\n");
						}
						unformattedQuery.Append(line);
					}
				}
			}
			//Add the last query.
			AddQueryIfNecessary(query,unformattedQuery.ToString());
			_progressFillQueries.PercentParsing=100;
			OnProgress?.Invoke(_progressFillQueries);
			//Queries have already been filtered out at this point unless the filters are blank.
			if(DateFrom.Year < 1880 && DateTo.Year < 1880) {
				//Show up to two weeks prior to the LastQueryDate.
				DateTo=LastQueryDate.Date;
				DateFrom=(FirstQueryDate.Date < LastQueryDate.AddDays(-14).Date ? LastQueryDate.AddDays(-14).Date : FirstQueryDate.Date);
				ListQueries=ListQueries.FindAll(x => x.TimeRan.Date.Between(DateFrom,DateTo));
			}
		}

		private void SetMySqlVersion(string line) {
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
		}

		private void AddQueryIfNecessary(Query query,string unformattedQuery) {
			if(query==null) {
				return;
			}
			//Add the last query
			query.SetUnformattedQuery(unformattedQuery);
			if(query.UnformattedQuery.Trim()!="") {
				//1-based counting.
				query.QueryNum=ListQueries.Count+1;
				ListQueries.Add(query);
			}
		}

		///<summary>Groups the queries by similar queries. Then it calculate statistics and weights for each group.</summary>
		public void AnalyzeGroups() {
			CalculateGroupStatistics();
			CalculateGroupWeight();
			ListQueryGroups=ListQueryGroups.OrderByDescending(x => x.GroupWeightRaw).ToList();
			for(int i=0;i<ListQueryGroups.Count;i++) {
				ListQueryGroups[i].GroupWeightRank=i+1;
			}
			CalculateGrade();
		}

		private bool AreQueriesSimilar(Query queryA,Query queryB) {
			//The closer the distance is to 0, the closer the strings are to each other.
			//The SlowQueryLog tool considers queries 'similar' if the first 100 characters are within a Levenshtein distance of 15.
			int threshold=15;
			//Create temporary variables that can be swapped around depending on which array is shorter.
			//C# is much faster at comparing integers than characters so we convert the SimilarityString into a SimilarityArray (of ints).
			int[] sourceArray=queryA.SimilarityArray;
			int[] targetArray=queryB.SimilarityArray;
			int sourceArrayLength=sourceArray.Length;
			int targetArrayLength=targetArray.Length;
			//Compare the lengths of the two queries.
			//If the strings (aka similarty arrays) differ by more than the allotted threshold then these queries are NOT similar.
			//This is because it would take at least 15 'insert' steps to get the strings to match up.
			if(Math.Abs(sourceArrayLength - targetArrayLength) > threshold) {
				return false;
			}
			//For speed purposes, always utilize the shorter query as the 'source' array.
			if(sourceArrayLength > targetArrayLength) {
				//Swap the arrays around and update the length variables.
				int[] tempArray=targetArray;
				targetArray=sourceArray;
				sourceArray=tempArray;
				sourceArrayLength=sourceArray.Length;
				targetArrayLength=targetArray.Length;
			}
			//Levenshtein distance algorithm moves along a matrix in order to determine the minimum number of steps it takes to make two strings equate.
			//Horizontally movements imply 'insertions', vertical movements imply 'deletions', and diagonal movements imply 'substitutions'.
			//Finally, in the bottom right of the matrix will be a number that yields how similar the two strings are (the lower the number, the more similar).
			/********************
			     M|E|N|T|O|R
			   0|1|2|3|4|5|6
			 C 1|1|2|3|4|5|6
			 E 2|2|1|2|3|4|5
			 N 3|3|2|1|2|3|4
			 T 4|4|3|2|1|2|3
			 E 5|5|4|3|2|2|3
			 R 6|6|5|4|3|3|2
			 S 7|7|6|5|4|4|3
			 ********************/
			//Initialize integer arrays for the Levenshtein distance algorithm.
			int[] distanceArrayCurrent=new int[sourceArrayLength + 1];
			int[] distanceArrayMinus1=new int[sourceArrayLength + 1];
			int[] distanceArrayMinus2=new int[sourceArrayLength + 1];
			int[] distanceArraySwap;
			//Set the top of the horizontal array from zero to the length of the array (number of characters in the first query).
			for(int i=0;i<=sourceArrayLength;i++) {
				distanceArrayCurrent[i]=i;
			}
			int indexTarget=0;
			int indexSourceMinus1=0;
			int indexSourceMinus2=-1;
			for(int j=1;j<=targetArrayLength;j++) {
				//For speed purposes, use a rotating set of three arrays rather than a massive matrix as visualized above (which is easier to understand, but slower).
				distanceArraySwap=distanceArrayMinus2;
				distanceArrayMinus2=distanceArrayMinus1;
				distanceArrayMinus1=distanceArrayCurrent;
				distanceArrayCurrent=distanceArraySwap;
				int minimumDistance=int.MaxValue;
				distanceArrayCurrent[0]=j;
				indexSourceMinus1=0;
				indexSourceMinus2=-1;
				//Calculate the minimum number of instructions it would take to turn the source query into the target query.
				for(int i=1;i<=sourceArrayLength;i++) {
					int cost=(sourceArray[indexSourceMinus1]==targetArray[indexTarget] ? 0 : 1);
					//Figure out which instruction is the quickest to make.
					int delete=distanceArrayCurrent[indexSourceMinus1] + 1;
					int insert=distanceArrayMinus1[i] + 1;
					int substitute=distanceArrayMinus1[indexSourceMinus1] + cost;
					//The fastest instruction is the one with the minimum value out of the three instructions.
					int min=(delete > insert)
						? (insert > substitute ? substitute : insert)
						: (delete > substitute ? substitute : delete);
					if(i > 1 && j > 1 && sourceArray[indexSourceMinus2]==targetArray[indexTarget] && sourceArray[indexSourceMinus1]==targetArray[j - 2]) {
						min=Math.Min(min,distanceArrayMinus2[indexSourceMinus2] + cost);
					}
					distanceArrayCurrent[i]=min;
					//Keep track of the overall minimum distance while crawling through the queries.
					if(min < minimumDistance) {
						minimumDistance=min;
					}
					indexSourceMinus1++;
					indexSourceMinus2++;
				}
				indexTarget++;
				//At this point, if we have already exceeded the threshold allotted; these strings are not similar enough.
				if(minimumDistance > threshold) {
					return false;
				}
			}
			//The closer the distance is to 0, the closer the strings are to each other.
			//The SlowQueryLog tool considers queries 'similar' if the first 100 characters are within a Levenshtein distance of 15.
			int result=distanceArrayCurrent[sourceArrayLength];
			if(result<=threshold) {
				return true;
			}
			return false;
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
		public void CalculateIsVictim() {
			double countTotalQueries=ListQueries.Count;
			double countProcessedQueries=0;
			for(int i=0;i<ListQueries.Count;i++) {
				if(_isInterrupted) {
					throw new Exception("Analyzer stopped prematurely!");
				}
				Query query=ListQueries[i];
				countProcessedQueries=(i + 1);
				double percentDone=((countProcessedQueries / countTotalQueries) * 100);
				_progressFillQueries.PercentVictim=percentDone;
				if(query.RowsExamined > 1000 || query.QueryExecutionTime < 3) {//not small enough or did not take long enough to be a victim
					OnProgress?.Invoke(_progressFillQueries);
					continue;
				}
				//Find queries ran 0-10 seconds after this query.
				List<Query> listQueriesInRange=new List<Query>();
				DateTime lowerBound=query.TimeRan;
				DateTime upperBound=query.TimeRan.AddSeconds(10);
				for(int j=i;j<ListQueries.Count;j++) {
					Query queryInRange=ListQueries[j];
					if(queryInRange.QueryNum==query.QueryNum) {
						continue;//The first one should always match, move onto the next query if there is one.
					}
					if(!queryInRange.TimeRan.Between(lowerBound,upperBound)) {
						break;//The list of queries should be ordered by TimeRan so there is no reason to keep considering queries later in the list.
					}
					listQueriesInRange.Add(queryInRange);
				}
				//Look for a victim query that took awhile to run.
				Query queryVictim=listQueriesInRange.FirstOrDefault(x => x.RowsExamined > 100000 && x.QueryExecutionTime > 5);
				query.IsVictim=false;
				if(queryVictim!=null) {
					query.IsVictim=true;
					query.Perpetrator=queryVictim.QueryNum;
				}
				OnProgress?.Invoke(_progressFillQueries);
			}
			_progressFillQueries.PercentVictim=100;
			OnProgress?.Invoke(_progressFillQueries);
		}

		private void CalculateGroupWeight() {
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

		private void CalculateGroupStatistics() {
			foreach(QueryGroup queryGroup in ListQueryGroups) {
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

		public void CalculateThirdParyScores() {
			double countTotalQueries=ListQueries.Count;
			double countProcessedQueries=0;
			for(int i=0;i<ListQueries.Count;i++) {
				if(_isInterrupted) {
					throw new Exception("Analyzer stopped prematurely!");
				}
				countProcessedQueries=(i + 1);
				double percentDone=((countProcessedQueries / countTotalQueries) * 100);
				_progressFillQueries.PercentThirdPartyScoring=percentDone;
				ListQueries[i].CalculateThirdParyScore();
				OnProgress?.Invoke(_progressFillQueries);
			}
			_progressFillQueries.PercentThirdPartyScoring=100;
			OnProgress?.Invoke(_progressFillQueries);
		}

		///<summary>Finds the appropriate query group to insert the query into.</summary>
		public void CreateQueryGroups() {
			ListQueryGroups.Clear();
			_listQueryGroupsDELETE.Clear();
			_listQueryGroupsINSERT.Clear();
			_listQueryGroupsSELECT.Clear();
			_listQueryGroupsUPDATE.Clear();
			_listQueryGroupsNonCRUD.Clear();
			Dictionary<string,List<Query>> dictSimilarityStringQueries=ListQueries.GroupBy(x => x.SimilarityString)
				.ToDictionary(x => x.Key,x => x.ToList());
			double countTotalQueries=dictSimilarityStringQueries.Count;
			double countGroupedQueries=0;
			foreach(var kvp in dictSimilarityStringQueries) {
				if(_isInterrupted) {
					throw new Exception("Analyzer stopped prematurely!");
				}
				countGroupedQueries++;
				double percentDone=((countGroupedQueries / countTotalQueries) * 100);
				_progressFillQueries.PercentGrouping=percentDone;
				QueryGroup queryGroup=GetQueryGroup(kvp.Value.First());
				foreach(Query query in kvp.Value) {
					queryGroup.ListQueriesInGroup.Add(query);
					query.QueryGroupNum=queryGroup.QueryGroupNum;
				}
				OnProgress?.Invoke(_progressFillQueries);
			}
			_progressFillQueries.PercentGrouping=100;
			OnProgress?.Invoke(_progressFillQueries);
		}

		///<summary>Extracts the time the query was run.</summary>
		private DateTime ExtractTimeRan(string line) {
			try {
				int currentIndex="# Time: ".Length;
				int year=SIn.Int("20"+line.Substring(currentIndex,2));
				currentIndex+=2;
				int month=SIn.Int(line.Substring(currentIndex,2));
				currentIndex+=2;
				int day=SIn.Int(line.Substring(currentIndex,2));
				currentIndex+=3;//3 as space between date and time
				int hour=SIn.Int(line.Substring(currentIndex,2));
				currentIndex+=3;//3 as to account for :
				int minute=SIn.Int(line.Substring(currentIndex,2));
				currentIndex+=3;//3 as to account for :
				int second=SIn.Int(line.Substring(currentIndex,2));
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

		private string ExtractUserName(string line) {
			try { 
				return line.Substring(line.IndexOf(":")+1, line.IndexOf("[") - (line.IndexOf(":")+1)).Trim();
			}
			catch {
				throw new Exception("Could not parse user name for one of the queries.");
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
				return SIn.Float(line.Substring(indexStart,line.IndexOf("Lock_time:")-indexStart-2));//-2 for two spaces
			}
			catch {
				throw new Exception("Could not parse for Query Execution time.");
			}
		}

		private float ExtractLockTime(string line) {
			try {
				int indexStart=line.IndexOf("Lock_time: ")+"Lock_time: ".Length;
				return SIn.Float(line.Substring(indexStart,line.IndexOf("Rows_sent:")-indexStart-1));
			}
			catch {
				throw new Exception("Could not parse Computer Name for one of the queries.");
			}
		}

		private long ExtractRowsSent(string line) {
			try {
				int indexStart=line.IndexOf("Rows_sent: ")+"Rows_sent: ".Length;
				return SIn.Long(line.Substring(indexStart,line.IndexOf("Rows_examined:")-indexStart-2));
			}
			catch {
				throw new Exception("Could not parse Computer Name for one of the queries.");
			}
		}

		private long ExtractRowsExamined(string line) {
			try {
				int indexStart=line.IndexOf("Rows_examined: ")+"Rows_examined: ".Length;
				return SIn.Long(line.Substring(indexStart));
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

		public void FormatQueries() {
			double countTotalQueries=ListQueries.Count;
			double countProcessedQueries=0;
			for(int i=0;i<ListQueries.Count;i++) {
				if(_isInterrupted) {
					throw new Exception("Analyzer stopped prematurely!");
				}
				countProcessedQueries=(i + 1);
				double percentDone=((countProcessedQueries / countTotalQueries) * 100);
				_progressFillQueries.PercentFormatting=percentDone;
				ListQueries[i].FormatQuery();
				OnProgress?.Invoke(_progressFillQueries);
			}
			_progressFillQueries.PercentFormatting=100;
			OnProgress?.Invoke(_progressFillQueries);
		}

		private QueryGroup GetQueryGroup(Query query) {
			List<QueryGroup> listQueryGroupsSubGroup;
			string similarityStringToUpper=query.SimilarityString.ToUpper();
			if(similarityStringToUpper.StartsWith("SELECT")) {
				listQueryGroupsSubGroup=_listQueryGroupsSELECT;
			}
			else if(similarityStringToUpper.StartsWith("INSERT")) {
				listQueryGroupsSubGroup=_listQueryGroupsINSERT;
			}
			else if(similarityStringToUpper.StartsWith("DELETE")) {
				listQueryGroupsSubGroup=_listQueryGroupsDELETE;
			}
			else if(similarityStringToUpper.StartsWith("UPDATE")) {
				listQueryGroupsSubGroup=_listQueryGroupsUPDATE;
			}
			else {
				listQueryGroupsSubGroup=_listQueryGroupsNonCRUD;
			}
			QueryGroup queryGroup=GetSimilarQueryGroup(query,listQueryGroupsSubGroup);
			if(queryGroup==null) {
				//No match was found. Create a new group and insert the group.
				queryGroup=new QueryGroup();
				queryGroup.QueryGroupNum=ListQueryGroups.Count+1;
				listQueryGroupsSubGroup.Add(queryGroup);
				ListQueryGroups.Add(queryGroup);
			}
			return queryGroup;
		}

		private QueryGroup GetSimilarQueryGroup(Query query,List<QueryGroup> listQueryGroups) {
			//Loop through every QueryGroup object and compute similarity on the similarity string passed in until a match is found.
			foreach(QueryGroup queryGroup in listQueryGroups) {
				if(_isInterrupted) {
					throw new Exception("Analyzer stopped prematurely!");
				}
				if(AreQueriesSimilar(queryGroup.ListQueriesInGroup.First(),query)) {
					return queryGroup;
				}
			}
			return null;
		}

		private bool IsFileValid(string filePath) {
			try {
				var file=File.Open(filePath,FileMode.Open);
				file.Close();
				return true;
			}
			catch {
				return false;
			}
		}
	}

	public class ProgressFillQueries {
		public double PercentParsing;
		public double PercentVictim;
		public double PercentGrouping;
		public double PercentFormatting;
		public double PercentThirdPartyScoring;
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

	///<summary>A structure to hold ML model prediction scores to determine if a query is third party or not.</summary>
	public class ThirdPartyScore {
		private float _isOpenDentalQuery;
		private float _isThirdPartyQuery;

		public float IsOpenDentalQueryScore {
			get {
				return (float)Math.Round(_isOpenDentalQuery * 100,2);
			}
		}

		public float IsThirdPartyQueryScore {
			get {
				return (float)Math.Round(_isThirdPartyQuery * 100,2);
			}
		}

		public ThirdPartyScore(float isOpenDentalQuery,float isThirdPartyQuery) {
			_isOpenDentalQuery=isOpenDentalQuery;
			_isThirdPartyQuery=isThirdPartyQuery;
		}
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
		public string UserName;
		public float QueryExecutionTime;
		public float LockTime;
		public long RowsSent;
		public long RowsExamined;
		public bool IsVictim;
		public long Perpetrator;
		public string UnformattedQuery="";

		private string _formattedQuery=null;
		public string FormattedQuery {
			get {
				if(_formattedQuery==null) {
					FormatQuery();
				}
				return _formattedQuery;
			}
		}

		private string _similarityString=null;
		public string SimilarityString {
			get {
				if(_similarityString==null) {
					_similarityString=FormattedQuery.Substring(0,(FormattedQuery.Length > 100 ? 100 : FormattedQuery.Length));
				}
				return _similarityString;
			}
		}

		private int[] _similarityArray=null;
		public int[] SimilarityArray {
			get {
				if(_similarityArray==null) {
					_similarityArray=SimilarityString.Select(x => Convert.ToInt32(x)).ToArray();
				}
				return _similarityArray;
			}
		}

		public bool IsLikelyUserQuery {
			get {
				return UnformattedQuery.Contains("@") || UnformattedQuery.Contains("Modified By");
			}
		}

		public bool IsLikelyThirdParty {
			get {
				return ThirdPartyScore.IsThirdPartyQueryScore > 70;
			}
		}
		
		private ThirdPartyScore _thirdPartyScore=null;
		public ThirdPartyScore ThirdPartyScore {
			get {
				if(_thirdPartyScore==null) {
					CalculateThirdParyScore();
				}
				return _thirdPartyScore;
			}
		}

		public void CalculateThirdParyScore() {
			ThirdPartyQueryModel.ModelInput modelInput=new ThirdPartyQueryModel.ModelInput();
			modelInput.Query_Text=UnformattedQuery;
			ThirdPartyQueryModel.ModelOutput modelOutput=new ThirdPartyQueryModel.ModelOutput();
			modelOutput=ThirdPartyQueryModel.Predict(modelInput);
			_thirdPartyScore=new ThirdPartyScore(modelOutput.Score[0],modelOutput.Score[1]);
		}

		public void FormatQuery() {
			TSqlStandardFormatter tSqlStandardFormatter=new TSqlStandardFormatter("\t",
				spacesPerTab: 1,
				maxLineWidth: 1000,
				expandCommaLists: false,
				trailingCommas: true,
				spaceAfterExpandedComma: false,
				expandBooleanExpressions: true,
				expandCaseStatements: true,
				expandBetweenConditions: false,
				breakJoinOnSections: true,
				uppercaseKeywords: true,
				htmlColoring: false,
				keywordStandardization: false);
			SqlFormattingManager sqlFormatter=new SqlFormattingManager(tokenizer: new TSqlStandardTokenizer(),
				parser: new TSqlStandardParser(),
				formatter: tSqlStandardFormatter);
			sqlFormatter.Formatter.ErrorOutputPrefix="";
			_formattedQuery=sqlFormatter.Format(UnformattedQuery);
		}

		///<summary>Manipuilates the unformatted query passed in in order to set the UnformattedQuery field on this query object.</summary>
		public void SetUnformattedQuery(string unformattedQuery) {
			//Remove all new lines and tabs
			unformattedQuery=unformattedQuery.Replace("\r\n"," ");
			unformattedQuery=unformattedQuery.Replace("\t","");
			//Break seperate queries into lines by ;
			//Not every string in the following list will be a query (some content within a string column could simply utilize the ';' char).
			//However, we want to explicitly look for common queries that can be removed (e.g. 'SET timestamp=1694571362;').
			List<string> listQueries=unformattedQuery.Split(new char[] { ';' },StringSplitOptions.RemoveEmptyEntries).ToList();
			//Loop through these 'queries' backwards and remove entries that start in an extremely specific manner.
			for(int i=listQueries.Count-1;i>=0;i--) {
				string queryToLower=listQueries[i].Trim().ToLower();
				if(queryToLower.StartsWith("set timestamp=")
					|| queryToLower.StartsWith("use "))
				{
					listQueries.RemoveAt(i);
				}
			}
			//Remove the Init DB query when there are other queries involved.
			//Preserve the Init DB query when it is by itself.
			//This means there is an issue with the server outside of MySQL that is causing slowness (hardware, antivirus, backups, windows, etc).
			if(listQueries.Count > 1) {
				listQueries.Remove("Init DB");
			}
			UnformattedQuery=string.Join(";\r\n",listQueries.Select(x => x.Trim()));
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
