using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacialValueManager
{
    public string expression { get; set; }
    public float Facial_Eye_L_Happy { get; set; } 
    public float Facial_Eye_R_Happy { get; set; }  
    public float Facial_Eye_L_Closed { get; set; }  
    public float Facial_Eye_R_Closed { get; set; }  
    public float Facial_Eye_L_Wide { get; set; }  
    public float Facial_Eye_R_Wide { get; set; }  

    public float Facial_Mouth_Sad { get; set; }  
    public float Facial_Mouth_Puff { get; set; }  
    public float Facial_Mouth_Smile { get; set; }  
        
    public float Facial_Eyebrow_L_Up { get; set; }  
    public float Facial_Eyebrow_R_Up { get; set; }  
    public float Facial_Eyebrow_L_Angry { get; set; }  
    public float Facial_Eyebrow_R_Angry { get; set; }  
    public float Facial_Eyebrow_L_Sad { get; set; }  
    public float Facial_Eyebrow_R_Sad { get; set; }  

    public float Facial_Mouth_E { get; set; }  
    public float Facial_Mouth_O { get; set; }  
    public float Facial_Mouth_JawOpen { get; set; }  
    public float Facial_Mouth_Extra01 { get; set; }  
    public float Facial_Mouth_Extra02 { get; set; }  
    public float Facial_Mouth_Extra03 { get; set; }  
    public float Facial_Mouth_BottomTeeth { get; set; }
    public bool Top_Teeth { get; set; }
    public float FacialValue = 100f;

    public FacialValueManager()
    {
        expression = "idle";
        Facial_Eye_L_Happy = 0f;
        Facial_Eye_R_Happy = 0f;
        Facial_Eye_L_Closed = 0f;
        Facial_Eye_R_Closed = 0f;
        Facial_Eye_L_Wide = 0f;
        Facial_Eye_R_Wide = 0f; ;
        Facial_Mouth_Sad = 0f; ;
        Facial_Mouth_Puff = 0f; ;
        Facial_Mouth_Smile = 0f; ;
        Facial_Eyebrow_L_Up = 0f; ;
        Facial_Eyebrow_R_Up = 0f; ;
        Facial_Eyebrow_L_Angry = 0f; ;
        Facial_Eyebrow_R_Angry = 0f; ;
        Facial_Eyebrow_L_Sad = 0f;
        Facial_Eyebrow_R_Sad = 0f;
        Facial_Mouth_E = 0f;
        Facial_Mouth_O = 0f;
        Facial_Mouth_JawOpen = 0f;
        Facial_Mouth_Extra01 = 0f;
        Facial_Mouth_Extra02 = 0f;
        Facial_Mouth_Extra03 = 0f;
        Facial_Mouth_BottomTeeth = 0f;
        Top_Teeth = false;
    }

    public FacialValueManager(string expression, float facial_Eye_L_Happy, float facial_Eye_R_Happy, float facial_Eye_L_Closed, float facial_Eye_R_Closed, float facial_Eye_L_Wide, float facial_Eye_R_Wide, float facial_Mouth_Sad, float facial_Mouth_Puff, float facial_Mouth_Smile, float facial_Eyebrow_L_Up, float facial_Eyebrow_R_Up, float facial_Eyebrow_L_Angry, float facial_Eyebrow_R_Angry, float facial_Eyebrow_L_Sad, float facial_Eyebrow_R_Sad, float facial_Mouth_E, float facial_Mouth_O, float facial_Mouth_JawOpen, float facial_Mouth_Extra01, float facial_Mouth_Extra02, float facial_Mouth_Extra03, float facial_Mouth_BottomTeeth, bool top_Teeth)
    {
        this.expression = expression;
        Facial_Eye_L_Happy = facial_Eye_L_Happy;
        Facial_Eye_R_Happy = facial_Eye_R_Happy;
        Facial_Eye_L_Closed = facial_Eye_L_Closed;
        Facial_Eye_R_Closed = facial_Eye_R_Closed;
        Facial_Eye_L_Wide = facial_Eye_L_Wide;
        Facial_Eye_R_Wide = facial_Eye_R_Wide;
        Facial_Mouth_Sad = facial_Mouth_Sad;
        Facial_Mouth_Puff = facial_Mouth_Puff;
        Facial_Mouth_Smile = facial_Mouth_Smile;
        Facial_Eyebrow_L_Up = facial_Eyebrow_L_Up;
        Facial_Eyebrow_R_Up = facial_Eyebrow_R_Up;
        Facial_Eyebrow_L_Angry = facial_Eyebrow_L_Angry;
        Facial_Eyebrow_R_Angry = facial_Eyebrow_R_Angry;
        Facial_Eyebrow_L_Sad = facial_Eyebrow_L_Sad;
        Facial_Eyebrow_R_Sad = facial_Eyebrow_R_Sad;
        Facial_Mouth_E = facial_Mouth_E;
        Facial_Mouth_O = facial_Mouth_O;
        Facial_Mouth_JawOpen = facial_Mouth_JawOpen;
        Facial_Mouth_Extra01 = facial_Mouth_Extra01;
        Facial_Mouth_Extra02 = facial_Mouth_Extra02;
        Facial_Mouth_Extra03 = facial_Mouth_Extra03;
        Facial_Mouth_BottomTeeth = facial_Mouth_BottomTeeth;
        Top_Teeth = top_Teeth;
    }

    public FacialValueManager(float facialValue)
    {
        FacialValue = facialValue;
    }
}
