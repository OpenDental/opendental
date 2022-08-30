using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using OpenDental.DataAccess;
using System.Collections.ObjectModel;
using OpenDental;
using System.ServiceProcess;
using System.Threading;
using System.Reflection;

namespace OpenDentBusiness.Tests {
	[TestFixture()]
	public abstract class DataTests<T>
			where T : DataObjectBase, new() {
		private const int DatabaseMaxInteger = short.MaxValue;
		// For types like double and float, saving the max value will not work:
		// http://bugs.mysql.com/bug.php?id=9262
		// That's why we use a haircut, to save a large (but not too large) value to the
		// database. Right now we go up to 5 nine's.
		private const double LargeValuesHaircut = 0.99999;

		Random random;
		AppDomain domain;
		ServiceWrapper wrapper;

		[SetUp()]
		public void Intialize() {
			random = new Random();
			StartService();
		}

		[TearDown()]
		public void CleanUp() {
			StopService();
		}

		private void StartService() {
			// Start the OD service in a new app domain
			domain = AppDomain.CreateDomain("ServiceDomain");
			wrapper = (ServiceWrapper)domain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().CodeBase, typeof(ServiceWrapper).FullName);
			wrapper.Start();
		}

		private void StopService() {
			wrapper.Stop();
			AppDomain.Unload(domain);
		}

		[Test()]
		public void Test() {
			RemotingClient.OpenDentBusinessIsLocal = true;
			// Connect to the local mysql server.
			DataConnection connection = new DataConnection();
			connection.SetDb("localhost", "opendental", "root", "", "root", "", DatabaseType.MySql);
			DataObjectFactory<T>.UseParameters = false;
			TestTableType();
			DataObjectFactory<T>.UseParameters = true;
			TestTableType();

			RemotingClient.OpenDentBusinessIsLocal = false;
			RemotingClient.ServerName = "localhost";
			RemotingClient.ServerPort = 9390;

			DataObjectFactory<T>.UseParameters = false;
			TestTableType();
			DataObjectFactory<T>.UseParameters = true;
			TestTableType();

			RemotingClient.Disconnect();
		}

