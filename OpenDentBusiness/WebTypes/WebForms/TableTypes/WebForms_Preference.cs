using OpenDentBusiness;
using System.Drawing;
using System.Xml.Serialization;
using System;
using System.Runtime.Serialization;

namespace OpenDentBusiness.WebTypes.WebForms {
	[Serializable]
	[CrudTable(IsMissingInGeneral=true,CrudLocationOverride=@"..\..\..\OpenDentBusiness\WebTypes\WebForms\Crud",NamespaceOverride="OpenDentBusiness.WebTypes.WebForms.Crud",CrudExcludePrefC=true)]
	public class WebForms_Preference:TableBase {
		///<summary>This is not a primar key but the CRUD generator requires one.  FK to customers.patient.PatNum.</summary>
		[CrudColumn(IsPriKey=true)]
		public long DentalOfficeID;
		///<summary></summary>
		[XmlIgnore]
		public Color ColorBorder;
		///<summary></summary>
		public string CultureName;
		///<summary>When true disables signatures in web forms.  False by default.</summary>
		public bool DisableSignatures;
		
		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ColorBorder",typeof(int))]
		public int ColorOverrideXml {
			get {
				return ColorBorder.ToArgb();
			}
			set {
				ColorBorder=Color.FromArgb(value);
			}
		}

    public WebForms_Preference(){
			
		}

    public WebForms_Preference(string cultureName=""){
			this.ColorBorder=Color.White;
			this.CultureName=cultureName;
		}
		
    public WebForms_Preference(Color colorBorder, string cultureName=""){
			this.ColorBorder=colorBorder;
			this.CultureName=cultureName;	
		}
		
    public WebForms_Preference Copy(){
			return (WebForms_Preference)this.MemberwiseClone();
		}
	}
}