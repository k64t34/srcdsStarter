start "Start DOD srcds" /D "%~d0%~p0"  srcdsStarter.exe  "DoDs-2021" C:\PROGRA~1\srcds.dod.2021 dod 172.19.1.44 27015 ^
 " -condebug -dev +sv_pure 1 +sv_voicecodec vaudio_celt"
rem 01092021 " -condebug -silent -dev -maxplayers 32 +sv_pure 1 +sv_voicecodec vaudio_celt" 
goto :EOF
rem  -insecure-steam -silent -console -game dod +sv_lan 1 -ip 172.19.1.44

https://developer.valvesoftware.com/wiki/Command_Line_Options#Command-line_parameters_5

-silent		Suppresses the dialog box that opens when you start steam. It is used when you have Steam set to auto-start when your computer turns on. (Steam must be off for this to work).
-dev		Show developer messages.
-maxplayers 32
+sv_pure 1
+sv_voicecodec vaudio_celt
-condebug	Logs all console output into the console.log text file.

srcdsStarter.exe always run srcds with following parameters:

-nocrashdialog	Stop some windows crash message boxes from showing up. 
-nomaster		Hides server from master serverlist.
-insecure 		Disable Valve Anti Cheat (VAC).
+sv_lan 1 
-silent 		Suppresses the dialog box that opens when you start steam. It is used when you have Steam set to auto-start when your computer turns on. (Steam must be off for this to work).
-maxplayers 32 
+sv_pure 1
-console		SrcDS will run in console mode (Windows only).

***************************************************
Start Source dedicated server ver 2.2021.4.16:
***************************************************
Usage: srcdsStarter <Server_name> <path> <mod> <ip> <port> <cmd> [<rcon_password>]
