using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityCave;
using Cave;
using VrpnX;
using Cave.StereoRendering;

public class ScreenReference : Editor
{
    //跟据CaveCamera中的Screens的数量，生成对应数量的Quad
    [MenuItem("LinkX/ScreenReference")]
    public static void ScreenReferenceCreate()
    {
        List<CaveScreen> caveScreen=YamlConfig.ConfigFille.caveCameraSetting.ScreenSettings.screens;
        CaveSpaceZeroRoot caveSpaceZeroRoot = GameObject.FindObjectOfType<CaveSpaceZeroRoot>();
       
        if (caveScreen == null || caveScreen.Count == 0)
        {
            Debug.LogError("Screens is null");
            return;
        }
        GameObject parent =new GameObject("*****ScreenRoot****");
        parent.transform.parent = caveSpaceZeroRoot.transform;
        parent.transform.localPosition = Vector3.zero;
        parent.transform.localEulerAngles = Vector3.zero;
        parent.transform.localScale = Vector3.one;

        for (int i = 0; i < caveScreen.Count; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = "Screen" + i;
            go.transform.parent = parent.transform;
            go.transform.localPosition = caveScreen[i].local.position;
            go.transform.localEulerAngles = caveScreen[i].local.eulerAngles;
            RuntimeDisplayRect displayRect = caveScreen[i].display;
            float _height = caveScreen[i].local.scale.y;
            go.transform.localScale = new Vector3(_height * displayRect.GetAspect(), caveScreen[i].local.scale.y, 1);
            go.GetComponent<MeshRenderer>().material = AssetDatabase.LoadAssetAtPath<Material>("Assets/LinkXWorkspace/Materials/Screen.mat");
        }
    }
    //删除ScreenRoot
    [MenuItem("LinkX/ScreenReferenceDelete")]
    public static void ScreenReferenceDelete()
    {
        GameObject parent = GameObject.Find("*****ScreenRoot****");
        if (parent == null)
        {
            Debug.LogError("ScreenRoot is null");
            return;
        }
        DestroyImmediate(parent);
    }
}
