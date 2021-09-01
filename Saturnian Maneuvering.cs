/*
* Saturnian Maneuvering OS
* Built by mlq1616
* https://github.com/mlq1819/Space-Engineers-Saturnian-Suite
* This suite handles thruster and gyroscope controls. 
* Includes Autoland, basic Autopiloting
* Include "Thruster" in LCD name to add to group.
*/
string Program_Name="Saturnian Maneuvering";
Color DEFAULT_TEXT_COLOR=new Color(197,137,255,255);
Color DEFAULT_BACKGROUND_COLOR=new Color(44,0,88,255);
double Speed_Limit=100;
double Acceptable_Angle=5;
bool Control_Gyroscopes=true;
bool Control_Thrusters=true;

class Prog{
	public static MyGridProgram P;
	public static TimeSpan FromSeconds(double seconds){
		return (new TimeSpan(0,0,0,(int)seconds,(int)(seconds*1000)%1000));
	}

	public static TimeSpan UpdateTimeSpan(TimeSpan old,double seconds){
		return old+FromSeconds(seconds);
	}
}

class GenericMethods<T> where T : class, IMyTerminalBlock{
	static IMyGridTerminalSystem TerminalSystem{
		get{
			return P.GridTerminalSystem;
		}
	}
	public static MyGridProgram P{
		get{
			return Prog.P;
		}
	}
	
	public static T GetFull(string name,Vector3D Ref,double mx_d=double.MaxValue){
		List<T> AllBlocks=new List<T>();
		List<T> MyBlocks=new List<T>();
		TerminalSystem.GetBlocksOfType<T>(AllBlocks);
		double min_distance=mx_d;
		foreach(T Block in AllBlocks){
			if(Block.CustomName.Equals(name)){
				double distance=(Ref-Block.GetPosition()).Length();
				min_distance=Math.Min(min_distance, distance);
				MyBlocks.Add(Block);
			}
		}
		foreach(T Block in MyBlocks){
			double distance=(Ref-Block.GetPosition()).Length();
			if(distance<=min_distance+0.1)
				return Block;
		}
		return null;
	}
	
	public static T GetFull(string name,IMyTerminalBlock Ref,double mx_d=double.MaxValue){
		return GetFull(name,Ref.GetPosition(),mx_d);
	}
	
	public static T GetFull(string name,double mx_d=double.MaxValue){
		return GetFull(name,P.Me,mx_d);
	}
	
	public static T GetConstruct(string name,IMyTerminalBlock Ref,double mx_d=double.MaxValue){
		List<T> input=GetAllConstruct(name,Ref,mx_d);
		if(input.Count>0)
			return input[0];
		return null;
	}
	
	public static T GetConstruct(string name,double mx_d=double.MaxValue){
		return GetConstruct(name,P.Me,mx_d);
	}
	
	public static List<T> GetAllConstruct(string name,IMyTerminalBlock Ref,double mx_d=double.MaxValue){
		List<T> input=GetAllContaining(name,Ref,mx_d);
		List<T> output=new List<T>();
		foreach(T Block in input){
			if(Ref.IsSameConstructAs(Block))
				output.Add(Block);
		}
		return output;
	}
	
	public static List<T> GetAllConstruct(string name){
		return GetAllConstruct(name,P.Me);
	}
	
	public static T GetContaining(string name,Vector3D Ref,double mx_d){
		List<T> AllBlocks=new List<T>();
		List<T> MyBlocks=new List<T>();
		TerminalSystem.GetBlocksOfType<T>(AllBlocks);
		double min_distance=mx_d;
		foreach(T Block in AllBlocks){
			if(Block.CustomName.Contains(name)){
				double distance=(Ref-Block.GetPosition()).Length();
				min_distance=Math.Min(min_distance,distance);
				MyBlocks.Add(Block);
			}
		}
		foreach(T Block in MyBlocks){
			double distance=(Ref-Block.GetPosition()).Length();
			if(distance<=min_distance+0.1)
				return Block;
		}
		return null;
	}
	
	public static T GetContaining(string name,IMyTerminalBlock Ref,double mx_d){
		return GetContaining(name,Ref.GetPosition(),mx_d);
	}
	
	public static T GetContaining(string name,double mx_d=double.MaxValue){
		return GetContaining(name,P.Me,mx_d);
	}
	
	public static List<T> GetAllContaining(string name,Vector3D Ref,double mx_d){
		List<T> AllBlocks=new List<T>();
		List<List<T>> MyLists=new List<List<T>>();
		List<T> MyBlocks=new List<T>();
		TerminalSystem.GetBlocksOfType<T>(AllBlocks);
		foreach(T Block in AllBlocks){
			if(Block.CustomName.Contains(name)){
				bool has_with_name=false;
				for(int i=0;i<MyLists.Count&&!has_with_name;i++){
					if(Block.CustomName.Equals(MyLists[i][0].CustomName)){
						MyLists[i].Add(Block);
						has_with_name=true;
						break;
					}
				}
				if(!has_with_name){
					List<T> new_list=new List<T>();
					new_list.Add(Block);
					MyLists.Add(new_list);
				}
			}
		}
		foreach(List<T> list in MyLists){
			if(list.Count==1){
				MyBlocks.Add(list[0]);
				continue;
			}
			double min_distance=mx_d;
			foreach(T Block in list){
				double distance=(Ref-Block.GetPosition()).Length();
				min_distance=Math.Min(min_distance, distance);
			}
			foreach(T Block in list){
				double distance=(Ref-Block.GetPosition()).Length();
				if(distance<=min_distance+0.1){
					MyBlocks.Add(Block);
					break;
				}
			}
		}
		return MyBlocks;
	}
	
	public static List<T> GetAllIncluding(string name,Vector3D Ref,double mx_d=double.MaxValue){
		List<T> AllBlocks=new List<T>();
		List<T> MyBlocks=new List<T>();
		TerminalSystem.GetBlocksOfType<T>(AllBlocks);
		foreach(T Block in AllBlocks){
			double distance=(Ref-Block.GetPosition()).Length();
			if(Block.CustomName.Contains(name)&&distance<=mx_d)
				MyBlocks.Add(Block);
		}
		return MyBlocks;
	}
	
	public static List<T> GetAllIncluding(string name,IMyTerminalBlock Ref,double mx_d=double.MaxValue){
		return GetAllIncluding(name,Ref.GetPosition(),mx_d);
	}
	
	public static List<T> GetAllIncluding(string name,double mx_d=double.MaxValue){
		return GetAllIncluding(name,P.Me,mx_d);
	}
	
	public static List<T> GetAllContaining(string name,IMyTerminalBlock Ref,double mx_d=double.MaxValue){
		return GetAllContaining(name,Ref.GetPosition(),mx_d);
	}
	
	public static List<T> GetAllContaining(string name,double mx_d=double.MaxValue){
		return GetAllContaining(name,P.Me,mx_d);
	}
	
	public static List<T> GetAllFunc(Func<T,bool> f){
		List<T> AllBlocks=new List<T>();
		List<T> MyBlocks=new List<T>();
		TerminalSystem.GetBlocksOfType<T>(AllBlocks);
		foreach(T Block in AllBlocks){
			if(f(Block))
				MyBlocks.Add(Block);
		}
		return MyBlocks;
	}
	
	public static T GetClosestFunc(Func<T,bool> f,Vector3D Ref,double mx_d=double.MaxValue){
		List<T> MyBlocks=GetAllFunc(f);
		double min_distance=mx_d;
		foreach(T Block in MyBlocks){
			double distance=(Ref-Block.GetPosition()).Length();
			min_distance=Math.Min(min_distance,distance);
		}
		foreach(T Block in MyBlocks){
			double distance=(Ref-Block.GetPosition()).Length();
			if(distance<=min_distance+0.1)
				return Block;
		}
		return null;
	}
	
	public static T GetClosestFunc(Func<T,bool> f,IMyTerminalBlock Ref,double mx_d=double.MaxValue){
		return GetClosestFunc(f,Ref.GetPosition(),mx_d);
	}
	
	public static T GetClosestFunc(Func<T,bool> f,double mx_d=double.MaxValue){
		return GetClosestFunc(f,P.Me,mx_d);
	}
	
	public static T GetGrid(string name,IMyCubeGrid Grid,IMyTerminalBlock Ref,double mx_d=double.MaxValue){
		List<T> input=GetAllGrid(name,Grid,Ref,mx_d);
		if(input.Count>0)
			return input[0];
		return null;
	}
	
	public static T GetGrid(string name,IMyCubeGrid Grid,double mx_d=double.MaxValue){
		return GetGrid(name,Grid,P.Me,mx_d);
	}
	
	public static List<T> GetAllGrid(string name,IMyCubeGrid Grid,IMyTerminalBlock Ref,double mx_d){
		List<T> output=new List<T>();
		List<T> input=GetAllContaining(name,Ref,mx_d);
		foreach(T Block in input){
			if(Block.CubeGrid==Grid)
				output.Add(Block);
		}
		return output;
	}
	
	public static List<T> GetAllGrid(string name,IMyCubeGrid Grid,double mx_d=double.MaxValue){
		return GetAllGrid(name,Grid,P.Me,mx_d);
	}
	
	public static List<T> SortByDistance(List<T> unsorted,Vector3D Ref){
		List<T> output=new List<T>();
		while(unsorted.Count>0){
			double min_distance=double.MaxValue;
			foreach(T Block in unsorted){
				double distance=(Ref-Block.GetPosition()).Length();
				min_distance=Math.Min(min_distance,distance);
			}
			for(int i=0; i<unsorted.Count; i++){
				double distance=(Ref-unsorted[i].GetPosition()).Length();
				if(distance<=min_distance+0.1){
					output.Add(unsorted[i]);
					unsorted.RemoveAt(i);
					break;
				}
			}
		}
		return output;
	}
	
	public static List<T> SortByDistance(List<T> unsorted,IMyTerminalBlock Ref){
		return SortByDistance(unsorted, Ref.GetPosition());
	}
	
	public static List<T> SortByDistance(List<T> unsorted){
		return SortByDistance(unsorted,P.Me);
	}
	
	private static double GetAngle(Vector3D v1,Vector3D v2,int i){
		v1.Normalize();
		v2.Normalize();
		double output=Math.Round(Math.Acos(v1.X*v2.X+v1.Y*v2.Y+v1.Z*v2.Z)*180/Math.PI,5);
		if(i>0&&output.ToString().Equals("NaN")){
			Random Rnd=new Random();
			Vector3D v3=new Vector3D(Rnd.Next(0,10)-5,Rnd.Next(0,10)-5,Rnd.Next(0,10)-5);
			v3.Normalize();
			if(Rnd.Next(0,1)==1)
				output=GetAngle(v1+v3/360,v2,i-1);
			else
				output=GetAngle(v1,v2+v3/360,i-1);
		}
		return output;
	}
	
	public static double GetAngle(Vector3D v1, Vector3D v2){
		return GetAngle(v1,v2,10);
	}
}

TimeSpan FromSeconds(double seconds){
	return Prog.FromSeconds(seconds);
}

TimeSpan UpdateTimeSpan(TimeSpan old,double seconds){
	return old+FromSeconds(seconds);
}

