#define MyAppName "Super Pete The Pirate"
#define MyAppVersion "1.0.3"
#define MyAppPublisher "Phantom Ignition"
#define MyAppExeName "Super Pete The Pirate.exe"
#define MyReleaseDir "..\Super Pete The Pirate\bin\DesktopGL\x86\Release"
#define MyDeployDir "Deploy"
#define MyNamespace "Super Pete The Pirate"
#define MyGuid "210141a5-42e8-4ffa-a785-e1ddbbce1242"
#define MyIcon "pete.ico"                
#define OpenAL "oalinst.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{{#MyGuid}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DisableProgramGroupPage=yes
;LicenseFile={#MyReleaseDir}\eula.txt
;InfoBeforeFile={#MyReleaseDir}\changelog.txt
OutputDir={#MyDeployDir} 
OutputBaseFilename={#MyNamespace}.{#MyAppVersion}.Windows.Setup
Compression=lzma
SolidCompression=yes
SetupIconFile={#MyIcon}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]                             
Source: {#OpenAL}; DestDir: {tmp}
Source: "{#MyReleaseDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{commonprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]                                                    
Filename: "{tmp}\{#OpenAL}"; Parameters: "/s"; StatusMsg: "Installing dependencies...";
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

