using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    [Header("Cursor Settings")]
    public RawImage cursorImage;  // The normal cursor RawImage
    public RawImage bonusCursorImage;  // The bonus game cursor RawImage (new)

    public float scale = 2.5f;
    public float rotationSpeed = 0f;

    public Vector2 normalCursorOffset = new Vector2(2f, -22f);
    public Vector2 bonusGameCursorOffset = new Vector2(0f, 0f);

    private Vector2 currentOffset;  // Track the current offset
    private RawImage currentCursor;  // Track which cursor to use (normal or bonus)

    void Start()
    {
        Cursor.visible = false;  // Hide the default system cursor
        Cursor.lockState = CursorLockMode.Confined;  // Lock cursor to the game window

        cursorImage.rectTransform.localScale = Vector3.one * scale;  // Set the normal cursor scale
        bonusCursorImage.rectTransform.localScale = Vector3.one * scale;  // Set the bonus cursor scale

        // Set the initial cursor to be the normal one
        currentCursor = cursorImage;
        currentOffset = normalCursorOffset;
        cursorImage.gameObject.SetActive(true);  // Make sure normal cursor is visible
        bonusCursorImage.gameObject.SetActive(false);  // Hide the bonus cursor initially
    }

    void Update()
    {
        // Use the active cursor and move it based on the current offset
        Vector2 targetPos = (Vector2)Input.mousePosition + currentOffset;
        currentCursor.rectTransform.position = targetPos;

        // Handle rotation (optional)
        if (rotationSpeed != 0f)
        {
            currentCursor.rectTransform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
    }

    // Method to switch between normal and bonus cursors
    public void SetBonusCursor(bool isBonusActive)
    {
        if (isBonusActive)
        {
            // Activate the bonus cursor and set its offset
            currentCursor = bonusCursorImage;
            bonusCursorImage.gameObject.SetActive(true);
            cursorImage.gameObject.SetActive(false);  // Hide normal cursor
            currentOffset = bonusGameCursorOffset;
        }
        else
        {
            // Revert back to the normal cursor and set its offset
            currentCursor = cursorImage;
            cursorImage.gameObject.SetActive(true);
            bonusCursorImage.gameObject.SetActive(false);  // Hide bonus cursor
            currentOffset = normalCursorOffset;
        }
    }
}