		private void TestTableType() {

			// Create a new object
			T t = DataObjectFactory<T>.NewObject();
			Assert.IsNotNull(t, "#1 (Object should not be null)");
			Assert.IsTrue(t.IsNew, "#5 (IsNew should be set)");
			Assert.IsFalse(t.IsDeleted, "#6 (IsDeleted should be false)");
			Assert.IsTrue(t.IsDirty, "#7 (IsDirty should be true)");

			// Fill the data
			Collection<DataFieldInfo> dataFields = DataObjectInfo<T>.GetDataFields(DataFieldMask.Data);
			foreach(DataFieldInfo dataField in dataFields) {
				DataObjectInfo<T>.SetProperty(dataField, t, Random(dataField.Field.FieldType));
			}

			// Save the object to the database
			DataObjectFactory<T>.WriteObject(t);

			if(!DataObjectInfo<T>.HasPrimaryKeyWithAutoNumber())
				throw new NotSupportedException();

			int id = DataObjectInfo<T>.GetPrimaryKey(t);
			Assert.AreNotEqual(0, id, "#2 (ID should not be zero)");
			Assert.IsFalse(t.IsDirty, "#8 (IsDirty should be false)");
			Assert.IsFalse(t.IsDeleted, "#9 (IsDeleted should be false)");
			Assert.IsFalse(t.IsNew, "#16 (IsNew should be false)");

			// Load a new object
			T database = DataObjectFactory<T>.CreateObject(id);
			foreach(DataFieldInfo dataField in dataFields) {
				object localValue = dataField.Field.GetValue(t);
				object databaseValue = dataField.Field.GetValue(database);

				// Text strings may be trimmed by the database. For now,
				// ignore that.
				if(dataField.Field.FieldType == typeof(string)) {
					string localText = (string)localValue;
					string databaseText = (string)databaseValue;

					if(databaseText.Length < localText.Length) {
						localValue = localText.Substring(0, databaseText.Length);
					}
				}

				if(dataField.Field.FieldType == typeof(double)) {
					Assert.AreEqual((double)localValue, (double)databaseValue, 0.01, string.Format("#3a - {0} (Value retrieved from database should equal stored value)", FormatFieldName(dataField)));
				}
				else if(dataField.Field.FieldType == typeof(float)) {
					Assert.AreEqual((float)localValue, (float)databaseValue, 0.01, string.Format("#3b - {0} (Value retrieved from database should equal stored value)", FormatFieldName(dataField)));
				}
				else {
					Assert.AreEqual(localValue, databaseValue, string.Format("#3x - {0} (Value retrieved from database should equal stored value)", FormatFieldName(dataField)));
				}
			}

			// Modify the object
			foreach(DataFieldInfo dataField in dataFields) {
				DataObjectInfo<T>.SetProperty(dataField, t, Random(dataField.Field.FieldType));
				Assert.IsTrue(DataObjectInfo<T>.HasChanged(dataField, t), string.Format("#11 - {0} (Field should be marked as dirty)", FormatFieldName(dataField)));
			}

			Assert.IsTrue(t.IsDirty, "#12 (Object should be dirty)");

			DataObjectFactory<T>.WriteObject(t);
			Assert.AreEqual(id, DataObjectInfo<T>.GetPrimaryKey(t), "#13 (Id should not change after saving)");
			Assert.IsFalse(t.IsNew, "#14 (IsDirty should not be set");
			Assert.IsFalse(t.IsNew, "#15 (IsNew should not be set");

			database = DataObjectFactory<T>.CreateObject(id);
			foreach(DataFieldInfo dataField in dataFields) {
				object localValue = dataField.Field.GetValue(t);
				object databaseValue = dataField.Field.GetValue(database);

				// Text strings may be trimmed by the database. For now,
				// ignore that.
				if(dataField.Field.FieldType == typeof(string)) {
					string localText = (string)localValue;
					string databaseText = (string)databaseValue;

					if(databaseText.Length < localText.Length) {
						localValue = localText.Substring(0, databaseText.Length);
					}
				}

				if(dataField.Field.FieldType == typeof(double)) {
					Assert.AreEqual((double)localValue, (double)databaseValue, 0.01, string.Format("#4a - {0}", FormatFieldName(dataField)));
				}
				else if(dataField.Field.FieldType == typeof(float)) {
					Assert.AreEqual((float)localValue, (float)databaseValue, 0.01, string.Format("#4b - {0}", FormatFieldName(dataField)));
				}
				else {
					Assert.AreEqual(localValue, databaseValue, string.Format("#4x - {0}", FormatFieldName(dataField)));
				}
			}

			// For all fields of value types, try to store the maximum and minimum value, to make sure
			// the type in the database can hold the values given by the program.
			TestExtremum(t, id, ExtremumType.Maximum);
			TestExtremum(t, id, ExtremumType.Minimum);

			// Delete the object
			DataObjectFactory<T>.DeleteObject(t);
		}

		private void TestExtremum(T value, int id, ExtremumType extremumType) {
			Collection<DataFieldInfo> dataFields = DataObjectInfo<T>.GetDataFields(DataFieldMask.Data);

			// Set the value
			foreach(DataFieldInfo field in dataFields) {
				Type fieldType = field.Field.FieldType;

				if(fieldType.IsValueType && fieldType.IsPrimitive) {
					DataObjectInfo<T>.SetProperty(field, value, Extremum(fieldType, extremumType));
				}
			}

			// Save the object
			DataObjectFactory<T>.WriteObject(value);
			value = DataObjectFactory<T>.CreateObject(id);

			// Verify the value
			foreach(DataFieldInfo field in dataFields) {
				Type fieldType = field.Field.FieldType;

				if (fieldType == typeof(double)) {
					object storedValue = field.Field.GetValue(value);
					Assert.AreEqual((double)Extremum(fieldType, extremumType), (double)storedValue, Math.Abs(1E-5 * (double)storedValue), string.Format("#17 - {0}: Maximum value should store correctly.", FormatFieldName(field)));
				}
				else if (fieldType.IsValueType && fieldType.IsPrimitive) {
					object storedValue = field.Field.GetValue(value);
					Assert.AreEqual(Extremum(fieldType, extremumType), storedValue, string.Format("#17 - {0}: Maximum value should store correctly.", FormatFieldName(field)));
				}
			}
		}

