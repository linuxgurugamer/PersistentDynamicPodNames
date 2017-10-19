using System.Text;

using PDPN;

public  class Constants
{
	public const string MODNAME="PersistentDynamicPodNames";
	public const string MODTITLE = "Persistent & Dynamic Pod Names";
	public static readonly string ROOT_PATH = KSPUtil.ApplicationRootPath;
	public static readonly string CONFIG_BASE_FOLDER = ROOT_PATH + "GameData/";
	public static string PDPN_BASE_FOLDER { get { return CONFIG_BASE_FOLDER + MODNAME + "/"; }}
	public static string PDPN_NODENAME = MODNAME;
    public static string pdpnTemplatesFile = PDPN_BASE_FOLDER + "PluginData/PDPN_Templates.cfg";
	public  string PDPN_TEMPLATES_FILE  {
        set { pdpnTemplatesFile = value; }
        get { return pdpnTemplatesFile; }
    }
    public string PDPN_CFG_FILE { get { return PDPN_BASE_FOLDER + "PluginData/PDPN_Settings.cfg"; } }
    public string PDPN_DEFAULT_TEMPLATES_FILE { get { return PDPN_BASE_FOLDER + "PluginData/PDPN_Default_Templates.cfg"; } }
    public static string PDPN_BUTTON  { get { return MODNAME + "/Textures/PDPN_Button";}}

	public static Configuration config;
	public static PDPN.PDPN_Persistent	persistent;
}