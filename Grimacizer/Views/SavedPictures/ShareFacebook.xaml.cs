using Facebook;
using Grimacizer7.Common;
using Grimacizer7.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Grimacizer7.Views.SavedPictures
{
    public partial class ShareFacebook : NotifyPhoneApplicationPage
    {
        private FacebookClient client;
        private string _pictureName;
        private int _pictureWidth;
        private int _pictureHeight;
        private WriteableBitmap _image;
        public WriteableBitmap Image
        {
            get
            {
                return _image;
            }
            set
            {
                if (_image != value)
                {
                    _image = value;
                    NotifyPropertyChanged(() => Image);
                }
            }
        }
        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    NotifyPropertyChanged(() => Message);
                }
            }
        }
        private bool _isCalculating;
        public bool IsCalculating
        {
            get
            {
                return _isCalculating;
            }
            set
            {
                if (_isCalculating != value)
                {
                    _isCalculating = value;
                    NotifyPropertyChanged(() => IsCalculating);
                }
            }
        }
        private Visibility _broserVisibility;
        public Visibility BrowserVisibility
        {
            get
            {
                return _broserVisibility;
            }
            set
            {
                if (_broserVisibility != value)
                {
                    _broserVisibility = value;
                    NotifyPropertyChanged(() => BrowserVisibility);
                }
            }
        }

        public ShareFacebook()
        {
            InitializeComponent();
            DataContext = this;
            Message = string.Empty;
            IsCalculating = false;
            BrowserVisibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;

            _pictureName = HttpUtility.UrlDecode(NavigationContext.QueryString["image"]);
            _pictureWidth = int.Parse(NavigationContext.QueryString["width"]);
            _pictureHeight = int.Parse(NavigationContext.QueryString["height"]);
            Image = LocalImagesHelper.ReadImageFromIsolatedStorage(_pictureName, _pictureWidth, _pictureHeight);

            client = new FacebookClient();
            client.PostCompleted += Facebook_PostComplete;

            //Checking for saved token
            if (FacebookClientHelpers.GetToken() != null)
                client.AccessToken = FacebookClientHelpers.GetToken();
        }

        private void Facebook_PostComplete(object sender, FacebookApiEventArgs args)
        {
            //Checking for errors
            if (args.Error != null)
            {
                //Authorization error
                if (args.Error is FacebookOAuthException)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show("Authorization Error"));
                    //Remove the actual token since it doesn't work anymore.
                    FacebookClientHelpers.SaveToken(null);
                    client.AccessToken = null;
                }
                else
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(args.Error.Message));
                }
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Message posted successfully");
                    NavigationService.GoBack();
                });
            }

            Dispatcher.BeginInvoke(() => IsCalculating = false);
        }

        private void Post_Click(object sender, RoutedEventArgs e)
        {
            IsCalculating = true;
            var parameters = new Dictionary<string, object>
            {
                { "client_id", Constants.FacebookApiKey },
                { "redirect_uri", "https://www.facebook.com/connect/login_success.html" },
                { "response_type", "token" },
                { "display", "touch" },
                { "scope", "publish_stream" },
            };
            Browser.Navigate(client.GetLoginUrl(parameters));
        }

        private void BrowserNavigated(object sender, NavigationEventArgs e)
        {
            FacebookOAuthResult oauthResult;
            if (!client.TryParseOAuthCallbackUrl(e.Uri, out oauthResult))
            {
                BrowserVisibility = Visibility.Visible;
                IsCalculating = false;
                return;
            }

            IsCalculating = true;
            if (oauthResult.IsSuccess)
            {
                client.AccessToken = oauthResult.AccessToken;
                FacebookClientHelpers.SaveToken(client.AccessToken);
                BrowserVisibility = Visibility.Collapsed;
                PostToWall();
            }
            else
            {
                MessageBox.Show(oauthResult.ErrorDescription);
                BrowserVisibility = Visibility.Collapsed;
            }
        }

        private void PostToWall()
        {
            IsCalculating = true;
            var parameters = new Dictionary<string, object>
            {
                { "message", Message }
            };

            using (var memoryStream = new MemoryStream())
            {
                Image.SaveJpeg(memoryStream, Image.PixelWidth, Image.PixelHeight, 0, 100);
                memoryStream.Seek(0, 0);
                var data = new byte[memoryStream.Length];
                memoryStream.Read(data, 0, data.Length);
                memoryStream.Close();

                var fbUpl = new FacebookMediaObject
                {
                    ContentType = "image/jpeg",
                    FileName = "image.jpeg",
                };
                fbUpl.SetValue(data);
                parameters.Add("file", fbUpl);
            }           

            client.PostAsync("me/photos", parameters);
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (BrowserVisibility == Visibility.Visible)
            {
                e.Cancel = true;
                BrowserVisibility = Visibility.Collapsed;
            }
        }
    }
}