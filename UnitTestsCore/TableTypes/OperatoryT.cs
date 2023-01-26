using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class OperatoryT {

		///<summary></summary>
		public static Operatory CreateOperatory(string abbrev="",string opName="",long provDentist=0,long provHygienist=0,long clinicNum=0
			,bool isHygiene=false,bool isWebSched=false,int itemOrder=0)
		{
			Operatory op=new Operatory();
			op.Abbrev=abbrev;
			op.ClinicNum=clinicNum;
			op.IsHygiene=isHygiene;
			op.IsWebSched=isWebSched;
			op.ItemOrder=itemOrder;
			if(opName=="") {
				op.OpName="ClinicNum: "+clinicNum.ToString();
			}
			else {
				op.OpName=opName;
			}
			op.ProvDentist=provDentist;
			op.ProvHygienist=provHygienist;
			Operatories.Insert(op);
			if(abbrev=="") {
				op.Abbrev=op.OperatoryNum.ToString();
				Operatories.Update(op);
			}
			Operatories.RefreshCache();
			return op;
		}

		public static void Update(Operatory op) {
			Operatories.Update(op);
			Operatories.RefreshCache();
		}

		///<summary>Deletes everything from the operatory table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearOperatoryTable() {
			string command="DELETE FROM operatory WHERE OperatoryNum > 0";
			DataCore.NonQ(command);
		}

		/// <summary>Creates a list of  3 operatories</summary>
		public static List<Operatory> CreateListOperatories() {
			List<Operatory> listOperatories=new List<Operatory>();
			Operatory operatoryOne=new Operatory();
			operatoryOne.OpName="Hoth";
			operatoryOne.Abbrev="HTH";
			Operatory operatoryTwo=new Operatory();
			operatoryTwo.OpName="Kashyyk";
			operatoryTwo.Abbrev="KSH";
			Operatory operatoryThree=new Operatory();
			operatoryThree.OpName="Death Star";
			operatoryThree.Abbrev="DSTR";
			listOperatories.Add(operatoryOne);
			listOperatories.Add(operatoryTwo);
			listOperatories.Add(operatoryThree);
			return listOperatories;
		}

		/// <summary>Creates list of 3 operatories with some empyt fields.</summary>
		public static List<Operatory> CreateListOperatoriesEmpties() {
			List<Operatory> listOperatories=new List<Operatory>();
			Operatory operatoryOne=new Operatory();
			operatoryOne.OpName="Hoth";
			Operatory operatoryTwo=new Operatory();
			operatoryTwo.OpName="Kashyyk";
			Operatory operatoryThree=new Operatory();
			operatoryThree.OpName="Death Star";
			listOperatories.Add(operatoryOne);
			listOperatories.Add(operatoryTwo);
			listOperatories.Add(operatoryThree);
			return listOperatories;
		}

	}
}
