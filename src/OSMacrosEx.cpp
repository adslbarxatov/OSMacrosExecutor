/////////////////////////////////////////////////////////////////////////////////////
// Подключение заголовочных файлов
#include "OSMacrosEx.h"

/////////////////////////////////////////////////////////////////////////////////////
// Переменные
schar commands[DEFAULTSIZE][DEFAULTSIZE];

/////////////////////////////////////////////////////////////////////////////////////
// Главная функция
int main (int argc, char* argv[])
	{
	// Переменные
	schar FileName[DEFAULTSIZE];	// Имя файла макроса
	schar str[DEFAULTSIZE];			// Считанная из файла строка команды
	uint repetitions = 0;			// Количество повторов
	uint linePause = 0;				// Пауза между строками

	uchar cycleFinisher = 0;		// Финализатор цикла
	uint currentCycle = 0,			// Текущая итерация цикла
		cycles = 0;					// Количество итераций цикла
	uint currentCycleLine = 0,		// Текущая строка блока цикла
		cycleLines = 0;				// Количество строк блока цикла
	uchar cyclePhase = 0;			// Фаза выполнения цикла

	ulong i, x, y, x2, y2;			// Буферные переменные
	ulong r, g, b;					// Дескрипторы для запроса цветов пикселей экрана

	HDC screenDC = NULL;			// Дескрипторы скриншотов
	HDC memDC = NULL;
	HBITMAP memBitmap;
	COLORREF color;

	FILE *F1;						// Файл макроса
	uint commandsQuantity = 0;		// Число команд (если есть в файле)

	HRESULT hr;						// Дескрипторы индикатора прогресса
	ITaskbarList3 *pTaskbarList = NULL;
	HWND hwnd = NULL;
	uint currentCommand;

	// Заголовок
	printf ("\n \x0F " OSME_PRODUCT " \x0F \n   by  " OSME_COMPANY "\n\n");
	system ("title " OSME_PRODUCT);

	// XPUN-контроль
	if (!CheckXPUNClass ())
		return -170;

	// Проверка корректности вызова программы
	if (argc < 2)
		{
		printf (" \x10 Usage: OSMacrosEx <full_path_to_macro> [number_of_repetitions] [pause_between_lines]\n");
		printf ("   - number of repetitions - value between %u and %u, default is %u\n",
			REPETITIONS_MIN, REPETITIONS_MAX, REPETITIONS_DEFAULT);
		printf ("   - pause between lines (in milliseconds) - value between %u and %u, default is %u\n\n",
			LINE_PAUSE_MIN, LINE_PAUSE_MAX, LINE_PAUSE_DEFAULT);

		printf (" \x10 €бЇ®«м§®ў ­ЁҐ: OSMacrosEx <Ї®«­л©_Їгвм_Є_¬ Єа®бг> [зЁб«®_Ї®ўв®а®ў] [Ї г§ _¬Ґ¦¤г_бва®Є ¬Ё]\n");
		printf ("   - зЁб«® Ї®ўв®а®ў - §­ зҐ­ЁҐ ¬Ґ¦¤г %u Ё %u, Ї® г¬®«з ­Ёо - %u\n",
			REPETITIONS_MIN, REPETITIONS_MAX, REPETITIONS_DEFAULT);
		printf ("   - Ї г§  ¬Ґ¦¤г бва®Є ¬Ё (ў ¬Ё««ЁбҐЄг­¤ е) - §­ зҐ­ЁҐ ¬Ґ¦¤г %u Ё %u, Ї® г¬®«з ­Ёо - %u\n\n",
			LINE_PAUSE_MIN, LINE_PAUSE_MAX, LINE_PAUSE_DEFAULT);

		return -1;
		}
	sprintf (FileName, argv[1]);

	// Получение количества повторов
	if (argc > 2)
		sscanf (argv[2], "%u", &repetitions);

	if (repetitions == 0)
		repetitions = REPETITIONS_DEFAULT;
	else if (repetitions < REPETITIONS_MIN)
		repetitions = REPETITIONS_MIN;
	else if (repetitions > REPETITIONS_MAX)
		repetitions = REPETITIONS_MAX;

	// Получение паузы между строками
	if (argc > 3)
		sscanf (argv[3], "%u", &linePause);

	if (linePause == 0)
		linePause = LINE_PAUSE_DEFAULT;
	else if (linePause < LINE_PAUSE_MIN)
		linePause = LINE_PAUSE_MIN;
	else if (linePause > LINE_PAUSE_MAX)
		linePause = LINE_PAUSE_MAX;

	// Инициализация прогресс-индикации
	hr = CoInitialize (NULL);
	if (hr >= 0)
		{
		hr = CoCreateInstance (CLSID_TaskbarList, NULL, CLSCTX_INPROC_SERVER,
			IID_ITaskbarList3, (void **)&pTaskbarList);

		if (hr >= 0)
			{
			pTaskbarList->HrInit ();

			// Признак успешной инициализации, используемый далее
			hwnd = GetConsoleWindow ();
			}
		}

	// Открытие файла
	if ((F1 = fopen (FileName, "r")) == NULL)
		{
		printf (" \x13 Specified file cannot be opened\n\n");
		return -2;
		}

	// Вывод указанных настроек
	printf (" \x10 Starting [%s]\n   with NoR = %u and PBL = %u...\n\n",
		FileName, repetitions, linePause);

	// Запуск на выполнение
	for (i = 0; i < repetitions; i++)
		{
		cycleFinisher = 0;
		currentCommand = 0;
		printf (" \x10 Repetition #%u started\n", i + 1);

		// Второе условие вместе с инкрементом сработает только в конце файла
		while (fgets (str, DEFAULTSIZE, F1) || !cycleFinisher++)
			{
			// Предохранители
			if (cycleFinisher)
				sprintf (str, "-");
			if (str[strlen (str) - 1] == '\n')
				str[strlen (str) - 1] = '\x0';

cycle:
			switch (str[0])
				{
				// Запуск программы / файла без ожидания завершения
				case CMD_RUN_APP_NO_WAIT:
					sprintf (ExecutionCommand, "start \"\" %s", str + 2);
					_EXEC;
					break;

				// Установка курсора в позицию экрана
				case CMD_SET_POSITION:
					sscanf (str + 2, "%u %u", &x, &y);
					_MOVETO (x, y);
					break;

				// Одинарные щелчки мыши
				case CMD_CLICK_L:
					_LDOWN;
					_LUP;
					break;

				case CMD_CLICK_R:
					_RCLICK;
					break;

				// Нажатие клавиш клавиатуры
				case CMD_KEY_DOWN:
					sscanf (str + 2, "%u %u", &x, &y);
					switch (x)
						{
						case 0:
							_ONEKEY (y);
							break;
						case 1:
							_DBLKEY (VK_SHIFT, y);
							break;
						case 2:
							_DBLKEY (VK_CONTROL, y);
							break;
						case 3:
							_TRPLKEY (VK_CONTROL, VK_SHIFT, y);
							break;
						case 4:
							_DBLKEY (VK_MENU, y);
							break;
						case 5:
							_TRPLKEY (VK_SHIFT, VK_MENU, y);
							break;
						case 6:
							_TRPLKEY (VK_CONTROL, VK_MENU, y);
							break;
						case 7:
							_QDRKEY (VK_CONTROL, VK_SHIFT, VK_MENU, y);
							break;
						case 8:
							_DBLKEY (VK_LWIN, y);
							break;
						}
					break;

				// Пауза
				case CMD_PAUSE:
					sscanf (str + 2, "%u", &x);
					_PAUSE (x);
					break;

				// Begin dragging
				case CMD_DRAG_START:
					_LDOWN;
					break;

				// End dragging
				case CMD_DRAG_END:
					_LUP;
					break;

				// Выполнение команды с ожиданием завершения
				case CMD_RUN_APP_WAIT:
					sprintf (ExecutionCommand, "%s", str + 2);
					_EXEC;
					break;

				// Выполнение команды ожидания изменения цвета пикселя
				case CMD_EXP_PIX_COLOR:
					sscanf (str + 2, "%u %u %u %u %u %u %u", &x, &y, &r, &g, &b, &x2, &y2);
					if ((x2 * y2 == 0) || (x >= x2) || (y >= y2) ||
						(r > 0xFF) || (g > 0xFF) || (b > 0xFF))
						break;

					if (!screenDC)
						{
						screenDC = GetDC (0);
						memDC = CreateCompatibleDC (screenDC);
						memBitmap = CreateCompatibleBitmap (screenDC, x2, y2);
						SelectObject (memDC, memBitmap);
						}

					do {
						BitBlt (memDC, 0, 0, x2, y2, screenDC, 0, 0, SRCCOPY);
						color = GetPixel (memDC, x, y);
						_PAUSE (500);
						} while ((((color >> 16) & 0xFF) == b) && (((color >> 8) & 0xFF) == g) &&
							((color & 0xFF) == r));
						break;

				// Запуск и остановка цикла
				case CMD_CYCLE_START:
					// Защита от вложенности
					if (cyclePhase == 1)
						continue;

					// Одна или менее итерация уже выполнены
					sscanf (str + 2, "%u", &cycles);
					if (cycles < 2)
						continue;

					// Начало наполнения цикла
					cyclePhase = 1;
					printf (" \x10  Cycle started\n");
					continue;

				case CMD_CYCLE_END:
					// Защита от неправильного порядка
					if (cyclePhase == 0)
						continue;

					// Запуск цикла
					currentCycleLine = 0;
					currentCycle = 1;	// Одна итерация уже выполнена
					cyclePhase = 2;

					printf (" \x10  Round %u\n", currentCycle + 1);
					break;

				// Служебные строки
				case PCMD_COMMANDS_QNT:
					sscanf (str + 2, "%u", &commandsQuantity);
					continue;

				// Неизвестная команда
				default:
					printf (" \x13   Command [%s] ignored - unknown command code\n", str);
					continue;
				}

			// Обязательная пауза между командами
			_PAUSE (linePause);

			// Отображение прогресса
			currentCommand++;
			if (hwnd && (commandsQuantity > 0))
				{
				pTaskbarList->SetProgressValue (hwnd, currentCommand, commandsQuantity);
				pTaskbarList->SetProgressState (hwnd, TBPF_NORMAL);
				}

			// Наполнение цикла
			if (cyclePhase == 1)
				{
				sprintf (commands[cycleLines], "%s", str);
				cycleLines++;
				}

			// Отображение ответа
			if (str[0] != CMD_CYCLE_END)
				printf (" \x10   Command [%s] executed\n", str);

			// Выполнение цикла
			if (cyclePhase == 2)
				{
				// Переход к следующей итерации
				if (currentCycleLine >= cycleLines)
					{
					currentCycleLine = 0;
					currentCycle++;

					// Завершение цикла
					if (currentCycle >= cycles)
						{
						cyclePhase = 0;
						cycleLines = currentCycle = cycles = 0;
						printf (" \x10  Cycle finished\n");
						continue;
						}
					else
						{
						printf (" \x10  Round %u\n", currentCycle + 1);
						}
					}

				// Переход к команде
				sprintf (str, "%s", commands[currentCycleLine++]);
				goto cycle;
				}
			}

		// Возврат в начало файла
		fseek (F1, 0, SEEK_SET);
		}

	// Завершение
	printf (" \x0F Execution completed\n\n");
	fclose (F1);

	if (screenDC)
		{
		DeleteObject (memBitmap);
		DeleteDC (memDC);
		ReleaseDC (0, screenDC);
		}

	if (hwnd)
		{
		pTaskbarList->SetProgressState (hwnd, TBPF_NOPROGRESS);
		pTaskbarList->Release ();

		CoUninitialize ();
		}
	}
