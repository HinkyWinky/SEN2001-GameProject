using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class GenerateMap : MonoBehaviour
    {
        [SerializeField] private Transform parentTransfrom = default;
        [SerializeField] private GameObject[] prefabs = new GameObject[4];
        [SerializeField, Range(0, 100)] private int xSize = 10;
        [SerializeField, Range(0, 100)] private int zSize = 10;
        private static List<GameObject> objects = new List<GameObject>();

        [Button]
        public void Generate()
        {
            for (int x = 0; x < xSize; x++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    Vector3 pos = new Vector3(x, 0, z);
                    Vector3 rot = new Vector3(0, Random.Range(0, 3) * 90, 0);
                    GameObject cell = Instantiate(prefabs[Random.Range(0, 4)], pos, Quaternion.Euler(rot));
                    cell.transform.SetParent(parentTransfrom, false);
                    objects.Add(cell);
                }
            }
        }

        [Button]
        public void Destroy()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                DestroyImmediate(objects[i]);
            }
        }
    }
}
