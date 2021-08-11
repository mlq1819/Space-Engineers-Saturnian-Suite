/*
* Saturnian Core - Hauler Ship
* Built by mlq1616
* https://github.com/mlq1819/Space-Engineers-Saturnian-Suite
* This is an AI Ship Core. It interfaces with the various Operating Systems to run a ship of a specific type.
* Required Operating Systems:
	Maneuvering
	Navigation
	Resource
* The Hauler Core is designed to move materials and resources between two or more destinations automatically.
* It is intended for ships built to transport materials.
* A "Dock" is a position on a static grid that can be docked by this ship through a connector. It also establishes how much of your materials to deposit or withdraw.


TODO: 
- Navigation Integration
- Resource Integration?
- Determine what functions all ship cores need
*/
string Program_Name="Saturnian Hauler Ship Core";
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
		/*default:
			Display.FontColor=DEFAULT_TEXT_COLOR;
			Display.BackgroundColor=DEFAULT_BACKGROUND_COLOR;
			Display.Alignment=TextAlignment.LEFT;
			Display.ContentType=ContentType.TEXT_AND_IMAGE;
			Display.Font="Monospace";
			Display.TextPadding=0;
			Display.FontSize=0.5f;
			break;*/
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

//Contains raw IDs for items of each type
public static class Item{
	public static List<MyItemType> All{
		get{
			List<MyItemType> output=new List<MyItemType>();
			foreach(MyItemType I in Raw.All)
				output.Add(I);
			foreach(MyItemType I in Ingot.All)
				output.Add(I);
			foreach(MyItemType I in Comp.All)
				output.Add(I);
			foreach(MyItemType I in Ammo.All)
				output.Add(I);
			foreach(MyItemType I in Tool.All)
				output.Add(I);
			foreach(MyItemType I in Cons.All)
				output.Add(I);
			output.Add(Datapad);
			output.Add(Package);
			output.Add(Credit);
			return output;
		}
	}
	
