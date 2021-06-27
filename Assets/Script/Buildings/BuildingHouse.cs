﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHouse : MonoBehaviour {

	//The Types of the Citizen that can occupy this house (their Social Class)
	public enum CitizenTypes{
		Lower = 1 ,
		Middle = 2 ,
		Upper =3 
	}

	//The social Class of the Citizens in this house
	public CitizenTypes citizenType;

	//The Building that employs the current house
	public GameObject employedBuilding;

	//if the Current house is Occupied or Empty
	public bool Occupied = true;

	//The Undertaker that will be generated by this house, if the occupiant died
	public GameObject undertaker;

	//The Sick Man that will be generated if this house is Sick
	public GameObject sickPerson;

	//The Plagued Man that will be generated if this house has plague
	public GameObject plaguedPerson;

	//The Plagued Man that will be generated if this house has plague
	public GameObject LeprosyPerson;

	//the Cycle at which the house is reoccupied in case it is Empty
	public int GeneratePersonCycle = 24;

	//SilverCoins collected by this house from being employed
	public int silverCoins = 0;

	//tax rate that this house has to pay
	public int taxRate = 0;

	//Time Counter used to Count Cycles
	private TimeCounter timeCounter;
	//Map Object used to access the Main Assets
	private Map map;

	void Awake(){
		//Initiate map
		map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
	
	}

	// Update is called once per frame
	void Update ()
	{
		//Update Timer
		if (timeCounter !=null){
			timeCounter.UpdateSeconds ();
			Reoccupy ();
		}

	}

	public void OccupiantDied(){
		try {
			//if the house is already vacant do not continue
			if (!Occupied)
				return;
			
			//Set the Occupied variable to false
			Occupied  = false;

			//Initiate Time Counter
			timeCounter = new TimeCounter(map);

			//Generate the UnderTaker
			GetComponent<PeopleManager>().GenerateSpecialPerson(undertaker);

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	public void Reoccupy(){
		try {
			
			//if the house is already occupied do no continue
			if (Occupied)
				return;
			
			//Check if it is time to reoccupy the house
			if (!timeCounter.CheckCycle (GeneratePersonCycle))
				return;
	
			//Occupy the House
			Occupied = true;

			//Disable the timer
			timeCounter = null;

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	public bool IsDiseased(){
		try {
			foreach (RiskEffectDisease  disease in GetComponents<RiskEffectDisease>()) {
				if (disease.enabled)
					return true;
			}
			return false;
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
			return false;
		}
	}
}