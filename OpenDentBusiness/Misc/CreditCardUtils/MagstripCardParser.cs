using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	public class MagstripCardParser {

		private const char TRACK_SEPARATOR='?';
		private const char FIELD_SEPARATOR='^';
		private string _inputStripeStr;
		private string _track1Data;
		private string _track2Data;
		private string _track3Data;
		private bool _needsParsing;
		private bool _hasTrack1;
		private bool _hasTrack2;
		private bool _hasTrack3;
		private string _accountHolder;
		private string _firstName;
		private string _lastName;
		private string _accountNumber;
		private int _expMonth;
		private int _expYear;

		public MagstripCardParser(string trackString) {
			_inputStripeStr=trackString;
			_needsParsing=true;
			Parse();
		}

		#region Properties
		public bool HasTrack1 {
			get { return _hasTrack1; }
		}

		public bool HasTrack2 {
			get { return _hasTrack2; }
		}

		public bool HasTrack3 {
			get { return _hasTrack3; }
		}

		public string Track1 {
			get { return _track1Data; }
		}

		public string Track2 {
			get { return _track2Data; }
		}

		public string Track3 {
			get { return _track3Data; }
		}

		public string TrackData {
			get { return _track1Data+_track2Data+_track3Data; }
		}

		public string AccountName {
			get { return _accountHolder; }
		}

		public string FirstName {
			get { return _firstName; }
		}

		public string LastName {
			get { return _lastName; }
		}

		public string AccountNumber {
			get { return _accountNumber; }
		}

		public int ExpirationMonth {
			get { return _expMonth; }
		}

		public int ExpirationYear {
			get { return _expYear; }
		}
		#endregion

		protected void Parse() {
			if(!_needsParsing) {
				return;
			}
			try {
				//Example: Track 1 Data Only
				//%B1234123412341234^CardUser/John^030510100000019301000000877000000?
				//Key off of the presence of "^" but not "="
				//Example: Track 2 Data Only
				//;1234123412341234=0305101193010877?
				//Key off of the presence of "=" but not "^"
				//Determine the presence of special characters
				string[] tracks=_inputStripeStr.Split(new char[] { TRACK_SEPARATOR },StringSplitOptions.RemoveEmptyEntries);
				if(tracks.Length>0) {
					_hasTrack1=true;
					_track1Data=tracks[0];
				}
				if(tracks.Length>1) {
					_hasTrack2=true;
					_track2Data=tracks[1];
				}
				if(tracks.Length>2) {
					_hasTrack3=true;
					_track3Data=tracks[2];
				}
				if(_hasTrack1) {
					ParseTrack1();
				}
				if(_hasTrack2) {
					ParseTrack2();
				}
				if(_hasTrack3) {
					ParseTrack3();
				}
			}
			catch(MagstripCardParseException) {
				throw;
			}
			catch(Exception ex) {
				throw new MagstripCardParseException(ex);
			}
			_needsParsing=false;
		}

		private void ParseTrack1() {
			if(String.IsNullOrEmpty(_track1Data)) {
				throw new MagstripCardParseException("Track 1 data is empty.");
			}
			string[] parts=_track1Data.Split(new char[] { FIELD_SEPARATOR },StringSplitOptions.None);
			if(parts.Length!=3) {
				throw new MagstripCardParseException("Missing last field separator (^) in track 1 data.");
			}
			_accountNumber=CreditCardUtils.StripNonDigits(parts[0]);
			if(!String.IsNullOrEmpty(parts[1])) {
				_accountHolder=parts[1].Trim();
			}
			if(!String.IsNullOrEmpty(_accountHolder)) {
				int nameDelim=_accountHolder.IndexOf("/");
				if(nameDelim>-1) {
					_lastName=_accountHolder.Substring(0,nameDelim);
					_firstName=_accountHolder.Substring(nameDelim+1);
				}
			}
			//date format: YYMM
			string expDate=parts[2].Substring(0,4);
			_expYear=ParseExpireYear(expDate);
			_expMonth=ParseExpireMonth(expDate);
		}

		private void ParseTrack2() {
			if(String.IsNullOrEmpty(_track2Data)) {
				throw new MagstripCardParseException("Track 2 data is empty.");
			}
			if(_track2Data.StartsWith(";")) {
				_track2Data=_track2Data.Substring(1);
			}
			//may have already parsed this info out if track 1 data present
			if(String.IsNullOrEmpty(_accountNumber) || (_expMonth==0 || _expYear==0)) {
				//Track 2 only cards
				//Ex: ;1234123412341234=0305101193010877?
				int sepIndex=_track2Data.IndexOf('=');
				if(sepIndex<0) {
					throw new MagstripCardParseException("Invalid track 2 data.");
				}
				string[] parts=_track2Data.Split(new char[] { '=' },StringSplitOptions.RemoveEmptyEntries);
				if(parts.Length!=2) {
					throw new MagstripCardParseException("Missing field separator (=) in track 2 data.");
				}
				if(String.IsNullOrEmpty(_accountNumber)) {
					_accountNumber=CreditCardUtils.StripNonDigits(parts[0]);
				}
				if(_expMonth==0 || _expYear==0) {
					//date format: YYMM
					string expDate=parts[1].Substring(0,4);
					_expYear=ParseExpireYear(expDate);
					_expMonth=ParseExpireMonth(expDate);
				}
			}
		}

		private void ParseTrack3() {
			//not implemented
		}

		private int ParseExpireMonth(string s) {
			s=CreditCardUtils.StripNonDigits(s);
			if(!ValidateExpiration(s)) {
				return 0;
			}
			if(s.Length>4) {
				s=s.Substring(0,4);
			}
			return int.Parse(s.Substring(2,2));
		}

		private int ParseExpireYear(string s) {
			s=CreditCardUtils.StripNonDigits(s);
			if(!ValidateExpiration(s)) {
				return 0;
			}
			if(s.Length>4) {
				s=s.Substring(0,4);
			}
			int y=int.Parse(s.Substring(0,2));
			if(y>80) {
				y+=1900;
			}
			else {
				y+=2000;
			}
			return y;
		}

		private bool ValidateExpiration(string s) {
			if(String.IsNullOrEmpty(s)) {
				return false;
			}
			if(s.Length<4) {
				return false;
			}
			return true;
		}

	}

	public class MagstripCardParseException:Exception {

		public MagstripCardParseException(Exception cause)
			: base(cause.Message,cause) {
		}

		public MagstripCardParseException(string msg)
			: base(msg) {
		}

		public MagstripCardParseException(string msg,Exception cause)
			: base(msg,cause) {
		}
	}

}
