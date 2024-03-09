using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelIntroduction : MonoBehaviour
{
    [SerializeField] Message[] messages;

    void Start()
    {
        TextSystem textSystem = FindObjectOfType<TextSystem>();
        foreach (Message m in messages)
            textSystem.RequestDisplay(m);
    }
}
