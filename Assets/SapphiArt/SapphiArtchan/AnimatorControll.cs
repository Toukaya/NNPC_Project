using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AnimatorControll : MonoBehaviour
{
    private Animator _Animator;
    [SerializeField] private string newAnimation = "idle"; 
    private string _Animation = "idle";
    private string _LastAnimation = "idle";
    private bool toidle = false;
    //private float _FacialValue = 0.0f;
    [SerializeField] private string expression = "idle";

    private ExpressionControl _ExpressionControl;
    FacialValueManager[] FacialValueManagerArray;

    void Start()
    {
        _Animator = gameObject.GetComponent<Animator>();

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
            new FacialValueManager("idle", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f , 0f , false),
            new FacialValueManager() { expression = "happy" , Facial_Eye_L_Happy =  30f, Facial_Eye_R_Happy = 30f, Facial_Mouth_Smile = 100f}
        };

        SetFacialtoIdle();
        //_ExpressionControl.FacialValueManager = FacialValueManagerArray.FirstOrDefault(fvm => fvm.expression == "idle");
    }
    void Update()
    {
        SetAnimation();
        ChangeExpression();
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
            if (_Animation != "walk" && _Animation != "run")
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
        }
    }

    void SetFacialtoIdle()
    {
        expression = "idle";
        ChangeExpression();
    }

    void ChangeExpression()
    {
        FacialValueManager tempFacialValueManager = FacialValueManagerArray.FirstOrDefault(fvm => fvm.expression == expression);
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
}