		private string FormatFieldName(DataFieldInfo field) {
			return string.Format("{0}.{1}", typeof(T).Name, field.Field.Name);
		}

		private enum ExtremumType {
			Minimum,
			Maximum
		}

		private object Extremum(Type dataType, ExtremumType extremumType) {
			if(dataType == typeof(bool)) {
				switch(extremumType) {
					case ExtremumType.Maximum:
						return true;
					case ExtremumType.Minimum:
						return false;
				}
			}
			if(dataType == typeof(byte)) {
				switch(extremumType) {
					case ExtremumType.Maximum:
						return byte.MaxValue;
					case ExtremumType.Minimum:
						return byte.MinValue;
				}
			}
			else if(dataType == typeof(short)) {
				switch(extremumType) {
					case ExtremumType.Maximum:
						return short.MaxValue;
					case ExtremumType.Minimum:
						return short.MinValue;
				}
			}
			else if(dataType == typeof(int)) {
				switch(extremumType) {
					case ExtremumType.Maximum:
						return int.MaxValue;
					case ExtremumType.Minimum:
						return int.MinValue;
				}
			}
			else if(dataType == typeof(long)) {
				switch(extremumType) {
					case ExtremumType.Maximum:
						return long.MaxValue;
					case ExtremumType.Minimum:
						return long.MinValue;
				}
			}
			else if(dataType == typeof(float)) {
				switch(extremumType) {
					case ExtremumType.Maximum:
						return float.MaxValue;
					case ExtremumType.Minimum:
						return float.MinValue;
				}
			}
			else if(dataType == typeof(double)) {
				switch(extremumType) {
					case ExtremumType.Maximum:
						return LargeValuesHaircut * double.MaxValue;
					case ExtremumType.Minimum:
						return LargeValuesHaircut * double.MinValue;
				}
			}
			else if(dataType == typeof(decimal)) {
				switch(extremumType) {
					case ExtremumType.Maximum:
						return decimal.MaxValue;
					case ExtremumType.Minimum:
						return decimal.MinValue;
				}
			}

			throw new NotSupportedException();
		}

		private object Random(Type dataType) {
			if(dataType == typeof(string)) {
				string value = string.Empty;
				for(int i = 0; i < 20; i++) {
					value += (char)(random.Next(33, 126));
				}
				return value;
			}
			else if(dataType == typeof(bool)) {
				return Convert.ToBoolean(random.Next(0, 1));
			}
			else if(dataType == typeof(Byte)) {
				byte[] buffer = new byte[1];
				random.NextBytes(buffer);
				return buffer[0];
			}
			else if(dataType == typeof(DateTime)) {
				DateTime value = DateTime.Today;
				value = value.AddDays(random.Next(-1000, 1000));
				return value;
			}
			else if(dataType == typeof(TimeSpan)) {
				return new TimeSpan(0, random.Next(0, 60), 0);
			}
			else if(dataType == typeof(double)) {
				return random.NextDouble();
			}
			else if(dataType == typeof(float)) {
				return (float)random.NextDouble();
			}
			else if(dataType == typeof(int)) {
				return random.Next(DatabaseMaxInteger + 1);
			}
			else if(dataType == typeof(short)) {
				return (short)random.Next(short.MaxValue);
			}
			else if(dataType.IsEnum) {
				Array values = Enum.GetValues(dataType);
				int index = random.Next(0, values.Length);
				return values.GetValue(index);
			}
			else {
				throw new NotSupportedException("The type " + dataType.Name + " is not supported");
			}
		}
	}
}
