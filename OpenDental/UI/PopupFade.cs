namespace OpenDental {
	///<summary>This is message box that will fade away after a brief waiting period.</summary>
	public class PopupFade{
		
		///<summary>This is message box that will fade away after a brief waiting period.</summary>
		public static void Show(object sender,string text,bool doShowCloseButton=true,bool doTranslate=true){
			FormPopupFade.ShowMessage(sender,text,doShowCloseButton,doTranslate);
		}
	}
}