using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class HoverObject : MonoBehaviour
{
	private Map map;
	private Vector3 hoverObjectPosition;
	public bool hover = true;
	public float snapX = 3;
	public float snapZ = 3;
	public bool hideOnPlacement = false;
	public int textureIndex = -1;

	void Awake ()
	{
		map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
	}

	void Start(){
		gameObject.GetComponent<WorldObject> ().setServiceHighlight (true);
	}

	// Update is called once per frame
	void Update ()
	{
		try {
			SnapToGrid ();
			ColorizeStatus ();
			TryPlaceObject ();
			TryDestroyObject ();
		} catch (System.Exception ex) {
			Debug.Log (ex.StackTrace);
		}
	}

	/// <summary>
	///Move the object on the grid by an fixed interval
	/// </summary>
	void SnapToGrid ()
	{
		//Only Snap The Hover object to the mouse position if not in Tile Mode
		//When in Tile Mode the objects are placed at specific places
		if (!hover)
			return;
		transform.position = map.terrainGrid.GetSnapPosition (snapX, snapZ);
		GetComponent<SpriteRenderer> ().sortingOrder = 1000;
	}

	/// <summary>
	/// Colorize the Object according to the plot where it currently stands
	///Red if Occupied, green if Free
	/// </summary>
	void ColorizeStatus ()
	{
		if (!CanPlace ())
			//if cannot place color Red
			GetComponent<SpriteRenderer> ().color = new Color (255, 0, 0);
		else
			//if can place color Green
			GetComponent<SpriteRenderer> ().color = new Color (0, 255, 0);
	
	}

	/// <summary>
	/// Checks if the hovering object can be placed in the current position
	/// it checks the position for every component of the object
	/// </summary>
	bool CanPlace ()
	{
		//in case in TileMode always allow placing
		if (!hover)
			return true;
		return !map.terrainGrid.PlotOccupied (transform.position,
			GetComponent<WorldObject> ().objectComponents);
	}

	/// <summary>
	/// Attempt to place the hover object at a position 
	/// Checks first if the position is clear
	/// </summary>
	public bool TryPlaceObject ()
	{
		//if in hover mode (Not Tiling multiple Objects)
		//the system will check the position: if it is clear before placing the object
		if (hover) {
			//in case not Tiling (Placing one object at a Time)
			//on Left Mouse Click
			if (Input.GetMouseButtonDown (0)) {
				if (CanPlace ()) {

					PlaceObject ();

					//Regenerate a new entity to be placed in case not in Tile Mode
					if (map.currentClickedButton!= null)
						map.currentClickedButton.GetComponent<BuildingButton> ().regenerateBuilding();
					
					return true;
				}
			}
		} else {
			//in case Tiling
			//on Left Mouse Release
			if (Input.GetMouseButtonUp (0)) {

				PlaceObject ();

				//Release the current Clicked button
				//Do not generate a new entity to be placed in case in Tile Mode
				map.currentClickedButton = null;

				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Creates Object and Sets its Color To Regular
	/// Occupies the Plots in the TerrainGrid
	/// And in case of Road/Wall/... it Checks for Corners to adjust their sprites according to their position
	/// </summary>
	private void PlaceObject ()
	{
		//Give Object original Color,Stop Hovering,Occupy all plots under the object
		GetComponent<SpriteRenderer> ().color = GetComponent<WorldObject>().originalColor;

		//Turn off Hover
		GetComponent<HoverObject> ().enabled = false;

		//Occupy the plots under the current object in the terrain Grid
		map.terrainGrid.OccupyPlots (gameObject);

		//in case the Object has Cornes Component (wall, Road), the below will check if their are nearby 
		//objects of same Tag in order to adjust their sprites to preserve a connected layout
		if (gameObject.GetComponent<BuildingCorners> () != null)
			gameObject.GetComponent<BuildingCorners> ().CheckCorners (true);

		//Set the Sorting of the Object in the Terrain, According to his position and size
		//Defines what sprite will be drawn first
		map.terrainGrid.SetSortingOrder (gameObject);

		//Turn Off the Hilight mode for Service Objects
		gameObject.GetComponent<WorldObject> ().setServiceHighlight (false);

		//Place the Object
		//transform.position= new Vector3(transform.position.x, transform.position.y + (GetComponent<WorldObject>().objectComponents[0].Size.y),transform.position.z);

		//When Placing the Object Set its NavMesh Obstacle to True
		//This is to accelerate the placement of the Hover Object
		if (GetComponentInChildren<NavMeshObstacle> () != null)
			GetComponentInChildren<NavMeshObstacle> ().enabled = true;
	
		if (textureIndex!=-1)
			map.terrainGrid.PaintTerrain (GetComponent<WorldObject> ().objectComponents [0].Size,
												new Vector3(transform.position.x,0,transform.position.z),
													textureIndex);

		if (hideOnPlacement)
			GetComponent<SpriteRenderer> ().enabled = false;
		
	}


	/// <summary>
	/// attempt to destory the object at its current position and clear all
	/// its related plots
	/// </summary>
	public bool TryDestroyObject ()
	{
		//on Left Mouse Click
		if (Input.GetMouseButtonDown (1)) {

			//Clear The Hilight In case a service Object is in Hover Mode and hilighting the Roads in Contact
			gameObject.GetComponent<WorldObject> ().setServiceHighlight (false);

			Destroy (gameObject);
			//only Clear plots if NOT in hover mode (When Tiling)
			//because otherwise the hovering object will not occupy plots untill placed
			if (!hover)
				map.terrainGrid.ClearPlots (gameObject);

			//clear from previously Selected Button, so the user can place a new building
			map.currentClickedButton = null;

			return true;

		}
		return false;
	}



}
