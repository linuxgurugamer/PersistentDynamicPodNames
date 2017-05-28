using UnityEngine;
//using System;
//using System.IO;

//using KSP.UI;
//using KSP.UI.Screens;

//using System.Linq;
//using System.Text;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Text.RegularExpressions;



namespace PDPN
{

	public class PDPN_EntryWindow : MonoBehaviour
	{

		private PersistentDynamicPodNames attachedModule;
		private Rect windowRect;
		private string workingvesselname;
		private VesselType workingvesseltype;
		private int workingpriority;
		private string workingprioritystring;

		private bool wasFocusedOnce = false;
		private int numberOfRepaints = 0;
		private bool gameEventHooksExist = false;
		private int myWindowId;
		// must be unique for Unity to not mash two nametag windows togehter.


		public void Invoke (PersistentDynamicPodNames module, string oldVesselName, VesselType oldVesselType, int oldPriority)
		{

			attachedModule = module;
			workingvesselname = oldVesselName;
			workingvesseltype = oldVesselType;
			workingpriority = oldPriority;
			workingprioritystring = workingpriority.ToString ();


			myWindowId = GetInstanceID (); // Use the Id of this MonoBehaviour to guarantee unique window ID.

			Vector3 screenPos = GetViewportPosFor (attachedModule.part.transform.position);

			// screenPos is in coords from 0 to 1, 0 to 1, not screen pixel coords.
			// Transform it to pixel coords:
			float xPixelPoint = screenPos.x * UnityEngine.Screen.width;
			float yPixelPoint = (1 - screenPos.y) * UnityEngine.Screen.height;
			const float WINDOW_WIDTH = 400;
			const float WINDOW_HEIGHT = 200;

			// windowRect = new Rect(xPixelWindow, yPixelPoint, windowWidth, 130);
			windowRect = new Rect (xPixelPoint - WINDOW_WIDTH / 2, yPixelPoint, WINDOW_WIDTH, WINDOW_HEIGHT);

			SetEnabled (true);

			if (HighLogic.LoadedSceneIsEditor)
				attachedModule.part.SetHighlight (false, false);

		}

		/// <summary>
		/// Catch the event of the part disappearing, from crashing or
		/// from unloading from distance or scene change, and ensure
		/// the window closes if it was open when that happens:
		/// </summary>
		/// <param name="whichPartWentAway">The callback is called for EVERY part
		/// that ever goes away, so we have to check if it's the right one</param>
		public void GoAwayEventCallback (Part whichPartWentAway)
		{
			if (whichPartWentAway != attachedModule.part)
				return;

			Close ();
		}

		/// <summary>
		/// If you try to set a Unity.Behaviour.enabled to false when it already IS false,
		/// and Unity hasn't fully finished configuring the MonoBehaviour yet, the Property's
		/// "set" code throws a null ref error. How lame is that?
		/// That's why I wrapped every attempt to set enabled's value with this check, because KSP
		/// tries running my hooks in this class before Unity's ready for them.
		/// </summary>
		private void SetEnabled (bool newVal)
		{
			// ReSharper disable once RedundantCheckBeforeAssignment
			if (newVal != enabled)
				enabled = newVal;

			if (enabled) {
				if (!gameEventHooksExist) {
					GameEvents.onPartDestroyed.Add (GoAwayEventCallback);
					GameEvents.onPartDie.Add (GoAwayEventCallback);
					gameEventHooksExist = true;
				}
			} else {
				if (gameEventHooksExist) {
					GameEvents.onPartDestroyed.Remove (GoAwayEventCallback);
					GameEvents.onPartDie.Remove (GoAwayEventCallback);                
					gameEventHooksExist = false;                    
				}
			}
		}

		/// <summary>
		/// Get the position in screen coordinates that the 3d world coordinates
		/// project onto, abstracting the two different ways KSP has to access
		/// the camera depending on view mode.
		/// Returned coords are in a system where the screen viewport goes from
		/// (0,0) to (1,1) and the Z coord is how far from the screen it is
		/// (-Z means behind you).
		/// </summary>
		private Vector3 GetViewportPosFor (Vector3 v)
		{
			return Utils.GetCurrentCamera ().WorldToViewportPoint (v);
		}

		void OnGUI ()
		{
			if (Event.current.type != EventType.Repaint)
				++numberOfRepaints;

			if (!enabled)
				return;

			//	Log.Info("PDPNWindow.OnGUI");
			GUI.skin = HighLogic.Skin;

			if (HighLogic.LoadedSceneIsEditor)
				EditorLogic.fetch.Lock (false, false, false, Constants.MODNAME + "Lock");

			GUI.skin = HighLogic.Skin;
			windowRect = GUILayout.Window (myWindowId, windowRect, PersistentPodNameEntryWindow, "Enter Persistent Pod Name");

			// Ensure that the first time the window is made, it gets keybaord focus,
			// but allow the focus to leave the window after that:
			// The reason for the "number of repaints" check is that OnGUI has to run
			// through several initial passes before all the components are present,
			// and if you call FocusControl on the first few passes, it has no effect.
			if (numberOfRepaints >= 2 && !wasFocusedOnce) {
				GUI.FocusControl ("Stored Vessel Name");
				wasFocusedOnce = true;
			}
		}

		public static string[] vtypes = new string[] { "Probe", "Rover", "Lander", "Ship", "Station", "Base", "Plane", "Relay" };
		int vtype = 3;

