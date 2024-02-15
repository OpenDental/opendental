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

		public static PatFieldDef UpsertPatFieldDef(string fieldName,PatFieldType fieldType,string pickList,int itemOrder=0,bool isHidden=false) {
			//If exists, update in db. Else insert to db.
			string command=$"SELECT * FROM patfielddef WHERE FieldName='{fieldName}'";
			PatFieldDef existingDef=OpenDentBusiness.Crud.PatFieldDefCrud.SelectOne(command);
			if(existingDef==null) {
				existingDef=new PatFieldDef() {
					FieldName=fieldName,
					FieldType=fieldType,
					PickList=pickList,
					ItemOrder=itemOrder,
					IsHidden=isHidden,
				};
				PatFieldDefs.Insert(existingDef);
			}
			else {
				existingDef.FieldType=fieldType; 
				existingDef.PickList=pickList;
				existingDef.ItemOrder=itemOrder;
				existingDef.IsHidden=isHidden;				
				PatFieldDefs.Update(existingDef);
			}
			PatFieldDefs.RefreshCache();
			return existingDef;
		}

		///<summary>Deletes (almost) everything from the patfielddef table. Preserves dump file's CareCredit PatFieldDefs.</summary>
		public static void ClearPatFieldDefTable() {
			string command="DELETE FROM patfielddef WHERE PatFieldDefNum > 3";
			DataCore.NonQ(command);
		}
	}
}
