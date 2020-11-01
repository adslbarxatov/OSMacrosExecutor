/////////////////////////////////////////////////////////////////////////////////////
// Подключение заголовочных файлов
#include "OSMacrosEx.h"

/////////////////////////////////////////////////////////////////////////////////////
// Переменные
FILE *F1;
schar commands[DEFAULTSIZE][DEFAULTSIZE];

/////////////////////////////////////////////////////////////////////////////////////
// Главная функция
int main (int argc, char* argv[])
	{
	// Переменные
	schar FileName[DEFAULTSIZE];	// Имя файла макроса
	schar str[DEFAULTSIZE];			// Считанная из файла строка команды
	uint repeats = 0;				// Количество повторов
	uchar cycleFinisher = 0;		// Финализатор цикла
	uint currentCycle = 0,			// Текущая итерация цикла
		cycles = 0;					// Количество итераций цикла
	uint currentCycleLine = 0,		// Текущая строка блока цикла
		cycleLines = 0;				// Количество строк блока цикла
	uchar cyclePhase = 0;			// Фаза выполнения цикла
	ulong i, x, y;					// Буферные переменные

	// Заголовок
	printf ("\n %s\n by %s\n\n", OSME_PRODUCT, OSME_COMPANY);

	// Проверка корректности вызова программы
	if (argc < 2)
		{
		printf (" \x13 Usage: OSMacrosEx <FullPathToMacroFile> [CountOfRepeats]\n\n");
		_EXITONERROR (-1)
		}
	sprintf (FileName, argv[1]);

	// Получение количества повторов
	if (argc > 2)
		sscanf (argv[2], "%u", &repeats);

	if (repeats < 1)
		repeats = 1;

	// Открытие файла
	if ((F1 = fopen (FileName, "r")) == NULL)
		{
		printf (" \x13 Specified file cannot be opened\n\n");
		_EXITONERROR (-2)
		}

	// Запуск на выполнение
	for (i = 0; i < repeats; i++)
		{
		cycleFinisher = 0;

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
				case '0':
					sprintf (ExecutionCommand, "start \"\" %s", str + 2);
					_EXEC
					break;

				// Установка курсора в позицию экрана
				case '1':
					sscanf (str + 2, "%u %u", &x, &y);
					_MOVETO (x, y)
					break;

				// Одинарные щелчки мыши
				case '2':
					_LDOWN
					_LUP
					break;

				case '3':
					_RCLICK
					break;

				// Нажатие клавиш клавиатуры
				case '4':
					sscanf (str + 2, "%u %u", &x, &y);
					switch (x)
						{
						case 0:
							_ONEKEY (y)
							break;
						case 1:
							_DBLKEY (VK_SHIFT, y)
							break;
						case 2:
							_DBLKEY (VK_CONTROL, y)
							break;
						case 3:
							_TRPLKEY (VK_CONTROL, VK_SHIFT, y)
							break;
						case 4:
							_DBLKEY (VK_MENU, y)
							break;
						case 5:
							_TRPLKEY (VK_SHIFT, VK_MENU, y)
							break;
						case 6:
							_TRPLKEY (VK_CONTROL, VK_MENU, y)
							break;
						case 7:
							_QDRKEY (VK_CONTROL, VK_SHIFT, VK_MENU, y)
							break;
						case 8:
							_DBLKEY (VK_LWIN, y)
							break;
						}
					break;

				// Пауза
				case '5':
					sscanf (str + 2, "%u", &x);
					_PAUSE (x)
						break;

				// Begin dragging
				case '6':
					_LDOWN
						break;

				// End dragging
				case '7':
					_LUP
						break;

				// Выполнение команды с ожиданием завершения
				case '8':
					sprintf (ExecutionCommand, "%s", str + 2);
					_EXEC
						break;

				// Запуск и остановка цикла
				case '+':
					// Защита от вложенности
					if (cyclePhase == 1)
						continue;

					// Одна или менее итерация уже выполнены
					sscanf (str + 2, "%u", &cycles);
					if (cycles < 2)
						continue;

					// Начало наполнения цикла
					cyclePhase = 1;
					printf (" \x10 Cycle started\n");
					continue;

				case '-':
					// Защита от неправильного порядка
					if (cyclePhase == 0)
						continue;

					// Запуск цикла
					currentCycleLine = 0;
					currentCycle = 1;	// Одна итерация уже выполнена
					cyclePhase = 2;

					printf (" \x10 Round %u\n", currentCycle + 1);
					break;

				// Неизвестная команда
				default:
					printf (" \x13 Command [%s] ignored (unknown command code)\n", str);
					continue;
				}

			// Обязательная пауза между командами
			_PAUSE (1)

			// Наполнение цикла
			if (cyclePhase == 1)
				{
				sprintf (commands[cycleLines], "%s", str);
				cycleLines++;
				}

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
						printf (" \x10 Cycle finished\n");
						continue;
						}
					else
						{
						printf (" \x10 Round %u\n", currentCycle + 1);
						}
					}

				// Переход к команде
				sprintf (str, "%s", commands[currentCycleLine++]);
				goto cycle;
				}

			// Отображение ответа
			printf (" \x10 Command [%s] executed\n", str);
			}

		// Возврат в начало файла
		fseek (F1, 0, SEEK_SET);
		}

	// Завершение
	printf (" \x0F Execution completed\n\n");
	}
