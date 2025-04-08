using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ClockDisplay : MonoBehaviour
{
    [SerializeField] private Clock clock;

    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        clock.ClockChange += OnClockChange;
    }

    private void OnDestroy()
    {
        clock.ClockChange -= OnClockChange;
    }

    private void OnClockChange(object sender, TimeSpan e)
    {
        text.SetText(e.ToString(@"hh\:mm"));
    }
}
