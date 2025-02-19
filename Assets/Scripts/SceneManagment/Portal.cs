using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagment
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] int sceneToLoad = 1;
        [SerializeField] Transform spawnPoint;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("amongus");
            if(other.tag == "Player")
            {
                Debug.Log("grrrr");
                StartCoroutine(Transition());
            }
        }
        
        private IEnumerator Transition()
        {
            DontDestroyOnLoad(gameObject);
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            Portal otherPortal = GetOtherPortal();
            UpdatePlayerPosition(otherPortal);
            Destroy(gameObject);
        }

        private Portal GetOtherPortal()
        {
            foreach(Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;//if same portal thats running the code, continue

                return portal;
            }

            return null;
        }

        private void UpdatePlayerPosition(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.transform.position = otherPortal.spawnPoint.position;
        }

    }
}

