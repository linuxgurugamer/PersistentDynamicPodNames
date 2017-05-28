//using KSP.IO;
using System;
//using System.Diagnostics;
//using System.Collections.Generic;
//using System.Text.RegularExpressions;
//using System.Linq;
//using System.Text;
using UnityEngine;
//using KSP.UI;



namespace PDPN
{

	public class PersistentDynamicPodNames: PartModule, IVesselAutoRename
	{

		static PDPN_EntryWindow pdpnEntryWindow;

		#region Storage

		[KSPField (isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string storedVesselName = "";
        
		[KSPField (isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public VesselType vesselType = VesselType.Unknown;

		[KSPField (isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public int priority = 0;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public bool needsManualInput = false;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public bool used = false;

        [KSPField (isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string originalStoredVesselName = "";

        public PDPN_VesselModule pdpnVesselModule { get { return vessel.GetComponent<PDPN_VesselModule>(); } }
        #endregion

        #region Events

        [KSPEvent (active = true, guiActive = true, guiActiveEditor = true, guiActiveUnfocused = false,  guiActiveUncommand = true, guiName = "Persistent pod name")]
		public void PDPNActivateEvent ()
		{
            if (pdpnEntryWindow != null)
				pdpnEntryWindow.Close ();

			GameObject gObj = new GameObject ("PDPNwindow", typeof(PDPN_EntryWindow));
			DontDestroyOnLoad (gObj);
			pdpnEntryWindow = (PDPN_EntryWindow)gObj.GetComponent (typeof(PDPN_EntryWindow));

			pdpnEntryWindow.Invoke (this, storedVesselName, Utils.getModulePartParent (this).vesselType, priority);

		}
#if false
        [KSPEvent(active = true, guiActive = true, guiActiveEditor = true, guiActiveUnfocused = false, guiActiveUncommand = true, guiName = "PDPN Debug Dump")]
        public void PDPNDumpDataEvent()
        {
            Log.Info("PDPNDumpDataEvent");
            Log.Info("storedVesselName: " + storedVesselName);
            Log.Info("vesselType: " + vesselType.ToString());
            Log.Info("priority: " + priority.ToString());
//            Log.Info("vesselId: " + vesselId.ToString());
            Log.Info("needsManualInput: " + needsManualInput.ToString());
            Log.Info("renamed: " + renamed.ToString());
            Log.Info("originalStoredVesselName: " + originalStoredVesselName);
            //Log.Info("originalShipName: " + originalShipName);
            //Log.Info("partOrigVesselGuid: " + partOrigVesselGuid.ToString());
            Log.Info("Constants.pdpnVesselModule.vessel.id: " + pdpnVesselModule.vessel.id.ToString());
        }
#endif
#endregion


        void OnStart()
        {
            Log.Info("PDPN.OnStart");
          

            GameEvents.onStageActivate.Add(CallbackOnStageActivate);
        }

        void OnDestroy()
        {
            GameEvents.onStageActivate.Remove(CallbackOnStageActivate);
        }

        public void CallbackOnStageActivate(int i)
        {
            Events["PDPNActivateEvent"].guiActiveUncommand = true;
          //  Events["PDPNActivateEvent"].guiActiveUnfocused = true;
        }
#if false

        public override void OnSave(ConfigNode node) 
		{
			Log.Info ("OnSave, part: " + moduleName + "  storedVesselName: " + storedVesselName);
			Log.Info ("this.part.partName: " + this.part.name);
		}
#endif

		public void TypingDone (string newVesselName, VesselType newVesselType, int newPriority)
		{
			storedVesselName = newVesselName;
			vesselType = newVesselType;
			priority = newPriority;
			TypingCancel ();
		}

		public void TypingCancel ()
		{
			pdpnEntryWindow.Close ();
			pdpnEntryWindow = null;
		}


		public  void FixedUpdate()
		{
            
            if (vesselType == VesselType.Unknown)
            {
                Part p = Utils.getModulePartParent(this);
                if (p != null)
                {
                    
                    vesselType = p.vesselType;

                    Log.Info("FixedUpdate  vesselType: " + vesselType.ToString() +
                        "  name: " + Utils.getModulePartParent(this).name);
                }
            }

            if (HighLogic.LoadedSceneIsFlight) {
				//Log.Info ("FixedUpdate partOrigVesselGuid: " + partOrigVesselGuid.ToString () + " id: " + FlightGlobals.ActiveVessel.id.ToString () + " needsManualInput: " + needsManualInput.ToString ()  + " p.partName: " + this.name +" ActiveVessel.vesselName: " + FlightGlobals.ActiveVessel.vesselName);
				//if (FlightGlobals.ActiveVessel.id == Utils.getCurrentVessel (this).id) {
				if (Utils.getCurrentVessel (this).isActiveVessel) {
					if (needsManualInput) {
						Log.Info ("FixedUpdate: needsManualInput: " + vessel.vesselName);
						Log.Info ("FixedUpdate");
						Log.Info ("ActiveVessel.id: " + FlightGlobals.ActiveVessel.id.ToString ());
						Log.Info ("Active vessel: " + Utils.getCurrentVessel (this).vesselName);
						Log.Info ("storedVesselName: " + storedVesselName);
						Log.Info ("vesselType: " + vesselType.ToString ());
						Log.Info ("priority: " + priority.ToString ());
						Log.Info ("needsManualInput: " + needsManualInput.ToString ());
						Log.Info ("p.partName: " + this.name);

                        string s = PDPN_SelectionWindow.formatManualEntryName(Utils.getActiveCommandPodModule(vessel.parts), vessel.vesselName, false, true, originalStoredVesselName);
                        if (s != "")
                            vessel.vesselName = s;
						PDPN_SelectionWindow.flightReady = true;
						PDPN_SelectionWindow.physicsCnt = 0;
                        needsManualInput = false;
                        pdpnVesselModule.renamed = true;
					}
				}
			}
		}

        //
        // Following support the inheritance of: IVesselAutoRename
        //
        // Since IVesselAutoRename doesn't seem to support VesselModule, this calls
        // the function in the Vesselmodule
        //
        public string GetVesselName()
		{
			Log.Info("\nGetVesselName");
            return pdpnVesselModule.GetVesselName ();
		}

		// Since IVesselAutoRename doesn't seem to support VesselModule, this calls
		// the function in the Vesselmodule
		//

		public  VesselType GetVesselType()
		{
            //pdpnVesselModule = vessel.GetComponent<PDPN_VesselModule>();
            return pdpnVesselModule.GetVesselType ();
		}

	}
}
