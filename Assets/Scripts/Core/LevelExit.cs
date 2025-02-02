using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.core
{
    public class LevelExit : MonoBehaviour
    {
        [SerializeField] GameObject pauseScreen;
        public void PauseMenu()
        {
            pauseScreen.SetActive(!pauseScreen.activeSelf);
            TimeFreeze();
        }

        private void TimeFreeze()
        {
            Time.timeScale = (Time.timeScale == 1f) ? 0f : 1f;

        }
    }
}

