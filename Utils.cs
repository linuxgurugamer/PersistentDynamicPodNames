using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Text.RegularExpressions;

//using KSP.UI;
//using KSP.UI.Util;
using KSP.UI.Screens;


namespace PDPN
{

	public static class Utils
	{
		public static Camera GetCurrentCamera ()
		{

			// man, KSP could really just use a simple "get whatever the current camera is" method:
			return HighLogic.LoadedSceneIsEditor ?
				EditorLogic.fetch.editorCamera : FlightCamera.fetch.mainCamera;
		}

		public static string GetAssemblyFileVersion ()
		{
			Assembly assembly = Assembly.GetExecutingAssembly ();

			System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo (assembly.Location);
			return fvi.FileVersion;            
		}

#if false
		List<Tuple<int, Part, PartModule>> sortByStageCommandPodList() {
			foreach (Part p in EditorLogic.fetch.ship.parts) {
				if (p.Modules.Contains (Constants.MODNAME)) {
					foreach (PartModule m in p.Modules) {
						if (m.moduleName == Constants.MODNAME)
							((PersistentDynamicPodNames)m).priority = p.inverseStage;
					}
				}
			}
		}

        public static void setPodVesselGuid (Vessel vessel, Guid id)
		{
			Log.Info ("setPodVesselGuid  id:" + id.ToString ());
			foreach (Part p in vessel.parts) {
				foreach (PartModule m in p.Modules) {
					if (m.moduleName == Constants.MODNAME) {
                        
						((PersistentDynamicPodNames)m).partOrigVesselGuid = id;
					}
				}
			}
		}

        public string Truncate(string source, int length)
        {
        	return ( source.Length > length ? source.Substring(0, length) : source);
        }
#endif

        public static PersistentDynamicPodNames getActiveCommandPodModule (List<Part> pl)
		{
			var acn = new PersistentDynamicPodNames ();
			if (pl == null)
				return null;
			
			acn.priority = -1;

           // List<Part> pl = Utils.getPartList(pm2);
            //for (int i = pl.Count - 1; i >= 0; --i)
            //{
                // Part part = pl[i];

             foreach (Part p in pl) {
				Log.Info ("getActiveCommandPodModule.getActiveCommandPodModule Part: " + p.name);
				foreach (PartModule m in p.Modules) {
					if (m.moduleName == Constants.MODNAME) {
						if (((PersistentDynamicPodNames)m).priority > acn.priority /* && ((PersistentDynamicPodNames)m).storedVesselName != "" */) {
							acn = ((PersistentDynamicPodNames)m);	
						}
					}
				}
			}
			return acn;
		}

		public static Part getActiveCommandPodPart (List<Part> pm2)
		{
			var acn = new PersistentDynamicPodNames ();
			Part lastPart = pm2.First ();
			acn.priority = -1;
            for (int i = pm2.Count - 1; i >= 0; --i) { 
                for (int i1 = pm2[i].Modules.Count - 1; i1 >= 0; --i1) {

                    PartModule m = pm2[i].Modules[i1];

                    if (m.moduleName == Constants.MODNAME) {
						if (((PersistentDynamicPodNames)m).priority > acn.priority && ((PersistentDynamicPodNames)m).storedVesselName != "") {
							acn = ((PersistentDynamicPodNames)m);
							lastPart = pm2[i];
						}
					}
				}
			}
			return lastPart;
		}

		public static Part getModulePartParent(PartModule pm) {
            Part lastPart = null;
            
            List<Part> plist = getPartList(pm);
            if (plist != null)
            {
                for (int i = plist.Count - 1; i >= 0; --i)
                {
                    PartModuleList pmoduleList = plist[i].Modules;
                    for (int i1 = pmoduleList.Count - 1; i1 >= 0; --i1)
                    {
                        if (pmoduleList[i1].moduleName == Constants.MODNAME)
                        {
                            lastPart = plist[i];
                            break;
                        }
                    }
                }
            }
			return lastPart;
		}


