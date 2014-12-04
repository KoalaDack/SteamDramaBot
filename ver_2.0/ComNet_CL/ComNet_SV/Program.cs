using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComNet_SV
{
    class Program
    {
        private static Server netServer;
        private static XML xml;
        private static Program main;
        static void Main(string[] args)
        {
            //Start instances of classes
            main = new Program();
            netServer = new Server();
            xml = new XML();
            main.start();

        }

        /**
         *  Starts the server, the comment generator, and all the other stuff
         **/

        void start()
        {
            //WARNING: Must start the server last!
            netServer.start("master", 500, 1212);  
        }

    }
}
