using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests {
	[TestClass]
	public class ODPrimitiveExtensionsTests:TestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		///<summary>Deep copy of a primitive</summary>
		[TestMethod]
		public void ODPrimitiveExtensions_DeepCopy_Primitive() {
			int orig=5;
			int copy=GenericTools.DeepCopy<int,int>(orig);
			orig=6;
			Assert.AreEqual(5,copy);
		}

		///<summary>Deep copy of a simple class</summary>
		[TestMethod]
		public void ODPrimitiveExtensions_DeepCopy_SimpleClass() {
			TestClass orig=new TestClass { Day=DateTime.Today,Name="me" };
			TestClass copy=GenericTools.DeepCopy<TestClass,TestClass>(orig);
			orig.Day=DateTime.MinValue;
			orig.Name="you";
			Assert.AreEqual("me",copy.Name);
			Assert.AreEqual(DateTime.Today,copy.Day);
		}

		///<summary>Deep copy of a simple class, showing private fields are copied as well</summary>
		[TestMethod]
		public void ODPrimitiveExtensions_DeepCopy_PrivateField() {
			TestClass orig=new TestClass(field:1);
			TestClass copy=GenericTools.DeepCopy<TestClass,TestClass>(orig);
			orig.SetField(2);
			Assert.AreEqual(2,orig.GetField());
			Assert.AreEqual(1,copy.GetField());
		}

		///<summary>Deep copy of a simple class, showing private properties are copied as well</summary>
		[TestMethod]
		public void ODPrimitiveExtensions_DeepCopy_PrivateProperty() {
			TestClass orig=new TestClass(charProperty:'Z');
			TestClass copy=GenericTools.DeepCopy<TestClass,TestClass>(orig);
			orig.SetProperty('A');
			Assert.AreEqual('A',orig.CharProperty());
			Assert.AreEqual('Z',copy.CharProperty());
		}

		///<summary>Some table types and helper classes have complex fields that require special attention within the Copy method.
		///The DeepCopy extension method should make a deep copy of those complex fields as well.</summary>
		[TestMethod]
		public void ODPrimitiveExtensions_DeepCopy_ComplexClass() {
			string expectedNameOrig="FieldNameOrig";
			string expectedNameDeepCopy="FieldNameDeepCopy";
			TestComplexClass orig=new TestComplexClass() {
				Parameters=new List<TestParameter>(),
				List=new List<TestClass>() { 
					new TestClass(){ Name=expectedNameOrig }
				},
			};
			TestComplexClass deepCopy=GenericTools.DeepCopy<TestComplexClass,TestComplexClass>(orig);
			deepCopy.List.First().Name=expectedNameDeepCopy;
			Assert.AreNotEqual(deepCopy.List.First().Name,orig.List.First().Name);
			Assert.AreEqual(expectedNameOrig,orig.List.First().Name);
			Assert.AreEqual(expectedNameDeepCopy,deepCopy.List.First().Name);
		}

		///<summary>Deep copy of a simple class into a derived class</summary>
		[TestMethod]
		public void ODPrimitiveExtensions_DeepCopy_DerivedClass() {
			TestClass orig=new TestClass { Day=DateTime.Today,Name="me" };
			TestDerivedClass copy=GenericTools.DeepCopy<TestClass,TestDerivedClass>(orig);
			orig.Day=DateTime.MinValue;
			orig.Name="you";
			Assert.AreEqual("me",copy.Name);
			Assert.AreEqual(DateTime.Today,copy.Day);
			Assert.AreEqual(DateTime.Today.ToString(),copy.DayAsString);
			Assert.AreEqual(0,copy.DerivedProp);//Properties added in a derived class should have default values.
			Assert.AreEqual(0,copy.DerivedField);//Fields added in a derived class should have default values.
		}

		///<summary>Deep copy of a List of a base class into a List of a derived class</summary>
		[TestMethod]
		public void ODPrimitiveExtensions_DeepCopyList_ListDerivedClass() {
			List<TestClass> listOrig=new List<TestClass> {
				new TestClass { Day=DateTime.Today,Name="me" },
			};
			List<TestDerivedClass> listCopy=ListTools.DeepCopy<TestClass,TestDerivedClass>(listOrig);
			listOrig[0].Day=DateTime.MinValue;
			listOrig[0].Name="you";
			Assert.AreEqual(listOrig.Count,listCopy.Count);
			Assert.AreEqual("me",listCopy[0].Name);
			Assert.AreEqual(DateTime.Today,listCopy[0].Day);
			Assert.AreEqual(DateTime.Today.ToString(),listCopy[0].DayAsString);
		}

		//A limitation to the DeepCopy method is the inability to create deep copies of the list that is wrapped by a ReadOnlyCollection.  Modifying the
		//wrapped list will result in a DeepCopied ReadOnlyCollection to also be modified (effectively a shallow copy).
		/////<summary>Deep copy of a ReadOnlyCollection.  The source List is altered after the copy is made.</summary>
		//[TestMethod]
		//public void ODPrimitiveExtensions_DeepCopy_ReadOnlyCollection() {
		//	List<string> list=new List<string> { "yes", "no" };
		//	ReadOnlyCollection<string> orig=new ReadOnlyCollection<string>(list);
		//	ReadOnlyCollection<string> copy=orig.DeepCopy<ReadOnlyCollection<string>,ReadOnlyCollection<string>>();
		//	list[0]="maybe";
		//	list[1]="never";
		//	Assert.AreEqual("yes",copy[0]);
		//	Assert.AreEqual("no",copy[1]);
		//}

		///<summary>Deep copy an IDictionary</summary>
		[TestMethod]
		public void ODPrimitiveExtensions_DeepCopy_IDictionary() {
			DateTime today=DateTime.Today;
			DateTime yesterday=DateTime.Today.AddDays(-1);
			IDictionary orig=new Dictionary<string,IEnumerable<TestClass>>();
			orig["today"]=new List<TestClass> { new TestClass(today) };
			orig["yesterday"]=new TestClass[] { new TestClass(yesterday) };
			IDictionary copy=GenericTools.DeepCopy<IDictionary,Dictionary<string,IEnumerable<TestClass>>>(orig);
			((Dictionary<string,IEnumerable<TestClass>>)orig)["today"].ElementAt(0).Day=DateTime.MinValue;
			((Dictionary<string,IEnumerable<TestClass>>)orig)["yesterday"].ElementAt(0).Day=DateTime.MaxValue;
			Assert.AreEqual(today,((Dictionary<string,IEnumerable<TestClass>>)copy)["today"].ElementAt(0).Day);
			Assert.AreEqual(yesterday,((Dictionary<string,IEnumerable<TestClass>>)copy)["yesterday"].ElementAt(0).Day);
		}

		///<summary>Deep copy a KeyValuePair</summary>
		[TestMethod]
		public void ODPrimitiveExtensions_DeepCopy_KeyValuePair() {
			DateTime today=DateTime.Today;
			DateTime yesterday=DateTime.Today.AddDays(-1);
			KeyValuePair<string,IEnumerable<TestClass>> orig=new KeyValuePair<string,IEnumerable<TestClass>>(
				"today",new List<TestClass> { new TestClass(today) });
			KeyValuePair<string,IEnumerable<TestClass>> copy=
				GenericTools.DeepCopy<KeyValuePair<string,IEnumerable<TestClass>>,KeyValuePair<string,IEnumerable<TestClass>>>(orig);
			orig.Value.ElementAt(0).Day=yesterday;
			Assert.AreEqual("today",copy.Key);
			Assert.AreEqual(today,copy.Value.ElementAt(0).Day);
		}

		///<summary>Deep copy a Tuple</summary>
		[TestMethod]
		public void ODPrimitiveExtensions_DeepCopy_Tuple() {
			DateTime today=DateTime.Today;
			DateTime yesterday=DateTime.Today.AddDays(-1);
			Tuple<string,IEnumerable<TestClass>,TestClass> orig=Tuple.Create<string,IEnumerable<TestClass>,TestClass>("today",new List<TestClass> { new TestClass(today) },new TestClass('A'));
			Tuple<string,IEnumerable<TestClass>,TestClass> copy=GenericTools.DeepCopy<Tuple<string,IEnumerable<TestClass>,TestClass>,Tuple<string,IEnumerable<TestClass>,TestClass>>(orig);
			orig.Item2.ElementAt(0).Day=yesterday;
			orig.Item3.SetProperty('B');
			Assert.AreEqual("today",copy.Item1);
			Assert.AreEqual(today,copy.Item2.ElementAt(0).Day);
			Assert.AreEqual('A',copy.Item3.CharProperty());
		}

		///<summary>Deep copy a struct</summary>
		[TestMethod]
		public void ODPrimitiveExtensions_DeepCopy_Struct() {
			DateTime today=DateTime.Today;
			DateTime yesterday=DateTime.Today.AddDays(-1);
			TestStruct orig=new TestStruct { ObjA=new TestClass(today),ObjB=new TestClass(yesterday) };
			TestStruct copy=GenericTools.DeepCopy<TestStruct,TestStruct>(orig);
			orig.ObjA.Day=DateTime.MinValue;
			orig.ObjB.Day=DateTime.MaxValue;
			Assert.AreEqual(today,copy.ObjA.Day);
			Assert.AreEqual(yesterday,copy.ObjB.Day);
		}

		private class TestClass {
			public DateTime Day;
			public string Name;
			private int _field;
			private char _charProperty { get; set; }
			public TestClass() { }
			public TestClass(DateTime day) {
				Day=day;
			}
			public TestClass(int field) {
				_field=field;
			}

			public TestClass(char charProperty) {
				_charProperty=charProperty;
			}

			public int GetField() {
				return _field;
			}

			public void SetField(int val) {
				_field=val;
			}

			public void SetProperty(char val) {
				_charProperty=val;
			}

			public char CharProperty() {
				return _charProperty;
			}
		}

		private class TestDerivedClass : TestClass{
			public string DayAsString => Day.ToString();
			public int DerivedProp { get; set; }
			public int DerivedField;
		}

		private struct TestStruct {
			public TestClass ObjA;
			public TestClass ObjB;
		}

		private class TestComplexClass {
			public List<TestParameter> Parameters;
			public List<TestClass> List;
		}

		private class TestParameter {
			public string Param;
		}
	}
}
