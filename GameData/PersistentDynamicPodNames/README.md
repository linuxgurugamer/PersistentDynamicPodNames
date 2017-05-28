Persistent Dynamic Pod Names

This mod has two related functions:
1.  Allow you to name each individual command pod in a vessel.  These names will be assigned to the entire vessel when the command pod becomes active
2.  Allow you to create one or more templates in a cfg file which can then be assigned to either a vessel and/or individual command pods.  The template is used when the vessel is launched (in the case where the template is assigned to the vessel in the editor), and when templates are assigned  to individual command pods, the template is processed when that command pod becomes active.

Files

PluginData/PDPN_Default_Templates.cfg	A file containing several simple templates as examples.  If no template file (next line) this file will be used
PluginData/PDPN_Templates.cfg			A text file created by the mod the first time it runs, and edited by the user to store templates
PluginData/PDPN_Settings.cfg			Currently stores a single setting regarding new templates.  This is a manually edit at this time.


Dialog Windows

There are several dialog windows used to control this mod.

Available Templates
This is accessed using the toolbar button (PDPN) while in the Space Center scene. This window will list all available templates, and allow you to specify which templates are active (active in this case means that it will appear in the template lists in the editors).
There are buttons which will allow you to enable or disable all of the templates.

[http://spacetux.net/images/PDPN/toolbar.png]
[http://spacetux.net/images/PDPN/availableTemplates.png]

--

PDPN Menu
There will be a PDPN button in the editor.  It has two functions.  Hovering the mouse above the button will display a menu with the following entries:

	Command Pods (opens a dialog)
	Set Priority to stage
	Set Priority to reverse stage

Clicking the button will bring up the Primary Ship Name Template Selection dialog
[http://spacetux.net/images/PDPN/editorMenu.png]

--

Primary Ship Name Template Selection
Accessed by clicking the PDPN button in the editor, this window presents a list of all active templates.  The selected template will be assigned to the vessel currently under construction.  After selection and closing the window, the template name will be put in the vessel name field at the top of the screen, with asterisks on either side (astericks are used to indicate it's a template).  If there is  nothing in the description field, a copy of the template will be put there.

[http://spacetux.net/images/PDPN/primaryShipNameTemplSel.png]

--

Command Pods
This dialog is accessed using the first menu item.  It displays a list of all the command pods (manned and unmanned) in the current vessel, in the priority order in which they will be used by this mod.  It displays each pod, the name or template selected, and what kind of vessel that command pod is set to.  Moving the mouse over each line will highlight the appropriate command pod.
At the right of each line will be an up and down arrow, this is used to move the pods up and down the priority list.
Double-clicking on an entry field  (either the name or the ship type) will bring up a pair of arrows (right/left) on each side of the field.  You can use those arrows to scroll through the available entries for that field;  name field will scroll through all active templates, ship type field will scroll through all the ship types.

[http://spacetux.net/images/PDPN/commandPods1.png]
[http://spacetux.net/images/PDPN/commandPods2.png]

--

Enter Persistent Pod Name
This dialog is accessed by right-clicking on a command pod and using the right-click menu item "Persistent pod name".  You can enter a sihp name, use the right/left arrows on either side of the entry field to scroll through the active templates, and select the vessel type by clicking the appropriate button.  This option is available in the flight scene as well.

[http://spacetux.net/images/PDPN/rightclick.png]
[http://spacetux.net/images/PDPN/enterPersistentPodName.png]

--

Template Layouts

A template can consist of the following types of information:

	Plain text, which is not touched and put into the final name exacty as entered
	[name]			The name will be processed and replaced immediately upon launch (or when the command pod becomes active)
	<name>			The name will be processed after launch and at every staging event
	(userDefName)	The userDefName is a user-selectedZ value.  The values will be defined in the cfg file
	#cntId#			This is a counter, which will start at 1 and increase by one every time a launch is done using this id.  You can have as many different counters as you like.  

The following example shows the syntax needed for defining a template.
All templates need to be specified using the "templateName".  In the example below, there are two templates defined, "exampleTemplate" and "Kepollo".
Then, the template is defined using the normal config file syntax
First line in the template is the template definition, followed by any special values.

The first template in the following example is a simple template called "exampleTemplate".
The template is defined as:  
	KSP-<year>:#usm#-(destination)
which is read as follows:
the <year> is replaced by the year of the launch
#usm# is a counter which increments by one for every launch
(destination) will have the user select a destination, defined below as planets

The second template, Kepollo, shows the use of one of the predefined name called "weightclass".  

	PersistentDynamicPodNames
	{
		templateName = exampleTemplate
		templateName = Kepollo

		exampleTemplate
		{
			template = KSP-<year>:#usm#-(destination)
			destination
			{
				values = planets
			}
		}

		Kepollo
		{
			template = Kepollo-[weightclass] #apolloCnt#

			weightclass
			{
				values = Light Intermediate Medium Heavy
				Light = 5
				Intermediate = 50
				Medium = 100
				Heavy = 250
			}
		}

	}

--

Special names

There are some special names known by the mod which have special meanings.  They are listed below, along with any explanation needed.

weightclass		Specifies a weight class the vessel is, based on total vessel mass.  The values are not fixed, you can specify as many as you like.  For each value specified, you need to put in a number which is the weight limit of that value.  An example definition is:

			# Weightclass, must be in order from lightest to heaviest.  Last one is the
			# catchall, for anything which is none of the earlier ones
			weightclass
			{
				values = Light Intermediate Medium Heavy
				Light = 5
				Intermediate = 50
				Medium = 100
				Heavy = 250
			}

All of the following special names have default values which can be replaced by defining the values you want in it's own section.  There are examples below for each of the special names listed below:

enginetype		A string of all enginetypes.  Enginetype is obtained from the game, and an abbreviation is created. Only one of each type will be added.  So for example, if a vessel has both solid and liquid fuel engines, a generated enginetype might be: 
fueltype		A string of all fuel types, ordered by engines/stages.
vesseltype		The vesseltype as defined by KSP
numstages		Number of stages in the vessel
curstagenum		Current stage number
propulsion		The type of propulsion
date			Game date
day				Day of year
year2			2 digit year
year3			3 digit year
year4			4 digit year
year
	
The following can only be used for a manual-inputted value
	destination		If nothing specified, will be all planets or moons.  
	planets			If nothing specified, then all planets
	moons			All moons in system

	# Weightclass, must be in order from lightest to heaviest.  Last one is the
	# catchall, for anything which is none of the earlier ones
	weightclass
	{
		values = Light Intermediate Medium Heavy
		Light = 5
		Intermediate = 50
		Medium = 100
		Heavy = 250
	}
	# fueltype is/are the type(s) of propellent(s) in the current stage.  The lines below the 
	# "values" line shows what will be replaced for each type;  ie:  a LiquidFuel engine would normall
	# default to "LF", but the below says to replace with "L"
	fueltype
	{
		values = LiquidFuel Oxidizer SolidFuel MonoPropellant XenonGas 
		LiquidFuel = L
		Oxidizer = O
		SolidFuel = S
		MonoPropellant = M
		XenonGas = X
	}

	vesseltype
	{
		values = Cargo SpacePlane Aircraft DisposableDrone ReusableDrone CrewTransfer Tanker Station Tug CompoundVehicle Rover
		Cargo = C 
		SpacePlane = SP
		Aircraft
		DisposableDrone = DD
		ReusableDrone = RD
		CrewTransfer = CT
		Tanker = T
		Station = S
		Tug = Tg
		CompoundVehicle = CV
		Rover = RV

	}

	# destination defaults to all planets
	# If values are specified, then only those are shown
	# special values:   planets moons all
	destination
	{
		values = planets
	#	values = Kerbin Mun Minmus Mono Eve Duna Dres Jool Eeloo AsteroidRedirectMission Solar Rescue
	#	Kerbin =
	#	Mun = Mu
	#	Minmus = Mi
	#	Mono = M
	#	Eve = E
	#	Duna = Du
	#	Dres = D
	#	Jool = J
	#	Eeloo = E
	#	AsteroidRedirectMission = ARM
	#	Solar = K
	}

	# Propulsion
	propulsion
	{
		values = Generic SolidBooster LiquidFuel Piston Turbine ScramJet Electric Nuclear MonoProp
		Generic = G
		SolidBooster = Srb
		LiquidFuel = L
		Piston = P
		Turbine = T
		ScramJet = SJ
		Electric = E
		Nuclear = N
		MonoProp = M
	}

	custom1
	{
		values = one two three four five
	}
	