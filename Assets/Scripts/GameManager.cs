using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] images;

   
    [SerializeField] private Button[] buttons;

    [SerializeField] public bool isImages = false;
    [SerializeField] public bool isbuttons = false;
    Character character;
    private void Awake()
    {
        character = FindObjectOfType<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isImages = true;
        }

        if (!isImages)
        {
            for (int i = 0; i < 3; i++)
            {
                images[i].SetActive(false);
                if (character.weaponsituation == i)
                {
                    images[i].SetActive(images[i]);
                }
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                images[i].SetActive(true);
                isbuttons = true;
            }
        }

        if (isbuttons)
        {
            for (int i = 0; i < 3; i++)
            {
                buttons[i].onClick.AddListener(MyClickFunction);
                images[i].SetActive(images[i]);
            }
        }
    }
    public void MyClickFunction()
    {
        character.weaponsituation = 1;
        isbuttons = false;
    }

}
