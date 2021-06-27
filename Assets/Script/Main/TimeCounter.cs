using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCounter  {

	//Used to Identify the Time that passed to count the week cycles
	private float currentTime;
	private float currentSeconds;
	private float tempCurrentSeconds;

	private Map map;
	public TimeCounter (Map map){
		this.map = map;
	}

	/// <summary>
	///Updates the Seconds for this Object in Order to Check the Week cycle
	/// </summary>
	public void UpdateSeconds ()
	{
		try {
			if (Time.time - currentTime >= 1) {
				currentSeconds += 1;
				currentTime = Time.time;
			}
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	public bool CheckCycle (int cycle)
	{
		if (tempCurrentSeconds == currentSeconds)
			return false;

		if (CheckWeekPassed (cycle, currentSeconds)) {
			tempCurrentSeconds = currentSeconds;
			return true;
		} else
			return false;

	}

	public bool CheckWeekPassed (int numberWeeks, float numberSecondsPassed)
	{
		try {
			if (numberSecondsPassed % (map.timeManager.weekLength * numberWeeks) == 0)
				return true;
			return false;
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
			return false;
		}
	}

	public bool CheckMonthPassed (int numberMonths, float numberSecondsPassed)
	{
		try {
			if ((numberSecondsPassed % map.timeManager.monthLength) % numberMonths == 0)
				return true;
			return false;
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
			return false;
		}
	}

	public bool CheckYearPassed (int numberYears, float numberSecondsPassed)
	{
		try {
			if ((numberSecondsPassed % map.timeManager.yearLength) % numberYears == 0)
				return true;
			return false;
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
			return false;
		}
	}
}