		public static Vessel getCurrentVessel (Part p)
		{
			if (p == null)
				Log.Info ("getCurrentVessel  p == null");
		
			return p.vessel;
		}

		public static Vessel getCurrentVessel (PartModule pm)
		{
			if (pm == null)
				Log.Info ("getCurrentVessel  pm == null");

			return pm.vessel;
		}

		public static List<Part> getPartList (PartModule pm)
		{
			if (HighLogic.LoadedSceneIsEditor) {
                if (EditorLogic.fetch.ship == null)
                    return null;
				return EditorLogic.fetch.ship.parts;
			} else {
				return getCurrentVessel (pm).parts;
			}
		}

		public static List<Tuple<int, Part, PartModule>> getSortedCommandPodList (List<Part> pl)
		{
			List<PDPN.Tuple<int, Part, PartModule>> podList = new List<PDPN.Tuple<int, Part, PartModule>> ();

           // List<Part> pl = Utils.getPartList(pm2);
            //for (int i = pl.Count - 1; i >= 0; --i)
            //{
                // Part part = pl[i];

             foreach (Part p in pl) {
				if (p.Modules.Contains (Constants.MODNAME)) {
					foreach (PartModule m in p.Modules) {
						if (m.moduleName == Constants.MODNAME) {
							podList.Add (new Tuple<int, Part, PartModule> (((PersistentDynamicPodNames)m).priority, p, m));
						}
					}
				}
			}
			return podList.OrderByDescending (o => o.First).ToList ();
		}

		public static List<Tuple<int, Part, PartModule>> getCommandPodList (List<Part> pl)
		{
			List<PDPN.Tuple<int, Part, PartModule>> podList = new List<PDPN.Tuple<int, Part, PartModule>> ();
			int lastpriority = 0;

           // List<Part> pl = parts;
            //for (int i = pl.Count - 1; i >= 0; --i)
            //{
                // Part part = pl[i];

              foreach (Part p in pl) {
				if (p.Modules.Contains (Constants.MODNAME)) {
					foreach (PartModule m in p.Modules) {
						if (m.moduleName == Constants.MODNAME) {
							if (((PersistentDynamicPodNames)m).priority == 0)
								((PersistentDynamicPodNames)m).priority = ++lastpriority;
							else if (((PersistentDynamicPodNames)m).priority > lastpriority)
								lastpriority = ((PersistentDynamicPodNames)m).priority;
							podList.Add (new Tuple<int, Part, PartModule> (((PersistentDynamicPodNames)m).priority, p, m));
						}
					}
				}
			}
			return podList;
		}
	
		// ===================================================
		public static KeyValuePair<string, PDPN.Tuple<string, NameValueCollection, bool>> GetFormat (string templateName)
		{
			Log.Info ("GetFormat templateName: " + templateName);
            if (templateName.Length <= 3)
            {
                Log.Error("templateName too short: [" + templateName + "]");

            }
            else
            {
                templateName = templateName.Substring(1, templateName.Length - 2);
                foreach (KeyValuePair<string, PDPN.Tuple<string, NameValueCollection, bool>> t in Constants.config.templates)
                {
                    if (t.Value.Third && t.Key == templateName)
                    {
                        return t;
                    }
                }
            }
			NameValueCollection pairs = new NameValueCollection ();
			return new KeyValuePair<string, PDPN.Tuple<string, NameValueCollection, bool>> ("", new PDPN.Tuple<string, NameValueCollection, bool> (templateName, pairs, true));
		}

		private static string ConvertDateString ()
		{
			string dateFormat = "yyyy-MM-dd--HH-mm-ss";

			return DateTime.Now.ToString (dateFormat);
		}

