using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text;
using System.Collections;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

public class AuthManager : MonoBehaviour
{
    [Header("Common UI")]
    [SerializeField] private TextMeshProUGUI TopText;
    [SerializeField] private TextMeshProUGUI MessageText;

    [Header("Login")]
    [SerializeField] private TMP_InputField EmailLoginInput;
    [SerializeField] private TMP_InputField PasswordLoginInput;
    [SerializeField] private GameObject LoginPage;

    [Header("SignUp")]
    [SerializeField] private TMP_InputField UsernameSignUpInput;
    [SerializeField] private TMP_InputField EmailSignUpInput;
    [SerializeField] private TMP_InputField PasswordSignUpInput;
    [SerializeField] private GameObject SignUpPage;

    private const string GmailPattern = @"^[a-zA-Z0-9._%+-]+@gmail\.com$";
    private Coroutine messageCoroutine;

    private void Start()
    {
        this.ResetData();

        if (!ValidateFields())
        {
            Debug.LogError("Một hoặc nhiều trường tham chiếu chưa được gán trong Unity Inspector.");
            return;
        }

        SetInputFieldValidationToNone(EmailLoginInput);
        SetInputFieldValidationToNone(PasswordLoginInput);
        SetInputFieldValidationToNone(UsernameSignUpInput);
        SetInputFieldValidationToNone(EmailSignUpInput);
        SetInputFieldValidationToNone(PasswordSignUpInput);

        StartCoroutine(SwitchToLoginPage());
    }

    private void ResetData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Đã xóa tất cả dữ liệu PlayerPrefs!");
    }

    private void SetInputFieldValidationToNone(TMP_InputField inputField)
    {
        if (inputField != null)
        {
            inputField.characterValidation = TMP_InputField.CharacterValidation.None;
        }
        else
        {
            Debug.LogWarning("InputField chưa được gán trong Inspector.");
        }
    }

    private bool ValidateFields()
    {
        bool isValid = TopText != null &&
                       MessageText != null &&
                       EmailLoginInput != null &&
                       PasswordLoginInput != null &&
                       UsernameSignUpInput != null &&
                       EmailSignUpInput != null &&
                       PasswordSignUpInput != null &&
                       LoginPage != null &&
                       SignUpPage != null;

        if (!isValid)
        {
            Debug.LogError("Lỗi tham chiếu: Vui lòng kiểm tra và gán đầy đủ các đối tượng UI trong Inspector.");
        }
        return isValid;
    }

    public void OpenLoginPage()
    {
        StartCoroutine(SwitchToLoginPage());
    }

    public void OpenSignUpPage()
    {
        StartCoroutine(SwitchToSignUpPage());
    }

    private IEnumerator SwitchToLoginPage()
    {
        if (!ValidateFields()) yield break;

        TopText.text = "Đăng nhập";
        yield return new WaitForSecondsRealtime(1.5f); // ⏳ Delay 1.5s
        yield return FadePages(SignUpPage, LoginPage);
        ClearInputFields();
        ClearMessage();
    }

    private IEnumerator SwitchToSignUpPage()
    {
        if (!ValidateFields()) yield break;

        TopText.text = "Đăng ký";
        yield return new WaitForSecondsRealtime(1.5f); // ⏳ Delay 1.5s
        yield return FadePages(LoginPage, SignUpPage);
        ClearInputFields();
        ClearMessage();
    }

    private IEnumerator FadePages(GameObject from, GameObject to)
    {
        if (from != null) from.SetActive(false);
        yield return null;
        if (to != null) to.SetActive(true);
    }

    private IEnumerator SwitchToLoginPageWithDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        yield return SwitchToLoginPage();
    }

    public void SignUp()
    {
        if (!ValidateFields()) return;

        string email = EmailSignUpInput.text.Trim();
        string username = UsernameSignUpInput.text.Trim();
        string password = PasswordSignUpInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowMessage("Vui lòng điền đầy đủ thông tin.", Color.yellow);
            return;
        }

        if (!Regex.IsMatch(email, GmailPattern))
        {
            ShowMessage("Vui lòng nhập địa chỉ email Gmail hợp lệ (ví dụ: example@gmail.com).", Color.yellow);
            return;
        }

        if (PlayerPrefs.HasKey(email))
        {
            ShowMessage("Email này đã được đăng ký. Vui lòng sử dụng email khác.", Color.yellow);
            return;
        }

        string hashedPassword = HashPassword(password);
        PlayerPrefs.SetString(email, hashedPassword);
        PlayerPrefs.SetString(email + "_username", username);
        PlayerPrefs.Save();

        ShowMessage("Tạo tài khoản thành công!", Color.green);
        StartCoroutine(SwitchToLoginPageWithDelay(2f));
    }

    public void Login()
    {
        if (!ValidateFields()) return;

        string email = EmailLoginInput.text.Trim();
        string password = PasswordLoginInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowMessage("Vui lòng điền đầy đủ email và mật khẩu.", Color.yellow);
            return;
        }

        if (!PlayerPrefs.HasKey(email))
        {
            ShowMessage("Email không tồn tại. Vui lòng đăng ký trước.", Color.red);
            return;
        }

        string storedHashedPassword = PlayerPrefs.GetString(email);
        string enteredHashedPassword = HashPassword(password);

        if (enteredHashedPassword == storedHashedPassword)
        {
            string username = PlayerPrefs.GetString(email + "_username", "Người dùng");
            ShowMessage($"Chào mừng trở lại, {username}!", Color.green);
            StartCoroutine(LoadSceneAfterDelay("MainMenu", 2f));
        }
        else
        {
            ShowMessage("Mật khẩu không chính xác. Vui lòng thử lại.", Color.red);
        }
    }

    private void ShowMessage(string message, Color color)
    {
        if (MessageText == null) return;

        MessageText.color = color;
        MessageText.text = message;

        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }
        messageCoroutine = StartCoroutine(HideMessageAfterDelay(3f));
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        ClearMessage();
    }

    private void ClearMessage()
    {
        if (MessageText != null)
        {
            MessageText.text = "";
        }
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        SceneManager.LoadScene(sceneName);
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(bytes);
            StringBuilder builder = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    private void ClearInputFields()
    {
        if (EmailLoginInput != null) EmailLoginInput.text = "";
        if (PasswordLoginInput != null) PasswordLoginInput.text = "";
        if (UsernameSignUpInput != null) UsernameSignUpInput.text = "";
        if (EmailSignUpInput != null) EmailSignUpInput.text = "";
        if (PasswordSignUpInput != null) PasswordSignUpInput.text = "";
    }
}