	public static class Raw{
		static string B_O="MyObjectBuilder_Ore";
		public static List<MyItemType> All{
			get{
				List<MyItemType> output=new List<MyItemType>();
				output.Add(Ice);
				output.Add(Stone);
				output.Add(Iron);
				output.Add(Nickel);
				output.Add(Silicon);
				output.Add(Cobalt);
				output.Add(Uranium);
				output.Add(Magnesium);
				output.Add(Silver);
				output.Add(Gold);
				output.Add(Platinum);
				output.Add(Scrap);
				output.Add(Organic);
				return output;
			}
		}
		public static MyItemType Ice=new MyItemType(B_O,"Ice");
		public static MyItemType Stone=new MyItemType(B_O,"Stone");
		public static MyItemType Iron=new MyItemType(B_O,"Iron");
		public static MyItemType Nickel=new MyItemType(B_O,"Nickel");
		public static MyItemType Silicon=new MyItemType(B_O,"Silicon");
		public static MyItemType Cobalt=new MyItemType(B_O,"Cobalt");
		public static MyItemType Uranium=new MyItemType(B_O,"Uranium");
		public static MyItemType Magnesium=new MyItemType(B_O,"Magnesium");
		public static MyItemType Silver=new MyItemType(B_O,"Silver");
		public static MyItemType Gold=new MyItemType(B_O,"Gold");
		public static MyItemType Platinum=new MyItemType(B_O,"Platinum");
		public static MyItemType Scrap=new MyItemType(B_O,"Scrap");
		public static MyItemType Organic=new MyItemType(B_O,"Organic");
	}
	public static class Ingot{
		static string B_I="MyObjectBuilder_Ingot";
		public static List<MyItemType> All{		
			get{
				List<MyItemType> output=new List<MyItemType>();
				output.Add(Stone);
				output.Add(Iron);
				output.Add(Nickel);
				output.Add(Silicon);
				output.Add(Cobalt);
				output.Add(Uranium);
				output.Add(Magnesium);
				output.Add(Silver);
				output.Add(Gold);
				output.Add(Platinum);
				output.Add(Scrap);
				return output;
			}
		}
		public static MyItemType Stone=new MyItemType(B_I,"Stone");
		public static MyItemType Iron=new MyItemType(B_I,"Iron");
		public static MyItemType Nickel=new MyItemType(B_I,"Nickel");
		public static MyItemType Silicon=new MyItemType(B_I,"Silicon");
		public static MyItemType Cobalt=new MyItemType(B_I,"Cobalt");
		public static MyItemType Uranium=new MyItemType(B_I,"Uranium");
		public static MyItemType Magnesium=new MyItemType(B_I,"Magnesium");
		public static MyItemType Silver=new MyItemType(B_I,"Silver");
		public static MyItemType Gold=new MyItemType(B_I,"Gold");
		public static MyItemType Platinum=new MyItemType(B_I,"Platinum");
		public static MyItemType Scrap=new MyItemType(B_I,"Scrap");
	}
	public static class Comp{
		static string B_C="MyObjectBuilder_Component";
		public static List<MyItemType> All{		
			get{
				List<MyItemType> output=new List<MyItemType>();
				output.Add(Steel);
				output.Add(Construction);
				output.Add(Interior);
				output.Add(Motor);
				output.Add(Computer);
				output.Add(Small);
				output.Add(Large);
				output.Add(Grid);
				output.Add(Display);
				output.Add(Girder);
				output.Add(Thrust);
				output.Add(Reactor);
				output.Add(Super);
				output.Add(Power);
				output.Add(Detector);
				output.Add(Grav);
				output.Add(Medical);
				output.Add(Radio);
				output.Add(Solar);
				output.Add(Explosive);
				output.Add(Zone);
				output.Add(Canvas);
				return output;
			}
		}
		public static MyItemType Steel=new MyItemType(B_C,"SteelPlate");
		public static MyItemType Construction=new MyItemType(B_C,"Construction");
		public static MyItemType Interior=new MyItemType(B_C,"InteriorPlate");
		public static MyItemType Motor=new MyItemType(B_C,"Motor");
		public static MyItemType Computer=new MyItemType(B_C,"Computer");
		public static MyItemType Small=new MyItemType(B_C,"SmallTube");
		public static MyItemType Large=new MyItemType(B_C,"LargeTube");
		public static MyItemType Grid=new MyItemType(B_C,"MetalGrid");
		public static MyItemType Display=new MyItemType(B_C,"Display");
		public static MyItemType Girder=new MyItemType(B_C,"Girder");
		public static MyItemType Glass=new MyItemType(B_C,"BulletproofGlass");
		public static MyItemType Thrust=new MyItemType(B_C,"Thrust");
		public static MyItemType Reactor=new MyItemType(B_C,"Reactor");
		public static MyItemType Super=new MyItemType(B_C,"Superconductor");
		public static MyItemType Power=new MyItemType(B_C,"PowerCell");
		public static MyItemType Detector=new MyItemType(B_C,"Detector");
		public static MyItemType Grav=new MyItemType(B_C,"GravityGenerator");
		public static MyItemType Medical=new MyItemType(B_C,"Medical");
		public static MyItemType Radio=new MyItemType(B_C,"RadioCommunication");
		public static MyItemType Solar=new MyItemType(B_C,"SolarCell");
		public static MyItemType Explosive=new MyItemType(B_C,"Explosives");
		public static MyItemType Zone=new MyItemType(B_C,"ZoneChip");
		public static MyItemType Canvas=new MyItemType(B_C,"Canvas");
	}
	public static class Ammo{
		static string B_A="MyObjectBuilder_AmmoMagazine";
		public static List<MyItemType> All{		
			get{
				List<MyItemType> output=new List<MyItemType>();
				output.Add(Missile);
				output.Add(Container);
				output.Add(Magazine);
				output.Add(RifleB);
				output.Add(RifleP);
				output.Add(RifleA);
				output.Add(RifleE);
				output.Add(PistolB);
				output.Add(PistolA);
				output.Add(PistolE);
				return output;
			}
		}
		public static MyItemType Missile=new MyItemType(B_A,"Missile200mm");
		public static MyItemType Container=new MyItemType(B_A,"NATO_25x184mm");
		public static MyItemType Magazine=new MyItemType(B_A,"NATO_5p56x45mm");
		public static MyItemType RifleB=new MyItemType(B_A,"AutomaticRifleGun_Mag_20rd");
		public static MyItemType RifleP=new MyItemType(B_A,"PreciseAutomaticRifleGun_Mag_5rd");
		public static MyItemType RifleA=new MyItemType(B_A,"RapidFireAutomaticRifleGun_Mag_50rd");
		public static MyItemType RifleE=new MyItemType(B_A,"UltimateAutomaticRifleGun_Mag_30rd");
		public static MyItemType PistolB=new MyItemType(B_A,"SemiAutoPistolMagazine");
		public static MyItemType PistolA=new MyItemType(B_A,"FullAutoPistolMagazine");
		public static MyItemType PistolE=new MyItemType(B_A,"ElitePistolMagazine");
	}
	public static class Tool{
		static string B_T="MyObjectBuilder_PhysicalGunObject";
		public static List<MyItemType> All{		
			get{
				List<MyItemType> output=new List<MyItemType>();
				output.Add(H2);
				output.Add(O2);
				output.Add(Welder1);
				output.Add(Welder2);
				output.Add(Welder3);
				output.Add(Welder4);
				output.Add(Grinder1);
				output.Add(Grinder2);
				output.Add(Grinder3);
				output.Add(Grinder4);
				output.Add(Drill1);
				output.Add(Drill2);
				output.Add(Drill3);
				output.Add(Drill4);
				output.Add(RifleB);
				output.Add(RifleP);
				output.Add(RifleA);
				output.Add(RifleE);
				output.Add(PistolB);
				output.Add(PistolA);
				output.Add(PistolE);
				output.Add(RocketB);
				output.Add(RocketP);
				return output;
			}
		}
		public static MyItemType H2=new MyItemType("MyObjectBuilder_GasContainerObject","HydrogenBottle");
		public static MyItemType O2=new MyItemType("MyObjectBuilder_OxygenContainerObject","OxygenBottle");
		public static MyItemType Welder1=new MyItemType(B_T,"WelderItem");
		public static MyItemType Welder2=new MyItemType(B_T,"Welder2Item");
		public static MyItemType Welder3=new MyItemType(B_T,"Welder3Item");
		public static MyItemType Welder4=new MyItemType(B_T,"Welder4Item");
		public static MyItemType Grinder1=new MyItemType(B_T,"AngleGrinderItem");
		public static MyItemType Grinder2=new MyItemType(B_T,"AngleGrinder2Item");
		public static MyItemType Grinder3=new MyItemType(B_T,"AngleGrinder3Item");
		public static MyItemType Grinder4=new MyItemType(B_T,"AngleGrinder4Item");
		public static MyItemType Drill1=new MyItemType(B_T,"HandDrillItem");
		public static MyItemType Drill2=new MyItemType(B_T,"HandDrill2Item");
		public static MyItemType Drill3=new MyItemType(B_T,"HandDrill3Item");
		public static MyItemType Drill4=new MyItemType(B_T,"HandDrill4Item");
		public static MyItemType RifleB=new MyItemType(B_T,"AutomaticRifleItem");
		public static MyItemType RifleP=new MyItemType(B_T,"PreciseAutomaticRifleItem");
		public static MyItemType RifleA=new MyItemType(B_T,"RapidFireAutomaticRifleItem");
		public static MyItemType RifleE=new MyItemType(B_T,"UltimateAutomaticRifleItem");
		public static MyItemType PistolB=new MyItemType(B_T,"SemiAutoPistolItem");
		public static MyItemType PistolA=new MyItemType(B_T,"FullAutoPistolItem");
		public static MyItemType PistolE=new MyItemType(B_T,"ElitePistolItem");
		public static MyItemType RocketB=new MyItemType(B_T,"BasicHandHeldLauncherItem");
		public static MyItemType RocketP=new MyItemType(B_T,"AdvancedHandHeldLauncherItem");
	}
	public static class Cons{
		static string B_C="MyObjectBuilder_ConsumableItem";
		public static List<MyItemType> All{		
			get{
				List<MyItemType> output=new List<MyItemType>();
				output.Add(Power);
				output.Add(Medical);
				output.Add(Clang);
				output.Add(Cosmic);
				return output;
			}
		}
		public static MyItemType Power=new MyItemType(B_C,"Powerkit");
		public static MyItemType Medical=new MyItemType(B_C,"");
		public static MyItemType Clang=new MyItemType(B_C,"ClangCola");
		public static MyItemType Cosmic=new MyItemType(B_C,"CosmicCoffee");
	}
	
