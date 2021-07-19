/*
* Saturnian Navigation OS
* Built by mlq1616
* https://github.com/mlq1819/Space-Engineers-Saturnian-Suite
* This suite handles navigation, major autopiloting, etc. 
* Include "Altitude" in LCD name to add to group.
*/
string Program_Name="Saturnian Navigation";
Color DEFAULT_TEXT_COLOR=new Color(197,137,255,255);
Color DEFAULT_BACKGROUND_COLOR=new Color(44,0,88,255);
double Acceptable_Angle=5;
int Graph_Length_Seconds=180;

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
	if(new_line)
		Me.GetSurface(0).WriteText(text+'\n', append);
	else
		Me.GetSurface(0).WriteText(text, append);
}

int Display_Count=3;
int Current_Display=1;
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

struct Altitude_Data{
	public TimeSpan Timestamp;
	public double Sealevel;
	public double Elevation;
	
	public Altitude_Data(double S,double E,TimeSpan start){
		Sealevel=S;
		Elevation=E;
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

List<CustomPanel> AltitudeLCDs;

Queue<Altitude_Data> Altitude_Graph;
double Altitude_Timer=0;

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

double Forward_Gs{
	get{
		return Forward_Thrust/Controller.CalculateShipMass().TotalMass/9.81;
	}
}
double Backward_Gs{
	get{
		return Backward_Thrust/Controller.CalculateShipMass().TotalMass/9.81;
	}
}
double Up_Gs{
	get{
		return Up_Thrust/Controller.CalculateShipMass().TotalMass/9.81;
	}
}
double Down_Gs{
	get{
		return Down_Thrust/Controller.CalculateShipMass().TotalMass/9.81;
	}
}
double Left_Gs{
	get{
		return Left_Thrust/Controller.CalculateShipMass().TotalMass/9.81;
	}
}
double Right_Gs{
	get{
		return Right_Thrust/Controller.CalculateShipMass().TotalMass/9.81;
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
	AltitudeLCDs=new List<CustomPanel>();
	Altitude_Graph=new Queue<Altitude_Data>();
	Notifications=new List<Notification>();
}

double MySize=0;
bool Setup(){
	Reset();
	List<IMyTextPanel> LCDs=GenericMethods<IMyTextPanel>.GetAllConstruct("Altitude");
	foreach(IMyTextPanel Panel in LCDs)
		AltitudeLCDs.Add(new CustomPanel(Panel));
	foreach(CustomPanel Panel in AltitudeLCDs){
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

bool _Autoland=false;
bool Autoland(){
	if(!Safety)
		return false;
	_Autoland=!_Autoland;
	return true;
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
	if(Altitude_Timer>0)
		Altitude_Timer-=seconds_since_last_update;
	Write("Display "+Current_Display.ToString()+"/"+Display_Count.ToString());
	UpdateMyDisplay();
	Echo(ToString(FromSeconds(seconds_since_last_update))+" since last cycle");
	Time_Since_Start=UpdateTimeSpan(Time_Since_Start,seconds_since_last_update);
	Echo(ToString(Time_Since_Start)+" since last reboot\n");
	Me.GetSurface(1).WriteText("\n"+ToString(Time_Since_Start)+" since last reboot",true);
}

void UpdateMyDisplay(){
	IMyTextSurface Display=Me.GetSurface(0);
	if(Current_Display==3){
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

string Graph_Text="";

void MarkAltitude(bool do_new=true){
	const int XLEN=48;
	const int XFULL=51;
	if(Gravity.Length()==0)
		return;
	if(do_new)
		Altitude_Timer=((double)Graph_Length_Seconds)/XFULL;
	while(Altitude_Graph.Count>0&&Time_Since_Start.TotalSeconds-Altitude_Graph.Peek().Timestamp.TotalSeconds>Graph_Length_Seconds)
		Altitude_Graph.Dequeue();
	if(do_new&&Altitude_Graph.Count<XLEN&&Gravity.Length()>0)
		Altitude_Graph.Enqueue(new Altitude_Data(Sealevel,Elevation,Time_Since_Start));
	if(Altitude_Graph.Count==0)
		return;
	double max=500;
	double min=double.MaxValue;
	foreach(Altitude_Data Data in Altitude_Graph){
		max=Math.Max(max,Data.Sealevel-Data.Elevation);
		min=Math.Min(min,Data.Sealevel-Data.Elevation);
	}
	max+=100;
	min=Math.Max(min-100,-1100);
	max=Math.Ceiling(max/100)*100;
	min=Math.Floor(min/100)*100;
	double interval=(max-min)/34.0;
	//50 wide, 30 tall
	char[][] Graph=new char[35][];
	for(int y=0;y<35;y++){
		Graph[y]=new char[XFULL];
		double altitude=(min+y*interval);
		int alt_num=(int)Math.Floor(altitude/1000);
		int low_alt=(int)Math.Floor((altitude-interval)/1000);
		char alt_10s=((Math.Abs(alt_num)/10)%10).ToString()[0];
		if(alt_num<0)
			alt_10s='-';
		char alt_1s=(Math.Abs(alt_num)%10).ToString()[0];
		for(int x=0;x<XFULL;x++){
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
	
	// if(Orbiting){
		// double Orbital_Altitude=0;
		
		// int Y=(int)Math.Round((Orbital_Altitude-min)/interval,0);
		// if(Y>=0&&Y<35){
			// for(int X=3;X<XFULL;X++){
				// if(X%2==0)
					// Graph[Y][X]='=';
			// }
		// }
	// }
	
	double time_interval=Graph_Length_Seconds/((double)XLEN);
	double End=Time_Since_Start.TotalSeconds;
	double Start=End-Graph_Length_Seconds;
	
	foreach(Altitude_Data Point in Altitude_Graph){
		int X=(int)Math.Ceiling((Point.Timestamp.TotalSeconds-Start)/time_interval);
		int Y_E=(int)Math.Round((Point.Sealevel-Point.Elevation)/interval,0);
		int Y_S=(int)Math.Round((Point.Sealevel-min)/interval,0);
		if(X>=0&&X<XLEN){
			if(Y_S>=0&&Y_S<35)
				Graph[Y_S][X+3]='○';
			if(Y_E>=0&&Y_E<35)
				Graph[Y_E][X+3]='_';
		}
	}
	if(do_new){
		string time=Math.Round(Altitude_Timer,3).ToString();
		for(int i=1;i<=time.Length;i++)
			Graph[34][XFULL-i]=time[time.Length-i];
	}
	string text="";
	for(int y=34;y>=0;y--){
		if(y<34)
			text+='\n';
		for(int x=0;x<XFULL;x++){
			text+=Graph[y][x];
		}
	}
	Graph_Text=text;
	foreach(CustomPanel Panel in AltitudeLCDs){
		Panel.Display.WriteText(text,false);
	}
	
}

bool Safety=true;
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
	
	Time_To_Crash=-1;
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
			if(!Me.CubeGrid.IsStatic){
				double from_center=(Controller.GetPosition()-PlanetCenter).Length();
				Vector3D next_position=Controller.GetPosition()+1*CurrentVelocity;
				double Elevation_per_second=(from_center-(next_position-PlanetCenter).Length());
				Time_To_Crash=(Elevation-MySize/2)/Elevation_per_second;
				bool need_print=true;
				if(_Autoland)
					Write("Autoland Enabled");
				if(Time_To_Crash>0){
					if(Safety&&Time_To_Crash<(5+CurrentSpeed/5)&&Controller.GetShipSpeed()>5){
						Controller.DampenersOverride=true;
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
						if(_Autoland&&(Time_To_Crash>30||(CurrentSpeed<=5&&CurrentSpeed>2.5&&Time_To_Crash>5)))
							Controller.DampenersOverride=false;
						need_print=false;
					}
					if(Elevation-MySize<5&&_Autoland)
						_Autoland=false;
				}
				if(need_print)
					Write("No crash likely at current velocity");
			}
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
			for(int i=0;i<Notifications.Count;i++){
				Notifications[i].Time=Math.Max(0,Notifications[i].Time-seconds_since_last_update);
				Write("\""+Notifications[i].Text+"\"");
				if(Notifications[i].Time<=0){
					Notifications.RemoveAt(i--);
					continue;
				}
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
			Me.GetSurface(0).WriteText(Graph_Text,false);
		else
			PrintNotifications();
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
	//
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
	if(argument.ToLower().Equals("autoland")){
		Autoland();
	}
	else if(argument.ToLower().Equals("factory reset")){
		FactoryReset();
	}
	if(Altitude_Timer<=0)
		MarkAltitude();
	
	Runtime.UpdateFrequency=GetUpdateFrequency();
}