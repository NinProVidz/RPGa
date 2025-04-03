using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Date : MonoBehaviour
{

    [SerializeField] public int month;
    [SerializeField] public int day;

    [SerializeField] TextMeshProUGUI dateText;

    Clock clock;

    // Start is called before the first frame update
    void Start()
    {
        clock = FindObjectOfType<Clock>();
        month = 7;
        day = 2;
    }

    // Update is called once per frame
    void Update()
    {
        dateText.text = month.ToString() + "/" + day.ToString();
        if(month == 7 && day == 31 && clock.counterTillDayChange == clock.maxCounter)
        {
            month++;
            day = 0;
        }
    }
}
