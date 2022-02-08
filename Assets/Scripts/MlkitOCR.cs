using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Linq;

public class MlkitOCR : MonoBehaviour
{
    [SerializeField] private Texture2D imageToRecognize;
    [SerializeField] private Text displayText;
    private string _text = "";
 
    WebCamTexture camTexture;
    public RawImage cameraViewImage;

    public void CameraOn()
    {
        ClearTextDisplay();

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.Log("no camera!");
            return;
        }

        WebCamDevice[] devices = WebCamTexture.devices;
        int selectedCameraIndex = -1;

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == false)
            {
                selectedCameraIndex = i;
                break;
            }
        }
        if (selectedCameraIndex >= 0)
        {
            camTexture = new WebCamTexture(devices[selectedCameraIndex].name);
            camTexture.requestedFPS = 30;
            cameraViewImage.texture = camTexture;
            camTexture.Play();
        }
    }

    public void clickCapture()
    {   
        ClearTextDisplay();
        
        Texture2D texture = new Texture2D(cameraViewImage.texture.width, cameraViewImage.texture.height, TextureFormat.ARGB32, false);
        texture.SetPixels(camTexture.GetPixels());
        texture.Apply();
        byte[] bitmap_ = texture.EncodeToPNG();

        var plugin = new AndroidJavaClass("com.example.ocr.PluginClass");
        _text = plugin.CallStatic<string>("runTextRecognition", bitmap_);

        displayText.text = _text;
        Destroy(texture);
    }

    public void CameraOff()
    {
        ClearTextDisplay();

        if (camTexture != null)
        {
            camTexture.Stop();
            WebCamTexture.Destroy(camTexture);
            camTexture = null;
        }
    }

    public void ClearTextDisplay()
    {
        _text = "";
        displayText.text = _text;
    }
}
