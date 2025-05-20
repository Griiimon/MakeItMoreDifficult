using System;
using System.Collections;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;
using Console = Il2CppScheduleOne.Console;
using Il2CppScheduleOne.Economy;
using System.Collections.Generic;
using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.Persistence;
using UnityEngine.Events;
using Il2CppScheduleOne.UI;
using Il2CppFishNet;


[assembly: MelonInfo(typeof(MakeItMoreDifficult.Core), "MakeItMoreDifficult", "0.3.0", "Griiimon")]
[assembly: MelonGame(null, null)]


namespace MakeItMoreDifficult
{

	public class Core : MelonMod
	{
		private static int rent = 0;

        private static int debt = 0;

		private static bool hasPayedToday = false;

		private static bool hasPlayerSpawned;

		private static GameObject dayUIElement;

		private static TextMeshProUGUI dayText;

		private static MelonPreferences_Category configCategory;

		private static Vector3 defaultPosition = new Vector3(550f, 510f, 0f);

        private static Dictionary<Customer, float> customerOrigSpending = new Dictionary<Customer, float>();

        private static Dictionary<string, int> propertyValues = new Dictionary<string, int>();

        private static SleepCanvas sleepCanvas = null;


		public override void OnInitializeMelon()
		{
			propertyValues.Add("Motel Room", 75);
			propertyValues.Add("Sweatshop", 800);
			propertyValues.Add("Storage Unit", 5000);
			propertyValues.Add("Bungalow", 6000);
			propertyValues.Add("Barn", 25000);
			propertyValues.Add("Docks Warehouse", 50000);

			base.LoggerInstance.Msg("Loaded successfully!!");
			Core.configCategory = MelonPreferences.CreateCategory("MakeItMoreDifficult");
		}


		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			bool flag = sceneName == "Main";
			if (flag)
			{
				MelonCoroutines.Start(Core.WaitForPlayer());
			}
			else
			{
				Core.hasPlayerSpawned = false;
			}
		}


		public override void OnLateInitializeMelon()
		{
			LoadManager.Instance.onLoadComplete.AddListener((UnityAction)UpdateCalculations);
		}


		public override void OnUpdate()
		{
			if (!IsServer())
				return;

			if (Input.GetKeyDown(KeyCode.L))
				UpdateCalculations();

			else if (Input.GetKeyDown(KeyCode.P) && !hasPayedToday)
			{
				Console.SubmitCommand("changecash -" + Core.debt);
				hasPayedToday = true;
				SetSleepButtonEnabled(true);
			}
		}


		private static IEnumerator WaitForPlayer()
		{
			while (Player.Local == null || Player.Local.gameObject == null)
			{
				yield return null;
			}
			bool flag = !Core.hasPlayerSpawned;
			if (flag)
			{
				Core.hasPlayerSpawned = true;
				MelonCoroutines.Start(Core.OnPlayerSpawned());

			}
			yield break;
		}


		private static void UpdateCalculations()
		{
			MelonLogger.Msg("Update Calculations");
			UpdateRent();
			ChangedayText();
			UpdateCustomerSpending();
		}


		private static IEnumerator OnPlayerSpawned()
		{
			yield return new WaitForSeconds(1f);
			bool flag = Player.Local.gameObject != null;
			if (flag)
			{
				Core.dayUIElement = Object.Instantiate<GameObject>(GameObject.Find("UI/HUD/Background"));
				GameObject ParentObj = GameObject.Find("UI/HUD/");
				Core.dayUIElement.transform.SetParent(ParentObj.transform);
				Vector3 SavedPosition = new Vector3(defaultPosition.x, defaultPosition.y, 0f);
				Core.dayUIElement.transform.localPosition = SavedPosition;
				Core.dayUIElement.SetActive(true);
				Core.dayUIElement.name = "MakeItMoreDifficult";
				Core.dayText = GameObject.Find("UI/HUD/MakeItMoreDifficult/TopScreenText").GetComponent<TextMeshProUGUI>();
				GetTimeManager().onDayPass += new Action(Core.OnDayPass);
				UpdateText(GetTimeManager());

				ParentObj = null;
				SavedPosition = default(Vector3);
			}

			SetSleepButtonEnabled(false);

			yield break;
		}


		private static void OnDayPass()
		{
			UpdateCalculations();
			ChangedayText();
			hasPayedToday = false;
			SetSleepButtonEnabled(false);
		}


		private static void SetSleepButtonEnabled(bool flag)
		{
			if (!IsServer())
				return;

			if (sleepCanvas == null)
			{
				sleepCanvas = Object.FindObjectOfType<SleepCanvas>();
			}

			sleepCanvas.SleepButton.interactable = flag;
		}


		private static void ChangedayText()
		{
			TimeManager timeManager = GetTimeManager();
			if (timeManager != null)
				UpdateText(timeManager);
		}


		private static void UpdateText(TimeManager timeManager)
		{
			int day = timeManager.ElapsedDays + 1;
			int protectionAndBribes = (int)(Mathf.Floor(Mathf.Pow(day * 5, 1.5f) / 5.0f) * 5);
			debt = protectionAndBribes + rent;
			Core.dayText.text = "Day #" + day.ToString() + " | $" + debt + " ($" + rent + " + $" + protectionAndBribes + ")";
		}


		private static void UpdateCustomerSpending()
		{
			int day = GetTimeManager().ElapsedDays + 1;

			foreach (Customer customer in Customer.UnlockedCustomers)
			{
				if (!customerOrigSpending.ContainsKey(customer))
				{
					customerOrigSpending.Add(customer, customer.CustomerData.MaxWeeklySpend);
				}

				if (customerOrigSpending.TryGetValue(customer, out float orig_value))
				{
					customer.customerData.MaxWeeklySpend = Mathf.Max(1f, Mathf.Pow(day, 1.5f) / 100f) * orig_value;
				}
			}
		}


		private static void UpdateRent()
		{
			var list = GetOwnedProperties();
			rent = 0;
			foreach (var property in list)
			{
				propertyValues.TryGetValue(property.propertyName, out int PropertyValue);
				rent += PropertyValue;
			}

			MelonLogger.Msg("Current Rent: " + rent);
		}


		private static List<Property> GetOwnedProperties()
		{
			var result = new List<Property>();
			foreach (Property property in Property.Properties)
			{
				if (property.IsOwned)
				{
					result.Add(property);
					MelonLogger.Msg("Player owns " + property.propertyName);
				}
			}
			return result;
		}


		public static TimeManager GetTimeManager()
		{
			return Object.FindObjectOfType<TimeManager>();

		}


		private static bool IsServer()
		{
			var nm = InstanceFinder.NetworkManager;
			return nm.IsServer;
		}

    }
}