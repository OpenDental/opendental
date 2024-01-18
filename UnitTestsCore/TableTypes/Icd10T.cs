using System;
using System.Collections.Generic;
using System.Text;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class Icd10T {
		///<summary>Deletes every entry in the in 'icd10' table.</summary>
		public static void ClearIcd10Table() {
			string command="DELETE from icd10";
			DataCore.NonQ(command);
		}

		///<summary>Inserts the new Icd10 code and returns it.</summary>
		public static Icd10 CreateIcd10(string icd10code = "",string description = "") {
			Icd10 icd10=new Icd10();
			if(icd10code=="") {
				icd10.Icd10Code="Z.100.100";
			}
			icd10.Icd10Code=icd10code;
			icd10.Description=description;
			icd10.Icd10Num=Icd10s.Insert(icd10);
			return icd10;
		}

		///<summary>Inserts the first 30 ICD10 codes.</summary>
		public static void Create30Icd10s() {
			List<Icd10> listIcd10s=new List<Icd10>();
			#region Cholera through Salmonella
			listIcd10s.Add(new Icd10 {
				Icd10Code="A00",
				Description="Cholera",
				IsCode="0"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A00.0",
				Description="Cholera due to Vibrio cholerae 01, biovar cholerae",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A00.1",
				Description="Cholera due to Vibrio cholerae 01, biovar eltor",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A00.9",
				Description="Cholera, unspecified",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A01",
				Description="Typhoid and paratyphoid fevers",
				IsCode="0"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A01.0",
				Description="Typhoid fever",
				IsCode="0"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A01.00",
				Description="Typhoid fever, unspecified",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A01.01",
				Description="Typhoid meningitis",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A01.02",
				Description="Typhoid fever with heart involvement",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A01.03",
				Description="Typhoid pneumonia",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A01.04",
				Description="Typhoid arthritis",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A01.05",
				Description="Typhoid osteomyelitis",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A01.09",
				Description="Typhoid fever with other complications",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A01.1",
				Description="Paratyphoid fever A",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A01.2",
				Description="Paratyphoid fever B",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A01.3",
				Description="Paratyphoid fever C",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A01.4",
				Description="Paratyphoid fever, unspecified",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A02",
				Description="Other salmonella infections",
				IsCode="0"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A02.0",
				Description="Salmonella enteritis",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A02.1",
				Description="Salmonella sepsis",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A02.2",
				Description="Localized salmonella infections",
				IsCode="0"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A02.20",
				Description="Localized salmonella infection, unspecified",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A02.21",
				Description="Salmonella meningitis",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A02.22",
				Description="Salmonella pneumonia",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A02.23",
				Description="Salmonella arthritis",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A02.24",
				Description="Salmonella osteomyelitis",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A02.25",
				Description="Salmonella pyelonephritis",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A02.29",
				Description="Salmonella with other localized infection",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A02.8",
				Description="Other specified salmonella infections",
				IsCode="1"
			});
			listIcd10s.Add(new Icd10 {
				Icd10Code="A02.9",
				Description="Salmonella infection, unspecified",
				IsCode="1"
			});
			#endregion
			for(int i=0;i<listIcd10s.Count;i++) {
				Icd10s.Insert(listIcd10s[i]);
			}
		}

	}
}
