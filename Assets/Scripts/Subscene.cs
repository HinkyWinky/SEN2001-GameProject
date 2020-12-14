using System;
using Game.UI;
using UnityEngine;

public class Subscene : MonoBehaviour
{
    [SerializeField] private CanvasX canvas = default;
    public CanvasX Canvas => canvas;

    [SerializeField] private CamCtrl camCtrl = default;
    public CamCtrl CamCtrl => camCtrl;

    [SerializeField] private Player player = default;
    public Player Player => player;

    [SerializeField] private Enemy1 enemy = default;
    public Enemy1 Enemy => enemy;

    private bool firstTime = false;

    private void Start()
    {
        Debug.Log("LEVEL SCENE: Start()");
        GameManager.Cur.SetSubscene();
    }

    private void Update()
    {
        if (!firstTime)
        {
            firstTime = true;
            Debug.Log("LEVEL SCENE: Update()");
        }
    }
}
