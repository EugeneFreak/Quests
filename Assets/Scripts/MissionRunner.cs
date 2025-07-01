using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class MissionRunner 
{
    private readonly MissionData _data;
    private readonly IMission _mission;
    private readonly Timer _timer = new();

    public MissionRunner(MissionData data)
    {
        _data = data;
        if (data.missionPrefab != null)
        {
            _mission = data.missionPrefab.GetComponent<IMission>();
        }

        Debug.Log($"������ ��������� ����������");
    }

    public async UniTask Implement(CancellationToken cancellationToken)
    {
        try { 
        Debug.Log($" @��������@ ������ {_data.missionName}");

            if (_mission == null)
            {
                Debug.LogError($"������ {_data.missionName} �� ��������� IMission ");
                return;
            }
        
        if (_data.startDelay > 0)
        {
            Debug.Log($"�������� ����� ������� {_data.missionName}");

            var timerCompleted = false;
            _timer.StartAsync((int)(_data.startDelay * 1000),
                () => timerCompleted = true);

            while (!timerCompleted && !cancellationToken.IsCancellationRequested)
            {
                await UniTask.Yield();
            }
            if (cancellationToken.IsCancellationRequested) 
            {
                Debug.LogError($"������ {_data.missionName} ��������");
                return;
            };
        }

        Debug.Log($" ������ ������ {_data.missionName}");
		bool missionCompleted = false;
        Action startHandler = () => Debug.Log($" ������ �������� {_data.missionName}");
		Action pointHandler = () => Debug.Log($" ������ ������������� ����� {_data.missionName}");
        Action finishHandler = () =>
        {
            missionCompleted = true;
            Debug.Log($" ������ ����������� {_data.missionName}");
        };

            _mission.OnStarted += startHandler;
            _mission.OnMissionPointReached += pointHandler;
            _mission.OnFinished += finishHandler;

			_mission.Start();

        while (!missionCompleted && !cancellationToken.IsCancellationRequested)
        {
            await UniTask.Yield();
        }
			_mission.OnStarted -= startHandler;
			_mission.OnMissionPointReached -= pointHandler;
			_mission.OnFinished -= finishHandler;

		if (cancellationToken.IsCancellationRequested)
        {
            Debug.Log($" ������ �������� {_data.missionName}");
        }
        else
			Debug.Log($" ������ ����������� {_data.missionName}");
		}
        catch (Exception ex)
        {
            Debug.LogWarning($"������ ������ ��-�� ��������� ������� �� OnFinished {_data.missionName} : {ex.Message}");
        }
	}
}
