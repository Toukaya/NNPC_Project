using System;
using System.Linq;
using UnityEngine;

public class AnimatorControl : MonoBehaviour
{
    private Animator _Animator;
    // private ClientNetworkManager _ClientNetworkManager;
    [SerializeField] 
    internal string newAnimation = "idle"; 
    private string _Animation = "idle";
    private string _LastAnimation = "idle";
    private bool toidle = false;
    //private float _FacialValue = 0.0f;
    [SerializeField] 
    internal string expression = "idle";
    // internal string text = "";

    private ExpressionControl _ExpressionControl;
    FacialValueManager[] FacialValueManagerArray;

    private bool Speak_Open = true;

    void Start()
    {
        _Animator = gameObject.GetComponent<Animator>();
        // _ClientNetworkManager = GetComponent<ClientNetworkManager>();

        Transform[] AnimatorChildren = GetComponentsInChildren<Transform>();

        _ExpressionControl = new ExpressionControl();

        foreach (Transform t in AnimatorChildren)
        {
            if (t.name == "face")
                _ExpressionControl.SetRendererFace(t.gameObject.GetComponent<SkinnedMeshRenderer>());
            if (t.name == "brow")
                _ExpressionControl.SetRendererBrow(t.gameObject.GetComponent<SkinnedMeshRenderer>());
            if (t.name == "BottomTeeth")
                _ExpressionControl.SetRendererBottomTeeth(t.gameObject.GetComponent<SkinnedMeshRenderer>());
            if (t.name == "tongue")
                _ExpressionControl.SetRendererTongue(t.gameObject.GetComponent<SkinnedMeshRenderer>());
            if (t.name == "TopTeeth")
                _ExpressionControl.SetRendererTopTeeth(t.gameObject.GetComponent<SkinnedMeshRenderer>());
        }
/*        _ExpressionControl._Renderer_Tongue.enabled = false;
        _ExpressionControl._Renderer_TopTeeth.enabled = false;*/

        FacialValueManagerArray = new FacialValueManager[]
        {
            new FacialValueManager("idle", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f , 0f, false),
            new FacialValueManager(){ expression = "happy" , Facial_Eye_L_Happy =  30f, Facial_Eye_R_Happy = 30f, Facial_Mouth_Smile = 100f},
            new FacialValueManager(){ expression = "smile", Facial_Mouth_Smile = 1.0f, Facial_Mouth_BottomTeeth = 0.5f,},
            new FacialValueManager(){ expression = "laugh", Facial_Mouth_Smile = 1.0f,Facial_Eye_L_Happy = 0.8f,Facial_Eye_R_Happy = 0.8f,Facial_Eye_L_Closed = 0.2f,Facial_Eye_R_Closed = 0.2f,Facial_Eyebrow_L_Up = 0.8f,Facial_Eyebrow_R_Up = 0.8f,Facial_Mouth_BottomTeeth = 0.5f,},
            new FacialValueManager(){ expression = "grin", Facial_Mouth_Smile = 1.0f,Facial_Eye_L_Happy = 0.5f,Facial_Eye_R_Happy = 0.5f,Facial_Eye_L_Closed = 0.2f,Facial_Eye_R_Closed = 0.2f,Top_Teeth = true,Facial_Mouth_BottomTeeth = 0.5f,},
            new FacialValueManager(){ expression = "bitter_smile", Facial_Eyebrow_L_Sad = 1.0f,Facial_Eyebrow_R_Sad = 1.0f,Facial_Eyebrow_L_Angry = 0.5f,Facial_Eyebrow_R_Angry = 0.5f,Facial_Eye_L_Closed = 0.2f,Facial_Eye_R_Closed = 0.2f,Facial_Mouth_Sad = 1.0f,Facial_Mouth_BottomTeeth = 0.5f,},
            new FacialValueManager(){ expression = "frown", Facial_Eyebrow_L_Sad = 0.4f,Facial_Eyebrow_R_Sad = 0.4f,Facial_Eyebrow_L_Angry = 0.8f,Facial_Eyebrow_R_Angry = 0.8f,Facial_Eye_L_Closed = 0.3f,Facial_Eye_R_Closed = 0.3f,Facial_Mouth_Sad = 0.5f,Facial_Mouth_BottomTeeth = 0.5f,},
            new FacialValueManager(){ expression = "glare", Facial_Eyebrow_L_Angry = 1.0f,Facial_Eyebrow_R_Angry = 1.0f,Facial_Eye_L_Closed = 0.5f,Facial_Eye_R_Closed = 0.5f,Facial_Mouth_Sad = 1.0f,Facial_Mouth_BottomTeeth = 0.5f,},
            new FacialValueManager(){ expression = "cry", Facial_Eyebrow_L_Sad = 1.0f,Facial_Eyebrow_R_Sad = 1.0f,Facial_Eye_L_Closed = 0.3f,Facial_Eye_R_Closed = 0.3f,Facial_Mouth_Sad = 1.0f,Facial_Mouth_BottomTeeth = 0.5f, },
            new FacialValueManager(){ expression = "surprise", Facial_Eyebrow_L_Up = 0.5f,Facial_Eyebrow_R_Up = 0.5f,Facial_Eye_L_Wide = 0.6f,Facial_Eye_R_Wide = 0.6f,Facial_Mouth_Sad = 0.7f,Facial_Mouth_BottomTeeth = 0.7f,},
            new FacialValueManager(){ expression = "fear", Facial_Eyebrow_L_Up = 0.6f,Facial_Eyebrow_R_Up = 0.6f,Facial_Eyebrow_L_Angry = 0.5f,Facial_Eyebrow_R_Angry = 0.5f,Facial_Eye_L_Closed = 0.2f,Facial_Eye_R_Closed = 0.2f,Facial_Mouth_Sad = 1.0f,Facial_Mouth_BottomTeeth = 0.5f,},
            new FacialValueManager(){ expression = "anger", Facial_Eyebrow_L_Angry = 1.0f,Facial_Eyebrow_R_Angry = 1.0f,Facial_Mouth_Sad = 0.5f,Facial_Mouth_BottomTeeth = 0.5f,},
            new FacialValueManager(){ expression = "disgust", Facial_Eyebrow_L_Sad = 0.4f,Facial_Eyebrow_R_Sad = 0.4f,Facial_Eyebrow_L_Angry = 1.0f,Facial_Eyebrow_R_Angry = 1.0f,Facial_Eye_L_Closed = 0.2f,Facial_Eye_R_Closed = 0.2f,Facial_Mouth_Sad = 0.5f,Facial_Mouth_BottomTeeth = 0.5f,},
            new FacialValueManager(){ expression = "confusion", Facial_Eyebrow_L_Angry = 0.3f,Facial_Eyebrow_R_Angry = 1.0f,Facial_Eye_L_Closed = 0.3f,Facial_Eye_R_Closed = 0.1f,Facial_Mouth_Sad = 0.5f,Facial_Mouth_BottomTeeth = 0.5f,},
            new FacialValueManager(){ expression = "curiosity", Facial_Eyebrow_L_Sad = 1.0f,Facial_Eyebrow_R_Sad = 1.0f,Facial_Eyebrow_L_Angry = 0.4f,Facial_Eyebrow_R_Angry = 0.4f,Facial_Eye_L_Closed = 0.2f,Facial_Eye_R_Closed = 0.2f,Facial_Mouth_Sad = 0.5f,Facial_Mouth_BottomTeeth = 0.5f,},
            new FacialValueManager(){ expression = "focus", Facial_Eye_L_Closed = 0.1f,Facial_Eye_R_Closed = 0.1f,Facial_Mouth_Sad = 0.5f,Facial_Mouth_BottomTeeth = 0.5f,},
            new FacialValueManager(){ expression = "satisfaction", Facial_Eye_L_Closed = 0.1f,Facial_Eye_R_Closed = 0.1f,Facial_Mouth_Smile = 1.0f,Facial_Mouth_BottomTeeth = 0.5f,},
        };

        SetFacialtoIdle();
        //_ExpressionControl.FacialValueManager = FacialValueManagerArray.FirstOrDefault(fvm => fvm.expression == "idle");
    }
    void Update()
    {
        //GetMessage();
        SetAnimation();
        ChangeExpression();
        // IsSpeaking();
        //Speak();
    }

