using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour {

	public int silverCoins = 5000;
	public Text silverCoinsLabel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		UpdateSilverCoinsLabel ();
	}

	void UpdateSilverCoinsLabel(){
		try {
			if (silverCoinsLabel.text != silverCoins.ToString())
				silverCoinsLabel.text = silverCoins.ToString();
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}
}
