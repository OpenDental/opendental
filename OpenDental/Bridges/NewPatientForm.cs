/*using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using OpenDentBusiness;
using OpenDental.Imaging;

namespace OpenDental.Bridges
{
		///<summary></summary>
    public partial class NewPatientForm : Form
    {

        private string sURL = "";
				///<summary></summary>
        public NewPatientForm()
        {
            InitializeComponent();

        }


        private void AddResults(string sResult)
        {
            txtResults.Text = sResult + "\r\n" + txtResults.Text;
            txtResults.Refresh();
        }

        private void NewPatientForm_Load(object sender, EventArgs e)
        {

        }
				///<summary></summary>
        public void ShowDownload(string sURLValue)
        {
            sURL = sURLValue;
            this.Show();
        }

        private void btnImportForms_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            
            XmlDocument x = new XmlDocument();
            AddResults("Downloading new patient list...");
            try
            {
                x.Load(sURL);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot access account.\r\n\r\nPlease make sure URL, username and password are correct.\r\n\r\nContact www.NewPatientForm.com for help.\r\n\r\n" + ex.Message);
                this.Close();
                return;
            }

            if (x.DocumentElement.ChildNodes.Count == 0)
            {
                MessageBox.Show("No new patients.");
                this.Close();
                return;
            }

            //Now that we have loaded all the new patient forms, loop through
            //each patient, import the xml and store the pdf file
            foreach (XmlNode ndeMessage in x.DocumentElement.ChildNodes)
            {
                string sPatientName = "";

                try
                {
                    sPatientName += ndeMessage.SelectSingleNode("PatientIdentification/NameLast").InnerText;
                }
                catch
                {
                    AddResults("No lastname found.");
                }

                try
                {
                    sPatientName += ", " + ndeMessage.SelectSingleNode("PatientIdentification/NameFirst").InnerText;
                }
                catch
                {
                    AddResults("No firstname found.");
                }

                if (MessageBox.Show("Do you want to import information for " + sPatientName + " ?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    AddResults("Adding patient " + sPatientName);

                    string sPDF = "";

                    try
                    {
                        sPDF = ndeMessage.SelectSingleNode("PatientIdentification/NewPatientForm").InnerText;
                    }
                    catch
                    {
                        AddResults("No pdf form found.");
                    }

                    //We have the encoded pdf in sPDF string so lets
                    //delete that node and try to import the patient
                    ndeMessage.SelectSingleNode("PatientIdentification").RemoveChild(ndeMessage.SelectSingleNode("PatientIdentification/NewPatientForm"));

                    FormImportXML frmX = new FormImportXML();
                    frmX.textMain.Text = ndeMessage.OuterXml;
                    frmX.butOK_Click(null, null);


                    //The patient info is entered, let's save the pdf document to the images folder

                    try
                    {
                        //We'll be working with a document

                        //First make sure we have a directory and
                        //everything is up to date
                        ContrDocs cd = new ContrDocs();

                        cd.RefreshModuleData(frmX.existingPatOld.PatNum);

						try {
							cd.imageStore.ImportPdf(sPDF);
						}
						catch{		
							MessageBox.Show(Lan.g(this, "Unable to write pdf file to disk."));
						}
                    }
                    catch
                    {
                        AddResults("Could not save pdf file to patient's file.");
                    }

                    AddResults("Done writing pdf file to disk");

                }
                else
                {
                    AddResults("Cacelled import for " + sPatientName + ".");
                }


            }
            this.Cursor = Cursors.Default;
            MessageBox.Show("Import complete.\r\n\r\nIf any form imports were cancelled or unsuccessful\r\nthey will need to be imported manually.");

            btnImportForms.Enabled = false;
            //clear form instanciations
           
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}*/