	public static MyItemType Datapad=new MyItemType("MyObjectBuilder_Datapad","Datapad");
	public static MyItemType Package=new MyItemType("MyObjectBuilder_Package","Package");
	public static MyItemType Credit=new MyItemType("MyObjectBuilder_PhysicalObject","SpaceCredit");
}

enum QuantityType{
	Percent,
	Value
}
struct Quantity{
	float Value;
	QuantityType Type;
	
	public Quantity(float value,QuantityType type){
		Type=type;
		value=Math.Max(0,value);
		if(Type==QuantityType.Percent)
			value=Math.Min(1,value);
		Value=value;
	}
	
	public override string ToString(){
		return "("+Value.ToString()+","+Type.ToString()+")";
	}
	
	public static bool TryParse(string input,out Quantity? output){
		output=null;
		if(input.IndexOf('(')!=0||input.IndexOf(')')!=input.Length-1)
			return false;
		string[] args=input.Substring(1,input.Length-2).Split(',');
		if(args.Length!=2)
			return false;
		float value;
		if(!float.TryParse(args[0],out value))
			return false;
		QuantityType? type;
		if((!Enum.TryParse(typeof(QuantityType),args[1],out type))||type==null)
			return false;
		output=new Quantity(value,(QuantityType)type);
		return true;
	}
}

