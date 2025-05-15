using System;
using System.Collections;
using Il2CppInterop.Runtime.Injection;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using Il2CppScheduleOne;
using Object = UnityEngine.Object;
using Console = Il2CppScheduleOne.Console;
using System.Runtime.InteropServices.ComTypes;
using Unity.Jobs.LowLevel.Unsafe;
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Persistence.Datas;

[assembly: MelonInfo(typeof(MakeItMoreDifficult.Core), "MakeItMoreDifficult", "0.0.1", "Griiimon")]
[assembly: MelonGame(null, null)]


namespace MakeItMoreDifficult
{

    public class Core : MelonMod
	{
		public static Core instance= null;

		private static int rent= 6000;

		public static int debt= 0;

		private static bool HasPlayerSpawned;

		private bool InMainGame;

		private static GameObject DayUIElement;

		private static TextMeshProUGUI DayText;

		private static MelonPreferences_Category ConfigCategory;

		private static Vector3 DefaultPosition = new Vector3(550f, 510f, 0f);

		public static bool payNow = false;


		public override void OnInitializeMelon()
		{
			instance= this;

			base.LoggerInstance.Msg("MakeItMoreDifficult mod loaded successfully!!");
			Core.ConfigCategory = MelonPreferences.CreateCategory("MakeItMoreDifficult");

		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			bool flag = sceneName == "Main";
			if (flag)
			{
	
                int day = 1;

                foreach (Customer customer in Customer.UnlockedCustomers)
                {
					MelonLogger.Msg("Max Spend " + customer.CustomerData.MaxWeeklySpend);

                    customer.customerData.MinWeeklySpend = day * 1000;
                    customer.customerData.MaxWeeklySpend = day * 2000;
				}


				this.InMainGame = true;
				MelonCoroutines.Start(Core.WaitForPlayer());
			}
			else
			{
				this.InMainGame = false;
				Core.HasPlayerSpawned = false;
			}
		}

		public override void OnUpdate()
		{
			if (payNow || Input.GetKeyDown(KeyCode.P))
			{
				Console.SubmitCommand("changecash -" + Core.debt);
				payNow = false;
			}

			if (Input.GetKeyDown(KeyCode.L))
				foreach (Customer customer in Customer.UnlockedCustomers)
				{
					customer.CustomerData.MaxWeeklySpend = 10000;
					MelonLogger.Msg("Max Spend " + customer.CustomerData.MaxWeeklySpend);
				}
        }


        private static IEnumerator WaitForPlayer()
		{
			while (Player.Local == null || Player.Local.gameObject == null)
			{
				yield return null;
			}
			bool flag = !Core.HasPlayerSpawned;
			if (flag)
			{
				Core.HasPlayerSpawned = true;
				MelonCoroutines.Start(Core.OnPlayerSpawned());
				//ClassInjector.RegisterTypeInIl2Cpp<UIDragging>();
			}
			yield break;
		}

		private static IEnumerator OnPlayerSpawned()
		{
			yield return new WaitForSeconds(1f);
			bool flag = Player.Local.gameObject != null;
			if (flag)
			{
				Core.DayUIElement = Object.Instantiate<GameObject>(GameObject.Find("UI/HUD/Background"));
				GameObject ParentObj = GameObject.Find("UI/HUD/");
				Core.DayUIElement.transform.SetParent(ParentObj.transform);
				Vector3 SavedPosition = new Vector3(DefaultPosition.x, DefaultPosition.y, 0f);
				Core.DayUIElement.transform.localPosition = SavedPosition;
				Core.DayUIElement.SetActive(true);
				Core.DayUIElement.name = "MakeItMoreDifficult";
				Core.DayText = GameObject.Find("UI/HUD/MakeItMoreDifficult/TopScreenText").GetComponent<TextMeshProUGUI>();
				TimeManager TManager = Object.FindObjectOfType<TimeManager>();
				TManager.onDayPass += new Action(Core.ChangeDayText);
				UpdateText(TManager);
				//Core.DayUIElement.AddComponent<UIDragging>();
				ParentObj = null;
				SavedPosition = default(Vector3);
				TManager = null;
			}
			yield break;
		}

		private static void ChangeDayText()
		{
			TimeManager timeManager = Object.FindObjectOfType<TimeManager>();
			UpdateText(timeManager);
		}


		private static void UpdateText(TimeManager timeManager)
		{
			int day = timeManager.ElapsedDays + 1;
			int tax= (int)(Mathf.Floor(Mathf.Pow(day * 5, 1.5f) / 5.0f) * 5);
			debt= tax + rent;
			Core.DayText.text = "Day #" + day.ToString() + " | $" + tax + " ( +$" + rent + " = $" + debt + " )";
		}

	}

}
