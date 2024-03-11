using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AnimatorController : MonoBehaviour
{
    private Animator _Animator;
    private AnimationManager _AnimationManager;
    private Dropdown _dropdown;
    private string _Animation = null;      //Character Animation Name
    private string _LastAnimation = null;  //Character Last Animation
    private bool toidle = false;
    private float _FacialValue = 0.0f;

    private SkinnedMeshRenderer _Renderer_Face;        //Character Skin Mesh Renderer for Face
    private SkinnedMeshRenderer _Renderer_Brow;        //Character Skin Mesh Renderer for Eyebrows
    private SkinnedMeshRenderer _Renderer_BottomTeeth;        //Character Skin Mesh Renderer for Bottom Teeth
    private SkinnedMeshRenderer _Renderer_Tongue;        //Character Tongue Skinned Mesh Renderer
    private SkinnedMeshRenderer _Renderer_TopTeeth;      //Character Top Teeth Skinned Mesh Renderer

    private float _Facial_Eye_L_Happy = 0.0f;
    private float _Facial_Eye_R_Happy = 0.0f;
    private float _Facial_Eye_L_Closed = 0.0f;
    private float _Facial_Eye_R_Closed = 0.0f;
    private float _Facial_Eye_L_Wide = 0.0f;
    private float _Facial_Eye_R_Wide = 0.0f;

    private float _Facial_Mouth_Sad = 0.0f;
    private float _Facial_Mouth_Puff = 0.0f;
    private float _Facial_Mouth_Smile = 0.0f;

    private float _Facial_Eyebrow_L_Up = 0.0f;
    private float _Facial_Eyebrow_R_Up = 0.0f;
    private float _Facial_Eyebrow_L_Angry = 0.0f;
    private float _Facial_Eyebrow_R_Angry = 0.0f;
    private float _Facial_Eyebrow_L_Sad = 0.0f;
    private float _Facial_Eyebrow_R_Sad = 0.0f;

    private float _Facial_Mouth_E = 0.0f;
    private float _Facial_Mouth_O = 0.0f;
    private float _Facial_Mouth_JawOpen = 0.0f;
    private float _Facial_Mouth_Extra01 = 0.0f;
    private float _Facial_Mouth_Extra02 = 0.0f;
    private float _Facial_Mouth_Extra03 = 0.0f;
    private float _Facial_Mouth_BottomTeeth = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _Animator = gameObject.GetComponent<Animator>();
        _AnimationManager = GameObject.Find("AnimationManager").GetComponent<AnimationManager>();
        _dropdown = GameObject.Find("Dropdown").GetComponent<Dropdown>();
        _dropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(_dropdown); });

        Transform[] AnimatorChildren = GetComponentsInChildren<Transform>();

        foreach (Transform t in AnimatorChildren)
        {
            if (t.name == "face")
                _Renderer_Face = t.gameObject.GetComponent<SkinnedMeshRenderer>();
            if (t.name == "brow")
                _Renderer_Brow = t.gameObject.GetComponent<SkinnedMeshRenderer>();
            if (t.name == "BottomTeeth")
                _Renderer_BottomTeeth = t.gameObject.GetComponent<SkinnedMeshRenderer>();
            if (t.name == "tongue")
                _Renderer_Tongue = t.gameObject.GetComponent<SkinnedMeshRenderer>();
            if (t.name == "TopTeeth")
                _Renderer_TopTeeth = t.gameObject.GetComponent<SkinnedMeshRenderer>();
        }
        _Renderer_Tongue.enabled = false;
        _Renderer_TopTeeth.enabled = false;
    }

    void OnDropdownValueChanged(Dropdown dropdown)
    {
        string selectedOption = dropdown.options[dropdown.value].text;
        _AnimationManager.SetAnimation(selectedOption);
    }

    void GetAnimation()
    {
        //Record Last Animation
        _LastAnimation = _Animation;

        if (_Animation == null)
            _Animation = "idle";

        else
        {
            //Set Animation Parameter
            _Animation = _AnimationManager._Animation;
        }
    }

    void SetAnimation()
    {
        SetAllAnimationFlagsToFalse();

        if (_Animation == "idle")
        {
            SetAllAnimationFlagsToFalse();
        }
        else
        {
            _Animator.SetBool("to" + _Animation, true);
        }
    }

    void SetAllAnimationFlagsToFalse()
    {
        _Animator.SetBool("towalk", false);
        _Animator.SetBool("torunning", false);
        _Animator.SetBool("tojump", false);
        _Animator.SetBool("towinpose", false);
        _Animator.SetBool("toko_big", false);
        _Animator.SetBool("todamage", false);
        _Animator.SetBool("tohit01", false);
        _Animator.SetBool("tohit02", false);
        _Animator.SetBool("tohit03", false);
        _Animator.SetBool("tosurprised", false);
        _Animator.SetBool("tostand", false);
        _Animator.SetBool("todancing", false);
        _Animator.SetBool("toangry", false);
        _Animator.SetBool("todisappointed", false);
        _Animator.SetBool("toexcited", false);
        _Animator.SetBool("tofocus", false);
        _Animator.SetBool("tohappy", false);
        _Animator.SetBool("tosad", false);
        _Animator.SetBool("tothankful", false);
        _Animator.SetBool("tothinking", false);
    }

    void ReturnToIdle()
    {
        if (_Animator.GetCurrentAnimatorStateInfo(0).IsName(_Animation))
        {
            if ( _Animation != "walk" && _Animation != "running")
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
            _dropdown.value = 0;
            toidle = false;
        }
    }

    void SetFacial()
    {
        _Renderer_Face.SetBlendShapeWeight(0, _Facial_Eye_L_Happy);
        _Renderer_Face.SetBlendShapeWeight(1, _Facial_Eye_R_Happy);
        _Renderer_Face.SetBlendShapeWeight(4, _Facial_Eye_L_Closed);
        _Renderer_Face.SetBlendShapeWeight(5, _Facial_Eye_R_Closed);
        _Renderer_Face.SetBlendShapeWeight(2, _Facial_Eye_L_Wide);
        _Renderer_Face.SetBlendShapeWeight(3, _Facial_Eye_R_Wide);

        _Renderer_Brow.SetBlendShapeWeight(0, _Facial_Eyebrow_L_Up);
        _Renderer_Brow.SetBlendShapeWeight(1, _Facial_Eyebrow_R_Up);
        _Renderer_Brow.SetBlendShapeWeight(2, _Facial_Eyebrow_L_Angry);
        _Renderer_Brow.SetBlendShapeWeight(3, _Facial_Eyebrow_R_Angry);
        _Renderer_Brow.SetBlendShapeWeight(4, _Facial_Eyebrow_L_Sad);
        _Renderer_Brow.SetBlendShapeWeight(5, _Facial_Eyebrow_R_Sad);

        _Renderer_Face.SetBlendShapeWeight(6, _Facial_Mouth_E);
        _Renderer_Face.SetBlendShapeWeight(8, _Facial_Mouth_O);
        _Renderer_Face.SetBlendShapeWeight(7, _Facial_Mouth_JawOpen);
        _Renderer_Face.SetBlendShapeWeight(12, _Facial_Mouth_Extra01);
        _Renderer_Face.SetBlendShapeWeight(13, _Facial_Mouth_Extra02);
        _Renderer_Face.SetBlendShapeWeight(14, _Facial_Mouth_Extra03);

        _Renderer_Face.SetBlendShapeWeight(9, _Facial_Mouth_Sad);
        _Renderer_Face.SetBlendShapeWeight(10, _Facial_Mouth_Puff);
        _Renderer_Face.SetBlendShapeWeight(11, _Facial_Mouth_Smile);

        if (_Renderer_BottomTeeth.isVisible)
            _Renderer_BottomTeeth.SetBlendShapeWeight(0, _Facial_Mouth_BottomTeeth);
    }

    void changeFacalValue()
    {
        _FacialValue = _AnimationManager._FacialValue;

        if( _Animation == "happy")
        {
            _Facial_Eye_L_Happy = _FacialValue * 0.3f;
            _Renderer_Face.SetBlendShapeWeight(0, _Facial_Eye_L_Happy);
            _Facial_Eye_R_Happy = _FacialValue * 0.3f;
            _Renderer_Face.SetBlendShapeWeight(1, _Facial_Eye_R_Happy);
            _Facial_Mouth_Smile = _FacialValue;
            _Renderer_Face.SetBlendShapeWeight(11, _Facial_Mouth_Smile);
        }
        else if (_Animation == "surprised")
        {
            _Facial_Mouth_O = _FacialValue * 0.5f;
            _Renderer_Face.SetBlendShapeWeight(8, _Facial_Mouth_O);
            _Facial_Mouth_JawOpen = _FacialValue * 1.0f;
            _Renderer_Face.SetBlendShapeWeight(7, _Facial_Mouth_JawOpen);
            _Facial_Eye_L_Wide = _FacialValue * 1.0f;
            _Renderer_Face.SetBlendShapeWeight(2, _Facial_Eye_L_Wide);
            _Facial_Eye_R_Wide = _FacialValue * 1.0f;
            _Renderer_Face.SetBlendShapeWeight(3, _Facial_Eye_R_Wide);
            _Facial_Eyebrow_L_Up = _FacialValue * 1.0f;
            _Renderer_Brow.SetBlendShapeWeight(0, _Facial_Eyebrow_L_Up);
            _Facial_Eyebrow_R_Up = _FacialValue * 1.0f;
            _Renderer_Brow.SetBlendShapeWeight(1, _Facial_Eyebrow_R_Up);
            _Facial_Eyebrow_L_Angry = _FacialValue * 0.4f;
            _Renderer_Brow.SetBlendShapeWeight(2, _Facial_Eyebrow_L_Angry);
            _Facial_Eyebrow_R_Angry = _FacialValue * 0.4f;
            _Renderer_Brow.SetBlendShapeWeight(3, _Facial_Eyebrow_R_Angry);
        }
        else if (_Animation == "angry")
        {
            _Facial_Eyebrow_L_Angry = _FacialValue * 0.5f;
            _Renderer_Brow.SetBlendShapeWeight(2, _Facial_Eyebrow_L_Angry);
            _Facial_Eyebrow_R_Angry = _FacialValue * 0.5f;
            _Renderer_Brow.SetBlendShapeWeight(3, _Facial_Eyebrow_R_Angry);
            _Facial_Eye_L_Closed = _FacialValue * 0.3f;
            _Renderer_Face.SetBlendShapeWeight(4, _Facial_Eye_L_Closed);
            _Facial_Eye_R_Closed = _FacialValue * 0.3f;
            _Renderer_Face.SetBlendShapeWeight(5, _Facial_Eye_R_Closed);
            _Facial_Mouth_E =_FacialValue * 0.5f;
            _Renderer_Face.SetBlendShapeWeight(6, _Facial_Mouth_E);
        }
        else if( _Animation == "disappointed")
        {
            _Facial_Eyebrow_L_Sad = _FacialValue * 1.0f;
            _Renderer_Brow.SetBlendShapeWeight(4, _Facial_Eyebrow_L_Sad);
            _Facial_Eyebrow_R_Sad = _FacialValue * 1.0f;
            _Renderer_Brow.SetBlendShapeWeight(5, _Facial_Eyebrow_R_Sad);
            _Facial_Eye_L_Closed = _FacialValue * 0.2f;
            _Renderer_Face.SetBlendShapeWeight(4, _Facial_Eye_L_Closed);
            _Facial_Eye_R_Closed = _FacialValue * 0.2f;
            _Renderer_Face.SetBlendShapeWeight(5, _Facial_Eye_R_Closed);
            _Facial_Mouth_BottomTeeth = _FacialValue * 1.0f;
            _Renderer_BottomTeeth.SetBlendShapeWeight(0, _Facial_Mouth_BottomTeeth);
            _Facial_Mouth_Extra01 = _FacialValue * 0.5f;
            _Renderer_Face.SetBlendShapeWeight(12, _Facial_Mouth_Extra01);
        }
        else if (_Animation == "excited")
        {
            _Facial_Eye_L_Happy = _FacialValue * 0.3f;
            _Renderer_Face.SetBlendShapeWeight(0, _Facial_Eye_L_Happy);
            _Facial_Eye_R_Happy = _FacialValue * 0.3f;
            _Renderer_Face.SetBlendShapeWeight(1, _Facial_Eye_R_Happy);
            _Facial_Mouth_Smile = _FacialValue;
            _Renderer_Face.SetBlendShapeWeight(11, _Facial_Mouth_Smile);
        }
        else if(_Animation == "sad")
        {
            _Facial_Eyebrow_L_Sad = _FacialValue * 1.0f;
            _Renderer_Brow.SetBlendShapeWeight(4, _Facial_Eyebrow_L_Sad);
            _Facial_Eyebrow_R_Sad = _FacialValue * 1.0f;
            _Renderer_Brow.SetBlendShapeWeight(5, _Facial_Eyebrow_R_Sad);
            _Facial_Eye_L_Closed = _FacialValue * 0.2f;
            _Renderer_Face.SetBlendShapeWeight(4, _Facial_Eye_L_Closed);
            _Facial_Eye_R_Closed = _FacialValue * 0.2f;
            _Renderer_Face.SetBlendShapeWeight(5, _Facial_Eye_R_Closed);
            _Facial_Mouth_BottomTeeth = _FacialValue * 0.6f;
            _Renderer_BottomTeeth.SetBlendShapeWeight(0, _Facial_Mouth_BottomTeeth);
            _Facial_Mouth_Extra01 = _FacialValue * 0.3f;
            _Renderer_Face.SetBlendShapeWeight(12, _Facial_Mouth_Extra01);
        }
        else if(_Animation == "thinking")
        {
            _Facial_Eye_L_Closed = _FacialValue * 0.3f;
            _Renderer_Face.SetBlendShapeWeight(4, _Facial_Eye_L_Closed);
            _Facial_Eye_R_Closed = _FacialValue * 0.3f;
            _Renderer_Face.SetBlendShapeWeight(5, _Facial_Eye_R_Closed);
            _Facial_Eyebrow_L_Angry = _FacialValue * 0.2f;
            _Renderer_Brow.SetBlendShapeWeight(2, _Facial_Eyebrow_L_Angry);
            _Facial_Eyebrow_R_Angry = _FacialValue * 0.2f;
            _Renderer_Brow.SetBlendShapeWeight(3, _Facial_Eyebrow_R_Angry);
        }
        else if(_Animation == "damage")
        {
            _Facial_Eyebrow_L_Sad = _FacialValue * 1.0f;
            _Renderer_Brow.SetBlendShapeWeight(4, _Facial_Eyebrow_L_Sad);
            _Facial_Eyebrow_R_Sad = _FacialValue * 1.0f;
            _Renderer_Brow.SetBlendShapeWeight(5, _Facial_Eyebrow_R_Sad);
            _Facial_Eye_L_Closed = _FacialValue * 0.3f;
            _Renderer_Face.SetBlendShapeWeight(4, _Facial_Eye_L_Closed);
            _Facial_Eye_R_Closed = _FacialValue * 0.3f;
            _Renderer_Face.SetBlendShapeWeight(5, _Facial_Eye_R_Closed);
            _Facial_Mouth_JawOpen = _FacialValue * 0.8f;
            _Renderer_Face.SetBlendShapeWeight(7, _Facial_Mouth_JawOpen);
            _Facial_Mouth_BottomTeeth = _FacialValue * 1.0f;
            _Renderer_BottomTeeth.SetBlendShapeWeight(0, _Facial_Mouth_BottomTeeth);
        }
        else if(_Animation == "winpose")
        {
            _Facial_Eyebrow_L_Up = _FacialValue * 0.5f;
            _Renderer_Brow.SetBlendShapeWeight(0, _Facial_Eyebrow_L_Up);
            _Facial_Eyebrow_R_Up = _FacialValue * 0.5f;
            _Renderer_Brow.SetBlendShapeWeight(1, _Facial_Eyebrow_R_Up);
            _Facial_Eye_L_Wide = _FacialValue * 0.5f;
            _Renderer_Face.SetBlendShapeWeight(2, _Facial_Eye_L_Wide);
            _Facial_Eye_R_Wide = _FacialValue * 0.5f;
            _Renderer_Face.SetBlendShapeWeight(3, _Facial_Eye_R_Wide);
            _Facial_Mouth_Smile = _FacialValue * 1.0f;
            _Renderer_Face.SetBlendShapeWeight(11, _Facial_Mouth_Smile);
        }
    }

    void SetFacialtoIdle()
    {
        _Facial_Eye_L_Happy = 0.0f;
        _Facial_Eye_R_Happy = 0.0f;
        _Facial_Eye_L_Closed = 0.0f;
        _Facial_Eye_R_Closed = 0.0f;
        _Facial_Eye_L_Wide = 0.0f;
        _Facial_Eye_R_Wide = 0.0f;

        _Facial_Mouth_Sad = 0.0f;
        _Facial_Mouth_Puff = 0.0f;
        _Facial_Mouth_Smile = 0.0f;

        _Facial_Eyebrow_L_Up = 0.0f;
        _Facial_Eyebrow_R_Up = 0.0f;
        _Facial_Eyebrow_L_Angry = 0.0f;
        _Facial_Eyebrow_R_Angry = 0.0f;
        _Facial_Eyebrow_L_Sad = 0.0f;
        _Facial_Eyebrow_R_Sad = 0.0f;

        _Facial_Mouth_E = 0.0f;
        _Facial_Mouth_O = 0.0f;
        _Facial_Mouth_JawOpen = 0.0f;
        _Facial_Mouth_Extra01 = 0.0f;
        _Facial_Mouth_Extra02 = 0.0f;
        _Facial_Mouth_Extra03 = 0.0f;
        _Facial_Mouth_BottomTeeth = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        GetAnimation();
        //Set New Animation
        if (_LastAnimation != _Animation)
        {
            SetAnimation();
            changeFacalValue();
        }
        else if(!_Animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            ReturnToIdle();
        }
        else hadtoIdle();
    }

    void LateUpdate()
    {
        //Set Facialto0
        SetFacial();
    }
}
