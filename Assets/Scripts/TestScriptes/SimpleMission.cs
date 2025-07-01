using System;
using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class SimpleMission : MonoBehaviour, IMission
{
	public event Action OnStarted;
	public event Action OnMissionPointReached;
	public event Action OnFinished;

	[SerializeField]
	private float duration = 3f;
	[SerializeField]
	private float checkPoint = 1f;
	
	void IMission.Start()
	{
		RealisationMissionAsync().Forget();
	}

	private async UniTaskVoid RealisationMissionAsync()
	{
		OnStarted?.Invoke();

		await UniTask.Delay(TimeSpan.FromSeconds(checkPoint));
		OnMissionPointReached?.Invoke();

		await UniTask.Delay(TimeSpan.FromSeconds(duration - checkPoint));
		OnFinished?.Invoke();
	}
}
