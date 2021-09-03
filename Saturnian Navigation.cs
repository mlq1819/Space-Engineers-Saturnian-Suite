/*
* Saturnian Navigation OS
* Built by mlq1616
* https://github.com/mlq1819/Space-Engineers-Saturnian-Suite
* This suite handles navigation, major autopiloting, etc. 
* Include "Altitude" in LCD name to add to group.


TODO: 
- EntityDatabase Integration
- Collision Prediction
*/
string Program_Name="Saturnian Navigation";
Color DEFAULT_TEXT_COLOR=new Color(197,137,255,255);
Color DEFAULT_BACKGROUND_COLOR=new Color(44,0,88,255);
double Acceptable_Angle=5;
int Graph_Length_Seconds=90;

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
		if(value==3&&AltitudeTerrainLCDs.Count>0)
			value=4;
		if(value==4&&AltitudeDistanceLCDs.Count>0)
			value=5;
		if(value==5&&DistanceTimeLCDs.Count>0)
			value=1;
		_Current_Display=value;
	}
}
double Display_Timer=5;
void Display(int display_number,string text,bool new_line=true,bool append=true){
	if(display_number==Current_Display)
		Write(text,new_line,append);
	else
		Echo(text);
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

struct Altitude_Terrain{
	public TimeSpan Timestamp;
	public double Sealevel;
	public double Elevation;
	public double Terrain{
		get{
			return Sealevel-Elevation;
		}
	}
	
	public Altitude_Terrain(double S,double E,TimeSpan start){
		Sealevel=S;
		Elevation=E;
		Timestamp=start;
	}
}

struct Altitude_Distance{
	public TimeSpan Timestamp;
	public double Sealevel;
	public double Distance;
	
	public Altitude_Distance(double S,double D,TimeSpan start){
		Sealevel=S;
		Distance=D;
		Timestamp=start;
	}
}

struct Distance_Time{
	public TimeSpan Timestamp;
	public double Distance;
	
	public Distance_Time(double D,TimeSpan start){
		Distance=D;
		Timestamp=start;
	}
}

TimeSpan Time_Since_Start=new TimeSpan(0);
long cycle=0;
char loading_char='|';
double seconds_since_last_update=0;
Random Rnd;

IMyShipController Controller;
List<IMyShipController> Controllers;
IMyProgrammableBlock MovementProgram;

List<CustomPanel> AltitudeTerrainLCDs;
List<CustomPanel> AltitudeDistanceLCDs;
List<CustomPanel> DistanceTimeLCDs;

Queue<Altitude_Terrain> Altitude_Terrain_Graph;
Queue<Altitude_Distance> Altitude_Distance_Graph;
Queue<Distance_Time> Distance_Time_Graph;
double Graph_Timer=0;

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
	return UpdateFrequency.Update10;
}

void Reset(){
	Operational=false;
	Runtime.UpdateFrequency=UpdateFrequency.None;
	Controller=null;
	Controllers=new List<IMyShipController>();
	for(int i=0;i<All_Thrusters.Length;i++)
		All_Thrusters[i]=new List<IMyThrust>();
	AltitudeTerrainLCDs=new List<CustomPanel>();
	AltitudeDistanceLCDs=new List<CustomPanel>();
	DistanceTimeLCDs=new List<CustomPanel>();
	Altitude_Terrain_Graph=new Queue<Altitude_Terrain>();
	Altitude_Distance_Graph=new Queue<Altitude_Distance>();
	Distance_Time_Graph=new Queue<Distance_Time>();
	Notifications=new List<Notification>();
	MovementProgram=null;
}

