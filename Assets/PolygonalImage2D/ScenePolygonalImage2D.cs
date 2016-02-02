using UnityEngine;
using System.Collections;

public class ScenePolygonalImage2D : MonoBehaviour
{
    public GameObject image0_;
    public GameObject image1_;

    private int mode_;

    void change(int n)
    {
        GameObject show = (n == 0)? image0_ : image1_;
        GameObject hide = (n == 0)? image1_ : image0_;
        for(int i=0; i<show.transform.childCount; ++i){
            show.transform.GetChild(i).gameObject.SetActive(true);
        }
        for(int i=0; i<hide.transform.childCount; ++i){
            hide.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    void Start()
    {
        change(0);
    }

	void OnGUI()
    {
        if(GUI.Button(new Rect(10, 10, 100, 40), "Change")){
            mode_ = (mode_ == 0)? 1 : 0;
            change(mode_);
        }
	}
}
