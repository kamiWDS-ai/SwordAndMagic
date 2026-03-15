using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private String SceneName;
    public void OnClick()
    {
        SceneManager.LoadScene(SceneName);
    }
}
