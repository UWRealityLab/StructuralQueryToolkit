using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour 
    {
        private static int camSelect;
        public int CamSelect
        {
            get
            {
                return camSelect;
            }
            set
            {
                camSelect = value;
                // Debug.Log("value =" + camSelect);
            }
        }
    }
