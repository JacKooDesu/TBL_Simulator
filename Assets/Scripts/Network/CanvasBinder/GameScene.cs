using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBL.NetCanvas;
public class GameScene : MonoBehaviour
{
    [SerializeField]
    GameObject playerMapping;
    [SerializeField]
    GameObject playerIconPrefab;
    [SerializeField]
    float radius;
    [SerializeField]
    int testPlayerCount = 5;

    protected void Start()
    {
        InitPlayerMapping();
    }

    void InitPlayerMapping()
    {
        for (int i = 0; i < testPlayerCount; ++i)
        {
            GameObject g = Instantiate(playerIconPrefab, playerMapping.transform);
            g.transform.localPosition = new Vector2(
                radius * Mathf.Sin((float)i / (float)testPlayerCount * 2 * Mathf.PI),
                radius * Mathf.Cos((float)i / (float)testPlayerCount * 2 * Mathf.PI));
        }
    }
}
