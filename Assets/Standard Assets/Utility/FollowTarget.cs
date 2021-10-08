using System;
using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class FollowTarget : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0f, 7.5f, 0f);

        public bool isSmooth = false;

        private Camera cameraMain;

        private void Awake()
        {
            cameraMain = Camera.main;    
        }

        private void FixedUpdate()
        {
            if (isSmooth)
            {
                transform.forward = Vector3.Lerp(transform.forward, target.forward, 0.1f);
                transform.position = Vector3.Lerp(transform.position, target.position + offset, 0.1f);
            } else
            {
                transform.position = target.position + offset;
            }
        }

        public void End()
        {
            if (!enabled)
            {
                return;
            }
            transform.localPosition = Vector3.zero;
            cameraMain.transform.rotation = rot;
            this.enabled = false;
        }

        private Quaternion rot = Quaternion.identity;
        private void OnEnable()
        {
            print("asd");
            rot = cameraMain.transform.rotation;
        }
    }


}