double MySize=0;
bool Setup(){
	Reset();
	List<IMyTextPanel> LCDs=GenericMethods<IMyTextPanel>.GetAllConstruct("Altitude-Terrain");
	List<CustomPanel> MyLCDs=new List<CustomPanel>();
	foreach(IMyTextPanel Panel in LCDs){
		AltitudeTerrainLCDs.Add(new CustomPanel(Panel));
		MyLCDs.Add(new CustomPanel(Panel));
		Panel.WritePublicTitle("Altitude-Terrain Graph",false);
	}
	LCDs=GenericMethods<IMyTextPanel>.GetAllConstruct("Altitude-Distance");
	foreach(IMyTextPanel Panel in LCDs){
		AltitudeDistanceLCDs.Add(new CustomPanel(Panel));
		MyLCDs.Add(new CustomPanel(Panel));
		Panel.WritePublicTitle("Altitude-Distance Graph",false);
	}
	LCDs=GenericMethods<IMyTextPanel>.GetAllConstruct("Distance-Time");
	foreach(IMyTextPanel Panel in LCDs){
		DistanceTimeLCDs.Add(new CustomPanel(Panel));
		MyLCDs.Add(new CustomPanel(Panel));
		Panel.WritePublicTitle("Distance-Time Graph",false);
	}
	foreach(CustomPanel Panel in MyLCDs){
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
		Panel.Display.FontSize=0.5f;
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
	MovementProgram=GenericMethods<IMyProgrammableBlock>.GetContaining("Maneuvering");
	if(MovementProgram==null)
		MovementProgram=GenericMethods<IMyProgrammableBlock>.GetContaining("Steering");
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
		Me.GetSurface(i).Font="Debug";
	}
	Me.GetSurface(1).FontSize=2.2f;
	Me.GetSurface(1).TextPadding=30.0f;
	Echo("Beginning initialization");
	Rnd=new Random();
	/*string[] args=this.Storage.Split('•');
	foreach(string arg in args){
		if(!arg.Contains(':'))
			continue;
		int index=arg.IndexOf(':');
		string name=arg.Substring(0,index);
		string data=arg.Substring(index+1);
		switch(name){
			
		}
	}*/
	Notifications=new List<Notification>();
	Task_Queue=new Queue<Task>();
	TaskParser(Me.CustomData);
	Setup();
}

public void Save(){
	this.Storage="";
	Me.CustomData="";
	foreach(Task T in Task_Queue){
		Me.CustomData+=T.ToString()+'•';
	}
}

bool Disable(){
	Operational=false;
	
	
	
	Runtime.UpdateFrequency=UpdateFrequency.None;
	Me.Enabled=false;
	return true;
}
bool FactoryReset(){
	Me.CustomData="";
	this.Storage="";
	Reset();
	Me.CustomData="";
	this.Storage="";
	Me.Enabled=false;
	return true;
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
	if(Graph_Timer>0)
		Graph_Timer-=seconds_since_last_update;
	Write("Display "+Current_Display.ToString()+"/"+Display_Count.ToString());
	UpdateMyDisplay();
	Echo(ToString(FromSeconds(seconds_since_last_update))+" since last cycle");
	Time_Since_Start=UpdateTimeSpan(Time_Since_Start,seconds_since_last_update);
	Echo(ToString(Time_Since_Start)+" since last reboot\n");
	Me.GetSurface(1).WriteText("\n"+ToString(Time_Since_Start)+" since last reboot",true);
}

void UpdateMyDisplay(){
	IMyTextSurface Display=Me.GetSurface(0);
	if(Current_Display>=3){
		Display.FontColor=DEFAULT_TEXT_COLOR;
		Display.BackgroundColor=DEFAULT_BACKGROUND_COLOR;
		Display.Alignment=TextAlignment.LEFT;
		Display.ContentType=ContentType.TEXT_AND_IMAGE;
		Display.Font="Monospace";
		Display.TextPadding=0;
		Display.FontSize=0.5f;
	}
	else{
		Display.FontColor=DEFAULT_TEXT_COLOR;
		Display.BackgroundColor=DEFAULT_BACKGROUND_COLOR;
		Display.Alignment=TextAlignment.CENTER;
		Display.ContentType=ContentType.TEXT_AND_IMAGE;
		Display.Font="Debug";
		Display.TextPadding=2;
		Display.FontSize=1;
	}
}