string ToString(TimeSpan ts){
	if(ts.TotalDays>=1)
		return Math.Round(ts.TotalDays,2).ToString()+" days";
	else if(ts.TotalHours>=1)
		return Math.Round(ts.TotalHours,2).ToString()+" hours";
	else if(ts.TotalMinutes>=1)
		return Math.Round(ts.TotalMinutes,2).ToString()+" minutes";
	else if(ts.TotalSeconds>=1)
		return Math.Round(ts.TotalSeconds,3).ToString()+" seconds";
	else 
		return Math.Round(ts.TotalMilliseconds,0).ToString()+" milliseconds";
}

bool HasBlockData(IMyTerminalBlock Block, string Name){
	if(Name.Contains(':'))
		return false;
	string[] args=Block.CustomData.Split('•');
	foreach(string argument in args){
		if(argument.IndexOf(Name+':')==0){
			return true;
		}
	}
	return false;
}
string GetBlockData(IMyTerminalBlock Block, string Name){
	if(Name.Contains(':'))
		return "";
	string[] args=Block.CustomData.Split('•');
	foreach(string argument in args){
		if(argument.IndexOf(Name+':')==0){
			return argument.Substring((Name+':').Length);
		}
	}
	return "";
}
bool SetBlockData(IMyTerminalBlock Block, string Name, string Data){
	if(Name.Contains(':'))
		return false;
	string[] args=Block.CustomData.Split('•');
	for(int i=0; i<args.Count(); i++){
		if(args[i].IndexOf(Name+':')==0){
			Block.CustomData=Name+':'+Data;
			for(int j=0; j<args.Count(); j++){
				if(j!=i){
					Block.CustomData+='•'+args[j];
				}
			}
			return true;
		}
	}
	Block.CustomData+='•'+Name+':'+Data;
	return true;
}
bool CanHaveJob(IMyTerminalBlock Block, string JobName){
	return (!HasBlockData(Block,"Job"))||GetBlockData(Block,"Job").Equals("None")||GetBlockData(Block, "Job").Equals(JobName);
}

Vector3D GlobalToLocal(Vector3D Global,IMyCubeBlock Ref){
	Vector3D Local=Vector3D.Transform(Global+Ref.GetPosition(), MatrixD.Invert(Ref.WorldMatrix));
	Local.Normalize();
	return Local*Global.Length();
}
Vector3D GlobalToLocalPosition(Vector3D Global,IMyCubeBlock Ref){
	Vector3D Local=Vector3D.Transform(Global, MatrixD.Invert(Ref.WorldMatrix));
	Local.Normalize();
	return Local*(Global-Ref.GetPosition()).Length();
}
Vector3D LocalToGlobal(Vector3D Local,IMyCubeBlock Ref){
	Vector3D Global=Vector3D.Transform(Local, Ref.WorldMatrix)-Ref.GetPosition();
	Global.Normalize();
	return Global*Local.Length();
}
Vector3D LocalToGlobalPosition(Vector3D Local,IMyCubeBlock Ref){
	return Vector3D.Transform(Local,Ref.WorldMatrix);
}

double GetAngle(Vector3D v1,Vector3D v2){
	return GenericMethods<IMyTerminalBlock>.GetAngle(v1,v2);
}

void Write(string text,bool new_line=true,bool append=true){
	Echo(text);
	IMyTextSurface Surface=Me.GetSurface(0);
	if(new_line){
		Vector2 SurfaceSize=Surface.SurfaceSize;
		string[] Full_Lines=text.Split('\n');
		if(!append)
			Surface.WriteText("",false);
		foreach(string Full_Line in Full_Lines){
			Vector2 StringSize=Surface.MeasureStringInPixels(new StringBuilder(Full_Line),Surface.Font,Surface.FontSize);
			int min_lines=(int)Math.Ceiling(((float)SurfaceSize.X)/StringSize.X);
			string[] words=Full_Line.Split(' ');
			string current_line="";
			for(int i=0;i<words.Length;i++){
				string next_line=current_line;
				if(current_line.Length>0)
					next_line+=' ';
				next_line+=words[i];
				if(current_line.Length>0&&Surface.MeasureStringInPixels(new StringBuilder(next_line),Surface.Font,Surface.FontSize).X>SurfaceSize.X){
					Surface.WriteText(current_line+'\n',true);
					current_line="";
				}
				if(current_line.Length>0)
					current_line+=' ';
				current_line+=words[i];
			}
			if(current_line.Length>0)
				Surface.WriteText(current_line+'\n',true);
		}
	}
	else
		Surface.WriteText(text,append);
}

int Display_Count=5;
int _Current_Display=1;
int Current_Display{
	get{
		return _Current_Display;
	}
	set{
		if(!Running_Thrusters){
			if(value==3||value==4)
				value=5;
		}
		if(value!=_Current_Display){
			_Current_Display=value;
			UpdateMyDisplay();
		}
	}
}
double Display_Timer=5;
void Display(int display_number,string text,bool new_line=true,bool append=true){
	if(display_number==Current_Display)
		Write(text,new_line,append);
	else
		Echo(text);
}
void UpdateMyDisplay(){
	IMyTextSurface Display=Me.GetSurface(0);
	switch(Current_Display){
		case 5:
			Display.FontColor=DEFAULT_TEXT_COLOR;
			Display.BackgroundColor=DEFAULT_BACKGROUND_COLOR;
			Display.Alignment=TextAlignment.LEFT;
			Display.ContentType=ContentType.TEXT_AND_IMAGE;
			Display.Font="Monospace";
			Display.TextPadding=0;
			Display.FontSize=1.25f;
			break;
		default:
			Display.FontColor=DEFAULT_TEXT_COLOR;
			Display.BackgroundColor=DEFAULT_BACKGROUND_COLOR;
			Display.Alignment=TextAlignment.CENTER;
			Display.ContentType=ContentType.TEXT_AND_IMAGE;
			Display.Font="Debug";
			Display.TextPadding=2;
			Display.FontSize=1;
			break;
	}
}

string GetRemovedString(string big_string, string small_string){
	string output=big_string;
	if(big_string.Contains(small_string)){
		output=big_string.Substring(0, big_string.IndexOf(small_string))+big_string.Substring(big_string.IndexOf(small_string)+small_string.Length);
	}
	return output;
}

struct CustomPanel{
	public IMyTextSurface Display;
	public bool Trans;
	public CustomPanel(IMyTextSurface d,bool t=false){
		Display=d;
		Trans=t;
	}
	public CustomPanel(IMyTextPanel p){
		Display=p as IMyTextSurface;
		Trans=p.CustomName.ToLower().Contains("transparent");
	}
}

TimeSpan Time_Since_Start=new TimeSpan(0);
long cycle=0;
char loading_char='|';
double seconds_since_last_update=0;
Random Rnd;

IMyShipController Controller;
List<IMyShipController> Controllers;
IMyGyro Gyroscope;
List<IMyLandingGear> LandingGear;

List<CustomPanel> ThrusterLCDs;

List<IMyThrust>[] All_Thrusters=new List<IMyThrust>[6];
List<IMyThrust> Forward_Thrusters{
	set{
		All_Thrusters[0]=value;
	}
	get{
		return All_Thrusters[0];
	}
}
List<IMyThrust> Backward_Thrusters{
	set{
		All_Thrusters[1]=value;
	}
	get{
		return All_Thrusters[1];
	}
}
List<IMyThrust> Up_Thrusters{
	set{
		All_Thrusters[2]=value;
	}
	get{
		return All_Thrusters[2];
	}
}
List<IMyThrust> Down_Thrusters{
	set{
		All_Thrusters[3]=value;
	}
	get{
		return All_Thrusters[3];
	}
}
List<IMyThrust> Left_Thrusters{
	set{
		All_Thrusters[4]=value;
	}
	get{
		return All_Thrusters[4];
	}
}
List<IMyThrust> Right_Thrusters{
	set{
		All_Thrusters[5]=value;
	}
	get{
		return All_Thrusters[5];
	}
}

bool Thrust_Check=false;
float _Max_Thrust;
float Max_Thrust{
	get{
		if(!Thrust_Check){
			_Max_Thrust=Forward_Thrust;
			_Max_Thrust=Math.Max(_Max_Thrust,Backward_Thrust);
			_Max_Thrust=Math.Max(_Max_Thrust,Up_Thrust);
			_Max_Thrust=Math.Max(_Max_Thrust,Down_Thrust);
			_Max_Thrust=Math.Max(_Max_Thrust,Left_Thrust);
			_Max_Thrust=Math.Max(_Max_Thrust,Right_Thrust);
			Thrust_Check=true;
		}
		return _Max_Thrust;
	}
}

float Forward_Thrust{
	get{
		float total=0;
		foreach(IMyThrust Thruster in Forward_Thrusters){
			if(Thruster.IsWorking)
				total+=Thruster.MaxEffectiveThrust;
		}
		return Math.Max(total,1);
	}
}
float Backward_Thrust{
	get{
		float total=0;
		foreach(IMyThrust Thruster in Backward_Thrusters){
			if(Thruster.IsWorking)
				total+=Thruster.MaxEffectiveThrust;
		}
		return Math.Max(total,1);
	}
}
float Up_Thrust{
	get{
		float total=0;
		foreach(IMyThrust Thruster in Up_Thrusters){
			if(Thruster.IsWorking)
				total+=Thruster.MaxEffectiveThrust;
		}
		return Math.Max(total,1);
	}
}
float Down_Thrust{
	get{
		float total=0;
		foreach(IMyThrust Thruster in Down_Thrusters){
			if(Thruster.IsWorking)
				total+=Thruster.MaxEffectiveThrust;
		}
		return Math.Max(total,1);
	}
}
float Left_Thrust{
	get{
		float total=0;
		foreach(IMyThrust Thruster in Left_Thrusters){
			if(Thruster.IsWorking)
				total+=Thruster.MaxEffectiveThrust;
		}
		return Math.Max(total,1);
	}
}
float Right_Thrust{
	get{
		float total=0;
		foreach(IMyThrust Thruster in Right_Thrusters){
			if(Thruster.IsWorking)
				total+=Thruster.MaxEffectiveThrust;
		}
		return Math.Max(total,1);
	}
}

double Forward_Acc{
	get{
		return Forward_Thrust/Controller.CalculateShipMass().TotalMass;
	}
}
double Backward_Acc{
	get{
		return Backward_Thrust/Controller.CalculateShipMass().TotalMass;
	}
}
double Up_Acc{
	get{
		return Up_Thrust/Controller.CalculateShipMass().TotalMass;
	}
}
double Down_Acc{
	get{
		return Down_Thrust/Controller.CalculateShipMass().TotalMass;
	}
}
double Left_Acc{
	get{
		return Left_Thrust/Controller.CalculateShipMass().TotalMass;
	}
}
double Right_Acc{
	get{
		return Right_Thrust/Controller.CalculateShipMass().TotalMass;
	}
}

double Forward_Gs{
	get{
		return Forward_Acc/9.81;
	}
}
double Backward_Gs{
	get{
		return Backward_Acc/9.81;
	}
}
double Up_Gs{
	get{
		return Up_Acc/9.81;
	}
}
double Down_Gs{
	get{
		return Down_Acc/9.81;
	}
}
double Left_Gs{
	get{
		return Left_Acc/9.81;
	}
}
double Right_Gs{
	get{
		return Right_Acc/9.81;
	}
}

