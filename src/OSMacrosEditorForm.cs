using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает главную форму программы
	/// </summary>
	public partial class OSMacrosEditorForm:Form
		{
		// Переменные
		private List<MacroCommand> commands = new List<MacroCommand> ();
		private SupportedLanguages al = Localization.CurrentLanguage;

		/// <summary>
		/// Конструктор главной формы программы
		/// </summary>
		public OSMacrosEditorForm ()
			{
			// Инициализация
			InitializeComponent ();

			LanguageCombo.Items.AddRange (Localization.LanguagesNames);
			try
				{
				LanguageCombo.SelectedIndex = (int)al;
				}
			catch
				{
				LanguageCombo.SelectedIndex = 0;
				}

			// Настройка контролов
			this.Text = ProgramDescription.AssemblyTitle;

			MouseX.Maximum = Screen.PrimaryScreen.Bounds.Width - 1;
			MouseY.Maximum = Screen.PrimaryScreen.Bounds.Height - 1;

			for (int i = 0; i < 256; i++)
				{
				KeyCode.Items.Add (((Keys)i).ToString () + " (" + i.ToString () + ")");
				}
			KeyCode.SelectedIndex = 0;
			}

		// Локализация формы
		private void LanguageCombo_SelectedIndexChanged (object sender, EventArgs e)
			{
			// Сохранение
			Localization.CurrentLanguage = al = (SupportedLanguages)LanguageCombo.SelectedIndex;

			// Локализация
			OFDialog.Title = Localization.GetText ("OFDialogTitle", al);
			SFDialog.Title = Localization.GetText ("SFDialogTitle", al);
			ExDialog.Title = Localization.GetText ("ExDialogTitle", al);
			FDialog.Title = Localization.GetText ("FDialogTitle", al);
			OFDialog.Filter = SFDialog.Filter = ExDialog.Filter = Localization.GetText ("OFDialogFilter", al);
			FDialog.Filter = Localization.GetText ("FDialogFilter", al);

			if (KeyModifiers.Items.Count == 0)
				{
				KeyModifiers.Items.Add (Localization.GetText ("NoModifiers", al));
				KeyModifiers.Items.Add ("Shift");
				KeyModifiers.Items.Add ("Ctrl");
				KeyModifiers.Items.Add ("Ctrl + Shift");
				KeyModifiers.Items.Add ("Alt");
				KeyModifiers.Items.Add ("Alt + Shift");
				KeyModifiers.Items.Add ("Ctrl + Alt");
				KeyModifiers.Items.Add ("Ctrl + Alt + Shift");
				KeyModifiers.Items.Add (Localization.GetText ("WinKey", al));

				KeyModifiers.SelectedIndex = 0;
				}
			else
				{
				KeyModifiers.Items[0] = Localization.GetText ("NoModifiers", al);
				KeyModifiers.Items[8] = Localization.GetText ("WinKey", al);
				}

			MFile.Text = Localization.GetText ("MFileText", al);
			MOpen.Text = Localization.GetText ("MOpenText", al);
			MSave.Text = Localization.GetText ("MSaveText", al);
			MExecute.Text = Localization.GetText ("MExecuteText", al);
			MQuit.Text = Localization.GetText ("MQuitText", al);

			MousePointerGroup.Text = Localization.GetText ("MousePointerGroupText", al);
			SetMousePointer.Text = Localization.GetText ("SetMousePointerText", al);
			AddMousePointer.Text = Localization.GetText ("AddMousePointerText", al);

			PauseGroup.Text = Localization.GetText ("PauseGroupText", al);
			MilliLabel.Text = Localization.GetText ("MilliLabelText", al);
			AddPause.Text = Localization.GetText ("AddPauseText", al);

			AddLeftClick.Text = Localization.GetText ("AddLeftClickText", al);
			AddRightClick.Text = Localization.GetText ("AddRightClickText", al);
			AddDragBeginning.Text = Localization.GetText ("AddDragBeginningText", al);
			AddDragEnding.Text = Localization.GetText ("AddDragEndingText", al);

			KeyboardGroup.Text = Localization.GetText ("KeyboardGroupText", al);
			KeyReceiver.Text = Localization.GetText ("KeyReceiverText", al);
			AddKeyPress.Text = Localization.GetText ("AddKeyPressText", al);

			ExecutionGroup.Text = Localization.GetText ("ExecutionGroupText", al);
			SelectFile.Text = Localization.GetText ("SetMousePointerText", al);
			WaitForFinish.Text = Localization.GetText ("WaitForFinishText", al);
			AddFileExecution.Text = Localization.GetText ("AddFileExecutionText", al);

			CommandsListLabel.Text = Localization.GetText ("CommandsListLabelText", al);
			ExitButton.Text = Localization.GetText ("MQuitText", al);
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
			e.Cancel = (MessageBox.Show (Localization.GetText ("QuitApplication", al),
				ProgramDescription.AssemblyDescription, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No);
			}

		// Загрузка макроса
		private void MOpen_Click (object sender, EventArgs e)
			{
			if (MessageBox.Show (Localization.GetText ("OpenExistingFile", al),
				ProgramDescription.AssemblyDescription, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
				OFDialog.ShowDialog ();
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
				MessageBox.Show (string.Format (Localization.GetText ("FileIsUnavailable", al), OFDialog.FileName),
					ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
				}

			StreamReader SR = new StreamReader (FS, Encoding.Default);

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
				MessageBox.Show (string.Format (Localization.GetText ("CannotCreateFile", al), SFDialog.FileName),
					ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
				}
			StreamWriter SW = new StreamWriter (FS, Encoding.Default);

			// Запись
			for (int i = 0; i < commands.Count; i++)
				{
				SW.WriteLine (commands[i].MacroFileCommandPresentation);
				}

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
			if (MessageBox.Show (Localization.GetText ("BeginMacro", al),
				ProgramDescription.AssemblyDescription, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
				return;

			// Проверка существования файла
			if (!System.IO.File.Exists (Application.StartupPath + "\\" + ProgramDescription.AssemblyExecutionModule))
				{
				MessageBox.Show (ProgramDescription.AssemblyExecutionModule + Localization.GetText ("ExecutionIsUnavailable", al),
					ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
				}

			// Обработка количества выполнений
			uint repeats = 0;
			try
				{
				repeats = uint.Parse (ExecutionRepeats.Text);
				}
			catch
				{
				}

			if (repeats < 1)
				{
				ExecutionRepeats.Text = "1";
				repeats = 1;
				}

			// Запуск
			this.WindowState = FormWindowState.Minimized;
			System.Diagnostics.Process.Start (Application.StartupPath + "\\" + ProgramDescription.AssemblyExecutionModule,
				"\"" + ExDialog.FileName + "\" " + repeats.ToString ());
			this.WindowState = FormWindowState.Normal;
			}

		// Выбор позиции указателя мыши
		private void SetMousePointer_Click (object sender, EventArgs e)
			{
			MousePointerSelector mps = new MousePointerSelector ();
			MouseX.Value = mps.MouseX;
			MouseY.Value = mps.MouseY;
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
			CommandPath.Text = FDialog.FileName.Substring (0, ((FDialog.FileName.Length > 200) ? 200 : FDialog.FileName.Length));
			}

		// Обновление списка команд
		private void UpdateCommandsList ()
			{
			// Обновление списка
			CommandsListBox.Items.Clear ();
			for (int i = 0; i < commands.Count; i++)
				{
				CommandsListBox.Items.Add (commands[i].CommandPresentation);
				}

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
			ProgramDescription.ShowAbout (false);
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
		}
	}
