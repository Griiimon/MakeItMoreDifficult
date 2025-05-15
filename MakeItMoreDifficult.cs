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
using System.Collections.Generic;
using Harmony;
using Il2CppScheduleOne.Property;

[assembly: MelonInfo(typeof(MakeItMoreDifficult.Core), "MakeItMoreDifficult", "0.2.0", "Griiimon")]
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

		public static Dictionary<Customer, float> customer_orig_spending= new Dictionary<Customer, float>();

		public static Dictionary<string, int> PropertyValues = new Dictionary<string, int>();

		public override void OnInitializeMelon()
		{
			instance= this;

			PropertyValues.Add("Motel Room", 75);
            PropertyValues.Add("Sweatshop", 800);
            PropertyValues.Add("Storage Unit", 5000);
            PropertyValues.Add("Bungalow", 6000);
            PropertyValues.Add("Barn", 25000);
            PropertyValues.Add("Docks Warehouse", 50000);

            base.LoggerInstance.Msg("Loaded successfully!!");
			Core.ConfigCategory = MelonPreferences.CreateCategory("MakeItMoreDifficult");

		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			bool flag = sceneName == "Main";
			if (flag)
			{
	
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
			if (Input.GetKeyDown(KeyCode.P))
			{
				Console.SubmitCommand("changecash -" + Core.debt);
				UpdateRent();
				UpdateCustomerSpending();
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

				UpdateRent();
				UpdateCustomerSpending();
			
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
				GetTimeManager().onDayPass += new Action(Core.ChangeDayText);
				UpdateText(GetTimeManager());

				ParentObj = null;
				SavedPosition = default(Vector3);
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


		private static void UpdateCustomerSpending()
		{
			int day = GetTimeManager().ElapsedDays + 1;

			foreach (Customer customer in Customer.UnlockedCustomers)
			{
				if (!customer_orig_spending.ContainsKey(customer))
				{
					customer_orig_spending.Add(customer, customer.CustomerData.MaxWeeklySpend);
					//MelonLogger.Msg("!!  " + customer.name + ": Orig Spend Cap " + customer.CustomerData.MaxWeeklySpend);
				}

				if (customer_orig_spending.TryGetValue(customer, out float orig_value))
				{
					customer.customerData.MaxWeeklySpend = Mathf.Max(1f, Mathf.Pow(day, 1.5f) / 100f) * orig_value;
					//MelonLogger.Msg(customer.name + ": New Spend Cap " + customer.CustomerData.MaxWeeklySpend);
				}
			}
			MelonLogger.Msg("Updated Customer spending cap");
        }

		private static void UpdateRent()
		{
			var list = GetOwnedProperties();
			rent = 0;
			foreach(var property in list)
			{
				PropertyValues.TryGetValue(property.propertyName, out int PropertyValue);
				rent+= PropertyValue;
			}

			MelonLogger.Msg("Current Rent: " + rent);
        }


        private static List<Property> GetOwnedProperties()
		{
			var result = new List<Property>();
			foreach(Property property in Property.Properties)
			{
				if (property.IsOwned)
				{
					result.Add(property);
                    MelonLogger.Msg("Player owns" + property.propertyName);
                }
            }
			return result;
		}
		
		private static TimeManager GetTimeManager()
		{
			return Object.FindObjectOfType<TimeManager>();

        }
    }

}
