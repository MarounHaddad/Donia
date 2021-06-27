using System;
using UnityEngine;
using System.Collections;

public class TaxCollection:MonoBehaviour {

	public float corruptionPercentage;

	public void CollectTaxes(BuildingHouse buildingHouse, 
								City city){
		if (buildingHouse.silverCoins > 0) {
			city.silverCoins +=  (int)(buildingHouse.taxRate - Math.Ceiling(corruptionPercentage * buildingHouse.taxRate / 100.0f));
			buildingHouse.silverCoins -=  buildingHouse.taxRate;
		}
			
	}
		

}
