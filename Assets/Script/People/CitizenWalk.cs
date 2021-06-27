using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CitizenWalk : MonoBehaviour
{

	private Map map;

	public RuntimeAnimatorController downWalkController;
	public RuntimeAnimatorController leftWalkController;

	public float walkSpeed = 2;

	public GameObject nextRoad;
	public BuildingManager.Directions generalDirection;

	public bool canDestroy = true;

	// Use this for initialization
	void Start ()
	{
		map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
		generalDirection = (BuildingManager.Directions)Mathf.RoundToInt (Random.Range (1f, 4f));
	}
	
	// Update is called once per frame
	void Update ()
	{
		DecideRandomWalk ();
	}

	private void DecideRandomWalk ()
	{
		try {
			//List of Plots that are occupied by the object
			List<Vector3> currentPlots;
			currentPlots = map.terrainGrid.CurrentOccupiedPlots (gameObject);

			if (generalDirection == BuildingManager.Directions.Down) {
				if (CheckRoad (currentPlots, BuildingManager.Directions.Down))
					generalDirection = BuildingManager.Directions.Down;
				else {
					generalDirection = (BuildingManager.Directions)Mathf.RoundToInt (Random.Range (1f, 4f));
					if (generalDirection == BuildingManager.Directions.Down)
						generalDirection = BuildingManager.Directions.Left;
				}
			}	

			if (generalDirection == BuildingManager.Directions.Left) {
				if (CheckRoad (currentPlots, BuildingManager.Directions.Left))
					generalDirection = BuildingManager.Directions.Left;
				else {
					generalDirection = (BuildingManager.Directions)Mathf.RoundToInt (Random.Range (1f, 4f));
					if (generalDirection == BuildingManager.Directions.Left)
						generalDirection = BuildingManager.Directions.Up;
				}
			}
			
			if (generalDirection == BuildingManager.Directions.Up) {
				if (CheckRoad (currentPlots, BuildingManager.Directions.Up))
					generalDirection = BuildingManager.Directions.Up;
				else {
					generalDirection = (BuildingManager.Directions)Mathf.RoundToInt (Random.Range (1f, 4f));
					if (generalDirection == BuildingManager.Directions.Up)
						generalDirection = BuildingManager.Directions.Right;
				}
			}

			if (generalDirection == BuildingManager.Directions.Right) {
				if (CheckRoad (currentPlots, BuildingManager.Directions.Right))
					generalDirection = BuildingManager.Directions.Right;
				else {
					generalDirection = (BuildingManager.Directions)Mathf.RoundToInt (Random.Range (1f, 4f));
					if (generalDirection == BuildingManager.Directions.Right)
						generalDirection = BuildingManager.Directions.Up;
				}
			}

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);	
		}
	}

	private bool CheckRoad (List<Vector3> currentPlots, BuildingManager.Directions direction)
	{
		List<GameObject> roads = map.buildingManager.CheckPlot (
			                         currentPlots,
			                         GetComponent<WorldObject> ().objectComponents, 
			                         direction, 
			                         map.roadTag);

		if (roads.Count != 0) {
			nextRoad = roads [0];
			Walk (direction);
			return true;
		} else
			return false;
	}

	private void Walk(BuildingManager.Directions direction)
	{
		try {
			float y =  map.mainTerrain.SampleHeight(transform.position)-100f;
			switch (direction) {
			case BuildingManager.Directions.Down:
				GetComponent<SpriteRenderer> ().flipX = false;
				GetComponent<Animator> ().runtimeAnimatorController = downWalkController;
				transform.position = new Vector3 (transform.position.x - walkSpeed * Time.deltaTime, y, transform.position.z);
				break;
			case BuildingManager.Directions.Right:
				GetComponent<SpriteRenderer> ().flipX = true;
				GetComponent<Animator> ().runtimeAnimatorController = downWalkController;
				transform.position = new Vector3 (transform.position.x, y, transform.position.z - walkSpeed * Time.deltaTime);
				break;
			case BuildingManager.Directions.Left:
				GetComponent<SpriteRenderer> ().flipX = false;
				GetComponent<Animator> ().runtimeAnimatorController = leftWalkController;
				transform.position = new Vector3 (transform.position.x, y, transform.position.z + walkSpeed * Time.deltaTime);
				break;
			case BuildingManager.Directions.Up:
				GetComponent<SpriteRenderer> ().flipX = true;
				GetComponent<Animator> ().runtimeAnimatorController = leftWalkController;
				transform.position = new Vector3 (transform.position.x + walkSpeed * Time.deltaTime, y, transform.position.z);
				break;
			default:
				break;
			}
		
			GetComponent<SpriteRenderer> ().sortingOrder = -(int)(transform.position.x + transform.position.z + 1);
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);	
		}
	}



}
