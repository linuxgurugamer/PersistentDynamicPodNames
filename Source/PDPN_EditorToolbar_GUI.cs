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


namespace PDPN
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class PDPN_EditorToolbar_GUI : MonoBehaviour
    {
        public ApplicationLauncherButton _appLauncherButton = null;

        private IButton toolbarButton = null;

        bool activated = false;

        private void Awake()
        {
            // Create the applauncher button and register the GUI for drawing
            if (_appLauncherButton == null)
                _appLauncherButton = InitAppLauncherButton();
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
                toggleToolbarButton(state1);
            };
            toolbarButton.Visible = true;
            if (toolbarButton.EffectivelyVisible)
                RemoveAppLauncherButton(_appLauncherButton);
            Log.Info("Done starting Toolbar button!");
        }

        void toggleToolbarbutton()
        {
            toggleToolbarButton(!activated);

        }

        void toggleToolbarButton(bool show)
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

            button = ApplicationLauncher.Instance.AddModApplication(toggleToolbarbutton, toggleToolbarbutton,
                () =>
                {
                    ShowMenu();
                }, //RUIToggleButton.OnHover
                () =>
                {
                    HideMenu();
                },
                //null, null, 
                null, null,
                ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.SPACECENTER,
                iconTexture);

            if (button == null)
            {
                Log.Info("PersistentDynamicPodNames -- Unable to create AppLauncher button");
            }

            return button;
        }

        public static void RemoveAppLauncherButton(ApplicationLauncherButton button)
        {
            if (button != null)
            {
                Log.Info("RemoveAppLauncherButton: " + button.ToString());

                ApplicationLauncher.Instance.RemoveModApplication(button);
            }
            else
            {
                Log.Info("RemoveAppLauncherButton: button is null");
            }
        }

        bool _showMenu = false;
        Rect _menuRect = new Rect();
        const float _menuWidth = 100.0f;
        // each additional button needs an additional 25 added to _menuHeight
        const float _menuHeight = 100.0f;
        const int _toolbarHeight = 42;

        public void ShowMenu()
        {
            Vector3 position = Input.mousePosition;
            int toolbarHeight = (int)(_toolbarHeight * GameSettings.UI_SCALE);
            _menuRect = new Rect()
            {
                xMin = position.x - _menuWidth / 2,
                xMax = position.x + _menuWidth / 2,
                yMin = Screen.height - toolbarHeight - _menuHeight,
                yMax = Screen.height - toolbarHeight
            };
            _showMenu = true;
        }

        public void HideMenu()
        {
            _showMenu = false;
        }

        public void OnAppLauncherTrue()
        {
            if (( /*!cfg.blizzyToolbar || */ !ToolbarManager.ToolbarAvailable) && _appLauncherButton == null)
            {
                Log.Info("PersistentDynamicPodNames -- OnAppLauncherTrue called without a button?!?");
                return;
            }

            EditorLogic.fetch.Lock(true, true, true, "PersistentDynamicPodNamesLocked");
            //_editorLocked = true;
            activated = true;
            Log.Info("PersistentDynamicPodNames.OnAppLauncherTrue, activated: " + activated.ToString());

        }

        public void OnAppLauncherFalse()
        {
            if ((/* !cfg.blizzyToolbar || */ !ToolbarManager.ToolbarAvailable) && _appLauncherButton == null)
            {
                Log.Info("PersistentDynamicPodNames -- OnAppLauncherFalse called without a button?!?");
                return;
            }
            Log.Info("PersistentDynamicPodNames.OnAppLauncherFalse 2");
            if (EditorLogic.fetch != null)
                EditorLogic.fetch.Unlock("PersistentDynamicPodNamesLocked");
            //_editorLocked = false;
            activated = false;
        }



        bool tempSelActive = false;
        private Rect bounds;
        float lastTimeShown = 0.0f;


        private void DrawTitle(String text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(text, HighLogic.Skin.label);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static string[] tmplListdest;
        static List<string> tmplList;

        public static void getTmplList()
        {
            tmplList = new List<string>();
            foreach (KeyValuePair<string, Tuple<string, NameValueCollection, bool>> aa in Constants.config.templates)
            {
                if (aa.Value.Third)
                    tmplList.Add("*" + aa.Key + "*" /* + " = " + aa.Value.Second.Get("template") */);

            }
            tmplListdest = new string[tmplList.Count()];
            int cnt = 0;
            //for (cnt = tmplList.Count() - 1; cnt >=0; --cnt) { 
            foreach (string s in tmplList)
            {
                tmplListdest[cnt] = tmplList[cnt];
                Log.Info("s: " + tmplListdest[cnt]);
                cnt++;
            }
        }

        const string primaryTemplSel = "Primary Ship Name Template Selection";
        private void OnGUI()
        {
            if (activated)
            {
                if (tempSelActive == false)
                {
                    tempSelActive = true;

                    GUI.skin = HighLogic.Skin;

                    getTmplList();

                    var size2 = HighLogic.Skin.GetStyle("Label").CalcSize(new GUIContent(primaryTemplSel));
                    int x = (int)size2.x + 20,
                    y = (int)size2.y;

                    int cnt2 = 0;
                    int x1 = 0;
                    //for (int i = tmplList.Count - 1; i >=0; --i) { 
                    foreach (string sar in tmplList)
                    {

                        var size = HighLogic.Skin.GetStyle("Label").CalcSize(new GUIContent(sar));
                        if (size.x > x1)
                            x1 = (int)size.x;
                        if (size.y > y)
                            y = (int)size.y;
                    }
                    cnt2 = tmplList.Count();
                    x += x1;
                    if (x < 100)
                        x = 100;
                    size2 = HighLogic.Skin.GetStyle("Button").CalcSize(new GUIContent("Select"));
                    y = y * cnt2 + (int)size2.y + 10;
                    Log.Info("cnt2: " + cnt2.ToString() + "  size2.y: " + size2.y.ToString() + "   y: " + y.ToString());

                    y = y > Screen.height - 100 ? Screen.height - 100 : y;
                    x = x > Screen.width - 100 ? Screen.width - 100 : x;

                    bounds = new Rect((Screen.width - x) - 25, (Screen.height - y) / 2, x, y);

                }

                bounds = GUILayout.Window(this.GetInstanceID(), bounds, this.TemplateSelection, primaryTemplSel, HighLogic.Skin.window);
            }
            else
            {
                tempSelActive = false;

                if (Event.current.type == EventType.Layout)
                {
                    if (_showMenu || _menuRect.Contains(Event.current.mousePosition) || (Time.fixedTime - lastTimeShown < 0.25f))
                        _menuRect = GUILayout.Window(this.GetInstanceID(), _menuRect, MenuContent, "PDPN Menu");
                    else
                        _menuRect = new Rect();
                }

            }
            if (cmdPodwindowVisible)
            {
                cmdPodWindow = GUILayout.Window(this.GetInstanceID() + 1, cmdPodWindow, CmdPodsWindow, "Command Pods");
            }
            else
            {
                selectedField = Field.none;
                newSelectedField = Field.none;
            }
        }


        bool cmdPodwindowVisible = false;
        Rect cmdPodWindow;
        const int WIDTH = 500;
        const int HEIGHT = 400;

        void MenuContent(int WindowID)
        {
            if (_showMenu || _menuRect.Contains(Event.current.mousePosition))
                lastTimeShown = Time.fixedTime;
            GUILayout.BeginVertical();
            if (GUILayout.Button("Command Pods"))
            {
                cmdPodwindowVisible = true;

                cmdPodWindow = new Rect()
                {
                    xMin = Screen.width - WIDTH - 5,
                    xMax = Screen.width - 5,
                    yMin = Screen.height / 2 - HEIGHT / 2,
                    yMax = Screen.height / 2 + HEIGHT / 2
                };

            }
            if (GUILayout.Button("Set Priority to stage"))
            {
                setPriorityByStageCommandPodList();
            }
            if (GUILayout.Button("Set Priority to reverse stage"))
            {
                setPriorityByReverseStageCommandPodList();
            }

            GUILayout.EndVertical();
        }


        Vector2 sitesScrollPosition;
        const int LINEHEIGHT = 30;
        public static GUIStyle readoutButtonStyle = new GUIStyle(HighLogic.Skin.button)
        {
            normal =
            {
                textColor = Color.white
            },
            //margin = new RectOffset(2, 2, 2, 2),
            margin = new RectOffset(),
            padding = new RectOffset(),
            alignment = TextAnchor.MiddleCenter,
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            stretchHeight = false
        };

        void setPriorityByStageCommandPodList()
        {

            //List<Part> pl = EditorLogic.fetch.ship.parts;
            //for (int i = pl.Count - 1; i >= 0; --i)
            //{
            // Part part = pl[i];

            foreach (Part p in EditorLogic.fetch.ship.parts)
            {
                if (p.Modules.Contains(Constants.MODNAME))
                {
                    foreach (PartModule m in p.Modules)
                    {
                        if (m.moduleName == Constants.MODNAME)
                            ((PersistentDynamicPodNames)m).priority = p.inverseStage;
                    }
                }
            }
        }
        void setPriorityByReverseStageCommandPodList()
        {
            int numstages = StageManager.GetStageCount(EditorLogic.fetch.ship.parts);

            //List<Part> pl = EditorLogic.fetch.ship.parts;
            //for (int i = pl.Count - 1; i >= 0; --i)
            //{
            // Part part = pl[i];

            foreach (Part p in EditorLogic.fetch.ship.parts)
            {
                if (p.Modules.Contains(Constants.MODNAME))
                {
                    foreach (PartModule m in p.Modules)
                    {
                        if (m.moduleName == Constants.MODNAME)
                            ((PersistentDynamicPodNames)m).priority = numstages - p.inverseStage;
                    }
                }
            }
        }

        List<Tuple<int, Part, PartModule>> sortedPodList;

        bool checkMouseLocation(Part p)
        {
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                showSelectedPart(p);
                return true;
            }
            return false;
        }
        float lastMouseDowntime = 0;
        bool one_click = false;
        bool mouseup = false;
        int selectedLine = -1;
        int newSelectedLine = -1;
        enum Field { none, name, vesseltype };
        Field selectedField = Field.none;
        Field newSelectedField = Field.none;

        void CmdPodsWindow(int WindowID)
        {
            UnityEngine.GUI.skin = HighLogic.Skin;
            GUIStyle t = new GUIStyle("TextField");
            t.fontSize = 12;
            sortedPodList = Utils.getSortedCommandPodList(EditorLogic.fetch.ship.parts);

            GUILayout.Label("Click field to select command pod for editing\nDouble-click to get selection arrows");
            GUILayout.BeginVertical();
            sitesScrollPosition = GUILayout.BeginScrollView(sitesScrollPosition, false, true, GUILayout.Height(HEIGHT - 3 * LINEHEIGHT));
            //int cnt = 0;
            Vector3 position = Input.mousePosition;
            Part p;
            int i = -1;
            bool mouseDown = Input.GetMouseButtonDown(0);
            bool mouseDoubleClick = false;
            if (mouseDown)
            {
                if (one_click && mouseup && Time.fixedTime - lastMouseDowntime < GameSettings.DOUBLECLICK_MOUSESPEED && lastMouseDowntime > 0)
                {
                    mouseDoubleClick = true;
                }
                else
                {
                    lastMouseDowntime = Time.fixedTime;
                    one_click = true;
                    mouseup = false;
                }

            }
            else
            {
                if (Time.fixedTime - lastMouseDowntime < GameSettings.DOUBLECLICK_MOUSESPEED)
                    mouseup = true;
                else
                    one_click = false;
            }

            foreach (Tuple<int, Part, PartModule> pt in sortedPodList)
            {
                p = pt.Second;
                i++;

                PartModule m = pt.Third;

                if (m.moduleName == Constants.MODNAME)
                {

                    GUILayout.BeginHorizontal();

                    GUILayout.Label(p.name, t, GUILayout.Width(100));
                    checkMouseLocation(p);

                    if (selectedLine == i && selectedField == Field.name)
                    {
                        if (GUILayout.Button("<", readoutButtonStyle, GUILayout.Width(15.0f), GUILayout.Height(25)))
                        {
                            getTmplList();
                            int x = 0;
                            //for (int i1 = tmplListdest.Length - 1; i1 >= 0; --i1) {
                            foreach (string s in tmplListdest)
                            {
                                if (((PersistentDynamicPodNames)m).storedVesselName == s)
                                {
                                    if (x == 0)
                                        ((PersistentDynamicPodNames)m).storedVesselName = tmplListdest[tmplListdest.Count() - 1];
                                    else
                                        ((PersistentDynamicPodNames)m).storedVesselName = tmplListdest[x - 1];
                                    break;
                                }
                                x++;
                            }
                            if (x == tmplListdest.Count())
                                ((PersistentDynamicPodNames)m).storedVesselName = tmplListdest[0];
                        }
                        checkMouseLocation(p);
                    }
                    //GUI.SetNextControlName(i.ToString());

                    ((PersistentDynamicPodNames)m).storedVesselName = GUILayout.TextField(((PersistentDynamicPodNames)m).storedVesselName, t, GUILayout.Width(225));
                    if (mouseDoubleClick && checkMouseLocation(p))
                    {
                        newSelectedLine = i;
                        newSelectedField = Field.name;
                    }
                    checkMouseLocation(p);
                    if (selectedLine == i && selectedField == Field.name)
                    {
                        if (GUILayout.Button(">", readoutButtonStyle, GUILayout.Width(15.0f), GUILayout.Height(25)))
                        {
                            getTmplList();
                            int x = 0;
                            //for (int i1 = tmplListdest.Length - 1; i1 >= 0; --i1)
                            //{
                            foreach (string s in tmplListdest)
                            {
                                if (((PersistentDynamicPodNames)m).storedVesselName == s)
                                {
                                    if (x == tmplListdest.Count() - 1)
                                        ((PersistentDynamicPodNames)m).storedVesselName = tmplListdest[0];
                                    else
                                        ((PersistentDynamicPodNames)m).storedVesselName = tmplListdest[x + 1];
                                    break;
                                }
                                x++;
                            }
                            if (x == tmplListdest.Count())
                                ((PersistentDynamicPodNames)m).storedVesselName = tmplListdest[0];
                        }
                        checkMouseLocation(p);
                    }

                    if (selectedLine == i && selectedField == Field.vesseltype)
                    {
                        if (GUILayout.Button("<", readoutButtonStyle, GUILayout.Width(15.0f), GUILayout.Height(25)))
                        {
                            int cnt = 0;
                            VesselType workingvesseltype = VesselType.Unknown;

                            //for (int cnt = PDPN_EntryWindow.vtypes.Length - 1; cnt >= 0; --cnt)
                            //{
                            foreach (string s in PDPN_EntryWindow.vtypes)
                            {
                                cnt++;
                                if (((PersistentDynamicPodNames)m).vesselType.ToString() == s)
                                {
                                    if (cnt == 1)
                                    {
                                        workingvesseltype = PDPN_EntryWindow.getVesselType(PDPN_EntryWindow.vtypes.Count() - 1);
                                    }
                                    else
                                    {
                                        workingvesseltype = PDPN_EntryWindow.getVesselType(cnt - 2);
                                    }
                                    break;
                                }

                            }
                            ((PersistentDynamicPodNames)m).vesselType = workingvesseltype;
                        }
                    }

                    checkMouseLocation(p);

                    GUILayout.Label(((PersistentDynamicPodNames)m).vesselType.ToString(), t, GUILayout.Width(50.0f));
                    if (mouseDoubleClick && checkMouseLocation(p))
                    {
                        newSelectedLine = i;
                        newSelectedField = Field.vesseltype;
                    }
                    if (selectedLine == i && selectedField == Field.vesseltype)
                    {
                        if (GUILayout.Button(">", readoutButtonStyle, GUILayout.Width(15.0f), GUILayout.Height(25)))
                        {
                            int cnt = 0;
                            VesselType workingvesseltype = VesselType.Unknown;

                            //for (int cnt = PDPN_EntryWindow.vtypes.Length - 1; cnt >= 0; --cnt)
                            //{
                            foreach (string s in PDPN_EntryWindow.vtypes)
                            {
                                cnt++;
                                if (((PersistentDynamicPodNames)m).vesselType.ToString() == s)
                                {
                                    if (cnt == PDPN_EntryWindow.vtypes.Count() - 1)
                                    {
                                        workingvesseltype = PDPN_EntryWindow.getVesselType(0);
                                    }
                                    else
                                    {
                                        workingvesseltype = PDPN_EntryWindow.getVesselType(cnt);
                                    }
                                    break;
                                }

                            }
                            ((PersistentDynamicPodNames)m).vesselType = workingvesseltype;
                        }
                    }

                    checkMouseLocation(p);

                    GUILayout.BeginVertical();
                    if (i == 0)
                        GUI.enabled = false;
                    if (GUILayout.Button("▲", readoutButtonStyle, GUILayout.Width(25.0f)))
                    {
                        Tuple<int, Part, PartModule> prev = sortedPodList[i - 1];
                        int x = ((PersistentDynamicPodNames)prev.Third).priority;
                        ((PersistentDynamicPodNames)prev.Third).priority = ((PersistentDynamicPodNames)m).priority;
                        ((PersistentDynamicPodNames)m).priority = x;
                    }

                    checkMouseLocation(p);
                    if (i == sortedPodList.Count - 1)
                        GUI.enabled = false;
                    else
                        GUI.enabled = true;
                    if (GUILayout.Button("▼", readoutButtonStyle, GUILayout.Width(25.0f)))
                    {
                        Tuple<int, Part, PartModule> next = sortedPodList[i + 1];
                        int x = ((PersistentDynamicPodNames)next.Third).priority;
                        ((PersistentDynamicPodNames)next.Third).priority = ((PersistentDynamicPodNames)m).priority;
                        ((PersistentDynamicPodNames)m).priority = x;
                    }

                    GUI.enabled = true;
                    checkMouseLocation(p);
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);

                }
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Hide Window"))
            {
                cmdPodwindowVisible = false;
                UnHighlightParts(EditorLogic.fetch.ship.parts);

            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            //Log.Info ("GUI.GetNameOfFocusedControl: " + GUI.GetNameOfFocusedControl ());

            selectedField = newSelectedField;
            selectedLine = newSelectedLine;
            GUI.DragWindow();
        }

        public Color goodStrutColor = XKCDColors.Green;
        Part lastHighlightedPart = null;
        void showSelectedPart(Part part)
        {
            if (lastHighlightedPart != part)
            {
                UnHighlightParts(EditorLogic.fetch.ship.parts);
                HighlightSinglePart(XKCDColors.Green, goodStrutColor, part);
            }
        }

        public void HighlightSinglePart(Color highlightC, Color edgeHighlightColor, Part p)
        {
            p.SetHighlightDefault();
            p.SetHighlightType(Part.HighlightType.AlwaysOn);
            p.SetHighlight(true, false);
            p.SetHighlightColor(highlightC);
            //p.highlighter.ConstantOn(edgeHighlightColor);
            //p.highlighter.SeeThroughOn();
            p.HighlightActive = true;
            p.SetOpacity(0.5f
                );
            lastHighlightedPart = p;
        }

        public void UnHighlightParts(List<Part> partList)
        {
            List<Part> pl = EditorLogic.fetch.ship.parts;
            // for (int i = pl.Count - 1; i >= 0; --i)
            //{
            // Part part = pl[i];

            foreach (Part p in partList)
            {
                p.SetHighlightDefault();
                // p.highlighter.Off();
                p.HighlightActive = false;
            }
            lastHighlightedPart = null;
        }

        int selectedTemplate = -1;
        Vector2 scrollPosition;
        private void TemplateSelection(int id)
        {

            GUI.enabled = true;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            selectedTemplate = GUILayout.SelectionGrid(selectedTemplate, tmplListdest, 1, "Toggle", GUILayout.ExpandWidth(true));
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            GUILayout.Space(16);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled = (selectedTemplate >= 0);
            if (GUILayout.Button("Select", GUILayout.ExpandWidth(true)))
            {
                //EditorLogic.fetch.shipNameField.text = "*" + tmplListdest[selectedTemplate] + "*";

                foreach (KeyValuePair<string, Tuple<string, NameValueCollection, bool>> aa in Constants.config.templates)
                {
                    if (aa.Value.Third && aa.Key == tmplListdest[selectedTemplate].Substring(1, tmplListdest[selectedTemplate].Length - 2))
                    {
                        EditorLogic.fetch.shipNameField.text = tmplListdest[selectedTemplate];
                        if (EditorLogic.fetch.shipDescriptionField.text == "")
                            EditorLogic.fetch.shipDescriptionField.text = aa.Value.Second.Get("template");

                        break;
                    }
                }
                OnAppLauncherFalse();

            }
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow();
            //	GUILayout.EndArea ();
        }

        /*
         * Called when the game is leaving the scene (or exiting). Perform any clean up work here.
         */
        void OnDestroy()
        {
            Log.Info("PDPN_EditorToolbar_GUI.OnDestory");
            toggleToolbarButton(false);
            if (toolbarButton != null)
                toolbarButton.Destroy();
            RemoveAppLauncherButton(_appLauncherButton);
        }
    }
}

