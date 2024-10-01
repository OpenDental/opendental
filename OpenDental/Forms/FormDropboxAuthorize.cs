using System;
using System.Windows.Forms;
using OpenDentBusiness;
using WebServiceSerializer;

namespace OpenDental {
	public partial class FormDropboxAuthorize:FormODBase {
		
		public ProgramProperty ProgramPropertyAccessToken;

		public FormDropboxAuthorize() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDropboxAuthorize_Load(object sender,EventArgs e) {
			try {
				string regKey=PrefC.GetString(PrefName.RegistrationKey);
				IWebServiceMainHQ iWebServiceMainHQ=WebServiceMainHQProxy.GetWebServiceMainHQInstance();
				string urlPrimitive=iWebServiceMainHQ.BuildOAuthUrl(regKey,OAuthApplicationNames.Dropbox.ToString());
				//In OpenDentalWebApps, see WebServiceMainHQ.asmx.cs, BuildOAuthUrl().
				string url=WebSerializer.DeserializePrimitiveOrThrow<string>(urlPrimitive);
				System.Diagnostics.Process.Start(url);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error:")+"  "+ex.Message);
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			try {
				string accessTokenFinal=WebSerializer.DeserializePrimitiveOrThrow<string>(
					WebServiceMainHQProxy.GetWebServiceMainHQInstance().GetDropboxAccessToken(WebSerializer.SerializePrimitive<string>(textAccessToken.Text)));
				ProgramPropertyAccessToken.PropertyValue=accessTokenFinal;
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error:")+"  "+ex.Message);
				return;
			}
			ProgramProperties.Update(ProgramPropertyAccessToken);
			DataValid.SetInvalid(InvalidType.Programs);
			DialogResult=DialogResult.OK;
		}

	}
}