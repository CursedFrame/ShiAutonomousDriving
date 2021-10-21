using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoScreen : MonoBehaviour
{
    public float messageDelayTimer, messageDelay = 5f; // time between messages
    public float messageIntervalTimer, messageInterval = 5f; // time that message indicator stays on screen
    public float messageFlashTimer, messageFlash = 0.5f; // time between message flashing
    public int numMessages = 1;
    public bool stopMessageFlag = false;
    private bool screenDisplaying = false;
    public GameObject backgroundObject, messageObject; // background infotainment screen to change color TEST
    private Image background, message;
    public Color backgroundColor;

    // Start is called before the first frame update
    void Start()
    {
        messageDelayTimer = messageDelay;
        messageFlashTimer = messageFlash;
        --numMessages; // First message countdown begins, must decrement
        background = backgroundObject.GetComponent<Image>();
        message = messageObject.GetComponent<Image>();
        backgroundColor = background.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (screenDisplaying){
            messageIntervalTimer -= Time.deltaTime;
            messageFlashTimer -= Time.deltaTime;
            
            if (messageFlashTimer <= 0){
                message.enabled = !message.enabled;
                messageFlashTimer = messageFlash;
            }

            if (messageIntervalTimer <= 0){
                screenDisplaying = false;
                background.color = backgroundColor;
                message.enabled = false;
            }
        }

        if (!stopMessageFlag){
            messageDelayTimer -= Time.deltaTime;

            if (messageDelayTimer <= 0){
                if (numMessages == 0){
                    --numMessages;
                    stopMessageFlag = true;
                }

                // Trigger pop up message on info screen
                screenDisplaying = true;
                background.color = Color.white;
                messageIntervalTimer = messageInterval;
            }
        }
    }
}