		private static int[] ConvertUT (double UT)
		{
			double time = UT;
			int[] ret = { 0, 0, 0, 0, 0 };

			ret [0] = (int)Math.Floor (time / (KSPUtil.dateTimeFormatter.Year)) + 1; //year
			time %= (KSPUtil.dateTimeFormatter.Year);
			ret [1] = (int)Math.Floor (time / KSPUtil.dateTimeFormatter.Day) + 1; //days
			time %= (KSPUtil.dateTimeFormatter.Day);
			ret [2] = (int)Math.Floor (time / (3600)); //hours
			time %= (3600);
			ret [3] = (int)Math.Floor (time / (60)); //minutes
			time %= (60);
			ret [4] = (int)Math.Floor (time); //seconds

			return ret; 
		}


		public static string formatName (PartModule pm2, string templateName, bool fillSelections = false)
		{
			Log.Info ("enter formatName:  [" + templateName + "]");
			if (templateName == null || templateName == "")
				return "";
            Log.Info("formatName 0  length: " + templateName.Length.ToString());
            if (templateName.Length < 3 || templateName [0] != '*' || templateName [templateName.Length - 1] != '*') {
				return templateName;
			}
			if (!fillSelections)
				PDPN_SelectionWindow.InitSelectionWindow ();

            int[] times = ConvertUT (Planetarium.GetUniversalTime ());

			KeyValuePair<string, PDPN.Tuple<string, NameValueCollection, bool>> t = GetFormat (templateName);
            Log.Info("formatName 3");

            string newVesselName = templateName;
			if (fillSelections || t.Key != "") {
				string template;
				if (fillSelections)
					template = templateName;
				else
					template = t.Value.Second.Get ("template");
				Log.Info ("formatName:  template: " + template);
				Log.Info ("flightStatus: " + PDPN_SelectionWindow.flightStatus.ToString ());
				newVesselName = template;
				string pattern = "";
				char prefix = ' ';
				char suffix = ' ';
				//	int cnt = 0;
				for (int p = 0; p < 2; p++) {
					switch (p) {
					case 0:
					// names surrounded by [brackets] are replaced immediately
						pattern = "\\[.*?\\]";
						prefix = '[';
						suffix = ']';
						break;
					case 1:				
						// names surrounded by <angle brackets> are replaced after launch & at staging
						pattern = "<.*?>";
						prefix = '<';
						suffix = '>';
						break;

					}
					Log.Info ("p: " + p.ToString () + "   pattern: " + pattern);
					Regex r = new Regex (pattern);

					Match m = r.Match (template);
					while (m.Success) {
						string id = m.Value.Substring (1, m.Value.Length - 2);
						string search = prefix + id + suffix;
						string replacement = "";
					
						Log.Info ("formatName  id: " + id + "   search: " + search);
						
						replacement = "";
						if (fillSelections) {
							for (int i = 0; i < PDPN_SelectionWindow.selTitleList.Count; i++) {
								if (PDPN_SelectionWindow.selTitleList [i] == id) {
									replacement = PDPN_SelectionWindow.selArList [i] [PDPN_SelectionWindow.selList [i]];
									Log.Info ("Final replacement: " + PDPN_SelectionWindow.selTitleList [i] + "  replacement: " + replacement);
									break;
								}
							}
						} else {
							// Format the replacement, then do the replace

							int stageCnt = StageManager.GetStageCount (Utils.getPartList (pm2));
							switch (id) {
							case "weightclass":

								replacement = getWeightClass (pm2, stageCnt - Utils.getCurrentVessel (pm2).currentStage, t);
								break;

							case "enginetype":
								replacement = getEngineTypes (pm2, stageCnt - Utils.getCurrentVessel (pm2).currentStage, t);
								break;

							case "fueltype":
								replacement = getPropellentTypes (pm2, stageCnt - Utils.getCurrentVessel (pm2).currentStage, t);
								break;

							case "vesseltype":
								replacement = getVesselType (pm2, stageCnt - Utils.getCurrentVessel (pm2).currentStage, t);
								break;

							case "numstages":
								replacement = stageCnt.ToString ();
								break;

							case "curstagenum":
								replacement = Utils.getCurrentVessel (pm2).currentStage.ToString ();
								break;

							case "propulsion":
								replacement = getPropulsion (pm2, stageCnt - Utils.getCurrentVessel (pm2).currentStage, t);
								break;

							case "date":
								replacement = ConvertDateString ();
								break;
							case "day":
								replacement = times [1].ToString ("D2");
								break;
							case "year2":
								replacement = times [0].ToString ("D2");
								break;
							case "year3":
								replacement = times [0].ToString ("D3");
								break;
							case "year":
								replacement = times [0].ToString ("D");
								break;

							case "year4":
								replacement = times [0].ToString ("D4");
								break;
							//case "ordinalday":
							//	break;

							default:

								break;

							}
						}

						if (replacement != "")
							newVesselName = newVesselName.Replace (search, replacement);

						m = m.NextMatch ();
					}

				}
			}
			Log.Info ("formatname vesselName: " + newVesselName);
			return newVesselName;
		}

