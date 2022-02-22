using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;
using OpenDental;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;

namespace UnitTests.ODtextBox_Tests {
	[TestClass]
	public class ODtextBoxTests : TestBase {

		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//Make sure the SpellCheck preference is enabled.
			PrefT.UpdateBool(PrefName.SpellCheckIsEnabled,true);
		}

		[TestMethod]
		public void ODtextBox_ClearWavyLines_ExcessiveWhitespace() {
			ODtextBox textBox=GetTextBox();
			//Create text that matches exactly what a customer had that was causing problems (see task #1584694 for details).
			StringBuilder strBuilder=new StringBuilder(@"Pt is going to apply to Nursing school, also does wedding hair and makeup

119/66,77 bpm+-");
			//Add 13,259 newlines because reasons.
			for(int i = 0;i<13259;i++) {
				strBuilder.Append("\r\n");
			}
			//Add the footer of the patient note that the customer had... true story.
			strBuilder.Append(@"Pt applying to Nuirsing school. 04/17 MC
BP-108/64
P-71  PS

");
			textBox.Text=strBuilder.ToString();
			//The following line took roughly 3 minutes to run when using the old way of clearing the wavy lines (hasAllLineHeights=true).
			//This unit test is simply here to make sure that the following line runs within a reasonable amount of time.
			//If it takes a long time to run we engineers will get frustrated with it and come investigate (or simply remove the test).
			textBox.ClearWavyLines();
			Assert.IsTrue(true);
		}

		[TestMethod]
		public void ODtextBox_SpellCheck_DifferentLineHeights() {
			//Create a text box of 3 lines of text where the FIRST misspelled word is on the second line of text.
			//Each line of text should have a different font size thus making each line have a different height.
			string rtf=@"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Microsoft Sans Serif;}}
\viewkind4\uc1\pard\f0\fs30 Large Font\fs17\par
Normal Fontz\par
\fs40 Enormous Fontz\fs17\par
}";
			using(ODtextBox textBox=GetTextBox(rtf:rtf)) {
				ODtextBox.SpellCheckResult spellCheckResult=textBox.SpellCheck();
				Assert.AreEqual(spellCheckResult.ListMisspelledWords.Count,2);
				Assert.AreEqual(spellCheckResult.ListVisibleWords.Count,6);
				Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLineRects.Count,3);
				Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines.Count,2);
				Assert.AreEqual(spellCheckResult.WavyLineArea.endCharIndex,37);
				Assert.AreEqual(spellCheckResult.WavyLineArea.startCharIndex,0);
				Assert.AreEqual(spellCheckResult.WavyLineArea.startLineIndex,0);
				//There should be three visible line heights (with line 2 and 3 having font misspelled as fontz) and they should be:
				//Line 1: Large Font (height=25)
				//Line 2: Normal Fontz (height=13)
				//Line 3: Enormous Fontz (height=31)
				List<int> listExpectedLineHeights=new List<int>() { 25, 13, 31 };
				for(int i=0;i<listExpectedLineHeights.Count;i++) {
					Assert.AreEqual(spellCheckResult.ListVisibleLineHeights[i],listExpectedLineHeights[i]);
				}
			}
		}

		[TestMethod]
		public void ODtextBox_SpellCheck_LotsOfText() {
			//Create a text box of 30 lines of misspelled words with lots of text.
			StringBuilder stringBuilder=new StringBuilder();
			for(int i=0;i<30;i++) {
				stringBuilder.AppendLine(LoremIpsum);
			}
			using(ODtextBox textBox=GetTextBox(stringBuilder.ToString())) {
				ODtextBox.SpellCheckResult spellCheckResult=textBox.SpellCheck();
				Assert.AreEqual(spellCheckResult.ListMisspelledWords.Count,161);
				Assert.AreEqual(spellCheckResult.ListVisibleWords.Count,201);
				Assert.AreEqual(spellCheckResult.ListVisibleLineHeights.Count,24);
				Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLineRects.Count,22);
				Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines.Count,154);
				Assert.AreEqual(spellCheckResult.WavyLineArea.endCharIndex,1301);
				Assert.AreEqual(spellCheckResult.WavyLineArea.startCharIndex,0);
				Assert.AreEqual(spellCheckResult.WavyLineArea.startLineIndex,0);
			}
		}

		[TestMethod]
		public void ODtextBox_SpellCheck_LotsOfLines() {
			//Create a text box of 300 lines of misspelled words (not a lot text though).
			StringBuilder stringBuilder=new StringBuilder();
			for(int i=0;i<300;i++) {
				stringBuilder.AppendLine("Misspelt Werdz");
			}
			using(ODtextBox textBox=GetTextBox(stringBuilder.ToString())) {
				ODtextBox.SpellCheckResult spellCheckResult=textBox.SpellCheck();
				//This particular test should always have 46 misspelled words, 46 visible words, 23 visible line heights, 22 wavy line rects, 44 wavy lines.
				Assert.AreEqual(spellCheckResult.ListMisspelledWords.Count,46);
				Assert.AreEqual(spellCheckResult.ListVisibleWords.Count,46);
				Assert.AreEqual(spellCheckResult.ListVisibleLineHeights.Count,23);
				Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLineRects.Count,22);
				Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines.Count,44);
				Assert.AreEqual(spellCheckResult.WavyLineArea.endCharIndex,344);
				Assert.AreEqual(spellCheckResult.WavyLineArea.startCharIndex,0);
				Assert.AreEqual(spellCheckResult.WavyLineArea.startLineIndex,0);
			}
		}

		[TestMethod]
		public void ODtextBox_SpellCheck_LotsOfWhitespace() {
			StringBuilder stringBuilder=new StringBuilder();
			//Create a text box of 300 lines of whitespace (no actual text).
			for(int i=0;i<300;i++) {
				stringBuilder.AppendLine("\t  \t  \t");
			}
			stringBuilder.Append("\r\n\t");//Just because.
			using(ODtextBox textBox=GetTextBox(stringBuilder.ToString())) {
				ODtextBox.SpellCheckResult spellCheckResult=textBox.SpellCheck();
				//This particular test should always have null misspelled words, 0 visible words, 23 visible line heights, 22 wavy line rects, null wavy lines
				Assert.IsNull(spellCheckResult.ListMisspelledWords);
				Assert.AreEqual(spellCheckResult.ListVisibleWords.Count,0);
				Assert.AreEqual(spellCheckResult.ListVisibleLineHeights.Count,23);
				Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLineRects.Count,22);
				Assert.IsNull(spellCheckResult.WavyLineArea.ListWavyLines);
				Assert.AreEqual(spellCheckResult.WavyLineArea.endCharIndex,183);
				Assert.AreEqual(spellCheckResult.WavyLineArea.startCharIndex,0);
				Assert.AreEqual(spellCheckResult.WavyLineArea.startLineIndex,0);
			}
		}

		[TestMethod]
		public void ODtextBox_SpellCheck_SpellingDisabled() {
			string text=@"Misspelt Werdz";
			using(ODtextBox textBox=GetTextBox(text,isSpellCheckEnabled:false)) {
				ODtextBox.SpellCheckResult spellCheckResult=textBox.SpellCheck();
				//This particular test should always have everything be null.
				Assert.IsNull(spellCheckResult.ListMisspelledWords);
				Assert.IsNull(spellCheckResult.ListVisibleWords);
				Assert.IsNull(spellCheckResult.ListVisibleLineHeights);
				Assert.IsNull(spellCheckResult.WavyLineArea);
			}
		}

		[TestMethod]
		public void ODtextBox_SpellCheck_WavyLinesBeginning() {
			//Make a tiny text box (100 pixels wide and 50 pixels tall) and put a decent amount of misspelled text inside.
			using(ODtextBox textBox=GetTextBox(LoremIpsum,100,50)) {
				//Make sure that the beginning of the text box has focus.
				textBox.Select(0,0);
				textBox.ScrollToCaret();
				ODtextBox.SpellCheckResult spellCheckResult=textBox.SpellCheck();
				//This particular test should always have 6 misspelled words, 8 visible words, 5 visible line heights, 3 wavy line rects, 4 wavy lines
				Assert.AreEqual(spellCheckResult.ListMisspelledWords.Count,6);
				//The misspelled words should be exactly "Lorem", "ipsum", "amet", "consectetur", "adipiscing", "elit"
				List<ODtextBox.MatchOD> listMisspelledWords=new List<ODtextBox.MatchOD>() {
					new ODtextBox.MatchOD() { StartCharIndex=0, Value="Lorem" },
					new ODtextBox.MatchOD() { StartCharIndex=6, Value="ipsum" },
					new ODtextBox.MatchOD() { StartCharIndex=22, Value="amet" },
					new ODtextBox.MatchOD() { StartCharIndex=28, Value="consectetur" },
					new ODtextBox.MatchOD() { StartCharIndex=40, Value="adipiscing" },
					new ODtextBox.MatchOD() { StartCharIndex=51, Value="elit" },
				};
				for(int i=0;i<listMisspelledWords.Count;i++) {
					Assert.AreEqual(spellCheckResult.ListMisspelledWords[i].Value,listMisspelledWords[i].Value);
					Assert.AreEqual(spellCheckResult.ListMisspelledWords[i].StartCharIndex,listMisspelledWords[i].StartCharIndex);
				}
				Assert.AreEqual(spellCheckResult.ListVisibleWords.Count,8);
				Assert.AreEqual(spellCheckResult.ListVisibleLineHeights.Count,5);
				Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLineRects.Count,3);
				Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines.Count,4);
				Assert.AreEqual(spellCheckResult.WavyLineArea.endCharIndex,57);
				Assert.AreEqual(spellCheckResult.WavyLineArea.startCharIndex,0);
				Assert.AreEqual(spellCheckResult.WavyLineArea.startLineIndex,0);
			}
		}

		[TestMethod]
		public void ODtextBox_SpellCheck_WavyLinesMiddle() {
			//Make a tiny text box (100 pixels wide and 50 pixels tall) and put a decent amount of misspelled text inside.
			using(ODtextBox textBox=GetTextBox(LoremIpsum,100,50)) {
				//Make sure that the middle of the text box has focus.
				textBox.Select((LoremIpsum.Length / 2),0);
				textBox.ScrollToCaret();
				ODtextBox.SpellCheckResult spellCheckResult=textBox.SpellCheck();
				//This particular test should always have 8 misspelled words, 12 visible words, 5 visible line heights, 4 wavy line rects, 4 wavy lines
				Assert.AreEqual(spellCheckResult.ListMisspelledWords.Count,8);
				//The misspelled words should be exactly "commodo", "consequat", "Duis", "aute", "irure", "reprehenderit", "voluptate", "velit"
				List<ODtextBox.MatchOD> listMisspelledWords=new List<ODtextBox.MatchOD>() {
					new ODtextBox.MatchOD() { StartCharIndex=213, Value="commodo" },
					new ODtextBox.MatchOD() { StartCharIndex=221, Value="consequat" },
					new ODtextBox.MatchOD() { StartCharIndex=232, Value="Duis" },
					new ODtextBox.MatchOD() { StartCharIndex=237, Value="aute" },
					new ODtextBox.MatchOD() { StartCharIndex=242, Value="irure" },
					new ODtextBox.MatchOD() { StartCharIndex=257, Value="reprehenderit" },
					new ODtextBox.MatchOD() { StartCharIndex=274, Value="voluptate" },
					new ODtextBox.MatchOD() { StartCharIndex=284, Value="velit" },
				};
				for(int i=0;i<listMisspelledWords.Count;i++) {
					Assert.AreEqual(spellCheckResult.ListMisspelledWords[i].Value,listMisspelledWords[i].Value);
					Assert.AreEqual(spellCheckResult.ListMisspelledWords[i].StartCharIndex,listMisspelledWords[i].StartCharIndex);
				}
				Assert.AreEqual(spellCheckResult.ListVisibleWords.Count,12);
				Assert.AreEqual(spellCheckResult.ListVisibleLineHeights.Count,5);
				Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLineRects.Count,4);
				Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines.Count,5);
				Assert.AreEqual(spellCheckResult.WavyLineArea.endCharIndex,289);
				Assert.AreEqual(spellCheckResult.WavyLineArea.startCharIndex,210);
				Assert.AreEqual(spellCheckResult.WavyLineArea.startLineIndex,15);
			}
		}

		[TestMethod]
		public void ODtextBox_SpellCheck_WavyLinesEnd() {
			//Make a tiny text box (100 pixels wide and 50 pixels tall) and put a decent amount of misspelled text inside.
			using(ODtextBox textBox=GetTextBox(LoremIpsum,100,50)) {
				//Make sure that the end of the text box has focus.
				textBox.Select((LoremIpsum.Length - 1),0);
				textBox.ScrollToCaret();
				ODtextBox.SpellCheckResult spellCheckResult=textBox.SpellCheck();
				//This particular test should always have 7 misspelled words, 9 visible words, 4 visible line heights, 4 wavy line rects, 4 wavy lines
				Assert.AreEqual(spellCheckResult.ListMisspelledWords.Count,7);
				//The misspelled words should be exactly "culpa", "qui", "officia", "deserunt", "mollit", "anim", "laborum"
				List<ODtextBox.MatchOD> listMisspelledWords=new List<ODtextBox.MatchOD>() {
					new ODtextBox.MatchOD() { StartCharIndex=391, Value="culpa" },
					new ODtextBox.MatchOD() { StartCharIndex=397, Value="qui" },
					new ODtextBox.MatchOD() { StartCharIndex=401, Value="officia" },
					new ODtextBox.MatchOD() { StartCharIndex=409, Value="deserunt" },
					new ODtextBox.MatchOD() { StartCharIndex=418, Value="mollit" },
					new ODtextBox.MatchOD() { StartCharIndex=425, Value="anim" },
					new ODtextBox.MatchOD() { StartCharIndex=437, Value="laborum" },
				};
				for(int i=0;i<listMisspelledWords.Count;i++) {
					Assert.AreEqual(spellCheckResult.ListMisspelledWords[i].Value,listMisspelledWords[i].Value);
					Assert.AreEqual(spellCheckResult.ListMisspelledWords[i].StartCharIndex,listMisspelledWords[i].StartCharIndex);
				}
				Assert.AreEqual(spellCheckResult.ListVisibleWords.Count,9);
				Assert.AreEqual(spellCheckResult.ListVisibleLineHeights.Count,4);
				Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLineRects.Count,4);
				Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines.Count,4);
				Assert.AreEqual(spellCheckResult.WavyLineArea.endCharIndex,444);
				Assert.AreEqual(spellCheckResult.WavyLineArea.startCharIndex,391);
				Assert.AreEqual(spellCheckResult.WavyLineArea.startLineIndex,27);
			}
		}

		[TestMethod]
		public void ODtextBox_SpellCheck_WavyLinesNone() {
			//There should not be a single word misspelled.
			using(ODtextBox textBox=GetTextBox(LoremIpsumEnglish)) {
				ODtextBox.SpellCheckResult spellCheckResult=textBox.SpellCheck();
				//This particular test should always have 0 misspelled words, 159 visible words, 17 visible line heights, 17 wavy line rects, null wavy lines
				Assert.AreEqual(spellCheckResult.ListMisspelledWords.Count,0);
				Assert.AreEqual(spellCheckResult.ListVisibleWords.Count,159);
				Assert.AreEqual(spellCheckResult.ListVisibleLineHeights.Count,18);
				Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLineRects.Count,18);
				Assert.IsNull(spellCheckResult.WavyLineArea.ListWavyLines);
			}
		}

		[TestMethod]
		public void ODtextBox_SpellCheck_WavyLinesSpanMultipleLines() {
			//Make a skinny text box (50 pixels wide and 500 pixels tall) and put a long misspelled word that wraps multiple lines inside.
			string text="verylongmispeltwerdzthatshouldwrapmultiplelinesdown and then anotherreallylongwerdthatismispeltonpurposeaswell";
			using(ODtextBox textBox=GetTextBox(text,50,500)) {
				ODtextBox.SpellCheckResult spellCheckResult=textBox.SpellCheck();
				//This particular test should always have 2 misspelled words, 4 visible words, 14 visible line heights, 14 wavy line rects, 13 wavy lines
				Assert.AreEqual(spellCheckResult.ListMisspelledWords.Count,2);
				Assert.AreEqual(spellCheckResult.ListVisibleWords.Count,4);
				Assert.AreEqual(spellCheckResult.ListVisibleLineHeights.Count,14);
				Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLineRects.Count,14);
				Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines.Count,13);
				Assert.AreEqual(spellCheckResult.WavyLineArea.endCharIndex,109);
				Assert.AreEqual(spellCheckResult.WavyLineArea.startCharIndex,0);
				Assert.AreEqual(spellCheckResult.WavyLineArea.startLineIndex,0);
				//There should be two misspelled words that wrap several times.
				for(int i=1;i<=13;i++) {//There are 13 lines in total.
					//The first word should be broken up onto 7 separate lines.
					if(i<=7) {
						//PointEnd will always be the same for every single line.
						Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines[i-1].PointEnd.X,7);
						Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines[i-1].PointEnd.Y,92);
						//PointStart will change each line by 13 pixels (because each line height is 13 tall).  This would be dynamic if the font changed.
						Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines[i-1].PointStart.X,1);
						Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines[i-1].PointStart.Y,1 + (i * 13));
						//The first 6 wavy lines should have the same amount of points.
						if(i < 7) {
							Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines[i-1].ListPoints.Count,24);
						}
						else {//The last wavy line will have less points because it isn't that long of a wavy line.
							Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines[i-1].ListPoints.Count,4);
						}
					}
					else {//The second word should be broken up onto 6 separate lines.
						//PointEnd will always be the same for every single line.
						Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines[i-1].PointEnd.X,30);
						Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines[i-1].PointEnd.Y,183);
						//PointStart will change each line by 13 pixels (because each line height is 13 tall).  This would be dynamic if the font changed.
						Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines[i-1].PointStart.X,1);
						//Start the Y at 14 due to a line inbetween the two misspelled words (line height of 13 so 13 + 1 = 14).
						Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines[i-1].PointStart.Y,14 + (i * 13));
						//The first 6 wavy lines should have the same amount of points.
						if(i < 13) {
							Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines[i-1].ListPoints.Count,24);
						}
						else {//The last wavy line will have less points because it isn't that long of a wavy line.
							Assert.AreEqual(spellCheckResult.WavyLineArea.ListWavyLines[i-1].ListPoints.Count,14);
						}
					}
				}
			}
		}

		[TestMethod]
		public void ODtextBox_SpellCheck_GetVisibleCharIndices() {
			ODtextBox textBox=GetTextBox("abc"+"\n"+"def"+"\n"+"ghi");
			//Test the Function
			ODtextBox.CharBounds indexes=textBox.GetVisibleCharIndices();
			Assert.AreEqual(0,indexes.StartCharIndex);
			Assert.AreEqual(textBox.TextLength-1,indexes.EndCharIndex);
		}

		private ODtextBox GetTextBox(string text="",int width=300,int height=300,bool multiline=true,bool isSpellCheckEnabled=true,string rtf="") {
			ODtextBox textBox=new ODtextBox();
			if(!string.IsNullOrEmpty(text)) {
				textBox.Text=text;
			}
			if(!string.IsNullOrEmpty(rtf)) {
				textBox.Rtf=rtf;
			}
			textBox.Width=width;
			textBox.Height=height;
			textBox.SpellCheckIsEnabled=isSpellCheckEnabled;
			textBox.Multiline=multiline;
			return textBox;
		}

		///<summary>Gets the Lorem Ipsum in native Latin form.  This will be considered horribly misspelled.</summary>
		private string LoremIpsum {
			get {
				return "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
			}
		}

		///<summary>Gets the Lorem Ipsum in English.  The entire text will be considered spelled correctly.</summary>
		private string LoremIpsumEnglish {
			get {
				return "But I must explain to you how all this mistaken idea of denouncing pleasure and praising pain was born and I will give you a complete account of the system, and expound the actual teachings of the great explorer of the truth, the master-builder of human happiness. No one rejects, dislikes, or avoids pleasure itself, because it is pleasure, but because those who do not know how to pursue pleasure rationally encounter consequences that are extremely painful. Nor again is there anyone who loves or pursues or desires to obtain pain of itself, because it is pain, but because occasionally circumstances occur in which toil and pain can procure him some great pleasure. To take a trivial example, which of us ever undertakes laborious physical exercise, except to obtain some advantage from it? But who has any right to find fault with a man who chooses to enjoy a pleasure that has no annoying consequences, or one who avoids a pain that produces no resultant pleasure?";
			}
		}

	}
}

