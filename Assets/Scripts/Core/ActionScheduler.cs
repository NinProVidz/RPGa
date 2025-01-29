using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        IAction currentAction;

      public void StartAction(IAction action)
        {
            // if we are doing an action and we want to keep doing that action
            if(currentAction == action) return;
            // if we are doing an action and we want to do the other action
            if(currentAction != null)
            {
                currentAction.Cancel();
            }    
            // if we are currently not doing an action and we want to do action
            currentAction = action;
        }
    }
}

