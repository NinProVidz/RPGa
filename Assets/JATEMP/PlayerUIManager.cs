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

    [Header("UI")]
    [SerializeField] private GameObject gameOverlay;
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject pausedMenu;

    [Header("Binding")]
    [SerializeField] private KeyCode GMKey = KeyCode.B;
    [SerializeField] private KeyCode PMKey = KeyCode.M;

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
        OpenGO();
    }

    private void Update()
    {
        if (Input.GetKeyDown(GMKey))
        {
            if(gameMenu.activeSelf == false)
            {
                OpenGM();
            }
            else
            {
                OpenGO();
            }
        }
        else if(Input.GetKeyDown(PMKey))
        {
            if (pausedMenu.activeSelf == false)
            {
                OpenPM();
            }
            else
            {
                OpenGO();
            }
        }
    }

    #endregion

    public void OpenGO()
    {
        Cursor.lockState = CursorLockMode.Locked;
        PlayerInputManager.instance.lomotionEnabled = true;
        PlayerInputManager.instance.cameraEnabled = true;
        Time.timeScale = 1;
        ShowOnly(0);
    }
    public void OpenGM()
    {
        Cursor.lockState = CursorLockMode.None;
        PlayerInputManager.instance.lomotionEnabled = false;
        PlayerInputManager.instance.cameraEnabled = false;
        Time.timeScale = 1;
        ShowOnly(1);
    }

    public void OpenPM()
    {
        Cursor.lockState = CursorLockMode.None;
        PlayerInputManager.instance.lomotionEnabled = false;
        PlayerInputManager.instance.cameraEnabled = false;
        Time.timeScale = 0;
        ShowOnly(2);
    }

    public void ShowOnly(int m)
    {
        CloseAll();

        switch (m)
        {
            case 0:
                gameOverlay.SetActive(true);
                break;
            case 1:
                gameMenu.SetActive(true);
                break;
            case 2:
                pausedMenu.SetActive(true);
                break;
        }
    }

    public void CloseAll()
    {
        gameOverlay.SetActive(false);
        gameMenu.SetActive(false);
        pausedMenu.SetActive(false);
    }

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
