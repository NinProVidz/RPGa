using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagemnt
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        [SerializeField] float fadeOutDuration = 3f;
        [SerializeField] float fadeInDuration = 5f;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            StartCoroutine(FadeOutIn());
        }

        public IEnumerator FadeOutIn()
        {
            yield return FadeOut(fadeOutDuration);
            yield return FadeIn(fadeInDuration);
        }

        public IEnumerator FadeOut(float time)
        {
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime / time;
                //time/time.Delta time = number if frames
                //change in alpha = 1/number of frames
                //so substitiute: 1/time/time.delta = Time.deltatime/time
                yield return null;
            }
        }
        public IEnumerator FadeIn(float time)
        {
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime / time;
                //time/time.Delta time = number if frames
                //change in alpha = 1/number of frames
                //so substitiute: 1/time/time.delta = Time.deltatime/time
                yield return null;
            }
        }
    }
}//essss

