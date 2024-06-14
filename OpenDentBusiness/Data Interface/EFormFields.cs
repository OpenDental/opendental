using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Documents;
using Newtonsoft.Json;
using System.Linq;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EFormFields{

		///<summary></summary>
		public static List<EFormField> GetForForm(long eFormNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<EFormField>>(MethodBase.GetCurrentMethod(),eFormNum);
			}
			string command="SELECT * FROM eformfield WHERE EFormNum = "+POut.Long(eFormNum)+" ORDER BY ItemOrder";
			return Crud.EFormFieldCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(EFormField eFormField){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				eFormField.EFormFieldNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eFormField);
				return eFormField.EFormFieldNum;
			}
			return Crud.EFormFieldCrud.Insert(eFormField);
		}

		///<summary></summary>
		public static void DeleteForForm(long eFormNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eFormNum);
				return;
			}
			string command="DELETE FROM eformfield WHERE EFormNum = "+POut.Long(eFormNum);
			Db.NonQ(command);
		}

		public static EFormField FromDef(EFormFieldDef eFormFieldDef,long patNum=0){
			EFormField eFormField=new EFormField();
			eFormField.PatNum=patNum;
			eFormField.FieldType=eFormFieldDef.FieldType;
			eFormField.DbLink=eFormFieldDef.DbLink;
			eFormField.ValueLabel=eFormFieldDef.ValueLabel;
			//eFormField.ValueString //set as part of fill
			eFormField.ItemOrder=eFormFieldDef.ItemOrder;
			eFormField.PickListVis=eFormFieldDef.PickListVis;
			eFormField.PickListDb=eFormFieldDef.PickListDb;
			eFormField.IsHorizStacking=eFormFieldDef.IsHorizStacking;
			eFormField.IsTextWrap=eFormFieldDef.IsTextWrap;
			eFormField.Width=eFormFieldDef.Width;
			eFormField.FontScale=eFormFieldDef.FontScale;
			eFormField.IsRequired=eFormFieldDef.IsRequired;
			eFormField.ConditionalParent=eFormFieldDef.ConditionalParent;
			eFormField.ConditionalValue=eFormFieldDef.ConditionalValue;
			return eFormField;
		}

		public static EFormFieldDef ToDef(EFormField eFormField){
			EFormFieldDef eFormFieldDef=new EFormFieldDef();
			eFormFieldDef.FieldType=eFormField.FieldType;
			eFormFieldDef.DbLink=eFormField.DbLink;
			eFormFieldDef.ValueLabel=eFormField.ValueLabel;
			//eFormField.ValueString //set as part of fill
			eFormFieldDef.ItemOrder=eFormField.ItemOrder;
			eFormFieldDef.PickListVis=eFormField.PickListVis;
			eFormFieldDef.PickListDb=eFormField.PickListDb;
			eFormFieldDef.IsHorizStacking=eFormField.IsHorizStacking;
			eFormFieldDef.IsTextWrap=eFormField.IsTextWrap;
			eFormFieldDef.Width=eFormField.Width;
			eFormFieldDef.FontScale=eFormField.FontScale;
			eFormFieldDef.IsRequired=eFormField.IsRequired;
			eFormFieldDef.ConditionalParent=eFormField.ConditionalParent;
			eFormFieldDef.ConditionalValue=eFormField.ConditionalValue;
			return eFormFieldDef;
		}

		public static List<EFormField> FromListDefs(List<EFormFieldDef> listEFormFieldDefs,long patNum=0){
			List<EFormField> listEFormFields=new List<EFormField>();
			for(int i=0;i<listEFormFieldDefs.Count;i++) {
				EFormField eFormField=FromDef(listEFormFieldDefs[i],patNum);
				listEFormFields.Add(eFormField);
			}
			return listEFormFields;
		}

		public static FlowDocument DeserializeFlowDocument(string xmlString){
			//if(!xmlString.StartsWith("<FlowDocument")){//must always be true, so something is wrong
			//	return new FlowDocument();
			//}
			string xamlString=xmlString;
			xamlString=xamlString.Replace("<FlowDocument>","<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">");
			using StringReader stringReader=new StringReader(xamlString);
			using XmlReader xmlReader=XmlReader.Create(stringReader);
			FlowDocument flowDocument=(FlowDocument)XamlReader.Load(xmlReader);
			return flowDocument;
		}

		public static string SerializeFlowDocument(FlowDocument flowDocument){
			//a few properties are set because it's coming from a richTextBox.
			//They need to be reset to match out pattern further down when we strip them out.
			flowDocument.PagePadding=new Thickness(0);
			flowDocument.AllowDrop=true;//
			MemoryStream memoryStream=new MemoryStream();
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Encoding = Encoding.UTF8;
			xmlWriterSettings.CloseOutput = false;//for xmlWriter.Close(); to not close the stream
			xmlWriterSettings.OmitXmlDeclaration=true;
			xmlWriterSettings.NewLineHandling=NewLineHandling.None;//new lines inside of runs are translated as a space, messing up the text
			using XmlWriter xmlWriter=XmlWriter.Create(memoryStream,xmlWriterSettings);
			XamlWriter.Save(flowDocument,xmlWriter);
			xmlWriter.Close();
			string xamlString = Encoding.UTF8.GetString(memoryStream.ToArray());
			memoryStream.Dispose();
			string pattern = @"<FlowDocument"
				+"[^>]*"//any number of characters that are not >
				+">";
			xamlString=Regex.Replace(xamlString,pattern,"<FlowDocument>");//get rid of all the attributes like xmlns. We will add that back for viewing.
			byte[] byteArray = Encoding.UTF8.GetBytes(xamlString);
			memoryStream = new MemoryStream(byteArray);
			//memoryStream.Position=0;
			using StreamReader streamReader=new StreamReader(memoryStream);
			string retVal=streamReader.ReadToEnd();
			memoryStream.Dispose();
			return retVal;
		}

		///<summary>Converts the ValueString, which stores the PickListDb value, to the PickListVis value. If the FieldType is not a radiobutton, it will return an empty string since this method should only be used with radiobuttons. If the ValueString is not found in PickListDb, it will also return an empty string.</summary>
		public static string ConvertValueStringDbToVis(EFormField eFormField) {
			if(eFormField.FieldType!=EnumEFormFieldType.RadioButtons) {//This only works with radiobutton fields.
				return "";
			}
			List<string> listPickListDb=eFormField.PickListDb.Split(',').ToList();
			List<string> listPickListVis=eFormField.PickListVis.Split(',').ToList();
			int idx=listPickListDb.IndexOf(eFormField.ValueString);
			if(idx==-1) {//ValueString was not found.
				return "";
			}
			return listPickListVis[idx];
		}
		
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<EFormField> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<EFormField>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM eformfield WHERE PatNum = "+POut.Long(patNum);
			return Crud.EFormFieldCrud.SelectMany(command);
		}
		
		///<summary>Gets one EFormField from the db.</summary>
		public static EFormField GetOne(long eFormFieldNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<EFormField>(MethodBase.GetCurrentMethod(),eFormFieldNum);
			}
			return Crud.EFormFieldCrud.SelectOne(eFormFieldNum);
		}

		

		///<summary></summary>
		public static void Update(EFormField eFormField){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eFormField);
				return;
			}
			Crud.EFormFieldCrud.Update(eFormField);
		}

		///<summary></summary>
		public static void Delete(long eFormFieldNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eFormFieldNum);
				return;
			}
			Crud.EFormFieldCrud.Delete(eFormFieldNum);
		}

		*/



	}
}