using ComplainerProd.Models;
using ComplainerProd.Test;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Popups;

namespace ComplainerProd.DataObject
{
    public enum HTTPMETHOD{
        GET,
        POST,
        PUT,
        DELETE
    }
    public class ServerObject
    {
        public IMobileServiceTable<User> UserTable;

        public IMobileServiceTable<Channel> ChannelTable;

        public IMobileServiceTable<Feedback> FeedbackTable;

        public IMobileServiceTable<Voting> VotingTable;

        public IMobileServiceTable<Comment> CommentTable;

        public IMobileServiceTable<ChannelStatistic> ChannelStatisticTable;


        public void IntializeTables()
        {
            UserTable = App.MobileService.GetTable<User>();
            ChannelTable = App.MobileService.GetTable<Channel>();
            FeedbackTable = App.MobileService.GetTable<Feedback>();
            VotingTable = App.MobileService.GetTable<Voting>();
            CommentTable = App.MobileService.GetTable<Comment>();
            ChannelStatisticTable = App.MobileService.GetTable<ChannelStatistic>();

        }

        public async Task<Feedback> AddFeedbackAsync(Feedback feedback)
        {
            Feedback result = null;
            try
            {
                JObject payload = JObject.FromObject(feedback);
                payload.Remove("Id");
                payload.Remove("createdAt");
                string response = await CallAPI(@"api/Feedbacks", HTTPMETHOD.POST, payload);
                result = JsonConvert.DeserializeObject<Feedback>(response);
            }
            catch (Exception e)
            {
                await HelpersClass.ShowDialogAsync("Something went wrong:" + e.Message);
            }
            return result;
        }

