using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;

public class AuthenManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TopText;
    [SerializeField] TextMeshProUGUI MessageText;

    [Header("Login")]
    [SerializeField] TextMeshProUGUI EmailLoginInput;
    [SerializeField] TextMeshProUGUI PasswordLoginInput;
    [SerializeField] GameObject LoginPage;

    [Header("SignUp")]
    [SerializeField] TextMeshProUGUI UsernameSignUpInput;
    [SerializeField] TextMeshProUGUI EmailSignUpInput;
    [SerializeField] TextMeshProUGUI PasswordSignUpInput;
    [SerializeField] GameObject SignUpPage;

    void Start()
    {
        // Validate all serialized fields
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
        string email = EmailSignUpInput.text;
        string username = UsernameSignUpInput.text;
        string password = PasswordSignUpInput.text;

        // Ensure inputs are not null or empty
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MessageText.text = "All fields are required.";
            return;
        }

        // Check if email already exists
        if (PlayerPrefs.HasKey(email))
        {
            MessageText.color = Color.yellow;
            MessageText.text = "Email is already registered. Please use a different email.";
            return;
        }

        // Hash the password
        string hashedPassword = HashPassword(password);

        // Save credentials
        PlayerPrefs.SetString(email, hashedPassword); // Store email-password pair
        PlayerPrefs.SetString(email + "_username", username); // Store username
        PlayerPrefs.Save();

        MessageText.color = Color.green;
        MessageText.text = "Account created successfully!";
        OpenLoginPage();
    }

    public void Login()
    {
        string email = EmailLoginInput.text;
        string password = PasswordLoginInput.text;

        // Ensure inputs are not null or empty
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            MessageText.color = Color.yellow;
            MessageText.text = "All fields are required.";
            return;
        }

        // Check if email exists
        if (!PlayerPrefs.HasKey(email))
        {
            MessageText.color = Color.red;
            MessageText.text = "Email not found. Please sign up first.";
            return;
        }

        // Validate hashed password
        string storedHashedPassword = PlayerPrefs.GetString(email);
        string enteredHashedPassword = HashPassword(password);

        if (enteredHashedPassword == storedHashedPassword)
        {
            string username = PlayerPrefs.GetString(email + "_username");
            MessageText.color = Color.green;
            MessageText.text = $"Welcome back, {username}!";
        }
        else
        {
            MessageText.color = Color.red;
            MessageText.text = "Invalid password. Please try again.";
        }
    }

    #region HelperFunctions
    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();

            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
    }
    #endregion
}
