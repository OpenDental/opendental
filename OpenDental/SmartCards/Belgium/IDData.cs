using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace OpenDental.SmartCards.Belgium {

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct IDData {
		[MarshalAs(UnmanagedType.I2)]
		public short Version;
		[MarshalAs(UnmanagedType.ByValArray,
				SizeConst = BelgianIdentityCard.MAX_CARD_NUMBER_LEN + 1)]
		public char[] CardNumber;
		[MarshalAs(UnmanagedType.ByValArray,
		   SizeConst = BelgianIdentityCard.MAX_CHIP_NUMBER_LEN + 1)]
		public char[] ChipNumber;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = BelgianIdentityCard.MAX_DATE_BEGIN_LEN + 1)]
		public char[] ValidityDateBegin;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = BelgianIdentityCard.MAX_DATE_END_LEN + 1)]
		public char[] ValidityDateEnd;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst =
					   BelgianIdentityCard.MAX_DELIVERY_MUNICIPALITY_LEN + 1)]
		public byte[] Municipality;
		[MarshalAs(UnmanagedType.ByValArray,
		   SizeConst = BelgianIdentityCard.MAX_NATIONAL_NUMBER_LEN + 1)]
		public char[] NationalNumber;
		[MarshalAs(UnmanagedType.ByValArray,
		   SizeConst = BelgianIdentityCard.MAX_NAME_LEN + 1)]
		public byte[] Name;
		[MarshalAs(UnmanagedType.ByValArray,
		   SizeConst = BelgianIdentityCard.MAX_FIRST_NAME1_LEN + 1)]
		public byte[] FirstName1;
		[MarshalAs(UnmanagedType.ByValArray,
		   SizeConst = BelgianIdentityCard.MAX_FIRST_NAME2_LEN + 1)]
		public byte[] FirstName2;
		[MarshalAs(UnmanagedType.ByValArray,
		   SizeConst = BelgianIdentityCard.MAX_FIRST_NAME3_LEN + 1)]
		public byte[] FirstName3;
		[MarshalAs(UnmanagedType.ByValArray,
		   SizeConst = BelgianIdentityCard.MAX_NATIONALITY_LEN + 1)]
		public char[] Nationality;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = BelgianIdentityCard.MAX_BIRTHPLACE_LEN + 1)]
		public byte[] BirthLocation;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = BelgianIdentityCard.MAX_BIRTHDATE_LEN + 1)]
		public char[] BirthDate;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = BelgianIdentityCard.MAX_SEX_LEN + 1)]
		public char[] Sex;
		[MarshalAs(UnmanagedType.ByValArray,
		   SizeConst = BelgianIdentityCard.MAX_NOBLE_CONDITION_LEN + 1)]
		public byte[] NobleCondition;
		[MarshalAs(UnmanagedType.I4)]
		public int DocumentType;
		[MarshalAs(UnmanagedType.Bool)]
		public bool WhiteCane;
		[MarshalAs(UnmanagedType.Bool)]
		public bool YellowCane;
		[MarshalAs(UnmanagedType.Bool)]
		public bool ExtendedMinority;
		[MarshalAs(UnmanagedType.ByValArray,
		   SizeConst = BelgianIdentityCard.MAX_HASH_PICTURE_LEN, ArraySubType = UnmanagedType.U1)]
		public byte[] HashPhoto;
		[MarshalAs(UnmanagedType.ByValArray,
			SizeConst = 6, ArraySubType = UnmanagedType.U1)]
		public byte[] RFU;
	}
}
