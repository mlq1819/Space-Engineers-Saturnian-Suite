/*
* Saturnian Maneuvering OS
* Built by mlq1616
* https://github.com/mlq1819/Space-Engineers-Saturnian-Suite
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
	
	public static double GetAngle(Vector3D v1, Vector3D v2){
		v1.Normalize();
		v2.Normalize();
		return Math.Round(Math.Acos(v1.X*v2.X + v1.Y*v2.Y + v1.Z*v2.Z)*57.295755,5);
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
	return UpdateFrequency.Update1;
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
}

void Reset(){
	Operational=false;
	Runtime.UpdateFrequency=UpdateFrequency.None;
	Controller=null;
	if(Gyroscope!=null)
		Gyroscope.GyroOverride=false;
	Gyroscope=null;
	for(int i=0;i<All_Thrusters.Length;i++){
		if(All_Thrusters[i]!=null){
			for(int j=0;j<All_Thrusters[i].Count;j++){
				if(All_Thrusters[i][j]!=null)
					ResetThruster(All_Thrusters[i][j]);
			}
		}
		All_Thrusters[i]=new List<IMyThrust>();
	}
	RestingSpeed=0;
}

double MySize=0;
bool Setup(){
	Reset();
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
	
	List<IMyTerminalBlock> AllTerminalBlocks=new List<IMyTerminalBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(AllTerminalBlocks);
	MySize=0;
	foreach(IMyTerminalBlock Block in AllTerminalBlocks){
		double distance=(Controller.GetPosition()-Block.GetPosition()).Length();
		MySize=Math.Max(MySize,distance);
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
	}
	Me.GetSurface(1).FontSize=2.2f;
	Me.GetSurface(1).TextPadding=40.0f;
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
	bool ship=!Me.CubeGrid.IsStatic;
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
	if(Math.Abs(input_pitch)<0.05f){
		input_pitch=current_pitch*0.99f;
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
	else{
		Pitch_Time=0;
		input_pitch*=30;
	}
	foreach(IMyShipController Ctrl in Controllers)
		input_yaw+=Math.Min(Math.Max(Ctrl.RotationIndicator.Y/100,-1),1);
	if(Math.Abs(input_yaw)<0.05f){
		input_yaw=current_yaw*0.99f;
	}
	else{
		Yaw_Time=0;
		input_yaw*=30;
	}
	foreach(IMyShipController Ctrl in Controllers)
		input_roll+=Ctrl.RollIndicator;
	if(Math.Abs(input_roll)<0.05f){
		input_roll=current_roll*0.99f;
		if(Safety&&Gravity.Length()>0&&Roll_Time>=1){
			double difference=GetAngle(Left_Vector,Gravity)-GetAngle(Right_Vector,Gravity);
			if(Math.Abs(difference)>Acceptable_Angle){
				input_roll-=(float)Math.Min(Math.Max(difference*5,-5),25)*gyro_multx*5;
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

bool Safety=true;
void SetThrusters(){
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
	}
	if(Controller.DampenersOverride){
		Write("Cruise Control: Off");
		Write("Dampeners: On");
		input_right-=(float)((Relative_CurrentVelocity.X-Relative_RestingVelocity.X)*Mass_Accomodation*damp_multx);
		input_up-=(float)((Relative_CurrentVelocity.Y-Relative_RestingVelocity.Y)*Mass_Accomodation*damp_multx);
		input_forward+=(float)((Relative_CurrentVelocity.Z-Relative_RestingVelocity.Z)*Mass_Accomodation*damp_multx);
	}
	else{
		if(Elevation>50||CurrentVelocity.Length()>10){
			Write("Cruise Control: On");
			Vector3D velocity_direction=CurrentVelocity;
			velocity_direction.Normalize();
			double angle=Math.Min(GetAngle(Forward_Vector, velocity_direction),GetAngle(Backward_Vector, velocity_direction));
			if(angle<=Acceptable_Angle/2){
				input_right-=(float)((Relative_CurrentVelocity.X-Relative_RestingVelocity.X)*Mass_Accomodation*damp_multx);
				input_up-=(float)((Relative_CurrentVelocity.Y-Relative_RestingVelocity.Y)*Mass_Accomodation*damp_multx);
				Write("Stabilizers: On ("+Math.Round(angle, 1)+"° dev)");
			}
			else
				Write("Stabilizers: Off ("+Math.Round(angle, 1)+"° dev)");
		}
		else{
			Write("Cruise Control: Off");
			Write("Dampeners: Off");
		}
	}
	
	effective_speed_limit=Math.Max(effective_speed_limit,5);
	if(!Safety)
		effective_speed_limit=300000000;
	
	Write("Effective Speed Limit:"+Math.Round(effective_speed_limit,1)+"mps");
	
	
	if(RestingSpeed==0&&Controller.DampenersOverride&&(Speed_Deviation+5)<effective_speed_limit){
		for(int i=0;i<All_Thrusters.Length;i++){
			foreach(IMyThrust Thruster in All_Thrusters[i])
				Thruster.ThrustOverride=0;
		}
		return;
	}
	
	if(Gravity.Length()>0&&Mass_Accomodation>0&&(Controller.GetShipSpeed()<100||GetAngle(CurrentVelocity,Gravity)>Acceptable_Angle)){
		if(!(_Autoland&&Time_To_Crash>15&&CurrentSpeed>5)){
			if(Elevation>Ev_Df&&CurrentSpeed>1){
				input_right-=(float)Adjusted_Gravity.X;
				input_up-=(float)Adjusted_Gravity.Y;
				input_forward+=(float)Adjusted_Gravity.Z;
			}
		}
	}
	
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
}

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
	Echo(Program_Name+" OS Cycle-"+cycle.ToString()+" ("+loading_char+")");
	Me.GetSurface(1).WriteText(Program_Name+" OS Cycle-"+cycle.ToString()+" ("+loading_char+")",false);
	seconds_since_last_update=Runtime.TimeSinceLastRun.TotalSeconds + (Runtime.LastRunTimeMs / 1000);
	Echo(ToString(FromSeconds(seconds_since_last_update))+" since last cycle");
	Time_Since_Start=UpdateTimeSpan(Time_Since_Start,seconds_since_last_update);
	Echo(ToString(Time_Since_Start)+" since last reboot\n");
	Me.GetSurface(1).WriteText("\n"+ToString(Time_Since_Start)+" since last reboot",true);
}

void UpdateSystemData(){
	Write("", false, false);
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
				Time_To_Crash=Elevation/Elevation_per_second;
				bool need_print=true;
				if(Time_To_Crash>0){
					if(Safety&&Time_To_Crash<15&&Controller.GetShipSpeed()>5){
						Controller.DampenersOverride=true;
						RestingSpeed=0;
						Write("Crash predicted within 15 seconds; enabling Dampeners");
						need_print=false;
					}
					else if(Time_To_Crash*Math.Max(Elevation,1000)<1800000&&Controller.GetShipSpeed()>1.0f){
						Write(Math.Round(Time_To_Crash,1).ToString()+" seconds to crash");
						if(_Autoland&&Time_To_Crash>30)
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

public void Main(string argument, UpdateType updateSource)
{
	try{
		UpdateProgramInfo();
		UpdateSystemData();
		if(!Me.CubeGrid.IsStatic){
			if(Elevation!=double.MaxValue){
				Write("Elevation: "+Math.Round(Elevation,1).ToString());
				Write("Sealevel: "+Math.Round(Sealevel,1).ToString());
			}
			if(Gravity.Length()>0)
				Write("Gravity:"+Math.Round(Gravity.Length()/9.814,2)+"Gs");
			Write("Maximum Power (Hovering): "+Math.Round(Up_Gs,2)+"Gs");
			Write("Maximum Power (Launching): "+Math.Round(Math.Max(Up_Gs,Forward_Gs),2)+"Gs");
		}
		if(argument.ToLower().Equals("autoland")){
			Autoland();
		}
		else if(argument.ToLower().Equals("factory reset")){
			FactoryReset();
		}
		if(_Autoland)
			Write("Autoland Enabled");
		
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
		Runtime.UpdateFrequency=GetUpdateFrequency();
	}
	catch(Exception E){
		Write(E.ToString());
		FactoryReset();
	}
}
