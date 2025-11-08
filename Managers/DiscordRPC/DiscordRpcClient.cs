using iiMenu.Managers.DiscordRPC.Events;
using iiMenu.Managers.DiscordRPC.Exceptions;
using iiMenu.Managers.DiscordRPC.IO;
using iiMenu.Managers.DiscordRPC.Logging;
using iiMenu.Managers.DiscordRPC.Message;
using iiMenu.Managers.DiscordRPC.RPC;
using iiMenu.Managers.DiscordRPC.RPC.Commands;
using iiMenu.Managers.DiscordRPC.RPC.Payload;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace iiMenu.Managers.DiscordRPC
{
	public sealed class DiscordRpcClient : IDisposable
	{
		public bool HasRegisteredUriScheme { get; private set; }

		public string ApplicationID { get; private set; }

		public string SteamID { get; private set; }

		public int ProcessID { get; private set; }

		public int MaxQueueSize { get; private set; }

		public bool IsDisposed { get; private set; }

		public ILogger Logger
		{
			get
			{
				return _logger;
			}
			set
			{
                _logger = value;
				if (connection != null)
				{
                    connection.Logger = value;
				}
			}
		}

		public bool AutoEvents { get; private set; }

		public bool SkipIdenticalPresence { get; set; }

		public int TargetPipe { get; private set; }

		public RichPresence CurrentPresence { get; private set; }

		public EventType Subscription { get; private set; }

		public User CurrentUser { get; private set; }

		public Configuration Configuration { get; private set; }

		public bool IsInitialized { get; private set; }

		public bool ShutdownOnly
		{
			get
			{
				return _shutdownOnly;
			}
			set
			{
                _shutdownOnly = value;
				if (connection != null)
				{
                    connection.ShutdownOnly = value;
				}
			}
		}

		// (add) Token: 0x06000028 RID: 40 RVA: 0x000021C4 File Offset: 0x000003C4
		// (remove) Token: 0x06000029 RID: 41 RVA: 0x000021FC File Offset: 0x000003FC
		public event OnReadyEvent OnReady;

		// (add) Token: 0x0600002A RID: 42 RVA: 0x00002234 File Offset: 0x00000434
		// (remove) Token: 0x0600002B RID: 43 RVA: 0x0000226C File Offset: 0x0000046C
		public event OnCloseEvent OnClose;

		// (add) Token: 0x0600002C RID: 44 RVA: 0x000022A4 File Offset: 0x000004A4
		// (remove) Token: 0x0600002D RID: 45 RVA: 0x000022DC File Offset: 0x000004DC
		public event OnErrorEvent OnError;

		// (add) Token: 0x0600002E RID: 46 RVA: 0x00002314 File Offset: 0x00000514
		// (remove) Token: 0x0600002F RID: 47 RVA: 0x0000234C File Offset: 0x0000054C
		public event OnPresenceUpdateEvent OnPresenceUpdate;

		// (add) Token: 0x06000030 RID: 48 RVA: 0x00002384 File Offset: 0x00000584
		// (remove) Token: 0x06000031 RID: 49 RVA: 0x000023BC File Offset: 0x000005BC
		public event OnSubscribeEvent OnSubscribe;

		// (add) Token: 0x06000032 RID: 50 RVA: 0x000023F4 File Offset: 0x000005F4
		// (remove) Token: 0x06000033 RID: 51 RVA: 0x0000242C File Offset: 0x0000062C
		public event OnUnsubscribeEvent OnUnsubscribe;

		// (add) Token: 0x06000034 RID: 52 RVA: 0x00002464 File Offset: 0x00000664
		// (remove) Token: 0x06000035 RID: 53 RVA: 0x0000249C File Offset: 0x0000069C
		public event OnJoinEvent OnJoin;

		// (add) Token: 0x06000036 RID: 54 RVA: 0x000024D4 File Offset: 0x000006D4
		// (remove) Token: 0x06000037 RID: 55 RVA: 0x0000250C File Offset: 0x0000070C
		public event OnSpectateEvent OnSpectate;

		// (add) Token: 0x06000038 RID: 56 RVA: 0x00002544 File Offset: 0x00000744
		// (remove) Token: 0x06000039 RID: 57 RVA: 0x0000257C File Offset: 0x0000077C
		public event OnJoinRequestedEvent OnJoinRequested;

		// (add) Token: 0x0600003A RID: 58 RVA: 0x000025B4 File Offset: 0x000007B4
		// (remove) Token: 0x0600003B RID: 59 RVA: 0x000025EC File Offset: 0x000007EC
		public event OnConnectionEstablishedEvent OnConnectionEstablished;

		// (add) Token: 0x0600003C RID: 60 RVA: 0x00002624 File Offset: 0x00000824
		// (remove) Token: 0x0600003D RID: 61 RVA: 0x0000265C File Offset: 0x0000085C
		public event OnConnectionFailedEvent OnConnectionFailed;

		// (add) Token: 0x0600003E RID: 62 RVA: 0x00002694 File Offset: 0x00000894
		// (remove) Token: 0x0600003F RID: 63 RVA: 0x000026CC File Offset: 0x000008CC
		public event OnRpcMessageEvent OnRpcMessage;

		public DiscordRpcClient(string applicationID) : this(applicationID, -1, null, true, null)
		{
		}

		public DiscordRpcClient(string applicationID, int pipe = -1, ILogger logger = null, bool autoEvents = true, INamedPipeClient client = null)
		{
			if (string.IsNullOrEmpty(applicationID))
			{
				throw new ArgumentNullException("applicationID");
			}
            ApplicationID = applicationID.Trim();
            TargetPipe = pipe;
            ProcessID = Process.GetCurrentProcess().Id;
            HasRegisteredUriScheme = false;
            AutoEvents = autoEvents;
            SkipIdenticalPresence = true;
            _logger = logger ?? new NullLogger();
            connection = new RpcConnection(ApplicationID, ProcessID, TargetPipe, client ?? new ManagedNamedPipeClient(), autoEvents ? 0U : 128U, 512U)
			{
				ShutdownOnly = _shutdownOnly,
				Logger = _logger
            };
            connection.OnRpcMessage += delegate(object sender, IMessage msg)
			{
				if (OnRpcMessage != null)
				{
                    OnRpcMessage(this, msg);
				}
				if (AutoEvents)
				{
                    ProcessMessage(msg);
				}
			};
		}

        public IMessage[] Invoke()
        {
            if (AutoEvents)
            {
                Logger.Error("Cannot Invoke client when AutomaticallyInvokeEvents has been set.", Array.Empty<object>());
                return new IMessage[0];
            }

            List<IMessage> messages = new List<IMessage>();

            foreach (IMessage message in connection.DequeueMessages())
            {
                ProcessMessage(message);
                messages.Add(message);
            }

            return messages.ToArray();
        }


        private void ProcessMessage(IMessage message)
		{
			if (message == null)
			{
				return;
			}
			switch (message.Type)
			{
			case MessageType.Ready:
			{
				ReadyMessage readyMessage = message as ReadyMessage;
				if (readyMessage != null)
				{
					object sync = _sync;
					lock (sync)
					{
                                Configuration = readyMessage.Configuration;
                                CurrentUser = readyMessage.User;
					}
                            SynchronizeState();
				}
				if (OnReady != null)
				{
                            OnReady(this, message as ReadyMessage);
					return;
				}
				break;
			}
			case MessageType.Close:
				if (OnClose != null)
				{
                        OnClose(this, message as CloseMessage);
					return;
				}
				break;
			case MessageType.Error:
				if (OnError != null)
				{
                        OnError(this, message as ErrorMessage);
					return;
				}
				break;
			case MessageType.PresenceUpdate:
			{
				object sync = _sync;
				lock (sync)
				{
					PresenceMessage presenceMessage = message as PresenceMessage;
					if (presenceMessage != null)
					{
						if (presenceMessage.Presence == null)
						{
                                    CurrentPresence = null;
						}
						else if (CurrentPresence == null)
						{
                                    CurrentPresence = new RichPresence().Merge(presenceMessage.Presence);
						}
						else
						{
                                    CurrentPresence.Merge(presenceMessage.Presence);
						}
						presenceMessage.Presence = CurrentPresence;
					}
				}
				if (OnPresenceUpdate != null)
				{
                            OnPresenceUpdate(this, message as PresenceMessage);
					return;
				}
				break;
			}
			case MessageType.Subscribe:
			{
				object sync = _sync;
				lock (sync)
				{
					SubscribeMessage subscribeMessage = message as SubscribeMessage;
                            Subscription |= subscribeMessage.Event;
				}
				if (OnSubscribe != null)
				{
                            OnSubscribe(this, message as SubscribeMessage);
					return;
				}
				break;
			}
			case MessageType.Unsubscribe:
			{
				object sync = _sync;
				lock (sync)
				{
					UnsubscribeMessage unsubscribeMessage = message as UnsubscribeMessage;
                            Subscription &= ~unsubscribeMessage.Event;
				}
				if (OnUnsubscribe != null)
				{
                            OnUnsubscribe(this, message as UnsubscribeMessage);
					return;
				}
				break;
			}
			case MessageType.Join:
				if (OnJoin != null)
				{
                        OnJoin(this, message as JoinMessage);
					return;
				}
				break;
			case MessageType.Spectate:
				if (OnSpectate != null)
				{
                        OnSpectate(this, message as SpectateMessage);
					return;
				}
				break;
			case MessageType.JoinRequest:
				if (Configuration != null)
				{
					JoinRequestMessage joinRequestMessage = message as JoinRequestMessage;
					if (joinRequestMessage != null)
					{
						joinRequestMessage.User.SetConfiguration(Configuration);
					}
				}
				if (OnJoinRequested != null)
				{
                        OnJoinRequested(this, message as JoinRequestMessage);
					return;
				}
				break;
			case MessageType.ConnectionEstablished:
				if (OnConnectionEstablished != null)
				{
                        OnConnectionEstablished(this, message as ConnectionEstablishedMessage);
					return;
				}
				break;
			case MessageType.ConnectionFailed:
				if (OnConnectionFailed != null)
				{
                        OnConnectionFailed(this, message as ConnectionFailedMessage);
					return;
				}
				break;
			default:
                    Logger.Error("Message was queued with no appropriate handle! {0}", new object[]
				{
					message.Type
				});
				break;
			}
		}

		public void Respond(JoinRequestMessage request, bool acceptRequest)
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException("Discord IPC Client");
			}
			if (connection == null)
			{
				throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
			}
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
            connection.EnqueueCommand(new RespondCommand
			{
				Accept = acceptRequest,
				UserID = request.User.ID.ToString()
			});
		}

		public void SetPresence(RichPresence presence)
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException("Discord IPC Client");
			}
			if (connection == null)
			{
				throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
			}
			if (!IsInitialized)
			{
                Logger.Warning("The client is not yet initialized, storing the presence as a state instead.", Array.Empty<object>());
			}
			if (presence == null)
			{
				if (!SkipIdenticalPresence || CurrentPresence != null)
				{
                    connection.EnqueueCommand(new PresenceCommand
					{
						PID = ProcessID,
						Presence = null
					});
				}
			}
			else
			{
				if (presence.HasSecrets() && !HasRegisteredUriScheme)
				{
					throw new BadPresenceException("Cannot send a presence with secrets as this object has not registered a URI scheme. Please enable the uri scheme registration in the DiscordRpcClient constructor.");
				}
				if (presence.HasParty() && presence.Party.Max < presence.Party.Size)
				{
					throw new BadPresenceException("Presence maximum party size cannot be smaller than the current size.");
				}
				if (presence.HasSecrets() && !presence.HasParty())
				{
                    Logger.Warning("The presence has set the secrets but no buttons will show as there is no party available.", Array.Empty<object>());
				}
				if (!SkipIdenticalPresence || !presence.Matches(CurrentPresence))
				{
                    connection.EnqueueCommand(new PresenceCommand
					{
						PID = ProcessID,
						Presence = presence.Clone()
					});
				}
			}
			object sync = _sync;
			lock (sync)
			{
                CurrentPresence = presence != null ? presence.Clone() : null;
			}
		}

		public RichPresence UpdateButtons(Button[] button = null)
		{
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
			object sync = _sync;
			RichPresence richPresence;
			lock (sync)
			{
				if (CurrentPresence == null)
				{
					richPresence = new RichPresence();
				}
				else
				{
					richPresence = CurrentPresence.Clone();
				}
			}
			richPresence.Buttons = button;
            SetPresence(richPresence);
			return richPresence;
		}

		public RichPresence SetButton(Button button, int index = 0)
		{
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
			object sync = _sync;
			RichPresence richPresence;
			lock (sync)
			{
				if (CurrentPresence == null)
				{
					richPresence = new RichPresence();
				}
				else
				{
					richPresence = CurrentPresence.Clone();
				}
			}
			richPresence.Buttons[index] = button;
            SetPresence(richPresence);
			return richPresence;
		}

		public RichPresence UpdateDetails(string details)
		{
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
			object sync = _sync;
			RichPresence richPresence;
			lock (sync)
			{
				if (CurrentPresence == null)
				{
					richPresence = new RichPresence();
				}
				else
				{
					richPresence = CurrentPresence.Clone();
				}
			}
			richPresence.Details = details;
            SetPresence(richPresence);
			return richPresence;
		}

		public RichPresence UpdateState(string state)
		{
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
			object sync = _sync;
			RichPresence richPresence;
			lock (sync)
			{
				if (CurrentPresence == null)
				{
					richPresence = new RichPresence();
				}
				else
				{
					richPresence = CurrentPresence.Clone();
				}
			}
			richPresence.State = state;
            SetPresence(richPresence);
			return richPresence;
		}

		public RichPresence UpdateParty(Party party)
		{
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
			object sync = _sync;
			RichPresence richPresence;
			lock (sync)
			{
				if (CurrentPresence == null)
				{
					richPresence = new RichPresence();
				}
				else
				{
					richPresence = CurrentPresence.Clone();
				}
			}
			richPresence.Party = party;
            SetPresence(richPresence);
			return richPresence;
		}

		public RichPresence UpdatePartySize(int size)
		{
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
			object sync = _sync;
			RichPresence richPresence;
			lock (sync)
			{
				if (CurrentPresence == null)
				{
					richPresence = new RichPresence();
				}
				else
				{
					richPresence = CurrentPresence.Clone();
				}
			}
			if (richPresence.Party == null)
			{
				throw new BadPresenceException("Cannot set the size of the party if the party does not exist");
			}
			richPresence.Party.Size = size;
            SetPresence(richPresence);
			return richPresence;
		}

		public RichPresence UpdatePartySize(int size, int max)
		{
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
			object sync = _sync;
			RichPresence richPresence;
			lock (sync)
			{
				if (CurrentPresence == null)
				{
					richPresence = new RichPresence();
				}
				else
				{
					richPresence = CurrentPresence.Clone();
				}
			}
			if (richPresence.Party == null)
			{
				throw new BadPresenceException("Cannot set the size of the party if the party does not exist");
			}
			richPresence.Party.Size = size;
			richPresence.Party.Max = max;
            SetPresence(richPresence);
			return richPresence;
		}

		public RichPresence UpdateLargeAsset(string key = null, string tooltip = null)
		{
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
			object sync = _sync;
			RichPresence richPresence;
			lock (sync)
			{
				if (CurrentPresence == null)
				{
					richPresence = new RichPresence();
				}
				else
				{
					richPresence = CurrentPresence.Clone();
				}
			}
			if (richPresence.Assets == null)
			{
				richPresence.Assets = new Assets();
			}
			richPresence.Assets.LargeImageKey = key ?? richPresence.Assets.LargeImageKey;
			richPresence.Assets.LargeImageText = tooltip ?? richPresence.Assets.LargeImageText;
            SetPresence(richPresence);
			return richPresence;
		}

		public RichPresence UpdateSmallAsset(string key = null, string tooltip = null)
		{
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
			object sync = _sync;
			RichPresence richPresence;
			lock (sync)
			{
				if (CurrentPresence == null)
				{
					richPresence = new RichPresence();
				}
				else
				{
					richPresence = CurrentPresence.Clone();
				}
			}
			if (richPresence.Assets == null)
			{
				richPresence.Assets = new Assets();
			}
			richPresence.Assets.SmallImageKey = key ?? richPresence.Assets.SmallImageKey;
			richPresence.Assets.SmallImageText = tooltip ?? richPresence.Assets.SmallImageText;
            SetPresence(richPresence);
			return richPresence;
		}

		public RichPresence UpdateSecrets(Secrets secrets)
		{
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
			object sync = _sync;
			RichPresence richPresence;
			lock (sync)
			{
				if (CurrentPresence == null)
				{
					richPresence = new RichPresence();
				}
				else
				{
					richPresence = CurrentPresence.Clone();
				}
			}
			richPresence.Secrets = secrets;
            SetPresence(richPresence);
			return richPresence;
		}

		public RichPresence UpdateStartTime()
		{
			return UpdateStartTime(DateTime.UtcNow);
		}

		public RichPresence UpdateStartTime(DateTime time)
		{
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
			object sync = _sync;
			RichPresence richPresence;
			lock (sync)
			{
				if (CurrentPresence == null)
				{
					richPresence = new RichPresence();
				}
				else
				{
					richPresence = CurrentPresence.Clone();
				}
			}
			if (richPresence.Timestamps == null)
			{
				richPresence.Timestamps = new Timestamps();
			}
			richPresence.Timestamps.Start = new DateTime?(time);
            SetPresence(richPresence);
			return richPresence;
		}

		public RichPresence UpdateEndTime()
		{
			return UpdateEndTime(DateTime.UtcNow);
		}

		public RichPresence UpdateEndTime(DateTime time)
		{
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
			object sync = _sync;
			RichPresence richPresence;
			lock (sync)
			{
				if (CurrentPresence == null)
				{
					richPresence = new RichPresence();
				}
				else
				{
					richPresence = CurrentPresence.Clone();
				}
			}
			if (richPresence.Timestamps == null)
			{
				richPresence.Timestamps = new Timestamps();
			}
			richPresence.Timestamps.End = new DateTime?(time);
            SetPresence(richPresence);
			return richPresence;
		}

		public RichPresence UpdateClearTime()
		{
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
			object sync = _sync;
			RichPresence richPresence;
			lock (sync)
			{
				if (CurrentPresence == null)
				{
					richPresence = new RichPresence();
				}
				else
				{
					richPresence = CurrentPresence.Clone();
				}
			}
			richPresence.Timestamps = null;
            SetPresence(richPresence);
			return richPresence;
		}

		public void ClearPresence()
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException("Discord IPC Client");
			}
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
			if (connection == null)
			{
				throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
			}
            SetPresence(null);
		}


		public void Subscribe(EventType type)
		{
            SetSubscription(Subscription | type);
		}

		[Obsolete("Replaced with Unsubscribe", true)]
		public void Unubscribe(EventType type)
		{
            SetSubscription(Subscription & ~type);
		}

		public void Unsubscribe(EventType type)
		{
            SetSubscription(Subscription & ~type);
		}

		public void SetSubscription(EventType type)
		{
			if (IsInitialized)
			{
                SubscribeToTypes(Subscription & ~type, true);
                SubscribeToTypes(~Subscription & type, false);
			}
			else
			{
                Logger.Warning("Client has not yet initialized, but events are being subscribed too. Storing them as state instead.", Array.Empty<object>());
			}
			object sync = _sync;
			lock (sync)
			{
                Subscription = type;
			}
		}

		private void SubscribeToTypes(EventType type, bool isUnsubscribe)
		{
			if (type == EventType.None)
			{
				return;
			}
			if (IsDisposed)
			{
				throw new ObjectDisposedException("Discord IPC Client");
			}
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
			if (connection == null)
			{
				throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
			}
			if (!HasRegisteredUriScheme)
			{
				throw new InvalidConfigurationException("Cannot subscribe/unsubscribe to an event as this application has not registered a URI Scheme. Call RegisterUriScheme().");
			}
			if ((type & EventType.Spectate) == EventType.Spectate)
			{
                connection.EnqueueCommand(new SubscribeCommand
				{
					Event = ServerEvent.ActivitySpectate,
					IsUnsubscribe = isUnsubscribe
				});
			}
			if ((type & EventType.Join) == EventType.Join)
			{
                connection.EnqueueCommand(new SubscribeCommand
				{
					Event = ServerEvent.ActivityJoin,
					IsUnsubscribe = isUnsubscribe
				});
			}
			if ((type & EventType.JoinRequest) == EventType.JoinRequest)
			{
                connection.EnqueueCommand(new SubscribeCommand
				{
					Event = ServerEvent.ActivityJoinRequest,
					IsUnsubscribe = isUnsubscribe
				});
			}
		}

		public void SynchronizeState()
		{
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
            SetPresence(CurrentPresence);
			if (HasRegisteredUriScheme)
			{
                SubscribeToTypes(Subscription, false);
			}
		}

		public bool Initialize()
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException("Discord IPC Client");
			}
			if (IsInitialized)
			{
				throw new UninitializedException("Cannot initialize a client that is already initialized");
			}
			if (connection == null)
			{
				throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
			}
			return IsInitialized = connection.AttemptConnection();
		}

		public void Deinitialize()
		{
			if (!IsInitialized)
			{
				throw new UninitializedException("Cannot deinitialize a client that has not been initalized.");
			}
            connection.Close();
            IsInitialized = false;
		}

		public void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}
			if (IsInitialized)
			{
                Deinitialize();
			}
            IsDisposed = true;
		}

		private ILogger _logger;

		private RpcConnection connection;

		private bool _shutdownOnly = true;

		private object _sync = new object();
	}
}
