using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Date : MonoBehaviour
{
    string[] names = new string[] {"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"};

    [SerializeField] public int month;
    [SerializeField] public int day;
    [SerializeField] public int namesIndex = 2;

    [SerializeField] TextMeshProUGUI dateText;
    [SerializeField] TextMeshProUGUI weekdayText;

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
        if (month == 7 && day >= 32 && clock.counterTillDayChange <= clock.maxCounter)
        {
            month++;
            day = 1;
        }
        weekdayText.text = names[namesIndex % 7];
    }
}