abstract class TypedCargo{
	public override bool Item{
		get{
			return false;
		}
	}
	public override bool Resource{
		get{
			return false;
		}
	}
}
class ItemCargo:TypedCargo{
	public MyItemType Type;
	public override bool Item{
		get{
			return true;
		}
	}
	
	public ItemCargo(MyItemType type){
		Type=type;
	}
	
	public override string ToString(){
		return Type.ToString();
	}
	
	public static bool TryParse(string input,out ItemCargo output){
		output=null;
		try{
			MyItemType type=MyItemType.Parse(input);
			if(type==null)
				return false;
			output=new ItemCargo(type);
		}
		finally{
			return false;
		}
	}
}
enum ResourceType{
	Power,
	Hydrogen,
	Oxygen
}
class ResourceCargo:TypedCargo{
	public ResourceType Type;
	public static bool Resource{
		get{
			return true;
		}
	}
	
	public ResourceCargo(ResourceType type){
		Type=type;
	}
	
	public override string ToString(){
		return Type.ToString();
	}
	
	public static bool TryParse(string input,out ResourceCargo output){
		output=null;
		ResourceType? type;
		if(Enum.TryParse(typeof(ResourceType),input,out type)&&type!=null){
			output=new ResourceCargo((ResourceType)type);
			return true;
		}
		return false;
	}
}
enum CargoDirection{
	Deposit,
	Collect
}

struct CargoOrder{
	public ItemCargo Cargo;
	public CargoDirection Direction;
	public bool Dynamic=false;
	public Quantity Value;
	
	public CargoOrder(ItemCargo cargo,CargoDirection direction){
		Cargo=cargo;
		Direction=direction;
		Dynamic=true;
	}
	
	public CargoOrder(ItemCargo cargo,CargoDirection direction,Quantity value){
		Cargo=cargo;
		Direction=direction;
		Value=value;
	}
	
	public override string ToString(){
		string output="{"+Cargo.ToString()+";"+Direction.ToString()+";";
		if(Dynamic)
			output+=Dynamic.ToString();
		else
			output+=Value.ToString();
		output+="}";
		return output;
	}
	
	public static bool TryParse(string input,out CargoOrder? output){
		output=null;
		if(input.IndexOf('{')!=0||input.IndexOf('}')!=input.Length-1)
			return false;
		string[] args=input.Substring(1,input.Length-1).Split(';');
		if(args.Length!=3)
			return false;
		ItemCargo cargo;
		if(((!ResourceCargo.TryParse(args[0],out cargo))&&(!ItemCargo.TryParse(args[0],out cargo)))||cargo==null)
			return false;
		CargoDirection? direction;
		if((!Enum.TryParse(typeof(CargoDirection),args[1],out direction))||direction==null)
			return false;
		bool dynamic=false;
		float value;
		if((!bool.TryParse(args[2],out dynamic))&&(!float.TryParse(args[2],out dynamic)))
			return false;
		if(dynamic)
			output=new CargoOrder(cargo,(CargoDirection)direction);
		else
			output=new CargoOrder(cargo,(CargoDirection)direction,value);
		return true;
	}
}

class Dock{
	public Vector3D DockPosition;
	public Vector3D DockDirection;
	public Vector3D DockUp;
	public IMyShipConnector DockingConnector;
	public Vector3D DockApproach{
		get{
			return DockPosition+2.5*DockDirection+25*DockUp;
		}
	}
	
