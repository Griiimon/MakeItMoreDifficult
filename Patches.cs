

using HarmonyLib;
using Il2CppScheduleOne.UI.Stations;



namespace MakeItMoreDifficult
{
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
}