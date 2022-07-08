// used to disable/enable our menus
// menus have a Menu.cs script attached to them
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] Menu[] menus;

    void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string menuName)
    {
        // open menu based on string
        for(int i = 0; i < menus.Length; i++) {
            // print(menus[i].menuName);
            if(menus[i].menuName == menuName) {
                // print("here");
                menus[i].Open();
            }
            else if(menus[i].open){
                CloseMenu(menus[i]);
            }
        }
    }

    public void OpenMenu(Menu menu)
    {
        // open menu based on Menu object
        // good for references inside of unity editor
        for(int i = 0; i < menus.Length; i++) {
            if(menus[i].open){
                CloseMenu(menus[i]);
            }
        }

        menu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
