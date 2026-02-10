# OS macros executor: user guide
> **ƒ** &nbsp;RD AAOW FDL; 19.01.2025; 22:03

---

- [General information](#general-information)
- [Download links](https://adslbarxatov.github.io/DPArray#os-macros-executor)
- [Версия на русском языке](https://adslbarxatov.github.io/OSMacrosExecutor/ru)

---

## General information

This tool allows you to create and execute your operating system macros.
Application may be used for imitation of user activities when:
- you need to perform many similar operations with files and / or programs;
- you cannot automate these activities using standard tools;
- some activities must be performed without your direct participation.

Application can “use” mouse, keyboard, command line and interrupts as same as
user with the same potentialities.

Just create macro using `OSMacrosEditor` tool and run them using `OSMacrosEx` tool.
If something went wrong, you can use this example file (`Test.osm`):

| Command | Description |
|-|-|
| `0 C:\Windows\notepad.exe` | Execute (`C:\Windows\notepad.exe`) |
| `5 3000` | Execution pause (3000 ms) |
| `4 0 49` | Key press (`1`) |
| `4 0 50` | Key press (`2`) |
| `4 0 51` | Key press (`3`) |
| `4 0 52` | Key press (`4`) |
| `4 0 53` | Key press (`5`) |
| `4 1 49` | Key press (`Shift` + `1`) |
| `5 2000` | Execution pause (2000 ms) |
| `0 mspaint` | Execute (`mspaint`) |
| `5 3000` | Execution pause (3000 ms) |
| `4 4 32` | Key press (`Alt` + `Space`) |
| `4 0 38` | Key press (`Up`) |
| `4 0 38` | Key press (`Up`) |
| `5 1000` | Execution pause (1000 ms) |
| `4 0 13` | Key press (`Enter`) |
| `5 2000` | Execution pause (2000 ms) |
| `1 4321 20507` | Set mouse position (relative to screen: x = 0.066, y = 0.313) |
| `6` | Begin dragging |
| `1 9843 22899` | Set mouse position (relative to screen: x = 0.150, y = 0.349) |
| `7` | End dragging |
| `1 4418 24096` | Set mouse position (relative to screen: x = 0.067, y = 0.368) |
| `6` | Begin dragging |
| `1 9507 19909` | Set mouse position (relative to screen: x = 0.145, y = 0.304) |
| `7` | End dragging |

&nbsp;



***Controls***:

- `F1` – displaying of this quick manual;
- `Ctrl` + `Q` – exiting application;
- `F4` – run the existing macro;
- `F5` – run the current macro;
- `Ctrl` + `O` – open the macro;
- `Ctrl` + `S` – save the current macro
