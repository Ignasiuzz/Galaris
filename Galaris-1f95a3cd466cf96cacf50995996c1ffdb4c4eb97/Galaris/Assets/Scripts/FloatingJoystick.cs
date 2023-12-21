using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public enum LJoyStickDirection {LHorizontal, LVertical, Both}
public enum RJoyStickDirection {RHorizontal, RVertical, Both}

public class FloatingJoystick : MonoBehaviour
{
    public LJoyStickDirection LJoyStickDirection = LJoyStickDirection.Both;     //JoyStickDirection declaration
    public RJoyStickDirection RJoyStickDirection = RJoyStickDirection.Both;

    public RectTransform LTfield;                               //Kairiojo joystick sprites
    public RectTransform LJoyBackground;
    public RectTransform LBackground;
    public RectTransform LHandle;
    [Range(0, 2f)] public float LHandleLimit = 1f;              //Galima pakeisti kaip toli is joysticko iseina tas mazas ratukas

    public RectTransform RTfield;                               //Desiniojo joystick sprites
    public RectTransform RJoyBackground;
    public RectTransform RBackground;
    public RectTransform RHandle;
    [Range(0, 2f)] public float RHandleLimit = 1f;              //Galima pakeisti kaip toli is joysticko iseina tas mazas ratukas

    public Vector2 Linput = Vector2.zero;                       //Kairys imput
    public float LVertical {get{return Linput.y;}}              //Kairiojo joystick vertikalus input y
    public float LHorizontal {get{return Linput.x;}}            //Kairiojo joystick horizontalus input x
    Vector2 LJoyPosition = Vector2.zero;                        //Pradine pozicija

    public Vector2 Rinput = Vector2.zero;                       //Kairys imput
    public float RVertical {get{return Rinput.y;}}              //Kairiojo joystick vertikalus input y
    public float RHorizontal {get{return Rinput.x;}}            //Kairiojo joystick horizontalus input x
    Vector2 RJoyPosition = Vector2.zero;                        //Pradine pozicija

    private Vector2 LstartingPoint;                             //Pradine pozicija
    private Vector2 RstartingPoint;
    private int leftTouch = 99;
    private int rightTouch = 99;

    //public static FloatingJoystick INSTANCE;

    //void Start() {
       // if (INSTANCE == null)
       // {
        //    INSTANCE = this;
      //      DontDestroyOnLoad(gameObject);
      //  }

   // }

    public void Update()
    {
        int i=0;
        while (i < Input.touchCount)                            //Programa veikia, jei yra bent vienas touch inputas
        {
            Touch touch = Input.GetTouch(i);
            Vector2 touchPos = touch.position;

            if (RectTransformUtility.RectangleContainsScreenPoint(LTfield, touchPos))       //Kairio joystick scriptas
            {   
                Touch Ltouch = Input.GetTouch(i);
                Vector2 LtouchPos = Ltouch.position;

                if (Ltouch.phase == TouchPhase.Began && RectTransformUtility.RectangleContainsScreenPoint(LJoyBackground, LtouchPos))       //Scripto pradzia
                {
                   // Debug.Log("Touch Began" + Ltouch.position);     //Touch pradzios debugger
                                                                    //Touch pradzioje keiciasi veriables
                    leftTouch = Ltouch.fingerId;
                    LstartingPoint = LtouchPos;
                    LBackground.gameObject.SetActive(true);         //Joystickas atsiranda
                    LBackground.position = LstartingPoint;
                    LJoyPosition = LstartingPoint;
                }
                else if (Ltouch.phase == TouchPhase.Ended)
                {
                   // Debug.Log("Touch End" + Ltouch.position);       //Touch pabaigos debugger

                    leftTouch = 99;
                    Linput = Vector2.zero;
                    LHandle.anchoredPosition = Vector2.zero;
                    LBackground.anchoredPosition = Vector2.zero;
                    LBackground.gameObject.SetActive(false);        //Joystickas paslepiamas
                }
                else if (Ltouch.phase == TouchPhase.Moved && leftTouch == Ltouch.fingerId)      //Joysticko pirsto sekimo scriptas
                {
                   // Debug.Log("Touch Moved" + Ltouch.position);

                    Vector2 LJoyDriection = LtouchPos - LJoyPosition;
                    Linput = (LJoyDriection.magnitude > LBackground.sizeDelta.x / 2f) ? LJoyDriection.normalized :
                        LJoyDriection / (LBackground.sizeDelta.x / 2f);
                    if (LJoyStickDirection == LJoyStickDirection.LHorizontal)
                        Linput = new Vector2(Linput.x, 0f);
                    if (LJoyStickDirection == LJoyStickDirection.LVertical)
                        Linput = new Vector2(0f, Linput.y);
                    LHandle.anchoredPosition = (Linput * LBackground.sizeDelta.x / 2f) * LHandleLimit;
                }
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(RTfield, touchPos))       //Desinio joystick scriptas
            {
                Touch Rtouch = Input.GetTouch(i);
                Vector2 RtouchPos = Rtouch.position;

                if (Rtouch.phase == TouchPhase.Began && RectTransformUtility.RectangleContainsScreenPoint(RJoyBackground, RtouchPos))       //Scripto pradzia
                {
                 //   Debug.Log("Touch Began" + Rtouch.position);     //Touch pradzios debugger
                                                                    //Touch pradzioje keiciasi veriables
                    rightTouch = Rtouch.fingerId;                   
                    RstartingPoint = RtouchPos;
                    RBackground.gameObject.SetActive(true);         //Joystickas atsiranda
                    RBackground.position = RstartingPoint;
                    RJoyPosition = RstartingPoint;
                }
                else if (Rtouch.phase == TouchPhase.Ended)
                {
                   // Debug.Log("Touch End" + Rtouch.position);       //Touch pabaigos debugger

                    rightTouch = 99;
                    Rinput = Vector2.zero;
                    RHandle.anchoredPosition = Vector2.zero;
                    RBackground.anchoredPosition = Vector2.zero;
                    RBackground.gameObject.SetActive(false);        //Joystickas paslepiamas
                }
                else if (Rtouch.phase == TouchPhase.Moved && rightTouch == Rtouch.fingerId)     //Joysticko pirsto sekimo scriptas
                {
                   // Debug.Log("Touch Moved" + Rtouch.position);

                    Vector2 RJoyDriection = RtouchPos - RJoyPosition;
                    Rinput = (RJoyDriection.magnitude > RBackground.sizeDelta.x / 2f) ? RJoyDriection.normalized :
                        RJoyDriection / (RBackground.sizeDelta.x / 2f);
                    if (RJoyStickDirection == RJoyStickDirection.RHorizontal)
                        Rinput = new Vector2(Rinput.x, 0f);
                    if (RJoyStickDirection == RJoyStickDirection.RVertical)
                        Rinput = new Vector2(0f, Rinput.y);
                    RHandle.anchoredPosition = (Rinput * RBackground.sizeDelta.x / 2f) * RHandleLimit;
                }
            }

            i++;
        }
    }
}