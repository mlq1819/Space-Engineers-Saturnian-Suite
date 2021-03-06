/*
* Saturnian Factory OS
* Built by mlq1616
* https://github.com/mlq1819/Space-Engineers-Saturnian-Suite
* This suite manages raw and processed material production, component production, and transportation of materials within the system.
* Include "Refining" in LCD name to add to Refinery group.
* Include "Assembling" in LCD name to add to Assembler group.
* Include "Material" in LCD name to add to Material Levels group.
* Include "Component" in LCD name to add to Component group. 
* Include "Network" in LCD name to add to the Networks group.


TODO: 
- Create Conveyor system objects
- Fill out item names
- Core Integration
*/
string Program_Name="Saturnian Factory";
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
	public static bool HasBlockData(IMyTerminalBlock Block, string Name){
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
	public static string GetBlockData(IMyTerminalBlock Block, string Name){
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
	public static bool SetBlockData(IMyTerminalBlock Block, string Name, string Data){
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
	public static bool CanHaveJob(IMyTerminalBlock Block, string JobName){
		return (!HasBlockData(Block,"Job"))||GetBlockData(Block,"Job").Equals("None")||GetBlockData(Block, "Job").Equals(JobName);
	}
	public static string GetRemovedString(string big_string, string small_string){
		string output=big_string;
		if(big_string.Contains(small_string)){
			output=big_string.Substring(0, big_string.IndexOf(small_string))+big_string.Substring(big_string.IndexOf(small_string)+small_string.Length);
		}
		return output;
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
	
	private static List<T> Get_AllBlocks(){
		List<T> output=new List<T>();
		TerminalSystem.GetBlocksOfType<T>(output);
		return output;
	}
	private static Rool<T> AllBlocks=new Rool<T>(Get_AllBlocks);
	
	public static T GetFull(string name,Vector3D Ref,double mx_d=double.MaxValue){
		List<T> MyBlocks=new List<T>();
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
		List<T> MyBlocks=new List<T>();
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
		List<List<T>> MyLists=new List<List<T>>();
		List<T> MyBlocks=new List<T>();
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
		if(name.Length==0&&mx_d==double.MaxValue)
			return AllBlocks;
		List<T> MyBlocks=new List<T>();
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
		List<T> MyBlocks=new List<T>();
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

public abstract class OneDone{
	public static List<OneDone> All;
	
	protected OneDone(){
		if(All==null)
			All=new List<OneDone>();
		All.Add(this);
	}
	
	public static void ResetAll(){
		if(All==null)
			return;
		for(int i=0;i<All.Count;i++)
			All[i].Reset();
	}
	
	public abstract void Reset();
}
public class OneDone<T>:OneDone{
	private T Default;
	public T Value;
	
	public OneDone(T value):base(){
		Default=value;
		Value=value;
	}
	
	public override void Reset(){
		Value=Default;
	}
	
	public static implicit operator T(OneDone<T> O){
		return O.Value;
	}
}
public class Rool<T>:IEnumerable<T>{
	// Run Only Once
	private List<T> _Value;
	public List<T> Value{
		get{
			if(!Ran.Value){
				_Value=Updater();
				Ran.Value=true;
			}
			return _Value;
		}
	}
	private OneDone<bool> Ran;
	private Func<List<T>> Updater;
	
	public Rool(Func<List<T>> updater){
		Ran=new OneDone<bool>(false);
		Updater=updater;
	}
	
	public IEnumerator<T> GetEnumerator(){
		return Value.GetEnumerator();
	}
	
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}
	
	public static implicit operator List<T>(Rool<T> R){
		return R.Value;
	}
}
public class Roo<T>{
	// Run Only Once
	private T _Value;
	public T Value{
		get{
			if(!Ran.Value){
				_Value=Updater();
				Ran.Value=true;
			}
			return _Value;
		}
	}
	private OneDone<bool> Ran;
	private Func<T> Updater;
	
	public Roo(Func<T> updater){
		Ran=new OneDone<bool>(false);
		Updater=updater;
	}
	
	public static implicit operator T(Roo<T> R){
		return R.Value;
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
	return Prog.HasBlockData(Block,Name);
}
string GetBlockData(IMyTerminalBlock Block, string Name){
	return Prog.GetBlockData(Block,Name);
}
bool SetBlockData(IMyTerminalBlock Block, string Name, string Data){
	return Prog.SetBlockData(Block,Name,Data);
}
bool CanHaveJob(IMyTerminalBlock Block, string JobName){
	return Prog.CanHaveJob(Block,JobName);
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

int Display_Count=1;
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
		default:
			Display.FontColor=DEFAULT_TEXT_COLOR;
			Display.BackgroundColor=DEFAULT_BACKGROUND_COLOR;
			Display.Alignment=TextAlignment.LEFT;
			Display.ContentType=ContentType.TEXT_AND_IMAGE;
			Display.Font="Monospace";
			Display.TextPadding=0;
			Display.FontSize=0.5f;
			break;
		/*default:
			Display.FontColor=DEFAULT_TEXT_COLOR;
			Display.BackgroundColor=DEFAULT_BACKGROUND_COLOR;
			Display.Alignment=TextAlignment.CENTER;
			Display.ContentType=ContentType.TEXT_AND_IMAGE;
			Display.Font="Debug";
			Display.TextPadding=2;
			Display.FontSize=1;
			break;*/
	}
}

string GetRemovedString(string big_string, string small_string){
	return Prog.GetRemovedString(big_string,small_string);
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

public enum MyRarity{
	VeryRare=1,
	Rare=2,
	Uncommon=3,
	Common=4,
	VeryCommon=5
}
public struct ModdedItem{
	public MyItemType Type;
	public MyRarity Rarity;
	
	public ModdedItem(MyItemType type,MyRarity rarity){
		Type=type;
		Rarity=rarity;
	}
	
	public ModdedItem(MyInventoryItem item){
		Type=item.Type;
		float amount=item.Amount.ToIntSafe();
		float multx=1;
		switch(Type.TypeId){
			case "MyObjectBuilder_Ore":
			case "MyObjectBuilder_Ingot":
				multx=100;
				break;
			case "MyObjectBuilder_Component":
				multx=10;
				break;
			case "MyObjectBuilder_PhysicalGunObject":
				multx=0.1f;
				break;
		}
		if(amount<=25*multx)
			Rarity=MyRarity.VeryRare;
		else if(amount<=2000*multx)
			Rarity=MyRarity.Rare;
		else if(amount<10000*multx)
			Rarity=MyRarity.Uncommon;
		else if(amount<20000*multx)
			Rarity=MyRarity.Common;
		else 
			Rarity=MyRarity.VeryCommon;
	}
	
	public override string ToString(){
		return Type.ToString()+";"+Rarity.ToString();
	}
	
	public static ModdedItem Parse(string input){
		int index=input.IndexOf(';');
		return new ModdedItem(MyItemType.Parse(input.Substring(0,index)),(MyRarity)Enum.Parse(typeof(MyRarity),input.Substring(index+1)));
	}
	
	public static bool TryParse(string input,out ModdedItem? output){
		output=null;
		try{
			output=Parse(input);
			return output!=null;
		}
		catch{
			return false;
		}
	}
}

//Contains raw IDs for items of each type
public static class Item{
	public static List<ModdedItem> MiscModded=new List<ModdedItem>();
	
	public static List<MyItemType> All{
		get{
			List<MyItemType> output=new List<MyItemType>();
			foreach(MyItemType Type in Vanilla)
				output.Add(Type);
			foreach(ModdedItem Type in Modded)
				output.Add(Type.Type);
			return output;
		}
	}
	public static List<MyItemType> Vanilla{
		get{
			List<MyItemType> output=new List<MyItemType>();
			foreach(MyItemType I in Raw.Vanilla)
				output.Add(I);
			foreach(MyItemType I in Ingot.Vanilla)
				output.Add(I);
			foreach(MyItemType I in Comp.Vanilla)
				output.Add(I);
			foreach(MyItemType I in Ammo.Vanilla)
				output.Add(I);
			foreach(MyItemType I in Tool.Vanilla)
				output.Add(I);
			foreach(MyItemType I in Cons.Vanilla)
				output.Add(I);
			output.Add(Datapad);
			output.Add(Package);
			output.Add(Credit);
			return output;
		}
	}
	public static List<ModdedItem> Modded{
		get{
			List<ModdedItem> output=new List<ModdedItem>();
			foreach(ModdedItem I in Raw.Modded)
				output.Add(I);
			foreach(ModdedItem I in Ingot.Modded)
				output.Add(I);
			foreach(ModdedItem I in Comp.Modded)
				output.Add(I);
			foreach(ModdedItem I in Ammo.Modded)
				output.Add(I);
			foreach(ModdedItem I in Tool.Modded)
				output.Add(I);
			foreach(ModdedItem I in Cons.Modded)
				output.Add(I);
			foreach(ModdedItem Type in Modded)
				output.Add(Type);
			return output;
		}
	}
	
	public static bool CheckExists(MyInventoryItem item){
		string TypeId=item.Type.TypeId;
		List<MyItemType> VanillaList;
		List<ModdedItem> ModdedList;
		switch(TypeId){
			case "MyObjectBuilder_Ore":
				VanillaList=Raw.Vanilla;
				ModdedList=Raw.Modded;
				break;
			case "MyObjectBuilder_Ingot":
				VanillaList=Ingot.Vanilla;
				ModdedList=Raw.Modded;
				break;
			case "MyObjectBuilder_Component":
				VanillaList=Comp.Vanilla;
				ModdedList=Comp.Modded;
				break;
			case "MyObjectBuilder_AmmoMagazine":
				VanillaList=Ammo.Vanilla;
				ModdedList=Ammo.Modded;
				break;
			case "MyObjectBuilder_PhysicalGunObject":
				VanillaList=Tool.Vanilla;
				ModdedList=Tool.Modded;
				break;
			case "MyObjectBuilder_ConsumableItem":
				VanillaList=Cons.Vanilla;
				ModdedList=Cons.Modded;
				break;
			default:
				VanillaList=Vanilla;
				ModdedList=MiscModded;
				break;
		}
		foreach(MyItemType Type in VanillaList){
			if(item.Type.Equals(Type))
				return true;
		}
		ModdedItem MyItem=new ModdedItem(item);
		for(int i=0;i<ModdedList.Count;i++){
			if(item.Type.Equals(ModdedList[i].Type)){
				if(ModdedList[i].Rarity<MyItem.Rarity)
					ModdedList[i]=MyItem;
				return true;
			}
		}
		ModdedList.Add(MyItem);
		return false;
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
	
	public static List<MyItemType> Search(string name){
		string[] args=name.Trim().ToLower().Split(' ');
		for(int i=0;i<args.Length;i++){
			if(args[i][args[i].Length-1]=='s')
				args[i]=args[i].Substring(0,args[i].Length-1);
		}
		List<MyItemType> output=new List<MyItemType>();
		foreach(MyItemType Type in All){
			bool match=true;
			string type=Type.TypeId.ToLower();
			string subtype=Type.SubtypeId.ToLower();
			foreach(string arg in args){
				if(type.Contains(arg)||arg.Contains(type))
					continue;
				else if(subtype.Contains(arg)||arg.Contains(subtype))
					continue;
				else{
					match=false;
					break;
				}
			}
			if(match)
				output.Add(Type);
		}
		return output;
	}
	
	public static class Raw{
		public static string B_O="MyObjectBuilder_Ore";
		public static List<ModdedItem> Modded=new List<ModdedItem>();
		public static List<MyItemType> All{
			get{
				List<MyItemType> output=new List<MyItemType>();
				foreach(MyItemType Type in Vanilla)
					output.Add(Type);
				foreach(ModdedItem Type in Modded)
					output.Add(Type.Type);
				return output;
			}
		}
		public static List<MyItemType> Vanilla{
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
		public static string B_I="MyObjectBuilder_Ingot";
		public static List<ModdedItem> Modded=new List<ModdedItem>();
		public static List<MyItemType> All{
			get{
				List<MyItemType> output=new List<MyItemType>();
				foreach(MyItemType Type in Vanilla)
					output.Add(Type);
				foreach(ModdedItem Type in Modded)
					output.Add(Type.Type);
				return output;
			}
		}
		public static List<MyItemType> Vanilla{
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
		public static string B_C="MyObjectBuilder_Component";
		public static List<ModdedItem> Modded=new List<ModdedItem>();
		public static List<MyItemType> All{
			get{
				List<MyItemType> output=new List<MyItemType>();
				foreach(MyItemType Type in Vanilla)
					output.Add(Type);
				foreach(ModdedItem Type in Modded)
					output.Add(Type.Type);
				return output;
			}
		}
		public static List<MyItemType> Vanilla{	
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
				output.Add(Glass);
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
		public static List<MyItemType> VeryCommon{
			get{
				List<MyItemType> output=new List<MyItemType>();
				output.Add(Steel);
				foreach(ModdedItem MyItem in Modded){
					if(MyItem.Rarity==MyRarity.VeryCommon)
						output.Add(MyItem.Type);
				}
				return output;
			}
		}
		public static List<MyItemType> Common{
			get{
				List<MyItemType> output=new List<MyItemType>();
				output.Add(Construction);
				output.Add(Interior);
				output.Add(Small);
				output.Add(Grid);
				output.Add(Glass);
				foreach(ModdedItem MyItem in Modded){
					if(MyItem.Rarity==MyRarity.Common)
						output.Add(MyItem.Type);
				}
				return output;
			}
		}
		public static List<MyItemType> Uncommon{
			get{
				List<MyItemType> output=new List<MyItemType>();
				output.Add(Motor);
				output.Add(Girder);
				output.Add(Thrust);
				foreach(ModdedItem MyItem in Modded){
					if(MyItem.Rarity==MyRarity.Uncommon)
						output.Add(MyItem.Type);
				}
				return output;
			}
		}
		public static List<MyItemType> Rare{
			get{
				List<MyItemType> output=new List<MyItemType>();
				output.Add(Computer);
				output.Add(Large);
				output.Add(Display);
				output.Add(Reactor);
				output.Add(Super);
				output.Add(Power);
				foreach(ModdedItem MyItem in Modded){
					if(MyItem.Rarity==MyRarity.Rare)
						output.Add(MyItem.Type);
				}
				return output;
			}
		}
		public static List<MyItemType> VeryRare{
			get{
				List<MyItemType> output=new List<MyItemType>();
				output.Add(Medical);
				output.Add(Grav);
				output.Add(Radio);
				output.Add(Solar);
				output.Add(Detector);
				output.Add(Explosive);
				output.Add(Zone);
				output.Add(Canvas);
				foreach(ModdedItem MyItem in Modded){
					if(MyItem.Rarity==MyRarity.VeryRare)
						output.Add(MyItem.Type);
				}
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
		public static string B_A="MyObjectBuilder_AmmoMagazine";
		public static List<ModdedItem> Modded=new List<ModdedItem>();
		public static List<MyItemType> All{
			get{
				List<MyItemType> output=new List<MyItemType>();
				foreach(MyItemType Type in Vanilla)
					output.Add(Type);
				foreach(ModdedItem Type in Modded)
					output.Add(Type.Type);
				return output;
			}
		}
		public static List<MyItemType> Vanilla{
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
		public static string B_T="MyObjectBuilder_PhysicalGunObject";
		public static List<ModdedItem> Modded=new List<ModdedItem>();
		public static List<MyItemType> All{
			get{
				List<MyItemType> output=new List<MyItemType>();
				foreach(MyItemType Type in Vanilla)
					output.Add(Type);
				foreach(ModdedItem Type in Modded)
					output.Add(Type.Type);
				return output;
			}
		}
		public static List<MyItemType> Vanilla{	
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
		public static string B_C="MyObjectBuilder_ConsumableItem";
		public static List<ModdedItem> Modded=new List<ModdedItem>();
		public static List<MyItemType> All{
			get{
				List<MyItemType> output=new List<MyItemType>();
				foreach(MyItemType Type in Vanilla)
					output.Add(Type);
				foreach(ModdedItem Type in Modded)
					output.Add(Type.Type);
				return output;
			}
		}
		public static List<MyItemType> Vanilla{
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
	
	public static void InitializeModdedItems(List<ModdedItem> ModdedItems){
		foreach(ModdedItem MyItem in ModdedItems){
			switch(MyItem.Type.TypeId){
				case "MyObjectBuilder_Ore":
					Raw.Modded.Add(MyItem);
					break;
				case "MyObjectBuilder_Ingot":
					Ingot.Modded.Add(MyItem);
					break;
				case "MyObjectBuilder_Component":
					Comp.Modded.Add(MyItem);
					break;
				case "MyObjectBuilder_AmmoMagazine":
					Ammo.Modded.Add(MyItem);
					break;
				case "MyObjectBuilder_PhysicalGunObject":
					Tool.Modded.Add(MyItem);
					break;
				case "MyObjectBuilder_ConsumableItem":
					Cons.Modded.Add(MyItem);
					break;
				default:
					MiscModded.Add(MyItem);
					break;
			}
		}
	}
}


class InvBlock{
	public IMyTerminalBlock Block;
	public MyItemType DefaultItem;
	public int InventoryCount{
		get{
			return Block.InventoryCount;
		}
	}
	public IMyInventory Inventory{
		get{
			return Block.GetInventory();
		}
	}
	public Inv_Network Network;
	
	public bool IsCargo{
		get{
			var t=Block as IMyCargoContainer;
			return t!=null;
		}
	}
	public bool IsConnector{
		get{
			var t=Block as IMyShipConnector;
			return t!=null;
		}
	}
	public bool IsSorter{
		get{
			var t=Block as IMyConveyorSorter;
			return t!=null;
		}
	}
	public bool IsTurret{
		get{
			var t=Block as IMyLargeTurretBase;
			return t!=null;
		}
	}
	public bool IsITurret{
		get{
			var t=Block as IMyLargeInteriorTurret;
			return t!=null;
		}
	}
	public bool IsGTurret{
		get{
			var t=Block as IMyLargeGatlingTurret;
			return t!=null;
		}
	}
	public bool IsMTurret{
		get{
			var t=Block as IMyLargeMissileTurret;
			return t!=null;
		}
	}
	public bool IsGun{
		get{
			var t=Block as IMySmallGatlingGun;
			return t!=null;
		}
	}
	public bool IsRocket{
		get{
			var t=Block as IMySmallMissileLauncher;
			return t!=null;
		}
	}
	public bool IsReactor{
		get{
			var t=Block as IMyReactor;
			return t!=null;
		}
	}
	public bool IsTank{
		get{
			var t=Block as IMyGasTank;
			return t!=null;
		}
	}
	public bool IsH2{
		get{
			if(!IsTank)
				return false;
			return Block.DefinitionDisplayNameText.ToLower().Contains("hydrogen");
		}
	}
	public bool IsO2{
		get{
			return IsTank&&!IsH2;
		}
	}
	public bool IsProduction{
		get{
			return IsGenerator||IsAssembler||IsRefinery;
		}
	}
	public bool IsGenerator{
		get{
			var t=Block as IMyGasGenerator;
			return t!=null;
		}
	}
	public bool IsAssembler{
		get{
			var t=Block as IMyAssembler;
			return t!=null;
		}
	}
	public bool IsRefinery{
		get{
			var t=Block as IMyRefinery;
			return t!=null;
		}
	}
	public bool IsParachute{
		get{
			var t=Block as IMyParachute;
			return t!=null;
		}
	}
	public bool IsDummy{
		get{
			var t=Block as IMyTargetDummyBlock;
			return t!=null;
		}
	}
	public bool IsSafeZone{
		get{
			var t=Block as IMySafeZoneBlock;
			return t!=null;
		}
	}
	public bool IsDrill{
		get{
			var t=Block as IMyShipDrill;
			return t!=null;
		}
	}
	public bool IsWelder{
		get{
			var t=Block as IMyShipWelder;
			return t!=null;
		}
	}
	public bool IsGrinder{
		get{
			var t=Block as IMyShipGrinder;
			return t!=null;
		}
	}
	
	public InvBlock(IMyTerminalBlock b){
		Block=b;
		if(IsCargo)
			Prog.SetBlockData(b,"BlockType","Cargo");
		else if(IsConnector)
			Prog.SetBlockData(b,"BlockType","Connector");
		else if(IsSorter)
			Prog.SetBlockData(b,"BlockType","Sorter");
		else if(IsTurret)
			Prog.SetBlockData(b,"BlockType","Turret");
		else if(IsGun)
			Prog.SetBlockData(b,"BlockType","Gun");
		else if(IsRocket)
			Prog.SetBlockData(b,"BlockType","Rocket");
		else if(IsReactor)
			Prog.SetBlockData(b,"BlockType","Reactor");
		else if(IsTank)
			Prog.SetBlockData(b,"BlockType","Tank");
		else if(IsGenerator)
			Prog.SetBlockData(b,"BlockType","Generator");
		else if(IsAssembler)
			Prog.SetBlockData(b,"BlockType","Assembler");
		else if(IsRefinery)
			Prog.SetBlockData(b,"BlockType","Refinery");
		else if(IsParachute)
			Prog.SetBlockData(b,"BlockType","Parachute");
		else if(IsDummy)
			Prog.SetBlockData(b,"BlockType","Dummy");
		else if(IsSafeZone)
			Prog.SetBlockData(b,"BlockType","SafeZone");
		else if(IsDrill)
			Prog.SetBlockData(b,"BlockType","Drill");
		else if(IsWelder)
			Prog.SetBlockData(b,"BlockType","Welder");
		else if(IsGrinder)
			Prog.SetBlockData(b,"BlockType","Grinder");
		
		DefaultItem=Item.Raw.Ice;
		if(IsCargo||IsConnector||IsWelder||IsGrinder||IsDummy)
			DefaultItem=Item.Comp.Steel;
		else if(IsReactor)
			DefaultItem=Item.Ingot.Uranium;
		else if(IsRefinery||IsDrill)
			DefaultItem=Item.Raw.Stone;
		else if(IsAssembler)
			DefaultItem=Item.Ingot.Iron;
		else if(IsGenerator)
			DefaultItem=Item.Raw.Ice;
		else if(IsGun||IsGTurret)
			DefaultItem=Item.Ammo.Container;
		else if(IsRocket||IsMTurret)
			DefaultItem=Item.Ammo.Missile;
		else if(IsITurret)
			DefaultItem=Item.Ammo.RifleA;
		else if(IsTank){
			if(IsH2)
				DefaultItem=Item.Tool.H2;
			else
				DefaultItem=Item.Tool.O2;
		}
		else if(IsParachute)
			DefaultItem=Item.Comp.Canvas;
		else if(IsSafeZone)
			DefaultItem=Item.Comp.Zone;
		else if(IsSorter){
			IMyConveyorSorter Sorter=Block as IMyConveyorSorter;
			List<MyInventoryItemFilter> Filter=new List<MyInventoryItemFilter>();
			Sorter.GetFilterList(Filter);
			if(Sorter.Mode==MyConveyorSorterMode.Whitelist){
				if(Filter.Count>0)
					DefaultItem=Filter[0].ItemType;
			}
			else{
				List<MyItemType> filter=new List<MyItemType>();
				foreach(MyInventoryItemFilter itemfilter in Filter)
					filter.Add(itemfilter.ItemType);
				foreach(MyItemType item in Item.All){
					if(!filter.Contains(item)){
						DefaultItem=item;
						break;
					}
				}
			}
		}
		Network=null;
	}
	
	public IMyInventory GetInventory(int n){
		return Block.GetInventory(n);
	}
	
	public bool SameNetwork(InvBlock o){
		if(!Block.IsSameConstructAs(o.Block))
			return false;
		if(!Inventory.CanTransferItemTo(o.Inventory,o.DefaultItem))
			return false;
		if(!o.Inventory.CanTransferItemTo(Inventory,DefaultItem))
			return false;
		return true;
	}
}
class CargoBlock:InvBlock{
	public IMyCargoContainer Cargo{
		get{
			return Block as IMyCargoContainer;
		}
	}
	public string CustomName{
		get{
			return Cargo?.CustomName??"null";
		}
	}
	string Name{
		get{
			return CustomName.ToLower();
		}
	}
	
	public bool Main{
		get{
			return Name.Contains("main");
		}
	}
	public bool Deep{
		get{
			return Name.Contains("deep");
		}
	}
	public bool Valid{
		get{
			return (Main||Deep)&&ItemTypes.Count>0;
		}
	}
	public static List<MyItemType> DeepItems;
	
	public List<MyItemType> ItemTypes;
	
	public void SetItemTypes(){
		ItemTypes=new List<MyItemType>();
		if(Main){
			if(Name.Contains("component")||Name.Contains("comp")){
				foreach(MyItemType Type in Item.Comp.All)
					ItemTypes.Add(Type);
			}
			else {
				bool ingots=false;
				bool ores=false;
				if(Name.Contains("material")){
					ingots=true;
					ores=true;
				}
				if(Name.Contains("ingot")||Name.Contains("processed")){
					ingots=true;
				}
				if(Name.Contains("raw")||Name.Contains("ore")){
					ores=true;
				}
				if(ingots){
					foreach(MyItemType Type in Item.Ingot.All)
						ItemTypes.Add(Type);
				}
				if(ores){
					foreach(MyItemType Type in Item.Raw.All)
						ItemTypes.Add(Type);
				}
			}
		}
		else if(Deep){
			int i1,i2;
			i1=Name.IndexOf('(')+1;
			i2=Name.IndexOf(')');
			if(i1>=0&&i2>i1){
				string str=Name.Substring(i1,i2-i1);
				string[] types=str.Split(',');
				foreach(string type in types){
					foreach(MyItemType Type in Item.Search(type.Trim())){
						if(Type.ToString().Contains("(null)"))
							continue;
						ItemTypes.Add(Type);
						if(Type.TypeId.Equals(Item.Raw.B_O)||Type.TypeId.Equals(Item.Ingot.B_I)){
							if(!DeepItems.Contains(Type))
								DeepItems.Add(Type);
						}
					}
				}
			}
		}
		string item_types="";
		for(int i=0;i<ItemTypes.Count;i++){
			if(i>0)
				item_types+=',';
			item_types+=ItemTypes[i].ToString();
		}
		Prog.SetBlockData(Block,"StorageTypes",item_types);
		if(Main)
			Prog.SetBlockData(Block,"CargoType","Main");
		else if(Deep)
			Prog.SetBlockData(Block,"CargoType","Deep");
	}
	
	public CargoBlock(IMyCargoContainer b):base(b){
		SetItemTypes();
	}
}

abstract class Network{
	public List<InvBlock> Nodes;
	public int Count{
		get{
			return Nodes.Count;
		}
	}
	public List<Network> Output;
	public List<Network> Input;
	
	protected Network(InvBlock i){
		Nodes=new List<InvBlock>();
		Nodes.Add(i);
		Output=new List<Network>();
		Input=new List<Network>();
	}
	
	public bool CanAdd(InvBlock node){
		return CanAdd(node,true);
	}
	
	public abstract bool CanAdd(InvBlock node,bool check_same);
	
	public bool ForceAdd(InvBlock node){
		Nodes.Add(node);
		return true;
	}
	
	public abstract bool Add(InvBlock node,bool check=true);
	
	public int TestConnection(){
		if(Nodes.Count<2)
			return -1;
		if(Nodes.Count<10){
			for(int i=0;i<Count;i++){
				for(int j=i+1;j<Count;j++){
					if(!Nodes[i].SameNetwork(Nodes[j]))
						return j;
				}
			}
		}
		else{
			Random Rnd=new Random();
			List<Vector2> indices=new List<Vector2>();
			int tries=0;
			int n1,n2;
			while(indices.Count<12&&(indices.Count<5||100>tries++)){
				int p=Rnd.Next(1,Count-1);
				n1=Rnd.Next(1,p);
				n2=Rnd.Next(p+1,Count);
				if(indices.Contains(new Vector2(n1,n2)))
					continue;
				indices.Add(new Vector2(n1,n2));
				if(!Nodes[0].SameNetwork(Nodes[n1]))
					return n1;
				if(!Nodes[0].SameNetwork(Nodes[n2]))
					return n2;
				if(!Nodes[n1].SameNetwork(Nodes[n2]))
					return n2;
			}
		}
		return -1;
	}
}
class Sort_Network:Network{
	
	
	public Sort_Network(InvBlock i):base(i){;}
	
	public override bool CanAdd(InvBlock node,bool check_same){
		return node.IsSorter;
	}
	
	public override bool Add(InvBlock node,bool check=true){
		if(!CanAdd(node,check))
			return false;
		Nodes.Add(node);
		return true;
	}
	
}
class Inv_Network:Network{
	int _Search_Index;
	public int Search_Index{
		get{
			return _Search_Index;
		}
	}
	public static Dictionary<MyItemType,MyFixedPoint> GlobalItems;
	public Dictionary<MyItemType,MyFixedPoint> MyItems;
	private Dictionary<MyItemType,MyFixedPoint> CountingItems;
	
	
	public int Increment_Search(){
		int value=Search_Index+1;
		if(Count>0){
			_Search_Index=value%Count;
			if(value>=Count){
				foreach(MyItemType Type in Item.All){
					if(!GlobalItems.ContainsKey(Type))
						GlobalItems.Add(Type,(MyFixedPoint)0);
					if(!MyItems.ContainsKey(Type))
						MyItems.Add(Type,(MyFixedPoint)0);
					if(!CountingItems.ContainsKey(Type))
						CountingItems.Add(Type,(MyFixedPoint)0);
					GlobalItems[Type]=MyFixedPoint.AddSafe(GlobalItems[Type],MyFixedPoint.AddSafe(CountingItems[Type],MyFixedPoint.MultiplySafe(-1,MyItems[Type])));
					MyItems[Type]=CountingItems[Type];
					CountingItems[Type]=(MyFixedPoint)0;
				}
			}
		}
		else
			_Search_Index=value;
		return Search_Index;
	}
	
	public void CountItems(MyInventoryItem MyItem){
		if(CountingItems.ContainsKey(MyItem.Type))
			CountingItems[MyItem.Type]=MyFixedPoint.AddSafe(CountingItems[MyItem.Type],MyItem.Amount);
		else
			CountingItems.Add(MyItem.Type,MyItem.Amount);
	}
	
	public Inv_Network(InvBlock i):base(i){
		_Search_Index=0;
		MyItems=new Dictionary<MyItemType,MyFixedPoint>();
		CountingItems=new Dictionary<MyItemType,MyFixedPoint>();
	}
	
	public bool InNetwork(InvBlock node){
		foreach(InvBlock Node in Nodes){
			if(node.Equals(Node))
				return true;
		}
		return false;
	}
	
	public override bool CanAdd(InvBlock node,bool check_same){
		
		if(Nodes.Count<100&&InNetwork(node))
			return false;
		if(Nodes.Count<10){
			foreach(InvBlock Node in Nodes){
				if((check_same&&Node.Equals(node))||!Node.SameNetwork(node))
					return false;
			}
			return true;
		}
		if((check_same&&Nodes[0].Equals(node))||!Nodes[0].SameNetwork(node))
			return false;
		List<int> indices=new List<int>();
		Random rnd=new Random();
		int tries=0;
		while(indices.Count<5&&(indices.Count<25||100>tries++)){
			int i=rnd.Next(0,Count);
			if(indices.Contains(i))
				continue;
			indices.Add(i);
			InvBlock Node=Nodes[i];
			if((check_same&&Node.Equals(node))||!Node.SameNetwork(node))
				return false;
		}
		return true;
	}
	
	public override bool Add(InvBlock node,bool check=true){
		if(check&&!CanAdd(node))
			return false;
		Nodes.Add(node);
		node.Network=this;
		return true;
	}
	
	protected bool Remove(InvBlock node){
		return Nodes.Remove(node);
	}
	
	protected bool RemoveAt(int i){
		if(i>Nodes.Count||i<0)
			return false;
		Nodes.RemoveAt(i);
		return true;
	}
	
	public bool Merge(Inv_Network O){
		foreach(InvBlock Node in O.Nodes){
			if(!CanAdd(Node))
				return false;
		}
		for(int i=O.Count-1;i>=0;i--){
			InvBlock Node=O.Nodes[i];
			if(!O.RemoveAt(i))
				return false;
			if(!Add(Node)){
				O.Add(Node,false);
				return false;
			}
		}
		return true;
	}
	
	public Inv_Network Split(int index){
		Inv_Network O=new Inv_Network(Nodes[index]);
		RemoveAt(index);
		for(int i=Nodes.Count-1;i>=0;i--){
			if(O.Count<=Count){
				if(O.Add(Nodes[i]))
					RemoveAt(i);
			}
			else{
				if((!CanAdd(Nodes[i],false))&&O.Add(Nodes[i]))
					RemoveAt(i);
			}
		}
		return O;
	}
	
	public Inv_Network Split(InvBlock node){
		return Split(Nodes.IndexOf(node));
	}
}

TimeSpan Time_Since_Start=new TimeSpan(0);
long cycle=0;
char loading_char='|';
double seconds_since_last_update=0;
Random Rnd;

List<CargoBlock> StorageBlocks;
List<InvBlock> InvBlocks;
List<Network> ConveyorNetworks;

List<CustomPanel> MaterialLCDs;
List<CustomPanel> ComponentLCDs;

bool InvBlockFunction(IMyTerminalBlock blk){
	return blk.IsSameConstructAs(Me)&&blk.InventoryCount>0;
}

UpdateFrequency GetUpdateFrequency(){
	return UpdateFrequency.Update100;
}

void Reset(){
	Operational=false;
	Runtime.UpdateFrequency=UpdateFrequency.None;
	StorageBlocks=new List<CargoBlock>();
	InvBlocks=new List<InvBlock>();
	ConveyorNetworks=new List<Network>();
	//Reset LCD Lists
	MaterialLCDs=new List<CustomPanel>();
	ComponentLCDs=new List<CustomPanel>();
	
	Notifications=new List<Notification>();
}

bool AutoBoot=false;
bool Setup(){
	Reset();
	Write("Beginning Setup");
	List<CustomPanel> MyLCDs=new List<CustomPanel>();
	List<IMyTextPanel> LCDs=GenericMethods<IMyTextPanel>.GetAllConstruct("Material");
	foreach(IMyTextPanel Panel in LCDs){
		CustomPanel panel=new CustomPanel(Panel);
		MaterialLCDs.Add(panel);
		MyLCDs.Add(panel);
	}
	LCDs=GenericMethods<IMyTextPanel>.GetAllConstruct("Component");
	foreach(IMyTextPanel Panel in LCDs){
		CustomPanel panel=new CustomPanel(Panel);
		ComponentLCDs.Add(panel);
		MyLCDs.Add(panel);
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
	
	List<IMyTerminalBlock> invBlocks=GenericMethods<IMyTerminalBlock>.GetAllFunc(InvBlockFunction);
	Write("Found "+invBlocks.Count.ToString()+" Inventory Blocks");
	int counter=0;
	foreach(IMyTerminalBlock b in invBlocks){
		Echo((++counter).ToString());
		IMyCargoContainer cargo=(b as IMyCargoContainer);
		Echo("cargo:"+(cargo?.CustomName??"null"));
		if(cargo!=null&&b.CustomName.ToLower().Contains("main")||b.CustomName.ToLower().Contains("deep")){
			CargoBlock block=new CargoBlock(cargo);
			StorageBlocks.Add(block);
			InvBlocks.Add(block);
		}
		else
			InvBlocks.Add(new InvBlock(b));
	}
	Write("Collected "+InvBlocks.Count.ToString()+" Inventory Blocks");
	if(InvBlocks.Count>0)
		ConveyorNetworks.Add(new Inv_Network(InvBlocks[0]));
	for(int i=1;i<InvBlocks.Count;i++){
		bool added=false;
		for(int j=0;j<ConveyorNetworks.Count;j++){
			if(ConveyorNetworks[j].Add(InvBlocks[i])){
				added=true;
				Write(InvBlocks[i].Block.CustomName+" added to existing network");
				break;
			}
		}
		if(!added){
			ConveyorNetworks.Add(new Inv_Network(InvBlocks[i]));
			Write(InvBlocks[i].Block.CustomName+" creates new network");
		}
	}
	Write("Created "+ConveyorNetworks.Count.ToString()+" Networks");
	for(int i=0;i<ConveyorNetworks.Count;i++){
		if(ConveyorNetworks[i].Count<=2){
			if(ConveyorNetworks[i].Count>=1)
				Write("  Removing micro-Network \""+ConveyorNetworks[i].Nodes[0].Block.CustomName+"\"");
			ConveyorNetworks.RemoveAt(i--);
			continue;
		}
	}
	Write("Trimmed to "+ConveyorNetworks.Count.ToString()+" Networks");
	for(int i=0;i<ConveyorNetworks.Count;i++){
		Network network=ConveyorNetworks[i];
		Write("Network "+(i+1).ToString()+": ("+network.Count.ToString()+" Nodes)");
		for(int j=0;j<network.Count;j++){
			string name=network.Nodes[j].Block.CustomName;
			if(name.Length>20)
				name=name.Substring(0,20);
			Write((j+1).ToString()+":"+name);
		}
	}
	if(HasBlockData(Me,"AutoBoot")){
		if(bool.TryParse(GetBlockData(Me,"AutoBoot"),out AutoBoot)&&AutoBoot)
			Me.Enabled=true;
	}
	List<ModdedItem> ModdedItems=new List<ModdedItem>();
	string mode="";
	string[] args=this.Storage.Split('\n');
	foreach(string arg in args){
		switch(arg){
			case "ModdedItems":
				mode=arg;
				break;
			default:
				switch(mode){
					case "ModdedItems":
						ModdedItem? MyItem;
						if(ModdedItem.TryParse(arg,out MyItem))
							ModdedItems.Add((ModdedItem)MyItem);
						break;
				}
				break;
		}
	}
	Item.InitializeModdedItems(ModdedItems);
	
	Operational=Me.IsWorking;
	Runtime.UpdateFrequency=GetUpdateFrequency();
	Write("Completed Setup");
	return true;
}

bool Operational=false;
public Program(){
	Echo("Beginning initialization");
	Prog.P=this;
	CargoBlock.DeepItems=new List<MyItemType>();
	Inv_Network.GlobalItems=new Dictionary<MyItemType,MyFixedPoint>();
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
	
	Rnd=new Random();
	Notifications=new List<Notification>();
	Task_Queue=new Queue<Task>();
	TaskParser(Me.CustomData);
	Echo("Completed initialization");
	Setup();
}

public void Save(){
	if(HasBlockData(Me,"AutoBoot"))
		bool.TryParse(GetBlockData(Me,"AutoBoot"),out AutoBoot);
	this.Storage="ModdedItems";
	foreach(ModdedItem MyItem in Item.Modded)
		this.Storage+="\n"+MyItem.ToString();
	Me.CustomData="";
	foreach(Task T in Task_Queue){
		Me.CustomData+=T.ToString()+'•';
	}
	SetBlockData(Me,"AutoBoot",AutoBoot.ToString());
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
	OneDone.ResetAll();
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
	//whoosh
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


List<CargoBlock> NetworkStorage(Inv_Network Network){
	List<CargoBlock> output=new List<CargoBlock>();
	foreach(CargoBlock Cargo in StorageBlocks){
		if(Network.InNetwork(Cargo))
			output.Add(Cargo);
	}
	return output;
}
string PrettyNumber(int num,int max){
	string output="";
	int digits=(int)Math.Min(1,Math.Floor(Math.Log(num+1))+1);
	int max_digits=(int)Math.Min(1,Math.Floor(Math.Log(max))+1);
	for(int k=digits;k<max_digits;k++)
		output+="0";
	output+=num.ToString();
	return output;
}

Vector2I GetSize(IMyTextSurface Display){
	if(Display.Font!="Monospace")
		Display.Font="Monospace";
	Vector2 Size=Display.SurfaceSize;
	Vector2 CharSize=Display.MeasureStringInPixels(new StringBuilder("|"),Display.Font,Display.FontSize);
	float Padding=(100-Display.TextPadding)/100f;
	return new Vector2I((int)(Padding*Size.X/CharSize.X-2),(int)(Padding*Size.Y/CharSize.Y));
}

void PrintMaterials(CustomPanel Panel){
	Vector2I Size=GetSize(Panel.Display);
	int YLEN=Math.Max(Math.Max(Item.Raw.All.Count,Item.Ingot.All.Count),10);
	while(Panel.Display.FontSize>0.1&&Size.X<50&&Size.Y<YLEN){
		float FontSize=Panel.Display.FontSize;
		FontSize=Math.Max(FontSize-0.1f,FontSize*.9f);
		Panel.Display.FontSize=FontSize;
		Size=GetSize(Panel.Display);
	}
	int XLEN=Size.X-11;
	string output="";
	foreach(MyItemType OreType in Item.Raw.All){
		if(OreType.SubtypeId.ToLower().Contains("scrap")||OreType.SubtypeId.ToLower().Contains("organic"))
			continue;
		string name=OreType.SubtypeId;
		if(name.ToLower().Equals("stone"))
			name="Gravel";
		if(name.Length>10)
			name=name.Substring(0,10);
		for(int x=name.Length;x<10;x++)
			output+=" ";
		for(int x=0;x<name.Length;x++)
			output+=name[x];
		output+=":";
		double OreQuantity=0;
		if(Inv_Network.GlobalItems.ContainsKey(OreType))
			OreQuantity=Inv_Network.GlobalItems[OreType].ToIntSafe();
		MyItemType IngotType=new MyItemType(Item.Ingot.B_I,OreType.SubtypeId);
		double BaseQuantity=0;
		if(Item.Ingot.All.Contains(IngotType)){
			double IngotQuantity=0;
			if(Inv_Network.GlobalItems.ContainsKey(IngotType))
				IngotQuantity=Inv_Network.GlobalItems[IngotType].ToIntSafe();
			BaseQuantity=IngotQuantity;
		}
		else
			BaseQuantity=OreQuantity;
		double Standard;
		if(OreType.Equals(Item.Raw.Ice)||OreType.Equals(Item.Raw.Iron))
			Standard=5000000;
		else if(OreType.Equals(Item.Raw.Gold)||OreType.Equals(Item.Raw.Silver)||OreType.Equals(Item.Raw.Platinum)||OreType.Equals(Item.Raw.Uranium))
			Standard=50000;
		else
			Standard=500000;
		int steps=(XLEN-1)*3/4;
		double Base=Standard/Math.Pow(1.2,steps);
		for(int x=0;x<XLEN;x++){
			double compare;
			if(x<XLEN*.75){
				compare=Base*Math.Pow(1.2,x);
				if(BaseQuantity<compare||BaseQuantity<=0)
					output+=' ';
				else{
					if(compare<Standard*.2)
						output+='•';
					else
						output+='■';
				}
			}
			else{
				compare=Standard+(x-XLEN*.75)*Standard*0.2;
				if(BaseQuantity<compare||BaseQuantity<=0)
					output+=' ';
				else
					output+='♦';
			}
		}
		output+='\n';
	}
	Panel.Display.WriteText(output,false);
}
void PrintComponents(CustomPanel Panel){
	Vector2I Size=GetSize(Panel.Display);
	int YLEN=Math.Max(Item.Comp.All.Count,10);
	while(Panel.Display.FontSize>0.1&&Size.X<50&&Size.Y<YLEN){
		float FontSize=Panel.Display.FontSize;
		FontSize=Math.Max(FontSize-0.1f,FontSize*.9f);
		Panel.Display.FontSize=FontSize;
		Size=GetSize(Panel.Display);
	}
	int XLEN=Size.X-11;
	string output="";
	foreach(MyItemType Type in Item.Comp.All){
		string name=Type.SubtypeId;
		if(name.ToLower().Contains("glass"))
			name="Glass";
		if(name.Length>10)
			name=name.Substring(0,10);
		for(int x=name.Length;x<10;x++)
			output+=" ";
		for(int x=0;x<name.Length;x++)
			output+=name[x];
		output+=":";
		double BaseQuantity=0;
		if(Inv_Network.GlobalItems.ContainsKey(Type))
			BaseQuantity=Inv_Network.GlobalItems[Type].ToIntSafe();
		double Standard;
		if(Item.Comp.VeryCommon.Contains(Type))
			Standard=500000;
		else if(Item.Comp.Common.Contains(Type))
			Standard=100000;
		else if(Item.Comp.Uncommon.Contains(Type))
			Standard=50000;
		else if(Item.Comp.Rare.Contains(Type))
			Standard=10000;
		else if(Item.Comp.VeryRare.Contains(Type))
			Standard=250;
		else
			Standard=1;
		
		string quantity_str=Math.Round(BaseQuantity,0).ToString();
		if(BaseQuantity>=1000000)
			quantity_str=Math.Round(BaseQuantity/1000000,1).ToString()+"M";
		else if(BaseQuantity>=1000)
			quantity_str=Math.Round(BaseQuantity/1000,1).ToString()+"K";
		int quantity_len=quantity_str.Length;
		
		int steps=(XLEN-7)*3/4;
		int X_Standard=(XLEN-6)*3/4;
		double Base=Standard/Math.Pow(1.2,steps);
		for(int x=0;x<XLEN-6;x++){
			double compare;
			if(x<X_Standard){
				compare=Base*Math.Pow(1.2,x);
				if(BaseQuantity<compare)
					output+=' ';
				else{
					if(compare<Standard*.2)
						output+='•';
					else
						output+='■';
				}
			}
			else{
				compare=Standard+(x-X_Standard)*Standard*0.2;
				if(BaseQuantity<compare||BaseQuantity<=0)
					output+=' ';
				else
					output+='♦';
			}
		}
		for(int i=quantity_len;i<6;i++)
			output+=" ";
		output+=quantity_str;
		output+='\n';
	}
	Panel.Display.WriteText(output,false);
}

void Main_Program(string argument){
	ProcessTasks();
	UpdateSystemData();
	if(argument.ToLower().Equals("factory reset")){
		FactoryReset();
	}
	int total_size=0;
	foreach(Inv_Network Network in ConveyorNetworks){
		total_size+=Network.Count;
	}
	Write("Scanning "+total_size.ToString()+" Inventories");
	for(int i=0;i<ConveyorNetworks.Count;i++){
		Inv_Network MyNetwork=ConveyorNetworks[i] as Inv_Network;
		if(MyNetwork==null)
			continue;
		int remaining_count=(int)Math.Min(Math.Ceiling((20f*MyNetwork.Count)/total_size),MyNetwork.Count);
		int starting_index=MyNetwork.Search_Index;
		
		string[] NodeStrings=new string[MyNetwork.Count];
		if(MyNetwork.Count<250){
			for(int j=0;j<MyNetwork.Count;j++)
				NodeStrings[j]="";
		}
		Write("Network "+PrettyNumber(i+1,ConveyorNetworks.Count-1)+"/"+ConveyorNetworks.Count.ToString()+": Scanning "+remaining_count.ToString()+"/"+MyNetwork.Count.ToString()+" Inventories");
		if(MyNetwork.Count==0)
			continue;
		List<CargoBlock> MyStorage=NetworkStorage(MyNetwork);
		List<MyItemType> ItemTypes=new List<MyItemType>();
		foreach(CargoBlock Storage in MyStorage){
			foreach(MyItemType Type in Storage.ItemTypes){
				if(!ItemTypes.Contains(Type))
					ItemTypes.Add(Type);
			}
		}
		do{
			InvBlock Block=MyNetwork.Nodes[MyNetwork.Search_Index];
			string check_str="\t Checking \""+Block.Block.CustomName+"\"";
			if(MyNetwork.Count<250)
				NodeStrings[MyNetwork.Search_Index]=check_str;
			else
				Write(check_str);
			
			CargoBlock Cargo=Block as CargoBlock;
			List<MyItemType> CompetingTypes=new List<MyItemType>();
			if(Cargo?.Valid??false){
				foreach(MyItemType Type in Cargo.ItemTypes)
					CompetingTypes.Add(Type);
			}
			
			for(int j=0;j<Block.InventoryCount;j++){
				IMyInventory Inventory=Block.GetInventory(j);
				List<MyInventoryItem> MyItems=new List<MyInventoryItem>();
				Inventory.GetItems(MyItems,null);
				foreach(MyInventoryItem MyItem in MyItems){
					MyNetwork.CountItems(MyItem);
					MyItemType Type=MyItem.Type;
					if(Block.IsAssembler&&Type.TypeId==Item.Ingot.B_I)
						continue;
					if(Block.IsRefinery&&Type.TypeId==Item.Raw.B_O)
						continue;
					if(Block.IsTurret)
						continue;
					if(Block.IsGun||Block.IsRocket)
						continue;
					if(Block.IsGenerator)
						continue;
					if(Block.IsParachute)
						continue;
					if(Block.IsDummy)
						continue;
					if(Block.IsSafeZone)
						continue;
					if(Block.IsWelder)
						continue;
					if(Block.IsTank)
						continue;
					bool competing=CompetingTypes.Contains(Type);
					foreach(CargoBlock MoveTo in MyStorage){
						if(MoveTo.Inventory.IsFull)
							continue;
						if(MoveTo.ItemTypes.Contains(Type)){
							if(competing){
								double my_quantity=MyItem.Amount.ToIntSafe();
								double target_quantity=0;
								if(MoveTo.Inventory.ContainItems(1,Type)){
									MyInventoryItem? targetItem=MoveTo.Inventory.FindItem(Type);
									if(targetItem!=null)
										target_quantity=((MyInventoryItem)targetItem).Amount.ToIntSafe();
								}
								double transfer_amount=0;
								if(MoveTo.Deep&&Cargo.Main){
									transfer_amount=Math.Max(0,my_quantity-5000);
								}
								else if(MoveTo.Main&&Cargo.Deep){
									transfer_amount=Math.Max(0,5000-target_quantity);
								}
								else{
									continue;
								}
								transfer_amount=Math.Min(transfer_amount,my_quantity);
								if(transfer_amount>0){
									if(Inventory.TransferItemTo(MoveTo.Inventory,MyItem,(MyFixedPoint)transfer_amount)){
										string trans_string="Transfered "+Math.Round(transfer_amount,0).ToString()+" Items of Type \""+Type.SubtypeId+"\"\n";
										if(MyNetwork.Count<250)
											NodeStrings[MyNetwork.Search_Index]+="\n"+trans_string;
										else
											Write(trans_string);
									}
								}
							}
							else{
								if(Inventory.TransferItemTo(MoveTo.Inventory,MyItem,null)){
									string trans_string="Transfered "+MyItem.Amount.ToString()+" Items of Type \""+Type.SubtypeId+"\"\n";
									if(MyNetwork.Count<250)
										NodeStrings[MyNetwork.Search_Index]+="\n"+trans_string;
									else
										Write(trans_string);
								}
							}
						}
					}
				}
			}
		}
		while(--remaining_count>0&&MyNetwork.Increment_Search()!=starting_index);
		
		if(MyNetwork.Count<250){
			List<string> ImportantNodes=new List<string>();
			for(int j=0;j<MyNetwork.Count;j++){
				if(NodeStrings[j].Length==0)
					NodeStrings[j]="\t Waiting "+MyNetwork.Nodes[j].Block.CustomName;
				NodeStrings[j]=PrettyNumber(j+1,MyNetwork.Count-1)+":"+NodeStrings[j];
				if(MyNetwork.Nodes[j].IsProduction){
					ImportantNodes.Add(NodeStrings[j]);
					NodeStrings[j]="";
				}
			}
			foreach(string str in ImportantNodes)
				Write(str);
			if(ImportantNodes.Count>0)
				Write("------");
			foreach(string str in NodeStrings){
				if(str.Length>0)
					Write(str);
			}
		}
	}
	
	if(cycle%6==0||cycle==2){
		foreach(CustomPanel Panel in MaterialLCDs)
			PrintMaterials(Panel);
		foreach(CustomPanel Panel in ComponentLCDs)
			PrintComponents(Panel);
	}
	
	
	Runtime.UpdateFrequency=GetUpdateFrequency();
}