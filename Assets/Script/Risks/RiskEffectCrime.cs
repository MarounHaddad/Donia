using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiskEffectCrime : MonoBehaviour {

	public enum CrimeTypes
	{
		Murder = 0,
		Theft = 1,
		Arsonist = 2
	}

	public GameObject criminal;

	void OnEnable () {
		try {
			
			GetComponent<PeopleManager>().GenerateSpecialPerson(criminal);

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}

	}

}
