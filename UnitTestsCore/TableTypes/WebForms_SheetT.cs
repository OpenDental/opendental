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
			field.SheetFieldDefNum=0;
			sheet.SheetFields.Add(field);
			field=new WebForms_SheetField();
			field.FieldName="fname";
			field.FieldValue=fName;
			field.SheetFieldDefNum=1;
			sheet.SheetFields.Add(field);
			field=new WebForms_SheetField();
			field.FieldName="bdate";
			field.FieldValue=birthdate.ToShortDateString();
			field.SheetFieldDefNum=2;
			sheet.SheetFields.Add(field);
			field=new WebForms_SheetField();
			field.FieldName="email";
			field.FieldValue=email;
			field.SheetFieldDefNum=3;
			sheet.SheetFields.Add(field);
			int i=3;
			foreach(string phoneNumber in listPhoneNumbers) {
				field=new WebForms_SheetField();
				field.FieldName="hmphone";//home, work, cell all treated the same.
				field.FieldValue=phoneNumber;
				field.SheetFieldDefNum=i++;
				sheet.SheetFields.Add(field);
			}
			return sheet;
		}

	}
}
