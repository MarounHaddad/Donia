using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Murderer : MonoBehaviour {

	//Map Object used to access the Main Assets
	private Map map;

	//Maxfind Victim Radius 
	public float maxFindVictimRadius = 100;

	//the Victim to be attacked by the murderer
	public GameObject Victim;

	public RuntimeAnimatorController AttackDownAnimator;
	public RuntimeAnimatorController AttackLeftAnimator;
	public RuntimeAnimatorController SurrenderAnimator;

	private bool runningBackHome = false;

	public float PolicemanAlertArea = 50;

	public  bool Surrendered = false;

	// Use this for initialization
	void Start () {
		map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
		ChooseVictim ();
		AlertPolicemen ();
	}
	
	// Update is called once per frame
	void Update () {
		if (this == null)
			return;

		if (Surrendered)
			return;
		
		//try to attack the victim (if a victim is chosen)
		TryAttackVictim ();

		//Check if it reached its hom
		//if it already killed its victim and getting back home
		CheckReachedHome ();
	}

	private void ChooseVictim(){
		try {
			//The Radius between the murderer and the victim
			float radius;

			//list of potential Targets
			GameObject[] targets = GameObject.FindGameObjectsWithTag(map.CitizenTag);

			if (targets.Length ==0){
				Destroy(gameObject);
				Destroy(this);
				return;
			}
			
			foreach (GameObject target in targets){
				radius = Vector3.Distance (target.transform.position, transform.position);
				if (radius > maxFindVictimRadius)
					continue;
				
				Victim = target;
				break;

			}

			if (Victim == null){
				Destroy(gameObject);
				Destroy(this);
				return;
			}

			GetComponent<FreeWalk> ().target = Victim.transform;
			Victim .GetComponent <CitizenWalk>().canDestroy= false;
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	private void TryAttackVictim(){
		try {
			if (this == null)
				return;
			
			Vector3 VictimDirection;

			if (Victim == null)
				return;
			
			float radius = Vector3.Distance (Victim.transform.position, transform.position);
			if (radius > 1)
				return;
			
			VictimDirection = (transform.position - Victim.transform.position).normalized;

			GetComponent<FreeWalk> ().enabled = false;
			GetComponent<NavMeshAgent> ().enabled = false;

			if (VictimDirection.x >= 0 && VictimDirection.z >= 0) {
				GetComponent<Animator> ().runtimeAnimatorController = AttackDownAnimator;
				GetComponent<SpriteRenderer> ().flipX = false;
			} else if (VictimDirection.x <= 0 && VictimDirection.z <= 0) {
				GetComponent<Animator> ().runtimeAnimatorController = AttackLeftAnimator;
				GetComponent<SpriteRenderer> ().flipX = true;
			} else if (VictimDirection.x <= 0 && VictimDirection.z >= 0) {
				GetComponent<Animator> ().runtimeAnimatorController = AttackDownAnimator;
				GetComponent<SpriteRenderer> ().flipX = true;
			} else if (VictimDirection.x >= 0 && VictimDirection.z <= 0) {
				GetComponent<Animator> ().runtimeAnimatorController = AttackLeftAnimator;
				GetComponent<SpriteRenderer> ().flipX = false;
			}

			Victim.GetComponent<WorldObject>().parentBuilding.GetComponent<BuildingHouse>().OccupiantDied();

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	private void RunBackHome(){
		try {
			runningBackHome = true;
			GetComponent<FreeWalk> ().enabled = true;
			GetComponent<NavMeshAgent> ().enabled = true;
			GetComponent<FreeWalk> ().target = GetComponent<WorldObject>().parentBuilding.transform;

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	private void CheckReachedHome(){
		try {
			if (!runningBackHome)
				return;
			
			float radius = Vector3.Distance (GetComponent<WorldObject>().parentBuilding.transform.position, transform.position);

			if (radius > 2)
				return;

			GetComponent<WorldObject>().parentBuilding.GetComponent <RiskEffectCrime>().enabled = false;
			Destroy(gameObject);
			Destroy(this);

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}


	private void AlertPolicemen(){
		try {
			float radius;

			GameObject[] policemen = GameObject.FindGameObjectsWithTag(map.PolicemanTag);

			//if no policeman found return
			if(policemen == null)
				return;

			//if no policeman found return
			if(policemen.Length==0)
				return;

			//loop through each policeman
			foreach (GameObject Policeman in policemen) {
				//get the radius between the policeman and the criminal
				radius = Vector3.Distance (Policeman.transform.position, transform.position);
				//if radius larger than max radius skip this policeman
				if (radius > PolicemanAlertArea)
					continue;
				if (Policeman.GetComponent<CatchCriminal>().criminals.Contains(gameObject))
					continue;
				
				Policeman.GetComponent<CitizenWalk>().enabled =false;
				Policeman.GetComponent<NavMeshAgent>().enabled = true;
				Policeman.GetComponent<FreeWalk>().enabled = true;
				Policeman.GetComponent<CatchCriminal>().enabled = true;
				Policeman.GetComponent<CatchCriminal>().criminals.Add(gameObject);

			}

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	public void Surrender(){
		try {
			
			Surrendered = true;
			GetComponent<Animator> ().runtimeAnimatorController = SurrenderAnimator;
			GetComponent<FreeWalk> ().enabled = false;
			GetComponent<NavMeshAgent> ().enabled = false;

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

}