	public List<CargoOrder> Orders;
	
	public Dock(Vector3D dockPosition,Vector3D dockDirection,Vector3D dockUp){
		DockPosition=dockPosition;
		DockDirection=dockDirection;
		DockUp=dockUp;
		Orders=new List<CargoOrder>();
	}
	
	protected Dock(Vector3D dockPosition,Vector3D dockDirection,Vector3D dockUp,List<CargoOrder> orders):this(dockPosition,docDirection,dockUp){
		Orders=orders;
	}
	
	public override string ToString(){
		string output="{("+DockPosition.ToString()+"),("+DockDirection.ToString()+"),("+DockUp.ToString()+"),([";
		for(int i=0;i<Orders.Count;i++){
			if(i>0)
				output+=',';
			output+=Orders[i].ToString();
		}
		output+="])}";
		return output;
	}
	
	public static bool TryParse(string input,out Dock output){
		output=null;
		int[] indices={-1,-1,-1};
		int strCount=0;
		if(input.IndexOf("{(")!=0||input.IndexOf("])}")!=input.Length-3)
			return false;
		for(int i=2;i<input.Length-3;i++){
			if(input.Substring(i,3).Equals("),(")){
				if(strCount>2)
					return false;
				indices[strCount++]=i;
			}
		}
		if(strCount<3)
			return false;
		try{
			string p1=input.Substring(2,indices[0]);
			string p2=input.Substring(indices[0]+3,indices[1]-(indices[0]+3));
			string p3=input.Substring(indices[1]+3,indices[2]-(indices[1]+3));
			string p4=input.Substring(indices[2]+3,input.Length-2-(indices[2]+3));
			
			Vector3D dockPosition,dockDirection,dockUp;
			if(!Vector3D.TryParse(p1,dockPosition))
				return false;
			if(!Vector3D.TryParse(p2,dockDirection))
				return false;
			if(!Vector3D.TryParse(p3,dockUp))
				return false;
			if(p4[0]!='['||p4[p4.Length-1]!=']')
				return false;
			p4=p4.Substring(1,p4.Length-1);
			Queue<int> orderIndices=new Queue<int>();
			int depth=0;
			for(int i=0;i<p4.Length;i++){
				char c=p4[i];
				switch(c){
					case '{':
						depth++;
						break;
					case '}':
						if(depth<=0)
							return false;
						depth--;
						break;
					case ','
						orderIndices.Enqueue(i);
						break;
				}
			}
			int lastIndex=0;
			List<CargoOrder> orders=new List<orders>();
			for(int i=0;i<p4.Length;i++){
				CargoOrder order;
				if(i==orderIndices.Peek()){
					if(!CargoOrder.TryParse(p4.Substring(lastIndex,i-1)))
						return false;
					orders.Add(order);
					lastIndex=i+1;
					orderIndices.Dequeue();
					if(orderIndices.Count==0){
						if(!CargoOrder.TryParse(p4.Substring(lastIndex)))
							return false;
						orders.Add(order);
						break;
					}
				}
			}
			output=new Dock(dockPosition,dockDirection,dockUp,orders);
			return true;
		}
		catch(Exception){
			return false;
		}
	}
	
}

TimeSpan Time_Since_Start=new TimeSpan(0);
long cycle=0;
char loading_char='|';
double seconds_since_last_update=0;
Random Rnd;

IMyShipController Controller;

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
	return UpdateFrequency.Update10;
}

void Reset(){
	Operational=false;
	Runtime.UpdateFrequency=UpdateFrequency.None;
	Controller=null;
	//Reset LCD Lists
	Notifications=new List<Notification>();
}

double MySize=0;
bool Setup(){
	Reset();
	/*List<IMyTextPanel> LCDs=GenericMethods<IMyTextPanel>.GetAllConstruct("Altitude");
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
	}*/
	Controller=GenericMethods<IMyShipController>.GetClosestFunc(MainControllerFunction);
	if(Controller==null)
		Controller=GenericMethods<IMyShipController>.GetClosestFunc(ControllerFunction);
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
	UpdateMyDisplay();
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
	if(argument.ToLower().Equals("factory reset")){
		FactoryReset();
	}
	
	
	Runtime.UpdateFrequency=GetUpdateFrequency();
}