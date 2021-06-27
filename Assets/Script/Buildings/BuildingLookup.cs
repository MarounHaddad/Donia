using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BuildingLookup : MonoBehaviour {

	//List of Buildings Available in This Lookup to be Placed
	public List<GameObject> buildings;

	// Use this for initialization
	void Start () {
		try {
			PopulateLookup();
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// Fill the Lookup according to the List of Game Objects (Buildings) assigned to it
	///The Description of the Lookup is derived from the Entity (Name)
	/// </summary>
	void PopulateLookup(){
		List<string> buildingsOptions = new List<string>();
		foreach (GameObject building in buildings) {
			buildingsOptions.Add(building.GetComponent<Entity>().Name);
		}
		GetComponent<Dropdown>().AddOptions(buildingsOptions);
	}

	/// <summary>
	/// return a GameObject according to the selected Option in the Lookup
	/// </summary>
	public GameObject GetSelectedBuilding(){
		foreach (GameObject building in buildings) {
			if (building.GetComponent<Entity>().Name == 
					GetComponent<Dropdown>().options[ GetComponent<Dropdown> ().value].text) {
				return building;
			}
		}
		return null;
	}

}
