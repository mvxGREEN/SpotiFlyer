using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views.InputMethods;
using AndroidHUD;
using AngleSharp;
using MPowerKit.ProgressRing;
using System.Text.RegularExpressions;
using UraniumUI.Material.Controls;

namespace SpotiFlyerMaui
{
    public partial class MainPage : ContentPage
    {
        private static readonly string Tag = nameof(MainPage);

        public IServiceDownload Services;

        public static readonly Regex SECONDARY_MP3_FILENAME_REGEX = new Regex(@"^\d{4}");

        private readonly uint ANIM_LENGTH = 333;
        public readonly string[] CharsToRemove = ["\\", ":", "*", "?", "<", ">", "|", "("];

        string currentText = "";

        string mTitle = "";
        public string MTitle
        {
            get { return mTitle; }
            set
            {
                if (value == mTitle)
                {
                    return;
                }

                mTitle = value;
                OnPropertyChanged("MTitle");
            }
        }

        string mArtist = "";
        public string MArtist
        {
            get { return mArtist; }
            set
            {
                if (value == mArtist)
                {
                    return;
                }

                mArtist = value;
                OnPropertyChanged("MArtist");
            }
        }

        int mLength = 0;
        public int MLength
        {
            get { return mLength; }
            set
            {
                if (value == mLength)
                {
                    return;
                }

                mLength = value;
                OnPropertyChanged("MLength");
            }
        }

        string mThumbnailUrl = "";
        public string MThumbnailUrl
        {
            get { return mThumbnailUrl; }
            set
            {
                if (value == mThumbnailUrl)
                {
                    return;
                }

                mThumbnailUrl = value;
                OnPropertyChanged("MThumbnailUrl");
            }
        }

        string mMessageProgress = "";
        public string MMessageProgress
        {
            get { return mMessageProgress; }
            set
            {
                Log.Debug(Tag, $"Setting mMessageProgress={value}");
                if (value == mMessageProgress)
                {
                    return;
                }

                mMessageProgress = value;
                OnPropertyChanged("MMessageProgress");
            }
        }

        string mMessageToast = "";
        public string MMessageToast
        {
            get { return mMessageToast; }
            set
            {
                Log.Debug(Tag, $"Setting mMessageToast={value}");
                if (value == mMessageToast)
                {
                    return;
                }

                mMessageToast = value;
                OnPropertyChanged("MMessageToast");
            }
        }

        string mFragmentTitle = "";
        public string MFragmentTitle
        {
            get { return mFragmentTitle; }
            set
            {
                if (value == mFragmentTitle)
                {
                    return;
                }

                mFragmentTitle = value;
                OnPropertyChanged("MFragmentTitle");
            }
        }

        string mFragmentSubtitle = "";
        public string MFragmentSubtitle
        {
            get { return mFragmentSubtitle; }
            set
            {
                if (value == mFragmentSubtitle)
                {
                    return;
                }

                mFragmentSubtitle = value;
                OnPropertyChanged("MFragmentSubtitle");
            }
        }

        string mFragmentBody = "";
        public string MFragmentBody
        {
            get { return mFragmentBody; }
            set
            {
                if (value == mFragmentBody)
                {
                    return;
                }

                mFragmentBody = value;
                OnPropertyChanged("MFragmentBody");
            }
        }

        string mFragmentPositive = "";
        public string MFragmentPositive
        {
            get { return mFragmentPositive; }
            set
            {
                if (value == mFragmentPositive)
                {
                    return;
                }

                mFragmentPositive = value;
                OnPropertyChanged("MFragmentPositive");
            }
        }

        string mFragmentDismiss = "";
        public string MFragmentDismiss
        {
            get { return mFragmentDismiss; }
            set
            {
                if (value == mFragmentDismiss)
                {
                    return;
                }

                mFragmentDismiss = value;
                OnPropertyChanged("MFragmentDismiss");
            }
        }

        string mEnableBkgTitle = "Enable Background:";
        public string MEnableBkgTitle
        {
            get { return mEnableBkgTitle; }
            set
            {
                if (value == mEnableBkgTitle)
                {
                    return;
                }

                mEnableBkgTitle = value;
                OnPropertyChanged("MEnableBkgTitle");
            }
        }

        string mEnableBkgSubtitle = "Download in background";
        public string MEnableBkgSubtitle
        {
            get { return mEnableBkgSubtitle; }
            set
            {
                if (value == mEnableBkgSubtitle)
                {
                    return;
                }

                mEnableBkgSubtitle = value;
                OnPropertyChanged("MEnableBkgSubtitle");
            }
        }

        bool mIsNotBkgEnabled = true;
        public bool MIsNotBkgEnabled
        {
            get { return mIsNotBkgEnabled; }
            set
            {
                if (value == mIsNotBkgEnabled)
                {
                    return;
                }

                mIsNotBkgEnabled = value;

                Console.WriteLine($"{Tag} mIsNotBkgEnabled={!mIsNotBkgEnabled}");
                Preferences.Default.Set("IS_BKG_ENABLED", !mIsNotBkgEnabled);
                OnPropertyChanged("MIsNotBkgEnabled");
                UpdateEnableBkgHolder();
            }
        }

