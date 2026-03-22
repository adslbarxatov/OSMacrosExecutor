using System;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает главную форму программы
	/// </summary>
	public partial class OSMacrosSettings: Form
		{
		/// <summary>
		/// Конструктор. Запускает главную форму
		/// </summary>
		public OSMacrosSettings ()
			{
			// Инициализация
			InitializeComponent ();
			RDGenerics.LoadWindowDimensions (this);
			this.Text = RDLocale.GetText ("MSettingsText");

			AcceptButton = BApply;
			BApply.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Apply);

			CancelButton = BCancel;
			BCancel.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel);

			AlwaysRequestMacroPathRadio.Text = RDLocale.GetText ("AlwaysRequestMacroPathRadio");
			UseMacroPathRadio.Text = RDLocale.GetText ("UseMacroPathRadio");
			if (string.IsNullOrWhiteSpace (DefaultMacroPath))
				{
				AlwaysRequestMacroPathRadio.Checked = true;
				}
			else
				{
				UseMacroPathRadio.Checked = true;
				BackupsPathField.Text = DefaultMacroPath;
				}

			LinePauseLabel.Text = RDLocale.GetText ("LinePauseLabel");
			LinePauseField.Minimum = LinePauseMinimum;
			LinePauseField.Maximum = LinePauseMaximum;
			try
				{
				LinePauseField.Value = LinePause;
				}
			catch
				{
				LinePauseField.Value = LinePauseDefault;
				}

			FBDialog.Description = RDLocale.GetText ("DefaultMacroPathTitle");

			// Запуск
			this.ShowDialog ();
			}

		// Закрытие окна
		private void OSMacrosSettings_FormClosing (object sender, FormClosingEventArgs e)
			{
			RDGenerics.SaveWindowDimensions (this);
			}

		// Применение или отмена настроек
		private void BApply_Click (object sender, EventArgs e)
			{
			if (AlwaysRequestMacroPathRadio.Checked || string.IsNullOrWhiteSpace (BackupsPathField.Text))
				{
				DefaultMacroPath = "";
				}
			else
				{
				if (!BackupsPathField.Text.EndsWith ('\\'))
					BackupsPathField.Text += "\\";

				DefaultMacroPath = BackupsPathField.Text;
				}

			LinePause = (uint)LinePauseField.Value;

			this.Close ();
			}

		private void BCancel_Click (object sender, EventArgs e)
			{
			this.Close ();
			}

		// Выбор пути сохранения
		private void SelectBackupsPath_Click (object sender, EventArgs e)
			{
			try
				{
				FBDialog.SelectedPath = BackupsPathField.Text;
				}
			catch { }

			if (FBDialog.ShowDialog () == DialogResult.OK)
				BackupsPathField.Text = FBDialog.SelectedPath;
			}

		// Переключение режима сохранения
		private void UseMacroPathRadio_CheckedChanged (object sender, EventArgs e)
			{
			BackupsPathField.Enabled = SelectBackupsPath.Enabled = UseMacroPathRadio.Checked;
			}

		/// <summary>
		/// Возвращает или задаёт путь хранения макросов по умолчанию
		/// </summary>
		public static string DefaultMacroPath
			{
			get
				{
				return RDGenerics.GetSettings (defaultMacroPathPar, "");
				}
			set
				{
				RDGenerics.SetSettings (defaultMacroPathPar, value);
				}
			}
		private const string defaultMacroPathPar = "DefaultMacroPath";

		/// <summary>
		/// Возвращает или задаёт паузу между отдельными командами в миллисекундах
		/// </summary>
		public static uint LinePause
			{
			get
				{
				return RDGenerics.GetSettings (linePausePar, 100);
				}
			set
				{
				RDGenerics.SetSettings (linePausePar, value);
				}
			}
		private const string linePausePar = "LinePause";

		/// <summary>
		/// Возвращает минимально возможное число повторов макроса
		/// </summary>
		public const uint RepetitionsMinimum = 1;

		/// <summary>
		/// Возвращает максимально возможное число повторов макроса
		/// </summary>
		public const uint RepetitionsMaximum = 100;

		/// <summary>
		/// Возвращает число повторов макроса по умолчанию
		/// </summary>
		public const uint RepetitionsDefault = 1;

		/// <summary>
		/// Возвращает минимально возможную паузу между командами
		/// </summary>
		public const uint LinePauseMinimum = 50;

		/// <summary>
		/// Возвращает максимально возможную паузу между командами
		/// </summary>
		public const uint LinePauseMaximum = 5000;

		/// <summary>
		/// Возвращает паузу между командами по умолчанию
		/// </summary>
		public const uint LinePauseDefault = 100;
		}
	}
