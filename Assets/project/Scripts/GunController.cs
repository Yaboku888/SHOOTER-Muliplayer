using UnityEngine.VFX;
using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour
{
    #region Variables
    [Header("Gun")]

    public AudioClip weaponChange;
    public AudioSource audioSource;

    public Gun[] guns;
    public Gun actualGun;
    public int indexGun = 0;
    public int maxGuns = 3;

    public float lastShootTime = 0;
    public float lasReload = 0;
    public float lastChangeTime;
    public float changeTime;
    public float distance;

    public bool isChangin = false;
    public bool reloanding = false;

    Vector3 currentRotation;
    Vector3 targetRotation;
    public float returnSpeed;
    public float snappines;

    public GameObject bulletHole;
    public float offset;

    [Header("Player")]
    public PlayerController playerController;

    [Header("Scope")]
    public Animator animator;
    public GameObject scopeOverlay;
    public bool isScoped = false;
    #endregion

    #region Unity Functions
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }
    private void Update()
    {
        if (actualGun != null)
        {
            if (lastShootTime <= 0 && !reloanding && !isChangin)
            {
                if (!actualGun.data.automatic)
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        if (actualGun.data.actualAmmo > 0)
                        {
                            Shoot();
                        }
                    }
                }
                else
                {
                    if (Input.GetButton("Fire1"))
                    {
                        if (actualGun.data.actualAmmo > 0)
                        {
                            Shoot();
                        }
                    }
                }
            }

            if (Input.GetButtonDown("Reload") && !reloanding && isChangin == false)
            {
                if (actualGun.data.actualAmmo < actualGun.data.maxAmmoCount)
                {
                    actualGun.audioSource.PlayOneShot(actualGun.reload);
                    lasReload = 0;
                    reloanding = true;
                }
            }
            if (reloanding == true)
            {
                lasReload += Time.deltaTime;
                if (lasReload >= actualGun.data.reloadTime)
                {
                    reloanding = false;
                    Reload();
                }
            }
            if (lastShootTime >= 0)
            {
                lastShootTime -= Time.deltaTime;
            }

            if (Input.GetButtonDown("Drop") &&!reloanding && !isChangin)
            {
                Drop();
            }

            if (Input.GetButtonDown("Fire2") && actualGun.data.sniper == true)
            {
                isScoped = !isScoped;
                animator.SetBool("Scoped", isScoped);
                if (isScoped)
                {
                    onScoped();
                }
                else
                {
                    OnUnScoped();
                }
              
            }
        }

        if (Input.GetButtonDown("Gn1") && !reloanding)
        {
            if (indexGun != 0)
            {
                audioSource.PlayOneShot(weaponChange);
                indexGun = 0;
                lastChangeTime = 0;

                if (actualGun != null)
                {
                    actualGun.gameObject.SetActive(false);
                    actualGun = null;
                }
                isChangin = true;
            }
        }
        if (Input.GetButtonDown("Gn2") && !reloanding)
        {
            if (indexGun != 1)
            {
                audioSource.PlayOneShot(weaponChange);
                indexGun = 1;
                lastChangeTime = 0;

                if (actualGun != null)
                {
                    actualGun.gameObject.SetActive(false);
                    actualGun = null;
                }
                isChangin = true;
            }
        }
        if (Input.GetButtonDown("Gn3") && !reloanding)
        {
            if (indexGun != 2)
            {
                audioSource.PlayOneShot(weaponChange);
                indexGun = 2;
                lastChangeTime = 0;

                if (actualGun != null)
                {
                    actualGun.gameObject.SetActive(false);
                    actualGun = null;

                }
                isChangin = true;
            }
        }

        if (Input.mouseScrollDelta.y != 0 && !reloanding)
        {
            indexGun += (int)Input.mouseScrollDelta.y;

            if (indexGun >= maxGuns)
            {
                indexGun = 0;
            }
            if (indexGun < 0)
            {
                indexGun = maxGuns - 1;
            }

            lastChangeTime = 0;

            if (actualGun != null)
            {
                actualGun.gameObject.SetActive(false);
                actualGun = null;
            }
            isChangin = true;
        }

        else
        {

        }
        if (isChangin)
        {
            lastChangeTime += Time.deltaTime;
            if (lastChangeTime >= changeTime)
            {
                Weaponchange(indexGun);
                isChangin = false;
            }
        }
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappines * Time.deltaTime);
        playerController.recoil.localRotation = Quaternion.Euler(currentRotation);

        if (Physics.Raycast(playerController.cam.transform.position, playerController.cam.transform.forward, out RaycastHit hit, distance))
        {
            if (hit.transform.tag == ("IsGun"))
            {
                if (Input.GetButtonDown("Drop") && !reloanding && !isChangin)
                {
                    GetGun(hit.transform.GetComponent<Gun>());
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        Debug.DrawRay(playerController.cam.transform.position, playerController.cam.transform.forward * distance);
    }
    #endregion

    #region Custom Functions
    private void Shoot()
    {
        if (Physics.Raycast(playerController.cam.transform.position,playerController.cam.transform.forward, out RaycastHit hit, actualGun.data.range))
        {
            if (hit.transform != null)
            {
                Debug.Log($"we Shotin at:{hit.transform.name}");
                GameObject go = Instantiate(bulletHole, hit.point + hit.normal * offset, Quaternion.LookRotation(hit.normal, Vector3.up));

                if (hit.transform.tag ==("IsGun"))
                {
                    Destroy(go);
                }
                Destroy(go, 10f);
            }
        }
        actualGun.data.actualAmmo--;
        lastShootTime = actualGun.data.fireRate;

        AddRecoil();

        actualGun.bulletEfect.Play();

        actualGun.audioSource.PlayOneShot(actualGun.shoot);
    }
    private void AddRecoil()
    {
        targetRotation += new Vector3(-actualGun.data.recoil.x,Random.Range(-actualGun.data.recoil.y, actualGun.data.recoil.y),0f);
    }
    void Reload()
    {
        actualGun.data.actualAmmo = actualGun.data.maxAmmoCount;
    }
    void Weaponchange(int index)
    {
        if (guns[index] != null)
        {
            actualGun = guns[index];
            actualGun.gameObject.SetActive(true);
        }
    }
    void Drop()
    {
        actualGun.GetComponent<Rigidbody>().useGravity = true;
        actualGun.GetComponent<Rigidbody>().isKinematic = false;
        actualGun.gameObject.transform.SetParent(null);
        actualGun = null;

        guns[indexGun] = null;
    }
    void GetGun(Gun getGun)
    {
        if (actualGun != null)
        {
            Drop();
        }
            
        actualGun = getGun;

        actualGun.GetComponent<Rigidbody>().useGravity = false;
        actualGun.GetComponent<Rigidbody>().isKinematic = true;

        audioSource.PlayOneShot(weaponChange);

        actualGun.transform.parent = playerController.GunPoint;
        actualGun.transform.localPosition = actualGun.data.offsset;
        actualGun.transform.localRotation = Quaternion.identity;

        guns[indexGun] = actualGun;
    }
    void OnUnScoped()
    {
        scopeOverlay.SetActive(isScoped);
    }
    IEnumerator onScoped()
    {
        yield return new WaitForSeconds(1f);

        scopeOverlay.SetActive(isScoped);
    }
    #endregion
}
