using System;
//using System.IO;
using System.Linq;
//using System.Reflection;
//using System.Text.RegularExpressions;
//using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using UnityEngine;
using KSP.UI.Screens;

#if true

namespace PDPN
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class PDPN_ConfigToolbar_GUI : MonoBehaviour
    {
        ApplicationLauncherButton _cfgAppLauncherButton = null;

        private static IButton toolbarButton = null;


        bool activated = false;
        private void Awake()
        {
            // Create the applauncher button and register the GUI for drawing
            if (_cfgAppLauncherButton == null)
                _cfgAppLauncherButton = InitAppLauncherButton();
            //if (cfg.blizzyToolbar)
            addToToolbar();
        }


        void addToToolbar()
        {
            Log.Info("addToToolbar");
            if (!ToolbarManager.ToolbarAvailable /* || !cfg.blizzyToolbar */ )
                return;
            Log.Info("Starting Toolbar button!");
            bool state1 = false;
            toolbarButton = ToolbarManager.Instance.add(Constants.MODNAME, "toggle");
            toolbarButton.TexturePath = Constants.PDPN_BUTTON + "24";
            toolbarButton.ToolTip = "Toggle " + Constants.MODTITLE + " window";
            toolbarButton.OnClick += (e) =>
            {
                Log.Info("button1 clicked, mouseButton: " + e.MouseButton);
                //button1.TexturePath = state1 ? "000_Toolbar/img_buttonTypeMNode" : "000_Toolbar/icon";
                state1 = !state1;
                toggleCfgToolbarButton(state1);
            };
            toolbarButton.Visible = true;
            if (toolbarButton.EffectivelyVisible)
                RemoveAppLauncherButton(_cfgAppLauncherButton);
            Log.Info("Done starting Toolbar button!");
        }

        void toggleCfgToolbarbutton()
        {
            toggleCfgToolbarButton(!activated);

        }

        void toggleCfgToolbarButton(bool show)
        {

            if (show /* && cfg.blizzyToolbar */)
            {
                //start the GUI
                OnAppLauncherTrue();
            }
            else
            {
                //close the GUI
                OnAppLauncherFalse();
            }
        }
        // Called next.
        private void Start()
        {
            try
            {
                Log.Info("PersistentDynamicPodNames [" + GetInstanceID().ToString("X") + "][" + Time.time.ToString("0.0000") + "]: Start");
            }
            catch (Exception ex)
            {
                Log.Info(ex.ToString());
            }
            // DontDestroyOnLoad (this);
        }

        public ApplicationLauncherButton InitAppLauncherButton()
        {
            ApplicationLauncherButton button = null;
            Texture2D iconTexture = null;
            if (GameDatabase.Instance.ExistsTexture(Constants.PDPN_BUTTON + "38"))
            {
                Log.Info("Button image exists: " + Constants.PDPN_BUTTON + "38");
                iconTexture = GameDatabase.Instance.GetTexture(Constants.PDPN_BUTTON + "38", false);
            }

            if (iconTexture == null)
            {
                Log.Info("PersistentDynamicPodNames -- Failed to load " + Constants.PDPN_BUTTON + "38");
            }

            button = ApplicationLauncher.Instance.AddModApplication(toggleCfgToolbarbutton, toggleCfgToolbarbutton,
                null, null,
                null, null,
                ApplicationLauncher.AppScenes.SPACECENTER,
                iconTexture);

            if (button == null)
            {
                Log.Info("PersistentDynamicPodNames -- Unable to create AppLauncher button");
            }

            return button;
        }

        public static void RemoveAppLauncherButton(ApplicationLauncherButton button)
        {
            if (button)
                ApplicationLauncher.Instance.RemoveModApplication(button);
        }


        public void OnAppLauncherTrue()
        {
            if (( /*!cfg.blizzyToolbar || */ !ToolbarManager.ToolbarAvailable) && _cfgAppLauncherButton == null)
            {
                Log.Info("PersistentDynamicPodNames -- OnAppLauncherTrue called without a button?!?");
                return;
            }


            activated = true;
            Log.Info("PersistentDynamicPodNames.OnAppLauncherTrue, activated: " + activated.ToString());

        }

        public void OnAppLauncherFalse()
        {
            if ((/* !cfg.blizzyToolbar || */ !ToolbarManager.ToolbarAvailable) && _cfgAppLauncherButton == null)
            {
                Log.Info("PersistentDynamicPodNames -- OnAppLauncherFalse called without a button?!?");
                return;
            }
            Log.Info("PersistentDynamicPodNames.OnAppLauncherFalse 1");

            activated = false;
        }

        private void DrawTitle(String text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(text, HighLogic.Skin.label);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        void getTmplList()
        {

        }


        const int WIDTH = 250;
        const int HEIGHT = 300;
        private Rect bounds = new Rect((Screen.width - WIDTH) / 2, (Screen.height - HEIGHT) / 2, WIDTH, HEIGHT);

        Vector2 sitesScrollPosition;
        const int LINEHEIGHT = 30;

        private void OnGUI()
        {
            if (activated)
            {
                GUI.skin = HighLogic.Skin;

                bounds = GUILayout.Window(this.GetInstanceID(), bounds, templateSelWin, "Available Templates", HighLogic.Skin.window);
            }

        }
        void templateSelWin(int WindowID)
        {
            GUILayout.BeginVertical();
            sitesScrollPosition = GUILayout.BeginScrollView(sitesScrollPosition, false, true, GUILayout.Height(HEIGHT - 3 * LINEHEIGHT));

            List<KeyValuePair<string, Tuple<string, NameValueCollection, bool>>> aal = Constants.config.templates;
 //           for (int i = aal.Count - 1; i >= 0; --i)
            foreach (KeyValuePair<string, Tuple<string, NameValueCollection, bool>> aa in Constants.config.templates)
            {
                GUILayout.BeginHorizontal();
                aa.Value.Third = GUILayout.Toggle(aa.Value.Third, "");
                GUILayout.Label(aa.Key);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Enable all", GUILayout.ExpandWidth(true)))
            {
                //List<KeyValuePair<string, Tuple<string, NameValueCollection, bool>>> aal = Constants.config.templates;
               // for (int i = aal.Count - 1; i >= 0; --i)
               foreach (KeyValuePair<string, Tuple<string, NameValueCollection, bool>> aa in Constants.config.templates)
                        aa.Value.Third = true;
            }
            if (GUILayout.Button("Disable all", GUILayout.ExpandWidth(true)))
            {
               // List<KeyValuePair<string, Tuple<string, NameValueCollection, bool>>> aal = Constants.config.templates;
               // for (int i = aal.Count - 1; i >= 0; --i)
                foreach (KeyValuePair<string, Tuple<string, NameValueCollection, bool>> aa in Constants.config.templates)
                    aa.Value.Third = false;
                //}
            }
            if (GUILayout.Button("Close Window", GUILayout.ExpandWidth(true)))
            {
                toggleCfgToolbarbutton();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void OnDestroy()
        {
            toggleCfgToolbarButton(false);
            if (toolbarButton != null)
                toolbarButton.Destroy();
            RemoveAppLauncherButton(_cfgAppLauncherButton);
            _cfgAppLauncherButton = null;
        }
    }
}

#endif