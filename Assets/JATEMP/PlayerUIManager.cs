using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UISoundType
{
    Highlight,
    Click,
    Confirm,
    Cancel
}

public class PlayerUIManager : MonoBehaviour
{
    #region Fields

    public static PlayerUIManager instance;

    [SerializeField] private GameObject gameOverlay;
    [SerializeField] private GameObject gameMenu;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip highlightSFX;
    [SerializeField] private AudioClip clickSFX;

    #endregion

    #region Unity Events

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        
    }

    #endregion



    #region Sound

    public void PlaySound(UISoundType type)
    {
        AudioClip clip = null;

        switch (type)
        {
            case UISoundType.Highlight:
                clip = highlightSFX;
                break;
            case UISoundType.Click:
                clip = clickSFX;
                break;
        }

        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlaySound(string type)
    {
        if (System.Enum.TryParse(type, out UISoundType parsedType))
        {
            PlaySound(parsedType);
        }
        else
        {
            Debug.LogWarning($"Invalid sound type: {type}");
        }
    }

    public void Sigma(string sagma)
    {
    }

    #endregion
}
