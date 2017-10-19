Rem unusable line

@echo off
set H=R:\KSP_1.3.1_dev
echo %H%


set d=%H%
if exist %d% goto one
mkdir %d%
:one
set d=%H%\Gamedata
if exist %d% goto two
mkdir %d%
:two
set d=%H%\Gamedata\PersistentDynamicPodNames
if exist %d% goto three
mkdir %d%
:three
set d=%H%\Gamedata\PersistentDynamicPodNames\Plugins
if exist %d% goto four
mkdir %d%
:four
set d=%H%\Gamedata\PersistentDynamicPodNames\Textures
if exist %d% goto five
mkdir %d%
:five
set d=%H%\Gamedata\PersistentDynamicPodNames\PluginData
if exist %d% goto six
mkdir %d%
:six

d:
cd D:\Users\jbb\github\PersistentDynamicPodNames

copy /Y "Source\bin\Debug\PersistentDynamicPodNames.dll" "%H%\GameData\PersistentDynamicPodNames\Plugins"
 
copy /Y "PDPN_Default_Templates.cfg" "%H%\GameData\PersistentDynamicPodNames\PluginData"
copy /Y "PDPN_Settings.cfg" "%H%\GameData\PersistentDynamicPodNames\PluginData"
copy /Y "PersistentDynamicPodNamesMM.cfg" "%H%\GameData\PersistentDynamicPodNames"

copy /Y Textures\* "%H%\GameData\PersistentDynamicPodNames\Textures"
 

rem copy /Y "%~dp0PersistentDynamicPodNames.version" "%H%\GameData\PersistentDynamicPodNames"
rem copy /Y "%~dp0License.txt" "%H%\GameData\PersistentDynamicPodNames"
rem copy /Y "%~dp0README.md" "%H%\GameData\PersistentDynamicPodNames"

