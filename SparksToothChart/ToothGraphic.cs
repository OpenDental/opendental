using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using OpenDentBusiness;
using SharpDX.Direct3D9;

namespace SparksToothChart {
	///<summary>A tooth graphic holds all the data for one tooth to be drawn.</summary>
	public class ToothGraphic {
		private string toothID;
		public List<VertexNormal> VertexNormals;
		///<summary>Corresponds to VertexNormal list, but DirectX required the verticies to be cached for rendering triangles.</summary>
		public VertexBuffer vb=null;
		///<summary>Collection of type ToothGroup.</summary>
		public List<ToothGroup> Groups;
		private bool visible;
		private static float[] DefaultOrthoXpos;
		///<summary>The rotation about the Y axis in degrees. Performed before any other rotation or translation.  Positive numbers are clockwise as viewed from occlusal.</summary>
		public float Rotate;
		///<summary>Rotation to the buccal or lingual about the X axis in degrees.  Positive numbers are to buccal. Happens 2nd, after rotateY.  Not common. Usually 0.</summary>
		public float TipB;
		///<summary>Rotation about the Z axis in degrees.  Aka tipping.  Positive numbers are mesial.  Happens last after all other rotations.</summary>
		public float TipM;
		///<summary>Shifting mesially or distally in millimeters.  Mesial is always positive, distal is negative.  Happens after all three rotations.</summary>
		public float ShiftM;
		///<summary>Shifting vertically in millimeters.  Aka supereruption.  Happens after all three rotations.  A positive number is super eruption, whether on the maxillary or mandibular.</summary>
		public float ShiftO;
		///<summary>Shifting buccolingually in millimeters.  Rare.  Usually 0.</summary>
		public float ShiftB;
		///<summary>This gets set to true if this tooth has an RCT.  The purpose of this flag is to cause the root to be painted transparently on top of the RCT.</summary>
		public bool IsRCT;
		///<summary>Set true to hide the number of the tooth from displaying.  Visible should also be set to false in this case.</summary>
		private bool hideNumber;
		///<summary>The X indicates that an extraction is planned.</summary>
		public bool DrawBigX;
		///<summary>If a big X is to be drawn, this will contain the color.</summary>
		public Color colorX;
		///<summary>If RCT, then this will contain the color.</summary>
		public Color colorRCT;
		///<summary>True if this tooth is set to show the primary letter in addition to the perm number.  This can only be true for 20 of the 32 perm teeth.  A primary tooth would never have this value set.  </summary>
		public bool ShowPrimaryLetter;
		//<summary>This gets set to true if tooth has a BU or a post.</summary>//no more.  Just set group visibility.
		//public bool IsBU;
		//<summary>If BU, then this will contain the color.</summary>//no more. Just use group color
		//public Color colorBU;
		///<summary>This gets set to true if tooth has an implant.</summary>
		public bool IsImplant;
		///<summary>If implant, then this will contain the color.</summary>
		public Color colorImplant;
		///<summary>This will be true if the tooth is a crown or retainer.  The reason it's needed is so that crowns will show on missing teeth with implants.</summary>
		public bool IsCrown;
		///<summary>True if the tooth is a bridge pontic, or a tooth on a denture or RPD.  This makes the crown visible from the F view, but not the occlusal.  It also makes the root invisible.</summary>
		public bool IsPontic;
		///<summary>This gets set to true if tooth has a sealant.</summary>
		public bool IsSealant;
		///<summary>If sealant, then this will contain the color.</summary>
		public Color colorSealant;
		///<summary>This gets set to true if tooth has a watch on it.</summary>
		public bool Watch;
		public Color colorWatch=Color.Red;
		///<summary>For perio, 1, 2, or 3.  It's a string because we'll later allow +.  The text gets drawn directly on the front of the tooth.</summary>
		public string Mobility;
		///<summary>If a mobility is set, the is the color.</summary>
		public Color colorMobility;

		///<summary>Shallow copy.  If the toothgraphic is being copied for use on a different DirectX device,
		///then pass in the toothgraphic for the other device in order to maintain the model data on the destination device.
		///If toothGraphicOtherDevice is not null, then return value will include shallow copies of vb and Groups from toothGraphicOtherDevice.</summary>
		public ToothGraphic Copy(ToothGraphic toothGraphicOtherDevice=null) {
			//We do not need a deep copy of VertexNormals, because the list is meant for read-only purposes.
			//We do not need a deep copy of vb, because the vertex buffer is meant for read-only purposes.
			//We do not need a deep copy of Groups, because the list is meant for read-only purposes.
			//We do not need a deep copy of DefaultOrthoXpos, because the array is meant for read-only purposes.
			ToothGraphic toothGraphicCopy=(ToothGraphic)this.MemberwiseClone();
			if(toothGraphicOtherDevice!=null) {
				//Some DirectX objects exist on one specific device and cannot be used on any other device without causing an exception.
				toothGraphicCopy.vb=toothGraphicOtherDevice.vb;//The vb is a VertexBuffer which exists on one specific device.
				toothGraphicCopy.Groups=toothGraphicOtherDevice.Groups;//The ToothGroup.facesDirectX is an IndexBuffer which exists on one specific device.
			}
			return toothGraphicCopy;
		}

		///<summary>Used in Clone()</summary>
		internal ToothGraphic() {
			
		}

		///<summary>Only called from ToothChartWrapper.ResetTeeth or from ToothChartOpenGL.ResetTeeth when program first loads.  Constructor requires passing in the toothID.  Exception will be thrown if not one of the following: 1-32 or A-T.  Always loads graphics data from local resources even if in simplemode.</summary>
		public ToothGraphic(string tooth_id) {
			if(tooth_id!="implant" && !IsValidToothID(tooth_id)) {
				///<summary>This will only happen if bugs in program</summary>
				throw new ApplicationException("Invalid tooth ID");
			}
			toothID=tooth_id;
			VertexNormals=new List<VertexNormal>();
			ImportObj();
			SetDefaultColors();
		}

