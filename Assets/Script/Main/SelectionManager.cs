using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour {

	public GameObject selectedObject;
	public Image imageSelectedObject;
	public Text labelSelectedObject;

	public Button destroyButton;
	public Button goHomeButton;
	public Button selectPersonButton;

	private Map map;

	// Use this for initialization
	void Start () {
		map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
		ClearSelection ();
	}
	
	// Update is called once per frame
	void Update () {
		Select ();
	}

	private void Select(){
		try {
			GameObject currentSelectedObject;

			//on Left Mouse Click
			if (Input.GetMouseButtonDown (0)) {
				Ray ray = map.mainCamera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray,out hit,100)){



					currentSelectedObject = hit.transform.parent.gameObject;

					if (currentSelectedObject ==null)
						return;

					if (currentSelectedObject.GetComponent<WorldObject>() == null)
						return;

					ClearSelection();

					SelectObject(currentSelectedObject);
					
				}
			}


			if (Input.GetMouseButtonDown (1)) {
				ClearSelection();
			}
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	public void SelectObject(GameObject currentSelectedObject){
		try {
			selectedObject =currentSelectedObject;
			imageSelectedObject.sprite = currentSelectedObject.GetComponent<SpriteRenderer>().sprite;
			imageSelectedObject.color = new Color (255,255,255,255);
			labelSelectedObject.text =currentSelectedObject.GetComponent<Entity>().Name;

			selectedObject.GetComponent<SpriteRenderer>().color = Color.green;

			if (selectedObject.GetComponent<BuildingService>() !=null)
				selectedObject.GetComponent<BuildingService>().ColorEmanated(Color.green,Color.cyan);

			SetMenuButtons();

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	public void ClearSelection(){
		try {
			if (selectedObject!= null){
				selectedObject.GetComponent<SpriteRenderer>().color = selectedObject.GetComponent<WorldObject>().originalColor;
			
			if (selectedObject.GetComponent<BuildingService>() !=null)
				selectedObject.GetComponent<BuildingService>().ColorEmanated(Color.white,Color.white);
			}

			selectedObject =null;
			imageSelectedObject.color = new Color(255,255,255,0);
			imageSelectedObject.sprite = null;
			labelSelectedObject.text = "";

			destroyButton.gameObject.SetActive(false);
			goHomeButton.gameObject.SetActive(false);
			selectPersonButton.gameObject.SetActive(false);

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	public void SetMenuButtons(){
		try {


			if (selectedObject.GetComponent<RiskEffectDamage>() != null)
				destroyButton.gameObject.SetActive(true);
					
			if (selectedObject.GetComponent<WorldObject>().parentBuilding != null)
				goHomeButton.gameObject.SetActive(true);	

			if (selectedObject.GetComponent<PeopleManager>()!= null)
				if (selectedObject.GetComponent<PeopleManager>().currentPerson!= null)
				selectPersonButton.gameObject.SetActive(true);
			
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}
}