		// if s is blank, scan name for cap letters, and use those for abbr.  If no caps
		// then use first 2 letters
		static string generateDefault (string str)
		{
			string r = "";
			foreach (char c in str) {
				if (char.IsUpper (c))
					r += c;
			}
			if (r == "")
				r = str.Substring (0, 2);
			return r;
		}

		static public string getWeightClass (PartModule pm2, int stage, KeyValuePair<string, PDPN.Tuple<string, NameValueCollection, bool>> t)
		{
			string replacement = "";
			float f = Utils.getCurrentVessel (pm2).GetTotalMass ();
			foreach (string s in t.Value.Second.AllKeys.Where(s=> s.StartsWith("weightclass"))) {
				if (s != "weightclass:values") {
					Log.Info ("weightclass: " + s + "   value: " + t.Value.Second.Get (s));
					replacement = s.Substring (s.IndexOf (':') + 1);
					if (float.Parse (t.Value.Second.Get (s)) > f) {
						Log.Info ("weightclass replacement: " + replacement);
						break;
					}
				}
			}
			return replacement;
		}

		static public string  getEngineTypes (PartModule pm2, int stage, KeyValuePair<string, PDPN.Tuple<string, NameValueCollection, bool>> t)
		{
			string enginetypeabbr = "NoEngine";
			Log.Info ("getEngineTypes");

           // List<Part> pl = Utils.getPartList(pm2);
            //for (int i = pl.Count - 1; i >= 0; --i)
            //{
                // Part part = pl[i];

              foreach (Part p in Utils.getPartList(pm2)) {
				if (p.Modules.Contains<ModuleEngines> ()) {
					foreach (PartModule pm in p.Modules) {
						Log.Info ("pm: " + pm.moduleName);
						if (pm.moduleName == "ModuleEngines") {
							ModuleEngines engine = pm as ModuleEngines;
							// Add a colon following if there is already an engine found
							//							if (enginetypeabbr.Length > 0 && enginetypeabbr [enginetypeabbr.Length - 1] != ':')
							//								enginetypeabbr += ':';
							foreach (Propellant propellant in engine.propellants) {
								Log.Info (p.partName + " uses " + propellant.name);

							//	string s = t.Value.Second.Get ("enginetype:" + propellant.name);
							//	Log.Info (propellant.name + ": " + propellant.name + "   value: " + s);
								// if s is blank, scan name for cap letters, and use those for abbr.  If no caps
								// then use first 2 letters
							//	string replacement = s;

							//	Log.Info (propellant.name + " replacement: " + replacement);

								string enginetype = engine.GetEngineType ().ToString ();

								string abbr = generateDefault (enginetype);

								if (enginetypeabbr == "NoEngine")
									enginetypeabbr = "";
								// Only add if it isn't in there already
								//if (abbr.Length < enginetypeabbr.Length)
								if ((abbr.Length > enginetypeabbr.Length) || !enginetypeabbr.Contains (':' + abbr + ':') && !(enginetypeabbr.Substring (0, abbr.Length) + ':' == abbr + ':'))
									enginetypeabbr += abbr;

								Log.Info (p.partName + " enginetype: " + engine.GetEngineType ().ToString () + " " + enginetype + " " + enginetypeabbr);
							}
						}
					}
				}
			}
			Log.Info ("enginetypeabbr: " + enginetypeabbr);
			if (enginetypeabbr.Length > 0 && enginetypeabbr [enginetypeabbr.Length - 1] == ':')
				enginetypeabbr.Remove (enginetypeabbr.Length - 1);
			return enginetypeabbr;

		}

