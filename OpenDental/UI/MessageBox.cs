using CodeBase;

namespace OpenDental {
	///<summary>Purposefully overrides or hides System.Windows.Forms.MessageBox from the entire OpenDental namespace by extending ODMessageBox.
	///This is so that we can inject our own code prior to System.Windows.Forms.MessageBox.Show().
	///This is necessary due to the fact that a progress window (or our splash screen) could be showing to a user and a question might need to be asked.
	///Since all of our progress windows are UIs that are owned by a separate thread, we need to invoke over to the progress window and ask it
	///to act as the "owner" of the dialog that we are about to show to the user so that it always shows on top of the progress window.</summary>
	public class MessageBox : ODMessageBox {

	}
}
