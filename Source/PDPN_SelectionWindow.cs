using System;
//using System.IO;

using System.Linq;
//using System.Text;
//using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

using UnityEngine;
//using KSP.UI;
using KSP.UI.Screens;

namespace PDPN
{
	[KSPAddon (KSPAddon.Startup.Flight, false)]
	public class PDPN_SelectionWindow : MonoBehaviour
	{
		static public List<string[]> selArList;
		static public List<int> selList;
		static public List<string> selTitleList;
        static Part cmdPod;

		public static bool doSelection = false;
		public static bool selectionComplete = false;
		bool selectionWindowActive = false;

		Rect SelectionWindowPos;

		// Launch is also a staging event
		public enum FlightStatusType
		{
			none,
			onPad,
			/* launch, */
			staging
        };

		static public FlightStatusType flightStatus = FlightStatusType.none;

		void Start ()
		{
			Log.Info ("PDPNSelectionWindow.Start");
			//GameEvents.onStageSeparation.Add (CallbackOnStageSeperation);
			//GameEvents.onLaunch.Add(CallbackOnlaunch);
			// GameEvents.OnFlightGlobalsReady.Add (CallbackOnFlightGlobalsReady);
			GameEvents.onFlightReady.Add (CallbackOnFlightReady);
            //			GameEvents.onPartCouple.Add (CallbackPartCouple);
            //GameEvents.onVesselWasModified.Add (this.CallbackOnVesselWasModified);
           
        }


        void Awake ()
		{
			Log.Info ("PDPNSelectionWindow.Awake");
		}

		void OnDestroy ()
		{
			Log.Info ("PDPNSelectionWindow.OnDestroy");
            GameEvents.onFlightReady.Remove(CallbackOnFlightReady);
        }