		static public string  getPropellentTypes (PartModule pm2, int stage, KeyValuePair<string, PDPN.Tuple<string, NameValueCollection, bool>> t)
		{
			string propellenttypeabbr = "";
			Log.Info ("getPropellentTypes");
			if (Utils.getCurrentVessel (pm2) == null) {
				Log.Info ("currentVessel is null");
				return "";
			}

           // List<Part> pl = Utils.getPartList(pm2);
            //for (int i = pl.Count - 1; i >= 0; --i)
            //{
                // Part part = pl[i];

              foreach (Part p in Utils.getPartList(pm2)) {
				if (p.Modules.Contains<ModuleEngines> ()) {
					//Log.Info ("Contains: Part: " + part.ToString() + " name: " + part.name + "   Stage: " + part.inverseStage.ToString () + "  currentStage: " + stage.ToString());

					foreach (PartModule pm in p.Modules) {
						if (pm.moduleName == "ModuleEngines") {
							ModuleEngines engine = pm as ModuleEngines;
							// Add a colon following if there is already an engine found
							if (propellenttypeabbr.Length > 0 && propellenttypeabbr [propellenttypeabbr.Length - 1] != ':')
								propellenttypeabbr += ':';
							foreach (Propellant propellant in engine.propellants) {
								Log.Info (p.partName + " uses " + propellant.name);

								string s = t.Value.Second.Get ("fueltype:" + propellant.name);
								Log.Info (propellant.name + ": " + propellant.name + "   value: " + s);
								if (s == "")
									s = generateDefault (propellant.name);
								// if s is blank, scan name for cap letters, and use those for abbr.  If no caps
								// then use first 2 letters
								propellenttypeabbr += s;
								Log.Info (propellant.name + " replacement: " + propellenttypeabbr);

							}
						}
					}
				}
			}
			Log.Info ("getPropellentTypes done: " + propellenttypeabbr);
			return propellenttypeabbr;

		}

		static public List<string> getPropellentsInEngines (PartModule pm2, int stage, KeyValuePair<string, PDPN.Tuple<string, NameValueCollection, bool>> t)
		{
			List<string> propellentsinengines = new List<string> ();

            //List<Part> pl = Utils.getPartList(pm2);
            //for (int i = pl.Count - 1; i >= 0; --i)
            //{
                // Part part = pl[i];

             foreach (Part p in Utils.getPartList(pm2)) {
				if (p.Modules.Contains<ModuleEngines> ()) {
					//Log.Info ("Contains: Part: " + part.partName + " name: " + part.name + "   Stage: " + part.inverseStage.ToString () + "  currentStage: " + stage.ToString());

					foreach (PartModule pm in p.Modules) {
						if (pm.moduleName == "ModuleEngines") {
							ModuleEngines engine = pm as ModuleEngines;

							foreach (Propellant propellant in engine.propellants) {
								Log.Info (p.partName + " uses " + propellant.name);
								
								propellentsinengines.AddUnique (propellant.name);
							}
						}
					}
				}
			}
			return propellentsinengines;
		}

