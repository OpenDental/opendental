using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>This is a class for handling US postal codes for states, districts and territories.  Used primarily for validating user input.</summary>
	public class USlocales {
		///<summary>A list of all US locals, including name and postal abbreviation.</summary>
		public static List<StateAbbr> ListAll=new List<StateAbbr>() {
			//50 States.
			new StateAbbr("Alaska","AK"),
			new StateAbbr("Alabama","AL"),
			new StateAbbr("Arkansas","AR"),
			new StateAbbr("Arizona","AZ"),
			new StateAbbr("California","CA"),
			new StateAbbr("Colorado","CO"),
			new StateAbbr("Connecticut","CT"),
			new StateAbbr("Delaware","DE"),
			new StateAbbr("Florida","FL"),
			new StateAbbr("Georgia","GA"),
			new StateAbbr("Hawaii","HI"),
			new StateAbbr("Iowa","IA"),
			new StateAbbr("Idaho","ID"),
			new StateAbbr("Illinois","IL"),
			new StateAbbr("Indiana","IN"),
			new StateAbbr("Kansas","KS"),
			new StateAbbr("Kentucky","KY"),
			new StateAbbr("Louisiana","LA"),
			new StateAbbr("Massachussetts","MA"),
			new StateAbbr("Maryland","MD"),
			new StateAbbr("Maine","ME"),
			new StateAbbr("Michigan","MI"),
			new StateAbbr("Minnesota","MN"),
			new StateAbbr("Missouri","MO"),
			new StateAbbr("Mississippi","MS"),
			new StateAbbr("Montana","MT"),
			new StateAbbr("North Carolina","NC"),
			new StateAbbr("North Dakota","ND"),
			new StateAbbr("Nebraska","NE"),
			new StateAbbr("New Hampshire","NH"),
			new StateAbbr("New Jersey","NJ"),
			new StateAbbr("New Mexico","NM"),
			new StateAbbr("Nevada","NV"),
			new StateAbbr("New York","NY"),
			new StateAbbr("Ohio","OH"),
			new StateAbbr("Oklahoma","OK"),
			new StateAbbr("Oregon","OR"),
			new StateAbbr("Pennsylvania","PA"),
			new StateAbbr("Rhode Island","RI"),
			new StateAbbr("South Carolina","SC"),
			new StateAbbr("South Dakota","SD"),
			new StateAbbr("Tennessee","TN"),
			new StateAbbr("Texas","TX"),
			new StateAbbr("Utah","UT"),
			new StateAbbr("Virginia","VA"),
			new StateAbbr("Vermont","VT"),
			new StateAbbr("Washington","WA"),
			new StateAbbr("Wisconsin","WI"),
			new StateAbbr("West Virginia","WV"),
			new StateAbbr("Wyoming","WY"),
			//US Districts
			new StateAbbr("District of Columbia","DC"),
			//US territories. Reference https://simple.wikipedia.org/wiki/U.S._postal_abbreviations
			new StateAbbr("American Samoa","AS"),
			new StateAbbr("Federated States of Micronesia","FM"),
			new StateAbbr("Guam","GU"),
			new StateAbbr("Marshall Islands","MH"),
			new StateAbbr("Northern Mariana Islands","MP"),
			new StateAbbr("Palau","PW"),
			new StateAbbr("Puerto Rico","PR"),
			new StateAbbr("United States Minor Outlying Islands","UM"),
			new StateAbbr("U.S. Virgin Islands","VI")
		};

		///<summary>Validates the provided postal code is in our list of locales (case insensitive).</summary>
		public static bool IsValidAbbr(string stateAbbr) {
			if(ListAll.Exists(x => x.Abbr==stateAbbr.ToUpper())){
				return true;
			}
			return false;
		}

		///<summary>Checks if the customer's country code is "US", "USA", "United States", or "United States of America" AND that the state abbreviation
		///is a valid state, district, or territory code (case insensitive).</summary>
		public static bool IsInUS(string stateAbbr,string country) {
			string validCountries="US,USA,UNITED STATES,UNITED STATES OF AMERICA";
			if(!IsValidAbbr(stateAbbr)) {
				return false;
			}
			if (string.IsNullOrWhiteSpace(country)) {
				return false;
			}
			if (!validCountries.Contains(country.ToUpper())){
				return false;
			}
			return true;
		}
	}
}
