using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash_Screen_Controller : MonoBehaviour
{
    public float Wait_duration;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadMainMenu());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(Wait_duration);
        SceneManager.LoadScene(1);
    }


}