Base6Directions.Direction Forward;
Base6Directions.Direction Backward{
	get{
		return Base6Directions.GetOppositeDirection(Forward);
	}
}
Base6Directions.Direction Up;
Base6Directions.Direction Down{
	get{
		return Base6Directions.GetOppositeDirection(Up);
	}
}
Base6Directions.Direction Left;
Base6Directions.Direction Right{
	get{
		return Base6Directions.GetOppositeDirection(Left);
	}
}

Vector3D Forward_Vector;
Vector3D Backward_Vector{
	get{
		return -1*Forward_Vector;
	}
}
Vector3D Up_Vector;
Vector3D Down_Vector{
	get{
		return -1*Up_Vector;
	}
}
Vector3D Left_Vector;
Vector3D Right_Vector{
	get{
		return -1*Left_Vector;
	}
}

float Mass_Accomodation=0.0f;
double Time_To_Crash=double.MaxValue;
double Time_To_Stop=0;

double RestingSpeed=0;
Vector3D RestingVelocity{
	get{
		if(RestingSpeed==0)
			return new Vector3D(0,0,0);
		return RestingSpeed*Forward_Vector;
	}
}
Vector3D Relative_RestingVelocity{
	get{
		return GlobalToLocal(RestingVelocity,Controller);
	}
}
double CurrentSpeed{
	get{
		return CurrentVelocity.Length();
	}
}
Vector3D CurrentVelocity;
Vector3D Velocity_Direction{
	get{
		Vector3D VD=CurrentVelocity;
		VD.Normalize();
		return VD;
	}
}
Vector3D Relative_CurrentVelocity{
	get{
		Vector3D output=Vector3D.Transform(CurrentVelocity+Controller.GetPosition(),MatrixD.Invert(Controller.WorldMatrix));
		output.Normalize();
		output*=CurrentVelocity.Length();
		return output;
	}
}
Vector3D Gravity;
Vector3D Relative_Gravity{
	get{
		return GlobalToLocal(Gravity,Controller);
	}
}
Vector3D Adjusted_Gravity{
	get{
		Vector3D temp=GlobalToLocal(Gravity,Controller);
		temp.Normalize();
		return temp*Mass_Accomodation;
	}
}
Vector3D Gravity_Direction{
	get{
		Vector3D direction=Gravity;
		direction.Normalize();
		return direction;
	}
}
double Speed_Deviation{
	get{
		return (CurrentVelocity-RestingVelocity).Length();
	}
}
Vector3D AngularVelocity;
Vector3D Relative_AngularVelocity{
	get{
		return GlobalToLocal(AngularVelocity,Controller);
	}
}

double Elevation;
double Sealevel;
Vector3D PlanetCenter;

bool MainControllerFunction(IMyShipController ctr){
	return ctr.IsMainCockpit&&ControllerFunction(ctr);
}
bool ControllerFunction(IMyShipController ctr){
	return ctr.IsSameConstructAs(Me)&&ctr.CanControlShip&&ctr.ControlThrusters;
}

UpdateFrequency GetUpdateFrequency(){
	if(Running_Thrusters||(Gyroscope!=null&&Gyroscope.GyroOverride))
		return UpdateFrequency.Update1;
	return UpdateFrequency.Update100;
}

string GetThrustTypeName(IMyThrust Thruster){
	string block_type=Thruster.BlockDefinition.SubtypeName;
	if(block_type.Contains("LargeBlock"))
		block_type=GetRemovedString(block_type,"LargeBlock");
	else if(block_type.Contains("SmallBlock"))
		block_type=GetRemovedString(block_type,"SmallBlock");
	if(block_type.Contains("Thrust"))
		block_type=GetRemovedString(block_type,"Thrust");
	string size="";
	if(block_type.Contains("Small")){
		size="Small";
		block_type=GetRemovedString(block_type,size);
	}
	else if(block_type.Contains("Large")){
		size="Large";
		block_type=GetRemovedString(block_type,size);
	}
	if((!block_type.ToLower().Contains("atmospheric"))&&(!block_type.ToLower().Contains("hydrogen")))
		block_type+="Ion";
	return (size+" "+block_type).Trim();
}
struct NameTuple{
	public string Name;
	public int Count;
	
	public NameTuple(string n,int c=0){
		Name=n;
		Count=c;
	}
}
void SetThrusterList(List<IMyThrust> Thrusters,string Direction){
	List<NameTuple> Thruster_Types=new List<NameTuple>();
	foreach(IMyThrust Thruster in Thrusters){
		if(!HasBlockData(Thruster,"DefaultOverride"))
			SetBlockData(Thruster,"DefaultOverride",Thruster.ThrustOverridePercentage.ToString());
		SetBlockData(Thruster,"Owner",Me.CubeGrid.EntityId.ToString());
		SetBlockData(Thruster,"DefaultName",Thruster.CustomName);
		string name=GetThrustTypeName(Thruster);
		bool found=false;
		for(int i=0;i<Thruster_Types.Count;i++){
			if(name.Equals(Thruster_Types[i].Name)){
				found=true;
				Thruster_Types[i]=new NameTuple(name,Thruster_Types[i].Count+1);
				break;
			}
		}
		if(!found)
			Thruster_Types.Add(new NameTuple(name,1));
	}
	foreach(IMyThrust Thruster in Thrusters){
		string name=GetThrustTypeName(Thruster);
		for(int i=0;i<Thruster_Types.Count;i++){
			if(name.Equals(Thruster_Types[i].Name)){
				Thruster.CustomName=(Direction+" "+name+" Thruster "+(Thruster_Types[i].Count).ToString()).Trim();
				Thruster_Types[i]=new NameTuple(name,Thruster_Types[i].Count-1);
				break;
			}
		}
	}
}
void ResetThruster(IMyThrust Thruster){
	if(HasBlockData(Thruster,"DefaultOverride")){
		float ThrustOverride=0.0f;
		if(float.TryParse(GetBlockData(Thruster,"DefaultOverride"),out ThrustOverride))
			Thruster.ThrustOverridePercentage=ThrustOverride;
		else
			Thruster.ThrustOverridePercentage=0.0f;
	}
	if(HasBlockData(Thruster,"DefaultName")){
		Thruster.CustomName=GetBlockData(Thruster,"DefaultName");
	}
	SetBlockData(Thruster,"Owner","0");
}
void ResetThrusters(){
	for(int i=0;i<All_Thrusters.Length;i++){
		foreach(IMyThrust Thruster in All_Thrusters[i])
			ResetThruster(Thruster);
	}
	Running_Thrusters=false;
}

void Reset(){
	Operational=false;
	Runtime.UpdateFrequency=UpdateFrequency.None;
	Controller=null;
	Controllers=new List<IMyShipController>();
	if(Gyroscope!=null)
		Gyroscope.GyroOverride=false;
	Gyroscope=null;
	LandingGear=new List<IMyLandingGear>();
	for(int i=0;i<All_Thrusters.Length;i++){
		if(All_Thrusters[i]!=null){
			for(int j=0;j<All_Thrusters[i].Count;j++){
				if(All_Thrusters[i][j]!=null)
					ResetThruster(All_Thrusters[i][j]);
			}
		}
		All_Thrusters[i]=new List<IMyThrust>();
	}
	ThrusterLCDs=new List<CustomPanel>();
	RestingSpeed=0;
	Notifications=new List<Notification>();
}

double MySize=0;
bool Setup(){
	Reset();
	List<IMyTextPanel> LCDs=GenericMethods<IMyTextPanel>.GetAllConstruct("Thruster");
	foreach(IMyTextPanel Panel in LCDs)
		ThrusterLCDs.Add(new CustomPanel(Panel));
	foreach(CustomPanel Panel in ThrusterLCDs){
		if(Panel.Trans){
			Panel.Display.FontColor=DEFAULT_BACKGROUND_COLOR;
			Panel.Display.BackgroundColor=new Color(0,0,0,0);
		}
		else{
			Panel.Display.FontColor=DEFAULT_TEXT_COLOR;
			Panel.Display.BackgroundColor=DEFAULT_BACKGROUND_COLOR;
		}
		Panel.Display.Font="Monospace";
		Panel.Display.Alignment=TextAlignment.LEFT;
		Panel.Display.ContentType=ContentType.TEXT_AND_IMAGE;
		Panel.Display.TextPadding=0;
		Panel.Display.FontSize=1.25f;
	}
	Controller=GenericMethods<IMyShipController>.GetClosestFunc(MainControllerFunction);
	if(Controller==null)
		Controller=GenericMethods<IMyShipController>.GetClosestFunc(ControllerFunction);
	Controllers=GenericMethods<IMyShipController>.GetAllFunc(ControllerFunction);
	if(Controller==null){
		Write("Failed to find Controller", false, false);
		return false;
	}
	bool has_main_ctrl=false;
	foreach(IMyShipController Ctrl in Controllers){
		if(Ctrl.CustomName.Equals(Controller.CustomName)){
			has_main_ctrl=true;
			break;
		}
	}
	if(!has_main_ctrl)
		Controllers.Add(Controller);
	Forward=Controller.Orientation.Forward;
	Up=Controller.Orientation.Up;
	Left=Controller.Orientation.Left;
	MySize=Controller.CubeGrid.GridSize;
	Gyroscope=GenericMethods<IMyGyro>.GetConstruct("Control Gyroscope");
	if(Gyroscope==null){
		Gyroscope=GenericMethods<IMyGyro>.GetConstruct("");
		if(Gyroscope==null&&!Me.CubeGrid.IsStatic)
			return false;
	}
	if(Gyroscope!=null){
		Gyroscope.CustomName="Control Gyroscope";
		Gyroscope.GyroOverride=Controller.IsUnderControl;
	}
	LandingGear=GenericMethods<IMyLandingGear>.GetAllConstruct("");
	GridTerminalSystem.GetBlocksOfType<IMyLandingGear>(LandingGear);
	List<IMyThrust> MyThrusters=GenericMethods<IMyThrust>.GetAllConstruct("");
	foreach(IMyThrust Thruster in MyThrusters){
		if(Thruster.CubeGrid!=Controller.CubeGrid)
			continue;
		Base6Directions.Direction ThrustDirection=Thruster.Orientation.Forward;
		if(ThrustDirection==Backward)
			Forward_Thrusters.Add(Thruster);
		else if(ThrustDirection==Forward)
			Backward_Thrusters.Add(Thruster);
		else if(ThrustDirection==Down)
			Up_Thrusters.Add(Thruster);
		else if(ThrustDirection==Up)
			Down_Thrusters.Add(Thruster);
		else if(ThrustDirection==Right)
			Left_Thrusters.Add(Thruster);
		else if(ThrustDirection==Left)
			Right_Thrusters.Add(Thruster);
	}
	SetThrusterList(Forward_Thrusters,"Forward");
	SetThrusterList(Backward_Thrusters,"Backward");
	SetThrusterList(Up_Thrusters,"Up");
	SetThrusterList(Down_Thrusters,"Down");
	SetThrusterList(Left_Thrusters,"Left");
	SetThrusterList(Right_Thrusters,"Right");
	
	Acceptable_Angle=Math.Min(Math.Max(0.5,200/MySize),10);
	Operational=Me.IsWorking;
	Runtime.UpdateFrequency=GetUpdateFrequency();
	return true;
}

