using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cod2Stats.Watcher
{
    public static class Watcher
    {
        private static ManualResetEvent _event = new ManualResetEvent(false);
        private static int _changes = 0;

        public static void Main(string[] args)
        {
            var watcher = new FileSystemWatcher();
            var serverLogFullFilePath = ConfigurationManager.AppSettings["serverLogFullFilePath"];
            var serverLogDirectoryPath = Path.GetDirectoryName(serverLogFullFilePath);
            var serverLogFilename = Path.GetFileName(serverLogFullFilePath);

            Console.WriteLine($"Watching {serverLogFullFilePath}");
            Console.WriteLine($"ServerLogDirectoryPath: {serverLogDirectoryPath}");
            Console.WriteLine($"ServerLogFilename: {serverLogFilename}");

            watcher.Path = serverLogDirectoryPath;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;
            watcher.Filter = serverLogFilename;
            watcher.Changed += OnLogUpdate;
            watcher.EnableRaisingEvents = true;

            _event.WaitOne();
        }

        private static void OnLogUpdate(object sender, FileSystemEventArgs e)
        {
            _changes++;
            Console.WriteLine($"File changed {_changes} times");


            var initGame = @"  0:00 InitGame: \g_antilag\1\g_gametype\hq\gamename\Call of Duty 2\mapname\mp_heckticfv\protocol\118\shortversion\1.3\sv_allowAnonymous\0\sv_floodProtect\1\sv_hostname\CoD2Host\sv_maxclients\20\sv_maxPing\0\sv_maxRate\0\sv_minPing\0\sv_privateClients\0\sv_punkbuster\0\sv_pure\1\sv_voice\1";
            var weapon   = " 24:45 Weapon;0;5;Pancho;frag_grenade_american_mp";
            var chat     = "  0:48 say;0;0;Lucky-Strike;hola mundo";
            var damage   = " 24:55 D;0;1;allies;Ghost;0;5;axis;Pancho;thompson_mp;30;MOD_PISTOL_BULLET;right_leg_lower";
            var kill     = " 24:56 K;0;0;axis;Lucky-Strike;0;3;allies;Dani;greasegun_mp;135;MOD_HEAD_SHOT;head";
            var join     = "  0:35 J;0;3;Dani";
            var quit     = " 29:21 Q;0;3;Dani";
            var lost     = " 29:08 L;allies;0;Ghost;0;feche;0;Dani;0;rangash";
            var win      = " 29:08 W;axis;0;Lucky-Strike;0;L;0;Pancho;0;Bryan";
            var shutdownGame = " 30:02 ShutdownGame:";
        }

        public class Event
        {
            public TimeSpan ServerTime { get; }
            public string Type { get; }
        }

        public class EventType
        {
            private readonly string _value;
            public EventType(string value)
            {
                _value = value;
                _types.Add(this);
            }

            private static List<EventType> _types = new List<EventType>();
            private static EventType _say = new EventType("say");
            private static EventType _sayTeam = new EventType("sayteam");
            private static EventType _join = new EventType("J");
            private static EventType _quit = new EventType("Q");

            public static EventType Say => _say;

            public static EventType SayTeam => _sayTeam;

            public static EventType Join => _join;

            public static EventType Quit => _quit;

            public static EventType Find(string value)
            {
                return _types.Single(x => x._value == value);
            }
        }

        public class Chat : Event
        {
            public int GUID { get; }

            public int ClientID { get; }

            public string ClientName { get; }

            public string Message { get; }
        }

        public class Player : Event
        {
            public int GUID { get; }

            public int ClientID { get; }

            public string ClientName { get; }
        }

        public class Shoot : Event
        {
            public int AttackeeGUID { get; }
            public int AttackeeID { get; }
            public string AttackeeTeam { get; }
            public string AttackeeName { get; }
            public int AttackerGUID { get; }
            public int AttackerID { get; }
            public string AttackerTeam { get; }
            public string AttackerName { get; }
            public int AttackerWeapon { get; }
            public int Damage { get; }
            public string DamageType { get; }
            public string DamageLocation { get; }
        }
    }
}
