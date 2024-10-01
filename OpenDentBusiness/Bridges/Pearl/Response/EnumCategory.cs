using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness.Pearl {
//Caries - Progressed or Caries - Incipient based depends if there exists dentin in the relationships metric_value

	///<summary>Pearl's enum for annotation categories. This will be used to set the ImageDraw's PearlLayer using EnumCategoryOD when processing results.</summary>
	public enum EnumCategory {
		///<summary>0 - None.</summary>
		None=0,
		///<summary>1 - Crown.</summary>
		Crown=1,
		///<summary>3 - Periapical Radiolucency.</summary>
		PeriapicalRadiolucency=3,
		///<summary>4 - Filling.</summary>
		Filling=4,
		///<summary>5 - Measurements.</summary>
		Measurements=5,
		///<summary>6 - Caries.</summary>
		Caries=6,
		///<summary>7 - Notable Margin.</summary>
		NotableMargin=7,
		///<summary>8 - Implant.</summary>
		Implant=8,
		///<summary>10 - Root Canal.</summary>
		RootCanal=10,
		///<summary>11 - Bridge.</summary>
		Bridge=11,
		///<summary>14 - Calculus.</summary>
		Calculus=14,
		///<summary>23 - Tooth Parts.</summary>
		ToothParts=23,
	}

	///<summary>Pearl's enum for toothpart conditions. This will be used to set the ImageDraw's PearlLayer using EnumCategoryOD when processing results.</summary>
	public enum EnumCondition {
		///<summary>0 - None.</summary>
		None=0,
		///<summary>90 - Bone.</summary>
		Bone=90,
		///<summary>91 - Enamel.</summary>
		Enamel=91,
		///<summary>92 - Dentin.</summary>
		Dentin=92,
		///<summary>93 - Pulp.</summary>
		Pulp=93,
		///<summary>94 - Cementum.</summary>
		Cementum=94,
		///<summary>95 - Restoration.</summary>
		Restoration=95,
		///<summary>16 - Inferior Alveolar Nerve. Not included in legend.</summary>
		InferiorAlveolarNerve=16,
		///<summary>17 - Sinus. Not included in legend.</summary>
		Sinus=17,
		///<summary>18 - NasalCavity. Not included in legend.</summary>
		NasalCavity=18,
		///<summary>19 - Bone. Not included in legend.</summary>
		Bo=19,
	}

	///<summary>This is for showing/hiding layers. Jordan-This list is tentative. I feel like there are a few missing. Each incoming Pearl annotation or toothpart must be able to be assigned to exactly one of these.</summary>
	public enum EnumCategoryOD {
		///<summary>0 - None.</summary>
		None=0,
		///<summary>1 - Crown.</summary>
		Crown=1,
		///<summary>2 - Periapical Radiolucency.</summary>
		PeriapicalRadiolucency=2,
		///<summary>3 - Filling.</summary>
		Filling=3,
		///<summary>4 - Anatomy.</summary>
		Anatomy=4,
		///<summary>5 - Caries -Progressed.</summary>
		CariesProgressed=5,
		///<summary>6 - Notable Margin.</summary>
		NotableMargin=6,
		///<summary>7 - Implant.</summary>
		Implant=7,
		///<summary>8 - Root Canal.</summary>
		RootCanal=8,
		///<summary>9 - Bridge.</summary>
		Bridge=9,
		///<summary>10 - Calculus.</summary>
		Calculus=10,
		///<summary>11 - Measurements.</summary>
		Measurements=11,
		///<summary>12 - Bone (Tooth Part).</summary>
		Bone=12,
		///<summary>13 - Cementum (Tooth Part).</summary>
		Cementum=13,
		///<summary>14 - Dentin (Tooth Part).</summary>
		Dentin=14,
		///<summary>15 - Enamel (Tooth Part).</summary>
		Enamel=15,
		///<summary>16 - Pulp (Tooth Part).</summary>
		Pulp=16,
		///<summary>17 - Restoration (Tooth Part).</summary>
		Restoration=17,
		///<summary>18 - Inferior Alveolar Nerve (Tooth Part). Not included in legend.</summary>
		InferiorAlveolarNerve=18,
		///<summary>19 - Sinus (Tooth Part). Not included in legend.</summary>
		Sinus=19,
		///<summary>20 - NasalCavity (Tooth Part). Not included in legend.</summary>
		NasalCavity=20,
		///<summary>21 - Caries -Incipient.</summary>
		CariesIncipient=21,
	}
}
