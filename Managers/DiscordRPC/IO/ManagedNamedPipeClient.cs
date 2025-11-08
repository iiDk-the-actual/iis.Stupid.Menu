using iiMenu.Managers.DiscordRPC.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace iiMenu.Managers.DiscordRPC.IO
{
	public sealed class ManagedNamedPipeClient : INamedPipeClient, IDisposable
	{
		public ILogger Logger { get; set; }

		public bool IsConnected
		{
			get
			{
				if (_isClosed)
				{
					return false;
				}
				object obj = l_stream;
				bool result;
				lock (obj)
				{
					result = _stream != null && _stream.IsConnected;
				}
				return result;
			}
		}

		public int ConnectedPipe
		{
			get
			{
				return _connectedPipe;
			}
		}

		public ManagedNamedPipeClient()
		{
            _buffer = new byte[PipeFrame.MAX_SIZE];
            Logger = new NullLogger();
            _stream = null;
		}

		public bool Connect(int pipe)
		{
            Logger.Trace("ManagedNamedPipeClient.Connection({0})", new object[]
			{
				pipe
			});
			if (_isDisposed)
			{
				throw new ObjectDisposedException("NamedPipe");
			}
			if (pipe > 9)
			{
				throw new ArgumentOutOfRangeException("pipe", "Argument cannot be greater than 9");
			}
			if (pipe < 0)
			{
				for (int i = 0; i < 10; i++)
				{
					if (AttemptConnection(i, false) || AttemptConnection(i, true))
					{
                        BeginReadStream();
						return true;
					}
				}
			}
			else if (AttemptConnection(pipe, false) || AttemptConnection(pipe, true))
			{
                BeginReadStream();
				return true;
			}
			return false;
		}

		private bool AttemptConnection(int pipe, bool isSandbox = false)
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException("_stream");
			}
			string text = isSandbox ? GetPipeSandbox() : "";
			if (isSandbox && text == null)
			{
                Logger.Trace("Skipping sandbox connection.", Array.Empty<object>());
				return false;
			}
            Logger.Trace("Connection Attempt {0} ({1})", new object[]
			{
				pipe,
				text
			});
			string pipeName = GetPipeName(pipe, text);
			try
			{
				object obj = l_stream;
				lock (obj)
				{
                    Logger.Info("Attempting to connect to '{0}'", new object[]
					{
						pipeName
					});
                    _stream = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                    _stream.Connect(0);
                    Logger.Trace("Waiting for connection...", Array.Empty<object>());
					do
					{
						Thread.Sleep(10);
					}
					while (!_stream.IsConnected);
				}
                Logger.Info("Connected to '{0}'", new object[]
				{
					pipeName
				});
                _connectedPipe = pipe;
                _isClosed = false;
			}
			catch (Exception ex)
			{
                Logger.Error("Failed connection to {0}. {1}", new object[]
				{
					pipeName,
					ex.Message
				});
                Close();
			}
            Logger.Trace("Done. Result: {0}", new object[]
			{
                _isClosed
            });
			return !_isClosed;
		}

		private void BeginReadStream()
		{
			if (_isClosed)
			{
				return;
			}
			try
			{
				object obj = l_stream;
				lock (obj)
				{
					if (_stream != null && _stream.IsConnected)
					{
                        Logger.Trace("Begining Read of {0} bytes", new object[]
						{
                            _buffer.Length
						});
                        _stream.BeginRead(_buffer, 0, _buffer.Length, new AsyncCallback(EndReadStream), _stream.IsConnected);
					}
				}
			}
			catch (ObjectDisposedException)
			{
                Logger.Warning("Attempted to start reading from a disposed pipe", Array.Empty<object>());
			}
			catch (InvalidOperationException)
			{
                Logger.Warning("Attempted to start reading from a closed pipe", Array.Empty<object>());
			}
			catch (Exception ex)
			{
                Logger.Error("An exception occured while starting to read a stream: {0}", new object[]
				{
					ex.Message
				});
                Logger.Error(ex.StackTrace, Array.Empty<object>());
			}
		}

		private void EndReadStream(IAsyncResult callback)
		{
            Logger.Trace("Ending Read", Array.Empty<object>());
			int num = 0;
			try
			{
				object framequeuelock = l_stream;
				lock (framequeuelock)
				{
					if (_stream == null || !_stream.IsConnected)
					{
						return;
					}
					num = _stream.EndRead(callback);
				}
			}
			catch (IOException)
			{
                Logger.Warning("Attempted to end reading from a closed pipe", Array.Empty<object>());
				return;
			}
			catch (NullReferenceException)
			{
                Logger.Warning("Attempted to read from a null pipe", Array.Empty<object>());
				return;
			}
			catch (ObjectDisposedException)
			{
                Logger.Warning("Attemped to end reading from a disposed pipe", Array.Empty<object>());
				return;
			}
			catch (Exception ex)
			{
                Logger.Error("An exception occured while ending a read of a stream: {0}", new object[]
				{
					ex.Message
				});
                Logger.Error(ex.StackTrace, Array.Empty<object>());
				return;
			}
            Logger.Trace("Read {0} bytes", new object[]
			{
				num
			});
			if (num > 0)
			{
				using (MemoryStream memoryStream = new MemoryStream(_buffer, 0, num))
				{
					try
					{
						PipeFrame item = default;
						if (item.ReadStream(memoryStream))
						{
                            Logger.Trace("Read a frame: {0}", new object[]
							{
								item.Opcode
							});
							object framequeuelock = _framequeuelock;
							lock (framequeuelock)
							{
                                _framequeue.Enqueue(item);
								goto IL_19E;
							}
						}
                        Logger.Error("Pipe failed to read from the data received by the stream.", Array.Empty<object>());
                        Close();
						IL_19E:
						goto IL_218;
					}
					catch (Exception ex2)
					{
                        Logger.Error("A exception has occured while trying to parse the pipe data: {0}", new object[]
						{
							ex2.Message
						});
                        Close();
						goto IL_218;
					}
				}
			}
			if (IsUnix())
			{
                Logger.Error("Empty frame was read on {0}, aborting.", new object[]
				{
					Environment.OSVersion
				});
                Close();
			}
			else
			{
                Logger.Warning("Empty frame was read. Please send report to Lachee.", Array.Empty<object>());
			}
			IL_218:
			if (!_isClosed && IsConnected)
			{
                Logger.Trace("Starting another read", Array.Empty<object>());
                BeginReadStream();
			}
		}

		public bool ReadFrame(out PipeFrame frame)
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException("_stream");
			}
			object framequeuelock = _framequeuelock;
			bool result;
			lock (framequeuelock)
			{
				if (_framequeue.Count == 0)
				{
					frame = default;
					result = false;
				}
				else
				{
					frame = _framequeue.Dequeue();
					result = true;
				}
			}
			return result;
		}

		public bool WriteFrame(PipeFrame frame)
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException("_stream");
			}
			if (_isClosed || !IsConnected)
			{
                Logger.Error("Failed to write frame because the stream is closed", Array.Empty<object>());
				return false;
			}
			try
			{
				frame.WriteStream(_stream);
				return true;
			}
			catch (IOException ex)
			{
                Logger.Error("Failed to write frame because of a IO Exception: {0}", new object[]
				{
					ex.Message
				});
			}
			catch (ObjectDisposedException)
			{
                Logger.Warning("Failed to write frame as the stream was already disposed", Array.Empty<object>());
			}
			catch (InvalidOperationException)
			{
                Logger.Warning("Failed to write frame because of a invalid operation", Array.Empty<object>());
			}
			return false;
		}

		public void Close()
		{
			if (_isClosed)
			{
                Logger.Warning("Tried to close a already closed pipe.", Array.Empty<object>());
				return;
			}
			try
			{
				object obj = l_stream;
				lock (obj)
				{
					if (_stream != null)
					{
						try
						{
                            _stream.Flush();
                            _stream.Dispose();
						}
						catch (Exception)
						{
						}
                        _stream = null;
                        _isClosed = true;
					}
					else
					{
                        Logger.Warning("Stream was closed, but no stream was available to begin with!", Array.Empty<object>());
					}
				}
			}
			catch (ObjectDisposedException)
			{
                Logger.Warning("Tried to dispose already disposed stream", Array.Empty<object>());
			}
			finally
			{
                _isClosed = true;
                _connectedPipe = -1;
			}
		}

		public void Dispose()
		{
			if (_isDisposed)
			{
				return;
			}
			if (!_isClosed)
			{
                Close();
			}
			object obj = l_stream;
			lock (obj)
			{
				if (_stream != null)
				{
                    _stream.Dispose();
                    _stream = null;
				}
			}
            _isDisposed = true;
		}

		public static string GetPipeName(int pipe, string sandbox)
		{
			if (!IsUnix())
			{
				return sandbox + string.Format("discord-ipc-{0}", pipe);
			}
			return Path.Combine(GetTemporaryDirectory(), sandbox + string.Format("discord-ipc-{0}", pipe));
		}

		public static string GetPipeName(int pipe)
		{
			return GetPipeName(pipe, "");
		}

		public static string GetPipeSandbox()
		{
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				return null;
			}
			return "snap.discord/";
		}

		private static string GetTemporaryDirectory()
		{
			return ((((null ?? Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR")) ?? Environment.GetEnvironmentVariable("TMPDIR")) ?? Environment.GetEnvironmentVariable("TMP")) ?? Environment.GetEnvironmentVariable("TEMP")) ?? "/tmp";
		}

		public static bool IsUnix()
		{
			PlatformID platform = Environment.OSVersion.Platform;
			return platform == PlatformID.Unix || platform == PlatformID.MacOSX;
		}

		private const string PIPE_NAME = "discord-ipc-{0}";

		private int _connectedPipe;

		private NamedPipeClientStream _stream;

		private byte[] _buffer = new byte[PipeFrame.MAX_SIZE];

		private Queue<PipeFrame> _framequeue = new Queue<PipeFrame>();

		private object _framequeuelock = new object();

		private volatile bool _isDisposed;

		private volatile bool _isClosed = true;

		private object l_stream = new object();
	}
}
