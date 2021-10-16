using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GmailWittMVC.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;

using System.Net.Http;
using System.Text;
using System.Threading;
using GmailWittMVC.Utilities;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;
using Newtonsoft.Json.Linq;


namespace GmailWittMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;


        static string[] Scopes =
        {
            GmailService.Scope.MailGoogleCom,
            GmailService.Scope.GmailAddonsCurrentMessageAction,



        };

        static string ApplicationName = "Gmail API .NET Quickstart";
        private const string client_Id = "897799633947-0dsuj8s3ecrq6pi2glc3n5r91hp332un.apps.googleusercontent.com";
        private const string client_Secret = "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";

        public HomeController(ILogger<HomeController> logger, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        
        [GoogleScopedAuthorize(GmailService.ScopeConstants.MailGoogleCom)]
        public async Task<IActionResult> Index([FromServices] IGoogleAuthProvider auth)
        {
            List<Message> _result = new List<Message>();
             GoogleCredential cred = await auth.GetCredentialAsync();
             var service = new GmailService(new BaseClientService.Initializer()
             {
                 HttpClientInitializer = cred,

             });

             UsersResource.MessagesResource.ListRequest _mess = service.Users.Messages.List("me");

             // _mess.Format = MessagesResource.GetRequest.FormatEnum.Full
             _mess.MaxResults = 30;
             _mess.IncludeSpamTrash = false;
             _mess.LabelIds = "INBOX";
             _mess.Q = "is:unread";
             
             var _messages = (await _mess.ExecuteAsync()).Messages;
             
             foreach (var item in _messages)
             {
                 var ms = service.Users.Messages.Get("me", item.Id);
                 ms.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Raw;
                 var msg = ms.Execute();
                 _result.Add(msg);
             }
             return View(_result);
             // var service = new DriveService(new BaseClientService.Initializer
             // {
             //     HttpClientInitializer = cred
             // });
             // var files = await service.Files.List().ExecuteAsync();
             // var fileNames = files.Files.Select(x => x.Name).ToList();
             // return View(fileNames);
        }

        // public async Task<IActionResult> Index()
        // {
        //
        //   
        //     // var _cs = new ClientSecrets { ClientId = client_Id, ClientSecret = client_Secret };
        //     // dsAuthorizationBroker.RedirectUri = "https://localhost:5001/home/auth";
        //     // UserCredential credential = await dsAuthorizationBroker.AuthorizeAsync(
        //     //     _cs,
        //     //     Scopes,
        //     //     "user",
        //     //     CancellationToken.None,
        //     //     null
        //     // );
        //     //
        //     // return Json( credential.RefreshTokenAsync(CancellationToken.None));
        // }

        // public async Task<IActionResult> auth(string Code, string scope)
        // {
        //     List<Message> _result = new List<Message>();
        //
        //
        //     using (var client = new HttpClient())
        //     {
        //
        //
        //         client.DefaultRequestHeaders.ExpectContinue = false;
        //         var result = client.PostAsync("https://www.googleapis.com/oauth2/v4/token",
        //             new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
        //             {
        //                 new KeyValuePair<string, string>("code", Code),
        //                 new KeyValuePair<string, string>("client_id", client_Id),
        //                 new KeyValuePair<string, string>("client_secret", client_Secret),
        //                 new KeyValuePair<string, string>("grant_type", "authorization_code"),
        //                 new KeyValuePair<string, string>("redirect_uri", "https://localhost:5001/home/auth"),
        //
        //             })).Result;
        //
        //         var _response = result.Content.ReadAsStringAsync().Result;
        //
        //         var obj = JObject.Parse(_response);
        //         string _token = (string)obj["access_token"];
        //         //var auth = new GoogleAuthProvider
        //         GoogleCredential cred = GoogleCredential.FromAccessToken(_token);
        //
        //
        //
        //         var service = new GmailService(new BaseClientService.Initializer()
        //         {
        //             HttpClientInitializer = cred,
        //
        //         });
        //
        //         UsersResource.MessagesResource.ListRequest _mess = service.Users.Messages.List("me");
        //
        //         // _mess.Format = MessagesResource.GetRequest.FormatEnum.Full
        //         _mess.MaxResults = 30;
        //         var _messages = await _mess.ExecuteAsync();
        //         IList<Message> m = _messages.Messages;
        //
        //
        //
        //         foreach (var item in m)
        //         {
        //             var ms = service.Users.Messages.Get("me", item.Id);
        //             ms.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Raw;
        //             var msg = ms.Execute();
        //             _result.Add(msg);
        //         }
        //
        //     }
        //
        //
        //
        //     return View(_result);
        //
        //
        // }

        

        [GoogleScopedAuthorize(GmailService.ScopeConstants.MailGoogleCom)]

        public async Task<IActionResult> Message(string Id, [FromServices] IGoogleAuthProvider auth)
        {
            GoogleCredential cred = await auth.GetCredentialAsync();
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = cred,

            });
            
            var ms = service.Users.Messages.Get("me", Id);
            
            ms.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Full;
            var msg = ms.Execute();

            ModifyMessageRequest _updateStatus = new ModifyMessageRequest();
            _updateStatus.AddLabelIds = null;
            _updateStatus.RemoveLabelIds = new List<string> {"UNREAD"};

            service.Users.Messages.Modify(_updateStatus, "me", Id).Execute();
            //
            return View(msg);
        }
        
 
        
         

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    
  

   

     
}