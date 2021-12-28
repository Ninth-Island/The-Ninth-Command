using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTransferer : MonoBehaviour
{
    
    [SerializeField] private GameObject pauseMenuPrefab;
    
    [SerializeField] public SpriteRenderer visorRenderer;
    [SerializeField] public SpriteRenderer helmetRenderer;
    [SerializeField] public SpriteRenderer BodyRenderer;
    [SerializeField] public SpriteRenderer ArmsRenderer;
    
    
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