		// cargo = C - needs cargo bay
		// spaceplane = SP - needs rockets and wings & landing gear
		// aircraft = A - wings, landing gear and launched from the SPH/runway
		// Disposable_Drone = DD - unmanned, no chutes
		// Reusable_Drone = RD - unmanned, chutes
		// Crew_Transfer = CT - non-command crew capacity
		// Tanker = T - part has resources other than electricity and the resource is either locked or isn't a fuel resource used by any engine
		// Station = S - no engines
		// Tug = Tg -
		// Compound_Vehicle = CV
		// Rover = Rv - needs wheels, not landing gear
		static public string  getVesselType (PartModule pm2, int stage, KeyValuePair<string, PDPN.Tuple<string, NameValueCollection, bool>> t)
		{
			List<string> enginepropellents = getPropellentsInEngines (pm2, stage, t);
			string vesseltypeabbr = "";
			bool cargo = false;
			bool wings = false;
			bool chutes = false;
			bool manned = false;
			bool nonCommandCrew = false;
			bool tanker = false;
			bool station = true;
			bool landingGear = false;
			bool wheels = false;
			bool engines = false;

            //List<Part> pl = Utils.getPartList(pm2);
            //for (int i = pl.Count - 1; i >= 0; --i)
            //{
            //    Part part = pl[i];

             foreach (Part part in Utils.getPartList(pm2)) {
				//Log.Info ("Contains: Part: " + part.partName + " name: " + part.name + "   Stage: " + part.inverseStage.ToString () + "  currentStage: " + stage.ToString ());
				cargo = cargo || part.Modules.Contains<ModuleCargoBay> ();
				wings = wings || part.Modules.Contains<ModuleLiftingSurface> ();
				chutes = chutes || part.Modules.Contains<ModuleParachute> ();
				engines = engines || part.Modules.Contains<ModuleEngines> ();
				station = station & !engines;
				nonCommandCrew = (part.CrewCapacity > 0 && !part.Modules.Contains<ModuleCommand> ());

				//	Log.Info (part.partName + " contains " + part.Resources.Count.ToString () + " resources");
				foreach (PartResource pr in part.Resources) {
					if (pr.resourceName != "ElectricCharge" && pr.resourceName != "MonoPropellant") {
						tanker = tanker || (!enginepropellents.Contains (pr.resourceName) || pr.flowState == false);
					}
				}

				if (part.Modules.Contains<ModuleWheelBase> ()) {
					foreach (PartModule pm in part.Modules) {
						if (pm.moduleName == "ModuleWheelBase") {
							ModuleWheelBase mwb = pm as ModuleWheelBase;
							landingGear = landingGear || (mwb.wheelType == WheelType.MOTORIZED);
							wheels = wheels || (mwb.wheelType == WheelType.MOTORIZED);
						}
					}

				}
			}
			manned = (Utils.getCurrentVessel (pm2).GetCrewCount () > 0);
			if (!chutes && !manned)
				vesseltypeabbr += "DD:";
			if (chutes && !manned)
				vesseltypeabbr += "RD:";
			if (nonCommandCrew)
				vesseltypeabbr += "CT";
			if (tanker)
				vesseltypeabbr += "T";
			if (station)
				vesseltypeabbr += "S";
			if (wheels)
				vesseltypeabbr += "RV";
			if (landingGear && wings && engines)
				vesseltypeabbr += "SP";
			if (vesseltypeabbr == "" && manned)
				vesseltypeabbr = "M";

			return vesseltypeabbr;
		}

		public static List<CelestialBody> getAllowableBodies (String filter = "ALL", bool currentMainBody = false)
		{
			CelestialBody parent;
			List<CelestialBody> bodiesList = new List<CelestialBody> ();
			CelestialBody[] tmpBodies = GameObject.FindObjectsOfType (typeof(CelestialBody)) as CelestialBody[]; 
		
			if (currentMainBody) {
				bodiesList.Add (FlightGlobals.currentMainBody);
				foreach (CelestialBody body in tmpBodies) {
					if (body.orbit != null && body.orbit.referenceBody != null) {
						parent = body.orbit.referenceBody;
					} else
						parent = null;
					if (parent == FlightGlobals.currentMainBody) {
						bodiesList.Add (body);
					}	
				}
			} else {

				Log.Info ("getAllowableBodies  filter: " + filter);
				foreach (CelestialBody body in tmpBodies) {
					if (body.orbit != null && body.orbit.referenceBody != null) {
						parent = body.orbit.referenceBody;
					} else
						parent = null;
					switch (filter) {
					case "ALL":
						bodiesList.Add (body);
						break;
					case "PLANETS":
						if (parent == Sun.Instance.sun)
							bodiesList.Add (body);
						break;
					case "MOONS":
						if (parent != Sun.Instance.sun && parent != null)
							bodiesList.Add (body);
						break;
					default:
						bodiesList.Add (body);
						break;
					}
				}
			}
			return bodiesList;
		}