        public async Task<List<Feedback>> GetFeedbackByUserAsync(string userId)
        {
            List<Feedback> result = new List<Feedback>();
            try
            {
                string response = await CallAPI($@"api/Feedbacks/User/{userId}", HTTPMETHOD.GET, null);
                result = JsonConvert.DeserializeObject<List<Feedback>>(response);
            }
            catch (Exception e)
            {
                await HelpersClass.ShowDialogAsync("Something went wrong:" + e.Message);
            }
            return result;
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksByChannelAsync(int id)
        {
            IEnumerable<Feedback> results = null;
            try
            {
                string response = await CallAPI($@"api/Feedbacks/Channel/{id}/{MainPage.instance.user.UserId}", HTTPMETHOD.GET, null);
                results = JsonConvert.DeserializeObject<IEnumerable<Feedback>>(response);
            }
            catch (Exception e)
            {
                await HelpersClass.ShowDialogAsync("Something went wrong:" + e.Message);
            }

            return results;
           
        }

        public async Task<bool> CheckAccessCodeValid(string accessCode)
        {
            try
            {
                return await App.MobileService
                .InvokeApiAsync<bool>("Admin", HttpMethod.Post, new Dictionary<string, string>() { { "accessCode", accessCode } });
            }
            catch(Exception e)
            {
                await HelpersClass.ShowDialogAsync("Check Access code valid failed: " + e.Message);
            }
            return true;
        }

        public async Task<bool> AddChannelAsync(Channel c)
        {
            bool result = false;

            if ((c.Name.Replace(" ", "")).Count() == 0)
                return false;
            if ((c.AccessCode.Replace(" ", "")).Count() == 0)
                return false;
            try
            {
                JObject payload = JObject.FromObject(c);
                payload.Remove("Id");

                string response = await CallAPI(@"api/Channels", HTTPMETHOD.POST, payload);
                result = true;
            }catch(Exception e)
            {
                await HelpersClass.ShowDialogAsync("Something went wrong:" + e.Message);   
            }
            return result;
        } //done

        public async Task<IEnumerable<Channel>> GetChannelAsync(string userId)
        {
            IEnumerable<Channel> results = null;
            try
            {
                string response = await CallAPI($@"api/Channels/user/{userId}", HTTPMETHOD.GET, null);
                results = JsonConvert.DeserializeObject<IEnumerable<Channel>>(response);
            }
            catch (Exception e)
            {
                await HelpersClass.ShowDialogAsync("Something went wrong:" + e.Message);
            }
            return results;
        } //done

        public async Task<Channel> GetChannelByAccessCode(string accessCode)
        {
            Channel result = null;
            try
            {
                string response = await CallAPI($@"api/Channels/Access/{accessCode}", HTTPMETHOD.GET, null);
                result = JsonConvert.DeserializeObject<Channel>(response);
            }
            catch (Exception e)
            {
                return null;
            }
            return result;
        }

        public async Task<bool> ActionChange(Feedback fb)
        {
            try
            {
                var payload = JObject.FromObject(fb);
                var feedback = await CallAPI($@"api/Feedbacks/{fb.Id}", HTTPMETHOD.PUT, payload);

                if(feedback != null)
                {
                    return true;
                }
                return false;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public async Task<Comment> AddComment(Comment c)
        {
            try
            {
                JObject payload = JObject.FromObject(c);
                payload.Remove("id");
                payload.Remove("createdAt");
                var response = await CallAPI($@"api/Comments", HTTPMETHOD.POST, payload);
                var comment = JsonConvert.DeserializeObject<Comment>(response);
                return comment;
                
            } catch(Exception e)
            {
                await HelpersClass.ShowDialogAsync(e.Message);
            }
            return null;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByFeedback(Feedback feedback)
        {
            IEnumerable<Comment> results = new ObservableCollection<Comment>();
            try
            {
                string response = await CallAPI($@"api/Comments/Feedback/{feedback.Id}", HTTPMETHOD.GET, null);
                results = JsonConvert.DeserializeObject<IEnumerable<Comment>>(response);
            }
            catch (Exception e)
            {
              
            }
            return results;
        }

        public async Task<ChannelStatistic> GetStatistic(int id)
        {
            try
            {
                var response = await CallAPI($@"api/Channels/Statistic/{id}", HTTPMETHOD.GET, null);

                var statistic = JsonConvert.DeserializeObject<ChannelStatistic>(response);

                if(statistic!=null)
                {
                    return statistic;
                }
            }
            catch(Exception e)
            {

            }
            return new ChannelStatistic();
        }

        public async Task<ChannelStatistic> GetUserStatistic(string id, string userID)
        {
            try
            {
                return await App.MobileService
                   .InvokeApiAsync<ChannelStatistic>("Admin", HttpMethod.Get, new Dictionary<string, string>() { { "channelForStatID", id }, { "userID",userID } });
            }
            catch (Exception e)
            {

            }
            return new ChannelStatistic();
        }

        //Return true if casting a vote is successful and not deactivated
        public async Task<bool> VoteAsync(string userID, int voteStatus, Feedback feedback)
        {
            JObject payload = new JObject();
            payload.Add("userId", userID);
            payload.Add("feedbackId", feedback.Id);
            payload.Add("channelId", feedback.ChannelID);
            payload.Add("voteStatus", voteStatus);
            var response = await CallAPI(@"api/Votes", HTTPMETHOD.POST, payload);
            Voting vote = JsonConvert.DeserializeObject<Voting>(response);
            if(vote!= null)
            {
                if (vote.VoteStatus == -1)
                    return false;
                if (voteStatus == vote.VoteStatus)
                    return true;
            }
            //else
            //{
            //    await VotingTable.InsertAsync(new Voting { UserID = userID, VoteStatus = voteStatus, FeedbackID = feedback.Id });
            //}
            return false;
        }

        public async Task<Feedback> RefreshVotes(Feedback fb)
        {
            var response = await CallAPI($@"/api/Feedbacks/{fb.Id}/{MainPage.instance.user.UserId}", HTTPMETHOD.GET, null);
            var feedback = JsonConvert.DeserializeObject<Feedback>(response);
            if(feedback==null)
            {
                throw new Exception("Problem while getting feedback");
            }
            return feedback;
        }

        public async Task<List<Feedback>> GetFeedbackFromUser(string userID, string sort)
        {
            try
            {
                //var l = await App.MobileService.InvokeApiAsync<List<Feedback>>("Admin", HttpMethod.Get, new Dictionary<string, string>() { { "userID", userID }, { "sort", sort } });
                //return l;
            }
            catch (Exception e)
            {
                return null;
            }
            return null;
        }

        public async Task<bool> DeleteChannel(Channel c)
        {
            //Get the feedback from channel c
            try
            {
                var response = await CallAPI($@"api/Channels/{c.Id}", HTTPMETHOD.DELETE, null);
                if(response == null)
                {
                    throw new ArgumentNullException();
                }
                return true;
            }
            catch (Exception e)
            {
                await HelpersClass.ShowDialogAsync("Something went wrong: " + e.Message);
                return false;
            }
        }
        // Define a method that performs the authentication process
        // using a Facebook sign-in. 
        public MobileServiceUser AuthenticateAsync()
        {
            string message;
            MobileServiceUser user = null;
        // This sample uses the Facebook provider.
            var providers = new List<MobileServiceAuthenticationProvider>()
                { MobileServiceAuthenticationProvider.Facebook, MobileServiceAuthenticationProvider.Google };

            // Use the PasswordVault to securely store and access credentials.
            PasswordVault vault = new PasswordVault();
         
            PasswordCredential credential = null;
            foreach (MobileServiceAuthenticationProvider p in providers)
            {
                try
                {
                    // Try to get an existing credential from the vault.
                    credential = vault.FindAllByResource(p.ToString()).FirstOrDefault();

                    //vault.Remove(credential);
                }
                catch (Exception)
                {
                    // When there is no matching resource an error occurs, which we ignore.
                }
              
            }
           

            if (credential != null)
            {
                // Create a user from the stored credentials.
                user = new MobileServiceUser(credential.UserName);
                credential.RetrievePassword();
                user.MobileServiceAuthenticationToken = credential.Password;

                // Set the user from the stored credentials.
                App.MobileService.CurrentUser = user;
               

                // Consider adding a check to determine if the token is 
                // expired, as shown in this post: http://aka.ms/jww5vp.

                message = string.Format("Cached credentials for user - {0}", user.UserId);
            }

            return user;
        }

        public string GetCurrentToken()
        {
            // This sample uses the Facebook provider.
            var providers = new List<MobileServiceAuthenticationProvider>()
                { MobileServiceAuthenticationProvider.Facebook, MobileServiceAuthenticationProvider.Google };

            // Use the PasswordVault to securely store and access credentials.
            PasswordVault vault = new PasswordVault();

            PasswordCredential credential = null;
            foreach (MobileServiceAuthenticationProvider p in providers)
            {
                try
                {
                    // Try to get an existing credential from the vault.
                    credential = vault.FindAllByResource(p.ToString()).FirstOrDefault();
                }
                catch (Exception)
                {
                    // When there is no matching resource an error occurs, which we ignore.
                }
            }

            credential.RetrievePassword();
            return credential.Password != "" ? credential.Password : null;
        }

        public async Task<Channel> GetChannelById(int id)
        {
            try
            {
                var response = await CallAPI($@"api/Channels/{id}", HTTPMETHOD.GET, null);
                var channel = JsonConvert.DeserializeObject<Channel>(response);
                if (channel == null)
                    throw new ArgumentNullException();
                return channel;
            }catch
            {
                await HelpersClass.ShowDialogAsync("Something might went wrong with channel duplication");
                return new Channel();
            }
        }

        public async Task<Channel> SearchChannelByCode(string AccessCode)
        {
            var list = await ChannelTable.Where(c => c.AccessCode == AccessCode).ToListAsync();
            if (list.Count == 1)
            {
                return list[0];
            }
            else
            {
                await HelpersClass.ShowDialogAsync("Something might went wrong with channel duplication");
                return new Channel();
            }
        }

        public async Task<List<Channel>> GetChannelFilter(string userID)
        {
            try
            {
                var l = await App.MobileService.InvokeApiAsync<List<Channel>>("Admin", HttpMethod.Get, new Dictionary<string, string>() { { "FeedbackUserID", userID }});
                return l;
            }catch(Exception e)
            {
                await HelpersClass.ShowDialogAsync("Có chiện gì xảy ra rồi: " + e.Message);
                return null;
            }
        }

        public void GetInfo(MobileServiceUser user)
        {
            //// MobileServiceAuthenticationToken <- your token
            //var facebook = new FacebookGraphAPI(user.MobileServiceAuthenticationToken);
            //try
            //{
            //    // Get user profile data 
            //    var u = facebook.GetObject("me", null);
            //}catch(Exception e)
            //{

            //}

        }

        internal void AddSubChannelAsync(string id, string text)
        {
            throw new NotImplementedException();
        }

        //public async Task<MobileServiceUser> CustomLogin(string username, string password)
        //{
        //    string message;
        //    MobileServiceUser user = null;

        //    // Use the PasswordVault to securely store and access credentials.
        //    PasswordVault vault = new PasswordVault();
        //    PasswordCredential credential = null;
        //    try
        //    {
        //        // get the token
        //        //var token = await GetAuthenticationToken(username, password);

        //        //// authenticate: create and use a mobile service user
        //        //user = new MobileServiceUser(token.Guid);
        //        //user.MobileServiceAuthenticationToken = token.Access_Token;

        //        //// Create and store the user credentials.
        //        //credential = new PasswordCredential("custom",
        //        //    user.UserId, user.MobileServiceAuthenticationToken);
        //        //vault.Add(credential); 
        //        //App.MobileService.CurrentUser = user;
        //    }
        //    catch (MobileServiceInvalidOperationException)
        //    {
        //        message = "You must log in. Login Required";
        //        await HelpersClass.ShowDialogAsync(message);
        //    }
        //    return user;
        //}



        //public async Task<AuthenticationToken>
        //GetAuthenticationToken(string username, string email, string password)
        //{
        //    try
        //    {
        //        //using (var pc = new PrincipalContext(ContextType.Domain))
        //        //{
        //        //    using (var up = new UserPrincipal(pc))
        //        //    {
        //        //        up.SamAccountName = username;
        //        //        up.EmailAddress = email;
        //        //        up.SetPassword(password);
        //        //        up.Enabled = true;
        //        //        up.ExpirePasswordNow();
        //        //        up.Save();
        //        //    }
        //        //}
        //    }
        //    catch (MobileServiceInvalidOperationException exception)
        //    {
        //        //if (string.Equals(exception.Message, "invalid_grant"))
        //        //    throw new InvalidGrantException("Wrong credentails",
        //        //                                    exception);
        //        //else
        //        //    throw;
        //    }
        //    return null;
        //}

        public async Task<MobileServiceUser> Login(MobileServiceAuthenticationProvider provider)
        {
            string message;
            MobileServiceUser user = null;
            // This sample uses the Facebook provider.
            // Use the PasswordVault to securely store and access credentials.
            PasswordVault vault = new PasswordVault();
            PasswordCredential credential = null;
            try
            {
                // Login with the identity provider.
                user = await App.MobileService
                    .LoginAsync(provider);


                // Create and store the user credentials.
                credential = new PasswordCredential(provider.ToString(),
                    user.UserId, user.MobileServiceAuthenticationToken);
                JObject obj = new JObject();
                obj.Add("Id", user.UserId);
                obj.Add("Name", credential.UserName);
                vault.Add(credential);

                string response = await CallAPI(@"/api/Users", HTTPMETHOD.POST, obj);
                message = string.Format("You are now logged in - {0}", user.UserId);
            }
            catch (MobileServiceInvalidOperationException)
            {
                message = "You must log in. Login Required";
                await HelpersClass.ShowDialogAsync(message);
            }
            catch 
            {

            }
            return user;
        }

        public async Task<string> CallAPI(string endpoint, HTTPMETHOD method, JObject payload = null)
        {
            string token = GetCurrentToken();
            if (token == null)
                throw new Exception("Cannot retrieve current token");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(@"http://localhost:55985");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", token);
                try
                {
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = null;
                    switch (method)
                    {
                        case HTTPMETHOD.GET:
                            response = await client.GetAsync(endpoint);
                            break;
                        case HTTPMETHOD.POST:
                            response = await client.PostAsync(endpoint, content);
                            break;
                        case HTTPMETHOD.PUT:
                            response = await client.PutAsync(endpoint, content);
                            break;
                        case HTTPMETHOD.DELETE:
                            response = await client.DeleteAsync(endpoint);
                            break;
                    }
                    

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        return jsonResponse;
                    }
                    else
                    {
                        throw new ArgumentNullException(); 
                    }
                }
                catch (Exception)
                {
                    //Could not connect to server
                    //Use more specific exception handling, this is just an example
                    return null;
                }
            }

            return null;
        }

        public async Task<bool> LogoutAsync()
        {
            // This sample uses the Facebook provider.
            var providers = new List<MobileServiceAuthenticationProvider>()
                { MobileServiceAuthenticationProvider.Facebook, MobileServiceAuthenticationProvider.Google };


            // Use the PasswordVault to securely store and access credentials.
            PasswordVault vault = new PasswordVault();
            PasswordCredential credential = null;
            foreach (MobileServiceAuthenticationProvider p in providers)
            {
                try
                {
                    // Try to get an existing credential from the vault.
                    credential = vault.FindAllByResource(p.ToString()).FirstOrDefault();
                    vault.Remove(credential);
                    await App.MobileService.LogoutAsync();
                    return true;
                }
                catch (Exception)
                {
                    // When there is no matching resource an error occurs, which we ignore.
                }
            }
           
            return false;
        }

        internal async Task<bool> DeleteFeedbackAsync(Feedback fb)
        {
            try
            {
                var response = await CallAPI($@"api/Feedbacks/{fb.Id}", HTTPMETHOD.DELETE, null);
                if (response != null)
                    return true;
            }
            catch
            {

            }
            return false;
        }

        internal async Task<bool> UpdateChannel(Channel c)
        {
            try
            {
                var payload = JObject.FromObject(c);

                var result = await CallAPI($@"api/Channels/{c.Id}", HTTPMETHOD.PUT, payload);
                if (result == null)
                {
                    throw new ArgumentNullException();
                }
                return true;
            }
            catch
            {
                // do sumthing
            }
            return false;
        }

        internal async Task ReportSpamForFeedback(string  fbID, string userID)
        {
            try
            {
                var result = await App.MobileService
                .InvokeApiAsync<bool>("Admin", HttpMethod.Post, new Dictionary<string, string>() { { "SpamfeedbackID", fbID }, { "userReportID", userID } });
            }catch(Exception e)
            {
                await HelpersClass.ShowDialogAsync("Internet và app don't talk anymore: "+ e.Message);
            }
            
        }
        internal async Task<bool> ReportSpamForComment(string cmtID, string userID)
        {
            try
            {
                var result = await App.MobileService
                .InvokeApiAsync<bool>("Admin", HttpMethod.Post, new Dictionary<string, string>() { { "SpamCommentID", cmtID }, { "userReportID", userID } });

                return result;
            }
            catch (Exception e)
            {
                await HelpersClass.ShowDialogAsync("Internet và app don't talk anymore: " + e.Message);
            }
            return false;
        }

        internal async Task<bool> AddImageToFeedback(string fileDecoded, string feedbackID, string userID)
        {
            var result = await App.MobileService
                .InvokeApiAsync<Feedback>("Admin", HttpMethod.Post, new Dictionary<string, string>() { { "image", fileDecoded }, { "feedbackID", feedbackID }, { "userID", userID} } );
            if (result != null)
            {
                return true;
            }
            else return false;
        }
    } 
}
