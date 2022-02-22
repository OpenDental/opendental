using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace ODR{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public class FormParameterInput : System.Windows.Forms.Form{
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panelMain;
		private ParameterCollection Parameters;
		///<summary>These are the actual input fields that get laid out on the window based on the type.</summary>
		private Control[] inputs;
		private Label[] labels;

		///<summary></summary>
		public FormParameterInput(ref ParameterCollection parameters){
			InitializeComponent();// Required for Windows Form Designer support
			//Lan.C("All", new System.Windows.Forms.Control[] {
			//	butOK,
			//	butCancel,
			//});
			//MultInput2.MultInputItems=new MultInputItemCollection();
			Parameters=parameters;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormParameterInput));
			this.butCancel = new System.Windows.Forms.Button();
			this.butOK = new System.Windows.Forms.Button();
			this.panelMain = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(525,275);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(435,275);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// panelMain
			// 
			this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelMain.AutoScroll = true;
			this.panelMain.Location = new System.Drawing.Point(20,12);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(580,247);
			this.panelMain.TabIndex = 2;
			// 
			// FormParameterInput
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(628,315);
			this.Controls.Add(this.panelMain);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormParameterInput";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Enter Parameters";
			this.Load += new System.EventHandler(this.FormParameterInput_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormParameterInput_Load(object sender, System.EventArgs e) {
			//For testing
			/*
			Parameters=new ParameterCollection();

			Parameter param=new Parameter();
			param.Prompt="You should put in some text in the box to the right";
			param.ValueType=ParamValueType.String;
			Parameters.Add(param);

			param=new Parameter();
			param.Prompt="Prompt";
			param.ValueType=ParamValueType.String;
			Parameters.Add(param);

			param=new Parameter();
			param.Prompt="Yet another prompt for info";
			param.ValueType=ParamValueType.String;
			Parameters.Add(param);

			param=new Parameter();
			param.Prompt="Testing checkbox";
			param.ValueType=ParamValueType.Boolean;
			Parameters.Add(param);

			param=new Parameter();
			param.Prompt="Testing Date";
			param.ValueType=ParamValueType.Date;
			Parameters.Add(param);

			param=new Parameter();
			param.Prompt="Testing Number";
			param.ValueType=ParamValueType.Number;
			Parameters.Add(param);
			
			param=new Parameter();
			param.Prompt="Select Dental Specialty";
			param.ValueType=ParamValueType.Enum;
			param.EnumerationType=EnumType.DentalSpecialty;
			Parameters.Add(param);
		
			//still need query*/
			
			Graphics g=this.CreateGraphics();
			this.SuspendLayout();
			int maxH=100;//600
			int requiredH=ArrangeControls(g);
			if(requiredH>=maxH){
				this.Height=maxH+(Height-panelMain.Height);
			}
			else{
				this.Height=requiredH+(Height-panelMain.Height);
			}
			/*if(requiredH>Height-2 && requiredH<maxH){//resize, but no scrollbar
				//this should be skipped on second pass because height adequate
				Height=requiredH+2;
				ResumeLayout();
				return;//Layout will be triggered again
			}
			if(requiredH>Height-2){//if the controls were too big to fit even with H at max
				vScrollBar2.Visible=true;
				panelSlide.Width-=vScrollBar2.Width;
				ArrangeControls(g);//then layout again, but we don't care about the H
			}*/
			g.Dispose();
			ResumeLayout();
		}

		/// <summary>Returns the required height.</summary>
		private int ArrangeControls(Graphics g){
			//580 is the initial width of the panel
			int inputW=280;//The input section on the right.
			int promptW=580-20-inputW;//20 is for the scrollbar on the right
			panelMain.Controls.Clear();
			int yPos=5;
			int itemH=0;//item height
			labels=new Label[Parameters.Count];
			inputs=new Control[Parameters.Count];
			for(int i=0;i<Parameters.Count;i++){
				//Calculate height
				itemH=(int)g.MeasureString(Parameters[i].Prompt,Font,promptW).Height;
				if(itemH<20)
					itemH=20;
				//promptingText
				labels[i]=new Label();
				labels[i].Location=new Point(5,yPos);
				//labels[i].Name="Label"+i.ToString();
				labels[i].Size=new Size(promptW-8,itemH);
				labels[i].Text=Parameters[i].Prompt;
				labels[i].TextAlign=ContentAlignment.MiddleRight;
				//labels[i].BorderStyle=BorderStyle.FixedSingle;//just used in debugging layout
				panelMain.Controls.Add(labels[i]);
				if(Parameters[i].ValueType==ParamValueType.Boolean){
					//add a checkbox
					inputs[i]=new CheckBox();
					inputs[i].Location=new Point(promptW,yPos+(itemH-20)/2);
					inputs[i].Size=new Size(inputW-5,20);
					if(Parameters[i].CurrentValues.Count==0)
						((CheckBox)inputs[i]).Checked=false;
					else
						((CheckBox)inputs[i]).Checked=true;
					((CheckBox)inputs[i]).FlatStyle=FlatStyle.System;
					panelMain.Controls.Add(inputs[i]);
				}
				else if(Parameters[i].ValueType==ParamValueType.Date){
					//add a validDate box
					inputs[i]=new ValidDate();
					inputs[i].Location=new Point(promptW,yPos+(itemH-20)/2);
					if(inputW<100){//not enough room for a fullsize box
						inputs[i].Size=new Size(inputW-20,20);
					}
					else{
						inputs[i].Size=new Size(75,20);
					}
					;
					if(Parameters[i].CurrentValues.Count>0){
						DateTime myDate=(DateTime)Parameters[i].CurrentValues[0];
						inputs[i].Text=myDate.ToShortDateString();
					}
					panelMain.Controls.Add(inputs[i]);
				}
				else if(Parameters[i].ValueType==ParamValueType.Enum){
					//add a psuedo combobox filled with values for one enumeration
					inputs[i]=new ComboBoxMulti();
					Type eType=Type.GetType("ODR."+Parameters[i].EnumerationType.ToString());
					for(int j=0;j<Enum.GetNames(eType).Length;j++){
						((ComboBoxMulti)inputs[i]).Items.Add(Enum.GetNames(eType)[j]);
						if(Parameters[i].CurrentValues.Count > 0
							&& Parameters[i].CurrentValues
							.Contains((int)(Enum.Parse(eType,Enum.GetNames(eType)[j])))  ) {
							((ComboBoxMulti)inputs[i]).SetSelected(j,true);
						}
					}
					inputs[i].Location=new Point(promptW,yPos+(itemH-20)/2);
					inputs[i].Size=new Size(inputW-5,20);
					panelMain.Controls.Add(inputs[i]);
				}
				else if(Parameters[i].ValueType==ParamValueType.Integer){
					//add a validNumber box
					inputs[i]=new ValidNumber();
					inputs[i].Location=new Point(promptW,yPos+(itemH-20)/2);
					if(inputW<100){//not enough room for a fullsize box
						inputs[i].Size=new Size(inputW-20,20);
					}
					else{
						inputs[i].Size=new Size(75,20);
					}
					if(Parameters[i].CurrentValues.Count>0){
						inputs[i].Text=((int)Parameters[i].CurrentValues[0]).ToString();
					}
					panelMain.Controls.Add(inputs[i]);
				}
				else if(Parameters[i].ValueType==ParamValueType.Number){
					//add a validDouble box
					inputs[i]=new ValidDouble();
					inputs[i].Location=new Point(promptW,yPos+(itemH-20)/2);
					if(inputW<100){//not enough room for a fullsize box
						inputs[i].Size=new Size(inputW-20,20);
					}
					else{
						inputs[i].Size=new Size(75,20);
					}
					if(Parameters[i].CurrentValues.Count>0){
						inputs[i].Text=((double)Parameters[i].CurrentValues[0]).ToString("n");
					}
					panelMain.Controls.Add(inputs[i]);
				}
				else if(Parameters[i].ValueType==ParamValueType.String){
					//add a textbox
					inputs[i]=new TextBox();
					inputs[i].Location=new Point(promptW,yPos+(itemH-20)/2);
					//inputs[i].Name=
					inputs[i].Size=new Size(inputW-5,20);
					if(Parameters[i].CurrentValues.Count>0){
						inputs[i].Text=Parameters[i].CurrentValues[0].ToString();
					}
					panelMain.Controls.Add(inputs[i]);
				}
				yPos+=itemH+5;
				//if(yPos>panelMain.Height && !vScrollBar2.Visible)
				//	return yPos;//There's not enough room, so stop and make the scrollbar visible.
			}
			//panelSlide.Height=yPos;
			//vScrollBar2.Maximum=panelSlide.Height;
			//vScrollBar2.Minimum=0;
			//vScrollBar2.LargeChange=panelMain.Height;
			//vScrollBar2.SmallChange=5;
			return yPos;
		}

		/*
		///<summary></summary>
		public void AddInputItem(string myPromptingText,ParamValueType myValueType,ArrayList myCurrentValues,EnumType myEnumerationType,DefCat myDefCategory,ReportFKType myFKType){
			MultInput2.AddInputItem(myPromptingText,myValueType,myCurrentValues,myEnumerationType,myDefCategory,myFKType);
		}*/

		/*
		///<summary>After this form closes, use this method to retrieve the data that the user entered.</summary>
		public ArrayList GetCurrentValues(int itemIndex){
			return MultInput2.GetCurrentValues(itemIndex);
		}*/

		//private void contrParamInput_SizeChanged(object sender, System.EventArgs e) {
		//	Height=contrParamInput.Bottom+90;
		//	Refresh();//this should trigger another layout
		//}

		private void butOK_Click(object sender, System.EventArgs e){
			//make sure all entries are valid
			for(int i=0;i<Parameters.Count;i++){
				if(Parameters[i].ValueType==ParamValueType.Date){
					if(((ValidDate)inputs[i]).errorProvider1.GetError(inputs[i])!=""){
						MessageBox.Show("Please fix data entry errors first.");
						return;
					}
				}
				if(Parameters[i].ValueType==ParamValueType.Integer){
					if(((ValidNumber)inputs[i]).errorProvider1.GetError(inputs[i])!=""){
						MessageBox.Show("Please fix data entry errors first.");
						return;

					}
				}
				if(Parameters[i].ValueType==ParamValueType.Number){
					if(((ValidDouble)inputs[i]).errorProvider1.GetError(inputs[i])!=""){
						MessageBox.Show("Please fix data entry errors first.");
						return;

					}
				}
			}
			//then fill the current values and output value.  For most fields, the length of CurrentValues will be 0 or 1.
			for(int i=0;i<Parameters.Count;i++){
				Parameters[i].CurrentValues=new ArrayList();
				if(Parameters[i].ValueType==ParamValueType.Boolean){
					if(((CheckBox)inputs[i]).Checked){
						Parameters[i].CurrentValues.Add(true);
					}
				}
				else if(Parameters[i].ValueType==ParamValueType.Date){
					Parameters[i].CurrentValues.Add(PIn.Date(inputs[i].Text));
				}
				/*else if(Parameters[i].ValueType==ParamValueType.Def){
					ComboBoxMulti comboBox=(ComboBoxMulti)inputs[i];
					for(int j=0;j<comboBox.SelectedIndices.Count;j++){
						retVal.Add(
							Defs.Short[(int)Parameters[i].DefCategory]
							[(int)comboBox.SelectedIndices[j]].DefNum);
					}
				}*/
				else if(Parameters[i].ValueType==ParamValueType.Enum){
					ComboBoxMulti comboBox=(ComboBoxMulti)inputs[i];
					Type eType=Type.GetType("ODR."+Parameters[i].EnumerationType.ToString());
					for(int j=0;j<comboBox.SelectedIndices.Count;j++){
						Parameters[i].CurrentValues.Add((int)(Enum.Parse(eType,Enum.GetNames(eType)[(int)comboBox.SelectedIndices[j]])));
					}
				}
				else if(Parameters[i].ValueType==ParamValueType.Integer){
					Parameters[i].CurrentValues.Add(PIn.Long(inputs[i].Text));
				}
				else if(Parameters[i].ValueType==ParamValueType.Number){
					Parameters[i].CurrentValues.Add(PIn.Double(inputs[i].Text));
				}
				else if(Parameters[i].ValueType==ParamValueType.String){
					if(inputs[i].Text!=""){
						//the text is first stripped of any ?'s
						Parameters[i].CurrentValues.Add(inputs[i].Text.Replace("?",""));//Regex.Replace(inputs[i].Text,@"\?",""));
					}
				}
				Parameters[i].FillOutputValue();
				//MessageBox.Show(multInputItems[1].CurrentValues.Count.ToString());
				//return retVal;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			//comboBox1.DroppedDown
			DialogResult=DialogResult.Cancel;
		}

	

		

		


	}
}





















