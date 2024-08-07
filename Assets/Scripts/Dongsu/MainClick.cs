using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainClick : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene("Main");
    }
}
