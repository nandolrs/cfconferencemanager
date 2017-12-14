using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceManagement
{
    class Program
    {
        static public void metodo11()
        {
            string x = Console.ReadLine();
            Console.WriteLine(x);
        }

        static public void  Metodo12()
        {
            string x = Console.ReadLine();
            char[] y = x.ToCharArray();
            int z1 = int.Parse(y[0].ToString());
            int z2 = int.Parse(y[2].ToString());
            int z3 = int.Parse(y[5].ToString());
            int w = (z1 + z2 + z3)/3;
            Console.WriteLine(w);


        }

        static public void Metodo13()
        {
            string x = Console.ReadLine();
            string frase = "";
            if (int.Parse(x) % 1 == 0 && int.Parse(x) % int.Parse(x) == 0)
                frase = "primo";
            else
                frase = "nao primo";



            Console.WriteLine(frase);


        }
        static public void Metodo14()
        {
            string x = Console.ReadLine();
            int y = int.Parse(x);
            int w = y;
            for (int z = y-1; z > 0; z--)
            {
                w = w * z;
            }
            if (y == 0) { w = 1; }
            Console.WriteLine(w);


        }

        static public void Metodo1()
        {
            string x = Console.ReadLine();
            int y = int.Parse(x);
            int w = 1;
            Console.WriteLine(w);
            for (int z = 1; z <= y+1; z++)
            {
                w = w + z-1;
                Console.WriteLine(w);
            }


        }


        static void Main(string[] args)
        {

            Metodo1();


            // trata a entrada de dados respectivamente por lista ou por teckado: -l <lista> OU  -k 

            Talk[] talksInput = new Talk[0];
            
            if (args.Length == 0) { return; }               // se nenhum parâmetro foi informado, retorna

            if (args[0] == "-l")                            // entrada por linha de comando
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Informe -l <'descricao1', 'descricao2', ...>");
                    return;
                }
                talksInput = InputByCommand(args);
            }else if (args[0] == "-k")                      // entrada por teclado
            {
                talksInput = InputByKeyboard();
            } else if (args[0] == "-xml")                   // entrada por arquivo xml
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Informe -xml <nome do arquivo>");
                    return;
                }
                talksInput = InputByXml(args);
            }

            //

            Conference conference = new Conference();
            Track[] tracks = new Track[0];
            Talk[] talks = new Talk[0];

            // monta a conferência

            conference.AddTrack();

            foreach (Talk talkEach in talksInput)
            {
                if (conference.tracks[conference.tracks.Length - 1].leftMinutes < talkEach.duration) // a trilha não tem mais espaço
                {
                    conference.AddTrack();
                }
                conference.tracks[conference.tracks.Length - 1].AddTalk(talkEach);
            }

            // ordena a matriz por hora inicio da Talk

            for (int ii = 0; ii < conference.tracks.Length; ii++)
            {
                Array.Sort(conference.tracks[ii].talks
                    , delegate (Talk talk1, Talk talk2)
                    { return talk1.hourStart.CompareTo(talk2.hourStart); }
                    );
            }

            //verifica se pode trazer o netwowk para as 16:00hs

            foreach (Track track in conference.tracks)
            {
                if (track.talks[track.talks.Length - 2].hourEnd < track.talks[track.talks.Length - 1].hourStart.AddHours(-1))
                {
                    track.talks[track.talks.Length - 1].hourStart = track.talks[track.talks.Length - 1].hourStart.AddHours(-1);
                }
            }

            // mostra os resultados

            Console.WriteLine("01.Conferencia");
            Console.WriteLine("01.01.Traks");
            foreach (Track trackOut in conference.tracks)
            {
                string trackCodigoLiteral = (trackOut.codigo + 100).ToString().Substring(1, 2);
                Console.WriteLine("01.01."+ trackCodigoLiteral + " " + trackOut.description);
                foreach (Talk talkOut in trackOut.talks)
                {
                    Console.WriteLine("01.01." + trackCodigoLiteral + ".01 Talk  " + talkOut.hourStart.ToString("HH:mm") + " - " + talkOut.description);
                }

            }

        }

        static Talk[] InputByCommand(string[] cmds)
        {
            Talk talk = null;
            Talk[] talksInput = new Talk[0];
            string talkInput;

            foreach (string cmd in cmds)
            {
                if (cmd == "-l") { continue; }

                talkInput = cmd;

                talk = new Talk();
                talk.description = talkInput;
                int posMin = talk.description.IndexOf("min");
                string minute = "";
                if (posMin > -1)
                {
                    int minuteValueByTry = 0;
                    minute = talk.description.Substring(posMin - 2, 2);
                    if (!int.TryParse(minute, out minuteValueByTry))
                    {
                        posMin = talk.description.IndexOf("min",posMin+2);
                        if (posMin > -1)
                        {
                            minuteValueByTry = 0;
                            minute = talk.description.Substring(posMin - 2, 2);
                        }
                        else
                        {
                            minute = "15";
                        }
                    }
                }
                else
                {
                    minute = "15";
                }
                talk.duration = int.Parse(minute);

                Array.Resize(ref talksInput, talksInput.Length + 1);
                talksInput[talksInput.Length - 1] = talk;
            }

            return talksInput;
        }

        static Talk[] InputByKeyboard()
        {
            int i = 0;
            string talkInput;
            Talk talk = null;
            Talk[] talksInput = new Talk[0];


            while (true) // 
            {
                Console.WriteLine("Input the title Talk (" + (1 + i).ToString() + "):");
                talkInput = Console.ReadLine();

                if (talkInput.ToLower() == "fim") { break; }

                talk = new Talk();
                talk.description = talkInput;
                string minute = talk.description.Substring(talk.description.IndexOf("min") - 2, 2);
                talk.duration = int.Parse(minute);


                Array.Resize(ref talksInput, talksInput.Length + 1);
                talksInput[talksInput.Length - 1] = talk;

                i++;

            }

            return talksInput;

        }

        static Talk[] InputByXml(string[] cmds)
        {
            Talk talk = null;
            Talk[] talksInput = new Talk[0];
            string talkInput;

            //

            string fileXmlPath = cmds[1];
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(fileXmlPath);

            foreach (System.Xml.XmlElement xmlNode in xmlDocument.SelectNodes("//Talk"))
            {
                talkInput = xmlNode.InnerText;

                talk = new Talk();
                talk.description = talkInput;
                int posMin = talk.description.IndexOf("min");
                string minute = "";
                if (posMin > -1)
                {
                    int minuteValueByTry = 0;
                    minute = talk.description.Substring(posMin - 2, 2);
                    if (!int.TryParse(minute, out minuteValueByTry))
                    {
                        posMin = talk.description.IndexOf("min", posMin + 2);
                        if (posMin > -1)
                        {
                            minuteValueByTry = 0;
                            minute = talk.description.Substring(posMin - 2, 2);
                        }
                        else
                        {
                            minute = "15";
                        }
                    }
                }
                else
                {
                    minute = "15";
                }
                talk.duration = int.Parse(minute);

                Array.Resize(ref talksInput, talksInput.Length + 1);
                talksInput[talksInput.Length - 1] = talk;
            }

            return talksInput;

        }


    }

    public class Conference
    {
        public Track[] tracks;                  // coleção de Track
        Track track;

        public Conference()
        {
            tracks = new Track[0];
        }

        public void AddTrack()                  // adiciona uma Track na Conference
        {
            // monta e adiciona a Track

            track = new Track();
            track.codigo = tracks.Length+1;
            track.description = "Track " + (tracks.Length +1).ToString(); 
            track.hourStart = (DateTime.Today).AddHours(9);
            track.hourEnd = (DateTime.Today).AddHours(17);
            track.hourNextTalk = track.hourStart;

            Array.Resize(ref tracks, tracks.Length + 1);
            tracks[tracks.Length - 1] = track;

            // monta e adiciona a talk de launch 

            Talk talk = new Talk();
            talk.changeConfigHour = false;
            talk.description = "Launch";
            talk.duration = 60;
            talk.hourStart = (DateTime.Today).AddHours(12);

            Array.Resize(ref track.talks, track.talks.Length + 1);
            track.talks[track.talks.Length - 1] = talk;

            // monta e adiciona a talk de network

            talk = new Talk();
            talk.changeConfigHour = false;
            talk.description = "Networking Event";
            talk.duration = 60;
            talk.hourStart = (DateTime.Today).AddHours(17);

            Array.Resize(ref track.talks, track.talks.Length + 1);
            track.talks[track.talks.Length - 1] = talk;

        }
    }

    public class Track                  
    {
        public int      codigo;         // código 
        public string   description;    // descrição
        public DateTime hourStart;      // hora de início
        public DateTime hourEnd;        // hora final
        public DateTime hourNextTalk;   // hora da próxima talk
        public Talk[]   talks;          // coleção de Talk da Track

        public Track()
        {
            talks = new Talk[0];
        }
        public int leftMinutes          // minutos restantes que ainda pode ser utilizados por Talks
        {
            get
            {
                double toReturn = (hourEnd - hourStart).TotalMinutes;
                foreach (Talk talk in talks)
                {
                    toReturn -= talk.duration;
                }
                return (int) toReturn;

            }
            set { }
        }

        public void AddTalk(Talk talk)  // adiciona uma Talk
        {
            // se não for Talk de Launch e Network, configura a hora de início e fim do Talk e ajusta a hora do próximo Talk da Track

            if (talk.changeConfigHour)
            {
                talk.hourStart = this.hourNextTalk;
                this.hourNextTalk = talk.hourEnd.AddMinutes(1);
                talk.hourStart = FindHourNextTalk(talk);
            }

            // Adiciona o Talk

            Array.Resize(ref talks, talks.Length + 1);
            talks[talks.Length - 1] = talk;
        }

        DateTime FindHourNextTalk(Talk talk)    // se detecta conflito de horário, retorna novo horário de início para nova Talk
        {
            DateTime toReturn = talk.hourStart;

            foreach (Talk talkSearch in this.talks)
            {
                if (talk.hourStart.Equals(talkSearch.hourStart)  || (talk.hourEnd >= talkSearch.hourStart && talk.hourEnd <= talkSearch.hourEnd))            // conflito detectado
                {
                    toReturn = talkSearch.hourEnd.AddMinutes(1);            // calcula o novo horário
                    this.hourNextTalk = talkSearch.hourEnd.AddMinutes(1);   // ajusta o próximo horário da Track também
                }
            }

            return toReturn;
        }
    } 

    public class Talk
    {
        public string   description;            // descrição
        public int      duration;               // duração
        public DateTime hourStart;              // hora de início
        public bool     changeConfigHour;       // indica que os horários da Talk devem ser ajustados

        public Talk()
        {
            changeConfigHour = true;
        }

        public DateTime hourEnd                 // hora final
        {
            get
            {
                DateTime toReturn = this.hourStart.AddMinutes(this.duration - 1);
                return toReturn;

            }
            set { }
        }

    }
}
