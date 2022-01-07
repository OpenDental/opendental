using System;

namespace OpenDentBusiness{

	///<summary>One row can hold up to six measurements for one tooth, all of the same type.  Always attached to a perioexam.</summary>
	[Serializable]
	[CrudTable(IsLargeTable=true)]
	public class PerioMeasure:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PerioMeasureNum;
		///<summary>FK to perioexam.PerioExamNum.</summary>
		public long PerioExamNum;
		///<summary>Enum:PerioSequenceType  eg probing, mobility, recession, etc.</summary>
		public PerioSequenceType SequenceType;
		///<summary>Valid values are 1-32. Every measurement must be associated with a tooth.</summary>
		public int IntTooth;
		///<summary>This is used when the measurement does not apply to a surface(mobility and skiptooth).  Valid values for all surfaces are 0 through 19, or -1 to represent no measurement taken.</summary>
		public int ToothValue;
		///<summary>-1 represents no measurement. Values of 100+ represent negative values (only used for Gingival Margins). e.g. To use a value of 105, subtract it from 100. (100 - 105 = -5)</summary>
		public int MBvalue;
		///<summary>.</summary>
		public int Bvalue;
		///<summary>.</summary>
		public int DBvalue;
		///<summary>.</summary>
		public int MLvalue;
		///<summary>.</summary>
		public int Lvalue;
		///<summary>.</summary>
		public int DLvalue;
		///<summary>Timestamp automatically generated and user not allowed to change.  The actual date of entry.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime SecDateTEntry;
		///<summary>Automatically updated by MySQL every time a row is added or changed. Could be changed due to user editing, custom queries or program
		///updates.  Not user editable with the UI.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;

		public PerioMeasure Copy(){
			return (PerioMeasure)this.MemberwiseClone();
		}

		//public PerioMeasure AdjustGMVals() {
		//  PerioMeasure pm=this.Copy();
		//  PerioMeasures.AdjustGMVals(pm);
		//  return pm;
		//}



	}

	///<summary>Blood,pus,plaque,and calculus. Used in ContrPerio.PerioCell</summary>
	[Flags]
	public enum BleedingFlags {
		///<summary>0</summary>
		None=0,
		///<summary>1</summary>
		Blood=1,
		///<summary>2</summary>
		Suppuration=2,
		///<summary>4</summary>
		Plaque=4,
		///<summary>8</summary>
		Calculus=8
	}

	///<summary>Currently, only six surfaces are supported, but more can always be added.</summary>
	public enum PerioSurf {
		///<summary>Might be used for things such as mobility or missing tooth.</summary>
		None,
		///<summary>1</summary>
		MB,
		///<summary>2</summary>
		B,
		///<summary>3</summary>
		DB,
		///<summary>4</summary>
		ML,
		///<summary>5</summary>
		L,
		///<summary>6</summary>
		DL
	}
	

}















