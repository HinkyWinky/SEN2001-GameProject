using System;
using System.Collections;
using System.Collections.Generic;
using Game.UI;
using UnityEngine;

public class SceneDatabase : MonoBehaviour
{
    [SerializeField] private CanvasX canvas = default;
    public CanvasX Canvas => canvas;

    [SerializeField] private CameraController cameraController = default;
    public CameraController CameraController => cameraController;

    [SerializeField] private Player player = default;
    public Player Player => player;

    [SerializeField] private Enemy enemy = default;
    public Enemy Enemy => enemy;

    private bool firstTime = false;

    private void Start()
    {
        GameManager.Cur.SetSceneDatabase();
        Debug.Log("LEVEL SCENE: Start()");
    }

    private void Update()
    {
        if (!firstTime)
        {
            Debug.Log("LEVEL SCENE: Update()");
            firstTime = true;
        }
    }
}
