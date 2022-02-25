using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelFrames : MonoBehaviour
{

    public GameObject[] models;

    public int FPS = 60;

    // Start is called before the first frame update
    private void Awake()
    {

        foreach (var model in models)
        {
            model.SetActive(false);
        }
    }

    // Update is called once per frame
    private IEnumerator Start()
    {
        int currentIndex = 0;

        models[currentIndex].SetActive(true);

        while(true)
        {
            yield return new WaitForSeconds(1f / FPS);
            
            models[currentIndex].SetActive(false);

            currentIndex = (currentIndex + 1) % models.Length;

            models[currentIndex].SetActive(true);
        }
    }
}
