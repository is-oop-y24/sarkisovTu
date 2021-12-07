using System;
using System.Collections.Generic;
using Banks.ConsoleInterface;
using Banks.Models;
using Banks.Services;
using Banks.Types;

namespace Banks
{
    internal static class Program
    {
        private static void Main()
        {
            CentralBankService centralBank = new CentralBankService();
            var console = new Cli(centralBank);
            console.Start();
        }
    }
}
