using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Animation))]
public class TitleCard : MonoBehaviour
{
    [SerializeField]
    float GameID = -1f;
    [SerializeField]
    Sprite BadLogo;
    // Start is called before the first frame update
    void Start()
    {
        ServerStuff.Server.introStart.AddListener(TryStartIntro);
    }

    void TryStartIntro(int id)
    {
        if(id == GameID)
        {
            if(Random.Range(0, 5) == 0)
            {
                GetComponent<Image>().sprite = BadLogo;
            }
            GetComponent<Animation>().Play();
        }
    }
}
