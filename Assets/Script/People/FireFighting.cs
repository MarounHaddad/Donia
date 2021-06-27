using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class FireFighting : MonoBehaviour
{

	public List<GameObject> buildingsOnFire;
	public GameObject returnToRoad;

	public RuntimeAnimatorController TurnOffDownAnimator;
	public RuntimeAnimatorController TurnOffLeftAnimator;
	public GameObject waterSplash;

	public int chanceTurnOffFire = 20;

	//Map Object used to access the Main Assets
	private Map map;

	void Awake ()
	{
		map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
		buildingsOnFire = new List<GameObject> ();
	}

	// Update is called once per frame
	void Update ()
	{
		SeekBuilding ();
		SeekRoad ();
		ReturnToRoad ();
		CheckBuildingOnFire ();
	}

	private void CheckBuildingOnFire ()
	{
		try {
			GameObject buildingOnFire;
			Vector3 buildingDirection;

			if (buildingsOnFire == null)
				return;

			if (buildingsOnFire.Count == 0)
				return;

			if (buildingsOnFire.FirstOrDefault () == null)
				buildingsOnFire.Remove (buildingsOnFire.FirstOrDefault ());

			buildingOnFire = buildingsOnFire.FirstOrDefault ();

			if (buildingOnFire == null)
				return;
			
			if (!buildingOnFire.GetComponent<RiskEffectFire>().enabled){
				buildingsOnFire.Remove (buildingOnFire);
				return;
			}
		
			if (!GetComponent<FreeWalk> ().enabled)
				return;

			if (GetComponent<FreeWalk> ().agent == null)
				return;

			returnToRoad = null;

			NavMeshPath path = new NavMeshPath();
			GetComponent<FreeWalk>().agent.CalculatePath(buildingOnFire.transform.position, path);

			if (path.status == NavMeshPathStatus.PathInvalid || path.status == NavMeshPathStatus.PathPartial) {
				buildingsOnFire.Remove(buildingOnFire);
				return;
			}

			float dist = GetComponent<FreeWalk> ().agent.remainingDistance; 
			float radius = Vector3.Distance (buildingOnFire.transform.position, transform.position);
			if (radius > 3)
				return;

			returnToRoad = GetComponent<CitizenWalk> ().nextRoad;

			GetComponent<FreeWalk> ().enabled = false;
			GetComponent<NavMeshAgent> ().enabled = false;

			buildingDirection = (transform.position - buildingOnFire.transform.position).normalized;

			if (buildingDirection.x >= 0 && buildingDirection.z >= 0) {
				GetComponent<Animator> ().runtimeAnimatorController = TurnOffDownAnimator;
				GetComponent<SpriteRenderer> ().flipX = false;
			} else if (buildingDirection.x <= 0 && buildingDirection.z <= 0) {
				GetComponent<Animator> ().runtimeAnimatorController = TurnOffLeftAnimator;
				GetComponent<SpriteRenderer> ().flipX = true;
			} else if (buildingDirection.x <= 0 && buildingDirection.z >= 0) {
				GetComponent<Animator> ().runtimeAnimatorController = TurnOffDownAnimator;
				GetComponent<SpriteRenderer> ().flipX = true;
			} else if (buildingDirection.x >= 0 && buildingDirection.z <= 0) {
				GetComponent<Animator> ().runtimeAnimatorController = TurnOffLeftAnimator;
				GetComponent<SpriteRenderer> ().flipX = false;
			}

			TryTurnOff();

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	private void SeekBuilding ()
	{
		try {
			GameObject buildingOnFire;

			if (buildingsOnFire == null)
				return;

			if (buildingsOnFire.Count == 0)
				return;

			if (buildingsOnFire.FirstOrDefault () == null)
				buildingsOnFire.Remove (buildingsOnFire.FirstOrDefault ());

			buildingOnFire = buildingsOnFire.FirstOrDefault ();

			if (buildingOnFire == null)
				return;
			
			if (!buildingOnFire.GetComponent<RiskEffectFire>().enabled){
				buildingsOnFire.Remove (buildingOnFire);
				return;
			}


			GetComponent<FreeWalk> ().target = buildingOnFire.transform;
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	private void SeekRoad ()
	{
		try {

			if (buildingsOnFire != null && buildingsOnFire.FirstOrDefault () == null)
				buildingsOnFire.Remove (buildingsOnFire.FirstOrDefault ());

			if (buildingsOnFire != null && buildingsOnFire.Count > 0)
				return;
			
			if (returnToRoad == null) {
				GameObject[] roads = GameObject.FindGameObjectsWithTag (map.roadTag);

				if (roads == null)
					return;

				if (roads.Length == 0)
					return;

				foreach (GameObject road in roads) {
					if (Vector3.Distance (road.transform.position, transform.position) <= 100) {
						returnToRoad = road;
						GetComponent<NavMeshAgent> ().enabled = true;
						GetComponent<FreeWalk> ().target = road.transform;
						GetComponent<FreeWalk> ().enabled = true;
						break;
					}
				}
			} else {
				if (!GetComponent<FreeWalk> ().enabled || GetComponent<FreeWalk> ().target == null) {
					GetComponent<NavMeshAgent> ().enabled = true;
					GetComponent<FreeWalk> ().target = returnToRoad.transform;
					GetComponent<FreeWalk> ().enabled = true;
				}
			}
			

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	private void ReturnToRoad ()
	{
		try {
			if (buildingsOnFire != null && buildingsOnFire.FirstOrDefault () == null)
				buildingsOnFire.Remove (buildingsOnFire.FirstOrDefault ());

			if (buildingsOnFire != null && buildingsOnFire.Count > 0)
				return;
			
			if (returnToRoad == null)
				return;

			if (GetComponent<NavMeshAgent> ().enabled){
				NavMeshPath path = new NavMeshPath();
				GetComponent<FreeWalk>().agent.CalculatePath(returnToRoad.transform.position, path);

				if (path.status == NavMeshPathStatus.PathPartial||path.status == NavMeshPathStatus.PathPartial) {
					Destroy(gameObject);
					return;
				}
			}

			float radius = Vector3.Distance (returnToRoad.transform.position, transform.position);
			if (radius > 1){
				return;
			}

			returnToRoad = null;

			GetComponent<NavMeshAgent> ().enabled = false;
			GetComponent<FreeWalk> ().enabled = false;
			GetComponent<FreeWalk> ().target = null;
			GetComponent<FireFighting> ().enabled = false;
			GetComponent<CitizenWalk> ().enabled = true;

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	public void CreateSplash (string message)
	{
		try {
			
			Vector3 rotation = new Vector3 (-170, 90, -90);
			Vector3 location = new Vector3 (transform.position.x, 0.5f, transform.position.z + 1);

			if (message != "CreateSplash")
				return;
			
			GameObject currentSplash;

			if (GetComponent<Animator> ().runtimeAnimatorController == TurnOffDownAnimator &&
			    !GetComponent<SpriteRenderer> ().flipX) {
				rotation = new Vector3 (-170, 90, -90);
				location = new Vector3 (transform.position.x + 1, 0.3f, transform.position.z + 1.5f);
			} else if (GetComponent<Animator> ().runtimeAnimatorController == TurnOffDownAnimator &&
			           GetComponent<SpriteRenderer> ().flipX) {
				rotation = new Vector3 (-170, 0, -90);
				location = new Vector3 (transform.position.x + 1.5f, 0.3f, transform.position.z + 1);
			} else if (GetComponent<Animator> ().runtimeAnimatorController == TurnOffLeftAnimator &&
			           !GetComponent<SpriteRenderer> ().flipX) {
				rotation = new Vector3 (-170, 180, -90);
				location = new Vector3 (transform.position.x + 2, 0.3f, transform.position.z + 2.5f);
			} else if (GetComponent<Animator> ().runtimeAnimatorController == TurnOffLeftAnimator &&
			           GetComponent<SpriteRenderer> ().flipX) {
				rotation = new Vector3 (-170, -90, -90);
				location = new Vector3 (transform.position.x + 2.5f, 0.3f, transform.position.z + 2);
			}


			//Generate Splash
			currentSplash = (GameObject)(GameObject.Instantiate (waterSplash,
				location, Quaternion.Euler (rotation)));	

			//Sort the Splash Properly in the Layer
			currentSplash.GetComponent<ParticleSystemRenderer> ().sortingOrder = 1;
		
			Destroy (currentSplash, 2);
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}


	private void TryTurnOff(){
		try {
			ServiceReceiver serviceReceiver;

			if (!RiskManager.CheckRisk(chanceTurnOffFire))
				return;
			if (buildingsOnFire.FirstOrDefault() == null)
				return;
			
			buildingsOnFire.FirstOrDefault().GetComponent<RiskEffectFire>().enabled = false;

			serviceReceiver = BuildingService.GetServiceReceiverbyService(buildingsOnFire.FirstOrDefault(),BuildingService.Services.Maintenance);

			serviceReceiver.satisfaction = serviceReceiver.maxSatisfaction;

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}
}
