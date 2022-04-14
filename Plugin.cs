using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace Xray
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static ManualLogSource logger;
        static ConfigEntry<KeyboardShortcut> toggleVisibleKey;

        private void Awake()
        {
            toggleVisibleKey = Config.Bind("General", "Toggle Key", new KeyboardShortcut(KeyCode.I), "Key to toggle held item visibility");

            // Plugin startup logic
            logger = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded");
            Logger.LogInfo($"Patching...");
            Harmony.CreateAndPatchAll(typeof(Plugin));
            Logger.LogInfo($"Patched");
        }

        [HarmonyPatch(typeof(HandToFront), "Update")]
        [HarmonyPostfix]
        static void Update_Postfix(GameObject ___equippedItem)
        {
            if (toggleVisibleKey.Value.IsDown())
            {
                if (___equippedItem != null && ___equippedItem.GetComponent<Renderer>() != null)
                {
                    ___equippedItem.GetComponent<Renderer>().enabled = !___equippedItem.GetComponent<Renderer>().enabled;
                }
            }
        }

        [HarmonyPatch(typeof(HandToFront), "DropHolding")]
        [HarmonyPrefix]
        static void DropHolding_Prefix(GameObject ___equippedItem)
        {
            if (___equippedItem != null && ___equippedItem.GetComponent<Renderer>() != null)
            {
                ___equippedItem.GetComponent<Renderer>().enabled = true;
            }
        }
    }
}