bool Operational=false;
public Program(){
	Prog.P=this;
	Me.CustomName=(Program_Name+" Programmable block").Trim();
	for(int i=0;i<Me.SurfaceCount;i++){
		Me.GetSurface(i).FontColor=DEFAULT_TEXT_COLOR;
		Me.GetSurface(i).BackgroundColor=DEFAULT_BACKGROUND_COLOR;
		Me.GetSurface(i).Alignment=TextAlignment.CENTER;
		Me.GetSurface(i).ContentType=ContentType.TEXT_AND_IMAGE;
	}
	Me.GetSurface(1).FontSize=2.2f;
	Me.GetSurface(1).TextPadding=30.0f;
	Echo("Beginning initialization");
	Rnd=new Random();
	string[] args=this.Storage.Split('•');
	foreach(string arg in args){
		if(!arg.Contains(':'))
			continue;
		int index=arg.IndexOf(':');
		string name=arg.Substring(0,index);
		string data=arg.Substring(index+1);
		switch(name){
			case "RestingSpeed":
				double.TryParse(data,out RestingSpeed);
				break;
		}
	}
	Notifications=new List<Notification>();
	Task_Queue=new Queue<Task>();
	TaskParser(Me.CustomData);
	Setup();
}

public void Save(){
	this.Storage="RestingSpeed:"+Math.Round(RestingSpeed,1).ToString();
	if(Gyroscope!=null)
		Gyroscope.GyroOverride=false;
	for(int i=0;i<All_Thrusters.Length;i++){
		foreach(IMyThrust Thruster in All_Thrusters[i])
			ResetThruster(Thruster);
	}
	Me.CustomData="";
	foreach(Task T in Task_Queue){
		Me.CustomData+=T.ToString()+'•';
	}
}

bool _Autoland=false;
bool Autoland(){
	if((!_Autoland)&&!Control_Thrusters)
		return false;
	if(!Safety)
		return false;
	_Autoland=!_Autoland;
	return true;
}
bool Disable(){
	Operational=false;
	ResetThrusters();
	if(Gyroscope!=null)
		Gyroscope.GyroOverride=false;
	Runtime.UpdateFrequency=UpdateFrequency.None;
	Me.Enabled=false;
	return true;
}
bool FactoryReset(){
	if(Gyroscope!=null)
		Gyroscope.GyroOverride=false;
	for(int i=0;i<All_Thrusters.Length;i++){
		foreach(IMyThrust Thruster in All_Thrusters[i])
			ResetThruster(Thruster);
	}
	Me.CustomData="";
	this.Storage="";
	Reset();
	Me.CustomData="";
	this.Storage="";
	Me.Enabled=false;
	return true;
}

//Sets gyroscope outputs from player input, dampeners, gravity, and autopilot
double Pitch_Time= 1.0f;
double Yaw_Time=1.0f;
double Roll_Time=1.0f;
bool Do_Direction=false;
Vector3D Target_Direction=new Vector3D(0,0,0);
bool Match_Direction=false;
bool Do_Up=false;
Vector3D Target_Up=new Vector3D(0,0,0);
void SetGyroscopes(){
	Gyroscope.GyroOverride=(AngularVelocity.Length()<3);
	float current_pitch=(float)Relative_AngularVelocity.X;
	float current_yaw=(float)Relative_AngularVelocity.Y;
	float current_roll=(float)Relative_AngularVelocity.Z;
	
	float gyro_count=0;
	List<IMyGyro> AllGyros=new List<IMyGyro>();
	GridTerminalSystem.GetBlocksOfType<IMyGyro>(AllGyros);
	foreach(IMyGyro Gyro in AllGyros){
		if(Gyro.IsWorking)
			gyro_count+=Gyro.GyroPower/100.0f;
	}
	float gyro_multx=(float)Math.Max(0.1f, Math.Min(1, 1.5f/(Controller.CalculateShipMass().PhysicalMass/gyro_count/1000000)));
	
	if(Match_Direction&&Do_Position&&Target_Distance>20){
		bool do_Match=true;
		Vector3D target_direction=Target_Position-Controller.GetPosition();
		target_direction.Normalize();
		if(Gravity.Length()>0){
			double Grav_Angle=GetAngle(target_direction,Gravity_Direction);
			if(Grav_Angle<60)
				do_Match=false;
			if(Grav_Angle>120&&Forward_Acc<Gravity.Length())
				do_Match=false;
		}
		if(do_Match){
			Do_Direction=true;
			Target_Direction=target_direction;
		}
	}
	
	float input_pitch=0;
	float input_yaw=0;
	float input_roll=0;
	
	if(Pitch_Time<1)
		Pitch_Time+=seconds_since_last_update;
	if(Yaw_Time<1)
		Yaw_Time+=seconds_since_last_update;
	if(Roll_Time<1)
		Roll_Time+=seconds_since_last_update;
	
	bool Undercontrol=false;
	foreach(IMyShipController Ctrl in Controllers)
		Undercontrol=Undercontrol||Ctrl.IsUnderControl;
	
	foreach(IMyShipController Ctrl in Controllers)
		input_pitch+=Math.Min(Math.Max(Ctrl.RotationIndicator.X/100,-1),1);
	
	double Direction_Angle=GetAngle(Forward_Vector,Target_Direction);
	
	if(Math.Abs(input_pitch)<0.05f){
		input_pitch=current_pitch*0.99f;
		bool do_adjust_pitch=Do_Direction;
		double v_difference=0;
		if(Do_Direction){
			v_difference=(GetAngle(Up_Vector,Target_Direction)-GetAngle(Down_Vector,Target_Direction))/2;
			if(Gravity.Length()>0){
				double target_grav=Math.Abs(90-GetAngle(Target_Direction,Gravity));
				if(target_grav<45){
					double grav_difference=(GetAngle(Up_Vector,Gravity)-GetAngle(Down_Vector,Gravity))/2;
					if(grav_difference<30&&Math.Abs(v_difference-Direction_Angle)>15)
						do_adjust_pitch=false;
					else if(Direction_Angle>90&&v_difference>90)
						do_adjust_pitch=false;
				}
			}
		}
		if(do_adjust_pitch){
			if(Math.Abs(v_difference)>Math.Min(1,Acceptable_Angle/2))
				input_pitch+=10*gyro_multx*((float)Math.Min(Math.Max(v_difference,-90),90)/90.0f);
		}
		else{
			float orbit_multx=1;
			if(Safety){
				if((((Elevation-MySize)<Controller.GetShipSpeed()*2&&(Elevation-MySize)<50)||Controller.DampenersOverride&&!Controller.IsUnderControl)&&GetAngle(Gravity,Forward_Vector)<120&&Pitch_Time>=1){
					double difference=Math.Abs(GetAngle(Gravity,Forward_Vector));
					if(difference<90)
						input_pitch-=10*gyro_multx*((float)Math.Min(Math.Abs((90-difference)/90),1));
				}
				if((Controller.DampenersOverride&&!Undercontrol)&&(GetAngle(Gravity,Forward_Vector)>(90+Acceptable_Angle/2))){
					double difference=Math.Abs(GetAngle(Gravity,Forward_Vector));
					if(difference>90+Acceptable_Angle/2)
						input_pitch+=10*gyro_multx*((float)Math.Min(Math.Abs((difference-90)/90),1))*orbit_multx;
				}
			}
		}
	}
	else{
		Pitch_Time=0;
		input_pitch*=30;
	}
	foreach(IMyShipController Ctrl in Controllers)
		input_yaw+=Math.Min(Math.Max(Ctrl.RotationIndicator.Y/100,-1),1);
	if(Math.Abs(input_yaw)<0.05f){
		input_yaw=current_yaw*0.99f;
		if(Do_Direction){
			double difference=(GetAngle(Left_Vector,Target_Direction)-GetAngle(Right_Vector,Target_Direction))/2;
			if(Direction_Angle>90){
				if(difference<0)
					difference=-180-difference;
				else if(difference>0)
					difference=180-difference;
				else
					difference=180;
			}
			if(Math.Abs(difference)>Math.Min(1,Acceptable_Angle/2))
				input_yaw+=10*gyro_multx*((float)Math.Min(Math.Max(difference,-90),90)/90.0f);
		}
	}
	else{
		Yaw_Time=0;
		input_yaw*=30;
	}
	foreach(IMyShipController Ctrl in Controllers)
		input_roll+=Ctrl.RollIndicator;
	if(Math.Abs(input_roll)<0.05f){
		input_roll=current_roll*0.99f;
		if(Do_Up){
			if((!Do_Direction)||GetAngle(Forward_Vector,Target_Direction)<90){
				double difference=(GetAngle(Left_Vector,Target_Up)-GetAngle(Right_Vector,Target_Up))/2;
				if(GetAngle(Up_Vector,Target_Up)>90){
					if(difference<0)
						difference=-180-difference;
					else if(difference>0)
						difference=180-difference;
					else
						difference=180;
				}
				float direction_multx=1.0f;
				if(Do_Direction&&Direction_Angle>Acceptable_Angle)
					direction_multx/=(float)(Direction_Angle/Acceptable_Angle);
				if(Math.Abs(difference)>Math.Min(1,Acceptable_Angle/2))
					input_roll+=30*gyro_multx*direction_multx*((float)Math.Min(Math.Max(difference,-90),90)/90.0f);
			}
		}
		else{ 
			if(Safety&&Gravity.Length()>0&&Roll_Time>=1){
				double difference=GetAngle(Left_Vector,Gravity)-GetAngle(Right_Vector,Gravity);
				if(Math.Abs(difference)>Acceptable_Angle){
					input_roll-=(float)Math.Min(Math.Max(difference*5,-5),25)*gyro_multx*5;
				}
			}
		}
	}
	else{
		Roll_Time=0;
		input_roll*=10;
	}
	
	Vector3D input=new Vector3D(input_pitch,input_yaw,input_roll);
	Vector3D global=Vector3D.TransformNormal(input,Controller.WorldMatrix);
	Vector3D output=Vector3D.TransformNormal(global,MatrixD.Invert(Gyroscope.WorldMatrix));
	output.Normalize();
	output*=input.Length();
	
	Gyroscope.Pitch=(float)output.X;
	Gyroscope.Yaw=(float)output.Y;
	Gyroscope.Roll=(float)output.Z;
}


double Distance_Speed_Limit(double distance){
	distance=Math.Abs(distance);
	if(distance<0.5)
		return 4*distance;
	if(distance<1.5)
		return 2;
	if(distance<2.5)
		return 2.5;
	if(distance<5)
		return distance;
	if(distance<25)
		return 10;
	if(distance<50)
		return 20;
	if(distance<100)
		return 25;
	if(distance<250)
		return 40;
	if(distance<500)
		return 50;
	return distance/10;
}
float Match_Thrust(double esl,double Relative_Speed,double Relative_Target_Speed,double Relative_Distance,float T1,float T2,Vector3D V1,Vector3D V2,float Relative_Gravity){
	double R_ESL=Math.Min(Elevation,Math.Min(esl,Distance_Speed_Limit(Relative_Distance)));
	float distance_multx=1.0f;
	double Target_Speed=0;
	double speed_change=Relative_Speed-Relative_Target_Speed;
	double deacceleration=Math.Abs(speed_change*Controller.CalculateShipMass().PhysicalMass);
	double time=0;
	//difference is required change in velocity; must be "0" when the target is reached
	if(speed_change>0)
		time=deacceleration/(T1-Relative_Gravity);
	else if(speed_change<0)
		time=deacceleration/(T2+Relative_Gravity);
	//deacceleration is the required change in force; divided by the thruster power, this is now the ammount of time required to make that change
	double acceleration_distance=(Math.Abs(Relative_Speed)+Target_Speed)*time/2;
	//acceleration_distance is the distance that will be covered during that change in speed
	bool increase=acceleration_distance<Math.Abs(Relative_Distance)*1.05+MySize+5;
	//increase determines whether to accelerate or deaccelerate, based on whether the acceleration distance is smaller than the distance to the target (with some wiggle room);
	if(!increase){
		distance_multx*=-1;
	}
	if(Relative_Distance<Relative_Target_Speed){
		if((CurrentVelocity+V1-RestingVelocity).Length()<=R_ESL)
			return -0.95f*T1*distance_multx;
	}
	else if(Relative_Distance>Relative_Target_Speed){
		if((CurrentVelocity+V2-RestingVelocity).Length()<=R_ESL)
			return 0.95f*T2*distance_multx;
	}
	return 0;
}

