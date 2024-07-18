using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDental;
using OpenDentBusiness;
using UnitTestsCore;
using System.IO;
using System.Xml;
using CodeBase;

namespace UnitTests.MarkupEdit_Tests {
	[TestClass]
	public class MarkupEditTests:TestBase {
		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		[TestMethod]
		public void MarkupEdit_ConvertToPlainText_HTML() {
			string htmltags="<h2>Ideas</h2> <h3>Common Passwords file</h3>A file would be &>included that would contain</br> the top 1000 or so";
			string htmlResult="Ideas Common Passwords fileA file would be &>included that would contain the top 1000 or so";
			Assert.AreEqual(htmlResult,MarkupEdit.ConvertToPlainText(htmltags));
		}

		[TestMethod]
		public void MarkupEdit_ConvertToPlainText_HtmlWebChatEndSessionMessage() {
			string htmltags="<b>The session has ended.  You can close this browser window.</b><br><br>You can save this conversation for future reference."
				+"  <a href=\"chathistory.aspx\">Save the chat history</a>.";
			string htmlResult="The session has ended.  You can close this browser window.You can save this conversation for future reference.  Save the chat history.";
			Assert.AreEqual(htmlResult,MarkupEdit.ConvertToPlainText(htmltags));
		}

		[TestMethod]
		public void MarkupEdit_ConvertToPlainText_PageLinks() {
			string PageLinks="This is a title: [[234]] that shouldn't be [There]";
			string pageLinkResult="This is a title:  that shouldn't be [There]";
			Assert.AreEqual(pageLinkResult,MarkupEdit.ConvertToPlainText(PageLinks));
		}

		[TestMethod]
		public void MarkupEdit_ConvertToPlainText_ODMarkupImage() {
			string markupString="[[[img:This]] : test]].";
			string markupStringResult="[This : test]].";
			Assert.AreEqual(markupStringResult,MarkupEdit.ConvertMarkupToPlainText(markupString));
		}

		[TestMethod]
		public void MarkupEdit_ConvertToPlainText_ODMarkupFont() {
			string markupString="[ [[font:timesnewroman|\r\nemail]] |: test]].";
			string markupStringResult="[ \r\nemail |: test]].";
			Assert.AreEqual(markupStringResult,MarkupEdit.ConvertMarkupToPlainText(markupString));
		}

		[TestMethod]
		public void MarkupEdit_ConvertToPlainText_ODMarkupColor() {
			string markupString="[| [[color:red|red\r\nred]] |: ]].";
			string markupStringResult="[| red\r\nred |: ]].";
			Assert.AreEqual(markupStringResult,MarkupEdit.ConvertMarkupToPlainText(markupString));
		}

		[TestMethod]
		public void MarkupEdit_ConvertToPlainText_ODTable() {
			string markupTable="This email contains HTML.\n{|\n!Width=\"100\"|Heading1!!Width=\"100\"|Heading2!!Width=\"100\"|Heading3\n|-\n|one||two||three\n|}";
			string markupTableResult="This email contains HTML.\n\n Heading1 Heading2 Heading3 \n one two three\n";
			Assert.AreEqual(markupTableResult,MarkupEdit.ConvertMarkupToPlainText(markupTable));
		}

		[TestMethod]
		public void MarkupEdit_ProcessList_OrderedList_TrailingCR() {
			string s="<td Width=\"100\"><p>#a<br/>#b<br/>#c<br/></p></td>";
			string result=MarkupEdit.ProcessList(s,"#");
			Assert.AreEqual("<td Width=\"100\"><p><ol><li><span class=\"ListItemContent\">a</span></li><li><span class=\"ListItemContent\">b</span></li><li><span class=\"ListItemContent\">c</span></li></ol><br/></p></td>",result);
		}

		[TestMethod]
		public void MarkupEdit_ProcessList_UnorderedList_TrailingCR() {
			string s="<td Width=\"100\"><p>*a<br/>*b<br/><br/>*c<br/></p></td>";
			string result=MarkupEdit.ProcessList(s,"*");
			Assert.AreEqual("<td Width=\"100\"><p><ul><li><span class=\"ListItemContent\">a</span></li><li><span class=\"ListItemContent\">b</span></li></ul><br/><ul><li><span class=\"ListItemContent\">c</span></li></ul><br/></p></td>",result);
		}
		
