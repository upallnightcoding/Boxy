using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour
{
    [SerializeField] private GameData gameData;
    [SerializeField] private TMP_Text size;
    [SerializeField] private Slider slider;

    private void Start()
    {
        slider.value = gameData.BoardSize;
    }

    public void OnSliderChanged(float value)
    {
        size.text = value.ToString();
        gameData.BoardSize = (int)value;
    }
}
