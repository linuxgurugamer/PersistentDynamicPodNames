﻿using KSP.IO;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI;


namespace PDPN
{
    /// This partial module is used to track the many per-module fields in
    /// a vessel.  The original implementation looped every FixedUpdate
    /// over every single part, and every single module in the part, to
    /// track certain values.  By registering for the OnVesselChanged,
    /// OnVesselDestroy, and OnVesselModified events, I can reduce the
    /// need to iterate over _everything_ per FixedUpdate.

    public class PDPN_VesselModule : VesselModule //, IVesselAutoRename
    {

        public Vessel vessel;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, guiName = "origGuid")]
        public Guid origVesselGuid;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public bool renamed = false;

        //        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = false, guiName = "name tag")]
        //        public bool needsManualInput = false;

        /// <summary>
        /// Start - check to see if the part has any launch clamps.
        /// </summary>
        public void Start()
        {
          Log.Info("PDPNVesselModule.Start");
          //  Constants.pdpnVesselModule = this;
            
            vessel = GetComponent<Vessel>();
            if (vessel == null || vessel.isEVA /* || !vessel.isCommandable */)
            {
                vessel = null;
                //Destroy(this);
                return;
            }
            
            Log.Info("PDPNVesselModule.Start  vessel.id: " + vessel.id.ToString() + "   origVesselGuid: " + origVesselGuid.ToString());
            vessel.vesselName = GetVesselName();
            vessel.vesselType = GetVesselType();

            //GameEvents.onStageSeparation.Add(CallbackOnStageSeperation);
            //GameEvents.OnFlightGlobalsReady.Add (CallbackOnFlightGlobalsReady);
            //GameEvents.onFlightReady.Add (CallbackOnFlightReady);
            GameEvents.onVesselWasModified.Add(CallbackOnVesselWasModified);

            //GameEvents.onPartCouple.Add (this.CallbackPartCouple);
            //GameEvents.onLaunch.Add(CallbackOnlaunch);
        }

        public void OnDestroy()
        {
            //GameEvents.onStageSeparation.Remove(CallbackOnStageSeperation);
            //GameEvents.OnFlightGlobalsReady.Remove(CallbackOnFlightGlobalsReady);
            //GameEvents.onFlightReady.Remove(CallbackOnFlightReady);
            GameEvents.onVesselWasModified.Remove(CallbackOnVesselWasModified);
        }
#if false
        public void CallbackOnStageSeperation(EventReport evt)
        {
            Log.Info("PDPNVesselModule.CallbackOnStageSeperation, Guid: " + vessel.id.ToString() + "   event: " + evt.ToString());
        }

        private void CallbackOnFlightReady()
        {
            Log.Info("PDPNVesselModule.CallbackOnFlightReady");
            //vessel = GetComponent<Vessel>();
            //if (vessel == null || vessel.isEVA || !vessel.isCommandable)
            //{
            //    vessel = null;
                //Destroy(this);
            //    return;
            //}
            //	doVesselRename ();

        }

        private void CallbackOnFlightGlobalsReady(bool evt)
        {
            Log.Info("PDPNSelectionWindow.CallbackOnFlightGlobalsReady");
            //vessel = GetComponent<Vessel>();
            //if (vessel == null || vessel.isEVA || !vessel.isCommandable)
            //{
            //    vessel = null;
                //Destroy(this);
            //    return;
            //}
        }
