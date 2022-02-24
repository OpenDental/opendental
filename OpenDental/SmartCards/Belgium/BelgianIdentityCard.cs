using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using OpenDentBusiness;

namespace OpenDental.SmartCards.Belgium {
    public class BelgianIdentityCard : SmartCardService {
        public BelgianIdentityCard(ISmartCardManager manager)
            : base(manager) {
            SupportedAtrs.Add(new byte[] { 0x3B, 0x98, 0x94, 0x40, 0x0A, 0xA5, 0x03, 0x01, 0x01, 0x01, 0xAD, 0x13, 0x10 });
            SupportedAtrs.Add(new byte[] { 0x3B, 0x98, 0x13, 0x40, 0x0A, 0xA5, 0x03, 0x01, 0x01, 0x01, 0xAD, 0x13, 0x11 });
        }

        #region Version Constants
        //
        // version info
        //
        private const int INTERFACE_VERSION = 1;	// Changes each time the interface is modified 
        private const int INTERFACE_COMPAT_VERSION = 1;	// Stays until incompatible changes in existing functions 
        #endregion

        #region Field Constants
        internal const int MAX_CARD_NUMBER_LEN = 12;
        internal const int MAX_CHIP_NUMBER_LEN = 32;
        internal const int MAX_DATE_BEGIN_LEN = 10;
        internal const int MAX_DATE_END_LEN = 10;
        internal const int MAX_DELIVERY_MUNICIPALITY_LEN = 80;
        internal const int MAX_NATIONAL_NUMBER_LEN = 11;
        internal const int MAX_NAME_LEN = 110;
        internal const int MAX_FIRST_NAME1_LEN = 95;
        internal const int MAX_FIRST_NAME2_LEN = 50;
        internal const int MAX_FIRST_NAME3_LEN = 3;
        internal const int MAX_NATIONALITY_LEN = 3;
        internal const int MAX_BIRTHPLACE_LEN = 80;
        internal const int MAX_BIRTHDATE_LEN = 10;
        internal const int MAX_SEX_LEN = 1;
        internal const int MAX_NOBLE_CONDITION_LEN = 50;
        internal const int MAX_DOCUMENT_TYPE_LEN = 2;
        internal const int MAX_SPECIAL_STATUS_LEN = 2;
        internal const int MAX_HASH_PICTURE_LEN = 20;
        internal const int MAX_STREET_LEN = 80;
        internal const int MAX_STREET_NR = 10;
        internal const int MAX_STREET_BOX_NR = 6;
        internal const int MAX_ZIP_LEN = 4;
        internal const int MAX_MUNICIPALITY_LEN = 67;
        internal const int MAX_COUNTRY_LEN = 4;
        internal const int MAX_RAW_ADDRESS_LEN = 512;
        internal const int MAX_RAW_ID_LEN = 1024;
        internal const int MAX_PICTURE_LEN = 4096;
        internal const int MAX_CERT_LEN = 2048;
        internal const int MAX_CERT_NUMBER = 10;
        internal const int MAX_CERT_LABEL_LEN = 256;
        internal const int MAX_SIGNATURE_LEN = 256;
        internal const int MAX_CARD_DATA_LEN = 28;
        internal const int MAX_CARD_DATA_SIG_LEN = MAX_SIGNATURE_LEN + MAX_CARD_DATA_LEN;
        internal const int MAX_CHALLENGE_LEN = 20;
        internal const int MAX_RESPONSE_LEN = 128;
        #endregion

        #region Interop Constants
        private const string EidLibDLL = "eidlib.dll";
        #endregion

        #region External methods
        [DllImport(EidLibDLL, EntryPoint = "BEID_InitEx", SetLastError = true)]
        private static extern Status InitEx(string readerName,
            OCSPPolicyOptions ocsp, CRLPolicyOptions crl, out IntPtr cardHandle,
            int interfaceVersion, int interfaceCompVersion);

        [DllImport(EidLibDLL, EntryPoint = "BEID_Exit", SetLastError = true)]
        internal static extern Status Exit();

        [DllImport(EidLibDLL, EntryPoint = "BEID_GetID", SetLastError = true)]
        internal static extern Status GetID(out IDData data, out CertifCheck check);

        #endregion

        public override Patient GetPatientInfo(string reader) {
            //
            // Define handle
            //
            IntPtr cardHandle;
            //
            // Initialize Library
            //
            Status returnStatus = InitEx(reader, OCSPPolicyOptions.NotUsed, CRLPolicyOptions.NotUsed, out cardHandle, 1, 1);

            // Read the ID from the card.
            IDData data;
            CertifCheck check;
            returnStatus = GetID(out data, out check);
            Patient patient = new Patient();

            string firstName1 = GetString(data.FirstName1);
            string firstName2 = GetString(data.FirstName2);
            string firstName3 = GetString(data.FirstName3);
            string name = GetString(data.Name);

            // Populate first name field.
            patient.FName = string.Empty;
            if (!string.IsNullOrEmpty(firstName1)) {
                if (patient.FName.Length > 0)
                    patient.FName += " ";
                patient.FName += firstName1;
            }
            if (!string.IsNullOrEmpty(firstName2)) {
                if (patient.FName.Length > 0)
                    patient.FName += " ";
                patient.FName += firstName2;
            }

            if (!string.IsNullOrEmpty(firstName3)) {
                if (patient.FName.Length > 0)
                    patient.FName += " ";
                patient.FName += firstName3;
            }

            // Populate last name field.
            patient.LName = name;

            Exit();

            return patient;
        }

        private string GetString(byte[] bytes) {
            if (bytes == null)
                return null;

            if (bytes.Length == 0)
                return string.Empty;

            int length = bytes.Length;
            int count = 0;

            for (int i = 0; i < length; i++) {
                if (bytes[i] == '\0') {
                    count = i;
                    break;
                }
            }

            return Encoding.ASCII.GetString(bytes, 0, count);
        }
    }
}