        static int getVesselType(VesselType v)
        {
          
            int cnt = 0;
            //for (int cnt = vtypes.Length - 1; cnt >= 0; --cnt)
            //{
            foreach (string s in vtypes)
            {
                if (v.ToString() == s)
                    return cnt;
                cnt++;
            }
            return 0;
        }

		public static VesselType getVesselType (int vtype)
		{
            VesselType workingvesseltype = VesselType.Unknown;

			switch (vtype) {
			    case 0:
				    workingvesseltype = VesselType.Probe;
				    break;
			    case 1:
				    workingvesseltype = VesselType.Rover;
				    break;
			    case 2:
				    workingvesseltype = VesselType.Lander;
				    break;
			    case 3:
				    workingvesseltype = VesselType.Ship;
				    break;
			    case 4:
				    workingvesseltype = VesselType.Station;
				    break;
			    case 5:
				    workingvesseltype = VesselType.Base;
				    break;
                case 6:
                    workingvesseltype = VesselType.Plane;
                    break;
                case 7:
                    workingvesseltype = VesselType.Relay;
                    break;

            }
            return workingvesseltype;
		}

		public void PersistentPodNameEntryWindow (int windowID)
		{
			if (!enabled)
				return;
			GUI.skin = HighLogic.Skin;
			Event e = Event.current;
			if (e.type == EventType.KeyDown) {
				if (e.keyCode == KeyCode.Return ||
					e.keyCode == KeyCode.KeypadEnter) {
					e.Use ();
					attachedModule.TypingDone (workingvesselname, workingvesseltype, workingpriority);
					Close ();
				}
			}
			GUILayout.BeginHorizontal ();
			int i = attachedModule.part.name.IndexOf ("(") - 1;
			if (i < 0)
				i = attachedModule.part.name.Length - 1;
			GUILayout.Label ("Pod: " + attachedModule.part.name.Substring (0, i));
			GUILayout.EndHorizontal ();
			//GUILayout.BeginHorizontal ();
			GUI.SetNextControlName ("Stored Vessel Name");
			//GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
            if (GUILayout.Button("<", PDPN_EditorToolbar_GUI.readoutButtonStyle, GUILayout.Width(15.0f), GUILayout.Height(25)))
            {
                PDPN_EditorToolbar_GUI.getTmplList();
                int x = 0;
                
                foreach (string s in PDPN_EditorToolbar_GUI.tmplListdest)
                {
                    if (workingvesselname == s)
                    {
                        if (x == 0)
                            workingvesselname = PDPN_EditorToolbar_GUI.tmplListdest[PDPN_EditorToolbar_GUI.tmplListdest.Length - 1];
                        else
                            workingvesselname = PDPN_EditorToolbar_GUI.tmplListdest[x - 1];
                        break;
                    }
                    x++;
                }
                if (x == PDPN_EditorToolbar_GUI.tmplListdest.Length)
                    workingvesselname = PDPN_EditorToolbar_GUI.tmplListdest[0];
            }
            workingvesselname = GUILayout.TextField (workingvesselname, GUILayout.MinWidth (160f));
            if (GUILayout.Button(">", PDPN_EditorToolbar_GUI.readoutButtonStyle, GUILayout.Width(15.0f), GUILayout.Height(25)))
            {
                PDPN_EditorToolbar_GUI.getTmplList();
                int x = 0;

                foreach (string s in PDPN_EditorToolbar_GUI.tmplListdest)
                {
                    if (workingvesselname == s)
                    {
                        if (x == PDPN_EditorToolbar_GUI.tmplListdest.Length - 1)
                            workingvesselname = PDPN_EditorToolbar_GUI.tmplListdest[0];
                        else
                            workingvesselname = PDPN_EditorToolbar_GUI.tmplListdest[x + 1];
                        break;
                    }
                    x++;
                }
                if (x == PDPN_EditorToolbar_GUI.tmplListdest.Length)
                    workingvesselname = PDPN_EditorToolbar_GUI.tmplListdest[0];
            }
            GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Priority:");

			string tmp = GUILayout.TextField (workingprioritystring, GUILayout.Width (60f));

			if (tmp == "" || int.TryParse (tmp, out workingpriority)) {
				workingprioritystring = tmp;
				if (tmp == "")
					workingpriority = 0;
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
            vtype = getVesselType(workingvesseltype);
			vtype = GUILayout.SelectionGrid (vtype, vtypes, 6, "Button", GUILayout.ExpandWidth (true));
			workingvesseltype = getVesselType (vtype);


			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Cancel")) {
				e.Use ();
				Close ();
			}
			if (GUILayout.Button ("Accept")) {
				e.Use ();
				attachedModule.TypingDone (workingvesselname, workingvesseltype, workingpriority);
				Close ();
			}
			GUILayout.EndHorizontal ();
			GUI.DragWindow ();
			// Before going any further, suppress any remaining unprocessed clicks
			// so they don't end up causing the editor to detach parts:
			if (e.type == EventType.MouseDown || e.type == EventType.MouseUp || e.type == EventType.MouseDrag)
				e.Use ();
		}

		public void Close ()
		{
			if (HighLogic.LoadedSceneIsEditor)
				EditorLogic.fetch.Unlock (Constants.MODNAME + "Lock");

			SetEnabled (false);

			if (HighLogic.LoadedSceneIsEditor)
				attachedModule.part.SetHighlight (false, false);
		}

		public void OnDestroy ()
		{
			SetEnabled (false);
		}
	}
}