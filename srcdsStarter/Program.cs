//[X]  Cycle start srcds
//TODO:[ ] Test param 0<port<65535
//TODO:[ ] Test param ip belong localhost
//[X] Hide windows after sucsesfull start srcds
//TODO:[ ]  add command and key
//TODO:[ ]  Check that this host has correct ip,  asighted in  cmd params
//TODO:[ ]  if server have the same parameters then just restart it over rcon


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
using System.Runtime.InteropServices;
//using System.Net.Sockets;
//using System.Timers;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using System.Collections;
//using System.ComponentModel;

namespace srcdsStarter
{
	class Program		
	{
		[DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool ShowWindow([In] IntPtr hWnd, [In] int nCmdShow);

		//Global
		static string srcds_name;  
		static string srcds_folder="";
		static string srcds_mod;
		static System.Net.IPAddress  srcds_ip/*=System.Net.IPAddress.Parse("10.80.68.220")*/;
		static int srcds_port=27015;
		static string srcds_cmd;
		static string srcds_rcon_password;
		
		static string srcds_run="srcds.exe";
		const string srds_default_command_line_params = "-nocrashdialog -nomaster -console -insecure +sv_lan 1 -silent -maxplayers 32 +sv_pure 1";
		static bool ReadyToRun=true; //Flag that condition, enironment, port, ip ready to start srcds.exe
		const ConsoleColor BGcolor = ConsoleColor.DarkYellow;
		const ConsoleColor FGcolor = ConsoleColor.Black;
		public static void Console_ResetColor() { Console.BackgroundColor = BGcolor; Console.ForegroundColor = FGcolor; }
		public static void Main(string[] args)
		{
			string title="Start Source dedicated server ver "+(FileVersionInfo.GetVersionInfo((Assembly.GetExecutingAssembly()).Location)).ProductVersion+": ";
			Console.Title=title;
			Console_ResetColor();
			Console.Clear();
			Console.ForegroundColor=ConsoleColor.Blue;
			Console.WriteLine("***************************************************");						
			Console.WriteLine(title);					
			Console.WriteLine("***************************************************");			
			
			int argsLength=args.Length;
			if (argsLength < 6) {
				Console.WriteLine("Usage: srcdsStarter <Server_name> <path> <mod> <ip> <port> <cmd> [<rcon_password>] ");
				ScriptFinish(true);
				System.Environment.Exit(0);
			}
			
			//for (int i=0;i!=argsLength;i++)
			//	{
			//	Console.WriteLine("arg({0})={1}",i,args[i]);
			//	}
			srcds_name=args[0];
			srcds_folder=args[1];
			srcds_mod=args[2];
			srcds_ip=System.Net.IPAddress.Parse(args[3]);
			srcds_port=Int32.Parse(args[4]);
			srcds_cmd=args[5];
			//srcds_rcon_password
				
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
				Console_ResetColor();
				ScriptFinish(true);
				System.Environment.Exit(4);				
				}
			if (!File.Exists(srcds_folder+srcds_run))//File srcds.exe  exist
				{
				Console.ForegroundColor=ConsoleColor.Red;
				Console.WriteLine("ERR: File {0} doesn't exist in folder {1} not found.",srcds_run,srcds_folder);
				Console_ResetColor();
				ScriptFinish(true);
				System.Environment.Exit(4);				
				}
			if (!Directory.Exists(srcds_folder+srcds_mod))//Mod folder exist
				{
				Console.ForegroundColor=ConsoleColor.Red;
				Console.WriteLine("ERR: Folder {0} not found in {1}.",srcds_mod,srcds_folder);
				Console_ResetColor();
				ScriptFinish(true);
				System.Environment.Exit(4);				
				}			
			//
			// Check IP:PORT
			//			
			IPEndPoint BusySocket=NetIO.BusyTCPSocket(srcds_port,srcds_ip);
			while (BusySocket!=null)
				{
				ReadyToRun=false;				
				Console.ForegroundColor=ConsoleColor.Yellow;
    			Console.Write("Socket TCP {1}:{0} are busy ",BusySocket.Port,BusySocket.Address);        		
    			int PID = NetIO.GetPortProcessID(BusySocket.Port);    			
    			if (PID!=0)
	    			{
    				Console.Write("by PID {0} ",PID);
    				string socket_process_name=Process.GetProcessById(PID).ProcessName;
    				Console.WriteLine("of process {0}",socket_process_name);
    				if (socket_process_name!="srcds")
    				{
	    				Console.ForegroundColor=ConsoleColor.Red;
	        			Console.WriteLine("Socket process is not srcds. Starter will close.");
						ScriptFinish(true);
	        			System.Environment.Exit(5);        			
    				}
    				int ParentPID = (int)K64t.ProcessUtil.GetParentProcessID(PID);
    				if (!K64t.ProcessUtil.IsProcessRunning(ParentPID))ParentPID=0;
    				if (ParentPID!=0)
    					{
    					Console.Write("Socket process has parent PID {0}",ParentPID);
    					socket_process_name=Process.GetProcessById((int)ParentPID).ProcessName;
    					Console.Write(" process {0}.",socket_process_name);
    					//Проверить что родительский процесс вообще запущен
    					if (socket_process_name=="srcdsStarter")
    						{
    						Console.WriteLine("Try to kill old srcdsStarter.");
    						Process.GetProcessById((int)ParentPID).Kill();
    						}
    					}
			    		//Close srcds over RCON		
			    		/*
			 			Console.ForegroundColor=ConsoleColor.Cyan;
			 			Console.WriteLine("Try to close srcds over rcon.");
			    		SourceRcon.SourceRcon RCon = new SourceRcon.SourceRcon();
						RCon.Errors += new SourceRcon.StringOutput(ErrorOutput);
						RCon.ServerOutput += new SourceRcon.StringOutput(ConsoleOutput);			
						try 
						{
							RCon.Connect(new IPEndPoint(srcds_ip, srcds_port),srcds_rcon_password);
							for (int i=0;i!=10;i++)
							{
							Thread.Sleep(1000);
							if (RCon.Connected) break;
							}
							
						}
						catch (Exception e)
						{
							Console.WriteLine(e.Message.ToString());
						}
						if (RCon.Connected)
					    {
						 	Console.ForegroundColor=ConsoleColor.Green;								
							Console.WriteLine("Connected");Console_ResetColor();
							
							try {
							RCon.ServerCommand("quit");
							}catch (Exception e){Console.ForegroundColor=ConsoleColor.Red;Console.WriteLine(e.Message);}
							Thread.Sleep(100);
						}
						else
						{
							Console.ForegroundColor=ConsoleColor.Red;
							Console.WriteLine("ERR: No connection.");
							Console_ResetColor();
						}			
						Thread.Sleep(1000);
						RCon=null;
						*/				
					//Trying to close srcds process
					Console.WriteLine("Closing srcds process with PID {0}...",PID);
					Process.GetProcessById((int)PID).Kill();
					Thread.Sleep(1000);
					if (K64t.ProcessUtil.IsProcessRunning(PID))
						Console.WriteLine("ERR");
					else	
						Console.WriteLine("OK");
	    			}
    			else    				
    				{
        			Console.WriteLine("by unknown process.");
        			Console.ForegroundColor=ConsoleColor.Red;
        			Console.WriteLine("Starter will close.");
        			ScriptFinish(true);
        			System.Environment.Exit(5);
    				}
    			
    			Thread.Sleep(1000);
    			BusySocket=	NetIO.BusyTCPSocket(srcds_port,srcds_ip);
				}
			ReadyToRun=true;
			/*BusySocket=	BusyUDPSocket();
			if (BusySocket!=null)
				{
				Console.ForegroundColor=ConsoleColor.Yellow;
    			Console.WriteLine("Socket UDP {1}:{0} are busy. Trying to close process",BusySocket.Port,BusySocket.Address);
        		ReadyToRun=false;
				}*/
				
			
			
			//
			// Run srcds.exe
			//
			Console.Title = title + " " + srcds_name + " "+DateTime.Now.ToString();
			if (ReadyToRun)
			{
		
			Process srcds = new Process();
			srcds.StartInfo.RedirectStandardOutput = true;
			srcds.StartInfo.RedirectStandardError = true;
			srcds.StartInfo.CreateNoWindow = true;
			srcds.StartInfo.WorkingDirectory=srcds_folder;			
			srcds.StartInfo.FileName = srcds_folder + srcds_run;			
			srcds.StartInfo.UseShellExecute=false;	//https://msdn.microsoft.com/ru-ru/library/system.diagnostics.processstartinfo.workingdirectory(v=vs.110).aspx			
			srcds.StartInfo.Arguments+=" -game "+srcds_mod;
			srcds.StartInfo.Arguments+=" +ip "+srcds_ip;
			srcds.StartInfo.Arguments+=" -port "+srcds_port;
			srcds.StartInfo.Arguments+=" +hostname "+srcds_name;			
			//srcds.StartInfo.Arguments+=" -nocrashdialog -nomaster -console -insecure +sv_lan 1 "+srcds_cmd;
			srcds.StartInfo.Arguments += srcds_cmd;
			srcds.StartInfo.Arguments += " " + srds_default_command_line_params;
			srcds.StartInfo.UseShellExecute = false;
			#if (bbDEBUG)
				srcds.StartInfo.RedirectStandardOutput = true;
			#else	
				srcds.StartInfo.RedirectStandardOutput = false;
			#endif	
			while (true)
			{
				Console.ForegroundColor=ConsoleColor.White;
				Console.Write("{1} \nRun {0} ",srcds_run,DateTime.Now);
				Console_ResetColor();
				Console.WriteLine(srcds.StartInfo.Arguments);
				try 
					{					
				        srcds.Start();
	            	}        	
	        	catch (Exception e)
		        	{
		        		Console.ForegroundColor=ConsoleColor.Red;	        	    
		        	    Console.WriteLine(e.Message);
		        	    Console_ResetColor();
		        	}
#if (bbDEBUG)
				string output =srcds.StandardOutput.ReadToEnd();  
				string err =srcds.StandardError.ReadToEnd();	
				Console.WriteLine(output);			
				Console.WriteLine(err);			
#endif
					IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
					ShowWindow(handle, 6);
					

					srcds.WaitForExit();
	        	if (srcds.ExitCode>0) 
		        	{
		        	Console.ForegroundColor=ConsoleColor.Red;
		        	Console.WriteLine("ERRORLEVEL "+srcds.ExitCode);
		        	Console_ResetColor();
		        	}
				}
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
			Console.Write("Press any key to exit . . . ");Console_ResetColor();
			DateTime timeoutvalue = DateTime.Now.AddSeconds(10);
			while (DateTime.Now < timeoutvalue)
				{
			    if (Console.KeyAvailable) break;
			      Thread.Sleep(100);
			    }
			}
		}		
		

		/* Это от RCON static void ErrorOutput(string input){			Console.WriteLine("Error: {0}", input);		}
		//****************************************************	
		static void ConsoleOutput(string input)	{			Console.WriteLine("Console: {0}", input);		}*/
		
	}

}