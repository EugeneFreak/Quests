using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;


public class MissionManager : MonoBehaviour
{
    private readonly List<CancellationTokenSource> _cancellationTokenSources = new List<CancellationTokenSource>();

    public void StartParallelChains(params MissionSequence[] missions)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        _cancellationTokenSources.Add(cancellationTokenSource);

        var tasks = new List<UniTask>();
		foreach (var missonData in missions)
		{
            tasks.Add(RunSequentialChain(missonData.missions, cancellationTokenSource.Token));

		}
        UniTask.WhenAll(tasks).Forget();
	}

	public void StartSequence(MissionSequence sequence)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        _cancellationTokenSources.Add(cancellationTokenSource);
        RunSequentialChain(sequence.missions, cancellationTokenSource.Token).Forget();
    }

	private async UniTask RunSequentialChain(MissionData[] missions, CancellationToken cancellationToken)
	{
		foreach (var missonData in missions)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await new MissionRunner(missonData).Implement(cancellationToken);
		}
	}

	

    /*private async UniTask StartParallel(MissionData[] missions, CancellationToken cancellationToken)
    {
        var tasks = new List<UniTask>();
		foreach (var missonData in missions)
		{
           tasks.Add(new MissionRunner(missonData).Implement(cancellationToken));

		}
        await UniTask.WhenAll(tasks);
	}*/

    

    public void CancelAllTaskes()
    {
        foreach (var cts in _cancellationTokenSources)
        {
            cts.Cancel();
            cts.Dispose();
        }
        _cancellationTokenSources.Clear();
        Debug.Log($"Все мисси отменены");
    }

	private void OnDestroy()
	{
        CancelAllTaskes();
	}

}
