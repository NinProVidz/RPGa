using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TutorialTimelineManager : MonoBehaviour
{

    [SerializeField] PlayableDirector director;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pauseTilPlayerMove()
    {
        director.Pause();
        StartCoroutine(WaitForPlayerMove());
    }

    public void pauseTilPlayerTurn()
    {
        director.Pause();
        StartCoroutine(WaitForPlayerTurn());
    }

    IEnumerator WaitForPlayerMove()
    {
        // Wait until player moves
        yield return new WaitUntil(() => CheckIfPlayerMoved());
        director.Resume();
    }

    IEnumerator WaitForPlayerTurn()
    {
        // Wait until player turns
        yield return new WaitUntil(() => CheckIfPlayerTurned());
        director.Resume();
    }

    public bool CheckIfPlayerMoved()
    {
        return PlayerInputManager.instance.moveAmount != 0;
    }

    public bool CheckIfPlayerTurned()
    {
        return PlayerInputManager.instance.cameraHorizontalInput != 0 || PlayerInputManager.instance.cameraVerticalInput != 0;
    }

    public void PauseUntilPlayerEntersTrigger(Collider triggerZone)
    {
        director.Pause();

        StartCoroutine(WaitForPlayerTrigger(triggerZone));
    }

    private IEnumerator WaitForPlayerTrigger(Collider triggerZone)
    {
        bool playerEntered = false;

        // Attach temporary trigger watcher
        TriggerWatcher watcher = triggerZone.gameObject.AddComponent<TriggerWatcher>();
        watcher.Initialize(() => playerEntered = true);

        // Wait until playerEntered becomes true
        yield return new WaitUntil(() => playerEntered);

        // Clean up
        Destroy(watcher);
        director.Resume();
    }

    
}
