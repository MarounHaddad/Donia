using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BuildingCorners : MonoBehaviour
{

	//The Sprites for Each Corner
	public Sprite cornerDownSprite;
	public Sprite cornerUpSprite;
	public Sprite cornerLeftSprite;
	public Sprite cornerRightSprite;
	public Sprite intersectionSprite;
	public Sprite downOnlySprite;
	public Sprite upOnlySprite;

	//The Tag to be Checked, in order to identify if the Object is a Road Or Wall or...
	public string tagToCheck;

	//map Object Used To Capture the main assets of the map
	private Map map;

	void Awake ()
	{
		try {

			map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}
		
	/// <summary>
	///Checks all Corners of teh Object(Top,Bottom,Left,Right) 
	///and if found alter their Sprites to the proper sprite
	/// if Cascade is set to True, it will apply the checkcorner functiont to every corner
	/// where an object is found
	/// </summary>
	public void CheckCorners (bool cascade = false)
	{
		try {
			GameObject upEntity = map.buildingManager.CheckPlot (
				                      GetComponent<WorldObject> ().objectPlots, 
				                      GetComponent<WorldObject> ().objectComponents,
				                      BuildingManager.Directions.Up,
				                      tagToCheck).FirstOrDefault ();
			
			GameObject downEntity =  map.buildingManager.CheckPlot (
				                        GetComponent<WorldObject> ().objectPlots, 
				                        GetComponent<WorldObject> ().objectComponents,
				                        BuildingManager.Directions.Down,
				                        tagToCheck).FirstOrDefault ();
			
			GameObject leftEntity =  map.buildingManager.CheckPlot (
				                        GetComponent<WorldObject> ().objectPlots, 
				                        GetComponent<WorldObject> ().objectComponents,
				                        BuildingManager.Directions.Left,
				                        tagToCheck).FirstOrDefault ();
			
			GameObject rightEntity =  map.buildingManager.CheckPlot (
				                         GetComponent<WorldObject> ().objectPlots, 
				                         GetComponent<WorldObject> ().objectComponents,
				                         BuildingManager.Directions.Right,
				                         tagToCheck).FirstOrDefault ();

			bool onUp = (upEntity != null);
			bool onDown = (downEntity != null);
			bool onLeft = (leftEntity != null);
			bool onRight = (rightEntity != null);

			if (onUp && onDown && onLeft && onRight)
				GetComponent<SpriteRenderer> ().sprite = intersectionSprite;

			if (onUp && onDown && !onLeft && !onRight)
				GetComponent<SpriteRenderer> ().flipX = true;

			if (!onUp && !onDown && onLeft && onRight)
				GetComponent<SpriteRenderer> ().flipX = false;

			if (!onUp && onDown && onLeft && !onRight) {
				GetComponent<SpriteRenderer> ().sprite = cornerRightSprite;
				GetComponent<SpriteRenderer> ().flipX = false;	
			}

			if (!onUp && onDown && !onLeft && onRight)
				GetComponent<SpriteRenderer> ().sprite = cornerUpSprite;

			if (onUp && !onDown && onLeft && !onRight)
				GetComponent<SpriteRenderer> ().sprite = cornerDownSprite;

			if (onUp && !onDown && !onLeft && onRight) {
				GetComponent<SpriteRenderer> ().sprite = cornerLeftSprite;
				GetComponent<SpriteRenderer> ().flipX = false;	
			}

			if (onUp && !onDown && onLeft && onRight)
				GetComponent<SpriteRenderer> ().sprite = downOnlySprite;

			if (onUp && onDown && onLeft && !onRight) {
				GetComponent<SpriteRenderer> ().sprite = downOnlySprite;
				GetComponent<SpriteRenderer> ().flipX = true;
			}
			if (!onUp && onDown && onLeft && onRight) {
				GetComponent<SpriteRenderer> ().sprite = upOnlySprite;
			}

			if (onUp && onDown && !onLeft && onRight) {
				GetComponent<SpriteRenderer> ().sprite = upOnlySprite;
				GetComponent<SpriteRenderer> ().flipX = true;
			}

			if (cascade) {
				if (onUp)
					upEntity.GetComponent<BuildingCorners> ().CheckCorners (false);
				if (onDown)
					downEntity.GetComponent<BuildingCorners> ().CheckCorners (false);
				if (onLeft)
					leftEntity.GetComponent<BuildingCorners> ().CheckCorners (false);
				if (onRight)
					rightEntity.GetComponent<BuildingCorners> ().CheckCorners (false);
			}

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

}
