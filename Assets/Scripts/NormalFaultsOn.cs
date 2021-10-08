using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalFaultsOn : MonoBehaviour {

    public GameObject normalFault1;
    public GameObject normalFault2;

    // Use this for initialization
    void Start () {

        normalFault1.SetActive(false);
        normalFault2.SetActive(false);
		
	}

    // Update is called once per frame
    public void NormalFaultsAppear()

    {
        normalFault1.SetActive(true);
        normalFault2.SetActive(true);
    }

		
	}

