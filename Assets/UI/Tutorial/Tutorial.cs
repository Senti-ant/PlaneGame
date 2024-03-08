using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    //Serialised so you can change things like duration easily.
    //The content of the tutorial is kinda hardcoded, since I'm not about to set up a whole system
    //for determining dialogue triggers, etc., just for this one tutorial in a jam game.

    //MAIN PATH: 
    //0. Welcome, air traffic controller, to the onboarding procedure!
    //1. *Cue laugh track*
    //2. Your job is to react if a plane does anything strange or unplanned.
    //3. Look! A plane wants to take off now! Approve the flight plan.
    //4. Tip: You can press space to fast forward time.
    //5. Great! You get score for any plane that arrives safely.
    //6. Please approve the next plane.
    //7. What the heck is going on there...?
    //8. Oh no! he crashed!
    //9. HEEEEEEYYYYYY Send a rescue plane by left-clicking the plane.
    //10. Rescue planes help you recover some score when a plane crashes.
    //11. Approve the next plane. This one will surely be entirely normal.
    //12. OH HECK OH CRAP,   RIGHT CLICK HIM!!!!
    //13. You did it, you saved the building!
    //14. From now on, right click planes to send help (in case of an accident).
    //15. And left click planes to send the military (in case of a hijacking).
    //16. The challenge is to come up with a correct response as soon as possible.
    //17. Good luck!

    //ALTERNATE PATHS:
    //IF NOT APPROVE PLANE:
    //Great job idiot, you failed the tutorial. Do-over?
    //IF BUILDING HIT:
    //Looks like you weren't quite fast enough. Do-over?

    [Header("Text")]
    [SerializeField] Message[] MainPath;
    [SerializeField] Message DidntApprove;
    [SerializeField] Message BuildingHit;

    [Header("Planes")]
    [SerializeField] GameObject NormalPlane;
    [SerializeField] GameObject CrashingPlane;
    [SerializeField] GameObject HijackedPlane;

    [Header("References")]
    [SerializeField] Airport TakeOff;
    [SerializeField] Airport Landing;
    [SerializeField] GameObject BuildingPrefab;
    [SerializeField] Vector2 BuildingPosition;
    [SerializeField] GameObject DoOverButton;
    [SerializeField] GameObject NextLevelButton;

    TextSystem textSystem;

    //For keeping track of the tutorial planes.
    private enum PlaneDepartureState { Pending, Departed, Canceled };
    PlaneDepartureState latestPlaneState = PlaneDepartureState.Pending;
    Plane latestPlane;

    //For keeping track of progress in case of do-over.
    GameObject building;
    int bestProgress = 0;
    int currentProgress = 0;

    void Start()
    {
        textSystem = FindObjectOfType<TextSystem>();

        TakeOff.OnFlightDeparts.AddListener(OnFlightDeparts);
        TakeOff.OnFlightCancelled.AddListener(OnFlightCancelled);

        building = Instantiate(BuildingPrefab, BuildingPosition, Quaternion.identity);
        StartCoroutine(TutorialRoutine());
    }

    void OnFlightDeparts(Plane plane)
    {
        latestPlane = plane;
        latestPlaneState = PlaneDepartureState.Departed;
    }
    void OnFlightCancelled() => latestPlaneState = PlaneDepartureState.Canceled;

    void OfferDoOver()
    {
        StopAllCoroutines();
        DoOverButton.SetActive(true);
    }

    public void StartDoOver()
    {
        DoOverButton.SetActive(false);

        currentProgress = 0;
        latestPlane = null;
        latestPlaneState = PlaneDepartureState.Pending;

        if (building == null)
            building = Instantiate(BuildingPrefab, BuildingPosition, Quaternion.identity);


        StartCoroutine(TutorialRoutine());
    }

    void OfferNextLevel()
    {
        StopAllCoroutines();
        NextLevelButton.SetActive(true);
    }
    public void StartNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    bool Skippable()
    {
        if (bestProgress > currentProgress)
        {
            return true;
        }
        else
        {
            bestProgress = currentProgress;
            return false;
        }
    }

    IEnumerator TutorialRoutine()
    {
        //Opening messages.
        if (!Skippable())
            yield return SendMainMessages(0, 2);
        currentProgress++;

        //A flight appears.
        if (!Skippable())
        {
            yield return ApproveNextPlane(NormalPlane, 3);
            textSystem.RequestDisplay(MainPath[4]);
            yield return WaitFor(latestPlane.OnLand, latestPlane);
            yield return SendMainMessages(5, 5);
        }
        currentProgress++;

        //The plane has landed.
        if (!Skippable())
        {

            //The next plane is ready.
            yield return ApproveNextPlane(CrashingPlane, 6);

            //The plane is flying.
            yield return WaitFor(latestPlane.OnAberrate, latestPlane);
            textSystem.RequestDisplay(MainPath[7]);
        
            //Wait until it crashes and demonstrate the SnR interceptor.
            float t = Time.timeSinceLevelLoad;
            textSystem.RequestDisplay(MainPath[8]);
            textSystem.RequestDisplay(MainPath[9]);
            textSystem.RequestDisplay(MainPath[10]);
            yield return WaitFor(GetComponent<InterceptorManager>().OnRescue, latestPlane);
            yield return new WaitUntil(() => Time.timeSinceLevelLoad - t > MainPath[7].readTime + MainPath[8].readTime + MainPath[9].readTime + MainPath[10].readTime);
        }
        currentProgress++;
        bestProgress = 3;
        
        yield return ApproveNextPlane(HijackedPlane, 11);

        //Wait until the plane gets hijacked.
        yield return WaitFor(latestPlane.OnAberrate, latestPlane);
        textSystem.RequestDisplay(MainPath[12]);

        //Then until it gets either gets intercepted or hits the building.
        bool crashed = false;
        latestPlane.OnCrash.AddListener(() => crashed = true);
        yield return new WaitUntil(() => latestPlane == null);
        if (crashed)
        {
            textSystem.RequestDisplay(BuildingHit);
            OfferDoOver();
            yield break;
        }
        else
            yield return SendMainMessages(13, 17);

        OfferNextLevel();
    }

    IEnumerator SendMainMessages(int start, int end)
    {
        for (int i = start; i <= end; i++)
        {
            textSystem.RequestDisplay(MainPath[i]);
            yield return new WaitForSeconds(MainPath[i].readTime);
        }
    }

    IEnumerator WaitFor(UnityEvent forWhat, MonoBehaviour emergencyStop)
    {
        bool eventHappened = false;
        UnityAction subscribe = () => eventHappened = true;

        forWhat.AddListener(subscribe);
        yield return new WaitUntil(() => eventHappened || emergencyStop == null);

        if (!eventHappened)
        {
            textSystem.RequestDisplay(DidntApprove);
            OfferDoOver();
            yield break;
        }
        else
            forWhat.RemoveListener(subscribe);
    }
    IEnumerator WaitFor<T>(UnityEvent<T> forWhat, MonoBehaviour emergencyStop)
    {
        bool eventHappened = false;
        UnityAction<T> subscribe = t => eventHappened = true;

        forWhat.AddListener(subscribe);
        yield return new WaitUntil(() => eventHappened || emergencyStop == null);

        if (!eventHappened)
        {
            textSystem.RequestDisplay(DidntApprove);
            OfferDoOver();
            yield break;
        }
        else
            forWhat.RemoveListener(subscribe);
    }

    IEnumerator ApproveNextPlane(GameObject nextPlane, int mainMessage)
    {
        latestPlaneState = PlaneDepartureState.Pending;
        TakeOff.EnqueueFlight(new FlightPlan(Time.timeSinceLevelLoad, TakeOff, Landing), nextPlane);
        textSystem.RequestDisplay(MainPath[mainMessage]);

        yield return new WaitUntil(() => latestPlaneState != PlaneDepartureState.Pending);

        //Branch: You cancel the flight.
        if (latestPlaneState == PlaneDepartureState.Canceled)
        {
            textSystem.RequestDisplay(DidntApprove);
            OfferDoOver();
            yield break;
        }
    }
}
