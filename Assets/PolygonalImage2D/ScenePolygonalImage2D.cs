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

    void duplicate(Vector2 offset, GameObject root)
    {
        GameObject[] tmps = new GameObject[root.transform.childCount];

        for(int i=0; i<root.transform.childCount; ++i){
            tmps[i] = GameObject.Instantiate<GameObject>(root.transform.GetChild(i).gameObject);
        }
        for(int i=0; i<tmps.Length; ++i){
            tmps[i].transform.SetParent(root.transform);
            Vector3 p = root.transform.GetChild(i).transform.localPosition;
            p.x += offset.x;
            p.y += offset.y;
            tmps[i].transform.localScale = Vector3.one;
            tmps[i].transform.localPosition = p;
        }
    }

    void Start()
    {
        for(int i = 0; i < 4; ++i) {
            Vector2 offset = new Vector2(5.0f, 5.0f) * (i+1);
            duplicate(offset, image0_);
            duplicate(offset, image1_);
        }
        change(0);
    }

	void OnGUI()
    {
        string text = (mode_ == 0)? "Default Image" : "Polygonal Image";
        if(GUI.Button(new Rect(10, 10, 100, 40), text)){
            mode_ = (mode_ == 0)? 1 : 0;
            change(mode_);
        }
	}
}
