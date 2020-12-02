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

    [SerializeField] private Enemy enemy = default;
    public Enemy Enemy => enemy;

    private bool firstTime = false;

    private void Start()
    {
        GameManager.Cur.SetSubscene();
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
