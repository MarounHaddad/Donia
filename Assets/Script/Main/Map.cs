using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class Map: MonoBehaviour
{
	public  Terrain mainTerrain;
	public  Camera mainCamera;
	public  LayerMask terrainHitLayer;
	public  TerrainGrid terrainGrid;
	public	TimeManager timeManager;
	public 	BuildingManager buildingManager;
	public 	City city;
	public SelectionManager selectionManager;
	public NavMeshSurface terrainsurfaceMesh;

	//The Tag used by Gameobjects that are considered as Road
	public string roadTag;
	//The Tag used by Gameobjects that are considered as houses
	public string houseTag;
	//The Tag used by Gameobjects that are considered as Firefighters
	public string firefighterTag;
	//The Tag used by Gameobjects that are considered as Doctors
	public string doctorTag;
	//The Tag used by Gameobjects that are considered as Doctors
	public string sickPersonTag;
	//The Tag used by Gameobjects that are considered as Citizens
	public string CitizenTag;
	//The Tag used by Gameobjects that are considered as Policeman
	public string PolicemanTag;

	public GameObject currentClickedButton;

	public Map ()
	{
	}

	/// <summary>
	/// Save the terrain texture in a file
	/// </summary>
	public void SaveTerrainTexture(){
		try {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Create(Application.persistentDataPath + "/"+  SceneManager.GetActiveScene().name +"_TerrainTextures.save");
			bf.Serialize(file, mainTerrain.terrainData.GetAlphamaps(0,0,mainTerrain.terrainData.alphamapWidth,mainTerrain.terrainData.alphamapHeight));
			file.Close();
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	/// <summary>
	/// load the terrain texture from a file
	/// </summary>
	public void LoadTerrainTexture(){
		try {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/"+  SceneManager.GetActiveScene().name +"_TerrainTextures.save", FileMode.Open);
			float[,,] alphamap = (float[,,] )bf.Deserialize(file);
			mainTerrain.terrainData.SetAlphamaps(0,0 ,alphamap);
			file.Close();
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}
	 
}

