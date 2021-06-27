using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiskManager : MonoBehaviour
{

	//current minimum satisfaction of all the services needed by this building
	//Used to color the building in dark shade according to its lowest satisfaction
	private int minSatisfaction = 100;
	
	// Update is called once per frame
	void Update ()
	{
		//Check the risks according to the Check Cycle of each Service Need
		CheckRisks ();
	}

	/// <summary>
	/// Check the risks according to the Check Cycle of each Service Need
	/// and Calculates the Risks of the consequences of the Service not being Met
	/// </summary>
	public void CheckRisks ()
	{
		try {
			//old Minimum satisfaction used to check if the Satisfaction did not change 
			//do Not color the buildings
			int oldMinSatisfaction = minSatisfaction;
			//set the Minimum Satisfaction to 100
			minSatisfaction = 100;
		
			//For Every Service Received by This Building
			foreach (ServiceReceiver serviceReceiver in GetComponents<ServiceReceiver>()) {
				//if the satifaction for that service is less than the Minimum
				//set the Minimum equal to this service Satisfaction
				if (serviceReceiver.satisfaction < minSatisfaction)
					minSatisfaction = serviceReceiver.satisfaction;

				//if it is Time To Check the Service
				if (!serviceReceiver.CheckCycle ())
					continue;

				//If No building is providing the Service
				//Reduce the service Satisfaction by the Deduction Rate
				if (serviceReceiver.serviceBuildings.Count == 0) {
					if (serviceReceiver.satisfaction > 0)
						serviceReceiver.satisfaction -= serviceReceiver.deduction;
				} else {
					//refule the satisfaction for this service
					//set it to Max
					serviceReceiver.satisfaction = serviceReceiver.maxSatisfaction;
				}

				//Check the Risk of the consequences of this Service 
				//according to the current Level of Satisfaction
				riskEffect (serviceReceiver);

				//if the satifaction for that service is less than the Minimum
				//set the Minimum equal to this service Satisfaction
				if (serviceReceiver.satisfaction < minSatisfaction)
					minSatisfaction = serviceReceiver.satisfaction;
			}


			//if the Satisfaction is Not 100 and the Satisfaction did not change do not color
			if (minSatisfaction != 100) {
				if (oldMinSatisfaction == minSatisfaction)
					return;
			} else {
				//if the Minimum Satisfaction is 100 and the Building Color is already White do not Color
				if (GetComponent<WorldObject> ().originalColor == Color.white)
					return;
			}

			//if the Building has a differnt color than its original color (Example Selected) do not color
			if (GetComponent<SpriteRenderer> ().color != GetComponent<WorldObject> ().originalColor)
				return;

			//if the Building is on fire do Not Color
			if (GetComponent<RiskEffectFire> () != null)
			if (GetComponent<RiskEffectFire> ().enabled)
				return;

			//if the building is diseased do not color
			if (GetComponent<BuildingHouse> () != null)
			if (GetComponent<BuildingHouse> ().IsDiseased ())
				return;
			
			//Set the Orignal Color Darkness according to the Current Satisfaction
			GetComponent<WorldObject> ().originalColor = new Color (
				(255 - (200 - (2 * minSatisfaction))) / 255f,
				(255 - (200 - (2 * minSatisfaction))) / 255f,
				(255 - (200 - (2 * minSatisfaction))) / 255f,
				GetComponent<WorldObject> ().originalColor.a);

			//color the Sprite with the new Original Color
			GetComponent<SpriteRenderer> ().color = GetComponent<WorldObject> ().originalColor;

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// Calculate the Consequences Risk of the Service 
	/// Example the Risk of Setting a building on fire because the Maintenance is not satisfied
	/// </summary>
	private void riskEffect (ServiceReceiver serviceReceiver)
	{
		bool willApplyRisks;
		bool willSetOnFire;
		bool willbeSick;

		RiskEffectDisease disease;


		willApplyRisks = CheckRisk (serviceReceiver.satisfaction * serviceReceiver.satisfaction);

		if (!willApplyRisks)
			return;
		
		switch (serviceReceiver.service) {

		//in Case of Maintenance Service
		case BuildingService.Services.Maintenance:

			//if the building can be set on fire and is Already on Fire
			//Do not do anything, the rest is handled by the RiskEffectFire Component
			if (GetComponent<RiskEffectFire> () != null) {
				if (GetComponent<RiskEffectFire> ().enabled)
					return;

				//if the building can be set on fire
				//Check the Risk of Fire if 
				if (GetComponent<RiskEffectFire> () != null) {
					//Check the Likelyhood of this building catching fire
					willSetOnFire = CheckRisk (GetComponent<RiskEffectFire> ().catchingFireRisk);
					if (willSetOnFire) {
						//if check risk is True, set building on fire
						GetComponent<RiskEffectFire> ().enabled = true;
						return;
					}
				}
			}

			//Destroy the Building, if the building did not catch fire
			GetComponent<RiskEffectDamage> ().enabled = true;

			break;

		//in Case of Water Service
		case BuildingService.Services.Water:

			//if the house is set not occupied do not proceed
			if (GetComponent<BuildingHouse> () != null)
			if (!GetComponent<BuildingHouse> ().Occupied)
				return;

				//Get malria Risk Effect Component
			disease = RiskEffectDisease.GetRiskDiseasebyDisease (gameObject, RiskEffectDisease.Diseases.Cholera);

			if (disease != null) {
				//Check the Likelyhood of this building catching Cholera
				willbeSick = CheckRisk (disease.catchingDiseaseRisk);
				if (willbeSick) {
					//if check risk is True, infect house with Cholera
					disease.enabled = true;
					return;
				}
			}

			break;

		//in Case of Health Care Service
		case BuildingService.Services.HealthCare:

			//if the house is set not occupied do not proceed
			if (GetComponent<BuildingHouse> () != null)
			if (!GetComponent<BuildingHouse> ().Occupied)
				return;

			//for every disease component of the house
			foreach (RiskEffectDisease diseaseRisk in GetComponents<RiskEffectDisease>()) {
				
				//Check the Likelyhood of this building having the disease
				willbeSick = CheckRisk (diseaseRisk.catchingDiseaseRisk);
				if (willbeSick) {
					//if check risk is True, infect house with the disease
					diseaseRisk.enabled = true;
					return;
				}

			}
				

			break;

		default:
			break;
		}
	}

	/// <summary>
	/// Calculate the Risk of something happening
	/// by generating a random number between 0 and the Risk value and checking if the value is 0
	/// if the Risk is set to 0 the check is over 2 (risk=>1/3)
	/// </summary>
	public static bool CheckRisk (int value)
	{
		try {
			if (value == 0)
				value = 2;
			if (0 == Mathf.RoundToInt (Random.Range (0, value)))
				return true;
			else
				return false;
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
			return false;
		}
	}



}
