using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTransferer : MonoBehaviour
{
    
    [SerializeField] private GameObject pauseMenuPrefab;
    
    [SerializeField] public SpriteRenderer visorRenderer;
    [SerializeField] public SpriteRenderer helmetRenderer;
    [SerializeField] public SpriteRenderer bodyRenderer;
    [SerializeField] public SpriteRenderer armsRenderer;
    [SerializeField] public SpriteRenderer bottomRenderer;
    [SerializeField] public Transform helmetTransform;
    [SerializeField] public Transform armTransform;
    
    
    private GameObject pauseMenu;
    
    public void TogglePause(){
        if (Time.timeScale == 1){
            Time.timeScale = 0;
            pauseMenu = Instantiate(pauseMenuPrefab).transform.GetChild(0).gameObject;
            pauseMenu.GetComponent<Pause>().SetColorTransferer(this);
        }
        else{
            Time.timeScale = 1;
            Destroy(pauseMenu); 
        }
    }
}
