using System;
using System.Drawing;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает форму выбора позиции указателя мыши
	/// </summary>
	public partial class MousePointerSelector: Form
		{
		/// <summary>
		/// Конструктор. Инициализирует форму
		/// </summary>
		public MousePointerSelector ()
			{
			InitializeComponent ();

			this.Left = this.Top = 0;
			this.Width = Screen.PrimaryScreen.Bounds.Width;
			this.Height = Screen.PrimaryScreen.Bounds.Height;

			// Запуск
			this.ShowDialog ();
			}

		// Выбор позиции и закрытие окна
		private void MousePointerSelector_MouseClick (object sender, MouseEventArgs e)
			{
			mouseX = (uint)e.X;
			mouseY = (uint)e.Y;
			pixelColor = ((Bitmap)this.BackgroundImage).GetPixel (e.X, e.Y);

			this.BackgroundImage.Dispose ();
			this.Close ();
			}

		/// <summary>
		/// Координата X указателя мыши
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
		/// Координата Y указателя мыши
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
		/// Возвращает цвет пикселя в месте установки указателя
		/// </summary>
		public Color PixelColor
			{
			get
				{
				return pixelColor;
				}
			}
		private Color pixelColor = Color.FromArgb (0, 0, 0);

		// Получение снимка экрана
		private void MousePointerSelector_Load (object sender, EventArgs e)
			{
			Bitmap b = new Bitmap (Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
			Graphics g = Graphics.FromImage (b);

			g.CopyFromScreen (0, 0, 0, 0, b.Size);
			g.Dispose ();
			this.BackgroundImage = b;
			}
		}
	}
