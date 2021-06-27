using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldObject : MonoBehaviour
{

	public bool occupiesPlot;

	public List<ObjectComponent> objectComponents;

	public bool randomRotation;

	public List<Sprite> randomSprites;

	public List<Sprite> possibleSprites;

	public List<Vector3> objectPlots;

	public bool animateBuild=true;

	public float buildspeedAnimation = 10;
	public float destroyspeedAnimation = 10;

	public Color originalColor = Color.white;

	public GameObject parentBuilding;

	public int destructionCost = 0;

	private Map map;

	void Awake ()
	{
		try {
			map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
			SetRandomSprite ();
			SetRandomRotation();
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	// Use this for initialization
	void Start ()
	{		
		try {
			//occupy a place in the World grid that has the same size as each Component size in the Object
			map.terrainGrid.OccupyPlots (gameObject);
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		//AnimateBuild();
	}

	void SetRandomSprite ()
	{
		int spriteIndex = 0;
		if (randomSprites == null)
			return;
		if (randomSprites.Count == 0)
			return;
		spriteIndex = Mathf.RoundToInt (Random.Range (0f, randomSprites.Count));
		if (spriteIndex == 0)
			return;
		GetComponent<SpriteRenderer> ().sprite = randomSprites [spriteIndex - 1];
	}

	void SetRandomRotation ()
	{
		if (!randomRotation)
			return;
		int roationIndex = 0;
		roationIndex = Mathf.RoundToInt (Random.Range (0f, 1f));
		if (roationIndex == 0)
			return;
		GetComponent<SpriteRenderer> ().flipX = true;
	}


	public void AnimateBuild(){
		try {
			if (!animateBuild)
					return;
			if (transform.position.y>0)
				transform.position= new Vector3(transform.position.x,transform.position.y - buildspeedAnimation *Time.deltaTime,transform.position.z);
			else{
				transform.position= new Vector3(transform.position.x,0,transform.position.z);
			}
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// used for the Service Objects
	/// To hilight them when the object is in Hover Mode and remove hilight when the Object is placed
	/// Hilight: the system will color Green all the connected roads within radius
	/// Withtout filling the services
	/// </summary>
	public void setServiceHighlight(bool highlight){
		if (gameObject.GetComponent<BuildingService> () == null)
			return;

		gameObject.GetComponent<BuildingService> ().highlight= highlight;
		gameObject.GetComponent<BuildingService> ().ClearHighlighted ();

	}



}
