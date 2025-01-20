using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceController : MonoBehaviour
{
    [System.Serializable]
    public struct PlayerGameData {
        public int player;
        public int lastCheckpoint;
        public int currentLap;
        public bool finished;
        public CarController carReference;
        public PlayerInput playerInput;
        public RaceHUDController playerHUD;

        // Constructor
        public PlayerGameData(int player, CarController carRef, PlayerInput pInput, RaceHUDController pHUD) {
            this.player = player;
            lastCheckpoint = -1;
            currentLap = 0;
            finished = false;
            carReference = carRef;
            playerInput = pInput;
            playerHUD = pHUD;
        }
    }

    public float timePenaltyOnRespawn = 2f;
    public Transform[] checkPoints;
    public Transform[] startingGrid;
    private int playersFinish = 0;

    public PlayerGameData[] playersGameData;

    void Start(){
        InitializeCheckPoints();
        InitializePlayers();
        StartCoroutine(RaceStartCountDown());
        SoundController.instance.StopSound("MainTheme");
        SoundController.instance.PlaySound("RaceMusic");
    }

    
    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (GameManager.instance.gamePaused) {
                GameManager.instance.ResumeGame();
            } else {
                GameManager.instance.SetGamePaused();
            }
            
        }

        
    }

    private void InitializeCheckPoints() {
        for (int i = 0; i < checkPoints.Length; i++) {
            CheckpointController checkpointController = checkPoints[i].GetComponent<CheckpointController>();
            checkpointController.raceController = this;
            checkpointController.index = i;
        }
    }

    private void InitializePlayers() {
        // Obtengo el total de jugadores a través del array de jugadores del GameManager
        int totalPlayers = GameManager.instance.playersSelectedCarIndexes.Length;
        // Inicializamos el array de PlayersGameData con su tamaño
        playersGameData = new PlayerGameData[totalPlayers];
        for (int i = 0; i < totalPlayers; i++) {
            // Inicializamos el jugador y sacamos el player game data devuelto por la función
            // que inicializa al jugador indicado
            PlayerGameData pgd = InitializePlayer(i + 1);
            // Lo añadimos al array
            playersGameData[i] = pgd;
        }
    }

    private PlayerGameData InitializePlayer(int player) {
        // CAR
        //Instanciación
        CarController carPrefab = GameManager.instance.carList[GameManager.instance.playersSelectedCarIndexes[player - 1]].racePrefab;
        // Instanciamos el coche
        CarController car = Instantiate(carPrefab);
        // Colocamos y orientamos el coche
        // Primero extraemos el punto de la parilla de salida
        Transform startPoint = startingGrid[player - 1];
        // Posicionamos el coche
        car.transform.position = startPoint.position;
        // Orientamos el coche
        car.transform.forward = startPoint.forward;

        // PLAYER INPUT
        PlayerInput playerInput = car.GetComponent<PlayerInput>();
        playerInput.player = player;

        // Suscribo el player al evento de respawn
        playerInput.OnRespawn += Respawn;
        

        // PLAYER CAMERA
        // Instanciamos una cámara para el jugador
        CameraController playerCamera = Instantiate(GameManager.instance.playerCameraPrefab);
        // Asignamos el jugador correspondiente a la cámara
        playerCamera.SetCameraPlayer(player, GameManager.instance.playersSelectedCarIndexes.Length);
        // Lo asignamos como objetivo
        playerCamera.target = car.transform;

        // PLAYER RACE HUD
        RaceHUDController playerHUD = Instantiate(GameManager.instance.hudPrefab);
        // Asignamos la camara del jugador al hud, de esta manera se secciona por pantallas
        playerHUD.raceHud.worldCamera = playerCamera.cam;
        playerHUD.SetLapText(0);
        playerHUD.SetPositionText(player);

        PlayerGameData gameData = new PlayerGameData(player, car, playerInput, playerHUD);


        return gameData;
    }

    public void PlayerHasPassThroughCheckpoint(int player, int checkpointIndex) {
        PlayerGameData pgd = playersGameData[player - 1];
        // Comprobamos si el checkpoint es correcto
        bool isCorrectCheckpoint = pgd.lastCheckpoint == (checkpointIndex - 1);
        bool isSameCheckPoint = pgd.lastCheckpoint == checkpointIndex;
        // Comprobamos si hemos completado una vuelta
        bool isALap = checkpointIndex == 0 && (pgd.lastCheckpoint == (checkPoints.Length - 1));

        // Si el checkpoint es correcto
        if (isCorrectCheckpoint || isALap) {
            // Actualizamos el checkpoint del jugador
            pgd.lastCheckpoint = checkpointIndex;
            // En caso de que sea una vuelta
            if (isALap) {
                // Incrementamos la vuelta actual
                pgd.currentLap++;
                if (pgd.currentLap == GameManager.instance.totalLaps) {
                    PlayerHasFinishedRace(player);
                }
                playersGameData[player - 1].playerHUD.SetLapText(pgd.currentLap);
            }
        }                                        

        // Compruebo si el checkpoint no es correcto 
        // La segunda comprobación es para que no haga respawn 2 veces
        if (!isCorrectCheckpoint && !isSameCheckPoint) {
            Respawn(player);
        }

        // Devolvemos el PlayerGameData actualizado a su lugar (los structs no funcionan como las clases. Funcionan como variables)
        playersGameData[player - 1] = pgd;

        if(isCorrectCheckpoint || isALap) {
            UpdatePlayersPosition();
            UpdatePositionText();
        }
    }                           

    public void Respawn(int player) {
        StartCoroutine(PenaltyTimer(player));
    }

    private IEnumerator PenaltyTimer(int player) {        
        playersGameData[player - 1].carReference.canMove = false;
        playersGameData[player - 1].playerInput.respawning = true;
        yield return new WaitForSeconds(timePenaltyOnRespawn);
        Transform respawnCheckPoint = checkPoints[playersGameData[player - 1].lastCheckpoint];
        playersGameData[player - 1].carReference.transform.position = respawnCheckPoint.position;
        playersGameData[player - 1].carReference.transform.forward = respawnCheckPoint.forward;
        playersGameData[player - 1].carReference.canMove = true;
        playersGameData[player - 1].playerInput.respawning = false;

    }

    private IEnumerator RaceStartCountDown() {
        ChangePlayerMovementState(false);
        SoundController.instance.PlaySound("CountDown");
        yield return new WaitForSeconds(3.5f);
        ChangePlayerMovementState(true);
    }

    private void ChangePlayerMovementState(bool state) {
        for(int i = 0; i < playersGameData.Length; i++) {
            PlayerGameData pgdAux = playersGameData[i];
            pgdAux.carReference.canMove = state;
            playersGameData[i] = pgdAux;
        }
    }

    private void UpdatePlayersPosition() {
        int pgdLenght = playersGameData.Length;

        // Primero ordeno por chekpoints
        for (int i = 0; i < pgdLenght - 1; i++) {
            for(int j = 0; j < pgdLenght - i - 1; j++) {
                // Si el checkpoint en el que estoy es mayor que el siguiente
                if(playersGameData[j].lastCheckpoint < playersGameData[j + 1].lastCheckpoint) {
                    PlayerGameData aux = playersGameData[j];
                    playersGameData[j] = playersGameData[j + 1];
                    playersGameData[j + 1] = aux;
                }
            }
        }

        // Segundo ordeno por vueltas
        for (int i = 0; i < pgdLenght - 1; i++) {
            for (int j = 0; j < pgdLenght - i - 1; j++) {
                // Si el checkpoint en el que estoy es mayor que el siguiente
                if (playersGameData[j].currentLap < playersGameData[j + 1].currentLap) {
                    PlayerGameData aux = playersGameData[j];
                    playersGameData[j] = playersGameData[j + 1];
                    playersGameData[j + 1] = aux;
                }
            }
        }

    }

    private void UpdatePositionText() {
        for(int i = 0; i < playersGameData.Length; i++) {
            playersGameData[i].playerHUD.SetPositionText(i + 1);
        }
    }

    private void PlayerHasFinishedRace(int player) {
        PlayerGameData pgd = playersGameData[player - 1];
        pgd.finished = true;
        playersGameData[player - 1] = pgd;
        playersFinish++;

        if(playersFinish == GameManager.instance.numberOfPlayers) {
            RaceHasEnded();
        }
    }

    private void RaceHasEnded() {
        Time.timeScale = 0;
    }
}
