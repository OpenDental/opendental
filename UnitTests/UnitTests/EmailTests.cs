using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestsCore;
using OpenDental;
using System.Reflection;
using System.Globalization;
using MySql.Data.MySqlClient;
using CodeBase;

namespace UnitTests.Email_Tests {
	[TestClass]
	public class EmailTests:TestBase {
		[TestMethod]
		public void EmailMessages_FindAndReplacePostalAddressTag() {
			//Format disclaimer.
			PrefT.UpdateString(PrefName.EmailDisclaimerTemplate,"This email has been sent to you from:\r\n[PostalAddress].\r\n\r\nHow to unsubscribe:\r\nIf you no longer want to receive any email messages from us, simply reply to this email with the word \"unsubscribe\" in the subject line.");
			//Setup practice address.
			PrefT.UpdateString(PrefName.PracticeAddress,"Practice Address1 Here");
			PrefT.UpdateString(PrefName.PracticeAddress2,"3275 Marietta St SE");
			PrefT.UpdateString(PrefName.PracticeCity,"Salem");
			PrefT.UpdateString(PrefName.PracticeST,"OR");
			PrefT.UpdateString(PrefName.PracticeZip,"97317");
			//Setup clinic address.
			Clinic clinic=ClinicT.CreateClinic();
			clinic.Address="Clinic Address1 Here";
			Clinics.Update(clinic);
			Clinics.RefreshCache();
			//Turn feature off.
			PrefT.UpdateBool(PrefName.EmailDisclaimerIsOn,false);
			string emailBody="Hi, this is an email.\r\n\r\nRegards,\r\nEvery OD Engineer... ever.";
			string emailBodyWithDisclaimer=EmailMessages.FindAndReplacePostalAddressTag(emailBody,0);
			//Feature is off so no disclaimer added.
			Assert.AreEqual(emailBody,emailBodyWithDisclaimer);
			//Turn feature on.
			PrefT.UpdateBool(PrefName.EmailDisclaimerIsOn,true);
			//Turn clinics off.
			PrefT.UpdateBool(PrefName.EasyNoClinics,true);
			emailBodyWithDisclaimer=EmailMessages.FindAndReplacePostalAddressTag(emailBody,0);
			//Feature is on so disclaimer added (no clinic).
			Assert.AreNotEqual(emailBody,emailBodyWithDisclaimer);
			Assert.IsTrue(emailBodyWithDisclaimer.EndsWith("subject line."));
			Assert.IsTrue(emailBodyWithDisclaimer.Contains("Practice Address"));
			Assert.IsFalse(emailBodyWithDisclaimer.Contains("Clinic Address"));
			//Turn clinics on.
			PrefT.UpdateBool(PrefName.EasyNoClinics,false);
			emailBodyWithDisclaimer=EmailMessages.FindAndReplacePostalAddressTag(emailBody,clinic.ClinicNum);
			//Feature is on so disclaimer added (with clinic).
			Assert.AreNotEqual(emailBody,emailBodyWithDisclaimer);
			Assert.IsTrue(emailBodyWithDisclaimer.EndsWith("subject line."));
			Assert.IsTrue(emailBodyWithDisclaimer.Contains("Clinic Address"));
			Assert.IsFalse(emailBodyWithDisclaimer.Contains("Practice Address"));
		}

		[TestMethod]
		public void EmailPreviewControl_ReplaceTemplateFields_AreAllReplacementStringsReplaced() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			ReferralT.CreateReferral(pat.PatNum);//LoadTemplate(...) will use this referral.
			RecallT.CreateRecall(pat.PatNum,0,DateTime.Now,new Interval());//LoadTemplate(...) will use this recall.
			string subject="";
			MessageReplaceType typesAll=(MessageReplaceType)Enum.GetValues(typeof(MessageReplaceType)).OfType<MessageReplaceType>().Sum(x => (int)x);
			foreach(FormMessageReplacements.ReplacementField field in FormMessageReplacements.GetReplacementFieldList(true,typesAll)) {
				if(field.IsSupported) {
					subject+=field.FieldName;
				}
			}
			subject=EmailPreviewControl.ReplaceTemplateFields(subject,pat,null,Clinics.GetClinic(pat.ClinicNum));
			Assert.IsFalse(subject.Any(x => x==']' || x=='['));
		}

