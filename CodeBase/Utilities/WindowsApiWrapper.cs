using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CodeBase {

	public static class WindowsApiWrapper {
		//This file is for all windows Desktop App API elements. It can be added to as needed.
		//http://www.pinvoke.net/
		//http://www.pinvoke.net/search.aspx?search=WM.USER&namespace=[All]

		private const int WM_USER=0x0400;
    private const uint SB_SETPARTS=WM_USER+4;
    private const uint SB_GETPARTS=WM_USER+6;
    private const uint SB_GETTEXTLENGTH=WM_USER+12;
    private const uint SB_GETTEXT=WM_USER+13;
		public const int SCF_SELECTION=0x0001;
		public const int EC_LEFTMARGIN=0x0001;
		public const int EC_RIGHTMARGIN=0x0002;

		#region Windows Enums
		///<summary>After the window has been created, these styles cannot be modified, except as noted.
		/// https://msdn.microsoft.com/en-us/library/windows/desktop/ms632600(v=vs.85).aspx </summary>
		public enum WinStyles:long {
			///<summary>The window has a thin-line border.</summary>
			WS_BORDER=0x00800000L,
			///<summary>The window has a title bar (includes the WS_BORDER style).</summary>
			WS_CAPTION=0x00C00000L,
			///<summary>The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the WS_POPUP style.</summary>
			WS_CHILD=0x40000000L,
			///<summary>Same as the WS_CHILD style.</summary>
			WS_CHILDWINDOW=0x40000000L,
			///<summary>Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window.</summary>
			WS_CLIPCHILDREN=0x02000000L,
			///<summary>Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message,
			///the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated.
			///If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child window,
			///to draw within the client area of a neighboring child window.</summary>
			WS_CLIPSIBLINGS=0x04000000L,
			///<summary>The window is initially disabled. A disabled window cannot receive input from the user. To change this after a window has been created, use the EnableWindow function.</summary>
			WS_DISABLED=0x08000000L,
			///<summary>The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.</summary>
			WS_DLGFRAME=0x00400000L,
			///<summary>The window is the first control of a group of controls. 
			///The group consists of this first control and all controls defined after it, up to the next control with the WS_GROUP style. 
			///The first control in each group usually has the WS_TABSTOP style so that the user can move from group to group. 
			///The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys.
			///You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.</summary>
			WS_GROUP=0x00020000L,
			///<summary>The window has a horizontal scroll bar.</summary>
			WS_HSCROLL=0x00100000L,
			///<summary>The window is initially minimized. Same as the WS_MINIMIZE style.</summary>
			WS_ICONIC=0x20000000L,
			///<summary>The window is initially maximized.</summary>
			WS_MAXIMIZE=0x01000000L,
			///<summary>The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary>
			WS_MAXIMIZEBOX=0x00010000L,
			///<summary>The window is initially minimized. Same as the WS_ICONIC style.</summary>
			WS_MINIMIZE=0x20000000L,
			///<summary>The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary>
			WS_MINIMIZEBOX=0x00020000L,
			///<summary>The window is an overlapped window. An overlapped window has a title bar and a border. Same as the WS_TILED style.</summary>
			WS_OVERLAPPED=0x00000000L,
			///<summary>The window is an overlapped window. Same as the WS_TILEDWINDOW style.</summary>
			WS_OVERLAPPEDWINDOW=(WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX),
			///<summary>The windows is a pop-up window. This style cannot be used with the WS_CHILD style.</summary>
			WS_POPUP=0x80000000L,
			///<summary>The window is a pop-up window. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible.</summary>
			WS_POPUPWINDOW=(WS_POPUP | WS_BORDER | WS_SYSMENU),
			///<summary>The window has a sizing border. Same as the WS_THICKFRAME style.</summary>
			WS_SIZEBOX=0x00040000L,
			///<summary>The window has a window menu on its title bar. The WS_CAPTION style must also be specified.</summary>
			WS_SYSMENU=0x00080000L,
			///<summary>The window is a control that can receive the keyboard focus when the user presses the TAB key. 
			///Pressing the TAB key changes the keyboard focus to the next control with the WS_TABSTOP style.
			///You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function. 
			///For user-created windows and modeless dialogs to work with tab stops, alter the message loop to call the IsDialogMessage function.</summary>
			WS_TABSTOP=0x00010000L,
			///<summary>The window has a sizing border. Same as the WS_SIZEBOX style.</summary>
			WS_THICKFRAME=0x00040000L,
			///<summary>The window is an overlapped window. An overlapped window has a title bar and a border. Same as the WS_OVERLAPPED style.</summary>
			WS_TILED=0x00000000L,
			///<summary>The window is an overlapped window. Same as the WS_OVERLAPPEDWINDOW style.</summary>
			WS_TILEDWINDOW=(WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX),
			///<summary>The window is initially visible.
			///This style can be turned on and off by using the ShowWindow or SetWindowPos function.</summary>
			WS_VISIBLE=0x10000000L,
			///<summary>The window has a vertical scroll bar.</summary>
			WS_VSCROLL=0x00200000L,
		}

		///<summary>Retrieves the handle to the ancestor of the specified window.
		///https://msdn.microsoft.com/en-us/library/windows/desktop/ms633502(v=vs.85).aspx </summary>
		public enum WinGetAncestor {
			///<summary>Retrieves the parent window. This does not include the owner, as it does with the GetParent function.</summary>
			GA_PARENT=1,
			///<summary>Retrieves the root window by walking the chain of parent windows.</summary>
			GA_ROOT=2,
			///<summary>Retrieves the owned root window by walking the chain of parent and owner windows returned by GetParent.</summary>
			GA_ROOTOWNER=3,
		}

		///<summary>Retrieves the opacity and transparency color key of a layered window.
		///https://msdn.microsoft.com/en-us/library/windows/desktop/ms633508(v=vs.85).aspx </summary>
		public enum WinGetLayeredWindowAttributes {
			///<summary>Use pbAlpha to determine the opacity of the layered window.</summary>
			LWA_ALPHA=0x00000002,
			///<summary>Use pcrKey as the transparency color.</summary>
			LWA_COLORKEY=0x00000001,
		}
		
		///<summary>Retrieves a handle to the next or previous window in the Z-Order. The next window is below the specified window; the previous window is above.
		///https://msdn.microsoft.com/en-us/library/windows/desktop/ms633509(v=vs.85).aspx </summary>
		public enum WinGetNextWindow {
			///<summary>Returns a handle to the window below the given window.</summary>
			GW_HWNDNEXT=2,
			///<summary>Returns a handle to the window above the given window.</summary>
			GW_HWNDPREV=3,
		}

		///<summary>Retrieves the current color of the specified display element. Display elements are the parts of a window and the display that appear on the system display screen.
		///https://msdn.microsoft.com/en-us/library/windows/desktop/ms724371(v=vs.85).aspx </summary>
		public enum WinGetSysColor {
			///<summary>Dark shadow for three-dimensional display elements.<summary>
			COLOR_3DDKSHADOW=21,
			///<summary>Face color for three-dimensional display elements and for dialog box backgrounds.</summary>
			COLOR_3DFACE=15,
			///<summary>Highlight color for three-dimensional display elements (for edges facing the light source.)</summary>
			COLOR_3DHIGHLIGHT=20,
			///<summary>Highlight color for three-dimensional display elements (for edges facing the light source.)</summary>
			COLOR_3DHILIGHT=20,
			///<summary>Light color for three-dimensional display elements (for edges facing the light source.)</summary>
			COLOR_3DLIGHT=22,
			///<summary>Shadow color for three-dimensional display elements (for edges facing away from the light source).</summary>
			COLOR_3DSHADOW=16,
			///<summary>Active window border.</summary>
			COLOR_ACTIVEBORDER=10,
			///<summary>Active window title bar.
			///Specifies the left side color in the color gradient of an active window's title bar if the gradient effect is enabled.</summary>
			COLOR_ACTIVECAPTION=2,
			///<summary>Background color of multiple document interface (MDI) applications.</summary>
			COLOR_APPWORKSPACE=12,
			///<summary>Desktop.</summary>
			COLOR_BACKGROUND=1,
			///<summary>Face color for three-dimensional display elements and for dialog box backgrounds.</summary>
			COLOR_BTNFACE=15,
			///<summary>Highlight color for three-dimensional display elements (for edges facing the light source.)</summary>
			COLOR_BTNHIGHLIGHT=20,
			///<summary>Highlight color for three-dimensional display elements (for edges facing the light source.)</summary>
			COLOR_BTNHILIGHT=20,
			///<summary>Shadow color for three-dimensional display elements (for edges facing away from the light source).</summary>
			COLOR_BTNSHADOW=16,
			///<summary>Text on push buttons.</summary>
			COLOR_BTNTEXT=18,
			///<summary>Text in caption, size box, and scroll bar arrow box.</summary>
			COLOR_CAPTIONTEXT=9,
			///<summary>Desktop.</summary>
			COLOR_DESKTOP=1,
			///<summary>Right side color in the color gradient of an active window's title bar. COLOR_ACTIVECAPTION specifies the left side color. 
			///Use SPI_GETGRADIENTCAPTIONS with the SystemParametersInfo function to determine whether the gradient effect is enabled.</summary>
			COLOR_GRADIENTACTIVECAPTION=27,
			///<summary>Right side color in the color gradient of an inactive window's title bar. COLOR_INACTIVECAPTION specifies the left side color.</summary>
			COLOR_GRADIENTINACTIVECAPTION=28,
			///<summary>Grayed (disabled) text. This color is set to 0 if the current display driver does not support a solid gray color.</summary>
			COLOR_GRAYTEXT=17,
			///<summary>Item(s) selected in a control.</summary>
			COLOR_HIGHLIGHT=13,
			///<summary>Text of item(s) selected in a control.</summary>
			COLOR_HIGHLIGHTTEXT=14,
			///<summary>Color for a hyperlink or hot-tracked item.</summary>
			COLOR_HOTLIGHT=26,
			///<summary>Inactive window border.</summary>
			COLOR_INACTIVEBORDER=11,
			///<summary>Inactive window caption.
			///Specifies the left side color in the color gradient of an inactive window's title bar if the gradient effect is enabled.</summary>
			COLOR_INACTIVECAPTION=3,
			///<summary>Color of text in an inactive caption.</summary>
			COLOR_INACTIVECAPTIONTEXT=19,
			///<summary>Background color for tooltip controls.</summary>
			COLOR_INFOBK=24,
			///<summary>Text color for tooltip controls.</summary>
			COLOR_INFOTEXT=23,
			///<summary>Menu background.</summary>
			COLOR_MENU=4,
			///<summary>The color used to highlight menu items when the menu appears as a flat menu (see SystemParametersInfo). The highlighted menu item is outlined with COLOR_HIGHLIGHT.</summary>
			COLOR_MENUHILIGHT=29,
			///<summary>The background color for the menu bar when menus appear as flat menus (see SystemParametersInfo). However, COLOR_MENU continues to specify the background color of the menu popup.</summary>
			COLOR_MENUBAR=30,
			///<summary>Text in menus.</summary>
			COLOR_MENUTEXT=7,
			///<summary>Scroll bar gray area.</summary>
			COLOR_SCROLLBAR=0,
			///<summary>Window background.</summary>
			COLOR_WINDOW=5,
			///<summary>Window frame.</summary>
			COLOR_WINDOWFRAME=6,
			///<summary>Text in windows.</summary>
			COLOR_WINDOWTEXT=8,
		}

		///<summary>Retrieves a handle to a window that has the specified relationship (Z-Order or owner) to the specified window.
		///https://msdn.microsoft.com/en-us/library/windows/desktop/ms633515(v=vs.85).aspx </summary>
		public enum WinGetWindow {
			///<summary>The retrieved handle identifies the child window at the top of the Z order, if the specified window is a parent window; otherwise, 
			///the retrieved handle is NULL. The function examines only child windows of the specified window. It does not examine descendant windows</summary>
			GW_CHILD=5,
			///<summary>The retrieved handle identifies the enabled popup window owned by the specified window (the search uses the first such window found using GW_HWNDNEXT); 
			///otherwise, if there are no enabled popup windows, the retrieved handle is that of the specified window.</summary>
			GW_ENABLEDPOPUP=6,
			///<summary>The retrieved handle identifies the window of the same type that is highest in the Z order.
			///If the specified window is a topmost window, the handle identifies a topmost window. If the specified window is a top-level window, the handle identifies a top-level window. 
			///If the specified window is a child window, the handle identifies a sibling window.</summary>
			GW_HWNDFIRST=0,
			///<summary>The retrieved handle identifies the window of the same type that is lowest in the Z order.
			///If the specified window is a topmost window, the handle identifies a topmost window. If the specified window is a top-level window, the handle identifies a top-level window. 
			///If the specified window is a child window, the handle identifies a sibling window.</summary>
			GW_HWNDLAST=1,
			///<summary>The retrieved handle identifies the window below the specified window in the Z order.
			///If the specified window is a topmost window, the handle identifies a topmost window. If the specified window is a top-level window, the handle identifies a top-level window. 
			///If the specified window is a child window, the handle identifies a sibling window.</summary>
			GW_HWNDNEXT=2,
			///<summary>The retrieved handle identifies the window above the specified window in the Z order.
			///If the specified window is a topmost window, the handle identifies a topmost window. If the specified window is a top-level window, the handle identifies a top-level window.
			///If the specified window is a child window, the handle identifies a sibling window.</summary>
			GW_HWNDPREV=3,
			///<summary>The retrieved handle identifies the specified window's owner window, if any. For more information, see Owned Windows.</summary>
			GW_OWNER=4,
		}

		///<summary>Constants for specific windows messages 
		///https://msdn.microsoft.com/en-us/library/windows/desktop/ff468921(v=vs.85).aspx </summary>
		public enum WinMessages {
			///<summary>Retrieves the menu handle for the current window.</summary>
			MN_GETHMENU=0x01E1,
			///<summary>Sent when the window background must be erased (for example, when a window is resized). The message is sent to prepare an invalidated portion of a window for painting.</summary>
			WM_ERASEBKGND=0x0014,
			///<summary>Retrieves the font with which the control is currently drawing its text.</summary>
			WM_GETFONT=0x0031,
			///<summary>Copies the text that corresponds to a window into a buffer provided by the caller.</summary>
			WM_GETTEXT=0x000D,
			///<summary>Determines the length, in characters, of the text associated with a window.</summary>
			WM_GETTEXTLENGTH=0x000E,
			///<summary>Sets the font that a control is to use when drawing text.</summary>
			WM_SETFONT=0x0030,
			///<summary>Associates a new large or small icon with a window. The system displays the large icon in the ALT+TAB dialog box, and the small icon in the window caption.</summary>
			WM_SETICON=0x0080,
			///<summary>Sets the text of a window.</summary>
			WM_SETTEXT=0x000C,
		}

		///<summary>Constants for specific windows notifications
		///https://msdn.microsoft.com/en-us/library/windows/desktop/ff468922(v=vs.85).aspx </summary>
		public enum WinNotifications {
			///<summary>Sent when a window belonging to a different application than the active window is about to be activated. 
			///The message is sent to the application whose window is being activated and to the application whose window is being deactivated.</summary>
			WM_ACTIVATEAPP=0x001C,
			///<summary>Sent to cancel certain modes, such as mouse capture. For example, the system sends this message to the active window when a dialog box or message box is displayed. 
			///Certain functions also send this message explicitly to the specified window regardless of whether it is the active window. 
			///For example, the EnableWindow function sends this message when disabling the specified window.</summary>
			WM_CANCELMODE=0x001F,
			///<summary>Sent to a child window when the user clicks the window's title bar or when the window is activated, moved, or sized.</summary>
			WM_CHILDACTIVATE=0x0022,
			///<summary>Sent as a signal that a window or an application should terminate.</summary>
			WM_CLOSE=0x0010,
			///<summary>Sent to all top-level windows when the system detects more than 12.5 percent of system time over a 30- to 60-second interval is being spent compacting memory. 
			///This indicates that system memory is low.</summary>
			WM_COMPACTING=0x0041,
			///<summary>Sent when an application requests that a window be created by calling the CreateWindowEx or CreateWindow function. (The message is sent before the function returns.) 
			///The window procedure of the new window receives this message after the window is created, but before the window becomes visible.</summary>
			WM_CREATE=0x0001,
			///<summary>Sent when a window is being destroyed. It is sent to the window procedure of the window being destroyed after the window is removed from the screen.
			///This message is sent first to the window being destroyed and then to the child windows (if any) as they are destroyed. During the processing of the message, 
			///it can be assumed that all child windows still exist.</summary>
			WM_DESTROY=0x0002,
			///<summary>Sent when an application changes the enabled state of a window. It is sent to the window whose enabled state is changing. 
			///This message is sent before the EnableWindow function returns, but after the enabled state (WS_DISABLED style bit) of the window has changed.</summary>
			WM_ENABLE=0x000A,
			///<summary>Sent one time to a window after it enters the moving or sizing modal loop. 
			///The window enters the moving or sizing modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the DefWindowProc function 
			///and the wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc returns.</summary>
			WM_ENTERSIZEMOVE=0x0231,
			///<summary>Sent one time to a window, after it has exited the moving or sizing modal loop. 
			///The window enters the moving or sizing modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the DefWindowProc function 
			///and the wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc returns.</summary>
			WM_EXITSIZEMOVE=0x0232,
			///<summary>Sent to a window to retrieve a handle to the large or small icon associated with a window. 
			///The system displays the large icon in the ALT+TAB dialog, and the small icon in the window caption.</summary>
			WM_GETICON=0x007F,
			///<summary>Sent to a window when the size or position of the window is about to change. 
			///An application can use this message to override the window's default maximized size and position, or its default minimum or maximum tracking size.</summary>
			WM_GETMINMAXINFO=0x0024,
			///<summary>Sent to the topmost affected window after an application's input language has been changed. 
			///You should make any application-specific settings and pass the message to the DefWindowProc function, which passes the message to all first-level child windows. 
			///These child windows can pass the message to DefWindowProc to have it pass the message to their child windows, and so on.</summary>
			WM_INPUTLANGCHANGE=0x0051,
			///<summary>Posted to the window with the focus when the user chooses a new input language,
			///either with the hotkey (specified in the Keyboard control panel application) or from the indicator on the system taskbar. 
			///An application can accept the change by passing the message to the DefWindowProc function or reject the change (and prevent it from taking place) by returning immediately.</summary>
			WM_INPUTLANGCHANGEREQUEST=0x0050,
			///<summary>Sent after a window has been moved.</summary>
			WM_MOVE=0x0003,
			///<summary>Sent to a window that the user is moving. By processing this message, an application can monitor the position of the drag rectangle and, if needed, change its position.</summary>
			WM_MOVING=0x0216,
			///<summary>Sent to a window when its nonclient area needs to be changed to indicate an active or inactive state.</summary>
			WM_NCACTIVATE=0x0086,
			///<summary>Sent when the size and position of a window's client area must be calculated. 
			///By processing this message, an application can control the content of the window's client area when the size or position of the window changes.</summary>
			WM_NCCALCSIZE=0x0083,
			///<summary>Sent prior to the WM_CREATE message when a window is first created.</summary>
			WM_NCCREATE=0x0081,
			///<summary>Notifies a window that its nonclient area is being destroyed. 
			///The DestroyWindow function sends the WM_NCDESTROY message to the window following the WM_DESTROY message.WM_DESTROY is used to free the allocated memory object associated with the window.</summary>
			WM_NCDESTROY=0x0082,
			///<summary>Performs no operation. An application sends the WM_NULL message if it wants to post a message that the recipient window will ignore.</summary>
			WM_NULL=0x0000,
			///<summary>Sent to a minimized (iconic) window. The window is about to be dragged by the user but does not have an icon defined for its class. 
			///An application can return a handle to an icon or cursor. The system displays this cursor or icon while the user drags the icon.</summary>
			WM_QUERYDRAGICON=0x0037,
			///<summary>Sent to an icon when the user requests that the window be restored to its previous size and position.</summary>
			WM_QUERYOPEN=0x0013,
			///<summary>Indicates a request to terminate an application, and is generated when the application calls the PostQuitMessage function. This message causes the GetMessage function to return zero.</summary>
			WM_QUIT=0x0012,
			///<summary>Sent to a window when the window is about to be hidden or shown.</summary>
			WM_SHOWWINDOW=0x0018,
			///<summary>Sent to a window after its size has changed.</summary>
			WM_SIZE=0x0005,
			///<summary>Sent to a window that the user is resizing. By processing this message, an application can monitor the size and position of the drag rectangle and, if needed, change its size or position.</summary>
			WM_SIZING=0x0214,
			///<summary>Sent to a window after the SetWindowLong function has changed one or more of the window's styles.</summary>
			WM_STYLECHANGED=0x007D,
			///<summary>Sent to a window when the SetWindowLong function is about to change one or more of the window's styles.</summary>
			WM_STYLECHANGING=0x007C,
			///<summary>Broadcast to every window following a theme change event. Examples of theme change events are the activation of a theme, 
			///the deactivation of a theme, or a transition from one theme to another.</summary>
			WM_THEMECHANGED=0x031A,
			///<summary>Sent to all windows after the user has logged on or off. When the user logs on or off, the system updates the user-specific settings. 
			///The system sends this message immediately after updating the settings.</summary>
			WM_USERCHANGED=0x0054,
			///<summary>Sent to a window whose size, position, or place in the Z order has changed as a result of a call to the SetWindowPos function or another window-management function.</summary>
			WM_WINDOWPOSCHANGED=0x0047,
			///<summary>Sent to a window whose size, position, or place in the Z order is about to change as a result of a call to the SetWindowPos function or another window-management function.</summary>
			WM_WINDOWPOSCHANGING=0x0046,
		}

		///<summary>The type of icon being retrieved. This parameter can be one of the following values.
		///https://msdn.microsoft.com/en-us/library/windows/desktop/ms632625(v=vs.85).aspx </summary>
		public enum WinGetIcon {
			///<summary>Retrieve the large icon for the window.</summary>
			ICON_BIG=1,
			///<summary>Retrieve the small icon for the window.</summary>
			ICON_SMALL=0,
			///<summary>Retrieves the small icon provided by the application. If the application does not provide one, the system uses the system-generated icon for that window.</summary>
			ICON_SMALL2=2,
		}

		[FlagsAttribute]
		///<summary>The new input locale. This parameter can be a combination of the following flags.
		///https://msdn.microsoft.com/en-us/library/windows/desktop/ms632630(v=vs.85).aspx </summary>
		public enum WinInputLangChangeRequest {
			///<summary>A hot key was used to choose the previous input locale in the installed list of input locales. This flag cannot be used with the INPUTLANGCHANGE_FORWARD flag.</summary>
			INPUTLANGCHANGE_BACKWARD=0x0004,
			///<summary>A hot key was used to choose the next input locale in the installed list of input locales. This flag cannot be used with the INPUTLANGCHANGE_BACKWARD flag.</summary>
			INPUTLANGCHANGE_FORWARD=0x0002,
			///<summary>The new input locale's keyboard layout can be used with the system character set.</summary>
			INPUTLANGCHANGE_SYSCHARSET=0x0001,
		}

		[FlagsAttribute]
		///<summary>If wParam is TRUE, the application should return zero or a combination of the following values.
		///https://msdn.microsoft.com/en-us/library/windows/desktop/ms632634(v=vs.85).aspx </summary>
		public enum WinNCCalcSize {
			///<summary>Specifies that the client area of the window is to be preserved and aligned with the top of the new position of the window. 
			///For example, to align the client area to the upper-left corner, return the WVR_ALIGNTOP and WVR_ALIGNLEFT values.</summary>
			WVR_ALIGNTOP=0x0010,
			///<summary>Specifies that the client area of the window is to be preserved and aligned with the right side of the new position of the window. 
			///For example, to align the client area to the lower-right corner, return the WVR_ALIGNRIGHT and WVR_ALIGNBOTTOM values.</summary>
			WVR_ALIGNRIGHT=0x0080,
			///<summary>Specifies that the client area of the window is to be preserved and aligned with the left side of the new position of the window. 
			///For example, to align the client area to the lower-left corner, return the WVR_ALIGNLEFT and WVR_ALIGNBOTTOM values.</summary>
			WVR_ALIGNLEFT=0x0020,
			///<summary>Specifies that the client area of the window is to be preserved and aligned with the bottom of the new position of the window. 
			///For example, to align the client area to the top-left corner, return the WVR_ALIGNTOP and WVR_ALIGNLEFT values.</summary>
			WVR_ALIGNBOTTOM=0x0040,
			///<summary>Used in combination with any other values, except WVR_VALIDRECTS, causes the window to be completely redrawn if the client rectangle changes size horizontally. 
			///This value is similar to CS_HREDRAW class style</summary>
			WVR_HREDRAW=0x0100,
			///<summary>Used in combination with any other values, except WVR_VALIDRECTS, causes the window to be completely redrawn if the client rectangle changes size vertically. 
			///This value is similar to CS_VREDRAW class style</summary>
			WVR_VREDRAW=0x0200,
			///<summary>This value causes the entire window to be redrawn. It is a combination of WVR_HREDRAW and WVR_VREDRAW values.</summary>
			WVR_REDRAW=0x0300,
			///<summary>This value indicates that, upon return from WM_NCCALCSIZE, the rectangles specified by the rgrc[1] and rgrc[2] members of the NCCALCSIZE_PARAMS structure 
			///contain valid destination and source area rectangles, respectively. The system combines these rectangles to calculate the area of the window to be preserved. 
			///The system copies any part of the window image that is within the source rectangle and clips the image to the destination rectangle. 
			///Both rectangles are in parent-relative or screen-relative coordinates. This flag cannot be combined with any other flags.</summary>
			WVR_VALIDRECTS=0x0400,
		}
		
		///<summary>The status of the window being shown. If lParam is zero, the message was sent because of a call to the ShowWindow function; otherwise, lParam is one of the following values.
		///https://msdn.microsoft.com/en-us/library/windows/desktop/ms632645(v=vs.85).aspx </summary>
		public enum WinShowWindow {
			///<summary>The window is being uncovered because a maximize window was restored or minimized.</summary>
			SW_OTHERUNZOOM=4,
			///<summary>The window is being covered by another window that has been maximized.</summary>
			SW_OTHERZOOM=2,
			///<summary>The window's owner window is being minimized.</summary>
			SW_PARENTCLOSING=1,
			///<summary>The window's owner window is being restored.</summary>
			SW_PARENTOPENING=3,
		}

		///<summary>The type of resizing requested. This parameter can be one of the following values.
		///https://msdn.microsoft.com/en-us/library/windows/desktop/ms632646(v=vs.85).aspx </summary>
		public enum WinSize {
			///<summary>Message is sent to all pop-up windows when some other window is maximized.</summary>
			SIZE_MAXHIDE=4,
			///<summary>The window has been maximized.</summary>
			SIZE_MAXIMIZED=2,
			///<summary>Message is sent to all pop-up windows when some other window has been restored to its former size.</summary>
			SIZE_MAXSHOW=3,
			///<summary>The window has been minimized.</summary>
			SIZE_MINIMIZED=1,
			///<summary>The window has been resized, but neither the SIZE_MINIMIZED nor SIZE_MAXIMIZED value applies.</summary>
			SIZE_RESTORED=0,
		}
		
		///<summary>The edge of the window that is being sized. This parameter can be one of the following values.
		///https://msdn.microsoft.com/en-us/library/windows/desktop/ms632647(v=vs.85).aspx </summary>
		public enum WinSizing {
			///<summary>Bottom edge</summary>
			WMSZ_BOTTOM=6,
			///<summary>Bottom-left corner</summary>
			WMSZ_BOTTOMLEFT=7,
			///<summary>Bottom-right corner</summary>
			WMSZ_BOTTOMRIGHT=8,
			///<summary>Left edge</summary>
			WMSZ_LEFT=1,
			///<summary>Right edge</summary>
			WMSZ_RIGHT=2,
			///<summary>Top edge</summary>
			WMSZ_TOP=3,
			///<summary>Top-left corner</summary>
			WMSZ_TOPLEFT=4,
			///<summary>Top-right corner</summary>
			WMSZ_TOPRIGHT=5,
		}

		[FlagsAttribute]
		///<summary>Indicates whether the window's styles or extended window styles have changed. This parameter can be one or more of the following values.
		///https://msdn.microsoft.com/en-us/library/windows/desktop/ms632648(v=vs.85).aspx </summary>
		public enum WinStyleChange {
			///<summary>The extended window styles have changed.</summary>
			GWL_EXSTYLE=-20,
			///<summary>The window styles have changed.</summary>
			GWL_STYLE=-16,
		}

		[FlagsAttribute]
		///<summary>Indicates whether the window's styles or extended window styles have changed. This parameter can be one or more of the following values.
		///https://msdn.microsoft.com/en-us/library/windows/desktop/ms632608(v=vs.85).aspx </summary>
		public enum WinTitleBarInfo {
			///<summary>The element can accept the focus.</summary>
			STATE_SYSTEM_FOCUSABLE=0x00100000,
			///<summary>The element is invisible.</summary>
			STATE_SYSTEM_INVISIBLE=0x00008000,
			///<summary>The element has no visible representation.</summary>
			STATE_SYSTEM_OFFSCREEN=0x00010000,
			///<summary>The element is unavailable.</summary>
			STATE_SYSTEM_UNAVAILABLE=0x00000001,
			///<summary>The element is in the pressed state.</summary>
			STATE_SYSTEM_PRESSED=0x00000008,
		}
		#endregion

		#region Windows Structs
		//https://msdn.microsoft.com/en-us/library/windows/desktop/ff468923(v=vs.85).aspx
		public struct ALTTABINFO {
			public uint cbSize; 
			public int cItems;
			public int cColumns;
			public int cRows;
			public int iColFocus;
			public int iRowFocus;
			public int cxItem;
			public int cyItem;
			public Point ptStart;
		}
		
		///<summary>Contains status information for the application-switching (ALT+TAB) window.</summary>
		public struct CHANGEFILTERSTRUCT {
			public uint cbSize;
			public uint ExtStatus;
		}

		///<summary>Contains information about the menu and first multiple-document interface (MDI) child window of an MDI client window. 
		///An application passes a pointer to this structure as the lpParam parameter of the CreateWindow function when creating an MDI client window.</summary>
		public struct CLIENTCREATESTRUCT {
			public IntPtr hWindowMenu;
			public uint idFirstChild;
		}

		///<summary>Defines the initialization parameters passed to the window procedure of an application. These members are identical to the parameters of the CreateWindowEx function.</summary>
		public struct CREATESTRUCT {
			public IntPtr lpCreateParams;
			public IntPtr hInstance;
			public IntPtr hMenu;
			public IntPtr hwndParent;
			public int cy;
			public int cx;
			public int y;
			public int x;
			public long style;
			public string lpszName;
			public string lpszClass;
			public uint dwExStyle;
		}

		///<summary>Contains information about a GUI thread.</summary>
		public struct GUITHREADINFO {
			public uint cbSize;
			public uint flags;
			public IntPtr hwndActive;
			public IntPtr hwndFocus;
			public IntPtr hwndCapture;
			public IntPtr hwndMenuOwner;
			public IntPtr hwndMoveSize;
			public IntPtr hwndCaret;
			public Rectangle rcCaret;
		}

		///<summary>Contains information about a window's maximized size and position and its minimum and maximum tracking size.</summary>
		public struct MINMAXINFO {
			public Point ptReserved;
			public Point ptMaxSize;
			public Point ptMaxPosition;
			public Point ptMinTrackSize;
			public Point ptMaxTrackSize;
		}
	
		///<summary>Contains information that an application can use while processing the WM_NCCALCSIZE message to calculate the size, position, and valid contents of the client area of a window.</summary>
		public struct NCCALCSIZE_PARAMS {
			public Rectangle[] rgrc;
			public Point lppos;
		}

		///<summary>Contains the styles for a window.</summary>
		public struct STYLESTRUCT {
			public int styleOld;
			public int styleNew;
		}		
	
		///<summary>Contains title bar information.</summary>
		public struct TITLEBARINFO {
			public uint cbSize;
			public Rectangle rcTitleBar;
			public uint[] rgstate;
		}
	
		///<summary>Expands on the information described in the TITLEBARINFO structure by including the coordinates of each element of the title bar.</summary>
		public struct TITLEBARINFOEX {
			public uint cbSize;
			public Rectangle rcTitleBar;
			public uint[] rgstate;
			public Rectangle[] rgrect;
		}
	
		///<summary>Used by UpdateLayeredWindowIndirect to provide position, size, shape, content, and translucency information for a layered window.</summary>
		public struct UPDATELAYEREDWINDOWINFO {
			public uint cbSize;
			public IntPtr hdcDst;
			public Point pptDst;
			public Size psize;
			public IntPtr hdcSrc;
			public Point pptSrc;
			public IntPtr crKey;
			public IntPtr pblend;
			public uint dwFlags;
			public Rectangle prcDirty;
		}
	
		///<summary>Contains window information.</summary>
		public struct WINDOWINFO {
			public uint cbSize;
			public Rectangle rcWindow;
			public Rectangle rcClient;
			public uint dwStyle;
			public uint dwExStyle;
			public uint dwWindowStatus;
			public uint cxWindowBorders;
			public uint cyWindowBorders;
			//public atom atomWindowType;
			public ushort wCreatorVersion;
		}
	
		///<summary>Contains information about the placement of a window on the screen.</summary>
		public struct WINDOWPLACEMENT {
			public uint length;
			public uint flags;
			public uint showCmd;
			public Point ptMinPosition;
			public Point ptMaxPosition;
			public Rectangle rcNormalPosition;
		}

		///<summary>Contains status information for the application-switching (ALT+TAB) window.</summary>
		public struct WINDOWPOS {
			public IntPtr hwnd; 
			public IntPtr hwndInsertAfter;
			public int x;
			public int y;
			public int cx;
			public int cy;
			public uint flags;
		}
	
		public struct CHARFORMAT {
			public int cbSize;
			public UInt32 dwMask;
			public UInt32 dwEffects;
			public Int32 yHeight;
			public Int32 yOffset;
			public Int32 crTextColor;
			public byte bCharSet;
			public byte bPitchAndFamily;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] szFaceName;
		}
		#endregion Windows Structs

		#region Paint Enums
		///<summary>The following messages are used with painting and drawing
		///https://msdn.microsoft.com/en-us/library/dd162761(v=vs.85).aspx </summary>
		public enum PaintMessages {
			///<summary>Retrieves the parent window. This does not include the owner, as it does with the GetParent function.</summary>
			WM_DISPLAYCHANGE=1,
			///<summary>Retrieves the root window by walking the chain of parent windows.</summary>
			WM_SETREDRAW=2,
			///<summary>Retrieves the owned root window by walking the chain of parent and owner windows returned by GetParent.</summary>
			GA_ROOTOWNER=3,
		}
		#endregion Paint Enums

		public enum ScrollBarCommands {
			SB_LINEUP = 0,
			SB_LINELEFT = 0,
			SB_LINEDOWN = 1,
			SB_LINERIGHT = 1,
			SB_PAGEUP = 2,
			SB_PAGELEFT = 2,
			SB_PAGEDOWN = 3,
			SB_PAGERIGHT = 3,
			SB_THUMBPOSITION = 4,
			SB_THUMBTRACK = 5,
			SB_TOP = 6,
			SB_LEFT = 6,
			SB_BOTTOM = 7,
			SB_RIGHT = 7,
			SB_ENDSCROLL = 8
		}

		///<summary>http://docs.embarcadero.com/products/rad_studio/delphiAndcpp2009/HelpUpdate2/EN/html/delphivclwin32/Messages.html</summary>
		public enum EM_Rich:int {
			EM_GETSEL = 0x00B0,
			EM_SETSEL = 0x00B1,
			EM_GETRECT = 0x00B2,
			EM_SETRECT = 0x00B3,
			EM_SETRECTNP = 0x00B4,
			EM_SCROLL = 0x00B5,
			EM_LINESCROLL = 0x00B6,
			EM_GETMODIFY = 0x00B8,
			EM_SETMODIFY = 0x00B9,
			EM_GETLINECOUNT = 0x00BA,
			EM_LINEINDEX = 0x00BB,
			EM_SETHANDLE = 0x00BC,
			EM_GETHANDLE = 0x00BD,
			EM_GETTHUMB = 0x00BE,
			EM_LINELENGTH = 0x00C1,
			EM_LINEFROMCHAR = 0x00C9,
			EM_GETFIRSTVISIBLELINE = 0x00CE,
			EM_SETMARGINS = 0x00D3,
			EM_GETMARGINS = 0x00D4,
			EM_POSFROMCHAR = 0x00D6,
			EM_CHARFROMPOS = 0x00D7,
			EXGETSEL = WM_USER + 52,
			EXSETSEL = WM_USER + 55,
			GETCHARFORMAT = WM_USER + 58,
			SETCHARFORMAT = WM_USER + 68,
			SETOPTIONS = WM_USER + 77,
			GETOPTIONS = WM_USER + 78,
			GETTEXTEX = WM_USER + 94,
			GETTEXTLENGTHEX = WM_USER + 95,
			SHOWSCROLLBAR = WM_USER + 96,
			SETTEXTEX = WM_USER + 97,
			GETSCROLLPOS = WM_USER + 221,
			SETSCROLLPOS = WM_USER + 222,
		}

		///<summary>http://docs.embarcadero.com/products/rad_studio/delphiAndcpp2009/HelpUpdate2/EN/html/delphivclwin32/Messages.html</summary>
		public enum WinMessagesOther {
			WM_PAINT = 0x000F,
			WM_CHAR = 0x0102,
			WM_VSCROLL = 0x115,
		}


		[Flags]
		public enum WinFlags {
			SWP_NOSIZE = 0x0001,
			SWP_NOMOVE = 0x0002,
			SWP_NOZORDER = 0x0004,
			SWP_NOREDRAW = 0x0008,
			SWP_NOACTIVATE = 0x0010,
			SWP_FRAMECHANGED = 0x0020,
			SWP_SHOWWINDOW = 0x0040,
			SWP_HIDEWINDOW = 0x0080,
			SWP_NOCOPYBITS = 0x0100,
			SWP_NOOWNERZORDER = 0x0200,
			SWP_NOSENDCHANGING = 0x0400,
		};

		[DllImport("User32.dll",EntryPoint="SendMessage",CharSet=CharSet.Auto)]
		public static extern int SendMessage(IntPtr hWnd,int msg,int wparam,IntPtr lparam);

		[DllImport("User32.dll",EntryPoint="SendMessage",CharSet=CharSet.Auto)]
		public static extern int SendMessage(IntPtr hWnd,int msg,int wparam,int lparam);

		[DllImport("User32.dll",EntryPoint="SendMessage",CharSet=CharSet.Auto)]
		public static extern int SendMessageRef(IntPtr hWnd,int msg,out int wparam,out int lparam);
	}

}