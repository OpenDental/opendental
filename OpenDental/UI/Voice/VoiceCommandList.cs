using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase;

namespace OpenDental.UI.Voice {
	///<summary>A list of all voice commands used in the program.</summary>
	public class VoiceCommandList {
		private static List<VoiceCommand> _commands=new List<VoiceCommand> {
			#region Global
				new VoiceCommand {
					Commands=new List<string> {
						"start listening",
					},
					ActionToPerform=VoiceCommandAction.StartListening,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.Global },
					Response="Listening"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"stop listening",
					},
					ActionToPerform=VoiceCommandAction.StopListening,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.Global },
					Response="No longer listening"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"give feedback",
						"start giving feedback",
						"turn feedback on"
					},
					ActionToPerform=VoiceCommandAction.GiveFeedback,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.Global },
					Response="Giving feedback"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"stop giving feedback",
						"turn feedback off"
					},
					ActionToPerform=VoiceCommandAction.StopGivingFeedback,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.Global },
					Response="No longer giving feedback"
				},
				new VoiceCommand {
					Commands=new List<string> { },
					ActionToPerform=VoiceCommandAction.DidntGetThat,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.Global, VoiceCommandArea.VoiceMsgBox },
					Response="I didn't get that"
				},
			#endregion Global
			#region PerioChart
				new VoiceCommand {
					Commands=new List<string> {
						"add perio exam",
						"new perio exam"
					},
					ActionToPerform=VoiceCommandAction.CreatePerioExam,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Adding perio exam"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero"
					},
					ActionToPerform=VoiceCommandAction.Zero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one"
					},
					ActionToPerform=VoiceCommandAction.One,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two"
					},
					ActionToPerform=VoiceCommandAction.Two,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three"
					},
					ActionToPerform=VoiceCommandAction.Three,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four"
					},
					ActionToPerform=VoiceCommandAction.Four,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five"
					},
					ActionToPerform=VoiceCommandAction.Five,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six"
					},
					ActionToPerform=VoiceCommandAction.Six,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven"
					},
					ActionToPerform=VoiceCommandAction.Seven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight"
					},
					ActionToPerform=VoiceCommandAction.Eight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine"
					},
					ActionToPerform=VoiceCommandAction.Nine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"ten"
					},
					ActionToPerform=VoiceCommandAction.Ten,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eleven"
					},
					ActionToPerform=VoiceCommandAction.Eleven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"twelve"
					},
					ActionToPerform=VoiceCommandAction.Twelve,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"thirteen"
					},
					ActionToPerform=VoiceCommandAction.Thirteen,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"fourteen"
					},
					ActionToPerform=VoiceCommandAction.Fourteen,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"fifteen"
					},
					ActionToPerform=VoiceCommandAction.Fifteen,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"sixteen"
					},
					ActionToPerform=VoiceCommandAction.Sixteen,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seventeen"
					},
					ActionToPerform=VoiceCommandAction.Seventeen,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eightteen"
					},
					ActionToPerform=VoiceCommandAction.Eighteen,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nineteen"
					},
					ActionToPerform=VoiceCommandAction.Nineteen,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				#region  Hard-Coded Triplets and Doubles
				new VoiceCommand {
					Commands=new List<string> {
						"three two three"
					},
					ActionToPerform=VoiceCommandAction.ThreeTwoThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four three four"
					},
					ActionToPerform=VoiceCommandAction.FourThreeFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three three three"
					},
					ActionToPerform=VoiceCommandAction.ThreeThreeThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two two two"
					},
					ActionToPerform=VoiceCommandAction.TwoTwoTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four four four"
					},
					ActionToPerform=VoiceCommandAction.FourFourFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two one two"
					},
					ActionToPerform=VoiceCommandAction.TwoOneTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four three three"
					},
					ActionToPerform=VoiceCommandAction.FourThreeThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three three four"
					},
					ActionToPerform=VoiceCommandAction.ThreeThreeFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two two three"
					},
					ActionToPerform=VoiceCommandAction.TwoTwoThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three two two"
					},
					ActionToPerform=VoiceCommandAction.ThreeTwoTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five four five"
					},
					ActionToPerform=VoiceCommandAction.FiveFourFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five three five"
					},
					ActionToPerform=VoiceCommandAction.FiveThreeFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three three five"
					},
					ActionToPerform=VoiceCommandAction.ThreeThreeFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five three three"
					},
					ActionToPerform=VoiceCommandAction.FiveThreeThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four four five"
					},
					ActionToPerform=VoiceCommandAction.FourFourFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five four four"
					},
					ActionToPerform=VoiceCommandAction.FiveFourFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five five five"
					},
					ActionToPerform=VoiceCommandAction.FiveFiveFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three four three"
					},
					ActionToPerform=VoiceCommandAction.ThreeFourThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four three five"
					},
					ActionToPerform=VoiceCommandAction.FourThreeFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five three four"
					},
					ActionToPerform=VoiceCommandAction.FiveThreeFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero zero zero"
					},
					ActionToPerform=VoiceCommandAction.ZeroZeroZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero zero one"
					},
					ActionToPerform=VoiceCommandAction.ZeroZeroOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero zero two"
					},
					ActionToPerform=VoiceCommandAction.ZeroZeroTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero zero three"
					},
					ActionToPerform=VoiceCommandAction.ZeroZeroThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero zero four"
					},
					ActionToPerform=VoiceCommandAction.ZeroZeroFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero zero five"
					},
					ActionToPerform=VoiceCommandAction.ZeroZeroFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero zero six"
					},
					ActionToPerform=VoiceCommandAction.ZeroZeroSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero zero seven"
					},
					ActionToPerform=VoiceCommandAction.ZeroZeroSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero zero eight"
					},
					ActionToPerform=VoiceCommandAction.ZeroZeroEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero zero nine"
					},
					ActionToPerform=VoiceCommandAction.ZeroZeroNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero one zero"
					},
					ActionToPerform=VoiceCommandAction.ZeroOneZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero one one"
					},
					ActionToPerform=VoiceCommandAction.ZeroOneOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="zero one one"//Otherwise it was pronouncing 1 as 'ohney'
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero one two"
					},
					ActionToPerform=VoiceCommandAction.ZeroOneTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero one three"
					},
					ActionToPerform=VoiceCommandAction.ZeroOneThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero one four"
					},
					ActionToPerform=VoiceCommandAction.ZeroOneFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero one five"
					},
					ActionToPerform=VoiceCommandAction.ZeroOneFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero one six"
					},
					ActionToPerform=VoiceCommandAction.ZeroOneSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero one seven"
					},
					ActionToPerform=VoiceCommandAction.ZeroOneSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero one eight"
					},
					ActionToPerform=VoiceCommandAction.ZeroOneEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero one nine"
					},
					ActionToPerform=VoiceCommandAction.ZeroOneNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero two zero"
					},
					ActionToPerform=VoiceCommandAction.ZeroTwoZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero two one"
					},
					ActionToPerform=VoiceCommandAction.ZeroTwoOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero two two"
					},
					ActionToPerform=VoiceCommandAction.ZeroTwoTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero two three"
					},
					ActionToPerform=VoiceCommandAction.ZeroTwoThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero two four"
					},
					ActionToPerform=VoiceCommandAction.ZeroTwoFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero two five"
					},
					ActionToPerform=VoiceCommandAction.ZeroTwoFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero two six"
					},
					ActionToPerform=VoiceCommandAction.ZeroTwoSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero two seven"
					},
					ActionToPerform=VoiceCommandAction.ZeroTwoSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero two eight"
					},
					ActionToPerform=VoiceCommandAction.ZeroTwoEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero two nine"
					},
					ActionToPerform=VoiceCommandAction.ZeroTwoNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero three zero"
					},
					ActionToPerform=VoiceCommandAction.ZeroThreeZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero three one"
					},
					ActionToPerform=VoiceCommandAction.ZeroThreeOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero three two"
					},
					ActionToPerform=VoiceCommandAction.ZeroThreeTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero three three"
					},
					ActionToPerform=VoiceCommandAction.ZeroThreeThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero three four"
					},
					ActionToPerform=VoiceCommandAction.ZeroThreeFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero three five"
					},
					ActionToPerform=VoiceCommandAction.ZeroThreeFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero three six"
					},
					ActionToPerform=VoiceCommandAction.ZeroThreeSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero three seven"
					},
					ActionToPerform=VoiceCommandAction.ZeroThreeSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero three eight"
					},
					ActionToPerform=VoiceCommandAction.ZeroThreeEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero three nine"
					},
					ActionToPerform=VoiceCommandAction.ZeroThreeNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero four zero"
					},
					ActionToPerform=VoiceCommandAction.ZeroFourZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero four one"
					},
					ActionToPerform=VoiceCommandAction.ZeroFourOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero four two"
					},
					ActionToPerform=VoiceCommandAction.ZeroFourTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero four three"
					},
					ActionToPerform=VoiceCommandAction.ZeroFourThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero four four"
					},
					ActionToPerform=VoiceCommandAction.ZeroFourFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero four five"
					},
					ActionToPerform=VoiceCommandAction.ZeroFourFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero four six"
					},
					ActionToPerform=VoiceCommandAction.ZeroFourSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero four seven"
					},
					ActionToPerform=VoiceCommandAction.ZeroFourSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero four eight"
					},
					ActionToPerform=VoiceCommandAction.ZeroFourEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero four nine"
					},
					ActionToPerform=VoiceCommandAction.ZeroFourNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero five zero"
					},
					ActionToPerform=VoiceCommandAction.ZeroFiveZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero five one"
					},
					ActionToPerform=VoiceCommandAction.ZeroFiveOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero five two"
					},
					ActionToPerform=VoiceCommandAction.ZeroFiveTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero five three"
					},
					ActionToPerform=VoiceCommandAction.ZeroFiveThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero five four"
					},
					ActionToPerform=VoiceCommandAction.ZeroFiveFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero five five"
					},
					ActionToPerform=VoiceCommandAction.ZeroFiveFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero five six"
					},
					ActionToPerform=VoiceCommandAction.ZeroFiveSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero five seven"
					},
					ActionToPerform=VoiceCommandAction.ZeroFiveSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero five eight"
					},
					ActionToPerform=VoiceCommandAction.ZeroFiveEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero five nine"
					},
					ActionToPerform=VoiceCommandAction.ZeroFiveNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero six zero"
					},
					ActionToPerform=VoiceCommandAction.ZeroSixZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero six one"
					},
					ActionToPerform=VoiceCommandAction.ZeroSixOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero six two"
					},
					ActionToPerform=VoiceCommandAction.ZeroSixTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero six three"
					},
					ActionToPerform=VoiceCommandAction.ZeroSixThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero six four"
					},
					ActionToPerform=VoiceCommandAction.ZeroSixFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero six five"
					},
					ActionToPerform=VoiceCommandAction.ZeroSixFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero six six"
					},
					ActionToPerform=VoiceCommandAction.ZeroSixSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero six seven"
					},
					ActionToPerform=VoiceCommandAction.ZeroSixSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero six eight"
					},
					ActionToPerform=VoiceCommandAction.ZeroSixEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero six nine"
					},
					ActionToPerform=VoiceCommandAction.ZeroSixNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero seven zero"
					},
					ActionToPerform=VoiceCommandAction.ZeroSevenZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero seven one"
					},
					ActionToPerform=VoiceCommandAction.ZeroSevenOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero seven two"
					},
					ActionToPerform=VoiceCommandAction.ZeroSevenTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero seven three"
					},
					ActionToPerform=VoiceCommandAction.ZeroSevenThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero seven four"
					},
					ActionToPerform=VoiceCommandAction.ZeroSevenFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero seven five"
					},
					ActionToPerform=VoiceCommandAction.ZeroSevenFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero seven six"
					},
					ActionToPerform=VoiceCommandAction.ZeroSevenSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero seven seven"
					},
					ActionToPerform=VoiceCommandAction.ZeroSevenSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero seven eight"
					},
					ActionToPerform=VoiceCommandAction.ZeroSevenEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero seven nine"
					},
					ActionToPerform=VoiceCommandAction.ZeroSevenNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero eight zero"
					},
					ActionToPerform=VoiceCommandAction.ZeroEightZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero eight one"
					},
					ActionToPerform=VoiceCommandAction.ZeroEightOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero eight two"
					},
					ActionToPerform=VoiceCommandAction.ZeroEightTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero eight three"
					},
					ActionToPerform=VoiceCommandAction.ZeroEightThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero eight four"
					},
					ActionToPerform=VoiceCommandAction.ZeroEightFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero eight five"
					},
					ActionToPerform=VoiceCommandAction.ZeroEightFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero eight six"
					},
					ActionToPerform=VoiceCommandAction.ZeroEightSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero eight seven"
					},
					ActionToPerform=VoiceCommandAction.ZeroEightSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero eight eight"
					},
					ActionToPerform=VoiceCommandAction.ZeroEightEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero eight nine"
					},
					ActionToPerform=VoiceCommandAction.ZeroEightNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero nine zero"
					},
					ActionToPerform=VoiceCommandAction.ZeroNineZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero nine one"
					},
					ActionToPerform=VoiceCommandAction.ZeroNineOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero nine two"
					},
					ActionToPerform=VoiceCommandAction.ZeroNineTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero nine three"
					},
					ActionToPerform=VoiceCommandAction.ZeroNineThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero nine four"
					},
					ActionToPerform=VoiceCommandAction.ZeroNineFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero nine five"
					},
					ActionToPerform=VoiceCommandAction.ZeroNineFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero nine six"
					},
					ActionToPerform=VoiceCommandAction.ZeroNineSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero nine seven"
					},
					ActionToPerform=VoiceCommandAction.ZeroNineSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero nine eight"
					},
					ActionToPerform=VoiceCommandAction.ZeroNineEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero nine nine"
					},
					ActionToPerform=VoiceCommandAction.ZeroNineNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one zero zero"
					},
					ActionToPerform=VoiceCommandAction.OneZeroZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one zero one"
					},
					ActionToPerform=VoiceCommandAction.OneZeroOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one zero two"
					},
					ActionToPerform=VoiceCommandAction.OneZeroTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one zero three"
					},
					ActionToPerform=VoiceCommandAction.OneZeroThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one zero four"
					},
					ActionToPerform=VoiceCommandAction.OneZeroFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one zero five"
					},
					ActionToPerform=VoiceCommandAction.OneZeroFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one zero six"
					},
					ActionToPerform=VoiceCommandAction.OneZeroSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one zero seven"
					},
					ActionToPerform=VoiceCommandAction.OneZeroSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one zero eight"
					},
					ActionToPerform=VoiceCommandAction.OneZeroEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one zero nine"
					},
					ActionToPerform=VoiceCommandAction.OneZeroNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one one zero"
					},
					ActionToPerform=VoiceCommandAction.OneOneZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="one one zero"//Otherwise it was pronouncing 1 as 'ohney'
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one one one"
					},
					ActionToPerform=VoiceCommandAction.OneOneOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one one two"
					},
					ActionToPerform=VoiceCommandAction.OneOneTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one one three"
					},
					ActionToPerform=VoiceCommandAction.OneOneThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="one one three"//Otherwise it was pronouncing 1 as 'ohney'
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one one four"
					},
					ActionToPerform=VoiceCommandAction.OneOneFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="one one four"//Otherwise it was pronouncing 1 as 'ohney'
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one one five"
					},
					ActionToPerform=VoiceCommandAction.OneOneFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="one one five"//Otherwise it was pronouncing 1 as 'ohney'
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one one six"
					},
					ActionToPerform=VoiceCommandAction.OneOneSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="one one six"//Otherwise it was pronouncing 1 as 'ohney'
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one one seven"
					},
					ActionToPerform=VoiceCommandAction.OneOneSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="one one seven"//Otherwise it was pronouncing 1 as 'ohney'
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one one eight"
					},
					ActionToPerform=VoiceCommandAction.OneOneEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="one one eight"//Otherwise it was pronouncing 1 as 'ohney'
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one one nine"
					},
					ActionToPerform=VoiceCommandAction.OneOneNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="one one nine"//Otherwise it was pronouncing 1 as 'ohney'
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one two zero"
					},
					ActionToPerform=VoiceCommandAction.OneTwoZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one two one"
					},
					ActionToPerform=VoiceCommandAction.OneTwoOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one two two"
					},
					ActionToPerform=VoiceCommandAction.OneTwoTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one two three"
					},
					ActionToPerform=VoiceCommandAction.OneTwoThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one two four"
					},
					ActionToPerform=VoiceCommandAction.OneTwoFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one two five"
					},
					ActionToPerform=VoiceCommandAction.OneTwoFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one two six"
					},
					ActionToPerform=VoiceCommandAction.OneTwoSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one two seven"
					},
					ActionToPerform=VoiceCommandAction.OneTwoSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one two eight"
					},
					ActionToPerform=VoiceCommandAction.OneTwoEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one two nine"
					},
					ActionToPerform=VoiceCommandAction.OneTwoNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one three zero"
					},
					ActionToPerform=VoiceCommandAction.OneThreeZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one three one"
					},
					ActionToPerform=VoiceCommandAction.OneThreeOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one three two"
					},
					ActionToPerform=VoiceCommandAction.OneThreeTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one three three"
					},
					ActionToPerform=VoiceCommandAction.OneThreeThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one three four"
					},
					ActionToPerform=VoiceCommandAction.OneThreeFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one three five"
					},
					ActionToPerform=VoiceCommandAction.OneThreeFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one three six"
					},
					ActionToPerform=VoiceCommandAction.OneThreeSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one three seven"
					},
					ActionToPerform=VoiceCommandAction.OneThreeSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one three eight"
					},
					ActionToPerform=VoiceCommandAction.OneThreeEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one three nine"
					},
					ActionToPerform=VoiceCommandAction.OneThreeNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one four zero"
					},
					ActionToPerform=VoiceCommandAction.OneFourZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one four one"
					},
					ActionToPerform=VoiceCommandAction.OneFourOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one four two"
					},
					ActionToPerform=VoiceCommandAction.OneFourTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one four three"
					},
					ActionToPerform=VoiceCommandAction.OneFourThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one four four"
					},
					ActionToPerform=VoiceCommandAction.OneFourFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one four five"
					},
					ActionToPerform=VoiceCommandAction.OneFourFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one four six"
					},
					ActionToPerform=VoiceCommandAction.OneFourSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one four seven"
					},
					ActionToPerform=VoiceCommandAction.OneFourSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one four eight"
					},
					ActionToPerform=VoiceCommandAction.OneFourEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one four nine"
					},
					ActionToPerform=VoiceCommandAction.OneFourNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one five zero"
					},
					ActionToPerform=VoiceCommandAction.OneFiveZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one five one"
					},
					ActionToPerform=VoiceCommandAction.OneFiveOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one five two"
					},
					ActionToPerform=VoiceCommandAction.OneFiveTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one five three"
					},
					ActionToPerform=VoiceCommandAction.OneFiveThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one five four"
					},
					ActionToPerform=VoiceCommandAction.OneFiveFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one five five"
					},
					ActionToPerform=VoiceCommandAction.OneFiveFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one five six"
					},
					ActionToPerform=VoiceCommandAction.OneFiveSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one five seven"
					},
					ActionToPerform=VoiceCommandAction.OneFiveSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one five eight"
					},
					ActionToPerform=VoiceCommandAction.OneFiveEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one five nine"
					},
					ActionToPerform=VoiceCommandAction.OneFiveNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one six zero"
					},
					ActionToPerform=VoiceCommandAction.OneSixZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one six one"
					},
					ActionToPerform=VoiceCommandAction.OneSixOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one six two"
					},
					ActionToPerform=VoiceCommandAction.OneSixTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one six three"
					},
					ActionToPerform=VoiceCommandAction.OneSixThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one six four"
					},
					ActionToPerform=VoiceCommandAction.OneSixFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one six five"
					},
					ActionToPerform=VoiceCommandAction.OneSixFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one six six"
					},
					ActionToPerform=VoiceCommandAction.OneSixSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one six seven"
					},
					ActionToPerform=VoiceCommandAction.OneSixSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one six eight"
					},
					ActionToPerform=VoiceCommandAction.OneSixEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one six nine"
					},
					ActionToPerform=VoiceCommandAction.OneSixNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one seven zero"
					},
					ActionToPerform=VoiceCommandAction.OneSevenZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one seven one"
					},
					ActionToPerform=VoiceCommandAction.OneSevenOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one seven two"
					},
					ActionToPerform=VoiceCommandAction.OneSevenTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one seven three"
					},
					ActionToPerform=VoiceCommandAction.OneSevenThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one seven four"
					},
					ActionToPerform=VoiceCommandAction.OneSevenFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one seven five"
					},
					ActionToPerform=VoiceCommandAction.OneSevenFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one seven six"
					},
					ActionToPerform=VoiceCommandAction.OneSevenSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one seven seven"
					},
					ActionToPerform=VoiceCommandAction.OneSevenSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one seven eight"
					},
					ActionToPerform=VoiceCommandAction.OneSevenEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one seven nine"
					},
					ActionToPerform=VoiceCommandAction.OneSevenNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one eight zero"
					},
					ActionToPerform=VoiceCommandAction.OneEightZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one eight one"
					},
					ActionToPerform=VoiceCommandAction.OneEightOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one eight two"
					},
					ActionToPerform=VoiceCommandAction.OneEightTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one eight three"
					},
					ActionToPerform=VoiceCommandAction.OneEightThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one eight four"
					},
					ActionToPerform=VoiceCommandAction.OneEightFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one eight five"
					},
					ActionToPerform=VoiceCommandAction.OneEightFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one eight six"
					},
					ActionToPerform=VoiceCommandAction.OneEightSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one eight seven"
					},
					ActionToPerform=VoiceCommandAction.OneEightSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one eight eight"
					},
					ActionToPerform=VoiceCommandAction.OneEightEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one eight nine"
					},
					ActionToPerform=VoiceCommandAction.OneEightNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one nine zero"
					},
					ActionToPerform=VoiceCommandAction.OneNineZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one nine one"
					},
					ActionToPerform=VoiceCommandAction.OneNineOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one nine two"
					},
					ActionToPerform=VoiceCommandAction.OneNineTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one nine three"
					},
					ActionToPerform=VoiceCommandAction.OneNineThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one nine four"
					},
					ActionToPerform=VoiceCommandAction.OneNineFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one nine five"
					},
					ActionToPerform=VoiceCommandAction.OneNineFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one nine six"
					},
					ActionToPerform=VoiceCommandAction.OneNineSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one nine seven"
					},
					ActionToPerform=VoiceCommandAction.OneNineSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one nine eight"
					},
					ActionToPerform=VoiceCommandAction.OneNineEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one nine nine"
					},
					ActionToPerform=VoiceCommandAction.OneNineNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two zero zero"
					},
					ActionToPerform=VoiceCommandAction.TwoZeroZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two zero one"
					},
					ActionToPerform=VoiceCommandAction.TwoZeroOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two zero two"
					},
					ActionToPerform=VoiceCommandAction.TwoZeroTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two zero three"
					},
					ActionToPerform=VoiceCommandAction.TwoZeroThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two zero four"
					},
					ActionToPerform=VoiceCommandAction.TwoZeroFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two zero five"
					},
					ActionToPerform=VoiceCommandAction.TwoZeroFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two zero six"
					},
					ActionToPerform=VoiceCommandAction.TwoZeroSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two zero seven"
					},
					ActionToPerform=VoiceCommandAction.TwoZeroSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two zero eight"
					},
					ActionToPerform=VoiceCommandAction.TwoZeroEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two zero nine"
					},
					ActionToPerform=VoiceCommandAction.TwoZeroNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two one zero"
					},
					ActionToPerform=VoiceCommandAction.TwoOneZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two one one"
					},
					ActionToPerform=VoiceCommandAction.TwoOneOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two one three"
					},
					ActionToPerform=VoiceCommandAction.TwoOneThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two one four"
					},
					ActionToPerform=VoiceCommandAction.TwoOneFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two one five"
					},
					ActionToPerform=VoiceCommandAction.TwoOneFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two one six"
					},
					ActionToPerform=VoiceCommandAction.TwoOneSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two one seven"
					},
					ActionToPerform=VoiceCommandAction.TwoOneSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two one eight"
					},
					ActionToPerform=VoiceCommandAction.TwoOneEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two one nine"
					},
					ActionToPerform=VoiceCommandAction.TwoOneNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two two zero"
					},
					ActionToPerform=VoiceCommandAction.TwoTwoZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two two one"
					},
					ActionToPerform=VoiceCommandAction.TwoTwoOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two two four"
					},
					ActionToPerform=VoiceCommandAction.TwoTwoFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two two five"
					},
					ActionToPerform=VoiceCommandAction.TwoTwoFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two two six"
					},
					ActionToPerform=VoiceCommandAction.TwoTwoSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two two seven"
					},
					ActionToPerform=VoiceCommandAction.TwoTwoSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two two eight"
					},
					ActionToPerform=VoiceCommandAction.TwoTwoEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two two nine"
					},
					ActionToPerform=VoiceCommandAction.TwoTwoNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two three zero"
					},
					ActionToPerform=VoiceCommandAction.TwoThreeZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two three one"
					},
					ActionToPerform=VoiceCommandAction.TwoThreeOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two three two"
					},
					ActionToPerform=VoiceCommandAction.TwoThreeTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two three three"
					},
					ActionToPerform=VoiceCommandAction.TwoThreeThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two three four"
					},
					ActionToPerform=VoiceCommandAction.TwoThreeFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two three five"
					},
					ActionToPerform=VoiceCommandAction.TwoThreeFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two three six"
					},
					ActionToPerform=VoiceCommandAction.TwoThreeSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two three seven"
					},
					ActionToPerform=VoiceCommandAction.TwoThreeSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two three eight"
					},
					ActionToPerform=VoiceCommandAction.TwoThreeEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two three nine"
					},
					ActionToPerform=VoiceCommandAction.TwoThreeNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two four zero"
					},
					ActionToPerform=VoiceCommandAction.TwoFourZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two four one"
					},
					ActionToPerform=VoiceCommandAction.TwoFourOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two four two"
					},
					ActionToPerform=VoiceCommandAction.TwoFourTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two four three"
					},
					ActionToPerform=VoiceCommandAction.TwoFourThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two four four"
					},
					ActionToPerform=VoiceCommandAction.TwoFourFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two four five"
					},
					ActionToPerform=VoiceCommandAction.TwoFourFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two four six"
					},
					ActionToPerform=VoiceCommandAction.TwoFourSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two four seven"
					},
					ActionToPerform=VoiceCommandAction.TwoFourSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two four eight"
					},
					ActionToPerform=VoiceCommandAction.TwoFourEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two four nine"
					},
					ActionToPerform=VoiceCommandAction.TwoFourNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two five zero"
					},
					ActionToPerform=VoiceCommandAction.TwoFiveZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two five one"
					},
					ActionToPerform=VoiceCommandAction.TwoFiveOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two five two"
					},
					ActionToPerform=VoiceCommandAction.TwoFiveTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two five three"
					},
					ActionToPerform=VoiceCommandAction.TwoFiveThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two five four"
					},
					ActionToPerform=VoiceCommandAction.TwoFiveFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two five five"
					},
					ActionToPerform=VoiceCommandAction.TwoFiveFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two five six"
					},
					ActionToPerform=VoiceCommandAction.TwoFiveSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two five seven"
					},
					ActionToPerform=VoiceCommandAction.TwoFiveSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two five eight"
					},
					ActionToPerform=VoiceCommandAction.TwoFiveEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two five nine"
					},
					ActionToPerform=VoiceCommandAction.TwoFiveNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two six zero"
					},
					ActionToPerform=VoiceCommandAction.TwoSixZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two six one"
					},
					ActionToPerform=VoiceCommandAction.TwoSixOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two six two"
					},
					ActionToPerform=VoiceCommandAction.TwoSixTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two six three"
					},
					ActionToPerform=VoiceCommandAction.TwoSixThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two six four"
					},
					ActionToPerform=VoiceCommandAction.TwoSixFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two six five"
					},
					ActionToPerform=VoiceCommandAction.TwoSixFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two six six"
					},
					ActionToPerform=VoiceCommandAction.TwoSixSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two six seven"
					},
					ActionToPerform=VoiceCommandAction.TwoSixSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two six eight"
					},
					ActionToPerform=VoiceCommandAction.TwoSixEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two six nine"
					},
					ActionToPerform=VoiceCommandAction.TwoSixNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two seven zero"
					},
					ActionToPerform=VoiceCommandAction.TwoSevenZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two seven one"
					},
					ActionToPerform=VoiceCommandAction.TwoSevenOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two seven two"
					},
					ActionToPerform=VoiceCommandAction.TwoSevenTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two seven three"
					},
					ActionToPerform=VoiceCommandAction.TwoSevenThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two seven four"
					},
					ActionToPerform=VoiceCommandAction.TwoSevenFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two seven five"
					},
					ActionToPerform=VoiceCommandAction.TwoSevenFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two seven six"
					},
					ActionToPerform=VoiceCommandAction.TwoSevenSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two seven seven"
					},
					ActionToPerform=VoiceCommandAction.TwoSevenSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two seven eight"
					},
					ActionToPerform=VoiceCommandAction.TwoSevenEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two seven nine"
					},
					ActionToPerform=VoiceCommandAction.TwoSevenNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two eight zero"
					},
					ActionToPerform=VoiceCommandAction.TwoEightZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two eight one"
					},
					ActionToPerform=VoiceCommandAction.TwoEightOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two eight two"
					},
					ActionToPerform=VoiceCommandAction.TwoEightTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two eight three"
					},
					ActionToPerform=VoiceCommandAction.TwoEightThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two eight four"
					},
					ActionToPerform=VoiceCommandAction.TwoEightFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two eight five"
					},
					ActionToPerform=VoiceCommandAction.TwoEightFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two eight six"
					},
					ActionToPerform=VoiceCommandAction.TwoEightSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two eight seven"
					},
					ActionToPerform=VoiceCommandAction.TwoEightSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two eight eight"
					},
					ActionToPerform=VoiceCommandAction.TwoEightEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two eight nine"
					},
					ActionToPerform=VoiceCommandAction.TwoEightNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two nine zero"
					},
					ActionToPerform=VoiceCommandAction.TwoNineZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two nine one"
					},
					ActionToPerform=VoiceCommandAction.TwoNineOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two nine two"
					},
					ActionToPerform=VoiceCommandAction.TwoNineTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two nine three"
					},
					ActionToPerform=VoiceCommandAction.TwoNineThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two nine four"
					},
					ActionToPerform=VoiceCommandAction.TwoNineFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two nine five"
					},
					ActionToPerform=VoiceCommandAction.TwoNineFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two nine six"
					},
					ActionToPerform=VoiceCommandAction.TwoNineSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two nine seven"
					},
					ActionToPerform=VoiceCommandAction.TwoNineSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two nine eight"
					},
					ActionToPerform=VoiceCommandAction.TwoNineEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two nine nine"
					},
					ActionToPerform=VoiceCommandAction.TwoNineNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three zero zero"
					},
					ActionToPerform=VoiceCommandAction.ThreeZeroZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three zero one"
					},
					ActionToPerform=VoiceCommandAction.ThreeZeroOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three zero two"
					},
					ActionToPerform=VoiceCommandAction.ThreeZeroTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three zero three"
					},
					ActionToPerform=VoiceCommandAction.ThreeZeroThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three zero four"
					},
					ActionToPerform=VoiceCommandAction.ThreeZeroFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three zero five"
					},
					ActionToPerform=VoiceCommandAction.ThreeZeroFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three zero six"
					},
					ActionToPerform=VoiceCommandAction.ThreeZeroSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three zero seven"
					},
					ActionToPerform=VoiceCommandAction.ThreeZeroSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three zero eight"
					},
					ActionToPerform=VoiceCommandAction.ThreeZeroEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three zero nine"
					},
					ActionToPerform=VoiceCommandAction.ThreeZeroNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three one zero"
					},
					ActionToPerform=VoiceCommandAction.ThreeOneZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three one one"
					},
					ActionToPerform=VoiceCommandAction.ThreeOneOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="three one one"//Otherwise it was pronouncing 1 as 'ohney'
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three one two"
					},
					ActionToPerform=VoiceCommandAction.ThreeOneTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three one three"
					},
					ActionToPerform=VoiceCommandAction.ThreeOneThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three one four"
					},
					ActionToPerform=VoiceCommandAction.ThreeOneFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three one five"
					},
					ActionToPerform=VoiceCommandAction.ThreeOneFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three one six"
					},
					ActionToPerform=VoiceCommandAction.ThreeOneSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three one seven"
					},
					ActionToPerform=VoiceCommandAction.ThreeOneSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three one eight"
					},
					ActionToPerform=VoiceCommandAction.ThreeOneEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three one nine"
					},
					ActionToPerform=VoiceCommandAction.ThreeOneNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three two zero"
					},
					ActionToPerform=VoiceCommandAction.ThreeTwoZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three two one"
					},
					ActionToPerform=VoiceCommandAction.ThreeTwoOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three two four"
					},
					ActionToPerform=VoiceCommandAction.ThreeTwoFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three two five"
					},
					ActionToPerform=VoiceCommandAction.ThreeTwoFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three two six"
					},
					ActionToPerform=VoiceCommandAction.ThreeTwoSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three two seven"
					},
					ActionToPerform=VoiceCommandAction.ThreeTwoSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three two eight"
					},
					ActionToPerform=VoiceCommandAction.ThreeTwoEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three two nine"
					},
					ActionToPerform=VoiceCommandAction.ThreeTwoNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three three zero"
					},
					ActionToPerform=VoiceCommandAction.ThreeThreeZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three three one"
					},
					ActionToPerform=VoiceCommandAction.ThreeThreeOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three three two"
					},
					ActionToPerform=VoiceCommandAction.ThreeThreeTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three three six"
					},
					ActionToPerform=VoiceCommandAction.ThreeThreeSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three three seven"
					},
					ActionToPerform=VoiceCommandAction.ThreeThreeSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three three eight"
					},
					ActionToPerform=VoiceCommandAction.ThreeThreeEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three three nine"
					},
					ActionToPerform=VoiceCommandAction.ThreeThreeNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three four zero"
					},
					ActionToPerform=VoiceCommandAction.ThreeFourZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three four one"
					},
					ActionToPerform=VoiceCommandAction.ThreeFourOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three four two"
					},
					ActionToPerform=VoiceCommandAction.ThreeFourTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three four four"
					},
					ActionToPerform=VoiceCommandAction.ThreeFourFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three four five"
					},
					ActionToPerform=VoiceCommandAction.ThreeFourFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three four six"
					},
					ActionToPerform=VoiceCommandAction.ThreeFourSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three four seven"
					},
					ActionToPerform=VoiceCommandAction.ThreeFourSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three four eight"
					},
					ActionToPerform=VoiceCommandAction.ThreeFourEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three four nine"
					},
					ActionToPerform=VoiceCommandAction.ThreeFourNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three five zero"
					},
					ActionToPerform=VoiceCommandAction.ThreeFiveZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three five one"
					},
					ActionToPerform=VoiceCommandAction.ThreeFiveOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three five two"
					},
					ActionToPerform=VoiceCommandAction.ThreeFiveTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three five three"
					},
					ActionToPerform=VoiceCommandAction.ThreeFiveThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three five four"
					},
					ActionToPerform=VoiceCommandAction.ThreeFiveFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three five five"
					},
					ActionToPerform=VoiceCommandAction.ThreeFiveFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three five six"
					},
					ActionToPerform=VoiceCommandAction.ThreeFiveSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three five seven"
					},
					ActionToPerform=VoiceCommandAction.ThreeFiveSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three five eight"
					},
					ActionToPerform=VoiceCommandAction.ThreeFiveEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three five nine"
					},
					ActionToPerform=VoiceCommandAction.ThreeFiveNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three six zero"
					},
					ActionToPerform=VoiceCommandAction.ThreeSixZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three six one"
					},
					ActionToPerform=VoiceCommandAction.ThreeSixOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three six two"
					},
					ActionToPerform=VoiceCommandAction.ThreeSixTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three six three"
					},
					ActionToPerform=VoiceCommandAction.ThreeSixThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three six four"
					},
					ActionToPerform=VoiceCommandAction.ThreeSixFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three six five"
					},
					ActionToPerform=VoiceCommandAction.ThreeSixFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three six six"
					},
					ActionToPerform=VoiceCommandAction.ThreeSixSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three six seven"
					},
					ActionToPerform=VoiceCommandAction.ThreeSixSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three six eight"
					},
					ActionToPerform=VoiceCommandAction.ThreeSixEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three six nine"
					},
					ActionToPerform=VoiceCommandAction.ThreeSixNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three seven zero"
					},
					ActionToPerform=VoiceCommandAction.ThreeSevenZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three seven one"
					},
					ActionToPerform=VoiceCommandAction.ThreeSevenOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three seven two"
					},
					ActionToPerform=VoiceCommandAction.ThreeSevenTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three seven three"
					},
					ActionToPerform=VoiceCommandAction.ThreeSevenThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three seven four"
					},
					ActionToPerform=VoiceCommandAction.ThreeSevenFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three seven five"
					},
					ActionToPerform=VoiceCommandAction.ThreeSevenFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three seven six"
					},
					ActionToPerform=VoiceCommandAction.ThreeSevenSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three seven seven"
					},
					ActionToPerform=VoiceCommandAction.ThreeSevenSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three seven eight"
					},
					ActionToPerform=VoiceCommandAction.ThreeSevenEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three seven nine"
					},
					ActionToPerform=VoiceCommandAction.ThreeSevenNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three eight zero"
					},
					ActionToPerform=VoiceCommandAction.ThreeEightZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three eight one"
					},
					ActionToPerform=VoiceCommandAction.ThreeEightOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three eight two"
					},
					ActionToPerform=VoiceCommandAction.ThreeEightTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three eight three"
					},
					ActionToPerform=VoiceCommandAction.ThreeEightThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three eight four"
					},
					ActionToPerform=VoiceCommandAction.ThreeEightFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three eight five"
					},
					ActionToPerform=VoiceCommandAction.ThreeEightFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three eight six"
					},
					ActionToPerform=VoiceCommandAction.ThreeEightSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three eight seven"
					},
					ActionToPerform=VoiceCommandAction.ThreeEightSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three eight eight"
					},
					ActionToPerform=VoiceCommandAction.ThreeEightEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three eight nine"
					},
					ActionToPerform=VoiceCommandAction.ThreeEightNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three nine zero"
					},
					ActionToPerform=VoiceCommandAction.ThreeNineZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three nine one"
					},
					ActionToPerform=VoiceCommandAction.ThreeNineOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three nine two"
					},
					ActionToPerform=VoiceCommandAction.ThreeNineTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three nine three"
					},
					ActionToPerform=VoiceCommandAction.ThreeNineThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three nine four"
					},
					ActionToPerform=VoiceCommandAction.ThreeNineFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three nine five"
					},
					ActionToPerform=VoiceCommandAction.ThreeNineFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three nine six"
					},
					ActionToPerform=VoiceCommandAction.ThreeNineSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three nine seven"
					},
					ActionToPerform=VoiceCommandAction.ThreeNineSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three nine eight"
					},
					ActionToPerform=VoiceCommandAction.ThreeNineEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three nine nine"
					},
					ActionToPerform=VoiceCommandAction.ThreeNineNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four zero zero"
					},
					ActionToPerform=VoiceCommandAction.FourZeroZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four zero one"
					},
					ActionToPerform=VoiceCommandAction.FourZeroOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four zero two"
					},
					ActionToPerform=VoiceCommandAction.FourZeroTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four zero three"
					},
					ActionToPerform=VoiceCommandAction.FourZeroThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four zero four"
					},
					ActionToPerform=VoiceCommandAction.FourZeroFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four zero five"
					},
					ActionToPerform=VoiceCommandAction.FourZeroFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four zero six"
					},
					ActionToPerform=VoiceCommandAction.FourZeroSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four zero seven"
					},
					ActionToPerform=VoiceCommandAction.FourZeroSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four zero eight"
					},
					ActionToPerform=VoiceCommandAction.FourZeroEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four zero nine"
					},
					ActionToPerform=VoiceCommandAction.FourZeroNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four one zero"
					},
					ActionToPerform=VoiceCommandAction.FourOneZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four one one"
					},
					ActionToPerform=VoiceCommandAction.FourOneOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="four one one"//Otherwise it was pronouncing 1 as 'ohney'
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four one two"
					},
					ActionToPerform=VoiceCommandAction.FourOneTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four one three"
					},
					ActionToPerform=VoiceCommandAction.FourOneThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four one four"
					},
					ActionToPerform=VoiceCommandAction.FourOneFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four one five"
					},
					ActionToPerform=VoiceCommandAction.FourOneFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four one six"
					},
					ActionToPerform=VoiceCommandAction.FourOneSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four one seven"
					},
					ActionToPerform=VoiceCommandAction.FourOneSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four one eight"
					},
					ActionToPerform=VoiceCommandAction.FourOneEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four one nine"
					},
					ActionToPerform=VoiceCommandAction.FourOneNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four two zero"
					},
					ActionToPerform=VoiceCommandAction.FourTwoZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four two one"
					},
					ActionToPerform=VoiceCommandAction.FourTwoOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four two two"
					},
					ActionToPerform=VoiceCommandAction.FourTwoTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four two three"
					},
					ActionToPerform=VoiceCommandAction.FourTwoThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four two four"
					},
					ActionToPerform=VoiceCommandAction.FourTwoFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four two five"
					},
					ActionToPerform=VoiceCommandAction.FourTwoFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four two six"
					},
					ActionToPerform=VoiceCommandAction.FourTwoSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four two seven"
					},
					ActionToPerform=VoiceCommandAction.FourTwoSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four two eight"
					},
					ActionToPerform=VoiceCommandAction.FourTwoEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four two nine"
					},
					ActionToPerform=VoiceCommandAction.FourTwoNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four three zero"
					},
					ActionToPerform=VoiceCommandAction.FourThreeZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four three one"
					},
					ActionToPerform=VoiceCommandAction.FourThreeOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four three two"
					},
					ActionToPerform=VoiceCommandAction.FourThreeTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four three six"
					},
					ActionToPerform=VoiceCommandAction.FourThreeSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four three seven"
					},
					ActionToPerform=VoiceCommandAction.FourThreeSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four three eight"
					},
					ActionToPerform=VoiceCommandAction.FourThreeEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four three nine"
					},
					ActionToPerform=VoiceCommandAction.FourThreeNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four four zero"
					},
					ActionToPerform=VoiceCommandAction.FourFourZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four four one"
					},
					ActionToPerform=VoiceCommandAction.FourFourOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four four two"
					},
					ActionToPerform=VoiceCommandAction.FourFourTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four four three"
					},
					ActionToPerform=VoiceCommandAction.FourFourThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four four six"
					},
					ActionToPerform=VoiceCommandAction.FourFourSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four four seven"
					},
					ActionToPerform=VoiceCommandAction.FourFourSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four four eight"
					},
					ActionToPerform=VoiceCommandAction.FourFourEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four four nine"
					},
					ActionToPerform=VoiceCommandAction.FourFourNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four five zero"
					},
					ActionToPerform=VoiceCommandAction.FourFiveZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four five one"
					},
					ActionToPerform=VoiceCommandAction.FourFiveOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four five two"
					},
					ActionToPerform=VoiceCommandAction.FourFiveTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four five three"
					},
					ActionToPerform=VoiceCommandAction.FourFiveThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four five four"
					},
					ActionToPerform=VoiceCommandAction.FourFiveFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four five five"
					},
					ActionToPerform=VoiceCommandAction.FourFiveFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four five six"
					},
					ActionToPerform=VoiceCommandAction.FourFiveSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four five seven"
					},
					ActionToPerform=VoiceCommandAction.FourFiveSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four five eight"
					},
					ActionToPerform=VoiceCommandAction.FourFiveEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four five nine"
					},
					ActionToPerform=VoiceCommandAction.FourFiveNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four six zero"
					},
					ActionToPerform=VoiceCommandAction.FourSixZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four six one"
					},
					ActionToPerform=VoiceCommandAction.FourSixOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four six two"
					},
					ActionToPerform=VoiceCommandAction.FourSixTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four six three"
					},
					ActionToPerform=VoiceCommandAction.FourSixThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four six four"
					},
					ActionToPerform=VoiceCommandAction.FourSixFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four six five"
					},
					ActionToPerform=VoiceCommandAction.FourSixFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four six six"
					},
					ActionToPerform=VoiceCommandAction.FourSixSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four six seven"
					},
					ActionToPerform=VoiceCommandAction.FourSixSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four six eight"
					},
					ActionToPerform=VoiceCommandAction.FourSixEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four six nine"
					},
					ActionToPerform=VoiceCommandAction.FourSixNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four seven zero"
					},
					ActionToPerform=VoiceCommandAction.FourSevenZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four seven one"
					},
					ActionToPerform=VoiceCommandAction.FourSevenOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four seven two"
					},
					ActionToPerform=VoiceCommandAction.FourSevenTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four seven three"
					},
					ActionToPerform=VoiceCommandAction.FourSevenThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four seven four"
					},
					ActionToPerform=VoiceCommandAction.FourSevenFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four seven five"
					},
					ActionToPerform=VoiceCommandAction.FourSevenFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four seven six"
					},
					ActionToPerform=VoiceCommandAction.FourSevenSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four seven seven"
					},
					ActionToPerform=VoiceCommandAction.FourSevenSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four seven eight"
					},
					ActionToPerform=VoiceCommandAction.FourSevenEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four seven nine"
					},
					ActionToPerform=VoiceCommandAction.FourSevenNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four eight zero"
					},
					ActionToPerform=VoiceCommandAction.FourEightZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four eight one"
					},
					ActionToPerform=VoiceCommandAction.FourEightOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four eight two"
					},
					ActionToPerform=VoiceCommandAction.FourEightTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four eight three"
					},
					ActionToPerform=VoiceCommandAction.FourEightThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four eight four"
					},
					ActionToPerform=VoiceCommandAction.FourEightFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four eight five"
					},
					ActionToPerform=VoiceCommandAction.FourEightFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four eight six"
					},
					ActionToPerform=VoiceCommandAction.FourEightSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four eight seven"
					},
					ActionToPerform=VoiceCommandAction.FourEightSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four eight eight"
					},
					ActionToPerform=VoiceCommandAction.FourEightEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four eight nine"
					},
					ActionToPerform=VoiceCommandAction.FourEightNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four nine zero"
					},
					ActionToPerform=VoiceCommandAction.FourNineZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four nine one"
					},
					ActionToPerform=VoiceCommandAction.FourNineOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four nine two"
					},
					ActionToPerform=VoiceCommandAction.FourNineTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four nine three"
					},
					ActionToPerform=VoiceCommandAction.FourNineThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four nine four"
					},
					ActionToPerform=VoiceCommandAction.FourNineFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four nine five"
					},
					ActionToPerform=VoiceCommandAction.FourNineFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four nine six"
					},
					ActionToPerform=VoiceCommandAction.FourNineSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four nine seven"
					},
					ActionToPerform=VoiceCommandAction.FourNineSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four nine eight"
					},
					ActionToPerform=VoiceCommandAction.FourNineEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four nine nine"
					},
					ActionToPerform=VoiceCommandAction.FourNineNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five zero zero"
					},
					ActionToPerform=VoiceCommandAction.FiveZeroZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five zero one"
					},
					ActionToPerform=VoiceCommandAction.FiveZeroOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five zero two"
					},
					ActionToPerform=VoiceCommandAction.FiveZeroTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five zero three"
					},
					ActionToPerform=VoiceCommandAction.FiveZeroThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five zero four"
					},
					ActionToPerform=VoiceCommandAction.FiveZeroFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five zero five"
					},
					ActionToPerform=VoiceCommandAction.FiveZeroFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five zero six"
					},
					ActionToPerform=VoiceCommandAction.FiveZeroSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five zero seven"
					},
					ActionToPerform=VoiceCommandAction.FiveZeroSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five zero eight"
					},
					ActionToPerform=VoiceCommandAction.FiveZeroEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five zero nine"
					},
					ActionToPerform=VoiceCommandAction.FiveZeroNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five one zero"
					},
					ActionToPerform=VoiceCommandAction.FiveOneZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five one one"
					},
					ActionToPerform=VoiceCommandAction.FiveOneOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="five one one"//Otherwise it was pronouncing 1 as 'ohney'
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five one two"
					},
					ActionToPerform=VoiceCommandAction.FiveOneTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five one three"
					},
					ActionToPerform=VoiceCommandAction.FiveOneThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five one four"
					},
					ActionToPerform=VoiceCommandAction.FiveOneFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five one five"
					},
					ActionToPerform=VoiceCommandAction.FiveOneFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five one six"
					},
					ActionToPerform=VoiceCommandAction.FiveOneSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five one seven"
					},
					ActionToPerform=VoiceCommandAction.FiveOneSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five one eight"
					},
					ActionToPerform=VoiceCommandAction.FiveOneEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five one nine"
					},
					ActionToPerform=VoiceCommandAction.FiveOneNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five two zero"
					},
					ActionToPerform=VoiceCommandAction.FiveTwoZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five two one"
					},
					ActionToPerform=VoiceCommandAction.FiveTwoOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five two two"
					},
					ActionToPerform=VoiceCommandAction.FiveTwoTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five two three"
					},
					ActionToPerform=VoiceCommandAction.FiveTwoThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five two four"
					},
					ActionToPerform=VoiceCommandAction.FiveTwoFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five two five"
					},
					ActionToPerform=VoiceCommandAction.FiveTwoFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five two six"
					},
					ActionToPerform=VoiceCommandAction.FiveTwoSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five two seven"
					},
					ActionToPerform=VoiceCommandAction.FiveTwoSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five two eight"
					},
					ActionToPerform=VoiceCommandAction.FiveTwoEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five two nine"
					},
					ActionToPerform=VoiceCommandAction.FiveTwoNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five three zero"
					},
					ActionToPerform=VoiceCommandAction.FiveThreeZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five three one"
					},
					ActionToPerform=VoiceCommandAction.FiveThreeOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five three two"
					},
					ActionToPerform=VoiceCommandAction.FiveThreeTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five three six"
					},
					ActionToPerform=VoiceCommandAction.FiveThreeSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five three seven"
					},
					ActionToPerform=VoiceCommandAction.FiveThreeSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five three eight"
					},
					ActionToPerform=VoiceCommandAction.FiveThreeEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five three nine"
					},
					ActionToPerform=VoiceCommandAction.FiveThreeNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five four zero"
					},
					ActionToPerform=VoiceCommandAction.FiveFourZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five four one"
					},
					ActionToPerform=VoiceCommandAction.FiveFourOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five four two"
					},
					ActionToPerform=VoiceCommandAction.FiveFourTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five four three"
					},
					ActionToPerform=VoiceCommandAction.FiveFourThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five four six"
					},
					ActionToPerform=VoiceCommandAction.FiveFourSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five four seven"
					},
					ActionToPerform=VoiceCommandAction.FiveFourSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five four eight"
					},
					ActionToPerform=VoiceCommandAction.FiveFourEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five four nine"
					},
					ActionToPerform=VoiceCommandAction.FiveFourNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five five zero"
					},
					ActionToPerform=VoiceCommandAction.FiveFiveZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five five one"
					},
					ActionToPerform=VoiceCommandAction.FiveFiveOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five five two"
					},
					ActionToPerform=VoiceCommandAction.FiveFiveTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five five three"
					},
					ActionToPerform=VoiceCommandAction.FiveFiveThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five five four"
					},
					ActionToPerform=VoiceCommandAction.FiveFiveFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five five six"
					},
					ActionToPerform=VoiceCommandAction.FiveFiveSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five five seven"
					},
					ActionToPerform=VoiceCommandAction.FiveFiveSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five five eight"
					},
					ActionToPerform=VoiceCommandAction.FiveFiveEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five five nine"
					},
					ActionToPerform=VoiceCommandAction.FiveFiveNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five six zero"
					},
					ActionToPerform=VoiceCommandAction.FiveSixZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five six one"
					},
					ActionToPerform=VoiceCommandAction.FiveSixOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five six two"
					},
					ActionToPerform=VoiceCommandAction.FiveSixTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five six three"
					},
					ActionToPerform=VoiceCommandAction.FiveSixThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five six four"
					},
					ActionToPerform=VoiceCommandAction.FiveSixFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five six five"
					},
					ActionToPerform=VoiceCommandAction.FiveSixFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five six six"
					},
					ActionToPerform=VoiceCommandAction.FiveSixSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five six seven"
					},
					ActionToPerform=VoiceCommandAction.FiveSixSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five six eight"
					},
					ActionToPerform=VoiceCommandAction.FiveSixEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five six nine"
					},
					ActionToPerform=VoiceCommandAction.FiveSixNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five seven zero"
					},
					ActionToPerform=VoiceCommandAction.FiveSevenZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five seven one"
					},
					ActionToPerform=VoiceCommandAction.FiveSevenOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five seven two"
					},
					ActionToPerform=VoiceCommandAction.FiveSevenTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five seven three"
					},
					ActionToPerform=VoiceCommandAction.FiveSevenThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five seven four"
					},
					ActionToPerform=VoiceCommandAction.FiveSevenFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five seven five"
					},
					ActionToPerform=VoiceCommandAction.FiveSevenFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five seven six"
					},
					ActionToPerform=VoiceCommandAction.FiveSevenSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five seven seven"
					},
					ActionToPerform=VoiceCommandAction.FiveSevenSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five seven eight"
					},
					ActionToPerform=VoiceCommandAction.FiveSevenEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five seven nine"
					},
					ActionToPerform=VoiceCommandAction.FiveSevenNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five eight zero"
					},
					ActionToPerform=VoiceCommandAction.FiveEightZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five eight one"
					},
					ActionToPerform=VoiceCommandAction.FiveEightOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five eight two"
					},
					ActionToPerform=VoiceCommandAction.FiveEightTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five eight three"
					},
					ActionToPerform=VoiceCommandAction.FiveEightThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five eight four"
					},
					ActionToPerform=VoiceCommandAction.FiveEightFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five eight five"
					},
					ActionToPerform=VoiceCommandAction.FiveEightFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five eight six"
					},
					ActionToPerform=VoiceCommandAction.FiveEightSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five eight seven"
					},
					ActionToPerform=VoiceCommandAction.FiveEightSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five eight eight"
					},
					ActionToPerform=VoiceCommandAction.FiveEightEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five eight nine"
					},
					ActionToPerform=VoiceCommandAction.FiveEightNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five nine zero"
					},
					ActionToPerform=VoiceCommandAction.FiveNineZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five nine one"
					},
					ActionToPerform=VoiceCommandAction.FiveNineOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five nine two"
					},
					ActionToPerform=VoiceCommandAction.FiveNineTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five nine three"
					},
					ActionToPerform=VoiceCommandAction.FiveNineThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five nine four"
					},
					ActionToPerform=VoiceCommandAction.FiveNineFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five nine five"
					},
					ActionToPerform=VoiceCommandAction.FiveNineFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five nine six"
					},
					ActionToPerform=VoiceCommandAction.FiveNineSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five nine seven"
					},
					ActionToPerform=VoiceCommandAction.FiveNineSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five nine eight"
					},
					ActionToPerform=VoiceCommandAction.FiveNineEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five nine nine"
					},
					ActionToPerform=VoiceCommandAction.FiveNineNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six zero zero"
					},
					ActionToPerform=VoiceCommandAction.SixZeroZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six zero one"
					},
					ActionToPerform=VoiceCommandAction.SixZeroOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six zero two"
					},
					ActionToPerform=VoiceCommandAction.SixZeroTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six zero three"
					},
					ActionToPerform=VoiceCommandAction.SixZeroThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six zero four"
					},
					ActionToPerform=VoiceCommandAction.SixZeroFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six zero five"
					},
					ActionToPerform=VoiceCommandAction.SixZeroFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six zero six"
					},
					ActionToPerform=VoiceCommandAction.SixZeroSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six zero seven"
					},
					ActionToPerform=VoiceCommandAction.SixZeroSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six zero eight"
					},
					ActionToPerform=VoiceCommandAction.SixZeroEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six zero nine"
					},
					ActionToPerform=VoiceCommandAction.SixZeroNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six one zero"
					},
					ActionToPerform=VoiceCommandAction.SixOneZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six one one"
					},
					ActionToPerform=VoiceCommandAction.SixOneOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six one two"
					},
					ActionToPerform=VoiceCommandAction.SixOneTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six one three"
					},
					ActionToPerform=VoiceCommandAction.SixOneThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six one four"
					},
					ActionToPerform=VoiceCommandAction.SixOneFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six one five"
					},
					ActionToPerform=VoiceCommandAction.SixOneFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six one six"
					},
					ActionToPerform=VoiceCommandAction.SixOneSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six one seven"
					},
					ActionToPerform=VoiceCommandAction.SixOneSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six one eight"
					},
					ActionToPerform=VoiceCommandAction.SixOneEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six one nine"
					},
					ActionToPerform=VoiceCommandAction.SixOneNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six two zero"
					},
					ActionToPerform=VoiceCommandAction.SixTwoZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six two one"
					},
					ActionToPerform=VoiceCommandAction.SixTwoOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six two two"
					},
					ActionToPerform=VoiceCommandAction.SixTwoTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six two three"
					},
					ActionToPerform=VoiceCommandAction.SixTwoThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six two four"
					},
					ActionToPerform=VoiceCommandAction.SixTwoFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six two five"
					},
					ActionToPerform=VoiceCommandAction.SixTwoFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six two six"
					},
					ActionToPerform=VoiceCommandAction.SixTwoSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six two seven"
					},
					ActionToPerform=VoiceCommandAction.SixTwoSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six two eight"
					},
					ActionToPerform=VoiceCommandAction.SixTwoEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six two nine"
					},
					ActionToPerform=VoiceCommandAction.SixTwoNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six three zero"
					},
					ActionToPerform=VoiceCommandAction.SixThreeZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six three one"
					},
					ActionToPerform=VoiceCommandAction.SixThreeOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six three two"
					},
					ActionToPerform=VoiceCommandAction.SixThreeTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six three three"
					},
					ActionToPerform=VoiceCommandAction.SixThreeThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six three four"
					},
					ActionToPerform=VoiceCommandAction.SixThreeFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six three five"
					},
					ActionToPerform=VoiceCommandAction.SixThreeFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six three six"
					},
					ActionToPerform=VoiceCommandAction.SixThreeSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six three seven"
					},
					ActionToPerform=VoiceCommandAction.SixThreeSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six three eight"
					},
					ActionToPerform=VoiceCommandAction.SixThreeEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six three nine"
					},
					ActionToPerform=VoiceCommandAction.SixThreeNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six four zero"
					},
					ActionToPerform=VoiceCommandAction.SixFourZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six four one"
					},
					ActionToPerform=VoiceCommandAction.SixFourOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six four two"
					},
					ActionToPerform=VoiceCommandAction.SixFourTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six four three"
					},
					ActionToPerform=VoiceCommandAction.SixFourThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six four four"
					},
					ActionToPerform=VoiceCommandAction.SixFourFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six four five"
					},
					ActionToPerform=VoiceCommandAction.SixFourFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six four six"
					},
					ActionToPerform=VoiceCommandAction.SixFourSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six four seven"
					},
					ActionToPerform=VoiceCommandAction.SixFourSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six four eight"
					},
					ActionToPerform=VoiceCommandAction.SixFourEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six four nine"
					},
					ActionToPerform=VoiceCommandAction.SixFourNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six five zero"
					},
					ActionToPerform=VoiceCommandAction.SixFiveZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six five one"
					},
					ActionToPerform=VoiceCommandAction.SixFiveOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six five two"
					},
					ActionToPerform=VoiceCommandAction.SixFiveTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six five three"
					},
					ActionToPerform=VoiceCommandAction.SixFiveThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six five four"
					},
					ActionToPerform=VoiceCommandAction.SixFiveFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six five five"
					},
					ActionToPerform=VoiceCommandAction.SixFiveFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six five six"
					},
					ActionToPerform=VoiceCommandAction.SixFiveSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six five seven"
					},
					ActionToPerform=VoiceCommandAction.SixFiveSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six five eight"
					},
					ActionToPerform=VoiceCommandAction.SixFiveEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six five nine"
					},
					ActionToPerform=VoiceCommandAction.SixFiveNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six six zero"
					},
					ActionToPerform=VoiceCommandAction.SixSixZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six six one"
					},
					ActionToPerform=VoiceCommandAction.SixSixOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six six two"
					},
					ActionToPerform=VoiceCommandAction.SixSixTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six six three"
					},
					ActionToPerform=VoiceCommandAction.SixSixThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six six four"
					},
					ActionToPerform=VoiceCommandAction.SixSixFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six six five"
					},
					ActionToPerform=VoiceCommandAction.SixSixFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six six six"
					},
					ActionToPerform=VoiceCommandAction.SixSixSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six six seven"
					},
					ActionToPerform=VoiceCommandAction.SixSixSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six six eight"
					},
					ActionToPerform=VoiceCommandAction.SixSixEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six six nine"
					},
					ActionToPerform=VoiceCommandAction.SixSixNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six seven zero"
					},
					ActionToPerform=VoiceCommandAction.SixSevenZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six seven one"
					},
					ActionToPerform=VoiceCommandAction.SixSevenOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six seven two"
					},
					ActionToPerform=VoiceCommandAction.SixSevenTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six seven three"
					},
					ActionToPerform=VoiceCommandAction.SixSevenThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six seven four"
					},
					ActionToPerform=VoiceCommandAction.SixSevenFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six seven five"
					},
					ActionToPerform=VoiceCommandAction.SixSevenFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six seven six"
					},
					ActionToPerform=VoiceCommandAction.SixSevenSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six seven seven"
					},
					ActionToPerform=VoiceCommandAction.SixSevenSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six seven eight"
					},
					ActionToPerform=VoiceCommandAction.SixSevenEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six seven nine"
					},
					ActionToPerform=VoiceCommandAction.SixSevenNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six eight zero"
					},
					ActionToPerform=VoiceCommandAction.SixEightZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six eight one"
					},
					ActionToPerform=VoiceCommandAction.SixEightOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six eight two"
					},
					ActionToPerform=VoiceCommandAction.SixEightTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six eight three"
					},
					ActionToPerform=VoiceCommandAction.SixEightThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six eight four"
					},
					ActionToPerform=VoiceCommandAction.SixEightFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six eight five"
					},
					ActionToPerform=VoiceCommandAction.SixEightFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six eight six"
					},
					ActionToPerform=VoiceCommandAction.SixEightSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six eight seven"
					},
					ActionToPerform=VoiceCommandAction.SixEightSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six eight eight"
					},
					ActionToPerform=VoiceCommandAction.SixEightEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six eight nine"
					},
					ActionToPerform=VoiceCommandAction.SixEightNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six nine zero"
					},
					ActionToPerform=VoiceCommandAction.SixNineZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six nine one"
					},
					ActionToPerform=VoiceCommandAction.SixNineOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six nine two"
					},
					ActionToPerform=VoiceCommandAction.SixNineTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six nine three"
					},
					ActionToPerform=VoiceCommandAction.SixNineThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six nine four"
					},
					ActionToPerform=VoiceCommandAction.SixNineFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six nine five"
					},
					ActionToPerform=VoiceCommandAction.SixNineFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six nine six"
					},
					ActionToPerform=VoiceCommandAction.SixNineSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six nine seven"
					},
					ActionToPerform=VoiceCommandAction.SixNineSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six nine eight"
					},
					ActionToPerform=VoiceCommandAction.SixNineEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six nine nine"
					},
					ActionToPerform=VoiceCommandAction.SixNineNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven zero zero"
					},
					ActionToPerform=VoiceCommandAction.SevenZeroZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven zero one"
					},
					ActionToPerform=VoiceCommandAction.SevenZeroOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven zero two"
					},
					ActionToPerform=VoiceCommandAction.SevenZeroTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven zero three"
					},
					ActionToPerform=VoiceCommandAction.SevenZeroThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven zero four"
					},
					ActionToPerform=VoiceCommandAction.SevenZeroFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven zero five"
					},
					ActionToPerform=VoiceCommandAction.SevenZeroFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven zero six"
					},
					ActionToPerform=VoiceCommandAction.SevenZeroSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven zero seven"
					},
					ActionToPerform=VoiceCommandAction.SevenZeroSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven zero eight"
					},
					ActionToPerform=VoiceCommandAction.SevenZeroEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven zero nine"
					},
					ActionToPerform=VoiceCommandAction.SevenZeroNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven one zero"
					},
					ActionToPerform=VoiceCommandAction.SevenOneZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven one one"
					},
					ActionToPerform=VoiceCommandAction.SevenOneOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="seven one one"//Otherwise it was pronouncing 1 as 'ohney'
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven one two"
					},
					ActionToPerform=VoiceCommandAction.SevenOneTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven one three"
					},
					ActionToPerform=VoiceCommandAction.SevenOneThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven one four"
					},
					ActionToPerform=VoiceCommandAction.SevenOneFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven one five"
					},
					ActionToPerform=VoiceCommandAction.SevenOneFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven one six"
					},
					ActionToPerform=VoiceCommandAction.SevenOneSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven one seven"
					},
					ActionToPerform=VoiceCommandAction.SevenOneSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven one eight"
					},
					ActionToPerform=VoiceCommandAction.SevenOneEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven one nine"
					},
					ActionToPerform=VoiceCommandAction.SevenOneNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven two zero"
					},
					ActionToPerform=VoiceCommandAction.SevenTwoZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven two one"
					},
					ActionToPerform=VoiceCommandAction.SevenTwoOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven two two"
					},
					ActionToPerform=VoiceCommandAction.SevenTwoTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven two three"
					},
					ActionToPerform=VoiceCommandAction.SevenTwoThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven two four"
					},
					ActionToPerform=VoiceCommandAction.SevenTwoFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven two five"
					},
					ActionToPerform=VoiceCommandAction.SevenTwoFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven two six"
					},
					ActionToPerform=VoiceCommandAction.SevenTwoSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven two seven"
					},
					ActionToPerform=VoiceCommandAction.SevenTwoSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven two eight"
					},
					ActionToPerform=VoiceCommandAction.SevenTwoEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven two nine"
					},
					ActionToPerform=VoiceCommandAction.SevenTwoNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven three zero"
					},
					ActionToPerform=VoiceCommandAction.SevenThreeZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven three one"
					},
					ActionToPerform=VoiceCommandAction.SevenThreeOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven three two"
					},
					ActionToPerform=VoiceCommandAction.SevenThreeTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven three three"
					},
					ActionToPerform=VoiceCommandAction.SevenThreeThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven three four"
					},
					ActionToPerform=VoiceCommandAction.SevenThreeFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven three five"
					},
					ActionToPerform=VoiceCommandAction.SevenThreeFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven three six"
					},
					ActionToPerform=VoiceCommandAction.SevenThreeSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven three seven"
					},
					ActionToPerform=VoiceCommandAction.SevenThreeSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven three eight"
					},
					ActionToPerform=VoiceCommandAction.SevenThreeEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven three nine"
					},
					ActionToPerform=VoiceCommandAction.SevenThreeNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven four zero"
					},
					ActionToPerform=VoiceCommandAction.SevenFourZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven four one"
					},
					ActionToPerform=VoiceCommandAction.SevenFourOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven four two"
					},
					ActionToPerform=VoiceCommandAction.SevenFourTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven four three"
					},
					ActionToPerform=VoiceCommandAction.SevenFourThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven four four"
					},
					ActionToPerform=VoiceCommandAction.SevenFourFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven four five"
					},
					ActionToPerform=VoiceCommandAction.SevenFourFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven four six"
					},
					ActionToPerform=VoiceCommandAction.SevenFourSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven four seven"
					},
					ActionToPerform=VoiceCommandAction.SevenFourSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven four eight"
					},
					ActionToPerform=VoiceCommandAction.SevenFourEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven four nine"
					},
					ActionToPerform=VoiceCommandAction.SevenFourNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven five zero"
					},
					ActionToPerform=VoiceCommandAction.SevenFiveZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven five one"
					},
					ActionToPerform=VoiceCommandAction.SevenFiveOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven five two"
					},
					ActionToPerform=VoiceCommandAction.SevenFiveTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven five three"
					},
					ActionToPerform=VoiceCommandAction.SevenFiveThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven five four"
					},
					ActionToPerform=VoiceCommandAction.SevenFiveFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven five five"
					},
					ActionToPerform=VoiceCommandAction.SevenFiveFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven five six"
					},
					ActionToPerform=VoiceCommandAction.SevenFiveSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven five seven"
					},
					ActionToPerform=VoiceCommandAction.SevenFiveSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven five eight"
					},
					ActionToPerform=VoiceCommandAction.SevenFiveEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven five nine"
					},
					ActionToPerform=VoiceCommandAction.SevenFiveNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven six zero"
					},
					ActionToPerform=VoiceCommandAction.SevenSixZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven six one"
					},
					ActionToPerform=VoiceCommandAction.SevenSixOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven six two"
					},
					ActionToPerform=VoiceCommandAction.SevenSixTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven six three"
					},
					ActionToPerform=VoiceCommandAction.SevenSixThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven six four"
					},
					ActionToPerform=VoiceCommandAction.SevenSixFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven six five"
					},
					ActionToPerform=VoiceCommandAction.SevenSixFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven six six"
					},
					ActionToPerform=VoiceCommandAction.SevenSixSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven six seven"
					},
					ActionToPerform=VoiceCommandAction.SevenSixSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven six eight"
					},
					ActionToPerform=VoiceCommandAction.SevenSixEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven six nine"
					},
					ActionToPerform=VoiceCommandAction.SevenSixNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven seven zero"
					},
					ActionToPerform=VoiceCommandAction.SevenSevenZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven seven one"
					},
					ActionToPerform=VoiceCommandAction.SevenSevenOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven seven two"
					},
					ActionToPerform=VoiceCommandAction.SevenSevenTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven seven three"
					},
					ActionToPerform=VoiceCommandAction.SevenSevenThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven seven four"
					},
					ActionToPerform=VoiceCommandAction.SevenSevenFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven seven five"
					},
					ActionToPerform=VoiceCommandAction.SevenSevenFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven seven six"
					},
					ActionToPerform=VoiceCommandAction.SevenSevenSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven seven seven"
					},
					ActionToPerform=VoiceCommandAction.SevenSevenSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven seven eight"
					},
					ActionToPerform=VoiceCommandAction.SevenSevenEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven seven nine"
					},
					ActionToPerform=VoiceCommandAction.SevenSevenNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven eight zero"
					},
					ActionToPerform=VoiceCommandAction.SevenEightZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven eight one"
					},
					ActionToPerform=VoiceCommandAction.SevenEightOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven eight two"
					},
					ActionToPerform=VoiceCommandAction.SevenEightTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven eight three"
					},
					ActionToPerform=VoiceCommandAction.SevenEightThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven eight four"
					},
					ActionToPerform=VoiceCommandAction.SevenEightFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven eight five"
					},
					ActionToPerform=VoiceCommandAction.SevenEightFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven eight six"
					},
					ActionToPerform=VoiceCommandAction.SevenEightSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven eight seven"
					},
					ActionToPerform=VoiceCommandAction.SevenEightSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven eight eight"
					},
					ActionToPerform=VoiceCommandAction.SevenEightEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven eight nine"
					},
					ActionToPerform=VoiceCommandAction.SevenEightNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven nine zero"
					},
					ActionToPerform=VoiceCommandAction.SevenNineZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven nine one"
					},
					ActionToPerform=VoiceCommandAction.SevenNineOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven nine two"
					},
					ActionToPerform=VoiceCommandAction.SevenNineTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven nine three"
					},
					ActionToPerform=VoiceCommandAction.SevenNineThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven nine four"
					},
					ActionToPerform=VoiceCommandAction.SevenNineFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven nine five"
					},
					ActionToPerform=VoiceCommandAction.SevenNineFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven nine six"
					},
					ActionToPerform=VoiceCommandAction.SevenNineSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven nine seven"
					},
					ActionToPerform=VoiceCommandAction.SevenNineSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven nine eight"
					},
					ActionToPerform=VoiceCommandAction.SevenNineEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven nine nine"
					},
					ActionToPerform=VoiceCommandAction.SevenNineNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight zero zero"
					},
					ActionToPerform=VoiceCommandAction.EightZeroZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight zero one"
					},
					ActionToPerform=VoiceCommandAction.EightZeroOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight zero two"
					},
					ActionToPerform=VoiceCommandAction.EightZeroTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight zero three"
					},
					ActionToPerform=VoiceCommandAction.EightZeroThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight zero four"
					},
					ActionToPerform=VoiceCommandAction.EightZeroFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight zero five"
					},
					ActionToPerform=VoiceCommandAction.EightZeroFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight zero six"
					},
					ActionToPerform=VoiceCommandAction.EightZeroSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight zero seven"
					},
					ActionToPerform=VoiceCommandAction.EightZeroSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight zero eight"
					},
					ActionToPerform=VoiceCommandAction.EightZeroEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight zero nine"
					},
					ActionToPerform=VoiceCommandAction.EightZeroNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight one zero"
					},
					ActionToPerform=VoiceCommandAction.EightOneZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight one one"
					},
					ActionToPerform=VoiceCommandAction.EightOneOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="eight one one"//Otherwise it was pronouncing 1 as 'ohney'
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight one two"
					},
					ActionToPerform=VoiceCommandAction.EightOneTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight one three"
					},
					ActionToPerform=VoiceCommandAction.EightOneThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight one four"
					},
					ActionToPerform=VoiceCommandAction.EightOneFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight one five"
					},
					ActionToPerform=VoiceCommandAction.EightOneFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight one six"
					},
					ActionToPerform=VoiceCommandAction.EightOneSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight one seven"
					},
					ActionToPerform=VoiceCommandAction.EightOneSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight one eight"
					},
					ActionToPerform=VoiceCommandAction.EightOneEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight one nine"
					},
					ActionToPerform=VoiceCommandAction.EightOneNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight two zero"
					},
					ActionToPerform=VoiceCommandAction.EightTwoZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight two one"
					},
					ActionToPerform=VoiceCommandAction.EightTwoOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight two two"
					},
					ActionToPerform=VoiceCommandAction.EightTwoTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight two three"
					},
					ActionToPerform=VoiceCommandAction.EightTwoThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight two four"
					},
					ActionToPerform=VoiceCommandAction.EightTwoFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight two five"
					},
					ActionToPerform=VoiceCommandAction.EightTwoFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight two six"
					},
					ActionToPerform=VoiceCommandAction.EightTwoSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight two seven"
					},
					ActionToPerform=VoiceCommandAction.EightTwoSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight two eight"
					},
					ActionToPerform=VoiceCommandAction.EightTwoEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight two nine"
					},
					ActionToPerform=VoiceCommandAction.EightTwoNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight three zero"
					},
					ActionToPerform=VoiceCommandAction.EightThreeZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight three one"
					},
					ActionToPerform=VoiceCommandAction.EightThreeOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight three two"
					},
					ActionToPerform=VoiceCommandAction.EightThreeTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight three three"
					},
					ActionToPerform=VoiceCommandAction.EightThreeThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight three four"
					},
					ActionToPerform=VoiceCommandAction.EightThreeFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight three five"
					},
					ActionToPerform=VoiceCommandAction.EightThreeFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight three six"
					},
					ActionToPerform=VoiceCommandAction.EightThreeSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight three seven"
					},
					ActionToPerform=VoiceCommandAction.EightThreeSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight three eight"
					},
					ActionToPerform=VoiceCommandAction.EightThreeEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight three nine"
					},
					ActionToPerform=VoiceCommandAction.EightThreeNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight four zero"
					},
					ActionToPerform=VoiceCommandAction.EightFourZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight four one"
					},
					ActionToPerform=VoiceCommandAction.EightFourOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight four two"
					},
					ActionToPerform=VoiceCommandAction.EightFourTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight four three"
					},
					ActionToPerform=VoiceCommandAction.EightFourThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight four four"
					},
					ActionToPerform=VoiceCommandAction.EightFourFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight four five"
					},
					ActionToPerform=VoiceCommandAction.EightFourFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight four six"
					},
					ActionToPerform=VoiceCommandAction.EightFourSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight four seven"
					},
					ActionToPerform=VoiceCommandAction.EightFourSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight four eight"
					},
					ActionToPerform=VoiceCommandAction.EightFourEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight four nine"
					},
					ActionToPerform=VoiceCommandAction.EightFourNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight five zero"
					},
					ActionToPerform=VoiceCommandAction.EightFiveZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight five one"
					},
					ActionToPerform=VoiceCommandAction.EightFiveOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight five two"
					},
					ActionToPerform=VoiceCommandAction.EightFiveTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight five three"
					},
					ActionToPerform=VoiceCommandAction.EightFiveThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight five four"
					},
					ActionToPerform=VoiceCommandAction.EightFiveFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight five five"
					},
					ActionToPerform=VoiceCommandAction.EightFiveFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight five six"
					},
					ActionToPerform=VoiceCommandAction.EightFiveSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight five seven"
					},
					ActionToPerform=VoiceCommandAction.EightFiveSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight five eight"
					},
					ActionToPerform=VoiceCommandAction.EightFiveEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight five nine"
					},
					ActionToPerform=VoiceCommandAction.EightFiveNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight six zero"
					},
					ActionToPerform=VoiceCommandAction.EightSixZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight six one"
					},
					ActionToPerform=VoiceCommandAction.EightSixOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight six two"
					},
					ActionToPerform=VoiceCommandAction.EightSixTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight six three"
					},
					ActionToPerform=VoiceCommandAction.EightSixThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight six four"
					},
					ActionToPerform=VoiceCommandAction.EightSixFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight six five"
					},
					ActionToPerform=VoiceCommandAction.EightSixFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight six six"
					},
					ActionToPerform=VoiceCommandAction.EightSixSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight six seven"
					},
					ActionToPerform=VoiceCommandAction.EightSixSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight six eight"
					},
					ActionToPerform=VoiceCommandAction.EightSixEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight six nine"
					},
					ActionToPerform=VoiceCommandAction.EightSixNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight seven zero"
					},
					ActionToPerform=VoiceCommandAction.EightSevenZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight seven one"
					},
					ActionToPerform=VoiceCommandAction.EightSevenOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight seven two"
					},
					ActionToPerform=VoiceCommandAction.EightSevenTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight seven three"
					},
					ActionToPerform=VoiceCommandAction.EightSevenThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight seven four"
					},
					ActionToPerform=VoiceCommandAction.EightSevenFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight seven five"
					},
					ActionToPerform=VoiceCommandAction.EightSevenFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight seven six"
					},
					ActionToPerform=VoiceCommandAction.EightSevenSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight seven seven"
					},
					ActionToPerform=VoiceCommandAction.EightSevenSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight seven eight"
					},
					ActionToPerform=VoiceCommandAction.EightSevenEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight seven nine"
					},
					ActionToPerform=VoiceCommandAction.EightSevenNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight eight zero"
					},
					ActionToPerform=VoiceCommandAction.EightEightZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight eight one"
					},
					ActionToPerform=VoiceCommandAction.EightEightOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight eight two"
					},
					ActionToPerform=VoiceCommandAction.EightEightTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight eight three"
					},
					ActionToPerform=VoiceCommandAction.EightEightThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight eight four"
					},
					ActionToPerform=VoiceCommandAction.EightEightFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight eight five"
					},
					ActionToPerform=VoiceCommandAction.EightEightFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight eight six"
					},
					ActionToPerform=VoiceCommandAction.EightEightSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight eight seven"
					},
					ActionToPerform=VoiceCommandAction.EightEightSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight eight eight"
					},
					ActionToPerform=VoiceCommandAction.EightEightEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight eight nine"
					},
					ActionToPerform=VoiceCommandAction.EightEightNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight nine zero"
					},
					ActionToPerform=VoiceCommandAction.EightNineZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight nine one"
					},
					ActionToPerform=VoiceCommandAction.EightNineOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight nine two"
					},
					ActionToPerform=VoiceCommandAction.EightNineTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight nine three"
					},
					ActionToPerform=VoiceCommandAction.EightNineThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight nine four"
					},
					ActionToPerform=VoiceCommandAction.EightNineFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight nine five"
					},
					ActionToPerform=VoiceCommandAction.EightNineFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight nine six"
					},
					ActionToPerform=VoiceCommandAction.EightNineSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight nine seven"
					},
					ActionToPerform=VoiceCommandAction.EightNineSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight nine eight"
					},
					ActionToPerform=VoiceCommandAction.EightNineEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight nine nine"
					},
					ActionToPerform=VoiceCommandAction.EightNineNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine zero zero"
					},
					ActionToPerform=VoiceCommandAction.NineZeroZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine zero one"
					},
					ActionToPerform=VoiceCommandAction.NineZeroOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine zero two"
					},
					ActionToPerform=VoiceCommandAction.NineZeroTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine zero three"
					},
					ActionToPerform=VoiceCommandAction.NineZeroThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine zero four"
					},
					ActionToPerform=VoiceCommandAction.NineZeroFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine zero five"
					},
					ActionToPerform=VoiceCommandAction.NineZeroFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine zero six"
					},
					ActionToPerform=VoiceCommandAction.NineZeroSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine zero seven"
					},
					ActionToPerform=VoiceCommandAction.NineZeroSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine zero eight"
					},
					ActionToPerform=VoiceCommandAction.NineZeroEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine zero nine"
					},
					ActionToPerform=VoiceCommandAction.NineZeroNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine one zero"
					},
					ActionToPerform=VoiceCommandAction.NineOneZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine one one"
					},
					ActionToPerform=VoiceCommandAction.NineOneOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="nine one one"//Otherwise it was pronouncing 1 as 'ohney'
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine one two"
					},
					ActionToPerform=VoiceCommandAction.NineOneTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine one three"
					},
					ActionToPerform=VoiceCommandAction.NineOneThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine one four"
					},
					ActionToPerform=VoiceCommandAction.NineOneFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine one five"
					},
					ActionToPerform=VoiceCommandAction.NineOneFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine one six"
					},
					ActionToPerform=VoiceCommandAction.NineOneSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine one seven"
					},
					ActionToPerform=VoiceCommandAction.NineOneSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine one eight"
					},
					ActionToPerform=VoiceCommandAction.NineOneEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine one nine"
					},
					ActionToPerform=VoiceCommandAction.NineOneNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine two zero"
					},
					ActionToPerform=VoiceCommandAction.NineTwoZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine two one"
					},
					ActionToPerform=VoiceCommandAction.NineTwoOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine two two"
					},
					ActionToPerform=VoiceCommandAction.NineTwoTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine two three"
					},
					ActionToPerform=VoiceCommandAction.NineTwoThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine two four"
					},
					ActionToPerform=VoiceCommandAction.NineTwoFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine two five"
					},
					ActionToPerform=VoiceCommandAction.NineTwoFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine two six"
					},
					ActionToPerform=VoiceCommandAction.NineTwoSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine two seven"
					},
					ActionToPerform=VoiceCommandAction.NineTwoSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine two eight"
					},
					ActionToPerform=VoiceCommandAction.NineTwoEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine two nine"
					},
					ActionToPerform=VoiceCommandAction.NineTwoNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine three zero"
					},
					ActionToPerform=VoiceCommandAction.NineThreeZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine three one"
					},
					ActionToPerform=VoiceCommandAction.NineThreeOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine three two"
					},
					ActionToPerform=VoiceCommandAction.NineThreeTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine three three"
					},
					ActionToPerform=VoiceCommandAction.NineThreeThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine three four"
					},
					ActionToPerform=VoiceCommandAction.NineThreeFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine three five"
					},
					ActionToPerform=VoiceCommandAction.NineThreeFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine three six"
					},
					ActionToPerform=VoiceCommandAction.NineThreeSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine three seven"
					},
					ActionToPerform=VoiceCommandAction.NineThreeSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine three eight"
					},
					ActionToPerform=VoiceCommandAction.NineThreeEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine three nine"
					},
					ActionToPerform=VoiceCommandAction.NineThreeNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine four zero"
					},
					ActionToPerform=VoiceCommandAction.NineFourZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine four one"
					},
					ActionToPerform=VoiceCommandAction.NineFourOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine four two"
					},
					ActionToPerform=VoiceCommandAction.NineFourTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine four three"
					},
					ActionToPerform=VoiceCommandAction.NineFourThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine four four"
					},
					ActionToPerform=VoiceCommandAction.NineFourFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine four five"
					},
					ActionToPerform=VoiceCommandAction.NineFourFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine four six"
					},
					ActionToPerform=VoiceCommandAction.NineFourSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine four seven"
					},
					ActionToPerform=VoiceCommandAction.NineFourSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine four eight"
					},
					ActionToPerform=VoiceCommandAction.NineFourEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine four nine"
					},
					ActionToPerform=VoiceCommandAction.NineFourNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine five zero"
					},
					ActionToPerform=VoiceCommandAction.NineFiveZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine five one"
					},
					ActionToPerform=VoiceCommandAction.NineFiveOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine five two"
					},
					ActionToPerform=VoiceCommandAction.NineFiveTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine five three"
					},
					ActionToPerform=VoiceCommandAction.NineFiveThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine five four"
					},
					ActionToPerform=VoiceCommandAction.NineFiveFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine five five"
					},
					ActionToPerform=VoiceCommandAction.NineFiveFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine five six"
					},
					ActionToPerform=VoiceCommandAction.NineFiveSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine five seven"
					},
					ActionToPerform=VoiceCommandAction.NineFiveSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine five eight"
					},
					ActionToPerform=VoiceCommandAction.NineFiveEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine five nine"
					},
					ActionToPerform=VoiceCommandAction.NineFiveNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine six zero"
					},
					ActionToPerform=VoiceCommandAction.NineSixZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine six one"
					},
					ActionToPerform=VoiceCommandAction.NineSixOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine six two"
					},
					ActionToPerform=VoiceCommandAction.NineSixTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine six three"
					},
					ActionToPerform=VoiceCommandAction.NineSixThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine six four"
					},
					ActionToPerform=VoiceCommandAction.NineSixFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine six five"
					},
					ActionToPerform=VoiceCommandAction.NineSixFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine six six"
					},
					ActionToPerform=VoiceCommandAction.NineSixSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine six seven"
					},
					ActionToPerform=VoiceCommandAction.NineSixSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine six eight"
					},
					ActionToPerform=VoiceCommandAction.NineSixEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine six nine"
					},
					ActionToPerform=VoiceCommandAction.NineSixNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine seven zero"
					},
					ActionToPerform=VoiceCommandAction.NineSevenZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine seven one"
					},
					ActionToPerform=VoiceCommandAction.NineSevenOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine seven two"
					},
					ActionToPerform=VoiceCommandAction.NineSevenTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine seven three"
					},
					ActionToPerform=VoiceCommandAction.NineSevenThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine seven four"
					},
					ActionToPerform=VoiceCommandAction.NineSevenFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine seven five"
					},
					ActionToPerform=VoiceCommandAction.NineSevenFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine seven six"
					},
					ActionToPerform=VoiceCommandAction.NineSevenSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine seven seven"
					},
					ActionToPerform=VoiceCommandAction.NineSevenSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine seven eight"
					},
					ActionToPerform=VoiceCommandAction.NineSevenEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine seven nine"
					},
					ActionToPerform=VoiceCommandAction.NineSevenNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine eight zero"
					},
					ActionToPerform=VoiceCommandAction.NineEightZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine eight one"
					},
					ActionToPerform=VoiceCommandAction.NineEightOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine eight two"
					},
					ActionToPerform=VoiceCommandAction.NineEightTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine eight three"
					},
					ActionToPerform=VoiceCommandAction.NineEightThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine eight four"
					},
					ActionToPerform=VoiceCommandAction.NineEightFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine eight five"
					},
					ActionToPerform=VoiceCommandAction.NineEightFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine eight six"
					},
					ActionToPerform=VoiceCommandAction.NineEightSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine eight seven"
					},
					ActionToPerform=VoiceCommandAction.NineEightSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine eight eight"
					},
					ActionToPerform=VoiceCommandAction.NineEightEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine eight nine"
					},
					ActionToPerform=VoiceCommandAction.NineEightNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine nine zero"
					},
					ActionToPerform=VoiceCommandAction.NineNineZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine nine one"
					},
					ActionToPerform=VoiceCommandAction.NineNineOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine nine two"
					},
					ActionToPerform=VoiceCommandAction.NineNineTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine nine three"
					},
					ActionToPerform=VoiceCommandAction.NineNineThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine nine four"
					},
					ActionToPerform=VoiceCommandAction.NineNineFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine nine five"
					},
					ActionToPerform=VoiceCommandAction.NineNineFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine nine six"
					},
					ActionToPerform=VoiceCommandAction.NineNineSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine nine seven"
					},
					ActionToPerform=VoiceCommandAction.NineNineSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine nine eight"
					},
					ActionToPerform=VoiceCommandAction.NineNineEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine nine nine"
					},
					ActionToPerform=VoiceCommandAction.NineNineNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero zero"
					},
					ActionToPerform=VoiceCommandAction.ZeroZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero one"
					},
					ActionToPerform=VoiceCommandAction.ZeroOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero two"
					},
					ActionToPerform=VoiceCommandAction.ZeroTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero three"
					},
					ActionToPerform=VoiceCommandAction.ZeroThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero four"
					},
					ActionToPerform=VoiceCommandAction.ZeroFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero five"
					},
					ActionToPerform=VoiceCommandAction.ZeroFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero six"
					},
					ActionToPerform=VoiceCommandAction.ZeroSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero seven"
					},
					ActionToPerform=VoiceCommandAction.ZeroSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero eight"
					},
					ActionToPerform=VoiceCommandAction.ZeroEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"zero nine"
					},
					ActionToPerform=VoiceCommandAction.ZeroNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one zero"
					},
					ActionToPerform=VoiceCommandAction.OneZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one one"
					},
					ActionToPerform=VoiceCommandAction.OneOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="one one",
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one two"
					},
					ActionToPerform=VoiceCommandAction.OneTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one three"
					},
					ActionToPerform=VoiceCommandAction.OneThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one four"
					},
					ActionToPerform=VoiceCommandAction.OneFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one five"
					},
					ActionToPerform=VoiceCommandAction.OneFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one six"
					},
					ActionToPerform=VoiceCommandAction.OneSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one seven"
					},
					ActionToPerform=VoiceCommandAction.OneSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one eight"
					},
					ActionToPerform=VoiceCommandAction.OneEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"one nine"
					},
					ActionToPerform=VoiceCommandAction.OneNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two zero"
					},
					ActionToPerform=VoiceCommandAction.TwoZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two one"
					},
					ActionToPerform=VoiceCommandAction.TwoOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two two"
					},
					ActionToPerform=VoiceCommandAction.TwoTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two three"
					},
					ActionToPerform=VoiceCommandAction.TwoThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two four"
					},
					ActionToPerform=VoiceCommandAction.TwoFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two five"
					},
					ActionToPerform=VoiceCommandAction.TwoFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two six"
					},
					ActionToPerform=VoiceCommandAction.TwoSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two seven"
					},
					ActionToPerform=VoiceCommandAction.TwoSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two eight"
					},
					ActionToPerform=VoiceCommandAction.TwoEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"two nine"
					},
					ActionToPerform=VoiceCommandAction.TwoNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three zero"
					},
					ActionToPerform=VoiceCommandAction.ThreeZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three one"
					},
					ActionToPerform=VoiceCommandAction.ThreeOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three two"
					},
					ActionToPerform=VoiceCommandAction.ThreeTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three three"
					},
					ActionToPerform=VoiceCommandAction.ThreeThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three four"
					},
					ActionToPerform=VoiceCommandAction.ThreeFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three five"
					},
					ActionToPerform=VoiceCommandAction.ThreeFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three six"
					},
					ActionToPerform=VoiceCommandAction.ThreeSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three seven"
					},
					ActionToPerform=VoiceCommandAction.ThreeSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three eight"
					},
					ActionToPerform=VoiceCommandAction.ThreeEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"three nine"
					},
					ActionToPerform=VoiceCommandAction.ThreeNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four zero"
					},
					ActionToPerform=VoiceCommandAction.FourZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four one"
					},
					ActionToPerform=VoiceCommandAction.FourOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four two"
					},
					ActionToPerform=VoiceCommandAction.FourTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four three"
					},
					ActionToPerform=VoiceCommandAction.FourThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four four"
					},
					ActionToPerform=VoiceCommandAction.FourFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four five"
					},
					ActionToPerform=VoiceCommandAction.FourFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four six"
					},
					ActionToPerform=VoiceCommandAction.FourSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four seven"
					},
					ActionToPerform=VoiceCommandAction.FourSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four eight"
					},
					ActionToPerform=VoiceCommandAction.FourEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"four nine"
					},
					ActionToPerform=VoiceCommandAction.FourNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five zero"
					},
					ActionToPerform=VoiceCommandAction.FiveZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five one"
					},
					ActionToPerform=VoiceCommandAction.FiveOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five two"
					},
					ActionToPerform=VoiceCommandAction.FiveTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five three"
					},
					ActionToPerform=VoiceCommandAction.FiveThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five four"
					},
					ActionToPerform=VoiceCommandAction.FiveFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five five"
					},
					ActionToPerform=VoiceCommandAction.FiveFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five six"
					},
					ActionToPerform=VoiceCommandAction.FiveSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five seven"
					},
					ActionToPerform=VoiceCommandAction.FiveSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five eight"
					},
					ActionToPerform=VoiceCommandAction.FiveEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"five nine"
					},
					ActionToPerform=VoiceCommandAction.FiveNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six zero"
					},
					ActionToPerform=VoiceCommandAction.SixZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six one"
					},
					ActionToPerform=VoiceCommandAction.SixOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six two"
					},
					ActionToPerform=VoiceCommandAction.SixTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six three"
					},
					ActionToPerform=VoiceCommandAction.SixThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six four"
					},
					ActionToPerform=VoiceCommandAction.SixFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six five"
					},
					ActionToPerform=VoiceCommandAction.SixFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six six"
					},
					ActionToPerform=VoiceCommandAction.SixSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six seven"
					},
					ActionToPerform=VoiceCommandAction.SixSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six eight"
					},
					ActionToPerform=VoiceCommandAction.SixEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"six nine"
					},
					ActionToPerform=VoiceCommandAction.SixNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven zero"
					},
					ActionToPerform=VoiceCommandAction.SevenZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven one"
					},
					ActionToPerform=VoiceCommandAction.SevenOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven two"
					},
					ActionToPerform=VoiceCommandAction.SevenTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven three"
					},
					ActionToPerform=VoiceCommandAction.SevenThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven four"
					},
					ActionToPerform=VoiceCommandAction.SevenFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven five"
					},
					ActionToPerform=VoiceCommandAction.SevenFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven six"
					},
					ActionToPerform=VoiceCommandAction.SevenSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven seven"
					},
					ActionToPerform=VoiceCommandAction.SevenSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven eight"
					},
					ActionToPerform=VoiceCommandAction.SevenEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"seven nine"
					},
					ActionToPerform=VoiceCommandAction.SevenNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight zero"
					},
					ActionToPerform=VoiceCommandAction.EightZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight one"
					},
					ActionToPerform=VoiceCommandAction.EightOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight two"
					},
					ActionToPerform=VoiceCommandAction.EightTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight three"
					},
					ActionToPerform=VoiceCommandAction.EightThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight four"
					},
					ActionToPerform=VoiceCommandAction.EightFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight five"
					},
					ActionToPerform=VoiceCommandAction.EightFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight six"
					},
					ActionToPerform=VoiceCommandAction.EightSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight seven"
					},
					ActionToPerform=VoiceCommandAction.EightSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight eight"
					},
					ActionToPerform=VoiceCommandAction.EightEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"eight nine"
					},
					ActionToPerform=VoiceCommandAction.EightNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine zero"
					},
					ActionToPerform=VoiceCommandAction.NineZero,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine one"
					},
					ActionToPerform=VoiceCommandAction.NineOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine two"
					},
					ActionToPerform=VoiceCommandAction.NineTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine three"
					},
					ActionToPerform=VoiceCommandAction.NineThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine four"
					},
					ActionToPerform=VoiceCommandAction.NineFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine five"
					},
					ActionToPerform=VoiceCommandAction.NineFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine six"
					},
					ActionToPerform=VoiceCommandAction.NineSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine seven"
					},
					ActionToPerform=VoiceCommandAction.NineSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine eight"
					},
					ActionToPerform=VoiceCommandAction.NineEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"nine nine"
					},
					ActionToPerform=VoiceCommandAction.NineNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				#endregion Hard-Coded Triplets and Doubles
				new VoiceCommand {
					Commands=new List<string> {
						"triplet",
						"triplets",
					},
					ActionToPerform=VoiceCommandAction.Triplets,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"check triplets",
					},
					ActionToPerform=VoiceCommandAction.CheckTriplets,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"uncheck triplets",
					},
					ActionToPerform=VoiceCommandAction.UncheckTriplets,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"bleeding",
						"mark bleeding"
					},
					ActionToPerform=VoiceCommandAction.Bleeding,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"bleeding distal",
						"mark bleeding distal"
					},
					ActionToPerform=VoiceCommandAction.BleedingDistal,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"bleeding facial",
						"mark bleeding facial"
					},
					ActionToPerform=VoiceCommandAction.BleedingFacial,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"bleeding lingual",
						"mark bleeding lingual"
					},
					ActionToPerform=VoiceCommandAction.BleedingLingual,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"bleeding mesial",
						"mark bleeding mesial"
					},
					ActionToPerform=VoiceCommandAction.BleedingMesial,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"calculus",
						"mark calculus"
					},
					ActionToPerform=VoiceCommandAction.Calculus,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"calculus distal",
						"mark calculus distal"
					},
					ActionToPerform=VoiceCommandAction.CalculusDistal,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"calculus facial",
						"mark calculus facial"
					},
					ActionToPerform=VoiceCommandAction.CalculusFacial,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"calculus lingual",
						"mark calculus lingual"
					},
					ActionToPerform=VoiceCommandAction.CalculusLingual,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"calculus mesial",
						"mark calculus mesial"
					},
					ActionToPerform=VoiceCommandAction.CalculusMesial,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"plaque"
					},
					ActionToPerform=VoiceCommandAction.Plaque,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"plaque distal",
						"mark plaque distal"
					},
					ActionToPerform=VoiceCommandAction.PlaqueDistal,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"plaque facial",
						"mark plaque facial"
					},
					ActionToPerform=VoiceCommandAction.PlaqueFacial,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"plaque lingual",
						"mark plaque lingual"
					},
					ActionToPerform=VoiceCommandAction.PlaqueLingual,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"plaque mesial",
						"mark plaque mesial"
					},
					ActionToPerform=VoiceCommandAction.PlaqueMesial,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"suppuration"
					},
					ActionToPerform=VoiceCommandAction.Suppuration,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"suppuration distal",
						"mark suppuration distal"
					},
					ActionToPerform=VoiceCommandAction.SuppurationDistal,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"suppuration facial",
						"mark suppuration facial"
					},
					ActionToPerform=VoiceCommandAction.SuppurationFacial,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"suppuration lingual",
						"mark suppuration lingual"
					},
					ActionToPerform=VoiceCommandAction.SuppurationLingual,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"suppuration mesial",
						"mark suppuration mesial"
					},
					ActionToPerform=VoiceCommandAction.SuppurationMesial,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						//"back", //This got confused with 'five' too often.
						"backspace"
					},
					ActionToPerform=VoiceCommandAction.Backspace,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"left"
					},
					ActionToPerform=VoiceCommandAction.Left,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"right"
					},
					ActionToPerform=VoiceCommandAction.Right,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"delete"
					},
					ActionToPerform=VoiceCommandAction.Delete,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"copy previous",
						"copy previous exam"
					},
					ActionToPerform=VoiceCommandAction.CopyPrevious,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Copying previous exam"
				},
			#region Go To Tooth W/O Facial/Lingual
				GoToToothCommandHelper("one",VoiceCommandAction.GoToToothOne),
				GoToToothCommandHelper("two",VoiceCommandAction.GoToToothTwo),
				GoToToothCommandHelper("three",VoiceCommandAction.GoToToothThree),
				GoToToothCommandHelper("four",VoiceCommandAction.GoToToothFour),
				GoToToothCommandHelper("five",VoiceCommandAction.GoToToothFive),
				GoToToothCommandHelper("six",VoiceCommandAction.GoToToothSix),
				GoToToothCommandHelper("seven",VoiceCommandAction.GoToToothSeven),
				GoToToothCommandHelper("eight",VoiceCommandAction.GoToToothEight),
				GoToToothCommandHelper("nine",VoiceCommandAction.GoToToothNine),
				GoToToothCommandHelper("ten",VoiceCommandAction.GoToToothTen),
				GoToToothCommandHelper("eleven",VoiceCommandAction.GoToToothEleven),
				GoToToothCommandHelper("twelve",VoiceCommandAction.GoToToothTwelve),
				GoToToothCommandHelper("thirteen",VoiceCommandAction.GoToToothThirteen),
				GoToToothCommandHelper("fourteen",VoiceCommandAction.GoToToothFourteen),
				GoToToothCommandHelper("fifteen",VoiceCommandAction.GoToToothFifteen),
				GoToToothCommandHelper("sixteen",VoiceCommandAction.GoToToothSixteen),
				GoToToothCommandHelper("seventeen",VoiceCommandAction.GoToToothSeventeen),
				GoToToothCommandHelper("eighteen",VoiceCommandAction.GoToToothEighteen),
				GoToToothCommandHelper("nineteen",VoiceCommandAction.GoToToothNineteen),
				GoToToothCommandHelper("twenty",VoiceCommandAction.GoToToothTwenty),
				GoToToothCommandHelper("twenty one",VoiceCommandAction.GoToToothTwentyOne),
				GoToToothCommandHelper("twenty two",VoiceCommandAction.GoToToothTwentyTwo),
				GoToToothCommandHelper("twenty three",VoiceCommandAction.GoToToothTwentyThree),
				GoToToothCommandHelper("twenty four",VoiceCommandAction.GoToToothTwentyFour),
				GoToToothCommandHelper("twenty five",VoiceCommandAction.GoToToothTwentyFive),
				GoToToothCommandHelper("twenty six",VoiceCommandAction.GoToToothTwentySix),
				GoToToothCommandHelper("twenty seven",VoiceCommandAction.GoToToothTwentySeven),
				GoToToothCommandHelper("twenty eight",VoiceCommandAction.GoToToothTwentyEight),
				GoToToothCommandHelper("twenty nine",VoiceCommandAction.GoToToothTwentyNine),
				GoToToothCommandHelper("thirty",VoiceCommandAction.GoToToothThirty),
				GoToToothCommandHelper("thirty one",VoiceCommandAction.GoToToothThirtyOne),
				GoToToothCommandHelper("thirty two",VoiceCommandAction.GoToToothThirtyTwo),
			#endregion
			#region Go To Tooth W/ Surfaces
				GoToToothWithSurfaceXCommandHelper("one","facial",VoiceCommandAction.GoToToothOneFacial),
				GoToToothWithSurfaceXCommandHelper("two","facial",VoiceCommandAction.GoToToothTwoFacial),
				GoToToothWithSurfaceXCommandHelper("three","facial",VoiceCommandAction.GoToToothThreeFacial),
				GoToToothWithSurfaceXCommandHelper("four","facial",VoiceCommandAction.GoToToothFourFacial),
				GoToToothWithSurfaceXCommandHelper("five","facial",VoiceCommandAction.GoToToothFiveFacial),
				GoToToothWithSurfaceXCommandHelper("six","facial",VoiceCommandAction.GoToToothSixFacial),
				GoToToothWithSurfaceXCommandHelper("seven","facial",VoiceCommandAction.GoToToothSevenFacial),
				GoToToothWithSurfaceXCommandHelper("eight","facial",VoiceCommandAction.GoToToothEightFacial),
				GoToToothWithSurfaceXCommandHelper("nine","facial",VoiceCommandAction.GoToToothNineFacial),
				GoToToothWithSurfaceXCommandHelper("ten","facial",VoiceCommandAction.GoToToothTenFacial),
				GoToToothWithSurfaceXCommandHelper("eleven","facial",VoiceCommandAction.GoToToothElevenFacial),
				GoToToothWithSurfaceXCommandHelper("twelve","facial",VoiceCommandAction.GoToToothTwelveFacial),
				GoToToothWithSurfaceXCommandHelper("thirteen","facial",VoiceCommandAction.GoToToothThirteenFacial),
				GoToToothWithSurfaceXCommandHelper("fourteen","facial",VoiceCommandAction.GoToToothFourteenFacial),
				GoToToothWithSurfaceXCommandHelper("fifteen","facial",VoiceCommandAction.GoToToothFifteenFacial),
				GoToToothWithSurfaceXCommandHelper("sixteen","facial",VoiceCommandAction.GoToToothSixteenFacial),
				GoToToothWithSurfaceXCommandHelper("seventeen","facial",VoiceCommandAction.GoToToothSeventeenFacial),
				GoToToothWithSurfaceXCommandHelper("eighteen","facial",VoiceCommandAction.GoToToothEighteenFacial),
				GoToToothWithSurfaceXCommandHelper("nineteen","facial",VoiceCommandAction.GoToToothNineteenFacial),
				GoToToothWithSurfaceXCommandHelper("twenty","facial",VoiceCommandAction.GoToToothTwentyFacial),
				GoToToothWithSurfaceXCommandHelper("twenty one","facial",VoiceCommandAction.GoToToothTwentyOneFacial),
				GoToToothWithSurfaceXCommandHelper("twenty two","facial",VoiceCommandAction.GoToToothTwentyTwoFacial),
				GoToToothWithSurfaceXCommandHelper("twenty three","facial",VoiceCommandAction.GoToToothTwentyThreeFacial),
				GoToToothWithSurfaceXCommandHelper("twenty four","facial",VoiceCommandAction.GoToToothTwentyFourFacial),
				GoToToothWithSurfaceXCommandHelper("twenty five","facial",VoiceCommandAction.GoToToothTwentyFiveFacial),
				GoToToothWithSurfaceXCommandHelper("twenty six","facial",VoiceCommandAction.GoToToothTwentySixFacial),
				GoToToothWithSurfaceXCommandHelper("twenty seven","facial",VoiceCommandAction.GoToToothTwentySevenFacial),
				GoToToothWithSurfaceXCommandHelper("twenty eight","facial",VoiceCommandAction.GoToToothTwentyEightFacial),
				GoToToothWithSurfaceXCommandHelper("twenty nine","facial",VoiceCommandAction.GoToToothTwentyNineFacial),
				GoToToothWithSurfaceXCommandHelper("thirty","facial",VoiceCommandAction.GoToToothThirtyFacial),
				GoToToothWithSurfaceXCommandHelper("thirty one","facial",VoiceCommandAction.GoToToothThirtyOneFacial),
				GoToToothWithSurfaceXCommandHelper("thirty two","facial",VoiceCommandAction.GoToToothThirtyTwoFacial),
				GoToToothWithSurfaceXCommandHelper("one","lingual",VoiceCommandAction.GoToToothOneLingual),
				GoToToothWithSurfaceXCommandHelper("two","lingual",VoiceCommandAction.GoToToothTwoLingual),
				GoToToothWithSurfaceXCommandHelper("three","lingual",VoiceCommandAction.GoToToothThreeLingual),
				GoToToothWithSurfaceXCommandHelper("four","lingual",VoiceCommandAction.GoToToothFourLingual),
				GoToToothWithSurfaceXCommandHelper("five","lingual",VoiceCommandAction.GoToToothFiveLingual),
				GoToToothWithSurfaceXCommandHelper("six","lingual",VoiceCommandAction.GoToToothSixLingual),
				GoToToothWithSurfaceXCommandHelper("seven","lingual",VoiceCommandAction.GoToToothSevenLingual),
				GoToToothWithSurfaceXCommandHelper("eight","lingual",VoiceCommandAction.GoToToothEightLingual),
				GoToToothWithSurfaceXCommandHelper("nine","lingual",VoiceCommandAction.GoToToothNineLingual),
				GoToToothWithSurfaceXCommandHelper("ten","lingual",VoiceCommandAction.GoToToothTenLingual),
				GoToToothWithSurfaceXCommandHelper("eleven","lingual",VoiceCommandAction.GoToToothElevenLingual),
				GoToToothWithSurfaceXCommandHelper("twelve","lingual",VoiceCommandAction.GoToToothTwelveLingual),
				GoToToothWithSurfaceXCommandHelper("thirteen","lingual",VoiceCommandAction.GoToToothThirteenLingual),
				GoToToothWithSurfaceXCommandHelper("fourteen","lingual",VoiceCommandAction.GoToToothFourteenLingual),
				GoToToothWithSurfaceXCommandHelper("fifteen","lingual",VoiceCommandAction.GoToToothFifteenLingual),
				GoToToothWithSurfaceXCommandHelper("sixteen","lingual",VoiceCommandAction.GoToToothSixteenLingual),
				GoToToothWithSurfaceXCommandHelper("seventeen","lingual",VoiceCommandAction.GoToToothSeventeenLingual),
				GoToToothWithSurfaceXCommandHelper("eighteen","lingual",VoiceCommandAction.GoToToothEighteenLingual),
				GoToToothWithSurfaceXCommandHelper("nineteen","lingual",VoiceCommandAction.GoToToothNineteenLingual),
				GoToToothWithSurfaceXCommandHelper("twenty","lingual",VoiceCommandAction.GoToToothTwentyLingual),
				GoToToothWithSurfaceXCommandHelper("twenty one","lingual",VoiceCommandAction.GoToToothTwentyOneLingual),
				GoToToothWithSurfaceXCommandHelper("twenty two","lingual",VoiceCommandAction.GoToToothTwentyTwoLingual),
				GoToToothWithSurfaceXCommandHelper("twenty three","lingual",VoiceCommandAction.GoToToothTwentyThreeLingual),
				GoToToothWithSurfaceXCommandHelper("twenty four","lingual",VoiceCommandAction.GoToToothTwentyFourLingual),
				GoToToothWithSurfaceXCommandHelper("twenty five","lingual",VoiceCommandAction.GoToToothTwentyFiveLingual),
				GoToToothWithSurfaceXCommandHelper("twenty six","lingual",VoiceCommandAction.GoToToothTwentySixLingual),
				GoToToothWithSurfaceXCommandHelper("twenty seven","lingual",VoiceCommandAction.GoToToothTwentySevenLingual),
				GoToToothWithSurfaceXCommandHelper("twenty eight","lingual",VoiceCommandAction.GoToToothTwentyEightLingual),
				GoToToothWithSurfaceXCommandHelper("twenty nine","lingual",VoiceCommandAction.GoToToothTwentyNineLingual),
				GoToToothWithSurfaceXCommandHelper("thirty","lingual",VoiceCommandAction.GoToToothThirtyLingual),
				GoToToothWithSurfaceXCommandHelper("thirty one","lingual",VoiceCommandAction.GoToToothThirtyOneLingual),
				GoToToothWithSurfaceXCommandHelper("thirty two","lingual",VoiceCommandAction.GoToToothThirtyTwoLingual),

				GoToToothWithSurfaceXCommandHelper("one","distal",VoiceCommandAction.GoToToothOneDistal),
				GoToToothWithSurfaceXCommandHelper("two","distal",VoiceCommandAction.GoToToothTwoDistal),
				GoToToothWithSurfaceXCommandHelper("three","distal",VoiceCommandAction.GoToToothThreeDistal),
				GoToToothWithSurfaceXCommandHelper("four","distal",VoiceCommandAction.GoToToothFourDistal),
				GoToToothWithSurfaceXCommandHelper("five","distal",VoiceCommandAction.GoToToothFiveDistal),
				GoToToothWithSurfaceXCommandHelper("six","distal",VoiceCommandAction.GoToToothSixDistal),
				GoToToothWithSurfaceXCommandHelper("seven","distal",VoiceCommandAction.GoToToothSevenDistal),
				GoToToothWithSurfaceXCommandHelper("eight","distal",VoiceCommandAction.GoToToothEightDistal),
				GoToToothWithSurfaceXCommandHelper("nine","distal",VoiceCommandAction.GoToToothNineDistal),
				GoToToothWithSurfaceXCommandHelper("ten","distal",VoiceCommandAction.GoToToothTenDistal),
				GoToToothWithSurfaceXCommandHelper("eleven","distal",VoiceCommandAction.GoToToothElevenDistal),
				GoToToothWithSurfaceXCommandHelper("twelve","distal",VoiceCommandAction.GoToToothTwelveDistal),
				GoToToothWithSurfaceXCommandHelper("thirteen","distal",VoiceCommandAction.GoToToothThirteenDistal),
				GoToToothWithSurfaceXCommandHelper("fourteen","distal",VoiceCommandAction.GoToToothFourteenDistal),
				GoToToothWithSurfaceXCommandHelper("fifteen","distal",VoiceCommandAction.GoToToothFifteenDistal),
				GoToToothWithSurfaceXCommandHelper("sixteen","distal",VoiceCommandAction.GoToToothSixteenDistal),
				GoToToothWithSurfaceXCommandHelper("seventeen","distal",VoiceCommandAction.GoToToothSeventeenDistal),
				GoToToothWithSurfaceXCommandHelper("eighteen","distal",VoiceCommandAction.GoToToothEighteenDistal),
				GoToToothWithSurfaceXCommandHelper("nineteen","distal",VoiceCommandAction.GoToToothNineteenDistal),
				GoToToothWithSurfaceXCommandHelper("twenty","distal",VoiceCommandAction.GoToToothTwentyDistal),
				GoToToothWithSurfaceXCommandHelper("twenty one","distal",VoiceCommandAction.GoToToothTwentyOneDistal),
				GoToToothWithSurfaceXCommandHelper("twenty two","distal",VoiceCommandAction.GoToToothTwentyTwoDistal),
				GoToToothWithSurfaceXCommandHelper("twenty three","distal",VoiceCommandAction.GoToToothTwentyThreeDistal),
				GoToToothWithSurfaceXCommandHelper("twenty four","distal",VoiceCommandAction.GoToToothTwentyFourDistal),
				GoToToothWithSurfaceXCommandHelper("twenty five","distal",VoiceCommandAction.GoToToothTwentyFiveDistal),
				GoToToothWithSurfaceXCommandHelper("twenty six","distal",VoiceCommandAction.GoToToothTwentySixDistal),
				GoToToothWithSurfaceXCommandHelper("twenty seven","distal",VoiceCommandAction.GoToToothTwentySevenDistal),
				GoToToothWithSurfaceXCommandHelper("twenty eight","distal",VoiceCommandAction.GoToToothTwentyEightDistal),
				GoToToothWithSurfaceXCommandHelper("twenty nine","distal",VoiceCommandAction.GoToToothTwentyNineDistal),
				GoToToothWithSurfaceXCommandHelper("thirty","distal",VoiceCommandAction.GoToToothThirtyDistal),
				GoToToothWithSurfaceXCommandHelper("thirty one","distal",VoiceCommandAction.GoToToothThirtyOneDistal),
				GoToToothWithSurfaceXCommandHelper("thirty two","distal",VoiceCommandAction.GoToToothThirtyTwoDistal),

				GoToToothWithSurfaceCommandHelper("one","distal",true,VoiceCommandAction.GoToToothOneDistalFacial),
				GoToToothWithSurfaceCommandHelper("two","distal",true,VoiceCommandAction.GoToToothTwoDistalFacial),
				GoToToothWithSurfaceCommandHelper("three","distal",true,VoiceCommandAction.GoToToothThreeDistalFacial),
				GoToToothWithSurfaceCommandHelper("four","distal",true,VoiceCommandAction.GoToToothFourDistalFacial),
				GoToToothWithSurfaceCommandHelper("five","distal",true,VoiceCommandAction.GoToToothFiveDistalFacial),
				GoToToothWithSurfaceCommandHelper("six","distal",true,VoiceCommandAction.GoToToothSixDistalFacial),
				GoToToothWithSurfaceCommandHelper("seven","distal",true,VoiceCommandAction.GoToToothSevenDistalFacial),
				GoToToothWithSurfaceCommandHelper("eight","distal",true,VoiceCommandAction.GoToToothEightDistalFacial),
				GoToToothWithSurfaceCommandHelper("nine","distal",true,VoiceCommandAction.GoToToothNineDistalFacial),
				GoToToothWithSurfaceCommandHelper("ten","distal",true,VoiceCommandAction.GoToToothTenDistalFacial),
				GoToToothWithSurfaceCommandHelper("eleven","distal",true,VoiceCommandAction.GoToToothElevenDistalFacial),
				GoToToothWithSurfaceCommandHelper("twelve","distal",true,VoiceCommandAction.GoToToothTwelveDistalFacial),
				GoToToothWithSurfaceCommandHelper("thirteen","distal",true,VoiceCommandAction.GoToToothThirteenDistalFacial),
				GoToToothWithSurfaceCommandHelper("fourteen","distal",true,VoiceCommandAction.GoToToothFourteenDistalFacial),
				GoToToothWithSurfaceCommandHelper("fifteen","distal",true,VoiceCommandAction.GoToToothFifteenDistalFacial),
				GoToToothWithSurfaceCommandHelper("sixteen","distal",true,VoiceCommandAction.GoToToothSixteenDistalFacial),
				GoToToothWithSurfaceCommandHelper("seventeen","distal",true,VoiceCommandAction.GoToToothSeventeenDistalFacial),
				GoToToothWithSurfaceCommandHelper("eighteen","distal",true,VoiceCommandAction.GoToToothEighteenDistalFacial),
				GoToToothWithSurfaceCommandHelper("nineteen","distal",true,VoiceCommandAction.GoToToothNineteenDistalFacial),
				GoToToothWithSurfaceCommandHelper("twenty","distal",true,VoiceCommandAction.GoToToothTwentyDistalFacial),
				GoToToothWithSurfaceCommandHelper("twenty one","distal",true,VoiceCommandAction.GoToToothTwentyOneDistalFacial),
				GoToToothWithSurfaceCommandHelper("twenty two","distal",true,VoiceCommandAction.GoToToothTwentyTwoDistalFacial),
				GoToToothWithSurfaceCommandHelper("twenty three","distal",true,VoiceCommandAction.GoToToothTwentyThreeDistalFacial),
				GoToToothWithSurfaceCommandHelper("twenty four","distal",true,VoiceCommandAction.GoToToothTwentyFourDistalFacial),
				GoToToothWithSurfaceCommandHelper("twenty five","distal",true,VoiceCommandAction.GoToToothTwentyFiveDistalFacial),
				GoToToothWithSurfaceCommandHelper("twenty six","distal",true,VoiceCommandAction.GoToToothTwentySixDistalFacial),
				GoToToothWithSurfaceCommandHelper("twenty seven","distal",true,VoiceCommandAction.GoToToothTwentySevenDistalFacial),
				GoToToothWithSurfaceCommandHelper("twenty eight","distal",true,VoiceCommandAction.GoToToothTwentyEightDistalFacial),
				GoToToothWithSurfaceCommandHelper("twenty nine","distal",true,VoiceCommandAction.GoToToothTwentyNineDistalFacial),
				GoToToothWithSurfaceCommandHelper("thirty","distal",true,VoiceCommandAction.GoToToothThirtyDistalFacial),
				GoToToothWithSurfaceCommandHelper("thirty one","distal",true,VoiceCommandAction.GoToToothThirtyOneDistalFacial),
				GoToToothWithSurfaceCommandHelper("thirty two","distal",true,VoiceCommandAction.GoToToothThirtyTwoDistalFacial),
				GoToToothWithSurfaceCommandHelper("one","distal",false,VoiceCommandAction.GoToToothOneDistalLingual),
				GoToToothWithSurfaceCommandHelper("two","distal",false,VoiceCommandAction.GoToToothTwoDistalLingual),
				GoToToothWithSurfaceCommandHelper("three","distal",false,VoiceCommandAction.GoToToothThreeDistalLingual),
				GoToToothWithSurfaceCommandHelper("four","distal",false,VoiceCommandAction.GoToToothFourDistalLingual),
				GoToToothWithSurfaceCommandHelper("five","distal",false,VoiceCommandAction.GoToToothFiveDistalLingual),
				GoToToothWithSurfaceCommandHelper("six","distal",false,VoiceCommandAction.GoToToothSixDistalLingual),
				GoToToothWithSurfaceCommandHelper("seven","distal",false,VoiceCommandAction.GoToToothSevenDistalLingual),
				GoToToothWithSurfaceCommandHelper("eight","distal",false,VoiceCommandAction.GoToToothEightDistalLingual),
				GoToToothWithSurfaceCommandHelper("nine","distal",false,VoiceCommandAction.GoToToothNineDistalLingual),
				GoToToothWithSurfaceCommandHelper("ten","distal",false,VoiceCommandAction.GoToToothTenDistalLingual),
				GoToToothWithSurfaceCommandHelper("eleven","distal",false,VoiceCommandAction.GoToToothElevenDistalLingual),
				GoToToothWithSurfaceCommandHelper("twelve","distal",false,VoiceCommandAction.GoToToothTwelveDistalLingual),
				GoToToothWithSurfaceCommandHelper("thirteen","distal",false,VoiceCommandAction.GoToToothThirteenDistalLingual),
				GoToToothWithSurfaceCommandHelper("fourteen","distal",false,VoiceCommandAction.GoToToothFourteenDistalLingual),
				GoToToothWithSurfaceCommandHelper("fifteen","distal",false,VoiceCommandAction.GoToToothFifteenDistalLingual),
				GoToToothWithSurfaceCommandHelper("sixteen","distal",false,VoiceCommandAction.GoToToothSixteenDistalLingual),
				GoToToothWithSurfaceCommandHelper("seventeen","distal",false,VoiceCommandAction.GoToToothSeventeenDistalLingual),
				GoToToothWithSurfaceCommandHelper("eighteen","distal",false,VoiceCommandAction.GoToToothEighteenDistalLingual),
				GoToToothWithSurfaceCommandHelper("nineteen","distal",false,VoiceCommandAction.GoToToothNineteenDistalLingual),
				GoToToothWithSurfaceCommandHelper("twenty","distal",false,VoiceCommandAction.GoToToothTwentyDistalLingual),
				GoToToothWithSurfaceCommandHelper("twenty one","distal",false,VoiceCommandAction.GoToToothTwentyOneDistalLingual),
				GoToToothWithSurfaceCommandHelper("twenty two","distal",false,VoiceCommandAction.GoToToothTwentyTwoDistalLingual),
				GoToToothWithSurfaceCommandHelper("twenty three","distal",false,VoiceCommandAction.GoToToothTwentyThreeDistalLingual),
				GoToToothWithSurfaceCommandHelper("twenty four","distal",false,VoiceCommandAction.GoToToothTwentyFourDistalLingual),
				GoToToothWithSurfaceCommandHelper("twenty five","distal",false,VoiceCommandAction.GoToToothTwentyFiveDistalLingual),
				GoToToothWithSurfaceCommandHelper("twenty six","distal",false,VoiceCommandAction.GoToToothTwentySixDistalLingual),
				GoToToothWithSurfaceCommandHelper("twenty seven","distal",false,VoiceCommandAction.GoToToothTwentySevenDistalLingual),
				GoToToothWithSurfaceCommandHelper("twenty eight","distal",false,VoiceCommandAction.GoToToothTwentyEightDistalLingual),
				GoToToothWithSurfaceCommandHelper("twenty nine","distal",false,VoiceCommandAction.GoToToothTwentyNineDistalLingual),
				GoToToothWithSurfaceCommandHelper("thirty","distal",false,VoiceCommandAction.GoToToothThirtyDistalLingual),
				GoToToothWithSurfaceCommandHelper("thirty one","distal",false,VoiceCommandAction.GoToToothThirtyOneDistalLingual),
				GoToToothWithSurfaceCommandHelper("thirty two","distal",false,VoiceCommandAction.GoToToothThirtyTwoDistalLingual),

				GoToToothWithSurfaceXCommandHelper("one","mesial",VoiceCommandAction.GoToToothOneMesial),
				GoToToothWithSurfaceXCommandHelper("two","mesial",VoiceCommandAction.GoToToothTwoMesial),
				GoToToothWithSurfaceXCommandHelper("three","mesial",VoiceCommandAction.GoToToothThreeMesial),
				GoToToothWithSurfaceXCommandHelper("four","mesial",VoiceCommandAction.GoToToothFourMesial),
				GoToToothWithSurfaceXCommandHelper("five","mesial",VoiceCommandAction.GoToToothFiveMesial),
				GoToToothWithSurfaceXCommandHelper("six","mesial",VoiceCommandAction.GoToToothSixMesial),
				GoToToothWithSurfaceXCommandHelper("seven","mesial",VoiceCommandAction.GoToToothSevenMesial),
				GoToToothWithSurfaceXCommandHelper("eight","mesial",VoiceCommandAction.GoToToothEightMesial),
				GoToToothWithSurfaceXCommandHelper("nine","mesial",VoiceCommandAction.GoToToothNineMesial),
				GoToToothWithSurfaceXCommandHelper("ten","mesial",VoiceCommandAction.GoToToothTenMesial),
				GoToToothWithSurfaceXCommandHelper("eleven","mesial",VoiceCommandAction.GoToToothElevenMesial),
				GoToToothWithSurfaceXCommandHelper("twelve","mesial",VoiceCommandAction.GoToToothTwelveMesial),
				GoToToothWithSurfaceXCommandHelper("thirteen","mesial",VoiceCommandAction.GoToToothThirteenMesial),
				GoToToothWithSurfaceXCommandHelper("fourteen","mesial",VoiceCommandAction.GoToToothFourteenMesial),
				GoToToothWithSurfaceXCommandHelper("fifteen","mesial",VoiceCommandAction.GoToToothFifteenMesial),
				GoToToothWithSurfaceXCommandHelper("sixteen","mesial",VoiceCommandAction.GoToToothSixteenMesial),
				GoToToothWithSurfaceXCommandHelper("seventeen","mesial",VoiceCommandAction.GoToToothSeventeenMesial),
				GoToToothWithSurfaceXCommandHelper("eighteen","mesial",VoiceCommandAction.GoToToothEighteenMesial),
				GoToToothWithSurfaceXCommandHelper("nineteen","mesial",VoiceCommandAction.GoToToothNineteenMesial),
				GoToToothWithSurfaceXCommandHelper("twenty","mesial",VoiceCommandAction.GoToToothTwentyMesial),
				GoToToothWithSurfaceXCommandHelper("twenty one","mesial",VoiceCommandAction.GoToToothTwentyOneMesial),
				GoToToothWithSurfaceXCommandHelper("twenty two","mesial",VoiceCommandAction.GoToToothTwentyTwoMesial),
				GoToToothWithSurfaceXCommandHelper("twenty three","mesial",VoiceCommandAction.GoToToothTwentyThreeMesial),
				GoToToothWithSurfaceXCommandHelper("twenty four","mesial",VoiceCommandAction.GoToToothTwentyFourMesial),
				GoToToothWithSurfaceXCommandHelper("twenty five","mesial",VoiceCommandAction.GoToToothTwentyFiveMesial),
				GoToToothWithSurfaceXCommandHelper("twenty six","mesial",VoiceCommandAction.GoToToothTwentySixMesial),
				GoToToothWithSurfaceXCommandHelper("twenty seven","mesial",VoiceCommandAction.GoToToothTwentySevenMesial),
				GoToToothWithSurfaceXCommandHelper("twenty eight","mesial",VoiceCommandAction.GoToToothTwentyEightMesial),
				GoToToothWithSurfaceXCommandHelper("twenty nine","mesial",VoiceCommandAction.GoToToothTwentyNineMesial),
				GoToToothWithSurfaceXCommandHelper("thirty","mesial",VoiceCommandAction.GoToToothThirtyMesial),
				GoToToothWithSurfaceXCommandHelper("thirty one","mesial",VoiceCommandAction.GoToToothThirtyOneMesial),
				GoToToothWithSurfaceXCommandHelper("thirty two","mesial",VoiceCommandAction.GoToToothThirtyTwoMesial),

				GoToToothWithSurfaceCommandHelper("one","mesial",true,VoiceCommandAction.GoToToothOneMesialFacial),
				GoToToothWithSurfaceCommandHelper("two","mesial",true,VoiceCommandAction.GoToToothTwoMesialFacial),
				GoToToothWithSurfaceCommandHelper("three","mesial",true,VoiceCommandAction.GoToToothThreeMesialFacial),
				GoToToothWithSurfaceCommandHelper("four","mesial",true,VoiceCommandAction.GoToToothFourMesialFacial),
				GoToToothWithSurfaceCommandHelper("five","mesial",true,VoiceCommandAction.GoToToothFiveMesialFacial),
				GoToToothWithSurfaceCommandHelper("six","mesial",true,VoiceCommandAction.GoToToothSixMesialFacial),
				GoToToothWithSurfaceCommandHelper("seven","mesial",true,VoiceCommandAction.GoToToothSevenMesialFacial),
				GoToToothWithSurfaceCommandHelper("eight","mesial",true,VoiceCommandAction.GoToToothEightMesialFacial),
				GoToToothWithSurfaceCommandHelper("nine","mesial",true,VoiceCommandAction.GoToToothNineMesialFacial),
				GoToToothWithSurfaceCommandHelper("ten","mesial",true,VoiceCommandAction.GoToToothTenMesialFacial),
				GoToToothWithSurfaceCommandHelper("eleven","mesial",true,VoiceCommandAction.GoToToothElevenMesialFacial),
				GoToToothWithSurfaceCommandHelper("twelve","mesial",true,VoiceCommandAction.GoToToothTwelveMesialFacial),
				GoToToothWithSurfaceCommandHelper("thirteen","mesial",true,VoiceCommandAction.GoToToothThirteenMesialFacial),
				GoToToothWithSurfaceCommandHelper("fourteen","mesial",true,VoiceCommandAction.GoToToothFourteenMesialFacial),
				GoToToothWithSurfaceCommandHelper("fifteen","mesial",true,VoiceCommandAction.GoToToothFifteenMesialFacial),
				GoToToothWithSurfaceCommandHelper("sixteen","mesial",true,VoiceCommandAction.GoToToothSixteenMesialFacial),
				GoToToothWithSurfaceCommandHelper("seventeen","mesial",true,VoiceCommandAction.GoToToothSeventeenMesialFacial),
				GoToToothWithSurfaceCommandHelper("eighteen","mesial",true,VoiceCommandAction.GoToToothEighteenMesialFacial),
				GoToToothWithSurfaceCommandHelper("nineteen","mesial",true,VoiceCommandAction.GoToToothNineteenMesialFacial),
				GoToToothWithSurfaceCommandHelper("twenty","mesial",true,VoiceCommandAction.GoToToothTwentyMesialFacial),
				GoToToothWithSurfaceCommandHelper("twenty one","mesial",true,VoiceCommandAction.GoToToothTwentyOneMesialFacial),
				GoToToothWithSurfaceCommandHelper("twenty two","mesial",true,VoiceCommandAction.GoToToothTwentyTwoMesialFacial),
				GoToToothWithSurfaceCommandHelper("twenty three","mesial",true,VoiceCommandAction.GoToToothTwentyThreeMesialFacial),
				GoToToothWithSurfaceCommandHelper("twenty four","mesial",true,VoiceCommandAction.GoToToothTwentyFourMesialFacial),
				GoToToothWithSurfaceCommandHelper("twenty five","mesial",true,VoiceCommandAction.GoToToothTwentyFiveMesialFacial),
				GoToToothWithSurfaceCommandHelper("twenty six","mesial",true,VoiceCommandAction.GoToToothTwentySixMesialFacial),
				GoToToothWithSurfaceCommandHelper("twenty seven","mesial",true,VoiceCommandAction.GoToToothTwentySevenMesialFacial),
				GoToToothWithSurfaceCommandHelper("twenty eight","mesial",true,VoiceCommandAction.GoToToothTwentyEightMesialFacial),
				GoToToothWithSurfaceCommandHelper("twenty nine","mesial",true,VoiceCommandAction.GoToToothTwentyNineMesialFacial),
				GoToToothWithSurfaceCommandHelper("thirty","mesial",true,VoiceCommandAction.GoToToothThirtyMesialFacial),
				GoToToothWithSurfaceCommandHelper("thirty one","mesial",true,VoiceCommandAction.GoToToothThirtyOneMesialFacial),
				GoToToothWithSurfaceCommandHelper("thirty two","mesial",true,VoiceCommandAction.GoToToothThirtyTwoMesialFacial),
				GoToToothWithSurfaceCommandHelper("one","mesial",false,VoiceCommandAction.GoToToothOneMesialLingual),
				GoToToothWithSurfaceCommandHelper("two","mesial",false,VoiceCommandAction.GoToToothTwoMesialLingual),
				GoToToothWithSurfaceCommandHelper("three","mesial",false,VoiceCommandAction.GoToToothThreeMesialLingual),
				GoToToothWithSurfaceCommandHelper("four","mesial",false,VoiceCommandAction.GoToToothFourMesialLingual),
				GoToToothWithSurfaceCommandHelper("five","mesial",false,VoiceCommandAction.GoToToothFiveMesialLingual),
				GoToToothWithSurfaceCommandHelper("six","mesial",false,VoiceCommandAction.GoToToothSixMesialLingual),
				GoToToothWithSurfaceCommandHelper("seven","mesial",false,VoiceCommandAction.GoToToothSevenMesialLingual),
				GoToToothWithSurfaceCommandHelper("eight","mesial",false,VoiceCommandAction.GoToToothEightMesialLingual),
				GoToToothWithSurfaceCommandHelper("nine","mesial",false,VoiceCommandAction.GoToToothNineMesialLingual),
				GoToToothWithSurfaceCommandHelper("ten","mesial",false,VoiceCommandAction.GoToToothTenMesialLingual),
				GoToToothWithSurfaceCommandHelper("eleven","mesial",false,VoiceCommandAction.GoToToothElevenMesialLingual),
				GoToToothWithSurfaceCommandHelper("twelve","mesial",false,VoiceCommandAction.GoToToothTwelveMesialLingual),
				GoToToothWithSurfaceCommandHelper("thirteen","mesial",false,VoiceCommandAction.GoToToothThirteenMesialLingual),
				GoToToothWithSurfaceCommandHelper("fourteen","mesial",false,VoiceCommandAction.GoToToothFourteenMesialLingual),
				GoToToothWithSurfaceCommandHelper("fifteen","mesial",false,VoiceCommandAction.GoToToothFifteenMesialLingual),
				GoToToothWithSurfaceCommandHelper("sixteen","mesial",false,VoiceCommandAction.GoToToothSixteenMesialLingual),
				GoToToothWithSurfaceCommandHelper("seventeen","mesial",false,VoiceCommandAction.GoToToothSeventeenMesialLingual),
				GoToToothWithSurfaceCommandHelper("eighteen","mesial",false,VoiceCommandAction.GoToToothEighteenMesialLingual),
				GoToToothWithSurfaceCommandHelper("nineteen","mesial",false,VoiceCommandAction.GoToToothNineteenMesialLingual),
				GoToToothWithSurfaceCommandHelper("twenty","mesial",false,VoiceCommandAction.GoToToothTwentyMesialLingual),
				GoToToothWithSurfaceCommandHelper("twenty one","mesial",false,VoiceCommandAction.GoToToothTwentyOneMesialLingual),
				GoToToothWithSurfaceCommandHelper("twenty two","mesial",false,VoiceCommandAction.GoToToothTwentyTwoMesialLingual),
				GoToToothWithSurfaceCommandHelper("twenty three","mesial",false,VoiceCommandAction.GoToToothTwentyThreeMesialLingual),
				GoToToothWithSurfaceCommandHelper("twenty four","mesial",false,VoiceCommandAction.GoToToothTwentyFourMesialLingual),
				GoToToothWithSurfaceCommandHelper("twenty five","mesial",false,VoiceCommandAction.GoToToothTwentyFiveMesialLingual),
				GoToToothWithSurfaceCommandHelper("twenty six","mesial",false,VoiceCommandAction.GoToToothTwentySixMesialLingual),
				GoToToothWithSurfaceCommandHelper("twenty seven","mesial",false,VoiceCommandAction.GoToToothTwentySevenMesialLingual),
				GoToToothWithSurfaceCommandHelper("twenty eight","mesial",false,VoiceCommandAction.GoToToothTwentyEightMesialLingual),
				GoToToothWithSurfaceCommandHelper("twenty nine","mesial",false,VoiceCommandAction.GoToToothTwentyNineMesialLingual),
				GoToToothWithSurfaceCommandHelper("thirty","mesial",false,VoiceCommandAction.GoToToothThirtyMesialLingual),
				GoToToothWithSurfaceCommandHelper("thirty one","mesial",false,VoiceCommandAction.GoToToothThirtyOneMesialLingual),
				GoToToothWithSurfaceCommandHelper("thirty two","mesial",false,VoiceCommandAction.GoToToothThirtyTwoMesialLingual),
			#endregion
				new VoiceCommand {
					Commands=new List<string> {
						"probing"
					},
					ActionToPerform=VoiceCommandAction.Probing,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"Muco Gingival Junction",
						"MGJ"
					},
					ActionToPerform=VoiceCommandAction.MucoGingivalJunction,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="MGJ"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"Gingival Margin"
					},
					ActionToPerform=VoiceCommandAction.GingivalMargin,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"Furcation"
					},
					ActionToPerform=VoiceCommandAction.Furcation,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"mobility"
					},
					ActionToPerform=VoiceCommandAction.Mobility,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"plus one"
					},
					ActionToPerform=VoiceCommandAction.PlusOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"plus two"
					},
					ActionToPerform=VoiceCommandAction.PlusTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"plus three"
					},
					ActionToPerform=VoiceCommandAction.PlusThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"plus four"
					},
					ActionToPerform=VoiceCommandAction.PlusFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"plus five"
					},
					ActionToPerform=VoiceCommandAction.PlusFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"plus six"
					},
					ActionToPerform=VoiceCommandAction.PlusSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"plus seven"
					},
					ActionToPerform=VoiceCommandAction.PlusSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"plus eight"
					},
					ActionToPerform=VoiceCommandAction.PlusEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"plus nine"
					},
					ActionToPerform=VoiceCommandAction.PlusNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth one"
					},
					ActionToPerform=VoiceCommandAction.SkipToothOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth one skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth two"
					},
					ActionToPerform=VoiceCommandAction.SkipToothTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth two skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth three"
					},
					ActionToPerform=VoiceCommandAction.SkipToothThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth three skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth four"
					},
					ActionToPerform=VoiceCommandAction.SkipToothFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth four skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth five"
					},
					ActionToPerform=VoiceCommandAction.SkipToothFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth five skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth six"
					},
					ActionToPerform=VoiceCommandAction.SkipToothSix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth six skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth seven"
					},
					ActionToPerform=VoiceCommandAction.SkipToothSeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth seven skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth eight"
					},
					ActionToPerform=VoiceCommandAction.SkipToothEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth eight skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth nine"
					},
					ActionToPerform=VoiceCommandAction.SkipToothNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth nine skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth ten"
					},
					ActionToPerform=VoiceCommandAction.SkipToothTen,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth ten skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth eleven"
					},
					ActionToPerform=VoiceCommandAction.SkipToothEleven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth eleven skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth twelve"
					},
					ActionToPerform=VoiceCommandAction.SkipToothTwelve,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth twelve skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth thirteen"
					},
					ActionToPerform=VoiceCommandAction.SkipToothThirteen,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth thirteen skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth fourteen"
					},
					ActionToPerform=VoiceCommandAction.SkipToothFourteen,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth fourteen skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth fifteen"
					},
					ActionToPerform=VoiceCommandAction.SkipToothFifteen,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth fifteen skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth sixteen"
					},
					ActionToPerform=VoiceCommandAction.SkipToothSixteen,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth sixteen skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth seventeen"
					},
					ActionToPerform=VoiceCommandAction.SkipToothSeventeen,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth seventeen skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth eighteen"
					},
					ActionToPerform=VoiceCommandAction.SkipToothEighteen,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth eighteen skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth nineteen"
					},
					ActionToPerform=VoiceCommandAction.SkipToothNineteen,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth nineteen skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth twenty"
					},
					ActionToPerform=VoiceCommandAction.SkipToothTwenty,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth twenty skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth twenty one"
					},
					ActionToPerform=VoiceCommandAction.SkipToothTwentyOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth twenty one skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth twenty two"
					},
					ActionToPerform=VoiceCommandAction.SkipToothTwentyTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth twenty two skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth twenty three"
					},
					ActionToPerform=VoiceCommandAction.SkipToothTwentyThree,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth twenty three skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth twenty four"
					},
					ActionToPerform=VoiceCommandAction.SkipToothTwentyFour,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth twenty four skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth twenty five"
					},
					ActionToPerform=VoiceCommandAction.SkipToothTwentyFive,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth twenty five skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth twenty six"
					},
					ActionToPerform=VoiceCommandAction.SkipToothTwentySix,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth twenty six skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth twenty seven"
					},
					ActionToPerform=VoiceCommandAction.SkipToothTwentySeven,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth twenty seven skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth twenty eight"
					},
					ActionToPerform=VoiceCommandAction.SkipToothTwentyEight,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth twenty eight skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth twenty nine"
					},
					ActionToPerform=VoiceCommandAction.SkipToothTwentyNine,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth twenty nine skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth thirty"
					},
					ActionToPerform=VoiceCommandAction.SkipToothThirty,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth thirty skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth thirty one"
					},
					ActionToPerform=VoiceCommandAction.SkipToothThirtyOne,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth thirty one skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip tooth thirty two"
					},
					ActionToPerform=VoiceCommandAction.SkipToothThirtyTwo,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth thirty two skipped"
				},
				new VoiceCommand {
					Commands=new List<string> {
						"skip this tooth",
						"skip current tooth"
					},
					ActionToPerform=VoiceCommandAction.SkipCurrentTooth,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
					Response="Tooth skipped"
				},
			#endregion PerioChart
			#region VoiceMsgBox
			new VoiceCommand {
					Commands=new List<string> {
						"yes",
					},
					ActionToPerform=VoiceCommandAction.Yes,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.VoiceMsgBox }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"no",
					},
					ActionToPerform=VoiceCommandAction.No,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.VoiceMsgBox }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"okay",
					},
					ActionToPerform=VoiceCommandAction.Ok,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.VoiceMsgBox }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"cancel",
					},
					ActionToPerform=VoiceCommandAction.Cancel,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.VoiceMsgBox }
				},
				new VoiceCommand {
					Commands=new List<string> {
						"resume path",
					},
					ActionToPerform=VoiceCommandAction.ResumePath,
					ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart }
				},
			#endregion VoiceMsgBox
		};

		///<summary>Gets all the commands used for the given areas of the program.</summary>
		public static List<VoiceCommand> GetCommands(List<VoiceCommandArea> listAreas) {
			return _commands.FindAll(x => x.ListAreas.Any(y => ListTools.In(y,listAreas)));
		}

		///<summary>Returns a new Go To Tooth voice command for the toothnum and voice command action passed in.</summary>
		private static VoiceCommand GoToToothCommandHelper(string toothNum,VoiceCommandAction action) {
			return new VoiceCommand {
				Commands=new List<string> {
						$"go to tooth {toothNum.ToLower()}",
						$"go to {toothNum.ToLower()}",
						$"select tooth {toothNum.ToLower()}",
						$"select {toothNum.ToLower()}"
					},
				ActionToPerform=action,
				ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
				Response=$"Tooth {toothNum.ToLower()}"
			};
		}

		///<summary>Returns a new Go To Tooth voice command for the toothnum, position, and voice command action passed in.</summary>
		private static VoiceCommand GoToToothWithSurfaceXCommandHelper(string toothNum,string surface,VoiceCommandAction action) {
			return new VoiceCommand {
				Commands=new List<string> {
						$"go to tooth {toothNum.ToLower()} {surface}",
						$"go to {toothNum.ToLower()} {surface}",
						$"select tooth {toothNum.ToLower()} {surface}",
						$"select {toothNum.ToLower()} {surface}"
					},
				ActionToPerform=action,
				ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
				Response=$"Tooth {toothNum.ToLower()} {surface}"
			};
		}

		///<summary>Returns a new Go To Tooth voice command for the toothnum, surface, position, and voice command action passed in.</summary>
		private static VoiceCommand GoToToothWithSurfaceCommandHelper(string toothNum,string surface,bool isFacial,VoiceCommandAction action) {
			string strPos=(isFacial ? "facial" : "lingual");
			return new VoiceCommand {
				Commands=new List<string> {
						$"go to tooth {toothNum.ToLower()} {surface} {strPos}",
						$"go to {toothNum.ToLower()} {surface} {strPos}",
						$"select tooth {toothNum.ToLower()} {surface} {strPos}",
						$"select {toothNum.ToLower()} {surface} {strPos}",
						$"go to tooth {toothNum.ToLower()} {strPos} {surface}",
						$"go to {toothNum.ToLower()} {strPos} {surface}",
						$"select tooth {toothNum.ToLower()} {strPos} {surface}",
						$"select {toothNum.ToLower()} {strPos} {surface}"
					},
				ActionToPerform=action,
				ListAreas=new List<VoiceCommandArea> { VoiceCommandArea.PerioChart },
				Response=$"Tooth {toothNum.ToLower()} {strPos} {surface}"
			};
		}
	}
}
