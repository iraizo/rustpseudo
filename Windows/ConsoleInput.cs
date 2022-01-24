using System;
using UnityEngine;

namespace Windows
{
	public class ConsoleInput
	{
		public string inputString = "";

		public string[] statusText = new string[3] { "", "", "" };

		internal float nextUpdate;

		public bool valid => Console.get_BufferWidth() > 0;

		public int lineWidth => Console.get_BufferWidth();

		public event Action<string> OnInputText;

		public void ClearLine(int numLines)
		{
			Console.set_CursorLeft(0);
			Console.Write(new string(' ', lineWidth * numLines));
			Console.set_CursorTop(Console.get_CursorTop() - numLines);
			Console.set_CursorLeft(0);
		}

		public void RedrawInputLine()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			ConsoleColor backgroundColor = Console.get_BackgroundColor();
			ConsoleColor foregroundColor = Console.get_ForegroundColor();
			try
			{
				Console.set_ForegroundColor((ConsoleColor)15);
				Console.set_CursorTop(Console.get_CursorTop() + 1);
				for (int i = 0; i < statusText.Length; i++)
				{
					Console.set_CursorLeft(0);
					Console.Write(statusText[i].PadRight(lineWidth));
				}
				Console.set_CursorTop(Console.get_CursorTop() - (statusText.Length + 1));
				Console.set_CursorLeft(0);
				Console.set_BackgroundColor((ConsoleColor)0);
				Console.set_ForegroundColor((ConsoleColor)10);
				ClearLine(1);
				if (inputString.Length == 0)
				{
					Console.set_BackgroundColor(backgroundColor);
					Console.set_ForegroundColor(foregroundColor);
					return;
				}
				if (inputString.Length < lineWidth - 2)
				{
					Console.Write(inputString);
				}
				else
				{
					Console.Write(inputString.Substring(inputString.Length - (lineWidth - 2)));
				}
			}
			catch (Exception)
			{
			}
			Console.set_BackgroundColor(backgroundColor);
			Console.set_ForegroundColor(foregroundColor);
		}

		internal void OnBackspace()
		{
			if (inputString.Length >= 1)
			{
				inputString = inputString.Substring(0, inputString.Length - 1);
				RedrawInputLine();
			}
		}

		internal void OnEscape()
		{
			inputString = "";
			RedrawInputLine();
		}

		internal void OnEnter()
		{
			ClearLine(statusText.Length);
			Console.set_ForegroundColor((ConsoleColor)10);
			Console.WriteLine("> " + inputString);
			string obj = inputString;
			inputString = "";
			if (this.OnInputText != null)
			{
				this.OnInputText(obj);
			}
			RedrawInputLine();
		}

		public void Update()
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Invalid comparison between Unknown and I4
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Invalid comparison between Unknown and I4
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Invalid comparison between Unknown and I4
			if (!valid)
			{
				return;
			}
			if (nextUpdate < Time.get_realtimeSinceStartup())
			{
				RedrawInputLine();
				nextUpdate = Time.get_realtimeSinceStartup() + 0.5f;
			}
			try
			{
				if (!Console.get_KeyAvailable())
				{
					return;
				}
			}
			catch (Exception)
			{
				return;
			}
			ConsoleKeyInfo val = Console.ReadKey();
			if ((int)((ConsoleKeyInfo)(ref val)).get_Key() == 13)
			{
				OnEnter();
			}
			else if ((int)((ConsoleKeyInfo)(ref val)).get_Key() == 8)
			{
				OnBackspace();
			}
			else if ((int)((ConsoleKeyInfo)(ref val)).get_Key() == 27)
			{
				OnEscape();
			}
			else if (((ConsoleKeyInfo)(ref val)).get_KeyChar() != 0)
			{
				inputString += ((ConsoleKeyInfo)(ref val)).get_KeyChar();
				RedrawInputLine();
			}
		}
	}
}
