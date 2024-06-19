using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace UnitTestsWeb {
	public class PatFieldDefT {
		public static PatFieldDef CreateTextPatFieldDef(int itemOrder=0) {
			PatFieldDef patFieldDef=new PatFieldDef {
				FieldName="Text",
				FieldType=PatFieldType.Text,
				IsHidden=false,
				ItemOrder=itemOrder,
				PickList="",
			};
			long patFieldDefNum=PatFieldDefs.Insert(patFieldDef);
			patFieldDef.PatFieldDefNum=patFieldDefNum;
			PatFieldDefs.Update(patFieldDef);
			return patFieldDef;
		}

		///<summary>Creates a PatFieldDef and either inserts if the fieldName is not already in use, or updates the PatFieldDef with a matching fieldName. Refreshes cache.
		///The PickList field was deprecated in 24.1, so it will always be inserted as an empty string.</summary>
		public static PatFieldDef UpsertPatFieldDef(string fieldName,PatFieldType fieldType,string pickList="deprecated",int itemOrder=0,bool isHidden=false) {
			//If exists, update in db. Else insert to db.
			string command=$"SELECT * FROM patfielddef WHERE FieldName='{fieldName}'";
			PatFieldDef existingDef=OpenDentBusiness.Crud.PatFieldDefCrud.SelectOne(command);
			if(existingDef==null) {
				existingDef=new PatFieldDef() {
					FieldName=fieldName,
					FieldType=fieldType,
					PickList="",
					ItemOrder=itemOrder,
					IsHidden=isHidden,
				};
				PatFieldDefs.Insert(existingDef);
			}
			else {
				existingDef.FieldType=fieldType; 
				existingDef.PickList="";
				existingDef.ItemOrder=itemOrder;
				existingDef.IsHidden=isHidden;
				PatFieldDefs.Update(existingDef);
			}
			PatFieldDefs.RefreshCache();
			return existingDef;
		}

		///<summary>Deletes (almost) everything from the patfielddef table. Preserves dump file's CareCredit PatFieldDefs.</summary>
		public static void ClearPatFieldDefTable() {
			string command="DELETE FROM patfielddef WHERE FieldName NOT IN ('CareCredit Pre-Approval Status','CareCredit Pre-Approval Amount','CareCredit Available Credit')";
			DataCore.NonQ(command);
		}
	}
}
