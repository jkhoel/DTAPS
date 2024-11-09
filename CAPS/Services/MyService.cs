using Microsoft.Extensions.Logging;

namespace CAPS.Services;

public interface IMyService
{
	void DoWork();
}

public partial class MyService : IMyService
{
	private readonly ILogger<MyService> _logger;

	public MyService(ILogger<MyService> logger)
	{
		_logger = logger;
	}

	public void DoWork()
	{
		_logger.LogInformation("Work is being done.");
	}
}