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
        pickers.GetChild(1).GetComponent<ColorPicker>().CurrentColor = colorTransferer.BodyRenderer.color;
        pickers.GetChild(2).GetComponent<ColorPicker>().CurrentColor = colorTransferer.helmetRenderer.color;
        pickers.GetChild(3).GetComponent<ColorPicker>().CurrentColor = colorTransferer.ArmsRenderer.color;
    }


    public void SetVisorColor(Color color){
        if (colorTransferer != null){
            colorTransferer.visorRenderer.color = color;
            colorTransferer.visorRenderer.gameObject.transform.parent.GetComponent<SpriteRenderer>().enabled = false;

            colorTransferer.visorRenderer.gameObject.SetActive(true);
        }
    }

    public void SetHelmetColor(Color color){
        if (colorTransferer != null){
            colorTransferer.helmetRenderer.color = color;
        }
    }

    public void SetBodyColor(Color color){
        if (colorTransferer != null){
            colorTransferer.BodyRenderer.color = color;
        }
    }

    public void SetArmsColor(Color color){
        if (colorTransferer != null){
            colorTransferer.ArmsRenderer.color = color;
        }
    }


}