		[TestMethod]
		public void MarkupEdit_ProcessList_ComplexContent_OrderedList() {
			string s="<td Width=\"100\"><p>#<a href=\"wiki:link1\">link1</a><br/>{{nbsp}}<span style=\"color:red ;\"> Hello</span><br/>#<a href=\"wiki:link 2\">link 2</a><br/><span style=\"color:blue ;\"> Hello</span><br/>#<span style=\"color:green ;\"> Hello</span><br/><a href=\"wiki:line 3\">line 3</a></p></td>";
			string result=MarkupEdit.ProcessList(s,"#");
			Assert.AreEqual("<td Width=\"100\"><p><ol><li><span class=\"ListItemContent\"><a href=\"wiki:link1\">link1</a></span></li></ol>{{nbsp}}<span style=\"color:red ;\"> Hello</span><ol><li><span class=\"ListItemContent\"><a href=\"wiki:link 2\">link 2</a></span></li></ol><span style=\"color:blue ;\"> Hello</span><ol><li><span class=\"ListItemContent\"><span style=\"color:green ;\"> Hello</span></span></li></ol><br/><a href=\"wiki:line 3\">line 3</a></p></td>",result);
		}

		[TestMethod]
		public void MarkupEdit_ProcessList_ComplexContent_UnorderedList() {
			string s="<td Width=\"100\"><p>*<a href=\"wiki:link1\">link1</a><br/>{{nbsp}}<span style=\"color:red ;\"> Hello</span><br/>*<a href=\"wiki:link 2\">link 2</a><br/><span style=\"color:blue ;\"> Hello</span><br/>*<span style=\"color:green ;\"> Hello</span><br/><a href=\"wiki:line 3\">line 3</a></p></td>";
			string result=MarkupEdit.ProcessList(s,"*");
			Assert.AreEqual("<td Width=\"100\"><p><ul><li><span class=\"ListItemContent\"><a href=\"wiki:link1\">link1</a></span></li></ul>{{nbsp}}<span style=\"color:red ;\"> Hello</span><ul><li><span class=\"ListItemContent\"><a href=\"wiki:link 2\">link 2</a></span></li></ul><span style=\"color:blue ;\"> Hello</span><ul><li><span class=\"ListItemContent\"><span style=\"color:green ;\"> Hello</span></span></li></ul><br/><a href=\"wiki:line 3\">line 3</a></p></td>",result);
		}
		
		[TestMethod]
		public void MarkupEdit_ProcessList_ComplexContent_OrderedList_NewLinesInListItems() {
			string s="<td Width=\"100\"><p>#<a href=\"wiki:link1\">link1</a><br/><br/><br/>{{nbsp}}<br/><br/><br/><span style=\"color:red ;\"> Hello</span><br/>#<a href=\"wiki:link 2\">link 2</a><br/><br/><br/><span style=\"color:blue ;\"> Hello</span><br/>#<span style=\"color:green ;\"> Hello</span><br/><br/><a href=\"wiki:line 3\">line 3</a></p></td>";
			string result=MarkupEdit.ProcessList(s,"#");
			Assert.AreEqual("<td Width=\"100\"><p><ol><li><span class=\"ListItemContent\"><a href=\"wiki:link1\">link1</a></span></li></ol><br/><br/>{{nbsp}}<br/><br/><span style=\"color:red ;\"> Hello</span><ol><li><span class=\"ListItemContent\"><a href=\"wiki:link 2\">link 2</a></span></li></ol><br/><br/><span style=\"color:blue ;\"> Hello</span><ol><li><span class=\"ListItemContent\"><span style=\"color:green ;\"> Hello</span></span></li></ol><br/><br/><a href=\"wiki:line 3\">line 3</a></p></td>",result);
		}

