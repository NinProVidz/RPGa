using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagment
{
    public class EggTeleportation : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnTriggerEnter(Collider other)
        {
            if(CompareTag("Hub") && other.tag == "Player")
            {
                SceneManager.LoadScene("Hubworld");
            }
            if (CompareTag("Bunker") && other.tag == "Player")
            {
                SceneManager.LoadScene("NotEgg");
            }
            if (CompareTag("Egg") && other.tag == "Player")
            {
                SceneManager.LoadScene("Bunker");
            }
            if (CompareTag("Reload") && other.tag == "Player")
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}

