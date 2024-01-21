using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerMenuUI : MonoBehaviour
{

    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button newLobbyButton;
    [SerializeField] private Button codeJoinButton;
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private GameObject multiplayerMenuUI;
    [SerializeField] private GameObject newLobbyUI;

    // Start is called before the first frame update
    void Start()
    {
        quickJoinButton.interactable = false;
        newLobbyButton.interactable = false;
        codeJoinButton.interactable = false;

        playerNameInputField.onValueChanged.AddListener(delegate
        {
            InputValueCheck();
            MultiplayerManager.Instance.SetPlayerName(playerNameInputField.text);
        });

        codeInputField.onValueChanged.AddListener(delegate
        {
            InputValueCheck();
        });


        quickJoinButton.onClick.AddListener(() =>
        {
            // Start Client
            NetworkManager.Singleton.StartClient();


            //LobbyManager.Instance.QuickJoin();
        });


        newLobbyButton.onClick.AddListener(() =>
        {
            //Start Host
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);

            //newLobbyUI.gameObject.SetActive(true);
        });

        codeJoinButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.JoinWithCode(codeInputField.text);
        });
    }

    public void InputValueCheck()
    {
        if (playerNameInputField.text != null && playerNameInputField.text.Length > 0)
        {
            quickJoinButton.interactable = true;
            newLobbyButton.interactable = true;
            if (codeInputField.text != null && codeInputField.text.Length > 0)
            {
                codeJoinButton.interactable = true;
            }
            else
            {
                codeJoinButton.interactable = false;
            }
        }
        else
        {
            quickJoinButton.interactable = false;
            newLobbyButton.interactable = false;
            codeJoinButton.interactable = false;
        }
    }
}
