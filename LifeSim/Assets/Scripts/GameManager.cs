using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[System.Serializable]
public class ActionPlace {
    public Transform transform_Enter;
    public Transform transform;
    public Data_Action data_Action;
}


public class GameManager : MonoBehaviour
{

    #region ƒVƒ“ƒOƒ‹ƒgƒ“
    public static GameManager instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            Destroy(this);
        }
    }
    #endregion

    public Transform point_Living;

    public List<ActionPlace> list_ActionPlace = new List<ActionPlace>();

    UI_Manager UI;

    public CinemachineVirtualCamera cinemachineVC;
    CinemachinePOV cinemachinePOV;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        UI = FindObjectOfType<UI_Manager>();

        cinemachinePOV = cinemachineVC.GetCinemachineComponent(CinemachineCore.Stage.Aim).GetComponent<CinemachinePOV>();

    }


    void Update()
    {
        Time.timeScale = UI.slider_TimeScale.value ;

        if (Input.GetMouseButton(2)) {
            cinemachinePOV.m_VerticalAxis.m_MaxSpeed = 1.5f;
            cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = 1.5f;
        }
        else {
            cinemachinePOV.m_VerticalAxis.m_MaxSpeed =0f;
            cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = 0f;
        }

    }
}
