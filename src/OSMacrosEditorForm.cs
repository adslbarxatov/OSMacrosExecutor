using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
		private List<MacroCommand> commands = [];
		private bool warningShown = false;

		/// <summary>
		/// Конструктор главной формы программы
		/// </summary>
		/// <param name="MacroFile">Имя файла для загрузки при старте программы</param>
		public OSMacrosEditorForm (string MacroFile)
			{
			// Инициализация
			InitializeComponent ();

			/*LanguageCombo.Items.AddRange (RDLocale.LanguagesNames);
			try
				{
				LanguageCombo.SelectedIndex = (int)RDLocale.CurrentLanguage;
				}
			catch
				{
				LanguageCombo.SelectedIndex = 0;
				}*/

			// Настройка контролов
			this.Text = RDGenerics.DefaultAssemblyVisibleName;
			RDGenerics.LoadWindowDimensions (this);
			LocalizeForm ();

			MouseX.Maximum = Screen.PrimaryScreen.Bounds.Width - 1;
			MouseY.Maximum = Screen.PrimaryScreen.Bounds.Height - 1;

			for (int i = 0; i < 256; i++)
				KeyCode.Items.Add (((Keys)i).ToString () + " (" + i.ToString () + ")");
			KeyCode.SelectedIndex = 0;

			// Загрузка файла, если требуется
			/*if (MacroFile != "")*/
			if (!string.IsNullOrWhiteSpace (MacroFile))
				{
				OFDialog.FileName = MacroFile;
				OFDialog_FileOk (null, null);
				}
			}

		// Локализация формы
		private void LocalizeForm ()
			{
			// Локализация
			OFDialog.Title = RDLocale.GetText ("OFDialogTitle");
			SFDialog.Title = RDLocale.GetText ("SFDialogTitle");
			FDialog.Title = RDLocale.GetText ("FDialogTitle");
			OFDialog.Filter = string.Format (RDLocale.GetText ("OFDialogFilter"),
				ProgramDescription.NewAppExtension, "macro");
			SFDialog.Filter = /*ExDialog.Filter =*/
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
			MExecuteCurrent.Text = RDLocale.GetText ("MExecuteCurrentText");
			MSettings.Text = RDLocale.GetText ("MSettingsText");
			MLanguage.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Control_InterfaceLanguageNC);

			MHelp.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Control_AppAbout);
			MQuit.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Exit);

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

			CycleGroup.Text = RDLocale.GetText ("CycleGroupText");
			BeginCycle.Text = RDLocale.GetText ("BeginCycleText");
			EndCycle.Text = RDLocale.GetText ("EndCycleText");
			CycleLabel.Text = RDLocale.GetText ("CycleLabelText");

			UpdateCommandsList (false);
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
				(RDInterface.LocalizedMessageBox (RDMessageFlags.Warning | RDMessageFlags.CenterText,
				"QuitApplication", RDLDefaultTexts.Button_Yes,
				RDLDefaultTexts.Button_No) == RDMessageButtons.ButtonTwo);

			RDGenerics.SaveWindowDimensions (this);
			}

		// Загрузка макроса
		private void MOpen_Click (object sender, EventArgs e)
			{
			if ((CommandsListBox.Items.Count == 0) ||
				(RDInterface.LocalizedMessageBox (RDMessageFlags.Warning | RDMessageFlags.CenterText,
				"OpenExistingFile", RDLDefaultTexts.Button_Yes,
				RDLDefaultTexts.Button_No) == RDMessageButtons.ButtonOne))
				{
				OFDialog.ShowDialog ();
				}
			}

		private void OFDialog_FileOk (object sender, CancelEventArgs e)
			{
			// Загрузка
			FileStream FS;
			try
				{
				FS = new FileStream (OFDialog.FileName, FileMode.Open);
				}
			catch
				{
				RDInterface.MessageBox (RDMessageFlags.Warning | RDMessageFlags.CenterText,
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

			UpdateCommandsList (false);
			}

		// Сохранение макроса
		private void MSave_Click (object sender, EventArgs e)
			{
			SFDialog.ShowDialog ();
			}

		private void SFDialog_FileOk (object sender, CancelEventArgs e)
			{
			// Инициализация
			FileStream FS;
			try
				{
				FS = new FileStream (SFDialog.FileName, FileMode.Create);
				}
			catch
				{
				RDInterface.MessageBox (RDMessageFlags.Warning | RDMessageFlags.CenterText,
					string.Format (RDLocale.GetDefaultText (RDLDefaultTexts.Message_SaveFailure_Fmt),
					SFDialog.FileName));
				return;
				}
			StreamWriter SW = new StreamWriter (FS, RDGenerics.GetEncoding (RDEncodings.CP1251));

			// Запись
			SW.WriteLine (MacroCommand.CommandsQuantityAlias + " " + commands.Count.ToString ());

			for (int i = 0; i < commands.Count; i++)
				SW.WriteLine (commands[i].MacroFileCommandPresentation);

			// Завершение
			SW.Close ();
			FS.Close ();
			}

		// Выполнение макроса
		private void MExecuteCurrent_Click (object sender, EventArgs e)
			{
			// Выбор варианта хранения файла для запуска
			if (string.IsNullOrWhiteSpace (OSMacrosSettings.DefaultMacroPath))
				{
				if (SFDialog.ShowDialog () != DialogResult.OK)
					return;
				}
			else
				{
				SFDialog.FileName = OSMacrosSettings.DefaultMacroPath + DateTime.Now.ToString ("dd-MM-yyyy HH-mm") +
					"." + ProgramDescription.NewAppExtension;
				SFDialog_FileOk (null, null);
				}

			/*ExDialog.FileName = SFDialog.FileName;
			ExDialog_FileOk (null, null);
			}

		// Запуск
		private void ExDialog_FileOk (object sender, CancelEventArgs e)
			{*/

			// Контроль
			if (!warningShown && RDInterface.LocalizedMessageBox (RDMessageFlags.Warning | RDMessageFlags.CenterText,
				"BeginMacro", RDLDefaultTexts.Button_Yes, RDLDefaultTexts.Button_No) !=
				RDMessageButtons.ButtonOne)
				return;
			warningShown = true;

			// Обработка количества выполнений
			string repeats = RDInterface.MessageBox (string.Format (RDLocale.GetText ("RepeatsMessage"),
				OSMacrosSettings.RepetitionsMinimum, OSMacrosSettings.RepetitionsMaximum),
				true, (uint)OSMacrosSettings.RepetitionsMaximum.ToString ().Length, OSMacrosSettings.RepetitionsDefault.ToString ());
			if (string.IsNullOrWhiteSpace (repeats))
				return;

			uint r;
			try
				{
				r = uint.Parse (repeats);

				if (r < OSMacrosSettings.RepetitionsMinimum)
					r = OSMacrosSettings.RepetitionsMinimum;
				else if (r > OSMacrosSettings.RepetitionsMaximum)
					r = OSMacrosSettings.RepetitionsMaximum;
				}
			catch
				{
				r = OSMacrosSettings.RepetitionsDefault;
				}

			// Запуск
			this.SendToBack ();

			Process p = Process.Start (RDGenerics.AppStartupPath + ProgramDescription.AssemblyExecutionModule,
				"\"" + SFDialog.FileName + "\" " + r.ToString () + " " + OSMacrosSettings.LinePause.ToString ());
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
		private void UpdateCommandsList (bool AddNew)
			{
			int idx = CommandsListBox.SelectedIndex;
			if (idx < 0)
				idx = 0;

			// Обновление списка
			CommandsListBox.Items.Clear ();
			for (int i = 0; i < commands.Count; i++)
				CommandsListBox.Items.Add (commands[i].CommandPresentation);

			if (CommandsListBox.Items.Count > 0)
				{
				if (AddNew)
					{
					CommandsListBox.SelectedIndex = CommandsListBox.Items.Count - 1;
					}
				else
					{
					try
						{
						CommandsListBox.SelectedIndex = idx;
						}
					catch
						{
						CommandsListBox.SelectedIndex = CommandsListBox.Items.Count - 1;
						}
					}
				}

			// Обновление кнопок
			MoveUp.Enabled = MoveDown.Enabled = (CommandsListBox.Items.Count > 1);
			DeleteItem.Enabled = (CommandsListBox.Items.Count > 0);
			}

		// Добавление команд
		private void AddCommand (MacroCommand MC)
			{
			// Число команд больше одной, выбранный индекс неотрицателен, и выбранная позиция не последняя
			int lineIndex = CommandsListBox.SelectedIndex;
			bool canInsert = (commands.Count > 1) && (lineIndex >= 0) && (lineIndex < commands.Count - 1);

			if (canInsert)
				commands.Insert (lineIndex + 1, MC);
			else
				commands.Add (MC);

			UpdateCommandsList (!canInsert);
			}

		private void AddMousePointer_Click (object sender, EventArgs e)
			{
			AddCommand (new MacroCommand ((uint)MouseX.Value, (uint)MouseY.Value));
			/*UpdateCommandsList ();*/
			}

		private void AddKeyPress_Click (object sender, EventArgs e)
			{
			AddCommand (new MacroCommand ((KeyModifiers)KeyModifiers.SelectedIndex, (Keys)KeyCode.SelectedIndex));
			/*UpdateCommandsList ();*/
			}

		private void AddPause_Click (object sender, EventArgs e)
			{
			AddCommand (new MacroCommand ((uint)PauseLength.Value));
			/*UpdateCommandsList ();*/
			}

		private void AddFileExecution_Click (object sender, EventArgs e)
			{
			if (CommandPath.Text == "")
				CommandPath.Text = "-";

			AddCommand (new MacroCommand (CommandPath.Text, WaitForFinish.Checked));
			/*UpdateCommandsList ();*/
			}

		private void AddLeftClick_Click (object sender, EventArgs e)
			{
			AddCommand (new MacroCommand (MouseCommands.LeftMouseClick));
			/*UpdateCommandsList ();*/
			}

		private void AddRightClick_Click (object sender, EventArgs e)
			{
			AddCommand (new MacroCommand (MouseCommands.RightMouseClick));
			/*UpdateCommandsList ();*/
			}

		private void AddDragBeginning_Click (object sender, EventArgs e)
			{
			AddCommand (new MacroCommand (MouseCommands.StartDragNDrop));
			/*UpdateCommandsList ();*/
			}

		private void AddDragEnding_Click (object sender, EventArgs e)
			{
			AddCommand (new MacroCommand (MouseCommands.FinishDragNDrop));
			/*UpdateCommandsList ();*/
			}

		private void BeginCycle_Click (object sender, EventArgs e)
			{
			AddCommand (new MacroCommand (true, (uint)CycleRounds.Value));
			}

		private void EndCycle_Click (object sender, EventArgs e)
			{
			AddCommand (new MacroCommand (false, 0));
			}

		private void AddWaitForColor_Click (object sender, EventArgs e)
			{
			AddCommand (new MacroCommand ((uint)MouseX.Value, (uint)MouseY.Value, SetPixelColor.BackColor));
			}

		// Изменение списка
		private void DeleteItem_Click (object sender, EventArgs e)
			{
			/*if (CommandsListBox.SelectedIndex >= 0)
				{*/
			if (CommandsListBox.SelectedIndex < 0)
				return;

			commands.RemoveAt (CommandsListBox.SelectedIndex);
			UpdateCommandsList (false);
			/*}*/
			}

		private void MoveUp_Click (object sender, EventArgs e)
			{
			// Выбрана не верхняя позиция
			int i = CommandsListBox.SelectedIndex;
			if (i <= 0)
				return;

			string command = commands[i].MacroFileCommandPresentation;

			commands.RemoveAt (i);
			commands.Insert (i - 1, MacroCommand.BuildMacroCommand (command));
			UpdateCommandsList (false);
			CommandsListBox.SelectedIndex = i - 1;
			}

		private void MoveDown_Click (object sender, EventArgs e)
			{
			// Выбрана не нижняя позиция
			int i = CommandsListBox.SelectedIndex;
			if ((i < 0) || (i >= CommandsListBox.Items.Count - 1))
				return;

			string command = commands[i].MacroFileCommandPresentation;

			commands.RemoveAt (i);
			commands.Insert (i + 1, MacroCommand.BuildMacroCommand (command));
			UpdateCommandsList (false);
			CommandsListBox.SelectedIndex = i + 1;
			/*}*/
			}

		// Отображение краткой справочной информации
		private void MHelp_Click (object sender, EventArgs e)
			{
			RDInterface.ShowAbout (false);
			}

		// Ручное изменение цвета пикселя
		private void SetPixelColor_Click (object sender, EventArgs e)
			{
			CDialog.Color = SetPixelColor.BackColor;
			if (CDialog.ShowDialog () == DialogResult.OK)
				SetPixelColor.BackColor = CDialog.Color;
			}

		// Вызов окна настроек
		private void MSettings_Click (object sender, EventArgs e)
			{
			_ = new OSMacrosSettings ();
			}

		// Выбор языка интерфейса
		private void MLanguage_Click (object sender, EventArgs e)
			{
			if (RDInterface.MessageBox ())
				LocalizeForm ();
			}
		}
	}
