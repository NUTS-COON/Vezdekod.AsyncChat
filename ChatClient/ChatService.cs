using System;
using System.Net;
using System.Threading.Tasks;
using Confluent.Kafka;
using Contract;
using Kafka.Client;
using Newtonsoft.Json;

namespace ChatClient
{
    public class ChatService
    {
        private const string Usage = "Supported comands:\n" +
                                     "show users - show all users\n" +
                                     "send user_name message - sending message to user_name";
        private const string ShowUsersCommand = "show users";
        private const string SendMessageCommandPrefix = "send";
        private const string HelpCommand = "help";
        private const string ExitCommand = "exit";
        private const string ReconnectCommand = "reconnect";
        
        private readonly ConsumerFactory<string, string> _consumerFactory;
        private readonly AsyncApiService _asyncApiService;
        
        private IConsumer<string, string> _consumer;
        private string _currentUser;
        private Task _consumerSubscriber;

        
        public ChatService(ConsumerFactory<string, string> consumerFactory, AsyncApiService asyncApiService)
        {
            _consumerFactory = consumerFactory;
            _asyncApiService = asyncApiService;
        }

        public async Task Run()
        {
            var connected = false;
            while (true)
            {
                try
                {
                    if (!connected)
                    {
                        connected = await Connect();
                    }

                    if (!connected)
                    {
                        continue;
                    }

                    var exit = await ProcessCommand();
                    if (exit)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong");
                    Console.WriteLine(e);
                }
            }
        }

        private async Task InitConsumer(string topic)
        {
            _consumer = await _consumerFactory.Create();
            _consumer.Subscribe(topic);
            _consumerSubscriber = Task.Run(() =>
            {
                while (true)
                {
                    var cr = _consumer.Consume();
                    _consumer.Commit(cr);
                    var message = JsonConvert.DeserializeObject<ChatMessage>(cr.Message.Value);
                    Console.Write($"[MESSAGE] From: {message.Sender}. Text:\n{message.Text}\n>");
                }
            });
        }

        private async Task<bool> Connect()
        {
            Console.Write($"Enter your name without space. If you want reconnect, after your name enter '{ReconnectCommand}': ");
            var input = Console.ReadLine();
            Console.WriteLine("Connecting...");

            var args = input.Split(" ");
            var name = args[0];
            if (args.Length == 2 && args[1] == ReconnectCommand)
            {
                await FinishConnection(name);
                return true;
            }

            var result = await _asyncApiService.Connect(name);
            if (result == HttpStatusCode.Conflict)
            {
                Console.WriteLine("Name already taken. Choose other");
                return false;
            }

            if (result != HttpStatusCode.OK)
            {
                Console.WriteLine("Something went wrong");
                return false;
            }

            await FinishConnection(name);
            return true;
        }

        private async Task FinishConnection(string name)
        {
            _currentUser = name;
            await InitConsumer(_currentUser);
            Console.WriteLine("Connected!");
            ShowUsage();
        }

        private async Task<bool> ProcessCommand()
        {
            Console.Write(">");
            var command = Console.ReadLine().ToLower();
            if (command.StartsWith(SendMessageCommandPrefix))
            {
                await SendMessage(command);
                return false;
            }
            
            switch (command)
            {
                case ExitCommand:
                    return true;
                case HelpCommand:
                    ShowUsage();
                    break;
                case ShowUsersCommand:
                    await ShowUsers();
                    break;
                default:
                    Console.WriteLine("Unknown command. Use 'help' for help");
                    break;
            }
            
            return false;
        }

        private async Task ShowUsers()
        {
            var users = await _asyncApiService.GetUsers();
            foreach (var user in users.Users)
            {
                Console.WriteLine(user);
            }
        }

        private async Task SendMessage(string command)
        {
            var args = command.Split(" ");
            if (args.Length < 3)
            {
                Console.WriteLine("Invalid send command");
                return;
            }

            var text = command.Replace($"{args[0]} {args[1]} ", "");
            var result = await _asyncApiService.SendMessage(args[1], text, _currentUser);
            if (result)
            {
                Console.WriteLine("Message successfully sent");
            }
            else
            {
                Console.WriteLine("Failed to send message");
            }
        }

        private void ShowUsage()
        {
            Console.WriteLine(Usage);
        }
    }
}