		public override string ToString() {
			string retVal=this.toothID;
			if(IsRCT) {
				retVal+=", RCT";
			}
			return retVal;
		}

		#region properties

		///<summary>ToothID is set when creating a tooth object. It can never be changed.  Valid values are 1-32 or A-T.</summary>
		public string ToothID {
			get {
				return toothID;
			}
			set {
				//
			}
		}

		///<summary>Set false if teeth are missing.  All primary teeth initially set to false.</summary>
		public bool Visible {
			get {
				return visible;
			}
			set {
				visible=value;
			}
		}

		///<summary>Set true to hide the number of the tooth from displaying.  Visible should also be set to false in this case.</summary>
		public bool HideNumber {
			get {
				return hideNumber;
			}
			set {
				hideNumber=value;
			}
		}

		#endregion properties

		#region Public Methods

		///<summary>Converts the VertexNormals list into a vertex buffer so the verticies can be used to render DirectX triangles.</summary>
		public void PrepareForDirectX(Device deviceRef){
			CleanupDirectX();
			ToothChartVertexPosNormX[] arrayVerts=new ToothChartVertexPosNormX[VertexNormals.Count];
			for(int i=0;i<VertexNormals.Count;i++) {
				VertexNormal v=VertexNormals[i];
				arrayVerts[i]=new ToothChartVertexPosNormX(v.Vertex.X,v.Vertex.Y,v.Vertex.Z,v.Normal.X,v.Normal.Y,v.Normal.Z);
			}
			vb=new VertexBuffer(deviceRef,arrayVerts.Length*ToothChartVertexPosNormX.StrideSize,Usage.WriteOnly,ToothChartVertexPosNormX.Format,Pool.Managed);
			SharpDX.DataStream ds=vb.Lock(0,0,LockFlags.None);
			ds.WriteRange(arrayVerts);
			vb.Unlock();
			for(int g=0;g<Groups.Count;g++) {
				Groups[g].PrepareForDirectX(deviceRef);
			}
		}

		public void CleanupDirectX(){
			if(vb!=null) {
				vb.Dispose();
				vb=null;
			}
			for(int g=0;g<Groups.Count;g++){
				Groups[g].CleanupDirectX();
			}
		}

		///<summary>Resets this tooth graphic to original location, visiblity, and no restorations.  If primary tooth, then Visible=false.  </summary>
		public void Reset() {
			if(Tooth.IsPrimary(ToothID)) {
				visible=false;
			}
			else {
				visible=true;
			}
			TipM=0;
			TipB=0;
			Rotate=0;
			ShiftB=0;
			ShiftM=0;
			ShiftO=0;
			SetDefaultColors();
			IsRCT=false;
			hideNumber=false;
			DrawBigX=false;
			ShowPrimaryLetter=false;
			//IsBU=false;
			IsImplant=false;
			IsCrown=false;
			IsPontic=false;
			IsSealant=false;
			Watch=false;
		}

		public void SetSurfaceColors(string surfaces,Color color){
			for(int i=0;i<surfaces.Length;i++){
				if(surfaces[i]=='M'){
					SetGroupColor(ToothGroupType.M,color);
					SetGroupColor(ToothGroupType.MF,color);
				}
				else if(surfaces[i]=='O') {
					SetGroupColor(ToothGroupType.O,color);
				}
				else if(surfaces[i]=='D') {
					SetGroupColor(ToothGroupType.D,color);
					SetGroupColor(ToothGroupType.DF,color);
				}
				else if(surfaces[i]=='B') {
					SetGroupColor(ToothGroupType.B,color);
				}
				else if(surfaces[i]=='L') {
					SetGroupColor(ToothGroupType.L,color);
				}
				else if(surfaces[i]=='V') {
					SetGroupColor(ToothGroupType.V,color);
				}
				else if(surfaces[i]=='I') {
					SetGroupColor(ToothGroupType.I,color);
					SetGroupColor(ToothGroupType.IF,color);
				}
				else if(surfaces[i]=='F') {
					SetGroupColor(ToothGroupType.F,color);
				}
			}
		}

		///<summary>Used to set enamel, cementum, and BU colors externally.</summary>
		public void SetGroupColor(ToothGroupType groupType,Color paintColor){
			for(int i=0;i<Groups.Count;i++){
				if(Groups[i].GroupType!=groupType){
					continue;
				}
				Groups[i].PaintColor=paintColor;
			}
			//if type not found, then no action is taken
		}

		///<summary>Used in constructor and also in Reset.  Also sets visibility of all groups to true except RCT visible false.</summary>
		private void SetDefaultColors(){
			for(int i=0;i<Groups.Count;i++){
				if(Groups[i].GroupType==ToothGroupType.Cementum) {
					Groups[i].PaintColor=Color.FromArgb(255,250,245,223);//255,250,230);
				}
				else {//enamel
					Groups[i].PaintColor=Color.FromArgb(255,250,250,240);//255,255,245);
				}
				if(Groups[i].GroupType==ToothGroupType.Canals
					|| Groups[i].GroupType==ToothGroupType.Buildup)
				{
					Groups[i].Visible=false;
				}
				else{
					Groups[i].Visible=true;
				}
			}
		}

		///<summary>Used to set the root invisible.</summary>
		public void SetGroupVisibility(ToothGroupType groupType,bool setVisible) {
			for(int i=0;i<Groups.Count;i++) {
				if(Groups[i].GroupType!=groupType) {
					continue;
				}
				Groups[i].Visible=setVisible;
			}
		}

		///<summary></summary>
		public ToothGroup GetGroup(ToothGroupType groupType){
			for(int i=0;i<Groups.Count;i++) {
				if(Groups[i].GroupType==groupType) {
					return Groups[i];
				}
			}
			return null;
		}

		///<summary>This is used when calling display lists.</summary>
		public int GetIndexForDisplayList(ToothGroup group) {
			int toothInt=Tooth.ToOrdinal(toothID);
			//this can be enhanced later, but it's very simple for now.
			return (toothInt*15)+(int)group.GroupType;
		}

