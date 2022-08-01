using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public GameObject playerPrefab;
    [HideInInspector]
    public GameObject playerObj;

    void Start()
    {
        playerObj = GameObject.Instantiate<GameObject>(playerPrefab);
        playerObj.name = playerPrefab.name;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (playerObj == null)
            {
                playerObj = GameObject.Instantiate<GameObject>(playerPrefab);
                playerObj.name = playerPrefab.name;
            }
            else
            {
                Destroy(playerObj);
            }
        }
    }
}
