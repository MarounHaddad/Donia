using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{

	public int currentYear = -500;
	public int currentMonth = 1;
	public int currentWeek = 1;

	public int yearLength = 0;
	public int monthLength = 0;
	public int weekLength = 120;

	public Text timeLabel;
	private TimeCounter timeCounter;

	private Map map;

	void Start ()
	{
		map = GameObject.FindGameObjectWithTag ("Map").GetComponent<Map> ();
		timeCounter = new TimeCounter (map);

		InitializeTime ();
	}

	// Update is called once per frame
	void Update ()
	{
		timeCounter.UpdateSeconds ();
		UpdateClock();
	}

	void InitializeTime ()
	{
		try {
			if (monthLength == 0)
				monthLength = weekLength * 4;
			if (yearLength == 0)
				yearLength = monthLength * 12;


			DisplayClock();
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}
		


	private void UpdateClock ()
	{
		try {
			if (timeCounter.CheckCycle(1)) {
				if (currentWeek == 4) {
					currentWeek = 0;
					if (currentMonth == 12) {
						currentMonth = 0;
						currentYear += 1;
					}
				
					currentMonth += 1;
				}

				currentWeek += 1;

				DisplayClock();
			}

		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}

	private void DisplayClock(){
		try {
			if (!timeLabel.Equals (null))
				timeLabel.text = 
					" Week:" + currentWeek + " " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName (currentMonth) + "/" + currentYear;
		} catch (System.Exception ex) {
			Debug.Log (ex.Message);
		}
	}
}
