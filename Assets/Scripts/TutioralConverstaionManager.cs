using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutioralConverstaionManager : MonoBehaviour
{

    public string[] Lines = {
    "Hello brave climber. Today your task will be to climb Mount Alverstone.", // auto 0 
    "Although you may feel scared, I must prepare you for your climb.", // auto 1
    "Walk towards the marked area on this hill and press E.", // auto 2
    "To climb, press the key or keys that are in the middle of the circle shown on screen.", //after they press e 3
    "Be careful though, as if you fail, you will fall to your dimese.", // auto 4 
    "Not bad, this is only the beginning. Whenever you feel ready, head on to the next Mountian.", // Activated when they finsh 5
    };
    public int index = 0;
    public string currentLine;
    public TMP_Text textBox;


    // Start is called before the first frame update
    void Start()
    {
        NextLine();
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void NextLine()
    {

        currentLine = Lines[index];
        textBox.text = currentLine;
        if (index != 2 && index !=4)
        {
            index++;
            Invoke("NextLine", 5f);
        }
    }


    public void SetToLine(int newIndex)
    {
        index = newIndex;
        currentLine = Lines[index];
        textBox.text = currentLine;
        NextLine();

    }
        
}
