Mod name:
Persistent Dynamic Pod name

One required cfg option: NameFormat
mod ships with default file


Ship names in-game:
	text							as is
  	text surrounded by {braces}		template name

format:
	names surrounded by [brackets] are replaced immediately.  
	names surrounded by <angle brackets> are replaced after launch & at staging

	names surrounded by (parentheses) are a user-inputed value, selected from values defined in cfg file


Each cfg section contains: cfgName, followed by multiple options:
		option name = option descr
		
Special formats:
	launch date/time options (replaced after launch)
	launch counter (tied to overall cfg)

In-flight options
	right-click to bring up dialog
	dialog will allow name to be auto-changed according to format rules
yet to be specified

--------------------------------------------------

launchcount (of ship, based on template after launch)
launch location
manned/unmanned

launch Date/time fields
	date - real date formatted as:  yyyy-MM-dd--HH-mm-ss
	day - 2 digit
	year2
	year3
	year4
	ordinalday (days since game started)

revision code(?) from crafthistory
numstages 						StageManager.StageCount()
curstagenum (indicating current stage #?) 	StageManager.CurrentStage()

Use GuiLayout.SelectionGrid for the multiple-choice entries:

					GUILayout.BeginVertical();
					generateButtonText = "What should we call this?";
					GUILayout.Label("Language: ", GUILayout.ExpandWidth(true));
					String[] langOpts = {"English", "Kerbal", "Both"};
					_lang = GUILayout.SelectionGrid(_lang, langOpts, 1, "toggle", GUILayout.ExpandWidth(true));
					GUILayout.Space(16);
					GUILayout.EndVertical();
				

types of power in current stage - # following would indicate how many of that kind of engine in the current stage
	liquid
	solid 
	mono
	xenon?
	nuclear?
VesselType
	cargo = C - needs cargo bay
	spaceplane = SP - needs rockets and wings
	Disposable_Drone = DD - unmanned, no chutes
	Reusable_Drone = RD - unmanned, chutes
	Crew_Transfer = CT - non-command crew capacity
	Tanker = T - tank capacity > ?% of ship
	Station = S - no engines
	Tug = Tg -
	Compound_Vehicle = CV
	Rover = Rv - needs wheels

weight class - light, medium, heavy
	light = 5
	medium=100
	heavy = 250

prefix (set in VAB)
========================================================

KSP Naming Formats
http://forum.kerbalspaceprogram.com/threads/135915-Does-anybody-here-use-their-own-system-of-aircraft-designation

Instell Inc. Aircraft Classification System

Closely based on the USA Tri-Service aircraft designation system.

Name format is [Prefix][VT][PM][SM][TM]-[Number][Name]

Vehicle type always goes before mission, prefix goes before everything.

Mission type isn't necessarily needed. For instance if you have a spaceplane that carries cargo to orbit, it would be fine to designate it "S-123" or "SC-123".

Status Prefix (ex: XF-23, YF-35B):
G=Permanently Grounded (Probably won't be used in a game like KSP.)
X=Experimental
Y=Prototype

Vehicle Type/Mission Type (Goes before secondary mission and number):
A=Attack (primarily air-to-ground combat)
B=Bomber (doesn't apply to fighter-bombers)
C=Cargo/Carrier
E=Electronic
F=Fighter (applies to fighter-bombers, multirole fighters, and some interceptors)
G=Glider
H=Helicopter
I=Interceptor (Fills a similar role to fighter. mission usually entails quick deployment to destroy a missile or other high-speed attack.)
K=Tanker (For refuelling)
L=Laser-equipped (Not applicable to Fighters or Attack aircraft equipped with laser targeting systems.)
M=Multi-mission (not applicable to multirole fighters. Think aircraft that would fit too many classifications to be easily explainable.)
O=Observation (Different from Reconnaissance.)
P=Patrol (Think aircraft that are built to fly for long periods of time to patrol an area, generally automated. Use kOS for this.)
Q=Unmanned Vehicle
R=Reconnaissance (similar to Observation, but generally more stealthy.)
S=Spaceplane
T=Trainer
U=Utility
V=VTOL aircraft
X=Special Research (If this applies, don't use the "X" Prefix, this is enough.)
Z=Lighter-Than-Air (pretty much exclusive to modded crafts.)

For secondary and tertiary missions just use the same classifications as above.

If aircraft has more than three classifications just use "M" to indicate a multi-mission aircraft.
========================================================
FRX[MY][RC][LSS]-[LBS]-[SBS]

[MY] = last 2 digits of model year

[RC] = revision code, omitted for the release-to-manufacturing model (revision zero).

[LSS] = Liquid Stack Specifier, syntax is L[CSS](.[CSS])... where
    [CSS] = Component Stage Specifier (order in LSS indicates staging)

[LBS] = Liquid Booster Specifier (also covers drop tanks)
    The syntax for this section isn't nailed down yet, I haven't really had a reason to build a FRX with liquid boosters or drop tanks.

[SBS] = Solid Booster Specifier, syntax is S[BSS].[BC] where
    [BSS] = Booster Size Specifier (03, 06, 12, 25, 37, and 50 are defined)
    [BC] = number of boosters of this type
========================================================
Three digits + Class Name
ex, 244 - Razor = Crew transfer SSTO with NTR for orbital maneuvering




>First - Purpose
0 Drone, Disposable
1 Drone, Reusable
2 Crew Transfer
3 Tanker
4 Station
5 Tug
6 Cargo
7 Carrier
9 Compound Vehicle


>Second - Propulsion
0 None
1 Chemical
2 Chemical Dual Mode
3 Nuclear Thermal
4 Chem-Nuclear Thermal Mixed
5 Chem Mixed
6 Mag Accelerated Ion
7 Nuclear Pulse
8 Nuclear Thermal Dual Mode
9 Physics Exploitation 


>Third - Operational Regime
0 Impactor
1 Surface Fixed
2 Non Atmospheric
3 Atmospheric
4 Transatmospheric
5 Interplanetary
6 Interstellar

========================================================
USM-1001-Mu

USM- Union Space Mission

1001- First mission of Year 1

-Mu- Mun mission, Kerbin missions have no suffix, others listed below:
Mi- Minmus
M- Moho
E- Eve system
Du- Duna system
D- Dres
J- Jool system
E- Eeloo
ARM- Asteroid Redirect Mission
K- Solar mission
R- Rescue mission e.g. Duna rescue mission will be USM-xxxx-R, not DuR.
========================================================
AB-C-DE-F Type H


A is the general purpose of the vehicle
C-Cargo
M-Military
T-Crew Transfer
E-Exploration

B is the subtype of the vehicle (grouped for convenience)
(C) Cargo Vessels:
U-Uranium (Does not include uraninite, only enriched and depleted uranium, enriched uranium is my primary source of income in KSP)
R-Rocket parts (not specific ones, just general ones)
O-Ore (not any specific ore, just general ores)
T-Tug (for colony buildings)
(M) Military Vessels:
B-Bombardment
DM-Defence, Mobile (located in space)
DS-Defence, surface (can be mobile or immobile)
T-Troop Transport
AS-Anti-Ship
(T) Crew Transfer VesselsSTS-Surface to Station
STP-Surface to planet
(E) Exploration Vessels)
P-Prospector (can find resources)
I-Interplanetary
R-Research (has science lab and equipment)
L-Lander

C stands for the type of navigation the vessel uses
A-Automated
M-Manual
AM-Both automated and manual control options

D stands for the engine type
N-Nuclear (Fission) Thermal
F-Nuclear Fusion
NP-Nuclear Pulse
C-Chemical
DC-Dual Mode Chemical
DN-Dual Mode Nuclear (Fission or Fusion)
E-Electric (Ion or something else)

E is the max Delta-V of the ship in a vacuum using it's main engine.

F is the number of kerbals that can be carried by the ship


H is a number that increases from one for every iteration with the same AB-CD-EF-G code

Any and all nicknames are added after the designation and in quotes, for example my orion drive battleship would be the:

MB-AM-NP<200 000-12 Type 3 "Duna"

EDIT:

Oh, and once the vessel is launched the stuff in quotes is dropped to be replaced by it's unique name and it gets a designation number which is the number of that vessel class launched before it, so if you were launching the 28th duna class vessel it would be the "MB-AM-NP<200 000-12 Type 3 28 Valance.
========================================================
ABC is the letter-based prefix for the craft. Here's the list of prefixes:

Aircraft
AC- Normal aircraft (ex. AC Hawk 1A)
ACX - Experimental or prototype aircraft. (ex. ACX Delta 2B)
FR - Fighter aircraft (ex. FR Eagle 3C)
BR - Bomber aircraft (ex. BR Condor 4D)
RC - Reconnaissance aircraft (ex. RC Falcon 5E)

Spacecraft
XSC - Experimental space craft (ex. XSC Arrow 1A)
CWT - Crew Transport (ex. CWT Hermes 2B)
CRT - Cargo Transport (ex .CRT Atlas 3C)
SSTO-(insert other prefix here) - SSTO which performs a function designated by the prefix (ex. SSTO-CT Pegasus 4D)
LDR - Lander (ex. LDR - Atlantis 5E (Mun)) (Note: Behind the usual suffix is denoted the intended body for the lander to land on in brackets)
MPCV - Multi-Purpose Crew Vehicle (ex. MPCV - Minerva 3C) (MPCVs can serve as CWTs or CRTs; however, they tend to have much more d/v to visit farther places)
IEV - Interplanetary Exploration Vehicle (ex. IMV Ares I) (NOTE: These usually don't have a suffixed iteration - instead, they have a suffix consisting of Roman numerals denoting the mission)
STN - Space Station (ex. STN - Copernicus (Kerbin) (NOTE: Instead of the suffixed iteration, the suffix denotes the body the station is in orbit of in brackets)
LBS - Land Base (ex. LBS - Centaur (Mun-1)) (NOTE: Instead of the suffixed iteration, the suffix denotes the body the base is on as well as the numerical order in which it was landed, 1 being the first base on said planet)
RVR - Rover (ex. RVR - Odyssey (Duna)) (NOTE: Instead of the suffixed iteration, the suffix denotes the body the rover is on)
========================================================
My designation system is as follows:

Special Designations:

X - Experimental
Y - Prototype
U - Unmanned


Propulsion:

J - Jet
P - Prop
R - Rotor (Helicopter)

Type:

HF - Heavy Freighter
MF - Medium Freighter
LF - Light Freighter
TF - Tanker Freighter (carries both cargo and fuel) 
CF - Converted Freighter (i.e, a converted passenger jet)

F - Fighter (Any interceptor, multi-role, fighter bomber, etc.)
B - Bomber
R - Reconnaissance 
K - Tanker (Different from TF as it only carries fuel)
T - Trainer
V - Crew transport
M - Medical Transport


So an experimental passenger jet carrying KSC staff would be XJV-whatever
========================================================

My sats are named like this:
[Purpose] Sat [Body] [Range] [Version]

Example: COM Sat Kerbin LR Mk1 which means, a communications satellite in Kerbin orbit with long range equipment version 1.
My polar satellites are always at least one range class above the aequatorial ones. So I don't need a distinction in the name here.

The "Purpose":
- COM = Communication
- SCAN = Survey scanner

The "Range":
- SSR = Super Short Range (Omni antenna only up to 5k km)
- SR = Short Range (Omni + Dishes <= 90k km)
- MR = Medium Range (Dishes <= 60M km)
- LR = Long Range (Dishes <= 400M km)

========================================================
In a normal game no, in RO/RSS/RP0 (Realism Overhaul and associated mods), my jet aircraft all have the prefix P-SF, such as P-SF-A1.

The P-SF stands for "Prototype - Sonic Flight" (which is probably incorrect terminology), but really I wanted a designation I could swap with "Probably shouldn't fly". A is the line, and 1 is the model. A line are twin engine craft named after cities, B line are single engine aircraft named after something to do with spinning around. 

P-SF-A1 "Kalcutta" (yeah, I did the K thing.)
P-SF-A2 "Delhi"

P-SF-B1 "Dervish"
P-SF-B2 "Vortex"
P-SF-B3 "Super Vortex"
========================================================
