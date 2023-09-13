using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpressionControl
{
    private SkinnedMeshRenderer _Renderer_Face;        //Character Skin Mesh Renderer for Face
    private SkinnedMeshRenderer _Renderer_Brow;        //Character Skin Mesh Renderer for Eyebrows
    private SkinnedMeshRenderer _Renderer_BottomTeeth;        //Character Skin Mesh Renderer for Bottom Teeth
    private SkinnedMeshRenderer _Renderer_Tongue;        //Character Tongue Skinned Mesh Renderer
    private SkinnedMeshRenderer _Renderer_TopTeeth;      //Character Top Teeth Skinned Mesh Renderer
    internal FacialValueManager FacialValueManager;

    public void SetRendererFace(SkinnedMeshRenderer Renderer_Face)
    {
        _Renderer_Face = Renderer_Face;
    }

    public void SetRendererBrow(SkinnedMeshRenderer Renderer_Brow)
    {
        _Renderer_Brow = Renderer_Brow;
    }

    public void SetRendererBottomTeeth(SkinnedMeshRenderer Renderer_BottomTeeth)
    {
        _Renderer_BottomTeeth = Renderer_BottomTeeth;
    }

    public void SetRendererTongue(SkinnedMeshRenderer Renderer_Tongue)
    {
        _Renderer_Tongue = Renderer_Tongue;
        _Renderer_Tongue.enabled = false;
    }

    public void SetRendererTopTeeth(SkinnedMeshRenderer Renderer_TopTeeth)
    {
        _Renderer_TopTeeth = Renderer_TopTeeth;
        _Renderer_TopTeeth.enabled = false;
    }

    public void SetFacial()
    {
        /*
         * Ìí¼Ó±íÇé³Ì¶ÈµÄ´úÂë¸ñÊ½
         * _Renderer_Face.SetBlendShapeWeight(0, FacialValueManager.Facial_Eye_L_Happy * FacialValueManager.FacialValue);
         */
        if(FacialValueManager != null) {
            _Renderer_Face.SetBlendShapeWeight(0, FacialValueManager.Facial_Eye_L_Happy * FacialValueManager.FacialValue);
            _Renderer_Face.SetBlendShapeWeight(1, FacialValueManager.Facial_Eye_R_Happy * FacialValueManager.FacialValue);
            _Renderer_Face.SetBlendShapeWeight(4, FacialValueManager.Facial_Eye_L_Closed * FacialValueManager.FacialValue);
            _Renderer_Face.SetBlendShapeWeight(5, FacialValueManager.Facial_Eye_R_Closed * FacialValueManager.FacialValue);
            _Renderer_Face.SetBlendShapeWeight(2, FacialValueManager.Facial_Eye_L_Wide * FacialValueManager.FacialValue);
            _Renderer_Face.SetBlendShapeWeight(3, FacialValueManager.Facial_Eye_R_Wide * FacialValueManager.FacialValue);

            _Renderer_Brow.SetBlendShapeWeight(0, FacialValueManager.Facial_Eyebrow_L_Up * FacialValueManager.FacialValue);
            _Renderer_Brow.SetBlendShapeWeight(1, FacialValueManager.Facial_Eyebrow_R_Up * FacialValueManager.FacialValue);
            _Renderer_Brow.SetBlendShapeWeight(2, FacialValueManager.Facial_Eyebrow_L_Angry * FacialValueManager.FacialValue);
            _Renderer_Brow.SetBlendShapeWeight(3, FacialValueManager.Facial_Eyebrow_R_Angry * FacialValueManager.FacialValue);
            _Renderer_Brow.SetBlendShapeWeight(4, FacialValueManager.Facial_Eyebrow_L_Sad * FacialValueManager.FacialValue);
            _Renderer_Brow.SetBlendShapeWeight(5, FacialValueManager.Facial_Eyebrow_R_Sad * FacialValueManager.FacialValue);

            _Renderer_Face.SetBlendShapeWeight(6, FacialValueManager.Facial_Mouth_E * FacialValueManager.FacialValue);
            _Renderer_Face.SetBlendShapeWeight(8, FacialValueManager.Facial_Mouth_O * FacialValueManager.FacialValue);
            _Renderer_Face.SetBlendShapeWeight(7, FacialValueManager.Facial_Mouth_JawOpen * FacialValueManager.FacialValue);
            _Renderer_Face.SetBlendShapeWeight(12, FacialValueManager.Facial_Mouth_Extra01 * FacialValueManager.FacialValue);
            _Renderer_Face.SetBlendShapeWeight(13, FacialValueManager.Facial_Mouth_Extra02 * FacialValueManager.FacialValue);
            _Renderer_Face.SetBlendShapeWeight(14, FacialValueManager.Facial_Mouth_Extra03 * FacialValueManager.FacialValue);

            _Renderer_Face.SetBlendShapeWeight(9, FacialValueManager.Facial_Mouth_Sad * FacialValueManager.FacialValue);
            _Renderer_Face.SetBlendShapeWeight(10, FacialValueManager.Facial_Mouth_Puff * FacialValueManager.FacialValue);
            _Renderer_Face.SetBlendShapeWeight(11, FacialValueManager.Facial_Mouth_Smile * FacialValueManager.FacialValue);

            if (_Renderer_BottomTeeth.isVisible)
                _Renderer_BottomTeeth.SetBlendShapeWeight(0, FacialValueManager.Facial_Mouth_BottomTeeth * FacialValueManager.FacialValue); 

            if(FacialValueManager.Top_Teeth == true)
            {
                _Renderer_TopTeeth.enabled = true;
            }
            else
            {
                _Renderer_TopTeeth.enabled = false;
            }
        }
        else
        {
            Debug.Log("Î´¶¨ÒåFacialValueManager" + FacialValueManager);
        }
    }
}
