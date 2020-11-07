using System;
using System.Threading;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает точку входа программы
	/// </summary>
	public static class OSMacrosEditorProgram
		{
		/// <summary>
		/// Главная точка входа для приложения
		/// </summary>
		[STAThread]
		public static void Main (string[] args)
			{
			// Инициализация
			Application.EnableVisualStyles ();
			Application.SetCompatibleTextRenderingDefault (false);

			// Запрос языка приложения
			SupportedLanguages al = Localization.CurrentLanguage;

			// Проверка запуска единственной копии
			bool result;
			Mutex instance = new Mutex (true, ProgramDescription.AssemblyTitle, out result);
			if (!result)
				{
				MessageBox.Show (string.Format (Localization.GetText ("AlreadyStarted", al), ProgramDescription.AssemblyTitle),
					ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
				}

			// Отображение справки и запроса на принятие Политики
			if (!ProgramDescription.AcceptEULA ())
				return;
			ProgramDescription.ShowAbout (true);

			// Запуск
			string macroFile = "";
			if (args.Length > 0)
				macroFile = args[0];

			Application.Run (new OSMacrosEditorForm (macroFile));
			}
		}
	}
