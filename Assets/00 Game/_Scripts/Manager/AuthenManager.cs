using System.Collections;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthenManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TopText;
    [SerializeField] private TextMeshProUGUI MessageText;

    [Header("Login")]
    [SerializeField] private TextMeshProUGUI EmailLoginInput;
    [SerializeField] private TextMeshProUGUI PasswordLoginInput;
    [SerializeField] private GameObject LoginPage;

    [Header("SignUp")]
    [SerializeField] private TextMeshProUGUI UsernameSignUpInput;
    [SerializeField] private TextMeshProUGUI EmailSignUpInput;
    [SerializeField] private TextMeshProUGUI PasswordSignUpInput;
    [SerializeField] private GameObject SignUpPage;

    private void Start()
    {
        if (!ValidateFields())
        {
            Debug.LogError("One or more required fields are not assigned in the Unity Inspector.");
            return;
        }

        OpenLoginPage();
    }

    private bool ValidateFields()
    {
        return TopText != null &&
               MessageText != null &&
               EmailLoginInput != null &&
               PasswordLoginInput != null &&
               UsernameSignUpInput != null &&
               EmailSignUpInput != null &&
               PasswordSignUpInput != null &&
               LoginPage != null &&
               SignUpPage != null;
    }

    public void OpenLoginPage()
    {
        if (!ValidateFields())
        {
            Debug.LogError("Cannot open LoginPage: Some fields are missing.");
            return;
        }

        TopText.text = "Login";
        LoginPage.SetActive(true);
        SignUpPage.SetActive(false);
        MessageText.text = "";
    }

    public void OpenSignUpPage()
    {
        if (!ValidateFields())
        {
            Debug.LogError("Cannot open SignUpPage: Some fields are missing.");
            return;
        }

        TopText.text = "SignUp";
        LoginPage.SetActive(false);
        SignUpPage.SetActive(true);
        MessageText.text = "";
    }

    public void SignUp()
    {
        string email = EmailSignUpInput.text.Trim();
        string username = UsernameSignUpInput.text.Trim();
        string password = PasswordSignUpInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MessageText.color = Color.yellow;
            MessageText.text = "All fields are required.";
            return;
        }

        if (PlayerPrefs.HasKey(email))
        {
            MessageText.color = Color.yellow;
            MessageText.text = "Email is already registered. Please use a different email.";
            return;
        }

        string hashedPassword = HashPassword(password);
        PlayerPrefs.SetString(email, hashedPassword);
        PlayerPrefs.SetString(email + "_username", username);
        PlayerPrefs.Save();

        MessageText.color = Color.green;
        MessageText.text = "Account created successfully!";
        OpenLoginPage();
    }

    public void Login()
    {
        string email = EmailLoginInput.text.Trim();
        string password = PasswordLoginInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            MessageText.color = Color.yellow;
            MessageText.text = "All fields are required.";
            return;
        }

        if (!PlayerPrefs.HasKey(email))
        {
            MessageText.color = Color.red;
            MessageText.text = "Email not found. Please sign up first.";
            return;
        }

        string storedHashedPassword = PlayerPrefs.GetString(email);
        string enteredHashedPassword = HashPassword(password);

        if (enteredHashedPassword == storedHashedPassword)
        {
            string username = PlayerPrefs.GetString(email + "_username");
            MessageText.color = Color.green;
            MessageText.text = $"Welcome, {username}!";
            StartCoroutine(LoadScene());
        }
        else
        {
            MessageText.color = Color.red;
            MessageText.text = "Invalid password. Please try again.";
        }
    }

    private IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Map1");
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new();

            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
