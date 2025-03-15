using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneController : MonoBehaviour
{
    public float walkSpeed = 1.5f;
    public Transform asylumDoor;
    public Light flashlight;
    public AudioSource thunderSound, whisperSound;

    private bool hasFlickered = false;
    private MonoBehaviour mouseLookScript; // Store the script reference

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        yield return new WaitForSeconds(1f);

        while (Vector3.Distance(transform.position, asylumDoor.position) > 2f)
        {
            transform.position += transform.forward * walkSpeed * Time.deltaTime;

            Camera.main.transform.localRotation = Quaternion.Euler(
                Mathf.Sin(Time.time * 2f) * 1f,
                Mathf.Sin(Time.time * 2f) * 2f,
                0f
            );

            if (Random.Range(0, 100) < 3)
            {
                thunderSound.Play();
            }

            if (!hasFlickered && Vector3.Distance(transform.position, asylumDoor.position) < 8f)
            {
                hasFlickered = true;
                StartCoroutine(FlashlightFlicker());
            }

            yield return null;
        }

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainGameScene");
    }

    IEnumerator FlashlightFlicker()
    {
        for (int i = 0; i < 3; i++)
        {
            flashlight.enabled = false;
            yield return new WaitForSeconds(0.1f);
            flashlight.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainGameScene");
        }
    }

    void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }
}