		// ===================================================
		public static KeyValuePair<string, PDPN.Tuple<string, NameValueCollection, bool>> GetFormat (string templateName)
		{
			Log.Info ("GetFormat templateName: " + templateName);

            if (templateName.Length >= 3)
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

        public static string formatManualEntryName(PartModule pm2, string templateName, bool fillSelections = false, bool inFlight = false, string storedTemplateName = "")
		{
			Log.Info ("enter formatManualEntryName:  " + templateName);
			//if (templateName == "")
			//	return "";
			if (pm2 == null)
				return "Unknown vessel debris";

			if (!inFlight && (!fillSelections && (templateName.Length < 3 || templateName [0] != '*' || templateName [templateName.Length - 1] != '*'))) {
				Log.Info ("returning: " + templateName);
				return templateName;
			}


            KeyValuePair<string, PDPN.Tuple<string, NameValueCollection, bool>> t;
            if (inFlight)
                t = GetFormat(storedTemplateName);
            else
                t = GetFormat(templateName);

			if (!fillSelections) {
				InitSelectionWindow ();
				flightStatus = FlightStatusType.onPad;
				templateName = Utils.formatName (pm2, templateName);
				Log.Info ("After call to Utils.formatName, templateName: " + templateName);
			}


			string newVesselName = "";
			if (fillSelections || inFlight || t.Key != "") {
				string template;
                    
				template = templateName;

				Log.Info ("formatManualEntryName:  template: " + template);
				Log.Info ("flightStatus: " + flightStatus.ToString ());
				newVesselName = template;
				string pattern = "";
				char prefix = ' ';
				char suffix = ' ';
			
				for (int pcnt = 0; pcnt < 2; pcnt++) {
					switch (pcnt) {
					case 0:
						pattern = "\\(.*?\\)";
						prefix = '(';
						suffix = ')';
						break;
					case 1:
						pattern = "\\#.*?\\#";
						prefix = '#';
						suffix = '#';
						break;
					}
                    Log.Info("formatManualEntryName pattern: " + pattern + "   template: " + template);
					Regex r = new Regex (pattern);

					Match m = r.Match (template);
					while (m.Success) {
						string id = m.Value.Substring (1, m.Value.Length - 2);
						string search = prefix + id + suffix;
						string replacement = "";
						//string engineType = "";
						Log.Info ("formatName  id: " + id + "   search: " + search);
						//replacement = (++cnt).ToString();
						replacement = "";
						if (fillSelections) {
							Log.Info ("fillSelections  id: " + id);
							for (int i = 0; i < selTitleList.Count; i++) {
								if (selTitleList [i] == id) {
									replacement = selArList [i] [selList [i]];
									Log.Info ("Final replacement: " + selTitleList [i] + "  replacement: " + replacement);
									break;
								}
							}
						} else {
							// Format the replacement, then do the replace
							if (prefix == '#') {
								replacement = PDPN_Persistent.getShipIdCount (id).ToString ();
							} else {
								int stageCnt = StageManager.GetStageCount (Utils.getPartList (pm2));
								switch (id) {

								case "destination":
                                    case "moons":
                                    case "planets":
									replacement = getDestination (id, pm2, stageCnt - Utils.getCurrentVessel (pm2).currentStage, t);
									break;

								default:
									replacement = getCustom (id, pm2, stageCnt - Utils.getCurrentVessel (pm2).currentStage, t);
									break;

								}
							}
						}

						if (replacement != "")
							newVesselName = newVesselName.Replace (search, replacement);

						m = m.NextMatch ();
					}

				}
				//}

				//				foreach (string s in t.Value.Second.AllKeys) {
				//					Log.Info ("Key: " + s + "     Value: " + t.Value.Second [s]);
				//				}
			}
			Log.Info ("formatname vesselName: " + newVesselName);
			return newVesselName;
		}

		static string  getDestination (string id, PartModule pm2, int stage, KeyValuePair<string, PDPN.Tuple<string, NameValueCollection, bool>> t)
		{
			//	string replacement = "";
			List<string> v = new List<string> ();

			string values = t.Value.Second.Get (id + ":values");
			Log.Info ("values: " + values);
			if (values != null && values.Length > 0) {
				if (values.Contains ("planets")) {
					values = values.Replace ("planets", "");
					List<CelestialBody> bodies = Utils.getAllowableBodies ("PLANETS");
					foreach (CelestialBody b in bodies) {
						v.Add (b.bodyName);
					}
				}
				if (values.Contains ("moons")) {
					values = values.Replace ("moons", "");
					List<CelestialBody> bodies = Utils.getAllowableBodies ("MOONS");
					foreach (CelestialBody b in bodies) {
						v.Add (b.bodyName);
					}
				}
				if (values.Contains ("all")) {
					values.Replace ("all", "");
					List<CelestialBody> bodies = Utils.getAllowableBodies ("ALL");
					foreach (CelestialBody b in bodies) {
						v.Add (b.bodyName);
					}
				}
				// Remove extra white spaces.  not the fastest, but works and speed isn't an issue here
				values = Regex.Replace (values, @"\s+", " ");
				if (values.Length > 0) {
					var d = values.Split (new char[]{ ' ' }, StringSplitOptions.None);
                    //for (int i1 = d.Length - 1; i1 >=0; --i1) {
                    foreach (string s in d) {
						Log.Info ("Added static value: " + s);
						v.Add (s);
					}
				}
			} else {
                if (id == "planets")
                {
                    List<CelestialBody> bodies = Utils.getAllowableBodies("PLANETS");
                    foreach (CelestialBody b in bodies)
                    {
                        v.Add(b.bodyName);
                    }
                }
                if (id == "moons")
                {
                    List<CelestialBody> bodies = Utils.getAllowableBodies("MOONS");
                    foreach (CelestialBody b in bodies)
                    {
                        v.Add(b.bodyName);
                    }
                }
                if (id == "destination")
                {
                    List<CelestialBody> bodies = Utils.getAllowableBodies();
                    foreach (CelestialBody b in bodies)
                    {
                        v.Add(b.bodyName);
                    }
                }
			}
			string[] dest = new string[v.Count ()];
			int cnt = 0;
            //for (int i1 = v.Count - 1; i1 >= 0; --i1) 
			foreach (string s in v)
				dest [cnt++] = s;

            Part p = Utils.getActiveCommandPodPart(((PersistentDynamicPodNames)pm2).pdpnVesselModule.pdpnVessel.parts);

            StartSelection (p, "destination", dest);

			return "";
		}

		static string getCustom (string id, PartModule pm2, int stage, KeyValuePair<string, PDPN.Tuple<string, NameValueCollection, bool>> t)
		{
			//	string replacement = "";
			List<string> v = new List<string> ();
			bool destSelected = false;
			Log.Info ("getCustom id: " + id);

			string values = t.Value.Second.Get (id + ":values");
			Log.Info ("getCustom values: [" + values + "]");
			if (values != null && values.Length > 0) {
				Log.Info ("values.Length: " + values.Length.ToString ());
				// Remove extra white spaces.  not the fastest, but works and speed isn't an issue here
				values = Regex.Replace (values, @"\s+", " ");
				if (values.Length > 0) {

					if (values.Contains ("planets")) {
						destSelected = true;
						values = values.Replace ("planets", "");
						List<CelestialBody> bodies = Utils.getAllowableBodies ("PLANETS");
						foreach (CelestialBody b in bodies) {
							v.Add (b.bodyName);
						}
					}
					if (values.Contains ("moons")) {
						destSelected = true;
						values = values.Replace ("moons", "");
						List<CelestialBody> bodies = Utils.getAllowableBodies ("MOONS");
						foreach (CelestialBody b in bodies) {
							v.Add (b.bodyName);
						}
					}
					if (values.Contains ("currentMainBody")) {
						destSelected = true;
						values = values.Replace ("currentMainBody", "");
						List<CelestialBody> bodies = Utils.getAllowableBodies (FlightGlobals.currentMainBody.ToString(), true);
						foreach (CelestialBody b in bodies) {
							v.Add (b.bodyName);
						}
					}
					if (values.Contains ("all")) {
						destSelected = true;
						values.Replace ("all", "");
						List<CelestialBody> bodies = Utils.getAllowableBodies ("ALL");
						foreach (CelestialBody b in bodies) {
							v.Add (b.bodyName);
						}
					}
					if (destSelected) {
						// Remove extra white spaces.  not the fastest, but works and speed isn't an option here
						values = Regex.Replace (values, @"\s+", " ");
						if (values.Length > 0) {
							var d = values.Split (new char[]{ ' ' }, StringSplitOptions.None);
                            //for (int i1 = d.Length - 1; i1 >= 0; --i1) { 
                            foreach (string s in d) {
								Log.Info ("Added static value: " + s);
								v.Add (s);
							}
						}
					} else {

						var d = values.Split (new char[]{ ' ' }, StringSplitOptions.None);
                        //for (int i1 = d.Length - 1; i1 >= 0; --i1)
                        //{
                        foreach (string s in d) {
							Log.Info ("Added static value: " + s);
							v.Add (s);
						}
					}
				}

				string[] dest = new string[v.Count ()];
				int cnt = 0;
				foreach (string s in v)
					dest [cnt++] = s;

                Part p = Utils.getActiveCommandPodPart(((PersistentDynamicPodNames)pm2).pdpnVesselModule.pdpnVessel.parts);
                StartSelection (p, id, dest);
			}
			Log.Info ("getCustom end");
			return "";
		}


		// ===================================================

		private const int PHYSICSWAIT = 2;
		public static bool flightReady = false;
		public static int physicsCnt = 0;

		private void CallbackOnFlightReady ()
		{
			Log.Info ("PDPNSelectionWindow.CallbackOnFlightReady");
			//	doVesselRename ();
			flightStatus = FlightStatusType.onPad;
			flightReady = true;
			physicsCnt = 0;
		}

        long lastTime = System.DateTime.Now.ToBinary();
        bool highlightOn = false;
		void LateUpdate ()
		{
			if (flightReady && physicsCnt++ > PHYSICSWAIT) {
				flightReady = false;
				Log.Info ("LateUpdate flightReady is true");

				//Utils.setPodVesselGuid (FlightGlobals.ActiveVessel, FlightGlobals.ActiveVessel.id);

				doVesselRename (Utils.getActiveCommandPodModule (FlightGlobals.ActiveVessel.parts));
			}
            if (doHighlighting)
            {
               // Log.Info("System.DateTime.Now: " + System.DateTime.Now.ToBinary().ToString() + "   lastTime: " + lastTime.ToString());
               
                if (System.DateTime.Now.ToBinary() > lastTime)
                {
                    // flash every 1/2 second
                    lastTime = System.DateTime.Now.ToBinary() + 5000000;
                    if (!highlightOn)
                        StartHighlight(cmdPod, XKCDColors.Green);
                    else
                        StopHighlight(cmdPod);
                    highlightOn = !highlightOn;
                }     
            }
		}


		void doVesselRename (PartModule pm2)
		{
			if (pm2 == null)
				return;

//            Utils.getCurrentVessel(pm2).vesselName = formatManualEntryName(pm2, Utils.getCurrentVessel(pm2).vesselName, false, true, Utils.getCurrentVessel(pm2).vesselName);
            Utils.getCurrentVessel(pm2).vesselName = formatManualEntryName(pm2, Utils.getCurrentVessel(pm2).vesselName, false, false);

            Log.Info ("Utils.getCurrentVessel(this).vesselName: " + Utils.getCurrentVessel (pm2).vesselName);

		}

		static public void InitSelectionWindow ()
		{
			Log.Info ("InitSelectionWindow");
			//selection = -1;
			PDPN_SelectionWindow.selectionComplete = false;
			PDPN_SelectionWindow.doSelection = false;
			selArList = new List<string[]> ();
			selTitleList = new List<string> ();
			selList = new List<int> ();
		}

		public static void StartSelection (Part p, string title, string[] selAr)
		{
			Log.Info ("StartSelection");
			//	selectionTitle = title;
			//	selectionArray = selAr;

			selArList.Add (selAr);
			selTitleList.Add (title);
			selList.Add (-1);
            cmdPod = p;

			doSelection = true;
		}

		void FillSelections (Vessel v)
		{
			Log.Info ("FillSelections start v.vesselName: " + v.vesselName);

            PersistentDynamicPodNames pdpn = Utils.getActiveCommandPodModule(v.parts);

            v.vesselName =	formatManualEntryName (pdpn, v.vesselName, true);
			Log.Info ("FillSelections end v.vesselName: " + v.vesselName);
            pdpn.pdpnVesselModule.renamed = true;

        }

        bool doHighlighting = false;
        public void StartHighlight(Part p, Color c)
        {
            Log.Info("StartHighlight");

            p.SetHighlightDefault();
            p.SetHighlightType(Part.HighlightType.AlwaysOn);
            p.SetHighlight(true, false);
            p.SetHighlightColor(c);
            //p.highlighter.ConstantOn(c);
            //p.highlighter.SeeThroughOn();
            p.HighlightActive = true;
            p.SetOpacity(0.5f);
        }

        public void StopHighlight(Part p)
        {
            Log.Info("StopHighlight");
            p.SetHighlightDefault();
            // p.highlighter.Off();
            p.HighlightActive = false;
        }


        public void SelectionWindow (int windowID)
		{	
			//GUILayout.BeginVertical ();
			int i = 0;

            doHighlighting = true;
            

            GUILayout.BeginHorizontal ();
            //GUILayout.Label("Active command pod:");
            GUILayout.Label(cmdPod.name, "textfield");

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            bool allselected = true;
			foreach (string s in selTitleList) {
				
				GUILayout.BeginVertical ();
				GUILayout.Space (16);
				GUILayout.Label (s);

				selList [i] = GUILayout.SelectionGrid (selList [i], selArList [i], 1, "toggle", GUILayout.ExpandWidth (true));
				if (selList [i] == -1)
					allselected = false;
				GUILayout.EndVertical ();
			
				i++;
			}


			GUILayout.EndHorizontal ();

			GUILayout.Space (16);
			GUI.enabled = allselected;
			if (GUILayout.Button ("OK", GUILayout.ExpandWidth (true))) {
				selectionComplete = true;
				FillSelections (FlightGlobals.ActiveVessel);
				FlightDriver.SetPause (false);
                StopHighlight(cmdPod);
                doHighlighting = false;
            }
            GUI.enabled = true;
			//GUILayout.EndVertical();
			GUI.DragWindow ();
		}


		void OnGUI ()
		{
			if (doSelection && !selectionComplete) {
				if (!FlightDriver.Pause)
					FlightDriver.SetPause (true);
				GUI.skin = HighLogic.Skin;
				int myWindowId = GetInstanceID ();
                string title = "Selection";
				if (selectionWindowActive == false) {
					selectionWindowActive = true;
					selectionComplete = false;

					var size2 = HighLogic.Skin.GetStyle ("Button").CalcSize (new GUIContent (title));
					int x = (int)size2.x + 20,
					y = (int)size2.y;

					int cnt = 0;
					foreach (string[] sar in selArList) {
						
						int x1 = 0;
						for (int i = 0; i < sar.Count (); i++) {
							var size = HighLogic.Skin.GetStyle ("Button").CalcSize (new GUIContent (sar [i]));
							if (size.x > x1)
								x1 = (int)size.x;
							if (size.y > y)
								y = (int)size.y;
						}
						x += x1;
						if (sar.Count () > cnt)
							cnt = sar.Count ();
					}

					size2 = HighLogic.Skin.GetStyle ("Button").CalcSize (new GUIContent ("OK"));
					y = y * cnt + (int)size2.y;
					SelectionWindowPos = new Rect ((Screen.width - x) / 2, (Screen.height - y) / 2, x, y);
				}
				SelectionWindowPos = GUILayout.Window (myWindowId + 1, SelectionWindowPos, SelectionWindow, title);
			}

		}

	}
}