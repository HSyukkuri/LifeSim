using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {
    public Data_Action data { get; private set; }
    public float currentDesire { get; private set; }
    public Transform transform { get; private set; }

    public Action(Data_Action _data, Transform _transform) {
        data = _data;
        transform = _transform;
        currentDesire = 0f;
    }

    public void Update(Action targetAction) {
        if(targetAction != null) {
            if(targetAction.data == data) {
                currentDesire = Mathf.Clamp01(currentDesire);
                currentDesire -= Time.deltaTime/ data.desireDecreaseSpeed;
                return;
            }
        }
        currentDesire += Time.deltaTime/ data.desireSpeed;
    }

}


[CreateAssetMenu(fileName = "新しいアクション", menuName = "自作データ/アクション")]
public class Data_Action : ScriptableObject
{
    [field: SerializeField] public AnimationClip clip { get; private set; }
    [field: SerializeField] public Desire desire { get; private set; }
    [field: SerializeField] public float desireSpeed { get; private set; }
    [field: SerializeField] public float desireDecreaseSpeed { get; private set; }
}
