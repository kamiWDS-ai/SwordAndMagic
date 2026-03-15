using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetButton : MonoBehaviour
{
    [SerializeField] private GameObject SetUI;

    private void Start()
    {
        if(SetUI!=null)
            SetUI.SetActive(false);
    }
    public void OnClick()
    {
        if(SetUI!=null)
            SetUI.SetActive(SetUI.activeInHierarchy?false:true);
    }
}
