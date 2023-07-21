using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Linq;
using UnitTestsCore;

namespace UnitTests.UnitTests
{
    [TestClass]
    public class PromotionTests {

        [TestMethod]
        public void Promotions_FilterForDuplicates_NoDuplicates() {
            List<MassEmailDestination> destinationListBefore = new List<MassEmailDestination>() {
                new MassEmailDestination() {
                    AptNum=1,
                    PatNum=1,
                    ToAddress="testEmail1@mail.com"
                },
                new MassEmailDestination() {
                    AptNum=1,
                    PatNum=2,
                    ToAddress="testEmail2@mail.com"
                },
            };
            List<MassEmailDestination> destinationListAfter = Promotions.FilterForDuplicates(destinationListBefore,new Dictionary<long,Patient>(),PromotionType.Manual);
            Assert.AreEqual(destinationListAfter.Count,destinationListBefore.Count);
            for(int i = 0;i<destinationListBefore.Count;i++) {
                Assert.AreEqual(destinationListBefore[i].PatNum,destinationListAfter[i].PatNum);
            }
        }

        [TestMethod]
        public void Promotions_FilterForDuplicates_BirthdayEmails() { 
            string emailAddress="testAddress01@mail.com";
            List<MassEmailDestination> destinationListBefore=new List<MassEmailDestination>() { 
               new MassEmailDestination() {
                    AptNum=1,
                    PatNum=1,
                    ToAddress=emailAddress
               },
               new MassEmailDestination() { 
                   AptNum=1,
                   PatNum=2,
                   ToAddress=emailAddress
               }
            };
            List<MassEmailDestination> destinationListAfter=Promotions.FilterForDuplicates(destinationListBefore,null,PromotionType.Birthday);
            for(int i=0;i<destinationListAfter.Count;i++) { 
                Assert.AreEqual(destinationListAfter[i].PatNum,destinationListBefore[i].PatNum);            
            }
            Assert.AreEqual(destinationListAfter.Count,destinationListBefore.Count);
        }

        [TestMethod]
        public void Promotions_FilterForDuplicates_NoGuarantor() { 
            string emailAddress="testAddress01@mail.com";
            Patient patDad=PatientT.CreatePatient("Smith",email:emailAddress);
            Patient patMom=PatientT.CreatePatient("Smith",email:emailAddress);
            Patient patSue=PatientT.CreatePatient("Smith",email:emailAddress);
            List<MassEmailDestination> destinationList=new List<MassEmailDestination>() {
                new MassEmailDestination() {
                    AptNum=1,
                    PatNum=patDad.PatNum,
                    ToAddress=emailAddress
				},
                new MassEmailDestination() {
                    AptNum=1,
                    PatNum=patMom.PatNum,
                    ToAddress=emailAddress
				},
                new MassEmailDestination() {
                    AptNum=1,
                    PatNum=patSue.PatNum,
                    ToAddress=emailAddress
				}
			};
            destinationList=Promotions.FilterForDuplicates(destinationList,new Dictionary<long, Patient>(),PromotionType.Manual);
            Assert.AreEqual(1,destinationList.Count);
            Assert.AreEqual(patDad.PatNum,destinationList.First().PatNum);
        }

        [TestMethod]
        public void Promotions_FilterForDuplicates_HasGuarantor() {
            string emailAddress = "testAddress01@mail.com";
            Patient patDad = PatientT.CreatePatient("Smith",email: emailAddress);
            Patient patMom = PatientT.CreatePatient("Smith",email: emailAddress);
            Patient patSue = PatientT.CreatePatient("Smith",email: emailAddress);
            List<MassEmailDestination> destinationList = new List<MassEmailDestination>() {
                new MassEmailDestination() {
                    AptNum=1,
                    PatNum=patDad.PatNum,
                    ToAddress=emailAddress
                },
                new MassEmailDestination() {
                    AptNum=1,
                    PatNum=patMom.PatNum,
                    ToAddress=emailAddress
                },
                new MassEmailDestination() {
                    AptNum=1,
                    PatNum=patSue.PatNum,
                    ToAddress=emailAddress
                }
            };
            Dictionary<long,Patient> dictGuarantor=new Dictionary<long,Patient>() {
                { patMom.PatNum,patMom },
			};
            destinationList=Promotions.FilterForDuplicates(destinationList,dictGuarantor,PromotionType.Manual);
            Assert.AreEqual(1,destinationList.Count);
            Assert.AreEqual(patMom.PatNum,destinationList.First().PatNum);
        }
    }
}