void MarkGraphs(bool do_new=true){
	if(do_new)
		Graph_Timer=((double)Graph_Length_Seconds)/60;
	while(Altitude_Terrain_Graph.Count>0&&Time_Since_Start.TotalSeconds-Altitude_Terrain_Graph.Peek().Timestamp.TotalSeconds>Graph_Length_Seconds)
		Altitude_Terrain_Graph.Dequeue();
	while(Altitude_Distance_Graph.Count>(Target_Altitude==double.MinValue?0:1024))
		Altitude_Distance_Graph.Dequeue();
	while(Distance_Time_Graph.Count>0&&Time_Since_Start.TotalSeconds-Distance_Time_Graph.Peek().Timestamp.TotalSeconds>Graph_Length_Seconds)
		Distance_Time_Graph.Dequeue();
	if(do_new){
		if(Gravity.Length()>0){
			Altitude_Terrain_Graph.Enqueue(new Altitude_Terrain(Sealevel,Elevation,Time_Since_Start));
			if(Target_Distance!=double.MinValue)
				Altitude_Distance_Graph.Enqueue(new Altitude_Distance(Sealevel,Target_Distance,Time_Since_Start));
		}
		if(Target_Distance!=double.MinValue)
			Distance_Time_Graph.Enqueue(new Distance_Time(Target_Distance,Time_Since_Start));
	}
}

Vector2I GetSize(IMyTextSurface Display){
	if(Display.Font!="Monospace")
		Display.Font="Monospace";
	Vector2 Size=Display.SurfaceSize;
	Vector2 CharSize=Display.MeasureStringInPixels(new StringBuilder("|"),Display.Font,Display.FontSize);
	float Padding=(100-Display.TextPadding)/100f;
	return new Vector2I((int)(Padding*Size.X/CharSize.X-2),(int)(Padding*Size.Y/CharSize.Y));
}


struct AltitudeTerrain_Point{
	public int X;
	public int Y_Ship;
	public int Y_Terrain;
	
	public AltitudeTerrain_Point(int x,int y_ship,int y_terrain){
		X=x;
		Y_Ship=y_ship;
		Y_Terrain=y_terrain;
	}
}

