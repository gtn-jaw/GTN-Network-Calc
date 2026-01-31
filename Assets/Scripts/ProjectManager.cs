using System;
using System.Threading.Tasks;
using UnityEngine;

public class ProjectManager
{
    public static EState appState = EState.START;

    public enum EState
    {
        START,
        BEFORESTART,
        AFTERSTART,
        RUNNING,
        EXITING,
    }

    public static event Action<EState> OnAppStateChanged;

    public static void UpdateAppState(EState newState)
    {
        appState = newState;

        switch (newState)
        {
            case EState.BEFORESTART:
                Debug.Log("App State: BEFORESTART");
                break;
            case EState.AFTERSTART:
                Debug.Log("App State: AFTERSTART");
                break;
            case EState.RUNNING:
                Debug.Log("App State: RUNNING");
                break;
            case EState.EXITING:
                Debug.Log("App State: EXITING");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnAppStateChanged?.Invoke(newState);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    void BeforeSceneLoad()
    {
        UpdateAppState(EState.BEFORESTART);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    async Task AfterSceneLoad()
    {
        await WaitUntil(() => appState == EState.BEFORESTART);
        UpdateAppState(EState.AFTERSTART);
    }

    public static async Task WaitUntil(Func<bool> predicate, int sleep = 50)
    {
        while (!predicate())
        {
            await Task.Delay(sleep);
        }
    }
}
