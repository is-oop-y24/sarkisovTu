using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static System.Console;

namespace Banks.ConsoleInterface
{
    public class ConsoleMenu
    {
        private int _selectedIndex;
        private List<string> _options;
        private string _prompt;

        public ConsoleMenu(string prompt, List<string> options)
        {
            _prompt = prompt;
            _options = options;
            _selectedIndex = 0;
        }

        public void DisplayOptions()
        {
            WriteLine(_prompt);
            for (int i = 0; i < _options.Count; i++)
            {
                string prefix;
                if (i == _selectedIndex)
                {
                    prefix = "*";
                    ForegroundColor = ConsoleColor.Black;
                    BackgroundColor = ConsoleColor.White;
                }
                else
                {
                    prefix = " ";
                    ForegroundColor = ConsoleColor.White;
                    BackgroundColor = ConsoleColor.Black;
                }

                string currentOption = _options[i];
                WriteLine($"{prefix}  << {currentOption} >>");
            }

            ResetColor();
        }

        public int Run()
        {
            ConsoleKey keyPressed;
            do
            {
                Clear();
                DisplayOptions();

                ConsoleKeyInfo keyInfo = ReadKey(true);
                keyPressed = keyInfo.Key;

                if (keyPressed == ConsoleKey.UpArrow)
                {
                    _selectedIndex--;
                    if (_selectedIndex == -1) _selectedIndex = _options.Count - 1;
                }
                else if (keyPressed == ConsoleKey.DownArrow)
                {
                    _selectedIndex++;
                    if (_selectedIndex == _options.Count) _selectedIndex = 0;
                }
            }
            while (keyPressed != ConsoleKey.Enter);
            return _selectedIndex;
        }
    }
}