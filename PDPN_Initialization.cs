using KSP.IO;
using System;
using System.Diagnostics;

using UnityEngine;


namespace PDPN
{

	[KSPAddon (KSPAddon.Startup.MainMenu, true)]
	public class PDPN_Initialization: MonoBehaviour
	{
		public void Start ()
		{
			Log.Info ("PDPNProcesser.Start");
			Constants.config = new Configuration ();

            // Configuration MUST be loaded before templates
            Constants.config.LoadConfiguration();
            Constants.config.LoadTemplates();

			DontDestroyOnLoad (this);

		}

	}
}

