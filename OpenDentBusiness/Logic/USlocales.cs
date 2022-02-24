using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>This is a class for handling US postal codes for states, districts and territories.  Used primarily for validating user input.</summary>
	public class USlocales {

		///<summary>A list of all US locals, including name and postal abbreviation.</summary>
		public static List<USlocale> ListAll=new List<USlocale>() {
			//50 States.
			new USlocale("Alaska","AK"),
			new USlocale("Alabama","AL"),
			new USlocale("Arkansas","AR"),
			new USlocale("Arizona","AZ"),
			new USlocale("California","CA"),
			new USlocale("Colorado","CO"),
			new USlocale("Connecticut","CT"),
			new USlocale("Delaware","DE"),
			new USlocale("Florida","FL"),
			new USlocale("Georgia","GA"),
			new USlocale("Hawaii","HI"),
			new USlocale("Iowa","IA"),
			new USlocale("Idaho","ID"),
			new USlocale("Illinois","IL"),
			new USlocale("Indiana","IN"),
			new USlocale("Kansas","KS"),
			new USlocale("Kentucky","KY"),
			new USlocale("Louisiana","LA"),
			new USlocale("Massachussetts","MA"),
			new USlocale("Maryland","MD"),
			new USlocale("Maine","ME"),
			new USlocale("Michigan","MI"),
			new USlocale("Minnesota","MN"),
			new USlocale("Missouri","MO"),
			new USlocale("Mississippi","MS"),
			new USlocale("Montana","MT"),
			new USlocale("North Carolina","NC"),
			new USlocale("North Dakota","ND"),
			new USlocale("Nebraska","NE"),
			new USlocale("New Hampshire","NH"),
			new USlocale("New Jersey","NJ"),
			new USlocale("New Mexico","NM"),
			new USlocale("Nevada","NV"),
			new USlocale("New York","NY"),
			new USlocale("Ohio","OH"),
			new USlocale("Oklahoma","OK"),
			new USlocale("Oregon","OR"),
			new USlocale("Pennsylvania","PA"),
			new USlocale("Rhode Island","RI"),
			new USlocale("South Carolina","SC"),
			new USlocale("South Dakota","SD"),
			new USlocale("Tennessee","TN"),
			new USlocale("Texas","TX"),
			new USlocale("Utah","UT"),
			new USlocale("Virginia","VA"),
			new USlocale("Vermont","VT"),
			new USlocale("Washington","WA"),
			new USlocale("Wisconsin","WI"),
			new USlocale("West Virginia","WV"),
			new USlocale("Wyoming","WY"),
			//US Districts
			new USlocale("District of Columbia","DC"),
			//US territories. Reference https://simple.wikipedia.org/wiki/U.S._postal_abbreviations
			new USlocale("American Samoa","AS"),
			new USlocale("Federated States of Micronesia","FM"),
			new USlocale("Guam","GU"),
			new USlocale("Marshall Islands","MH"),
			new USlocale("Northern Mariana Islands","MP"),
			new USlocale("Palau","PW"),
			new USlocale("Puerto Rico","PR"),
			new USlocale("United States Minor Outlying Islands","UM"),
			new USlocale("U.S. Virgin Islands","VI")
		};

		///<summary>Validates the provided postal code is in our list of locales (case insensitive).</summary>
		public static bool IsValidAbbr(string stateAbbr) {
			return ListAll.Any(x => x.PostalAbbr==stateAbbr.ToUpper());
		}

		///<summary>Checks if the customer's country code is "US", "USA", "United States", or "United States of America" AND that the state abbreviation
		///is a valid state, district, or territory code (case insensitive).</summary>
		public static bool IsInUS(string stateAbbr,string country) {
			string validCountries="US,USA,UNITED STATES,UNITED STATES OF AMERICA";
			return USlocales.IsValidAbbr(stateAbbr) && !string.IsNullOrWhiteSpace(country) && validCountries.Contains(country.ToUpper());
		}
	}

	///<summary>Just a wrapper for a Tuple(string,string) which stores the locale name as Item1 and the postal abbreviation as Item2.</summary>
	public class USlocale:Tuple<string,string> {

		public string Name { get { return Item1; } }

		public string PostalAbbr { get { return Item2; } }

		public USlocale(string name,string abbr):base(name,abbr) { }
	}
}