		#endregion Public Methods

		#region static functions
		//The reason these are all here instead of just using the Tooth class is that the graphical chart does not support supernumerary teeth,
		//so these methods handle that properly.

		///<summary>Strict validator for toothID. Will not handle any international tooth nums since all internal toothID's are in US format.  True if 1-32 or A-T.  Since supernumerary not currently handled, 51-82 and AS-TS return false.  Any invalid values also return false.</summary>
		public static bool IsValidToothID(string tooth_id) {
			if(!Tooth.IsValidDB(tooth_id)) {
				return false;
			}
			if(Tooth.IsSuperNum(tooth_id)){
				return false;
			}
			return true;
		}

		///<summary>The tooth_id should be validated before coming here, but it won't crash if invalid.  Primary or perm are ok.  Empty and null are also ok.  Result is always 1-32 or -1 if invalid tooth_id.  Supernumerary not allowed</summary>
		public static int IdToInt(string tooth_id) {
			if(!IsValidToothID(tooth_id)) {
				return -1;
			}
			return Tooth.ToInt(tooth_id);
		}

		///<summary>The actual specific width of the tooth in mm.  Used to lay out the teeth on the screen.  Value taken from Wheelers Dental Anatomy.  Used in the orthographic projection.  Also used when building the teeth in the first place.  The perspective projection will need to adjust these numbers because of the curvature of the arch.  Only the widths of permanent teeth are used for layout.  If a primary tooth is passed in, it will give an error.</summary>
		public static float GetWidth(string tooth_id) {
			switch(tooth_id) {
				case "1":
				case "16":
					return 8.5f;
				case "2":
				case "15":
					return 9f;
				case "3":
				case "14":
					return 10f;
				case "4":
				case "5":
				case "13":
				case "12":
					return 7f;
				case "6":
				case "11":
					return 7.5f;
				case "7":
				case "10":
					return 6.5f;
				case "8":
				case "9":
					return 8.5f;
				case "17":
				case "32":
					return 10f;
				case "18":
				case "31":
					return 10.5f;
				case "19":
				case "30":
					return 11f;
				case "20":
				case "21":
				case "29":
				case "28":
					return 7f;
				case "22":
				case "27":
					return 7f;
				case "23":
				case "26":
					return 5f;//5.5f;
				case "24":
				case "25":
					return 5f;
				default:
					throw new ApplicationException(tooth_id);
				//return 0;
			}
		}

		///<summary></summary>
		public static float GetWidth(int tooth_num) {
			return GetWidth(tooth_num.ToString());
		}

		///<summary>The x position of the center of the given tooth, with midline being 0.  Calculated once, then used to quickly calculate mouse positions and tooth positions.  All values are in mm.</summary>
		public static float GetDefaultOrthoXpos(int intTooth) {
			if(DefaultOrthoXpos==null) {
				DefaultOrthoXpos=new float[33];//0-32, 0 not used
				float spacing=0;//the distance between each adjacent tooth.
				DefaultOrthoXpos[8]=-spacing/2f-GetWidth(8)/2f;
				DefaultOrthoXpos[7]=DefaultOrthoXpos[8]-spacing-GetWidth(8)/2f-GetWidth(7)/2f;
				DefaultOrthoXpos[6]=DefaultOrthoXpos[7]-spacing-GetWidth(7)/2f-GetWidth(6)/2f;
				DefaultOrthoXpos[5]=DefaultOrthoXpos[6]-spacing-GetWidth(6)/2f-GetWidth(5)/2f;
				DefaultOrthoXpos[4]=DefaultOrthoXpos[5]-spacing-GetWidth(5)/2f-GetWidth(4)/2f;
				DefaultOrthoXpos[3]=DefaultOrthoXpos[4]-spacing-GetWidth(4)/2f-GetWidth(3)/2f;
				DefaultOrthoXpos[2]=DefaultOrthoXpos[3]-spacing-GetWidth(3)/2f-GetWidth(2)/2f;
				DefaultOrthoXpos[1]=DefaultOrthoXpos[2]-spacing-GetWidth(2)/2f-GetWidth(1)/2f;
				DefaultOrthoXpos[9]=-DefaultOrthoXpos[8];
				DefaultOrthoXpos[10]=-DefaultOrthoXpos[7];
				DefaultOrthoXpos[11]=-DefaultOrthoXpos[6];
				DefaultOrthoXpos[12]=-DefaultOrthoXpos[5];
				DefaultOrthoXpos[13]=-DefaultOrthoXpos[4];
				DefaultOrthoXpos[14]=-DefaultOrthoXpos[3];
				DefaultOrthoXpos[15]=-DefaultOrthoXpos[2];
				DefaultOrthoXpos[16]=-DefaultOrthoXpos[1];
				DefaultOrthoXpos[24]=spacing/2f+GetWidth(24)/2f;
				DefaultOrthoXpos[23]=DefaultOrthoXpos[24]+spacing+GetWidth(24)/2f+GetWidth(23)/2f;
				DefaultOrthoXpos[22]=DefaultOrthoXpos[23]+spacing+GetWidth(23)/2f+GetWidth(22)/2f;
				DefaultOrthoXpos[21]=DefaultOrthoXpos[22]+spacing+GetWidth(22)/2f+GetWidth(21)/2f;
				DefaultOrthoXpos[20]=DefaultOrthoXpos[21]+spacing+GetWidth(21)/2f+GetWidth(20)/2f;
				DefaultOrthoXpos[19]=DefaultOrthoXpos[20]+spacing+GetWidth(20)/2f+GetWidth(19)/2f;
				DefaultOrthoXpos[18]=DefaultOrthoXpos[19]+spacing+GetWidth(19)/2f+GetWidth(18)/2f;
				DefaultOrthoXpos[17]=DefaultOrthoXpos[18]+spacing+GetWidth(18)/2f+GetWidth(17)/2f;
				DefaultOrthoXpos[25]=-DefaultOrthoXpos[24];
				DefaultOrthoXpos[26]=-DefaultOrthoXpos[23];
				DefaultOrthoXpos[27]=-DefaultOrthoXpos[22];
				DefaultOrthoXpos[28]=-DefaultOrthoXpos[21];
				DefaultOrthoXpos[29]=-DefaultOrthoXpos[20];
				DefaultOrthoXpos[30]=-DefaultOrthoXpos[19];
				DefaultOrthoXpos[31]=-DefaultOrthoXpos[18];
				DefaultOrthoXpos[32]=-DefaultOrthoXpos[17];
			}
			if(intTooth<1 || intTooth>32) {
				throw new ApplicationException("Invalid tooth_num: "+intTooth.ToString());//just for debugging
			}
			return DefaultOrthoXpos[intTooth];
		}

