using CodeBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	public class EFormInternal {

		public static List<EFormDef> GetAllInternal() {
			List<EFormDef> listEFormDefs=new List<EFormDef>();
			//Unlike sheets, we don't bother to have an enum for our internal types. 
			//Instead, they are just listed here for loading up in the setup window.
			EFormDef eFormDef;
			eFormDef=GetEFormFromResource(Properties.Resources.EFormPatientRegistration);
			listEFormDefs.Add(eFormDef);
			eFormDef=GetEFormFromResource(Properties.Resources.EFormMedicalHistory);
			listEFormDefs.Add(eFormDef);
			eFormDef=GetEFormFromResource(Properties.Resources.EFormExtractionConsent);
			listEFormDefs.Add(eFormDef);
			//Only show the internal Demo EForm in Debug mode.
			//I think maybe this is no longer useful.
			//if(ODBuild.IsDebug()) {
			//	eFormDef=GetEFormFromResource(Properties.Resources.EFormDemo);
			//	listEFormDefs.Add(eFormDef);
			//}
			return listEFormDefs;
		}

		private static EFormDef GetEFormFromResource(string xmlDoc) {
			EFormDef eFormDef=new EFormDef();
			XmlSerializer xmlSerializer=new XmlSerializer(typeof(EFormDef));
			using TextReader textReader = new StringReader(xmlDoc);
			eFormDef=(EFormDef)xmlSerializer.Deserialize(textReader);
			for(int i=0;i<eFormDef.ListEFormFieldDefs.Count;i++){
				eFormDef.ListEFormFieldDefs[i].EFormDefNum=0;
				eFormDef.ListEFormFieldDefs[i].EFormFieldDefNum=0;
				//XML parsers are required to normalize \r\n to just \n.
				//That's a problem, because without the \r, it will display improperly in textboxes.
				//So, we convert \n that are by themselves to \r\n.
				//In the XML, line feeds (\n) within tag values simply show as white space, but we could theoretically use &#10; if we want.
				//In sheets, we were missing this for a while, so lone \n's snuck in and we have tricks to deal with those.
				//In EForms, the textbox where this would be a problem is the one in the field editor, like FrmEFormLabelEdit.cs.
				string pattern=@"(?<!\r)"//Negative lookbehind assertion. It looks for absence of \r before any \n
					+"\n";
				eFormDef.ListEFormFieldDefs[i].ValueLabel=Regex.Replace(eFormDef.ListEFormFieldDefs[i].ValueLabel,pattern,"\r\n");
			}
			eFormDef.IsInternal=true;
			eFormDef.EFormDefNum=0;
			return eFormDef;
		}
		

	}
}
