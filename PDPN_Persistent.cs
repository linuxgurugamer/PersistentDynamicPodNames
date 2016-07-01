using System;
using System.Text.RegularExpressions;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;


namespace PDPN
{
	[KSPScenario (ScenarioCreationOptions.AddToAllGames, new GameScenes[] {
		GameScenes.SPACECENTER,
		GameScenes.EDITOR,
		GameScenes.FLIGHT,
		GameScenes.TRACKSTATION,
		GameScenes.SPACECENTER
	})]
	public class PDPN_Persistent : ScenarioModule
	{
		public static bool inited = false;
		static bool loaded = false;

		private static List<Tuple<string, int>> shipListCnts;
		[Persistent]
        private List<string> shipListIdCnts;

        [Persistent]
        private List<string> activeTemplates;

		public static int getShipIdCount(string str)
		{
			Log.Info ("getShipIdCount id: " + str);
			foreach (Tuple<string, int> t in shipListCnts) {
				if (t.First == str) {
					return ++t.Second;
				}
			}
			Tuple<string, int> t1 = new Tuple<string, int> (str, 1);
			shipListCnts.Add (t1);
			return 1;
		}

		public override void OnSave(ConfigNode node)
		{
			Log.Info ("PDPNPersistent.OnSave");
			shipListIdCnts = new List<string>();

			foreach (Tuple<string, int> t in shipListCnts) {
				shipListIdCnts.Add (t.First + ":" + t.Second.ToString());
			}

            if (Constants.config.templates != null)
            {
                activeTemplates = new List<string>();
                foreach (KeyValuePair<string, Tuple<string, NameValueCollection, bool>> aa in Constants.config.templates)
                {
                    string s = aa.Value.Third.ToString() + ":" + aa.Key;
                    activeTemplates.Add(s);
                }
            }

            base.OnSave(node);
			node.AddNode (ConfigNode.CreateConfigFromObject (this));
		}

		public override void OnLoad(ConfigNode node)
		{
			if (loaded)
				return;
			loaded = true;
			Log.Info ("PDPNPersistent.OnLoad");
			base.OnLoad(node);
			shipListCnts = new List<Tuple<string, int>> ();
			try {
				ConfigNode.LoadObjectFromConfig(this, node.GetNode(GetType().FullName));
				int cnt;
				bool rc;

				Tuple<string, int> t;
				if (shipListIdCnts != null && shipListCnts != null) {

                    //for (int i1 = shipListIdCnts.Count - 1; i1 >= 0; --i1)
                    //{
                       foreach (string s in shipListIdCnts) {
						Log.Info("s: " + s);
						if (s.Contains(":")) {
							rc = Int32.TryParse(s.Substring(s.IndexOf(":") + 1), out cnt);
							if (rc == false)
								cnt = 0;
							t = new Tuple<string, int> (s.Substring(0, s.IndexOf(":")), cnt);
							Log.Info("Loaded: " + s.Substring(0, s.IndexOf(":") ) + " :" + cnt.ToString());
							shipListCnts.Add (t);
						}
					}
					shipListIdCnts = null;

				}
                if (activeTemplates != null)
                {
                    //Log.Info("Sizeof template list: " + Constants.config.templates.Count.ToString());
                    //for (int i1 = activeTemplates.Count - 1; i1 >= 0; --i1)
                    foreach (string s in activeTemplates)
                    {
                        int i = s.IndexOf(":");
                        bool b = bool.Parse(s.Substring(0, i));
                       // Log.Info("key: " + s.Substring(i + 1) + "   bool: " + b.ToString());

                        foreach (KeyValuePair<string, Tuple<string, NameValueCollection, bool>> aa in Constants.config.templates)
                        {
                            if (aa.Key == s.Substring(i+1))
                            {
                                aa.Value.Third = b;
                                break;
                            }
                        }

                    }
                }
			} catch {
			}
		}


		override public void  OnAwake()
		{
			//Log.Info ("KRASHPersistent.Awake");
			Constants.persistent = this;
			inited = true;
		}

#if false
        public void Start()
		{
			
			//Tuple<string, int>t = new Tuple<string, int>("test2", 34);
			//shipListCnts.Add (t);
		}
#endif
	}


}

