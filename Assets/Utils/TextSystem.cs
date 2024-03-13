using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class TextSystem : MonoBehaviour
{
    [SerializeField]
    Image TextBar;
    [SerializeField]
    TMP_Text Text;
    [SerializeField]
    RandomSFX TypingNoises;

    Queue<Message> messages; 

    void Awake() {messages = new Queue<Message>();}
    void Start() {StartCoroutine(DisplayRoutine());}

    public void RequestDisplay(Message message)
    {
        messages.Enqueue(message);
    }
    public void RequestDisplay(string content, bool typeout, float readtime, bool clearOnNextMessage = false)
    {
        messages.Enqueue(new Message(content, typeout, readtime, clearOnNextMessage));
    }
    public void ClearDisplay()
    {
        StopAllCoroutines();
        messages.Clear();
        Text.text = "";
        StartCoroutine(DisplayRoutine());
    }

    IEnumerator DisplayRoutine()
    {
        while (true)
        {
            while (messages.Count == 0)
            {
                if (TextBar.color.a > 0f)
                {
                    TextBar.color = new Color(1, 1, 1, TextBar.color.a - 0.04f);
                    Text.alpha -= 0.04f;
                    yield return new WaitForSeconds(0.05f);
                }
                else
                    yield return new WaitUntil(() => messages.Count > 0);
            }

            TextBar.color = new Color(1, 1, 1, 1);
            Text.alpha = 1f;

            Message toDisplay = messages.Dequeue();
            Text.text = "";
            float startTime = Time.timeSinceLevelLoad;
            
            if (toDisplay.typeOut)
            {
                if (TypingNoises != null) 
                    TypingNoises.Play();
                for (int i = 0; i < toDisplay.content.Length; i++)
                {
                    Text.text += toDisplay.content[i];
                    yield return new WaitForSecondsRealtime(0.02f);
                }
            }
            else
                Text.text = toDisplay.content;

            yield return new WaitUntil
            (
                () => Time.timeSinceLevelLoad - startTime >= toDisplay.readTime
                      || (toDisplay.clearOnNextMessage && messages.Any())
            );
        }
    }

}

[System.Serializable]
public struct Message
{
    public string content;
    public bool typeOut;
    public float readTime;
    public bool clearOnNextMessage;

    public Message(string _content, bool _typeout, float _readtime, bool _clearOnNextMessage)
    {
        content = _content;
        typeOut = _typeout;
        readTime = _readtime;
        clearOnNextMessage = _clearOnNextMessage;
    }
}