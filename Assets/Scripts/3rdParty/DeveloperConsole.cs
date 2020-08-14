using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeveloperConsole : SingletonBehaviour<DeveloperConsole>
{
    #region Property
    //readonly string bindingsGroup = "devConsole";
    Canvas dbgCanvas;
    GameObject parentObj;
    GameObject animationParent;
    Text txt;
    Text inputTxt;
    Image bg;
    const int maxChars = 2000;
    public bool shown { get; private set; } = false;

    Text fpsTxt;
    bool fpsShown => fpsTxt.gameObject.activeSelf;
    //private int fpsAccumulator = 0;
    //private float fpsPeriod = 0.5f;
    //private float fpsNextPeriod = 0;
    private string displayText = "{0} fps";

    bool animating = false;
    float animationTime = 0;
    Vector3 start;
    Vector3 end;

    Dictionary<string, ConsoleCommand> commands = new Dictionary<string, ConsoleCommand>();
    List<string> commandNames = new List<string>();

    int LogLevel = 0;

    private List<string> _scriptExecutedHistory = new List<string>();
    private int _scriptExecutedHistoryIndex;
    #endregion

    protected override void Awake()
    {
        if (!GameSetting.DeveloperConsole)
        {
            base.enabled = false;
            return;
        }

        parentObj = new GameObject("DeveloperConsole", typeof(Canvas), typeof(CanvasScaler));
        Canvas dbgCanvas = parentObj.GetComponent<Canvas>();
        dbgCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        dbgCanvas.pixelPerfect = true;
        dbgCanvas.sortingOrder = 1337; //make highest to make sure console stays on top
        parentObj.transform.SetParent(transform);

        GameObject fpsObj = new GameObject("FpsInfo", typeof(Text));
        fpsObj.transform.SetParent(parentObj.transform);
        fpsObj.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
        fpsObj.transform.localPosition = new Vector3(Screen.width / 2 - fpsObj.GetComponent<RectTransform>().sizeDelta.x / 2, Screen.height / 2 - fpsObj.GetComponent<RectTransform>().sizeDelta.y / 2, 0);
        fpsTxt = fpsObj.GetComponent<Text>();
        fpsTxt.font = Resources.Load<Font>("LucidaGrande");
        fpsTxt.fontSize = 20;
        fpsTxt.color = Color.green;
        fpsTxt.supportRichText = false;
        fpsTxt.raycastTarget = false;
        fpsTxt.alignment = TextAnchor.MiddleCenter;
        fpsTxt.horizontalOverflow = HorizontalWrapMode.Overflow;
        fpsTxt.verticalOverflow = VerticalWrapMode.Overflow;
        fpsObj.SetActive(false);

        animationParent = new GameObject("AnimationParent", typeof(Canvas));
        animationParent.transform.SetParent(parentObj.transform);
        animationParent.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        animationParent.GetComponent<RectTransform>().anchorMax = Vector2.one;
        animationParent.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        animationParent.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        animationParent.transform.localPosition = new Vector3(0, Screen.height, 0);

        GameObject bgObj = new GameObject("DeveloperConsoleBG", typeof(Image));
        bgObj.transform.SetParent(animationParent.transform);
        bg = bgObj.GetComponent<Image>();
        bg.rectTransform.anchorMin = Vector2.zero;
        bg.rectTransform.anchorMax = Vector2.one;
        bg.rectTransform.sizeDelta = Vector2.zero;
        bg.rectTransform.anchoredPosition = Vector2.zero;
        bg.color = new Color(0, 0, 0, 0.5f);

        GameObject textObj = new GameObject("DeveloperConsoleText", typeof(Text), typeof(Outline));
        textObj.transform.SetParent(animationParent.transform);
        txt = textObj.GetComponent<Text>();
        //txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.font = Resources.Load<Font>("LucidaGrande");
        txt.alignment = TextAnchor.LowerLeft;
        txt.verticalOverflow = VerticalWrapMode.Overflow;
        txt.rectTransform.anchorMin = Vector2.zero;
        txt.rectTransform.anchorMax = Vector2.one;
        txt.rectTransform.sizeDelta = new Vector2(0, -35);
        txt.rectTransform.anchoredPosition = Vector2.zero;
        // textObj.GetComponent<Outline>().effectColor = Color.black;
        txt.text = "==================DeveloperConsole=====================\n" +
                   "=======================================================\n" +
                   "=======================================================\n\n\n\n\n\n";

        GameObject inputTextObj = new GameObject("DeveloperConsoleText", typeof(Text), typeof(Outline));
        inputTextObj.transform.SetParent(animationParent.transform);
        inputTxt = inputTextObj.GetComponent<Text>();
        //inputTxt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        inputTxt.font = Resources.Load<Font>("LucidaGrande");
        inputTxt.fontSize = 16;
        inputTxt.color = Color.green;
        inputTxt.alignment = TextAnchor.LowerLeft;
        inputTxt.verticalOverflow = VerticalWrapMode.Overflow;
        inputTxt.rectTransform.anchorMin = Vector2.zero;
        inputTxt.rectTransform.anchorMax = Vector2.one;
        inputTxt.rectTransform.sizeDelta = Vector2.zero;
        inputTxt.rectTransform.anchoredPosition = Vector2.zero;
        // textObj.GetComponent<Outline>().effectColor = Color.black;
        inputTxt.text = ">";
    }

    void Start()
    {
        base.Persistent = true;

        //fpsNextPeriod = Time.realtimeSinceStartup + fpsPeriod;

        RegisterBasicCommands();
        RegisterOtherCommands();

        Application.logMessageReceived += HandleLog;
    }

    void Update()
    {
        if (!GameSetting.DeveloperConsole)
        {
            return;
        }
        
		// FPS Module
		if (fpsShown)
        {
            fpsTxt.text = string.Format(displayText, (1.0f / Time.smoothDeltaTime).ToString("#0.0"));
        }

        // Console Module
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            Toggle();
            return;
        }

        if (shown)
        {
            if (Input.inputString.Contains("\n") || Input.inputString.Contains("\r"))
            {
                ProcessInput();
            }
            else if (Input.inputString.Contains("\b"))
            {
                inputTxt.text = inputTxt.text.Substring(0, Mathf.Max(inputTxt.text.Length - 1, 1));
            }
            else if (Input.inputString != "")
            {
                inputTxt.text += Input.inputString;
            }

            if (_scriptExecutedHistory.Count != 0)
            {
                if (Input.GetKeyDown(KeyCode.PageUp))
                {
                    inputTxt.text = ">" + _scriptExecutedHistory[_scriptExecutedHistoryIndex != 0 ? _scriptExecutedHistoryIndex-- : 0];
                }
                if (Input.GetKeyDown(KeyCode.PageDown))
                {
                    inputTxt.text = ">" + _scriptExecutedHistory[_scriptExecutedHistoryIndex != _scriptExecutedHistory.Count - 1 ? _scriptExecutedHistoryIndex++ : _scriptExecutedHistory.Count - 1];
                }
            }
        }
    }

    private void HandleUnityConsoleLog(string message, string stackTrace, LogType type)
    {
        string coloredMessage = type == LogType.Log
            ? message
            : (type == LogType.Error ? message.Colored(Colors.red) : message.Colored(Colors.yellow));

        Log(((double)Time.time).ToString("0.00") + " - " + coloredMessage);
    }

    public void RegisterCommand(string commandName, ConsoleCommand newCommand)
    {
        commandName = commandName.Trim().ToLower();

        if (commandName.Contains(" "))
        {
            Log("[ERROR] Tried to register command containing whitespace.");
        }
        else if (!Instance.commands.ContainsKey(commandName))
        {
            commands.Add(commandName, newCommand);
            commandNames.Add(commandName);
            commandNames.Sort();
        }
        else
        {
            Log("[ERROR] Tried to register command \"" + commandName + "\" but it is already registered.");
        }
    }

    public void ExecuteCommand(string command)
    {
        inputTxt.text = $">{command}";
        ProcessInput();
    }

    public void Toggle()
    {
        Vector3 up = new Vector3(0, Screen.height, 0);
        Vector3 down = new Vector3(0, Screen.height / 2, 0);

        shown = !shown;

        if (shown)
        {
            _scriptExecutedHistoryIndex = _scriptExecutedHistory.Count == 0 ? 0 : _scriptExecutedHistory.Count - 1;
        }

        start = shown ? up : down;
        end = shown ? down : up;

        if (!animating)
        {
            animating = true;
            StartCoroutine(Animate());
        }
        else
        {
            animationTime = 1f - animationTime;
            start = !shown ? down : up;
            end = !shown ? up : down;
        }
    }

    #region Log
    public void Log(System.Object obj)
    {
        if (txt.text.Length > maxChars)
        {
            txt.text = txt.text.Substring(1500);
        }

        txt.text += "\n" + obj.ToString();
    }

    public void LogWarning(System.Object obj)
    {
        Log(obj.ToString().Colored(Colors.yellow));
    }

    public void LogError(System.Object obj)
    {
        Log(obj.ToString().Colored(Colors.red));
    }
    #endregion

    IEnumerator Animate()
    {
        animationTime = 0f;
        while (animationTime < 1f)
        {
            animationTime += Time.deltaTime * 4f;
            if (animationTime > 1f) animationTime = 1f;
            animationParent.transform.localPosition = Vector3.LerpUnclamped(start, end, animationTime * animationTime * (3f - 2f * animationTime)); //smooth step

            yield return null;
        }
        animating = false;
    }

    void ProcessInput()
    {
        // 无输入内容
        if (inputTxt.text.Substring(1) == string.Empty)
        {
            Log("~");
            inputTxt.text = ">";
            return;
        }

        // 记录输入指令历史
        _scriptExecutedHistory.Add(inputTxt.text.Substring(1));

        string[] inputArr = inputTxt.text.Substring(1).Split(' ');
        List<string> parameters = new List<string>();
        string command = inputArr[0].ToLower();
        Log(">" + inputTxt.text.Substring(1));

        for (int i = 1; i < inputArr.Length; i++)
        {
            if (inputArr[i] == "") continue;
            else if (inputArr[i].StartsWith("\""))
            {
                string quoteParam = inputArr[i];
                while (!inputArr[i].EndsWith("\"") && i < inputArr.Length)
                {
                    quoteParam += " " + inputArr[i];
                    i++;
                }
                parameters.Add(quoteParam);
            }
            else
            {
                parameters.Add(inputArr[i]);
            }

        }

        if (commands.ContainsKey(command.ToLower()))
        {
            if (parameters.Count > 0 && parameters[0] == "?")
            {
                Log(commands[command].HelpMessage);
            }
            else
            {
                Log(commands[command].Execute(parameters.ToArray()));
            }

            //Toggle();
        }
        else
        {
            // Log(("\n未知的命令: \"" + command + "\"\n").Colored(Colors.red));
            Log(("\n未知的命令: \"" + command + "\"\n"));
        }

        inputTxt.text = ">";
    }

    void RegisterBasicCommands()
    {
        RegisterCommand("version", new ConsoleCommand((x) => { return "Developer Console v9.1.0"; }, "version - Displays the version of the Developer Console."));
        RegisterCommand("help", new ConsoleCommand(
                (x) =>
                {
                    int idx = 0;
                    if (x.Length > 0)
                    {
                        int.TryParse(x[0], out idx);
                        idx -= 1;
                    }

                    int commandCount = commands.Count;

                    if (idx > commandCount)
                    {
                        idx = commandCount - 10;
                    }
                    else if (idx < 0)
                    {
                        idx = 0;
                    }

                    string helpString = "Showing commands " + (idx + 1) + " - " + Mathf.Min(idx + 10, commandCount) + " of " + commandCount + ":";

                    for (int i = idx; i < idx + 10 && i < commandCount; i++)
                    {
                        helpString += "\n  " + (i + 1) + " - " + commandNames[i] /*+ " : " + commands[commandNames[i]].HelpMessage*/;
                    }

                    helpString += "\nType \"?\" after a command for more details.";

                    return helpString;
                },
                "help <number> - Displays a list of commands."
        ));
        RegisterCommand("Logging", new ConsoleCommand(
            (x) =>
            {
                int logLevel = -1;

                if (x.Length > 0)
                {
                    if (int.TryParse(x[0], out logLevel))
                    {
                        LogLevel = Mathf.Clamp(logLevel, 0, 3);

                        if (LogLevel == 0)
                        {
                            Application.logMessageReceived -= Application_logMessageReceived;
                        }
                        else
                        {
                            Application.logMessageReceived += Application_logMessageReceived;
                        }

                        return "Logging level set to " + LogLevel;
                    }
                }

                return "Logging level is " + LogLevel;
            },
            "Sets logging level for the console.\n 0 - No logging.\n1 - Errors only.\n2 - Errors and Warnings.\n3 - Errors, Warnings and Debugs."
        ));
        RegisterCommand("UnityLog", new ConsoleCommand((x) => { unityLogShown = !unityLogShown; return unityLogShown ? "激活UnityLog显示" : "关闭UnityLog显示"; }, "Displays the Unity Log."));
        RegisterCommand("Fps", new ConsoleCommand((x) => { fpsTxt.gameObject.SetActive(!fpsTxt.gameObject.activeSelf); return fpsShown ? "激活fps显示" : "关闭fps显示"; }, "Displays the Fps."));
    }

    void RegisterOtherCommands()
    {

	}

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Log && LogLevel < 3) return;
        if (type == LogType.Warning && LogLevel < 2) return;

        Log(type + "\n" + condition + "\n" + stackTrace);
    }

    // ********************************************************************
    // GUILog
    // ********************************************************************
    private bool unityLogShown = false;
    private static Queue<string> guiQueue = new Queue<string>(6);
    void OnGUI()
    {
        //GUI.Label(new Rect(0f, 20f, 300, 100), $"shown?: {shown}");
        //GUI.Label(new Rect(0f, 40f, 300, 100), $"animationTime: {animationTime}");

        if (!unityLogShown)
            return;
        GUILayout.BeginArea(new Rect(0.0f, (float)(Screen.height - 140), (float)Screen.width, 140f));
        foreach (string text in guiQueue)
            GUILayout.Label(text);
        GUILayout.EndArea();
    }
    private void HandleLog(string message, string stackTrace, LogType type)
    {
        string coloredMessage = type == LogType.Log
            ? message
            : (type == LogType.Error ? message.Colored(Colors.red) : message.Colored(Colors.yellow));

        guiQueue.Enqueue(((double)Time.time).ToString("0.00") + " - " + coloredMessage);
        if (guiQueue.Count <= 5)
            return;
        guiQueue.Dequeue();
    }
}

public class ConsoleCommand
{
    public readonly Func<string[], string> Execute;
    public readonly string HelpMessage;

    public ConsoleCommand(Func<string[], string> ExecuteMethod, string HelpMessage)
    {
        Execute = ExecuteMethod;
        this.HelpMessage = HelpMessage;
    }
}