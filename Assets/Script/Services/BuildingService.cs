using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingService : MonoBehaviour
{

	//List of Services that can be provided by the Building
	public enum Services
	{
		None = 0,
		Water = 1,
		Maintenance = 2,
		HealthCare = 3,
		Security = 4,
		TaxCollection = 5
	}

	//LOWER CLASS PARAMETERS
	//Number of needed Citizens of Lower Class
	public int neededLowerCitizen = 0;
	//Wages of the Lower Class Citizen
	public int wageLowerCitizen = 0;
	//List of Employed Lower Citizens (Houses Lower)
	private List<GameObject> employedLowerHouses;

	//MIDDLE CLASS PARAMETERS
	//Number of needed Citizens of Middle Class
	public int neededMiddleCitizen = 0;
	//Wages of the Middle Class Citizen
	public int wageMiddleCitizen = 0;
	//List of Employed Middle Citizens (Houses Middle)
	private List<GameObject> employedMiddleHouses;

	//UPPER CLASS PARAMETERS
	//Number of needed Citizens of Upper Class
	public int neededUpperCitizen = 0;
	//Wages of the Upper Class Citizen
	public int wageUpperCitizen = 0;
	//List of Employed Upper Citizens (Houses Upper)
	private List<GameObject> employedUpperHouses;

	//The Number of Weeks On which the service is distributed
	public int eminateCycleWeeks = 1;

	//The number of Blocks (Squares of size 1) where the service will be distributed
	//The Service is distributed in Four directions on Roads only and from roads passed to Houses
	public int serviceArea = 0;

	//The service Type Distributed by this buildings
	public Services service;

	//Used to Color in Green all the connected Roads without filling their Service
	public bool highlight = false;

	//List of Roads where the service is distributed
	private List<GameObject> emanatedRoads;

	//List of Houses where the service is distributed
	private List<GameObject> emanatedHouses;

	//List of Highlighted Roads
	private List<GameObject> highlightedRoads;

	//List of Highlighted houses
	private List<GameObject> highlightedHouses;

	//map Object Used To Capture the main assets of the map
	private Map map;

	//Time Counter that calculates/Checks the cycles
	private TimeCounter timeCounter;

	//First Step Executed
	void Awake ()
	{
		try {
			
			//initiate the map object
			map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();

			//initiate the time Counter
			timeCounter = new TimeCounter (map);

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	// Use this for initialization
	void Start ()
	{
		//initiate all Lists
		emanatedRoads = new List<GameObject> ();
		highlightedRoads = new List<GameObject> (); 

		emanatedHouses = new List<GameObject> ();
		highlightedHouses = new List<GameObject> (); 

		employedLowerHouses = new List<GameObject> ();
		employedMiddleHouses = new List<GameObject> ();
		employedUpperHouses = new List<GameObject> ();

	}
	
	// Update is called once per frame
	void Update ()
	{
		//Update the seconds so the cycle calculation works properly
		timeCounter.UpdateSeconds ();

		//Emanate the Roads=> propagate the Service throught the Roads (Then From Roads to Houses)
		EmanateRoads ();
	}

	/// <summary>
	/// Emanate the Roads, propagate the service of the game object accross all the Roads that are connected
	/// To the Object whithin in a certain radius
	/// it calls the function EnimateRoad recursively passing the Road as a parameter
	/// For the first step it passes itself (The Service Building)
	/// </summary>
	void EmanateRoads ()
	{
		try {

			//If The Number of Required Weeks To Eminate the Service is not yet passed Do not Continue
			if (timeCounter.CheckCycle (eminateCycleWeeks) && !highlight) {

				//Clear the Enamnated Roads/Employed houses before Emanating the Roads again
				ClearEmanated ();

				//Eminate the Roads, recursively - a call for every direction-Hilight is false in this case
				EmanateRoad (gameObject, highlight, BuildingManager.Directions.Up);
				EmanateRoad (gameObject, highlight, BuildingManager.Directions.Down);
				EmanateRoad (gameObject, highlight, BuildingManager.Directions.Left);
				EmanateRoad (gameObject, highlight, BuildingManager.Directions.Right);
			
				EmanateHouses (highlight);

			}

			//If in Hilight Mode
			if (highlight) {

				//Clear the hilighted Roads before highlighting the Roads again
				ClearHighlighted ();

				//highlight the Roads, recursively
				EmanateRoad (gameObject, highlight, BuildingManager.Directions.Up);
				EmanateRoad (gameObject, highlight, BuildingManager.Directions.Down);
				EmanateRoad (gameObject, highlight, BuildingManager.Directions.Left);
				EmanateRoad (gameObject, highlight, BuildingManager.Directions.Right);

				EmanateHouses (highlight);
			}

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// Emanate the Roads according to a direction and to the Current Road
	/// This Funcion is Called Recursively untill all the Roads in 4 Directions not yet Emanated and within Radius are Covered 
	/// </summary>
	void EmanateRoad (GameObject road, 
	                  bool highlightMode, 
	                  BuildingManager.Directions direction)
	{
		try {
			//List of Plots that are occupied by the object
			List<Vector3> currentPlots;

			if (highlightMode)
				//if hilight Mode - the Building is in Hover Mode (Not yet placed)
				//get the temporary Plots currently occupied by the building
				currentPlots = map.terrainGrid.CurrentOccupiedPlots (road);
			else
				//if not Hilight Mode, the object is placed
				//Get the actual Plots occupied by the Object
				currentPlots = road.GetComponent<WorldObject> ().objectPlots;
			
			//Get the List of Roads according to the direction passed to Emanate Road
			//Top,Down,Right,Left
			List<GameObject> linkedRoads = map.buildingManager.CheckPlot (
				                               currentPlots,
				                               road.GetComponent<WorldObject> ().objectComponents, 
				                               direction, 
				                               map.roadTag);

			//For Each Road found on Top
			//Call the Function Recursively
			foreach (GameObject linkRoad in linkedRoads) {

				if (direction == BuildingManager.Directions.Up)
					//if the Road Found did not cross the Service Ara
				if (linkRoad.transform.position.x > gameObject.transform.position.x + serviceArea)
					continue;

				if (direction == BuildingManager.Directions.Down)
					//if the Road Found did not cross the Service Ara
				if (linkRoad.transform.position.x < gameObject.transform.position.x - serviceArea)
					continue;

				if (direction == BuildingManager.Directions.Left)
					//if the Road Found did not cross the Service Ara
				if (linkRoad.transform.position.z > gameObject.transform.position.z + serviceArea)
					continue;

				if (direction == BuildingManager.Directions.Right)
					//if the Road Found did not cross the Service Ara
				if (linkRoad.transform.position.z < gameObject.transform.position.z - serviceArea)
					continue;


				if (highlightMode) {
					//if Road Already Highlighted Skip
					if (!highlightedRoads.Contains (linkRoad)) {
						//Color Road in Green
						linkRoad.GetComponent<SpriteRenderer> ().color = Color.green;
						//Enable the Sprite of the Road
						linkRoad.GetComponent<SpriteRenderer> ().enabled = true;

						//Add Road to List of Highlighted Roads
						highlightedRoads.Add (linkRoad);
						//Emanate Through the Roads (in all directions) Recursively
						EmanateRoad (linkRoad, highlightMode, BuildingManager.Directions.Up);
						EmanateRoad (linkRoad, highlightMode, BuildingManager.Directions.Down);
						EmanateRoad (linkRoad, highlightMode, BuildingManager.Directions.Left);
						EmanateRoad (linkRoad, highlightMode, BuildingManager.Directions.Right);

					}
				} else {
					//if Road Already Emnanated Skip
					if (!emanatedRoads.Contains (linkRoad)) {
						//Add Road to List of Emanated Roads
						emanatedRoads.Add (linkRoad);
						//Emanate Through the Roads (in all directions) Recursively
						EmanateRoad (linkRoad, highlightMode, BuildingManager.Directions.Up);
						EmanateRoad (linkRoad, highlightMode, BuildingManager.Directions.Down);
						EmanateRoad (linkRoad, highlightMode, BuildingManager.Directions.Left);
						EmanateRoad (linkRoad, highlightMode, BuildingManager.Directions.Right);
					}
				}
			}

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	private void EmanateHouses (bool highlightMode)
	{
		if (highlightMode) {
			//Loop Through hilighted Roads and set their Color to Green
			foreach (GameObject road in highlightedRoads) {
				if (road == null)
					continue;
				//Emanated through the Houses Surrrounding the Road
				//Emanate through the Houses First than emanate through the connected Roads
				EmanateHouse (road, highlightMode, BuildingManager.Directions.Up, false);
				EmanateHouse (road, highlightMode, BuildingManager.Directions.Down, false);
				EmanateHouse (road, highlightMode, BuildingManager.Directions.Left, false);
				EmanateHouse (road, highlightMode, BuildingManager.Directions.Right, false);	
			}
		} else {
			//Loop Through Emanated Roads and set their Color to Green
			foreach (GameObject road in emanatedRoads) {
				if (road == null)
					continue;
				//Emanated through the Houses Surrrounding the Road
				//Emanate through the Houses First than emanate through the connected Roads
				EmanateHouse (road, highlightMode, BuildingManager.Directions.Up, true);
				EmanateHouse (road, highlightMode, BuildingManager.Directions.Down, true);
				EmanateHouse (road, highlightMode, BuildingManager.Directions.Left, true);
				EmanateHouse (road, highlightMode, BuildingManager.Directions.Right, true);	
			}


			//Loop Through Emanated Roads and set their Color to Green
			foreach (GameObject road in emanatedRoads) {
				if (road == null)
					continue;
				//Emanated through the Houses Surrrounding the Road
				//Emanate through the Houses First than emanate through the connected Roads
				EmanateHouse (road, highlightMode, BuildingManager.Directions.Up, false);
				EmanateHouse (road, highlightMode, BuildingManager.Directions.Down, false);
				EmanateHouse (road, highlightMode, BuildingManager.Directions.Left, false);
				EmanateHouse (road, highlightMode, BuildingManager.Directions.Right, false);	
			}
		}

	}

	/// <summary>
	/// Emanate the Houses according to a direction and to the Current Road
	/// This Funcion is Called 4 times for the 4 directions for every Road found in EmanateRoad
	/// </summary>
	private void EmanateHouse (GameObject road, 
	                           bool highlightMode, 
	                           BuildingManager.Directions direction, 
	                           bool employMode)
	{
		try {
			//List of Plots that are occupied by the object
			List<Vector3> currentPlots;
			if (highlightMode)
				//if hilight Mode - the object is in Hover Mode
				//get the temporary Plots currently occupied by the building
				currentPlots = map.terrainGrid.CurrentOccupiedPlots (road);
			else
				//if not Hilight Mode, the object is placed
				//Get the actual Plots occupied by the Object
				currentPlots = road.GetComponent<WorldObject> ().objectPlots;

			//Get the List of Houses according to the direction passed to Emanate House
			//Top,Down,Right,Left
			List<GameObject> linkedHouses = map.buildingManager.CheckPlot (
				                                currentPlots,
				                                road.GetComponent<WorldObject> ().objectComponents, 
				                                direction, "", service);

			foreach (GameObject house in linkedHouses) {

				if (highlightMode) {
					//if House Already Highlighted Skip
					if (!highlightedHouses.Contains (house)) {
						//Color In Green
						house.GetComponent<SpriteRenderer> ().color = Color.green;
						//add House to the List of Highlighted Houses
						highlightedHouses.Add (house);
					}
				} else {
				
					if (employMode &&
					    house.GetComponent<BuildingHouse> () != null &&
					    house.GetComponent<HoverObject> ().enabled == false) {

						//Try to Employ the house in case it is not yet employed
						Employ (house.GetComponent<BuildingHouse> ());

					} else {
						//if House Already Emanated Skip
						if (!emanatedHouses.Contains (house)) {
							if (CanProvideService ()) {
								//Add The Service of this Building to the House 
								if (GetServiceReceiverbyService (house, service) != null &&
								    house.GetComponent<HoverObject> ().enabled == false) {
									GetServiceReceiverbyService (house, service).serviceBuildings.Add (gameObject);
									ServiceAction(house);
								}

								//add House to the List of Highlighted Houses
								emanatedHouses.Add (house);
							}
						}		
					}
				}

			}

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}



	/// <summary>
	/// Checks the House(Componant BuidlingHouse) if it has the needed Employees of every Class
	/// </summary>
	private void Employ (BuildingHouse buildingHouse)
	{
		if (buildingHouse == null)
			return;
		
		/// if the Number of needed Lower Citizens is not yet met
		if (employedLowerHouses.Count < neededLowerCitizen) {
			//if the House is Lower Class
			if (buildingHouse.citizenType == BuildingHouse.CitizenTypes.Lower) {
				//if the House is Not yet Employed, Employ the House
				if (buildingHouse.employedBuilding == null) {
					//if the building was occupied(occupiant not dead)
					if (buildingHouse.Occupied) {
						//if City was able to Pay Wage
						if (PayWage (buildingHouse)) {
							buildingHouse.employedBuilding = gameObject;
							employedLowerHouses.Add (buildingHouse.gameObject);
						}
					}
				}
			}
		}

		/// if the Number of needed Middle Citizens is not yet met
		if (employedMiddleHouses.Count < neededMiddleCitizen) {
			//if the House is Middle Class
			if (buildingHouse.citizenType == BuildingHouse.CitizenTypes.Middle) {
				//if the House is Not yet Employed, Employ the House
				if (buildingHouse.employedBuilding == null) {
					//if the building was occupied(occupiant not dead)
					if (buildingHouse.Occupied) {
						//if City was able to Pay Wage
						if (PayWage (buildingHouse)) {
							buildingHouse.employedBuilding = gameObject;
							employedMiddleHouses.Add (buildingHouse.gameObject);
						}
					}
				}
			}
		}

		/// if the Number of needed Upper Citizens is not yet met
		if (employedUpperHouses.Count < neededUpperCitizen) {
			//if the House is Upper Class
			if (buildingHouse.citizenType == BuildingHouse.CitizenTypes.Upper) {
				//if the House is Not yet Employed, Employ the House
				if (buildingHouse.employedBuilding == null) {
					//if the building was occupied(occupiant not dead)
					if (buildingHouse.Occupied) {
						//if City was able to Pay Wage
						if (PayWage (buildingHouse)) {
							buildingHouse.employedBuilding = gameObject;	
							employedUpperHouses.Add (buildingHouse.gameObject);
						}
					}
				}
			}
		}
		
	}

	/// <summary>
	/// Checks if the city has the needed Coins Deduct the Wage(Per Class from the Citizen Coins reserver
	/// Return True (to allow Employing the House
	/// </summary>
	public bool PayWage (BuildingHouse buildingHouse)
	{
		//if the citizen class needed is Lower class 
		//check the Lower Class wage demanded by this Building can be afforded by the City
		if (buildingHouse.citizenType == BuildingHouse.CitizenTypes.Lower) {
			if (map.city.silverCoins >= wageLowerCitizen) {
				//deduct the wage from teh City treasury
				map.city.silverCoins -= wageLowerCitizen;
				buildingHouse.silverCoins += wageLowerCitizen;
				return true;
			} else {
				return false;
			}
			//if the citizen class needed is Middle class 
			//check the Middle Class wage demanded by this Building can be afforded by the City
		} else if (buildingHouse.citizenType == BuildingHouse.CitizenTypes.Middle) {
			if (map.city.silverCoins >= wageMiddleCitizen) {
				//deduct the wage from teh City treasury
				map.city.silverCoins -= wageMiddleCitizen;
				buildingHouse.silverCoins += wageMiddleCitizen;
				return true;
			} else {
				return false;
			}
			//if the citizen class needed is Upper class 
			//check the Upper Class wage demanded by this Building can be afforded by the City
		} else if (buildingHouse.citizenType == BuildingHouse.CitizenTypes.Upper) {
			if (map.city.silverCoins >= wageUpperCitizen) {
				//deduct the wage from teh City treasury
				map.city.silverCoins -= wageUpperCitizen;
				buildingHouse.silverCoins += wageUpperCitizen;
				return true;
			} else {
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Checks if the building has the needed staff to provide the service
	/// </summary> 
	public bool CanProvideService ()
	{
		//Check Lower Class Employees
		if (employedLowerHouses.Count < neededLowerCitizen) {
			return false;
		}
		//Check Middle Class Employees
		if (employedMiddleHouses.Count < neededMiddleCitizen) {
			return false;
		}
		//Check Upper Class Employees
		if (employedUpperHouses.Count < neededUpperCitizen) {
			return false;
		}
		return true;
	}


	/// <summary>
	/// Clear all Emanated Roads and Houses
	/// </summary>
	public void ClearEmanated ()
	{
		try {
		
			//Loop Through Emanated Houses Remove The Service from the House
			foreach (GameObject house in emanatedHouses) {
				if (house == null)
					continue;
				if (GetServiceReceiverbyService (house, service) == null)
					continue;
				GetServiceReceiverbyService (house, service).serviceBuildings.Remove (gameObject);
			}

			//Loop Through Employed Lower Class Houses and Clear their Employed Building (Free Them)
			foreach (GameObject house in employedLowerHouses) {
				if (house == null)
					continue;
				house.GetComponent<BuildingHouse> ().employedBuilding = null;
			}

			//Loop Through Employed Middle Class Houses and Clear their Employed Building (Free Them)
			foreach (GameObject house in employedMiddleHouses) {
				if (house == null)
					continue;
				house.GetComponent<BuildingHouse> ().employedBuilding = null;
			}

			//Loop Through Employed Upper Class Houses and Clear their Employed Building (Free Them)
			foreach (GameObject house in employedUpperHouses) {
				if (house == null)
					continue;
				house.GetComponent<BuildingHouse> ().employedBuilding = null;
			}

			//Clear All emanated and employed Lists
			emanatedRoads.Clear ();
			emanatedHouses.Clear ();
			employedLowerHouses.Clear ();
			employedMiddleHouses.Clear ();
			employedUpperHouses.Clear ();

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// Clear all Highlighted Roads and Houses
	/// </summary>
	public void ClearHighlighted ()
	{
		try {

			//Loop Through the Highlighted Roads and Reset their color
			foreach (GameObject road in highlightedRoads) {
				if (road == null)
					continue;
				road.GetComponent<SpriteRenderer> ().color = road.GetComponent<WorldObject> ().originalColor;
				road.GetComponent<SpriteRenderer> ().enabled = false;
			}

			//Loop Through the Highlighted Houses and Reset their color
			foreach (GameObject house in highlightedHouses) {
				if (house == null)
					continue;
				house.GetComponent<SpriteRenderer> ().color = house.GetComponent<WorldObject> ().originalColor;
			}

			//Clear All Highlighted Lists
			highlightedRoads.Clear ();
			highlightedHouses.Clear ();

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// When the Object is destrory Clear all Eminated and Highlighted
	/// Freeing the Employ Houses and Clearing their services
	/// </summary>
	void OnDestroy ()
	{
		try {
			ClearEmanated ();
			ClearHighlighted ();
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// Loop Thourgh Roads and Color Them
	/// Loop Through the Serviced Houses and Color Them
	/// Loop Through the Employed Houses and Color Them
	/// </summary>
	public void ColorEmanated (Color  emanatedColor, Color employedColor)
	{
		try {
			
			//Loop Through Emanated Roads and set their Color to "emanated Color"
			foreach (GameObject road in emanatedRoads) {
				if (road == null)
					continue;
				
				if (emanatedColor == Color.white) {
					road.GetComponent<SpriteRenderer> ().color = road.GetComponent<WorldObject> ().originalColor;
					road.GetComponent <SpriteRenderer> ().enabled = false;
				} else {
					road.GetComponent<SpriteRenderer> ().color = emanatedColor;
					road.GetComponent <SpriteRenderer> ().enabled = true;
				}
					
			}

			//Loop Through Emanated Houses and set their Color to "emanated Color"
			foreach (GameObject house in emanatedHouses) {
				if (house == null)
					continue;
				if (emanatedColor == Color.white)
					house.GetComponent<SpriteRenderer> ().color = house.GetComponent<WorldObject> ().originalColor;
				else
					house.GetComponent<SpriteRenderer> ().color = emanatedColor;
			}

			//Loop Through Employed Lower Class Houses and  set their Color to "Employed Color"
			foreach (GameObject house in employedLowerHouses) {
				if (house == null)
					continue;
				if (employedColor == Color.white)
					house.GetComponent<SpriteRenderer> ().color = house.GetComponent<WorldObject> ().originalColor;
				else
					house.GetComponent<SpriteRenderer> ().color = employedColor;
			}

			//Loop Through Employed Middle Class Houses and  set their Color to "Employed Color"
			foreach (GameObject house in employedMiddleHouses) {
				if (house == null)
					continue;
				if (employedColor == Color.white)
					house.GetComponent<SpriteRenderer> ().color = house.GetComponent<WorldObject> ().originalColor;
				else
					house.GetComponent<SpriteRenderer> ().color = employedColor;
			}

			//Loop Through Employed Upper Class Houses and  set their Color to "Employed Color"
			foreach (GameObject house in employedUpperHouses) {
				if (house == null)
					continue;
				if (employedColor == Color.white)
					house.GetComponent<SpriteRenderer> ().color = house.GetComponent<WorldObject> ().originalColor;
				else
					house.GetComponent<SpriteRenderer> ().color = employedColor;
			}
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// returns the List of ServiceReceiver Components on a certain object
	/// according to a service
	/// </summary>
	public static ServiceReceiver GetServiceReceiverbyService (GameObject entity, Services service)
	{
		try {
			if (entity.GetComponents<ServiceReceiver> () == null)
				return null;

			if (entity.GetComponents<ServiceReceiver> ().Length == 0)
				return null;
			
			foreach (ServiceReceiver serviceReceiver in entity.GetComponents<ServiceReceiver>()) {
				if (serviceReceiver.service == service)
					return serviceReceiver;
			}
			return null;
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
			return null;
		}
	}

	/// <summary>
	/// returns the List of ServiceProvider Components on a certain object
	/// according to a service
	/// </summary>
	public static BuildingService GetServiceProviderbyService (GameObject entity, Services service)
	{
		try {
			if (entity.GetComponents<BuildingService> () == null)
				return null;

			if (entity.GetComponents<BuildingService> ().Length == 0)
				return null;

			foreach (BuildingService serviceProvider in entity.GetComponents<BuildingService>()) {
				if (serviceProvider.service == service)
					return serviceProvider;
			}
			return null;
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
			return null;
		}
	}

	private void ServiceAction(GameObject building){
		try {
			if (service == Services.TaxCollection)
				if (building.GetComponent<BuildingHouse>() != null)
					GetComponent<TaxCollection>().CollectTaxes(building.GetComponent<BuildingHouse>(),map.city);
			
				
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}
}
