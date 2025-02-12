using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        public bool triggered;

        private void Start()
        {
            GetComponent<PlayableDirector>().Stop();
            triggered = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player") && triggered == false)
            {
                GetComponent<PlayableDirector>().Play();
                triggered = true;
            }
        }
    }
}//eeeeee

