using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Runtime.InteropServices;
using CodeBase;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace OpenDental {
	public partial class SheetEditMobileCtrl:UserControl {
		#region Private Data and Properties
		///<summary>Each new control gets its own name using a unique id. This allows it to be uniquely identified and not mistaken for a different (similar) panel.</summary>
		private static long _controlIdCount=0;
		///<summary>Drag/drop icon image. Must be disposed after each use.</summary>
		private Bitmap _dragBitmap=null;
		///<summary>Drag/drop icon cursor. Must be disposed after each use.</summary>
		private Cursor _dragCursor=null;
		///<summary>Scroll position to jump on next timer tick if dragging near top/bottom of panelPreview.</summary>
		private int _scrollJump=0;
		///<summary>Allows this control to be floated in a separate form.</summary>
		private Form _formFloat=null;
		///<summary>Event thrown when new selection is made.</summary>
		public event EventHandler<long> SheetFieldDefSelected;
		///<summary>Event thrown when 'Use Mobile Layout' checkbox value changes. 
		///Owner of this control should keep their version of SheetDef.HasMobileLayout in sync as this event fires.</summary>
		public event EventHandler<bool> HasMobileLayoutChanged;
		///<summary>Event thrown when edit is requested. Normally only one item in the list which is the SheetDefNum of the SheetDef to be edited. 
		///If this is a radio group then the list will include all SheetDefNums in the group.</summary>
		public event EventHandler<SheetFieldDefEditArgs> SheetFieldDefEdit;

		public class SheetFieldDefEditArgs:EventArgs {
			public List<long> SheetFieldDefNums { get; private set; }
			public bool IsRadioItem { get; private set; }
			public SheetFieldDefEditArgs(List<long> sheetFieldDefNums,bool isRadioItem) {
				SheetFieldDefNums=new List<long>(sheetFieldDefNums);
				IsRadioItem=isRadioItem;
			}
		}

		///<summary>Event raised when user clicks Add Header button.</summary>
		public event EventHandler AddMobileHeader;
		public class NewMobileFieldValueArgs :EventArgs {
			public long SheetFieldDefNum { get; private set; }
			public string NewFieldValue { get; private set; }
			public NewMobileFieldValueArgs(long sheetFieldDefNum,string newFieldValue) {
				SheetFieldDefNum=sheetFieldDefNum;
				NewFieldValue=newFieldValue;
			}
		}
		///<summary>Event raised when user edits MobileHeader's UiLabelMobile.</summary>
		public event EventHandler<NewMobileFieldValueArgs> NewMobileHeader;
		///<summary>Event raised when user edits StaticTexts's FieldValue.</summary>
		public event EventHandler<NewMobileFieldValueArgs> NewStaticText;
		private const int INNER_CONTROL_WIDTH=200;
		///<summary>Background color of each panel and its children when not dragging.</summary>
		private Color _colorPanelBG=Color.White;
		private bool _isReadOnly=false;

		///<summary>If true then edits are not allowed and many control events will be disabled.</summary>
		public bool IsReadOnly {
			get {
				return _isReadOnly;
			}
			set {
				_isReadOnly=value;
				labelDragToOrder.Visible=!IsReadOnly;
				checkUseMobileLayout.Enabled=!IsReadOnly;
			}
		}

		///<summary>Set this to provide Lan.g() translations.</summary>
		public Func<string,string> TranslationProvider;
		///<summary>True if user presses "Order fields from Desktop". Mimic the order from desktop into mobile.</summary>
		private bool _tabOrderMobileToMimicTabOrder=false;
		private Color _colorFieldsNeedMove=Color.LightSalmon;
		private string GetTranslation(string transIn) {
			return TranslationProvider==null ? transIn : TranslationProvider(transIn);
		}

		///<summary>Next call to SheetDef setter will scroll to the panel which this PK belongs to. Used when adding a new field. 
		///Will be reset after one-time use so stale value does not get scrolled later.</summary>
		public long ScrollToSheetFieldDefNum { get; set; }
		///<summary></summary>
		private string _languageCur="";

		private SheetDef _sheetDef;
		///<summary>Creates an internal deep copy of the SheetDef and it's SheetFieldDefs. The original object will not be modified.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<SheetFieldDef> SheetFieldDefs {
			get {
				//All children should have tag that is a sheet field.
				return panelPreview.Controls.Cast<Control>()
					.SelectMany(x => x.Controls.Cast<Control>(),(x,y) => new {
						TagPanel=x.Tag,
						TagCtrl=y.Tag,
					})
					.Where(x => (x.TagPanel!=null && x.TagPanel is SheetFieldDef) || (x.TagCtrl != null && x.TagCtrl is SheetFieldDef))
					.Select(x => (SheetFieldDef)x.TagPanel??(SheetFieldDef)x.TagCtrl).ToList();
			}
		}
		///<summary>Creates an internal deep copy of the SheetDef and it's SheetFieldDefs. The original object will not be modified.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SheetDef SheetDef {
			set {
				//Capture previous SheetDefNum so we can check if this is a new SheetDef or an update to the existing instance.
				long sheetDefNumOld=_sheetDef==null?-1:_sheetDef.SheetDefNum;
				if(value==null) {
					_sheetDef=new SheetDef() {
						SheetFieldDefs=new List<SheetFieldDef>(),
						Parameters=new List<SheetParameter>(),
					};
				}
				//Set this field before merging. It may have been changed intentionally by the owner of this control before setting the new field defs.
				checkUseMobileLayout.Checked=value.HasMobileLayout;
				bool updateExisting=sheetDefNumOld==value.SheetDefNum;
				if(updateExisting) { //This is just an update so save changes before setting new fields.
					MergeMobileSheetFieldDefs(value,false);
				}
				//Make a deep copy of the SheetDef.
				_sheetDef=value.Copy();
				_sheetDef.SheetFieldDefs=value.SheetFieldDefs==null ? new List<SheetFieldDef>() : value.SheetFieldDefs.Select(x => x.Copy()).ToList();
				_sheetDef.Parameters=value.Parameters==null ? new List<SheetParameter>() : value.Parameters.Select(x => x.Copy()).ToList();
				List<SheetEditMobilePanel> newPanels=new List<SheetEditMobilePanel>();
				Func<bool> isReadOnly=new Func<bool>(() => { return IsReadOnly; } );
				//Each SheetFieldDef item goes into the panel of available fields.
				//Only mobile-friendly fields.
				var mobileFields=_sheetDef.SheetFieldDefs.Where(x =>
						OpenDentBusiness.SheetFieldDefs.IsMobileFieldType(x.FieldType,x.TabOrderMobile,x.FieldName) && x.Language==_languageCur
					).ToList();
				//2 different ways that fields can be grouped for radio groups so make a super-set of both styles.
				Func<SheetFieldDef,bool> criteria1=new Func<SheetFieldDef, bool>((x) => {
					//Misc and it has a RadioButtonGroup.						
					return x.FieldName=="misc" && !string.IsNullOrEmpty(x.RadioButtonGroup);
				});
				Func<SheetFieldDef,bool> criteria2=new Func<SheetFieldDef, bool>((x) => {
					//Not misc but it has a RadioButtonValue.						
					return x.FieldName!="misc" && !string.IsNullOrEmpty(x.RadioButtonValue);
				});
				var radioGroupsSuperSet=mobileFields
					.Where(x => x.FieldType==SheetFieldType.CheckBox)
					.Where(x => criteria1(x) || criteria2(x))
					//If TabOrderMobile has not been established (0) then move to the bottom.
					.OrderByDescending(x => x.TabOrderMobile>0)
					//Sort each group by TabOrderMobile.
					.OrderBy(x => x.TabOrderMobile)
					.ToList();
				//The first way.
				var radioFields1=radioGroupsSuperSet
					.Where(x => criteria1(x))
					.GroupBy(x => x.RadioButtonGroup)
					//Common anonymous class def that can be unioned and sorted below.
					.Select(x => new {
						Name=x.First().RadioButtonGroup,
						MaxTabOrderDesktop=x.Max(y => y.TabOrder),
						MaxTabOrderMobile=x.Max(y => y.TabOrderMobile),
						Items=x.ToList()
					})
					//Must have at least 2 items to be a radio group. Otherwise it is still just a checkbox.
					.Where(x => x.Items.Count>1).ToList();
				//The second way.
				var radioFields2=radioGroupsSuperSet
					//Don't include any fields that have already been handled above.
					.Where(x => criteria2(x))
					.GroupBy(x => x.FieldName)
					//Common anonymous class def that can be unioned and sorted below.
					.Select(x => new {
						Name=x.First().FieldName,
						MaxTabOrderDesktop=x.Max(y => y.TabOrder),
						MaxTabOrderMobile=x.Max(y => y.TabOrderMobile),
						Items=x.ToList()
					})
					//Must have at least 2 items to be a radio group. Otherwise it is still just a checkbox.
					.Where(x => x.Items.Count>1).ToList();
				//Remove all items that have already been accounted for above.
				List<SheetFieldDef> alreadyGrouped=radioFields1.SelectMany(x => x.Items)
					.Union(radioFields2.SelectMany(x => x.Items))
					.ToList();
				var checkboxGroups=mobileFields
					.Except(alreadyGrouped)
					//If TabOrderMobile has not been established (0) then move to the bottom.
					.OrderByDescending(x => x.TabOrderMobile>0)
					//Sort each group by TabOrderMobile.
					.OrderBy(x => x.TabOrderMobile)
					.Where(x=> x.FieldName=="misc" && x.FieldType==SheetFieldType.CheckBox && !string.IsNullOrEmpty(x.UiLabelMobile) && string.IsNullOrEmpty(x.RadioButtonGroup))
					.GroupBy(x => x.UiLabelMobile)
					.Select(x => new {
						Name=x.Key,
						MaxTabOrderDesktop=x.Max(y => y.TabOrder),
						MaxTabOrderMobile=x.Max(y => y.TabOrderMobile),
						Items=x.ToList()
					})
					//Must have at least 2 items to be a checkbox group. Otherwise it is still just a checkbox.
					.Where(x => x.Items.Count>1).ToList();
				var removeSheetDefNums=radioFields1
					.SelectMany(x => x.Items.Select(y => y.SheetFieldDefNum))
					.Union(radioFields2.SelectMany(x => x.Items.Select(y => y.SheetFieldDefNum)))
					.Union(checkboxGroups.SelectMany(x=> x.Items.Select(y => y.SheetFieldDefNum))).ToList();
				int removed=mobileFields.RemoveAll(x => removeSheetDefNums.Any(y => y==x.SheetFieldDefNum));
				//Whatever is left is a stand-alone mobile-friendly field.
				var nonRadioFields=mobileFields
					//Common anonymous class def that can be unioned and sorted below.
					.Select(x => new {
						Name=x.FieldName,
						MaxTabOrderDesktop=x.TabOrder,
						MaxTabOrderMobile=x.TabOrderMobile,
						Items=new List<SheetFieldDef>() { x }
					}).ToList();
				//Union the 3 lists and sort for final display.
				var finalFields=radioFields1
					.Union(radioFields2)
					.Union(checkboxGroups)
					.Union(nonRadioFields)
					//New Mobile headers to top.
					.OrderByDescending(x => x.Items[0].FieldType==SheetFieldType.MobileHeader && x.MaxTabOrderMobile==0)					
					//Mobile tab order fields to top.
					.ThenByDescending(x => x.MaxTabOrderMobile>0)
					//Sort mobile fields.
					.ThenBy(x => x.MaxTabOrderMobile)
					//We are now left with mobile-friendly fields that do not yet have a mobile tab order.
					//Desktop tab order ascending (leave 0 at the bottom).
					.ThenByDescending(x => x.MaxTabOrderDesktop>0)
					.ThenBy(x => x.MaxTabOrderDesktop)
					//Name is always the tie-breaker.
					.ThenBy(x => x.Name).ToList()
					//We only need the lists of SheetFieldDefs.
					.Select(x => x.Items).ToList();
				SheetEditMobilePanel panelScrollTo=null;
				//These are now properly grouped and ordered. Each group is 1 single row in the UI. The row may have mutliple fields if it is a group box.
				foreach(List<SheetFieldDef> sheetFields in finalFields) {
					#region addSheetDef Action
					//The first SFD in the list will represent all items in the group.
					SheetFieldDef sheetField=sheetFields[0];
					SheetEditMobilePanel panel=new SheetEditMobilePanel(isReadOnly) {
						Width=panelPreview.Width-22,
						Header=string.IsNullOrEmpty(sheetField.UiLabelMobile) ?
							(sheetFields.Count>1 ? "RadioGroup" : sheetField.FieldType.ToString())+" - "+sheetField.FieldName : sheetField.UiLabelMobile,
						IsHeaderValid=sheetFields.Count>1 ? true : !string.IsNullOrEmpty(sheetField.UiLabelMobile),
						FieldType=GetTranslation(sheetField.FieldType.GetDescription()),
						ButtonLabel=GetTranslation("Drag to change order or double-click to edit label"),
					};
					if(sheetField.SheetFieldDefNum==ScrollToSheetFieldDefNum) { //We will scroll to this panel after we are done adding all panels.
						panelScrollTo=panel;
					}
					var mouseEnter=new EventHandler((sender,e) => SetControlAndChildrenBackColor(panel,Color.LightGray));
					var mouseLeave=new EventHandler((sender,e) => {
						if(panel.NeedsMove){
							SetControlAndChildrenBackColor(panel,_colorFieldsNeedMove);
						}
						else{
							SetControlAndChildrenBackColor(panel,_colorPanelBG);
						}
					});
					//Mouse events are not fired when a drag is activated. Must handle drag events in this case instead of mouse events.
					var dragOver=new DragEventHandler((sender,e) => {
						Control controlDraggingOver=(Control)sender;
						Control controlDragging=(Control)e.Data.GetData(e.Data.GetFormats()[0]);
						if(controlDragging is SheetEditMobileRadioButton) { //Dragging over a Radio Item.
							if(controlDraggingOver is SheetEditMobileRadioButton) {
								SetControlAndChildrenBackColor(controlDraggingOver,Color.LightYellow);
							}
						}
						else { //Dragging over the main panel.
							SetControlAndChildrenBackColor(panel,Color.LightYellow);
						}
					});
					var dragLeave=new EventHandler((sender,e) => {
						if(panel.NeedsMove){
							SetControlAndChildrenBackColor(panel,_colorFieldsNeedMove);
						}
						else{
							SetControlAndChildrenBackColor(panel,_colorPanelBG);
						}
					});
					bool isCheckBox=false;
					Func<SheetFieldDef,SheetEditMobileRadioButton> addCheckBox=new Func<SheetFieldDef,SheetEditMobileRadioButton>((sheetFieldRadioItem) => {
						SheetEditMobileRadioButton radioItem=new SheetEditMobileRadioButton(isReadOnly) {
							Tag=sheetFieldRadioItem,
							Width=INNER_CONTROL_WIDTH,
							Height=20,
							Anchor=AnchorStyles.Right,
						};
						//radioItem.Text should be set by caller of this func after return.
						if(SheetFieldDefEdit!=null) {
							radioItem.TextClick+=new EventHandler((sender,e) => {
								SheetFieldDefEdit.Invoke(this,new SheetFieldDefEditArgs(new List<long> { sheetFieldRadioItem.SheetFieldDefNum },true));
								SetHighlightedFieldDefs(new List<long> {sheetFieldRadioItem.SheetFieldDefNum });
							});
						}
						radioItem.Selected+=new EventHandler((sender,e) => {
							SheetFieldDefSelected?.Invoke(this,sheetFieldRadioItem.SheetFieldDefNum);
						});
						radioItem.MouseEnter+=new EventHandler((sender,e) => {
							SetControlAndChildrenBackColor(radioItem,Color.LightGray);
						});
						radioItem.MouseLeave+=new EventHandler((sender,e) => {
							if(panel.NeedsMove){
								SetControlAndChildrenBackColor(radioItem,_colorFieldsNeedMove);
							}
							else{
								SetControlAndChildrenBackColor(radioItem,_colorPanelBG);
							}
						});
						panel.ButtonLabel=GetTranslation("Drag to change order or click checkbox button to edit single item");
						panel.Controls.Add(radioItem);
						//This is a checkbox panel so we won't register for DoubleClick below. Checkbox will take care of that.
						isCheckBox=true;
						return radioItem;
					});
					if(sheetFields.Count > 1 && sheetField.FieldType==SheetFieldType.CheckBox && sheetField.FieldName=="misc" && sheetFields.GroupBy(x=>x.UiLabelMobile).Count()==1 
						&& (sheetFields.GroupBy(x=>x.RadioButtonGroup).Count()!=1 || sheetFields.All(x=>x.RadioButtonGroup.IsNullOrEmpty()))) {
							#region CheckBox Group
							panel.FieldType=GetTranslation("CheckBox Group");
							List<CheckBox>checkItems=new List<CheckBox>();
							Action<SheetFieldDef> addCheckOption=new Action<SheetFieldDef>((sheetFieldCheckItem) => {
								SheetEditMobileRadioButton checkboxItem=addCheckBox(sheetFieldCheckItem);
								checkboxItem.Text=sheetFieldCheckItem.UiLabelMobileRadioButton;
								checkboxItem.MouseMove+=new MouseEventHandler((sender,e) => TryStartDragging(checkboxItem,e.Location));
								checkItems.Add(checkboxItem);
							});
						foreach(var checkboxItem in sheetFields) {
								addCheckOption(checkboxItem);
							}
							#endregion
						}
					else if(sheetFields.Count>1 || !string.IsNullOrEmpty(sheetFields[0].RadioButtonValue)) {
						#region RadioGroup
						panel.FieldType=GetTranslation("Radio Group");
						List<SheetEditMobileRadioButton> radioItems=new List<SheetEditMobileRadioButton>();
						Action<SheetFieldDef> addRadioOption=new Action<SheetFieldDef>((sheetFieldRadioItem) => {
							#region addRadioOption
							SheetEditMobileRadioButton radioItem=addCheckBox(sheetFieldRadioItem);
							//Misc will use UiLabelMobileRadioButton, pre-defined will use RadioButtonValue.
							sheetFieldRadioItem.UiLabelMobileRadioButton=string.IsNullOrEmpty(sheetFieldRadioItem.UiLabelMobileRadioButton) ?
								sheetFieldRadioItem.RadioButtonValue : sheetFieldRadioItem.UiLabelMobileRadioButton ;
							radioItem.Text=sheetFieldRadioItem.UiLabelMobileRadioButton;							
							if(SheetFieldDefEdit==null) {
								radioItem.TextClick+=new EventHandler((sender,e) => {
									using InputBox input=new InputBox(GetTranslation("Edit label for radio item"),sheetFieldRadioItem.UiLabelMobileRadioButton);
									if(input.ShowDialog()==DialogResult.OK && !string.IsNullOrEmpty(input.textResult.Text)) {
										sheetFieldRadioItem.UiLabelMobileRadioButton=input.textResult.Text;
										radioItem.Text=input.textResult.Text;
									}
								});
							}							
							radioItem.CheckedChanged+=new EventHandler((sender,e) => {
								if(!radioItem.Checked) {
									return;
								}
								//Uncheck all others to give radio group effect.
								radioItems.ForEach(x => {
									if(x!=radioItem) {
										x.Checked=false;
									}
								});
							});							
							radioItem.MouseMove+=new MouseEventHandler((sender,e) => TryStartDragging(radioItem,e.Location));
							radioItems.Add(radioItem);
							#endregion
						});
						foreach(var radioItem in sheetFields) {
							//Prefer RadioButtonValue then FieldValue. If both are empty then just empty.
							addRadioOption(radioItem);
						}
						#endregion
					}
					else if(sheetField.FieldType==SheetFieldType.ComboBox) {
						#region ComboBox
						string split=sheetField.FieldValue;
						int i=split.IndexOf(';');
						if(i==0) {
							split=split.Substring(1);
						}
						ComboBox combo=new ComboBox() {
							Width=INNER_CONTROL_WIDTH,
							Height=20,
							Anchor=AnchorStyles.Right,
							DropDownStyle=ComboBoxStyle.DropDownList,
							Tag=sheetField,
						};
						combo.Items.AddRange(split.Split('|').Select(x => x).Cast<object>().ToArray());
						combo.MouseEnter+=mouseEnter;
						combo.MouseLeave+=mouseLeave;
						panel.Controls.Add(combo);
						#endregion
					}
					else if(sheetField.FieldType==SheetFieldType.CheckBox) {
						#region Checkbox
						SheetEditMobileRadioButton checkBox=addCheckBox(sheetField);
						checkBox.Text=sheetField.UiLabelMobileRadioButton;
						//No panel header in this case. The checkbox holds the label.
						panel.Header="";
						#endregion
					}
					else if(sheetField.FieldType==SheetFieldType.InputField) {
						#region InputField
						TextBox textBox=new TextBox() {
							Width=INNER_CONTROL_WIDTH,
							Height=20,
							Anchor=AnchorStyles.Right,
							Tag=sheetField,
							ReadOnly=true,
							Text=sheetField.FieldName.IsNullOrEmpty() ? "Input Text" : sheetField.FieldName,
							TextAlign=HorizontalAlignment.Right,
						};
						panel.Controls.Add(textBox);
						#endregion
					}
					else if(sheetField.FieldType==SheetFieldType.SigBox) {
						#region SigBox
						TextBox picBox=new TextBox() {
							Width=INNER_CONTROL_WIDTH,
							Height=60,
							Multiline=true,
							Anchor=AnchorStyles.Right,
							BorderStyle=BorderStyle.FixedSingle,
							Tag=sheetField,
							ReadOnly=true,
							Text="\r\nSign Here",
							TextAlign=HorizontalAlignment.Center,
						};
						panel.Controls.Add(picBox);
						#endregion
					}
					else if(sheetField.FieldType==SheetFieldType.OutputText) {
						#region OutputText
						TextBox textBox=new TextBox() {
							Width=INNER_CONTROL_WIDTH,
							Height=20,
							Anchor=AnchorStyles.Right,
							Tag=sheetField,
							ReadOnly=true,
							Text="Output Text",
							TextAlign=HorizontalAlignment.Right,
						};
						panel.Controls.Add(textBox);
						#endregion
					}
					else if(sheetField.FieldType==SheetFieldType.MobileHeader) {
						panel.MobileHeader=sheetField.UiLabelMobile;
						panel.Tag=sheetField;
					}
					else if(sheetField.FieldType==SheetFieldType.StaticText) {
						panel.MobileHeader=sheetField.FieldValue;
						panel.Tag=sheetField;
					}
					else {
						throw new Exception("Unsupported mobile FieldType: "+sheetField.FieldType.ToString());
					}
					panel.DoubleClick+=new EventHandler((sender,e) => {
						#region Panel DoubleClick
						if(IsReadOnly) {
							return;
						}
						if(SheetFieldDefEdit!=null) { //Owner wants to handle editing.
							SheetFieldDefEdit?.Invoke(this,new SheetFieldDefEditArgs(sheetFields.Select(x => x.SheetFieldDefNum).ToList(),false));
							if(sheetField.FieldType==SheetFieldType.CheckBox) {
								SetHighlightedFieldDefs(sheetFields.Select(x => x.SheetFieldDefNum).ToList());
							}
							return;
						}
						//Owner is not handling editing so handle it here.
						using InputBox input=new InputBox(GetTranslation("Edit label for")+" "+
							(sheetField.FieldType==SheetFieldType.StaticText ? GetTranslation(SheetFieldType.StaticText.ToString()) : sheetField.FieldName),
							sheetField.FieldType==SheetFieldType.StaticText ? sheetField.FieldValue : sheetField.UiLabelMobile);
						if(input.ShowDialog()!=DialogResult.OK||string.IsNullOrEmpty(input.textResult.Text)) {
							return;
						}
						string newLabel=input.textResult.Text;
						sheetFields.ForEach(x => x.UiLabelMobile=newLabel);
						panel.Header=newLabel;
						panel.IsHeaderValid=!string.IsNullOrEmpty(newLabel);
						if(sheetField.FieldType==SheetFieldType.MobileHeader) {
							panel.MobileHeader=newLabel;
							NewMobileHeader?.Invoke(this,new NewMobileFieldValueArgs(sheetField.SheetFieldDefNum,newLabel));
						}
						if(sheetField.FieldType==SheetFieldType.StaticText) {
							panel.MobileHeader=newLabel;
							sheetField.FieldValue=newLabel;
							NewStaticText?.Invoke(this,new NewMobileFieldValueArgs(sheetField.SheetFieldDefNum,newLabel));
						}
						#endregion
					});
					panel.MouseDown+=new MouseEventHandler((sender,e) => {
						#region MouseDown
						if(e.Button!=MouseButtons.Left) {
							return;
						}
						//Mouse-down will initiate both click and drag. 
						//Start both here and determine if we are clicking or dragging once we start moving the mouse.
						panel.DragAtt.StartMouseDown(e.Location);
						#endregion
					});
					panel.MouseUp+=new MouseEventHandler((sender,e) => {
						#region MouseUp
						if(panel.DragAtt.CanMouseUp) { //We have not moved the mouse far enough to be considered a drag so it must be a click.							
							SheetFieldDefSelected?.Invoke(this,sheetField.SheetFieldDefNum);
						}
						//Reset the panel for click and drag for next time.
						panel.DragAtt.DisableMouseEvents();
						#endregion
					});
					panel.MouseMove+=new MouseEventHandler((sender,e) => {
						#region MouseMove
						if(IsReadOnly) {
							return;
						}
						TryStartDragging(panel,e.Location);
						this.Cursor=Cursors.SizeAll;
						#endregion
					});
					panel.MouseLeave+=new EventHandler((sender,e) => {
						this.Cursor=Cursors.Default;
					});
					panel.MouseEnter+=mouseEnter;
					panel.MouseLeave+=mouseLeave;
					panel.DragOver+=dragOver;
					panel.DragOver+=panel_DragOver;
					panel.DragLeave+=dragLeave;
					panel.DragEnter+=panel_DragEnter;
					panel.DragDrop+=panel_DragDrop;
					panel.BackColor=panel.NeedsMove ? _colorFieldsNeedMove : _colorPanelBG;
					//Start with margin only.
					int panelHeight=15+panel.Padding.Top;
					//All controls must allow drop so we can detect when it is being dragged over and place the dragged control in that exact position.
					panel.Controls.Cast<Control>().ForEach(x => {
						panelHeight+=x.Height;
						x.DragOver+=panel_DragOver;
						x.DragEnter+=panel_DragEnter;
						x.DragDrop+=panel_DragDrop;
						x.DragOver+=dragOver;
						x.DragLeave+=dragLeave;
						x.AllowDrop=true;
						x.Padding=new Padding(0);
						x.Left=panel.Padding.Left;
						x.Margin=new Padding(0,0,2,0);
						x.BackColor=panel.NeedsMove ? _colorFieldsNeedMove : _colorPanelBG;
						//Give this control a unique name so we can find it later.
						x.Name=(++_controlIdCount).ToString();
					});
					panel.Name=(++_controlIdCount).ToString();
					panel.Height=Math.Max(panelHeight,panel.Height);
					newPanels.Add(panel);
					#endregion
				}
				panelPreview.SuspendLayout();
				//Each control must be individually disposed in order to avoid memory leaks.
				//Grab a copy of the list.
				var controls=new List<Control>(panelPreview.Controls.Cast<Control>());
				if(_tabOrderMobileToMimicTabOrder) {
					//For each panel, check if the controls within or the panel itself have a TabOrder=0. If yes, color the panel.
					foreach(var panel in newPanels) {
						//This happens with radio buttons
						if(panel.Tag==null) { 
							foreach(Control cont in panel.Controls) {
								if(IsUnOrderable((SheetFieldDef)cont.Tag)) {
									MarkUnOrderable(panel);
									break;
								}
							}
						}
						else {
							if(IsUnOrderable((SheetFieldDef)panel.Tag)) {
								MarkUnOrderable(panel);
							}
						}
						//Reset so no, unintended consequences 
						_tabOrderMobileToMimicTabOrder=false;
					}
				}
				//The following two loops check if there are any panels that are colored and need to stay colored after the page refreshes
				List<SheetEditMobilePanel> listPanelsNeedColor=panelPreview.Controls.Cast<SheetEditMobilePanel>().Where(x => x.NeedsMove==true).ToList();
				List<long>listSfdNum=new List<long>();
				//Old panels with color
				foreach(SheetEditMobilePanel panel in listPanelsNeedColor) {
					SheetFieldDef tag=(SheetFieldDef)panel.Tag;
					if(tag==null) {
						foreach(Control ctrl in panel.Controls) {
							SheetFieldDef currentSFD=(SheetFieldDef)ctrl.Tag;
							if(currentSFD!=null) {
								listSfdNum.Add(currentSFD.SheetFieldDefNum);
							}
						}
					}
					else {
						listSfdNum.Add(tag.SheetFieldDefNum);
					}
				}
				//New panels that need color
				foreach(SheetEditMobilePanel panel in newPanels) {
					SheetFieldDef tag=(SheetFieldDef)panel.Tag;
					if(tag==null) {
						foreach(Control ctrl in panel.Controls) {
							SheetFieldDef currentSFD=(SheetFieldDef)ctrl.Tag;
							if(currentSFD!=null) {
								if(listSfdNum.Contains(currentSFD.SheetFieldDefNum)) {
									MarkUnOrderable(panel);
								}
							}
						}
					}
					else {
						if(listSfdNum.Contains(tag.SheetFieldDefNum)) {
							MarkUnOrderable(panel);
						}
					}
				}
				panelPreview.Controls.Clear();
				//Dispose each control that had previously belonged to the panel.
				foreach(var c in controls) {
					c.Dispose();
				}
				panelPreview.Controls.AddRange(newPanels.ToArray());
				panelPreview.ResumeLayout();
				if(panelScrollTo!=null) {
					panelPreview.ScrollControlIntoView(panelScrollTo,true);
				}
				//Reset so we don't accidentally scroll to this panel next time.
				ScrollToSheetFieldDefNum=-1;
			}
		}

		private bool IsUnOrderable(SheetFieldDef sheetFieldCur) {
			if(sheetFieldCur!=null && sheetFieldCur.TabOrder==0) {
				return true;
			}
			return false;
		}

		private void MarkUnOrderable(SheetEditMobilePanel panel) {
			panel.NeedsMove=true;
			SetControlAndChildrenBackColor(panel,_colorFieldsNeedMove);
		}
		#endregion

		public SheetEditMobileCtrl() {
			InitializeComponent();
			Name="sheetEditMobileCtrl";
		}

		#region Public Interface
		///<summary>SheetEditMobileCtrl is working with deep copies of the original list of SheetFieldDefs. 
		///This method will merge any changes made by the control into the items of the provided sheetDef.SheetFieldDefs.
		///Returns true if sheetDef.SheetFieldDefs items were modified. Otherwise, err Action will be invoked and returns false.
		///Returns true and will not merge if sheetDef.IsMobileLayoutAllowed==false.
		///This method does not save to the db. Calling method should save the final version of sheetDef to the db as needed.</summary>
		///<param name="sheetDef">TabOrderMobile and UiLabelMobile of each item in sheetDef.SheetFieldDefs will be modified with values that were specified by SheetEditMobileCtrl 
		///If an item is mobile-friendly, it will be given a TabOrderMobile>0 and a non-empty UiLabelMobile. TabOrderMobile will repeat for items in the same radio group.
		///If an items is not mobile-friendly, it will be given a TabOrderMobile=0 and should be omitted from the mobile web layout.
		///If HasMobileLayout is true then all mobile display labels will be validated and forced to be non-empty.
		///This would prevent user from saving these changes to the db.</param>
		public bool MergeMobileSheetFieldDefs(SheetDef sheetDef,bool requireValidation,Action<string> err=null) {
			try {
				if(sheetDef==null||sheetDef.SheetFieldDefs==null) {
					throw new Exception("Invalid sheet cannot be merged.");
				}
				if(!SheetDefs.IsMobileAllowed(sheetDef.SheetType)) { //Return silently. No need to merge.
					return true;
				}
				if(IsReadOnly) {
					throw new Exception("Mobile field editing is not allowed in Read-Only mode.");
				}
				if(_sheetDef==null) {
					throw new Exception("Sheet Def not set in mobile field editor.");
				}
				if(sheetDef.SheetDefNum!=_sheetDef.SheetDefNum) {
					throw new Exception("SheetDefNum mismtach. Unable to save mobile field settings.");
				}
				//We made deep copies of all SheetFieldDefs when we entered the control so we can modify fields here without consequence.
				List<SheetFieldDef> listMobileSheetDefs=SheetFieldDefs;//List of SheetFieldDefs pulled from panel UI
				string languageCheck=listMobileSheetDefs.FirstOrDefault()?.Language??"";//Blank string is used for 'Default' translation.
				//StaticText can be excluded by user choice. See FormSheetFieldStatic.butOk_Click() for reference.
				//Do not include StaticText fields that do not have an existing TabOrderMobile.
				var listRemoveStatics=sheetDef.SheetFieldDefs.Where(x => x.FieldType==SheetFieldType.StaticText
					&& x.TabOrderMobile<=0 
					&& x.Language==languageCheck
				).Select(x => x.SheetFieldDefNum);
				//Add in StaticText fields that have a TabOrderMobile but are not already included in our list.
				var listAddStatics=sheetDef.SheetFieldDefs.Where(x => x.FieldType==SheetFieldType.StaticText 
					&& x.TabOrderMobile>0 
					&& x.Language==languageCheck
					&& !listMobileSheetDefs.Any(y => y.SheetFieldDefNum==x.SheetFieldDefNum)
				);
				listMobileSheetDefs.RemoveAll(x => listRemoveStatics.Contains(x.SheetFieldDefNum));
				if(listAddStatics.Count()>0) {
					listMobileSheetDefs.InsertRange(0,listAddStatics);
				}
				if(_tabOrderMobileToMimicTabOrder) {
					//Want to set the TabOrderMobile as close to the TabOrder as possible. Any field that has a TabOrder=0 cannot be ordered. We will
					//color these fields and bring them to the top of the list to alert the user they need to move them into the correct position.
					List<SheetFieldDef> listUnOrderable=new List<SheetFieldDef>();
					List<SheetFieldDef> listOrderableFields=new List<SheetFieldDef>();
					listUnOrderable.AddRange(listMobileSheetDefs.Where(x => IsUnOrderable(x)).OrderBy(y => y.FieldType).ToList());
					listOrderableFields.AddRange(listMobileSheetDefs
						.Except(listUnOrderable)
						.OrderBy(x => x.TabOrder)
						.ToList());
					listMobileSheetDefs.Clear();
					listMobileSheetDefs.AddRange(listUnOrderable);
					listMobileSheetDefs.AddRange(listOrderableFields);
				}
				//Re-arrange the TabOrderMobile. 1 based.
				int tabOrderMobile=1;
				foreach(SheetFieldDef sheetField in listMobileSheetDefs) {
					sheetField.TabOrderMobile=tabOrderMobile++;
				}
				var sheetDefNumsMobile=listMobileSheetDefs.Where(x => x.TabOrderMobile>0).Select(x => x.SheetFieldDefNum);
				List<SheetFieldDef> listNonMobileSheetDefs=_sheetDef.SheetFieldDefs.Where(x => x.Language==languageCheck && !sheetDefNumsMobile.Contains(x.SheetFieldDefNum)).ToList();
				//Give all non mobile sheet defs tab order of 0. This will exclude them from the mobile web layout.
				listNonMobileSheetDefs.ForEach(x => x.TabOrderMobile=0);
				var checkBoxes=panelPreview.Controls.Cast<SheetEditMobilePanel>()
					.SelectMany(x => x.Controls.Cast<Control>(),(x,y) => new {
						Pnl = x,
						Ctrl = y,
						CtrlTag = (SheetFieldDef)((y.Tag is SheetFieldDef) ? y.Tag : null),
						PnlTag = (SheetFieldDef)((x.Tag is SheetFieldDef) ? x.Tag : null),
					})
					.Where(x => x.Ctrl is SheetEditMobileRadioButton);
				//Only validate if required. User is allowed to save changes but not release to the web if SheetDef.HasMobileLayout==false.
				if(requireValidation && sheetDef.HasMobileLayout && listMobileSheetDefs.Any(x => {
					if(x.FieldType==SheetFieldType.StaticText) {
						//StaticText fields must have a valid FieldValue. UiLabelMobile is limited to 255 characters so we will use FieldValue where we might need more text.
						return string.IsNullOrEmpty(x.FieldValue);
					}
					if(x.FieldType==SheetFieldType.CheckBox) {
						var checkBox=checkBoxes.FirstOrDefault(y => y.CtrlTag.SheetFieldDefNum==x.SheetFieldDefNum);
						//All checkboxes should have valid Text (UiLabelMobileRadioButton).
						if(checkBox!=null && string.IsNullOrEmpty(checkBox.Ctrl.Text)) {
							return true;
						}
						if(x.FieldName!="misc") {
							if(
								//Has a group.
								!string.IsNullOrEmpty(x.FieldName) &&
								//Has not group caption.
								string.IsNullOrEmpty(x.UiLabelMobile)) 
							{
								//Invalid.
								return true;
							}
						}
					}
					//All other field types must have a valid UiLabelMobile.
					else if(string.IsNullOrEmpty(x.UiLabelMobile)) {
						return true;
					}
					return false;
				})) 
				{
					throw new Exception("All mobile fields must have a valid Mobile Caption. Turn off 'Use Mobile Layout' for this Sheet or set all Mobile Captions.");
				}
				//It is safe to make changes.
				List<SheetFieldDef> listAllSheetDefs=listMobileSheetDefs.Union(listNonMobileSheetDefs).ToList();
				foreach(var sfdOld in sheetDef.SheetFieldDefs) {
					var sfdNew=listAllSheetDefs.FirstOrDefault(x => x.SheetFieldDefNum==sfdOld.SheetFieldDefNum);
					//New fields being merged in will not exist yet in the list so don't try to save the TabOrderMobile in that case.
					if(sfdNew!=null) { 
						sfdOld.TabOrderMobile=sfdNew.TabOrderMobile;
					}
				}
				sheetDef.HasMobileLayout=_sheetDef.HasMobileLayout;
				return true;
			}
			catch(Exception e) {
				err?.Invoke(e.Message);
			}
			return false;
		}

		///<summary>Gets the fieldDefNums for Highlighted radio/checkbox controls.</summary>
		public List<long> GetHighlightedFieldDefs(List<long> listSheetFieldDefNums) {
			List<long> listHighlightedSheetFields=new List<long>();
			//Find any controls which these def nums belong to.
			var controlsToHighlight=panelPreview.Controls.Cast<SheetEditMobilePanel>()
				.SelectMany(x => x.Controls.Cast<Control>(), (x, y) => new {
					Pnl=x,
					Ctrl=y,
					CtrlTag=(SheetFieldDef)((y.Tag is SheetFieldDef)?y.Tag:null),
					PnlTag=(SheetFieldDef)((x.Tag is SheetFieldDef)?x.Tag:null),
				});
			controlsToHighlight
				//Panel has child control which holds the tag.
				.Where(x => x.CtrlTag!=null && listSheetFieldDefNums.Contains(x.CtrlTag.SheetFieldDefNum))
				.ForEach(x => {
					if(x.Ctrl is SheetEditMobileRadioButton && ((SheetEditMobileRadioButton)x.Ctrl).IsHighlighted) {
						listHighlightedSheetFields.Add(x.CtrlTag.SheetFieldDefNum);
					}
				});
			return listHighlightedSheetFields;
		}

		///<summary>Adds a highlighted border to panels which include any of the SheetFieldDefNums provided.</summary>
		public void SetHighlightedFieldDefs(List<long> listSheetFieldDefNums) {
			ClearHighlights();
			//Find any controls which these def nums belong to.
			var controlsToHighlight=panelPreview.Controls.Cast<SheetEditMobilePanel>()
				.SelectMany(x => x.Controls.Cast<Control>(), (x, y) => new {
					Pnl=x,
					Ctrl=y,
					CtrlTag=(SheetFieldDef)((y.Tag is SheetFieldDef)?y.Tag:null),
					PnlTag=(SheetFieldDef)((x.Tag is SheetFieldDef)?x.Tag:null),
				});
			controlsToHighlight
				//Panel has child control which holds the tag (everything except MobileHeader).
				.Where(x => x.CtrlTag!=null && listSheetFieldDefNums.Contains(x.CtrlTag.SheetFieldDefNum))
				.ForEach(x => {
					if(x.Ctrl is SheetEditMobileRadioButton) {
						((SheetEditMobileRadioButton)x.Ctrl).IsHighlighted=true;
					}
					if(x.Pnl is SheetEditMobilePanel) {
						x.Pnl.IsHighlighted=true;
					}
				});
			//Panel itself holds the tag (only MobileHeader).
			controlsToHighlight
			.Where(x => x.PnlTag!=null&&listSheetFieldDefNums.Contains(x.PnlTag.SheetFieldDefNum))
				.ForEach(x => {
					if(x.Pnl is SheetEditMobilePanel) {
						x.Pnl.IsHighlighted=true;
					}
				});
		}

		///<summary>Clear all highlighting previously set by SetHighlightedFieldDefs().</summary>
		public void ClearHighlights() {
			panelPreview.Controls.Cast<SheetEditMobilePanel>()
				.ForEach(x => {
					x.IsHighlighted=false;
					x.Controls.Cast<Control>()
						.Where(y => y is SheetEditMobileRadioButton)
						.Select(y => (SheetEditMobileRadioButton)y)
						.ForEach(y => y.IsHighlighted=false);
				});
		}
		#endregion

		#region Scrolling
		private void scrollTimer_Tick(object sender,EventArgs e) {
			if(panelPreview.ClientRectangle.Contains(panelPreview.PointToClient(MousePosition))) {
				Point p=panelPreview.AutoScrollPosition;
				panelPreview.AutoScrollPosition=new Point(-p.X,-p.Y+_scrollJump);
			}
			else {
				scrollTimer.Enabled=false;
			}
		}

		///<summary>Move scroll positions as we are dragging and approaching the top/bottom.</summary>
		private void panel_DragOver(object sender,DragEventArgs e) {
			Point p=panelPreview.PointToClient(new Point(e.X, e.Y));
			if(p.Y<50) { //Near the top, scroll up.
				_scrollJump=-20;
				scrollTimer.Enabled=true;
			}
			else if(p.Y>panelPreview.ClientSize.Height-50) { //Near the bottom, scroll down.
				_scrollJump=20;
				scrollTimer.Enabled=true;
			}
			else { //Stop scrolling.
				scrollTimer.Enabled=false;
			}
		}
		#endregion

		#region Events
		///<summary></summary>
		private void TryStartDragging(IHasDragAttributes dragAttr,Point mouseLoc) {
			if(IsReadOnly) {
				return;
			}
			if(!dragAttr.DragAtt.CanStartDragging) { //The mouse isn't down so we can't be dragging.
				return;
			}
			if(!dragAttr.DragAtt.HasStartedDragging(mouseLoc)) { //We have not moved the mouse far enough to be considered a drag so don't start yet.
				return;
			}
			//We have now moved the mouse far enough to be considered a drag so start the drag and disable the click. 
			//This mouse down can no longer be considered a click.
			Control ctrlDrag=dragAttr.DragAtt.Ctrl;
			dragAttr.DragAtt.DisableMouseEvents();
			//Change this control's cursor to an image of this label.
			_dragBitmap=new Bitmap(ctrlDrag.Width,ctrlDrag.Height);
			ctrlDrag.DrawToBitmap(_dragBitmap,new Rectangle(Point.Empty,_dragBitmap.Size));
			_dragBitmap.MakeTransparent(Color.White);
			_dragCursor=new Cursor(_dragBitmap.GetHicon());
			//Start dragging. The drag data is the unique name of the control. 
			//We will look for this and find the control when we need to find which panel we are dragging.
			this.DoDragDrop(ctrlDrag,DragDropEffects.Move);
		}

		///<summary>Anchoring does not work when the parent control is a Panel. Use brute force anchoring.</summary>
		private void panelPreview_Resize(object sender,EventArgs e) {
			panelPreview.Controls.Cast<Control>().ForEach(x => x.Width=panelPreview.Width-22);
		}

		private void panel_DragEnter(object sender,DragEventArgs e) {
			//The data being dragged should be a control on this form.
			Control control=(Control)e.Data.GetData(e.Data.GetFormats()[0]);
			if(control==null) {
				return;
			}
			if(this.Controls.Find(control.Name,true).Count()<=0) {
				return;
			}
			//Allow the move effect over this sender control.
			e.Effect=DragDropEffects.Move;
		}

		private void panel_DragDrop(object sender,DragEventArgs e) {
			//Stop auto scrolling if we are near the top/bottom.
			scrollTimer.Enabled=false;
			//Dispose drag cursor and restore to default.
			Cursor.Current=Cursors.Default;
			ODException.SwallowAnyException(() => {
				_dragCursor.Dispose();
				_dragCursor=null;
				_dragBitmap.Dispose();
				_dragBitmap=null;
			});
			Control controlDroppedOnto=(Control)sender;
			Control controlDragging=(Control)e.Data.GetData(e.Data.GetFormats()[0]);
			if(controlDragging==null) {
				return;
			}
			SheetEditMobilePanel panelDrop=null;
			SheetEditMobilePanel panelDragging=null;
			bool isPanelDragging=true;
			//if it's a checkbox don't do color dragging logic and dont set to panel
			if(controlDragging.Tag!=null && ((SheetFieldDef)controlDragging.Tag).FieldType==SheetFieldType.CheckBox) {
				isPanelDragging=false;
			}
			else {
				panelDragging=(SheetEditMobilePanel)controlDragging;
			}
			int newControlIndex=-1;
			if(controlDragging is SheetEditMobileRadioButton) {
				if(!(controlDroppedOnto is SheetEditMobileRadioButton)) {
					return;
				}
				//A radio item dropped onto another radio item. Change the dropped radio item's child index.
				panelDrop=(SheetEditMobilePanel)controlDroppedOnto.Parent;
				newControlIndex=panelDrop.Controls.IndexOf(controlDroppedOnto);
				panelDrop.Controls.SetChildIndex(controlDragging,newControlIndex);
				if(isPanelDragging && panelDragging.NeedsMove) {
					panelDragging.NeedsMove=false;
				}
				Color panelDropColor=panelDrop.NeedsMove ? _colorFieldsNeedMove : _colorPanelBG;
				SetControlAndChildrenBackColor(panelDrop,panelDropColor);
				if(isPanelDragging) {
					Color panelDraggingColor=panelDragging.NeedsMove ? _colorFieldsNeedMove : _colorPanelBG;
					SetControlAndChildrenBackColor(panelDragging,panelDraggingColor);
				}
				
			}
			else {				
				if(controlDroppedOnto!=panelPreview) { //Dropped onto a sub-control so find its position in the panel.
					if(controlDroppedOnto is SheetEditMobilePanel) { //Dropped onto whitespace on a panel.
						panelDrop=(SheetEditMobilePanel)controlDroppedOnto;
					}
					else { //Dropped onto a control on a panel (button, label, etc)
						panelDrop=(SheetEditMobilePanel)controlDroppedOnto.Parent;
					}
					newControlIndex=panelPreview.Controls.IndexOf(panelDrop);
					if(isPanelDragging && panelDragging.NeedsMove) {
						panelDragging.NeedsMove=false;
					}
					Color panelDropColor=panelDrop.NeedsMove ? _colorFieldsNeedMove : _colorPanelBG;
					SetControlAndChildrenBackColor(panelDrop,panelDropColor);
					if(isPanelDragging) {
						Color panelDraggingColor=panelDragging.NeedsMove ? _colorFieldsNeedMove : _colorPanelBG;
						SetControlAndChildrenBackColor(panelDragging,panelDraggingColor);
					}
				}
				if(newControlIndex>=0) { //Position at the exact index it was dropped.
					panelPreview.Controls.SetChildIndex(controlDragging,newControlIndex);
				}
			}		
		}

		private void checkUseMobileLayout_CheckedChanged(object sender,EventArgs e) {
			if(!checkUseMobileLayout.Checked && _sheetDef!=null && _sheetDef.SheetDefNum>0 && EClipboardSheetDefs.IsSheetDefInUse(_sheetDef.SheetDefNum)) {
				MsgBox.Show("This sheet is currently being used by eClipboard, which requires sheets to have a mobile layout. " +
					"You must remove this form from eClipboard rules before you can remove the mobile layout for this sheet.");
				checkUseMobileLayout.Checked=true;
			}
			else {
				if(_sheetDef!=null) {
					_sheetDef.HasMobileLayout=checkUseMobileLayout.Checked;
				}
				HasMobileLayoutChanged?.Invoke(this,checkUseMobileLayout.Checked);
			}
		}
		
		private void butAddHeader_Click(object sender,EventArgs e) {
			AddMobileHeader?.Invoke(this,new EventArgs());
		}

		///<summary>Set the back color of the ctrl provided and any of its top-level children.</summary>
		private void SetControlAndChildrenBackColor(Control ctrl,Color color) {
			if(IsReadOnly) {
				return;
			}
			ctrl.BackColor=color;
			ctrl.Controls.Cast<Control>().ForEach(x => x.BackColor=color);
		}

		///<summary>Fires as a control is being dragged over the control.</summary>
		private void SheetEditMobileCtrl_GiveFeedback(object sender,GiveFeedbackEventArgs e) {
			if(_dragCursor==null) {
				return;
			}
			if(e.Effect!=DragDropEffects.Move) {
				return;
			}
			//Use the cursor we created on mouse down event.
			e.UseDefaultCursors=false;
			Cursor.Current=_dragCursor;
		}

		///<summary>Orders the fields in the mobile view as close to the desktop order as possible.</summary>
		private void butOrderFields_Click(object sender,EventArgs e) {
			_tabOrderMobileToMimicTabOrder=true;
			//This will redraw the panel with the fields re-ordered. Re-ordering of fields happens in MergeMobileFields because SheetFieldDefs
			//has no set.
			SheetDef=_sheetDef;
			int numUnorderableMobileFields=SheetFieldDefs
				.Where(x => IsUnOrderable(x) && x.TabOrderMobile>0)
				.Count();
			//User needs to rearrage unorderable fields to put them in the correct position
			if(numUnorderableMobileFields>0) {
				MessageBox.Show(Lan.g(this,"One or more fields were found that cannot be ordered.\n" +
					"Please drag and drop the fields to the correct location and click OK in the main window to save the mobile order."));
			}
			else {
				MessageBox.Show(Lan.g(this,"Fields ordered."));
			}
		}

		///<summary>Change the show/hide state of the modeless popup. If forceOpen is true then always opens. If false then opens if currently closed or closes if currently open.</summary>
		public void ShowHideModeless(bool forceOpen,string title,Form owner,Rectangle bounds) {
			string finalTitle=GetTranslation("Mobile Preview")+" - "+title;
			if(_formFloat!=null) { //Already floating so just close.
				if(forceOpen) {
					_formFloat.Text=title;
					_formFloat.BringToFront();
				}
				else {
					_formFloat.Close();
				}
				return;
			}
			_formFloat=new Form() {
				MinimizeBox=false,
				MaximizeBox=false,
				ShowIcon=false,
				ShowInTaskbar=false,
				ControlBox=false,
				Size=this.Size,
				Text=title,
				Owner=owner
			};
			this.Location=new Point(0,0);
			this.Dock=DockStyle.Fill;
			_formFloat.Controls.Add(this);
			_formFloat.FormClosing+=new FormClosingEventHandler((sender1,e1) => {
				_formFloat.Controls.Remove(this);
				_formFloat=null;
			});
			//Non-modal so it floats.
			_formFloat.Bounds=bounds;
			_formFloat.Show();//this makes it move, so set bounds again
			_formFloat.Bounds=bounds;
		}
		#endregion

		///<summary></summary>
		public void UpdateLanguage(string selectedLanguage) {
			_languageCur=selectedLanguage;
		}
	}

	#region Classes and Interfaces
	///<summary>Draws an outline when highlighted.</summary>
	public class SheetEditMobileRadioButton:CheckBox, IHasDragAttributes {
		public DragAttributes DragAtt { get; set; }
		private bool _isHighlighted=false;
		private Rectangle _rText=Rectangle.Empty;
		Rectangle _rDraggable=Rectangle.Empty;
		private bool _isOverText=false;
		public event EventHandler TextClick;
		public event EventHandler Selected;
		private Func<bool> _isReadOnly;
		///<summary>Set to true to draw a red border around this panel. False removes the border.</summary>
		public bool IsHighlighted {
			get {
				return _isHighlighted;
			}
			set {
				_isHighlighted=value;
				Invalidate(false);
			}
		}
		public SheetEditMobileRadioButton(Func<bool> isReadOnly) {
			_isReadOnly=isReadOnly;
			DragAtt=new DragAttributes(this);
			this.MouseMove+=new MouseEventHandler((o,e) => {
				if(_isReadOnly!=null && _isReadOnly()) {
					return;
				}
				this.Cursor=_rDraggable.Contains(e.Location) ? Cursors.SizeAll : Cursors.Default;
				_isOverText=_rText.Contains(e.Location);
				this.Invalidate(_rText);
			});
			this.MouseLeave+=new EventHandler((o,e) => {
				if(_isReadOnly!=null&&_isReadOnly()) {
					return;
				}
				this.Cursor=Cursors.Default;
				_isOverText=false;
				this.Invalidate(_rText);
			});
			this.MouseDown+=new MouseEventHandler((sender,e) => {				
				if(e.Button!=MouseButtons.Left) {
					return;
				}
				//Mouse-down will initiate both click and drag. 
				//Start both here and determine if we are clicking or dragging once we start moving the mouse.
				DragAtt.StartMouseDown(e.Location);
			});
			this.MouseUp+=new MouseEventHandler((sender,e) => {				
				if(DragAtt.CanMouseUp) { //We have not moved the mouse far enough to be considered a drag so it must be a click.
					Selected?.Invoke(this,new EventArgs());
				}
				//Reset the panel for click and drag for next time.
				DragAtt.DisableMouseEvents();
				if(_isReadOnly!=null&&_isReadOnly()) {
					return;
				}
				if(_rText.Contains(e.Location)) {
					TextClick?.Invoke(this,e);
				}
			});
		}

		protected override void OnPaint(PaintEventArgs pevent) {
			pevent.Graphics.Clear(BackColor);
			Rectangle rBounds=new Rectangle(0,0,this.Width-2,this.Height-2);
			Rectangle rDotExterior=new Rectangle(rBounds.Right-rBounds.Height,rBounds.Top,rBounds.Height,rBounds.Height);
			//Text.
			using(SolidBrush brush=new SolidBrush(this.ForeColor)) {
				int textWidth=(int)Math.Ceiling(pevent.Graphics.MeasureString(this.Text,this.Font).Width);
				//Add 4px padding and assure min width.
				int width=Math.Max(textWidth+4,20);
				//Add 3px margin.
				_rText=new Rectangle(rDotExterior.Left-width-3,rBounds.Top,width,rBounds.Height);
				ControlPaint.DrawButton(pevent.Graphics,_rText,_isOverText ? ButtonState.Checked : ButtonState.Normal);
				if(string.IsNullOrEmpty(Text)) { //Missing required field. Fill with red.
					pevent.Graphics.FillRectangle(Brushes.Red,new Rectangle(_rText.Left+2,_rText.Top+2,_rText.Width-4,_rText.Height-4));
				}
				else {
					pevent.Graphics.DrawString(this.Text,this.Font,brush,_rText,new StringFormat() { Alignment=StringAlignment.Center,LineAlignment=StringAlignment.Center });
				}
			}
			//Draw highlight outline.
			if(_isHighlighted) {
				using(Pen p=new Pen(Color.Black,1)) {
					p.DashPattern=new float[]{ 2,2 };
					pevent.Graphics.DrawRectangle(p,rBounds);
				}
			}			
			//Dot exterior.
			pevent.Graphics.DrawEllipse(Pens.Black,rDotExterior);
			//Set draggable region for cursor.
			_rDraggable=new Rectangle(0,0,_rText.Left,this.Height);
		}
	}

	///<summary>Draws an outline when highlighted and manages width and padding. Decides if it is ok to click/drag.</summary>
	public class SheetEditMobilePanel:FlowLayoutPanel, IHasDragAttributes {
		private const int PEN_WIDTH=2;
		private const int DEFAULT_HEIGHT=60;
		private bool _isHighlighted=false;
		private bool _isHeaderValid=false;
		///<summary>Displays double-click command in top-right.</summary>
		private string _buttonLabel="";
		public string ButtonLabel {
			get {
				return _buttonLabel;
			}
			set {
				_buttonLabel=value;
				Invalidate();
			}
		}
		private string _header="";
		///<summary>Displays UiLabelMobile in the top-left.</summary>
		public string Header {
			get {
				return _header;
			}
			set {
				_header=value;
				Invalidate();
			}
		}
		///<summary>Set to true to draw a red border around the header. False removes the border.</summary>
		public bool IsHeaderValid {
			get {
				return _isHeaderValid;
			}
			set {
				_isHeaderValid=value;
				Invalidate(false);
			}
		}
		private string _fieldType="";
		///<summary>Displays FieldType in bottom-left.</summary>
		public string FieldType {
			get {
				return _fieldType;
			}
			set {
				_fieldType=value;
				Invalidate();
			}
		}
		private string _mobileHeader="";
		///<summary>Sets the text displayed in the middle-center of this control. Only used for MobileHeader.</summary>
		public string MobileHeader {
			get {
				return _mobileHeader;
			}
			set {
				_mobileHeader=value;
				Invalidate();
			}
		}
		public DragAttributes DragAtt { get; set; }
		///<summary>Set to true to draw a red border around this panel. False removes the border.</summary>
		public bool IsHighlighted {
			set {
				_isHighlighted=value;
				Invalidate(false);
			}
		}
		///<summary>Returns the current width of this panel minus its right/left padding minus the width of its border.
		///This can be used to determine the width of any child control which is added to this panel. Also used to resize child controls.</summary>
		public int ChildControlWidth {
			get {
				return this.Width-(PEN_WIDTH*4);
			}
		}
		///<summary>Used with mobile tab order when order cannot be determined. This item could not be ordered and will need to be ordered manually.</summary>
		public bool NeedsMove { get; set; }
		private bool _isOverBounds=false;
		private Func<bool> _isReadOnly;

		///<summary>Sets several pertinent fields that make this a SheetEditMobilePanel instead of a plain FlowLayoutPanel.</summary>
		public SheetEditMobilePanel(Func<bool> isReadOnly) {
			_isReadOnly=isReadOnly;
			Height=DEFAULT_HEIGHT;
			FlowDirection=FlowDirection.TopDown;
			BorderStyle=BorderStyle.None;
			WrapContents=false;
			AllowDrop=true;
			Margin=new Padding(0,0,0,10);
			Padding=new Padding(0,20,0,0);
			SetStyle(ControlStyles.UserPaint|ControlStyles.ResizeRedraw|ControlStyles.DoubleBuffer|ControlStyles.AllPaintingInWmPaint,true);
			DragAtt=new DragAttributes(this);
			//Adding a fake control at the top will set the alignment for the rest of the controls.
			//https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/how-to-anchor-and-dock-child-controls-in-a-flowlayoutpanel-control
			Controls.Add(new Label() {
				Size=new Size(ChildControlWidth,0),
				AutoSize=false,
			});
			this.MouseMove+=new MouseEventHandler((o,e) => {
				if(_isReadOnly!=null&&_isReadOnly()) {
					return;
				}
				_isOverBounds=true;
				this.Invalidate(this.Bounds);
			});
			this.MouseLeave+=new EventHandler((o,e) => {
				if(_isReadOnly!=null&&_isReadOnly()) {
					return;
				}
				_isOverBounds=ClientRectangle.Contains(PointToClient(MousePosition));
				this.Invalidate(this.Bounds);
			});
			//Anchoring does not work when the parent control is a Panel. Use brute force anchoring.
			this.Resize+=new EventHandler((sender,e) => {
				Controls[0].Width=ChildControlWidth;
			});
		}

		///<summary>Draw a border. Black if not highlighted, red if highlighted.</summary>
		protected override void OnPaintBackground(PaintEventArgs e) {
			e.Graphics.Clear(this.BackColor);
			Color color=Color.Black;
			float penWidth=PEN_WIDTH/2;
			if(_isHighlighted) {
				color=Color.Red;
				penWidth=PEN_WIDTH;
			}
			Rectangle rBounds=new Rectangle(
				(int)penWidth,
				(int)penWidth,
				ClientSize.Width-(2*PEN_WIDTH),
				ClientSize.Height-(2*PEN_WIDTH));
			//Draw border.
			using(Pen pen = new Pen(color,penWidth)) {
				e.Graphics.DrawRectangle(pen,rBounds);
			}
			using(Brush b = new SolidBrush(this.ForeColor)) {
				int footerHeight=20;
				//Draw FieldType footer and capture height.
				using(Font f = new Font(this.Font,FontStyle.Italic)) {
					footerHeight=(int)Math.Ceiling(e.Graphics.MeasureString(FieldType,f).Height)+3;
					e.Graphics.DrawString(FieldType,f,b,2,this.Height-footerHeight);
					//Draw 'Double-click to edit' label.
					if(_isOverBounds) {
						int width=(int)Math.Ceiling(e.Graphics.MeasureString(ButtonLabel,this.Font).Width)+6;
						e.Graphics.DrawString(_buttonLabel,this.Font,b,this.Width-width,this.Height-footerHeight);
					}
				}
				if(string.IsNullOrEmpty(MobileHeader)) {
					//Draw outline when necessary.
					if(!IsHeaderValid) { 
						var sz=e.Graphics.MeasureString(Header,this.Font);
						e.Graphics.FillRectangle(Brushes.Red,4,4,sz.Width,sz.Height);
						e.Graphics.DrawRectangle(Pens.Black,4,4,sz.Width,sz.Height);
					}
					//Draw UiLabelMobile.
					e.Graphics.DrawString(Header,this.Font,IsHeaderValid ? b : Brushes.White,4,4);
				}
				else {
					//Draw Mobile Header
					int top=20;
					if(this.Controls.Count==1) { //The bottom of our fake label we inserted in the ctor.
						top=this.Controls[0].Bottom;
					}
					var headerSize=e.Graphics.MeasureString(MobileHeader,this.Font,rBounds.Width);
					int allowedHeight=DEFAULT_HEIGHT-top-footerHeight;
					int newHeight=this.Height;
					if(headerSize.Height>allowedHeight) {
						newHeight=top+footerHeight+(int)headerSize.Height;
					}
					else if(headerSize.Height<allowedHeight) {
						newHeight=DEFAULT_HEIGHT;
					}
					if(this.Height!=newHeight) {
						this.Height=newHeight;
					}
					e.Graphics.DrawString(
						MobileHeader,
						this.Font,
						b,
						new RectangleF(
							2+(rBounds.Width-headerSize.Width)/2,
							top+(this.Height-top-headerSize.Height-footerHeight)/2,
							headerSize.Width,
							headerSize.Height));
				}
			}
		}
	}

	///<summary>When there's only 1 control in the panel and the user clicks on it, .NET tries to scroll to the control. 
	///This invariably forces the panel to scroll up. This little hack prevents that.</summary>
	public class SheetEditMobilePreviewPanel:FlowLayoutPanel {
		bool _allowScroll=false;
		protected override Point ScrollToControl(Control activeControl) {
			return _allowScroll ? base.ScrollToControl(activeControl) : DisplayRectangle.Location;
		}
		
		/// <summary>Scrolls the specified child control into view with default behavior. 
		/// Use this when you actually want to scroll to the control.</summary>
		public void ScrollControlIntoView(Control activeControl,bool allowScroll) {
			_allowScroll=allowScroll;
			base.ScrollControlIntoView(activeControl);
			_allowScroll=false;
		}
	}

	///<summary>Indicates that implementer has a DragAttributes instance and can be drag/dropped.</summary>
	public interface IHasDragAttributes {
		DragAttributes DragAtt { get; set; }
	}

	///<summary>Helper class to be used by customer controls to facilitate drag and drop much easier.</summary>
	public class DragAttributes {
		public Control Ctrl {
			private set;
			get;
		}
		private Point _mouseDownStart=Point.Empty;
		public bool CanStartDragging { get; private set; }
		public bool CanMouseUp { get; private set; }

		public DragAttributes(Control control) {
			Ctrl=control;
		}

		///<summary>Returns true if X or Y position of mouse has changed by 10 pixels since mouse has been down.</summary>
		public bool HasStartedDragging(Point location) {
			return Math.Abs(location.X-_mouseDownStart.X)>10||Math.Abs(location.Y-_mouseDownStart.Y)>10;
		}

		///<summary>Call on MouseDown to indicate that this panel is now eligible for click or drag.</summary>
		public void StartMouseDown(Point location) {
			_mouseDownStart=location;
			CanMouseUp=true;
			CanStartDragging=true;
		}

		///<summary>Call on MouseUp to indicate that this panel is no longer eligible for click or drag.</summary>
		public void DisableMouseEvents() {
			CanStartDragging=false;
			CanMouseUp=false;
			_mouseDownStart=Point.Empty;
		}
	}
	#endregion
}