		///<summary>Ensures the body of an email can be decoded properly when the raw text is given in various formats, as would be seen when sent from
		///different email clients.  Includes html with multi-byte characters to verify these characters are decoded correctly.</summary>
		[TestMethod]
		public void EmailMessages_DecodeBodyText_VariousFormats() {
			List<Tuple<string,bool,string>> listTestSubjects=new List<Tuple<string,bool,string>>() {
				//Raw,Expected
				//Godaddy with =3D typed in the email
				new Tuple<string,bool,string>(@"<html><body><span style=3D""font-family:Verdana; color:#000000; font-size:10pt;""><div><span style=3D"""">test =3D3D more text =3D</span></div></span></body></html>"
					,false,@"<html><body><span style=""font-family:Verdana; color:#000000; font-size:10pt;""><div><span style="""">test =3D more text =</span></div></span></body></html>"),
				//Godaddy with various non-ascii characters.
				new Tuple<string,bool,string>(@"<html><body><span style=3D""font-family:Verdana; color:#000000; font-size:10pt;""><div class=3D""gs"" style=3D""""><div class=3D"""" style=3D""""><div id=3D"":n1"" class=3D""ii gt"" style=3D""""><div id=3D"":n0"" class=3D""a3s aXjCH "" style=3D""""><div dir=3D""ltr"" style=3D"""">chuck nu=C2=A4 =C3=82 =C3=80 =C2=A2<div class=3D""yj6qo"" style=3D""""></div><div class=3D""adL"" style=3D""""><br style=3D""""></div></div><div class=3D""adL"" style=3D""""></div></div></div><div class=3D""hi"" style=3D""""></div></div></div></span></body></html>"
					,false,@"<html><body><span style=""font-family:Verdana; color:#000000; font-size:10pt;""><div class=""gs"" style=""""><div class="""" style=""""><div id="":n1"" class=""ii gt"" style=""""><div id="":n0"" class=""a3s aXjCH "" style=""""><div dir=""ltr"" style="""">chuck nu¤ Â À ¢<div class=""yj6qo"" style=""""></div><div class=""adL"" style=""""><br style=""""></div></div><div class=""adL"" style=""""></div></div></div><div class=""hi"" style=""""></div></div></div></span></body></html>"),
				//Gmail base64
				new Tuple<string,bool,string>(@"<div dir=""ltr"">chuck nu¤ Â À ¢<br></div>",true
					,@"<div dir=""ltr"">chuck nu¤ Â À ¢<br></div>"),
				//Gmail non-base64 with =3D typed in email and a new line at the end.
				new Tuple<string,bool,string>(@"=3D and other things 
",false,@"= and other things 
"),
				//Gmail non-base64 with multiple lines and plain text
				//Gmail non-base64 with =3D typed in email and a new line at the end.
				new Tuple<string,bool,string>(@"plain text
multiple lines

",false,@"plain text
multiple lines

"),
			};
			Encoding encoding=Encoding.GetEncoding("utf-8");
			foreach(Tuple<string,bool,string> test in listTestSubjects) {
				Assert.AreEqual(test.Item3,EmailMessages.DecodeBodyText("=",test.Item1,encoding));
			}
		}

		///<summary>Ensures email subject lines with various formatting and special characters (ascii vs non-ascii) are interpreted correctly.</summary>
		[TestMethod]
		public void EmailMessages_ProcessInlineEncodedText_VariousFormats() {
			List<Tuple<string,string>> listTestSubjects=new List<Tuple<string,string>>() {
				//Raw,Expected
				//UTF-8 Base64 encoded string with non-ascii characters.
				new Tuple<string,string>("=?UTF-8?B?RndkOiDCoiDDhiAxMjM0NSDDpiDDvyBzb21lIGFzY2lpIGNoYXJzIMOCIMOD?=","Fwd: ¢ Æ 12345 æ ÿ some ascii chars Â Ã"),
				//UTF-8 QuotedPrintable encoded string with non-ascii charaters.
				new Tuple<string,string>("=?UTF-8?Q?nu=C2=A4=20=C3=82=20=C3=80=20=C2=A2?=","nu¤ Â À ¢"),
				//UTF-8 QuotedPrintable string embedded within a larger string.
				new Tuple<string,string>("[FWD: test =?UTF-8?Q?=3D=33D=20more=20text=20=3D=5D?=","[FWD: test =3D more text =]"),
				//Plain text string.
				new Tuple<string,string>("regular old plain text subject line","regular old plain text subject line"),
				//Empty string.
				new Tuple<string,string>("",""),
				//Multiple inline UTF-8 QuotedPrintable encoded strings.
				new Tuple<string,string>("=?utf-8?Q?RE:_Half_year_dental_check-up_at?==?utf-8?Q?_Super_&_Clean_Teeth_for_=C3=81_Team?=with regular text in between=?utf-8?Q?_Third_Clause?="
					,"RE: Half year dental check-up at Super & Clean Teeth for Á Teamwith regular text in between Third Clause"),
				//Multiple inline encoded strings with a variety of formats.
				new Tuple<string, string>(@"=?utf-8?Q?encodedtext?= =?iso-8859-1?q?this=20is=20some=20text?=
=?ISO-8859-1?B?SWYgeW91IGNhbiByZWFkIHRoaXMgeW8=?=
    =?ISO-8859-2?B?dSB1bmRlcnN0YW5kIHRoZSBleGFtcGxlLg==?=",@"encodedtext this is some text
If you can read this yo
    u understand the example."),
				//Normal plaintext email address
				new Tuple<string, string>("service@open-dent.com","service@open-dent.com"),
				//Aliased email address, with UTF-8 QuotedPrintable inline encoded string.
				new Tuple<string,string>("=?UTF-8?Q?Bobby_Wiggleh=C3=81rt?= <opendentaltestemail@gmail.com>"
				,"Bobby WigglehÁrt <opendentaltestemail@gmail.com>"),
				//using a 'c' instead of 'B' (Base64) or 'Q' (Quoted Printable)
				new Tuple<string,string>("=?UTF-8?c?nu=C2=A4=20=C3=82=20=C3=80=20=C2=A2?=","nu¤ Â À ¢"),
				//Assert that an email message containing any characters that cannot be represented by a byte do not cause parsing to fail.
				//E.g. the typical hyphen character is '-' but some email will contain '–' which causes an "Arithmetic operation resulted in an overflow." UE.
				//The failure happens when we try to decode the body text of the email for non-ascii characters by casting each char to a byte.
				new Tuple<string,string>("=?UTF-8?Q?hyphen: -  EN Dash: –?=","hyphen: -  EN Dash: –"),
				//Assert that the same email message containing hex characters parses into the human-readable format ('=E2=80=93' equates to '–').
				new Tuple<string,string>("=?UTF-8?Q?=68=79=70=68=65=6E=3A=20=2D=20=20=45=4E=20=44=61=73=68=3A=20=E2=80=93?=","hyphen: -  EN Dash: –"),
				//Assert that Cp1252 (Windows-1252) encoded text can be parsed.
				new Tuple<string,string>("=?Cp1252?Q?Oregon=92s_rain=92s_refreshing?=","Oregon’s rain’s refreshing"),
			};
			foreach(Tuple<string,string> test in listTestSubjects) {
				Assert.AreEqual(test.Item2,EmailMessages.ProcessInlineEncodedText(test.Item1));
			}
		}

		///<summary>Ensures that Email Reply correctly decodes any HTML encoded characters in the response EmailMessage.</summary>
		[TestMethod]
		public void EmailMessages_CreateReply_HtmlEncoding() {
			EmailMessage receivedEmail=EmailMessageT.CreateEmailMessage(0,fromAddress:"opendentaltestemail@gmail.com",toAddress:"opendentalman@gmail.com"
				,recipientAddress:"abc@123.com",subject:"=?UTF-8?Q?nu=C2=A4=20=C3=82=20=C3=80=20=C2=A2?=");
			receivedEmail.RawEmailIn=@"MIME-Version: 1.0
Date: Thu, 10 Oct 2019 06:27:02 -0700
Message-ID: <CAALTEpk8yAUh7pO=FzgCy0r0b20Fi5vefw_8yhRvstMfTvRtAQ@mail.gmail.com>
Subject: & subject
From: =?UTF-8?Q?Bobby_Wiggleh=C3=81rt?= <opendentaltestemail@gmail.com>
To: Bobby Wigglehart <opendentaltestemail@gmail.com>
Content-Type: multipart/alternative; boundary=""0000000000005e7d3705948e5be6""
X-Antivirus: AVG (VPS 191009-2, 10/09/2019), Inbound message
X-Antivirus-Status: Clean

--0000000000005e7d3705948e5be6
Content-Type: text/plain; charset=""UTF-8""

non-breaking space  
less than <
greater than >
ampersand &

--0000000000005e7d3705948e5be6
Content-Type: text/html; charset=""UTF-8""
Content-Transfer-Encoding: quoted-printable

<div dir=3D""ltr"">non-breaking space &nbsp;<div>less than &lt;</div><div>greater than &gt;</div><div>ampersand &amp;=C2=A0</div></div>

--0000000000005e7d3705948e5be6--";
			EmailMessage replyEmail=EmailMessages.CreateReply(receivedEmail,null);
			Assert.AreEqual(receivedEmail.FromAddress,replyEmail.ToAddress);
			Assert.AreEqual(receivedEmail.RecipientAddress,replyEmail.FromAddress);
			Assert.AreEqual("RE: nu¤ Â À ¢",replyEmail.Subject);
			Assert.AreEqual("\r\n\r\n\r\nOn "+DateTime.MinValue.ToString()+" opendentaltestemail@gmail.com sent:\r\n>non-breaking space  less than <greater than >ampersand &"
				,replyEmail.BodyText);
		}

		///<summary>Ensures that Email Reply correctly decodes any HTML encoded characters in the response EmailMessage.</summary>
		[TestMethod]
		public void EmailMessages_CreateForward_HtmlEncoding() {
			EmailMessage receivedEmail=EmailMessageT.CreateEmailMessage(0,fromAddress:"opendentaltestemail@gmail.com",toAddress:"opendentalman@gmail.com"
				,subject:"=?UTF-8?Q?nu=C2=A4=20=C3=82=20=C3=80=20=C2=A2?=");
			receivedEmail.RawEmailIn=@"MIME-Version: 1.0
Date: Thu, 10 Oct 2019 06:27:02 -0700
Message-ID: <CAALTEpk8yAUh7pO=FzgCy0r0b20Fi5vefw_8yhRvstMfTvRtAQ@mail.gmail.com>
Subject: & subject
From: =?UTF-8?Q?Bobby_Wiggleh=C3=81rt?= <opendentaltestemail@gmail.com>
To: Bobby Wigglehart <opendentaltestemail@gmail.com>
Content-Type: multipart/alternative; boundary=""0000000000005e7d3705948e5be6""
X-Antivirus: AVG (VPS 191009-2, 10/09/2019), Inbound message
X-Antivirus-Status: Clean

--0000000000005e7d3705948e5be6
Content-Type: text/plain; charset=""UTF-8""

non-breaking space  
less than <
greater than >
ampersand &

--0000000000005e7d3705948e5be6
Content-Type: text/html; charset=""UTF-8""
Content-Transfer-Encoding: quoted-printable

<div dir=3D""ltr"">non-breaking space &nbsp;<div>less than &lt;</div><div>greater than &gt;</div><div>ampersand &amp;=C2=A0</div></div>

--0000000000005e7d3705948e5be6--";
			EmailAddress emailAddress=new EmailAddress() { EmailUsername="abc@123.com" };
			EmailMessage forwardEmail=EmailMessages.CreateForward(receivedEmail,emailAddress);
			Assert.AreEqual(emailAddress.EmailUsername,forwardEmail.FromAddress);
			Assert.AreEqual("FWD: nu¤ Â À ¢",forwardEmail.Subject);
			Assert.AreEqual("\r\n\r\n\r\nOn "+DateTime.MinValue.ToString()+" opendentaltestemail@gmail.com sent:\r\n>non-breaking space  less than <greater than >ampersand &"
				,forwardEmail.BodyText);
		}

		#region FindAndReplaceImageTagsWithAttachedImage_Tests
		///<summary>A test to ensure the regex is functional in FindAndReplaceImageTagsWithAttachedImage(...) when there is no image in the email
		///but there is HTML tags.</summary>
		[TestMethod]
		public void EmailMessages_FindAndReplaceImageTagsWithAttachedImage_HtmlBodyNoImage() {
			string localHtml=@"<html><head><title>Email Stuff</head><body><p><hr><a href=""http://www.google.com"">Google Link</a>Check out my <i>great</i> link!</p></body></html>";
			bool areImagesDownloaded=false;
			string actualLocalPath=EmailMessages.FindAndReplaceImageTagsWithAttachedImage(localHtml,areImagesDownloaded,out List<string> listLocalImagePaths);
			Assert.AreEqual(actualLocalPath,localHtml);
		}

		///<summary>A test to ensure the regex is functional in FindAndReplaceImageTagsWithAttachedImage(...) when there is only an image in 
		///the email.</summary>
		[TestMethod]
		public void EmailMessages_FindAndReplaceImageTagsWithAttachedImage_OnlyImage() {
			//This unit test might not be needed anymore because it doesn't actually test anything specific.
			//The following HTML is technically invalid (the 'image' doesn't exist anywhere and might be a URL to an image so we just leave it alone).
			string localHtml=@"<html><head><title>Email Stuff</head><body><img src=""image""></img></body></html>";
			bool areImagesDownloaded=false;
			string actualLocalPath=EmailMessages.FindAndReplaceImageTagsWithAttachedImage(localHtml,areImagesDownloaded,out List<string> listLocalImagePaths);
			Assert.AreEqual(localHtml,actualLocalPath);//Verify that no replacement took place.
		}

		///<summary>A test to ensure the regex is functional in FindAndReplaceImageTagsWithAttachedImage(...) when there are multiple images 
		///and HTML tags in the email. This case previously failed when the Regex within the method was <img src=""(.*)""/?></img>.</summary>
		[TestMethod]
		public void EmailMessages_FindAndReplaceImageTagsWithAttachedImage_HtmlBodyWithMultiImages() {
			string tempFile1=PrefC.GetRandomTempFile(".jpg");
			string tempFile2=PrefC.GetRandomTempFile(".jpg");
			string localHtml=$@"<html><head><title>Email Stuff</head><body><p>Text Text Text Text<img src=""{tempFile1}""></img><span></span><img src=""{tempFile2}""></img>Text Text Text Text</p></body></html>";
			bool areImagesDownloaded=false;
			string actualLocalPath=EmailMessages.FindAndReplaceImageTagsWithAttachedImage(localHtml,areImagesDownloaded,out List<string> listLocalImagePaths);
			string expectedLocalPath=$@"<html><head><title>Email Stuff</head><body><p>Text Text Text Text<img src=""cid:{System.IO.Path.GetFileName(tempFile1)}""></img><span></span><img src=""cid:{System.IO.Path.GetFileName(tempFile2)}""></img>Text Text Text Text</p></body></html>";
			Assert.AreEqual(expectedLocalPath,actualLocalPath);
		}

		///<summary>A test to ensure the regex is functional in FindAndReplaceImageTagsWithAttachedImage(...) when there is an image that is closed using
		///only the slash inside the initial image tag instead of a distinct </img> tag.</summary>
		[TestMethod]
		public void EmailMessages_FindAndReplaceImageTagsWithAttachedImage_ClosingSlashInsideImg() {
			string tempFile=PrefC.GetRandomTempFile(".jpg");
			string localHtml=$@"<html><head><title>Email Stuff</head><body><img src=""{tempFile}""/></body></html>";
			bool areImagesDownloaded=false;
			string actualLocalPath=EmailMessages.FindAndReplaceImageTagsWithAttachedImage(localHtml,areImagesDownloaded,out List<string> listLocalImagePaths);
			string expectedLocalPath=$@"<html><head><title>Email Stuff</head><body><img src=""cid:{System.IO.Path.GetFileName(tempFile)}""/></body></html>";
			Assert.AreEqual(expectedLocalPath,actualLocalPath);
		}
		#endregion

		///<summary>Assert that an extra space prior to the day portion of the "Date:" header can be parsed.
		///E.g. typical format is "Sat, 1 Nov 1997 09:55:06 -0600" but some emails come in like "Sat,  1 Nov 1997 09:55:06 -0600"</summary>
		[TestMethod]
		public void EmailMessages_ProcessRawEmailMessageIn_DateExtraSpace() {
			string strRawEmail=@"From: John Doe <jdoe@machine.example>
To: Mary Smith <mary@example.net>
Subject: Saying Hello
Date: Sat,  1 Nov 1997 09:55:06 -0600
Message-ID: <1234@local.machine.example>

This is a message just to say hello.
So, ""Hello"".";
			EmailMessage emailMessage=null;
			try {
				emailMessage=EmailMessages.ProcessRawEmailMessageIn(strRawEmail,0,
					EmailAddressT.CreateEmailAddress(emailUserName: "jdoe@machine.example"),false);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			Assert.AreEqual(new DateTime(1997,11,1,15,55,06),emailMessage.MsgDateTime.ToUniversalTime());//The time "09:55:06 -0600" is "15:55:06" UTC.
		}

		///<summary>Assert that a single digit in the hour portion of the "Date:" header can be parsed.
		///E.g. typical format is "Sat, 1 Nov 1997 09:55:06 -0600" but some emails come in like "Sat, 1 Nov 1997 9:55:06 -0600"</summary>
		[TestMethod]
		public void EmailMessages_ProcessRawEmailMessageIn_DateSingleDigitHour() {
			string strRawEmail=@"From: John Doe <jdoe@machine.example>
To: Mary Smith <mary@example.net>
Subject: Saying Hello
Date: Sat, 1 Nov 1997 9:55:06 -0600
Message-ID: <1234@local.machine.example>

This is a message just to say hello.
So, ""Hello"".";
			EmailMessage emailMessage=null;
			try {
				emailMessage=EmailMessages.ProcessRawEmailMessageIn(strRawEmail,0,
					EmailAddressT.CreateEmailAddress(emailUserName: "jdoe@machine.example"),false);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			Assert.AreEqual(new DateTime(1997,11,1,15,55,06),emailMessage.MsgDateTime.ToUniversalTime());//The time "09:55:06 -0600" is "15:55:06" UTC.
		}

		///<summary>Assert that the abbreviated time zone at the end of the "Date:" header can be parsed.
		///E.g. typical format(s) are is "Thu, 11 Feb 2016 06:58:09 -0600" but some emails come in like "Thu, 11 Feb 2016 06:58:09 CST"
		///NOTE: The abbreviated time zone is not supported by DateTime.ParseExact so CST will not be considered when parsing.</summary>
		[TestMethod]
		public void EmailMessages_ProcessRawEmailMessageIn_DateTimeZoneAbbreviation() {
			string strRawEmail=@"From: John Doe <jdoe@machine.example>
To: Mary Smith <mary@example.net>
Subject: Saying Hello
Date: Thu, 11 Feb 2016 06:58:09 CST
Message-ID: <1234@local.machine.example>

This is a message just to say hello.
So, ""Hello"".";
			EmailMessage emailMessage=null;
			try {
				emailMessage=EmailMessages.ProcessRawEmailMessageIn(strRawEmail,0,
					EmailAddressT.CreateEmailAddress(emailUserName: "jdoe@machine.example"),false);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			//The abbreviated time zone is not supported by DateTime.ParseExact so do not assert the UTC version of emailMessage.MsgDateTime.
			Assert.AreEqual(new DateTime(2016,02,11,06,58,09),emailMessage.MsgDateTime);
		}

		///<summary>Assert that a mime part with a Content-Disposition of "attachment" does not require the "name:" directive.
		///E.g. typical format(s) of the Content-Disposition header are:
		///Content-Disposition: attachment
		///Content-Disposition: attachment; name="fieldName"
		///Content-Disposition: attachment; name="fieldName"; filename="filename.jpg"</summary>
		[TestMethod]
		public void EmailMessages_ProcessRawEmailMessageIn_MimePartAttachmentNoName() {
			string strRawEmail=@"From: John Doe <jdoe@machine.example>
To: Mary Smith <mary@example.net>
Subject: Saying Hello
Date: Thu, 11 Feb 2016 06:58:09 CST
Message-ID: <1234@local.machine.example>
Content-Type: multipart/mixed; boundary=""B1""

This is a multi-part message in MIME format.
--B1
Content-Type: multipart/alternative; boundary=""B2""
MIME-Version: 1.0

--B2
Content-Type: text/plain; charset=us-ascii
Content-Transfer-Encoding: 7bit

Hello from Argentina!

--B2
Content-Type: text/plain; charset=us-ascii
Content-Transfer-Encoding: 7bit

This is a message just to say hello.
So, ""Hello"".

--B2--
--B1
Content-Type: image/gif
Content-Transfer-Encoding: base64
Content-Disposition: attachment; filename=""map_of_Argentina.gif""

R01GOD1hJQA1AKIAAP/////78P/omn19fQAAAAAAAAAAAAAAACwAAAAAJQA1AAAD7Qi63P5w
wEmjBCLrnQnhYCgM1wh+pkgqqeC9XrutmBm7hAK3tP31gFcAiFKVQrGFR6kscnonTe7FAAad
GugmRu3CmiBt57fsVq3Y0VFKnpYdxPC6M7Ze4crnnHum4oN6LFJ1bn5NXTN7OF5fQkN5WYow
BEN2dkGQGWJtSzqGTICJgnQuTJN/WJsojad9qXMuhIWdjXKjY4tenjo6tjVssk2gaWq3uGNX
U6ZGxseyk8SasGw3J9GRzdTQky1iHNvcPNNI4TLeKdfMvy0vMqLrItvuxfDW8ubjueDtJufz
7itICBxISKDBgwgTKjyYAAA7
--B1--";
			EmailMessage emailMessage=null;
			try {
				emailMessage=EmailMessages.ProcessRawEmailMessageIn(strRawEmail,0,
					EmailAddressT.CreateEmailAddress(emailUserName: "jdoe@machine.example"),false);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			Assert.IsNotNull(emailMessage.Attachments);
			Assert.AreEqual(emailMessage.Attachments.Count,1);
		}

		///<summary>Assert that poorly chosen boundaries do not cause the mime parts to fail to parse.
		///E.g. There was an email with several boundaries and they each boundary contained the first boundary:
		///boundary #1 = D775FB8094F7C52EF0C994F5B1152B71
		///boundary #2 = D775FB8094F7C52EF0C994F5B1152B712
		///boundary #3 = D775FB8094F7C52EF0C994F5B1152B713</summary>
		[TestMethod]
		public void EmailMessages_ProcessRawEmailMessageIn_InvalidBoundaries() {
			string strRawEmail=@"From: John Doe <jdoe@machine.example>
To: Mary Smith <mary@example.net>
Subject: Saying Hello
Date: Thu, 11 Feb 2016 06:58:09 CST
Message-ID: <1234@local.machine.example>
Content-Type: multipart/mixed; boundary=""B1""

This is a multi-part message in MIME format.
--B1
Content-Type: multipart/alternative; boundary=""B12""
MIME-Version: 1.0

--B12
Content-Type: text/plain; charset=us-ascii
Content-Transfer-Encoding: 7bit

Hello from Argentina!

--B12
Content-Type: text/plain; charset=us-ascii
Content-Transfer-Encoding: 7bit

This is a message just to say hello.
So, ""Hello"".

--B12--
--B1
Content-Type: image/gif
Content-Transfer-Encoding: base64
Content-Disposition: attachment; filename=""map_of_Argentina.gif""

R01GOD1hJQA1AKIAAP/////78P/omn19fQAAAAAAAAAAAAAAACwAAAAAJQA1AAAD7Qi63P5w
wEmjBCLrnQnhYCgM1wh+pkgqqeC9XrutmBm7hAK3tP31gFcAiFKVQrGFR6kscnonTe7FAAad
GugmRu3CmiBt57fsVq3Y0VFKnpYdxPC6M7Ze4crnnHum4oN6LFJ1bn5NXTN7OF5fQkN5WYow
BEN2dkGQGWJtSzqGTICJgnQuTJN/WJsojad9qXMuhIWdjXKjY4tenjo6tjVssk2gaWq3uGNX
U6ZGxseyk8SasGw3J9GRzdTQky1iHNvcPNNI4TLeKdfMvy0vMqLrItvuxfDW8ubjueDtJufz
7itICBxISKDBgwgTKjyYAAA7
--B1--";
			EmailMessage emailMessage=null;
			try {
				emailMessage=EmailMessages.ProcessRawEmailMessageIn(strRawEmail,0,
					EmailAddressT.CreateEmailAddress(emailUserName: "jdoe@machine.example"),false);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			Assert.IsNotNull(emailMessage.Attachments);
			Assert.AreEqual(emailMessage.Attachments.Count,1);
		}

		///<summary>Assert that the invalid character ';' within the "To:" header does not cause an error.
		///When all recipients are in the bcc field, some clients (gmail) input "undisclosed-recipients:;" into the "To:" header.</summary>
		[TestMethod]
		public void EmailMessages_ProcessRawEmailMessageIn_UndisclosedRecipients() {
			string strRawEmail=@"From: John Doe <jdoe@machine.example>
To: undisclosed-recipients:;
Bcc: Mary Smith <mary@example.net>
Subject: Saying Hello
Date: Sat,  1 Nov 1997 09:55:06 -0600
Message-ID: <1234@local.machine.example>

This is a message just to say hello.
So, ""Hello"".";
			EmailMessage emailMessage=null;
			try {
				emailMessage=EmailMessages.ProcessRawEmailMessageIn(strRawEmail,0,
					EmailAddressT.CreateEmailAddress(emailUserName: "jdoe@machine.example"),false);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			Assert.AreEqual(emailMessage.ToAddress,"");
		}

		///<summary>Assert that the invalid character ';' within the "To:" header does not cause an error.
		///When all recipients are in the bcc field, some clients input "undisclosed recipients:;" into the "To:" header.</summary>
		[TestMethod]
		public void EmailMessages_ProcessRawEmailMessageIn_UndisclosedRecipients_NoHyphen() {
			string strRawEmail=@"From: John Doe <jdoe@machine.example>
To: undisclosed recipients:;
Bcc: Mary Smith <mary@example.net>
Subject: Saying Hello
Date: Sat,  1 Nov 1997 09:55:06 -0600
Message-ID: <1234@local.machine.example>

This is a message just to say hello.
So, ""Hello"".";
			EmailMessage emailMessage=null;
			try {
				emailMessage=EmailMessages.ProcessRawEmailMessageIn(strRawEmail,0,
					EmailAddressT.CreateEmailAddress(emailUserName: "jdoe@machine.example"),false);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			Assert.AreEqual(emailMessage.ToAddress,"");
		}

		///<summary>Assert that the invalid character ';' within the "To:" header does not cause an error.
		///When all recipients are in the bcc field, some clients input "undisclosed-recipients: ;" into the "To:" header.</summary>
		[TestMethod]
		public void EmailMessages_ProcessRawEmailMessageIn_UndisclosedRecipients_ExtraSpace() {
			string strRawEmail=@"From: John Doe <jdoe@machine.example>
To: undisclosed-recipients: ;
Bcc: Mary Smith <mary@example.net>
Subject: Saying Hello
Date: Sat,  1 Nov 1997 09:55:06 -0600
Message-ID: <1234@local.machine.example>

This is a message just to say hello.
So, ""Hello"".";
			EmailMessage emailMessage=null;
			try {
				emailMessage=EmailMessages.ProcessRawEmailMessageIn(strRawEmail,0,
					EmailAddressT.CreateEmailAddress(emailUserName: "jdoe@machine.example"),false);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			Assert.AreEqual(emailMessage.ToAddress,"");
		}

		///<summary>Assert that emails with no recipients can be parsed.
		///Some clients (Apple mail) remove all address fields (To, cc, bcc) from the header.</summary>
		[TestMethod]
		public void EmailMessages_ProcessRawEmailMessageIn_UndisclosedRecipients_NoRecipients() {
			string emailUserName="jdoe@machine.example";
			string strRawEmail=@"From: John Doe <jdoe@machine.example>
Subject: Saying Hello
Date: Sat,  1 Nov 1997 09:55:06 -0600
Message-ID: <1234@local.machine.example>

This is a message just to say hello.
So, ""Hello"".";
			EmailMessage emailMessage=null;
			try {
				emailMessage=EmailMessages.ProcessRawEmailMessageIn(strRawEmail,0,
					EmailAddressT.CreateEmailAddress(emailUserName: emailUserName),false);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			Assert.AreEqual(emailUserName,emailMessage.BccAddress);//We have specific code to fake a BCC with the current emailUserName.
		}

		///<summary>Asserts that the EmailMessages.GetMailboxForAddress() function returns all emails for the given address,
		///including ones sent via an alias (e.g. "Bob Smith Dental <bob.smith@gmail.com>")</summary>
		[TestMethod]
		public void EmailMessages_GetMailboxForAddress_ReturnsEmailsSentFromAlias() {
			string senderAddress=$"{MiscUtils.CreateRandomAlphaNumericString(10)}-sender@opendental.com";
			string emailUserName=$"{MiscUtils.CreateRandomAlphaNumericString(10)}-username@opendental.com";
			EmailAddress testEmail=EmailAddressT.CreateEmailAddress(senderAddress,emailUserName);
			string[] fromAddresses=new string[] { senderAddress, emailUserName, $"Email Alias <{emailUserName}>", $"Email Alias <{senderAddress}>" };
			for(int i=0;i<fromAddresses.Length;i++) {
				EmailMessageT.CreateEmailMessage(
					patNum:0,
					fromAddress:fromAddresses[i],
					toAddress:$"{MiscUtils.CreateRandomAlphaNumericString(10)}-recipient@notopendental.com",
					sentOrReceived:EmailSentOrReceived.Sent,
					msgDateTime:DateTime.Now);
			}
			List<EmailMessage> returnedMessages=EmailMessages.GetMailboxForAddress(testEmail,DateTime.Today,DateTime.Today,MailboxType.Sent);
			Assert.AreEqual(returnedMessages.Count,fromAddresses.Length);
		}

		///<summary>Asserts that the query in EmailMessages.GetMailboxForAddress() includes all columns necessary for EmailMessageCrud.TableToList(),
		///since the query is not a SELECT * in order to limit the amount of data loaded into memory.
		///</summary>
		[TestMethod]
		public void EmailMessages_GetMailboxForAddress_QueryGetsAllColumns() {
			try {
				EmailAddress emailAddress=new EmailAddress { EmailUsername="opendentaltestemail@gmail.com",SenderAddress="opendentaltestemail@gmail.com" };
				EmailMessages.GetMailboxForAddress(emailAddress,DateTime.Today,DateTime.Today,MailboxType.Inbox);
			}
			catch(MySqlException e) {
				Assert.Fail($"A column may not have been included in the query in {nameof(EmailMessages.GetMailboxForAddress)}. {e.Message}");
			}
		}

		///<summary>Ensures that when an image file is in not in OpenDentalImages, but is on another part of the computer (or another computer on the network)
		///it will creates the localPath as expected and thus send normally.</summary>
		[TestMethod]
		public void EmailMessages_FindAndReplaceImageTagsWithAttachedImage_ClientImageSends() {
			string tempFile=PrefC.GetRandomTempFile(".jpg");
			string localPath=@"<html><head><title>Email Stuff</head><body><img src="""+tempFile+@"""/></body></html>";
			string actualLocalPath=EmailMessages.FindAndReplaceImageTagsWithAttachedImage(localPath.ToString(),false,out List<string> listLocalImagePaths);
			string expectedPath=@"<html><head><title>Email Stuff</head><body><img src=""cid:"
				+System.IO.Path.GetFileName(tempFile)
				+@"""/></body></html>";
			Assert.AreEqual(expectedPath,actualLocalPath);
			Assert.IsTrue(ImageStore.TryDeleteFile(tempFile));
		}

		///<summary>Ensures that when an image is in OpenDentImages it creates the localPath as normal.</summary>
		[TestMethod]
		public void EmailMessages_FindAndReplaceImageTagsWithAttachedImage_LocalImageSends() {
			string tempFile=PrefC.GetRandomTempFile(".jpg");
			string filePath=ImageStore.GetEmailImagePath();
			string newFileAndPathName=FileAtoZ.CombinePaths(filePath,System.IO.Path.GetFileName(tempFile));
			FileAtoZ.Copy(tempFile,newFileAndPathName,FileAtoZSourceDestination.AtoZToLocal);
			Assert.IsTrue(FileAtoZ.Exists(newFileAndPathName));
			string localPath=@"<html><head><title>Email Stuff</head><body><img src="""+tempFile+@"""/></body></html>";
			string actualLocalPath=EmailMessages.FindAndReplaceImageTagsWithAttachedImage(localPath.ToString(),false,out List<string> listLocalImagePaths);
			string expectedPath=@"<html><head><title>Email Stuff</head><body><img src=""cid:"
				+System.IO.Path.GetFileName(tempFile)
				+@"""/></body></html>";
			Assert.AreEqual(expectedPath,actualLocalPath);
			Assert.IsTrue(ImageStore.TryDeleteFile(tempFile));
			Assert.IsTrue(ImageStore.TryDeleteFile(newFileAndPathName));
		}

		///<summary>Ensures that when an image file is sourced from the web it creats the localPath as expected and thus sends normally.</summary>
		[TestMethod]
		public void EmailMessages_FindAndReplaceImageTagsWithAttachedImage_WebImageSends() {
			string localHtml=@"<html><head><title>Email Stuff</head><body><img src=""https://www.bob.com/image.jpg""></img></body></html>";
			bool areImagesDownloaded=false;
			string actualLocalPath=EmailMessages.FindAndReplaceImageTagsWithAttachedImage(localHtml,areImagesDownloaded,out List<string> listLocalImagePaths);
			string expectedLocalPath=@"<html><head><title>Email Stuff</head><body><img src=""https://www.bob.com/image.jpg""></img></body></html>";
			Assert.AreEqual(expectedLocalPath,actualLocalPath);
		}

	}
}
