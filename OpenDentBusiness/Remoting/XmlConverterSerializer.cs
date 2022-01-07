using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using CodeBase;

namespace OpenDentBusiness {

	///<summary>Code here not included in XmlConverter to avoid references in that file that we do not want for the OpenDentalEmail project.</summary>
	public class XmlConverterSerializer {

		///<summary>Should accept any type, including simple types, OD types, Arrays, Lists, and arrays of DtoObject.  But not DataTable or DataSet.  If we find a type that isn't supported, then we need to add it.</summary>
		public static string Serialize<T>(T obj) {
			StringBuilder strBuild=new StringBuilder();
			XmlWriter writer;
			if(ODBuild.IsDebug()) {
				XmlWriterSettings settings=new XmlWriterSettings();
				settings.Indent=true;
				settings.IndentChars="   ";
				//using the constructor decreases performance and leads to memory leaks.
				//But it makes the xml much more readable
				writer=XmlWriter.Create(strBuild,settings);
			}
			else {
				writer=XmlWriter.Create(strBuild);
			}
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			serializer.Serialize(writer,obj);
			writer.Close();
			return strBuild.ToString();
		}

		///<summary>For late binding of class type.</summary>
		public static string Serialize(Type classType,object obj) {
			StringBuilder strBuild=new StringBuilder();
			XmlWriter writer;
			if(ODBuild.IsDebug()) {
				XmlWriterSettings settings=new XmlWriterSettings();
				settings.Indent=true;
				settings.IndentChars="   ";
				//settings.NewLineHandling=NewLineHandling.None;//an attempt to not remove \r in strings.  Failed.
				//using the constructor decreases performance and leads to memory leaks.
				//But it makes the xml much more readable
				writer=XmlWriter.Create(strBuild,settings);
			}
			else {
				writer=XmlWriter.Create(strBuild);
			}
			XmlSerializer serializer;
			if(classType==typeof(Color)) {
				serializer = new XmlSerializer(typeof(int));
				serializer.Serialize(writer,((Color)obj).ToArgb());
			}
			else if(classType==typeof(TimeSpan)) {
				serializer = new XmlSerializer(typeof(long));
				serializer.Serialize(writer,((TimeSpan)obj).Ticks);
			}
			else {
				if(obj!=null) {
					obj=XmlConverter.XmlEscapeRecursion(classType,obj);//search object for string fields to escape
				}
				serializer = new XmlSerializer(classType);
				serializer.Serialize(writer,obj);
			}
			writer.Close();
			return strBuild.ToString();
			//the result will be fully qualified xml, including declaration.  Example:
			/*
			{<?xml version="1.0" encoding="utf-16"?>
<Userod xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
   <IsNew>false</IsNew>
   <UserNum>1</UserNum>
   <UserName>Admin</UserName>
   <Password />
   <UserGroupNum>1</UserGroupNum>
   <EmployeeNum>0</EmployeeNum>
   <ClinicNum>0</ClinicNum>
   <ProvNum>0</ProvNum>
   <IsHidden>false</IsHidden>
   <TaskListInBox>0</TaskListInBox>
   <AnesthProvType>3</AnesthProvType>
   <DefaultHidePopups>false</DefaultHidePopups>
   <PasswordIsStrong>false</PasswordIsStrong>
</Userod>}*/
		}

		///<summary>Should accept any type.  Tested types include System types, OD types, Arrays, Lists, arrays of DtoObject, null DataObjectBase, null arrays, null Lists.  But not DataTable or DataSet.  If we find a type that isn't supported, then we need to add it.  Types that are currently unsupported include Arrays of DataObjectBase that contain a null.  Lists that contain nulls are untested and may be an issue for DataObjectBase.</summary>
		public static T Deserialize<T>(string xmlData) {
			Type type = typeof(T);
			/*later.  I don't think arrays will null objects will be an issue.
			if(type.IsArray) {
				Type arrayType=type.GetElementType();
				if(arrayType.BaseType==typeof(DataObjectBase)) {
					//split into items
				}
			}*/
			if(type.IsGenericType) {//List<>
				//because the built-in deserializer does not handle null list<>, but instead returns an empty list.
				//<ArrayOfDocument xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xsi:nil="true" />
				if(Regex.IsMatch(xmlData,"<ArrayOf[^>]*xsi:nil=\"true\"")) {
					return default(T);//null
				}
			}
			StringReader strReader=new StringReader(xmlData);
			//XmlReader reader=XmlReader.Create(strReader);
			XmlTextReader reader=new XmlTextReader(strReader);
			XmlSerializer serializer;
			T retVal;
			if(type==typeof(Color)) {
				serializer = new XmlSerializer(typeof(int));
				retVal=(T)((object)Color.FromArgb((int)serializer.Deserialize(reader)));
			}
			else if(type==typeof(TimeSpan)) {
				serializer = new XmlSerializer(typeof(long));
				retVal=(T)((object)TimeSpan.FromTicks((long)serializer.Deserialize(reader)));
			}
			else if(type.IsInterface) {
				//For methods that return an interface, we serialize the return object as a DtoObject.
				serializer=new XmlSerializer(typeof(DtoObject));
				retVal=(T)((DtoObject)serializer.Deserialize(reader)).Obj;
			}
			else {
				serializer = new XmlSerializer(type);
				retVal=(T)serializer.Deserialize(reader);
				if(retVal!=null) {
					retVal=(T)XmlConverter.XmlUnescapeRecursion(type,retVal);
				}
			}
			strReader.Close();
			reader.Close();
			return retVal;
		}

	}
}
