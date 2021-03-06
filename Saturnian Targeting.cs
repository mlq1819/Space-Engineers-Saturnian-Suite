/*
* Saturnian Targeting OS
* Built by mlq1616
* https://github.com/mlq1819/Space-Engineers-Saturnian-Suite
* This suite handles rotor-turrets, turret controlling, etc.
* Include "Targeting" in LCD name to add to group.
* Rotor Turrets must be built such that the first Motor controls Left-Right, and the second controls Up-Down. They must also be assumed to default to 0° for both motors. The second Motor can be a Hinge. Rotor Turret must also have a Remote Control and a forward-facing Camera.
* When Aiming and Firing, any Red Lights on the RotorTurret will be turned on; these same lights are turned off when resetting.

TODO:
- MotorTurret Aiming
- User controls all turrets at once
- 

*/
string Program_Name="Saturnian Targeting";
Color DEFAULT_TEXT_COLOR=new Color(197,137,255,255);
Color DEFAULT_BACKGROUND_COLOR=new Color(44,0,88,255);

class Prog{
	public static MyGridProgram P;
	public static TimeSpan FromSeconds(double seconds){
		return (new TimeSpan(0,0,0,(int)seconds,(int)(seconds*1000)%1000));
	}

	public static TimeSpan UpdateTimeSpan(TimeSpan old,double seconds){
		return old+FromSeconds(seconds);
	}
	public static string GetRemovedString(string big_string, string small_string){
		string output=big_string;
		if(big_string.Contains(small_string)){
			output=big_string.Substring(0, big_string.IndexOf(small_string))+big_string.Substring(big_string.IndexOf(small_string)+small_string.Length);
		}
		return output;
	}
	public static Vector3D GlobalToLocal(Vector3D Global,IMyCubeBlock Ref){
		Vector3D Local=Vector3D.Transform(Global+Ref.GetPosition(), MatrixD.Invert(Ref.WorldMatrix));
		Local.Normalize();
		return Local*Global.Length();
	}
	public static Vector3D LocalToGlobal(Vector3D Local,IMyCubeBlock Ref){
	Vector3D Global=Vector3D.Transform(Local, Ref.WorldMatrix)-Ref.GetPosition();
	Global.Normalize();
	return Global*Local.Length();
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
		List<T> input=GetAllIncluding(name,Ref,mx_d);
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

int Display_Count{
	get{
		return TurretTypes+1;
	}
}
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

struct Angle{
	private float _Degrees;
	public float Degrees{
		get{
			return _Degrees;
		}
		set{
			_Degrees=value%360;
			while(_Degrees<0)
				_Degrees+=360;
		}
	}
	
	public Angle(float degrees){
		_Degrees=degrees;
		Degrees=degrees;
	}
	
	public static Angle FromRadians(float Rads){
		return new Angle((float)(Rads/Math.PI*180));
	}
	
	public float Difference_From_Top(Angle other){
		if(other.Degrees>=Degrees)
			return other.Degrees-Degrees;
		return other.Degrees-Degrees+360;
	}
	
	public float Difference_From_Bottom(Angle other){
		if(other.Degrees<=Degrees)
			return Degrees-other.Degrees;
		return Degrees-other.Degrees+360;
	}
	
	public float Difference(Angle other){
		return Math.Min(Difference_From_Top(other),Difference_From_Bottom(other));
	}
	
	public static bool IsBetween(Angle Bottom, Angle Middle, Angle Top){
		return Bottom.Difference_From_Top(Middle)<=Bottom.Difference_From_Top(Top);
	}
	
	public static bool TryParse(string parse,out Angle output){
		output=new Angle(0);
		float d;
		if(!float.TryParse(parse.Substring(0,Math.Max(0,parse.Length-1)),out d))
			return false;
		output=new Angle(d);
		return true;
	}
	
	public static Angle operator +(Angle a1, Angle a2){
		return new Angle(a1.Degrees+a2.Degrees);
	}
	
	public static Angle operator +(Angle a1, float a2){
		return new Angle(a1.Degrees+a2);
	}
	
	public static Angle operator +(float a1, Angle a2){
		return a2 + a1;
	}
	
	public static Angle operator -(Angle a1, Angle a2){
		return new Angle(a1.Degrees-a2.Degrees);
	}
	
	public static Angle operator -(Angle a1, float a2){
		return new Angle(a1.Degrees-a2);
	}
	
	public static Angle operator -(float a1, Angle a2){
		return new Angle(a1-a2.Degrees);
	}
	
	public static Angle operator *(Angle a1, float m){
		return new Angle(a1.Degrees*m);
	}
	
	public static Angle operator *(float m, Angle a2){
		return a2*m;
	}
	
	public static Angle operator /(Angle a1, float m){
		return new Angle(a1.Degrees/m);
	}
	
	public static bool operator ==(Angle a1, Angle a2){
		float degrees=(a1-a2).Degrees;
		if(degrees>180)
			degrees-=360;
		return Math.Abs(degrees)<0.000001f;
	}
	
	public static bool operator !=(Angle a1, Angle a2){
		return Math.Abs(a1.Degrees-a2.Degrees)>=0.000001f;
	}
	
	public override bool Equals(object o){
		return (o.GetType()==this.GetType()) && this==((Angle)o);
	}
	
	public override int GetHashCode(){
		return Degrees.GetHashCode();
	}
	
	public static bool operator >(Angle a1, Angle a2){
		return a1.Difference_From_Top(a2)>a1.Difference_From_Bottom(a2);
	}
	
	public static bool operator >=(Angle a1, Angle a2){
		return (a1==a2)||(a1>a2);
	}
	
	public static bool operator <=(Angle a1, Angle a2){
		return (a1==a2)||(a1<a2);
	}
	
	public static bool operator <(Angle a1, Angle a2){
		return a1.Difference_From_Top(a2)<a1.Difference_From_Bottom(a2);
	}
	
	public override string ToString(){
		if(Degrees>=180)
			return (Degrees-360).ToString()+'°';
		else
			return Degrees.ToString()+'°';
	}
	
	public string ToString(int n){
		n=Math.Min(0,n);
		if(Degrees>=180)
			return Math.Round(Degrees-360,n).ToString()+'°';
		else
			return Math.Round(Degrees,n).ToString()+'°';
	}
	
	public static string LinedFloat(float deg, int sections=10){
		string output="[";
		for(int i=-180/sections;i<0;i++){
			if(deg<0){
				if(deg/sections<=i+1)
					output+='|';
				else
					output+=' ';
			}
			else
				output+=' ';
		}
		for(int i=0;i<180/sections;i++){
			if(deg<0)
				output+=' ';
			else{
				if(deg/sections>=(i+1))
					output+='|';
				else
					output+=' ';
			}
		}
		output+=']';
		return output;
	}
	
	public string LinedString(int sections=10){
		if(Degrees>=180)
			return LinedFloat(Degrees-360,sections);
		return LinedFloat(Degrees,sections);
	}
	
	public string LinedString(Angle Comparison){
		string output="Actual: "+LinedString();
		output+="\nComparison: ";
		float deg=Difference(Comparison);
		if(deg>=180)
			output+=LinedFloat(deg-360);
		else
			output+=LinedFloat(deg);
		return output;
	}
}


TimeSpan Time_Since_Start=new TimeSpan(0);
long cycle=0;
char loading_char='|';
double seconds_since_last_update=0;
Random Rnd;

IMyShipController Controller;
List<IMyShipController> Controllers;

List<CustomPanel> WeaponLCDs;

List<IMyLargeTurretBase> AllTurrets{
	get{
		List<IMyLargeTurretBase> Output=new List<IMyLargeTurretBase>();
		foreach(IMyLargeGatlingTurret T in GatlingTurrets)
			Output.Add((IMyLargeTurretBase)T);
		foreach(IMyLargeMissileTurret T in MissileTurrets)
			Output.Add((IMyLargeTurretBase)T);
		foreach(IMyLargeInteriorTurret T in InteriorTurrets)
			Output.Add((IMyLargeTurretBase)T);
		return Output;
	}
}
List<IMyLargeGatlingTurret> GatlingTurrets;
List<IMyLargeMissileTurret> MissileTurrets;
List<IMyLargeInteriorTurret> InteriorTurrets;
List<RotorTurret> RotorTurrets;
int TurretTypes{
	get{
		int output=0;
		if(GatlingTurrets.Count>0)
			output++;
		if(MissileTurrets.Count>0)
			output++;
		if(InteriorTurrets.Count>0)
			output++;
		if(RotorTurrets.Count>0)
			output++;
		return output;
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

bool MainControllerFunction(IMyShipController ctr){
	return ctr.IsMainCockpit&&ControllerFunction(ctr);
}
bool ControllerFunction(IMyShipController ctr){
	return ctr.IsSameConstructAs(Me)&&ctr.CanControlShip&&ctr.ControlThrusters;
}

UpdateFrequency GetUpdateFrequency(){
	if(RotorTurrets.Count>0)
		return UpdateFrequency.Update1;
	return UpdateFrequency.Update10;
}

void Turret_Setup(){
	GatlingTurrets=GenericMethods<IMyLargeGatlingTurret>.GetAllIncluding("");
	MissileTurrets=GenericMethods<IMyLargeMissileTurret>.GetAllIncluding("");
	InteriorTurrets=GenericMethods<IMyLargeInteriorTurret>.GetAllIncluding("");
	RotorTurrets=new List<RotorTurret>();
	List<IMyMotorStator> AllMotors=GenericMethods<IMyMotorStator>.GetAllGrid("",Me.CubeGrid);
	foreach(IMyMotorStator Motor in AllMotors){
		GyroTurret G;
		if(GyroTurret.TryGet(Motor,out G))
			RotorTurrets.Add(G);
		else{
			MotorTurret M;
			if(MotorTurret.TryGet(Motor,out M))
				RotorTurrets.Add(M);
		}
	}
}

void Reset(){
	Operational=false;
	Runtime.UpdateFrequency=UpdateFrequency.None;
	Controller=null;
	Controllers=new List<IMyShipController>();
	WeaponLCDs=new List<CustomPanel>();
	GatlingTurrets=new List<IMyLargeGatlingTurret>();
	MissileTurrets=new List<IMyLargeMissileTurret>();
	InteriorTurrets=new List<IMyLargeInteriorTurret>();
	RotorTurrets=new List<RotorTurret>();
	Notifications=new List<Notification>();
}

bool Setup(){
	Reset();
	List<IMyTextPanel> LCDs=GenericMethods<IMyTextPanel>.GetAllConstruct("Targeting");
	foreach(IMyTextPanel Panel in LCDs)
		WeaponLCDs.Add(new CustomPanel(Panel));
	foreach(CustomPanel Panel in WeaponLCDs){
		if(Panel.Trans){
			Panel.Display.FontColor=DEFAULT_BACKGROUND_COLOR;
			Panel.Display.BackgroundColor=new Color(0,0,0,0);
		}
		else{
			Panel.Display.FontColor=DEFAULT_TEXT_COLOR;
			Panel.Display.BackgroundColor=DEFAULT_BACKGROUND_COLOR;
		}
		Panel.Display.Font="DEBUG";
		Panel.Display.Alignment=TextAlignment.CENTER;
		Panel.Display.ContentType=ContentType.TEXT_AND_IMAGE;
		Panel.Display.TextPadding=0;
		Panel.Display.FontSize=2.2f;
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
	Turret_Setup();
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
	Write("Display "+Current_Display.ToString()+"/"+Display_Count.ToString());
	Echo(ToString(FromSeconds(seconds_since_last_update))+" since last cycle");
	Time_Since_Start=UpdateTimeSpan(Time_Since_Start,seconds_since_last_update);
	Echo(ToString(Time_Since_Start)+" since last reboot\n");
	Me.GetSurface(1).WriteText("\n"+ToString(Time_Since_Start)+" since last reboot",true);
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

class VelocityTuple{
	public Vector3D Velocity;
	public double Timer;
	
	public VelocityTuple(Vector3D v,double t=0){
		Velocity=v;
		Timer=t;
	}
}

enum RTStatus{
	Init0=0,
	InitYaw=1,
	Init1=2,
	InitPitch=3,
	Unlinked=4,
	Linked=5
}
abstract class RotorTurret{
	public List<IMySmallGatlingGun> Guns;
	public string Name{
		get{
			return YawMotor.CustomName;
		}
	}
	public string DisplayName{
		get{
			string name=Name;
			if(name.Contains("Advanced"))
				name=Prog.GetRemovedString(name,"Advanced");
			if(name.Contains("Rotor"))
				return Prog.GetRemovedString(name,"Rotor").Trim();
			else if(name.Contains("Hinge"))
				return Prog.GetRemovedString(name,"Hinge").Trim();
			return name.Trim();
		}
	}
	protected IMyMotorStator YawMotor;
	protected IMyMotorStator PitchMotor;
	protected bool PitchParity=true; //True means positive angles are up, false means positive angles are down
	public IMyRemoteControl Remote;
	public IMyCameraBlock Camera;
	private double Default_Yaw=0;
	private double Default_Pitch=0;
	public double TurretAngle{
		get{
			return GetAngle(Forward_Vector,Default_Vector);
		}
	}
	public double Yaw{
		get{
			double yaw=(YawMotor.Angle*180/Math.PI)-Default_Yaw;
			yaw=(yaw+180)%360-180;
			return yaw;
		}
	}
	public double Pitch{
		get{
			double pitch=(PitchMotor.Angle*180/Math.PI)-Default_Pitch;
			pitch=(pitch+180)%360-180;
			return pitch;
		}
	}
	private RTStatus _Status;
	public RTStatus Status{
		get{
			return _Status;
		}
		set{
			if(value==RTStatus.Unlinked&&Turret!=null)
				_Status=RTStatus.Linked;
			else
				_Status=value;
		}
	}
	public IMyLargeTurretBase Turret=null;
	public List<IMyLightingBlock> Lights;
	
	public Vector3D Default_Vector{
		get{
			return Prog.LocalToGlobal(new Vector3D(0,0,1),YawMotor);
		}
	}
	public Vector3D Forward_Vector{
		get{
			return Prog.LocalToGlobal(new Vector3D(0,0,-1),Remote);
		}
	}
	public Vector3D Backward_Vector{
		get{
			return -1*Forward_Vector;
		}
	}
	public Vector3D Up_Vector{
		get{
			return Prog.LocalToGlobal(new Vector3D(0,1,0),Remote);
		}
	}
	public Vector3D Down_Vector{
		get{
			return -1*Up_Vector;
		}
	}
	public Vector3D Left_Vector{
		get{
			return Prog.LocalToGlobal(new Vector3D(-1,0,0),Remote);
		}
	}
	public Vector3D Right_Vector{
		get{
			return -1*Left_Vector;
		}
	}
	
	protected RotorTurret(IMyMotorStator yawmotor,IMyMotorStator pitchmotor,List<IMySmallGatlingGun> guns,IMyRemoteControl remote,IMyCameraBlock camera){
		YawMotor=yawmotor;
		YawMotor.LowerLimitDeg=Math.Min(YawMotor.LowerLimitDeg,0);
		YawMotor.UpperLimitDeg=Math.Max(YawMotor.UpperLimitDeg,0);
		PitchMotor=pitchmotor;
		Guns=guns;
		Remote=remote;
		Camera=camera;
		Camera.EnableRaycast=true;
		if(Remote.CustomData.Length>0)
			Turret=GenericMethods<IMyLargeTurretBase>.GetFull(Remote.CustomData);
		Lights=new List<IMyLightingBlock>();
		List<IMyLightingBlock> lights=GenericMethods<IMyLightingBlock>.GetAllGrid("",Remote.CubeGrid,Camera,double.MaxValue);
		foreach(IMyLightingBlock Light in lights){
			if(Light.Color.R>=225&&Light.Color.G<=150&&Light.Color.B<=150)
				Lights.Add(Light);
		}
	}
	
	public abstract bool Initialize();
	
	public abstract bool Aim(Vector3D Target,Vector3D Velocity,bool reset=false);
	
	protected double GunTimer=0;
	protected bool Increase_GunTimer=false;
	protected bool Decrease_GunTimer=true;
	public bool Aim(Vector3D Target){
		return Aim(Target,GetSpeed(Target));
	}
	
	public double CameraTimer{
		get{
			if(Camera.CustomData.Length==0)
				return 0;
			string[] args=Camera.CustomData.Split('\n');
			if(args.Length!=3)
				return 0;
			double output=0;
			double.TryParse(args[0],out output);
			return output;
		}
		set{
			Camera.CustomData=Math.Round(value,3).ToString()+'\n'+CameraClear.ToString()+'\n'+Math.Round(TargetTimer,3).ToString();
		}
	}
	public bool CameraClear{
		get{
			if(Camera.CustomData.Length==0)
				return false;
			string[] args=Camera.CustomData.Split('\n');
			if(args.Length!=3)
				return false;
			bool output=false;
			bool.TryParse(args[1],out output);
			return output;
		}
		set{
			Camera.CustomData=Math.Round(CameraTimer,3).ToString()+'\n'+value.ToString()+'\n'+Math.Round(TargetTimer,3).ToString();
		}
	}
	public double TargetTimer{
		get{
			if(Camera.CustomData.Length==0)
				return 0;
			string[] args=Camera.CustomData.Split('\n');
			if(args.Length!=3)
				return 0;
			double output=0;
			double.TryParse(args[2],out output);
			return output;
		}
		set{
			Camera.CustomData=Math.Round(CameraTimer,3).ToString()+'\n'+CameraClear.ToString()+'\n'+Math.Round(value,3).ToString();
		}
	}
	
	public bool ValidTarget(MyDetectedEntityInfo Entity){
		if(Entity.Type==MyDetectedEntityType.None)
			return true;
		if(Entity.EntityId==Camera.CubeGrid.EntityId)
			return true;
		if(Entity.Relationship==MyRelationsBetweenPlayerAndBlock.Enemies)
			return true;
		if(Entity.Relationship==MyRelationsBetweenPlayerAndBlock.Owner)
			return false;
		if(Entity.Relationship==MyRelationsBetweenPlayerAndBlock.Friends)
			return false;
		return true;
	}
	Vector3D Last_Velocity=new Vector3D(0,0,0);
	public Vector3D GetSpeed(Vector3D Target){
		if(TargetTimer>=1&&Camera.AvailableScanRange>=1000){
			MyDetectedEntityInfo Entity=Camera.Raycast(Target);
			if(Entity.Type!=MyDetectedEntityType.None&&ValidTarget(Entity)){
				Last_Velocity=Entity.Velocity;
			}
			else
				Last_Velocity=Last_Velocity*(2.0/3);
			TargetTimer=0;
		}
		return Last_Velocity;
	}
	public bool ClearVision(){
		double timer=CameraTimer;
		if(timer<1||Camera.AvailableScanRange<1000)
			return CameraClear;
		bool clear=true;
		clear=clear&&ValidTarget(Camera.Raycast(Camera.GetPosition()+5*Forward_Vector+2.5*Left_Vector));
		clear=clear&&ValidTarget(Camera.Raycast(Camera.GetPosition()+5*Forward_Vector+2.5*Right_Vector));
		clear=clear&&ValidTarget(Camera.Raycast(Camera.GetPosition()+15*Forward_Vector+2.5*Left_Vector));
		clear=clear&&ValidTarget(Camera.Raycast(Camera.GetPosition()+15*Forward_Vector+2.5*Right_Vector));
		clear=clear&&ValidTarget(Camera.Raycast(Camera.GetPosition()+50*Forward_Vector+2.5*Left_Vector));
		clear=clear&&ValidTarget(Camera.Raycast(Camera.GetPosition()+50*Forward_Vector+2.5*Right_Vector));
		clear=clear&&ValidTarget(Camera.Raycast(Camera.GetPosition()+800*Forward_Vector));
		CameraTimer=0;
		CameraClear=clear;
		return clear;
	}
	
	public VelocityTuple SavedVelocity=new VelocityTuple(new Vector3D(0,0,0));
	public void UpdateTimers(double seconds){
		seconds=Math.Abs(seconds);
		SavedVelocity.Timer+=seconds;
		if(Camera!=null&&Camera.EnableRaycast){
			double c_timer=CameraTimer;
			double t_timer=TargetTimer;
			double pool=seconds;
			if(c_timer<t_timer){
				double difference=Math.Min(pool,t_timer-c_timer/3);
				pool-=difference;
				c_timer+=difference;
			}
			c_timer+=pool/2;
			t_timer+=pool/2;
			CameraTimer=c_timer;
			TargetTimer=t_timer;
			if(Increase_GunTimer)
				GunTimer=Math.Min(5,GunTimer+seconds);
			if(Decrease_GunTimer)
				GunTimer=Math.Max(0,GunTimer-seconds);
		}
	}
	protected Vector3D GetPredictedVelocity(Vector3D Velocity,double Distance){
		Vector3D difference=(Velocity-SavedVelocity.Velocity)/SavedVelocity.Timer;
		SavedVelocity=new VelocityTuple(Velocity);
		return Velocity+(difference*Distance/800);
	}
	
	public bool CanAim(Vector3D Target){
		Vector3D Direction=Target-Remote.GetPosition();
		double Distance=Direction.Length();
		Direction.Normalize();
		
		double Default_Deflect=GetAngle(Default_Vector,Target);
		if(Default_Deflect>90)
			return false;
		
		Vector3D Elevation=Prog.GlobalToLocal(Direction,YawMotor);
		Elevation.Normalize();
		double rise=Elevation.Y;
		double run=Math.Sqrt(Math.Pow(Elevation.X,2)+Math.Pow(Elevation.Z,2));
		
		Vector3D Yaw_Up=Prog.GlobalToLocal(new Vector3D(0,1,0),YawMotor);
		if(GetAngle(Yaw_Up,Up_Vector)>90)
			rise*=-1;
		
		float elevation=(float)(Math.Atan(rise/run)*180/Math.PI);
		if(!PitchParity)
			elevation*=-1;
		if(elevation>=0){
			if(PitchMotor.UpperLimitDeg==float.MaxValue)
				return true;
			return elevation<PitchMotor.UpperLimitDeg;
		}
		else{
			if(PitchMotor.LowerLimitDeg==float.MaxValue)
				return true;
			return elevation>PitchMotor.LowerLimitDeg;
		}
	}
	
	public bool Link(IMyLargeTurretBase turret){
		if(turret==null){
			if(Turret==null)
				Remote.CustomData="";
			return false;
		}
		Turret=turret;
		if(Status==RTStatus.Unlinked)
			Status=RTStatus.Linked;
		Remote.CustomData=Turret.CustomName;
		return true;
	}
	
	public void SetLights(bool On){
		foreach(IMyLightingBlock Light in Lights)
			Light.Enabled=On;
	}
	
	public abstract bool Reset();
	
	public static double GetAngle(Vector3D v1,Vector3D v2){
		return GenericMethods<IMyTerminalBlock>.GetAngle(v1,v2);
	}
}

class MotorTurret:RotorTurret{
	protected bool YawParity=true; //True means positive angles are left, false means positive angles are right
	
	private MotorTurret(IMyMotorStator yawmotor,IMyMotorStator pitchmotor,List<IMySmallGatlingGun> guns,IMyRemoteControl remote,IMyCameraBlock camera):base(yawmotor,pitchmotor,guns,remote,camera){
		YawMotor.Torque=Math.Max(YawMotor.Torque,640000);
		PitchMotor.Torque=Math.Max(PitchMotor.Torque,640000);
		Status=RTStatus.Init0;
		if(YawMotor.CustomData.Length>0&&bool.TryParse(YawMotor.CustomData,out YawParity)){
			Status=RTStatus.Init1;
			if(PitchMotor.CustomData.Length>0&&bool.TryParse(PitchMotor.CustomData,out PitchParity))
				Status=RTStatus.Unlinked;
		}
		if(Turret==null&&Remote.CustomData.Length>0)
			Link(GenericMethods<IMyLargeTurretBase>.GetFull(Remote.CustomData.Trim()));
	}
	
	public static bool TryGet(IMyMotorStator Yaw,out MotorTurret Output){
		Output=null;
		IMyMotorStator yaw=Yaw;
		if(!yaw.IsAttached)
			return false;
		IMyMotorStator pitch=GenericMethods<IMyMotorStator>.GetGrid("",yaw.TopGrid,yaw,double.MaxValue);
		if(pitch==null||!pitch.IsAttached)
			return false;
		List<IMySmallGatlingGun> guns=GenericMethods<IMySmallGatlingGun>.GetAllGrid("",pitch.TopGrid,pitch,double.MaxValue);
		if(guns.Count==0)
			return false;
		IMyRemoteControl remote=GenericMethods<IMyRemoteControl>.GetGrid("",pitch.TopGrid,pitch,double.MaxValue);
		if(remote==null)
			return false;
		IMyCameraBlock camera=GenericMethods<IMyCameraBlock>.GetGrid("",remote.CubeGrid,remote);
		if(camera==null)
			return false;
		Output=new MotorTurret(yaw,pitch,guns,remote,camera);
		return true;
	}
	
	public static float GetRad(IMyMotorStator Motor,float TargetAngle=0,float Speed=1){
		return (float)(Speed*(Motor.Angle-(TargetAngle*Math.PI/180))/-1);
	}
	
	//Rad ==> 30/PI RPM
	public static float GetRPM(IMyMotorStator Motor,float TargetAngle=0,float Speed=1){
		return (float)(GetRad(Motor,TargetAngle,Speed)*30/Math.PI);
	}
	
	public static float GetRadD(IMyMotorStator Motor,bool Parity,float TargetAngle=0,float Speed=1){
		float difference;
		if(Parity)
			difference=-1*Angle.FromRadians(Motor.Angle).Difference_From_Top(new Angle(TargetAngle));
		else
			difference=Angle.FromRadians(Motor.Angle).Difference_From_Bottom(new Angle(TargetAngle));
		return (float)(Speed*difference/-1*(Math.PI/30));
	}
	
	public static float GetRPMD(IMyMotorStator Motor,bool Parity,float TargetAngle=0,float Speed=1){
		return (float)(GetRadD(Motor,Parity,TargetAngle,Speed)*30/Math.PI);
	}
	
	//Resets the turret to Default Vector
	bool Init(){
		Reset();
		if(GetAngle(Forward_Vector,Default_Vector)<1)
			return true;
		return false;
	}
	void Init0(){
		if(Init())
			Status=RTStatus.InitYaw;
	}
	void Init1(){
		if(Init())
			Status=RTStatus.InitPitch;
	}
	
	//Learns the pitch parity by adjusting the target velocity
	void InitYaw(){
		Angle angle=Angle.FromRadians(YawMotor.Angle);
		if(Math.Abs((angle.Degrees+180)%360-180)>=1.5){
			double difference=GetAngle(Default_Vector,Left_Vector)-GetAngle(Default_Vector,Right_Vector);
			YawParity=(difference<0);
			YawMotor.CustomData=YawParity.ToString();
			Status=RTStatus.Init1;
		}
		else{
			YawMotor.TargetVelocityRPM=GetRPM(YawMotor,2);
			PitchMotor.TargetVelocityRad=0;
		}
	}
	
	//Learns the pitch parity by tilting either up or down
	void InitPitch(){
		Angle angle=Angle.FromRadians(PitchMotor.Angle);
		bool flip=false;
		if(PitchMotor.UpperLimitDeg<2)
			flip=true;
		if(Math.Abs((angle.Degrees+180)%360-180)>=1.5){
			double difference=GetAngle(Default_Vector,Left_Vector)-GetAngle(Default_Vector,Right_Vector);
			if(flip)
				PitchParity=(difference<0);
			else
				PitchParity=(difference>=0);
			PitchMotor.CustomData=PitchParity.ToString();
			Status=RTStatus.Unlinked;
		}
		else{
			if(flip)
				PitchMotor.TargetVelocityRPM=GetRPM(PitchMotor,-2);
			else
				PitchMotor.TargetVelocityRPM=GetRPM(PitchMotor,2);
			YawMotor.TargetVelocityRad=0;
		}
	}
	
	public override bool Initialize(){
		RTStatus Last_Status=Status;
		switch(Status){
			case RTStatus.Init0:
				Init0();
				break;
			case RTStatus.InitYaw:
				InitYaw();
				break;
			case RTStatus.Init1:
				Init1();
				break;
			case RTStatus.InitPitch:
				InitPitch();
				break;
			default:
				return true;
		}
		if(Last_Status!=Status)
			return Initialize();
		return false;
	}
	
	public override bool Reset(){
		SavedVelocity=new VelocityTuple(new Vector3D(0,0,0));
		YawMotor.TargetVelocityRad=GetRad(YawMotor);
		PitchMotor.TargetVelocityRad=GetRad(PitchMotor);
		GunTimer=0;
		SetLights(false);
		return Math.Abs(TurretAngle)<1;
	}
	
	public override bool Aim(Vector3D Target,Vector3D Velocity,bool reset=false){
		if(Remote.IsUnderControl)
			return false;
		SetLights(!reset);
		double Distance=(Target-Remote.GetPosition()).Length();
		if(!reset)
			Velocity=GetPredictedVelocity(Velocity,Distance);
		Target+=Velocity*Distance/400;
		Vector3D Direction=Target-Remote.GetPosition();
		Distance=Direction.Length();
		Direction.Normalize();
		
		double Angle=GetAngle(Direction,Forward_Vector);
		double AngularVelocity=Remote.GetShipVelocities().AngularVelocity.Length();
		
		float Speed_Multx=1;
		if(Angle>0.5&&AngularVelocity<0.1)
			Speed_Multx*=(float)Math.Min(800/Distance,16)*5;
		
		if(AngularVelocity<0.1)
			Speed_Multx*=3;
		
		double difference_pitch=GetAngle(Direction,Up_Vector)-GetAngle(Direction,Down_Vector);
		float Current_Pitch=(float)(PitchMotor.Angle*180/Math.PI);
		Angle Target_Pitch=new Angle(Current_Pitch)+new Angle((float)difference_pitch);
		if(Target_Pitch>=new Angle(Current_Pitch))
			PitchMotor.TargetVelocityRad=GetRadD(PitchMotor,true,(float)Target_Pitch.Degrees,Speed_Multx/36);
		else
			PitchMotor.TargetVelocityRad=GetRadD(PitchMotor,false,(float)Target_Pitch.Degrees,Speed_Multx/36);
		
		
		double difference_yaw=GetAngle(Direction,Left_Vector)-GetAngle(Direction,Right_Vector);
		double difference_fb=GetAngle(Forward_Vector,Direction)-GetAngle(Backward_Vector,Direction);
		difference_yaw=(difference_yaw+180)%360-180;
		if(difference_fb>90)
			difference_yaw=270;
		float Current_Yaw=(float)(YawMotor.Angle*180/Math.PI);
		Angle Target_Yaw=new Angle(Current_Yaw)+new Angle((float)difference_yaw);
		Prog.P.Echo("difference_yaw:"+(new Angle((float)difference_yaw)).ToString(1));
		Prog.P.Echo("Current_Yaw:"+(new Angle((float)Current_Yaw)).ToString(1));
		Prog.P.Echo("Target_Yaw:"+Target_Yaw.ToString(1));
		
		
		if(Target_Yaw>=new Angle(Current_Yaw))
			YawMotor.TargetVelocityRPM=Math.Min(GetRPMD(YawMotor,true,(float)Target_Yaw.Degrees,Speed_Multx),10);
		else
			YawMotor.TargetVelocityRPM=Math.Max(GetRPMD(YawMotor,false,(float)Target_Yaw.Degrees,Speed_Multx),-10);
		
		
		double Max_Allowed_Angle=1;
		double Distance_Comp=Math.Min(Math.Max(400/Distance,1),8);
		double Speed_Comp=Velocity.Length()/20;
		Max_Allowed_Angle+=Math.Sqrt(Math.Pow(Distance_Comp,2)+Math.Pow(Speed_Comp,2));
		
		bool Clear_Shot=ClearVision();
		
		bool ValidShot=Clear_Shot&&Angle<=Max_Allowed_Angle;
		Increase_GunTimer=ValidShot&&!reset;
		ValidShot=Clear_Shot&&Angle<=Max_Allowed_Angle+Math.Max(Math.Min(GunTimer,5),0);
		Decrease_GunTimer=(!ValidShot)||reset;
		return ValidShot;
	}
}

class GyroTurret:RotorTurret{
	public IMyGyro Gyroscope;
	
	private GyroTurret(IMyMotorStator yawmotor,IMyMotorStator pitchmotor,List<IMySmallGatlingGun> guns,IMyRemoteControl remote,IMyCameraBlock camera,IMyGyro gyro):base(yawmotor,pitchmotor,guns,remote,camera){
		Status=RTStatus.Init0;
		YawMotor.Torque=250;
		PitchMotor.Torque=250;
		if(PitchMotor.CustomData.Length>0&&bool.TryParse(PitchMotor.CustomData,out PitchParity))
			Status=RTStatus.Unlinked;
		Gyroscope=gyro;
		Gyroscope.Pitch=0;
		Gyroscope.Yaw=0;
		Gyroscope.Roll=0;
		Gyroscope.GyroOverride=false;
		if(Turret==null&&Remote.CustomData.Length>0)
			Link(GenericMethods<IMyLargeTurretBase>.GetFull(Remote.CustomData.Trim()));
	}
	
	public static bool TryGet(IMyMotorStator Yaw,out GyroTurret Output){
		Output=null;
		IMyMotorStator yaw=Yaw;
		if(!yaw.IsAttached)
			return false;
		IMyMotorStator pitch=GenericMethods<IMyMotorStator>.GetGrid("",yaw.TopGrid,yaw,double.MaxValue);
		if(pitch==null||!pitch.IsAttached)
			return false;
		List<IMySmallGatlingGun> guns=GenericMethods<IMySmallGatlingGun>.GetAllGrid("",pitch.TopGrid,pitch,double.MaxValue);
		if(guns.Count==0)
			return false;
		IMyRemoteControl remote=GenericMethods<IMyRemoteControl>.GetGrid("",pitch.TopGrid,pitch,double.MaxValue);
		if(remote==null)
			return false;
		IMyCameraBlock camera=GenericMethods<IMyCameraBlock>.GetGrid("",remote.CubeGrid,remote);
		if(camera==null)
			return false;
		IMyGyro gyro=GenericMethods<IMyGyro>.GetGrid("",pitch.TopGrid,remote,double.MaxValue);
		if(gyro==null)
			return false;
		Output=new GyroTurret(yaw,pitch,guns,remote,camera,gyro);
		return true;
	}
	
	//Resets the turret to Default Vector
	void Init0(){
		if(GetAngle(Forward_Vector,Default_Vector)<1)
			Status=RTStatus.InitPitch;
		else
			Reset();
	}
	
	//Learns the pitch parity by tilting either up or down
	void InitPitch(){
		Angle angle=Angle.FromRadians(PitchMotor.Angle);
		bool RotorBelow=true;
		Vector3D Yaw_Up=Prog.GlobalToLocal(new Vector3D(0,1,0),YawMotor);
		if(GetAngle(Yaw_Up,Up_Vector)>90)
			RotorBelow=false;
		Vector3D Target=Forward_Vector;
		if(RotorBelow)
			Target+=Up_Vector;
		else
			Target+=Down_Vector;
		Target.Normalize();
		Target=Target*10+Remote.GetPosition();
		
		if(Math.Abs((angle.Degrees+180)%360-180)>=1.5){
			Angle zero=new Angle(0);
			if(RotorBelow)
				PitchParity=angle>zero;
			else
				PitchParity=angle<zero;
			PitchMotor.CustomData=PitchParity.ToString();
			Status=RTStatus.Unlinked;
		}
		else{
			Aim(Target,new Vector3D(0,0,0),true);
		}
	} 
	
	//Used to learn parities
	public override bool Initialize(){
		RTStatus Last_Status=Status;
		switch(Status){
			case RTStatus.Init0:
				Init0();
				break;
			case RTStatus.InitPitch:
				InitPitch();
				break;
			default:
				return true;
		}
		if(Last_Status!=Status)
			return Initialize();
		return false;
	}
	
	public override bool Aim(Vector3D Target,Vector3D Velocity,bool reset=false){
		if(Remote.IsUnderControl){
			Gyroscope.GyroOverride=false;
			return false;
		}
		Gyroscope.GyroOverride=true;
		
		double Distance=(Target-Remote.GetPosition()).Length();
		if(!reset){
			SetLights(true);
			Velocity=GetPredictedVelocity(Velocity,Distance);
		}
		Target+=Velocity*Distance/400;
		Vector3D Direction=Target-Remote.GetPosition();
		Distance=Direction.Length();
		Direction.Normalize();
		
		if(Distance>850){
			Reset();
			return false;
		}
		
		Vector3D Aimed_Target=Target;
		
		double Angle=GetAngle(Direction,Forward_Vector);
		double AngularVelocity=Remote.GetShipVelocities().AngularVelocity.Length();
		
		float Yaw_Multx=1;
		if(Angle>0.5&&AngularVelocity<0.1)
			Yaw_Multx*=(float)Math.Min(800/Distance,16)*5;
		
		Vector3D Aimed_Direction=Aimed_Target-Remote.GetPosition();
		double Aimed_Distance=Aimed_Direction.Length();
		Aimed_Direction.Normalize();
		
		if(AngularVelocity<0.1)
			Yaw_Multx*=3;
		
		Gyroscope.GyroOverride=true;
		
		float input_pitch=(float)Prog.GlobalToLocal(Remote.GetShipVelocities().AngularVelocity,Remote).X*0.99f;
		double difference_pitch=GetAngle(Up_Vector,Aimed_Direction)-GetAngle(Down_Vector,Aimed_Direction);
		difference_pitch=(difference_pitch+180)%360-180;
		if(Math.Abs(difference_pitch)>0.1)
			input_pitch+=2.5f*((float)Math.Min(Math.Max(difference_pitch,-90),90)/90.0f);
		float input_yaw=(float)Prog.GlobalToLocal(Remote.GetShipVelocities().AngularVelocity,Remote).Y*0.99f;
		
		double difference_yaw=(GetAngle(Left_Vector,Aimed_Direction)-GetAngle(Right_Vector,Aimed_Direction))/2;
		double difference_fb=GetAngle(Forward_Vector,Aimed_Direction)-GetAngle(Backward_Vector,Aimed_Direction);
		difference_yaw=(difference_yaw+180)%360-180;
		if(difference_fb>90)
			difference_yaw=270;
		
		
		if(Math.Abs(difference_yaw)>0.1){
			if(Math.Abs(difference_yaw)<5&&Math.Abs(difference_yaw)>1){
				if(difference_yaw>0)
					difference_yaw=5;
				else
					difference_yaw=-5;
			}
			input_yaw+=5*Yaw_Multx*((float)Math.Min(Math.Max(difference_yaw,-90),90)/90.0f);
		}
		
		Vector3D input=new Vector3D(input_pitch,input_yaw,0);
		Vector3D global=Vector3D.TransformNormal(input,Remote.WorldMatrix);
		Vector3D output=Vector3D.TransformNormal(global,MatrixD.Invert(Gyroscope.WorldMatrix));
		output.Normalize();
		output*=input.Length();
		
		Gyroscope.Pitch=(float)output.X;
		Gyroscope.Yaw=(float)output.Y;
		Gyroscope.Roll=(float)output.Z;
		
		double Max_Allowed_Angle=1;
		double Distance_Comp=Math.Min(Math.Max(400/Distance,1),8);
		double Speed_Comp=Velocity.Length()/20;
		Max_Allowed_Angle+=Math.Sqrt(Math.Pow(Distance_Comp,2)+Math.Pow(Speed_Comp,2));
		
		double Targeted_Angle=GetAngle(Direction,Forward_Vector);
		
		bool Clear_Shot=ClearVision();
		
		bool ValidShot=Clear_Shot&&Targeted_Angle<=Max_Allowed_Angle;
		Increase_GunTimer=ValidShot&&!reset;
		ValidShot=Clear_Shot&&Targeted_Angle<=Max_Allowed_Angle+Math.Max(Math.Min(GunTimer,5),0);
		Decrease_GunTimer=(!ValidShot)||reset;
		return ValidShot;
	}
	
	public override bool Reset(){
		SavedVelocity=new VelocityTuple(new Vector3D(0,0,0));
		bool output=Aim(Default_Vector*10+Remote.GetPosition(),new Vector3D(0,0,0),true);
		SavedVelocity=new VelocityTuple(new Vector3D(0,0,0));
		GunTimer=0;
		SetLights(false);
		return output;
	}
}

string[] SplitQuotes(string input,char c){
	string[] quotes=input.Split('\"');
	List<string> output=new List<string>();
	for(int i=0;i<quotes.Length;i++){
		if(i%2==0){
			string[] args=quotes[i].Split(c);
			for(int j=0;j<args.Length;j++){
				if(args[j].Length>0)
					output.Add(args[j]);
			}
		}
		else if(quotes[i].Length>0)
			output.Add(quotes[i]);
	}
	return output.ToArray();
}

bool Link(string argument){
	string[] args=SplitQuotes(argument.Substring(5),' ');
	if(args.Length!=3)
		return false;
	if(!args[1].ToLower().Equals("with"))
		return false;
	RotorTurret RT=null;
	foreach(RotorTurret R in RotorTurrets){
		if(R.Name.Equals(args[0])){
			RT=R;
			break;
		}
	}
	if(RT==null)
		return false;
	IMyLargeTurretBase Turret=null;
	foreach(IMyLargeTurretBase T in AllTurrets){
		if(T.CustomName.Equals(args[2])){
			Turret=T;
			break;
		}
	}
	if(Turret==null)
		return false;
	return RT.Link(Turret);
}

double Setup_Timer=0;
string TurretStatus(IMyLargeTurretBase T){
	string status="Idle";
	if(!T.IsFunctional)
		status="Damaged";
	else if(!T.IsWorking)
		status="Offline";
	else if(T.HasTarget)
		status="Targeting";
	else if(T.IsAimed)
		status="Aimed";
	return status;
}
void Main_Program(string argument){
	ProcessTasks();
	UpdateSystemData();
	Setup_Timer+=seconds_since_last_update;
	for(int i=0;i<RotorTurrets.Count;i++){
		RotorTurrets[i].UpdateTimers(seconds_since_last_update);
	}
	if(Setup_Timer>30){
		Setup_Timer=0;
		Turret_Setup();
	}
	if(argument.ToLower().Equals("factory reset")){
		FactoryReset();
	}
	else if(argument.ToLower().IndexOf("link:")==0)
		Link(argument);
	if(RotorTurrets.Count>0){
		string put="s";
		if(RotorTurrets.Count==1)
			put="";
		Display(1,RotorTurrets.Count.ToString()+" Rotor Turret"+put);
	}
	if(GatlingTurrets.Count>0){
		string put="s";
		if(GatlingTurrets.Count==1)
			put="";
		Display(1,GatlingTurrets.Count.ToString()+" Gatling Turret"+put);
	}
	if(MissileTurrets.Count>0){
		string put="s";
		if(MissileTurrets.Count==1)
			put="";
		Display(1,MissileTurrets.Count.ToString()+" Missile Turret"+put);
	}
	if(InteriorTurrets.Count>0){
		string put="s";
		if(GatlingTurrets.Count==1)
			put="";
		Display(1,InteriorTurrets.Count.ToString()+" Interior Turret"+put);
	}
	if(RotorTurrets.Count+AllTurrets.Count==0)
		Display(1,"0 Turrets");
	int display_number=2;
	if(RotorTurrets.Count>0){
		Display(display_number,"---Rotor Turrets---");
		for(int i=0;i<RotorTurrets.Count;i++){
			RotorTurret T=RotorTurrets[i];
			if(T.Status==RTStatus.Linked){
				string T_Name=GetRemovedString(T.Turret.CustomName," Turret").Trim();
				if(T_Name.Length>12)
					Display(display_number,(i+1).ToString()+") "+T.DisplayName+":Linked to "+T_Name.Substring(T_Name.Length-9)+"...["+TurretStatus(T.Turret)+"] ("+Math.Round(T.TurretAngle,1).ToString()+"°)");
				else
					Display(display_number,(i+1).ToString()+") "+T.DisplayName+":Linked to "+T_Name+"["+TurretStatus(T.Turret)+"] ("+Math.Round(T.TurretAngle,1).ToString()+"°)");
			}
				
			else
				Display(display_number,(i+1).ToString()+") "+T.DisplayName+":"+T.Status.ToString()+" ("+Math.Round(T.TurretAngle,1).ToString()+"°)");
		}
		display_number++;
	}
	if(GatlingTurrets.Count>0){
		Display(display_number,"---Gatling Turrets---");
		for(int i=0;i<GatlingTurrets.Count;i++){
			IMyLargeTurretBase T=GatlingTurrets[i];
			Display(display_number,(i+1).ToString()+") "+T.CustomName+":"+TurretStatus(T));
		}
		display_number++;
	}
	if(MissileTurrets.Count>0){
		Display(display_number,"---Missile Turrets---");
		for(int i=0;i<MissileTurrets.Count;i++){
			IMyLargeTurretBase T=MissileTurrets[i];
			Display(display_number,(i+1).ToString()+") "+T.CustomName+":"+TurretStatus(T));
		}
		display_number++;
	}
	if(InteriorTurrets.Count>0){
		Display(display_number,"---Interior Turrets---");
		for(int i=0;i<InteriorTurrets.Count;i++){
			IMyLargeTurretBase T=InteriorTurrets[i];
			Display(display_number,(i+1).ToString()+") "+T.CustomName+":"+TurretStatus(T));
		}
		display_number++;
	}
	
	
	foreach(RotorTurret R in RotorTurrets){
		if(R.Status<RTStatus.Unlinked)
			R.Initialize();
		bool Firing=false;
		if(R.Status>=RTStatus.Unlinked){
			if(R.Turret!=null&&R.Turret.IsFunctional){
				if(R.Turret.HasTarget){
					MyDetectedEntityInfo Entity=R.Turret.GetTargetedEntity();
					if(R.Aim(Entity.Position,Entity.Velocity))
						Firing=true;
				}
				else
					R.Reset();
			}
			else{
				List<IMyLargeMissileTurret> turretlist=new List<IMyLargeMissileTurret>();
				foreach(IMyLargeMissileTurret T in MissileTurrets)
					turretlist.Add(T);
				turretlist=GenericMethods<IMyLargeMissileTurret>.SortByDistance(turretlist,R.Remote.GetPosition());
				bool has_target=false;
				MyDetectedEntityInfo Entity=new MyDetectedEntityInfo();
				foreach(IMyLargeMissileTurret T in turretlist){
					if(T.HasTarget){
						Entity=T.GetTargetedEntity();
						if(R.CanAim(Entity.Position)){
							has_target=true;
							break;
						}
					}
				}
				if(has_target){
					if(R.Aim(Entity.Position,Entity.Velocity))
						Firing=true;
				}
				else if(!R.Remote.IsUnderControl)
					R.Reset();
				
			}
		}
		foreach(IMySmallGatlingGun Gun in R.Guns){
			if(Gun.GetValue<bool>("Shoot")!=Firing)
				Gun.SetValue<bool>("Shoot",Firing);
		}
	}
	
	
	Runtime.UpdateFrequency=GetUpdateFrequency();
}