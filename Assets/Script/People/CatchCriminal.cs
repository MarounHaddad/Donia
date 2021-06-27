using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class CatchCriminal : MonoBehaviour {

	public List<GameObject> criminals;
	public GameObject returnToRoad;

	public RuntimeAnimatorController AttackDownAnimator;
	public RuntimeAnimatorController AttackLeftAnimator;

	private Map map;

	// Use this for initialization
	void Awake () {
		map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
		criminals = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		ChaseCriminal ();
		TryAttackCriminal ();
		SeekRoad ();
		ReturnToRoad ();
	}

	private void ChaseCriminal(){
		try {
			if (criminals ==null)
				return;
			if (criminals.Count ==0)
				return;
			if (criminals.FirstOrDefault() == null){
				criminals.Remove(criminals.FirstOrDefault());
				return;
			}

			GetComponent<FreeWalk> ().target = criminals.FirstOrDefault().transform;
		
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);	
		}
	}

	private void TryAttackCriminal(){
		try {
			
			Vector3 criminalDirection;
			GameObject criminal;

			if (criminals ==null)
				return;
			if (criminals.Count ==0)
				return;
			if (criminals.FirstOrDefault() == null){
				criminals.Remove(criminals.FirstOrDefault());
				return;
			}

			criminal = criminals.FirstOrDefault();

			float radius = Vector3.Distance (criminal.transform.position, transform.position);
			if (radius > 1)
				return;

			criminalDirection = (transform.position - criminal.transform.position).normalized;

			GetComponent<FreeWalk> ().enabled = false;
			GetComponent<NavMeshAgent> ().enabled = false;

			if (criminalDirection.x >= 0 && criminalDirection.z >= 0) {
				GetComponent<Animator> ().runtimeAnimatorController = AttackDownAnimator;
				GetComponent<SpriteRenderer> ().flipX = false;
			} else if (criminalDirection.x <= 0 && criminalDirection.z <= 0) {
				GetComponent<Animator> ().runtimeAnimatorController = AttackLeftAnimator;
				GetComponent<SpriteRenderer> ().flipX = true;
			} else if (criminalDirection.x <= 0 && criminalDirection.z >= 0) {
				GetComponent<Animator> ().runtimeAnimatorController = AttackDownAnimator;
				GetComponent<SpriteRenderer> ().flipX = true;
			} else if (criminalDirection.x >= 0 && criminalDirection.z <= 0) {
				GetComponent<Animator> ().runtimeAnimatorController = AttackLeftAnimator;
				GetComponent<SpriteRenderer> ().flipX = false;
			}

			criminal.GetComponent<Murderer>().Surrender();
			criminals.Remove(criminals.FirstOrDefault());

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);	
		}
	}

	private void SeekRoad ()
	{
		try {

			if (criminals != null && criminals.FirstOrDefault () == null)
				criminals.Remove (criminals.FirstOrDefault ());

			if (criminals != null && criminals.Count > 0)
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
			if (criminals != null && criminals.FirstOrDefault () == null)
				criminals.Remove (criminals.FirstOrDefault ());

			if (criminals != null && criminals.Count > 0)
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
			GetComponent<CatchCriminal> ().enabled = false;
			GetComponent<CitizenWalk> ().enabled = true;

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

}
