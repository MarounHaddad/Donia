using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiskEffectDamage : MonoBehaviour {

	//Map Object sued to access the main assets
	private Map map;

	//Dust Particles
	public GameObject dust;

	// Use this for initialization
	void Start () {
		//Initiate Map
		map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
	}
	
	// Update is called once per frame
	void Update () {
		//
		AnimateDestroy ();
	}

	/// <summary>
	/// animate the Building (Move Down Untill a certain Point)
	/// and Then Destroy it
	/// </summary>
	public void AnimateDestroy(){
		try {
			//Disable the Build Animation so the Building doesnt snap Back
			GetComponent<WorldObject>().animateBuild = false;

			//Check if the Building hasnt reached a low equal to its hight 
			//Move it down
			if (transform.position.y>-transform.localScale.y){
				//Move the building Down on the Y axis
				//The speed of the Animation is determined by "destroyspeedAnimation" parameter
				transform.position= new Vector3(transform.position.x,transform.position.y - GetComponent<WorldObject>().destroyspeedAnimation *Time.deltaTime,transform.position.z);

				//PLace the Building in the Same Sorting Layer as the Road
				//This is To properly Place the Building in the Front or Back of the Road while it goes down
				//in its normal state the building will always have a higher layer than the Road
				GetComponent<SpriteRenderer>().sortingLayerName = "Road";

			}
			else
				//when the Low point is reached 
				//Destroy the Building
				DestroyObject();
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// Destory the Building and Dust Particles 
	/// clear the Plots occupied by the Building and Destroy its related Objects
	/// </summary>
	public void DestroyObject(){

		GameObject dustAnimation;

		//Clear the current Plots of the Building
		//So Other Plots can be Placed
		map.terrainGrid.ClearPlots(gameObject);

		//Destory the Current Person (if any) linked to this building
		if (GetComponent<PeopleManager> () != null)
			if (GetComponent<PeopleManager> ().currentPerson != null)
				Destroy (GetComponent<PeopleManager> ().currentPerson);

		//Clear the Selection of the Building
		map.selectionManager.ClearSelection ();

		//Initiate a new Dust Particle Object
		dustAnimation= (GameObject)(GameObject.Instantiate (dust,
			new Vector3(transform.position.x,1,transform.position.z), Quaternion.Euler (new Vector3 (-90, 0, 0))));

		//Destroy the Object
		//The Services/Hilight will be cleared in the BuildingService Componant OnDestroy Method
		Destroy (gameObject);

		//Destory the Dust Particles after the animation had played in a second
		Destroy (dustAnimation, dustAnimation.GetComponent<ParticleSystem> ().main.duration + 1);

	}
		
}
