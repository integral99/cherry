﻿using System;
using System.Reflection;

namespace Cherry
{
    class Program
    {
        static Discord.Discord d;
        static void Main(string[] args)
        {
            Settings s = new Settings();
            PluginManager.LoadPlugins(s);

            d = new Discord.Discord(s.BotToken);
            d.addMessageHandler(PluginManager.GetPluginMessageHandlers());
            PluginManager.SetPluginsMessageSendTarget(d.sendMessage);
            d.MainAsync();
            while(true)
            {
                string command = Console.ReadLine();
                switch(command)
                {
                    case "reload":
                        s = new Settings();
                        PluginManager.LoadPlugins(s);
                        d.resetMessageHandler(PluginManager.GetPluginMessageHandlers());
                        PluginManager.SetPluginsMessageSendTarget(d.sendMessage);
                        break;
                }
            }
        }
    }
}
 
