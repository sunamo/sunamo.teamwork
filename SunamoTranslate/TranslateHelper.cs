using Google.Apis.Auth.OAuth2;
using Google.Cloud.Translation.V2;
using System;
using Google.Apis.Storage.v1;
using Google.Apis.Services;
using Google.Apis.Translate.v2;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Must be instance, because calling static method dont call constructor and will throw not implemented exception
/// </summary>
public class TranslateHelper
{
static Type type = typeof(TranslateHelper);
    private GoogleCredential _credential = null;
    /// <summary>
    /// now return void instead GoogleCredential because is authorized in GDataTranslate project
    /// </summary>
    /// <param name="appName"></param>
    /// <param name="projectId"></param>
    /// <param name="jsonText"></param>
    /// <returns></returns>
    public void AuthExplicit(string appName, string projectId, string jsonText)
    {
        if (_credential == null)
        {
            _credential = GoogleCredential.FromJson(jsonText);
            // Inject the Cloud Storage scope if required.
            if (_credential.IsCreateScopedRequired)
            {
                _credential = _credential.CreateScoped(new[]
                {
                    StorageService.Scope.CloudPlatform,
                    TranslateService.Scope.CloudTranslation
                });
            }
            var storage = new StorageService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = appName,
            });
        }
        //return _credential;
    }
    /// <summary>
    /// Due to saving and reading already translated cs to en in txt
    /// </summary>
    public static TranslateHelper Instance = new TranslateHelper();
    public readonly string AlreadyTranslatedFile = AppData.ci.GetFileCommonSettings("CsTranslatedToEn.txt");
    public readonly string AlreadyTranslatedFileLong = AppData.ci.GetFileCommonSettings("CsTranslatedToEnLong.txt");
    private Dictionary<string, string> _csToEn = new Dictionary<string, string>();
    private TranslationClient _client = null;
    private TranslateHelper()
    {
        // ctor must be empty due to calling AllProjectsSearchHelper.AuthGoogleTranslate which create instance and in its actual code is CheckCredentials which raise exception
    }
    bool initialized = false;

    public void Init()
    {
        if (!initialized)
        {
            initialized = true;
            var data = SF.GetAllElementsFile(AlreadyTranslatedFile);
            foreach (var line in data)
            {
                var key = line[0];
                if (!_csToEn.ContainsKey(key))
                {
                    // Check for uncomplete file
                    if (line.Count > 1)
                    {
                        _csToEn.Add(key, line[1]);
                    }
                }
            }
            CheckCredentials();
            _client = TranslationClient.Create(_credential);
        }
    }
    /// <summary>
    /// A2 = cs, en
    /// A3 = null, will be automatically determined
    /// </summary>
    /// <param name="input"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public string Translate(string input, string to, string from = null)
    {
        if (input == "Nepodarilo se nacist prvek, pridejte nejake a akci opakujte")
        {

        }

        if (from.Contains("cs") && to.Contains("en"))
        {
            if (_csToEn.ContainsKey(input))
            {
                return _csToEn[input];
            }
        }
        else if (from.Contains("en") && to.Contains("cs"))
        {
            if (_csToEn.ContainsValue(input))
            {
                return _csToEn.FirstOrDefault(x => x.Value == input).Key;
            }
        }

#if DEBUG
        //DebugLogger.Instance.WriteLine($"Translate {input} from {from} to {to}");
#endif
        var response = _client.TranslateText(input, to, from);
        var result = response.TranslatedText;
        if (from.Contains("cs") && to.Contains("en"))
        {
            if (TranslateHelper.IsToSaveInCsTranslateToEn(input))
            {
                SF.AppendToFile(AlreadyTranslatedFile, SF.PrepareToSerialization2(CA.ToListString(input, result)));
            }
        }
        else if (from.Contains("en") && to.Contains("cs"))
        {
            if (TranslateHelper.IsToSaveInCsTranslateToEn(input))
            {
                SF.AppendToFile(AlreadyTranslatedFile, SF.PrepareToSerialization2(CA.ToListString(result, input)));
            }
        }

#if DEBUG
        DebugLogger.Instance.WriteLine("Translated: " + result);
#endif
        return result;
    }
    private void CheckCredentials()
    {
        if (_credential == null)
        {
            ThrowExceptions.Custom(Exc.GetStackTrace(), type, Exc.CallingMethod(),"Please authenticate first, credential object cant be null");
        }
    }
    public static bool IsToSaveInCsTranslateToEn(string first)
    {
        return SH.OccurencesOfStringIn(first, " ") < 3;
    }
}