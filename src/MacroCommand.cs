using System;
using System.Drawing;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Доступные типы макрокоманд
	/// </summary>
	public enum CommandTypes
		{
		/// <summary>
		/// Выполнить приложение или файл
		/// </summary>
		ExecuteCommand = 0,

		/// <summary>
		/// Установить позицию мыши
		/// </summary>
		SetCursorPosition = 1,

		/// <summary>
		/// Нажатие левой кнопки мыши
		/// </summary>
		LeftMouseClick = 2,

		/// <summary>
		/// Нажатие правой кнопки мыши
		/// </summary>
		RightMouseClick = 3,

		/// <summary>
		/// Нажатие клавиш клавиатуры
		/// </summary>
		PressKeys = 4,

		/// <summary>
		/// Пауза в выполнении макроса
		/// </summary>
		ExecutionPause = 5,

		/// <summary>
		/// Начало перетаскивания
		/// </summary>
		StartDragNDrop = 6,

		/// <summary>
		/// Конец перетаскивания
		/// </summary>
		FinishDragNDrop = 7,

		/// <summary>
		/// Выполнить приложение или файл и ждать завершения
		/// </summary>
		ExecuteCommandAndWait = 8,

		/// <summary>
		/// Начать цикл
		/// </summary>
		BeginCycle = 10,

		/// <summary>
		/// Завершить цикл
		/// </summary>
		EndCycle = 11,

		/// <summary>
		/// Ждать, пока пиксель не изменит цвет
		/// </summary>
		WaitForPixelChange = 12
		}

	/// <summary>
	/// Доступные макрокоманды мыши
	/// </summary>
	public enum MouseCommands
		{
		/// <summary>
		/// Нажатие левой кнопки мыши
		/// </summary>
		LeftMouseClick = 2,

		/// <summary>
		/// Нажатие правой кнопки мыши
		/// </summary>
		RightMouseClick = 3,

		/// <summary>
		/// Начало перетаскивания
		/// </summary>
		StartDragNDrop = 6,

		/// <summary>
		/// Конец перетаскивания
		/// </summary>
		FinishDragNDrop = 7
		}

	/// <summary>
	/// Доступные модификаторы нажатий клавиш
	/// </summary>
	public enum KeyModifiers
		{
		/// <summary>
		/// Без модификаторов
		/// </summary>
		NoModifiers = 0,

		/// <summary>
		/// Shift
		/// </summary>
		Shift = 1,

		/// <summary>
		/// Ctrl
		/// </summary>
		Control = 2,

		/// <summary>
		/// Ctrl + Shift
		/// </summary>
		ShiftControl = 3,

		/// <summary>
		/// Alt
		/// </summary>
		Alt = 4,

		/// <summary>
		/// Alt + Shift
		/// </summary>
		ShiftAlt = 5,

		/// <summary>
		/// Ctrl + Alt
		/// </summary>
		ControlAlt = 6,

		/// <summary>
		/// Ctrl + Alt + Shift
		/// </summary>
		ShiftControlAlt = 7,

		/// <summary>
		/// Win key
		/// </summary>
		WinKey = 8
		}

	/// <summary>
	/// Класс описывает отдельную макрокоманду
	/// </summary>
	public class MacroCommand
		{
		// Переменные
		private static char[] splitters = new char[] { ' ', '\t' };

		/// <summary>
		/// Тип макрокоманды
		/// </summary>
		public CommandTypes CommandType
			{
			get
				{
				return commandType;
				}
			}
		private CommandTypes commandType;

		/// <summary>
		/// Путь для выполняемого приложения или файла
		/// </summary>
		public string CommandPath
			{
			get
				{
				return commandPath;
				}
			}
		private string commandPath = "";

		/// <summary>
		/// Координата X курсора мыши
		/// </summary>
		public uint MouseX
			{
			get
				{
				return mouseX;
				}
			}
		private uint mouseX = 0;

		/// <summary>
		/// Координата Y курсора мыши
		/// </summary>
		public uint MouseY
			{
			get
				{
				return mouseY;
				}
			}
		private uint mouseY = 0;

		/// <summary>
		/// Длительность паузы исполнения макроса
		/// </summary>
		public uint PauseLength
			{
			get
				{
				return pauseLength;
				}
			}
		private uint pauseLength = 0;

		/// <summary>
		/// Количество повторов цикла
		/// </summary>
		public uint CycleRounds
			{
			get
				{
				return cycleRounds;
				}
			}
		private uint cycleRounds = 0;

		/// <summary>
		/// Количество повторов цикла
		/// </summary>
		public Color PixelColor
			{
			get
				{
				return pixelColor;
				}
			}
		private Color pixelColor = Color.FromArgb (0, 0, 0);

		/// <summary>
		/// Конструктор. Создаёт команду управления мышью
		/// </summary>
		/// <param name="Command">Команда мыши</param>
		public MacroCommand (MouseCommands Command)
			{
			commandType = (CommandTypes)Command;
			}

		/// <summary>
		/// Конструктор. Создаёт команду приостановки исполнения макроса на указанное время
		/// </summary>
		/// <param name="Pause">Длительность паузы в миллисекундах</param>
		public MacroCommand (uint Pause)
			{
			commandType = CommandTypes.ExecutionPause;
			pauseLength = Pause;
			}

		/// <summary>
		/// Конструктор. Создаёт команду исполнения команды или файла
		/// </summary>
		/// <param name="ExecutionPath">Команда или путь к файлу</param>
		/// <param name="WaitForCompletion">Флаг указывает, следует ли ожидать завершения выполнения команды</param>
		public MacroCommand (string ExecutionPath, bool WaitForCompletion)
			{
			commandType = (WaitForCompletion ? CommandTypes.ExecuteCommandAndWait : CommandTypes.ExecuteCommand);
			commandPath = ExecutionPath;
			}

		/// <summary>
		/// Конструктор. Создаёт команду начала или окончания цикла
		/// </summary>
		/// <param name="CycleBeginning">Тип команды</param>
		/// <param name="CycleRounds">Количество повторов</param>
		public MacroCommand (bool CycleBeginning, uint CycleRounds)
			{
			commandType = (CycleBeginning ? CommandTypes.BeginCycle : CommandTypes.EndCycle);
			cycleRounds = CycleRounds;
			if (cycleRounds < 2)
				cycleRounds = 2;
			}

		/// <summary>
		/// Конструктор. Создаёт команду установки курсора мыши в указанную позицию
		/// </summary>
		/// <param name="X">Координата X курсора мыши</param>
		/// <param name="Y">Координата Y курсора мыши</param>
		public MacroCommand (uint X, uint Y)
			{
			commandType = CommandTypes.SetCursorPosition;
			mouseX = X;
			mouseY = Y;
			}

		/// <summary>
		/// Модификатор нажатой клавиши
		/// </summary>
		public KeyModifiers PressedKeyModifier
			{
			get
				{
				return pressedKeyModifier;
				}
			}
		private KeyModifiers pressedKeyModifier = KeyModifiers.NoModifiers;

		/// <summary>
		/// Нажатая клавиша
		/// </summary>
		public Keys PressedKey
			{
			get
				{
				return pressedKey;
				}
			}
		private Keys pressedKey = Keys.None;

		/// <summary>
		/// Конструктор. Создаёт команду нажатия клавиш
		/// </summary>
		/// <param name="Modifiers">Модификатор клавиши</param>
		/// <param name="Key">Клавиша</param>
		public MacroCommand (KeyModifiers Modifiers, Keys Key)
			{
			commandType = CommandTypes.PressKeys;
			pressedKeyModifier = Modifiers;
			pressedKey = Key;
			}

		/// <summary>
		/// Конструктор. Создаёт команду ожидания изменения цвета указанного пикселя
		/// </summary>
		/// <param name="X">Координата X пикселя экрана</param>
		/// <param name="Y">Координата Y пикселя экрана</param>
		/// <param name="TrueColor">Цвет пикселя, являющийся условием для продолжения ожидания</param>
		public MacroCommand (uint X, uint Y, Color TrueColor)
			{
			commandType = CommandTypes.WaitForPixelChange;
			mouseX = X;
			mouseY = Y;
			pixelColor = Color.FromArgb (255, TrueColor);
			}

		/// <summary>
		/// Возвращает представление команды, используемое для записи в файл
		/// </summary>
		public string MacroFileCommandPresentation
			{
			get
				{
				string res;
				switch (commandType)
					{
					case CommandTypes.BeginCycle:
						res = "+ ";
						break;

					case CommandTypes.EndCycle:
						res = "-";
						break;

					default:
						res = ((int)commandType).ToString () + " ";
						break;
					}

				switch (commandType)
					{
					case CommandTypes.ExecuteCommand:
					case CommandTypes.ExecuteCommandAndWait:
						return (res + commandPath);

					case CommandTypes.ExecutionPause:
						return (res + pauseLength.ToString ());

					case CommandTypes.FinishDragNDrop:
					case CommandTypes.LeftMouseClick:
					case CommandTypes.RightMouseClick:
					case CommandTypes.StartDragNDrop:
					case CommandTypes.EndCycle:
						return res;

					case CommandTypes.PressKeys:
						return (res + ((int)pressedKeyModifier).ToString () + " " + ((int)pressedKey).ToString ());

					case CommandTypes.SetCursorPosition:
						double x = ((double)mouseX / (double)(Screen.PrimaryScreen.Bounds.Width - 1)) * (double)0xFFFF;
						double y = ((double)mouseY / (double)(Screen.PrimaryScreen.Bounds.Height - 1)) * (double)0xFFFF;
						return res + ((uint)Math.Ceiling (x)).ToString () + " " + ((uint)Math.Ceiling (y)).ToString ();

					case CommandTypes.WaitForPixelChange:
						return "C " + mouseX.ToString () + " " + mouseY.ToString () + " " +
							pixelColor.R.ToString () + " " + pixelColor.G.ToString () + " " +
							pixelColor.B.ToString () + " " + Screen.PrimaryScreen.Bounds.Width.ToString () + " " +
							Screen.PrimaryScreen.Bounds.Height.ToString ();

					case CommandTypes.BeginCycle:
						return (res + cycleRounds.ToString ());

					default:
						throw new Exception ("Parameters exchange failure at point 1. Debug needed");
					}
				}
			}

		/// <summary>
		/// Возвращает текстовое представление команды
		/// </summary>
		public string CommandPresentation
			{
			get
				{
				switch (commandType)
					{
					case CommandTypes.ExecuteCommand:
						return "Execution: " + commandPath;

					case CommandTypes.ExecuteCommandAndWait:
						return "Execution and waiting: " + commandPath;

					case CommandTypes.ExecutionPause:
						return "Execution pause (" + pauseLength.ToString () + " ms)";

					case CommandTypes.FinishDragNDrop:
						return "End dragging";

					case CommandTypes.LeftMouseClick:
						return "Left mouse button click";

					case CommandTypes.PressKeys:
						return "Key press (" + pressedKeyModifier.ToString () + ", " + pressedKey.ToString () + ")";

					case CommandTypes.RightMouseClick:
						return "Right mouse button click";

					case CommandTypes.SetCursorPosition:
						return "Set mouse pointer position to (" + mouseX.ToString () + "; " + mouseY.ToString () + ")";

					case CommandTypes.StartDragNDrop:
						return "Begin dragging";

					case CommandTypes.BeginCycle:
						return " BEGIN CYCLE (" + cycleRounds.ToString () + " t.)";

					case CommandTypes.EndCycle:
						return " END CYCLE";

					case CommandTypes.WaitForPixelChange:
						return "Wait while Pos (" + mouseX.ToString () + "; " + mouseY.ToString () + ") != RGB (" +
							pixelColor.R.ToString () + "; " + pixelColor.G.ToString () + "; " +
							pixelColor.B.ToString () + ")";

					default:
						throw new Exception ("Parameters exchange failure at point 2. Debug needed");
					}
				}
			}

		/// <summary>
		/// Метод формирует команду из её файлового представления
		/// </summary>
		/// <param name="CommandPresentation">Файловое представление команды</param>
		/// <returns>Возвращает сформированную команду или null в случае невозможности её формирования</returns>
		public static MacroCommand BuildMacroCommand (string CommandPresentation)
			{
			// Разбор команды
			if (CommandPresentation == null)
				return null;

			string[] values = CommandPresentation.Split (splitters, StringSplitOptions.RemoveEmptyEntries);
			if (values.Length < 1)  // Пропуск пустых строк
				return null;

			// Обработка типа команды
			CommandTypes command;
			try
				{
				if (values[0] == "+")
					command = CommandTypes.BeginCycle;
				else if (values[0] == "-")
					command = CommandTypes.EndCycle;
				else if (values[0] == "C")
					command = CommandTypes.WaitForPixelChange;
				else
					command = (CommandTypes)uint.Parse (values[0]);
				}
			catch
				{
				return null;
				}

			// Обработка содержимого команды
			switch (command)
				{
				case CommandTypes.ExecuteCommand:
				case CommandTypes.ExecuteCommandAndWait:
					if (values.Length > 1)
						{
						string path = "";
						for (int i = 1; i < values.Length; i++)
							{
							path += (values[i] + " ");
							}
						return new MacroCommand (path, (command == CommandTypes.ExecuteCommandAndWait));
						}
					return null;

				case CommandTypes.ExecutionPause:
					uint pause = 0;
					try
						{
						pause = uint.Parse (values[1]); // Вызовет исключение и при отсутствии параметра
						}
					catch
						{
						}

					if (pause != 0) // Заведомо считанное значение, не равное нулю
						{
						return new MacroCommand (pause);
						}
					return null;    // Все ошибки

				case CommandTypes.FinishDragNDrop:
				case CommandTypes.LeftMouseClick:
				case CommandTypes.RightMouseClick:
				case CommandTypes.StartDragNDrop:
					return new MacroCommand ((MouseCommands)command);

				case CommandTypes.PressKeys:
					uint m = 0, k = 256;
					try
						{
						m = uint.Parse (values[1]); // Вызовут исключение и при отсутствии параметра
						k = uint.Parse (values[2]);
						}
					catch
						{
						}

					if ((m > 8) || (k > 255))
						return null;

					try
						{
						return new MacroCommand ((KeyModifiers)m, (Keys)k);     // Может вызвать сбой на этапе преобразования кода клавиши
						}
					catch
						{
						}
					return null;

				case CommandTypes.SetCursorPosition:
					uint x = 0, y = 0x10000;
					try
						{
						x = uint.Parse (values[1]); // Вызовут исключение и при отсутствии параметра
						y = uint.Parse (values[2]);
						}
					catch
						{
						}

					if ((x < 0x10000) && (y < 0x10000))
						{
						x = (uint)(((double)x / (double)0xFFFF) * (double)(Screen.PrimaryScreen.Bounds.Width - 1));
						y = (uint)(((double)y / (double)0xFFFF) * (double)(Screen.PrimaryScreen.Bounds.Height - 1));
						return new MacroCommand (x, y);
						}
					return null;

				case CommandTypes.WaitForPixelChange:
					x = 0;
					y = 0;
					Color c = Color.FromArgb (0, 0, 0);
					try
						{
						x = uint.Parse (values[1]); // Вызовут исключение и при отсутствии параметра
						y = uint.Parse (values[2]);
						c = Color.FromArgb (int.Parse (values[3]), int.Parse (values[4]), int.Parse (values[5]));

						return new MacroCommand (x, y, c);
						}
					catch
						{
						}

					return null;

				case CommandTypes.BeginCycle:
					uint cycleRounds = 0;
					try
						{
						cycleRounds = uint.Parse (values[1]);
						}
					catch
						{
						}

					if (cycleRounds > 1)
						{
						return new MacroCommand (true, cycleRounds);
						}
					return null;    // Все ошибки

				case CommandTypes.EndCycle:
					return new MacroCommand (false, 0);

				// Команда неопознана
				default:
					return null;
				}
			}
		}
	}
