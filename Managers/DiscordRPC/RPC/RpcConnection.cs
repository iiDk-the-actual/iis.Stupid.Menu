using iiMenu.Managers.DiscordRPC.Converters;
using iiMenu.Managers.DiscordRPC.Events;
using iiMenu.Managers.DiscordRPC.Helper;
using iiMenu.Managers.DiscordRPC.IO;
using iiMenu.Managers.DiscordRPC.Logging;
using iiMenu.Managers.DiscordRPC.Message;
using iiMenu.Managers.DiscordRPC.RPC.Commands;
using iiMenu.Managers.DiscordRPC.RPC.Payload;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC.RPC
{
	internal class RpcConnection : IDisposable
	{
		public ILogger Logger
		{
			get
			{
				return _logger;
			}
			set
			{
                _logger = value;
				if (namedPipe != null)
				{
                    namedPipe.Logger = value;
				}
			}
		}

		// (add) Token: 0x060000DC RID: 220 RVA: 0x000048E4 File Offset: 0x00002AE4
		// (remove) Token: 0x060000DD RID: 221 RVA: 0x0000491C File Offset: 0x00002B1C
		public event OnRpcMessageEvent OnRpcMessage;

		public RpcState State
		{
			get
			{
				object obj = l_states;
				RpcState state;
				lock (obj)
				{
					state = _state;
				}
				return state;
			}
		}

		public Configuration Configuration
		{
			get
			{
				Configuration result = null;
				object obj = l_config;
				lock (obj)
				{
					result = _configuration;
				}
				return result;
			}
		}

		public bool IsRunning
		{
			get
			{
				return thread != null;
			}
		}

		public bool ShutdownOnly { get; set; }

		public RpcConnection(string applicationID, int processID, int targetPipe, INamedPipeClient client, uint maxRxQueueSize = 128U, uint maxRtQueueSize = 512U)
		{
			this.applicationID = applicationID;
			this.processID = processID;
			this.targetPipe = targetPipe;
            namedPipe = client;
            ShutdownOnly = true;
            Logger = new ConsoleLogger();
            delay = new BackoffDelay(500, 60000);
            _maxRtQueueSize = maxRtQueueSize;
            _rtqueue = new Queue<ICommand>((int)(_maxRtQueueSize + 1U));
            _maxRxQueueSize = maxRxQueueSize;
            _rxqueue = new Queue<IMessage>((int)(_maxRxQueueSize + 1U));
            nonce = 0L;
		}

		private long GetNextNonce()
		{
            nonce += 1L;
			return nonce;
		}

		internal void EnqueueCommand(ICommand command)
		{
            Logger.Trace("Enqueue Command: {0}", new object[]
			{
				command.GetType().FullName
			});
			if (aborting || shutdown)
			{
				return;
			}
			object obj = l_rtqueue;
			lock (obj)
			{
				if (_rtqueue.Count == (long)(ulong)_maxRtQueueSize)
				{
                    Logger.Error("Too many enqueued commands, dropping oldest one. Maybe you are pushing new presences to fast?", Array.Empty<object>());
                    _rtqueue.Dequeue();
				}
                _rtqueue.Enqueue(command);
			}
		}

		private void EnqueueMessage(IMessage message)
		{
			try
			{
				if (OnRpcMessage != null)
				{
                    OnRpcMessage(this, message);
				}
			}
			catch (Exception ex)
			{
                Logger.Error("Unhandled Exception while processing event: {0}", new object[]
				{
					ex.GetType().FullName
				});
                Logger.Error(ex.Message, Array.Empty<object>());
                Logger.Error(ex.StackTrace, Array.Empty<object>());
			}
			if (_maxRxQueueSize <= 0U)
			{
                Logger.Trace("Enqueued Message, but queue size is 0.", Array.Empty<object>());
				return;
			}
            Logger.Trace("Enqueue Message: {0}", new object[]
			{
				message.Type
			});
			object obj = l_rxqueue;
			lock (obj)
			{
				if (_rxqueue.Count == (long)(ulong)_maxRxQueueSize)
				{
                    Logger.Warning("Too many enqueued messages, dropping oldest one.", Array.Empty<object>());
                    _rxqueue.Dequeue();
				}
                _rxqueue.Enqueue(message);
			}
		}

		internal IMessage DequeueMessage()
		{
			object obj = l_rxqueue;
			IMessage result;
			lock (obj)
			{
				if (_rxqueue.Count == 0)
				{
					result = null;
				}
				else
				{
					result = _rxqueue.Dequeue();
				}
			}
			return result;
		}

		internal IMessage[] DequeueMessages()
		{
			object obj = l_rxqueue;
			IMessage[] result;
			lock (obj)
			{
				IMessage[] array = _rxqueue.ToArray();
                _rxqueue.Clear();
				result = array;
			}
			return result;
		}

		private void MainLoop()
		{
            Logger.Info("RPC Connection Started", Array.Empty<object>());
			if (Logger.Level <= LogLevel.Trace)
			{
                Logger.Trace("============================", Array.Empty<object>());
                Logger.Trace("Assembly:             " + Assembly.GetAssembly(typeof(RichPresence)).FullName, Array.Empty<object>());
                Logger.Trace("Pipe:                 " + namedPipe.GetType().FullName, Array.Empty<object>());
                Logger.Trace("Platform:             " + Environment.OSVersion.ToString(), Array.Empty<object>());
                Logger.Trace("applicationID:        " + applicationID, Array.Empty<object>());
                Logger.Trace("targetPipe:           " + targetPipe.ToString(), Array.Empty<object>());
                Logger.Trace("POLL_RATE:            " + POLL_RATE.ToString(), Array.Empty<object>());
                Logger.Trace("_maxRtQueueSize:      " + _maxRtQueueSize.ToString(), Array.Empty<object>());
                Logger.Trace("_maxRxQueueSize:      " + _maxRxQueueSize.ToString(), Array.Empty<object>());
                Logger.Trace("============================", Array.Empty<object>());
			}
			while (!aborting && !shutdown)
			{
				try
				{
					if (namedPipe == null)
					{
                        Logger.Error("Something bad has happened with our pipe client!", Array.Empty<object>());
                        aborting = true;
						return;
					}
                    Logger.Trace("Connecting to the pipe through the {0}", new object[]
					{
                        namedPipe.GetType().FullName
					});
					if (namedPipe.Connect(targetPipe))
					{
                        Logger.Trace("Connected to the pipe. Attempting to establish handshake...", Array.Empty<object>());
                        EnqueueMessage(new ConnectionEstablishedMessage
						{
							ConnectedPipe = namedPipe.ConnectedPipe
						});
                        EstablishHandshake();
                        Logger.Trace("Connection Established. Starting reading loop...", Array.Empty<object>());
						bool flag = true;
						while (flag && !aborting && !shutdown && namedPipe.IsConnected)
						{
							PipeFrame frame;
							if (namedPipe.ReadFrame(out frame))
							{
                                Logger.Trace("Read Payload: {0}", new object[]
								{
									frame.Opcode
								});
								switch (frame.Opcode)
								{
								case Opcode.Frame:
								{
									if (shutdown)
									{
                                                Logger.Warning("Skipping frame because we are shutting down.", Array.Empty<object>());
										goto IL_46B;
									}
									if (frame.Data == null)
									{
                                                Logger.Error("We received no data from the frame so we cannot get the event payload!", Array.Empty<object>());
										goto IL_46B;
									}
									EventPayload eventPayload = null;
									try
									{
										eventPayload = frame.GetObject<EventPayload>();
									}
									catch (Exception ex)
									{
                                                Logger.Error("Failed to parse event! {0}", new object[]
										{
											ex.Message
										});
                                                Logger.Error("Data: {0}", new object[]
										{
											frame.Message
										});
									}
									try
									{
										if (eventPayload != null)
										{
                                                    ProcessFrame(eventPayload);
										}
										goto IL_46B;
									}
									catch (Exception ex2)
									{
                                                Logger.Error("Failed to process event! {0}", new object[]
										{
											ex2.Message
										});
                                                Logger.Error("Data: {0}", new object[]
										{
											frame.Message
										});
										goto IL_46B;
									}
								}
								case Opcode.Close:
								{
									ClosePayload @object = frame.GetObject<ClosePayload>();
                                            Logger.Warning("We have been told to terminate by discord: ({0}) {1}", new object[]
									{
										@object.Code,
										@object.Reason
									});
                                            EnqueueMessage(new CloseMessage
									{
										Code = @object.Code,
										Reason = @object.Reason
									});
									flag = false;
									goto IL_46B;
								}
								case Opcode.Ping:
                                        Logger.Trace("PING", Array.Empty<object>());
									frame.Opcode = Opcode.Pong;
                                        namedPipe.WriteFrame(frame);
									goto IL_46B;
								case Opcode.Pong:
                                        Logger.Trace("PONG", Array.Empty<object>());
									goto IL_46B;
								}
                                Logger.Error("Invalid opcode: {0}", new object[]
								{
									frame.Opcode
								});
								flag = false;
							}
							IL_46B:
							if (!aborting && namedPipe.IsConnected)
							{
                                ProcessCommandQueue();
                                queueUpdatedEvent.WaitOne(POLL_RATE);
							}
						}
                        Logger.Trace("Left main read loop for some reason. Aborting: {0}, Shutting Down: {1}", new object[]
						{
                            aborting,
                            shutdown
                        });
					}
					else
					{
                        Logger.Error("Failed to connect for some reason.", Array.Empty<object>());
                        EnqueueMessage(new ConnectionFailedMessage
						{
							FailedPipe = targetPipe
                        });
					}
					if (!aborting && !shutdown)
					{
						long num = delay.NextDelay();
                        Logger.Trace("Waiting {0}ms before attempting to connect again", new object[]
						{
							num
						});
						Thread.Sleep(delay.NextDelay());
					}
				}
				catch (Exception ex3)
				{
                    Logger.Error("Unhandled Exception: {0}", new object[]
					{
						ex3.GetType().FullName
					});
                    Logger.Error(ex3.Message, Array.Empty<object>());
                    Logger.Error(ex3.StackTrace, Array.Empty<object>());
				}
				finally
				{
					if (namedPipe.IsConnected)
					{
                        Logger.Trace("Closing the named pipe.", Array.Empty<object>());
                        namedPipe.Close();
					}
                    SetConnectionState(RpcState.Disconnected);
				}
			}
            Logger.Trace("Left Main Loop", Array.Empty<object>());
			if (namedPipe != null)
			{
                namedPipe.Dispose();
			}
            Logger.Info("Thread Terminated, no longer performing RPC connection.", Array.Empty<object>());
		}

		private void ProcessFrame(EventPayload response)
		{
            Logger.Info("Handling Response. Cmd: {0}, Event: {1}", new object[]
			{
				response.Command,
				response.Event
			});
			if (response.Event != null && response.Event.Value == ServerEvent.Error)
			{
                Logger.Error("Error received from the RPC", Array.Empty<object>());
				ErrorMessage @object = response.GetObject<ErrorMessage>();
                Logger.Error("Server responded with an error message: ({0}) {1}", new object[]
				{
					@object.Code.ToString(),
					@object.Message
				});
                EnqueueMessage(@object);
				return;
			}
			if (State == RpcState.Connecting && response.Command == Command.Dispatch && response.Event != null && response.Event.Value == ServerEvent.Ready)
			{
                Logger.Info("Connection established with the RPC", Array.Empty<object>());
                SetConnectionState(RpcState.Connected);
                delay.Reset();
				ReadyMessage object2 = response.GetObject<ReadyMessage>();
				object obj = l_config;
				lock (obj)
				{
                    _configuration = object2.Configuration;
					object2.User.SetConfiguration(_configuration);
				}
                EnqueueMessage(object2);
				return;
			}
			if (State != RpcState.Connected)
			{
                Logger.Trace("Received a frame while we are disconnected. Ignoring. Cmd: {0}, Event: {1}", new object[]
				{
					response.Command,
					response.Event
				});
				return;
			}
			switch (response.Command)
			{
			case Command.Dispatch:
                    ProcessDispatch(response);
				return;
			case Command.SetActivity:
			{
				if (response.Data == null)
				{
                            EnqueueMessage(new PresenceMessage());
					return;
				}
				RichPresenceResponse object3 = response.GetObject<RichPresenceResponse>();
                        EnqueueMessage(new PresenceMessage(object3));
				return;
			}
			case Command.Subscribe:
			case Command.Unsubscribe:
			{
				new JsonSerializer().Converters.Add(new EnumSnakeCaseConverter());
				ServerEvent value = response.GetObject<EventPayload>().Event.Value;
				if (response.Command == Command.Subscribe)
				{
                            EnqueueMessage(new SubscribeMessage(value));
					return;
				}
                        EnqueueMessage(new UnsubscribeMessage(value));
				return;
			}
			case Command.SendActivityJoinInvite:
                    Logger.Trace("Got invite response ack.", Array.Empty<object>());
				return;
			case Command.CloseActivityJoinRequest:
                    Logger.Trace("Got invite response reject ack.", Array.Empty<object>());
				return;
			default:
                    Logger.Error("Unkown frame was received! {0}", new object[]
				{
					response.Command
				});
				return;
			}
		}

		private void ProcessDispatch(EventPayload response)
		{
			if (response.Command != Command.Dispatch)
			{
				return;
			}
			if (response.Event == null)
			{
				return;
			}
			switch (response.Event.Value)
			{
			case ServerEvent.ActivityJoin:
			{
				JoinMessage @object = response.GetObject<JoinMessage>();
                        EnqueueMessage(@object);
				return;
			}
			case ServerEvent.ActivitySpectate:
			{
				SpectateMessage object2 = response.GetObject<SpectateMessage>();
                        EnqueueMessage(object2);
				return;
			}
			case ServerEvent.ActivityJoinRequest:
			{
				JoinRequestMessage object3 = response.GetObject<JoinRequestMessage>();
                        EnqueueMessage(object3);
				return;
			}
			default:
                    Logger.Warning("Ignoring {0}", new object[]
				{
					response.Event.Value
				});
				return;
			}
		}

		private void ProcessCommandQueue()
		{
			if (State != RpcState.Connected)
			{
				return;
			}
			if (aborting)
			{
                Logger.Warning("We have been told to write a queue but we have also been aborted.", Array.Empty<object>());
			}
			bool flag = true;
			ICommand command = null;
			while (flag && namedPipe.IsConnected)
			{
				object obj = l_rtqueue;
				lock (obj)
				{
					flag = _rtqueue.Count > 0;
					if (!flag)
					{
						break;
					}
					command = _rtqueue.Peek();
				}
				if (shutdown || !aborting && LOCK_STEP)
				{
					flag = false;
				}
				IPayload payload = command.PreparePayload(GetNextNonce());
                Logger.Trace("Attempting to send payload: {0}", new object[]
				{
					payload.Command
				});
				PipeFrame frame = default;
				if (command is CloseCommand)
				{
                    SendHandwave();
                    Logger.Trace("Handwave sent, ending queue processing.", Array.Empty<object>());
					obj = l_rtqueue;
					lock (obj)
					{
                        _rtqueue.Dequeue();
					}
					return;
				}
				if (aborting)
				{
                    Logger.Warning("- skipping frame because of abort.", Array.Empty<object>());
					obj = l_rtqueue;
					lock (obj)
					{
                        _rtqueue.Dequeue();
						continue;
					}
				}
				frame.SetObject(Opcode.Frame, payload);
                Logger.Trace("Sending payload: {0}", new object[]
				{
					payload.Command
				});
				if (namedPipe.WriteFrame(frame))
				{
                    Logger.Trace("Sent Successfully.", Array.Empty<object>());
					obj = l_rtqueue;
					lock (obj)
					{
                        _rtqueue.Dequeue();
						continue;
					}
				}
                Logger.Warning("Something went wrong during writing!", Array.Empty<object>());
				return;
			}
		}

		private void EstablishHandshake()
		{
            Logger.Trace("Attempting to establish a handshake...", Array.Empty<object>());
			if (State != RpcState.Disconnected)
			{
                Logger.Error("State must be disconnected in order to start a handshake!", Array.Empty<object>());
				return;
			}
            Logger.Trace("Sending Handshake...", Array.Empty<object>());
			if (!namedPipe.WriteFrame(new PipeFrame(Opcode.Handshake, new Handshake
			{
				Version = VERSION,
				ClientID = applicationID
            })))
			{
                Logger.Error("Failed to write a handshake.", Array.Empty<object>());
				return;
			}
            SetConnectionState(RpcState.Connecting);
		}

		private void SendHandwave()
		{
            Logger.Info("Attempting to wave goodbye...", Array.Empty<object>());
			if (State == RpcState.Disconnected)
			{
                Logger.Error("State must NOT be disconnected in order to send a handwave!", Array.Empty<object>());
				return;
			}
			if (!namedPipe.WriteFrame(new PipeFrame(Opcode.Close, new Handshake
			{
				Version = VERSION,
				ClientID = applicationID
            })))
			{
                Logger.Error("failed to write a handwave.", Array.Empty<object>());
				return;
			}
		}

		public bool AttemptConnection()
		{
            Logger.Info("Attempting a new connection", Array.Empty<object>());
			if (thread != null)
			{
                Logger.Error("Cannot attempt a new connection as the previous connection thread is not null!", Array.Empty<object>());
				return false;
			}
			if (State != RpcState.Disconnected)
			{
                Logger.Warning("Cannot attempt a new connection as the previous connection hasn't changed state yet.", Array.Empty<object>());
				return false;
			}
			if (aborting)
			{
                Logger.Error("Cannot attempt a new connection while aborting!", Array.Empty<object>());
				return false;
			}
            thread = new Thread(new ThreadStart(MainLoop));
            thread.Name = "Discord IPC Thread";
            thread.IsBackground = true;
            thread.Start();
			return true;
		}

		private void SetConnectionState(RpcState state)
		{
            Logger.Trace("Setting the connection state to {0}", new object[]
			{
				state.ToString().ToSnakeCase().ToUpperInvariant()
			});
			object obj = l_states;
			lock (obj)
			{
                _state = state;
			}
		}

		public void Shutdown()
		{
            Logger.Trace("Initiated shutdown procedure", Array.Empty<object>());
            shutdown = true;
			object obj = l_rtqueue;
			lock (obj)
			{
                _rtqueue.Clear();
				if (CLEAR_ON_SHUTDOWN)
				{
                    _rtqueue.Enqueue(new PresenceCommand
					{
						PID = processID,
						Presence = null
					});
				}
                _rtqueue.Enqueue(new CloseCommand());
			}
            queueUpdatedEvent.Set();
		}

		public void Close()
		{
			if (thread == null)
			{
                Logger.Error("Cannot close as it is not available!", Array.Empty<object>());
				return;
			}
			if (aborting)
			{
                Logger.Error("Cannot abort as it has already been aborted", Array.Empty<object>());
				return;
			}
			if (ShutdownOnly)
			{
                Shutdown();
				return;
			}
            Logger.Trace("Updating Abort State...", Array.Empty<object>());
            aborting = true;
            queueUpdatedEvent.Set();
		}

		public void Dispose()
		{
            ShutdownOnly = false;
            Close();
		}

		public static readonly int VERSION = 1;

		public static readonly int POLL_RATE = 1000;

		private static readonly bool CLEAR_ON_SHUTDOWN = true;

		private static readonly bool LOCK_STEP = false;

		private ILogger _logger;

		private RpcState _state;

		private readonly object l_states = new object();

		private Configuration _configuration;

		private readonly object l_config = new object();

		private volatile bool aborting;

		private volatile bool shutdown;

		private string applicationID;

		private int processID;

		private long nonce;

		private Thread thread;

		private INamedPipeClient namedPipe;

		private int targetPipe;

		private readonly object l_rtqueue = new object();

		private readonly uint _maxRtQueueSize;

		private Queue<ICommand> _rtqueue;

		private readonly object l_rxqueue = new object();

		private readonly uint _maxRxQueueSize;

		private Queue<IMessage> _rxqueue;

		private AutoResetEvent queueUpdatedEvent = new AutoResetEvent(false);

		private BackoffDelay delay;
	}
}