		static public Dictionary<EngineType, PDPN.Tuple<string, bool>> getEnginePropulsion (PartModule pm2, int stage, KeyValuePair<string, PDPN.Tuple<string, NameValueCollection, bool>> t)
		{
			Log.Info ("getEnginePropulsion");


			Dictionary<EngineType, PDPN.Tuple<string, bool>> propellentsinengines = new Dictionary<EngineType, PDPN.Tuple<string, bool>> ();
			EngineType lastAdded = EngineType.Generic;

            //List<Part> pl = Utils.getPartList(pm2);
            //for (int i = pl.Count - 1; i >= 0; --i)
            //{
               // Part part = pl[i];

             foreach (Part p in Utils.getPartList(pm2)) {
				if (p.Modules.Contains<ModuleEngines> ()) {
					//Log.Info ("Contains: Part: " + part.partName + " name: " + part.name + "   Stage: " + part.inverseStage.ToString () + "  currentStage: " + stage.ToString());

					foreach (PartModule pm in p.Modules) {
						if (pm.moduleName == "ModuleEngines" || pm.moduleName == "ModuleEnginesFX") {
							ModuleEngines engine = pm as ModuleEngines;
							if (!propellentsinengines.ContainsKey (engine.engineType))
								propellentsinengines.Add (engine.engineType, new PDPN.Tuple<string, bool> (engine.engineType.ToString (), false));
							lastAdded = engine.engineType;
							Log.Info ("engineType: " + engine.engineType.ToString ());
						}
						if (pm.moduleName == "MultiModeEngine") {

							PDPN.Tuple<string, bool> val;
							if (propellentsinengines.TryGetValue (lastAdded, out val)) {
								val.Second = true;
								propellentsinengines [lastAdded] = val;
							}

							//	propellentsinengines
							Log.Info ("MultiModeEngine detected");
						}
					}
				}
			}
			return propellentsinengines;
		}

		static public string  getPropulsion (PartModule pm2, int stage, KeyValuePair<string, PDPN.Tuple<string, NameValueCollection, bool>> t)
		{
			string replacement = "";
			Dictionary<EngineType, PDPN.Tuple<string, bool>> v;

			v = getEnginePropulsion (pm2, stage, t);
			foreach (KeyValuePair<EngineType, PDPN.Tuple<string, bool>> s in v) {
				Log.Info ("s: " + s.Key.ToString () + "   s.value: " + s.Value.First);

				string str = t.Value.Second.Get ("propulsion:" + s.Value.First);
				Log.Info ("propulsion" + ": " + s.Value.First + "   value: " + str);
				if (str == "")
					str = generateDefault (s.Value.First);
				// if s is blank, scan name for cap letters, and use those for abbr.  If no caps
				// then use first 2 letters
				replacement += str;
				if (s.Value.Second)
					replacement += "Mme";

			}
			Log.Info ("generated propulsion replacement: " + replacement);

			//	replacement = "propulsion";
			return replacement;
		}


		static public void buildStagingList (PartModule pm2)
		{
            //List<Part> pl = Utils.getPartList(pm2);
            //for (int i = pl.Count - 1; i >= 0; --i) { 

            foreach (Part p in Utils.getPartList(pm2)) {
				//Log.Info ("Part: " + p.partName + " name: " + p.name + "   Stage: " + p.inverseStage.ToString ());
				if (p.Modules.Contains<ModuleCommand> () || p.Modules.Contains<ModuleEngines> ()) {
					//Log.Info ("Contains: Part: " + p.partName + " name: " + p.name + "   Stage: " + p.inverseStage.ToString ());
				}
			}
		}
		// ===================================================
	}
}