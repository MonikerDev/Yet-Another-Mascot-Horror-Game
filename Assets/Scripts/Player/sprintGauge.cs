using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sprintGauge : MonoBehaviour
{
    public GameObject player;
	public Slider gauge;
	public PlayerMovement pm;

	private void Start()
	{
		pm = player.GetComponent<PlayerMovement>();
		gauge.maxValue = pm.maxSprintEnergy;
	}

	// Update is called once per frame
	void Update()
    {
		gauge.value = pm.currSprintEnergy;
    }
}
