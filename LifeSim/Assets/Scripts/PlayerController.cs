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
            //�ړI�n�ֈړ��X�e�[�g
            case State.MoveToDestination: {
                    if (stateEnter) {
                        //��ԗ~���̑傫���A�N�V�������擾
                        targetAction = list_Action[0];
                        //�������̃A�N�V�����̗~�����P�������Ă�����
                        if(targetAction.currentDesire >= 1f) {
                            //�G���^�[�A�j���[�V����������
                            if(targetAction.transform_Enter != null) {
                                //transform_Enter�܂ňړ�
                                navMeshAgent.SetDestination(targetAction.transform_Enter.position);
                            }
                            //�G���^�[���[�V�������Ȃ��i�g�C����x�b�h�Ȃǁj
                            else {
                                //�I�u�W�F�N�g�̌��_�ֈړ�
                                navMeshAgent.SetDestination(targetAction.transform.position);
                            }
                        }
                        //�������̃A�N�V�����̗~����1��������Ă�����
                        else {
                            //���r���O�Ɉړ�����
                            navMeshAgent.SetDestination(GameManager.instance.point_Living.position);
                            targetAction = null;
                        }

                        //�������[�V�����𔭓�
                        ChangeAnimState(Anim_State.Stand);
                    }

                    //�ړI�n�ɂ��ǂ蒅����
                    if (navMeshAgent.remainingDistance <= 0.01f && !navMeshAgent.pathPending) {
                        //targetAction������i�~��������j
                        if(targetAction != null) {
                            //�A�N�V�����G���^�[�X�e�[�g�Ɉڍs
                            ChangeState(State.Action_Enter);
                        }
                        //targetAction�������i������邱�Ƃ��Ȃ��A���r���O�ɓ��������j
                        else {
                            //�_���_������X�e�[�g�Ɉڍs
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
                        //�i�r���b�V�����~
                        navMeshAgent.enabled = false;
                        //transform_Enter�����邩�Ȃ����ŕ��򂷂�
                        //transform_Enter������
                        if(targetAction.transform_Enter != null) {
                            //�C�J�����̈ʒu�ƕ������G���^�[�|�W�V�����ɂ��낦��
                            transform.position = targetAction.transform_Enter.position;
                            transform.rotation = targetAction.transform_Enter.rotation;
                            //�G���^�[�A�j���[�V��������
                            animatorOverrideController["Anim_Base_Enter"] = targetAction.data.clip_Enter;
                            ChangeAnimState(Anim_State.ActionEnter);

                            //�I�u�W�F�N�g�i���j�̃A�j���[�^�[���擾
                            Animator anim_Object = targetAction.transform.GetComponent<Animator>();
                            //�I�u�W�F�N�g�̃G���^�[�A�j���[�V��������
                            anim_Object.SetInteger("ID", 1);
                        }
                        //transform_Enter������
                        else {
                            //���̂܂܃A�N�V�����X�e�[�g�փW�����v
                            ChangeState(State.Action);
                            return;
                        }
                    }
                    
                    //3�b�o��
                    if(stateTime >= 3.0f) {

                        //�I�u�W�F�N�g�i���j�̃A�j���[�V�����X�e�[�g��ҋ@�ɖ߂�
                        Animator anim_Object = targetAction.transform.GetComponent<Animator>();
                        anim_Object.SetInteger("ID", 0);

                        //�A�N�V�����X�e�[�g�Ɉڍs
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
