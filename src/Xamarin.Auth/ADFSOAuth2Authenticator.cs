using System;
using System.Threading.Tasks;

namespace Xamarin.Auth
{
#if XAMARIN_AUTH_INTERNAL
	internal class ADFSOAuth2Authenticator : OAuth2Authenticator
#else
	public class ADFSOAuth2Authenticator : OAuth2Authenticator
#endif
	{
		public ADFSOAuth2Authenticator(
			string clientId,
			string resource,
			Uri authorizeUrl,
			Uri redirectUrl,
			Uri accessTokenUrl,
			GetUsernameAsyncFunc getUsernameAsync = null)
			: base(clientId, resource, authorizeUrl, redirectUrl, getUsernameAsync)
		{
			if (accessTokenUrl == null) {
				throw new ArgumentNullException("accessTokenUrl");
			}
			this.accessTokenUrl = accessTokenUrl;
		}

		public override Task<Uri> GetInitialUrlAsync()
		{
			var url = new Uri(string.Format(
				          "{0}?client_id={1}&redirect_uri={2}&response_type={3}&resource={4}&state={5}",
				          AuthorizeUrl.AbsoluteUri,
				          Uri.EscapeDataString(ClientId),
				          Uri.EscapeDataString(RedirectUrl.AbsoluteUri),
				          "code",
				          Uri.EscapeDataString(Resource),
				          Uri.EscapeDataString(requestState)));

			var tcs = new TaskCompletionSource<Uri>();
			tcs.SetResult(url);
			return tcs.Task;
		}

		public String Resource {
			get { return Scope; }
		}

		public override bool ShouldLoadPage (Uri url)
		{
			if (UrlMatchesRedirect(url)) {
				return false;
			}
			return true;
		}

		private bool UrlMatchesRedirect (Uri url)
		{
			return url.Host == RedirectUrl.Host && url.LocalPath == RedirectUrl.LocalPath;
		}
	}
}

