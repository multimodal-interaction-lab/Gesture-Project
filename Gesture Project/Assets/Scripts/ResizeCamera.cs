using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class ResizeCamera : MonoBehaviour
{

    public enum CameraView
    {
        Free = 0,
        Square
    }

    [SerializeField]
    CameraView cameraView = CameraView.Square;
    [SerializeField]
    bool center = true;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float scale = 1.0f;
    [SerializeField]
    bool runOnlyOnce = false;

    [SerializeField]
    float targetSize;

    // Internal
    float _cachedHeight = 0.0f;
    float _cachedWidth = 0.0f;
    Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
        this.CheckScreenType();
    }

    void Update()
    {
        if (!this.runOnlyOnce)
        {
            this.CheckScreenType();
        }
    }

    void CheckScreenType()
    {
        switch (this.cameraView)
        {
            case CameraView.Square:
                this.SetSquare();
                break;
            case CameraView.Free:
                {
                    _camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Gets the size of the screen.
    /// </summary>
    void RefreshScreenSize()
    {
        this._cachedHeight = Screen.height;
        this._cachedWidth = Screen.width;
    }

    /// <summary>
    /// Sets the square.
    /// </summary>
    void SetSquare()
    {
        this.RefreshScreenSize();
        if (this._cachedHeight < this._cachedWidth)
        {
            float ratio = this._cachedHeight / this._cachedWidth;

            _camera.rect = new Rect(_camera.rect.x, _camera.rect.y, ratio * targetSize, targetSize);

            if (this.center == true)
            {
                _camera.rect = new Rect(((1.0f - ratio * this.scale) / 2), _camera.rect.y * this.scale, _camera.rect.width * this.scale, _camera.rect.height * this.scale);
            }
        }
        else
        {
            float ratio = this._cachedWidth / this._cachedHeight;

            _camera.rect = new Rect(_camera.rect.x, _camera.rect.y, targetSize, ratio * targetSize);

            if (this.center == true)
            {
                _camera.rect = new Rect(_camera.rect.x, (1.0f - ratio) / 2, _camera.rect.width, _camera.rect.height);
            }
        }
    }

    public void ScrictView(CameraView cameraView)
    {
        this.cameraView = cameraView;
    }
}