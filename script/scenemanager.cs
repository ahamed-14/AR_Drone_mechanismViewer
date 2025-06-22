
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.XR.CoreUtils;


public class SceneController : MonoBehaviour
{

    public void SwitchScenes(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

