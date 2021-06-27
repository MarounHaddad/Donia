using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FreeWalk : MonoBehaviour
{
	public enum Directions
	{
		direction0 = 0,
		direction45 = 1,
		direction90 = 2,
		direction135 = 3,
		direction180 = 4,
		direction225 = 5,
		direction270 = 6,
		direction315 = 7
	}

	public Transform target;


	public enum Modes
	{
		walk = 0,
		run = 1
	}

	public RuntimeAnimatorController walkAnimator0;
	public RuntimeAnimatorController walkAnimator45;
	public RuntimeAnimatorController walkAnimator135;
	public RuntimeAnimatorController walkAnimator225;
	public RuntimeAnimatorController walkAnimator270;

	public RuntimeAnimatorController runAnimator0;
	public RuntimeAnimatorController runAnimator45;
	public RuntimeAnimatorController runAnimator135;
	public RuntimeAnimatorController runAnimator225;
	public RuntimeAnimatorController unAnimator270;

	private RuntimeAnimatorController currentAnimator0;
	private RuntimeAnimatorController currentAnimator45;
	private RuntimeAnimatorController currentAnimator135;
	private RuntimeAnimatorController currentAnimator225;
	private RuntimeAnimatorController currentAnimator270;

	public Modes mode = Modes.walk;

	public float walkSpeed = 2;
	public float RunSpeed = 5;

	public Directions direction;

	public bool moveOnClick = true;

	public NavMeshAgent agent;

	private Vector3 currentPosition;

	Vector3 targetOldPosition = Vector3.zero;

	BuildingManager.Directions targetOldDirection;
	Vector3 targetDestination;

	// check every one second
	public float checkEvery = 1f;
	float time;


	private Map map;

	BuildingManager.Directions oldTargetDirection;

	// Use this for initialization
	void Start ()
	{
		map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
		agent = GetComponent<NavMeshAgent> ();

		agent.updateRotation = false;
		currentPosition = transform.position;
		SetMode (mode);

	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		SeekTarget ();
		SetDirection ();
		SetRotation ();
		SetSortingOrder ();
		SetTargetOnClick ();
	}

	public void SetMode (Modes newMode)
	{
		try {
			mode = newMode;
			switch (mode) {
			case Modes.walk:
				currentAnimator0 = walkAnimator0;
				currentAnimator45 = walkAnimator45;
				currentAnimator135 = walkAnimator135;
				currentAnimator225 = walkAnimator225;
				currentAnimator270 = walkAnimator270;
				agent.speed = walkSpeed;
				break;
			default:
				currentAnimator0 = runAnimator0;
				currentAnimator45 = runAnimator45;
				currentAnimator135 = runAnimator135;
				currentAnimator225 = runAnimator225;
				currentAnimator270 = unAnimator270;
				agent.speed = RunSpeed;
				break;
			}
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);	
		}
	}

	private void SeekTarget ()
	{
		try {
			if (target == null)
				return;
		
			if (target.GetComponent<CitizenWalk> () != null)
				FollowCitizen ();
			else
				FollowStatic ();
				
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);	
		}
	}

	private void FollowStatic ()
	{
		try {
			if (time == 0 && targetOldPosition == Vector3.zero) {
				agent.destination = target.transform.position;
				targetOldPosition = target.transform.position;
			}

			time += Time.deltaTime;

			if (time >= checkEvery) {
				agent.SetDestination (target.position);
				time = 0;
			}
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);	
		}
	}

	private void FollowCitizen ()
	{
		try {
			if (targetDestination == Vector3.zero){
				setTargetDestination();
			}
			else{
				if (oldTargetDirection != target.GetComponent<CitizenWalk> ().generalDirection ){
					setTargetDestination ();
					oldTargetDirection= target.GetComponent<CitizenWalk> ().generalDirection; 
				}else{
					if (Vector3.Distance (target.position,this.transform.position) < 3){
						agent.enabled = false;
						transform.position =  Vector3.MoveTowards(transform.position, target.position,  RunSpeed * Time.deltaTime);
					}else{
						agent.enabled = true;
						if (target.GetComponent<CitizenWalk> ().generalDirection == BuildingManager.Directions.Down)
						if (target.transform.position.x <= targetDestination.x)
							setTargetDestination ();
						if (target.GetComponent<CitizenWalk> ().generalDirection == BuildingManager.Directions.Up)
						if (target.transform.position.x >= targetDestination.x)
							setTargetDestination ();
						if (target.GetComponent<CitizenWalk> ().generalDirection == BuildingManager.Directions.Left)
						if (target.transform.position.z >= targetDestination.z)
							setTargetDestination ();
						if (target.GetComponent<CitizenWalk> ().generalDirection == BuildingManager.Directions.Right)
						if (target.transform.position.z <= targetDestination.z)
							setTargetDestination ();
					}

					
				}

			}
		
				
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);	
		}
	}

	private void setTargetDestination ()
	{
		try {
			switch (target.GetComponent<CitizenWalk> ().generalDirection) {
			case	BuildingManager.Directions.Down:
				targetDestination = new Vector3 (target.transform.position.x - 2, 0, target.transform.position.z-2);
				agent.destination = targetDestination;
				break;
			case	BuildingManager.Directions.Up:
				targetDestination = new Vector3 (target.transform.position.x + 2, 0, target.transform.position.z+2);
				agent.destination = targetDestination;
				break;
			case	BuildingManager.Directions.Left:
				targetDestination = new Vector3 (target.transform.position.x+2, 0, target.transform.position.z + 2);
				agent.destination = targetDestination;
				break;
			case	BuildingManager.Directions.Right:
				targetDestination = new Vector3 (target.transform.position.x-2, 0, target.transform.position.z - 2);
				agent.destination = targetDestination;
				break;
			default:
				
				break;
			}
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);	
		}
	}

	private void SetSortingOrder ()
	{
		try {
			GetComponent<SpriteRenderer> ().sortingOrder = -(int)(transform.position.x + transform.position.z + 1);
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);	
		}
	}

	private void SetDirection ()
	{
		try {
	
			if (agent.nextPosition.x < currentPosition.x &&
			    (agent.nextPosition.z >= currentPosition.z - 0.01 && agent.nextPosition.z <= currentPosition.z + 0.01))
				direction = Directions.direction0;
			else if (agent.nextPosition.x < currentPosition.x - 0.01 &&
			         agent.nextPosition.z < currentPosition.z - 0.01)
				direction = Directions.direction45;
			else if ((agent.nextPosition.x >= currentPosition.x - 0.01 && agent.nextPosition.x <= currentPosition.x + 0.01) &&
			         agent.nextPosition.z < currentPosition.z - 0.01)
				direction = Directions.direction90;
			else if (agent.nextPosition.x > currentPosition.x + 0.01 &&
			         agent.nextPosition.z < currentPosition.z - 0.01)
				direction = Directions.direction135;
			else if (agent.nextPosition.x > currentPosition.x + 0.01 &&
			         (agent.nextPosition.z >= currentPosition.z - 0.01 && agent.nextPosition.z <= currentPosition.z + 0.01))
				direction = Directions.direction180;
			else if (agent.nextPosition.x > currentPosition.x + 0.01 &&
			         agent.nextPosition.z > currentPosition.z + 0.01)
				direction = Directions.direction225;
			else if ((agent.nextPosition.x >= currentPosition.x - 0.01 && agent.nextPosition.x <= currentPosition.x + 0.01) &&
			         agent.nextPosition.z > currentPosition.z + 0.01)
				direction = Directions.direction270;
			else if (agent.nextPosition.x < currentPosition.x - 0.01 &&
			         agent.nextPosition.z > currentPosition.z + 0.01)
				direction = Directions.direction315;

			currentPosition = agent.nextPosition;
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);	
		}
	}

	private void SetRotation ()
	{
		try {
			switch (direction) {
			case Directions.direction0:
				GetComponent<SpriteRenderer> ().flipX = false;
				GetComponent<Animator> ().runtimeAnimatorController = currentAnimator0;
				break;
			case Directions.direction45:
				GetComponent<SpriteRenderer> ().flipX = false;
				GetComponent<Animator> ().runtimeAnimatorController = currentAnimator45;
				break;
			case Directions.direction90:
				GetComponent<SpriteRenderer> ().flipX = true;
				GetComponent<Animator> ().runtimeAnimatorController = currentAnimator0;
				break;
			case Directions.direction135:
				GetComponent<SpriteRenderer> ().flipX = false;
				GetComponent<Animator> ().runtimeAnimatorController = currentAnimator135;
				break;
			case Directions.direction180:
				GetComponent<SpriteRenderer> ().flipX = true;
				GetComponent<Animator> ().runtimeAnimatorController = currentAnimator270;
				break;
			case Directions.direction225:
				GetComponent<SpriteRenderer> ().flipX = false;
				GetComponent<Animator> ().runtimeAnimatorController = currentAnimator225;
				break;
			case Directions.direction270:
				GetComponent<SpriteRenderer> ().flipX = false;
				GetComponent<Animator> ().runtimeAnimatorController = currentAnimator270;
				break;
			case Directions.direction315:
				GetComponent<SpriteRenderer> ().flipX = true;
				GetComponent<Animator> ().runtimeAnimatorController = currentAnimator135;
				break;
			default:
				break;
			}
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);	
		}
	}

	void SetTargetOnClick ()
	{
		try {
			if (!moveOnClick)
				return;
			if (Input.GetMouseButtonDown (0)) {
				agent.SetDestination (map.terrainGrid.GetClickPosition ());
			}
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);	
		}
	}

}
