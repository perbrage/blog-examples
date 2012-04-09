using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SignalR;
using SignalR.Client;
using SignalR.Infrastructure;
using Connection = SignalR.Client.Connection;
using IConnection = SignalR.IConnection;

namespace Brage.Infrastructure.Broker
{
    public class EventBroker : IEventBroker
    {
        private readonly IConnection _serverConnection;
        private readonly Stack<Connection> _clientConnections;
        private readonly ICollection<IDisposable> _subscriptions;
        private readonly IScheduler _scheduler;
        private readonly ISubject<IEvent> _subject;
        private readonly JsonSerializerSettings _includeTypeJsonSetting;

        public event EventHandler<ConnectionStatusEventArgs> ConnectionStatus;

        private Boolean _inLocalSubscriptionMode;

        public EventBroker()
        {
            _clientConnections = new Stack<Connection>();
            _subscriptions = new List<IDisposable>();
            _scheduler = new EventLoopScheduler();
            _subject = new Subject<IEvent>();

            _includeTypeJsonSetting = new JsonSerializerSettings
                                          {
                                              TypeNameHandling = TypeNameHandling.All
                                          };
        }

        public EventBroker(String publishingUri)
            : this()
        {
            var server = new SignalR.Hosting.Self.Server(publishingUri);
            server.MapConnection<EchoConnection>("/echo");
            server.Start();

            var connectionManager = server.DependencyResolver.Resolve<IConnectionManager>();
            _serverConnection = connectionManager.GetConnection<EchoConnection>();
        }

        public IEventBroker Publish<TEvent>(TEvent @event)
            where TEvent : IEvent
        {
            _subject.OnNext(@event);

            if (_serverConnection != null)
                _serverConnection.Broadcast(JsonConvert.SerializeObject(@event, 
                                                                        Formatting.None, 
                                                                        _includeTypeJsonSetting));

            return this;
        }

        public ISubscribe Locally()
        {
            _inLocalSubscriptionMode = true;
            return this;
        }

        public ISubscribe Remotely(String remoteEventStreamUri)
        {
            _inLocalSubscriptionMode = false;

            var connection = new Connection(remoteEventStreamUri + "echo");

            connection.Start()
                      .ContinueWith(task =>
                      {
                          var errorMessage = task.Exception == null ? "Undefined error" 
                                                                    : task.Exception.GetBaseException().Message;

                          OnConnectionStatus(task.IsFaulted ? new ConnectionStatusEventArgs(errorMessage) 
                                                            : new ConnectionStatusEventArgs());
                      });

            Task.WaitAll();

            _clientConnections.Push(connection);

            return this;
        }

        ISubscribe ISubscribe.Subscribe<TEvent>(Action<TEvent> onConsume)
        {
            ((ISubscribe)this).Subscribe(null, onConsume);
            return this;
        }

        ISubscribe ISubscribe.Subscribe<TEvent>(IEventConsumer<TEvent> eventConsumer)
        {
            ((ISubscribe)this).Subscribe(eventConsumer.Filters, eventConsumer.Handle);
            return this;
        }

        ISubscribe ISubscribe.Subscribe<TEvent>(Func<TEvent, Boolean> filter, 
                                                Action<TEvent> onConsume)
        {
            _subscriptions.Add(GetCurrentObservable<TEvent>()
                                .Where(filter ?? (x => true))
                                .ObserveOn(_scheduler)
                                .Subscribe(onConsume));
            return this;
        }

        private IObservable<TEvent> GetCurrentObservable<TEvent>()
        {
            return _inLocalSubscriptionMode ? _subject.Where(o => o is TEvent).Cast<TEvent>()
                                            : GetCurrentConnection()
                                                        .AsObservable()
                                                        .Where(IsEventOfCorrectType<TEvent>)
                                                        .Select(JsonConvert.DeserializeObject<TEvent>)
                                                        .AsObservable();
        }

        private Boolean IsEventOfCorrectType<TEvent>(String jsonString)
        {
            var type = typeof(TEvent);
            var typeName = String.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
            var token = JObject.Parse(jsonString);
            var eventType = (String)token["$type"];

            if (eventType == null)
                return false;

            return eventType == typeName;
        }

        private Connection GetCurrentConnection()
        {
            return _clientConnections.Peek();
        }

        private void OnConnectionStatus(ConnectionStatusEventArgs e)
        {
            if (ConnectionStatus != null)
                ConnectionStatus(this, e);
        }

        public void Dispose()
        {
            if (_subscriptions != null)
                foreach (var subscription in _subscriptions)
                    subscription.Dispose();

            if (_clientConnections != null)
            {
                foreach (var clientConnection in _clientConnections)
                    clientConnection.Stop();

                _clientConnections.Clear();
            }

            if (_serverConnection != null)
                _serverConnection.Close();
        }
    }
}
