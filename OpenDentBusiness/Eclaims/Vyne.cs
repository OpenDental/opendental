using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace OpenDentBusiness.Eclaims
{
    ///<summary>Summary description for Vyne.</summary>
    public class Vyne
    {

        #region Endpoints

        private static string AuthEndpoint = $@"https://rl7.rss-llc.com/rlclient/prodng/login";
        private static string AuthLinkEndpoint = $@"https://rl7.rss-llc.com/rlclient/prodng/getauthorizedlink";
        private static string SubmitEndpoint = $@"https://rl7integrations.rss-llc.com/upload/prodng/upload_x12";

        #endregion

        private static JavaScriptSerializer _jsSerializer = new JavaScriptSerializer();
        private static VyneAuthenticationResponse _trellisAuthInfo { get; set; }
        private static Clearinghouse _clearinghouseClin;

        public static string ErrorMessage { get; set; }
        public static string Password { get; private set; }

        public delegate string[][] FillVyneDelegate(long claimNum, long patNum, List<ClaimFormItem> listItems);

        ///<summary>Vyne Trellis Integration.</summary>
        public Vyne() { }

        /// <summary>
        /// Open an authenticated Trellis browser window
        /// </summary>
        /// <param name="clearinghouseClin"></param>
        public static void OpenSite(Clearinghouse clearinghouseClin)
        {
            try
            {
                var link = _trellisAuthInfo.rPracticeUrl;

                if (!string.IsNullOrEmpty(link))
                {
                    link = link.Replace("\"", "") + "&machinename=" + Environment.MachineName;
                    Process.Start(link);
                }
                else
                {
                    MessageBox.Show("Could not open Vyne Trellis");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Launch:
        ///		Login, save auth object
        ///		Process X12 and submit to Vyne
        /// </summary>
        /// <param name="clearinghouseClin"></param>
        /// <param name="batchNum"></param>
        /// <returns></returns>
        public static bool Launch(Clearinghouse clearinghouseClin, int batchNum)
        {
            var success = false;

            _clearinghouseClin = clearinghouseClin;

            try
            {
                var loginStatus = Login(clearinghouseClin.LoginID, clearinghouseClin.Password);

                if (loginStatus)
                {
                    ProcessLocalX12Files(clearinghouseClin);
                    success = true;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                x837Controller.Rollback(clearinghouseClin, batchNum);
            }

            return success;
        }

        /// <summary>
        /// Authenticate against Vyne Trellis
        /// </summary>
        public static bool Login(string username, string password)
        {
            var result = false;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new Exception("Username or Password cannot be empty.\nUpdate the username and password fields in clearinghouse settings.");
            }

            try
            {
                var user = new { Username = username, Password = password };
                var json = _jsSerializer.Serialize(user);

                var client = new WebClient();
                client.Headers.Add("Content-Type:application/json");

                var response = client.UploadString($"{AuthEndpoint}", json);

                if (!string.IsNullOrEmpty(response))
                {
                    _trellisAuthInfo = JsonConvert.DeserializeObject<VyneAuthenticationResponse>(response);

                    if (!string.IsNullOrEmpty(_trellisAuthInfo?.AuthToken))
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error logging into Vyne. Please try again. Details: \n" + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Load any files from the export path and submit them
        /// </summary>
        /// <param name="clearinghouseClin"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public static bool ProcessLocalX12Files(Clearinghouse clearinghouseClin)
        {
            var result = false;

            if (_trellisAuthInfo == null || string.IsNullOrEmpty(_trellisAuthInfo.AuthToken))
            {
                throw new ApplicationException("Could not process X12 files. Not authenticated.");
            }

            var filePath = clearinghouseClin.ExportPath;
            var fileNames = Directory.GetFiles(filePath);

            if (fileNames.Length > 1)
            {
                for (int f = 0; f < fileNames.Length; f++)
                {
                    File.Delete(fileNames[f]);
                }
                Directory.Delete(filePath);
                throw new ApplicationException("A previous batch submission was found in an incomplete state.  You will need to resubmit your most recent batch as well as this batch.  Also check reports to be certain that all expected claims went through.");
            }

            if (fileNames.Length == 0)
            {
                throw new ApplicationException("There were no files to process.");
            }

            var fileName = fileNames[0];
            var fileText = File.ReadAllText(fileName);

            var req = new VyneSubmissionRequest()
            {
                X12 = Convert.ToBase64String(Encoding.ASCII.GetBytes(fileText)),
                Version = Application.ProductVersion.ToString(),
                Username = Convert.ToBase64String(Encoding.ASCII.GetBytes(clearinghouseClin.LoginID)),
                Password = Convert.ToBase64String(Encoding.ASCII.GetBytes(clearinghouseClin.Password))
            };

            var json = _jsSerializer.Serialize(req);

            var client = new WebClient();
            client.Headers.Add("Content-Type:application/json");
            client.QueryString = new System.Collections.Specialized.NameValueCollection() { };
            client.QueryString.Add("customerId", _trellisAuthInfo.AuthToken);
            var response = client.UploadString($"{SubmitEndpoint}", json);

            if (string.IsNullOrEmpty(response))
            {
                File.Delete(fileName);
                result = true;
            }
            else
            {
                ErrorMessage = "Could not submit the claim to Vyne Trellis";
                throw new ApplicationException(ErrorMessage);
            }

            return result;
        }
    }

    /// <summary>
    /// Object to store the authentication response
    /// </summary>
    public class VyneAuthenticationResponse
    {
        public string JwtTokens { get; set; }
        public string AuthenticationUrl { get; set; }
        public string rPracticeUrl { get; set; }
        public string AuthToken { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string PasswordHash { get; set; }
        public List<string> Errors { get; set; }
        public string RegistrationToken { get; set; }
    }

    /// <summary>
    /// X12 object to submit the claim
    /// </summary>
    public class VyneSubmissionRequest
    {
        public string X12 { get; set; }
        public string Version { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
