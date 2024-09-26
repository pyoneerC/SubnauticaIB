// CHANGE LOG
// 
// CHANGES || version VERSION
//
// "Enable/Disable Headbob, Changed look rotations - should result in reduced camera jitters" || version 1.0.1

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class FirstPersonController : MonoBehaviour
{
    private Rigidbody _rb;

    #region Camera Movement Variables

    public Camera playerCamera;

    public float fov = 60f;
    public bool invertCamera;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    // Crosshair
    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    // Internal Variables
    private float _yaw;
    private float _pitch;
    private Image _crosshairObject;

    #region Camera Zoom Variables

    public bool enableZoom = true;
    public bool holdToZoom;
    public KeyCode zoomKey = KeyCode.Mouse1;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    // Internal Variables
    private bool _isZoomed;

    #endregion
    #endregion

    #region Movement Variables

    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    // Internal Variables
    private bool _isWalking;

    #region Sprint

    public bool enableSprint = true;
    public bool unlimitedSprint;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float sprintSpeed = 7f;
    public float sprintDuration = 5f;
    public float sprintCooldown = .5f;
    public float sprintFOV = 80f;
    public float sprintFOVStepTime = 10f;

    // Sprint Bar
    public bool useSprintBar = true;
    public bool hideBarWhenFull = true;
    [FormerlySerializedAs("sprintBarBG")] public Image sprintBarBg;
    public Image sprintBar;
    public float sprintBarWidthPercent = .3f;
    public float sprintBarHeightPercent = .015f;

    // Internal Variables
    private CanvasGroup _sprintBarCg;
    private bool _isSprinting;
    private float _sprintRemaining;
    private float _sprintBarWidth;
    private float _sprintBarHeight;
    private bool _isSprintCooldown;
    private float _sprintCooldownReset;

    #endregion

    #region Jump

    public bool enableJump = true;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;

    // Internal Variables
    private bool _isGrounded;

    #endregion

    #region Crouch

    public bool enableCrouch = true;
    public bool holdToCrouch = true;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float crouchHeight = .75f;
    public float speedReduction = .5f;

    // Internal Variables
    private bool _isCrouched;
    private Vector3 _originalScale;

    #endregion
    #endregion

    #region Head Bob

    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 5f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    // Internal Variables
    private Vector3 _jointOriginalPos;
    private float _timer;

    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _crosshairObject = GetComponentInChildren<Image>();

        // Set internal variables
        playerCamera.fieldOfView = fov;
        _originalScale = transform.localScale;
        _jointOriginalPos = joint.localPosition;

        if (!unlimitedSprint)
        {
            _sprintRemaining = sprintDuration;
            _sprintCooldownReset = sprintCooldown;
        }
    }

    void Start()
    {
        if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if(crosshair)
        {
            _crosshairObject.sprite = crosshairImage;
            _crosshairObject.color = crosshairColor;
        }
        else
        {
            _crosshairObject.gameObject.SetActive(false);
        }

        #region Sprint Bar

        _sprintBarCg = GetComponentInChildren<CanvasGroup>();

        if(useSprintBar)
        {
            sprintBarBg.gameObject.SetActive(true);
            sprintBar.gameObject.SetActive(true);

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            _sprintBarWidth = screenWidth * sprintBarWidthPercent;
            _sprintBarHeight = screenHeight * sprintBarHeightPercent;

            sprintBarBg.rectTransform.sizeDelta = new Vector3(_sprintBarWidth, _sprintBarHeight, 0f);
            sprintBar.rectTransform.sizeDelta = new Vector3(_sprintBarWidth - 2, _sprintBarHeight - 2, 0f);

            if(hideBarWhenFull)
            {
                _sprintBarCg.alpha = 0;
            }
        }
        else
        {
            sprintBarBg.gameObject.SetActive(false);
            sprintBar.gameObject.SetActive(false);
        }

        #endregion
    }

    float _camRotation;

    private void Update()
    {
        #region Camera

        // Control camera movement
        if(cameraCanMove)
        {
            _yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;

            if (!invertCamera)
            {
                _pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
            }
            else
            {
                // Inverted Y
                _pitch += mouseSensitivity * Input.GetAxis("Mouse Y");
            }

            // Clamp pitch between lookAngle
            _pitch = Mathf.Clamp(_pitch, -maxLookAngle, maxLookAngle);

            transform.localEulerAngles = new Vector3(0, _yaw, 0);
            playerCamera.transform.localEulerAngles = new Vector3(_pitch, 0, 0);
        }

        #region Camera Zoom

        if (enableZoom)
        {
            // Changes isZoomed when key is pressed
            // Behavior for toogle zoom
            if(Input.GetKeyDown(zoomKey) && !holdToZoom && !_isSprinting)
            {
                _isZoomed = !_isZoomed;
            }

            // Changes isZoomed when key is pressed
            // Behavior for hold to zoom
            if(holdToZoom && !_isSprinting)
            {
                if(Input.GetKeyDown(zoomKey))
                {
                    _isZoomed = true;
                }
                else if(Input.GetKeyUp(zoomKey))
                {
                    _isZoomed = false;
                }
            }

            // Lerps camera.fieldOfView to allow for a smooth transistion
            if(_isZoomed)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, zoomStepTime * Time.deltaTime);
            }
            else if(!_isZoomed && !_isSprinting)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, zoomStepTime * Time.deltaTime);
            }
        }

        #endregion
        #endregion

        #region Sprint

        if(enableSprint)
        {
            if(_isSprinting)
            {
                _isZoomed = false;
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);

                // Drain sprint remaining while sprinting
                if(!unlimitedSprint)
                {
                    _sprintRemaining -= 1 * Time.deltaTime;
                    if (_sprintRemaining <= 0)
                    {
                        _isSprinting = false;
                        _isSprintCooldown = true;
                    }
                }
            }
            else
            {
                // Regain sprint while not sprinting
                _sprintRemaining = Mathf.Clamp(_sprintRemaining += 1 * Time.deltaTime, 0, sprintDuration);
            }

            // Handles sprint cooldown 
            // When sprint remaining == 0 stops sprint ability until hitting cooldown
            if(_isSprintCooldown)
            {
                sprintCooldown -= 1 * Time.deltaTime;
                if (sprintCooldown <= 0)
                {
                    _isSprintCooldown = false;
                }
            }
            else
            {
                sprintCooldown = _sprintCooldownReset;
            }

            // Handles sprintBar 
            if(useSprintBar && !unlimitedSprint)
            {
                float sprintRemainingPercent = _sprintRemaining / sprintDuration;
                sprintBar.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
            }
        }

        #endregion

        #region Jump

        // Gets input and calls jump method
        if(enableJump && Input.GetKeyDown(jumpKey) && _isGrounded)
        {
            Jump();
        }

        #endregion

        #region Crouch

        if (enableCrouch)
        {
            if(Input.GetKeyDown(crouchKey) && !holdToCrouch)
            {
                Crouch();
            }
            
            if(Input.GetKeyDown(crouchKey) && holdToCrouch)
            {
                _isCrouched = false;
                Crouch();
            }
            else if(Input.GetKeyUp(crouchKey) && holdToCrouch)
            {
                _isCrouched = true;
                Crouch();
            }
        }

        #endregion

        CheckGround();

        if(enableHeadBob)
        {
            HeadBob();
        }
    }

    void FixedUpdate()
    {
        #region Movement

        if (playerCanMove)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // Checks if player is walking and isGrounded
            // Will allow head bob
            if (targetVelocity.x != 0 || targetVelocity.z != 0 && _isGrounded)
            {
                _isWalking = true;
            }
            else
            {
                _isWalking = false;
            }

            // All movement calculations shile sprint is active
            if (enableSprint && Input.GetKey(sprintKey) && _sprintRemaining > 0f && !_isSprintCooldown)
            {
                targetVelocity = transform.TransformDirection(targetVelocity) * sprintSpeed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = _rb.velocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                // Player is only moving when valocity change != 0
                // Makes sure fov change only happens during movement
                if (velocityChange.x != 0 || velocityChange.z != 0)
                {
                    _isSprinting = true;

                    if (_isCrouched)
                    {
                        Crouch();
                    }

                    if (hideBarWhenFull && !unlimitedSprint)
                    {
                        _sprintBarCg.alpha += 5 * Time.deltaTime;
                    }
                }

                _rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }
            // All movement calculations while walking
            else
            {
                _isSprinting = false;

                if (hideBarWhenFull && Mathf.Approximately(_sprintRemaining, sprintDuration))
                {
                    //_sprintBarCg.alpha -= 3 * Time.deltaTime;
                }

                targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = _rb.velocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                _rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }
        }

        #endregion
    }

    // Sets isGrounded based on a raycast sent straigth down from the player object
    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = .75f;

        if (Physics.Raycast(origin, direction, out _, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }

    private void Jump()
    {
        // Adds force to the player rigidbody to jump
        if (_isGrounded)
        {
            _rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
            _isGrounded = false;
        }

        // When crouched and using toggle system, will uncrouch for a jump
        if(_isCrouched && !holdToCrouch)
        {
            Crouch();
        }
    }

    private void Crouch()
    {
        // Stand player up to full height
        // Brings walkSpeed back up to original speed
        if(_isCrouched)
        {
            transform.localScale = new Vector3(_originalScale.x, _originalScale.y, _originalScale.z);
            walkSpeed /= speedReduction;

            _isCrouched = false;
        }
        // Crouches player down to set height
        // Reduces walkSpeed
        else
        {
            transform.localScale = new Vector3(_originalScale.x, crouchHeight, _originalScale.z);
            walkSpeed *= speedReduction;

            _isCrouched = true;
        }
    }

    private void HeadBob()
    {
        if(_isWalking)
        {
            // Calculates HeadBob speed during sprint
            if(_isSprinting)
            {
                _timer += Time.deltaTime * (bobSpeed + sprintSpeed);
            }
            // Calculates HeadBob speed during crouched movement
            else if (_isCrouched)
            {
                _timer += Time.deltaTime * (bobSpeed * speedReduction);
            }
            // Calculates HeadBob speed during walking
            else
            {
                _timer += Time.deltaTime * bobSpeed;
            }
            // Applies HeadBob movement
            joint.localPosition = new Vector3(_jointOriginalPos.x + Mathf.Sin(_timer) * bobAmount.x, _jointOriginalPos.y + Mathf.Sin(_timer) * bobAmount.y, _jointOriginalPos.z + Mathf.Sin(_timer) * bobAmount.z);
        }
        else
        {
            // Resets when play stops moving
            _timer = 0;
            joint.localPosition = new Vector3(Mathf.Lerp(joint.localPosition.x, _jointOriginalPos.x, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.y, _jointOriginalPos.y, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.z, _jointOriginalPos.z, Time.deltaTime * bobSpeed));
        }
    }
}



