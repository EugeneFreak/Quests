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

        Debug.Log($"Создан экземпляр обработчик");
    }

    public async UniTask Implement(CancellationToken cancellationToken)
    {
        try { 
        Debug.Log($" @Загрузка@ миссии {_data.missionName}");

            if (_mission == null)
            {
                Debug.LogError($"Миссия {_data.missionName} не реализует IMission ");
                return;
            }
        
        if (_data.startDelay > 0)
        {
            Debug.Log($"Задержка перед миссией {_data.missionName}");

            var timerCompleted = false;
            _timer.StartAsync((int)(_data.startDelay * 1000),
                () => timerCompleted = true);

            while (!timerCompleted && !cancellationToken.IsCancellationRequested)
            {
                await UniTask.Yield();
            }
            if (cancellationToken.IsCancellationRequested) 
            {
                Debug.LogError($"Миссия {_data.missionName} отменена");
                return;
            };
        }

        Debug.Log($" ЗАпуск миссии {_data.missionName}");
		bool missionCompleted = false;
        Action startHandler = () => Debug.Log($" Миссия началась {_data.missionName}");
		Action pointHandler = () => Debug.Log($" Миссия промежуточная точка {_data.missionName}");
        Action finishHandler = () =>
        {
            missionCompleted = true;
            Debug.Log($" Миссия Закончилась {_data.missionName}");
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
            Debug.Log($" Миссия отменена {_data.missionName}");
        }
        else
			Debug.Log($" Миссия закончилась {_data.missionName}");
		}
        catch (Exception ex)
        {
            Debug.LogWarning($"Утечка памяти из-за отсуствия отписки от OnFinished {_data.missionName} : {ex.Message}");
        }
	}
}
