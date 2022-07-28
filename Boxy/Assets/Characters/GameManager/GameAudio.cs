using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class GameAudio : MonoBehaviour
{
    [SerializeField] private AudioSource selectedAnchorPeg;
    [SerializeField] private AudioSource selectedPinPeg;
    [SerializeField] private AudioSource createBox;

    public void PlaySelectedAnchorPeg()
    {
        selectedAnchorPeg.Play();
    }

    public void PlaySelectedPinPeg()
    {
        selectedPinPeg.Play();
    }

    public void PlayCreateBox()
    {
        createBox.Play();
    }
}
