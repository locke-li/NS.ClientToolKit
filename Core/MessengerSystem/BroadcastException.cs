
using System;

namespace CenturyGame.Core.MessengerSystem
{
	public class BroadcastException : Exception
	{
		public BroadcastException(string msg) : base(msg)
		{
		}
	}
}
