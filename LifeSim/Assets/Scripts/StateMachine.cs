using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T : System.Enum
{
    public T currentState { get; private set; }
    public float currentStateTime { get; private set; } = 0;
    int stateFlame = 0;
    public bool stateEnter { get {
            stateFlame++;
            return stateFlame == 1 ? true : false;
            }  
    }
    Transform transform;
    public Vector3 stateEnterPos { get; private set; }
    public Quaternion stateEnterRor { get; private set; }

    public StateMachine(Transform _transform = null) {
        transform = _transform;
    }



    public int phase { get; private set; } = 0;
    public float phaseTime { get; private set; } = 0f;

    public void ChangeState(T newState) {
        stateFlame = 0;
        currentState = newState;
        currentStateTime = 0;

        ChangePhase(0);

        if(transform != null) {
            stateEnterPos = transform.position;
            stateEnterRor = transform.rotation;
        }
    }

    public void ChangePhase(int newPhase) {
        phase = newPhase;
        phaseTime = 0f;
    }

    public void OnUpdate() {
        currentStateTime += Time.deltaTime;
        phaseTime += Time.deltaTime;
    }
}
