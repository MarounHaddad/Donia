using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceReceiver : MonoBehaviour {

	//The Service Received by this Building
	public BuildingService.Services service;
	//List of Buildings(Service Providers) That are Providing the Service for this Building
	public List<GameObject> serviceBuildings;

	//The Maximum Satisfaction that can be reached by this Building for this service
	public int maxSatisfaction = 100;
	//The current Satisfaction that is reached for this Service
	public int satisfaction = 100;
	//The rate of satisfaction deduction that occures every Cycle (satisfactionCheckCycle)
	public int deduction = 10;
	//The rate of satisfaction Cycle Check (in Weeks) 
	public int satisfactionCheckCycle = 1;

	//map object used to access the main assets
	private Map map;
	//Time Counter to Count the Cycles
	private TimeCounter timeCounter;

	// Update is called once per frame
	void Start ()
	{
		//initiate map
		map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
		//initiate Counter
		timeCounter = new TimeCounter (map);
	}


	// Update is called once per frame
	void Update ()
	{
		if (timeCounter == null)
			return;
		//Update Counter Seconds
		timeCounter.UpdateSeconds();
	}

	/// <summary>
	/// Check if a satisfaction Cycle has passed for this Service
	/// Note that there might be multiple ServiceReceivers on the Same Object
	/// each with his own cycle 
	/// </summary>
	public bool CheckCycle(){
		if( timeCounter.CheckCycle(satisfactionCheckCycle))
				return true;
			else 
				return false;
	}
}