double Target_Altitude=double.MinValue;
double Target_Distance=double.MinValue;
void PrintAltitudeTerrain(CustomPanel Panel){
	if(Runtime.UpdateFrequency==UpdateFrequency.Update1&&cycle%10!=0)
		return;
	Vector2I Size=GetSize(Panel.Display);
	while(Panel.Display.FontSize>0.1&&Size.X<50&&Size.Y<40){
		float FontSize=Panel.Display.FontSize;
		FontSize=Math.Max(FontSize-0.1f,FontSize*.9f);
		Panel.Display.FontSize=FontSize;
		Size=GetSize(Panel.Display);
	}
	int XLEN=Size.X-3;
	
	double max=double.MinValue;
	double min=double.MaxValue;
	foreach(Altitude_Terrain Data in Altitude_Terrain_Graph){
		max=Math.Max(max,Math.Max(Data.Terrain,Data.Sealevel));
		min=Math.Min(min,Math.Min(Data.Terrain,Data.Sealevel));
	}
	
	if(Target_Altitude!=double.MinValue){
		max=Math.Max(max,Target_Altitude);
		min=Math.Min(min,Target_Altitude);
	}
	else if(Altitude_Terrain_Graph.Count==0){
		min=-1000;
		max=1000;
	}
	max=Math.Max(min+500,max+50);
	min=min-50;
	max=Math.Ceiling(max/100)*100;
	min=Math.Floor(min/100)*100;
	
	double interval=(max-min)/(Size.Y-1);
	
	char[][] Graph=new char[Size.Y][];
	for(int y=0;y<Size.Y;y++){
		Graph[y]=new char[Size.X];
		double altitude=(min+y*interval);
		int alt_num=(int)Math.Floor(altitude/1000);
		int low_alt=(int)Math.Floor((altitude-interval)/1000);
		char alt_10s=((Math.Abs(alt_num)/10)%10).ToString()[0];
		if(alt_num<0)
			alt_10s='-';
		char alt_1s=(Math.Abs(alt_num)%10).ToString()[0];
		for(int x=0;x<Size.X;x++){
			Graph[y][x]=' ';
			if(x<2){
				if(alt_num!=low_alt||(min==0&&y==0)){
					if(x==0)
						Graph[y][x]=alt_10s;
					else
						Graph[y][x]=alt_1s;
				}
				else if(((int)Math.Floor(altitude/500))!=((int)Math.Floor((altitude-interval)/500)))
					Graph[y][x]='-';
				else if(x==1&&((int)Math.Floor(altitude/250))!=((int)Math.Floor((altitude-interval)/250)))
					Graph[y][x]='-';
			}
			else if(x==2){
				Graph[y][x]='|';
			}
		}
	}
	
	double time_interval=Graph_Length_Seconds/((double)XLEN);
	double End=Time_Since_Start.TotalSeconds;
	double Start=End-Graph_Length_Seconds;
	
	List<AltitudeTerrain_Point> Points=new List<AltitudeTerrain_Point>();
	foreach(Altitude_Terrain Point in Altitude_Terrain_Graph){
		int X=(int)Math.Ceiling((Point.Timestamp.TotalSeconds-Start)/time_interval);
		int Y_Ship=(int)Math.Round((Point.Sealevel-min)/interval,0);
		int Y_Terrain=(int)Math.Round((Point.Terrain-min)/interval,0);
		if(X>=0&&X<XLEN)
			Points.Add(new AltitudeTerrain_Point(X,Y_Ship,Y_Terrain));
	}
	
	int Y_Target=(Target_Altitude==double.MinValue?-1:(int)Math.Round((Target_Altitude-min)/interval,0));
	if(Y_Target>=0&&Y_Target<Size.Y){
		for(int i=0;i<XLEN;i+=2)
			Graph[Y_Target][i+3]='=';
	}
	else{
		Y_Target=-1;
	}
	
	for(int i=0;i<Points.Count;i++){
		AltitudeTerrain_Point Point=Points[i];
		int X=Point.X;
		int Y_Ship=Point.Y_Ship;
		int Y_Terrain=Point.Y_Terrain;
		
		for(int j=0;j<Size.Y;j++){
			if(j!=Y_Target||X%2!=0)
				Graph[j][X+3]=' ';
		}
		int min_ship=Y_Ship;
		int min_terrain=Y_Terrain;
		int terrain_from=0,terrain_to=0;
		if(i>0){
			AltitudeTerrain_Point Target=Points[i-1];
			min_ship=Math.Min(min_ship,Target.Y_Ship);
			min_terrain=Math.Min(min_terrain,Target.Y_Terrain);
			terrain_from=Y_Terrain-Target.Y_Terrain;
		}
		if(i+1<Points.Count){
			AltitudeTerrain_Point Target=Points[i+1];
			min_ship=Math.Min(min_ship,Target.Y_Ship);
			min_terrain=Math.Min(min_terrain,Target.Y_Terrain);
			terrain_to=Target.Y_Terrain-Y_Terrain;
		}
		
		
		if(Y_Ship-min_ship>1){
			for(int j=min_ship+1;j<Y_Ship;j++)
				Graph[j][X+3]='•';
		}
		for(int j=0;j<Y_Terrain;j++){
			if(j>min_terrain)
				Graph[j][X+3]='|';
			else
				Graph[j][X+3]='■';
		}
		
		
		if(Y_Ship>=0&&Y_Ship<Size.Y)
			Graph[Y_Ship][X+3]='○';
		
		if(Y_Terrain>=0&&Y_Terrain<Size.Y){
			int difference=Math.Abs(terrain_to)-Math.Abs(terrain_from);
			if(difference>0&&terrain_to>0)
				Graph[Y_Terrain][X+3]='/';
			else if(difference>0&&terrain_from>0)
				Graph[Y_Terrain][X+3]='\\';
			else
				Graph[Y_Terrain][X+3]='_';
		}
	}
	
	string time=Math.Round(Graph_Timer,3).ToString();
	for(int i=1;i<=time.Length;i++)
		Graph[Size.Y-1][Size.X-i]=time[time.Length-i];
	
	string text="";
	for(int y=Size.Y-1;y>=0;y--){
		if(y<Size.Y-1)
			text+='\n';
		for(int x=0;x<Size.X;x++){
			text+=Graph[y][x];
		}
	}
	Panel.Display.WriteText(text,false);
}

