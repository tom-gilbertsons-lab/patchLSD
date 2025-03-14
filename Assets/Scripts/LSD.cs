using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LSD : MonoBehaviour
{
    // Manager Objects with Associated Scripts
    public GameObject sessionManagerObject;
    private SessionManager sessionManager;

    public GameObject eegObject;
    private EegStream eegStream;

    // Actual Game Objects 
    public GameObject questionMark;
    public GameObject leaveButton;
    public GameObject stayButton;

    private Vector3 leftButtonPos;
    private Vector3 rightButtonPos;

    public Color originalColour;

    public float highlightDuration = 2f;

    private void Awake()
    {
        sessionManager = sessionManagerObject.GetComponent<SessionManager>();
        eegStream = eegObject.GetComponent<EegStream>();
        leftButtonPos = leaveButton.transform.localPosition; //grabbing positions 
        rightButtonPos = stayButton.transform.localPosition;
        originalColour = leaveButton.GetComponent<Button>().colors.normalColor;

       
     
        //make sure on start all objects in the LSD are inactive 
        DeactivateLSDObjects();
    }

    public void ChoicePhase()
    {
        StartCoroutine(Choice());
    }

    private IEnumerator Choice()
    {
        // Show question mark before displaying the buttons
        yield return StartCoroutine(QMark());
        bool isLeft = Random.value < 0.5f;

        ActivateButtons(true);

        eegStream.LogButtonsAppear();

        if (isLeft)
        {
            leaveButton.transform.localPosition = leftButtonPos;
            stayButton.transform.localPosition = rightButtonPos;
        }
        else
        {
            leaveButton.transform.localPosition = rightButtonPos;
            stayButton.transform.localPosition = leftButtonPos;
        }
        Interactable(true);
        AddListeners();
    }

    private void OnLeaveChoice()
    {
        StartCoroutine(HandleButtonClick(leaveButton, true));
    }

    private void OnStayChoice()
    {
        StartCoroutine(HandleButtonClick(stayButton, false));
    }

    private IEnumerator HandleButtonClick(GameObject button, bool isLeave)
    {
        // Make buttons non-interactable
        Interactable(false);
        button.GetComponent<Image>().color = Color.yellow;

        // Wait for the highlight duration
        yield return new WaitForSeconds(highlightDuration);
        // Revert the button color
        button.GetComponent<Image>().color = originalColour;

        sessionManager.MadeLSD(isLeave);
        DeactivateLSDObjects();
    }

    private IEnumerator QMark()
    {
        eegStream.LogChoicePhaseBegins();
        questionMark.SetActive(true);
        yield return new WaitForSeconds(2);
        questionMark.SetActive(false);
    }

    private void DeactivateLSDObjects()
    {
        questionMark.SetActive(false);
        ActivateButtons(false);
    }

    private void ActivateButtons(bool active)
    {
        leaveButton.SetActive(active);
        stayButton.SetActive(active);
    }

    private void Interactable(bool active)
    {
        leaveButton.GetComponent<Button>().interactable = active;
        stayButton.GetComponent<Button>().interactable = active;
       
    }

    private void AddListeners()
    {
        leaveButton.GetComponent<Button>().onClick.AddListener(OnLeaveChoice);
        stayButton.GetComponent<Button>().onClick.AddListener(OnStayChoice);
    }
}
