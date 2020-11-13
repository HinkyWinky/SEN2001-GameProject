using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    public GameState CurGameState { get; private set; }

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