bool Safety=true;
bool Do_Position=false;
Vector3D Target_Position=new Vector3D(0,0,0);
Vector3D Relative_Target_Position{
	get{
		return GlobalToLocalPosition(Target_Position,Controller);
	}
}
double Target_Distance{
	get{
		return (Target_Position-Controller.GetPosition()).Length();
	}
}
double True_Target_Distance{
	get{
		return (True_Target_Position-Controller.GetPosition()).Length();
	}
}
bool Running_Thrusters=false;
void SetThrusters(){
	Running_Thrusters=true;
	float input_forward=0.0f;
	float input_up=0.0f;
	float input_right=0.0f;
	float damp_multx=0.99f;
	double effective_speed_limit=Speed_Limit;
	
	bool Undercontrol=false;
	foreach(IMyShipController Ctrl in Controllers)
		Undercontrol=Undercontrol||Ctrl.IsUnderControl;
	
	double Ev_Df=Math.Max(0,Math.Min(20,MySize/4))+10;
	if(Safety){
		if(Elevation<200+Ev_Df)
			effective_speed_limit=Math.Min(effective_speed_limit,Math.Sqrt(Math.Max(Elevation-Ev_Df,0)/200)*Speed_Limit);
		if(Time_To_Crash<30&&Time_To_Crash>=0)
			effective_speed_limit=Math.Min(effective_speed_limit,Math.Sqrt(Time_To_Crash/30)*Speed_Limit);
		if(Do_Position)
			effective_speed_limit=Math.Min(effective_speed_limit,Math.Sqrt(True_Target_Distance/4)*4);
	}
	if(Controller.DampenersOverride){
		Display(3,"Cruise Control: Off");
		Display(3,"Dampeners: On");
		input_right-=(float)((Relative_CurrentVelocity.X-Relative_RestingVelocity.X)*Mass_Accomodation*damp_multx);
		input_up-=(float)((Relative_CurrentVelocity.Y-Relative_RestingVelocity.Y)*Mass_Accomodation*damp_multx);
		input_forward+=(float)((Relative_CurrentVelocity.Z-Relative_RestingVelocity.Z)*Mass_Accomodation*damp_multx);
	}
	else{
		if(Elevation>50||CurrentVelocity.Length()>10){
			Display(3,"Cruise Control: On");
			Vector3D velocity_direction=CurrentVelocity;
			velocity_direction.Normalize();
			double angle=Math.Min(GetAngle(Forward_Vector, velocity_direction),GetAngle(Backward_Vector, velocity_direction));
			if(angle<=Acceptable_Angle/2){
				input_right-=(float)((Relative_CurrentVelocity.X-Relative_RestingVelocity.X)*Mass_Accomodation*damp_multx);
				input_up-=(float)((Relative_CurrentVelocity.Y-Relative_RestingVelocity.Y)*Mass_Accomodation*damp_multx);
				Display(3,"Stabilizers: On ("+Math.Round(angle, 1)+"° dev)");
			}
			else
				Display(3,"Stabilizers: Off ("+Math.Round(angle, 1)+"° dev)");
		}
		else{
			Display(3,"Cruise Control: Off");
			Display(3,"Dampeners: Off");
		}
	}
	
	double ExpectedForwardMovement=CurrentSpeed/60;
	double UpwardMovementLimit=ExpectedForwardMovement*2;
	if((cycle>10&&Runtime.UpdateFrequency==UpdateFrequency.Update1)?LastElevation-Elevation>UpwardMovementLimit:false)
		effective_speed_limit/=2;
	
	effective_speed_limit=Math.Max(effective_speed_limit,5);
	if(!Safety)
		effective_speed_limit=300000000;
	
	Display(3,"Effective Speed Limit:"+Math.Round(effective_speed_limit,1)+"mps");
	
	
	if(RestingSpeed==0&&Controller.DampenersOverride&&(Speed_Deviation+5)<effective_speed_limit&&!Do_Position){
		for(int i=0;i<All_Thrusters.Length;i++){
			foreach(IMyThrust Thruster in All_Thrusters[i])
				Thruster.ThrustOverride=0;
		}
		Running_Thrusters=false;
		return;
	}
	
	if(Gravity.Length()>0&&Mass_Accomodation>0&&(Controller.GetShipSpeed()<100||GetAngle(CurrentVelocity,Gravity)>Acceptable_Angle)){
		if(!(_Autoland&&Time_To_Crash>15&&CurrentSpeed>5)){
			if(!((!Controller.DampenersOverride)&&Elevation<Ev_Df&&CurrentSpeed<1)){
				input_right-=(float)Adjusted_Gravity.X;
				input_up-=(float)Adjusted_Gravity.Y;
				input_forward+=(float)Adjusted_Gravity.Z;
			}
		}
	}
	
	if(Do_Position){
		if(Target_Distance>1500)
			Write("Target Position: "+Math.Round(True_Target_Distance/1000,1)+"kM");
		else
			Write("Target Position: "+Math.Round(True_Target_Distance,0)+"M");
		float thrust_value=Match_Thrust(effective_speed_limit,Relative_CurrentVelocity.X,RestingVelocity.X,Relative_Target_Position.X,Left_Thrust,Right_Thrust,Left_Vector,Right_Vector,-1*(float)Adjusted_Gravity.X);
		if(Math.Abs(thrust_value)>=1)
			input_right=thrust_value-(float)Adjusted_Gravity.X;
		thrust_value=Match_Thrust(effective_speed_limit,Relative_CurrentVelocity.Y,RestingVelocity.Y,Relative_Target_Position.Y,Down_Thrust,Up_Thrust,Down_Vector,Up_Vector,-1*(float)Adjusted_Gravity.Y);
		if(Math.Abs(thrust_value)>=1)
			input_up=thrust_value-(float)Adjusted_Gravity.Y;
		thrust_value=-1*Match_Thrust(effective_speed_limit,Relative_CurrentVelocity.Z,RestingVelocity.Z,Relative_Target_Position.Z,Forward_Thrust,Backward_Thrust,Forward_Vector,Backward_Vector,(float)Adjusted_Gravity.Z);
		if(Math.Abs(thrust_value)>=1)
			input_forward=thrust_value+(float)Adjusted_Gravity.Z;
	}
	else{
		foreach(IMyShipController Ctrl in Controllers){
			if(Ctrl.IsUnderControl&&Math.Abs(Ctrl.MoveIndicator.X)>0.5f){
				if(Ctrl.MoveIndicator.X>0){
					if((!Safety)||(CurrentVelocity+Right_Vector-RestingVelocity).Length()<=effective_speed_limit)
						input_right=0.95f*Right_Thrust;
					else
						input_right=Math.Min(input_right,0);
				} else {
					if((!Safety)||(CurrentVelocity+Left_Vector-RestingVelocity).Length()<=effective_speed_limit)
						input_right=-0.95f*Left_Thrust;
					else
						input_right=Math.Max(input_right,0);
				}
			}
		}
		
		foreach(IMyShipController Ctrl in Controllers){
			if(Ctrl.IsUnderControl&&Math.Abs(Ctrl.MoveIndicator.Y)>0.5f){
				if(Ctrl.MoveIndicator.Y>0){
					bool grav=GetAngle(Up_Vector,Gravity_Direction)>150;
					if((!Safety)||(CurrentVelocity+Up_Vector-RestingVelocity).Length()<=effective_speed_limit||(grav&&(Elevation<100+Ev_Df)))
						input_up=0.95f*Up_Thrust;
					else
						input_up=Math.Min(input_up,0);
				} else {
					if((!Safety)||(CurrentVelocity+Down_Vector-RestingVelocity).Length()<=effective_speed_limit)
						input_up=-0.95f*Down_Thrust;
					else
						input_up=Math.Max(input_up,0);
				}
			}
		}
		
		foreach(IMyShipController Ctrl in Controllers){
			if(Ctrl.IsUnderControl&&Math.Abs(Ctrl.MoveIndicator.Z)>0.5f){
				if(Ctrl.MoveIndicator.Z<0){
					if((!Safety)||(CurrentVelocity+Up_Vector-RestingVelocity).Length()<=effective_speed_limit)
						input_forward=0.95f*Forward_Thrust;
					else
						input_forward=Math.Min(input_forward,0);
				} 
				else{
					if((!Safety)||(CurrentVelocity+Down_Vector-RestingVelocity).Length()<=effective_speed_limit)
						input_forward=-0.95f*Backward_Thrust;
					else
						input_forward=Math.Max(input_forward,0);
				}
			}
		}
	}
	
	float output_forward=0.0f;
	float output_backward=0.0f;
	if(input_forward/Forward_Thrust>0.01f)
		output_forward=Math.Min(Math.Abs(input_forward/Forward_Thrust),1);
	else if(input_forward/Backward_Thrust<-0.01f)
		output_backward=Math.Min(Math.Abs(input_forward/Backward_Thrust),1);
	float output_up=0.0f;
	float output_down=0.0f;
	if(input_up/Up_Thrust>0.01f)
		output_up=Math.Min(Math.Abs(input_up/Up_Thrust), 1);
	else if(input_up/Down_Thrust<-0.01f)
		output_down=Math.Min(Math.Abs(input_up/Down_Thrust), 1);
	float output_right=0.0f;
	float output_left=0.0f;
	if(input_right/Right_Thrust>0.01f)
		output_right=Math.Min(Math.Abs(input_right/Right_Thrust), 1);
	else if(input_right/Left_Thrust<-0.01f)
		output_left=Math.Min(Math.Abs(input_right/Left_Thrust), 1);
	
	const float MIN_THRUST=0.0001f;
	foreach(IMyThrust Thruster in Forward_Thrusters){
		Thruster.ThrustOverridePercentage=output_forward;
		if(output_forward<=0)
			Thruster.ThrustOverride=MIN_THRUST;
	}
	foreach(IMyThrust Thruster in Backward_Thrusters){
		Thruster.ThrustOverridePercentage=output_backward;
		if(output_backward<=0)
			Thruster.ThrustOverride=MIN_THRUST;
	}
	foreach(IMyThrust Thruster in Up_Thrusters){
		Thruster.ThrustOverridePercentage=output_up;
		if(output_up<=0)
			Thruster.ThrustOverride=MIN_THRUST;
	}
	foreach(IMyThrust Thruster in Down_Thrusters){
		Thruster.ThrustOverridePercentage=output_down;
		if(output_down<=0)
			Thruster.ThrustOverride=MIN_THRUST;
	}
	foreach(IMyThrust Thruster in Right_Thrusters){
		Thruster.ThrustOverridePercentage=output_right;
		if(output_right<=0)
			Thruster.ThrustOverride=MIN_THRUST;
	}
	foreach(IMyThrust Thruster in Left_Thrusters){
		Thruster.ThrustOverridePercentage=output_left;
		if(output_left<=0)
			Thruster.ThrustOverride=MIN_THRUST;
	}
	if(output_forward>0)
		Display(4,"Thrust Forwd:"+Math.Round(output_forward,1)+"%");
	if(output_backward>0)
		Display(4,"Thrust Back:"+Math.Round(output_backward,1)+"%");
	if(output_up>0)
		Display(4,"Thrust Up:"+Math.Round(output_up,1)+"%");
	if(output_down>0)
		Display(4,"Thrust Down:"+Math.Round(output_down,1)+"%");
	if(output_left>0)
		Display(4,"Thrust Left:"+Math.Round(output_left,1)+"%");
	if(output_right>0)
		Display(4,"Thrust Right:"+Math.Round(output_right,1)+"%");
}

