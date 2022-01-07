using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness.WebTypes.WebForms;

namespace UnitTestsCore {
	public class WebForms_SheetT {

		///<summary>Creates a given sheet for the following information.</summary>
		public static WebForms_Sheet CreateWebFormSheet(string lName,string fName,DateTime birthdate,string email,List<string> listPhoneNumbers) {
			WebForms_Sheet sheet=new WebForms_Sheet();
			sheet.SheetFields=new List<WebForms_SheetField>();
			WebForms_SheetField field=new WebForms_SheetField();
			field.FieldName="lname";
			field.FieldValue=lName;
			sheet.SheetFields.Add(field);
			field=new WebForms_SheetField();
			field.FieldName="fname";
			field.FieldValue=fName;
			sheet.SheetFields.Add(field);
			field=new WebForms_SheetField();
			field.FieldName="bdate";
			field.FieldValue=birthdate.ToShortDateString();
			sheet.SheetFields.Add(field);
			field=new WebForms_SheetField();
			field.FieldName="email";
			field.FieldValue=email;
			sheet.SheetFields.Add(field);
			foreach(string phoneNumber in listPhoneNumbers) {
				field=new WebForms_SheetField();
				field.FieldName="hmphone";//home, work, cell all treated the same.
				field.FieldValue=phoneNumber;
				sheet.SheetFields.Add(field);
			}
			return sheet;
		}

	}
}
