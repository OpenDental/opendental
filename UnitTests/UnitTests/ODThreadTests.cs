using CodeBase;
using DataConnectionBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace UnitTests.ODThread_Tests {
	[TestClass]
	public class ODThreadTests:TestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			RunTestsAgainstDirectConnection();//Run against direct connection as these tests have many queries ran and are not relevant to middle tier.
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
			RevertMiddleTierSettingsIfNeeded();
			//Some weird database connection stuff happens in this class so set the main db connection back to the UnitTest db.
			UnitTestsCore.DatabaseTools.SetDbConnection(UnitTestDbName,false);
		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		[TestMethod]
		///<summary>The ThreadStatic database context should be inherited by any child thread that is spawned.</summary>
		public void ODThread_SetDbT_ParentChildDatabaseContext() {
			Exception ex=null;
			string methodName=MethodBase.GetCurrentMethod().Name;
			string databaseName=POut.String(methodName).ToLower();
			string odThreadGroupName=methodName+"_GroupName";
			//The main thread needs to connect to a unique database that the child thread should NOT end up getting connected to.
			//Use a query to get our current database context because unit tests utilizes a connection string and we only care about DataConnection.Database
			string databaseMain=LargeTableHelper.GetCurrentDatabase();//should be pointing to a 'unittestXXX' database
			Assert.IsFalse(string.IsNullOrEmpty(databaseMain));
			try {
				ODThread odThreadParent=new ODThread(workerParent => {
					//The parent thread needs to connect to a database that the main thread is not connected to.
					CreateDatabaseIfNeeded(databaseName);
					new DataConnection().SetDbT("localhost",databaseName,"root","","","",DatabaseType.MySql);
					VerifyCurrentDatabaseName(databaseName);
					//Now the parent should spawn a child thread which should default to the thread specific database connection from the parent.
					ODThread odThreadChild=new ODThread(workerChild => {
						//The child thread should default to the database context of its parent and NOT the database context from the main thread.
						VerifyCurrentDatabaseName(databaseName);
					});
					odThreadChild.AddExceptionHandler(e => ex=e);
					odThreadChild.Name=MethodBase.GetCurrentMethod().Name+"_Child";
					odThreadChild.GroupName=odThreadGroupName;
					odThreadChild.Start();
					odThreadChild.Join(Timeout.Infinite);
					VerifyCurrentDatabaseName(databaseName);
				});
				odThreadParent.AddExceptionHandler(e => ex=e);
				odThreadParent.Name=MethodBase.GetCurrentMethod().Name+"_Parent";
				odThreadParent.GroupName=odThreadGroupName;
				odThreadParent.Start();
				odThreadParent.Join(Timeout.Infinite);//Manually join the parent to the main thread so that we know for certain that our child thread exists.
				VerifyCurrentDatabaseName(databaseMain);
			}
			catch(Exception e) {
				ex=e;
			}
			finally {
				DropDatabase(databaseName);
			}
			Assert.IsNull(ex,ex?.Message);//Asserts do not bubble up as expected to the main thread so do a manual assert at the end of the day.
		}

		[TestMethod]
		///<summary>The following test calls setDb within a child thread. This will change the static database connection for all threads. The 
		///parent thread, or any other thread already alive, should not have their context changed.</summary>
		public void ODThread_SetDb_ChildContext() {
			Exception ex=null;
			string methodName=MethodBase.GetCurrentMethod().Name;
			string databaseName=POut.String(methodName).ToLower();
			string odThreadGroupName=methodName+"_GroupName";
			string databaseMain=LargeTableHelper.GetCurrentDatabase();//should be pointing to a 'unittestXXX' database
			Assert.IsFalse(string.IsNullOrEmpty(databaseMain));
			try {
				ODThread odThreadAffect=new ODThread((o) => {
					string databaseBefore=LargeTableHelper.GetCurrentDatabase();//should be pointing to a 'unittestXXX' database
					Assert.AreEqual(databaseMain,databaseBefore);
					ODThread odThreadChangeDb=new ODThread(workerChild => {
						//The parent thread needs to call SetDb on a different database than the main thread is connected to.
						CreateDatabaseIfNeeded(databaseName);
						new DataConnection().SetDb("localhost",databaseName,"root","","","",DatabaseType.MySql);
						VerifyCurrentDatabaseName(databaseName);
					});
					odThreadChangeDb.AddExceptionHandler(e => ex=e);
					odThreadChangeDb.Name=MethodBase.GetCurrentMethod().Name+"_Child";
					odThreadChangeDb.GroupName=odThreadGroupName;
					//This thread needs to only start the above thread and then make sure it is still connected to the unittestXXX database when complete.
					odThreadChangeDb.Start();
					odThreadChangeDb.Join(Timeout.Infinite);//Manually join the parent to the main thread so that we know for certain that our child thread exists.
					VerifyCurrentDatabaseName(databaseMain);
				});
				odThreadAffect.AddExceptionHandler(e => ex=e);
				odThreadAffect.Name=MethodBase.GetCurrentMethod().Name+"_ThreadAffected";
				odThreadAffect.Start();
				odThreadAffect.Join(Timeout.Infinite);
				//The main thread should still be on the original database after this.
				VerifyCurrentDatabaseName(databaseMain);
				//Verify that new thread is not affected by the call to SetDb() from the child thread above.
				//You should only SetDb() once per application instance. 
				//After that, it does nothing to any thread (existing or subsequent) but the thread that calls it.
				ODThread odThreadChangeDb1= new ODThread(workerChild => {
					VerifyCurrentDatabaseName(databaseMain);
				});
				odThreadChangeDb1.AddExceptionHandler(e => ex=e);
				odThreadChangeDb1.Name=MethodBase.GetCurrentMethod().Name+"_Child";
				odThreadChangeDb1.GroupName=odThreadGroupName;
				odThreadChangeDb1.Start();
				odThreadChangeDb1.Join(Timeout.Infinite);//Manually join the parent to the main thread so that we know for certain that our child thread exists.				
			}
			catch(Exception e) {
				ex=e;
			}
			finally {
				DropDatabase(databaseName);
			}
			Assert.IsNull(ex,ex?.Message);//Asserts do not bubble up as expected to the main thread so do a manual assert at the end of the day.
		}

		private void VerifyCurrentDatabaseName(string expected) {
			Assert.AreEqual(expected,LargeTableHelper.GetCurrentDatabase());
		}

		[TestMethod]
		///<summary>Tests that RunParallel itself is a synchronous method call, meaning that any subsequent code will not execute until either the timeout
		///is reached or all actions are completed.</summary>
		public void ODThread_RunParallel_IsSynchronous() {
			int timeoutMS=1;
			DateTime start=DateTime.Now;
			object testObject=null;
			bool isRunParallelFinished=false;
			List<Action> listActions=new List<Action>();
			listActions.Add(() => {
				while(!isRunParallelFinished) {
					//Wait here until ODThread.RunParallel finishes.
				}
				testObject=new object();//This line should never execute.
			});
			int numThreads=4;//Mimic what is used in AccountModules.GetAll(...).
			ODThread.RunParallel(listActions,timeoutMS,numThreads);
			isRunParallelFinished=true;
			Assert.IsNull(testObject);//Thread did not finish naturally; was aborted due to timeout.
			isRunParallelFinished=false;
			ODThread.RunParallel(listActions,timeoutMS,numThreads,isLegacy:false);
			isRunParallelFinished=true;
			Assert.IsNull(testObject);//Thread did not finish naturally; was aborted due to timeout.
		}

		[TestMethod]
		///<summary>ODThread should be able to catch all exceptions after abort is called on it. If this test fails, an exception is being thrown and 
		///not being caught from within ODThread.</summary>
		public void ODThread_CatchThreadAbortException() {
			bool result=true;
			ODThread thread=new ODThread(o => {
				try {
					Thread.Sleep(TimeSpan.FromMinutes(10));
				}
				catch(Exception ex) {
					//The thread abort exception will come in here. Because of some code in this exception, a different type of exception is thrown.
					ex.DoNothing();
					throw new Exception("Exception that is not a ThreadAbortException");
				}
			});
			thread.AddExceptionHandler(ex => {
				//If the exception handler is every reached, the exception caused by aborting was not caught correctly.
				result=false;
			});
			thread.Start();
			Thread.Sleep(TimeSpan.FromSeconds(1));//Give the thread a chance to start.
			thread.Join(0);//Will cause abort to occur instantly.
			thread.Join(Timeout.Infinite);//Now actually join and wait for the thread to finish.
			Assert.IsTrue(result);
		}

		[TestMethod]
		///<summary>An exception should not cause an ODThread set to autocleanup to fail to clean itself up from the list of running ODThreads.</summary>
		public void ODThread_AutoCleanup_OnThreadAbortException() {
			//Arrange
			string method=MethodBase.GetCurrentMethod().Name;
			ODThread.RegisterForUnhandledExceptions(null,(ex,str) => { });//Stops exception from crashing the test.
			ODThread threadAutoCleanup=new ODThread((o) => {
				throw new Exception();
			});
			threadAutoCleanup.GroupName=method;
			threadAutoCleanup.Name="AutoCleanup";
			ODThread threadDirty=new ODThread((o) => {
				throw new Exception();
			});
			threadDirty.GroupName=method;
			threadDirty.Name="Dirty";
			//Act
			threadAutoCleanup.Start(isAutoCleanup:true);
			threadDirty.Start(isAutoCleanup:false);
			threadAutoCleanup.Join(5000);
			threadDirty.Join(5000);
			List<ODThread> listThreads=ODThread.GetThreadsByGroupName(method);
			//Assert
			Assert.AreEqual(1,listThreads.Count);
			Assert.AreEqual("Dirty",listThreads.First().Name);
			//Cleanup
			ODThread.RegisterForUnhandledExceptions(null,null);//Stops exception from crashing the test.
		}

		///<summary>Shows that all Actions passed to ODThread.RunParallel complete.</summary>
		[TestMethod]
		public void ODThread_RunParallel_AllActionsComplete() {
			ODThread_RunParallel_AllActionsComplete(isLegacy:true);			
			ODThread_RunParallel_AllActionsComplete(isLegacy:false);
		}

		///<summary>Shows that all Actions passed to ODThread.RunParallel complete.</summary>
		private void ODThread_RunParallel_AllActionsComplete(bool isLegacy) {
			//Arrange
			int numThreads=8;
			int countActions=1024;
			List<ParallelAction> listActions=new List<ParallelAction>();
			listActions=Enumerable.Range(0,countActions).Select(x => new ParallelAction(0)).ToList();
			//Act
			ODThread.RunParallel(listActions.Select(x => x.Action).ToList(),Timeout.InfiniteTimeSpan,numThreads,isLegacy:isLegacy);
			//Assert
			Assert.IsTrue(listActions.All(x => x.DidRun));
		}

		///<summary>Shows that all Actions passed to ODThread.RunParallel complete.</summary>
		[TestMethod]
		public void ODThread_RunParallel_IsParallel() {
			ODThread_RunParallel_IsParallel(isLegacy:true);			
			ODThread_RunParallel_IsParallel(isLegacy:false);
		}

		///<summary>Shows that all Actions passed to ODThread.RunParallel complete.</summary>
		private void ODThread_RunParallel_IsParallel(bool isLegacy) {
			//Arrange
			int numThreads=8;
			int countActions=8;
			List<ParallelAction> listActions=new List<ParallelAction>();
			object _lock=new object();
			bool areAllRunning=false;
			listActions=Enumerable.Range(0,countActions).Select(x => new ParallelAction(1
				,sleepWhile:() => {
					lock(_lock) {
						//Keep each action running until all actions have been started.  Then, all actions will release, allowing the threads to complete.
						areAllRunning=areAllRunning ? true : listActions.All(y => y.IsRunning);
						return !areAllRunning;
					}
				})
			).ToList();
			//Act
			ODThread.RunParallel(listActions.Select(x => x.Action).ToList(),50,numThreads,isLegacy:isLegacy);
			//Assert
			Assert.IsTrue(listActions.All(x => x.DidRun));//Will fail if timeout.
		}

		[TestMethod]
		public void ODThread_RunParallel_ImprovedRandomActionRuntimes() {
			//Arrange
			int numThreads=8;
			int countActions=256;
			long runTimeLegacy,runTimeImproved;
			bool didAllRunLegacy,didAllRunImproved;
			Random rnd=new Random();
			for(int i=0;i<5;i++) {
				List<ParallelAction> listActions=new List<ParallelAction>();
				listActions=Enumerable.Range(0,countActions).Select(x => new ParallelAction(rnd.Next(1,countActions))).ToList();
				Stopwatch sw=new Stopwatch();
				//Act
				sw.Start();
				ODThread.RunParallel(listActions.Select(x => x.Action).ToList(),Timeout.InfiniteTimeSpan,numThreads,isLegacy:true);//Only uses 7 threads
				sw.Stop();
				runTimeLegacy=sw.ElapsedMilliseconds;
				didAllRunLegacy=listActions.All(x => x.DidRun);
				listActions.ForEach(x => x.DidRun=false);
				sw.Reset();
				sw.Start();
				ODThread.RunParallel(listActions.Select(x => x.Action).ToList(),Timeout.InfiniteTimeSpan,numThreads,isLegacy:false);//Uses all 8 threads, with better load balancing
				sw.Stop();
				runTimeImproved=sw.ElapsedMilliseconds;
				didAllRunImproved=listActions.All(x => x.DidRun);
				sw.Stop();
				_log.WriteLine($"Legacy(ms): {runTimeLegacy},		Improved(ms): {runTimeImproved}",LogLevel.Information);
				//Assert
				//Assert.IsTrue(runTimeImproved<runTimeLegacy);
				Assert.IsTrue(didAllRunLegacy);
				Assert.IsTrue(didAllRunImproved);
			}
		}

		[TestMethod]
		public void ODThread_RunParallel_ImprovedUnequalActionRuntimes() {
			//Arrange
			int numThreads=8;
			int countActions=256;
			long runTimeLegacy,runTimeImproved;
			bool didAllRunLegacy,didAllRunImproved;
			for(int i=0;i<5;i++) {
				List<ParallelAction> listActions=new List<ParallelAction>();
				//Stack the deck against the Legacy algorithm.  The first thread will get the longest running actions.
				listActions=Enumerable.Range(0,countActions).Select(x => new ParallelAction(countActions-x+1)).ToList();
				Stopwatch sw=new Stopwatch();
				//Act
				sw.Start();
				ODThread.RunParallel(listActions.Select(x => x.Action).ToList(),Timeout.InfiniteTimeSpan,numThreads,isLegacy:true);//Only uses 7 threads
				sw.Stop();
				runTimeLegacy=sw.ElapsedMilliseconds;
				didAllRunLegacy=listActions.All(x => x.DidRun);
				listActions.ForEach(x => x.DidRun=false);
				sw.Reset();
				sw.Start();
				ODThread.RunParallel(listActions.Select(x => x.Action).ToList(),Timeout.InfiniteTimeSpan,numThreads,isLegacy:false);//Uses all 8 threads, with better load balancing
				sw.Stop();
				runTimeImproved=sw.ElapsedMilliseconds;
				didAllRunImproved=listActions.All(x => x.DidRun);
				sw.Stop();
				_log.WriteLine($"Legacy(ms): {runTimeLegacy},		Improved(ms): {runTimeImproved}",LogLevel.Information);
				//Assert
				//Assert.IsTrue(runTimeImproved<runTimeLegacy);
				Assert.IsTrue(didAllRunLegacy);
				Assert.IsTrue(didAllRunImproved);
			}
		}

		private class ParallelAction {
			public bool IsRunning;
			public bool DidRun;
			private int _sleepMs;
			private Func<bool> _sleepWhile = () => false;
			private object _lock=new object();

			public ParallelAction(int sleepMs,Func<bool> sleepWhile=null) {
				_sleepMs=sleepMs;
				_sleepWhile=sleepWhile??_sleepWhile;
			}

			public Action Action => () => {
				lock(_lock) {
					IsRunning=true;
					do {
						if(DidRun) {
							throw new Exception("Action was already run.");
						}
						Thread.Sleep(_sleepMs);
					}while(_sleepWhile());
					DidRun=true;
					IsRunning=false;
				}
			};
		}

	}
}