Vector2I GetSize(IMyTextSurface Display){
	if(Display.Font!="Monospace")
		Display.Font="Monospace";
	Vector2 Size=Display.SurfaceSize;
	Vector2 CharSize=Display.MeasureStringInPixels(new StringBuilder("|"),Display.Font,Display.FontSize);
	float Padding=(100-Display.TextPadding)/100f;
	return new Vector2I((int)(Padding*Size.X/CharSize.X-2),(int)(Padding*Size.Y/CharSize.Y));
}

void Thruster_Graph(CustomPanel Panel){
	Vector2I Size=GetSize(Panel.Display);
	while(Panel.Display.FontSize>0.1&&Size.X<19&&Size.Y<19){
		float FontSize=Panel.Display.FontSize;
		FontSize=Math.Max(FontSize-0.1f,FontSize*.9f);
		Panel.Display.FontSize=FontSize;
		Size=GetSize(Panel.Display);
	}
	float output_forward=0,output_backward=0,output_up=0,output_down=0,output_left=0,output_right=0;
	if(Forward_Thrusters.Count>0)
		output_forward=Forward_Thrusters[0].ThrustOverridePercentage*Forward_Thrust;
	if(Backward_Thrusters.Count>0)
		output_backward=Backward_Thrusters[0].ThrustOverridePercentage*Backward_Thrust;
	if(Up_Thrusters.Count>0)
		output_up=Up_Thrusters[0].ThrustOverridePercentage*Up_Thrust;
	if(Down_Thrusters.Count>0)
		output_down=Down_Thrusters[0].ThrustOverridePercentage*Down_Thrust;
	if(Left_Thrusters.Count>0)
		output_left=Left_Thrusters[0].ThrustOverridePercentage*Left_Thrust;
	if(Right_Thrusters.Count>0)
		output_right=Right_Thrusters[0].ThrustOverridePercentage*Right_Thrust;
	
	int Width=(int)Size.X-4;
	string output="Thruster Output";
	output+="\nFB|";
	for(int i=3;i-3<Width/2;i++){
		int val=Width/2-(i-3);
		float thrust=Max_Thrust*((float)val-0.5f)/Width*2;
		if(thrust>Forward_Thrust)
			output+=' ';
		else if(thrust>output_forward)
			output+='○';
		else
			output+='•';
	}
	output+="|";
	for(int i=Width/2+4;i<Width;i++){
		int val=(i-3)-Width/2;
		float thrust=Max_Thrust*((float)val-0.5f)/Width*2;
		if(thrust>Backward_Thrust)
			output+=' ';
		else if(thrust>output_backward)
			output+='○';
		else
			output+='•';
	}
	
	output+="\nUD:";
	for(int i=3;i-3<Width/2;i++){
		int val=Width/2-(i-3);
		float thrust=Max_Thrust*((float)val-0.5f)/Width*2;
		if(thrust>Up_Thrust)
			output+=' ';
		else if(thrust>output_up)
			output+='○';
		else
			output+='•';
	}
	output+="|";
	for(int i=Width/2+4;i<Width;i++){
		int val=(i-3)-Width/2;
		float thrust=Max_Thrust*((float)val-0.5f)/Width*2;
		if(thrust>Down_Thrust)
			output+=' ';
		else if(thrust>output_down)
			output+='○';
		else
			output+='•';
	}
	
	output+="\nLR:";
	for(int i=3;i-3<Width/2;i++){
		int val=Width/2-(i-3);
		float thrust=Max_Thrust*((float)val-0.5f)/Width*2;
		if(thrust>Left_Thrust)
			output+=' ';
		else if(thrust>output_left)
			output+='○';
		else
			output+='•';
	}
	output+="|";
	for(int i=Width/2+4;i<Width;i++){
		int val=(i-3)-Width/2;
		float thrust=Max_Thrust*((float)val-0.5f)/Width*2;
		if(thrust>Right_Thrust)
			output+=' ';
		else if(thrust>output_right)
			output+='○';
		else
			output+='•';
	}
	
	Width=(int)Size.X-3;
	
	output+="\nFw:";
	for(int val=0;val<Width;val++){
		float thrust=Max_Thrust*((float)val-0.5f)/Width;
		if(thrust>Forward_Thrust)
			output+=' ';
		else if(thrust>output_forward)
			output+='○';
		else
			output+='•';
	}
	output+="\nBw:";
	for(int val=0;val<Width;val++){
		float thrust=Max_Thrust*((float)val-0.5f)/Width;
		if(thrust>Backward_Thrust)
			output+=' ';
		else if(thrust>output_backward)
			output+='○';
		else
			output+='•';
	}
	output+="\nUp:";
	for(int val=0;val<Width;val++){
		float thrust=Max_Thrust*((float)val-0.5f)/Width;
		if(thrust>Up_Thrust)
			output+=' ';
		else if(thrust>output_up)
			output+='○';
		else
			output+='•';
	}
	output+="\nDo:";
	for(int val=0;val<Width;val++){
		float thrust=Max_Thrust*((float)val-0.5f)/Width;
		if(thrust>Down_Thrust)
			output+=' ';
		else if(thrust>output_down)
			output+='○';
		else
			output+='•';
	}
	output+="\nLt:";
	for(int val=0;val<Width;val++){
		float thrust=Max_Thrust*((float)val-0.5f)/Width;
		if(thrust>Left_Thrust)
			output+=' ';
		else if(thrust>output_left)
			output+='○';
		else
			output+='•';
	}
	output+="\nRt:";
	for(int val=0;val<Width;val++){
		float thrust=Max_Thrust*((float)val-0.5f)/Width;
		if(thrust>Right_Thrust)
			output+=' ';
		else if(thrust>output_right)
			output+='○';
		else
			output+='•';
	}
	
	Panel.Display.WriteText(output,false);
}

class Notification{
	public string Text;
	public double Time;
	
	public Notification(string x,double t){
		Text=x;
		Time=t;
	}
}
List<Notification> Notifications;

void UpdateProgramInfo(){
	cycle=(++cycle)%long.MaxValue;
	switch(loading_char){
		case '|':
			loading_char='\\';
			break;
		case '\\':
			loading_char='-';
			break;
		case '-':
			loading_char='/';
			break;
		case '/':
			loading_char='|';
			break;
	}
	Write("",false,false);
	Echo(Program_Name+" OS\nCycle-"+cycle.ToString()+" ("+loading_char+")");
	Me.GetSurface(1).WriteText(Program_Name+" OS\nCycle-"+cycle.ToString()+" ("+loading_char+")",false);
	seconds_since_last_update=Runtime.TimeSinceLastRun.TotalSeconds + (Runtime.LastRunTimeMs / 1000);
	Display_Timer-=seconds_since_last_update;
	if(Display_Timer<=0){
		Current_Display=(Current_Display%Display_Count)+1;
		Display_Timer=5;
	}
	Write("Display "+Current_Display.ToString()+"/"+Display_Count.ToString());
	Echo(ToString(FromSeconds(seconds_since_last_update))+" since last cycle");
	Time_Since_Start=UpdateTimeSpan(Time_Since_Start,seconds_since_last_update);
	Echo(ToString(Time_Since_Start)+" since last reboot\n");
	Me.GetSurface(1).WriteText("\n"+ToString(Time_Since_Start)+" since last reboot",true);
}

void Crash_And_Autolanding(){
	double from_center=(Controller.GetPosition()-PlanetCenter).Length();
	Vector3D next_position=Controller.GetPosition()+1*CurrentVelocity;
	double Elevation_per_second=(from_center-(next_position-PlanetCenter).Length());
	Time_To_Crash=(Elevation-MySize/2)/Elevation_per_second;
	bool need_print=true;
	if(_Autoland)
		Write("Autoland Enabled");
	if(Time_To_Crash>0){
		if(Safety&&Time_To_Crash-Time_To_Stop<5&&Controller.GetShipSpeed()>5){
			Controller.DampenersOverride=true;
			RestingSpeed=0;
			for(int i=0;i<Notifications.Count;i++){
				if(Notifications[i].Text.IndexOf("Crash predicted within ")==0&&Notifications[i].Text.Contains(" seconds:\nEnabling Dampeners...")){
					Notifications.RemoveAt(i--);
					continue;
				}
			}
			Notifications.Add(new Notification("Crash predicted within "+Math.Round(5+CurrentSpeed/5,1)+" seconds:\nEnabling Dampeners...",2));
			need_print=false;
		}
		else if(Time_To_Crash*Math.Max(Elevation,1000)<1800000&&Controller.GetShipSpeed()>1.0f){
			Write(Math.Round(Time_To_Crash,1).ToString()+" seconds to crash");
			if(_Autoland&&(Time_To_Crash-Time_To_Stop>15||(CurrentSpeed<=5&&CurrentSpeed>2.5&&Time_To_Crash-Time_To_Stop>5)))
				Controller.DampenersOverride=false;
			need_print=false;
		}
		if(Elevation-MySize<5&&_Autoland)
			_Autoland=false;
	}
	if(need_print)
		Write("No crash likely at current velocity");
}
double LastElevation=double.MaxValue;
void UpdateSystemData(){
	Thrust_Check=false;
	Vector3D base_vector=new Vector3D(0,0,-1);
	Forward_Vector=LocalToGlobal(base_vector,Controller);
	Forward_Vector.Normalize();
	base_vector=new Vector3D(0,1,0);
	Up_Vector=LocalToGlobal(base_vector,Controller);
	Up_Vector.Normalize();
	base_vector=new Vector3D(-1,0,0);
	Left_Vector=LocalToGlobal(base_vector,Controller);
	Left_Vector.Normalize();
	Gravity=Controller.GetNaturalGravity();
	CurrentVelocity=Controller.GetShipVelocities().LinearVelocity;
	AngularVelocity=Controller.GetShipVelocities().AngularVelocity;
	
	//Time to Stop: Counter Velocity
	Vector3D Time=new Vector3D(0,0,0);
	if(Relative_CurrentVelocity.X>=0)
		Time.X=Left_Acc-Relative_Gravity.X;
	else
		Time.X=Right_Acc-Relative_Gravity.X;
	Time.X=Math.Abs(Relative_CurrentVelocity.X/Time.X);
	if(Relative_CurrentVelocity.Y>=0)
		Time.Y=Down_Acc-Relative_Gravity.Y;
	else
		Time.Y=Up_Acc-Relative_Gravity.Y;
	Time.Y=Math.Abs(Relative_CurrentVelocity.Y/Time.Y);
	if(Relative_CurrentVelocity.Z>=0)
		Time.Z=Forward_Acc-Relative_Gravity.Z;
	else
		Time.Z=Backward_Acc-Relative_Gravity.Z;
	Time.Z=Math.Abs(Relative_CurrentVelocity.X/Time.Z);
	Time_To_Stop=Math.Max(Math.Max(Time.X,Time.Y),Time.Z);
	
	Time_To_Crash=-1;
	LastElevation=Elevation;
	Elevation=double.MaxValue;
	if(Controller.TryGetPlanetElevation(MyPlanetElevation.Sealevel,out Sealevel)){
		if(Controller.TryGetPlanetPosition(out PlanetCenter)){
			if(Sealevel<6000&&Controller.TryGetPlanetElevation(MyPlanetElevation.Surface,out Elevation)){
				if(Sealevel>5000){
					double difference=Sealevel-5000;
					Elevation=((Elevation*(1000-difference))+(Sealevel*difference))/1000;
				}
				else if(Elevation<500){
					double terrain_height=(Controller.GetPosition()-PlanetCenter).Length()-Elevation;
					foreach(IMyLandingGear Block in LandingGear)
						Elevation=Math.Min(Elevation,(Block.GetPosition()-PlanetCenter).Length()-terrain_height);
				}
			}
			else
				Elevation=Sealevel;
			if(!Me.CubeGrid.IsStatic)
				Crash_And_Autolanding();
		}
		else
			PlanetCenter=new Vector3D(0,0,0);
	}
	else
		Sealevel=double.MaxValue;
	Elevation=Math.Max(Elevation,0);
	Mass_Accomodation=(float)(Controller.CalculateShipMass().PhysicalMass*Gravity.Length());
}

