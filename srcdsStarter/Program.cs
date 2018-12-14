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
		static string srcds_ip="10.80.68.220";
		static int srcds_port=28015;
		static string srcds_cmd="";
		static string srcds_rcon_password="JA2PI";
		
		static string srcds_run="srcds.exe";		
			
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
			FolderIO.CheckFolderString(ref srcds_folder);
			Console.WriteLine("name=	"+srcds_name);
			Console.WriteLine("folder=	"+srcds_folder);
			Console.WriteLine("mod=	"+srcds_mod);
			Console.WriteLine("ip=	"+srcds_ip);
			Console.WriteLine("port=	"+srcds_port);
			Console.WriteLine("command line=	"+srcds_cmd);
			
			
			
			//
			//Test that all ready to start
			//
			if (!Directory.Exists(srcds_folder))//Folder with srcds exist
				{
				Console.ForegroundColor=ConsoleColor.Red;
				Console.WriteLine("ERR: Folder {0} not found.",srcds_folder);
				Console.ResetColor();
				ScriptFinish(true);
				System.Environment.Exit(4);				
				}
			if (!File.Exists(srcds_folder+srcds_run))//File srcds.exe  exist
				{
				Console.ForegroundColor=ConsoleColor.Red;
				Console.WriteLine("ERR: File {0} doesn't exist in folder {1} not found.",srcds_run,srcds_folder);
				Console.ResetColor();
				ScriptFinish(true);
				System.Environment.Exit(4);				
				}
			if (!Directory.Exists(srcds_folder+srcds_mod))//Mod folder exist
				{
				Console.ForegroundColor=ConsoleColor.Red;
				Console.WriteLine("ERR: Folder {0} not found in {1}.",srcds_mod,srcds_folder);
				Console.ResetColor();
				ScriptFinish(true);
				System.Environment.Exit(4);				
				}		
			
			Console.Title = title + " " + srcds_name + " "+DateTime.Now.ToString();
			//
			// Run srcds.exe
			//
			Console.ForegroundColor=ConsoleColor.White;
			Console.Write("\nRun {0} ",srcds_run);
			Console.ResetColor();
			Process srcds = new Process();
			srcds.StartInfo.RedirectStandardOutput = true;
			srcds.StartInfo.RedirectStandardError = true;
			srcds.StartInfo.CreateNoWindow = true;
			srcds.StartInfo.WorkingDirectory=srcds_folder;			
			srcds.StartInfo.FileName = srcds_folder + srcds_run;			
			srcds.StartInfo.UseShellExecute=false;	//https://msdn.microsoft.com/ru-ru/library/system.diagnostics.processstartinfo.workingdirectory(v=vs.110).aspx			
			srcds.StartInfo.Arguments+="-nocrashdialog -nomaster -console -insecure -maxplayers 32 -tickrate 300 +sv_lan 1";			
			srcds.StartInfo.Arguments+=" -game "+srcds_mod;
			srcds.StartInfo.Arguments+=" +ip "+srcds_ip;
			srcds.StartInfo.Arguments+=" -port "+srcds_port;
			srcds.StartInfo.Arguments+=" +hostname "+srcds_name;
			Console.WriteLine(srcds.StartInfo.Arguments);
			srcds.StartInfo.UseShellExecute = false;
			#if (bbDEBUG)
				srcds.StartInfo.RedirectStandardOutput = true;
			#else	
				srcds.StartInfo.RedirectStandardOutput = false;
			#endif	
			try 
				{					
			        srcds.Start();
            	}        	
        	catch (Exception e)
	        	{
	        		Console.ForegroundColor=ConsoleColor.Red;	        	    
	        	    Console.WriteLine(e.Message);
	        	    Console.ResetColor();
	        	}
        	#if (bbDEBUG)
			string output =srcds.StandardOutput.ReadToEnd();  
			string err =srcds.StandardError.ReadToEnd();	
			Console.WriteLine(output);			
			Console.WriteLine(err);			
			#endif
        	srcds.WaitForExit();
        	if (srcds.ExitCode>0) 
	        	{
	        	Console.ForegroundColor=ConsoleColor.Red;
	        	Console.WriteLine("ERRORLEVEL "+srcds.ExitCode);
	        	Console.ResetColor();
	        	}
        	
        	
        	
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