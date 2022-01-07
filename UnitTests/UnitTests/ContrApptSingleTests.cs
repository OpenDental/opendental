using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDental;

namespace UnitTests.ContrApptSingle_Tests {
	///<summary>This test was created to test that we don't get an "Object currently in use elsewhere" exception.
	///GDI+ isn't thread safe, so when two threads try to access the image data an exception is thrown. 
	///This test creates 20 threads who all try and access the image 500 times each.</summary>
	[TestClass]
	public class ContrApptSingleTests:TestBase {
		/*[TestMethod]
		public void ContrApptSingle_ShadowGet_Threads() {
			ContrApptSingle cas=new ContrApptSingle();
			//Spawn 20 threads to all try and access it.
			cas.Shadow=new Bitmap(100,100);//Fake shadown.  Good size that trips the error 1000x1000 uses too much memory (because bitmaps)
			bool success=true;//Accessed by all threads, set to false if any of the threads fails.
			List<Action> listThreads=new List<Action>();
			//Creates 20 threads, which access the bitmap 100 times each.  
			//This way we have a good chance of two threads accessing the image at the same time.
			Action action=new Action(() => {
				for(int i=0;i<500;i++) {
					if(!success) { return; }//Don't do work we don't need to.
					try {
						Bitmap test=cas.Shadow;//The clone shares image data with the original.
						//This will attempt to access the test bitmap
						//If it is not thread safe, then it will throw an exception.
						test.GetThumbnailImage(64,64,null,IntPtr.Zero);
						test.Dispose();
						test=null;
					}
					catch(InvalidOperationException e) {//Catching "System.InvalidOperationException: The object is currently in use elsewhere."
						e.DoNothing();//Swallow the actual exception because this is a thread.
						success=false;//Communicate failure to the outside world.
						return;
					}
				}
			});
			for(int i=0;i<20;i++) {
				listThreads.Add(action);
			}
			ODThread.RunParallel(listThreads,Timeout.InfiniteTimeSpan,listThreads.Count);
			Assert.IsTrue(success);//The assertion will pass if none of threads hit an access violation.  Throws an exception if success=false.
		}

		///<summary>This shows how the ContrAptSingle.Shadow bitmap is thread safe.
		///ContrApptSingle_Shadow_ThreadSafetyFailure shows how this test would fail with a regular bitmap object.
		///This method will create 20 threads that all try and make changes on a bitmap.  The ContrApptSingle class properly
		///manages the threads so we don't get a GDI+ exception.</summary>
		[TestMethod]
		public void ContrApptSingle_Shadow_ThreadSafety() {
			List<Action> listThreads=new List<Action>();
			ContrApptSingle cas=new ContrApptSingle();
			cas.Shadow=new Bitmap(100,100);//Fake shadow.
			//This will be set to true if ANY thread fails.
			bool success=true;
			Action action=new Action(() => {
				for(int i=0;i<100;i++) {
					try {
						//This block of code originally caused issues with the shadow.  Mimics ContrApptSingle.CreateShadow().
						using(Graphics g=Graphics.FromImage(cas.Shadow)) {//This is where GDI+ exception would occur if we did not create deep copy.
							string pattern="//////////////";//Mimic ApptSingleDrawings.DrawEntireAppt() by drawing fake lines on the fake shadow.
							for(int j=0;j<pattern.Length;j++) {
								g.DrawLine(Pens.Silver,1,j*12,6,j*12);
							}
						}
					}
					catch {
						success=false;
						return;
					}
				}
			});
			for(int i=0;i<20;i++) {
				listThreads.Add(action);
			}
			ODThread.RunParallel(listThreads,Timeout.InfiniteTimeSpan,listThreads.Count);
			Assert.IsTrue(success);//The assertion will pass if none of threads hit an access violation.  Throws an exception if success=false.
		}

		///<summary>This is a counter-example to ContrApptSingle_Shadow_ThreadSafety.
		///This method will create 20 threads that all try and make changes on a bitmap.</summary>
		[TestMethod]
		public void ContrApptSingle_Shadow_ThreadSafetyFailure() {
			List<Action> listThreads=new List<Action>();
			bool success=true;
			Bitmap fakeShadow=new Bitmap(100,100);
			Action action=new Action(() => {
				for(int i=0;i<500;i++) {
					try {
						//This block of code originally caused issues with the shadow.  Mimics ContrApptSingle.CreateShadow().
						using(Graphics g=Graphics.FromImage(fakeShadow)) {//This line should cause exceptions.
							string pattern="//////////////";//Mimic ApptSingleDrawings.DrawEntireAppt() by drawing fake lines on the fake shadow.
							for(int j=0;j<pattern.Length;j++) {
								g.DrawLine(Pens.Silver,1,j*12,6,j*12);
							}
						}
					}
					catch {
						//This should be hit at least once.
						success=false;
						return;
					}
				}
			});
			for(int i=0;i<20;i++) {
				listThreads.Add(action);
			}
			ODThread.RunParallel(listThreads,Timeout.InfiniteTimeSpan,listThreads.Count);
			Assert.IsFalse(success);//The test should always fail.
		}*/
	}
}