void PrintNotifications(){
	if(Notifications.Count>0){
		string written=Me.GetSurface(0).GetText();
		Me.GetSurface(0).WriteText("",false);
		try{
			Write("--Notifications--");
			Dictionary<string,int> N_Counter=new Dictionary<string,int>();
			List<string> Messages=new List<string>();
			for(int i=0;i<Notifications.Count;i++){
				Notifications[i].Time=Math.Max(0,Notifications[i].Time-seconds_since_last_update);
				string text=Notifications[i].Text;
				if(N_Counter.ContainsKey(text))
					N_Counter[text]++;
				else{
					N_Counter.Add(text,1);
					Messages.Add(text);
				}
				if(Notifications[i].Time<=0){
					Notifications.RemoveAt(i--);
					continue;
				}
			}
			foreach(string Text in Messages){
				string str="";
				int count=N_Counter[Text];
				if(count>1)
					str="("+count.ToString()+") ";
				Write("\""+str+Text+"\"");
			}
			Write("--Program--");
		}
		catch(Exception e){
			Me.GetSurface(0).WriteText(written,true);
			throw e;
		}
		Me.GetSurface(0).WriteText(written,true);
	}
}

public void Main(string argument,UpdateType updateSource){
	try{
		UpdateProgramInfo();
		if(updateSource==UpdateType.Script)
			TaskParser(argument);
		else if(updateSource!=UpdateType.Terminal)
			Main_Program(argument);
		else{
			if(argument.ToLower().IndexOf("task:")==0)
				TaskParser(argument.Substring(5));
			else
				Main_Program(argument);
		}
		PrintNotifications();
		if(Current_Display==5)
			Thruster_Graph(new CustomPanel(Me.GetSurface(0)));
	}
	catch(Exception E){
		Write(E.ToString());
		FactoryReset();
	}
}

enum Quantifier{
	Once=0,
	Numbered=1,
	Until=2,
	Stop=3
}
struct TaskFormat{
	public string Type;
	public List<Quantifier> Durations;
	public Vector2 QualifierLimits;
	
	public TaskFormat(string T,List<Quantifier> Q,Vector2 L){
		Type=T;
		Durations=new List<Quantifier>();
		foreach(Quantifier q in Q)
			Durations.Add(q);
		QualifierLimits=L;
	}
	
	public bool Validate(Task input){
		if(!input.Type.Equals(Type))
			return false;
		if(!Durations.Contains(input.Duration))
			return false;
		if(input.Duration==Quantifier.Numbered){
			if(input.Qualifiers.Count-1<QualifierLimits.X)
				return false;
			if(QualifierLimits.Y>=0&&input.Qualifiers.Count-1>QualifierLimits.Y)
				return false;
		}
		else if(input.Duration==Quantifier.Stop){
			if(input.Qualifiers.Count!=0)
				return false;
		}
		else{
			if(input.Qualifiers.Count<QualifierLimits.X)
				return false;
			if(QualifierLimits.Y>=0&&input.Qualifiers.Count>QualifierLimits.Y)
				return false;
		}
		return true;
	}
}
class Task{
	public string Type;
	public Quantifier Duration;
	public List<string> Qualifiers;
	
	public bool Valid{
		get{
			int t=0;
			if(Type.Length==0)
				return false;
			if(!Type.Substring(0,1).Equals(Type.Substring(0,1).ToUpper()))
				return false;
			if(!Type.Substring(1).Equals(Type.Substring(1).ToLower()))
				return false;
			switch(Duration){
				case Quantifier.Numbered:
					if(Qualifiers.Count<1||!Int32.TryParse(Qualifiers[0],out t))
						return false;
					if(t<0)
						return false;
					break;
				case Quantifier.Stop:
					if(Qualifiers.Count>0)
						return false;
					break;
			}
			foreach(string Q in Qualifiers){
				if(Q.Contains('•')||Q.Contains('\n'))
					return false;
			}
			foreach(TaskFormat Format in ValidFormats){
				if(Format.Validate(this))
					return true;
			}
			return false;
		}
	}
	
	public Task(string T,Quantifier D){
		Type=T;
		Duration=D;
		Qualifiers=new List<string>();
	}
	
	public Task(string T, Quantifier D, List<string> Q):this(T,D){
		foreach(string s in Q)
			Qualifiers.Add(s);
	}
	
	public override string ToString(){
		string output=Type+'\n'+Duration.ToString();
		foreach(string Q in Qualifiers)
			output+='\n'+Q;
		return output;
	}
	
	public static bool TryParse(string input,out Task output){
		output=null;
		string[] args=input.Split('\n');
		if(args.Length<2)
			return false;
		if(args[0].Length==0)
			return false;
		string type=args[0];
		Quantifier duration;
		if(!Quantifier.TryParse(args[1],out duration))
			return false;
		List<string> qualifiers=new List<string>();
		for(int i=2;i<args.Length;i++){
			qualifiers.Add(args[i]);
		}
		output=new Task(type,duration,qualifiers);
		return output.Valid;
	}
	
	public static List<TaskFormat> ValidFormats{
		get{
			List<TaskFormat> output=new List<TaskFormat>();
			
			output.Add(new TaskFormat(
			"Send",
			new List<Quantifier>(new Quantifier[] {Quantifier.Once,Quantifier.Numbered}),
			new Vector2(1,-1)
			)); //Params: ProgName, [Arguments]
			
			output.Add(new TaskFormat(
			"Direction",
			new List<Quantifier>(new Quantifier[] {Quantifier.Numbered,Quantifier.Until,Quantifier.Stop}),
			new Vector2(1,1)
			)); //Params: Vector3D
			
			output.Add(new TaskFormat(
			"Up",
			new List<Quantifier>(new Quantifier[] {Quantifier.Numbered,Quantifier.Until,Quantifier.Stop}),
			new Vector2(1,1)
			)); //Params: Vector3D
			
			output.Add(new TaskFormat(
			"Go",
			new List<Quantifier>(new Quantifier[] {Quantifier.Numbered,Quantifier.Until,Quantifier.Stop}),
			new Vector2(1,1)
			)); //Params: Vector3D
			
			output.Add(new TaskFormat(
			"Match",
			new List<Quantifier>(new Quantifier[] {Quantifier.Numbered,Quantifier.Until,Quantifier.Stop}),
			new Vector2(0,0)
			)); //No Params
			
			output.Add(new TaskFormat(
			"Autoland",
			new List<Quantifier>(new Quantifier[] {Quantifier.Numbered,Quantifier.Until,Quantifier.Stop}),
			new Vector2(1,1)
			)); //Params: bool
			
			return output;
		}
	}
}
Queue<Task> Task_Queue; //When a task is added, it is added to the Task Queue to be performed

//Sends an argument to a programmable block
bool Task_Send(Task task){
	IMyProgrammableBlock target=GenericMethods<IMyProgrammableBlock>.GetFull(task.Qualifiers[0]);
	if(target==null)
		return false;
	string arguments="";
	for(int i=1;i<task.Qualifiers.Count;i++){
		if(i!=1)
			arguments+='\n';
		arguments+=task.Qualifiers[i];
	}
	return target.TryRun(arguments);
}

//Tells the ship to orient to a specific forward direction
bool Task_Direction(Task task){
	Vector3D direction=new Vector3D(0,0,0);
	if(task.Qualifiers[0].IndexOf("At ")==0){
		if(Vector3D.TryParse(task.Qualifiers.Last().Substring(3),out direction)){
			Target_Direction=direction-Controller.GetPosition();
			Target_Direction.Normalize();
			Do_Direction=true;
			return true;
		}
	}
	else if(Vector3D.TryParse(task.Qualifiers.Last(),out direction)){
		Target_Direction=direction;
		if(Target_Direction.Length()==0)
			return false;
		Target_Direction.Normalize();
		Do_Direction=true;
		return true;
	}
	return false;
}

//Tells the ship to orient to a specific up direction
bool Task_Up(Task task){
	Vector3D direction=new Vector3D(0,0,0);
	if(Vector3D.TryParse(task.Qualifiers.Last(),out direction)){
		Target_Up=direction;
		Target_Up.Normalize();
		Do_Up=true;
		return true;
	}
	return false;
}

struct Plane{
	public double A;
	public double B;
	public double C;
	public double D;
	
	public Plane(double a,double b,double c,double d){
		A=a;
		B=b;
		C=c;
		D=d;
	}
	
	public Plane(Vector3D a,Vector3D b,Vector3D c){
		A=(b.Y-a.Y)*(c.Z-a.Z)-(c.Y-a.Y)*(b.Z-a.Z);
		B=(b.Z-a.Z)*(c.X-a.X)-(c.Z-a.Z)*(b.X-a.X);
		C=(b.X-a.X)*(c.Y-a.Y)-(c.X-a.X)*(b.Y-a.Y);
		D=-1*(A*a.X+B*a.Y+C*a.Z);
	}
	
	//Creates a Tangent Plane at the given point with the given normal
	public Plane(Vector3D Point,Vector3D Normal){
		A=Normal.X;
		B=Normal.Y;
		C=Normal.Z;
		D=-1*(Normal.X*Point.X+Normal.Y*Point.Y+Normal.Z*Point.Z);
	}
	
	//Creates a Normal Vector from the plane
	public Vector3D Normal(){
		Vector3D output=new Vector3D(A,B,C);
		output.Normalize();
		return output;
	}
}
struct Sphere{
	public Vector3D Center;
	public double Radius;
	
