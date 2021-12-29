using System;
using System.Collections;
using System.Collections.Generic;
using HSVPicker;
using UnityEngine;

public class Pause : MonoBehaviour{

    private ColorTransferer colorTransferer;
    private Color[] _colors;

    public void SetColorTransferer(ColorTransferer ct){
        colorTransferer = ct;
    }

    private void Start(){
        Transform pickers = gameObject.transform.GetChild(1);

        pickers.GetChild(0).GetComponent<ColorPicker>().CurrentColor = colorTransferer.visorRenderer.color;
        pickers.GetChild(1).GetComponent<ColorPicker>().CurrentColor = colorTransferer.bodyRenderer.color;
        pickers.GetChild(2).GetComponent<ColorPicker>().CurrentColor = colorTransferer.helmetRenderer.color;
        pickers.GetChild(3).GetComponent<ColorPicker>().CurrentColor = colorTransferer.armsRenderer.color;
        
    }


    public void SetVisorColor(Color color){
        if (colorTransferer != null){
            colorTransferer.visorRenderer.color = color;
            colorTransferer.visorRenderer.gameObject.transform.parent.GetComponent<SpriteRenderer>().enabled = false;

            colorTransferer.helmetTransform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
            Transform VisorR  =colorTransferer.helmetTransform.GetChild(2);
            VisorR.gameObject.SetActive(true);
            VisorR.GetComponent<SpriteRenderer>().color = color;


            colorTransferer.visorRenderer.gameObject.SetActive(true);
        }
    }

    public void SetHelmetColor(Color color){
        if (colorTransferer != null){
            colorTransferer.helmetRenderer.color = color;
            colorTransferer.bottomRenderer.color = color;
            colorTransferer.helmetTransform.GetChild(0).GetComponent<SpriteRenderer>().color = color;

        }
    }

    public void SetBodyColor(Color color){
        if (colorTransferer != null){
            colorTransferer.bodyRenderer.color = color;
        }
    }

    public void SetArmsColor(Color color){
        if (colorTransferer != null){
            colorTransferer.armsRenderer.color = color;
            colorTransferer.armTransform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
        }
    }


}
