using System;
using ToyBoxHHH;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CustomRenderQueue : MonoBehaviour
{
    public UnityEngine.Rendering.CompareFunction comparison = UnityEngine.Rendering.CompareFunction.Always;

    public bool apply = false;

    private void OnEnable()
    {
        Change();
    }

    [DebugButton]
    private void Change()
    {
        Graphic image = GetComponent<Graphic>();
        Material existingGlobalMat = image.materialForRendering;
        Material updatedMaterial = new Material(existingGlobalMat);
        updatedMaterial.SetInt("unity_GUIZTestMode", (int) comparison);
        image.material = updatedMaterial;
    }
}