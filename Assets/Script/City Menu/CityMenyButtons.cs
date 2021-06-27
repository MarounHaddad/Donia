using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityMenyButtons : MonoBehaviour {

	public SelectionManager selectionManager;

	private Map map;

	void Awake ()
	{
		try {
			map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DestroyBuilding(){
		
		if (selectionManager.selectedObject == null)
			return;

		if (selectionManager.selectedObject.GetComponent<RiskEffectDamage>() == null)
			return;

		//if (UnityEditor.EditorUtility.DisplayDialog ("Destruction Cost " + selectionManager.selectedObject.GetComponent<WorldObject> ().destructionCost,
		//	   											"Are you sure you want destroy it?", "Yes", "No"))
		//	TryDestroyBuilding ();

	}

	void TryDestroyBuilding(){
		try {

			selectionManager.selectedObject.GetComponent<RiskEffectDamage>().enabled=true;

			map.city.silverCoins -= selectionManager.selectedObject.GetComponent<WorldObject>().destructionCost;

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	public void GoHome(){
		try {
			if (selectionManager.selectedObject.GetComponent<WorldObject>().parentBuilding == null)
				return;
			
				GameObject parentBuilding = selectionManager.selectedObject.GetComponent<WorldObject>().parentBuilding;
				selectionManager.ClearSelection();
			selectionManager.SelectObject(parentBuilding);
			
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	public void SelectPerson(){
		try {
			if (selectionManager.selectedObject.GetComponent<PeopleManager>() == null)
				return;

			if (selectionManager.selectedObject.GetComponent<PeopleManager>().currentPerson == null)
				return;
			
				GameObject currentPerson = selectionManager.selectedObject.GetComponent<PeopleManager>().currentPerson;
				selectionManager.ClearSelection();
				selectionManager.SelectObject(currentPerson);

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

}
