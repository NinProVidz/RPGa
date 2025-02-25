using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagment
{
    public class Portal : MonoBehaviour
    {

        enum DestinationIdentifier
        {
            A, B, C, D
        }

        [SerializeField] int sceneToLoad = 1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutDuration = 1.5f;
        [SerializeField] float fadeInDuration = 3f;
        [SerializeField] float fadeWaitTime = 1f;

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
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutDuration);
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            Portal otherPortal = GetOtherPortal();
            UpdatePlayerPosition(otherPortal);

            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInDuration);


            Destroy(gameObject);
        }

        private Portal GetOtherPortal()
        {
            foreach(Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;//if same portal thats running the code, continue
                if (portal.destination != destination) continue;

                return portal;
            }
            
            return null;
        }

        private void UpdatePlayerPosition(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }

    }
}

