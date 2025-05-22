

using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.Persistence.Datas;
using Il2CppScheduleOne.Quests;
using Il2CppScheduleOne.UI.Stations;
using MelonLoader;
using Console = Il2CppScheduleOne.Console;


namespace MakeItMoreDifficult
{
    #region Stations

    [HarmonyPatch(typeof(BrickPressCanvas), "BeginButtonPressed")]
    internal static class BrickPressCanvasPatch
    {
        private static bool Prefix(BrickPressCanvas __instance)
        {
            return !Core.GetTimeManager().IsEndOfDay;
        }

    }

    [HarmonyPatch(typeof(CauldronCanvas), "BeginButtonPressed")]
    internal static class CauldronCanvasPatch
    {
        private static bool Prefix(CauldronCanvas __instance)
        {
            return !Core.GetTimeManager().IsEndOfDay;
        }

    }

    [HarmonyPatch(typeof(ChemistryStationCanvas), "BeginButtonPressed")]
    internal static class ChemistryStationCanvasPatch
    {
        private static bool Prefix(ChemistryStationCanvas __instance)
        {
            return !Core.GetTimeManager().IsEndOfDay;
        }

    }

    [HarmonyPatch(typeof(LabOvenCanvas), "BeginButtonPressed")]
    internal static class LabOvenCanvasPatch
    {
        private static bool Prefix(LabOvenCanvas __instance)
        {
            return !Core.GetTimeManager().IsEndOfDay;
        }

    }

    [HarmonyPatch(typeof(MixingStationCanvas), "BeginButtonPressed")]
    internal static class MixingStationCanvasPatch
    {
        private static bool Prefix(MixingStationCanvas __instance)
        {
            return !Core.GetTimeManager().IsEndOfDay;
        }

    }


    [HarmonyPatch(typeof(PackagingStationCanvas), "BeginButtonPressed")]
    internal static class PackagingStationCanvasPatch
    {
        private static bool Prefix(PackagingStationCanvas __instance)
        {
            return !Core.GetTimeManager().IsEndOfDay;
        }

    }

    #endregion

    #region Quests
    [HarmonyPatch(typeof(Quest_WelcomeToHylandPoint), "Update")]
    internal static class WelcomeToHylandPointPatch
    {
        private static bool Prefix(Quest_WelcomeToHylandPoint __instance)
        {
            if (__instance.QuestState != EQuestState.Active || !InstanceFinder.IsServer)
                return false;

            if (__instance.ReadMessagesQuest.State == EQuestState.Active)
            {
                Core.instance.LoggerInstance.Msg("Destroy RV");
                __instance.ReadMessagesQuest.Complete();
                __instance.ReturnToRVQuest.Complete();
            }

            Console.SubmitCommand("changecash -1000");

            return false;
        }

    }

/*    [HarmonyPatch(typeof(Quest), "InitializeQuest")]
    internal static class InitializeQuestPatch
    {
        private static bool Prefix(Quest __instance)
        {
            if (__instance.title == "Getting Started")
            {
                __instance.Entries.Clear();
                Core.instance.LoggerInstance.Msg("Getting Started Quest Patched");
                return false;
            }

            return true;
        }

    }
*/

    #endregion
}