/////////////////////////////////////////////////////////////////////////////////////
// Снятие предупреждений о безопасности при отладке
#define _CRT_SECURE_NO_WARNINGS

// Подключение заголовочных файлов
#include <stdio.h>					// Базовая библиотека ввода / вывода
#include <windows.h>				// Команды управления
#include <process.h>				// Библиотека запуска процессов
#include "..\Generics\CSTypes.h"	// Описание подстановочных типов данных
#include "..\Generics\xpun.h"		// Описание подстановочных типов данных

// Константы
#define DEFAULTSIZE		501

/////////////////////////////////////////////////////////////////////////////////////
// Ресурсы: данные о создаваемом приложении
#define OSME_VERSION				3,5,0,0
#define OSME_VERSION_S				"3.5.0.0"
#define OSME_PRODUCT				"Operating system macros executor"
#define OSME_COMPANY				FDL_COMPANY
// Активен с 30.07.2017; 21:14

/////////////////////////////////////////////////////////////////////////////////////
// Макросы
// Управление
#define _PAUSE(p)		Sleep (p);
#define _EXITONERROR(c)	return c;
schar ExecutionCommand[DEFAULTSIZE];
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