void PrintAltitudeDistance(CustomPanel Panel){
	if(Runtime.UpdateFrequency==UpdateFrequency.Update1&&cycle%10!=0)
		return;
	Vector2I Size=GetSize(Panel.Display);
	while(Panel.Display.FontSize>0.1&&Size.X<50&&Size.Y<40){
		float FontSize=Panel.Display.FontSize;
		FontSize=Math.Max(FontSize-0.1f,FontSize*.9f);
		Panel.Display.FontSize=FontSize;
		Size=GetSize(Panel.Display);
	}
	int XLEN=Size.X-3;
	int YLEN=Size.Y-2;
	
	double max_sealevel=double.MinValue;
	double min_sealevel=double.MaxValue;
	double max_distance=1000;
	double min_distance=0;
	foreach(Altitude_Distance Data in Altitude_Distance_Graph){
		max_sealevel=Math.Max(max_sealevel,Data.Sealevel);
		min_sealevel=Math.Min(min_sealevel,Data.Sealevel);
		max_distance=Math.Max(max_distance,Data.Distance);
	}
	if(Altitude_Distance_Graph.Count==0){
		min_sealevel=-1000;
		max_sealevel=1000;
	}
	
	max_sealevel=Math.Max(min_sealevel+500,max_sealevel+50);
	min_sealevel=min_sealevel-50;
	max_sealevel=Math.Ceiling(max_sealevel/100)*100;
	min_sealevel=Math.Floor(min_sealevel/100)*100;
	
	double sealevel_interval=(max_sealevel-min_sealevel)/(YLEN-1);
	double distance_interval=(max_distance-min_distance)/(XLEN-1);
	
	char[][] Graph=new char[Size.Y][];
	for(int y=0;y<Size.Y;y++){
		Graph[y]=new char[Size.X];
		double altitude=(min_sealevel+y*sealevel_interval);
		int alt_num=(int)Math.Floor(altitude/1000);
		int low_alt=(int)Math.Floor((altitude-sealevel_interval)/1000);
		char alt_10s=((Math.Abs(alt_num)/10)%10).ToString()[0];
		if(alt_num<0)
			alt_10s='-';
		char alt_1s=(Math.Abs(alt_num)%10).ToString()[0];
		for(int x=0;x<Size.X;x++){
			Graph[y][x]=' ';
			if(x<2&&y>=2){
				if(alt_num!=low_alt||(min_sealevel==0&&y==0)){
					if(x==0&&alt_10s!='0')
						Graph[y][x]=alt_10s;
					else if(x==1)
						Graph[y][x]=alt_1s;
				}
				else if(((int)Math.Floor(altitude/500))!=((int)Math.Floor((altitude-sealevel_interval)/500)))
					Graph[y][x]='-';
				else if(x==1&&((int)Math.Floor(altitude/250))!=((int)Math.Floor((altitude-sealevel_interval)/250)))
					Graph[y][x]='-';
			}
			else if(x==2){
				Graph[y][x]='|';
			}
		}
	}
	for(int x=3;x<Size.X;x++)
		Graph[1][x]='-';
	for(int x=3;x<Size.X;x++){
		double distance=(min_distance+x*distance_interval);
		int alt_num=(int)Math.Floor(distance/1000);
		int low_alt=(int)Math.Floor((distance-distance_interval)/1000);
		char alt_10s=((Math.Abs(alt_num)/10)%10).ToString()[0];
		char alt_1s=(Math.Abs(alt_num)%10).ToString()[0];
		if(alt_num!=low_alt||(min_distance==0&&x==3)){
			Graph[1][x]='+';
			if(x>3&&alt_10s!='0')
				Graph[0][x-1]=alt_10s;
			Graph[0][x]=alt_1s;
			continue;
		}
		else if(((int)Math.Floor(distance/500))!=((int)Math.Floor((distance-distance_interval)/500)))
			Graph[0][x]='|';
	}
	
	if(Target_Altitude!=double.MinValue){
		int Y=(int)Math.Round((Target_Altitude-min_sealevel)/sealevel_interval,0);
		for(int x=0;x<Size.X;x++){
			if(x<3||x%2==0){
				if(x>=0&&x<Size.X&&Y>=0&&Y<YLEN)
					Graph[Y+2][x]='=';
			}
		}
		
	}
	
	foreach(Altitude_Distance Point in Altitude_Distance_Graph){
		int X=(int)Math.Round((Point.Distance-min_distance)/distance_interval,0);
		int Y=(int)Math.Round((Point.Sealevel-min_sealevel)/sealevel_interval,0);
		if(X>=0&&X<XLEN&&Y>=0&&Y<YLEN)
			Graph[Y+2][X+3]='○';
	}
	
	string text="";
	for(int y=Size.Y-1;y>=0;y--){
		if(y<Size.Y-1)
			text+='\n';
		for(int x=0;x<Size.X;x++){
			text+=Graph[y][x];
		}
	}
	Panel.Display.WriteText(text,false);
}

