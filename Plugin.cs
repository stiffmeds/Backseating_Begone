using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

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
        [HarmonyPatch(typeof(MatchManager), "NET_SendEmoteCard")]
        public static bool DoEmoteCardPrefix()
        {
            if (Plugin.medsEmotesCards.Value)
                return false;
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MatchManager), "NET_EmoteTarget")]
        public static bool EmoteTargetPrefix(ref MatchManager __instance, string _id, byte _action)
        {
            if (!((int)_action == 2 || (int)_action == 3))
                return true;
            bool TargetAlly = false;
            Hero[] medsTeamHero = Traverse.Create(__instance).Field("TeamHero").GetValue<Hero[]>();
            for (int index = 0; index < medsTeamHero.Length; ++index)
            {
                if (medsTeamHero[index] != null && medsTeamHero[index].Id == _id && medsTeamHero[index].Alive)
                {
                    TargetAlly = true;
                    break;
                }
            }
            if ((TargetAlly && Plugin.medsEmotesTargetAllies.Value) || (!TargetAlly && Plugin.medsEmotesTargetEnemies.Value))
                return false;
            return true;
        }
    }
}
