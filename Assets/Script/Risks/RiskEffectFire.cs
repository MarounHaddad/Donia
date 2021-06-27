using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RiskEffectFire : MonoBehaviour
{

	//Fire Particles
	public GameObject fire;
	//The Generated Fire Particle Object
	private GameObject currentFire;
	//The current Color Of the object, used in case the building return
	//to its original state
	private Color currentObjectColor;

	//The Cycle in Weeks on Which the Fire will attempt to spread
	public int fireSpreadCycle = 1;
	//The Risk of the Fire Spreading (Example Risk is 1 in 3)
	public int fireSpreadRisk = 3;
	//The Radius in which the occupiant of the building on fire will die
	public int occupiantDyingRisk = 3;
	//The Radius in which the Building On Fire will be destroyed
	public int destroyedByFireRisk = 3;
	//The Radius in which the Building will catch fire
	public int catchingFireRisk = 3;

	//The Radius in which the Building On Fire will Call for Firefighters Help
	public int firefighterCheckArea = 60;


	//Map Object used to access the Main Assets
	private Map map;
	//Time Counter used to Count Cycles
	private TimeCounter timeCounter;

	// Use this for initialization
	void OnEnable ()
	{
		try {
			//Initiate map
			map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
			//Initiate Time Counter
			timeCounter = new TimeCounter(map);

			//Generate Fire
			//Fire is Directly Generated when the Componant RiskEffectFire is Enabled
			currentFire = (GameObject)(GameObject.Instantiate (fire,
				new Vector3 (transform.position.x, 0, transform.position.z), Quaternion.Euler (new Vector3 (-90, 0, 0))));	

			//Sort the Fire Properly in the Layer
			currentFire.GetComponent<ParticleSystemRenderer> ().sortingOrder = GetComponent<SpriteRenderer> ().sortingOrder + 1;
			//Sor the Fire Properly in the Children of the Fire Object (Fire/Smoke...)
			foreach (ParticleSystemRenderer childFire in currentFire.GetComponentsInChildren <ParticleSystemRenderer>()) {
				childFire.sortingOrder = GetComponent<SpriteRenderer> ().sortingOrder + 1;
			}

			//store the current color to be reverted later
			currentObjectColor = GetComponent<WorldObject> ().originalColor;

			//Color The Building In Orange (Set its original Color to Orange)
			GetComponent<WorldObject> ().originalColor = new Color (255 / 255f, 180 / 255f, 180 / 255f, GetComponent<WorldObject> ().originalColor.a);
			GetComponent<SpriteRenderer> ().color = GetComponent<WorldObject> ().originalColor;

			//Call Firefighters for Help at the start of the fire
			CallForHelp();

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}

	}

	// Update is called once per frame
	void Update ()
	{
		//Update Timer
		timeCounter.UpdateSeconds ();
		//Try To Spread The Fire
		TrySpreadFire ();
	}

	/// <summary>
	///  Spread the Fire Every Cycle in 4 directions
	/// with a Risk of the Buidling Getting Destroy
	/// and the Occupiant Dying
	/// </summary>
	private void TrySpreadFire ()
	{
		try {
			//Check of it is time to Spread the Fire
			if (!timeCounter.CheckCycle (fireSpreadCycle))
				return;

			//Call Firefighters for Help every Spread Cycle
			CallForHelp();

			//Check the Risk of being Destroyed by Fire
			//risk is randomly Calculated 1 in destroyedByFireRisk
			if (RiskManager.CheckRisk (destroyedByFireRisk)) {
				//if RiskCheck Return True distroy the Building by enabling the Damage Effect
				GetComponent<RiskEffectDamage> ().enabled = true;
				return;
			}

			//Check the Risk of Occupiant Dying
			//risk is randomly Calculated 1 in OccupiantDyingRisk
			if (GetComponent<BuildingHouse>() != null)
				if (RiskManager.CheckRisk (occupiantDyingRisk)) {
					//if RiskCheck Return True set the Occupied Property to False
					GetComponent<BuildingHouse> ().OccupiantDied();
					return;
				}

			//Attemp to spread the Fire in all Directions
			SpreadFire (BuildingManager.Directions.Up);
			SpreadFire (BuildingManager.Directions.Down);
			SpreadFire (BuildingManager.Directions.Left);
			SpreadFire (BuildingManager.Directions.Right);

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// Try to Spread the Fire in a Direction
	/// </summary>
	private void SpreadFire (BuildingManager.Directions direction)
	{
		try {
			//Get the List of all linked Buildings
			List<GameObject> linkedBuildings = map.buildingManager.CheckPlot(
				                                   GetComponent<WorldObject> ().objectPlots,
				                                   GetComponent<WorldObject> ().objectComponents, 
				                                   direction);

			//For every found Building in that Direction
			//Check the Risk of being Destroyed by Fire
			//risk is randomly Calculated 1 in fireSpreadRisk
			foreach (GameObject building in linkedBuildings) {
				//If the Building has RiskEffectFire Componant (Can Catch Fire)
				if (building.GetComponent<RiskEffectFire> () == null)
					return;
				//Check the Risk 1 in fireSpreadRisk
				if (RiskManager.CheckRisk (fireSpreadRisk)) {
					//if the Risk Check is True, Enable The RiskEffectFire for that building (set it on Fire)
					building.GetComponent<RiskEffectFire> ().enabled = true;
				}
			}

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// When building is on Fire call surrounding Firfighters for Help
	/// Within a certain Radius "firefighterCheckArea"
	/// </summary>	
	private void CallForHelp(){
		try {
			//The Radius between each Firefighter and the burning Building
			float radius;

			//List of firefighters in the City
			GameObject[] firefighters = GameObject.FindGameObjectsWithTag(map.firefighterTag);

			//if not firfighters found return
			if(firefighters == null)
				return;
			
			//if not firfighters found return
			if(firefighters.Length==0)
				return;

			//loop through each firefighter
			foreach (GameObject firefighter in firefighters) {
				//get the radius between the firefigther and the building
				radius = Vector3.Distance (firefighter.transform.position, transform.position);
				//if radius larger than max radius skip this firefigther
				if (radius > firefighterCheckArea)
					continue;
				//disable CitizenWalk and Enable Navigation
				firefighter.GetComponent<CitizenWalk>().enabled =false;
				firefighter.GetComponent<NavMeshAgent>().enabled = true;
				firefighter.GetComponent<FreeWalk>().enabled = true;
				//if the firefighter already has this building in his queue skip
				if (!firefighter.GetComponent<FireFighting>().buildingsOnFire.Contains(gameObject))
					firefighter.GetComponent<FireFighting>().buildingsOnFire.Add(gameObject);
				//Enable firefighting for the firefighter
				firefighter.GetComponent<FireFighting>().enabled = true;

			}

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// Called whe the Component is destroyed
	/// Destroy the Fire after one second
	/// </summary>
	private void OnDestroy(){
		try {
			if (currentFire != null)
				Destroy (currentFire, 1);
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// Turn Off the Fire on the Current building
	/// </summary>
	public void TurnOffFire(){
		try {
			//if there is a fire particle object destroy it
			if (currentFire != null)
				Destroy (currentFire, 1);
			
			//disable RiskEffectFire Component
			GetComponent<RiskEffectFire>().enabled = false;
			//Return the Color to its original
			GetComponent<WorldObject> ().originalColor = currentObjectColor;
			GetComponent<SpriteRenderer> ().color = GetComponent<WorldObject> ().originalColor;
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	public void OnDisable(){
		TurnOffFire ();
	}
}