void PrintDistanceTime(CustomPanel Panel){
	if(Runtime.UpdateFrequency==UpdateFrequency.Update1&&cycle%10!=0)
		return;
	Vector2I Size=GetSize(Panel.Display);
	while(Panel.Display.FontSize>0.1&&Size.X<50&&Size.Y<40){
		float FontSize=Panel.Display.FontSize;
		FontSize=Math.Max(FontSize-0.1f,FontSize*.9f);
		Panel.Display.FontSize=FontSize;
		Size=GetSize(Panel.Display);
	}
	int XLEN=Size.X-3;
	
	double max=1000;
	double min=0;
	foreach(Distance_Time Data in Distance_Time_Graph){
		max=Math.Max(max,Data.Distance);
		min=Math.Min(min,Data.Distance);
	}
	max=Math.Max(min+500,max+50);
	min=min-50;
	max=Math.Ceiling(max/100)*100;
	min=Math.Floor(min/100)*100;
	
	double interval=(max-min)/(Size.X-1);
	
	char[][] Graph=new char[Size.Y][];
	for(int y=0;y<Size.Y;y++){
		Graph[y]=new char[Size.X];
		for(int x=0;x<Size.X;x++)
			Graph[y][x]=' ';
	}
	for(int x=0;x<Size.X;x++){
		double distance=(min+x*interval);
		int alt_num=(int)Math.Floor(distance/1000);
		int low_alt=(int)Math.Floor((distance-interval)/1000);
		char alt_10s=((Math.Abs(alt_num)/10)%10).ToString()[0];
		char alt_1s=(Math.Abs(alt_num)%10).ToString()[0];
		Graph[1][x]='-';
		if(alt_num!=low_alt||(min==0&&x==0)){
			if(x>0&&alt_10s!='0')
				Graph[0][x-1]=alt_10s;
			Graph[0][x]=alt_1s;
			Graph[1][x]='+';
		}
		else if(((int)Math.Floor(distance/500))!=((int)Math.Floor((distance-interval)/500)))
			Graph[0][x]='|';
	}
	
	double time_interval=Graph_Length_Seconds/((double)Size.Y);
	double End=Time_Since_Start.TotalSeconds;
	double Start=End-Graph_Length_Seconds;
	
	List<Vector2I> Points=new List<Vector2I>();
	foreach(Distance_Time Point in Distance_Time_Graph){
		int X=(int)Math.Round((Point.Distance-min)/interval,0);
		int Y=(int)Math.Ceiling((Point.Timestamp.TotalSeconds-Start)/time_interval)+2;
		if(X>=0&&X<XLEN&&Y>=2&&Y<Size.Y)
			Points.Add(new Vector2I(X,Y));
	}
	
	for(int i=0;i<Points.Count;i++){
		Vector2I Point=Points[i];
		int X=Point.X;
		int Y=Point.Y;
		int min_ship=X;
		if(i>0)
			min_ship=Math.Min(min_ship,Points[i-1].X);
		if(i+1<Points.Count)
			min_ship=Math.Min(min_ship,Points[i+1].X);
		
		
		if(X-min_ship>1){
			for(int j=min_ship+1;j<X;j++)
				Graph[Y][j]='•';
		}
		
		
		if(X>=0&&X<Size.X)
			Graph[Y][X]='○';
	}
	
	string time=Math.Round(Graph_Timer,3).ToString();
	for(int i=1;i<=time.Length;i++)
		Graph[Size.Y-1][Size.X-i]=time[time.Length-i];
	
	string text="";
	for(int y=Size.Y-1;y>=0;y--){
		if(y<Size.Y-1)
			text+='\n';
		for(int x=0;x<Size.X;x++){
			text+=Graph[y][x];
		}
	}
	Panel.Display.WriteText(text,false);
}

