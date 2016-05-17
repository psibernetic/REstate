﻿using Nancy;
using Nancy.Cryptography;
using Nancy.ModelBinding;
using Nancy.Owin;
using Nancy.Responses.Negotiation;
using REstate.Web.Auth.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Psibr.Platform;
using Psibr.Platform.Repositories;
using REstate.Platform;
using SignInDelegate = System.Func<System.Func<System.Guid, System.Collections.Generic.IDictionary<string, object>>, bool, string>;

namespace REstate.Web.Auth.Modules
{
    public class AuthenticationModule
        : NancyModule
    {

        public AuthenticationModule(AuthRoutePrefix prefix, REstatePlatformConfiguration configuration,
            IAuthRepositoryContextFactory authRepositoryContextFactory, CryptographyConfiguration crypto)
            : base(prefix)
        {

            Get["/login"] = async (parameters, ct) =>
                await Task.FromResult(View["login.html"]);


            Post["/login"] = async (parameters, ct) =>
            {
                var credentials = this.Bind<CredentialAuthenticationRequest>();

                if (string.IsNullOrWhiteSpace(credentials?.Username) || string.IsNullOrWhiteSpace(credentials.Password))
                    return Response.AsRedirect(configuration.AuthHttpService.Address + "login");

                var environment = Context.GetOwinEnvironment();
                var signInDelegate = (SignInDelegate)environment["jwtandcookie.signin"];

                var passwordHash = Convert.ToBase64String(crypto.HmacProvider
                    .GenerateHmac(credentials.Password));

                IPrincipal principal;
                using (var repository = authRepositoryContextFactory.OpenAuthRepositoryContext())
                {
                    principal = await repository
                        .LoadPrincipalByCredentials(credentials.Username, passwordHash, ct);
                }

                if (principal == null) return Response.AsRedirect(configuration.AuthHttpService.Address + "login");

                var jwt = signInDelegate((jti) => new Dictionary<string, object>
                {
                    { "sub", principal.UserOrApplicationName},
                    { "apikey", crypto.EncryptionProvider.Encrypt(jti + principal.ApiKey)},
                    { "claims", principal.Claims }
                }, true);

                return Response.AsRedirect(configuration.AdminHttpService.Address);
            };

            Post["/apikey"] = async (parameters, ct) =>
            {
                var apiKey = this.Bind<ApiKeyAuthenticationRequest>().ApiKey;

                var environment = Context.GetOwinEnvironment();
                var signInDelegate = (SignInDelegate)environment["jwtandcookie.signin"];

                IPrincipal principal;
                using (var repository = authRepositoryContextFactory.OpenAuthRepositoryContext())
                {
                    principal = await repository.LoadPrincipalByApiKey(apiKey, ct);
                }

                if (principal == null) return 401;

                var jwt = signInDelegate((jti) => new Dictionary<string, object>
                {
                    { "sub", principal.UserOrApplicationName},
                    { "apikey", crypto.EncryptionProvider.Encrypt(jti + principal.ApiKey)},
                    { "claims", principal.Claims }
                }, false);

                return Negotiate
                    .WithModel(await Task.FromResult(new { jwt }))
                    .WithAllowedMediaRange(new MediaRange("application/json"));
            };
        }
    }
}