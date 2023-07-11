using Microsoft.Extensions.Logging;
using Octokit;

namespace GithubClassTracker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        // OptionsPattern
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service starting...");
            string owner = "mynkow";
            string repoName = "gg"; ;


            ICredentialStore credStore = new LocalCredStore();
            var client = new GitHubClient(new ProductHeaderValue(owner), credStore);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Repository repo = await client.Repository.Get(owner, repoName);

                    var commitOptions = new ApiOptions()
                    {
                        StartPage = 1,
                        PageCount = 1,
                        PageSize = 1
                    };

                    var lastCommitCollection = await client.Repository.Commit.GetAll(owner, repoName, commitOptions);
                    var lastCommitMeta = lastCommitCollection.FirstOrDefault();

                    if (lastCommitMeta is not null)
                    {
                        _logger.LogInformation(lastCommitMeta.Commit.Message);

                        var lastCommit = await client.Repository.Commit.Get(repo.Id, lastCommitMeta.Sha);


                        bool hasBadFiles = lastCommit.Files.Where(file => file.Filename.EndsWith("class1.cs", StringComparison.OrdinalIgnoreCase)).Any();
                        if (hasBadFiles)
                        {
                            // send email
                            _logger.LogWarning("We have a bad file");
                        }


                        // github repo
                        // if commit contains class1.cs
                        // email
                    }
                    await Task.Delay(5000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to get lastCommit");
                }
            }
        }
    }

    public class LocalCredStore : ICredentialStore
    {
        public Task<Credentials> GetCredentials()
        {
            Credentials cred = new Credentials("your_token_here");

            return Task.FromResult(cred);
        }
    }
}
