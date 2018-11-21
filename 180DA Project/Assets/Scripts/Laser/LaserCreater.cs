using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCreater : MonoBehaviour {
	public AudioClip laserCountdown;
	public AudioClip laserFire;
	public Transform laserPrefab;
	// public Transform particlePrefab;
	private List<int> laneNums;
	private bool isFiring;
	private int minLasers = 6;
	private int maxLasers = 9;

	void Start () {
		InitializeLaneList();
		isFiring = false;
	}
	
	void Update () {
		if (!isFiring) StartCoroutine(ShootLasers());
	}

	private IEnumerator ShootLasers() 
	{
		isFiring = true;
		List<int> lanesToFireLasers = GetRandomLanes();
		int list_count = lanesToFireLasers.Count;
		GameState.PlayClip(laserCountdown);
		// for(int i = 0; i < list_count; i++)
		// {
		// 	Instantiate(particlePrefab, new Vector3(16f, (lanesToFireLasers[i] + 0.5f)), Quaternion.identity);
		// }
		yield return new WaitForSeconds(laserCountdown.length);
		GameState.PlayClip(laserFire);
		for(int i = 0; i < list_count; i++)
		{
			Instantiate(laserPrefab, new Vector3(16.5f, (lanesToFireLasers[i] + 0.5f)), Quaternion.identity);
		}
		yield return new WaitForSeconds(1f);
		isFiring = false;
	}

	private List<int> GetRandomLanes() 
	{
		List<int> laneNumsCopy = new List<int>(laneNums);
		List<int> lanesToFireLasers = new List<int>();
		int numLasersToFire = Random.Range(minLasers, maxLasers);
		for (int i = 0; i < numLasersToFire; i++)
		{
			int index = Random.Range(0, laneNumsCopy.Count);
			lanesToFireLasers.Add(laneNumsCopy[index]);
			laneNumsCopy.RemoveAt(index);
		}
		return lanesToFireLasers;
	}
	private void InitializeLaneList() {
		laneNums = new List<int>();
		for (int i = 1; i <= GameState.numLanes; i++)
			laneNums.Add(i);
	}
}