    void LateUpdate()
    {
        //Set Facialto0
        _ExpressionControl.SetFacial();
    }

    void SetAnimation()
    {
        //Record Last Animation
        _LastAnimation = _Animation;

        //Set Animation Parameter
        _Animation = newAnimation;

        //Set New Animation
        if (_LastAnimation != _Animation)
        {
            ChangeAnimation();
            ChangeExpression();
        }
        else if (!_Animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            ReturnToIdle();
        }
        else hadtoIdle();
    }

/*    void GetMessage()
    {
        newAnimation = _ClientNetworkManager.action;
        expression = _ClientNetworkManager.emotion;
        text = _ClientNetworkManager.text;
    }*/
    void ChangeAnimation()
    {
        SetAllAnimationFlagsToFalse();

        if (_Animation != "idle")
        {
            _Animator.SetBool("to" + _Animation, true);
        }
    }

    void SetAllAnimationFlagsToFalse()
    {
        _Animator.SetBool("towalk", false);
        _Animator.SetBool("torun", false);
        _Animator.SetBool("tojump", false);
        _Animator.SetBool("towave_hand", false);
        _Animator.SetBool("toclap", false);
        _Animator.SetBool("tonod", false);
        _Animator.SetBool("toshake_head", false);
        _Animator.SetBool("tobow", false);
        _Animator.SetBool("tohandshake", false);
        _Animator.SetBool("tofist_bump", false);
        _Animator.SetBool("tosalute", false);
        _Animator.SetBool("toshrug", false);
        _Animator.SetBool("topoint", false);
        _Animator.SetBool("tothinking", false);
/*        _Animator.SetBool("todisappointed", false);
        _Animator.SetBool("toexcited", false);
        _Animator.SetBool("tofocus", false);
        _Animator.SetBool("tohappy", false);
        _Animator.SetBool("tosad", false);
        _Animator.SetBool("tothankful", false);
        _Animator.SetBool("tothinking", false);*/
    }

