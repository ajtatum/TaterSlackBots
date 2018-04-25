using System;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using Serilog;

namespace SB.StanLee
{
    public class StanLeeWinService : IMicroService
    {
	    private IMicroServiceController controller;

	    public StanLeeWinService()
	    {
		    controller = null;
	    }

	    public StanLeeWinService(IMicroServiceController controller)
	    {
		    this.controller = controller;
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
