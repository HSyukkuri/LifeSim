using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshUpdater : MonoBehaviour
{
    public SkinnedMeshRenderer targetMesh;//髪の毛のメッシュ

    [ContextMenu("メッシュをかぶせる")]
    public void ConvertMesh() {
        SkinnedMeshRenderer bodyMesh = GetComponent<SkinnedMeshRenderer>();//身体のメッシュ

        SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(targetMesh);
        newMesh.transform.SetParent(transform);
        newMesh.bones = bodyMesh.bones;
        newMesh.rootBone = bodyMesh.rootBone;
    }
}
