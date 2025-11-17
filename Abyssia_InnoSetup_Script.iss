; Script généré pour Abyssia – Build final Unity
#define MyAppName "Abyssia"
#define MyAppVersion "1.0"
#define MyAppPublisher "Emilien OLIVAREZ, Inc."
#define MyAppURL "https://github.com/eolivarez2008/Abyssia"

[Setup]
AppId={{B06BEBDF-BF6B-41F4-8ED8-92EF9386B9C5}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
PrivilegesRequired=admin
OutputBaseFilename=Abyssia_v1_installer
SolidCompression=yes
WizardStyle=modern dynamic windows11

[Languages]
Name: "french"; MessagesFile: "compiler:Languages\French.isl"

[Files]
Source: "C:\Users\eoliv\Documents\Projets\Abyssia\Builds\Game 2D.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\eoliv\Documents\Projets\Abyssia\Builds\DirectML.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\eoliv\Documents\Projets\Abyssia\Builds\UnityPlayer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\eoliv\Documents\Projets\Abyssia\Builds\D3D12\*"; DestDir: "{app}\D3D12"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\eoliv\Documents\Projets\Abyssia\Builds\Game 2D_Data\*"; DestDir: "{app}\Game 2D_Data"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\eoliv\Documents\Projets\Abyssia\Builds\MonoBleedingEdge\*"; DestDir: "{app}\MonoBleedingEdge"; Flags: ignoreversion recursesubdirs createallsubdirs

[Tasks]
Name: "desktopicon"; Description: "Créer un raccourci sur le bureau"; GroupDescription: "Tâches supplémentaires"; Flags: unchecked

[Icons]
; Raccourci menu démarrer pour le jeu
Name: "{group}\{#MyAppName}"; Filename: "{app}\Game 2D.exe"
; Raccourci bureau
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\Game 2D.exe"; Tasks: desktopicon
; Raccourci vers le site web (optionnel)
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
; Raccourci désinstallation
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
