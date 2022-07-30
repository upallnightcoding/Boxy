using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour
{
    [SerializeField] private GameData gameData;

    [SerializeField] private Slider sizeSlider;
    [SerializeField] private TMP_Text size;

    [SerializeField] private Slider levelSlider;
    [SerializeField] private TMP_Text level;

    private string[] levels = { "EASY", "MEDIUM", "HARD" };

    private void Start()
    {
        sizeSlider.value = gameData.BoardSize;
        levelSlider.value = 0;

        sizeSlider.onValueChanged.AddListener((value) =>
        {
            size.text = value.ToString();
            gameData.BoardSize = (int)value;
        });

        levelSlider.onValueChanged.AddListener((value) =>
        {
            level.text = levels[(int)value];
            gameData.level = (GameLevel) value;
        });
    }
}


