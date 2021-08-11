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
* Commands:
- Add Cargo Dock
	Adds an empty Cargo Dock for the ship and sets it as the next destination.
	You must have a docking connector attached to a static grid.
- Remove Next Cargo Dock
	Permanently deletes the next scheduled cargo dock.
- Add Cargo Collect: <quantity> <type> <itemtype> <optional itemsubtype>
	Adds the specified cargo order as a collection for the next cargo dock.
	<quantity> specifies how much cargo. It may be set to "dynamic", a number, or a percent.
	<type> specifies the type of collection. This may be set to "item" or "resource".
	<itemtype> specifies the type of cargo. Values depend on <type>.
		For <type:resource>, it can be "power", "hydrogen", or "oxygen".
		For <type:item>, it can be "ingot","ore","component","ammo","tool","consumable","datapad","package", or "credit"
	<itemsybtype> specifies the subtype of the items. Only available for <type:item>
		For a full lookup of valid item names, please check the Item class.
- Add Cargo Deposit: <quantity> <type> <itemtype> <optional itemsubtype>
	Adds the specified cargo order as a deposit for the next cargo dock.
	<quantity> specifies how much cargo. It may be set to "dynamic", a number, or a percent.
	<type> specifies the type of collection. This may be set to "item" or "resource".
	<itemtype> specifies the type of cargo. Values depend on <type>.
		For <type:resource>, it can be "power", "hydrogen", or "oxygen".
		For <type:item>, it can be "ingot","ore","component","ammo","tool","consumable","datapad","package", or "credit"
	<itemsybtype> specifies the subtype of the items. Only available for <type:item>
		For a full lookup of valid item names, please check the Item class.


- Factory Reset
	Resets all settings and turns off the programmable block.
- Add Dock
	Adds a fueling dock for the ship to return to if low on fuel.
	You must have a docking connector attached to a static grid.
- Remove Dock
	Removes the current/nearest-within-100-meters fueling dock.





