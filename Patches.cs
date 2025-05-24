

using HarmonyLib;
using Il2CppFishNet;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.EntityFramework;
using Il2CppScheduleOne.Law;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.Persistence.Datas;
using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.Quests;
using Il2CppScheduleOne.UI.Stations;
using MelonLoader;
using System.Collections.Generic;
using System.Linq;
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

    #region Penalties

    [HarmonyPatch(typeof(PenaltyHandler), "ProcessCrimeList")]
    internal static class PenaltyHandlerPatch
    {
        private static bool Prefix(ref Il2CppSystem.Collections.Generic.List<string> __result, Il2CppSystem.Collections.Generic.Dictionary<Crime, int> crimes)
        {
            var list = new Il2CppSystem.Collections.Generic.List<string>();
            float num = 0f;
            //Crime[] array = crimes.Keys.ToArray();
            Core.instance.LoggerInstance.Msg("Total Crimes " + crimes.Keys.Count);
            var keys = new List<Crime>();
            foreach (var kvp in ((Il2CppSystem.Collections.Generic.Dictionary<Crime, int>)crimes))
            {
                Core.instance.LoggerInstance.Msg("Crime " + kvp.Key.CrimeName);
                keys.Add(kvp.Key);
            }
            Crime[] array = keys.ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                Core.instance.LoggerInstance.Msg("Array Crime " + array[i].CrimeName);
                Core.instance.LoggerInstance.Msg(" Class " + array[i].ObjectClass.ToString());

                if (array[i].CrimeName == "Possession of controlled substances")
                {
                    float num2 = 5f * (float)crimes[array[i]];
                    num += num2;
                    list.Add(crimes[array[i]] + " controlled substances confiscated");
                }
                else if (array[i].CrimeName == "Possession of low-severity drug")
                {
                    float num3 = 10f * (float)crimes[array[i]];
                    num += num3;
                    Core.instance.LoggerInstance.Msg("hit");
                    list.Add(crimes[array[i]] + " low-severity drugs confiscated");
                }
                else if (array[i].CrimeName == "Possession of moderate-severity drug")
                {
                    float num4 = 20f * (float)crimes[array[i]];
                    num += num4;
                    list.Add(crimes[array[i]] + " moderate-severity drugs confiscated");
                    Core.instance.LoggerInstance.Msg("hit");
                }
                else if (array[i].CrimeName == "Possession of high-severity drug")
                {
                    float num5 = 30f * (float)crimes[array[i]];
                    num += num5;
                    list.Add(crimes[array[i]] + " high-severity drugs confiscated");
                }
                else if (array[i].CrimeName == "Evading arrest")
                {
                    num += 50f;
                }
                else if (array[i].CrimeName == "Failure to comply with police instruction")
                {
                    num += 50f;
                }
                else if (array[i].CrimeName == "Violating curfew")
                {
                    num += 100f;
                }
                else if (array[i].CrimeName == "Attempting to sell illicit items")
                {
                    num += 150f;
                }
                else if (array[i].CrimeName == "Assault")
                {
                    num += 75f;
                    Core.instance.LoggerInstance.Msg("hit");
                }
                else if (array[i].CrimeName == "Assault with a deadly weapon")
                {
                    num += 150f;
                }
                else if (array[i].CrimeName == "Vandalism")
                {
                    num += 50f;
                }
                else if (array[i].CrimeName == "Theft")
                {
                    num += 50f;
                }
                else if (array[i].CrimeName == "Brandishing a weapon")
                {
                    num += 50f;
                }
                else if (array[i].CrimeName == "Discharge of a firearm in a public place")
                {
                    num += 50f;
                }
            }

            Core.instance.LoggerInstance.Msg("Total list" + list.Count);
            Core.instance.LoggerInstance.Msg("Orig fine" + num);


            num *= 10f;

            if (num > 0f)
            {
                float cash = Mathf.Min(num, NetworkSingleton<MoneyManager>.Instance.cashBalance);
                float transfer = 0f;
                if (cash < num)
                    //transfer= Mathf.Min(num - cash, NetworkSingleton<MoneyManager>.Instance.SyncAccessor_onlineBalance)
                    transfer = num - cash;

                string text = MoneyManager.FormatAmount(num, showDecimals: true) + " fine";
                /*                if (num6 == num)
                                {
                                    text += " (paid in cash)";
                                }
                                else
                                {
                                    text = text + " (" + MoneyManager.FormatAmount(num6, showDecimals: true) + " paid";
                                    text += " - insufficient cash)";
                                }
                  */
                list.Add(text);
                if (cash > 0f)
                    NetworkSingleton<MoneyManager>.Instance.ChangeCashBalance(-cash);

                if (transfer > 0f)
                    NetworkSingleton<MoneyManager>.Instance.CreateOnlineTransaction("Pay fine", -transfer, 1f, string.Empty);
            }

            __result = list;

            return false;
        }
    }

    #endregion
}