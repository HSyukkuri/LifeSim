using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.UI;
using System.Linq;


public enum Desire {
    Toilet,
    Eat,
    Sleep,
}


public class PlayerController : MonoBehaviour
{

    public Text text;

    enum State {
        MoveToDestination,
        Action,
        DoNothing,
    }

    State currentState = State.MoveToDestination;
    bool stateEnter = false;
    float stateTime = 0f;

    List<Action> list_Action = new List<Action>();
    Action targetAction;




    void ChangeState(State newState) {
        currentState = newState;
        stateEnter = true;
        stateTime = 0f;
        Debug.Log(currentState.ToString());
    }

    enum Anim_State {
        Stand = 0,
        Action = 1,
    }

    void ChangeAnimState(Anim_State state) {
        animator.SetInteger("ID", (int)state);
    }

    NavMeshAgent navMeshAgent;
    Animator animator;
    AnimatorOverrideController animatorOverrideController;

    private void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        foreach (var item in GameManager.instance.list_ActionPlace) {
            Action newAction = new Action(item.data_Action, item.transform);
            list_Action.Add(newAction);
        }

        ChangeState(State.MoveToDestination);

    }

    private void Update() {
        stateTime += Time.deltaTime;

        if (targetAction != null && targetAction.currentDesire > 2f) {
            if (targetAction.currentDesire > 3f) {
                navMeshAgent.speed = 3.5f;
            }
            else {
                navMeshAgent.speed = Mathf.Lerp(1.8f, 3.5f, targetAction.currentDesire - 2f);
            }

        }
        else {
            navMeshAgent.speed = 1.8f;
        }

        float speed = navMeshAgent.velocity.magnitude;

        animator.SetFloat("PlayerSpeed", speed);

        foreach (var item in list_Action) {
            if(currentState == State.Action) {
                item.Update(targetAction);
            }
            else {
                item.Update(null);
            }

        }

        list_Action.Sort((action1, action2) => action2.currentDesire.CompareTo(action1.currentDesire));

        text.text = "";

        foreach(var sortedDesireElement in list_Action) {
            text.text += sortedDesireElement.data.desire.ToString() + ":" + sortedDesireElement.currentDesire + "\n";
        }





        switch (currentState) {

            case State.MoveToDestination: {
                    if (stateEnter) {

                        targetAction = list_Action[0];

                        if(targetAction.currentDesire >= 1f) {

                            navMeshAgent.SetDestination(targetAction.transform.position);
                            
                        }
                        else {
                            navMeshAgent.SetDestination(GameManager.instance.point_Living.position);
                            targetAction = null;
                        }

                        ChangeAnimState(Anim_State.Stand);
                    }

                    //–Ú“I’n‚É‚½‚Ç‚è’…‚¢‚½
                    if (navMeshAgent.remainingDistance <= 0.01f && !navMeshAgent.pathPending) {

                        if(targetAction != null) {
                            ChangeState(State.Action);
                        }
                        else {
                            ChangeState(State.DoNothing);
                        }
                        return;
                    }
                    return;
                }

            case State.DoNothing: {
                    if (stateEnter) {
                        
                    }

                    if (list_Action[0].currentDesire >= 1) {
                        ChangeState(State.MoveToDestination);
                        return;
                    }
                    return;
                }

            case State.Action: {
                    if (stateEnter) {
                        navMeshAgent.enabled = false;
                        animatorOverrideController["Anim_Base"] = targetAction.data.clip;
                        ChangeAnimState(Anim_State.Action);
                        transform.position = targetAction.transform.position;
                        transform.rotation = targetAction.transform.rotation;
                    }


                    if(targetAction.currentDesire <= 0f) {
                        navMeshAgent.enabled = true;
                        ChangeState(State.MoveToDestination);
                        return;
                    }

                    return;

                }


        }
    }

    private void LateUpdate() {
        
        if(stateTime != 0) {
            stateEnter = false;
        }

    }


}
