using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Healing : MonoBehaviour {

	public List<GameObject> diseasedBuildings;
	public GameObject returnToRoad;

	public RuntimeAnimatorController HealDownAnimator;
	public RuntimeAnimatorController HealLeftAnimator;


	//Map Object used to access the Main Assets
	private Map map;

	void Awake ()
	{
		map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
		diseasedBuildings = new List<GameObject> ();
	}

	// Update is called once per frame
	void Update ()
	{
		SeekBuilding ();
		SeekRoad ();
		ReturnToRoad ();
		CheckDiseasedBuilding();
	}

	private void CheckDiseasedBuilding ()
	{
		try {
			GameObject buildingDiseased;
			Vector3 buildingDirection;

			if (diseasedBuildings == null)
				return;

			if (diseasedBuildings.Count == 0)
				return;

			if (diseasedBuildings.FirstOrDefault () == null)
				diseasedBuildings.Remove (diseasedBuildings.FirstOrDefault ());

			buildingDiseased = diseasedBuildings.FirstOrDefault ();

			if (buildingDiseased == null)
				return;

			if (!buildingDiseased.GetComponent<BuildingHouse>().IsDiseased()){
				diseasedBuildings.Remove (buildingDiseased);
				return;
			}

			if (!GetComponent<FreeWalk> ().enabled)
				return;

			if (GetComponent<FreeWalk> ().agent == null)
				return;

			returnToRoad = null;

			NavMeshPath path = new NavMeshPath();
			GetComponent<FreeWalk>().agent.CalculatePath(buildingDiseased.transform.position, path);

			if (path.status == NavMeshPathStatus.PathInvalid || path.status == NavMeshPathStatus.PathPartial) {
				diseasedBuildings.Remove(buildingDiseased);
				return;
			}

			float dist = GetComponent<FreeWalk> ().agent.remainingDistance; 
			float radius = Vector3.Distance (buildingDiseased.transform.position, transform.position);
			if (radius > 3)
				return;

			returnToRoad = GetComponent<CitizenWalk> ().nextRoad;

			GetComponent<FreeWalk> ().enabled = false;
			GetComponent<NavMeshAgent> ().enabled = false;

			buildingDirection = (transform.position - buildingDiseased.transform.position).normalized;

			if (buildingDirection.x >= 0 && buildingDirection.z >= 0) {
				GetComponent<Animator> ().runtimeAnimatorController = HealDownAnimator;
				GetComponent<SpriteRenderer> ().flipX = false;
			} else if (buildingDirection.x <= 0 && buildingDirection.z <= 0) {
				GetComponent<Animator> ().runtimeAnimatorController = HealLeftAnimator;
				GetComponent<SpriteRenderer> ().flipX = true;
			} else if (buildingDirection.x <= 0 && buildingDirection.z >= 0) {
				GetComponent<Animator> ().runtimeAnimatorController = HealDownAnimator;
				GetComponent<SpriteRenderer> ().flipX = true;
			} else if (buildingDirection.x >= 0 && buildingDirection.z <= 0) {
				GetComponent<Animator> ().runtimeAnimatorController = HealLeftAnimator;
				GetComponent<SpriteRenderer> ().flipX = false;
			}

			TryHeal();

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	private void SeekBuilding ()
	{
		try {
			GameObject buildingDiseased;

			if (diseasedBuildings == null)
				return;

			if (diseasedBuildings.Count == 0)
				return;

			if (diseasedBuildings.FirstOrDefault () == null)
				diseasedBuildings.Remove (diseasedBuildings.FirstOrDefault ());

			buildingDiseased = diseasedBuildings.FirstOrDefault ();

			if (buildingDiseased == null)
				return;

			if (!buildingDiseased.GetComponent<BuildingHouse >().IsDiseased()){
				diseasedBuildings.Remove (buildingDiseased);
				return;
			}


			GetComponent<FreeWalk> ().target = buildingDiseased.transform;
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	private void SeekRoad ()
	{
		try {

			if (diseasedBuildings != null && diseasedBuildings.FirstOrDefault () == null)
				diseasedBuildings.Remove (diseasedBuildings.FirstOrDefault ());

			if (diseasedBuildings != null && diseasedBuildings.Count > 0)
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
			if (diseasedBuildings != null && diseasedBuildings.FirstOrDefault () == null)
				diseasedBuildings.Remove (diseasedBuildings.FirstOrDefault ());

			if (diseasedBuildings != null && diseasedBuildings.Count > 0)
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

			if (GetComponent<FreeWalk> ().target != returnToRoad.transform)
				GetComponent<FreeWalk> ().target =  returnToRoad.transform;
			
			float radius = Vector3.Distance (returnToRoad.transform.position, transform.position);
			if (radius > 1){
				return;
			}

			returnToRoad = null;

			GetComponent<NavMeshAgent> ().enabled = false;
			GetComponent<FreeWalk> ().enabled = false;
			GetComponent<FreeWalk> ().target = null;
			GetComponent<Healing> ().enabled = false;
			GetComponent<CitizenWalk> ().enabled = true;

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}


	private void TryHeal(){
		try {
			ServiceReceiver serviceReceiver;

			if (diseasedBuildings.FirstOrDefault() == null)
				return;

			RiskEffectDisease[] Diseases;
			Diseases = diseasedBuildings.FirstOrDefault().GetComponents<RiskEffectDisease>();

			if (Diseases == null)
				return;
			
			//for every Disease Risk check the Chance of the Disease being Healed by Doctor
			foreach (RiskEffectDisease disease in Diseases) {
				//if the Chance is True stop the disease
				if (RiskManager.CheckRisk(disease.canHealbyDoctorChance))
				{
					disease.enabled  = false;
				}
				
			}

			//if the house is not diseased set the HealthCare satisfaction to maximum
			if (!diseasedBuildings.FirstOrDefault().GetComponent<BuildingHouse>().IsDiseased()){
				serviceReceiver = BuildingService.GetServiceReceiverbyService(diseasedBuildings.FirstOrDefault(),BuildingService.Services.HealthCare);
				serviceReceiver.satisfaction = serviceReceiver.maxSatisfaction;	
			}


		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}
}