		///<summary></summary>
		public static bool IsMaxillary(string tooth_id) {
			if(!IsValidToothID(tooth_id)) {
				return false;
			}
			return Tooth.IsMaxillary(tooth_id);
		}

		///<summary></summary>
		public static bool IsAnterior(string tooth_id) {
			if(!IsValidToothID(tooth_id)) {
				return false;
			}
			return Tooth.IsAnterior(tooth_id);
		}

		///<summary>True if on the right side of the mouth.</summary>
		public static bool IsRight(string tooth_id) {
			if(!IsValidToothID(tooth_id)) {
				return false;
			}
			int intTooth=IdToInt(tooth_id);
			if(intTooth>=1 && intTooth<=8) {
				return true;
			}
			if(intTooth>=25 && intTooth<=32) {
				return true;
			}
			return false;
		}

		#endregion static functions

		///<summary>Should only be run on startup for efficiency.</summary>
		private void ImportObj() {
			byte[] buffer=null;
			if(toothID=="1" || toothID=="16") {
				buffer=Properties.Resources.tooth1;
			}
			else if(toothID=="2" || toothID=="15") {
				buffer=Properties.Resources.tooth2;
			}
			else if(toothID=="3" || toothID=="14") {
				//file+="3.obj";
				buffer=Properties.Resources.tooth3;
			}
			else if(toothID=="4" || toothID=="13") {
				buffer=Properties.Resources.tooth4;
			}
			else if(toothID=="5" || toothID=="12") {
				buffer=Properties.Resources.tooth5;
			}
			else if(toothID=="6" || toothID=="11") {
				buffer=Properties.Resources.tooth6;
			}
			else if(toothID=="7" ||toothID=="10") {
				buffer=Properties.Resources.tooth7;
			}
			else if(toothID=="8" || toothID=="9") {
				buffer=Properties.Resources.tooth8;
			}
			else if(toothID=="17" || toothID=="32") {
				buffer=Properties.Resources.tooth32;
			}
			else if(toothID=="18" || toothID=="31") {
				buffer=Properties.Resources.tooth31;
			}
			else if(toothID=="19" || toothID=="30") {
				buffer=Properties.Resources.tooth30;
			}
			else if(toothID=="20" || toothID=="29") {
				buffer=Properties.Resources.tooth29;
			}
			else if(toothID=="21" || toothID=="28") {
				buffer=Properties.Resources.tooth28;
			}
			else if(toothID=="22" || toothID=="27") {
				buffer=Properties.Resources.tooth27;
			}
			else if(toothID=="23" || toothID=="24" || toothID=="25" ||toothID=="26") {
				buffer=Properties.Resources.tooth25;
			}
			else if(toothID=="A" || toothID=="J") {
				buffer=Properties.Resources.toothA;
			}
			else if(toothID=="B" || toothID=="I") {
				buffer=Properties.Resources.toothB;
			}
			else if(toothID=="C" || toothID=="H") {
				buffer=Properties.Resources.toothC;
			}
			else if(toothID=="D" || toothID=="G") {
				buffer=Properties.Resources.toothD;
			}
			else if(toothID=="E" || toothID=="F") {
				buffer=Properties.Resources.toothE;
			}
			else if(toothID=="P" || toothID=="O" || toothID=="Q" || toothID=="N") {
				buffer=Properties.Resources.toothP;
			}
			else if(toothID=="R" || toothID=="M") {
				buffer=Properties.Resources.toothR;
			}
			else if(toothID=="S" || toothID=="L") {
				buffer=Properties.Resources.toothS;
			}
			else if(toothID=="T" || toothID=="K") {
				buffer=Properties.Resources.toothT;
			}
			else if(toothID=="implant"){
				buffer=Properties.Resources.implant;
			}
			bool flipHorizontally=false;
			if(toothID!="implant" && IdToInt(toothID)>=9 && IdToInt(toothID)<=24) {
				flipHorizontally=true;
			}
			//There will not necessarily be the same number of vertices as normals.
			//But as they get paired up later, we will create a 1:1 relationship.
			List<Vertex3f> verts=new List<Vertex3f>();
			List<Vertex3f> norms=new List<Vertex3f>();
			Groups=new List<ToothGroup>();
			//ArrayList ALf=new ArrayList();//faces always part of a group
			List<Face> faces=new List<Face>();
			MemoryStream stream=new MemoryStream(buffer);
			using(StreamReader sr = new StreamReader(stream)){
				String line;
				Vertex3f vertex;
				string[] items;
				string[] subitems;
				Face face;
				ToothGroup group=null;
				while((line = sr.ReadLine()) != null) {
					if(line.StartsWith("#")//comment
						|| line.StartsWith("mtllib")//material library.  We build our own.
						|| line.StartsWith("usemtl")//use material
						|| line.StartsWith("o")) {//object. There's only one object 
						continue;
					}
					if(line.StartsWith("v ")) {//vertex
						items=line.Split(new char[] { ' ' });
						vertex=new Vertex3f();//float[3];
						if(flipHorizontally) {
							vertex.X=-Convert.ToSingle(items[1],CultureInfo.InvariantCulture);
						}
						else {
							vertex.X=Convert.ToSingle(items[1],CultureInfo.InvariantCulture);
						}
						vertex.Y=Convert.ToSingle(items[2],CultureInfo.InvariantCulture);
						vertex.Z=Convert.ToSingle(items[3],CultureInfo.InvariantCulture);
						verts.Add(vertex);
						continue;
					}
					if(line.StartsWith("vn")) {//vertex normal
						items=line.Split(new char[] { ' ' });
						vertex=new Vertex3f();//new float[3];
						if(flipHorizontally) {
							vertex.X=-Convert.ToSingle(items[1],CultureInfo.InvariantCulture);
						}
						else {
							vertex.X=Convert.ToSingle(items[1],CultureInfo.InvariantCulture);
						}
						vertex.Y=Convert.ToSingle(items[2],CultureInfo.InvariantCulture);
						vertex.Z=Convert.ToSingle(items[3],CultureInfo.InvariantCulture);
						norms.Add(vertex);
						continue;
					}
					if(line.StartsWith("g")) {//group
						if(group != null) {
							//move all faces into the existing group
							group.Faces=new List<Face>(faces);
							//move the existing group into the AL
							Groups.Add(group);
						}
						//start a new group to which all subsequent faces will be attached.
						group=new ToothGroup();
						faces=new List<Face>();
						//group.PaintColor=Color.FromArgb(255,255,253,209);//default to enamel
						switch(line) {
							default:
								group.GroupType=ToothGroupType.None;
								break;
							case "g cube1_Cementum":
								group.GroupType=ToothGroupType.Cementum;
								break;
							case "g cube1_Enamel2":
								group.GroupType=ToothGroupType.Enamel;
								break;
							case "g cube1_M":
								group.GroupType=ToothGroupType.M;
								break;
							case "g cube1_D":
								group.GroupType=ToothGroupType.D;
								break;
							case "g cube1_F":
								group.GroupType=ToothGroupType.F;
								break;
							case "g cube1_I":
								group.GroupType=ToothGroupType.I;
								break;
							case "g cube1_L":
								group.GroupType=ToothGroupType.L;
								break;
							case "g cube1_V":
								group.GroupType=ToothGroupType.V;
								break;
							case "g cube1_B":
								group.GroupType=ToothGroupType.B;
								break;
							case "g cube1_O":
								group.GroupType=ToothGroupType.O;
								break;
							case "g cube1_Canals":
								group.GroupType=ToothGroupType.Canals;
								break;
							case "g cube1_Buildup":
								group.GroupType=ToothGroupType.Buildup;
								break;
							case "g cube1_Implant":
								group.GroupType=ToothGroupType.Implant;
								break;
							case "g cube1_EnamelF":
								group.GroupType=ToothGroupType.EnamelF;
								break;
							case "g cube1_DF":
								group.GroupType=ToothGroupType.DF;
								break;
							case "g cube1_MF":
								group.GroupType=ToothGroupType.MF;
								break;
							case "g cube1_IF":
								group.GroupType=ToothGroupType.IF;
								break;
						}
					}
					if(line.StartsWith("f")) {//face. Usually 4 vertices, but not always.
						items=line.Split(new char[] { ' ' });
						face=new Face();
						VertexNormal vertnorm;
						int vertIdx;
						int normIdx;
						//do we need to load these backwards for flipping, so they'll still be counterclockwise?
						//It seems to work anyway, but it's something to keep in mind for later.
						for(int i=1;i<items.Length;i++){//face.GetLength(0);i++) {
							subitems=items[i].Split(new char[] { '/' });// eg: 9//9  this is an index to a given vertex/normal.
							vertnorm=new VertexNormal();//unlike the old way of just storing idxs, we will actually store vertices.
							vertIdx=Convert.ToInt32(subitems[0])-1;
							normIdx=Convert.ToInt32(subitems[2])-1;
							vertnorm.Vertex=verts[vertIdx];
							vertnorm.Normal=norms[normIdx];
							face.IndexList.Add(GetIndexForVertNorm(vertnorm));
						}
						faces.Add(face);
						continue;
					}
				}//while readline
				//For the very last group, move all faces into the group
				group.Faces=new List<Face>(faces);//new int[ALf.Count][][];
				//move the last group into the AL
				Groups.Add(group);
			}
			//MessageBox.Show(Vertices[2,2].ToString());
		}

