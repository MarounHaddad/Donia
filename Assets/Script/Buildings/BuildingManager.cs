using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildingManager :MonoBehaviour
{


	//map Object Used To Capture the main assets of the map
	private  Map map;

	//General Directions Handled by Buildings (4 directions only, no corners)
	public enum Directions
	{
		Down = 1,
		Up = 2,
		Left = 3,
		Right = 4
	}

	void Awake ()
	{
		try {
			//Initialize the map component
			map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();

		} catch (System.Exception ex) {
			Debug.Log (ex.StackTrace);
		}
	}


	/// <summary>
	///Checks if there are Objects at a certain Direction with the certain Tag
	/// it returns the list of found objects (null if not object is found)
	/// </summary>
	public  List<GameObject> CheckPlot(List<Vector3> plots, 
										List<ObjectComponent> components, 
											Directions direction, 
												string tagToCheck="", 
													BuildingService.Services serviceReceiver = BuildingService.Services.None, 
														BuildingService.Services serviceProvider = BuildingService.Services.None)
	{
		try {
			List<GameObject> entities = new List<GameObject> ();
			int xPos;
			int yPos;
			int zPos;

			foreach (ObjectComponent component in components) {

				List<Vector3> checkPlots = new List<Vector3>();
				switch (direction) {
				case Directions.Down:
					checkPlots = plots.Where(p => p.x.Equals(plots[0].x)).ToList();
					break;
				case Directions.Up:
					checkPlots = plots.Where(p => p.x.Equals(plots[0].x + component.Size.x-1)).ToList();
					break;
				case Directions.Left:
					checkPlots = plots.Where(p => p.z.Equals(plots[0].z + component.Size.z-1)).ToList();
					break;
				case Directions.Right:
					checkPlots = plots.Where(p => p.z.Equals(plots[0].z)).ToList();
					break;
				default:
					checkPlots = plots.Where(p => p.x.Equals(plots[0].x)).ToList();
					break;
				}

				foreach (Vector3 plot in checkPlots) {
					GameObject entity;	

					switch (direction) {
					case Directions.Down:
						xPos = (int)(plot.x - 1);
						zPos = (int)(plot.z);
						break;
					case Directions.Up:
						xPos = (int)(plot.x + 1);
						zPos = (int)(plot.z);
						break;
					case Directions.Left:
						xPos = (int)(plot.x);
						zPos = (int)(plot.z + 1);
						break;
					case Directions.Right:
						xPos = (int)(plot.x);
						zPos = (int)(plot.z - 1);
						break;
					default:
						xPos = (int)(plot.x - 1);
						zPos = (int)(plot.z);
						break;
					}

					yPos = (int)(plot.y);

					//return the entity at that position
					entity = map.terrainGrid.GetPlotContentAtPosition (new Vector3 (xPos, yPos, zPos));
					
					if (entity == null)
						continue;
					
					if (entities.Contains(entity))
						continue;

					if (tagToCheck != ""){
						if (entity.tag != tagToCheck)
							continue;
					}else if (serviceReceiver != BuildingService.Services.None){
						
						if (BuildingService.GetServiceReceiverbyService(entity,serviceReceiver) == null)
							continue;
					}else if (serviceProvider != BuildingService.Services.None){

						if (BuildingService.GetServiceProviderbyService(entity,serviceProvider) == null)
							continue;
					}

					entities.Add (entity);
				}
			}

			return entities;

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
			return null;
		}
	}


}
