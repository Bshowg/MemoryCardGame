using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    TMP_InputField input;

    [SerializeField]
    Button button;
    // Start is called before the first frame update
    void Start()
    {
        button.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (string.IsNullOrEmpty(input.text))
        {
            button.interactable = false;

        }else
        {
            button.interactable = true;
        }
    }

    public void PlayClick()
    {

        PlayerPrefs.SetString("PlayerName", this.input.text);
        SceneManager.LoadScene(1);
    } 
}
