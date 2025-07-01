using System;
using UnityEngine;

public class TestMissions : MonoBehaviour
{
    [SerializeField]
    private MissionSequence _chain1;
	[SerializeField]
	private MissionSequence _chain2;

	private MissionManager _manager;

	private void Start()
	{
		_manager = GetComponent<MissionManager>();
		_manager.StartParallelChains(_chain1, _chain2);
	}

}
