

using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.EntityFramework;
using Il2CppScheduleOne.Persistence.Datas;
using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.Quests;
using Il2CppScheduleOne.UI.Stations;
using MelonLoader;
using UnityEngine;
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
        private static bool objectsAreRemoved = false;

        private static bool Prefix(Quest_WelcomeToHylandPoint __instance)
        {
            if (__instance.QuestState != EQuestState.Active)// || !InstanceFinder.IsServer)
                return false;

    
            if(!objectsAreRemoved)
                RemoveObjects(__instance.RV);

            // TODO use money manager
            Console.SubmitCommand("changecash -1000");

            return true;
        }

        private static void RemoveObjects(RV rv)
        {
            string[] objectNames = { "@Properties/RV/RV/SM_Prop_Tarp_Generic_01", "@Properties/RV/RV/SM_Item_Radio_01", "@Properties/RV/RV/Vase", "@Properties/RV/RV/rv/Main/Interior/Bench/", "@Properties/RV/RV/rv/Main/Interior/Cabinets/", "@Properties/RV/RV/rv/Main/Interior/Bed/", "@Properties/RV/RV/Ashtray", "@Properties/RV/RV/Bedside_Table/", "@Properties/RV/RV/Container/Grid (1)/", "@Properties/RV/RV/rv/Main/Wall.001/" };
            foreach(string objName in objectNames)
            {
                GameObject gameObject = GameObject.Find(objName);
                if (gameObject != null)
                    gameObject.SetActive(false);
            }

            while(rv.BuildableItems.Count > 0)
            {
                BuildableItem item = rv.BuildableItems[0];
                item.DestroyItem();
            }
        
            objectsAreRemoved = true;
        }

    }



    #endregion
}