using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] images;
    [SerializeField] RectTransform imagesW;
    [SerializeField] bool isImages = false;
    Character character;
    private void Awake()
    {
        character = FindObjectOfType<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isImages)
        {
            for (int i = 0; i < 3; i++)
            {
                images[i].SetActive(false);
                if (character.weaponsituation == i)
                {
                    images[i].SetActive(images[i]);
                    images[i].transform.position = imagesW.position;
                }
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                images[i].SetActive(true);

            }
        }
    }
}
