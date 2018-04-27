using System;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using Serilog;

namespace SB.StanLee
{
    public class StanLeeWinService : IMicroService
    {
	    private IMicroServiceController _controller;

	    public StanLeeWinService()
	    {
		    _controller = null;
	    }

	    public StanLeeWinService(IMicroServiceController controller)
	    {
		    _controller = controller;
	    }
		
	    public void Start()
	    {
		    Console.WriteLine("I started");
	    }

	    public void Stop()
	    {
			Log.CloseAndFlush();
		    Console.WriteLine("I stopped");
	    }
    }
}
