/*
 * Created by SharpDevelop.
 * User: skorik
 * Date: 12.12.2018
 * Time: 18:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */ 
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Reflection;
using System.Timers;


namespace srcdsStarter
{
	class Program		
	{
		//Global
		static string srcds_name="DOD2018";  
		static string srcds_folder="                 f:\\soft\\Games\\Steam\\srcds.dod.07122018             ";
		static string srcds_mod="dod";
		static string srcds_ip="172.19.1.44";
		static int srcds_port=28015;
		static string srcds_cmd="";
		static string srcds_rcon_password="JA2PI";
		
		string srcds_run="srcds.exe";		
			
		public static void Main(string[] args)
		{
			string title="Start Source dedicated server ver "+(FileVersionInfo.GetVersionInfo((Assembly.GetExecutingAssembly()).Location)).ProductVersion+": ";
			Console.Title=title;
			Console.ForegroundColor=ConsoleColor.Magenta;
			Console.WriteLine("***************************************************");						
			Console.WriteLine(title);			
			Console.WriteLine("***************************************************");			
			Console.ResetColor();
			
			/*if (args.Length < 1) {
				Console.WriteLine("Usage: srcdsStarter <Server_name> <path> <mod> <ip> <port> <cmd> ");
				ScriptFinish(true);
				System.Environment.Exit(0);
			}*/
			Console.WriteLine("name=	"+srcds_name);
			Console.WriteLine("folder=	"+srcds_folder);
			Console.WriteLine("mod=	"+srcds_mod);
			Console.WriteLine("ip=	"+srcds_ip);
			Console.WriteLine("port=	"+srcds_port);
			Console.WriteLine("command line=	"+srcds_cmd);
			
			FolderIO.CheckFolderString(ref srcds_folder);
			
			
			//Test that all ready to start
			if (!Directory.Exists(srcds_folder))//Folder with srcds exist
				{
				Console.ForegroundColor=ConsoleColor.Red;
				Console.WriteLine("ERR: Folder {0} not found.",srcds_folder);
				Console.ResetColor();
				ScriptFinish(true);
				System.Environment.Exit(4);				
				}
			if (!Directory.Exists(srcds_folder))//File srcds.exe  exist
				{
				Console.ForegroundColor=ConsoleColor.Red;
				Console.WriteLine("ERR: Folder {0} not found.",srcds_folder);
				Console.ResetColor();
				ScriptFinish(true);
				System.Environment.Exit(4);				
				}
			//Test File 			
			
			
			Console.Title = title + " " + srcds_name + " "+DateTime.Now.ToString();
			
			
						
			ScriptFinish(true);
		}
		//****************************************************	
		public static void ScriptFinish(bool pause){
		//****************************************************			
		if (pause)
			{
			Console.ForegroundColor=ConsoleColor.White;
			Console.WriteLine();
			Console.Write("Press any key to exit . . . ");Console.ResetColor();
			DateTime timeoutvalue = DateTime.Now.AddSeconds(10);
			while (DateTime.Now < timeoutvalue)
				{
			    if (Console.KeyAvailable) break;
			      Thread.Sleep(100);
			    }
			}
		}
	}
}