		[TestMethod]
		public void MarkupEdit_ProcessList_ComplexContent_UnorderedList_NewLinesInListItems() {
			string s="<td Width=\"100\"><p>*<a href=\"wiki:link1\">link1</a><br/><br/><br/>{{nbsp}}<span style=\"color:red ;\"> Hello</span><br/>*<a href=\"wiki:link 2\">link 2</a><br/><br/><br/><span style=\"color:blue ;\"> Hello</span><br/>*<span style=\"color:green ;\"> Hello</span><br/><br/><br/><a href=\"wiki:line 3\">line 3</a></p></td>";
			string result=MarkupEdit.ProcessList(s,"*");
			Assert.AreEqual("<td Width=\"100\"><p><ul><li><span class=\"ListItemContent\"><a href=\"wiki:link1\">link1</a></span></li></ul><br/><br/>{{nbsp}}<span style=\"color:red ;\"> Hello</span><ul><li><span class=\"ListItemContent\"><a href=\"wiki:link 2\">link 2</a></span></li></ul><br/><br/><span style=\"color:blue ;\"> Hello</span><ul><li><span class=\"ListItemContent\"><span style=\"color:green ;\"> Hello</span></span></li></ul><br/><br/><br/><a href=\"wiki:line 3\">line 3</a></p></td>",result);
		}

		[TestMethod]
		public void MarkupEdit_ProcessList_LineDoesNotStartWithPrefixChars_UnorderedList() {
			string s="<body>2 * 2 = 4</body>";
			string result=MarkupEdit.ProcessList(s,"*");
			Assert.AreEqual("<body>2 * 2 = 4</body>",result);
		}

		[TestMethod]
		public void MarkupEdit_ProcessList_LineDoesNotStartWithPrefixChars_OrderedList() {
			string s="<body>Math is #fun</body>";
			string result=MarkupEdit.ProcessList(s,"#");
			Assert.AreEqual("<body>Math is #fun</body>",result);
		}

		[TestMethod]
		public void MarkupEdit_ProcessList_LineDoesNotStartWithPrefixChars_UnorderedList_Table() {
			//Create a table with 1 row nad 1 column as the only thing on the wiki page we're processing
			string s="<body><table>\r\n<tr>\r\n<th Width=\"100\"><p>Heading3</p></th>\r\n</tr>\r\n<tr>\r\n<td Width=\"100\"><p>2 * 2 = 4</p></td>\r\n</tr>\r\n</table></body>";
			string result=MarkupEdit.ProcessList(s,"*");
			Assert.AreEqual("<body><table>\r\n<tr>\r\n<th Width=\"100\"><p>Heading3</p></th>\r\n</tr>\r\n<tr>\r\n<td Width=\"100\"><p>2 * 2 = 4</p></td>\n</tr>\r\n</table></body>",result);
			//Only difference should be \n instead of \r\n since that is what we join the lines on so that ProcessParagraph can handle them properly.   ^
		}

		[TestMethod]
		public void MarkupEdit_ProcessList_LineDoesNotStartWithPrefixChars_OrderedList_Table() {
			//Create a table with 1 row nad 1 column as the only thing on the wiki page we're processing
			string s="<body><table>\r\n<tr>\r\n<th Width=\"100\"><p>Heading3</p></th>\r\n</tr>\r\n<tr>\r\n<td Width=\"100\"><p>Math is #fun</p></td>\r\n</tr>\r\n</table></body>";
			string result=MarkupEdit.ProcessList(s,"#");
			Assert.AreEqual("<body><table>\r\n<tr>\r\n<th Width=\"100\"><p>Heading3</p></th>\r\n</tr>\r\n<tr>\r\n<td Width=\"100\"><p>Math is #fun</p></td>\n</tr>\r\n</table></body>",result);
			//Only difference should be \n instead of \r\n since that is what we join the lines on so that ProcessParagraph can handle them properly.       ^
		}

