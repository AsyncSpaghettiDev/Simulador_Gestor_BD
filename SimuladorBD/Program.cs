using System;
using System.IO;

namespace SimuladorBD {
    class Program {
        static void Main() {
            Console.WindowWidth = (int) ( Console.LargestWindowWidth * 0.75 );
            Menu mainMenu = new Menu(Path.GetFullPath("./"));
            mainMenu.Show();
        }
    }
}
