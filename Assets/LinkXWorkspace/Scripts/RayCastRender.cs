using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class RayCastRender : MonoBehaviour
{
    
    [SerializeField]
    private LineRenderer lineRender;
    Vector3 _pos;
    bool isMove = false;

    private void Start()
    {
        lineRender=transform.GetComponent<LineRenderer>();
    }
    //从Transform的位置发出一条射线，方向为Transform的前方，返回值为射线碰撞到的物体
    private void Update()
    {
        lineRender.SetPosition(0, transform.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            //if (Input.GetMouseButton(0))
            //{
            //    if (hit.collider != null)
            //    {
            //        if (hit.collider.gameObject.layer != 5)
            //        {
            //            if (!isMove)
            //            {
            //                isMove = true;
            //                _pos = hit.collider.gameObject.transform.position - transform.position;
            //            }
            //            hit.collider.gameObject.transform.position = transform.position + _pos;
            //        }
            //    }
            //}
            //else
            //{
            //    isMove = false;
            //}
            lineRender.SetPosition(1, hit.point);
        }
        else
        {
            lineRender.SetPosition(1, transform.TransformPoint(transform.localPosition + new Vector3(0, 0, 1000)));
        }
    }

}
