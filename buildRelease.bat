
@echo off

set RELEASEDIR=d:\Users\jbb\release
set ZIP="c:\Program Files\7-zip\7z.exe"

d:
cd D:\Users\jbb\github\PersistentDynamicPodNames



set VERSIONFILE=PersistentDynamicPodNames.version
rem The following requires the JQ program, available here: https://stedolan.github.io/jq/download/
c:\local\jq-win64  ".VERSION.MAJOR" %VERSIONFILE% >tmpfile
set /P major=<tmpfile

c:\local\jq-win64  ".VERSION.MINOR"  %VERSIONFILE% >tmpfile
set /P minor=<tmpfile

c:\local\jq-win64  ".VERSION.PATCH"  %VERSIONFILE% >tmpfile
set /P patch=<tmpfile

c:\local\jq-win64  ".VERSION.BUILD"  %VERSIONFILE% >tmpfile
set /P build=<tmpfile
del tmpfile
set VERSION=%major%.%minor%.%patch%
if "%build%" NEQ "0"  set VERSION=%VERSION%.%build%

echo %VERSION%

mkdir GameData\PersistentDynamicPodNames
mkdir GameData\PersistentDynamicPodNames\Textures
mkdir GameData\PersistentDynamicPodNames\PluginData
mkdir GameData\PersistentDynamicPodNames\Plugins


copy /Y "%~dp0Source\bin\Release\PersistentDynamicPodNames.dll" "GameData\PersistentDynamicPodNames\Plugins"

copy /Y "PDPN_Default_Templates.cfg" "GameData\PersistentDynamicPodNames\PluginData"
copy /Y "PDPN_Settings.cfg" "GameData\PersistentDynamicPodNames\PluginData"
copy /Y "PersistentDynamicPodNamesMM.cfg" "GameData\PersistentDynamicPodNames"
copy /Y "SampleTemplates.txt" "GameData\PersistentDynamicPodNames"

copy /Y Textures\* "GameData\PersistentDynamicPodNames\Textures"

copy /Y "PersistentDynamicPodNames.version" "GameData\PersistentDynamicPodNames"

copy /Y "License.txt" "GameData\PersistentDynamicPodNames"
copy /Y "README.md" "GameData\PersistentDynamicPodNames"

copy /Y ..\MiniAVC.dll  "GameData\PersistentDynamicPodNames"
copy /Y ..\ModuleManager.2.8.1.dll  "GameData"
copy /Y AdditionalCreditsAndLicenses.txt  "GameData\PersistentDynamicPodNames"


set FILE="%RELEASEDIR%\PersistentDynamicPodNames-%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% Gamedata\PersistentDynamicPodNames
pause