	public Sphere(Vector3D c,double r){
		Center=c;
		Radius=r;
	}
	
	public bool On(Vector3D p){
		return Math.Abs(Math.Pow(p.X-Center.X,2)+Math.Pow(p.Y-Center.Y,2)+Math.Pow(p.Z-Center.Z,2)-Radius)<0.00001;
	}
}
struct Line{
	public Vector3D R;
	public Vector3D V;
	
	//Tangent Line on the Circle, where Vector3D Point is on the circle that results from the intersection of Plane P and Sphere S
	public Line(Plane P,Sphere S,Vector3D Point){
		Vector3D Tangent_Normal=Point-S.Center;
		Tangent_Normal.Normalize();
		V=Vector3D.Cross(P.Normal(),Tangent_Normal);
		R=Point;
	}
}

Vector3D True_Target_Position=new Vector3D(0,0,0);
//Tells the ship to fly to a specific location
bool Task_Go(Task task){
	Vector3D position=new Vector3D(0,0,0);
	if(Vector3D.TryParse(task.Qualifiers.Last(),out position)){
		Target_Position=position;
		True_Target_Position=position;
		Do_Position=true;
		if(Gravity.Length()>0){
			Vector3D MyPosition=Controller.GetPosition();
			double my_radius=(MyPosition-PlanetCenter).Length();
			Vector3D target_direction=Target_Position-PlanetCenter;
			double target_radius=target_direction.Length();
			target_direction.Normalize();
			double planet_radius=my_radius-Elevation;
			double sealevel_radius=my_radius-Sealevel;
			Vector3D me_direction=MyPosition-PlanetCenter;
			me_direction.Normalize();
			double planet_angle=GetAngle(me_direction,target_direction);
			Write("Planetary Angle: "+Math.Round(planet_angle,1).ToString()+"°");
			if(target_radius>sealevel_radius+100){
				if(target_radius>planet_radius/3){
					if(planet_angle>30){
						if(target_radius>my_radius+2000)
							Target_Position=PlanetCenter-(planet_radius*4/3*Gravity_Direction);
						return true;
					}
				}
				else{
					Target_Position=PlanetCenter-((sealevel_radius+500)*Gravity_Direction);
					return true;
				}
			}
			if((planet_angle>2.5||(planet_angle>5&&target_radius-my_radius>2000))||Elevation-MySize/2<Math.Max(30,Target_Distance/10)){
				while(planet_angle==180){
					//This offsets the angle so we can create a full Plane from the 3 points: Center,Here,Target
					Vector3D offset=new Vector3D(Rnd.Next(0,10)-5,Rnd.Next(0,10)-5,Rnd.Next(0,10)-5);
					offset.Normalize();
					Target_Position+=offset;
					target_direction=Target_Position-PlanetCenter;
					target_direction.Normalize();
					planet_angle=GetAngle(me_direction,target_direction);
				}
				double ExpectedForwardMovement=CurrentSpeed/60;
				double UpwardMovementLimit=ExpectedForwardMovement*2;
				bool WorryElevation=(cycle>10&&Runtime.UpdateFrequency==UpdateFrequency.Update1)?LastElevation-Elevation>UpwardMovementLimit:false;
				double GoalElevation=WorryElevation?500:300;
				
				if(Elevation<(WorryElevation?400:250)+MySize){
					//This increases the cruising altitude if the elevation is too low, for collision avoidance
					my_radius+=Math.Max(Math.Min(9.375*(GoalElevation-(Elevation-MySize)),2500),250);
					MyPosition=(my_radius)*me_direction+PlanetCenter;
				}
				Target_Position=(my_radius)*target_direction+PlanetCenter;
				//Target is now at same altitude with respect to sealevel
				Plane Bisect=new Plane(Target_Position,MyPosition,PlanetCenter);
				//This plane now bisects the planet along both the current location and target
				Sphere Planet=new Sphere(PlanetCenter,my_radius);
				//This sphere now represents the planet at the current elevation
				Line Tangent=new Line(Bisect,Planet,MyPosition);
				//This line now represents the tangent of the planet in a direction that lines up with the target
				Vector3D Goal_Direction=Tangent.V;
				Vector3D My_Direction=Target_Position-MyPosition;
				My_Direction.Normalize();
				if(GetAngle(Goal_Direction,My_Direction)>GetAngle(-1*Goal_Direction,My_Direction))
					Goal_Direction*=-1;
				Target_Position=Goal_Direction*2500+MyPosition;
			}
		}
		return true;
	}
	return false;
}

//Tells the ship to match position
bool Task_Match(Task task){
	Match_Direction=true;
	return true;
}

//Sets Autoland either on or off
bool Task_Autoland(Task task){
	bool do_autoland=false;
	if(bool.TryParse(task.Qualifiers.Last(),out do_autoland)){
		if(do_autoland)
			return _Autoland||Autoland();
		else
			return (!_Autoland)||Autoland();
	}
	return false;
}

bool PerformTask(Task task){
	if(task.Duration==Quantifier.Stop){
		Queue<Task> Recycling=new Queue<Task>();
		bool found=false;
		while(Task_Queue.Count>0){
			Task t=Task_Queue.Dequeue();
			if(!t.Type.Equals(task.Type))
				Recycling.Enqueue(t);
			else
				found=true;
		}
		while(Recycling.Count>0)
			Task_Queue.Enqueue(Recycling.Dequeue());
		return found;
	}
	switch(task.Type){
		case "Send":
			return Task_Send(task);
		case "Direction":
			return Task_Direction(task);
		case "Up":
			return Task_Up(task);
		case "Go":
			return Task_Go(task);
		case "Match":
			return Task_Match(task);
		case "Autoland":
			return Task_Autoland(task);
		
	}
	return false;
}

void ProcessTasks(){
	Task_Resetter();
	if(Task_Queue.Count==0)
		return;
	Queue<Task> Recycling=new Queue<Task>();
	while(Task_Queue.Count>0){
		Task task=Task_Queue.Dequeue();
		if(!task.Valid){
			Notifications.Add(new Notification("Discarded invalid Task: \""+task.ToString()+"\"",5));
			continue;
		}
		if(!PerformTask(task)){
			Recycling.Enqueue(task);
			Write("Failed to run task "+task.Type.ToUpper());
		}
		else{
			switch(task.Duration){
				case Quantifier.Numbered:
					int num=0;
					Int32.TryParse(task.Qualifiers[0],out num);
					num--;
					if(num>0){
						task.Qualifiers[0]=num.ToString();
						Recycling.Enqueue(task);
					}
					Write("Ran task "+task.Type.ToUpper()+" ["+num.ToString()+"]");
					break;
				case Quantifier.Until:
					Recycling.Enqueue(task);
					Write("Ran task "+task.Type.ToUpper()+" [u]");
					break;
				default:
					Notifications.Add(new Notification("Ran task "+task.Type.ToUpper(),10));
					break;
			}
		}
	}
	while(Recycling.Count>0)
		Task_Queue.Enqueue(Recycling.Dequeue());
}

void Task_Resetter(){
	Do_Direction=false;
	Do_Up=false;
	Do_Position=false;
	Match_Direction=false;
	if(_Autoland)
		Autoland();
}

void Task_Pruner(Task task){
	bool duplicate=false;
	foreach(Task t in Task_Queue){
		if(t.Type==task.Type){
			duplicate=true;
			break;
		}
	}
	if(duplicate){
		Queue<Task> Recycling=new Queue<Task>();
		while(Task_Queue.Count>0){
			Task t=Task_Queue.Dequeue();
			if(!t.Type.Equals(task.Type))
				Recycling.Enqueue(t);
		}
		while(Recycling.Count>0)
			Task_Queue.Enqueue(Recycling.Dequeue());
	}
}

void TaskParser(string argument){
	string[] tasks=argument.Split('•');
	foreach(string task in tasks){
		if(task.Trim().Length==0)
			continue;
		Task t=null;
		if(Task.TryParse(task,out t)){
			if(t.Duration==Quantifier.Stop)
				PerformTask(t);
			else{
				Task_Pruner(t);
				Task_Queue.Enqueue(t);
			}
		}
		else{
			if(t==null)
				Notifications.Add(new Notification("Failed to parse \""+task+"\"",15));
			else{
				Notifications.Add(new Notification("Failed to parse \""+task+"\": Got\""+t.ToString()+"\"",15));
			}
		}
	}
}

void Main_Program(string argument){
	ProcessTasks();
	UpdateSystemData();
	if(!Me.CubeGrid.IsStatic){
		if(Elevation!=double.MaxValue){
			Display(1,"Elevation: "+Math.Round(Elevation,1).ToString());
			Display(1,"Sealevel: "+Math.Round(Sealevel,1).ToString());
		}
		if(Gravity.Length()>0){
			Display(1,"Gravity:"+Math.Round(Gravity.Length()/9.814,2)+"Gs");
			double grav=Relative_Gravity.Y;
			if(Relative_Gravity.Y>0.1)
				Display(1,"Gravity Up:"+Math.Round(Math.Abs(grav)/9.814,2)+"Gs");
			else if(Relative_Gravity.Y<-0.1)
				Display(1,"Gravity Down:"+Math.Round(Math.Abs(grav)/9.814,2)+"Gs");
			grav=Relative_Gravity.Z;
			if(Relative_Gravity.Z>0.1)
				Display(1,"Gravity Back:"+Math.Round(Math.Abs(grav)/9.814,2)+"Gs");
			else if(Relative_Gravity.Z<-0.1)
				Display(1,"Gravity Forwd:"+Math.Round(Math.Abs(grav)/9.814,2)+"Gs");
			grav=Relative_Gravity.X;
			if(Relative_Gravity.X>0.1)
				Display(1,"Gravity Right:"+Math.Round(Math.Abs(grav)/9.814,2)+"Gs");
			else if(Relative_Gravity.X<-0.1)
				Display(1,"Gravity Left:"+Math.Round(Math.Abs(grav)/9.814,2)+"Gs");
		}
		Display(2,"Acceleration Up:"+Math.Round(Up_Gs,2)+"Gs");
		Display(2,"Acceleration Forwd:"+Math.Round(Forward_Gs,2)+"Gs");
		Display(2,"Acceleration Back:"+Math.Round(Backward_Gs,2)+"Gs");
		Display(2,"Acceleration Down:"+Math.Round(Down_Gs,2)+"Gs");
		Display(2,"Acceleration Left:"+Math.Round(Left_Gs,2)+"Gs");
		Display(2,"Acceleration Right:"+Math.Round(Right_Gs,2)+"Gs");
	}
	if(argument.ToLower().Equals("autoland")){
		Autoland();
	}
	else if(argument.ToLower().Equals("factory reset")){
		FactoryReset();
	}
	
	if(!Me.CubeGrid.IsStatic&&Controller.CalculateShipMass().PhysicalMass>0){
		if(Control_Thrusters)
			SetThrusters();
		else
			ResetThrusters();
		if(Control_Gyroscopes)
			SetGyroscopes();
		else
			Gyroscope.GyroOverride=false;
	}
	else
		ResetThrusters();
	foreach(CustomPanel Panel in ThrusterLCDs)
		Thruster_Graph(Panel);
	Runtime.UpdateFrequency=GetUpdateFrequency();
}