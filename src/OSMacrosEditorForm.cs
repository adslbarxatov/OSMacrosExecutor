using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает главную форму программы
	/// </summary>
	public partial class OSMacrosEditorForm: Form
		{
		// Переменные
		private List<MacroCommand> commands = new List<MacroCommand> ();

		/// <summary>
		/// Конструктор главной формы программы
		/// </summary>
		/// <param name="MacroFile">Имя файла для загрузки при старте программы</param>
		public OSMacrosEditorForm (string MacroFile)
			{
			// Инициализация
			InitializeComponent ();

			LanguageCombo.Items.AddRange (Localization.LanguagesNames);
			try
				{
				LanguageCombo.SelectedIndex = (int)Localization.CurrentLanguage;
				}
			catch
				{
				LanguageCombo.SelectedIndex = 0;
				}

			// Настройка контролов
			this.Text = ProgramDescription.AssemblyTitle;
			RDGenerics.LoadWindowDimensions (this);

			MouseX.Maximum = Screen.PrimaryScreen.Bounds.Width - 1;
			MouseY.Maximum = Screen.PrimaryScreen.Bounds.Height - 1;

			for (int i = 0; i < 256; i++)
				KeyCode.Items.Add (((Keys)i).ToString () + " (" + i.ToString () + ")");
			KeyCode.SelectedIndex = 0;

			// Загрузка файла, если требуется
			if (MacroFile != "")
				{
				OFDialog.FileName = MacroFile;
				OFDialog_FileOk (null, null);
				}
			}

		// Локализация формы
		private void LanguageCombo_SelectedIndexChanged (object sender, EventArgs e)
			{
			// Сохранение
			Localization.CurrentLanguage = (SupportedLanguages)LanguageCombo.SelectedIndex;

			// Локализация
			OFDialog.Title = Localization.GetText ("OFDialogTitle");
			SFDialog.Title = Localization.GetText ("SFDialogTitle");
			ExDialog.Title = Localization.GetText ("ExDialogTitle");
			FDialog.Title = Localization.GetText ("FDialogTitle");
			OFDialog.Filter = SFDialog.Filter = ExDialog.Filter =
				string.Format (Localization.GetText ("OFDialogFilter"), ProgramDescription.AppExtension);
			FDialog.Filter = Localization.GetText ("FDialogFilter");

			if (KeyModifiers.Items.Count == 0)
				{
				KeyModifiers.Items.Add (Localization.GetText ("NoModifiers"));
				KeyModifiers.Items.Add ("Shift");
				KeyModifiers.Items.Add ("Ctrl");
				KeyModifiers.Items.Add ("Ctrl + Shift");
				KeyModifiers.Items.Add ("Alt");
				KeyModifiers.Items.Add ("Alt + Shift");
				KeyModifiers.Items.Add ("Ctrl + Alt");
				KeyModifiers.Items.Add ("Ctrl + Alt + Shift");
				KeyModifiers.Items.Add (Localization.GetText ("WinKey"));

				KeyModifiers.SelectedIndex = 0;
				}
			else
				{
				KeyModifiers.Items[0] = Localization.GetText ("NoModifiers");
				KeyModifiers.Items[8] = Localization.GetText ("WinKey");
				}

			MFile.Text = Localization.GetText ("MFileText");
			MOpen.Text = Localization.GetText ("MOpenText");
			MSave.Text = Localization.GetText ("MSaveText");
			MExecute.Text = Localization.GetText ("MExecuteText");
			MRegister.Text = Localization.GetText ("MRegisterText");
			MQuit.Text = Localization.GetText ("MQuitText");

			MousePointerGroup.Text = Localization.GetText ("MousePointerGroupText");
			SetMousePointer.Text = Localization.GetText ("SetMousePointerText");
			AddMousePointer.Text = Localization.GetText ("AddMousePointerText");
			AddWaitForColor.Text = Localization.GetText ("AddWaitForColorText");

			PauseGroup.Text = Localization.GetText ("PauseGroupText");
			MilliLabel.Text = Localization.GetText ("MilliLabelText");
			AddPause.Text = Localization.GetText ("AddPauseText");

			AddLeftClick.Text = Localization.GetText ("AddLeftClickText");
			AddRightClick.Text = Localization.GetText ("AddRightClickText");
			AddDragBeginning.Text = Localization.GetText ("AddDragBeginningText");
			AddDragEnding.Text = Localization.GetText ("AddDragEndingText");

			KeyboardGroup.Text = Localization.GetText ("KeyboardGroupText");
			KeyReceiver.Text = Localization.GetText ("KeyReceiverText");
			AddKeyPress.Text = Localization.GetText ("AddKeyPressText");

			ExecutionGroup.Text = Localization.GetText ("ExecutionGroupText");
			SelectFile.Text = Localization.GetText ("SetMousePointerText");
			WaitForFinish.Text = Localization.GetText ("WaitForFinishText");
			AddFileExecution.Text = Localization.GetText ("AddFileExecutionText");

			CommandsListLabel.Text = Localization.GetText ("CommandsListLabelText");
			ExitButton.Text = Localization.GetText ("MQuitText");

			BeginCycle.Text = Localization.GetText ("BeginCycleText");
			EndCycle.Text = Localization.GetText ("EndCycleText");
			CycleLabel.Text = Localization.GetText ("CycleLabelText");
			}

		// Выход из программы
		private void MQuit_Click (object sender, EventArgs e)
			{
			this.Close ();
			}

		private void ExitButton_Click (object sender, EventArgs e)
			{
			this.Close ();
			}

		// Контроль выхода
		private void MainForm_FormClosing (object sender, FormClosingEventArgs e)
			{
			e.Cancel = (CommandsListBox.Items.Count != 0) &&
				(RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center,
				"QuitApplication", LzDefaultTextValues.Button_Yes,
				LzDefaultTextValues.Button_No) == RDMessageButtons.ButtonTwo);

			RDGenerics.SaveWindowDimensions (this);
			}

		// Загрузка макроса
		private void MOpen_Click (object sender, EventArgs e)
			{
			if ((CommandsListBox.Items.Count == 0) ||
				(RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center,
				"OpenExistingFile", LzDefaultTextValues.Button_Yes,
				LzDefaultTextValues.Button_No) == RDMessageButtons.ButtonOne))
				{
				OFDialog.ShowDialog ();
				}
			}

		private void OFDialog_FileOk (object sender, CancelEventArgs e)
			{
			// Загрузка
			FileStream FS = null;
			try
				{
				FS = new FileStream (OFDialog.FileName, FileMode.Open);
				}
			catch
				{
				/*RDGenerics.MessageBox (RDMessageTypes.Warning_Center,
					string.Format (Localization.GetText ("FileIsUnavailable"), OFDialog.FileName));*/
				RDGenerics.MessageBox (RDMessageTypes.Warning_Center,
					Localization.GetFileProcessingMessage (OFDialog.FileName,
					LzFileProcessingMessageTypes.Load_Failure));
				return;
				}

			StreamReader SR = new StreamReader (FS, RDGenerics.GetEncoding (SupportedEncodings.UTF8));

			// Чтение
			commands.Clear ();

			while (!SR.EndOfStream)
				{
				string s = SR.ReadLine ();
				MacroCommand cmd = MacroCommand.BuildMacroCommand (s);
				if (cmd != null)
					commands.Add (cmd);
				}

			// Завершено
			SR.Close ();
			FS.Close ();

			UpdateCommandsList ();
			}

		// Сохранение макроса
		private void MSave_Click (object sender, EventArgs e)
			{
			SFDialog.ShowDialog ();
			}

		private void SFDialog_FileOk (object sender, CancelEventArgs e)
			{
			// Инициализация
			FileStream FS = null;
			try
				{
				FS = new FileStream (SFDialog.FileName, FileMode.Create);
				}
			catch
				{
				/*RDGenerics.MessageBox (RDMessageTypes.Warning_Center,
					string.Format (Localization.GetText ("CannotCreateFile"), SFDialog.FileName));*/
				RDGenerics.MessageBox (RDMessageTypes.Warning_Center,
					Localization.GetFileProcessingMessage (SFDialog.FileName,
					LzFileProcessingMessageTypes.Save_Failure));
				return;
				}
			StreamWriter SW = new StreamWriter (FS, RDGenerics.GetEncoding (SupportedEncodings.UTF8));

			// Запись
			for (int i = 0; i < commands.Count; i++)
				SW.WriteLine (commands[i].MacroFileCommandPresentation);

			// Завершение
			SW.Close ();
			FS.Close ();
			}

		// Выполнение макроса
		private void MExecute_Click (object sender, EventArgs e)
			{
			ExDialog.ShowDialog ();
			}

		private void ExDialog_FileOk (object sender, CancelEventArgs e)
			{
			if (RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "BeginMacro",
				LzDefaultTextValues.Button_Yes, LzDefaultTextValues.Button_No) !=
				RDMessageButtons.ButtonOne)
				return;

			// Проверка существования файла
			if (!File.Exists (RDGenerics.AppStartupPath + ProgramDescription.AssemblyExecutionModule))
				{
				RDGenerics.MessageBox (RDMessageTypes.Warning_Center,
					ProgramDescription.AssemblyExecutionModule + Localization.GetText ("ExecutionIsUnavailable"));
				return;
				}

			// Обработка количества выполнений
			uint repeats = 0;
			try
				{
				repeats = uint.Parse (ExecutionRepeats.Text);
				}
			catch { }

			if (repeats < 1)
				{
				ExecutionRepeats.Text = "1";
				repeats = 1;
				}

			// Запуск
			this.WindowState = FormWindowState.Minimized;
			Process.Start (RDGenerics.AppStartupPath + ProgramDescription.AssemblyExecutionModule,
				"\"" + ExDialog.FileName + "\" " + repeats.ToString ());
			this.WindowState = FormWindowState.Normal;
			}

		// Выбор позиции указателя мыши
		private void SetMousePointer_Click (object sender, EventArgs e)
			{
			MousePointerSelector mps = new MousePointerSelector ();

			MouseX.Value = mps.MouseX;
			MouseY.Value = mps.MouseY;
			SetPixelColor.BackColor = mps.PixelColor;
			}

		// Выбор комбинации клавиш
		private void KeyReceiver_KeyDown (object sender, KeyEventArgs e)
			{
			KeyModifiers.SelectedIndex = (e.Shift ? 0x1 : 0x0) | (e.Control ? 0x2 : 0x0) | (e.Alt ? 0x4 : 0x0);
			KeyCode.SelectedIndex = (int)e.KeyCode;
			}

		// Выбор файла для исполнения
		private void SelectFile_Click (object sender, EventArgs e)
			{
			FDialog.ShowDialog ();
			}

		private void FDialog_FileOk (object sender, CancelEventArgs e)
			{
			CommandPath.Text = FDialog.FileName.Substring (0, ((FDialog.FileName.Length > 200) ? 200 :
				FDialog.FileName.Length));
			}

		// Обновление списка команд
		private void UpdateCommandsList ()
			{
			// Обновление списка
			CommandsListBox.Items.Clear ();
			for (int i = 0; i < commands.Count; i++)
				CommandsListBox.Items.Add (commands[i].CommandPresentation);

			if (CommandsListBox.Items.Count > 0)
				CommandsListBox.SelectedIndex = CommandsListBox.Items.Count - 1;

			// Обновление кнопок
			MoveUp.Enabled = MoveDown.Enabled = (CommandsListBox.Items.Count > 1);
			DeleteItem.Enabled = (CommandsListBox.Items.Count > 0);
			}

		// Добавление команд
		private void AddMousePointer_Click (object sender, EventArgs e)
			{
			commands.Add (new MacroCommand ((uint)MouseX.Value, (uint)MouseY.Value));
			UpdateCommandsList ();
			}

		private void AddKeyPress_Click (object sender, EventArgs e)
			{
			commands.Add (new MacroCommand ((KeyModifiers)KeyModifiers.SelectedIndex, (Keys)KeyCode.SelectedIndex));
			UpdateCommandsList ();
			}

		private void AddPause_Click (object sender, EventArgs e)
			{
			commands.Add (new MacroCommand ((uint)PauseLength.Value));
			UpdateCommandsList ();
			}

		private void AddFileExecution_Click (object sender, EventArgs e)
			{
			if (CommandPath.Text == "")
				CommandPath.Text = "-";

			commands.Add (new MacroCommand (CommandPath.Text, WaitForFinish.Checked));
			UpdateCommandsList ();
			}

		private void AddLeftClick_Click (object sender, EventArgs e)
			{
			commands.Add (new MacroCommand (MouseCommands.LeftMouseClick));
			UpdateCommandsList ();
			}

		private void AddRightClick_Click (object sender, EventArgs e)
			{
			commands.Add (new MacroCommand (MouseCommands.RightMouseClick));
			UpdateCommandsList ();
			}

		private void AddDragBeginning_Click (object sender, EventArgs e)
			{
			commands.Add (new MacroCommand (MouseCommands.StartDragNDrop));
			UpdateCommandsList ();
			}

		private void AddDragEnding_Click (object sender, EventArgs e)
			{
			commands.Add (new MacroCommand (MouseCommands.FinishDragNDrop));
			UpdateCommandsList ();
			}

		// Изменение списка
		private void DeleteItem_Click (object sender, EventArgs e)
			{
			if (CommandsListBox.SelectedIndex >= 0)
				{
				commands.RemoveAt (CommandsListBox.SelectedIndex);
				UpdateCommandsList ();
				}
			}

		private void MoveUp_Click (object sender, EventArgs e)
			{
			if (CommandsListBox.SelectedIndex <= 0)
				return;

			// Выбрана не верхняя позиция
			string command = commands[CommandsListBox.SelectedIndex].MacroFileCommandPresentation;
			int i = CommandsListBox.SelectedIndex;

			commands.RemoveAt (i);
			commands.Insert (i - 1, MacroCommand.BuildMacroCommand (command));
			UpdateCommandsList ();
			CommandsListBox.SelectedIndex = i - 1;
			}

		private void MoveDown_Click (object sender, EventArgs e)
			{
			// Выбрана не нижняя позиция
			if ((CommandsListBox.SelectedIndex >= 0) && (CommandsListBox.SelectedIndex < CommandsListBox.Items.Count - 1))
				{
				string command = commands[CommandsListBox.SelectedIndex].MacroFileCommandPresentation;
				int i = CommandsListBox.SelectedIndex;

				commands.RemoveAt (i);
				commands.Insert (i + 1, MacroCommand.BuildMacroCommand (command));
				UpdateCommandsList ();
				CommandsListBox.SelectedIndex = i + 1;
				}
			}

		// Отображение краткой справочной информации
		private void MainForm_HelpButtonClicked (object sender, CancelEventArgs e)
			{
			// Отмена обработки события вызова справки
			e.Cancel = true;

			// О программе
			RDGenerics.ShowAbout (false);
			}

		// Установка количества запусков макроса
		private void ExecutionRepeats_TextChanged (object sender, EventArgs e)
			{
			if (ExecutionRepeats.Text != "")
				{
				try
					{
					uint i = uint.Parse (ExecutionRepeats.Text);
					}
				catch
					{
					ExecutionRepeats.Text = "1";
					}
				}
			}

		// Управление циклами
		private void BeginCycle_Click (object sender, EventArgs e)
			{
			commands.Add (new MacroCommand (true, (uint)CycleRounds.Value));
			UpdateCommandsList ();
			}

		private void EndCycle_Click (object sender, EventArgs e)
			{
			commands.Add (new MacroCommand (false, 0));
			UpdateCommandsList ();
			}

		// Метод регистрирует сопоставление для файлов
		private void MRegister_Click (object sender, EventArgs e)
			{
			ProgramDescription.RegisterAppExtensions ();
			}

		// Добавление команды ожидания изменения пикселя
		private void AddWaitForColor_Click (object sender, EventArgs e)
			{
			commands.Add (new MacroCommand ((uint)MouseX.Value, (uint)MouseY.Value, SetPixelColor.BackColor));
			UpdateCommandsList ();
			}
		}
	}
