using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RiskEffectDisease : MonoBehaviour
{

	public enum Diseases
	{
		none = 0,
		Malaria = 1,
		Plague = 2,
		Pneumonia = 3,
		Leprosy = 4,
		Cholera = 5
	}

	public Diseases disease;

	public bool canSpread = true;

	//The Cycle in Weeks on Which the Disease will attempt to spread
	public int diseaseSpreadCycle = 1;

	//The Risk of the Disease being Spread by this building (Example Risk is 1 in 3)
	public int diseaseSpreadRisk = 3;

	//The cycle For Calling Help from surrounding doctors
	public int callForHelpCycle = 1;

	//The risk that the diseased person will die
	public int occupiantDyingRisk = 3;

	//The Risk of this building catching the Disease
	public int catchingDiseaseRisk = 3;

	//The Radius in which the diseased Building will Call for Doctors Help
	public int doctorsCheckArea = 30;

	//The chance of the house being healed by doctors
	public int canHealbyDoctorChance = 3;

	//The chance of the house being healed on its own
	public int canHealOnItsOwnChance = 20;

	public Color diseasedBuildingColor;

	//Map Object used to access the Main Assets
	private Map map;
	//Time Counter used to Count Cycles
	private TimeCounter timeCounter;

	//The current Color Of the object, used in case the building return
	//to its original state
	private Color currentObjectColor;

	// Use this for initialization
	void OnEnable ()
	{
		try {
			//Initiate map
			map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
			//Initiate Time Counter
			timeCounter = new TimeCounter (map);

			//store the current color to be reverted later
			currentObjectColor = GetComponent<WorldObject> ().originalColor;

			//Color The Building In Orange (Set its original Color to Orange)
			GetComponent<WorldObject> ().originalColor = new Color (diseasedBuildingColor.r, 
				diseasedBuildingColor.g,
				diseasedBuildingColor.b, 
				GetComponent<WorldObject> ().originalColor.a);
			GetComponent<SpriteRenderer> ().color = GetComponent<WorldObject> ().originalColor;

			//generate a sick person
			switch (disease) {
			case	Diseases.Leprosy :
				GetComponent<PeopleManager> ().GenerateSpecialPerson (GetComponent<BuildingHouse> ().LeprosyPerson);
				break;
			case	Diseases.Plague :
				GetComponent<PeopleManager> ().GenerateSpecialPerson (GetComponent<BuildingHouse> ().plaguedPerson);
				break;
			default:
				GetComponent<PeopleManager> ().GenerateSpecialPerson (GetComponent<BuildingHouse> ().sickPerson);
			break;
			}


			//Call Doctors for Help at the start of the Disease
			CallForHelp ();

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}

	}

	// Update is called once per frame
	void Update ()
	{
		//Update Timer
		timeCounter.UpdateSeconds ();
		//Try To Spread The Disease
		TrySpreadDisease ();
	}

	/// <summary>
	///  Spread the Disease Every Cycle in 4 directions
	/// with a Risk of the Occupiant Dying
	/// </summary>
	private void TrySpreadDisease ()
	{
		try {

			//Call Doctors for Help every callForHelpCycle and if the house can heal on itsown
			if (timeCounter.CheckCycle (callForHelpCycle)){
				CallForHelp ();

				//Check the Risk of Occupiant Dying
				//risk is randomly Calculated 1 in OccupiantDyingRisk
				if (GetComponent<BuildingHouse> () != null){
					if (RiskManager.CheckRisk (occupiantDyingRisk)) {
						//if RiskCheck Return True set the kill the occupiant
						GetComponent<BuildingHouse> ().OccupiantDied ();
						this.enabled = false;
					}
				}

				//Check the chance of the house healing on its own
				if (RiskManager.CheckRisk (canHealOnItsOwnChance)) {
					this.enabled = false;
				}

			}


			//if this disease type cannot spread, do not continue
			if (!canSpread)
				return;
			
			//Check of it is time to Spread the Disease
			if (!timeCounter.CheckCycle (diseaseSpreadCycle))
				return;

			
			//Attemp to spread the Disease in all Directions
			SpreadDisease (BuildingManager.Directions.Up);
			SpreadDisease (BuildingManager.Directions.Down);
			SpreadDisease (BuildingManager.Directions.Left);
			SpreadDisease (BuildingManager.Directions.Right);

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// Try to Spread the Disease in a Direction
	/// </summary>
	private void SpreadDisease (BuildingManager.Directions direction)
	{
		try {
			//Get the List of all linked Buildings
			List<GameObject> linkedBuildings = map.buildingManager.CheckPlot (
				                                   GetComponent<WorldObject> ().objectPlots,
				                                   GetComponent<WorldObject> ().objectComponents, 
				                                   direction);

			//For every found Building in that Direction
			//Check the Risk of Catching the Disease
			//risk is randomly Calculated 1 in DiseaseSpreadRisk
			foreach (GameObject building in linkedBuildings) {
				//If the Building has RiskEffectDisease Componant (Can be Diseased)
				if (building.GetComponent<RiskEffectDisease> () == null)
					return;

				//If the Building is not occupied, skip
				if (!building.GetComponent<BuildingHouse> ().Occupied)
					return;
				
				//Check the Risk 1 in diseaseSpreadRisk
				if (RiskManager.CheckRisk (diseaseSpreadRisk)) {
					RiskEffectDisease riskDisease = GetRiskDiseasebyDisease (building, disease);
					//if the Risk Check is True, Enable The Disease for that building 
					riskDisease.enabled = true;
				}
			}

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// When building is  Diseased, call surrounding doctors for Help
	/// Within a certain Radius "doctorsCheckArea"
	/// </summary>	
	private void CallForHelp ()
	{
		try {
			//The Radius between each doctor and the  Building
			float radius;

			//List of doctors in the City
			GameObject[] doctors = GameObject.FindGameObjectsWithTag (map.doctorTag);

			//if no doctors found return
			if (doctors == null)
				return;

			//if no doctors found return
			if (doctors.Length == 0)
				return;

			//loop through each doctor
			foreach (GameObject doctor in doctors) {
				//get the radius between the doctor and the building
				radius = Vector3.Distance (doctor.transform.position, transform.position);
				//if radius larger than max radius skip this doctor
				if (radius > doctorsCheckArea)
					continue;
				//disable CitizenWalk and Enable Navigation
				doctor.GetComponent<CitizenWalk> ().enabled = false;
				doctor.GetComponent<NavMeshAgent> ().enabled = true;
				doctor.GetComponent<FreeWalk> ().enabled = true;
				//if the doctor already has this building in his queue skip
				if (!doctor.GetComponent<Healing> ().diseasedBuildings.Contains (gameObject))
					doctor.GetComponent<Healing> ().diseasedBuildings.Add (gameObject);
				//Enable firefighting for the firefighter
				doctor.GetComponent<Healing> ().enabled = true;

			}

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// Heal the Current building
	/// </summary>
	public void Heal ()
	{
		try {

			this.enabled = false;

			//Return the Color to its original
			GetComponent<WorldObject> ().originalColor = currentObjectColor;
			GetComponent<SpriteRenderer> ().color = GetComponent<WorldObject> ().originalColor;

			//Destroy the Sick Person GameObject
			if (GetComponent<PeopleManager> ().currentPerson != null &&
			    GetComponent<PeopleManager> ().currentPerson.tag == map.sickPersonTag) {

				Destroy (GetComponent<PeopleManager> ().currentPerson);
				GetComponent<PeopleManager> ().currentPerson = null;
			}


		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	public void OnDisable ()
	{
		Heal ();
	}

	/// <summary>
	/// returns the Disease Risk Effect related a disease
	/// </summary>
	public static RiskEffectDisease GetRiskDiseasebyDisease (GameObject entity, Diseases diseaseToCheck)
	{
		try {
			if (entity.GetComponents<RiskEffectDisease> () == null)
				return null;

			if (entity.GetComponents<RiskEffectDisease> ().Length == 0)
				return null;

			foreach (RiskEffectDisease riskEffectDisease in entity.GetComponents<RiskEffectDisease>()) {
				if (riskEffectDisease.disease == diseaseToCheck)
					return riskEffectDisease;
			}
			return null;
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
			return null;
		}
	}

}
