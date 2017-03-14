using UnityEngine;
using System.Collections;

public class DestroyOnNonMobile : MonoBehaviour
{
	void Start ()
    {
        #if !UNITY_ANDROID && !UNITY_IPHONE && !UNITY_BLACKBERRY && !UNITY_WINRT
        Destroy(this.gameObject);
        #endif
    }
}
