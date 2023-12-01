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
        Action_Enter,
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
        ActionEnter = 1,
        Action = 2,
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
            Action newAction = new Action(item.data_Action,item.transform_Enter, item.transform);
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
            //目的地へ移動ステート
            case State.MoveToDestination: {
                    if (stateEnter) {
                        //一番欲求の大きいアクションを取得
                        targetAction = list_Action[0];
                        //もしそのアクションの欲求が１を上回っていたら
                        if(targetAction.currentDesire >= 1f) {
                            //エンターアニメーションがある
                            if(targetAction.transform_Enter != null) {
                                //transform_Enterまで移動
                                navMeshAgent.SetDestination(targetAction.transform_Enter.position);
                            }
                            //エンターモーションがない（トイレやベッドなど）
                            else {
                                //オブジェクトの原点へ移動
                                navMeshAgent.SetDestination(targetAction.transform.position);
                            }
                        }
                        //もしそのアクションの欲求が1を下回っていたら
                        else {
                            //リビングに移動する
                            navMeshAgent.SetDestination(GameManager.instance.point_Living.position);
                            targetAction = null;
                        }

                        //歩くモーションを発動
                        ChangeAnimState(Anim_State.Stand);
                    }

                    //目的地にたどり着いた
                    if (navMeshAgent.remainingDistance <= 0.01f && !navMeshAgent.pathPending) {
                        //targetActionがある（欲求がある）
                        if(targetAction != null) {
                            //アクションエンターステートに移行
                            ChangeState(State.Action_Enter);
                        }
                        //targetActionが無い（何もやることがない、リビングに到着した）
                        else {
                            //ダラダラするステートに移行
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

            case State.Action_Enter: {
                    if (stateEnter) {
                        //ナビメッシュを停止
                        navMeshAgent.enabled = false;
                        //transform_Enterがあるかないかで分岐する
                        //transform_Enterがある
                        if(targetAction.transform_Enter != null) {
                            //イカちゃんの位置と方向をエンターポジションにそろえる
                            transform.position = targetAction.transform_Enter.position;
                            transform.rotation = targetAction.transform_Enter.rotation;
                            //エンターアニメーション発動
                            animatorOverrideController["Anim_Base_Enter"] = targetAction.data.clip_Enter;
                            ChangeAnimState(Anim_State.ActionEnter);

                            //オブジェクト（机）のアニメーターを取得
                            Animator anim_Object = targetAction.transform.GetComponent<Animator>();
                            //オブジェクトのエンターアニメーション発動
                            anim_Object.SetInteger("ID", 1);
                        }
                        //transform_Enterが無い
                        else {
                            //そのままアクションステートへジャンプ
                            ChangeState(State.Action);
                            return;
                        }
                    }
                    
                    //3秒経過
                    if(stateTime >= 3.0f) {

                        //オブジェクト（机）のアニメーションステートを待機に戻す
                        Animator anim_Object = targetAction.transform.GetComponent<Animator>();
                        anim_Object.SetInteger("ID", 0);

                        //アクションステートに移行
                        ChangeState(State.Action);
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
