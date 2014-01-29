using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player : MonoBehaviour
{
    public enum PowerTypes 
    {
        redLight = 0,
        infinateSonar = 1,
        stunBlast = 2,
        invisibility = 3,
        projectileThrow = 4
    }

    #region Variables
    public static float playerScore = 0;

    public GUISkin guiSkin;
    public float playerAcceleration = 3.0f;
    public float playerRotation = 2.0f;
    public float topSpeed = 5.0f;
    public float health = 3;
    public GameObject projectile;
    public AudioClip[] movmentClips;
    public AudioClip[] bloodSounds;
    public AudioClip[] burpSounds;
    public AudioClip belchSound;
    public AudioClip[] Screams;
    public AudioClip Cross;
    public AudioClip HolyWater;
    public AudioClip Sun;
    public AudioClip invinsibilitySound, ultraSonarSound, stunSound;
    
    public Sonar sonar;

    [System.NonSerialized]
    public List<PowerTypes> inventory = new List<PowerTypes>();
    private int powerActiveValue = 0;
    private bool infinateSonar = false;
    [System.NonSerialized]
    public bool stunBlast = false;
    [System.NonSerialized]
    public bool invisibility = false;
    [System.NonSerialized]
    public bool redBeam = false;
    
    private float bloodAudioTimer = 0;
    private int bloodAudioPlay = 0;
    private int playersAlive = 0;
    private bool activeRechargeMode = false;
    #endregion

    #region Power Up IEnumerators

    private IEnumerator infinateSonarFinish(float t)
    {
        yield return new WaitForSeconds(t);
        infinateSonar = false;
    }

    private IEnumerator stunBlastFinish(float t)
    {
        yield return new WaitForSeconds(t);
        stunBlast = false;
    }

    private IEnumerator invisibilityFinish(float t)
    {
        yield return new WaitForSeconds(t);
        invisibility = false;
    }

    #endregion

    public IEnumerator FinishGame(float time)
    {
        yield return new WaitForSeconds(time);
        Application.LoadLevel("end");
    }

    private void Start()
    {
        inventory.Add(PowerTypes.redLight);

        if (Network.peerType == NetworkPeerType.Server)
            playersAlive = Network.connections.Length;

        StartCoroutine(MovmentSound());
        StartCoroutine(DoSonarSound(0));
    }

    private void OnGUI()
    {
        GUI.skin = guiSkin;

        if (!activeRechargeMode)
            GUI.Box(new Rect(5, Screen.height - 35, sonar.sonarBattery * 2, 30), "");
        else
            GUI.Box(new Rect(5, Screen.height - 35, sonar.sonarBattery * 2, 30), "", guiSkin.customStyles[0]);

        switch (inventory[powerActiveValue])
        {
            case PowerTypes.redLight:
                GUI.Label(new Rect(Screen.width - 205, 35, 200, 30), "Red Light");
                break;
            case PowerTypes.infinateSonar:
                GUI.Label(new Rect(Screen.width - 205, 35, 200, 30), "Ultimate Sonar");
                break;
            case PowerTypes.stunBlast:
                GUI.Label(new Rect(Screen.width - 205, 35, 200, 30), "Stun Blast");
                break;
            case PowerTypes.invisibility:
                GUI.Label(new Rect(Screen.width - 205, 35, 200, 30), "Invinsibility");
                break;
            case PowerTypes.projectileThrow:
                GUI.Label(new Rect(Screen.width - 205, 35, 200, 30), "Projectile");
                break;
        }

        GUILayout.Label("Health: " + health);
        GUILayout.Label("Score: " + playerScore);
    }

    private void Update()
    {
        if (Network.peerType == NetworkPeerType.Disconnected || networkView.isMine)
        {
            #region Movment

            if (bloodAudioPlay != 5)
            {
                Vector3 userInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                userInput *= playerAcceleration;

                if (rigidbody.velocity.magnitude < topSpeed)
                    rigidbody.AddForce(userInput * 10, ForceMode.Force);
            }

            #endregion

            #region Rotation

            Vector3 mousePosition = transform.position + transform.forward;
            Ray ray = Camera.main.camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
                mousePosition = hitInfo.point;

            Vector2 mouseRelation = new Vector2(transform.position.x - mousePosition.x, transform.position.z - mousePosition.z);
            float rotationAngle = 0;
            rotationAngle = Mathf.Atan(-mouseRelation.y / mouseRelation.x) * 180 / Mathf.PI;

            rotationAngle += 270;
            if (mouseRelation.x < 0)
                rotationAngle += 180;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(rotationAngle, transform.up), Time.deltaTime * playerRotation);

            #endregion

            #region Audio Managment

            if (bloodAudioTimer > 0)
                bloodAudioTimer -= Time.deltaTime;
            else if (bloodAudioPlay != 0)
            {
                if (bloodAudioPlay == 5)
                    StartCoroutine(Burp(true));
                else
                    StartCoroutine(Burp(false));

                bloodAudioPlay = 0;
            }

            #endregion

            #region Sonar Controls

            if (Input.GetButtonDown("Fire1") && !Input.GetButton("Fire2") && sonar.sonarBattery > 3 && !activeRechargeMode)
            {
                sonar.pulser = 0;
                sonar.useSonar = true;

                if (sonar.sonarBattery <= 4)
                    activeRechargeMode = true;
            }

            if (Input.GetButtonUp("Fire1"))
                sonar.useSonar = false;

            #endregion

            #region Powerups

            if (inventory[powerActiveValue] == PowerTypes.redLight)
            {
                if (Input.GetButton("Fire2") && !sonar.useSonar && sonar.sonarBattery > 3 && !activeRechargeMode)
                {
                    sonar.attack.audio.volume = 0.8f;
                    redBeam = true;
                    sonar.attack.light.intensity += Time.deltaTime * 2;
                    sonar.sonarBattery -= Time.deltaTime * sonar.batteryUsage * 3;
                    if (sonar.sonarBattery <= 4)
                        activeRechargeMode = true;
                }
                else
                {
                    sonar.attack.audio.volume = 0;
                    redBeam = false;
                    sonar.attack.light.intensity -= Time.deltaTime * 3;
                }
            }
            else if (inventory[powerActiveValue] == PowerTypes.invisibility)
            {
                redBeam = false;
                if (Input.GetButtonDown("Fire2"))
                {
                    invisibility = true;
                    StartCoroutine(invisibilityFinish(5.0f));
                    StartCoroutine(createSound(invinsibilitySound));

                    inventory.Remove(PowerTypes.invisibility);
                    powerActiveValue = 0;
                }
            }
            else if (inventory[powerActiveValue] == PowerTypes.infinateSonar)
            {
                redBeam = false;
                if (Input.GetButtonDown("Fire2"))
                {
                    infinateSonar = true;
                    StartCoroutine(infinateSonarFinish(5.0f));
                    StartCoroutine(createSound(ultraSonarSound));

                    inventory.Remove(PowerTypes.infinateSonar);
                    powerActiveValue = 0;
                }
            }
            else if (inventory[powerActiveValue] == PowerTypes.stunBlast)
            {
                redBeam = false;
                if (Input.GetButtonDown("Fire2"))
                {
                    stunBlast = true;
                    StartCoroutine(stunBlastFinish(5.0f));
                    StartCoroutine(createSound(stunSound));

                    inventory.Remove(PowerTypes.stunBlast);
                    powerActiveValue = 0;
                }
            }
            else if (inventory[powerActiveValue] == PowerTypes.projectileThrow)
            {
                redBeam = false;
                if (Input.GetButtonDown("Fire2"))
                {
                    GameObject p = (GameObject)Instantiate(projectile, transform.position + (transform.forward * 2), transform.rotation);
                    p.rigidbody.AddForce(p.transform.forward * 500, ForceMode.Acceleration);

                    inventory.Remove(PowerTypes.projectileThrow);
                    powerActiveValue = 0;
                }
            }
            else
                powerActiveValue = 0;

            #endregion

            if (Input.GetKeyDown("space"))
            {
                powerActiveValue++;
                if (powerActiveValue >= inventory.Count)
                    powerActiveValue = 0;
            }
            else if (Input.GetKeyDown("backspace"))
            {
                powerActiveValue--;
                if (powerActiveValue < 0)
                    powerActiveValue = inventory.Count - 1;
            }
        }
        
        #region Sonar

        if (infinateSonar)
        {
            sonar.pulser += Time.deltaTime;
            if (sonar.pulser >= 360)
                sonar.pulser = 0;
            sonar.pulse.light.intensity = Mathf.Sin(sonar.pulser * sonar.pulseFrequency * 5) * sonar.pulseIntensity * 5;
            sonar.bouncingLight.light.intensity = Mathf.Sin(-sonar.pulser * sonar.bounceFrequency * 5) * sonar.bounceIntensity * 5;
        }
        else if (sonar.useSonar)
        {
            sonar.pulser += Time.deltaTime;
            if (sonar.pulser >= 360)
                sonar.pulser = 0;
            sonar.pulse.light.intensity = Mathf.Sin(sonar.pulser * sonar.pulseFrequency) * sonar.pulseIntensity;
            sonar.bouncingLight.light.intensity = Mathf.Sin(-sonar.pulser * sonar.bounceFrequency) * sonar.bounceIntensity;

            if (sonar.sonarBattery > 0 && !infinateSonar)
                sonar.sonarBattery -= Time.deltaTime * sonar.batteryUsage * 0.5f;
            if (sonar.sonarBattery <= 4)
                activeRechargeMode = true;
        }
        else
        {
            sonar.pulse.light.intensity = Mathf.Lerp(sonar.pulse.light.intensity, 0, Time.deltaTime * sonar.fadeTime);
            sonar.bouncingLight.light.intensity = Mathf.Lerp(sonar.bouncingLight.light.intensity, 0, Time.deltaTime * sonar.fadeTime);

            if (sonar.sonarBattery < 100)
            {
                sonar.sonarBattery += Time.deltaTime * sonar.batteryRecovery;
                if (sonar.sonarBattery > 80 && activeRechargeMode)
                    activeRechargeMode = false;
            }
        }

        #endregion

        if (health <= 0)
            Application.LoadLevel(Application.loadedLevel);
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.collider.gameObject.tag)
        {
            case "BloodDroplet":
                playerScore += 100;

                Destroy(collision.collider.gameObject);

                if (bloodAudioPlay < bloodSounds.Length)
                {
                    if (bloodAudioTimer <= 0)
                        bloodAudioPlay = 0;

                    bloodAudioTimer = 1.5f;
                    StartCoroutine(createSound(bloodSounds[bloodAudioPlay], 0.85f));
                    bloodAudioPlay++;
                }
                break;
            case "HolyWater":
                if (!invisibility)
                    health -= 1;

                Destroy(collision.collider.gameObject);
                if (health <= 0)
                {
                    if (Network.peerType == NetworkPeerType.Disconnected)
                        Application.LoadLevel(Application.loadedLevel);
                    else
                    {
                        if (Network.peerType == NetworkPeerType.Server)
                            playerDied();
                        else
                            networkView.RPC("playerDied", RPCMode.Server);
                    }
                }

                StartCoroutine(screamCreator(HolyWater));
                break;
            case "Spikes":
                health--;
                StartCoroutine(createSound(Screams[Random.Range(0, Screams.Length)]));
                break;
        }
    }

    #region Audio Methods

    public IEnumerator screamCreator(AudioClip clip)
    {
        StartCoroutine(createSound(clip));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(createSound(Screams[Random.Range(0, Screams.Length)], 0.6f));
    }

    public IEnumerator createSound(AudioClip clip, float audioLevel = 1)
    {
        GameObject g = new GameObject();
        g.AddComponent<AudioSource>();
        g.audio.pitch = Random.Range(0.95f, 1.05f);
        g.audio.volume = audioLevel;
        g.audio.clip = clip;
        g.audio.Play();

        yield return new WaitForSeconds(clip.length);

        Destroy(g);
    }

    private IEnumerable waitAndDestroy(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(obj);
    }

    private IEnumerator Burp(bool belch)
    {
        GameObject g = new GameObject();
        g.AddComponent<AudioSource>();
        g.transform.position = Vector3.zero;
        if (belch)
        {
            g.audio.clip = belchSound;
            g.audio.pitch = Random.Range(0.95f, 1.05f);
            g.audio.volume = 0.75f;
            g.audio.Play();
            yield return new WaitForSeconds(belchSound.length + 0.6f);
        }
        g.audio.clip = burpSounds[Random.Range(0, burpSounds.Length)];
        g.audio.pitch = Random.Range(0.95f, 1.05f);
        g.audio.volume = 0.75f;
        g.audio.Play();
        yield return new WaitForSeconds(g.audio.clip.length);
        Destroy(g);
    }

    private IEnumerator DoSonarSound(int x)
    {
        if (sonar.useSonar)
        {
            if (x == 1)
            {
                sonar.pulse.audio.volume = 0.5f;
                sonar.pulse.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Cave;
                x = 0;
            }
            else
            {
                sonar.pulse.audio.volume = 1.0f;
                sonar.pulse.GetComponent<AudioReverbZone>().reverbPreset = AudioReverbPreset.Off;
                x = 1;
            }
            sonar.pulse.audio.Play();
            yield return new WaitForSeconds(sonar.pulse.audio.clip.length - 0.3f);
            StartCoroutine(DoSonarSound(x));
        }
        else
        {
            yield return new WaitForEndOfFrame();
            StartCoroutine(DoSonarSound(0));
        }
    }

    private IEnumerator MovmentSound()
    {
        if (rigidbody.velocity.magnitude > 1)
        {
            switch (Random.Range(0, 4))
            {
                case 0:
                case 1:
                    audio.clip = movmentClips[0];
                    break;
                case 2:
                    audio.clip = movmentClips[1];
                    break;
                case 3:
                    audio.clip = movmentClips[2];
                    break;
                case 4:
                    audio.clip = movmentClips[3];
                    break;
            }
            audio.Play();
            yield return new WaitForSeconds(Random.Range(audio.clip.length + 0.7f, audio.clip.length + 0.9f));
        }
        else
            yield return new WaitForEndOfFrame();

        StartCoroutine(MovmentSound());
    }

    #endregion

    #region RPC Network Messages

    [RPC]
    private void playerDied()
    {
        playersAlive--;
        if (playersAlive == 0)
        {
            networkView.RPC("restartLevel", RPCMode.AllBuffered);
        }
    }

    [RPC]
    private void restartLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    #endregion

    #region Server Methods

    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("Clean up after player " + player);
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }

    private void OnPlayerConnected(NetworkPlayer player)
    {
        playersAlive++;
    }

    #endregion
}

[System.Serializable]
public class Sonar
{
    public GameObject attack;
    public GameObject pulse;
    public GameObject bouncingLight;
    public float pulseFrequency;
    public float pulseIntensity;
    public float bounceFrequency;
    public float bounceIntensity;
    public float fadeTime;
    public float batteryUsage;
    public float batteryRecovery;


    [System.NonSerialized]
    public float sonarBattery = 100.0f;
    [System.NonSerialized]
    public bool useSonar = false;
    [System.NonSerialized]
    public float pulser = 0;
}