void UpdateSystemData(){
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
					List<IMyLandingGear> AllBlocks=new List<IMyLandingGear>();
					GridTerminalSystem.GetBlocksOfType<IMyLandingGear>(AllBlocks);
					foreach(IMyLandingGear Block in AllBlocks)
						Elevation=Math.Min(Elevation,(Block.GetPosition()-PlanetCenter).Length()-terrain_height);
				}
			}
			else
				Elevation=Sealevel;
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
		if(Current_Display==3)
			PrintAltitudeTerrain(new CustomPanel(Me.GetSurface(0)));
		else if(Current_Display==4)
			PrintAltitudeDistance(new CustomPanel(Me.GetSurface(0)));
		else if(Current_Display==5)
			PrintDistanceTime(new CustomPanel(Me.GetSurface(0)));
		else
			PrintNotifications();
	}
	catch(Exception E){
		try{
			if(AltitudeTerrainLCDs!=null){
				foreach(CustomPanel Panel in AltitudeTerrainLCDs){
					Panel.Display.BackgroundColor=new Color(255,0,0);
					Panel.Display.FontColor=new Color(0,0,0);
				}
			}
			if(AltitudeDistanceLCDs!=null){
				foreach(CustomPanel Panel in AltitudeDistanceLCDs){
					Panel.Display.BackgroundColor=new Color(255,0,0);
					Panel.Display.FontColor=new Color(0,0,0);
				}
			}
			if(DistanceTimeLCDs!=null){
				foreach(CustomPanel Panel in DistanceTimeLCDs){
					Panel.Display.BackgroundColor=new Color(255,0,0);
					Panel.Display.FontColor=new Color(0,0,0);
				}
			}
		}
		finally{
			Write(E.ToString());
			FactoryReset();
		}
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
	if(MovementProgram==null)
		return false;
	return MovementProgram.TryRun(task.ToString());
}

//Tells the ship to orient to a specific up direction
bool Task_Up(Task task){
	if(MovementProgram==null)
		return false;
	return MovementProgram.TryRun(task.ToString());
}

//Tells the ship to fly to a specific location
Vector3D Last_Target=new Vector3D(0,0,0);
bool Task_Go(Task task){
	if(MovementProgram==null)
		return false;
	Vector3D position=new Vector3D(0,0,0);
	if(Vector3D.TryParse(task.Qualifiers.Last(),out position)){
		Vector3D Target_Position=position;
		if((Target_Position-Last_Target).Length()>400){
			Altitude_Distance_Graph.Clear();
			Distance_Time_Graph.Clear();
		}
		Last_Target=Target_Position;
		Target_Distance=(Target_Position-Controller.GetPosition()).Length();
		if(Gravity.Length()>0){
			double sealevel_radius=(Controller.GetPosition()-PlanetCenter).Length()-Sealevel;
			double target_radius=(Target_Position-PlanetCenter).Length();
			Target_Altitude=target_radius-sealevel_radius;
		}
		return MovementProgram.TryRun(task.ToString());
	}
	return false;
}

