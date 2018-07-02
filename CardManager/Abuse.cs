using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;

namespace CardManager
{
	class Abuse
	{
		
		bool IsFirstTurn = true;
		//Thread thread;

		List<string> CheckedGames = new List<string>();

		public int Amount { get; set; }
	}
}