    void ReturnToIdle()
    {
        if (_Animator.GetCurrentAnimatorStateInfo(0).IsName(_Animation))
        {
            if (_Animation != "walk" && _Animation != "run" && _Animation!= "thinking")
            {
                SetAllAnimationFlagsToFalse();
                toidle = true;
            }
        }
    }

    void hadtoIdle()
    {
        if (toidle && _Animation != "idle")
        {
            SetFacialtoIdle();
            toidle = false;
            newAnimation = "idle";
        }
    }

    void SetFacialtoIdle()
    {
        expression = "idle";
        ChangeExpression();
    }

    void ChangeExpression()
    {
        FacialValueManager tempFacialValueManager = FacialValueManagerArray.FirstOrDefault(fvm => fvm.expression.Equals(expression, StringComparison.CurrentCultureIgnoreCase));
        if(tempFacialValueManager != null)
        {
            _ExpressionControl.FacialValueManager = tempFacialValueManager;
        }
        else
        {
            tempFacialValueManager = FacialValueManagerArray.FirstOrDefault(fvm => fvm.expression == "idle");
            if (tempFacialValueManager != null)
            {
                _ExpressionControl.FacialValueManager = tempFacialValueManager;
            }
            Debug.Log("表情" + expression +"不存在");
        }
    }

    void Speak()
    {
        if (Speak_Open)
        {
            if (_ExpressionControl.FacialValueManager.Facial_Mouth_JawOpen < 0.8f)     //说话开合度
                _ExpressionControl.FacialValueManager.Facial_Mouth_JawOpen += 0.02f;    //说话速度
            else
            {
                Speak_Open = false;       //置换为合嘴
            }
        }
        else
        {
            if (_ExpressionControl.FacialValueManager.Facial_Mouth_JawOpen > 0f)        //说话开合度
                _ExpressionControl.FacialValueManager.Facial_Mouth_JawOpen -= 0.02f;     //说话速度
            else
            {
                Speak_Open = true;          //置换为开嘴
            }
        }
    }

    public void Speak(float volume)
    {
        if (_ExpressionControl.FacialValueManager.Facial_Mouth_JawOpen < volume)
            _ExpressionControl.FacialValueManager.Facial_Mouth_JawOpen += 0.2f;
        else if (_ExpressionControl.FacialValueManager.Facial_Mouth_JawOpen > 0f)
            _ExpressionControl.FacialValueManager.Facial_Mouth_JawOpen -= 0.2f;
    }
    
    // void IsSpeaking()
    // {
    //     if ( _ClientNetworkManager.audioSource != null && _ClientNetworkManager.audioSource.isPlaying) 
    //     { 
    //         Speak(); 
    //     }
    //     else{
    //         Speak_Open = true;
    //         _ExpressionControl.FacialValueManager.Facial_Mouth_JawOpen = 0f;
    //     }
    // }
}
