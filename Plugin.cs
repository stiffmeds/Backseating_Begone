using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace Backseating_Begone
{
    [BepInPlugin("com.meds.backseatingbegone", "Backseating Begone", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> medsEmotesCards { get; private set; }
        public static ConfigEntry<bool> medsEmotesTargetEnemies { get; private set; }
        public static ConfigEntry<bool> medsEmotesTargetAllies { get; private set; }

        private readonly Harmony harmony = new("com.meds.backseatingbegone");
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Backseating begone! Go on, git!");
            medsEmotesCards = Config.Bind(new ConfigDefinition("Disable Emotes", "Cards"), true, new ConfigDescription("Disable emotes on cards."));
            medsEmotesTargetEnemies = Config.Bind(new ConfigDefinition("Disable Emotes", "Target Enemies"), true, new ConfigDescription("Disable emotes that target enemies."));
            medsEmotesTargetAllies = Config.Bind(new ConfigDefinition("Disable Emotes", "Target Allies"), true, new ConfigDescription("Disable emotes that target allies."));
            harmony.PatchAll();
        }
    }
    [HarmonyPatch]
    public class Byebye
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MatchManager), "DoEmoteCard")]
        public static bool DoEmoteCardPrefix()
        {
            if (Plugin.medsEmotesCards.Value)
                return false;
            return true;
        }
    }
}
