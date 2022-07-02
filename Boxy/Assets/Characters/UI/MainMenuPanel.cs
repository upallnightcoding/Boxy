using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField] private GameData gameData;

    // Start is called before the first frame update
    void Start()
    {
        gameData.Initialize();
    }

}
