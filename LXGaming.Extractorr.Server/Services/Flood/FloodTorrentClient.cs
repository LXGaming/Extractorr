using System.Collections.Immutable;
using System.Net;
using System.Security.Authentication;
using System.Text.Encodings.Web;
using LXGaming.Extractorr.Server.Services.Flood.Models;
using LXGaming.Extractorr.Server.Services.Torrent.Client;
using LXGaming.Extractorr.Server.Utilities;

namespace LXGaming.Extractorr.Server.Services.Flood;

public class FloodTorrentClient : TorrentClientBase {

    private readonly object _lock;
    private bool _initialAuthentication;
    private volatile Task<bool> _authenticateTask;

    public FloodTorrentClient(TorrentClientOptions options, IServiceProvider serviceProvider)
        : base(options, serviceProvider) {
        _lock = new object();
        _initialAuthentication = true;
        _authenticateTask = AuthenticateInternalAsync();
    }

    public async Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> func,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        bool skipAuthentication = false, CancellationToken cancellationToken = default) {
        if (skipAuthentication) {
            using var request = func();
            return await HttpClient.SendAsync(request, completionOption, cancellationToken);
        }

        var existingAuthenticateTask = _authenticateTask;
        if (await existingAuthenticateTask) {
            using var request = func();
            var response = await HttpClient.SendAsync(request, completionOption, cancellationToken);
            if (response.StatusCode != HttpStatusCode.Forbidden) {
                return response;
            }

            response.Dispose();
        }

        if (_authenticateTask == existingAuthenticateTask) {
            lock (_lock) {
                if (_authenticateTask == existingAuthenticateTask) {
                    _authenticateTask = AuthenticateInternalAsync();
                }
            }
        }

        if (await _authenticateTask) {
            using var request = func();
            return await HttpClient.SendAsync(request, completionOption, cancellationToken);
        }

        throw new AuthenticationException("Authentication failed");
    }

    protected async Task<bool> AuthenticateInternalAsync() {
        try {
            var authenticate = await AuthenticateAsync();
            if (!authenticate.Success) {
                return false;
            }

            Logger.LogInformation("{State} with {Client} as {Username} ({Level})",
                _initialAuthentication ? "Authenticated" : "Reauthenticated", this, authenticate.Username,
                authenticate.Level);

            _initialAuthentication = false;
            return true;
        } catch (Exception ex) {
            Logger.LogWarning(ex, "Encountered an error while authenticating with {Client}", this);
            return false;
        }
    }

    #region Auth
    protected async Task<Authenticate> AuthenticateAsync() {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/authenticate");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string?> {
            { "username", Options.Username },
            { "password", Options.Password }
        });
        using var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        return await WebService.DeserializeAsync<Authenticate>(response);
    }
    #endregion

    #region Torrents
    public async Task<TorrentProperties?> GetTorrentAsync(string hash) {
        var torrentListSummary = await GetTorrentsAsync();
        return torrentListSummary.Torrents.GetValueOrDefault(hash);
    }

    public async Task<TorrentListSummary> GetTorrentsAsync() {
        using var response = await SendAsync(
            () => new HttpRequestMessage(HttpMethod.Get, "api/torrents"),
            HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        return await WebService.DeserializeAsync<TorrentListSummary>(response);
    }

    public async Task<ImmutableArray<TorrentContent>> GetTorrentContentsAsync(string hash) {
        var encodedHash = UrlEncoder.Default.Encode(hash);

        using var response = await SendAsync(
            () => new HttpRequestMessage(HttpMethod.Get, $"api/torrents/{encodedHash}/contents"),
            HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        return await WebService.DeserializeAsync<ImmutableArray<TorrentContent>>(response);
    }

    public async Task SetTorrentTagsAsync(IEnumerable<string> hashes, IEnumerable<string> tags) {
        using var response = await SendAsync(() => {
            var request = new HttpRequestMessage(HttpMethod.Patch, "api/torrents/tags");
            request.Content = JsonContent.Create(new Dictionary<string, object> {
                { "hashes", hashes },
                { "tags", tags },
            }, Constants.MediaTypeHeaderValues.ApplicationJson, WebService.JsonSerializerOptions);

            return request;
        }, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
    }
    #endregion
}