//Tells the ship to match position
bool Task_Match(Task task){
	if(MovementProgram==null)
		return false;
	return MovementProgram.TryRun(task.ToString());
}

//Sets Autoland either on or off
bool Task_Autoland(Task task){
	if(MovementProgram==null)
		return false;
	return MovementProgram.TryRun(task.ToString());
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
	Target_Altitude=double.MinValue;
	Target_Distance=double.MinValue;
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
		if(Gravity.Length()>0)
			Display(1,"Gravity:"+Math.Round(Gravity.Length()/9.814,2)+"Gs");
		Display(2,"Maximum Power (Hovering): "+Math.Round(Up_Gs,2)+"Gs");
		Display(2,"Maximum Power (Launching): "+Math.Round(Math.Max(Up_Gs,Forward_Gs),2)+"Gs");
	}
	if(argument.ToLower().Equals("factory reset")){
		FactoryReset();
	}
	if(Graph_Timer<=0)
		MarkGraphs();
	
	try{
		foreach(CustomPanel Panel in AltitudeTerrainLCDs){
			PrintAltitudeTerrain(Panel);
			if(Panel.Trans){
				Panel.Display.FontColor=DEFAULT_BACKGROUND_COLOR;
				Panel.Display.BackgroundColor=new Color(0,0,0,0);
			}
			else{
				Panel.Display.FontColor=DEFAULT_TEXT_COLOR;
				Panel.Display.BackgroundColor=DEFAULT_BACKGROUND_COLOR;
			}
		}
	} catch(Exception e){
		Notifications.Add(new Notification("Exception in AltitudeTerrainLCDs:\n"+e.Message,10));
		foreach(CustomPanel Panel in AltitudeTerrainLCDs){
			Panel.Display.BackgroundColor=new Color(255,255,0);
		}
	}
	try{
		foreach(CustomPanel Panel in AltitudeDistanceLCDs){
			PrintAltitudeDistance(Panel);
			if(Panel.Trans){
				Panel.Display.FontColor=DEFAULT_BACKGROUND_COLOR;
				Panel.Display.BackgroundColor=new Color(0,0,0,0);
			}
			else{
				Panel.Display.FontColor=DEFAULT_TEXT_COLOR;
				Panel.Display.BackgroundColor=DEFAULT_BACKGROUND_COLOR;
			}
		}
	} catch(Exception e){
		Notifications.Add(new Notification("Exception in AltitudeDistanceLCDs:\n"+e.Message,10));
		foreach(CustomPanel Panel in AltitudeDistanceLCDs){
			Panel.Display.BackgroundColor=new Color(255,255,0);
		}
	}
	try{
		foreach(CustomPanel Panel in DistanceTimeLCDs){
			PrintDistanceTime(Panel);
			if(Panel.Trans){
				Panel.Display.FontColor=DEFAULT_BACKGROUND_COLOR;
				Panel.Display.BackgroundColor=new Color(0,0,0,0);
			}
			else{
				Panel.Display.FontColor=DEFAULT_TEXT_COLOR;
				Panel.Display.BackgroundColor=DEFAULT_BACKGROUND_COLOR;
			}
		}
	} catch(Exception e){
		Notifications.Add(new Notification("Exception in DistanceTimeLCDs:\n"+e.Message,10));
		foreach(CustomPanel Panel in DistanceTimeLCDs){
			Panel.Display.BackgroundColor=new Color(255,255,0);
		}
	}
	
	Runtime.UpdateFrequency=GetUpdateFrequency();
}