		///<summary>Tries to find an existing VertexNormal in the list for this tooth.  If it can, then it returns that index.  If it can't then it adds this VertexNormal to the list and returns the last index.</summary>
		private int GetIndexForVertNorm(VertexNormal vertnorm) {
			//if(VertexNormals
			for(int i=0;i<VertexNormals.Count;i++) {
				if(VertexNormals[i].Vertex != vertnorm.Vertex) {
					continue;
				}
				if(VertexNormals[i].Normal != vertnorm.Normal) {
					continue;
				}
				return i;
			}
			//couldn't find
			VertexNormals.Add(vertnorm);
			return VertexNormals.Count-1;
		}

		///<summary>For any given tooth, there may only be one line in the returned list, or some teeth might have a few lines representing the root canals.</summary>
		public List<LineSimple> GetRctLines() {
			List<LineSimple> retVal=new List<LineSimple>();
			LineSimple line;
			#region Permanent Teeth
			#region Maxillary/Upper
			if(toothID=="1") {
				line=new LineSimple(
					-.7f,9.6f,1.6f, 
					.6f,8,1.6f ,
					1.2f,5.8f,1.6f ,
					.8f,0,.9f);
				retVal.Add(line);
				line=new LineSimple(
					-1.8f,9.5f,1.6f,
					-1.6f,8,1.6f,
					-1.6f,5.8f,1.6f,
					-.9f,0,.9f);
				retVal.Add(line);
			}
			if(toothID=="16") {
				line=new LineSimple(
					.7f,9.6f,1.6f, 
					-.6f,8,1.6f,
					-1.2f,5.8f,1.6f,
					-.8f,0,.9f);
				retVal.Add(line);
				line=new LineSimple(
					1.8f,9.5f,1.6f , 
					1.6f,8,1.6f ,
					1.6f,5.8f,1.6f ,
					.9f,0,.9f );
				retVal.Add(line);
			}
			if(toothID=="2") {
				line=new LineSimple(
					.3f,10.6f,3.4f , 
					1.4f,8,3.2f ,
					1.7f,5,1.9f ,
					.9f,0,1f);
				retVal.Add(line);
				line=new LineSimple(
					-1.8f,10.5f,3.4f , 
					-2,7,3.2f ,
					-1.7f,4,1.9f ,
					-1,0,1.1f );
				retVal.Add(line);
				line=new LineSimple(
					-2,11.5f,-3.7f , 
					-.6f,6.3f,-4 ,
					0,0,-2.3f);
				retVal.Add(line);
			}
			if(toothID=="15") {
				line=new LineSimple(
					-.3f,10.6f,3.4f , 
					-1.4f,8,3.2f ,
					-1.7f,5,1.9f ,
					-.9f,0,1f );
				retVal.Add(line);
				line=new LineSimple(
					1.8f,10.5f,3.4f , 
					2,7,3.2f ,
					1.7f,4,1.9f ,
					1,0,1.1f);
				retVal.Add(line);
				line=new LineSimple(
					2,11.5f,-3.7f , 
					.6f,6.3f,-4 ,
					0,0,-2.3f );
				retVal.Add(line);
			}
			if(toothID=="3") {
				line=new LineSimple(
					1.4f,11.5f,3.4f , 
					2.2f,9.4f,3.2f ,
					2.4f,6.7f,3.2f ,
					1.2f,0,1.1f);
				retVal.Add(line);
				line=new LineSimple(
					-2.7f,11.5f,3.4f , 
					-2.9f,9,3.2f ,
					-2.6f,5,3.2f ,
					-1.2f,0,1.1f );
				retVal.Add(line);
				line=new LineSimple(
					0,12.5f,-4.3f, 
					0,9.4f,-4.3f ,
					0,0,-2.2f );
				retVal.Add(line);
			}
			if(toothID=="14") {
				line=new LineSimple(
					-1.4f,11.5f,3.4f, 
					-2.2f,9.4f,3.2f ,
					-2.4f,6.7f,3.2f ,
					-1.2f,0,1.1f);
				retVal.Add(line);
				line=new LineSimple(
					2.7f,11.5f,3.4f , 
					2.9f,9,3.2f ,
					2.6f,5,3.2f ,
					1.2f,0,1.1f );
				retVal.Add(line);
				line=new LineSimple(
					0,12.5f,-4.3f , 
					0,9.4f,-4.3f ,
					0,0,-2.2f );
				retVal.Add(line);
			}
			if(toothID=="4") {
				line=new LineSimple(
					0,13.5f,1.2f , 
					-.2f,10,.6f ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="13") {
				line=new LineSimple(
					0,13.5f,1.2f , 
					.2f,10,.6f ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="5") {
				line=new LineSimple(
					-1.1f,13.5f,1.6f , 
					0,6,1.6f ,
					0,0,1 );
				retVal.Add(line);
			}
			if(toothID=="12") {
				line=new LineSimple(
					1.1f,13.5f,1.6f , 
					0,6,1.6f ,
					0,0,1);
				retVal.Add(line);
			}
			if(toothID=="6") {
				line=new LineSimple(
					-.4f,16.5f,0 , 
					0,11,0 ,
					0,0,0);
				retVal.Add(line);
			}
			if(toothID=="11") {
				line=new LineSimple(
					.4f,16.5f,0 , 
					0,11,0,
					0,0,0);
				retVal.Add(line);
			}
			if(toothID=="7") {
				line=new LineSimple(
					-.8f,12.5f,.6f , 
					-.3f,10,0 ,
					0,0,0);
				retVal.Add(line);
			}
			if(toothID=="10") {
				line=new LineSimple(
					.8f,12.5f,.6f , 
					.3f,10,0 ,
					0,0,0);
				retVal.Add(line);
			}
			if(toothID=="8"){
				line=new LineSimple(
					0,12.6f,0 , 
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="9") {
				line=new LineSimple( 
					0,12.6f,0 , 
					0,0,0 );
				retVal.Add(line);
			}
			#endregion Maxillary/Upper
			#region Mandibular/Lower
			if(toothID=="25" || toothID=="26") {
				line=new LineSimple(
					-.5f,-12,0 ,
					-.2f,-9,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="24" || toothID=="23") {
				line=new LineSimple(
					.5f,-12,0 , 
					.2f,-9,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="27") {
				line=new LineSimple(
					-.5f,-15.5f,0,
					-.1f,-13,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="22") {
				line=new LineSimple(
					.5f,-15.5f,0 ,
					.1f,-13,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="28") {
				line=new LineSimple(
					-.2f,-13.5f,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="21") {
				line=new LineSimple(
					.2f,-13.5f,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="29") {
				line=new LineSimple(
					-.3f,-14,0 ,
					0,-12,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="20") {
				line=new LineSimple(
					.3f,-14,0 ,
					0,-12,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="30") {
				line=new LineSimple(
					.9f,-13.5f,0 , 
					2.2f,-10,0 ,
					2.6f,-7,0 ,
					1.7f,0,0);
				retVal.Add(line);
				line=new LineSimple(
					-4.3f,-13.5f,0 , 
					-4f,-9,0 ,
					-3.3f,-5,0 ,
					-1.7f,0,0 );
				retVal.Add(line);
			}
			if(toothID=="19") {
				line=new LineSimple(
					-.9f,-13.5f,0 , 
					-2.2f,-10,0 ,
					-2.6f,-7,0 ,
					-1.7f,0,0 );
				retVal.Add(line);
				line=new LineSimple(
					4.3f,-13.5f,0 , 
					4f,-9,0 ,
					3.3f,-5,0 ,
					1.7f,0,0  );
				retVal.Add(line);
			}
			if(toothID=="31") {
				line=new LineSimple(
					0,-12.5f,0 , 
					1.8f,-7.5f,0 ,
					2.2f,-4,0 ,
					1.7f,0,0);
				retVal.Add(line);
				line=new LineSimple(
					-3.4f,-12.5f,0 , 
					-3.4f,-8,0 ,
					-3f,-5,0 ,
					-1.7f,0,0);
				retVal.Add(line);
			}
			if(toothID=="18") {
				line=new LineSimple(
					0,-12.5f,0 , 
					-1.8f,-7.5f,0 ,
					-2.2f,-4,0 ,
					-1.7f,0,0 );
				retVal.Add(line);
				line=new LineSimple(
					3.4f,-12.5f,0 , 
					3.4f,-8,0 ,
					3f,-5,0 ,
					1.7f,0,0 );
				retVal.Add(line);
			}
			if(toothID=="32") {
				line=new LineSimple(
					-.7f,-10.5f,0 , 
					.8f,-7.5f,0 ,
					1.7f,-4,0 ,
					1.6f,0,0 );
				retVal.Add(line);
				line=new LineSimple(
					-3.2f,-10.5f,0 , 
					-3.4f,-8,0 ,
					-3f,-5,0 ,
					-1.7f,0,0 );
				retVal.Add(line);
			}
			if(toothID=="17") {
				line=new LineSimple(
					.7f,-10.5f,0 , 
					-.8f,-7.5f,0 ,
					-1.7f,-4,0 ,
					-1.6f,0,0);
				retVal.Add(line);
				line=new LineSimple(
					3.2f,-10.5f,0 , 
					3.4f,-8,0 ,
					3f,-5,0 ,
					1.7f,0,0);
				retVal.Add(line);
			}
			#endregion Mandibular/Lower
			#endregion Permanent Teeth
			#region Primary Teeth
			#region Maxillary/Upper
			if(toothID=="A") {
				line=new LineSimple(
					-3.7f,10.4f,3.4f, 
					-3.6f,7f,3.2f ,
					-3.4f,6f,3.2f ,
					-1.5f,0,1.1f);
				retVal.Add(line);
				line=new LineSimple(
					3.1f,11.1f,3.4f , 
					3.3f,9,3.2f ,
					3f,5,3.2f ,
					1.4f,0,1.1f );
				retVal.Add(line);
			}
			if(toothID=="J") {
				line=new LineSimple(
					3.7f,10.4f,3.4f, 
					3.6f,7f,3.2f ,
					3.4f,6f,3.2f ,
					1.5f,0,1.1f);
				retVal.Add(line);
				line=new LineSimple(
					-3.1f,11.1f,3.4f , 
					-3.3f,9,3.2f ,
					-3f,5,3.2f ,
					-1.4f,0,1.1f );
				retVal.Add(line);
			}
			if(toothID=="B") {
				line=new LineSimple(
					-3.7f,9f,3.4f, 
					-3.6f,7f,3.2f ,
					-3.4f,6f,3.2f ,
					-1.5f,0,1.1f);
				retVal.Add(line);
				line=new LineSimple(
					3f,9.6f,3.4f , 
					3.2f,8.8f,3.2f ,
					2.9f,5f,3.2f ,
					1.5f,0,1.1f );
				retVal.Add(line);
			}
			if(toothID=="I") {
				line=new LineSimple(
					3.7f,9f,3.4f, 
					3.6f,7f,3.2f ,
					3.4f,6f,3.2f ,
					1.5f,0,1.1f);
				retVal.Add(line);
				line=new LineSimple(
					-3f,9.6f,3.4f , 
					-3.2f,8.8f,3.2f ,
					-2.9f,5f,3.2f ,
					-1.5f,0,1.1f );
				retVal.Add(line);
			}
			if(toothID=="C") {
				line=new LineSimple(
					0,5.5f,0, 
					0,3,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="H") {
				line=new LineSimple(
					0,5.5f,0, 
					0,3,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="D") {
				line=new LineSimple(
					0,6.3f,0, 
					0,3,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="G") {
				line=new LineSimple(
					0,6.3f,0, 
					0,3,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="E") {
				line=new LineSimple(
					0,5,0 , 
					0,3,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="F") {
				line=new LineSimple(
					0,5,0 , 
					0,3,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			#endregion Maxillary/Upper
			#region Mandibular/Lower
			if(toothID=="P" || toothID=="Q") {
				line=new LineSimple(
					0,-6.5f,0 , 
					0,-3,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="O" || toothID=="N") {
				line=new LineSimple(
					0,-6.5f,0 , 
					0,-3,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="R") {
				line=new LineSimple(
					0,-4.8f,0 , 
					0,-4.5f,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="M") {
				line=new LineSimple(
					0,-4.8f,0 , 
					0,-4.5f,0 ,
					0,0,0 );
				retVal.Add(line);
			}
			if(toothID=="S") {
				line=new LineSimple(
					-3.7f,-8.8f,3.4f, 
					-3.6f,-7f,3.2f ,
					-3.4f,-6f,3.2f ,
					-1.5f,0,1.1f);
				retVal.Add(line);
				line=new LineSimple(
					3.2f,-9.4f,3.4f , 
					3.3f,-7.5f,3.2f ,
					3f,-5f,3.2f ,
					1.4f,0,1.1f );
				retVal.Add(line);
			}
			if(toothID=="L") {
				line=new LineSimple(
					3.7f,-8.8f,3.4f, 
					3.6f,-7f,3.2f ,
					3.4f,-6f,3.2f ,
					1.5f,0,1.1f);
				retVal.Add(line);
				line=new LineSimple(
					-3.2f,-9.4f,3.4f , 
					-3.3f,-7.5f,3.2f ,
					-3f,-5f,3.2f ,
					-1.4f,0,1.1f );
				retVal.Add(line);
			}
			if(toothID=="T") {
				line=new LineSimple(
					4.5f,-10.8f,3.4f, 
					4f,-7f,3.2f ,
					3.5f,-6f,3.2f ,
					1.5f,0,1.1f);
				retVal.Add(line);
				line=new LineSimple(
					-5.1f,-10.9f,3.4f , 
					-4.5f,-9,3.2f ,
					-3.5f,-5,3.2f ,
					-1.4f,0,1.1f );
				retVal.Add(line);
			}
			if(toothID=="K") {
				line=new LineSimple(
					-4.5f,-10.8f,3.4f, 
					-4f,-7f,3.2f ,
					-3.5f,-6f,3.2f ,
					-1.5f,0,1.1f);
				retVal.Add(line);
				line=new LineSimple(
					5.1f,-10.9f,3.4f , 
					4.5f,-9,3.2f ,
					3.5f,-5,3.2f ,
					1.4f,0,1.1f );
				retVal.Add(line);
			}
			#endregion Mandibular/Lower
			#endregion Primary Teeth
			return retVal;
		}

		///<summary></summary>
		public LineSimple GetSealantLine() {
			if(IsMaxillary(toothID)){
				return new LineSimple(
					1.5f,-4f,1.5f,
					.75f,-4f,2.25f,
					-.75f,-4f,2.25f,
					-1.5f,-4f,1.5f,
					-1.5f,-4f,.75f,
					1.5f,-4f,-.75f,
					1.5f,-4f,-1.5f,
					.75f,-4f,-2.25f,
					-.75f,-4f,-2.25f,
					-1.5f,-4f,-1.5f);
			}
			else{
				return new LineSimple(
					-1.5f,4f,1.5f,
					-.75f,4f,2.25f,
					.75f,4f,2.25f,
					1.5f,4f,1.5f,
					1.5f,4f,.75f,
					-1.5f,4f,-.75f,
					-1.5f,4f,-1.5f,
					-.75f,4f,-2.25f,
					.75f,4f,-2.25f,
					1.5f,4f,-1.5f);
			}
		}

		public LineSimple GetWatchLine(){
			return new LineSimple(
				0f,0f,0f,
				.8f,-2.65f,0f,
				1.6f,0f,0f,
				2.4f,-2.65f,0f,
				3.2f,0f,0f);
		}

		/*
		///<summary></summary>
		public Triangle GetBUpoly() {
			if(toothID=="1") {
				return new Triangle(
					-1.5f,0,0 , 
					-1.5f,2.3f,0 ,
					0,1.5f,0,
					1.4f,2.3f,0 ,
					1.4f,0,0);
			}
			if(toothID=="16") {
				return new Triangle(
					1.5f,0,0 , 
					1.5f,2.3f,0 ,
					0,1.5f,0,
					-1.4f,2.3f,0 ,
					-1.4f,0,0 );
			}
			if(toothID=="2") {
				return new Triangle(
					-1.8f,0,0 , 
					-1.8f,2.3f,0 ,
					0,1.5f,0,
					1.6f,2.3f,0 ,
					1.6f,0,0 );
			}
			if(toothID=="15") {
				return new Triangle(
					1.8f,0,0 , 
					1.8f,2.3f,0 ,
					0,1.5f,0,
					-1.6f,2.3f,0 ,
					-1.6f,0,0);
			}
			if(toothID=="3") {
				return new Triangle(
					-2.3f,0,0 , 
					-2.3f,2.6f,0 ,
					0,1.7f,0,
					2.1f,2.6f,0 ,
					2.1f,0,0 );
			}
			if(toothID=="14") {
				return new Triangle( 
					2.3f,0,0 , 
					2.3f,2.6f,0 ,
					0,1.7f,0,
					-2.1f,2.6f,0 ,
					-2.1f,0,0 );
			}
			if(toothID=="4"
				|| toothID=="5"
				|| toothID=="6"
				|| toothID=="7"
				|| toothID=="8"
				|| toothID=="9"
				|| toothID=="10"
				|| toothID=="11"
				|| toothID=="12"
				|| toothID=="13"
				) {
				return new Triangle(
					-.8f,0,0 , 
					-.8f,3.5f,0 ,
					.8f,3.5f,0 ,
					.8f,0,0 );
			}
			if(toothID=="23"
				|| toothID=="24"
				|| toothID=="25"
				|| toothID=="26"
				) {
				return new Triangle(
					-.7f,0,0 , 
					-.7f,-3.5f,0 ,
					.7f,-3.5f,0 ,
					.7f,0,0);
			}
			if(toothID=="20"
				|| toothID=="21"
				|| toothID=="22"
				|| toothID=="27"
				|| toothID=="28"
				|| toothID=="29"
				) {
				return new Triangle(
					-.8f,0,0 , 
					-.8f,-3.5f,0 ,
					.8f,-3.5f,0 ,
					.8f,0,0 );
			}
			if(toothID=="30") {
				return new Triangle(
					-2.8f,0,0 , 
					-2.8f,-2.4f,0 ,
					0,-1.5f,0,
					2.3f,-2.4f,0 ,
					2.3f,0,0 );
			}
			if(toothID=="19") {
				return new Triangle(
					2.8f,0,0 , 
					2.8f,-2.4f,0 ,
					0,-1.5f,0,
					-2.3f,-2.4f,0 ,
					-2.3f,0,0 );
			}
			if(toothID=="31") {
				return new Triangle( 
					-2.6f,0,0 , 
					-2.6f,-2.1f,0 ,
					0,-1.5f,0,
					2.3f,-2.1f,0 ,
					2.3f,0.5f,0 );
			}
			if(toothID=="18") {
				return new Triangle(
					2.6f,0,0 , 
					2.6f,-2.1f,0 ,
					0,-1.5f,0,
					-2.3f,-2.1f,0 ,
					-2.3f,0.5f,0 );
			}
			if(toothID=="32") {
				return new Triangle(
					-2.6f,0,0 , 
					-2.6f,-2.1f,0 ,
					0,-1.5f,0,
					2.1f,-2.1f,0 ,
					2.1f,0,0 );
			}
			if(toothID=="17") {
				return new Triangle(
					2.6f,0,0 , 
					2.6f,-2.1f,0 ,
					0,-1.5f,0,
					-2.1f,-2.1f,0 ,
					-2.1f,0,0 );
			}
			return new Triangle();
		}
		*/
	}
}
