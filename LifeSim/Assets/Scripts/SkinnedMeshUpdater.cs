using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshUpdater : MonoBehaviour
{
    public SkinnedMeshRenderer targetMesh;//���̖т̃��b�V��

    [ContextMenu("���b�V�������Ԃ���")]
    public void ConvertMesh() {
        SkinnedMeshRenderer bodyMesh = GetComponent<SkinnedMeshRenderer>();//�g�̂̃��b�V��

        SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(targetMesh);
        newMesh.transform.SetParent(transform);
        newMesh.bones = bodyMesh.bones;
        newMesh.rootBone = bodyMesh.rootBone;
    }
}
