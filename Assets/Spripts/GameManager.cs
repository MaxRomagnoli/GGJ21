using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Player variables")]
    [SerializeField] private Transform player;
    [SerializeField] private LookAtTarget playerLookAtTarget;
    [SerializeField] private FirstPersonMovement firstPersonMovement;
    [SerializeField] private FirstPersonLook firstPersonLook;
    [SerializeField] private RaycastSomething raycastSomething;
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private AudioSource takeSound;
    [SerializeField] private AudioSource airportSound;
    [SerializeField] private AudioSource storageSound;

    [Header("Game variables")]
    [SerializeField] private bool startNow = true;
    [SerializeField] private GameObject startSpawner;
    [SerializeField] private TextureSwiper startBelt;
    [SerializeField] private float spawnRate = 1f;
    [SerializeField] private int lifes = 5;
    [SerializeField] private float beltSpeed = 2f;
    [SerializeField] private float suitcaseForce = 1000f;
    [SerializeField] private float mapLimit = -10f;
    [SerializeField] private GameObject[] objToFound;
    
    [Header("Monitor variables")]
    [SerializeField] private MeshRenderer[] monitorRenderer;
    [SerializeField] private Material monitorMaterial;
    [SerializeField] private AudioSource monitorAudio;
    
    [Header("Rei-chan variables")]
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private GameObject reiChan;
    [SerializeField] private GameObject canvas3D;
    
    [Header("UI variables")]
    [SerializeField] private GameObject panelGame;
    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private GameObject panelOptions;
    //[SerializeField] private Slider lifesSlider;
    [SerializeField] private Text lifesText;
    [SerializeField] private Text suitcasesText;
    [SerializeField] private Text infoText;
    [SerializeField] private Text textTalk;

    private enum GameState { Play, Talking, Talked, GameOver };
    private Queue<Transform> objsToSpawn = new Queue<Transform>();
    private List<Transform> spawnPoints = new List<Transform>();
    private List<Transform> spawnExits = new List<Transform>();
    private GameState gameState;
    private float currentSpawnRate;
    private Vector2 textureSpeed;
    private int founded;

    // Start is called before the first frame update
    void Start()
    {
        //Variables
        Time.timeScale = 1;
        textureSpeed = new Vector2(0, beltSpeed * 0.1f);
        gameState = GameState.Play;
        founded = objToFound.Length;
        
        //Panels
        panelGame.SetActive(true);
        panelGameOver.SetActive(false);
        panelOptions.SetActive(false);
        canvas3D.SetActive(false);

        //UI
        lifesText.text = lifes.ToString() + " lifes";
        suitcasesText.text = founded.ToString() + " suitcases on " + objToFound.Length;
        //lifesSlider.maxValue = lifes;
        //lifesSlider.value = lifes;

        GetAllSpawns();
        ActiveAllObjects(false);
    }

    public void StartStorageSound() {
        if(!storageSound.isPlaying) {
            airportSound.Stop();
            storageSound.Play();
        }
    }

    public void StartTalking() {
        if(gameState == GameState.Play) {
            gameState = GameState.Talking;
            StartCoroutine(WaitAndSee());
        }
    }

    public void TakeObject(GameObject _obj) {
        if(_obj == null) { return; }
        Destroy(_obj);
        founded++;
        takeSound.Play();
        if(AllObjsFound()) {
            GameOver(true);
        }
        else {
            suitcasesText.text = founded.ToString() + " suitcases on " + objToFound.Length;
        }
    }

    private IEnumerator WaitAndSee()
    {
        //Disattiva controlli Player
        firstPersonMovement.enabled = false;
        firstPersonLook.enabled = false;
        raycastSomething.enabled = false;
        startSpawner.SetActive(true);

        // Visualizza Canvas 3D
        textTalk.text = "Hi, i'm Rin-chan! Give me your suitcases";
        canvas3D.SetActive(true);
        playerLookAtTarget.enabled = true;
        playerLookAtTarget.SetTarget(reiChan);
        if(!startNow) { yield return new WaitForSeconds(5f); }

        // Visualizza Valigie
        canvas3D.SetActive(false);
        startBelt.Activate(true);
        foreach (GameObject item in objToFound)
        {
            founded--;
            suitcasesText.text = founded.ToString() + " suitcases on " + objToFound.Length;
            item.SetActive(true);
            playerLookAtTarget.SetTarget(item);
            if(!startNow) { yield return new WaitForSeconds(1f); }
        }
        if(!startNow) {  yield return new WaitForSeconds(2f); }

        // Cambia materiale per tutti i monitor
        foreach (MeshRenderer renderer in monitorRenderer)
        {
            renderer.material = monitorMaterial;
        }

        // Visualizza monitor
        monitorAudio.Play();
        if(!startNow) {  yield return new WaitForSeconds(0.5f); }
        playerLookAtTarget.SetTarget(monitorRenderer[0].gameObject);
        if(!startNow) { yield return new WaitForSeconds(2f); }

        // Visualizza Canvas 3D
        textTalk.text = "OH NO! Your flight has been canceled, your bags will be lost!";
        canvas3D.SetActive(true);
        playerLookAtTarget.SetTarget(reiChan);
        if(!startNow) { yield return new WaitForSeconds(5f); }
        textTalk.text = "And there's nothing i can do!";
        if(!startNow) { yield return new WaitForSeconds(5f); }

        // Inchino
        textTalk.text = "Sorry";
        characterAnimator.SetTrigger("bowTrigger");
        yield return new WaitForSeconds(0.5f);

        // Riattiva comandi
        gameState = GameState.Talked;
        playerLookAtTarget.enabled = false;
        firstPersonMovement.enabled = true;
        firstPersonLook.enabled = true;
        raycastSomething.enabled = true;

        // Inchino
        while(true) {
            canvas3D.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            canvas3D.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public Vector2 TextureSpeed() {
        return textureSpeed;
    }

    public float BeltSpeed() {
        return beltSpeed;
    }

    public float SuitcaseForce() {
        return suitcaseForce;
    }

    public bool IsOutOfMap(Transform _transform) {
        return _transform.position.y < mapLimit;
    }

    void GetAllSpawns()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("spawnPoint"))
        {
            spawnPoints.Add(go.transform);
        }

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("spawnExit"))
        {
            spawnExits.Add(go.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Se il gioco non è in play, non fare altro
        if(gameState != GameState.Play && gameState != GameState.Talked) { return; }

        // Spawn once in frame rate
        if(currentSpawnRate < 0 && objsToSpawn.Count > 0) {
            currentSpawnRate = spawnRate;
            Transform spawnPosition = GetSpawnPointPosition();
            Transform objSpawned = objsToSpawn.Dequeue ();
            objSpawned.gameObject.SetActive(true);
            objSpawned.position = spawnPosition.position;
            objSpawned.rotation = spawnPosition.rotation;
        } else {
            currentSpawnRate -= Time.deltaTime;
        }

        // Se il giocatore cade dalla mappa, perde
        /*if(IsOutOfMap(player)) {
            if(GetDamage()) {
                GameOver(false);
            }
            else {
                // TODO: Non funziona
                player.position = GetSpawnPointPosition();
            }
        }*/
    }

    public bool GetDamage(int damage = 1) {
        lifes -= damage;
        lifesText.text = lifes.ToString() + " lifes";
        //lifesSlider.value = lifes;
        deathSound.Play();
        return lifes <= 0;
    }

    void ActiveAllObjects(bool _toSet) {
        foreach (GameObject item in objToFound)
        {
            item.SetActive(_toSet);
        }
    }

    bool AllObjsFound() {
        return founded == objToFound.Length;
    }

    public void GameOver(bool win) {

        // Disattiva scripts
        firstPersonMovement.enabled = false;
        firstPersonLook.enabled = false;
        raycastSomething.enabled = false;

        // Sblocca il mouse
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        gameState = GameState.GameOver;
        Time.timeScale = 0;

        if(win) {
            infoText.text = "You Win!\nYou found all the suitcases!";
        }
        else {
            infoText.text = "You Lost!\nYou found " + founded.ToString() + " suitcases on " + objToFound.Length;
        }
        
        // Panels
        panelGame.SetActive(false);
        panelGameOver.SetActive(true);
        panelOptions.SetActive(false);
    }

    public void AddInPool(Transform _transform) {
        if(spawnExits == null || _transform == null) { return; }
        _transform.gameObject.SetActive(false);
        objsToSpawn.Enqueue(_transform);
    }

    public Transform GetSpawnPointPosition() {
        return spawnExits[Random.Range(0, spawnExits.Count)];
    }

    /*void OnDrawGizmos()
    {
        GetAllSpawns();

        // Disegna tutti gli spawnPoints in editor
        if(spawnPoints != null) {
            foreach (Transform child in spawnPoints)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(child.transform.position, 1);
            }
        }
        
        // Disegna tutti gli spawnPoints in editor
        if(spawnExits != null) {
            foreach (Transform child in spawnExits)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(child.transform.position, 1);
            }
        }
    }*/

    /* BUTTONS */

    public void Replay() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Exit() {
        #if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OpenOptions() {
        panelGame.SetActive(false);
        panelGameOver.SetActive(false);
        panelOptions.SetActive(true);
    }

    public void OpenMainMenu() {
        panelGame.SetActive(false);
        panelGameOver.SetActive(true);
        panelOptions.SetActive(false);
    }

    public void SapeOptionsMenu() {
        // TODO: Save PlayerPrefs
        OpenMainMenu();
    }

}
