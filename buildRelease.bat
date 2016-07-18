
rem @echo off

set DEFHOMEDRIVE=d:
set DEFHOMEDIR=%DEFHOMEDRIVE%%HOMEPATH%
set HOMEDIR=
set HOMEDRIVE=%CD:~0,2%

set RELEASEDIR=d:\Users\jbb\release
set ZIP="c:\Program Files\7-zip\7z.exe"
echo Default homedir: %DEFHOMEDIR%

set /p HOMEDIR= "Enter Home directory, or <CR> for default: "

if "%HOMEDIR%" == "" (
set HOMEDIR=%DEFHOMEDIR%
)
echo %HOMEDIR%

SET _test=%HOMEDIR:~1,1%
if "%_test%" == ":" (
set HOMEDRIVE=%HOMEDIR:~0,2%
)

d:
cd D:\Users\jbb\github\PersistentDynamicPodNames

type PersistentDynamicPodNames.version
set /p VERSION= "Enter version: "


mkdir %HOMEDIR%\install\GameData\PersistentDynamicPodNames
mkdir %HOMEDIR%\install\GameData\PersistentDynamicPodNames\Textures
mkdir %HOMEDIR%\install\GameData\PersistentDynamicPodNames\PluginData
mkdir %HOMEDIR%\install\GameData\PersistentDynamicPodNames\Plugins


del /y %HOMEDIR%\install\GameData\PersistentDynamicPodNames
del /y %HOMEDIR%\install\GameData\PersistentDynamicPodNames\Textures


copy /Y "%~dp0bin\Release\PersistentDynamicPodNames.dll" "%HOMEDIR%\install\GameData\PersistentDynamicPodNames\Plugins"

copy /Y "PDPN_Default_Templates.cfg" "%HOMEDIR%\install\GameData\PersistentDynamicPodNames\PluginData"
copy /Y "%~dp0PDPN_Settings.cfg" "%HOMEDIR%\install\GameData\PersistentDynamicPodNames\PluginData"
copy /Y "%~dp0PersistentDynamicPodNamesMM.cfg" "%HOMEDIR%\install\GameData\PersistentDynamicPodNames"
copy /Y "%~dp0SampleTemplates.txt" "%HOMEDIR%\install\GameData\PersistentDynamicPodNames"

copy /Y Textures\* "%HOMEDIR%\install\GameData\PersistentDynamicPodNames\Textures"

copy /Y "PersistentDynamicPodNames.version" "%HOMEDIR%\install\GameData\PersistentDynamicPodNames"

copy /Y "License.txt" "%HOMEDIR%\install\GameData\PersistentDynamicPodNames"
copy /Y "README.md" "%HOMEDIR%\install\GameData\PersistentDynamicPodNames"
copy /Y MiniAVC.dll  "%HOMEDIR%\install\GameData\PersistentDynamicPodNames"
copy /Y ModuleManager.2.6.25.dll  "%HOMEDIR%\install\GameData\PersistentDynamicPodNames"
copy /Y AdditionalCreditsAndLicenses.txt  "%HOMEDIR%\install\GameData\PersistentDynamicPodNames"





%HOMEDRIVE%
cd %HOMEDIR%\install

set FILE="%RELEASEDIR%\PersistentDynamicPodNames-%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% Gamedata\PersistentDynamicPodNames
