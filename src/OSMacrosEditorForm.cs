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

			LanguageCombo.Items.AddRange (RDLocale.LanguagesNames);
			try
				{
				LanguageCombo.SelectedIndex = (int)RDLocale.CurrentLanguage;
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
			RDLocale.CurrentLanguage = (RDLanguages)LanguageCombo.SelectedIndex;

			// Локализация
			OFDialog.Title = RDLocale.GetText ("OFDialogTitle");
			SFDialog.Title = RDLocale.GetText ("SFDialogTitle");
			ExDialog.Title = RDLocale.GetText ("ExDialogTitle");
			FDialog.Title = RDLocale.GetText ("FDialogTitle");
			OFDialog.Filter = string.Format (RDLocale.GetText ("OFDialogFilter"),
				ProgramDescription.NewAppExtension, "macro");
			SFDialog.Filter = ExDialog.Filter =
				string.Format (RDLocale.GetText ("SFDialogFilter"), ProgramDescription.NewAppExtension);
			FDialog.Filter = RDLocale.GetText ("FDialogFilter");

			if (KeyModifiers.Items.Count == 0)
				{
				KeyModifiers.Items.Add (RDLocale.GetText ("NoModifiers"));
				KeyModifiers.Items.Add ("Shift");
				KeyModifiers.Items.Add ("Ctrl");
				KeyModifiers.Items.Add ("Ctrl + Shift");
				KeyModifiers.Items.Add ("Alt");
				KeyModifiers.Items.Add ("Alt + Shift");
				KeyModifiers.Items.Add ("Ctrl + Alt");
				KeyModifiers.Items.Add ("Ctrl + Alt + Shift");
				KeyModifiers.Items.Add (RDLocale.GetText ("WinKey"));

				KeyModifiers.SelectedIndex = 0;
				}
			else
				{
				KeyModifiers.Items[0] = RDLocale.GetText ("NoModifiers");
				KeyModifiers.Items[8] = RDLocale.GetText ("WinKey");
				}

			MFile.Text = RDLocale.GetText ("MFileText");
			MOpen.Text = RDLocale.GetText ("MOpenText");
			MSave.Text = RDLocale.GetText ("MSaveText");
			MExecute.Text = RDLocale.GetText ("MExecuteText");
			MExecuteCurrent.Text = RDLocale.GetText ("MExecuteCurrentText");

			MHelp.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout);
			MQuit.Text = ExitButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Exit);

			MousePointerGroup.Text = RDLocale.GetText ("MousePointerGroupText");
			SetMousePointer.Text = RDLocale.GetText ("SetMousePointerText");
			AddMousePointer.Text = RDLocale.GetText ("AddMousePointerText");
			AddWaitForColor.Text = RDLocale.GetText ("AddWaitForColorText");

			PauseGroup.Text = RDLocale.GetText ("PauseGroupText");
			MilliLabel.Text = RDLocale.GetText ("MilliLabelText");
			AddPause.Text = RDLocale.GetText ("AddPauseText");

			AddLeftClick.Text = RDLocale.GetText ("AddLeftClickText");
			AddRightClick.Text = RDLocale.GetText ("AddRightClickText");
			AddDragBeginning.Text = RDLocale.GetText ("AddDragBeginningText");
			AddDragEnding.Text = RDLocale.GetText ("AddDragEndingText");

			KeyboardGroup.Text = RDLocale.GetText ("KeyboardGroupText");
			KeyReceiver.Text = RDLocale.GetText ("KeyReceiverText");
			AddKeyPress.Text = RDLocale.GetText ("AddKeyPressText");

			ExecutionGroup.Text = RDLocale.GetText ("ExecutionGroupText");
			SelectFile.Text = RDLocale.GetText ("SetMousePointerText");
			WaitForFinish.Text = RDLocale.GetText ("WaitForFinishText");
			AddFileExecution.Text = RDLocale.GetText ("AddFileExecutionText");

			CommandsListLabel.Text = RDLocale.GetText ("CommandsListLabelText");

			BeginCycle.Text = RDLocale.GetText ("BeginCycleText");
			EndCycle.Text = RDLocale.GetText ("EndCycleText");
			CycleLabel.Text = RDLocale.GetText ("CycleLabelText");
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
				"QuitApplication", RDLDefaultTexts.Button_Yes,
				RDLDefaultTexts.Button_No) == RDMessageButtons.ButtonTwo);

			RDGenerics.SaveWindowDimensions (this);
			}

		// Загрузка макроса
		private void MOpen_Click (object sender, EventArgs e)
			{
			if ((CommandsListBox.Items.Count == 0) ||
				(RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center,
				"OpenExistingFile", RDLDefaultTexts.Button_Yes,
				RDLDefaultTexts.Button_No) == RDMessageButtons.ButtonOne))
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
				RDGenerics.MessageBox (RDMessageTypes.Warning_Center,
					string.Format (RDLocale.GetDefaultText (RDLDefaultTexts.Message_LoadFailure_Fmt),
					OFDialog.FileName));
				return;
				}

			StreamReader SR = new StreamReader (FS, RDGenerics.GetEncoding (RDEncodings.CP1251));

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
				RDGenerics.MessageBox (RDMessageTypes.Warning_Center,
					string.Format (RDLocale.GetDefaultText (RDLDefaultTexts.Message_SaveFailure_Fmt),
					SFDialog.FileName));
				return;
				}
			StreamWriter SW = new StreamWriter (FS, RDGenerics.GetEncoding (RDEncodings.CP1251));

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

		private void MExecuteCurrent_Click (object sender, EventArgs e)
			{
			if (SFDialog.ShowDialog () != DialogResult.OK)
				return;

			ExDialog.FileName = SFDialog.FileName;
			ExDialog_FileOk (null, null);
			}

		private void ExDialog_FileOk (object sender, CancelEventArgs e)
			{
			if (RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "BeginMacro",
				RDLDefaultTexts.Button_Yes, RDLDefaultTexts.Button_No) !=
				RDMessageButtons.ButtonOne)
				return;

			// Проверка существования файла
			if (!RDGenerics.CheckLibraries (ProgramDescription.AssemblyExecutionModule, true))
				return;

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
			this.SendToBack ();

			Process p = Process.Start (RDGenerics.AppStartupPath + ProgramDescription.AssemblyExecutionModule,
				"\"" + ExDialog.FileName + "\" " + repeats.ToString ());
			p.WaitForExit ();

			this.BringToFront ();
			}

		// Выбор позиции указателя мыши
		private void SetMousePointer_Click (object sender, EventArgs e)
			{
			this.SendToBack ();
			MousePointerSelector mps = new MousePointerSelector ();
			this.BringToFront ();

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
		private void MHelp_Click (object sender, EventArgs e)
			{
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

		// Добавление команды ожидания изменения пикселя
		private void AddWaitForColor_Click (object sender, EventArgs e)
			{
			commands.Add (new MacroCommand ((uint)MouseX.Value, (uint)MouseY.Value, SetPixelColor.BackColor));
			UpdateCommandsList ();
			}

		// Ручное изменение цвета пикселя
		private void SetPixelColor_Click (object sender, EventArgs e)
			{
			CDialog.Color = SetPixelColor.BackColor;
			if (CDialog.ShowDialog () == DialogResult.OK)
				SetPixelColor.BackColor = CDialog.Color;
			}
		}
	}