		[TestMethod]
		public void MarkupEdit_ProcessList_InatallHAProxyUbuntuWikiPage() {
			//Markup for the [[Install HA Proxu - Ubuntu]] wiki page before the lists (# and *) have been processed.
			string s="<body><a href=\"wiki:Setting Up a Server With HAProxy\">Setting Up a Server With HAProxy</a>\r\n<h1>Install HAProxy - Ubuntu</h1>\r\nThis page assumes you are sitting down at a brand new Ubuntu Linux server and know the credentials to remote into it with <span style=\"font-family:courier;\">root</span> or <span style=\"font-family:courier;\">administrator</span>.\r\n\r\n<h2>Add a New User</h2>\r\nThere are lots of HAProxy Linux boxes and some even have different distributions of Linux installed and have different default users. Create a <span style=\"font-family:courier;\">devops</span> user so that anyone can sit down at a terminal and log into the server.\r\n#Remote into the new Ubuntu Linux machine with the credentials provided by IT (or whoever set up the server).\r\n#Add a new user called <span style=\"font-family:courier;\">devops</span> if it doesn't already exist:  <span style=\"font-family:courier;\">sudo adduser devops</span>\r\n#Allow this new user to execute <span style=\"font-family:courier;\">sudo</span> commands:  <span style=\"font-family:courier;\">sudo usermod -aG sudo devops</span>\r\n\r\n<h2>Expand LVM Partition</h2>\r\nThe default Ubuntu Server installation utilizes a 100GB LVM (logical volume manager) which is just not enough room most of the time.\r\nRun the following commands to verify that the current server has a small LVM that can expand.\r\n#Look at the Size, Avail, and Use% from the following command:  <span style=\"font-family:courier;\">df -h</span>\r\n#Check the free space on the Volume Group by looking at Free PE / Size from:  <span style=\"font-family:courier;\">sudo vgdisplay</span>\r\n#The following command can be executed to see the current LV size:  <span style=\"font-family:courier;\">sudo lvdisplay</span>\r\n\r\nIf there is a volume of 100GB that is named something similar to <span style=\"font-family:courier;\">/dev/ubuntu-vg/ubuntu-lv</span> and there is free space, then do the following steps to expand the volume. \r\n#Expand the logical volume to use up the rest of the free space:  <span style=\"font-family:courier;\">sudo lvextend -l +100%FREE /dev/ubuntu-vg/ubuntu-lv</span>\r\n#Verify it changed via:  <span style=\"font-family:courier;\">sudo lvdisplay</span>\r\n#Extend the filesystem on top of the logical volume with:  <span style=\"font-family:courier;\">sudo resize2fs /dev/mapper/ubuntu--vg-ubuntu--lv</span>\r\n#Verify it changed via:  <span style=\"font-family:courier;\">df -h</span>\r\n\r\n<h2>Install HAProxy Service</h2>\r\nThere is a way to install very specific versions of HAProxy but it should be fine to just install the lastest LTS package that Ubuntu provides.\r\n#Remote into the new Ubuntu Linux machine with the <span style=\"font-family:courier;\">devops</span> user.\r\n#Install the HAProxy package with the following command:  <span style=\"font-family:courier;\">sudo apt install haproxy</span>\r\n#Verify that HAProxy was installed and started:  <span style=\"font-family:courier;\">sudo haproxy -v</span>\r\n\r\n<h2>Configure HAProxy Service</h2>\r\nThe following file path is the default config file for new installations of HAProxy:  <span style=\"font-family:courier;\">/etc/haproxy/haproxy.cfg</span>\r\n\r\n<h3>Config File</h3>\r\n#Remote into the new Ubuntu Linux machine with the <span style=\"font-family:courier;\">devops</span> user.\r\n#Change directory to the haproxy dir:  <span style=\"font-family:courier;\">cd /etc/haproxy/</span>\r\n#Fill out <span style=\"font-family:courier;\">haproxy.cfg</span> for the service(s) that it will be load balaing. It is often a good idea to look at or copy a config file from another server as a starting point.\r\n#Add the new config file to source control, under <span style=\"font-family:courier;\">...Unversioned/HAProxyConfigs/[ServerName].cfg</span>\r\n\r\n<h3>Backup and Staging</h3>\r\nHQ has a script that likes to make use of a backup and a staging version of the config file. Create them out of the config file that was just created.\r\n#Remote into the new Ubuntu Linux machine with the <span style=\"font-family:courier;\">devops</span> user.\r\n#Change directory to the haproxy dir:  <span style=\"font-family:courier;\">cd /etc/haproxy/</span>\r\n#Create the backup file:  <span style=\"font-family:courier;\">sudo cp haproxy.cfg haproxy.cfg.bak</span>\r\n#Create the staging file:  <span style=\"font-family:courier;\">sudo cp haproxy.cfg haproxy.cfg.staging</span>\r\n\r\n<h3>Open Ports</h3>\r\nUbuntu uses UFW or Uncomplicated Firewall.\r\n#Remote into the new Ubuntu Linux machine with the <span style=\"font-family:courier;\">devops</span> user.\r\n#Check what ports are already open: <span style=\"font-family:courier;\">sudo ufw status verbose</span>\r\n#Allow SSH traffic for telnet and other services:  <span style=\"font-family:courier;\">sudo ufw allow 22</span>\r\n#Allow HTTP service specific traffic:  <span style=\"font-family:courier;\">sudo ufw allow http</span>\r\n#Allow HTTPS service specific traffic:  <span style=\"font-family:courier;\">sudo ufw allow https</span>\r\n#Allow TCP and UDP on any custom port one at a time, replace &lt;port> with the desired port:  <span style=\"font-family:courier;\">sudo ufw allow &lt;port></span>\r\n#Enable UFW:  <span style=\"font-family:courier;\">sudo ufw enable</span>\r\n\r\n<h2>HAProxy Updater Script</h2>\r\nThe script that is used to push config changes from our Git repository to the server needs to be downloaded onto the server within the home directory of the <span style=\"font-family:courier;\">devops</span> user.\r\n\r\n<h3>Personal Access Token</h3>\r\nThe script is stored in our Git repository and can be downloaded from GitLab utilizing a personal access token.\r\n#Navigate to the following URL in order to generate a <span style=\"font-family:courier;\">read_repository</span> personal access token:  <a href=\"https://gitlab.opendental.com:23617/-/profile/personal_access_tokens\">https://gitlab.opendental.com:23617/-/profile/personal_access_tokens</a>\r\n#Enter a temporary <span style=\"font-family:courier;\">Token name</span>.\r\n#Set the <span style=\"font-family:courier;\">Expiration date</span> to tomorrow (it defaults to a month out which is far too long).\r\n#Check the <span style=\"font-family:courier;\">read_repository</span> check box.\r\n#Click <span style=\"font-family:courier;\">Create personal access token</span>.\r\n#Copy the new token from the <span style=\"font-family:courier;\">Your new personal access token</span> box.\r\n\r\n<h3>Download Script</h3>\r\n#Remote into the new Ubuntu Linux machine with the <span style=\"font-family:courier;\">devops</span> user.\r\n#Change directory to the <span style=\"font-family:courier;\">devops</span> home dir if not already there:  <span style=\"font-family:courier;\">cd ~</span>\r\n#Download the script from our GitLab server, replace &lt;AccessToken> with an active personal access token generated above:  <span style=\"font-family:courier;\">wget -O ~/haproxy_updater.sh --header=\"PRIVATE-TOKEN: &lt;AccessToken>\" https://gitlab.opendental.com:23617/api/v4/projects/95/repository/files/HAProxyConfigs%2Fhaproxy_updater_ubuntu.sh/raw?ref=master</span>\r\n\r\n<h3>Execute Script</h3>\r\nExecute the script to make sure it can correctly push new config settings to the HAProxy service.\r\n#Remote into the new Ubuntu Linux machine with the <span style=\"font-family:courier;\">devops</span> user.\r\n#Change directory to the <span style=\"font-family:courier;\">devops</span> home dir if not already there:  <span style=\"font-family:courier;\">cd ~</span>\r\n#Run the script:  <span style=\"font-family:courier;\">sudo sh haproxy_updater.sh</span></body>";
			string result=MarkupEdit.ProcessList(s,"#");
			XmlDocument doc=new XmlDocument();
			using(StringReader reader=new StringReader(s)) {
				doc.Load(reader);//This will throw if there are any errors in the HTML output by ProcessLists
			}
			Assert.IsFalse(result.Contains("#"));
		}

