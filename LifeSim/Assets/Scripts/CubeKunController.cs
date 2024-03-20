using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeKunController : MonoBehaviour
{

    public enum State {
        Move,
        Rotate,
    }

    StateMachine<State> state = new StateMachine<State>();




    void Start()
    {
        //�ړ��X�e�[�g����n�܂�
        state.ChangeState(State.Move);
    }


    void Update()
    {

        state.OnUpdate();

        switch (state.currentState) {

            //�ړ��X�e�[�g
            case State.Move: {
                    if (state.stateEnter) {
                        Debug.Log("�ړ��J�n");
                    }

                    //�O�i
                    transform.position += transform.forward * Time.deltaTime;

                    //3�b�o��
                    if(state.currentStateTime >= 3f) {
                        state.ChangeState(State.Rotate);
                    }

                    return;
                }

            //��]�X�e�[�g
            case State.Rotate: {
                    if (state.stateEnter) {
                        Debug.Log("��]�J�n");
                    }

                    //�ҋ@�t�F�[�Y
                    if(state.phase == 0) {
                        //��b�o��
                        if(state.phaseTime >= 1f) {
                            //��]�t�F�[�Y�Ɉڍs
                            state.ChangePhase(1);
                        }
                    }

                    //��]�t�F�[�Y
                    if(state.phase == 1) {
                        //��]
                        transform.Rotate(0f, 90f * Time.deltaTime, 0f);
                        //1�b�o�߁i90�x��]�����j
                        if(state.phaseTime >= 1f) {
                            //��]��̑ҋ@�t�F�[�Y�Ɉڍs
                            state.ChangePhase(2);
                        }
                    }

                    //��]��̑ҋ@�t�F�[�Y
                    if(state.phase == 2) {
                        //��b�ҋ@
                        if(state.phaseTime >= 1f) {
                            //�ړ��X�e�[�g�Ɉڍs
                            state.ChangeState(State.Move);
                            return;
                        }
                    }

                    return;
                }


        }
    }
}
