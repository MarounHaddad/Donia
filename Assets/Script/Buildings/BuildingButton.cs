using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildingButton : MonoBehaviour
{

	public BuildingLookup buildingLookup;

	public bool tileMode = true;
	public bool BoxMode = false;

	public int maxRows = 999;
	public int maxColumns = 999;

	public Text textRountCount;
	public Text textColumnCount;

	private bool inTileMode = false;

	private Map map;

	private Vector3 initialTile;
	private Vector3 finalTile;

	private int buildingColumnsTile;
	private int buildingRowsTile;

	private int buildingColumnsPreviouseTile;
	private int buildingRowsPreviouseTile;

	private List<GameObject> buildings;

	private  GameObject initialBuilding;

	public int currentSpriteIndex;
	private List<Sprite> possibleSprites;

	void Awake ()
	{
		map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
		currentSpriteIndex = 0;

		if (textRountCount != null && textColumnCount != null) {
			textRountCount.text = "";
			textColumnCount.text = "";
		}
	}


	public void GenerateBuilding ()
	{
		//if another entity is being placed in Hover mode, do not allow generating a new building
		if (map.currentClickedButton != null)
			return;

		possibleSprites =  buildingLookup.GetSelectedBuilding ().GetComponent<WorldObject> ().possibleSprites;
		currentSpriteIndex = 0;

		//generate Building
		initialBuilding = GenerateBuildingAtPosition (new Vector3 (), true);

		//set the current button to this button, so no other building button can be placed
		//untill this one is placed
		map.currentClickedButton = gameObject;

	}

	public void regenerateBuilding ()
	{

		initialBuilding = GenerateBuildingAtPosition (new Vector3 (), true);
		map.currentClickedButton = gameObject;

	}

	public GameObject GenerateBuildingAtPosition (Vector3 buildingPosition, bool hover, bool flipX = false)
	{
		GameObject building;
		building = buildingLookup.GetSelectedBuilding ();
		if (building.Equals (null))
			return null;
		building = (GameObject)(GameObject.Instantiate (building,
			buildingPosition, Quaternion.Euler (new Vector3 (0, 45, 0))));
		building.GetComponent<HoverObject> ().enabled = true;
		building.GetComponent<HoverObject> ().hover = hover;
		building.GetComponent<SpriteRenderer> ().color = new Color (0, 255, 0);
		building.GetComponent<SpriteRenderer> ().flipX = flipX;

		if (possibleSprites !=null && possibleSprites.Count >0)
			building.GetComponent<SpriteRenderer> ().sprite = possibleSprites [currentSpriteIndex];
		return building;
	}

	void Update ()
	{
		try {
			ChangeSprite();
			TileBuildings ();
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	void TileBuildings ()
	{
		if (!tileMode)
			return;
		
		Vector3 localPosition = new Vector3 ();
		localPosition = map.terrainGrid.GetSnapPosition (buildingLookup.GetSelectedBuilding ().GetComponent<HoverObject> ().snapX,
			buildingLookup.GetSelectedBuilding ().GetComponent<HoverObject> ().snapZ);

		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			
			if (initialBuilding == null)
				inTileMode = false;
			else
				inTileMode = true;
			
			initialTile = localPosition;
			buildings = new List<GameObject> ();
		}

		if (Input.GetMouseButtonDown (1)) {
			inTileMode = false;
		}

		if (!inTileMode)
			return;
		
		if (Input.GetKey (KeyCode.Mouse0)) {
			finalTile = localPosition;

			Vector3 newBuildingPosition = new Vector3 ();
			Vector3 buildingSize = buildingLookup.GetSelectedBuilding ().GetComponent<WorldObject> ().objectComponents [0].Size;
			int xFactor = 1;
			int zFactor = 1;
			if (finalTile.x < initialTile.x)
				xFactor = -1;
			if (finalTile.z < initialTile.z)
				zFactor = -1;
			
			buildingRowsTile = Mathf.Abs (Mathf.FloorToInt ((finalTile.x - initialTile.x) /
			(int)buildingLookup.GetSelectedBuilding ().GetComponent<WorldObject> ().objectComponents [0].Size.x)) + 1;
		
			buildingColumnsTile = Mathf.Abs (Mathf.FloorToInt ((finalTile.z - initialTile.z) /
			(int)buildingLookup.GetSelectedBuilding ().GetComponent<WorldObject> ().objectComponents [0].Size.z)) + 1;

			bool flipX = false;
			if (buildingRowsTile > buildingColumnsTile)
				flipX = true;
			
			if (buildingRowsPreviouseTile == buildingRowsTile && buildingColumnsPreviouseTile == buildingColumnsTile)
				return;

		
			foreach (GameObject building in buildings) {
				map.terrainGrid.ClearPlots (building);
				Destroy (building);

			}

	
			buildings.Clear ();

			buildingRowsPreviouseTile = buildingRowsTile;
			buildingColumnsPreviouseTile = buildingColumnsTile;
				
			//Fill Footer Labels according to the Rows and Columns Count
			if (textRountCount != null && textColumnCount != null) {
				textRountCount.text = "R:" + buildingRowsTile.ToString ();
				textColumnCount.text = "C:" + buildingColumnsTile.ToString ();
			}

			for (int rowBuilding = 0; rowBuilding < buildingRowsTile; rowBuilding++) {
				for (int columnBuilding = 0; columnBuilding < buildingColumnsTile; columnBuilding++) {
					newBuildingPosition = new Vector3 (initialTile.x + (rowBuilding * buildingSize.x * xFactor), 
						initialTile.y,
						initialTile.z + (columnBuilding * buildingSize.z * zFactor));
					if (!map.terrainGrid.PlotOccupied (newBuildingPosition,
						    buildingLookup.GetSelectedBuilding ().GetComponent<WorldObject> ().objectComponents)) {

						if (!flipX) {
							if (rowBuilding >= maxRows || columnBuilding >= maxColumns)
								continue;
						} else {
							if (rowBuilding >= maxColumns || columnBuilding >= maxRows)
								continue;
						}

						if (BoxMode) {
							if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
								if (buildingColumnsTile >= buildingRowsTile) {
									if (rowBuilding == 0 || columnBuilding == buildingColumnsTile - 1)
										buildings.Add (GenerateBuildingAtPosition (newBuildingPosition, false, flipX));
								} else {
									if (columnBuilding == 0 || rowBuilding == buildingRowsTile - 1)
										buildings.Add (GenerateBuildingAtPosition (newBuildingPosition, false, flipX));
								}
									
							} else {
								if (rowBuilding == 0 || rowBuilding == buildingRowsTile - 1 || columnBuilding == 0 || columnBuilding == buildingColumnsTile - 1)
									buildings.Add (GenerateBuildingAtPosition (newBuildingPosition, false, flipX));
							}
						} else
							buildings.Add (GenerateBuildingAtPosition (newBuildingPosition, false, flipX));
							

						if (initialBuilding != null) {
							//map.terrainGrid.ClearPlots (initialBuilding);
							Destroy (initialBuilding);
						}
					}
				}
			}
			if (buildings.Count == 0)
				map.currentClickedButton = null;
		} 
	
	}

	private void ChangeSprite(){
		try {
			if (possibleSprites == null)
				return;
			if (possibleSprites.Count == 0)
				return;
			if (!Input.GetKeyUp(KeyCode.R))
				return;

			if (currentSpriteIndex == possibleSprites.Count -1)
				currentSpriteIndex = 0;
			else
				currentSpriteIndex +=1;

			if (initialBuilding != null)
				initialBuilding.GetComponent<SpriteRenderer>().sprite = possibleSprites[currentSpriteIndex];
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

}