		[TestMethod]
		public void MarkupEdit_ProcessList_NewLinesAtStartOfList() {
			string markup="<body>\n#<a href=\"wiki:Jane Doe\">Jane Doe</a> \n#<a href=\"wiki:John Doe\">John Doe</a>\n</body>";
			string result=MarkupEdit.ProcessList(markup,"#");
			Assert.AreEqual(result,"<body>\n<ol><li><span class=\"ListItemContent\"><a href=\"wiki:Jane Doe\">Jane Doe</a> </span></li>\n<li><span class=\"ListItemContent\"><a href=\"wiki:John Doe\">John Doe</a></span></li></ol>\n</body>");
		}

		[TestMethod]
		public void MarkupEdit_ProcessList_NewLinesAtStartOfList_Table() {
			string markup="<body><table>\n<tr>\n<th Width=\"145\"><p>Heading1</p></th>\n</tr>\n<tr>\n<td Width=\"145\"><p><br/>#<a href=\"wiki:Jane Doe\">Jane Doe</a> <br/>#<a href=\"wiki:John Doe\">John Doe</a></p></td>\n</tr>\n</table></body>";
			string result=MarkupEdit.ProcessList(markup,"#");
			Assert.AreEqual(result,"<body><table>\n<tr>\n<th Width=\"145\"><p>Heading1</p></th>\n</tr>\n<tr>\n<td Width=\"145\"><p><br/><ol><li><span class=\"ListItemContent\"><a href=\"wiki:Jane Doe\">Jane Doe</a> </span></li><li><span class=\"ListItemContent\"><a href=\"wiki:John Doe\">John Doe</a></span></li></ol></p></td>\n</tr>\n</table></body>");
		}

