using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour {

    internal string _Animation = null;
    internal string _EyesChangeType = null;
    internal string _EyebrowsChangeType = null;
    internal string _MouthChangeType = null;
    internal string _GeneralChangeType = null;
    internal float _FacialValue = 0.0f;
    internal bool _FacialValueBool = false;

    public void SetAnimation(string newAnimation)
    {
        _Animation = newAnimation;
    }



    public void SetFacialValue(float newFacialValue)
    {
        _FacialValue = newFacialValue;
        //Debug.Log(_FacialValue);
    }
}