        // MAIN PAGE
        public MainPage(IServiceDownload s)
        {
            InitializeComponent();
            BindingContext = this;
            Services = s;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // prepare destination file dirs
            Spotiflyer.PrepareFileDirs();

            // check bkg enabled
            MIsNotBkgEnabled = !Preferences.Default.Get("IS_BKG_ENABLED", false);
            Console.WriteLine($"{Tag}, IS_BKG_ENABLED={!MIsNotBkgEnabled}");

            MainActivity.ActivityCurrent.CheckForIntent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            // Use Dispatcher to run async code on the main thread for UI interactions
            Dispatcher.Dispatch(async () =>
            {
                HidePopup();
            });

            // Return true to prevent the default back button action immediately
            return true;
        }

        // ON CLICKED
        private void OnAboutClicked(object sender, EventArgs e)
        {
            var aboutUrl = "https://mobileapps.green/";
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(aboutUrl));
            MainActivity.ActivityCurrent.StartActivity(intent);
        }

        private void OnPrivacyPolicyClicked(object sender, EventArgs e)
        {
            var privacyUrl = "https://mobileapps.green/privacy-policy";
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(privacyUrl));
            MainActivity.ActivityCurrent.StartActivity(intent);
        }

        private void OnHelpClicked(object sender, EventArgs e)
        {
            Console.WriteLine($"{Tag} OnHelpClicked");
            ShowPopup("Help");
        }

        private void OnCloseClicked(object sender, EventArgs e)
        {
            HidePopup();
        }

        private void OnEnableBkgClicked(object sender, EventArgs e)
        {
            Console.WriteLine($"{Tag} OnEnableBkgClicked");
            
            // set var
            MIsNotBkgEnabled = false;

            // update ui
            UpdateEnableBkgHolder();

            // request permission to ignore battery optimization
            Intent intent = new Intent(Android.Provider.Settings.ActionSettings);
            intent.SetAction(Android.Provider.Settings.ActionRequestIgnoreBatteryOptimizations);
            intent.SetData(Android.Net.Uri.Parse("package:" + Platform.CurrentActivity.PackageName));
            MainActivity.ActivityCurrent.StartActivity(intent);
        }

        private void OnPasteClicked(object sender, EventArgs e)
        {
            Console.WriteLine($"{Tag} OnPasteClicked");
            ResetVars();
            ClearTextfield();

            string clip = Clipboard.GetTextAsync().Result;
            Log.Info(Tag, "clipboard text: " + clip);

            TextField mTextField = (TextField)FindByName("main_textfield");
            mTextField.Text = clip;
        }

        private void OnDownloadClicked(object sender, EventArgs e)
        {
            Console.WriteLine($"{Tag} OnDownloadClicked");
            
            int counter = 0;
            Spotiflyer.MInputs.ForEach(input =>
            {
                Console.WriteLine($"{Tag} MInputs[{counter++}]={input}");
            });

            // register finish reciever
            if ((int)Build.VERSION.SdkInt >= 33)
            {
                Platform.AppContext.RegisterReceiver(MainActivity.MFinishReceiver, new IntentFilter("69"), ReceiverFlags.Exported);
            }
            else
            {
                Platform.AppContext.RegisterReceiver(MainActivity.MFinishReceiver, new IntentFilter("69"));
            }

            ShowDownloadingUI();

            // start download tasks from service
            Services.Start();
        }

        private void OnShareClicked(object sender, EventArgs e)
        {
            Console.WriteLine($"{Tag} OnShareClicked");

            // TODO implement file sharing
        }

        // INPUT
        private readonly string INPUT_REGEX = "^(https?:\\/\\/)?(www\\.)?open\\.spotify\\.com(\\/intl-[a-z]{2})?\\/(track|album|playlist|show|episode|audiobook)\\/([a-zA-Z0-9]+)(\\?.*)?$";
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Console.WriteLine($"{Tag} OnTextChanged");

            string oldText = e.OldTextValue;
            string newText = e.NewTextValue;
            string input = ((TextField)sender).Text;

            if (input != null)
            {
                int lengthDiff;
                if (oldText == null)
                {
                    lengthDiff = newText.Length;
                }
                else
                {
                    lengthDiff = newText.Length - oldText.Length;
                }

                Log.Info(Tag, "length difference: " + lengthDiff);

                if (input.Length == 0)
                {
                    Log.Info(Tag, "text field text cleared!");
                    currentText = "";
                    ShowEmptyUI();
                }
                else if (lengthDiff > 1 || lengthDiff == 0)
                {
                    Log.Info(Tag, "text pasted");
                    if (input != null && !Spotiflyer.MIsShared)
                    {
                        currentText = input;
                        HandleInput(input);
                    }
                }
                else if (lengthDiff == 1)
                {
                    // character typed
                }
                else
                {
                    // character deleted
                }
            }
            else
            {
                Log.Warn(Tag, "input is null!");
                AndHUD.Shared.ShowToast(Platform.CurrentActivity, "Please copy a link", MaskType.None, TimeSpan.FromMilliseconds(3000), false);
            }
        }

        private void OnTextCompleted(object sender, EventArgs e)
        {
            Log.Info(Tag, "OnTextCompleted");
            string input = ((TextField)FindByName("main_textfield")).Text.ToString();
            HandleInput(input);
        }

        public void HandleInput(String input)
        {
            Log.Info(Tag, "HandleInput input=" + input);

            // check for internet connection
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;
            if (accessType != NetworkAccess.Internet)
            {
                MMessageToast = "Not connected to the internet.";
                Log.Warn(Tag, MMessageToast);
                AndHUD.Shared.ShowError(Platform.CurrentActivity, MMessageToast, MaskType.Black, TimeSpan.FromSeconds(2));

                return;
            }

            // validate and log input
            var match = Regex.Match(input, INPUT_REGEX, RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                // log invalid input
                Console.WriteLine($"{Tag} input invalid");
                // TODO show toast 
            } else
            {
                // check if album
                if (input.Contains("/album"))
                {
                    Spotiflyer.MIsAlbum = true;
                }

                // update ui
                MMessageProgress = "Loading…";
                ShowLoadingUI();

                // load url
                string url = input.Substring(input.IndexOf("https://"));
                if (url.Contains("spotify.com/playlist") || url.Contains("spotify.com/album"))
                {
                    Console.WriteLine("loading batch");
                    LoadBatch(url);
                }
                else
                {
                    Console.WriteLine("loading non-batch");
                    LoadTrack(url);
                }
            }
        }

        // LOAD / DOWNLOAD
        public void ResetVars()
        {
            Console.WriteLine($"{Tag} ResetVars");

            Spotiflyer.MIsAlbum = false;
            Spotiflyer.MInputs = [];
            Spotiflyer.MCountTracksFinal = 0;
            Spotiflyer.MCountTracks = 0;

            MThumbnailUrl = "";
            MMessageToast = "";

            MTitle = "";
            MArtist = "";
            MLength = 0;
            MMessageProgress = "";
            
            Spotiflyer.MFailedShowInter = false;
        }

        public async Task LoadTrack(string url)
        {
            Console.WriteLine($"LoadTrack url={url}");

            // open url as document
            var config = Configuration.Default.WithDefaultLoader();
            var address = url;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(address);

            // get html, title
            string title = document.Title;
            string html = document.Body.ToHtml();
            IEnumerable<string> htmlChunks = Split(html, 3500);
            document.Dispose();

            // log html
            Console.WriteLine($"page title={title}");
            Console.WriteLine($"page html.length={html.Length}");
            foreach (string v in htmlChunks)
            {
                Console.WriteLine(v);
            }

            // validate html
            if (html.Length < 3500)
            {
                Console.WriteLine("HTML too short!");
                MMessageToast = "Unexpected error!\nPlease try again later";
                AndHUD.Shared.ShowError(Platform.CurrentActivity, MMessageToast, MaskType.Black, TimeSpan.FromSeconds(3));
                ShowEmptyUI();
                return;
            }

            // extract track info
            string songTitle = "";
            string songArtist = "";
            //string songAlbum = "";
            string trimmed = "";

            if (title.Contains(" - "))
            {
                Console.WriteLine($"extracting from document title");

                songTitle = title.Substring(0, title.IndexOf(" - "));

                if (songTitle.Contains("feat."))
                {
                    // rm everything after a 'feat.'
                    songTitle = songTitle.Substring(0, songTitle.IndexOf("feat.") - 1);
                }

                // remove unsafe characters
                foreach (var c in CharsToRemove)
                {
                    songTitle = songTitle.Replace(c, string.Empty);
                }

                // log title
                MTitle = songTitle;
                Console.WriteLine($"found songTitle={songTitle}");

                if (title.Contains(" by ") && title.Contains(" | "))
                {
                    Console.WriteLine("extracting artist from document title");

                    // extract artist
                    int si = title.LastIndexOf(" by ") + 4;
                    songArtist = title.Substring(si,
                        title.LastIndexOf(" | ") - si);

                    // remove unsafe characters
                    foreach (var c in CharsToRemove)
                    {
                        songArtist = songArtist.Replace(c, string.Empty);
                    }

                    // log artist
                    MArtist = songArtist;
                    Console.WriteLine($"found songArtist={songArtist}");
                }
            }
            
            // extract author
            if (html.Contains("<h1"))
            {
                Console.WriteLine("extracting from <h1");

                // extract h1 element
                int startIndex = html.IndexOf("<h1");
                int length = html.IndexOf("</h1") - startIndex;
                string h1 = html.Substring(startIndex, length);
                Console.WriteLine($"H1: {h1}");

                // extract title
                songTitle = h1.Substring(h1.IndexOf('>') + 1);
                trimmed = songTitle;
                if (songTitle.Contains("feat."))
                {
                    // rm everything after a 'feat'
                    trimmed = songTitle.Substring(0, songTitle.IndexOf("feat."));
                }

                // remove unsafe characters
                foreach (var c in CharsToRemove)
                {
                    trimmed = trimmed.Replace(c, string.Empty);
                }

                // log title
                songTitle = trimmed;
                MTitle = songTitle;
                Console.WriteLine($"trimmed songTitle={songTitle}");

                // extract author/artist/narrator
                if (html.Contains("Author\">") || html.Contains("Audiobook • "))
                {
                    Console.WriteLine("extracting author");

                    // extract author
                    int s = 0;
                    if (html.Contains("Author\">"))
                    {
                        s = html.IndexOf("Author\">") + 8;
                    } else
                    {
                        s = html.IndexOf("Audiobook • ") + 12;
                    }
                    int e = html.IndexOf('<', s) - s;
                    string author = html.Substring(s, e);

                    // extract inner data
                    if (author.StartsWith('<'))
                    {
                        author = author[(author.IndexOf('>') + 1)..];
                    }

                    // remove unsafe characters
                    foreach (var c in CharsToRemove)
                    {
                        author = author.Replace(c, string.Empty);
                    }

                    // log author
                    songArtist = author;
                    MArtist = songArtist;
                    Console.WriteLine($"{Tag} extracted author={author} songArtist={songArtist}");
                }
                else if (html.Contains("href=\"/artist"))
                {
                    Console.WriteLine("extracting artist");

                    // extract artist
                    string a = html.Substring(html.IndexOf("href=\"/artist"));
                    int s = a.IndexOf('>') + 1;
                    int e = a.IndexOf('<') - s;
                    songArtist = a.Substring(s, e);
                    Console.WriteLine($"{Tag} songArtist={songArtist}");

                    // remove unsafe characters
                    trimmed = songArtist;
                    foreach (var c in CharsToRemove)
                    {
                        trimmed = songArtist.Replace(c, string.Empty);
                    }

                    // log artist
                    Log.Info(Tag, "found artist: " + trimmed);
                    songArtist = trimmed;
                    MArtist = songArtist;
                }

                /* TODO add narrator to songArtist
                 * 
                if (html.Contains("Narrated By "))
                {
                    Console.WriteLine("extracting narrator");

                    // extract narrator
                    int s = html.IndexOf("Narrated By ") + 12;
                    int e = html.IndexOf('<', s) - s;
                    string narrator = html.Substring(s, e);

                    // extract inner data
                    if (narrator.StartsWith('<'))
                    {
                        narrator = narrator[(narrator.IndexOf('>') + 1)..];
                    }

                    // remove unsafe characters
                    foreach (var c in CharsToRemove)
                    {
                        trimmed = narrator.Replace(c, string.Empty);
                    }

                    // log narrator
                    songArtist += "," + narrator;
                    MArtist = songArtist;
                    Console.WriteLine($"{Tag} extracted narrator={narrator}, songArtist={songArtist}");
                }
                */

                // extract thumbnail url
                ExtractThumbnail(html);

                // build input
                string inp = "title=" + songTitle + ",artist=" + songArtist;
                if (MLength > 0)
                {
                    inp = inp + ",length=" + MLength;
                }
                Spotiflyer.MInputs.Add(inp);
                Spotiflyer.MCountTracksFinal = Spotiflyer.MInputs.Count;

                // log input
                Console.WriteLine($"inp={inp}");
                Console.WriteLine($"MInputs.Count={Spotiflyer.MInputs.Count}");

                // update ui
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ShowPreviewUI();
                });
            }
        }

        public async Task LoadBatch(string batch_url)
        {
            Log.Info(Tag, "LoadBatch url= " + batch_url);

            // load html from url into document
            var config = Configuration.Default.WithDefaultLoader();
            var address = batch_url;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(address);

            // get document html
            string title = document.Title;
            string body = document.Body.ToHtml();
            string head = document.Head.ToHtml();
            IEnumerable<string> htmlChunks = Split(body, 3500);
            document.Dispose();

            Log.Info(Tag, title);
            Log.Info(Tag, "HEAD LENGTH: " + head.Length);
            Log.Info(Tag, "BODY LENGTH: " + body.Length);

            // print body
            foreach (string v in htmlChunks)
            {
                Log.Info(Tag, v);
            }

            // check if body is too small
            if (body.Length < 3500)
            {
                Log.Error(Tag, "error: html too short; invalid url");

                MMessageToast = "Unexpected error, please try again";
                AndHUD.Shared.ShowError(Platform.CurrentActivity, MMessageToast, MaskType.Black, TimeSpan.FromSeconds(3));

                return;
            }

            ExtractTitle(title);

            ExtractAlbumArtist(title, body);

            if (MArtist.Length == 0)
            {
                // TODO find track artist within playlist
            }

            ExtractThumbnail(body);

            // extract all batch inputs
            Spotiflyer.MInputs = await ExtractInputs(head, body);

            // display track count
            MArtist = Spotiflyer.MInputs.Count + " tracks";

            // update ui
            ShowPreviewUI();
        }

        private async Task<List<string>> ExtractInputs(string head, string body)
        {
            Console.WriteLine($"{Tag} ExtractInputs");
            List<string> urls = [];
            List<string> inputs = [];
            string TRACK_URL_BASE = "open.spotify.com/track/";
            while (head.Contains(TRACK_URL_BASE))
            {
                head = head.Substring(head.IndexOf(TRACK_URL_BASE) + TRACK_URL_BASE.Length);
                string id = head.Substring(0, head.IndexOf("\""));
                string url = "\"/track/" + id + "\"";
                Console.WriteLine($"{Tag} found track url #{urls.Count + 1}: {url}");
                urls.Add(url);
            }
            Console.WriteLine($"{Tag} found {urls.Count} track urls");

            // locate title of each track
            foreach (string u in urls)
            {
                // use track url to find title
                if (body.Contains(u))
                {
                    Console.WriteLine($"{Tag} found {u} in html body");

                    // extract track title
                    int s = body.IndexOf("><", body.IndexOf(u)) + 2;
                    int end = body.IndexOf("</", s);
                    while (body.IndexOf("><", s) > -1 && body.IndexOf("><", s) < end)
                    {
                        s = body.IndexOf("><", s) + 2;
                    }
                    s = body.IndexOf(">", s) + 1;

                    string title = body.Substring(s, end - s);


                    Console.WriteLine($"{Tag} found track title: title={title}");

                    if (title.Contains("feat."))
                    {
                        // rm everything after a 'feat.'
                        title = title.Substring(0, title.IndexOf("feat.") - 1);
                    }
                    // restore our old friend ampersand
                    if (title.Contains("&amp;"))
                    {
                        title.Replace("&amp;", "&");
                    }
                    // remove unsafe characters
                    foreach (var c in CharsToRemove)
                    {
                        title = title.Replace(c, string.Empty);
                    }

                    // Fetch the specific artist for this track, using the global MArtist as a fallback
                    string artist = ExtractTrackArtist(body, end, MArtist);
                    Console.WriteLine($"{Tag} final extracted track artist: artist={artist}");

                    // build input string
                    string input = $"title={title},artist={artist}";
                    Log.Info(Tag, "adding input string: " + input);
                    inputs.Add(input);
                }
                else
                {
                    Log.Error(Tag, "track title not found in body! url=" + u);
                }
            }

            Spotiflyer.MCountTracksFinal = inputs.Count;
            Spotiflyer.MCountTracks = 0;

            MTitle = $"{MTitle}";

            return inputs;
        }

        private void ExtractTitle(string t)
        {
            // extract title
            if (t.Contains(" - "))
            {
                string title = t.Substring(0, t.IndexOf(" - "));

                if (title.Contains("feat."))
                {
                    // rm everything after a 'feat.'
                    title = title.Substring(0, title.IndexOf("feat.") - 1);
                }

                // remove unsafe characters
                foreach (var c in CharsToRemove)
                {
                    title = title.Replace(c, string.Empty);
                }

                MTitle = title;
                Console.WriteLine($"{Tag} MTitle={MTitle}");
            } else if (t.Contains(" | "))
            {
                string title = t.Substring(0, t.IndexOf(" | "));

                // remove unsafe characters
                foreach (var c in CharsToRemove)
                {
                    title = title.Replace(c, string.Empty);
                }

                MTitle = title;
                Console.WriteLine($"{Tag} MTitle={MTitle}");
            } else if (t != null)
            {
                MTitle = t;
                Console.WriteLine($"{Tag} MTitle={MTitle}");
            }
        }

        private string ExtractTrackArtist(string body, int trackTitleEndIndex, string fallbackArtist)
        {
            string artist = fallbackArtist;
            try
            {
                // Limit search scope so we don't accidentally grab the artist of the NEXT track
                int nextTrackIndex = body.IndexOf("\"/track/", trackTitleEndIndex);
                if (nextTrackIndex == -1) nextTrackIndex = body.Length;

                // 1. Check for Spotify's explicit artist link
                int artistLinkIndex = body.IndexOf("href=\"/artist/", trackTitleEndIndex);

                if (artistLinkIndex > -1 && artistLinkIndex < nextTrackIndex)
                {
                    int start = body.IndexOf(">", artistLinkIndex) + 1;
                    int end = body.IndexOf("</", start);
                    artist = body.Substring(start, end - start);
                }
                // 2. Fallback to finding the first valid span (for some older UI variants)
                else
                {
                    int spanIndex = body.IndexOf("<span", trackTitleEndIndex);
                    if (spanIndex > -1 && spanIndex < nextTrackIndex)
                    {
                        int start = body.IndexOf(">", spanIndex) + 1;
                        int length = body.IndexOf("</", start) - start;
                        artist = body.Substring(start, length);

                        // Skip nested spans if we accidentally grabbed a wrapper
                        if (artist.Contains("<span"))
                        {
                            spanIndex = body.IndexOf("<span", spanIndex + length);
                            if (spanIndex > -1 && spanIndex < nextTrackIndex)
                            {
                                start = body.IndexOf(">", spanIndex) + 1;
                                length = body.IndexOf("</", start) - start;
                                artist = body.Substring(start, length);
                            }
                        }
                    }
                }

                // 3. Clean up HTML entities and illegal file characters
                artist = artist.Replace("&amp;", "&");
                foreach (var c in CharsToRemove)
                {
                    artist = artist.Replace(c, string.Empty);
                }

                // 4. Trim to primary artist if multiple are comma-separated
                if (artist.Contains(","))
                {
                    artist = artist.Substring(0, artist.IndexOf(","));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Tag} Error extracting track artist: {ex.Message}");
            }

            Console.WriteLine($"ExtractTrackArtist: artist={artist}");

            return string.IsNullOrWhiteSpace(artist) ? fallbackArtist : artist.Trim();
        }

        private void ExtractAlbumArtist(string title, string html)
        {
            Console.WriteLine("ExtractAlbumArtist");

            // Attempt to grab the global artist from the page title
            if (title.Contains(" by ") && title.Contains(" | "))
            {
                int si = title.LastIndexOf(" by ") + 4;
                string artist = title.Substring(si, title.LastIndexOf(" | ") - si);

                foreach (var c in CharsToRemove)
                {
                    artist = artist.Replace(c, string.Empty);
                }

                MArtist = artist.Trim();
                Console.WriteLine($"{Tag} Global Fallback MArtist={MArtist}");
            }
            else
            {
                MArtist = "Unknown Artist"; // Safe fallback if title doesn't contain it
                Console.WriteLine($"{Tag} No global artist found in title.");
            }
        }

        private void ExtractThumbnail(string html)
        {
            Console.WriteLine("ExtractThumbnail");

            // extract thumbnail
            string thtml = html;
            string thumbnailUrlBase = "i.scdn.co/image/";
            //string thumbnailUrlArg = "srcset=\"";
            string https = "https://";

            if (thtml.Contains(thumbnailUrlBase))
            {
                thtml = thtml.Substring(thtml.IndexOf(thumbnailUrlBase));
                MThumbnailUrl = https + thtml.Substring(0, thtml.IndexOf("\""));

                Log.Info(Tag, $"MThumbnailUrl: {MThumbnailUrl}");
            }
            else
            {
                Log.Error(Tag, "thumbnail not found!");
            }
        }

        static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        // DIALOG FRAGMENT
        public async void ShowPopup(string title)
        {
            Console.WriteLine($"{Tag}: ShowPopup({title})");

            // fill views
            MFragmentTitle = title;
            if (title == "Upgrade")
            {
                MFragmentSubtitle = "SpotiFlyer Gold";
                MFragmentBody = "✅  Playlists\n✅  Albums\n✅  Fastest Speed\n✅  Ad-Free\n";
                MFragmentPositive = "Get It!";
                MFragmentDismiss = "Close";
                ((Image)FindByName("launcher_image")).Source = ImageSource.FromFile("ic_launcher_spotloader_round.png");
                //((Button)FindByName("yearly_button")).IsVisible = true;
            }
            else if (title == "Help")
            {
                MFragmentSubtitle = "How to Use SpotiFlyer:";
                MFragmentBody = "➊  Copy URL\n  ⓘ  Click \"Share\" then \"Copy link\"\n➋  Paste URL (Tap ⚡)\n➌  Download (Tap ⬇)\n    ⓘ  File saved in Documents";
                MFragmentDismiss = "Close";
                ((Button)FindByName("dismiss_button")).IsVisible = true;
                ((Button)FindByName("positive_button")).IsVisible = false;
                ((Image)FindByName("launcher_image")).Source = ImageSource.FromFile("ic_launcher_spotloader_round.png");
            }
            else if (title == "Rate")
            {
                MFragmentSubtitle = "";
                MFragmentBody = "Should I maintain SpotiFlyer?\nLet me know <3";
                MFragmentPositive = "Rate";
                MFragmentDismiss = "Close";
                ((Button)FindByName("dismiss_button")).IsVisible = true;
                ((Button)FindByName("positive_button")).IsVisible = true;
                ((Image)FindByName("launcher_image")).Source = ImageSource.FromFile("ic_launcher_spotloader_round.png");
            }
            else if (title == "SoundLoader")
            {
                MFragmentSubtitle = "Downloader for Soundcloud";
                MFragmentBody = "You might like this app too\n\n✦Ad by Green Mobile✦";
                MFragmentPositive = "Free";
                MFragmentDismiss = "Close";
            }
            else if (title == "InSave")
            {
                MFragmentSubtitle = "Downloader for Instagram";
                MFragmentBody = "You might like this too\n\n✦Ad by Green Mobile✦";
                MFragmentPositive = "Free";
                MFragmentDismiss = "Nah";
            }
            else if (title == "SaveFrom")
            {
                MFragmentSubtitle = "Downloader for Videos";
                MFragmentBody = "You might like this too\n\n✦Ad by Green Mobile✦";
                MFragmentPositive = "Free";
                MFragmentDismiss = "Nah";
            }
            else if (title == "VscoLoader")
            {
                MFragmentSubtitle = "Downloader for VSCO";
                MFragmentBody = "You might like this too\n\n✦Ad by Green Mobile✦";
                MFragmentPositive = "Free";
                MFragmentDismiss = "Nah";
            }
            else if (title == "Musix")
            {
                MFragmentSubtitle = "Audio Player";
                MFragmentBody = "Play your music.\nAd-free.\n✦Ad by Green Mobile✦";
                MFragmentPositive = "Get It!";
                MFragmentDismiss = "Close";
                ((Button)FindByName("dismiss_button")).IsVisible = true;
                ((Button)FindByName("positive_button")).IsVisible = true;
                ((Image)FindByName("launcher_image")).Source = ImageSource.FromFile("ic_launcher_musix_round.png");
            }

            // fade in
            AbsoluteLayout fragment = (AbsoluteLayout)FindByName("fragment_layout");
            fragment.Opacity = 0.0;
            fragment.IsVisible = true;
            await fragment.FadeTo(1.0, ANIM_LENGTH);
        }

        public async void HidePopup()
        {
            Console.WriteLine($"{Tag} HidePopup");

            // fade out
            AbsoluteLayout fragment = (AbsoluteLayout)FindByName("fragment_layout");
            fragment.Opacity = 1.0;
            await fragment.FadeTo(0.0, ANIM_LENGTH);
            fragment.IsVisible = false;

            // hide yearly button
            //((Button)FindByName("yearly_button")).IsVisible = false;

            // clear text
            MFragmentTitle = "";
            MFragmentSubtitle = "";
            MFragmentBody = "";
            MFragmentPositive = "";
            MFragmentDismiss = "";
        }

        public void UpdateEnableBkgHolder()
        {
            Console.WriteLine($"{Tag} UpdateEnableBkgHolder() MBkgIsNotEnabled={MIsNotBkgEnabled}");

            // Logic: 
            // Show item if: User IS Gold (!MIsNotGold) AND Background IS NOT enabled (MIsNotBkgEnabled)
            // Hide item otherwise.

            if (MIsNotBkgEnabled)
            {
                Console.WriteLine($"{Tag} showing enable bkg item");
                // MAUI ToolbarItems don't have IsVisible, so we Add/Remove them from the list.
                if (!ToolbarItems.Contains(enable_bkg_item))
                {
                    ToolbarItems.Add(enable_bkg_item);
                }
            }
            else
            {
                Console.WriteLine($"{Tag} hiding enable bkg item");
                if (ToolbarItems.Contains(enable_bkg_item))
                {
                    ToolbarItems.Remove(enable_bkg_item);
                }
            }
        }

        // USER INTERFACE
        private void HideKeyboard()
        {
            // hide keyboard
            Context context = Platform.CurrentActivity;
            var inputMethodManager = context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            if (inputMethodManager != null && context is Android.App.Activity)
            {
                var activity = context as Android.App.Activity;
                var token = activity.CurrentFocus?.WindowToken;
                inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);

                activity.Window.DecorView.ClearFocus();
            }
        }

        public async Task ClearTextfield()
        {
            TextField mTextField = (TextField)FindByName("main_textfield");
            if (mTextField != null)
            {
                mTextField.Text = "";
            }
        }

        public async Task ShowEmptyUI()
        {
            Console.WriteLine($"{Tag}: ShowEmptyUI");

            ResetVars();
            HidePopup();

            // hide buttons
            ButtonView finishBtn = (ButtonView)FindByName("finish_btn");
            ButtonView dlBtn = (ButtonView)FindByName("dl_btn");
            dlBtn.Opacity = 0.0;
            finishBtn.Opacity = 0.0;
            finishBtn.IsVisible = false;
            ((Image)FindByName("preview_img")).Opacity = 0.0;
            ((ButtonView)FindByName("dl_btn")).Opacity = 0.0;
            ((ProgressRing)FindByName("progress_ring")).Opacity = 0.0;
            ((Label)FindByName("progress_label")).Opacity = 0.0;
            ((Frame)FindByName("downloader_frame")).Opacity = 0.0;
        }

        public async Task ShowLoadingUI()
        {
            HideKeyboard();

            // change progress message
            MMessageProgress = "Loading…";

            // show indeterminate progress ring
            ProgressRing pr = (ProgressRing)FindByName("progress_ring");
            pr.IsIndeterminate = true;
            pr.FadeTo(1.0, ANIM_LENGTH);
            await ((Label)FindByName("progress_label")).FadeTo(1.0, ANIM_LENGTH);
        }

        public void ShowPreviewUI()
        {
            Console.WriteLine($"{Tag}: ShowPreviewUI");

            // hide finish button
            ButtonView finishBtn = (ButtonView)FindByName("finish_btn");
            finishBtn.Opacity = 0.0;
            finishBtn.IsVisible = false;

            // show downloader
            ((Frame)FindByName("downloader_frame")).Opacity = 1.0;
            ButtonView dlBtn = (ButtonView)FindByName("dl_btn");
            dlBtn.Opacity = 1.0;
            dlBtn.IsVisible = true;

            // hide loading progress ring
            MMessageProgress = "";
            ((ProgressRing)FindByName("progress_ring")).Opacity = 0.0;
            ((Label)FindByName("progress_label")).Opacity = 0.0;

            // hide downloading progress ring
            ProgressRing prd = (ProgressRing)FindByName("progress_ring_dlr");
            prd.Opacity = 0.0;
            prd.IsVisible = false;

            // increase thumbnail opacity
            ((Image)FindByName("preview_img")).Opacity = 1.0;
        }

        public async Task ShowDownloadingUI()
        {
            // change progress message
            MMessageProgress = "Finding download…";

            ProgressRing pr = (ProgressRing)FindByName("progress_ring");
            ProgressRing prd = (ProgressRing)FindByName("progress_ring_dlr");
            ButtonView dlBtn = (ButtonView)FindByName("dl_btn");

            pr.IsIndeterminate = true;

            // hide preview UI
            pr.FadeTo(1.0, ANIM_LENGTH);
            pr.FadeTo(1.0, ANIM_LENGTH);
            ((Label)FindByName("progress_label")).FadeTo(1.0, ANIM_LENGTH);
            await dlBtn.FadeTo(0.0, ANIM_LENGTH);

            // show downloading UI
            ((Image)FindByName("preview_img")).FadeTo(0.45, ANIM_LENGTH);
            dlBtn.IsVisible = false;
            prd.IsVisible = true;
            prd.FadeTo(1.0, ANIM_LENGTH);
        }

        public async Task ShowFinishUI()
        {
            Console.WriteLine($"{Tag}: ShowFinishUI");

            // count successful runs
            int runs = 1;
            if (Preferences.Default.ContainsKey("SUCCESSFUL_RUNS"))
            {
                runs += Preferences.Default.Get("SUCCESSFUL_RUNS", 0);
            }
            Spotiflyer.successfulRuns = runs;
            Console.WriteLine($"{Tag} SUCCESSFUL_RUNS={runs}");
            Preferences.Default.Set("SUCCESSFUL_RUNS", runs);

            // show success message
            MMessageToast = $"Saved! In {Spotiflyer.AbsPathDocsSpotiflyer}";
            AndHUD.Shared.ShowSuccess(MainActivity.ActivityCurrent, MMessageToast, MaskType.Black, TimeSpan.FromMilliseconds(1600));

            // update ui
            ProgressRing prd = (ProgressRing)FindByName("progress_ring_dlr");
            ButtonView finishBtn = (ButtonView)FindByName("finish_btn");
            ((Image)FindByName("preview_img")).FadeTo(0.85, ANIM_LENGTH);
            prd.FadeTo(0.0, ANIM_LENGTH);
            ((ProgressRing)FindByName("progress_ring")).FadeTo(0.0, ANIM_LENGTH);
            await ((Label)FindByName("progress_label")).FadeTo(0.0, ANIM_LENGTH);

            // show finish ui
            prd.IsVisible = false;
            finishBtn.IsVisible = true;
            finishBtn.FadeTo(1.0, ANIM_LENGTH);

            // clear notification
            if (Services != null)
            {
                Services.Stop();
            }
        }

        // BROADCAST RECEIVER
        [BroadcastReceiver(Enabled = true, Exported = false)]
        public class FinishReceiver : BroadcastReceiver
        {
            readonly string Tag = nameof(FinishReceiver);
            public override void OnReceive(Context context, Intent intent)
            {
                Log.Info(Tag, "OnReceive");

                // check if shared
                if (Spotiflyer.MIsShared)
                {
                    Console.WriteLine($"{Tag} shared, finishing activity");

                    // update ui
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        MainPage mp = ((MainPage)Shell.Current.CurrentPage);

                        // update vars
                        mp.ResetVars();
                        Spotiflyer.MIsShared = false;

                        // increment successful runs
                        int runs = 1;
                        if (Preferences.Default.ContainsKey("SUCCESSFUL_RUNS"))
                        {
                            runs += Preferences.Default.Get("SUCCESSFUL_RUNS", 0);
                        }
                        Spotiflyer.successfulRuns = runs;

                        // set in prefs
                        Preferences.Default.Set("SUCCESSFUL_RUNS", runs);
                        Console.WriteLine($"{Tag} SUCCESSFUL_RUNS={runs}");

                        // show success message
                        mp.MMessageToast = $"Saved! In {Spotiflyer.AbsPathDocsSpotiflyer}";
                        AndHUD.Shared.ShowSuccess(MainActivity.ActivityCurrent, mp.MMessageToast, MaskType.Black, TimeSpan.FromMilliseconds(1600));

                        // clear views
                        await ((MainPage)Shell.Current.CurrentPage).ClearTextfield();
                        await ((MainPage)Shell.Current.CurrentPage).ShowEmptyUI();
                        await Task.Delay(400);
                        // finish activity
                        MainActivity.ActivityCurrent.FinishAfterTransition();
                    });
                } else
                {
                    Console.WriteLine($"{Tag} showing finish UI");
                    // show finish views
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        MainPage mp = ((MainPage)Shell.Current.CurrentPage);

                        mp.ShowFinishUI();
                    });
                }

                // unregister receiver
                try
                {
                    Console.WriteLine("trying to unregister finish receiver...");
                    context.UnregisterReceiver(this);
                }
                catch (Exception) {
                    Console.WriteLine("finish receiver already unregistered");
                }
            }
        }
    }
}
