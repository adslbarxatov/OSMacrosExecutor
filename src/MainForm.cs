using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace OSMacrosEditor
	{
	/// <summary>
	/// Класс описывает главную форму программы
	/// </summary>
	public partial class MainForm:Form
		{
		// Переменные
		private List<MacroCommand> commands = new List<MacroCommand> ();

		/// <summary>
		/// Конструктор главной формы программы
		/// </summary>
		public MainForm ()
			{
			InitializeComponent ();

			// Настройка контролов
			this.Text = ProgramDescription.AssemblyTitle;

			OFDialog.Title = "Select macro for editing";
			SFDialog.Title = "Select new macro's placement";
			ExDialog.Title = "Select macro for execution";
			FDialog.Title = "Select file for execution";
			OFDialog.Filter = SFDialog.Filter = ExDialog.Filter = "OS macro file (*.macro)|*.macro";
			FDialog.Filter = "All files|*.*";

			MouseX.Maximum = Screen.PrimaryScreen.Bounds.Width - 1;
			MouseY.Maximum = Screen.PrimaryScreen.Bounds.Height - 1;

			KeyModifiers.Items.Add ("No modifiers");
			KeyModifiers.Items.Add ("Shift");
			KeyModifiers.Items.Add ("Ctrl");
			KeyModifiers.Items.Add ("Ctrl + Shift");
			KeyModifiers.Items.Add ("Alt");
			KeyModifiers.Items.Add ("Alt + Shift");
			KeyModifiers.Items.Add ("Ctrl + Alt");
			KeyModifiers.Items.Add ("Ctrl + Alt + Shift");
			KeyModifiers.Items.Add ("Win key");
			KeyModifiers.SelectedIndex = 0;

			for (int i = 0; i < 256; i++)
				{
				KeyCode.Items.Add (((Keys)i).ToString () + " (" + i.ToString () + ")");
				}
			KeyCode.SelectedIndex = 0;
			}

		// Выход из программы
		private void MQuit_Click (object sender, System.EventArgs e)
			{
			this.Close ();
			}

		private void ExitButton_Click (object sender, System.EventArgs e)
			{
			this.Close ();
			}

		// Контроль выхода
		private void MainForm_FormClosing (object sender, FormClosingEventArgs e)
			{
			e.Cancel = (MessageBox.Show ("Do you want to quit application?\n\nAll unsaved data will be lost!",
				ProgramDescription.AssemblyDescription, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No);
			}

		// Загрузка макроса
		private void MOpen_Click (object sender, System.EventArgs e)
			{
			if (MessageBox.Show ("Do you want to open existing file?\n\nAll unsaved data will be lost!", ProgramDescription.AssemblyDescription,
				MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
				{
				OFDialog.ShowDialog ();
				}
			}

		private void OFDialog_FileOk (object sender, System.ComponentModel.CancelEventArgs e)
			{
			// Загрузка
			FileStream FS = null;
			try
				{
				FS = new FileStream (OFDialog.FileName, FileMode.Open);
				}
			catch
				{
				MessageBox.Show ("Cannot open file \"" + OFDialog.FileName + "\".\n\nMake sure that file is available and try again",
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
		private void MSave_Click (object sender, System.EventArgs e)
			{
			SFDialog.ShowDialog ();
			}

		private void SFDialog_FileOk (object sender, System.ComponentModel.CancelEventArgs e)
			{
			// Инициализация
			FileStream FS = null;
			try
				{
				FS = new FileStream (SFDialog.FileName, FileMode.Create);
				}
			catch
				{
				MessageBox.Show ("Cannot create file \"" + SFDialog.FileName + "\".\n\nMake sure that its placement is available and try again",
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
		private void MExecute_Click (object sender, System.EventArgs e)
			{
			ExDialog.ShowDialog ();
			}

		private void ExDialog_FileOk (object sender, System.ComponentModel.CancelEventArgs e)
			{
			if (MessageBox.Show ("During execution using of keyboard or mouse is not recommended.\n\nBegin macro?",
				ProgramDescription.AssemblyDescription, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
				{
				// Проверка существования файла
				if (!System.IO.File.Exists (Application.StartupPath + "\\OSMacrosEx.exe"))
					{
					MessageBox.Show ("OSMacrosEx (execution module) is missing. Execution is unavailable",
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
				System.Diagnostics.Process.Start (Application.StartupPath + "\\OSMacrosEx.exe", "\"" + ExDialog.FileName + "\" " +
					repeats.ToString ());
				this.WindowState = FormWindowState.Normal;
				}
			}

		// Выбор позиции указателя мыши
		private void SetMousePointer_Click (object sender, System.EventArgs e)
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
			/*if ((KeyCode.Value >= 16) && (KeyCode.Value <= 18))
				{
				KeyCode.Value = 0;
				}*/
			}

		// Выбор файла для исполнения
		private void SelectFile_Click (object sender, System.EventArgs e)
			{
			FDialog.ShowDialog ();
			}

		private void FDialog_FileOk (object sender, System.ComponentModel.CancelEventArgs e)
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
				{
				CommandsListBox.SelectedIndex = CommandsListBox.Items.Count - 1;
				}

			// Обновление кнопок
			MoveUp.Enabled = MoveDown.Enabled = (CommandsListBox.Items.Count > 1);
			DeleteItem.Enabled = (CommandsListBox.Items.Count > 0);
			}

		// Добавление команд
		private void AddMousePointer_Click (object sender, System.EventArgs e)
			{
			commands.Add (new MacroCommand ((uint)MouseX.Value, (uint)MouseY.Value));
			UpdateCommandsList ();
			}

		private void AddKeyPress_Click (object sender, System.EventArgs e)
			{
			commands.Add (new MacroCommand ((KeyModifiers)KeyModifiers.SelectedIndex, (Keys)KeyCode.SelectedIndex));
			UpdateCommandsList ();
			}

		private void AddPause_Click (object sender, System.EventArgs e)
			{
			commands.Add (new MacroCommand ((uint)PauseLength.Value));
			UpdateCommandsList ();
			}

		private void AddFileExecution_Click (object sender, System.EventArgs e)
			{
			if (CommandPath.Text == "")
				CommandPath.Text = "-";

			commands.Add (new MacroCommand (CommandPath.Text, WaitForFinish.Checked));
			UpdateCommandsList ();
			}

		private void AddLeftClick_Click (object sender, System.EventArgs e)
			{
			commands.Add (new MacroCommand (MouseCommands.LeftMouseClick));
			UpdateCommandsList ();
			}

		private void AddRightClick_Click (object sender, System.EventArgs e)
			{
			commands.Add (new MacroCommand (MouseCommands.RightMouseClick));
			UpdateCommandsList ();
			}

		private void AddDragBeginning_Click (object sender, System.EventArgs e)
			{
			commands.Add (new MacroCommand (MouseCommands.StartDragNDrop));
			UpdateCommandsList ();
			}

		private void AddDragEnding_Click (object sender, System.EventArgs e)
			{
			commands.Add (new MacroCommand (MouseCommands.FinishDragNDrop));
			UpdateCommandsList ();
			}

		// Изменение списка
		private void DeleteItem_Click (object sender, System.EventArgs e)
			{
			if (CommandsListBox.SelectedIndex >= 0)
				{
				commands.RemoveAt (CommandsListBox.SelectedIndex);
				UpdateCommandsList ();
				}
			}

		private void MoveUp_Click (object sender, System.EventArgs e)
			{
			if (CommandsListBox.SelectedIndex > 0)	// Выбрана не верхняя позиция
				{
				string command = commands[CommandsListBox.SelectedIndex].MacroFileCommandPresentation;
				int i = CommandsListBox.SelectedIndex;

				commands.RemoveAt (i);
				commands.Insert (i - 1, MacroCommand.BuildMacroCommand (command));
				UpdateCommandsList ();
				CommandsListBox.SelectedIndex = i - 1;
				}
			}

		private void MoveDown_Click (object sender, System.EventArgs e)
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
		private void MainForm_HelpButtonClicked (object sender, System.ComponentModel.CancelEventArgs e)
			{
			// Отмена обработки события вызова справки
			e.Cancel = true;

			// Отображение
			MessageBox.Show (ProgramDescription.AssemblyTitle + "\n" + ProgramDescription.AssemblyDescription + "\n" +
				ProgramDescription.AssemblyCopyright + "\n" + ProgramDescription.AssemblyLastUpdate + "\n\n" +
				"Application may be used for imitation of user activities when:\n" +
				" • you need to perform many similar operations with files and/or programs;\n" +
				" • you cannot automate these activities using standard tools;\n" +
				" • some activities must be performed without your direct participation.\n" +
				"Application can 'use' mouse, keyboard, command line and interrupts as same as user with the same potentialities",
				ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}

		// Установка количества запусков макроса
		private void ExecutionRepeats_TextChanged (object sender, System.EventArgs e)
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