// Custom Editor
#if UNITY_EDITOR
    [CustomEditor(typeof(FirstPersonController)), InitializeOnLoadAttribute]
    public class FirstPersonControllerEditor : Editor
    {
    FirstPersonController _fpc;
    SerializedObject _serFpc;

    private void OnEnable()
    {
        _fpc = (FirstPersonController)target;
        _serFpc = new SerializedObject(_fpc);
    }

    public override void OnInspectorGUI()
    {
        _serFpc.Update();

        EditorGUILayout.Space();
        GUILayout.Label("Modular First Person Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
        GUILayout.Label("By Jess Case", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        GUILayout.Label("version 1.0.1", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        EditorGUILayout.Space();

        #region Camera Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Camera Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        _fpc.playerCamera = (Camera)EditorGUILayout.ObjectField(new GUIContent("Camera", "Camera attached to the controller."), _fpc.playerCamera, typeof(Camera), true);
        _fpc.fov = EditorGUILayout.Slider(new GUIContent("Field of View", "The camera’s view angle. Changes the player camera directly."), _fpc.fov, _fpc.zoomFOV, 179f);
        _fpc.cameraCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Camera Rotation", "Determines if the camera is allowed to move."), _fpc.cameraCanMove);

        GUI.enabled = _fpc.cameraCanMove;
        _fpc.invertCamera = EditorGUILayout.ToggleLeft(new GUIContent("Invert Camera Rotation", "Inverts the up and down movement of the camera."), _fpc.invertCamera);
        _fpc.mouseSensitivity = EditorGUILayout.Slider(new GUIContent("Look Sensitivity", "Determines how sensitive the mouse movement is."), _fpc.mouseSensitivity, .1f, 10f);
        _fpc.maxLookAngle = EditorGUILayout.Slider(new GUIContent("Max Look Angle", "Determines the max and min angle the player camera is able to look."), _fpc.maxLookAngle, 40, 90);
        GUI.enabled = true;

        _fpc.lockCursor = EditorGUILayout.ToggleLeft(new GUIContent("Lock and Hide Cursor", "Turns off the cursor visibility and locks it to the middle of the screen."), _fpc.lockCursor);

        _fpc.crosshair = EditorGUILayout.ToggleLeft(new GUIContent("Auto Crosshair", "Determines if the basic crosshair will be turned on, and sets is to the center of the screen."), _fpc.crosshair);

        // Only displays crosshair options if crosshair is enabled
        if(_fpc.crosshair)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Crosshair Image", "Sprite to use as the crosshair."));
            _fpc.crosshairImage = (Sprite)EditorGUILayout.ObjectField(_fpc.crosshairImage, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _fpc.crosshairColor = EditorGUILayout.ColorField(new GUIContent("Crosshair Color", "Determines the color of the crosshair."), _fpc.crosshairColor);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        #region Camera Zoom Setup

        GUILayout.Label("Zoom", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        _fpc.enableZoom = EditorGUILayout.ToggleLeft(new GUIContent("Enable Zoom", "Determines if the player is able to zoom in while playing."), _fpc.enableZoom);

        GUI.enabled = _fpc.enableZoom;
        _fpc.holdToZoom = EditorGUILayout.ToggleLeft(new GUIContent("Hold to Zoom", "Requires the player to hold the zoom key instead if pressing to zoom and unzoom."), _fpc.holdToZoom);
        _fpc.zoomKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Zoom Key", "Determines what key is used to zoom."), _fpc.zoomKey);
        _fpc.zoomFOV = EditorGUILayout.Slider(new GUIContent("Zoom FOV", "Determines the field of view the camera zooms to."), _fpc.zoomFOV, .1f, _fpc.fov);
        _fpc.zoomStepTime = EditorGUILayout.Slider(new GUIContent("Step Time", "Determines how fast the FOV transitions while zooming in."), _fpc.zoomStepTime, .1f, 10f);
        GUI.enabled = true;

        #endregion

        #endregion

        #region Movement Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Movement Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        _fpc.playerCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Player Movement", "Determines if the player is allowed to move."), _fpc.playerCanMove);

        GUI.enabled = _fpc.playerCanMove;
        _fpc.walkSpeed = EditorGUILayout.Slider(new GUIContent("Walk Speed", "Determines how fast the player will move while walking."), _fpc.walkSpeed, .1f, _fpc.sprintSpeed);
        GUI.enabled = true;

        EditorGUILayout.Space();

        #region Sprint

        GUILayout.Label("Sprint", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        _fpc.enableSprint = EditorGUILayout.ToggleLeft(new GUIContent("Enable Sprint", "Determines if the player is allowed to sprint."), _fpc.enableSprint);

        GUI.enabled = _fpc.enableSprint;
        _fpc.unlimitedSprint = EditorGUILayout.ToggleLeft(new GUIContent("Unlimited Sprint", "Determines if 'Sprint Duration' is enabled. Turning this on will allow for unlimited sprint."), _fpc.unlimitedSprint);
        _fpc.sprintKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Sprint Key", "Determines what key is used to sprint."), _fpc.sprintKey);
        _fpc.sprintSpeed = EditorGUILayout.Slider(new GUIContent("Sprint Speed", "Determines how fast the player will move while sprinting."), _fpc.sprintSpeed, _fpc.walkSpeed, 20f);

        //GUI.enabled = !fpc.unlimitedSprint;
        _fpc.sprintDuration = EditorGUILayout.Slider(new GUIContent("Sprint Duration", "Determines how long the player can sprint while unlimited sprint is disabled."), _fpc.sprintDuration, 1f, 20f);
        _fpc.sprintCooldown = EditorGUILayout.Slider(new GUIContent("Sprint Cooldown", "Determines how long the recovery time is when the player runs out of sprint."), _fpc.sprintCooldown, .1f, _fpc.sprintDuration);
        //GUI.enabled = true;

        _fpc.sprintFOV = EditorGUILayout.Slider(new GUIContent("Sprint FOV", "Determines the field of view the camera changes to while sprinting."), _fpc.sprintFOV, _fpc.fov, 179f);
        _fpc.sprintFOVStepTime = EditorGUILayout.Slider(new GUIContent("Step Time", "Determines how fast the FOV transitions while sprinting."), _fpc.sprintFOVStepTime, .1f, 20f);

        _fpc.useSprintBar = EditorGUILayout.ToggleLeft(new GUIContent("Use Sprint Bar", "Determines if the default sprint bar will appear on screen."), _fpc.useSprintBar);

        // Only displays sprint bar options if sprint bar is enabled
        if(_fpc.useSprintBar)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            _fpc.hideBarWhenFull = EditorGUILayout.ToggleLeft(new GUIContent("Hide Full Bar", "Hides the sprint bar when sprint duration is full, and fades the bar in when sprinting. Disabling this will leave the bar on screen at all times when the sprint bar is enabled."), _fpc.hideBarWhenFull);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Bar BG", "Object to be used as sprint bar background."));
            _fpc.sprintBarBg = (Image)EditorGUILayout.ObjectField(_fpc.sprintBarBg, typeof(Image), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Bar", "Object to be used as sprint bar foreground."));
            _fpc.sprintBar = (Image)EditorGUILayout.ObjectField(_fpc.sprintBar, typeof(Image), true);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            _fpc.sprintBarWidthPercent = EditorGUILayout.Slider(new GUIContent("Bar Width", "Determines the width of the sprint bar."), _fpc.sprintBarWidthPercent, .1f, .5f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _fpc.sprintBarHeightPercent = EditorGUILayout.Slider(new GUIContent("Bar Height", "Determines the height of the sprint bar."), _fpc.sprintBarHeightPercent, .001f, .025f);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
        GUI.enabled = true;

        EditorGUILayout.Space();

        #endregion

        #region Jump

        GUILayout.Label("Jump", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        _fpc.enableJump = EditorGUILayout.ToggleLeft(new GUIContent("Enable Jump", "Determines if the player is allowed to jump."), _fpc.enableJump);

        GUI.enabled = _fpc.enableJump;
        _fpc.jumpKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Jump Key", "Determines what key is used to jump."), _fpc.jumpKey);
        _fpc.jumpPower = EditorGUILayout.Slider(new GUIContent("Jump Power", "Determines how high the player will jump."), _fpc.jumpPower, .1f, 20f);
        GUI.enabled = true;

        EditorGUILayout.Space();

        #endregion

        #region Crouch

        GUILayout.Label("Crouch", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        _fpc.enableCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Enable Crouch", "Determines if the player is allowed to crouch."), _fpc.enableCrouch);

        GUI.enabled = _fpc.enableCrouch;
        _fpc.holdToCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Hold To Crouch", "Requires the player to hold the crouch key instead if pressing to crouch and uncrouch."), _fpc.holdToCrouch);
        _fpc.crouchKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Crouch Key", "Determines what key is used to crouch."), _fpc.crouchKey);
        _fpc.crouchHeight = EditorGUILayout.Slider(new GUIContent("Crouch Height", "Determines the y scale of the player object when crouched."), _fpc.crouchHeight, .1f, 1);
        _fpc.speedReduction = EditorGUILayout.Slider(new GUIContent("Speed Reduction", "Determines the percent 'Walk Speed' is reduced by. 1 being no reduction, and .5 being half."), _fpc.speedReduction, .1f, 1);
        GUI.enabled = true;

        #endregion

        #endregion

        #region Head Bob

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Head Bob Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        _fpc.enableHeadBob = EditorGUILayout.ToggleLeft(new GUIContent("Enable Head Bob", "Determines if the camera will bob while the player is walking."), _fpc.enableHeadBob);


        GUI.enabled = _fpc.enableHeadBob;
        _fpc.joint = (Transform)EditorGUILayout.ObjectField(new GUIContent("Camera Joint", "Joint object position is moved while head bob is active."), _fpc.joint, typeof(Transform), true);
        _fpc.bobSpeed = EditorGUILayout.Slider(new GUIContent("Speed", "Determines how often a bob rotation is completed."), _fpc.bobSpeed, 1, 20);
        _fpc.bobAmount = EditorGUILayout.Vector3Field(new GUIContent("Bob Amount", "Determines the amount the joint moves in both directions on every axes."), _fpc.bobAmount);
        GUI.enabled = true;

        #endregion

        //Sets any changes from the prefab
        if(GUI.changed)
        {
            EditorUtility.SetDirty(_fpc);
            Undo.RecordObject(_fpc, "FPC Change");
            _serFpc.ApplyModifiedProperties();
        }
    }

}

#endif