#endif
        private void CallbackOnVesselWasModified(Vessel v)
        {
            Log.Info("CallbackOnVesselWasModified, guid: " + v.id.ToString() + "  vessel: " + v.vesselName);
            
            if (v != vessel)
            {
                Log.Info("CallbackOnVesselWasModified v.id: " + v.id.ToString() + "   vessel.id: " + vessel.id.ToString());
            }
           
            vessel.vesselName = GetVesselName();
            vessel.vesselType = GetVesselType();
        }

        //public void FixedUpdate()
        //{
        //	Log.Info ("PDPNVesselModule.Fixedupdate");
        //}

        public override void OnAwake()
        {
            Log.Info("PDPNVesselModule.OnAwake");
            base.OnAwake();

            if (!GameDatabase.Instance.IsReady())
            {
                return;
                //throw new Exception("GameDatabase is not ready?");
            }
            vessel = GetComponent<Vessel>();
            Log.Info("PDPNVesselModule.OnAwake  vessel.id: " + vessel.id.ToString());
        }

        
        //
        // Following support the inheritance of: IVesselAutoRename
        //
        public string GetVesselName()
        {
            Log.Info("PDPNVesselModule.GetVesselName, id: " + vessel.id.ToString());

            
            Log.Info("origVesselGuid: " + origVesselGuid.ToString());
            PersistentDynamicPodNames acn = Utils.getActiveCommandPodModule(vessel.parts);

            if (acn == null)
            {
                return vessel.vesselName;
//                return "unknown Vessel";
            }
            string returnString = acn.vessel.vesselName;
            Log.Info("returnString: " + returnString);

            if (renamed && acn.used)
            {
                Log.Info("vessel already renamed & active pod used");
                if (acn.vessel.vesselName == "")
                    acn.vessel.vesselName = acn.vessel.vesselType.ToString();
                Log.Info("returning vessel name  vessel.id: " + vessel.id.ToString() + "   vesselName: " + acn.vessel.vesselName);
                return acn.vessel.vesselName;
            }
            else
            {
                Part p = Utils.getActiveCommandPodPart(vessel.parts);

                Log.Info("vessel.vesselName: " + vessel.vesselName);
                Log.Info("acn.vessel.vesselName: " + acn.vessel.vesselName);
                // Log.Info("partOrigVesselGuid: " + acn.partOrigVesselGuid.ToString());
                Log.Info("storedVesselName: " + acn.storedVesselName);
                Log.Info("vesselType: " + acn.vesselType.ToString());
                Log.Info("priority: " + acn.priority.ToString());
                Log.Info("needsManualInput: " + acn.needsManualInput.ToString());
                Log.Info("p.partName: " + p.name);
                if (acn.storedVesselName != "")
                    returnString = acn.storedVesselName;
                acn.used = true;
                renamed = true;
               
                if (returnString == "")
                {
                    Log.Info("No name specified in pod");
                    Log.Info("returning vessel name  vessel.id: " + vessel.id.ToString() + "   vesselName: " + vessel.vesselName);
                    return acn.vessel.vesselName;
                }
            }

            if (returnString == "")
                returnString = "debris";
            Log.Info("Before formatName, returnString: " + returnString);
            returnString = Utils.formatName(acn, returnString);
            Log.Info("After formatName, returnString: " + returnString);
            //
            // Check for a manual-input field
            //
            string pattern = "\\(.*?\\)|\\#.*?\\#";
            Regex r = new Regex(pattern);

            Match m = r.Match(returnString);
            acn.needsManualInput = m.Success;




            if (acn.needsManualInput)
                acn.originalStoredVesselName = acn.storedVesselName;
            Log.Info("final acn.needsManualInput: " + acn.needsManualInput.ToString());
            Log.Info("final returnString: " + returnString + "  vessel.id: " + vessel.id.ToString() + "\n");
            return returnString;
        }

        public VesselType GetVesselType()
        {
            Log.Info("PDPNVesselModule.GetVesselType");

            Log.Info("origVesselGuid: " + origVesselGuid.ToString() + "   this.vessel.id: " + this.vessel.id.ToString());

            PersistentDynamicPodNames acn = Utils.getActiveCommandPodModule(vessel.parts);

            if (acn == null)
                return vessel.vesselType;

            Part p = Utils.getActiveCommandPodPart(vessel.parts);
            Log.Info("GetVesselType");
            Log.Info("vessel.vesselName: " + vessel.vesselName);
            Log.Info("storedVesselName: " + acn.storedVesselName);
            Log.Info("vesselType: " + acn.vesselType.ToString());
            Log.Info("priority: " + acn.priority.ToString());
            Log.Info("renamed: " + acn.needsManualInput.ToString());
            Log.Info("p.partName: " + p.name);

            return acn.vesselType;
        }

    }
}

