using System.Collections;
using System.Collections.Generic;
using Game.UI;
using UnityEngine;

public class SceneDatabase : MonoBehaviour
{
    [SerializeField] private CanvasX canvas = default;
    public CanvasX Canvas => canvas;

    [SerializeField] private PlayerController playerController = default;
    public PlayerController PlayerController => playerController;
}
