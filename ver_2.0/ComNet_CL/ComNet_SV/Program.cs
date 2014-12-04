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
        private static Program main;
        static void Main(string[] args)
        {
            //Start instances of classes
            netServer = new Server();
            main = new Program();
            main.start();

        }

        /**
         *  Starts the server, the comment generator, and all the other stuff
         **/
        void start()
        {
            netServer.start("master", 500, 1212);
            XML.load();
        }

    }
}
