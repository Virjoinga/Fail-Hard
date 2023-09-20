using UnityEngine;

public class UniWebDemo : MonoBehaviour
{
	public GameObject cubePrefab;

	public TextMesh tipTextMesh;

	private UniWebView _webView;

	private string _errorMessage;

	private GameObject _cube;

	private Vector3 _moveVector;

	private void Start()
	{
		_cube = Object.Instantiate(cubePrefab) as GameObject;
		_cube.GetComponent<UniWebViewCube>().webViewDemo = this;
		_moveVector = Vector3.zero;
	}

	private void Update()
	{
		if (_cube != null)
		{
			if (_cube.transform.position.y < -5f)
			{
				Object.Destroy(_cube);
				_cube = null;
			}
			else
			{
				_cube.transform.Translate(_moveVector * Time.deltaTime);
			}
		}
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(0f, Screen.height - 150, 150f, 80f), "Open"))
		{
			_webView = GetComponent<UniWebView>();
			if (_webView == null)
			{
				_webView = base.gameObject.AddComponent<UniWebView>();
				_webView.OnReceivedMessage += OnReceivedMessage;
				_webView.OnLoadComplete += OnLoadComplete;
				_webView.OnWebViewShouldClose += OnWebViewShouldClose;
				_webView.OnEvalJavaScriptFinished += OnEvalJavaScriptFinished;
				_webView.InsetsForScreenOreitation += InsetsForScreenOreitation;
			}
			_webView.url = "http://uniwebview.onevcat.com/demo/index1-1.html";
			_webView.Load();
			_errorMessage = null;
		}
		if (_webView != null && GUI.Button(new Rect(150f, Screen.height - 150, 150f, 80f), "Back"))
		{
			_webView.GoBack();
		}
		if (_webView != null && GUI.Button(new Rect(300f, Screen.height - 150, 150f, 80f), "ToolBar"))
		{
			if (_webView.toolBarShow)
			{
				_webView.HideToolBar(true);
			}
			else
			{
				_webView.ShowToolBar(true);
			}
		}
		if (_errorMessage != null)
		{
			GUI.Label(new Rect(0f, 0f, Screen.width, 80f), _errorMessage);
		}
	}

	private void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
	{
		if (success)
		{
			webView.Show();
			return;
		}
		Debug.Log("Something wrong in webview loading: " + errorMessage);
		_errorMessage = errorMessage;
	}

	private void OnReceivedMessage(UniWebView webView, UniWebViewMessage message)
	{
		Debug.Log("Received a message from native");
		Debug.Log(message.rawMessage);
		if (string.Equals(message.path, "move"))
		{
			Vector3 moveVector = Vector3.zero;
			if (string.Equals(message.args["direction"], "up"))
			{
				moveVector = new Vector3(0f, 0f, 1f);
			}
			else if (string.Equals(message.args["direction"], "down"))
			{
				moveVector = new Vector3(0f, 0f, -1f);
			}
			else if (string.Equals(message.args["direction"], "left"))
			{
				moveVector = new Vector3(-1f, 0f, 0f);
			}
			else if (string.Equals(message.args["direction"], "right"))
			{
				moveVector = new Vector3(1f, 0f, 0f);
			}
			int result = 0;
			if (int.TryParse(message.args["distance"], out result))
			{
				moveVector *= (float)result;
			}
			_moveVector = moveVector;
		}
		else if (string.Equals(message.path, "add"))
		{
			if (_cube != null)
			{
				Object.Destroy(_cube);
			}
			_cube = Object.Instantiate(cubePrefab) as GameObject;
			_cube.GetComponent<UniWebViewCube>().webViewDemo = this;
			_moveVector = Vector3.zero;
		}
		else if (string.Equals(message.path, "close"))
		{
			webView.Hide();
			Object.Destroy(webView);
			webView.OnReceivedMessage -= OnReceivedMessage;
			webView.OnLoadComplete -= OnLoadComplete;
			webView.OnWebViewShouldClose -= OnWebViewShouldClose;
			webView.OnEvalJavaScriptFinished -= OnEvalJavaScriptFinished;
			webView.InsetsForScreenOreitation -= InsetsForScreenOreitation;
			_webView = null;
		}
	}

	public void ShowAlertInWebview(float time, bool first)
	{
		_moveVector = Vector3.zero;
		if (first)
		{
			_webView.EvaluatingJavaScript("sample(" + time + ")");
		}
	}

	private void OnEvalJavaScriptFinished(UniWebView webView, string result)
	{
		Debug.Log("js result: " + result);
		tipTextMesh.text = "<color=#000000>" + result + "</color>";
	}

	private bool OnWebViewShouldClose(UniWebView webView)
	{
		if (webView == _webView)
		{
			_webView = null;
			return true;
		}
		return false;
	}

	private UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation)
	{
		int aBottom = (int)((float)UniWebViewHelper.screenHeight * 0.5f);
		if (orientation == UniWebViewOrientation.Portrait)
		{
			return new UniWebViewEdgeInsets(5, 5, aBottom, 5);
		}
		return new UniWebViewEdgeInsets(5, 5, aBottom, 5);
	}
}
