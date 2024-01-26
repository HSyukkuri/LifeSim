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

    StateMachine<State> state;

    List<Action> list_Action = new List<Action>();
    Action targetAction;

    

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
        //ステートマシーン初期化
        state = new StateMachine<State>(transform);

        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        foreach (var item in GameManager.instance.list_ActionPlace) {
            Action newAction = new Action(item.data_Action,item.transform_Enter, item.transform);
            list_Action.Add(newAction);
        }

        state.ChangeState(State.MoveToDestination);

    }

    private void Update() {
        state.OnUpdate();

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
            if(state.currentState == State.Action) {
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





        switch (state.currentState) {
            //目的地へ移動ステート
            case State.MoveToDestination: {
                    if (state.stateEnter) {
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
                            state.ChangeState(State.Action_Enter);
                        }
                        //targetActionが無い（何もやることがない、リビングに到着した）
                        else {
                            //ダラダラするステートに移行
                            state.ChangeState(State.DoNothing);
                        }
                        return;
                    }
                    return;
                }

            case State.DoNothing: {
                    if (state.stateEnter) {
                        
                    }

                    if (list_Action[0].currentDesire >= 1) {
                        state.ChangeState(State.MoveToDestination);
                        return;
                    }
                    return;
                }

            //アクション開始
            case State.Action_Enter: {
                    if (state.stateEnter) {
                        //ナビメッシュを停止
                        navMeshAgent.enabled = false;
                        //transform_Enterがあるかないかで分岐する
                        //transform_Enterがある
                        if(targetAction.transform_Enter != null) {
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
                            state.ChangeState(State.Action);
                            return;
                        }
                    }

                    //フェーズ0 プレイヤーの位置とオブジェクトの位置を合わせる。
                    if(state.phase == 0) {
                        //0.25秒かけてちょっとずつ位置と向きをずらす。
                        transform.position = Vector3.Lerp(state.stateEnterPos, targetAction.transform_Enter.position, state.phaseTime / 0.25f);
                        transform.rotation = Quaternion.Lerp(state.stateEnterRor, targetAction.transform_Enter.rotation, state.phaseTime / 0.25f);

                        //0.25秒経過
                        if(state.phaseTime >= 0.25f) {
                            //位置をぴったり確定させる
                            transform.position = targetAction.transform_Enter.position;
                            transform.rotation = targetAction.transform_Enter.rotation;
                            //次のフェーズへ移行
                            state.ChangePhase(1);
                        }
                    }

                    //フェーズ１アニメーションが終わるまで待機
                    if(state.phase == 1) {
                        //3秒経過
                        if (state.currentStateTime >= 3.0f) {

                            //オブジェクト（机）のアニメーションステートを待機に戻す
                            Animator anim_Object = targetAction.transform.GetComponent<Animator>();
                            anim_Object.SetInteger("ID", 0);

                            //アクションステートに移行
                            state.ChangeState(State.Action);
                            return;
                        }
                    }
                    


                    return;
                }

            case State.Action: {
                    if (state.stateEnter) {
                        navMeshAgent.enabled = false;
                        animatorOverrideController["Anim_Base"] = targetAction.data.clip;
                        ChangeAnimState(Anim_State.Action);
                        transform.position = targetAction.transform.position;
                        transform.rotation = targetAction.transform.rotation;
                    }


                    if(targetAction.currentDesire <= 0f) {
                        navMeshAgent.enabled = true;
                        state.ChangeState(State.MoveToDestination);
                        return;
                    }

                    return;

                }


        }
    }

    private void LateUpdate() {
        


    }


}
