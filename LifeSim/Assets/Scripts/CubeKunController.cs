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
        //移動ステートから始まる
        state.ChangeState(State.Move);
    }


    void Update()
    {

        state.OnUpdate();

        switch (state.currentState) {

            //移動ステート
            case State.Move: {
                    if (state.stateEnter) {
                        Debug.Log("移動開始");
                    }

                    //前進
                    transform.position += transform.forward * Time.deltaTime;

                    //3秒経過
                    if(state.currentStateTime >= 3f) {
                        state.ChangeState(State.Rotate);
                    }

                    return;
                }

            //回転ステート
            case State.Rotate: {
                    if (state.stateEnter) {
                        Debug.Log("回転開始");
                    }

                    //待機フェーズ
                    if(state.phase == 0) {
                        //一秒経過
                        if(state.phaseTime >= 1f) {
                            //回転フェーズに移行
                            state.ChangePhase(1);
                        }
                    }

                    //回転フェーズ
                    if(state.phase == 1) {
                        //回転
                        transform.Rotate(0f, 90f * Time.deltaTime, 0f);
                        //1秒経過（90度回転した）
                        if(state.phaseTime >= 1f) {
                            //回転後の待機フェーズに移行
                            state.ChangePhase(2);
                        }
                    }

                    //回転後の待機フェーズ
                    if(state.phase == 2) {
                        //一秒待機
                        if(state.phaseTime >= 1f) {
                            //移動ステートに移行
                            state.ChangeState(State.Move);
                            return;
                        }
                    }

                    return;
                }


        }
    }
}