TODO: 
- Navigation Integration
- Resource Integration?
	Pass docking distance
	
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
	
	public static List<MyItemType> ByString(string name){
		List<MyItemType> output=new List<MyItemType>();
		int index=name.Trim().IndexOf(' ');
		string subtype="";
		if(index==-1)
			index=name.Length;
		else
			subtype=name.Substring(index+1).ToLower();
		string type=name.Substring(0,index).ToLower();
		if(type.Equals("raw")||type.Equals("ore"))
			return output.Concat(Raw.ByString(subtype)).ToList();
		if(type.Equals("ingot")||type.Equals("wafer")||type.Equals("powder"))
			return output.Concat(Ingot.ByString(subtype)).ToList();
		if(type.Equals("component")||type.Equals("comp"))
			return output.Concat(Comp.ByString(subtype)).ToList();
		if(type.Equals("ammo")||type.Equals("ammunition"))
			return output.Concat(Ammo.ByString(subtype)).ToList();
		if(type.Equals("tool")||type.Equals("gun")||type.Equals("weapon"))
			return output.Concat(Tool.ByString(subtype)).ToList();
		if(type.Equals("consumable")||type.Equals("cons"))
			return output.Concat(Cons.ByString(subtype)).ToList();
		if(type.Equals("data")||type.Equals("datapad")){
			output.Add(Datapad);
			return output;
		}
		if(type.Equals("package")){
			output.Add(Package);
			return output;
		}
		if(type.Equals("credit")||type.Equals("sc"))
			output.Add(Credit);
		return output;
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
		public static List<MyItemType> ByString(string subtype){
			if(subtype.Trim().Length==0)
				return All;
			List<MyItemType> output=new List<MyItemType>();
			foreach(MyItemType item in All){
				if(item.SubtypeId.ToLower().Equals(subtype))
					output.Add(item);
			}
			if(output.Count==0){
				foreach(MyItemType item in All){
					if(item.SubtypeId.ToLower().Contains(subtype)||subtype.Contains(item.SubtypeId.ToLower()))
						output.Add(item);
				}
			}
			return output;
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
		public static List<MyItemType> ByString(string subtype){
			if(subtype.Trim().Length==0)
				return All;
			List<MyItemType> output=new List<MyItemType>();
			foreach(MyItemType item in All){
				if(item.SubtypeId.ToLower().Equals(subtype))
					output.Add(item);
			}
			if(output.Count==0){
				foreach(MyItemType item in All){
					if(item.SubtypeId.ToLower().Contains(subtype)||subtype.Contains(item.SubtypeId.ToLower()))
						output.Add(item);
				}
			}
			return output;
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
		public static List<MyItemType> ByString(string subtype){
			if(subtype.Trim().Length==0)
				return All;
			List<MyItemType> output=new List<MyItemType>();
			foreach(MyItemType item in All){
				if(item.SubtypeId.ToLower().Equals(subtype))
					output.Add(item);
			}
			if(output.Count==0){
				foreach(MyItemType item in All){
					if(item.SubtypeId.ToLower().Contains(subtype)||subtype.Contains(item.SubtypeId.ToLower()))
						output.Add(item);
				}
			}
			return output;
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
		public static List<MyItemType> ByString(string subtype){
			if(subtype.Trim().Length==0)
				return All;
			List<MyItemType> output=new List<MyItemType>();
			foreach(MyItemType item in All){
				if(item.SubtypeId.ToLower().Equals(subtype))
					output.Add(item);
			}
			if(output.Count==0){
				foreach(MyItemType item in All){
					if(item.SubtypeId.ToLower().Contains(subtype)||subtype.Contains(item.SubtypeId.ToLower()))
						output.Add(item);
				}
			}
			return output;
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
		public static List<MyItemType> ByString(string subtype){
			if(subtype.Trim().Length==0)
				return All;
			List<MyItemType> output=new List<MyItemType>();
			foreach(MyItemType item in All){
				if(item.SubtypeId.ToLower().Equals(subtype))
					output.Add(item);
			}
			if(output.Count==0){
				foreach(MyItemType item in All){
					if(item.SubtypeId.ToLower().Contains(subtype)||subtype.Contains(item.SubtypeId.ToLower()))
						output.Add(item);
				}
			}
			return output;
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
		public static List<MyItemType> ByString(string subtype){
			if(subtype.Trim().Length==0)
				return All;
			List<MyItemType> output=new List<MyItemType>();
			foreach(MyItemType item in All){
				if(item.SubtypeId.ToLower().Equals(subtype))
					output.Add(item);
			}
			if(output.Count==0){
				foreach(MyItemType item in All){
					if(item.SubtypeId.ToLower().Contains(subtype)||subtype.Contains(item.SubtypeId.ToLower()))
						output.Add(item);
				}
			}
			return output;
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
	bool Valid{
		get{
			if(Value<0)
				return false;
			if(Type.Equals(QuantityType.Percent))
				return Value<=1;
			return true;
		}
	}
	public static Quantity Invalid{
		get{
			return new Quantity(-1,QuantityType.Value);
		}
	}
	
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
	
	public static bool TryParse(string input,out Quantity output){
		output=Invalid;
		if(input.IndexOf('(')!=0||input.IndexOf(')')!=input.Length-1)
			return false;
		string[] args=input.Substring(1,input.Length-2).Split(',');
		if(args.Length!=2)
			return false;
		float value;
		if(!float.TryParse(args[0],out value))
			return false;
		QuantityType type;
		if(!Enum.TryParse(args[1],out type))
			return false;
		output=new Quantity(value,type);
		return true;
	}
}

abstract class TypedCargo{
	public virtual bool Item{
		get{
			return false;
		}
	}
	public virtual bool Resource{
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
		catch(Exception){
			;
		}
		return false;
	}
}
enum ResourceType{
	Power,
	Hydrogen,
	Oxygen
}
class ResourceCargo:TypedCargo{
	public ResourceType Type;
	public override bool Resource{
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
		ResourceType type;
		if(Enum.TryParse(input,out type)){
			output=new ResourceCargo(type);
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
	public TypedCargo Cargo;
	public CargoDirection Direction;
	public bool Dynamic;
	public Quantity? Value;
	
	public CargoOrder(TypedCargo cargo,CargoDirection direction){
		Cargo=cargo;
		Direction=direction;
		Dynamic=true;
		Value=null;
	}
	
	public CargoOrder(TypedCargo cargo,CargoDirection direction,Quantity value){
		Cargo=cargo;
		Direction=direction;
		Value=value;
		Dynamic=false;
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
		ResourceCargo cargo_r;
		ItemCargo cargo_i;
		TypedCargo cargo;
		if(ResourceCargo.TryParse(args[0],out cargo_r))
			cargo=cargo_r;
		else if(ItemCargo.TryParse(args[0],out cargo_i))
			cargo=cargo_i;
		else
			return false;
		CargoDirection direction;
		if((!Enum.TryParse(args[1],out direction)))
			return false;
		bool dynamic=false;
		Quantity value=Quantity.Invalid;
		if((!bool.TryParse(args[2],out dynamic))&&(!Quantity.TryParse(args[2],out value)))
			return false;
		if(dynamic)
			output=new CargoOrder(cargo,direction);
		else
			output=new CargoOrder(cargo,direction,(Quantity)value);
		return true;
	}
}
class Dock{
	private IMyShipConnector _DockingConnector;
	public IMyShipConnector DockingConnector{
		get{
			return _DockingConnector;
		}
		set{
			_DockingConnector=value;
			RefreshDockName();
		}
	}
	public Vector3D DockPosition;
	public Vector3D DockDirection;
	public Vector3D DockUp;
	public Vector3D DockApproach{
		get{
			return DockPosition+2.5*DockDirection+25*DockUp;
		}
	}
	public string DockName;
	
	public Dock(IMyShipConnector dockingConnector,Vector3D dockPosition,Vector3D dockDirection,Vector3D dockUp,string dockName="Unnamed Dock"){
		DockName=dockName;
		DockingConnector=dockingConnector;
		DockPosition=dockPosition;
		DockDirection=dockDirection;
		DockUp=dockUp;
	}
	
	public override string ToString(){
		return "{"+DockName+";"+DockingConnector.CustomName.ToString()+";"+DockPosition.ToString()+";"+DockDirection.ToString()+";"+DockUp.ToString()+"}";
	}
	
	public static bool TryParse(string input,out Dock output){
		output=null;
		if(input.IndexOf('{')!=0||input.IndexOf('}')!=input.Length-1)
			return false;
		string[] args=input.Substring(1,input.Length-1).Split(';');
		if(args.Length!=5)
			return false;
		IMyShipConnector dockingConnector=GenericMethods<IMyShipConnector>.GetConstruct(args[0]);
		if(dockingConnector==null)
			return false;
		Vector3D dockPosition,dockDirection,dockUp;
		if(!Vector3D.TryParse(args[2],out dockPosition))
			return false;
		if(!Vector3D.TryParse(args[3],out dockDirection))
			return false;
		if(!Vector3D.TryParse(args[4],out dockUp))
			return false;
		output=new Dock(dockingConnector,dockPosition,dockDirection,dockUp,args[0]);
		return true;
	}
	
	public void RefreshDockName(){
		if(DockingConnector!=null&&DockingConnector.Status==MyShipConnectorStatus.Connected){
			IMyShipConnector Other=DockingConnector.OtherConnector;
			if(Other!=null)
				DockName=Other.CubeGrid.CustomName;
		}
	}
}
class CargoDock:Dock{
	public List<CargoOrder> Orders;
	
	public CargoDock(IMyShipConnector dockingConnector,Vector3D dockPosition,Vector3D dockDirection,Vector3D dockUp,string dockName="Unnamed Cargo Dock"):base(dockingConnector,dockPosition,dockDirection,dockUp,dockName){
		Orders=new List<CargoOrder>();
	}
	
	protected CargoDock(IMyShipConnector dockingConnector,Vector3D dockPosition,Vector3D dockDirection,Vector3D dockUp,List<CargoOrder> orders,string dockName):base(dockingConnector,dockPosition,dockDirection,dockUp,dockName){
		Orders=orders;
	}
	
	public bool AddOrder(CargoOrder Order){
		for(int i=0;i<Orders.Count;i++){
			CargoOrder order=Orders[i];
			if(order.Cargo.Equals(order.Cargo)){
				Orders.RemoveAt(i);
				Orders.Add(Order);
				return true;
			}
			if(order.Cargo.Item&&Order.Cargo.Item){
				MyItemType order_item=(order.Cargo as ItemCargo).Type;
				MyItemType Order_item=(Order.Cargo as ItemCargo).Type;
				if(order_item.TypeId==Order_item.TypeId){
					if(order_item.SubtypeId==Order_item.SubtypeId){
						Orders.RemoveAt(i);
						Orders.Add(Order);
						return true;
					}
				}
			}
		}
		Orders.Add(Order);
		return true;
	}
	
	public override string ToString(){
		string output="{("+DockName+"),("+DockingConnector.CustomName.ToString()+"),("+DockPosition.ToString()+"),("+DockDirection.ToString()+"),("+DockUp.ToString()+"),([";
		for(int i=0;i<Orders.Count;i++){
			if(i>0)
				output+=',';
			output+=Orders[i].ToString();
		}
		output+="])}";
		return output;
	}
	
	public static bool TryParse(string input,out CargoDock output){
		output=null;
		int[] indices={-1,-1,-1,-1,-1};
		int strCount=0;
		if(input.IndexOf("{(")!=0||input.IndexOf("])}")!=input.Length-3)
			return false;
		for(int i=2;i<input.Length-3;i++){
			if(input.Substring(i,3).Equals("),(")){
				if(strCount>4)
					return false;
				indices[strCount++]=i;
			}
		}
		if(strCount<5)
			return false;
		try{
			string p1=input.Substring(2,indices[0]);
			string p2=input.Substring(indices[0]+3,indices[1]-(indices[0]+3));
			string p3=input.Substring(indices[1]+3,indices[2]-(indices[1]+3));
			string p4=input.Substring(indices[2]+3,indices[3]-(indices[2]+3));
			string p5=input.Substring(indices[3]+3,indices[4]-(indices[3]+3));
			string p6=input.Substring(indices[4]+3,input.Length-2-(indices[4]+3));
			IMyShipConnector dockingConnector=GenericMethods<IMyShipConnector>.GetConstruct(p2);
			if(dockingConnector==null)
				return false;
			Vector3D dockPosition,dockDirection,dockUp;
			if(!Vector3D.TryParse(p3,out dockPosition))
				return false;
			if(!Vector3D.TryParse(p4,out dockDirection))
				return false;
			if(!Vector3D.TryParse(p5,out dockUp))
				return false;
			if(p6[0]!='['||p6[p6.Length-1]!=']')
				return false;
			p6=p6.Substring(1,p6.Length-1);
			Queue<int> orderIndices=new Queue<int>();
			int depth=0;
			for(int i=0;i<p6.Length;i++){
				char c=p6[i];
				switch(c){
					case '{':
						depth++;
						break;
					case '}':
						if(depth<=0)
							return false;
						depth--;
						break;
					case ',':
						orderIndices.Enqueue(i);
						break;
				}
			}
			int lastIndex=0;
			List<CargoOrder> orders=new List<CargoOrder>();
			for(int i=0;i<p6.Length;i++){
				CargoOrder? order;
				if(i==orderIndices.Peek()){
					if(!CargoOrder.TryParse(p6.Substring(lastIndex,i-1),out order))
						return false;
					orders.Add((CargoOrder)order);
					lastIndex=i+1;
					orderIndices.Dequeue();
					if(orderIndices.Count==0){
						if(!CargoOrder.TryParse(p6.Substring(lastIndex),out order))
							return false;
						orders.Add((CargoOrder)order);
						break;
					}
				}
			}
			output=new CargoDock(dockingConnector,dockPosition,dockDirection,dockUp,orders,p1);
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
List<Dock> FuelingDocks;
Queue<CargoDock> CargoDocks;
List<IMyShipConnector> DockingConnectors;

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
	DockingConnectors=new List<IMyShipConnector>();
	List<Dock> FuelingDocks=new List<Dock>();
	Queue<CargoDock> CargoDocks=new Queue<CargoDock>();
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
	Forward=Controller.Orientation.Forward;
	Up=Controller.Orientation.Up;
	Left=Controller.Orientation.Left;
	MySize=Controller.CubeGrid.GridSize;
	DockingConnectors=GenericMethods<IMyShipConnector>.GetAllConstruct("");
	ConnectorPruner();
	string mode="";
	string[] args=this.Storage.Split('\n');
	foreach(string arg in args){
		switch(arg){
			case "Docks":
				mode=arg;
				break;
			case "Cargo Docks":
				mode=arg;
				break;
			default:
				switch(mode){
					case "Docks":
						Dock dock;
						if(Dock.TryParse(arg,out dock))
							FuelingDocks.Add(dock);
						break;
					case "Cargo Docks":
						CargoDock cargoDock;
						if(CargoDock.TryParse(arg,out cargoDock))
							CargoDocks.Enqueue(cargoDock);
						break;
				}
				break;
		}
	}
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
	Notifications=new List<Notification>();
	Task_Queue=new Queue<Task>();
	TaskParser(Me.CustomData);
	Setup();
}

public void Save(){
	this.Storage="Docks";
	foreach(Dock dock in FuelingDocks)
		this.Storage+='\n'+dock.ToString();
	this.Storage+="\nCargo Docks";
	foreach(CargoDock dock in CargoDocks)
		this.Storage+='\n'+dock.ToString();
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

int ConnectorPruner(){
	int removed=0;
	for(int i=0;i<DockingConnectors.Count;i++){
		IMyShipConnector Connector=DockingConnectors[i];
		if(Connector.Status==MyShipConnectorStatus.Connected){
			IMyShipConnector Other=Connector.OtherConnector;
			if(Other==null)
				continue;
			if(Other.CubeGrid==Connector.CubeGrid||Other.CubeGrid.GridSizeEnum==MyCubeSize.Small){
				removed++;
				DockingConnectors.RemoveAt(i--);
				continue;
			}
		}
	}
	if(removed>0)
		Notifications.Add(new Notification("Pruned "+removed.ToString()+" DockingConnectors",5));
	return removed;
}

bool AddDock(){
	ConnectorPruner();
	foreach(IMyShipConnector Connector in DockingConnectors){
		if(Connector.Status==MyShipConnectorStatus.Connected&&Connector.OtherConnector.CubeGrid.GridSizeEnum==MyCubeSize.Large&&Connector.OtherConnector.CubeGrid.IsStatic){
			IMyShipConnector DockConnector=Connector.OtherConnector;
			foreach(Dock dock in FuelingDocks){
				if((dock.DockPosition-DockConnector.GetPosition()).Length()<2)
					return false;
			}
			Vector3D dockPosition=DockConnector.GetPosition();
			Vector3D dockDirection=LocalToGlobal(new Vector3D(0,0,-1),DockConnector);
			dockDirection.Normalize();
			FuelingDocks.Add(new Dock(Connector,dockPosition,dockDirection,Up_Vector));
			Notifications.Add(new Notification("Created new Fueling Dock with name \""+FuelingDocks[FuelingDocks.Count-1].DockName+"\"",10));
			return true;
		}
	}
	return false;
}

bool RemoveDock(){
	ConnectorPruner();
	double distance=5;
	for(int d=0;d<2;d++){
		foreach(IMyShipConnector Connector in DockingConnectors){
			if(Connector.Status==MyShipConnectorStatus.Connected&&Connector.OtherConnector.CubeGrid.GridSizeEnum==MyCubeSize.Large&&Connector.OtherConnector.CubeGrid.IsStatic){
				IMyShipConnector DockConnector=Connector.OtherConnector;
				for(int i=0;i<FuelingDocks.Count;i++){
					Dock dock=FuelingDocks[i];
					if((dock.DockPosition-DockConnector.GetPosition()).Length()<distance){
						Notifications.Add(new Notification("Removed Fueling Dock with name \""+dock.DockName+"\"",10));
						FuelingDocks.RemoveAt(i);
						return true;
					}
				}
			}
		}
		distance*=10;
	}
	return false;
}

bool AddCargoDock(){
	ConnectorPruner();
	foreach(IMyShipConnector Connector in DockingConnectors){
		if(Connector.Status==MyShipConnectorStatus.Connected&&Connector.OtherConnector.CubeGrid.GridSizeEnum==MyCubeSize.Large&&Connector.OtherConnector.CubeGrid.IsStatic){
			IMyShipConnector DockConnector=Connector.OtherConnector;
			foreach(CargoDock dock in CargoDocks){
				if((dock.DockPosition-DockConnector.GetPosition()).Length()<2)
					return false;
			}
			Vector3D dockPosition=DockConnector.GetPosition();
			Vector3D dockDirection=LocalToGlobal(new Vector3D(0,0,-1),DockConnector);
			dockDirection.Normalize();
			CargoDocks.Enqueue(new CargoDock(Connector,dockPosition,dockDirection,Up_Vector));
			for(int i=0;i<CargoDocks.Count-1;i++)
				CargoDocks.Enqueue(CargoDocks.Dequeue());
			Notifications.Add(new Notification("Created new Cargo Dock with name \""+CargoDocks.Peek().DockName+"\"",10));
			return true;
		}
	}
	return false;
}

bool RemoveNextCargoDock(){
	ConnectorPruner();
	if(CargoDocks.Count==0)
		return false;
	Notifications.Add(new Notification("Removed Cargo Dock with name \""+CargoDocks.Peek().DockName+"\"",10));
	return CargoDocks.Dequeue()!=null;
}

bool AddOrder(string data,CargoDirection direction){
	string[] args=data.Split(' ');
	if(args.Length<3||args.Length>4)
		return false;
	bool dynamic=args[0].Equals("dynamic");
	Quantity value=Quantity.Invalid;
	if(!dynamic){
		QuantityType qt=QuantityType.Value;
		if(args[0][args[0].Length-1]=='%'){
			qt=QuantityType.Percent;
			args[0]=args[0].Substring(0,args[0].Length-1);
		}
		float v;
		if(!float.TryParse(args[0],out v))
			return false;
		value=new Quantity(v,qt);
	}
	
	TypedCargo cargo;
	CargoOrder order;
	if(args[1].Equals("resource")){
		if(args.Length!=3)
			return false;
		ResourceType type;
		switch(args[2]){
			case "power":
				type=ResourceType.Power;
				break;
			case "hydrogen":
				type=ResourceType.Hydrogen;
				break;
			case "oxygen":
				type=ResourceType.Oxygen;
				break;
			default:
				return false;
		}
		cargo=new ResourceCargo(type);
	}
	else if(args[1].Equals("item")){
		string subtype="";
		if(args.Length==4)
			subtype=args[3];
		List<MyItemType> items=Item.ByString(args[2]+' '+subtype);
		if(items.Count==1)
			cargo=new ItemCargo(items[0]);
		else if(items.Count==0)
			return false;
		else {
			int success=0,failed=0;
			foreach(MyItemType item in items){
				if(dynamic)
					order=new CargoOrder(new ItemCargo(item),direction);
				else
					order=new CargoOrder(new ItemCargo(item),direction,value);
				if(CargoDocks.Peek().AddOrder(order)){
					success++;
					Notifications.Add(new Notification("\tSuccess: "+item.ToString(),10));
				}
				else{
					failed++;
					Notifications.Add(new Notification("\tFailure: "+item.ToString(),10));
				}
			}
			string s="s";
			if(success==1)
				s="";
			if(success>0)
				Notifications.Add(new Notification("Succesfully added "+success.ToString()+" new "+direction.ToString()+" Order"+s+".",10));
			s="s";
			if(failed==1)
				s="";
			if(failed>0)
				Notifications.Add(new Notification("Failed to add "+failed.ToString()+" new "+direction.ToString()+" Order"+s+".",10));
			return failed==0&&success>0;
		}
	}
	else
		return false;
	if(dynamic)
		order=new CargoOrder(cargo,direction);
	else
		order=new CargoOrder(cargo,direction,value);
	bool output=CargoDocks.Peek().AddOrder(order);
	if(output)
		Notifications.Add(new Notification("Succesfully added new "+direction.ToString()+" Order.",10));
	return output;
}

bool AddCollect(string data){
	return AddOrder(data,CargoDirection.Collect);
}

bool AddDeposit(string data){
	return AddOrder(data,CargoDirection.Deposit);
}

void Main_Program(string argument){
	ProcessTasks();
	UpdateSystemData();
	if(argument.ToLower().Equals("factory reset")){
		FactoryReset();
	}
	else if(argument.ToLower().Equals("add dock"))
		AddDock();
	else if(argument.ToLower().Equals("remove dock"))
		RemoveDock();
	else if(argument.ToLower().Equals("add cargo dock"))
		AddCargoDock();
	else if(argument.ToLower().Equals("remove next cargo dock"))
		RemoveNextCargoDock();
	else if(argument.ToLower().IndexOf("add cargo collect:")==0)
		AddCollect(argument.Substring(18).ToLower());
	else if(argument.ToLower().IndexOf("add cargo deposit:")==0)
		AddDeposit(argument.Substring(18).ToLower());
	
	
	Runtime.UpdateFrequency=GetUpdateFrequency();
}