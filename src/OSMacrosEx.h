/////////////////////////////////////////////////////////////////////////////////////
// Снятие предупреждений о безопасности при отладке
#define _CRT_SECURE_NO_WARNINGS

// Подключение заголовочных файлов
#include <stdio.h>		// Базовая библиотека ввода / вывода
#include <conio.h>		// Работа с консолью
#include <windows.h>	// Команды управления
#include <process.h>	// Библиотека запуска процессов

// Константы
#define DEFAULTSIZE	501

/////////////////////////////////////////////////////////////////////////////////////
// Макросы
	// Управление
#define _PAUSE(p)		Sleep (p);
#define _EXITONERROR(c)	/*_getch ();*/ return c;
char ExecutionCommand[DEFAULTSIZE];
#define _EXEC			system (ExecutionCommand);

	// Мышь
#define _MOVETO(x,y)	mouse_event (MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, x, y, 0, 0);
#define _LDOWN			mouse_event (MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
#define _LUP			mouse_event (MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
#define _RCLICK			mouse_event (MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);	\
						mouse_event (MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);

	// Клавиатура
#define _ONEKEY(k)		keybd_event (k, 0, 0, 0);							\
						keybd_event (k, 0, KEYEVENTF_KEYUP, 0);
#define _DBLKEY(k1,k2)	keybd_event (k1, 0, 0, 0);							\
						keybd_event (k2, 0, 0, 0);							\
						keybd_event (k1, 0, KEYEVENTF_KEYUP, 0);			\
						keybd_event (k2, 0, KEYEVENTF_KEYUP, 0);
#define _TRPLKEY(k1,k2,k3)	keybd_event (k1, 0, 0, 0);						\
							keybd_event (k2, 0, 0, 0);						\
							keybd_event (k3, 0, 0, 0);						\
							keybd_event (k1, 0, KEYEVENTF_KEYUP, 0);		\
							keybd_event (k2, 0, KEYEVENTF_KEYUP, 0);		\
							keybd_event (k3, 0, KEYEVENTF_KEYUP, 0);

#define _QDRKEY(k1,k2,k3,k4)	keybd_event (k1, 0, 0, 0);					\
								keybd_event (k2, 0, 0, 0);					\
								keybd_event (k3, 0, 0, 0);					\
								keybd_event (k4, 0, 0, 0);					\
								keybd_event (k1, 0, KEYEVENTF_KEYUP, 0);	\
								keybd_event (k2, 0, KEYEVENTF_KEYUP, 0);	\
								keybd_event (k3, 0, KEYEVENTF_KEYUP, 0);	\
								keybd_event (k4, 0, KEYEVENTF_KEYUP, 0);