		[TestMethod]
		public void MarkupEdit_ProcessList_MixedListOfOrderedAndUnorderedElements() {
			string markup="<body>\r\n#<a href=\"wiki:Jane Doe\">Jane Doe</a>\r\n*Test\r\n#<a href=\"wiki:John Doe\">John Doe</a>\r\n*Testing\r\n</body>";
			string result=MarkupEdit.ProcessList(markup,"*");
			result=MarkupEdit.ProcessList(result,"#");
			XmlDocument doc=new XmlDocument();
			using(StringReader reader=new StringReader(result)) {
				doc.Load(reader);//This will throw if there are any errors in the HTML output by ProcessLists
			}
			Assert.IsFalse(result.Contains("#"));
			Assert.IsFalse(result.Contains("*"));
		}

		[TestMethod]
		public void MarkupEdit_ProcessList_ContentFollowedByOrderedAndUnorderedElements() {
			string markup="<body><table>\n<tr>\n<th Width=\"800\"><p>Column Header</p></th>\n</tr>\n<tr>\n<td Width=\"800\"><p>This is some cell content.<br/>Within this cell is an ordered list of 3 items<br/>#item one<br/>#item two<br/>#item three<br/>Followed by an unordered list of 3 items<br/>*item one<br/>*item two<br/>*item three</p></td>\n</tr>\n</table></body>";
			string result=MarkupEdit.ProcessList(markup,"*");
			result=MarkupEdit.ProcessList(result,"#");
			Assert.AreEqual(result,"<body><table>\n<tr>\n<th Width=\"800\"><p>Column Header</p></th>\n</tr>\n<tr>\n<td Width=\"800\"><p>This is some cell content.<br/>Within this cell is an ordered list of 3 items<br/><ol><li><span class=\"ListItemContent\">item one</span></li><li><span class=\"ListItemContent\">item two</span></li><li><span class=\"ListItemContent\">item three</span></li></ol>Followed by an unordered list of 3 items<ul><li><span class=\"ListItemContent\">item one</span></li><li><span class=\"ListItemContent\">item two</span></li><li><span class=\"ListItemContent\">item three</span></li></ul></p></td>\n</tr>\n</table></body>");
		}

		[TestMethod]
		public void MarkupEdit_ProcessList_TableContentContainsBrs() {
			string markup="<body><td Width=\"100\"><p>*h<br/><br/>i<br/>j</p></td></body>";
			string result=MarkupEdit.ProcessList(markup,"*");
			Assert.AreEqual("<body><td Width=\"100\"><p><ul><li><span class=\"ListItemContent\">h</span></li></ul><br/><br/>i<br/>j</p></td></body>",result);
		}

		[TestMethod]
		public void MarkupEdit_ReduceTagGroupingsByOne_OrderedAndUnorderedLists() {
			string markup="<ul><li><span class=\"ListItemContent\">h</span></li></ul><br/><br/><ol><li><span class=\"ListItemContent\">i</span></li></ol><br/><ul><li><span class=\"ListItemContent\">j</span></li></ul>";
			string result=MarkupEdit.ReduceTagGroupingsByOne(markup,"<br/>");
			Assert.AreEqual("<ul><li><span class=\"ListItemContent\">h</span></li></ul><br/><ol><li><span class=\"ListItemContent\">i</span></li></ol><ul><li><span class=\"ListItemContent\">j</span></li></ul>",result);
		}
	}
}
