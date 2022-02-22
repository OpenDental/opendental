using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDental.SmartCards.Belgium {

	public enum ErrorCodeOptions : int {
		/// <summary>
		/// Function succeeded
		/// </summary>
		OK = 0,
		/// <summary>
		/// Unknown system error (see system error code)
		/// </summary>
		E_SYSTEM = 1,
		/// <summary>
		/// Unknown PC/SC error (see PC/SC error code)
		/// </summary>
		E_PCSC = 2,
		/// <summary>
		/// Unknown card error (see card status word)
		/// </summary>
		E_CARD = 3,
		/// <summary>
		/// Invalid parameter (NULL pointer, out of bound, etc.)
		/// </summary>
		E_BAD_PARAM = 4,
		/// <summary>
		/// An internal consistency check failed
		/// </summary>
		E_INTERNAL = 5,
		/// <summary>
		/// The supplied handle was invalid
		/// </summary>
		E_INVALID_HANDLE = 6,
		/// <summary>
		/// The data buffer to receive returned data is too small for the 
		/// returned data
		/// </summary>
		E_INSUFFICIENT_BUFFER = 7,
		/// <summary>
		/// An internal communications error has been detected
		/// </summary>
		E_COMM_ERROR = 8,
		/// <summary>
		/// A specified timeout value has expired
		/// </summary>
		E_TIMEOUT = 9,
		/// <summary>
		/// Unknown card detected
		/// </summary>
		E_UNKNOWN_CARD = 10,
		/// <summary>
		/// Input on pinpad cancelled
		/// </summary>
		E_KEYPAD_CANCELLED = 11,
		/// <summary>
		/// Timout returned from pinpad
		/// </summary>
		E_KEYPAD_TIMEOUT = 12,
		/// <summary>
		/// The two PINs did not match
		/// </summary>
		E_KEYPAD_PIN_MISMATCH = 13,
		/// <summary>
		/// Message too long on pinpad
		/// </summary>
		E_KEYPAD_MSG_TOO_LONG = 14,
		/// <summary>
		///  Invalid PIN length
		/// </summary>
		E_INVALID_PIN_LENGTH = 15,
		/// <summary>
		/// Error in a signature verification or a certificate validation
		/// </summary>
		E_VERIFICATION = 16,
		/// <summary>
		///  Library not initialized
		/// </summary>
		E_NOT_INITIALIZED = 17,
		/// <summary>
		/// An internal error has been detected, but the source is unknown
		/// </summary>
		E_UNKNOWN = 18,
		/// <summary>
		/// Function is not supported
		/// </summary>
		E_UNSUPPORTED_FUNCTION = 19,
		/// <summary>
		/// Incorrect library version
		/// </summary>
		E_INCORRECT_VERSION = 20
	}
}
