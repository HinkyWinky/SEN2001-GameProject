using Sirenix.OdinInspector;
using UnityEngine;

public class StateCtrl : MonoBehaviour
{
    [ShowInInspector, ReadOnly] public GameState CurGameState { get; private set; }

    public bool GameRunning { get; private set; }

    public void ChangeGameState(GameState state)
    {
        CurGameState = state;
        Debug.Log("GAME STATE: " + CurGameState);
    }

    public bool CompareGameState(GameState state)
    {
        return CurGameState == state;
    }

}
