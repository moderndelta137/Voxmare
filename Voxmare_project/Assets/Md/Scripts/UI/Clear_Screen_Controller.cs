using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Clear_Screen_Controller : MonoBehaviour
{
    public AudioClip Wake_clip;
    public AudioClip Love_clip;
    private AudioSource SE_source;
    public Eyelid_Controller eye;
    // Start is called before the first frame update
    void Start()
    {
        SE_source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Openeye()
    {
        eye.EyeOpen();
    }

    public void Closeeye()
    {
        eye.EyeClose();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void PlayWakeSE()
    {
        SE_source.clip = Wake_clip;
        SE_source.Play();
    }

    public void PlayLoveSE()
    {
        SE_source.clip = Love_clip;
        SE_source.Play();
    }
}
