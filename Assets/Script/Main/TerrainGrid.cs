using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainGrid : MonoBehaviour
{

	// Use this for initialization
	public bool[,] Plots;
	public Dictionary<string,GameObject> plotContent;
	private Map map;
	private TimeCounter timeCounter;

	public int BuildCycle = 1;

	void Awake ()
	{
		try {

			map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
			timeCounter = new TimeCounter(map);

			//initialize the terrain plot grid to have the same size as the terrain size
			Plots = new bool[ (int)GetComponent<Terrain> ().terrainData.size.x, 
				(int)GetComponent<Terrain> ().terrainData.size.z];

			//initialize the terrain plot Dictionary Content that will hold each object by Position
			plotContent = new Dictionary<string,GameObject> ();

		} catch (System.Exception ex) {
			Debug.Log (ex.StackTrace);
		}
	}

	// Update is called once per frame
	void Update ()
	{
		timeCounter.UpdateSeconds ();
		if (timeCounter.CheckCycle(BuildCycle)) {
			map.terrainsurfaceMesh.BuildNavMesh ();
		}
	}

	public bool PlotOccupied (Vector3 objectPosition, List<ObjectComponent> objectComponents)
	{
		int xPos;
		int zPos;

		foreach (ObjectComponent component in objectComponents) {
			for (int x = 1; x <= component.Size.x; x++) {
				for (int z = 1; z <= component.Size.z; z++) {
					xPos = (int)(objectPosition.x + component.Position.x + x);
					zPos = (int)(objectPosition.z + component.Position.z + z);

					if (Plots [xPos, zPos])
						return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	///Reserve a place on the Terrain Map
	///For Every Component in the object
	/// </summary>
	public List<Vector3> CurrentOccupiedPlots (GameObject entity)
	{
		int xPos;
		int yPos;
		int zPos;
		List<Vector3> currentPlots  = new List<Vector3> ();
		foreach (ObjectComponent component in entity.GetComponent<WorldObject>().objectComponents) {
			for (int x = 1; x <= component.Size.x; x++) {
				for (int z = 1; z <= component.Size.z; z++) {

					xPos = (int)(entity.transform.position.x + component.Position.x + x);
					yPos = 0;
					zPos = (int)(entity.transform.position.z + component.Position.z + z);
					currentPlots.Add (new Vector3 (xPos, yPos, zPos));
				}
			}
		}

		return currentPlots;

	}

	/// <summary>
	///Reserve a place on the Terrain Map
	///For Every Component in the object
	/// </summary>
	public void OccupyPlots (GameObject entity)
	{
		int xPos;
		int yPos;
		int zPos;
		entity.GetComponent<WorldObject> ().objectPlots = new List<Vector3> ();
		foreach (ObjectComponent component in entity.GetComponent<WorldObject>().objectComponents) {
			for (int x = 1; x <= component.Size.x; x++) {
				for (int z = 1; z <= component.Size.z; z++) {
					
					xPos = (int)(entity.transform.position.x + component.Position.x + x);
					yPos = 0;
					zPos = (int)(entity.transform.position.z + component.Position.z + z);

					Plots [xPos, zPos] = true;
					if (!plotContent.ContainsKey (GetPlotContentKey (new Vector3 (xPos, yPos, zPos))))
						plotContent.Add (GetPlotContentKey (new Vector3 (xPos, yPos, zPos)), entity);
					entity.GetComponent<WorldObject> ().objectPlots.Add (new Vector3 (xPos, yPos, zPos));
				}
			}
		}

	}

	/// <summary>
	///Clears the already occupied location
	/// </summary>
	public void ClearPlots (GameObject entity)
	{
		int xPos;
		int yPos;
		int zPos;

		foreach (ObjectComponent component in entity.GetComponent<WorldObject>().objectComponents) {
			for (int x = 1; x <= component.Size.x; x++) {
				for (int z = 1; z <= component.Size.z; z++) {

					xPos = (int)(entity.transform.position.x + component.Position.x + x);
					yPos = 0;
					zPos = (int)(entity.transform.position.z + component.Position.z + z);

					Plots [xPos, zPos] = false;
					if (plotContent.ContainsKey (GetPlotContentKey (new Vector3 (xPos, yPos, zPos))))
						plotContent.Remove (GetPlotContentKey (new Vector3 (xPos, yPos, zPos)));
					entity.GetComponent<WorldObject> ().objectPlots.Remove (new Vector3 (xPos, yPos, zPos));
				}
			}
		}

	}

	/// <summary>
	/// Get the vector of the Snap 
	/// used in Hovering object (When placing a new building) to get the correct position of the Object
	/// </summary>
	public Vector3 GetSnapPosition (float snapX, float snapZ)
	{
		Vector3 hoverObjectPosition;
		Ray castPoint = map.mainCamera.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (castPoint, out hit, Mathf.Infinity, map.terrainHitLayer)) {
			hoverObjectPosition = new Vector3 ();
			hoverObjectPosition.x = Mathf.RoundToInt (hit.point.x / snapX) * snapX;
			hoverObjectPosition.y = Mathf.RoundToInt (hit.point.y);
			hoverObjectPosition.z = Mathf.RoundToInt (hit.point.z / snapZ) * snapZ;
			return hoverObjectPosition;
		}
		return Vector3.zero;
	}

	/// <summary>
	/// Get the vector of the Mouse Click on the Screen
	/// </summary>
	public Vector3 GetClickPosition ()
	{
		Vector3 clickPosition;
		Ray castPoint = map.mainCamera.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (castPoint, out hit, Mathf.Infinity, map.terrainHitLayer)) {
			clickPosition = new Vector3 ();
			clickPosition.x = hit.point.x ;
			clickPosition.y = hit.point.y;
			clickPosition.z = hit.point.z;
			return clickPosition;
		}
		return Vector3.zero;
	}

	/// <summary>
	/// Gets the Key of PlotContent from the position Vector (the Key is of Form "x,y,z"
	/// </summary>
	public string GetPlotContentKey (Vector3 position)
	{
		return position.x + "," + position.y + "," + position.z;
	}

	/// <summary>
	/// Gets the Object at a certain Position
	/// </summary>
	public GameObject GetPlotContentAtPosition (Vector3 position)
	{
		//call GetPlotContentKey to build the Key of PlotContent from the position Vector (the Key is of Form "x,y,z"
		//if the Plot contains an Object at that position return the object at that position
		if (plotContent.ContainsKey (GetPlotContentKey (position)))
			return plotContent [GetPlotContentKey (position)];
		else
			return null;
	}

	/// <summary>
	/// Adjusts the Sorting Depth of a Building according to its position and Size in Order to 
	/// Keep the nearest Sprites Drawn First
	/// </summary>
	public void SetSortingOrder(GameObject entity){
		entity.GetComponent<SpriteRenderer>().sortingOrder =   -(int)(entity.transform.position.x + 
																		entity.transform.position.z + 
																			entity.GetComponent<WorldObject>().objectComponents[0].Size.x ) ;
	}

	public void PaintTerrain(Vector3 size, Vector3 position,int textureIndex){
		try {
			TerrainData terrainData = map.mainTerrain.terrainData;
			//get current paint mask
			int x =(int) (((position.x- map.mainTerrain.transform.position.x)/terrainData.size.x) * (terrainData.alphamapWidth));
			int z =(int) (((position.z- map.mainTerrain.transform.position.z)/terrainData.size.z) * (terrainData.alphamapHeight));

			float[, ,] alphas = terrainData.GetAlphamaps(Mathf.RoundToInt(x),Mathf.RoundToInt(z), (int)size.x,(int)size.z);

			// make sure every grid on the terrain is modified
			for (int i = 0; i < (int)(size.x); i++)
			{
				for (int j =0; j < (int)(size.z); j++)
				{
					//for each point of mask do:
					//paint all from old texture to new texture (saving already painted in new texture)

					for (int index = 0; index < alphas.GetLength(2);index++){
						if (index == textureIndex)
							alphas[i, j, index] = 1;
						else
							alphas[i, j, index] = 0f;
					}

				}
			}

			// apply the new alpha
			terrainData.SetAlphamaps(x,z,alphas);
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

		
}
