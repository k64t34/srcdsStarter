//-> Check that this host has correct ip,  asighted in  cmd params
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
using System.Net.Sockets;
using System.Timers;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections;
using System.ComponentModel;

namespace srcdsStarter
{
	class Program		
	{
		//Global
		static string srcds_name="DOD2018";  
		static string srcds_folder="                 f:\\soft\\Games\\Steam\\srcds.dod.07122018             ";
		static string srcds_mod="dod";
		static System.Net.IPAddress  srcds_ip=System.Net.IPAddress.Parse("10.80.68.220");
		static int srcds_port=27015;
		static string srcds_cmd="";
		//static string srcds_rcon_password="JA2PI";
		
		static string srcds_run="srcds.exe";	
		static bool ReadyToRun=true; //Flag that condition, enironment, port, ip ready to start srcds.exe
			
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
			//
			// Check IP:PORT
			//
			
			IPEndPoint BusySocket=BusyTCPSocket();
			while (BusySocket!=null)
				{
				ReadyToRun=false;				
				Console.ForegroundColor=ConsoleColor.Yellow;
    			Console.Write("Socket TCP {1}:{0} are busy ",BusySocket.Port,BusySocket.Address);        		
    			int PID = GetPortProcessID(BusySocket.Port);    			
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
    				uint ParentPID = K64t.ProcessUtil.GetParentProcessID(PID);
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
    				//.Trying to close process PID {0}
    				//if 
    				//uint ParentPID = K64t.ProcessUtil.GetParentProcessID(PID);
    			    //Console.WriteLine("parentPID=",ParentPID);
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
    			BusySocket=	BusyTCPSocket();
				}
			
			BusySocket=	BusyUDPSocket();
			if (BusySocket!=null)
				{
				Console.ForegroundColor=ConsoleColor.Yellow;
    			Console.WriteLine("Socket UDP {1}:{0} are busy. Trying to close process",BusySocket.Port,BusySocket.Address);
        		ReadyToRun=false;
				}
				
			
			
			//
			// Run srcds.exe
			//
			Console.Title = title + " " + srcds_name + " "+DateTime.Now.ToString();
			if (ReadyToRun)
			{			
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
		public static string GetPortPID(int Port){
		string procName="";
		//try { procName = Process.GetProcessById(pid).ProcessName; } 
		//catch (Exception) { procName = "-";}
		return procName;
		}
		//****************************************************	
		public static IPEndPoint BusyTCPSocket(){			
		//****************************************************
		IPEndPoint result = null;
		IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
		IPEndPoint[]  endPoints = (properties.GetActiveTcpListeners());
		int i_max=endPoints.Length;
		for (int i=0;i!=i_max;i++)
			{
			if (endPoints[i].Port==srcds_port)
				{
				if (endPoints[i].Address.Equals(srcds_ip) || endPoints[i].Address.Equals(System.Net.IPAddress.Parse("0.0.0.0")))
    				{
					result=endPoints[i];
    				break;
    				}
				}
			}
		return result;
		}
		//****************************************************	
		public static IPEndPoint BusyUDPSocket(){			
		//****************************************************
		IPEndPoint result = null;
		IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
		IPEndPoint[]  endPoints = (properties.GetActiveUdpListeners());
		int i_max=endPoints.Length;
		for (int i=0;i!=i_max;i++)
			{
			if (endPoints[i].Port==srcds_port)
				{
				if (endPoints[i].Address.Equals(srcds_ip) || endPoints[i].Address.Equals(System.Net.IPAddress.Parse("0.0.0.0")))
    				{
					result=endPoints[i];
    				break;
    				}
				}
			}
		return result;
		}
		//****************************************************	
		public static int GetPortProcessID(int Port){			
		//****************************************************
		//C# Sample to list all the active TCP and UDP connections using Windows Form appl
		//https://code.msdn.microsoft.com/windowsdesktop/C-Sample-to-list-all-the-4817b58f/view/Discussions#content			
		//Build your own netstat.exe with c#
		//https://timvw.be/2007/09/09/build-your-own-netstatexe-with-c/
		int result=0;
		Process netstat = new Process();
		netstat.StartInfo.RedirectStandardOutput = true;
		netstat.StartInfo.RedirectStandardError = true; //ComSpec=C:\Windows\system32\cmd.exe
		netstat.StartInfo.CreateNoWindow = true;
		netstat.StartInfo.WorkingDirectory="C:\\Windows\\system32\\";	//SystemRoot=C:\Windows, windir	c:\Windows\System32\findstr.exe c:\Windows\System32\netstat.exe	
		netstat.StartInfo.FileName = netstat.StartInfo.WorkingDirectory + "cmd.exe";			
		netstat.StartInfo.UseShellExecute=false;	//https://msdn.microsoft.com/ru-ru/library/system.diagnostics.processstartinfo.workingdirectory(v=vs.110).aspx			
		//ok netstat.StartInfo.Arguments+="/C netstat -nao | findstr 27015";
		netstat.StartInfo.Arguments+="/Q /C FOR /F \"tokens=5\" %p IN ('netstat -nao ^| findstr /i LISTENING ^| findstr 27015') do echo %p ";			
		#if (DEBUG)
		Debug.WriteLine("GetPortProcessID WorkingDirectory={0}",netstat.StartInfo.WorkingDirectory);
		Debug.WriteLine("GetPortProcessID FileName={0}",netstat.StartInfo.FileName);
		Debug.WriteLine(netstat.StartInfo.Arguments);				
		#endif	
		try 
			{					
		        netstat.Start();
        	}        	
    	catch (Exception e)
        	{
        		Console.ForegroundColor=ConsoleColor.Red;	        	    
        	    Console.WriteLine(e.Message);
        	    Console.ResetColor();
        	}
    	
		string output =netstat.StandardOutput.ReadToEnd();  
		string err =netstat.StandardError.ReadToEnd();	
		#if (DEBUG)
		Debug.WriteLine(output);			
		Debug.WriteLine(err);			
		#endif
    	netstat.WaitForExit();
    	if (netstat.ExitCode>0) 
        	{
    		#if (DEBUG)
        	Console.ForegroundColor=ConsoleColor.Red;
        	Console.WriteLine("ERRORLEVEL "+netstat.ExitCode);
        	Console.ResetColor();
        	#endif
        	}
    	else
    		result=Int32.Parse(output);
		return result;
		}
		
	}

}