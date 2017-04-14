
rem @echo off

set DEFHOMEDRIVE=d:
set DEFHOMEDIR=%DEFHOMEDRIVE%%HOMEPATH%
set HOMEDIR=
set HOMEDRIVE=%CD:~0,2%

set RELEASEDIR=d:\Users\jbb\release
set ZIP="c:\Program Files\7-zip\7z.exe"
echo Default homedir: %DEFHOMEDIR%

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

mkdir %HOMEDIR%\install\GameData\PersistentDynamicPodNames
mkdir %HOMEDIR%\install\GameData\PersistentDynamicPodNames\Textures
mkdir %HOMEDIR%\install\GameData\PersistentDynamicPodNames\PluginData
mkdir %HOMEDIR%\install\GameData\PersistentDynamicPodNames\Plugins


del /q %HOMEDIR%\install\GameData\PersistentDynamicPodNames\*
del /q %HOMEDIR%\install\GameData\PersistentDynamicPodNames\Textures\*


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
