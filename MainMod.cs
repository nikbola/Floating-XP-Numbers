using Game;
using Game.UI;
using HarmonyLib;
using Onyx;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace FloatingXPNumbers
{
    public static class MainMod
    {
        public static UnityModManager.ModEntry mod;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            DataLoader.Load(modEntry);

            mod = modEntry;

            return true;
        }

        [HarmonyPatch(typeof(CharacterStats), nameof(CharacterStats.AddExperience), MethodType.Normal)]
        public class Game_CharacterStats_Patch
        {
            [HarmonyPrefix]
            static bool AddExperiencePrefix(CharacterStats __instance, int xp)
            {
                UIPopcornTextManager.ShowNotice("XP: " + xp.ToString(), __instance.gameObject, Storage.floatDuration);
                mod.Logger.Log(xp.ToString());
                return true;
            }
        }

        [HarmonyPatch(typeof(UIPopcornTextManager), "ShowNoticeImpl")]
        public class UIPopcornTextManager_patch
        {
            [HarmonyPrefix]
            static bool ShowNoticeImplPrefix(UIPopcornTextManager __instance, string warning, GameObject victim, float duration)
            {
                if (warning.StartsWith("XP"))
                {
                    if (GameState.IsCutsceneOrConversationRunning())
                    {
                        mod.Logger.Log("Active conversation!");
                        Storage.XPInfo info = new Storage.XPInfo(warning, victim, duration, __instance);
                        Storage.XPQueue.Enqueue(info);
                    }
                    else
                    {
                        var mInstantiate = AccessTools.Method(typeof(UIPopcornTextManager), "InstantiateString");
                        var mAddTextToList = AccessTools.Method(typeof(UIPopcornTextManager), "AddTextToList");
                        UIPopcornText popcornText = mInstantiate.Invoke(__instance, null) as UIPopcornText;
                        popcornText.Set(warning, Color.yellow);
                        popcornText.Play(victim, duration);
                        object[] args = new object[2];
                        args[0] = victim;
                        args[1] = popcornText;
                        mAddTextToList.Invoke(__instance, args);
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(ConversationManager), nameof(ConversationManager.EndConversation))]
        public class EndConversation_Patch
        {
            [HarmonyPostfix]
            static void EndConversationPostfix(ConversationManager __instance)
            {
                var mInstantiate = AccessTools.Method(typeof(UIPopcornTextManager), "InstantiateString");
                var mAddTextToList = AccessTools.Method(typeof(UIPopcornTextManager), "AddTextToList");
                while (Storage.XPQueue.Count > 0)
                {
                    Storage.XPInfo info = Storage.XPQueue.Dequeue();
                    UIPopcornText popcornText = mInstantiate.Invoke(info.popcornTextManager, null) as UIPopcornText;
                    popcornText.Set(info.warning, Color.yellow);
                    popcornText.Play(info.victim, info.duration);
                    object[] args = new object[2];
                    args[0] = info.victim;
                    args[1] = popcornText;
                    mAddTextToList.Invoke(info.popcornTextManager, args);
                }
            }
        }

        [HarmonyPatch(typeof(Cutscene), nameof(Cutscene.EndCutscene))]
        public class EndCutscene_Patch
        {
            [HarmonyPostfix]
            static void EndCutscenePostfix(Cutscene __instance)
            {
                var mInstantiate = AccessTools.Method(typeof(UIPopcornTextManager), "InstantiateString");
                var mAddTextToList = AccessTools.Method(typeof(UIPopcornTextManager), "AddTextToList");
                while (Storage.XPQueue.Count > 0)
                {
                    Storage.XPInfo info = Storage.XPQueue.Dequeue();
                    UIPopcornText popcornText = mInstantiate.Invoke(info.popcornTextManager, null) as UIPopcornText;
                    popcornText.Set(info.warning, Color.yellow);
                    popcornText.Play(info.victim, info.duration);
                    object[] args = new object[2];
                    args[0] = info.victim;
                    args[1] = popcornText;
                    mAddTextToList.Invoke(info.popcornTextManager, args);
                }
            }
        }
    }
}
